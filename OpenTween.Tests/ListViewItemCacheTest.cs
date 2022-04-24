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
using System.Windows.Forms;
using OpenTween.Models;
using Xunit;

namespace OpenTween
{
    public class ListViewItemCacheTest
    {
        [Fact]
        public void Cache_InvalidSizeTest()
        {
            var startIndex = 10;
            var endIndex = 19;
            Assert.Throws<ArgumentException>(
                () => new ListViewItemCache(startIndex, endIndex, new (ListViewItem, PostClass)[9])
            );
            Assert.Throws<ArgumentException>(
                () => new ListViewItemCache(startIndex, endIndex, new (ListViewItem, PostClass)[11])
            );
        }

        [Fact]
        public void Count_Test()
        {
            var cache = new ListViewItemCache(10, 19, new (ListViewItem, PostClass)[10]);
            Assert.Equal(10, cache.Count);
        }

        [Theory]
        [InlineData(9, false)]
        [InlineData(10, true)]
        [InlineData(19, true)]
        [InlineData(20, false)]
        public void Contains_Test(int index, bool expected)
        {
            var cache = new ListViewItemCache(10, 19, new (ListViewItem, PostClass)[10]);
            Assert.Equal(expected, cache.Contains(index));
        }

        [Theory]
        [InlineData(9, 19, false)]
        [InlineData(9, 20, false)]
        [InlineData(10, 19, true)]
        [InlineData(10, 20, false)]
        public void IsSupersetOf_Test(int start, int end, bool expected)
        {
            var cache = new ListViewItemCache(10, 19, new (ListViewItem, PostClass)[10]);
            Assert.Equal(expected, cache.IsSupersetOf(start, end));
        }

        [Fact]
        public void TryGetValue_FoundTest()
        {
            var item = new ListViewItem();
            var post = new PostClass();
            var cache = new ListViewItemCache(10, 10, new[] { (item, post) });

            Assert.True(cache.TryGetValue(10, out var actualItem, out var actualPost));
            Assert.Equal(item, actualItem);
            Assert.Equal(post, actualPost);
        }

        [Fact]
        public void TryGetValue_NotFoundTest()
        {
            var item = new ListViewItem();
            var post = new PostClass();
            var cache = new ListViewItemCache(10, 10, new[] { (item, post) });

            Assert.False(cache.TryGetValue(9, out _, out _));
            Assert.False(cache.TryGetValue(11, out _, out _));
        }
    }
}
