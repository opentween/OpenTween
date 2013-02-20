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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Design;
using System.ComponentModel;
using OpenTween.Api;

namespace OpenTween
{
    /// <summary>
    /// API 実行回数制限に到達するまでの目安を表示する ToolStripItem
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
    public class ToolStripAPIGauge : ToolStripButton
    {
        public ToolStripAPIGauge()
            : base()
        {
            this.Text = "API ???/???";
            this.ToolTipText = "API rest ???/???" + Environment.NewLine + "(reset after ??? minutes)";

            this.DisplayStyle = ToolStripItemDisplayStyle.Text;
        }

        /// <summary>
        /// ゲージに表示される横棒グラフの幅
        /// </summary>
        [DefaultValue(8)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int GaugeHeight
        {
            get { return this._GaugeHeight; }
            set
            {
                this._GaugeHeight = value;

                this.UpdateGaugeBounds();
                this.Invalidate();
            }
        }
        private int _GaugeHeight = 8;

        /// <summary>
        /// API 実行回数制限の値
        /// </summary>
        [Browsable(false)]
        public ApiLimit ApiLimit
        {
            get { return this._ApiLimit; }
            set
            {
                this._ApiLimit = value;

                this.UpdateRemainMinutes();
                this.UpdateText();
                this.UpdateGaugeBounds();
                this.Invalidate();
            }
        }
        private ApiLimit _ApiLimit = null;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string ToolTipText
        {
            get { return base.ToolTipText; }
            set { base.ToolTipText = value; }
        }

        [DefaultValue(ToolStripItemDisplayStyle.Text)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public new ToolStripItemDisplayStyle DisplayStyle
        {
            get { return base.DisplayStyle; }
            set { base.DisplayStyle = value; }
        }

        protected double remainMinutes = -1;

        protected virtual void UpdateRemainMinutes()
        {
            if (this._ApiLimit != null)
                this.remainMinutes = (this._ApiLimit.AccessLimitResetDate - DateTime.Now).TotalMinutes;
            else
                this.remainMinutes = -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            if (this.apiGaugeBounds != Rectangle.Empty)
                g.FillRectangle(Brushes.LightBlue, this.apiGaugeBounds);

            if (this.timeGaugeBounds != Rectangle.Empty)
                g.FillRectangle(Brushes.LightPink, this.timeGaugeBounds);

            base.OnPaint(e);
        }

        #region from Tween v1.1.0.0

        // The code in this region block is based on code written by the following authors:
        //   (C) 2010 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
        //   (C) 2010 Moz (@syo68k)

        internal Rectangle apiGaugeBounds = Rectangle.Empty;
        internal Rectangle timeGaugeBounds = Rectangle.Empty;

        protected virtual void UpdateGaugeBounds()
        {
            if (this._ApiLimit == null || this._GaugeHeight < 1)
            {
                this.apiGaugeBounds = Rectangle.Empty;
                this.timeGaugeBounds = Rectangle.Empty;
                return;
            }

            var apiGaugeValue = (double)this._ApiLimit.AccessLimitRemain / this._ApiLimit.AccessLimitCount;
            this.apiGaugeBounds = new Rectangle(
                0,
                (this.Height - this._GaugeHeight * 2) / 2,
                (int)(this.Width * apiGaugeValue),
                this._GaugeHeight
            );

            var timeGaugeValue = this.remainMinutes >= 60 ? 1.00 : this.remainMinutes / 60;
            this.timeGaugeBounds = new Rectangle(
                0,
                this.apiGaugeBounds.Top + this._GaugeHeight,
                (int)(this.Width * timeGaugeValue),
                this._GaugeHeight
            );
        }

        protected virtual void UpdateText()
        {
            string remainCountText;
            string maxCountText;
            string minuteText;

            if (this._ApiLimit == null)
            {
                remainCountText = "???";
                maxCountText = "???";
                minuteText = "???";
            }
            else
            {
                remainCountText = this._ApiLimit.AccessLimitRemain.ToString();
                maxCountText = this._ApiLimit.AccessLimitCount.ToString();
                minuteText = Math.Ceiling(this.remainMinutes).ToString();
            }

            var textFormat = "API {0}/{1}";
            this.Text = string.Format(textFormat, remainCountText, maxCountText);

            var toolTipTextFormat =
                "API rest {0}/{1}" + Environment.NewLine +
                "(reset after {2} minutes)";

            this.ToolTipText = String.Format(toolTipTextFormat, remainCountText, maxCountText, minuteText);
        }

        #endregion
    }
}
