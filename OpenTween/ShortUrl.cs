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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenTween
{
    /// <summary>
    /// 短縮 URL サービスによる URL の展開・短縮を行うクラス
    /// </summary>
    public class ShortUrl
    {
        private static Lazy<ShortUrl> _instance = new Lazy<ShortUrl>(() => new ShortUrl(new HttpVarious()), true);

        /// <summary>
        /// ShortUrl のインストタンスを取得します
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

        /// <summary>
        /// リダイレクトのタイムアウト時間 (単位: ms)
        /// </summary>
        public int RedirectTimeout { get; set; }

        public string BitlyId { get; set; }
        public string BitlyKey { get; set; }

        private HttpVarious http;
        private ConcurrentDictionary<string, string> urlCache = new ConcurrentDictionary<string, string>();

        private static readonly Regex HtmlLinkPattern = new Regex(@"(<a href="")(.+?)("")");

        private static readonly HashSet<string> ShortUrlHosts = new HashSet<string>()
        {
            "airme.us",
            "amzn.to",
            "bctiny.com",
            "bit.ly",
            "bkite.com",
            "blip.fm",
            "budurl.com",
            "cli.gs",
            "digg.com",
            "dlvr.it",
            "fb.me",
            "feeds.feedburner.com",
            "ff.im",
            "flic.kr",
            "goo.gl",
            "ht.ly",
            "htn.to",
            "icanhaz.com",
            "is.gd",
            "j.mp",
            "linkbee.com",
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
            "traceurl.com",
            "tumblr.com",
            "twitthis.com",
            "twurl.nl",
            "urlenco.de",
            "ustre.am",
            "ux.nu",
            "www.qurl.com",
            "youtu.be",
        };

        internal ShortUrl(HttpVarious http)
        {
            this.DisableExpanding = false;
            this.PurgeCount = 500;
            this.RedirectTimeout = 1000;
            this.BitlyId = "";
            this.BitlyKey = "";

            this.http = http;
        }

        /// <summary>
        /// 短縮 URL を展開します
        /// </summary>
        /// <param name="uri">展開するURL</param>
        /// <returns>展開されたURL</returns>
        public string ExpandUrl(string uri)
        {
            if (this.DisableExpanding)
                return uri;

            try
            {
                if (!ShortUrlHosts.Contains(new Uri(uri).Host))
                    return uri;

                string expanded;
                if (this.urlCache.TryGetValue(uri, out expanded))
                    return expanded;

                if (this.urlCache.Count > this.PurgeCount)
                    this.urlCache.Clear();

                expanded = this.http.GetRedirectTo(uri, this.RedirectTimeout);
                this.urlCache[uri] = expanded;

                return expanded;
            }
            catch (UriFormatException)
            {
                return uri;
            }
        }

        /// <summary>
        /// HTML内に含まれるリンクのURLを展開する
        /// </summary>
        /// <param name="html">処理対象のHTML</param>
        /// <returns>展開されたURLを含むHTML</returns>
        public string ExpandUrlHtml(string html)
        {
            if (this.DisableExpanding)
                return html;

            return HtmlLinkPattern.Replace(html, m => m.Groups[1].Value + this.ExpandUrl(m.Groups[2].Value) + m.Groups[3].Value);
        }

        /// <summary>
        /// 指定された短縮URLサービスを使用してURLを短縮します
        /// </summary>
        /// <param name="shortenerType">使用する短縮URLサービス</param>
        /// <param name="srcUrl">短縮するURL</param>
        /// <returns>短縮されたURL</returns>
        public string ShortenUrl(MyCommon.UrlConverter shortenerType, string srcUrl)
        {
            srcUrl = MyCommon.urlEncodeMultibyteChar(srcUrl);

            // 既に短縮されている状態のURLであれば短縮しない
            if (ShortUrlHosts.Contains(new Uri(srcUrl).Host))
                return srcUrl;

            string result;
            switch (shortenerType)
            {
                case MyCommon.UrlConverter.TinyUrl:
                    result = this.ShortenByTinyUrl(srcUrl);
                    break;
                case MyCommon.UrlConverter.Isgd:
                    result = this.ShortenByIsgd(srcUrl);
                    break;
                case MyCommon.UrlConverter.Twurl:
                    result = this.ShortenByTwurl(srcUrl);
                    break;
                case MyCommon.UrlConverter.Bitly:
                    result = this.ShortenByBitly(srcUrl, "bit.ly");
                    break;
                case MyCommon.UrlConverter.Jmp:
                    result = this.ShortenByBitly(srcUrl, "j.mp");
                    break;
                case MyCommon.UrlConverter.Uxnu:
                    result = this.ShortenByUxnu(srcUrl);
                    break;
                default:
                    throw new ArgumentException("Unknown shortener.", "shortenerType");
            }

            // 短縮の結果逆に長くなった場合は短縮前のURLを返す
            if (srcUrl.Length < result.Length)
                result = srcUrl;

            return result;
        }

        private string ShortenByTinyUrl(string srcUrl)
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("http://tinyurl.com/xxxxxx".Length > srcUrl.Length)
                return srcUrl;

            var param = new Dictionary<string, string>
            {
                {"url", srcUrl},
            };

            string response;
            if (!this.http.PostData("http://tinyurl.com/api-create.php", param, out response))
                throw new WebApiException("Failed to create URL.", response);

            if (!Regex.IsMatch(response, @"^https?://"))
                throw new WebApiException("Failed to create URL.", response);

            return response.TrimEnd();
        }

        private string ShortenByIsgd(string srcUrl)
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("http://is.gd/xxxx".Length > srcUrl.Length)
                return srcUrl;

            var param = new Dictionary<string, string>
            {
                {"format", "simple"},
                {"url", srcUrl},
            };

            string response;
            if (!this.http.PostData("http://is.gd/create.php", param, out response))
                throw new WebApiException("Failed to create URL.", response);

            if (!Regex.IsMatch(response, @"^https?://"))
                throw new WebApiException("Failed to create URL.", response);

            return response.TrimEnd();
        }

        private string ShortenByTwurl(string srcUrl)
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("http://twurl.nl/xxxxxx".Length > srcUrl.Length)
                return srcUrl;

            var param = new Dictionary<string, string>
            {
                {"link[url]", srcUrl},
            };

            string response;
            if (!this.http.PostData("http://tweetburner.com/links", param, out response))
                throw new WebApiException("Failed to create URL.", response);

            if (!Regex.IsMatch(response, @"^https?://"))
                throw new WebApiException("Failed to create URL.", response);

            return response.TrimEnd();
        }

        private string ShortenByBitly(string srcUrl, string domain = "bit.ly")
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("http://bit.ly/xxxx".Length > srcUrl.Length)
                return srcUrl;

            // bit.ly 短縮機能実装のプライバシー問題の暫定対応
            // ログインIDとAPIキーが指定されていない場合は短縮せずにPOSTする
            // 参照: http://sourceforge.jp/projects/opentween/lists/archive/dev/2012-January/000020.html
            if (string.IsNullOrEmpty(this.BitlyId) || string.IsNullOrEmpty(this.BitlyKey))
                return srcUrl;

            var param = new Dictionary<string, string>
            {
                {"login", this.BitlyId},
                {"apiKey", this.BitlyKey},
                {"format", "txt"},
                {"domain", domain},
                {"longUrl", srcUrl},
            };

            string response;
            if (!this.http.GetData("https://api-ssl.bitly.com/v3/shorten", param, out response))
                throw new WebApiException("Failed to create URL.", response);

            if (!Regex.IsMatch(response, @"^https?://"))
                throw new WebApiException("Failed to create URL.", response);

            return response.TrimEnd();
        }

        private string ShortenByUxnu(string srcUrl)
        {
            // 明らかに長くなると推測できる場合は短縮しない
            if ("http://ux.nx/xxxxxx".Length > srcUrl.Length)
                return srcUrl;

            var param = new Dictionary<string, string>
            {
                {"format", "plain"},
                {"url", srcUrl},
            };

            string response;
            if (!this.http.GetData("http://ux.nu/api/short", param, out response))
                throw new WebApiException("Failed to create URL.", response);

            if (!Regex.IsMatch(response, @"^https?://"))
                throw new WebApiException("Failed to create URL.", response);

            return response.TrimEnd();
        }
    }
}
