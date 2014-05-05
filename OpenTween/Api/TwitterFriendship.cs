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
    public class TwitterFriendship
    {
        [DataMember(Name = "relationship")]
        public TwitterFriendship.RelationshipItem Relationship { get; set; }

        [DataContract]
        public class RelationshipItem
        {
            [DataMember(Name = "target")]
            public TwitterFriendship.RelationshipTarget Target { get; set; }

            [DataMember(Name = "source")]
            public TwitterFriendship.RelationshipSource Source { get; set; }
        }

        [DataContract]
        public class RelationshipTarget
        {
            [DataMember(Name = "id")]
            public long Id { get; set; }

            [DataMember(Name = "id_str")]
            public string IdStr { get; set; }

            [DataMember(Name = "screen_name")]
            public string ScreenName { get; set; }

            [DataMember(Name = "following")]
            public bool Following { get; set; }

            [DataMember(Name = "followed_by")]
            public bool FollowedBy { get; set; }
        }

        [DataContract]
        public class RelationshipSource
        {
            [DataMember(Name = "id")]
            public long Id { get; set; }

            [DataMember(Name = "id_str")]
            public string IdStr { get; set; }

            [DataMember(Name = "screen_name")]
            public string ScreenName { get; set; }

            [DataMember(Name = "following")]
            public bool Following { get; set; }

            [DataMember(Name = "followed_by")]
            public bool FollowedBy { get; set; }

            [DataMember(Name = "marked_spam")]
            public bool? MarkedSpam { get; set; }

            [DataMember(Name = "all_replies")]
            public bool? AllReplies { get; set; }

            [DataMember(Name = "want_retweets")]
            public bool? WantRetweets { get; set; }

            [DataMember(Name = "can_dm")]
            public bool CanDm { get; set; }

            [DataMember(Name = "blocking")]
            public bool? Blocking { get; set; }
        }

        /// <exception cref="SerializationException"/>
        public static TwitterFriendship ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterFriendship>(json);
        }
    }
}
