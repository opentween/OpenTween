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

namespace OpenTween.Api
{
    public class ApiLimitTest
    {
        public static readonly TheoryData<object?, bool> EqualsTestCase = new()
        {
            { new ApiLimit(150, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0)), true },
            { new ApiLimit(350, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0)), false },
            { new ApiLimit(150, 150, new DateTimeUtc(2013, 1, 1, 0, 0, 0)), false },
            { new ApiLimit(150, 100, new DateTimeUtc(2012, 12, 31, 0, 0, 0)), false },
            { null, false },
            { new object(), false },
        };

        [Theory]
        [MemberData(nameof(EqualsTestCase))]
        public void EqualsTest(object? obj2, bool expected)
        {
            var obj1 = new ApiLimit(150, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0));

            Assert.Equal(expected, obj1.Equals(obj2));
        }
    }
}
