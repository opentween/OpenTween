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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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
        public Uri SourceUri { get; set; }
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
        public List<MediaInfo> Media { get; set; }
        public long[] QuoteStatusIds { get; set; }

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
                Uri SourceUri,
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
            this.SourceUri = SourceUri;
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
                    (this.SourceUri == other.SourceUri) &&
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
            clone.Media = new List<MediaInfo>(this.Media);
            clone.QuoteStatusIds = this.QuoteStatusIds.ToArray();

            return clone;
        }
#endregion
    }

    public class MediaInfo
    {
        public string Url { get; private set; }
        public string VideoUrl { get; private set; }

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
        private Dictionary<long, PostClass> _statuses = new Dictionary<long, PostClass>();
        private List<long> _addedIds;
        private List<long> _deletedIds = new List<long>();
        private Dictionary<long, PostClass> _retweets = new Dictionary<long, PostClass>();
        private Dictionary<long, PostClass> _quotes = new Dictionary<long, PostClass>();
        private Stack<TabClass> _removedTab = new Stack<TabClass>();

        public ISet<long> BlockIds = new HashSet<long>();
        public ISet<long> MuteUserIds = new HashSet<long>();

        //発言の追加
        //AddPost(複数回) -> DistributePosts          -> SubmitUpdate

        //トランザクション用
        private int _addCount;
        private string _soundFile;
        private List<PostClass> _notifyPosts;
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
            tb.SortMode = this.SortMode;
            tb.SortOrder = this.SortOrder;
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

        private SortOrder _sortOrder = SortOrder.Ascending;
        public SortOrder SortOrder
        {
            get { return this._sortOrder; }
            set
            {
                this._sortOrder = value;
                foreach (var tab in _tabs.Values)
                {
                    tab.SortOrder = value;
                }
            }
        }

        private ComparerMode _sortMode = ComparerMode.Id;
        public ComparerMode SortMode
        {
            get { return this._sortMode; }
            set
            {
                this._sortMode = value;
                foreach (var tab in _tabs.Values)
                {
                    tab.SortMode = value;
                }
            }
        }

        public SortOrder ToggleSortOrder(ComparerMode sortMode)
        {
            if (this.SortMode == sortMode)
            {
                if (this.SortOrder == SortOrder.Ascending)
                {
                    this.SortOrder = SortOrder.Descending;
                }
                else
                {
                    this.SortOrder = SortOrder.Ascending;
                }
            }
            else
            {
                this.SortMode = sortMode;
                this.SortOrder = SortOrder.Ascending;
            }

            this.SortPosts();

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
                        tab.Remove(Id);
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
                if (!isUserStream || this.SortMode != ComparerMode.Id)
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
                if (this.IsMuted(Item))
                    return;

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
                            if (SettingCommon.Instance.HideDuplicatedRetweets &&
                                !Item.IsMe &&
                                Item.RetweetedId != null &&
                                this._retweets.TryGetValue(Item.RetweetedId.Value, out status) &&
                                status.RetweetedCount > 0) return;

                            if (BlockIds.Contains(Item.UserId))
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

        public bool IsMuted(PostClass post)
        {
            var muteTab = this.GetTabByType(MyCommon.TabUsageType.Mute);
            if (muteTab != null && muteTab.AddFiltered(post) == MyCommon.HITRESULT.Move)
                return true;

            // これ以降は Twitter 標準のミュート機能に準じた判定

            // Recent以外のツイートと、リプライはミュート対象外
            // 参照: https://support.twitter.com/articles/20171399-muting-users-on-twitter
            if (!string.IsNullOrEmpty(post.RelTabName) || post.IsReply)
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
                            item.SourceUri,
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
                foreach (var post in homeTab.Posts.Values)
                {
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
                return _statuses.TryGetValue(ID, out status)
                    ? status
                    : _retweets.TryGetValue(ID, out status)
                    ? status
                    : _quotes.TryGetValue(ID, out status)
                    ? status
                    : this.GetTabsInnerStorageType()
                          .Where(t => t.Contains(ID))
                          .Select(t => t.Posts[ID]).FirstOrDefault();
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
                    if (tb.TabType == MyCommon.TabUsageType.Mute)
                        continue;

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
                                tbr.Remove(post.StatusId);
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
        private List<PostFilterRule> _filters;
        private List<long> _ids;
        private List<TemporaryId> _tmpIds = new List<TemporaryId>();
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
            TabName = "";
            _filters = new List<PostFilterRule>();
            Protected = false;
            Notify = true;
            SoundFile = "";
            UnreadManage = true;
            _ids = new List<long>();
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
            IEnumerable<long> sortedIds;
            if (this.SortOrder == SortOrder.Ascending)
            {
                switch (this.SortMode)
                {
                    case ComparerMode.Id:
                        sortedIds = this._ids.OrderBy(x => x);
                        break;
                    case ComparerMode.Data:
                        sortedIds = this._ids.OrderBy(x => this.Posts[x].TextFromApi);
                        break;
                    case ComparerMode.Name:
                        sortedIds = this._ids.OrderBy(x => this.Posts[x].ScreenName);
                        break;
                    case ComparerMode.Nickname:
                        sortedIds = this._ids.OrderBy(x => this.Posts[x].Nickname);
                        break;
                    case ComparerMode.Source:
                        sortedIds = this._ids.OrderBy(x => this.Posts[x].Source);
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }
            else
            {
                switch (this.SortMode)
                {
                    case ComparerMode.Id:
                        sortedIds = this._ids.OrderByDescending(x => x);
                        break;
                    case ComparerMode.Data:
                        sortedIds = this._ids.OrderByDescending(x => this.Posts[x].TextFromApi);
                        break;
                    case ComparerMode.Name:
                        sortedIds = this._ids.OrderByDescending(x => this.Posts[x].ScreenName);
                        break;
                    case ComparerMode.Nickname:
                        sortedIds = this._ids.OrderByDescending(x => this.Posts[x].Nickname);
                        break;
                    case ComparerMode.Source:
                        sortedIds = this._ids.OrderByDescending(x => this.Posts[x].Source);
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }

            this._ids = sortedIds.ToList();
        }

        [XmlIgnore]
        public ComparerMode SortMode { get; set; }

        [XmlIgnore]
        public SortOrder SortOrder { get; set; }

        //無条件に追加
        private void Add(long ID, bool Read)
        {
            if (this._ids.Contains(ID)) return;

            if (this.SortMode == ComparerMode.Id)
            {
                if (this.SortOrder == SortOrder.Ascending)
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

            if (!Read)
                this.unreadIds.Add(ID);
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

            if (this.TabType != MyCommon.TabUsageType.Mute &&
                rslt != MyCommon.HITRESULT.None && rslt != MyCommon.HITRESULT.Exclude)
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
            if (!this._ids.Contains(Id))
                return;

            this._ids.Remove(Id);
            this.unreadIds.Remove(Id);

            if (this.IsInnerStorageTabType)
                this._innerPosts.Remove(Id);
        }

        public bool UnreadManage { get; set; }

        // v1.0.5で「タブを固定(Locked)」から「タブを保護(Protected)」に名称変更
        [XmlElement(ElementName = "Locked")]
        public bool Protected { get; set; }

        public bool Notify { get; set; }

        public string SoundFile { get; set; }

        /// <summary>
        /// 最も古い未読ツイートのIDを返します。
        /// ただし、未読がない場合または UnreadManage が false の場合は -1 を返します
        /// </summary>
        [XmlIgnore]
        public long OldestUnreadId
        {
            get
            {
                if (!this.UnreadManage || !SettingCommon.Instance.UnreadManage)
                    return -1L;

                if (this.unreadIds.Count == 0)
                    return -1L;

                return this.unreadIds.Min;
            }
        }

        /// <summary>
        /// 最も古い未読ツイートのインデックス番号を返します。
        /// ただし、未読がない場合または UnreadManage が false の場合は -1 を返します
        /// </summary>
        [XmlIgnore]
        public int OldestUnreadIndex
        {
            get
            {
                var unreadId = this.OldestUnreadId;
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
                throw new ArgumentException("statusId");

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
            _tmpIds.Clear();
            this.unreadIds.Clear();
            _innerPosts.Clear();
        }

        public PostClass this[int Index]
        {
            get
            {
                var id = GetId(Index);
                if (id < 0) throw new ArgumentException("Index can't find. Index=" + Index.ToString() + "/TabName=" + TabName);
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
            MyCommon.TabUsageType.Related;

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
