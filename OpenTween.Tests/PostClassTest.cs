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
using System.Text;
using NUnit.Framework;
using System.Reflection;

namespace OpenTween
{
    [TestFixture]
    class PostClassTest
    {
        class TestPostClass : PostClass
        {
            public TestPostClass(
                string Nickname = null,
                string textFromApi = null,
                string text = null,
                string ImageUrl = null,
                string screenName = null,
                DateTime createdAt = new DateTime(),
                long statusId = 0L,
                bool IsFav = false,
                bool IsRead = false,
                bool IsReply = false,
                bool IsExcludeReply = false,
                bool IsProtect = false,
                bool IsOwl = false,
                bool IsMark = false,
                string InReplyToUser = null,
                long InReplyToStatusId = 0L,
                string Source = null,
                string SourceHtml = null,
                List<string> ReplyToList = null,
                bool IsMe = false,
                bool IsDm = false,
                long userId = 0L,
                bool FilterHit = false,
                string RetweetedBy = null,
                long RetweetedId = 0L,
                StatusGeo Geo = null) :
                base(Nickname, textFromApi, text, ImageUrl, screenName, createdAt, statusId, IsFav, IsRead,
                IsReply, IsExcludeReply, IsProtect, IsOwl, IsMark, InReplyToUser, InReplyToStatusId, Source,
                SourceHtml, ReplyToList, IsMe, IsDm, userId, FilterHit, RetweetedBy, RetweetedId, Geo)
            {
            }

            protected override PostClass GetRetweetSource(long statusId)
            {
                return PostClassTest.TestCases.ContainsKey(statusId) ?
                    PostClassTest.TestCases[statusId] :
                    null;
            }
        }

        private static Dictionary<long, PostClass> TestCases;

        [SetUp]
        public void SetUpTestCases()
        {
            PostClassTest.TestCases = new Dictionary<long, PostClass>
            {
                {1L, new TestPostClass(statusId: 1L)},
                {2L, new TestPostClass(statusId: 2L, IsFav: true)},
                {3L, new TestPostClass(statusId: 3L, IsFav: false, RetweetedId: 2L)},
            };
        }

        [Test]
        public void CloneTest()
        {
            var post = new PostClass();
            var clonePost = post.Clone();

            TestUtils.CheckDeepCloning(post, clonePost);
        }

        [TestCase(null, Result = null)]
        [TestCase("", Result = "")]
        [TestCase("aaa\nbbb", Result = "aaa bbb")]
        public string TextSingleLineTest(string text)
        {
            var post = new TestPostClass(textFromApi: text);

            return post.TextSingleLine;
        }

        [TestCase(1L, Result = false)]
        [TestCase(2L, Result = true)]
        [TestCase(3L, Result = true)]
        public bool GetIsFavTest(long statusId)
        {
            return PostClassTest.TestCases[statusId].IsFav;
        }

        [Test, Combinatorial]
        public void SetIsFavTest(
            [Values(2L, 3L)] long statusId,
            [Values(true, false)] bool isFav)
        {
            var post = PostClassTest.TestCases[statusId];

            post.IsFav = isFav;
            Assert.That(post.IsFav, Is.EqualTo(isFav));

            if (post.RetweetedId != 0)
                Assert.That(PostClassTest.TestCases[post.RetweetedId].IsFav, Is.EqualTo(isFav));
        }

        [Test, Combinatorial]
        public void StateIndexTest(
            [Values(true, false)] bool protect,
            [Values(true, false)] bool mark,
            [Values(true, false)] bool reply,
            [Values(true, false)] bool geo)
        {
            var post = new TestPostClass();
            var except = 0x00;

            post.IsProtect = protect;
            if (protect) except |= 0x01;

            post.IsMark = mark;
            if (mark) except |= 0x02;

            post.InReplyToStatusId = reply ? 100L : 0L;
            if (reply) except |= 0x04;

            post.PostGeo = geo ? new PostClass.StatusGeo { Lat = -47.15, Lng = -126.716667 } : new PostClass.StatusGeo();
            if (geo) except |= 0x08;

            except -= 1;

            Assert.That(post.StateIndex, Is.EqualTo(except));
        }

        [Test]
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

            Assert.That(post.InReplyToStatusId, Is.EqualTo(0L));
            Assert.That(post.InReplyToUser, Is.EqualTo(""));
            Assert.That(post.InReplyToUserId, Is.EqualTo(0L));
            Assert.That(post.IsReply, Is.False);
            Assert.That(post.ReplyToList, Is.Empty);
            Assert.That(post.StateIndex, Is.EqualTo(-1));
        }
    }
}
