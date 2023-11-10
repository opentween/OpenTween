// OpenTween - Client of Twitter
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween
{
    public class TaskCollectionTest
    {
        [Fact]
        public async Task RunAll_Test()
        {
            var tasks = new TaskCollection();

            var tcs1 = new TaskCompletionSource<int>();
            tasks.Add(() => tcs1.Task);

            var tcs2 = new TaskCompletionSource<int>();
            tasks.Add(() => tcs2.Task);

            var runAllTask = tasks.RunAll();

            // すべての Task が完了するまで待機する
            Assert.NotEqual(runAllTask, await Task.WhenAny(runAllTask, Task.Delay(100)));

            tcs1.SetResult(0);
            Assert.NotEqual(runAllTask, await Task.WhenAny(runAllTask, Task.Delay(100)));

            tcs2.SetResult(0);
            Assert.Equal(runAllTask, await Task.WhenAny(runAllTask, Task.Delay(100)));
        }

        [Fact]
        public async Task RunAll_EmptyTest()
        {
            var tasks = new TaskCollection();
            await tasks.RunAll();
        }

        [Fact]
        public async Task RunAll_RunOnThreadPoolEnabledTest()
        {
            var parentThreadId = Thread.CurrentThread.ManagedThreadId;
            var tasks = new TaskCollection();

            tasks.Add(() =>
            {
                var childThreadId = Thread.CurrentThread.ManagedThreadId;
                Assert.NotEqual(parentThreadId, childThreadId);
                return Task.CompletedTask;
            });

            await tasks.RunAll(runOnThreadPool: true);
        }

        [Fact]
        public async Task RunAll_RunOnThreadPoolDisabledTest()
        {
            var parentThreadId = Thread.CurrentThread.ManagedThreadId;
            var tasks = new TaskCollection();

            tasks.Add(() =>
            {
                var childThreadId = Thread.CurrentThread.ManagedThreadId;
                Assert.Equal(parentThreadId, childThreadId);
                return Task.CompletedTask;
            });

            await tasks.RunAll(runOnThreadPool: false);
        }

        [Fact]
        public async Task IgnoreException_IgnoredTest()
        {
            var tasks = new TaskCollection();
            tasks.Add(() => Task.FromException(new OperationCanceledException()));
            tasks.IgnoreException(ex => ex is OperationCanceledException);

            await tasks.RunAll();
        }

        [Fact]
        public async Task IgnoreException_NotIgnoredTest()
        {
            var tasks = new TaskCollection();
            tasks.Add(() => Task.FromException(new OperationCanceledException()));
            tasks.IgnoreException(ex => ex is IOException);

            await Assert.ThrowsAsync<OperationCanceledException>(
                () => tasks.RunAll()
            );
        }
    }
}
