// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using Xunit;

namespace OpenTween.Thumbnail
{
    public class ThumbnailGeneratorTest
    {
        [Theory]
        [InlineData("https://www.instagram.com/p/aaaaaaaaaaa/", "aaaaaaaaaaa")]
        [InlineData("http://www.instagram.com/p/aaaaaaaaaaa/", "aaaaaaaaaaa")]
        [InlineData("https://i.instagram.com/p/aaaaaaaaaaa/", "aaaaaaaaaaa")]
        [InlineData("https://instagram.com/p/aaaaaaaaaaa/", "aaaaaaaaaaa")]
        [InlineData("https://instagr.am/p/aaaaaaaaaaa/", "aaaaaaaaaaa")]
        [InlineData("https://www.instagram.com/hogehoge/p/aaaaaaaaaaa/", "aaaaaaaaaaa")] // ユーザー名付き
        [InlineData("https://www.instagram.com/p/aaaaaaaaaaa/?utm_medium=copy_link", "aaaaaaaaaaa")] // トラッキングパラメータ付き
        [InlineData("https://www.instagram.com/hogehoge/", null)] // プロフィールページ
        public void InstagramPattern_IsMatchTest(string testUrl, string? expected)
        {
            var match = ThumbnailGenerator.InstagramPattern.Match(testUrl);

            var matchedMediaId = match.Success ? match.Groups["mediaId"].Value : null;
            Assert.Equal(expected, matchedMediaId);
        }
    }
}
