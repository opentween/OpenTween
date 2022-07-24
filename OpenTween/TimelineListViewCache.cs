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
            SettingCommon settings
        )
        {
            this.listView = listView;
            this.tab = tab;
            this.settings = settings;

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

        internal void CreateCache(int startIndex, int endIndex)
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

        private (ListViewItem Item, ListItemStyle Style) CreateItem(PostClass post)
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

            var style = this.DetermineListItemStyle(post);
            this.ApplyListItemStyle(itm, style);

            return (itm, style);
        }

        public void RefreshStyle(int index)
        {
            var post = this.tab[index];
            var style = this.DetermineListItemStyle(post);

            var listCache = this.listItemCache;
            if (listCache != null && listCache.TryGetValue(index, out var item, out var currentStyle))
            {
                // スタイルに変化がない場合は何もせず終了
                if (currentStyle == style)
                    return;

                listCache.UpdateStyle(index, style);
            }
            else
            {
                item = this.listView.Items[index];
            }

            // ValidateRectが呼ばれる前に選択色などの描画を済ませておく
            this.listView.Update();

            this.ApplyListItemStyle(item, style);
            this.listView.RefreshItem(index);
        }

        public void RefreshStyle()
        {
            var listCache = this.listItemCache;
            if (listCache == null)
                return;

            var updatedIndices = new List<int>();

            foreach (var (_, currentStyle, index) in listCache.WithIndex())
            {
                var post = this.tab[index];
                var style = this.DetermineListItemStyle(post);
                if (currentStyle == style)
                    continue;

                listCache.UpdateStyle(index, style);
                updatedIndices.Add(index);
            }

            // ValidateRectが呼ばれる前に選択色などの描画を済ませておく
            this.listView.Update();

            foreach (var index in updatedIndices)
            {
                if (!listCache.TryGetValue(index, out var item, out var style))
                    continue;

                this.ApplyListItemStyle(item, style);
            }

            updatedIndices.Add(this.tab.SelectedIndex);
            this.listView.RefreshItems(updatedIndices);
        }

        public ListViewItem GetItem(int index)
        {
            var listCache = this.listItemCache;
            if (listCache != null)
            {
                if (listCache.TryGetValue(index, out var item, out _))
                    return item;
            }

            var post = this.tab[index];
            return this.CreateItem(post).Item;
        }

        public ListItemStyle GetStyle(int index)
        {
            var listCache = this.listItemCache;
            if (listCache != null)
            {
                if (listCache.TryGetValue(index, out _, out var style))
                    return style;
            }

            var post = this.tab[index];
            return this.DetermineListItemStyle(post);
        }

        private void ApplyListItemStyle(ListViewItem item, ListItemStyle style)
            => item.SubItems[5].Text = this.GetUnreadMark(style.UnreadMark);

        private string GetUnreadMark(bool unreadMark)
            => unreadMark ? "★" : "";

        private ListItemStyle DetermineListItemStyle(PostClass post)
        {
            var unreadManageEnabled = this.tab.UnreadManage && this.settings.UnreadManage;
            var useUnreadStyle = unreadManageEnabled && this.settings.UseUnreadStyle;

            var basePost = this.tab.AnchorPost ?? this.tab.SelectedPost;

            return new(
                this.DetermineUnreadMark(post, unreadManageEnabled),
                this.DetermineBackColor(basePost, post),
                this.DetermineForeColor(post, useUnreadStyle),
                this.DetermineFont(post, useUnreadStyle)
            );
        }

        private bool DetermineUnreadMark(PostClass post, bool unreadManageEnabled)
        {
            if (!unreadManageEnabled)
                return false;

            return !post.IsRead;
        }

        private ListItemBackColor DetermineBackColor(PostClass? basePost, PostClass post)
        {
            if (basePost == null)
                return ListItemBackColor.None;

            // @先
            if (post.StatusId == basePost.InReplyToStatusId)
                return ListItemBackColor.AtTo;

            // 自分=発言者
            if (post.IsMe)
                return ListItemBackColor.Self;

            // 自分宛返信
            if (post.IsReply)
                return ListItemBackColor.AtSelf;

            // 返信先
            if (basePost.ReplyToList.Any(x => x.UserId == post.UserId))
                return ListItemBackColor.AtFromTarget;

            // その人への返信
            if (post.ReplyToList.Any(x => x.UserId == basePost.UserId))
                return ListItemBackColor.AtTarget;

            // 発言者
            if (post.UserId == basePost.UserId)
                return ListItemBackColor.Target;

            // その他
            return ListItemBackColor.None;
        }

        private ListItemForeColor DetermineForeColor(PostClass post, bool useUnreadStyle)
        {
            if (post.IsFav)
                return ListItemForeColor.Fav;

            if (post.RetweetedId != null)
                return ListItemForeColor.Retweet;

            if (post.IsOwl && (post.IsDm || this.settings.OneWayLove))
                return ListItemForeColor.OWL;

            if (useUnreadStyle && !post.IsRead)
                return ListItemForeColor.Unread;

            return ListItemForeColor.None;
        }

        private ListItemFont DetermineFont(PostClass post, bool useUnreadStyle)
        {
            if (useUnreadStyle && !post.IsRead)
                return ListItemFont.Unread;

            return ListItemFont.Readed;
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
            => e.Item = this.GetItem(e.ItemIndex);

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

    public enum ListItemBackColor
    {
        None,
        Self,
        AtSelf,
        Target,
        AtTarget,
        AtFromTarget,
        AtTo,
    }

    public enum ListItemForeColor
    {
        None,
        Fav,
        Retweet,
        OWL,
        Unread,
    }

    public enum ListItemFont
    {
        Readed,
        Unread,
    }

    public readonly record struct ListItemStyle(
        bool UnreadMark,
        ListItemBackColor BackColor,
        ListItemForeColor ForeColor,
        ListItemFont Font
    );

    public class ListViewItemCache
    {
        /// <summary>キャッシュする範囲の開始インデックス</summary>
        public int StartIndex { get; }

        /// <summary>キャッシュする範囲の終了インデックス</summary>
        public int EndIndex { get; }

        /// <summary>キャッシュされた範囲に対応する <see cref="ListViewItem"/> と <see cref="ListItemStyle"> の配列</summary>
        public (ListViewItem, ListItemStyle)[] Cache { get; }

        /// <summary>キャッシュされたアイテムの件数</summary>
        public int Count
            => this.EndIndex - this.StartIndex + 1;

        public ListViewItemCache(int startIndex, int endIndex, (ListViewItem, ListItemStyle)[] cache)
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
        public bool TryGetValue(int index, [NotNullWhen(true)] out ListViewItem? item, out ListItemStyle style)
        {
            if (this.Contains(index))
            {
                (item, style) = this.Cache[index - this.StartIndex];
                return true;
            }
            else
            {
                item = null;
                style = default;
                return false;
            }
        }

        public IEnumerable<(ListViewItem Item, ListItemStyle Stype, int Index)> WithIndex()
        {
            foreach (var ((item, style), index) in this.Cache.WithIndex())
                yield return (item, style, index + this.StartIndex);
        }

        public void UpdateStyle(int index, ListItemStyle style)
        {
            if (!this.Contains(index))
                return;

            this.Cache[index - this.StartIndex].Item2 = style;
        }

        private static bool IsCacheSizeValid<T>(int startIndex, int endIndex, T[] cache)
            => cache.Length == (endIndex - startIndex + 1);
    }
}
