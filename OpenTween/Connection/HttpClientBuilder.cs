// OpenTween - Client of Twitter
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net.Http;

namespace OpenTween.Connection
{
    public class HttpClientBuilder
    {
        private readonly List<Action<WebRequestHandler>> setupHttpClientHandler = new();
        private readonly List<Func<HttpMessageHandler, HttpMessageHandler>> customHandlers = new();
        private readonly List<Action<HttpClient>> setupHttpClient = new();

        public void SetupHttpClientHandler(Action<WebRequestHandler> func)
            => this.setupHttpClientHandler.Add(func);

        public void AddHandler(Func<HttpMessageHandler, HttpMessageHandler> func)
            => this.customHandlers.Add(func);

        public void SetupHttpClient(Action<HttpClient> func)
            => this.setupHttpClient.Add(func);

        public HttpClient Build()
        {
            WebRequestHandler? handler = null;
            HttpMessageHandler? wrappedHandler = null;
            HttpClient? client = null;
            try
            {
                handler = new();
                foreach (var setupFunc in this.setupHttpClientHandler)
                    setupFunc(handler);

                wrappedHandler = handler;
                foreach (var handlerFunc in this.customHandlers)
                    wrappedHandler = handlerFunc(wrappedHandler);

                client = new(wrappedHandler, disposeHandler: true);

                foreach (var setupFunc in this.setupHttpClient)
                    setupFunc(client);

                return client;
            }
            catch
            {
                client?.Dispose();
                wrappedHandler?.Dispose();
                handler?.Dispose();
                throw;
            }
        }
    }
}
