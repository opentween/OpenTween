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
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace OpenTween
{
    [TestFixture]
    class TwitterTest
    {
        [TestCase("https://twitter.com/twitterapi/status/22634515958",
            Result = new[] { "22634515958" })]
        [TestCase("<a target=\"_self\" href=\"https://t.co/aaaaaaaa\" title=\"https://twitter.com/twitterapi/status/22634515958\">twitter.com/twitterapi/stat…</a>",
            Result = new[] { "22634515958" })]
        [TestCase("<a target=\"_self\" href=\"https://t.co/bU3oR95KIy\" title=\"https://twitter.com/haru067/status/224782458816692224\">https://t.co/bU3oR95KIy</a>" +
            "<a target=\"_self\" href=\"https://t.co/bbbbbbbb\" title=\"https://twitter.com/karno/status/311081657790771200\">https://t.co/bbbbbbbb</a>",
            Result = new[] { "224782458816692224", "311081657790771200" })]
        [TestCase("https://mobile.twitter.com/muji_net/status/21984934471",
            Result = new[] { "21984934471" })]
        [TestCase("https://twitter.com/imgazyobuzi/status/293333871171354624/photo/1",
            Result = new[] { "293333871171354624" })]
        public string[] StatusUrlRegexTest(string url)
        {
            return Twitter.StatusUrlRegex.Matches(url).Cast<Match>()
                .Select(x => x.Groups["StatusId"].Value).ToArray();
        }

        [TestCase("http://favstar.fm/users/twitterapi/status/22634515958",
            Result = new[] { "22634515958" })]
        [TestCase("http://ja.favstar.fm/users/twitterapi/status/22634515958",
            Result = new[] { "22634515958" })]
        [TestCase("http://favstar.fm/t/22634515958",
            Result = new[] { "22634515958" })]
        [TestCase("http://aclog.koba789.com/i/312485321239564288",
            Result = new[] { "312485321239564288" })]
        [TestCase("http://frtrt.net/solo_status.php?status=263483634307198977",
            Result = new[] { "263483634307198977" })]
        public string[] ThirdPartyStatusUrlRegexTest(string url)
        {
            return Twitter.ThirdPartyStatusUrlRegex.Matches(url).Cast<Match>()
                .Select(x => x.Groups["StatusId"].Value).ToArray();
        }
    }
}
