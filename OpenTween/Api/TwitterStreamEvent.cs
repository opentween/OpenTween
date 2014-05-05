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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Api
{
    [DataContract]
    public class TwitterStreamEvent
    {
        [DataMember(Name = "target")]
        public TwitterUser Target { get; set; }

        [DataMember(Name = "source")]
        public TwitterUser Source { get; set; }

        [DataMember(Name = "event")]
        public string Event { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        /// <exception cref="SerializationException"/>
        public static TwitterStreamEvent ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterStreamEvent>(json);
        }
    }

    [DataContract]
    public class TwitterStreamEvent<T> : TwitterStreamEvent
    {
        [DataMember(Name = "target_object")]
        public T TargetObject { get; set; }

        /// <exception cref="SerializationException"/>
        public static new TwitterStreamEvent<T> ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterStreamEvent<T>>(json);
        }
    }

    [DataContract]
    public class TwitterStreamEventDirectMessage
    {
        [DataMember(Name = "direct_message")]
        public TwitterDirectMessage DirectMessage;

        /// <exception cref="SerializationException"/>
        public static TwitterStreamEventDirectMessage ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterStreamEventDirectMessage>(json);
        }
    }
}
