// OpenTween - Client of Twitter
// Copyright (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Net;

namespace OpenTween.Thumbnail.Services
{
    class ViaMe : SimpleThumbnailService
    {
        public ViaMe(string pattern, string replacement = "${0}")
            : base(pattern, replacement)
        {
        }

        public override ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
        {
            var apiUrl = base.ReplaceUrl(url);
            if (apiUrl == null) return null;

            using (var client = new OTWebClient())
            using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(client.DownloadData(apiUrl), XmlDictionaryReaderQuotas.Max))
            {
                var xElm = XElement.Load(jsonReader);

                var thumbUrlElm = xElm.XPathSelectElement("/response/post/thumb_url");
                if (thumbUrlElm == null)
                {
                    return null;
                }

                var textElm = xElm.XPathSelectElement("/response/post/text");

                return new ThumbnailInfo()
                {
                    ImageUrl = url,
                    ThumbnailUrl = thumbUrlElm.Value,
                    TooltipText = textElm == null ? null : textElm.Value,
                };
            }
        }
    }
}
