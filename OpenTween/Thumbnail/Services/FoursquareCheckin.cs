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
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Connection;

namespace OpenTween.Thumbnail.Services
{
    class FoursquareCheckin : IThumbnailService
    {
        public static readonly Regex UrlPatternRegex =
            new Regex(@"^https?://(?:foursquare\.com|www\.swarmapp\.com)/.+?/checkin/(?<checkin_id>[0-9a-z]+)(?:\?s=(?<signature>[^&]+))?");

        public static readonly string ApiBase = "https://api.foursquare.com/v2";

        protected HttpClient http
        {
            get { return this.localHttpClient ?? Networking.Http; }
        }
        private readonly HttpClient localHttpClient;

        public FoursquareCheckin()
            : this(null)
        {
        }

        public FoursquareCheckin(HttpClient http)
        {
            this.localHttpClient = http;
        }

        public override async Task<ThumbnailInfo> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            // ツイートに位置情報が付与されている場合は何もしない
            if (post.PostGeo.Lat != 0 || post.PostGeo.Lng != 0)
                return null;

            var match = UrlPatternRegex.Match(url);

            if (!match.Success)
                return null;

            var checkinIdGroup = match.Groups["checkin_id"];
            var signatureGroup = match.Groups["signature"];

            try
            {
                // Foursquare のベニュー情報を取得
                // 参照: https://developer.foursquare.com/docs/venues/venues

                var query = new Dictionary<string, string>
                {
                    {"client_id", ApplicationSettings.FoursquareClientId},
                    {"client_secret", ApplicationSettings.FoursquareClientSecret},
                    {"v", "20140419"}, // https://developer.foursquare.com/overview/versioning
                };

                if (signatureGroup.Success)
                    query["signature"] = signatureGroup.Value;

                var apiUrl = new Uri(ApiBase + "/checkins/" + checkinIdGroup.Value + "?" + MyCommon.BuildQueryString(query));

                using (var response = await this.http.GetAsync(apiUrl, token).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    var jsonBytes = await response.Content.ReadAsByteArrayAsync()
                        .ConfigureAwait(false);

                    var location = ParseIntoLocation(jsonBytes);
                    if (location == null)
                        return null;

                    var map = MapThumb.GetDefaultInstance();

                    return new ThumbnailInfo
                    {
                        ImageUrl = map.CreateMapLinkUrl(location.Latitude, location.Longitude),
                        ThumbnailUrl = map.CreateStaticMapUrl(location.Latitude, location.Longitude),
                        TooltipText = null,
                    };
                }
            }
            catch (HttpRequestException) { }

            return null;
        }

        internal static GlobalLocation ParseIntoLocation(byte[] jsonBytes)
        {
            using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(jsonBytes, XmlDictionaryReaderQuotas.Max))
            {
                var xElm = XElement.Load(jsonReader);

                var locationElm = xElm.XPathSelectElement("/response/checkin/venue/location");

                // 座標が得られなかった場合
                if (locationElm == null)
                    return null;

                // 月など、地球以外の星の座標である場合
                var planetElm = locationElm.Element("planet");
                if (planetElm != null && planetElm.Value != "earth")
                    return null;

                return new GlobalLocation
                {
                    Latitude = double.Parse(locationElm.Element("lat").Value),
                    Longitude = double.Parse(locationElm.Element("lng").Value),
                };
            }
        }
    }
}
