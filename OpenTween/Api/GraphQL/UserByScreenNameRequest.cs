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
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Connection;

namespace OpenTween.Api.GraphQL
{
    public class UserByScreenNameRequest
    {
        private static readonly Uri EndpointUri = new("https://twitter.com/i/api/graphql/xc8f1g7BYqr6VTzTbvNlGw/UserByScreenName");

        required public string ScreenName { get; set; }

        public Dictionary<string, string> CreateParameters()
        {
            return new()
            {
                ["variables"] = $$"""
                    {"screen_name":"{{this.ScreenName}}","withSafetyModeUserFields":true}
                    """,
                ["features"] = """
                    {"hidden_profile_likes_enabled":false,"hidden_profile_subscriptions_enabled":false,"responsive_web_graphql_exclude_directive_enabled":true,"verified_phone_label_enabled":false,"subscriptions_verification_info_verified_since_enabled":true,"highlights_tweets_tab_ui_enabled":true,"creator_subscriptions_tweet_preview_api_enabled":true,"responsive_web_graphql_skip_user_profile_image_extensions_enabled":false,"responsive_web_graphql_timeline_navigation_enabled":true}
                    """,
                ["fieldToggles"] = """
                    {"withAuxiliaryUserLabels":false}
                    """,
            };
        }

        public async Task<TwitterGraphqlUser> Send(IApiConnection apiConnection)
        {
            var param = this.CreateParameters();

            XElement rootElm;
            try
            {
                using var stream = await apiConnection.GetStreamAsync(EndpointUri, param);
                using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);
                rootElm = XElement.Load(jsonReader);
            }
            catch (IOException ex)
            {
                throw new WebApiException("IO Error", ex);
            }
            catch (NotSupportedException ex)
            {
                // NotSupportedException: Stream does not support reading. のエラーが時々報告される
                throw new WebApiException("Stream Error", ex);
            }

            ErrorResponse.ThrowIfError(rootElm);

            var userElm = rootElm.XPathSelectElement("/data/user/result");

            return new(userElm);
        }
    }
}
