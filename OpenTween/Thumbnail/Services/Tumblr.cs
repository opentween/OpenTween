// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Xml;

namespace OpenTween.Thumbnail.Services
{
    class Tumblr : SimpleThumbnailService
    {
        public Tumblr(string pattern, string replacement = "${0}")
            : base(pattern, replacement)
        {
        }

        public override ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
        {
            var http = new HttpVarious();
            var TargetUrl = url;
            var tmp = http.GetRedirectTo(TargetUrl);
            while (!TargetUrl.Equals(tmp))
            {
                TargetUrl = tmp;
                tmp = http.GetRedirectTo(TargetUrl);
            }

            var apiUrl = base.ReplaceUrl(url);
            if (apiUrl == null) return null;

            var src = "";
            string imgurl = null;
            string errmsg;
            if (http.GetData(apiUrl, null, out src, 0, out errmsg, ""))
            {
                var xdoc = new XmlDocument();
                try
                {
                    xdoc.LoadXml(src);

                    var type = xdoc.SelectSingleNode("/tumblr/posts/post").Attributes["type"].Value;
                    if (type == "photo")
                    {
                        imgurl = xdoc.SelectSingleNode("/tumblr/posts/post/photo-url").InnerText;
                    }
                    else
                    {
                        errmsg = "PostType:" + type;
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }

                return new ThumbnailInfo()
                {
                    ImageUrl = url,
                    ThumbnailUrl = imgurl,
                    TooltipText = null,
                };
            }
            return null;
        }
    }
}
