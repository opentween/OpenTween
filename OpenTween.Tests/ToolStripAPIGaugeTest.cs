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
        [FreezeClock]
        public void ApiEndpointTest()
        {
            using (var toolStrip = new ToolStripAPIGauge())
            {
                MyCommon.TwitterApiInfo.AccessLimit["endpoint1"] = new ApiLimit(15, 15, Clock.Now.AddMinutes(15));
                MyCommon.TwitterApiInfo.AccessLimit["endpoint2"] = new ApiLimit(180, 18, Clock.Now.AddMinutes(5));

                // toolStrip.ApiEndpoint の初期値は null

                Assert.Equal(null, toolStrip.ApiEndpoint);
                Assert.Equal(null, toolStrip.ApiLimit);

                toolStrip.ApiEndpoint = "endpoint1";

                Assert.Equal("endpoint1", toolStrip.ApiEndpoint);
                Assert.Equal(new ApiLimit(15, 15, Clock.Now.AddMinutes(15)), toolStrip.ApiLimit);

                toolStrip.ApiEndpoint = "endpoint2";

                Assert.Equal("endpoint2", toolStrip.ApiEndpoint);
                Assert.Equal(new ApiLimit(180, 18, Clock.Now.AddMinutes(5)), toolStrip.ApiLimit);

                MyCommon.TwitterApiInfo.AccessLimit["endpoint2"] = new ApiLimit(180, 17, Clock.Now.AddMinutes(5));
                toolStrip.ApiEndpoint = "endpoint2";

                Assert.Equal("endpoint2", toolStrip.ApiEndpoint);
                Assert.Equal(new ApiLimit(180, 17, Clock.Now.AddMinutes(5)), toolStrip.ApiLimit);

                toolStrip.ApiEndpoint = "hoge";

                Assert.Equal("hoge", toolStrip.ApiEndpoint);
                Assert.Equal(null, toolStrip.ApiLimit);

                toolStrip.ApiEndpoint = "";

                Assert.Equal(null, toolStrip.ApiEndpoint);
                Assert.Equal(null, toolStrip.ApiLimit);

                MyCommon.TwitterApiInfo.AccessLimit.Clear();
            }
        }

        [Fact]
        public void GaugeHeightTest()
        {
            using (var toolStrip = new ToolStripAPIGauge())
            {
                toolStrip.AutoSize = false;
                toolStrip.Size = new Size(100, 10);

                MyCommon.TwitterApiInfo.AccessLimit["endpoint"] = new ApiLimit(15, 15, DateTime.MaxValue);
                toolStrip.ApiEndpoint = "endpoint";

                toolStrip.GaugeHeight = 5;

                Assert.Equal(new Rectangle(0, 0, 100, 5), toolStrip.apiGaugeBounds);
                Assert.Equal(new Rectangle(0, 5, 100, 5), toolStrip.timeGaugeBounds);

                toolStrip.GaugeHeight = 3;

                Assert.Equal(new Rectangle(0, 2, 100, 3), toolStrip.apiGaugeBounds);
                Assert.Equal(new Rectangle(0, 5, 100, 3), toolStrip.timeGaugeBounds);

                toolStrip.GaugeHeight = 0;

                Assert.Equal(Rectangle.Empty, toolStrip.apiGaugeBounds);
                Assert.Equal(Rectangle.Empty, toolStrip.timeGaugeBounds);

                MyCommon.TwitterApiInfo.AccessLimit.Clear();
            }
        }

        [Fact]
        public void TextTest()
        {
            using (var toolStrip = new ToolStripAPIGauge())
            {
                MyCommon.TwitterApiInfo.AccessLimit["/statuses/home_timeline"] = new ApiLimit(15, 15, DateTime.Now.AddMinutes(15));
                MyCommon.TwitterApiInfo.AccessLimit["/statuses/user_timeline"] = new ApiLimit(180, 18, DateTime.Now.AddMinutes(-2));
                MyCommon.TwitterApiInfo.AccessLimit["/search/tweets"] = new ApiLimit(180, 90, DateTime.Now.AddMinutes(5));

                // toolStrip.ApiEndpoint の初期値は null

                Assert.Equal("API ???/???", toolStrip.Text);
                Assert.Equal("API rest unknown ???/???" + Environment.NewLine + "(reset after ??? minutes)", toolStrip.ToolTipText);

                toolStrip.ApiEndpoint = "/search/tweets";

                Assert.Equal("API 90/180", toolStrip.Text);
                Assert.Equal("API rest /search/tweets 90/180" + Environment.NewLine + "(reset after 5 minutes)", toolStrip.ToolTipText);

                toolStrip.ApiEndpoint = "/statuses/user_timeline";

                Assert.Equal("API ???/???", toolStrip.Text);
                Assert.Equal("API rest /statuses/user_timeline ???/???" + Environment.NewLine + "(reset after ??? minutes)", toolStrip.ToolTipText);

                MyCommon.TwitterApiInfo.AccessLimit["/statuses/user_timeline"] = new ApiLimit(180, 180, DateTime.Now.AddMinutes(15));
                toolStrip.ApiEndpoint = "/statuses/user_timeline";

                Assert.Equal("API 180/180", toolStrip.Text);
                Assert.Equal("API rest /statuses/user_timeline 180/180" + Environment.NewLine + "(reset after 15 minutes)", toolStrip.ToolTipText);

                MyCommon.TwitterApiInfo.AccessLimit["/statuses/user_timeline"] = new ApiLimit(180, 179, DateTime.Now.AddMinutes(15));
                toolStrip.ApiEndpoint = "/statuses/user_timeline";

                Assert.Equal("API 179/180", toolStrip.Text);
                Assert.Equal("API rest /statuses/user_timeline 179/180" + Environment.NewLine + "(reset after 15 minutes)", toolStrip.ToolTipText);

                toolStrip.ApiEndpoint = "hoge";

                Assert.Equal("API ???/???", toolStrip.Text);
                Assert.Equal("API rest hoge ???/???" + Environment.NewLine + "(reset after ??? minutes)", toolStrip.ToolTipText);

                toolStrip.ApiEndpoint = "";

                Assert.Equal("API ???/???", toolStrip.Text);
                Assert.Equal("API rest unknown ???/???" + Environment.NewLine + "(reset after ??? minutes)", toolStrip.ToolTipText);

                MyCommon.TwitterApiInfo.AccessLimit.Clear();
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

                // toolStrip.ApiEndpoint の初期値は null

                Assert.Equal(Rectangle.Empty, toolStrip.apiGaugeBounds);
                Assert.Equal(Rectangle.Empty, toolStrip.timeGaugeBounds);

                MyCommon.TwitterApiInfo.AccessLimit["endpoint"] = new ApiLimit(150, 60, Clock.Now.AddMinutes(3));
                toolStrip.ApiEndpoint = "endpoint";

                Assert.Equal(new Rectangle(0, 0, 40, 5), toolStrip.apiGaugeBounds); // 40% (60/150)
                Assert.Equal(new Rectangle(0, 5, 20, 5), toolStrip.timeGaugeBounds); // 20% (3/15)

                toolStrip.ApiEndpoint = "";

                Assert.Equal(Rectangle.Empty, toolStrip.apiGaugeBounds);
                Assert.Equal(Rectangle.Empty, toolStrip.timeGaugeBounds);

                MyCommon.TwitterApiInfo.AccessLimit.Clear();
            }
        }
    }
}
