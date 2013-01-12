// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using NUnit.Framework;
using System.Reflection;
using System.Windows.Forms;
using OpenTween.Thumbnail;
using OpenTween.Thumbnail.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween
{
    [TestFixture]
    class TweetThumbnailTest
    {
        class TestThumbnailService : SimpleThumbnailService
        {
            protected string tooltip;

            public TestThumbnailService(string pattern, string replacement, string tooltip)
                : base(pattern, replacement)
            {
                this.tooltip = tooltip;
            }

            public override ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
            {
                var thumbinfo = base.GetThumbnailInfo(url, post);

                if (thumbinfo != null && this.tooltip != null)
                {
                    var match = this.regex.Match(url);
                    thumbinfo.TooltipText = match.Result(this.tooltip);
                }

                return thumbinfo;
            }
        }

        [TestFixtureSetUp]
        public void ThumbnailGeneratorSetup()
        {
            ThumbnailGenerator.Services.Clear();
            ThumbnailGenerator.Services.AddRange(new[]
            {
                new TestThumbnailService(@"^https?://foo.example.com/(.+)$", @"dot.gif", null),
                new TestThumbnailService(@"^https?://bar.example.com/(.+)$", @"dot.gif", @"${1}"),
            });
        }

        [Test]
        public void CreatePictureBoxTest()
        {
            using (var thumbBox = new TweetThumbnail())
            {
                var method = typeof(TweetThumbnail).GetMethod("CreatePictureBox", BindingFlags.Instance | BindingFlags.NonPublic);
                var picbox = method.Invoke(thumbBox, new[] { "pictureBox1" }) as PictureBox;

                Assert.That(picbox, Is.Not.Null);
                Assert.That(picbox.Name, Is.EqualTo("pictureBox1"));
                Assert.That(picbox.SizeMode, Is.EqualTo(PictureBoxSizeMode.Zoom));
                Assert.That(picbox.WaitOnLoad, Is.False);
                Assert.That(picbox.Dock, Is.EqualTo(DockStyle.Fill));

                picbox.Dispose();
            }
        }

        [Test]
        public void CancelAsyncTest()
        {
            using (var thumbbox = new TweetThumbnail())
            {
                var post = new PostClass();

                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                var task = thumbbox.ShowThumbnailAsync(post);

                thumbbox.CancelAsync();

                Assert.That(task.IsCanceled, Is.True);
            }
        }

        [Test]
        public void SetThumbnailCountTest(
            [Values(0, 1, 2)] int count)
        {
            using (var thumbbox = new TweetThumbnail())
            {
                var method = typeof(TweetThumbnail).GetMethod("SetThumbnailCount", BindingFlags.Instance | BindingFlags.NonPublic);
                method.Invoke(thumbbox, new[] { (object)count });

                Assert.That(thumbbox.pictureBox.Count, Is.EqualTo(count));

                var num = 0;
                foreach (var picbox in thumbbox.pictureBox)
                {
                    Assert.That(picbox.Name, Is.EqualTo("pictureBox" + num));
                    num++;
                }

                Assert.That(thumbbox.Controls, Is.EquivalentTo(new Control[]{ thumbbox.scrollBar }.Concat(thumbbox.pictureBox)));

                Assert.That(thumbbox.scrollBar.Minimum, Is.EqualTo(0));
                Assert.That(thumbbox.scrollBar.Maximum, Is.EqualTo(count));
            }
        }

        [Test]
        public void ShowThumbnailAsyncTest()
        {
            var post = new PostClass
            {
                TextFromApi = "てすと http://foo.example.com/abcd",
                Media = new Dictionary<string, string>
                {
                    {"http://foo.example.com/abcd", "http://foo.example.com/abcd"},
                },
            };

            using (var thumbbox = new TweetThumbnail())
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                thumbbox.ShowThumbnailAsync(post).Wait();

                Assert.That(thumbbox.scrollBar.Maximum, Is.EqualTo(0));
                Assert.That(thumbbox.scrollBar.Enabled, Is.False);

                Assert.That(thumbbox.pictureBox.Count, Is.EqualTo(1));
                Assert.That(thumbbox.pictureBox[0].ImageLocation, Is.EqualTo("dot.gif"));

                var thumbinfo = thumbbox.pictureBox[0].Tag as ThumbnailInfo;
                Assert.That(thumbinfo, Is.Not.Null);
                Assert.That(thumbinfo.ImageUrl, Is.EqualTo("http://foo.example.com/abcd"));
                Assert.That(thumbinfo.ThumbnailUrl, Is.EqualTo("dot.gif"));

                Assert.That(thumbbox.toolTip.GetToolTip(thumbbox.pictureBox[0]), Is.EqualTo(""));
            }
        }

        [Test]
        public void ShowThumbnailAsyncTest2()
        {
            var post = new PostClass
            {
                TextFromApi = "てすと http://foo.example.com/abcd http://bar.example.com/efgh",
                Media = new Dictionary<string, string>
                {
                    {"http://foo.example.com/abcd", "http://foo.example.com/abcd"},
                    {"http://bar.example.com/efgh", "http://bar.example.com/efgh"},
                },
            };

            using (var thumbbox = new TweetThumbnail())
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                thumbbox.ShowThumbnailAsync(post).Wait();

                Assert.That(thumbbox.scrollBar.Maximum, Is.EqualTo(1));
                Assert.That(thumbbox.scrollBar.Enabled, Is.True);

                Assert.That(thumbbox.pictureBox.Count, Is.EqualTo(2));
                Assert.That(thumbbox.pictureBox[0].ImageLocation, Is.EqualTo("dot.gif"));
                Assert.That(thumbbox.pictureBox[1].ImageLocation, Is.EqualTo("dot.gif"));

                var thumbinfo = thumbbox.pictureBox[0].Tag as ThumbnailInfo;
                Assert.That(thumbinfo, Is.Not.Null);
                Assert.That(thumbinfo.ImageUrl, Is.EqualTo("http://foo.example.com/abcd"));
                Assert.That(thumbinfo.ThumbnailUrl, Is.EqualTo("dot.gif"));

                thumbinfo = thumbbox.pictureBox[1].Tag as ThumbnailInfo;
                Assert.That(thumbinfo, Is.Not.Null);
                Assert.That(thumbinfo.ImageUrl, Is.EqualTo("http://bar.example.com/efgh"));
                Assert.That(thumbinfo.ThumbnailUrl, Is.EqualTo("dot.gif"));

                Assert.That(thumbbox.toolTip.GetToolTip(thumbbox.pictureBox[0]), Is.EqualTo(""));
                Assert.That(thumbbox.toolTip.GetToolTip(thumbbox.pictureBox[1]), Is.EqualTo("efgh"));
            }
        }

        [Test]
        public void ThumbnailLoadingEventTest()
        {
            using (var thumbbox = new TweetThumbnail())
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

                bool eventCalled;
                thumbbox.ThumbnailLoading +=
                    (s, e) => { eventCalled = true; };

                var post = new PostClass
                {
                    TextFromApi = "てすと",
                    Media = new Dictionary<string, string>
                    {
                    },
                };
                eventCalled = false;
                thumbbox.ShowThumbnailAsync(post).Wait();

                Assert.That(eventCalled, Is.False);

                var post2 = new PostClass
                {
                    TextFromApi = "てすと http://foo.example.com/abcd",
                    Media = new Dictionary<string, string>
                    {
                        {"http://foo.example.com/abcd", "http://foo.example.com/abcd"},
                    },
                };
                eventCalled = false;
                thumbbox.ShowThumbnailAsync(post2).Wait();

                Assert.That(eventCalled, Is.True);
            }
        }

        [Test]
        [Ignore("なぜかTravis CIだと通らないテスト")]
        public void ScrollTest()
        {
            var post = new PostClass
            {
                TextFromApi = "てすと http://foo.example.com/abcd http://foo.example.com/efgh",
                Media = new Dictionary<string, string>
                {
                    {"http://foo.example.com/abcd", "http://foo.example.com/abcd"},
                    {"http://foo.example.com/efgh", "http://foo.example.com/efgh"},
                },
            };

            using (var thumbbox = new TweetThumbnail())
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                thumbbox.ShowThumbnailAsync(post).Wait();

                Assert.That(thumbbox.scrollBar.Minimum, Is.EqualTo(0));
                Assert.That(thumbbox.scrollBar.Maximum, Is.EqualTo(1));

                thumbbox.scrollBar.Value = 0;

                thumbbox.ScrollUp();
                Assert.That(thumbbox.scrollBar.Value, Is.EqualTo(1));
                Assert.That(thumbbox.pictureBox[0].Visible, Is.False);
                Assert.That(thumbbox.pictureBox[1].Visible, Is.True);

                thumbbox.ScrollUp();
                Assert.That(thumbbox.scrollBar.Value, Is.EqualTo(1));
                Assert.That(thumbbox.pictureBox[0].Visible, Is.False);
                Assert.That(thumbbox.pictureBox[1].Visible, Is.True);

                thumbbox.ScrollDown();
                Assert.That(thumbbox.scrollBar.Value, Is.EqualTo(0));
                Assert.That(thumbbox.pictureBox[0].Visible, Is.True);
                Assert.That(thumbbox.pictureBox[1].Visible, Is.False);

                thumbbox.ScrollDown();
                Assert.That(thumbbox.scrollBar.Value, Is.EqualTo(0));
                Assert.That(thumbbox.pictureBox[0].Visible, Is.True);
                Assert.That(thumbbox.pictureBox[1].Visible, Is.False);
            }
        }
    }
}
