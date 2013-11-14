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
        public static Font GlobalFont { get; set; }

        static OTBaseForm()
        {
            GlobalFont = SystemFonts.MessageBoxFont;
        }

        protected OTBaseForm()
            : base()
        {
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
    }
}
