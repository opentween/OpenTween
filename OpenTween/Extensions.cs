// OpenTween - Client of Twitter
// Copyright (c) 2015 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTween
{
    internal static class Extensions
    {
        /// <summary>
        /// WebBrowserで選択中のテキストを取得します
        /// </summary>
        public static string GetSelectedText(this WebBrowser webBrowser)
        {
            dynamic document = webBrowser.Document.DomDocument;
            dynamic textRange = document.selection.createRange();
            string selectedText = textRange.text;

            return selectedText;
        }

        public static ReadLockTransaction BeginReadTransaction(this ReaderWriterLockSlim lockObj)
            => new(lockObj);

        public static WriteLockTransaction BeginWriteTransaction(this ReaderWriterLockSlim lockObj)
            => new(lockObj);

        public static UpgradeableReadLockTransaction BeginUpgradeableReadTransaction(this ReaderWriterLockSlim lockObj)
            => new(lockObj);

        /// <summary>
        /// 一方のカルチャがもう一方のカルチャを内包するかを判断します
        /// </summary>
        public static bool Contains(this CultureInfo @this, CultureInfo that)
        {
            if (@this.Equals(that))
                return true;

            // InvariantCulture の親カルチャは InvariantCulture 自身であるため、false になったら打ち切る
            if (!that.Parent.Equals(that))
                return Contains(@this, that.Parent);

            return false;
        }

        public static int FindIndex<T>(this IEnumerable<T> enumerable, Predicate<T> finder)
        {
            if (enumerable is List<T> list)
                return list.FindIndex(finder);

            if (enumerable is T[] array)
                return Array.FindIndex(array, finder);

            var index = 0;

            foreach (var item in enumerable)
            {
                if (finder(item))
                    return index;

                index++;
            }

            return -1;
        }

        public static IEnumerable<(T Value, int Index)> WithIndex<T>(this IEnumerable<T> enumerable)
        {
            var i = 0;
            foreach (var value in enumerable)
                yield return (value, i++);
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }

        /// <summary>
        /// 文字列をコードポイント単位に分割して返します
        /// </summary>
        public static IEnumerable<int> ToCodepoints(this string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            static IEnumerable<int> GetEnumerable(string input)
            {
                var i = 0;
                var length = input.Length;
                while (i < length)
                {
                    if (char.IsSurrogatePair(input, i))
                    {
                        yield return char.ConvertToUtf32(input, i);
                        i += 2;
                    }
                    else
                    {
                        yield return input[i];
                        i++;
                    }
                }
            }

            return GetEnumerable(s);
        }

        /// <summary>
        /// 指定された部分文字列のコードポイント単位での文字数を返す
        /// </summary>
        /// <param name="s">文字列</param>
        /// <param name="start">開始位置</param>
        /// <param name="end">終了位置</param>
        public static int GetCodepointCount(this string s, int start, int end)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (start < 0 || start > s.Length)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (end < 0 || end > s.Length)
                throw new ArgumentOutOfRangeException(nameof(end));
            if (start > end)
                throw new ArgumentOutOfRangeException(nameof(start));

            var count = 0;
            for (var i = start; i < end; i += char.IsSurrogatePair(s, i) ? 2 : 1)
                count++;

            return count;
        }

        public static Task ForEachAsync<T>(this IObservable<T> observable, Action<T> subscriber)
        {
            return ForEachAsync(observable, value =>
            {
                subscriber(value);
                return Task.CompletedTask;
            });
        }

        public static Task ForEachAsync<T>(this IObservable<T> observable, Func<T, Task> subscriber)
            => ForEachAsync(observable, subscriber, CancellationToken.None);

        public static Task ForEachAsync<T>(this IObservable<T> observable, Action<T> subscriber, CancellationToken cancellationToken)
        {
            return ForEachAsync(
                observable,
                value =>
                {
                    subscriber(value);
                    return Task.CompletedTask;
                },
                cancellationToken
            );
        }

        public static async Task ForEachAsync<T>(this IObservable<T> observable, Func<T, Task> subscriber, CancellationToken cancellationToken)
        {
            var observer = new ForEachObserver<T>(subscriber);
            using var unsubscriber = observable.Subscribe(observer);

            using (cancellationToken.Register(() => unsubscriber.Dispose()))
                await observer.Task.ConfigureAwait(false);
        }

        private class ForEachObserver<T> : IObserver<T>
        {
            private readonly Func<T, Task> subscriber;
            private readonly TaskCompletionSource<int> tcs = new();

            public Task Task
                => this.tcs.Task;

            public ForEachObserver(Func<T, Task> subscriber)
                => this.subscriber = subscriber;

            public async void OnNext(T value)
            {
                try
                {
                    await this.subscriber(value);
                }
                catch (Exception ex)
                {
                    this.tcs.TrySetException(ex);
                }
            }

            public void OnCompleted()
                => this.tcs.TrySetResult(1);

            public void OnError(Exception error)
                => this.tcs.TrySetException(error);
        }
    }
}
