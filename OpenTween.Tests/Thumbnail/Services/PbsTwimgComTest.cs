// OpenTween - Client of Twitter
// Copyright (c) 2020 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Models;
using Xunit;

namespace OpenTween.Thumbnail.Services
{
    public class PbsTwimgComTest
    {
        [Fact]
        public void ModernUrlPattern_Test()
        {
            var mediaUrl = "https://pbs.twimg.com/media/DYlFv51VwAUdqWr?format=jpg&name=large";
            var matches = PbsTwimgCom.ModernUrlPattern.Match(mediaUrl);

            Assert.True(matches.Success);
            Assert.Equal("https://pbs.twimg.com/media/DYlFv51VwAUdqWr", matches.Groups["base_url"].Value);
            Assert.Equal("jpg", matches.Groups["format"].Value);
        }

        [Fact]
        public void ModernUrlPattern_UnorderedTest()
        {
            var mediaUrl = "https://pbs.twimg.com/media/DYlFv51VwAUdqWr?name=large&format=jpg";
            var matches = PbsTwimgCom.ModernUrlPattern.Match(mediaUrl);

            Assert.True(matches.Success);
            Assert.Equal("https://pbs.twimg.com/media/DYlFv51VwAUdqWr", matches.Groups["base_url"].Value);
            Assert.Equal("jpg", matches.Groups["format"].Value);
        }

        [Fact]
        public void ModernUrlPattern_LegacyUrlTest()
        {
            var mediaUrl = "https://pbs.twimg.com/media/DYlFv51VwAUdqWr.jpg:large";
            var matches = PbsTwimgCom.ModernUrlPattern.Match(mediaUrl);

            Assert.False(matches.Success);
        }

        [Fact]
        public void LegacyUrlPattern_Test()
        {
            var mediaUrl = "https://pbs.twimg.com/media/DYlFv51VwAUdqWr.jpg";
            var matches = PbsTwimgCom.LegacyUrlPattern.Match(mediaUrl);

            Assert.True(matches.Success);
            Assert.Equal("https://pbs.twimg.com/media/DYlFv51VwAUdqWr", matches.Groups["base_url"].Value);
            Assert.Equal("jpg", matches.Groups["format"].Value);
        }

        [Fact]
        public void LegacyUrlPattern_SizeNameTest()
        {
            var mediaUrl = "https://pbs.twimg.com/media/DYlFv51VwAUdqWr.jpg:large";
            var matches = PbsTwimgCom.LegacyUrlPattern.Match(mediaUrl);

            Assert.True(matches.Success);
            Assert.Equal("https://pbs.twimg.com/media/DYlFv51VwAUdqWr", matches.Groups["base_url"].Value);
            Assert.Equal("jpg", matches.Groups["format"].Value);
        }

        [Fact]
        public void LegacyUrlPattern_ModernUrlTest()
        {
            var mediaUrl = "https://pbs.twimg.com/media/DYlFv51VwAUdqWr?format=jpg&name=large";
            var matches = PbsTwimgCom.LegacyUrlPattern.Match(mediaUrl);

            Assert.False(matches.Success);
        }

        [Fact]
        public async Task GetThumbnailInfoAsync_ModernUrlTest()
        {
            var mediaUrl = "https://pbs.twimg.com/media/DYlFv51VwAUdqWr?format=jpg&name=large";

            var service = new PbsTwimgCom();
            var thumb = await service.GetThumbnailInfoAsync(mediaUrl, new PostClass(), CancellationToken.None);

            Assert.NotNull(thumb);
            Assert.Equal("https://pbs.twimg.com/media/DYlFv51VwAUdqWr?format=jpg&name=large", thumb!.ThumbnailImageUrl);
            Assert.Equal("https://pbs.twimg.com/media/DYlFv51VwAUdqWr?format=jpg&name=orig", thumb!.FullSizeImageUrl);
        }

        [Fact]
        public async Task GetThumbnailInfoAsync_LegacyUrlTest()
        {
            var mediaUrl = "https://pbs.twimg.com/media/DYlFv51VwAUdqWr.jpg";

            var service = new PbsTwimgCom();
            var thumb = await service.GetThumbnailInfoAsync(mediaUrl, new PostClass(), CancellationToken.None);

            Assert.NotNull(thumb);
            Assert.Equal("https://pbs.twimg.com/media/DYlFv51VwAUdqWr?format=jpg&name=large", thumb!.ThumbnailImageUrl);
            Assert.Equal("https://pbs.twimg.com/media/DYlFv51VwAUdqWr?format=jpg&name=orig", thumb!.FullSizeImageUrl);
        }
    }
}
