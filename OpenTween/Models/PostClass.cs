// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
//           (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween.Models
{
    public record PostClass()
    {
        public readonly record struct StatusGeo(
            double Longitude,
            double Latitude
        );

        public string Nickname { get; init; } = "";

        public string TextFromApi { get; init; } = "";

        /// <summary>スクリーンリーダーでの読み上げを考慮したテキスト</summary>
        public string AccessibleText { get; init; } = "";

        public string ImageUrl { get; init; } = "";

        public string ScreenName { get; init; } = "";

        public DateTimeUtc CreatedAt { get; init; }

        /// <summary>ソート用の日時</summary>
        /// <remarks>
        /// <see cref="CreatedAt"/>はリツイートの場合にRT元の日時を表すため、
        /// ソート用に使用するタイムスタンプを保持する必要がある
        /// </remarks>
        public DateTimeUtc CreatedAtForSorting { get; init; }

        public long StatusId { get; init; }

        public string Text
        {
            get
            {
                if (this.expandComplatedAll)
                    return this.text;

                var expandedHtml = this.ReplaceToExpandedUrl(this.text, out this.expandComplatedAll);
                if (this.expandComplatedAll)
                    this.text = expandedHtml;

                return expandedHtml;
            }
            init => this.text = value;
        }

        private string text = "";

        public bool IsReply { get; init; }

        public string? InReplyToUser { get; init; }

        public string Source { get; init; } = "";

        public Uri? SourceUri { get; init; }

        public List<(long UserId, string ScreenName)> ReplyToList { get; init; } = new();

        public bool IsMe { get; init; }

        public bool IsDm { get; init; }

        public long UserId { get; init; }

        public string? RetweetedBy { get; init; }

        public long? RetweetedId { get; init; }

        public long? RetweetedByUserId { get; init; }

        public long? InReplyToUserId { get; init; }

        public List<MediaInfo> Media { get; init; } = new();

        public long[] QuoteStatusIds { get; init; } = Array.Empty<long>();

        public ExpandedUrlInfo[] ExpandedUrls { get; init; } = Array.Empty<ExpandedUrlInfo>();

        /// <summary>
        /// <see cref="PostClass"/> に含まれる t.co の展開後の URL を保持するクラス
        /// </summary>
        public class ExpandedUrlInfo : ICloneable
        {
            public static bool AutoExpand { get; set; } = true;

            /// <summary>展開前の t.co ドメインの URL</summary>
            public string Url { get; }

            /// <summary>展開後の URL</summary>
            /// <remarks>
            /// <see cref="ShortUrl"/> による展開が完了するまでは Entity に含まれる expanded_url の値を返します
            /// </remarks>
            public string ExpandedUrl => this.expandedUrl;

            /// <summary><see cref="ShortUrl"/> による展開を行うタスク</summary>
            public Task ExpandTask { get; private set; }

            /// <summary><see cref="DeepExpandAsync"/> による展開が完了したか否か</summary>
            public bool ExpandedCompleted => this.ExpandTask.IsCompleted;

            protected string expandedUrl;

            public ExpandedUrlInfo(string url, string expandedUrl)
                : this(url, expandedUrl, deepExpand: true)
            {
            }

            public ExpandedUrlInfo(string url, string expandedUrl, bool deepExpand)
            {
                this.Url = url;
                this.expandedUrl = expandedUrl;

                if (AutoExpand && deepExpand)
                    this.ExpandTask = this.DeepExpandAsync();
                else
                    this.ExpandTask = Task.CompletedTask;
            }

            protected virtual async Task DeepExpandAsync()
            {
                var origUrl = this.expandedUrl;
                var newUrl = await ShortUrl.Instance.ExpandUrlAsync(origUrl)
                    .ConfigureAwait(false);

                Interlocked.CompareExchange(ref this.expandedUrl, newUrl, origUrl);
            }

            public ExpandedUrlInfo Clone()
                => new(this.Url, this.ExpandedUrl, deepExpand: false);

            object ICloneable.Clone()
                => this.Clone();
        }

        [Flags]
        private enum States
        {
            None = 0,
            Protect = 1,
            Mark = 2,
            Reply = 4,
            Geo = 8,
        }

        public int StateIndex
        {
            get
            {
                var states = States.None;

                if (this.IsProtect)
                    states |= States.Protect;
                if (this.IsMark)
                    states |= States.Mark;
                if (this.InReplyToStatusId != null)
                    states |= States.Reply;
                if (this.PostGeo != null)
                    states |= States.Geo;

                return (int)states - 1;
            }
        }

        public string TextSingleLine
            => this.TextFromApi.Replace("\n", " ");

        public long? InReplyToStatusId { get; init; }

        public bool IsProtect { get; init; }

        public StatusGeo? PostGeo { get; set; }

        public bool IsFav { get; set; }

        public bool IsRead { get; set; }

        public bool IsExcludeReply { get; set; }

        public bool IsOwl { get; set; }

        public bool IsMark { get; set; }

        public bool IsDeleted { get; set; }

        public bool FilterHit { get; set; }

        public int RetweetedCount { get; set; }

        public int FavoritedCount { get; set; }

        private bool expandComplatedAll = false;

        // 互換性のために用意
        public string SourceHtml
        {
            get
            {
                if (this.SourceUri == null)
                    return WebUtility.HtmlEncode(this.Source);

                return string.Format(
                    "<a href=\"{0}\" rel=\"nofollow\">{1}</a>",
                    WebUtility.HtmlEncode(this.SourceUri.AbsoluteUri),
                    WebUtility.HtmlEncode(this.Source));
            }
        }

        /// <summary>
        /// このツイートが指定したユーザーによって削除可能であるかを判定します
        /// </summary>
        /// <param name="selfUserId">ツイートを削除しようとするユーザーのID</param>
        /// <returns>削除可能であれば true、そうでなければ false</returns>
        public bool CanDeleteBy(long selfUserId)
        {
            // 自分が送った DM と自分に届いた DM のどちらも削除可能
            if (this.IsDm)
                return true;

            // 自分のツイート or 他人に RT された自分のツイート
            if (this.UserId == selfUserId)
                return true;

            // 自分が RT したツイート
            if (this.RetweetedByUserId == selfUserId)
                return true;

            return false;
        }

        /// <summary>
        /// このツイートが指定したユーザーによってリツイート可能であるかを判定します
        /// </summary>
        /// <param name="selfUserId">リツイートしようとするユーザーのID</param>
        /// <returns>リツイート可能であれば true、そうでなければ false</returns>
        public bool CanRetweetBy(long selfUserId)
        {
            // DM は常にリツイート不可
            if (this.IsDm)
                return false;

            // 自分のツイートであれば鍵垢であるかに関わらずリツイート可
            if (this.UserId == selfUserId)
                return true;

            return !this.IsProtect;
        }

        public PostClass ConvertToOriginalPost()
        {
            if (this.RetweetedId == null)
                throw new InvalidOperationException();

            var originalPost = this with
            {
                StatusId = this.RetweetedId.Value,
                CreatedAtForSorting = this.CreatedAt,
                RetweetedId = null,
                RetweetedBy = "",
                RetweetedByUserId = null,
                RetweetedCount = 1,
            };

            return originalPost;
        }

        public string GetExpandedUrl(string urlStr)
        {
            var urlInfo = this.ExpandedUrls.FirstOrDefault(x => x.Url == urlStr);
            if (urlInfo == null)
                return urlStr;

            return urlInfo.ExpandedUrl;
        }

        public string[] GetExpandedUrls()
            => this.ExpandedUrls.Select(x => x.ExpandedUrl).ToArray();

        /// <summary>
        /// <paramref name="html"/> に含まれる短縮 URL を展開済みの URL に置換します
        /// </summary>
        /// <param name="html">置換する対象の HTML 文字列</param>
        /// <param name="completedAll">全ての URL の展開が完了していれば true、未完了の URL があれば false</param>
        private string ReplaceToExpandedUrl(string html, out bool completedAll)
        {
            if (this.ExpandedUrls.Length == 0)
            {
                completedAll = true;
                return html;
            }

            completedAll = true;

            foreach (var urlInfo in this.ExpandedUrls)
            {
                if (!urlInfo.ExpandedCompleted)
                    completedAll = false;

                var tcoUrl = urlInfo.Url;
                var expandedUrl = MyCommon.ConvertToReadableUrl(urlInfo.ExpandedUrl);
                html = html.Replace($"title=\"{WebUtility.HtmlEncode(tcoUrl)}\"",
                    $"title=\"{WebUtility.HtmlEncode(expandedUrl)}\"");
            }

            return html;
        }
    }
}
