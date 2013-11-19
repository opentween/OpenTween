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
using NUnit.Framework;

namespace OpenTween
{
    [TestFixture]
    public class ParseArgumentsTest
    {
        [Test]
        public void NoOptionsTest()
        {
            var args = new string[] { };

            Assert.That(MyApplication.ParseArguments(args), Is.Empty);
        }

        [Test]
        public void SingleOptionTest()
        {
            var args = new[] { "/foo" };

            Assert.That(MyApplication.ParseArguments(args), Is.EquivalentTo(new Dictionary<string, string>
            {
                {"foo", ""},
            }));
        }

        [Test]
        public void MultipleOptionsTest()
        {
            var args = new[] { "/foo", "/bar" };

            Assert.That(MyApplication.ParseArguments(args), Is.EquivalentTo(new Dictionary<string, string>
            {
                {"foo", ""},
                {"bar", ""},
            }));
        }

        [Test]
        public void OptionWithArgumentTest()
        {
            var args = new[] { "/foo:hogehoge" };

            Assert.That(MyApplication.ParseArguments(args), Is.EquivalentTo(new Dictionary<string, string>
            {
                {"foo", "hogehoge"},
            }));
        }

        [Test]
        public void OptionWithEmptyArgumentTest()
        {
            var args = new[] { "/foo:" };

            Assert.That(MyApplication.ParseArguments(args), Is.EquivalentTo(new Dictionary<string, string>
            {
                {"foo", ""},
            }));
        }

        [Test]
        public void IgroreInvalidOptionsTest()
        {
            var args = new string[] { "--foo", "/" };

            Assert.That(MyApplication.ParseArguments(args), Is.Empty);
        }
    }
}
