// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;

namespace OpenTween.Api
{
    public class ApiLimit
    {
        /// <summary>
        /// API 実行回数制限の値
        /// </summary>
        public int AccessLimitCount { get; }

        /// <summary>
        /// API 実行回数制限までの残回数
        /// </summary>
        public int AccessLimitRemain { get; }

        /// <summary>
        /// API 実行回数制限がリセットされる日時
        /// </summary>
        public DateTime AccessLimitResetDate { get; }

        /// <summary>
        /// API 実行回数制限値を取得した日時
        /// </summary>
        public DateTime UpdatedAt { get; }

        public ApiLimit(int limitCount, int limitRemain, DateTime resetDate)
            : this(limitCount, limitRemain, resetDate, DateTime.Now)
        {
        }

        public ApiLimit(int limitCount, int limitRemain, DateTime resetDate, DateTime updatedAt)
        {
            this.AccessLimitCount = limitCount;
            this.AccessLimitRemain = limitRemain;
            this.AccessLimitResetDate = resetDate;
            this.UpdatedAt = updatedAt;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ApiLimit);
        }

        public bool Equals(ApiLimit obj)
        {
            return obj != null &&
                this.AccessLimitCount == obj.AccessLimitCount &&
                this.AccessLimitRemain == obj.AccessLimitRemain &&
                this.AccessLimitResetDate == obj.AccessLimitResetDate;
        }

        public override int GetHashCode()
        {
            return this.AccessLimitCount ^ this.AccessLimitRemain ^ this.AccessLimitResetDate.GetHashCode();
        }
    }
}
