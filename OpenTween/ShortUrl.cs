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
using OpenTween.Connection;

namespace OpenTween
{
    /// <summary>
    /// 短縮 URL サービスによる URL の展開・短縮を行うクラス
    /// </summary>
    public class ShortUrl
    {
        private static Lazy<ShortUrl> _instance;

        /// <summary>
        /// ShortUrl のインスタンスを取得します
        /// </summary>
        public static ShortUrl Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// 短縮 URL の展開を無効にするか否か
        /// </summary>
        public bool DisableExpanding { get; set; }

        /// <summary>
        /// 短縮 URL のキャッシュを定期的にクリアする回数
        /// </summary>
        public int PurgeCount { get; set; }

        public string BitlyId { get; set; }
        public string BitlyKey { get; set; }

        private HttpClient http;
        private ConcurrentDictionary<Uri, Uri> urlCache = new ConcurrentDictionary<Uri, Uri>();

        private static readonly Regex HtmlLinkPattern = new Regex(@"(<a href="")(.+?)("")");

        private static readonly HashSet<string> ShortUrlHosts = new HashSet<string>()
        {
            "4sq.com",
            "airme.us",
            "amzn.to",
            "bctiny.com",
            "bit.ly",
            "bkite.com",
            "blip.fm",
            "budurl.com",
            "buff.ly",
            "cli.gs",
            "digg.com",
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
            "icanhaz.com",
            "ift.tt",
            "is.gd",
            "j.mp",
            "linkbee.com",
            "moby.to",
            "moi.st",
            "nico.ms",
            "nsfw.in",
            "on.fb.me",
            "ow.ly",
            "p.tl",
            "pic.gd",
            "qurl.com",
            "rubyurl.com",
            "snipurl.com",
            "snurl.com",
            "t.co",
            "tinami.jp",
            "tiny.cc",
            "tinyurl.com",
            "tl.gd",
            "tmblr.co",
            "traceurl.com",
            "tumblr.com",
            "twitthis.com",
            "twme.jp",
            "twurl.nl",
            "u-rl.jp",
            "urlenco.de",
            "urx2.nu",
            "ustre.am",
            "ux.nu",
            "wp.me",
            "www.qurl.com",
            "www.tumblr.com",
            "youtu.be",
        };

        static ShortUrl()
        {
            _instance = new Lazy<ShortUrl>(() => new ShortUrl(), true);
        }

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
        {
            return this.ExpandUrlAsync(uri, 10);
        }

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
                if (!ShortUrlHosts.Contains(uri.Host) && !IsIrregularShortUrl(uri))
                    return uri;

                Uri expanded;
                if (this.urlCache.TryGetValue(uri, out expanded))
                    return expanded;

                if (this.urlCache.Count > this.PurgeCount)
                    this.urlCache.Clear();

                expanded = null;
                try
                {
                    expanded = await this.GetRedirectTo(uri)
                        .ConfigureAwait(false);
                }
                catch (TaskCanceledException) { }
                catch (HttpRequestException) { }

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
        {
            return this.ExpandUrlAsync(uriStr, 10);
        }

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
        {
            return this.ExpandUrlHtmlAsync(html, 10).Result;
        }

        /// <summary>
        /// HTML内に含まれるリンクのURLを非同期に展開する
        /// </summary>
        /// <param name="html">処理対象のHTML</param>
        /// <returns>URLの展開を行うタスク</returns>
        public Task<string> ExpandUrlHtmlAsync(string html)
        {
            return this.ExpandUrlHtmlAsync(html, 10);
        }

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
                switch (shortenerType)
                {
                    case MyCommon.UrlConverter.TinyUrl:
                        return await this.ShortenByTinyUrlAsync(srcUri)
                            .ConfigureAwait(false);
                    case MyCommon.UrlConverter.Isgd:
                        return await this.ShortenByIsgdAsync(srcUri)
                            .ConfigureAwait(false);
                    case MyCommon.UrlConverter.Twurl:
                        return await this.ShortenByTwurlAsync(srcUri)
                            .ConfigureAwait(false);
                    case MyCommon.UrlConverter.Bitly:
                        return await this.ShortenByBitlyAsync(srcUri, "bit.ly")
                            .ConfigureAwait(false);
                    case MyCommon.UrlConverter.Jmp:
                        return await this.ShortenByBitlyAsync(srcUri, "j.mp")
                            .ConfigureAwait(false);
                    case MyCommon.UrlConverter.Uxnu:
                        return await this.ShortenByUxnuAsync(srcUri)
                            .ConfigureAwait(false);
                    default:
                        throw new ArgumentException("Unknown shortener.", "shortenerType");
                }
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
            if ("http://tinyurl.com/xxxxxx".Length > srcUri.OriginalString.Length)
                return srcUri;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("url", srcUri.OriginalString),
            });

            using (var response = await this.http.PostAsync("http://tinyurl.com/api-create.php", content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                if (!Regex.IsMatch(result, @"^https?://"))
                    throw new WebApiException("Failed to create URL.", result);

                return new Uri(result.TrimEnd());
            }
        }

        private async Task<Uri> ShortenByIsgdAsync(Uri srcUri)
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("http://is.gd/xxxx".Length > srcUri.OriginalString.Length)
                return srcUri;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("format", "simple"),
                new KeyValuePair<string, string>("url", srcUri.OriginalString),
            });

            using (var response = await this.http.PostAsync("http://is.gd/create.php", content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                if (!Regex.IsMatch(result, @"^https?://"))
                    throw new WebApiException("Failed to create URL.", result);

                return new Uri(result.TrimEnd());
            }
        }

        private async Task<Uri> ShortenByTwurlAsync(Uri srcUri)
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("http://twurl.nl/xxxxxx".Length > srcUri.OriginalString.Length)
                return srcUri;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("link[url]", srcUri.OriginalString),
            });

            using (var response = await this.http.PostAsync("http://tweetburner.com/links", content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                if (!Regex.IsMatch(result, @"^https?://"))
                    throw new WebApiException("Failed to create URL.", result);

                return new Uri(result.TrimEnd());
            }
        }

        private async Task<Uri> ShortenByBitlyAsync(Uri srcUri, string domain = "bit.ly")
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("http://bit.ly/xxxx".Length > srcUri.OriginalString.Length)
                return srcUri;

            // bit.ly 短縮機能実装のプライバシー問題の暫定対応
            // ログインIDとAPIキーが指定されていない場合は短縮せずにPOSTする
            // 参照: http://sourceforge.jp/projects/opentween/lists/archive/dev/2012-January/000020.html
            if (string.IsNullOrEmpty(this.BitlyId) || string.IsNullOrEmpty(this.BitlyKey))
                return srcUri;

            var query = new Dictionary<string, string>
            {
                {"login", this.BitlyId},
                {"apiKey", this.BitlyKey},
                {"format", "txt"},
                {"domain", domain},
                {"longUrl", srcUri.OriginalString},
            };

            var uri = new Uri("https://api-ssl.bitly.com/v3/shorten?" + MyCommon.BuildQueryString(query));
            using (var response = await this.http.GetAsync(uri).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                if (!Regex.IsMatch(result, @"^https?://"))
                    throw new WebApiException("Failed to create URL.", result);

                return new Uri(result.TrimEnd());
            }
        }

        private async Task<Uri> ShortenByUxnuAsync(Uri srcUri)
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("http://ux.nx/xxxxxx".Length > srcUri.OriginalString.Length)
                return srcUri;

            var query = new Dictionary<string, string>
            {
                {"format", "plain"},
                {"url", srcUri.OriginalString},
            };

            var uri = new Uri("http://ux.nu/api/short?" + MyCommon.BuildQueryString(query));
            using (var response = await this.http.GetAsync(uri).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                if (!Regex.IsMatch(result, @"^https?://"))
                    throw new WebApiException("Failed to create URL.", result);

                return new Uri(result.TrimEnd());
            }
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

        private async Task<Uri> GetRedirectTo(Uri url)
        {
            var request = new HttpRequestMessage(HttpMethod.Head, url);

            using (var response = await this.http.SendAsync(request).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    // ステータスコードが 3xx であれば例外を発生させない
                    if ((int)response.StatusCode / 100 != 3)
                        response.EnsureSuccessStatusCode();
                }

                var redirectedUrl = response.Headers.Location;

                if (redirectedUrl == null)
                    return null;

                if (redirectedUrl.IsAbsoluteUri)
                    return redirectedUrl;
                else
                    return new Uri(url, redirectedUrl);
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        private static HttpClient CreateDefaultHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
            };

            var http = Networking.CreateHttpClient(handler);
            http.Timeout = TimeSpan.FromSeconds(5);

            return http;
        }
    }
}
