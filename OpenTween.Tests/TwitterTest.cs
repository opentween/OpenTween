// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text.RegularExpressions;
using OpenTween.Api;
using OpenTween.Api.DataModel;
using OpenTween.Models;
using OpenTween.Setting;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class TwitterTest
    {
        [Theory]
        [InlineData("https://twitter.com/twitterapi/status/22634515958",
            new[] { "22634515958" })]
        [InlineData("<a target=\"_self\" href=\"https://t.co/aaaaaaaa\" title=\"https://twitter.com/twitterapi/status/22634515958\">twitter.com/twitterapi/stat…</a>",
            new[] { "22634515958" })]
        [InlineData("<a target=\"_self\" href=\"https://t.co/bU3oR95KIy\" title=\"https://twitter.com/haru067/status/224782458816692224\">https://t.co/bU3oR95KIy</a>" +
            "<a target=\"_self\" href=\"https://t.co/bbbbbbbb\" title=\"https://twitter.com/karno/status/311081657790771200\">https://t.co/bbbbbbbb</a>",
            new[] { "224782458816692224", "311081657790771200" })]
        [InlineData("https://mobile.twitter.com/muji_net/status/21984934471",
            new[] { "21984934471" })]
        [InlineData("https://twitter.com/imgazyobuzi/status/293333871171354624/photo/1",
            new[] { "293333871171354624" })]
        public void StatusUrlRegexTest(string url, string[] expected)
        {
            var results = Twitter.StatusUrlRegex.Matches(url).Cast<Match>()
                .Select(x => x.Groups["StatusId"].Value).ToArray();

            Assert.Equal(expected, results);
        }

        [Theory]
        [InlineData("https://twitter.com/twitterapi/status/22634515958", true)]
        [InlineData("http://twitter.com/twitterapi/status/22634515958", true)]
        [InlineData("https://mobile.twitter.com/twitterapi/status/22634515958", true)]
        [InlineData("http://mobile.twitter.com/twitterapi/status/22634515958", true)]
        [InlineData("https://twitter.com/i/web/status/22634515958", false)]
        [InlineData("https://twitter.com/imgazyobuzi/status/293333871171354624/photo/1", false)]
        [InlineData("https://pic.twitter.com/gbxdb2Oj", false)]
        [InlineData("https://twitter.com/messages/compose?recipient_id=514241801", true)]
        [InlineData("http://twitter.com/messages/compose?recipient_id=514241801", true)]
        [InlineData("https://twitter.com/messages/compose?recipient_id=514241801&text=%E3%81%BB%E3%81%92", true)]
        public void AttachmentUrlRegexTest(string url, bool isMatch)
            => Assert.Equal(isMatch, Twitter.AttachmentUrlRegex.IsMatch(url));

        [Theory]
        [InlineData("http://favstar.fm/users/twitterapi/status/22634515958", new[] { "22634515958" })]
        [InlineData("http://ja.favstar.fm/users/twitterapi/status/22634515958", new[] { "22634515958" })]
        [InlineData("http://favstar.fm/t/22634515958", new[] { "22634515958" })]
        [InlineData("http://aclog.koba789.com/i/312485321239564288", new[] { "312485321239564288" })]
        [InlineData("http://frtrt.net/solo_status.php?status=263483634307198977", new[] { "263483634307198977" })]
        public void ThirdPartyStatusUrlRegexTest(string url, string[] expected)
        {
            var results = Twitter.ThirdPartyStatusUrlRegex.Matches(url).Cast<Match>()
                .Select(x => x.Groups["StatusId"].Value).ToArray();

            Assert.Equal(expected, results);
        }

        [Fact]
        public void FindTopOfReplyChainTest()
        {
            var posts = new Dictionary<long, PostClass>
            {
                [950L] = new PostClass { StatusId = 950L, InReplyToStatusId = null }, // このツイートが末端
                [987L] = new PostClass { StatusId = 987L, InReplyToStatusId = 950L },
                [999L] = new PostClass { StatusId = 999L, InReplyToStatusId = 987L },
                [1000L] = new PostClass { StatusId = 1000L, InReplyToStatusId = 999L },
            };
            Assert.Equal(950L, Twitter.FindTopOfReplyChain(posts, 1000L).StatusId);
            Assert.Equal(950L, Twitter.FindTopOfReplyChain(posts, 950L).StatusId);
            Assert.Throws<ArgumentException>(() => Twitter.FindTopOfReplyChain(posts, 500L));

            posts = new Dictionary<long, PostClass>
            {
                // 1200L は posts の中に存在しない
                [1210L] = new PostClass { StatusId = 1210L, InReplyToStatusId = 1200L },
                [1220L] = new PostClass { StatusId = 1220L, InReplyToStatusId = 1210L },
                [1230L] = new PostClass { StatusId = 1230L, InReplyToStatusId = 1220L },
            };
            Assert.Equal(1210L, Twitter.FindTopOfReplyChain(posts, 1230L).StatusId);
            Assert.Equal(1210L, Twitter.FindTopOfReplyChain(posts, 1210L).StatusId);
        }

        [Fact]
        public void CreateAccessibleText_MediaAltTest()
        {
            var text = "https://t.co/hoge";
            var entities = new TwitterEntities
            {
                Media = new[]
                {
                    new TwitterEntityMedia
                    {
                        Indices = new[] { 0, 17 },
                        Url = "https://t.co/hoge",
                        DisplayUrl = "pic.twitter.com/hoge",
                        ExpandedUrl = "https://twitter.com/hoge/status/1234567890/photo/1",
                        AltText = "代替テキスト",
                    },
                },
            };

            var expectedText = string.Format(Properties.Resources.ImageAltText, "代替テキスト");

            Assert.Equal(expectedText, Twitter.CreateAccessibleText(text, entities, quotedStatus: null, quotedStatusLink: null));
        }

        [Fact]
        public void CreateAccessibleText_MediaNoAltTest()
        {
            var text = "https://t.co/hoge";
            var entities = new TwitterEntities
            {
                Media = new[]
                {
                    new TwitterEntityMedia
                    {
                        Indices = new[] { 0, 17 },
                        Url = "https://t.co/hoge",
                        DisplayUrl = "pic.twitter.com/hoge",
                        ExpandedUrl = "https://twitter.com/hoge/status/1234567890/photo/1",
                        AltText = null,
                    },
                },
            };

            var expectedText = "pic.twitter.com/hoge";

            Assert.Equal(expectedText, Twitter.CreateAccessibleText(text, entities, quotedStatus: null, quotedStatusLink: null));
        }

        [Fact]
        public void CreateAccessibleText_QuotedUrlTest()
        {
            var text = "https://t.co/hoge";
            var entities = new TwitterEntities
            {
                Urls = new[]
                {
                    new TwitterEntityUrl
                    {
                        Indices = new[] { 0, 17 },
                        Url = "https://t.co/hoge",
                        DisplayUrl = "twitter.com/hoge/status/1…",
                        ExpandedUrl = "https://twitter.com/hoge/status/1234567890",
                    },
                },
            };
            var quotedStatus = new TwitterStatus
            {
                Id = 1234567890L,
                IdStr = "1234567890",
                User = new TwitterUser
                {
                    Id = 1111,
                    IdStr = "1111",
                    ScreenName = "foo",
                },
                FullText = "test",
            };

            var expectedText = string.Format(Properties.Resources.QuoteStatus_AccessibleText, "foo", "test");

            Assert.Equal(expectedText, Twitter.CreateAccessibleText(text, entities, quotedStatus, quotedStatusLink: null));
        }

        [Fact]
        public void CreateAccessibleText_QuotedUrlWithPermelinkTest()
        {
            var text = "hoge";
            var entities = new TwitterEntities();
            var quotedStatus = new TwitterStatus
            {
                Id = 1234567890L,
                IdStr = "1234567890",
                User = new TwitterUser
                {
                    Id = 1111,
                    IdStr = "1111",
                    ScreenName = "foo",
                },
                FullText = "test",
            };
            var quotedStatusLink = new TwitterQuotedStatusPermalink
            {
                Url = "https://t.co/hoge",
                Display = "twitter.com/hoge/status/1…",
                Expanded = "https://twitter.com/hoge/status/1234567890",
            };

            var expectedText = "hoge " + string.Format(Properties.Resources.QuoteStatus_AccessibleText, "foo", "test");

            Assert.Equal(expectedText, Twitter.CreateAccessibleText(text, entities, quotedStatus, quotedStatusLink));
        }

        [Fact]
        public void CreateAccessibleText_QuotedUrlNoReferenceTest()
        {
            var text = "https://t.co/hoge";
            var entities = new TwitterEntities
            {
                Urls = new[]
                {
                    new TwitterEntityUrl
                    {
                        Indices = new[] { 0, 17 },
                        Url = "https://t.co/hoge",
                        DisplayUrl = "twitter.com/hoge/status/1…",
                        ExpandedUrl = "https://twitter.com/hoge/status/1234567890",
                    },
                },
            };
            var quotedStatus = (TwitterStatus?)null;

            var expectedText = "twitter.com/hoge/status/1…";

            Assert.Equal(expectedText, Twitter.CreateAccessibleText(text, entities, quotedStatus, quotedStatusLink: null));
        }

        [Fact]
        public void CreateHtmlAnchor_Test()
        {
            var text = "@twitterapi #BreakingMyTwitter https://t.co/mIJcSoVSK3";
            var entities = new TwitterEntities
            {
                UserMentions = new[]
                {
                    new TwitterEntityMention { Indices = new[] { 0, 11 }, ScreenName = "twitterapi" },
                },
                Hashtags = new[]
                {
                    new TwitterEntityHashtag { Indices = new[] { 12, 30 }, Text = "BreakingMyTwitter" },
                },
                Urls = new[]
                {
                    new TwitterEntityUrl
                    {
                        Indices = new[] { 31, 54 },
                        Url = "https://t.co/mIJcSoVSK3",
                        DisplayUrl = "apps-of-a-feather.com",
                        ExpandedUrl = "http://apps-of-a-feather.com/",
                    },
                },
            };

            var expectedHtml = @"<a class=""mention"" href=""https://twitter.com/twitterapi"">@twitterapi</a>"
                + @" <a class=""hashtag"" href=""https://twitter.com/search?q=%23BreakingMyTwitter"">#BreakingMyTwitter</a>"
                + @" <a href=""https://t.co/mIJcSoVSK3"" title=""https://t.co/mIJcSoVSK3"">apps-of-a-feather.com</a>";

            Assert.Equal(expectedHtml, Twitter.CreateHtmlAnchor(text, entities, quotedStatusLink: null));
        }

        [Fact]
        public void CreateHtmlAnchor_NicovideoTest()
        {
            var text = "sm9";
            var entities = new TwitterEntities();

            var expectedHtml = @"<a href=""https://www.nicovideo.jp/watch/sm9"">sm9</a>";

            Assert.Equal(expectedHtml, Twitter.CreateHtmlAnchor(text, entities, quotedStatusLink: null));
        }

        [Fact]
        public void CreateHtmlAnchor_QuotedUrlWithPermelinkTest()
        {
            var text = "hoge";
            var entities = new TwitterEntities();
            var quotedStatusLink = new TwitterQuotedStatusPermalink
            {
                Url = "https://t.co/hoge",
                Display = "twitter.com/hoge/status/1…",
                Expanded = "https://twitter.com/hoge/status/1234567890",
            };

            var expectedHtml = @"hoge"
                + @" <a href=""https://t.co/hoge"" title=""https://t.co/hoge"">twitter.com/hoge/status/1…</a>";

            Assert.Equal(expectedHtml, Twitter.CreateHtmlAnchor(text, entities, quotedStatusLink));
        }

        [Fact]
        public void ParseSource_Test()
        {
            var sourceHtml = "<a href=\"http://twitter.com\" rel=\"nofollow\">Twitter Web Client</a>";

            var expected = ("Twitter Web Client", new Uri("http://twitter.com/"));
            Assert.Equal(expected, Twitter.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_PlainTextTest()
        {
            var sourceHtml = "web";

            var expected = ("web", (Uri?)null);
            Assert.Equal(expected, Twitter.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_RelativeUriTest()
        {
            // 参照: https://twitter.com/kim_upsilon/status/477796052049752064
            var sourceHtml = "<a href=\"erased_45416\" rel=\"nofollow\">erased_45416</a>";

            var expected = ("erased_45416", new Uri("https://twitter.com/erased_45416"));
            Assert.Equal(expected, Twitter.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_EmptyTest()
        {
            // 参照: https://twitter.com/kim_upsilon/status/595156014032244738
            var sourceHtml = "";

            var expected = ("", (Uri?)null);
            Assert.Equal(expected, Twitter.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_NullTest()
        {
            string? sourceHtml = null;

            var expected = ("", (Uri?)null);
            Assert.Equal(expected, Twitter.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_UnescapeTest()
        {
            var sourceHtml = "<a href=\"http://example.com/?aaa=123&amp;bbb=456\" rel=\"nofollow\">&lt;&lt;hogehoge&gt;&gt;</a>";

            var expected = ("<<hogehoge>>", new Uri("http://example.com/?aaa=123&bbb=456"));
            Assert.Equal(expected, Twitter.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_UnescapeNoUriTest()
        {
            var sourceHtml = "&lt;&lt;hogehoge&gt;&gt;";

            var expected = ("<<hogehoge>>", (Uri?)null);
            Assert.Equal(expected, Twitter.ParseSource(sourceHtml));
        }

        [Fact]
        public void GetQuoteTweetStatusIds_EntityTest()
        {
            var entities = new[]
            {
                new TwitterEntityUrl
                {
                    Url = "https://t.co/3HXq0LrbJb",
                    ExpandedUrl = "https://twitter.com/kim_upsilon/status/599261132361072640",
                },
            };

            var statusIds = Twitter.GetQuoteTweetStatusIds(entities, quotedStatusLink: null);
            Assert.Equal(new[] { 599261132361072640L }, statusIds);
        }

        [Fact]
        public void GetQuoteTweetStatusIds_QuotedStatusLinkTest()
        {
            var entities = new TwitterEntities();
            var quotedStatusLink = new TwitterQuotedStatusPermalink
            {
                Url = "https://t.co/3HXq0LrbJb",
                Expanded = "https://twitter.com/kim_upsilon/status/599261132361072640",
            };

            var statusIds = Twitter.GetQuoteTweetStatusIds(entities, quotedStatusLink);
            Assert.Equal(new[] { 599261132361072640L }, statusIds);
        }

        [Fact]
        public void GetQuoteTweetStatusIds_UrlStringTest()
        {
            var urls = new[]
            {
                "https://twitter.com/kim_upsilon/status/599261132361072640",
            };

            var statusIds = Twitter.GetQuoteTweetStatusIds(urls);
            Assert.Equal(new[] { 599261132361072640L }, statusIds);
        }

        [Fact]
        public void GetQuoteTweetStatusIds_OverflowTest()
        {
            var urls = new[]
            {
                // 符号付き 64 ビット整数の範囲を超える値
                "https://twitter.com/kim_upsilon/status/9999999999999999999",
            };

            var statusIds = Twitter.GetQuoteTweetStatusIds(urls);
            Assert.Empty(statusIds);
        }

        [Fact]
        public void GetApiResultCount_DefaultTest()
        {
            var oldInstance = SettingManagerTest.Common;
            SettingManagerTest.Common = new SettingCommon();

            var timeline = SettingManager.Common.CountApi;
            var reply = SettingManager.Common.CountApiReply;
            var more = SettingManager.Common.MoreCountApi;
            var startup = SettingManager.Common.FirstCountApi;
            var favorite = SettingManager.Common.FavoritesCountApi;
            var list = SettingManager.Common.ListCountApi;
            var search = SettingManager.Common.SearchCountApi;
            var usertl = SettingManager.Common.UserTimelineCountApi;

            // デフォルト値チェック
            Assert.False(SettingManager.Common.UseAdditionalCount);
            Assert.Equal(60, timeline);
            Assert.Equal(40, reply);
            Assert.Equal(200, more);
            Assert.Equal(100, startup);
            Assert.Equal(40, favorite);
            Assert.Equal(100, list);
            Assert.Equal(100, search);
            Assert.Equal(20, usertl);

            // Timeline,Reply
            Assert.Equal(timeline, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Timeline, false, false));
            Assert.Equal(reply, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Reply, false, false));

            // その他はTimelineと同値になる
            Assert.Equal(timeline, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Favorites, false, false));
            Assert.Equal(timeline, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.List, false, false));
            Assert.Equal(timeline, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.PublicSearch, false, false));
            Assert.Equal(timeline, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.UserTimeline, false, false));

            SettingManagerTest.Common = oldInstance;
        }

        [Fact]
        public void GetApiResultCount_AdditionalCountTest()
        {
            var oldInstance = SettingManagerTest.Common;
            SettingManagerTest.Common = new SettingCommon();

            var timeline = SettingManager.Common.CountApi;
            var reply = SettingManager.Common.CountApiReply;
            var more = SettingManager.Common.MoreCountApi;
            var startup = SettingManager.Common.FirstCountApi;
            var favorite = SettingManager.Common.FavoritesCountApi;
            var list = SettingManager.Common.ListCountApi;
            var search = SettingManager.Common.SearchCountApi;
            var usertl = SettingManager.Common.UserTimelineCountApi;

            SettingManager.Common.UseAdditionalCount = true;

            // Timeline
            Assert.Equal(timeline, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Timeline, false, false));
            Assert.Equal(more, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Timeline, true, false));
            Assert.Equal(startup, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Timeline, false, true));

            // Reply
            Assert.Equal(reply, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Reply, false, false));
            Assert.Equal(more, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Reply, true, false));
            Assert.Equal(reply, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Reply, false, true));  // Replyの値が使われる

            // Favorites
            Assert.Equal(favorite, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Favorites, false, false));
            Assert.Equal(favorite, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Favorites, true, false));
            Assert.Equal(favorite, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Favorites, false, true));

            SettingManager.Common.FavoritesCountApi = 0;

            Assert.Equal(timeline, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Favorites, false, false));
            Assert.Equal(more, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Favorites, true, false));
            Assert.Equal(startup, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Favorites, false, true));

            // List
            Assert.Equal(list, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.List, false, false));
            Assert.Equal(list, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.List, true, false));
            Assert.Equal(list, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.List, false, true));

            SettingManager.Common.ListCountApi = 0;

            Assert.Equal(timeline, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.List, false, false));
            Assert.Equal(more, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.List, true, false));
            Assert.Equal(startup, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.List, false, true));

            // PublicSearch
            Assert.Equal(search, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.PublicSearch, false, false));
            Assert.Equal(search, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.PublicSearch, true, false));
            Assert.Equal(search, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.PublicSearch, false, true));

            SettingManager.Common.SearchCountApi = 0;

            Assert.Equal(timeline, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.PublicSearch, false, false));
            Assert.Equal(search, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.PublicSearch, true, false));  // MoreCountApiの値がPublicSearchの最大値に制限される
            Assert.Equal(startup, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.PublicSearch, false, true));

            // UserTimeline
            Assert.Equal(usertl, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.UserTimeline, false, false));
            Assert.Equal(usertl, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.UserTimeline, true, false));
            Assert.Equal(usertl, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.UserTimeline, false, true));

            SettingManager.Common.UserTimelineCountApi = 0;

            Assert.Equal(timeline, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.UserTimeline, false, false));
            Assert.Equal(more, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.UserTimeline, true, false));
            Assert.Equal(startup, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.UserTimeline, false, true));

            SettingManagerTest.Common = oldInstance;
        }

        [Fact]
        public void GetTextLengthRemain_Test()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);

            Assert.Equal(280, twitter.GetTextLengthRemain(""));
            Assert.Equal(272, twitter.GetTextLengthRemain("hogehoge"));
        }

        [Fact]
        public void GetTextLengthRemain_DirectMessageTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);

            // 2015年8月から DM の文字数上限が 10,000 文字に変更された
            // https://twittercommunity.com/t/41348
            twitter.Configuration.DmTextCharacterLimit = 10000;

            Assert.Equal(10000, twitter.GetTextLengthRemain("D twitter "));
            Assert.Equal(9992, twitter.GetTextLengthRemain("D twitter hogehoge"));

            // t.co に短縮される分の文字数を考慮
            twitter.Configuration.ShortUrlLength = 20;
            Assert.Equal(9971, twitter.GetTextLengthRemain("D twitter hogehoge http://example.com/"));

            twitter.Configuration.ShortUrlLengthHttps = 21;
            Assert.Equal(9970, twitter.GetTextLengthRemain("D twitter hogehoge https://example.com/"));
        }

        [Fact]
        public void GetTextLengthRemain_UrlTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);

            // t.co に短縮される分の文字数を考慮
            twitter.TextConfiguration.TransformedURLLength = 20;
            Assert.Equal(260, twitter.GetTextLengthRemain("http://example.com/"));
            Assert.Equal(260, twitter.GetTextLengthRemain("http://example.com/hogehoge"));
            Assert.Equal(251, twitter.GetTextLengthRemain("hogehoge http://example.com/"));

            Assert.Equal(260, twitter.GetTextLengthRemain("https://example.com/"));
            Assert.Equal(260, twitter.GetTextLengthRemain("https://example.com/hogehoge"));
            Assert.Equal(251, twitter.GetTextLengthRemain("hogehoge https://example.com/"));
        }

        [Fact]
        public void GetTextLengthRemain_UrlWithoutSchemeTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);

            // t.co に短縮される分の文字数を考慮
            twitter.TextConfiguration.TransformedURLLength = 20;
            Assert.Equal(260, twitter.GetTextLengthRemain("example.com"));
            Assert.Equal(260, twitter.GetTextLengthRemain("example.com/hogehoge"));
            Assert.Equal(251, twitter.GetTextLengthRemain("hogehoge example.com"));

            // スキーム (http://) を省略かつ末尾が ccTLD の場合は t.co に短縮されない
            Assert.Equal(270, twitter.GetTextLengthRemain("example.jp"));
            // ただし、末尾にパスが続く場合は t.co に短縮される
            Assert.Equal(260, twitter.GetTextLengthRemain("example.jp/hogehoge"));
        }

        [Fact]
        public void GetTextLengthRemain_SurrogatePairTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);

            Assert.Equal(278, twitter.GetTextLengthRemain("🍣"));
            Assert.Equal(267, twitter.GetTextLengthRemain("🔥🐔🔥 焼き鳥"));
        }

        [Fact]
        public void GetTextLengthRemain_EmojiTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);

            // 絵文字の文字数カウントの仕様変更に対するテストケース
            // https://twittercommunity.com/t/114607

            Assert.Equal(279, twitter.GetTextLengthRemain("©")); // 基本多言語面の絵文字
            Assert.Equal(277, twitter.GetTextLengthRemain("©\uFE0E")); // 異字体セレクタ付き (text style)
            Assert.Equal(279, twitter.GetTextLengthRemain("©\uFE0F")); // 異字体セレクタ付き (emoji style)
            Assert.Equal(278, twitter.GetTextLengthRemain("🍣")); // 拡張面の絵文字
            Assert.Equal(279, twitter.GetTextLengthRemain("#⃣")); // 合字で表現される絵文字
            Assert.Equal(278, twitter.GetTextLengthRemain("👦\U0001F3FF")); // Emoji modifier 付きの絵文字
            Assert.Equal(278, twitter.GetTextLengthRemain("\U0001F3FF")); // Emoji modifier 単体
            Assert.Equal(278, twitter.GetTextLengthRemain("👨\u200D🎨")); // ZWJ で結合された絵文字
            Assert.Equal(278, twitter.GetTextLengthRemain("🏃\u200D♀\uFE0F")); // ZWJ と異字体セレクタを含む絵文字
        }

        [Fact]
        public void GetTextLengthRemain_BrokenSurrogateTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);

            // 投稿欄に IME から絵文字を入力すると HighSurrogate のみ入力された状態で TextChanged イベントが呼ばれることがある
            Assert.Equal(278, twitter.GetTextLengthRemain("\ud83d"));
            Assert.Equal(9999, twitter.GetTextLengthRemain("D twitter \ud83d"));
        }
    }
}
