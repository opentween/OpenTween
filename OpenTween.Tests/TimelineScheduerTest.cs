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
    public class TimelineScheduerTest
    {
        private class TestTimelineScheduler : TimelineScheduler
        {
            public MockTimer MockTimer = new(() => Task.CompletedTask);

            public TestTimelineScheduler()
                : base()
            {
            }

            protected override ITimer CreateTimer(Func<Task> callback)
                => this.MockTimer = new MockTimer(callback);
        }

        [Fact]
        public async Task Callback_Test()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 0, 0, 0)))
            {
                using var scheduler = new TestTimelineScheduler();
                var mockTimer = scheduler.MockTimer;

                Assert.False(mockTimer.IsTimerRunning);

                var count = 0;
                scheduler.UpdateFunc[TimelineSchedulerTaskType.Home] = () =>
                {
                    count++;
                    TestUtils.DriftTime(TimeSpan.FromSeconds(10));
                    return Task.CompletedTask;
                };
                scheduler.UpdateInterval[TimelineSchedulerTaskType.Home] = TimeSpan.FromMinutes(1);
                scheduler.Enabled = true;

                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.Zero, mockTimer.DueTime);

                // 0:00:00 - 0:00:10
                await mockTimer.Invoke();

                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromSeconds(50), mockTimer.DueTime); // UpdateFunc 内で掛かった時間を減算する
            }
        }

        [Fact]
        public async Task Callback_SystemResumeTest()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 0, 0, 0)))
            {
                using var scheduler = new TestTimelineScheduler();
                var mockTimer = scheduler.MockTimer;

                Assert.False(mockTimer.IsTimerRunning);

                var count = 0;
                scheduler.UpdateFunc[TimelineSchedulerTaskType.Home] = () =>
                {
                    count++;
                    return Task.CompletedTask;
                };
                scheduler.UpdateInterval[TimelineSchedulerTaskType.Home] = TimeSpan.FromMinutes(1);
                scheduler.UpdateAfterSystemResume = TimeSpan.FromMinutes(10);
                scheduler.Enabled = true;

                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.Zero, mockTimer.DueTime);

                // 0:00:00
                await mockTimer.Invoke();

                Assert.Equal(1, count); // invoked
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(1), mockTimer.DueTime);

                scheduler.SystemResumed();

                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(10), mockTimer.DueTime);

                // 0:10:00
                TestUtils.DriftTime(TimeSpan.FromMinutes(10));
                await mockTimer.Invoke();

                Assert.Equal(2, count); // invoked
                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(1), mockTimer.DueTime);
            }
        }

        [Fact]
        public void RefreshSchedule_Test()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 0, 0, 0)))
            {
                using var scheduler = new TestTimelineScheduler();
                var mockTimer = scheduler.MockTimer;

                scheduler.Enabled = true;
                Assert.False(mockTimer.IsTimerRunning);

                scheduler.LastUpdatedAt[TimelineSchedulerTaskType.Home] = DateTimeUtc.Now;
                scheduler.UpdateInterval[TimelineSchedulerTaskType.Home] = TimeSpan.FromMinutes(1);
                scheduler.RefreshSchedule();

                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(1), mockTimer.DueTime);
            }
        }

        [Fact]
        public void RefreshSchedule_EmptyTest()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 0, 0, 0)))
            {
                using var scheduler = new TestTimelineScheduler();
                var mockTimer = scheduler.MockTimer;

                scheduler.Enabled = true;
                Assert.False(mockTimer.IsTimerRunning);

                scheduler.RefreshSchedule();
                Assert.False(mockTimer.IsTimerRunning);
            }
        }

        [Fact]
        public void RefreshSchedule_MultipleTest()
        {
            using (TestUtils.FreezeTime(new DateTimeUtc(2022, 1, 1, 0, 0, 0)))
            {
                using var scheduler = new TestTimelineScheduler();
                var mockTimer = scheduler.MockTimer;

                scheduler.Enabled = true;
                Assert.False(mockTimer.IsTimerRunning);

                scheduler.LastUpdatedAt[TimelineSchedulerTaskType.Home] = DateTimeUtc.Now;
                scheduler.UpdateInterval[TimelineSchedulerTaskType.Home] = TimeSpan.FromMinutes(2);
                scheduler.LastUpdatedAt[TimelineSchedulerTaskType.Mention] = DateTimeUtc.Now;
                scheduler.UpdateInterval[TimelineSchedulerTaskType.Mention] = TimeSpan.FromMinutes(3);
                scheduler.RefreshSchedule();

                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(2), mockTimer.DueTime);

                scheduler.LastUpdatedAt[TimelineSchedulerTaskType.Home] = DateTimeUtc.Now;
                scheduler.UpdateInterval[TimelineSchedulerTaskType.Home] = TimeSpan.FromMinutes(2);
                scheduler.LastUpdatedAt[TimelineSchedulerTaskType.Mention] = DateTimeUtc.Now - TimeSpan.FromMinutes(2);
                scheduler.UpdateInterval[TimelineSchedulerTaskType.Mention] = TimeSpan.FromMinutes(3);
                scheduler.RefreshSchedule();

                Assert.True(mockTimer.IsTimerRunning);
                Assert.Equal(TimeSpan.FromMinutes(1), mockTimer.DueTime);
            }
        }
    }
}
