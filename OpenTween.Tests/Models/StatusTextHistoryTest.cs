// OpenTween - Client of Twitter
// Copyright (c) 2024 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using Xunit;

namespace OpenTween.Models
{
    public class StatusTextHistoryTest
    {
        [Fact]
        public void Initialize_Test()
        {
            var history = new StatusTextHistory();
            Assert.Single(history.Items);
            Assert.Equal(new("", null), history.Items[0]);
            Assert.Equal(0, history.HistoryIndex);
        }

        [Fact]
        public void SetCurrentItem_Test()
        {
            var history = new StatusTextHistory();
            history.SetCurrentItem("@hoge aaa", (new TwitterStatusId("111"), "hoge"));
            Assert.Single(history.Items);
            Assert.Equal(new("@hoge aaa", (new TwitterStatusId("111"), "hoge")), history.Items[0]);
            Assert.Equal(0, history.HistoryIndex);
        }

        [Fact]
        public void Back_NoItemsTest()
        {
            var history = new StatusTextHistory();
            history.Back();
            Assert.Single(history.Items);
            Assert.Equal(new("", null), history.Items[0]);
            Assert.Equal(0, history.HistoryIndex);
        }

        [Fact]
        public void Back_HasItemsTest()
        {
            var history = new StatusTextHistory();
            history.AddLast("@hoge aaa", (new TwitterStatusId("111"), "hoge"));
            history.Back();

            Assert.Equal(2, history.Items.Count);
            Assert.Equal(new("@hoge aaa", (new TwitterStatusId("111"), "hoge")), history.Items[0]);
            Assert.Equal(new("", null), history.Items[1]);
            Assert.Equal(0, history.HistoryIndex);
        }

        [Fact]
        public void Forward_NoItemsTest()
        {
            var history = new StatusTextHistory();
            history.Forward();
            Assert.Single(history.Items);
            Assert.Equal(new("", null), history.Items[0]);
            Assert.Equal(0, history.HistoryIndex);
        }

        [Fact]
        public void Forward_HasItemsTest()
        {
            var history = new StatusTextHistory();
            history.AddLast("@hoge aaa", (new TwitterStatusId("111"), "hoge"));
            history.Back();
            history.Forward();

            Assert.Equal(2, history.Items.Count);
            Assert.Equal(new("@hoge aaa", (new TwitterStatusId("111"), "hoge")), history.Items[0]);
            Assert.Equal(new("", null), history.Items[1]);
            Assert.Equal(1, history.HistoryIndex);
        }

        [Fact]
        public void AddLast_Test()
        {
            var history = new StatusTextHistory();
            history.AddLast("@hoge aaa", (new TwitterStatusId("111"), "hoge"));
            Assert.Equal(2, history.Items.Count);
            Assert.Equal(new("@hoge aaa", (new TwitterStatusId("111"), "hoge")), history.Items[0]);
            Assert.Equal(new("", null), history.Items[1]);
            Assert.Equal(1, history.HistoryIndex);
        }

        [Fact]
        public void Peek_EmptyTest()
        {
            var history = new StatusTextHistory();
            Assert.Null(history.Peek());
        }

        [Fact]
        public void Peek_HasItemsTest()
        {
            var history = new StatusTextHistory();
            history.AddLast("@hoge aaa", (new TwitterStatusId("111"), "hoge"));

            Assert.Equal(new("@hoge aaa", (new TwitterStatusId("111"), "hoge")), history.Peek());
        }
    }
}
