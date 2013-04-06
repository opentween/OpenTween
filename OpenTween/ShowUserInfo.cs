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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.Net;

namespace OpenTween
{
    public partial class ShowUserInfo : Form
    {
        public ShowUserInfo()
        {
            InitializeComponent();
        }
        private TwitterDataModel.User userInfo = null;
        private UserInfo _info = new UserInfo();
        private Image icondata = null;
        private List<string> atlist = new List<string>();
        private string descriptionTxt;
        private string recentPostTxt;

        private const string Mainpath = "http://twitter.com/";
        private const string Followingpath = "/following";
        private const string Followerspath = "/followers";
        private const string Favpath = "/favorites";

        private string Home;
        private string Following;
        private string Followers;
        private string Favorites;
        private TweenMain MyOwner;
        private string FriendshipResult = "";

        private void InitPath()
        {
            Home = Mainpath + _info.ScreenName;
            Following = Home + Followingpath;
            Followers = Home + Followerspath;
            Favorites = Home + Favpath;
        }

        private void InitTooltip()
        {
            ToolTip1.SetToolTip(LinkLabelTweet, Home);
            ToolTip1.SetToolTip(LinkLabelFollowing, Following);
            ToolTip1.SetToolTip(LinkLabelFollowers, Followers);
            ToolTip1.SetToolTip(LinkLabelFav, Favorites);
        }

        private bool AnalizeUserInfo(TwitterDataModel.User user)
        {
            if (user == null) return false;

            try
            {
                _info.Id = user.Id;
                _info.Name = WebUtility.HtmlDecode(user.Name).Trim();
                _info.ScreenName = user.ScreenName;
                _info.Location = WebUtility.HtmlDecode(user.Location);
                _info.Description = WebUtility.HtmlDecode(user.Description);
                _info.ImageUrl = new Uri(user.ProfileImageUrlHttps);
                _info.Url = user.Url;
                _info.Protect = user.Protected;
                _info.FriendsCount = user.FriendsCount;
                _info.FollowersCount = user.FollowersCount;
                _info.FavoriteCount = user.FavouritesCount;
                _info.CreatedAt = MyCommon.DateTimeParse(user.CreatedAt);
                _info.StatusesCount = user.StatusesCount;
                _info.Verified = user.Verified;
                try
                {
                    _info.RecentPost = user.Status.Text;
                    _info.PostCreatedAt = MyCommon.DateTimeParse(user.Status.CreatedAt);
                    _info.PostSource = user.Status.Source;
                    if (!_info.PostSource.Contains("</a>"))
                    {
                        _info.PostSource += "</a>";
                    }
                }
                catch (Exception)
                {
                    _info.RecentPost = null;
                    _info.PostCreatedAt = new DateTime();
                    _info.PostSource = null;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void SetLinklabelWeb(string data)
        {
            string webtext;
            string jumpto;
            webtext = MyOwner.TwitterInstance.PreProcessUrl("<a href=\"" + data + "\">Dummy</a>");
            webtext = ShortUrl.Resolve(webtext, false);
            jumpto = Regex.Match(webtext, @"<a href=""(?<url>.*?)""").Groups["url"].Value;
            ToolTip1.SetToolTip(LinkLabelWeb, jumpto);
            LinkLabelWeb.Tag = jumpto;
            LinkLabelWeb.Text = data;
        }

        private string MakeDescriptionBrowserText(string data)
        {
            descriptionTxt = MyOwner.createDetailHtml(
                                    MyOwner.TwitterInstance.CreateHtmlAnchor(WebUtility.HtmlEncode(data), atlist, null));
            return descriptionTxt;
        }

        private void ShowUserInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            //TweenMain.TopMost = !TweenMain.TopMost;
            //TweenMain.TopMost = !TweenMain.TopMost;
        }

        private void ShowUserInfo_Load(object sender, EventArgs e)
        {
            MyOwner = (TweenMain)this.Owner;
            if (!AnalizeUserInfo(userInfo))
            {
                MessageBox.Show(Properties.Resources.ShowUserInfo1);
                this.Close();
                return;
            }

            //アイコンロード
            BackgroundWorkerImageLoader.RunWorkerAsync();

            InitPath();
            InitTooltip();
            this.Text = this.Text.Insert(0, _info.ScreenName + " ");
            LabelId.Text = _info.Id.ToString();
            LabelScreenName.Text = _info.ScreenName;
            LabelName.Text = _info.Name;

            LabelLocation.Text = _info.Location;

            SetLinklabelWeb(_info.Url);

            DescriptionBrowser.Visible = false;
            MakeDescriptionBrowserText(_info.Description);

            RecentPostBrowser.Visible = false;
            if (_info.RecentPost != null)
            {
                recentPostTxt = MyOwner.createDetailHtml(
                     MyOwner.TwitterInstance.CreateHtmlAnchor(ref _info.RecentPost, atlist, userInfo.Status.Entities, null) +
                     " Posted at " + _info.PostCreatedAt.ToString() +
                     " via " + _info.PostSource);
            }

            LinkLabelFollowing.Text = _info.FriendsCount.ToString();
            LinkLabelFollowers.Text = _info.FollowersCount.ToString();
            LinkLabelFav.Text = _info.FavoriteCount.ToString();
            LinkLabelTweet.Text = _info.StatusesCount.ToString();

            LabelCreatedAt.Text = _info.CreatedAt.ToString();

            if (_info.Protect)
                LabelIsProtected.Text = Properties.Resources.Yes;
            else
                LabelIsProtected.Text = Properties.Resources.No;

            if (_info.Verified)
                LabelIsVerified.Text = Properties.Resources.Yes;
            else
                LabelIsVerified.Text = Properties.Resources.No;

            if (MyOwner.TwitterInstance.Username == _info.ScreenName)
            {
                ButtonEdit.Enabled = true;
                ChangeIconToolStripMenuItem.Enabled = true;
                ButtonBlock.Enabled = false;
                ButtonReportSpam.Enabled = false;
                ButtonBlockDestroy.Enabled = false;
            }
            else
            {
                ButtonEdit.Enabled = false;
                ChangeIconToolStripMenuItem.Enabled = false;
                ButtonBlock.Enabled = true;
                ButtonReportSpam.Enabled = true;
                ButtonBlockDestroy.Enabled = true;
            }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public TwitterDataModel.User User
        {
            set { this.userInfo = value; }
        }

        private void LinkLabelWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_info.Url != null)
                MyOwner.OpenUriAsync(LinkLabelWeb.Text);
        }

        private void LinkLabelFollowing_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MyOwner.OpenUriAsync(Following);
        }

        private void LinkLabelFollowers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MyOwner.OpenUriAsync(Followers);
        }

        private void LinkLabelTweet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MyOwner.OpenUriAsync(Home);
        }

        private void LinkLabelFav_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MyOwner.OpenUriAsync(Favorites);
        }

        private void ButtonFollow_Click(object sender, EventArgs e)
        {
            string ret = MyOwner.TwitterInstance.PostFollowCommand(_info.ScreenName);
            if (!string.IsNullOrEmpty(ret))
            {
                MessageBox.Show(Properties.Resources.FRMessage2 + ret);
            }
            else
            {
                MessageBox.Show(Properties.Resources.FRMessage3);
                LabelIsFollowing.Text = Properties.Resources.GetFriendshipInfo1;
                ButtonFollow.Enabled = false;
                ButtonUnFollow.Enabled = true;
            }
        }

        private void ButtonUnFollow_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(_info.ScreenName + Properties.Resources.ButtonUnFollow_ClickText1,
                               Properties.Resources.ButtonUnFollow_ClickText2,
                               MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string ret = MyOwner.TwitterInstance.PostRemoveCommand(_info.ScreenName);
                if (!string.IsNullOrEmpty(ret))
                {
                    MessageBox.Show(Properties.Resources.FRMessage2 + ret);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.FRMessage3);
                    LabelIsFollowing.Text = Properties.Resources.GetFriendshipInfo2;
                    ButtonFollow.Enabled = true;
                    ButtonUnFollow.Enabled = false;
                }
            }
        }

        private void ShowUserInfo_Activated(object sender, EventArgs e)
        {
            //画面が他画面の裏に隠れると、アイコン画像が再描画されない問題の対応
            if (UserPicture.Image != null)
                UserPicture.Invalidate(false);
        }

        private void ShowUserInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            UserPicture.Image = null;
            if (icondata != null)
                icondata.Dispose();
        }

        private void BackgroundWorkerImageLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            string name = _info.ImageUrl.ToString();
            icondata = (new HttpVarious()).GetImage(name.Replace("_normal", "_bigger"));
            if (MyOwner.TwitterInstance.Username == _info.ScreenName) return;

            _info.isFollowing = false;
            _info.isFollowed = false;
            FriendshipResult = MyOwner.TwitterInstance.GetFriendshipInfo(_info.ScreenName, ref _info.isFollowing, ref _info.isFollowed);
        }

        private void BackgroundWorkerImageLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (icondata != null)
                    UserPicture.Image = icondata;
            }
            catch (Exception)
            {
                UserPicture.Image = null;
            }

            if (MyOwner.TwitterInstance.Username == _info.ScreenName)
            {
                // 自分の場合
                LabelIsFollowing.Text = "";
                LabelIsFollowed.Text = "";
                ButtonFollow.Enabled = false;
                ButtonUnFollow.Enabled = false;
            }
            else
            {
                if (string.IsNullOrEmpty(FriendshipResult))
                {
                    if (_info.isFollowing)
                    {
                        LabelIsFollowing.Text = Properties.Resources.GetFriendshipInfo1;
                    }
                    else
                    {
                        LabelIsFollowing.Text = Properties.Resources.GetFriendshipInfo2;
                    }
                    ButtonFollow.Enabled = !_info.isFollowing;
                    if (_info.isFollowed)
                    {
                        LabelIsFollowed.Text = Properties.Resources.GetFriendshipInfo3;
                    }
                    else
                    {
                        LabelIsFollowed.Text = Properties.Resources.GetFriendshipInfo4;
                    }
                    ButtonUnFollow.Enabled = _info.isFollowing;
                }
                else
                {
                    MessageBox.Show(FriendshipResult);
                    ButtonUnFollow.Enabled = false;
                    ButtonFollow.Enabled = false;
                    LabelIsFollowed.Text = Properties.Resources.GetFriendshipInfo6;
                    LabelIsFollowing.Text = Properties.Resources.GetFriendshipInfo6;
                }
            }

        }

        private void ShowUserInfo_Shown(object sender, EventArgs e)
        {
            DescriptionBrowser.DocumentText = descriptionTxt;
            DescriptionBrowser.Visible = true;
            if (_info.RecentPost != null)
            {
                RecentPostBrowser.DocumentText = recentPostTxt;
                RecentPostBrowser.Visible = true;
            }
            else
            {
                LabelRecentPost.Text = Properties.Resources.ShowUserInfo2;
            }
            ButtonClose.Focus();
        }

        private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.AbsoluteUri != "about:blank")
            {
                e.Cancel = true;

                if (e.Url.AbsoluteUri.StartsWith("http://twitter.com/search?q=%23") ||
                    e.Url.AbsoluteUri.StartsWith("https://twitter.com/search?q=%23"))
                {
                    //ハッシュタグの場合は、タブで開く
                    string urlStr = Uri.UnescapeDataString(e.Url.AbsoluteUri);
                    string hash = urlStr.Substring(urlStr.IndexOf("#"));
                    MyOwner.HashSupl.AddItem(hash);
                    MyOwner.HashMgr.AddHashToHistory(hash.Trim(), false);
                    MyOwner.AddNewTabForSearch(hash);
                    return;
                }
                else
                {
                    Match m = Regex.Match(e.Url.AbsoluteUri, @"^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
                    if (AppendSettingDialog.Instance.OpenUserTimeline && m.Success && MyOwner.IsTwitterId(m.Result("${ScreenName}")))
                    {
                        MyOwner.AddNewTabForUserTimeline(m.Result("${ScreenName}"));
                    }
                    else
                    {
                        MyOwner.OpenUriAsync(e.Url.OriginalString);
                    }
                }
            }
        }

        private void WebBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            WebBrowser ComponentInstance = (WebBrowser)sender;
            if (ComponentInstance.StatusText.StartsWith("http"))
            {
                ToolTip1.Show(ComponentInstance.StatusText, this, PointToClient(MousePosition));
            }
            else if (string.IsNullOrEmpty(DescriptionBrowser.StatusText))
            {
                ToolTip1.Hide(this);
            }
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebBrowser sc = ContextMenuRecentPostBrowser.SourceControl as WebBrowser;
            if (sc != null)
                sc.Document.ExecCommand("SelectAll", false, null);
        }

        private void SelectionCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebBrowser sc = ContextMenuRecentPostBrowser.SourceControl as WebBrowser;
            if (sc != null)
            {
                string _selText = MyOwner.WebBrowser_GetSelectionText(ref sc);
                if (_selText != null)
                {
                    try
                    {
                        Clipboard.SetDataObject(_selText, false, 5, 100);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            WebBrowser sc = ContextMenuRecentPostBrowser.SourceControl as WebBrowser;
            if (sc != null)
            {
                string _selText = MyOwner.WebBrowser_GetSelectionText(ref sc);
                if (_selText == null)
                    SelectionCopyToolStripMenuItem.Enabled = false;
                else
                    SelectionCopyToolStripMenuItem.Enabled = true;
            }
        }

        private void ShowUserInfo_MouseEnter(object sender, EventArgs e)
        {
            ToolTip1.Hide(this);
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MyOwner.OpenUriAsync("https://support.twitter.com/groups/31-twitter-basics/topics/111-features/articles/268350-x8a8d-x8a3c-x6e08-x307f-x30a2-x30ab-x30a6-x30f3-x30c8-x306b-x3064-x3044-x3066");
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MyOwner.OpenUriAsync("https://support.twitter.com/groups/31-twitter-basics/topics/107-my-profile-account-settings/articles/243055-x516c-x958b-x3001-x975e-x516c-x958b-x30a2-x30ab-x30a6-x30f3-x30c8-x306b-x3064-x3044-x3066");
        }

        private void ButtonSearchPosts_Click(object sender, EventArgs e)
        {
            MyOwner.AddNewTabForUserTimeline(_info.ScreenName);
        }

        private void UserPicture_DoubleClick(object sender, EventArgs e)
        {
            if (UserPicture.Image != null)
            {
                string name = _info.ImageUrl.ToString();
                MyOwner.OpenUriAsync(name.Remove(name.LastIndexOf("_normal"), 7));
            }
        }

        private void UserPicture_MouseEnter(object sender, EventArgs e)
        {
            UserPicture.Cursor = Cursors.Hand;
        }

        private void UserPicture_MouseLeave(object sender, EventArgs e)
        {
            UserPicture.Cursor = Cursors.Default;
        }

        private class UpdateProfileArgs
        {
            public Twitter tw;
            public string name;
            public string location;
            public string url;
            public string description;
        }

        private void UpdateProfile_Dowork(object sender, DoWorkEventArgs e)
        {
            UpdateProfileArgs arg = (UpdateProfileArgs)e.Argument;
            e.Result = arg.tw.PostUpdateProfile(arg.name,
                                                arg.url,
                                                arg.location,
                                                arg.description);
        }

        private void UpddateProfile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string res = (string)e.Result;
            if (res.StartsWith("err:", StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show(res);
            }
        }

        private bool IsEditing = false;
        private string ButtonEditText = "";

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            // 自分以外のプロフィールは変更できない
            if (MyOwner.TwitterInstance.Username != _info.ScreenName) return;

            if (!IsEditing)
            {
                ButtonEditText = ButtonEdit.Text;
                ButtonEdit.Text = Properties.Resources.UserInfoButtonEdit_ClickText1;

                //座標初期化,プロパティ設定
                TextBoxName.Location = LabelName.Location;
                TextBoxName.Height = LabelName.Height;
                TextBoxName.Width = LabelName.Width;
                TextBoxName.BackColor = MyOwner.InputBackColor;
                TextBoxName.MaxLength = 20;
                TextBoxName.Text = LabelName.Text;
                TextBoxName.TabStop = true;
                TextBoxName.Visible = true;
                LabelName.Visible = false;

                TextBoxLocation.Location = LabelLocation.Location;
                TextBoxLocation.Height = LabelLocation.Height;
                TextBoxLocation.Width = LabelLocation.Width;
                TextBoxLocation.BackColor = MyOwner.InputBackColor;
                TextBoxLocation.MaxLength = 30;
                TextBoxLocation.Text = LabelLocation.Text;
                TextBoxLocation.TabStop = true;
                TextBoxLocation.Visible = true;
                LabelLocation.Visible = false;

                TextBoxWeb.Location = LinkLabelWeb.Location;
                TextBoxWeb.Height = LinkLabelWeb.Height;
                TextBoxWeb.Width = LinkLabelWeb.Width;
                TextBoxWeb.BackColor = MyOwner.InputBackColor;
                TextBoxWeb.MaxLength = 100;
                TextBoxWeb.Text = _info.Url;
                TextBoxWeb.TabStop = true;
                TextBoxWeb.Visible = true;
                LinkLabelWeb.Visible = false;

                TextBoxDescription.Location = DescriptionBrowser.Location;
                TextBoxDescription.Height = DescriptionBrowser.Height;
                TextBoxDescription.Width = DescriptionBrowser.Width;
                TextBoxDescription.BackColor = MyOwner.InputBackColor;
                TextBoxDescription.MaxLength = 160;
                TextBoxDescription.Text = _info.Description;
                TextBoxDescription.Multiline = true;
                TextBoxDescription.ScrollBars = ScrollBars.Vertical;
                TextBoxDescription.TabStop = true;
                TextBoxDescription.Visible = true;
                DescriptionBrowser.Visible = false;

                TextBoxName.Focus();
                TextBoxName.Select(TextBoxName.Text.Length, 0);

                IsEditing = true;
            }
            else
            {
                UpdateProfileArgs arg = new UpdateProfileArgs();

                if (TextBoxName.Modified ||
                    TextBoxLocation.Modified ||
                    TextBoxWeb.Modified ||
                    TextBoxDescription.Modified)
                {
                    arg.tw = MyOwner.TwitterInstance;
                    arg.name = TextBoxName.Text.Trim();
                    arg.url = TextBoxWeb.Text.Trim();
                    arg.location = TextBoxLocation.Text.Trim();
                    arg.description = TextBoxDescription.Text.Trim();

                    using (FormInfo dlg = new FormInfo(this, Properties.Resources.UserInfoButtonEdit_ClickText2,
                                                       UpdateProfile_Dowork,
                                                       UpddateProfile_RunWorkerCompleted,
                                                       arg))
                    {
                        dlg.ShowDialog();
                        if (!string.IsNullOrEmpty(dlg.Result.ToString()))
                        {
                            return;
                        }
                    }
                }


                LabelName.Text = TextBoxName.Text;
                _info.Name = LabelName.Text;
                TextBoxName.TabStop = false;
                TextBoxName.Visible = false;
                LabelName.Visible = true;

                LabelLocation.Text = TextBoxLocation.Text;
                _info.Location = LabelLocation.Text;
                TextBoxLocation.TabStop = false;
                TextBoxLocation.Visible = false;
                LabelLocation.Visible = true;

                SetLinklabelWeb(TextBoxWeb.Text);
                _info.Url = TextBoxWeb.Text;
                TextBoxWeb.TabStop = false;
                TextBoxWeb.Visible = false;
                LinkLabelWeb.Visible = true;

                DescriptionBrowser.DocumentText = MakeDescriptionBrowserText(TextBoxDescription.Text);
                _info.Description = TextBoxDescription.Text;
                TextBoxDescription.TabStop = false;
                TextBoxDescription.Visible = false;
                DescriptionBrowser.Visible = true;

                ButtonEdit.Text = ButtonEditText;

                IsEditing = false;
            }

        }

        class UpdateProfileImageArgs
        {
            public Twitter tw;
            public string FileName;
        }

        private void UpdateProfileImage_Dowork(object sender, DoWorkEventArgs e)
        {
            UpdateProfileImageArgs arg = (UpdateProfileImageArgs)e.Argument;
            e.Result = arg.tw.PostUpdateProfileImage(arg.FileName);
        }

        private void UpdateProfileImage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string res = "";
            TwitterDataModel.User user = null;

            if (e.Result == null)
            {
                return;
            }


            // アイコンを取得してみる
            // が、古いアイコンのユーザーデータが返ってくるため反映/判断できない

            try
            {
                res = MyOwner.TwitterInstance.GetUserInfo(_info.ScreenName, ref user);
                Image img = (new HttpVarious()).GetImage(user.ProfileImageUrlHttps);
                if (img != null)
                {
                    UserPicture.Image = img;
                }
            }
            catch (Exception)
            {
            }
        }

        private void doChangeIcon(string filename)
        {
            string res = "";
            UpdateProfileImageArgs arg = new UpdateProfileImageArgs() { tw = MyOwner.TwitterInstance, FileName = filename };

            using (FormInfo dlg = new FormInfo(this, Properties.Resources.ChangeIconToolStripMenuItem_ClickText3,
                                               UpdateProfileImage_Dowork,
                                               UpdateProfileImage_RunWorkerCompleted,
                                               arg))
            {
                dlg.ShowDialog();
                res = dlg.Result as string;
                if (!string.IsNullOrEmpty(res))
                {
                    // "Err:"が付いたエラーメッセージが返ってくる
                    MessageBox.Show(res + "\r\n" + Properties.Resources.ChangeIconToolStripMenuItem_ClickText4);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.ChangeIconToolStripMenuItem_ClickText5);
                }
            }
        }

        private void ChangeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialogIcon.Filter = Properties.Resources.ChangeIconToolStripMenuItem_ClickText1;
            OpenFileDialogIcon.Title = Properties.Resources.ChangeIconToolStripMenuItem_ClickText2;
            OpenFileDialogIcon.FileName = "";

            DialogResult rslt = OpenFileDialogIcon.ShowDialog();

            if (rslt != DialogResult.OK)
            {
                return;
            }

            string fn = OpenFileDialogIcon.FileName;
            if (isValidIconFile(new FileInfo(fn)))
            {
                doChangeIcon(fn);
            }
            else
            {
                MessageBox.Show(Properties.Resources.ChangeIconToolStripMenuItem_ClickText6);
            }
        }

        private void ButtonBlock_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(_info.ScreenName + Properties.Resources.ButtonBlock_ClickText1,
                                Properties.Resources.ButtonBlock_ClickText2,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = MyOwner.TwitterInstance.PostCreateBlock(_info.ScreenName);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res + Environment.NewLine + Properties.Resources.ButtonBlock_ClickText3);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.ButtonBlock_ClickText4);
                }
            }
        }

        private void ButtonReportSpam_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(_info.ScreenName + Properties.Resources.ButtonReportSpam_ClickText1,
                                Properties.Resources.ButtonReportSpam_ClickText2,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = MyOwner.TwitterInstance.PostReportSpam(_info.ScreenName);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res + Environment.NewLine + Properties.Resources.ButtonReportSpam_ClickText3);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.ButtonReportSpam_ClickText4);
                }
            }
        }

        private void ButtonBlockDestroy_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(_info.ScreenName + Properties.Resources.ButtonBlockDestroy_ClickText1,
                                Properties.Resources.ButtonBlockDestroy_ClickText2,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string res = MyOwner.TwitterInstance.PostDestroyBlock(_info.ScreenName);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res + Environment.NewLine + Properties.Resources.ButtonBlockDestroy_ClickText3);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.ButtonBlockDestroy_ClickText4);
                }
            }
        }

        private bool isValidExtension(string ext)
        {
            return ext.Equals(".jpg") || ext.Equals(".jpeg") || ext.Equals(".png") || ext.Equals(".gif");
        }

        private bool isValidIconFile(FileInfo info)
        {
            string ext = info.Extension.ToLower();
            return isValidExtension(ext) && info.Length < 700 * 1024 && !MyCommon.IsAnimatedGif(info.FullName);
        }

        private void ShowUserInfo_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string filename = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                FileInfo fl = new FileInfo(filename);

                e.Effect = DragDropEffects.None;
                if (isValidIconFile(fl))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ShowUserInfo_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string filename = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
                doChangeIcon(filename);
            }
        }
    }
}
