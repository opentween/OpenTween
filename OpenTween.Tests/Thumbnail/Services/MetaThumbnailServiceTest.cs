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
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace OpenTween.Thumbnail.Services
{
    public class MetaThumbnailServiceTest
    {
        class TestMetaThumbnailService : MetaThumbnailService
        {
            public string FakeHtml { get; set; }

            public TestMetaThumbnailService(string urlPattern)
                : base(null, urlPattern)
            {
            }

            protected override Task<string> FetchImageUrlAsync(string url, CancellationToken token)
            {
                return Task.FromResult(this.FakeHtml);
            }
        }

        [Fact]
        public async Task OGPMetaTest()
        {
            var service = new TestMetaThumbnailService(@"http://example.com/.+");

            service.FakeHtml = @"
<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML+RDFa 1.0//EN' 'http://www.w3.org/MarkUp/DTD/xhtml-rdfa-1.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml'>
<head>
  <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'/>
  <meta property='og:image' content='http://img.example.com/abcd'/>
  <title>hogehoge</title>
</head>
<body>
  <p>hogehoge</p>
</body>
</html>
";
            var thumbinfo = await service.GetThumbnailInfoAsync("http://example.com/abcd", null, CancellationToken.None);

            Assert.NotNull(thumbinfo);
            Assert.Equal("http://example.com/abcd", thumbinfo.ImageUrl);
            Assert.Equal("http://img.example.com/abcd", thumbinfo.ThumbnailUrl);
            Assert.Null(thumbinfo.TooltipText);
        }

        [Fact]
        public async Task TwitterMetaTest()
        {
            var service = new TestMetaThumbnailService(@"http://example.com/.+");

            service.FakeHtml = @"
<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.01//EN' 'http://www.w3.org/TR/html4/strict.dtd'>

<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
<meta name='twitter:image' content='http://img.example.com/abcd'>
<title>hogehoge</title>

<p>hogehoge
";
            var thumbinfo = await service.GetThumbnailInfoAsync("http://example.com/abcd", null, CancellationToken.None);

            Assert.NotNull(thumbinfo);
            Assert.Equal("http://example.com/abcd", thumbinfo.ImageUrl);
            Assert.Equal("http://img.example.com/abcd", thumbinfo.ThumbnailUrl);
            Assert.Null(thumbinfo.TooltipText);
        }

        [Fact]
        public async Task InvalidMetaTest()
        {
            var service = new TestMetaThumbnailService(@"http://example.com/.+");

            service.FakeHtml = @"
<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.01//EN' 'http://www.w3.org/TR/html4/strict.dtd'>

<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
<meta name='twitter:image' value='http://img.example.com/abcd'>
<title>hogehoge</title>

<p>hogehoge
";
            var thumbinfo = await service.GetThumbnailInfoAsync("http://example.com/abcd", null, CancellationToken.None);

            Assert.NotNull(thumbinfo);
            Assert.Equal("http://example.com/abcd", thumbinfo.ImageUrl);
            Assert.Equal("http://img.example.com/abcd", thumbinfo.ThumbnailUrl);
            Assert.Null(thumbinfo.TooltipText);
        }

        [Fact]
        public async Task NoMetaTest()
        {
            var service = new TestMetaThumbnailService(@"http://example.com/.+");

            service.FakeHtml = @"
<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.01//EN' 'http://www.w3.org/TR/html4/strict.dtd'>

<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
<title>hogehoge</title>

<p>hogehoge
";
            var thumbinfo = await service.GetThumbnailInfoAsync("http://example.com/abcd", null, CancellationToken.None);

            Assert.Null(thumbinfo);
        }
    }
}
