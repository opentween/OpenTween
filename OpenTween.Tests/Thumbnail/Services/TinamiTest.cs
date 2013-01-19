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
using System.Xml.Linq;

namespace OpenTween.Thumbnail.Services
{
    [TestFixture]
    class TinamiTest
    {
        class TestTinami : Tinami
        {
            public string FakeXml { get; set; }

            public TestTinami(string pattern, string replacement)
                : base(pattern, replacement)
            {
            }

            protected override XDocument FetchContentInfoApi(string url)
            {
                Assert.That(url, Is.StringMatching(@"http://api\.tinami\.com/content/info\?cont_id=.+&api_key=.+"));

                return XDocument.Parse(this.FakeXml);
            }
        }

        [Test]
        public void ApiTest()
        {
            var service = new TestTinami(@"^http://www\.tinami\.com/view/(?<ContentId>\d+)$",
                "http://api.tinami.com/content/info?cont_id=${ContentId}&api_key=" + ApplicationSettings.TINAMIApiKey);

            service.FakeXml = @"<?xml version='1.0' encoding='utf-8' ?>
<rsp stat='ok'>
  <content type='illust' issupport='1' iscollection='0'>
    <title>ほげほげ</title>
    <description>説明</description>
    <thumbnails>
      <thumbnail_150x150 url='http://img.tinami.com/hogehoge_150.gif' width='112' height='120'/>
    </thumbnails>
    <image>
      <url>http://img.tinami.com/hogehoge_full.gif</url>
      <width>640</width>
      <height>480</height>
    </image>
  </content>
</rsp>";
            var thumbinfo = service.GetThumbnailInfo("http://www.tinami.com/view/12345", null);

            Assert.That(thumbinfo, Is.Not.Null);
            Assert.That(thumbinfo.ImageUrl, Is.EqualTo("http://www.tinami.com/view/12345"));
            Assert.That(thumbinfo.ThumbnailUrl, Is.EqualTo("http://img.tinami.com/hogehoge_150.gif"));
            Assert.That(thumbinfo.TooltipText, Is.EqualTo("説明"));
        }

        [Test]
        public void ApiErrorTest()
        {
            var service = new TestTinami(@"^http://www\.tinami\.com/view/(?<ContentId>\d+)$",
                "http://api.tinami.com/content/info?cont_id=${ContentId}&api_key=" + ApplicationSettings.TINAMIApiKey);

            service.FakeXml = @"<?xml version='1.0' encoding='utf-8'?>
<rsp stat='user_only'>
  <err msg='この作品は登録ユーザー限定の作品です。'/>
</rsp>";
            var thumbinfo = service.GetThumbnailInfo("http://www.tinami.com/view/12345", null);

            Assert.That(thumbinfo, Is.Null);
        }
    }
}
