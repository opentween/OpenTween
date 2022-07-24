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
using System.IO;
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
            => this.MyCommonSetup();

        private void MyCommonSetup()
        {
            var mockAssembly = new Mock<_Assembly>();
            mockAssembly.Setup(m => m.GetName()).Returns(new AssemblyName("OpenTween"));

            MyCommon.EntryAssembly = mockAssembly.Object;
        }

        [Fact]
        public void Initialize_Test()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            Assert.Null(twitterApi.ApiConnection);

            twitterApi.Initialize("*** AccessToken ***", "*** AccessSecret ***", userId: 100L, screenName: "hogehoge");

            Assert.IsType<TwitterApiConnection>(twitterApi.ApiConnection);

            var apiConnection = (TwitterApiConnection)twitterApi.ApiConnection!;
            Assert.Equal("*** AccessToken ***", apiConnection.AccessToken);
            Assert.Equal("*** AccessSecret ***", apiConnection.AccessSecret);

            Assert.Equal(100L, twitterApi.CurrentUserId);
            Assert.Equal("hogehoge", twitterApi.CurrentScreenName);

            // 複数回 Initialize を実行した場合は新たに TwitterApiConnection が生成される
            twitterApi.Initialize("*** AccessToken2 ***", "*** AccessSecret2 ***", userId: 200L, screenName: "foobar");

            var oldApiConnection = apiConnection;
            Assert.True(oldApiConnection.IsDisposed);

            Assert.IsType<TwitterApiConnection>(twitterApi.ApiConnection);

            apiConnection = (TwitterApiConnection)twitterApi.ApiConnection!;
            Assert.Equal("*** AccessToken2 ***", apiConnection.AccessToken);
            Assert.Equal("*** AccessSecret2 ***", apiConnection.AccessSecret);

            Assert.Equal(200L, twitterApi.CurrentUserId);
            Assert.Equal("foobar", twitterApi.CurrentScreenName);
        }

        [Fact]
        public async Task StatusesHomeTimeline_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterStatus[]>(
                    new Uri("statuses/home_timeline.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                            { "count", "200" },
                            { "max_id", "900" },
                            { "since_id", "100" },
                    },
                    "/statuses/home_timeline")
            )
            .ReturnsAsync(Array.Empty<TwitterStatus>());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesHomeTimeline(200, maxId: 900L, sinceId: 100L)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesMentionsTimeline_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterStatus[]>(
                    new Uri("statuses/mentions_timeline.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                            { "count", "200" },
                            { "max_id", "900" },
                            { "since_id", "100" },
                    },
                    "/statuses/mentions_timeline")
            )
            .ReturnsAsync(Array.Empty<TwitterStatus>());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesMentionsTimeline(200, maxId: 900L, sinceId: 100L)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesUserTimeline_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterStatus[]>(
                    new Uri("statuses/user_timeline.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "screen_name", "twitterapi" },
                            { "include_rts", "true" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                            { "count", "200" },
                            { "max_id", "900" },
                            { "since_id", "100" },
                    },
                    "/statuses/user_timeline")
            )
            .ReturnsAsync(Array.Empty<TwitterStatus>());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesUserTimeline("twitterapi", count: 200, maxId: 900L, sinceId: 100L)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesShow_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterStatus>(
                    new Uri("statuses/show.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "id", "100" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                    },
                    "/statuses/show/:id")
            )
            .ReturnsAsync(new TwitterStatus { Id = 100L });

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesShow(statusId: 100L)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesLookup_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterStatus[]>(
                    new Uri("statuses/lookup.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                        { "id", "100,200" },
                        { "include_entities", "true" },
                        { "include_ext_alt_text", "true" },
                        { "tweet_mode", "extended" },
                    },
                    "/statuses/lookup"
                )
            )
            .ReturnsAsync(Array.Empty<TwitterStatus>());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesLookup(statusIds: new[] { "100", "200" })
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesUpdate_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterStatus>(
                    new Uri("statuses/update.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "status", "hogehoge" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                            { "in_reply_to_status_id", "100" },
                            { "media_ids", "10,20" },
                            { "auto_populate_reply_metadata", "true" },
                            { "exclude_reply_user_ids", "100,200" },
                            { "attachment_url", "https://twitter.com/twitterapi/status/22634515958" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterStatus()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesUpdate(
                    "hogehoge",
                    replyToId: 100L,
                    mediaIds: new[] { 10L, 20L },
                    autoPopulateReplyMetadata: true,
                    excludeReplyUserIds: new[] { 100L, 200L },
                    attachmentUrl: "https://twitter.com/twitterapi/status/22634515958"
                )
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesUpdate_ExcludeReplyUserIdsEmptyTest()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterStatus>(
                    new Uri("statuses/update.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                        { "status", "hogehoge" },
                        { "include_entities", "true" },
                        { "include_ext_alt_text", "true" },
                        { "tweet_mode", "extended" },
                        // exclude_reply_user_ids は空の場合には送信されない
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterStatus()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesUpdate("hogehoge", replyToId: null, mediaIds: null, excludeReplyUserIds: Array.Empty<long>())
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesDestroy_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterStatus>(
                    new Uri("statuses/destroy.json", UriKind.Relative),
                    new Dictionary<string, string> { { "id", "100" } })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterStatus { Id = 100L }));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesDestroy(statusId: 100L)
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesRetweet_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterStatus>(
                    new Uri("statuses/retweet.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "id", "100" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterStatus()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesRetweet(100L)
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task SearchTweets_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterSearchResult>(
                    new Uri("search/tweets.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "q", "from:twitterapi" },
                            { "result_type", "recent" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                            { "lang", "en" },
                            { "count", "200" },
                            { "max_id", "900" },
                            { "since_id", "100" },
                    },
                    "/search/tweets")
            )
            .ReturnsAsync(new TwitterSearchResult());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.SearchTweets("from:twitterapi", "en", count: 200, maxId: 900L, sinceId: 100L)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsOwnerships_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterLists>(
                    new Uri("lists/ownerships.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "screen_name", "twitterapi" },
                            { "cursor", "-1" },
                            { "count", "100" },
                    },
                    "/lists/ownerships")
            )
            .ReturnsAsync(new TwitterLists());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsOwnerships("twitterapi", cursor: -1L, count: 100)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsSubscriptions_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterLists>(
                    new Uri("lists/subscriptions.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "screen_name", "twitterapi" },
                            { "cursor", "-1" },
                            { "count", "100" },
                    },
                    "/lists/subscriptions")
            )
            .ReturnsAsync(new TwitterLists());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsSubscriptions("twitterapi", cursor: -1L, count: 100)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsMemberships_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterLists>(
                    new Uri("lists/memberships.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "screen_name", "twitterapi" },
                            { "cursor", "-1" },
                            { "count", "100" },
                            { "filter_to_owned_lists", "true" },
                    },
                    "/lists/memberships")
            )
            .ReturnsAsync(new TwitterLists());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsMemberships("twitterapi", cursor: -1L, count: 100, filterToOwnedLists: true)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsCreate_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterList>(
                    new Uri("lists/create.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "name", "hogehoge" },
                            { "description", "aaaa" },
                            { "mode", "private" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterList()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsCreate("hogehoge", description: "aaaa", @private: true)
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsUpdate_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterList>(
                    new Uri("lists/update.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "list_id", "12345" },
                            { "name", "hogehoge" },
                            { "description", "aaaa" },
                            { "mode", "private" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterList()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsUpdate(12345L, name: "hogehoge", description: "aaaa", @private: true)
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsDestroy_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterList>(
                    new Uri("lists/destroy.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "list_id", "12345" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterList()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsDestroy(12345L)
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsStatuses_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterStatus[]>(
                    new Uri("lists/statuses.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "list_id", "12345" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                            { "count", "200" },
                            { "max_id", "900" },
                            { "since_id", "100" },
                            { "include_rts", "true" },
                    },
                    "/lists/statuses")
            )
            .ReturnsAsync(Array.Empty<TwitterStatus>());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsStatuses(12345L, count: 200, maxId: 900L, sinceId: 100L, includeRTs: true)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsMembers_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterUsers>(
                    new Uri("lists/members.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "list_id", "12345" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                            { "cursor", "-1" },
                    },
                    "/lists/members")
            )
            .ReturnsAsync(new TwitterUsers());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsMembers(12345L, cursor: -1)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsMembersShow_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterUser>(
                    new Uri("lists/members/show.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "list_id", "12345" },
                            { "screen_name", "twitterapi" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                    },
                    "/lists/members/show")
            )
            .ReturnsAsync(new TwitterUser());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsMembersShow(12345L, "twitterapi")
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsMembersCreate_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterUser>(
                    new Uri("lists/members/create.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "list_id", "12345" },
                            { "screen_name", "twitterapi" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterUser()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsMembersCreate(12345L, "twitterapi")
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsMembersDestroy_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterUser>(
                    new Uri("lists/members/destroy.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "list_id", "12345" },
                            { "screen_name", "twitterapi" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterUser()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsMembersDestroy(12345L, "twitterapi")
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task DirectMessagesEventsList_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterMessageEventList>(
                    new Uri("direct_messages/events/list.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "count", "50" },
                            { "cursor", "12345abcdefg" },
                    },
                    "/direct_messages/events/list")
            )
            .ReturnsAsync(new TwitterMessageEventList());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.DirectMessagesEventsList(count: 50, cursor: "12345abcdefg")
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task DirectMessagesEventsNew_Test()
        {
            var mock = new Mock<IApiConnection>();
            var responseText = @"{
  ""event"": {
    ""type"": ""message_create"",
    ""message_create"": {
      ""target"": {
        ""recipient_id"": ""12345""
      },
      ""message_data"": {
        ""text"": ""hogehoge"",
        ""attachment"": {
          ""type"": ""media"",
          ""media"": {
            ""id"": ""67890""
          }
        }
      }
    }
  }
}";
            mock.Setup(x =>
                x.PostJsonAsync<TwitterMessageEventSingle>(
                    new Uri("direct_messages/events/new.json", UriKind.Relative),
                    responseText)
            )
            .ReturnsAsync(LazyJson.Create(new TwitterMessageEventSingle()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.DirectMessagesEventsNew(recipientId: 12345L, text: "hogehoge", mediaId: 67890L)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task DirectMessagesEventsDestroy_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.DeleteAsync(
                    new Uri("direct_messages/events/destroy.json?id=100", UriKind.Relative))
            )
            .Returns(Task.CompletedTask);

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.DirectMessagesEventsDestroy(eventId: "100")
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task UsersShow_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterUser>(
                    new Uri("users/show.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "screen_name", "twitterapi" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                    },
                    "/users/show/:id")
            )
            .ReturnsAsync(new TwitterUser { ScreenName = "twitterapi" });

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.UsersShow(screenName: "twitterapi")
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task UsersLookup_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterUser[]>(
                    new Uri("users/lookup.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "user_id", "11111,22222" },
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                    },
                    "/users/lookup")
            )
            .ReturnsAsync(Array.Empty<TwitterUser>());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.UsersLookup(userIds: new[] { "11111", "22222" })
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task UsersReportSpam_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterUser>(
                    new Uri("users/report_spam.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "screen_name", "twitterapi" },
                            { "tweet_mode", "extended" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterUser { ScreenName = "twitterapi" }));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.UsersReportSpam(screenName: "twitterapi")
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task FavoritesList_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterStatus[]>(
                    new Uri("favorites/list.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                            { "count", "200" },
                            { "max_id", "900" },
                            { "since_id", "100" },
                    },
                    "/favorites/list")
            )
            .ReturnsAsync(Array.Empty<TwitterStatus>());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FavoritesList(200, maxId: 900L, sinceId: 100L)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task FavoritesCreate_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterStatus>(
                    new Uri("favorites/create.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "id", "100" },
                            { "tweet_mode", "extended" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterStatus { Id = 100L }));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FavoritesCreate(statusId: 100L)
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task FavoritesDestroy_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterStatus>(
                    new Uri("favorites/destroy.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "id", "100" },
                            { "tweet_mode", "extended" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterStatus { Id = 100L }));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FavoritesDestroy(statusId: 100L)
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task FriendshipsShow_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterFriendship>(
                    new Uri("friendships/show.json", UriKind.Relative),
                    new Dictionary<string, string> { { "source_screen_name", "twitter" }, { "target_screen_name", "twitterapi" } },
                    "/friendships/show")
            )
            .ReturnsAsync(new TwitterFriendship());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FriendshipsShow(sourceScreenName: "twitter", targetScreenName: "twitterapi")
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task FriendshipsCreate_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterFriendship>(
                    new Uri("friendships/create.json", UriKind.Relative),
                    new Dictionary<string, string> { { "screen_name", "twitterapi" } })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterFriendship()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FriendshipsCreate(screenName: "twitterapi")
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task FriendshipsDestroy_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterFriendship>(
                    new Uri("friendships/destroy.json", UriKind.Relative),
                    new Dictionary<string, string> { { "screen_name", "twitterapi" } })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterFriendship()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FriendshipsDestroy(screenName: "twitterapi")
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task NoRetweetIds_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<long[]>(
                    new Uri("friendships/no_retweets/ids.json", UriKind.Relative),
                    null,
                    "/friendships/no_retweets/ids")
            )
            .ReturnsAsync(Array.Empty<long>());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.NoRetweetIds()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task FollowersIds_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterIds>(
                    new Uri("followers/ids.json", UriKind.Relative),
                    new Dictionary<string, string> { { "cursor", "-1" } },
                    "/followers/ids")
            )
            .ReturnsAsync(new TwitterIds());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FollowersIds(cursor: -1L)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task MutesUsersIds_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterIds>(
                    new Uri("mutes/users/ids.json", UriKind.Relative),
                    new Dictionary<string, string> { { "cursor", "-1" } },
                    "/mutes/users/ids")
            )
            .ReturnsAsync(new TwitterIds());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MutesUsersIds(cursor: -1L)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task BlocksIds_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterIds>(
                    new Uri("blocks/ids.json", UriKind.Relative),
                    new Dictionary<string, string> { { "cursor", "-1" } },
                    "/blocks/ids")
            )
            .ReturnsAsync(new TwitterIds());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.BlocksIds(cursor: -1L)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task BlocksCreate_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterUser>(
                    new Uri("blocks/create.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "screen_name", "twitterapi" },
                            { "tweet_mode", "extended" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterUser()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.BlocksCreate(screenName: "twitterapi")
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task BlocksDestroy_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterUser>(
                    new Uri("blocks/destroy.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "screen_name", "twitterapi" },
                            { "tweet_mode", "extended" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterUser()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.BlocksDestroy(screenName: "twitterapi")
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task AccountVerifyCredentials_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterUser>(
                    new Uri("account/verify_credentials.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                    },
                    "/account/verify_credentials")
            )
            .ReturnsAsync(new TwitterUser
            {
                Id = 100L,
                ScreenName = "opentween",
            });

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.AccountVerifyCredentials()
                .ConfigureAwait(false);

            Assert.Equal(100L, twitterApi.CurrentUserId);
            Assert.Equal("opentween", twitterApi.CurrentScreenName);

            mock.VerifyAll();
        }

        [Fact]
        public async Task AccountUpdateProfile_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterUser>(
                    new Uri("account/update_profile.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                            { "name", "Name" },
                            { "url", "http://example.com/" },
                            { "location", "Location" },
                            { "description", "&lt;script&gt;alert(1)&lt;/script&gt;" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterUser()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.AccountUpdateProfile(name: "Name", url: "http://example.com/", location: "Location", description: "<script>alert(1)</script>")
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task AccountUpdateProfileImage_Test()
        {
            using var image = TestUtils.CreateDummyImage();
            using var media = new MemoryImageMediaItem(image);
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterUser>(
                    new Uri("account/update_profile_image.json", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                            { "include_entities", "true" },
                            { "include_ext_alt_text", "true" },
                            { "tweet_mode", "extended" },
                    },
                    new Dictionary<string, IMediaItem> { { "image", media } })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterUser()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.AccountUpdateProfileImage(media)
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ApplicationRateLimitStatus_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterRateLimits>(
                    new Uri("application/rate_limit_status.json", UriKind.Relative),
                    null,
                    "/application/rate_limit_status")
            )
            .ReturnsAsync(new TwitterRateLimits());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ApplicationRateLimitStatus()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task Configuration_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterConfiguration>(
                    new Uri("help/configuration.json", UriKind.Relative),
                    null,
                    "/help/configuration")
            )
            .ReturnsAsync(new TwitterConfiguration());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.Configuration()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task MediaUploadInit_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterUploadMediaInit>(
                    new Uri("https://upload.twitter.com/1.1/media/upload.json", UriKind.Absolute),
                    new Dictionary<string, string>
                    {
                            { "command", "INIT" },
                            { "total_bytes", "123456" },
                            { "media_type", "image/png" },
                            { "media_category", "dm_image" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterUploadMediaInit()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MediaUploadInit(totalBytes: 123456L, mediaType: "image/png", mediaCategory: "dm_image")
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task MediaUploadAppend_Test()
        {
            using var image = TestUtils.CreateDummyImage();
            using var media = new MemoryImageMediaItem(image);
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostAsync(
                    new Uri("https://upload.twitter.com/1.1/media/upload.json", UriKind.Absolute),
                    new Dictionary<string, string>
                    {
                            { "command", "APPEND" },
                            { "media_id", "11111" },
                            { "segment_index", "1" },
                    },
                    new Dictionary<string, IMediaItem> { { "media", media } })
            )
            .Returns(Task.CompletedTask);

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MediaUploadAppend(mediaId: 11111L, segmentIndex: 1, media: media)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task MediaUploadFinalize_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostLazyAsync<TwitterUploadMediaResult>(
                    new Uri("https://upload.twitter.com/1.1/media/upload.json", UriKind.Absolute),
                    new Dictionary<string, string>
                    {
                            { "command", "FINALIZE" },
                            { "media_id", "11111" },
                    })
            )
            .ReturnsAsync(LazyJson.Create(new TwitterUploadMediaResult()));

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MediaUploadFinalize(mediaId: 11111L)
                .IgnoreResponse()
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task MediaUploadStatus_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterUploadMediaResult>(
                    new Uri("https://upload.twitter.com/1.1/media/upload.json", UriKind.Absolute),
                    new Dictionary<string, string>
                    {
                            { "command", "STATUS" },
                            { "media_id", "11111" },
                    },
                    null)
            )
            .ReturnsAsync(new TwitterUploadMediaResult());

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MediaUploadStatus(mediaId: 11111L)
                .ConfigureAwait(false);

            mock.VerifyAll();
        }

        [Fact]
        public async Task MediaMetadataCreate_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.PostJsonAsync(
                    new Uri("https://upload.twitter.com/1.1/media/metadata/create.json", UriKind.Absolute),
                    "{\"media_id\": \"12345\", \"alt_text\": {\"text\": \"hogehoge\"}}")
            )
            .Returns(Task.CompletedTask);

            using var twitterApi = new TwitterApi(ApiKey.Create("fake_consumer_key"), ApiKey.Create("fake_consumer_secret"));
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MediaMetadataCreate(mediaId: 12345L, altText: "hogehoge")
                .ConfigureAwait(false);

            mock.VerifyAll();
        }
    }
}
