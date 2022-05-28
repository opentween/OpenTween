// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;
using System.Threading.Tasks;
using OpenTween.Api.DataModel;
using OpenTween.Connection;

namespace OpenTween.Api.TwitterV2
{
    public class GetTimelineRequest
    {
        public static readonly string EndpointName = "/2/users/:id/timelines/reverse_chronological";

        public long UserId { get; set; }

        public int? MaxResults { get; set; }

        public string? UntilId { get; set; }

        public string? SinceId { get; set; }

        public GetTimelineRequest(long userId)
            => this.UserId = userId;

        private Uri CreateEndpointUri()
            => new($"/2/users/{this.UserId}/timelines/reverse_chronological", UriKind.Relative);

        private Dictionary<string, string> CreateParameters()
        {
            var param = new Dictionary<string, string>
            {
                ["tweet.fields"] = "id",
            };

            if (this.MaxResults != null)
                param["max_results"] = this.MaxResults.ToString();

            if (this.UntilId != null)
                param["until_id"] = this.UntilId;

            if (this.SinceId != null)
                param["since_id"] = this.SinceId;

            return param;
        }

        public Task<TwitterV2TweetIds> Send(IApiConnection apiConnection)
        {
            var uri = this.CreateEndpointUri();
            var param = this.CreateParameters();

            return apiConnection.GetAsync<TwitterV2TweetIds>(uri, param, EndpointName);
        }
    }
}
