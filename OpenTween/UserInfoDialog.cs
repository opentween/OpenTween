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
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using OpenTween.Api;
using OpenTween.Api.DataModel;
using OpenTween.Connection;

namespace OpenTween
{
    public partial class UserInfoDialog : OTBaseForm
    {
        private TwitterUser displayUser = null!;
        private CancellationTokenSource? cancellationTokenSource = null;

        private readonly TweenMain mainForm;
        private readonly TwitterApi twitterApi;

        public UserInfoDialog(TweenMain mainForm, TwitterApi twitterApi)
        {
            this.mainForm = mainForm;
            this.twitterApi = twitterApi;

            this.InitializeComponent();

            // LabelScreenName のフォントを OTBaseForm.GlobalFont に変更
            this.LabelScreenName.Font = this.ReplaceToGlobalFont(this.LabelScreenName.Font);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        private void CancelLoading()
        {
            CancellationTokenSource? oldTokenSource = null;
            try
            {
                var newTokenSource = new CancellationTokenSource();
                oldTokenSource = Interlocked.Exchange(ref this.cancellationTokenSource, newTokenSource);
            }
            finally
            {
                if (oldTokenSource != null)
                {
                    oldTokenSource.Cancel();
                    oldTokenSource.Dispose();
                }
            }
        }

        public async Task ShowUserAsync(TwitterUser user)
        {
            if (this.IsDisposed)
                return;

            if (user == null || user == this.displayUser)
                return;

            this.CancelLoading();

            var cancellationToken = this.cancellationTokenSource!.Token;

            this.displayUser = user;

            this.LabelId.Text = user.IdStr;
            this.LabelScreenName.Text = user.ScreenName;
            this.LabelName.Text = user.Name;
            this.LabelLocation.Text = user.Location ?? "";
            this.LabelCreatedAt.Text = MyCommon.DateTimeParse(user.CreatedAt).ToLocalTimeString();

            if (user.Protected)
                this.LabelIsProtected.Text = Properties.Resources.Yes;
            else
                this.LabelIsProtected.Text = Properties.Resources.No;

            if (user.Verified)
                this.LabelIsVerified.Text = Properties.Resources.Yes;
            else
                this.LabelIsVerified.Text = Properties.Resources.No;

            var followingUrl = "https://twitter.com/" + user.ScreenName + "/following";
            this.LinkLabelFollowing.Text = user.FriendsCount.ToString();
            this.LinkLabelFollowing.Tag = followingUrl;
            this.ToolTip1.SetToolTip(this.LinkLabelFollowing, followingUrl);

            var followersUrl = "https://twitter.com/" + user.ScreenName + "/followers";
            this.LinkLabelFollowers.Text = user.FollowersCount.ToString();
            this.LinkLabelFollowers.Tag = followersUrl;
            this.ToolTip1.SetToolTip(this.LinkLabelFollowers, followersUrl);

            var favoritesUrl = "https://twitter.com/" + user.ScreenName + "/favorites";
            this.LinkLabelFav.Text = user.FavouritesCount.ToString();
            this.LinkLabelFav.Tag = favoritesUrl;
            this.ToolTip1.SetToolTip(this.LinkLabelFav, favoritesUrl);

            var profileUrl = "https://twitter.com/" + user.ScreenName;
            this.LinkLabelTweet.Text = user.StatusesCount.ToString();
            this.LinkLabelTweet.Tag = profileUrl;
            this.ToolTip1.SetToolTip(this.LinkLabelTweet, profileUrl);

            if (this.twitterApi.CurrentUserId == user.Id)
            {
                this.ButtonEdit.Enabled = true;
                this.ChangeIconToolStripMenuItem.Enabled = true;
                this.ButtonBlock.Enabled = false;
                this.ButtonReportSpam.Enabled = false;
                this.ButtonBlockDestroy.Enabled = false;
            }
            else
            {
                this.ButtonEdit.Enabled = false;
                this.ChangeIconToolStripMenuItem.Enabled = false;
                this.ButtonBlock.Enabled = true;
                this.ButtonReportSpam.Enabled = true;
                this.ButtonBlockDestroy.Enabled = true;
            }

            await Task.WhenAll(new[]
            {
                this.SetDescriptionAsync(user.Description, user.Entities?.Description, cancellationToken),
                this.SetRecentStatusAsync(user.Status, cancellationToken),
                this.SetLinkLabelWebAsync(user.Url, user.Entities?.Url, cancellationToken),
                this.SetUserImageAsync(user.ProfileImageUrlHttps, cancellationToken),
                this.LoadFriendshipAsync(user.ScreenName, cancellationToken),
            });
        }

        private async Task SetDescriptionAsync(string? descriptionText, TwitterEntities? entities, CancellationToken cancellationToken)
        {
            if (descriptionText != null)
            {
                var urlEntities = entities?.Urls ?? Array.Empty<TwitterEntityUrl>();

                foreach (var entity in urlEntities)
                    entity.ExpandedUrl = await ShortUrl.Instance.ExpandUrlAsync(entity.ExpandedUrl);

                // user.entities には urls 以外のエンティティが含まれていないため、テキストをもとに生成する
                var mergedEntities = urlEntities.AsEnumerable<TwitterEntity>()
                    .Concat(TweetExtractor.ExtractHashtagEntities(descriptionText))
                    .Concat(TweetExtractor.ExtractMentionEntities(descriptionText))
                    .Concat(TweetExtractor.ExtractEmojiEntities(descriptionText));

                var html = TweetFormatter.AutoLinkHtml(descriptionText, mergedEntities);
                html = this.mainForm.CreateDetailHtml(html);

                if (cancellationToken.IsCancellationRequested)
                    return;

                this.DescriptionBrowser.DocumentText = html;
            }
            else
            {
                this.DescriptionBrowser.DocumentText = this.mainForm.CreateDetailHtml("");
            }
        }

        private async Task SetUserImageAsync(string imageUri, CancellationToken cancellationToken)
        {
            var oldImage = this.UserPicture.Image;
            this.UserPicture.Image = null;
            oldImage?.Dispose();

            // ProfileImageUrlHttps が null になる場合があるらしい
            // 参照: https://sourceforge.jp/ticket/browse.php?group_id=6526&tid=33871
            if (imageUri == null)
                return;

            await this.UserPicture.SetImageFromTask(async () =>
            {
                var sizeName = Twitter.DecideProfileImageSize(this.UserPicture.Width);
                var uri = Twitter.CreateProfileImageUrl(imageUri, sizeName);

                using var imageStream = await Networking.Http.GetStreamAsync(uri)
                    .ConfigureAwait(false);

                var image = await MemoryImage.CopyFromStreamAsync(imageStream)
                    .ConfigureAwait(false);

                if (cancellationToken.IsCancellationRequested)
                {
                    image.Dispose();
                    throw new OperationCanceledException(cancellationToken);
                }

                return image;
            });
        }

        private async Task SetLinkLabelWebAsync(string? uri, TwitterEntities? entities, CancellationToken cancellationToken)
        {
            if (uri != null)
            {
                var origUrl = entities?.Urls?.FirstOrDefault()?.ExpandedUrl ?? uri;
                var expandedUrl = await ShortUrl.Instance.ExpandUrlAsync(origUrl);

                if (cancellationToken.IsCancellationRequested)
                    return;

                this.LinkLabelWeb.Text = expandedUrl;
                this.LinkLabelWeb.Tag = expandedUrl;
                this.ToolTip1.SetToolTip(this.LinkLabelWeb, expandedUrl);
            }
            else
            {
                this.LinkLabelWeb.Text = "";
                this.LinkLabelWeb.Tag = null;
                this.ToolTip1.SetToolTip(this.LinkLabelWeb, null);
            }
        }

        private async Task SetRecentStatusAsync(TwitterStatus? status, CancellationToken cancellationToken)
        {
            if (status != null)
            {
                var entities = status.MergedEntities;
                var urlEntities = entities.Urls ?? Array.Empty<TwitterEntityUrl>();

                foreach (var entity in urlEntities)
                    entity.ExpandedUrl = await ShortUrl.Instance.ExpandUrlAsync(entity.ExpandedUrl);

                var mergedEntities = entities.Concat(TweetExtractor.ExtractEmojiEntities(status.FullText));

                var html = TweetFormatter.AutoLinkHtml(status.FullText, mergedEntities);
                html = this.mainForm.CreateDetailHtml(html +
                    " Posted at " + MyCommon.DateTimeParse(status.CreatedAt).ToLocalTimeString() +
                    " via " + status.Source);

                if (cancellationToken.IsCancellationRequested)
                    return;

                this.RecentPostBrowser.DocumentText = html;
            }
            else
            {
                this.RecentPostBrowser.DocumentText = this.mainForm.CreateDetailHtml(Properties.Resources.ShowUserInfo2);
            }
        }

        private async Task LoadFriendshipAsync(string screenName, CancellationToken cancellationToken)
        {
            this.LabelIsFollowing.Text = "";
            this.LabelIsFollowed.Text = "";
            this.ButtonFollow.Enabled = false;
            this.ButtonUnFollow.Enabled = false;

            if (this.twitterApi.CurrentScreenName == screenName)
                return;

            TwitterFriendship friendship;
            try
            {
                friendship = await this.twitterApi.FriendshipsShow(this.twitterApi.CurrentScreenName, screenName);
            }
            catch (WebApiException)
            {
                this.LabelIsFollowed.Text = Properties.Resources.GetFriendshipInfo6;
                this.LabelIsFollowing.Text = Properties.Resources.GetFriendshipInfo6;
                return;
            }

            if (cancellationToken.IsCancellationRequested)
                return;

            var isFollowing = friendship.Relationship.Source.Following;
            var isFollowedBy = friendship.Relationship.Source.FollowedBy;

            this.LabelIsFollowing.Text = isFollowing
                ? Properties.Resources.GetFriendshipInfo1
                : Properties.Resources.GetFriendshipInfo2;

            this.LabelIsFollowed.Text = isFollowedBy
                ? Properties.Resources.GetFriendshipInfo3
                : Properties.Resources.GetFriendshipInfo4;

            this.ButtonFollow.Enabled = !isFollowing;
            this.ButtonUnFollow.Enabled = isFollowing;
        }

        private void ShowUserInfo_Load(object sender, EventArgs e)
        {
            this.TextBoxName.Location = this.LabelName.Location;
            this.TextBoxName.Height = this.LabelName.Height;
            this.TextBoxName.Width = this.LabelName.Width;
            this.TextBoxName.BackColor = this.mainForm.InputBackColor;

            this.TextBoxLocation.Location = this.LabelLocation.Location;
            this.TextBoxLocation.Height = this.LabelLocation.Height;
            this.TextBoxLocation.Width = this.LabelLocation.Width;
            this.TextBoxLocation.BackColor = this.mainForm.InputBackColor;

            this.TextBoxWeb.Location = this.LinkLabelWeb.Location;
            this.TextBoxWeb.Height = this.LinkLabelWeb.Height;
            this.TextBoxWeb.Width = this.LinkLabelWeb.Width;
            this.TextBoxWeb.BackColor = this.mainForm.InputBackColor;

            this.TextBoxDescription.Location = this.DescriptionBrowser.Location;
            this.TextBoxDescription.Height = this.DescriptionBrowser.Height;
            this.TextBoxDescription.Width = this.DescriptionBrowser.Width;
            this.TextBoxDescription.BackColor = this.mainForm.InputBackColor;
            this.TextBoxDescription.Multiline = true;
            this.TextBoxDescription.ScrollBars = ScrollBars.Vertical;
        }

        private async void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var linkLabel = (LinkLabel)sender;

            var linkUrl = (string)linkLabel.Tag;
            if (linkUrl == null)
                return;

            await MyCommon.OpenInBrowserAsync(this, linkUrl);
        }

        private async void ButtonFollow_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this.ButtonFollow))
            {
                try
                {
                    await this.twitterApi.FriendshipsCreate(this.displayUser.ScreenName)
                        .IgnoreResponse();
                }
                catch (WebApiException ex)
                {
                    MessageBox.Show(Properties.Resources.FRMessage2 + ex.Message);
                    return;
                }
            }

            MessageBox.Show(Properties.Resources.FRMessage3);
            this.LabelIsFollowing.Text = Properties.Resources.GetFriendshipInfo1;
            this.ButtonFollow.Enabled = false;
            this.ButtonUnFollow.Enabled = true;
        }

        private async void ButtonUnFollow_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                this.displayUser.ScreenName + Properties.Resources.ButtonUnFollow_ClickText1,
                Properties.Resources.ButtonUnFollow_ClickText2,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                using (ControlTransaction.Disabled(this.ButtonUnFollow))
                {
                    try
                    {
                        await this.twitterApi.FriendshipsDestroy(this.displayUser.ScreenName)
                            .IgnoreResponse();
                    }
                    catch (WebApiException ex)
                    {
                        MessageBox.Show(Properties.Resources.FRMessage2 + ex.Message);
                        return;
                    }
                }

                MessageBox.Show(Properties.Resources.FRMessage3);
                this.LabelIsFollowing.Text = Properties.Resources.GetFriendshipInfo2;
                this.ButtonFollow.Enabled = true;
                this.ButtonUnFollow.Enabled = false;
            }
        }

        private void ShowUserInfo_Activated(object sender, EventArgs e)
        {
            // 画面が他画面の裏に隠れると、アイコン画像が再描画されない問題の対応
            if (this.UserPicture.Image != null)
                this.UserPicture.Invalidate(false);
        }

        private void ShowUserInfo_Shown(object sender, EventArgs e)
            => this.ButtonClose.Focus();

        private async void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.AbsoluteUri != "about:blank")
            {
                e.Cancel = true;

                // Ctrlを押しながらリンクを開いた場合は、設定と逆の動作をするフラグを true としておく
                await this.mainForm.OpenUriAsync(e.Url, MyCommon.IsKeyDown(Keys.Control));
            }
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var browser = (WebBrowser)this.ContextMenuRecentPostBrowser.SourceControl;
            browser.Document.ExecCommand("SelectAll", false, null);
        }

        private void SelectionCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var browser = (WebBrowser)this.ContextMenuRecentPostBrowser.SourceControl;
            var selectedText = browser.GetSelectedText();
            if (selectedText != null)
            {
                try
                {
                    Clipboard.SetDataObject(selectedText, false, 5, 100);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ContextMenuRecentPostBrowser_Opening(object sender, CancelEventArgs e)
        {
            var browser = (WebBrowser)this.ContextMenuRecentPostBrowser.SourceControl;
            var selectedText = browser.GetSelectedText();

            this.SelectionCopyToolStripMenuItem.Enabled = selectedText != null;
        }

        private async void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            => await MyCommon.OpenInBrowserAsync(this, "https://support.twitter.com/groups/31-twitter-basics/topics/111-features/articles/268350-x8a8d-x8a3c-x6e08-x307f-x30a2-x30ab-x30a6-x30f3-x30c8-x306b-x3064-x3044-x3066");

        private async void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            => await MyCommon.OpenInBrowserAsync(this, "https://support.twitter.com/groups/31-twitter-basics/topics/107-my-profile-account-settings/articles/243055-x516c-x958b-x3001-x975e-x516c-x958b-x30a2-x30ab-x30a6-x30f3-x30c8-x306b-x3064-x3044-x3066");

        private async void ButtonSearchPosts_Click(object sender, EventArgs e)
            => await this.mainForm.AddNewTabForUserTimeline(this.displayUser.ScreenName);

        private async void UserPicture_Click(object sender, EventArgs e)
        {
            var imageUrl = this.displayUser.ProfileImageUrlHttps;
            imageUrl = Twitter.CreateProfileImageUrl(imageUrl, "original");

            await MyCommon.OpenInBrowserAsync(this, imageUrl);
        }

        private bool isEditing = false;
        private string buttonEditText = "";

        private async void ButtonEdit_Click(object sender, EventArgs e)
        {
            // 自分以外のプロフィールは変更できない
            if (this.twitterApi.CurrentUserId != this.displayUser.Id)
                return;

            using (ControlTransaction.Disabled(this.ButtonEdit))
            {
                if (!this.isEditing)
                {
                    this.buttonEditText = this.ButtonEdit.Text;
                    this.ButtonEdit.Text = Properties.Resources.UserInfoButtonEdit_ClickText1;

                    this.TextBoxName.Text = this.LabelName.Text;
                    this.TextBoxName.Enabled = true;
                    this.TextBoxName.Visible = true;
                    this.LabelName.Visible = false;

                    this.TextBoxLocation.Text = this.LabelLocation.Text;
                    this.TextBoxLocation.Enabled = true;
                    this.TextBoxLocation.Visible = true;
                    this.LabelLocation.Visible = false;

                    this.TextBoxWeb.Text = this.displayUser.Url;
                    this.TextBoxWeb.Enabled = true;
                    this.TextBoxWeb.Visible = true;
                    this.LinkLabelWeb.Visible = false;

                    this.TextBoxDescription.Text = this.displayUser.Description;
                    this.TextBoxDescription.Enabled = true;
                    this.TextBoxDescription.Visible = true;
                    this.DescriptionBrowser.Visible = false;

                    this.TextBoxName.Focus();
                    this.TextBoxName.Select(this.TextBoxName.Text.Length, 0);

                    this.isEditing = true;
                }
                else
                {
                    Task? showUserTask = null;

                    if (this.TextBoxName.Modified ||
                        this.TextBoxLocation.Modified ||
                        this.TextBoxWeb.Modified ||
                        this.TextBoxDescription.Modified)
                    {
                        try
                        {
                            var response = await this.twitterApi.AccountUpdateProfile(
                                this.TextBoxName.Text,
                                this.TextBoxWeb.Text,
                                this.TextBoxLocation.Text,
                                this.TextBoxDescription.Text);

                            var user = await response.LoadJsonAsync();
                            showUserTask = this.ShowUserAsync(user);
                        }
                        catch (WebApiException ex)
                        {
                            MessageBox.Show($"Err:{ex.Message}(AccountUpdateProfile)");
                            return;
                        }
                    }

                    this.TextBoxName.Enabled = false;
                    this.TextBoxName.Visible = false;
                    this.LabelName.Visible = true;

                    this.TextBoxLocation.Enabled = false;
                    this.TextBoxLocation.Visible = false;
                    this.LabelLocation.Visible = true;

                    this.TextBoxWeb.Enabled = false;
                    this.TextBoxWeb.Visible = false;
                    this.LinkLabelWeb.Visible = true;

                    this.TextBoxDescription.Enabled = false;
                    this.TextBoxDescription.Visible = false;
                    this.DescriptionBrowser.Visible = true;

                    this.ButtonEdit.Text = this.buttonEditText;

                    this.isEditing = false;

                    if (showUserTask != null)
                        await showUserTask;
                }
            }
        }

        private async Task DoChangeIcon(string filename)
        {
            try
            {
                var mediaItem = new FileMediaItem(filename);

                await this.twitterApi.AccountUpdateProfileImage(mediaItem)
                    .IgnoreResponse();
            }
            catch (WebApiException ex)
            {
                MessageBox.Show("Err:" + ex.Message + Environment.NewLine + Properties.Resources.ChangeIconToolStripMenuItem_ClickText4);
                return;
            }

            MessageBox.Show(Properties.Resources.ChangeIconToolStripMenuItem_ClickText5);

            try
            {
                var user = await this.twitterApi.UsersShow(this.displayUser.ScreenName);

                if (user != null)
                    await this.ShowUserAsync(user);
            }
            catch (WebApiException ex)
            {
                MessageBox.Show($"Err:{ex.Message}(UsersShow)");
            }
        }

        private async void ChangeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenFileDialogIcon.Filter = Properties.Resources.ChangeIconToolStripMenuItem_ClickText1;
            this.OpenFileDialogIcon.Title = Properties.Resources.ChangeIconToolStripMenuItem_ClickText2;
            this.OpenFileDialogIcon.FileName = "";

            var rslt = this.OpenFileDialogIcon.ShowDialog();

            if (rslt != DialogResult.OK)
            {
                return;
            }

            var fn = this.OpenFileDialogIcon.FileName;
            if (this.IsValidIconFile(new FileInfo(fn)))
            {
                await this.DoChangeIcon(fn);
            }
            else
            {
                MessageBox.Show(Properties.Resources.ChangeIconToolStripMenuItem_ClickText6);
            }
        }

        private async void ButtonBlock_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                this.displayUser.ScreenName + Properties.Resources.ButtonBlock_ClickText1,
                Properties.Resources.ButtonBlock_ClickText2,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                using (ControlTransaction.Disabled(this.ButtonBlock))
                {
                    try
                    {
                        await this.twitterApi.BlocksCreate(this.displayUser.ScreenName)
                            .IgnoreResponse();
                    }
                    catch (WebApiException ex)
                    {
                        MessageBox.Show("Err:" + ex.Message + Environment.NewLine + Properties.Resources.ButtonBlock_ClickText3);
                        return;
                    }

                    MessageBox.Show(Properties.Resources.ButtonBlock_ClickText4);
                }
            }
        }

        private async void ButtonReportSpam_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                this.displayUser.ScreenName + Properties.Resources.ButtonReportSpam_ClickText1,
                Properties.Resources.ButtonReportSpam_ClickText2,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                using (ControlTransaction.Disabled(this.ButtonReportSpam))
                {
                    try
                    {
                        await this.twitterApi.UsersReportSpam(this.displayUser.ScreenName)
                            .IgnoreResponse();
                    }
                    catch (WebApiException ex)
                    {
                        MessageBox.Show("Err:" + ex.Message + Environment.NewLine + Properties.Resources.ButtonReportSpam_ClickText3);
                        return;
                    }

                    MessageBox.Show(Properties.Resources.ButtonReportSpam_ClickText4);
                }
            }
        }

        private async void ButtonBlockDestroy_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                this.displayUser.ScreenName + Properties.Resources.ButtonBlockDestroy_ClickText1,
                Properties.Resources.ButtonBlockDestroy_ClickText2,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                using (ControlTransaction.Disabled(this.ButtonBlockDestroy))
                {
                    try
                    {
                        await this.twitterApi.BlocksDestroy(this.displayUser.ScreenName)
                            .IgnoreResponse();
                    }
                    catch (WebApiException ex)
                    {
                        MessageBox.Show("Err:" + ex.Message + Environment.NewLine + Properties.Resources.ButtonBlockDestroy_ClickText3);
                        return;
                    }

                    MessageBox.Show(Properties.Resources.ButtonBlockDestroy_ClickText4);
                }
            }
        }

        private bool IsValidExtension(string ext)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            return allowedExtensions.Contains(ext, StringComparer.InvariantCultureIgnoreCase);
        }

        private bool IsValidIconFile(FileInfo info)
        {
            return this.IsValidExtension(info.Extension) &&
                info.Length < 700 * 1024 &&
                !MyCommon.IsAnimatedGif(info.FullName);
        }

        private void ShowUserInfo_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                !e.Data.GetDataPresent(DataFormats.Html, false)) // WebBrowserコントロールからの絵文字画像D&Dは弾く
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                if (files.Length != 1)
                    return;

                var finfo = new FileInfo(files[0]);
                if (this.IsValidIconFile(finfo))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private async void ShowUserInfo_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                !e.Data.GetDataPresent(DataFormats.Html, false)) // WebBrowserコントロールからの絵文字画像D&Dは弾く
            {
                var ret = MessageBox.Show(
                    this,
                    Properties.Resources.ChangeIconToolStripMenuItem_Confirm,
                    ApplicationSettings.ApplicationName,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);
                if (ret != DialogResult.OK)
                    return;

                var filename = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                await this.DoChangeIcon(filename);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var cts = this.cancellationTokenSource;
                cts?.Cancel();
                cts?.Dispose();

                var oldImage = this.UserPicture.Image;
                this.UserPicture.Image = null;
                oldImage?.Dispose();

                this.components?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
