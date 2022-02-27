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
using System.Runtime.Serialization;

namespace OpenTween.Api.DataModel
{
    [DataContract]
    public class TwitterMessageEvent
    {
        [DataMember(Name = "created_timestamp")]
        public string CreatedTimestamp { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "message_create")]
        public TwitterMessageEventCreate MessageCreate { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
    }

    [DataContract]
    public class TwitterMessageEventCreate
    {
        [DataMember(Name = "message_data")]
        public Data MessageData { get; set; }

        [DataContract]
        public class Data
        {
            [DataMember(Name = "attachment", IsRequired = false)]
            public MessageAttachment? Attachment { get; set; }

            [DataContract]
            public class MessageAttachment
            {
                [DataMember(Name = "type")]
                public string Type { get; set; }

                [DataMember(Name = "media")]
                public TwitterEntityMedia Media { get; set; }
            }

            [DataMember(Name = "text")]
            public string Text { get; set; }

            [DataMember(Name = "entities")]
            public TwitterEntities Entities { get; set; }
        }

        [DataMember(Name = "sender_id")]
        public string SenderId { get; set; }

        [DataMember(Name = "source_app_id", IsRequired = false)]
        public string? SourceAppId { get; set; }

        [DataMember(Name = "target")]
        public MessageTarget Target { get; set; }

        [DataContract]
        public class MessageTarget
        {
            [DataMember(Name = "recipient_id")]
            public string RecipientId { get; set; }
        }
    }
}
