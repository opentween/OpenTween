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
// with this program. if (not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace OpenTween
{
    public class HttpOAuthApiProxy : HttpConnectionOAuth
    {
        private const string _apiHost = "api.twitter.com";
        private static string _proxyHost = "";

        public static string ProxyHost
        {
            set
            {
                if (string.IsNullOrEmpty(value) || value == _apiHost)
                    _proxyHost = "";
                else
                    _proxyHost = value;
            }
        }

        protected override string CreateSignature(string tokenSecret,
                                         string method,
                                         Uri uri,
                                         Dictionary<string, string> parameter)
        {
            //パラメタをソート済みディクショナリに詰替（OAuthの仕様）
            SortedDictionary<string, string> sorted = new SortedDictionary<string, string>(parameter);
            //URLエンコード済みのクエリ形式文字列に変換
            string paramString = CreateQueryString(sorted);
            //アクセス先URLの整形
            string url = string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath);
            //本来のアクセス先URLに再設定（api.twitter.com固定）
            if (!string.IsNullOrEmpty(_proxyHost) && url.StartsWith(uri.Scheme + "://" + _proxyHost))
                url = url.Replace(uri.Scheme + "://" + _proxyHost, uri.Scheme + "://" + _apiHost);
            //署名のベース文字列生成（&区切り）。クエリ形式文字列は再エンコードする
            string signatureBase = String.Format("{0}&{1}&{2}", method, UrlEncode(url), UrlEncode(paramString));
            //署名鍵の文字列をコンシューマー秘密鍵とアクセストークン秘密鍵から生成（&区切り。アクセストークン秘密鍵なくても&残すこと）
            string key = UrlEncode(consumerSecret) + "&";
            if (!string.IsNullOrEmpty(tokenSecret)) key += UrlEncode(tokenSecret);
            //鍵生成＆署名生成
            using (HMACSHA1 hmac = new HMACSHA1(Encoding.ASCII.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase));
                return Convert.ToBase64String(hash);
            }
        }

    }
}
