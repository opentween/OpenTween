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

#nullable enable

using System.Linq;
using OpenTween.Models;

namespace OpenTween.Setting.Panel
{
    public class TabTypeAggregation
    {
        public static Result Aggregate(TabInformations tabInfo)
        {
            var countByTabTypes = tabInfo.Tabs.GroupBy(x => x.TabType)
                .ToDictionary(x => x.Key, x => x.Count());

            int GetCountByTabType(MyCommon.TabUsageType tabType)
                => countByTabTypes.TryGetValue(tabType, out var count) ? count : 0;

            return new(
                HomeTabs: GetCountByTabType(MyCommon.TabUsageType.Home),
                MentionsTabs: GetCountByTabType(MyCommon.TabUsageType.Mentions),
                DMTabs: GetCountByTabType(MyCommon.TabUsageType.DirectMessage),
                SearchTabs: GetCountByTabType(MyCommon.TabUsageType.PublicSearch),
                ListTabs: GetCountByTabType(MyCommon.TabUsageType.Lists),
                UserTabs: GetCountByTabType(MyCommon.TabUsageType.UserTimeline)
            );
        }

        public readonly record struct Result(
            int HomeTabs,
            int MentionsTabs,
            int DMTabs,
            int SearchTabs,
            int ListTabs,
            int UserTabs
        );
    }
}
