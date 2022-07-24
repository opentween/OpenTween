// OpenTween - Client of Twitter
// Copyright (c) 2014 spx (@5px)
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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace OpenTween
{
    public class OTSplitContainer : SplitContainer
    {
        /// <summary>
        /// Panel1 と Panel2 の中身が入れ替わった状態かどうかを取得または設定する。
        /// true が設定された場合、Panel に関連するプロパティの処理内容も入れ替わるので注意。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPanelInverted
        {
            get => this.isPanelInverted;
            set
            {
                if (this.isPanelInverted == value)
                    return;
                this.isPanelInverted = value;

                // Panel1 と Panel2 の中身を入れ替え
                using (ControlTransaction.Layout(this, false))
                using (ControlTransaction.Layout(base.Panel1, false))
                using (ControlTransaction.Layout(base.Panel2, false))
                {
                    var cont1 = new List<Control>(base.Panel1.Controls.Cast<Control>());
                    var cont2 = new List<Control>(base.Panel2.Controls.Cast<Control>());
                    base.Panel1.Controls.Clear();
                    base.Panel2.Controls.Clear();

                    // 関連するプロパティを反転させる
                    if (base.FixedPanel != FixedPanel.None)
                        base.FixedPanel = (base.FixedPanel == FixedPanel.Panel1) ? FixedPanel.Panel2 : FixedPanel.Panel1;

                    base.SplitterDistance = this.SplitterTotalWidth - (base.SplitterDistance + this.SplitterWidth);

                    (base.Panel2MinSize, base.Panel1MinSize) = (base.Panel1MinSize, base.Panel2MinSize);
                    (base.Panel2Collapsed, base.Panel1Collapsed) = (base.Panel1Collapsed, base.Panel2Collapsed);

                    base.Panel1.Controls.AddRange(cont2.ToArray());
                    base.Panel2.Controls.AddRange(cont1.ToArray());
                }
            }
        }

        private bool isPanelInverted = false;

        /// <summary>
        /// SplitContainer.Orientation プロパティの設定に応じて、スプリッタが移動する方向の幅を返す。
        /// </summary>
        private int SplitterTotalWidth
            => (base.Orientation == Orientation.Horizontal) ? base.Height : base.Width;

        /// <summary>
        /// IsPanelInverted プロパティの設定に応じて、SplitContainer.Panel1 または SplitContainer.Panel2 を返す。
        /// </summary>
        public new SplitterPanel Panel1
            => this.IsPanelInverted ? base.Panel2 : base.Panel1;

        /// <summary>
        /// IsPanelInverted プロパティの設定に応じて、SplitContainer.Panel1 または SplitContainer.Panel2 を返す。
        /// </summary>
        public new SplitterPanel Panel2
            => this.IsPanelInverted ? base.Panel1 : base.Panel2;

        /// <summary>
        /// IsPanelInverted プロパティの設定に応じて、SplitContainer.FixedPanel を返す。
        /// </summary>
        public new FixedPanel FixedPanel
        {
            get
            {
                if (base.FixedPanel != FixedPanel.None && this.IsPanelInverted)
                    return (base.FixedPanel == FixedPanel.Panel1) ? FixedPanel.Panel2 : FixedPanel.Panel1;
                else
                    return base.FixedPanel;
            }

            set
            {
                if (value != FixedPanel.None && this.IsPanelInverted)
                    base.FixedPanel = (value == FixedPanel.Panel1) ? FixedPanel.Panel2 : FixedPanel.Panel1;
                else
                    base.FixedPanel = value;
            }
        }

        /// <summary>
        /// IsPanelInverted プロパティの設定に応じて、SplitContainer.SplitterDistance を返す。
        /// </summary>
        public new int SplitterDistance
        {
            get
            {
                if (this.IsPanelInverted)
                    return this.SplitterTotalWidth - (base.SplitterDistance + this.SplitterWidth);
                else
                    return base.SplitterDistance;
            }

            set
            {
                if (this.IsPanelInverted)
                    base.SplitterDistance = this.SplitterTotalWidth - (value + this.SplitterWidth);
                else
                    base.SplitterDistance = value;
            }
        }

        /// <summary>
        /// IsPanelInverted プロパティの設定に応じて、SplitContainer.Panel1MinSize または SplitContainer.Panel2MinSize を返す。
        /// </summary>
        public new int Panel1MinSize
        {
            get => this.IsPanelInverted ? base.Panel2MinSize : base.Panel1MinSize;
            set
            {
                if (this.IsPanelInverted)
                    base.Panel2MinSize = value;
                else
                    base.Panel1MinSize = value;
            }
        }

        /// <summary>
        /// IsPanelInverted プロパティの設定に応じて、SplitContainer.Panel1MinSize または SplitContainer.Panel2MinSize を返す。
        /// </summary>
        public new int Panel2MinSize
        {
            get => this.IsPanelInverted ? base.Panel1MinSize : base.Panel2MinSize;
            set
            {
                if (this.IsPanelInverted)
                    base.Panel1MinSize = value;
                else
                    base.Panel2MinSize = value;
            }
        }

        /// <summary>
        /// IsPanelInverted プロパティの設定に応じて、SplitContainer.Panel1Collapsed または SplitContainer.Panel2Collapsed を返す。
        /// </summary>
        public new bool Panel1Collapsed
        {
            get => this.IsPanelInverted ? base.Panel2Collapsed : base.Panel1Collapsed;
            set
            {
                if (this.IsPanelInverted)
                    base.Panel2Collapsed = value;
                else
                    base.Panel1Collapsed = value;
            }
        }

        /// <summary>
        /// IsPanelInverted プロパティの設定に応じて、SplitContainer.Panel1Collapsed または SplitContainer.Panel2Collapsed を返す。
        /// </summary>
        public new bool Panel2Collapsed
        {
            get => this.IsPanelInverted ? base.Panel1Collapsed : base.Panel2Collapsed;
            set
            {
                if (this.IsPanelInverted)
                    base.Panel1Collapsed = value;
                else
                    base.Panel2Collapsed = value;
            }
        }
    }
}
