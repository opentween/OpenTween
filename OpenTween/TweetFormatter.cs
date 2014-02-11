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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenTween
{
    /// <summary>
    /// ツイートの Entity 情報をもとにリンク化などを施すクラス
    /// </summary>
    public class TweetFormatter
    {
        public static string AutoLinkHtml(string text, TwitterDataModel.Entities entities)
        {
            if (entities == null)
                return AutoLinkHtml(text, Enumerable.Empty<TwitterDataModel.Entity>());

            if (entities.Urls == null)
                entities.Urls = new TwitterDataModel.Urls[0];

            if (entities.Hashtags == null)
                entities.Hashtags = new TwitterDataModel.Hashtags[0];

            if (entities.UserMentions == null)
                entities.UserMentions = new TwitterDataModel.UserMentions[0];

            if (entities.Media == null)
                entities.Media = new TwitterDataModel.Media[0];

            var entitiesQuery = entities.Urls.Cast<TwitterDataModel.Entity>()
                .Concat(entities.Hashtags)
                .Concat(entities.UserMentions)
                .Concat(entities.Media)
                .Where(x => x != null)
                .Where(x => x.Indices != null && x.Indices.Length == 2);

            return string.Concat(AutoLinkHtmlInternal(text, entitiesQuery));
        }

        public static string AutoLinkHtml(string text, IEnumerable<TwitterDataModel.Entity> entities)
        {
            if (entities == null)
                entities = Enumerable.Empty<TwitterDataModel.Entity>();

            var entitiesQuery = entities
                .Where(x => x != null)
                .Where(x => x.Indices != null && x.Indices.Length == 2);

            return string.Concat(AutoLinkHtmlInternal(text, entitiesQuery));
        }

        private static IEnumerable<string> AutoLinkHtmlInternal(string text, IEnumerable<TwitterDataModel.Entity> entities)
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
                    yield return t(e(text.Substring(curIndex, startIndex - curIndex)));

                var targetText = text.Substring(startIndex, endIndex - startIndex);

                if (entity is TwitterDataModel.Urls)
                    yield return FormatUrlEntity(targetText, (TwitterDataModel.Urls)entity);
                else if (entity is TwitterDataModel.Hashtags)
                    yield return FormatHashtagEntity(targetText, (TwitterDataModel.Hashtags)entity);
                else if (entity is TwitterDataModel.UserMentions)
                    yield return FormatMentionEntity(targetText, (TwitterDataModel.UserMentions)entity);
                else
                    yield return t(e(targetText));

                curIndex = endIndex;
            }

            if (curIndex != text.Length)
                yield return t(e(text.Substring(curIndex)));
        }

        /// <summary>
        /// エンティティの Indices をサロゲートペアを考慮して調整します
        /// </summary>
        private static IEnumerable<TwitterDataModel.Entity> FixEntityIndices(string text, IEnumerable<TwitterDataModel.Entity> entities)
        {
            var curIndex = 0;
            var indexOffset = 0; // サロゲートペアによる indices のズレを表す

            foreach (var entity in entities.OrderBy(x => x.Indices[0]))
            {
                var startIndex = entity.Indices[0];
                var endIndex = entity.Indices[1];

                for (var i = curIndex; i < (startIndex + indexOffset); i++)
                    if (i + 1 < text.Length && char.IsSurrogatePair(text[i], text[i + 1]))
                        indexOffset++;

                startIndex += indexOffset;
                curIndex = startIndex;

                for (var i = curIndex; i < (endIndex + indexOffset); i++)
                    if (i + 1 < text.Length && char.IsSurrogatePair(text[i], text[i + 1]))
                        indexOffset++;

                endIndex += indexOffset;
                curIndex = endIndex;

                entity.Indices[0] = startIndex;
                entity.Indices[1] = endIndex;

                yield return entity;
            }
        }

        private static string FormatUrlEntity(string targetText, TwitterDataModel.Urls entity)
        {
            string expandedUrl;

            // 過去に存在した壊れたエンティティの対策
            // 参照: https://dev.twitter.com/discussions/12628
            if (entity.DisplayUrl == null)
            {
                expandedUrl = MyCommon.ConvertToReadableUrl(targetText);
                return "<a href=\"" + e(entity.Url) + "\" title=\"" + e(expandedUrl) + "\">" + t(e(targetText)) + "</a>";
            }

            expandedUrl = MyCommon.ConvertToReadableUrl(entity.ExpandedUrl);
            return "<a href=\"" + e(entity.Url) + "\" title=\"" + e(expandedUrl) + "\">" + t(e(entity.DisplayUrl)) + "</a>";
        }

        private static string FormatHashtagEntity(string targetText, TwitterDataModel.Hashtags entity)
        {
            return "<a href=\"https://twitter.com/search?q=%23" + eu(entity.Text) + "\">" + t(e(targetText)) + "</a>";
        }

        private static string FormatMentionEntity(string targetText, TwitterDataModel.UserMentions entity)
        {
            return "<a href=\"https://twitter.com/" + eu(entity.ScreenName) + "\">" + t(e(targetText)) + "</a>";
        }

        // 長いのでエイリアスとして e(...), eu(...), t(...) でエスケープできるようにする
        private static Func<string, string> e = EscapeHtml;
        private static Func<string, string> eu = Uri.EscapeDataString;
        private static Func<string, string> t = FilterText;

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
                switch (c)
                {
                    case '<':
                        result.Append("&lt;");
                        break;
                    case '>':
                        result.Append("&gt;");
                        break;
                    case '&':
                        result.Append("&amp;");
                        break;
                    case '"':
                        result.Append("&quot;");
                        break;
                    case '\'':
                        result.Append("&#39;");
                        break;
                    default:
                        result.Append(c);
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// HTML の属性値ではない、通常のテキストに対するフィルタ処理
        /// </summary>
        private static string FilterText(string text)
        {
            text = text.Replace("\n", "<br>");

            return text;
        }
    }
}
