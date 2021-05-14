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

#nullable enable annotations

using System.Runtime.Serialization;

namespace OpenTween.Api.DataModel
{
    [DataContract]
    public class MastodonAccount
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "acct")]
        public string Acct { get; set; }

        [DataMember(Name = "display_name")]
        public string DisplayName { get; set; }

        [DataMember(Name = "locked")]
        public bool Locked { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "followers_count")]
        public int FollowersCount { get; set; }

        [DataMember(Name = "following_count")]
        public int FollowingCount { get; set; }

        [DataMember(Name = "statuses_count")]
        public int StatusesCount { get; set; }

        [DataMember(Name = "note")]
        public string Note { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }

        [DataMember(Name = "avatar_static")]
        public string AvatarStatic { get; set; }

        [DataMember(Name = "header")]
        public string Header { get; set; }

        [DataMember(Name = "header_static")]
        public string HeaderStatic { get; set; }
    }
}
