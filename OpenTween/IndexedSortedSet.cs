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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenTween
{
    /// <summary>
    /// インデックスでのアクセスが可能なソートされた重複のないコレクション
    /// </summary>
    public class IndexedSortedSet<T> : ISet<T>, IReadOnlyList<T>
    {
        private readonly List<T> innerList;
        private readonly IComparer<T> comparer;

        public int Count
            => this.innerList.Count;

        public bool IsReadOnly
            => false;

        public T this[int index]
            => this.innerList[index];

        public IndexedSortedSet()
            : this(Enumerable.Empty<T>(), Comparer<T>.Default)
        {
        }

        public IndexedSortedSet(IComparer<T> comparer)
            : this(Enumerable.Empty<T>(), comparer)
        {
        }

        public IndexedSortedSet(IEnumerable<T> items)
            : this(items, Comparer<T>.Default)
        {
        }

        public IndexedSortedSet(IEnumerable<T> items, IComparer<T> comparer)
        {
            this.innerList = new List<T>();
            this.comparer = comparer;

            this.UnionWith(items);
        }

        public bool Add(T item)
        {
            if (this.innerList.Count == 0)
            {
                this.innerList.Add(item);
                return true;
            }

            var index = this.innerList.BinarySearch(item, comparer);
            if (index >= 0)
                return false;

            // ~index → item の次に大きい要素のインデックス
            this.innerList.Insert(~index, item);

            return true;
        }

        void ICollection<T>.Add(T item)
            => this.Add(item);

        public bool Remove(T item)
        {
            var index = this.IndexOf(item);
            if (index == -1)
                return false;

            this.innerList.RemoveAt(index);

            return true;
        }

        public void Clear()
            => this.innerList.Clear();

        public bool Contains(T item)
            => this.IndexOf(item) != -1;

        public int IndexOf(T item)
        {
            var index = this.innerList.BinarySearch(item, this.comparer);

            return index < 0 ? -1 : index;
        }

        public void RemoveAt(int index)
            => this.innerList.RemoveAt(index);

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex >= array.Length)
                throw new ArgumentException($"{nameof(arrayIndex)} is equal to or greater than {nameof(array)}.Length.", nameof(arrayIndex));
            if (array.Length - arrayIndex < this.Count)
                throw new ArgumentException("The destination array is too small.", nameof(array));

            foreach (var pair in this)
                array[arrayIndex++] = pair;
        }

        public IEnumerator<T> GetEnumerator()
            => this.innerList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                this.Add(item);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            var containsMark = new bool[this.Count];

            foreach (var item in other)
            {
                var index = this.IndexOf(item);
                if (index != -1)
                    containsMark[index] = true;
            }

            foreach (var index in MyCommon.CountDown(this.Count - 1, 0))
            {
                if (!containsMark[index])
                    this.RemoveAt(index);
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                this.Remove(item);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (this.Contains(item))
                    this.Remove(item);
                else
                    this.Add(item);
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            var containsMark = new bool[this.Count];

            foreach (var item in other)
            {
                var index = this.IndexOf(item);
                if (index != -1)
                    containsMark[index] = true;
            }

            return containsMark.All(x => x == true);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (!this.Contains(item))
                    return false;
            }

            return true;
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
            => !this.SetEquals(other) && this.IsSupersetOf(other);

        public bool IsProperSubsetOf(IEnumerable<T> other)
            => !this.SetEquals(other) && this.IsSubsetOf(other);

        public bool Overlaps(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (this.Contains(item))
                    return true;
            }

            return false;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            var containsMark = new bool[this.Count];

            foreach (var item in other)
            {
                var index = this.IndexOf(item);
                if (index == -1)
                    return false;

                containsMark[index] = true;
            }

            return containsMark.All(x => x == true);
        }
    }
}
