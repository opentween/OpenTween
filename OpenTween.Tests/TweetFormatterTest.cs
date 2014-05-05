// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using OpenTween.Api;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class TweetFormatterTest
    {
        [Fact]
        public void FormatUrlEntity_Test()
        {
            var text = "http://t.co/KYi7vMZzRt";
            var entities = new[]
            {
                new TwitterDataModel.Urls
                {
                    Indices = new[] { 0, 22 },
                    DisplayUrl = "twitter.com",
                    ExpandedUrl = "http://twitter.com/",
                    Url = "http://t.co/KYi7vMZzRt",
                },
            };

            var expected = "<a href=\"http://t.co/KYi7vMZzRt\" title=\"http://twitter.com/\">twitter.com</a>";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void FormatHashtagEntity_Test()
        {
            var text = "#OpenTween";
            var entities = new[]
            {
                new TwitterDataModel.Hashtags
                {
                    Indices = new[] { 0, 10 },
                    Text = "OpenTween",
                },
            };

            var expected = "<a class=\"hashtag\" href=\"https://twitter.com/search?q=%23OpenTween\">#OpenTween</a>";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void FormatMentionEntity_Test()
        {
            var text = "@TwitterAPI";
            var entities = new[]
            {
                new TwitterDataModel.UserMentions
                {
                    Indices = new[] { 0, 11 },
                    Id = 6253282L,
                    Name = "Twitter API",
                    ScreenName = "twitterapi",
                },
            };

            var expected = "<a class=\"mention\" href=\"https://twitter.com/twitterapi\">@TwitterAPI</a>";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void FormatMediaEntity_Test()
        {
            var text = "http://t.co/h5dCr4ftN4";
            var entities = new[]
            {
                new TwitterDataModel.Media
                {
                    Indices = new[] { 0, 22 },
                    Sizes = new TwitterDataModel.Sizes
                    {
                        Large = new TwitterDataModel.SizeElement { Resize = "fit", h = 329, w = 1024 },
                        Medium = new TwitterDataModel.SizeElement { Resize = "fit", h = 204, w = 600 },
                        Small = new TwitterDataModel.SizeElement { Resize = "fit", h = 116, w = 340 },
                        Thumb = new TwitterDataModel.SizeElement { Resize = "crop", h = 150, w = 150 },
                    },
                    Type = "photo",
                    Id = 426404550379986940L,
                    MediaUrl = "http://pbs.twimg.com/media/BerkrewCYAAV4Kf.png",
                    MediaUrlHttps = "https://pbs.twimg.com/media/BerkrewCYAAV4Kf.png",
                    Url = "http://t.co/h5dCr4ftN4",
                    DisplayUrl = "pic.twitter.com/h5dCr4ftN4",
                    ExpandedUrl = "http://twitter.com/kim_upsilon/status/426404550371598337/photo/1",
                },
            };

            var expected = "<a href=\"http://t.co/h5dCr4ftN4\" title=\"http://twitter.com/kim_upsilon/status/426404550371598337/photo/1\">pic.twitter.com/h5dCr4ftN4</a>";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EntityNullTest()
        {
            var text = "てすとてすとー";
            TwitterDataModel.Entities entities = null;

            var expected = "てすとてすとー";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EntityNullTest2()
        {
            var text = "てすとてすとー";
            TwitterDataModel.Entities entities = new TwitterDataModel.Entities
            {
                Urls = null,
                Hashtags = null,
                UserMentions = null,
                Media = null,
            };

            var expected = "てすとてすとー";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EntityNullTest3()
        {
            var text = "てすとてすとー";
            IEnumerable<TwitterDataModel.Entity> entities = null;

            var expected = "てすとてすとー";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EntityNullTest4()
        {
            var text = "てすとてすとー";
            IEnumerable<TwitterDataModel.Entity> entities = new TwitterDataModel.Entity[] { null };

            var expected = "てすとてすとー";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EscapeTest()
        {
            // Twitter APIの中途半端なエスケープの対象とならない「"」や「'」に対するエスケープ処理を施す
            var text = "\"\'@twitterapi\'\"";
            var entities = new[]
            {
                new TwitterDataModel.UserMentions
                {
                    Indices = new[] { 2, 13 },
                    Id = 6253282L,
                    Name = "Twitter API",
                    ScreenName = "twitterapi",
                },
            };

            var expected = "&quot;&#39;<a class=\"mention\" href=\"https://twitter.com/twitterapi\">@twitterapi</a>&#39;&quot;";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EscapeTest2()
        {
            // 「<」や「>」についてはエスケープされた状態でAPIからテキストが返されるため、二重エスケープとならないように考慮する
            var text = "&lt;b&gt; @twitterapi &lt;/b&gt;";
            var entities = new[]
            {
                new TwitterDataModel.UserMentions
                {
                    Indices = new[] { 10, 21 },
                    Id = 6253282L,
                    Name = "Twitter API",
                    ScreenName = "twitterapi",
                },
            };

            var expected = "&lt;b&gt; <a class=\"mention\" href=\"https://twitter.com/twitterapi\">@twitterapi</a> &lt;/b&gt;";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EscapeTest3()
        {
            // 万が一「<」や「>」がエスケープされていない状態のテキストを受け取っても適切にエスケープが施されるようにする
            var text = "<b> @twitterapi </b>";
            var entities = new[]
            {
                new TwitterDataModel.UserMentions
                {
                    Indices = new[] { 4, 15 },
                    Id = 6253282L,
                    Name = "Twitter API",
                    ScreenName = "twitterapi",
                },
            };

            var expected = "&lt;b&gt; <a class=\"mention\" href=\"https://twitter.com/twitterapi\">@twitterapi</a> &lt;/b&gt;";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EscapeUrlTest()
        {
            // 日本語ハッシュタグのリンク先URLを適切にエスケープする
            var text = "#ぜんぶ雪のせいだ";
            var entities = new[]
            {
                new TwitterDataModel.Hashtags
                {
                    Indices = new[] { 0, 9 },
                    Text = "ぜんぶ雪のせいだ",
                },
            };

            var expected = "<a class=\"hashtag\" href=\"https://twitter.com/search?q=%23%E3%81%9C%E3%82%93%E3%81%B6%E9%9B%AA%E3%81%AE%E3%81%9B%E3%81%84%E3%81%A0\">#ぜんぶ雪のせいだ</a>";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_SurrogatePairTest()
        {
            // UTF-16 で 4 バイトで表される文字を含むツイート
            // 参照: https://sourceforge.jp/ticket/browse.php?group_id=6526&tid=33079
            var text = "🐬🐬 @irucame 🐬🐬";
            var entities = new[]
            {
                new TwitterDataModel.UserMentions
                {
                    Indices = new[] { 3, 11 },
                    Id = 89942943L,
                    ScreenName = "irucame",
                },
            };

            var expected = "🐬🐬 <a class=\"mention\" href=\"https://twitter.com/irucame\">@irucame</a> 🐬🐬";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_SurrogatePairTest2()
        {
            // 現時点では存在しないものの、ハッシュタグなどエンティティ内にサロゲートペアが含まれる場合も考慮する
            var text = "🐬🐬 #🐬🐬 🐬🐬 #🐬🐬 🐬🐬";
            var entities = new[]
            {
                new TwitterDataModel.Hashtags
                {
                    Indices = new[] { 3, 6 },
                    Text = "🐬🐬",
                },
                new TwitterDataModel.Hashtags
                {
                    Indices = new[] { 10, 13 },
                    Text = "🐬🐬",
                },
            };

            var expected = "🐬🐬 <a class=\"hashtag\" href=\"https://twitter.com/search?q=%23%F0%9F%90%AC%F0%9F%90%AC\">#🐬🐬</a> " +
                "🐬🐬 <a class=\"hashtag\" href=\"https://twitter.com/search?q=%23%F0%9F%90%AC%F0%9F%90%AC\">#🐬🐬</a> 🐬🐬";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_CompositeCharacterTest()
        {
            // 合成文字 é ( \u00e9 ) を含むツイート
            // 参照: https://dev.twitter.com/issues/251
            var text = "Caf\u00e9 #test";
            var entities = new[]
            {
                new TwitterDataModel.Hashtags
                {
                    Indices = new[] { 5, 10 },
                    Text = "test",
                },
            };

            var expected = "Caf\u00e9 <a class=\"hashtag\" href=\"https://twitter.com/search?q=%23test\">#test</a>";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_CombiningCharacterSequenceTest()
        {
            // 結合文字列 é ( e + \u0301 ) を含むツイート
            // 参照: https://dev.twitter.com/issues/251
            var text = "Cafe\u0301 #test";
            var entities = new[]
            {
                new TwitterDataModel.Hashtags
                {
                    Indices = new[] { 6, 11 },
                    Text = "test",
                },
            };

            var expected = "Cafe\u0301 <a class=\"hashtag\" href=\"https://twitter.com/search?q=%23test\">#test</a>";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_BreakLineTest()
        {
            var text = "てすと\nてすと\nてすと";
            TwitterDataModel.Entities entities = null;

            var expected = "てすと<br>てすと<br>てすと";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }
    }
}
