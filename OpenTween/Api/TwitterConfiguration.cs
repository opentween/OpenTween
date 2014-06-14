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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Api
{
    [DataContract]
    public class TwitterConfiguration
    {
        [DataMember(Name = "characters_reserved_per_media")]
        public int CharactersReservedPerMedia { get; set; }

        [DataMember(Name = "photo_size_limit")]
        public long PhotoSizeLimit { get; set; }

        [DataMember(Name = "photo_sizes")]
        public TwitterMediaSizes PhotoSizes { get; set; }

        [DataMember(Name = "non_username_paths")]
        public string[] NonUsernamePaths { get; set; }

        [DataMember(Name = "short_url_length")]
        public int ShortUrlLength { get; set; }

        [DataMember(Name = "short_url_length_https")]
        public int ShortUrlLengthHttps { get; set; }

        [DataMember(Name = "max_media_per_upload")]
        public int MaxMediaPerUpload { get; set; }

        /// <exception cref="SerializationException"/>
        public static TwitterConfiguration ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterConfiguration>(json);
        }

        /// <summary>
        /// 設定が取得できるまでの間に代わりに使用する適当な値を返します
        /// </summary>
        public static TwitterConfiguration DefaultConfiguration()
        {
            return new TwitterConfiguration
            {
                CharactersReservedPerMedia = 20,
                ShortUrlLength = 19,
                ShortUrlLengthHttps = 20,
            };
        }
    }
}
