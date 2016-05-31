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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween.Models
{
    public class PostClass : ICloneable
    {
        public struct StatusGeo : IEquatable<StatusGeo>
        {
            public double Longitude { get; }
            public double Latitude { get; }

            public StatusGeo(double longitude, double latitude)
            {
                this.Longitude = longitude;
                this.Latitude = latitude;
            }

            public override int GetHashCode()
                => this.Longitude.GetHashCode() ^ this.Latitude.GetHashCode();

            public override bool Equals(object obj)
                => obj is StatusGeo && this.Equals((StatusGeo)obj);

            public bool Equals(StatusGeo other)
                => this.Longitude == other.Longitude && this.Latitude == other.Longitude;

            public static bool operator ==(StatusGeo left, StatusGeo right)
                => left.Equals(right);

            public static bool operator !=(StatusGeo left, StatusGeo right)
                => !left.Equals(right);
        }
        public string Nickname { get; set; }
        public string TextFromApi { get; set; }

        /// <summary>スクリーンリーダーでの読み上げを考慮したテキスト</summary>
        public string AccessibleText { get; set; }

        public string ImageUrl { get; set; }
        public string ScreenName { get; set; }
        public DateTime CreatedAt { get; set; }
        public long StatusId { get; set; }
        private bool _IsFav;

        public string Text
        {
            get
            {
                if (this.expandComplatedAll)
                    return this._text;

                var expandedHtml = this.ReplaceToExpandedUrl(this._text, out this.expandComplatedAll);
                if (this.expandComplatedAll)
                    this._text = expandedHtml;

                return expandedHtml;
            }
            set { this._text = value; }
        }
        private string _text;

        public bool IsRead { get; set; }
        public bool IsReply { get; set; }
        public bool IsExcludeReply { get; set; }
        private bool _IsProtect;
        public bool IsOwl { get; set; }
        private bool _IsMark;
        public string InReplyToUser { get; set; }
        private long? _InReplyToStatusId;
        public string Source { get; set; }
        public Uri SourceUri { get; set; }
        public List<string> ReplyToList { get; set; }
        public bool IsMe { get; set; }
        public bool IsDm { get; set; }
        public long UserId { get; set; }
        public bool FilterHit { get; set; }
        public string RetweetedBy { get; set; }
        public long? RetweetedId { get; set; }
        private bool _IsDeleted = false;
        private StatusGeo? _postGeo = null;
        public int RetweetedCount { get; set; }
        public long? RetweetedByUserId { get; set; }
        public long? InReplyToUserId { get; set; }
        public List<MediaInfo> Media { get; set; }
        public long[] QuoteStatusIds { get; set; }
        public ExpandedUrlInfo[] ExpandedUrls { get; set; }

        /// <summary>
        /// <see cref="PostClass"/> に含まれる t.co の展開後の URL を保持するクラス
        /// </summary>
        public class ExpandedUrlInfo : ICloneable
        {
            /// <summary>展開前の t.co ドメインの URL</summary>
            public string Url { get; }

            /// <summary>展開後の URL</summary>
            /// <remarks>
            /// <see cref="ShortUrl"/> による展開が完了するまでは Entity に含まれる expanded_url の値を返します
            /// </remarks>
            public string ExpandedUrl => this._expandedUrl;

            /// <summary><see cref="ShortUrl"/> による展開を行うタスク</summary>
            public Task ExpandTask { get; private set; }

            /// <summary><see cref="DeepExpandAsync"/> による展開が完了したか否か</summary>
            public bool ExpandedCompleted => this.ExpandTask.IsCompleted;

            protected string _expandedUrl;

            public ExpandedUrlInfo(string url, string expandedUrl)
                : this(url, expandedUrl, deepExpand: true)
            {
            }

            public ExpandedUrlInfo(string url, string expandedUrl, bool deepExpand)
            {
                this.Url = url;
                this._expandedUrl = expandedUrl;

                if (deepExpand)
                    this.ExpandTask = this.DeepExpandAsync();
                else
                    this.ExpandTask = Task.FromResult(0);
            }

            protected virtual async Task DeepExpandAsync()
            {
                var origUrl = this._expandedUrl;
                var newUrl = await ShortUrl.Instance.ExpandUrlAsync(origUrl)
                    .ConfigureAwait(false);

                Interlocked.CompareExchange(ref this._expandedUrl, newUrl, origUrl);
            }

            public ExpandedUrlInfo Clone()
                => new ExpandedUrlInfo(this.Url, this.ExpandedUrl, deepExpand: false);

            object ICloneable.Clone()
                => this.Clone();
        }

        public int FavoritedCount { get; set; }

        private States _states = States.None;
        private bool expandComplatedAll = false;

        [Flags]
        private enum States
        {
            None = 0,
            Protect = 1,
            Mark = 2,
            Reply = 4,
            Geo = 8,
        }

        public PostClass()
        {
            RetweetedBy = "";
            Media = new List<MediaInfo>();
            ReplyToList = new List<string>();
            QuoteStatusIds = new long[0];
            ExpandedUrls = new ExpandedUrlInfo[0];
        }

        public string TextSingleLine
        {
            get
            {
                return this.TextFromApi == null ? null : this.TextFromApi.Replace("\n", " ");
            }
        }

        public bool IsFav
        {
            get
            {
                if (this.RetweetedId != null)
                {
                    var post = this.RetweetSource;
                    if (post != null)
                    {
                        return post.IsFav;
                    }
                }

                return _IsFav;
            }
            set
            {
                _IsFav = value;
                if (this.RetweetedId != null)
                {
                    var post = this.RetweetSource;
                    if (post != null)
                    {
                        post.IsFav = value;
                    }
                }
            }
        }

        public bool IsProtect
        {
            get
            {
                return _IsProtect;
            }
            set
            {
                if (value)
                {
                    _states = _states | States.Protect;
                }
                else
                {
                    _states = _states & ~States.Protect;
                }
                _IsProtect = value;
            }
        }
        public bool IsMark
        {
            get
            {
                return _IsMark;
            }
            set
            {
                if (value)
                {
                    _states = _states | States.Mark;
                }
                else
                {
                    _states = _states & ~States.Mark;
                }
                _IsMark = value;
            }
        }
        public long? InReplyToStatusId
        {
            get
            {
                return _InReplyToStatusId;
            }
            set
            {
                if (value != null)
                {
                    _states = _states | States.Reply;
                }
                else
                {
                    _states = _states & ~States.Reply;
                }
                _InReplyToStatusId = value;
            }
        }

        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (value)
                {
                    this.InReplyToStatusId = null;
                    this.InReplyToUser = "";
                    this.InReplyToUserId = null;
                    this.IsReply = false;
                    this.ReplyToList = new List<string>();
                    this._states = States.None;
                }
                _IsDeleted = value;
            }
        }

        protected virtual PostClass RetweetSource
        {
            get
            {
                return TabInformations.GetInstance().RetweetSource(this.RetweetedId.Value);
            }
        }

        public StatusGeo? PostGeo
        {
            get
            {
                return _postGeo;
            }
            set
            {
                if (value != null)
                {
                    _states |= States.Geo;
                }
                else
                {
                    _states &= ~States.Geo;
                }
                _postGeo = value;
            }
        }

        public int StateIndex
        {
            get
            {
                return (int)_states - 1;
            }
        }

        // 互換性のために用意
        public string SourceHtml
        {
            get
            {
                if (this.SourceUri == null)
                    return WebUtility.HtmlEncode(this.Source);

                return string.Format("<a href=\"{0}\" rel=\"nofollow\">{1}</a>",
                    WebUtility.HtmlEncode(this.SourceUri.AbsoluteUri), WebUtility.HtmlEncode(this.Source));
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

        public PostClass ConvertToOriginalPost()
        {
            if (this.RetweetedId == null)
                throw new InvalidOperationException();

            var originalPost = this.Clone();

            originalPost.StatusId = this.RetweetedId.Value;
            originalPost.RetweetedId = null;
            originalPost.RetweetedBy = "";
            originalPost.RetweetedByUserId = null;
            originalPost.RetweetedCount = 1;

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

        public PostClass Clone()
        {
            var clone = (PostClass)this.MemberwiseClone();
            clone.ReplyToList = new List<string>(this.ReplyToList);
            clone.Media = new List<MediaInfo>(this.Media);
            clone.QuoteStatusIds = this.QuoteStatusIds.ToArray();
            clone.ExpandedUrls = this.ExpandedUrls.Select(x => x.Clone()).ToArray();

            return clone;
        }

        object ICloneable.Clone()
            => this.Clone();

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this.Equals((PostClass)obj);
        }

        public bool Equals(PostClass other)
        {
            if (other == null) return false;
            return (this.Nickname == other.Nickname) &&
                    (this.TextFromApi == other.TextFromApi) &&
                    (this.ImageUrl == other.ImageUrl) &&
                    (this.ScreenName == other.ScreenName) &&
                    (this.CreatedAt == other.CreatedAt) &&
                    (this.StatusId == other.StatusId) &&
                    (this.IsFav == other.IsFav) &&
                    (this.Text == other.Text) &&
                    (this.IsRead == other.IsRead) &&
                    (this.IsReply == other.IsReply) &&
                    (this.IsExcludeReply == other.IsExcludeReply) &&
                    (this.IsProtect == other.IsProtect) &&
                    (this.IsOwl == other.IsOwl) &&
                    (this.IsMark == other.IsMark) &&
                    (this.InReplyToUser == other.InReplyToUser) &&
                    (this.InReplyToStatusId == other.InReplyToStatusId) &&
                    (this.Source == other.Source) &&
                    (this.SourceUri == other.SourceUri) &&
                    (this.ReplyToList.SequenceEqual(other.ReplyToList)) &&
                    (this.IsMe == other.IsMe) &&
                    (this.IsDm == other.IsDm) &&
                    (this.UserId == other.UserId) &&
                    (this.FilterHit == other.FilterHit) &&
                    (this.RetweetedBy == other.RetweetedBy) &&
                    (this.RetweetedId == other.RetweetedId) &&
                    (this.IsDeleted == other.IsDeleted) &&
                    (this.InReplyToUserId == other.InReplyToUserId);

        }

        public override int GetHashCode()
        {
            return this.StatusId.GetHashCode();
        }
    }
}
