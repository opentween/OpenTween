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
using System.Runtime.Serialization;
using System.Text;

namespace OpenTween
{
    /// <summary>
    /// Twitter などの API 実行時に発生した例外を扱うクラス
    /// </summary>
    [Serializable]
    public class WebApiException : Exception
    {
        public readonly string ResponseText = null;

        public WebApiException() : base() { }
        public WebApiException(string message) : base(message) { }
        public WebApiException(string message, Exception innerException) : base(message, innerException) { }

        public WebApiException(string message, string responseText)
            : this(message)
        {
            this.ResponseText = responseText;
        }

        public WebApiException(string message, string responseText, Exception innerException)
            : this(message, innerException)
        {
            this.ResponseText = responseText;
        }

        protected WebApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ResponseText = info.GetString("ResponseText");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ResponseText", this.ResponseText);
        }
    }
}
