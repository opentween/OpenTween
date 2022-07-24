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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween
{
    public class DebounceTimerTest
    {
        private class TestDebounceTimer : DebounceTimer
        {
            public MockTimer MockTimer = new(() => Task.CompletedTask);

            public TestDebounceTimer(Func<Task> timerCallback, TimeSpan interval, bool leading, bool trailing)
                : base(timerCallback, interval, leading, trailing)
            {
            }

            protected override ITimer CreateTimer(Func<Task> callback)
                => this.MockTimer = new MockTimer(callback);
        }

        [Fact]
        public async Task Callback_Debounce_Trailing_Test()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 0, 0, 0)))
            {
                var count = 0;

                Task Callback()
                {
                    count++;
                    TestUtils.DriftTime(TimeSpan.FromSeconds(10));
                    return Task.CompletedTask;
                }

                var interval = TimeSpan.FromMinutes(2);
                var maxWait = TimeSpan.MaxValue;
                using var debouncing = new TestDebounceTimer(Callback, interval, leading: false, trailing: true);
                var mockTimer = debouncing.MockTimer;

                Assert.Equal(0, count);
                Assert.False(mockTimer.IsTimerRunning);

                // 0:00:00 - 0:00:00
                await debouncing.Call();

                // call
                Assert.Equal(0, count);
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(2), mockTimer.DueTime);

                // 0:01:00 - 0:01:00
                TestUtils.DriftTime(TimeSpan.FromMinutes(1));
                await debouncing.Call();

                // call (throttled)
                Assert.Equal(0, count);
                Assert.True(mockTimer.IsTimerRunning);

                // 0:02:00 - 0:02:00
                TestUtils.DriftTime(TimeSpan.FromMinutes(1));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(0, count);
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(1), mockTimer.DueTime);

                // 0:03:00 - 0:03:10
                TestUtils.DriftTime(TimeSpan.FromMinutes(2));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(1, count); // invoked (trailing)
                Assert.False(mockTimer.IsTimerRunning);
            }
        }

        [Fact]
        public async Task Callback_Debounce_Trailing_CallOnceTest()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 0, 0, 0)))
            {
                var count = 0;

                Task Callback()
                {
                    count++;
                    TestUtils.DriftTime(TimeSpan.FromSeconds(10));
                    return Task.CompletedTask;
                }

                var interval = TimeSpan.FromMinutes(2);
                var maxWait = TimeSpan.MaxValue;
                using var debouncing = new TestDebounceTimer(Callback, interval, leading: false, trailing: true);
                var mockTimer = debouncing.MockTimer;

                Assert.Equal(0, count);
                Assert.False(mockTimer.IsTimerRunning);

                // 0:00:00 - 0:00:00
                await debouncing.Call();

                // call
                Assert.Equal(0, count);
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(2), mockTimer.DueTime);

                // 0:02:00 - 0:02:10
                TestUtils.DriftTime(TimeSpan.FromMinutes(2));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(1, count); // invoked (trailing)
                Assert.False(mockTimer.IsTimerRunning);
            }
        }

        [Fact]
        public async Task Callback_Debounce_Trailing_ResumeTest()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 0, 0, 0)))
            {
                var count = 0;

                Task Callback()
                {
                    count++;
                    TestUtils.DriftTime(TimeSpan.FromSeconds(10));
                    return Task.CompletedTask;
                }

                var interval = TimeSpan.FromMinutes(2);
                var maxWait = TimeSpan.MaxValue;
                using var debouncing = new TestDebounceTimer(Callback, interval, leading: false, trailing: true);
                var mockTimer = debouncing.MockTimer;

                Assert.Equal(0, count);
                Assert.False(mockTimer.IsTimerRunning);

                // 0:00:00 - 0:00:00
                await debouncing.Call();

                // call
                Assert.Equal(0, count);
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(2), mockTimer.DueTime);

                // 0:02:00 - 0:02:10
                TestUtils.DriftTime(TimeSpan.FromMinutes(2));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(1, count); // invoked (trailing)
                Assert.False(mockTimer.IsTimerRunning);

                // 0:02:10 - 0:02:10
                await debouncing.Call();

                // call
                Assert.Equal(1, count);
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(2), mockTimer.DueTime);

                // 0:04:10 - 0:04:20
                TestUtils.DriftTime(TimeSpan.FromMinutes(2));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(2, count); // invoked (trailing)
                Assert.False(mockTimer.IsTimerRunning);
            }
        }

        [Fact]
        public async Task Callback_Debounce_LeadingAndTrailing_Test()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 0, 0, 0)))
            {
                var count = 0;

                Task Callback()
                {
                    count++;
                    TestUtils.DriftTime(TimeSpan.FromSeconds(10));
                    return Task.CompletedTask;
                }

                var interval = TimeSpan.FromMinutes(2);
                var maxWait = TimeSpan.MaxValue;
                using var debouncing = new TestDebounceTimer(Callback, interval, leading: true, trailing: true);
                var mockTimer = debouncing.MockTimer;

                Assert.Equal(0, count);
                Assert.False(mockTimer.IsTimerRunning);

                // 0:00:00 - 0:00:10
                await debouncing.Call();

                // call
                Assert.Equal(1, count); // invoked (leading)
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(2), mockTimer.DueTime);

                // 0:01:10 - 0:01:10
                TestUtils.DriftTime(TimeSpan.FromMinutes(1));
                await debouncing.Call();

                // call (throttled)
                Assert.Equal(1, count);
                Assert.True(mockTimer.IsTimerRunning);

                // 0:02:10 - 0:02:10
                TestUtils.DriftTime(TimeSpan.FromMinutes(1));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(1, count);
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(1), mockTimer.DueTime);

                // 0:03:10 - 0:03:20
                TestUtils.DriftTime(TimeSpan.FromMinutes(1));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(2, count); // invoked (trailing)
                Assert.False(mockTimer.IsTimerRunning);
            }
        }

        [Fact]
        public async Task Callback_Debounce_LeadingAndTrailing_CallOnceTest()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 0, 0, 0)))
            {
                var count = 0;

                Task Callback()
                {
                    count++;
                    TestUtils.DriftTime(TimeSpan.FromSeconds(10));
                    return Task.CompletedTask;
                }

                var interval = TimeSpan.FromMinutes(2);
                var maxWait = TimeSpan.MaxValue;
                using var debouncing = new TestDebounceTimer(Callback, interval, leading: true, trailing: true);
                var mockTimer = debouncing.MockTimer;

                Assert.Equal(0, count);
                Assert.False(mockTimer.IsTimerRunning);

                // 0:00:00 - 0:00:10
                await debouncing.Call();

                // call
                Assert.Equal(1, count); // invoked (leading)
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(2), mockTimer.DueTime);

                // 0:02:10 - 0:02:10
                TestUtils.DriftTime(TimeSpan.FromMinutes(2));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(1, count); // skip trailing
                Assert.False(mockTimer.IsTimerRunning);
            }
        }

        [Fact]
        public async Task Callback_Debounce_LeadingAndTrailing_ResumeTest()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 0, 0, 0)))
            {
                var count = 0;

                Task Callback()
                {
                    count++;
                    TestUtils.DriftTime(TimeSpan.FromSeconds(10));
                    return Task.CompletedTask;
                }

                var interval = TimeSpan.FromMinutes(2);
                var maxWait = TimeSpan.MaxValue;
                using var debouncing = new TestDebounceTimer(Callback, interval, leading: true, trailing: true);
                var mockTimer = debouncing.MockTimer;

                Assert.Equal(0, count);
                Assert.False(mockTimer.IsTimerRunning);

                // 0:00:00 - 0:00:10
                await debouncing.Call();

                // call
                Assert.Equal(1, count); // invoked (leading)
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(2), mockTimer.DueTime);

                // 0:01:10 - 0:01:10
                TestUtils.DriftTime(TimeSpan.FromMinutes(1));
                await debouncing.Call();

                // call (throttled)
                Assert.Equal(1, count);
                Assert.True(mockTimer.IsTimerRunning);

                // 0:02:10 - 0:02:10
                TestUtils.DriftTime(TimeSpan.FromMinutes(1));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(1, count);
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(1), mockTimer.DueTime);

                // 0:03:10 - 0:03:20
                TestUtils.DriftTime(TimeSpan.FromMinutes(1));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(2, count); // invoked (trailing)
                Assert.False(mockTimer.IsTimerRunning);

                // 0:03:20 - 0:03:30
                await debouncing.Call();

                // call
                Assert.Equal(3, count); // invoked (leading)
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(2), mockTimer.DueTime);

                // 0:05:30 - 0:05:30
                TestUtils.DriftTime(TimeSpan.FromMinutes(2));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(3, count); // skip trailing
                Assert.False(mockTimer.IsTimerRunning);
            }
        }

        [Fact]
        public async Task Callback_Debounce_SystemClockChangedTest()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 1, 0, 0)))
            {
                var count = 0;

                Task Callback()
                {
                    count++;
                    TestUtils.DriftTime(TimeSpan.FromSeconds(10));
                    return Task.CompletedTask;
                }

                var interval = TimeSpan.FromMinutes(2);
                var maxWait = TimeSpan.MaxValue;
                using var debouncing = new TestDebounceTimer(Callback, interval, leading: false, trailing: true);
                var mockTimer = debouncing.MockTimer;

                Assert.Equal(0, count);
                Assert.False(mockTimer.IsTimerRunning);

                // 1:00:00 - 1:00:00
                await debouncing.Call();

                // call
                Assert.Equal(0, count);
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(2), mockTimer.DueTime);

                // 0:00:00
                // システムの時刻が変更され1時間前に戻った場合
                TestUtils.DriftTime(TimeSpan.FromHours(-1));

                // 0:02:00 - 0:02:00
                TestUtils.DriftTime(TimeSpan.FromMinutes(2));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(0, count);
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(2), mockTimer.DueTime);

                // 0:04:00 - 0:04:10
                TestUtils.DriftTime(TimeSpan.FromMinutes(2));
                await mockTimer.Invoke();

                // timer
                Assert.Equal(1, count);
                Assert.False(mockTimer.IsTimerRunning);
            }
        }
    }
}
