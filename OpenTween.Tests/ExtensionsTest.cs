// OpenTween - Client of Twitter
// Copyright (c) 2017 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween
{
    public class ExtensionsTest
    {
        [Theory]
        [InlineData("ja", "ja-JP", true)]
        [InlineData("ja", "ja", true)]
        [InlineData("ja-JP", "ja-JP", true)]
        [InlineData("ja-JP", "ja", false)]
        // 4 階層以上の親を持つカルチャ
        // 参照: https://msdn.microsoft.com/ja-jp/library/dd997383(v=vs.100).aspx#%E6%96%B0%E3%81%97%E3%81%84%E7%89%B9%E5%AE%9A%E3%82%AB%E3%83%AB%E3%83%81%E3%83%A3
        [InlineData("zh-Hant", "zh-TW", true)]
        [InlineData("zh-Hant", "zh-CHT", true)]
        [InlineData("zh-Hant", "zh-Hant", true)]
        [InlineData("zh-Hant", "zh", false)]
        public void Contains_Test(string thisCultureStr, string thatCultureStr, bool expected)
        {
            var thisCulture = new CultureInfo(thisCultureStr);
            var thatCulture = new CultureInfo(thatCultureStr);
            Assert.Equal(expected, thisCulture.Contains(thatCulture));
        }

        public void Contains_InvariantCultureTest()
        {
            // InvariantCulture は全てのカルチャを内包する
            Assert.True(CultureInfo.InvariantCulture.Contains(new CultureInfo("ja")));
            Assert.True(CultureInfo.InvariantCulture.Contains(CultureInfo.InvariantCulture));
        }
    }
}
