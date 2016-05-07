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
using System.Net.Http;
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
using System.Xml.XPath;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTween.Api;
using OpenTween.Api.DataModel;
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
        public static readonly Regex DMSendTextRegex = new Regex(@"^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public TwitterApi Api { get; }
        public TwitterConfiguration Configuration { get; private set; }

        delegate void GetIconImageDelegate(PostClass post);
        private readonly object LockObj = new object();
        private ISet<long> followerId = new HashSet<long>();
        private bool _GetFollowerResult = false;
        private long[] noRTId = new long[0];
        private bool _GetNoRetweetResult = false;

        //プロパティからアクセスされる共通情報
        private string _uname;

        private bool _readOwnPost;
        private List<string> _hashList = new List<string>();

        //max_idで古い発言を取得するために保持（lists分は個別タブで管理）
        private long minHomeTimeline = long.MaxValue;
        private long minMentions = long.MaxValue;
        private long minDirectmessage = long.MaxValue;
        private long minDirectmessageSent = long.MaxValue;

        //private FavoriteQueue favQueue;

        //private List<PostClass> _deletemessages = new List<PostClass>();

        public Twitter() : this(new TwitterApi())
        {
        }

        public Twitter(TwitterApi api)
        {
            this.Api = api;
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

        public void ClearAuthInfo()
        {
            Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
            this.ResetApiStatus();
        }

        [Obsolete]
        public void VerifyCredentials()
        {
            try
            {
                this.VerifyCredentialsAsync().Wait();
            }
            catch (AggregateException ex) when (ex.InnerException is WebApiException)
            {
                throw new WebApiException(ex.InnerException.Message, ex);
            }
        }

        public async Task VerifyCredentialsAsync()
        {
            var user = await this.Api.AccountVerifyCredentials()
                .ConfigureAwait(false);

            this.UpdateUserStats(user);
        }

        public void Initialize(string token, string tokenSecret, string username, long userId)
        {
            //OAuth認証
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(tokenSecret) || string.IsNullOrEmpty(username))
            {
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
            }
            this.ResetApiStatus();
            this.Api.Initialize(token, tokenSecret, userId, username);
            _uname = username.ToLowerInvariant();
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

                    if (!urlStr.StartsWith("http://", StringComparison.Ordinal)
                        && !urlStr.StartsWith("https://", StringComparison.Ordinal)
                        && !urlStr.StartsWith("ftp://", StringComparison.Ordinal))
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

        public async Task PostStatus(string postStr, long? reply_to, IReadOnlyList<long> mediaIds = null)
        {
            this.CheckAccountState();

            if (mediaIds == null &&
                Twitter.DMSendTextRegex.IsMatch(postStr))
            {
                await this.SendDirectMessage(postStr)
                    .ConfigureAwait(false);
                return;
            }

            var response = await this.Api.StatusesUpdate(postStr, reply_to, mediaIds)
                .ConfigureAwait(false);

            var status = await response.LoadJsonAsync()
                .ConfigureAwait(false);

            this.UpdateUserStats(status.User);

            if (IsPostRestricted(status))
            {
                throw new WebApiException("OK:Delaying?");
            }
        }

        public async Task PostStatusWithMultipleMedia(string postStr, long? reply_to, IMediaItem[] mediaItems)
        {
            this.CheckAccountState();

            if (Twitter.DMSendTextRegex.IsMatch(postStr))
            {
                await this.SendDirectMessage(postStr)
                    .ConfigureAwait(false);
                return;
            }

            if (mediaItems.Length == 0)
                throw new WebApiException("Err:Invalid Files!");

            var uploadTasks = from m in mediaItems
                              select this.UploadMedia(m);

            var mediaIds = await Task.WhenAll(uploadTasks)
                .ConfigureAwait(false);

            await this.PostStatus(postStr, reply_to, mediaIds)
                .ConfigureAwait(false);
        }

        public async Task<long> UploadMedia(IMediaItem item)
        {
            this.CheckAccountState();

            var response = await this.Api.MediaUpload(item)
                .ConfigureAwait(false);

            var media = await response.LoadJsonAsync()
                .ConfigureAwait(false);

            return media.MediaId;
        }

        public async Task SendDirectMessage(string postStr)
        {
            this.CheckAccountState();
            this.CheckAccessLevel(TwitterApiAccessLevel.ReadWriteAndDirectMessage);

            var mc = Twitter.DMSendTextRegex.Match(postStr);

            var response = await this.Api.DirectMessagesNew(mc.Groups["body"].Value, mc.Groups["id"].Value)
                .ConfigureAwait(false);

            var dm = await response.LoadJsonAsync()
                .ConfigureAwait(false);

            this.UpdateUserStats(dm.Sender);
        }

        public async Task PostRetweet(long id, bool read)
        {
            this.CheckAccountState();

            //データ部分の生成
            var target = id;
            var post = TabInformations.GetInstance()[id];
            if (post == null)
            {
                throw new WebApiException("Err:Target isn't found.");
            }
            if (TabInformations.GetInstance()[id].RetweetedId != null)
            {
                target = TabInformations.GetInstance()[id].RetweetedId.Value; //再RTの場合は元発言をRT
            }

            var response = await this.Api.StatusesRetweet(target)
                .ConfigureAwait(false);

            var status = await response.LoadJsonAsync()
                .ConfigureAwait(false);

            //ReTweetしたものをTLに追加
            post = CreatePostsFromStatusData(status);
            if (post == null)
                throw new WebApiException("Invalid Json!");

            //二重取得回避
            lock (LockObj)
            {
                if (TabInformations.GetInstance().ContainsKey(post.StatusId))
                    return;
            }
            //Retweet判定
            if (post.RetweetedId == null)
                throw new WebApiException("Invalid Json!");
            //ユーザー情報
            post.IsMe = true;

            post.IsRead = read;
            post.IsOwl = false;
            if (_readOwnPost) post.IsRead = true;
            post.IsDm = false;

            TabInformations.GetInstance().AddPost(post);
        }

        public string Username
            => this.Api.CurrentScreenName;

        public long UserId
            => this.Api.CurrentUserId;

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

        public bool RestrictFavCheck { get; set; }

#region "バージョンアップ"
        public void GetTweenBinary(string strVer)
        {
            try
            {
                //本体
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/Tween" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(),
                                                    Path.Combine(MyCommon.settingPath, "TweenNew.exe")))
                {
                    throw new WebApiException("Err:Download failed");
                }
                //英語リソース
                if (!Directory.Exists(Path.Combine(MyCommon.settingPath, "en")))
                {
                    Directory.CreateDirectory(Path.Combine(MyCommon.settingPath, "en"));
                }
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenResEn" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(),
                                                    Path.Combine(Path.Combine(MyCommon.settingPath, "en"), "Tween.resourcesNew.dll")))
                {
                    throw new WebApiException("Err:Download failed");
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
                    throw new WebApiException("Err:Download failed");
                }
                //シリアライザDLL
                if (!(new HttpVarious()).GetDataToFile("http://tween.sourceforge.jp/TweenDll" + strVer + ".gz?" + DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(),
                                                    Path.Combine(MyCommon.settingPath, "TweenNew.XmlSerializers.dll")))
                {
                    throw new WebApiException("Err:Download failed");
                }
            }
            catch (Exception ex)
            {
                throw new WebApiException("Err:Download failed", ex);
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

        public int FollowersCount { get; private set; }
        public int FriendsCount { get; private set; }
        public int StatusesCount { get; private set; }
        public string Location { get; private set; } = "";
        public string Bio { get; private set; } = "";

        /// <summary>ユーザーのフォロワー数などの情報を更新します</summary>
        private void UpdateUserStats(TwitterUser self)
        {
            this.FollowersCount = self.FollowersCount;
            this.FriendsCount = self.FriendsCount;
            this.StatusesCount = self.StatusesCount;
            this.Location = self.Location;
            this.Bio = self.Description;
        }

        /// <summary>
        /// 渡された取得件数がWORKERTYPEに応じた取得可能範囲に収まっているか検証する
        /// </summary>
        public static bool VerifyApiResultCount(MyCommon.WORKERTYPE type, int count)
        {
            return count >= 20 && count <= GetMaxApiResultCount(type);
        }

        /// <summary>
        /// 渡された取得件数が更新時の取得可能範囲に収まっているか検証する
        /// </summary>
        public static bool VerifyMoreApiResultCount(int count)
        {
            return count >= 20 && count <= 200;
        }

        /// <summary>
        /// 渡された取得件数が起動時の取得可能範囲に収まっているか検証する
        /// </summary>
        public static bool VerifyFirstApiResultCount(int count)
        {
            return count >= 20 && count <= 200;
        }

        /// <summary>
        /// WORKERTYPEに応じた取得可能な最大件数を取得する
        /// </summary>
        public static int GetMaxApiResultCount(MyCommon.WORKERTYPE type)
        {
            // 参照: REST APIs - 各endpointのcountパラメータ
            // https://dev.twitter.com/rest/public
            switch (type)
            {
                case MyCommon.WORKERTYPE.Timeline:
                case MyCommon.WORKERTYPE.Reply:
                case MyCommon.WORKERTYPE.UserTimeline:
                case MyCommon.WORKERTYPE.Favorites:
                case MyCommon.WORKERTYPE.DirectMessegeRcv:
                case MyCommon.WORKERTYPE.DirectMessegeSnt:
                case MyCommon.WORKERTYPE.List:  // 不明
                    return 200;

                case MyCommon.WORKERTYPE.PublicSearch:
                    return 100;

                default:
                    throw new InvalidOperationException("Invalid type: " + type);
            }
        }

        /// <summary>
        /// WORKERTYPEに応じた取得件数を取得する
        /// </summary>
        public static int GetApiResultCount(MyCommon.WORKERTYPE type, bool more, bool startup)
        {
            if (type == MyCommon.WORKERTYPE.DirectMessegeRcv ||
                type == MyCommon.WORKERTYPE.DirectMessegeSnt)
            {
                return 20;
            }

            if (SettingCommon.Instance.UseAdditionalCount)
            {
                switch (type)
                {
                    case MyCommon.WORKERTYPE.Favorites:
                        if (SettingCommon.Instance.FavoritesCountApi != 0)
                            return SettingCommon.Instance.FavoritesCountApi;
                        break;
                    case MyCommon.WORKERTYPE.List:
                        if (SettingCommon.Instance.ListCountApi != 0)
                            return SettingCommon.Instance.ListCountApi;
                        break;
                    case MyCommon.WORKERTYPE.PublicSearch:
                        if (SettingCommon.Instance.SearchCountApi != 0)
                            return SettingCommon.Instance.SearchCountApi;
                        break;
                    case MyCommon.WORKERTYPE.UserTimeline:
                        if (SettingCommon.Instance.UserTimelineCountApi != 0)
                            return SettingCommon.Instance.UserTimelineCountApi;
                        break;
                }
                if (more && SettingCommon.Instance.MoreCountApi != 0)
                {
                    return Math.Min(SettingCommon.Instance.MoreCountApi, GetMaxApiResultCount(type));
                }
                if (startup && SettingCommon.Instance.FirstCountApi != 0 && type != MyCommon.WORKERTYPE.Reply)
                {
                    return Math.Min(SettingCommon.Instance.FirstCountApi, GetMaxApiResultCount(type));
                }
            }

            // 上記に当てはまらない場合の共通処理
            var count = SettingCommon.Instance.CountApi;

            if (type == MyCommon.WORKERTYPE.Reply)
                count = SettingCommon.Instance.CountApiReply;

            return Math.Min(count, GetMaxApiResultCount(type));
        }

        public async Task GetTimelineApi(bool read, MyCommon.WORKERTYPE gType, bool more, bool startup)
        {
            this.CheckAccountState();

            var count = GetApiResultCount(gType, more, startup);

            TwitterStatus[] statuses;
            if (gType == MyCommon.WORKERTYPE.Timeline)
            {
                if (more)
                {
                    statuses = await this.Api.StatusesHomeTimeline(count, maxId: this.minHomeTimeline)
                        .ConfigureAwait(false);
                }
                else
                {
                    statuses = await this.Api.StatusesHomeTimeline(count)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                if (more)
                {
                    statuses = await this.Api.StatusesMentionsTimeline(count, maxId: this.minMentions)
                        .ConfigureAwait(false);
                }
                else
                {
                    statuses = await this.Api.StatusesMentionsTimeline(count)
                        .ConfigureAwait(false);
                }
            }

            var minimumId = CreatePostsFromJson(statuses, gType, null, read);

            if (minimumId != null)
            {
                if (gType == MyCommon.WORKERTYPE.Timeline)
                    this.minHomeTimeline = minimumId.Value;
                else
                    this.minMentions = minimumId.Value;
            }
        }

        public async Task GetUserTimelineApi(bool read, string userName, TabClass tab, bool more)
        {
            this.CheckAccountState();

            var count = GetApiResultCount(MyCommon.WORKERTYPE.UserTimeline, more, false);

            TwitterStatus[] statuses;
            if (string.IsNullOrEmpty(userName))
            {
                var target = tab.User;
                if (string.IsNullOrEmpty(target)) return;
                userName = target;
                statuses = await this.Api.StatusesUserTimeline(userName, count)
                    .ConfigureAwait(false);
            }
            else
            {
                if (more)
                {
                    statuses = await this.Api.StatusesUserTimeline(userName, count, maxId: tab.OldestId)
                        .ConfigureAwait(false);
                }
                else
                {
                    statuses = await this.Api.StatusesUserTimeline(userName, count)
                        .ConfigureAwait(false);
                }
            }

            var minimumId = CreatePostsFromJson(statuses, MyCommon.WORKERTYPE.UserTimeline, tab, read);

            if (minimumId != null)
                tab.OldestId = minimumId.Value;
        }

        public async Task<PostClass> GetStatusApi(bool read, long id)
        {
            this.CheckAccountState();

            var status = await this.Api.StatusesShow(id)
                .ConfigureAwait(false);

            var item = CreatePostsFromStatusData(status);
            if (item == null)
                throw new WebApiException("Err:Can't create post");

            item.IsRead = read;
            if (item.IsMe && !read && _readOwnPost) item.IsRead = true;

            return item;
        }

        public async Task GetStatusApi(bool read, long id, TabClass tab)
        {
            var post = await this.GetStatusApi(read, id)
                .ConfigureAwait(false);

            //非同期アイコン取得＆StatusDictionaryに追加
            if (tab != null && tab.IsInnerStorageTabType)
                tab.AddPostToInnerStorage(post);
            else
                TabInformations.GetInstance().AddPost(post);
        }

        private PostClass CreatePostsFromStatusData(TwitterStatus status)
        {
            return CreatePostsFromStatusData(status, false);
        }

        private PostClass CreatePostsFromStatusData(TwitterStatus status, bool favTweet)
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

                if (favTweet)
                {
                    post.IsFav = true;
                }
                else
                {
                    //幻覚fav対策
                    var tc = TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites);
                    post.IsFav = tc.Contains(retweeted.Id);
                }

                if (retweeted.Coordinates != null)
                    post.PostGeo = new PostClass.StatusGeo(retweeted.Coordinates.Coordinates[0], retweeted.Coordinates.Coordinates[1]);

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
                post.IsMe = post.RetweetedBy.ToLowerInvariant().Equals(_uname);
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

                if (favTweet)
                {
                    post.IsFav = true;
                }
                else
                {
                    //幻覚fav対策
                    var tc = TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites);
                    post.IsFav = tc.Contains(post.StatusId) && TabInformations.GetInstance()[post.StatusId].IsFav;
                }

                if (status.Coordinates != null)
                    post.PostGeo = new PostClass.StatusGeo(status.Coordinates.Coordinates[0], status.Coordinates.Coordinates[1]);

                //以下、ユーザー情報
                var user = status.User;

                if (user == null || user.ScreenName == null) return null;

                post.UserId = user.Id;
                post.ScreenName = user.ScreenName;
                post.Nickname = user.Name.Trim();
                post.ImageUrl = user.ProfileImageUrlHttps;
                post.IsProtect = user.Protected;
                post.IsMe = post.ScreenName.ToLowerInvariant().Equals(_uname);
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

            post.ExpandedUrls = entities.OfType<TwitterEntityUrl>()
                .Select(x => new PostClass.ExpandedUrlInfo(x.Url, x.ExpandedUrl))
                .ToArray();

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
            var urls = entities.OfType<TwitterEntityUrl>().Select(x => x.ExpandedUrl);

            return GetQuoteTweetStatusIds(urls);
        }

        public static IEnumerable<long> GetQuoteTweetStatusIds(IEnumerable<string> urls)
        {
            foreach (var url in urls)
            {
                var match = Twitter.StatusUrlRegex.Match(url);
                if (match.Success)
                {
                    long statusId;
                    if (long.TryParse(match.Groups["StatusId"].Value, out statusId))
                        yield return statusId;
                }
            }
        }

        private long? CreatePostsFromJson(TwitterStatus[] items, MyCommon.WORKERTYPE gType, TabClass tab, bool read)
        {
            long? minimumId = null;

            foreach (var status in items)
            {
                PostClass post = null;
                post = CreatePostsFromStatusData(status);
                if (post == null) continue;

                if (minimumId == null || minimumId.Value > post.StatusId)
                    minimumId = post.StatusId;

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
                if (gType != MyCommon.WORKERTYPE.UserTimeline &&
                    post.RetweetedByUserId != null && this.noRTId.Contains(post.RetweetedByUserId.Value)) continue;

                post.IsRead = read;
                if (post.IsMe && !read && _readOwnPost) post.IsRead = true;

                //非同期アイコン取得＆StatusDictionaryに追加
                if (tab != null && tab.IsInnerStorageTabType)
                    tab.AddPostToInnerStorage(post);
                else
                    TabInformations.GetInstance().AddPost(post);
            }

            return minimumId;
        }

        private long? CreatePostsFromSearchJson(TwitterSearchResult items, TabClass tab, bool read, int count, bool more)
        {
            long? minimumId = null;

            foreach (var result in items.Statuses)
            {
                var post = CreatePostsFromStatusData(result);
                if (post == null)
                    continue;

                if (minimumId == null || minimumId.Value > post.StatusId)
                    minimumId = post.StatusId;

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

                //非同期アイコン取得＆StatusDictionaryに追加
                if (tab != null && tab.IsInnerStorageTabType)
                    tab.AddPostToInnerStorage(post);
                else
                    TabInformations.GetInstance().AddPost(post);
            }

            return minimumId;
        }

        private void CreateFavoritePostsFromJson(TwitterStatus[] item, bool read)
        {
            var favTab = TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.Favorites);

            foreach (var status in item)
            {
                //二重取得回避
                lock (LockObj)
                {
                    if (favTab.Contains(status.Id)) continue;
                }

                var post = CreatePostsFromStatusData(status, true);
                if (post == null) continue;

                post.IsRead = read;

                TabInformations.GetInstance().AddPost(post);
            }
        }

        public async Task GetListStatus(bool read, TabClass tab, bool more, bool startup)
        {
            var count = GetApiResultCount(MyCommon.WORKERTYPE.List, more, startup);

            TwitterStatus[] statuses;
            if (more)
            {
                statuses = await this.Api.ListsStatuses(tab.ListInfo.Id, count, maxId: tab.OldestId, includeRTs: SettingCommon.Instance.IsListsIncludeRts)
                    .ConfigureAwait(false);
            }
            else
            {
                statuses = await this.Api.ListsStatuses(tab.ListInfo.Id, count, includeRTs: SettingCommon.Instance.IsListsIncludeRts)
                    .ConfigureAwait(false);
            }

            var minimumId = CreatePostsFromJson(statuses, MyCommon.WORKERTYPE.List, tab, read);

            if (minimumId != null)
                tab.OldestId = minimumId.Value;
        }

        /// <summary>
        /// startStatusId からリプライ先の発言を辿る。発言は posts 以外からは検索しない。
        /// </summary>
        /// <returns>posts の中から検索されたリプライチェインの末端</returns>
        internal static PostClass FindTopOfReplyChain(IDictionary<Int64, PostClass> posts, Int64 startStatusId)
        {
            if (!posts.ContainsKey(startStatusId))
                throw new ArgumentException("startStatusId (" + startStatusId + ") が posts の中から見つかりませんでした。", nameof(startStatusId));

            var nextPost = posts[startStatusId];
            while (nextPost.InReplyToStatusId != null)
            {
                if (!posts.ContainsKey(nextPost.InReplyToStatusId.Value))
                    break;
                nextPost = posts[nextPost.InReplyToStatusId.Value];
            }

            return nextPost;
        }

        public async Task GetRelatedResult(bool read, TabClass tab)
        {
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
                    p = await this.GetStatusApi(read, tab.RelationTargetPost.StatusId)
                        .ConfigureAwait(false);
                    tab.RelationTargetPost = p;
                }
            }
            relPosts.Add(tab.RelationTargetPost.StatusId, tab.RelationTargetPost);

            Exception lastException = null;

            // in_reply_to_status_id を使用してリプライチェインを辿る
            var nextPost = FindTopOfReplyChain(relPosts, tab.RelationTargetPost.StatusId);
            var loopCount = 1;
            while (nextPost.InReplyToStatusId != null && loopCount++ <= 20)
            {
                var inReplyToId = nextPost.InReplyToStatusId.Value;

                var inReplyToPost = TabInformations.GetInstance()[inReplyToId];
                if (inReplyToPost == null)
                {
                    try
                    {
                        inReplyToPost = await this.GetStatusApi(read, inReplyToId)
                            .ConfigureAwait(false);
                    }
                    catch (WebApiException ex)
                    {
                        lastException = ex;
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

                    var p = TabInformations.GetInstance()[_statusId];
                    if (p == null)
                    {
                        try
                        {
                            p = await this.GetStatusApi(read, _statusId)
                                .ConfigureAwait(false);
                        }
                        catch (WebApiException ex)
                        {
                            lastException = ex;
                            break;
                        }
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

                tab.AddPostToInnerStorage(p);
            });

            if (lastException != null)
                throw new WebApiException(lastException.Message, lastException);
        }

        public async Task GetSearch(bool read, TabClass tab, bool more)
        {
            var count = GetApiResultCount(MyCommon.WORKERTYPE.PublicSearch, more, false);

            long? maxId = null;
            long? sinceId = null;
            if (more)
            {
                maxId = tab.OldestId - 1;
            }
            else
            {
                sinceId = tab.SinceId;
            }

            var searchResult = await this.Api.SearchTweets(tab.SearchWords, tab.SearchLang, count, maxId, sinceId)
                .ConfigureAwait(false);

            if (!TabInformations.GetInstance().ContainsTab(tab))
                return;

            var minimumId = this.CreatePostsFromSearchJson(searchResult, tab, read, count, more);

            if (minimumId != null)
                tab.OldestId = minimumId.Value;
        }

        private void CreateDirectMessagesFromJson(TwitterDirectMessage[] item, MyCommon.WORKERTYPE gType, bool read)
        {
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

                    post.ExpandedUrls = message.Entities.OfType<TwitterEntityUrl>()
                        .Select(x => new PostClass.ExpandedUrlInfo(x.Url, x.ExpandedUrl))
                        .ToArray();

                    //以下、ユーザー情報
                    TwitterUser user;
                    if (gType == MyCommon.WORKERTYPE.UserStream)
                    {
                        if (this.Api.CurrentUserId == message.Recipient.Id)
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
                    MyCommon.TraceOut(ex, MethodBase.GetCurrentMethod().Name);
                    MessageBox.Show("Parse Error(CreateDirectMessagesFromJson)");
                    continue;
                }

                post.IsRead = read;
                if (post.IsMe && !read && _readOwnPost) post.IsRead = true;
                post.IsReply = false;
                post.IsExcludeReply = false;
                post.IsDm = true;

                var dmTab = TabInformations.GetInstance().GetTabByType(MyCommon.TabUsageType.DirectMessage);
                dmTab.AddPostToInnerStorage(post);
            }
        }

        public async Task GetDirectMessageApi(bool read, MyCommon.WORKERTYPE gType, bool more)
        {
            this.CheckAccountState();
            this.CheckAccessLevel(TwitterApiAccessLevel.ReadWriteAndDirectMessage);

            var count = GetApiResultCount(gType, more, false);

            TwitterDirectMessage[] messages;
            if (gType == MyCommon.WORKERTYPE.DirectMessegeRcv)
            {
                if (more)
                {
                    messages = await this.Api.DirectMessagesRecv(count, maxId: this.minDirectmessage)
                        .ConfigureAwait(false);
                }
                else
                {
                    messages = await this.Api.DirectMessagesRecv(count)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                if (more)
                {
                    messages = await this.Api.DirectMessagesSent(count, maxId: this.minDirectmessageSent)
                        .ConfigureAwait(false);
                }
                else
                {
                    messages = await this.Api.DirectMessagesSent(count)
                        .ConfigureAwait(false);
                }
            }

            CreateDirectMessagesFromJson(messages, gType, read);
        }

        public async Task GetFavoritesApi(bool read, bool more)
        {
            this.CheckAccountState();

            var count = GetApiResultCount(MyCommon.WORKERTYPE.Favorites, more, false);

            var statuses = await this.Api.FavoritesList(count)
                .ConfigureAwait(false);

            CreateFavoritePostsFromJson(statuses, read);
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
                        if (m.AltText != null)
                        {
                            text = text.Replace(m.Url, string.Format(Properties.Resources.ImageAltText, m.AltText));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(m.DisplayUrl)) text = text.Replace(m.Url, m.DisplayUrl);
                        }
                    }
                }
            }
            return text;
        }

        /// <summary>
        /// フォロワーIDを更新します
        /// </summary>
        /// <exception cref="WebApiException"/>
        public async Task RefreshFollowerIds()
        {
            if (MyCommon._endingFlag) return;

            var cursor = -1L;
            var newFollowerIds = new HashSet<long>();
            do
            {
                var ret = await this.Api.FollowersIds(cursor)
                    .ConfigureAwait(false);

                if (ret.Ids == null)
                    throw new WebApiException("ret.ids == null");

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

        /// <summary>
        /// RT 非表示ユーザーを更新します
        /// </summary>
        /// <exception cref="WebApiException"/>
        public async Task RefreshNoRetweetIds()
        {
            if (MyCommon._endingFlag) return;

            this.noRTId = await this.Api.NoRetweetIds()
                .ConfigureAwait(false);

            this._GetNoRetweetResult = true;
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
        public async Task RefreshConfiguration()
        {
            this.Configuration = await this.Api.Configuration()
                .ConfigureAwait(false);
        }

        public async Task GetListsApi()
        {
            this.CheckAccountState();

            var ownedLists = await TwitterLists.GetAllItemsAsync(x => this.Api.ListsOwnerships(this.Username, cursor: x))
                .ConfigureAwait(false);

            var subscribedLists = await TwitterLists.GetAllItemsAsync(x => this.Api.ListsSubscriptions(this.Username, cursor: x))
                .ConfigureAwait(false);

            TabInformations.GetInstance().SubscribableLists = Enumerable.Concat(ownedLists, subscribedLists)
                .Select(x => new ListElement(x, this))
                .ToList();
        }

        public async Task DeleteList(long listId)
        {
            await this.Api.ListsDestroy(listId)
                .IgnoreResponse()
                .ConfigureAwait(false);

            var tabinfo = TabInformations.GetInstance();

            tabinfo.SubscribableLists = tabinfo.SubscribableLists
                .Where(x => x.Id != listId)
                .ToList();
        }

        public async Task<ListElement> EditList(long listId, string new_name, bool isPrivate, string description)
        {
            var response = await this.Api.ListsUpdate(listId, new_name, description, isPrivate)
                .ConfigureAwait(false);

            var list = await response.LoadJsonAsync()
                .ConfigureAwait(false);

            return new ListElement(list, this);
        }

        public async Task<long> GetListMembers(long listId, List<UserInfo> lists, long cursor)
        {
            this.CheckAccountState();

            var users = await this.Api.ListsMembers(listId, cursor)
                .ConfigureAwait(false);

            Array.ForEach(users.Users, u => lists.Add(new UserInfo(u)));

            return users.NextCursor;
        }

        public async Task CreateListApi(string listName, bool isPrivate, string description)
        {
            this.CheckAccountState();

            var response = await this.Api.ListsCreate(listName, description, isPrivate)
                .ConfigureAwait(false);

            var list = await response.LoadJsonAsync()
                .ConfigureAwait(false);

            TabInformations.GetInstance().SubscribableLists.Add(new ListElement(list, this));
        }

        public async Task<bool> ContainsUserAtList(long listId, string user)
        {
            this.CheckAccountState();

            try
            {
                await this.Api.ListsMembersShow(listId, user)
                    .ConfigureAwait(false);

                return true;
            }
            catch (TwitterApiException ex)
                when (ex.ErrorResponse.Errors.Any(x => x.Code == TwitterErrorCode.NotFound))
            {
                return false;
            }
        }

        public string CreateHtmlAnchor(string text, List<string> AtList, TwitterEntities entities, List<MediaInfo> media)
        {
            if (entities != null)
            {
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
                        var screenName = ent.ScreenName.ToLowerInvariant();
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
                                    media.Add(new MediaInfo(ent.MediaUrl, ent.AltText, ent.ExpandedUrl));
                                }
                                else
                                    media.Add(new MediaInfo(ent.MediaUrl, ent.AltText, videoUrl: null));
                            }
                        }
                    }
                }
            }

            // PostClass.ExpandedUrlInfo を使用して非同期に URL 展開を行うためここでは expanded_url を使用しない
            text = TweetFormatter.AutoLinkHtml(text, entities, keepTco: true);

            text = Regex.Replace(text, "(^|[^a-zA-Z0-9_/&#＃@＠>=.~])(sm|nm)([0-9]{1,10})", "$1<a href=\"http://www.nicovideo.jp/watch/$2$3\">$2$3</a>");
            text = PreProcessUrl(text); //IDN置換

            return text;
        }

        private static readonly Uri SourceUriBase = new Uri("https://twitter.com/");

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
                    sourceUri = new Uri(SourceUriBase, uriStr);
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

        public async Task<TwitterApiStatus> GetInfoApi()
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid) return null;

            if (MyCommon._endingFlag) return null;

            var limits = await this.Api.ApplicationRateLimitStatus()
                .ConfigureAwait(false);

            MyCommon.TwitterApiInfo.UpdateFromJson(limits);

            return MyCommon.TwitterApiInfo;
        }

        /// <summary>
        /// ブロック中のユーザーを更新します
        /// </summary>
        /// <exception cref="WebApiException"/>
        public async Task RefreshBlockIds()
        {
            if (MyCommon._endingFlag) return;

            var cursor = -1L;
            var newBlockIds = new HashSet<long>();
            do
            {
                var ret = await this.Api.BlocksIds(cursor)
                    .ConfigureAwait(false);

                newBlockIds.UnionWith(ret.Ids);
                cursor = ret.NextCursor;
            } while (cursor != 0);

            newBlockIds.Remove(this.UserId); // 元のソースにあったので一応残しておく

            TabInformations.GetInstance().BlockIds = newBlockIds;
        }

        /// <summary>
        /// ミュート中のユーザーIDを更新します
        /// </summary>
        /// <exception cref="WebApiException"/>
        public async Task RefreshMuteUserIdsAsync()
        {
            if (MyCommon._endingFlag) return;

            var ids = await TwitterIds.GetAllItemsAsync(x => this.Api.MutesUsersIds(x))
                .ConfigureAwait(false);

            TabInformations.GetInstance().MuteUserIds = new HashSet<long>(ids);
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
            => ((TwitterApiConnection)this.Api.Connection).AccessToken;

        public string AccessTokenSecret
            => ((TwitterApiConnection)this.Api.Connection).AccessSecret;

        private void CheckAccountState()
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid)
                throw new WebApiException("Auth error. Check your account");
        }

        private void CheckAccessLevel(TwitterApiAccessLevel accessLevelFlags)
        {
            if (!this.AccessLevel.HasFlag(accessLevelFlags))
                throw new WebApiException("Auth Err:try to re-authorization.");
        }

        private void CheckStatusCode(HttpStatusCode httpStatus, string responseText,
            [CallerMemberName] string callerMethodName = "")
        {
            if (httpStatus == HttpStatusCode.OK)
            {
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                return;
            }

            if (string.IsNullOrWhiteSpace(responseText))
            {
                if (httpStatus == HttpStatusCode.Unauthorized)
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;

                throw new WebApiException("Err:" + httpStatus + "(" + callerMethodName + ")");
            }

            try
            {
                var errors = TwitterError.ParseJson(responseText).Errors;
                if (errors == null || !errors.Any())
                {
                    throw new WebApiException("Err:" + httpStatus + "(" + callerMethodName + ")", responseText);
                }

                foreach (var error in errors)
                {
                    if (error.Code == TwitterErrorCode.InvalidToken ||
                        error.Code == TwitterErrorCode.SuspendedAccount)
                    {
                        Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                    }
                }

                throw new WebApiException("Err:" + string.Join(",", errors.Select(x => x.ToString())) + "(" + callerMethodName + ")", responseText);
            }
            catch (SerializationException) { }

            throw new WebApiException("Err:" + httpStatus + "(" + callerMethodName + ")", responseText);
        }

        public int GetTextLengthRemain(string postText)
        {
            var matchDm = Twitter.DMSendTextRegex.Match(postText);
            if (matchDm.Success)
                return this.GetTextLengthRemainInternal(matchDm.Groups["body"].Value, isDm: true);

            return this.GetTextLengthRemainInternal(postText, isDm: false);
        }

        private int GetTextLengthRemainInternal(string postText, bool isDm)
        {
            var textLength = 0;

            var pos = 0;
            while (pos < postText.Length)
            {
                textLength++;

                if (char.IsSurrogatePair(postText, pos))
                    pos += 2; // サロゲートペアの場合は2文字分進める
                else
                    pos++;
            }

            var urls = TweetExtractor.ExtractUrls(postText);
            foreach (var url in urls)
            {
                var shortUrlLength = url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                    ? this.Configuration.ShortUrlLengthHttps
                    : this.Configuration.ShortUrlLength;

                textLength += shortUrlLength - url.Length;
            }

            if (isDm)
                return this.Configuration.DmTextCharacterLimit - textLength;
            else
                return 140 - textLength;
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
            ["favorite"] = MyCommon.EVENTTYPE.Favorite,
            ["unfavorite"] = MyCommon.EVENTTYPE.Unfavorite,
            ["follow"] = MyCommon.EVENTTYPE.Follow,
            ["list_member_added"] = MyCommon.EVENTTYPE.ListMemberAdded,
            ["list_member_removed"] = MyCommon.EVENTTYPE.ListMemberRemoved,
            ["block"] = MyCommon.EVENTTYPE.Block,
            ["unblock"] = MyCommon.EVENTTYPE.Unblock,
            ["user_update"] = MyCommon.EVENTTYPE.UserUpdate,
            ["deleted"] = MyCommon.EVENTTYPE.Deleted,
            ["list_created"] = MyCommon.EVENTTYPE.ListCreated,
            ["list_destroyed"] = MyCommon.EVENTTYPE.ListDestroyed,
            ["list_updated"] = MyCommon.EVENTTYPE.ListUpdated,
            ["unfollow"] = MyCommon.EVENTTYPE.Unfollow,
            ["list_user_subscribed"] = MyCommon.EVENTTYPE.ListUserSubscribed,
            ["list_user_unsubscribed"] = MyCommon.EVENTTYPE.ListUserUnsubscribed,
            ["mute"] = MyCommon.EVENTTYPE.Mute,
            ["unmute"] = MyCommon.EVENTTYPE.Unmute,
            ["quoted_tweet"] = MyCommon.EVENTTYPE.QuotedTweet,
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
                        XElement idElm;
                        if ((idElm = xElm.Element("delete").Element("direct_message")?.Element("id")) != null)
                        {
                            id = 0;
                            long.TryParse(idElm.Value, out id);

                            this.PostDeleted?.Invoke(this, new PostDeletedEventArgs(id));
                        }
                        else if ((idElm = xElm.Element("delete").Element("status")?.Element("id")) != null)
                        {
                            id = 0;
                            long.TryParse(idElm.Value, out id);

                            this.PostDeleted?.Invoke(this, new PostDeletedEventArgs(id));
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
                    else if (xElm.Element("retweeted_status") != null)
                    {
                        var sourceUserId = xElm.XPathSelectElement("/user/id_str").Value;
                        var targetUserId = xElm.XPathSelectElement("/retweeted_status/user/id_str").Value;

                        // 自分に関係しないリツイートの場合は無視する
                        var selfUserId = this.UserId.ToString();
                        if (sourceUserId == selfUserId || targetUserId == selfUserId)
                        {
                            // 公式 RT をイベントとしても扱う
                            var evt = CreateEventFromRetweet(xElm);
                            if (evt != null)
                            {
                                this.StoredEvent.Insert(0, evt);

                                this.UserStreamEventReceived?.Invoke(this, new UserStreamEventReceivedEventArgs(evt));
                            }
                        }

                        // 従来通り公式 RT の表示も行うため return しない
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
                    try
                    {
                        var message = TwitterStreamEventDirectMessage.ParseJson(line).DirectMessage;
                        this.CreateDirectMessagesFromJson(new[] { message }, MyCommon.WORKERTYPE.UserStream, false);
                    }
                    catch (SerializationException ex)
                    {
                        throw TwitterApiException.CreateFromException(ex, line);
                    }
                }
                else
                {
                    try
                    {
                        var status = TwitterStatus.ParseJson(line);
                        this.CreatePostsFromJson(new[] { status }, MyCommon.WORKERTYPE.UserStream, null, false);
                    }
                    catch (SerializationException ex)
                    {
                        throw TwitterApiException.CreateFromException(ex, line);
                    }
                }
            }
            catch (WebApiException ex)
            {
                MyCommon.TraceOut(ex);
                return;
            }
            catch(NullReferenceException)
            {
                MyCommon.TraceOut("NullRef StatusArrived: " + line);
            }

            this.NewPostFromStream?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// UserStreamsから受信した公式RTをイベントに変換します
        /// </summary>
        private FormattedEvent CreateEventFromRetweet(XElement xElm)
        {
            return new FormattedEvent
            {
                Eventtype = MyCommon.EVENTTYPE.Retweet,
                Event = "retweet",
                CreatedAt = MyCommon.DateTimeParse(xElm.XPathSelectElement("/created_at").Value),
                IsMe = xElm.XPathSelectElement("/user/id_str").Value == this.UserId.ToString(),
                Username = xElm.XPathSelectElement("/user/screen_name").Value,
                Target = string.Format("@{0}:{1}", new[]
                {
                    xElm.XPathSelectElement("/retweeted_status/user/screen_name").Value,
                    WebUtility.HtmlDecode(xElm.XPathSelectElement("/retweeted_status/text").Value),
                }),
                Id = long.Parse(xElm.XPathSelectElement("/retweeted_status/id_str").Value),
            };
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
            evt.IsMe = evt.Username.ToLowerInvariant().Equals(this.Username.ToLowerInvariant());

            MyCommon.EVENTTYPE eventType;
            eventTable.TryGetValue(eventData.Event, out eventType);
            evt.Eventtype = eventType;

            TwitterStreamEvent<TwitterStatus> tweetEvent;

            switch (eventData.Event)
            {
                case "access_revoked":
                case "access_unrevoked":
                case "user_delete":
                case "user_suspend":
                    return;
                case "follow":
                    if (eventData.Target.ScreenName.ToLowerInvariant().Equals(_uname))
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
                    tweetEvent = TwitterStreamEvent<TwitterStatus>.ParseJson(content);
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
                            favTab.AddPostImmediately(post.StatusId, post.IsRead);

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
                case "quoted_tweet":
                    if (evt.IsMe) return;

                    tweetEvent = TwitterStreamEvent<TwitterStatus>.ParseJson(content);
                    evt.Target = "@" + tweetEvent.TargetObject.User.ScreenName + ":" + WebUtility.HtmlDecode(tweetEvent.TargetObject.Text);
                    evt.Id = tweetEvent.TargetObject.Id;

                    if (SettingCommon.Instance.IsRemoveSameEvent)
                    {
                        if (this.StoredEvent.Any(ev => ev.Username == evt.Username && ev.Eventtype == evt.Eventtype && ev.Target == evt.Target))
                            return;
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

            this.UserStreamEventReceived?.Invoke(this, new UserStreamEventReceivedEventArgs(evt));
        }

        private void userStream_Started()
        {
            this.UserStreamStarted?.Invoke(this, EventArgs.Empty);
        }

        private void userStream_Stopped()
        {
            this.UserStreamStopped?.Invoke(this, EventArgs.Empty);
        }

        public bool UserStreamActive
            => this.userStream == null ? false : this.userStream.IsStreamActive;

        public void StartUserStream()
        {
            var newStream = new TwitterUserstream(this.Api);

            newStream.StatusArrived += userStream_StatusArrived;
            newStream.Started += userStream_Started;
            newStream.Stopped += userStream_Stopped;

            newStream.Start(this.AllAtReply, this.TrackWord);

            var oldStream = Interlocked.Exchange(ref this.userStream, newStream);
            oldStream?.Dispose();
        }

        public void StopUserStream()
        {
            var oldStream = Interlocked.Exchange(ref this.userStream, null);
            oldStream?.Dispose();
        }

        public void ReconnectUserStream()
        {
            this.StartUserStream();
        }

        private class TwitterUserstream : IDisposable
        {
            public bool AllAtReplies { get; private set; }
            public string TrackWords { get; private set; }

            public bool IsStreamActive { get; private set; }

            public event Action<string> StatusArrived;
            public event Action Stopped;
            public event Action Started;

            private TwitterApi twitterApi;

            private Task streamTask;
            private CancellationTokenSource streamCts;

            public TwitterUserstream(TwitterApi twitterApi)
            {
                this.twitterApi = twitterApi;
            }

            public void Start(bool allAtReplies, string trackwords)
            {
                this.AllAtReplies = allAtReplies;
                this.TrackWords = trackwords;

                var cts = new CancellationTokenSource();

                this.streamCts = cts;
                this.streamTask = Task.Run(async () =>
                {
                    try
                    {
                        await this.UserStreamLoop(cts.Token)
                            .ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) { }
                });
            }

            public void Stop()
            {
                this.streamCts?.Cancel();

                // streamTask の完了を待たずに IsStreamActive を false にセットする
                this.IsStreamActive = false;
                this.Stopped?.Invoke();
            }

            private async Task UserStreamLoop(CancellationToken cancellationToken)
            {
                TimeSpan? sleep = null;
                for (;;)
                {
                    if (sleep != null)
                    {
                        await Task.Delay(sleep.Value, cancellationToken)
                            .ConfigureAwait(false);
                        sleep = null;
                    }

                    if (!MyCommon.IsNetworkAvailable())
                    {
                        sleep = TimeSpan.FromSeconds(30);
                        continue;
                    }

                    this.IsStreamActive = true;
                    this.Started?.Invoke();

                    try
                    {
                        var replies = this.AllAtReplies ? "all" : null;

                        using (var stream = await this.twitterApi.UserStreams(replies, this.TrackWords)
                            .ConfigureAwait(false))
                        using (var reader = new StreamReader(stream))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = await reader.ReadLineAsync()
                                    .ConfigureAwait(false);

                                cancellationToken.ThrowIfCancellationRequested();

                                this.StatusArrived?.Invoke(line);
                            }
                        }

                        // キャンセルされていないのにストリームが終了した場合
                        sleep = TimeSpan.FromSeconds(30);
                    }
                    catch (TwitterApiException) { sleep = TimeSpan.FromSeconds(30); }
                    catch (IOException) { sleep = TimeSpan.FromSeconds(30); }
                    catch (OperationCanceledException)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            throw;

                        // cancellationToken によるキャンセルではない（＝タイムアウトエラー）
                        sleep = TimeSpan.FromSeconds(30);
                    }
                    catch (Exception ex)
                    {
                        MyCommon.ExceptionOut(ex);
                        sleep = TimeSpan.FromSeconds(30);
                    }
                    finally
                    {
                        this.IsStreamActive = false;
                        this.Stopped?.Invoke();
                    }
                }
            }

            private bool disposed = false;

            public void Dispose()
            {
                if (this.disposed)
                    return;

                this.disposed = true;

                this.Stop();

                this.Started = null;
                this.Stopped = null;
                this.StatusArrived = null;
            }
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
        public long StatusId { get; }

        public PostDeletedEventArgs(long statusId)
        {
            this.StatusId = statusId;
        }
    }

    public class UserStreamEventReceivedEventArgs : EventArgs
    {
        public Twitter.FormattedEvent EventData { get; }

        public UserStreamEventReceivedEventArgs(Twitter.FormattedEvent eventData)
        {
            this.EventData = eventData;
        }
    }
}
