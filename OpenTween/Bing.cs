// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using OpenTween.Connection;

namespace OpenTween
{
    public class Bing
    {
        private static readonly List<string> LanguageTable = new List<string>
        {
            "af",
            "sq",
            "ar-sa",
            "ar-iq",
            "ar-eg",
            "ar-ly",
            "ar-dz",
            "ar-ma",
            "ar-tn",
            "ar-om",
            "ar-ye",
            "ar-sy",
            "ar-jo",
            "ar-lb",
            "ar-kw",
            "ar-ae",
            "ar-bh",
            "ar-qa",
            "eu",
            "bg",
            "be",
            "ca",
            "zh-tw",
            "zh-cn",
            "zh-hk",
            "zh-sg",
            "hr",
            "cs",
            "da",
            "nl",
            "nl-be",
            "en",
            "en-us",
            "en-gb",
            "en-au",
            "en-ca",
            "en-nz",
            "en-ie",
            "en-za",
            "en-jm",
            "en",
            "en-bz",
            "en-tt",
            "et",
            "fo",
            "fa",
            "fi",
            "fr",
            "fr-be",
            "fr-ca",
            "fr-ch",
            "fr-lu",
            "gd",
            "ga",
            "de",
            "de-ch",
            "de-at",
            "de-lu",
            "de-li",
            "el",
            "he",
            "hi",
            "hu",
            "is",
            "id",
            "it",
            "it-ch",
            "ja",
            "ko",
            "ko",
            "lv",
            "lt",
            "mk",
            "ms",
            "mt",
            "no",
            "no",
            "pl",
            "pt-br",
            "pt",
            "rm",
            "ro",
            "ro-mo",
            "ru",
            "ru-mo",
            "sz",
            "sr",
            "sr",
            "sk",
            "sl",
            "sb",
            "es",
            "es-mx",
            "es-gt",
            "es-cr",
            "es-pa",
            "es-do",
            "es-ve",
            "es-co",
            "es-pe",
            "es-ar",
            "es-ec",
            "es-cl",
            "es-uy",
            "es-py",
            "es-bo",
            "es-sv",
            "es-hn",
            "es-ni",
            "es-pr",
            "sx",
            "sv",
            "sv-fi",
            "th",
            "ts",
            "tn",
            "tr",
            "uk",
            "ur",
            "ve",
            "vi",
            "xh",
            "ji",
            "zu",
        };

        private static readonly string TranslateUri =
            "https://api.datamarket.azure.com/Data.ashx/Bing/MicrosoftTranslator/v1/Translate";

        protected HttpClient http
        {
            get { return this.localHttpClient ?? Networking.Http; }
        }
        private readonly HttpClient localHttpClient;

        public Bing()
            : this(null)
        {
        }

        public Bing(HttpClient http)
        {
            this.localHttpClient = http;
        }

        /// <summary>
        /// Microsoft Translator API を使用した翻訳を非同期に行います
        /// </summary>
        /// <exception cref="HttpRequestException"/>
        public async Task<string> TranslateAsync(string text, string langFrom, string langTo)
        {
            var param = new Dictionary<string, string>
            {
                {"Text", "'" + text + "'"},
                {"To", "'" + langTo + "'"},
                {"$format", "Raw"},
            };

            if (langFrom != null)
                param["From"] = "'" + langFrom + "'";

            var uri = new Uri(TranslateUri + "?" + MyCommon.BuildQueryString(param));
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = CreateBasicAuthHeaderValue(ApplicationSettings.AzureMarketplaceKey, ApplicationSettings.AzureMarketplaceKey);

            using (var response = await this.http.SendAsync(request).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var xmlStr = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                var xdoc = XDocument.Parse(xmlStr);

                return xdoc.Root.Value;
            }
        }

        public static string GetLanguageEnumFromIndex(int index)
        {
            return LanguageTable[index];
        }

        public static int GetIndexFromLanguageEnum(string lang)
        {
            return LanguageTable.IndexOf(lang);
        }

        internal static AuthenticationHeaderValue CreateBasicAuthHeaderValue(string user, string pass)
        {
            var paramBytes = Encoding.UTF8.GetBytes(user + ":" + pass);
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(paramBytes));
        }
    }
}
