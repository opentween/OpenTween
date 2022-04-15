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
    public class SettingLocal : SettingBase<SettingLocal>
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

        [XmlIgnore]
        public int[] ColumnsWidth { get; } = { 48, 80, 290, 120, 50, 16, 32, 50 };

        public int Width1
        {
            get => this.ColumnsWidth[0];
            set => this.ColumnsWidth[0] = value;
        }

        public int Width2
        {
            get => this.ColumnsWidth[1];
            set => this.ColumnsWidth[1] = value;
        }

        public int Width3
        {
            get => this.ColumnsWidth[2];
            set => this.ColumnsWidth[2] = value;
        }

        public int Width4
        {
            get => this.ColumnsWidth[3];
            set => this.ColumnsWidth[3] = value;
        }

        public int Width5
        {
            get => this.ColumnsWidth[4];
            set => this.ColumnsWidth[4] = value;
        }

        public int Width6
        {
            get => this.ColumnsWidth[5];
            set => this.ColumnsWidth[5] = value;
        }

        public int Width7
        {
            get => this.ColumnsWidth[6];
            set => this.ColumnsWidth[6] = value;
        }

        public int Width8
        {
            get => this.ColumnsWidth[7];
            set => this.ColumnsWidth[7] = value;
        }

        [XmlIgnore]
        public int[] ColumnsOrder { get; } = { 2, 3, 4, 5, 6, 1, 0, 7 };

        public int DisplayIndex1
        {
            get => this.ColumnsOrder[0];
            set => this.ColumnsOrder[0] = value;
        }

        public int DisplayIndex2
        {
            get => this.ColumnsOrder[1];
            set => this.ColumnsOrder[1] = value;
        }

        public int DisplayIndex3
        {
            get => this.ColumnsOrder[2];
            set => this.ColumnsOrder[2] = value;
        }

        public int DisplayIndex4
        {
            get => this.ColumnsOrder[3];
            set => this.ColumnsOrder[3] = value;
        }

        public int DisplayIndex5
        {
            get => this.ColumnsOrder[4];
            set => this.ColumnsOrder[4] = value;
        }

        public int DisplayIndex6
        {
            get => this.ColumnsOrder[5];
            set => this.ColumnsOrder[5] = value;
        }

        public int DisplayIndex7
        {
            get => this.ColumnsOrder[6];
            set => this.ColumnsOrder[6] = value;
        }

        public int DisplayIndex8
        {
            get => this.ColumnsOrder[7];
            set => this.ColumnsOrder[7] = value;
        }

        public string BrowserPath = "";
        public ProxyType ProxyType = ProxyType.IE;
        public string ProxyAddress = "127.0.0.1";
        public int ProxyPort = 80;
        public string ProxyUser = "";
        public bool StatusMultiline = false;
        public int StatusTextHeight = 38;
        public int PreviewDistance = -1;

        public string? FontUnreadStr { get; set; }

        public string? ColorUnreadStr { get; set; }

        public string? FontReadStr { get; set; }

        public string? ColorReadStr { get; set; }

        public string? ColorFavStr { get; set; }

        public string? ColorOWLStr { get; set; }

        public string? ColorRetweetStr { get; set; }

        public string? FontDetailStr { get; set; }

        public string? ColorSelfStr { get; set; }

        public string? ColorAtSelfStr { get; set; }

        public string? ColorTargetStr { get; set; }

        public string? ColorAtTargetStr { get; set; }

        public string? ColorAtFromTargetStr { get; set; }

        public string? ColorAtToStr { get; set; }

        public string? ColorInputBackcolorStr { get; set; }

        public string? ColorInputFontStr { get; set; }

        public string? FontInputFontStr { get; set; }

        public string? ColorListBackcolorStr { get; set; }

        public string? ColorDetailBackcolorStr { get; set; }

        public string? ColorDetailStr { get; set; }

        public string? ColorDetailLinkStr { get; set; }

        /// <summary>
        /// [隠し設定] UI フォントを指定します
        /// </summary>
        /// <remarks>
        /// フォントによっては一部レイアウトが崩れるためこっそり追加
        /// </remarks>
        public string? FontUIGlobalStr { get; set; }

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

        /// <summary>
        /// 指定されたスケールと SettingLocal.ScaleDimension のスケールとの拡大比を返します
        /// </summary>
        public SizeF GetConfigScaleFactor(SizeF currentSizeDimension)
            => new(
                currentSizeDimension.Width / this.ScaleDimension.Width,
                currentSizeDimension.Height / this.ScaleDimension.Height);
    }
}
