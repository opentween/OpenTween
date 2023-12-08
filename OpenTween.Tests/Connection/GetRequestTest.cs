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
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace OpenTween.Connection
{
    public class GetRequestTest
    {
        [Fact]
        public void CreateMessage_Test()
        {
            var request = new GetRequest
            {
                RequestUri = new("statuses/show.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["id"] = "12345",
                },
            };

            var baseUri = new Uri("https://api.twitter.com/v1/");
            using var requestMessage = request.CreateMessage(baseUri);

            Assert.Equal(HttpMethod.Get, requestMessage.Method);
            Assert.Equal(new("https://api.twitter.com/v1/statuses/show.json?id=12345"), requestMessage.RequestUri);
        }

        [Fact]
        public void BuildUriWithQuery_Test()
        {
            var uri = new Uri("https://example.com/hoge");
            var query = new Dictionary<string, string>
            {
                ["foo"] = "bar",
            };
            Assert.Equal(new("https://example.com/hoge?foo=bar"), GetRequest.BuildUriWithQuery(uri, query));
        }

        [Fact]
        public void BuildUriWithQuery_NullTest()
        {
            var uri = new Uri("https://example.com/hoge");
            Assert.Equal(new("https://example.com/hoge"), GetRequest.BuildUriWithQuery(uri, null));
        }

        [Fact]
        public void BuildUriWithQuery_CannotMergeTest()
        {
            var uri = new Uri("https://example.com/hoge?aaa=111");
            var query = new Dictionary<string, string>
            {
                ["bbb"] = "222",
            };
            Assert.Throws<NotSupportedException>(
                () => GetRequest.BuildUriWithQuery(uri, query)
            );
        }
    }
}
