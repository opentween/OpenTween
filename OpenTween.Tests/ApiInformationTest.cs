// OpenTween - Client of Twitter
// Copyright (c) 2012      the40san <http://sourceforge.jp/users/the40san/>
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
using System.Threading.Tasks;
using NUnit.Framework;


namespace OpenTween
{
    [TestFixture]
    public class ApiInformationTest
    {
        [Test]
        public void Initialize()
        {
            ApiInformation apiinfo = new ApiInformation();
            apiinfo.Initialize();

            Assert.That(apiinfo.HttpHeaders["X-RateLimit-Remaining"], Is.EqualTo("-1"));
            Assert.That(apiinfo.HttpHeaders["X-RateLimit-Limit"], Is.EqualTo("-1"));
            Assert.That(apiinfo.HttpHeaders["X-RateLimit-Reset"], Is.EqualTo("-1"));

            Assert.That(apiinfo.HttpHeaders["X-Access-Level"], Is.EqualTo("read-write-directmessages"));

            Assert.That(apiinfo.HttpHeaders["X-MediaRateLimit-Remaining"], Is.EqualTo("-1"));
            Assert.That(apiinfo.HttpHeaders["X-MediaRateLimit-Limit"], Is.EqualTo("-1"));
            Assert.That(apiinfo.HttpHeaders["X-MediaRateLimit-Reset"], Is.EqualTo("-1"));
            

        }

        [Test]
        [Combinatorial] 
        public void Test_MaxCount([Values(100, 0, -100)]int value)
        {
            ApiInformation apiinfo = new ApiInformation();
            apiinfo.MaxCount = value;
            Assert.That(apiinfo.MaxCount, Is.EqualTo(value));
        }


        [TestCase(-100, Result = -100)]
        [TestCase(0, Result = 0)]
        [TestCase(100, Result = 100)]
        [TestCase(int.MaxValue, Result = int.MaxValue)]
        [TestCase(int.MinValue, Result = int.MinValue)]
        public int Test_RemainCount(int value)
        {
            ApiInformation apiinfo = new ApiInformation();
            apiinfo.RemainCount = value;
            return apiinfo.RemainCount;
        }





        [TestCase(-100, Result = -100)]
        [TestCase(0, Result = 0)]
        [TestCase(100, Result = 100)]
        [TestCase(int.MaxValue, Result = int.MaxValue)]
        [TestCase(int.MinValue, Result = int.MinValue)]
        public int Test_MediaMaxCount(int value)
        {
            ApiInformation apiinfo = new ApiInformation();
            apiinfo.MediaMaxCount = value;
            return apiinfo.MediaMaxCount;
        }

        [TestCase(-100, Result = -100)]
        [TestCase(0, Result = 0)]
        [TestCase(100, Result = 100)]
        [TestCase(int.MaxValue, Result = int.MaxValue)]
        [TestCase(int.MinValue, Result = int.MinValue)]
        public int Test_ResetTimeInSeconds(int value)
        {
            ApiInformation apiinfo = new ApiInformation();
            apiinfo.ResetTimeInSeconds = value;
            return apiinfo.ResetTimeInSeconds;
        }

        [TestCase(-100, Result = -100)]
        [TestCase(0, Result = 0)]
        [TestCase(100, Result = 100)]
        [TestCase(int.MaxValue, Result = int.MaxValue)]
        [TestCase(int.MinValue, Result = int.MinValue)]
        public int Test_UsingCount(int value)
        {
            ApiInformation apiinfo = new ApiInformation();
            apiinfo.UsingCount = value;
            return apiinfo.UsingCount;
        }

        //↓以下DateTime系

        [Test]
        public void Test_ConvertResetTimeInSecondsToResetTime()
        {
            ApiInformation apiinfo = new ApiInformation();
            DateTime d = apiinfo.ConvertResetTimeInSecondsToResetTime(-1);
            Assert.That(d, Is.EqualTo(new DateTime()));
        }

        [Test]
        public void Test_MediaResetTime()
        {
            ApiInformation apiinfo = new ApiInformation();
            DateTime d = new DateTime(1970, 1, 1, 0, 0, 0);
            apiinfo.MediaResetTime = d;
            Assert.That(apiinfo.MediaResetTime, Is.EqualTo(d));
        }


    }
}
