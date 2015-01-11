// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Windows.Forms;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class TabClassTest
    {
        [Fact]
        public void OldestUnreadId_Test()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            // 未読なし
            Assert.Equal(-1L, tab.OldestUnreadId);

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(100L, tab.OldestUnreadId);

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 50L,
                IsRead = true, // 既読
            });
            tab.AddSubmit();

            Assert.Equal(100L, tab.OldestUnreadId);
        }

        [Fact]
        public void OldestUnreadId_DisabledTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            // 未読表示無効
            tab.UnreadManage = false;

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(-1L, tab.OldestUnreadId);
        }

        [Fact]
        public void UnreadCount_Test()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            // 未読なし
            Assert.Equal(0, tab.UnreadCount);

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(1, tab.UnreadCount);

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 50L,
                IsRead = true, // 既読
            });
            tab.AddSubmit();

            Assert.Equal(1, tab.UnreadCount);
        }

        [Fact]
        public void UnreadCount_DisabledTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            // 未読表示無効
            tab.UnreadManage = false;

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(0, tab.UnreadCount);
        }

        [Fact]
        public void OldestUnreadIndex_Test()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            // 未読なし
            Assert.Equal(-1, tab.OldestUnreadIndex);

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 50L,
                IsRead = true, // 既読
            });
            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 150L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            tab.SortMode = ComparerMode.Id;
            tab.SortOrder = SortOrder.Ascending;
            tab.Sort();

            Assert.Equal(1, tab.OldestUnreadIndex);
        }

        [Fact]
        public void OldestUnreadIndex_DisabledTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            // 未読表示無効
            tab.UnreadManage = false;

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            tab.SortMode = ComparerMode.Id;
            tab.SortOrder = SortOrder.Ascending;
            tab.Sort();

            Assert.Equal(-1, tab.OldestUnreadIndex);
        }

        [Fact]
        public void SetReadState_MarkAsReadTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(1, tab.UnreadCount);

            tab.SetReadState(100L, true); // 既読にする

            Assert.Equal(0, tab.UnreadCount);
        }

        [Fact]
        public void SetReadState_MarkAsUnreadTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = true, // 既読
            });
            tab.AddSubmit();

            Assert.Equal(0, tab.UnreadCount);

            tab.SetReadState(100L, false); // 未読にする

            Assert.Equal(1, tab.UnreadCount);
        }

        [Fact]
        public void FilterArraySetter_Test()
        {
            var tab = new TabClass();

            var filter = new PostFilterRule();
            tab.FilterArray = new[] { filter };

            Assert.Equal(new[] { filter }, tab.FilterArray);
            Assert.True(tab.FilterModified);
        }

        [Fact]
        public void AddFilter_Test()
        {
            var tab = new TabClass();

            var filter = new PostFilterRule();
            tab.AddFilter(filter);

            Assert.Equal(new[] { filter }, tab.FilterArray);
            Assert.True(tab.FilterModified);
        }

        [Fact]
        public void RemoveFilter_Test()
        {
            var tab = new TabClass();

            var filter = new PostFilterRule();
            tab.FilterArray = new[] { filter };
            tab.FilterModified = false;

            tab.RemoveFilter(filter);

            Assert.Empty(tab.FilterArray);
            Assert.True(tab.FilterModified);
        }

        [Fact]
        public void OnFilterModified_Test()
        {
            var tab = new TabClass();

            var filter = new PostFilterRule();
            tab.FilterArray = new[] { filter };
            tab.FilterModified = false;

            // TabClass に紐付いているフィルタを変更
            filter.FilterSource = "OpenTween";

            Assert.True(tab.FilterModified);
        }

        [Fact]
        public void OnFilterModified_DetachedTest()
        {
            var tab = new TabClass();

            var filter = new PostFilterRule();
            tab.FilterArray = new[] { filter };

            tab.RemoveFilter(filter);
            tab.FilterModified = false;

            // TabClass から既に削除されたフィルタを変更
            filter.FilterSource = "OpenTween";

            Assert.False(tab.FilterModified);
        }
    }

    public class TabUsageTypeExtTest
    {
        [Theory]
        [InlineData(MyCommon.TabUsageType.Home,          true)]
        [InlineData(MyCommon.TabUsageType.Mentions,      true)]
        [InlineData(MyCommon.TabUsageType.DirectMessage, true)]
        [InlineData(MyCommon.TabUsageType.Favorites,     true)]
        [InlineData(MyCommon.TabUsageType.UserDefined,   false)]
        [InlineData(MyCommon.TabUsageType.Lists,         false)]
        [InlineData(MyCommon.TabUsageType.UserTimeline,  false)]
        [InlineData(MyCommon.TabUsageType.PublicSearch,  false)]
        [InlineData(MyCommon.TabUsageType.Related,       false)]
        public void IsDefault_Test(MyCommon.TabUsageType tabType, bool expected)
        {
            Assert.Equal(expected, tabType.IsDefault());
        }

        [Theory]
        [InlineData(MyCommon.TabUsageType.Home,          false)]
        [InlineData(MyCommon.TabUsageType.Mentions,      true)]
        [InlineData(MyCommon.TabUsageType.DirectMessage, false)]
        [InlineData(MyCommon.TabUsageType.Favorites,     false)]
        [InlineData(MyCommon.TabUsageType.UserDefined,   true)]
        [InlineData(MyCommon.TabUsageType.Lists,         false)]
        [InlineData(MyCommon.TabUsageType.UserTimeline,  false)]
        [InlineData(MyCommon.TabUsageType.PublicSearch,  false)]
        [InlineData(MyCommon.TabUsageType.Related,       false)]
        public void IsDistributable_Test(MyCommon.TabUsageType tabType, bool expected)
        {
            Assert.Equal(expected, tabType.IsDistributable());
        }

        [Theory]
        [InlineData(MyCommon.TabUsageType.Home,          false)]
        [InlineData(MyCommon.TabUsageType.Mentions,      false)]
        [InlineData(MyCommon.TabUsageType.DirectMessage, true)]
        [InlineData(MyCommon.TabUsageType.Favorites,     false)]
        [InlineData(MyCommon.TabUsageType.UserDefined,   false)]
        [InlineData(MyCommon.TabUsageType.Lists,         true)]
        [InlineData(MyCommon.TabUsageType.UserTimeline,  true)]
        [InlineData(MyCommon.TabUsageType.PublicSearch,  true)]
        [InlineData(MyCommon.TabUsageType.Related,       true)]
        public void IsInnerStorage_Test(MyCommon.TabUsageType tabType, bool expected)
        {
            Assert.Equal(expected, tabType.IsInnerStorage());
        }
    }
}
