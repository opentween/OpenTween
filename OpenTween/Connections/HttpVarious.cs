// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
// 
// This file is part of OpenTween.
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details. 
// 
// You should have received a copy of the GNU General public License along
// with this program. if (not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;


namespace OpenTween
{
    public class HttpVarious : HttpConnection
    {
        public string GetRedirectTo(string url)
        {
            try
            {
                HttpWebRequest req = CreateRequest(HeadMethod, new Uri(url), null, false);
                req.Timeout = 5000;
                req.AllowAutoRedirect = false;
                string data;
                Dictionary<string, string> head = new Dictionary<string, string>();
                HttpStatusCode ret = GetResponse(req, out data, head, false);
                if (head.ContainsKey("Location"))
                {
                    return head["Location"];
                }
                else
                {
                    return url;
                }
            }
            catch (Exception)
            {
                return url;
            }
        }

        public Image GetImage(Uri url)
        {
            return GetImage(url.ToString());
        }

        public Image GetImage(string url)
        {
            return GetImage(url, 10000);
        }

        public Image GetImage(string url, int timeout)
        {
            string errmsg;
            return GetImage(url, "", timeout, out errmsg);
        }

        public Image GetImage(string url, string referer)
        {
            string errmsg;
            return GetImage(url, referer, 10000, out errmsg);
        }

        public Image GetImage(string url, string referer, int timeout, out string errmsg)
        {
            return GetImageInternal(CheckValidImage, url, referer, timeout, out errmsg);
        }

        public Image GetIconImage(string url, int timeout)
        {
            string errmsg;
            return GetImageInternal(CheckValidIconImage, url, "", timeout, out errmsg);
        }

        private delegate Image CheckValidImageDelegate(Image img, int width, int height);

        private Image GetImageInternal(CheckValidImageDelegate CheckImage, string url, string referer, int timeout, out string errmsg)
        {
            try
            {
                HttpWebRequest req = CreateRequest(GetMethod, new Uri(url), null, false);
                if (!String.IsNullOrEmpty(referer)) req.Referer = referer;
                if (timeout < 3000 || timeout > 30000)
                {
                    req.Timeout = 10000;
                }
                else
                {
                    req.Timeout = timeout;
                }
                Bitmap img;
                HttpStatusCode ret = GetResponse(req, out img, null, false);
                if (ret == HttpStatusCode.OK)
                {
                    errmsg = "";
                }
                else
                {
                    errmsg = ret.ToString();
                }
                if (img != null) img.Tag = url;
                if (ret == HttpStatusCode.OK) return CheckImage(img, img.Width, img.Height);
                return null;
            }
            catch (WebException ex)
            {
                errmsg = ex.Message;
                return null;
            }
            catch (Exception)
            {
                errmsg = "";
                return null;
            }
        }

        public bool PostData(string Url, Dictionary<string, string> param)
        {
            try
            {
                HttpWebRequest req = CreateRequest(PostMethod, new Uri(Url), param, false);
                HttpStatusCode res = this.GetResponse(req, null, false);
                if (res == HttpStatusCode.OK) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool PostData(string Url, Dictionary<string, string> param, out string content)
        {
            try
            {
                HttpWebRequest req = CreateRequest(PostMethod, new Uri(Url), param, false);
                HttpStatusCode res = this.GetResponse(req, out content, null, false);
                if (res == HttpStatusCode.OK) return true;
                return false;
            }
            catch (Exception)
            {
                content = null;
                return false;
            }
        }

        public bool GetData(string Url, Dictionary<string, string> param, out string content, string userAgent)
        {
            string errmsg;
            return GetData(Url, param, out content, 100000, out errmsg, userAgent);
        }

        public bool GetData(string Url, Dictionary<string, string> param, out string content)
        {
            return GetData(Url, param, out content, 100000);
        }

        public bool GetData(string Url, Dictionary<string, string> param, out string content, int timeout)
        {
            string errmsg;
            return GetData(Url, param, out content, timeout, out errmsg, "");
        }

        public bool GetData(string Url, Dictionary<string, string> param, out string content, int timeout, out string errmsg, string userAgent)
        {
            try
            {
                HttpWebRequest req = CreateRequest(GetMethod, new Uri(Url), param, false);
                if (timeout < 3000 || timeout > 100000)
                {
                    req.Timeout = 10000;
                }
                else
                {
                    req.Timeout = timeout;
                }
                if (!String.IsNullOrEmpty(userAgent)) req.UserAgent = userAgent;
                HttpStatusCode res = this.GetResponse(req, out content, null, false);
                if (res == HttpStatusCode.OK)
                {
                    errmsg = "";
                    return true;
                }
                errmsg = res.ToString();
                return false;
            }
            catch (Exception ex)
            {
                content = null;
                errmsg = ex.Message;
                return false;
            }
        }

        public HttpStatusCode GetContent(string method, Uri Url, Dictionary<string, string> param, out string content, Dictionary<string, string> headerInfo, string userAgent)
        {
            //Searchで使用。呼び出し元で例外キャッチしている。
            HttpWebRequest req = CreateRequest(method, Url, param, false);
            req.UserAgent = userAgent;
            return this.GetResponse(req, out content, headerInfo, false);
        }

        public bool GetDataToFile(string Url, string savePath)
        {
            try
            {
                HttpWebRequest req = CreateRequest(GetMethod, new Uri(Url), null, false);
                req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                req.UserAgent = MyCommon.GetUserAgentString();
                using (FileStream strm = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        HttpStatusCode res = this.GetResponse(req, strm, null, false);
                        if (res == HttpStatusCode.OK) return true;
                        return false;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Image CheckValidIconImage(Image img, int width, int height)
        {
            return CheckValidImage(img, 48, 48);
        }

        public Image CheckValidImage(Image img, int width, int height)
        {
            if (img == null) return null;

            Bitmap bmp = null;

            try
            {
                bmp = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(img, 0, 0, width, height);
                }
                bmp.Tag = img.Tag;

                Bitmap result = bmp;
                bmp = null; //返り値のBitmapはDisposeしない
                return result;
            }
            catch (Exception)
            {
                if (bmp != null)
                {
                    bmp.Dispose();
                    bmp = null;
                }

                bmp = new Bitmap(width, height);
                bmp.Tag = img.Tag;

                Bitmap result = bmp;
                bmp = null; //返り値のBitmapはDisposeしない
                return result;
            }
            finally
            {
                if (bmp != null) bmp.Dispose();
                img.Dispose();
            }
        }
    }
}