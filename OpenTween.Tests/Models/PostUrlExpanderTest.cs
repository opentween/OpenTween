// OpenTween - Client of Twitter
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween.Models
{
    public class PostUrlExpanderTest
    {
        [Fact]
        public async Task Expand_Test()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // https://bit.ly/abcde -> https://example.com/abcde
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://bit.ly/abcde"), x.RequestUri);

                return new HttpResponseMessage(HttpStatusCode.TemporaryRedirect)
                {
                    Headers = { Location = new Uri("https://example.com/abcde") },
                };
            });

            var post = new PostClass
            {
                Text = """<a href="https://t.co/aaaaaaa" title="https://t.co/aaaaaaa">bit.ly/abcde</a>""",
                ExpandedUrls = new[]
                {
                    new PostClass.ExpandedUrlInfo(
                        // 展開前の t.co ドメインの URL
                        Url: "https://t.co/aaaaaaa",

                        // Entity の expanded_url に含まれる URL
                        ExpandedUrl: "https://bit.ly/abcde"
                    ),
                },
            };

            var urlInfo = post.ExpandedUrls.Single();

            // 短縮 URL の展開が完了していない状態
            //   → この段階では Entity に含まれる expanded_url の URL が使用される
            Assert.False(urlInfo.ExpandCompleted);
            Assert.Equal("https://bit.ly/abcde", urlInfo.ExpandedUrl);
            Assert.Equal("https://bit.ly/abcde", post.GetExpandedUrl("https://t.co/aaaaaaa"));
            Assert.Equal(new[] { "https://bit.ly/abcde" }, post.GetExpandedUrls());
            Assert.Equal("""<a href="https://t.co/aaaaaaa" title="https://bit.ly/abcde">bit.ly/abcde</a>""", post.Text);

            // bit.ly 展開後の URL は「https://example.com/abcde」
            var expander = new PostUrlExpander(shortUrl);
            await expander.Expand(post);

            // 短縮 URL の展開が完了した後の状態
            //   → 再帰的な展開後の URL が使用される
            urlInfo = post.ExpandedUrls.Single();
            Assert.True(urlInfo.ExpandCompleted);
            Assert.Equal("https://example.com/abcde", urlInfo.ExpandedUrl);
            Assert.Equal("https://example.com/abcde", post.GetExpandedUrl("https://t.co/aaaaaaa"));
            Assert.Equal(new[] { "https://example.com/abcde" }, post.GetExpandedUrls());
            Assert.Equal("""<a href="https://t.co/aaaaaaa" title="https://example.com/abcde">bit.ly/abcde</a>""", post.Text);
        }
    }
}
