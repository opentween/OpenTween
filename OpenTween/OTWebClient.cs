// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;

namespace OpenTween
{
    public class OTWebClient : WebClient
    {
        public string UserAgent { get; set; }
        public int Timeout { get; set; }

        public OTWebClient()
        {
            this.UserAgent = MyCommon.GetUserAgentString();
            this.Timeout = 10000;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address);
            var hreq = req as HttpWebRequest;

            if (hreq != null)
            {
                hreq.UserAgent = this.UserAgent;
                hreq.Timeout = this.Timeout;
            }

            return req;
        }

        public new Task<byte[]> DownloadDataAsync(Uri address)
        {
            return this.DownloadDataAsync(address, CancellationToken.None);
        }

        public Task<byte[]> DownloadDataAsync(Uri address, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<byte[]>();

            token.Register(() =>
            {
                this.CancelAsync();
            });

            DownloadDataCompletedEventHandler onCompleted = null;

            onCompleted = (s, e) =>
            {
                this.DownloadDataCompleted -= onCompleted;

                if (e.Cancelled)
                {
                    tcs.TrySetCanceled();
                    return;
                }
                if (e.Error != null)
                {
                    tcs.TrySetException(e.Error);
                    return;
                }

                tcs.SetResult(e.Result);
            };

            this.DownloadDataCompleted += onCompleted;
            base.DownloadDataAsync(address);

            return tcs.Task;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event DownloadDataCompletedEventHandler DownloadDataCompleted
        {
            add { base.DownloadDataCompleted += value; }
            remove { base.DownloadDataCompleted -= value; }
        }
    }
}
