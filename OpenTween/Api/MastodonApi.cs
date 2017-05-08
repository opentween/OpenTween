// OpenTween - Client of Twitter
// Copyright (c) 2017 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Threading.Tasks;
using OpenTween.Api.DataModel;
using OpenTween.Connection;

namespace OpenTween.Api
{
    public sealed class MastodonApi : IDisposable
    {
        public IMastodonApiConnection Connection { get; }

        public MastodonApi(Uri instanceUri, string? accessToken)
        {
            this.Connection = new MastodonApiConnection(instanceUri, accessToken);
        }

        public Task<MastodonStatus[]> TimelinesHome(long? maxId = null, long? sinceId = null, int? limit = null)
        {
            var endpoint = new Uri("/api/v1/timelines/home", UriKind.Relative);
            var param = new Dictionary<string, string>();

            if (maxId != null)
                param["max_id"] = maxId.ToString();
            if (sinceId != null)
                param["since_id"] = sinceId.ToString();
            if (limit != null)
                param["limit"] = limit.ToString();

            return this.Connection.GetAsync<MastodonStatus[]>(endpoint, param);
        }

        public Task StatusesDelete(long statusId)
        {
            var endpoint = new Uri($"/api/v1/statuses/{statusId}", UriKind.Relative);

            return this.Connection.PostLazyAsync<object>(HttpMethod.Delete, endpoint, null).IgnoreResponse();
        }

        public Task<LazyJson<MastodonStatus>> StatusesFavourite(long statusId)
        {
            var endpoint = new Uri($"/api/v1/statuses/{statusId}/favourite", UriKind.Relative);

            return this.Connection.PostLazyAsync<MastodonStatus>(endpoint, null);
        }

        public Task<LazyJson<MastodonStatus>> StatusesUnfavourite(long statusId)
        {
            var endpoint = new Uri($"/api/v1/statuses/{statusId}/unfavourite", UriKind.Relative);

            return this.Connection.PostLazyAsync<MastodonStatus>(endpoint, null);
        }

        public Task<LazyJson<MastodonStatus>> StatusesReblog(long statusId)
        {
            var endpoint = new Uri($"/api/v1/statuses/{statusId}/reblog", UriKind.Relative);

            return this.Connection.PostLazyAsync<MastodonStatus>(endpoint, null);
        }

        public Task<LazyJson<MastodonStatus>> StatusesUnreblog(long statusId)
        {
            var endpoint = new Uri($"/api/v1/statuses/{statusId}/unreblog", UriKind.Relative);

            return this.Connection.PostLazyAsync<MastodonStatus>(endpoint, null);
        }

        public void Dispose()
            => this.Connection.Dispose();
    }
}
