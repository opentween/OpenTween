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
            var text = "http://t.co/6IwepKCM0P";
            var entities = new[]
            {
                new TwitterEntityUrl
                {
                    Indices = new[] { 0, 22 },
                    DisplayUrl = "example.com",
                    ExpandedUrl = "http://example.com/",
                    Url = "http://t.co/6IwepKCM0P",
                },
            };

            var expected = "<a href=\"http://t.co/6IwepKCM0P\" title=\"http://example.com/\">example.com</a>";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void FormatUrlEntity_TwitterComTest()
        {
            var text = "https://t.co/0Ko1I27m0a";
            var entities = new[]
            {
                new TwitterEntityUrl
                {
                    Indices = new[] { 0, 23 },
                    DisplayUrl = "twitter.com/twitterapi",
                    ExpandedUrl = "https://twitter.com/twitterapi",
                    Url = "https://t.co/0Ko1I27m0a",
                },
            };

            // twitter.com 宛のリンクは t.co を経由せずにリンクする
            var expected = "<a href=\"https://twitter.com/twitterapi\" title=\"https://twitter.com/twitterapi\">twitter.com/twitterapi</a>";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void FormatHashtagEntity_Test()
        {
            var text = "#OpenTween";
            var entities = new[]
            {
                new TwitterEntityHashtag
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
                new TwitterEntityMention
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
                new TwitterEntityMedia
                {
                    Indices = new[] { 0, 22 },
                    Sizes = new TwitterMediaSizes
                    {
                        Large = new TwitterMediaSizes.Size { Resize = "fit", Height = 329, Width = 1024 },
                        Medium = new TwitterMediaSizes.Size { Resize = "fit", Height = 204, Width = 600 },
                        Small = new TwitterMediaSizes.Size { Resize = "fit", Height = 116, Width = 340 },
                        Thumb = new TwitterMediaSizes.Size { Resize = "crop", Height = 150, Width = 150 },
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
            TwitterEntities entities = null;

            var expected = "てすとてすとー";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EntityNullTest2()
        {
            var text = "てすとてすとー";
            TwitterEntities entities = new TwitterEntities
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
            IEnumerable<TwitterEntity> entities = null;

            var expected = "てすとてすとー";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EntityNullTest4()
        {
            var text = "てすとてすとー";
            IEnumerable<TwitterEntity> entities = new TwitterEntity[] { null };

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
                new TwitterEntityMention
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
                new TwitterEntityMention
                {
                    Indices = new[] { 10, 21 },
                    Id = 6253282L,
                    Name = "Twitter API",
                    ScreenName = "twitterapi",
                },
            };

            var expected = "&lt;b&gt;&nbsp;<a class=\"mention\" href=\"https://twitter.com/twitterapi\">@twitterapi</a>&nbsp;&lt;/b&gt;";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EscapeTest3()
        {
            // 万が一「<」や「>」がエスケープされていない状態のテキストを受け取っても適切にエスケープが施されるようにする
            var text = "<b> @twitterapi </b>";
            var entities = new[]
            {
                new TwitterEntityMention
                {
                    Indices = new[] { 4, 15 },
                    Id = 6253282L,
                    Name = "Twitter API",
                    ScreenName = "twitterapi",
                },
            };

            var expected = "&lt;b&gt;&nbsp;<a class=\"mention\" href=\"https://twitter.com/twitterapi\">@twitterapi</a>&nbsp;&lt;/b&gt;";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_EscapeUrlTest()
        {
            // 日本語ハッシュタグのリンク先URLを適切にエスケープする
            var text = "#ぜんぶ雪のせいだ";
            var entities = new[]
            {
                new TwitterEntityHashtag
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
                new TwitterEntityMention
                {
                    Indices = new[] { 3, 11 },
                    Id = 89942943L,
                    ScreenName = "irucame",
                },
            };

            var expected = "🐬🐬&nbsp;<a class=\"mention\" href=\"https://twitter.com/irucame\">@irucame</a>&nbsp;🐬🐬";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_SurrogatePairTest2()
        {
            // 現時点では存在しないものの、ハッシュタグなどエンティティ内にサロゲートペアが含まれる場合も考慮する
            var text = "🐬🐬 #🐬🐬 🐬🐬 #🐬🐬 🐬🐬";
            var entities = new[]
            {
                new TwitterEntityHashtag
                {
                    Indices = new[] { 3, 6 },
                    Text = "🐬🐬",
                },
                new TwitterEntityHashtag
                {
                    Indices = new[] { 10, 13 },
                    Text = "🐬🐬",
                },
            };

            var expected = "🐬🐬&nbsp;<a class=\"hashtag\" href=\"https://twitter.com/search?q=%23%F0%9F%90%AC%F0%9F%90%AC\">#🐬🐬</a>&nbsp;" +
                "🐬🐬&nbsp;<a class=\"hashtag\" href=\"https://twitter.com/search?q=%23%F0%9F%90%AC%F0%9F%90%AC\">#🐬🐬</a>&nbsp;🐬🐬";
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
                new TwitterEntityHashtag
                {
                    Indices = new[] { 5, 10 },
                    Text = "test",
                },
            };

            var expected = "Caf\u00e9&nbsp;<a class=\"hashtag\" href=\"https://twitter.com/search?q=%23test\">#test</a>";
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
                new TwitterEntityHashtag
                {
                    Indices = new[] { 6, 11 },
                    Text = "test",
                },
            };

            var expected = "Cafe\u0301&nbsp;<a class=\"hashtag\" href=\"https://twitter.com/search?q=%23test\">#test</a>";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_BreakLineTest()
        {
            var text = "てすと\nてすと\nてすと";
            TwitterEntities entities = null;

            var expected = "てすと<br>てすと<br>てすと";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }

        [Fact]
        public void AutoLinkHtml_OverlappedEntitiesTest()
        {
            // extended_entities で追加される、区間が重複したエンティティを考慮
            // 参照: https://dev.twitter.com/docs/api/multiple-media-extended-entities

            var text = "\"I hope you'll keep...building bonds of friendship that will enrich your lives &amp; enrich our world\" \u2014FLOTUS in China, http://t.co/fxmuQN9JL9";
            var entities = new[]
            {
                new TwitterEntityMedia
                {
                    DisplayUrl = "pic.twitter.com/fxmuQN9JL9",
                    ExpandedUrl = "http://twitter.com/FLOTUS/status/449660889793581056/photo/1",
                    Indices = new[] { 121, 143 },
                    MediaUrlHttps = "https://pbs.twimg.com/media/Bj2EH6yIQAEYvxu.jpg",
                    Url = "http://t.co/fxmuQN9JL9",
                },
                new TwitterEntityMedia
                {
                    DisplayUrl = "pic.twitter.com/fxmuQN9JL9",
                    ExpandedUrl = "http://twitter.com/FLOTUS/status/449660889793581056/photo/1",
                    Indices = new[] { 121, 143 },
                    MediaUrlHttps = "https://pbs.twimg.com/media/Bj2EHxAIIAE8dtg.jpg",
                    Url = "http://t.co/fxmuQN9JL9",
                },
                new TwitterEntityMedia
                {
                    DisplayUrl = "pic.twitter.com/fxmuQN9JL9",
                    ExpandedUrl = "http://twitter.com/FLOTUS/status/449660889793581056/photo/1",
                    Indices = new[] { 121, 143 },
                    MediaUrlHttps = "https://pbs.twimg.com/media/Bj2EH3pIYAE4LQn.jpg",
                    Url = "http://t.co/fxmuQN9JL9",
                },
                new TwitterEntityMedia
                {
                    DisplayUrl = "pic.twitter.com/fxmuQN9JL9",
                    ExpandedUrl = "http://twitter.com/FLOTUS/status/449660889793581056/photo/1",
                    Indices = new[] { 121, 143 },
                    MediaUrlHttps = "https://pbs.twimg.com/media/Bj2EL3DIEAAzGAX.jpg",
                    Url = "http://t.co/fxmuQN9JL9",
                },
            };

            var expected = "&quot;I&nbsp;hope&nbsp;you&#39;ll&nbsp;keep...building&nbsp;bonds&nbsp;of&nbsp;friendship&nbsp;that&nbsp;will&nbsp;enrich&nbsp;your&nbsp;lives&nbsp;&amp;&nbsp;enrich&nbsp;our&nbsp;world&quot;&nbsp;\u2014FLOTUS&nbsp;in&nbsp;China,&nbsp;" +
                "<a href=\"http://t.co/fxmuQN9JL9\" title=\"http://twitter.com/FLOTUS/status/449660889793581056/photo/1\">pic.twitter.com/fxmuQN9JL9</a>";
            Assert.Equal(expected, TweetFormatter.AutoLinkHtml(text, entities));
        }
    }
}
