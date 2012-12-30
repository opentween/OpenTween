using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Xml.Linq;

namespace OpenTween.Thumbnail.Services
{
    [TestFixture]
    class TinamiTest
    {
        class TestTinami : Tinami
        {
            public string FakeXml { get; set; }

            public TestTinami(string pattern, string replacement)
                : base(pattern, replacement)
            {
            }

            protected override XDocument FetchContentInfoApi(string url)
            {
                Assert.That(url, Is.StringMatching(@"http://api\.tinami\.com/content/info\?cont_id=.+&api_key=.+"));

                return XDocument.Parse(this.FakeXml);
            }
        }

        [Test]
        public void ApiTest()
        {
            var service = new TestTinami(@"^http://www\.tinami\.com/view/(?<ContentId>\d+)$",
                "http://api.tinami.com/content/info?cont_id=${ContentId}&api_key=" + ApplicationSettings.TINAMIApiKey);

            service.FakeXml = @"<?xml version='1.0' encoding='utf-8' ?>
<rsp stat='ok'>
  <content type='illust' issupport='1' iscollection='0'>
    <title>ほげほげ</title>
    <description>説明</description>
    <thumbnails>
      <thumbnail_150x150 url='http://img.tinami.com/hogehoge_150.gif' width='112' height='120'/>
    </thumbnails>
    <image>
      <url>http://img.tinami.com/hogehoge_full.gif</url>
      <width>640</width>
      <height>480</height>
    </image>
  </content>
</rsp>";
            var thumbinfo = service.GetThumbnailInfo("http://www.tinami.com/view/12345", null);

            Assert.That(thumbinfo, Is.Not.Null);
            Assert.That(thumbinfo.ImageUrl, Is.EqualTo("http://www.tinami.com/view/12345"));
            Assert.That(thumbinfo.ThumbnailUrl, Is.EqualTo("http://img.tinami.com/hogehoge_150.gif"));
            Assert.That(thumbinfo.TooltipText, Is.EqualTo("説明"));
        }

        [Test]
        public void ApiErrorTest()
        {
            var service = new TestTinami(@"^http://www\.tinami\.com/view/(?<ContentId>\d+)$",
                "http://api.tinami.com/content/info?cont_id=${ContentId}&api_key=" + ApplicationSettings.TINAMIApiKey);

            service.FakeXml = @"<?xml version='1.0' encoding='utf-8'?>
<rsp stat='user_only'>
  <err msg='この作品は登録ユーザー限定の作品です。'/>
</rsp>";
            var thumbinfo = service.GetThumbnailInfo("http://www.tinami.com/view/12345", null);

            Assert.That(thumbinfo, Is.Null);
        }
    }
}
