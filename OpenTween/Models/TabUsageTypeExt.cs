// OpenTween - Client of Twitter
// Copyright (c) 2015 spx (@5px)
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Models
{
    /// <summary>
    /// enum TabUsageType に対応する拡張メソッドを定義したクラス
    /// </summary>
    public static class TabUsageTypeExt
    {
        private const MyCommon.TabUsageType DefaultTabTypeMask =
            MyCommon.TabUsageType.Home |
            MyCommon.TabUsageType.Mentions |
            MyCommon.TabUsageType.DirectMessage |
            MyCommon.TabUsageType.Favorites |
            MyCommon.TabUsageType.Mute;

        private const MyCommon.TabUsageType DistributableTabTypeMask =
            MyCommon.TabUsageType.Mentions |
            MyCommon.TabUsageType.UserDefined |
            MyCommon.TabUsageType.Mute;

        private const MyCommon.TabUsageType InnerStorageTabTypeMask =
            MyCommon.TabUsageType.DirectMessage |
            MyCommon.TabUsageType.PublicSearch |
            MyCommon.TabUsageType.Lists |
            MyCommon.TabUsageType.UserTimeline |
            MyCommon.TabUsageType.Related |
            MyCommon.TabUsageType.SearchResults;

        /// <summary>
        /// デフォルトタブかどうかを示す値を取得します。
        /// </summary>
        public static bool IsDefault(this MyCommon.TabUsageType tabType)
            => (tabType & DefaultTabTypeMask) != 0;

        /// <summary>
        /// 振り分け可能タブかどうかを示す値を取得します。
        /// </summary>
        public static bool IsDistributable(this MyCommon.TabUsageType tabType)
            => (tabType & DistributableTabTypeMask) != 0;

        /// <summary>
        /// 内部ストレージを使用するタブかどうかを示す値を取得します。
        /// </summary>
        public static bool IsInnerStorage(this MyCommon.TabUsageType tabType)
            => (tabType & InnerStorageTabTypeMask) != 0;
    }
}
