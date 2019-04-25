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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween
{
    /// <summary>
    /// コールバック先の関数を <see cref="Interval"/> 未満の頻度で呼ばないよう制御するタイマー
    /// </summary>
    public class ThrottlingTimer : IDisposable
    {
        private const int TIMER_DISABLED = 0;
        private const int TIMER_ENABLED = 1;

        private readonly Timer throttlingTimer;
        private readonly Func<Task> timerCallback;

        private DateTimeUtc lastCalled = DateTimeUtc.MinValue;
        private DateTimeUtc lastInvoked = DateTimeUtc.MinValue;
        private int refreshTimerEnabled = 0;

        public TimeSpan Interval { get; }

        public ThrottlingTimer(Func<Task> timerCallback, TimeSpan interval)
        {
            this.timerCallback = timerCallback;
            this.Interval = interval;
            this.throttlingTimer = new Timer(this.Execute);
        }

        public void Call()
        {
            this.lastCalled = DateTimeUtc.Now;

            if (this.refreshTimerEnabled == TIMER_DISABLED)
            {
                lock (this.throttlingTimer)
                {
                    if (Interlocked.CompareExchange(ref this.refreshTimerEnabled, TIMER_ENABLED, TIMER_DISABLED) == TIMER_DISABLED)
                        this.throttlingTimer.Change(dueTime: 0, period: Timeout.Infinite);
                }
            }
        }

        private async void Execute(object _)
        {
            var timerExpired = this.lastCalled < this.lastInvoked;
            if (timerExpired)
            {
                // 前回実行時より後に lastInvoked が更新されていなければタイマーを止める
                Interlocked.CompareExchange(ref this.refreshTimerEnabled, TIMER_DISABLED, TIMER_ENABLED);
            }
            else
            {
                this.lastInvoked = DateTimeUtc.Now;
                await this.timerCallback().ConfigureAwait(false);

                // dueTime は Execute が呼ばれる度に再設定する (period は使用しない)
                // これにより timerCallback の実行に Interval 以上の時間が掛かっても重複して実行されることはなくなる
                lock (this.throttlingTimer)
                    this.throttlingTimer.Change(dueTime: (int)this.Interval.TotalMilliseconds, period: Timeout.Infinite);
            }
        }

        public void Dispose()
            => this.throttlingTimer.Dispose();

        // lodash.js の _.throttle 的な処理をしたかったメソッド
        public static ThrottlingTimer Throttle(Func<Task> callback, TimeSpan wait)
            => new ThrottlingTimer(callback, wait);
    }
}
