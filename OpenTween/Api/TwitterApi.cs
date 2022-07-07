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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Api.DataModel;
using OpenTween.Connection;

namespace OpenTween.Api
{
    public sealed class TwitterApi : IDisposable
    {
        public long CurrentUserId { get; private set; }

        public string CurrentScreenName { get; private set; } = "";

        public IApiConnection Connection => this.ApiConnection ?? throw new InvalidOperationException();

        internal IApiConnection? ApiConnection;

        private readonly ApiKey consumerKey;
        private readonly ApiKey consumerSecret;

        public TwitterApi(ApiKey consumerKey, ApiKey consumerSecret)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
        }

        public void Initialize(string accessToken, string accessSecret, long userId, string screenName)
        {
            var newInstance = new TwitterApiConnection(this.consumerKey, this.consumerSecret, accessToken, accessSecret);
            var oldInstance = Interlocked.Exchange(ref this.ApiConnection, newInstance);
            oldInstance?.Dispose();

            this.CurrentUserId = userId;
            this.CurrentScreenName = screenName;
        }

        public Task<TwitterStatus[]> StatusesHomeTimeline(int? count = null, long? maxId = null, long? sinceId = null)
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
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

            return this.Connection.GetAsync<TwitterStatus[]>(endpoint, param, "/statuses/home_timeline");
        }

        public Task<TwitterStatus[]> StatusesMentionsTimeline(int? count = null, long? maxId = null, long? sinceId = null)
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
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

            return this.Connection.GetAsync<TwitterStatus[]>(endpoint, param, "/statuses/mentions_timeline");
        }

        public Task<TwitterStatus[]> StatusesUserTimeline(string screenName, int? count = null, long? maxId = null, long? sinceId = null)
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
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

            return this.Connection.GetAsync<TwitterStatus[]>(endpoint, param, "/statuses/user_timeline");
        }

        public Task<TwitterStatus> StatusesShow(long statusId)
        {
            var endpoint = new Uri("statuses/show.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.ToString(),
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

        public Task<LazyJson<TwitterStatus>> StatusesUpdate(
            string status,
            long? replyToId,
            IReadOnlyList<long>? mediaIds,
            bool? autoPopulateReplyMetadata = null,
            IReadOnlyList<long>? excludeReplyUserIds = null,
            string? attachmentUrl = null)
        {
            var endpoint = new Uri("statuses/update.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["status"] = status,
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            if (replyToId != null)
                param["in_reply_to_status_id"] = replyToId.ToString();
            if (mediaIds != null && mediaIds.Count > 0)
                param.Add("media_ids", string.Join(",", mediaIds));
            if (autoPopulateReplyMetadata != null)
                param["auto_populate_reply_metadata"] = autoPopulateReplyMetadata.Value ? "true" : "false";
            if (excludeReplyUserIds != null && excludeReplyUserIds.Count > 0)
                param["exclude_reply_user_ids"] = string.Join(",", excludeReplyUserIds);
            if (attachmentUrl != null)
                param["attachment_url"] = attachmentUrl;

            return this.Connection.PostLazyAsync<TwitterStatus>(endpoint, param);
        }

        public Task<LazyJson<TwitterStatus>> StatusesDestroy(long statusId)
        {
            var endpoint = new Uri("statuses/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.ToString(),
            };

            return this.Connection.PostLazyAsync<TwitterStatus>(endpoint, param);
        }

        public Task<LazyJson<TwitterStatus>> StatusesRetweet(long statusId)
        {
            var endpoint = new Uri("statuses/retweet.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.ToString(),
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            return this.Connection.PostLazyAsync<TwitterStatus>(endpoint, param);
        }

        public Task<TwitterSearchResult> SearchTweets(string query, string? lang = null, int? count = null, long? maxId = null, long? sinceId = null)
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
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

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

        public Task<LazyJson<TwitterList>> ListsCreate(string name, string? description = null, bool? @private = null)
        {
            var endpoint = new Uri("lists/create.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["name"] = name,
            };

            if (description != null)
                param["description"] = description;
            if (@private != null)
                param["mode"] = @private.Value ? "private" : "public";

            return this.Connection.PostLazyAsync<TwitterList>(endpoint, param);
        }

        public Task<LazyJson<TwitterList>> ListsUpdate(long listId, string? name = null, string? description = null, bool? @private = null)
        {
            var endpoint = new Uri("lists/update.json", UriKind.Relative);
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

            return this.Connection.PostLazyAsync<TwitterList>(endpoint, param);
        }

        public Task<LazyJson<TwitterList>> ListsDestroy(long listId)
        {
            var endpoint = new Uri("lists/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["list_id"] = listId.ToString(),
            };

            return this.Connection.PostLazyAsync<TwitterList>(endpoint, param);
        }

        public Task<TwitterStatus[]> ListsStatuses(long listId, int? count = null, long? maxId = null, long? sinceId = null, bool? includeRTs = null)
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
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();
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

        public Task<LazyJson<TwitterUser>> ListsMembersCreate(long listId, string screenName)
        {
            var endpoint = new Uri("lists/members/create.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["list_id"] = listId.ToString(),
                ["screen_name"] = screenName,
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            return this.Connection.PostLazyAsync<TwitterUser>(endpoint, param);
        }

        public Task<LazyJson<TwitterUser>> ListsMembersDestroy(long listId, string screenName)
        {
            var endpoint = new Uri("lists/members/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["list_id"] = listId.ToString(),
                ["screen_name"] = screenName,
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };

            return this.Connection.PostLazyAsync<TwitterUser>(endpoint, param);
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

        public Task<LazyJson<TwitterMessageEventSingle>> DirectMessagesEventsNew(long recipientId, string text, long? mediaId = null)
        {
            var endpoint = new Uri("direct_messages/events/new.json", UriKind.Relative);

            var attachment = "";
            if (mediaId != null)
            {
                attachment = "," + $@"
        ""attachment"": {{
          ""type"": ""media"",
          ""media"": {{
            ""id"": ""{JsonUtils.EscapeJsonString(mediaId.ToString())}""
          }}
        }}";
            }

            var json = $@"{{
  ""event"": {{
    ""type"": ""message_create"",
    ""message_create"": {{
      ""target"": {{
        ""recipient_id"": ""{JsonUtils.EscapeJsonString(recipientId.ToString())}""
      }},
      ""message_data"": {{
        ""text"": ""{JsonUtils.EscapeJsonString(text)}""{attachment}
      }}
    }}
  }}
}}";

            return this.Connection.PostJsonAsync<TwitterMessageEventSingle>(endpoint, json);
        }

        public Task DirectMessagesEventsDestroy(string eventId)
        {
            var endpoint = new Uri("direct_messages/events/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = eventId.ToString(),
            };

            // なぜか application/x-www-form-urlencoded でパラメーターを送ると Bad Request になる謎仕様
            endpoint = new Uri(endpoint.OriginalString + "?" + MyCommon.BuildQueryString(param), UriKind.Relative);

            return this.Connection.DeleteAsync(endpoint);
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

        public Task<LazyJson<TwitterUser>> UsersReportSpam(string screenName)
        {
            var endpoint = new Uri("users/report_spam.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
                ["tweet_mode"] = "extended",
            };

            return this.Connection.PostLazyAsync<TwitterUser>(endpoint, param);
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

        public Task<LazyJson<TwitterStatus>> FavoritesCreate(long statusId)
        {
            var endpoint = new Uri("favorites/create.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.ToString(),
                ["tweet_mode"] = "extended",
            };

            return this.Connection.PostLazyAsync<TwitterStatus>(endpoint, param);
        }

        public Task<LazyJson<TwitterStatus>> FavoritesDestroy(long statusId)
        {
            var endpoint = new Uri("favorites/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.ToString(),
                ["tweet_mode"] = "extended",
            };

            return this.Connection.PostLazyAsync<TwitterStatus>(endpoint, param);
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

        public Task<LazyJson<TwitterFriendship>> FriendshipsCreate(string screenName)
        {
            var endpoint = new Uri("friendships/create.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            return this.Connection.PostLazyAsync<TwitterFriendship>(endpoint, param);
        }

        public Task<LazyJson<TwitterFriendship>> FriendshipsDestroy(string screenName)
        {
            var endpoint = new Uri("friendships/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            return this.Connection.PostLazyAsync<TwitterFriendship>(endpoint, param);
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

        public Task<LazyJson<TwitterUser>> BlocksCreate(string screenName)
        {
            var endpoint = new Uri("blocks/create.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
                ["tweet_mode"] = "extended",
            };

            return this.Connection.PostLazyAsync<TwitterUser>(endpoint, param);
        }

        public Task<LazyJson<TwitterUser>> BlocksDestroy(string screenName)
        {
            var endpoint = new Uri("blocks/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
                ["tweet_mode"] = "extended",
            };

            return this.Connection.PostLazyAsync<TwitterUser>(endpoint, param);
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

        public Task<LazyJson<TwitterUser>> AccountUpdateProfile(string name, string url, string? location, string? description)
        {
            var endpoint = new Uri("account/update_profile.json", UriKind.Relative);
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

            return this.Connection.PostLazyAsync<TwitterUser>(endpoint, param);
        }

        public Task<LazyJson<TwitterUser>> AccountUpdateProfileImage(IMediaItem image)
        {
            var endpoint = new Uri("account/update_profile_image.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
                ["tweet_mode"] = "extended",
            };
            var paramMedia = new Dictionary<string, IMediaItem>
            {
                ["image"] = image,
            };

            return this.Connection.PostLazyAsync<TwitterUser>(endpoint, param, paramMedia);
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

        public Task<LazyJson<TwitterUploadMediaInit>> MediaUploadInit(long totalBytes, string mediaType, string? mediaCategory = null)
        {
            var endpoint = new Uri("https://upload.twitter.com/1.1/media/upload.json");
            var param = new Dictionary<string, string>
            {
                ["command"] = "INIT",
                ["total_bytes"] = totalBytes.ToString(),
                ["media_type"] = mediaType,
            };

            if (mediaCategory != null)
                param["media_category"] = mediaCategory;

            return this.Connection.PostLazyAsync<TwitterUploadMediaInit>(endpoint, param);
        }

        public Task MediaUploadAppend(long mediaId, int segmentIndex, IMediaItem media)
        {
            var endpoint = new Uri("https://upload.twitter.com/1.1/media/upload.json");
            var param = new Dictionary<string, string>
            {
                ["command"] = "APPEND",
                ["media_id"] = mediaId.ToString(),
                ["segment_index"] = segmentIndex.ToString(),
            };
            var paramMedia = new Dictionary<string, IMediaItem>
            {
                ["media"] = media,
            };

            return this.Connection.PostAsync(endpoint, param, paramMedia);
        }

        public Task<LazyJson<TwitterUploadMediaResult>> MediaUploadFinalize(long mediaId)
        {
            var endpoint = new Uri("https://upload.twitter.com/1.1/media/upload.json");
            var param = new Dictionary<string, string>
            {
                ["command"] = "FINALIZE",
                ["media_id"] = mediaId.ToString(),
            };

            return this.Connection.PostLazyAsync<TwitterUploadMediaResult>(endpoint, param);
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

        public Task MediaMetadataCreate(long mediaId, string altText)
        {
            var endpoint = new Uri("https://upload.twitter.com/1.1/media/metadata/create.json");

            var escapedAltText = JsonUtils.EscapeJsonString(altText);
            var json = $@"{{""media_id"": ""{mediaId}"", ""alt_text"": {{""text"": ""{escapedAltText}""}}}}";

            return this.Connection.PostJsonAsync(endpoint, json);
        }

        public OAuthEchoHandler CreateOAuthEchoHandler(Uri authServiceProvider, Uri? realm = null)
            => ((TwitterApiConnection)this.Connection).CreateOAuthEchoHandler(authServiceProvider, realm);

        public void Dispose()
            => this.ApiConnection?.Dispose();
    }
}
