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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTween
{
    public class TaskCollection
    {
        private readonly List<Func<Task>> taskFuncs;
        private Func<Exception, bool> ignoreExceptionFunc = _ => true;

        public TaskCollection()
            => this.taskFuncs = new();

        public TaskCollection(int capacity)
            => this.taskFuncs = new(capacity);

        public void Add(Func<Task> func)
            => this.taskFuncs.Add(func);

        public void Add(IEnumerable<Func<Task>> tasks)
            => this.taskFuncs.AddRange(tasks);

        public TaskCollection IgnoreException(Func<Exception, bool> condition)
        {
            this.ignoreExceptionFunc = condition;
            return this;
        }

        public Task RunAll()
            => this.RunAll(runOnThreadPool: false);

        public Task RunAll(bool runOnThreadPool)
            => Task.WhenAll(this.taskFuncs.Select(x => this.WrapAsyncFunc(x, runOnThreadPool)));

        private Task WrapAsyncFunc(Func<Task> func, bool runOnThreadPool)
        {
            Task WrappedFunc()
                => AsyncExceptionBoundary.Wrap(func, this.ignoreExceptionFunc);

            if (runOnThreadPool)
                return Task.Run(WrappedFunc);
            else
                return WrappedFunc();
        }
    }
}
