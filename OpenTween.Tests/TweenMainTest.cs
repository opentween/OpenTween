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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenTween.Api;
using OpenTween.Api.DataModel;
using OpenTween.Connection;
using OpenTween.Models;
using OpenTween.Setting;
using OpenTween.Thumbnail;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class TweenMainTest
    {
        private record TestContext(
            SettingManager Settings
        );

        private void UsingTweenMain(Action<TweenMain, TestContext> func)
        {
            var settings = new SettingManager("");
            var tabinfo = new TabInformations();
            using var twitterApi = new TwitterApi();
            using var twitter = new Twitter(twitterApi);
            using var imageCache = new ImageCache();
            using var iconAssets = new IconAssetsManager();
            var thumbnailGenerator = new ThumbnailGenerator(new(autoupdate: false));

            using var tweenMain = new TweenMain(settings, tabinfo, twitter, imageCache, iconAssets, thumbnailGenerator);
            var context = new TestContext(settings);

            func(tweenMain, context);
        }

        [WinFormsFact]
        public void Initialize_Test()
            => this.UsingTweenMain((_, _) => { });

        [WinFormsFact]
        public void FormatStatusText_NewLineTest()
        {
            this.UsingTweenMain((tweenMain, _) =>
            {
                Assert.Equal("aaa\nbbb", tweenMain.FormatStatusText("aaa\r\nbbb"));
            });
        }

        [WinFormsFact]
        public void FormatStatusText_NewLineInDMTest()
        {
            this.UsingTweenMain((tweenMain, _) =>
            {
                // DM にも適用する
                Assert.Equal("D opentween aaa\nbbb", tweenMain.FormatStatusText("D opentween aaa\r\nbbb"));
            });
        }

        [WinFormsFact]
        public void FormatStatusText_SeparateUrlAndFullwidthCharacter_EnabledTest()
        {
            this.UsingTweenMain((tweenMain, context) =>
            {
                tweenMain.SeparateUrlAndFullwidthCharacter = true;
                Assert.Equal("https://example.com/ あああ", tweenMain.FormatStatusText("https://example.com/あああ"));
            });
        }

        [WinFormsFact]
        public void FormatStatusText_SeparateUrlAndFullwidthCharacter_DisabledTest()
        {
            this.UsingTweenMain((tweenMain, context) =>
            {
                tweenMain.SeparateUrlAndFullwidthCharacter = false;
                Assert.Equal("https://example.com/あああ", tweenMain.FormatStatusText("https://example.com/あああ"));
            });
        }

        [WinFormsFact]
        public void FormatStatusText_ReplaceFullwidthSpace_EnabledTest()
        {
            this.UsingTweenMain((tweenMain, context) =>
            {
                context.Settings.Common.WideSpaceConvert = true;
                Assert.Equal("aaa bbb", tweenMain.FormatStatusText("aaa　bbb"));
            });
        }

        [WinFormsFact]
        public void FormatStatusText_ReplaceFullwidthSpaceInDM_EnabledTest()
        {
            this.UsingTweenMain((tweenMain, context) =>
            {
                context.Settings.Common.WideSpaceConvert = true;

                // DM にも適用する
                Assert.Equal("D opentween aaa bbb", tweenMain.FormatStatusText("D opentween aaa　bbb"));
            });
        }

        [WinFormsFact]
        public void FormatStatusText_ReplaceFullwidthSpace_DisabledTest()
        {
            this.UsingTweenMain((tweenMain, context) =>
            {
                context.Settings.Common.WideSpaceConvert = false;
                Assert.Equal("aaa　bbb", tweenMain.FormatStatusText("aaa　bbb"));
            });
        }

        [WinFormsFact]
        public void FormatStatusText_UseRecommendedFooter_Test()
        {
            this.UsingTweenMain((tweenMain, context) =>
            {
                context.Settings.Local.UseRecommendStatus = true;
                Assert.Matches(new Regex(@"^aaa \[TWNv\d+\]$"), tweenMain.FormatStatusText("aaa"));
            });
        }

        [WinFormsFact]
        public void FormatStatusText_CustomFooterText_Test()
        {
            this.UsingTweenMain((tweenMain, context) =>
            {
                context.Settings.Local.StatusText = "foo";
                Assert.Equal("aaa foo", tweenMain.FormatStatusText("aaa"));
            });
        }

        [WinFormsFact]
        public void FormatStatusText_DisableFooterIfSendingDM_Test()
        {
            this.UsingTweenMain((tweenMain, context) =>
            {
                context.Settings.Local.StatusText = "foo";

                // DM の場合はフッターを無効化する
                Assert.Equal("D opentween aaa", tweenMain.FormatStatusText("D opentween aaa"));
            });
        }

        [WinFormsFact]
        public void FormatStatusText_DisableFooterIfContainsUnofficialRT_Test()
        {
            this.UsingTweenMain((tweenMain, context) =>
            {
                context.Settings.Local.StatusText = "foo";

                // 非公式 RT を含む場合はフッターを無効化する
                Assert.Equal("aaa RT @foo: bbb", tweenMain.FormatStatusText("aaa RT @foo: bbb"));
            });
        }

        [WinFormsFact]
        public void FormatStatusText_PreventSmsCommand_Test()
        {
            this.UsingTweenMain((tweenMain, context) =>
            {
                // 「D+」などから始まる文字列をツイートしようとすると SMS コマンドと誤認されてエラーが返される問題の回避策
                Assert.Equal("\u200bd+aaaa", tweenMain.FormatStatusText("d+aaaa"));
            });
        }

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
            var statusText = """<a href="http://t.co/KYi7vMZzRt" title="http://twitter.com/">twitter.com</a>""";

            Assert.Equal("http://twitter.com/", TweenMain.CreateRetweetUnofficial(statusText, false));
        }

        [Fact]
        public void CreateRetweetUnofficial_MentionTest()
        {
            var statusText = """<a class="mention" href="https://twitter.com/twitterapi">@TwitterAPI</a>""";

            Assert.Equal("@TwitterAPI", TweenMain.CreateRetweetUnofficial(statusText, false));
        }

        [Fact]
        public void CreateRetweetUnofficial_HashtagTest()
        {
            var statusText = """<a class="hashtag" href="https://twitter.com/search?q=%23OpenTween">#OpenTween</a>""";

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
