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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

#nullable enable annotations

namespace OpenTween.Api.DataModel
{
    // 参照: https://developer.twitter.com/en/docs/tweets/data-dictionary/overview/entities-object

    [DataContract]
    public class TwitterEntities : IEnumerable<TwitterEntity>
    {
        [DataMember(Name = "hashtags", IsRequired = false)]
        public TwitterEntityHashtag[]? Hashtags { get; set; }

        [DataMember(Name = "media", IsRequired = false)]
        public TwitterEntityMedia[]? Media { get; set; }

        [DataMember(Name = "symbols", IsRequired = false)]
        public TwitterEntitySymbol[]? Symbols { get; set; }

        [DataMember(Name = "urls", IsRequired = false)]
        public TwitterEntityUrl[]? Urls { get; set; }

        [DataMember(Name = "user_mentions", IsRequired = false)]
        public TwitterEntityMention[]? UserMentions { get; set; }

        public IEnumerator<TwitterEntity> GetEnumerator()
        {
            var entities = Enumerable.Empty<TwitterEntity>();

            if (this.Hashtags != null)
                entities = entities.Concat(this.Hashtags);

            if (this.Media != null)
                entities = entities.Concat(this.Media);

            if (this.Symbols != null)
                entities = entities.Concat(this.Symbols);

            if (this.Urls != null)
                entities = entities.Concat(this.Urls);

            if (this.UserMentions != null)
                entities = entities.Concat(this.UserMentions);

            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
