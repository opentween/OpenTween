// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

namespace OpenTween
{
    public class Outputz
    {
        private static string myOuturl;
        private static string myApikey;

        private static bool state;

        public static string OutUrl
        {
            get { return myOuturl; }
            set { myOuturl = value; }
        }

        public static string Key
        {
            get { return myApikey; }
            set { myApikey = value; }
        }

        public static bool Enabled
        {
            get { return state; }
            set { state = value; }
        }

        public bool Post(int length)
        {
            if (state == false) return true;

            string output = "http://outputz.com/api/post";
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("key", myApikey);
            param.Add("uri", myOuturl);
            param.Add("size", length.ToString());

            return (new HttpVarious()).PostData(output, param);
        }
    }
}
