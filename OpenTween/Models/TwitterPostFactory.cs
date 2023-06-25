// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net;
using System.Text.RegularExpressions;
using OpenTween.Api.DataModel;

namespace OpenTween.Models
{
    public class TwitterPostFactory
    {
        private static readonly Uri SourceUriBase = new("https://twitter.com/");

        private readonly TabInformations tabinfo;
        private readonly HashSet<string> receivedHashTags = new();

        public TwitterPostFactory(TabInformations tabinfo)
            => this.tabinfo = tabinfo;

        public string[] GetReceivedHashtags()
        {
            lock (this.receivedHashTags)
            {
                var hashtags = this.receivedHashTags.ToArray();
                this.receivedHashTags.Clear();
                return hashtags;
            }
        }

        public PostClass CreateFromStatus(
            TwitterStatus status,
            long selfUserId,
            ISet<long> followerIds,
            bool favTweet = false
        )
        {
            var statusUser = status.User ?? TwitterUser.CreateUnknownUser();

            // リツイートでない場合は null
            var retweetedStatus = (TwitterStatus?)null;
            var retweeterUser = (TwitterUser?)null;
            if (status.RetweetedStatus != null)
            {
                // リツイート元のツイート
                retweetedStatus = status.RetweetedStatus;
                // リツイートを行ったユーザー
                retweeterUser = statusUser;
            }

            // リツイートであるか否かに関わらず常にオリジナルのツイート及びユーザーを指す
            var originalStatus = retweetedStatus ?? status;
            var originalStatusUser = originalStatus.User ?? TwitterUser.CreateUnknownUser();

            bool isFav = favTweet;
            if (isFav == false)
            {
                // 幻覚fav対策 (8a5717dd のコミット参照)
                var favTab = this.tabinfo.FavoriteTab;
                isFav = favTab.Contains(originalStatus.Id);
            }

            var geo = (PostClass.StatusGeo?)null;
            if (originalStatus.Coordinates != null)
                geo = new(originalStatus.Coordinates.Coordinates[0], originalStatus.Coordinates.Coordinates[1]);

            var entities = originalStatus.MergedEntities;
            var quotedStatusLink = originalStatus.QuotedStatusPermalink;

            if (quotedStatusLink != null && entities.Urls != null && entities.Urls.Any(x => x.ExpandedUrl == quotedStatusLink.Expanded))
                quotedStatusLink = null; // 移行期は entities.urls と quoted_status_permalink の両方に含まれる場合がある

            // HTMLに整形
            var text = CreateHtmlAnchor(originalStatus.FullText, entities, quotedStatusLink);

            var textFromApi = this.ReplaceTextFromApi(originalStatus.FullText, entities, quotedStatusLink);
            textFromApi = WebUtility.HtmlDecode(textFromApi);
            textFromApi = textFromApi.Replace("<3", "\u2661");

            var accessibleText = CreateAccessibleText(originalStatus.FullText, entities, originalStatus.QuotedStatus, quotedStatusLink);
            accessibleText = WebUtility.HtmlDecode(accessibleText);
            accessibleText = accessibleText.Replace("<3", "\u2661");

            var (replyToList, media) = this.ExtractEntities(entities);

            var quoteStatusIds = GetQuoteTweetStatusIds(entities, quotedStatusLink)
                .Where(x => x != status.Id && x != originalStatus.Id)
                .Distinct().ToArray();

            var expandedUrls = entities.OfType<TwitterEntityUrl>()
                .Select(x => new PostClass.ExpandedUrlInfo(x.Url, x.ExpandedUrl))
                .ToArray();

            // メモリ使用量削減 (同一のテキストであれば同一の string インスタンスを参照させる)
            if (text == textFromApi)
                text = textFromApi;

            if (accessibleText == textFromApi)
                accessibleText = textFromApi;

            // 他の発言と重複しやすい (共通化できる) 文字列は string.Intern を通す
            var screenName = string.Intern(originalStatusUser.ScreenName);
            var nickname = string.Intern(originalStatusUser.Name);
            var imageUrl = string.Intern(originalStatusUser.ProfileImageUrlHttps);

            // Source整形
            var (sourceText, sourceUri) = ParseSource(originalStatus.Source);

            var isOwl = false;
            if (followerIds.Count > 0)
                isOwl = !followerIds.Contains(originalStatusUser.Id);

            return new()
            {
                // status から生成
                StatusId = status.Id,
                IsMe = statusUser.Id == selfUserId,

                // originalStatus から生成
                CreatedAt = MyCommon.DateTimeParse(originalStatus.CreatedAt),
                Text = text,
                TextFromApi = textFromApi,
                AccessibleText = accessibleText,
                QuoteStatusIds = quoteStatusIds,
                ExpandedUrls = expandedUrls,
                ReplyToList = replyToList,
                Media = media,
                PostGeo = geo,
                Source = string.Intern(sourceText),
                SourceUri = sourceUri,
                IsFav = isFav,
                IsReply = retweetedStatus != null && replyToList.Any(x => x.UserId == selfUserId),
                InReplyToStatusId = originalStatus.InReplyToStatusId,
                InReplyToUser = originalStatus.InReplyToScreenName,
                InReplyToUserId = originalStatus.InReplyToUserId,

                // originalStatusUser から生成
                UserId = originalStatusUser.Id,
                ScreenName = screenName,
                Nickname = nickname,
                ImageUrl = imageUrl,
                IsProtect = originalStatusUser.Protected,
                IsOwl = isOwl,

                // retweetedStatus から生成
                RetweetedId = retweetedStatus?.Id,

                // retweeterUser から生成
                RetweetedBy = retweeterUser != null ? string.Intern(retweeterUser.ScreenName) : null,
                RetweetedByUserId = retweeterUser?.Id,
            };
        }

        public PostClass CreateFromDirectMessageEvent(
            TwitterMessageEvent eventItem,
            IReadOnlyDictionary<string, TwitterUser> users,
            IReadOnlyDictionary<string, TwitterMessageEventList.App> apps,
            long selfUserId
        )
        {
            var timestamp = long.Parse(eventItem.CreatedTimestamp);
            var createdAt = DateTimeUtc.UnixEpoch + TimeSpan.FromTicks(timestamp * TimeSpan.TicksPerMillisecond);

            // 本文
            var rawText = eventItem.MessageCreate.MessageData.Text;

            var entities = eventItem.MessageCreate.MessageData.Entities;
            var mediaEntity = eventItem.MessageCreate.MessageData.Attachment?.Media;

            if (mediaEntity != null)
                entities.Media = new[] { mediaEntity };

            // HTMLに整形
            var text = CreateHtmlAnchor(rawText, entities, quotedStatusLink: null);

            var textFromApi = this.ReplaceTextFromApi(rawText, entities, quotedStatusLink: null);
            textFromApi = WebUtility.HtmlDecode(textFromApi);
            textFromApi = textFromApi.Replace("<3", "\u2661");

            var accessibleText = CreateAccessibleText(rawText, entities, quotedStatus: null, quotedStatusLink: null);
            accessibleText = WebUtility.HtmlDecode(accessibleText);
            accessibleText = accessibleText.Replace("<3", "\u2661");

            var (replyToList, media) = this.ExtractEntities(entities);

            var quoteStatusIds = GetQuoteTweetStatusIds(entities, quotedStatusLink: null)
                .Distinct().ToArray();

            var expandedUrls = entities.OfType<TwitterEntityUrl>()
                .Select(x => new PostClass.ExpandedUrlInfo(x.Url, x.ExpandedUrl))
                .ToArray();

            // 以下、ユーザー情報
            var senderIsMe = eventItem.MessageCreate.SenderId == selfUserId.ToString(CultureInfo.InvariantCulture);
            var displayUserId = senderIsMe
                ? eventItem.MessageCreate.Target.RecipientId
                : eventItem.MessageCreate.SenderId;

            if (!users.TryGetValue(displayUserId, out var displayUser))
                displayUser = TwitterUser.CreateUnknownUser();

            // メモリ使用量削減 (同一のテキストであれば同一の string インスタンスを参照させる)
            if (text == textFromApi)
                text = textFromApi;
            if (accessibleText == textFromApi)
                accessibleText = textFromApi;

            // 他の発言と重複しやすい (共通化できる) 文字列は string.Intern を通す
            var screenName = string.Intern(displayUser.ScreenName);
            var nickname = string.Intern(displayUser.Name);
            var imageUrl = string.Intern(displayUser.ProfileImageUrlHttps);

            var source = (string?)null;
            var sourceUri = (Uri?)null;
            var appId = eventItem.MessageCreate.SourceAppId;
            if (appId != null && apps.TryGetValue(appId, out var app))
            {
                source = string.Intern(app.Name);

                try
                {
                    sourceUri = new Uri(SourceUriBase, app.Url);
                }
                catch (UriFormatException)
                {
                }
            }

            return new()
            {
                StatusId = long.Parse(eventItem.Id),
                IsDm = true,
                CreatedAt = createdAt,
                Text = text,
                TextFromApi = textFromApi,
                AccessibleText = accessibleText,
                QuoteStatusIds = quoteStatusIds,
                ExpandedUrls = expandedUrls,
                ReplyToList = replyToList,
                Media = media,
                Source = source ?? "",
                SourceUri = sourceUri,

                // displayUser から生成
                UserId = displayUser.Id,
                ScreenName = screenName,
                Nickname = nickname,
                ImageUrl = imageUrl,
                IsProtect = displayUser.Protected,
                IsMe = senderIsMe,
                IsOwl = !senderIsMe,
            };
        }

        private string ReplaceTextFromApi(string text, TwitterEntities? entities, TwitterQuotedStatusPermalink? quotedStatusLink)
        {
            if (entities?.Urls != null)
            {
                foreach (var m in entities.Urls)
                {
                    if (!MyCommon.IsNullOrEmpty(m.DisplayUrl))
                        text = text.Replace(m.Url, m.DisplayUrl);
                }
            }

            if (entities?.Media != null)
            {
                foreach (var m in entities.Media)
                {
                    if (!MyCommon.IsNullOrEmpty(m.DisplayUrl))
                        text = text.Replace(m.Url, m.DisplayUrl);
                }
            }

            if (quotedStatusLink != null)
                text += " " + quotedStatusLink.Display;

            return text;
        }

        private (List<(long UserId, string ScreenName)> ReplyToList, List<MediaInfo> Media) ExtractEntities(TwitterEntities? entities)
        {
            var atList = new List<(long UserId, string ScreenName)>();
            var media = new List<MediaInfo>();

            if (entities == null)
                return (atList, media);

            if (entities.Hashtags != null)
            {
                var hashtags = entities.Hashtags.Select(x => $"#{x.Text}");

                lock (this.receivedHashTags)
                    this.receivedHashTags.UnionWith(hashtags);
            }

            if (entities.UserMentions != null)
            {
                foreach (var ent in entities.UserMentions)
                    atList.Add((ent.Id, ent.ScreenName));
            }

            if (entities.Media != null)
            {
                foreach (var ent in entities.Media)
                {
                    if (media.Any(x => x.Url == ent.MediaUrlHttps))
                        continue;

                    var videoUrl =
                        ent.VideoInfo != null && ent.Type == "animated_gif" || ent.Type == "video"
                        ? ent.ExpandedUrl
                        : null;

                    var mediaInfo = new MediaInfo(ent.MediaUrlHttps, ent.AltText, videoUrl);
                    media.Add(mediaInfo);
                }
            }

            return (atList, media);
        }

        private static string CreateAccessibleText(string text, TwitterEntities? entities, TwitterStatus? quotedStatus, TwitterQuotedStatusPermalink? quotedStatusLink)
        {
            if (entities == null)
                return text;

            if (entities.Urls != null)
            {
                foreach (var entity in entities.Urls)
                {
                    if (quotedStatus != null)
                    {
                        var matchStatusUrl = Twitter.StatusUrlRegex.Match(entity.ExpandedUrl);
                        if (matchStatusUrl.Success && matchStatusUrl.Groups["StatusId"].Value == quotedStatus.IdStr)
                        {
                            var quotedText = CreateAccessibleText(quotedStatus.FullText, quotedStatus.MergedEntities, quotedStatus: null, quotedStatusLink: null);
                            text = text.Replace(entity.Url, string.Format(Properties.Resources.QuoteStatus_AccessibleText, quotedStatus.User.ScreenName, quotedText));
                            continue;
                        }
                    }

                    if (!MyCommon.IsNullOrEmpty(entity.DisplayUrl))
                        text = text.Replace(entity.Url, entity.DisplayUrl);
                }
            }

            if (entities.Media != null)
            {
                foreach (var entity in entities.Media)
                {
                    if (!MyCommon.IsNullOrEmpty(entity.AltText))
                    {
                        text = text.Replace(entity.Url, string.Format(Properties.Resources.ImageAltText, entity.AltText));
                    }
                    else if (!MyCommon.IsNullOrEmpty(entity.DisplayUrl))
                    {
                        text = text.Replace(entity.Url, entity.DisplayUrl);
                    }
                }
            }

            if (quotedStatus != null && quotedStatusLink != null)
            {
                var quoteText = CreateAccessibleText(quotedStatus.FullText, quotedStatus.MergedEntities, quotedStatus: null, quotedStatusLink: null);
                text += " " + string.Format(Properties.Resources.QuoteStatus_AccessibleText, quotedStatus.User.ScreenName, quoteText);
            }

            return text;
        }

        internal static string CreateHtmlAnchor(string text, TwitterEntities entities, TwitterQuotedStatusPermalink? quotedStatusLink)
        {
            var mergedEntities = entities.Concat(TweetExtractor.ExtractEmojiEntities(text));

            // PostClass.ExpandedUrlInfo を使用して非同期に URL 展開を行うためここでは expanded_url を使用しない
            text = TweetFormatter.AutoLinkHtml(text, mergedEntities, keepTco: true);

            text = Regex.Replace(text, "(^|[^a-zA-Z0-9_/&#＃@＠>=.~])(sm|nm)([0-9]{1,10})", "$1<a href=\"https://www.nicovideo.jp/watch/$2$3\">$2$3</a>");
            text = PreProcessUrl(text); // IDN置換

            if (quotedStatusLink != null)
            {
                text += string.Format(" <a href=\"{0}\" title=\"{0}\">{1}</a>",
                    WebUtility.HtmlEncode(quotedStatusLink.Expanded),
                    WebUtility.HtmlEncode(quotedStatusLink.Display));
            }

            return text;
        }

        private static string PreProcessUrl(string orgData)
        {
            int posl1;
            var posl2 = 0;
            var href = "<a href=\"";

            while (true)
            {
                if (orgData.IndexOf(href, posl2, StringComparison.Ordinal) > -1)
                {
                    // IDN展開
                    posl1 = orgData.IndexOf(href, posl2, StringComparison.Ordinal);
                    posl1 += href.Length;
                    posl2 = orgData.IndexOf("\"", posl1, StringComparison.Ordinal);
                    var urlStr = orgData.Substring(posl1, posl2 - posl1);

                    if (!urlStr.StartsWith("http://", StringComparison.Ordinal)
                        && !urlStr.StartsWith("https://", StringComparison.Ordinal)
                        && !urlStr.StartsWith("ftp://", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var replacedUrl = MyCommon.IDNEncode(urlStr);
                    if (replacedUrl == null) continue;
                    if (replacedUrl == urlStr) continue;

                    orgData = orgData.Replace("<a href=\"" + urlStr, "<a href=\"" + replacedUrl);
                    posl2 = 0;
                }
                else
                {
                    break;
                }
            }
            return orgData;
        }

        /// <summary>
        /// Twitter APIから得たHTML形式のsource文字列を分析し、source名とURLに分離します
        /// </summary>
        public static (string SourceText, Uri? SourceUri) ParseSource(string? sourceHtml)
        {
            if (MyCommon.IsNullOrEmpty(sourceHtml))
                return ("", null);

            string sourceText;
            Uri? sourceUri;

            // sourceHtmlの例: <a href="http://twitter.com" rel="nofollow">Twitter Web Client</a>

            var match = Regex.Match(sourceHtml, "^<a href=\"(?<uri>.+?)\".*?>(?<text>.+)</a>$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                sourceText = WebUtility.HtmlDecode(match.Groups["text"].Value);
                try
                {
                    var uriStr = WebUtility.HtmlDecode(match.Groups["uri"].Value);
                    sourceUri = new Uri(SourceUriBase, uriStr);
                }
                catch (UriFormatException)
                {
                    sourceUri = null;
                }
            }
            else
            {
                sourceText = WebUtility.HtmlDecode(sourceHtml);
                sourceUri = null;
            }

            return (sourceText, sourceUri);
        }

        /// <summary>
        /// ツイートに含まれる引用ツイートのURLからステータスIDを抽出
        /// </summary>
        public static IEnumerable<long> GetQuoteTweetStatusIds(IEnumerable<TwitterEntity>? entities, TwitterQuotedStatusPermalink? quotedStatusLink)
        {
            entities ??= Enumerable.Empty<TwitterEntity>();

            var urls = entities.OfType<TwitterEntityUrl>().Select(x => x.ExpandedUrl);

            if (quotedStatusLink != null)
                urls = urls.Append(quotedStatusLink.Expanded);

            return GetQuoteTweetStatusIds(urls);
        }

        public static IEnumerable<long> GetQuoteTweetStatusIds(IEnumerable<string> urls)
        {
            foreach (var url in urls)
            {
                var match = Twitter.StatusUrlRegex.Match(url);
                if (match.Success)
                {
                    if (long.TryParse(match.Groups["StatusId"].Value, out var statusId))
                        yield return statusId;
                }
            }
        }
    }
}
