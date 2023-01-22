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
using System.Linq;
using System.Threading.Tasks;
using OpenTween.Api;
using OpenTween.Api.DataModel;
using OpenTween.Connection;
using OpenTween.Setting;

namespace OpenTween.Models
{
    public class TwitterStatusPost : PostClass
    {
        private readonly Twitter twitter;

        public TwitterStatusPost(Twitter twitter)
            => this.twitter = twitter;

        public override async Task FavoriteAsync(SettingCommon settingCommon)
        {
            var statusId = this.IsRetweet ? this.RetweetedId : this.StatusId;

            try
            {
                await this.twitter.Api.FavoritesCreate(statusId)
                    .IgnoreResponse()
                    .ConfigureAwait(false);
            }
            catch (TwitterApiException ex)
                when (ex.Errors.All(x => x.Code == TwitterErrorCode.AlreadyFavorited))
            {
                // エラーコード 139 のみの場合は成功と見なす
            }

            if (settingCommon.RestrictFavCheck)
            {
                var status = await this.twitter.Api.StatusesShow(statusId)
                    .ConfigureAwait(false);

                if (status.Favorited != true)
                    throw new WebApiException("NG(Restricted?)");
            }

            this.IsFav = true;
        }

        public override async Task UnfavoriteAsync()
        {
            var statusId = this.IsRetweet ? this.RetweetedId : this.StatusId;

            await this.twitter.Api.FavoritesDestroy(statusId)
                .IgnoreResponse()
                .ConfigureAwait(false);

            this.IsFav = false;
        }

        public override Task<PostClass?> RetweetAsync(SettingCommon settingCommon)
        {
            var statusId = this.IsRetweet ? this.RetweetedId : this.StatusId;

            var read = !settingCommon.UnreadManage || settingCommon.Read;

            return this.twitter.PostRetweet(statusId, read);
        }

        public override Task DeleteAsync()
        {
            if (this.IsRetweet && this.RetweetedByUserId == this.twitter.UserId)
            {
                // 自分が RT したツイート (自分が RT した自分のツイートも含む)
                //   => RT を取り消し
                return this.twitter.Api.StatusesDestroy(this.StatusId)
                    .IgnoreResponse();
            }
            else
            {
                if (this.UserId != this.twitter.UserId)
                    throw new InvalidOperationException();

                if (this.IsRetweet)
                {
                    // 他人に RT された自分のツイート
                    //   => RT 元の自分のツイートを削除
                    return this.twitter.Api.StatusesDestroy(this.RetweetedId)
                        .IgnoreResponse();
                }
                else
                {
                    // 自分のツイート
                    //   => ツイートを削除
                    return this.twitter.Api.StatusesDestroy(this.StatusId)
                        .IgnoreResponse();
                }
            }
        }
    }
}
