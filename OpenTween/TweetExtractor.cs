// OpenTween - Client of Twitter
// Copyright (c) 2015 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenTween.Api.DataModel;

namespace OpenTween
{
    public static class TweetExtractor
    {
        /// <summary>
        /// テキストから URL を抽出して返します
        /// </summary>
        public static IEnumerable<string> ExtractUrls(string text)
        {
            return ExtractUrlEntities(text).Select(x => x.Url);
        }

        /// <summary>
        /// テキストから URL を抽出してエンティティとして返します
        /// </summary>
        public static IEnumerable<TwitterEntityUrl> ExtractUrlEntities(string text)
        {
            var urlMatches = Regex.Matches(text, Twitter.rgUrl, RegexOptions.IgnoreCase).Cast<Match>();
            foreach (var m in urlMatches)
            {
                var before = m.Groups["before"].Value;
                var url = m.Groups["url"].Value;
                var protocol = m.Groups["protocol"].Value;
                var domain = m.Groups["domain"].Value;
                var path = m.Groups["path"].Value;

                var validUrl = false;
                if (protocol.Length == 0)
                {
                    if (Regex.IsMatch(before, Twitter.url_invalid_without_protocol_preceding_chars))
                        continue;

                    string lasturl = null;

                    var last_url_invalid_match = false;
                    var domainMatches = Regex.Matches(domain, Twitter.url_valid_ascii_domain, RegexOptions.IgnoreCase).Cast<Match>();
                    foreach (var mm in domainMatches)
                    {
                        lasturl = mm.Value;
                        last_url_invalid_match = Regex.IsMatch(lasturl, Twitter.url_invalid_short_domain, RegexOptions.IgnoreCase);
                        if (!last_url_invalid_match)
                        {
                            validUrl = true;
                        }
                    }

                    if (last_url_invalid_match && path.Length != 0)
                    {
                        validUrl = true;
                    }
                }
                else
                {
                    validUrl = true;
                }

                if (validUrl)
                {
                    var startPos = m.Groups["url"].Index;
                    var endPos = startPos + m.Groups["url"].Length;

                    yield return new TwitterEntityUrl
                    {
                        Indices = new[] { startPos, endPos },
                        Url = url,
                        ExpandedUrl = url,
                        DisplayUrl = url,
                    };
                }
            }
        }

        /// <summary>
        /// テキストからメンションを抽出してエンティティとして返します
        /// </summary>
        public static IEnumerable<TwitterEntityMention> ExtractMentionEntities(string text)
        {
            // リスト
            var matchesAtList = Regex.Matches(text, @"(^|[^a-zA-Z0-9_/])([@＠][a-zA-Z0-9_]{1,20}/[a-zA-Z][a-zA-Z0-9\p{IsLatin-1Supplement}\-]{0,79})");
            foreach (var match in matchesAtList.Cast<Match>())
            {
                var groupMention = match.Groups[2];
                var startPos = groupMention.Index;
                var endPos = startPos + groupMention.Length;

                yield return new TwitterEntityMention
                {
                    Indices = new[] { startPos, endPos },
                    ScreenName = groupMention.Value.Substring(1), // 先頭の「@」は取り除く
                };
            }

            // 通常のメンション
            var matchesAtUser = Regex.Matches(text, "(^|[^a-zA-Z0-9_/])([@＠][a-zA-Z0-9_]{1,20})([^a-zA-Z0-9_/]|$)");
            foreach (var match in matchesAtUser.Cast<Match>())
            {
                var groupMention = match.Groups[2];
                var startPos = groupMention.Index;
                var endPos = startPos + groupMention.Length;

                yield return new TwitterEntityMention
                {
                    Indices = new[] { startPos, endPos },
                    ScreenName = groupMention.Value.Substring(1), // 先頭の「@」は取り除く
                };
            }
        }

        /// <summary>
        /// テキストからハッシュタグを抽出してエンティティとして返します
        /// </summary>
        public static IEnumerable<TwitterEntityHashtag> ExtractHashtagEntities(string text)
        {
            var matches = Regex.Matches(text, Twitter.HASHTAG);
            foreach (var match in matches.Cast<Match>())
            {
                var groupHashtagSharp = match.Groups[2];
                var groupHashtagText = match.Groups[3];
                var startPos = groupHashtagSharp.Index;
                var endPos = startPos + groupHashtagSharp.Length + groupHashtagText.Length;

                yield return new TwitterEntityHashtag
                {
                    Indices = new[] { startPos, endPos },
                    Text = groupHashtagText.Value,
                };
            }
        }
    }
}
