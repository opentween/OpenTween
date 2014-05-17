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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace OpenTween
{
    public class PostClass : ICloneable
    {
        public class StatusGeo
        {
            public double Lng { get; set; }
            public double Lat { get; set; }

            public override bool Equals(object obj)
            {
                var geo = obj as StatusGeo;
                return geo != null && geo.Lng == this.Lng && geo.Lat == this.Lat;
            }

            public override int GetHashCode()
            {
                return this.Lng.GetHashCode() ^ this.Lat.GetHashCode();
            }
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
        public string SourceHtml { get; set; }
        public List<string> ReplyToList { get; set; }
        public bool IsMe { get; set; }
        public bool IsDm { get; set; }
        public long UserId { get; set; }
        public bool FilterHit { get; set; }
        public string RetweetedBy { get; set; }
        public long? RetweetedId { get; set; }
        private bool _IsDeleted = false;
        private StatusGeo _postGeo = new StatusGeo();
        public int RetweetedCount { get; set; }
        public long? RetweetedByUserId { get; set; }
        public long? InReplyToUserId { get; set; }
        public Dictionary<string, string> Media { get; set; }

        public string RelTabName { get; set; }
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

        public PostClass(string Nickname,
                string textFromApi,
                string text,
                string ImageUrl,
                string screenName,
                DateTime createdAt,
                long statusId,
                bool IsFav,
                bool IsRead,
                bool IsReply,
                bool IsExcludeReply,
                bool IsProtect,
                bool IsOwl,
                bool IsMark,
                string InReplyToUser,
                long? InReplyToStatusId,
                string Source,
                string SourceHtml,
                List<string> ReplyToList,
                bool IsMe,
                bool IsDm,
                long userId,
                bool FilterHit,
                string RetweetedBy,
                long? RetweetedId,
                StatusGeo Geo)
            : this()
        {
            this.Nickname = Nickname;
            this.TextFromApi = textFromApi;
            this.ImageUrl = ImageUrl;
            this.ScreenName = screenName;
            this.CreatedAt = createdAt;
            this.StatusId = statusId;
            _IsFav = IsFav;
            this.Text = text;
            this.IsRead = IsRead;
            this.IsReply = IsReply;
            this.IsExcludeReply = IsExcludeReply;
            _IsProtect = IsProtect;
            this.IsOwl = IsOwl;
            _IsMark = IsMark;
            this.InReplyToUser = InReplyToUser;
            _InReplyToStatusId = InReplyToStatusId;
            this.Source = Source;
            this.SourceHtml = SourceHtml;
            this.ReplyToList = ReplyToList;
            this.IsMe = IsMe;
            this.IsDm = IsDm;
            this.UserId = userId;
            this.FilterHit = FilterHit;
            this.RetweetedBy = RetweetedBy;
            this.RetweetedId = RetweetedId;
            _postGeo = Geo;
        }

        public PostClass()
        {
            RetweetedBy = "";
            RelTabName = "";
            Media = new Dictionary<string, string>();
            ReplyToList = new List<string>();
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
                    var post = this.GetRetweetSource(this.RetweetedId.Value);
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
                    var post = this.GetRetweetSource(this.RetweetedId.Value);
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

        public StatusGeo PostGeo
        {
            get
            {
                return _postGeo;
            }
            set
            {
                if (value != null && (value.Lat != 0 || value.Lng != 0))
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

        protected virtual PostClass GetRetweetSource(long statusId)
        {
            return TabInformations.GetInstance().RetweetSource(statusId);
        }

        [Obsolete("Use PostClass.Clone() instead.")]
        public PostClass Copy()
        {
            return this.Clone();
        }

        public PostClass Clone()
        {
            return (PostClass)((ICloneable)this).Clone();
        }

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
                    (this.SourceHtml == other.SourceHtml) &&
                    (this.ReplyToList.SequenceEqual(other.ReplyToList)) &&
                    (this.IsMe == other.IsMe) &&
                    (this.IsDm == other.IsDm) &&
                    (this.UserId == other.UserId) &&
                    (this.FilterHit == other.FilterHit) &&
                    (this.RetweetedBy == other.RetweetedBy) &&
                    (this.RetweetedId == other.RetweetedId) &&
                    (this.RelTabName == other.RelTabName) &&
                    (this.IsDeleted == other.IsDeleted) &&
                    (this.InReplyToUserId == other.InReplyToUserId);

        }

        public override int GetHashCode()
        {
            return this.StatusId.GetHashCode();
        }

#region "IClonable.Clone"
        object ICloneable.Clone()
        {
            var clone = (PostClass)this.MemberwiseClone();
            clone.ReplyToList = new List<string>(this.ReplyToList);
            clone.PostGeo = new StatusGeo { Lng = this.PostGeo.Lng, Lat = this.PostGeo.Lat };
            clone.Media = new Dictionary<string, string>(this.Media);

            return clone;
        }
#endregion
    }

    public sealed class TabInformations
    {
        //個別タブの情報をDictionaryで保持
        private IdComparerClass _sorter;
        private Dictionary<string, TabClass> _tabs = new Dictionary<string, TabClass>();
        private Dictionary<long, PostClass> _statuses = new Dictionary<long, PostClass>();
        private List<long> _addedIds;
        private List<long> _deletedIds = new List<long>();
        private Dictionary<long, PostClass> _retweets = new Dictionary<long, PostClass>();
        private Stack<TabClass> _removedTab = new Stack<TabClass>();
        private List<ScrubGeoInfo> _scrubGeo = new List<ScrubGeoInfo>();

        private class ScrubGeoInfo
        {
            public long UserId = 0;
            public long UpToStatusId = 0;
        }

        public List<long> BlockIds = new List<long>();
        public List<long> MuteUserIds = new List<long>();

        //発言の追加
        //AddPost(複数回) -> DistributePosts          -> SubmitUpdate

        //トランザクション用
        private int _addCount;
        private string _soundFile;
        private List<PostClass> _notifyPosts;
        private readonly object LockObj = new object();
        private readonly object LockUnread = new object();

        private static TabInformations _instance = new TabInformations();

        //List
        private List<ListElement> _lists = new List<ListElement>();

        private TabInformations()
        {
            _sorter = new IdComparerClass();
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
            tb.Sorter.Mode = _sorter.Mode;
            tb.Sorter.Order = _sorter.Order;
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
                if (IsDefaultTab(TabName)) return; //念のため
                var tb = _tabs[TabName];
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
                        if (!exist) homeTab.Add(Id, _statuses[Id].IsRead, false);
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

        public void SortPosts()
        {
            foreach (var tab in _tabs.Values)
            {
                tab.Sort();
            }
        }

        public SortOrder SortOrder
        {
            get
            {
                return _sorter.Order;
            }
            set
            {
                _sorter.Order = value;
                foreach (var tab in _tabs.Values)
                {
                    tab.Sorter.Order = value;
                }
            }
        }

        public IdComparerClass.ComparerMode SortMode
        {
            get
            {
                return _sorter.Mode;
            }
            set
            {
                _sorter.Mode = value;
                foreach (var tab in _tabs.Values)
                {
                    tab.Sorter.Mode = value;
                }
            }
        }

        public SortOrder ToggleSortOrder(IdComparerClass.ComparerMode SortMode)
        {
            if (_sorter.Mode == SortMode)
            {
                if (_sorter.Order == SortOrder.Ascending)
                {
                    _sorter.Order = SortOrder.Descending;
                }
                else
                {
                    _sorter.Order = SortOrder.Ascending;
                }
                foreach (var tab in _tabs.Values)
                {
                    tab.Sorter.Order = _sorter.Order;
                }
            }
            else
            {
                _sorter.Mode = SortMode;
                _sorter.Order = SortOrder.Ascending;
                foreach (var tab in _tabs.Values)
                {
                    tab.Sorter.Mode = SortMode;
                    tab.Sorter.Order = SortOrder.Ascending;
                }
            }
            this.SortPosts();
            return _sorter.Order;
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
                var tn = tab.TabName;

                if (_statuses.TryGetValue(Id, out post))
                {
                    //指定タブから該当ID削除
                    var tType = tab.TabType;
                    if (tab.Contains(Id))
                    {
                        if (tab.UnreadManage && !post.IsRead)    //未読管理
                        {
                            lock (LockUnread)
                            {
                                tab.UnreadCount--;
                                this.SetNextUnreadId(Id, tab);
                            }
                        }
                        tab.Remove(Id);
                    }
                    //FavタブからRetweet発言を削除する場合は、他の同一参照Retweetも削除
                    if (tType == MyCommon.TabUsageType.Favorites && post.RetweetedId != null)
                    {
                        for (int i = 0; i < tab.AllCount; i++)
                        {
                            PostClass rPost = null;
                            try
                            {
                                rPost = this[tn, i];
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                break;
                            }
                            if (rPost.RetweetedId != null && rPost.RetweetedId == post.RetweetedId)
                            {
                                if (tab.UnreadManage && !rPost.IsRead)    //未読管理
                                {
                                    lock (LockUnread)
                                    {
                                        tab.UnreadCount--;
                                        this.SetNextUnreadId(rPost.StatusId, tab);
                                    }
                                }
                                tab.Remove(rPost.StatusId);
                            }
                        }
                    }
                }
                //TabType=PublicSearchの場合（Postの保存先がTabClass内）
                //if (tab.Contains(StatusId) &&
                //   (tab.TabType = MyCommon.TabUsageType.PublicSearch || tab.TabType = MyCommon.TabUsageType.DirectMessage))
                //{
                //    post = tab.Posts[StatusId];
                //    if (tab.UnreadManage && !post.IsRead)    //未読管理
                //    {
                //        lock (LockUnread)
                //        {
                //            tab.UnreadCount--;
                //            this.SetNextUnreadId(StatusId, tab);
                //        }
                //    }
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
                    p.PostGeo = new PostClass.StatusGeo();
                }

                var userPosts2 = from tb in this.GetTabsInnerStorageType()
                                 from post in tb.Posts.Values
                                 where post.UserId == userId && post.UserId <= upToStatusId
                                 select post;

                foreach (var p in userPosts2)
                {
                    p.PostGeo = new PostClass.StatusGeo();
                }
            }
        }

        public void RemovePostReserve(long id)
        {
            lock (LockObj)
            {
                this._deletedIds.Add(id);
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
                    {
                        if (tab.UnreadManage && !tab.Posts[Id].IsRead)    //未読管理
                        {
                            lock (LockUnread)
                            {
                                tab.UnreadCount--;
                                this.SetNextUnreadId(Id, tab);
                            }
                        }
                        tab.Remove(Id);
                    }
                }
                if (_statuses.ContainsKey(Id)) _statuses.Remove(Id);
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

        public int GetOldestUnreadIndex(string TabName)
        {
            var tb = _tabs[TabName];
            if (tb.OldestUnreadId > -1 &&
                tb.Contains(tb.OldestUnreadId) &&
                tb.UnreadCount > 0)
            {
                //未読アイテムへ
                bool isRead = tb.Posts[tb.OldestUnreadId].IsRead;
                if (isRead)
                {
                    //状態不整合（最古未読ＩＤが実は既読）
                    lock (LockUnread)
                    {
                        this.SetNextUnreadId(-1, tb);  //頭から探索
                    }
                    if (tb.OldestUnreadId == -1)
                    {
                        return -1;
                    }
                    else
                    {
                        return tb.IndexOf(tb.OldestUnreadId);
                    }
                }
                else
                {
                    return tb.IndexOf(tb.OldestUnreadId);    //最短経路;
                }
            }
            else
            {
                //一見未読なさそうだが、未読カウントはあるので探索
                //if (tb.UnreadCount > 0)
                if (!(tb.UnreadManage && AppendSettingDialog.Instance.UnreadManage)) return -1;
                lock (LockUnread)
                {
                    this.SetNextUnreadId(-1, tb);
                }
                if (tb.OldestUnreadId == -1)
                {
                    return -1;
                }
                else
                {
                    return tb.IndexOf(tb.OldestUnreadId);
                }
                //else
                //{
                //    return -1;
                //}
            }
        }

        private void SetNextUnreadId(long CurrentId, TabClass Tab)
        {
            //CurrentID:今既読にしたID(OldestIDの可能性あり)
            //最古未読が設定されていて、既読の場合（1発言以上存在）
            try
            {
                Dictionary<long, PostClass> posts = Tab.Posts;

                PostClass oldestUnreadPost;
                if (Tab.OldestUnreadId > -1 &&
                    posts.TryGetValue(Tab.OldestUnreadId, out oldestUnreadPost) &&
                    oldestUnreadPost.IsRead &&
                    _sorter.Mode == IdComparerClass.ComparerMode.Id)     //次の未読探索
                {
                    if (Tab.UnreadCount == 0)
                    {
                        //未読数０→最古未読なし
                        Tab.OldestUnreadId = -1;
                    }
                    else if (Tab.OldestUnreadId == CurrentId && CurrentId > -1)
                    {
                        //最古IDを既読にしたタイミング→次のIDから続けて探索
                        var idx = Tab.IndexOf(CurrentId);
                        if (idx > -1)
                        {
                            //続きから探索
                            FindUnreadId(idx, Tab);
                            return;
                        }
                    }
                }
            }
            catch (KeyNotFoundException)
            {
            }

            //頭から探索
            FindUnreadId(-1, Tab);
        }

        private void FindUnreadId(int StartIdx, TabClass Tab)
        {
            if (Tab.AllCount == 0)
            {
                Tab.OldestUnreadId = -1;
                Tab.UnreadCount = 0;
                return;
            }
            var toIdx = 0;
            var stp = 1;
            Tab.OldestUnreadId = -1;
            if (_sorter.Order == SortOrder.Ascending)
            {
                if (StartIdx == -1)
                {
                    StartIdx = 0;
                }
                else
                {
                    //StartIdx++;
                    if (StartIdx > Tab.AllCount - 1) StartIdx = Tab.AllCount - 1; //念のため
                }
                toIdx = Tab.AllCount - 1;
                if (toIdx < 0) toIdx = 0; //念のため
                stp = 1;
            }
            else
            {
                if (StartIdx == -1)
                {
                    StartIdx = Tab.AllCount - 1;
                }
                else
                {
                    //StartIdx--;
                }
                if (StartIdx < 0) StartIdx = 0; //念のため
                toIdx = 0;
                stp = -1;
            }

            Dictionary<long, PostClass> posts = Tab.Posts;

            for (int i = StartIdx; ; i+= stp)
            {
                var id = Tab.GetId(i);
                if (id > -1 && !posts[id].IsRead)
                {
                    Tab.OldestUnreadId = id;
                    break;
                }

                if (i == toIdx) break;
            }
        }

        public int DistributePosts()
        {
            lock (LockObj)
            {
                //戻り値は追加件数
                //if (_addedIds == null) return 0;
                //if (_addedIds.Count == 0) return 0;

                if (_addedIds == null) _addedIds = new List<long>();
                if (_notifyPosts == null) _notifyPosts = new List<PostClass>();
                try
                {
                    this.Distribute();    //タブに仮振分
                }
                catch (KeyNotFoundException)
                {
                    //タブ変更により振分が失敗した場合
                }
                var retCnt = _addedIds.Count;
                _addCount += retCnt;
                _addedIds.Clear();
                _addedIds = null;     //後始末
                return retCnt;     //件数
            }
        }

        public int SubmitUpdate(ref string soundFile,
                                ref PostClass[] notifyPosts,
                                ref bool isMentionIncluded,
                                ref bool isDeletePost,
                                bool isUserStream)
        {
            //注：メインスレッドから呼ぶこと
            lock (LockObj)
            {
                if (_notifyPosts == null)
                {
                    soundFile = "";
                    notifyPosts = null;
                    return 0;
                }

                foreach (var tb in _tabs.Values)
                {
                    if (tb.IsInnerStorageTabType)
                    {
                        _addCount += tb.GetTemporaryCount();
                    }
                    tb.AddSubmit(ref isMentionIncluded);  //振分確定（各タブに反映）
                }
                ////UserStreamで反映間隔10秒以下だったら、30秒ごとにソートする
                ////10秒以上だったら毎回ソート
                //static DateTime lastSort = DateTime.Now;
                //if (AppendSettingDialog.Instance.UserstreamPeriodInt < 10 && isUserStream)
                //{
                //    if (Now.Subtract(lastSort) > TimeSpan.FromSeconds(30))
                //    {
                //        lastSort = DateTime.Now;
                //        isUserStream = false;
                //    }
                //}
                //else
                //{
                //    isUserStream = false;
                //}
                if (!isUserStream || this.SortMode != IdComparerClass.ComparerMode.Id)
                {
                    this.SortPosts();
                }
                if (isUserStream)
                {
                    isDeletePost = this._deletedIds.Count > 0;
                    foreach (var id in this._deletedIds)
                    {
                        //this.DeletePost(StatusId)
                        this.RemovePost(id);
                    }
                    this._deletedIds.Clear();
                }

                soundFile = _soundFile;
                _soundFile = "";
                notifyPosts = _notifyPosts.ToArray();
                _notifyPosts.Clear();
                _notifyPosts = null;
                var retCnt = _addCount;
                _addCount = 0;
                return retCnt;    //件数（EndUpdateの戻り値と同じ）
            }
        }

        private void Distribute()
        {
            //各タブのフィルターと照合。合致したらタブにID追加
            //通知メッセージ用に、表示必要な発言リストと再生サウンドを返す
            //notifyPosts = new List<PostClass>();
            var homeTab = GetTabByType(MyCommon.TabUsageType.Home);
            var replyTab = GetTabByType(MyCommon.TabUsageType.Mentions);
            var dmTab = GetTabByType(MyCommon.TabUsageType.DirectMessage);
            var favTab = GetTabByType(MyCommon.TabUsageType.Favorites);
            foreach (var id in _addedIds)
            {
                var post = _statuses[id];
                var add = false;  //通知リスト追加フラグ
                var mv = false;   //移動フラグ（Recent追加有無）
                var rslt = MyCommon.HITRESULT.None;
                post.IsExcludeReply = false;
                foreach (var tab in _tabs.Values)
                {
                    rslt = tab.AddFiltered(post);
                    if (rslt != MyCommon.HITRESULT.None && rslt != MyCommon.HITRESULT.Exclude)
                    {
                        if (rslt == MyCommon.HITRESULT.CopyAndMark) post.IsMark = true; //マークあり
                        else if (rslt == MyCommon.HITRESULT.Move)
                        {
                            mv = true; //移動
                            post.IsMark = false;
                        }
                        if (tab.Notify) add = true; //通知あり
                        if (!string.IsNullOrEmpty(tab.SoundFile) && string.IsNullOrEmpty(_soundFile))
                        {
                            _soundFile = tab.SoundFile; //wavファイル（未設定の場合のみ）
                        }
                        post.FilterHit = true;
                    }
                    else
                    {
                        if (rslt == MyCommon.HITRESULT.Exclude && tab.TabType == MyCommon.TabUsageType.Mentions)
                        {
                            post.IsExcludeReply = true;
                        }
                        post.FilterHit = false;
                    }
                }
                if (!mv)  //移動されなかったらRecentに追加
                {
                    homeTab.Add(post.StatusId, post.IsRead, true);
                    if (!string.IsNullOrEmpty(homeTab.SoundFile) && string.IsNullOrEmpty(_soundFile)) _soundFile = homeTab.SoundFile;
                    if (homeTab.Notify) add = true;
                }
                if (post.IsReply && !post.IsExcludeReply)    //除外ルール適用のないReplyならReplyタブに追加
                {
                    replyTab.Add(post.StatusId, post.IsRead, true);
                    if (!string.IsNullOrEmpty(replyTab.SoundFile)) _soundFile = replyTab.SoundFile;
                    if (replyTab.Notify) add = true;
                }
                if (post.IsFav)    //Fav済み発言だったらFavoritesタブに追加
                {
                    if (favTab.Contains(post.StatusId))
                    {
                        //取得済みなら非通知
                        //_soundFile = "";
                        add = false;
                    }
                    else
                    {
                        favTab.Add(post.StatusId, post.IsRead, true);
                        if (!string.IsNullOrEmpty(favTab.SoundFile) && string.IsNullOrEmpty(_soundFile)) _soundFile = favTab.SoundFile;
                        if (favTab.Notify) add = true;
                    }
                }
                if (add) _notifyPosts.Add(post);
            }
            foreach (var tb in _tabs.Values)
            {
                if (tb.IsInnerStorageTabType)
                {
                    if (tb.Notify)
                    {
                        if (tb.GetTemporaryCount() > 0)
                        {
                            foreach (var post in tb.GetTemporaryPosts())
                            {
                                var exist = false;
                                foreach (var npost in _notifyPosts)
                                {
                                    if (npost.StatusId == post.StatusId)
                                    {
                                        exist = true;
                                        break;
                                    }
                                }
                                if (!exist) _notifyPosts.Add(post);
                            }
                            if (!string.IsNullOrEmpty(tb.SoundFile))
                            {
                                if (tb.TabType == MyCommon.TabUsageType.DirectMessage || string.IsNullOrEmpty(_soundFile))
                                {
                                    _soundFile = tb.SoundFile;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddPost(PostClass Item)
        {
            lock (LockObj)
            {
                if (string.IsNullOrEmpty(Item.RelTabName))
                {
                    if (!Item.IsDm)
                    {
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
                            if (AppendSettingDialog.Instance.HideDuplicatedRetweets &&
                                !Item.IsMe &&
                                Item.RetweetedId != null &&
                                this._retweets.TryGetValue(Item.RetweetedId.Value, out status) &&
                                status.RetweetedCount > 0) return;

                            if (BlockIds.Contains(Item.UserId))
                                return;

                            if (MuteUserIds.Contains(Item.UserId) && !Item.IsReply)
                                return;

                            _statuses.Add(Item.StatusId, Item);
                        }
                        if (Item.RetweetedId != null)
                        {
                            this.AddRetweet(Item);
                        }
                        if (Item.IsFav && _retweets.ContainsKey(Item.StatusId))
                        {
                            return;    //Fav済みのRetweet元発言は追加しない
                        }
                        if (_addedIds == null) _addedIds = new List<long>(); //タブ追加用IDコレクション準備
                        _addedIds.Add(Item.StatusId);
                    }
                    else
                    {
                        //DM
                        var tb = this.GetTabByType(MyCommon.TabUsageType.DirectMessage);
                        if (tb.Contains(Item.StatusId)) return;
                        tb.AddPostToInnerStorage(Item);
                    }
                }
                else
                {
                    //公式検索、リスト、関連発言の場合
                    TabClass tb;
                    this.Tabs.TryGetValue(Item.RelTabName, out tb);
                    if (tb == null) return;
                    if (tb.Contains(Item.StatusId)) return;
                    //tb.Add(Item.StatusId, Item.IsRead, true);
                    tb.AddPostToInnerStorage(Item);
                }
            }
        }

        private void AddRetweet(PostClass item)
        {
            var retweetedId = item.RetweetedId.Value;

            //true:追加、False:保持済み
            PostClass status;
            if (_retweets.TryGetValue(retweetedId, out status))
            {
                status.RetweetedCount++;
                if (status.RetweetedCount > 10)
                {
                    status.RetweetedCount = 0;
                }
                return;
            }

            _retweets.Add(
                        item.RetweetedId.Value,
                        new PostClass(
                            item.Nickname,
                            item.TextFromApi,
                            item.Text,
                            item.ImageUrl,
                            item.ScreenName,
                            item.CreatedAt,
                            item.RetweetedId.Value,
                            item.IsFav,
                            item.IsRead,
                            item.IsReply,
                            item.IsExcludeReply,
                            item.IsProtect,
                            item.IsOwl,
                            item.IsMark,
                            item.InReplyToUser,
                            item.InReplyToStatusId,
                            item.Source,
                            item.SourceHtml,
                            item.ReplyToList,
                            item.IsMe,
                            item.IsDm,
                            item.UserId,
                            item.FilterHit,
                            "",
                            null,
                            item.PostGeo
                        )
                    );
            _retweets[retweetedId].RetweetedCount++;
        }

        public void SetReadAllTab(bool Read, string TabName, int Index)
        {
            //Read:true=既読へ　false=未読へ
            var tb = _tabs[TabName];

            if (tb.UnreadManage == false) return; //未読管理していなければ終了

            var Id = tb.GetId(Index);
            if (Id < 0) return;
            PostClass post = tb.Posts[Id];

            if (post.IsRead == Read) return; //状態変更なければ終了

            post.IsRead = Read;

            lock (LockUnread)
            {
                if (Read)
                {
                    tb.UnreadCount--;
                    this.SetNextUnreadId(Id, tb);  //次の未読セット
                    //他タブの最古未読ＩＤはタブ切り替え時に。
                    if (tb.IsInnerStorageTabType)
                    {
                        //一般タブ
                        PostClass status;
                        if (_statuses.TryGetValue(Id, out status) && !status.IsRead)
                        {
                            foreach (var tab in _tabs.Values)
                            {
                                if (tab.UnreadManage &&
                                    !tab.IsInnerStorageTabType &&
                                    tab.Contains(Id))
                                {
                                    tab.UnreadCount--;
                                    if (tab.OldestUnreadId == Id) tab.OldestUnreadId = -1;
                                }
                            }
                            status.IsRead = true;
                        }
                    }
                    else
                    {
                        //一般タブ
                        foreach (var tab in _tabs.Values)
                        {
                            if (tab != tb &&
                                tab.UnreadManage &&
                                !tab.IsInnerStorageTabType &&
                                tab.Contains(Id))
                            {
                                tab.UnreadCount--;
                                if (tab.OldestUnreadId == Id) tab.OldestUnreadId = -1;
                            }
                        }
                    }
                    //内部保存タブ
                    foreach (var tab in _tabs.Values)
                    {
                        if (tab != tb &&
                            tab.IsInnerStorageTabType &&
                            tab.Contains(Id))
                        {
                            var tPost = tab.Posts[Id];
                            if (!tPost.IsRead)
                            {
                                if (tab.UnreadManage)
                                {
                                    tab.UnreadCount--;
                                    if (tab.OldestUnreadId == Id) tab.OldestUnreadId = -1;
                                }
                                tPost.IsRead = true;
                            }
                        }
                    }
                }
                else
                {
                    tb.UnreadCount++;
                    //if (tb.OldestUnreadId > Id || tb.OldestUnreadId = -1) tb.OldestUnreadId = Id
                    if (tb.OldestUnreadId > Id) tb.OldestUnreadId = Id;
                    if (tb.IsInnerStorageTabType)
                    {
                        //一般タブ
                        PostClass status;
                        if (_statuses.TryGetValue(Id, out status) && status.IsRead)
                        {
                            foreach (var tab in _tabs.Values)
                            {
                                if (tab.UnreadManage &&
                                    !tab.IsInnerStorageTabType &&
                                    tab.Contains(Id))
                                {
                                    tab.UnreadCount++;
                                    if (tab.OldestUnreadId > Id) tab.OldestUnreadId = Id;
                                }
                            }
                            status.IsRead = false;
                        }
                    }
                    else
                    {
                        //一般タブ
                        foreach (var tab in _tabs.Values)
                        {
                            if (tab != tb &&
                                tab.UnreadManage &&
                                !tab.IsInnerStorageTabType &&
                                tab.Contains(Id))
                            {
                                tab.UnreadCount++;
                                if (tab.OldestUnreadId > Id) tab.OldestUnreadId = Id;
                            }
                        }
                    }
                    //内部保存タブ
                    foreach (var tab in _tabs.Values)
                    {
                        if (tab != tb &&
                            tab.IsInnerStorageTabType &&
                            tab.Contains(Id))
                        {
                            var tPost = tab.Posts[Id];
                            if (tPost.IsRead)
                            {
                                if (tab.UnreadManage)
                                {
                                    tab.UnreadCount++;
                                    if (tab.OldestUnreadId > Id) tab.OldestUnreadId = Id;
                                }
                                tPost.IsRead = false;
                            }
                        }
                    }
                }
            }
        }

        // TODO: パフォーマンスを勘案して、戻すか決める
        public void SetRead(bool Read, string TabName, int Index)
        {
            //Read:true=既読へ　false=未読へ
            var tb = _tabs[TabName];

            if (tb.UnreadManage == false) return; //未読管理していなければ終了

            var Id = tb.GetId(Index);
            if (Id < 0) return;
            PostClass post = tb.Posts[Id];

            if (post.IsRead == Read) return; //状態変更なければ終了

            post.IsRead = Read; //指定の状態に変更

            lock (LockUnread)
            {
                if (Read)
                {
                    tb.UnreadCount--;
                    this.SetNextUnreadId(Id, tb);  //次の未読セット
                    //他タブの最古未読ＩＤはタブ切り替え時に。
                    if (tb.IsInnerStorageTabType) return;
                    foreach (var tab in _tabs.Values)
                    {
                        if (tab != tb &&
                            tab.UnreadManage &&
                            !tab.IsInnerStorageTabType &&
                            tab.Contains(Id))
                        {
                            tab.UnreadCount--;
                            if (tab.OldestUnreadId == Id) tab.OldestUnreadId = -1;
                        }
                    }
                }
                else
                {
                    tb.UnreadCount++;
                    //if (tb.OldestUnreadId > Id || tb.OldestUnreadId == -1) tb.OldestUnreadId = Id;
                    if (tb.OldestUnreadId > Id) tb.OldestUnreadId = Id;
                    if (tb.IsInnerStorageTabType) return;
                    foreach (var tab in _tabs.Values)
                    {
                        if (tab != tb &&
                            tab.UnreadManage &&
                            !tab.IsInnerStorageTabType &&
                            tab.Contains(Id))
                        {
                            tab.UnreadCount++;
                            if (tab.OldestUnreadId > Id) tab.OldestUnreadId = Id;
                        }
                    }
                }
            }
        }

        public void SetRead()
        {
            var tb = GetTabByType(MyCommon.TabUsageType.Home);
            if (tb.UnreadManage == false) return;

            lock (LockObj)
            {
                for (int i = 0; i < tb.AllCount - 1; i++)
                {
                    var id = tb.GetId(i);
                    if (id < 0) return;
                    var tPost = _statuses[id];
                    if (!tPost.IsReply &&
                        !tPost.IsRead &&
                        !tPost.FilterHit)
                    {
                        tPost.IsRead = true;
                        this.SetNextUnreadId(id, tb);  //次の未読セット
                        foreach (var tab in _tabs.Values)
                        {
                            if (tab.UnreadManage &&
                                tab.Contains(id))
                            {
                                tab.UnreadCount--;
                                if (tab.OldestUnreadId == id) tab.OldestUnreadId = -1;
                            }
                        }
                    }
                }
            }
        }

        public PostClass this[long ID]
        {
            get
            {
                PostClass status;
                return _statuses.TryGetValue(ID, out status)
                    ? status
                    : this.GetTabsInnerStorageType()
                          .Where(t => t.Contains(ID))
                          .Select(t => t.Posts[ID]).FirstOrDefault();
            }
        }

        public PostClass this[string TabName, int Index]
        {
            get
            {
                TabClass tb;
                if (!_tabs.TryGetValue(TabName, out tb)) throw new ArgumentException("TabName=" + TabName + " is not contained.");
                var id = tb.GetId(Index);
                if (id < 0) throw new ArgumentException("Index can't find. Index=" + Index.ToString() + "/TabName=" + TabName);
                try
                {
                    return tb.Posts[tb.GetId(Index)];
                }
                catch (Exception ex)
                {
                    throw new Exception("Index=" + Index.ToString() + "/TabName=" + TabName, ex);
                }
            }
        }

        public PostClass[] this[string TabName, int StartIndex, int EndIndex]
        {
            get
            {
                TabClass tb;
                if (!_tabs.TryGetValue(TabName, out tb)) throw new ArgumentException("TabName=" + TabName + " is not contained.");
                var length = EndIndex - StartIndex + 1;
                var posts = new PostClass[length];
                for (int i = 0; i < length; i++)
                {
                    posts[i] = tb.Posts[tb.GetId(StartIndex + i)];
                }
                return posts;
            }
        }

        //public ReadOnly int ItemCount
        //{
        //    get
        //    {
        //        lock (LockObj)
        //    {
        //            return _statuses.Count   //DM,公式検索は除く
        //        }
        //    }
        //}

        public bool ContainsKey(long Id)
        {
            //DM,公式検索は非対応
            lock (LockObj)
            {
                return _statuses.ContainsKey(Id);
            }
        }

        public bool ContainsKey(long Id, string TabName)
        {
            //DM,公式検索は対応版
            lock (LockObj)
            {
                TabClass tab;
                return _tabs.TryGetValue(TabName, out tab) && tab.Contains(Id);
            }
        }

        public void SetUnreadManage(bool Manage)
        {
            if (Manage)
            {
                foreach (var tab in _tabs.Values)
                {
                    if (tab.UnreadManage)
                    {
                        lock (LockUnread)
                        {
                            var cnt = 0;
                            var oldest = long.MaxValue;
                            Dictionary<long, PostClass> posts = tab.Posts;
                            foreach (var id in tab.BackupIds)
                            {
                                if (!posts[id].IsRead)
                                {
                                    cnt++;
                                    if (oldest > id) oldest = id;
                                }
                            }
                            if (oldest == long.MaxValue) oldest = -1;
                            tab.OldestUnreadId = oldest;
                            tab.UnreadCount = cnt;
                        }
                    }
                }
            }
            else
            {
                foreach (var tab in _tabs.Values)
                {
                    if (tab.UnreadManage && tab.UnreadCount > 0)
                    {
                        lock (LockUnread)
                        {
                            tab.UnreadCount = 0;
                            tab.OldestUnreadId = -1;
                        }
                    }
                }
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
                var tbr = GetTabByType(MyCommon.TabUsageType.Home);
                var replyTab = GetTabByType(MyCommon.TabUsageType.Mentions);
                foreach (var tb in _tabs.Values.ToArray())
                {
                    if (tb.FilterModified)
                    {
                        tb.FilterModified = false;
                        var orgIds = tb.BackupIds;
                        tb.ClearIDs();
                        //////////////フィルター前のIDsを退避。どのタブにも含まれないidはrecentへ追加
                        //////////////moveフィルターにヒットした際、recentに該当あればrecentから削除
                        foreach (var post in _statuses.Values)
                        {
                            if (post.IsDm) continue;
                            var rslt = MyCommon.HITRESULT.None;
                            rslt = tb.AddFiltered(post);
                            switch (rslt)
                            {
                                case MyCommon.HITRESULT.CopyAndMark:
                                post.IsMark = true; //マークあり
                                post.FilterHit = true;
                                break;
                                case MyCommon.HITRESULT.Move:
                                tbr.Remove(post.StatusId, post.IsRead);
                                post.IsMark = false;
                                post.FilterHit = true;
                                break;
                            case MyCommon.HITRESULT.Copy:
                                post.IsMark = false;
                                post.FilterHit = true;
                                break;
                            case MyCommon.HITRESULT.Exclude:
                                if (tb.TabName == replyTab.TabName && post.IsReply) post.IsExcludeReply = true;
                                if (post.IsFav) GetTabByType(MyCommon.TabUsageType.Favorites).Add(post.StatusId, post.IsRead, true);
                                post.FilterHit = false;
                                break;
                            case MyCommon.HITRESULT.None:
                                if (tb.TabName == replyTab.TabName && post.IsReply) replyTab.Add(post.StatusId, post.IsRead, true);
                                if (post.IsFav) GetTabByType(MyCommon.TabUsageType.Favorites).Add(post.StatusId, post.IsRead, true);
                                post.FilterHit = false;
                                break;
                            }
                        }
                        tb.AddSubmit();  //振分確定
                        foreach (var id in orgIds)
                        {
                            var hit = false;
                            foreach (var tbTemp in _tabs.Values.ToArray())
                            {
                                if (tbTemp.Contains(id))
                                {
                                    hit = true;
                                    break;
                                }
                            }
                            if (!hit) tbr.Add(id, _statuses[id].IsRead, false);
                        }
                    }
                }
                this.SortPosts();
            }
        }

        public long[] GetId(string TabName, ListView.SelectedIndexCollection IndexCollection)
        {
            if (IndexCollection.Count == 0) return null;

            var tb = _tabs[TabName];
            var Ids = new long[IndexCollection.Count];
            for (int i = 0; i < Ids.Length; i++)
            {
                Ids[i] = tb.GetId(IndexCollection[i]);
            }
            return Ids;
        }

        public long GetId(string TabName, int Index)
        {
            return _tabs[TabName].GetId(Index);
        }

        public int[] IndexOf(string TabName, long[] Ids)
        {
            if (Ids == null) return null;
            var idx = new int[Ids.Length];
            var tb = _tabs[TabName];
            for (int i = 0; i < Ids.Length; i++)
            {
                idx[i] = tb.IndexOf(Ids[i]);
            }
            return idx;
        }

        public int IndexOf(string TabName, long Id)
        {
            return _tabs[TabName].IndexOf(Id);
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
                        if (!Hit) _statuses.Remove(Id);
                    }
                }

                //指定タブをクリア
                tb.ClearIDs();
            }
        }

        public void SetTabUnreadManage(string TabName, bool Manage)
        {
            var tb = _tabs[TabName];
            lock (LockUnread)
            {
                if (Manage)
                {
                    var cnt = 0;
                    var oldest = long.MaxValue;
                    Dictionary<long, PostClass> posts = tb.Posts;
                    foreach (var id in tb.BackupIds)
                    {
                        if (!posts[id].IsRead)
                        {
                            cnt++;
                            if (oldest > id) oldest = id;
                        }
                    }
                    if (oldest == long.MaxValue) oldest = -1;
                    tb.OldestUnreadId = oldest;
                    tb.UnreadCount = cnt;
                }
                else
                {
                    tb.OldestUnreadId = -1;
                    tb.UnreadCount = 0;
                }
            }
            tb.UnreadManage = Manage;
        }

        public void RefreshOwl(List<long> follower)
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
                foreach (var tab in _tabs.Values)
                {
                    if (tab.TabType == tabType) return tab;
                }
                return null;
            }
        }

        public List<TabClass> GetTabsByType(MyCommon.TabUsageType tabType)
        {
            //合致したタブをListで返す
            //合致しなければ空のListを返す
            lock (LockObj)
            {
                var tbs = new List<TabClass>();
                foreach (var tb in _tabs.Values)
                {
                    if ((tabType & tb.TabType) == tb.TabType) tbs.Add(tb);
                }
                return tbs;
            }
        }

        public List<TabClass> GetTabsInnerStorageType()
        {
            //合致したタブをListで返す
            //合致しなければ空のListを返す
            lock (LockObj)
            {
                var tbs = new List<TabClass>();
                foreach (var tb in _tabs.Values)
                {
                    if (tb.IsInnerStorageTabType) tbs.Add(tb);
                }
                return tbs;
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

        // デフォルトタブの判定処理
        public bool IsDefaultTab(string tabName)
        {
            TabClass tab;
            if (tabName != null &&
               _tabs.TryGetValue(tabName, out tab) &&
               (tab.TabType == MyCommon.TabUsageType.Home ||
               tab.TabType == MyCommon.TabUsageType.Mentions ||
               tab.TabType == MyCommon.TabUsageType.DirectMessage ||
               tab.TabType == MyCommon.TabUsageType.Favorites))
            {
                return true;
            }

            return false;
        }

        //振り分け可能タブの判定処理
        public bool IsDistributableTab(string tabName)
        {
            TabClass tab;
            if (tabName != null &&
                _tabs.TryGetValue(tabName, out tab) &&
                (tab.TabType == MyCommon.TabUsageType.Mentions ||
                tab.TabType == MyCommon.TabUsageType.UserDefined))
            {
                return true;
            }

            return false;
        }

        public string GetUniqueTabName()
        {
            var tabNameTemp = "MyTab" + (_tabs.Count + 1).ToString();
            for (int i = 2; i <= 100; i++)
            {
                if (_tabs.ContainsKey(tabNameTemp))
                {
                    tabNameTemp = "MyTab" + (_tabs.Count + i).ToString();
                }
                else
                {
                    break;
                }
            }
            return tabNameTemp;
        }

        public Dictionary<long, PostClass> Posts
        {
            get
            {
                return _statuses;
            }
        }
    }

    [Serializable]
    public sealed class TabClass
    {
        private bool _unreadManage = false;
        private List<PostFilterRule> _filters;
        private int _unreadCount = 0;
        private List<long> _ids;
        private List<TemporaryId> _tmpIds = new List<TemporaryId>();
        private MyCommon.TabUsageType _tabType = MyCommon.TabUsageType.Undefined;

        [NonSerialized]
        private IdComparerClass _sorter = new IdComparerClass();

        private readonly object _lockObj = new object();

        public string User { get; set; }

#region "検索"
        //Search query
        private string _searchLang = "";
        private string _searchWords = "";
        private string _nextPageQuery = "";

        public string SearchLang
        {
            get
            {
                return _searchLang;
            }
            set
            {
                SinceId = 0;
                _searchLang = value;
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
                SinceId = 0;
                _searchWords = value.Trim();
            }
        }

        public string NextPageQuery
        {
            get
            {
                return _nextPageQuery;
            }
            set
            {
                _nextPageQuery = value;
            }
        }

        public int GetSearchPage(int count)
        {
            return ((_ids.Count / count) + 1);
        }
        private Dictionary<string, string> _beforeQuery = new Dictionary<string, string>();
        public void SaveQuery(bool more)
        {
            var qry = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(_searchWords))
            {
                _beforeQuery = qry;
                return;
            }
            qry.Add("q", _searchWords);
            if (!string.IsNullOrEmpty(_searchLang)) qry.Add("lang", _searchLang);
            _beforeQuery = qry;
        }

        public bool IsQueryChanged()
        {
            var qry = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(_searchWords))
            {
                qry.Add("q", _searchWords);
                if (!string.IsNullOrEmpty(_searchLang)) qry.Add("lang", _searchLang);
            }
            if (qry.Count != _beforeQuery.Count) return true;

            foreach (var kvp in qry)
            {
                if (!_beforeQuery.ContainsKey(kvp.Key) || _beforeQuery[kvp.Key] != kvp.Value)
                {
                    return true;
                }
            }
            return false;
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
        public Dictionary<long, PostClass> Posts { get; private set; }

        private Dictionary<long, PostClass> _innerPosts;

        public PostClass[] GetTemporaryPosts()
        {
            var tempPosts = new List<PostClass>();
            if (_tmpIds.Count == 0) return tempPosts.ToArray();
            foreach (var tempId in _tmpIds)
            {
                tempPosts.Add(Posts[tempId.Id]);
            }
            return tempPosts.ToArray();
        }

        public int GetTemporaryCount()
        {
            return _tmpIds.Count;
        }

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
            _innerPosts = new Dictionary<long, PostClass>();
            Posts = _innerPosts;
            SoundFile = "";
            OldestUnreadId = -1;
            TabName = "";
            _filters = new List<PostFilterRule>();
            Protected = false;
            Notify = true;
            SoundFile = "";
            _unreadManage = true;
            _ids = new List<long>();
            this.OldestUnreadId = -1;
            _tabType = MyCommon.TabUsageType.Undefined;
            _listInfo = null;
        }

        public TabClass(string TabName, MyCommon.TabUsageType TabType, ListElement list) : this()
        {
            this.TabName = TabName;
            this.TabType = TabType;
            this.ListInfo = list;
        }

        public void Sort()
        {
            if (_sorter.Mode == IdComparerClass.ComparerMode.Id)
            {
                _ids.Sort(_sorter.CmpMethod());
                return;
            }
            long[] ar = null;
            if (_sorter.Order == SortOrder.Ascending)
            {
                switch (_sorter.Mode)
                {
                    case IdComparerClass.ComparerMode.Data:
                        ar = _ids.OrderBy(n => _sorter.posts[n].TextFromApi).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        ar = _ids.OrderBy(n => _sorter.posts[n].ScreenName).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Nickname:
                        ar = _ids.OrderBy(n => _sorter.posts[n].Nickname).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        ar = _ids.OrderBy(n => _sorter.posts[n].Source).ToArray();
                        break;
                }
            }
            else
            {
                switch (_sorter.Mode)
                {
                    case IdComparerClass.ComparerMode.Data:
                        ar = _ids.OrderByDescending(n => _sorter.posts[n].TextFromApi).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Name:
                        ar = _ids.OrderByDescending(n => _sorter.posts[n].ScreenName).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Nickname:
                        ar = _ids.OrderByDescending(n => _sorter.posts[n].Nickname).ToArray();
                        break;
                    case IdComparerClass.ComparerMode.Source:
                        ar = _ids.OrderByDescending(n => _sorter.posts[n].Source).ToArray();
                        break;
                }
            }
            _ids = new List<long>(ar);
        }

        public IdComparerClass Sorter
        {
            get
            {
                return _sorter;
            }
        }

        //無条件に追加
        private void Add(long ID, bool Read)
        {
            if (this._ids.Contains(ID)) return;

            if (this.Sorter.Mode == IdComparerClass.ComparerMode.Id)
            {
                if (this.Sorter.Order == SortOrder.Ascending)
                {
                    this._ids.Add(ID);
                }
                else
                {
                    this._ids.Insert(0, ID);
                }
            }
            else
            {
                this._ids.Add(ID);
            }

            if (!Read && this._unreadManage)
            {
                this._unreadCount++;
                if (ID < this.OldestUnreadId) this.OldestUnreadId = ID;
                //if (this.OldestUnreadId == -1)
                //{
                //    this.OldestUnreadId = ID;
                //}
                //else
                //{
                //    if (ID < this.OldestUnreadId) this.OldestUnreadId = ID;
                //}
            }
        }

        public void Add(long ID, bool Read, bool Temporary)
        {
            if (!Temporary)
            {
                this.Add(ID, Read);
            }
            else
            {
                _tmpIds.Add(new TemporaryId(ID, Read));
            }
        }

        //フィルタに合致したら追加
        public MyCommon.HITRESULT AddFiltered(PostClass post)
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

            if (rslt != MyCommon.HITRESULT.None && rslt != MyCommon.HITRESULT.Exclude)
            {
                _tmpIds.Add(new TemporaryId(post.StatusId, post.IsRead));
            }

            return rslt; //マーク付けは呼び出し元で行うこと
        }

        //検索結果の追加
        public void AddPostToInnerStorage(PostClass Post)
        {
            if (_innerPosts.ContainsKey(Post.StatusId)) return;
            _innerPosts.Add(Post.StatusId, Post);
            _tmpIds.Add(new TemporaryId(Post.StatusId, Post.IsRead));
        }

        public void AddSubmit(ref bool isMentionIncluded)
        {
            if (_tmpIds.Count == 0) return;
            _tmpIds.Sort((x, y) => x.Id.CompareTo(y.Id));
            foreach (var tId in _tmpIds)
            {
                if (this.TabType == MyCommon.TabUsageType.Mentions && TabInformations.GetInstance()[tId.Id].IsReply) isMentionIncluded = true;
                this.Add(tId.Id, tId.Read);
            }
            _tmpIds.Clear();
        }

        public void AddSubmit()
        {
            bool mention = false;
            AddSubmit(ref mention);
        }

        public void Remove(long Id)
        {
            if (!this._ids.Contains(Id)) return;
            this._ids.Remove(Id);
            if (this.IsInnerStorageTabType) _innerPosts.Remove(Id);
        }

        public void Remove(long Id, bool Read)
        {
            if (!this._ids.Contains(Id)) return;

            if (!Read && this._unreadManage)
            {
                this._unreadCount--;
                this.OldestUnreadId = -1;
            }

            this._ids.Remove(Id);
            if (this.IsInnerStorageTabType) _innerPosts.Remove(Id);
        }

        public bool UnreadManage
        {
            get
            {
                return _unreadManage;
            }
            set
            {
                this._unreadManage = value;
                if (!value)
                {
                    this.OldestUnreadId = -1;
                    this._unreadCount = 0;
                }
            }
        }

        // v1.0.5で「タブを固定(Locked)」から「タブを保護(Protected)」に名称変更
        [XmlElement(ElementName = "Locked")]
        public bool Protected { get; set; }

        public bool Notify { get; set; }

        public string SoundFile { get; set; }

        [XmlIgnore]
        public long OldestUnreadId { get; set; }

        [XmlIgnore]
        public int UnreadCount
        {
            get
            {
                return this.UnreadManage && AppendSettingDialog.Instance.UnreadManage ? _unreadCount : 0;
            }
            set
            {
                if (value < 0) value = 0;
                _unreadCount = value;
            }
        }

        public int AllCount
        {
            get
            {
                return this._ids.Count;
            }
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
                this.FilterModified = true;
            }
        }

        public bool AddFilter(PostFilterRule filter)
        {
            lock (this._lockObj)
            {
                if (_filters.Contains(filter)) return false;
                _filters.Add(filter);
                this.FilterModified = true;
                return true;
            }
        }

        public void EditFilter(PostFilterRule original, PostFilterRule modified)
        {
            original.FilterBody = modified.FilterBody;
            original.FilterName = modified.FilterName;
            original.UseNameField = modified.UseNameField;
            original.FilterByUrl = modified.FilterByUrl;
            original.UseRegex = modified.UseRegex;
            original.CaseSensitive = modified.CaseSensitive;
            original.FilterRt = modified.FilterRt;
            original.UseLambda = modified.UseLambda;
            original.FilterSource = modified.FilterSource;
            original.ExFilterBody = modified.ExFilterBody;
            original.ExFilterName = modified.ExFilterName;
            original.ExUseNameField = modified.ExUseNameField;
            original.ExFilterByUrl = modified.ExFilterByUrl;
            original.ExUseRegex = modified.ExUseRegex;
            original.ExCaseSensitive = modified.ExCaseSensitive;
            original.ExFilterRt = modified.ExFilterRt;
            original.ExUseLambda = modified.ExUseLambda;
            original.ExFilterSource = modified.ExFilterSource;
            original.MoveMatches = modified.MoveMatches;
            original.MarkMatches = modified.MarkMatches;
            this.FilterModified = true;
        }

        [XmlIgnore]
        public List<PostFilterRule> Filters
        {
            get
            {
                lock (this._lockObj)
                {
                    return _filters;
                }
            }
            set
            {
                lock (this._lockObj)
                {
                    _filters = value;
                }
            }
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
                    foreach (var filters in value)
                    {
                        _filters.Add(filters);
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
            _tmpIds.Clear();
            _unreadCount = 0;
            this.OldestUnreadId = -1;
            _innerPosts.Clear();
        }

        public long GetId(int Index)
        {
            return Index < _ids.Count ? _ids[Index] : -1;
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
                _sorter.posts = Posts;
            }
        }

        public bool IsInnerStorageTabType
        {
            get
            {
                if (_tabType == MyCommon.TabUsageType.PublicSearch ||
                    _tabType == MyCommon.TabUsageType.DirectMessage ||
                    _tabType == MyCommon.TabUsageType.Lists ||
                    _tabType == MyCommon.TabUsageType.UserTimeline ||
                    _tabType == MyCommon.TabUsageType.Related)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    //ソート比較クラス：ID比較のみ
    public sealed class IdComparerClass : IComparer<long>
    {
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

        private SortOrder _order;
        private ComparerMode _mode;
        private Dictionary<long, PostClass> _statuses;
        private Comparison<long> _CmpMethod;

        /// <summary>
        /// 昇順か降順か Setの際は同時に比較関数の切り替えを行う
        /// </summary>
        public SortOrder Order
        {
            get
            {
                return _order;
            }
            set
            {
                _order = value;
                SetCmpMethod(_mode, _order);
            }
        }

        /// <summary>
        /// 並び替えの方法 Setの際は同時に比較関数の切り替えを行う
        /// </summary>
        public ComparerMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                SetCmpMethod(_mode, _order);
            }
        }

        /// <summary>
        /// ListViewItemComparerクラスのコンストラクタ（引数付は未使用）
        /// </summary>
        /// <param name="col">並び替える列番号</param>
        /// <param name="ord">昇順か降順か</param>
        /// <param name="cmod">並び替えの方法</param>

        public IdComparerClass()
        {
            _order = SortOrder.Ascending;
            _mode = ComparerMode.Id;
            SetCmpMethod(_mode, _order);
        }

        public Dictionary<long, PostClass> posts
        {
            set
            {
                _statuses = value;
            }
            get
            {
                return _statuses;
            }
        }

        // 指定したソートモードとソートオーダーに従い使用する比較関数のアドレスを返す
        public Comparison<long> CmpMethod(ComparerMode _sortmode, SortOrder _sortorder)
        {
            //get
            {
                Comparison<long> _method = null;
                if (_sortorder == SortOrder.Ascending)
                {
                    // 昇順
                    switch (_sortmode)
                    {
                    case ComparerMode.Data:
                        _method = Compare_ModeData_Ascending;
                        break;
                    case ComparerMode.Id:
                        _method = Compare_ModeId_Ascending;
                        break;
                    case ComparerMode.Name:
                        _method = Compare_ModeName_Ascending;
                        break;
                    case ComparerMode.Nickname:
                        _method = Compare_ModeNickName_Ascending;
                        break;
                    case ComparerMode.Source:
                        _method = Compare_ModeSource_Ascending;
                        break;
                    }
                }
                else
                {
                    // 降順
                    switch (_sortmode)
                    {
                    case ComparerMode.Data:
                        _method = Compare_ModeData_Descending;
                        break;
                    case ComparerMode.Id:
                        _method = Compare_ModeId_Descending;
                        break;
                    case ComparerMode.Name:
                        _method = Compare_ModeName_Descending;
                        break;
                    case ComparerMode.Nickname:
                        _method = Compare_ModeNickName_Descending;
                        break;
                    case ComparerMode.Source:
                        _method = Compare_ModeSource_Descending;
                        break;
                    }
                }
                return _method;
            }
        }

        // ソートモードとソートオーダーに従い使用する比較関数のアドレスを返す
        // (overload 現在の使用中の比較関数のアドレスを返す)
        public Comparison<long> CmpMethod()
        {
            //get
            {
                return _CmpMethod;
            }
        }

        // ソートモードとソートオーダーに従い比較関数のアドレスを切り替え
        private void SetCmpMethod(ComparerMode mode, SortOrder order)
        {
            _CmpMethod = this.CmpMethod(mode, order);
        }

        //xがyより小さいときはマイナスの数、大きいときはプラスの数、
        //同じときは0を返す (こちらは未使用 一応比較関数群呼び出しの形のまま残しておく)
        int IComparer<long>.Compare(long x, long y)
        {
            return _CmpMethod(x, y);
        }

        // 比較用関数群 いずれもステータスIDの順序を考慮する
        // 本文比較　昇順
        public int Compare_ModeData_Ascending(long x, long y)
        {
            var result = string.Compare(_statuses[x].TextFromApi, _statuses[y].TextFromApi);
            if (result == 0) result = x.CompareTo(y);
            return result;
        }

        // 本文比較　降順
        public int Compare_ModeData_Descending(long x, long y)
        {
            var result = string.Compare(_statuses[y].TextFromApi, _statuses[x].TextFromApi);
            if (result == 0) result = y.CompareTo(x);
            return result;
        }

        // ステータスID比較　昇順
        public int Compare_ModeId_Ascending(long x, long y)
        {
            return x.CompareTo(y);
        }

        // ステータスID比較　降順
        public int Compare_ModeId_Descending(long x, long y)
        {
            return y.CompareTo(x);
        }

        // 表示名比較　昇順
        public int Compare_ModeName_Ascending(long x, long y)
        {
            var result = string.Compare(_statuses[x].ScreenName, _statuses[y].ScreenName);
            if (result == 0) result = x.CompareTo(y);
            return result;
        }

        // 表示名比較　降順
        public int Compare_ModeName_Descending(long x, long y)
        {
            var result = string.Compare(_statuses[y].ScreenName, _statuses[x].ScreenName);
            if (result == 0) result = y.CompareTo(x);
            return result;
        }

        // ユーザー名比較　昇順
        public int Compare_ModeNickName_Ascending(long x, long y)
        {
            var result = string.Compare(_statuses[x].Nickname, _statuses[y].Nickname);
            if (result == 0) result = x.CompareTo(y);
            return result;
        }

        // ユーザー名比較　降順
        public int Compare_ModeNickName_Descending(long x, long y)
        {
            var result = string.Compare(_statuses[y].Nickname, _statuses[x].Nickname);
            if (result == 0) result = y.CompareTo(x);
            return result;
        }

        // Source比較　昇順
        public int Compare_ModeSource_Ascending(long x, long y)
        {
            var result = string.Compare(_statuses[x].Source, _statuses[y].Source);
            if (result == 0) result = x.CompareTo(y);
            return result;
        }

        // Source比較　降順
        public int Compare_ModeSource_Descending(long x, long y)
        {
            var result = string.Compare(_statuses[y].Source, _statuses[x].Source);
            if (result == 0) result = y.CompareTo(x);
            return result;
        }
    }
}
