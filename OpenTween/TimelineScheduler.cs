// OpenTween - Client of Twitter
// Copyright (c) 2019 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween
{
    public class TimelineScheduler : IDisposable
    {
        private static readonly TimelineSchedulerTaskType[] AllTaskTypes =
        {
            TimelineSchedulerTaskType.Home,
            TimelineSchedulerTaskType.Mention,
            TimelineSchedulerTaskType.Dm,
            TimelineSchedulerTaskType.PublicSearch,
            TimelineSchedulerTaskType.User,
            TimelineSchedulerTaskType.List,
            TimelineSchedulerTaskType.Config,
        };

        private readonly ITimer timer;

        private bool enabled = false;
        private bool systemResumeMode = false;
        private bool preventTimerUpdate = false;

        public bool IsDisposed { get; private set; } = false;

        public bool Enabled
        {
            get => this.enabled;
            set
            {
                if (this.enabled == value)
                    return;

                this.enabled = value;
                this.Reset();
            }
        }

        public DateTimeUtc SystemResumedAt { get; private set; } = DateTimeUtc.MinValue;
        public TimeSpan UpdateAfterSystemResume { get; set; } = Timeout.InfiniteTimeSpan;

        public bool EnableUpdateSystemResume
            => this.UpdateAfterSystemResume != Timeout.InfiniteTimeSpan;

        public Dictionary<TimelineSchedulerTaskType, DateTimeUtc> LastUpdatedAt { get; }
            = new Dictionary<TimelineSchedulerTaskType, DateTimeUtc>();

        public Dictionary<TimelineSchedulerTaskType, TimeSpan> UpdateInterval { get; }
            = new Dictionary<TimelineSchedulerTaskType, TimeSpan>();

        public Dictionary<TimelineSchedulerTaskType, Func<Task>> UpdateFunc { get; }
            = new Dictionary<TimelineSchedulerTaskType, Func<Task>>();

        public IEnumerable<TimelineSchedulerTaskType> EnabledTaskTypes
            => TimelineScheduler.AllTaskTypes.Where(x => this.IsEnabledType(x));

        public TimelineScheduler()
        {
            this.timer = this.CreateTimer(this.TimerCallback);

            foreach (var taskType in TimelineScheduler.AllTaskTypes)
            {
                this.LastUpdatedAt[taskType] = DateTimeUtc.MinValue;
                this.UpdateInterval[taskType] = Timeout.InfiniteTimeSpan;
            }
        }

        protected virtual ITimer CreateTimer(Func<Task> callback)
            => new AsyncTimer(callback);

        public bool IsEnabledType(TimelineSchedulerTaskType task)
            => this.UpdateInterval[task] != Timeout.InfiniteTimeSpan;

        public void RefreshSchedule()
        {
            if (this.preventTimerUpdate)
                return; // TimerCallback 内で更新されるのでここは単に無視してよい

            if (this.Enabled)
                this.timer.Change(this.NextTimerDelay(), Timeout.InfiniteTimeSpan);
            else
                this.timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public void SystemResumed()
        {
            if (!this.EnableUpdateSystemResume)
                return;

            this.SystemResumedAt = DateTimeUtc.Now;
            this.systemResumeMode = true;
            this.RefreshSchedule();
        }

        public void Reset()
        {
            foreach (var taskType in TimelineScheduler.AllTaskTypes)
                this.LastUpdatedAt[taskType] = DateTimeUtc.MinValue;

            this.systemResumeMode = false;
            this.RefreshSchedule();
        }

        private async Task TimerCallback()
        {
            try
            {
                this.preventTimerUpdate = true;

                if (this.systemResumeMode)
                    await this.TimerCallback_AfterSystemResume().ConfigureAwait(false);
                else
                    await this.TimerCallback_Normal().ConfigureAwait(false);
            }
            finally
            {
                this.preventTimerUpdate = false;
                this.RefreshSchedule();
            }
        }

        private async Task TimerCallback_Normal()
        {
            var now = DateTimeUtc.Now;
            var round = TimeSpan.FromSeconds(1); // 1秒未満の差異であればまとめて実行する
            var tasks = new List<TimelineSchedulerTaskType>(capacity: TimelineScheduler.AllTaskTypes.Length);

            foreach (var taskType in this.EnabledTaskTypes)
            {
                var nextScheduledAt = this.LastUpdatedAt[taskType] + this.UpdateInterval[taskType];
                if (nextScheduledAt - now < round)
                    tasks.Add(taskType);
            }

            await this.RunUpdateTasks(tasks, now).ConfigureAwait(false);
        }

        private async Task TimerCallback_AfterSystemResume()
        {
            // systemResumeMode では一定期間経過後に全てのタイムラインを更新する
            var now = DateTimeUtc.Now;

            this.systemResumeMode = false;
            await this.RunUpdateTasks(TimelineScheduler.AllTaskTypes, now).ConfigureAwait(false);
        }

        private async Task RunUpdateTasks(IEnumerable<TimelineSchedulerTaskType> taskTypes, DateTimeUtc now)
        {
            var updateTasks = new List<Func<Task>>(capacity: TimelineScheduler.AllTaskTypes.Length);

            foreach (var taskType in taskTypes)
            {
                this.LastUpdatedAt[taskType] = now;
                if (this.UpdateFunc.TryGetValue(taskType, out var func))
                    updateTasks.Add(func);
            }

            await Task.WhenAll(updateTasks.Select(x => Task.Run(x)))
                .ConfigureAwait(false);
        }

        private TimeSpan NextTimerDelay()
        {
            TimeSpan delay;

            if (this.systemResumeMode)
            {
                // systemResumeMode が有効な間は UpdateAfterSystemResume 以外の設定値を見ない
                var nextScheduledUpdateAll = this.SystemResumedAt + this.UpdateAfterSystemResume;
                delay = nextScheduledUpdateAll - DateTimeUtc.Now;
            }
            else
            {
                // 次に更新が予定される時刻を判定する
                var min = DateTimeUtc.MaxValue;

                foreach (var taskType in this.EnabledTaskTypes)
                {
                    var nextScheduledAt = this.LastUpdatedAt[taskType] + this.UpdateInterval[taskType];
                    if (nextScheduledAt < min)
                        min = nextScheduledAt;
                }

                if (min == DateTimeUtc.MaxValue)
                    return Timeout.InfiniteTimeSpan;

                delay = min - DateTimeUtc.Now;
            }

            return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed)
                return;

            if (disposing)
                this.timer.Dispose();

            this.IsDisposed = true;
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public enum TimelineSchedulerTaskType
    {
        Home,
        Mention,
        Dm,
        PublicSearch,
        User,
        List,
        Config,
    }
}
