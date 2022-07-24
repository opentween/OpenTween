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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Models
{
    public abstract class InternalStorageTabModel : TabModel
    {
        protected readonly ConcurrentDictionary<long, PostClass> internalPosts = new();

        public override ConcurrentDictionary<long, PostClass> Posts
            => this.internalPosts;

        protected InternalStorageTabModel(string tabName)
            : base(tabName)
        {
        }

        public override void AddPostQueue(PostClass post)
        {
            if (TabInformations.GetInstance().IsMuted(post, isHomeTimeline: false))
                return;

            this.internalPosts.TryAdd(post.StatusId, post);

            base.AddPostQueue(post);
        }

        public override void EnqueueRemovePost(long statusId, bool setIsDeleted)
        {
            base.EnqueueRemovePost(statusId, setIsDeleted);

            if (setIsDeleted)
            {
                if (this.internalPosts.TryGetValue(statusId, out var post))
                    post.IsDeleted = true;
            }
        }

        public override bool RemovePostImmediately(long statusId)
        {
            if (!base.RemovePostImmediately(statusId))
                return false;

            this.internalPosts.TryRemove(statusId, out _);

            return true;
        }

        public override void ClearIDs()
        {
            base.ClearIDs();
            this.internalPosts.Clear();
        }

        internal override bool SetReadState(long statusId, bool read)
        {
            if (this.Posts.TryGetValue(statusId, out var post))
                post.IsRead = read;

            return base.SetReadState(statusId, read);
        }
    }
}
