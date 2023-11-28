// OpenTween - Client of Twitter
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Api.DataModel;

namespace OpenTween.Api.GraphQL
{
    public class TwitterGraphqlUser
    {
        public const string TypeName = "User";

        public XElement Element { get; }

        public TwitterGraphqlUser(XElement element)
        {
            var typeName = element.Element("__typename")?.Value;
            if (typeName != TypeName)
                throw new ArgumentException($"Invalid itemType: {typeName}", nameof(element));

            this.Element = element;
        }

        public TwitterUser ToTwitterUser()
        {
            try
            {
                return TwitterGraphqlUser.ParseUser(this.Element);
            }
            catch (WebApiException ex)
            {
                ex.ResponseText = JsonUtils.JsonXmlToString(this.Element);
                MyCommon.TraceOut(ex);
                throw;
            }
        }

        public static TwitterUser ParseUser(XElement userElm)
        {
            var userLegacyElm = userElm.Element("legacy") ?? throw CreateParseError();

            static string GetText(XElement elm, string name)
                => elm.Element(name)?.Value ?? throw CreateParseError();

            static string? GetTextOrNull(XElement elm, string name)
                => elm.Element(name)?.Value;

            return new()
            {
                Id = long.Parse(GetText(userElm, "rest_id")),
                IdStr = GetText(userElm, "rest_id"),
                Name = GetText(userLegacyElm, "name"),
                ProfileImageUrlHttps = GetText(userLegacyElm, "profile_image_url_https"),
                ScreenName = GetText(userLegacyElm, "screen_name"),
                Protected = GetTextOrNull(userLegacyElm, "protected") == "true",
                Verified = GetTextOrNull(userLegacyElm, "verified") == "true",
                CreatedAt = GetText(userLegacyElm, "created_at"),
                FollowersCount = int.Parse(GetText(userLegacyElm, "followers_count")),
                FriendsCount = int.Parse(GetText(userLegacyElm, "friends_count")),
                FavouritesCount = int.Parse(GetText(userLegacyElm, "favourites_count")),
                StatusesCount = int.Parse(GetText(userLegacyElm, "statuses_count")),
                Description = GetTextOrNull(userLegacyElm, "description"),
                Location = GetTextOrNull(userLegacyElm, "location"),
                Url = GetTextOrNull(userLegacyElm, "url"),
                Entities = new()
                {
                    Description = new()
                    {
                        Urls = userLegacyElm.XPathSelectElements("entities/description/urls/item")
                            .Select(x => new TwitterEntityUrl()
                            {
                                Indices = x.XPathSelectElements("indices/item").Select(x => int.Parse(x.Value)).ToArray(),
                                DisplayUrl = GetText(x, "display_url"),
                                ExpandedUrl = GetText(x, "expanded_url"),
                                Url = GetText(x, "url"),
                            })
                            .ToArray(),
                    },
                    Url = new()
                    {
                        Urls = userLegacyElm.XPathSelectElements("entities/url/urls/item")
                            .Select(x => new TwitterEntityUrl()
                            {
                                Indices = x.XPathSelectElements("indices/item").Select(x => int.Parse(x.Value)).ToArray(),
                                DisplayUrl = GetText(x, "display_url"),
                                ExpandedUrl = GetText(x, "expanded_url"),
                                Url = GetText(x, "url"),
                            })
                            .ToArray(),
                    },
                },
            };
        }

        private static Exception CreateParseError()
            => throw new WebApiException("Parse error on User");
    }
}
