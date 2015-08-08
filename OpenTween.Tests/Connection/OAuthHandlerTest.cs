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
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween.Connection
{
    public class OAuthHandlerTest
    {
        [Fact]
        public async Task GetParameter_UriQueryTest()
        {
            var requestUri = new Uri("http://example.com/api?aaa=1&bbb=2");

            var actual = await OAuthHandler.GetParameters(requestUri, content: null)
                .ConfigureAwait(false);
            var expected = new[]
            {
                new KeyValuePair<string, string>("aaa", "1"),
                new KeyValuePair<string, string>("bbb", "2"),
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetParameter_FormUrlEncodedTest()
        {
            var requestUri = new Uri("http://example.com/api");
            var formParams = new[]
            {
                new KeyValuePair<string, string>("aaa", "1"),
                new KeyValuePair<string, string>("bbb", "2"),
            };

            using (var content = new FormUrlEncodedContent(formParams))
            {
                var actual = await OAuthHandler.GetParameters(requestUri, content)
                    .ConfigureAwait(false);
                var expected = new[]
                {
                    new KeyValuePair<string, string>("aaa", "1"),
                    new KeyValuePair<string, string>("bbb", "2"),
                };

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async Task GetParameter_MultipartTest()
        {
            var requestUri = new Uri("http://example.com/api");

            using (var content = new MultipartFormDataContent())
            using (var paramA = new StringContent("1"))
            using (var paramB = new StringContent("2"))
            {
                content.Add(paramA, "aaa");
                content.Add(paramB, "bbb");

                var actual = await OAuthHandler.GetParameters(requestUri, content)
                    .ConfigureAwait(false);

                // multipart/form-data のリクエストではパラメータを署名対象にしない
                Assert.Empty(actual);
            }
        }
    }
}
