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

#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using OpenTween.Api;
using OpenTween.Connection;

namespace OpenTween
{
    /// <summary>
    /// 短縮 URL サービスによる URL の展開・短縮を行うクラス
    /// </summary>
    public class ShortUrl
    {
        private static readonly Lazy<ShortUrl> InstanceLazy;

        /// <summary>
        /// ShortUrl のインスタンスを取得します
        /// </summary>
        public static ShortUrl Instance
            => InstanceLazy.Value;

        /// <summary>
        /// 短縮 URL の展開を無効にするか否か
        /// </summary>
        public bool DisableExpanding { get; set; }

        /// <summary>
        /// 短縮 URL のキャッシュを定期的にクリアする回数
        /// </summary>
        public int PurgeCount { get; set; }

        public string BitlyAccessToken { get; set; } = "";

        public string BitlyId { get; set; } = "";

        public string BitlyKey { get; set; } = "";

        private HttpClient http;
        private readonly ConcurrentDictionary<Uri, Uri> urlCache = new();

        private static readonly Regex HtmlLinkPattern = new(@"(<a href="")(.+?)("")");

        private static readonly HashSet<string> ShortUrlHosts = new()
        {
            "4sq.com",
            "amzn.to",
            "bit.ly",
            "blip.fm",
            "budurl.com",
            "budurl.me",
            "buff.ly",
            "disq.us",
            "dlvr.it",
            "fb.me",
            "feedly.com",
            "feeds.feedburner.com",
            "ff.im",
            "flic.kr",
            "goo.gl",
            "ht.ly",
            "htn.to",
            "ift.tt",
            "is.gd",
            "j.mp",
            "moby.to",
            "moi.st",
            "nico.ms",
            "on.digg.com",
            "on.fb.me",
            "ow.ly",
            "qurl.com",
            "t.co",
            "tinami.jp",
            "tiny.cc",
            "tinyurl.com",
            "tl.gd",
            "tmblr.co",
            "tumblr.com",
            "twme.jp",
            "urx2.nu",
            "ux.nu",
            "wp.me",
            "www.qurl.com",
            "www.tumblr.com",
            "youtu.be",
        };

        /// <summary>
        /// HTTPS非対応の短縮URLサービス
        /// </summary>
        private static readonly HashSet<string> InsecureDomains = new()
        {
            "budurl.com",
            "ff.im",
            "ht.ly",
            "moi.st",
            "ow.ly",
            "tinami.jp",
            "tl.gd",
            "twme.jp",
            "urx2.nu",
        };

        static ShortUrl()
            => InstanceLazy = new Lazy<ShortUrl>(() => new ShortUrl(), true);

        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        internal ShortUrl()
            : this(CreateDefaultHttpClient())
        {
            Networking.WebProxyChanged += (o, e) =>
            {
                var newClient = CreateDefaultHttpClient();
                var oldClient = Interlocked.Exchange(ref this.http, newClient);
                oldClient.Dispose();
            };
        }

        internal ShortUrl(HttpClient http)
        {
            this.DisableExpanding = false;
            this.PurgeCount = 500;
            this.BitlyId = "";
            this.BitlyKey = "";

            this.http = http;
        }

        [Obsolete]
        public string ExpandUrl(string uri)
        {
            try
            {
                return this.ExpandUrlAsync(new Uri(uri), 10).Result.AbsoluteUri;
            }
            catch (UriFormatException)
            {
                return uri;
            }
        }

        /// <summary>
        /// 短縮 URL を非同期に展開します
        /// </summary>
        /// <param name="uri">展開するURL</param>
        /// <returns>URLの展開を行うタスク</returns>
        public Task<Uri> ExpandUrlAsync(Uri uri)
            => this.ExpandUrlAsync(uri, 10);

        /// <summary>
        /// 短縮 URL を非同期に展開します
        /// </summary>
        /// <param name="uri">展開するURL</param>
        /// <param name="redirectLimit">再帰的に展開を試みる上限</param>
        /// <returns>URLの展開を行うタスク</returns>
        public async Task<Uri> ExpandUrlAsync(Uri uri, int redirectLimit)
        {
            if (this.DisableExpanding)
                return uri;

            if (redirectLimit <= 0)
                return uri;

            if (!uri.IsAbsoluteUri)
                return uri;

            try
            {
                if (!ShortUrlHosts.Contains(uri.Host) && !this.IsIrregularShortUrl(uri))
                    return uri;

                if (this.urlCache.TryGetValue(uri, out var expanded))
                    return expanded;

                if (this.urlCache.Count > this.PurgeCount)
                    this.urlCache.Clear();

                expanded = null;
                try
                {
                    expanded = await this.GetRedirectTo(uri)
                        .ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                }
                catch (HttpRequestException)
                {
                }

                if (expanded == null || expanded == uri)
                    return uri;

                this.urlCache[uri] = expanded;

                var recursiveExpanded = await this.ExpandUrlAsync(expanded, --redirectLimit)
                    .ConfigureAwait(false);

                // URL1 -> URL2 -> URL3 のように再帰的に展開されたURLを URL1 -> URL3 としてキャッシュに格納する
                if (recursiveExpanded != expanded)
                    this.urlCache[uri] = recursiveExpanded;

                return recursiveExpanded;
            }
            catch (UriFormatException)
            {
                return uri;
            }
        }

        /// <summary>
        /// 短縮 URL を非同期に展開します
        /// </summary>
        /// <remarks>
        /// 不正なURLが渡された場合は例外を投げず uriStr をそのまま返します
        /// </remarks>
        /// <param name="uriStr">展開するURL</param>
        /// <returns>URLの展開を行うタスク</returns>
        public Task<string> ExpandUrlAsync(string uriStr)
            => this.ExpandUrlAsync(uriStr, 10);

        /// <summary>
        /// 短縮 URL を非同期に展開します
        /// </summary>
        /// <remarks>
        /// 不正なURLが渡された場合は例外を投げず uriStr をそのまま返します
        /// </remarks>
        /// <param name="uriStr">展開するURL</param>
        /// <param name="redirectLimit">再帰的に展開を試みる上限</param>
        /// <returns>URLの展開を行うタスク</returns>
        public async Task<string> ExpandUrlAsync(string uriStr, int redirectLimit)
        {
            Uri uri;

            try
            {
                if (!uriStr.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    uri = new Uri("http://" + uriStr);
                else
                    uri = new Uri(uriStr);
            }
            catch (UriFormatException)
            {
                return uriStr;
            }

            var expandedUri = await this.ExpandUrlAsync(uri, redirectLimit)
                .ConfigureAwait(false);

            return expandedUri.OriginalString;
        }

        [Obsolete]
        public string ExpandUrlHtml(string html)
            => this.ExpandUrlHtmlAsync(html, 10).Result;

        /// <summary>
        /// HTML内に含まれるリンクのURLを非同期に展開する
        /// </summary>
        /// <param name="html">処理対象のHTML</param>
        /// <returns>URLの展開を行うタスク</returns>
        public Task<string> ExpandUrlHtmlAsync(string html)
            => this.ExpandUrlHtmlAsync(html, 10);

        /// <summary>
        /// HTML内に含まれるリンクのURLを非同期に展開する
        /// </summary>
        /// <param name="html">処理対象のHTML</param>
        /// <param name="redirectLimit">再帰的に展開を試みる上限</param>
        /// <returns>URLの展開を行うタスク</returns>
        public Task<string> ExpandUrlHtmlAsync(string html, int redirectLimit)
        {
            if (this.DisableExpanding)
                return Task.FromResult(html);

            return HtmlLinkPattern.ReplaceAsync(html, async m =>
                m.Groups[1].Value + await this.ExpandUrlAsync(m.Groups[2].Value, redirectLimit).ConfigureAwait(false) + m.Groups[3].Value);
        }

        /// <summary>
        /// 指定された短縮URLサービスを使用してURLを短縮します
        /// </summary>
        /// <param name="shortenerType">使用する短縮URLサービス</param>
        /// <param name="srcUri">短縮するURL</param>
        /// <returns>短縮されたURL</returns>
        public async Task<Uri> ShortenUrlAsync(MyCommon.UrlConverter shortenerType, Uri srcUri)
        {
            // 既に短縮されている状態のURLであれば短縮しない
            if (ShortUrlHosts.Contains(srcUri.Host))
                return srcUri;

            try
            {
                return shortenerType switch
                {
                    MyCommon.UrlConverter.TinyUrl => await this.ShortenByTinyUrlAsync(srcUri)
                            .ConfigureAwait(false),
                    MyCommon.UrlConverter.Isgd => await this.ShortenByIsgdAsync(srcUri)
                            .ConfigureAwait(false),
                    MyCommon.UrlConverter.Bitly => await this.ShortenByBitlyAsync(srcUri, "bit.ly")
                            .ConfigureAwait(false),
                    MyCommon.UrlConverter.Jmp => await this.ShortenByBitlyAsync(srcUri, "j.mp")
                            .ConfigureAwait(false),
                    MyCommon.UrlConverter.Uxnu => await this.ShortenByUxnuAsync(srcUri)
                            .ConfigureAwait(false),
                    _ => throw new ArgumentException("Unknown shortener.", nameof(shortenerType)),
                };
            }
            catch (OperationCanceledException)
            {
                // 短縮 URL の API がタイムアウトした場合
                return srcUri;
            }
        }

        private async Task<Uri> ShortenByTinyUrlAsync(Uri srcUri)
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("https://tinyurl.com/xxxxxxxx".Length > srcUri.OriginalString.Length)
                return srcUri;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("url", srcUri.OriginalString),
            });

            using var response = await this.http.PostAsync("https://tinyurl.com/api-create.php", content)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            if (!Regex.IsMatch(result, @"^https?://"))
                throw new WebApiException("Failed to create URL.", result);

            return this.UpgradeToHttpsIfAvailable(new Uri(result.TrimEnd()));
        }

        private async Task<Uri> ShortenByIsgdAsync(Uri srcUri)
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("https://is.gd/xxxxxx".Length > srcUri.OriginalString.Length)
                return srcUri;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("format", "simple"),
                new KeyValuePair<string, string>("url", srcUri.OriginalString),
            });

            using var response = await this.http.PostAsync("https://is.gd/create.php", content)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            if (!Regex.IsMatch(result, @"^https?://"))
                throw new WebApiException("Failed to create URL.", result);

            return new Uri(result.TrimEnd());
        }

        private async Task<Uri> ShortenByBitlyAsync(Uri srcUri, string domain = "bit.ly")
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("https://bit.ly/xxxxxxx".Length > srcUri.OriginalString.Length)
                return srcUri;

            // OAuth2 アクセストークンまたは API キー (旧方式) のいずれも設定されていなければ短縮しない
            if (MyCommon.IsNullOrEmpty(this.BitlyAccessToken) && (MyCommon.IsNullOrEmpty(this.BitlyId) || MyCommon.IsNullOrEmpty(this.BitlyKey)))
                return srcUri;

            var bitly = new BitlyApi
            {
                EndUserAccessToken = this.BitlyAccessToken,
                EndUserLoginName = this.BitlyId,
                EndUserApiKey = this.BitlyKey,
            };

            var result = await bitly.ShortenAsync(srcUri, domain)
                .ConfigureAwait(false);

            return this.UpgradeToHttpsIfAvailable(result);
        }

        private async Task<Uri> ShortenByUxnuAsync(Uri srcUri)
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("https://ux.nu/xxxxx".Length > srcUri.OriginalString.Length)
                return srcUri;

            var query = new Dictionary<string, string>
            {
                ["format"] = "plain",
                ["url"] = srcUri.OriginalString,
            };

            var uri = new Uri("https://ux.nu/api/short?" + MyCommon.BuildQueryString(query));
            using var response = await this.http.GetAsync(uri)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            if (!Regex.IsMatch(result, @"^https?://"))
                throw new WebApiException("Failed to create URL.", result);

            return new Uri(result.TrimEnd());
        }

        private bool IsIrregularShortUrl(Uri uri)
        {
            // Flickrの https://www.flickr.com/photo.gne?short=... 形式のURL
            // flic.kr ドメインのURLを展開する途中に経由する
            if (uri.Host.EndsWith("flickr.com", StringComparison.OrdinalIgnoreCase) &&
                uri.PathAndQuery.StartsWith("/photo.gne", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private async Task<Uri?> GetRedirectTo(Uri url)
        {
            url = this.UpgradeToHttpsIfAvailable(url);

            var request = new HttpRequestMessage(HttpMethod.Head, url);

            using var response = await this.http.SendAsync(request)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                // ステータスコードが 3xx であれば例外を発生させない
                if ((int)response.StatusCode / 100 != 3)
                    response.EnsureSuccessStatusCode();
            }

            var redirectedUrl = response.Headers.Location;

            if (redirectedUrl == null)
                return null;

            // サーバーが URL を適切にエンコードしていない場合、OriginalString に非 ASCII 文字が含まれる。
            // その場合、redirectedUrl は文字化けしている可能性があるため使用しない
            // 参照: http://stackoverflow.com/questions/1888933
            if (redirectedUrl.OriginalString.Any(x => x < ' ' || x > '~'))
                return null;

            if (redirectedUrl.IsAbsoluteUri)
                return redirectedUrl;
            else
                return new Uri(url, redirectedUrl);
        }

        /// <summary>
        /// 指定されたURLのスキームを https:// に書き換える
        /// </summary>
        private Uri UpgradeToHttpsIfAvailable(Uri original)
        {
            if (original.Scheme != "http")
                return original;

            if (InsecureDomains.Contains(original.Host))
                return original;

            var builder = new UriBuilder(original);
            builder.Scheme = "https";
            builder.Port = 443;

            return builder.Uri;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        private static HttpClient CreateDefaultHttpClient()
        {
            var handler = Networking.CreateHttpClientHandler();
            handler.AllowAutoRedirect = false;

            var http = Networking.CreateHttpClient(handler);
            http.Timeout = TimeSpan.FromSeconds(30);

            return http;
        }
    }
}
