// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class TweenMainText
    {
        [Fact]
        public void GetUrlFromDataObject_XMozUrlTest()
        {
            var dataBytes = Encoding.Unicode.GetBytes("https://twitter.com/\nTwitter\0");
            using (var memstream = new MemoryStream(dataBytes))
            {
                var data = new DataObject("text/x-moz-url", memstream);

                var expected = new Tuple<string, string>("https://twitter.com/", "Twitter");
                Assert.Equal(expected, TweenMain.GetUrlFromDataObject(data));
            }
        }

        [Fact]
        public void GetUrlFromDataObject_IESiteModeToUrlTest()
        {
            var dataBytes = Encoding.Unicode.GetBytes("https://twitter.com/\0Twitter\0");
            using (var memstream = new MemoryStream(dataBytes))
            {
                var data = new DataObject("IESiteModeToUrl", memstream);

                var expected = new Tuple<string, string>("https://twitter.com/", "Twitter");
                Assert.Equal(expected, TweenMain.GetUrlFromDataObject(data));
            }
        }

        [Fact]
        public void GetUrlFromDataObject_UniformResourceLocatorWTest()
        {
            var dataBytes = Encoding.Unicode.GetBytes("https://twitter.com/\0");
            using (var memstream = new MemoryStream(dataBytes))
            {
                var data = new DataObject("UniformResourceLocatorW", memstream);

                var expected = new Tuple<string, string>("https://twitter.com/", null);
                Assert.Equal(expected, TweenMain.GetUrlFromDataObject(data));
            }
        }

        [Fact]
        public void GetUrlFromDataObject_UnknownFormatTest()
        {
            using (var memstream = new MemoryStream(new byte[0]))
            {
                var data = new DataObject("application/x-hogehoge", memstream);

                Assert.Throws<NotSupportedException>(() => TweenMain.GetUrlFromDataObject(data));
            }
        }
    }
}
