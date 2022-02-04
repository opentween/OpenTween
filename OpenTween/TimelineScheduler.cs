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
    public class TimelineScheduler
    {
        private readonly AsyncTimer timer;

        private bool enabled = false;
        private bool systemResumeMode = false;
        private bool preventTimerUpdate = false;

        public bool Enabled
        {
            get => this.enabled;
            set
            {
                if (this.enabled == value)
                    return;

                if (value)
                {
                    var now = DateTimeUtc.Now;
                    this.LastUpdateHome = now;
                    this.LastUpdateMention = now;
                    this.LastUpdateDm = now;
                    this.LastUpdatePublicSearch = now;
                    this.LastUpdateUser = now;
                    this.LastUpdateList = now;
                    this.LastUpdateConfig = now;
                }
                this.enabled = value;
                this.RefreshSchedule();
            }
        }

        public DateTimeUtc LastUpdateHome { get; private set; } = DateTimeUtc.MinValue;
        public DateTimeUtc LastUpdateMention { get; private set; } = DateTimeUtc.MinValue;
        public DateTimeUtc LastUpdateDm { get; private set; } = DateTimeUtc.MinValue;
        public DateTimeUtc LastUpdatePublicSearch { get; private set; } = DateTimeUtc.MinValue;
        public DateTimeUtc LastUpdateUser { get; private set; } = DateTimeUtc.MinValue;
        public DateTimeUtc LastUpdateList { get; private set; } = DateTimeUtc.MinValue;
        public DateTimeUtc LastUpdateConfig { get; private set; } = DateTimeUtc.MinValue;
        public DateTimeUtc SystemResumedAt { get; private set; } = DateTimeUtc.MinValue;

        public TimeSpan UpdateIntervalHome { get; set; } = Timeout.InfiniteTimeSpan;
        public TimeSpan UpdateIntervalMention { get; set; } = Timeout.InfiniteTimeSpan;
        public TimeSpan UpdateIntervalDm { get; set; } = Timeout.InfiniteTimeSpan;
        public TimeSpan UpdateIntervalPublicSearch { get; set; } = Timeout.InfiniteTimeSpan;
        public TimeSpan UpdateIntervalUser { get; set; } = Timeout.InfiniteTimeSpan;
        public TimeSpan UpdateIntervalList { get; set; } = Timeout.InfiniteTimeSpan;
        public TimeSpan UpdateIntervalConfig { get; set; } = Timeout.InfiniteTimeSpan;
        public TimeSpan UpdateAfterSystemResume { get; set; } = Timeout.InfiniteTimeSpan;

        public bool EnableUpdateHome
            => this.UpdateIntervalHome != Timeout.InfiniteTimeSpan;

        public bool EnableUpdateMention
            => this.UpdateIntervalMention != Timeout.InfiniteTimeSpan;

        public bool EnableUpdateDm
            => this.UpdateIntervalDm != Timeout.InfiniteTimeSpan;

        public bool EnableUpdatePublicSearch
            => this.UpdateIntervalPublicSearch != Timeout.InfiniteTimeSpan;

        public bool EnableUpdateUser
            => this.UpdateIntervalUser != Timeout.InfiniteTimeSpan;

        public bool EnableUpdateList
            => this.UpdateIntervalList != Timeout.InfiniteTimeSpan;

        public bool EnableUpdateConfig
            => this.UpdateIntervalConfig != Timeout.InfiniteTimeSpan;

        public bool EnableUpdateSystemResume
            => this.UpdateAfterSystemResume != Timeout.InfiniteTimeSpan;

        public Func<Task>? UpdateHome;
        public Func<Task>? UpdateMention;
        public Func<Task>? UpdateDm;
        public Func<Task>? UpdatePublicSearch;
        public Func<Task>? UpdateUser;
        public Func<Task>? UpdateList;
        public Func<Task>? UpdateConfig;

        [Flags]
        private enum UpdateTask
        {
            None = 0,
            Home = 1,
            Mention = 1 << 2,
            Dm = 1 << 3,
            PublicSearch = 1 << 4,
            User = 1 << 5,
            List = 1 << 6,
            Config = 1 << 7,
            All = Home | Mention | Dm | PublicSearch | User | List | Config,
        }

        public TimelineScheduler()
            => this.timer = new AsyncTimer(this.TimerCallback);

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
            var tasks = UpdateTask.None;

            if (this.EnableUpdateHome)
            {
                var nextScheduledHome = this.LastUpdateHome + this.UpdateIntervalHome;
                if (nextScheduledHome - now < round)
                    tasks |= UpdateTask.Home;
            }

            if (this.EnableUpdateMention)
            {
                var nextScheduledMention = this.LastUpdateMention + this.UpdateIntervalMention;
                if (nextScheduledMention - now < round)
                    tasks |= UpdateTask.Mention;
            }

            if (this.EnableUpdateDm)
            {
                var nextScheduledDm = this.LastUpdateDm + this.UpdateIntervalDm;
                if (nextScheduledDm - now < round)
                    tasks |= UpdateTask.Dm;
            }

            if (this.EnableUpdatePublicSearch)
            {
                var nextScheduledPublicSearch = this.LastUpdatePublicSearch + this.UpdateIntervalPublicSearch;
                if (nextScheduledPublicSearch - now < round)
                    tasks |= UpdateTask.PublicSearch;
            }

            if (this.EnableUpdateUser)
            {
                var nextScheduledUser = this.LastUpdateUser + this.UpdateIntervalUser;
                if (nextScheduledUser - now < round)
                    tasks |= UpdateTask.User;
            }

            if (this.EnableUpdateList)
            {
                var nextScheduledList = this.LastUpdateList + this.UpdateIntervalList;
                if (nextScheduledList - now < round)
                    tasks |= UpdateTask.List;
            }

            if (this.EnableUpdateConfig)
            {
                var nextScheduledConfig = this.LastUpdateConfig + this.UpdateIntervalConfig;
                if (nextScheduledConfig - now < round)
                    tasks |= UpdateTask.Config;
            }

            await this.RunUpdateTasks(tasks, now).ConfigureAwait(false);
        }

        private async Task TimerCallback_AfterSystemResume()
        {
            // systemResumeMode では一定期間経過後に全てのタイムラインを更新する
            var now = DateTimeUtc.Now;

            this.systemResumeMode = false;
            await this.RunUpdateTasks(UpdateTask.All, now).ConfigureAwait(false);
        }

        private async Task RunUpdateTasks(UpdateTask tasks, DateTimeUtc now)
        {
            var updateTasks = new List<Func<Task>>(capacity: 7);

            if ((tasks & UpdateTask.Home) == UpdateTask.Home)
            {
                this.LastUpdateHome = now;
                if (this.UpdateHome != null)
                    updateTasks.Add(this.UpdateHome);
            }

            if ((tasks & UpdateTask.Mention) == UpdateTask.Mention)
            {
                this.LastUpdateMention = now;
                if (this.UpdateMention != null)
                    updateTasks.Add(this.UpdateMention);
            }

            if ((tasks & UpdateTask.Dm) == UpdateTask.Dm)
            {
                this.LastUpdateDm = now;
                if (this.UpdateDm != null)
                    updateTasks.Add(this.UpdateDm);
            }

            if ((tasks & UpdateTask.PublicSearch) == UpdateTask.PublicSearch)
            {
                this.LastUpdatePublicSearch = now;
                if (this.UpdatePublicSearch != null)
                    updateTasks.Add(this.UpdatePublicSearch);
            }

            if ((tasks & UpdateTask.User) == UpdateTask.User)
            {
                this.LastUpdateUser = now;
                if (this.UpdateUser != null)
                    updateTasks.Add(this.UpdateUser);
            }

            if ((tasks & UpdateTask.List) == UpdateTask.List)
            {
                this.LastUpdateList = now;
                if (this.UpdateList != null)
                    updateTasks.Add(this.UpdateList);
            }

            if ((tasks & UpdateTask.Config) == UpdateTask.Config)
            {
                this.LastUpdateConfig = now;
                if (this.UpdateConfig != null)
                    updateTasks.Add(this.UpdateConfig);
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

                return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
            }

            // 次に更新が予定される時刻を判定する
            var min = DateTimeUtc.MaxValue;

            if (this.EnableUpdateHome)
            {
                var nextScheduledHome = this.LastUpdateHome + this.UpdateIntervalHome;
                if (nextScheduledHome < min)
                    min = nextScheduledHome;
            }

            if (this.EnableUpdateMention)
            {
                var nextScheduledMention = this.LastUpdateMention + this.UpdateIntervalMention;
                if (nextScheduledMention < min)
                    min = nextScheduledMention;
            }

            if (this.EnableUpdateDm)
            {
                var nextScheduledDm = this.LastUpdateDm + this.UpdateIntervalDm;
                if (nextScheduledDm < min)
                    min = nextScheduledDm;
            }

            if (this.EnableUpdatePublicSearch)
            {
                var nextScheduledPublicSearch = this.LastUpdatePublicSearch + this.UpdateIntervalPublicSearch;
                if (nextScheduledPublicSearch < min)
                    min = nextScheduledPublicSearch;
            }

            if (this.EnableUpdateUser)
            {
                var nextScheduledUser = this.LastUpdateUser + this.UpdateIntervalUser;
                if (nextScheduledUser < min)
                    min = nextScheduledUser;
            }

            if (this.EnableUpdateList)
            {
                var nextScheduledList = this.LastUpdateList + this.UpdateIntervalList;
                if (nextScheduledList < min)
                    min = nextScheduledList;
            }

            if (this.EnableUpdateConfig)
            {
                var nextScheduledConfig = this.LastUpdateConfig + this.UpdateIntervalConfig;
                if (nextScheduledConfig < min)
                    min = nextScheduledConfig;
            }

            delay = min - DateTimeUtc.Now;

            return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
        }
    }
}
