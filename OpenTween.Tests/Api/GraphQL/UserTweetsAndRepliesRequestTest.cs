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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using OpenTween.Connection;
using Xunit;

namespace OpenTween.Api.GraphQL
{
    public class UserTweetsAndRepliesRequestTest
    {
        [Fact]
        public async Task Send_Test()
        {
            using var responseStream = File.OpenRead("Resources/Responses/UserTweetsAndReplies_SimpleTweet.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.GetStreamAsync(It.IsAny<Uri>(), It.IsAny<IDictionary<string, string>>())
                )
                .Callback<Uri, IDictionary<string, string>>((url, param) =>
                {
                    Assert.Equal(new("https://twitter.com/i/api/graphql/YlkSUg0mRBx7-EkxCvc-bw/UserTweetsAndReplies"), url);
                    Assert.Equal(2, param.Count);
                    Assert.Equal("""{"userId":"40480664","count":20,"includePromotedContent":true,"withCommunity":true,"withVoice":true,"withV2Timeline":true}""", param["variables"]);
                    Assert.True(param.ContainsKey("features"));
                })
                .ReturnsAsync(responseStream);

            var request = new UserTweetsAndRepliesRequest(userId: "40480664")
            {
                Count = 20,
            };

            var response = await request.Send(mock.Object).ConfigureAwait(false);
            Assert.Single(response.Tweets);
            Assert.Equal("DAABCgABF_tTnZvAJxEKAAIWes8rE1oQAAgAAwAAAAEAAA", response.CursorTop);
            Assert.Equal("DAABCgABF_tTnZu__-0KAAIWZa6KTRoAAwgAAwAAAAIAAA", response.CursorBottom);

            mock.VerifyAll();
        }

        [Fact]
        public async Task Send_RequestCursor_Test()
        {
            using var responseStream = File.OpenRead("Resources/Responses/UserTweetsAndReplies_SimpleTweet.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.GetStreamAsync(It.IsAny<Uri>(), It.IsAny<IDictionary<string, string>>())
                )
                .Callback<Uri, IDictionary<string, string>>((url, param) =>
                {
                    Assert.Equal(new("https://twitter.com/i/api/graphql/YlkSUg0mRBx7-EkxCvc-bw/UserTweetsAndReplies"), url);
                    Assert.Equal(2, param.Count);
                    Assert.Equal("""{"userId":"40480664","count":20,"includePromotedContent":true,"withCommunity":true,"withVoice":true,"withV2Timeline":true,"cursor":"aaa"}""", param["variables"]);
                    Assert.True(param.ContainsKey("features"));
                })
                .ReturnsAsync(responseStream);

            var request = new UserTweetsAndRepliesRequest(userId: "40480664")
            {
                Count = 20,
                Cursor = "aaa",
            };

            await request.Send(mock.Object).ConfigureAwait(false);
            mock.VerifyAll();
        }
    }
}
