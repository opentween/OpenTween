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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTween.Models
{
    public sealed class TabInformations
    {
        //個別タブの情報をDictionaryで保持
        private Dictionary<string, TabModel> _tabs = new Dictionary<string, TabModel>();
        private ConcurrentDictionary<long, PostClass> _statuses = new ConcurrentDictionary<long, PostClass>();
        private Dictionary<long, PostClass> _retweets = new Dictionary<long, PostClass>();
        private Dictionary<long, PostClass> _quotes = new Dictionary<long, PostClass>();
        private Stack<TabModel> _removedTab = new Stack<TabModel>();

        public ISet<long> BlockIds = new HashSet<long>();
        public ISet<long> MuteUserIds = new HashSet<long>();

        //発言の追加
        //AddPost(複数回) -> DistributePosts          -> SubmitUpdate

        private ConcurrentQueue<long> addQueue = new ConcurrentQueue<long>();

        /// <summary>通知サウンドを再生する優先順位</summary>
        private Dictionary<MyCommon.TabUsageType, int> notifyPriorityByTabType = new Dictionary<MyCommon.TabUsageType, int>
        {
            [MyCommon.TabUsageType.DirectMessage] = 100,
            [MyCommon.TabUsageType.Mentions] = 90,
            [MyCommon.TabUsageType.UserDefined] = 80,
            [MyCommon.TabUsageType.Home] = 70,
            [MyCommon.TabUsageType.Favorites] = 60,
        };

        //トランザクション用
        private readonly object LockObj = new object();

        private static TabInformations _instance = new TabInformations();

        //List
        private List<ListElement> _lists = new List<ListElement>();

        private TabInformations()
        {
        }

        public static TabInformations GetInstance()
        {
            return _instance;    //singleton
        }

        public List<ListElement> SubscribableLists
        {
            get
            {
                return _lists;
            }
            set
            {
                if (value != null && value.Count > 0)
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
                _lists = value;
            }
        }

        public bool AddTab(TabModel tab)
        {
            if (this._tabs.ContainsKey(tab.TabName))
                return false;

            this._tabs.Add(tab.TabName, tab);
            tab.SetSortMode(this.SortMode, this.SortOrder);

            return true;
        }

        //public void AddTab(string TabName, TabClass Tab)
        //{
        //    _tabs.Add(TabName, Tab);
        //}

        public void RemoveTab(string TabName)
        {
            lock (LockObj)
            {
                var tb = GetTabByName(TabName);
                if (tb.IsDefaultTabType) return; //念のため

                if (!tb.IsInnerStorageTabType)
                {
                    var homeTab = GetTabByType(MyCommon.TabUsageType.Home);
                    var dmTab = GetTabByType(MyCommon.TabUsageType.DirectMessage);

                    for (int idx = 0; idx < tb.AllCount; ++idx)
                    {
                        var exist = false;
                        var Id = tb.GetStatusIdAt(idx);
                        if (Id < 0) continue;
                        foreach (var tab in _tabs.Values)
                        {
                            if (tab != tb && tab != dmTab)
                            {
                                if (tab.Contains(Id))
                                {
                                    exist = true;
                                    break;
                                }
                            }
                        }
                        if (!exist) homeTab.AddPostImmediately(Id, _statuses[Id].IsRead);
                    }
                }
                _removedTab.Push(tb);
                _tabs.Remove(TabName);
            }
        }

        public Stack<TabModel> RemovedTab
        {
            get { return _removedTab; }
        }

        public bool ContainsTab(string TabText)
        {
            return _tabs.ContainsKey(TabText);
        }

        public bool ContainsTab(TabModel ts)
        {
            return _tabs.ContainsValue(ts);
        }

        /// <summary>
        /// 指定されたタブ名を元に、既存のタブ名との重複を避けた名前を生成します
        /// </summary>
        /// <param name="baseTabName">作成したいタブ名</param>
        /// <returns>生成されたタブ名</returns>
        /// <exception cref="TabException">タブ名の生成を 100 回試行して失敗した場合</exception>
        public string MakeTabName(string baseTabName)
        {
            return this.MakeTabName(baseTabName, 100);
        }

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

        public Dictionary<string, TabModel> Tabs
        {
            get
            {
                return _tabs;
            }
            set
            {
                _tabs = value;
            }
        }

        public Dictionary<string, TabModel>.KeyCollection KeysTab
        {
            get
            {
                return _tabs.Keys;
            }
        }

        public SortOrder SortOrder { get; private set; }

        public ComparerMode SortMode { get; private set; }

        public void SetSortMode(ComparerMode mode, SortOrder sortOrder)
        {
            this.SortMode = mode;
            this.SortOrder = sortOrder;

            foreach (var tab in this._tabs.Values)
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

        //    public PostClass RetweetSource(long Id)
        //    {
        //        get
        //        {
        //            if (_retweets.ContainsKey(Id))
        //            {
        //                return _retweets[Id];
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }
        public PostClass RetweetSource(long Id)
        {
            PostClass status;
            return _retweets.TryGetValue(Id, out status)
                ? status
                : null;
        }

        public void ScrubGeoReserve(long id, long upToStatusId)
        {
            lock (LockObj)
            {
                //this._scrubGeo.Add(new ScrubGeoInfo With {.UserId = id, .UpToStatusId = upToStatusId});
                this.ScrubGeo(id, upToStatusId);
            }
        }

        private void ScrubGeo(long userId, long upToStatusId)
        {
            lock (LockObj)
            {
                var userPosts = from post in this._statuses.Values
                                where post.UserId == userId && post.UserId <= upToStatusId
                                select post;

                foreach (var p in userPosts)
                {
                    p.PostGeo = null;
                }

                var userPosts2 = from tb in this.GetTabsInnerStorageType()
                                 from post in tb.Posts.Values
                                 where post.UserId == userId && post.UserId <= upToStatusId
                                 select post;

                foreach (var p in userPosts2)
                {
                    p.PostGeo = null;
                }
            }
        }

        public void RemovePostFromAllTabs(long statusId, bool setIsDeleted)
        {
            foreach (var tab in this.Tabs.Values)
            {
                tab.EnqueueRemovePost(statusId, setIsDeleted);
            }
        }

        public int SubmitUpdate()
        {
            string soundFile;
            PostClass[] notifyPosts;
            bool newMentionOrDm, isDeletePost;

            return this.SubmitUpdate(out soundFile, out notifyPosts, out newMentionOrDm,
                out isDeletePost);
        }

        public int SubmitUpdate(out string soundFile, out PostClass[] notifyPosts,
            out bool newMentionOrDm, out bool isDeletePost)
        {
            // 注：メインスレッドから呼ぶこと
            lock (this.LockObj)
            {
                soundFile = "";
                notifyPosts = new PostClass[0];
                newMentionOrDm = false;
                isDeletePost = false;

                var addedCountTotal = 0;
                var removedIdsAll = new List<long>();
                var notifyPostsList = new List<PostClass>();

                var currentNotifyPriority = -1;

                foreach (var tab in this._tabs.Values)
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
                                PostClass post;
                                if (tab.Posts.TryGetValue(statusId, out post))
                                    notifyPostsList.Add(post);
                            }
                        }

                        // 通知サウンドは TabClass.Notify の値に関わらず鳴らす
                        // SettingCommon.PlaySound が false であれば TweenMain 側で無効化される
                        if (!string.IsNullOrEmpty(tab.SoundFile))
                        {
                            int notifyPriority;
                            if (!this.notifyPriorityByTabType.TryGetValue(tab.TabType, out notifyPriority))
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
                    PostClass removedPost;
                    this._statuses.TryRemove(removedId, out removedPost);
                }

                return addedCountTotal;
            }
        }

        public int DistributePosts()
        {
            lock (this.LockObj)
            {
                var homeTab = this.GetTabByType(MyCommon.TabUsageType.Home);
                var replyTab = this.GetTabByType(MyCommon.TabUsageType.Mentions);
                var favTab = this.GetTabByType(MyCommon.TabUsageType.Favorites);

                var distributableTabs = this.GetTabsByType<FilterTabModel>()
                    .ToArray();

                var adddedCount = 0;

                long statusId;
                while (this.addQueue.TryDequeue(out statusId))
                {
                    PostClass post;
                    if (!this._statuses.TryGetValue(statusId, out post))
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

        public void AddPost(PostClass Item)
        {
            Debug.Assert(!Item.IsDm, "DM は TabClass.AddPostToInnerStorage を使用する");

            lock (LockObj)
            {
                if (this.IsMuted(Item, isHomeTimeline: true))
                    return;

                PostClass status;
                if (_statuses.TryGetValue(Item.StatusId, out status))
                {
                    if (Item.IsFav)
                    {
                        if (Item.RetweetedId == null)
                        {
                            status.IsFav = true;
                        }
                        else
                        {
                            Item.IsFav = false;
                        }
                    }
                    else
                    {
                        return;        //追加済みなら何もしない
                    }
                }
                else
                {
                    if (Item.IsFav && Item.RetweetedId != null) Item.IsFav = false;
                    //既に持っている公式RTは捨てる
                    if (SettingCommon.Instance.HideDuplicatedRetweets &&
                        !Item.IsMe &&
                        Item.RetweetedId != null &&
                        this._retweets.TryGetValue(Item.RetweetedId.Value, out status) &&
                        status.RetweetedCount > 0) return;

                    if (BlockIds.Contains(Item.UserId))
                        return;

                    _statuses.TryAdd(Item.StatusId, Item);
                }
                if (Item.RetweetedId != null)
                {
                    this.AddRetweet(Item);
                }
                if (Item.IsFav && _retweets.ContainsKey(Item.StatusId))
                {
                    return;    //Fav済みのRetweet元発言は追加しない
                }
                this.addQueue.Enqueue(Item.StatusId);
            }
        }

        public bool IsMuted(PostClass post, bool isHomeTimeline)
        {
            var muteTab = this.GetTabByType<MuteTabModel>();
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

        private void AddRetweet(PostClass item)
        {
            var retweetedId = item.RetweetedId.Value;

            PostClass status;
            if (this._retweets.TryGetValue(retweetedId, out status))
            {
                status.RetweetedCount++;
                if (status.RetweetedCount > 10)
                {
                    status.RetweetedCount = 0;
                }
                return;
            }

            this._retweets.Add(retweetedId, item.ConvertToOriginalPost());
        }

        public bool AddQuoteTweet(PostClass item)
        {
            lock (LockObj)
            {
                if (IsMuted(item, isHomeTimeline: false) || BlockIds.Contains(item.UserId))
                    return false;

                _quotes[item.StatusId] = item;
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
            lock (LockObj)
            {
                foreach (var tab in this._tabs.Values)
                {
                    if (!tab.Contains(statusId))
                        continue;

                    tab.SetReadState(statusId, read);
                }

                // TabInformations自身が保持しているツイートであればここで IsRead を変化させる
                PostClass post;
                if (this.Posts.TryGetValue(statusId, out post))
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
            var homeTab = this.GetTabByType(MyCommon.TabUsageType.Home);

            lock (LockObj)
            {
                foreach (var statusId in homeTab.GetUnreadIds())
                {
                    PostClass post;
                    if (!this.Posts.TryGetValue(statusId, out post))
                        continue;

                    if (post.IsReply || post.FilterHit)
                        continue;

                    this.SetReadAllTab(post.StatusId, read: true);
                }
            }
        }

        public PostClass this[long ID]
        {
            get
            {
                PostClass status;
                if (this._statuses.TryGetValue(ID, out status))
                    return status;

                if (this._retweets.TryGetValue(ID, out status))
                    return status;

                if (this._quotes.TryGetValue(ID, out status))
                    return status;

                return this.GetTabsInnerStorageType()
                    .Select(x => x.Posts.TryGetValue(ID, out status) ? status : null)
                    .Where(x => x != null)
                    .FirstOrDefault();
            }
        }

        public bool ContainsKey(long Id)
        {
            //DM,公式検索は非対応
            lock (LockObj)
            {
                return _statuses.ContainsKey(Id);
            }
        }

        public void RenameTab(string Original, string NewName)
        {
            var tb = _tabs[Original];
            _tabs.Remove(Original);
            tb.TabName = NewName;
            _tabs.Add(NewName, tb);
        }

        public void FilterAll()
        {
            lock (LockObj)
            {
                var homeTab = GetTabByType(MyCommon.TabUsageType.Home);
                var detachedIdsAll = Enumerable.Empty<long>();

                foreach (var tab in _tabs.Values.OfType<FilterTabModel>().ToArray())
                {
                    if (tab.TabType == MyCommon.TabUsageType.Mute)
                        continue;

                    // フィルタに変更のあったタブのみを対象とする
                    if (!tab.FilterModified)
                        continue;

                    tab.FilterModified = false;

                    // フィルタ実行前の時点でタブに含まれていたstatusIdを記憶する
                    var orgIds = tab.StatusIds;
                    tab.ClearIDs();

                    foreach (var post in _statuses.Values)
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
                    foreach (var tbTemp in _tabs.Values.ToArray())
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
                        PostClass post;
                        if (this._statuses.TryGetValue(id, out post))
                            homeTab.AddPostImmediately(post.StatusId, post.IsRead);
                    }
                }
            }
        }

        public void ClearTabIds(string TabName)
        {
            //不要なPostを削除
            lock (LockObj)
            {
                var tb = _tabs[TabName];
                if (!tb.IsInnerStorageTabType)
                {
                    foreach (var Id in tb.StatusIds)
                    {
                        var Hit = false;
                        foreach (var tab in _tabs.Values)
                        {
                            if (tab.Contains(Id))
                            {
                                Hit = true;
                                break;
                            }
                        }
                        if (!Hit)
                        {
                            PostClass removedPost;
                            _statuses.TryRemove(Id, out removedPost);
                        }
                    }
                }

                //指定タブをクリア
                tb.ClearIDs();
            }
        }

        public void RefreshOwl(ISet<long> follower)
        {
            lock (LockObj)
            {
                if (follower.Count > 0)
                {
                    foreach (var post in _statuses.Values)
                    {
                        //if (post.UserId = 0 || post.IsDm) Continue For
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
                    foreach (var post in _statuses.Values)
                    {
                        post.IsOwl = false;
                    }
                }
            }
        }

        public TabModel GetTabByType(MyCommon.TabUsageType tabType)
        {
            //Home,Mentions,DM,Favは1つに制限する
            //その他のタイプを指定されたら、最初に合致したものを返す
            //合致しなければnullを返す
            lock (LockObj)
            {
                return this._tabs.Values
                    .FirstOrDefault(x => x.TabType.HasFlag(tabType));
            }
        }

        public T GetTabByType<T>() where T : TabModel
        {
            lock (this.LockObj)
                return this._tabs.Values.OfType<T>().FirstOrDefault();
        }

        public TabModel[] GetTabsByType(MyCommon.TabUsageType tabType)
        {
            lock (LockObj)
            {
                return this._tabs.Values
                    .Where(x => x.TabType.HasFlag(tabType))
                    .ToArray();
            }
        }

        public T[] GetTabsByType<T>() where T : TabModel
        {
            lock (this.LockObj)
                return this._tabs.Values.OfType<T>().ToArray();
        }

        public TabModel[] GetTabsInnerStorageType()
        {
            lock (LockObj)
            {
                return this._tabs.Values
                    .Where(x => x.IsInnerStorageTabType)
                    .ToArray();
            }
        }

        public TabModel GetTabByName(string tabName)
        {
            lock (LockObj)
            {
                TabModel tab;
                return _tabs.TryGetValue(tabName, out tab)
                    ? tab
                    : null;
            }
        }

        public ConcurrentDictionary<long, PostClass> Posts
            => this._statuses;
    }
}
