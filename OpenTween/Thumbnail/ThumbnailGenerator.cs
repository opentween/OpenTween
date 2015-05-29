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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Thumbnail.Services;

namespace OpenTween.Thumbnail
{
    class ThumbnailGenerator
    {
        public static List<IThumbnailService> Services { get; protected set; }

        internal static ImgAzyobuziNet ImgAzyobuziNetInstance { get; private set; }

        static ThumbnailGenerator()
        {
            ThumbnailGenerator.Services = new List<IThumbnailService>();
        }

        public static void InitializeGenerator()
        {
            ImgAzyobuziNetInstance = new ImgAzyobuziNet(autoupdate: true);

            ThumbnailGenerator.Services = new List<IThumbnailService>()
            {
                // ton.twitter.com
                new TonTwitterCom(),

                // twitter.com/tweet_video
                new TwitterComVideo(),

                // pic.twitter.com
                new SimpleThumbnailService(
                    @"^https?://pbs\.twimg\.com/.*$",
                    "${0}",
                    "${0}:orig"),

                // youtube
                new Youtube(),

                // ニコニコ動画
                new Nicovideo(),

                // vimeo
                new Vimeo(),

                // DirectLink
                new SimpleThumbnailService(@"^https?://.*(\.jpg|\.jpeg|\.gif|\.png|\.bmp)$", "${0}"),

                // img.azyobuzi.net
                ImgAzyobuziNetInstance,

                // ImgUr
                new SimpleThumbnailService(
                    @"^https?://(?:i\.)?imgur\.com/(\w+)(?:\..{3})?$",
                    "http://img.imgur.com/${1}l.jpg",
                    "http://img.imgur.com/${1}.jpg"),

                // Twitpic
                new SimpleThumbnailService(
                    @"^http://(www\.)?twitpic\.com/(?<photoId>\w+)(/full/?)?$",
                    "http://twitpic.com/show/thumb/${photoId}",
                    "http://twitpic.com/show/large/${photoId}"),

                // yfrog
                new SimpleThumbnailService(
                    @"^https?://yfrog\.com/(\w+)$",
                    "${0}:small",
                    "${0}"),

                // Lockerz
                new SimpleThumbnailService(
                    @"^https?://(tweetphoto\.com/[0-9]+|pic\.gd/[a-z0-9]+|(lockerz|plixi)\.com/[ps]/[0-9]+)$",
                    "http://api.plixi.com/api/tpapi.svc/imagefromurl?size=thumbnail&url=${0}",
                    "http://api.plixi.com/api/tpapi.svc/imagefromurl?size=big&url=${0}"),

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

                // PhotoShare
                new SimpleThumbnailService(@"^https?://(?:www\.)?bcphotoshare\.com/photos/\d+/(\d+)$", "http://images.bcphotoshare.com/storages/${1}/thumb180.jpg"),

                // img.ly
                new SimpleThumbnailService(@"^https?://img\.ly/(\w+)$",
                    "http://img.ly/show/thumb/${1}",
                    "http://img.ly/show/full/${1}"),

                // Twitgoo
                new SimpleThumbnailService(@"^https?://twitgoo\.com/(\w+)$",
                    "http://twitgoo.com/${1}/mini",
                    "http://twitgoo.com/${1}/img"),

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
                    "http://photozou.jp/p/thumb/${photoId}",
                    "http://photozou.jp/p/img/${photoId}"),

                // Piapro
                new MetaThumbnailService(@"^https?://piapro\.jp/(?:content/[0-9a-z]+|t/[0-9a-zA-Z_\-]+)$"),

                // Tumblr
                new Tumblr(),

                // ついっぷるフォト
                new SimpleThumbnailService(@"^https?://p\.twipple\.jp/(?<contentId>[0-9a-z]+)", "http://p.twipple.jp/show/large/${contentId}"),

                // mypix/shamoji
                new SimpleThumbnailService(@"^https?://www\.(mypix\.jp|shamoji\.info)/app\.php/picture/(?<contentId>[0-9a-z]+)", "${0}/thumb.jpg"),

                // ow.ly
                new SimpleThumbnailService(@"^https?://ow\.ly/i/(\w+)$", "http://static.ow.ly/photos/thumb/${1}.jpg"),

                // cloudfiles
                new SimpleThumbnailService(@"^https?://c[0-9]+\.cdn[0-9]+\.cloudfiles\.rackspacecloud\.com/[a-z_0-9]+", "${0}"),

                // Instagram
                new SimpleThumbnailService(
                    @"^https?://(?:instagram.com|instagr\.am|i\.instagram\.com)/p/.+/",
                    "${0}media/?size=m",
                    "${0}media/?size=l"),

                // pikubo
                new SimpleThumbnailService(
                    @"^https?://pikubo\.me/([a-z0-9-_]+)",
                    "http://pikubo.me/q/${1}",
                    "http://pikubo.me/l/${1}"),

                // Foursquare
                new FoursquareCheckin(),

                // TINAMI
                new Tinami(),

                // TwitrPix
                new SimpleThumbnailService(
                    @"^https?://twitrpix\.com/(\w+)$",
                    "http://img.twitrpix.com/thumb/${1}",
                    "http://img.twitrpix.com/${1}"),

                // Pckles
                new SimpleThumbnailService(
                    @"^https?://pckles\.com/\w+/\w+$",
                    "${0}.resize.jpg",
                    "${0}.jpg"),

                // via.me
                new ViaMe(),

                // tuna.be
                new SimpleThumbnailService(@"^https?://tuna\.be/t/(?<entryId>[a-zA-Z0-9\.\-_]+)$", "http://tuna.be/show/thumb/${entryId}"),

                // Path (path.com)
                new MetaThumbnailService(@"^https?://path.com/p/\w+$"),

                // GIFMAGAZINE
                new SimpleThumbnailService(@"^https?://gifmagazine\.net/post_images/(\d+)", "http://img.gifmagazine.net/gifmagazine/images/${1}/original.gif"),

                // SoundCloud
                new MetaThumbnailService(@"^https?://soundcloud.com/[\w-]+/[\w-]+$"),
            };
        }

        public static async Task<IEnumerable<ThumbnailInfo>> GetThumbnailsAsync(PostClass post, CancellationToken token)
        {
            var thumbnails = new List<ThumbnailInfo>();

            foreach (var media in post.Media)
            {
                var thumbInfo = await ThumbnailGenerator.GetThumbnailInfoAsync(media.Url, post, token)
                    .ConfigureAwait(false);

                if (thumbInfo != null)
                    thumbnails.Add(thumbInfo);

                token.ThrowIfCancellationRequested();
            }

            if (post.PostGeo != null && !(post.PostGeo.Lat == 0 && post.PostGeo.Lng == 0))
            {
                var map = MapThumb.GetDefaultInstance();
                thumbnails.Add(new ThumbnailInfo()
                {
                    ImageUrl = map.CreateMapLinkUrl(post.PostGeo.Lat, post.PostGeo.Lng),
                    ThumbnailUrl = map.CreateStaticMapUrl(post.PostGeo.Lat, post.PostGeo.Lng),
                    TooltipText = null,
                });
            }

            return thumbnails;
        }

        public static async Task<ThumbnailInfo> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            foreach (var generator in ThumbnailGenerator.Services)
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
