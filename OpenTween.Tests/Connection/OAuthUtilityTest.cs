// OpenTween - Client of Twitter
// Copyright (c) 2016 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween.Connection
{
    public class OAuthUtilityTest
    {
        [Fact]
        public void GetOAuthParameter_Test()
        {
            var param = OAuthUtility.GetOAuthParameter(ApiKey.Create("ConsumerKey"), "Token");

            Assert.Equal("ConsumerKey", param["oauth_consumer_key"]);
            Assert.Equal("HMAC-SHA1", param["oauth_signature_method"]);

            var unixTimeNow = DateTimeUtc.Now.ToUnixTime();
            Assert.InRange(long.Parse(param["oauth_timestamp"]), unixTimeNow - 5, unixTimeNow);

            Assert.NotEmpty(param["oauth_nonce"]);
            Assert.Equal("1.0", param["oauth_version"]);
            Assert.Equal("Token", param["oauth_token"]);
        }

        [Fact]
        public void CreateSignature_Test()
        {
            // GET http://example.com/hoge?aaa=foo に対する署名を生成
            // 実際の param は oauth_consumer_key などのパラメーターが加わった状態で渡される
            var oauthSignature = OAuthUtility.CreateSignature(
                ApiKey.Create("ConsumerSecret"),
                "TokenSecret",
                "GET",
                new Uri("http://example.com/hoge"),
                new Dictionary<string, string> { ["aaa"] = "foo" });

            var expectSignatureBase = "GET&http%3A%2F%2Fexample.com%2Fhoge&aaa%3Dfoo";
            var expectSignatureKey = "ConsumerSecret&TokenSecret";

            using var hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(expectSignatureKey));
            var expectSignature = Convert.ToBase64String(hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(expectSignatureBase)));
            Assert.Equal(expectSignature, oauthSignature);
        }

        [Fact]
        public void CreateSignature_NormarizeParametersTest()
        {
            // GET http://example.com/hoge?aaa=foo&bbb=bar に対する署名を生成
            // 複数のパラメータが渡される場合は name 順でソートされる
            var oauthSignature = OAuthUtility.CreateSignature(
                ApiKey.Create("ConsumerSecret"),
                "TokenSecret",
                "GET",
                new Uri("http://example.com/hoge"),
                new Dictionary<string, string>
                {
                    ["bbb"] = "bar",
                    ["aaa"] = "foo",
                });

            var expectSignatureBase = "GET&http%3A%2F%2Fexample.com%2Fhoge&aaa%3Dfoo%26bbb%3Dbar";
            var expectSignatureKey = "ConsumerSecret&TokenSecret";

            using var hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(expectSignatureKey));
            var expectSignature = Convert.ToBase64String(hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(expectSignatureBase)));
            Assert.Equal(expectSignature, oauthSignature);
        }

        [Fact]
        public void CreateSignature_EmptyTokenSecretTest()
        {
            // GET http://example.com/hoge?aaa=foo に対する署名を生成
            // リクエストトークンの発行時は tokenSecret が空の状態で署名を生成することになる
            var oauthSignature = OAuthUtility.CreateSignature(
                ApiKey.Create("ConsumerSecret"),
                null,
                "GET",
                new Uri("http://example.com/hoge"),
                new Dictionary<string, string> { ["aaa"] = "foo" });

            var expectSignatureBase = "GET&http%3A%2F%2Fexample.com%2Fhoge&aaa%3Dfoo";
            var expectSignatureKey = "ConsumerSecret&"; // 末尾の & は除去されない

            using var hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(expectSignatureKey));
            var expectSignature = Convert.ToBase64String(hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(expectSignatureBase)));
            Assert.Equal(expectSignature, oauthSignature);
        }

        [Fact]
        public void CreateAuthorization_Test()
        {
            var authorization = OAuthUtility.CreateAuthorization(
                "GET",
                new Uri("http://example.com/hoge"),
                new Dictionary<string, string> { ["aaa"] = "hoge" },
                ApiKey.Create("ConsumerKey"),
                ApiKey.Create("ConsumerSecret"),
                "AccessToken",
                "AccessSecret",
                "Realm");

            Assert.StartsWith("OAuth ", authorization, StringComparison.Ordinal);

            var parsedParams = authorization.Substring(6).Split(',')
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Split(new[] { '=' }, 2))
                .ToDictionary(x => x[0], x => x[1].Substring(1, x[1].Length - 2)); // x[1] は前後の「"」を除去する

            var expectAuthzParamKeys = new[]
            {
                "realm",
                "oauth_consumer_key",
                "oauth_nonce",
                "oauth_signature_method",
                "oauth_timestamp",
                "oauth_token",
                "oauth_version",
                "oauth_signature",
            };
            Assert.Equal(expectAuthzParamKeys, parsedParams.Keys, AnyOrderComparer<string>.Instance);

            Assert.Equal("Realm", parsedParams["realm"]);

            // Signature Base Strings には realm を含めない
            var expectSignatureBase = "GET&http%3A%2F%2Fexample.com%2Fhoge&" +
                "aaa%3Dhoge%26" +
                "oauth_consumer_key%3DConsumerKey%26" +
                $"oauth_nonce%3D{parsedParams["oauth_nonce"]}%26" +
                "oauth_signature_method%3DHMAC-SHA1%26" +
                $"oauth_timestamp%3D{parsedParams["oauth_timestamp"]}%26" +
                "oauth_token%3DAccessToken%26" +
                "oauth_version%3D1.0";

            var expectSignatureKey = "ConsumerSecret&AccessSecret";

            using var hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(expectSignatureKey));
            var expectSignature = Convert.ToBase64String(hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(expectSignatureBase)));
            Assert.Equal(expectSignature, Uri.UnescapeDataString(parsedParams["oauth_signature"]));
        }
    }
}
