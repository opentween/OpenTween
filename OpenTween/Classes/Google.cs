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
using System.Runtime.Serialization;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web;

namespace OpenTween
{
    public class Google
    {
#region "Translation"
        // http://code.google.com/intl/ja/apis/ajaxlanguage/documentation/#fonje
        // デベロッパー ガイド - Google AJAX Language API - Google Code

        private const string TranslateEndPoint = "http://ajax.googleapis.com/ajax/services/language/translate";
        private const string LanguageDetectEndPoint = "https://ajax.googleapis.com/ajax/services/language/detect";

#region "言語テーブル定義"
        private static List<string> LanguageTable = new List<string>
        {
            "af",
            "sq",
            "am",
            "ar",
            "hy",
            "az",
            "eu",
            "be",
            "bn",
            "bh",
            "br",
            "bg",
            "my",
            "ca",
            "chr",
            "zh",
            "zh-CN",
            "zh-TW",
            "co",
            "hr",
            "cs",
            "da",
            "dv",
            "nl",
            "en",
            "eo",
            "et",
            "fo",
            "tl",
            "fi",
            "fr",
            "fy",
            "gl",
            "ka",
            "de",
            "el",
            "gu",
            "ht",
            "iw",
            "hi",
            "hu",
            "is",
            "id",
            "iu",
            "ga",
            "it",
            "ja",
            "jw",
            "kn",
            "kk",
            "km",
            "ko",
            "ku",
            "ky",
            "lo",
            "la",
            "lv",
            "lt",
            "lb",
            "mk",
            "ms",
            "ml",
            "mt",
            "mi",
            "mr",
            "mn",
            "ne",
            "no",
            "oc",
            "or",
            "ps",
            "fa",
            "pl",
            "pt",
            "pt-PT",
            "pa",
            "qu",
            "ro",
            "ru",
            "sa",
            "gd",
            "sr",
            "sd",
            "si",
            "sk",
            "sl",
            "es",
            "su",
            "sw",
            "sv",
            "syr",
            "tg",
            "ta",
            "tt",
            "te",
            "th",
            "bo",
            "to",
            "tr",
            "uk",
            "ur",
            "uz",
            "ug",
            "vi",
            "cy",
            "yi",
            "yo",
        };
#endregion

        [DataContract]
        public class TranslateResponseData
        {
            [DataMember(Name = "translatedText")] public string TranslatedText;
        }


        [DataContract]
        private class TranslateResponse
        {
            [DataMember(Name = "responseData")] public TranslateResponseData ResponseData = null;
            [DataMember(Name = "responseDetails")] public string ResponseDetails = null;
            [DataMember(Name = "responseStatus")] public HttpStatusCode ResponseStatus = 0;
        }


        [DataContract]
        public class LanguageDetectResponseData
        {
            [DataMember(Name = "language")] public string Language;
            [DataMember(Name = "isReliable")] public bool IsReliable;
            [DataMember(Name = "confidence")] public double Confidence;
        }

        [DataContract]
        private class LanguageDetectResponse
        {
            [DataMember(Name = "responseData")] public LanguageDetectResponseData ResponseData = null;
            [DataMember(Name = "responseDetails")] public string ResponseDetails = null;
            [DataMember(Name = "responseStatus")] public HttpStatusCode ResponseStatus = 0;
        }

        public bool Translate(string srclng, string dstlng, string source, ref string destination, ref string ErrMsg)
        {
            var http = new HttpVarious();
            var apiurl = TranslateEndPoint;
            var headers = new Dictionary<string, string>();
            headers.Add("v", "1.0");

            ErrMsg = "";
            if (string.IsNullOrEmpty(srclng) || string.IsNullOrEmpty(dstlng))
            {
                return false;
            }
            headers.Add("User-Agent", MyCommon.GetUserAgentString());
            headers.Add("langpair", srclng + "|" + dstlng);

            headers.Add("q", source);

            var content = "";
            if (http.GetData(apiurl, headers, out content))
            {
                var serializer = new DataContractJsonSerializer(typeof(TranslateResponse));
                TranslateResponse res;

                try
                {
                    res = MyCommon.CreateDataFromJson<TranslateResponse>(content);
                }
                catch(Exception)
                {
                    ErrMsg = "Err:Invalid JSON";
                    return false;
                }

                if (res.ResponseData == null)
                {
                    ErrMsg = "Err:" + res.ResponseDetails;
                    return false;
                }
                var _body = res.ResponseData.TranslatedText;
                var buf = HttpUtility.UrlDecode(_body);

                destination = string.Copy(buf);
                return true;
            }
            return false;
        }

        public string LanguageDetect(string source)
        {
            var http = new HttpVarious();
            var apiurl = LanguageDetectEndPoint;
            var headers = new Dictionary<string, string>();
            headers.Add("User-Agent", MyCommon.GetUserAgentString());
            headers.Add("v", "1.0");
            headers.Add("q", source);
            var content = "";
            if (http.GetData(apiurl, headers, out content))
            {
                var serializer = new DataContractJsonSerializer(typeof(LanguageDetectResponse));
                try
                {
                    var res = MyCommon.CreateDataFromJson<LanguageDetectResponse>(content);
                    return res.ResponseData.Language;
                }
                catch(Exception)
                {
                    return "";
                }
            }
            return "";
        }

        public string GetLanguageEnumFromIndex(int index)
        {
            return LanguageTable[index];
        }

        public int GetIndexFromLanguageEnum(string lang)
        {
            return LanguageTable.IndexOf(lang);
        }
#endregion

#region "UrlShortener"
        // http://code.google.com/intl/ja/apis/urlshortener/v1/getting_started.html
        // Google URL Shortener API

        [DataContract]
        private class UrlShortenerParameter
        {
            [DataMember(Name = "longUrl")] public string LongUrl = null;
        }

        [DataContract]
        private class UrlShortenerResponse
        {

        }

        public string Shorten(string source)
        {
            var http = new HttpVarious();
            var apiurl = "https://www.googleapis.com/urlshortener/v1/url";
            var headers = new Dictionary<string, string>();
            headers.Add("User-Agent", MyCommon.GetUserAgentString());
            headers.Add("Content-Type", "application/json");

            http.PostData(apiurl, headers);
            return "";
        }
#endregion

#region "GoogleMaps"
        public string CreateGoogleStaticMapsUri(GlobalLocation locate)
        {
            return CreateGoogleStaticMapsUri(locate.Latitude, locate.Longitude);
        }

        public string CreateGoogleStaticMapsUri(double lat, double lng)
        {
            int width = AppendSettingDialog.Instance.FoursquarePreviewWidth;
            int height = AppendSettingDialog.Instance.FoursquarePreviewHeight;
            int zoom = AppendSettingDialog.Instance.FoursquarePreviewZoom;
            string location = lat.ToString() + "," + lng.ToString();

            return "http://maps.google.com/maps/api/staticmap?center=" + location + "&size=" + width + "x" + height + "&zoom=" + zoom + "&markers=" + location + "&sensor=false";
        }

        public string CreateGoogleMapsUri(GlobalLocation locate)
        {
            return CreateGoogleMapsUri(locate.Latitude, locate.Longitude);
        }

        public string CreateGoogleMapsUri(double lat, double lng)
        {
            int zoom = AppendSettingDialog.Instance.FoursquarePreviewZoom;
            string location = lat.ToString() + "," + lng.ToString();

            return "http://maps.google.com/maps?ll=" + location + "&z=" + zoom + "&q=" + location;
        }

        public class GlobalLocation
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string LocateInfo { get; set; }
        }

#endregion
    }
}
