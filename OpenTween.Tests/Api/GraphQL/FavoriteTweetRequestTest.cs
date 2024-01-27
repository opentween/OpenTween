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
using OpenTween.Connection;
using Xunit;

namespace OpenTween.Api.GraphQL
{
    public class FavoriteTweetRequestTest
    {
        [Fact]
        public async Task Send_Test()
        {
            using var apiResponse = await TestUtils.CreateApiResponse("Resources/Responses/FavoriteTweet.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.SendAsync(It.IsAny<IHttpRequest>())
                )
                .Callback<IHttpRequest>(x =>
                {
                    var request = Assert.IsType<PostJsonRequest>(x);
                    Assert.Equal(new("https://twitter.com/i/api/graphql/lI07N6Otwv1PhnEgXILM7A/FavoriteTweet"), request.RequestUri);
                    Assert.Equal("""{"variables":{"tweet_id":"12345"},"queryId":"lI07N6Otwv1PhnEgXILM7A"}""", request.JsonString);
                })
                .ReturnsAsync(apiResponse);

            var request = new FavoriteTweetRequest
            {
                TweetId = new("12345"),
            };

            await request.Send(mock.Object);

            mock.VerifyAll();
        }

        [Fact]
        public async Task Send_AlreadyFavoritedTest()
        {
            using var apiResponse = await TestUtils.CreateApiResponse("Resources/Responses/FavoriteTweet_AlreadyFavorited.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x => x.SendAsync(It.IsAny<IHttpRequest>()))
                .ReturnsAsync(apiResponse);

            var request = new FavoriteTweetRequest
            {
                TweetId = new("12345"),
            };

            // 重複によるエラーレスポンスが返っているが例外を発生させない
            await request.Send(mock.Object);

            mock.VerifyAll();
        }
    }
}
