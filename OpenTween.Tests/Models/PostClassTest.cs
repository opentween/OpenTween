// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace OpenTween.Models
{
    public class PostClassTest
    {
        private class PostClassGroup
        {
            private readonly Dictionary<long, PostClass> testCases;

            public PostClassGroup(params TestPostClass[] postClasses)
            {
                this.testCases = new Dictionary<long, PostClass>();
                foreach (var p in postClasses)
                {
                    p.Group = this;
                    this.testCases.Add(p.StatusId, p);
                }
            }

            public PostClass this[long id] => this.testCases[id];
        }

        private class TestPostClass : PostClass
        {
            public PostClassGroup? Group;

            protected override PostClass RetweetSource
            {
                get
                {
                    var retweetedId = this.RetweetedId!.Value;
                    var group = this.Group;
                    if (group == null)
                        throw new InvalidOperationException("TestPostClass needs group");

                    return group[retweetedId];
                }
            }
        }

        private readonly PostClassGroup postGroup;

        public PostClassTest()
        {
            this.postGroup = new PostClassGroup(
                new TestPostClass { StatusId = 1L },
                new TestPostClass { StatusId = 2L, IsFav = true },
                new TestPostClass { StatusId = 3L, IsFav = false, RetweetedId = 2L });
        }

        [Fact]
        public void CloneTest()
        {
            var post = new PostClass();
            var clonePost = post.Clone();

            TestUtils.CheckDeepCloning(post, clonePost);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("aaa\nbbb", "aaa bbb")]
        public void TextSingleLineTest(string text, string expected)
        {
            var post = new PostClass { TextFromApi = text };

            Assert.Equal(expected, post.TextSingleLine);
        }

        [Theory]
        [InlineData(1L, false)]
        [InlineData(2L, true)]
        [InlineData(3L, true)]
        public void GetIsFavTest(long statusId, bool expected)
            => Assert.Equal(expected, this.postGroup[statusId].IsFav);

        [Theory]
        [InlineData(2L, true)]
        [InlineData(2L, false)]
        [InlineData(3L, true)]
        [InlineData(3L, false)]
        public void SetIsFavTest(long statusId, bool isFav)
        {
            var post = this.postGroup[statusId];

            post.IsFav = isFav;
            Assert.Equal(isFav, post.IsFav);

            if (post.RetweetedId != null)
                Assert.Equal(isFav, this.postGroup[post.RetweetedId.Value].IsFav);
        }

#pragma warning disable SA1008 // Opening parenthesis should be spaced correctly
        [Theory]
        [InlineData(false, false, false, false, -0x01)]
        [InlineData( true, false, false, false, 0x00)]
        [InlineData(false,  true, false, false, 0x01)]
        [InlineData( true,  true, false, false, 0x02)]
        [InlineData(false, false,  true, false, 0x03)]
        [InlineData( true, false,  true, false, 0x04)]
        [InlineData(false,  true,  true, false, 0x05)]
        [InlineData( true,  true,  true, false, 0x06)]
        [InlineData(false, false, false,  true, 0x07)]
        [InlineData( true, false, false,  true, 0x08)]
        [InlineData(false,  true, false,  true, 0x09)]
        [InlineData( true,  true, false,  true, 0x0A)]
        [InlineData(false, false,  true,  true, 0x0B)]
        [InlineData( true, false,  true,  true, 0x0C)]
        [InlineData(false,  true,  true,  true, 0x0D)]
        [InlineData( true,  true,  true,  true, 0x0E)]
#pragma warning restore SA1008
        public void StateIndexTest(bool protect, bool mark, bool reply, bool geo, int expected)
        {
            var post = new TestPostClass
            {
                IsProtect = protect,
                IsMark = mark,
                InReplyToStatusId = reply ? (long?)100L : null,
                PostGeo = geo ? new PostClass.StatusGeo(-126.716667, -47.15) : (PostClass.StatusGeo?)null,
            };

            Assert.Equal(expected, post.StateIndex);
        }

        [Fact]
        public void SourceHtml_Test()
        {
            var post = new TestPostClass
            {
                Source = "Twitter Web Client",
                SourceUri = new Uri("http://twitter.com/"),
            };

            Assert.Equal("<a href=\"http://twitter.com/\" rel=\"nofollow\">Twitter Web Client</a>", post.SourceHtml);
        }

        [Fact]
        public void SourceHtml_PlainTextTest()
        {
            var post = new TestPostClass
            {
                Source = "web",
                SourceUri = null,
            };

            Assert.Equal("web", post.SourceHtml);
        }

        [Fact]
        public void SourceHtml_EscapeTest()
        {
            var post = new TestPostClass
            {
                Source = "<script>alert(1)</script>",
                SourceUri = new Uri("http://example.com/?aaa=123&bbb=456"),
            };

            Assert.Equal("<a href=\"http://example.com/?aaa=123&amp;bbb=456\" rel=\"nofollow\">&lt;script&gt;alert(1)&lt;/script&gt;</a>", post.SourceHtml);
        }

        [Fact]
        public void SourceHtml_EscapePlainTextTest()
        {
            var post = new TestPostClass
            {
                Source = "<script>alert(1)</script>",
                SourceUri = null,
            };

            Assert.Equal("&lt;script&gt;alert(1)&lt;/script&gt;", post.SourceHtml);
        }

        [Fact]
        public void DeleteTest()
        {
            var post = new TestPostClass
            {
                InReplyToStatusId = 10L,
                InReplyToUser = "hogehoge",
                InReplyToUserId = 100L,
                IsReply = true,
                ReplyToList = { (100L, "hogehoge") },
            };

            post.IsDeleted = true;

            Assert.Null(post.InReplyToStatusId);
            Assert.Equal("", post.InReplyToUser);
            Assert.Null(post.InReplyToUserId);
            Assert.False(post.IsReply);
            Assert.Empty(post.ReplyToList);
            Assert.Equal(-1, post.StateIndex);
        }

        [Fact]
        public void CanDeleteBy_SentDMTest()
        {
            var post = new TestPostClass
            {
                IsDm = true,
                IsMe = true, // 自分が送信した DM
                UserId = 222L, // 送信先ユーザーID
            };

            Assert.True(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanDeleteBy_ReceivedDMTest()
        {
            var post = new TestPostClass
            {
                IsDm = true,
                IsMe = false, // 自分が受け取った DM
                UserId = 222L, // 送信元ユーザーID
            };

            Assert.True(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanDeleteBy_MyTweetTest()
        {
            var post = new TestPostClass
            {
                UserId = 111L, // 自分のツイート
            };

            Assert.True(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanDeleteBy_OthersTweetTest()
        {
            var post = new TestPostClass
            {
                UserId = 222L, // 他人のツイート
            };

            Assert.False(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanDeleteBy_RetweetedByMeTest()
        {
            var post = new TestPostClass
            {
                RetweetedByUserId = 111L, // 自分がリツイートした
                UserId = 222L, // 他人のツイート
            };

            Assert.True(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanDeleteBy_RetweetedByOthersTest()
        {
            var post = new TestPostClass
            {
                RetweetedByUserId = 333L, // 他人がリツイートした
                UserId = 222L, // 他人のツイート
            };

            Assert.False(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanDeleteBy_MyTweetHaveBeenRetweetedByOthersTest()
        {
            var post = new TestPostClass
            {
                RetweetedByUserId = 222L, // 他人がリツイートした
                UserId = 111L, // 自分のツイート
            };

            Assert.True(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanRetweetBy_DMTest()
        {
            var post = new TestPostClass
            {
                IsDm = true,
                IsMe = false, // 自分が受け取った DM
                UserId = 222L, // 送信元ユーザーID
            };

            Assert.False(post.CanRetweetBy(selfUserId: 111L));
        }

        [Fact]
        public void CanRetweetBy_MyTweetTest()
        {
            var post = new TestPostClass
            {
                UserId = 111L, // 自分のツイート
            };

            Assert.True(post.CanRetweetBy(selfUserId: 111L));
        }

        [Fact]
        public void CanRetweetBy_ProtectedMyTweetTest()
        {
            var post = new TestPostClass
            {
                UserId = 111L, // 自分のツイート
                IsProtect = true,
            };

            Assert.True(post.CanRetweetBy(selfUserId: 111L));
        }

        [Fact]
        public void CanRetweetBy_OthersTweet_NotProtectedTest()
        {
            var post = new TestPostClass
            {
                UserId = 222L, // 他人のツイート
                IsProtect = false,
            };

            Assert.True(post.CanRetweetBy(selfUserId: 111L));
        }

        [Fact]
        public void CanRetweetBy_OthersTweet_ProtectedTest()
        {
            var post = new TestPostClass
            {
                UserId = 222L, // 他人のツイート
                IsProtect = true,
            };

            Assert.False(post.CanRetweetBy(selfUserId: 111L));
        }

        [Fact]
        public void ConvertToOriginalPost_Test()
        {
            var retweetPost = new PostClass
            {
                StatusId = 100L,
                ScreenName = "@aaa",
                UserId = 1L,

                RetweetedId = 50L,
                RetweetedBy = "@bbb",
                RetweetedByUserId = 2L,
                RetweetedCount = 0,
            };

            var originalPost = retweetPost.ConvertToOriginalPost();

            Assert.Equal(50L, originalPost.StatusId);
            Assert.Equal("@aaa", originalPost.ScreenName);
            Assert.Equal(1L, originalPost.UserId);

            Assert.Null(originalPost.RetweetedId);
            Assert.Equal("", originalPost.RetweetedBy);
            Assert.Null(originalPost.RetweetedByUserId);
            Assert.Equal(1, originalPost.RetweetedCount);
        }

        [Fact]
        public void ConvertToOriginalPost_ErrorTest()
        {
            // 公式 RT でないツイート
            var post = new PostClass { StatusId = 100L, RetweetedId = null };

            Assert.Throws<InvalidOperationException>(() => post.ConvertToOriginalPost());
        }

        private class FakeExpandedUrlInfo : PostClass.ExpandedUrlInfo
        {
            public TaskCompletionSource<string> FakeResult = new();

            public FakeExpandedUrlInfo(string url, string expandedUrl, bool deepExpand)
                : base(url, expandedUrl, deepExpand)
            {
            }

            protected override async Task DeepExpandAsync()
                => this.expandedUrl = await this.FakeResult.Task;
        }

        [Fact]
        public async Task ExpandedUrls_BasicScenario()
        {
            PostClass.ExpandedUrlInfo.AutoExpand = true;

            var post = new PostClass
            {
                Text = "<a href=\"http://t.co/aaaaaaa\" title=\"http://t.co/aaaaaaa\">bit.ly/abcde</a>",
                ExpandedUrls = new[]
                {
                    new FakeExpandedUrlInfo(
                        // 展開前の t.co ドメインの URL
                        url: "http://t.co/aaaaaaa",

                        // Entity の expanded_url に含まれる URL
                        expandedUrl: "http://bit.ly/abcde",

                        // expandedUrl をさらに ShortUrl クラスで再帰的に展開する
                        deepExpand: true
                    ),
                },
            };

            var urlInfo = (FakeExpandedUrlInfo)post.ExpandedUrls.Single();

            // ExpandedUrlInfo による展開が完了していない状態
            //   → この段階では Entity に含まれる expanded_url の URL が使用される
            Assert.False(urlInfo.ExpandedCompleted);
            Assert.Equal("http://bit.ly/abcde", urlInfo.ExpandedUrl);
            Assert.Equal("http://bit.ly/abcde", post.GetExpandedUrl("http://t.co/aaaaaaa"));
            Assert.Equal(new[] { "http://bit.ly/abcde" }, post.GetExpandedUrls());
            Assert.Equal("<a href=\"http://t.co/aaaaaaa\" title=\"http://bit.ly/abcde\">bit.ly/abcde</a>", post.Text);

            // bit.ly 展開後の URL は「http://example.com/abcde」
            urlInfo.FakeResult.SetResult("http://example.com/abcde");
            await urlInfo.ExpandTask;

            // ExpandedUrlInfo による展開が完了した後の状態
            //   → 再帰的な展開後の URL が使用される
            Assert.True(urlInfo.ExpandedCompleted);
            Assert.Equal("http://example.com/abcde", urlInfo.ExpandedUrl);
            Assert.Equal("http://example.com/abcde", post.GetExpandedUrl("http://t.co/aaaaaaa"));
            Assert.Equal(new[] { "http://example.com/abcde" }, post.GetExpandedUrls());
            Assert.Equal("<a href=\"http://t.co/aaaaaaa\" title=\"http://example.com/abcde\">bit.ly/abcde</a>", post.Text);
        }
    }
}
