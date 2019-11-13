// OpenTween - Client of Twitter
// Copyright (c) 2019 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

namespace OpenTween.Models
{
    public class TabCollectionTest
    {
        [Fact]
        public void Getter_Test()
        {
            var tabs = new TabCollection();
            var tab = new PublicSearchTabModel("hoge");
            tabs.Add(tab);

            Assert.Equal(tab, tabs["hoge"]);
        }

        [Fact]
        public void IndexOf_TabName_Test()
        {
            var tabs = new TabCollection();
            var tab1 = new PublicSearchTabModel("aaaaa");
            var tab2 = new PublicSearchTabModel("bbbbb");

            tabs.Add(tab1);
            tabs.Add(tab2);

            Assert.Equal(1, tabs.IndexOf("bbbbb"));
        }

        [Fact]
        public void IndexOf_TabName_NotExistsTest()
        {
            var tabs = new TabCollection();

            Assert.Equal(-1, tabs.IndexOf("hoge"));
        }

        [Fact]
        public void TryGetValue_TabName_Test()
        {
            var tabs = new TabCollection();
            var tab1 = new PublicSearchTabModel("aaaaa");
            var tab2 = new PublicSearchTabModel("bbbbb");

            tabs.Add(tab1);
            tabs.Add(tab2);

            Assert.True(tabs.TryGetValue("bbbbb", out var actualTab));
            Assert.Equal(tab2, actualTab);
        }

        [Fact]
        public void TryGetValue_TabName_NotExistsTest()
        {
            var tabs = new TabCollection();

            Assert.False(tabs.TryGetValue("hoge", out var actualTab));
            Assert.Null(actualTab);
        }
    }
}
