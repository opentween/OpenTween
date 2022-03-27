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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using OpenTween.Connection;

namespace OpenTween
{
    public class SettingLocal : SettingBase<SettingLocal>, IDisposable
    {
        #region Settingクラス基本
        public static SettingLocal Load()
            => LoadSettings();

        public void Save()
            => SaveSettings(this);
        #endregion

        /// <summary>
        /// ウィンドウサイズ等の保存時のDPI
        /// </summary>
        public SizeF ScaleDimension = SizeF.Empty;

        public Point FormLocation = new(0, 0);
        public int SplitterDistance = 200;
        public Size FormSize = new(600, 500);

        /// <summary>
        /// 文末ステータス
        /// </summary>
        public string StatusText = "";

        public bool UseRecommendStatus = false;
        public int Width1 = 48;
        public int Width2 = 80;
        public int Width3 = 290;
        public int Width4 = 120;
        public int Width5 = 50;
        public int Width6 = 16;
        public int Width7 = 32;
        public int Width8 = 50;
        public int DisplayIndex1 = 2;
        public int DisplayIndex2 = 3;
        public int DisplayIndex3 = 4;
        public int DisplayIndex4 = 5;
        public int DisplayIndex5 = 6;
        public int DisplayIndex6 = 1;
        public int DisplayIndex7 = 0;
        public int DisplayIndex8 = 7;
        public string BrowserPath = "";
        public ProxyType ProxyType = ProxyType.IE;
        public string ProxyAddress = "127.0.0.1";
        public int ProxyPort = 80;
        public string ProxyUser = "";
        public bool StatusMultiline = false;
        public int StatusTextHeight = 38;
        public int PreviewDistance = -1;

        [XmlIgnore]
        public Font FontUnread = new(SystemFonts.DefaultFont, FontStyle.Bold | FontStyle.Underline);

        public string FontUnreadStr
        {
            get => this.FontToString(this.FontUnread);
            set => this.FontUnread = this.StringToFont(value);
        }

        [XmlIgnore]
        public Color ColorUnread = System.Drawing.SystemColors.ControlText;

        public string ColorUnreadStr
        {
            get => this.ColorToString(this.ColorUnread);
            set => this.ColorUnread = this.StringToColor(value);
        }

        [XmlIgnore]
        public Font FontRead = System.Drawing.SystemFonts.DefaultFont;

        public string FontReadStr
        {
            get => this.FontToString(this.FontRead);
            set => this.FontRead = this.StringToFont(value);
        }

        [XmlIgnore]
        public Color ColorRead = System.Drawing.SystemColors.ControlText;

        public string ColorReadStr
        {
            get => this.ColorToString(this.ColorRead);
            set => this.ColorRead = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorFav = Color.FromKnownColor(System.Drawing.KnownColor.Red);

        public string ColorFavStr
        {
            get => this.ColorToString(this.ColorFav);
            set => this.ColorFav = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorOWL = Color.FromKnownColor(System.Drawing.KnownColor.Blue);

        public string ColorOWLStr
        {
            get => this.ColorToString(this.ColorOWL);
            set => this.ColorOWL = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorRetweet = Color.FromKnownColor(System.Drawing.KnownColor.Green);

        public string ColorRetweetStr
        {
            get => this.ColorToString(this.ColorRetweet);
            set => this.ColorRetweet = this.StringToColor(value);
        }

        [XmlIgnore]
        public Font FontDetail = System.Drawing.SystemFonts.DefaultFont;

        public string FontDetailStr
        {
            get => this.FontToString(this.FontDetail);
            set => this.FontDetail = this.StringToFont(value);
        }

        [XmlIgnore]
        public Color ColorSelf = Color.FromKnownColor(System.Drawing.KnownColor.AliceBlue);

        public string ColorSelfStr
        {
            get => this.ColorToString(this.ColorSelf);
            set => this.ColorSelf = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorAtSelf = Color.FromKnownColor(System.Drawing.KnownColor.AntiqueWhite);

        public string ColorAtSelfStr
        {
            get => this.ColorToString(this.ColorAtSelf);
            set => this.ColorAtSelf = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorTarget = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon);

        public string ColorTargetStr
        {
            get => this.ColorToString(this.ColorTarget);
            set => this.ColorTarget = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorAtTarget = Color.FromKnownColor(System.Drawing.KnownColor.LavenderBlush);

        public string ColorAtTargetStr
        {
            get => this.ColorToString(this.ColorAtTarget);
            set => this.ColorAtTarget = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorAtFromTarget = Color.FromKnownColor(System.Drawing.KnownColor.Honeydew);

        public string ColorAtFromTargetStr
        {
            get => this.ColorToString(this.ColorAtFromTarget);
            set => this.ColorAtFromTarget = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorAtTo = Color.FromKnownColor(System.Drawing.KnownColor.Pink);

        public string ColorAtToStr
        {
            get => this.ColorToString(this.ColorAtTo);
            set => this.ColorAtTo = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorInputBackcolor = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon);

        public string ColorInputBackcolorStr
        {
            get => this.ColorToString(this.ColorInputBackcolor);
            set => this.ColorInputBackcolor = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorInputFont = Color.FromKnownColor(System.Drawing.KnownColor.ControlText);

        public string ColorInputFontStr
        {
            get => this.ColorToString(this.ColorInputFont);
            set => this.ColorInputFont = this.StringToColor(value);
        }

        [XmlIgnore]
        public Font FontInputFont = System.Drawing.SystemFonts.DefaultFont;

        public string FontInputFontStr
        {
            get => this.FontToString(this.FontInputFont);
            set => this.FontInputFont = this.StringToFont(value);
        }

        [XmlIgnore]
        public Color ColorListBackcolor = Color.FromKnownColor(System.Drawing.KnownColor.Window);

        public string ColorListBackcolorStr
        {
            get => this.ColorToString(this.ColorListBackcolor);
            set => this.ColorListBackcolor = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorDetailBackcolor = Color.FromKnownColor(System.Drawing.KnownColor.Window);

        public string ColorDetailBackcolorStr
        {
            get => this.ColorToString(this.ColorDetailBackcolor);
            set => this.ColorDetailBackcolor = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorDetail = Color.FromKnownColor(System.Drawing.KnownColor.ControlText);

        public string ColorDetailStr
        {
            get => this.ColorToString(this.ColorDetail);
            set => this.ColorDetail = this.StringToColor(value);
        }

        [XmlIgnore]
        public Color ColorDetailLink = Color.FromKnownColor(System.Drawing.KnownColor.Blue);

        public string ColorDetailLinkStr
        {
            get => this.ColorToString(this.ColorDetailLink);
            set => this.ColorDetailLink = this.StringToColor(value);
        }

        [XmlIgnore]
        public Font? FontUIGlobal = null;

        /// <summary>
        /// [隠し設定] UI フォントを指定します
        /// </summary>
        /// <remarks>
        /// フォントによっては一部レイアウトが崩れるためこっそり追加
        /// </remarks>
        public string? FontUIGlobalStr
        {
            get => this.FontUIGlobal != null ? this.FontToString(this.FontUIGlobal) : null;
            set => this.FontUIGlobal = value != null ? this.StringToFont(value) : null;
        }

        [XmlIgnore]
        public string ProxyPassword = "";

        public string EncryptProxyPassword
        {
            get
            {
                var pwd = this.ProxyPassword;
                if (MyCommon.IsNullOrEmpty(pwd)) pwd = "";
                if (pwd.Length > 0)
                {
                    try
                    {
                        return MyCommon.EncryptString(pwd);
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }

            set
            {
                var pwd = value;
                if (MyCommon.IsNullOrEmpty(pwd)) pwd = "";
                if (pwd.Length > 0)
                {
                    try
                    {
                        pwd = MyCommon.DecryptString(pwd);
                    }
                    catch (Exception)
                    {
                        pwd = "";
                    }
                }
                this.ProxyPassword = pwd;
            }
        }

        /// <summary>
        /// 絵文字の表示に Twemoji (https://github.com/twitter/twemoji) を使用するか
        /// </summary>
        public bool UseTwemoji = true;

        [XmlIgnore]
        private readonly FontConverter fontConverter = new();

        protected string FontToString(Font font)
            => this.fontConverter.ConvertToString(font);

        protected Font StringToFont(string str)
            => (Font)this.fontConverter.ConvertFromString(str);

        [XmlIgnore]
        private readonly ColorConverter colorConverter = new();

        protected string ColorToString(Color color)
            => this.colorConverter.ConvertToString(color);

        protected Color StringToColor(string str)
            => (Color)this.colorConverter.ConvertFromString(str);

        /// <summary>
        /// 指定されたスケールと SettingLocal.ScaleDimension のスケールとの拡大比を返します
        /// </summary>
        public SizeF GetConfigScaleFactor(SizeF currentSizeDimension)
            => new(
                currentSizeDimension.Width / this.ScaleDimension.Width,
                currentSizeDimension.Height / this.ScaleDimension.Height);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.FontUnread?.Dispose();
                this.FontRead?.Dispose();
                this.FontDetail?.Dispose();
                this.FontInputFont?.Dispose();
            }
        }
    }
}
