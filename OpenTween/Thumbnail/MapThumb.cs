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
using System.Threading.Tasks;
using OpenTween.Models;
using OpenTween.Setting;
using OpenTween.Thumbnail.Services;

namespace OpenTween.Thumbnail
{
    public abstract class MapThumb
    {
        public abstract Task<ThumbnailInfo> GetThumbnailInfoAsync(PostClass.StatusGeo geo);

        private static MapThumb defaultInstance = null!;

        public static MapThumb GetDefaultInstance()
        {
            var confValue = SettingManager.Instance.Common.MapThumbnailProvider;
            var classType = confValue switch
            {
                MapProvider.OpenStreetMap => typeof(MapThumbOSM),
                MapProvider.GoogleMaps => typeof(MapThumbGoogle),
                _ => throw new NotSupportedException($"Map Provider '{confValue}' is not supported."),
            };

            if (MapThumb.defaultInstance == null || MapThumb.defaultInstance.GetType() != classType)
            {
                MapThumb.defaultInstance = (MapThumb)Activator.CreateInstance(classType);
            }

            return MapThumb.defaultInstance;
        }
    }

    public record GlobalLocation(
        double Latitude,
        double Longitude
    );

    public enum MapProvider
    {
        OpenStreetMap,
        GoogleMaps,
    }
}
