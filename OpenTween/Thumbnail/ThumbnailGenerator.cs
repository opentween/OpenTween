// OpenTween - Client of Twitter
// Copyright (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;
using System.Text.RegularExpressions;
using OpenTween.Thumbnail.Services;

namespace OpenTween.Thumbnail
{
    class ThumbnailGenerator
    {
        private static List<IThumbnailService> generator = new List<IThumbnailService>();

        public static void InitializeGenerator()
        {
            ThumbnailGenerator.generator = new List<IThumbnailService>()
            {
                // DirectLink
                new SimpleThumbnailService(@"^https?://.*(\.jpg|\.jpeg|\.gif|\.png|\.bmp)$", "${0}"),

                // img.azyobuzi.net
                new ImgAzyobuziNet(),

                // ImgUr
                new SimpleThumbnailService(@"^http://(?:i\.)?imgur\.com/(\w+)(?:\..{3})?$", "http://img.imgur.com/${1}l.jpg"),

                // Twitpic
                new SimpleThumbnailService(@"^http://(www\.)?twitpic\.com/(?<photoId>\w+)(/full/?)?$", "http://twitpic.com/show/thumb/${photoId}"),

                // yfrog
                new SimpleThumbnailService(@"^http://yfrog\.com/(\w+)$", "${0}:small"),

                // Lockerz
                new SimpleThumbnailService(@"^http://(tweetphoto\.com/[0-9]+|pic\.gd/[a-z0-9]+|(lockerz|plixi)\.com/[ps]/[0-9]+)$", "http://api.plixi.com/api/tpapi.svc/imagefromurl?size=thumbnail&url=${0}"),

                // MobyPicture
                new SimpleThumbnailService(@"^http://moby\.to/(\w+)$", "http://mobypicture.com/?${1}:small"),

                // 携帯百景
                new SimpleThumbnailService(@"^http://movapic\.com/pic/(\w+)$", "http://image.movapic.com/pic/s_${1}.jpeg"),

                // はてなフォトライフ
                new SimpleThumbnailService(@"^http://f\.hatena\.ne\.jp/(([a-z])[a-z0-9_-]{1,30}[a-z0-9])/((\d{8})\d+)$", "http://img.f.hatena.ne.jp/images/fotolife/${2}/${1}/${4}/${3}_120.jpg"),

                // PhotoShare
                new SimpleThumbnailService(@"^http://(?:www\.)?bcphotoshare\.com/photos/\d+/(\d+)$", "http://images.bcphotoshare.com/storages/${1}/thumb180.jpg"),

                // PhotoShare
                new PhotoShareShortlink(@"^http://bctiny\.com/p(\w+)$"),

                // img.ly
                new SimpleThumbnailService(@"^http://img\.ly/(\w+)$", "http://img.ly/show/thumb/${1}"),

                // Twitgoo
                new SimpleThumbnailService(@"^http://twitgoo\.com/(\w+)$", "http://twitgoo.com/${1}/mini"),

                // youtube
                new Youtube(@"^http://(?:(www\.youtube\.com)|(youtu\.be))/(watch\?v=)?(?<videoid>([\w\-]+))", "http://i.ytimg.com/vi/${videoid}/default.jpg"),

                // ニコニコ動画
                new Nicovideo(@"^http://(?:(www|ext)\.nicovideo\.jp/watch|nico\.ms)/(?:sm|nm)?([0-9]+)(\?.+)?$", "http://www.nicovideo.jp/api/getthumbinfo/${id}"),

                // ニコニコ静画
                new SimpleThumbnailService(@"^http://(?:seiga\.nicovideo\.jp/seiga/|nico\.ms/)im(?<id>\d+)", "http://lohas.nicoseiga.jp/thumb/${id}q?"),

                // pixiv
                new MetaThumbnailService(@"^http://www\.pixiv\.net/(member_illust|index)\.php\?(?=.*mode=(medium|big))(?=.*illust_id=(?<illustId>[0-9]+)).*$"),

                // flickr
                new MetaThumbnailService(@"^http://www\.flickr\.com/.+$"),

                // フォト蔵
                new SimpleThumbnailService(@"^http://photozou\.jp/photo/show/(?<userId>[0-9]+)/(?<photoId>[0-9]+)", "http://photozou.jp/p/thumb/${photoId}"),

                // TwitVideo
                new SimpleThumbnailService(@"^http://twitvideo\.jp/(\w+)$", "http://twitvideo.jp/img/thumb/${1}"),

                // Piapro
                new MetaThumbnailService(@"^http://piapro\.jp/(?:content/[0-9a-z]+|t/[0-9a-zA-Z_\-]+)$"),

                // Tumblr
                new Tumblr(@"(?<base>http://.+?\.tumblr\.com/)post/(?<postID>[0-9]+)(/(?<subject>.+?)/)?", "${base}api/read?id=${postID}"),

                // ついっぷるフォト
                new SimpleThumbnailService(@"^http://p\.twipple\.jp/(?<contentId>[0-9a-z]+)", "http://p.twipple.jp/show/large/${contentId}"),

                // mypix/shamoji
                new SimpleThumbnailService(@"^http://www\.(mypix\.jp|shamoji\.info)/app\.php/picture/(?<contentId>[0-9a-z]+)", "${0}/thumb.jpg"),

                // ow.ly
                new SimpleThumbnailService(@"^http://ow\.ly/i/(\w+)$", "http://static.ow.ly/photos/thumb/${1}.jpg"),

                // vimeo
                new Vimeo(@"http://vimeo\.com/(?<postID>[0-9]+)", "http://vimeo.com/api/oembed.xml?url=${0}"),

                // cloudfiles
                new SimpleThumbnailService(@"^http://c[0-9]+\.cdn[0-9]+\.cloudfiles\.rackspacecloud\.com/[a-z_0-9]+", "${0}"),

                // Instagram
                new SimpleThumbnailService(@"^http://instagr\.am/p/.+/", "${0}media/?size=m"),

                // pikubo
                new SimpleThumbnailService(@"^http://pikubo\.me/([a-z0-9-_]+)", "http://pikubo.me/q/${1}"),

                // Foursquare
                new Services.Foursquare(@"^https?://(4sq|foursquare)\.com/.+"),

                // TINAMI
                new Tinami(@"^http://www\.tinami\.com/view/(?<ContentId>\d+)$", "http://api.tinami.com/content/info?cont_id=${ContentId}&api_key=" + ApplicationSettings.TINAMIApiKey),

                // pic.twitter.com
                new SimpleThumbnailService(@"^https?://p\.twimg\.com/.*$", "${0}:thumb"),

                // TwitrPix
                new SimpleThumbnailService(@"^http://twitrpix\.com/(\w+)$", "http://img.twitrpix.com/thumb/${1}"),

                // Pckles
                new SimpleThumbnailService(@"^https?://pckles\.com/\w+/\w+$", "${0}.resize.jpg"),

                // via.me
                new ViaMe(@"^https?://via\.me/-(\w+)$", "http://via.me/api/v1/posts/$1"),

                // tuna.be
                new SimpleThumbnailService(@"^http://tuna\.be/t/(?<entryId>[a-zA-Z0-9\.\-_]+)$", "http://tuna.be/show/thumb/${entryId}"),

                // Path (path.com)
                new MetaThumbnailService(@"^https?://path.com/p/\w+$"),
            };
        }

        public static List<ThumbnailInfo> GetThumbnails(PostClass post)
        {
            var thumbnails = new List<ThumbnailInfo>();

            if (post.Media != null)
            {
                foreach (var media in post.Media)
                {
                    var thumbInfo = ThumbnailGenerator.GetThumbnailInfo(media.Value, post);
                    if (thumbInfo != null)
                    {
                        thumbnails.Add(thumbInfo);
                    }
                }
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

        public static ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
        {
            foreach (var generator in ThumbnailGenerator.generator)
            {
                var result = generator.GetThumbnailInfo(url, post);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
