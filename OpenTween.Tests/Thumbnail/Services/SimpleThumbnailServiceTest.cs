﻿// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace OpenTween.Thumbnail.Services
{
    public class SimpleThumbnailServiceTest
    {
        [Fact]
        public async Task RegexMatchTest()
        {
            var service = new SimpleThumbnailService(@"http://example.com/(.+)", @"http://img.example.com/$1");

            var thumbinfo = await service.GetThumbnailInfoAsync("http://example.com/abcd", null, CancellationToken.None);

            Assert.NotNull(thumbinfo);
            Assert.Equal("http://example.com/abcd", thumbinfo.ImageUrl);
            Assert.Equal("http://img.example.com/abcd", thumbinfo.ThumbnailUrl);
            Assert.Null(thumbinfo.TooltipText);
        }

        [Fact]
        public async Task RegexNotMatchTest()
        {
            var service = new SimpleThumbnailService(@"http://example.com/(.+)", @"http://img.example.com/\1");

            var thumbinfo = await service.GetThumbnailInfoAsync("http://hogehoge.com/abcd", null, CancellationToken.None);

            Assert.Null(thumbinfo);
        }
    }
}
