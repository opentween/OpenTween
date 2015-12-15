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
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details. 
// 
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using OpenTween.Connection;

namespace OpenTween
{
    public class HttpOAuthApiProxy : HttpConnectionOAuth
    {
        private const string _apiHost = "api.twitter.com";
        private static string _proxyHost = "";

        public static string ProxyHost
        {
            get { return _proxyHost; }
            set
            {
                if (string.IsNullOrEmpty(value) || value == _apiHost)
                    _proxyHost = "";
                else
                    _proxyHost = value;
            }
        }

        protected override void AppendOAuthInfo(HttpWebRequest webRequest, Dictionary<string, string> query,
            string token, string tokenSecret)
        {
            var requestUri = webRequest.RequestUri;

            // 本来のアクセス先URLに再設定（api.twitter.com固定）
            if (!string.IsNullOrEmpty(_proxyHost) && requestUri.Host == _proxyHost)
            {
                var rewriteUriStr = requestUri.GetLeftPart(UriPartial.Scheme) + _proxyHost + requestUri.PathAndQuery;
                requestUri = new Uri(rewriteUriStr);
            }

            var credential = OAuthUtility.CreateAuthorization(webRequest.Method, requestUri, query,
                this.consumerKey, this.consumerSecret, token, tokenSecret);

            webRequest.Headers.Add(HttpRequestHeader.Authorization, credential);
        }

    }
}
