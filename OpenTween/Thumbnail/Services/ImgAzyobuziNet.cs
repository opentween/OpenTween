// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Connection;

namespace OpenTween.Thumbnail.Services
{
    class ImgAzyobuziNet : IThumbnailService, IDisposable
    {
        protected string[] ApiHosts = {
            "http://img.azyobuzi.net/api/",
            "http://img.opentween.org/api/",
        };

        protected string ApiBase;
        protected IEnumerable<Regex> UrlRegex = null;
        protected Timer UpdateTimer;

        protected HttpClient http
        {
            get { return this.localHttpClient ?? Networking.Http; }
        }
        private readonly HttpClient localHttpClient;

        private object LockObj = new object();

        public ImgAzyobuziNet(bool autoupdate)
            : this(null, autoupdate)
        {
        }

        public ImgAzyobuziNet(HttpClient http)
            : this(http, autoupdate: false)
        {
        }

        public ImgAzyobuziNet(HttpClient http, bool autoupdate)
        {
            this.UpdateTimer = new Timer(async _ => await this.LoadRegexAsync());
            this.AutoUpdate = autoupdate;

            this.Enabled = true;
            this.DisabledInDM = true;

            this.localHttpClient = http;
        }

        public bool AutoUpdate
        {
            get { return this._AutoUpdate; }
            set
            {
                if (value)
                    this.StartAutoUpdate();
                else
                    this.StopAutoUpdate();

                this._AutoUpdate = value;
            }
        }
        private bool _AutoUpdate = false;

        /// <summary>
        /// img.azyobizi.net によるサムネイル情報の取得を有効にするか
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// ダイレクトメッセージに含まれる画像URLに対して img.azyobuzi.net を使用しない
        /// </summary>
        public bool DisabledInDM { get; set; }

        protected void StartAutoUpdate()
        {
            this.UpdateTimer.Change(0, 30 * 60 * 1000); ; // 30分おきに更新
        }

        protected void StopAutoUpdate()
        {
            this.UpdateTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public async Task LoadRegexAsync()
        {
            foreach (var host in this.ApiHosts)
            {
                try
                {
                    var result = await this.LoadRegexAsync(host)
                        .ConfigureAwait(false);

                    if (result) return;
                }
                catch (Exception)
                {
#if DEBUG
                    throw;
#endif
                }
            }

            // どのサーバーも使用できない場合
            lock (this.LockObj)
            {
                this.UrlRegex = null;
                this.ApiBase = null;
            }
        }

        public async Task<bool> LoadRegexAsync(string apiBase)
        {
            try
            {
                var jsonBytes = await this.FetchRegexAsync(apiBase)
                    .ConfigureAwait(false);

                using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(jsonBytes, XmlDictionaryReaderQuotas.Max))
                {
                    var xElm = XElement.Load(jsonReader);

                    if (xElm.Element("error") != null)
                        return false;

                    lock (this.LockObj)
                    {
                        this.UrlRegex = xElm.Elements("item")
                            .Where(x => x.Element("name").Value != "Tumblr") // Tumblrのサムネイル表示には img.azyobuzi.net を使用しない
                            .Select(e => new Regex(e.Element("regex").Value, RegexOptions.IgnoreCase))
                            .ToArray();

                        this.ApiBase = apiBase;
                    }
                }

                return true;
            }
            catch (HttpRequestException) { } // サーバーが2xx以外のステータスコードを返した場合
            catch (OperationCanceledException) { } // リクエストがタイムアウトした場合
            catch (XmlException) { } // サーバーが不正なJSONを返した場合

            return false;
        }

        protected virtual async Task<byte[]> FetchRegexAsync(string apiBase)
        {
            using (var cts = new CancellationTokenSource(millisecondsDelay: 1000))
            using (var response = await this.http.GetAsync(apiBase + "regex.json", cts.Token)
                .ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync()
                    .ConfigureAwait(false);
            }
        }

        public override Task<ThumbnailInfo> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            return Task.Run(() =>
            {
                if (!this.Enabled)
                    return null;

                if (this.DisabledInDM && post != null && post.IsDm)
                    return null;

                lock (this.LockObj)
                {
                    if (this.UrlRegex == null)
                        return null;

                    foreach (var regex in this.UrlRegex)
                    {
                        if (regex.IsMatch(url))
                        {
                            return new ThumbnailInfo
                            {
                                ImageUrl = url,
                                ThumbnailUrl = this.ApiBase + "redirect?size=large&uri=" + Uri.EscapeDataString(url),
                                FullSizeImageUrl = this.ApiBase + "redirect?size=full&uri=" + Uri.EscapeDataString(url),
                                TooltipText = null,
                            };
                        }
                    }
                }

                return null;
            }, token);
        }

        public virtual void Dispose()
        {
            this.UpdateTimer.Dispose();
        }
    }
}
