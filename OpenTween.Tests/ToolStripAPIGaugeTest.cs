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
using NUnit.Framework;
using OpenTween.Api;

namespace OpenTween
{
    [TestFixture]
    class ToolStripAPIGaugeTest
    {
        [Test]
        public void GaugeHeightTest()
        {
            using (var toolStrip = new ToolStripAPIGauge())
            {
                toolStrip.AutoSize = false;
                toolStrip.Size = new Size(100, 10);
                toolStrip.ApiLimit = new ApiLimit(150, 150, DateTime.MaxValue);

                toolStrip.GaugeHeight = 5;

                Assert.That(toolStrip.apiGaugeBounds, Is.EqualTo(new Rectangle(0, 0, 100, 5)));
                Assert.That(toolStrip.timeGaugeBounds, Is.EqualTo(new Rectangle(0, 5, 100, 5)));

                toolStrip.GaugeHeight = 3;

                Assert.That(toolStrip.apiGaugeBounds, Is.EqualTo(new Rectangle(0, 2, 100, 3)));
                Assert.That(toolStrip.timeGaugeBounds, Is.EqualTo(new Rectangle(0, 5, 100, 3)));

                toolStrip.GaugeHeight = 0;

                Assert.That(toolStrip.apiGaugeBounds, Is.EqualTo(Rectangle.Empty));
                Assert.That(toolStrip.timeGaugeBounds, Is.EqualTo(Rectangle.Empty));
            }
        }

        [Test]
        public void TextTest()
        {
            using (var toolStrip = new ToolStripAPIGauge())
            {
                // toolStrip.ApiLimit の初期値は null

                Assert.That(toolStrip.Text, Is.EqualTo("API ???/???"));
                Assert.That(toolStrip.ToolTipText, Is.EqualTo("API rest ???/???" + Environment.NewLine + "(reset after ??? minutes)"));

                toolStrip.ApiLimit = new ApiLimit(150, 100, DateTime.Now.AddMinutes(10));

                Assert.That(toolStrip.Text, Is.EqualTo("API 100/150"));
                Assert.That(toolStrip.ToolTipText, Is.EqualTo("API rest 100/150" + Environment.NewLine + "(reset after 10 minutes)"));

                toolStrip.ApiLimit = null;

                Assert.That(toolStrip.Text, Is.EqualTo("API ???/???"));
                Assert.That(toolStrip.ToolTipText, Is.EqualTo("API rest ???/???" + Environment.NewLine + "(reset after ??? minutes)"));
            }
        }

        class TestToolStripAPIGauge : ToolStripAPIGauge
        {
            public DateTime Now { get; set; } // 現在時刻

            protected override void UpdateRemainMinutes()
            {
                if (this.ApiLimit != null)
                    this.remainMinutes = (this.ApiLimit.AccessLimitResetDate - this.Now).TotalMinutes;
                else
                    this.remainMinutes = -1;
            }
        }

        [Test]
        public void GaugeBoundsTest()
        {
            using (var toolStrip = new TestToolStripAPIGauge())
            {
                toolStrip.AutoSize = false;
                toolStrip.Size = new Size(100, 10);
                toolStrip.GaugeHeight = 5;

                // 現在時刻を偽装
                toolStrip.Now = new DateTime(2013, 1, 1, 0, 0, 0);

                // toolStrip.ApiLimit の初期値は null

                Assert.That(toolStrip.apiGaugeBounds, Is.EqualTo(Rectangle.Empty));
                Assert.That(toolStrip.timeGaugeBounds, Is.EqualTo(Rectangle.Empty));

                toolStrip.ApiLimit = new ApiLimit(150, 60, toolStrip.Now.AddMinutes(15));

                Assert.That(toolStrip.apiGaugeBounds, Is.EqualTo(new Rectangle(0, 0, 40, 5))); // 40% (60/150)
                Assert.That(toolStrip.timeGaugeBounds, Is.EqualTo(new Rectangle(0, 5, 25, 5))); // 25% (15/60)

                toolStrip.ApiLimit = null;

                Assert.That(toolStrip.apiGaugeBounds, Is.EqualTo(Rectangle.Empty));
                Assert.That(toolStrip.timeGaugeBounds, Is.EqualTo(Rectangle.Empty));
            }
        }
    }
}
