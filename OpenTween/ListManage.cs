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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween
{
    public partial class ListManage : OTBaseForm
    {
        private readonly Twitter tw;

        public ListManage(Twitter tw)
        {
            this.InitializeComponent();

            this.tw = tw;
        }

        private void ListManage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.EditCheckBox.Checked)
                this.OKEditButton.PerformClick();
        }

        private async void ListManage_Load(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this))
            {
                try
                {
                    var lists = (IReadOnlyList<ListElement>)TabInformations.GetInstance().SubscribableLists;
                    if (lists.Count == 0)
                        lists = await this.FetchListsAsync();

                    this.UpdateListsListBox(lists);
                }
                catch (OperationCanceledException)
                {
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }
                catch (WebApiException)
                {
                    this.DialogResult = DialogResult.Abort;
                    return;
                }
            }
        }

        private void ListsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedItem == null) return;

            var list = (ListElement)this.ListsList.SelectedItem;
            this.UsernameTextBox.Text = list.Username;
            this.NameTextBox.Text = list.Name;
            this.PublicRadioButton.Checked = list.IsPublic;
            this.PrivateRadioButton.Checked = !list.IsPublic;
            this.MemberCountTextBox.Text = list.MemberCount.ToString();
            this.SubscriberCountTextBox.Text = list.SubscriberCount.ToString();
            this.DescriptionText.Text = list.Description;

            this.UserList.Items.Clear();
            foreach (var user in list.Members)
                this.UserList.Items.Add(user);

            this.GetMoreUsersButton.Text = this.UserList.Items.Count > 0 ? Properties.Resources.ListManageGetMoreUsers2 : Properties.Resources.ListManageGetMoreUsers1;
        }

        private void EditCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.AddListButton.Enabled = !this.EditCheckBox.Checked;
            this.EditCheckBox.Enabled = !this.EditCheckBox.Checked;
            this.DeleteListButton.Enabled = !this.EditCheckBox.Checked;

            this.NameTextBox.ReadOnly = !this.EditCheckBox.Checked;
            this.PublicRadioButton.Enabled = this.EditCheckBox.Checked;
            this.PrivateRadioButton.Enabled = this.EditCheckBox.Checked;
            this.DescriptionText.ReadOnly = !this.EditCheckBox.Checked;
            this.ListsList.Enabled = !this.EditCheckBox.Checked;

            this.OKEditButton.Enabled = this.EditCheckBox.Checked;
            this.CancelEditButton.Enabled = this.EditCheckBox.Checked;
            this.EditCheckBox.AutoCheck = !this.EditCheckBox.Checked;

            this.MemberGroup.Enabled = !this.EditCheckBox.Checked;
            this.UserGroup.Enabled = !this.EditCheckBox.Checked;
            this.CloseButton.Enabled = !this.EditCheckBox.Checked;

            this.UsernameTextBox.TabStop = !this.EditCheckBox.Checked;
            this.MemberCountTextBox.TabStop = !this.EditCheckBox.Checked;
            this.SubscriberCountTextBox.TabStop = !this.EditCheckBox.Checked;
            if (this.EditCheckBox.Checked == true) this.NameTextBox.Focus();
        }

        private async void OKEditButton_Click(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedItem == null) return;

            using (ControlTransaction.Disabled(this))
            {
                var listItem = (ListElement)this.ListsList.SelectedItem;

                if (MyCommon.IsNullOrEmpty(this.NameTextBox.Text))
                {
                    MessageBox.Show(Properties.Resources.ListManageOKButton1);
                    return;
                }

                listItem.Name = this.NameTextBox.Text;
                listItem.IsPublic = this.PublicRadioButton.Checked;
                listItem.Description = this.DescriptionText.Text;

                try
                {
                    await listItem.Refresh();
                }
                catch (WebApiException ex)
                {
                    MessageBox.Show(string.Format(Properties.Resources.ListManageOKButton2, ex.Message));
                    return;
                }

                this.ListsList.Items.Clear();
                this.ListManage_Load(this, EventArgs.Empty);

                this.EditCheckBox.AutoCheck = true;
                this.EditCheckBox.Checked = false;
            }
        }

        private void CancelEditButton_Click(object sender, EventArgs e)
        {
            this.EditCheckBox.AutoCheck = true;
            this.EditCheckBox.Checked = false;

            for (var i = this.ListsList.Items.Count - 1; i >= 0; i--)
            {
                if (this.ListsList.Items[i] is NewListElement)
                    this.ListsList.Items.RemoveAt(i);
            }

            this.ListsList_SelectedIndexChanged(this.ListsList, EventArgs.Empty);
        }

        private async void RefreshUsersButton_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this))
            {
                if (this.ListsList.SelectedItem == null) return;
                this.UserList.Items.Clear();

                var list = (ListElement)this.ListsList.SelectedItem;
                try
                {
                    await list.RefreshMembers();
                }
                catch (WebApiException ex)
                {
                    MessageBox.Show(string.Format(Properties.Resources.ListManageGetListMembersCallback1, ex.Message));
                    return;
                }

                this.ListsList_SelectedIndexChanged(this.ListsList, EventArgs.Empty);
                this.GetMoreUsersButton.Text = Properties.Resources.ListManageGetMoreUsers1;
            }
        }

        private async void GetMoreUsersButton_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this))
            {
                if (this.ListsList.SelectedItem == null) return;
                this.UserList.Items.Clear();

                var list = (ListElement)this.ListsList.SelectedItem;
                try
                {
                    await list.GetMoreMembers();
                }
                catch (WebApiException ex)
                {
                    MessageBox.Show(string.Format(Properties.Resources.ListManageGetListMembersCallback1, ex.Message));
                    return;
                }

                this.ListsList_SelectedIndexChanged(this.ListsList, EventArgs.Empty);
                this.GetMoreUsersButton.Text = Properties.Resources.ListManageGetMoreUsers1;
            }
        }

        private async void DeleteUserButton_Click(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedItem == null || this.UserList.SelectedItem == null)
                return;

            using (ControlTransaction.Disabled(this))
            {
                var list = (ListElement)this.ListsList.SelectedItem;
                var user = (UserInfo)this.UserList.SelectedItem;
                if (MessageBox.Show(Properties.Resources.ListManageDeleteUser1, ApplicationSettings.ApplicationName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        await this.tw.Api.ListsMembersDestroy(list.Id, user.ScreenName)
                            .IgnoreResponse();
                    }
                    catch (WebApiException ex)
                    {
                        MessageBox.Show(string.Format(Properties.Resources.ListManageDeleteUser2, ex.Message));
                        return;
                    }

                    var idx = this.ListsList.SelectedIndex;
                    list.Members.Remove(user);
                    this.ListsList_SelectedIndexChanged(this.ListsList, EventArgs.Empty);
                    if (idx < this.ListsList.Items.Count) this.ListsList.SelectedIndex = idx;
                }
            }
        }

        private async void DeleteListButton_Click(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedItem == null) return;

            using (ControlTransaction.Disabled(this))
            {
                var list = (ListElement)this.ListsList.SelectedItem;

                if (MessageBox.Show(Properties.Resources.ListManageDeleteLists1, ApplicationSettings.ApplicationName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        await this.tw.DeleteList(list.Id);
                    }
                    catch (WebApiException ex)
                    {
                        MessageBox.Show(Properties.Resources.ListManageOKButton2, ex.Message);
                        return;
                    }

                    try
                    {
                        await this.tw.GetListsApi();
                    }
                    catch (WebApiException ex)
                    {
                        MessageBox.Show(Properties.Resources.ListsDeleteFailed, ex.Message);
                        return;
                    }

                    this.ListsList.Items.Clear();
                    this.ListManage_Load(this, EventArgs.Empty);
                }
            }
        }

        private void AddListButton_Click(object sender, EventArgs e)
        {
            var newList = new NewListElement(this.tw);
            this.ListsList.Items.Add(newList);
            this.ListsList.SelectedItem = newList;
            this.EditCheckBox.Checked = true;
            this.EditCheckBox_CheckedChanged(this.EditCheckBox, EventArgs.Empty);
        }

        private async void UserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.UserList.SelectedItem == null)
            {
                this.UserIcon.Image?.Dispose();
                this.UserIcon.Image = null;
                this.UserLocation.Text = "";
                this.UserWeb.Text = "";
                this.UserFollowNum.Text = "0";
                this.UserFollowerNum.Text = "0";
                this.UserPostsNum.Text = "0";
                this.UserProfile.Text = "";
                this.UserTweetDateTime.Text = "";
                this.UserTweet.Text = "";
                this.DeleteUserButton.Enabled = false;
            }
            else
            {
                var user = (UserInfo)this.UserList.SelectedItem;
                this.UserLocation.Text = user.Location;
                this.UserWeb.Text = user.Url;
                this.UserFollowNum.Text = user.FriendsCount.ToString("#,###,##0");
                this.UserFollowerNum.Text = user.FollowersCount.ToString("#,###,##0");
                this.UserPostsNum.Text = user.StatusesCount.ToString("#,###,##0");
                this.UserProfile.Text = user.Description;
                if (!MyCommon.IsNullOrEmpty(user.RecentPost))
                {
                    this.UserTweetDateTime.Text = user.PostCreatedAt.ToLocalTimeString("yy/MM/dd HH:mm");
                    this.UserTweet.Text = user.RecentPost;
                }
                else
                {
                    this.UserTweetDateTime.Text = "";
                    this.UserTweet.Text = "";
                }
                this.DeleteUserButton.Enabled = true;

                if (user.ImageUrl != null)
                    await this.LoadUserIconAsync(user.ImageUrl, user.Id);
            }
        }

        private async Task LoadUserIconAsync(Uri imageUri, long userId)
        {
            var oldImage = this.UserIcon.Image;
            this.UserIcon.Image = null;
            oldImage?.Dispose();

            await this.UserIcon.SetImageFromTask(async () =>
            {
                var sizeName = Twitter.DecideProfileImageSize(this.UserIcon.Width);
                var uri = Twitter.CreateProfileImageUrl(imageUri.AbsoluteUri, sizeName);

                using var imageStream = await Networking.Http.GetStreamAsync(uri);
                var image = await MemoryImage.CopyFromStreamAsync(imageStream);

                // 画像の読み込み中に選択中のユーザーが変化していたらキャンセルとして扱う
                var selectedUser = (UserInfo)this.UserList.SelectedItem;
                if (selectedUser.Id != userId)
                {
                    image.Dispose();
                    throw new OperationCanceledException();
                }

                return image;
            });
        }

        private async void RefreshListsButton_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this))
            {
                try
                {
                    var lists = await this.FetchListsAsync();
                    this.UpdateListsListBox(lists);
                }
                catch (OperationCanceledException)
                {
                }
                catch (WebApiException ex)
                {
                    MessageBox.Show(string.Format(Properties.Resources.ListsDeleteFailed, ex.Message));
                }
            }
        }

        private async Task<IReadOnlyList<ListElement>> FetchListsAsync()
        {
            using var dialog = new WaitingDialog(Properties.Resources.ListsGetting);
            var cancellationToken = dialog.EnableCancellation();

            var task = this.tw.GetListsApi();
            await dialog.WaitForAsync(this, task);

            cancellationToken.ThrowIfCancellationRequested();

            return TabInformations.GetInstance().SubscribableLists;
        }

        private void UpdateListsListBox(IEnumerable<ListElement> lists)
        {
            using (ControlTransaction.Update(this.ListsList))
            {
                this.ListsList.Items.Clear();
                foreach (var listItem in lists.Where(x => x.UserId == this.tw.UserId))
                {
                    this.ListsList.Items.Add(listItem);
                }
                if (this.ListsList.Items.Count > 0)
                    this.ListsList.SelectedIndex = 0;
                this.ListsList.Focus();
            }
        }

        private async void UserWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            => await MyCommon.OpenInBrowserAsync(this, this.UserWeb.Text);

        private class NewListElement : ListElement
        {
            public bool IsCreated { get; private set; } = false;

            public NewListElement(Twitter tw)
                => this.tw = tw;

            public override async Task Refresh()
            {
                if (this.IsCreated)
                {
                    await base.Refresh().ConfigureAwait(false);
                }
                else
                {
                    await this.tw.CreateListApi(this.Name, !this.IsPublic, this.Description)
                        .ConfigureAwait(false);

                    this.IsCreated = true;
                }
            }

            public override string ToString()
            {
                if (this.IsCreated)
                    return base.ToString();
                else
                    return "NewList";
            }
        }

        private void ListManage_Validating(object sender, CancelEventArgs e)
        {
            if (this.EditCheckBox.Checked)
            {
                e.Cancel = true;
                this.CancelButton.PerformClick();
            }
        }
    }
}
