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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween.Thumbnail.Services
{
    public class ImgAzyobuziNet : IThumbnailService, IDisposable
    {
        protected string[] apiHosts =
        {
            "https://img.azyobuzi.net/api/",
            "https://img.opentween.org/api/",
        };

        protected string[] excludedServiceNames =
        {
            "Instagram",
            "Twitter",
            "Tumblr",
            "Gyazo",
        };

        protected string? apiBase;
        protected IEnumerable<Regex>? urlRegex = null;
        protected AsyncTimer updateTimer;

        protected HttpClient Http
            => this.localHttpClient ?? Networking.Http;

        private readonly HttpClient? localHttpClient;

        private readonly object lockObj = new();

        public ImgAzyobuziNet(bool autoupdate)
            : this(null, autoupdate)
        {
        }

        public ImgAzyobuziNet(HttpClient? http)
            : this(http, autoupdate: false)
        {
        }

        public ImgAzyobuziNet(HttpClient? http, bool autoupdate)
        {
            this.updateTimer = new AsyncTimer(this.LoadRegexAsync);
            this.AutoUpdate = autoupdate;

            this.Enabled = true;
            this.DisabledInDM = true;

            this.localHttpClient = http;
        }

        public bool AutoUpdate
        {
            get => this.autoUpdate;
            set
            {
                if (value)
                    this.StartAutoUpdate();
                else
                    this.StopAutoUpdate();

                this.autoUpdate = value;
            }
        }

        private bool autoUpdate = false;

        /// <summary>
        /// img.azyobizi.net によるサムネイル情報の取得を有効にするか
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// ダイレクトメッセージに含まれる画像URLに対して img.azyobuzi.net を使用しない
        /// </summary>
        public bool DisabledInDM { get; set; }

        protected void StartAutoUpdate()
            => this.updateTimer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(30)); // 30分おきに更新

        protected void StopAutoUpdate()
            => this.updateTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        public async Task LoadRegexAsync()
        {
            foreach (var host in this.apiHosts)
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
            lock (this.lockObj)
            {
                this.urlRegex = null;
                this.apiBase = null;
            }
        }

        public async Task<bool> LoadRegexAsync(string apiBase)
        {
            try
            {
                var jsonBytes = await this.FetchRegexAsync(apiBase)
                    .ConfigureAwait(false);

                using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(jsonBytes, XmlDictionaryReaderQuotas.Max);
                var xElm = XElement.Load(jsonReader);

                if (xElm.Element("error") != null)
                    return false;

                lock (this.lockObj)
                {
                    this.urlRegex = xElm.Elements("item")
                        .Where(x => !this.excludedServiceNames.Contains(x.Element("name").Value))
                        .Select(e => new Regex(e.Element("regex").Value, RegexOptions.IgnoreCase))
                        .ToArray();

                    this.apiBase = apiBase;
                }

                return true;
            }
            catch (HttpRequestException)
            {
                // サーバーが2xx以外のステータスコードを返した場合
            }
            catch (OperationCanceledException)
            {
                // リクエストがタイムアウトした場合
            }
            catch (XmlException)
            {
                // サーバーが不正なJSONを返した場合
            }

            return false;
        }

        protected virtual async Task<byte[]> FetchRegexAsync(string apiBase)
        {
            using var cts = new CancellationTokenSource(millisecondsDelay: 1000);
            using var response = await this.Http.GetAsync(apiBase + "regex.json", cts.Token)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync()
                .ConfigureAwait(false);
        }

        public override Task<ThumbnailInfo?> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            return Task.Run(() =>
            {
                if (!this.Enabled)
                    return null;

                if (this.DisabledInDM && post != null && post.IsDm)
                    return null;

                lock (this.lockObj)
                {
                    if (this.urlRegex == null)
                        return null;

                    foreach (var regex in this.urlRegex)
                    {
                        if (regex.IsMatch(url))
                        {
                            return new ThumbnailInfo
                            {
                                MediaPageUrl = url,
                                ThumbnailImageUrl = this.apiBase + "redirect?size=large&uri=" + Uri.EscapeDataString(url),
                                FullSizeImageUrl = this.apiBase + "redirect?size=full&uri=" + Uri.EscapeDataString(url),
                                TooltipText = null,
                            };
                        }
                    }
                }

                return null;
            },
            token);
        }

        public virtual void Dispose()
            => this.updateTimer.Dispose();
    }
}
