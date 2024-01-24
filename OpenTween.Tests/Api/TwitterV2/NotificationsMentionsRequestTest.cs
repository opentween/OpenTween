// OpenTween - Client of Twitter
// Copyright (c) 2024 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using OpenTween.Api.GraphQL;
using OpenTween.Connection;
using Xunit;

namespace OpenTween.Api.TwitterV2
{
    public class NotificationsMentionsRequestTest
    {
        [Fact]
        public async Task Send_Test()
        {
            using var apiResponse = await TestUtils.CreateApiResponse("Resources/Responses/NotificationsMentions.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.SendAsync(It.IsAny<IHttpRequest>())
                )
                .Callback<IHttpRequest>(x =>
                {
                    var request = Assert.IsType<GetRequest>(x);
                    Assert.Equal(new("https://twitter.com/i/api/2/notifications/mentions.json"), request.RequestUri);
                    var query = request.Query!;
                    Assert.Equal("20", query["count"]);
                    Assert.DoesNotContain("cursor", query);
                    Assert.Equal("/2/notifications/mentions", request.EndpointName);
                })
                .ReturnsAsync(apiResponse);

            var request = new NotificationsMentionsRequest()
            {
                Count = 20,
            };

            var response = await request.Send(mock.Object);
            var status = Assert.Single(response.Statuses);
            Assert.Equal("1748671085438988794", status.IdStr);
            Assert.Equal("40480664", status.User.IdStr);

            Assert.Equal("DAABDAABCgABAAAAAC4B0ZQIAAIAAAACCAADm5udsQgABCaolIMACwACAAAAC0FZMG1xVjB6VEZjAAA", response.CursorTop);
            Assert.Equal("DAACDAABCgABAAAAAC4B0ZQIAAIAAAACCAADm5udsQgABCaolIMACwACAAAAC0FZMG1xVjB6VEZjAAA", response.CursorBottom);

            mock.VerifyAll();
        }

        [Fact]
        public async Task Send_RequestCursorTest()
        {
            using var apiResponse = await TestUtils.CreateApiResponse("Resources/Responses/NotificationsMentions.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.SendAsync(It.IsAny<IHttpRequest>())
                )
                .Callback<IHttpRequest>(x =>
                {
                    var request = Assert.IsType<GetRequest>(x);
                    Assert.Equal(new("https://twitter.com/i/api/2/notifications/mentions.json"), request.RequestUri);
                    var query = request.Query!;
                    Assert.Equal("20", query["count"]);
                    Assert.Equal("aaa", query["cursor"]);
                    Assert.Equal("/2/notifications/mentions", request.EndpointName);
                })
                .ReturnsAsync(apiResponse);

            var request = new NotificationsMentionsRequest()
            {
                Count = 20,
                Cursor = "aaa",
            };

            await request.Send(mock.Object);
            mock.VerifyAll();
        }
    }
}
