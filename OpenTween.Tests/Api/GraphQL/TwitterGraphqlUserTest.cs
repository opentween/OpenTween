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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace OpenTween.Api.GraphQL
{
    public class TwitterGraphqlUserTest
    {
        private XElement LoadResponseDocument(string filename)
        {
            using var stream = File.OpenRead($"Resources/Responses/{filename}");
            using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);
            return XElement.Load(jsonReader);
        }

        [Fact]
        public void ToTwitterUser_Test()
        {
            var userElm = this.LoadResponseDocument("User_Simple.json");
            var graphqlUser = new TwitterGraphqlUser(userElm);
            var user = graphqlUser.ToTwitterUser();

            Assert.Equal("514241801", user.IdStr);
            Assert.Equal("opentween", user.ScreenName);
        }

        [Fact]
        public void ToTwitterUser_EntityWithoutDisplayUrlTest()
        {
            var userElm = this.LoadResponseDocument("User_EntityWithoutDisplayUrl.json");
            var graphqlUser = new TwitterGraphqlUser(userElm);
            var user = graphqlUser.ToTwitterUser();

            Assert.Equal("4104111", user.IdStr);
            var urlEntity = user.Entities?.Url?.Urls.First()!;
            Assert.Equal("http://earthquake.transrain.net/", urlEntity.Url);
            Assert.Equal(new[] { 0, 32 }, urlEntity.Indices);
            Assert.Null(urlEntity.DisplayUrl);
            Assert.Null(urlEntity.ExpandedUrl);
        }
    }
}
