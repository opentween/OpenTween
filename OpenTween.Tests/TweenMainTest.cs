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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTween.Api.DataModel;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class TweenMainTest
    {
        [Fact]
        public void GetUrlFromDataObject_XMozUrlTest()
        {
            var dataBytes = Encoding.Unicode.GetBytes("https://twitter.com/\nTwitter\0");
            using var memstream = new MemoryStream(dataBytes);
            var data = new DataObject("text/x-moz-url", memstream);

            var expected = ("https://twitter.com/", "Twitter");
            Assert.Equal(expected, TweenMain.GetUrlFromDataObject(data));
        }

        [Fact]
        public void GetUrlFromDataObject_IESiteModeToUrlTest()
        {
            var dataBytes = Encoding.Unicode.GetBytes("https://twitter.com/\0Twitter\0");
            using var memstream = new MemoryStream(dataBytes);
            var data = new DataObject("IESiteModeToUrl", memstream);

            var expected = ("https://twitter.com/", "Twitter");
            Assert.Equal(expected, TweenMain.GetUrlFromDataObject(data));
        }

        [Fact]
        public void GetUrlFromDataObject_UniformResourceLocatorWTest()
        {
            var dataBytes = Encoding.Unicode.GetBytes("https://twitter.com/\0");
            using var memstream = new MemoryStream(dataBytes);
            var data = new DataObject("UniformResourceLocatorW", memstream);

            var expected = ("https://twitter.com/", (string?)null);
            Assert.Equal(expected, TweenMain.GetUrlFromDataObject(data));
        }

        [Fact]
        public void GetUrlFromDataObject_UnknownFormatTest()
        {
            using var memstream = new MemoryStream(Array.Empty<byte>());
            var data = new DataObject("application/x-hogehoge", memstream);

            Assert.Throws<NotSupportedException>(() => TweenMain.GetUrlFromDataObject(data));
        }

        [Fact]
        public void CreateRetweetUnofficial_UrlTest()
        {
            var statusText = "<a href=\"http://t.co/KYi7vMZzRt\" title=\"http://twitter.com/\">twitter.com</a>";

            Assert.Equal("http://twitter.com/", TweenMain.CreateRetweetUnofficial(statusText, false));
        }

        [Fact]
        public void CreateRetweetUnofficial_MentionTest()
        {
            var statusText = "<a class=\"mention\" href=\"https://twitter.com/twitterapi\">@TwitterAPI</a>";

            Assert.Equal("@TwitterAPI", TweenMain.CreateRetweetUnofficial(statusText, false));
        }

        [Fact]
        public void CreateRetweetUnofficial_HashtagTest()
        {
            var statusText = "<a class=\"hashtag\" href=\"https://twitter.com/search?q=%23OpenTween\">#OpenTween</a>";

            Assert.Equal("#OpenTween", TweenMain.CreateRetweetUnofficial(statusText, false));
        }

        [Fact]
        public void CreateRetweetUnofficial_SingleLineTest()
        {
            var statusText = "123<br>456<br>789";

            Assert.Equal("123 456 789", TweenMain.CreateRetweetUnofficial(statusText, false));
        }

        [Fact]
        public void CreateRetweetUnofficial_MultiLineTest()
        {
            var statusText = "123<br>456<br>789";

            Assert.Equal("123" + Environment.NewLine + "456" + Environment.NewLine + "789", TweenMain.CreateRetweetUnofficial(statusText, true));
        }

        [Fact]
        public void CreateRetweetUnofficial_DecodeTest()
        {
            var statusText = "&lt;&gt;&quot;&#39;&nbsp;";

            Assert.Equal("<>\"' ", TweenMain.CreateRetweetUnofficial(statusText, false));
        }

        [Fact]
        public void CreateRetweetUnofficial_WithFormatterTest()
        {
            // TweetFormatterでHTMLに整形 → CreateRetweetUnofficialで復元 までの動作が正しく行えているか

            var text = "#てすと @TwitterAPI \n http://t.co/KYi7vMZzRt";
            var entities = new TwitterEntity[]
            {
                new TwitterEntityHashtag
                {
                    Indices = new[] { 0, 4 },
                    Text = "てすと",
                },
                new TwitterEntityMention
                {
                    Indices = new[] { 5, 16 },
                    Id = 6253282L,
                    Name = "Twitter API",
                    ScreenName = "twitterapi",
                },
                new TwitterEntityUrl
                {
                    Indices = new[] { 19, 41 },
                    DisplayUrl = "twitter.com",
                    ExpandedUrl = "http://twitter.com/",
                    Url = "http://t.co/KYi7vMZzRt",
                },
            };

            var html = TweetFormatter.AutoLinkHtml(text, entities);

            var expected = "#てすと @TwitterAPI " + Environment.NewLine + " http://twitter.com/";
            Assert.Equal(expected, TweenMain.CreateRetweetUnofficial(html, true));
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("hoge", false)]
        [InlineData("@twitterapi ", true)]
        [InlineData("@twitterapi @opentween ", true)]
        [InlineData("@twitterapi @opentween hoge", false)]
        public void TextContainsOnlyMentions_Test(string input, bool expected)
            => Assert.Equal(expected, TweenMain.TextContainsOnlyMentions(input));
    }
}
