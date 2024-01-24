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

using System.Threading.Tasks;
using Moq;
using OpenTween.Api.DataModel;
using OpenTween.Connection;
using Xunit;

namespace OpenTween.Api.GraphQL
{
    public class HomeLatestTimelineRequestTest
    {
        [Fact]
        public async Task Send_Test()
        {
            using var apiResponse = await TestUtils.CreateApiResponse("Resources/Responses/HomeLatestTimeline.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.SendAsync(It.IsAny<IHttpRequest>())
                )
                .Callback<IHttpRequest>(x =>
                {
                    var request = Assert.IsType<GetRequest>(x);
                    Assert.Equal(new("https://twitter.com/i/api/graphql/lAKISuk_McyDUlhS2Zmv4A/HomeLatestTimeline"), request.RequestUri);
                    var query = request.Query!;
                    Assert.Equal(2, query.Count);
                    Assert.Equal("""{"includePromotedContent":true,"latestControlAvailable":true,"requestContext":"launch","count":20}""", query["variables"]);
                    Assert.True(query.ContainsKey("features"));
                    Assert.Equal("HomeLatestTimeline", request.EndpointName);
                })
                .ReturnsAsync(apiResponse);

            var request = new HomeLatestTimelineRequest
            {
                Count = 20,
            };

            var response = await request.Send(mock.Object);
            Assert.Single(response.Tweets);
            Assert.Equal("DAABCgABGENe-W5AJxEKAAIWeWboXhcQAAgAAwAAAAEAAA", response.CursorTop);
            Assert.Equal("DAABCgABGENe-W4__5oKAAIWK_5v3BcQAAgAAwAAAAIAAA", response.CursorBottom);

            mock.VerifyAll();
        }

        [Fact]
        public async Task Send_RequestCursor_Test()
        {
            using var apiResponse = await TestUtils.CreateApiResponse("Resources/Responses/HomeLatestTimeline.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.SendAsync(It.IsAny<IHttpRequest>())
                )
                .Callback<IHttpRequest>(x =>
                {
                    var request = Assert.IsType<GetRequest>(x);
                    Assert.Equal(new("https://twitter.com/i/api/graphql/lAKISuk_McyDUlhS2Zmv4A/HomeLatestTimeline"), request.RequestUri);
                    var query = request.Query!;
                    Assert.Equal(2, query.Count);
                    Assert.Equal("""{"includePromotedContent":true,"latestControlAvailable":true,"requestContext":"launch","count":20,"cursor":"aaa"}""", query["variables"]);
                    Assert.True(query.ContainsKey("features"));
                    Assert.Equal("HomeLatestTimeline", request.EndpointName);
                })
                .ReturnsAsync(apiResponse);

            var request = new HomeLatestTimelineRequest
            {
                Count = 20,
                Cursor = "aaa",
            };

            await request.Send(mock.Object);
            mock.VerifyAll();
        }

        [Fact]
        public async Task Send_RateLimitTest()
        {
            using var apiResponse = await TestUtils.CreateApiResponse("Resources/Responses/HomeLatestTimeline_RateLimit.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.SendAsync(It.IsAny<IHttpRequest>())
                )
                .Callback<IHttpRequest>(x =>
                {
                    var request = Assert.IsType<GetRequest>(x);
                    Assert.Equal(new("https://twitter.com/i/api/graphql/lAKISuk_McyDUlhS2Zmv4A/HomeLatestTimeline"), request.RequestUri);
                })
                .ReturnsAsync(apiResponse);

            var request = new HomeLatestTimelineRequest();

            var ex = await Assert.ThrowsAsync<TwitterApiException>(
                () => request.Send(mock.Object)
            );
            var errorItem = Assert.Single(ex.Errors);
            Assert.Equal(TwitterErrorCode.RateLimit, errorItem.Code);

            mock.VerifyAll();
        }
    }
}
