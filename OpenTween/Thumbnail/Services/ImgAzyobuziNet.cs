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
using System.Net;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace OpenTween.Thumbnail.Services
{
    class ImgAzyobuziNet : IThumbnailService, IDisposable
    {
        protected string[] ApiHosts = {
            "https://ss1.coressl.jp/img.azyobuzi.net/api/",
            "http://img.azyobuzi.net/api/",
            "http://img.opentween.org/api/",
        };

        protected string ApiBase;
        protected IEnumerable<Regex> UrlRegex = null;
        protected Timer UpdateTimer;

        private object LockObj = new object();

        public ImgAzyobuziNet(bool autoupdate = false)
        {
            this.UpdateTimer = new Timer(_ => this.LoadRegex());
            this.AutoUpdate = autoupdate;
        }

        public bool AutoUpdate
        {
            get { return this._AutoUpdate; }
            set
            {
                if (value)
                    this.StartAutoUpdate();
                else
                    this.StopAutoUpdate();

                this._AutoUpdate = value;
            }
        }
        private bool _AutoUpdate = false;

        protected void StartAutoUpdate()
        {
            this.UpdateTimer.Change(0, 30 * 60 * 1000); ; // 30分おきに更新
        }

        protected void StopAutoUpdate()
        {
            this.UpdateTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void LoadRegex()
        {
            foreach (var host in this.ApiHosts)
            {
                try
                {
                    var result = this.LoadRegex(host);
                    if (result) return;
                }
                catch (Exception)
                {
#if DEBUG
                    throw;
#endif
                }
            }

            // どのサーバーも使用できない場合
            lock (this.LockObj)
            {
                this.UrlRegex = null;
                this.ApiBase = null;
            }
        }

        public bool LoadRegex(string apiBase)
        {
            try
            {
                using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(this.FetchRegex(apiBase), XmlDictionaryReaderQuotas.Max))
                {
                    var xElm = XElement.Load(jsonReader);

                    if (xElm.Element("error") != null)
                        return false;

                    lock (this.LockObj)
                    {
                        this.UrlRegex = xElm.Elements("item")
                            .Select(e => new Regex(e.Element("regex").Value, RegexOptions.IgnoreCase))
                            .ToArray();

                        this.ApiBase = apiBase;
                    }
                }

                return true;
            }
            catch (WebException) { } // サーバーが2xx以外のステータスコードを返した場合
            catch (XmlException) { } // サーバーが不正なJSONを返した場合

            return false;
        }

        protected virtual byte[] FetchRegex(string apiBase)
        {
            using (var client = new OTWebClient() { Timeout = 1000 })
            {
                return client.DownloadData(apiBase + "regex.json");
            }
        }

        public override ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
        {
            lock (this.LockObj)
            {
                if (this.UrlRegex == null)
                    return null;

                foreach (var regex in this.UrlRegex)
                {
                    if (regex.IsMatch(url))
                    {
                        return new ThumbnailInfo()
                        {
                            ImageUrl = url,
                            ThumbnailUrl = this.ApiBase + "redirect?size=large&uri=" + Uri.EscapeDataString(url),
                            TooltipText = null,
                        };
                    }
                }
            }

            return null;
        }

        public virtual void Dispose()
        {
            this.UpdateTimer.Dispose();
        }
    }
}
