// OpenTween - Client of Twitter
// Copyright (c) 2015 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween
{
    public class TweetExtractorTest
    {
        [Fact]
        public void ExtractUrls_Test()
        {
            Assert.Equal(new[] { "http://example.com/" }, TweetExtractor.ExtractUrls("http://example.com/"));
            Assert.Equal(new[] { "http://example.com/hogehoge" }, TweetExtractor.ExtractUrls("http://example.com/hogehoge"));
            Assert.Equal(new[] { "http://example.com/" }, TweetExtractor.ExtractUrls("hogehoge http://example.com/"));

            Assert.Equal(new[] { "https://example.com/" }, TweetExtractor.ExtractUrls("https://example.com/"));
            Assert.Equal(new[] { "https://example.com/hogehoge" }, TweetExtractor.ExtractUrls("https://example.com/hogehoge"));
            Assert.Equal(new[] { "https://example.com/" }, TweetExtractor.ExtractUrls("hogehoge https://example.com/"));

            Assert.Equal(new[] { "example.com" }, TweetExtractor.ExtractUrls("example.com"));
            Assert.Equal(new[] { "example.com/hogehoge" }, TweetExtractor.ExtractUrls("example.com/hogehoge"));
            Assert.Equal(new[] { "example.com" }, TweetExtractor.ExtractUrls("hogehoge example.com"));

            // スキーム (http://) を省略かつ末尾が ccTLD の場合は t.co に短縮されない
            Assert.Empty(TweetExtractor.ExtractUrls("example.jp"));
            // ただし、末尾にパスが続く場合は t.co に短縮される
            Assert.Equal(new[] { "example.jp/hogehoge" }, TweetExtractor.ExtractUrls("example.jp/hogehoge"));
        }
    }
}
