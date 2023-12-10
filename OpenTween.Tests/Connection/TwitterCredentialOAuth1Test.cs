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

using System.Net.Http;
using Xunit;

namespace OpenTween.Connection
{
    public class TwitterCredentialOAuth1Test
    {
        [Fact]
        public void CreateHttpHandler_Test()
        {
            var appToken = new TwitterAppToken
            {
                AuthType = APIAuthType.OAuth1,
                OAuth1CustomConsumerKey = ApiKey.Create("consumer_key"),
                OAuth1CustomConsumerSecret = ApiKey.Create("consumer_secret"),
            };
            var credential = new TwitterCredentialOAuth1(appToken, "access_token", "access_secret");

            using var innerHandler = new HttpClientHandler();
            using var handler = credential.CreateHttpHandler(innerHandler);

            var oauthHandler = Assert.IsType<OAuthHandler>(handler);
            Assert.Equal("consumer_key", oauthHandler.ConsumerKey.Value);
            Assert.Equal("consumer_secret", oauthHandler.ConsumerSecret.Value);
            Assert.Equal("access_token", oauthHandler.AccessToken);
            Assert.Equal("access_secret", oauthHandler.AccessSecret);
            Assert.Same(innerHandler, oauthHandler.InnerHandler);
        }
    }
}
