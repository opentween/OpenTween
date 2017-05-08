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
using System.Threading.Tasks;
using OpenTween.Connection;

namespace OpenTween.Models
{
    public class MastodonPost : PostClass
    {
        private readonly Mastodon mastodon;

        public MastodonPost(Mastodon mastodon)
            => this.mastodon = mastodon;

        public override async Task FavoriteAsync(SettingCommon settingCommon)
        {
            var statusId = this.IsRetweet ? this.RetweetedId : this.StatusId;

            await this.mastodon.Api.StatusesFavourite(statusId)
                .IgnoreResponse()
                .ConfigureAwait(false);

            this.IsFav = true;
        }

        public override async Task UnfavoriteAsync()
        {
            var statusId = this.IsRetweet ? this.RetweetedId : this.StatusId;

            await this.mastodon.Api.StatusesUnfavourite(statusId)
                .IgnoreResponse()
                .ConfigureAwait(false);

            this.IsFav = false;
        }

        public override async Task<PostClass?> RetweetAsync(SettingCommon settingCommon)
        {
            var statusId = this.IsRetweet ? this.RetweetedId : this.StatusId;

            var response = await this.mastodon.Api.StatusesReblog(statusId)
                .ConfigureAwait(false);

            var status = await response.LoadJsonAsync()
                .ConfigureAwait(false);

            return this.mastodon.CreatePost(status);
        }

        public override Task DeleteAsync()
        {
            if (this.IsRetweet && this.RetweetedByUserId == this.mastodon.UserId)
            {
                // 自分がブーストしたトゥート (自分がブーストした自分のトゥートも含む)
                //   => ブーストを取り消し
                return this.mastodon.Api.StatusesUnreblog(this.StatusId);
            }
            else
            {
                if (this.UserId != this.mastodon.UserId)
                    throw new InvalidOperationException();

                if (this.IsRetweet)
                    // 他人にブーストされた自分のトゥート
                    //   => ブースト元の自分のトゥートを削除
                    return this.mastodon.Api.StatusesDelete(this.RetweetedId);
                else
                    // 自分のトゥート
                    //   => トゥートを削除
                    return this.mastodon.Api.StatusesDelete(this.StatusId);
            }
        }
    }
}
