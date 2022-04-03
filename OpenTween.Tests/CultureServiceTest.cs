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
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class CultureServiceTest
    {
        [Theory]
        [InlineData("ja-JP", "ja-JP")]
        [InlineData("fr-FR", "en")] // 対応するカルチャが無い場合は en にフォールバックする
        [InlineData("zh-CN", "en")] // zh-CHS リソースは v1.3.7 から削除
        [InlineData("zh-TW", "en")]
        public void GetPreferredCulture_Test(string currentCulture, string expectedCulture)
        {
            var actual = CultureService.GetPreferredCulture(new CultureInfo(currentCulture));
            Assert.Equal(expectedCulture, actual.Name);
        }
    }
}
