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

#nullable enable

using System;
using System.Globalization;

namespace OpenTween
{
    /// <summary>
    /// <see cref="DateTimeKind.Utc"/> に固定された <see cref="DateTime"/> を扱うための構造体
    /// </summary>
    public readonly struct DateTimeUtc : IComparable<DateTimeUtc>, IEquatable<DateTimeUtc>
    {
        public static DateTimeUtc MinValue { get; }
            = new DateTimeUtc(DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc));

        public static DateTimeUtc MaxValue { get; }
            = new DateTimeUtc(DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc));

        public static DateTimeUtc UnixEpoch { get; }
            = new DateTimeUtc(1970, 1, 1, 0, 0, 0);

        public static DateTimeUtc Now
            => UseFakeNow ? FakeNow + FakeNowDrift : new DateTimeUtc(DateTime.UtcNow);

        // テストコード用
        internal static bool UseFakeNow = false;
        internal static DateTimeUtc FakeNow = DateTimeUtc.MinValue;
        internal static TimeSpan FakeNowDrift = TimeSpan.Zero;

        private readonly DateTime datetime;

        public DateTimeUtc(int year, int month, int day)
            : this(year, month, day, hour: 0, minute: 0, second: 0)
        {
        }

        public DateTimeUtc(int year, int month, int day, int hour, int minute, int second)
            : this(year, month, day, hour, minute, second, millisecond: 0)
        {
        }

        public DateTimeUtc(int year, int month, int day, int hour, int minute, int second, int millisecond)
            : this(new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc))
        {
        }

        public DateTimeUtc(DateTimeOffset datetimeOffset)
            : this(datetimeOffset.UtcDateTime)
        {
        }

        public DateTimeUtc(long utcTicks)
            : this(new DateTime(utcTicks, DateTimeKind.Utc))
        {
        }

        public DateTimeUtc(DateTime datetime)
        {
            if (datetime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("datetime には UTC に変換された時刻が必須です", nameof(datetime));

            this.datetime = datetime;
        }

        public long UtcTicks
            => this.datetime.Ticks;

        public long ToUnixTime()
            => (long)(this - UnixEpoch).TotalSeconds;

        public DateTimeOffset ToDateTimeOffset()
            => new(this.datetime);

        public DateTimeOffset ToLocalTime()
            => this.ToDateTimeOffset().ToLocalTime();

        public DateTime ToDateTimeUnsafe()
            => this.datetime;

        public int CompareTo(DateTimeUtc other)
            => this.datetime.CompareTo(other.datetime);

        public bool Equals(DateTimeUtc other)
            => this == other;

        public override bool Equals(object obj)
            => obj is DateTimeUtc other && this.Equals(other);

        public override int GetHashCode()
            => this.datetime.GetHashCode();

        public override string ToString()
            => this.ToString("G");

        public string ToString(string format)
            => this.ToDateTimeOffset().ToString(format);

        public string ToLocalTimeString()
            => this.ToLocalTimeString("G");

        public string ToLocalTimeString(string format)
            => this.ToLocalTime().ToString(format);

        public static DateTimeUtc operator +(DateTimeUtc a, TimeSpan b)
            => new(a.datetime + b);

        public static DateTimeUtc operator -(DateTimeUtc a, TimeSpan b)
            => new(a.datetime - b);

        public static TimeSpan operator -(DateTimeUtc a, DateTimeUtc b)
            => a.datetime - b.datetime;

        public static bool operator ==(DateTimeUtc a, DateTimeUtc b)
            => a.datetime == b.datetime;

        public static bool operator !=(DateTimeUtc a, DateTimeUtc b)
            => a.datetime != b.datetime;

        public static bool operator <(DateTimeUtc a, DateTimeUtc b)
            => a.datetime < b.datetime;

        public static bool operator <=(DateTimeUtc a, DateTimeUtc b)
            => a.datetime <= b.datetime;

        public static bool operator >(DateTimeUtc a, DateTimeUtc b)
            => a.datetime > b.datetime;

        public static bool operator >=(DateTimeUtc a, DateTimeUtc b)
            => a.datetime >= b.datetime;

        public static DateTimeUtc FromUnixTime(long unixTime)
            => UnixEpoch + TimeSpan.FromTicks(unixTime * TimeSpan.TicksPerSecond);

        public static DateTimeUtc Parse(string input, IFormatProvider formatProvider)
            => new(DateTimeOffset.Parse(input, formatProvider, DateTimeStyles.AssumeUniversal));

        public static bool TryParse(string input, IFormatProvider formatProvider, out DateTimeUtc result)
        {
            if (DateTimeOffset.TryParse(input, formatProvider, DateTimeStyles.AssumeUniversal, out var datetimeOffset))
            {
                result = new DateTimeUtc(datetimeOffset);
                return true;
            }

            result = MinValue;
            return false;
        }

        public static bool TryParseExact(string input, string[] formats, IFormatProvider formatProvider, out DateTimeUtc result)
        {
            if (DateTimeOffset.TryParseExact(input, formats, formatProvider, DateTimeStyles.AssumeUniversal, out var datetimeOffset))
            {
                result = new DateTimeUtc(datetimeOffset);
                return true;
            }

            result = MinValue;
            return false;
        }
    }
}
