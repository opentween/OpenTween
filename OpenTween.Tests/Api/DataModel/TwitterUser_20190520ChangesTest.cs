// OpenTween - Client of Twitter
// Copyright (c) 2019 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using Xunit;

namespace OpenTween.Api.DataModel
{
    public class TwitterUser_20190520ChangesTest
    {
        [Fact]
        public void ParseJsonTest()
        {
            // 2019-05-20 に予定されている User object の仕様変更テスト
            //  (廃止されるフィールドに null がセットされる)
            // https://twittercommunity.com/t/124732

            var json = """
                {
                    "id": 6253282,
                    "id_str": "6253282",
                    "name": "Twitter API",
                    "screen_name": "TwitterAPI",
                    "location": "San Francisco, CA",
                    "profile_location": null,
                    "description": "The Real Twitter API. Tweets about API changes, service issues and our Developer Platform. Don't get an answer? It's on my website.",
                    "url": "https:\/\/t.co\/8IkCzCDr19",
                    "protected": false,
                    "followers_count": 6133601,
                    "friends_count": 12,
                    "listed_count": 12935,
                    "created_at": "Wed May 23 06:01:13 +0000 2007",
                    "favourites_count": 31,
                    "utc_offset": null,
                    "time_zone": null,
                    "geo_enabled": null,
                    "verified": true,
                    "statuses_count": 3657,
                    "lang": null,
                    "contributors_enabled": null,
                    "is_translator": null,
                    "is_translation_enabled": null,
                    "profile_background_color": null,
                    "profile_background_image_url": null,
                    "profile_background_image_url_https": null,
                    "profile_background_tile": null,
                    "profile_image_url": null,
                    "profile_image_url_https": "https:\/\/pbs.twimg.com\/profile_images\/942858479592554497\/BbazLO9L_normal.jpg",
                    "profile_banner_url": "https:\/\/pbs.twimg.com\/profile_banners\/6253282\/1497491515",
                    "profile_image_extensions_alt_text": null,
                    "profile_banner_extensions_alt_text": null,
                    "profile_link_color": null,
                    "profile_sidebar_border_color": null,
                    "profile_sidebar_fill_color": null,
                    "profile_text_color": null,
                    "profile_use_background_image": null,
                    "has_extended_profile": null,
                    "default_profile": false,
                    "default_profile_image": false,
                    "following": null,
                    "follow_request_sent": null,
                    "notifications": null,
                    "translator_type": null
                }
                """;
            TwitterUser.ParseJson(json);
        }
    }
}
