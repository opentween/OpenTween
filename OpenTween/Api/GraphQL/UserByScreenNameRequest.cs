﻿// OpenTween - Client of Twitter
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
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Connection;

namespace OpenTween.Api.GraphQL
{
    public class UserByScreenNameRequest
    {
        public static readonly string EndpointName = "UserByScreenName";

        private static readonly Uri EndpointUri = new("https://twitter.com/i/api/graphql/xc8f1g7BYqr6VTzTbvNlGw/UserByScreenName");

        public required string ScreenName { get; set; }

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
            var request = new GetRequest
            {
                RequestUri = EndpointUri,
                Query = this.CreateParameters(),
                EndpointName = EndpointName,
            };

            using var response = await apiConnection.SendAsync(request)
                .ConfigureAwait(false);

            var rootElm = await response.ReadAsJsonXml()
                .ConfigureAwait(false);

            ErrorResponse.ThrowIfError(rootElm);

            try
            {
                var userElm = rootElm.XPathSelectElement("/data/user/result");
                this.ThrowIfUserUnavailable(userElm);

                return new(userElm);
            }
            catch (WebApiException ex)
            {
                ex.ResponseText = JsonUtils.JsonXmlToString(rootElm);
                throw;
            }
        }

        private void ThrowIfUserUnavailable(XElement? userElm)
        {
            if (userElm == null)
            {
                var errorText = "User is not available.";
                throw new WebApiException(errorText);
            }

            var typeName = userElm.Element("__typename")?.Value;
            if (typeName == "UserUnavailable")
            {
                var errorText = userElm.Element("message")?.Value ?? "User is not available.";
                throw new WebApiException(errorText);
            }
        }
    }
}
