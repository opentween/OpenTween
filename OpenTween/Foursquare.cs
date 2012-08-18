// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using OpenTween.Thumbnail;

namespace OpenTween
{
    public class Foursquare : HttpConnection
    {
        private static Foursquare _instance = new Foursquare();
        public static Foursquare GetInstance
        {
            get
            {
                return _instance;
            }
        }

        private Dictionary<string, string> _authKey = new Dictionary<string, string>
        {
            {"client_id", ApplicationSettings.FoursquareClientId},
            {"client_secret", ApplicationSettings.FoursquareClientSecret},
        };

        private Dictionary<string, GlobalLocation> CheckInUrlsVenueCollection = new Dictionary<string, GlobalLocation>();

        private string GetVenueId(string url)
        {
            var content = "";
            try
            {
                var res = GetContent("GET", new Uri(url), null, ref content);
                if (res != HttpStatusCode.OK) return "";
            }
            catch (Exception)
            {
                return "";
            }
            var mc = Regex.Match(content, "/venue/(?<venueId>[0-9]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                var vId = mc.Result("${venueId}");
                return vId;
            }
            else
            {
                return "";
            }
        }

        private FourSquareDataModel.Venue GetVenueInfo(string venueId)
        {
            var content = "";
            try
            {
                var res = GetContent("GET",
                                     new Uri("https://api.foursquare.com/v2/venues/" + venueId),
                                     _authKey,
                                     ref content);

                if (res == HttpStatusCode.OK)
                {
                    FourSquareDataModel.FourSquareData curData = null;
                    try
                    {
                        curData = MyCommon.CreateDataFromJson<FourSquareDataModel.FourSquareData>(content);
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                    return curData.Response.Venue;
                }
                else
                {
                    //FourSquareDataModel.FourSquareData curData = null;
                    //try
                    //{
                    //    curData = CreateDataFromJson<FourSquareDataModel.FourSquareData>(content);
                    //}
                    //catch (Exception)
                    //{
                    //    return null;;
                    //}
                    //MessageBox.Show(res.ToString() + Environment.NewLine + curData.Meta.ErrorType + Environment.NewLine + curData.Meta.ErrorDetail);
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetMapsUri(string url, ref string refText)
        {
            if (!AppendSettingDialog.Instance.IsPreviewFoursquare) return null;

            var urlId = Regex.Replace(url, @"https?://(4sq|foursquare)\.com/", "");

            if (CheckInUrlsVenueCollection.ContainsKey(urlId))
            {
                refText = CheckInUrlsVenueCollection[urlId].LocateInfo;
                return MapThumb.GetDefaultInstance().CreateStaticMapUrl(CheckInUrlsVenueCollection[urlId]);
            }

            FourSquareDataModel.Venue curVenue = null;
            var venueId = GetVenueId(url);
            if (string.IsNullOrEmpty(venueId)) return null;

            curVenue = GetVenueInfo(venueId);
            if (curVenue == null) return null;

            var curLocation = new GlobalLocation {Latitude = curVenue.Location.Latitude, Longitude = curVenue.Location.Longitude, LocateInfo = CreateVenueInfoText(curVenue)};
            //例外発生の場合があるため
            if (!CheckInUrlsVenueCollection.ContainsKey(urlId)) CheckInUrlsVenueCollection.Add(urlId, curLocation);
            refText = curLocation.LocateInfo;
            return MapThumb.GetDefaultInstance().CreateStaticMapUrl(curLocation);
        }

        private string CreateVenueInfoText(FourSquareDataModel.Venue info)
        {
            return info.Name + Environment.NewLine + info.Stats.UsersCount.ToString() + "/" + info.Stats.CheckinsCount.ToString() + Environment.NewLine + info.Location.Address + Environment.NewLine + info.Location.City + info.Location.State + Environment.NewLine + info.Location.Latitude.ToString() + Environment.NewLine + info.Location.Longitude.ToString();
        }
        public HttpStatusCode GetContent(string method,
                    Uri requestUri,
                    Dictionary<string, string> param,
                    ref string content)
        {
            var webReq  = CreateRequest(method,
                                        requestUri,
                                        param,
                                        false);
            var code = GetResponse(webReq, out content, null, false);
            return code;
        }

    #region "FourSquare DataModel"

        public class FourSquareDataModel
        {
            [DataContract]
            public class FourSquareData
            {
                [DataMember(Name = "meta", IsRequired = false)] public Meta Meta;
                [DataMember(Name = "response", IsRequired = false)] public Response Response;
            }

            [DataContract]
            public class Response
            {
                [DataMember(Name = "venue", IsRequired = false)] public Venue Venue;
            }

            [DataContract]
            public class Venue
            {
                [DataMember(Name = "id")] public string Id;
                [DataMember(Name = "name")] public string Name;
                [DataMember(Name = "location")] public Location Location;
                [DataMember(Name = "verified")] public bool Verified;
                [DataMember(Name = "stats")] public Stats Stats;
                [DataMember(Name = "mayor")] public Mayor Mayor;
                [DataMember(Name = "shortUrl")] public string ShortUrl;
                [DataMember(Name = "timeZone")] public string TimeZone;
            }

            [DataContract]
            public class Location
            {
                [DataMember(Name = "address")] public string Address;
                [DataMember(Name = "city")] public string City;
                [DataMember(Name = "state")] public string State;
                [DataMember(Name = "lat")] public double Latitude;
                [DataMember(Name = "lng")] public double Longitude;
            }

            [DataContract]
            public class Stats
            {
                [DataMember(Name = "checkinsCount")] public int CheckinsCount;
                [DataMember(Name = "usersCount")] public int UsersCount;
            }


            [DataContract]
            public class Mayor
            {
                [DataMember(Name = "count")] public int Count;
                [DataMember(Name = "user", IsRequired = false)] public FoursquareUser User;
            }

            [DataContract]
            public class FoursquareUser
            {
                [DataMember(Name = "id")] public int Id;
                [DataMember(Name = "firstName")] public string FirstName;
                [DataMember(Name = "photo")] public string Photo;
                [DataMember(Name = "gender")] public string Gender;
                [DataMember(Name = "homeCity")] public string HomeCity;
            }

            [DataContract]
            public class Meta
            {
                [DataMember(Name = "code")] public int Code;
                [DataMember(Name = "errorType", IsRequired = false)] public string ErrorType;
                [DataMember(Name = "errorDetail", IsRequired = false)] public string ErrorDetail;
            }
        }
    #endregion


    }
}
