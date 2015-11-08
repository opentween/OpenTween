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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace OpenTween
{
    public class PostClass : ICloneable
    {
        public struct StatusGeo : IEquatable<StatusGeo>
        {
            public double Longitude { get; }
            public double Latitude { get; }

            public StatusGeo(double longitude, double latitude)
            {
                this.Longitude = longitude;
                this.Latitude = latitude;
            }

            public override int GetHashCode()
                => this.Longitude.GetHashCode() ^ this.Latitude.GetHashCode();

            public override bool Equals(object obj)
                => obj is StatusGeo ? this.Equals((StatusGeo)obj) : false;

            public bool Equals(StatusGeo other)
                => this.Longitude == other.Longitude && this.Latitude == other.Longitude;

            public static bool operator ==(StatusGeo left, StatusGeo right)
                => left.Equals(right);

            public static bool operator !=(StatusGeo left, StatusGeo right)
                => !left.Equals(right);
        }
        public string Nickname { get; set; }
        public string TextFromApi { get; set; }
        public string ImageUrl { get; set; }
        public string ScreenName { get; set; }
        public DateTime CreatedAt { get; set; }
        public long StatusId { get; set; }
        private bool _IsFav;
        public string Text { get; set; }
        public bool IsRead { get; set; }
        public bool IsReply { get; set; }
        public bool IsExcludeReply { get; set; }
        private bool _IsProtect;
        public bool IsOwl { get; set; }
        private bool _IsMark;
        public string InReplyToUser { get; set; }
        private long? _InReplyToStatusId;
        public string Source { get; set; }
        public Uri SourceUri { get; set; }
        public List<string> ReplyToList { get; set; }
        public bool IsMe { get; set; }
        public bool IsDm { get; set; }
        public long UserId { get; set; }
        public bool FilterHit { get; set; }
        public string RetweetedBy { get; set; }
        public long? RetweetedId { get; set; }
        private bool _IsDeleted = false;
        private StatusGeo? _postGeo = null;
        public int RetweetedCount { get; set; }
        public long? RetweetedByUserId { get; set; }
        public long? InReplyToUserId { get; set; }
        public List<MediaInfo> Media { get; set; }
        public long[] QuoteStatusIds { get; set; }

        public int FavoritedCount { get; set; }

        private States _states = States.None;

        [Flags]
        private enum States
        {
            None = 0,
            Protect = 1,
            Mark = 2,
            Reply = 4,
            Geo = 8,
        }

        public PostClass()
        {
            RetweetedBy = "";
            Media = new List<MediaInfo>();
            ReplyToList = new List<string>();
            QuoteStatusIds = new long[0];
        }

        public string TextSingleLine
        {
            get
            {
                return this.TextFromApi == null ? null : this.TextFromApi.Replace("\n", " ");
            }
        }

        public bool IsFav
        {
            get
            {
                if (this.RetweetedId != null)
                {
                    var post = this.RetweetSource;
                    if (post != null)
                    {
                        return post.IsFav;
                    }
                }

                return _IsFav;
            }
            set
            {
                _IsFav = value;
                if (this.RetweetedId != null)
                {
                    var post = this.RetweetSource;
                    if (post != null)
                    {
                        post.IsFav = value;
                    }
                }
            }
        }

        public bool IsProtect
        {
            get
            {
                return _IsProtect;
            }
            set
            {
                if (value)
                {
                    _states = _states | States.Protect;
                }
                else
                {
                    _states = _states & ~States.Protect;
                }
                _IsProtect = value;
            }
        }
        public bool IsMark
        {
            get
            {
                return _IsMark;
            }
            set
            {
                if (value)
                {
                    _states = _states | States.Mark;
                }
                else
                {
                    _states = _states & ~States.Mark;
                }
                _IsMark = value;
            }
        }
        public long? InReplyToStatusId
        {
            get
            {
                return _InReplyToStatusId;
            }
            set
            {
                if (value != null)
                {
                    _states = _states | States.Reply;
                }
                else
                {
                    _states = _states & ~States.Reply;
                }
                _InReplyToStatusId = value;
            }
        }

        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (value)
                {
                    this.InReplyToStatusId = null;
                    this.InReplyToUser = "";
                    this.InReplyToUserId = null;
                    this.IsReply = false;
                    this.ReplyToList = new List<string>();
                    this._states = States.None;
                }
                _IsDeleted = value;
            }
        }

        protected virtual PostClass RetweetSource
        {
            get
            {
                return TabInformations.GetInstance().RetweetSource(this.RetweetedId.Value);
            }
        }

        public StatusGeo? PostGeo
        {
            get
            {
                return _postGeo;
            }
            set
            {
                if (value != null)
                {
                    _states |= States.Geo;
                }
                else
                {
                    _states &= ~States.Geo;
                }
                _postGeo = value;
            }
        }

        public int StateIndex
        {
            get
            {
                return (int)_states - 1;
            }
        }

        // 互換性のために用意
        public string SourceHtml
        {
            get
            {
                if (this.SourceUri == null)
                    return WebUtility.HtmlEncode(this.Source);

                return string.Format("<a href=\"{0}\" rel=\"nofollow\">{1}</a>",
                    WebUtility.HtmlEncode(this.SourceUri.AbsoluteUri), WebUtility.HtmlEncode(this.Source));
            }
        }

        /// <summary>
        /// このツイートが指定したユーザーによって削除可能であるかを判定します
        /// </summary>
        /// <param name="selfUserId">ツイートを削除しようとするユーザーのID</param>
        /// <returns>削除可能であれば true、そうでなければ false</returns>
        public bool CanDeleteBy(long selfUserId)
        {
            // 自分が送った DM と自分に届いた DM のどちらも削除可能
            if (this.IsDm)
                return true;

            // 自分のツイート or 他人に RT された自分のツイート
            if (this.UserId == selfUserId)
                return true;

            // 自分が RT したツイート
            if (this.RetweetedByUserId == selfUserId)
                return true;

            return false;
        }

        public PostClass ConvertToOriginalPost()
        {
            if (this.RetweetedId == null)
                throw new InvalidOperationException();

            var originalPost = this.Clone();

            originalPost.StatusId = this.RetweetedId.Value;
            originalPost.RetweetedId = null;
            originalPost.RetweetedBy = "";
            originalPost.RetweetedByUserId = null;
            originalPost.RetweetedCount = 1;

            return originalPost;
        }

        public PostClass Clone()
        {
            var clone = (PostClass)this.MemberwiseClone();
            clone.ReplyToList = new List<string>(this.ReplyToList);
            clone.Media = new List<MediaInfo>(this.Media);
            clone.QuoteStatusIds = this.QuoteStatusIds.ToArray();

            return clone;
        }

        object ICloneable.Clone()
            => this.Clone();

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this.Equals((PostClass)obj);
        }

        public bool Equals(PostClass other)
        {
            if (other == null) return false;
            return (this.Nickname == other.Nickname) &&
                    (this.TextFromApi == other.TextFromApi) &&
                    (this.ImageUrl == other.ImageUrl) &&
                    (this.ScreenName == other.ScreenName) &&
                    (this.CreatedAt == other.CreatedAt) &&
                    (this.StatusId == other.StatusId) &&
                    (this.IsFav == other.IsFav) &&
                    (this.Text == other.Text) &&
                    (this.IsRead == other.IsRead) &&
                    (this.IsReply == other.IsReply) &&
                    (this.IsExcludeReply == other.IsExcludeReply) &&
                    (this.IsProtect == other.IsProtect) &&
                    (this.IsOwl == other.IsOwl) &&
                    (this.IsMark == other.IsMark) &&
                    (this.InReplyToUser == other.InReplyToUser) &&
                    (this.InReplyToStatusId == other.InReplyToStatusId) &&
                    (this.Source == other.Source) &&
                    (this.SourceUri == other.SourceUri) &&
                    (this.ReplyToList.SequenceEqual(other.ReplyToList)) &&
                    (this.IsMe == other.IsMe) &&
                    (this.IsDm == other.IsDm) &&
                    (this.UserId == other.UserId) &&
                    (this.FilterHit == other.FilterHit) &&
                    (this.RetweetedBy == other.RetweetedBy) &&
                    (this.RetweetedId == other.RetweetedId) &&
                    (this.IsDeleted == other.IsDeleted) &&
                    (this.InReplyToUserId == other.InReplyToUserId);

        }

        public override int GetHashCode()
        {
            return this.StatusId.GetHashCode();
        }
    }

    public class MediaInfo
    {
        public string Url { get; }
        public string VideoUrl { get; }

        public MediaInfo(string url)
            : this(url, null)
        {
        }

        public MediaInfo(string url, string videoUrl)
        {
            this.Url = url;
            this.VideoUrl = !string.IsNullOrEmpty(videoUrl) ? videoUrl : null;
        }

        public override bool Equals(object obj)
        {
            var info = obj as MediaInfo;
            return info != null &&
                info.Url == this.Url &&
                info.VideoUrl == this.VideoUrl;
        }

        public override int GetHashCode()
        {
            return (this.Url == null ? 0 : this.Url.GetHashCode()) ^
                   (this.VideoUrl == null ? 0 : this.VideoUrl.GetHashCode());
        }

        public override string ToString()
        {
            return this.Url;
        }
    }

    public sealed class TabInformations
    {
        //個別タブの情報をDictionaryで保持
        private Dictionary<string, TabClass> _tabs = new Dictionary<string, TabClass>();
        private ConcurrentDictionary<long, PostClass> _statuses = new ConcurrentDictionary<long, PostClass>();
        private Dictionary<long, PostClass> _retweets = new Dictionary<long, PostClass>();
        private Dictionary<long, PostClass> _quotes = new Dictionary<long, PostClass>();
        private Stack<TabClass> _removedTab = new Stack<TabClass>();

        public ISet<long> BlockIds = new HashSet<long>();
        public ISet<long> MuteUserIds = new HashSet<long>();

        //発言の追加
        //AddPost(複数回) -> DistributePosts          -> SubmitUpdate

        private ConcurrentQueue<long> addQueue = new ConcurrentQueue<long>();
        private ConcurrentQueue<long> deleteQueue = new ConcurrentQueue<long>();

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
                    foreach (var tb in this.GetTabsByType(MyCommon.TabUsageType.Lists))
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

        public bool AddTab(string TabName, MyCommon.TabUsageType TabType, ListElement List)
        {
            if (_tabs.ContainsKey(TabName)) return false;
            var tb = new TabClass(TabName, TabType, List);
            _tabs.Add(TabName, tb);
            tb.SetSortMode(this.SortMode, this.SortOrder);
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
                        var Id = tb.GetId(idx);
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

        public Stack<TabClass> RemovedTab
        {
            get { return _removedTab; }
        }

        public bool ContainsTab(string TabText)
        {
            return _tabs.ContainsKey(TabText);
        }

        public bool ContainsTab(TabClass ts)
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

        public Dictionary<string, TabClass> Tabs
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

        public Dictionary<string, TabClass>.KeyCollection KeysTab
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

        public void RemoveFavPost(long Id)
        {
            lock (LockObj)
            {
                PostClass post;
                var tab = this.GetTabByType(MyCommon.TabUsageType.Favorites);

                if (_statuses.TryGetValue(Id, out post))
                {
                    //指定タブから該当ID削除
                    var tType = tab.TabType;
                    if (tab.Contains(Id))
                        tab.Remove(Id);

                    //FavタブからRetweet発言を削除する場合は、他の同一参照Retweetも削除
                    if (tType == MyCommon.TabUsageType.Favorites && post.RetweetedId != null)
                    {
                        for (int i = 0; i < tab.AllCount; i++)
                        {
                            PostClass rPost = null;
                            try
                            {
                                rPost = tab[i];
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                break;
                            }
                            if (rPost.RetweetedId != null && rPost.RetweetedId == post.RetweetedId)
                            {
                                tab.Remove(rPost.StatusId);
                            }
                        }
                    }
                }
                //TabType=PublicSearchの場合（Postの保存先がTabClass内）
                //if (tab.Contains(StatusId) &&
                //   (tab.TabType = MyCommon.TabUsageType.PublicSearch || tab.TabType = MyCommon.TabUsageType.DirectMessage))
                //{
                //    tab.Remove(StatusId);
                //}
            }
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

        public void RemovePostReserve(long id)
        {
            lock (LockObj)
            {
                this.deleteQueue.Enqueue(id);
                this.DeletePost(id);   //UI選択行がずれるため、RemovePostは使用しない
            }
        }

        public void RemovePost(long Id)
        {
            lock (LockObj)
            {
                //各タブから該当ID削除
                foreach (var tab in _tabs.Values)
                {
                    if (tab.Contains(Id))
                        tab.Remove(Id);
                }

                PostClass removedPost;
                _statuses.TryRemove(Id, out removedPost);
            }
        }

        private void DeletePost(long Id)
        {
            lock (LockObj)
            {
                PostClass post;
                if (_statuses.TryGetValue(Id, out post))
                {
                    post.IsDeleted = true;
                }
                foreach (var tb in this.GetTabsInnerStorageType())
                {
                    if (tb.Contains(Id))
                    {
                        post = tb.Posts[Id];
                        post.IsDeleted = true;
                    }
                }
            }
        }

        public int SubmitUpdate(bool isUserStream = false)
        {
            string soundFile;
            PostClass[] notifyPosts;
            bool isMentionIncluded, isDeletePost;

            return this.SubmitUpdate(out soundFile, out notifyPosts, out isMentionIncluded,
                out isDeletePost, isUserStream);
        }

        public int SubmitUpdate(out string soundFile, out PostClass[] notifyPosts,
            out bool isMentionIncluded, out bool isDeletePost, bool isUserStream)
        {
            // 注：メインスレッドから呼ぶこと
            lock (this.LockObj)
            {
                soundFile = "";
                notifyPosts = new PostClass[0];
                isMentionIncluded = false;
                isDeletePost = false;

                var totalPosts = 0;
                var notifyPostsList = new List<PostClass>();

                var currentNotifyPriority = -1;

                foreach (var tab in this._tabs.Values)
                {
                    // 振分確定 (各タブに反映)
                    var addedIds = tab.AddSubmit(ref isMentionIncluded);

                    if (addedIds.Count != 0 && tab.Notify)
                    {
                        // 通知対象のリストに追加
                        foreach (var statusId in addedIds)
                        {
                            PostClass post;
                            if (tab.Posts.TryGetValue(statusId, out post))
                                notifyPostsList.Add(post);
                        }

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

                    totalPosts += addedIds.Count;
                }

                notifyPosts = notifyPostsList.Distinct().ToArray();

                if (isUserStream)
                {
                    long statusId;
                    while (this.deleteQueue.TryDequeue(out statusId))
                    {
                        this.RemovePost(statusId);
                        isDeletePost = true;
                    }
                }

                return totalPosts;
            }
        }

        public int DistributePosts()
        {
            lock (this.LockObj)
            {
                var homeTab = this.GetTabByType(MyCommon.TabUsageType.Home);
                var replyTab = this.GetTabByType(MyCommon.TabUsageType.Mentions);
                var favTab = this.GetTabByType(MyCommon.TabUsageType.Favorites);

                var distributableTabs = this._tabs.Values.Where(x => x.IsDistributableTabType)
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
                        homeTab.AddPostQueue(post.StatusId, post.IsRead);

                    // 除外ルール適用のないReplyならReplyタブに追加
                    if (post.IsReply && !excludedReply)
                        replyTab.AddPostQueue(post.StatusId, post.IsRead);

                    // Fav済み発言だったらFavoritesタブに追加
                    if (post.IsFav)
                        favTab.AddPostQueue(post.StatusId, post.IsRead);

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
                if (this.IsMuted(Item))
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

        public bool IsMuted(PostClass post)
        {
            var muteTab = this.GetTabByType(MyCommon.TabUsageType.Mute);
            if (muteTab != null && muteTab.AddFiltered(post) == MyCommon.HITRESULT.Move)
                return true;

            // これ以降は Twitter 標準のミュート機能に準じた判定

            // リプライはミュート対象外
            // 参照: https://support.twitter.com/articles/20171399-muting-users-on-twitter
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
                if (IsMuted(item) || BlockIds.Contains(item.UserId))
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

                foreach (var tab in _tabs.Values.ToArray())
                {
                    if (!tab.IsDistributableTabType)
                        continue;

                    if (tab.TabType == MyCommon.TabUsageType.Mute)
                        continue;

                    // フィルタに変更のあったタブのみを対象とする
                    if (!tab.FilterModified)
                        continue;

                    tab.FilterModified = false;

                    // フィルタ実行前の時点でタブに含まれていたstatusIdを記憶する
                    var orgIds = tab.BackupIds;
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
                            homeTab.Remove(post.StatusId);

                        if (tab.TabType == MyCommon.TabUsageType.Mentions)
                        {
                            post.IsExcludeReply = excluded;

                            // 除外ルール適用のないReplyならReplyタブに追加
                            if (post.IsReply && !excluded)
                                tab.AddPostImmediately(post.StatusId, post.IsRead);
                        }
                    }

                    // フィルタの更新によってタブから取り除かれたツイートのID
                    var detachedIds = orgIds.Except(tab.BackupIds).ToArray();

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
                    foreach (var Id in tb.BackupIds)
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

        public TabClass GetTabByType(MyCommon.TabUsageType tabType)
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

        public TabClass[] GetTabsByType(MyCommon.TabUsageType tabType)
        {
            lock (LockObj)
            {
                return this._tabs.Values
                    .Where(x => x.TabType.HasFlag(tabType))
                    .ToArray();
            }
        }

        public TabClass[] GetTabsInnerStorageType()
        {
            lock (LockObj)
            {
                return this._tabs.Values
                    .Where(x => x.IsInnerStorageTabType)
                    .ToArray();
            }
        }

        public TabClass GetTabByName(string tabName)
        {
            lock (LockObj)
            {
                TabClass tab;
                return _tabs.TryGetValue(tabName, out tab)
                    ? tab
                    : null;
            }
        }

        public ConcurrentDictionary<long, PostClass> Posts
            => this._statuses;
    }

    [Serializable]
    public sealed class TabClass
    {
        private List<PostFilterRule> _filters;
        private IndexedSortedSet<long> _ids;
        private ConcurrentQueue<TemporaryId> addQueue = new ConcurrentQueue<TemporaryId>();
        private SortedSet<long> unreadIds = new SortedSet<long>();
        private MyCommon.TabUsageType _tabType = MyCommon.TabUsageType.Undefined;

        private readonly object _lockObj = new object();

        public string User { get; set; }

#region "検索"
        //Search query
        private string _searchLang = "";
        private string _searchWords = "";

        public string SearchLang
        {
            get
            {
                return _searchLang;
            }
            set
            {
                _searchLang = value;
                this.ResetFetchIds();
            }
        }
        public string SearchWords
        {
            get
            {
                return _searchWords;
            }
            set
            {
                _searchWords = value.Trim();
                this.ResetFetchIds();
            }
        }

        private Dictionary<string, string> _beforeQuery = new Dictionary<string, string>();

        public bool IsSearchQueryChanged
        {
            get
            {
                var qry = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(_searchWords))
                {
                    qry.Add("q", _searchWords);
                    if (!string.IsNullOrEmpty(_searchLang)) qry.Add("lang", _searchLang);
                }
                if (qry.Count != _beforeQuery.Count)
                {
                    _beforeQuery = qry;
                    return true;
                }

                foreach (var kvp in qry)
                {
                    string value;
                    if (!_beforeQuery.TryGetValue(kvp.Key, out value) || value != kvp.Value)
                    {
                        _beforeQuery = qry;
                        return true;
                    }
                }
                return false;
            }
        }
#endregion

#region "リスト"
        [NonSerialized]
        private ListElement _listInfo;
        public ListElement ListInfo
        {
            get
            {
                return _listInfo;
            }
            set
            {
                _listInfo = value;
            }
        }
#endregion

        [XmlIgnore]
        public PostClass RelationTargetPost { get; set; }

        [XmlIgnore]
        public long OldestId = long.MaxValue;

        [XmlIgnore]
        public long SinceId { get; set; }

        [XmlIgnore]
        public ConcurrentDictionary<long, PostClass> Posts { get; private set; }

        private ConcurrentDictionary<long, PostClass> _innerPosts;

        private struct TemporaryId
        {
            public long Id;
            public bool Read;

            public TemporaryId(long argId, bool argRead)
            {
                Id = argId;
                Read = argRead;
            }
        }

        public TabClass()
        {
            _innerPosts = new ConcurrentDictionary<long, PostClass>();
            Posts = _innerPosts;
            SoundFile = "";
            TabName = "";
            _filters = new List<PostFilterRule>();
            Protected = false;
            Notify = true;
            SoundFile = "";
            UnreadManage = true;
            _ids = new IndexedSortedSet<long>();
            _tabType = MyCommon.TabUsageType.Undefined;
            _listInfo = null;
        }

        public TabClass(string TabName, MyCommon.TabUsageType TabType, ListElement list) : this()
        {
            this.TabName = TabName;
            this.TabType = TabType;
            this.ListInfo = list;
        }

        /// <summary>
        /// タブ更新時に使用する SinceId, OldestId をリセットする
        /// </summary>
        public void ResetFetchIds()
        {
            this.SinceId = 0L;
            this.OldestId = long.MaxValue;
        }

        /// <summary>
        /// ソート対象のフィールドとソート順を設定し、ソートを実行します
        /// </summary>
        public void SetSortMode(ComparerMode mode, SortOrder sortOrder)
        {
            this.SortMode = mode;
            this.SortOrder = sortOrder;

            this.ApplySortMode();
        }

        private void ApplySortMode()
        {
            var sign = this.SortOrder == SortOrder.Ascending ? 1 : -1;

            Comparison<long> comparison;
            if (this.SortMode == ComparerMode.Id)
            {
                comparison = (x, y) => sign * x.CompareTo(y);
            }
            else
            {
                Comparison<PostClass> postComparison;
                switch (this.SortMode)
                {
                    case ComparerMode.Data:
                        postComparison = (x, y) => Comparer<string>.Default.Compare(x?.TextFromApi, y?.TextFromApi);
                        break;
                    case ComparerMode.Name:
                        postComparison = (x, y) => Comparer<string>.Default.Compare(x?.ScreenName, y?.ScreenName);
                        break;
                    case ComparerMode.Nickname:
                        postComparison = (x, y) => Comparer<string>.Default.Compare(x?.Nickname, y?.Nickname);
                        break;
                    case ComparerMode.Source:
                        postComparison = (x, y) => Comparer<string>.Default.Compare(x?.Source, y?.Source);
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }

                comparison = (x, y) =>
                {
                    PostClass xPost, yPost;
                    this.Posts.TryGetValue(x, out xPost);
                    this.Posts.TryGetValue(y, out yPost);

                    var compare = sign * postComparison(xPost, yPost);
                    if (compare != 0)
                        return compare;

                    // 同値であれば status_id で比較する
                    return sign * x.CompareTo(y);
                };
            }

            var comparer = Comparer<long>.Create(comparison);

            this._ids = new IndexedSortedSet<long>(this._ids, comparer);
            this.unreadIds = new SortedSet<long>(this.unreadIds, comparer);
        }

        [XmlIgnore]
        public ComparerMode SortMode { get; private set; }

        [XmlIgnore]
        public SortOrder SortOrder { get; private set; }

        public void AddPostQueue(long statusId, bool read)
        {
            this.addQueue.Enqueue(new TemporaryId(statusId, read));
        }

        //無条件に追加
        public void AddPostImmediately(long ID, bool Read)
        {
            if (this._ids.Contains(ID)) return;

            this._ids.Add(ID);

            if (!Read)
                this.unreadIds.Add(ID);
        }

        //フィルタに合致したら追加
        public MyCommon.HITRESULT AddFiltered(PostClass post, bool immediately = false)
        {
            if (this.IsInnerStorageTabType) return MyCommon.HITRESULT.None;

            var rslt = MyCommon.HITRESULT.None;
            //全フィルタ評価（優先順位あり）
            lock (this._lockObj)
            {
                foreach (var ft in _filters)
                {
                    try
                    {
                        switch (ft.ExecFilter(post))   //フィルタクラスでヒット判定
                        {
                            case MyCommon.HITRESULT.None:
                                break;
                            case MyCommon.HITRESULT.Copy:
                                if (rslt != MyCommon.HITRESULT.CopyAndMark) rslt = MyCommon.HITRESULT.Copy;
                                break;
                            case MyCommon.HITRESULT.CopyAndMark:
                                rslt = MyCommon.HITRESULT.CopyAndMark;
                                break;
                            case MyCommon.HITRESULT.Move:
                                rslt = MyCommon.HITRESULT.Move;
                                break;
                            case MyCommon.HITRESULT.Exclude:
                                rslt = MyCommon.HITRESULT.Exclude;
                                goto exit_for;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        // ExecFilterでNullRef出る場合あり。暫定対応
                        MyCommon.TraceOut("ExecFilterでNullRef: " + ft.ToString());
                        rslt = MyCommon.HITRESULT.None;
                    }
                }
            exit_for:
                ;
            }

            if (this.TabType != MyCommon.TabUsageType.Mute &&
                rslt != MyCommon.HITRESULT.None && rslt != MyCommon.HITRESULT.Exclude)
            {
                if (immediately)
                    this.AddPostImmediately(post.StatusId, post.IsRead);
                else
                    this.AddPostQueue(post.StatusId, post.IsRead);
            }

            return rslt; //マーク付けは呼び出し元で行うこと
        }

        //検索結果の追加
        public void AddPostToInnerStorage(PostClass Post)
        {
            if (_innerPosts.ContainsKey(Post.StatusId)) return;
            _innerPosts.TryAdd(Post.StatusId, Post);
            this.AddPostQueue(Post.StatusId, Post.IsRead);
        }

        public IList<long> AddSubmit(ref bool isMentionIncluded)
        {
            var addedIds = new List<long>();

            TemporaryId tId;
            while (this.addQueue.TryDequeue(out tId))
            {
                if (this.TabType == MyCommon.TabUsageType.Mentions && TabInformations.GetInstance()[tId.Id].IsReply) isMentionIncluded = true;
                this.AddPostImmediately(tId.Id, tId.Read);
                addedIds.Add(tId.Id);
            }

            return addedIds;
        }

        public void AddSubmit()
        {
            bool mention = false;
            AddSubmit(ref mention);
        }

        public void Remove(long Id)
        {
            if (!this._ids.Contains(Id))
                return;

            this._ids.Remove(Id);
            this.unreadIds.Remove(Id);

            if (this.IsInnerStorageTabType)
            {
                PostClass removedPost;
                this._innerPosts.TryRemove(Id, out removedPost);
            }
        }

        public bool UnreadManage { get; set; }

        // v1.0.5で「タブを固定(Locked)」から「タブを保護(Protected)」に名称変更
        [XmlElement(ElementName = "Locked")]
        public bool Protected { get; set; }

        public bool Notify { get; set; }

        public string SoundFile { get; set; }

        /// <summary>
        /// 次に表示する未読ツイートのIDを返します。
        /// ただし、未読がない場合または UnreadManage が false の場合は -1 を返します
        /// </summary>
        [XmlIgnore]
        public long NextUnreadId
        {
            get
            {
                if (!this.UnreadManage || !SettingCommon.Instance.UnreadManage)
                    return -1L;

                if (this.unreadIds.Count == 0)
                    return -1L;

                // unreadIds はリストのインデックス番号順に並んでいるため、
                // 例えば ID 順の整列であれば昇順なら上から、降順なら下から順に返せば過去→現在の順になる
                return this.SortOrder == SortOrder.Ascending ? this.unreadIds.Min : this.unreadIds.Max;
            }
        }

        /// <summary>
        /// 次に表示する未読ツイートのインデックス番号を返します。
        /// ただし、未読がない場合または UnreadManage が false の場合は -1 を返します
        /// </summary>
        [XmlIgnore]
        public int NextUnreadIndex
        {
            get
            {
                var unreadId = this.NextUnreadId;
                return unreadId != -1 ? this.IndexOf(unreadId) : -1;
            }
        }

        /// <summary>
        /// 未読ツイートの件数を返します。
        /// ただし、未読がない場合または UnreadManage が false の場合は 0 を返します
        /// </summary>
        [XmlIgnore]
        public int UnreadCount
        {
            get
            {
                if (!this.UnreadManage || !SettingCommon.Instance.UnreadManage)
                    return 0;

                return this.unreadIds.Count;
            }
        }

        public int AllCount
        {
            get
            {
                return this._ids.Count;
            }
        }

        /// <summary>
        /// 未読ツイートの ID を配列で返します
        /// </summary>
        public long[] GetUnreadIds()
        {
            lock (this._lockObj)
            {
                return this.unreadIds.ToArray();
            }
        }

        /// <summary>
        /// タブ内の既読状態を変更します
        /// </summary>
        /// <remarks>
        /// 全タブを横断して既読状態を変える TabInformation.SetReadAllTab() の内部で呼び出されるメソッドです
        /// </remarks>
        /// <param name="statusId">変更するツイートのID</param>
        /// <param name="read">既読状態</param>
        /// <returns>既読状態に変化があれば true、変化がなければ false</returns>
        internal bool SetReadState(long statusId, bool read)
        {
            if (!this._ids.Contains(statusId))
                throw new ArgumentException(nameof(statusId));

            if (this.IsInnerStorageTabType)
                this.Posts[statusId].IsRead = read;

            if (read)
                return this.unreadIds.Remove(statusId);
            else
                return this.unreadIds.Add(statusId);
        }

        public PostFilterRule[] GetFilters()
        {
            lock (this._lockObj)
            {
                return _filters.ToArray();
            }
        }

        public void RemoveFilter(PostFilterRule filter)
        {
            lock (this._lockObj)
            {
                _filters.Remove(filter);
                filter.PropertyChanged -= this.OnFilterModified;
                this.FilterModified = true;
            }
        }

        public bool AddFilter(PostFilterRule filter)
        {
            lock (this._lockObj)
            {
                if (_filters.Contains(filter)) return false;
                filter.PropertyChanged += this.OnFilterModified;
                _filters.Add(filter);
                this.FilterModified = true;
                return true;
            }
        }

        private void OnFilterModified(object sender, PropertyChangedEventArgs e)
        {
            this.FilterModified = true;
        }

        public PostFilterRule[] FilterArray
        {
            get
            {
                lock (this._lockObj)
                {
                    return _filters.ToArray();
                }
            }
            set
            {
                lock (this._lockObj)
                {
                    foreach (var oldFilter in this._filters)
                    {
                        oldFilter.PropertyChanged -= this.OnFilterModified;
                    }

                    this._filters.Clear();
                    this.FilterModified = true;

                    foreach (var newFilter in value)
                    {
                        _filters.Add(newFilter);
                        newFilter.PropertyChanged += this.OnFilterModified;
                    }
                }
            }
        }
        public bool Contains(long ID)
        {
            return _ids.Contains(ID);
        }

        public void ClearIDs()
        {
            _ids.Clear();
            this.unreadIds.Clear();
            _innerPosts.Clear();

            Interlocked.Exchange(ref this.addQueue, new ConcurrentQueue<TemporaryId>());
        }

        public PostClass this[int Index]
        {
            get
            {
                var id = GetId(Index);
                if (id < 0) throw new ArgumentException("Index can't find. Index=" + Index.ToString() + "/TabName=" + TabName, nameof(Index));
                return Posts[id];
            }
        }

        public PostClass[] this[int StartIndex, int EndIndex]
        {
            get
            {
                var length = EndIndex - StartIndex + 1;
                var posts = new PostClass[length];
                for (int i = 0; i < length; i++)
                {
                    posts[i] = Posts[GetId(StartIndex + i)];
                }
                return posts;
            }
        }

        public long[] GetId(ListView.SelectedIndexCollection IndexCollection)
        {
            if (IndexCollection.Count == 0) return null;

            var Ids = new long[IndexCollection.Count];
            for (int i = 0; i < Ids.Length; i++)
            {
                Ids[i] = GetId(IndexCollection[i]);
            }
            return Ids;
        }

        public long GetId(int Index)
        {
            return Index < _ids.Count ? _ids[Index] : -1;
        }

        public int[] IndexOf(long[] Ids)
        {
            if (Ids == null) return null;
            var idx = new int[Ids.Length];
            for (int i = 0; i < Ids.Length; i++)
            {
                idx[i] = IndexOf(Ids[i]);
            }
            return idx;
        }

        public int IndexOf(long ID)
        {
            return _ids.IndexOf(ID);
        }

        [XmlIgnore]
        public bool FilterModified { get; set; }

        public long[] BackupIds
        {
            get
            {
                return _ids.ToArray();
            }
        }

        public string TabName { get; set; }

        public MyCommon.TabUsageType TabType
        {
            get
            {
                return _tabType;
            }
            set
            {
                _tabType = value;
                if (this.IsInnerStorageTabType)
                {
                    Posts = _innerPosts;
                }
                else
                {
                    Posts = TabInformations.GetInstance().Posts;
                }
            }
        }

        public bool IsDefaultTabType
        {
            get
            {
                return _tabType.IsDefault();
            }
        }

        public bool IsDistributableTabType
        {
            get
            {
                return _tabType.IsDistributable();
            }
        }

        public bool IsInnerStorageTabType
        {
            get
            {
                return _tabType.IsInnerStorage();
            }
        }
    }

    /// <summary>
    /// enum TabUsageType に対応する拡張メソッドを定義したクラス
    /// </summary>
    public static class TabUsageTypeExt
    {
        const MyCommon.TabUsageType DefaultTabTypeMask =
            MyCommon.TabUsageType.Home |
            MyCommon.TabUsageType.Mentions |
            MyCommon.TabUsageType.DirectMessage |
            MyCommon.TabUsageType.Favorites |
            MyCommon.TabUsageType.Mute;

        const MyCommon.TabUsageType DistributableTabTypeMask =
            MyCommon.TabUsageType.Mentions |
            MyCommon.TabUsageType.UserDefined |
            MyCommon.TabUsageType.Mute;

        const MyCommon.TabUsageType InnerStorageTabTypeMask =
            MyCommon.TabUsageType.DirectMessage |
            MyCommon.TabUsageType.PublicSearch |
            MyCommon.TabUsageType.Lists |
            MyCommon.TabUsageType.UserTimeline |
            MyCommon.TabUsageType.Related |
            MyCommon.TabUsageType.SearchResults;

        /// <summary>
        /// デフォルトタブかどうかを示す値を取得します。
        /// </summary>
        public static bool IsDefault(this MyCommon.TabUsageType tabType)
        {
            return (tabType & DefaultTabTypeMask) != 0;
        }

        /// <summary>
        /// 振り分け可能タブかどうかを示す値を取得します。
        /// </summary>
        public static bool IsDistributable(this MyCommon.TabUsageType tabType)
        {
            return (tabType & DistributableTabTypeMask) != 0;
        }

        /// <summary>
        /// 内部ストレージを使用するタブかどうかを示す値を取得します。
        /// </summary>
        public static bool IsInnerStorage(this MyCommon.TabUsageType tabType)
        {
            return (tabType & InnerStorageTabTypeMask) != 0;
        }
    }

    /// <summary>
    /// 比較する方法
    /// </summary>
    public enum ComparerMode
    {
        Id,
        Data,
        Name,
        Nickname,
        Source,
    }
}
