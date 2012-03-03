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

using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenTween
{
    public class ListElement
    {
        public long Id = 0;
        public string Name = "";
        public string Description = "";
        public string Slug = "";
        public bool IsPublic = true;
        public int SubscriberCount = 0;   //購読者数
        public int MemberCount = 0;   //リストメンバ数
        public long UserId = 0;
        public string Username = "";
        public string Nickname = "";

        protected dynamic _tw;

        private List<UserInfo> _members = null;
        private long _cursor = -1;

        public ListElement()
        {
        }

        public ListElement(TwitterDataModel.ListElementData listElementData, dynamic tw)
        {
            this.Description = listElementData.Description;
            this.Id = listElementData.Id;
            this.IsPublic = (listElementData.Mode == "public");
            this.MemberCount = listElementData.MemberCount;
            this.Name = listElementData.Name;
            this.SubscriberCount = listElementData.SubscriberCount;
            this.Slug = listElementData.Slug;
            this.Nickname = listElementData.User.Name.Trim();
            this.Username = listElementData.User.ScreenName;
            this.UserId = listElementData.User.Id;

            this._tw = tw;
        }

        public virtual string Refresh()
        {
            ListElement t = this;
            return _tw.EditList(this.Id.ToString(), Name, !this.IsPublic, this.Description, ref t);
        }

        [XmlIgnore]
        public List<UserInfo> Members
        {
            get
            {
                if (this._members == null) this._members = new List<UserInfo>();
                return this._members;
            }
        }

        [XmlIgnore]
        public long Cursor
        {
            get
            {
                return _cursor;
            }
        }

        public string RefreshMembers()
        {
            var users = new List<UserInfo>();
            _cursor = -1;
            var result = this._tw.GetListMembers(this.Id.ToString(), users, ref _cursor);
            this._members = users;
            return string.IsNullOrEmpty(result) ? this.ToString() : result;
        }

        public string GetMoreMembers()
        {
            var result = this._tw.GetListMembers(this.Id.ToString(), this._members, ref _cursor);
            return string.IsNullOrEmpty(result) ? this.ToString() : result;
        }

        public override string ToString()
        {
            return "@" + Username + "/" + Name + " [" + (this.IsPublic ? "public" : "Protected") + "]";
        }
    }
}
