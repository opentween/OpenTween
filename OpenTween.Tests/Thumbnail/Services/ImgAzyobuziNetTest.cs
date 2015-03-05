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
using System.Net.Http;
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
    public class ImgAzyobuziNetTest
    {
        class TestImgAzyobuziNet : ImgAzyobuziNet
        {
            public TestImgAzyobuziNet()
                : this(new[] { "http://img.azyobuzi.net/api/" })
            {
            }

            public TestImgAzyobuziNet(string[] apiHosts)
                : base(null, autoupdate: false)
            {
                this.ApiHosts = apiHosts;
                this.LoadRegexAsync().Wait();
            }

            public string GetApiBase()
            {
                return this.ApiBase;
            }

            protected override Task<byte[]> FetchRegexAsync(string apiBase)
            {
                return Task.Run(() =>
                {
                    if (apiBase == "http://down.example.com/api/")
                        throw new HttpRequestException();

                    if (apiBase == "http://error.example.com/api/")
                        return Encoding.UTF8.GetBytes("{\"error\": {\"code\": 5001}}");

                    if (apiBase == "http://invalid.example.com/api/")
                        return Encoding.UTF8.GetBytes("<<<INVALID JSON>>>");

                    return Encoding.UTF8.GetBytes("[{\"name\": \"hogehoge\", \"regex\": \"^https?://example.com/(.+)$\"}]");
                });
            }
        }

        [Fact]
        public async Task HostFallbackTest()
        {
            var service = new TestImgAzyobuziNet(new[] { "http://avail1.example.com/api/", "http://avail2.example.com/api/" });
            await service.LoadRegexAsync();
            Assert.Equal("http://avail1.example.com/api/", service.GetApiBase());

            service = new TestImgAzyobuziNet(new[] { "http://down.example.com/api/", "http://avail.example.com/api/" });
            await service.LoadRegexAsync();
            Assert.Equal("http://avail.example.com/api/", service.GetApiBase());

            service = new TestImgAzyobuziNet(new[] { "http://error.example.com/api/", "http://avail.example.com/api/" });
            await service.LoadRegexAsync();
            Assert.Equal("http://avail.example.com/api/", service.GetApiBase());

            service = new TestImgAzyobuziNet(new[] { "http://invalid.example.com/api/", "http://avail.example.com/api/" });
            await service.LoadRegexAsync();
            Assert.Equal("http://avail.example.com/api/", service.GetApiBase());

            service = new TestImgAzyobuziNet(new[] { "http://down.example.com/api/" });
            await service.LoadRegexAsync();
            Assert.Null(service.GetApiBase());
        }

        [Fact]
        public async Task ServerOutageTest()
        {
            var service = new TestImgAzyobuziNet(new[] { "http://down.example.com/api/" });

            await service.LoadRegexAsync();
            Assert.Null(service.GetApiBase());

            var thumbinfo = await service.GetThumbnailInfoAsync("http://example.com/abcd", null, CancellationToken.None);
            Assert.Null(thumbinfo);
        }

        [Fact]
        public async Task MatchTest()
        {
            var service = new TestImgAzyobuziNet();
            var thumbinfo = await service.GetThumbnailInfoAsync("http://example.com/abcd", null, CancellationToken.None);

            Assert.NotNull(thumbinfo);
            Assert.Equal("http://example.com/abcd", thumbinfo.ImageUrl);
            Assert.Equal("http://img.azyobuzi.net/api/redirect?size=large&uri=http%3A%2F%2Fexample.com%2Fabcd", thumbinfo.ThumbnailUrl);
            Assert.Null(thumbinfo.TooltipText);
        }

        [Fact]
        public async Task NotMatchTest()
        {
            var service = new TestImgAzyobuziNet();
            var thumbinfo = await service.GetThumbnailInfoAsync("http://hogehoge.com/abcd", null, CancellationToken.None);

            Assert.Null(thumbinfo);
        }

        [Fact]
        public async Task DisabledInDM_Test()
        {
            var service = new TestImgAzyobuziNet();
            service.DisabledInDM = true;

            var post = new PostClass
            {
                TextFromApi = "http://example.com/abcd",
                IsDm = true,
            };

            var thumbinfo = await service.GetThumbnailInfoAsync("http://example.com/abcd", post, CancellationToken.None);

            Assert.Null(thumbinfo);
        }

        [Fact]
        public async Task Enabled_FalseTest()
        {
            var service = new TestImgAzyobuziNet();
            service.Enabled = false;

            var thumbinfo = await service.GetThumbnailInfoAsync("http://example.com/abcd", null, CancellationToken.None);

            Assert.Null(thumbinfo);
        }
    }
}
