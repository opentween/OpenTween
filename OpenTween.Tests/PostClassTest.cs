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
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class PostClassTest
    {
        class TestPostClass : PostClass
        {
            protected override PostClass RetweetSource
            {
                get
                {
                    var retweetedId = this.RetweetedId.Value;

                    return PostClassTest.TestCases.ContainsKey(retweetedId) ?
                        PostClassTest.TestCases[retweetedId] :
                        null;
                }
            }
        }

        private static Dictionary<long, PostClass> TestCases;

        public PostClassTest()
        {
            PostClassTest.TestCases = new Dictionary<long, PostClass>
            {
                [1L] = new TestPostClass { StatusId = 1L },
                [2L] = new TestPostClass { StatusId = 2L, IsFav = true },
                [3L] = new TestPostClass { StatusId = 3L, IsFav = false, RetweetedId = 2L },
            };
        }

        [Fact]
        public void CloneTest()
        {
            var post = new PostClass();
            var clonePost = post.Clone();

            TestUtils.CheckDeepCloning(post, clonePost);
        }

        [Theory]
        [InlineData(null,  null)]
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
        {
            Assert.Equal(expected, PostClassTest.TestCases[statusId].IsFav);
        }

        [Theory]
        [InlineData(2L, true)]
        [InlineData(2L, false)]
        [InlineData(3L, true)]
        [InlineData(3L, false)]
        public void SetIsFavTest(long statusId, bool isFav)
        {
            var post = PostClassTest.TestCases[statusId];

            post.IsFav = isFav;
            Assert.Equal(isFav, post.IsFav);

            if (post.RetweetedId != null)
                Assert.Equal(isFav, PostClassTest.TestCases[post.RetweetedId.Value].IsFav);
        }

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
                ReplyToList = new List<string> {"hogehoge"},
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

            Assert.Equal(null, originalPost.RetweetedId);
            Assert.Equal("", originalPost.RetweetedBy);
            Assert.Equal(null, originalPost.RetweetedByUserId);
            Assert.Equal(1, originalPost.RetweetedCount);
        }

        [Fact]
        public void ConvertToOriginalPost_ErrorTest()
        {
            // 公式 RT でないツイート
            var post = new PostClass { StatusId = 100L, RetweetedId = null };

            Assert.Throws<InvalidOperationException>(() => post.ConvertToOriginalPost());
        }
    }
}
