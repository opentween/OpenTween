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
using System.Net.Http;
using Moq;
using Xunit;

namespace OpenTween.Connection
{
    public class HttpClientBuilderTest
    {
        [Fact]
        public void Build_Test()
        {
            var builder = new HttpClientBuilder();
            using var client = builder.Build();
        }

        [Fact]
        public void SetupHttpClientHandler_Test()
        {
            var builder = new HttpClientBuilder();
            builder.SetupHttpClientHandler(x => x.AllowAutoRedirect = true);
            builder.AddHandler(x =>
            {
                var httpClientHandler = (HttpClientHandler)x;
                Assert.True(httpClientHandler.AllowAutoRedirect);
                return x;
            });
            using var client = builder.Build();
        }

        [Fact]
        public void AddHandler_Test()
        {
            var count = 0;

            var builder = new HttpClientBuilder();
            builder.AddHandler(x =>
            {
                count++;
                Assert.IsType<WebRequestHandler>(x);
                return x;
            });
            using var client = builder.Build();

            Assert.Equal(1, count);
        }

        [Fact]
        public void AddHandler_NestingTest()
        {
            var count = 0;
            HttpMessageHandler? handler = null;

            var builder = new HttpClientBuilder();
            builder.AddHandler(x =>
            {
                count++;
                handler = Mock.Of<HttpMessageHandler>();
                return handler;
            });
            builder.AddHandler(x =>
            {
                count++;
                Assert.NotNull(x);
                Assert.Same(handler, x);
                return x;
            });
            using var client = builder.Build();

            Assert.Equal(2, count);
        }

        [Fact]
        public void SetupHttpClient_Test()
        {
            var builder = new HttpClientBuilder();
            builder.SetupHttpClient(x => x.Timeout = TimeSpan.FromSeconds(10));
            using var client = builder.Build();

            Assert.Equal(TimeSpan.FromSeconds(10), client.Timeout);
        }
    }
}
