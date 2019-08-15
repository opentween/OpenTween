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

        private long lastCalledTick;
        private long lastInvokedTick;
        private int refreshTimerEnabled = TIMER_DISABLED;

        public TimeSpan Interval { get; }
        public TimeSpan MaxWait { get; }
        public bool InvokeLeading { get; }
        public bool InvokeTrailing { get; }

        private DateTimeUtc LastCalled
        {
            get => new DateTimeUtc(Interlocked.Read(ref this.lastCalledTick));
            set => Interlocked.Exchange(ref this.lastCalledTick, value.UtcTicks);
        }

        private DateTimeUtc LastInvoked
        {
            get => new DateTimeUtc(Interlocked.Read(ref this.lastInvokedTick));
            set => Interlocked.Exchange(ref this.lastInvokedTick, value.UtcTicks);
        }

        public ThrottlingTimer(Func<Task> timerCallback, TimeSpan interval, TimeSpan maxWait, bool leading, bool trailing)
        {
            this.timerCallback = timerCallback;
            this.Interval = interval;
            this.MaxWait = maxWait;
            this.LastCalled = DateTimeUtc.MinValue;
            this.LastInvoked = DateTimeUtc.MinValue;
            this.InvokeLeading = leading;
            this.InvokeTrailing = trailing;
            this.throttlingTimer = new Timer(this.Execute);
        }

        public void Call()
        {
            this.LastCalled = DateTimeUtc.Now;

            if (this.refreshTimerEnabled == TIMER_DISABLED)
            {
                this.refreshTimerEnabled = TIMER_ENABLED;
                this.LastInvoked = DateTimeUtc.MinValue;
                _ = Task.Run(async () =>
                {
                    if (this.InvokeLeading)
                        await this.timerCallback().ConfigureAwait(false);

                    this.throttlingTimer.Change(dueTime: this.Interval, period: Timeout.InfiniteTimeSpan);
                });
            }
        }

        private async void Execute(object _)
        {
            var lastCalled = this.LastCalled;
            var lastInvoked = this.LastInvoked;

            var timerExpired = lastCalled < lastInvoked;
            if (timerExpired)
            {
                // 前回実行時より後に lastInvoked が更新されていなければタイマーを止める
                this.refreshTimerEnabled = TIMER_DISABLED;

                if (this.InvokeTrailing)
                    await this.timerCallback().ConfigureAwait(false);
            }
            else
            {
                var now = DateTimeUtc.Now;

                if ((now - lastInvoked) >= this.MaxWait)
                    await this.timerCallback().ConfigureAwait(false);

                this.LastInvoked = now;

                // dueTime は Execute が呼ばれる度に再設定する (period は使用しない)
                // これにより timerCallback の実行に Interval 以上の時間が掛かっても重複して実行されることはなくなる
                lock (this.throttlingTimer)
                    this.throttlingTimer.Change(dueTime: this.Interval, period: Timeout.InfiniteTimeSpan);
            }
        }

        public void Dispose()
            => this.throttlingTimer.Dispose();

        // lodash.js の _.throttle, _.debounce 的な処理をしたかったメソッド群
        public static ThrottlingTimer Throttle(Func<Task> callback, TimeSpan wait, bool leading = true, bool trailing = true)
            => new ThrottlingTimer(callback, wait, maxWait: wait, leading, trailing);

        public static ThrottlingTimer Debounce(Func<Task> callback, TimeSpan wait, bool leading = false, bool trailing = true)
            => new ThrottlingTimer(callback, wait, maxWait: TimeSpan.MaxValue, leading, trailing);
    }
}
