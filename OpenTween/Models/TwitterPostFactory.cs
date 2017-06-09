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
            Twitter twitter,
            TwitterStatus status,
            long selfUserId,
            ISet<long> followerIds,
            bool favTweet = false
        )
        {
            var post = new TwitterStatusPost(twitter);
            TwitterEntities entities;
            string sourceHtml;

            post.StatusId = status.Id;
            if (status.RetweetedStatus != null)
            {
                var retweeted = status.RetweetedStatus;

                post.CreatedAt = MyCommon.DateTimeParse(retweeted.CreatedAt);

                // Id
                post.RetweetedId = retweeted.Id;
                // 本文
                post.TextFromApi = retweeted.FullText;
                entities = retweeted.MergedEntities;
                sourceHtml = retweeted.Source;
                // Reply先
                post.InReplyToStatusId = retweeted.InReplyToStatusId;
                post.InReplyToUser = retweeted.InReplyToScreenName;
                post.InReplyToUserId = status.InReplyToUserId;

                if (favTweet)
                {
                    post.IsFav = true;
                }
                else
                {
                    // 幻覚fav対策
                    var favTab = this.tabinfo.FavoriteTab;
                    post.IsFav = favTab.Contains(retweeted.Id);
                }

                if (retweeted.Coordinates != null)
                    post.PostGeo = new PostClass.StatusGeo(retweeted.Coordinates.Coordinates[0], retweeted.Coordinates.Coordinates[1]);

                // 以下、ユーザー情報
                var user = retweeted.User;
                if (user != null)
                {
                    post.UserId = user.Id;
                    post.ScreenName = user.ScreenName;
                    post.Nickname = user.Name.Trim();
                    post.ImageUrl = user.ProfileImageUrlHttps;
                    post.IsProtect = user.Protected;
                }
                else
                {
                    post.UserId = 0L;
                    post.ScreenName = "?????";
                    post.Nickname = "Unknown User";
                }

                // Retweetした人
                if (status.User != null)
                {
                    post.RetweetedBy = status.User.ScreenName;
                    post.RetweetedByUserId = status.User.Id;
                    post.IsMe = post.RetweetedByUserId == selfUserId;
                }
                else
                {
                    post.RetweetedBy = "?????";
                    post.RetweetedByUserId = 0L;
                }
            }
            else
            {
                post.CreatedAt = MyCommon.DateTimeParse(status.CreatedAt);
                // 本文
                post.TextFromApi = status.FullText;
                entities = status.MergedEntities;
                sourceHtml = status.Source;
                post.InReplyToStatusId = status.InReplyToStatusId;
                post.InReplyToUser = status.InReplyToScreenName;
                post.InReplyToUserId = status.InReplyToUserId;

                if (favTweet)
                {
                    post.IsFav = true;
                }
                else
                {
                    // 幻覚fav対策
                    var favTab = this.tabinfo.FavoriteTab;
                    post.IsFav = favTab.Posts.TryGetValue(post.StatusId, out var tabinfoPost) && tabinfoPost.IsFav;
                }

                if (status.Coordinates != null)
                    post.PostGeo = new PostClass.StatusGeo(status.Coordinates.Coordinates[0], status.Coordinates.Coordinates[1]);

                // 以下、ユーザー情報
                var user = status.User;
                if (user != null)
                {
                    post.UserId = user.Id;
                    post.ScreenName = user.ScreenName;
                    post.Nickname = user.Name.Trim();
                    post.ImageUrl = user.ProfileImageUrlHttps;
                    post.IsProtect = user.Protected;
                    post.IsMe = post.UserId == selfUserId;
                }
                else
                {
                    post.UserId = 0L;
                    post.ScreenName = "?????";
                    post.Nickname = "Unknown User";
                }
            }
            // HTMLに整形
            var textFromApi = post.TextFromApi;

            var quotedStatusLink = (status.RetweetedStatus ?? status).QuotedStatusPermalink;

            if (quotedStatusLink != null && entities.Urls != null && entities.Urls.Any(x => x.ExpandedUrl == quotedStatusLink.Expanded))
                quotedStatusLink = null; // 移行期は entities.urls と quoted_status_permalink の両方に含まれる場合がある

            post.Text = CreateHtmlAnchor(textFromApi, entities, quotedStatusLink);
            post.TextFromApi = textFromApi;
            post.TextFromApi = this.ReplaceTextFromApi(post.TextFromApi, entities, quotedStatusLink);
            post.TextFromApi = WebUtility.HtmlDecode(post.TextFromApi);
            post.TextFromApi = post.TextFromApi.Replace("<3", "\u2661");
            post.AccessibleText = CreateAccessibleText(textFromApi, entities, (status.RetweetedStatus ?? status).QuotedStatus, quotedStatusLink);
            post.AccessibleText = WebUtility.HtmlDecode(post.AccessibleText);
            post.AccessibleText = post.AccessibleText.Replace("<3", "\u2661");

            this.ExtractEntities(entities, post.ReplyToList, post.Media);

            post.QuoteStatusIds = GetQuoteTweetStatusIds(entities, quotedStatusLink)
                .Where(x => x != post.StatusId && !(post.IsRetweet && x == post.RetweetedId))
                .Distinct().ToArray();

            post.ExpandedUrls = entities.OfType<TwitterEntityUrl>()
                .Select(x => new PostClass.ExpandedUrlInfo(x.Url, x.ExpandedUrl))
                .ToArray();

            // メモリ使用量削減 (同一のテキストであれば同一の string インスタンスを参照させる)
            if (post.Text == post.TextFromApi)
                post.Text = post.TextFromApi;
            if (post.AccessibleText == post.TextFromApi)
                post.AccessibleText = post.TextFromApi;

            // 他の発言と重複しやすい (共通化できる) 文字列は string.Intern を通す
            post.ScreenName = string.Intern(post.ScreenName);
            post.Nickname = string.Intern(post.Nickname);
            post.ImageUrl = string.Intern(post.ImageUrl);
            if (post.IsRetweet)
                post.RetweetedBy = string.Intern(post.RetweetedBy);

            // Source整形
            var (sourceText, sourceUri) = ParseSource(sourceHtml);
            post.Source = string.Intern(sourceText);
            post.SourceUri = sourceUri;

            post.IsReply = !post.IsRetweet && post.ReplyToList.Any(x => x.UserId == selfUserId);
            post.IsExcludeReply = false;

            if (post.IsMe)
            {
                post.IsOwl = false;
            }
            else
            {
                if (followerIds.Count > 0)
                    post.IsOwl = !followerIds.Contains(post.UserId);
            }

            post.IsDm = false;
            return post;
        }

        public PostClass CreateFromDirectMessageEvent(
            Twitter twitter,
            TwitterMessageEvent eventItem,
            IReadOnlyDictionary<string, TwitterUser> users,
            IReadOnlyDictionary<string, TwitterMessageEventList.App> apps,
            long selfUserId
        )
        {
            var post = new TwitterDmPost(twitter);
            post.StatusId = long.Parse(eventItem.Id);

            var timestamp = long.Parse(eventItem.CreatedTimestamp);
            post.CreatedAt = DateTimeUtc.UnixEpoch + TimeSpan.FromTicks(timestamp * TimeSpan.TicksPerMillisecond);
            // 本文
            var textFromApi = eventItem.MessageCreate.MessageData.Text;

            var entities = eventItem.MessageCreate.MessageData.Entities;
            var mediaEntity = eventItem.MessageCreate.MessageData.Attachment?.Media;

            if (mediaEntity != null)
                entities.Media = new[] { mediaEntity };

            // HTMLに整形
            post.Text = CreateHtmlAnchor(textFromApi, entities, quotedStatusLink: null);
            post.TextFromApi = this.ReplaceTextFromApi(textFromApi, entities, quotedStatusLink: null);
            post.TextFromApi = WebUtility.HtmlDecode(post.TextFromApi);
            post.TextFromApi = post.TextFromApi.Replace("<3", "\u2661");
            post.AccessibleText = CreateAccessibleText(textFromApi, entities, quotedStatus: null, quotedStatusLink: null);
            post.AccessibleText = WebUtility.HtmlDecode(post.AccessibleText);
            post.AccessibleText = post.AccessibleText.Replace("<3", "\u2661");
            post.IsFav = false;

            this.ExtractEntities(entities, post.ReplyToList, post.Media);

            post.QuoteStatusIds = GetQuoteTweetStatusIds(entities, quotedStatusLink: null)
                .Distinct().ToArray();

            post.ExpandedUrls = entities.OfType<TwitterEntityUrl>()
                .Select(x => new PostClass.ExpandedUrlInfo(x.Url, x.ExpandedUrl))
                .ToArray();

            // 以下、ユーザー情報
            string userId;
            if (eventItem.MessageCreate.SenderId != selfUserId.ToString(CultureInfo.InvariantCulture))
            {
                userId = eventItem.MessageCreate.SenderId;
                post.IsMe = false;
                post.IsOwl = true;
            }
            else
            {
                userId = eventItem.MessageCreate.Target.RecipientId;
                post.IsMe = true;
                post.IsOwl = false;
            }

            if (users.TryGetValue(userId, out var user))
            {
                post.UserId = user.Id;
                post.ScreenName = user.ScreenName;
                post.Nickname = user.Name.Trim();
                post.ImageUrl = user.ProfileImageUrlHttps;
                post.IsProtect = user.Protected;
            }
            else
            {
                post.UserId = 0L;
                post.ScreenName = "?????";
                post.Nickname = "Unknown User";
            }

            // メモリ使用量削減 (同一のテキストであれば同一の string インスタンスを参照させる)
            if (post.Text == post.TextFromApi)
                post.Text = post.TextFromApi;
            if (post.AccessibleText == post.TextFromApi)
                post.AccessibleText = post.TextFromApi;

            // 他の発言と重複しやすい (共通化できる) 文字列は string.Intern を通す
            post.ScreenName = string.Intern(post.ScreenName);
            post.Nickname = string.Intern(post.Nickname);
            post.ImageUrl = string.Intern(post.ImageUrl);

            var appId = eventItem.MessageCreate.SourceAppId;
            if (appId != null && apps.TryGetValue(appId, out var app))
            {
                post.Source = string.Intern(app.Name);

                try
                {
                    post.SourceUri = new Uri(SourceUriBase, app.Url);
                }
                catch (UriFormatException)
                {
                }
            }

            post.IsReply = false;
            post.IsExcludeReply = false;
            post.IsDm = true;

            return post;
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

        private void ExtractEntities(TwitterEntities? entities, List<(long UserId, string ScreenName)> atList, List<MediaInfo> media)
        {
            if (entities == null)
                return;

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
                if (media != null)
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
            }
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
