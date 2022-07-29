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
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Models;
using OpenTween.Setting;

namespace OpenTween
{
    public partial class TweetDetailsView : UserControl
    {
        private TweenMain Owner
            => this.owner ?? throw this.NotInitializedException();

        /// <summary>プロフィール画像のキャッシュ</summary>
        private ImageCache IconCache
            => this.iconCache ?? throw this.NotInitializedException();

        /// <summary><see cref="PostClass"/> のダンプを表示するか</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DumpPostClass { get; set; }

        /// <summary>現在表示中の発言</summary>
        public PostClass? CurrentPost { get; private set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ThemeManager Theme
        {
            get => this.themeManager ?? throw this.NotInitializedException();
            set => this.themeManager = value;
        }

        [DefaultValue(false)]
        public new bool TabStop
        {
            get => base.TabStop;
            set => base.TabStop = value;
        }

        /// <summary>ステータスバーに表示するテキストの変化を通知するイベント</summary>
        public event EventHandler<TweetDetailsViewStatusChengedEventArgs>? StatusChanged;

        /// <summary><see cref="ContextMenuPostBrowser"/> 展開時の <see cref="PostBrowser"/>.StatusText を保持するフィールド</summary>
        private string postBrowserStatusText = "";

        private TweenMain? owner;
        private ImageCache? iconCache;
        private ThemeManager? themeManager;

        public TweetDetailsView()
        {
            this.InitializeComponent();

            this.TabStop = false;

            // 発言詳細部の初期化
            this.NameLinkLabel.Text = "";
            this.DateTimeLabel.Text = "";
            this.SourceLinkLabel.Text = "";

            new InternetSecurityManager(this.PostBrowser);
            this.PostBrowser.AllowWebBrowserDrop = false;  // COMException を回避するため、ActiveX の初期化が終わってから設定する
        }

        public void Initialize(TweenMain owner, ImageCache iconCache, ThemeManager themeManager)
        {
            this.owner = owner;
            this.iconCache = iconCache;
            this.themeManager = themeManager;
        }

        private Exception NotInitializedException()
            => new InvalidOperationException("Cannot call before initialization");

        public void ClearPostBrowser()
            => this.PostBrowser.DocumentText = this.Owner.CreateDetailHtml("");

        public async Task ShowPostDetails(PostClass post)
        {
            this.CurrentPost = post;

            var loadTasks = new List<Task>();

            using (ControlTransaction.Update(this.TableLayoutPanel1))
            {
                this.SourceLinkLabel.Text = post.Source;
                this.SourceLinkLabel.TabStop = false; // Text を更新すると勝手に true にされる

                string nameText;
                if (post.IsDm)
                {
                    if (post.IsOwl)
                        nameText = "DM FROM <- ";
                    else
                        nameText = "DM TO -> ";
                }
                else
                {
                    nameText = "";
                }
                nameText += post.ScreenName + "/" + post.Nickname;
                if (post.RetweetedId != null)
                    nameText += $" (RT:{post.RetweetedBy})";

                this.NameLinkLabel.Text = nameText;

                var nameForeColor = SystemColors.ControlText;
                if (post.IsOwl && (SettingManager.Instance.Common.OneWayLove || post.IsDm))
                    nameForeColor = this.Theme.ColorOWL;
                if (post.RetweetedId != null)
                    nameForeColor = this.Theme.ColorRetweet;
                if (post.IsFav)
                    nameForeColor = this.Theme.ColorFav;

                this.NameLinkLabel.LinkColor = nameForeColor;
                this.NameLinkLabel.ActiveLinkColor = nameForeColor;

                loadTasks.Add(this.SetUserPictureAsync(post.ImageUrl));

                this.DateTimeLabel.Text = post.CreatedAt.ToLocalTimeString();
            }

            if (this.DumpPostClass)
            {
                var sb = new StringBuilder(512);

                sb.Append("-----Start PostClass Dump<br>");
                sb.AppendFormat("TextFromApi           : {0}<br>", post.TextFromApi);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", post.TextFromApi);
                sb.AppendFormat("StatusId             : {0}<br>", post.StatusId);
                sb.AppendFormat("ImageUrl       : {0}<br>", post.ImageUrl);
                sb.AppendFormat("InReplyToStatusId    : {0}<br>", post.InReplyToStatusId);
                sb.AppendFormat("InReplyToUser  : {0}<br>", post.InReplyToUser);
                sb.AppendFormat("IsDM           : {0}<br>", post.IsDm);
                sb.AppendFormat("IsFav          : {0}<br>", post.IsFav);
                sb.AppendFormat("IsMark         : {0}<br>", post.IsMark);
                sb.AppendFormat("IsMe           : {0}<br>", post.IsMe);
                sb.AppendFormat("IsOwl          : {0}<br>", post.IsOwl);
                sb.AppendFormat("IsProtect      : {0}<br>", post.IsProtect);
                sb.AppendFormat("IsRead         : {0}<br>", post.IsRead);
                sb.AppendFormat("IsReply        : {0}<br>", post.IsReply);

                foreach (var nm in post.ReplyToList.Select(x => x.ScreenName))
                {
                    sb.AppendFormat("ReplyToList    : {0}<br>", nm);
                }

                sb.AppendFormat("ScreenName           : {0}<br>", post.ScreenName);
                sb.AppendFormat("NickName       : {0}<br>", post.Nickname);
                sb.AppendFormat("Text   : {0}<br>", post.Text);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", post.Text);
                sb.AppendFormat("CreatedAt          : {0}<br>", post.CreatedAt.ToLocalTimeString());
                sb.AppendFormat("Source         : {0}<br>", post.Source);
                sb.AppendFormat("UserId            : {0}<br>", post.UserId);
                sb.AppendFormat("FilterHit      : {0}<br>", post.FilterHit);
                sb.AppendFormat("RetweetedBy    : {0}<br>", post.RetweetedBy);
                sb.AppendFormat("RetweetedId    : {0}<br>", post.RetweetedId);

                sb.AppendFormat("Media.Count    : {0}<br>", post.Media.Count);
                if (post.Media.Count > 0)
                {
                    for (var i = 0; i < post.Media.Count; i++)
                    {
                        var info = post.Media[i];
                        sb.AppendFormat("Media[{0}].Url         : {1}<br>", i, info.Url);
                        sb.AppendFormat("Media[{0}].VideoUrl    : {1}<br>", i, info.VideoUrl ?? "---");
                    }
                }
                sb.Append("-----End PostClass Dump<br>");

                this.PostBrowser.DocumentText = this.Owner.CreateDetailHtml(sb.ToString());
                return;
            }

            using (ControlTransaction.Update(this.PostBrowser))
            {
                this.PostBrowser.DocumentText =
                    this.Owner.CreateDetailHtml(post.IsDeleted ? "(DELETED)" : post.Text);

                this.PostBrowser.Document.Window.ScrollTo(0, 0);
            }

            loadTasks.Add(this.AppendQuoteTweetAsync(post));

            await Task.WhenAll(loadTasks);
        }

        public void ScrollDownPostBrowser(bool forward)
        {
            var doc = this.PostBrowser.Document;
            if (doc == null) return;

            var tags = doc.GetElementsByTagName("html");
            if (tags.Count > 0)
            {
                if (forward)
                    tags[0].ScrollTop += this.Theme.FontDetail.Height;
                else
                    tags[0].ScrollTop -= this.Theme.FontDetail.Height;
            }
        }

        public void PageDownPostBrowser(bool forward)
        {
            var doc = this.PostBrowser.Document;
            if (doc == null) return;

            var tags = doc.GetElementsByTagName("html");
            if (tags.Count > 0)
            {
                if (forward)
                    tags[0].ScrollTop += this.PostBrowser.ClientRectangle.Height - this.Theme.FontDetail.Height;
                else
                    tags[0].ScrollTop -= this.PostBrowser.ClientRectangle.Height - this.Theme.FontDetail.Height;
            }
        }

        public HtmlElement[] GetLinkElements()
        {
            return this.PostBrowser.Document.Links.Cast<HtmlElement>()
                .Where(x => x.GetAttribute("className") != "tweet-quote-link") // 引用ツイートで追加されたリンクを除く
                .ToArray();
        }

        private async Task SetUserPictureAsync(string normalImageUrl, bool force = false)
        {
            if (MyCommon.IsNullOrEmpty(normalImageUrl))
                return;

            if (this.IconCache == null)
                return;

            this.ClearUserPicture();

            var imageSize = Twitter.DecideProfileImageSize(this.UserPicture.Width);
            var cachedImage = this.IconCache.TryGetLargerOrSameSizeFromCache(normalImageUrl, imageSize);
            if (cachedImage != null)
            {
                // 既にキャッシュされていればそれを表示して終了
                this.UserPicture.Image = cachedImage.Clone();
                return;
            }

            // 小さいサイズの画像がキャッシュにある場合は高解像度の画像が取得できるまでの間表示する
            var fallbackImage = this.IconCache.TryGetLargerOrSameSizeFromCache(normalImageUrl, "mini");
            if (fallbackImage != null)
                this.UserPicture.Image = fallbackImage.Clone();

            await this.UserPicture.SetImageFromTask(
                async () =>
                {
                    var imageUrl = Twitter.CreateProfileImageUrl(normalImageUrl, imageSize);
                    var image = await this.IconCache.DownloadImageAsync(imageUrl, force)
                        .ConfigureAwait(false);

                    return image.Clone();
                },
                useStatusImage: false
            );
        }

        /// <summary>
        /// UserPicture.Image に設定されている画像を破棄します。
        /// </summary>
        private void ClearUserPicture()
        {
            if (this.UserPicture.Image != null)
            {
                var oldImage = this.UserPicture.Image;
                this.UserPicture.Image = null;
                oldImage.Dispose();
            }
        }

        /// <summary>
        /// 発言詳細欄のツイートURLを展開する
        /// </summary>
        private async Task AppendQuoteTweetAsync(PostClass post)
        {
            var quoteStatusIds = post.QuoteStatusIds;
            if (quoteStatusIds.Length == 0 && post.InReplyToStatusId == null)
                return;

            // 「読み込み中」テキストを表示
            var loadingQuoteHtml = quoteStatusIds.Select(x => FormatQuoteTweetHtml(x, Properties.Resources.LoadingText, isReply: false));

            var loadingReplyHtml = string.Empty;
            if (post.InReplyToStatusId != null)
                loadingReplyHtml = FormatQuoteTweetHtml(post.InReplyToStatusId.Value, Properties.Resources.LoadingText, isReply: true);

            var body = post.Text + string.Concat(loadingQuoteHtml) + loadingReplyHtml;

            using (ControlTransaction.Update(this.PostBrowser))
                this.PostBrowser.DocumentText = this.Owner.CreateDetailHtml(body);

            // 引用ツイートを読み込み
            var loadTweetTasks = quoteStatusIds.Select(x => this.CreateQuoteTweetHtml(x, isReply: false)).ToList();

            if (post.InReplyToStatusId != null)
                loadTweetTasks.Add(this.CreateQuoteTweetHtml(post.InReplyToStatusId.Value, isReply: true));

            var quoteHtmls = await Task.WhenAll(loadTweetTasks);

            // 非同期処理中に表示中のツイートが変わっていたらキャンセルされたものと扱う
            if (this.CurrentPost != post || this.CurrentPost.IsDeleted)
                return;

            body = post.Text + string.Concat(quoteHtmls);

            using (ControlTransaction.Update(this.PostBrowser))
                this.PostBrowser.DocumentText = this.Owner.CreateDetailHtml(body);
        }

        private async Task<string> CreateQuoteTweetHtml(long statusId, bool isReply)
        {
            var post = TabInformations.GetInstance()[statusId];
            if (post == null)
            {
                try
                {
                    post = await this.Owner.TwitterInstance.GetStatusApi(false, statusId)
                        .ConfigureAwait(false);
                }
                catch (WebApiException ex)
                {
                    return FormatQuoteTweetHtml(statusId, WebUtility.HtmlEncode($"Err:{ex.Message}(GetStatus)"), isReply);
                }

                post.IsRead = true;
                if (!TabInformations.GetInstance().AddQuoteTweet(post))
                    return FormatQuoteTweetHtml(statusId, "This Tweet is unavailable.", isReply);
            }

            return FormatQuoteTweetHtml(post, isReply);
        }

        internal static string FormatQuoteTweetHtml(PostClass post, bool isReply)
        {
            var innerHtml = "<p>" + StripLinkTagHtml(post.Text) + "</p>" +
                " &mdash; " + WebUtility.HtmlEncode(post.Nickname) +
                " (@" + WebUtility.HtmlEncode(post.ScreenName) + ") " +
                WebUtility.HtmlEncode(post.CreatedAt.ToLocalTimeString());

            return FormatQuoteTweetHtml(post.StatusId, innerHtml, isReply);
        }

        internal static string FormatQuoteTweetHtml(long statusId, string innerHtml, bool isReply)
        {
            var blockClassName = "quote-tweet";

            if (isReply)
                blockClassName += " reply";

            return "<a class=\"quote-tweet-link\" href=\"//opentween/status/" + statusId + "\">" +
                $"<blockquote class=\"{blockClassName}\">{innerHtml}</blockquote>" +
                "</a>";
        }

        /// <summary>
        /// 指定されたHTMLからリンクを除去します
        /// </summary>
        internal static string StripLinkTagHtml(string html)
            => Regex.Replace(html, @"<a[^>]*>(.*?)</a>", "$1"); // a 要素はネストされていない前提の正規表現パターン

        public async Task DoTranslation()
        {
            if (this.CurrentPost == null || this.CurrentPost.IsDeleted)
                return;

            await this.DoTranslation(this.CurrentPost.TextFromApi);
        }

        private async Task DoTranslation(string str)
        {
            if (MyCommon.IsNullOrEmpty(str))
                return;

            var bing = new Bing();
            try
            {
                var translatedText = await bing.TranslateAsync(str,
                    langFrom: null,
                    langTo: SettingManager.Instance.Common.TranslateLanguage);

                this.PostBrowser.DocumentText = this.Owner.CreateDetailHtml(translatedText);
            }
            catch (WebApiException e)
            {
                this.RaiseStatusChanged("Err:" + e.Message);
            }
            catch (OperationCanceledException)
            {
                this.RaiseStatusChanged("Err:Timeout");
            }
        }

        private async Task DoSearchToolStrip(string url)
        {
            // 発言詳細で「選択文字列で検索」（選択文字列取得）
            var selText = this.PostBrowser.GetSelectedText();

            if (selText != null)
            {
                if (url == Properties.Resources.SearchItem4Url)
                {
                    // 公式検索
                    this.Owner.AddNewTabForSearch(selText);
                    return;
                }

                var tmp = string.Format(url, Uri.EscapeDataString(selText));
                await MyCommon.OpenInBrowserAsync(this, tmp);
            }
        }

        private string? GetUserId()
        {
            var m = Regex.Match(this.postBrowserStatusText, @"^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?$");
            if (m.Success && this.Owner.IsTwitterId(m.Result("${ScreenName}")))
                return m.Result("${ScreenName}");
            else
                return null;
        }

        protected void RaiseStatusChanged(string statusText)
            => this.StatusChanged?.Invoke(this, new TweetDetailsViewStatusChengedEventArgs(statusText));

        private void TweetDetailsView_FontChanged(object sender, EventArgs e)
        {
            // OTBaseForm.GlobalFont による UI フォントの変更に対応
            var origFont = this.NameLinkLabel.Font;
            this.NameLinkLabel.Font = new Font(this.Font.Name, origFont.Size, origFont.Style);
        }

        #region TableLayoutPanel1

        private async void UserPicture_Click(object sender, EventArgs e)
        {
            var screenName = this.CurrentPost?.ScreenName;
            if (screenName != null)
                await this.Owner.ShowUserStatus(screenName, showInputDialog: false);
        }

        private async void PostBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.AbsoluteUri != "about:blank")
            {
                await this.ShowPostDetails(this.CurrentPost!); // 現在の発言を表示し直す (Navigated の段階ではキャンセルできない)
                await MyCommon.OpenInBrowserAsync(this, e.Url.OriginalString);
            }
        }

        private async void PostBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Scheme == "data")
            {
                this.RaiseStatusChanged(this.PostBrowser.StatusText.Replace("&", "&&"));
            }
            else if (e.Url.AbsoluteUri != "about:blank")
            {
                e.Cancel = true;
                // Ctrlを押しながらリンクを開いた場合は、設定と逆の動作をするフラグを true としておく
                await this.Owner.OpenUriAsync(e.Url, MyCommon.IsKeyDown(Keys.Control));
            }
        }

        private async void PostBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            var keyRes = this.Owner.CommonKeyDown(e.KeyData, FocusedControl.PostBrowser, out var asyncTask);
            if (keyRes)
            {
                e.IsInputKey = true;
            }
            else
            {
                if (Enum.IsDefined(typeof(Shortcut), (Shortcut)e.KeyData))
                {
                    var shortcut = (Shortcut)e.KeyData;
                    switch (shortcut)
                    {
                        case Shortcut.CtrlA:
                        case Shortcut.CtrlC:
                        case Shortcut.CtrlIns:
                            // 既定の動作を有効にする
                            break;
                        default:
                            // その他のショートカットキーは無効にする
                            e.IsInputKey = true;
                            break;
                    }
                }
            }

            if (asyncTask != null)
                await asyncTask;
        }

        private void PostBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.PostBrowser.StatusText.StartsWith("http", StringComparison.Ordinal)
                    || this.PostBrowser.StatusText.StartsWith("ftp", StringComparison.Ordinal)
                    || this.PostBrowser.StatusText.StartsWith("data", StringComparison.Ordinal))
                {
                    this.RaiseStatusChanged(this.PostBrowser.StatusText.Replace("&", "&&"));
                }
                if (MyCommon.IsNullOrEmpty(this.PostBrowser.StatusText))
                {
                    this.RaiseStatusChanged(statusText: "");
                }
            }
            catch (Exception)
            {
            }
        }

        private async void SourceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var sourceUri = this.CurrentPost?.SourceUri;
            if (sourceUri != null && e.Button == MouseButtons.Left)
            {
                await MyCommon.OpenInBrowserAsync(this, sourceUri.AbsoluteUri);
            }
        }

        private void SourceLinkLabel_MouseEnter(object sender, EventArgs e)
        {
            var sourceUri = this.CurrentPost?.SourceUri;
            if (sourceUri != null)
            {
                this.RaiseStatusChanged(MyCommon.ConvertToReadableUrl(sourceUri.AbsoluteUri));
            }
        }

        private void SourceLinkLabel_MouseLeave(object sender, EventArgs e)
            => this.RaiseStatusChanged(statusText: "");

        #endregion

        #region ContextMenuUserPicture

        private void ContextMenuUserPicture_Opening(object sender, CancelEventArgs e)
        {
            // 発言詳細のアイコン右クリック時のメニュー制御
            if (this.CurrentPost != null)
            {
                var name = this.CurrentPost.ImageUrl;
                if (!MyCommon.IsNullOrEmpty(name))
                {
                    var idx = name.LastIndexOf('/');
                    if (idx != -1)
                    {
                        name = Path.GetFileName(name.Substring(idx));
                        if (name.Contains("_normal.") || name.EndsWith("_normal", StringComparison.Ordinal))
                        {
                            name = name.Replace("_normal", "");
                            this.IconNameToolStripMenuItem.Text = name;
                            this.IconNameToolStripMenuItem.Enabled = true;
                        }
                        else
                        {
                            this.IconNameToolStripMenuItem.Enabled = false;
                            this.IconNameToolStripMenuItem.Text = Properties.Resources.ContextMenuStrip3_OpeningText1;
                        }
                    }
                    else
                    {
                        this.IconNameToolStripMenuItem.Enabled = false;
                        this.IconNameToolStripMenuItem.Text = Properties.Resources.ContextMenuStrip3_OpeningText1;
                    }

                    this.ReloadIconToolStripMenuItem.Enabled = true;

                    if (this.IconCache.TryGetFromCache(this.CurrentPost.ImageUrl) != null)
                    {
                        this.SaveIconPictureToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        this.SaveIconPictureToolStripMenuItem.Enabled = false;
                    }
                }
                else
                {
                    this.IconNameToolStripMenuItem.Enabled = false;
                    this.ReloadIconToolStripMenuItem.Enabled = false;
                    this.SaveIconPictureToolStripMenuItem.Enabled = false;
                    this.IconNameToolStripMenuItem.Text = Properties.Resources.ContextMenuStrip3_OpeningText1;
                }
            }
            else
            {
                this.IconNameToolStripMenuItem.Enabled = false;
                this.ReloadIconToolStripMenuItem.Enabled = false;
                this.SaveIconPictureToolStripMenuItem.Enabled = false;
                this.IconNameToolStripMenuItem.Text = Properties.Resources.ContextMenuStrip3_OpeningText2;
            }
            if (this.CurrentPost != null)
            {
                if (this.CurrentPost.UserId == this.Owner.TwitterInstance.UserId)
                {
                    this.FollowToolStripMenuItem.Enabled = false;
                    this.UnFollowToolStripMenuItem.Enabled = false;
                    this.ShowFriendShipToolStripMenuItem.Enabled = false;
                    this.ShowUserStatusToolStripMenuItem.Enabled = true;
                    this.SearchPostsDetailNameToolStripMenuItem.Enabled = true;
                    this.SearchAtPostsDetailNameToolStripMenuItem.Enabled = false;
                    this.ListManageUserContextToolStripMenuItem3.Enabled = true;
                }
                else
                {
                    this.FollowToolStripMenuItem.Enabled = true;
                    this.UnFollowToolStripMenuItem.Enabled = true;
                    this.ShowFriendShipToolStripMenuItem.Enabled = true;
                    this.ShowUserStatusToolStripMenuItem.Enabled = true;
                    this.SearchPostsDetailNameToolStripMenuItem.Enabled = true;
                    this.SearchAtPostsDetailNameToolStripMenuItem.Enabled = true;
                    this.ListManageUserContextToolStripMenuItem3.Enabled = true;
                }
            }
            else
            {
                this.FollowToolStripMenuItem.Enabled = false;
                this.UnFollowToolStripMenuItem.Enabled = false;
                this.ShowFriendShipToolStripMenuItem.Enabled = false;
                this.ShowUserStatusToolStripMenuItem.Enabled = false;
                this.SearchPostsDetailNameToolStripMenuItem.Enabled = false;
                this.SearchAtPostsDetailNameToolStripMenuItem.Enabled = false;
                this.ListManageUserContextToolStripMenuItem3.Enabled = false;
            }
        }

        private async void FollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.CurrentPost == null)
                return;

            if (this.CurrentPost.UserId == this.Owner.TwitterInstance.UserId)
                return;

            await this.Owner.FollowCommand(this.CurrentPost.ScreenName);
        }

        private async void UnFollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.CurrentPost == null)
                return;

            if (this.CurrentPost.UserId == this.Owner.TwitterInstance.UserId)
                return;

            await this.Owner.RemoveCommand(this.CurrentPost.ScreenName, false);
        }

        private async void ShowFriendShipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.CurrentPost == null)
                return;

            if (this.CurrentPost.UserId == this.Owner.TwitterInstance.UserId)
                return;

            await this.Owner.ShowFriendship(this.CurrentPost.ScreenName);
        }

        // ListManageUserContextToolStripMenuItem3.Click は ListManageUserContextToolStripMenuItem_Click を共用

        private async void ShowUserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.CurrentPost == null)
                return;

            await this.Owner.ShowUserStatus(this.CurrentPost.ScreenName, false);
        }

        private async void SearchPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.CurrentPost == null)
                return;

            await this.Owner.AddNewTabForUserTimeline(this.CurrentPost.ScreenName);
        }

        private void SearchAtPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.CurrentPost == null)
                return;

            this.Owner.AddNewTabForSearch("@" + this.CurrentPost.ScreenName);
        }

        private async void IconNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var imageNormalUrl = this.CurrentPost?.ImageUrl;
            if (MyCommon.IsNullOrEmpty(imageNormalUrl))
                return;

            var imageOriginalUrl = Twitter.CreateProfileImageUrl(imageNormalUrl, "original");
            await MyCommon.OpenInBrowserAsync(this, imageOriginalUrl);
        }

        private async void ReloadIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var imageUrl = this.CurrentPost?.ImageUrl;
            if (MyCommon.IsNullOrEmpty(imageUrl))
                return;

            await this.SetUserPictureAsync(imageUrl, force: true);
        }

        private void SaveIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var imageUrl = this.CurrentPost?.ImageUrl;
            if (MyCommon.IsNullOrEmpty(imageUrl))
                return;

            var memoryImage = this.IconCache.TryGetFromCache(imageUrl);
            if (memoryImage == null)
                return;

            this.Owner.SaveFileDialog1.FileName = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);

            if (this.Owner.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using var orgBmp = new Bitmap(memoryImage.Image);
                    using var bmp2 = new Bitmap(orgBmp.Size.Width, orgBmp.Size.Height);

                    using (var g = Graphics.FromImage(bmp2))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                        g.DrawImage(orgBmp, 0, 0, orgBmp.Size.Width, orgBmp.Size.Height);
                    }
                    bmp2.Save(this.Owner.SaveFileDialog1.FileName);
                }
                catch (Exception)
                {
                    // 処理中にキャッシュアウトする可能性あり
                }
            }
        }

        #endregion

        #region ContextMenuPostBrowser

        private void ContextMenuPostBrowser_Opening(object ender, CancelEventArgs e)
        {
            // URLコピーの項目の表示/非表示
            if (this.PostBrowser.StatusText.StartsWith("http", StringComparison.Ordinal))
            {
                this.postBrowserStatusText = this.PostBrowser.StatusText;
                var name = this.GetUserId();
                this.UrlCopyContextMenuItem.Enabled = true;
                if (name != null)
                {
                    this.FollowContextMenuItem.Enabled = true;
                    this.RemoveContextMenuItem.Enabled = true;
                    this.FriendshipContextMenuItem.Enabled = true;
                    this.ShowUserStatusContextMenuItem.Enabled = true;
                    this.SearchPostsDetailToolStripMenuItem.Enabled = true;
                    this.IdFilterAddMenuItem.Enabled = true;
                    this.ListManageUserContextToolStripMenuItem.Enabled = true;
                    this.SearchAtPostsDetailToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.FollowContextMenuItem.Enabled = false;
                    this.RemoveContextMenuItem.Enabled = false;
                    this.FriendshipContextMenuItem.Enabled = false;
                    this.ShowUserStatusContextMenuItem.Enabled = false;
                    this.SearchPostsDetailToolStripMenuItem.Enabled = false;
                    this.IdFilterAddMenuItem.Enabled = false;
                    this.ListManageUserContextToolStripMenuItem.Enabled = false;
                    this.SearchAtPostsDetailToolStripMenuItem.Enabled = false;
                }

                if (Regex.IsMatch(this.postBrowserStatusText, @"^https?://twitter.com/search\?q=%23"))
                    this.UseHashtagMenuItem.Enabled = true;
                else
                    this.UseHashtagMenuItem.Enabled = false;
            }
            else
            {
                this.postBrowserStatusText = "";
                this.UrlCopyContextMenuItem.Enabled = false;
                this.FollowContextMenuItem.Enabled = false;
                this.RemoveContextMenuItem.Enabled = false;
                this.FriendshipContextMenuItem.Enabled = false;
                this.ShowUserStatusContextMenuItem.Enabled = false;
                this.SearchPostsDetailToolStripMenuItem.Enabled = false;
                this.SearchAtPostsDetailToolStripMenuItem.Enabled = false;
                this.UseHashtagMenuItem.Enabled = false;
                this.IdFilterAddMenuItem.Enabled = false;
                this.ListManageUserContextToolStripMenuItem.Enabled = false;
            }
            // 文字列選択されていないときは選択文字列関係の項目を非表示に
            var selText = this.PostBrowser.GetSelectedText();
            if (selText == null)
            {
                this.SelectionSearchContextMenuItem.Enabled = false;
                this.SelectionCopyContextMenuItem.Enabled = false;
                this.SelectionTranslationToolStripMenuItem.Enabled = false;
            }
            else
            {
                this.SelectionSearchContextMenuItem.Enabled = true;
                this.SelectionCopyContextMenuItem.Enabled = true;
                this.SelectionTranslationToolStripMenuItem.Enabled = true;
            }
            // 発言内に自分以外のユーザーが含まれてればフォロー状態全表示を有効に
            var ma = Regex.Matches(this.PostBrowser.DocumentText, @"href=""https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?""");
            var fAllFlag = false;
            foreach (Match mu in ma)
            {
                if (!mu.Result("${ScreenName}").Equals(this.Owner.TwitterInstance.Username, StringComparison.InvariantCultureIgnoreCase))
                {
                    fAllFlag = true;
                    break;
                }
            }
            this.FriendshipAllMenuItem.Enabled = fAllFlag;

            if (this.CurrentPost == null)
                this.TranslationToolStripMenuItem.Enabled = false;
            else
                this.TranslationToolStripMenuItem.Enabled = true;

            e.Cancel = false;
        }

        private async void SearchGoogleContextMenuItem_Click(object sender, EventArgs e)
            => await this.DoSearchToolStrip(Properties.Resources.SearchItem2Url);

        private async void SearchWikipediaContextMenuItem_Click(object sender, EventArgs e)
            => await this.DoSearchToolStrip(Properties.Resources.SearchItem1Url);

        private async void SearchPublicSearchContextMenuItem_Click(object sender, EventArgs e)
            => await this.DoSearchToolStrip(Properties.Resources.SearchItem4Url);

        private void CurrentTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細の選択文字列で現在のタブを検索
            var selText = this.PostBrowser.GetSelectedText();

            if (selText != null)
            {
                var searchOptions = new SearchWordDialog.SearchOptions(
                    SearchWordDialog.SearchType.Timeline,
                    selText,
                    NewTab: false,
                    CaseSensitive: false,
                    UseRegex: false
                );

                this.Owner.SearchDialog.ResultOptions = searchOptions;

                this.Owner.DoTabSearch(
                    searchOptions.Query,
                    searchOptions.CaseSensitive,
                    searchOptions.UseRegex,
                    TweenMain.SEARCHTYPE.NextSearch);
            }
        }

        private void SelectionCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            // 発言詳細で「選択文字列をコピー」
            var selText = this.PostBrowser.GetSelectedText();
            try
            {
                Clipboard.SetDataObject(selText, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UrlCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var link in this.PostBrowser.Document.Links.Cast<HtmlElement>())
                {
                    if (link.GetAttribute("href") == this.postBrowserStatusText)
                    {
                        var linkStr = link.GetAttribute("title");
                        if (MyCommon.IsNullOrEmpty(linkStr))
                            linkStr = link.GetAttribute("href");

                        Clipboard.SetDataObject(linkStr, false, 5, 100);
                        return;
                    }
                }

                Clipboard.SetDataObject(this.postBrowserStatusText, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectionAllContextMenuItem_Click(object sender, EventArgs e)
            => this.PostBrowser.Document.ExecCommand("SelectAll", false, null); // 発言詳細ですべて選択

        private async void FollowContextMenuItem_Click(object sender, EventArgs e)
        {
            var name = this.GetUserId();
            if (name != null)
                await this.Owner.FollowCommand(name);
        }

        private async void RemoveContextMenuItem_Click(object sender, EventArgs e)
        {
            var name = this.GetUserId();
            if (name != null)
                await this.Owner.RemoveCommand(name, false);
        }

        private async void FriendshipContextMenuItem_Click(object sender, EventArgs e)
        {
            var name = this.GetUserId();
            if (name != null)
                await this.Owner.ShowFriendship(name);
        }

        private async void FriendshipAllMenuItem_Click(object sender, EventArgs e)
        {
            var ma = Regex.Matches(this.PostBrowser.DocumentText, @"href=""https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?""");
            var ids = new List<string>();
            foreach (Match mu in ma)
            {
                if (!mu.Result("${ScreenName}").Equals(this.Owner.TwitterInstance.Username, StringComparison.InvariantCultureIgnoreCase))
                {
                    ids.Add(mu.Result("${ScreenName}"));
                }
            }

            await this.Owner.ShowFriendship(ids.ToArray());
        }

        private async void ShowUserStatusContextMenuItem_Click(object sender, EventArgs e)
        {
            var name = this.GetUserId();
            if (name != null)
                await this.Owner.ShowUserStatus(name);
        }

        private async void SearchPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var name = this.GetUserId();
            if (name != null)
                await this.Owner.AddNewTabForUserTimeline(name);
        }

        private void SearchAtPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var name = this.GetUserId();
            if (name != null) this.Owner.AddNewTabForSearch("@" + name);
        }

        private void IdFilterAddMenuItem_Click(object sender, EventArgs e)
        {
            var name = this.GetUserId();
            if (name != null)
                this.Owner.AddFilterRuleByScreenName(name);
        }

        private void ListManageUserContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;

            string? user;
            if (menuItem.Owner == this.ContextMenuPostBrowser)
            {
                user = this.GetUserId();
                if (user == null) return;
            }
            else if (this.CurrentPost != null)
            {
                user = this.CurrentPost.ScreenName;
            }
            else
            {
                return;
            }

            this.Owner.ListManageUserContext(user);
        }

        private void UseHashtagMenuItem_Click(object sender, EventArgs e)
        {
            var m = Regex.Match(this.postBrowserStatusText, @"^https?://twitter.com/search\?q=%23(?<hash>.+)$");
            if (m.Success)
                this.Owner.SetPermanentHashtag(Uri.UnescapeDataString(m.Groups["hash"].Value));
        }

        private async void SelectionTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var text = this.PostBrowser.GetSelectedText();
            await this.DoTranslation(text);
        }

        private async void TranslationToolStripMenuItem_Click(object sender, EventArgs e)
            => await this.DoTranslation();

        #endregion

        #region ContextMenuSource

        private void ContextMenuSource_Opening(object sender, CancelEventArgs e)
        {
            if (this.CurrentPost == null || this.CurrentPost.IsDeleted || this.CurrentPost.IsDm)
            {
                this.SourceCopyMenuItem.Enabled = false;
                this.SourceUrlCopyMenuItem.Enabled = false;
            }
            else
            {
                this.SourceCopyMenuItem.Enabled = true;
                this.SourceUrlCopyMenuItem.Enabled = true;
            }
        }

        private void SourceCopyMenuItem_Click(object sender, EventArgs e)
        {
            if (this.CurrentPost == null)
                return;

            try
            {
                Clipboard.SetDataObject(this.CurrentPost.Source, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SourceUrlCopyMenuItem_Click(object sender, EventArgs e)
        {
            var sourceUri = this.CurrentPost?.SourceUri;
            if (sourceUri == null)
                return;

            try
            {
                Clipboard.SetDataObject(sourceUri.AbsoluteUri, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private async void NameLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var screenName = this.CurrentPost?.ScreenName;
            if (screenName != null)
                await this.Owner.ShowUserStatus(screenName, showInputDialog: false);
        }

        private async void DateTimeLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.CurrentPost == null)
                return;

            var statusUrl = MyCommon.GetStatusUrl(this.CurrentPost);
            await MyCommon.OpenInBrowserAsync(this, statusUrl);
        }
    }

    public class TweetDetailsViewStatusChengedEventArgs : EventArgs
    {
        /// <summary>ステータスバーに表示するテキスト</summary>
        /// <remarks>
        /// 空文字列の場合は <see cref="TweenMain"/> の既定のテキストを表示する
        /// </remarks>
        public string StatusText { get; }

        public TweetDetailsViewStatusChengedEventArgs(string statusText)
            => this.StatusText = statusText;
    }
}
