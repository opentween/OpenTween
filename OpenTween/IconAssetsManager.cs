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

#nullable enable

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace OpenTween
{
    public sealed class IconAssetsManager : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        /// <summary>ウィンドウ左上のアイコン</summary>
        public Icon IconMain { get; }

        /// <summary>タブ見出し未読表示アイコン</summary>
        public Icon IconTab { get; }

        /// <summary>タスクトレイ: 通常時アイコン</summary>
        public Icon IconTray { get; }

        /// <summary>タスクトレイ: エラー時アイコン</summary>
        public Icon IconTrayError { get; }

        /// <summary>タスクトレイ: オフライン時アイコン</summary>
        public Icon IconTrayOffline { get; }

        /// <summary>タスクトレイ: Reply通知アイコン</summary>
        public Icon IconTrayReply { get; }

        /// <summary>タスクトレイ: Reply通知アイコン（点滅時）</summary>
        public Icon IconTrayReplyBlink { get; }

        /// <summary>タスクトレイ: 更新中アイコン</summary>
        public Icon[] IconTrayRefresh { get; }

        private readonly Icon? iconMain;
        private readonly Icon? iconTab;
        private readonly Icon? iconAt;
        private readonly Icon? iconAtRed;
        private readonly Icon? iconAtSmoke;
        private readonly Icon? iconReply;
        private readonly Icon? iconReplyBlink;
        private readonly Icon? iconRefresh1;
        private readonly Icon? iconRefresh2;
        private readonly Icon? iconRefresh3;
        private readonly Icon? iconRefresh4;

        public IconAssetsManager()
            : this(Path.Combine(Application.StartupPath, "Icons"))
        {
        }

        public IconAssetsManager(string iconsDir)
        {
            this.iconMain = this.LoadIcon(iconsDir, "MIcon.ico");
            this.iconTab = this.LoadIcon(iconsDir, "Tab.ico");
            this.iconAt = this.LoadIcon(iconsDir, "At.ico");
            this.iconAtRed = this.LoadIcon(iconsDir, "AtRed.ico");
            this.iconAtSmoke = this.LoadIcon(iconsDir, "AtSmoke.ico");
            this.iconReply = this.LoadIcon(iconsDir, "Reply.ico");
            this.iconReplyBlink = this.LoadIcon(iconsDir, "ReplyBlink.ico");
            this.iconRefresh1 = this.LoadIcon(iconsDir, "Refresh.ico");
            this.iconRefresh2 = this.LoadIcon(iconsDir, "Refresh2.ico");
            this.iconRefresh3 = this.LoadIcon(iconsDir, "Refresh3.ico");
            this.iconRefresh4 = this.LoadIcon(iconsDir, "Refresh4.ico");

            this.IconMain = this.iconMain ?? Properties.Resources.MIcon;
            this.IconTab = this.iconTab ?? Properties.Resources.TabIcon;
            this.IconTray = this.iconAt ?? this.iconMain ?? Properties.Resources.At;
            this.IconTrayError = this.iconAtRed ?? Properties.Resources.AtRed;
            this.IconTrayOffline = this.iconAtSmoke ?? Properties.Resources.AtSmoke;

            if (this.iconReply != null && this.iconReplyBlink != null)
            {
                this.IconTrayReply = this.iconReply;
                this.IconTrayReplyBlink = this.iconReplyBlink;
            }
            else
            {
                this.IconTrayReply = this.iconReply ?? this.iconReplyBlink ?? Properties.Resources.Reply;
                this.IconTrayReplyBlink = this.IconTray;
            }

            if (this.iconRefresh1 == null)
            {
                this.IconTrayRefresh = new[]
                {
                    Properties.Resources.Refresh, Properties.Resources.Refresh2,
                    Properties.Resources.Refresh3, Properties.Resources.Refresh4,
                };
            }
            else if (this.iconRefresh2 == null)
            {
                this.IconTrayRefresh = new[] { this.iconRefresh1 };
            }
            else if (this.iconRefresh3 == null)
            {
                this.IconTrayRefresh = new[] { this.iconRefresh1, this.iconRefresh2 };
            }
            else if (this.iconRefresh4 == null)
            {
                this.IconTrayRefresh = new[] { this.iconRefresh1, this.iconRefresh2, this.iconRefresh3 };
            }
            else // iconRefresh1 から iconRefresh4 まで全て揃っている
            {
                this.IconTrayRefresh = new[] { this.iconRefresh1, this.iconRefresh2, this.iconRefresh3, this.iconRefresh4 };
            }
        }

        private Icon? LoadIcon(string baseDir, string fileName)
        {
            var filePath = Path.Combine(baseDir, fileName);
            if (!File.Exists(filePath))
                return null;

            try
            {
                return new(filePath);
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.iconMain?.Dispose();
            this.iconTab?.Dispose();
            this.iconAt?.Dispose();
            this.iconAtRed?.Dispose();
            this.iconAtSmoke?.Dispose();
            this.iconReply?.Dispose();
            this.iconReplyBlink?.Dispose();
            this.iconRefresh1?.Dispose();
            this.iconRefresh2?.Dispose();
            this.iconRefresh3?.Dispose();
            this.iconRefresh4?.Dispose();
            this.IsDisposed = true;
        }
    }
}
