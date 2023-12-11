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
        [Theory]
        [InlineData("", "")]
        [InlineData("aaa\nbbb", "aaa bbb")]
        public void TextSingleLineTest(string text, string expected)
        {
            var post = new PostClass { TextFromApi = text };

            Assert.Equal(expected, post.TextSingleLine);
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
            var post = new PostClass
            {
                IsProtect = protect,
                IsMark = mark,
                InReplyToStatusId = reply ? new TwitterStatusId("100") : null,
                PostGeo = geo ? new PostClass.StatusGeo(-126.716667, -47.15) : (PostClass.StatusGeo?)null,
            };

            Assert.Equal(expected, post.StateIndex);
        }

        [Fact]
        public void SourceHtml_Test()
        {
            var post = new PostClass
            {
                Source = "Twitter Web Client",
                SourceUri = new Uri("http://twitter.com/"),
            };

            Assert.Equal("""<a href="http://twitter.com/" rel="nofollow">Twitter Web Client</a>""", post.SourceHtml);
        }

        [Fact]
        public void SourceHtml_PlainTextTest()
        {
            var post = new PostClass
            {
                Source = "web",
                SourceUri = null,
            };

            Assert.Equal("web", post.SourceHtml);
        }

        [Fact]
        public void SourceHtml_EscapeTest()
        {
            var post = new PostClass
            {
                Source = "<script>alert(1)</script>",
                SourceUri = new Uri("http://example.com/?aaa=123&bbb=456"),
            };

            Assert.Equal("""<a href="http://example.com/?aaa=123&amp;bbb=456" rel="nofollow">&lt;script&gt;alert(1)&lt;/script&gt;</a>""", post.SourceHtml);
        }

        [Fact]
        public void SourceHtml_EscapePlainTextTest()
        {
            var post = new PostClass
            {
                Source = "<script>alert(1)</script>",
                SourceUri = null,
            };

            Assert.Equal("&lt;script&gt;alert(1)&lt;/script&gt;", post.SourceHtml);
        }

        [Fact]
        public void CanDeleteBy_SentDMTest()
        {
            var post = new PostClass
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
            var post = new PostClass
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
            var post = new PostClass
            {
                UserId = 111L, // 自分のツイート
            };

            Assert.True(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanDeleteBy_OthersTweetTest()
        {
            var post = new PostClass
            {
                UserId = 222L, // 他人のツイート
            };

            Assert.False(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanDeleteBy_RetweetedByMeTest()
        {
            var post = new PostClass
            {
                RetweetedByUserId = 111L, // 自分がリツイートした
                UserId = 222L, // 他人のツイート
            };

            Assert.True(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanDeleteBy_RetweetedByOthersTest()
        {
            var post = new PostClass
            {
                RetweetedByUserId = 333L, // 他人がリツイートした
                UserId = 222L, // 他人のツイート
            };

            Assert.False(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanDeleteBy_MyTweetHaveBeenRetweetedByOthersTest()
        {
            var post = new PostClass
            {
                RetweetedByUserId = 222L, // 他人がリツイートした
                UserId = 111L, // 自分のツイート
            };

            Assert.True(post.CanDeleteBy(selfUserId: 111L));
        }

        [Fact]
        public void CanRetweetBy_DMTest()
        {
            var post = new PostClass
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
            var post = new PostClass
            {
                UserId = 111L, // 自分のツイート
            };

            Assert.True(post.CanRetweetBy(selfUserId: 111L));
        }

        [Fact]
        public void CanRetweetBy_ProtectedMyTweetTest()
        {
            var post = new PostClass
            {
                UserId = 111L, // 自分のツイート
                IsProtect = true,
            };

            Assert.True(post.CanRetweetBy(selfUserId: 111L));
        }

        [Fact]
        public void CanRetweetBy_OthersTweet_NotProtectedTest()
        {
            var post = new PostClass
            {
                UserId = 222L, // 他人のツイート
                IsProtect = false,
            };

            Assert.True(post.CanRetweetBy(selfUserId: 111L));
        }

        [Fact]
        public void CanRetweetBy_OthersTweet_ProtectedTest()
        {
            var post = new PostClass
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
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 2, 0, 0, 0),
                CreatedAt = new(2023, 1, 1, 0, 0, 0),
                ScreenName = "@aaa",
                UserId = 1L,

                RetweetedId = new TwitterStatusId("50"),
                RetweetedBy = "@bbb",
                RetweetedByUserId = 2L,
                RetweetedCount = 0,
            };

            var originalPost = retweetPost.ConvertToOriginalPost();

            Assert.Equal(new TwitterStatusId("50"), originalPost.StatusId);
            Assert.Equal(new(2023, 1, 1, 0, 0, 0), originalPost.CreatedAt);
            Assert.Equal(new(2023, 1, 1, 0, 0, 0), originalPost.CreatedAtForSorting);
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
            var post = new PostClass { StatusId = new TwitterStatusId("100"), RetweetedId = null };

            Assert.Throws<InvalidOperationException>(() => post.ConvertToOriginalPost());
        }
    }
}
