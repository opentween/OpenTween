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

namespace OpenTween.Setting.Panel
{
    public class GraphqlRequestEstimation
    {
        public static int CalcDailyRequestCount(Intervals intervals, TabTypeAggregation.Result tabCounts)
        {
            const int dayInSeconds = 1 * 24 * 60 * 60;

            static int CalcDailyCount(int interval, int tabCount)
                => interval == 0 ? 0 : dayInSeconds / interval * tabCount;

            var homeDailyCount = CalcDailyCount(intervals.Home, tabCounts.HomeTabs);
            var searchDailyCount = CalcDailyCount(intervals.Search, tabCounts.SearchTabs);
            var listDailyCount = CalcDailyCount(intervals.List, tabCounts.ListTabs);
            var userDaylyCount = CalcDailyCount(intervals.User, tabCounts.UserTabs);

            return homeDailyCount + searchDailyCount + listDailyCount + userDaylyCount;
        }

        public readonly record struct Intervals(
            int Home,
            int Search,
            int List,
            int User
        );
    }
}
