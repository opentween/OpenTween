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

using System.Runtime.Serialization;

namespace OpenTween.Api.DataModel
{
    [DataContract]
    public class TwitterTextConfiguration
    {
        [DataMember(Name = "version")]
        public int Version { get; set; }

        [DataMember(Name = "maxWeightedTweetLength")]
        public int MaxWeightedTweetLength { get; set; }

        [DataMember(Name = "scale")]
        public int Scale { get; set; }

        [DataMember(Name = "defaultWeight")]
        public int DefaultWeight { get; set; }

        [DataMember(Name = "transformedURLLength")]
        public int TransformedURLLength { get; set; }

        [DataContract]
        public class CodepointRange
        {
            [DataMember(Name = "start")]
            public int Start { get; set; }

            [DataMember(Name = "end")]
            public int End { get; set; }

            [DataMember(Name = "weight")]
            public int Weight { get; set; }
        }

        [DataMember(Name = "ranges")]
        public CodepointRange[] Ranges { get; set; }

        /// <exception cref="SerializationException"/>
        public static TwitterTextConfiguration ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterTextConfiguration>(json);
        }

        public static TwitterTextConfiguration DefaultConfiguration()
        {
            // 参照: https://github.com/twitter/twitter-text/blob/v2.0.5/config/v2.json
            return new TwitterTextConfiguration
            {
                Version = 2,
                MaxWeightedTweetLength = 280,
                Scale = 100,
                DefaultWeight = 200,
                TransformedURLLength = 23,
                Ranges = new[]
                {
                    new CodepointRange { Start = 0, End = 4351, Weight = 100 },
                    new CodepointRange { Start = 8192, End = 8205, Weight = 100 },
                    new CodepointRange { Start = 8208, End = 8223, Weight = 100 },
                    new CodepointRange { Start = 8242, End = 8247, Weight = 100 },
                },
            };
        }
    }
}
