// OpenTween - Client of Twitter
// Copyright (c) 2024 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Api.DataModel;
using OpenTween.Api.GraphQL;
using OpenTween.Connection;

namespace OpenTween.Api.TwitterV2
{
    public class NotificationsMentionsRequest
    {
        public static readonly string EndpointName = "/2/notifications/mentions";

        private static readonly Uri EndpointUri = new("https://twitter.com/i/api/2/notifications/mentions.json");

        public int Count { get; set; } = 100;

        public string? Cursor { get; set; }

        public Dictionary<string, string> CreateParameters()
        {
            var param = new Dictionary<string, string>()
            {
                ["include_profile_interstitial_type"] = "1",
                ["include_blocking"] = "1",
                ["include_blocked_by"] = "1",
                ["include_followed_by"] = "1",
                ["include_want_retweets"] = "1",
                ["include_mute_edge"] = "1",
                ["include_can_dm"] = "1",
                ["include_can_media_tag"] = "1",
                ["include_ext_has_nft_avatar"] = "1",
                ["include_ext_is_blue_verified"] = "1",
                ["include_ext_verified_type"] = "1",
                ["include_ext_profile_image_shape"] = "1",
                ["skip_status"] = "1",
                ["cards_platform"] = "Web-12",
                ["include_cards"] = "1",
                ["include_ext_alt_text"] = "true",
                ["include_ext_limited_action_results"] = "true",
                ["include_quote_count"] = "true",
                ["include_reply_count"] = "1",
                ["tweet_mode"] = "extended",
                ["include_ext_views"] = "true",
                ["include_entities"] = "true",
                ["include_user_entities"] = "true",
                ["include_ext_media_color"] = "true",
                ["include_ext_media_availability"] = "true",
                ["include_ext_sensitive_media_warning"] = "true",
                ["include_ext_trusted_friends_metadata"] = "true",
                ["send_error_codes"] = "true",
                ["simple_quoted_tweet"] = "true",
                ["requestContext"] = "ptr",
                ["ext"] = "mediaStats,highlightedLabel,hasNftAvatar,voiceInfo,birdwatchPivot,superFollowMetadata,unmentionInfo,editControl",
                ["count"] = this.Count.ToString(CultureInfo.InvariantCulture),
            };

            if (!MyCommon.IsNullOrEmpty(this.Cursor))
                param["cursor"] = this.Cursor;

            return param;
        }

        public async Task<NotificationsResponse> Send(IApiConnection apiConnection)
        {
            var request = new GetRequest
            {
                RequestUri = EndpointUri,
                Query = this.CreateParameters(),
                EndpointName = EndpointName,
            };

            using var response = await apiConnection.SendAsync(request)
                .ConfigureAwait(false);

            var responseBytes = await response.ReadAsBytes()
                .ConfigureAwait(false);

            ResponseRoot parsedObjects;
            XElement rootElm;
            try
            {
                parsedObjects = MyCommon.CreateDataFromJson<ResponseRoot>(responseBytes);

                using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(
                    responseBytes,
                    XmlDictionaryReaderQuotas.Max
                );

                rootElm = XElement.Load(jsonReader);
            }
            catch (SerializationException ex)
            {
                var responseText = Encoding.UTF8.GetString(responseBytes);
                throw TwitterApiException.CreateFromException(ex, responseText);
            }
            catch (XmlException ex)
            {
                var responseText = Encoding.UTF8.GetString(responseBytes);
                throw new TwitterApiException("Invalid JSON", ex) { ResponseText = responseText };
            }

            ErrorResponse.ThrowIfError(rootElm);

            var tweetIds = rootElm.XPathSelectElements("//content/item/content/tweet/id")
                .Select(x => x.Value)
                .ToArray();

            var statuses = new List<TwitterStatus>(tweetIds.Length);
            foreach (var tweetId in tweetIds)
            {
                if (!parsedObjects.GlobalObjects.Tweets.TryGetValue(tweetId, out var tweet))
                    continue;

                var userId = tweet.UserId;
                if (!parsedObjects.GlobalObjects.Users.TryGetValue(userId, out var user))
                    continue;

                tweet.User = user;
                statuses.Add(tweet);
            }

            var tweets = TimelineTweet.ExtractTimelineTweets(rootElm);
            var cursorTop = rootElm.XPathSelectElement("//content/operation/cursor[cursorType[text()='Top']]/value")?.Value;
            var cursorBottom = rootElm.XPathSelectElement("//content/operation/cursor[cursorType[text()='Bottom']]/value")?.Value;

            return new(statuses.ToArray(), cursorTop, cursorBottom);
        }

        [DataContract]
        private record ResponseRoot(
            [property: DataMember(Name = "globalObjects")]
            ResponseGlobalObjects GlobalObjects
        );

        [DataContract]
        private record ResponseGlobalObjects(
            [property: DataMember(Name = "users")]
            Dictionary<string, TwitterUser> Users,
            [property: DataMember(Name = "tweets")]
            Dictionary<string, ResponseTweet> Tweets
        );

        [DataContract]
        private class ResponseTweet : TwitterStatus
        {
            [DataMember(Name = "user_id")]
            public string UserId { get; set; } = "";
        }

        public readonly record struct NotificationsResponse(
            TwitterStatus[] Statuses,
            string? CursorTop,
            string? CursorBottom
        );
    }
}
