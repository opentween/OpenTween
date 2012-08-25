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

namespace OpenTween.Thumbnail.Services
{
    class Nicovideo : SimpleThumbnailService
    {
        public Nicovideo(string pattern, string replacement = "${0}")
            : base(pattern, replacement)
        {
        }

        public override ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
        {
            var apiUrl = base.ReplaceUrl(url);
            if (apiUrl == null) return null;

            var http = new HttpVarious();
            var src = "";
            var imgurl = "";
            string errmsg;
            if ((new HttpVarious()).GetData(apiUrl, null, out src, 0, out errmsg, MyCommon.GetUserAgentString()))
            {
                var sb = new StringBuilder();
                var xdoc = new XmlDocument();
                try
                {
                    xdoc.LoadXml(src);
                    var status = xdoc.SelectSingleNode("/nicovideo_thumb_response").Attributes["status"].Value;
                    if (status == "ok")
                    {
                        imgurl = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/thumbnail_url").InnerText;

                        //ツールチップに動画情報をセットする
                        string tmp;

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/title").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText1);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {

                        }

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/length").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText2);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {

                        }

                        try
                        {
                            var tm = new DateTime();
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/first_retrieve").InnerText;
                            if (DateTime.TryParse(tmp, out tm))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText3);
                                sb.Append(tm.ToString());
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {

                        }

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/view_counter").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText4);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {

                        }

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/comment_num").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText5);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {

                        }
                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/mylist_counter").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText6);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else if (status == "fail")
                    {
                        var errcode = xdoc.SelectSingleNode("/nicovideo_thumb_response/error/code").InnerText;
                        errmsg = errcode;
                        imgurl = "";
                    }
                    else
                    {
                        errmsg = "UnknownResponse";
                        imgurl = "";
                    }

                }
                catch (Exception)
                {
                    imgurl = "";
                    errmsg = "Invalid XML";
                }

                if (!string.IsNullOrEmpty(imgurl))
                {
                    return new ThumbnailInfo()
                    {
                        ImageUrl = url,
                        ThumbnailUrl = imgurl,
                        TooltipText = sb.ToString().Trim()
                    };
                }
            }

            return null;
        }
    }
}
