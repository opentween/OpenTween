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
    public class TwitterApiStatus11
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

        public TwitterApiStatus11()
        {
            this.AccessLimit = new EndpointLimits(this);
        }

        public void Reset()
        {
            this.AccessLevel = TwitterApiAccessLevel.Anonymous;
            this.AccessLimit.Clear();
            this.MediaUploadLimit = null;
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

        protected virtual void OnAccessLimitUpdated(AccessLimitUpdatedEventArgs e)
        {
            if (this.AccessLimitUpdated != null)
                this.AccessLimitUpdated(this, e);
        }

        public class EndpointLimits
        {
            public readonly TwitterApiStatus11 Owner;

            private Dictionary<string, ApiLimit> innerDict = new Dictionary<string, ApiLimit>();

            public EndpointLimits(TwitterApiStatus11 owner)
            {
                this.Owner = owner;
            }

            public ApiLimit this[string endpoint]
            {
                get
                {
                    if (this.innerDict.ContainsKey(endpoint))
                        return this.innerDict[endpoint];

                    return null;
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
        }
    }
}
