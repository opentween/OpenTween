// OpenTween - Client of Twitter
// Copyright (c) 2019 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace OpenTween.Api
{
    public class JsonUtilsTest
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("123ABCabc", "123ABCabc")]
        [InlineData(@"\", @"\\")]
        [InlineData("\"", "\\\"")]
        [InlineData("\n", @"\u000A")]
        [InlineData("\U0001D11E", @"\uD834\uDD1E")]
        public void EscapeJsonString_Test(string targetText, string expectedText)
            => Assert.Equal(expectedText, JsonUtils.EscapeJsonString(targetText));

        [Fact]
        public void JsonXmlToString_WholeTest()
        {
            var json = """{"hoge":{"aaa":12345}}""";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);
            var rootElement = XElement.Load(jsonReader);

            Assert.Equal("""{"hoge":{"aaa":12345}}""", JsonUtils.JsonXmlToString(rootElement));
        }

        [Fact]
        public void JsonXmlToString_SubsetTest()
        {
            var json = """{"hoge":{"aaa":12345}}""";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);
            var rootElement = XElement.Load(jsonReader);

            Assert.Equal("""{"aaa":12345}""", JsonUtils.JsonXmlToString(rootElement.Element("hoge")));
        }

        [Fact]
        public void JsonXmlToString_NullTest()
        {
            var json = """{"hoge":null}""";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);
            var rootElement = XElement.Load(jsonReader);

            Assert.Equal("null", JsonUtils.JsonXmlToString(rootElement.Element("hoge")));
        }
    }
}
