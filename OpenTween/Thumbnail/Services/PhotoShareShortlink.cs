// OpenTween - Client of Twitter
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
using System.Text;
using System.Text.RegularExpressions;

namespace OpenTween.Thumbnail.Services
{
    class PhotoShareShortlink : IThumbnailService
    {
        protected Regex regex;

        public PhotoShareShortlink(string pattern)
        {
            this.regex = new Regex(pattern);
        }

        public override ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
        {
            var match = this.regex.Match(url);

            if (!match.Success) return null;

            return new ThumbnailInfo()
            {
                ImageUrl = url,
                ThumbnailUrl = "http://images.bcphotoshare.com/storages/" + RadixConvert.ToInt32(match.Result("${1}"), 36) + "/thumb180.jpg",
                TooltipText = null,
            };
        }
    }
}
