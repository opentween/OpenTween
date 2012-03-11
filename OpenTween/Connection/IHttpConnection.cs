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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace OpenTween
{
    public interface IHttpConnection
    {
        HttpStatusCode GetContent(string method,
                Uri requestUri,
                Dictionary<string, string> param,
                ref Stream content,
                string userAgent);

        HttpStatusCode GetContent(string method,
                Uri requestUri,
                Dictionary<string, string> param,
                ref string content,
                Dictionary<string, string> headerInfo,
                CallbackDelegate callback);

        HttpStatusCode GetContent(string method,
                Uri requestUri,
                Dictionary<string, string> param,
                List<KeyValuePair<string, FileInfo>> binary,
                ref string content,
                Dictionary<string, string> headerInfo,
                CallbackDelegate callback);

        void RequestAbort();

        HttpStatusCode Authenticate(Uri url, string username, string password, ref string content);

        string AuthUsername { get; }
        long AuthUserId { get; set; }
    }

    /// <summary>
    /// APIメソッドの処理が終了し呼び出し元へ戻る直前に呼ばれるデリゲート
    /// </summary>
    /// <param name="sender">メソッド名</param>
    /// <param name="code">APIメソッドの返したHTTPステータスコード</param>
    /// <param name="content">APIメソッドの処理結果</param>
    /// <remarks>contentはNothingになることがあるのでチェックを必ず行うこと</remarks>
    public delegate void CallbackDelegate(object sender, ref HttpStatusCode code, ref string content);
}
