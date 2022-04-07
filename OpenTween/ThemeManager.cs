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

namespace OpenTween
{
    public sealed class ThemeManager : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        /// <summary>未読用フォント</summary>
        public Font FontUnread { get; }

        /// <summary>既読用フォント</summary>
        public Font FontReaded { get; }

        /// <summary>発言詳細部用フォント</summary>
        public Font FontDetail { get; }

        /// <summary>入力欄フォント</summary>
        public Font FontInputFont { get; }

        /// <summary>未読用文字色</summary>
        public Color ColorUnread { get; }

        /// <summary>既読用文字色</summary>
        public Color ColorRead { get; }

        /// <summary>Fav用文字色</summary>
        public Color ColorFav { get; }

        /// <summary>片思い用文字色</summary>
        public Color ColorOWL { get; }

        /// <summary>Retweet用文字色</summary>
        public Color ColorRetweet { get; }

        /// <summary>選択中の行用文字色</summary>
        public Color ColorHighLight { get; }

        /// <summary>発言詳細部用色</summary>
        public Color ColorDetail { get; }

        /// <summary>発言詳細部用リンク文字色</summary>
        public Color ColorDetailLink { get; }

        /// <summary>発言詳細部用背景色</summary>
        public Color ColorDetailBackcolor { get; }

        /// <summary>自分の発言用背景色</summary>
        public Color ColorSelf { get; }

        /// <summary>自分宛返信用背景色</summary>
        public Color ColorAtSelf { get; }

        /// <summary>選択発言者の他の発言用背景色</summary>
        public Color ColorTarget { get; }

        /// <summary>選択発言中の返信先用背景色</summary>
        public Color ColorAtTarget { get; }

        /// <summary>選択発言者への返信発言用背景色</summary>
        public Color ColorAtFromTarget { get; }

        /// <summary>選択発言の唯一＠先</summary>
        public Color ColorAtTo { get; }

        /// <summary>リスト部通常発言背景色</summary>
        public Color ColorListBackcolor { get; }

        /// <summary>入力欄背景色</summary>
        public Color ColorInputBackcolor { get; }

        /// <summary>入力欄文字色</summary>
        public Color ColorInputFont { get; }

        public Brush BrushSelf { get; }

        public Brush BrushAtSelf { get; }

        public Brush BrushTarget { get; }

        public Brush BrushAtTarget { get; }

        public Brush BrushAtFromTarget { get; }

        public Brush BrushAtTo { get; }

        public Brush BrushListBackcolor { get; }

        public ThemeManager(SettingLocal settingLocal)
        {
            this.FontUnread = settingLocal.FontUnread;
            this.FontReaded = settingLocal.FontRead;
            this.FontDetail = settingLocal.FontDetail;
            this.FontInputFont = settingLocal.FontInputFont;

            this.ColorUnread = settingLocal.ColorUnread;
            this.ColorRead = settingLocal.ColorRead;
            this.ColorFav = settingLocal.ColorFav;
            this.ColorOWL = settingLocal.ColorOWL;
            this.ColorRetweet = settingLocal.ColorRetweet;
            this.ColorHighLight = Color.FromKnownColor(KnownColor.HighlightText);
            this.ColorDetail = settingLocal.ColorDetail;
            this.ColorDetailLink = settingLocal.ColorDetailLink;
            this.ColorDetailBackcolor = settingLocal.ColorDetailBackcolor;
            this.ColorSelf = settingLocal.ColorSelf;
            this.ColorAtSelf = settingLocal.ColorAtSelf;
            this.ColorTarget = settingLocal.ColorTarget;
            this.ColorAtTarget = settingLocal.ColorAtTarget;
            this.ColorAtFromTarget = settingLocal.ColorAtFromTarget;
            this.ColorAtTo = settingLocal.ColorAtTo;
            this.ColorListBackcolor = settingLocal.ColorListBackcolor;
            this.ColorInputBackcolor = settingLocal.ColorInputBackcolor;
            this.ColorInputFont = settingLocal.ColorInputFont;

            this.BrushSelf = new SolidBrush(this.ColorSelf);
            this.BrushAtSelf = new SolidBrush(this.ColorAtSelf);
            this.BrushTarget = new SolidBrush(this.ColorTarget);
            this.BrushAtTarget = new SolidBrush(this.ColorAtTarget);
            this.BrushAtFromTarget = new SolidBrush(this.ColorAtFromTarget);
            this.BrushAtTo = new SolidBrush(this.ColorAtTo);
            this.BrushListBackcolor = new SolidBrush(this.ColorListBackcolor);
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.FontUnread.Dispose();
            this.FontReaded.Dispose();
            this.FontDetail.Dispose();
            this.FontInputFont.Dispose();
            this.BrushSelf.Dispose();
            this.BrushAtSelf.Dispose();
            this.BrushTarget.Dispose();
            this.BrushAtTarget.Dispose();
            this.BrushAtFromTarget.Dispose();
            this.BrushAtTo.Dispose();
            this.BrushListBackcolor.Dispose();

            this.IsDisposed = true;
        }

        public static void ApplyGlobalUIFont(SettingLocal settingLocal)
        {
            var font = settingLocal.FontUIGlobal;
            if (font != null)
                OTBaseForm.GlobalFont = font;
        }
    }
}
