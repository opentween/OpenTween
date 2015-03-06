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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace OpenTween.Thumbnail.Services
{
    public class TinamiTest
    {
        class TestTinami : Tinami
        {
            public string FakeXml { get; set; }

            public TestTinami()
                : base(null)
            {
            }

            protected override Task<XDocument> FetchContentInfoApiAsync(string contentId, CancellationToken token)
            {
                return Task.FromResult(XDocument.Parse(this.FakeXml));
            }
        }

        [Fact]
        public async Task ApiTest()
        {
            var service = new TestTinami();

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
            var thumbinfo = await service.GetThumbnailInfoAsync("http://www.tinami.com/view/12345", null, CancellationToken.None);

            Assert.NotNull(thumbinfo);
            Assert.Equal("http://www.tinami.com/view/12345", thumbinfo.ImageUrl);
            Assert.Equal("http://img.tinami.com/hogehoge_150.gif", thumbinfo.ThumbnailUrl);
            Assert.Equal("説明", thumbinfo.TooltipText);
        }

        [Fact]
        public async Task ApiErrorTest()
        {
            var service = new TestTinami();

            service.FakeXml = @"<?xml version='1.0' encoding='utf-8'?>
<rsp stat='user_only'>
  <err msg='この作品は登録ユーザー限定の作品です。'/>
</rsp>";
            var thumbinfo = await service.GetThumbnailInfoAsync("http://www.tinami.com/view/12345", null, CancellationToken.None);

            Assert.Null(thumbinfo);
        }
    }
}
