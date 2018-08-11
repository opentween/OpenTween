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

using System.Runtime.Serialization;

namespace OpenTween.Api.DataModel
{
    public interface ITwitterStreamMessage
    {
    }

    public class StreamMessageStatus : ITwitterStreamMessage
    {
        public TwitterStatusCompat Status { get; }

        public StreamMessageStatus(TwitterStatusCompat status)
            => this.Status = status;

        public static StreamMessageStatus ParseJson(string json)
            => new StreamMessageStatus(TwitterStatusCompat.ParseJson(json));
    }

    public class StreamMessageEvent : ITwitterStreamMessage
    {
        public TwitterStreamEvent Event { get; }
        public string Json { get; }

        public StreamMessageEvent(TwitterStreamEvent eventData, string json)
        {
            this.Event = eventData;
            this.Json = json;
        }

        public TwitterStreamEvent<T> ParseTargetObjectAs<T>()
            => TwitterStreamEvent<T>.ParseJson(this.Json);

        public static StreamMessageEvent ParseJson(string json)
            => new StreamMessageEvent(TwitterStreamEvent.ParseJson(json), json);
    }

    [DataContract]
    public class StreamMessageDirectMessage : ITwitterStreamMessage
    {
        [DataMember(Name = "direct_message")]
        public TwitterDirectMessage DirectMessage { get; set; }

        public static StreamMessageDirectMessage ParseJson(string json)
            => MyCommon.CreateDataFromJson<StreamMessageDirectMessage>(json);
    }

    [DataContract]
    public class StreamMessageDelete : ITwitterStreamMessage
    {
        [DataContract]
        public class DeletedId
        {
            [DataMember(Name = "id")]
            public long Id { get; set; }
        }

        [DataMember(Name = "direct_message", IsRequired = false)]
        public DeletedId DirectMessage { get; set; } // Nullable

        [DataMember(Name = "status", IsRequired = false)]
        public DeletedId Status { get; set; } // Nullable

        public static StreamMessageDelete ParseJson(string json)
            => MyCommon.CreateDataFromJson<StreamMessageDelete>(json);
    }

    [DataContract]
    public class StreamMessageScrubGeo : ITwitterStreamMessage
    {
        [DataMember(Name = "user_id")]
        public long UserId { get; set; }

        [DataMember(Name = "up_to_status_id")]
        public long UpToStatusId { get; set; }

        public static StreamMessageScrubGeo ParseJson(string json)
            => MyCommon.CreateDataFromJson<StreamMessageScrubGeo>(json);
    }

    public class StreamMessageKeepAlive : ITwitterStreamMessage
    {
    }

    public class StreamMessageUnknown : ITwitterStreamMessage
    {
        public string Json { get; }

        public StreamMessageUnknown(string json)
            => this.Json = json;
    }
}
