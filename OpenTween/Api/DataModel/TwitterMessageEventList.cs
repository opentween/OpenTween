// OpenTween - Client of Twitter
// Copyright (c) 2018 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

#nullable enable annotations

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Api.DataModel
{
    [DataContract]
    public class TwitterMessageEventList
    {
        [DataMember(Name = "apps")]
        public Dictionary<string, App> Apps { get; set; }

        [DataContract]
        public class App
        {
            [DataMember(Name = "id")]
            public string Id { get; set; }

            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "url")]
            public string Url { get; set; }
        }

        [DataMember(Name = "events")]
        public TwitterMessageEvent[] Events { get; set; }

        [DataMember(Name = "next_cursor", IsRequired = false)]
        public string? NextCursor { get; set; }
    }

    [DataContract]
    public class TwitterMessageEventSingle
    {
        [DataMember(Name = "event")]
        public TwitterMessageEvent Event { get; set; }
    }
}
