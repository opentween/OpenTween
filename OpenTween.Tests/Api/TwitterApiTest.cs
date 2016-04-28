// OpenTween - Client of Twitter
// Copyright (c) 2016 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Threading.Tasks;
using Moq;
using OpenTween.Api.DataModel;
using OpenTween.Connection;
using Xunit;

namespace OpenTween.Api
{
    public class TwitterApiTest
    {
        public TwitterApiTest()
        {
            this.MyCommonSetup();
        }

        public void MyCommonSetup()
        {
            var mockAssembly = new Mock<_Assembly>();
            mockAssembly.Setup(m => m.GetName()).Returns(new AssemblyName("OpenTween"));

            MyCommon.EntryAssembly = mockAssembly.Object;
        }

        [Fact]
        public void Initialize_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                Assert.Null(twitterApi.apiConnection);

                twitterApi.Initialize("*** AccessToken ***", "*** AccessSecret ***", userId: 100L, screenName: "hogehoge");

                Assert.IsType<TwitterApiConnection>(twitterApi.apiConnection);

                var apiConnection = (TwitterApiConnection)twitterApi.apiConnection;
                Assert.Equal("*** AccessToken ***", apiConnection.AccessToken);
                Assert.Equal("*** AccessSecret ***", apiConnection.AccessSecret);

                Assert.Equal(100L, twitterApi.CurrentUserId);
                Assert.Equal("hogehoge", twitterApi.CurrentScreenName);

                // 複数回 Initialize を実行した場合は新たに TwitterApiConnection が生成される
                twitterApi.Initialize("*** AccessToken2 ***", "*** AccessSecret2 ***", userId: 200L, screenName: "foobar");

                var oldApiConnection = apiConnection;
                Assert.True(oldApiConnection.IsDisposed);

                Assert.IsType<TwitterApiConnection>(twitterApi.apiConnection);

                apiConnection = (TwitterApiConnection)twitterApi.apiConnection;
                Assert.Equal("*** AccessToken2 ***", apiConnection.AccessToken);
                Assert.Equal("*** AccessSecret2 ***", apiConnection.AccessSecret);

                Assert.Equal(200L, twitterApi.CurrentUserId);
                Assert.Equal("foobar", twitterApi.CurrentScreenName);
            }
        }

        [Fact]
        public async Task StatusesShow_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.GetAsync<TwitterStatus>(
                        new Uri("statuses/show.json", UriKind.Relative),
                        new Dictionary<string, string> { { "id", "100" }, { "include_entities", "true" } })
                )
                .ReturnsAsync(new TwitterStatus { Id = 100L });

                twitterApi.apiConnection = mock.Object;

                await twitterApi.StatusesShow(statusId: 100L)
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task FavoritesCreate_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterStatus>(
                        new Uri("favorites/create.json", UriKind.Relative),
                        new Dictionary<string, string> { { "id", "100" } })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterStatus { Id = 100L }));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.FavoritesCreate(statusId: 100L)
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task FavoritesDestroy_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterStatus>(
                        new Uri("favorites/destroy.json", UriKind.Relative),
                        new Dictionary<string, string> { { "id", "100" } })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterStatus { Id = 100L }));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.FavoritesDestroy(statusId: 100L)
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }
    }
}
