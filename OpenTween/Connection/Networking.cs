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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween.Connection
{
    public static class Networking
    {
        public static TimeSpan DefaultTimeout { get; set; }

        /// <summary>
        /// 通信に使用するプロキシの種類
        /// </summary>
        public static ProxyType ProxyType
        {
            get { return proxyType; }
        }

        /// <summary>
        /// 通信に使用するプロキシ
        /// </summary>
        public static IWebProxy Proxy
        {
            get { return proxy; }
        }

        /// <summary>
        /// OpenTween 内で共通して使用する HttpClient インスタンス
        /// </summary>
        public static HttpClient Http
        {
            get { return globalHttpClient; }
        }

        /// <summary>
        /// Webプロキシの設定が変更された場合に発生します
        /// </summary>
        public static event EventHandler WebProxyChanged;

        private static bool initialized = false;
        private static HttpClient globalHttpClient;
        private static ProxyType proxyType = ProxyType.IE;
        private static IWebProxy proxy = null;

        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        static Networking()
        {
            DefaultTimeout = TimeSpan.FromSeconds(20);
            globalHttpClient = CreateHttpClient(new HttpClientHandler());
        }

        /// <summary>
        /// ネットワーク接続前に行う処理。起動時に一回だけ実行する必要があります。
        /// </summary>
        public static void Initialize()
        {
            Networking.initialized = true;

            ServicePointManager.Expect100Continue = false;
        }

        public static void SetWebProxy(ProxyType proxyType, string proxyAddress, int proxyPort,
            string proxyUser, string proxyPassword)
        {
            IWebProxy proxy;
            switch (proxyType)
            {
                case ProxyType.None:
                    proxy = null;
                    break;
                case ProxyType.Specified:
                    proxy = new WebProxy(proxyAddress, proxyPort);
                    if (!string.IsNullOrEmpty(proxyUser) || !string.IsNullOrEmpty(proxyPassword))
                        proxy.Credentials = new NetworkCredential(proxyUser, proxyPassword);
                    break;
                case ProxyType.IE:
                default:
                    proxy = WebRequest.GetSystemWebProxy();
                    break;
            }

            Networking.proxyType = proxyType;
            Networking.proxy = proxy;

            NativeMethods.SetProxy(proxyType, proxyAddress, proxyPort, proxyUser, proxyPassword);

            OnWebProxyChanged(EventArgs.Empty);
        }

        /// <summary>
        /// プロキシ等の設定を施した HttpClient インスタンスを生成します
        /// </summary>
        /// <remarks>
        /// 通常は Networking.Http を使用すべきです。
        /// このメソッドを使用する場合は、WebProxyChanged イベントが発生する度に HttpClient を生成し直すように実装してください。
        /// </remarks>
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        public static HttpClient CreateHttpClient(HttpClientHandler handler)
        {
            if (Networking.Proxy != null)
            {
                handler.UseProxy = true;
                handler.Proxy = Networking.Proxy;
            }
            else
            {
                handler.UseProxy = false;
            }

            var client = new HttpClient(handler);
            client.Timeout = Networking.DefaultTimeout;
            client.DefaultRequestHeaders.Add("User-Agent", Networking.GetUserAgentString());

            return client;
        }

        public static string GetUserAgentString(bool fakeMSIE = false)
        {
            if (fakeMSIE)
                return MyCommon.GetAssemblyName() + "/" + MyCommon.FileVersion + " (compatible; MSIE 10.0)";
            else
                return MyCommon.GetAssemblyName() + "/" + MyCommon.FileVersion;
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
            var newClient = Networking.CreateHttpClient(new HttpClientHandler());
            var oldClient = Interlocked.Exchange(ref globalHttpClient, newClient);
            oldClient.Dispose();

            if (WebProxyChanged != null)
                WebProxyChanged(null, e);
        }
    }

    public enum ProxyType
    {
        None,
        IE,
        Specified,
    }
}
