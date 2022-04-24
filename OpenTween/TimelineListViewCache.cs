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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OpenTween.Models;
using OpenTween.OpenTweenCustomControl;

namespace OpenTween
{
    public sealed class TimelineListViewCache : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        public ThemeManager Theme { get; set; }

        public bool IsListSizeMismatched
            => this.listView.VirtualListSize != this.tab.AllCount;

        private readonly DetailsListView listView;
        private readonly TabModel tab;
        private readonly SettingCommon settings;

        /// <summary>
        /// 現在表示している発言一覧の <see cref="ListView"/> に対するキャッシュ
        /// </summary>
        /// <remarks>
        /// キャッシュクリアのために null が代入されることがあるため、
        /// 使用する場合には <see cref="listItemCache"/> に対して直接メソッド等を呼び出さずに
        /// 一旦ローカル変数に代入してから参照すること。
        /// </remarks>
        private ListViewItemCache? listItemCache = null;

        public TimelineListViewCache(
            DetailsListView listView,
            TabModel tab,
            SettingCommon settings,
            ThemeManager theme
        )
        {
            this.listView = listView;
            this.tab = tab;
            this.settings = settings;
            this.Theme = theme;

            this.RegisterHandlers();
            this.listView.VirtualMode = true;
            this.UpdateListSize();
         }

        private void RegisterHandlers()
        {
            this.listView.CacheVirtualItems += this.ListView_CacheVirtualItems;
            this.listView.RetrieveVirtualItem += this.ListView_RetrieveVirtualItem;
        }

        private void UnregisterHandlers()
        {
            this.listView.CacheVirtualItems -= this.ListView_CacheVirtualItems;
            this.listView.RetrieveVirtualItem -= this.ListView_RetrieveVirtualItem;
        }

        public void UpdateListSize()
        {
            try
            {
                // リスト件数更新
                this.listView.VirtualListSize = this.tab.AllCount;
            }
            catch (NullReferenceException ex)
            {
                // WinForms 内部で ListView.set_TopItem が発生させている例外
                // https://ja.osdn.net/ticket/browse.php?group_id=6526&tid=36588
                MyCommon.TraceOut(ex, $"TabType: {this.tab.TabType}, Count: {this.tab.AllCount}, ListSize: {this.listView.VirtualListSize}");
            }
        }

        private void CreateCache(int startIndex, int endIndex)
        {
            if (this.tab.AllCount == 0)
                return;

            // インデックスを 0...(tabInfo.AllCount - 1) の範囲内にする
            int FilterRange(int index)
                => Math.Max(Math.Min(index, this.tab.AllCount - 1), 0);

            // キャッシュ要求（要求範囲±30を作成）
            startIndex = FilterRange(startIndex - 30);
            endIndex = FilterRange(endIndex + 30);

            var cacheLength = endIndex - startIndex + 1;

            var posts = this.tab[startIndex, endIndex]; // 配列で取得
            var listItems = Enumerable.Range(0, cacheLength)
                .Select(x => this.CreateItem(posts[x]))
                .ToArray();

            var listCache = new ListViewItemCache(
                startIndex,
                endIndex,
                listItems
            );

            Interlocked.Exchange(ref this.listItemCache, listCache);
        }

        /// <summary>
        /// DetailsListView のための ListViewItem のキャッシュを消去する
        /// </summary>
        public void PurgeCache()
            => Interlocked.Exchange(ref this.listItemCache, null);

        internal ListViewItem CreateItem(PostClass post)
        {
            var mk = new StringBuilder();

            if (post.FavoritedCount > 0) mk.Append("+" + post.FavoritedCount);

            ListViewItem itm;
            if (post.RetweetedId == null)
            {
                string[] sitem =
                {
                    "",
                    post.Nickname,
                    post.IsDeleted ? "(DELETED)" : post.AccessibleText.Replace('\n', ' '),
                    post.CreatedAt.ToLocalTimeString(this.settings.DateTimeFormat),
                    post.ScreenName,
                    "",
                    mk.ToString(),
                    post.Source,
                };
                itm = new ListViewItem(sitem);
            }
            else
            {
                string[] sitem =
                {
                    "",
                    post.Nickname,
                    post.IsDeleted ? "(DELETED)" : post.AccessibleText.Replace('\n', ' '),
                    post.CreatedAt.ToLocalTimeString(this.settings.DateTimeFormat),
                    post.ScreenName + Environment.NewLine + "(RT:" + post.RetweetedBy + ")",
                    "",
                    mk.ToString(),
                    post.Source,
                };
                itm = new ListViewItem(sitem);
            }
            itm.Tag = post;

            var read = post.IsRead;
            // 未読管理していなかったら既読として扱う
            if (!this.tab.UnreadManage || !this.settings.UnreadManage)
                read = true;

            this.ChangeItemStyleRead(read, itm, post);
            this.ColorizeList(itm, post);

            return itm;
        }

        public void ChangeCacheStyleRead(bool read, int index)
        {
            // Read:true=既読 false=未読
            // 未読管理していなかったら既読として扱う
            if (!this.tab.UnreadManage ||
               !this.settings.UnreadManage) read = true;

            var listCache = this.listItemCache;
            if (listCache == null)
                return;

            // キャッシュに含まれていないアイテムは対象外
            if (!listCache.TryGetValue(index, out var itm))
                return;

            var post = this.tab[index];
            this.ChangeItemStyleRead(read, itm, post);
        }

        private void ChangeItemStyleRead(bool read, ListViewItem item, PostClass post)
        {
            Font fnt;
            string star;
            // フォント
            if (read)
            {
                fnt = this.Theme.FontReaded;
                star = "";
            }
            else
            {
                fnt = this.Theme.FontUnread;
                star = "★";
            }
            if (item.SubItems[5].Text != star)
                item.SubItems[5].Text = star;

            // 文字色
            Color cl;
            if (post.IsFav)
                cl = this.Theme.ColorFav;
            else if (post.RetweetedId != null)
                cl = this.Theme.ColorRetweet;
            else if (post.IsOwl && (post.IsDm || this.settings.OneWayLove))
                cl = this.Theme.ColorOWL;
            else if (read || !this.settings.UseUnreadStyle)
                cl = this.Theme.ColorRead;
            else
                cl = this.Theme.ColorUnread;

            if (item.Index == -1)
            {
                item.ForeColor = cl;
                if (this.settings.UseUnreadStyle)
                    item.Font = fnt;
            }
            else
            {
                this.listView.Update();
                if (this.settings.UseUnreadStyle)
                    this.listView.ChangeItemFontAndColor(item, cl, fnt);
                else
                    this.listView.ChangeItemForeColor(item, cl);
            }
        }

        public void ColorizeList()
        {
            // Index:更新対象のListviewItem.Index。Colorを返す。
            // -1は全キャッシュ。Colorは返さない（ダミーを戻す）
            var basePost = this.tab.AnchorPost ?? this.tab.SelectedPost;
            if (basePost == null)
                return;

            var listCache = this.listItemCache;
            if (listCache == null)
                return;

            // ValidateRectが呼ばれる前に選択色などの描画を済ませておく
            this.listView.Update();

            foreach (var (listViewItem, index) in listCache.WithIndex())
            {
                var post = this.tab[index];
                var backColor = this.JudgeColor(basePost, post);
                this.listView.ChangeItemBackColor(listViewItem, backColor);
            }
        }

        private void ColorizeList(ListViewItem item, PostClass post)
        {
            // Index:更新対象のListviewItem.Index。Colorを返す。
            // -1は全キャッシュ。Colorは返さない（ダミーを戻す）
            var basePost = this.tab.AnchorPost ?? this.tab.SelectedPost;
            if (basePost == null)
                return;

            if (item.Index == -1)
                item.BackColor = this.JudgeColor(basePost, post);
            else
                this.listView.ChangeItemBackColor(item, this.JudgeColor(basePost, post));
        }

        internal Color JudgeColor(PostClass basePost, PostClass targetPost)
        {
            Color cl;
            if (targetPost.StatusId == basePost.InReplyToStatusId)
                // @先
                cl = this.Theme.ColorAtTo;
            else if (targetPost.IsMe)
                // 自分=発言者
                cl = this.Theme.ColorSelf;
            else if (targetPost.IsReply)
                // 自分宛返信
                cl = this.Theme.ColorAtSelf;
            else if (basePost.ReplyToList.Any(x => x.UserId == targetPost.UserId))
                // 返信先
                cl = this.Theme.ColorAtFromTarget;
            else if (targetPost.ReplyToList.Any(x => x.UserId == basePost.UserId))
                // その人への返信
                cl = this.Theme.ColorAtTarget;
            else if (targetPost.UserId == basePost.UserId)
                // 発言者
                cl = this.Theme.ColorTarget;
            else
                // その他
                cl = this.Theme.ColorListBackcolor;

            return cl;
        }

        private void ListView_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            var listCache = this.listItemCache;
            if (listCache != null && listCache.IsSupersetOf(e.StartIndex, e.EndIndex))
            {
                // If the newly requested cache is a subset of the old cache,
                // no need to rebuild everything, so do nothing.
                return;
            }

            // Now we need to rebuild the cache.
            this.CreateCache(e.StartIndex, e.EndIndex);
        }

        private void ListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var listCache = this.listItemCache;
            if (listCache != null)
            {
                if (listCache.TryGetValue(e.ItemIndex, out var item))
                {
                    e.Item = item;
                    return;
                }
            }

            // A cache miss, so create a new ListViewItem and pass it back.
            try
            {
                e.Item = this.CreateItem(this.tab[e.ItemIndex]);
            }
            catch (Exception)
            {
                // 不正な要求に対する間に合わせの応答
                string[] sitem = { "", "", "", "", "", "", "", "" };
                e.Item = new ListViewItem(sitem);
            }
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            // RetrieveVirtualItem が呼ばれないようにするため 0 をセットする
            this.listView.VirtualListSize = 0;

            this.UnregisterHandlers();
            this.PurgeCache();
            this.IsDisposed = true;
        }
    }

    public class ListViewItemCache
    {
        /// <summary>キャッシュする範囲の開始インデックス</summary>
        public int StartIndex { get; }

        /// <summary>キャッシュする範囲の終了インデックス</summary>
        public int EndIndex { get; }

        /// <summary>キャッシュされた範囲に対応する <see cref="ListViewItem"/> の配列</summary>
        public ListViewItem[] Cache { get; }

        /// <summary>キャッシュされたアイテムの件数</summary>
        public int Count
            => this.EndIndex - this.StartIndex + 1;

        public ListViewItemCache(int startIndex, int endIndex, ListViewItem[] cache)
        {
            if (!IsCacheSizeValid(startIndex, endIndex, cache))
                throw new ArgumentException("Cache size mismatch", nameof(cache));

            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
            this.Cache = cache;
        }

        /// <summary>指定されたインデックスがキャッシュの範囲内であるか判定します</summary>
        /// <returns><paramref name="index"/> がキャッシュの範囲内であれば true、それ以外は false</returns>
        public bool Contains(int index)
            => index >= this.StartIndex && index <= this.EndIndex;

        /// <summary>指定されたインデックスの範囲が全てキャッシュの範囲内であるか判定します</summary>
        /// <returns><paramref name="rangeStart"/> から <paramref name="rangeEnd"/> の範囲が全てキャッシュの範囲内であれば true、それ以外は false</returns>
        public bool IsSupersetOf(int rangeStart, int rangeEnd)
            => rangeStart >= this.StartIndex && rangeEnd <= this.EndIndex;

        /// <summary>指定されたインデックスの <see cref="ListViewItem"/> をキャッシュから取得することを試みます</summary>
        /// <returns>取得に成功すれば true、それ以外は false</returns>
        public bool TryGetValue(int index, [NotNullWhen(true)] out ListViewItem? item)
        {
            if (this.Contains(index))
            {
                item = this.Cache[index - this.StartIndex];
                return true;
            }
            else
            {
                item = null;
                return false;
            }
        }

        public IEnumerable<(ListViewItem Item, int Index)> WithIndex()
        {
            foreach (var (item, index) in this.Cache.WithIndex())
                yield return (item, index + this.StartIndex);
        }

        private static bool IsCacheSizeValid<T>(int startIndex, int endIndex, T[] cache)
            => cache.Length == (endIndex - startIndex + 1);
    }
}
