// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using Moq;
using OpenTween;
using OpenTween.Models;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class MyCommonTest
    {
        [Theory]
        [InlineData("http://日本語.idn.icann.org/", "http://xn--wgv71a119e.idn.icann.org/")]
        [InlineData("http://例え.テスト/", "http://xn--r8jz45g.xn--zckzah/")]
        public void IDNEncodeTest(string uri, string expected)
            => Assert.Equal(expected, MyCommon.IDNEncode(uri));

        [Theory]
        [InlineData("http://xn--wgv71a119e.idn.icann.org/", "http://日本語.idn.icann.org/")]
        [InlineData("http://xn--r8jz45g.xn--zckzah/", "http://例え.テスト/")]
        [InlineData("http://xn--a/", "http://xn--a/")] // 不正なpunycode
        public void IDNDecodeTest(string uri, string expected)
            => Assert.Equal(expected, MyCommon.IDNDecode(uri));

        [Theory]
        [InlineData("http://xn--r8jz45g.xn--zckzah/", "http://例え.テスト/")]
        [InlineData("http://ja.wikipedia.org/wiki/%3F", "http://ja.wikipedia.org/wiki/%3F")] // "?" に変換しない
        [InlineData("http://ja.wikipedia.org/wiki/%E3%83%9E%E3%82%B8LOVE1000%25",
            "http://ja.wikipedia.org/wiki/マジLOVE1000%25")] // "%" も変換しない
        [InlineData("http://xn--a/%E3%81%82", "http://xn--a/あ")] // 不正なpunycode
        [InlineData("http://example..com/", "http://example..com/")] // 不正なURL
        [InlineData("http://example.com/%E3%81%82%FF", "http://example.com/あ%FF")] // 不正なUTF-8シーケンス
        [InlineData("http://example.com/%E3%81%82%ED%A0%80", "http://example.com/あ%ED%A0%80")] // 不正なUTF-8シーケンス (high surrogate)
        public void ConvertToReadableUrl(string url, string expected)
            => Assert.Equal(expected, MyCommon.ConvertToReadableUrl(url));

        [Theory]
        [InlineData(new int[] { 1, 2, 3, 4 }, 0, 3, new int[] { 2, 3, 4, 1 })] // 左ローテイト?
        [InlineData(new int[] { 1, 2, 3, 4 }, 3, 0, new int[] { 4, 1, 2, 3 })] // 右ローテイト?
        [InlineData(new int[] { 1, 2, 3, 4, 5 }, 1, 3, new int[] { 1, 3, 4, 2, 5 })]
        [InlineData(new int[] { 1, 2, 3, 4, 5 }, 3, 1, new int[] { 1, 4, 2, 3, 5 })]
        public void MoveArrayItemTest(int[] values, int idx_fr, int idx_to, int[] expected)
        {
            // MoveArrayItem は values を直接変更するため複製を用意する
            var copy = new int[values.Length];
            Array.Copy(values, copy, values.Length);

            MyCommon.MoveArrayItem(copy, idx_fr, idx_to);
            Assert.Equal(expected, copy);
        }

        [Fact]
        public void EncryptStringTest()
        {
            var str = "hogehoge";

            var crypto = MyCommon.EncryptString(str);
            Assert.NotEqual(str, crypto);

            var decrypt = MyCommon.DecryptString(crypto);
            Assert.Equal(str, decrypt);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x02 }, 3, new byte[] { 0x01, 0x02, 0x00 })]
        [InlineData(new byte[] { 0x01, 0x02 }, 2, new byte[] { 0x01, 0x02 })]
        [InlineData(new byte[] { 0x01, 0x02 }, 1, new byte[] { 0x03 })]
        public void ResizeBytesArrayTest(byte[] bytes, int size, byte[] expected)
            => Assert.Equal(expected, MyCommon.ResizeBytesArray(bytes, size));

        [Theory]
        [InlineData("Resources/re.gif", true)]
        [InlineData("Resources/re1.gif", false)]
        [InlineData("Resources/re1.png", false)]
        public void IsAnimatedGifTest(string filename, bool expected)
            => Assert.Equal(expected, MyCommon.IsAnimatedGif(filename));

        public static readonly TheoryData<string, DateTimeUtc> DateTimeParseTestCase = new()
        {
            { "Sun Nov 25 06:10:00 +00:00 2012", new DateTimeUtc(2012, 11, 25, 6, 10, 0) },
            { "Sun, 25 Nov 2012 06:10:00 +00:00", new DateTimeUtc(2012, 11, 25, 6, 10, 0) },
        };

        [Theory]
        [MemberData(nameof(DateTimeParseTestCase))]
        public void DateTimeParseTest(string date, DateTimeUtc excepted)
            => Assert.Equal(excepted, MyCommon.DateTimeParse(date));

        [DataContract]
        public struct JsonData
        {
            [DataMember(Name = "id")]
            public string Id { get; set; }

            [DataMember(Name = "body")]
            public string Body { get; set; }
        }

        public static readonly TheoryData<string, JsonData> CreateDataFromJsonTestCase = new()
        {
            {
                @"{""id"":""1"", ""body"":""hogehoge""}",
                new JsonData { Id = "1", Body = "hogehoge" }
            },
        };

        [Theory]
        [MemberData(nameof(CreateDataFromJsonTestCase))]
        public void CreateDataFromJsonTest<T>(string json, T expected)
            => Assert.Equal(expected, MyCommon.CreateDataFromJson<T>(json));

        [Theory]
        [InlineData("hoge123@example.com", true)]
        [InlineData("hogehoge", false)]
        [InlineData("foo.bar@example.com", true)]
        [InlineData("foo..bar@example.com", false)]
        [InlineData("foobar.@example.com", false)]
        [InlineData("foo+bar@example.com", true)]
        public void IsValidEmailTest(string email, bool expected)
            => Assert.Equal(expected, MyCommon.IsValidEmail(email));

        [Theory]
        [InlineData(Keys.Shift, new[] { Keys.Shift }, true)]
        [InlineData(Keys.Shift, new[] { Keys.Control }, false)]
        [InlineData(Keys.Control | Keys.Alt, new[] { Keys.Control }, true)]
        [InlineData(Keys.Control | Keys.Alt, new[] { Keys.Alt }, true)]
        [InlineData(Keys.Control | Keys.Alt, new[] { Keys.Control, Keys.Alt }, true)]
        [InlineData(Keys.Control | Keys.Alt, new[] { Keys.Shift }, false)]
        public void IsKeyDownTest(Keys modifierKeys, Keys[] checkKeys, bool expected)
            => Assert.Equal(expected, MyCommon.IsKeyDownInternal(modifierKeys, checkKeys));

        [Fact]
        public void GetAssemblyNameTest()
        {
            var mockAssembly = new Mock<_Assembly>();
            mockAssembly.Setup(m => m.GetName()).Returns(new AssemblyName("OpenTween"));
            MyCommon.EntryAssembly = mockAssembly.Object;

            Assert.Equal("OpenTween", MyCommon.GetAssemblyName());
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("%AppName%", "OpenTween")]
        [InlineData("%AppName% %AppName%", "OpenTween OpenTween")]
        public void ReplaceAppNameTest(string str, string excepted)
            => Assert.Equal(excepted, MyCommon.ReplaceAppName(str, "OpenTween"));

        [Theory]
        [InlineData("1.0.0.0", "1.0.0")]
        [InlineData("1.0.0.1", "1.0.1-dev")]
        [InlineData("1.0.0.12", "1.0.1-dev+build.12")]
        [InlineData("1.0.1.0", "1.0.1")]
        [InlineData("1.0.9.1", "1.0.10-dev")]
        [InlineData("1.1.0.0", "1.1.0")]
        [InlineData("1.9.9.1", "1.9.10-dev")]
        public void GetReadableVersionTest(string fileVersion, string expected)
            => Assert.Equal(expected, MyCommon.GetReadableVersion(fileVersion));

        public static readonly TheoryData<PostClass, string> GetStatusUrlTest1TestCase = new()
        {
            {
                new PostClass { StatusId = 249493863826350080L, ScreenName = "Favstar_LM", RetweetedId = null, RetweetedBy = null },
                "https://twitter.com/Favstar_LM/status/249493863826350080"
            },
            {
                new PostClass { StatusId = 216033842434289664L, ScreenName = "haru067", RetweetedId = 200245741443235840L, RetweetedBy = "re4k" },
                "https://twitter.com/haru067/status/200245741443235840"
            },
        };

        [Theory]
        [MemberData(nameof(GetStatusUrlTest1TestCase))]
        public void GetStatusUrlTest1(PostClass post, string expected)
            => Assert.Equal(expected, MyCommon.GetStatusUrl(post));

        [Theory]
        [InlineData("Favstar_LM", 249493863826350080L, "https://twitter.com/Favstar_LM/status/249493863826350080")]
        [InlineData("haru067", 200245741443235840L, "https://twitter.com/haru067/status/200245741443235840")]
        public void GetStatusUrlTest2(string screenName, long statusId, string expected)
            => Assert.Equal(expected, MyCommon.GetStatusUrl(screenName, statusId));

        [Fact]
        public void GetErrorLogPathTest()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var mockAssembly = new Mock<_Assembly>();
                mockAssembly.Setup(m => m.Location).Returns(@"C:\hogehoge\OpenTween\OpenTween.exe");
                MyCommon.EntryAssembly = mockAssembly.Object;

                Assert.Equal(@"C:\hogehoge\OpenTween\ErrorLogs", MyCommon.GetErrorLogPath());
            }
            else
            {
                var mockAssembly = new Mock<_Assembly>();
                mockAssembly.Setup(m => m.Location).Returns(@"/hogehoge/OpenTween/OpenTween.exe");
                MyCommon.EntryAssembly = mockAssembly.Object;

                Assert.Equal(@"/hogehoge/OpenTween/ErrorLogs", MyCommon.GetErrorLogPath());
            }
        }

        [Fact]
        public void CountUp_Test()
        {
            var actual = MyCommon.CountUp(from: 1, to: 5);

            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, actual);
        }

        [Fact]
        public void CountUp_FromAndToAreEqualTest()
        {
            var actual = MyCommon.CountUp(from: 1, to: 1);

            Assert.Equal(new[] { 1 }, actual);
        }

        [Fact]
        public void CountUp_ToIsLessThanFromTest()
        {
            var actual = MyCommon.CountUp(from: 1, to: 0);

            Assert.Empty(actual);
        }

        [Fact]
        public void CountDown_Test()
        {
            var actual = MyCommon.CountDown(from: 5, to: 1);

            Assert.Equal(new[] { 5, 4, 3, 2, 1 }, actual);
        }

        [Fact]
        public void CountDown_FromAndToAreEqualTest()
        {
            var actual = MyCommon.CountDown(from: 5, to: 5);

            Assert.Equal(new[] { 5 }, actual);
        }

        [Fact]
        public void CountDown_ToIsGreaterThanFromTest()
        {
            var actual = MyCommon.CountDown(from: 5, to: 6);

            Assert.Empty(actual);
        }

        [Fact]
        public void CircularCountUp_Test()
        {
            var actual = MyCommon.CircularCountUp(length: 6, startIndex: 3);

            Assert.Equal(new[] { 3, 4, 5, 0, 1, 2 }, actual);
        }

        [Fact]
        public void CircularCountUp_StartFromZeroTest()
        {
            var actual = MyCommon.CircularCountUp(length: 6, startIndex: 0);

            Assert.Equal(new[] { 0, 1, 2, 3, 4, 5 }, actual);
        }

        [Fact]
        public void CircularCountDown_Test()
        {
            var actual = MyCommon.CircularCountDown(length: 6, startIndex: 3);

            Assert.Equal(new[] { 3, 2, 1, 0, 5, 4 }, actual);
        }

        [Fact]
        public void CircularCountDown_StartFromLastIndexTest()
        {
            var actual = MyCommon.CircularCountDown(length: 6, startIndex: 5);

            Assert.Equal(new[] { 5, 4, 3, 2, 1, 0 }, actual);
        }

        [Fact]
        public void CreateBrowserProcessStartInfo_DefaultBrowserTest()
        {
            var startInfo = MyCommon.CreateBrowserProcessStartInfo(browserPathWithArgs: null, "https://example.com/");
            Assert.Equal("https://example.com/", startInfo.FileName);
            Assert.Equal("", startInfo.Arguments);
            Assert.True(startInfo.UseShellExecute);
        }

        [Fact]
        public void CreateBrowserProcessStartInfo_BrowserPathTest()
        {
            var startInfo = MyCommon.CreateBrowserProcessStartInfo("C:\\browser.exe", "https://example.com/");
            Assert.Equal("C:\\browser.exe", startInfo.FileName);
            Assert.Equal("\"https://example.com/\"", startInfo.Arguments);
            Assert.False(startInfo.UseShellExecute);
        }

        [Fact]
        public void CreateBrowserProcessStartInfo_BrowserPathWithSpacesTest()
        {
            var startInfo = MyCommon.CreateBrowserProcessStartInfo("C:\\Program Files\\browser.exe", "https://example.com/");
            Assert.Equal("C:\\Program Files\\browser.exe", startInfo.FileName);
            Assert.Equal("\"https://example.com/\"", startInfo.Arguments);
            Assert.False(startInfo.UseShellExecute);
        }

        [Fact]
        public void CreateBrowserProcessStartInfo_QuotedBrowserPathTest()
        {
            var startInfo = MyCommon.CreateBrowserProcessStartInfo("\"C:\\Program Files\\browser.exe\"", "https://example.com/");
            Assert.Equal("C:\\Program Files\\browser.exe", startInfo.FileName);
            Assert.Equal("\"https://example.com/\"", startInfo.Arguments);
            Assert.False(startInfo.UseShellExecute);
        }

        [Fact]
        public void CreateBrowserProcessStartInfo_QuotedBrowserPathWithArgsTest()
        {
            var startInfo = MyCommon.CreateBrowserProcessStartInfo("\"C:\\Program Files\\browser.exe\" /hoge", "https://example.com/");
            Assert.Equal("C:\\Program Files\\browser.exe", startInfo.FileName);
            Assert.Equal("/hoge \"https://example.com/\"", startInfo.Arguments);
            Assert.False(startInfo.UseShellExecute);
        }

        public static readonly TheoryData<int[], (int, int)[]> ToRangeChunkTestCase = new()
        {
            {
                new[] { 1 },
                new[] { (1, 1) }
            },
            {
                new[] { 1, 2 },
                new[] { (1, 2) }
            },
            {
                new[] { 1, 3 },
                new[] { (1, 1), (3, 3) }
            },
        };

        [Theory]
        [MemberData(nameof(ToRangeChunkTestCase))]
        public void ToRangeChunk_Test(int[] values, (int Start, int End)[] expected)
        {
            Assert.Equal(expected, MyCommon.ToRangeChunk(values));
        }
    }
}
