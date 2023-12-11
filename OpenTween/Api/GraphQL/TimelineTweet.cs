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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Api.DataModel;

namespace OpenTween.Api.GraphQL
{
    public class TimelineTweet
    {
        public const string TypeName = nameof(TimelineTweet);

        public XElement Element { get; }

        public bool IsTombstone
            => this.tombstoneElm != null;

        private readonly XElement? tombstoneElm;

        public TimelineTweet(XElement element)
        {
            var typeName = element.Element("itemType")?.Value;
            if (typeName != TypeName)
                throw new ArgumentException($"Invalid itemType: {typeName}", nameof(element));

            this.Element = element;
            this.tombstoneElm = this.TryGetTombstoneElm();
        }

        private XElement? TryGetTombstoneElm()
            => this.Element.XPathSelectElement("tweet_results/result[__typename[text()='TweetTombstone']]");

        public TwitterStatus ToTwitterStatus()
        {
            this.ThrowIfTweetIsTombstone();

            try
            {
                var resultElm = this.Element.Element("tweet_results")?.Element("result") ?? throw CreateParseError();
                var status = TimelineTweet.ParseTweetUnion(resultElm);

                if (this.Element.Element("promotedMetadata") != null)
                    status.IsPromoted = true;

                return status;
            }
            catch (WebApiException ex)
            {
                ex.ResponseText = JsonUtils.JsonXmlToString(this.Element);
                MyCommon.TraceOut(ex);
                throw;
            }
        }

        public void ThrowIfTweetIsTombstone()
        {
            if (this.tombstoneElm == null)
                return;

            var tombstoneText = this.tombstoneElm.XPathSelectElement("tombstone/text/text")?.Value;
            var message = tombstoneText ?? "Tweet is not available";
            var json = JsonUtils.JsonXmlToString(this.Element);

            throw new WebApiException(message, json);
        }

        public static TwitterStatus ParseTweetUnion(XElement tweetUnionElm)
        {
            var tweetElm = GetTweetTypeName(tweetUnionElm) switch
            {
                "Tweet" => tweetUnionElm,
                "TweetWithVisibilityResults" => tweetUnionElm.Element("tweet") ?? throw CreateParseError(),
                _ => throw CreateParseError(),
            };

            return TimelineTweet.ParseTweet(tweetElm);
        }

        public static string GetTweetTypeName(XElement tweetUnionElm)
            => tweetUnionElm.Element("__typename")?.Value ?? throw CreateParseError();

        public static TwitterStatus ParseTweet(XElement tweetElm)
        {
            var tweetLegacyElm = tweetElm.Element("legacy") ?? throw CreateParseError();
            var userElm = tweetElm.Element("core")?.Element("user_results")?.Element("result") ?? throw CreateParseError();
            var retweetedTweetElm = tweetLegacyElm.Element("retweeted_status_result")?.Element("result");
            var user = new TwitterGraphqlUser(userElm);
            var quotedTweetElm = tweetElm.Element("quoted_status_result")?.Element("result") ?? null;
            var quotedStatusPermalink = tweetLegacyElm.Element("quoted_status_permalink") ?? null;
            var isQuotedTweetTombstone = quotedTweetElm != null && GetTweetTypeName(quotedTweetElm) == "TweetTombstone";

            static string GetText(XElement elm, string name)
                => elm.Element(name)?.Value ?? throw CreateParseError();

            static string? GetTextOrNull(XElement elm, string name)
                => elm.Element(name)?.Value;

            return new()
            {
                IdStr = GetText(tweetElm, "rest_id"),
                Source = GetText(tweetElm, "source"),
                CreatedAt = GetText(tweetLegacyElm, "created_at"),
                FullText = GetText(tweetLegacyElm, "full_text"),
                InReplyToScreenName = GetTextOrNull(tweetLegacyElm, "in_reply_to_screen_name"),
                InReplyToStatusIdStr = GetTextOrNull(tweetLegacyElm, "in_reply_to_status_id_str"),
                InReplyToUserId = GetTextOrNull(tweetLegacyElm, "in_reply_to_user_id_str") is string userId ? long.Parse(userId) : null,
                Favorited = GetTextOrNull(tweetLegacyElm, "favorited") is string favorited ? favorited == "true" : null,
                Entities = new()
                {
                    UserMentions = tweetLegacyElm.XPathSelectElements("entities/user_mentions/item")
                        .Select(x => new TwitterEntityMention()
                        {
                            Indices = x.XPathSelectElements("indices/item").Select(x => int.Parse(x.Value)).ToArray(),
                            ScreenName = GetText(x, "screen_name"),
                        })
                        .ToArray(),
                    Urls = tweetLegacyElm.XPathSelectElements("entities/urls/item")
                        .Select(x => new TwitterEntityUrl()
                        {
                            Indices = x.XPathSelectElements("indices/item").Select(x => int.Parse(x.Value)).ToArray(),
                            DisplayUrl = GetTextOrNull(x, "display_url"),
                            ExpandedUrl = GetTextOrNull(x, "expanded_url"),
                            Url = GetText(x, "url"),
                        })
                        .ToArray(),
                    Hashtags = tweetLegacyElm.XPathSelectElements("entities/hashtags/item")
                        .Select(x => new TwitterEntityHashtag()
                        {
                            Indices = x.XPathSelectElements("indices/item").Select(x => int.Parse(x.Value)).ToArray(),
                            Text = GetText(x, "text"),
                        })
                        .ToArray(),
                },
                ExtendedEntities = new()
                {
                    Media = tweetLegacyElm.XPathSelectElements("extended_entities/media/item")
                        .Select(x => new TwitterEntityMedia()
                        {
                            Indices = x.XPathSelectElements("indices/item").Select(x => int.Parse(x.Value)).ToArray(),
                            DisplayUrl = GetText(x, "display_url"),
                            ExpandedUrl = GetText(x, "expanded_url"),
                            Url = GetText(x, "url"),
                            MediaUrlHttps = GetText(x, "media_url_https"),
                            Type = GetText(x, "type"),
                            AltText = GetTextOrNull(x, "ext_alt_text"),
                        })
                        .ToArray(),
                },
                User = user.ToTwitterUser(),
                RetweetedStatus = retweetedTweetElm != null ? TimelineTweet.ParseTweetUnion(retweetedTweetElm) : null,
                IsQuoteStatus = GetTextOrNull(tweetLegacyElm, "is_quote_status") == "true",
                QuotedStatus = quotedTweetElm != null && !isQuotedTweetTombstone ? TimelineTweet.ParseTweetUnion(quotedTweetElm) : null,
                QuotedStatusIdStr = GetTextOrNull(tweetLegacyElm, "quoted_status_id_str"),
                QuotedStatusPermalink = quotedStatusPermalink == null ? null : new()
                {
                    Url = GetText(quotedStatusPermalink, "url"),
                    Expanded = GetText(quotedStatusPermalink, "expanded"),
                    Display = GetText(quotedStatusPermalink, "display"),
                },
            };
        }

        private static Exception CreateParseError()
            => throw new WebApiException("Parse error on TimelineTweet");

        public static TimelineTweet[] ExtractTimelineTweets(XElement element)
        {
            return element.XPathSelectElements($"//itemContent[itemType[text()='{TypeName}']][tweetDisplayType[text()='Tweet' or text()='SelfThread']]")
                .Select(x => new TimelineTweet(x))
                .ToArray();
        }
    }
}
