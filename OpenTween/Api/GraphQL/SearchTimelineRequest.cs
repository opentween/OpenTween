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
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Connection;

namespace OpenTween.Api.GraphQL
{
    public class SearchTimelineRequest
    {
        private static readonly Uri EndpointUri = new("https://twitter.com/i/api/graphql/lZ0GCEojmtQfiUQa5oJSEw/SearchTimeline");

        public string RawQuery { get; set; }

        public int Count { get; set; } = 20;

        public string? Cursor { get; set; }

        public SearchTimelineRequest(string rawQuery)
            => this.RawQuery = rawQuery;

        public Dictionary<string, string> CreateParameters()
        {
            return new()
            {
                ["variables"] = "{" +
                    $@"""rawQuery"":""{JsonUtils.EscapeJsonString(this.RawQuery)}""," +
                    $@"""count"":{this.Count}," +
                    $@"""product"":""Latest""" +
                    (this.Cursor != null ? $@",""cursor"":""{JsonUtils.EscapeJsonString(this.Cursor)}""" : "") +
                    "}",
                ["features"] = "{" +
                    @"""responsive_web_graphql_exclude_directive_enabled"":true," +
                    @"""verified_phone_label_enabled"":false," +
                    @"""responsive_web_home_pinned_timelines_enabled"":true," +
                    @"""creator_subscriptions_tweet_preview_api_enabled"":true," +
                    @"""responsive_web_graphql_timeline_navigation_enabled"":true," +
                    @"""responsive_web_graphql_skip_user_profile_image_extensions_enabled"":false," +
                    @"""c9s_tweet_anatomy_moderator_badge_enabled"":true," +
                    @"""tweetypie_unmention_optimization_enabled"":true," +
                    @"""responsive_web_edit_tweet_api_enabled"":true," +
                    @"""graphql_is_translatable_rweb_tweet_is_translatable_enabled"":true," +
                    @"""view_counts_everywhere_api_enabled"":true," +
                    @"""longform_notetweets_consumption_enabled"":true," +
                    @"""responsive_web_twitter_article_tweet_consumption_enabled"":false," +
                    @"""tweet_awards_web_tipping_enabled"":false," +
                    @"""freedom_of_speech_not_reach_fetch_enabled"":true," +
                    @"""standardized_nudges_misinfo"":true," +
                    @"""tweet_with_visibility_results_prefer_gql_limited_actions_policy_enabled"":true," +
                    @"""longform_notetweets_rich_text_read_enabled"":true," +
                    @"""longform_notetweets_inline_media_enabled"":true," +
                    @"""responsive_web_media_download_video_enabled"":false," +
                    @"""responsive_web_enhance_cards_enabled"":false" +
                    "}",
            };
        }

        public async Task<TimelineResponse> Send(IApiConnection apiConnection)
        {
            var param = this.CreateParameters();

            XElement rootElm;
            try
            {
                using var stream = await apiConnection.GetStreamAsync(EndpointUri, param);
                using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);
                rootElm = XElement.Load(jsonReader);
            }
            catch (IOException ex)
            {
                throw new WebApiException("IO Error", ex);
            }
            catch (NotSupportedException ex)
            {
                // NotSupportedException: Stream does not support reading. のエラーが時々報告される
                throw new WebApiException("Stream Error", ex);
            }

            ErrorResponse.ThrowIfError(rootElm);

            var tweets = TimelineTweet.ExtractTimelineTweets(rootElm);
            var cursorBottom = rootElm.XPathSelectElement("//content[__typename[text()='TimelineTimelineCursor']][cursorType[text()='Bottom']]/value")?.Value;

            return new(tweets, cursorBottom);
        }
    }
}
