// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTween.Api;

namespace OpenTween
{
    public class Twitter : IDisposable
    {
        #region Regexp from twitter-text-js

        // The code in this region code block incorporates works covered by
        // the following copyright and permission notices:
        //
        //   Copyright 2011 Twitter, Inc.
        //
        //   Licensed under the Apache License, Version 2.0 (the "License"); you
        //   may not use this work except in compliance with the License. You
        //   may obtain a copy of the License in the LICENSE file, or at:
        //
        //   http://www.apache.org/licenses/LICENSE-2.0
        //
        //   Unless required by applicable law or agreed to in writing, software
        //   distributed under the License is distributed on an "AS IS" BASIS,
        //   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
        //   implied. See the License for the specific language governing
        //   permissions and limitations under the License.

        //Hashtag用正規表現
        private const string LATIN_ACCENTS = @"\u00c0-\u00d6\u00d8-\u00f6\u00f8-\u00ff\u0100-\u024f\u0253\u0254\u0256\u0257\u0259\u025b\u0263\u0268\u026f\u0272\u0289\u028b\u02bb\u1e00-\u1eff";
        private const string NON_LATIN_HASHTAG_CHARS = @"\u0400-\u04ff\u0500-\u0527\u1100-\u11ff\u3130-\u3185\uA960-\uA97F\uAC00-\uD7AF\uD7B0-\uD7FF";
        //private const string CJ_HASHTAG_CHARACTERS = @"\u30A1-\u30FA\uFF66-\uFF9F\uFF10-\uFF19\uFF21-\uFF3A\uFF41-\uFF5A\u3041-\u3096\u3400-\u4DBF\u4E00-\u9FFF\u20000-\u2A6DF\u2A700-\u2B73F\u2B740-\u2B81F\u2F800-\u2FA1F";
        private const string CJ_HASHTAG_CHARACTERS = @"\u30A1-\u30FA\u30FC\u3005\uFF66-\uFF9F\uFF10-\uFF19\uFF21-\uFF3A\uFF41-\uFF5A\u3041-\u309A\u3400-\u4DBF\p{IsCJKUnifiedIdeographs}";
        private const string HASHTAG_BOUNDARY = @"^|$|\s|「|」|。|\.|!";
        private const string HASHTAG_ALPHA = "[a-z_" + LATIN_ACCENTS + NON_LATIN_HASHTAG_CHARS + CJ_HASHTAG_CHARACTERS + "]";
        private const string HASHTAG_ALPHANUMERIC = "[a-z0-9_" + LATIN_ACCENTS + NON_LATIN_HASHTAG_CHARS + CJ_HASHTAG_CHARACTERS + "]";
        private const string HASHTAG_TERMINATOR = "[^a-z0-9_" + LATIN_ACCENTS + NON_LATIN_HASHTAG_CHARS + CJ_HASHTAG_CHARACTERS + "]";
        public const string HASHTAG = "(" + HASHTAG_BOUNDARY + ")(#|＃)(" + HASHTAG_ALPHANUMERIC + "*" + HASHTAG_ALPHA + HASHTAG_ALPHANUMERIC + "*)(?=" + HASHTAG_TERMINATOR + "|" + HASHTAG_BOUNDARY + ")";
        //URL正規表現
        private const string url_valid_preceding_chars = @"(?:[^A-Za-z0-9@＠$#＃\ufffe\ufeff\uffff\u202a-\u202e]|^)";
        public const string url_invalid_without_protocol_preceding_chars = @"[-_./]$";
        private const string url_invalid_domain_chars = @"\!'#%&'\(\)*\+,\\\-\.\/:;<=>\?@\[\]\^_{|}~\$\u2000-\u200a\u0009-\u000d\u0020\u0085\u00a0\u1680\u180e\u2028\u2029\u202f\u205f\u3000\ufffe\ufeff\uffff\u202a-\u202e";
        private const string url_valid_domain_chars = @"[^" + url_invalid_domain_chars + "]";
        private const string url_valid_subdomain = @"(?:(?:" + url_valid_domain_chars + @"(?:[_-]|" + url_valid_domain_chars + @")*)?" + url_valid_domain_chars + @"\.)";
        private const string url_valid_domain_name = @"(?:(?:" + url_valid_domain_chars + @"(?:-|" + url_valid_domain_chars + @")*)?" + url_valid_domain_chars + @"\.)";
        private const string url_valid_GTLD = @"(?:(?:aero|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|xxx)(?=[^0-9a-zA-Z]|$))";
        private const string url_valid_CCTLD = @"(?:(?:ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|sk|sl|sm|sn|so|sr|ss|st|su|sv|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|za|zm|zw)(?=[^0-9a-zA-Z]|$))";
        private const string url_valid_punycode = @"(?:xn--[0-9a-z]+)";
        private const string url_valid_domain = @"(?<domain>" + url_valid_subdomain + "*" + url_valid_domain_name + "(?:" + url_valid_GTLD + "|" + url_valid_CCTLD + ")|" + url_valid_punycode + ")";
        public const string url_valid_ascii_domain = @"(?:(?:[a-z0-9" + LATIN_ACCENTS + @"]+)\.)+(?:" + url_valid_GTLD + "|" + url_valid_CCTLD + "|" + url_valid_punycode + ")";
        public const string url_invalid_short_domain = "^" + url_valid_domain_name + url_valid_CCTLD + "$";
        private const string url_valid_port_number = @"[0-9]+";

        private const string url_valid_general_path_chars = @"[a-z0-9!*';:=+,.$/%#\[\]\-_~|&" + LATIN_ACCENTS + "]";
        private const string url_balance_parens = @"(?:\(" + url_valid_general_path_chars + @"+\))";
        private const string url_valid_path_ending_chars = @"(?:[+\-a-z0-9=_#/" + LATIN_ACCENTS + "]|" + url_balance_parens + ")";
        private const string pth = "(?:" +
            "(?:" +
                url_valid_general_path_chars + "*" +
                "(?:" + url_balance_parens + url_valid_general_path_chars + "*)*" +
                url_valid_path_ending_chars +
                ")|(?:@" + url_valid_general_path_chars + "+/)" +
            ")";
        private const string qry = @"(?<query>\?[a-z0-9!?*'();:&=+$/%#\[\]\-_.,~|]*[a-z0-9_&=#/])?";
        public const string rgUrl = @"(?<before>" + url_valid_preceding_chars + ")" +
                                    "(?<url>(?<protocol>https?://)?" +
                                    "(?<domain>" + url_valid_domain + ")" +
                                    "(?::" + url_valid_port_number + ")?" +
                                    "(?<path>/" + pth + "*)?" +
                                    qry +
                                    ")";

        #endregion

        /// <summary>
        /// Twitter API のステータスページのURL
        /// </summary>
        public const string ServiceAvailabilityStatusUrl = "https://status.io.watchmouse.com/7617";

        /// <summary>
        /// ツイートへのパーマリンクURLを判定する正規表現
        /// </summary>
        public static readonly Regex StatusUrlRegex = new Regex(@"https?://([^.]+\.)?twitter\.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)/status(es)?/(?<StatusId>[0-9]+)(/photo)?", RegexOptions.IgnoreCase);

        /// <summary>
        /// FavstarやaclogなどTwitter関連サービスのパーマリンクURLからステータスIDを抽出する正規表現
        /// </summary>
        public static readonly Regex ThirdPartyStatusUrlRegex = new Regex(@"https?://(?:[^.]+\.)?(?:
  favstar\.fm/users/[a-zA-Z0-9_]+/status/       # Favstar
| favstar\.fm/t/                                # Favstar (short)
| aclog\.koba789\.com/i/                        # aclog
| frtrt\.net/solo_status\.php\?status=          # RtRT
)(?<StatusId>[0-9]+)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        delegate void GetIconImageDelegate(PostClass post);
        private readonly object LockObj = new object();
        private List<long> followerId = new List<long>();
        private bool _GetFollowerResult = false;
        private List<long> noRTId = new List<long>();
        private bool _GetNoRetweetResult = false;

        private int _followersCount = 0;
        private int _friendsCount = 0;
        private int _statusesCount = 0;
        private string _location = "";
        private string _bio = "";
        private string _protocol = "https://";

        //プロパティからアクセスされる共通情報
        private string _uname;
        private int _iconSz;
        private bool _getIcon;

        private bool _tinyUrlResolve;
        private bool _restrictFavCheck;

        private bool _readOwnPost;
        private List<string> _hashList = new List<string>();

        private Outputz op = new Outputz();
        //max_idで古い発言を取得するために保持（lists分は個別タブで管理）
        private long minHomeTimeline = long.MaxValue;
        private long minMentions = long.MaxValue;
        private long minDirectmessage = long.MaxValue;
        private long minDirectmessageSent = long.MaxValue;

        //private FavoriteQueue favQueue;

        private HttpTwitter twCon = new HttpTwitter();

        //private List<PostClass> _deletemessages = new List<PostClass>();

        public string Authenticate(string username, string password)
        {
            HttpStatusCode res;
            var content = "";

            MyCommon.TwitterApiInfo.Reset();
            try
            {
                res = twCon.AuthUserAndPass(username, password, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
            case HttpStatusCode.OK:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                _uname = username.ToLower();
                if (AppendSettingDialog.Instance.UserstreamStartup) this.ReconnectUserStream();
                return "";
            case HttpStatusCode.Unauthorized:
                {
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return Properties.Resources.Unauthorized + Environment.NewLine + content;
                    }
                    else
                    {
                        return "Auth error:" + errMsg;
                    }
                }
            case HttpStatusCode.Forbidden:
                {
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return "Err:Forbidden";
                    }
                    else
                    {
                        return "Err:" + errMsg;
                    }
                }
            default:
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string StartAuthentication(ref string pinPageUrl)
        {
            //OAuth PIN Flow
            bool res;

            MyCommon.TwitterApiInfo.Reset();
            try
            {
                res = twCon.AuthGetRequestToken(ref pinPageUrl);
            }
            catch(Exception)
            {
                return "Err:" + "Failed to access auth server.";
            }

            return "";
        }

        public string Authenticate(string pinCode)
        {
            HttpStatusCode res;
            var content = "";

            MyCommon.TwitterApiInfo.Reset();
            try
            {
                res = twCon.AuthGetAccessToken(pinCode);
            }
            catch(Exception)
            {
                return "Err:" + "Failed to access auth acc server.";
            }

            switch (res)
            {
            case HttpStatusCode.OK:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                _uname = Username.ToLower();
                if (AppendSettingDialog.Instance.UserstreamStartup) this.ReconnectUserStream();
                return "";
            case HttpStatusCode.Unauthorized:
                {
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return "Check the PIN or retry." + Environment.NewLine + content;
                    }
                    else
                    {
                        return "Auth error:" + errMsg;
                    }
                }
            case HttpStatusCode.Forbidden:
                {
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return "Err:Forbidden";
                    }
                    else
                    {
                        return "Err:" + errMsg;
                    }
                }
            default:
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public void ClearAuthInfo()
        {
            Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
            MyCommon.TwitterApiInfo.Reset();
            twCon.ClearAuthInfo();
        }

        public void VerifyCredentials()
        {
            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.VerifyCredentials(ref content);
            }
            catch(Exception)
            {
                return;
            }

            if (res == HttpStatusCode.OK)
            {
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                TwitterDataModel.User user;
                try
                {
                    user = MyCommon.CreateDataFromJson<TwitterDataModel.User>(content);
                }
                catch(SerializationException)
                {
                    return;
                }
                twCon.AuthenticatedUserId = user.Id;
            }
        }

        private string GetErrorMessageJson(string content)
        {
            try
            {
                if (!string.IsNullOrEmpty(content))
                {
                    using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(content), XmlDictionaryReaderQuotas.Max))
                    {
                        var xElm = XElement.Load(jsonReader);
                        if (xElm.Element("error") != null)
                        {
                            return xElm.Element("error").Value;
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
                else
                {
                    return "";
                }
            }
            catch(Exception)
            {
                return "";
            }
        }

        public void Initialize(string token, string tokenSecret, string username, long userId)
        {
            //OAuth認証
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(tokenSecret) || string.IsNullOrEmpty(username))
            {
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
            }
            MyCommon.TwitterApiInfo.Reset();
            twCon.Initialize(token, tokenSecret, username, userId);
            _uname = username.ToLower();
            if (AppendSettingDialog.Instance.UserstreamStartup) this.ReconnectUserStream();
        }

        public string PreProcessUrl(string orgData)
        {
            int posl1;
            var posl2 = 0;
            //var IDNConveter = new IdnMapping();
            var href = "<a href=\"";

            while (true)
            {
                if (orgData.IndexOf(href, posl2, StringComparison.Ordinal) > -1)
                {
                    var urlStr = "";
                    // IDN展開
                    posl1 = orgData.IndexOf(href, posl2, StringComparison.Ordinal);
                    posl1 += href.Length;
                    posl2 = orgData.IndexOf("\"", posl1, StringComparison.Ordinal);
                    urlStr = orgData.Substring(posl1, posl2 - posl1);

                    if (!urlStr.StartsWith("http://") && !urlStr.StartsWith("https://") && !urlStr.StartsWith("ftp://"))
                    {
                        continue;
                    }

                    var replacedUrl = MyCommon.IDNDecode(urlStr);
                    if (replacedUrl == null) continue;
                    if (replacedUrl == urlStr) continue;

                    orgData = orgData.Replace("<a href=\"" + urlStr, "<a href=\"" + replacedUrl);
                    posl2 = 0;
                }
                else
                {
                    break;
                }
            }
            return orgData;
        }

        private string GetPlainText(string orgData)
        {
            return WebUtility.HtmlDecode(Regex.Replace(orgData, "(?<tagStart><a [^>]+>)(?<text>[^<]+)(?<tagEnd></a>)", "${text}"));
        }

        // htmlの簡易サニタイズ(詳細表示に不要なタグの除去)

        private string SanitizeHtml(string orgdata)
        {
            var retdata = orgdata;

            retdata = Regex.Replace(retdata, "<(script|object|applet|image|frameset|fieldset|legend|style).*" +
                "</(script|object|applet|image|frameset|fieldset|legend|style)>", "", RegexOptions.IgnoreCase);

            retdata = Regex.Replace(retdata, "<(frame|link|iframe|img)>", "", RegexOptions.IgnoreCase);

            return retdata;
        }

        private string AdjustHtml(string orgData)
        {
            var retStr = orgData;
            //var m = Regex.Match(retStr, "<a [^>]+>[#|＃](?<1>[a-zA-Z0-9_]+)</a>");
            //while (m.Success)
            //{
            //    lock (LockObj)
            //    {
            //        _hashList.Add("#" + m.Groups(1).Value);
            //    }
            //    m = m.NextMatch;
            //}
            retStr = Regex.Replace(retStr, "<a [^>]*href=\"/", "<a href=\"https://twitter.com/");
            retStr = retStr.Replace("<a href=", "<a target=\"_self\" href=");
            retStr = Regex.Replace(retStr, @"(\r\n?|\n)", "<br>"); // CRLF, CR, LF は全て <br> に置換する

            //半角スペースを置換(Thanks @anis774)
            var ret = false;
            do
            {
                ret = EscapeSpace(ref retStr);
            } while (!ret);

            return SanitizeHtml(retStr);
        }

        private bool EscapeSpace(ref string html)
        {
            //半角スペースを置換(Thanks @anis774)
            var isTag = false;
            for (int i = 0; i < html.Length; i++)
            {
                if (html[i] == '<')
                {
                    isTag = true;
                }
                if (html[i] == '>')
                {
                    isTag = false;
                }

                if ((!isTag) && (html[i] == ' '))
                {
                    html = html.Remove(i, 1);
                    html = html.Insert(i, "&nbsp;");
                    return false;
                }
            }
            return true;
        }

        private struct PostInfo
        {
            public string CreatedAt;
            public string Id;
            public string Text;
            public string UserId;
            public PostInfo(string Created, string IdStr, string txt, string uid)
            {
                CreatedAt = Created;
                Id = IdStr;
                Text = txt;
                UserId = uid;
            }
            public bool Equals(PostInfo dst)
            {
                if (this.CreatedAt == dst.CreatedAt && this.Id == dst.Id && this.Text == dst.Text && this.UserId == dst.UserId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        static private PostInfo _prev = new PostInfo("", "", "", "");
        private bool IsPostRestricted(TwitterDataModel.Status status)
        {
            var _current = new PostInfo("", "", "", "");

            _current.CreatedAt = status.CreatedAt;
            _current.Id = status.IdStr;
            if (status.Text == null)
            {
                _current.Text = "";
            }
            else
            {
                _current.Text = status.Text;
            }
            _current.UserId = status.User.IdStr;

            if (_current.Equals(_prev))
            {
                return true;
            }
            _prev.CreatedAt = _current.CreatedAt;
            _prev.Id = _current.Id;
            _prev.Text = _current.Text;
            _prev.UserId = _current.UserId;

            return false;
        }

        public string PostStatus(string postStr, long reply_to)
        {

            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            postStr = postStr.Trim();

            if (Regex.Match(postStr, "^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.+)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
            {
                return SendDirectMessage(postStr);
            }

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                res = twCon.UpdateStatus(postStr, reply_to, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
            case HttpStatusCode.OK:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                TwitterDataModel.Status status;
                try
                {
                    status = MyCommon.CreateDataFromJson<TwitterDataModel.Status>(content);
                }
                catch(SerializationException ex)
                {
                    MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                    return "Err:Json Parse Error(DataContractJsonSerializer)";
                }
                catch(Exception ex)
                {
                    MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                    return "Err:Invalid Json!";
                }
                _followersCount = status.User.FollowersCount;
                _friendsCount = status.User.FriendsCount;
                _statusesCount = status.User.StatusesCount;
                _location = status.User.Location;
                _bio = status.User.Description;

                if (IsPostRestricted(status))
                {
                    return "OK:Delaying?";
                }
                if (op.Post(postStr.Length))
                {
                    return "";
                }
                else
                {
                    return "Outputz:Failed";
                }
            case HttpStatusCode.NotFound:
                return "";
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.BadRequest:
                {
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return "Warn:" + res.ToString();
                    }
                    else
                    {
                        return "Warn:" + errMsg;
                    }
                }
            case HttpStatusCode.Conflict:
            case HttpStatusCode.ExpectationFailed:
            case HttpStatusCode.Gone:
            case HttpStatusCode.LengthRequired:
            case HttpStatusCode.MethodNotAllowed:
            case HttpStatusCode.NotAcceptable:
            case HttpStatusCode.PaymentRequired:
            case HttpStatusCode.PreconditionFailed:
            case HttpStatusCode.RequestedRangeNotSatisfiable:
            case HttpStatusCode.RequestEntityTooLarge:
            case HttpStatusCode.RequestTimeout:
            case HttpStatusCode.RequestUriTooLong:
                //仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                return "Warn:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            case HttpStatusCode.Unauthorized:
                {
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return Properties.Resources.Unauthorized;
                    }
                    else
                    {
                        return "Auth err:" + errMsg;
                    }
                }
            default:
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string PostStatusWithMedia(string postStr, long reply_to, FileInfo mediaFile)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            postStr = postStr.Trim();

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                res = twCon.UpdateStatusWithMedia(postStr, reply_to, mediaFile, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
            case HttpStatusCode.OK:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                TwitterDataModel.Status status;
                try
                {
                    status = MyCommon.CreateDataFromJson<TwitterDataModel.Status>(content);
                }
                catch(SerializationException ex)
                {
                    MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                    return "Err:Json Parse Error(DataContractJsonSerializer)";
                }
                catch(Exception ex)
                {
                    MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                    return "Err:Invalid Json!";
                }
                _followersCount = status.User.FollowersCount;
                _friendsCount = status.User.FriendsCount;
                _statusesCount = status.User.StatusesCount;
                _location = status.User.Location;
                _bio = status.User.Description;

                if (IsPostRestricted(status))
                {
                    return "OK:Delaying?";
                }
                if (op.Post(postStr.Length))
                {
                    return "";
                }
                else
                {
                    return "Outputz:Failed";
                }
            case HttpStatusCode.NotFound:
                return "";
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.BadRequest:
                {
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return "Warn:" + res.ToString();
                    }
                    else
                    {
                        return "Warn:" + errMsg;
                    }
                }
            case HttpStatusCode.Conflict:
            case HttpStatusCode.ExpectationFailed:
            case HttpStatusCode.Gone:
            case HttpStatusCode.LengthRequired:
            case HttpStatusCode.MethodNotAllowed:
            case HttpStatusCode.NotAcceptable:
            case HttpStatusCode.PaymentRequired:
            case HttpStatusCode.PreconditionFailed:
            case HttpStatusCode.RequestedRangeNotSatisfiable:
            case HttpStatusCode.RequestEntityTooLarge:
            case HttpStatusCode.RequestTimeout:
            case HttpStatusCode.RequestUriTooLong:
                //仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                return "Warn:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            case HttpStatusCode.Unauthorized:
                {
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return Properties.Resources.Unauthorized;
                    }
                    else
                    {
                        return "Auth err:" + errMsg;
                    }
                }
            default:
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string SendDirectMessage(string postStr)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";
            if (MyCommon.TwitterApiInfo.AccessLevel == TwitterApiAccessLevel.Read || MyCommon.TwitterApiInfo.AccessLevel == TwitterApiAccessLevel.ReadWrite)
            {
                return "Auth Err:try to re-authorization.";
            }

            postStr = postStr.Trim();

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            var mc = Regex.Match(postStr, "^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            try
            {
                res = twCon.SendDirectMessage(mc.Groups["body"].Value, mc.Groups["id"].Value, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
            case HttpStatusCode.OK:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                TwitterDataModel.Directmessage status;
                try
                {
                    status = MyCommon.CreateDataFromJson<TwitterDataModel.Directmessage>(content);
                }
                catch(SerializationException ex)
                {
                    MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                    return "Err:Json Parse Error(DataContractJsonSerializer)";
                }
                catch(Exception ex)
                {
                    MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                    return "Err:Invalid Json!";
                }
                _followersCount = status.Sender.FollowersCount;
                _friendsCount = status.Sender.FriendsCount;
                _statusesCount = status.Sender.StatusesCount;
                _location = status.Sender.Location;
                _bio = status.Sender.Description;

                if (op.Post(postStr.Length))
                {
                    return "";
                }
                else
                {
                    return "Outputz:Failed";
                }
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.BadRequest:
                {
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return "Warn:" + res.ToString();
                    }
                    else
                    {
                        return "Warn:" + errMsg;
                    }
                }
            case HttpStatusCode.Conflict:
            case HttpStatusCode.ExpectationFailed:
            case HttpStatusCode.Gone:
            case HttpStatusCode.LengthRequired:
            case HttpStatusCode.MethodNotAllowed:
            case HttpStatusCode.NotAcceptable:
            case HttpStatusCode.NotFound:
            case HttpStatusCode.PaymentRequired:
            case HttpStatusCode.PreconditionFailed:
            case HttpStatusCode.RequestedRangeNotSatisfiable:
            case HttpStatusCode.RequestEntityTooLarge:
            case HttpStatusCode.RequestTimeout:
            case HttpStatusCode.RequestUriTooLong:
                //仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                return "Warn:" + res.ToString();
            case HttpStatusCode.Unauthorized:
                {
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return Properties.Resources.Unauthorized;
                    }
                    else
                    {
                        return "Auth err:" + errMsg;
                    }
                }
            default:
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string RemoveStatus(long id)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;

            try
            {
                res = twCon.DestroyStatus(id);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
            case HttpStatusCode.OK:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                return "";
            case HttpStatusCode.Unauthorized:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                return Properties.Resources.Unauthorized;
            case HttpStatusCode.NotFound:
                return "";
            default:
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string PostRetweet(long id, bool read)
        {
            if (MyCommon._endingFlag) return "";
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            //データ部分の生成
            var target = id;
            var post = TabInformations.GetInstance()[id];
            if (post == null)
            {
                return "Err:Target isn't found.";
            }
            if (TabInformations.GetInstance()[id].RetweetedId > 0)
            {
                target = TabInformations.GetInstance()[id].RetweetedId; //再RTの場合は元発言をRT
            }

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                res = twCon.RetweetStatus(target, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            if (res == HttpStatusCode.Unauthorized)
            {
                //Blockユーザーの発言をRTすると認証エラー返る
                //Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid
                return Properties.Resources.Unauthorized + " or blocked user.";
            }
            else if (res != HttpStatusCode.OK)
            {
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;

            TwitterDataModel.Status status;
            try
            {
                status = MyCommon.CreateDataFromJson<TwitterDataModel.Status>(content);
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }

            //ReTweetしたものをTLに追加
            post = CreatePostsFromStatusData(status);
            if (post == null) return "Invalid Json!";

            //二重取得回避
            lock (LockObj)
            {
                if (TabInformations.GetInstance().ContainsKey(post.StatusId)) return "";
            }
            //Retweet判定
            if (post.RetweetedId == 0) return "Invalid Json!";
            //ユーザー情報
            post.IsMe = true;

            post.IsRead = read;
            post.IsOwl = false;
            if (_readOwnPost) post.IsRead = true;
            post.IsDm = false;

            TabInformations.GetInstance().AddPost(post);

            return "";
        }

        public string RemoveDirectMessage(long id, PostClass post)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";
            if (MyCommon.TwitterApiInfo.AccessLevel == TwitterApiAccessLevel.Read || MyCommon.TwitterApiInfo.AccessLevel == TwitterApiAccessLevel.ReadWrite)
            {
                return "Auth Err:try to re-authorization.";
            }

            HttpStatusCode res = HttpStatusCode.BadRequest;

            //if (post.IsMe)
            //    _deletemessages.Add(post)
            //}
            try
            {
                res = twCon.DestroyDirectMessage(id);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
            case HttpStatusCode.OK:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                return "";
            case HttpStatusCode.Unauthorized:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                return Properties.Resources.Unauthorized;
            case HttpStatusCode.NotFound:
                return "";
            default:
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string PostFollowCommand(string screenName)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.CreateFriendships(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
            case HttpStatusCode.OK:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                return "";
            case HttpStatusCode.Unauthorized:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                return Properties.Resources.Unauthorized;
            case HttpStatusCode.Forbidden:
                var errMsg = GetErrorMessageJson(content);
                if (string.IsNullOrEmpty(errMsg))
                {
                    return "Err:Forbidden(" + MethodBase.GetCurrentMethod().Name + ")";
                }
                else
                {
                    return "Err:" + errMsg;
                }
            default:
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string PostRemoveCommand(string screenName)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.DestroyFriendships(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
            case HttpStatusCode.OK:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                return "";
            case HttpStatusCode.Unauthorized:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                return Properties.Resources.Unauthorized;
            case HttpStatusCode.Forbidden:
                var errMsg = GetErrorMessageJson(content);
                if (string.IsNullOrEmpty(errMsg))
                {
                    return "Err:Forbidden(" + MethodBase.GetCurrentMethod().Name + ")";
                }
                else
                {
                    return "Err:" + errMsg;
                }
            default:
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string PostCreateBlock(string screenName)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.CreateBlock(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return "Err:Forbidden(" + MethodBase.GetCurrentMethod().Name + ")";
                    }
                    else
                    {
                        return "Err:" + errMsg;
                    }
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string PostDestroyBlock(string screenName)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.DestroyBlock(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
            case HttpStatusCode.OK:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                return "";
            case HttpStatusCode.Unauthorized:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                return Properties.Resources.Unauthorized;
            case HttpStatusCode.Forbidden:
                var errMsg = GetErrorMessageJson(content);
                if (string.IsNullOrEmpty(errMsg))
                {
                    return "Err:Forbidden(" + MethodBase.GetCurrentMethod().Name + ")";
                }
                else
                {
                    return "Err:" + errMsg;
                }
            default:
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string PostReportSpam(string screenName)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.ReportSpam(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
            case HttpStatusCode.OK:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                return "";
            case HttpStatusCode.Unauthorized:
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                return Properties.Resources.Unauthorized;
            case HttpStatusCode.Forbidden:
                var errMsg = GetErrorMessageJson(content);
                if (string.IsNullOrEmpty(errMsg))
                {
                    return "Err:Forbidden(" + MethodBase.GetCurrentMethod().Name + ")";
                }
                else
                {
                    return "Err:" + errMsg;
                }
            default:
                return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string GetFriendshipInfo(string screenName, ref bool isFollowing, ref bool isFollowed)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                res = twCon.ShowFriendships(_uname, screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    try
                    {
                        var relation = MyCommon.CreateDataFromJson<TwitterDataModel.Relationship>(content);
                        isFollowing = relation.relationship.Source.Following;
                        isFollowed = relation.relationship.Source.FollowedBy;
                        return "";
                    }
                    catch(SerializationException ex)
                    {
                        MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                        return "Err:Json Parse Error(DataContractJsonSerializer)";
                    }
                    catch(Exception ex)
                    {
                        MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                        return "Err:Invalid Json!";
                    }
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string GetUserInfo(string screenName, ref TwitterDataModel.User user)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            user = null;
            try
            {
                res = twCon.ShowUserInfo(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    try
                    {
                        user = MyCommon.CreateDataFromJson<TwitterDataModel.User>(content);
                    }
                    catch (SerializationException ex)
                    {
                        MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                        return "Err:Json Parse Error(DataContractJsonSerializer)";
                    }
                    catch (Exception ex)
                    {
                        MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                        return "Err:Invalid Json!";
                    }
                    return "";
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return Properties.Resources.Unauthorized;
                    }
                    else
                    {
                        return "Auth err:" + errMsg;
                    }
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string GetStatus_Retweeted_Count(long StatusId, ref int retweeted_count)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (MyCommon._endingFlag) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.ShowStatuses(StatusId, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.Forbidden:
                    return "Err:protected user's tweet";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            TwitterDataModel.Status status;
            try
            {
                status = MyCommon.CreateDataFromJson<TwitterDataModel.Status>(content);
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }
            int tmp;
            if (int.TryParse(status.RetweetCount, out tmp))
                retweeted_count = tmp;
            return "";
        }

        public string PostFavAdd(long id)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            //if (this.favQueue == null) this.favQueue = new FavoriteQueue(this)

            //if (this.favQueue.Contains(id)) this.favQueue.Remove(id)

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                res = twCon.CreateFavorites(id, ref content);
            }
            catch(Exception ex)
            {
                //this.favQueue.Add(id)
                //return "Err:->FavoriteQueue:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    //this.favQueue.FavoriteCacheStart();
                    if (!_restrictFavCheck) return "";
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return "Err:Forbidden(" + MethodBase.GetCurrentMethod().Name + ")";
                    }
                    else
                    {
                        //if (errMsg.Contains("It's great that you like so many updates"))
                        //    //this.favQueue.Add(id)
                        //    return "Err:->FavoriteQueue:" + errMsg;
                        //}
                        return "Err:" + errMsg;
                    }
                //Case HttpStatusCode.BadGateway, HttpStatusCode.ServiceUnavailable, HttpStatusCode.InternalServerError, HttpStatusCode.RequestTimeout
                //    //this.favQueue.Add(id)
                //    return "Err:->FavoriteQueue:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            //http://twitter.com/statuses/show/id.xml APIを発行して本文を取得

            //var content = "";
            content = "";
            try
            {
                res = twCon.ShowStatuses(id, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    TwitterDataModel.Status status;
                    try
                    {
                        status = MyCommon.CreateDataFromJson<TwitterDataModel.Status>(content);
                    }
                    catch (SerializationException ex)
                    {
                        MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                        return "Err:Json Parse Error(DataContractJsonSerializer)";
                    }
                    catch (Exception ex)
                    {
                        MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                        return "Err:Invalid Json!";
                    }
                    if (status.Favorited)
                    {
                        return "";
                    }
                    else
                    {
                        return "NG(Restricted?)";
                    }
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string PostFavRemove(long id)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            //if (this.favQueue == null) this.favQueue = new FavoriteQueue(this)

            //if (this.favQueue.Contains(id))
            //    this.favQueue.Remove(id)
            //    return "";
            //}

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                res = twCon.DestroyFavorites(id, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return "Err:Forbidden(" + MethodBase.GetCurrentMethod().Name + ")";
                    }
                    else
                    {
                        return "Err:" + errMsg;
                    }
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string PostUpdateProfile(string name, string url, string location, string description)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                res = twCon.UpdateProfile(name, url, location, description, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return "Err:Forbidden(" + MethodBase.GetCurrentMethod().Name + ")";
                    }
                    else
                    {
                        return "Err:" + errMsg;
                    }
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string PostUpdateProfileImage(string filename)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                res = twCon.UpdateProfileImage(new FileInfo(filename), ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    return "";
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.Forbidden:
                    var errMsg = GetErrorMessageJson(content);
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        return "Err:Forbidden(" + MethodBase.GetCurrentMethod().Name + ")";
                    }
                    else
                    {
                        return "Err:" + errMsg;
                    }
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }
        }

        public string Username
        {
            get
            {
                return twCon.AuthenticatedUsername;
            }
        }

        public long UserId
        {
            get
            {
                return twCon.AuthenticatedUserId;
            }
        }

        public string Password
        {
            get
            {
                return twCon.Password;
            }
        }

        private static MyCommon.ACCOUNT_STATE _accountState = MyCommon.ACCOUNT_STATE.Valid;
        public static MyCommon.ACCOUNT_STATE AccountState
        {
            get
            {
                return _accountState;
            }
            set
            {
                _accountState = value;
            }
        }

        public bool GetIcon
        {
            set
            {
                _getIcon = value;
            }
        }

        public bool TinyUrlResolve
        {
            set
            {
                _tinyUrlResolve = value;
            }
        }

        public bool RestrictFavCheck
        {
            set
            {
                _restrictFavCheck = value;
            }
        }

        public int IconSize
        {
            set
            {
                _iconSz = value;
            }
        }

#region "バージョンアップ"
        public string GetVersionInfo()
        {
            var content = "";
            if (!(new HttpVarious()).GetData(ApplicationSettings.VersionInfoUrl + "?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), null, out content, MyCommon.GetUserAgentString()))
            {
                throw new Exception("GetVersionInfo Failed");
            }
            return content;
        }

        public string GetTweenBinary(string strVer)
        {
            try
            {
                //本体
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/Tween" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(),
                                                    Path.Combine(MyCommon.settingPath, "TweenNew.exe")))
                {
                    return "Err:Download failed";
                }
                //英語リソース
                if (!Directory.Exists(Path.Combine(MyCommon.settingPath, "en")))
                {
                    Directory.CreateDirectory(Path.Combine(MyCommon.settingPath, "en"));
                }
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenResEn" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(),
                                                    Path.Combine(Path.Combine(MyCommon.settingPath, "en"), "Tween.resourcesNew.dll")))
                {
                    return "Err:Download failed";
                }
                //その他言語圏のリソース。取得失敗しても継続
                //UIの言語圏のリソース
                var curCul = "";
                if (!Thread.CurrentThread.CurrentUICulture.IsNeutralCulture)
                {
                    var idx = Thread.CurrentThread.CurrentUICulture.Name.LastIndexOf('-');
                    if (idx > -1)
                    {
                        curCul = Thread.CurrentThread.CurrentUICulture.Name.Substring(0, idx);
                    }
                    else
                    {
                        curCul = Thread.CurrentThread.CurrentUICulture.Name;
                    }
                }
                else
                {
                    curCul = Thread.CurrentThread.CurrentUICulture.Name;
                }
                if (!string.IsNullOrEmpty(curCul) && curCul != "en" && curCul != "ja")
                {
                    if (!Directory.Exists(Path.Combine(MyCommon.settingPath, curCul)))
                    {
                        Directory.CreateDirectory(Path.Combine(MyCommon.settingPath, curCul));
                    }
                    if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenRes" + curCul + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(),
                                                        Path.Combine(Path.Combine(MyCommon.settingPath, curCul), "Tween.resourcesNew.dll")))
                    {
                        //return "Err:Download failed";
                    }
                }
                //スレッドの言語圏のリソース
                string curCul2;
                if (!Thread.CurrentThread.CurrentCulture.IsNeutralCulture)
                {
                    var idx = Thread.CurrentThread.CurrentCulture.Name.LastIndexOf('-');
                    if (idx > -1)
                    {
                        curCul2 = Thread.CurrentThread.CurrentCulture.Name.Substring(0, idx);
                    }
                    else
                    {
                        curCul2 = Thread.CurrentThread.CurrentCulture.Name;
                    }
                }
                else
                {
                    curCul2 = Thread.CurrentThread.CurrentCulture.Name;
                }
                if (!string.IsNullOrEmpty(curCul2) && curCul2 != "en" && curCul2 != curCul)
                {
                    if (!Directory.Exists(Path.Combine(MyCommon.settingPath, curCul2)))
                    {
                        Directory.CreateDirectory(Path.Combine(MyCommon.settingPath, curCul2));
                    }
                    if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenRes" + curCul2 + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(),
                                                    Path.Combine(Path.Combine(MyCommon.settingPath, curCul2), "Tween.resourcesNew.dll")))
                    {
                        //return "Err:Download failed";
                    }
                }

                //アップデータ
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenUp3.gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(),
                                                    Path.Combine(MyCommon.settingPath, "TweenUp3.exe")))
                {
                    return "Err:Download failed";
                }
                //シリアライザDLL
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenDll" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(),
                                                    Path.Combine(MyCommon.settingPath, "TweenNew.XmlSerializers.dll")))
                {
                    return "Err:Download failed";
                }
                return "";
            }
            catch(Exception)
            {
                return "Err:Download failed";
            }
        }
#endregion

        public bool ReadOwnPost
        {
            get
            {
                return _readOwnPost;
            }
            set
            {
                _readOwnPost = value;
            }
        }

        public int FollowersCount
        {
            get
            {
                return _followersCount;
            }
        }

        public int FriendsCount
        {
            get
            {
                return _friendsCount;
            }
        }

        public int StatusesCount
        {
            get
            {
                return _statusesCount;
            }
        }

        public string Location
        {
            get
            {
                return _location;
            }
        }

        public string Bio
        {
            get
            {
                return _bio;
            }
        }

        public bool UseSsl
        {
            set
            {
                HttpTwitter.UseSsl = value;
                if (value)
                {
                    _protocol = "https://";
                }
                else
                {
                    _protocol = "http://";
                }
            }
        }

        public string GetTimelineApi(bool read,
                                MyCommon.WORKERTYPE gType,
                                bool more,
                                bool startup)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (MyCommon._endingFlag) return "";

            HttpStatusCode res;
            var content = "";
            var count = AppendSettingDialog.Instance.CountApi;
            if (gType == MyCommon.WORKERTYPE.Reply) count = AppendSettingDialog.Instance.CountApiReply;
            if (AppendSettingDialog.Instance.UseAdditionalCount)
            {
                if (more && AppendSettingDialog.Instance.MoreCountApi != 0)
                {
                    count = AppendSettingDialog.Instance.MoreCountApi;
                }
                else if (startup && AppendSettingDialog.Instance.FirstCountApi != 0 && gType == MyCommon.WORKERTYPE.Timeline)
                {
                    count = AppendSettingDialog.Instance.FirstCountApi;
                }
            }
            try
            {
                if (gType == MyCommon.WORKERTYPE.Timeline)
                {
                    if (more)
                    {
                        res = twCon.HomeTimeline(count, this.minHomeTimeline, 0, ref content);
                    }
                    else
                    {
                        res = twCon.HomeTimeline(count, 0, 0, ref content);
                    }
                }
                else
                {
                    if (more)
                    {
                        res = twCon.Mentions(count, this.minMentions, 0, ref content);
                    }
                    else
                    {
                        res = twCon.Mentions(count, 0, 0, ref content);
                    }
                }
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            if (gType == MyCommon.WORKERTYPE.Timeline)
            {
                return CreatePostsFromJson(content, gType, null, read, count, ref this.minHomeTimeline);
            }
            else
            {
                return CreatePostsFromJson(content, gType, null, read, count, ref this.minMentions);
            }
        }

        public string GetUserTimelineApi(bool read,
                                         int count,
                                         string userName,
                                         TabClass tab,
                                         bool more)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (MyCommon._endingFlag) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            if (count == 0) count = 20;
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    var target = tab.User;
                    if (string.IsNullOrEmpty(target)) return "";
                    userName = target;
                    res = twCon.UserTimeline(0, target, count, 0, 0, ref content);
                }
                else
                {
                    if (more)
                    {
                        res = twCon.UserTimeline(0, userName, count, tab.OldestId, 0, ref content);
                    }
                    else
                    {
                        res = twCon.UserTimeline(0, userName, count, 0, 0, ref content);
                    }
                }
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    return "Err:@" + userName + "'s Tweets are protected.";
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            List<TwitterDataModel.Status> items;
            try
            {
                items = MyCommon.CreateDataFromJson<List<TwitterDataModel.Status>>(content);
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            foreach (var status in items)
            {
                var item = CreatePostsFromStatusData(status);
                if (item == null) continue;
                if (item.StatusId < tab.OldestId) tab.OldestId = item.StatusId;
                item.IsRead = read;
                if (item.IsMe && !read && _readOwnPost) item.IsRead = true;
                if (tab != null) item.RelTabName = tab.TabName;
                //非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(item);
            }

            return "";
        }

        public string GetStatusApi(bool read,
                                   Int64 id,
                                   ref PostClass post)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (MyCommon._endingFlag) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.ShowStatuses(id, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.Forbidden:
                    return "Err:protected user's tweet";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            TwitterDataModel.Status status;
            try
            {
                status = MyCommon.CreateDataFromJson<TwitterDataModel.Status>(content);
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            var item = CreatePostsFromStatusData(status);
            if (item == null) return "Err:Can't create post";
            item.IsRead = read;
            if (item.IsMe && !read && _readOwnPost) item.IsRead = true;

            post = item;
            return "";
        }

        public string GetStatusApi(bool read,
                                   Int64 id,
                                   TabClass tab)
        {
            PostClass post = null;
            var r = this.GetStatusApi(read, id, ref post);

            if (string.IsNullOrEmpty(r))
            {
                if (tab != null) post.RelTabName = tab.TabName;
                //非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(post);
            }

            return r;
        }

        private PostClass CreatePostsFromStatusData(TwitterDataModel.Status status)
        {
            var post = new PostClass();
            TwitterDataModel.Entities entities;

            post.StatusId = status.Id;
            if (status.RetweetedStatus != null)
            {
                var retweeted = status.RetweetedStatus;

                post.CreatedAt = MyCommon.DateTimeParse(retweeted.CreatedAt);

                //Id
                post.RetweetedId = retweeted.Id;
                //本文
                post.TextFromApi = retweeted.Text;
                entities = retweeted.Entities;
                //Source取得（htmlの場合は、中身を取り出し）
                post.Source = retweeted.Source;
                //Reply先
                long inReplyToStatusId;
                long.TryParse(retweeted.InReplyToStatusId, out inReplyToStatusId);
                post.InReplyToStatusId = inReplyToStatusId;
                post.InReplyToUser = retweeted.InReplyToScreenName;
                long inReplyToUserId;
                long.TryParse(status.InReplyToUserId, out inReplyToUserId);
                post.InReplyToUserId = inReplyToUserId;

                //幻覚fav対策
                var tc = TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites);
                post.IsFav = tc.Contains(post.RetweetedId);

                if (retweeted.Geo != null) post.PostGeo = new PostClass.StatusGeo {Lat = retweeted.Geo.Coordinates[0], Lng = retweeted.Geo.Coordinates[1]};

                //以下、ユーザー情報
                var user = retweeted.User;

                if (user.ScreenName == null || status.User.ScreenName == null) return null;

                post.UserId = user.Id;
                post.ScreenName = user.ScreenName;
                post.Nickname = user.Name.Trim();
                post.ImageUrl = user.ProfileImageUrlHttps;
                post.IsProtect = user.Protected;

                //Retweetした人
                post.RetweetedBy = status.User.ScreenName;
                post.RetweetedByUserId = status.User.Id;
                post.IsMe = post.RetweetedBy.ToLower().Equals(_uname);
            }
            else
            {
                post.CreatedAt = MyCommon.DateTimeParse(status.CreatedAt);
                //本文
                post.TextFromApi = status.Text;
                entities = status.Entities;
                //Source取得（htmlの場合は、中身を取り出し）
                post.Source = status.Source;
                long inReplyToStatusId;
                long.TryParse(status.InReplyToStatusId, out inReplyToStatusId);
                post.InReplyToStatusId = inReplyToStatusId;
                post.InReplyToUser = status.InReplyToScreenName;
                long inReplyToUserId;
                long.TryParse(status.InReplyToUserId, out inReplyToUserId);
                post.InReplyToUserId = inReplyToUserId;

                if (status.Geo != null) post.PostGeo = new PostClass.StatusGeo {Lat = status.Geo.Coordinates[0], Lng = status.Geo.Coordinates[1]};

                //以下、ユーザー情報
                var user = status.User;

                if (user.ScreenName == null) return null;

                post.UserId = user.Id;
                post.ScreenName = user.ScreenName;
                post.Nickname = user.Name.Trim();
                post.ImageUrl = user.ProfileImageUrlHttps;
                post.IsProtect = user.Protected;
                post.IsMe = post.ScreenName.ToLower().Equals(_uname);

                //幻覚fav対策
                var tc = TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites);
                post.IsFav = tc.Contains(post.StatusId) && TabInformations.GetInstance()[post.StatusId].IsFav;
            }
            //HTMLに整形
            string textFromApi = post.TextFromApi;
            post.Text = CreateHtmlAnchor(ref textFromApi, post.ReplyToList, entities, post.Media);
            post.TextFromApi = textFromApi;
            post.TextFromApi = this.ReplaceTextFromApi(post.TextFromApi, entities);
            post.TextFromApi = WebUtility.HtmlDecode(post.TextFromApi);
            post.TextFromApi = post.TextFromApi.Replace("<3", "\u2661");

            //Source整形
            CreateSource(post);

            post.IsReply = post.ReplyToList.Contains(_uname);
            post.IsExcludeReply = false;

            if (post.IsMe)
            {
                post.IsOwl = false;
            }
            else
            {
                if (followerId.Count > 0) post.IsOwl = !followerId.Contains(post.UserId);
            }

            post.IsDm = false;
            return post;
        }

        private string CreatePostsFromJson(string content, MyCommon.WORKERTYPE gType, TabClass tab, bool read, int count, ref long minimumId)
        {
            List<TwitterDataModel.Status> items;
            try
            {
                items = MyCommon.CreateDataFromJson<List<TwitterDataModel.Status>>(content);
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";;
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            foreach (var status in items)
            {
                PostClass post = null;
                post = CreatePostsFromStatusData(status);
                if (post == null) continue;

                if (minimumId > post.StatusId) minimumId = post.StatusId;
                //二重取得回避
                lock (LockObj)
                {
                    if (tab == null)
                    {
                        if (TabInformations.GetInstance().ContainsKey(post.StatusId)) continue;
                    }
                    else
                    {
                        if (TabInformations.GetInstance().ContainsKey(post.StatusId, tab.TabName)) continue;
                    }
                }

                //RT禁止ユーザーによるもの
                if (post.RetweetedId > 0 && this.noRTId.Contains(post.RetweetedByUserId)) continue;

                post.IsRead = read;
                if (post.IsMe && !read && _readOwnPost) post.IsRead = true;

                if (tab != null) post.RelTabName = tab.TabName;
                //非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(post);
            }

            return "";
        }

        private string CreatePostsFromSearchJson(string content, TabClass tab, bool read, int count, ref long minimumId, bool more)
        {
            TwitterDataModel.SearchResult items;
            try
            {
                items = MyCommon.CreateDataFromJson<TwitterDataModel.SearchResult>(content);
            }
            catch (SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }
            foreach (var result in items.Results)
            {
                PostClass post = null;
                post = CreatePostsFromSearchResultData(result);
                if (post == null) continue;

                if (minimumId > post.StatusId) minimumId = post.StatusId;
                if (!more && post.StatusId > tab.SinceId) tab.SinceId = post.StatusId;
                //二重取得回避
                lock (LockObj)
                {
                    if (tab == null)
                    {
                        if (TabInformations.GetInstance().ContainsKey(post.StatusId)) continue;
                    }
                    else
                    {
                        if (TabInformations.GetInstance().ContainsKey(post.StatusId, tab.TabName)) continue;
                    }
                }

                post.IsRead = read;
                if ((post.IsMe && !read) && this._readOwnPost) post.IsRead = true;

                if (tab != null) post.RelTabName = tab.TabName;
                //非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(post);
            }

            return "";
        }

        private PostClass CreatePostsFromSearchResultData(TwitterDataModel.SearchResultData status)
        {
            var post = new PostClass();
            post.StatusId = status.Id;
            post.CreatedAt = MyCommon.DateTimeParse(status.CreatedAt);
            //本文
            post.TextFromApi = status.Text;
            var entities = status.Entities;
            post.Source = WebUtility.HtmlDecode(status.Source);
            post.InReplyToStatusId = status.InReplyToStatusId;
            post.InReplyToUser = status.ToUser;
            post.InReplyToUserId = !status.ToUserId.HasValue ? 0 : (long)status.ToUserId;

            if (status.Geo != null) post.PostGeo = new PostClass.StatusGeo { Lat = status.Geo.Coordinates[0], Lng = status.Geo.Coordinates[1] };

            if (status.FromUser == null) return null;

            post.UserId = status.FromUserId;
            post.ScreenName = status.FromUser;
            post.Nickname = status.FromUserName.Trim();
            post.ImageUrl = status.ProfileImageUrl;
            post.IsProtect = false;
            post.IsMe = post.ScreenName.ToLower().Equals(this._uname);

            //幻覚fav対策
            var tc = TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites);
            post.IsFav = tc.Contains(post.StatusId) && TabInformations.GetInstance()[post.StatusId].IsFav;

            //HTMLに整形
            string textFromApi = post.TextFromApi;
            post.Text = this.CreateHtmlAnchor(ref textFromApi, post.ReplyToList, entities, post.Media);
            post.TextFromApi = this.ReplaceTextFromApi(post.TextFromApi, entities);
            post.TextFromApi = WebUtility.HtmlDecode(post.TextFromApi);
            post.TextFromApi = post.TextFromApi.Replace("<3", "\u2661");

            //Source整形
            this.CreateSource(post);

            post.IsReply = post.ReplyToList.Contains(this._uname);
            post.IsExcludeReply = false;
            post.IsOwl = false;
            post.IsDm = false;

            return post;
        }

        private string CreatePostsFromPhoenixSearch(string content, MyCommon.WORKERTYPE gType, TabClass tab, bool read, int count, ref long minimumId, ref string nextPageQuery)
        {
            TwitterDataModel.SearchResultPhoenix items;
            try
            {
                items = MyCommon.CreateDataFromJson<TwitterDataModel.SearchResultPhoenix>(content);
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            nextPageQuery = items.NextPage;

            foreach (var status in items.Statuses)
            {
                PostClass post = null;
                post = CreatePostsFromStatusData(status);
                if (post == null) continue;

                if (minimumId > post.StatusId) minimumId = post.StatusId;
                //二重取得回避
                lock (LockObj)
                {
                    if (tab == null)
                    {
                        if (TabInformations.GetInstance().ContainsKey(post.StatusId)) continue;
                    }
                    else
                    {
                        if (TabInformations.GetInstance().ContainsKey(post.StatusId, tab.TabName)) continue;
                    }
                }

                post.IsRead = read;
                if (post.IsMe && !read && _readOwnPost) post.IsRead = true;

                if (tab != null) post.RelTabName = tab.TabName;
                //非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance().AddPost(post);
            }

            return string.IsNullOrEmpty(items.ErrMsg) ? "" : "Err:" + items.ErrMsg;
        }

        public string GetListStatus(bool read,
                                TabClass tab,
                                bool more,
                                bool startup)
        {
            if (MyCommon._endingFlag) return "";

            HttpStatusCode res;
            var content = "";
            int count;
            if (AppendSettingDialog.Instance.UseAdditionalCount)
            {
                count = AppendSettingDialog.Instance.ListCountApi;
                if (count == 0)
                {
                    if (more && AppendSettingDialog.Instance.MoreCountApi != 0)
                    {
                        count = AppendSettingDialog.Instance.MoreCountApi;
                    }
                    else if (startup && AppendSettingDialog.Instance.FirstCountApi != 0)
                    {
                        count = AppendSettingDialog.Instance.FirstCountApi;
                    }
                    else
                    {
                        count = AppendSettingDialog.Instance.CountApi;
                    }
                }
            }
            else
            {
                count = AppendSettingDialog.Instance.CountApi;
            }
            try
            {
                if (more)
                {
                    res = twCon.GetListsStatuses(tab.ListInfo.UserId, tab.ListInfo.Id, count, tab.OldestId, 0, AppendSettingDialog.Instance.IsListStatusesIncludeRts, ref content);
                }
                else
                {
                    res = twCon.GetListsStatuses(tab.ListInfo.UserId, tab.ListInfo.Id, count, 0, 0, AppendSettingDialog.Instance.IsListStatusesIncludeRts, ref content);
                }
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            return CreatePostsFromJson(content, MyCommon.WORKERTYPE.List, tab, read, count, ref tab.OldestId);
        }

        /// <summary>
        /// startStatusId からリプライ先の発言を辿る。発言は posts 以外からは検索しない。
        /// </summary>
        /// <returns>posts の中から検索されたリプライチェインの末端</returns>
        internal static PostClass FindTopOfReplyChain(IDictionary<Int64, PostClass> posts, Int64 startStatusId)
        {
            if (!posts.ContainsKey(startStatusId))
                throw new ArgumentException("startStatusId (" + startStatusId + ") が posts の中から見つかりませんでした。");

            var nextPost = posts[startStatusId];
            while (nextPost.InReplyToStatusId != 0)
            {
                if (!posts.ContainsKey(nextPost.InReplyToStatusId))
                    break;
                nextPost = posts[nextPost.InReplyToStatusId];
            }

            return nextPost;
        }

        public string GetRelatedResult(bool read, TabClass tab)
        {
            var rslt = "";
            var relPosts = new Dictionary<Int64, PostClass>();
            if (tab.RelationTargetPost.TextFromApi.Contains("@") && tab.RelationTargetPost.InReplyToStatusId == 0)
            {
                //検索結果対応
                var p = TabInformations.GetInstance()[tab.RelationTargetPost.StatusId];
                if (p != null && p.InReplyToStatusId > 0)
                {
                    tab.RelationTargetPost = p;
                }
                else
                {
                    rslt = this.GetStatusApi(read, tab.RelationTargetPost.StatusId, ref p);
                    if (!string.IsNullOrEmpty(rslt)) return rslt;
                    tab.RelationTargetPost = p;
                }
            }
            relPosts.Add(tab.RelationTargetPost.StatusId, tab.RelationTargetPost.Clone());

            // 一周目: 非公式な related_results API を使用してリプライチェインを辿る
            var nextPost = relPosts[tab.RelationTargetPost.StatusId];
            var loopCount = 1;
            do
            {
                rslt = this.GetRelatedResultsApi(nextPost, relPosts);
                if (!string.IsNullOrEmpty(rslt)) break;
                nextPost = FindTopOfReplyChain(relPosts, nextPost.StatusId);
            } while (nextPost.InReplyToStatusId != 0 && loopCount++ <= 5);

            // 二周目: in_reply_to_status_id を使用してリプライチェインを辿る
            nextPost = FindTopOfReplyChain(relPosts, tab.RelationTargetPost.StatusId);
            loopCount = 1;
            while (nextPost.InReplyToStatusId != 0 && loopCount++ <= 20)
            {
                var inReplyToId = nextPost.InReplyToStatusId;

                var inReplyToPost = TabInformations.GetInstance()[inReplyToId];
                if (inReplyToPost != null)
                {
                    inReplyToPost = inReplyToPost.Clone();
                }
                else
                {
                    var errorText = this.GetStatusApi(read, inReplyToId, ref inReplyToPost);
                    if (!string.IsNullOrEmpty(errorText))
                    {
                        rslt = errorText;
                        break;
                    }
                }

                relPosts.Add(inReplyToPost.StatusId, inReplyToPost);

                nextPost = FindTopOfReplyChain(relPosts, nextPost.StatusId);
            }

            //MRTとかに対応のためツイート内にあるツイートを指すURLを取り込む
            var text = tab.RelationTargetPost.Text;
            var ma = Twitter.StatusUrlRegex.Matches(text).Cast<Match>()
                .Concat(Twitter.ThirdPartyStatusUrlRegex.Matches(text).Cast<Match>());
            foreach (var _match in ma)
            {
                Int64 _statusId;
                if (Int64.TryParse(_match.Groups["StatusId"].Value, out _statusId))
                {
                    if (relPosts.ContainsKey(_statusId))
                        continue;

                    PostClass p = null;
                    var _post = TabInformations.GetInstance()[_statusId];
                    if (_post == null)
                    {
                        this.GetStatusApi(read, _statusId, ref p);
                    }
                    else
                    {
                        p = _post.Clone();
                    }

                    if (p != null)
                        relPosts.Add(p.StatusId, p);
                }
            }

            relPosts.Values.ToList().ForEach(p =>
            {
                if (p.IsMe && !read && this._readOwnPost)
                    p.IsRead = true;
                else
                    p.IsRead = read;

                p.RelTabName = tab.TabName;
                TabInformations.GetInstance().AddPost(p);
            });

            return rslt;
        }

        private string GetRelatedResultsApi(PostClass post,
                                            IDictionary<Int64, PostClass> relatedPosts)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (MyCommon._endingFlag) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                if (post.RetweetedId > 0)
                {
                    res = twCon.GetRelatedResults(post.RetweetedId, ref content);
                }
                else
                {
                    res = twCon.GetRelatedResults(post.StatusId, ref content);
                }
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            List<TwitterDataModel.RelatedResult> items;
            try
            {
                items = MyCommon.CreateDataFromJson<List<TwitterDataModel.RelatedResult>>(content);
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            foreach (var relatedData in items)
            {
                foreach (var result in relatedData.Results)
                {
                    var item = CreatePostsFromStatusData(result.Status);
                    if (item == null) continue;
                    //非同期アイコン取得＆StatusDictionaryに追加
                    if (!relatedPosts.ContainsKey(item.StatusId))
                        relatedPosts.Add(item.StatusId, item);
                }
            }

            return "";
        }

        public string GetSearch(bool read,
                            TabClass tab,
                            bool more)
        {
            if (MyCommon._endingFlag) return "";

            HttpStatusCode res;
            var content = "";
            var page = 0;
            var sinceId = 0L;
            var count = 100;
            if (AppendSettingDialog.Instance.UseAdditionalCount &&
                AppendSettingDialog.Instance.SearchCountApi != 0)
            {
                count = AppendSettingDialog.Instance.SearchCountApi;
            }
            else
            {
                count = AppendSettingDialog.Instance.CountApi;
            }
            if (more)
            {
                page = tab.GetSearchPage(count);
            }
            else
            {
                sinceId = tab.SinceId;
            }

            try
            {
                // TODO:一時的に40>100件に 件数変更UI作成の必要あり
                res = twCon.Search(tab.SearchWords, tab.SearchLang, count, page, sinceId, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.BadRequest:
                    return "Invalid query";
                case HttpStatusCode.NotFound:
                    return "Invalid query";
                case HttpStatusCode.PaymentRequired: //API Documentには420と書いてあるが、該当コードがないので402にしてある
                    return "Search API Limit?";
                case HttpStatusCode.OK:
                    break;
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            if (!TabInformations.GetInstance().ContainsTab(tab)) return "";

            return this.CreatePostsFromSearchJson(content, tab, read, count, ref tab.OldestId, more);
        }

        public string GetPhoenixSearch(bool read,
                                TabClass tab,
                                bool more)
        {
            if (MyCommon._endingFlag) return "";

            HttpStatusCode res;
            var content = "";
            var page = 0;
            var sinceId = 0L;
            var count = 100;
            var querystr = "";
            if (AppendSettingDialog.Instance.UseAdditionalCount &&
                AppendSettingDialog.Instance.SearchCountApi != 0)
            {
                count = AppendSettingDialog.Instance.SearchCountApi;
            }
            if (more)
            {
                page = tab.GetSearchPage(count);
                if (!string.IsNullOrEmpty(tab.NextPageQuery))
                {
                    querystr = tab.NextPageQuery;
                }
            }
            else
            {
                sinceId = tab.SinceId;
            }

            try
            {
                if (string.IsNullOrEmpty(querystr))
                {
                    res = twCon.PhoenixSearch(tab.SearchWords, tab.SearchLang, count, page, sinceId, ref content);
                }
                else
                {
                    res = twCon.PhoenixSearch(querystr, ref content);
                }
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }
            switch (res)
            {
                case HttpStatusCode.BadRequest:
                    return "Invalid query";
                case HttpStatusCode.NotFound:
                    return "Invalid query";
                case HttpStatusCode.PaymentRequired: //API Documentには420と書いてあるが、該当コードがないので402にしてある
                    return "Search API Limit?";
                case HttpStatusCode.OK:
                    break;
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            if (!TabInformations.GetInstance().ContainsTab(tab)) return "";

            //// TODO
            //// 遡るための情報max_idやnext_pageの情報を保持する

            string nextPageQuery = tab.NextPageQuery;
            var ret = CreatePostsFromPhoenixSearch(content, MyCommon.WORKERTYPE.PublicSearch, tab, read, count, ref tab.OldestId, ref nextPageQuery);
            tab.NextPageQuery = nextPageQuery;
            return ret;
        }

        private string CreateDirectMessagesFromJson(string content, MyCommon.WORKERTYPE gType, bool read)
        {
            List<TwitterDataModel.Directmessage> item;
            try
            {
                if (gType == MyCommon.WORKERTYPE.UserStream)
                {
                    var itm = MyCommon.CreateDataFromJson<List<TwitterDataModel.DirectmessageEvent>>(content);
                    item = new List<TwitterDataModel.Directmessage>();
                    foreach (var dat in itm)
                    {
                        item.Add(dat.Directmessage);
                    }
                }
                else
                {
                    item = MyCommon.CreateDataFromJson<List<TwitterDataModel.Directmessage>>(content);
                }
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            foreach (var message in item)
            {
                var post = new PostClass();
                try
                {
                    post.StatusId = message.Id;
                    if (gType != MyCommon.WORKERTYPE.UserStream)
                    {
                        if (gType == MyCommon.WORKERTYPE.DirectMessegeRcv)
                        {
                            if (minDirectmessage > post.StatusId) minDirectmessage = post.StatusId;
                        }
                        else
                        {
                            if (minDirectmessageSent > post.StatusId) minDirectmessageSent = post.StatusId;
                        }
                    }

                    //二重取得回避
                    lock (LockObj)
                    {
                        if (TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.DirectMessage).Contains(post.StatusId)) continue;
                    }
                    //sender_id
                    //recipient_id
                    post.CreatedAt = MyCommon.DateTimeParse(message.CreatedAt);
                    //本文
                    post.TextFromApi = message.Text;
                    //HTMLに整形
                    post.Text = CreateHtmlAnchor(post.TextFromApi, post.ReplyToList, post.Media);
                    post.TextFromApi = WebUtility.HtmlDecode(post.TextFromApi);
                    post.TextFromApi = post.TextFromApi.Replace("<3", "\u2661");
                    post.IsFav = false;

                    //以下、ユーザー情報
                    TwitterDataModel.User user;
                    if (gType == MyCommon.WORKERTYPE.UserStream)
                    {
                        if (twCon.AuthenticatedUsername.Equals(message.Recipient.ScreenName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            user = message.Sender;
                            post.IsMe = false;
                            post.IsOwl = true;
                        }
                        else
                        {
                            user = message.Recipient;
                            post.IsMe = true;
                            post.IsOwl = false;
                        }
                    }
                    else
                    {
                        if (gType == MyCommon.WORKERTYPE.DirectMessegeRcv)
                        {
                            user = message.Sender;
                            post.IsMe = false;
                            post.IsOwl = true;
                        }
                        else
                        {
                            user = message.Recipient;
                            post.IsMe = true;
                            post.IsOwl = false;
                        }
                    }

                    post.UserId = user.Id;
                    post.ScreenName = user.ScreenName;
                    post.Nickname = user.Name.Trim();
                    post.ImageUrl = user.ProfileImageUrlHttps;
                    post.IsProtect = user.Protected;
                }
                catch(Exception ex)
                {
                    MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                    MessageBox.Show("Parse Error(CreateDirectMessagesFromJson)");
                    continue;
                }

                post.IsRead = read;
                if (post.IsMe && !read && _readOwnPost) post.IsRead = true;
                post.IsReply = false;
                post.IsExcludeReply = false;
                post.IsDm = true;

                TabInformations.GetInstance().AddPost(post);
            }

            return "";

        }

        public string GetDirectMessageApi(bool read,
                                MyCommon.WORKERTYPE gType,
                                bool more)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";
            if (MyCommon.TwitterApiInfo.AccessLevel == TwitterApiAccessLevel.Read || MyCommon.TwitterApiInfo.AccessLevel == TwitterApiAccessLevel.ReadWrite)
            {
                return "Auth Err:try to re-authorization.";
            }

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                if (gType == MyCommon.WORKERTYPE.DirectMessegeRcv)
                {
                    if (more)
                    {
                        res = twCon.DirectMessages(20, minDirectmessage, 0, ref content);
                    }
                    else
                    {
                        res = twCon.DirectMessages(20, 0, 0, ref content);
                    }
                }
                else
                {
                    if (more)
                    {
                        res = twCon.DirectMessagesSent(20, minDirectmessageSent, 0, ref content);
                    }
                    else
                    {
                        res = twCon.DirectMessagesSent(20, 0, 0, ref content);
                    }
                }
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            return CreateDirectMessagesFromJson(content, gType, read);
        }

        static int page_ = 1;
        public string GetFavoritesApi(bool read,
                            MyCommon.WORKERTYPE gType,
                            bool more)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (MyCommon._endingFlag) return "";

            HttpStatusCode res;
            var content = "";
            var count = AppendSettingDialog.Instance.CountApi;
            if (AppendSettingDialog.Instance.UseAdditionalCount &&
                AppendSettingDialog.Instance.FavoritesCountApi != 0)
            {
                count = AppendSettingDialog.Instance.FavoritesCountApi;
            }

            // 前ページ取得の場合はページカウンタをインクリメント、それ以外の場合はページカウンタリセット
            if (more)
            {
                page_++;
            }
            else
            {
                page_ = 1;
            }

            try
            {
                res = twCon.Favorites(count, page_, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            var serializer = new DataContractJsonSerializer(typeof(List<TwitterDataModel.Status>));
            List<TwitterDataModel.Status> item;

            try
            {
                item = MyCommon.CreateDataFromJson<List<TwitterDataModel.Status>>(content);
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Invalid Json!";
            }

            foreach (var status in item)
            {
                var post = new PostClass();
                TwitterDataModel.Entities entities;

                try
                {
                    post.StatusId = status.Id;
                    //二重取得回避
                    lock (LockObj)
                    {
                        if (TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).Contains(post.StatusId)) continue;
                    }
                    //Retweet判定
                    if (status.RetweetedStatus != null)
                    {
                        var retweeted = status.RetweetedStatus;
                        post.CreatedAt = MyCommon.DateTimeParse(retweeted.CreatedAt);

                        //Id
                        post.RetweetedId = post.StatusId;
                        //本文
                        post.TextFromApi = retweeted.Text;
                        entities = retweeted.Entities;
                        //Source取得（htmlの場合は、中身を取り出し）
                        post.Source = retweeted.Source;
                        //Reply先
                        long inReplyToStatusId;
                        long.TryParse(retweeted.InReplyToStatusId, out inReplyToStatusId);
                        post.InReplyToStatusId = inReplyToStatusId;
                        post.InReplyToUser = retweeted.InReplyToScreenName;
                        long inReplyToUserId;
                        long.TryParse(retweeted.InReplyToUserId, out inReplyToUserId);
                        post.InReplyToUserId = inReplyToUserId;
                        post.IsFav = true;

                        //以下、ユーザー情報
                        var user = retweeted.User;
                        post.UserId = user.Id;
                        post.ScreenName = user.ScreenName;
                        post.Nickname = user.Name.Trim();
                        post.ImageUrl = user.ProfileImageUrlHttps;
                        post.IsProtect = user.Protected;

                        //Retweetした人
                        post.RetweetedBy = status.User.ScreenName;
                        post.IsMe = post.RetweetedBy.ToLower().Equals(_uname);
                    }
                    else
                    {
                        post.CreatedAt = MyCommon.DateTimeParse(status.CreatedAt);

                        //本文
                        post.TextFromApi = status.Text;
                        entities = status.Entities;
                        //Source取得（htmlの場合は、中身を取り出し）
                        post.Source = status.Source;
                        long inReplyToStatusId;
                        long.TryParse(status.InReplyToStatusId, out inReplyToStatusId);
                        post.InReplyToStatusId = inReplyToStatusId;
                        post.InReplyToUser = status.InReplyToScreenName;
                        long inReplyToUserId;
                        long.TryParse(status.InReplyToUserId, out inReplyToUserId);
                        post.InReplyToUserId = inReplyToUserId;

                        post.IsFav = true;

                        //以下、ユーザー情報
                        var user = status.User;
                        post.UserId = user.Id;
                        post.ScreenName = user.ScreenName;
                        post.Nickname = user.Name.Trim();
                        post.ImageUrl = user.ProfileImageUrlHttps;
                        post.IsProtect = user.Protected;
                        post.IsMe = post.ScreenName.ToLower().Equals(_uname);
                    }
                    //HTMLに整形
                    string textFromApi = post.TextFromApi;
                    post.Text = CreateHtmlAnchor(ref textFromApi, post.ReplyToList, entities, post.Media);
                    post.TextFromApi = textFromApi;
                    post.TextFromApi = this.ReplaceTextFromApi(post.TextFromApi, entities);
                    post.TextFromApi = WebUtility.HtmlDecode(post.TextFromApi);
                    post.TextFromApi = post.TextFromApi.Replace("<3", "\u2661");
                    //Source整形
                    CreateSource(post);

                    post.IsRead = read;
                    post.IsReply = post.ReplyToList.Contains(_uname);
                    post.IsExcludeReply = false;

                    if (post.IsMe)
                    {
                        post.IsOwl = false;
                    }
                    else
                    {
                        if (followerId.Count > 0) post.IsOwl = !followerId.Contains(post.UserId);
                    }

                    post.IsDm = false;
                }
                catch(Exception ex)
                {
                    MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                    continue;
                }

                TabInformations.GetInstance().AddPost(post);

            }

            return "";
        }

        private string ReplaceTextFromApi(string text, TwitterDataModel.Entities entities)
        {
            if (entities != null)
            {
                if (entities.Urls != null)
                {
                    foreach (var m in entities.Urls)
                    {
                        if (!string.IsNullOrEmpty(m.DisplayUrl)) text = text.Replace(m.Url, m.DisplayUrl);
                    }
                }
                if (entities.Media != null)
                {
                    foreach (var m in entities.Media)
                    {
                        if (!string.IsNullOrEmpty(m.DisplayUrl)) text = text.Replace(m.Url, m.DisplayUrl);
                    }
                }
            }
            return text;
        }

        public string GetFollowersApi()
        {
            if (MyCommon._endingFlag) return "";
            long cursor = -1;
            var tmpFollower = new List<long>(followerId);

            followerId.Clear();
            do
            {
                var ret = FollowerApi(ref cursor);
                if (!string.IsNullOrEmpty(ret))
                {
                    followerId.Clear();
                    followerId.AddRange(tmpFollower);
                    _GetFollowerResult = false;
                    return ret;
                }
            } while (cursor > 0);

            TabInformations.GetInstance().RefreshOwl(followerId);

            _GetFollowerResult = true;
            return "";
        }

        public bool GetFollowersSuccess
        {
            get
            {
                return _GetFollowerResult;
            }
        }

        private string FollowerApi(ref long cursor)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                res = twCon.FollowerIds(cursor, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            try
            {
                var followers = MyCommon.CreateDataFromJson<TwitterDataModel.Ids>(content);
                followerId.AddRange(followers.Id);
                cursor = followers.NextCursor;
                return "";
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public string GetNoRetweetIdsApi()
        {
            if (MyCommon._endingFlag) return "";
            long cursor = -1;
            var tmpIds = new List<long>(noRTId);

            noRTId.Clear();
            do
            {
                var ret = NoRetweetApi(ref cursor);
                if (!string.IsNullOrEmpty(ret))
                {
                    noRTId.Clear();
                    noRTId.AddRange(tmpIds);
                    _GetNoRetweetResult = false;
                    return ret;
                }
            } while (cursor > 0);

            _GetNoRetweetResult = true;
            return "";
        }

        private string NoRetweetApi(ref long cursor)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                res = twCon.NoRetweetIds(cursor, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            try
            {
                var ids = MyCommon.CreateDataFromJson<long[]>(content);
                noRTId.AddRange(ids);
                cursor = 0;  //0より小さければ何でも良い。
                return "";
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public bool GetNoRetweetSuccess
        {
            get
            {
                return _GetNoRetweetResult;
            }
        }

        public string ConfigurationApi()
        {
            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.GetConfiguration(ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            try
            {
                AppendSettingDialog.Instance.TwitterConfiguration = MyCommon.CreateDataFromJson<TwitterDataModel.Configuration>(content);
                return "";
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public string GetListsApi()
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            long cursor = -1;

            var lists = new List<ListElement>();
            do
            {
                try
                {
                    res = twCon.GetLists(this.Username, cursor, ref content);
                }
                catch(Exception ex)
                {
                    return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
                }

                switch (res)
                {
                    case HttpStatusCode.OK:
                        Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                        break;
                    case HttpStatusCode.Unauthorized:
                        Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                        return Properties.Resources.Unauthorized;
                    case HttpStatusCode.BadRequest:
                        return "Err:API Limits?";
                    default:
                        return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
                }

                try
                {
                    var lst = MyCommon.CreateDataFromJson<TwitterDataModel.Lists>(content);
                    lists.AddRange(from le in lst.lists select new ListElement(le, this));
                    cursor = lst.NextCursor;
                }
                catch(SerializationException ex)
                {
                    MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                    return "Err:Json Parse Error(DataContractJsonSerializer)";
                }
                catch(Exception ex)
                {
                    MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                    return "Err:Invalid Json!";
                }
            } while (cursor != 0);

            cursor = -1;
            content = "";
            do
            {
                try
                {
                    res = twCon.GetListsSubscriptions(this.Username, cursor, ref content);
                }
                catch(Exception ex)
                {
                    return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
                }

                switch (res)
                {
                    case HttpStatusCode.OK:
                        Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                        break;
                    case HttpStatusCode.Unauthorized:
                        Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                        return Properties.Resources.Unauthorized;
                    case HttpStatusCode.BadRequest:
                        return "Err:API Limits?";
                    default:
                        return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
                }

                try
                {
                    var lst = MyCommon.CreateDataFromJson<TwitterDataModel.Lists>(content);
                    lists.AddRange(from le in lst.lists select new ListElement(le, this));
                    cursor = lst.NextCursor;
                }
                catch(SerializationException ex)
                {
                    MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                    return "Err:Json Parse Error(DataContractJsonSerializer)";
                }
                catch(Exception ex)
                {
                    MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                    return "Err:Invalid Json!";
                }
            } while (cursor != 0);

            TabInformations.GetInstance().SubscribableLists = lists;
            return "";
        }

        public string DeleteList(string list_id)
        {
            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.DeleteListID(this.Username, list_id, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            return "";
        }

        public string EditList(string list_id, string new_name, bool isPrivate, string description, ref ListElement list)
        {
            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.UpdateListID(this.Username, list_id, new_name, isPrivate, description, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            try
            {
                var le = MyCommon.CreateDataFromJson<TwitterDataModel.ListElementData>(content);
                var newList = new ListElement(le, this);
                list.Description = newList.Description;
                list.Id = newList.Id;
                list.IsPublic = newList.IsPublic;
                list.MemberCount = newList.MemberCount;
                list.Name = newList.Name;
                list.SubscriberCount = newList.SubscriberCount;
                list.Slug = newList.Slug;
                list.Nickname = newList.Nickname;
                list.Username = newList.Username;
                list.UserId = newList.UserId;
                return "";
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }

        }

        public string GetListMembers(string list_id, List<UserInfo> lists, ref long cursor)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            //Do
            try
            {
                res = twCon.GetListMembers(this.Username, list_id, cursor, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            try
            {
                var users = MyCommon.CreateDataFromJson<TwitterDataModel.Users>(content);
                Array.ForEach<TwitterDataModel.User>(
                    users.users,
                    new Action<TwitterDataModel.User>(u => lists.Add(new UserInfo(u))));
                cursor = users.NextCursor;
                return "";
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public string CreateListApi(string listName, bool isPrivate, string description)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.CreateLists(listName, isPrivate, description, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            try
            {
                var le = MyCommon.CreateDataFromJson<TwitterDataModel.ListElementData>(content);
                TabInformations.GetInstance().SubscribableLists.Add(new ListElement(le, this));
                return "";
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }
        }

        public string ContainsUserAtList(string listId, string user, ref bool value)
        {
            value = false;

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = this.twCon.ShowListMember(listId, user, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                case HttpStatusCode.NotFound:
                    value = false;
                    return "";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            try
            {
                var u = MyCommon.CreateDataFromJson<TwitterDataModel.User>(content);
                value = true;
                return "";
            }
            catch(Exception)
            {
                value = false;
                return "";
            }
        }

        public string AddUserToList(string listId, string user)
        {
            var content = "";
            HttpStatusCode res = HttpStatusCode.BadRequest;

            try
            {
                res = twCon.CreateListMembers(listId, user, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:" + GetErrorMessageJson(content);
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            return "";
        }

        public string RemoveUserToList(string listId, string user)
        {

            var content = "";
            HttpStatusCode res = HttpStatusCode.BadRequest;

            try
            {
                res = twCon.DeleteListMembers(listId, user, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:" + GetErrorMessageJson(content);
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            return "";
        }

        private class range
        {
            public int fromIndex { get; set; }
            public int toIndex { get; set; }
            public range(int fromIndex, int toIndex)
            {
                this.fromIndex = fromIndex;
                this.toIndex = toIndex;
            }
        }
        public string CreateHtmlAnchor(string Text, List<string> AtList, Dictionary<string, string> media)
        {
            if (Text == null) return null;
            var retStr = Text.Replace("&gt;", "<<<<<tweenだいなり>>>>>").Replace("&lt;", "<<<<<tweenしょうなり>>>>>");
            //uriの正規表現
            //const string url_valid_domain = "(?<domain>(?:[^\p{P}\s][\.\-_](?=[^\p{P}\s])|[^\p{P}\s]){1,}\.[a-z]{2,}(?::[0-9]+)?)"
            //const string url_valid_general_path_chars = "[a-z0-9!*';:=+$/%#\[\]\-_,~]"
            //const string url_balance_parens = "(?:\(" + url_valid_general_path_chars + "+\))"
            //const string url_valid_url_path_ending_chars = "(?:[a-z0-9=_#/\-\+]+|" + url_balance_parens + ")"
            //const string pth = "(?:" + url_balance_parens +
            //    "|@" + url_valid_general_path_chars + "+/" +
            //    "|[.,]?" + url_valid_general_path_chars + "+" +
            //    ")"
            //const string pth2 = "(/(?:" +
            //    pth + "+" + url_valid_url_path_ending_chars + "|" +
            //    pth + "+" + url_valid_url_path_ending_chars + "?|" +
            //    url_valid_url_path_ending_chars +
            //    ")?)?"
            //const string qry = "(?<query>\?[a-z0-9!*'();:&=+$/%#\[\]\-_.,~]*[a-z0-9_&=#])?"
            //const string rgUrl = "(?<before>(?:[^\""':!=#]|^|\:/))" +
            //                            "(?<url>(?<protocol>https?://)" +
            //                            url_valid_domain +
            //                            pth2 +
            //                            qry +
            //                            ")"
            //const string rgUrl = "(?<before>(?:[^\""':!=#]|^|\:/))" +
            //                            "(?<url>(?<protocol>https?://|www\.)" +
            //                            url_valid_domain +
            //                            pth2 +
            //                            qry +
            //                            ")"
            //絶対パス表現のUriをリンクに置換
            retStr = Regex.Replace(retStr,
                                   rgUrl,
                                   new MatchEvaluator((Match mu) =>
                                                      {
                                                          var sb = new StringBuilder(mu.Result("${before}<a href=\""));
                                                          //if (mu.Result("${protocol}").StartsWith("w", StringComparison.OrdinalIgnoreCase))
                                                          //    sb.Append("http://");
                                                          //}
                                                          var url = mu.Result("${url}");
                                                          var title = ShortUrl.ResolveMedia(url, true);
                                                          if (url != title)
                                                          {
                                                              title = ShortUrl.ResolveMedia(title, false);
                                                          }
                                                          sb.Append(url + "\" title=\"" + title + "\">").Append(url).Append("</a>");
                                                          if (media != null && !media.ContainsKey(url)) media.Add(url, title);
                                                          return sb.ToString();
                                                      }),
                                   RegexOptions.IgnoreCase);

            //@先をリンクに置換（リスト）
            retStr = Regex.Replace(retStr,
                                   @"(^|[^a-zA-Z0-9_/])([@＠]+)([a-zA-Z0-9_]{1,20}/[a-zA-Z][a-zA-Z0-9\p{IsLatin-1Supplement}\-]{0,79})",
                                   "$1$2<a href=\"/$3\">$3</a>");

            var m = Regex.Match(retStr, "(^|[^a-zA-Z0-9_])[@＠]([a-zA-Z0-9_]{1,20})");
            while (m.Success)
            {
                if (!AtList.Contains(m.Result("$2").ToLower())) AtList.Add(m.Result("$2").ToLower());
                m = m.NextMatch();
            }
            //@先をリンクに置換
            retStr = Regex.Replace(retStr,
                                   "(^|[^a-zA-Z0-9_/])([@＠])([a-zA-Z0-9_]{1,20})",
                                   "$1$2<a href=\"/$3\">$3</a>");

            //ハッシュタグを抽出し、リンクに置換
            var anchorRange = new List<range>();
            for (int i = 0; i < retStr.Length; i++)
            {
                var index = retStr.IndexOf("<a ", i);
                if (index > -1 && index < retStr.Length)
                {
                    i = index;
                    var toIndex = retStr.IndexOf("</a>", index);
                    if (toIndex > -1)
                    {
                        anchorRange.Add(new range(index, toIndex + 3));
                        i = toIndex;
                    }
                }
            }
            //retStr = Regex.Replace(retStr,
            //                       "(^|[^a-zA-Z0-9/&])([#＃])([0-9a-zA-Z_]*[a-zA-Z_]+[a-zA-Z0-9_\xc0-\xd6\xd8-\xf6\xf8-\xff]*)",
            //                       new MatchEvaluator(Function(mh As Match)
            //                                              foreach (var rng in anchorRange)
            //                                              {
            //                                                  if (mh.Index >= rng.fromIndex &&
            //                                                   mh.Index <= rng.toIndex) return mh.Result("$0");
            //                                              }
            //                                              if (IsNumeric(mh.Result("$3"))) return mh.Result("$0");
            //                                              lock (LockObj)
            //                                              {
            //                                                  _hashList.Add("#" + mh.Result("$3"))
            //                                              }
            //                                              return mh.Result("$1") + "<a href=\"" + _protocol + "twitter.com/search?q=%23" + mh.Result("$3") + "\">" + mh.Result("$2$3") + "</a>";
            //                                          }),
            //                                      RegexOptions.IgnoreCase)
            retStr = Regex.Replace(retStr,
                                   HASHTAG,
                                   new MatchEvaluator(mh =>
                                                      {
                                                          foreach (var rng in anchorRange)
                                                          {
                                                              if (mh.Index >= rng.fromIndex &&
                                                               mh.Index <= rng.toIndex) return mh.Result("$0");
                                                          }
                                                          lock (LockObj)
                                                          {
                                                              _hashList.Add("#" + mh.Result("$3"));
                                                          }
                                                          return mh.Result("$1") + "<a href=\"" + _protocol + "twitter.com/search?q=%23" + mh.Result("$3") + "\">" + mh.Result("$2$3") + "</a>";
                                                      }),
                                                  RegexOptions.IgnoreCase);


            retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/&#＃@＠>=.~])(sm|nm)([0-9]{1,10})", "$1<a href=\"http://www.nicovideo.jp/watch/$2$3\">$2$3</a>");

            retStr = retStr.Replace("<<<<<tweenだいなり>>>>>", "&gt;").Replace("<<<<<tweenしょうなり>>>>>", "&lt;");

            //retStr = AdjustHtml(ShortUrl.Resolve(PreProcessUrl(retStr), true)) //IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
            retStr = AdjustHtml(PreProcessUrl(retStr)); //IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
            return retStr;
        }

        private class EntityInfo
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public string Text { get; set; }
            public string Html { get; set; }
            public string Display { get; set; }
        }
        public string CreateHtmlAnchor(ref string Text, List<string> AtList, TwitterDataModel.Entities entities, Dictionary<string, string> media)
        {
            var ret = Text;

            if (entities != null)
            {
                var etInfo = new SortedList<int, EntityInfo>();
                //URL
                if (entities.Urls != null)
                {
                    foreach (var ent in entities.Urls)
                    {
                        if (string.IsNullOrEmpty(ent.DisplayUrl))
                        {
                            etInfo.Add(ent.Indices[0],
                                       new EntityInfo {StartIndex = ent.Indices[0],
                                                       EndIndex = ent.Indices[1],
                                                       Text = ent.Url,
                                                       Html = "<a href=\"" + ent.Url + "\">" + ent.Url + "</a>"});
                        }
                        else
                        {
                            var expanded = ShortUrl.ResolveMedia(ent.ExpandedUrl, false);
                            etInfo.Add(ent.Indices[0],
                                       new EntityInfo {StartIndex = ent.Indices[0],
                                                       EndIndex = ent.Indices[1],
                                                       Text = ent.Url,
                                                       Html = "<a href=\"" + ent.Url + "\" title=\"" + expanded + "\">" + ent.DisplayUrl + "</a>",
                                                       Display = ent.DisplayUrl});
                            if (media != null && !media.ContainsKey(ent.Url)) media.Add(ent.Url, expanded);
                        }
                    }
                }
                if (entities.Hashtags != null)
                {
                    foreach (var ent in entities.Hashtags)
                    {
                        var hash = Text.Substring(ent.Indices[0], ent.Indices[1] - ent.Indices[0]);
                        etInfo.Add(ent.Indices[0],
                                   new EntityInfo {StartIndex = ent.Indices[0],
                                                   EndIndex = ent.Indices[1],
                                                   Text = hash,
                                                   Html = "<a href=\"" + _protocol + "twitter.com/search?q=%23" + ent.Text + "\">" + hash + "</a>"});
                        lock (LockObj)
                        {
                            _hashList.Add("#" + ent.Text);
                        }
                    }
                }
                if (entities.UserMentions != null)
                {
                    foreach (var ent in entities.UserMentions)
                    {
                        var screenName = Text.Substring(ent.Indices[0] + 1, ent.Indices[1] - ent.Indices[0] - 1);
                        etInfo.Add(ent.Indices[0] + 1,
                                   new EntityInfo {StartIndex = ent.Indices[0] + 1,
                                                   EndIndex = ent.Indices[1],
                                                   Text = ent.ScreenName,
                                                   Html = "<a href=\"/" + ent.ScreenName + "\">" + screenName + "</a>"});
                        if (!AtList.Contains(ent.ScreenName.ToLower())) AtList.Add(ent.ScreenName.ToLower());
                    }
                }
                if (entities.Media != null)
                {
                    foreach (var ent in entities.Media)
                    {
                        if (ent.Type == "photo")
                        {
                            etInfo.Add(ent.Indices[0],
                                       new EntityInfo {StartIndex = ent.Indices[0],
                                                       EndIndex = ent.Indices[1],
                                                       Text = ent.Url,
                                                       Html = "<a href=\"" + ent.Url + "\" title=\"" + ent.ExpandedUrl + "\">" + ent.DisplayUrl + "</a>",
                                                       Display = ent.DisplayUrl});
                            if (media != null && !media.ContainsKey(ent.Url)) media.Add(ent.Url, ent.MediaUrl);
                        }
                    }
                }
                if (etInfo.Count > 0)
                {
                    try
                    {
                        var idx = 0;
                        ret = "";
                        foreach (var et in etInfo)
                        {
                            ret += Text.Substring(idx, et.Key - idx) + et.Value.Html;
                            idx = et.Value.EndIndex;
                        }
                        ret += Text.Substring(idx);
                    }
                    catch(ArgumentOutOfRangeException)
                    {
                        //Twitterのバグで不正なエンティティ（Index指定範囲が重なっている）が返ってくる場合の対応
                        ret = Text;
                        entities = null;
                        if (media != null) media.Clear();
                    }
                }
            }

            ret = Regex.Replace(ret, "(^|[^a-zA-Z0-9_/&#＃@＠>=.~])(sm|nm)([0-9]{1,10})", "$1<a href=\"http://www.nicovideo.jp/watch/$2$3\">$2$3</a>");
            ret = AdjustHtml(ShortUrl.Resolve(PreProcessUrl(ret), false)); //IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与

            return ret;
        }

        //Source整形
        private void CreateSource(PostClass post)
        {
            if (string.IsNullOrEmpty(post.Source)) return;

            if (post.Source.StartsWith("<"))
            {
                if (!post.Source.Contains("</a>"))
                {
                    post.Source += "</a>";
                }
                var mS = Regex.Match(post.Source, ">(?<source>.+)<");
                if (mS.Success)
                {
                    post.SourceHtml = string.Copy(ShortUrl.Resolve(PreProcessUrl(post.Source), false));
                    post.Source = WebUtility.HtmlDecode(mS.Result("${source}"));
                }
                else
                {
                    post.Source = "";
                    post.SourceHtml = "";
                }
            }
            else
            {
                if (post.Source == "web")
                {
                    post.SourceHtml = Properties.Resources.WebSourceString;
                }
                else if (post.Source == "Keitai Mail")
                {
                    post.SourceHtml = Properties.Resources.KeitaiMailSourceString;
                }
                else
                {
                    post.SourceHtml = string.Copy(post.Source);
                }
            }
        }

        public bool GetInfoApi()
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return true;

            if (MyCommon._endingFlag) return true;

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";
            try
            {
                res = twCon.RateLimitStatus(ref content);
            }
            catch(Exception)
            {
                MyCommon.TwitterApiInfo.Reset();
                return false;
            }

            if (res != HttpStatusCode.OK) return false;

            try
            {
                var limit = MyCommon.CreateDataFromJson<TwitterDataModel.RateLimitStatus>(content);
                MyCommon.TwitterApiInfo.UpdateFromApi(limit);
                return true;
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                MyCommon.TwitterApiInfo.Reset();
                return false;
            }
        }
        public string GetBlockUserIds()
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res = HttpStatusCode.BadRequest;
            var content = "";

            try
            {
                res = twCon.GetBlockUserIds(ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            switch (res)
            {
                case HttpStatusCode.OK:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    break;
                case HttpStatusCode.Unauthorized:
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    return Properties.Resources.Unauthorized;
                case HttpStatusCode.BadRequest:
                    return "Err:API Limits?";
                default:
                    return "Err:" + res.ToString() + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            try
            {
                var Ids = MyCommon.CreateDataFromJson<List<long>>(content);
                if (Ids.Contains(this.UserId)) Ids.Remove(this.UserId);
                TabInformations.GetInstance().BlockIds.AddRange(Ids);
                return ("");
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex.Message + Environment.NewLine + content);
                return "Err:Json Parse Error(DataContractJsonSerializer)";
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                return "Err:Invalid Json!";
            }

        }

        public string[] GetHashList()
        {
            string[] hashArray;
            lock (LockObj)
            {
                hashArray = _hashList.ToArray();
                _hashList.Clear();
            }
            return hashArray;
        }

        public string AccessToken
        {
            get
            {
                return twCon.AccessToken;
            }
        }

        public string AccessTokenSecret
        {
            get
            {
                return twCon.AccessTokenSecret;
            }
        }

#region "UserStream"
        private string trackWord_ = "";
        public string TrackWord
        {
            get
            {
                return trackWord_;
            }
            set
            {
                trackWord_ = value;
            }
        }
        private bool allAtReply_ = false;
        public bool AllAtReply
        {
            get
            {
                return allAtReply_;
            }
            set
            {
                allAtReply_ = value;
            }
        }

        public event Action NewPostFromStream;
        public event Action UserStreamStarted;
        public event Action UserStreamStopped;
        public event Action<long> PostDeleted;
        public event Action<FormattedEvent> UserStreamEventReceived;
        private DateTime _lastUserstreamDataReceived;
        private TwitterUserstream userStream;

        public class FormattedEvent
        {
            public MyCommon.EVENTTYPE Eventtype { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Event { get; set; }
            public string Username { get; set; }
            public string Target { get; set; }
            public Int64 Id { get; set; }
            public bool IsMe { get; set; }
        }

        public List<FormattedEvent> storedEvent_ = new List<FormattedEvent>();
        public List<FormattedEvent> StoredEvent
        {
            get
            {
                return storedEvent_;
            }
            set
            {
                storedEvent_ = value;
            }
        }

        private class EventTypeTableElement
        {
            public string Name;
            public MyCommon.EVENTTYPE Type;

            public EventTypeTableElement(string name, MyCommon.EVENTTYPE type)
            {
                this.Name = name;
                this.Type = type;
            }
        }

        private EventTypeTableElement[] EventTable = {
            new EventTypeTableElement("favorite", MyCommon.EVENTTYPE.Favorite),
            new EventTypeTableElement("unfavorite", MyCommon.EVENTTYPE.Unfavorite),
            new EventTypeTableElement("follow", MyCommon.EVENTTYPE.Follow),
            new EventTypeTableElement("list_member_added", MyCommon.EVENTTYPE.ListMemberAdded),
            new EventTypeTableElement("list_member_removed", MyCommon.EVENTTYPE.ListMemberRemoved),
            new EventTypeTableElement("block", MyCommon.EVENTTYPE.Block),
            new EventTypeTableElement("unblock", MyCommon.EVENTTYPE.Unblock),
            new EventTypeTableElement("user_update", MyCommon.EVENTTYPE.UserUpdate),
            new EventTypeTableElement("deleted", MyCommon.EVENTTYPE.Deleted),
            new EventTypeTableElement("list_created", MyCommon.EVENTTYPE.ListCreated),
            new EventTypeTableElement("list_updated", MyCommon.EVENTTYPE.ListUpdated),
            new EventTypeTableElement("unfollow", MyCommon.EVENTTYPE.Unfollow),
        };

        public MyCommon.EVENTTYPE EventNameToEventType(string EventName)
        {
            return (from tbl in EventTable where tbl.Name.Equals(EventName) select tbl.Type).FirstOrDefault();
        }

        public bool IsUserstreamDataReceived
        {
            get
            {
                return DateTime.Now.Subtract(this._lastUserstreamDataReceived).TotalSeconds < 31;
            }
        }

        private void userStream_StatusArrived(string line)
        {
            this._lastUserstreamDataReceived = DateTime.Now;
            if (string.IsNullOrEmpty(line)) return;

            var isDm = false;

            try
            {
                using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(line), XmlDictionaryReaderQuotas.Max))
                {
                    var xElm = XElement.Load(jsonReader);
                    if (xElm.Element("friends") != null)
                    {
                        Debug.WriteLine("friends");
                        return;
                    }
                    else if (xElm.Element("delete") != null)
                    {
                        Debug.WriteLine("delete");
                        Int64 id;
                        if (xElm.Element("delete").Element("direct_message") != null &&
                            xElm.Element("delete").Element("direct_message").Element("id") != null)
                        {
                            id = 0;
                            long.TryParse(xElm.Element("delete").Element("direct_message").Element("id").Value, out id);
                            if (PostDeleted != null)
                            {
                                PostDeleted(id);
                            }
                        }
                        else if (xElm.Element("delete").Element("status") != null &&
                            xElm.Element("delete").Element("status").Element("id") != null)
                        {
                            id = 0;
                            long.TryParse(xElm.Element("delete").Element("status").Element("id").Value, out id);
                            if (PostDeleted != null)
                            {
                                PostDeleted(id);
                            }
                        }
                        else
                        {
                            MyCommon.TraceOut("delete:" + line);
                            return;
                        }
                        for (int i = this.StoredEvent.Count - 1; i >= 0; i--)
                        {
                            var sEvt = this.StoredEvent[i];
                            if (sEvt.Id == id && (sEvt.Event == "favorite" || sEvt.Event == "unfavorite"))
                            {
                                this.StoredEvent.RemoveAt(i);
                            }
                        }
                        return;
                    }
                    else if (xElm.Element("limit") != null)
                    {
                        Debug.WriteLine(line);
                        return;
                    }
                    else if (xElm.Element("event") != null)
                    {
                        Debug.WriteLine("event: " + xElm.Element("event").Value);
                        CreateEventFromJson(line);
                        return;
                    }
                    else if (xElm.Element("direct_message") != null)
                    {
                        Debug.WriteLine("direct_message");
                        isDm = true;
                    }
                    else if (xElm.Element("scrub_geo") != null)
                    {
                        try
                        {
                            TabInformations.GetInstance().ScrubGeoReserve(long.Parse(xElm.Element("scrub_geo").Element("user_id").Value),
                                                                        long.Parse(xElm.Element("scrub_geo").Element("up_to_status_id").Value));
                        }
                        catch(Exception)
                        {
                            MyCommon.TraceOut("scrub_geo:" + line);
                        }
                        return;
                    }
                }

                var res = new StringBuilder();
                res.Length = 0;
                res.Append("[");
                res.Append(line);
                res.Append("]");

                if (isDm)
                {
                    CreateDirectMessagesFromJson(res.ToString(), MyCommon.WORKERTYPE.UserStream, false);
                }
                else
                {
                    long dummy = 0;
                    CreatePostsFromJson(res.ToString(), MyCommon.WORKERTYPE.Timeline, null, false, 0, ref dummy);
                }
            }
            catch(NullReferenceException)
            {
                MyCommon.TraceOut("NullRef StatusArrived: " + line);
            }

            if (NewPostFromStream != null)
            {
                NewPostFromStream();
            }
        }

        private void CreateEventFromJson(string content)
        {
            TwitterDataModel.EventData eventData = null;
            try
            {
                eventData = MyCommon.CreateDataFromJson<TwitterDataModel.EventData>(content);
            }
            catch(SerializationException ex)
            {
                MyCommon.TraceOut(ex, "Event Serialize Exception!" + Environment.NewLine + content);
            }
            catch(Exception ex)
            {
                MyCommon.TraceOut(ex, "Event Exception!" + Environment.NewLine + content);
            }

            var evt = new FormattedEvent();
            evt.CreatedAt = MyCommon.DateTimeParse(eventData.CreatedAt);
            evt.Event = eventData.Event;
            evt.Username = eventData.Source.ScreenName;
            evt.IsMe = evt.Username.ToLower().Equals(this.Username.ToLower());
            evt.Eventtype = EventNameToEventType(evt.Event);
            switch (eventData.Event)
            {
                case "access_revoked":
                    return;
                case "follow":
                    if (eventData.Target.ScreenName.ToLower().Equals(_uname))
                    {
                        if (!this.followerId.Contains(eventData.Source.Id)) this.followerId.Add(eventData.Source.Id);
                    }
                    else
                    {
                        return;    //Block後のUndoをすると、SourceとTargetが逆転したfollowイベントが帰ってくるため。
                    }
                    evt.Target = "";
                    break;
                case "unfollow":
                    return;
                case "favorite":
                case "unfavorite":
                    evt.Target = "@" + eventData.TargetObject.User.ScreenName + ":" + WebUtility.HtmlDecode(eventData.TargetObject.Text);
                    evt.Id = eventData.TargetObject.Id;
                    if (AppendSettingDialog.Instance.IsRemoveSameEvent)
                    {
                        if (StoredEvent.Any(ev =>
                                           {
                                               return ev.Username == evt.Username && ev.Eventtype == evt.Eventtype && ev.Target == evt.Target;
                                           })) return;
                    }
                    if (TabInformations.GetInstance().ContainsKey(eventData.TargetObject.Id))
                    {
                        var post = TabInformations.GetInstance()[eventData.TargetObject.Id];
                        if (eventData.Event == "favorite")
                        {
                            if (evt.Username.ToLower().Equals(_uname))
                            {
                                post.IsFav = true;
                                TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).Add(post.StatusId, post.IsRead, false);
                            }
                            else
                            {
                                post.FavoritedCount++;
                                if (!TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).Contains(post.StatusId))
                                {
                                    if (AppendSettingDialog.Instance.FavEventUnread && post.IsRead)
                                    {
                                        post.IsRead = false;
                                    }
                                    TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).Add(post.StatusId, post.IsRead, false);
                                }
                                else
                                {
                                    if (AppendSettingDialog.Instance.FavEventUnread)
                                    {
                                        TabInformations.GetInstance().SetRead(false, TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).TabName, TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites).IndexOf(post.StatusId));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (evt.Username.ToLower().Equals(_uname))
                            {
                                post.IsFav = false;
                            }
                            else
                            {
                                post.FavoritedCount--;
                                if (post.FavoritedCount < 0) post.FavoritedCount = 0;
                            }
                        }
                    }
                    break;
                case "list_member_added":
                case "list_member_removed":
                case "list_updated":
                    evt.Target = eventData.TargetObject.FullName;
                    break;
                case "block":
                    if (!TabInformations.GetInstance().BlockIds.Contains(eventData.Target.Id)) TabInformations.GetInstance().BlockIds.Add(eventData.Target.Id);
                    evt.Target = "";
                    break;
                case "unblock":
                    if (TabInformations.GetInstance().BlockIds.Contains(eventData.Target.Id)) TabInformations.GetInstance().BlockIds.Remove(eventData.Target.Id);
                    evt.Target = "";
                    break;
                case "user_update":
                    evt.Target = "";
                    break;
                case "list_created":
                    evt.Target = "";
                    break;
                default:
                    MyCommon.TraceOut("Unknown Event:" + evt.Event + Environment.NewLine + content);
                    break;
            }
            this.StoredEvent.Insert(0, evt);
            if (UserStreamEventReceived != null)
            {
                UserStreamEventReceived(evt);
            }
        }

        private void userStream_Started()
        {
            if (UserStreamStarted != null)
            {
                UserStreamStarted();
            }
        }

        private void userStream_Stopped()
        {
            if (UserStreamStopped != null)
            {
                UserStreamStopped();
            }
        }

        public bool UserStreamEnabled
        {
            get
            {
                return userStream == null ? false : userStream.Enabled;
            }
        }

        public void StartUserStream()
        {
            if (userStream != null)
            {
                StopUserStream();
            }
            userStream = new TwitterUserstream(twCon);
            userStream.StatusArrived += userStream_StatusArrived;
            userStream.Started += userStream_Started;
            userStream.Stopped += userStream_Stopped;
            userStream.Start(this.AllAtReply, this.TrackWord);
        }

        public void StopUserStream()
        {
            if (userStream != null) userStream.Dispose();
            userStream = null;
            if (!MyCommon._endingFlag)
            {
                if (UserStreamStopped != null)
                {
                    UserStreamStopped();
                }
            }
        }

        public void ReconnectUserStream()
        {
            if (userStream != null)
            {
                this.StartUserStream();
            }
        }

        private class TwitterUserstream : IDisposable
        {
            public event Action<string> StatusArrived;
            public event Action Stopped;
            public event Action Started;
            private HttpTwitter twCon;

            private Thread _streamThread;
            private bool _streamActive;

            private bool _allAtreplies = false;
            private string _trackwords = "";

            public TwitterUserstream(HttpTwitter twitterConnection)
            {
                twCon = (HttpTwitter)twitterConnection.Clone();
            }

            public void Start(bool allAtReplies, string trackwords)
            {
                this.AllAtReplies = allAtReplies;
                this.TrackWords = trackwords;
                _streamActive = true;
                if (_streamThread != null && _streamThread.IsAlive) return;
                _streamThread = new Thread(UserStreamLoop);
                _streamThread.Name = "UserStreamReceiver";
                _streamThread.IsBackground = true;
                _streamThread.Start();
            }

            public bool Enabled
            {
                get
                {
                    return _streamActive;
                }
            }

            public bool AllAtReplies
            {
                get
                {
                    return _allAtreplies;
                }
                set
                {
                    _allAtreplies = value;
                }
            }

            public string TrackWords
            {
                get
                {
                    return _trackwords;
                }
                set
                {
                    _trackwords = value;
                }
            }

            private void UserStreamLoop()
            {
                var sleepSec = 0;
                do
                {
                    Stream st = null;
                    StreamReader sr = null;
                    try
                    {
                        if (!MyCommon.IsNetworkAvailable())
                        {
                            sleepSec = 30;
                            continue;
                        }

                        if (Started != null)
                        {
                            Started();
                        }
                        var res = twCon.UserStream(ref st, _allAtreplies, _trackwords, MyCommon.GetUserAgentString());

                        switch (res)
                        {
                            case HttpStatusCode.OK:
                                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                                break;
                            case HttpStatusCode.Unauthorized:
                                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                                sleepSec = 120;
                                continue;
                        }

                        if (st == null)
                        {
                            sleepSec = 30;
                            //MyCommon.TraceOut("Stop:stream is null")
                            continue;
                        }

                        sr = new StreamReader(st);

                        while (_streamActive && !sr.EndOfStream && Twitter.AccountState == MyCommon.ACCOUNT_STATE.Valid)
                        {
                            if (StatusArrived != null)
                            {
                                StatusArrived(sr.ReadLine());
                            }
                            //this.LastTime = Now;
                        }

                        if (sr.EndOfStream || Twitter.AccountState == MyCommon.ACCOUNT_STATE.Invalid)
                        {
                            sleepSec = 30;
                            //MyCommon.TraceOut("Stop:EndOfStream")
                            continue;
                        }
                        break;
                    }
                    catch(WebException ex)
                    {
                        if (ex.Status == WebExceptionStatus.Timeout)
                        {
                            sleepSec = 30;                        //MyCommon.TraceOut("Stop:Timeout")
                        }
                        else if (ex.Response != null && (int)((HttpWebResponse)ex.Response).StatusCode == 420)
                        {
                            //MyCommon.TraceOut("Stop:Connection Limit")
                            break;
                        }
                        else
                        {
                            sleepSec = 30;
                            //MyCommon.TraceOut("Stop:WebException " + ex.Status.ToString())
                        }
                    }
                    catch(ThreadAbortException)
                    {
                        break;
                    }
                    catch(IOException)
                    {
                        sleepSec = 30;
                        //MyCommon.TraceOut("Stop:IOException with Active." + Environment.NewLine + ex.Message)
                    }
                    catch(ArgumentException ex)
                    {
                        //System.ArgumentException: ストリームを読み取れませんでした。
                        //サーバー側もしくは通信経路上で切断された場合？タイムアウト頻発後発生
                        sleepSec = 30;
                        MyCommon.TraceOut(ex, "Stop:ArgumentException");
                    }
                    catch(Exception ex)
                    {
                        MyCommon.TraceOut("Stop:Exception." + Environment.NewLine + ex.Message);
                        MyCommon.ExceptionOut(ex);
                        sleepSec = 30;
                    }
                    finally
                    {
                        if (_streamActive)
                        {
                            if (Stopped != null)
                            {
                                Stopped();
                            }
                        }
                        twCon.RequestAbort();
                        if (sr != null) sr.Close();
                        if (sleepSec > 0)
                        {
                            var ms = 0;
                            while (_streamActive && ms < sleepSec * 1000)
                            {
                                Thread.Sleep(500);
                                ms += 500;
                            }
                        }
                        sleepSec = 0;
                    }
                } while (this._streamActive);

                if (_streamActive)
                {
                    if (Stopped != null)
                    {
                        Stopped();
                    }
                }
                MyCommon.TraceOut("Stop:EndLoop");
            }

#region "IDisposable Support"
            private bool disposedValue; // 重複する呼び出しを検出するには

            // IDisposable
            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                        _streamActive = false;
                        if (_streamThread != null && _streamThread.IsAlive)
                        {
                            _streamThread.Abort();
                        }
                    }

                    // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下の Finalize() をオーバーライドします。
                    // TODO: 大きなフィールドを null に設定します。
                }
                this.disposedValue = true;
            }

            // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードがある場合にのみ、Finalize() をオーバーライドします。
            //protected Overrides void Finalize()
            //{
            //    // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            //    Dispose(false)
            //    MyBase.Finalize()
            //}

            // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
            public void Dispose()
            {
                // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
                Dispose(true);
                GC.SuppressFinalize(this);
            }
#endregion

        }
#endregion

#region "IDisposable Support"
        private bool disposedValue; // 重複する呼び出しを検出するには

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    this.StopUserStream();
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下の Finalize() をオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。
            }
            this.disposedValue = true;
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードがある場合にのみ、Finalize() をオーバーライドします。
        //protected Overrides void Finalize()
        //{
        //    // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //    Dispose(false)
        //    MyBase.Finalize()
        //}

        // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
#endregion
    }
}
