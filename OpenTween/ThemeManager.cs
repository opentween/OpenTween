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

        public Font FontUnreadBold { get; }

        /// <summary>既読用フォント</summary>
        public Font FontReaded { get; }

        public Font FontReadedBold { get; }

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

        public Brush BrushHighLight { get; }

        /// <summary>Listにフォーカスがないときの選択行の背景色</summary>
        public Brush BrushDeactiveSelection { get; }

        public ThemeManager(SettingLocal settingLocal)
        {
            var fontConverter = new FontConverter();

            this.FontUnread = ConvertStringToFont(fontConverter, settingLocal.FontUnreadStr)
                ?? new(SystemFonts.DefaultFont, FontStyle.Bold | FontStyle.Underline);

            this.FontUnreadBold = new(this.FontUnread, FontStyle.Bold);

            this.FontReaded = ConvertStringToFont(fontConverter, settingLocal.FontReadStr)
                ?? SystemFonts.DefaultFont;

            this.FontReadedBold = new(this.FontReaded, FontStyle.Bold);

            this.FontDetail = ConvertStringToFont(fontConverter, settingLocal.FontDetailStr)
                ?? SystemFonts.DefaultFont;

            this.FontInputFont = ConvertStringToFont(fontConverter, settingLocal.FontInputFontStr)
                ?? SystemFonts.DefaultFont;

            var colorConverter = new ColorConverter();

            this.ColorUnread = ConvertStringToColor(colorConverter, settingLocal.ColorUnreadStr)
                ?? SystemColors.ControlText;

            this.ColorRead = ConvertStringToColor(colorConverter, settingLocal.ColorReadStr)
                ?? SystemColors.ControlText;

            this.ColorFav = ConvertStringToColor(colorConverter, settingLocal.ColorFavStr)
                ?? Color.FromKnownColor(KnownColor.Red);

            this.ColorOWL = ConvertStringToColor(colorConverter, settingLocal.ColorOWLStr)
                ?? Color.FromKnownColor(KnownColor.Blue);

            this.ColorRetweet = ConvertStringToColor(colorConverter, settingLocal.ColorRetweetStr)
                ?? Color.FromKnownColor(KnownColor.Green);

            this.ColorHighLight = Color.FromKnownColor(KnownColor.HighlightText);

            this.ColorDetail = ConvertStringToColor(colorConverter, settingLocal.ColorDetailStr)
                ?? Color.FromKnownColor(KnownColor.ControlText);

            this.ColorDetailLink = ConvertStringToColor(colorConverter, settingLocal.ColorDetailLinkStr)
                ?? Color.FromKnownColor(KnownColor.Blue);

            this.ColorDetailBackcolor = ConvertStringToColor(colorConverter, settingLocal.ColorDetailBackcolorStr)
                ?? Color.FromKnownColor(KnownColor.Window);

            this.ColorSelf = ConvertStringToColor(colorConverter, settingLocal.ColorSelfStr)
                ?? Color.FromKnownColor(KnownColor.AliceBlue);

            this.ColorAtSelf = ConvertStringToColor(colorConverter, settingLocal.ColorAtSelfStr)
                ?? Color.FromKnownColor(KnownColor.AntiqueWhite);

            this.ColorTarget = ConvertStringToColor(colorConverter, settingLocal.ColorTargetStr)
                ?? Color.FromKnownColor(KnownColor.LemonChiffon);

            this.ColorAtTarget = ConvertStringToColor(colorConverter, settingLocal.ColorAtTargetStr)
                ?? Color.FromKnownColor(KnownColor.LavenderBlush);

            this.ColorAtFromTarget = ConvertStringToColor(colorConverter, settingLocal.ColorAtFromTargetStr)
                ?? Color.FromKnownColor(KnownColor.Honeydew);

            this.ColorAtTo = ConvertStringToColor(colorConverter, settingLocal.ColorAtToStr)
                ?? Color.FromKnownColor(KnownColor.Pink);

            this.ColorListBackcolor = ConvertStringToColor(colorConverter, settingLocal.ColorListBackcolorStr)
                ?? Color.FromKnownColor(KnownColor.Window);

            this.ColorInputBackcolor = ConvertStringToColor(colorConverter, settingLocal.ColorInputBackcolorStr)
                ?? Color.FromKnownColor(KnownColor.LemonChiffon);

            this.ColorInputFont = ConvertStringToColor(colorConverter, settingLocal.ColorInputFontStr)
                ?? Color.FromKnownColor(KnownColor.ControlText);

            this.BrushSelf = new SolidBrush(this.ColorSelf);
            this.BrushAtSelf = new SolidBrush(this.ColorAtSelf);
            this.BrushTarget = new SolidBrush(this.ColorTarget);
            this.BrushAtTarget = new SolidBrush(this.ColorAtTarget);
            this.BrushAtFromTarget = new SolidBrush(this.ColorAtFromTarget);
            this.BrushAtTo = new SolidBrush(this.ColorAtTo);
            this.BrushListBackcolor = new SolidBrush(this.ColorListBackcolor);
            this.BrushHighLight = new SolidBrush(Color.FromKnownColor(KnownColor.Highlight));
            this.BrushDeactiveSelection = new SolidBrush(Color.FromKnownColor(KnownColor.ButtonFace));
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.FontUnread.Dispose();
            this.FontUnreadBold.Dispose();
            this.FontReaded.Dispose();
            this.FontReadedBold.Dispose();
            this.FontDetail.Dispose();
            this.FontInputFont.Dispose();
            this.BrushSelf.Dispose();
            this.BrushAtSelf.Dispose();
            this.BrushTarget.Dispose();
            this.BrushAtTarget.Dispose();
            this.BrushAtFromTarget.Dispose();
            this.BrushAtTo.Dispose();
            this.BrushListBackcolor.Dispose();
            this.BrushHighLight.Dispose();
            this.BrushDeactiveSelection.Dispose();

            this.IsDisposed = true;
        }

        public static Font? ConvertStringToFont(FontConverter converter, string? fontStr)
        {
            if (MyCommon.IsNullOrEmpty(fontStr))
                return null;

            try
            {
                return (Font)converter.ConvertFromString(fontStr);
            }
            catch
            {
                return null;
            }
        }

        public static string? ConvertFontToString(FontConverter converter, Font font, Font defaultValue)
        {
            static bool Equals(Font font1, Font font2)
                => font1.Name == font2.Name && font1.Size == font2.Size && font1.Unit == font2.Unit && font1.Style == font2.Style;

            if (Equals(font, defaultValue))
                return null;

            return converter.ConvertToString(font);
        }

        public static Color? ConvertStringToColor(ColorConverter converter, string? colorStr)
        {
            if (MyCommon.IsNullOrEmpty(colorStr))
                return null;

            try
            {
                return (Color)converter.ConvertFromString(colorStr);
            }
            catch
            {
                return null;
            }
        }

        public static string? ConvertColorToString(ColorConverter converter, Color color, Color defaultValue)
        {
            if (color == defaultValue)
                return null;

            return converter.ConvertToString(color);
        }

        public static void ApplyGlobalUIFont(SettingLocal settingLocal)
        {
            var fontConverter = new FontConverter();
            var font = ConvertStringToFont(fontConverter, settingLocal.FontUIGlobalStr);
            if (font != null)
                OTBaseForm.GlobalFont = font;
        }
    }
}
