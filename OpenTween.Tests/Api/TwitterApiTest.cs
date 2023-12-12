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
using System.Net;
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
            using var twitterApi = new TwitterApi();
            var apiConnection = Assert.IsType<TwitterApiConnection>(twitterApi.Connection);
            Assert.IsType<TwitterCredentialNone>(apiConnection.Credential);

            var credential = new TwitterCredentialOAuth1(TwitterAppToken.GetDefault(), "*** AccessToken ***", "*** AccessSecret ***");
            twitterApi.Initialize(credential, userId: 100L, screenName: "hogehoge");

            apiConnection = Assert.IsType<TwitterApiConnection>(twitterApi.Connection);
            Assert.Same(credential, apiConnection.Credential);

            Assert.Equal(100L, twitterApi.CurrentUserId);
            Assert.Equal("hogehoge", twitterApi.CurrentScreenName);

            // 複数回 Initialize を実行した場合は新たに TwitterApiConnection が生成される
            var credential2 = new TwitterCredentialOAuth1(TwitterAppToken.GetDefault(), "*** AccessToken2 ***", "*** AccessSecret2 ***");
            twitterApi.Initialize(credential2, userId: 200L, screenName: "foobar");

            var oldApiConnection = apiConnection;
            Assert.True(oldApiConnection.IsDisposed);

            apiConnection = Assert.IsType<TwitterApiConnection>(twitterApi.Connection);
            Assert.Same(credential2, apiConnection.Credential);

            Assert.Equal(200L, twitterApi.CurrentUserId);
            Assert.Equal("foobar", twitterApi.CurrentScreenName);
        }

        private Mock<IApiConnection> CreateApiConnectionMock<T>(Action<T> verifyRequest)
            where T : IHttpRequest
            => this.CreateApiConnectionMock(verifyRequest, "");

        private Mock<IApiConnection> CreateApiConnectionMock<T>(Action<T> verifyRequest, string responseText)
            where T : IHttpRequest
        {
            Func<T, bool> verifyRequestWrapper = r =>
            {
                verifyRequest(r);
                // Assert メソッドを使用する想定のため、失敗した場合は例外が発生しここまで到達しない
                return true;
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseText),
            };
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.SendAsync(
                    It.Is<T>(r => verifyRequestWrapper(r))
                )
            )
            .ReturnsAsync(new ApiResponse(responseMessage));

            return mock;
        }

        [Fact]
        public async Task StatusesHomeTimeline_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("statuses/home_timeline.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                        ["count"] = "200",
                        ["max_id"] = "900",
                        ["since_id"] = "100",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/statuses/home_timeline", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(Array.Empty<TwitterStatus>())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesHomeTimeline(200, maxId: new("900"), sinceId: new("100"));

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesMentionsTimeline_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("statuses/mentions_timeline.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                        ["count"] = "200",
                        ["max_id"] = "900",
                        ["since_id"] = "100",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/statuses/mentions_timeline", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(Array.Empty<TwitterStatus>())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesMentionsTimeline(200, maxId: new("900"), sinceId: new("100"));

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesUserTimeline_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("statuses/user_timeline.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["screen_name"] = "twitterapi",
                        ["include_rts"] = "true",
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                        ["count"] = "200",
                        ["max_id"] = "900",
                        ["since_id"] = "100",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/statuses/user_timeline", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(Array.Empty<TwitterStatus>())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesUserTimeline("twitterapi", count: 200, maxId: new("900"), sinceId: new("100"));

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesShow_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("statuses/show.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["id"] = "100",
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/statuses/show/:id", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterStatus())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesShow(statusId: new("100"));

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesLookup_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("statuses/lookup.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["id"] = "100,200",
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/statuses/lookup", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(Array.Empty<TwitterStatus>())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesLookup(statusIds: new[] { "100", "200" });

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesUpdate_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("statuses/update.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["status"] = "hogehoge",
                    ["include_entities"] = "true",
                    ["include_ext_alt_text"] = "true",
                    ["tweet_mode"] = "extended",
                    ["in_reply_to_status_id"] = "100",
                    ["media_ids"] = "10,20",
                    ["auto_populate_reply_metadata"] = "true",
                    ["exclude_reply_user_ids"] = "100,200",
                    ["attachment_url"] = "https://twitter.com/twitterapi/status/22634515958",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesUpdate(
                    "hogehoge",
                    replyToId: new("100"),
                    mediaIds: new[] { 10L, 20L },
                    autoPopulateReplyMetadata: true,
                    excludeReplyUserIds: new[] { 100L, 200L },
                    attachmentUrl: "https://twitter.com/twitterapi/status/22634515958"
                )
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesUpdate_ExcludeReplyUserIdsEmptyTest()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("statuses/update.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["status"] = "hogehoge",
                    ["include_entities"] = "true",
                    ["include_ext_alt_text"] = "true",
                    ["tweet_mode"] = "extended",
                    // exclude_reply_user_ids は空の場合には送信されない
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesUpdate("hogehoge", replyToId: null, mediaIds: null, excludeReplyUserIds: Array.Empty<long>())
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesDestroy_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("statuses/destroy.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["id"] = "100",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesDestroy(statusId: new("100"))
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task StatusesRetweet_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("statuses/retweet.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["id"] = "100",
                    ["include_entities"] = "true",
                    ["include_ext_alt_text"] = "true",
                    ["tweet_mode"] = "extended",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.StatusesRetweet(new("100"))
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task SearchTweets_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("search/tweets.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["q"] = "from:twitterapi",
                        ["result_type"] = "recent",
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                        ["lang"] = "en",
                        ["count"] = "200",
                        ["max_id"] = "900",
                        ["since_id"] = "100",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/search/tweets", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterSearchResult())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.SearchTweets("from:twitterapi", "en", count: 200, maxId: new("900"), sinceId: new("100"));

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsOwnerships_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("lists/ownerships.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["screen_name"] = "twitterapi",
                        ["cursor"] = "-1",
                        ["count"] = "100",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/lists/ownerships", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterLists())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsOwnerships("twitterapi", cursor: -1L, count: 100);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsSubscriptions_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("lists/subscriptions.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["screen_name"] = "twitterapi",
                        ["cursor"] = "-1",
                        ["count"] = "100",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/lists/subscriptions", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterLists())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsSubscriptions("twitterapi", cursor: -1L, count: 100);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsMemberships_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("lists/memberships.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["screen_name"] = "twitterapi",
                        ["cursor"] = "-1",
                        ["count"] = "100",
                        ["filter_to_owned_lists"] = "true",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/lists/memberships", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterLists())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsMemberships("twitterapi", cursor: -1L, count: 100, filterToOwnedLists: true);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsCreate_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("lists/create.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["name"] = "hogehoge",
                    ["description"] = "aaaa",
                    ["mode"] = "private",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsCreate("hogehoge", description: "aaaa", @private: true)
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsUpdate_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("lists/update.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["list_id"] = "12345",
                    ["name"] = "hogehoge",
                    ["description"] = "aaaa",
                    ["mode"] = "private",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsUpdate(12345L, name: "hogehoge", description: "aaaa", @private: true)
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsDestroy_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("lists/destroy.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["list_id"] = "12345",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsDestroy(12345L)
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsStatuses_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("lists/statuses.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["list_id"] = "12345",
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                        ["count"] = "200",
                        ["max_id"] = "900",
                        ["since_id"] = "100",
                        ["include_rts"] = "true",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/lists/statuses", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(Array.Empty<TwitterStatus>())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsStatuses(12345L, count: 200, maxId: new("900"), sinceId: new("100"), includeRTs: true);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsMembers_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("lists/members.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["list_id"] = "12345",
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                        ["cursor"] = "-1",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/lists/members", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(Array.Empty<TwitterUser>())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsMembers(12345L, cursor: -1);

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsMembersShow_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("lists/members/show.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["list_id"] = "12345",
                        ["screen_name"] = "twitterapi",
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/lists/members/show", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterUser())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsMembersShow(12345L, "twitterapi");

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsMembersCreate_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("lists/members/create.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["list_id"] = "12345",
                    ["screen_name"] = "twitterapi",
                    ["include_entities"] = "true",
                    ["include_ext_alt_text"] = "true",
                    ["tweet_mode"] = "extended",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsMembersCreate(12345L, "twitterapi")
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task ListsMembersDestroy_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("lists/members/destroy.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["list_id"] = "12345",
                    ["screen_name"] = "twitterapi",
                    ["include_entities"] = "true",
                    ["include_ext_alt_text"] = "true",
                    ["tweet_mode"] = "extended",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ListsMembersDestroy(12345L, "twitterapi")
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task DirectMessagesEventsList_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("direct_messages/events/list.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["count"] = "50",
                        ["cursor"] = "12345abcdefg",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/direct_messages/events/list", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterMessageEventList())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.DirectMessagesEventsList(count: 50, cursor: "12345abcdefg");

            mock.VerifyAll();
        }

        [Fact]
        public async Task DirectMessagesEventsNew_Test()
        {
            var requestJson = """
                {
                  "event": {
                    "type": "message_create",
                    "message_create": {
                      "target": {
                        "recipient_id": "12345"
                      },
                      "message_data": {
                        "text": "hogehoge",
                        "attachment": {
                          "type": "media",
                          "media": {
                            "id": "67890"
                          }
                        }
                      }
                    }
                  }
                }
                """;

            var mock = this.CreateApiConnectionMock<PostJsonRequest>(r =>
            {
                Assert.Equal(new("direct_messages/events/new.json", UriKind.Relative), r.RequestUri);
                Assert.Equal(requestJson, r.JsonString);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.DirectMessagesEventsNew(recipientId: 12345L, text: "hogehoge", mediaId: 67890L);

            mock.VerifyAll();
        }

        [Fact]
        public async Task DirectMessagesEventsDestroy_Test()
        {
            var mock = this.CreateApiConnectionMock<DeleteRequest>(r =>
            {
                Assert.Equal(new("direct_messages/events/destroy.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["id"] = "100",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.DirectMessagesEventsDestroy(eventId: new("100"));

            mock.VerifyAll();
        }

        [Fact]
        public async Task UsersShow_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("users/show.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["screen_name"] = "twitterapi",
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/users/show/:id", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterUser())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.UsersShow(screenName: "twitterapi");

            mock.VerifyAll();
        }

        [Fact]
        public async Task UsersLookup_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("users/lookup.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["user_id"] = "11111,22222",
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/users/lookup", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(Array.Empty<TwitterUser>())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.UsersLookup(userIds: new[] { "11111", "22222" });

            mock.VerifyAll();
        }

        [Fact]
        public async Task UsersReportSpam_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("users/report_spam.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["screen_name"] = "twitterapi",
                    ["tweet_mode"] = "extended",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.UsersReportSpam(screenName: "twitterapi")
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task FavoritesList_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("favorites/list.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["include_entities"] = "true",
                        ["include_ext_alt_text"] = "true",
                        ["tweet_mode"] = "extended",
                        ["count"] = "200",
                        ["max_id"] = "900",
                        ["since_id"] = "100",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/favorites/list", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterStatus())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FavoritesList(200, maxId: 900L, sinceId: 100L);

            mock.VerifyAll();
        }

        [Fact]
        public async Task FavoritesCreate_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("favorites/create.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["id"] = "100",
                    ["tweet_mode"] = "extended",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FavoritesCreate(statusId: new("100"))
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task FavoritesDestroy_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("favorites/destroy.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["id"] = "100",
                    ["tweet_mode"] = "extended",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FavoritesDestroy(statusId: new("100"))
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task FriendshipsShow_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("friendships/show.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["source_screen_name"] = "twitter",
                        ["target_screen_name"] = "twitterapi",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/friendships/show", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterFriendship())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FriendshipsShow(sourceScreenName: "twitter", targetScreenName: "twitterapi");

            mock.VerifyAll();
        }

        [Fact]
        public async Task FriendshipsCreate_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("friendships/create.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["screen_name"] = "twitterapi",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FriendshipsCreate(screenName: "twitterapi")
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task FriendshipsDestroy_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("friendships/destroy.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["screen_name"] = "twitterapi",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FriendshipsDestroy(screenName: "twitterapi")
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task NoRetweetIds_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("friendships/no_retweets/ids.json", UriKind.Relative), r.RequestUri);
                    Assert.Null(r.Query);
                    Assert.Equal("/friendships/no_retweets/ids", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(Array.Empty<long>())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.NoRetweetIds();

            mock.VerifyAll();
        }

        [Fact]
        public async Task FollowersIds_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("followers/ids.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["cursor"] = "-1",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/followers/ids", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterIds())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.FollowersIds(cursor: -1L);

            mock.VerifyAll();
        }

        [Fact]
        public async Task MutesUsersIds_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("mutes/users/ids.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["cursor"] = "-1",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/mutes/users/ids", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterIds())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MutesUsersIds(cursor: -1L);

            mock.VerifyAll();
        }

        [Fact]
        public async Task BlocksIds_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("blocks/ids.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["cursor"] = "-1",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/blocks/ids", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterIds())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.BlocksIds(cursor: -1L);

            mock.VerifyAll();
        }

        [Fact]
        public async Task BlocksCreate_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("blocks/create.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["screen_name"] = "twitterapi",
                    ["tweet_mode"] = "extended",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.BlocksCreate(screenName: "twitterapi")
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task BlocksDestroy_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("blocks/destroy.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["screen_name"] = "twitterapi",
                    ["tweet_mode"] = "extended",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.BlocksDestroy(screenName: "twitterapi")
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task AccountVerifyCredentials_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("account/verify_credentials.json", UriKind.Relative), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        { "include_entities", "true" },
                        { "include_ext_alt_text", "true" },
                        { "tweet_mode", "extended" },
                    };
                    Assert.Equal(expectedQuery, r.Query);
                    Assert.Equal("/account/verify_credentials", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterUser
                {
                    Id = 100L,
                    ScreenName = "opentween",
                })
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.AccountVerifyCredentials();

            Assert.Equal(100L, twitterApi.CurrentUserId);
            Assert.Equal("opentween", twitterApi.CurrentScreenName);

            mock.VerifyAll();
        }

        [Fact]
        public async Task AccountUpdateProfile_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("account/update_profile.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["include_entities"] = "true",
                    ["include_ext_alt_text"] = "true",
                    ["tweet_mode"] = "extended",
                    ["name"] = "Name",
                    ["url"] = "http://example.com/",
                    ["location"] = "Location",
                    ["description"] = "&lt;script&gt;alert(1)&lt;/script&gt;",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.AccountUpdateProfile(name: "Name", url: "http://example.com/", location: "Location", description: "<script>alert(1)</script>")
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task AccountUpdateProfileImage_Test()
        {
            using var image = TestUtils.CreateDummyImage();
            using var media = new MemoryImageMediaItem(image);

            var mock = this.CreateApiConnectionMock<PostMultipartRequest>(r =>
            {
                Assert.Equal(new("account/update_profile_image.json", UriKind.Relative), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["include_entities"] = "true",
                    ["include_ext_alt_text"] = "true",
                    ["tweet_mode"] = "extended",
                };
                Assert.Equal(expectedQuery, r.Query);
                var expectedMedia = new Dictionary<string, IMediaItem>
                {
                    ["image"] = media,
                };
                Assert.Equal(expectedMedia, r.Media);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.AccountUpdateProfileImage(media)
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task ApplicationRateLimitStatus_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("application/rate_limit_status.json", UriKind.Relative), r.RequestUri);
                    Assert.Null(r.Query);
                    Assert.Equal("/application/rate_limit_status", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterRateLimits())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.ApplicationRateLimitStatus();

            mock.VerifyAll();
        }

        [Fact]
        public async Task Configuration_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("help/configuration.json", UriKind.Relative), r.RequestUri);
                    Assert.Null(r.Query);
                    Assert.Equal("/help/configuration", r.EndpointName);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterConfiguration())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.Configuration();

            mock.VerifyAll();
        }

        [Fact]
        public async Task MediaUploadInit_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("https://upload.twitter.com/1.1/media/upload.json"), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["command"] = "INIT",
                    ["total_bytes"] = "123456",
                    ["media_type"] = "image/png",
                    ["media_category"] = "dm_image",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MediaUploadInit(totalBytes: 123456L, mediaType: "image/png", mediaCategory: "dm_image")
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task MediaUploadAppend_Test()
        {
            using var image = TestUtils.CreateDummyImage();
            using var media = new MemoryImageMediaItem(image);

            var mock = this.CreateApiConnectionMock<PostMultipartRequest>(r =>
            {
                Assert.Equal(new("https://upload.twitter.com/1.1/media/upload.json"), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["command"] = "APPEND",
                    ["media_id"] = "11111",
                    ["segment_index"] = "1",
                };
                Assert.Equal(expectedQuery, r.Query);
                var expectedMedia = new Dictionary<string, IMediaItem>
                {
                    ["media"] = media,
                };
                Assert.Equal(expectedMedia, r.Media);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MediaUploadAppend(mediaId: 11111L, segmentIndex: 1, media: media);

            mock.VerifyAll();
        }

        [Fact]
        public async Task MediaUploadFinalize_Test()
        {
            var mock = this.CreateApiConnectionMock<PostRequest>(r =>
            {
                Assert.Equal(new("https://upload.twitter.com/1.1/media/upload.json"), r.RequestUri);
                var expectedQuery = new Dictionary<string, string>
                {
                    ["command"] = "FINALIZE",
                    ["media_id"] = "11111",
                };
                Assert.Equal(expectedQuery, r.Query);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MediaUploadFinalize(mediaId: 11111L)
                .IgnoreResponse();

            mock.VerifyAll();
        }

        [Fact]
        public async Task MediaUploadStatus_Test()
        {
            var mock = this.CreateApiConnectionMock<GetRequest>(
                r =>
                {
                    Assert.Equal(new("https://upload.twitter.com/1.1/media/upload.json"), r.RequestUri);
                    var expectedQuery = new Dictionary<string, string>
                    {
                        ["command"] = "STATUS",
                        ["media_id"] = "11111",
                    };
                    Assert.Equal(expectedQuery, r.Query);
                },
                JsonUtils.SerializeJsonByDataContract(new TwitterUploadMediaResult())
            );

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MediaUploadStatus(mediaId: 11111L);

            mock.VerifyAll();
        }

        [Fact]
        public async Task MediaMetadataCreate_Test()
        {
            var mock = this.CreateApiConnectionMock<PostJsonRequest>(r =>
            {
                Assert.Equal(new("https://upload.twitter.com/1.1/media/metadata/create.json"), r.RequestUri);
                Assert.Equal("""{"media_id": "12345", "alt_text": {"text": "hogehoge"}}""", r.JsonString);
            });

            using var twitterApi = new TwitterApi();
            twitterApi.ApiConnection = mock.Object;

            await twitterApi.MediaMetadataCreate(mediaId: 12345L, altText: "hogehoge");

            mock.VerifyAll();
        }
    }
}
