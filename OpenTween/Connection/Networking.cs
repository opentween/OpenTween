// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween.Connection
{
    public static class Networking
    {
        public static TimeSpan DefaultTimeout { get; set; }

        public static TimeSpan UploadImageTimeout { get; set; }

        /// <summary>
        /// 通信に使用するプロキシの種類
        /// </summary>
        public static ProxyType ProxyType { get; private set; } = ProxyType.IE;

        /// <summary>
        /// 通信に使用するプロキシ
        /// </summary>
        public static IWebProxy? Proxy { get; private set; } = null;

        /// <summary>
        /// OpenTween 内で共通して使用する HttpClient インスタンス
        /// </summary>
        public static HttpClient Http => globalHttpClient;

        /// <summary>
        /// pbs.twimg.com で IPv4 を強制的に使用する
        /// </summary>
        public static bool ForceIPv4
        {
            get => forceIPv4;
            set
            {
                if (forceIPv4 == value)
                    return;

                forceIPv4 = value;

                // Network.Http を再作成させる
                OnWebProxyChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Webプロキシの設定が変更された場合に発生します
        /// </summary>
        public static event EventHandler? WebProxyChanged;

        private static bool initialized = false;
        private static HttpClient globalHttpClient;
        private static bool forceIPv4 = false;

        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        static Networking()
        {
            DefaultTimeout = TimeSpan.FromSeconds(20);
            UploadImageTimeout = TimeSpan.FromSeconds(60);
            globalHttpClient = CreateHttpClient(new HttpClientHandler());
        }

        /// <summary>
        /// ネットワーク接続前に行う処理。起動時に一回だけ実行する必要があります。
        /// </summary>
        public static void Initialize()
        {
            Networking.initialized = true;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.CheckCertificateRevocationList = true;
        }

        public static void SetWebProxy(
            ProxyType proxyType,
            string proxyAddress,
            int proxyPort,
            string proxyUser,
            string proxyPassword)
        {
            IWebProxy? proxy;
            switch (proxyType)
            {
                case ProxyType.None:
                    proxy = null;
                    break;
                case ProxyType.Specified:
                    proxy = new WebProxy(proxyAddress, proxyPort);
                    if (!MyCommon.IsNullOrEmpty(proxyUser) || !MyCommon.IsNullOrEmpty(proxyPassword))
                        proxy.Credentials = new NetworkCredential(proxyUser, proxyPassword);
                    break;
                case ProxyType.IE:
                default:
                    proxy = WebRequest.GetSystemWebProxy();
                    break;
            }

            Networking.ProxyType = proxyType;
            Networking.Proxy = proxy;

            NativeMethods.SetProxy(proxyType, proxyAddress, proxyPort);

            OnWebProxyChanged(EventArgs.Empty);
        }

        /// <summary>
        /// OpenTween で必要な設定を施した HttpClientHandler インスタンスを生成します
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        public static WebRequestHandler CreateHttpClientHandler()
        {
            var handler = new WebRequestHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            if (Networking.Proxy != null)
            {
                handler.UseProxy = true;
                handler.Proxy = Networking.Proxy;
            }
            else
            {
                handler.UseProxy = false;
            }

            return handler;
        }

        /// <summary>
        /// OpenTween で必要な設定を施した HttpClient インスタンスを生成します
        /// </summary>
        /// <remarks>
        /// 通常は Networking.Http を使用すべきです。
        /// このメソッドを使用する場合は、WebProxyChanged イベントが発生する度に HttpClient を生成し直すように実装してください。
        /// </remarks>
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        public static HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            HttpClient client;
            if (ForceIPv4)
                client = new HttpClient(new ForceIPv4Handler(handler));
            else
                client = new HttpClient(handler);

            client.Timeout = Networking.DefaultTimeout;

            return client;
        }

        public static string GetUserAgentString(bool fakeMSIE = false)
        {
            if (fakeMSIE)
                return ApplicationSettings.AssemblyName + "/" + MyCommon.FileVersion + " (compatible; MSIE 10.0)";
            else
                return ApplicationSettings.AssemblyName + "/" + MyCommon.FileVersion;
        }

        /// <summary>
        /// Initialize() メソッドが事前に呼ばれているか確認します
        /// </summary>
        internal static void CheckInitialized()
        {
            if (!Networking.initialized)
                throw new InvalidOperationException("Sequence error.(not initialized)");
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        private static void OnWebProxyChanged(EventArgs e)
        {
            var newClient = Networking.CreateHttpClient(Networking.CreateHttpClientHandler());
            var oldClient = Interlocked.Exchange(ref globalHttpClient, newClient);
            oldClient.Dispose();

            WebProxyChanged?.Invoke(null, e);
        }

        private class ForceIPv4Handler : DelegatingHandler
        {
            private readonly IPAddress? ipv4Address;

            public ForceIPv4Handler(HttpMessageHandler innerHandler)
                : base(innerHandler)
            {
                foreach (var address in Dns.GetHostAddresses("pbs.twimg.com"))
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        this.ipv4Address = address;
                }
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (this.ipv4Address != null)
                {
                    var requestUri = request.RequestUri;
                    if (requestUri.Host == "pbs.twimg.com")
                    {
                        var rewriteUriStr = requestUri.GetLeftPart(UriPartial.Scheme) + this.ipv4Address + requestUri.PathAndQuery;
                        request.RequestUri = new Uri(rewriteUriStr);
                        request.Headers.Host = "pbs.twimg.com";
                    }
                }

                return base.SendAsync(request, cancellationToken);
            }
        }
    }

    public enum ProxyType
    {
        None,
        IE,
        Specified,
    }
}
