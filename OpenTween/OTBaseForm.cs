// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenTween
{
    /// <summary>
    /// OpenTween で使用する全てのフォームの基底となるクラス
    /// </summary>
    public class OTBaseForm : Form
    {
        /// <summary>
        /// 全てのフォームで共通して使用する UI フォント
        /// </summary>
        /// <remarks>
        /// SettingLocal.xml に FontUIGlobalStr 要素を追加する事で変更できます
        /// </remarks>
        public static Font GlobalFont { get; set; }

        /// <summary>
        /// デザイン時のスケールと現在のスケールの比
        /// </summary>
        /// <remarks>
        /// 例えば、デザイン時が 96 dpi (96.0, 96.0) で実行時が 120dpi (120.0, 120.0) の場合は 1.25, 1.25 が返ります
        /// </remarks>
        public SizeF CurrentScaleFactor { get; private set; }

        protected OTBaseForm()
            : base()
        {
            this.CurrentScaleFactor = new SizeF(1.0f, 1.0f);

            this.Load += (o, e) =>
            {
                // デフォルトの UI フォントを変更
                if (OTBaseForm.GlobalFont != null)
                    this.Font = OTBaseForm.GlobalFont;
            };
        }

        /// <summary>
        /// source で指定されたフォントのスタイルを維持しつつ GlobalFont に置き換えた Font を返します
        /// </summary>
        protected Font ReplaceToGlobalFont(Font source)
        {
            if (OTBaseForm.GlobalFont == null)
                return source;

            return new Font(OTBaseForm.GlobalFont.Name, source.Size, source.Style);
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            const float baseDpi = 96.0f;

            this.CurrentScaleFactor = new SizeF(
                this.CurrentAutoScaleDimensions.Width / baseDpi,
                this.CurrentAutoScaleDimensions.Height / baseDpi);
        }

        /// <summary>
        /// 標準の ListView のスケーリングでは不十分な処理を補います
        /// </summary>
        public static void ScaleChildControl(ListView listview, SizeF factor)
        {
            // カラム幅
            foreach (ColumnHeader col in listview.Columns)
            {
                col.Width = ScaleBy(factor.Width, col.Width);
            }
        }

        /// <summary>
        /// 標準の VScrollBar のスケーリングでは不十分な処理を補います
        /// </summary>
        public static void ScaleChildControl(VScrollBar scrollBar, SizeF factor)
        {
            scrollBar.Width = ScaleBy(factor.Width, scrollBar.Width);
        }

        /// <summary>
        /// 標準の ImageList のスケーリングでは不十分な処理を補います
        /// </summary>
        public static void ScaleChildControl(ImageList imageList, SizeF factor)
        {
            imageList.ImageSize = ScaleBy(factor, imageList.ImageSize);
        }

        public static Size ScaleBy(SizeF factor, Size size)
        {
            return Size.Round(new SizeF(size.Width * factor.Width, size.Height * factor.Height));
        }

        public static int ScaleBy(float factor, int size)
        {
            return (int)Math.Round(size * factor);
        }
    }
}
