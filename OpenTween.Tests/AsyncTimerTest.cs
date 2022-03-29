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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween
{
    public class AsyncTimerTest
    {
        [Fact]
        public async Task Change_FiringTest()
        {
            var tcs = new TaskCompletionSource<bool>();
            using var timer = new AsyncTimer(() =>
            {
                tcs.SetResult(true);
                return Task.CompletedTask;
            });

            var dueTime = TimeSpan.FromMilliseconds(15);
            var period = TimeSpan.FromMilliseconds(100);
            timer.Change(dueTime, period);

            var timeout = Task.Delay(1000);
            Assert.NotEqual(timeout, await Task.WhenAny(tcs.Task, timeout));
        }

        [Fact]
        public void Change_PropertiesTest()
        {
            using var timer = new AsyncTimer(() => Task.CompletedTask);

            Assert.Equal(Timeout.InfiniteTimeSpan, timer.DueTime);
            Assert.Equal(Timeout.InfiniteTimeSpan, timer.Period);

            var dueTime = TimeSpan.FromMilliseconds(30);
            var period = TimeSpan.FromMilliseconds(60);
            timer.Change(dueTime, period);

            Assert.Equal(dueTime, timer.DueTime);
            Assert.Equal(period, timer.Period);
        }

        [Fact]
        public async Task UnhandledException_Test()
        {
            var tcs = new TaskCompletionSource<Exception>();

            void Handler(object sender, ThreadExceptionEventArgs ev)
                => tcs.TrySetResult(ev.Exception);

            try
            {
                AsyncTimer.UnhandledException += Handler;

                using var timer = new AsyncTimer(() =>
                {
                    throw new ApplicationException();
                });
                timer.Change(TimeSpan.FromMilliseconds(15), Timeout.InfiniteTimeSpan);

                var timeout = Task.Delay(1000);
                Assert.NotEqual(timeout, await Task.WhenAny(tcs.Task, timeout));

                Assert.IsType<ApplicationException>(tcs.Task.Result);
            }
            finally
            {
                AsyncTimer.UnhandledException -= Handler;
            }
        }
    }
}
