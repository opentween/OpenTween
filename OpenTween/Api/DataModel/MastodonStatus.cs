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
    public class MastodonStatus
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "account")]
        public MastodonAccount Account { get; set; }

        [DataMember(Name = "in_reply_to_id")]
        public long? InReplyToId { get; set; }

        [DataMember(Name = "in_reply_to_account_id")]
        public long? InReplyToAccountId { get; set; }

        [DataMember(Name = "reblog")]
        public MastodonStatus Reblog { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "reblogs_count")]
        public int ReblogsCount { get; set; }

        [DataMember(Name = "favourites_count")]
        public int FavouritesCount { get; set; }

        [DataMember(Name = "reblogged")]
        public bool? Reblogged { get; set; }

        [DataMember(Name = "favourited")]
        public bool? Favourited { get; set; }

        [DataMember(Name = "sensitive")]
        public bool? Sensitive { get; set; }

        [DataMember(Name = "spoiler_text")]
        public string SpoilerText { get; set; }

        [DataMember(Name = "visibility")]
        public string Visibility { get; set; }

        [DataMember(Name = "media_attachments")]
        public MastodonAttachment[] MediaAttachments { get; set; }

        [DataMember(Name = "mentions")]
        public MastodonMention[] Mentions { get; set; }

        [DataMember(Name = "tags")]
        public MastodonTag[] Tags { get; set; }

        [DataMember(Name = "application")]
        public MastodonApplication Application { get; set; }
    }
}
