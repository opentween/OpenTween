// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
//           (c) 2013      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Runtime.Serialization;
using System.Collections.Generic;
using OpenTween.Api;

namespace OpenTween.Api
{
    public class TwitterDataModel
    {
        [DataContract]
        public class Annotations
        {
            [DataMember(Name = "ConversationRole", IsRequired = false)] public string ConversationRole;
            [DataMember(Name = "FromUser", IsRequired = false)] public string FromUser;
        }

        [DataContract]
        public class Friendsevent
        {
            [DataMember(Name = "friends")] public Int64[] Friends;
        }

        [DataContract]
        public class DeletedStatusContent
        {
            [DataMember(Name = "id")] public Int64 Id;
            [DataMember(Name = "user_id")] public Int64 UserId;
        }

        [DataContract]
        public class DeletedStatus
        {
            [DataMember(Name = "status")] public DeletedStatusContent Status;
        }

        [DataContract]
        public class DeleteEvent
        {
            [DataMember(Name = "delete")] public DeletedStatus Event;
        }

        [DataContract]
        public class DeletedDirectmessage
        {
            [DataMember(Name = "direct_message")] public DeletedStatusContent Directmessage;
        }

        [DataContract]
        public class DeleteDirectmessageEvent
        {
            [DataMember(Name = "delete")] public DeletedDirectmessage Event;
        }
        [DataContract]
        public class DirectmessageEvent
        {
            [DataMember(Name = "direct_message")] public TwitterDirectMessage Directmessage;
        }

        [DataContract]
        public class TrackCount
        {
            [DataMember(Name = "track")] public int Track;
        }

        [DataContract]
        public class LimitEvent
        {
            [DataMember(Name = "limit")] public TrackCount Limit;
        }

        [DataContract]
        public class RelatedTweet
        {
            [DataMember(Name = "annotations")] public Annotations Annotations;
            [DataMember(Name = "kind")] public string Kind;
            [DataMember(Name = "score")] public double Score;
            [DataMember(Name = "value")] public TwitterStatus Status;
        }

        [DataContract]
        public class RelatedResult
        {
            [DataMember(Name = "annotations")] public Annotations Annotations;
            [DataMember(Name = "groupName")] public string GroupName;
            [DataMember(Name = "resultType")] public string ResultType;
            [DataMember(Name = "results")] public RelatedTweet[] Results;
            [DataMember(Name = "score")] public double Score;
        }

        [DataContract]
        public class RateLimitStatus
        {
            [DataMember(Name = "reset_time_in_seconds")] public int ResetTimeInSeconds;
            [DataMember(Name = "remaining_hits")] public int RemainingHits;
            [DataMember(Name = "reset_time")] public string ResetTime;
            [DataMember(Name = "hourly_limit")] public int HourlyLimit;
            [DataMember(Name = "photos")] public MediaRateLimitStatus Photos;
        }

        [DataContract]
        public class MediaRateLimitStatus
        {
            [DataMember(Name = "reset_time_in_seconds")] public int RestTimeInSeconds;
            [DataMember(Name = "remaining_hits")] public int RemainingHits;
            [DataMember(Name = "reset_time")] public string ResetTime;
            [DataMember(Name = "daily_limit")] public int DailyLimit;
        }
    }
}
