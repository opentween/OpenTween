// OpenTween - Client of Twitter
// Copyright (c) 2018 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
    /// lodash の <c>_.throttle()</c> に相当する機能となっている
    /// </remarks>
    public class ThrottleTimer : IDisposable
    {
        private readonly ITimer throttlingTimer;
        private readonly Func<Task> timerCallback;
        private readonly object lockObject = new();

        private bool calledSinceLastInvoke;
        private bool refreshTimerEnabled;

        public TimeSpan Interval { get; }

        public ThrottleTimer(Func<Task> timerCallback, TimeSpan interval)
        {
            this.timerCallback = timerCallback;
            this.Interval = interval;
            this.throttlingTimer = this.CreateTimer(this.Execute);
            this.calledSinceLastInvoke = false;
            this.refreshTimerEnabled = false;
        }

        protected virtual ITimer CreateTimer(Func<Task> callback)
            => new AsyncTimer(callback);

        public async Task Call()
        {
            bool startTimer;
            lock (this.lockObject)
            {
                this.calledSinceLastInvoke = true;
                if (this.refreshTimerEnabled)
                {
                    startTimer = false;
                }
                else
                {
                    startTimer = true;
                    this.refreshTimerEnabled = true;
                }
            }

            if (startTimer)
            {
                await this.Invoke().ConfigureAwait(false);
                this.throttlingTimer.Change(dueTime: this.Interval, period: Timeout.InfiniteTimeSpan);
            }
        }

        private async Task Execute()
        {
            bool invoke;
            lock (this.lockObject)
            {
                if (this.calledSinceLastInvoke)
                {
                    invoke = true;
                }
                else
                {
                    invoke = false;
                    this.refreshTimerEnabled = false;
                }
            }

            if (invoke)
            {
                await this.Invoke().ConfigureAwait(false);
                this.throttlingTimer.Change(dueTime: this.Interval, period: Timeout.InfiniteTimeSpan);
            }
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
            => this.throttlingTimer.Dispose();

        public static ThrottleTimer Create(Func<Task> callback, TimeSpan wait)
            => new(callback, wait);
    }
}
