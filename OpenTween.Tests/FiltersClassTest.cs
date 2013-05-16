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
using NUnit.Framework;

namespace OpenTween
{
    [TestFixture]
    class FiltersClassTest
    {
        [Test]
        public void NameTest()
        {
            var filter = new FiltersClass();
            PostClass post;

            filter.NameFilter = "hoge";
            post = new PostClass { ScreenName = "hoge", Text = "test" };
            Assert.That(filter.IsHit(post), Is.EqualTo(MyCommon.HITRESULT.CopyAndMark));

            filter.NameFilter = "hoge";
            post = new PostClass { ScreenName = "foo", Text = "test" };
            Assert.That(filter.IsHit(post), Is.EqualTo(MyCommon.HITRESULT.None));

            // NameFilter は RetweetedBy にもマッチする
            filter.NameFilter = "hoge";
            post = new PostClass { ScreenName = "foo", Text = "test", RetweetedBy = "hoge" };
            Assert.That(filter.IsHit(post), Is.EqualTo(MyCommon.HITRESULT.CopyAndMark));

            filter.NameFilter = "hoge";
            post = new PostClass { ScreenName = "foo", Text = "test", RetweetedBy = "bar" };
            Assert.That(filter.IsHit(post), Is.EqualTo(MyCommon.HITRESULT.None));

            // NameFilter は部分一致ではない
            filter.NameFilter = "hoge";
            post = new PostClass { ScreenName = "hogehoge", Text = "test" };
            Assert.That(filter.IsHit(post), Is.EqualTo(MyCommon.HITRESULT.None));

            // 大小文字を区別しないオプション
            filter.NameFilter = "hoge";
            filter.CaseSensitive = false;
            post = new PostClass { ScreenName = "Hoge", Text = "test" };
            Assert.That(filter.IsHit(post), Is.EqualTo(MyCommon.HITRESULT.CopyAndMark));
        }

        [Test]
        public void NameRegexTest()
        {
            var filter = new FiltersClass { UseRegex = true };
            PostClass post;

            filter.NameFilter = "hoge(hoge)+";
            post = new PostClass { ScreenName = "hogehoge", Text = "test" };
            Assert.That(filter.IsHit(post), Is.EqualTo(MyCommon.HITRESULT.CopyAndMark));

            filter.NameFilter = "hoge(hoge)+";
            post = new PostClass { ScreenName = "hoge", Text = "test" };
            Assert.That(filter.IsHit(post), Is.EqualTo(MyCommon.HITRESULT.None));

            // NameFilter は RetweetedBy にもマッチする
            filter.NameFilter = "hoge(hoge)+";
            post = new PostClass { ScreenName = "foo", Text = "test", RetweetedBy = "hogehoge" };
            Assert.That(filter.IsHit(post), Is.EqualTo(MyCommon.HITRESULT.CopyAndMark));

            filter.NameFilter = "hoge(hoge)+";
            post = new PostClass { ScreenName = "foo", Text = "test", RetweetedBy = "hoge2" };
            Assert.That(filter.IsHit(post), Is.EqualTo(MyCommon.HITRESULT.None));

            // 大小文字を区別しないオプション
            filter.NameFilter = "hoge(hoge)+";
            filter.CaseSensitive = false;
            post = new PostClass { ScreenName = "HogeHogeHoge", Text = "test" };
            Assert.That(filter.IsHit(post), Is.EqualTo(MyCommon.HITRESULT.CopyAndMark));
        }
    }
}
