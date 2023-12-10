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

using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenTween.Api;
using Xunit;

namespace OpenTween.Connection
{
    public class ApiResponseTest
    {
        [Fact]
        public async Task ReadAsBytes_Test()
        {
            using var responseContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
            using var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent,
            };
            using var response = new ApiResponse(responseMessage);

            Assert.Equal(new byte[] { 1, 2, 3 }, await response.ReadAsBytes());
        }

        [DataContract]
        public struct TestJson
        {
            [DataMember(Name = "foo")]
            public int Foo { get; set; }
        }

        [Fact]
        public async Task ReadAsJson_Test()
        {
            using var responseContent = new StringContent("""{"foo":123}""");
            using var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent,
            };
            using var response = new ApiResponse(responseMessage);

            Assert.Equal(new() { Foo = 123 }, await response.ReadAsJson<TestJson>());
        }

        [Fact]
        public async Task ReadAsJson_InvalidJsonTest()
        {
            using var responseContent = new StringContent("### Invalid JSON Response ###");
            using var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent,
            };
            using var response = new ApiResponse(responseMessage);

            var ex = await Assert.ThrowsAsync<TwitterApiException>(
                () => response.ReadAsJson<TestJson>()
            );
            Assert.Equal("### Invalid JSON Response ###", ex.ResponseText);
        }

        [Fact]
        public async Task ReadAsJsonXml_Test()
        {
            using var responseContent = new StringContent("""{"foo":123}""");
            using var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent,
            };
            using var response = new ApiResponse(responseMessage);

            var rootElm = await response.ReadAsJsonXml();
            var xmlString = rootElm.ToString(SaveOptions.DisableFormatting);
            Assert.Equal("""<root type="object"><foo type="number">123</foo></root>""", xmlString);
        }

        [Fact]
        public async Task ReadAsJsonXml_InvalidJsonTest()
        {
            using var responseContent = new StringContent("### Invalid JSON Response ###");
            using var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent,
            };
            using var response = new ApiResponse(responseMessage);

            var ex = await Assert.ThrowsAsync<TwitterApiException>(
                () => response.ReadAsJsonXml()
            );
            Assert.Equal("### Invalid JSON Response ###", ex.ResponseText);
        }

        [Fact]
        public async Task ReadAsLazyJson_Test()
        {
            using var responseContent = new StringContent("""{"foo":123}""");
            using var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent,
            };
            using var response = new ApiResponse(responseMessage);

            using var lazyJson = response.ReadAsLazyJson<TestJson>();
            Assert.Equal(new() { Foo = 123 }, await lazyJson.LoadJsonAsync());
        }

        [Fact]
        public async Task ReadAsLazyJson_DisposeTest()
        {
            using var responseContent = new StringContent("""{"foo":123}""");
            using var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent,
            };
            using var response = new ApiResponse(responseMessage);
            using var lazyJson = response.ReadAsLazyJson<TestJson>();
            response.Dispose(); // ApiResponse を先に破棄しても LazyJson に影響しないことをテストする

            Assert.Equal(new() { Foo = 123 }, await lazyJson.LoadJsonAsync());
        }
    }
}
