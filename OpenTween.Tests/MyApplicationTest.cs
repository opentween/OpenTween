// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class ParseArgumentsTest
    {
        [Fact]
        public void NoOptionsTest()
        {
            var args = new string[] { };

            Assert.Empty(MyApplication.ParseArguments(args));
        }

        [Fact]
        public void SingleOptionTest()
        {
            var args = new[] { "/foo" };

            Assert.Equal(new Dictionary<string, string>
            {
                ["foo"] = "",
            },
            MyApplication.ParseArguments(args));
        }

        [Fact]
        public void MultipleOptionsTest()
        {
            var args = new[] { "/foo", "/bar" };

            Assert.Equal(new Dictionary<string, string>
            {
                ["foo"] = "",
                ["bar"] = "",
            },
            MyApplication.ParseArguments(args));
        }

        [Fact]
        public void OptionWithArgumentTest()
        {
            var args = new[] { "/foo:hogehoge" };

            Assert.Equal(new Dictionary<string, string>
            {
                ["foo"] = "hogehoge",
            },
            MyApplication.ParseArguments(args));
        }

        [Fact]
        public void OptionWithEmptyArgumentTest()
        {
            var args = new[] { "/foo:" };

            Assert.Equal(new Dictionary<string, string>
            {
                ["foo"] = "",
            },
            MyApplication.ParseArguments(args));
        }

        [Fact]
        public void IgroreInvalidOptionsTest()
        {
            var args = new string[] { "--foo", "/" };

            Assert.Empty(MyApplication.ParseArguments(args));
        }

        [Fact]
        public void DuplicateOptionsTest()
        {
            var args = new[] { "/foo:abc", "/foo:123" };

            Assert.Equal(new Dictionary<string, string>
            {
                ["foo"] = "123",
            },
            MyApplication.ParseArguments(args));
        }
    }
}
