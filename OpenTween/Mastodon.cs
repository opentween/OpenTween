// OpenTween - Client of Twitter
// Copyright (c) 2017 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenTween.Api;
using OpenTween.Api.DataModel;
using OpenTween.Models;

namespace OpenTween
{
    public sealed class Mastodon : IDisposable
    {
        public long UserId { get; private set; }

        public string Username { get; private set; } = "";

        public MastodonApi Api => this.ApiInternal ?? throw new WebApiException("Unauthorized");

        internal MastodonApi ApiInternal = null!;

        public void Initialize(MastodonCredential account)
        {
            this.ApiInternal = new MastodonApi(new Uri(account.InstanceUri), account.AccessTokenPlain);

            this.UserId = account.UserId;
            this.Username = account.Username;
        }

        public async Task<PostClass[]> GetHomeTimelineAsync(MastodonHomeTab tab, bool backward)
        {
            MastodonStatus[] statuses;
            if (backward)
            {
                statuses = await this.Api.TimelinesHome(maxId: tab.OldestId)
                    .ConfigureAwait(false);
            }
            else
            {
                statuses = await this.Api.TimelinesHome()
                    .ConfigureAwait(false);
            }

            return statuses.Select(x => this.CreatePost(x)).ToArray();
        }

        public PostClass CreatePost(MastodonStatus status)
        {
            var post = new MastodonPost(this);
            post.StatusId = status.Id; // TODO: Twitterの status_id と衝突する

            if (status.Reblog != null)
            {
                var reblog = status.Reblog;
                post.CreatedAt = this.ParseDateTime(reblog.CreatedAt);
                post.RetweetedId = reblog.Id;
                post.TextFromApi = Regex.Replace(reblog.Content, "<[^>]+>", "");
                post.Text = reblog.Content;
                post.IsFav = reblog.Favourited ?? false;

                if (reblog.InReplyToId != null)
                {
                    post.InReplyToStatusId = reblog.InReplyToId.Value;
                    post.InReplyToUserId = reblog.InReplyToAccountId!.Value;
                    post.InReplyToUser = reblog.Mentions.FirstOrDefault()?.Acct ?? "";
                }

                post.UserId = reblog.Account.Id;
                post.ScreenName = reblog.Account.Acct;
                post.Nickname = reblog.Account.DisplayName;
                post.ImageUrl = reblog.Account.AvatarStatic;
                post.IsProtect = !(reblog.Visibility == "public" || reblog.Visibility == "unlisted");

                post.RetweetedBy = status.Account.Acct;
                post.RetweetedByUserId = status.Account.Id;
                post.IsMe = status.Account.Id == this.UserId;
            }
            else // status.Reblog == null
            {
                post.CreatedAt = this.ParseDateTime(status.CreatedAt);
                post.TextFromApi = Regex.Replace(status.Content, "<[^>]+>", "");
                post.Text = status.Content;
                post.IsFav = status.Favourited ?? false;

                if (status.InReplyToId != null)
                {
                    post.InReplyToStatusId = status.InReplyToId.Value;
                    post.InReplyToUserId = status.InReplyToAccountId!.Value;
                    post.InReplyToUser = status.Mentions.FirstOrDefault()?.Acct ?? "";
                }

                post.UserId = status.Account.Id;
                post.ScreenName = status.Account.Acct;
                post.Nickname = status.Account.DisplayName;
                post.ImageUrl = status.Account.AvatarStatic;
                post.IsProtect = !(status.Visibility == "public" || status.Visibility == "unlisted");
                post.IsMe = status.Account.Id == this.UserId;
            }

            post.TextFromApi = WebUtility.HtmlDecode(post.TextFromApi);
            post.TextFromApi = post.TextFromApi.Replace("<3", "\u2661");
            post.AccessibleText = post.TextFromApi;

            post.QuoteStatusIds = new long[0];
            post.ExpandedUrls = new PostClass.ExpandedUrlInfo[0];

            var application = status.Application;
            if (application != null)
            {
                post.Source = application.Name;
                post.SourceUri = application.Website != null ? new Uri(application.Website) : null;
            }
            else
            {
                post.Source = "";
            }

            return post;
        }

        public DateTimeUtc ParseDateTime(string datetime)
            => DateTimeUtc.Parse(datetime, DateTimeFormatInfo.InvariantInfo);

        public void Dispose()
            => this.ApiInternal?.Dispose();
    }
}
