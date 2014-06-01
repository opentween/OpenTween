// OpenTween - Client of Twitter
// Copyright (c) 2014 spx (@5px)
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
    // 参照: https://dev.twitter.com/docs/platform-objects/entities
    //       https://dev.twitter.com/docs/api/multiple-media-extended-entities

    [DataContract]
    public class TwitterUploadMediaResult
    {
        [DataContract]
        public class ImageInfo
        {
            [DataMember(Name = "w")]
            public int Width { get; set; }

            [DataMember(Name = "h")]
            public int Height { get; set; }

            [DataMember(Name = "image_type")]
            public string ImageType { get; set; }
        }

        [DataMember(Name = "image")]
        public TwitterUploadMediaResult.ImageInfo MediaInfo { get; set; }

        [DataMember(Name = "media_id")]
        public long MediaId { get; set; }

        [DataMember(Name = "media_id_string")]
        public string MediaIdStr { get; set; }

        [DataMember(Name = "size")]
        public long Size { get; set; }

        /// <exception cref="SerializationException"/>
        public static TwitterUploadMediaResult ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterUploadMediaResult>(json);
        }
    }
}
