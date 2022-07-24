// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenTween.Api.DataModel;

namespace OpenTween
{
    public class ListElement
    {
        public long Id = 0;
        public string Name = "";
        public string Description = "";
        public string Slug = "";
        public bool IsPublic = true;
        public int SubscriberCount = 0;   // 購読者数
        public int MemberCount = 0;   // リストメンバ数
        public long UserId = 0;
        public string Username = "";
        public string Nickname = "";

        protected Twitter tw = null!;

        private List<UserInfo> members = new();

        [XmlIgnore]
        public long Cursor { get; private set; } = -1;

        public ListElement()
        {
        }

        public ListElement(TwitterList listElementData, Twitter tw)
        {
            this.Description = listElementData.Description;
            this.Id = listElementData.Id;
            this.IsPublic = listElementData.Mode == "public";
            this.MemberCount = listElementData.MemberCount;
            this.Name = listElementData.Name;
            this.SubscriberCount = listElementData.SubscriberCount;
            this.Slug = listElementData.Slug;
            this.Nickname = listElementData.User.Name.Trim();
            this.Username = listElementData.User.ScreenName;
            this.UserId = listElementData.User.Id;

            this.tw = tw;
        }

        public virtual async Task Refresh()
        {
            var newList = await this.tw.EditList(this.Id, this.Name, !this.IsPublic, this.Description)
                .ConfigureAwait(false);

            this.Description = newList.Description;
            this.Id = newList.Id;
            this.IsPublic = newList.IsPublic;
            this.MemberCount = newList.MemberCount;
            this.Name = newList.Name;
            this.SubscriberCount = newList.SubscriberCount;
            this.Slug = newList.Slug;
            this.Nickname = newList.Nickname;
            this.Username = newList.Username;
            this.UserId = newList.UserId;
        }

        [XmlIgnore]
        public List<UserInfo> Members
            => this.members;

        public async Task RefreshMembers()
        {
            var users = new List<UserInfo>();
            this.Cursor = await this.tw.GetListMembers(this.Id, users, cursor: -1)
                .ConfigureAwait(false);
            this.members = users;
        }

        public async Task GetMoreMembers()
            => this.Cursor = await this.tw.GetListMembers(this.Id, this.members, this.Cursor)
                .ConfigureAwait(false);

        public override string ToString()
            => $"@{this.Username}/{this.Name} [{(this.IsPublic ? "public" : "Protected")}]";
    }
}
