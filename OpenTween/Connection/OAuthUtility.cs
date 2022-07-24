// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      spinor (@tplantd) <http://d.hatena.ne.jp/spinor/>
//           (c) 2015      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Connection
{
    public static class OAuthUtility
    {
        /// <summary>
        /// OAuth署名のoauth_nonce算出用乱数クラス
        /// </summary>
        private static readonly Random NonceRandom = new();

        /// <summary>
        /// HTTPリクエストに追加するAuthorizationヘッダの値を生成します
        /// </summary>
        /// <param name="httpMethod">リクエストのHTTPメソッド</param>
        /// <param name="requestUri">リクエスト先のURI</param>
        /// <param name="query">OAuth追加情報＋クエリ or POSTデータ</param>
        /// <param name="consumerKey">コンシューマーキー</param>
        /// <param name="consumerSecret">コンシューマーシークレット</param>
        /// <param name="token">アクセストークン、もしくはリクエストトークン。未取得なら空文字列</param>
        /// <param name="tokenSecret">アクセストークンシークレット。認証処理では空文字列</param>
        /// <param name="realm">realm (必要な場合のみ)</param>
        public static string CreateAuthorization(
            string httpMethod,
            Uri requestUri,
            IEnumerable<KeyValuePair<string, string>>? query,
            ApiKey consumerKey,
            ApiKey consumerSecret,
            string token,
            string tokenSecret,
            string? realm = null)
        {
            // OAuth共通情報取得
            var parameter = GetOAuthParameter(consumerKey, token);
            // OAuth共通情報にquery情報を追加
            if (query != null)
            {
                foreach (var (key, value) in query)
                    parameter.Add(key, value);
            }
            // 署名の作成・追加
            parameter.Add("oauth_signature", CreateSignature(consumerSecret, tokenSecret, httpMethod, requestUri, parameter));
            // HTTPリクエストのヘッダに追加
            var sb = new StringBuilder("OAuth ");

            if (realm != null)
                sb.AppendFormat("realm=\"{0}\",", realm);

            foreach (var (key, value) in parameter)
                // 各種情報のうち、oauth_で始まる情報のみ、ヘッダに追加する。各情報はカンマ区切り、データはダブルクォーテーションで括る
                if (key.StartsWith("oauth_", StringComparison.Ordinal))
                    sb.AppendFormat("{0}=\"{1}\",", key, MyCommon.UrlEncode(value));

            return sb.ToString();
        }

        /// <summary>
        /// OAuthで使用する共通情報を取得する
        /// </summary>
        /// <param name="token">アクセストークン、もしくはリクエストトークン。未取得なら空文字列</param>
        /// <returns>OAuth情報のディクショナリ</returns>
        public static Dictionary<string, string> GetOAuthParameter(ApiKey consumerKey, string token)
        {
            var parameter = new Dictionary<string, string>
            {
                ["oauth_consumer_key"] = consumerKey.Value,
                ["oauth_signature_method"] = "HMAC-SHA1",
                ["oauth_timestamp"] = DateTimeUtc.Now.ToUnixTime().ToString(), // epoch秒
                ["oauth_nonce"] = NonceRandom.Next(123400, 9999999).ToString(),
                ["oauth_version"] = "1.0",
            };
            if (!MyCommon.IsNullOrEmpty(token))
                parameter.Add("oauth_token", token); // トークンがあれば追加
            return parameter;
        }

        /// <summary>
        /// OAuth認証ヘッダの署名作成
        /// </summary>
        /// <param name="tokenSecret">アクセストークン秘密鍵</param>
        /// <param name="method">HTTPメソッド文字列</param>
        /// <param name="uri">アクセス先Uri</param>
        /// <param name="parameter">クエリ、もしくはPOSTデータ</param>
        /// <returns>署名文字列</returns>
        public static string CreateSignature(ApiKey consumerSecret, string? tokenSecret, string method, Uri uri, Dictionary<string, string> parameter)
        {
            // パラメタをソート済みディクショナリに詰替（OAuthの仕様）
            var sorted = new SortedDictionary<string, string>(parameter);
            // URLエンコード済みのクエリ形式文字列に変換
            var paramString = MyCommon.BuildQueryString(sorted);
            // アクセス先URLの整形
            var url = string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath);
            // 署名のベース文字列生成（&区切り）。クエリ形式文字列は再エンコードする
            var signatureBase = string.Format("{0}&{1}&{2}", method, MyCommon.UrlEncode(url), MyCommon.UrlEncode(paramString));
            // 署名鍵の文字列をコンシューマー秘密鍵とアクセストークン秘密鍵から生成（&区切り。アクセストークン秘密鍵なくても&残すこと）
            var key = MyCommon.UrlEncode(consumerSecret.Value) + "&";
            if (!MyCommon.IsNullOrEmpty(tokenSecret))
                key += MyCommon.UrlEncode(tokenSecret);
            // 鍵生成＆署名生成
            using var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase));
            return Convert.ToBase64String(hash);
        }
    }
}
