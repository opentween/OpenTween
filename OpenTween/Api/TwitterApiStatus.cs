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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OpenTween.Api
{
    public class TwitterApiStatus
    {
        public TwitterApiAccessLevel AccessLevel { get; set; }
        public EndpointLimits AccessLimit { get; private set; }
        public ApiLimit MediaUploadLimit { get; set; }

        public class AccessLimitUpdatedEventArgs : EventArgs
        {
            public readonly string EndpointName;

            public AccessLimitUpdatedEventArgs(string endpointName)
            {
                this.EndpointName = endpointName;
            }
        }
        public event EventHandler<AccessLimitUpdatedEventArgs> AccessLimitUpdated;

        public TwitterApiStatus()
        {
            this.AccessLimit = new EndpointLimits(this);
        }

        public void Reset()
        {
            this.AccessLevel = TwitterApiAccessLevel.Anonymous;
            this.AccessLimit.Clear();
            this.MediaUploadLimit = null;
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

            // たまに出てくる空文字列は無視する
            if (string.IsNullOrEmpty(header[headerName]))
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

        public void UpdateFromHeader(IDictionary<string, string> header, string endpointName)
        {
            var rateLimit = TwitterApiStatus.ParseRateLimit(header, "X-Rate-Limit-");
            if (rateLimit != null)
                this.AccessLimit[endpointName] = rateLimit;

            var mediaLimit = TwitterApiStatus.ParseRateLimit(header, "X-MediaRateLimit-");
            if (mediaLimit != null)
                this.MediaUploadLimit = mediaLimit;

            var accessLevel = TwitterApiStatus.ParseAccessLevel(header, "X-Access-Level");
            if (accessLevel.HasValue)
                this.AccessLevel = accessLevel.Value;
        }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void UpdateFromJson(string json)
        {
            using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(json), XmlDictionaryReaderQuotas.Max))
            {
                var xElm = XElement.Load(jsonReader);
                XNamespace a = "item";

                var q =
                    from res in xElm.Element("resources").Descendants(a + "item") // a:item 要素を列挙
                    select new {
                        endpointName = res.Attribute("item").Value,
                        limit = new ApiLimit(
                            int.Parse(res.Element("limit").Value),
                            int.Parse(res.Element("remaining").Value),
                            UnixEpoch.AddSeconds(long.Parse(res.Element("reset").Value)).ToLocalTime()
                        ),
                    };
                this.AccessLimit.AddAll(q.ToDictionary(x => x.endpointName, x => x.limit));
            }
        }

        protected virtual void OnAccessLimitUpdated(AccessLimitUpdatedEventArgs e)
        {
            if (this.AccessLimitUpdated != null)
                this.AccessLimitUpdated(this, e);
        }

        public class EndpointLimits : IEnumerable<KeyValuePair<string, ApiLimit>>
        {
            public readonly TwitterApiStatus Owner;

            private ConcurrentDictionary<string, ApiLimit> innerDict = new ConcurrentDictionary<string, ApiLimit>();

            public EndpointLimits(TwitterApiStatus owner)
            {
                this.Owner = owner;
            }

            public ApiLimit this[string endpoint]
            {
                get
                {
                    ApiLimit limit;
                    return this.innerDict.TryGetValue(endpoint, out limit)
                        ? limit
                        : null;
                }
                set
                {
                    this.innerDict[endpoint] = value;
                    this.Owner.OnAccessLimitUpdated(new AccessLimitUpdatedEventArgs(endpoint));
                }
            }

            public void Clear()
            {
                this.innerDict.Clear();
                this.Owner.OnAccessLimitUpdated(new AccessLimitUpdatedEventArgs(null));
            }

            public void AddAll(IDictionary<string, ApiLimit> resources)
            {
                foreach (var res in resources)
                {
                    this.innerDict[res.Key] = res.Value;
                }

                this.Owner.OnAccessLimitUpdated(new AccessLimitUpdatedEventArgs(null));
            }

            public IEnumerator<KeyValuePair<string, ApiLimit>> GetEnumerator()
            {
                return this.innerDict.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
