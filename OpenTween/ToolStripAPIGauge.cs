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

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using OpenTween.Api;

namespace OpenTween
{
    /// <summary>
    /// API 実行回数制限に到達するまでの目安を表示する ToolStripItem
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
    public class ToolStripAPIGauge : ToolStripStatusLabel
    {
        public ToolStripAPIGauge()
        {
            this.UpdateText();

            this.DisplayStyle = ToolStripItemDisplayStyle.Text;
        }

        /// <summary>
        /// ゲージに表示される横棒グラフの幅
        /// </summary>
        [DefaultValue(8)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int GaugeHeight
        {
            get => this.gaugeHeight;
            set
            {
                this.gaugeHeight = value;

                this.UpdateGaugeBounds();
                this.Invalidate();
            }
        }

        private int gaugeHeight = 8;

        /// <summary>
        /// API 実行回数制限の値
        /// </summary>
        [Browsable(false)]
        public ApiLimit? ApiLimit
        {
            get => this.apiLimit;
            private set
            {
                this.apiLimit = value;

                this.UpdateRemainMinutes();
                this.UpdateText();
                this.UpdateGaugeBounds();
                this.Invalidate();
            }
        }

        private ApiLimit? apiLimit = null;

        /// <summary>
        /// API エンドポイント名
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ApiEndpoint
        {
            get => this.apiEndpoint;
            set
            {
                if (MyCommon.IsNullOrEmpty(value))
                {
                    // リセット
                    this.apiEndpoint = null;
                    this.ApiLimit = null;
                    return;
                }

                var apiLimit = MyCommon.TwitterApiInfo.AccessLimit[value];

                if (this.apiEndpoint != value)
                {
                    // ApiEndpointが変更されているので更新する
                    this.apiEndpoint = value;
                    this.ApiLimit = apiLimit;
                }
                else
                {
                    // ApiLimitが変更されていれば更新する
                    if (this.apiLimit == null ||
                        !this.apiLimit.Equals(apiLimit))
                    {
                        this.ApiLimit = apiLimit;
                    }
                }
            }
        }

        private string? apiEndpoint = null;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string ToolTipText
        {
            get => base.ToolTipText;
            set => base.ToolTipText = value;
        }

        [DefaultValue(ToolStripItemDisplayStyle.Text)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public new ToolStripItemDisplayStyle DisplayStyle
        {
            get => base.DisplayStyle;
            set => base.DisplayStyle = value;
        }

        protected double remainMinutes = -1;

        protected virtual void UpdateRemainMinutes()
        {
            if (this.apiLimit != null)
                this.remainMinutes = (this.apiLimit.AccessLimitResetDate - DateTimeUtc.Now).TotalMinutes;
            else
                this.remainMinutes = -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            if (this.ApiGaugeBounds != Rectangle.Empty)
                g.FillRectangle(Brushes.LightBlue, this.ApiGaugeBounds);

            if (this.TimeGaugeBounds != Rectangle.Empty)
                g.FillRectangle(Brushes.LightPink, this.TimeGaugeBounds);

            base.OnPaint(e);
        }

        #region from Tween v1.1.0.0

        // The code in this region block is based on code written by the following authors:
        //   (C) 2010 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
        //   (C) 2010 Moz (@syo68k)

        internal Rectangle ApiGaugeBounds = Rectangle.Empty;
        internal Rectangle TimeGaugeBounds = Rectangle.Empty;

        protected virtual void UpdateGaugeBounds()
        {
            if (this.apiLimit == null || this.gaugeHeight < 1)
            {
                this.ApiGaugeBounds = Rectangle.Empty;
                this.TimeGaugeBounds = Rectangle.Empty;
                return;
            }

            var apiGaugeValue = (double)this.apiLimit.AccessLimitRemain / this.apiLimit.AccessLimitCount;
            this.ApiGaugeBounds = new Rectangle(
                0,
                (this.Height - this.gaugeHeight * 2) / 2,
                (int)(this.Width * apiGaugeValue),
                this.gaugeHeight
            );

            var timeGaugeValue = this.remainMinutes >= 15 ? 1.00 : this.remainMinutes / 15;
            this.TimeGaugeBounds = new Rectangle(
                0,
                this.ApiGaugeBounds.Top + this.gaugeHeight,
                (int)(this.Width * timeGaugeValue),
                this.gaugeHeight
            );
        }

        protected virtual void UpdateText()
        {
            string remainCountText;
            string maxCountText;
            string minuteText;

            if (this.apiLimit == null || this.remainMinutes < 0)
            {
                remainCountText = "???";
                maxCountText = "???";
                minuteText = "???";
            }
            else
            {
                remainCountText = this.apiLimit.AccessLimitRemain.ToString();
                maxCountText = this.apiLimit.AccessLimitCount.ToString();
                minuteText = Math.Ceiling(this.remainMinutes).ToString();
            }

            var endpointText = MyCommon.IsNullOrEmpty(this.apiEndpoint) ? "unknown" : this.apiEndpoint;

            var textFormat = "API {0}/{1}";
            this.Text = string.Format(textFormat, remainCountText, maxCountText);

            var toolTipTextFormat =
                "API rest {0} {1}/{2}" + Environment.NewLine +
                "(reset after {3} minutes)";

            this.ToolTipText = string.Format(toolTipTextFormat, endpointText, remainCountText, maxCountText, minuteText);
        }

        #endregion
    }
}
