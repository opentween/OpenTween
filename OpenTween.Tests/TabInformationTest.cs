// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class TabInformationTest
    {
        private TabInformations tabinfo;

        public TabInformationTest()
        {
            this.tabinfo = Activator.CreateInstance(typeof(TabInformations), true) as TabInformations;

            // 標準のタブを追加
            this.tabinfo.AddTab("Recent", MyCommon.TabUsageType.Home, null);
            this.tabinfo.AddTab("Reply", MyCommon.TabUsageType.Mentions, null);
            this.tabinfo.AddTab("DM", MyCommon.TabUsageType.DirectMessage, null);
            this.tabinfo.AddTab("Favorites", MyCommon.TabUsageType.Favorites, null);

            // TabInformation.GetInstance() で取得できるようにする
            var field = typeof(TabInformations).GetField("_instance",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField);
            field.SetValue(null, this.tabinfo);
        }

        [Fact]
        public void MakeTabName_Test()
        {
            var baseTabName = "NewTab";
            Assert.Equal("NewTab", this.tabinfo.MakeTabName(baseTabName, 5));
        }

        [Fact]
        public void MakeTabName_RetryTest()
        {
            this.tabinfo.AddTab("NewTab", MyCommon.TabUsageType.UserDefined, null);
            this.tabinfo.AddTab("NewTab2", MyCommon.TabUsageType.UserDefined, null);

            var baseTabName = "NewTab";
            Assert.Equal("NewTab3", this.tabinfo.MakeTabName(baseTabName, 5));
        }

        [Fact]
        public void MakeTabName_RetryErrorTest()
        {
            this.tabinfo.AddTab("NewTab", MyCommon.TabUsageType.UserDefined, null);
            this.tabinfo.AddTab("NewTab2", MyCommon.TabUsageType.UserDefined, null);
            this.tabinfo.AddTab("NewTab3", MyCommon.TabUsageType.UserDefined, null);
            this.tabinfo.AddTab("NewTab4", MyCommon.TabUsageType.UserDefined, null);
            this.tabinfo.AddTab("NewTab5", MyCommon.TabUsageType.UserDefined, null);

            var baseTabName = "NewTab";
            Assert.Throws<TabException>(() => this.tabinfo.MakeTabName(baseTabName, 5));
        }
    }
}
