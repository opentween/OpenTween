﻿// OpenTween - Client of Twitter
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
                        new Dictionary<string, string> {
                            { "id", "100" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                        },
                        "/statuses/show/:id")
                )
                .ReturnsAsync(new TwitterStatus { Id = 100L });

                twitterApi.apiConnection = mock.Object;

                await twitterApi.StatusesShow(statusId: 100L)
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task StatusesUpdate_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterStatus>(
                        new Uri("statuses/update.json", UriKind.Relative),
                        new Dictionary<string, string> {
                            { "status", "hogehoge" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "in_reply_to_status_id", "100" },
                            { "media_ids", "10,20" },
                        })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterStatus()));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.StatusesUpdate("hogehoge", replyToId: 100L, mediaIds: new[] { 10L, 20L })
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task StatusesDestroy_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterStatus>(
                        new Uri("statuses/destroy.json", UriKind.Relative),
                        new Dictionary<string, string> { { "id", "100" } })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterStatus { Id = 100L }));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.StatusesDestroy(statusId: 100L)
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task DirectMessagesNew_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterDirectMessage>(
                        new Uri("direct_messages/new.json", UriKind.Relative),
                        new Dictionary<string, string> {
                            { "text", "hogehoge" },
                            { "screen_name", "opentween" },
                        })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterDirectMessage()));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.DirectMessagesNew("hogehoge", "opentween")
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task DirectMessagesDestroy_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterDirectMessage>(
                        new Uri("direct_messages/destroy.json", UriKind.Relative),
                        new Dictionary<string, string> { { "id", "100" } })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterDirectMessage { Id = 100L }));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.DirectMessagesDestroy(statusId: 100L)
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task UsersShow_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.GetAsync<TwitterUser>(
                        new Uri("users/show.json", UriKind.Relative),
                        new Dictionary<string, string> {
                            { "screen_name", "twitterapi" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                        },
                        "/users/show/:id")
                )
                .ReturnsAsync(new TwitterUser { ScreenName = "twitterapi" });

                twitterApi.apiConnection = mock.Object;

                await twitterApi.UsersShow(screenName: "twitterapi")
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task UsersReportSpam_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterUser>(
                        new Uri("users/report_spam.json", UriKind.Relative),
                        new Dictionary<string, string> { { "screen_name", "twitterapi" } })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterUser { ScreenName = "twitterapi" }));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.UsersReportSpam(screenName: "twitterapi")
                    .IgnoreResponse()
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

        [Fact]
        public async Task FriendshipsShow_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.GetAsync<TwitterFriendship>(
                        new Uri("friendships/show.json", UriKind.Relative),
                        new Dictionary<string, string> { { "source_screen_name", "twitter" }, { "target_screen_name", "twitterapi" } },
                        "/friendships/show")
                )
                .ReturnsAsync(new TwitterFriendship());

                twitterApi.apiConnection = mock.Object;

                await twitterApi.FriendshipsShow(sourceScreenName: "twitter", targetScreenName: "twitterapi")
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task FriendshipsCreate_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterFriendship>(
                        new Uri("friendships/create.json", UriKind.Relative),
                        new Dictionary<string, string> { { "screen_name", "twitterapi" } })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterFriendship()));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.FriendshipsCreate(screenName: "twitterapi")
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task FriendshipsDestroy_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterFriendship>(
                        new Uri("friendships/destroy.json", UriKind.Relative),
                        new Dictionary<string, string> { { "screen_name", "twitterapi" } })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterFriendship()));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.FriendshipsDestroy(screenName: "twitterapi")
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task BlocksCreate_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterUser>(
                        new Uri("blocks/create.json", UriKind.Relative),
                        new Dictionary<string, string> { { "screen_name", "twitterapi" } })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterUser()));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.BlocksCreate(screenName: "twitterapi")
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task BlocksDestroy_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterUser>(
                        new Uri("blocks/destroy.json", UriKind.Relative),
                        new Dictionary<string, string> { { "screen_name", "twitterapi" } })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterUser()));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.BlocksDestroy(screenName: "twitterapi")
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task AccountUpdateProfile_Test()
        {
            using (var twitterApi = new TwitterApi())
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterUser>(
                        new Uri("account/update_profile.json", UriKind.Relative),
                        new Dictionary<string, string> {
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "name", "Name" },
                            { "url", "http://example.com/" },
                            { "location", "Location" },
                            { "description", "&lt;script&gt;alert(1)&lt;/script&gt;" },
                        })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterUser()));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.AccountUpdateProfile(name: "Name", url: "http://example.com/", location: "Location", description: "<script>alert(1)</script>")
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task AccountUpdateProfileImage_Test()
        {
            using (var twitterApi = new TwitterApi())
            using (var image = TestUtils.CreateDummyImage())
            using (var media = new MemoryImageMediaItem(image))
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterUser>(
                        new Uri("account/update_profile_image.json", UriKind.Relative),
                        new Dictionary<string, string> {
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                        },
                        new Dictionary<string, IMediaItem> { { "image", media } })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterUser()));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.AccountUpdateProfileImage(media)
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }

        [Fact]
        public async Task MediaUpload_Test()
        {
            using (var twitterApi = new TwitterApi())
            using (var image = TestUtils.CreateDummyImage())
            using (var media = new MemoryImageMediaItem(image))
            {
                var mock = new Mock<IApiConnection>();
                mock.Setup(x =>
                    x.PostLazyAsync<TwitterUploadMediaResult>(
                        new Uri("https://upload.twitter.com/1.1/media/upload.json", UriKind.Absolute),
                        null,
                        new Dictionary<string, IMediaItem> { { "media", media } })
                )
                .ReturnsAsync(LazyJson.Create(new TwitterUploadMediaResult()));

                twitterApi.apiConnection = mock.Object;

                await twitterApi.MediaUpload(media)
                    .IgnoreResponse()
                    .ConfigureAwait(false);

                mock.VerifyAll();
            }
        }
    }
}