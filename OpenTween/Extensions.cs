// OpenTween - Client of Twitter
// Copyright (c) 2015 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTween
{
    internal static class Extensions
    {
        /// <summary>
        /// WebBrowserで選択中のテキストを取得します
        /// </summary>
        public static string GetSelectedText(this WebBrowser webBrowser)
        {
            dynamic document = webBrowser.Document.DomDocument;
            dynamic textRange = document.selection.createRange();
            string selectedText = textRange.text;

            return selectedText;
        }

        public static ReadLockTransaction BeginReadTransaction(this ReaderWriterLockSlim lockObj)
            => new ReadLockTransaction(lockObj);

        public static WriteLockTransaction BeginWriteTransaction(this ReaderWriterLockSlim lockObj)
            => new WriteLockTransaction(lockObj);

        public static UpgradeableReadLockTransaction BeginUpgradeableReadTransaction(this ReaderWriterLockSlim lockObj)
            => new UpgradeableReadLockTransaction(lockObj);

        /// <summary>
        /// 一方のカルチャがもう一方のカルチャを内包するかを判断します
        /// </summary>
        public static bool Contains(this CultureInfo @this, CultureInfo that)
        {
            if (@this.Equals(that))
                return true;

            // InvariantCulture の親カルチャは InvariantCulture 自身であるため、false になったら打ち切る
            if (!that.Parent.Equals(that))
                return Contains(@this, that.Parent);

            return false;
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }
}
