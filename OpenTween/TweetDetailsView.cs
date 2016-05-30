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

namespace OpenTween
{
    public partial class TweetDetailsView : UserControl
    {
        public TweenMain Owner { get; set; }

        /// <summary>プロフィール画像のキャッシュ</summary>
        public ImageCache IconCache { get; set; }

        /// <summary><see cref="PostClass"/> のダンプを表示するか</summary>
        public bool DumpPostClass { get; set; }

        /// <summary>現在表示中の発言</summary>
        public PostClass CurrentPost { get; private set; }

        [DefaultValue(false)]
        public new bool TabStop
        {
            get { return base.TabStop; }
            set { base.TabStop = value; }
        }

        /// <summary>ステータスバーに表示するテキストの変化を通知するイベント</summary>
        public event EventHandler<TweetDetailsViewStatusChengedEventArgs> StatusChanged;

        /// <summary><see cref="ContextMenuPostBrowser"/> 展開時の <see cref="PostBrowser"/>.StatusText を保持するフィールド</summary>
        private string _postBrowserStatusText = "";

        public TweetDetailsView()
        {
            this.InitializeComponent();

            this.TabStop = false;

            //発言詳細部の初期化
            NameLabel.Text = "";
            DateTimeLabel.Text = "";
            SourceLinkLabel.Text = "";

            new InternetSecurityManager(PostBrowser);
            this.PostBrowser.AllowWebBrowserDrop = false;  // COMException を回避するため、ActiveX の初期化が終わってから設定する
        }

        public async Task ShowPostDetails(PostClass post)
        {
            this.CurrentPost = post;

            var loadTasks = new List<Task>();

            using (ControlTransaction.Update(this.TableLayoutPanel1))
            {
                SourceLinkLabel.Text = post.Source;
                SourceLinkLabel.Tag = post.SourceUri;
                SourceLinkLabel.TabStop = false; // Text を更新すると勝手に true にされる

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
                    nameText += " (RT:" + post.RetweetedBy + ")";

                NameLabel.Text = nameText;
                NameLabel.Tag = post.ScreenName;

                var nameForeColor = SystemColors.ControlText;
                if (post.IsOwl && (SettingCommon.Instance.OneWayLove || post.IsDm))
                    nameForeColor = this.Owner._cfgLocal.ColorOWL;
                if (post.RetweetedId != null)
                    nameForeColor = this.Owner._cfgLocal.ColorRetweet;
                if (post.IsFav)
                    nameForeColor = this.Owner._cfgLocal.ColorFav;
                NameLabel.ForeColor = nameForeColor;

                loadTasks.Add(this.SetUserPictureAsync(post.ImageUrl));

                DateTimeLabel.Text = post.CreatedAt.ToString();
            }

            if (this.DumpPostClass)
            {
                StringBuilder sb = new StringBuilder(512);

                sb.Append("-----Start PostClass Dump<br>");
                sb.AppendFormat("TextFromApi           : {0}<br>", post.TextFromApi);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", post.TextFromApi);
                sb.AppendFormat("StatusId             : {0}<br>", post.StatusId);
                //sb.AppendFormat("ImageIndex     : {0}<br>", post.ImageIndex);
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

                foreach (string nm in post.ReplyToList)
                {
                    sb.AppendFormat("ReplyToList    : {0}<br>", nm);
                }

                sb.AppendFormat("ScreenName           : {0}<br>", post.ScreenName);
                sb.AppendFormat("NickName       : {0}<br>", post.Nickname);
                sb.AppendFormat("Text   : {0}<br>", post.Text);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", post.Text);
                sb.AppendFormat("CreatedAt          : {0}<br>", post.CreatedAt);
                sb.AppendFormat("Source         : {0}<br>", post.Source);
                sb.AppendFormat("UserId            : {0}<br>", post.UserId);
                sb.AppendFormat("FilterHit      : {0}<br>", post.FilterHit);
                sb.AppendFormat("RetweetedBy    : {0}<br>", post.RetweetedBy);
                sb.AppendFormat("RetweetedId    : {0}<br>", post.RetweetedId);

                sb.AppendFormat("Media.Count    : {0}<br>", post.Media.Count);
                if (post.Media.Count > 0)
                {
                    for (int i = 0; i < post.Media.Count; i++)
                    {
                        var info = post.Media[i];
                        sb.AppendFormat("Media[{0}].Url         : {1}<br>", i, info.Url);
                        sb.AppendFormat("Media[{0}].VideoUrl    : {1}<br>", i, info.VideoUrl ?? "---");
                    }
                }
                sb.Append("-----End PostClass Dump<br>");

                PostBrowser.DocumentText = this.Owner.createDetailHtml(sb.ToString());
                return;
            }

            using (ControlTransaction.Update(this.PostBrowser))
            {
                this.PostBrowser.DocumentText =
                    this.Owner.createDetailHtml(post.IsDeleted ? "(DELETED)" : post.Text);

                this.PostBrowser.Document.Window.ScrollTo(0, 0);
            }

            loadTasks.Add(this.AppendQuoteTweetAsync(post));

            await Task.WhenAll(loadTasks);
        }

        public void ScrollDownPostBrowser(bool forward)
        {
            var doc = PostBrowser.Document;
            if (doc == null) return;

            var tags = doc.GetElementsByTagName("html");
            if (tags.Count > 0)
            {
                if (forward)
                    tags[0].ScrollTop += this.Owner._cfgLocal.FontDetail.Height;
                else
                    tags[0].ScrollTop -= this.Owner._cfgLocal.FontDetail.Height;
            }
        }

        public void PageDownPostBrowser(bool forward)
        {
            var doc = PostBrowser.Document;
            if (doc == null) return;

            var tags = doc.GetElementsByTagName("html");
            if (tags.Count > 0)
            {
                if (forward)
                    tags[0].ScrollTop += PostBrowser.ClientRectangle.Height - this.Owner._cfgLocal.FontDetail.Height;
                else
                    tags[0].ScrollTop -= PostBrowser.ClientRectangle.Height - this.Owner._cfgLocal.FontDetail.Height;
            }
        }

        public HtmlElement[] GetLinkElements()
        {
            return this.PostBrowser.Document.Links.Cast<HtmlElement>()
                .Where(x => x.GetAttribute("className") != "tweet-quote-link") // 引用ツイートで追加されたリンクを除く
                .ToArray();
        }

        private async Task SetUserPictureAsync(string imageUrl, bool force = false)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            if (this.IconCache == null)
                return;

            this.ClearUserPicture();

            await this.UserPicture.SetImageFromTask(async () =>
            {
                var image = await this.IconCache.DownloadImageAsync(imageUrl, force)
                    .ConfigureAwait(false);

                return await image.CloneAsync()
                    .ConfigureAwait(false);
            });
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
                this.PostBrowser.DocumentText = this.Owner.createDetailHtml(body);

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
                this.PostBrowser.DocumentText = this.Owner.createDetailHtml(body);
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
                WebUtility.HtmlEncode(post.CreatedAt.ToString());

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
        {
            // a 要素はネストされていない前提の正規表現パターン
            return Regex.Replace(html, @"<a[^>]*>(.*?)</a>", "$1");
        }

        public async Task DoTranslation()
        {
            if (this.CurrentPost == null || this.CurrentPost.IsDeleted)
                return;

            await this.DoTranslation(this.CurrentPost.TextFromApi);
        }

        private async Task DoTranslation(string str)
        {
            if (string.IsNullOrEmpty(str))
                return;

            var bing = new Bing();
            try
            {
                var translatedText = await bing.TranslateAsync(str,
                    langFrom: null,
                    langTo: SettingCommon.Instance.TranslateLanguage);

                this.PostBrowser.DocumentText = this.Owner.createDetailHtml(translatedText);
            }
            catch (HttpRequestException e)
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
            //発言詳細で「選択文字列で検索」（選択文字列取得）
            string _selText = this.PostBrowser.GetSelectedText();

            if (_selText != null)
            {
                if (url == Properties.Resources.SearchItem4Url)
                {
                    //公式検索
                    this.Owner.AddNewTabForSearch(_selText);
                    return;
                }

                string tmp = string.Format(url, Uri.EscapeDataString(_selText));
                await this.Owner.OpenUriInBrowserAsync(tmp);
            }
        }

        private string GetUserId()
        {
            Match m = Regex.Match(this._postBrowserStatusText, @"^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?$");
            if (m.Success && this.Owner.IsTwitterId(m.Result("${ScreenName}")))
                return m.Result("${ScreenName}");
            else
                return null;
        }

        protected void RaiseStatusChanged(string statusText)
        {
            this.StatusChanged?.Invoke(this, new TweetDetailsViewStatusChengedEventArgs(statusText));
        }

        private void TweetDetailsView_FontChanged(object sender, EventArgs e)
        {
            // OTBaseForm.GlobalFont による UI フォントの変更に対応
            var origFont = this.NameLabel.Font;
            this.NameLabel.Font = new Font(this.Font.Name, origFont.Size, origFont.Style);
        }

        #region TableLayoutPanel1

        private async void UserPicture_DoubleClick(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                await this.Owner.OpenUriInBrowserAsync(MyCommon.TwitterUrl + NameLabel.Tag);
            }
        }

        private void UserPicture_MouseEnter(object sender, EventArgs e)
        {
            this.UserPicture.Cursor = Cursors.Hand;
        }

        private void UserPicture_MouseLeave(object sender, EventArgs e)
        {
            this.UserPicture.Cursor = Cursors.Default;
        }

        private async void PostBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.AbsoluteUri != "about:blank")
            {
                await this.ShowPostDetails(this.CurrentPost); // 現在の発言を表示し直す (Navigated の段階ではキャンセルできない)
                await this.Owner.OpenUriInBrowserAsync(e.Url.OriginalString);
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
            Task asyncTask;
            bool KeyRes = this.Owner.CommonKeyDown(e.KeyData, FocusedControl.PostBrowser, out asyncTask);
            if (KeyRes)
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
                if (PostBrowser.StatusText.StartsWith("http", StringComparison.Ordinal)
                    || PostBrowser.StatusText.StartsWith("ftp", StringComparison.Ordinal)
                    || PostBrowser.StatusText.StartsWith("data", StringComparison.Ordinal))
                {
                    this.RaiseStatusChanged(this.PostBrowser.StatusText.Replace("&", "&&"));
                }
                if (string.IsNullOrEmpty(PostBrowser.StatusText))
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
            var sourceUri = (Uri)this.SourceLinkLabel.Tag;
            if (sourceUri != null && e.Button == MouseButtons.Left)
            {
                await this.Owner.OpenUriInBrowserAsync(sourceUri.AbsoluteUri);
            }
        }

        private void SourceLinkLabel_MouseEnter(object sender, EventArgs e)
        {
            var sourceUri = (Uri)this.SourceLinkLabel.Tag;
            if (sourceUri != null)
            {
                this.RaiseStatusChanged(MyCommon.ConvertToReadableUrl(sourceUri.AbsoluteUri));
            }
        }

        private void SourceLinkLabel_MouseLeave(object sender, EventArgs e)
        {
            this.RaiseStatusChanged(statusText: "");
        }

        #endregion

        #region ContextMenuUserPicture

        private void ContextMenuUserPicture_Opening(object sender, CancelEventArgs e)
        {
            //発言詳細のアイコン右クリック時のメニュー制御
            if (this.CurrentPost != null)
            {
                string name = this.CurrentPost.ImageUrl;
                if (!string.IsNullOrEmpty(name))
                {
                    int idx = name.LastIndexOf('/');
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
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id == this.Owner.TwitterInstance.Username)
                {
                    FollowToolStripMenuItem.Enabled = false;
                    UnFollowToolStripMenuItem.Enabled = false;
                    ShowFriendShipToolStripMenuItem.Enabled = false;
                    ShowUserStatusToolStripMenuItem.Enabled = true;
                    SearchPostsDetailNameToolStripMenuItem.Enabled = true;
                    SearchAtPostsDetailNameToolStripMenuItem.Enabled = false;
                    ListManageUserContextToolStripMenuItem3.Enabled = true;
                }
                else
                {
                    FollowToolStripMenuItem.Enabled = true;
                    UnFollowToolStripMenuItem.Enabled = true;
                    ShowFriendShipToolStripMenuItem.Enabled = true;
                    ShowUserStatusToolStripMenuItem.Enabled = true;
                    SearchPostsDetailNameToolStripMenuItem.Enabled = true;
                    SearchAtPostsDetailNameToolStripMenuItem.Enabled = true;
                    ListManageUserContextToolStripMenuItem3.Enabled = true;
                }
            }
            else
            {
                FollowToolStripMenuItem.Enabled = false;
                UnFollowToolStripMenuItem.Enabled = false;
                ShowFriendShipToolStripMenuItem.Enabled = false;
                ShowUserStatusToolStripMenuItem.Enabled = false;
                SearchPostsDetailNameToolStripMenuItem.Enabled = false;
                SearchAtPostsDetailNameToolStripMenuItem.Enabled = false;
                ListManageUserContextToolStripMenuItem3.Enabled = false;
            }
        }

        private async void FollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != this.Owner.TwitterInstance.Username)
                {
                    await this.Owner.FollowCommand(id);
                }
            }
        }

        private async void UnFollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != this.Owner.TwitterInstance.Username)
                {
                    await this.Owner.RemoveCommand(id, false);
                }
            }
        }

        private async void ShowFriendShipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != this.Owner.TwitterInstance.Username)
                {
                    await this.Owner.ShowFriendship(id);
                }
            }
        }

        // ListManageUserContextToolStripMenuItem3.Click は ListManageUserContextToolStripMenuItem_Click を共用

        private async void ShowUserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                await this.Owner.ShowUserStatus(id, false);
            }
        }

        private void SearchPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                this.Owner.AddNewTabForUserTimeline(id);
            }
        }

        private void SearchAtPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                this.Owner.AddNewTabForSearch("@" + id);
            }
        }

        private async void IconNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var imageUrl = this.CurrentPost?.ImageUrl;
            if (string.IsNullOrEmpty(imageUrl))
                return;

            await this.Owner.OpenUriInBrowserAsync(imageUrl.Remove(imageUrl.LastIndexOf("_normal", StringComparison.Ordinal), 7)); // "_normal".Length
        }

        private async void ReloadIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var imageUrl = this.CurrentPost?.ImageUrl;
            if (string.IsNullOrEmpty(imageUrl))
                return;

            await this.SetUserPictureAsync(imageUrl, force: true);
        }

        private void SaveIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var imageUrl = this.CurrentPost?.ImageUrl;
            if (string.IsNullOrEmpty(imageUrl))
                return;

            this.Owner.SaveFileDialog1.FileName = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);

            if (this.Owner.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Image orgBmp = new Bitmap(IconCache.TryGetFromCache(imageUrl).Image))
                    {
                        using (Bitmap bmp2 = new Bitmap(orgBmp.Size.Width, orgBmp.Size.Height))
                        {
                            using (Graphics g = Graphics.FromImage(bmp2))
                            {
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                                g.DrawImage(orgBmp, 0, 0, orgBmp.Size.Width, orgBmp.Size.Height);
                            }
                            bmp2.Save(this.Owner.SaveFileDialog1.FileName);
                        }
                    }
                }
                catch (Exception)
                {
                    //処理中にキャッシュアウトする可能性あり
                }
            }
        }

        #endregion

        #region ContextMenuPostBrowser

        private void ContextMenuPostBrowser_Opening(object ender, CancelEventArgs e)
        {
            // URLコピーの項目の表示/非表示
            if (PostBrowser.StatusText.StartsWith("http", StringComparison.Ordinal))
            {
                this._postBrowserStatusText = PostBrowser.StatusText;
                string name = GetUserId();
                UrlCopyContextMenuItem.Enabled = true;
                if (name != null)
                {
                    FollowContextMenuItem.Enabled = true;
                    RemoveContextMenuItem.Enabled = true;
                    FriendshipContextMenuItem.Enabled = true;
                    ShowUserStatusContextMenuItem.Enabled = true;
                    SearchPostsDetailToolStripMenuItem.Enabled = true;
                    IdFilterAddMenuItem.Enabled = true;
                    ListManageUserContextToolStripMenuItem.Enabled = true;
                    SearchAtPostsDetailToolStripMenuItem.Enabled = true;
                }
                else
                {
                    FollowContextMenuItem.Enabled = false;
                    RemoveContextMenuItem.Enabled = false;
                    FriendshipContextMenuItem.Enabled = false;
                    ShowUserStatusContextMenuItem.Enabled = false;
                    SearchPostsDetailToolStripMenuItem.Enabled = false;
                    IdFilterAddMenuItem.Enabled = false;
                    ListManageUserContextToolStripMenuItem.Enabled = false;
                    SearchAtPostsDetailToolStripMenuItem.Enabled = false;
                }

                if (Regex.IsMatch(this._postBrowserStatusText, @"^https?://twitter.com/search\?q=%23"))
                    UseHashtagMenuItem.Enabled = true;
                else
                    UseHashtagMenuItem.Enabled = false;
            }
            else
            {
                this._postBrowserStatusText = "";
                UrlCopyContextMenuItem.Enabled = false;
                FollowContextMenuItem.Enabled = false;
                RemoveContextMenuItem.Enabled = false;
                FriendshipContextMenuItem.Enabled = false;
                ShowUserStatusContextMenuItem.Enabled = false;
                SearchPostsDetailToolStripMenuItem.Enabled = false;
                SearchAtPostsDetailToolStripMenuItem.Enabled = false;
                UseHashtagMenuItem.Enabled = false;
                IdFilterAddMenuItem.Enabled = false;
                ListManageUserContextToolStripMenuItem.Enabled = false;
            }
            // 文字列選択されていないときは選択文字列関係の項目を非表示に
            string _selText = this.PostBrowser.GetSelectedText();
            if (_selText == null)
            {
                SelectionSearchContextMenuItem.Enabled = false;
                SelectionCopyContextMenuItem.Enabled = false;
                SelectionTranslationToolStripMenuItem.Enabled = false;
            }
            else
            {
                SelectionSearchContextMenuItem.Enabled = true;
                SelectionCopyContextMenuItem.Enabled = true;
                SelectionTranslationToolStripMenuItem.Enabled = true;
            }
            //発言内に自分以外のユーザーが含まれてればフォロー状態全表示を有効に
            MatchCollection ma = Regex.Matches(this.PostBrowser.DocumentText, @"href=""https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?""");
            bool fAllFlag = false;
            foreach (Match mu in ma)
            {
                if (mu.Result("${ScreenName}").ToLowerInvariant() != this.Owner.TwitterInstance.Username.ToLowerInvariant())
                {
                    fAllFlag = true;
                    break;
                }
            }
            this.FriendshipAllMenuItem.Enabled = fAllFlag;

            if (this.CurrentPost == null)
                TranslationToolStripMenuItem.Enabled = false;
            else
                TranslationToolStripMenuItem.Enabled = true;

            e.Cancel = false;
        }

        private async void SearchGoogleContextMenuItem_Click(object sender, EventArgs e)
        {
            await this.DoSearchToolStrip(Properties.Resources.SearchItem2Url);
        }

        private async void SearchWikipediaContextMenuItem_Click(object sender, EventArgs e)
        {
            await this.DoSearchToolStrip(Properties.Resources.SearchItem1Url);
        }

        private async void SearchPublicSearchContextMenuItem_Click(object sender, EventArgs e)
        {
            await this.DoSearchToolStrip(Properties.Resources.SearchItem4Url);
        }

        private void CurrentTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //発言詳細の選択文字列で現在のタブを検索
            string _selText = this.PostBrowser.GetSelectedText();

            if (_selText != null)
            {
                var searchOptions = new SearchWordDialog.SearchOptions(
                    SearchWordDialog.SearchType.Timeline,
                    _selText,
                    newTab: false,
                    caseSensitive: false,
                    useRegex: false);

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
            //発言詳細で「選択文字列をコピー」
            string _selText = this.PostBrowser.GetSelectedText();
            try
            {
                Clipboard.SetDataObject(_selText, false, 5, 100);
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
                    if (link.GetAttribute("href") == this._postBrowserStatusText)
                    {
                        var linkStr = link.GetAttribute("title");
                        if (string.IsNullOrEmpty(linkStr))
                            linkStr = link.GetAttribute("href");

                        Clipboard.SetDataObject(linkStr, false, 5, 100);
                        return;
                    }
                }

                Clipboard.SetDataObject(this._postBrowserStatusText, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectionAllContextMenuItem_Click(object sender, EventArgs e)
        {
            //発言詳細ですべて選択
            PostBrowser.Document.ExecCommand("SelectAll", false, null);
        }

        private async void FollowContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
                await this.Owner.FollowCommand(name);
        }

        private async void RemoveContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
                await this.Owner.RemoveCommand(name, false);
        }

        private async void FriendshipContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
                await this.Owner.ShowFriendship(name);
        }

        private async void FriendshipAllMenuItem_Click(object sender, EventArgs e)
        {
            MatchCollection ma = Regex.Matches(this.PostBrowser.DocumentText, @"href=""https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?""");
            List<string> ids = new List<string>();
            foreach (Match mu in ma)
            {
                if (mu.Result("${ScreenName}").ToLower() != this.Owner.TwitterInstance.Username.ToLower())
                {
                    ids.Add(mu.Result("${ScreenName}"));
                }
            }

            await this.Owner.ShowFriendship(ids.ToArray());
        }

        private async void ShowUserStatusContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
                await this.Owner.ShowUserStatus(name);
        }

        private void SearchPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null) this.Owner.AddNewTabForUserTimeline(name);
        }

        private void SearchAtPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null) this.Owner.AddNewTabForSearch("@" + name);
        }

        private void IdFilterAddMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
                this.Owner.AddFilterRuleByScreenName(name);
        }

        private async void ListManageUserContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

            string user;
            if (menuItem.Owner == this.ContextMenuPostBrowser)
            {
                user = GetUserId();
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

            await this.Owner.ListManageUserContext(user);
        }

        private void UseHashtagMenuItem_Click(object sender, EventArgs e)
        {
            Match m = Regex.Match(this._postBrowserStatusText, @"^https?://twitter.com/search\?q=%23(?<hash>.+)$");
            if (m.Success)
                this.Owner.SetPermanentHashtag(Uri.UnescapeDataString(m.Groups["hash"].Value));
        }

        private async void SelectionTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var text = this.PostBrowser.GetSelectedText();
            await this.DoTranslation(text);
        }

        private async void TranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await this.DoTranslation();
        }

        #endregion

        #region ContextMenuSource

        private void ContextMenuSource_Opening(object sender, CancelEventArgs e)
        {
            if (this.CurrentPost == null || this.CurrentPost.IsDeleted || this.CurrentPost.IsDm)
            {
                SourceCopyMenuItem.Enabled = false;
                SourceUrlCopyMenuItem.Enabled = false;
            }
            else
            {
                SourceCopyMenuItem.Enabled = true;
                SourceUrlCopyMenuItem.Enabled = true;
            }
        }

        private void SourceCopyMenuItem_Click(object sender, EventArgs e)
        {
            string selText = SourceLinkLabel.Text;
            try
            {
                Clipboard.SetDataObject(selText, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SourceUrlCopyMenuItem_Click(object sender, EventArgs e)
        {
            var sourceUri = (Uri)this.SourceLinkLabel.Tag;
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
    }

    public class TweetDetailsViewStatusChengedEventArgs : EventArgs
    {
        /// <summary>ステータスバーに表示するテキスト</summary>
        /// <remarks>
        /// 空文字列の場合は <see cref="TweenMain"/> の既定のテキストを表示する
        /// </remarks>
        public string StatusText { get; }

        public TweetDetailsViewStatusChengedEventArgs(string statusText)
        {
            this.StatusText = statusText;
        }
    }
}
