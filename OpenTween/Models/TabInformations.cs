// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
//           (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Setting;

namespace OpenTween.Models
{
    public sealed class TabInformations
    {
        // 個別タブの情報をDictionaryで保持
        public IReadOnlyTabCollection Tabs
            => this.tabs;

        public MuteTabModel MuteTab { get; private set; } = new();

        public ConcurrentDictionary<long, PostClass> Posts { get; } = new();

        private readonly Dictionary<long, PostClass> quotes = new();
        private readonly ConcurrentDictionary<long, int> retweetsCount = new();

        public Stack<TabModel> RemovedTab { get; } = new();

        public ISet<long> BlockIds { get; set; } = new HashSet<long>();

        public ISet<long> MuteUserIds { get; set; } = new HashSet<long>();

        // 発言の追加
        // AddPost(複数回) -> DistributePosts          -> SubmitUpdate

        private readonly TabCollection tabs = new();
        private readonly ConcurrentQueue<long> addQueue = new();

        /// <summary>通知サウンドを再生する優先順位</summary>
        private readonly Dictionary<MyCommon.TabUsageType, int> notifyPriorityByTabType = new()
        {
            [MyCommon.TabUsageType.DirectMessage] = 100,
            [MyCommon.TabUsageType.Mentions] = 90,
            [MyCommon.TabUsageType.UserDefined] = 80,
            [MyCommon.TabUsageType.Home] = 70,
            [MyCommon.TabUsageType.Favorites] = 60,
        };

        // トランザクション用
        private readonly object lockObj = new();

        private static readonly TabInformations Instance = new();

        // List
        private List<ListElement> lists = new();

        internal TabInformations()
        {
        }

        public static TabInformations GetInstance()
            => Instance; // singleton

        public string SelectedTabName { get; private set; } = "";

        public TabModel SelectedTab
            => this.Tabs[this.SelectedTabName];

        public int SelectedTabIndex
            => this.Tabs.IndexOf(this.SelectedTabName);

        public List<ListElement> SubscribableLists
        {
            get => this.lists;
            set
            {
                if (value.Count > 0)
                {
                    foreach (var tb in this.GetTabsByType<ListTimelineTabModel>())
                    {
                        foreach (var list in value)
                        {
                            if (tb.ListInfo.Id == list.Id)
                            {
                                tb.ListInfo = list;
                                break;
                            }
                        }
                    }
                }
                this.lists = value;
            }
        }

        public bool AddTab(TabModel tab)
        {
            lock (this.lockObj)
            {
                if (tab is MuteTabModel muteTab)
                {
                    this.MuteTab = muteTab;
                    return true;
                }

                if (this.Tabs.Contains(tab.TabName))
                    return false;

                this.tabs.Add(tab);
                tab.SetSortMode(this.SortMode, this.SortOrder);

                return true;
            }
        }

        public void RemoveTab(string tabName)
        {
            lock (this.lockObj)
            {
                var tb = this.GetTabByName(tabName);
                if (tb == null || tb.IsDefaultTabType) return; // 念のため

                if (!tb.IsInnerStorageTabType)
                {
                    var homeTab = this.HomeTab;
                    var dmTab = this.DirectMessageTab;

                    for (var idx = 0; idx < tb.AllCount; ++idx)
                    {
                        var exist = false;
                        var id = tb.GetStatusIdAt(idx);
                        if (id < 0) continue;
                        foreach (var tab in this.Tabs)
                        {
                            if (tab != tb && tab != dmTab)
                            {
                                if (tab.Contains(id))
                                {
                                    exist = true;
                                    break;
                                }
                            }
                        }
                        if (!exist) homeTab.AddPostImmediately(id, this.Posts[id].IsRead);
                    }
                }
                this.RemovedTab.Push(tb);
                this.tabs.Remove(tabName);
            }
        }

        public void ReplaceTab(TabModel tab)
        {
            if (!this.ContainsTab(tab.TabName))
                throw new ArgumentOutOfRangeException(nameof(tab));

            var index = this.tabs.IndexOf(tab);
            this.tabs.RemoveAt(index);
            this.tabs.Insert(index, tab);
        }

        public void MoveTab(int newIndex, TabModel tab)
        {
            if (!this.ContainsTab(tab))
                throw new ArgumentOutOfRangeException(nameof(tab));

            this.tabs.Remove(tab);
            this.tabs.Insert(newIndex, tab);
        }

        public bool ContainsTab(string tabText)
            => this.Tabs.Contains(tabText);

        public bool ContainsTab(TabModel ts)
            => this.Tabs.Contains(ts);

        public void SelectTab(string tabName)
        {
            if (!this.Tabs.Contains(tabName))
                throw new ArgumentException($"{tabName} does not exist.", nameof(tabName));

            this.SelectedTabName = tabName;
        }

        public void LoadTabsFromSettings(SettingTabs settingTabs)
        {
            foreach (var tabSetting in settingTabs.Tabs)
            {
                var tab = this.CreateTabFromSettings(tabSetting);
                if (tab == null)
                    continue;

                if (this.ContainsTab(tab.TabName))
                    tab.TabName = this.MakeTabName("MyTab");

                this.AddTab(tab);
            }
        }

        public TabModel? CreateTabFromSettings(SettingTabs.SettingTabItem tabSetting)
        {
            var tabName = tabSetting.TabName;

            TabModel? tab = tabSetting.TabType switch
            {
                MyCommon.TabUsageType.Home
                    => new HomeTabModel(tabName),
                MyCommon.TabUsageType.Mentions
                    => new MentionsTabModel(tabName),
                MyCommon.TabUsageType.DirectMessage
                    => new DirectMessagesTabModel(tabName),
                MyCommon.TabUsageType.Favorites
                    => new FavoritesTabModel(tabName),
                MyCommon.TabUsageType.UserDefined
                    => new FilterTabModel(tabName),
                MyCommon.TabUsageType.UserTimeline
                    => new UserTimelineTabModel(tabName, tabSetting.User!),
                MyCommon.TabUsageType.PublicSearch
                    => new PublicSearchTabModel(tabName)
                    {
                        SearchWords = tabSetting.SearchWords,
                        SearchLang = tabSetting.SearchLang,
                    },
                MyCommon.TabUsageType.Lists
                    => new ListTimelineTabModel(tabName, tabSetting.ListInfo!),
                MyCommon.TabUsageType.Mute
                    => new MuteTabModel(tabName),
                _ => null,
            };

            if (tab == null)
                return null;

            tab.UnreadManage = tabSetting.UnreadManage;
            tab.Protected = tabSetting.Protected;
            tab.Notify = tabSetting.Notify;
            tab.SoundFile = tabSetting.SoundFile;

            if (tab is FilterTabModel filterTab)
            {
                filterTab.FilterArray = tabSetting.FilterArray;
                filterTab.FilterModified = false;
            }

            return tab;
        }

        /// <summary>
        /// デフォルトのタブを追加する
        /// </summary>
        public void AddDefaultTabs()
        {
            if (this.GetTabByType<HomeTabModel>() == null)
                this.AddTab(new HomeTabModel());

            if (this.GetTabByType<MentionsTabModel>() == null)
            {
                var mentionsTab = new MentionsTabModel
                {
                    Notify = true,
                };
                this.AddTab(mentionsTab);
            }

            if (this.GetTabByType<DirectMessagesTabModel>() == null)
            {
                var dmTab = new DirectMessagesTabModel
                {
                    Notify = true,
                };
                this.AddTab(dmTab);
            }

            if (this.GetTabByType<FavoritesTabModel>() == null)
                this.AddTab(new FavoritesTabModel());
        }

        /// <summary>
        /// 指定されたタブ名を元に、既存のタブ名との重複を避けた名前を生成します
        /// </summary>
        /// <param name="baseTabName">作成したいタブ名</param>
        /// <returns>生成されたタブ名</returns>
        /// <exception cref="TabException">タブ名の生成を 100 回試行して失敗した場合</exception>
        public string MakeTabName(string baseTabName)
            => this.MakeTabName(baseTabName, 100);

        /// <summary>
        /// 指定されたタブ名を元に、既存のタブ名との重複を避けた名前を生成します
        /// </summary>
        /// <param name="baseTabName">作成したいタブ名</param>
        /// <param name="retryCount">重複を避けたタブ名を生成する試行回数</param>
        /// <returns>生成されたタブ名</returns>
        /// <exception cref="TabException">retryCount で指定された回数だけタブ名の生成を試行して失敗した場合</exception>
        public string MakeTabName(string baseTabName, int retryCount)
        {
            if (!this.ContainsTab(baseTabName))
                return baseTabName;

            foreach (var i in Enumerable.Range(2, retryCount - 1))
            {
                var tabName = baseTabName + i;
                if (!this.ContainsTab(tabName))
                {
                    return tabName;
                }
            }

            var message = string.Format(Properties.Resources.TabNameDuplicate_Text, baseTabName);
            throw new TabException(message);
        }

        public SortOrder SortOrder { get; private set; }

        public ComparerMode SortMode { get; private set; }

        public void SetSortMode(ComparerMode mode, SortOrder sortOrder)
        {
            this.SortMode = mode;
            this.SortOrder = sortOrder;

            foreach (var tab in this.Tabs)
                tab.SetSortMode(mode, sortOrder);
        }

        public SortOrder ToggleSortOrder(ComparerMode sortMode)
        {
            var sortOrder = this.SortOrder;

            if (this.SortMode == sortMode)
            {
                if (sortOrder == SortOrder.Ascending)
                    sortOrder = SortOrder.Descending;
                else
                    sortOrder = SortOrder.Ascending;
            }
            else
            {
                sortOrder = SortOrder.Ascending;
            }

            this.SetSortMode(sortMode, sortOrder);

            return this.SortOrder;
        }

        public PostClass? RetweetSource(long id)
            => this.Posts.TryGetValue(id, out var status) ? status : null;

        public void RemovePostFromAllTabs(long statusId, bool setIsDeleted)
        {
            foreach (var tab in this.Tabs)
            {
                tab.EnqueueRemovePost(statusId, setIsDeleted);
            }

            if (setIsDeleted)
            {
                if (this.Posts.TryGetValue(statusId, out var post))
                    post.IsDeleted = true;
            }
        }

        public int SubmitUpdate()
            => this.SubmitUpdate(out _, out _, out _, out _);

        public int SubmitUpdate(
            out string soundFile,
            out PostClass[] notifyPosts,
            out bool newMentionOrDm,
            out bool isDeletePost)
        {
            // 注：メインスレッドから呼ぶこと
            lock (this.lockObj)
            {
                soundFile = "";
                notifyPosts = Array.Empty<PostClass>();
                newMentionOrDm = false;
                isDeletePost = false;

                var addedCountTotal = 0;
                var removedIdsAll = new List<long>();
                var notifyPostsList = new List<PostClass>();

                var currentNotifyPriority = -1;

                foreach (var tab in this.Tabs)
                {
                    // 振分確定 (各タブに反映)
                    var addedIds = tab.AddSubmit();

                    if (tab.TabType == MyCommon.TabUsageType.Mentions ||
                        tab.TabType == MyCommon.TabUsageType.DirectMessage)
                    {
                        if (addedIds.Count > 0)
                            newMentionOrDm = true;
                    }

                    if (addedIds.Count != 0)
                    {
                        if (tab.Notify)
                        {
                            // 通知対象のリストに追加
                            foreach (var statusId in addedIds)
                            {
                                if (tab.Posts.TryGetValue(statusId, out var post))
                                    notifyPostsList.Add(post);
                            }
                        }

                        // 通知サウンドは TabClass.Notify の値に関わらず鳴らす
                        // SettingCommon.PlaySound が false であれば TweenMain 側で無効化される
                        if (!MyCommon.IsNullOrEmpty(tab.SoundFile))
                        {
                            if (!this.notifyPriorityByTabType.TryGetValue(tab.TabType, out var notifyPriority))
                                notifyPriority = 0;

                            if (notifyPriority > currentNotifyPriority)
                            {
                                // より優先度の高い通知を再生する
                                soundFile = tab.SoundFile;
                                currentNotifyPriority = notifyPriority;
                            }
                        }
                    }

                    addedCountTotal += addedIds.Count;

                    var removedIds = tab.RemoveSubmit();
                    removedIdsAll.AddRange(removedIds);
                }

                notifyPosts = notifyPostsList.Distinct().ToArray();

                if (removedIdsAll.Count > 0)
                    isDeletePost = true;

                foreach (var removedId in removedIdsAll.Distinct())
                {
                    var orphaned = true;
                    foreach (var tab in this.Tabs)
                    {
                        if (tab.Contains(removedId))
                        {
                            orphaned = false;
                            break;
                        }
                    }

                    // 全てのタブから表示されなくなった発言は this._statuses からも削除する
                    if (orphaned)
                        this.Posts.TryRemove(removedId, out var removedPost);
                }

                return addedCountTotal;
            }
        }

        public int DistributePosts()
        {
            lock (this.lockObj)
            {
                var homeTab = this.HomeTab;
                var replyTab = this.MentionTab;
                var favTab = this.FavoriteTab;

                var distributableTabs = this.GetTabsByType<FilterTabModel>()
                    .ToArray();

                var adddedCount = 0;

                while (this.addQueue.TryDequeue(out var statusId))
                {
                    if (!this.Posts.TryGetValue(statusId, out var post))
                        continue;

                    var filterHit = false; // フィルタにヒットしたタブがあるか
                    var mark = false; // フィルタによってマーク付けされたか
                    var excludedReply = false; // リプライから除外されたか
                    var moved = false; // Recentタブから移動するか (Recentタブに表示しない)

                    foreach (var tab in distributableTabs)
                    {
                        // 各振り分けタブのフィルタを実行する
                        switch (tab.AddFiltered(post))
                        {
                            case MyCommon.HITRESULT.Copy:
                                filterHit = true;
                                break;
                            case MyCommon.HITRESULT.CopyAndMark:
                                filterHit = true;
                                mark = true;
                                break;
                            case MyCommon.HITRESULT.Move:
                                filterHit = true;
                                moved = true;
                                break;
                            case MyCommon.HITRESULT.None:
                                break;
                            case MyCommon.HITRESULT.Exclude:
                                if (tab.TabType == MyCommon.TabUsageType.Mentions)
                                    excludedReply = true;
                                break;
                        }
                    }

                    post.FilterHit = filterHit;
                    post.IsMark = mark;
                    post.IsExcludeReply = excludedReply;

                    // 移動されなかったらRecentに追加
                    if (!moved)
                        homeTab.AddPostQueue(post);

                    // 除外ルール適用のないReplyならReplyタブに追加
                    if (post.IsReply && !excludedReply)
                        replyTab.AddPostQueue(post);

                    // Fav済み発言だったらFavoritesタブに追加
                    if (post.IsFav)
                        favTab.AddPostQueue(post);

                    adddedCount++;
                }

                return adddedCount;
            }
        }

        public void AddPost(PostClass item)
        {
            Debug.Assert(!item.IsDm, "DM は TabClass.AddPostToInnerStorage を使用する");

            lock (this.lockObj)
            {
                if (this.IsMuted(item, isHomeTimeline: true))
                    return;

                if (this.Posts.TryGetValue(item.StatusId, out var status))
                {
                    if (item.IsFav)
                    {
                        if (item.RetweetedId == null)
                        {
                            status.IsFav = true;
                        }
                        else
                        {
                            item.IsFav = false;
                        }
                    }
                    else
                    {
                        return;        // 追加済みなら何もしない
                    }
                }
                else
                {
                    if (item.IsFav && item.RetweetedId != null) item.IsFav = false;

                    // 既に持っている公式RTは捨てる
                    if (item.RetweetedId != null && SettingManager.Instance.Common.HideDuplicatedRetweets)
                    {
                        var retweetCount = this.UpdateRetweetCount(item);

                        if (retweetCount > 1 && !item.IsMe)
                            return;
                    }

                    if (this.BlockIds.Contains(item.UserId))
                        return;

                    this.Posts.TryAdd(item.StatusId, item);
                }
                if (item.IsFav && this.retweetsCount.ContainsKey(item.StatusId))
                {
                    return;    // Fav済みのRetweet元発言は追加しない
                }
                this.addQueue.Enqueue(item.StatusId);
            }
        }

        public bool IsMuted(PostClass post, bool isHomeTimeline)
        {
            var muteTab = this.MuteTab;
            if (muteTab != null && muteTab.AddFiltered(post) == MyCommon.HITRESULT.Move)
                return true;

            // これ以降は Twitter 標準のミュート機能に準じた判定
            // 参照: https://support.twitter.com/articles/20171399-muting-users-on-twitter

            // ホームタイムライン以外 (検索・リストなど) は対象外
            if (!isHomeTimeline)
                return false;

            // リプライはミュート対象外
            if (post.IsReply)
                return false;

            if (this.MuteUserIds.Contains(post.UserId))
                return true;

            if (post.RetweetedByUserId != null && this.MuteUserIds.Contains(post.RetweetedByUserId.Value))
                return true;

            return false;
        }

        private int UpdateRetweetCount(PostClass retweetPost)
        {
            if (retweetPost.RetweetedId == null)
                throw new InvalidOperationException();

            var retweetedId = retweetPost.RetweetedId.Value;

            return this.retweetsCount.AddOrUpdate(retweetedId, 1, (k, v) => v >= 10 ? 1 : v + 1);
        }

        public bool AddQuoteTweet(PostClass item)
        {
            lock (this.lockObj)
            {
                if (this.IsMuted(item, isHomeTimeline: false) || this.BlockIds.Contains(item.UserId))
                    return false;

                this.quotes[item.StatusId] = item;
                return true;
            }
        }

        /// <summary>
        /// 全てのタブを横断して既読状態を変更します
        /// </summary>
        /// <param name="statusId">変更するツイートのID</param>
        /// <param name="read">既読状態</param>
        /// <returns>既読状態に変化があれば true、変化がなければ false</returns>
        public bool SetReadAllTab(long statusId, bool read)
        {
            lock (this.lockObj)
            {
                foreach (var tab in this.Tabs)
                {
                    if (!tab.Contains(statusId))
                        continue;

                    tab.SetReadState(statusId, read);
                }

                // TabInformations自身が保持しているツイートであればここで IsRead を変化させる
                if (this.Posts.TryGetValue(statusId, out var post))
                    post.IsRead = read;

                return true;
            }
        }

        /// <summary>
        /// Home タブのツイートを全て既読にします。
        /// ただし IsReply または FilterHit が true なものを除きます。
        /// </summary>
        public void SetReadHomeTab()
        {
            var homeTab = this.HomeTab;

            lock (this.lockObj)
            {
                foreach (var statusId in homeTab.GetUnreadIds())
                {
                    if (!this.Posts.TryGetValue(statusId, out var post))
                        continue;

                    if (post.IsReply || post.FilterHit)
                        continue;

                    this.SetReadAllTab(post.StatusId, read: true);
                }
            }
        }

        public PostClass? this[long id]
        {
            get
            {
                if (this.Posts.TryGetValue(id, out var status))
                    return status;

                if (this.quotes.TryGetValue(id, out status))
                    return status;

                return this.GetTabsInnerStorageType()
                    .Select(x => x.Posts.TryGetValue(id, out status) ? status : null)
                    .FirstOrDefault(x => x != null);
            }
        }

        public bool ContainsKey(long id)
        {
            // DM,公式検索は非対応
            lock (this.lockObj)
            {
                return this.Posts.ContainsKey(id);
            }
        }

        public void RenameTab(string original, string newName)
        {
            lock (this.lockObj)
            {
                var index = this.Tabs.IndexOf(original);
                var tb = this.Tabs[original];
                this.tabs.RemoveAt(index);
                tb.TabName = newName;

                if (this.SelectedTabName == original)
                    this.SelectedTabName = newName;

                this.tabs.Insert(index, tb);
            }
        }

        public void FilterAll()
        {
            lock (this.lockObj)
            {
                var homeTab = this.HomeTab;
                var detachedIdsAll = Enumerable.Empty<long>();

                foreach (var tab in this.Tabs.OfType<FilterTabModel>().ToArray())
                {
                    // フィルタに変更のあったタブのみを対象とする
                    if (!tab.FilterModified)
                        continue;

                    tab.FilterModified = false;

                    // フィルタ実行前の時点でタブに含まれていたstatusIdを記憶する
                    var orgIds = tab.StatusIds;
                    tab.ClearIDs();

                    foreach (var post in this.Posts.Values)
                    {
                        var filterHit = false; // フィルタにヒットしたタブがあるか
                        var mark = false; // フィルタによってマーク付けされたか
                        var excluded = false; // 除外フィルタによって除外されたか
                        var moved = false; // Recentタブから移動するか (Recentタブに表示しない)

                        switch (tab.AddFiltered(post, immediately: true))
                        {
                            case MyCommon.HITRESULT.Copy:
                                filterHit = true;
                                break;
                            case MyCommon.HITRESULT.CopyAndMark:
                                filterHit = true;
                                mark = true;
                                break;
                            case MyCommon.HITRESULT.Move:
                                filterHit = true;
                                moved = true;
                                break;
                            case MyCommon.HITRESULT.None:
                                break;
                            case MyCommon.HITRESULT.Exclude:
                                excluded = true;
                                break;
                        }

                        post.FilterHit = filterHit;
                        post.IsMark = mark;

                        // 移動されたらRecentから除去
                        if (moved)
                            homeTab.RemovePostImmediately(post.StatusId);

                        if (tab.TabType == MyCommon.TabUsageType.Mentions)
                        {
                            post.IsExcludeReply = excluded;

                            // 除外ルール適用のないReplyならReplyタブに追加
                            if (post.IsReply && !excluded)
                                tab.AddPostImmediately(post.StatusId, post.IsRead);
                        }
                    }

                    // フィルタの更新によってタブから取り除かれたツイートのID
                    var detachedIds = orgIds.Except(tab.StatusIds).ToArray();

                    detachedIdsAll = detachedIdsAll.Concat(detachedIds);
                }

                // detachedIdsAll のうち、最終的にどのタブにも振り分けられていないツイートがあればRecentに追加
                foreach (var id in detachedIdsAll)
                {
                    var hit = false;
                    foreach (var tbTemp in this.Tabs.ToArray())
                    {
                        if (!tbTemp.IsDistributableTabType)
                            continue;

                        if (tbTemp.Contains(id))
                        {
                            hit = true;
                            break;
                        }
                    }

                    if (!hit)
                    {
                        if (this.Posts.TryGetValue(id, out var post))
                            homeTab.AddPostImmediately(post.StatusId, post.IsRead);
                    }
                }
            }
        }

        public void ClearTabIds(string tabName)
        {
            // 不要なPostを削除
            lock (this.lockObj)
            {
                var tb = this.Tabs[tabName];
                if (!tb.IsInnerStorageTabType)
                {
                    foreach (var id in tb.StatusIds)
                    {
                        var hit = false;
                        foreach (var tab in this.Tabs)
                        {
                            if (tab.Contains(id))
                            {
                                hit = true;
                                break;
                            }
                        }
                        if (!hit)
                            this.Posts.TryRemove(id, out var removedPost);
                    }
                }

                // 指定タブをクリア
                tb.ClearIDs();
            }
        }

        public void RefreshOwl(ISet<long> follower)
        {
            lock (this.lockObj)
            {
                var allPosts = this.GetTabsInnerStorageType()
                    .SelectMany(x => x.Posts.Values)
                    .Concat(this.Posts.Values);

                if (follower.Count > 0)
                {
                    foreach (var post in allPosts)
                    {
                        if (post.IsMe)
                        {
                            post.IsOwl = false;
                        }
                        else
                        {
                            post.IsOwl = !follower.Contains(post.UserId);
                        }
                    }
                }
                else
                {
                    foreach (var post in allPosts)
                    {
                        post.IsOwl = false;
                    }
                }
            }
        }

        public HomeTabModel HomeTab
            => this.GetTabByType<HomeTabModel>()!;

        public DirectMessagesTabModel DirectMessageTab
            => this.GetTabByType<DirectMessagesTabModel>()!;

        public MentionsTabModel MentionTab
            => this.GetTabByType<MentionsTabModel>()!;

        public FavoritesTabModel FavoriteTab
            => this.GetTabByType<FavoritesTabModel>()!;

        public TabModel? GetTabByType(MyCommon.TabUsageType tabType)
        {
            // Home,Mentions,DM,Favは1つに制限する
            // その他のタイプを指定されたら、最初に合致したものを返す
            // 合致しなければnullを返す
            lock (this.lockObj)
            {
                return this.Tabs.FirstOrDefault(x => x.TabType.HasFlag(tabType));
            }
        }

        public T? GetTabByType<T>()
            where T : TabModel
        {
            lock (this.lockObj)
                return this.Tabs.OfType<T>().FirstOrDefault();
        }

        public TabModel[] GetTabsByType(MyCommon.TabUsageType tabType)
        {
            lock (this.lockObj)
            {
                return this.Tabs
                    .Where(x => x.TabType.HasFlag(tabType))
                    .ToArray();
            }
        }

        public T[] GetTabsByType<T>()
            where T : TabModel
        {
            lock (this.lockObj)
                return this.Tabs.OfType<T>().ToArray();
        }

        public TabModel[] GetTabsInnerStorageType()
        {
            lock (this.lockObj)
            {
                return this.Tabs
                    .Where(x => x.IsInnerStorageTabType)
                    .ToArray();
            }
        }

        public TabModel? GetTabByName(string tabName)
        {
            lock (this.lockObj)
            {
                return this.Tabs.TryGetValue(tabName, out var tab)
                    ? tab
                    : null;
            }
        }
    }
}
