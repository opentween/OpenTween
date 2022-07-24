// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;
using System.Text.RegularExpressions;
using OpenTween.Api.DataModel;
using OpenTween.Setting;

namespace OpenTween
{
    /// <summary>
    /// ツイートの Entity 情報をもとにリンク化などを施すクラス
    /// </summary>
    public static class TweetFormatter
    {
        public static string AutoLinkHtml(string text, IEnumerable<TwitterEntity>? entities, bool keepTco = false)
        {
            if (entities == null)
                entities = Enumerable.Empty<TwitterEntity>();

            var entitiesQuery = entities
                .Where(x => x != null)
                .Where(x => x.Indices != null && x.Indices.Length == 2);

            return string.Concat(AutoLinkHtmlInternal(text, entitiesQuery, keepTco));
        }

        private static IEnumerable<string> AutoLinkHtmlInternal(string text, IEnumerable<TwitterEntity> entities, bool keepTco)
        {
            var curIndex = 0;

            foreach (var entity in FixEntityIndices(text, entities))
            {
                var startIndex = entity.Indices[0];
                var endIndex = entity.Indices[1];

                if (curIndex > startIndex)
                    continue; // 区間が重複する不正なエンティティを無視する

                if (startIndex > endIndex)
                    continue; // 区間が不正なエンティティを無視する

                if (startIndex > text.Length || endIndex > text.Length)
                    continue; // 区間が文字列長を越えている不正なエンティティを無視する

                if (curIndex != startIndex)
                    yield return T(E(text.Substring(curIndex, startIndex - curIndex)));

                var targetText = text.Substring(startIndex, endIndex - startIndex);

                yield return entity switch
                {
                    TwitterEntityUrl urlEntity => FormatUrlEntity(targetText, urlEntity, keepTco),
                    TwitterEntityHashtag hashtagEntity => FormatHashtagEntity(targetText, hashtagEntity),
                    TwitterEntityMention mentionEntity => FormatMentionEntity(targetText, mentionEntity),
                    TwitterEntityEmoji emojiEntity => FormatEmojiEntity(targetText, emojiEntity),
                    _ => T(E(targetText)),
                };

                curIndex = endIndex;
            }

            if (curIndex != text.Length)
                yield return T(E(text.Substring(curIndex)));
        }

        /// <summary>
        /// エンティティの Indices をサロゲートペアを考慮して調整します
        /// </summary>
        private static IEnumerable<TwitterEntity> FixEntityIndices(string text, IEnumerable<TwitterEntity> entities)
        {
            var curIndex = 0;
            var indexOffset = 0; // サロゲートペアによる indices のズレを表す

            foreach (var entity in entities.OrderBy(x => x.Indices[0]))
            {
                var startIndex = entity.Indices[0];
                var endIndex = entity.Indices[1];

                for (var i = curIndex; i < (startIndex + indexOffset); i++)
                {
                    if (i + 1 < text.Length && char.IsSurrogatePair(text[i], text[i + 1]))
                        indexOffset++;
                }

                startIndex += indexOffset;
                curIndex = startIndex;

                for (var i = curIndex; i < (endIndex + indexOffset); i++)
                {
                    if (i + 1 < text.Length && char.IsSurrogatePair(text[i], text[i + 1]))
                        indexOffset++;
                }

                endIndex += indexOffset;
                curIndex = endIndex;

                entity.Indices[0] = startIndex;
                entity.Indices[1] = endIndex;

                yield return entity;
            }
        }

        private static string FormatUrlEntity(string targetText, TwitterEntityUrl entity, bool keepTco)
        {
            string expandedUrl;

            // 過去に存在した壊れたエンティティの対策
            // 参照: https://dev.twitter.com/discussions/12628
            if (entity.DisplayUrl == null)
            {
                expandedUrl = MyCommon.ConvertToReadableUrl(targetText);
                return "<a href=\"" + E(entity.Url) + "\" title=\"" + E(expandedUrl) + "\">" + T(E(targetText)) + "</a>";
            }

            var linkUrl = entity.Url;

            expandedUrl = keepTco ? linkUrl : MyCommon.ConvertToReadableUrl(entity.ExpandedUrl);

            var mediaEntity = entity as TwitterEntityMedia;

            var titleText = mediaEntity?.AltText ?? expandedUrl;

            // twitter.com へのリンクは t.co を経由せずに直接リンクする (但し pic.twitter.com はそのまま)
            if (mediaEntity == null)
            {
                if (entity.ExpandedUrl.StartsWith("https://twitter.com/", StringComparison.Ordinal) ||
                    entity.ExpandedUrl.StartsWith("http://twitter.com/", StringComparison.Ordinal))
                {
                    linkUrl = entity.ExpandedUrl;
                }
            }

            return "<a href=\"" + E(linkUrl) + "\" title=\"" + E(titleText) + "\">" + T(E(entity.DisplayUrl)) + "</a>";
        }

        private static string FormatHashtagEntity(string targetText, TwitterEntityHashtag entity)
            => "<a class=\"hashtag\" href=\"https://twitter.com/search?q=%23" + EU(entity.Text) + "\">" + T(E(targetText)) + "</a>";

        private static string FormatMentionEntity(string targetText, TwitterEntityMention entity)
            => "<a class=\"mention\" href=\"https://twitter.com/" + EU(entity.ScreenName) + "\">" + T(E(targetText)) + "</a>";

        private static string FormatEmojiEntity(string targetText, TwitterEntityEmoji entity)
        {
            if (!SettingManager.Instance.Local.UseTwemoji)
                return T(E(targetText));

            if (MyCommon.IsNullOrEmpty(entity.Url))
                return "";

            return "<img class=\"emoji\" src=\"" + E(entity.Url) + "\" alt=\"" + E(entity.Text) + "\" />";
        }

        // 長いのでエイリアスとして e(...), eu(...), t(...) でエスケープできるようにする
        private static readonly Func<string, string> E = EscapeHtml;
        private static readonly Func<string, string> EU = Uri.EscapeDataString;
        private static readonly Func<string, string> T = FilterText;

        private static string EscapeHtml(string text)
        {
            // Twitter API は "<" ">" "&" だけ中途半端にエスケープした状態のテキストを返すため、
            // これらの文字だけ一旦エスケープを解除する
            text = text.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");

            var result = new StringBuilder(100);
            foreach (var c in text)
            {
                // 「<」「>」「&」「"」「'」についてエスケープ処理を施す
                // 参照: http://d.hatena.ne.jp/ockeghem/20070510/1178813849
                result.Append(c switch
                {
                    '<' => "&lt;",
                    '>' => "&gt;",
                    '&' => "&amp;",
                    '"' => "&quot;",
                    '\'' => "&#39;",
                    _ => c,
                });
            }

            return result.ToString();
        }

        /// <summary>
        /// HTML の属性値ではない、通常のテキストに対するフィルタ処理
        /// </summary>
        private static string FilterText(string text)
        {
            text = text.Replace("\n", "<br>");
            text = Regex.Replace(text, "  ", " &nbsp;");

            return text;
        }
    }
}
