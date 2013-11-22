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
using OpenTween.Api;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class ToolStripAPIGaugeTest
    {
        [Fact]
        public void GaugeHeightTest()
        {
            using (var toolStrip = new ToolStripAPIGauge())
            {
                toolStrip.AutoSize = false;
                toolStrip.Size = new Size(100, 10);
                toolStrip.ApiLimit = new ApiLimit(150, 150, DateTime.MaxValue);

                toolStrip.GaugeHeight = 5;

                Assert.Equal(new Rectangle(0, 0, 100, 5), toolStrip.apiGaugeBounds);
                Assert.Equal(new Rectangle(0, 5, 100, 5), toolStrip.timeGaugeBounds);

                toolStrip.GaugeHeight = 3;

                Assert.Equal(new Rectangle(0, 2, 100, 3), toolStrip.apiGaugeBounds);
                Assert.Equal(new Rectangle(0, 5, 100, 3), toolStrip.timeGaugeBounds);

                toolStrip.GaugeHeight = 0;

                Assert.Equal(Rectangle.Empty, toolStrip.apiGaugeBounds);
                Assert.Equal(Rectangle.Empty, toolStrip.timeGaugeBounds);
            }
        }

        [Fact]
        public void TextTest()
        {
            using (var toolStrip = new ToolStripAPIGauge())
            {
                // toolStrip.ApiLimit の初期値は null

                Assert.Equal("API ???/???", toolStrip.Text);
                Assert.Equal("API rest ???/???" + Environment.NewLine + "(reset after ??? minutes)", toolStrip.ToolTipText);

                toolStrip.ApiLimit = new ApiLimit(15, 14, DateTime.Now.AddMinutes(15));

                Assert.Equal("API 14/15", toolStrip.Text);
                Assert.Equal("API rest 14/15" + Environment.NewLine + "(reset after 15 minutes)", toolStrip.ToolTipText);

                toolStrip.ApiLimit = null;

                Assert.Equal("API ???/???", toolStrip.Text);
                Assert.Equal("API rest ???/???" + Environment.NewLine + "(reset after ??? minutes)", toolStrip.ToolTipText);
            }
        }

        class TestToolStripAPIGauge : ToolStripAPIGauge
        {
            protected override void UpdateRemainMinutes()
            {
                if (this.ApiLimit != null)
                    // DateTime の代わりに Clock.Now を使用することで FreezeClock 属性の影響を受けるようになる
                    this.remainMinutes = (this.ApiLimit.AccessLimitResetDate - Clock.Now).TotalMinutes;
                else
                    this.remainMinutes = -1;
            }
        }

        [Fact]
        [FreezeClock]
        public void GaugeBoundsTest()
        {
            using (var toolStrip = new TestToolStripAPIGauge())
            {
                toolStrip.AutoSize = false;
                toolStrip.Size = new Size(100, 10);
                toolStrip.GaugeHeight = 5;

                // toolStrip.ApiLimit の初期値は null

                Assert.Equal(Rectangle.Empty, toolStrip.apiGaugeBounds);
                Assert.Equal(Rectangle.Empty, toolStrip.timeGaugeBounds);

                toolStrip.ApiLimit = new ApiLimit(150, 60, Clock.Now.AddMinutes(15));

                Assert.Equal(new Rectangle(0, 0, 40, 5), toolStrip.apiGaugeBounds); // 40% (60/150)
                Assert.Equal(new Rectangle(0, 5, 25, 5), toolStrip.timeGaugeBounds); // 25% (15/60)

                toolStrip.ApiLimit = null;

                Assert.Equal(Rectangle.Empty, toolStrip.apiGaugeBounds);
                Assert.Equal(Rectangle.Empty, toolStrip.timeGaugeBounds);
            }
        }
    }
}
