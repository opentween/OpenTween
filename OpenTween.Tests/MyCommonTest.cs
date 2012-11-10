using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using OpenTween;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace OpenTween
{
    [TestFixture]
    public class MyCommonTest
    {
        [TestCase("http://ja.wikipedia.org/wiki/Wikipedia", Result = "http://ja.wikipedia.org/wiki/Wikipedia")]
        [TestCase("http://ja.wikipedia.org/wiki/メインページ",
            Result = "http://ja.wikipedia.org/wiki/%E3%83%A1%E3%82%A4%E3%83%B3%E3%83%9A%E3%83%BC%E3%82%B8")]
        [TestCase("http://fr.wikipedia.org/wiki/Café", Result = "http://fr.wikipedia.org/wiki/Caf%E9")]
        [TestCase("http://ja.wikipedia.org/wiki/勇気100%", Result = "http://ja.wikipedia.org/wiki/%E5%8B%87%E6%B0%97100%25")]
        [TestCase("http://ja.wikipedia.org/wiki/Bio_100%", Result = "http://ja.wikipedia.org/wiki/Bio_100%25")]
        public string urlEncodeMultibyteCharTest(string uri)
        {
            return MyCommon.urlEncodeMultibyteChar(uri);
        }

        [TestCase("http://日本語.idn.icann.org/", Result = "http://xn--wgv71a119e.idn.icann.org/")]
        [TestCase("http://例え.テスト/", Result = "http://xn--r8jz45g.xn--zckzah/")]
        public string IDNDecodeTest(string uri)
        {
            return MyCommon.IDNDecode(uri);
        }

        [TestCase(new int[] { 1, 2, 3, 4 }, 0, 3, Result = new int[] { 2, 3, 4, 1 })] // 左ローテイト?
        [TestCase(new int[] { 1, 2, 3, 4 }, 3, 0, Result = new int[] { 4, 1, 2, 3 })] // 右ローテイト?
        [TestCase(new int[] { 1, 2, 3, 4, 5 }, 1, 3, Result = new int[] { 1, 3, 4, 2, 5 })]
        [TestCase(new int[] { 1, 2, 3, 4, 5 }, 3, 1, Result = new int[] { 1, 4, 2, 3, 5 })]
        public int[] MoveArrayItemTest(int[] values, int idx_fr, int idx_to)
        {
            // MoveArrayItem は values を直接変更するため複製を用意する
            var copy = new int[values.Length];
            Array.Copy(values, copy, values.Length);

            MyCommon.MoveArrayItem(copy, idx_fr, idx_to);
            return copy;
        }

        [Test]
        public void EncryptStringTest()
        {
            var str = "hogehoge";

            var crypto = MyCommon.EncryptString(str);
            Assert.That(crypto, Is.Not.EqualTo(str));

            var decrypt = MyCommon.DecryptString(crypto);
            Assert.That(decrypt, Is.EqualTo(str));
        }

        [TestCase(new byte[] { 0x01, 0x02 }, 3, Result = new byte[] { 0x01, 0x02, 0x00 })]
        [TestCase(new byte[] { 0x01, 0x02 }, 2, Result = new byte[] { 0x01, 0x02 })]
        [TestCase(new byte[] { 0x01, 0x02 }, 1, Result = new byte[] { 0x03 })]
        public byte[] ResizeBytesArrayTest(byte[] bytes, int size)
        {
            return MyCommon.ResizeBytesArray(bytes, size);
        }

        [TestCase("Resources/re.gif", Result = true)]
        [TestCase("Resources/re1.gif", Result = false)]
        [TestCase("Resources/re1.png", Result = false)]
        public bool IsAnimatedGifTest(string filename)
        {
            return MyCommon.IsAnimatedGif(filename);
        }

        static object[] DateTimeParse_TestCase =
        {
            new object[] {
                "Sun Nov 25 06:10:00 +00:00 2012",
                new DateTime(2012, 11, 25, 6, 10, 0, DateTimeKind.Utc)
            },
            new object[] {
                "Sun, 25 Nov 2012 06:10:00 +00:00",
                new DateTime(2012, 11, 25, 6, 10, 0, DateTimeKind.Utc)
            },
        };
        [TestCaseSource("DateTimeParse_TestCase")]
        public void DateTimeParseTest(string date, DateTime except)
        {
            Assert.That(MyCommon.DateTimeParse(date).ToUniversalTime(), Is.EqualTo(except));
        }

        [DataContract]
        public struct JsonData
        {
            [DataMember(Name = "id")] public string Id { get; set; }
            [DataMember(Name = "body")] public string Body { get; set; }
        }
        static object[] CreateDataFromJson_TestCase =
        {
            new object[] {
                @"{""id"":""1"", ""body"":""hogehoge""}",
                new JsonData { Id = "1", Body = "hogehoge" },
            },
        };
        [TestCaseSource("CreateDataFromJson_TestCase")]
        public void CreateDataFromJsonTest<T>(string json, T expect)
        {
            Assert.That(MyCommon.CreateDataFromJson<T>(json), Is.EqualTo(expect));
        }

        [TestCase("hoge123@example.com", Result = true)]
        [TestCase("hogehoge", Result = false)]
        [TestCase("foo.bar@example.com", Result = true)]
        [TestCase("foo..bar@example.com", Result = false)]
        [TestCase("foobar.@example.com", Result = false)]
        [TestCase("foo+bar@example.com", Result = true)]
        public bool IsValidEmailTest(string email)
        {
            return MyCommon.IsValidEmail(email);
        }

        [TestCase(Keys.Shift, Keys.Shift, Result = true)]
        [TestCase(Keys.Shift, Keys.Control, Result = false)]
        [TestCase(Keys.Control | Keys.Alt, Keys.Control, Result = true)]
        [TestCase(Keys.Control | Keys.Alt, Keys.Alt, Result = true)]
        [TestCase(Keys.Control | Keys.Alt, Keys.Control, Keys.Alt, Result = true)]
        [TestCase(Keys.Control | Keys.Alt, Keys.Shift, Result = false)]
        public bool IsKeyDownTest(Keys modifierKeys, params Keys[] checkKeys)
        {
            return MyCommon._IsKeyDown(modifierKeys, checkKeys);
        }

        [Test]
        public void GetAssemblyNameTest()
        {
            var mockAssembly = Substitute.For<_Assembly>();
            mockAssembly.GetName().Returns(new AssemblyName("OpenTween"));
            MyCommon.EntryAssembly = mockAssembly;

            Assert.That(MyCommon.GetAssemblyName(), Is.EqualTo("OpenTween"));
        }

        [TestCase("", "")]
        [TestCase("%AppName%", "OpenTween")]
        [TestCase("%AppName% %AppName%", "OpenTween OpenTween")]
        public void ReplaceAppNameTest(string str, string except)
        {
            Assert.That(MyCommon.ReplaceAppName(str, "OpenTween"), Is.EqualTo(except));
        }

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

        static object[] GetStatusUrlTest1_TestCase =
        {
            new object[] {
                new PostClass { StatusId = 249493863826350080L, ScreenName = "Favstar_LM", RetweetedId = 0L, RetweetedBy = null },
                "https://twitter.com/Favstar_LM/status/249493863826350080",
            },
            new object[] {
                new PostClass { StatusId = 216033842434289664L, ScreenName = "haru067", RetweetedId = 200245741443235840L, RetweetedBy = "re4k"},
                "https://twitter.com/haru067/status/200245741443235840",
            },
        };
        [TestCaseSource("GetStatusUrlTest1_TestCase")]
        public void GetStatusUrlTest1(PostClass post, string except)
        {
            Assert.That(MyCommon.GetStatusUrl(post), Is.EqualTo(except));
        }

        [TestCase("Favstar_LM", 249493863826350080L, "https://twitter.com/Favstar_LM/status/249493863826350080")]
        [TestCase("haru067", 200245741443235840L, "https://twitter.com/haru067/status/200245741443235840")]
        public void GetStatusUrlTest2(string screenName, long statusId, string except)
        {
            Assert.That(MyCommon.GetStatusUrl(screenName, statusId), Is.EqualTo(except));
        }
    }
}
