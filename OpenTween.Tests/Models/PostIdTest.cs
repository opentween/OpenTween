// OpenTween - Client of Twitter
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace OpenTween.Models
{
    public class PostIdTest
    {
        private PostId CreatePostId(string type, string id)
        {
            var mock = new Mock<PostId>() { CallBase = true };
            mock.Setup(x => x.IdType).Returns(type);
            mock.Setup(x => x.Id).Returns(id);
            return mock.Object;
        }

        [Fact]
        public void CompareTo_Test()
        {
            var a = this.CreatePostId("mastodon", "200");
            var b = this.CreatePostId("twitter", "100");
            Assert.True(a.CompareTo(b) < 0);
            Assert.True(b.CompareTo(a) > 0);
            Assert.Equal(0, a.CompareTo(a));
        }

        [Fact]
        public void CompareTo_SameIdTypeTest()
        {
            var a = this.CreatePostId("twitter", "100");
            var b = this.CreatePostId("twitter", "200");
            Assert.True(a.CompareTo(b) < 0);
            Assert.True(b.CompareTo(a) > 0);
            Assert.Equal(0, a.CompareTo(a));
        }

        [Fact]
        public void CompareTo_IdLengthTest()
        {
            var a = this.CreatePostId("twitter", "200");
            var b = this.CreatePostId("twitter", "1000");
            Assert.True(a.CompareTo(b) < 0);
            Assert.True(b.CompareTo(a) > 0);
        }

        [Fact]
        public void OperatorGreaterThan_Test()
        {
            var a = this.CreatePostId("twitter", "100");
            var b = this.CreatePostId("twitter", "200");
#pragma warning disable CS1718
            Assert.False(a < a);
            Assert.True(a < b);
            Assert.False(b < a);
            Assert.False(b < b);
            Assert.True(a <= a);
            Assert.True(a <= b);
            Assert.False(b <= a);
            Assert.True(b <= b);
#pragma warning restore CS1718
        }

        [Fact]
        public void OperatorLessThan_Test()
        {
            var a = this.CreatePostId("twitter", "100");
            var b = this.CreatePostId("twitter", "200");
#pragma warning disable CS1718
            Assert.False(a > a);
            Assert.False(a > b);
            Assert.True(b > a);
            Assert.False(b > b);
            Assert.True(a >= a);
            Assert.False(a >= b);
            Assert.True(b >= a);
            Assert.True(b >= b);
#pragma warning restore CS1718
        }

        [Fact]
        public void Equals_Test()
        {
            var a = this.CreatePostId("twitter", "100");
            var b = this.CreatePostId("twitter", "100");
            Assert.True(a.Equals(b));
            Assert.True(b.Equals(a));
            Assert.True(a == b);
            Assert.True(b == a);
        }

        [Fact]
        public void Equals_NotSameIdTypeTest()
        {
            var a = this.CreatePostId("mastodon", "100");
            var b = this.CreatePostId("twitter", "100");
            Assert.False(a.Equals(b));
            Assert.False(b.Equals(a));
            Assert.True(a != b);
            Assert.True(b != a);
        }

        [Fact]
        public void Equals_NotSameIdTest()
        {
            var a = this.CreatePostId("twitter", "100");
            var b = this.CreatePostId("twitter", "200");
            Assert.False(a.Equals(b));
            Assert.False(b.Equals(a));
            Assert.True(a != b);
            Assert.True(b != a);
        }

        [Fact]
        public void GetHashCode_SameIdTest()
        {
            var a = this.CreatePostId("twitter", "100");
            var b = this.CreatePostId("twitter", "100");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_NotSameIdTypeTest()
        {
            var a = this.CreatePostId("mastodon", "100");
            var b = this.CreatePostId("twitter", "100");
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_NotSameIdTest()
        {
            var a = this.CreatePostId("twitter", "100");
            var b = this.CreatePostId("twitter", "200");
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
