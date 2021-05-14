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
    public class MastodonAttachment
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "remote_url")]
        public string RemoteUrl { get; set; }

        [DataMember(Name = "preview_url")]
        public string PreviewUrl { get; set; }

        [DataMember(Name = "text_url")]
        public string TextUrl { get; set; }
    }
}
