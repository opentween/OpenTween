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
        public string CurrentScreenName { get; private set; }

        public IApiConnection Connection => this.apiConnection;

        internal IApiConnection apiConnection;

        public void Initialize(string accessToken, string accessSecret, long userId, string screenName)
        {
            var newInstance = new TwitterApiConnection(accessToken, accessSecret);
            var oldInstance = Interlocked.Exchange(ref this.apiConnection, newInstance);
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
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

            return this.apiConnection.GetAsync<TwitterStatus[]>(endpoint, param, "/statuses/home_timeline");
        }

        public Task<TwitterStatus[]> StatusesMentionsTimeline(int? count = null, long? maxId = null, long? sinceId = null)
        {
            var endpoint = new Uri("statuses/mentions_timeline.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

            return this.apiConnection.GetAsync<TwitterStatus[]>(endpoint, param, "/statuses/mentions_timeline");
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
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

            return this.apiConnection.GetAsync<TwitterStatus[]>(endpoint, param, "/statuses/user_timeline");
        }

        public Task<TwitterStatus> StatusesShow(long statusId)
        {
            var endpoint = new Uri("statuses/show.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.ToString(),
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            return this.apiConnection.GetAsync<TwitterStatus>(endpoint, param, "/statuses/show/:id");
        }

        public Task<LazyJson<TwitterStatus>> StatusesUpdate(string status, long? replyToId, IReadOnlyList<long> mediaIds)
        {
            var endpoint = new Uri("statuses/update.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["status"] = status,
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            if (replyToId != null)
                param["in_reply_to_status_id"] = replyToId.ToString();
            if (mediaIds != null && mediaIds.Count > 0)
                param.Add("media_ids", string.Join(",", mediaIds));

            return this.apiConnection.PostLazyAsync<TwitterStatus>(endpoint, param);
        }

        public Task<LazyJson<TwitterStatus>> StatusesDestroy(long statusId)
        {
            var endpoint = new Uri("statuses/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.ToString(),
            };

            return this.apiConnection.PostLazyAsync<TwitterStatus>(endpoint, param);
        }

        public Task<LazyJson<TwitterStatus>> StatusesRetweet(long statusId)
        {
            var endpoint = new Uri("statuses/retweet.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.ToString(),
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            return this.apiConnection.PostLazyAsync<TwitterStatus>(endpoint, param);
        }

        public Task<TwitterSearchResult> SearchTweets(string query, string lang = null, int? count = null, long? maxId = null, long? sinceId = null)
        {
            var endpoint = new Uri("search/tweets.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["q"] = query,
                ["result_type"] = "recent",
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            if (lang != null)
                param["lang"] = lang;
            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

            return this.apiConnection.GetAsync<TwitterSearchResult>(endpoint, param, "/search/tweets");
        }

        public Task<TwitterLists> ListsOwnerships(string screenName, long? cursor = null)
        {
            var endpoint = new Uri("lists/ownerships.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            if (cursor != null)
                param["cursor"] = cursor.ToString();

            return this.apiConnection.GetAsync<TwitterLists>(endpoint, param, "/lists/ownerships");
        }

        public Task<TwitterLists> ListsSubscriptions(string screenName, long? cursor = null)
        {
            var endpoint = new Uri("lists/subscriptions.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            if (cursor != null)
                param["cursor"] = cursor.ToString();

            return this.apiConnection.GetAsync<TwitterLists>(endpoint, param, "/lists/subscriptions");
        }

        public Task<LazyJson<TwitterList>> ListsCreate(string name, string description = null, bool? @private = null)
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

            return this.apiConnection.PostLazyAsync<TwitterList>(endpoint, param);
        }

        public Task<LazyJson<TwitterList>> ListsUpdate(long listId, string name = null, string description = null, bool? @private = null)
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

            return this.apiConnection.PostLazyAsync<TwitterList>(endpoint, param);
        }

        public Task<LazyJson<TwitterList>> ListsDestroy(long listId)
        {
            var endpoint = new Uri("lists/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["list_id"] = listId.ToString(),
            };

            return this.apiConnection.PostLazyAsync<TwitterList>(endpoint, param);
        }

        public Task<TwitterStatus[]> ListsStatuses(long listId, int? count = null, long? maxId = null, long? sinceId = null, bool? includeRTs = null)
        {
            var endpoint = new Uri("lists/statuses.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["list_id"] = listId.ToString(),
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();
            if (includeRTs != null)
                param["include_rts"] = includeRTs.Value ? "true" : "false";

            return this.apiConnection.GetAsync<TwitterStatus[]>(endpoint, param, "/lists/statuses");
        }

        public Task<TwitterUsers> ListsMembers(long listId, long? cursor = null)
        {
            var endpoint = new Uri("lists/members.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["list_id"] = listId.ToString(),
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            if (cursor != null)
                param["cursor"] = cursor.ToString();

            return this.apiConnection.GetAsync<TwitterUsers>(endpoint, param, "/lists/members");
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
            };

            return this.apiConnection.GetAsync<TwitterUser>(endpoint, param, "/lists/members/show");
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
            };

            return this.apiConnection.PostLazyAsync<TwitterUser>(endpoint, param);
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
            };

            return this.apiConnection.PostLazyAsync<TwitterUser>(endpoint, param);
        }

        public Task<TwitterDirectMessage[]> DirectMessagesRecv(int? count = null, long? maxId = null, long? sinceId = null)
        {
            var endpoint = new Uri("direct_messages.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["full_text"] = "true",
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

            return this.apiConnection.GetAsync<TwitterDirectMessage[]>(endpoint, param, "/direct_messages");
        }

        public Task<TwitterDirectMessage[]> DirectMessagesSent(int? count = null, long? maxId = null, long? sinceId = null)
        {
            var endpoint = new Uri("direct_messages/sent.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["full_text"] = "true",
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

            return this.apiConnection.GetAsync<TwitterDirectMessage[]>(endpoint, param, "/direct_messages/sent");
        }

        public Task<LazyJson<TwitterDirectMessage>> DirectMessagesNew(string status, string sendTo)
        {
            var endpoint = new Uri("direct_messages/new.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["text"] = status,
                ["screen_name"] = sendTo,
            };

            return this.apiConnection.PostLazyAsync<TwitterDirectMessage>(endpoint, param);
        }

        public Task<LazyJson<TwitterDirectMessage>> DirectMessagesDestroy(long statusId)
        {
            var endpoint = new Uri("direct_messages/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.ToString(),
            };

            return this.apiConnection.PostLazyAsync<TwitterDirectMessage>(endpoint, param);
        }

        public Task<TwitterUser> UsersShow(string screenName)
        {
            var endpoint = new Uri("users/show.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            return this.apiConnection.GetAsync<TwitterUser>(endpoint, param, "/users/show/:id");
        }

        public Task<LazyJson<TwitterUser>> UsersReportSpam(string screenName)
        {
            var endpoint = new Uri("users/report_spam.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            return this.apiConnection.PostLazyAsync<TwitterUser>(endpoint, param);
        }

        public Task<TwitterStatus[]> FavoritesList(int? count = null, long? maxId = null, long? sinceId = null)
        {
            var endpoint = new Uri("favorites/list.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            if (count != null)
                param["count"] = count.ToString();
            if (maxId != null)
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();

            return this.apiConnection.GetAsync<TwitterStatus[]>(endpoint, param, "/favorites/list");
        }

        public Task<LazyJson<TwitterStatus>> FavoritesCreate(long statusId)
        {
            var endpoint = new Uri("favorites/create.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.ToString(),
            };

            return this.apiConnection.PostLazyAsync<TwitterStatus>(endpoint, param);
        }

        public Task<LazyJson<TwitterStatus>> FavoritesDestroy(long statusId)
        {
            var endpoint = new Uri("favorites/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["id"] = statusId.ToString(),
            };

            return this.apiConnection.PostLazyAsync<TwitterStatus>(endpoint, param);
        }

        public Task<TwitterFriendship> FriendshipsShow(string sourceScreenName, string targetScreenName)
        {
            var endpoint = new Uri("friendships/show.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["source_screen_name"] = sourceScreenName,
                ["target_screen_name"] = targetScreenName,
            };

            return this.apiConnection.GetAsync<TwitterFriendship>(endpoint, param, "/friendships/show");
        }

        public Task<LazyJson<TwitterFriendship>> FriendshipsCreate(string screenName)
        {
            var endpoint = new Uri("friendships/create.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            return this.apiConnection.PostLazyAsync<TwitterFriendship>(endpoint, param);
        }

        public Task<LazyJson<TwitterFriendship>> FriendshipsDestroy(string screenName)
        {
            var endpoint = new Uri("friendships/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            return this.apiConnection.PostLazyAsync<TwitterFriendship>(endpoint, param);
        }

        public Task<long[]> NoRetweetIds(long? cursor = null)
        {
            var endpoint = new Uri("friendships/no_retweets/ids.json", UriKind.Relative);

            return this.apiConnection.GetAsync<long[]>(endpoint, null, "/friendships/no_retweets/ids");
        }

        public Task<TwitterIds> FollowersIds(long? cursor = null)
        {
            var endpoint = new Uri("followers/ids.json", UriKind.Relative);
            var param = new Dictionary<string, string>();

            if (cursor != null)
                param["cursor"] = cursor.ToString();

            return this.apiConnection.GetAsync<TwitterIds>(endpoint, param, "/followers/ids");
        }

        public Task<TwitterIds> MutesUsersIds(long? cursor = null)
        {
            var endpoint = new Uri("mutes/users/ids.json", UriKind.Relative);
            var param = new Dictionary<string, string>();

            if (cursor != null)
                param["cursor"] = cursor.ToString();

            return this.apiConnection.GetAsync<TwitterIds>(endpoint, param, "/mutes/users/ids");
        }

        public Task<TwitterIds> BlocksIds(long? cursor = null)
        {
            var endpoint = new Uri("blocks/ids.json", UriKind.Relative);
            var param = new Dictionary<string, string>();

            if (cursor != null)
                param["cursor"] = cursor.ToString();

            return this.apiConnection.GetAsync<TwitterIds>(endpoint, param, "/blocks/ids");
        }

        public Task<LazyJson<TwitterUser>> BlocksCreate(string screenName)
        {
            var endpoint = new Uri("blocks/create.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            return this.apiConnection.PostLazyAsync<TwitterUser>(endpoint, param);
        }

        public Task<LazyJson<TwitterUser>> BlocksDestroy(string screenName)
        {
            var endpoint = new Uri("blocks/destroy.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["screen_name"] = screenName,
            };

            return this.apiConnection.PostLazyAsync<TwitterUser>(endpoint, param);
        }

        public async Task<TwitterUser> AccountVerifyCredentials()
        {
            var endpoint = new Uri("account/verify_credentials.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };

            var user = await this.apiConnection.GetAsync<TwitterUser>(endpoint, param, "/account/verify_credentials")
                .ConfigureAwait(false);

            this.CurrentUserId = user.Id;
            this.CurrentScreenName = user.ScreenName;

            return user;
        }

        public Task<LazyJson<TwitterUser>> AccountUpdateProfile(string name, string url, string location, string description)
        {
            var endpoint = new Uri("account/update_profile.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
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

            return this.apiConnection.PostLazyAsync<TwitterUser>(endpoint, param);
        }

        public Task<LazyJson<TwitterUser>> AccountUpdateProfileImage(IMediaItem image)
        {
            var endpoint = new Uri("account/update_profile_image.json", UriKind.Relative);
            var param = new Dictionary<string, string>
            {
                ["include_entities"] = "true",
                ["include_ext_alt_text"] = "true",
            };
            var paramMedia = new Dictionary<string, IMediaItem>
            {
                ["image"] = image,
            };

            return this.apiConnection.PostLazyAsync<TwitterUser>(endpoint, param, paramMedia);
        }

        public Task<TwitterRateLimits> ApplicationRateLimitStatus()
        {
            var endpoint = new Uri("application/rate_limit_status.json", UriKind.Relative);

            return this.apiConnection.GetAsync<TwitterRateLimits>(endpoint, null, "/application/rate_limit_status");
        }

        public Task<TwitterConfiguration> Configuration()
        {
            var endpoint = new Uri("help/configuration.json", UriKind.Relative);

            return this.apiConnection.GetAsync<TwitterConfiguration>(endpoint, null, "/help/configuration");
        }

        public Task<LazyJson<TwitterUploadMediaResult>> MediaUpload(IMediaItem media)
        {
            var endpoint = new Uri("https://upload.twitter.com/1.1/media/upload.json");
            var paramMedia = new Dictionary<string, IMediaItem>
            {
                ["media"] = media,
            };

            return this.apiConnection.PostLazyAsync<TwitterUploadMediaResult>(endpoint, null, paramMedia);
        }

        public Task<Stream> UserStreams(string replies = null, string track = null)
        {
            var endpoint = new Uri("https://userstream.twitter.com/1.1/user.json");
            var param = new Dictionary<string, string>();

            if (replies != null)
                param["replies"] = replies;
            if (track != null)
                param["track"] = track;

            return this.apiConnection.GetStreamAsync(endpoint, param);
        }

        public OAuthEchoHandler CreateOAuthEchoHandler(Uri authServiceProvider, Uri realm = null)
        {
            return ((TwitterApiConnection)this.apiConnection).CreateOAuthEchoHandler(authServiceProvider, realm);
        }

        public void Dispose()
        {
            this.apiConnection?.Dispose();
        }
    }
}
