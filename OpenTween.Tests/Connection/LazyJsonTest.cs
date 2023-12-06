// OpenTween - Client of Twitter
// Copyright (c) 2016 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTween.Api;
using Xunit;

namespace OpenTween.Connection
{
    public class LazyJsonTest
    {
        [Fact]
        public async Task LoadJsonAsync_Test()
        {
            var body = Encoding.UTF8.GetBytes("\"hogehoge\"");
            using var bodyStream = new MemoryStream(body);
            using var response = new HttpResponseMessage();
            response.Content = new StreamContent(bodyStream);

            using var lazyJson = new LazyJson<string>(response);

            // この時点ではまだレスポンスボディは読まれない
            Assert.Equal(0, bodyStream.Position);

            var result = await lazyJson.LoadJsonAsync();

            Assert.Equal("hogehoge", result);
        }

        [Fact]
        public async Task LoadJsonAsync_InvalidJsonTest()
        {
            var body = Encoding.UTF8.GetBytes("### Invalid JSON ###");
            using var bodyStream = new MemoryStream(body);
            using var response = new HttpResponseMessage();
            response.Content = new StreamContent(bodyStream);

            using var lazyJson = new LazyJson<string>(response);

            // この時点ではまだレスポンスボディは読まれない
            Assert.Equal(0, bodyStream.Position);

            var exception = await Assert.ThrowsAnyAsync<WebApiException>(
                () => lazyJson.LoadJsonAsync()
            );

            Assert.IsType<SerializationException>(exception.InnerException);
        }

        [Fact]
        public async Task IgnoreResponse_Test()
        {
            using var bodyStream = new InvalidStream();
            using var response = new HttpResponseMessage();

            // IgnoreResponse() によってレスポンスの Stream が読まれずに破棄されることをテストするため、
            // 読み込みが行われると IOException が発生する InvalidStream クラスを bodyStream に使用している
            response.Content = new StreamContent(bodyStream);

            using var lazyJson = new LazyJson<string>(response);

            // レスポンスボディを読まずに破棄
            await Task.FromResult(lazyJson)
                .IgnoreResponse();

            Assert.True(bodyStream.IsDisposed);
        }

        private class InvalidStream : Stream
        {
            public bool IsDisposed { get; private set; } = false;

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => 100L;

            public override long Position
            {
                get => 0L;
                set => throw new NotSupportedException();
            }

            public override void Flush()
                => throw new NotSupportedException();

            public override int Read(byte[] buffer, int offset, int count)
                => throw new IOException();

            public override long Seek(long offset, SeekOrigin origin)
                => throw new NotSupportedException();

            public override void SetLength(long value)
                => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count)
                => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                this.IsDisposed = true;
            }
        }
    }
}
