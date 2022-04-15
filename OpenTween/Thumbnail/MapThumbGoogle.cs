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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Models;
using OpenTween.Setting;

namespace OpenTween.Thumbnail
{
    public class MapThumbGoogle : MapThumb
    {
        public override Task<ThumbnailInfo> GetThumbnailInfoAsync(PostClass.StatusGeo geo)
        {
            var thumb = new ThumbnailInfo
            {
                MediaPageUrl = this.CreateMapLinkUrl(geo.Latitude, geo.Longitude),
                ThumbnailImageUrl = this.CreateStaticMapUrl(geo.Latitude, geo.Longitude),
                TooltipText = null,
            };

            return Task.FromResult(thumb);
        }

        public string CreateStaticMapUrl(double latitude, double longitude)
        {
            var width = SettingManager.Instance.Common.MapThumbnailWidth; // この辺なんとかならんかなあ
            var height = SettingManager.Instance.Common.MapThumbnailHeight;
            var zoom = SettingManager.Instance.Common.MapThumbnailZoom;
            var location = latitude + "," + longitude;

            var baseUrl = "https://maps.googleapis.com/maps/api/staticmap";

            return baseUrl + "?center=" + location + "&size=" + width + "x" + height + "&zoom=" + zoom + "&markers=" + location + "&sensor=false";
        }

        public string CreateMapLinkUrl(double latitude, double longitude)
        {
            var zoom = SettingManager.Instance.Common.MapThumbnailZoom;
            var location = latitude + "," + longitude;

            var baseUrl = "https://maps.google.co.jp/maps";

            return baseUrl + "?ll=" + location + "&z=" + zoom + "&q=" + location;
        }
    }
}
