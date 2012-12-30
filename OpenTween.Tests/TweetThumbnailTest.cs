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

namespace OpenTween
{
    [TestFixture]
    class TweetThumbnailTest
    {
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
                Assert.That(picbox.Visible, Is.False);

                picbox.Dispose();
            }
        }

        class TestTweetThumbnail : TweetThumbnail
        {
            protected override List<ThumbnailInfo> GetThumbailInfo(PostClass post)
            {
                return new List<ThumbnailInfo>
                {
                    new ThumbnailInfo
                    {
                        ImageUrl = "http://example.com/abcd",
                        ThumbnailUrl = "dot.gif", // 1x1の黒画像
                        TooltipText = "ほげほげ",
                    },
                };
            }
        }

        [Test]
        [Ignore("Travis CI 上で実行時に何故か実行時間制限を超えることがあるため")]
        public void CancelAsyncTest()
        {
            using (var thumbbox = new TestTweetThumbnail())
            {
                var post = new PostClass();
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

                Assert.That(thumbbox.scrollBar.Minimum, Is.EqualTo(0));
                Assert.That(thumbbox.scrollBar.Maximum, Is.EqualTo(count));
            }
        }
    }
}
