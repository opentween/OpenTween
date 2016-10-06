// OpenTween - Client of Twitter
// Copyright (c) 2012 the40san <http://sourceforge.jp/users/the40san/>
//           (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    /// <summary>
    /// Bingクラスのテストクラス
    /// </summary>
    public class BingTest
    {
        [Theory]
        [InlineData("af", 0)]
        [InlineData("sq", 1)]
        [InlineData("ja", 67)]
        public void GetLanguageEnumFromIndex_Test(string expected, int index)
        {
            Assert.Equal(expected, Bing.GetLanguageEnumFromIndex(index));
        }

        [Theory]
        [InlineData(0, "af")]
        [InlineData(1, "sq")]
        [InlineData(67, "ja")]
        public void GetIndexFromLanguageEnum_Test(int expected, string lang)
        {
            Assert.Equal(expected, Bing.GetIndexFromLanguageEnum(lang));
        }
    }
}
