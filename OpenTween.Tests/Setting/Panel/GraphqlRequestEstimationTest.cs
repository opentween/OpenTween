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

using Xunit;

namespace OpenTween.Setting.Panel
{
    public class GraphqlRequestEstimationTest
    {
        [Fact]
        public void CalcDailyRequestCount_Test()
        {
            var tabCounts = new TabTypeAggregation.Result(
                HomeTabs: 1,
                MentionsTabs: 1,
                DMTabs: 1,
                SearchTabs: 2,
                ListTabs: 1,
                UserTabs: 1
            );
            var intervals = new GraphqlRequestEstimation.Intervals(
                Home: 90, // 960 requests / day
                Search: 180, // 480 requests / day
                List: 180, // 480 requests / day
                User: 600 // 144 requests / day
            );
            Assert.Equal(2544, GraphqlRequestEstimation.CalcDailyRequestCount(intervals, tabCounts));
        }

        [Fact]
        public void CalcDailyRequestCount_DisableAutoRefreshTest()
        {
            var tabCounts = new TabTypeAggregation.Result(
                HomeTabs: 1,
                MentionsTabs: 1,
                DMTabs: 1,
                SearchTabs: 2,
                ListTabs: 1,
                UserTabs: 1
            );
            var intervals = new GraphqlRequestEstimation.Intervals(
                Home: 0, // 0 は自動更新の無効化を表す
                Search: 0,
                List: 0,
                User: 0
            );
            Assert.Equal(0, GraphqlRequestEstimation.CalcDailyRequestCount(intervals, tabCounts));
        }
    }
}
