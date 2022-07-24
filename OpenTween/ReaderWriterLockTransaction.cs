// OpenTween - Client of Twitter
// Copyright (c) 2016 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
#pragma warning disable SA1649

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween
{
    public sealed class ReadLockTransaction : IDisposable
    {
        private readonly ReaderWriterLockSlim lockObj;

        public ReadLockTransaction(ReaderWriterLockSlim lockObj)
        {
            this.lockObj = lockObj ?? throw new ArgumentNullException(nameof(lockObj));
            this.lockObj.EnterReadLock();
        }

        public void Dispose()
            => this.lockObj.ExitReadLock();
    }

    public sealed class WriteLockTransaction : IDisposable
    {
        private readonly ReaderWriterLockSlim lockObj;

        public WriteLockTransaction(ReaderWriterLockSlim lockObj)
        {
            this.lockObj = lockObj ?? throw new ArgumentNullException(nameof(lockObj));
            this.lockObj.EnterWriteLock();
        }

        public void Dispose()
            => this.lockObj.ExitWriteLock();
    }

    public sealed class UpgradeableReadLockTransaction : IDisposable
    {
        private readonly ReaderWriterLockSlim lockObj;

        public UpgradeableReadLockTransaction(ReaderWriterLockSlim lockObj)
        {
            this.lockObj = lockObj ?? throw new ArgumentNullException(nameof(lockObj));
            this.lockObj.EnterUpgradeableReadLock();
        }

        public WriteLockTransaction UpgradeToWriteLock()
            => new(this.lockObj);

        public void Dispose()
            => this.lockObj.ExitUpgradeableReadLock();
    }
}
