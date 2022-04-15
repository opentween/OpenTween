// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Xml.Serialization;
using OpenTween.Models;

namespace OpenTween
{
    public class SettingTabs : SettingBase<SettingTabs>
    {
#region Settingクラス基本
        public static SettingTabs Load(string settingsPath)
            => LoadSettings(settingsPath);

        public void Save(string settingsPath)
            => SaveSettings(this, settingsPath);

        public SettingTabs()
            => this.Tabs = new List<SettingTabItem>();
#endregion

        public List<SettingTabItem> Tabs;

        [XmlType("TabClass")]
        public class SettingTabItem
        {
            /// <summary>タブの表示名</summary>
            public string TabName { get; set; } = "";

            /// <summary>タブの種類</summary>
            public MyCommon.TabUsageType TabType { get; set; }

            /// <summary>未読管理を有効にする</summary>
            public bool UnreadManage { get; set; }

            /// <summary>タブを保護する</summary>
            [XmlElement(ElementName = "Locked")] // v1.0.5で「タブを固定(Locked)」から「タブを保護(Protected)」に名称変更
            public bool Protected { get; set; }

            /// <summary>新着通知表示を有効にする</summary>
            public bool Notify { get; set; }

            /// <summary>通知音</summary>
            public string SoundFile { get; set; } = "";

            /// <summary>
            /// 振り分けルール (<see cref="MyCommon.TabUsageType.UserDefined"/> で使用)
            /// </summary>
            public PostFilterRule[] FilterArray { get; set; } = Array.Empty<PostFilterRule>();

            /// <summary>
            /// 表示するユーザーのスクリーンネーム (<see cref="MyCommon.TabUsageType.UserTimeline"/> で使用)
            /// </summary>
            public string? User { get; set; }

            /// <summary>
            /// 検索文字列 (<see cref="MyCommon.TabUsageType.PublicSearch"/> で使用)
            /// </summary>
            public string SearchWords { get; set; } = "";

            /// <summary>
            /// 検索対象の言語 (<see cref="MyCommon.TabUsageType.PublicSearch"/> で使用)
            /// </summary>
            public string SearchLang { get; set; } = "";

            /// <summary>
            /// 表示するリスト (<see cref="MyCommon.TabUsageType.Lists"/> で使用)
            /// </summary>
            public ListElement? ListInfo { get; set; }
        }
    }
}
