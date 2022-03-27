// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween
{
    /// <summary>
    /// コールバック先の関数を <see cref="Interval"/> 未満の頻度で呼ばないよう制御するタイマー
    /// </summary>
    /// <remarks>
    /// lodash の <c>_.debounce()</c> に相当する機能となっている
    /// </remarks>
    public class DebounceTimer : IDisposable
    {
        private readonly ITimer debouncingTimer;
        private readonly Func<Task> timerCallback;
        private readonly object lockObject = new();

        private DateTimeUtc lastCall;
        private bool calledSinceLastInvoke;
        private bool refreshTimerEnabled;

        public TimeSpan Interval { get; }

        public bool InvokeLeading { get; }

        public bool InvokeTrailing { get; }

        public DebounceTimer(Func<Task> timerCallback, TimeSpan interval, bool leading, bool trailing)
        {
            this.timerCallback = timerCallback;
            this.Interval = interval;
            this.InvokeLeading = leading;
            this.InvokeTrailing = trailing;
            this.debouncingTimer = this.CreateTimer(this.Execute);
            this.lastCall = DateTimeUtc.MinValue;
            this.calledSinceLastInvoke = false;
            this.refreshTimerEnabled = false;
        }

        protected virtual ITimer CreateTimer(Func<Task> callback)
            => new AsyncTimer(callback);

        public async Task Call()
        {
            bool startTimer, invoke;
            lock (this.lockObject)
            {
                this.lastCall = DateTimeUtc.Now;
                this.calledSinceLastInvoke = true;
                if (this.refreshTimerEnabled)
                {
                    startTimer = false;
                    invoke = false;
                }
                else
                {
                    startTimer = true;
                    invoke = this.InvokeLeading;
                    this.refreshTimerEnabled = true;
                }
            }

            if (startTimer)
            {
                if (invoke)
                    await this.Invoke().ConfigureAwait(false);

                this.debouncingTimer.Change(dueTime: this.Interval, period: Timeout.InfiniteTimeSpan);
            }
        }

        private async Task Execute()
        {
            bool startTimer, invoke;
            TimeSpan wait;
            lock (this.lockObject)
            {
                var sinceLastCall = DateTimeUtc.Now - this.lastCall;

                if (sinceLastCall < TimeSpan.Zero)
                {
                    // システムの時計が過去の時刻に変更された場合は無限ループを防ぐために lastCall をリセットする
                    this.lastCall = DateTimeUtc.Now;
                    sinceLastCall = TimeSpan.Zero;
                }

                if (sinceLastCall < this.Interval)
                {
                    startTimer = true;
                    wait = this.Interval - sinceLastCall;
                    invoke = false;
                }
                else
                {
                    startTimer = false;
                    wait = TimeSpan.Zero;
                    invoke = this.InvokeTrailing && this.calledSinceLastInvoke;
                    this.refreshTimerEnabled = false;
                }
            }

            if (invoke)
                await this.Invoke().ConfigureAwait(false);

            if (startTimer)
                this.debouncingTimer.Change(dueTime: wait, period: Timeout.InfiniteTimeSpan);
        }

        private async Task Invoke()
        {
            await Task.Run(async () =>
            {
                lock (this.lockObject)
                    this.calledSinceLastInvoke = false;

                await this.timerCallback().ConfigureAwait(false);
            });
        }

        public void Dispose()
            => this.debouncingTimer.Dispose();

        public static DebounceTimer Create(Func<Task> callback, TimeSpan wait, bool leading = false, bool trailing = true)
            => new(callback, wait, leading, trailing);
    }
}
