// OpenTween - Client of Twitter
// Copyright (c) 2012 the40san <http://sourceforge.jp/users/the40san/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenTween;

namespace OpenTween.Tests
{
    /// <summary>
    /// OpenTween.Foursquareクラステスト用
    /// </summary>
    [TestFixture]
    class FoursquareTest
    {

        [Test]
        public void Test_GetInstance()
        {
            Assert.That(Foursquare.GetInstance, Is.TypeOf(typeof(Foursquare)));
            Assert.That(Foursquare.GetInstance, Is.Not.Null);
        }

        //[TestCase("https://ja.foursquare.com/v/starbucks-coffee-jr%E6%9D%B1%E6%B5%B7-%E5%93%81%E5%B7%9D%E9%A7%85%E5%BA%97/4b5fd527f964a52036ce29e3", Result = "")]
        public string Test_GetMapsUri(string url)
        {
            AppendSettingDialog.Instance.IsPreviewFoursquare = true;
            string refText ="";
            return Foursquare.GetInstance.GetMapsUri(url, ref refText);
        }
        
    }
}
