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
using OpenTween.Api;
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

        [Fact]
        public void ExtractUrlEntities_Test()
        {
            var entity = TweetExtractor.ExtractUrlEntities("hogehoge http://example.com/").Single();

            Assert.Equal(new[] { 9, 28 }, entity.Indices);
            Assert.Equal("http://example.com/", entity.Url);
            Assert.Equal("http://example.com/", entity.ExpandedUrl);
            Assert.Equal("http://example.com/", entity.DisplayUrl);
        }

        [Fact]
        public void ExtractUrlEntities_SurrogatePairTest()
        {
            var entity = TweetExtractor.ExtractUrlEntities("✨ http://example.com/ ✨").Single();

            Assert.Equal(new[] { 2, 21 }, entity.Indices);
            Assert.Equal("http://example.com/", entity.Url);
            Assert.Equal("http://example.com/", entity.ExpandedUrl);
            Assert.Equal("http://example.com/", entity.DisplayUrl);
        }

        [Fact]
        public void ExtractUrlEntities_CompositeCharacterTest()
        {
            // 合成文字 é ( \u00e9 ) を含むツイート (1文字としてカウントする)
            // 参照: https://dev.twitter.com/issues/251
            var entity = TweetExtractor.ExtractUrlEntities("Caf\u00e9 http://example.com/").Single();

            Assert.Equal(new[] { 5, 24 }, entity.Indices);
            Assert.Equal("http://example.com/", entity.Url);
            Assert.Equal("http://example.com/", entity.ExpandedUrl);
            Assert.Equal("http://example.com/", entity.DisplayUrl);
        }

        [Fact]
        public void ExtractUrlEntities_CombiningCharacterSequenceTest()
        {
            // 結合文字列 é ( e + \u0301 ) を含むツイート (2文字としてカウントする)
            // 参照: https://dev.twitter.com/issues/251
            var entity = TweetExtractor.ExtractUrlEntities("Cafe\u0301 http://example.com/").Single();

            Assert.Equal(new[] { 6, 25 }, entity.Indices);
            Assert.Equal("http://example.com/", entity.Url);
            Assert.Equal("http://example.com/", entity.ExpandedUrl);
            Assert.Equal("http://example.com/", entity.DisplayUrl);
        }

        [Fact]
        public void ExtractMentionEntities_Test()
        {
            var entity = TweetExtractor.ExtractMentionEntities("hogehoge @twitterapi").Single();

            Assert.Equal(new[] { 9, 20 }, entity.Indices);
            Assert.Equal("@twitterapi", entity.ScreenName);
        }

        [Fact]
        public void ExtractMentionEntities_ListTest()
        {
            var entity = TweetExtractor.ExtractMentionEntities("hogehoge @twitter/developers").Single();

            Assert.Equal(new[] { 9, 28 }, entity.Indices);
            Assert.Equal("@twitter/developers", entity.ScreenName);
        }

        [Fact]
        public void ExtractHashtagEntities_Test()
        {
            var entity = TweetExtractor.ExtractHashtagEntities("hogehoge #test").Single();

            Assert.Equal(new[] { 9, 14 }, entity.Indices);
            Assert.Equal("#test", entity.Text);
        }
    }
}
