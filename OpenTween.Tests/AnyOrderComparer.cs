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

namespace OpenTween
{
    /// <summary>
    /// 順不定なコレクションの比較を行います
    /// </summary>
    internal class AnyOrderComparer<T> : IEqualityComparer<IEnumerable<T>>
        where T : IEquatable<T>
    {
        public static readonly AnyOrderComparer<T> Instance = new();

        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            var xList = new LinkedList<T>(x);

            foreach (var item in y)
            {
                var node = xList.Find(item);
                if (node == null)
                    return false;

                xList.Remove(node);
            }

            return xList.Count == 0;
        }

        public int GetHashCode(IEnumerable<T> obj)
            => throw new NotImplementedException();
    }
}
