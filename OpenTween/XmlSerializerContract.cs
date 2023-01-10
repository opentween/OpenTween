// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Xml.Serialization;

#nullable enable

namespace Microsoft.Xml.Serialization.GeneratedAssembly
{
    /// <summary>
    /// <c>OpenTween.XmlSerializer.dll</c> のロードを防ぐための偽の XmlSerializerContract クラス
    /// </summary>
    /// <remarks>
    /// このクラスでは <see cref="CanSerialize(Type)"/> が常に false を返すため、
    /// <see cref="XmlSerializer"/> は実行時にシリアライザを動的に生成します。
    /// </remarks>
    public class XmlSerializerContract : XmlSerializerImplementation
    {
        public override XmlSerializationReader Reader { get; } = new FakeXmlSerializerReader();

        public override XmlSerializationWriter Writer { get; } = new FakeXmlSerializerWriter();

        public override Hashtable ReadMethods { get; } = new();

        public override Hashtable WriteMethods { get; } = new();

        public override Hashtable TypedSerializers { get; } = new();

        public override bool CanSerialize(Type type)
            => false;

        public override XmlSerializer? GetSerializer(Type type)
            => null;
    }

    public class FakeXmlSerializerReader : XmlSerializationReader
    {
        protected override void InitCallbacks()
        {
        }

        protected override void InitIDs()
        {
        }
    }

    public class FakeXmlSerializerWriter : XmlSerializationWriter
    {
        protected override void InitCallbacks()
        {
        }
    }
}
