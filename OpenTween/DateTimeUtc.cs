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

namespace OpenTween
{
    /// <summary>
    /// <see cref="DateTimeKind.Utc"/> に固定された <see cref="DateTime"/> を扱うための構造体
    /// </summary>
    public struct DateTimeUtc : IComparable<DateTimeUtc>, IEquatable<DateTimeUtc>
    {
        public static DateTimeUtc Now
            => new DateTimeUtc(DateTime.UtcNow);

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

        public DateTimeUtc(DateTime datetime)
        {
            if (datetime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("datetime には UTC に変換された時刻が必須です", nameof(datetime));

            this.datetime = datetime;
        }

        public DateTimeOffset ToDateTimeOffset()
            => new DateTimeOffset(this.datetime);

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

        public static DateTimeUtc operator +(DateTimeUtc a, TimeSpan b)
            => new DateTimeUtc(a.datetime + b);

        public static DateTimeUtc operator -(DateTimeUtc a, TimeSpan b)
            => new DateTimeUtc(a.datetime - b);

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
    }
}
