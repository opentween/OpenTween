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
                return geo != null && geo.Lng == this.Lng && geo.Lat == this.Lng;
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
        private long _InReplyToStatusId;
        public string Source { get; set; }
        public string SourceHtml { get; set; }
        public List<string> ReplyToList { get; set; }
        public bool IsMe { get; set; }
        public bool IsDm { get; set; }
        public long UserId { get; set; }
        public bool FilterHit { get; set; }
        public string RetweetedBy { get; set; }
        public long RetweetedId { get; set; }
        private bool _IsDeleted = false;
        private StatusGeo _postGeo = new StatusGeo();
        public int RetweetedCount { get; set; }
        public long RetweetedByUserId { get; set; }
        public long InReplyToUserId { get; set; }
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
                long InReplyToStatusId,
                string Source,
                string SourceHtml,
                List<string> ReplyToList,
                bool IsMe,
                bool IsDm,
                long userId,
                bool FilterHit,
                string RetweetedBy,
                long RetweetedId,
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
                if (this.RetweetedId > 0 && this.GetRetweetSource(this.RetweetedId) != null)
                {
                    return this.GetRetweetSource(this.RetweetedId).IsFav;
                }
                else
                {
                    return _IsFav;
                }
            }
            set
            {
                _IsFav = value;
                if (this.RetweetedId > 0 && this.GetRetweetSource(this.RetweetedId) != null)
                {
                    this.GetRetweetSource(this.RetweetedId).IsFav = value;
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
        public long InReplyToStatusId
        {
            get
            {
                return _InReplyToStatusId;
            }
            set
            {
                if (value > 0)
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
                    this.InReplyToStatusId = 0;
                    this.InReplyToUser = "";
                    this.InReplyToUserId = 0;
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
            RemovedTab = _removedTab;
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
            _tabs.Add(TabName, new TabClass(TabName, TabType, List));
            _tabs[TabName].Sorter.Mode = _sorter.Mode;
            _tabs[TabName].Sorter.Order = _sorter.Order;
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
                if (!_tabs[TabName].IsInnerStorageTabType)
                {
                    var homeTab = GetTabByType(MyCommon.TabUsageType.Home);
                    var dmName = GetTabByType(MyCommon.TabUsageType.DirectMessage).TabName;

                    for (int idx = 0; idx < _tabs[TabName].AllCount; ++idx)
                    {
                        var exist = false;
                        var Id = _tabs[TabName].GetId(idx);
                        if (Id < 0) continue;
                        foreach (var key in _tabs.Keys)
                        {
                            if (key != TabName && key != dmName)
                            {
                                if (_tabs[key].Contains(Id))
                                {
                                    exist = true;
                                    break;
                                }
                            }
                        }
                        if (!exist) homeTab.Add(Id, _statuses[Id].IsRead, false);
                    }
                }
                _removedTab.Push(_tabs[TabName]);
                _tabs.Remove(TabName);
            }
        }

        public Stack<TabClass> RemovedTab;

        public bool ContainsTab(string TabText)
        {
            return _tabs.ContainsKey(TabText);
        }

        public bool ContainsTab(TabClass ts)
        {
            return _tabs.ContainsValue(ts);
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
            foreach (var key in _tabs.Keys)
            {
                _tabs[key].Sort();
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
                foreach (var key in _tabs.Keys)
                {
                    _tabs[key].Sorter.Order = value;
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
                foreach (var key in _tabs.Keys)
                {
                    _tabs[key].Sorter.Mode = value;
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
                foreach (var key in _tabs.Keys)
                {
                    _tabs[key].Sorter.Order = _sorter.Order;
                }
            }
            else
            {
                _sorter.Mode = SortMode;
                _sorter.Order = SortOrder.Ascending;
                foreach (var key in _tabs.Keys)
                {
                    _tabs[key].Sorter.Mode = SortMode;
                    _tabs[key].Sorter.Order = SortOrder.Ascending;
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
            if (_retweets.ContainsKey(Id))
            {
                return _retweets[Id];
            }
            else
            {
                return null;
            }
        }

        public void RemoveFavPost(long Id)
        {
            lock (LockObj)
            {
                PostClass post = null;
                var tab = this.GetTabByType(MyCommon.TabUsageType.Favorites);
                var tn = tab.TabName;
                if (_statuses.ContainsKey(Id))
                {
                    post = _statuses[Id];
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
                    if (tType == MyCommon.TabUsageType.Favorites && post.RetweetedId > 0)
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
                            if (rPost.RetweetedId > 0 && rPost.RetweetedId == post.RetweetedId)
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
                PostClass post = null;
                //if (_statuses.ContainsKey(Id))
                //各タブから該当ID削除
                foreach (var key in _tabs.Keys)
                {
                    var tab = _tabs[key];
                    if (tab.Contains(Id))
                    {
                        if (!tab.IsInnerStorageTabType)
                        {
                            post = _statuses[Id];
                            if (tab.UnreadManage && !post.IsRead)    //未読管理
                            {
                                lock (LockUnread)
                                {
                                    tab.UnreadCount--;
                                    this.SetNextUnreadId(Id, tab);
                                }
                            }
                        }
                        else //未読数がずれる可能性があるためtab.Postsの未読も確認する
                        {
                            if (tab.UnreadManage && !tab.Posts[Id].IsRead)    //未読管理
                            {
                                lock (LockUnread)
                                {
                                    tab.UnreadCount--;
                                    this.SetNextUnreadId(Id, tab);
                                }
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
                PostClass post = null;
                if (_statuses.ContainsKey(Id))
                {
                    post = _statuses[Id];
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
                bool isRead;
                if (!tb.IsInnerStorageTabType)
                {
                    isRead = _statuses[tb.OldestUnreadId].IsRead;
                }
                else
                {
                    isRead = tb.Posts[tb.OldestUnreadId].IsRead;
                }
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
                Dictionary<long, PostClass> posts;
                if (!Tab.IsInnerStorageTabType)
                {
                    posts = _statuses;
                }
                else
                {
                    posts = Tab.Posts;
                }
                if (Tab.OldestUnreadId > -1 &&
                    posts.ContainsKey(Tab.OldestUnreadId) &&
                    posts[Tab.OldestUnreadId].IsRead &&
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
                        }
                        else
                        {
                            //頭から探索
                            FindUnreadId(-1, Tab);
                        }
                    }
                    else
                    {
                        //頭から探索
                        FindUnreadId(-1, Tab);
                    }
                }
                else
                {
                    //頭から探索
                    FindUnreadId(-1, Tab);
                }
            }
            catch (KeyNotFoundException)
            {
                //頭から探索
                FindUnreadId(-1, Tab);
            }
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

            Dictionary<long, PostClass> posts;
            if (!Tab.IsInnerStorageTabType)
            {
                posts = _statuses;
            }
            else
            {
                posts = Tab.Posts;
            }

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
                foreach (var tn in _tabs.Keys)
                {
                    rslt = _tabs[tn].AddFiltered(post);
                    if (rslt != MyCommon.HITRESULT.None && rslt != MyCommon.HITRESULT.Exclude)
                    {
                        if (rslt == MyCommon.HITRESULT.CopyAndMark) post.IsMark = true; //マークあり
                        if (rslt == MyCommon.HITRESULT.Move)
                        {
                            mv = true; //移動
                            post.IsMark = false;
                        }
                        if (_tabs[tn].Notify) add = true; //通知あり
                        if (!string.IsNullOrEmpty(_tabs[tn].SoundFile) && string.IsNullOrEmpty(_soundFile))
                        {
                            _soundFile = _tabs[tn].SoundFile; //wavファイル（未設定の場合のみ）
                        }
                        post.FilterHit = true;
                    }
                    else
                    {
                        if (rslt == MyCommon.HITRESULT.Exclude && _tabs[tn].TabType == MyCommon.TabUsageType.Mentions)
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
                        if (_statuses.ContainsKey(Item.StatusId))
                        {
                            if (Item.IsFav)
                            {
                                if (Item.RetweetedId == 0)
                                {
                                    _statuses[Item.StatusId].IsFav = true;
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
                            if (Item.IsFav && Item.RetweetedId > 0) Item.IsFav = false;
                            //既に持っている公式RTは捨てる
                            if (AppendSettingDialog.Instance.HideDuplicatedRetweets &&
                                !Item.IsMe &&
                                this._retweets.ContainsKey(Item.RetweetedId) &&
                                this._retweets[Item.RetweetedId].RetweetedCount > 0) return;
                            if (BlockIds.Contains(Item.UserId)) return;
                            _statuses.Add(Item.StatusId, Item);
                        }
                        if (Item.RetweetedId > 0)
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
                    if (this.Tabs.ContainsKey(Item.RelTabName))
                    {
                        tb = this.Tabs[Item.RelTabName];
                    }
                    else
                    {
                        return;
                    }
                    if (tb == null) return;
                    if (tb.Contains(Item.StatusId)) return;
                    //tb.Add(Item.StatusId, Item.IsRead, true);
                    tb.AddPostToInnerStorage(Item);
                }
            }
        }

        private void AddRetweet(PostClass item)
        {
            //true:追加、False:保持済み
            if (_retweets.ContainsKey(item.RetweetedId))
            {
                _retweets[item.RetweetedId].RetweetedCount++;
                if (_retweets[item.RetweetedId].RetweetedCount > 10)
                {
                    _retweets[item.RetweetedId].RetweetedCount = 0;
                }
                return;
            }

            _retweets.Add(
                        item.RetweetedId,
                        new PostClass(
                            item.Nickname,
                            item.TextFromApi,
                            item.Text,
                            item.ImageUrl,
                            item.ScreenName,
                            item.CreatedAt,
                            item.RetweetedId,
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
                            0,
                            item.PostGeo
                        )
                    );
            _retweets[item.RetweetedId].RetweetedCount++;
        }

        public void SetReadAllTab(bool Read, string TabName, int Index)
        {
            //Read:true=既読へ　false=未読へ
            var tb = _tabs[TabName];

            if (tb.UnreadManage == false) return; //未読管理していなければ終了

            var Id = tb.GetId(Index);
            if (Id < 0) return;
            PostClass post;
            if (!tb.IsInnerStorageTabType)
            {
                post = _statuses[Id];
            }
            else
            {
                post = tb.Posts[Id];
            }

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
                        if (_statuses.ContainsKey(Id) && !_statuses[Id].IsRead)
                        {
                            foreach (var key in _tabs.Keys)
                            {
                                if (_tabs[key].UnreadManage &&
                                    _tabs[key].Contains(Id) &&
                                    !_tabs[key].IsInnerStorageTabType)
                                {
                                    _tabs[key].UnreadCount--;
                                    if (_tabs[key].OldestUnreadId == Id) _tabs[key].OldestUnreadId = -1;
                                }
                            }
                            _statuses[Id].IsRead = true;
                        }
                    }
                    else
                    {
                        //一般タブ
                        foreach (var key in _tabs.Keys)
                        {
                            if (key != TabName &&
                                _tabs[key].UnreadManage &&
                                _tabs[key].Contains(Id) &&
                                !_tabs[key].IsInnerStorageTabType)
                            {
                                _tabs[key].UnreadCount--;
                                if (_tabs[key].OldestUnreadId == Id) _tabs[key].OldestUnreadId = -1;
                            }
                        }
                    }
                    //内部保存タブ
                    foreach (var key in _tabs.Keys)
                    {
                        if (key != TabName &&
                            _tabs[key].Contains(Id) &&
                            _tabs[key].IsInnerStorageTabType &&
                            !_tabs[key].Posts[Id].IsRead)
                        {
                            if (_tabs[key].UnreadManage)
                            {
                                _tabs[key].UnreadCount--;
                                if (_tabs[key].OldestUnreadId == Id) _tabs[key].OldestUnreadId = -1;
                            }
                            _tabs[key].Posts[Id].IsRead = true;
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
                        if (_statuses.ContainsKey(Id) && _statuses[Id].IsRead)
                        {
                            foreach (var key in _tabs.Keys)
                            {
                                if (_tabs[key].UnreadManage &&
                                   _tabs[key].Contains(Id) &&
                                   !_tabs[key].IsInnerStorageTabType)
                                    _tabs[key].UnreadCount++;
                                {
                                    if (_tabs[key].OldestUnreadId > Id) _tabs[key].OldestUnreadId = Id;
                                }
                            }
                            _statuses[Id].IsRead = false;
                        }
                    }
                    else
                    {
                        //一般タブ
                        foreach (var key in _tabs.Keys)
                        {
                            if (key != TabName &&
                               _tabs[key].UnreadManage &&
                               _tabs[key].Contains(Id) &&
                               !_tabs[key].IsInnerStorageTabType)
                            {
                                _tabs[key].UnreadCount++;
                                if (_tabs[key].OldestUnreadId > Id) _tabs[key].OldestUnreadId = Id;
                            }
                        }
                    }
                    //内部保存タブ
                    foreach (var key in _tabs.Keys)
                    {
                        if (key != TabName &&
                            _tabs[key].Contains(Id) &&
                            _tabs[key].IsInnerStorageTabType &&
                            _tabs[key].Posts[Id].IsRead)
                        {
                            if (_tabs[key].UnreadManage)
                            {
                                _tabs[key].UnreadCount++;
                                if (_tabs[key].OldestUnreadId > Id) _tabs[key].OldestUnreadId = Id;
                            }
                            _tabs[key].Posts[Id].IsRead = false;
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
            PostClass post;
            if (!tb.IsInnerStorageTabType)
            {
                post = _statuses[Id];
            }
            else
            {
                post = tb.Posts[Id];
            }

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
                    foreach (var key in _tabs.Keys)
                    {
                        if (key != TabName &&
                            _tabs[key].UnreadManage &&
                            _tabs[key].Contains(Id) &&
                            !_tabs[key].IsInnerStorageTabType)
                        {
                            _tabs[key].UnreadCount--;
                            if (_tabs[key].OldestUnreadId == Id) _tabs[key].OldestUnreadId = -1;
                        }
                    }
                }
                else
                {
                    tb.UnreadCount++;
                    //if (tb.OldestUnreadId > Id || tb.OldestUnreadId == -1) tb.OldestUnreadId = Id;
                    if (tb.OldestUnreadId > Id) tb.OldestUnreadId = Id;
                    if (tb.IsInnerStorageTabType) return;
                    foreach (var key in _tabs.Keys)
                    {
                        if (key != TabName &&
                            _tabs[key].UnreadManage &&
                            _tabs[key].Contains(Id) &&
                            !_tabs[key].IsInnerStorageTabType)
                        {
                            _tabs[key].UnreadCount++;
                            if (_tabs[key].OldestUnreadId > Id) _tabs[key].OldestUnreadId = Id;
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
                    if (!_statuses[id].IsReply &&
                        !_statuses[id].IsRead &&
                        !_statuses[id].FilterHit)
                    {
                        _statuses[id].IsRead = true;
                        this.SetNextUnreadId(id, tb);  //次の未読セット
                        foreach (var key in _tabs.Keys)
                        {
                            if (_tabs[key].UnreadManage &&
                                _tabs[key].Contains(id))
                            {
                                _tabs[key].UnreadCount--;
                                if (_tabs[key].OldestUnreadId == id) _tabs[key].OldestUnreadId = -1;
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
                if (_statuses.ContainsKey(ID)) return _statuses[ID];
                foreach (var tb in this.GetTabsInnerStorageType())
                {
                    if (tb.Contains(ID))
                    {
                        return tb.Posts[ID];
                    }
                }
                return null;
            }
        }

        public PostClass this[string TabName, int Index]
        {
            get
            {
                if (!_tabs.ContainsKey(TabName)) throw new ArgumentException("TabName=" + TabName + " is not contained.");
                var id = _tabs[TabName].GetId(Index);
                if (id < 0) throw new ArgumentException("Index can//t find. Index=" + Index.ToString() + "/TabName=" + TabName);
                try
                {
                    if (_tabs[TabName].IsInnerStorageTabType)
                    {
                        return _tabs[TabName].Posts[_tabs[TabName].GetId(Index)];
                    }
                    else
                    {
                        return _statuses[_tabs[TabName].GetId(Index)];
                    }
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
                var length = EndIndex - StartIndex + 1;
                var posts = new PostClass[length];
                if (_tabs[TabName].IsInnerStorageTabType)
                {
                    for (int i = 0; i < length; i++)
                    {
                        posts[i] = _tabs[TabName].Posts[_tabs[TabName].GetId(StartIndex + i)];
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++)
                    {
                        posts[i] = _statuses[_tabs[TabName].GetId(StartIndex + i)];
                    }
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
                if (_tabs.ContainsKey(TabName))
                {
                    return _tabs[TabName].Contains(Id);
                }
                else
                {
                    return false;
                }
            }
        }

        public void SetUnreadManage(bool Manage)
        {
            if (Manage)
            {
                foreach (var key in _tabs.Keys)
                {
                    var tb = _tabs[key];
                    if (tb.UnreadManage)
                    {
                        lock (LockUnread)
                        {
                            var cnt = 0;
                            var oldest = long.MaxValue;
                            Dictionary<long, PostClass> posts;
                            if (!tb.IsInnerStorageTabType)
                            {
                                posts = _statuses;
                            }
                            else
                            {
                                posts = tb.Posts;
                            }
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
                    }
                }
            }
            else
            {
                foreach (var key in _tabs.Keys)
                {
                    var tb = _tabs[key];
                    if (tb.UnreadManage && tb.UnreadCount > 0)
                    {
                        lock (LockUnread)
                        {
                            tb.UnreadCount = 0;
                            tb.OldestUnreadId = -1;
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
                        foreach (var id in _statuses.Keys)
                        {
                            var post = _statuses[id];
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
                if (!_tabs[TabName].IsInnerStorageTabType)
                {
                    foreach (var Id in _tabs[TabName].BackupIds)
                    {
                        var Hit = false;
                        foreach (var tb in _tabs.Values)
                        {
                            if (tb.Contains(Id))
                            {
                                Hit = true;
                                break;
                            }
                        }
                        if (!Hit) _statuses.Remove(Id);
                    }
                }

                //指定タブをクリア
                _tabs[TabName].ClearIDs();
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
                    Dictionary<long, PostClass> posts;
                    if (!tb.IsInnerStorageTabType)
                    {
                        posts = _statuses;
                    }
                    else
                    {
                        posts = tb.Posts;
                    }
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
                    foreach (var id in _statuses.Keys)
                    {
                        _statuses[id].IsOwl = false;
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
                foreach (var tb in _tabs.Values)
                {
                    if (tb != null && tb.TabType == tabType) return tb;
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
                if (_tabs.ContainsKey(tabName)) return _tabs[tabName];
                return null;
            }
        }

        // デフォルトタブの判定処理
        public bool IsDefaultTab(string tabName)
        {
            if (tabName != null &&
               _tabs.ContainsKey(tabName) &&
               (_tabs[tabName].TabType == MyCommon.TabUsageType.Home ||
               _tabs[tabName].TabType == MyCommon.TabUsageType.Mentions ||
               _tabs[tabName].TabType == MyCommon.TabUsageType.DirectMessage ||
               _tabs[tabName].TabType == MyCommon.TabUsageType.Favorites))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //振り分け可能タブの判定処理
        public bool IsDistributableTab(string tabName)
        {
            return tabName != null &&
                this._tabs.ContainsKey(tabName) &&
                (_tabs[tabName].TabType == MyCommon.TabUsageType.Mentions ||
                 _tabs[tabName].TabType == MyCommon.TabUsageType.UserDefined);
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
        private List<FiltersClass> _filters;
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
        public Dictionary<long, PostClass> Posts { get; set; }

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
            Posts = new Dictionary<long, PostClass>();
            SoundFile = "";
            OldestUnreadId = -1;
            TabName = "";
            _filters = new List<FiltersClass>();
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
            _tabType = TabType;
            this.ListInfo = list;
            if (this.IsInnerStorageTabType)
            {
                _sorter.posts = Posts;
            }
            else
            {
                _sorter.posts = TabInformations.GetInstance().Posts;
            }
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
                        switch (ft.IsHit(post))   //フィルタクラスでヒット判定
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
                        //IsHitでNullRef出る場合あり。暫定対応
                        MyCommon.TraceOut("IsHitでNullRef: " + ft.ToString());
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
            if (Posts.ContainsKey(Post.StatusId)) return;
            Posts.Add(Post.StatusId, Post);
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
            if (this.IsInnerStorageTabType) Posts.Remove(Id);
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
            if (this.IsInnerStorageTabType) Posts.Remove(Id);
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

        public FiltersClass[] GetFilters()
        {
            lock (this._lockObj)
            {
                return _filters.ToArray();
            }
        }

        public void RemoveFilter(FiltersClass filter)
        {
            lock (this._lockObj)
            {
                _filters.Remove(filter);
                this.FilterModified = true;
            }
        }

        public bool AddFilter(FiltersClass filter)
        {
            lock (this._lockObj)
            {
                if (_filters.Contains(filter)) return false;
                _filters.Add(filter);
                this.FilterModified = true;
                return true;
            }
        }

        public void EditFilter(FiltersClass original, FiltersClass modified)
        {
            original.BodyFilter = modified.BodyFilter;
            original.NameFilter = modified.NameFilter;
            original.SearchBoth = modified.SearchBoth;
            original.SearchUrl = modified.SearchUrl;
            original.UseRegex = modified.UseRegex;
            original.CaseSensitive = modified.CaseSensitive;
            original.IsRt = modified.IsRt;
            original.UseLambda = modified.UseLambda;
            original.Source = modified.Source;
            original.ExBodyFilter = modified.ExBodyFilter;
            original.ExNameFilter = modified.ExNameFilter;
            original.ExSearchBoth = modified.ExSearchBoth;
            original.ExSearchUrl = modified.ExSearchUrl;
            original.ExUseRegex = modified.ExUseRegex;
            original.ExCaseSensitive = modified.ExCaseSensitive;
            original.IsExRt = modified.IsExRt;
            original.ExUseLambda = modified.ExUseLambda;
            original.ExSource = modified.ExSource;
            original.MoveFrom = modified.MoveFrom;
            original.SetMark = modified.SetMark;
            this.FilterModified = true;
        }

        [XmlIgnore]
        public List<FiltersClass> Filters
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

        public FiltersClass[] FilterArray
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
            if (Posts != null)
            {
                Posts.Clear();
            }
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
                    _sorter.posts = Posts;
                }
                else
                {
                    _sorter.posts = TabInformations.GetInstance().Posts;
                }
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

    [Serializable]
    public sealed class FiltersClass : System.IEquatable<FiltersClass>
    {
        private string _name = "";
        private List<string> _body = new List<string>();
        private bool _searchBoth = true;
        private bool _searchUrl = false;
        private bool _caseSensitive = false;
        private bool _useRegex = false;
        private bool _isRt = false;
        private string _source = "";
        private string _exname = "";
        private List<string> _exbody = new List<string>();
        private bool _exsearchBoth = true;
        private bool _exsearchUrl = false;
        private bool _exuseRegex = false;
        private bool _excaseSensitive = false;
        private bool _isExRt = false;
        private string _exSource = "";
        private bool _moveFrom = false;
        private bool _setMark = true;
        private bool _useLambda = false;
        private bool _exuseLambda = false;

        public FiltersClass() {}

        //フィルタ一覧に表示する文言生成
        private string MakeSummary()
        {
            var fs = new StringBuilder();
            if (!string.IsNullOrEmpty(_name) || _body.Count > 0 || _isRt || !string.IsNullOrEmpty(_source))
            {
                if (_searchBoth)
                {
                    if (!string.IsNullOrEmpty(_name))
                    {
                        fs.AppendFormat(Properties.Resources.SetFiltersText1, _name);
                    }
                    else
                    {
                        fs.Append(Properties.Resources.SetFiltersText2);
                    }
                }
                if (_body.Count > 0)
                {
                    fs.Append(Properties.Resources.SetFiltersText3);
                    foreach (var bf in _body)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }
                    fs.Length--;
                    fs.Append(Properties.Resources.SetFiltersText4);
                }
                fs.Append("(");
                if (_searchBoth)
                {
                    fs.Append(Properties.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Properties.Resources.SetFiltersText6);
                }
                if (_useRegex)
                {
                    fs.Append(Properties.Resources.SetFiltersText7);
                }
                if (_searchUrl)
                {
                    fs.Append(Properties.Resources.SetFiltersText8);
                }
                if (_caseSensitive)
                {
                    fs.Append(Properties.Resources.SetFiltersText13);
                }
                if (_isRt)
                {
                    fs.Append("RT/");
                }
                if (_useLambda)
                {
                    fs.Append("LambdaExp/");
                }
                if (!string.IsNullOrEmpty(_source))
                {
                    fs.AppendFormat("Src…{0}/", _source);
                }
                fs.Length--;
                fs.Append(")");
            }
            if (!string.IsNullOrEmpty(_exname) || _exbody.Count > 0 || _isExRt || !string.IsNullOrEmpty(_exSource))
            {
                //除外
                fs.Append(Properties.Resources.SetFiltersText12);
                if (_exsearchBoth)
                {
                    if (!string.IsNullOrEmpty(_exname))
                    {
                        fs.AppendFormat(Properties.Resources.SetFiltersText1, _exname);
                    }
                    else
                    {
                        fs.Append(Properties.Resources.SetFiltersText2);
                    }
                }
                if (_exbody.Count > 0)
                {
                    fs.Append(Properties.Resources.SetFiltersText3);
                    foreach (var bf in _exbody)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }
                    fs.Length--;
                    fs.Append(Properties.Resources.SetFiltersText4);
                }
                fs.Append("(");
                if (_exsearchBoth)
                {
                    fs.Append(Properties.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Properties.Resources.SetFiltersText6);
                }
                if (_exuseRegex)
                {
                    fs.Append(Properties.Resources.SetFiltersText7);
                }
                if (_exsearchUrl)
                {
                    fs.Append(Properties.Resources.SetFiltersText8);
                }
                if (_excaseSensitive)
                {
                    fs.Append(Properties.Resources.SetFiltersText13);
                }
                if (_isExRt)
                {
                    fs.Append("RT/");
                }
                if (_exuseLambda)
                {
                    fs.Append("LambdaExp/");
                }
                if (!string.IsNullOrEmpty(_exSource))
                {
                    fs.AppendFormat("Src…{0}/", _exSource);
                }
                fs.Length--;
                fs.Append(")");
            }

            fs.Append("(");
            if (_moveFrom)
            {
                fs.Append(Properties.Resources.SetFiltersText9);
            }
            else
            {
                fs.Append(Properties.Resources.SetFiltersText11);
            }
            if (!_moveFrom && _setMark)
            {
                fs.Append(Properties.Resources.SetFiltersText10);
            }
            else if (!_moveFrom)
            {
                fs.Length--;
            }

            fs.Append(")");

            return fs.ToString();
        }

        public string NameFilter
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string ExNameFilter
        {
            get
            {
                return _exname;
            }
            set
            {
                _exname = value;
            }
        }

        [XmlIgnore]
        public List<string> BodyFilter
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }
        }

        public string[] BodyFilterArray
        {
            get
            {
                return _body.ToArray();
            }
            set
            {
                _body = new List<string>();
                foreach (var filter in value)
                {
                    _body.Add(filter);
                }
            }
        }

        [XmlIgnore]
        public List<string> ExBodyFilter
        {
            get
            {
                return _exbody;
            }
            set
            {
                _exbody = value;
            }
        }

        public string[] ExBodyFilterArray
        {
            get
            {
                return _exbody.ToArray();
            }
            set
            {
                _exbody = new List<string>();
                foreach (var filter in value)
                {
                    _exbody.Add(filter);
                }
            }
        }

        public bool SearchBoth
        {
            get
            {
                return _searchBoth;
            }
            set
            {
                _searchBoth = value;
            }
        }

        public bool ExSearchBoth
        {
            get
            {
                return _exsearchBoth;
            }
            set
            {
                _exsearchBoth = value;
            }
        }

        public bool MoveFrom
        {
            get
            {
                return _moveFrom;
            }
            set
            {
                _moveFrom = value;
            }
        }

        public bool SetMark
        {
            get
            {
                return _setMark;
            }
            set
            {
                _setMark = value;
            }
        }

        public bool SearchUrl
        {
            get
            {
                return _searchUrl;
            }
            set
            {
                _searchUrl = value;
            }
        }

        public bool ExSearchUrl
        {
            get
            {
                return _exsearchUrl;
            }
            set
            {
                _exsearchUrl = value;
            }
        }

        public bool CaseSensitive
        {
            get
            {
                return _caseSensitive;
            }
            set
            {
                _caseSensitive = value;
            }
        }

        public bool ExCaseSensitive
        {
            get
            {
                return _excaseSensitive;
            }
            set
            {
                _excaseSensitive = value;
            }
        }

        public bool UseLambda
        {
            get
            {
                return _useLambda;
            }
            set
            {
                _useLambda = value;
            }
        }

        public bool ExUseLambda
        {
            get
            {
                return _exuseLambda;
            }
            set
            {
                _exuseLambda = value;
            }
        }

        public bool UseRegex
        {
            get
            {
                return _useRegex;
            }
            set
            {
                _useRegex = value;
            }
        }

        public bool ExUseRegex
        {
            get
            {
                return _exuseRegex;
            }
            set
            {
                _exuseRegex = value;
            }
        }

        public bool IsRt
        {
            get
            {
                return _isRt;
            }
            set
            {
                _isRt = value;
            }
        }

        public bool IsExRt
        {
            get
            {
                return _isExRt;
            }
            set
            {
                _isExRt = value;
            }
        }

        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        public string ExSource
        {
            get
            {
                return _exSource;
            }
            set
            {
                _exSource = value;
            }
        }

        public override string ToString()
        {
            return MakeSummary();
        }

        public bool ExecuteLambdaExpression(string expr, PostClass post)
        {
            return false;
            // TODO DynamicQuery相当のGPLv3互換なライブラリで置換する
        }

        public bool ExecuteExLambdaExpression(string expr, PostClass post)
        {
            return false;
            // TODO DynamicQuery相当のGPLv3互換なライブラリで置換する
        }

        public MyCommon.HITRESULT IsHit(PostClass post)
        {
            var bHit = true;
            string tBody;
            string tSource;
            if (_searchUrl)
            {
                tBody = post.Text;
                tSource = post.SourceHtml;
            }
            else
            {
                tBody = post.TextFromApi;
                tSource = post.Source;
            }
            //検索オプション
            System.StringComparison compOpt;
            System.Text.RegularExpressions.RegexOptions rgOpt;
            if (_caseSensitive)
            {
                compOpt = StringComparison.Ordinal;
                rgOpt = RegexOptions.None;
            }
            else
            {
                compOpt = StringComparison.OrdinalIgnoreCase;
                rgOpt = RegexOptions.IgnoreCase;
            }
            if (_searchBoth)
            {
                if (string.IsNullOrEmpty(_name) ||
                    (!_useRegex &&
                     (post.ScreenName.Equals(_name, compOpt) ||
                      post.RetweetedBy.Equals(_name, compOpt)
                     )
                    ) ||
                    (_useRegex &&
                     (Regex.IsMatch(post.ScreenName, _name, rgOpt) ||
                      (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, _name, rgOpt))
                     )
                    ))
                {
                    if (_useLambda)
                    {
                        if (!ExecuteLambdaExpression(_body[0], post)) bHit = false;
                    }
                    else
                    {
                        foreach (var fs in _body)
                        {
                            if (_useRegex)
                            {
                                if (!Regex.IsMatch(tBody, fs, rgOpt)) bHit = false;
                            }
                            else
                            {
                                if (_caseSensitive)
                                {
                                    if (!tBody.Contains(fs)) bHit = false;
                                }
                                else
                                {
                                    if (!tBody.ToLower().Contains(fs.ToLower())) bHit = false;
                                }
                            }
                            if (!bHit) break;
                        }
                    }
                }
                else
                {
                    bHit = false;
                }
            }
            else
            {
                if (_useLambda)
                {
                    if (!ExecuteLambdaExpression(_body[0], post)) bHit = false;
                }
                else
                {
                    foreach (var fs in _body)
                    {
                        if (_useRegex)
                        {
                            if (!(Regex.IsMatch(post.ScreenName, fs, rgOpt) ||
                                 (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, rgOpt)) ||
                                 Regex.IsMatch(tBody, fs, rgOpt))) bHit = false;
                        }
                        else
                        {
                            if (_caseSensitive)
                            {
                                if (!(post.ScreenName.Contains(fs) ||
                                    post.RetweetedBy.Contains(fs) ||
                                    tBody.Contains(fs))) bHit = false;
                            }
                            else
                            {
                                if (!(post.ScreenName.ToLower().Contains(fs.ToLower()) ||
                                    post.RetweetedBy.ToLower().Contains(fs.ToLower()) ||
                                    tBody.ToLower().Contains(fs.ToLower()))) bHit = false;
                            }
                        }
                        if (!bHit) break;
                    }
                }
            }
            if (_isRt)
            {
                if (post.RetweetedId == 0) bHit = false;
            }
            if (!string.IsNullOrEmpty(_source))
            {
                if (_useRegex)
                {
                    if (!Regex.IsMatch(tSource, _source, rgOpt)) bHit = false;
                }
                else
                {
                    if (!tSource.Equals(_source, compOpt)) bHit = false;
                }
            }
            if (bHit)
            {
                //除外判定
                if (_exsearchUrl)
                {
                    tBody = post.Text;
                    tSource = post.SourceHtml;
                }
                else
                {
                    tBody = post.TextFromApi;
                    tSource = post.Source;
                }

                var exFlag = false;
                if (!string.IsNullOrEmpty(_exname) || _exbody.Count > 0)
                {
                    if (_excaseSensitive)
                    {
                        compOpt = StringComparison.Ordinal;
                        rgOpt = RegexOptions.None;
                    }
                    else
                    {
                        compOpt = StringComparison.OrdinalIgnoreCase;
                        rgOpt = RegexOptions.IgnoreCase;
                    }
                    if (_exsearchBoth)
                    {
                        if (string.IsNullOrEmpty(_exname) ||
                            (!_exuseRegex &&
                             (post.ScreenName.Equals(_exname, compOpt) ||
                              post.RetweetedBy.Equals(_exname, compOpt)
                             )
                            ) ||
                            (_exuseRegex &&
                                (Regex.IsMatch(post.ScreenName, _exname, rgOpt) ||
                                 (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, _exname, rgOpt))
                                )
                            ))
                        {
                            if (_exbody.Count > 0)
                            {
                                if (_exuseLambda)
                                {
                                    if (ExecuteExLambdaExpression(_exbody[0], post)) exFlag = true;
                                }
                                else
                                {
                                    foreach (var fs in _exbody)
                                    {
                                        if (_exuseRegex)
                                        {
                                            if (Regex.IsMatch(tBody, fs, rgOpt)) exFlag = true;
                                        }
                                        else
                                        {
                                            if (_excaseSensitive)
                                            {
                                                if (tBody.Contains(fs)) exFlag = true;
                                            }
                                            else
                                            {
                                                if (tBody.ToLower().Contains(fs.ToLower())) exFlag = true;
                                            }
                                        }
                                        if (exFlag) break;
                                    }
                                }
                            }
                            else
                            {
                                exFlag = true;
                            }
                        }
                    }
                    else
                    {
                        if (_exuseLambda)
                        {
                            if (ExecuteExLambdaExpression(_exbody[0], post)) exFlag = true;
                        }
                        else
                        {
                            foreach (var fs in _exbody)
                            {
                                if (_exuseRegex)
                                {
                                    if (Regex.IsMatch(post.ScreenName, fs, rgOpt) ||
                                       (!string.IsNullOrEmpty(post.RetweetedBy) && Regex.IsMatch(post.RetweetedBy, fs, rgOpt)) ||
                                       Regex.IsMatch(tBody, fs, rgOpt)) exFlag = true;
                                }
                                else
                                {
                                    if (_excaseSensitive)
                                    {
                                        if (post.ScreenName.Contains(fs) ||
                                           post.RetweetedBy.Contains(fs) ||
                                           tBody.Contains(fs)) exFlag = true;
                                    }
                                    else
                                    {
                                        if (post.ScreenName.ToLower().Contains(fs.ToLower()) ||
                                           post.RetweetedBy.ToLower().Contains(fs.ToLower()) ||
                                           tBody.ToLower().Contains(fs.ToLower())) exFlag = true;
                                    }
                                }
                                if (exFlag) break;
                            }
                        }
                    }
                }
                if (_isExRt)
                {
                    if (post.RetweetedId > 0) exFlag = true;
                }
                if (!string.IsNullOrEmpty(_exSource))
                {
                    if (_exuseRegex)
                    {
                        if (Regex.IsMatch(tSource, _exSource, rgOpt)) exFlag = true;
                    }
                    else
                    {
                        if (tSource.Equals(_exSource, compOpt)) exFlag = true;
                    }
                }

                if (string.IsNullOrEmpty(_name) && _body.Count == 0 && !_isRt && string.IsNullOrEmpty(_source))
                {
                    bHit = false;
                }
                if (bHit)
                {
                    if (!exFlag)
                    {
                        if (_moveFrom)
                        {
                            return MyCommon.HITRESULT.Move;
                        }
                        else
                        {
                            if (_setMark)
                            {
                                return MyCommon.HITRESULT.CopyAndMark;
                            }
                            return MyCommon.HITRESULT.Copy;
                        }
                    }
                    else
                    {
                        return MyCommon.HITRESULT.Exclude;
                    }
                }
                else
                {
                    if (exFlag)
                    {
                        return MyCommon.HITRESULT.Exclude;
                    }
                    else
                    {
                        return MyCommon.HITRESULT.None;
                    }
                }
            }
            else
            {
                return MyCommon.HITRESULT.None;
            }
        }

        public bool Equals(FiltersClass other)
        {
            if (this.BodyFilter.Count != other.BodyFilter.Count) return false;
            if (this.ExBodyFilter.Count != other.ExBodyFilter.Count) return false;
            for (int i = 0; i < this.BodyFilter.Count; i++)
            {
                if (this.BodyFilter[i] != other.BodyFilter[i]) return false;
            }
            for (int i = 0; i < this.ExBodyFilter.Count; i++)
            {
                if (this.ExBodyFilter[i] != other.ExBodyFilter[i]) return false;
            }

            return (this.MoveFrom == other.MoveFrom) &
                   (this.SetMark == other.SetMark) &
                   (this.NameFilter == other.NameFilter) &
                   (this.SearchBoth == other.SearchBoth) &
                   (this.SearchUrl == other.SearchUrl) &
                   (this.UseRegex == other.UseRegex) &
                   (this.ExNameFilter == other.ExNameFilter) &
                   (this.ExSearchBoth == other.ExSearchBoth) &
                   (this.ExSearchUrl == other.ExSearchUrl) &
                   (this.ExUseRegex == other.ExUseRegex) &
                   (this.IsRt == other.IsRt) &
                   (this.Source == other.Source) &
                   (this.IsExRt == other.IsExRt) &
                   (this.ExSource == other.ExSource) &
                   (this.UseLambda == other.UseLambda) &
                   (this.ExUseLambda == other.ExUseLambda);
        }

        public FiltersClass CopyTo(FiltersClass destination)
        {
            if (this.BodyFilter.Count > 0)
            {
                foreach (var flt in this.BodyFilter)
                {
                    destination.BodyFilter.Add(string.Copy(flt));
                }
            }

            if (this.ExBodyFilter.Count > 0)
            {
                foreach (var flt in this.ExBodyFilter)
                {
                    destination.ExBodyFilter.Add(string.Copy(flt));
                }
            }

            destination.MoveFrom = this.MoveFrom;
            destination.SetMark = this.SetMark;
            destination.NameFilter = this.NameFilter;
            destination.SearchBoth = this.SearchBoth;
            destination.SearchUrl = this.SearchUrl;
            destination.UseRegex = this.UseRegex;
            destination.ExNameFilter = this.ExNameFilter;
            destination.ExSearchBoth = this.ExSearchBoth;
            destination.ExSearchUrl = this.ExSearchUrl;
            destination.ExUseRegex = this.ExUseRegex;
            destination.IsRt = this.IsRt;
            destination.Source = this.Source;
            destination.IsExRt = this.IsExRt;
            destination.ExSource = this.ExSource;
            destination.UseLambda = this.UseLambda;
            destination.ExUseLambda = this.ExUseLambda;
            return destination;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this.Equals((FiltersClass)obj);
        }

        public override int GetHashCode()
        {
            return this.MoveFrom.GetHashCode() ^
                   this.SetMark.GetHashCode() ^
                   this.BodyFilter.GetHashCode() ^
                   this.NameFilter.GetHashCode() ^
                   this.SearchBoth.GetHashCode() ^
                   this.SearchUrl.GetHashCode() ^
                   this.UseRegex.GetHashCode() ^
                   this.ExBodyFilter.GetHashCode() ^
                   this.ExNameFilter.GetHashCode() ^
                   this.ExSearchBoth.GetHashCode() ^
                   this.ExSearchUrl.GetHashCode() ^
                   this.ExUseRegex.GetHashCode() ^
                   this.IsRt.GetHashCode() ^
                   this.Source.GetHashCode() ^
                   this.IsExRt.GetHashCode() ^
                   this.ExSource.GetHashCode() ^
                   this.UseLambda.GetHashCode() ^
                   this.ExUseLambda.GetHashCode();
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
