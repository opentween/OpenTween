using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenTween;

namespace OpenTween
{
    [TestFixture]
    public class MyCommonTest
    {
        [TestCase("1.0.0.0", "1.0.0")]
        [TestCase("1.0.0.1", "1.0.1-beta1")]
        [TestCase("1.0.0.9", "1.0.1-beta9")]
        [TestCase("1.0.1.0", "1.0.1")]
        [TestCase("1.0.9.1", "1.1.0-beta1")]
        [TestCase("1.1.0.0", "1.1.0")]
        [TestCase("1.9.9.1", "2.0.0-beta1")]
        public void GetReadableVersionTest(string fileVersion, string expect)
        {
            Assert.That(OpenTween.MyCommon.GetReadableVersion(fileVersion), Is.EqualTo(expect));
        }
    }
}
