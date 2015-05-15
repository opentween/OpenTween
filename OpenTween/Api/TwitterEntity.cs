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

namespace OpenTween.Api
{
    // 参照: https://dev.twitter.com/docs/platform-objects/entities

    [DataContract]
    public class TwitterEntities : IEnumerable<TwitterEntity>
    {
        [DataMember(Name = "hashtags", IsRequired = false)]
        public TwitterEntityHashtag[] Hashtags { get; set; }

        [DataMember(Name = "media", IsRequired = false)]
        public TwitterEntityMedia[] Media { get; set; }

        [DataMember(Name = "urls", IsRequired = false)]
        public TwitterEntityUrl[] Urls { get; set; }

        [DataMember(Name = "user_mentions", IsRequired = false)]
        public TwitterEntityMention[] UserMentions { get; set; }

        public IEnumerator<TwitterEntity> GetEnumerator()
        {
            var entities = Enumerable.Empty<TwitterEntity>();

            if (this.Hashtags != null)
                entities = entities.Concat(this.Hashtags);
            if (this.Media != null)
                entities = entities.Concat(this.Media);
            if (this.Urls != null)
                entities = entities.Concat(this.Urls);
            if (this.UserMentions != null)
                entities = entities.Concat(this.UserMentions);

            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    [DataContract]
    public abstract class TwitterEntity
    {
        [DataMember(Name = "indices")]
        public int[] Indices { get; set; }
    }

    [DataContract]
    public class TwitterEntityHashtag : TwitterEntity
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }
    }

    [DataContract]
    public class TwitterEntityMedia : TwitterEntityUrl
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "id_str")]
        public string IdStr { get; set; }

        [DataMember(Name = "media_url")]
        public string MediaUrl { get; set; }

        [DataMember(Name = "media_url_https")]
        public string MediaUrlHttps { get; set; }

        [DataMember(Name = "sizes")]
        public TwitterMediaSizes Sizes { get; set; }

        [DataMember(Name = "source_status_id")]
        public long SourceStatusId { get; set; }

        [DataMember(Name = "source_status_id_str")]
        public string SourceStatusIdStr { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "video_info", IsRequired = false)]
        public TwitterMediaVideoInfo VideoInfo { get; set; }
    }

    [DataContract]
    public class TwitterMediaSizes
    {
        [DataMember(Name = "thumb")]
        public TwitterMediaSizes.Size Thumb { get; set; }

        [DataMember(Name = "large")]
        public TwitterMediaSizes.Size Large { get; set; }

        [DataMember(Name = "medium")]
        public TwitterMediaSizes.Size Medium { get; set; }

        [DataMember(Name = "small")]
        public TwitterMediaSizes.Size Small { get; set; }

        [DataContract]
        public class Size
        {
            [DataMember(Name = "h")]
            public int Height { get; set; }

            [DataMember(Name = "resize")]
            public string Resize { get; set; }

            [DataMember(Name = "w")]
            public int Width { get; set; }
        }
    }

    [DataContract]
    public class TwitterMediaVideoInfo
    {
        [DataMember(Name = "aspect_ratio")]
        public int[] AspectRatio { get; set; }

        [DataMember(Name = "duration_millis", IsRequired = false)]
        public long? DurationMillis { get; set; }

        [DataMember(Name = "variants")]
        public TwitterMediaVideoInfo.Variant[] Variants { get; set; }

        [DataContract]
        public class Variant
        {
            [DataMember(Name = "bitrate")]
            public int? Bitrate { get; set; }

            [DataMember(Name = "content_type")]
            public string ContentType { get; set; }

            [DataMember(Name = "url")]
            public string Url { get; set; }
        }
    }

    [DataContract]
    public class TwitterEntityUrl : TwitterEntity
    {
        [DataMember(Name = "display_url")]
        public string DisplayUrl { get; set; }

        [DataMember(Name = "expanded_url")]
        public string ExpandedUrl { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }

    [DataContract]
    public class TwitterEntityMention : TwitterEntity
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "id_str")]
        public string IdStr { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }
    }
}
