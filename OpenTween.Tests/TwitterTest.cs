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

            var expected = ("web", (Uri)null);
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

            var expected = ("", (Uri)null);
            Assert.Equal(expected, Twitter.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_NullTest()
        {
            string sourceHtml = null;

            var expected = ("", (Uri)null);
            Assert.Equal(expected, Twitter.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_UnescapeTest()
        {
            string sourceHtml = "<a href=\"http://example.com/?aaa=123&amp;bbb=456\" rel=\"nofollow\">&lt;&lt;hogehoge&gt;&gt;</a>";

            var expected = ("<<hogehoge>>", new Uri("http://example.com/?aaa=123&bbb=456"));
            Assert.Equal(expected, Twitter.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_UnescapeNoUriTest()
        {
            string sourceHtml = "&lt;&lt;hogehoge&gt;&gt;";

            var expected = ("<<hogehoge>>", (Uri)null);
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

            var statusIds = Twitter.GetQuoteTweetStatusIds(entities);
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
            var dm = 20;  // DMは固定値
            var more = SettingManager.Common.MoreCountApi;
            var startup = SettingManager.Common.FirstCountApi;
            var favorite = SettingManager.Common.FavoritesCountApi;
            var list = SettingManager.Common.ListCountApi;
            var search = SettingManager.Common.SearchCountApi;
            var usertl = SettingManager.Common.UserTimelineCountApi;

            // デフォルト値チェック
            Assert.Equal(false, SettingManager.Common.UseAdditionalCount);
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

            // DM
            Assert.Equal(dm, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.DirectMessegeRcv, false, false));
            Assert.Equal(dm, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.DirectMessegeSnt, false, false));

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
            var dm = 20;  // DMは固定値
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
            Assert.Equal(reply, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Reply, false, true));  //Replyの値が使われる

            // DM
            Assert.Equal(dm, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.DirectMessegeRcv, false, false));
            Assert.Equal(dm, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.DirectMessegeSnt, false, false));

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
            Assert.Equal(search, Twitter.GetApiResultCount(MyCommon.WORKERTYPE.PublicSearch, true, false));  //MoreCountApiの値がPublicSearchの最大値に制限される
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
            using (var twitter = new Twitter())
            {
                Assert.Equal(140, twitter.GetTextLengthRemain(""));
                Assert.Equal(132, twitter.GetTextLengthRemain("hogehoge"));
            }
        }

        [Fact]
        public void GetTextLengthRemain_DirectMessageTest()
        {
            using (var twitter = new Twitter())
            {
                // 2015年8月から DM の文字数上限が 10,000 文字に変更された
                // https://twittercommunity.com/t/41348
                twitter.Configuration.DmTextCharacterLimit = 10000;

                Assert.Equal(10000, twitter.GetTextLengthRemain("D twitter "));
                Assert.Equal(9992, twitter.GetTextLengthRemain("D twitter hogehoge"));
            }
        }

        [Fact]
        public void GetTextLengthRemain_UrlTest()
        {
            using (var twitter = new Twitter())
            {
                // t.co に短縮される分の文字数を考慮
                twitter.Configuration.ShortUrlLength = 20;
                Assert.Equal(120, twitter.GetTextLengthRemain("http://example.com/"));
                Assert.Equal(120, twitter.GetTextLengthRemain("http://example.com/hogehoge"));
                Assert.Equal(111, twitter.GetTextLengthRemain("hogehoge http://example.com/"));

                twitter.Configuration.ShortUrlLengthHttps = 21;
                Assert.Equal(119, twitter.GetTextLengthRemain("https://example.com/"));
                Assert.Equal(119, twitter.GetTextLengthRemain("https://example.com/hogehoge"));
                Assert.Equal(110, twitter.GetTextLengthRemain("hogehoge https://example.com/"));
            }
        }

        [Fact]
        public void GetTextLengthRemain_UrlWithoutSchemeTest()
        {
            using (var twitter = new Twitter())
            {
                // t.co に短縮される分の文字数を考慮
                twitter.Configuration.ShortUrlLength = 20;
                Assert.Equal(120, twitter.GetTextLengthRemain("example.com"));
                Assert.Equal(120, twitter.GetTextLengthRemain("example.com/hogehoge"));
                Assert.Equal(111, twitter.GetTextLengthRemain("hogehoge example.com"));

                // スキーム (http://) を省略かつ末尾が ccTLD の場合は t.co に短縮されない
                Assert.Equal(130, twitter.GetTextLengthRemain("example.jp"));
                // ただし、末尾にパスが続く場合は t.co に短縮される
                Assert.Equal(120, twitter.GetTextLengthRemain("example.jp/hogehoge"));
            }
        }

        [Fact]
        public void GetTextLengthRemain_SurrogatePairTest()
        {
            using (var twitter = new Twitter())
            {
                Assert.Equal(139, twitter.GetTextLengthRemain("🍣"));
                Assert.Equal(133, twitter.GetTextLengthRemain("🔥🐔🔥 焼き鳥"));
            }
        }
    }
}
