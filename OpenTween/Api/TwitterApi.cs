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

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Api.DataModel;
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween.Api
{
    public sealed class TwitterApi : IDisposable
    {
        public long CurrentUserId { get; private set; }

        public string CurrentScreenName { get; private set; } = "";

        public IApiConnectionLegacy Connection => this.ApiConnection;

        internal IApiConnectionLegacy ApiConnection;

        public APIAuthType AuthType { get; private set; } = APIAuthType.None;

        public TwitterApi()
            => this.ApiConnection = new TwitterApiConnection(new TwitterCredentialNone());

        public void Initialize(ITwitterCredential credential, long userId, string screenName)
        {
            this.AuthType = credential.AuthType;

            var newInstance = new TwitterApiConnection(credential);
            var oldInstance = Interlocked.Exchange(ref this.ApiConnection, newInstance);
            oldInstance?.Dispose();

            this.CurrentUserId = userId;
            this.CurrentScreenName = screenName;
        }

        public Task<TwitterStatus[]> StatusesHomeTimeline(int? count = null, TwitterStatusId? maxId = null, TwitterStatusId? sinceId = null)
        {
            var endpoint = new Uri("statuses/home_timeline.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.Id;
            if (sinceId != null)
                param["since_id"] = sinceId.Id;

            return this.Connection.GetAsync<TwitterStatus[]>(endpoint, param, "/statuses/home_timeline");
        }

        public Task<TwitterStatus[]> StatusesMentionsTimeline(int? count = null, TwitterStatusId? maxId = null, TwitterStatusId? sinceId = null)
        {
            var endpoint = new Uri("statuses/mentions_timeline.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.Id;
            if (sinceId != null)
                param["since_id"] = sinceId.Id;

            return this.Connection.GetAsync<TwitterStatus[]>(endpoint, param, "/statuses/mentions_timeline");
        }

        public Task<TwitterStatus[]> StatusesUserTimeline(string screenName, int? count = null, TwitterStatusId? maxId = null, TwitterStatusId? sinceId = null)
        {
            var endpoint = new Uri("statuses/user_timeline.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
                ["include_rts"] = "true",
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.Id;
            if (sinceId != null)
                param["since_id"] = sinceId.Id;

            return this.Connection.GetAsync<TwitterStatus[]>(endpoint, param, "/statuses/user_timeline");
        }

        public Task<TwitterStatus> StatusesShow(TwitterStatusId statusId)
        {
            var endpoint = new Uri("statuses/show.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.Id,
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            return this.Connection.GetAsync<TwitterStatus>(endpoint, param, "/statuses/show/:id");
        }

        public Task<TwitterStatus[]> StatusesLookup(IReadOnlyList<string> statusIds)
        {
            var endpoint = new Uri("statuses/lookup.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = string.Join(",", statusIds),
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            return this.Connection.GetAsync<TwitterStatus[]>(endpoint, param, "/statuses/lookup");
        }

        public async Task<LazyJson<TwitterStatus>> StatusesUpdate(
            string status,
            TwitterStatusId? replyToId,
            IReadOnlyList<long>? mediaIds,
            bool? autoPopulateReplyMetadata = null,
            IReadOnlyList<long>? excludeReplyUserIds = null,
            string? attachmentUrl = null)
        {
            var param = new Dictionary<string, string>
            {
                ["status"] = status,
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            if (replyToId != null)
                param["in_reply_to_status_id"] = replyToId.Id;
            if (mediaIds != null && mediaIds.Count > 0)
                param.Add("media_ids", string.Join(",", mediaIds));
            if (autoPopulateReplyMetadata != null)
                param["auto_populate_reply_metadata"] = autoPopulateReplyMetadata.Value ? "true" : "false";
            if (excludeReplyUserIds != null && excludeReplyUserIds.Count > 0)
                param["exclude_reply_user_ids"] = string.Join(",", excludeReplyUserIds);
            if (attachmentUrl != null)
                param["attachment_url"] = attachmentUrl;

            var request = new PostRequest
            {
                RequestUri = new("statuses/update.json", UriKind.Relative),
                Query = param,
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterStatus>();
        }

        public async Task<LazyJson<TwitterStatus>> StatusesDestroy(TwitterStatusId statusId)
        {
            var request = new PostRequest
            {
                RequestUri = new("statuses/destroy.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["id"] = statusId.Id,
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterStatus>();
        }

        public async Task<LazyJson<TwitterStatus>> StatusesRetweet(TwitterStatusId statusId)
        {
            var request = new PostRequest
            {
                RequestUri = new("statuses/retweet.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["id"] = statusId.Id,
                    ["include_entities"] = "true",
                    ["include_ext_alt_text"] = "true",
                    ["tweet_mode"] = "extended",
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterStatus>();
        }

        public Task<TwitterSearchResult> SearchTweets(string query, string? lang = null, int? count = null, TwitterStatusId? maxId = null, TwitterStatusId? sinceId = null)
        {
            var endpoint = new Uri("search/tweets.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["q"] = query,
                ["result_type"] = "recent",
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            if (lang != null)
                param["lang"] = lang;
            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.Id;
            if (sinceId != null)
                param["since_id"] = sinceId.Id;

            return this.Connection.GetAsync<TwitterSearchResult>(endpoint, param, "/search/tweets");
        }

        public Task<TwitterLists> ListsOwnerships(string screenName, long? cursor = null, int? count = null)
        {
            var endpoint = new Uri("lists/ownerships.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            if (cursor != null)
                param["cursor"] = cursor.ToString();
            if (count != null)
                param["count"] = count.ToString();

            return this.Connection.GetAsync<TwitterLists>(endpoint, param, "/lists/ownerships");
        }

        public Task<TwitterLists> ListsSubscriptions(string screenName, long? cursor = null, int? count = null)
        {
            var endpoint = new Uri("lists/subscriptions.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            if (cursor != null)
                param["cursor"] = cursor.ToString();
            if (count != null)
                param["count"] = count.ToString();

            return this.Connection.GetAsync<TwitterLists>(endpoint, param, "/lists/subscriptions");
        }

        public Task<TwitterLists> ListsMemberships(string screenName, long? cursor = null, int? count = null, bool? filterToOwnedLists = null)
        {
            var endpoint = new Uri("lists/memberships.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            if (cursor != null)
                param["cursor"] = cursor.ToString();
            if (count != null)
                param["count"] = count.ToString();
            if (filterToOwnedLists != null)
                param["filter_to_owned_lists"] = filterToOwnedLists.Value ? "true" : "false";

            return this.Connection.GetAsync<TwitterLists>(endpoint, param, "/lists/memberships");
        }

        public async Task<LazyJson<TwitterList>> ListsCreate(string name, string? description = null, bool? @private = null)
        {
            var param = new Dictionary<string, string>
            {
                ["name"] = name,
            };

            if (description != null)
                param["description"] = description;
            if (@private != null)
                param["mode"] = @private.Value ? "private" : "public";

            var request = new PostRequest
            {
                RequestUri = new("lists/create.json", UriKind.Relative),
                Query = param,
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterList>();
        }

        public async Task<LazyJson<TwitterList>> ListsUpdate(long listId, string? name = null, string? description = null, bool? @private = null)
        {
            var param = new Dictionary<string, string>
            {
                ["list_id"] = listId.ToString(),
            };

            if (name != null)
                param["name"] = name;
            if (description != null)
                param["description"] = description;
            if (@private != null)
                param["mode"] = @private.Value ? "private" : "public";

            var request = new PostRequest
            {
                RequestUri = new("lists/update.json", UriKind.Relative),
                Query = param,
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterList>();
        }

        public async Task<LazyJson<TwitterList>> ListsDestroy(long listId)
        {
            var request = new PostRequest
            {
                RequestUri = new("lists/destroy.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["list_id"] = listId.ToString(),
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterList>();
        }

        public Task<TwitterStatus[]> ListsStatuses(long listId, int? count = null, TwitterStatusId? maxId = null, TwitterStatusId? sinceId = null, bool? includeRTs = null)
        {
            var endpoint = new Uri("lists/statuses.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["list_id"] = listId.ToString(),
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.Id;
            if (sinceId != null)
                param["since_id"] = sinceId.Id;
            if (includeRTs != null)
                param["include_rts"] = includeRTs.Value ? "true" : "false";

            return this.Connection.GetAsync<TwitterStatus[]>(endpoint, param, "/lists/statuses");
        }

        public Task<TwitterUsers> ListsMembers(long listId, long? cursor = null)
        {
            var endpoint = new Uri("lists/members.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["list_id"] = listId.ToString(),
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            if (cursor != null)
                param["cursor"] = cursor.ToString();

            return this.Connection.GetAsync<TwitterUsers>(endpoint, param, "/lists/members");
        }

        public Task<TwitterUser> ListsMembersShow(long listId, string screenName)
        {
            var endpoint = new Uri("lists/members/show.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["list_id"] = listId.ToString(),
                ["screen_name"] = screenName,
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            return this.Connection.GetAsync<TwitterUser>(endpoint, param, "/lists/members/show");
        }

        public async Task<LazyJson<TwitterUser>> ListsMembersCreate(long listId, string screenName)
        {
            var request = new PostRequest
            {
                RequestUri = new("lists/members/create.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["list_id"] = listId.ToString(),
                    ["screen_name"] = screenName,
                    ["include_entities"] = "true",
                    ["include_ext_alt_text"] = "true",
                    ["tweet_mode"] = "extended",
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterUser>();
        }

        public async Task<LazyJson<TwitterUser>> ListsMembersDestroy(long listId, string screenName)
        {
            var request = new PostRequest
            {
                RequestUri = new("lists/members/destroy.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["list_id"] = listId.ToString(),
                    ["screen_name"] = screenName,
                    ["include_entities"] = "true",
                    ["include_ext_alt_text"] = "true",
                    ["tweet_mode"] = "extended",
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterUser>();
        }

        public Task<TwitterMessageEventList> DirectMessagesEventsList(int? count = null, string? cursor = null)
        {
            var endpoint = new Uri("direct_messages/events/list.json", UriKind.Relative);
            var param = new Dictionary<string, string>();

            if (count != null)
                param["count"] = count.ToString();
            if (cursor != null)
                param["cursor"] = cursor;

            return this.Connection.GetAsync<TwitterMessageEventList>(endpoint, param, "/direct_messages/events/list");
        }

        public async Task<LazyJson<TwitterMessageEventSingle>> DirectMessagesEventsNew(long recipientId, string text, long? mediaId = null)
        {
            var attachment = "";
            if (mediaId != null)
            {
                attachment = ",\r\n" + $$"""
                            "attachment": {
                              "type": "media",
                              "media": {
                                "id": "{{JsonUtils.EscapeJsonString(mediaId.ToString())}}"
                              }
                            }
                    """;
            }

            var json = $$"""
                {
                  "event": {
                    "type": "message_create",
                    "message_create": {
                      "target": {
                        "recipient_id": "{{JsonUtils.EscapeJsonString(recipientId.ToString())}}"
                      },
                      "message_data": {
                        "text": "{{JsonUtils.EscapeJsonString(text)}}"{{attachment}}
                      }
                    }
                  }
                }
                """;

            var request = new PostJsonRequest
            {
                RequestUri = new("direct_messages/events/new.json", UriKind.Relative),
                JsonString = json,
            };

            var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterMessageEventSingle>();
        }

        public async Task DirectMessagesEventsDestroy(TwitterDirectMessageId eventId)
        {
            var request = new DeleteRequest
            {
                RequestUri = new("direct_messages/events/destroy.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["id"] = eventId.Id,
                },
            };

            await this.Connection.SendAsync(request)
                .IgnoreResponse()
                .ConfigureAwait(false);
        }

        public Task<TwitterUser> UsersShow(string screenName)
        {
            var endpoint = new Uri("users/show.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            return this.Connection.GetAsync<TwitterUser>(endpoint, param, "/users/show/:id");
        }

        public Task<TwitterUser[]> UsersLookup(IReadOnlyList<string> userIds)
        {
            var endpoint = new Uri("users/lookup.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["user_id"] = string.Join(",", userIds),
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            return this.Connection.GetAsync<TwitterUser[]>(endpoint, param, "/users/lookup");
        }

        public async Task<LazyJson<TwitterUser>> UsersReportSpam(string screenName)
        {
            var request = new PostRequest
            {
                RequestUri = new("users/report_spam.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["screen_name"] = screenName,
                    ["tweet_mode"] = "extended",
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterUser>();
        }

        public Task<TwitterStatus[]> FavoritesList(int? count = null, long? maxId = null, long? sinceId = null)
        {
            var endpoint = new Uri("favorites/list.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

            return this.Connection.GetAsync<TwitterStatus[]>(endpoint, param, "/favorites/list");
        }

        public async Task<LazyJson<TwitterStatus>> FavoritesCreate(TwitterStatusId statusId)
        {
            var request = new PostRequest
            {
                RequestUri = new("favorites/create.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["id"] = statusId.Id,
                    ["tweet_mode"] = "extended",
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterStatus>();
        }

        public async Task<LazyJson<TwitterStatus>> FavoritesDestroy(TwitterStatusId statusId)
        {
            var request = new PostRequest
            {
                RequestUri = new("favorites/destroy.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["id"] = statusId.Id,
                    ["tweet_mode"] = "extended",
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterStatus>();
        }

        public Task<TwitterFriendship> FriendshipsShow(string sourceScreenName, string targetScreenName)
        {
            var endpoint = new Uri("friendships/show.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["source_screen_name"] = sourceScreenName,
                ["target_screen_name"] = targetScreenName,
            };

            return this.Connection.GetAsync<TwitterFriendship>(endpoint, param, "/friendships/show");
        }

        public async Task<LazyJson<TwitterFriendship>> FriendshipsCreate(string screenName)
        {
            var request = new PostRequest
            {
                RequestUri = new("friendships/create.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["screen_name"] = screenName,
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterFriendship>();
        }

        public async Task<LazyJson<TwitterFriendship>> FriendshipsDestroy(string screenName)
        {
            var request = new PostRequest
            {
                RequestUri = new("friendships/destroy.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["screen_name"] = screenName,
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterFriendship>();
        }

        public Task<long[]> NoRetweetIds()
        {
            var endpoint = new Uri("friendships/no_retweets/ids.json", UriKind.Relative);

            return this.Connection.GetAsync<long[]>(endpoint, null, "/friendships/no_retweets/ids");
        }

        public Task<TwitterIds> FollowersIds(long? cursor = null)
        {
            var endpoint = new Uri("followers/ids.json", UriKind.Relative);
            var param = new Dictionary<string, string>();

            if (cursor != null)
                param["cursor"] = cursor.ToString();

            return this.Connection.GetAsync<TwitterIds>(endpoint, param, "/followers/ids");
        }

        public Task<TwitterIds> MutesUsersIds(long? cursor = null)
        {
            var endpoint = new Uri("mutes/users/ids.json", UriKind.Relative);
            var param = new Dictionary<string, string>();

            if (cursor != null)
                param["cursor"] = cursor.ToString();

            return this.Connection.GetAsync<TwitterIds>(endpoint, param, "/mutes/users/ids");
        }

        public Task<TwitterIds> BlocksIds(long? cursor = null)
        {
            var endpoint = new Uri("blocks/ids.json", UriKind.Relative);
            var param = new Dictionary<string, string>();

            if (cursor != null)
                param["cursor"] = cursor.ToString();

            return this.Connection.GetAsync<TwitterIds>(endpoint, param, "/blocks/ids");
        }

        public async Task<LazyJson<TwitterUser>> BlocksCreate(string screenName)
        {
            var request = new PostRequest
            {
                RequestUri = new("blocks/create.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["screen_name"] = screenName,
                    ["tweet_mode"] = "extended",
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterUser>();
        }

        public async Task<LazyJson<TwitterUser>> BlocksDestroy(string screenName)
        {
            var request = new PostRequest
            {
                RequestUri = new("blocks/destroy.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["screen_name"] = screenName,
                    ["tweet_mode"] = "extended",
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterUser>();
        }

        public async Task<TwitterUser> AccountVerifyCredentials()
        {
            var endpoint = new Uri("account/verify_credentials.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            var user = await this.Connection.GetAsync<TwitterUser>(endpoint, param, "/account/verify_credentials")
                .ConfigureAwait(false);

            this.CurrentUserId = user.Id;
            this.CurrentScreenName = user.ScreenName;

            return user;
        }

        public async Task<LazyJson<TwitterUser>> AccountUpdateProfile(string name, string url, string? location, string? description)
        {
            var param = new Dictionary<string, string>
            {
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            if (name != null)
                param["name"] = name;
            if (url != null)
                param["url"] = url;
            if (location != null)
                param["location"] = location;

            if (description != null)
            {
                // name, location, description に含まれる < > " の文字はTwitter側で除去されるが、
                // twitter.com の挙動では description でのみ &lt; 等の文字参照を使って表示することができる
                var escapedDescription = description.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
                param["description"] = escapedDescription;
            }

            var request = new PostRequest
            {
                RequestUri = new("account/update_profile.json", UriKind.Relative),
                Query = param,
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterUser>();
        }

        public async Task<LazyJson<TwitterUser>> AccountUpdateProfileImage(IMediaItem image)
        {
            var request = new PostMultipartRequest
            {
                RequestUri = new("account/update_profile_image.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["include_entities"] = "true",
                    ["include_ext_alt_text"] = "true",
                    ["tweet_mode"] = "extended",
                },
                Media = new Dictionary<string, IMediaItem>
                {
                    ["image"] = image,
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterUser>();
        }

        public Task<TwitterRateLimits> ApplicationRateLimitStatus()
        {
            var endpoint = new Uri("application/rate_limit_status.json", UriKind.Relative);

            return this.Connection.GetAsync<TwitterRateLimits>(endpoint, null, "/application/rate_limit_status");
        }

        public Task<TwitterConfiguration> Configuration()
        {
            var endpoint = new Uri("help/configuration.json", UriKind.Relative);

            return this.Connection.GetAsync<TwitterConfiguration>(endpoint, null, "/help/configuration");
        }

        public async Task<LazyJson<TwitterUploadMediaInit>> MediaUploadInit(long totalBytes, string mediaType, string? mediaCategory = null)
        {
            var param = new Dictionary<string, string>
            {
                ["command"] = "INIT",
                ["total_bytes"] = totalBytes.ToString(),
                ["media_type"] = mediaType,
            };

            if (mediaCategory != null)
                param["media_category"] = mediaCategory;

            var request = new PostRequest
            {
                RequestUri = new("https://upload.twitter.com/1.1/media/upload.json"),
                Query = param,
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterUploadMediaInit>();
        }

        public async Task MediaUploadAppend(long mediaId, int segmentIndex, IMediaItem media)
        {
            var request = new PostMultipartRequest
            {
                RequestUri = new("https://upload.twitter.com/1.1/media/upload.json"),
                Query = new Dictionary<string, string>
                {
                    ["command"] = "APPEND",
                    ["media_id"] = mediaId.ToString(),
                    ["segment_index"] = segmentIndex.ToString(),
                },
                Media = new Dictionary<string, IMediaItem>
                {
                    ["media"] = media,
                },
            };

            await this.Connection.SendAsync(request)
                .IgnoreResponse()
                .ConfigureAwait(false);
        }

        public async Task<LazyJson<TwitterUploadMediaResult>> MediaUploadFinalize(long mediaId)
        {
            var request = new PostRequest
            {
                RequestUri = new("https://upload.twitter.com/1.1/media/upload.json"),
                Query = new Dictionary<string, string>
                {
                    ["command"] = "FINALIZE",
                    ["media_id"] = mediaId.ToString(),
                },
            };

            using var response = await this.Connection.SendAsync(request)
                .ConfigureAwait(false);

            return response.ReadAsLazyJson<TwitterUploadMediaResult>();
        }

        public Task<TwitterUploadMediaResult> MediaUploadStatus(long mediaId)
        {
            var endpoint = new Uri("https://upload.twitter.com/1.1/media/upload.json");
            var param = new Dictionary<string, string>
            {
                ["command"] = "STATUS",
                ["media_id"] = mediaId.ToString(),
            };

            return this.Connection.GetAsync<TwitterUploadMediaResult>(endpoint, param, endpointName: null);
        }

        public async Task MediaMetadataCreate(long mediaId, string altText)
        {
            var escapedAltText = JsonUtils.EscapeJsonString(altText);
            var request = new PostJsonRequest
            {
                RequestUri = new("https://upload.twitter.com/1.1/media/metadata/create.json"),
                JsonString = $$$"""{"media_id": "{{{mediaId}}}", "alt_text": {"text": "{{{escapedAltText}}}"}}""",
            };

            await this.Connection.SendAsync(request)
                .IgnoreResponse()
                .ConfigureAwait(false);
        }

        public OAuthEchoHandler CreateOAuthEchoHandler(HttpMessageHandler innerHandler, Uri authServiceProvider, Uri? realm = null)
            => ((TwitterApiConnection)this.Connection).CreateOAuthEchoHandler(innerHandler, authServiceProvider, realm);

        public void Dispose()
            => this.ApiConnection?.Dispose();
    }
}
