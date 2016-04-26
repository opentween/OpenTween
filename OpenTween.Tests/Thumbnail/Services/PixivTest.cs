// OpenTween - Client of Twitter
// Copyright (c) 2016 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Threading.Tasks;
using Xunit;

namespace OpenTween.Thumbnail.Services
{
    public class PixivTest
    {
        [Fact]
        public void UrlPattern_Test()
        {
            var regex = new Regex(Pixiv.UrlPattern);

            // サムネイル表示には MetaThumbnailService を使用するため、
            // ここでは単に正規表現が URL にマッチするかのみ確認する（パターン内のグループ等は検証しない）

            // 通常のイラスト URL
            var match = regex.Match("http://www.pixiv.net/member_illust.php?illust_id=1234567&mode=medium");
            Assert.True(match.Success);

            // パラメータ順が違ってもマッチするか
            match = regex.Match("http://www.pixiv.net/member_illust.php?mode=medium&illust_id=1234567");
            Assert.True(match.Success);

            // mode=big の URL でもマッチするか
            match = regex.Match("http://www.pixiv.net/member_illust.php?mode=big&illust_id=1234567");
            Assert.True(match.Success);

            // タッチ端末向け URL
            match = regex.Match("http://touch.pixiv.net/member_illust.php?illust_id=1234567&mode=medium");
            Assert.True(match.Success);
        }
    }
}
