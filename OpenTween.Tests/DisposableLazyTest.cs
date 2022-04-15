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
using Moq;
using Xunit;

namespace OpenTween
{
    public class DisposableLazyTest
    {
        [Fact]
        public void Value_Test()
        {
            var mock = new Mock<IDisposable>();
            var obj = mock.Object;

            var lazy = new DisposableLazy<IDisposable>(() => obj);
            Assert.False(lazy.IsValueCreated);
            Assert.Equal(obj, lazy.Value);
            Assert.True(lazy.IsValueCreated);
        }

        [Fact]
        public void Value_DisposedErrorTest()
        {
            var mock = new Mock<IDisposable>();
            var obj = mock.Object;

            var lazy = new DisposableLazy<IDisposable>(() => obj);
            lazy.Dispose();

            Assert.Throws<ObjectDisposedException>(() => lazy.Value);
        }

        [Fact]
        public void Dispose_BeforeValueCreatedTest()
        {
            var mock = new Mock<IDisposable>();

            var lazy = new DisposableLazy<IDisposable>(() => mock.Object);
            lazy.Dispose();

            Assert.False(lazy.IsValueCreated);
            Assert.True(lazy.IsDisposed);
            mock.Verify(x => x.Dispose(), Times.Never());
        }

        [Fact]
        public void Dispose_AfterValueCreatedTest()
        {
            var mock = new Mock<IDisposable>();

            var lazy = new DisposableLazy<IDisposable>(() => mock.Object);
            _ = lazy.Value;
            lazy.Dispose();

            Assert.True(lazy.IsValueCreated);
            Assert.True(lazy.IsDisposed);
            mock.Verify(x => x.Dispose(), Times.Once());
        }
    }
}
