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
using OpenTween.Api;
using OpenTween.Api.DataModel;
using OpenTween.Connection;

namespace OpenTween
{
    public partial class MyLists : OTBaseForm
    {
        private readonly TwitterApi twitterApi = null!;
        private readonly string contextScreenName = null!;

        /// <summary>自分が所有しているリスト</summary>
        private ListElement[] ownedLists = Array.Empty<ListElement>();

        /// <summary>操作対象のユーザーが追加されているリストのID</summary>
        private long[] addedListIds = Array.Empty<long>();

        public MyLists()
            => this.InitializeComponent();

        public MyLists(string screenName, TwitterApi twitterApi)
        {
            this.InitializeComponent();

            this.twitterApi = twitterApi;
            this.contextScreenName = screenName;

            this.Text = screenName + Properties.Resources.MyLists1;
        }

        private async Task RefreshListBox()
        {
            using (var dialog = new WaitingDialog(Properties.Resources.ListsGetting))
            {
                var cancellationToken = dialog.EnableCancellation();

                var task = Task.Run(() => this.FetchMembershipListIds());
                await dialog.WaitForAsync(this, task);

                cancellationToken.ThrowIfCancellationRequested();
            }

            using (ControlTransaction.Update(this.ListsCheckedListBox))
            {
                this.ListsCheckedListBox.Items.Clear();

                foreach (var list in this.ownedLists)
                {
                    var added = this.addedListIds.Contains(list.Id);
                    this.ListsCheckedListBox.Items.Add(list, isChecked: added);
                }
            }
        }

        private async Task FetchMembershipListIds()
        {
            var ownedListData = await TwitterLists.GetAllItemsAsync(x =>
                this.twitterApi.ListsOwnerships(this.twitterApi.CurrentScreenName, cursor: x, count: 1000))
                    .ConfigureAwait(false);

            this.ownedLists = ownedListData.Select(x => new ListElement(x, null!)).ToArray();

            var listsUserAddedTo = await TwitterLists.GetAllItemsAsync(x =>
                this.twitterApi.ListsMemberships(this.contextScreenName, cursor: x, count: 1000, filterToOwnedLists: true))
                    .ConfigureAwait(false);

            this.addedListIds = listsUserAddedTo.Select(x => x.Id).ToArray();
        }

        private async Task AddToList(ListElement list)
        {
            try
            {
                await this.twitterApi.ListsMembersCreate(list.Id, this.contextScreenName)
                    .IgnoreResponse();

                var index = this.ListsCheckedListBox.Items.IndexOf(list);
                this.ListsCheckedListBox.SetItemCheckState(index, CheckState.Checked);
            }
            catch (WebApiException ex)
            {
                MessageBox.Show(string.Format(Properties.Resources.ListManageOKButton2, ex.Message));
            }
        }

        private async Task RemoveFromList(ListElement list)
        {
            try
            {
                await this.twitterApi.ListsMembersDestroy(list.Id, this.contextScreenName)
                    .IgnoreResponse();

                var index = this.ListsCheckedListBox.Items.IndexOf(list);
                this.ListsCheckedListBox.SetItemCheckState(index, CheckState.Unchecked);
            }
            catch (WebApiException ex)
            {
                MessageBox.Show(string.Format(Properties.Resources.ListManageOKButton2, ex.Message));
            }
        }

        private async void MyLists_Load(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this))
            {
                try
                {
                    await this.RefreshListBox();
                }
                catch (OperationCanceledException)
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                catch (WebApiException ex)
                {
                    MessageBox.Show($"Failed to get lists. ({ex.Message})");
                    this.DialogResult = DialogResult.Abort;
                }
            }
        }

        private async void ListsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // 他のイベント等で操作中の場合は無視する
            if (!this.Enabled)
                return;

            using (ControlTransaction.Disabled(this))
            {
                var list = (ListElement)this.ListsCheckedListBox.Items[e.Index];

                if (e.CurrentValue == CheckState.Unchecked)
                    await this.AddToList(list);
                else
                    await this.RemoveFromList(list);
            }
        }

        private void ListsCheckedListBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    // 項目が無い部分をクリックしても、選択されている項目のチェック状態が変更されてしまうので、その対策
                    for (var index = 0; index < this.ListsCheckedListBox.Items.Count; index++)
                    {
                        if (this.ListsCheckedListBox.GetItemRectangle(index).Contains(e.Location))
                            return;
                    }
                    this.ListsCheckedListBox.SelectedItem = null;
                    break;
                case MouseButtons.Right:
                    // コンテキストメニューの項目実行時にSelectedItemプロパティを利用出来るように
                    for (var index = 0; index < this.ListsCheckedListBox.Items.Count; index++)
                    {
                        if (this.ListsCheckedListBox.GetItemRectangle(index).Contains(e.Location))
                        {
                            this.ListsCheckedListBox.SetSelected(index, true);
                            return;
                        }
                    }
                    this.ListsCheckedListBox.SelectedItem = null;
                    break;
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
            => e.Cancel = this.ListsCheckedListBox.SelectedItem == null;

        private async void MenuItemAdd_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this))
            {
                await this.AddToList((ListElement)this.ListsCheckedListBox.SelectedItem);
            }
        }

        private async void MenuItemDelete_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this))
            {
                await this.RemoveFromList((ListElement)this.ListsCheckedListBox.SelectedItem);
            }
        }

        private async void MenuItemReload_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this))
            {
                try
                {
                    await this.RefreshListBox();
                }
                catch (OperationCanceledException)
                {
                }
                catch (WebApiException ex)
                {
                    MessageBox.Show($"Failed to get lists. ({ex.Message})");
                }
            }
        }

        private async void ListRefreshButton_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this))
            {
                try
                {
                    await this.RefreshListBox();
                }
                catch (OperationCanceledException)
                {
                }
                catch (WebApiException ex)
                {
                    MessageBox.Show($"Failed to get lists. ({ex.Message})");
                }
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
            => this.Close();
    }
}
