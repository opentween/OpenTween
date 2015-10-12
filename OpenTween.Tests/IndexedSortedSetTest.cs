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
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween
{
    public class IndexedSortedSetTest
    {
        [Fact]
        public void IndexedSortedSet_Test()
        {
            var set = new IndexedSortedSet<int>();
            set.Add(100);
            set.Add(20);
            set.Add(600);

            Assert.Equal(3, set.Count);
            Assert.Equal(20, set[0]);
            Assert.Equal(100, set[1]);
            Assert.Equal(600, set[2]);

            set.Remove(100);
            set.Add(5);

            Assert.Equal(3, set.Count);
            Assert.Equal(5, set[0]);
            Assert.Equal(20, set[1]);
            Assert.Equal(600, set[2]);
        }

        [Fact]
        public void Constructor_Test()
        {
            var set = new IndexedSortedSet<int>();
            Assert.Empty(set);

            set.Add(2);
            set.Add(1);
            set.Add(3);
            Assert.Equal(new[] { 1, 2, 3 }, set);
        }

        [Fact]
        public void Constructor_WithComparerTest()
        {
            // 逆順にソートする Comparer
            var comparer = Comparer<int>.Create((x, y) => -x.CompareTo(y));
            var set = new IndexedSortedSet<int>(comparer);
            Assert.Empty(set);

            set.Add(2);
            set.Add(1);
            set.Add(3);
            Assert.Equal(new[] { 3, 2, 1 }, set);
        }

        [Fact]
        public void Constructor_WithEnumerableTest()
        {
            var set = new IndexedSortedSet<int>(new[] { 2, 1, 3 });

            Assert.Equal(new[] { 1, 2, 3 }, set);
        }

        [Fact]
        public void Constructor_WithEnumerableAndComparerTest()
        {
            // 逆順にソートする Comparer
            var comparer = Comparer<int>.Create((x, y) => -x.CompareTo(y));
            var set = new IndexedSortedSet<int>(new[] { 2, 1, 3 }, comparer);

            Assert.Equal(new[] { 3, 2, 1 }, set);
        }

        [Fact]
        public void Count_Test()
        {
            var set = new IndexedSortedSet<int>();

            Assert.Equal(0, set.Count);

            set.Add(1);
            Assert.Equal(1, set.Count);
        }

        [Fact]
        public void IsReadOnly_Test()
        {
            var set = new IndexedSortedSet<int>();
            Assert.False(set.IsReadOnly);
        }

        [Fact]
        public void Getter_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3 };

            Assert.Equal(1, set[0]);
            Assert.Equal(2, set[1]);
            Assert.Equal(3, set[2]);

            Assert.Throws<ArgumentOutOfRangeException>(() => set[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => set[99]);
        }

        [Fact]
        public void Add_Test()
        {
            var set = new IndexedSortedSet<int>();

            Assert.True(set.Add(3));
            Assert.True(set.Add(1));
            Assert.True(set.Add(2));
            Assert.Equal(new[] { 1, 2, 3 }, set);

            Assert.False(set.Add(1));
            Assert.Equal(new[] { 1, 2, 3 }, set);
        }

        [Fact]
        public void Add_ICollectionTest()
        {
            var set = new IndexedSortedSet<int>();
            var collection = (ICollection<int>)set;

            collection.Add(3);
            collection.Add(1);
            collection.Add(2);
            Assert.Equal(new[] { 1, 2, 3 }, set);

            collection.Add(1);
            Assert.Equal(new[] { 1, 2, 3 }, set);
        }

        [Fact]
        public void Remove_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3 };

            Assert.True(set.Remove(2));
            Assert.Equal(new[] { 1, 3 }, set);

            Assert.False(set.Remove(999));
            Assert.Equal(new[] { 1, 3 }, set);
        }

        [Fact]
        public void Clear_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3 };

            set.Clear();
            Assert.Empty(set);
        }

        [Fact]
        public void Contains_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3 };

            Assert.True(set.Contains(2));
            Assert.False(set.Contains(999));
        }

        [Fact]
        public void IndexOf_Test()
        {
            var set = new IndexedSortedSet<int> { 10, 15, 20 };

            Assert.Equal(0, set.IndexOf(10));
            Assert.Equal(1, set.IndexOf(15));
            Assert.Equal(2, set.IndexOf(20));

            Assert.Equal(-1, set.IndexOf(0));
            Assert.Equal(-1, set.IndexOf(12));
            Assert.Equal(-1, set.IndexOf(50));
        }

        [Fact]
        public void RemoveAt_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3 };

            set.RemoveAt(1);
            Assert.Equal(new[] { 1, 3 }, set);

            Assert.Throws<ArgumentOutOfRangeException>(() => set.RemoveAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => set.RemoveAt(99));
        }

        [Fact]
        public void CopyTo_Test()
        {
            var set = new IndexedSortedSet<string> { "aaa", "bbb", "ccc" };

            var array = new string[5];
            set.CopyTo(array, 0);

            Assert.Equal("aaa", array[0]);
            Assert.Equal("bbb", array[1]);
            Assert.Equal("ccc", array[2]);
            Assert.Null(array[3]);
            Assert.Null(array[4]);

            array = new string[5];
            set.CopyTo(array, 1);

            Assert.Null(array[0]);
            Assert.Equal("aaa", array[1]);
            Assert.Equal("bbb", array[2]);
            Assert.Equal("ccc", array[3]);
            Assert.Null(array[4]);

            array = new string[5];

            Assert.Throws<ArgumentException>(() => set.CopyTo(array, 3));
            Assert.Throws<ArgumentException>(() => set.CopyTo(array, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(array, -1));
            Assert.Throws<ArgumentNullException>(() => set.CopyTo(null, 0));
        }

        [Fact]
        public void Enumerator_Test()
        {
            var set = new IndexedSortedSet<string> { "aaa", "bbb", "ccc" };

            var enumerator = set.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Equal("aaa", enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal("bbb", enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal("ccc", enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void Enumerator_NonGenericTest()
        {
            var set = new IndexedSortedSet<string> { "aaa", "bbb", "ccc" };

            var enumerator = ((IEnumerable)set).GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Equal("aaa", enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal("bbb", enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal("ccc", enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void UnionWith_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3, 4 };

            // 和集合
            set.UnionWith(new[] { 6, 4, 3, 5 });

            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, set);
        }

        [Fact]
        public void IntersectWith_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3, 4 };

            // 積集合
            set.IntersectWith(new[] { 6, 4, 3, 5 });

            Assert.Equal(new[] { 3, 4 }, set);
        }

        [Fact]
        public void ExceptWith_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3, 4 };

            // 差集合
            set.ExceptWith(new[] { 6, 4, 3, 5 });

            Assert.Equal(new[] { 1, 2 }, set);
        }

        [Fact]
        public void SymmetricExceptWith_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3, 4 };

            // 対称差
            set.SymmetricExceptWith(new[] { 6, 4, 3, 5 });

            Assert.Equal(new[] { 1, 2, 5, 6 }, set);
        }

        [Fact]
        public void IsSubsetOf_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3, 4 };

            // 部分集合
            Assert.True(set.IsSubsetOf(new[] { 1, 2, 3, 4, 5, 6 }));
            Assert.True(set.IsSubsetOf(new[] { 1, 2, 3, 4 }));
            Assert.False(set.IsSubsetOf(new[] { 1, 3, 5, 7 }));
            Assert.False(set.IsSubsetOf(new[] { -1, -2, -3, -4 }));
        }

        [Fact]
        public void IsSupersetOf_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3, 4 };

            // 上位集合
            Assert.True(set.IsSupersetOf(new[] { 2, 3 }));
            Assert.True(set.IsSupersetOf(new[] { 1, 2, 3, 4 }));
            Assert.False(set.IsSupersetOf(new[] { 1, 2, 3, 4, 5, 6 }));
            Assert.False(set.IsSupersetOf(new[] { -1, -2, -3, -4 }));
        }

        [Fact]
        public void IsProperSubsetOf_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3, 4 };

            // 真部分集合
            Assert.True(set.IsProperSubsetOf(new[] { 1, 2, 3, 4, 5, 6 }));
            Assert.False(set.IsProperSubsetOf(new[] { 1, 2, 3, 4 }));
            Assert.False(set.IsProperSubsetOf(new[] { 1, 3, 5, 7 }));
            Assert.False(set.IsProperSubsetOf(new[] { -1, -2, -3, -4 }));
        }

        [Fact]
        public void IsProperSupersetOf_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3, 4 };

            // 真上位集合
            Assert.True(set.IsProperSupersetOf(new[] { 2, 3 }));
            Assert.False(set.IsProperSupersetOf(new[] { 1, 2, 3, 4 }));
            Assert.False(set.IsProperSupersetOf(new[] { 1, 2, 3, 4, 5, 6 }));
            Assert.False(set.IsProperSupersetOf(new[] { -1, -2, -3, -4 }));
        }

        [Fact]
        public void Overlaps_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3, 4 };

            // 共通する要素が存在するか
            Assert.True(set.Overlaps(new[] { 1, 10, 100, 1000 }));
            Assert.True(set.Overlaps(new[] { 1, 2, 3, 4 }));
            Assert.False(set.Overlaps(new[] { -1, -2, -3, -4 }));
        }

        [Fact]
        public void SetEquals_Test()
        {
            var set = new IndexedSortedSet<int> { 1, 2, 3, 4 };

            // 等しい集合であるか
            Assert.True(set.SetEquals(new[] { 1, 2, 3, 4 }));
            Assert.False(set.SetEquals(new[] { 1, 2 }));
            Assert.False(set.SetEquals(new[] { 1, 2, 3, 4, 5, 6 }));
            Assert.False(set.SetEquals(new[] { 8, 9 }));

            // 重複は無視される
            Assert.True(set.SetEquals(new[] { 1, 2, 2, 3, 3, 4 }));
        }
    }
}
