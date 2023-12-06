// OpenTween - Client of Twitter
// Copyright (c) 2017 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Moq;
using Xunit;

namespace OpenTween
{
    public class ExtensionsTest
    {
        [Theory]
        [InlineData("ja", "ja-JP", true)]
        [InlineData("ja", "ja", true)]
        [InlineData("ja-JP", "ja-JP", true)]
        [InlineData("ja-JP", "ja", false)]
        // 4 階層以上の親を持つカルチャ
        // 参照: https://msdn.microsoft.com/ja-jp/library/dd997383(v=vs.100).aspx#%E6%96%B0%E3%81%97%E3%81%84%E7%89%B9%E5%AE%9A%E3%82%AB%E3%83%AB%E3%83%81%E3%83%A3
        [InlineData("zh-Hant", "zh-TW", true)]
        [InlineData("zh-Hant", "zh-CHT", true)]
        [InlineData("zh-Hant", "zh-Hant", true)]
        [InlineData("zh-Hant", "zh", false)]
        public void Contains_Test(string thisCultureStr, string thatCultureStr, bool expected)
        {
            var thisCulture = new CultureInfo(thisCultureStr);
            var thatCulture = new CultureInfo(thatCultureStr);
            Assert.Equal(expected, thisCulture.Contains(thatCulture));
        }

        [Fact]
        public void Contains_InvariantCultureTest()
        {
            // InvariantCulture は全てのカルチャを内包する
            Assert.True(CultureInfo.InvariantCulture.Contains(new CultureInfo("ja")));
            Assert.True(CultureInfo.InvariantCulture.Contains(CultureInfo.InvariantCulture));
        }

        [Theory]
        [InlineData("aaa", new string[0], -1)]
        [InlineData("aaa", new[] { "aaa" }, 0)]
        [InlineData("bbb", new[] { "aaa" }, -1)]
        [InlineData("bbb", new[] { "aaa", "bbb" }, 1)]
        public void FindIndex_Test(string item, string[] array, int expected)
        {
            // このテストでは items が List<T> または T[] のインスタンスと認識されないようにする
            var items = new LinkedList<string>(array).AsEnumerable();
            Assert.Equal(expected, items.FindIndex(x => x == item));
        }

        [Fact]
        public void FindIndex_ListTest()
        {
            var items = new List<string> { "aaa", "bbb" }.AsEnumerable();
            Assert.Equal(1, items.FindIndex(x => x == "bbb"));
        }

        [Fact]
        public void FindIndex_ArrayTest()
        {
            var items = new[] { "aaa", "bbb" }.AsEnumerable();
            Assert.Equal(1, items.FindIndex(x => x == "bbb"));
        }

        [Theory]
        [InlineData("abc", new int[] { 'a', 'b', 'c' })]
        [InlineData("🍣", new int[] { 0x1f363 })] // サロゲートペア
        public void ToCodepoints_Test(string s, int[] expected)
            => Assert.Equal(expected, s.ToCodepoints());

        [Theory]
        // char.ConvertToUtf32 をそのまま使用するとエラーになるパターン
        [InlineData(new[] { '\ud83c' }, new[] { 0xd83c })] // 壊れたサロゲートペア (LowSurrogate が無い)
        [InlineData(new[] { '\udf63' }, new[] { 0xdf63 })] // 壊れたサロゲートペア (HighSurrogate が無い)
        public void ToCodepoints_BrokenSurrogateTest(char[] s, int[] expected)
        {
            // InlineDataAttribute で壊れたサロゲートペアの string を扱えないため char[] を使う
            Assert.Equal(expected, new string(s).ToCodepoints());
        }

        [Fact]
        public void ToCodepoints_ErrorTest()
            => Assert.Throws<ArgumentNullException>(() => ((string)null!).ToCodepoints());

        [Theory]
        [InlineData("", 0, 0, 0)]
        [InlineData("sushi 🍣", 0, 8, 7)]
        [InlineData("sushi 🍣", 0, 5, 5)]
        [InlineData("sushi 🍣", 6, 8, 1)]
        [InlineData("sushi 🍣", 6, 7, 1)] // サロゲートペアの境界を跨ぐ範囲 (LowSurrogate が無い)
        [InlineData("sushi 🍣", 7, 8, 1)] // サロゲートペアの境界を跨ぐ範囲 (HighSurrogate が無い)
        public void GetCodepointCount_Test(string str, int start, int end, int expected)
            => Assert.Equal(expected, str.GetCodepointCount(start, end));

        [Fact]
        public void GetCodepointCount_ErrorTest()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null!).GetCodepointCount(0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => "abc".GetCodepointCount(-1, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => "abc".GetCodepointCount(0, 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => "abc".GetCodepointCount(4, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => "abc".GetCodepointCount(2, 1));
        }

        [WinFormsFact]
        public async Task TryInvoke_InvokeNotRequiredTest()
        {
            var tcs = new TaskCompletionSource<int>();
            using var control = new Control();
            control.CreateControl();

            var uiThreadId = Thread.CurrentThread.ManagedThreadId;
            var ret = control.TryInvoke(() =>
            {
                Assert.Equal(uiThreadId, Thread.CurrentThread.ManagedThreadId);
                tcs.SetResult(1);
            });
            Assert.True(ret);

            await tcs.Task;
        }

        [WinFormsFact]
        public async Task TryInvoke_InvokeRequiredTest()
        {
            var tcs = new TaskCompletionSource<int>();
            using var control = new Control();
            control.CreateControl();

            var uiThreadId = Thread.CurrentThread.ManagedThreadId;
            await Task.Run(() =>
            {
                var workerThreadId = Thread.CurrentThread.ManagedThreadId;
                var ret = control.TryInvoke(() =>
                {
                    Assert.NotEqual(workerThreadId, Thread.CurrentThread.ManagedThreadId);
                    Assert.Equal(uiThreadId, Thread.CurrentThread.ManagedThreadId);
                    tcs.SetResult(1);
                });
                Assert.True(ret);
            });

            await tcs.Task;
        }

        [WinFormsFact]
        public void TryInvoke_DisposedTest()
        {
            var control = new Control();
            control.CreateControl();
            control.Dispose();

            var ret = control.TryInvoke(
                () => Assert.Fail("should not be called")
            );
            Assert.False(ret);
        }

        [Fact]
        public async Task ForEachAsync_Test()
        {
            var mock = new Mock<IObservable<int>>();
            mock.Setup(x => x.Subscribe(It.IsNotNull<IObserver<int>>()))
                .Callback<IObserver<int>>(x =>
                {
                    x.OnNext(1);
                    x.OnNext(2);
                    x.OnNext(3);
                    x.OnCompleted();
                })
                .Returns(Mock.Of<IDisposable>());

            var results = new List<int>();

            await mock.Object.ForEachAsync(x => results.Add(x));

            Assert.Equal(new[] { 1, 2, 3 }, results);
        }

        [Fact]
        public async Task ForEachAsync_EmptyTest()
        {
            var mock = new Mock<IObservable<int>>();
            mock.Setup(x => x.Subscribe(It.IsNotNull<IObserver<int>>()))
                .Callback<IObserver<int>>(x => x.OnCompleted())
                .Returns(Mock.Of<IDisposable>());

            var results = new List<int>();

            await mock.Object.ForEachAsync(x => results.Add(x));

            Assert.Empty(results);
        }

        [Fact]
        public async Task ForEachAsync_CancelledTest()
        {
            var mockUnsubscriber = new Mock<IDisposable>();

            var mockObservable = new Mock<IObservable<int>>();
            mockObservable.Setup(x => x.Subscribe(It.IsNotNull<IObserver<int>>()))
                .Callback<IObserver<int>>(x =>
                {
                    x.OnNext(1);
                    x.OnNext(2);
                    x.OnNext(3);
                    x.OnCompleted();
                })
                .Returns(mockUnsubscriber.Object);

            var cts = new CancellationTokenSource();

            await mockObservable.Object.ForEachAsync(x => cts.Cancel(), cts.Token);

            mockUnsubscriber.Verify(x => x.Dispose(), Times.AtLeastOnce());
        }

        [Fact]
        public async Task ForEachAsync_ErrorOccursedAtObservableTest()
        {
            var mockObservable = new Mock<IObservable<int>>();
            mockObservable.Setup(x => x.Subscribe(It.IsNotNull<IObserver<int>>()))
                .Callback<IObserver<int>>(x =>
                {
                    x.OnNext(1);
                    x.OnError(new Exception());
                })
                .Returns(Mock.Of<IDisposable>());

            var results = new List<int>();

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await mockObservable.Object.ForEachAsync(x => results.Add(x));
            });
            Assert.Equal(new[] { 1 }, results);
        }

        [Fact]
        public async Task ForEachAsync_ErrorOccursedAtSubscriberTest()
        {
            var mockUnsubscriber = new Mock<IDisposable>();

            var mockObservable = new Mock<IObservable<int>>();
            mockObservable.Setup(x => x.Subscribe(It.IsNotNull<IObserver<int>>()))
                .Callback<IObserver<int>>(x =>
                {
                    x.OnNext(1);
                    x.OnCompleted();
                })
                .Returns(mockUnsubscriber.Object);

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await mockObservable.Object.ForEachAsync(x => throw new Exception());
            });

            mockUnsubscriber.Verify(x => x.Dispose(), Times.AtLeastOnce());
        }
    }
}
