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

using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;
using System;

namespace OpenTween
{
    public class ShortUrl
    {
        private static string[] _ShortUrlService = {
            "http://t.co/",
            "http://tinyurl.com/",
            "http://is.gd/",
            "http://bit.ly/",
            "http://j.mp/",
            "http://goo.gl/",
            "http://htn.to/",
            "http://amzn.to/",
            "http://flic.kr/",
            "http://ux.nu/",
            "http://youtu.be/",
            "http://p.tl/",
            "http://nico.ms",
            "http://moi.st/",
            "http://snipurl.com/",
            "http://snurl.com/",
            "http://nsfw.in/",
            "http://icanhaz.com/",
            "http://tiny.cc/",
            "http://urlenco.de/",
            "http://linkbee.com/",
            "http://traceurl.com/",
            "http://twurl.nl/",
            "http://cli.gs/",
            "http://rubyurl.com/",
            "http://budurl.com/",
            "http://ff.im/",
            "http://twitthis.com/",
            "http://blip.fm/",
            "http://tumblr.com/",
            "http://www.qurl.com/",
            "http://digg.com/",
            "http://ustre.am/",
            "http://pic.gd/",
            "http://airme.us/",
            "http://qurl.com/",
            "http://bctiny.com/",
            "http://ow.ly/",
            "http://bkite.com/",
            "http://dlvr.it/",
            "http://ht.ly/",
            "http://tl.gd/",
            "http://feeds.feedburner.com/",
            "http://on.fb.me/",
            "http://fb.me/",
            "http://tinami.jp/",
        };

        private static string _bitlyId = "";
        private static string _bitlyKey = "";
        private static bool _isresolve = true;
        private static bool _isForceResolve = true;
        private static Dictionary<string, string> urlCache = new Dictionary<string, string>();

        private static readonly object _lockObj = new object();

        public static string BitlyId
        {
            set { _bitlyId = value; }
        }

        public static string BitlyKey
        {
            set { _bitlyKey = value; }
        }

        public static bool IsResolve
        {
            get { return _isresolve; }
            set { _isresolve = value; }
        }

        public static bool IsForceResolve
        {
            get { return _isForceResolve; }
            set { _isForceResolve = value; }
        }

        public static string Resolve(string orgData, bool tcoResolve)
        {
            if (!_isresolve) return orgData;
            lock (_lockObj)
            {
                if (urlCache.Count > 500)
                {
                    urlCache.Clear(); //定期的にリセット
                }
            }

            List<string> urlList = new List<string>();
            MatchCollection m = Regex.Matches(orgData, "<a href=\"(?<svc>http://.+?/)(?<path>[^\"]+)?\"", RegexOptions.IgnoreCase);
            foreach (Match orgUrlMatch in m)
            {
                string orgUrl = orgUrlMatch.Result("${svc}");
                string orgUrlPath = orgUrlMatch.Result("${path}");
                if ((_isForceResolve || Array.IndexOf(_ShortUrlService, orgUrl) > -1) &&
                   !urlList.Contains(orgUrl + orgUrlPath) && orgUrl != "http://twitter.com/")
                {
                    if (!tcoResolve && (orgUrl == "http://t.co/" || orgUrl == "https://t.co")) continue;
                    lock (_lockObj)
                    {
                        urlList.Add(orgUrl + orgUrlPath);
                    }
                }
            }

            foreach (string orgUrl in urlList)
            {
                if (urlCache.ContainsKey(orgUrl))
                {
                    try
                    {
                        orgData = orgData.Replace("<a href=\"" + orgUrl + "\"", "<a href=\"" + urlCache[orgUrl] + "\"");
                    }
                    catch (Exception)
                    {
                        //Through
                    }
                }
                else
                {
                    try
                    {
                        //urlとして生成できない場合があるらしい
                        //string urlstr = new Uri(urlEncodeMultibyteChar(orgUrl)).GetLeftPart(UriPartial.Path);
                        string retUrlStr = "";
                        string tmpurlStr = new Uri(MyCommon.urlEncodeMultibyteChar(orgUrl)).GetLeftPart(UriPartial.Path);
                        HttpVarious httpVar = new HttpVarious();
                        retUrlStr = MyCommon.urlEncodeMultibyteChar(httpVar.GetRedirectTo(tmpurlStr));
                        if (retUrlStr.StartsWith("http"))
                        {
                            retUrlStr = retUrlStr.Replace("\"", "%22");  //ダブルコーテーションがあるとURL終端と判断されるため、これだけ再エンコード
                            orgData = orgData.Replace("<a href=\"" + tmpurlStr, "<a href=\"" + retUrlStr);
                            lock (_lockObj)
                            {
                                if (!urlCache.ContainsKey(orgUrl)) urlCache.Add(orgUrl, retUrlStr);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //Through
                    }
                }
            }

            return orgData;
        }

        public static string ResolveMedia(string orgData, bool tcoResolve)
        {
            if (!_isresolve) return orgData;
            lock (_lockObj)
            {
                if (urlCache.Count > 500)
                    urlCache.Clear(); //定期的にリセット
            }
            
            Match m = Regex.Match(orgData, "(?<svc>https?://.+?/)(?<path>[^\"]+)?", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                string orgUrl = m.Result("${svc}");
                string orgUrlPath = m.Result("${path}");
                if ((_isForceResolve ||
                    Array.IndexOf(_ShortUrlService, orgUrl) > -1) && orgUrl != "http://twitter.com/")
                {
                    if (!tcoResolve && (orgUrl == "http://t.co/" || orgUrl == "https://t.co/")) return orgData;
                    orgUrl += orgUrlPath;
                    if (urlCache.ContainsKey(orgUrl))
                    {
                        return orgData.Replace(orgUrl, urlCache[orgUrl]);
                    }
                    else
                    {
                        try
                        {
                            //urlとして生成できない場合があるらしい
                            //string urlstr = new Uri(urlEncodeMultibyteChar(orgUrl)).GetLeftPart(UriPartial.Path);
                            string retUrlStr = "";
                            string tmpurlStr = new Uri(MyCommon.urlEncodeMultibyteChar(orgUrl)).GetLeftPart(UriPartial.Path);
                            HttpVarious httpVar = new HttpVarious();
                            retUrlStr = MyCommon.urlEncodeMultibyteChar(httpVar.GetRedirectTo(tmpurlStr));
                            if (retUrlStr.StartsWith("http"))
                            {
                                retUrlStr = retUrlStr.Replace("\"", "%22");  //ダブルコーテーションがあるとURL終端と判断されるため、これだけ再エンコード
                                lock (_lockObj)
                                {
                                    if (!urlCache.ContainsKey(orgUrl)) urlCache.Add(orgUrl, orgData.Replace(tmpurlStr, retUrlStr));
                                }
                                return orgData.Replace(tmpurlStr, retUrlStr);
                            }
                        }
                        catch (Exception)
                        {
                            return orgData;
                        }
                    }
                }
            }

            return orgData;
        }

        public static string Make(MyCommon.UrlConverter ConverterType, string SrcUrl)
        {
            string src = "";
            try
            {
                src = MyCommon.urlEncodeMultibyteChar(SrcUrl);
            }
            catch (Exception)
            {
                return "Can't convert";
            }
            string orgSrc = SrcUrl;
            Dictionary<string, string> param = new Dictionary<string, string>();
            string content = "";

            foreach (string svc in _ShortUrlService)
            {
                if (SrcUrl.StartsWith(svc))
                    return "Can't convert";
            }

            //nico.msは短縮しない
            if (SrcUrl.StartsWith("http://nico.ms/")) return "Can't convert";

            SrcUrl = HttpUtility.UrlEncode(SrcUrl);

            switch (ConverterType)
            {
                case MyCommon.UrlConverter.TinyUrl:       //tinyurl
                    if (SrcUrl.StartsWith("http"))
                    {
                        if ("http://tinyurl.com/xxxxxx".Length > src.Length && !src.Contains("?") && !src.Contains("#"))
                        {
                            // 明らかに長くなると推測できる場合は圧縮しない
                            content = src;
                            break;
                        }
                        if (!(new HttpVarious()).PostData("http://tinyurl.com/api-create.php?url=" + SrcUrl, null, out content))
                        {
                            return "Can't convert";
                        }
                    }
                    if (!content.StartsWith("http://tinyurl.com/"))
                    {
                        return "Can't convert";
                    }
                    break;
                case MyCommon.UrlConverter.Isgd:
                    if (SrcUrl.StartsWith("http"))
                    {
                        if ("http://is.gd/xxxx".Length > src.Length && !src.Contains("?") && !src.Contains("#"))
                        {
                            // 明らかに長くなると推測できる場合は圧縮しない
                            content = src;
                            break;
                        }
                        if (!(new HttpVarious()).PostData("http://is.gd/api.php?longurl=" + SrcUrl, null, out content))
                        {
                            return "Can't convert";
                        }
                    }
                    if (!content.StartsWith("http://is.gd/"))
                    {
                        return "Can't convert";
                    }
                    break;
                case MyCommon.UrlConverter.Twurl:
                    if (SrcUrl.StartsWith("http"))
                    {
                        if ("http://twurl.nl/xxxxxx".Length > src.Length && !src.Contains("?") && !src.Contains("#"))
                        {
                            // 明らかに長くなると推測できる場合は圧縮しない
                            content = src;
                            break;
                        }
                        param.Add("link[url]", orgSrc); //twurlはpostメソッドなので日本語エンコードのみ済ませた状態で送る
                        if (!(new HttpVarious()).PostData("http://tweetburner.com/links", param, out content))
                        {
                            return "Can't convert";
                        }
                    }
                    if (!content.StartsWith("http://twurl.nl/"))
                    {
                        return "Can't convert";
                    }
                    break;
                case MyCommon.UrlConverter.Bitly:
                case MyCommon.UrlConverter.Jmp:
                    const string BitlyApiVersion = "3";
                    if (SrcUrl.StartsWith("http"))
                    {
                        if ("http://bit.ly/xxxx".Length > src.Length && !src.Contains("?") && !src.Contains("#"))
                        {
                            // 明らかに長くなると推測できる場合は圧縮しない
                            content = src;
                            break;
                        }
                        if (string.IsNullOrEmpty(_bitlyId) || string.IsNullOrEmpty(_bitlyKey))
                        {
                            // bit.ly 短縮機能実装のプライバシー問題の暫定対応
                            // ログインIDとAPIキーが指定されていない場合は短縮せずにPOSTする
                            // 参照: http://sourceforge.jp/projects/opentween/lists/archive/dev/2012-January/000020.html
                            content = src;
                            break;
                        }
                        string req = "";
                        req = "http://api.bitly.com/v" + BitlyApiVersion + "/shorten?";
                        req += "login=" + _bitlyId +
                            "&apiKey=" + _bitlyKey +
                            "&format=txt" +
                            "&longUrl=" + SrcUrl;
                        if (ConverterType == MyCommon.UrlConverter.Jmp) req += "&domain=j.mp";
                        if (!(new HttpVarious()).GetData(req, null, out content))
                        {
                            return "Can't convert";
                        }
                    }
                    break;
                case MyCommon.UrlConverter.Uxnu:
                    if (SrcUrl.StartsWith("http"))
                    {
                        if ("http://ux.nx/xxxxxx".Length > src.Length && !src.Contains("?") && !src.Contains("#"))
                        {
                            // 明らかに長くなると推測できる場合は圧縮しない
                            content = src;
                            break;
                        }
                        if (!(new HttpVarious()).PostData("http://ux.nu/api/short?url=" + SrcUrl + "&format=plain", null, out content))
                        {
                            return "Can't convert";
                        }
                    }
                    if (!content.StartsWith("http://ux.nu/"))
                    {
                        return "Can't convert";
                    }
                    break;
            }
            //変換結果から改行を除去
            char[] ch = {'\r', '\n'};
            content = content.TrimEnd(ch);
            if (src.Length < content.Length) content = src; // 圧縮の結果逆に長くなった場合は圧縮前のURLを返す
            return content;
        }
    }
}