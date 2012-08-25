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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace OpenTween.Thumbnail.Services
{
    class Youtube : SimpleThumbnailService
    {
        public Youtube(string pattern, string replacement = "${0}")
            : base(pattern, replacement)
        {
        }

        public override ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
        {
            var imgUrl = base.ReplaceUrl(url);
            if (imgUrl == null) return null;

            // 参考
            // http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_video_entries.html
            // デベロッパー ガイド: Data API プロトコル - 単独の動画情報の取得 - YouTube の API とツール - Google Code
            // http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_understanding_video_feeds.html#Understanding_Feeds_and_Entries
            // デベロッパー ガイド: Data API プロトコル - 動画のフィードとエントリについて - YouTube の API とツール - Google Code
            var videourl = (new HttpVarious()).GetRedirectTo(url);
            var mc = Regex.Match(videourl, @"^http://(?:(www\.youtube\.com)|(youtu\.be))/(watch\?v=)?(?<videoid>([\w\-]+))", RegexOptions.IgnoreCase);
            if (videourl.StartsWith("http://www.youtube.com/index?ytsession="))
            {
                videourl = url;
                mc = Regex.Match(videourl, @"^http://(?:(www\.youtube\.com)|(youtu\.be))/(watch\?v=)?(?<videoid>([\w\-]+))", RegexOptions.IgnoreCase);
            }
            if (mc.Success)
            {
                var apiurl = "http://gdata.youtube.com/feeds/api/videos/" + mc.Groups["videoid"].Value;
                var src = "";
                if ((new HttpVarious()).GetData(apiurl, null, out src, 5000))
                {
                    var sb = new StringBuilder();
                    var xdoc = new XmlDocument();
                    try
                    {
                        xdoc.LoadXml(src);
                        var nsmgr = new XmlNamespaceManager(xdoc.NameTable);
                        nsmgr.AddNamespace("root", "http://www.w3.org/2005/Atom");
                        nsmgr.AddNamespace("app", "http://purl.org/atom/app#");
                        nsmgr.AddNamespace("media", "http://search.yahoo.com/mrss/");

                        var xentryNode = xdoc.DocumentElement.SelectSingleNode("/root:entry/media:group", nsmgr);
                        var xentry = (XmlElement)xentryNode;
                        var tmp = "";
                        try
                        {
                            tmp = xentry["media:title"].InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.YouTubeInfoText1);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            var sec = 0;
                            if (int.TryParse(xentry["yt:duration"].Attributes["seconds"].Value, out sec))
                            {
                                sb.Append(Properties.Resources.YouTubeInfoText2);
                                sb.AppendFormat("{0:d}:{1:d2}", sec / 60, sec % 60);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            var tmpdate = new DateTime();
                            xentry = (XmlElement)xdoc.DocumentElement.SelectSingleNode("/root:entry", nsmgr);
                            if (DateTime.TryParse(xentry["published"].InnerText, out tmpdate))
                            {
                                sb.Append(Properties.Resources.YouTubeInfoText3);
                                sb.Append(tmpdate);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            var count = 0;
                            xentry = (XmlElement)xdoc.DocumentElement.SelectSingleNode("/root:entry", nsmgr);
                            tmp = xentry["yt:statistics"].Attributes["viewCount"].Value;
                            if (int.TryParse(tmp, out count))
                            {
                                sb.Append(Properties.Resources.YouTubeInfoText4);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            xentry = (XmlElement)xdoc.DocumentElement.SelectSingleNode("/root:entry/app:control", nsmgr);
                            if (xentry != null)
                            {
                                sb.Append(xentry["yt:state"].Attributes["name"].Value);
                                sb.Append(":");
                                sb.Append(xentry["yt:state"].InnerText);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                        }

                        //mc = Regex.Match(videourl, @"^http://www\.youtube\.com/watch\?v=([\w\-]+)", RegexOptions.IgnoreCase)
                        //if (mc.Success)
                        //{
                        // imgurl = mc.Result("http://i.ytimg.com/vi/${1}/default.jpg");
                        //}
                        //mc = Regex.Match(videourl, @"^http://youtu\.be/([\w\-]+)", RegexOptions.IgnoreCase)
                        //if (mc.Success)
                        //{
                        // imgurl = mc.Result("http://i.ytimg.com/vi/${1}/default.jpg");
                        //}

                    }
                    catch (Exception)
                    {

                    }

                    return new ThumbnailInfo()
                    {
                        ImageUrl = url,
                        ThumbnailUrl = imgUrl,
                        TooltipText = sb.ToString().Trim(),
                    };
                }

            }
            return null;
        }
    }
}
