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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween.Api.GraphQL
{
    public class DeleteTweetRequest
    {
        private static readonly Uri EndpointUri = new("https://twitter.com/i/api/graphql/VaenaVgh5q5ih7kvyVjgtg/DeleteTweet");

        required public TwitterStatusId TweetId { get; set; }

        public string CreateRequestBody()
        {
            return $$"""
            {"variables":{"tweet_id":"{{JsonUtils.EscapeJsonString(this.TweetId.Id)}}","dark_request":false},"queryId":"VaenaVgh5q5ih7kvyVjgtg"}
            """;
        }

        public async Task Send(IApiConnection apiConnection)
        {
            var json = this.CreateRequestBody();
            var responseText = await apiConnection.PostJsonAsync(EndpointUri, json);
            ErrorResponse.ThrowIfError(responseText);
        }
    }
}
