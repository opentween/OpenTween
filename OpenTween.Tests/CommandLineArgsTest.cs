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

using Xunit;

namespace OpenTween
{
    public class CommandLineArgsTest
    {
        [Fact]
        public void ParseArguments_NoOptionsTest()
        {
            var args = new string[] { };
            var parsedArgs = new CommandLineArgs(args);

            Assert.Empty(parsedArgs);
        }

        [Fact]
        public void ParseArguments_SingleOptionTest()
        {
            var args = new[] { "/foo" };
            var parsedArgs = new CommandLineArgs(args);

            Assert.Single(parsedArgs);
            Assert.Equal("", parsedArgs["foo"]);
        }

        [Fact]
        public void ParseArguments_MultipleOptionsTest()
        {
            var args = new[] { "/foo", "/bar" };
            var parsedArgs = new CommandLineArgs(args);

            Assert.Equal(2, parsedArgs.Count);
            Assert.Equal("", parsedArgs["foo"]);
            Assert.Equal("", parsedArgs["bar"]);
        }

        [Fact]
        public void ParseArguments_OptionWithArgumentTest()
        {
            var args = new[] { "/foo:hogehoge" };
            var parsedArgs = new CommandLineArgs(args);

            Assert.Single(parsedArgs);
            Assert.Equal("hogehoge", parsedArgs["foo"]);
        }

        [Fact]
        public void ParseArguments_OptionWithEmptyArgumentTest()
        {
            var args = new[] { "/foo:" };
            var parsedArgs = new CommandLineArgs(args);

            Assert.Single(parsedArgs);
            Assert.Equal("", parsedArgs["foo"]);
        }

        [Fact]
        public void ParseArguments_IgroreInvalidOptionsTest()
        {
            var args = new string[] { "--foo", "/" };
            var parsedArgs = new CommandLineArgs(args);

            Assert.Empty(parsedArgs);
        }

        [Fact]
        public void ParseArguments_DuplicateOptionsTest()
        {
            var args = new[] { "/foo:abc", "/foo:123" };
            var parsedArgs = new CommandLineArgs(args);

            Assert.Single(parsedArgs);
            Assert.Equal("123", parsedArgs["foo"]);
        }
    }
}
