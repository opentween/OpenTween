// OpenTween - Client of Twitter
// Copyright (c) 2015 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Threading.Tasks;
using System.Windows.Forms;
using Xunit;

namespace OpenTween
{
    public class OTBaseFormTest
    {
        private class TestForm : OTBaseForm
        {
        }

        public OTBaseFormTest()
            => this.SetupSynchronizationContext();

        protected void SetupSynchronizationContext()
            => WindowsFormsSynchronizationContext.AutoInstall = false;

        [Fact]
        public async Task InvokeAsync_Test()
        {
            using var form = new TestForm();
            await Task.Run(async () =>
            {
                await form.InvokeAsync(() => form.Text = "hoge");
            });

            Assert.Equal("hoge", form.Text);
        }

        [Fact]
        public async Task InvokeAsync_ReturnValueTest()
        {
            using var form = new TestForm();
            form.Text = "hoge";

            await Task.Run(async () =>
            {
                var ret = await form.InvokeAsync(() => form.Text);
                Assert.Equal("hoge", ret);
            });
        }

        [Fact]
        public async Task InvokeAsync_TaskTest()
        {
            using var form = new TestForm();
            await Task.Run(async () =>
            {
                await form.InvokeAsync(async () =>
                {
                    await Task.Delay(1);
                    form.Text = "hoge";
                });
            });

            Assert.Equal("hoge", form.Text);
        }

        [Fact]
        public async Task InvokeAsync_TaskWithValueTest()
        {
            using var form = new TestForm();
            form.Text = "hoge";

            await Task.Run(async () =>
            {
                var ret = await form.InvokeAsync(async () =>
                {
                    await Task.Delay(1);
                    return form.Text;
                });

                Assert.Equal("hoge", ret);
            });
        }

        [Fact]
        public void ScaleChildControl_ListViewTest()
        {
            using var listview = new ListView { Width = 200, Height = 200 };
            listview.Columns.AddRange(new[]
            {
                new ColumnHeader { Width = 60 },
                new ColumnHeader { Width = 140 },
            });

            OTBaseForm.ScaleChildControl(listview, new SizeF(1.25f, 1.25f));

            Assert.Equal(75, listview.Columns[0].Width);
            Assert.Equal(175, listview.Columns[1].Width);
        }

        [Fact]
        public void ScaleChildControl_VScrollBarTest()
        {
            using var scrollBar = new VScrollBar { Width = 20, Height = 200 };
            OTBaseForm.ScaleChildControl(scrollBar, new SizeF(2.0f, 2.0f));

            Assert.Equal(40, scrollBar.Width);
        }
    }
}
