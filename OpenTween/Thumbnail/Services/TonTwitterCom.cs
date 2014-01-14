// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OpenTween.Thumbnail.Services
{
    /// <summary>
    /// Twitter の DM に添付された画像をサムネイル表示するためのクラス
    /// </summary>
    class TonTwitterCom : IThumbnailService
    {
        /// <summary>
        /// OAuth のトークン等を設定させるためのデリゲート
        /// </summary>
        internal static Action<HttpConnectionOAuth> InitializeOAuthToken;

        public override ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
        {
            if (InitializeOAuthToken == null)
                return null;

            if (!url.StartsWith(@"https://ton.twitter.com/1.1/ton/data/"))
                return null;

            return new TonTwitterCom.Thumbnail
            {
                ImageUrl = url,
                ThumbnailUrl = url,
                TooltipText = null,
                FullSizeImageUrl = url,
            };
        }

        public class Thumbnail : ThumbnailInfo
        {
            public override Task<MemoryImage> LoadThumbnailImageAsync(CancellationToken token)
            {
                return Task.Factory.StartNew(() =>
                {
                    var oauth = new HttpOAuthApiProxy();
                    TonTwitterCom.InitializeOAuthToken(oauth);

                    Stream response = null;
                    var statusCode = oauth.GetContent("GET", new Uri(this.ThumbnailUrl), null, ref response, MyCommon.GetUserAgentString());

                    using (response)
                    {
                        if (statusCode == HttpStatusCode.OK)
                            return MemoryImage.CopyFromStream(response);
                        else
                            throw new WebException(statusCode.ToString(), WebExceptionStatus.ProtocolError);
                    }
                },
                token, TaskCreationOptions.None, TaskScheduler.Default); // 明示しないと TaskScheduler.Current になり UI スレッド上で実行されてしまう
            }
        }
    }
}
