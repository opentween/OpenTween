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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween.Connection
{
    public class PostMultipartRequestTest
    {
        [Fact]
        public async Task CreateMessage_Test()
        {
            using var image = TestUtils.CreateDummyImage();
            using var media = new MemoryImageMediaItem(image);

            var request = new PostMultipartRequest
            {
                RequestUri = new("hoge/aaa.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["aaaa"] = "1111",
                    ["bbbb"] = "2222",
                },
                Media = new Dictionary<string, IMediaItem>
                {
                    ["media1"] = media,
                },
            };

            var baseUri = new Uri("https://example.com/v1/");
            using var requestMessage = request.CreateMessage(baseUri);

            Assert.Equal(HttpMethod.Post, requestMessage.Method);
            Assert.Equal(new("https://example.com/v1/hoge/aaa.json"), requestMessage.RequestUri);

            using var requestContent = Assert.IsType<MultipartFormDataContent>(requestMessage.Content);
            var boundary = requestContent.Headers.ContentType.Parameters.Cast<NameValueHeaderValue>()
                .First(y => y.Name == "boundary").Value;

            // 前後のダブルクオーテーションを除去
            boundary = boundary.Substring(1, boundary.Length - 2);

            var expectedText =
                $"--{boundary}\r\n" +
                "Content-Type: text/plain; charset=utf-8\r\n" +
                "Content-Disposition: form-data; name=aaaa\r\n" +
                "\r\n" +
                "1111\r\n" +
                $"--{boundary}\r\n" +
                "Content-Type: text/plain; charset=utf-8\r\n" +
                "Content-Disposition: form-data; name=bbbb\r\n" +
                "\r\n" +
                "2222\r\n" +
                $"--{boundary}\r\n" +
                $"Content-Disposition: form-data; name=media1; filename={media.Name}; filename*=utf-8''{media.Name}\r\n" +
                "\r\n";

            var expected = Encoding.UTF8.GetBytes(expectedText)
                .Concat(image.Stream.ToArray())
                .Concat(Encoding.UTF8.GetBytes($"\r\n--{boundary}--\r\n"));

            Assert.Equal(expected, await requestContent.ReadAsByteArrayAsync());
        }
    }
}
