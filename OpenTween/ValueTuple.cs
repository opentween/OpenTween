// OpenTween - Client of Twitter
// Copyright (c) 2017 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ValueTuple が .NET Framework に追加されるまでの繋ぎのための定義

namespace System
{
    internal struct ValueTuple<T1, T2> : IStructuralEquatable
    {
        public T1 Item1;
        public T2 Item2;

        public ValueTuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other is ValueTuple<T1, T2> otherTuple)
            {
                return comparer.Equals(this.Item1, otherTuple.Item1)
                    && comparer.Equals(this.Item2, otherTuple.Item2);
            }

            return false;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
            => comparer.GetHashCode(this.Item1) ^ comparer.GetHashCode(this.Item2);
    }
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Class | AttributeTargets.Struct)]
    internal class TupleElementNamesAttribute : Attribute
    {
        public IList<string> TransformNames { get; }

        public TupleElementNamesAttribute(string[] names)
            => this.TransformNames = names;
    }
}