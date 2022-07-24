// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Models;
using OpenTween.Thumbnail.Services;

namespace OpenTween.Thumbnail
{
    public sealed class ThumbnailGenerator
    {
        public static readonly Regex InstagramPattern = new(
            @"^https?://(?:instagram.com|instagr\.am|i\.instagram\.com|www\.instagram\.com)/([^/]+/)?p/(?<mediaId>[^/]+)/(\?.*)?$",
            RegexOptions.IgnoreCase
        );

        public List<IThumbnailService> Services { get; }

        public ImgAzyobuziNet ImgAzyobuziNet { get; }

        public ThumbnailGenerator(ImgAzyobuziNet imgAzyobuziNet)
        {
            this.ImgAzyobuziNet = imgAzyobuziNet;

            this.Services = new List<IThumbnailService>
            {
                // ton.twitter.com
                new TonTwitterCom(),

                // twitter.com/tweet_video
                new TwitterComVideo(),

                // pic.twitter.com
                new PbsTwimgCom(),

                // youtube
                new Youtube(),

                // ニコニコ動画
                new Nicovideo(),

                // vimeo
                new Vimeo(),

                // DirectLink
                new SimpleThumbnailService(@"^https?://.*(\.jpg|\.jpeg|\.gif|\.png|\.bmp)$", "${0}"),

                // img.azyobuzi.net
                this.ImgAzyobuziNet,

                // ImgUr
                new SimpleThumbnailService(
                    @"^https?://(?:i\.)?imgur\.com/(\w+)(?:\..{3})?$",
                    "https://img.imgur.com/${1}l.jpg",
                    "https://img.imgur.com/${1}.jpg"),

                // Twitpic
                new MetaThumbnailService(@"^https?://(www\.)?twitpic\.com/.+$"),

                // MobyPicture
                new MetaThumbnailService(@"^https?://(?:www\.)?mobypicture.com/user/\w+/view/\d+$"),

                // 携帯百景
                new SimpleThumbnailService(
                    @"^https?://movapic\.com/pic/(\w+)$",
                    "http://image.movapic.com/pic/s_${1}.jpeg",
                    "http://image.movapic.com/pic/m_${1}.jpeg"),

                // はてなフォトライフ
                new SimpleThumbnailService(
                    @"^https?://f\.hatena\.ne\.jp/(([a-z])[a-z0-9_-]{1,30}[a-z0-9])/((\d{8})\d+)$",
                    "http://img.f.hatena.ne.jp/images/fotolife/${2}/${1}/${4}/${3}_120.jpg",
                    "http://img.f.hatena.ne.jp/images/fotolife/${2}/${1}/${4}/${3}.jpg"),

                // ニコニコ静画
                new SimpleThumbnailService(
                    @"^https?://(?:seiga\.nicovideo\.jp/seiga/|nico\.ms/)im(?<id>\d+)",
                    "http://lohas.nicoseiga.jp/thumb/${id}q?",
                    "http://lohas.nicoseiga.jp/thumb/${id}l?"),

                // pixiv
                new Pixiv(),

                // flickr
                new MetaThumbnailService(@"^https?://www\.flickr\.com/.+$"),

                // フォト蔵
                new SimpleThumbnailService(
                    @"^https?://photozou\.jp/photo/show/(?<userId>[0-9]+)/(?<photoId>[0-9]+)",
                    "https://photozou.jp/p/thumb/${photoId}",
                    "https://photozou.jp/p/img/${photoId}"),

                // Piapro
                new MetaThumbnailService(@"^https?://piapro\.jp/(?:content/[0-9a-z]+|t/[0-9a-zA-Z_\-]+)$"),

                // Tumblr
                new Tumblr(),

                // mypix/shamoji
                new SimpleThumbnailService(@"^https?://www\.(mypix\.jp|shamoji\.info)/app\.php/picture/(?<contentId>[0-9a-z]+)", "${0}/thumb.jpg"),

                // ow.ly
                new SimpleThumbnailService(@"^https?://ow\.ly/i/(\w+)$", "http://static.ow.ly/photos/thumb/${1}.jpg"),

                // cloudfiles
                new SimpleThumbnailService(@"^https?://c[0-9]+\.cdn[0-9]+\.cloudfiles\.rackspacecloud\.com/[a-z_0-9]+", "${0}"),

                // Instagram
                new SimpleThumbnailService(
                    InstagramPattern,
                    "https://www.instagram.com/p/${mediaId}/media/?size=m",
                    "https://www.instagram.com/p/${mediaId}/media/?size=l"),

                // Foursquare
                new FoursquareCheckin(),

                // TINAMI
                new Tinami(),

                // tuna.be
                new SimpleThumbnailService(@"^https?://tuna\.be/t/(?<entryId>[a-zA-Z0-9\.\-_]+)$", "https://tuna.be/show/thumb/${entryId}"),

                // GIFMAGAZINE
                new SimpleThumbnailService(@"^https?://gifmagazine\.net/post_images/(\d+)", "https://img.gifmagazine.net/gifmagazine/images/${1}/original.gif"),

                // SoundCloud
                new MetaThumbnailService(@"^https?://soundcloud.com/[\w-]+/[\w-]+$"),

                // Gyazo (参考: http://qiita.com/uiureo/items/9ea55b07dff28a322a9e)
                new SimpleThumbnailService(@"^https?://gyazo\.com/([a-zA-Z0-9]+)/?$", "https://gyazo.com/${1}/raw"),
            };
        }

        public async Task<IEnumerable<ThumbnailInfo>> GetThumbnailsAsync(PostClass post, CancellationToken token)
        {
            var thumbnails = new List<ThumbnailInfo>();

            var expandedUrls = Enumerable.Concat(
                post.GetExpandedUrls(), post.Media.Select(x => x.Url));

            foreach (var expandedUrl in expandedUrls)
            {
                var thumbInfo = await this.GetThumbnailInfoAsync(expandedUrl, post, token)
                    .ConfigureAwait(false);

                if (thumbInfo != null)
                    thumbnails.Add(thumbInfo);

                token.ThrowIfCancellationRequested();
            }

            if (post.PostGeo != null)
            {
                var map = MapThumb.GetDefaultInstance();
                var thumb = await map.GetThumbnailInfoAsync(post.PostGeo.Value)
                    .ConfigureAwait(false);
                thumbnails.Add(thumb);
            }

            return thumbnails;
        }

        public async Task<ThumbnailInfo?> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            foreach (var generator in this.Services)
            {
                var result = await generator.GetThumbnailInfoAsync(url, post, token)
                    .ConfigureAwait(false);

                if (result != null)
                    return result;

                token.ThrowIfCancellationRequested();
            }

            return null;
        }
    }
}
