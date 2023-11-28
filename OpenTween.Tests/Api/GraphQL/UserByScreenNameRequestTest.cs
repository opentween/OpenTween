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
    public class UserByScreenNameRequestTest
    {
        [Fact]
        public async Task Send_Test()
        {
            using var responseStream = File.OpenRead("Resources/Responses/UserByScreenName.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.GetStreamAsync(It.IsAny<Uri>(), It.IsAny<IDictionary<string, string>>())
                )
                .Callback<Uri, IDictionary<string, string>>((url, param) =>
                {
                    Assert.Equal(new("https://twitter.com/i/api/graphql/xc8f1g7BYqr6VTzTbvNlGw/UserByScreenName"), url);
                    Assert.Contains(@"""screen_name"":""opentween""", param["variables"]);
                })
                .ReturnsAsync(responseStream);

            var request = new UserByScreenNameRequest
            {
                ScreenName = "opentween",
            };

            var user = await request.Send(mock.Object).ConfigureAwait(false);
            Assert.Equal("514241801", user.ToTwitterUser().IdStr);

            mock.VerifyAll();
        }

        [Fact]
        public async Task Send_UserUnavailableTest()
        {
            using var responseStream = File.OpenRead("Resources/Responses/UserByScreenName_Suspended.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.GetStreamAsync(It.IsAny<Uri>(), It.IsAny<IDictionary<string, string>>())
                )
                .ReturnsAsync(responseStream);

            var request = new UserByScreenNameRequest
            {
                ScreenName = "elonmusk",
            };

            var ex = await Assert.ThrowsAsync<WebApiException>(
                () => request.Send(mock.Object)
            );
            Assert.Equal("User is suspended", ex.Message);

            mock.VerifyAll();
        }
    }
}
