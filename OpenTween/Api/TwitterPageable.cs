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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Api
{
    [DataContract]
    public abstract class TwitterPageable<T>
    {
        public abstract T[] Items { get; }

        [DataMember(Name = "next_cursor")]
        public long NextCursor { get; set; }

        [DataMember(Name = "next_cursor_str")]
        public string NextCursorStr { get; set; }

        [DataMember(Name = "previous_cursor")]
        public long PreviousCursor { get; set; }

        [DataMember(Name = "previous_cursor_str")]
        public string PreviousCursorStr { get; set; }
    }

    [DataContract]
    public class TwitterIds : TwitterPageable<long>
    {
        [DataMember(Name = "ids")]
        public long[] Ids { get; set; }

        [IgnoreDataMember]
        public override long[] Items
        {
            get { return this.Ids; }
        }

        /// <exception cref="SerializationException"/>
        public static TwitterIds ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterIds>(json);
        }
    }

    [DataContract]
    public class TwitterUsers : TwitterPageable<TwitterUser>
    {
        [DataMember(Name = "users")]
        public TwitterUser[] Users { get; set; }

        [IgnoreDataMember]
        public override TwitterUser[] Items
        {
            get { return this.Users; }
        }

        /// <exception cref="SerializationException"/>
        public static TwitterUsers ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterUsers>(json);
        }
    }

    [DataContract]
    public class TwitterLists : TwitterPageable<TwitterList>
    {
        [DataMember(Name = "lists")]
        public TwitterList[] Lists { get; set; }

        [IgnoreDataMember]
        public override TwitterList[] Items
        {
            get { return this.Lists; }
        }

        /// <exception cref="SerializationException"/>
        public static TwitterLists ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterLists>(json);
        }
    }
}
