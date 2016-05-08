// OpenTween - Client of Twitter
// Copyright (c) 2016 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Api.DataModel
{
    [DataContract]
    public class TwitterRateLimits
    {
        [DataMember(Name = "rate_limit_context")]
        public TwitterRateLimitContext RateLimitContext { get; set; }

        [DataContract]
        public class TwitterRateLimitContext
        {
            [DataMember(Name = "access_token")]
            public string AccessToken { get; set; }
        }

        [DataMember(Name = "resources")]
        public IDictionary<string, IDictionary<string, TwitterRateLimit>> Resources { get; set; }

        [DataContract]
        public class TwitterRateLimit
        {
            [DataMember(Name = "limit")]
            public int Limit { get; set; }

            [DataMember(Name = "remaining")]
            public int Remaining { get; set; }

            [DataMember(Name = "reset")]
            public long Reset { get; set; }
        }

        /// <exception cref="SerializationException"/>
        public static TwitterRateLimits ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterRateLimits>(json);
        }
    }
}
