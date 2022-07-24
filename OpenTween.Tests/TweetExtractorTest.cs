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
        public void ExtractUrlEntities_MultipleTest()
        {
            var entities = TweetExtractor.ExtractUrlEntities("hogehoge http://aaa.example.com/ http://bbb.example.com/").ToArray();

            Assert.Equal(2, entities.Length);
            Assert.Equal(new[] { 9, 32 }, entities[0].Indices);
            Assert.Equal("http://aaa.example.com/", entities[0].Url);
            Assert.Equal(new[] { 33, 56 }, entities[1].Indices);
            Assert.Equal("http://bbb.example.com/", entities[1].Url);
        }

        [Fact]
        public void ExtractUrlEntities_SurrogatePairTest()
        {
            var entity = TweetExtractor.ExtractUrlEntities("🍣 http://example.com/ 🍣").Single();

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

            // Indices は「@twitterapi」の範囲を指すが、ScreenName には「@」を含めない
            Assert.Equal(new[] { 9, 20 }, entity.Indices);
            Assert.Equal("twitterapi", entity.ScreenName);
        }

        [Fact]
        public void ExtractMentionEntities_MultipleTest()
        {
            var entities = TweetExtractor.ExtractMentionEntities("hogehoge @twitterapi @opentween").ToArray();

            Assert.Equal(2, entities.Length);
            Assert.Equal(new[] { 9, 20 }, entities[0].Indices);
            Assert.Equal("twitterapi", entities[0].ScreenName);
            Assert.Equal(new[] { 21, 31 }, entities[1].Indices);
            Assert.Equal("opentween", entities[1].ScreenName);
        }

        [Fact]
        public void ExtractMentionEntities_ListTest()
        {
            var entity = TweetExtractor.ExtractMentionEntities("hogehoge @twitter/developers").Single();

            // Indices は「@twitter/developers」の範囲を指すが、ScreenName には「@」を含めない
            Assert.Equal(new[] { 9, 28 }, entity.Indices);
            Assert.Equal("twitter/developers", entity.ScreenName);
        }

        [Fact]
        public void ExtractMentionEntities_SurrogatePairTest()
        {
            var entity = TweetExtractor.ExtractMentionEntities("🍣 @twitterapi").Single();

            Assert.Equal(new[] { 2, 13 }, entity.Indices);
            Assert.Equal("twitterapi", entity.ScreenName);
        }

        [Fact]
        public void ExtractHashtagEntities_Test()
        {
            var entity = TweetExtractor.ExtractHashtagEntities("hogehoge #test").Single();

            // Indices は「#test」の範囲を指すが、Text には「#」を含めない
            Assert.Equal(new[] { 9, 14 }, entity.Indices);
            Assert.Equal("test", entity.Text);
        }

        [Fact]
        public void ExtractHashtagEntities_MultipleTest()
        {
            var entities = TweetExtractor.ExtractHashtagEntities("hogehoge #test #test2").ToArray();

            Assert.Equal(2, entities.Length);
            Assert.Equal(new[] { 9, 14 }, entities[0].Indices);
            Assert.Equal("test", entities[0].Text);
            Assert.Equal(new[] { 15, 21 }, entities[1].Indices);
            Assert.Equal("test2", entities[1].Text);
        }

        [Fact]
        public void ExtractHashtagEntities_SurrogatePairTest()
        {
            var entity = TweetExtractor.ExtractHashtagEntities("🍣 #sushi").Single();

            Assert.Equal(new[] { 2, 8 }, entity.Indices);
            Assert.Equal("sushi", entity.Text);
        }

        [Fact]
        public void ExtractEmojiEntities_Test()
        {
            var entity = TweetExtractor.ExtractEmojiEntities("star ✨").Single();

            Assert.Equal(new[] { 5, 6 }, entity.Indices);
            Assert.Equal("✨", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/2728.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_SurrogatePairTest()
        {
            var entity = TweetExtractor.ExtractEmojiEntities("𠮷野家 🍚").Single();

            // 「𠮷」「🍚」は UTF-16 でそれぞれ 2byte になるがインデックスはコードポイント単位で数えなければならない
            Assert.Equal(new[] { 4, 5 }, entity.Indices);
            Assert.Equal("🍚", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/1f35a.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_VariationSelector_TextStyleTest()
        {
            // 異字体セレクタを使用して明示的にテキストスタイルで表示させている文字
            var origText = "©\uFE0E"; // U+00A9 + U+FE0E (text style)
            var entities = TweetExtractor.ExtractEmojiEntities(origText);

            Assert.Empty(entities);
        }

        [Fact]
        public void ExtractEmojiEntities_VariationSelector_EmojiStyleTest()
        {
            // 異字体セレクタを使用して明示的に絵文字スタイルで表示させている文字
            var origText = "©\uFE0F"; // U+00A9 + U+FE0F (emoji style)
            var entity = TweetExtractor.ExtractEmojiEntities(origText).Single();

            Assert.Equal(new[] { 0, 2 }, entity.Indices);
            Assert.Equal("©", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/a9.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_VariationSelector_UnnecessaryEmojiStyleTest()
        {
            // 余分な U+FE0F が付いている場合
            var origText = "🍣\uFE0F"; // U+1F363 + U+FE0F (emoji style)
            var entities = TweetExtractor.ExtractEmojiEntities(origText).ToArray();

            Assert.Equal(2, entities.Length);

            Assert.Equal(new[] { 0, 1 }, entities[0].Indices);
            Assert.Equal("🍣", entities[0].Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/1f363.png", entities[0].Url);

            Assert.Equal(new[] { 1, 2 }, entities[1].Indices);
            Assert.Equal("", entities[1].Text);
            Assert.Equal("", entities[1].Url);
        }

        [Fact]
        public void ExtractEmojiEntities_CombiningCharacterTest()
        {
            var origText = "#⃣"; // U+0023 U+20E3 (合字)
            var entity = TweetExtractor.ExtractEmojiEntities(origText).Single();

            Assert.Equal(new[] { 0, 2 }, entity.Indices);
            Assert.Equal("#⃣", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/23-20e3.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_Unicode10Test()
        {
            // Unicode 10.0/Emoji 5.0 で追加された絵文字
            var origText = "🦒"; // U+1F992 (GIRAFFE)
            var entity = TweetExtractor.ExtractEmojiEntities(origText).Single();

            Assert.Equal(new[] { 0, 1 }, entity.Indices);
            Assert.Equal("🦒", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/1f992.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_Unicode11Test()
        {
            // Unicode 11.0 で追加された絵文字
            var origText = "🦸"; // U+1F9B8 (SUPERHERO)
            var entity = TweetExtractor.ExtractEmojiEntities(origText).Single();

            Assert.Equal(new[] { 0, 1 }, entity.Indices);
            Assert.Equal("🦸", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/1f9b8.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_Unicode12Test()
        {
            // Unicode 12.0 で追加された絵文字
            var origText = "🧅"; // U+1F9C5 (ONION)
            var entity = TweetExtractor.ExtractEmojiEntities(origText).Single();

            Assert.Equal(new[] { 0, 1 }, entity.Indices);
            Assert.Equal("🧅", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/1f9c5.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_Unicode13Test()
        {
            // Unicode 13.0 で追加された絵文字
            var origText = "🥷"; // U+1F977 (NINJA)
            var entity = TweetExtractor.ExtractEmojiEntities(origText).Single();

            Assert.Equal(new[] { 0, 1 }, entity.Indices);
            Assert.Equal("🥷", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/1f977.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_Unicode14Test()
        {
            // Unicode 14.0 で追加された絵文字
            var origText = "🫠"; // U+1FAE0 (MELTING FACE)
            var entity = TweetExtractor.ExtractEmojiEntities(origText).Single();

            Assert.Equal(new[] { 0, 1 }, entity.Indices);
            Assert.Equal("🫠", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/1fae0.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_EmojiModifiers_CombiningTest()
        {
            // Emoji modifiers を使用した合字 (リガチャー)
            var origText = "👦\U0001F3FF"; // U+1F466 (BOY) + U+1F3FF (EMOJI MODIFIER FITZPATRICK TYPE-6)
            var entity = TweetExtractor.ExtractEmojiEntities(origText).Single();

            Assert.Equal(new[] { 0, 2 }, entity.Indices);
            Assert.Equal("👦\U0001F3FF", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/1f466-1f3ff.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_EmojiModifiers_SingleTest()
        {
            // Emoji modifiers は単体でも絵文字として表示される
            var origText = "\U0001F3FF"; // U+1F3FB (EMOJI MODIFIER FITZPATRICK TYPE-6)
            var entity = TweetExtractor.ExtractEmojiEntities(origText).Single();

            Assert.Equal(new[] { 0, 1 }, entity.Indices);
            Assert.Equal("\U0001F3FF", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/1f3ff.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_EmojiZWJSequenceTest()
        {
            // 複数の絵文字を U+200D (ZERO WIDTH JOINER) で繋げて表現する絵文字
            var origText = "👨\u200D🎨"; // U+1F468 (MAN) + U+200D + U+1F3A8 (ARTIST PALETTE)
            var entity = TweetExtractor.ExtractEmojiEntities(origText).Single();

            Assert.Equal(new[] { 0, 3 }, entity.Indices);
            Assert.Equal("👨\u200D🎨", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/1f468-200d-1f3a8.png", entity.Url);
        }

        [Fact]
        public void ExtractEmojiEntities_EmojiZWJSequenceWithVariationSelectorTest()
        {
            // 複数の絵文字を U+200D (ZERO WIDTH JOINER) で繋げて表現 + 異字体セレクタ U+FE0F を含む絵文字
            // この場合は URL 生成時に異字体セレクタ U+FE0F を除去しない
            var origText = "🏃\u200D♀\uFE0F"; // U+1F3C3 (RUNNER) + U+200D + U+2640 (FEMARE SIGN) + U+FE0F
            var entity = TweetExtractor.ExtractEmojiEntities(origText).Single();

            Assert.Equal(new[] { 0, 4 }, entity.Indices);
            Assert.Equal("🏃\u200D♀\uFE0F", entity.Text);
            Assert.Equal("https://twemoji.maxcdn.com/2/72x72/1f3c3-200d-2640-fe0f.png", entity.Url);
        }
    }
}
