// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween.Thumbnail.Services
{
    public class Nicovideo : IThumbnailService
    {
        public static readonly Regex UrlPatternRegex =
            new(@"^https?://(?:(www|ext)\.nicovideo\.jp/watch|nico\.ms)/(?<id>(?:sm|nm)?[0-9]+)(\?.+)?$");

        public override async Task<ThumbnailInfo?> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            var match = Nicovideo.UrlPatternRegex.Match(url);
            if (!match.Success)
                return null;

            try
            {
                var requestUri = new Uri("http://www.nicovideo.jp/api/getthumbinfo/" + match.Groups["id"].Value);
                var responseText = await Networking.Http.GetStringAsync(requestUri);

                var xdoc = XDocument.Parse(responseText);

                var responseElement = xdoc.Element("nicovideo_thumb_response");
                if (responseElement == null || responseElement.Attribute("status").Value != "ok")
                    return null;

                var thumbElement = responseElement.Element("thumb");
                if (thumbElement == null)
                    return null;

                var thumbUrlElement = thumbElement.Element("thumbnail_url");
                if (thumbUrlElement == null)
                    return null;

                return new ThumbnailInfo
                {
                    MediaPageUrl = url,
                    ThumbnailImageUrl = thumbUrlElement.Value,
                    TooltipText = BuildTooltip(thumbElement),
                    IsPlayable = true,
                };
            }
            catch (XmlException)
            {
            }
            catch (HttpRequestException)
            {
            }

            return null;
        }

        internal static string BuildTooltip(XElement thumbElement)
        {
            var tooltip = new StringBuilder(200);

            var titleElement = thumbElement.Element("title");
            if (titleElement != null)
            {
                tooltip.Append(Properties.Resources.NiconicoInfoText1);
                tooltip.Append(titleElement.Value);
                tooltip.AppendLine();
            }

            var lengthElement = thumbElement.Element("length");
            if (lengthElement != null)
            {
                tooltip.Append(Properties.Resources.NiconicoInfoText2);
                tooltip.Append(lengthElement.Value);
                tooltip.AppendLine();
            }

            var firstRetrieveElement = thumbElement.Element("first_retrieve");
            if (firstRetrieveElement != null && DateTimeUtc.TryParse(firstRetrieveElement.Value, DateTimeFormatInfo.InvariantInfo, out var firstRetrieveDate))
            {
                tooltip.Append(Properties.Resources.NiconicoInfoText3);
                tooltip.Append(firstRetrieveDate.ToLocalTimeString());
                tooltip.AppendLine();
            }

            var viewCounterElement = thumbElement.Element("view_counter");
            if (viewCounterElement != null)
            {
                tooltip.Append(Properties.Resources.NiconicoInfoText4);
                tooltip.Append(viewCounterElement.Value);
                tooltip.AppendLine();
            }

            var commentNumElement = thumbElement.Element("comment_num");
            if (commentNumElement != null)
            {
                tooltip.Append(Properties.Resources.NiconicoInfoText5);
                tooltip.Append(commentNumElement.Value);
                tooltip.AppendLine();
            }

            var mylistCounterElement = thumbElement.Element("mylist_counter");
            if (mylistCounterElement != null)
            {
                tooltip.Append(Properties.Resources.NiconicoInfoText6);
                tooltip.Append(mylistCounterElement.Value);
                tooltip.AppendLine();
            }

            return tooltip.ToString();
        }
    }
}
