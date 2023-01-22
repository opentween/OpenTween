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
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween.Connection
{
    public class TwitterComCookieHandler : DelegatingHandler
    {
        public string RawCookie { get; private set; } = "";

        public string CsrfToken { get; private set; } = "";

        public string AuthToken { get; private set; } = "";

        private readonly ApiKey bearerToken;

        public TwitterComCookieHandler(HttpMessageHandler innerHandler, ApiKey bearerToken)
            : base(innerHandler)
        {
            this.bearerToken = bearerToken;
        }

        public TwitterComCookieHandler(HttpMessageHandler innerHandler, string rawCookie)
            : base(innerHandler)
        {
            this.bearerToken = ApplicationSettings.TwitterComBearerToken;
            this.SetCookie(rawCookie);
        }

        public void SetCookie(string rawCookie)
        {
            this.RawCookie = rawCookie;

            var pairs = rawCookie.Split(';')
                .Select(x => x.Trim().Split(new[] { '=' }, 2))
                .Where(x => x.Length == 2)
                .Select(x => new KeyValuePair<string, string>(x[0], x[1]));

            string? GetValue(string key)
                => pairs.Where(x => x.Key == key).Select(x => x.Value).FirstOrDefault();

            this.CsrfToken = GetValue("ct0") ?? "";
            this.AuthToken = GetValue("auth_token") ?? "";
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!MyCommon.IsNullOrEmpty(this.RawCookie))
            {
                request.Headers.Add("x-twitter-auth-type", "OAuth2Session");
                request.Headers.Add("x-csrf-token", this.CsrfToken);
                request.Headers.Add("cookie", this.GenerateCookieValue());
                request.Headers.Authorization = new("Bearer", this.bearerToken.Value);
            }

            return await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }

        private string GenerateCookieValue()
            => $"ct0={this.CsrfToken}; auth_token={this.AuthToken}";
    }
}
