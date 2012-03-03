// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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

namespace OpenTween
{
    public static class nicoms
    {
        private static string[] _nicovideo =
        {
            "www.nicovideo.jp/watch/",
            "live.nicovideo.jp/watch/",
            "live.nicovideo.jp/gate/",
            "live.nicolive.jp/gate/",
            "co.nicovideo.jp/community/",
            "com.nicovideo.jp/community/",
            "ch.nicovideo.jp/channel/",
            "nicovideo.jp/watch/",
            "seiga.nicovideo.jp/bbs/",
            "www.niconicommons.jp/material/",
            "niconicommons.jp/material/",
            "news.nicovideo.jp/watch/",
        };

        public static string Shorten(string url)
        {
            //整形（http(s)://を削除）
            if (url.Length > 7 && url.Length < 128 && url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                url = url.Substring(7);
            }
            else
            {
                return url;
            }

            foreach (var nv in _nicovideo)
            {
                if (url.StartsWith(nv)) return string.Format("{0}{1}", "http://nico.ms/", url.Substring(nv.Length));
            }

            var i = url.IndexOf("nicovideo.jp/user/", StringComparison.OrdinalIgnoreCase);
            if (i == 0 || i == 4) return string.Format("{0}{1}", "http://nico.ms/", url.Substring(13 + i));

            i = url.IndexOf("nicovideo.jp/mylist/", StringComparison.OrdinalIgnoreCase);
            if (i == 0 || i == 4) return string.Format("{0}{1}", "http://nico.ms/", url.Substring(13 + i));

            i = url.IndexOf("seiga.nicovideo.jp/watch/", StringComparison.OrdinalIgnoreCase);
            if (i == 0) return string.Format("{0}{1}", "http://nico.ms/", url.Substring(25));

            return "http://" + url;
        }
    }
}
