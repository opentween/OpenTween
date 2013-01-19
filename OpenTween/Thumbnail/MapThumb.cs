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
using System.Text;

namespace OpenTween.Thumbnail
{
    public class GlobalLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocateInfo { get; set; }
    }

    public enum MapProvider
    {
        OpenStreetMap,
        GoogleMaps,
    }

    abstract class MapThumb
    {
        public string CreateStaticMapUrl(GlobalLocation location)
        {
            return CreateStaticMapUrl(location.Latitude, location.Longitude);
        }

        public abstract string CreateStaticMapUrl(double latitude, double longitude);

        public string CreateMapLinkUrl(GlobalLocation location)
        {
            return CreateMapLinkUrl(location.Latitude, location.Longitude);
        }

        public abstract string CreateMapLinkUrl(double latitude, double longitude);

        private static MapThumb defaultInstance = null;

        public static MapThumb GetDefaultInstance()
        {
            Type classType;

            MapProvider confValue = AppendSettingDialog.Instance.MapThumbnailProvider;
            switch (confValue)
            {
                case MapProvider.OpenStreetMap:
                    classType = typeof(MapThumbOSM);
                    break;
                case MapProvider.GoogleMaps:
                    classType = typeof(MapThumbGoogle);
                    break;
                default:
                    throw new NotSupportedException("Map Provider '" + confValue + "' is not supported.");
            }

            if (MapThumb.defaultInstance == null || MapThumb.defaultInstance.GetType() != classType)
            {
                MapThumb.defaultInstance = Activator.CreateInstance(classType) as MapThumb;
            }

            return MapThumb.defaultInstance;
        }
    }
}
