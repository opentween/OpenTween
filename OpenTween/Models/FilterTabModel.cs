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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Models
{
    public class FilterTabModel : TabModel
    {
        public override MyCommon.TabUsageType TabType
            => MyCommon.TabUsageType.UserDefined;

        public bool FilterModified { get; set; }

        private readonly List<PostFilterRule> filters = new();
        private readonly object lockObjFilters = new();

        public FilterTabModel(string tabName)
            : base(tabName)
        {
        }

        // フィルタに合致したら追加
        public MyCommon.HITRESULT AddFiltered(PostClass post, bool immediately = false)
        {
            if (this.IsInnerStorageTabType)
                return MyCommon.HITRESULT.None;

            var rslt = MyCommon.HITRESULT.None;

            // 全フィルタ評価（優先順位あり）
            lock (this.lockObjFilters)
            {
                foreach (var ft in this.filters)
                {
                    try
                    {
                        switch (ft.ExecFilter(post)) // フィルタクラスでヒット判定
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
                        MyCommon.TraceOut("ExecFilterでNullRef: " + ft);
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
                    this.AddPostQueue(post);
            }

            return rslt; // マーク付けは呼び出し元で行うこと
        }

        public PostFilterRule[] GetFilters()
        {
            lock (this.lockObjFilters)
            {
                return this.filters.ToArray();
            }
        }

        public void RemoveFilter(PostFilterRule filter)
        {
            lock (this.lockObjFilters)
            {
                this.filters.Remove(filter);
                filter.PropertyChanged -= this.OnFilterModified;
                this.FilterModified = true;
            }
        }

        public bool AddFilter(PostFilterRule filter)
        {
            lock (this.lockObjFilters)
            {
                if (this.filters.Contains(filter)) return false;
                filter.PropertyChanged += this.OnFilterModified;
                this.filters.Add(filter);
                this.FilterModified = true;
                return true;
            }
        }

        private void OnFilterModified(object sender, PropertyChangedEventArgs e)
            => this.FilterModified = true;

        public PostFilterRule[] FilterArray
        {
            get
            {
                lock (this.lockObjFilters)
                {
                    return this.filters.ToArray();
                }
            }

            set
            {
                lock (this.lockObjFilters)
                {
                    foreach (var oldFilter in this.filters)
                    {
                        oldFilter.PropertyChanged -= this.OnFilterModified;
                    }

                    this.filters.Clear();
                    this.FilterModified = true;

                    foreach (var newFilter in value)
                    {
                        this.filters.Add(newFilter);
                        newFilter.PropertyChanged += this.OnFilterModified;
                    }
                }
            }
        }

        public override Task RefreshAsync(Twitter tw, bool backward, bool startup, IProgress<string> progress)
        {
            var homeTab = TabInformations.GetInstance().HomeTab;

            return homeTab.RefreshAsync(tw, backward, startup, progress);
        }
    }
}
