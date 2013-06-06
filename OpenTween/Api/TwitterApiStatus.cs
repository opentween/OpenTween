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

namespace OpenTween.Api
{
    public class TwitterApiStatus
    {
        public ApiLimit AccessLimit
        {
            get { return this._AccessLimit; }
            set
            {
                this._AccessLimit = value;
                this.OnAccessLimitUpdated(EventArgs.Empty);
            }
        }
        public ApiLimit _AccessLimit;

        public ApiLimit MediaUploadLimit { get; set; }
        public TwitterApiAccessLevel AccessLevel { get; set; }

        public event EventHandler AccessLimitUpdated;

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        protected internal TwitterApiStatus()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.AccessLimit = null;
            this.MediaUploadLimit = null;
            this.AccessLevel = TwitterApiAccessLevel.Anonymous;
        }

        public void UpdateFromHeader(IDictionary<string, string> header)
        {
            var rateLimit = TwitterApiStatus.ParseRateLimit(header, "X-RateLimit-");
            if (rateLimit != null)
                this.AccessLimit = rateLimit;

            var mediaLimit = TwitterApiStatus.ParseRateLimit(header, "X-MediaRateLimit-");
            if (mediaLimit != null)
                this.MediaUploadLimit = mediaLimit;

            var accessLevel = TwitterApiStatus.ParseAccessLevel(header, "X-Access-Level");
            if (accessLevel.HasValue)
                this.AccessLevel = accessLevel.Value;
        }

        internal static ApiLimit ParseRateLimit(IDictionary<string, string> header, string prefix)
        {
            var limitCount = (int?)ParseHeaderValue(header, prefix + "Limit");
            var limitRemain = (int?)ParseHeaderValue(header, prefix + "Remaining");
            var limitReset = ParseHeaderValue(header, prefix + "Reset");

            if (limitCount == null || limitRemain == null || limitReset == null)
                return null;

            var limitResetDate = UnixEpoch.AddSeconds(limitReset.Value).ToLocalTime();
            return new ApiLimit(limitCount.Value, limitRemain.Value, limitResetDate);
        }

        internal static TwitterApiAccessLevel? ParseAccessLevel(IDictionary<string, string> header, string headerName)
        {
            if (!header.ContainsKey(headerName))
                return null;

            switch (header[headerName])
            {
                case "read-write-directmessages":
                case "read-write-privatemessages":
                    return TwitterApiAccessLevel.ReadWriteAndDirectMessage;
                case "read-write":
                    return TwitterApiAccessLevel.ReadWrite;
                case "read":
                    return TwitterApiAccessLevel.Read;
                case "":
                    // たまに出てくる空文字列は無視する
                    return null;
                default:
                    MyCommon.TraceOut("Unknown ApiAccessLevel:" + header[headerName]);
                    return TwitterApiAccessLevel.ReadWriteAndDirectMessage;
            }
        }

        internal static long? ParseHeaderValue(IDictionary<string, string> dict, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (!dict.ContainsKey(key)) continue;

                long result;
                if (long.TryParse(dict[key], out result))
                    return result;
            }

            return null;
        }

        public void UpdateFromApi(TwitterDataModel.RateLimitStatus limit)
        {
            if (limit == null)
                throw new ArgumentNullException();

            this.AccessLimit = new ApiLimit(limit.HourlyLimit, limit.RemainingHits, MyCommon.DateTimeParse(limit.ResetTime));

            var mediaLimit = limit.Photos;
            if (mediaLimit != null)
            {
                this.MediaUploadLimit = new ApiLimit(mediaLimit.DailyLimit, mediaLimit.RemainingHits, MyCommon.DateTimeParse(mediaLimit.ResetTime));
            }
        }

        protected virtual void OnAccessLimitUpdated(EventArgs e)
        {
            if (this.AccessLimitUpdated != null)
                this.AccessLimitUpdated(this, e);
        }
    }
}
