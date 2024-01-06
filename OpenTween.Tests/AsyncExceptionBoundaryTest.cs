// OpenTween - Client of Twitter
// Copyright (c) 2024 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween
{
    public class AsyncExceptionBoundaryTest
    {
        [Fact]
        public async Task Wrap_SynchronousTest()
        {
            static Task AsyncFunc()
                => throw new OperationCanceledException();

            // 例外を無視して終了する
            await AsyncExceptionBoundary.Wrap(AsyncFunc);
        }

        [Fact]
        public async Task Wrap_AsynchronousTest()
        {
            static async Task AsyncFunc()
            {
                await Task.Yield();
                throw new OperationCanceledException();
            }

            // 例外を無視して終了する
            await AsyncExceptionBoundary.Wrap(AsyncFunc);
        }

        [Fact]
        public async Task Wrap_IgnoredTest()
        {
            static Task AsyncFunc()
                => throw new OperationCanceledException();

            static bool IgnoreException(Exception ex)
                => ex is OperationCanceledException;

            // 例外を無視して終了する
            await AsyncExceptionBoundary.Wrap(AsyncFunc, IgnoreException);
        }

        [Fact]
        public async Task Wrap_NotIgnoredTest()
        {
            static Task AsyncFunc()
                => throw new IOException();

            static bool IgnoreException(Exception ex)
                => ex is OperationCanceledException;

            // 例外を返して終了する
            await Assert.ThrowsAsync<IOException>(
                () => AsyncExceptionBoundary.Wrap(AsyncFunc, IgnoreException)
            );
        }

        [Fact]
        public async Task IgnoreException_Test()
        {
            var task = Task.FromException(new OperationCanceledException());

            // 例外を無視して終了する
            await AsyncExceptionBoundary.IgnoreException(task);
        }

        [Fact]
        public async Task IgnoreExceptionAndDispose_Test()
        {
            var task = Task.FromException<int>(new OperationCanceledException());

            // 例外を無視して終了する
            await AsyncExceptionBoundary.IgnoreExceptionAndDispose(task);
        }

        [Fact]
        public async Task IgnoreExceptionAndDispose_DisposeTest()
        {
            using var image = TestUtils.CreateDummyImage();
            var task = Task.FromResult<object>(image); // IDisposable であることを静的に判定できない場合も想定

            // 正常終了したとき Result が IDisposable だった場合は破棄する
            await AsyncExceptionBoundary.IgnoreExceptionAndDispose(task);
            Assert.True(image.IsDisposed);
        }

        [Fact]
        public async Task IgnoreExceptionAndDispose_IEnumerableTest()
        {
            using var image1 = TestUtils.CreateDummyImage();
            using var image2 = TestUtils.CreateDummyImage();
            var tasks = new[]
            {
                Task.FromResult<object>(image1), // IDisposable であることを静的に判定できない場合も想定
                Task.FromResult<object>(image2),
            };

            // 正常終了したとき Result が IDisposable だった場合は破棄する
            await AsyncExceptionBoundary.IgnoreExceptionAndDispose(tasks);
            Assert.True(image1.IsDisposed);
            Assert.True(image2.IsDisposed);
        }
    }
}
