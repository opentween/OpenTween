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
using OpenTween.Api.DataModel;
using Xunit;

namespace OpenTween.Models
{
    public class TwitterPostFactoryTest
    {
        private static readonly ISet<long> EmptyIdSet = new HashSet<long>();

        private readonly Random random = new();

        public TwitterPostFactoryTest()
            => PostClass.ExpandedUrlInfo.AutoExpand = false;

        private TabInformations CreateTabinfo()
        {
            var tabinfo = new TabInformations();
            tabinfo.AddDefaultTabs();
            return tabinfo;
        }

        private TwitterStatus CreateStatus()
        {
            var statusId = this.random.Next(10000);

            return new()
            {
                Id = statusId,
                IdStr = statusId.ToString(),
                CreatedAt = "Sat Jan 01 00:00:00 +0000 2022",
                FullText = "hoge",
                Source = "<a href=\"https://www.opentween.org/\" rel=\"nofollow\">OpenTween</a>",
                Entities = new(),
                User = this.CreateUser(),
            };
        }

        private TwitterUser CreateUser()
        {
            var userId = this.random.Next(10000);

            return new()
            {
                Id = userId,
                IdStr = userId.ToString(),
                ScreenName = "tetete",
                Name = "ててて",
                ProfileImageUrlHttps = "https://example.com/profile.png",
            };
        }

        [Fact]
        public void CreateFromStatus_Test()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());
            var status = this.CreateStatus();
            var post = factory.CreateFromStatus(status, selfUserId: 20000L, followerIds: EmptyIdSet);

            Assert.Equal(status.Id, post.StatusId);
            Assert.Equal(new DateTimeUtc(2022, 1, 1, 0, 0, 0), post.CreatedAt);
            Assert.Equal("hoge", post.Text);
            Assert.Equal("hoge", post.TextFromApi);
            Assert.Equal("hoge", post.TextSingleLine);
            Assert.Equal("hoge", post.AccessibleText);
            Assert.Empty(post.ReplyToList);
            Assert.Empty(post.QuoteStatusIds);
            Assert.Empty(post.ExpandedUrls);
            Assert.Empty(post.Media);
            Assert.Null(post.PostGeo);
            Assert.Equal("OpenTween", post.Source);
            Assert.Equal("https://www.opentween.org/", post.SourceUri?.OriginalString);
            Assert.Equal(0, post.FavoritedCount);
            Assert.False(post.IsFav);
            Assert.False(post.IsDm);
            Assert.False(post.IsDeleted);
            Assert.False(post.IsRead);
            Assert.False(post.IsExcludeReply);
            Assert.False(post.FilterHit);
            Assert.False(post.IsMark);

            Assert.False(post.IsReply);
            Assert.Null(post.InReplyToStatusId);
            Assert.Null(post.InReplyToUserId);
            Assert.Null(post.InReplyToUser);

            Assert.Null(post.RetweetedId);
            Assert.Null(post.RetweetedBy);
            Assert.Null(post.RetweetedByUserId);

            Assert.Equal(status.User.Id, post.UserId);
            Assert.Equal("tetete", post.ScreenName);
            Assert.Equal("ててて", post.Nickname);
            Assert.Equal("https://example.com/profile.png", post.ImageUrl);
            Assert.False(post.IsProtect);
            Assert.False(post.IsOwl);
            Assert.False(post.IsMe);
        }

        [Fact]
        public void CreateFromStatus_AuthorTest()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());
            var status = this.CreateStatus();
            var selfUserId = status.User.Id;
            var post = factory.CreateFromStatus(status, selfUserId, followerIds: EmptyIdSet);

            Assert.True(post.IsMe);
        }

        [Fact]
        public void CreateFromStatus_FollowerTest()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());
            var status = this.CreateStatus();
            var followerIds = new HashSet<long> { status.User.Id };
            var post = factory.CreateFromStatus(status, selfUserId: 20000L, followerIds);

            Assert.False(post.IsOwl);
        }

        [Fact]
        public void CreateFromStatus_NotFollowerTest()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());
            var status = this.CreateStatus();
            var followerIds = new HashSet<long> { 30000L };
            var post = factory.CreateFromStatus(status, selfUserId: 20000L, followerIds);

            Assert.True(post.IsOwl);
        }

        [Fact]
        public void CreateFromStatus_RetweetTest()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());
            var originalStatus = this.CreateStatus();

            var retweetStatus = this.CreateStatus();
            retweetStatus.RetweetedStatus = originalStatus;
            retweetStatus.Source = "<a href=\"https://mobile.twitter.com\" rel=\"nofollow\">Twitter Web App</a>";

            var post = factory.CreateFromStatus(retweetStatus, selfUserId: 20000L, followerIds: EmptyIdSet);

            Assert.Equal(retweetStatus.Id, post.StatusId);
            Assert.Equal(retweetStatus.User.Id, post.RetweetedByUserId);
            Assert.Equal(originalStatus.Id, post.RetweetedId);
            Assert.Equal(originalStatus.User.Id, post.UserId);

            Assert.Equal("OpenTween", post.Source);
            Assert.Equal("https://www.opentween.org/", post.SourceUri?.OriginalString);
        }

        private TwitterMessageEvent CreateDirectMessage(string senderId, string recipientId)
        {
            var messageId = this.random.Next(10000);

            return new()
            {
                Type = "message_create",
                Id = messageId.ToString(),
                CreatedTimestamp = "1640995200000",
                MessageCreate = new()
                {
                    SenderId = senderId,
                    Target = new()
                    {
                        RecipientId = recipientId,
                    },
                    MessageData = new()
                    {
                        Text = "hoge",
                        Entities = new(),
                    },
                    SourceAppId = "22519141",
                },
            };
        }

        private Dictionary<string, TwitterMessageEventList.App> CreateApps()
        {
            return new()
            {
                ["22519141"] = new()
                {
                    Id = "22519141",
                    Name = "OpenTween",
                    Url = "https://www.opentween.org/",
                },
            };
        }

        [Fact]
        public void CreateFromDirectMessageEvent_Test()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());

            var selfUser = this.CreateUser();
            var otherUser = this.CreateUser();
            var eventItem = this.CreateDirectMessage(senderId: otherUser.IdStr, recipientId: selfUser.IdStr);
            var users = new Dictionary<string, TwitterUser>()
            {
                [selfUser.IdStr] = selfUser,
                [otherUser.IdStr] = otherUser,
            };
            var apps = this.CreateApps();
            var post = factory.CreateFromDirectMessageEvent(eventItem, users, apps, selfUserId: selfUser.Id);

            Assert.Equal(long.Parse(eventItem.Id), post.StatusId);
            Assert.Equal(new DateTimeUtc(2022, 1, 1, 0, 0, 0), post.CreatedAt);
            Assert.Equal("hoge", post.Text);
            Assert.Equal("hoge", post.TextFromApi);
            Assert.Equal("hoge", post.TextSingleLine);
            Assert.Equal("hoge", post.AccessibleText);
            Assert.Empty(post.ReplyToList);
            Assert.Empty(post.QuoteStatusIds);
            Assert.Empty(post.ExpandedUrls);
            Assert.Empty(post.Media);
            Assert.Null(post.PostGeo);
            Assert.Equal("OpenTween", post.Source);
            Assert.Equal("https://www.opentween.org/", post.SourceUri?.OriginalString);
            Assert.Equal(0, post.FavoritedCount);
            Assert.False(post.IsFav);
            Assert.True(post.IsDm);
            Assert.False(post.IsDeleted);
            Assert.False(post.IsRead);
            Assert.False(post.IsExcludeReply);
            Assert.False(post.FilterHit);
            Assert.False(post.IsMark);

            Assert.False(post.IsReply);
            Assert.Null(post.InReplyToStatusId);
            Assert.Null(post.InReplyToUserId);
            Assert.Null(post.InReplyToUser);

            Assert.Null(post.RetweetedId);
            Assert.Null(post.RetweetedBy);
            Assert.Null(post.RetweetedByUserId);

            Assert.Equal(otherUser.Id, post.UserId);
            Assert.Equal("tetete", post.ScreenName);
            Assert.Equal("ててて", post.Nickname);
            Assert.Equal("https://example.com/profile.png", post.ImageUrl);
            Assert.False(post.IsProtect);
            Assert.True(post.IsOwl);
            Assert.False(post.IsMe);
        }

        [Fact]
        public void CreateFromDirectMessageEvent_SenderTest()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());

            var selfUser = this.CreateUser();
            var otherUser = this.CreateUser();
            var eventItem = this.CreateDirectMessage(senderId: selfUser.IdStr, recipientId: otherUser.IdStr);
            var users = new Dictionary<string, TwitterUser>()
            {
                [selfUser.IdStr] = selfUser,
                [otherUser.IdStr] = otherUser,
            };
            var apps = this.CreateApps();
            var post = factory.CreateFromDirectMessageEvent(eventItem, users, apps, selfUserId: selfUser.Id);

            Assert.Equal(otherUser.Id, post.UserId);
            Assert.False(post.IsOwl);
            Assert.True(post.IsMe);
        }

        [Fact]
        public void CreateFromStatus_MediaAltTest()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());

            var status = this.CreateStatus();
            status.FullText = "https://t.co/hoge";
            status.ExtendedEntities = new()
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

            var post = factory.CreateFromStatus(status, selfUserId: 100L, followerIds: EmptyIdSet);

            var accessibleText = string.Format(Properties.Resources.ImageAltText, "代替テキスト");
            Assert.Equal(accessibleText, post.AccessibleText);
            Assert.Equal("<a href=\"https://t.co/hoge\" title=\"代替テキスト\">pic.twitter.com/hoge</a>", post.Text);
            Assert.Equal("pic.twitter.com/hoge", post.TextFromApi);
            Assert.Equal("pic.twitter.com/hoge", post.TextSingleLine);
        }

        [Fact]
        public void CreateFromStatus_MediaNoAltTest()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());

            var status = this.CreateStatus();
            status.FullText = "https://t.co/hoge";
            status.ExtendedEntities = new()
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

            var post = factory.CreateFromStatus(status, selfUserId: 100L, followerIds: EmptyIdSet);

            Assert.Equal("pic.twitter.com/hoge", post.AccessibleText);
            Assert.Equal("<a href=\"https://t.co/hoge\" title=\"https://twitter.com/hoge/status/1234567890/photo/1\">pic.twitter.com/hoge</a>", post.Text);
            Assert.Equal("pic.twitter.com/hoge", post.TextFromApi);
            Assert.Equal("pic.twitter.com/hoge", post.TextSingleLine);
        }

        [Fact]
        public void CreateFromStatus_QuotedUrlTest()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());

            var status = this.CreateStatus();
            status.FullText = "https://t.co/hoge";
            status.Entities = new()
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
            status.QuotedStatus = new()
            {
                Id = 1234567890L,
                IdStr = "1234567890",
                User = new()
                {
                    Id = 1111,
                    IdStr = "1111",
                    ScreenName = "foo",
                },
                FullText = "test",
            };

            var post = factory.CreateFromStatus(status, selfUserId: 100L, followerIds: EmptyIdSet);

            var accessibleText = string.Format(Properties.Resources.QuoteStatus_AccessibleText, "foo", "test");
            Assert.Equal(accessibleText, post.AccessibleText);
            Assert.Equal("<a href=\"https://twitter.com/hoge/status/1234567890\" title=\"https://twitter.com/hoge/status/1234567890\">twitter.com/hoge/status/1…</a>", post.Text);
            Assert.Equal("twitter.com/hoge/status/1…", post.TextFromApi);
            Assert.Equal("twitter.com/hoge/status/1…", post.TextSingleLine);
        }

        [Fact]
        public void CreateFromStatus_QuotedUrlWithPermelinkTest()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());

            var status = this.CreateStatus();
            status.FullText = "hoge";
            status.QuotedStatus = new()
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
            status.QuotedStatusPermalink = new()
            {
                Url = "https://t.co/hoge",
                Display = "twitter.com/hoge/status/1…",
                Expanded = "https://twitter.com/hoge/status/1234567890",
            };

            var post = factory.CreateFromStatus(status, selfUserId: 100L, followerIds: EmptyIdSet);

            var accessibleText = "hoge " + string.Format(Properties.Resources.QuoteStatus_AccessibleText, "foo", "test");
            Assert.Equal(accessibleText, post.AccessibleText);
            Assert.Equal("hoge <a href=\"https://twitter.com/hoge/status/1234567890\" title=\"https://twitter.com/hoge/status/1234567890\">twitter.com/hoge/status/1…</a>", post.Text);
            Assert.Equal("hoge twitter.com/hoge/status/1…", post.TextFromApi);
            Assert.Equal("hoge twitter.com/hoge/status/1…", post.TextSingleLine);
        }

        [Fact]
        public void CreateFromStatus_QuotedUrlNoReferenceTest()
        {
            var factory = new TwitterPostFactory(this.CreateTabinfo());

            var status = this.CreateStatus();
            status.FullText = "https://t.co/hoge";
            status.Entities = new()
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
            status.QuotedStatus = null;

            var post = factory.CreateFromStatus(status, selfUserId: 100L, followerIds: EmptyIdSet);

            var accessibleText = "twitter.com/hoge/status/1…";
            Assert.Equal(accessibleText, post.AccessibleText);
            Assert.Equal("<a href=\"https://twitter.com/hoge/status/1234567890\" title=\"https://twitter.com/hoge/status/1234567890\">twitter.com/hoge/status/1…</a>", post.Text);
            Assert.Equal("twitter.com/hoge/status/1…", post.TextFromApi);
            Assert.Equal("twitter.com/hoge/status/1…", post.TextSingleLine);
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

            Assert.Equal(expectedHtml, TwitterPostFactory.CreateHtmlAnchor(text, entities, quotedStatusLink: null));
        }

        [Fact]
        public void CreateHtmlAnchor_NicovideoTest()
        {
            var text = "sm9";
            var entities = new TwitterEntities();

            var expectedHtml = @"<a href=""https://www.nicovideo.jp/watch/sm9"">sm9</a>";

            Assert.Equal(expectedHtml, TwitterPostFactory.CreateHtmlAnchor(text, entities, quotedStatusLink: null));
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
                + @" <a href=""https://twitter.com/hoge/status/1234567890"" title=""https://twitter.com/hoge/status/1234567890"">twitter.com/hoge/status/1…</a>";

            Assert.Equal(expectedHtml, TwitterPostFactory.CreateHtmlAnchor(text, entities, quotedStatusLink));
        }

        [Fact]
        public void ParseSource_Test()
        {
            var sourceHtml = "<a href=\"http://twitter.com\" rel=\"nofollow\">Twitter Web Client</a>";

            var expected = ("Twitter Web Client", new Uri("http://twitter.com/"));
            Assert.Equal(expected, TwitterPostFactory.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_PlainTextTest()
        {
            var sourceHtml = "web";

            var expected = ("web", (Uri?)null);
            Assert.Equal(expected, TwitterPostFactory.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_RelativeUriTest()
        {
            // 参照: https://twitter.com/kim_upsilon/status/477796052049752064
            var sourceHtml = "<a href=\"erased_45416\" rel=\"nofollow\">erased_45416</a>";

            var expected = ("erased_45416", new Uri("https://twitter.com/erased_45416"));
            Assert.Equal(expected, TwitterPostFactory.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_EmptyTest()
        {
            // 参照: https://twitter.com/kim_upsilon/status/595156014032244738
            var sourceHtml = "";

            var expected = ("", (Uri?)null);
            Assert.Equal(expected, TwitterPostFactory.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_NullTest()
        {
            string? sourceHtml = null;

            var expected = ("", (Uri?)null);
            Assert.Equal(expected, TwitterPostFactory.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_UnescapeTest()
        {
            var sourceHtml = "<a href=\"http://example.com/?aaa=123&amp;bbb=456\" rel=\"nofollow\">&lt;&lt;hogehoge&gt;&gt;</a>";

            var expected = ("<<hogehoge>>", new Uri("http://example.com/?aaa=123&bbb=456"));
            Assert.Equal(expected, TwitterPostFactory.ParseSource(sourceHtml));
        }

        [Fact]
        public void ParseSource_UnescapeNoUriTest()
        {
            var sourceHtml = "&lt;&lt;hogehoge&gt;&gt;";

            var expected = ("<<hogehoge>>", (Uri?)null);
            Assert.Equal(expected, TwitterPostFactory.ParseSource(sourceHtml));
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

            var statusIds = TwitterPostFactory.GetQuoteTweetStatusIds(entities, quotedStatusLink: null);
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

            var statusIds = TwitterPostFactory.GetQuoteTweetStatusIds(entities, quotedStatusLink);
            Assert.Equal(new[] { 599261132361072640L }, statusIds);
        }

        [Fact]
        public void GetQuoteTweetStatusIds_UrlStringTest()
        {
            var urls = new[]
            {
                "https://twitter.com/kim_upsilon/status/599261132361072640",
            };

            var statusIds = TwitterPostFactory.GetQuoteTweetStatusIds(urls);
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

            var statusIds = TwitterPostFactory.GetQuoteTweetStatusIds(urls);
            Assert.Empty(statusIds);
        }
    }
}
