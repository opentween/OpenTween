// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace OpenTween
{
    /// <summary>
    /// LRU によるキャッシュを行うための辞書クラス
    /// </summary>
    public class LRUCacheDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// 保持するアイテムの個数
        /// </summary>
        /// <remarks>
        /// TrimLimit を上回る個数のアイテムが格納されている場合は Trim メソッド実行時に除去される。
        /// </remarks>
        public int TrimLimit { get; set; }

        /// <summary>
        /// 自動で Trim メソッドを実行する、参照回数の閾値
        /// </summary>
        /// <remarks>
        /// AutoTrimCount で指定された回数だけアイテムが参照される度に Trim メソッドが実行される。
        /// </remarks>
        public int AutoTrimCount { get; set; }

        public class CacheRemovedEventArgs : EventArgs
        {
            public KeyValuePair<TKey, TValue> Item { get; }

            public CacheRemovedEventArgs(KeyValuePair<TKey, TValue> item)
                => this.Item = item;
        }

        public event EventHandler<CacheRemovedEventArgs>? CacheRemoved;

        internal LinkedList<KeyValuePair<TKey, TValue>> InnerList;
        internal Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> InnerDict;

        internal int AccessCount = 0;

        public LRUCacheDictionary()
            : this(trimLimit: int.MaxValue, autoTrimCount: int.MaxValue)
        {
        }

        public LRUCacheDictionary(int trimLimit, int autoTrimCount)
        {
            this.TrimLimit = trimLimit;
            this.AutoTrimCount = autoTrimCount;

            this.InnerList = new LinkedList<KeyValuePair<TKey, TValue>>();
            this.InnerDict = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
        }

        /// <summary>
        /// innerList の並び順を最近アクセスがあった順に維持するために、
        /// 辞書内のアイテムが参照する度に呼び出されるメソッド。
        /// </summary>
        protected void UpdateAccess(LinkedListNode<KeyValuePair<TKey, TValue>> node)
        {
            this.InnerList.Remove(node);
            this.InnerList.AddFirst(node);
        }

        public bool Trim()
        {
            if (this.Count <= this.TrimLimit) return false;

            for (var i = this.Count; i > this.TrimLimit; i--)
            {
                var node = this.InnerList.Last;
                this.InnerList.Remove(node);
                this.InnerDict.Remove(node.Value.Key);

                this.CacheRemoved?.Invoke(this, new CacheRemovedEventArgs(node.Value));
            }

            return true;
        }

        internal bool AutoTrim()
        {
            if (this.AccessCount < this.AutoTrimCount) return false;

            this.AccessCount = 0; // カウンターをリセット

            return this.Trim();
        }

        public void Add(TKey key, TValue value)
            => this.Add(new KeyValuePair<TKey, TValue>(key, value));

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            var node = new LinkedListNode<KeyValuePair<TKey, TValue>>(item);
            this.InnerList.AddFirst(node);
            this.InnerDict.Add(item.Key, node);

            this.AccessCount++;
            this.AutoTrim();
        }

        public bool ContainsKey(TKey key)
            => this.InnerDict.ContainsKey(key);

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!this.InnerDict.TryGetValue(item.Key, out var node)) return false;

            return EqualityComparer<TValue>.Default.Equals(node.Value.Value, item.Value);
        }

        public bool Remove(TKey key)
        {
            if (!this.InnerDict.TryGetValue(key, out var node)) return false;

            this.InnerList.Remove(node);

            return this.InnerDict.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!this.InnerDict.TryGetValue(item.Key, out var node)) return false;

            if (!EqualityComparer<TValue>.Default.Equals(node.Value.Value, item.Value))
                return false;

            this.InnerList.Remove(node);

            return this.InnerDict.Remove(item.Key);
        }

#pragma warning disable CS8767
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767
        {
            var ret = this.InnerDict.TryGetValue(key, out var node);

            if (!ret)
            {
                value = default!;
                return false;
            }

            this.UpdateAccess(node);
            value = node.Value.Value;

            this.AccessCount++;
            this.AutoTrim();

            return true;
        }

        public ICollection<TKey> Keys
            => this.InnerDict.Keys;

        public ICollection<TValue> Values
            => this.InnerDict.Values.Select(x => x.Value.Value).ToList();

        public TValue this[TKey key]
        {
            get
            {
                var node = this.InnerDict[key];
                this.UpdateAccess(node);

                this.AccessCount++;
                this.AutoTrim();

                return node.Value.Value;
            }

            set
            {
                var pair = new KeyValuePair<TKey, TValue>(key, value);

                if (this.InnerDict.TryGetValue(key, out var node))
                {
                    this.InnerList.Remove(node);
                    node.Value = pair;
                }
                else
                {
                    node = new LinkedListNode<KeyValuePair<TKey, TValue>>(pair);
                    this.InnerDict[key] = node;
                }

                this.InnerList.AddFirst(node);

                this.AccessCount++;
                this.AutoTrim();
            }
        }

        public void Clear()
        {
            this.InnerList.Clear();
            this.InnerDict.Clear();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex >= array.Length)
                throw new ArgumentException("arrayIndex is equal to or greater than array.Length.", nameof(arrayIndex));
            if (array.Length - arrayIndex < this.Count)
                throw new ArgumentException("The destination array is too small.", nameof(array));

            foreach (var pair in this)
                array[arrayIndex++] = pair;
        }

        public int Count
            => this.InnerDict.Count;

        public bool IsReadOnly
            => false;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => this.InnerDict.Select(x => x.Value.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
