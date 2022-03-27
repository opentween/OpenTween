// OpenTween - Client of Twitter
// Copyright (c) 2018 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Globalization;
using Xunit;

namespace OpenTween
{
    public class DateTimeUtcTest
    {
        [Fact]
        public void Constructor_DateTest()
        {
            var utc = new DateTimeUtc(2018, 5, 6);

            Assert.Equal(new DateTime(2018, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc),
                utc.ToDateTimeUnsafe());
        }

        [Fact]
        public void Constructor_DateAndTimeTest()
        {
            var utc = new DateTimeUtc(2018, 5, 6, 11, 22, 33);

            Assert.Equal(new DateTime(2018, 5, 6, 11, 22, 33, 0, DateTimeKind.Utc),
                utc.ToDateTimeUnsafe());
        }

        [Fact]
        public void Constructor_DateAndTimeMillisecondsTest()
        {
            var utc = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 456);

            Assert.Equal(new DateTime(2018, 5, 6, 11, 22, 33, 456, DateTimeKind.Utc),
                utc.ToDateTimeUnsafe());
        }

        [Fact]
        public void Constructor_DateTimeOffsetTest()
        {
            var datetimeOffset = new DateTimeOffset(2018, 5, 6, 11, 22, 33, 456, TimeSpan.FromHours(9));
            var utc = new DateTimeUtc(datetimeOffset);

            Assert.Equal(new DateTime(2018, 5, 6, 2, 22, 33, 456, DateTimeKind.Utc),
                utc.ToDateTimeUnsafe());
        }

        [Fact]
        public void Constructor_DateTimeTest()
        {
            var datetime = new DateTime(2018, 5, 6, 11, 22, 33, DateTimeKind.Utc);
            var utc = new DateTimeUtc(datetime);

            Assert.Equal(datetime, utc.ToDateTimeUnsafe());
        }

        [Fact]
        public void Constructor_LocalDateTimeTest()
        {
            Assert.Throws<ArgumentException>(
                () => new DateTimeUtc(new DateTime(2018, 5, 6, 12, 0, 0, DateTimeKind.Local)));

            Assert.Throws<ArgumentException>(
                () => new DateTimeUtc(new DateTime(2018, 5, 6, 12, 0, 0, DateTimeKind.Unspecified)));
        }

        [Fact]
        public void ToUnixTime_Test()
        {
            var utc = new DateTimeUtc(2009, 2, 13, 23, 31, 30, 0);

            Assert.Equal(1234567890, utc.ToUnixTime());
        }

        [Fact]
        public void ToDateTimeOffset()
        {
            var utc = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);

            Assert.Equal(new DateTimeOffset(2018, 5, 6, 11, 22, 33, 111, TimeSpan.Zero),
                utc.ToDateTimeOffset());
        }

        [Fact]
        public void ToLocalTime()
        {
            var utc = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var expected = new DateTimeOffset(2018, 5, 6, 11, 22, 33, 111, TimeSpan.Zero).ToLocalTime();

            Assert.Equal(expected, utc.ToLocalTime());
        }

        [Fact]
        public void CompareTo_Test()
        {
            var utc1 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var utc2 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var utc3 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 222);

            Assert.Equal(0, utc1.CompareTo(utc2));
            Assert.True(utc1.CompareTo(utc3) < 0);
            Assert.True(utc3.CompareTo(utc1) > 0);
        }

        [Fact]
        public void Equals_Test()
        {
            var utc1 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var utc2 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var utc3 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 222);

            Assert.True(utc1.Equals(utc2));
            Assert.True(utc1.Equals((object)utc2));

            Assert.False(utc1.Equals(utc3));
            Assert.False(utc1.Equals((object)utc3));
        }

        [Fact]
        public void GetHashCode_Test()
        {
            var utc1 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var utc2 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var utc3 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 222);

            Assert.Equal(utc1.GetHashCode(), utc2.GetHashCode());
            Assert.NotEqual(utc1.GetHashCode(), utc3.GetHashCode());
        }

        [Fact]
        public void ToString_Test()
        {
            var datetime = new DateTime(2018, 5, 6, 11, 22, 33, 111, DateTimeKind.Utc);
            var utc = new DateTimeUtc(datetime);

            Assert.Equal(datetime.ToString(), utc.ToString());
        }

        [Fact]
        public void ToString_FormatTest()
        {
            var utc = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);

            Assert.Equal("2018-05-06 11:22:33.111 +00:00", utc.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"));
        }

        [Fact]
        public void ToLocalTimeString_Test()
        {
            var datetime = new DateTime(2018, 5, 6, 11, 22, 33, 111, DateTimeKind.Local);
            var utc = new DateTimeUtc(datetime.ToUniversalTime());

            Assert.Equal(datetime.ToString(), utc.ToLocalTimeString());
        }

        [Fact]
        public void ToLocalTimeString_FormatTest()
        {
            var localDatetime = new DateTime(2018, 5, 6, 11, 22, 33, 111, DateTimeKind.Local);
            var utc = new DateTimeUtc(localDatetime.ToUniversalTime());

            Assert.Equal(localDatetime.ToString("O"), utc.ToLocalTimeString("O"));
        }

        [Fact]
        public void OperatorPlus_Test()
        {
            var utc = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var diff = TimeSpan.FromDays(1);

            Assert.Equal(new DateTime(2018, 5, 7, 11, 22, 33, 111, DateTimeKind.Utc),
                (utc + diff).ToDateTimeUnsafe());
        }

        [Fact]
        public void OperatorMinus_TimeSpanTest()
        {
            var utc = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var diff = TimeSpan.FromDays(1);

            Assert.Equal(new DateTime(2018, 5, 5, 11, 22, 33, 111, DateTimeKind.Utc),
                (utc - diff).ToDateTimeUnsafe());
        }

        [Fact]
        public void OperatorMinus_DateTimeTest()
        {
            var utc1 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var utc2 = new DateTimeUtc(2018, 5, 7, 11, 22, 33, 111);

            Assert.Equal(TimeSpan.Zero, utc1 - utc1);
            Assert.Equal(TimeSpan.FromDays(-1), utc1 - utc2);
            Assert.Equal(TimeSpan.FromDays(1), utc2 - utc1);
        }

        [Fact]
        public void OperatorEqual_Test()
        {
            var utc1 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var utc2 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 222);

#pragma warning disable CS1718
            Assert.True(utc1 == utc1);
            Assert.False(utc1 == utc2);

            Assert.False(utc1 != utc1);
            Assert.True(utc1 != utc2);
#pragma warning restore CS1718
        }

        [Fact]
        public void OperatorCompare_Test()
        {
            var utc1 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111);
            var utc2 = new DateTimeUtc(2018, 5, 6, 11, 22, 33, 222);

#pragma warning disable CS1718
            Assert.False(utc1 < utc1);
            Assert.True(utc1 < utc2);
            Assert.False(utc2 < utc1);

            Assert.True(utc1 <= utc1);
            Assert.True(utc1 <= utc2);
            Assert.False(utc2 <= utc1);

            Assert.False(utc1 > utc1);
            Assert.False(utc1 > utc2);
            Assert.True(utc2 > utc1);

            Assert.True(utc1 >= utc1);
            Assert.False(utc1 >= utc2);
            Assert.True(utc2 >= utc1);
#pragma warning restore CS1718
        }

        [Fact]
        public void MinValue_Test()
            => Assert.Equal(DateTime.MinValue.Ticks, DateTimeUtc.MinValue.ToDateTimeUnsafe().Ticks);

        [Fact]
        public void MaxValue_Test()
            => Assert.Equal(DateTime.MaxValue.Ticks, DateTimeUtc.MaxValue.ToDateTimeUnsafe().Ticks);

        [Fact]
        public void FromUnixTime_Test()
        {
            var utc = DateTimeUtc.FromUnixTime(1234567890);

            Assert.Equal(new DateTime(2009, 2, 13, 23, 31, 30, 0, DateTimeKind.Utc),
                utc.ToDateTimeUnsafe());
        }

        public static readonly TheoryData<string, DateTimeUtc> ParseTestFixtures = new()
        {
            { "2018-05-06T11:22:33.111", new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111) },
            { "2018-05-06T11:22:33.111+00:00", new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111) },
            { "2018-05-06T11:22:33.111+09:00", new DateTimeUtc(2018, 5, 6, 2, 22, 33, 111) },
        };

        [Theory]
        [MemberData(nameof(ParseTestFixtures))]
        public void Parse_Test(string input, DateTimeUtc expected)
            => Assert.Equal(expected, DateTimeUtc.Parse(input, DateTimeFormatInfo.InvariantInfo));

        [Fact]
        public void Parse_ErrorTest()
            => Assert.Throws<FormatException>(() => DateTimeUtc.Parse("### INVALID ###", DateTimeFormatInfo.InvariantInfo));

        public static readonly TheoryData<string, bool, DateTimeUtc> TryParseTestFixtures = new()
        {
            { "2018-05-06T11:22:33.111", true, new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111) },
            { "2018-05-06T11:22:33.111+00:00", true, new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111) },
            { "2018-05-06T11:22:33.111+09:00", true, new DateTimeUtc(2018, 5, 6, 2, 22, 33, 111) },
            { "### INVALID ###", false, DateTimeUtc.MinValue },
        };

        [Theory]
        [MemberData(nameof(TryParseTestFixtures))]
        public void TryParse_Test(string input, bool expectedParsed, DateTimeUtc expectedResult)
        {
            var parsed = DateTimeUtc.TryParse(input, DateTimeFormatInfo.InvariantInfo, out var result);

            Assert.Equal(expectedParsed, parsed);
            Assert.Equal(expectedResult, result);
        }

        public static readonly TheoryData<string, string, bool, DateTimeUtc> TryParseExactTestFixtures = new()
        {
            { "2018-05-06 11:22:33.111", "yyyy-MM-dd HH:mm:ss.fff", true, new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111) },
            { "2018-05-06 11:22:33.111 +00:00", "yyyy-MM-dd HH:mm:ss.fff zzz", true, new DateTimeUtc(2018, 5, 6, 11, 22, 33, 111) },
            { "2018-05-06 11:22:33.111 +09:00", "yyyy-MM-dd HH:mm:ss.fff zzz", true, new DateTimeUtc(2018, 5, 6, 2, 22, 33, 111) },
            { "2018-05-06 11:22:33.111", "yyyy/MM/dd HH:mm:ss", false, DateTimeUtc.MinValue },
            { "### INVALID ###", "yyyy-MM-dd HH:mm:ss.fff", false, DateTimeUtc.MinValue },
        };

        [Theory]
        [MemberData(nameof(TryParseExactTestFixtures))]
        public void TryParseExact_Test(string input, string format, bool expectedParsed, DateTimeUtc expectedResult)
        {
            var parsed = DateTimeUtc.TryParseExact(input, new[] { format }, DateTimeFormatInfo.InvariantInfo, out var result);

            Assert.Equal(expectedParsed, parsed);
            Assert.Equal(expectedResult, result);
        }
    }
}
