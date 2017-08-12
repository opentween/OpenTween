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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Setting;

namespace OpenTween.Models
{
    public abstract class TabModel
    {
        public string TabName { get; set; }

        public bool UnreadManage { get; set; } = true;
        public bool Protected { get; set; }
        public bool Notify { get; set; } = true;
        public string SoundFile { get; set; } = "";

        public ComparerMode SortMode { get; private set; }
        public SortOrder SortOrder { get; private set; }

        public long OldestId { get; set; } = long.MaxValue;
        public long SinceId { get; set; }

        public abstract MyCommon.TabUsageType TabType { get; }

        public virtual ConcurrentDictionary<long, PostClass> Posts
            => TabInformations.GetInstance().Posts;

        public int AllCount => this._ids.Count;
        public long[] StatusIds => this._ids.ToArray();

        public bool IsDefaultTabType => this.TabType.IsDefault();
        public bool IsDistributableTabType => this.TabType.IsDistributable();
        public bool IsInnerStorageTabType => this.TabType.IsInnerStorage();

        /// <summary>
        /// 次回起動時にも保持されるタブか（SettingTabsに保存されるか）
        /// </summary>
        public virtual bool IsPermanentTabType => true;

        private IndexedSortedSet<long> _ids = new IndexedSortedSet<long>();
        private ConcurrentQueue<TemporaryId> addQueue = new ConcurrentQueue<TemporaryId>();
        private ConcurrentQueue<long> removeQueue = new ConcurrentQueue<long>();
        private SortedSet<long> unreadIds = new SortedSet<long>();

        private readonly object _lockObj = new object();

        protected TabModel(string tabName)
        {
            this.TabName = tabName;
        }

        public abstract Task RefreshAsync(Twitter tw, bool backward, bool startup, IProgress<string> progress);

        private struct TemporaryId
        {
            public long StatusId { get; }
            public bool Read { get; }

            public TemporaryId(long statusId, bool read)
            {
                this.StatusId = statusId;
                this.Read = read;
            }
        }

        public virtual void AddPostQueue(PostClass post)
        {
            if (!this.Posts.ContainsKey(post.StatusId))
                throw new ArgumentException("Specified post not exists in storage", nameof(post));

            this.addQueue.Enqueue(new TemporaryId(post.StatusId, post.IsRead));
        }

        //無条件に追加
        internal bool AddPostImmediately(long statusId, bool read)
        {
            if (!this._ids.Add(statusId))
                return false;

            if (!read)
                this.unreadIds.Add(statusId);

            return true;
        }

        public IReadOnlyList<long> AddSubmit()
        {
            var addedIds = new List<long>();

            while (this.addQueue.TryDequeue(out var tId))
            {
                if (this.AddPostImmediately(tId.StatusId, tId.Read))
                    addedIds.Add(tId.StatusId);
            }

            return addedIds;
        }

        public virtual void EnqueueRemovePost(long statusId, bool setIsDeleted)
        {
            this.removeQueue.Enqueue(statusId);
        }

        public virtual bool RemovePostImmediately(long statusId)
        {
            if (!this._ids.Remove(statusId))
                return false;

            this.unreadIds.Remove(statusId);
            return true;
        }

        public IReadOnlyList<long> RemoveSubmit()
        {
            var removedIds = new List<long>();

            while (this.removeQueue.TryDequeue(out var statusId))
            {
                if (this.RemovePostImmediately(statusId))
                    removedIds.Add(statusId);
            }

            return removedIds;
        }

        public virtual void ClearIDs()
        {
            this._ids.Clear();
            this.unreadIds.Clear();

            Interlocked.Exchange(ref this.addQueue, new ConcurrentQueue<TemporaryId>());
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
                    this.Posts.TryGetValue(x, out var xPost);
                    this.Posts.TryGetValue(y, out var yPost);

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

        /// <summary>
        /// 次に表示する未読ツイートのIDを返します。
        /// ただし、未読がない場合または UnreadManage が false の場合は -1 を返します
        /// </summary>
        public long NextUnreadId
        {
            get
            {
                if (!this.UnreadManage || !SettingManager.Common.UnreadManage)
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
                if (!this.UnreadManage || !SettingManager.Common.UnreadManage)
                    return 0;

                return this.unreadIds.Count;
            }
        }

        /// <summary>
        /// 未読ツイートの ID を配列で返します
        /// </summary>
        public long[] GetUnreadIds()
        {
            lock (this._lockObj)
                return this.unreadIds.ToArray();
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

        public bool Contains(long statusId)
            => this._ids.Contains(statusId);

        public PostClass this[int index]
        {
            get
            {
                if (!this.Posts.TryGetValue(this.GetStatusIdAt(index), out var post))
                    throw new ArgumentOutOfRangeException(nameof(index), "Post not exists");

                return post;
            }
        }

        public PostClass[] this[int startIndex, int endIndex]
        {
            get
            {
                if (startIndex < 0)
                    throw new ArgumentOutOfRangeException(nameof(startIndex));
                if (endIndex >= this.AllCount)
                    throw new ArgumentOutOfRangeException(nameof(endIndex));
                if (startIndex > endIndex)
                    throw new ArgumentException($"{nameof(startIndex)} must be equal to or less than {nameof(endIndex)}.", nameof(startIndex));

                var length = endIndex - startIndex + 1;
                var posts = new PostClass[length];

                var i = 0;
                foreach (var idx in Enumerable.Range(startIndex, length))
                {
                    var statusId = this.GetStatusIdAt(idx);
                    this.Posts.TryGetValue(statusId, out posts[i++]);
                }

                return posts;
            }
        }

        public long[] GetStatusIdAt(IEnumerable<int> indexes)
            => indexes.Select(x => this.GetStatusIdAt(x)).ToArray();

        public long GetStatusIdAt(int index)
            => this._ids[index];

        public int[] IndexOf(long[] statusIds)
        {
            if (statusIds == null)
                throw new ArgumentNullException(nameof(statusIds));

            return statusIds.Select(x => this.IndexOf(x)).ToArray();
        }

        public int IndexOf(long statusId)
            => this._ids.IndexOf(statusId);

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

            IEnumerable<int> searchIndices;

            if (!reverse)
                searchIndices = MyCommon.CircularCountUp(this.AllCount, startIndex);
            else
                searchIndices = MyCommon.CircularCountDown(this.AllCount, startIndex);

            foreach (var index in searchIndices)
            {
                if (!this.Posts.TryGetValue(this.GetStatusIdAt(index), out var post))
                    continue;

                if (stringComparer(post.Nickname) || stringComparer(post.TextFromApi) || stringComparer(post.ScreenName))
                {
                    yield return index;
                }
            }
        }
    }
}
