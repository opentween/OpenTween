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
using System.Net.Http;
using System.Text;

namespace OpenTween.Connection
{
    public class PostJsonRequest : IHttpRequest
    {
        public required Uri RequestUri { get; set; }

        public required string JsonString { get; set; }

        public string? EndpointName { get; set; }

        public TimeSpan Timeout { get; set; } = Networking.DefaultTimeout;

        public HttpRequestMessage CreateMessage(Uri baseUri)
            => new()
            {
                Method = HttpMethod.Post,
                RequestUri = new(baseUri, this.RequestUri),
                Content = new StringContent(this.JsonString, Encoding.UTF8, "application/json"),
            };
    }
}
