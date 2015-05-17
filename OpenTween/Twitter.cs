// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
//           (c) 2013      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTween.Api;
using OpenTween.Connection;

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

        /// <summary>
        /// DM送信かどうかを判定する正規表現
        /// </summary>
        public static readonly Regex DMSendTextRegex = new Regex(@"^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public TwitterConfiguration Configuration { get; private set; }

        delegate void GetIconImageDelegate(PostClass post);
        private readonly object LockObj = new object();
        private ISet<long> followerId = new HashSet<long>();
        private bool _GetFollowerResult = false;
        private long[] noRTId = new long[0];
        private bool _GetNoRetweetResult = false;

        private int _followersCount = 0;
        private int _friendsCount = 0;
        private int _statusesCount = 0;
        private string _location = "";
        private string _bio = "";

        //プロパティからアクセスされる共通情報
        private string _uname;

        private bool _restrictFavCheck;

        private bool _readOwnPost;
        private List<string> _hashList = new List<string>();

        //max_idで古い発言を取得するために保持（lists分は個別タブで管理）
        private long minHomeTimeline = long.MaxValue;
        private long minMentions = long.MaxValue;
        private long minDirectmessage = long.MaxValue;
        private long minDirectmessageSent = long.MaxValue;

        //private FavoriteQueue favQueue;

        private HttpTwitter twCon = new HttpTwitter();

        //private List<PostClass> _deletemessages = new List<PostClass>();

        public Twitter()
        {
            this.Configuration = TwitterConfiguration.DefaultConfiguration();
        }

        public TwitterApiAccessLevel AccessLevel
        {
            get
            {
                return MyCommon.TwitterApiInfo.AccessLevel;
            }
        }

        protected void ResetApiStatus()
        {
            MyCommon.TwitterApiInfo.Reset();
        }

        public string Authenticate(string username, string password)
        {
            this.ResetApiStatus();

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.AuthUserAndPass(username, password, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            _uname = username.ToLower();
            if (SettingCommon.Instance.UserstreamStartup) this.ReconnectUserStream();
            return "";
        }

        public string StartAuthentication(ref string pinPageUrl)
        {
            //OAuth PIN Flow
            this.ResetApiStatus();
            try
            {
                var res = twCon.AuthGetRequestToken(ref pinPageUrl);
                if (!res)
                    return "Err:Failed to access auth server.";
            }
            catch(Exception)
            {
                return "Err:" + "Failed to access auth server.";
            }

            return "";
        }

        public string Authenticate(string pinCode)
        {
            this.ResetApiStatus();

            HttpStatusCode res;
            try
            {
                res = twCon.AuthGetAccessToken(pinCode);
            }
            catch(Exception)
            {
                return "Err:" + "Failed to access auth acc server.";
            }

            var err = this.CheckStatusCode(res, null);
            if (err != null) return err;

            _uname = Username.ToLower();
            if (SettingCommon.Instance.UserstreamStartup) this.ReconnectUserStream();
            return "";
        }

        public void ClearAuthInfo()
        {
            Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
            this.ResetApiStatus();
            twCon.ClearAuthInfo();
        }

        public void VerifyCredentials()
        {
            HttpStatusCode res;
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
                TwitterUser user;
                try
                {
                    user = TwitterUser.ParseJson(content);
                }
                catch(SerializationException)
                {
                    return;
                }
                twCon.AuthenticatedUserId = user.Id;
            }
        }

        public void Initialize(string token, string tokenSecret, string username, long userId)
        {
            //OAuth認証
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(tokenSecret) || string.IsNullOrEmpty(username))
            {
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
            }
            this.ResetApiStatus();
            twCon.Initialize(token, tokenSecret, username, userId);
            _uname = username.ToLower();
            if (SettingCommon.Instance.UserstreamStartup) this.ReconnectUserStream();
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

                    var replacedUrl = MyCommon.IDNEncode(urlStr);
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
        private bool IsPostRestricted(TwitterStatus status)
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

        public string PostStatus(string postStr, long? reply_to, List<long> mediaIds = null)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (mediaIds == null &&
                Twitter.DMSendTextRegex.IsMatch(postStr))
            {
                return SendDirectMessage(postStr);
            }

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.UpdateStatus(postStr, reply_to, mediaIds, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            // 投稿に成功していても404が返ることがあるらしい: https://dev.twitter.com/discussions/1213
            if (res == HttpStatusCode.NotFound) return "";

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            TwitterStatus status;
            try
            {
                status = TwitterStatus.ParseJson(content);
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
            return "";
        }

        public string PostStatusWithMedia(string postStr, long? reply_to, FileInfo mediaFile)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.UpdateStatusWithMedia(postStr, reply_to, mediaFile, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            // 投稿に成功していても404が返ることがあるらしい: https://dev.twitter.com/discussions/1213
            if (res == HttpStatusCode.NotFound) return "";

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            TwitterStatus status;
            try
            {
                status = TwitterStatus.ParseJson(content);
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
            return "";
        }

        public string PostStatusWithMultipleMedia(string postStr, long? reply_to, List<FileInfo> mediaFiles)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (Twitter.DMSendTextRegex.IsMatch(postStr))
            {
                return SendDirectMessage(postStr);
            }

            var mediaIds = new List<long>();

            foreach (var mediaFile in mediaFiles)
            {
                long? mediaId = null;
                var err = UploadMedia(mediaFile, ref mediaId);
                if (!mediaId.HasValue || !string.IsNullOrEmpty(err)) return err;
                mediaIds.Add(mediaId.Value);
            }

            if (mediaIds.Count == 0)
                return "Err:Invalid Files!";

            return PostStatus(postStr, reply_to, mediaIds);
        }

        public string UploadMedia(FileInfo mediaFile, ref long? mediaId)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.UploadMedia(mediaFile, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            TwitterUploadMediaResult status;
            try
            {
                status = TwitterUploadMediaResult.ParseJson(content);
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

            mediaId = status.MediaId;
            return "";
        }

        public string SendDirectMessage(string postStr)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (this.AccessLevel == TwitterApiAccessLevel.Read || this.AccessLevel == TwitterApiAccessLevel.ReadWrite)
            {
                return "Auth Err:try to re-authorization.";
            }

            var mc = Twitter.DMSendTextRegex.Match(postStr);

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.SendDirectMessage(mc.Groups["body"].Value, mc.Groups["id"].Value, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            TwitterDirectMessage status;
            try
            {
                status = TwitterDirectMessage.ParseJson(content);
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

            return "";
        }

        public string RemoveStatus(long id)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res;
            try
            {
                res = twCon.DestroyStatus(id);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            return this.CheckStatusCode(res, null) ?? "";
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
            if (TabInformations.GetInstance()[id].RetweetedId != null)
            {
                target = TabInformations.GetInstance()[id].RetweetedId.Value; //再RTの場合は元発言をRT
            }

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.RetweetStatus(target, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            TwitterStatus status;
            try
            {
                status = TwitterStatus.ParseJson(content);
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
            if (post.RetweetedId == null) return "Invalid Json!";
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

            if (this.AccessLevel == TwitterApiAccessLevel.Read || this.AccessLevel == TwitterApiAccessLevel.ReadWrite)
            {
                return "Auth Err:try to re-authorization.";
            }

            //if (post.IsMe)
            //    _deletemessages.Add(post)
            //}

            HttpStatusCode res;
            try
            {
                res = twCon.DestroyDirectMessage(id);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            return this.CheckStatusCode(res, null) ?? "";
        }

        public string PostFollowCommand(string screenName)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.CreateFriendships(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            return this.CheckStatusCode(res, content) ?? "";
        }

        public string PostRemoveCommand(string screenName)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.DestroyFriendships(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            return this.CheckStatusCode(res, content) ?? "";
        }

        public string PostCreateBlock(string screenName)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.CreateBlock(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            return this.CheckStatusCode(res, content) ?? "";
        }

        public string PostDestroyBlock(string screenName)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.DestroyBlock(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            return this.CheckStatusCode(res, content) ?? "";
        }

        public string PostReportSpam(string screenName)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.ReportSpam(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            return this.CheckStatusCode(res, content) ?? "";
        }

        public string GetFriendshipInfo(string screenName, ref bool isFollowing, ref bool isFollowed)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.ShowFriendships(_uname, screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            try
            {
                var friendship = TwitterFriendship.ParseJson(content);
                isFollowing = friendship.Relationship.Source.Following;
                isFollowed = friendship.Relationship.Source.FollowedBy;
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

        public string GetUserInfo(string screenName, ref TwitterUser user)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            user = null;

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.ShowUserInfo(screenName, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            try
            {
                user = TwitterUser.ParseJson(content);
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
        }

        public string GetStatus_Retweeted_Count(long StatusId, ref int retweeted_count)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (MyCommon._endingFlag) return "";

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.ShowStatuses(StatusId, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            TwitterStatus status;
            try
            {
                status = TwitterStatus.ParseJson(content);
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
            retweeted_count = status.RetweetCount;
            return "";
        }

        public string PostFavAdd(long id)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            //if (this.favQueue == null) this.favQueue = new FavoriteQueue(this)

            //if (this.favQueue.Contains(id)) this.favQueue.Remove(id)

            HttpStatusCode res;
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

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            if (!_restrictFavCheck) return "";

            //http://twitter.com/statuses/show/id.xml APIを発行して本文を取得

            try
            {
                res = twCon.ShowStatuses(id, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            TwitterStatus status;
            try
            {
                status = TwitterStatus.ParseJson(content);
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
            if (status.Favorited == true)
            {
                return "";
            }
            else
            {
                return "NG(Restricted?)";
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

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.DestroyFavorites(id, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            return this.CheckStatusCode(res, content) ?? "";
        }

        public TwitterUser PostUpdateProfile(string name, string url, string location, string description)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid)
                throw new WebApiException("AccountState invalid");

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.UpdateProfile(name, url, location, description, ref content);
            }
            catch(Exception ex)
            {
                throw new WebApiException("Err:" + ex.Message, content, ex);
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null)
                throw new WebApiException(err, content);

            try
            {
                return TwitterUser.ParseJson(content);
            }
            catch (SerializationException e)
            {
                var ex = new WebApiException("Err:Json Parse Error(DataContractJsonSerializer)", content, e);
                MyCommon.TraceOut(ex);
                throw ex;
            }
            catch (Exception e)
            {
                var ex = new WebApiException("Err:Invalid Json!", content, e);
                MyCommon.TraceOut(ex);
                throw ex;
            }
        }

        public string PostUpdateProfileImage(string filename)
        {
            if (MyCommon._endingFlag) return "";

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.UpdateProfileImage(new FileInfo(filename), ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            return this.CheckStatusCode(res, content) ?? "";
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

        public bool RestrictFavCheck
        {
            set
            {
                _restrictFavCheck = value;
            }
        }

#region "バージョンアップ"
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

        public string GetTimelineApi(bool read,
                                MyCommon.WORKERTYPE gType,
                                bool more,
                                bool startup)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (MyCommon._endingFlag) return "";

            HttpStatusCode res;
            var content = "";
            var count = SettingCommon.Instance.CountApi;
            if (gType == MyCommon.WORKERTYPE.Reply) count = SettingCommon.Instance.CountApiReply;
            if (SettingCommon.Instance.UseAdditionalCount)
            {
                if (more && SettingCommon.Instance.MoreCountApi != 0)
                {
                    count = SettingCommon.Instance.MoreCountApi;
                }
                else if (startup && SettingCommon.Instance.FirstCountApi != 0 && gType == MyCommon.WORKERTYPE.Timeline)
                {
                    count = SettingCommon.Instance.FirstCountApi;
                }
            }
            try
            {
                if (gType == MyCommon.WORKERTYPE.Timeline)
                {
                    if (more)
                    {
                        res = twCon.HomeTimeline(count, this.minHomeTimeline, null, ref content);
                    }
                    else
                    {
                        res = twCon.HomeTimeline(count, null, null, ref content);
                    }
                }
                else
                {
                    if (more)
                    {
                        res = twCon.Mentions(count, this.minMentions, null, ref content);
                    }
                    else
                    {
                        res = twCon.Mentions(count, null, null, ref content);
                    }
                }
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

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

            HttpStatusCode res;
            var content = "";

            if (count == 0) count = 20;
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    var target = tab.User;
                    if (string.IsNullOrEmpty(target)) return "";
                    userName = target;
                    res = twCon.UserTimeline(null, target, count, null, null, ref content);
                }
                else
                {
                    if (more)
                    {
                        res = twCon.UserTimeline(null, userName, count, tab.OldestId, null, ref content);
                    }
                    else
                    {
                        res = twCon.UserTimeline(null, userName, count, null, null, ref content);
                    }
                }
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            if (res == HttpStatusCode.Unauthorized)
                return "Err:@" + userName + "'s Tweets are protected.";

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            TwitterStatus[] items;
            try
            {
                items = TwitterStatus.ParseJsonArray(content);
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

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.ShowStatuses(id, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            if (res == HttpStatusCode.Forbidden)
                return "Err:protected user's tweet";

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            TwitterStatus status;
            try
            {
                status = TwitterStatus.ParseJson(content);
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

        private PostClass CreatePostsFromStatusData(TwitterStatus status)
        {
            var post = new PostClass();
            TwitterEntities entities;
            string sourceHtml;

            post.StatusId = status.Id;
            if (status.RetweetedStatus != null)
            {
                var retweeted = status.RetweetedStatus;

                post.CreatedAt = MyCommon.DateTimeParse(retweeted.CreatedAt);

                //Id
                post.RetweetedId = retweeted.Id;
                //本文
                post.TextFromApi = retweeted.Text;
                entities = retweeted.MergedEntities;
                sourceHtml = retweeted.Source;
                //Reply先
                post.InReplyToStatusId = retweeted.InReplyToStatusId;
                post.InReplyToUser = retweeted.InReplyToScreenName;
                post.InReplyToUserId = status.InReplyToUserId;

                //幻覚fav対策
                var tc = TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites);
                post.IsFav = tc.Contains(retweeted.Id);

                if (retweeted.Coordinates != null) post.PostGeo = new PostClass.StatusGeo { Lng = retweeted.Coordinates.Coordinates[0], Lat = retweeted.Coordinates.Coordinates[1] };

                //以下、ユーザー情報
                var user = retweeted.User;

                if (user == null || user.ScreenName == null || status.User.ScreenName == null) return null;

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
                entities = status.MergedEntities;
                sourceHtml = status.Source;
                post.InReplyToStatusId = status.InReplyToStatusId;
                post.InReplyToUser = status.InReplyToScreenName;
                post.InReplyToUserId = status.InReplyToUserId;

                if (status.Coordinates != null) post.PostGeo = new PostClass.StatusGeo { Lng = status.Coordinates.Coordinates[0], Lat = status.Coordinates.Coordinates[1] };

                //以下、ユーザー情報
                var user = status.User;

                if (user == null || user.ScreenName == null) return null;

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
            post.Text = CreateHtmlAnchor(textFromApi, post.ReplyToList, entities, post.Media);
            post.TextFromApi = textFromApi;
            post.TextFromApi = this.ReplaceTextFromApi(post.TextFromApi, entities);
            post.TextFromApi = WebUtility.HtmlDecode(post.TextFromApi);
            post.TextFromApi = post.TextFromApi.Replace("<3", "\u2661");

            post.QuoteStatusIds = GetQuoteTweetStatusIds(entities)
                .Where(x => x != post.StatusId && x != post.RetweetedId)
                .Distinct().ToArray();

            //Source整形
            var source = ParseSource(sourceHtml);
            post.Source = source.Item1;
            post.SourceUri = source.Item2;

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

        /// <summary>
        /// ツイートに含まれる引用ツイートのURLからステータスIDを抽出
        /// </summary>
        public static IEnumerable<long> GetQuoteTweetStatusIds(IEnumerable<TwitterEntity> entities)
        {
            foreach (var entity in entities)
            {
                var entityUrl = entity as TwitterEntityUrl;
                if (entityUrl == null)
                    continue;

                var match = Twitter.StatusUrlRegex.Match(entityUrl.ExpandedUrl);
                if (match.Success)
                {
                    yield return long.Parse(match.Groups["StatusId"].Value);
                }
            }
        }

        private string CreatePostsFromJson(string content, MyCommon.WORKERTYPE gType, TabClass tab, bool read, int count, ref long minimumId)
        {
            TwitterStatus[] items;
            try
            {
                items = TwitterStatus.ParseJsonArray(content);
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
                        if (tab.Contains(post.StatusId)) continue;
                    }
                }

                //RT禁止ユーザーによるもの
                if (post.RetweetedByUserId != null && this.noRTId.Contains(post.RetweetedByUserId.Value)) continue;

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
            TwitterSearchResult items;
            try
            {
                items = TwitterSearchResult.ParseJson(content);
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
            foreach (var result in items.Statuses)
            {
                PostClass post = null;
                post = CreatePostsFromStatusData(result);

                if (post == null)
                {
                    // Search API は相変わらずぶっ壊れたデータを返すことがあるため、必要なデータが欠如しているものは取得し直す
                    var ret = this.GetStatusApi(read, result.Id, ref post);
                    if (!string.IsNullOrEmpty(ret)) continue;
                }

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
                        if (tab.Contains(post.StatusId)) continue;
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

        public string GetListStatus(bool read,
                                TabClass tab,
                                bool more,
                                bool startup)
        {
            if (MyCommon._endingFlag) return "";

            HttpStatusCode res;
            var content = "";
            int count;
            if (SettingCommon.Instance.UseAdditionalCount)
            {
                count = SettingCommon.Instance.ListCountApi;
                if (count == 0)
                {
                    if (more && SettingCommon.Instance.MoreCountApi != 0)
                    {
                        count = SettingCommon.Instance.MoreCountApi;
                    }
                    else if (startup && SettingCommon.Instance.FirstCountApi != 0)
                    {
                        count = SettingCommon.Instance.FirstCountApi;
                    }
                    else
                    {
                        count = SettingCommon.Instance.CountApi;
                    }
                }
            }
            else
            {
                count = SettingCommon.Instance.CountApi;
            }
            try
            {
                if (more)
                {
                    res = twCon.GetListsStatuses(tab.ListInfo.UserId, tab.ListInfo.Id, count, tab.OldestId, null, SettingCommon.Instance.IsListsIncludeRts, ref content);
                }
                else
                {
                    res = twCon.GetListsStatuses(tab.ListInfo.UserId, tab.ListInfo.Id, count, null, null, SettingCommon.Instance.IsListsIncludeRts, ref content);
                }
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

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
            while (nextPost.InReplyToStatusId != null)
            {
                if (!posts.ContainsKey(nextPost.InReplyToStatusId.Value))
                    break;
                nextPost = posts[nextPost.InReplyToStatusId.Value];
            }

            return nextPost;
        }

        public string GetRelatedResult(bool read, TabClass tab)
        {
            var rslt = "";
            var relPosts = new Dictionary<Int64, PostClass>();
            if (tab.RelationTargetPost.TextFromApi.Contains("@") && tab.RelationTargetPost.InReplyToStatusId == null)
            {
                //検索結果対応
                var p = TabInformations.GetInstance()[tab.RelationTargetPost.StatusId];
                if (p != null && p.InReplyToStatusId != null)
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

            // in_reply_to_status_id を使用してリプライチェインを辿る
            var nextPost = FindTopOfReplyChain(relPosts, tab.RelationTargetPost.StatusId);
            var loopCount = 1;
            while (nextPost.InReplyToStatusId != null && loopCount++ <= 20)
            {
                var inReplyToId = nextPost.InReplyToStatusId.Value;

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

        public string GetSearch(bool read,
                            TabClass tab,
                            bool more)
        {
            if (MyCommon._endingFlag) return "";

            HttpStatusCode res;
            var content = "";
            long? maxId = null;
            long? sinceId = null;
            var count = 100;
            if (SettingCommon.Instance.UseAdditionalCount &&
                SettingCommon.Instance.SearchCountApi != 0)
            {
                count = SettingCommon.Instance.SearchCountApi;
            }
            else
            {
                count = SettingCommon.Instance.CountApi;
            }
            if (more)
            {
                maxId = tab.OldestId - 1;
            }
            else
            {
                sinceId = tab.SinceId;
            }

            try
            {
                // TODO:一時的に40>100件に 件数変更UI作成の必要あり
                res = twCon.Search(tab.SearchWords, tab.SearchLang, count, maxId, sinceId, ref content);
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

        private string CreateDirectMessagesFromJson(string content, MyCommon.WORKERTYPE gType, bool read)
        {
            TwitterDirectMessage[] item;
            try
            {
                if (gType == MyCommon.WORKERTYPE.UserStream)
                {
                    item = new[] { TwitterStreamEventDirectMessage.ParseJson(content).DirectMessage };
                }
                else
                {
                    item = TwitterDirectMessage.ParseJsonArray(content);
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
                    var textFromApi = message.Text;
                    //HTMLに整形
                    post.Text = CreateHtmlAnchor(textFromApi, post.ReplyToList, message.Entities, post.Media);
                    post.TextFromApi = this.ReplaceTextFromApi(textFromApi, message.Entities);
                    post.TextFromApi = WebUtility.HtmlDecode(post.TextFromApi);
                    post.TextFromApi = post.TextFromApi.Replace("<3", "\u2661");
                    post.IsFav = false;

                    post.QuoteStatusIds = GetQuoteTweetStatusIds(message.Entities).Distinct().ToArray();

                    //以下、ユーザー情報
                    TwitterUser user;
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

            if (this.AccessLevel == TwitterApiAccessLevel.Read || this.AccessLevel == TwitterApiAccessLevel.ReadWrite)
            {
                return "Auth Err:try to re-authorization.";
            }

            HttpStatusCode res;
            var content = "";

            try
            {
                if (gType == MyCommon.WORKERTYPE.DirectMessegeRcv)
                {
                    if (more)
                    {
                        res = twCon.DirectMessages(20, minDirectmessage, null, ref content);
                    }
                    else
                    {
                        res = twCon.DirectMessages(20, null, null, ref content);
                    }
                }
                else
                {
                    if (more)
                    {
                        res = twCon.DirectMessagesSent(20, minDirectmessageSent, null, ref content);
                    }
                    else
                    {
                        res = twCon.DirectMessagesSent(20, null, null, ref content);
                    }
                }
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            return CreateDirectMessagesFromJson(content, gType, read);
        }

        static int page_ = 1;
        public string GetFavoritesApi(bool read,
                            MyCommon.WORKERTYPE gType,
                            bool more)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            if (MyCommon._endingFlag) return "";

            var count = SettingCommon.Instance.CountApi;
            if (SettingCommon.Instance.UseAdditionalCount &&
                SettingCommon.Instance.FavoritesCountApi != 0)
            {
                count = SettingCommon.Instance.FavoritesCountApi;
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

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.Favorites(count, page_, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            TwitterStatus[] item;
            try
            {
                item = TwitterStatus.ParseJsonArray(content);
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
                TwitterEntities entities;
                string sourceHtml;

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
                        entities = retweeted.MergedEntities;
                        sourceHtml = retweeted.Source;
                        //Reply先
                        post.InReplyToStatusId = retweeted.InReplyToStatusId;
                        post.InReplyToUser = retweeted.InReplyToScreenName;
                        post.InReplyToUserId = retweeted.InReplyToUserId;
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
                        entities = status.MergedEntities;
                        sourceHtml = status.Source;
                        post.InReplyToStatusId = status.InReplyToStatusId;
                        post.InReplyToUser = status.InReplyToScreenName;
                        post.InReplyToUserId = status.InReplyToUserId;

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
                    post.Text = CreateHtmlAnchor(textFromApi, post.ReplyToList, entities, post.Media);
                    post.TextFromApi = textFromApi;
                    post.TextFromApi = this.ReplaceTextFromApi(post.TextFromApi, entities);
                    post.TextFromApi = WebUtility.HtmlDecode(post.TextFromApi);
                    post.TextFromApi = post.TextFromApi.Replace("<3", "\u2661");

                    post.QuoteStatusIds = GetQuoteTweetStatusIds(entities)
                        .Where(x => x != post.StatusId && x != post.RetweetedId)
                        .Distinct().ToArray();

                    //Source整形
                    var source = ParseSource(sourceHtml);
                    post.Source = source.Item1;
                    post.SourceUri = source.Item2;

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

        private string ReplaceTextFromApi(string text, TwitterEntities entities)
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

        /// <summary>
        /// フォロワーIDを更新します
        /// </summary>
        /// <exception cref="WebApiException"/>
        public void RefreshFollowerIds()
        {
            if (MyCommon._endingFlag) return;

            var cursor = -1L;
            var newFollowerIds = new HashSet<long>();
            do
            {
                var ret = this.GetFollowerIdsApi(ref cursor);
                newFollowerIds.UnionWith(ret.Ids);
                cursor = ret.NextCursor;
            } while (cursor != 0);

            this.followerId = newFollowerIds;
            TabInformations.GetInstance().RefreshOwl(this.followerId);

            this._GetFollowerResult = true;
        }

        public bool GetFollowersSuccess
        {
            get
            {
                return _GetFollowerResult;
            }
        }

        private TwitterIds GetFollowerIdsApi(ref long cursor)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid)
                throw new WebApiException("AccountState invalid");

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.FollowerIds(cursor, ref content);
            }
            catch(Exception e)
            {
                throw new WebApiException("Err:" + e.Message + "(" + MethodBase.GetCurrentMethod().Name + ")", e);
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null)
                throw new WebApiException(err, content);

            try
            {
                var ret = TwitterIds.ParseJson(content);

                if (ret.Ids == null)
                {
                    var ex = new WebApiException("Err: ret.id == null (GetFollowerIdsApi)", content);
                    MyCommon.ExceptionOut(ex);
                    throw ex;
                }

                return ret;
            }
            catch(SerializationException e)
            {
                var ex = new WebApiException("Err:Json Parse Error(DataContractJsonSerializer)", content, e);
                MyCommon.TraceOut(ex);
                throw ex;
            }
            catch(Exception e)
            {
                var ex = new WebApiException("Err:Invalid Json!", content, e);
                MyCommon.TraceOut(ex);
                throw ex;
            }
        }

        /// <summary>
        /// RT 非表示ユーザーを更新します
        /// </summary>
        /// <exception cref="WebApiException"/>
        public void RefreshNoRetweetIds()
        {
            if (MyCommon._endingFlag) return;

            this.noRTId = this.NoRetweetIdsApi();

            this._GetNoRetweetResult = true;
        }

        private long[] NoRetweetIdsApi()
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid)
                throw new WebApiException("AccountState invalid");

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.NoRetweetIds(ref content);
            }
            catch(Exception e)
            {
                throw new WebApiException("Err:" + e.Message + "(" + MethodBase.GetCurrentMethod().Name + ")", e);
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null)
                throw new WebApiException(err, content);

            try
            {
                return MyCommon.CreateDataFromJson<long[]>(content);
            }
            catch(SerializationException e)
            {
                var ex = new WebApiException("Err:Json Parse Error(DataContractJsonSerializer)", content, e);
                MyCommon.TraceOut(ex);
                throw ex;
            }
            catch(Exception e)
            {
                var ex = new WebApiException("Err:Invalid Json!", content, e);
                MyCommon.TraceOut(ex);
                throw ex;
            }
        }

        public bool GetNoRetweetSuccess
        {
            get
            {
                return _GetNoRetweetResult;
            }
        }

        /// <summary>
        /// t.co の文字列長などの設定情報を更新します
        /// </summary>
        /// <exception cref="WebApiException"/>
        public void RefreshConfiguration()
        {
            this.Configuration = this.ConfigurationApi();
        }

        private TwitterConfiguration ConfigurationApi()
        {
            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.GetConfiguration(ref content);
            }
            catch(Exception e)
            {
                throw new WebApiException("Err:" + e.Message + "(" + MethodBase.GetCurrentMethod().Name + ")", e);
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null)
                throw new WebApiException(err, content);

            try
            {
                return TwitterConfiguration.ParseJson(content);
            }
            catch(SerializationException e)
            {
                var ex = new WebApiException("Err:Json Parse Error(DataContractJsonSerializer)", content, e);
                MyCommon.TraceOut(ex);
                throw ex;
            }
            catch(Exception e)
            {
                var ex = new WebApiException("Err:Invalid Json!", content, e);
                MyCommon.TraceOut(ex);
                throw ex;
            }
        }

        public string GetListsApi()
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return "";

            HttpStatusCode res;
            IEnumerable<ListElement> lists;
            var content = "";

            try
            {
                res = twCon.GetLists(this.Username, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            try
            {
                lists = TwitterList.ParseJsonArray(content)
                    .Select(x => new ListElement(x, this));
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

            try
            {
                res = twCon.GetListsSubscriptions(this.Username, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            try
            {
                lists = lists.Concat(TwitterList.ParseJsonArray(content)
                    .Select(x => new ListElement(x, this)));
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

            TabInformations.GetInstance().SubscribableLists = lists.ToList();
            return "";
        }

        public string DeleteList(string list_id)
        {
            HttpStatusCode res;
            var content = "";

            try
            {
                res = twCon.DeleteListID(this.Username, list_id, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            return "";
        }

        public string EditList(string list_id, string new_name, bool isPrivate, string description, ref ListElement list)
        {
            HttpStatusCode res;
            var content = "";

            try
            {
                res = twCon.UpdateListID(this.Username, list_id, new_name, isPrivate, description, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            try
            {
                var le = TwitterList.ParseJson(content);
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

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.GetListMembers(this.Username, list_id, cursor, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message;
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            try
            {
                var users = TwitterUsers.ParseJson(content);
                Array.ForEach<TwitterUser>(
                    users.Users,
                    u => lists.Add(new UserInfo(u)));
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

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.CreateLists(listName, isPrivate, description, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            try
            {
                var le = TwitterList.ParseJson(content);
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

            HttpStatusCode res;
            var content = "";

            try
            {
                res = this.twCon.ShowListMember(listId, user, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            if (res == HttpStatusCode.NotFound)
            {
                value = false;
                return "";
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            try
            {
                TwitterUser.ParseJson(content);
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
            HttpStatusCode res;
            var content = "";

            try
            {
                res = twCon.CreateListMembers(listId, user, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

            return "";
        }

        public string RemoveUserToList(string listId, string user)
        {
            HttpStatusCode res;
            var content = "";

            try
            {
                res = twCon.DeleteListMembers(listId, user, ref content);
            }
            catch(Exception ex)
            {
                return "Err:" + ex.Message + "(" + MethodBase.GetCurrentMethod().Name + ")";
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return err;

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
        public async Task<string> CreateHtmlAnchorAsync(string Text, List<string> AtList, Dictionary<string, string> media)
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
            retStr = await new Regex(rgUrl, RegexOptions.IgnoreCase).ReplaceAsync(retStr, async mu =>
            {
                var sb = new StringBuilder(mu.Result("${before}<a href=\""));
                //if (mu.Result("${protocol}").StartsWith("w", StringComparison.OrdinalIgnoreCase))
                //    sb.Append("http://");
                //}
                var url = mu.Result("${url}");
                var title = await ShortUrl.Instance.ExpandUrlAsync(url);
                sb.Append(url + "\" title=\"" + MyCommon.ConvertToReadableUrl(title) + "\">").Append(url).Append("</a>");
                if (media != null && !media.ContainsKey(url)) media.Add(url, title);
                return sb.ToString();
            });

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
                                                          return mh.Result("$1") + "<a href=\"https://twitter.com/search?q=%23" + mh.Result("$3") + "\">" + mh.Result("$2$3") + "</a>";
                                                      }),
                                                  RegexOptions.IgnoreCase);


            retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/&#＃@＠>=.~])(sm|nm)([0-9]{1,10})", "$1<a href=\"http://www.nicovideo.jp/watch/$2$3\">$2$3</a>");

            retStr = retStr.Replace("<<<<<tweenだいなり>>>>>", "&gt;").Replace("<<<<<tweenしょうなり>>>>>", "&lt;");

            //retStr = AdjustHtml(ShortUrl.Resolve(PreProcessUrl(retStr), true)) //IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
            retStr = AdjustHtml(PreProcessUrl(retStr)); //IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
            return retStr;
        }

        public async Task<string> CreateHtmlAnchorAsync(string text, List<string> AtList, TwitterEntities entities, List<MediaInfo> media)
        {
            if (entities != null)
            {
                if (entities.Urls != null)
                {
                    foreach (var ent in entities.Urls)
                    {
                        ent.ExpandedUrl = await ShortUrl.Instance.ExpandUrlAsync(ent.ExpandedUrl)
                            .ConfigureAwait(false);

                        if (media != null && !media.Any(info => info.Url == ent.ExpandedUrl))
                            media.Add(new MediaInfo(ent.ExpandedUrl));
                    }
                }
                if (entities.Hashtags != null)
                {
                    lock (this.LockObj)
                    {
                        this._hashList.AddRange(entities.Hashtags.Select(x => "#" + x.Text));
                    }
                }
                if (entities.UserMentions != null)
                {
                    foreach (var ent in entities.UserMentions)
                    {
                        var screenName = ent.ScreenName.ToLower();
                        if (!AtList.Contains(screenName))
                            AtList.Add(screenName);
                    }
                }
                if (entities.Media != null)
                {
                    if (media != null)
                    {
                        foreach (var ent in entities.Media)
                        {
                            if (!media.Any(x => x.Url == ent.MediaUrl))
                            {
                                if (ent.VideoInfo != null &&
                                    ent.Type == "animated_gif" || ent.Type == "video")
                                {
                                    //var videoUrl = ent.VideoInfo.Variants
                                    //    .Where(v => v.ContentType == "video/mp4")
                                    //    .OrderByDescending(v => v.Bitrate)
                                    //    .Select(v => v.Url).FirstOrDefault();
                                    media.Add(new MediaInfo(ent.MediaUrl, ent.ExpandedUrl));
                                }
                                else
                                    media.Add(new MediaInfo(ent.MediaUrl));
                            }
                        }
                    }
                }
            }

            text = TweetFormatter.AutoLinkHtml(text, entities);

            text = Regex.Replace(text, "(^|[^a-zA-Z0-9_/&#＃@＠>=.~])(sm|nm)([0-9]{1,10})", "$1<a href=\"http://www.nicovideo.jp/watch/$2$3\">$2$3</a>");
            text = PreProcessUrl(text); //IDN置換

            return text;
        }

        [Obsolete]
        public string CreateHtmlAnchor(string text, List<string> AtList, TwitterEntities entities, List<MediaInfo> media)
        {
            return this.CreateHtmlAnchorAsync(text, AtList, entities, media).Result;
        }

        /// <summary>
        /// Twitter APIから得たHTML形式のsource文字列を分析し、source名とURLに分離します
        /// </summary>
        public static Tuple<string, Uri> ParseSource(string sourceHtml)
        {
            if (string.IsNullOrEmpty(sourceHtml))
                return Tuple.Create<string, Uri>("", null);

            string sourceText;
            Uri sourceUri;

            // sourceHtmlの例: <a href="http://twitter.com" rel="nofollow">Twitter Web Client</a>

            var match = Regex.Match(sourceHtml, "^<a href=\"(?<uri>.+?)\".*?>(?<text>.+)</a>$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                sourceText = WebUtility.HtmlDecode(match.Groups["text"].Value);
                try
                {
                    var uriStr = WebUtility.HtmlDecode(match.Groups["uri"].Value);
                    sourceUri = new Uri(new Uri("https://twitter.com/"), uriStr);
                }
                catch (UriFormatException)
                {
                    sourceUri = null;
                }
            }
            else
            {
                sourceText = WebUtility.HtmlDecode(sourceHtml);
                sourceUri = null;
            }

            return Tuple.Create(sourceText, sourceUri);
        }

        public TwitterApiStatus GetInfoApi()
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return null;

            if (MyCommon._endingFlag) return null;

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.RateLimitStatus(ref content);
            }
            catch (Exception)
            {
                this.ResetApiStatus();
                return null;
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null) return null;

            try
            {
                MyCommon.TwitterApiInfo.UpdateFromJson(content);
                return MyCommon.TwitterApiInfo;
            }
            catch (Exception ex)
            {
                MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name + " " + content);
                MyCommon.TwitterApiInfo.Reset();
                return null;
            }
        }

        /// <summary>
        /// ブロック中のユーザーを更新します
        /// </summary>
        /// <exception cref="WebApiException"/>
        public void RefreshBlockIds()
        {
            if (MyCommon._endingFlag) return;

            var cursor = -1L;
            var newBlockIds = new HashSet<long>();
            do
            {
                var ret = this.GetBlockIdsApi(cursor);
                newBlockIds.UnionWith(ret.Ids);
                cursor = ret.NextCursor;
            } while (cursor != 0);

            newBlockIds.Remove(this.UserId); // 元のソースにあったので一応残しておく

            TabInformations.GetInstance().BlockIds = newBlockIds;
        }

        public TwitterIds GetBlockIdsApi(long cursor)
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid)
                throw new WebApiException("AccountState invalid");

            HttpStatusCode res;
            var content = "";
            try
            {
                res = twCon.GetBlockUserIds(ref content, cursor);
            }
            catch(Exception e)
            {
                throw new WebApiException("Err:" + e.Message + "(" + MethodBase.GetCurrentMethod().Name + ")", e);
            }

            var err = this.CheckStatusCode(res, content);
            if (err != null)
                throw new WebApiException(err, content);

            try
            {
                return TwitterIds.ParseJson(content);
            }
            catch(SerializationException e)
            {
                var ex = new WebApiException("Err:Json Parse Error(DataContractJsonSerializer)", content, e);
                MyCommon.TraceOut(ex);
                throw ex;
            }
            catch(Exception e)
            {
                var ex = new WebApiException("Err:Invalid Json!", content, e);
                MyCommon.TraceOut(ex);
                throw ex;
            }
        }

        /// <summary>
        /// ミュート中のユーザーIDを更新します
        /// </summary>
        /// <exception cref="WebApiException"/>
        public async Task RefreshMuteUserIdsAsync()
        {
            if (MyCommon._endingFlag) return;

            var ids = await TwitterIds.GetAllItemsAsync(this.GetMuteUserIdsApiAsync)
                .ConfigureAwait(false);

            TabInformations.GetInstance().MuteUserIds = new HashSet<long>(ids);
        }

        public async Task<TwitterIds> GetMuteUserIdsApiAsync(long cursor)
        {
            var content = "";

            try
            {
                var res = await Task.Run(() => twCon.GetMuteUserIds(ref content, cursor))
                    .ConfigureAwait(false);

                var err = this.CheckStatusCode(res, content);
                if (err != null)
                    throw new WebApiException(err, content);

                return TwitterIds.ParseJson(content);
            }
            catch (WebException ex)
            {
                var ex2 = new WebApiException("Err:" + ex.Message, ex);
                MyCommon.TraceOut(ex2);
                throw ex2;
            }
            catch (SerializationException ex)
            {
                var ex2 = new WebApiException("Err:Json Parse Error(DataContractJsonSerializer)", content, ex);
                MyCommon.TraceOut(ex2);
                throw ex2;
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

        private string CheckStatusCode(HttpStatusCode httpStatus, string responseText,
            [CallerMemberName] string callerMethodName = "")
        {
            if (httpStatus == HttpStatusCode.OK)
            {
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                return null;
            }

            if (string.IsNullOrWhiteSpace(responseText))
            {
                if (httpStatus == HttpStatusCode.Unauthorized)
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;

                return "Err:" + httpStatus + "(" + callerMethodName + ")";
            }

            try
            {
                var errors = TwitterError.ParseJson(responseText).Errors;
                if (errors == null || !errors.Any())
                {
                    return "Err:" + httpStatus + "(" + callerMethodName + ")";
                }

                foreach (var error in errors)
                {
                    if (error.Code == TwitterErrorCode.InvalidToken ||
                        error.Code == TwitterErrorCode.SuspendedAccount)
                    {
                        Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    }
                }

                return "Err:" + string.Join(",", errors.Select(x => x.ToString())) + "(" + callerMethodName + ")";
            }
            catch (SerializationException) { }

            return "Err:" + httpStatus + "(" + callerMethodName + ")";
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

        public event EventHandler NewPostFromStream;
        public event EventHandler UserStreamStarted;
        public event EventHandler UserStreamStopped;
        public event EventHandler<PostDeletedEventArgs> PostDeleted;
        public event EventHandler<UserStreamEventReceivedEventArgs> UserStreamEventReceived;
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

        private readonly IReadOnlyDictionary<string, MyCommon.EVENTTYPE> eventTable = new Dictionary<string, MyCommon.EVENTTYPE>
        {
            { "favorite", MyCommon.EVENTTYPE.Favorite },
            { "unfavorite", MyCommon.EVENTTYPE.Unfavorite },
            { "follow", MyCommon.EVENTTYPE.Follow },
            { "list_member_added", MyCommon.EVENTTYPE.ListMemberAdded },
            { "list_member_removed", MyCommon.EVENTTYPE.ListMemberRemoved },
            { "block", MyCommon.EVENTTYPE.Block },
            { "unblock", MyCommon.EVENTTYPE.Unblock },
            { "user_update", MyCommon.EVENTTYPE.UserUpdate },
            { "deleted", MyCommon.EVENTTYPE.Deleted },
            { "list_created", MyCommon.EVENTTYPE.ListCreated },
            { "list_destroyed", MyCommon.EVENTTYPE.ListDestroyed },
            { "list_updated", MyCommon.EVENTTYPE.ListUpdated },
            { "unfollow", MyCommon.EVENTTYPE.Unfollow },
            { "list_user_subscribed", MyCommon.EVENTTYPE.ListUserSubscribed },
            { "list_user_unsubscribed", MyCommon.EVENTTYPE.ListUserUnsubscribed },
            { "mute", MyCommon.EVENTTYPE.Mute },
            { "unmute", MyCommon.EVENTTYPE.Unmute },
        };

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

            if (line.First() != '{' || line.Last() != '}')
            {
                MyCommon.TraceOut("Invalid JSON (StatusArrived):" + Environment.NewLine + line);
                return;
            }

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

                            if (this.PostDeleted != null)
                                this.PostDeleted(this, new PostDeletedEventArgs(id));
                        }
                        else if (xElm.Element("delete").Element("status") != null &&
                            xElm.Element("delete").Element("status").Element("id") != null)
                        {
                            id = 0;
                            long.TryParse(xElm.Element("delete").Element("status").Element("id").Value, out id);

                            if (this.PostDeleted != null)
                                this.PostDeleted(this, new PostDeletedEventArgs(id));
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

                if (isDm)
                {
                    CreateDirectMessagesFromJson(line, MyCommon.WORKERTYPE.UserStream, false);
                }
                else
                {
                    long dummy = 0;
                    CreatePostsFromJson("[" + line + "]", MyCommon.WORKERTYPE.Timeline, null, false, 0, ref dummy);
                }
            }
            catch(NullReferenceException)
            {
                MyCommon.TraceOut("NullRef StatusArrived: " + line);
            }

            if (this.NewPostFromStream != null)
                this.NewPostFromStream(this, EventArgs.Empty);
        }

        private void CreateEventFromJson(string content)
        {
            TwitterStreamEvent eventData = null;
            try
            {
                eventData = TwitterStreamEvent.ParseJson(content);
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

            MyCommon.EVENTTYPE eventType;
            eventTable.TryGetValue(eventData.Event, out eventType);
            evt.Eventtype = eventType;

            switch (eventData.Event)
            {
                case "access_revoked":
                case "access_unrevoked":
                case "user_delete":
                case "user_suspend":
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
                    evt.Target = "@" + eventData.Target.ScreenName;
                    break;
                case "favorited_retweet":
                case "retweeted_retweet":
                    return;
                case "favorite":
                case "unfavorite":
                    var tweetEvent = TwitterStreamEvent<TwitterStatus>.ParseJson(content);
                    evt.Target = "@" + tweetEvent.TargetObject.User.ScreenName + ":" + WebUtility.HtmlDecode(tweetEvent.TargetObject.Text);
                    evt.Id = tweetEvent.TargetObject.Id;

                    if (SettingCommon.Instance.IsRemoveSameEvent)
                    {
                        if (this.StoredEvent.Any(ev => ev.Username == evt.Username && ev.Eventtype == evt.Eventtype && ev.Target == evt.Target))
                            return;
                    }

                    var tabinfo = TabInformations.GetInstance();

                    PostClass post;
                    var statusId = tweetEvent.TargetObject.Id;
                    if (!tabinfo.Posts.TryGetValue(statusId, out post))
                        break;

                    if (eventData.Event == "favorite")
                    {
                        var favTab = tabinfo.GetTabByType(MyCommon.TabUsageType.Favorites);
                        if (!favTab.Contains(post.StatusId))
                            favTab.Add(post.StatusId, post.IsRead, false);

                        if (tweetEvent.Source.Id == this.UserId)
                        {
                            post.IsFav = true;
                        }
                        else if (tweetEvent.Target.Id == this.UserId)
                        {
                            post.FavoritedCount++;

                            if (SettingCommon.Instance.FavEventUnread)
                                tabinfo.SetReadAllTab(post.StatusId, read: false);
                        }
                    }
                    else // unfavorite
                    {
                        if (tweetEvent.Source.Id == this.UserId)
                        {
                            post.IsFav = false;
                        }
                        else if (tweetEvent.Target.Id == this.UserId)
                        {
                            post.FavoritedCount = Math.Max(0, post.FavoritedCount - 1);
                        }
                    }
                    break;
                case "list_member_added":
                case "list_member_removed":
                case "list_created":
                case "list_destroyed":
                case "list_updated":
                case "list_user_subscribed":
                case "list_user_unsubscribed":
                    var listEvent = TwitterStreamEvent<TwitterList>.ParseJson(content);
                    evt.Target = listEvent.TargetObject.FullName;
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
                
                // Mute / Unmute
                case "mute":
                    evt.Target = "@" + eventData.Target.ScreenName;
                    if (!TabInformations.GetInstance().MuteUserIds.Contains(eventData.Target.Id))
                    {
                        TabInformations.GetInstance().MuteUserIds.Add(eventData.Target.Id);
                    }
                    break;
                case "unmute":
                    evt.Target = "@" + eventData.Target.ScreenName;
                    if (TabInformations.GetInstance().MuteUserIds.Contains(eventData.Target.Id))
                    {
                        TabInformations.GetInstance().MuteUserIds.Remove(eventData.Target.Id);
                    }
                    break;

                default:
                    MyCommon.TraceOut("Unknown Event:" + evt.Event + Environment.NewLine + content);
                    break;
            }
            this.StoredEvent.Insert(0, evt);

            if (this.UserStreamEventReceived != null)
                this.UserStreamEventReceived(this, new UserStreamEventReceivedEventArgs(evt));
        }

        private void userStream_Started()
        {
            if (this.UserStreamStarted != null)
                this.UserStreamStarted(this, EventArgs.Empty);
        }

        private void userStream_Stopped()
        {
            if (this.UserStreamStopped != null)
                this.UserStreamStopped(this, EventArgs.Empty);
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
                if (this.UserStreamStopped != null)
                    this.UserStreamStopped(this, EventArgs.Empty);
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
                        var res = twCon.UserStream(ref st, _allAtreplies, _trackwords, Networking.GetUserAgentString());

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
                        _streamActive = false;
                        if (_streamThread != null && _streamThread.IsAlive)
                        {
                            _streamThread.Abort();
                        }
                    }
                }
                this.disposedValue = true;
            }

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
                    this.StopUserStream();
                }
            }
            this.disposedValue = true;
        }

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

    public class PostDeletedEventArgs : EventArgs
    {
        public long StatusId
        {
            get { return this.statusId; }
        }

        private readonly long statusId;

        public PostDeletedEventArgs(long statusId)
        {
            this.statusId = statusId;
        }
    }

    public class UserStreamEventReceivedEventArgs : EventArgs
    {
        public Twitter.FormattedEvent EventData
        {
            get { return this.eventData; }
        }

        private readonly Twitter.FormattedEvent eventData;

        public UserStreamEventReceivedEventArgs(Twitter.FormattedEvent eventData)
        {
            this.eventData = eventData;
        }
    }
}
