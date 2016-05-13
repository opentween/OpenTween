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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTween.Models
{
    public sealed class TabClass
    {
        private List<PostFilterRule> _filters;
        private IndexedSortedSet<long> _ids;
        private ConcurrentQueue<TemporaryId> addQueue = new ConcurrentQueue<TemporaryId>();
        private ConcurrentQueue<long> removeQueue = new ConcurrentQueue<long>();
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

        public PostClass RelationTargetPost { get; set; }

        public long OldestId = long.MaxValue;

        public long SinceId { get; set; }

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

        public TabClass(string name, MyCommon.TabUsageType type) : this(name, type, list: null)
        {
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
                    default:
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

        public ComparerMode SortMode { get; private set; }
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
            if (_innerPosts.ContainsKey(Post.StatusId))
                return;

            if (TabInformations.GetInstance().IsMuted(Post, isHomeTimeline: false))
                return;

            _innerPosts.TryAdd(Post.StatusId, Post);
            this.AddPostQueue(Post.StatusId, Post.IsRead);
        }

        public IList<long> AddSubmit()
        {
            var addedIds = new List<long>();

            TemporaryId tId;
            while (this.addQueue.TryDequeue(out tId))
            {
                this.AddPostImmediately(tId.Id, tId.Read);
                addedIds.Add(tId.Id);
            }

            return addedIds;
        }

        public void EnqueueRemovePost(long statusId, bool setIsDeleted)
        {
            this.removeQueue.Enqueue(statusId);

            if (setIsDeleted && this.IsInnerStorageTabType)
            {
                PostClass post;
                if (this._innerPosts.TryGetValue(statusId, out post))
                    post.IsDeleted = true;
            }
        }

        public bool RemovePostImmediately(long statusId)
        {
            if (!this._ids.Remove(statusId))
                return false;

            this.unreadIds.Remove(statusId);

            if (this.IsInnerStorageTabType)
            {
                PostClass removedPost;
                this._innerPosts.TryRemove(statusId, out removedPost);
            }

            return true;
        }

        public IReadOnlyList<long> RemoveSubmit()
        {
            var removedIds = new List<long>();

            long statusId;
            while (this.removeQueue.TryDequeue(out statusId))
            {
                if (this.RemovePostImmediately(statusId))
                    removedIds.Add(statusId);
            }

            return removedIds;
        }

        public bool UnreadManage { get; set; }
        public bool Protected { get; set; }
        public bool Notify { get; set; }
        public string SoundFile { get; set; }

        /// <summary>
        /// 次に表示する未読ツイートのIDを返します。
        /// ただし、未読がない場合または UnreadManage が false の場合は -1 を返します
        /// </summary>
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
                throw new ArgumentOutOfRangeException(nameof(statusId));

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

        public IEnumerable<int> SearchPostsAll(Func<string, bool> stringComparer)
            => this.SearchPostsAll(stringComparer, reverse: false);

        public IEnumerable<int> SearchPostsAll(Func<string, bool> stringComparer, int startIndex)
            => this.SearchPostsAll(stringComparer, startIndex, reverse: false);

        public IEnumerable<int> SearchPostsAll(Func<string, bool> stringComparer, bool reverse)
        {
            var startIndex = reverse ? this.AllCount - 1 : 0;

            return this.SearchPostsAll(stringComparer, startIndex, reverse: false);
        }

        /// <summary>
        /// タブ内の発言を指定された条件で検索します
        /// </summary>
        /// <param name="stringComparer">発言内容、スクリーン名、名前と比較する条件。マッチしたら true を返す</param>
        /// <param name="startIndex">検索を開始する位置</param>
        /// <param name="reverse">インデックスの昇順に検索する場合は false、降順の場合は true</param>
        /// <returns></returns>
        public IEnumerable<int> SearchPostsAll(Func<string, bool> stringComparer, int startIndex, bool reverse)
        {
            if (this.AllCount == 0)
                yield break;

            var searchIndices = Enumerable.Empty<int>();

            if (!reverse)
            {
                // startindex ...末尾
                if (startIndex != this.AllCount - 1)
                    searchIndices = MyCommon.CountUp(startIndex, this.AllCount - 1);

                // 先頭 ... (startIndex - 1)
                if (startIndex != 0)
                    searchIndices = searchIndices.Concat(MyCommon.CountUp(0, startIndex - 1));
            }
            else
            {
                // startIndex ... 先頭
                if (startIndex != 0)
                    searchIndices = MyCommon.CountDown(startIndex, 0);

                // 末尾 ... (startIndex - 1)
                if (startIndex != this.AllCount - 1)
                    searchIndices = searchIndices.Concat(MyCommon.CountDown(this.AllCount - 1, startIndex - 1));
            }

            foreach (var index in searchIndices)
            {
                PostClass post;
                if (!this.Posts.TryGetValue(this.GetId(index), out post))
                    continue;

                if (stringComparer(post.Nickname) || stringComparer(post.TextFromApi) || stringComparer(post.ScreenName))
                {
                    yield return index;
                }
            }
        }

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
}
