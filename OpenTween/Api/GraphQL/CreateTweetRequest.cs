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

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Api.DataModel;
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween.Api.GraphQL
{
    public class CreateTweetRequest
    {
        private static readonly Uri EndpointUri = new("https://twitter.com/i/api/graphql/tTsjMKyhajZvK4q76mpIBg/CreateTweet");

        required public string TweetText { get; set; }

        public TwitterStatusId? InReplyToTweetId { get; set; }

        public string[] ExcludeReplyUserIds { get; set; } = Array.Empty<string>();

        public string[] MediaIds { get; set; } = Array.Empty<string>();

        public string? AttachmentUrl { get; set; }

        [DataContract]
        private record RequestBody(
            [property: DataMember(Name = "variables")]
            Variables Variables,
            [property: DataMember(Name = "features")]
            Dictionary<string, bool> Features,
            [property: DataMember(Name = "queryId")]
            string QueryId
        );

        [DataContract]
        private record Variables(
            [property: DataMember(Name = "tweet_text")]
            string TweetText,
            [property: DataMember(Name = "dark_request")]
            bool DarkRequest,
            [property: DataMember(Name = "reply", EmitDefaultValue = false)]
            VariableReply? Reply,
            [property: DataMember(Name = "media", EmitDefaultValue = false)]
            VariableMedia? Media,
            [property: DataMember(Name = "attachment_url", EmitDefaultValue = false)]
            string? AttachmentUrl
        );

        [DataContract]
        private record VariableReply(
            [property: DataMember(Name = "in_reply_to_tweet_id")]
            string InReplyToTweetId,
            [property : DataMember(Name = "exclude_reply_user_ids")]
            string[] ExcludeReplyUserIds
        );

        [DataContract]
        private record VariableMedia(
            [property : DataMember(Name = "media_entities")]
            VariableMediaEntity[] MediaEntities,
            [property: DataMember(Name = "possibly_sensitive")]
            bool PossiblySensitive
        );

        [DataContract]
        private record VariableMediaEntity(
            [property: DataMember(Name = "media_id")]
            string MediaId,
            [property : DataMember(Name = "tagged_users")]
            string[] TaggedUsers
        );

        public string CreateRequestBody()
        {
#pragma warning disable SA1118
            var body = new RequestBody(
                Variables: new(
                    TweetText: this.TweetText,
                    DarkRequest: false,
                    Reply: this.InReplyToTweetId != null
                        ? new(
                            InReplyToTweetId: this.InReplyToTweetId.Id,
                            ExcludeReplyUserIds: this.ExcludeReplyUserIds
                        )
                        : null,
                    Media: this.MediaIds.Length > 0
                        ? new(
                            MediaEntities: this.MediaIds
                                .Select(x => new VariableMediaEntity(
                                    MediaId: x,
                                    TaggedUsers: Array.Empty<string>()
                                ))
                                .ToArray(),
                            PossiblySensitive: false
                        )
                        : null,
                    AttachmentUrl: this.AttachmentUrl
                ),
                Features: new()
                {
                    ["tweetypie_unmention_optimization_enabled"] = true,
                    ["responsive_web_edit_tweet_api_enabled"] = true,
                    ["graphql_is_translatable_rweb_tweet_is_translatable_enabled"] = true,
                    ["view_counts_everywhere_api_enabled"] = true,
                    ["longform_notetweets_consumption_enabled"] = true,
                    ["responsive_web_twitter_article_tweet_consumption_enabled"] = false,
                    ["tweet_awards_web_tipping_enabled"] = false,
                    ["longform_notetweets_rich_text_read_enabled"] = true,
                    ["longform_notetweets_inline_media_enabled"] = true,
                    ["responsive_web_graphql_exclude_directive_enabled"] = true,
                    ["verified_phone_label_enabled"] = false,
                    ["freedom_of_speech_not_reach_fetch_enabled"] = true,
                    ["standardized_nudges_misinfo"] = true,
                    ["tweet_with_visibility_results_prefer_gql_limited_actions_policy_enabled"] = true,
                    ["responsive_web_media_download_video_enabled"] = false,
                    ["responsive_web_graphql_skip_user_profile_image_extensions_enabled"] = false,
                    ["responsive_web_graphql_timeline_navigation_enabled"] = true,
                    ["responsive_web_enhance_cards_enabled"] = true,
                },
                QueryId: "tTsjMKyhajZvK4q76mpIBg"
            );
#pragma warning restore SA1118
            return JsonUtils.SerializeJsonByDataContract(body);
        }

        public async Task<TwitterStatus> Send(IApiConnection apiConnection)
        {
            var json = this.CreateRequestBody();
            var response = await apiConnection.PostJsonAsync(EndpointUri, json);
            var responseBytes = Encoding.UTF8.GetBytes(response);
            using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(responseBytes, XmlDictionaryReaderQuotas.Max);

            var rootElm = XElement.Load(jsonReader);
            var tweetElm = rootElm.XPathSelectElement("/data/create_tweet/tweet_results/result") ?? throw CreateParseError();

            return TimelineTweet.ParseTweet(tweetElm);
        }

        private static Exception CreateParseError()
            => throw new WebApiException($"Parse error on CreateTweet");
    }
}
