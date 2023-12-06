// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween.Thumbnail.Services
{
    public class YoutubeTest
    {
        [Theory]
        [InlineData("https://www.youtube.com/watch?v=aaaaa", "aaaaa")]
        [InlineData("https://www.youtube.com/watch?v=aaaaa#hoge", "aaaaa")]
        [InlineData("https://www.youtube.com/watch?feature=youtu.be&v=aaaaa&app=desktop", "aaaaa")]
        [InlineData("https://m.youtube.com/watch?v=aaaaa", "aaaaa")]
        [InlineData("https://gaming.youtube.com/watch?v=aaaaa", "aaaaa")]
        [InlineData("https://music.youtube.com/watch?v=aaaaa", "aaaaa")]
        [InlineData("https://www.youtube.com/shorts/aaaaa", "aaaaa")]
        [InlineData("https://www.youtube.com/embed/aaaaa", "aaaaa")]
        [InlineData("https://youtu.be/aaaaa", "aaaaa")]
        [InlineData("https://youtu.be/aaaaa?t=123", "aaaaa")]
        [InlineData("https://www.youtube.com/channel/aaaaa", null)] // チャンネルページ
        public void UrlPatternRegex_Test(string testUrl, string? expected)
        {
            var match = Youtube.UrlPatternRegex.Match(testUrl);

            var matchedVideoId = match.Success ? match.Groups["videoId"].Value : null;
            Assert.Equal(expected, matchedVideoId);
        }
    }
}
