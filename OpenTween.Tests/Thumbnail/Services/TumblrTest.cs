using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace OpenTween.Thumbnail.Services
{
    public class TumblrTest
    {
        [Fact]
        public void ParsePostJson_Test()
        {
            var json = @"{
  ""meta"": { ""status"": 200, ""msg"": ""OK"" },
  ""response"": {
    ""blog"": { },
    ""posts"": [
      {
        ""id"": 1234567,
        ""post_url"": ""http://example.com/post/1234567"",
        ""type"": ""photo"",
        ""photos"": [
          {
            ""caption"": """",
            ""alt_sizes"": [
              {
                ""width"": 1280,
                ""height"": 722,
                ""url"": ""http://example.com/photo/1280/1234567/1/tumblr_hogehoge""
              }
            ]
          }
        ]
      }
    ]
  }
}";
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var thumbs = Tumblr.ParsePhotoPostJson(jsonBytes);

            var expected = new[]
            {
                new ThumbnailInfo
                {
                    ImageUrl = "http://example.com/post/1234567",
                    ThumbnailUrl = "http://example.com/photo/1280/1234567/1/tumblr_hogehoge",
                    TooltipText = null,
                },
            };
            Assert.Equal(expected, thumbs);
        }

        [Fact]
        public async Task GetThumbnailInfoAsync_RequestTest()
        {
            var handler = new HttpMessageHandlerMock();

            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Get, x.Method);
                Assert.Equal("https://api.tumblr.com/v2/blog/hoge.tumblr.com/posts",
                    x.RequestUri.GetLeftPart(UriPartial.Path));

                var query = HttpUtility.ParseQueryString(x.RequestUri.Query);

                Assert.Equal(ApplicationSettings.TumblrConsumerKey, query["api_key"]);
                Assert.Equal("1234567", query["id"]);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(@"{
  ""meta"": { ""status"": 200, ""msg"": ""OK"" },
  ""response"": { ""blog"": { }, ""posts"": { } }
}"),
                };
            });

            using (var http = new HttpClient(handler))
            {
                var service = new Tumblr(http);

                var url = "http://hoge.tumblr.com/post/1234567/tetetete";
                var thumb = await service.GetThumbnailInfoAsync(url, null, CancellationToken.None)
                    .ConfigureAwait(false);
            }

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task GetThumbnailInfoAsync_CustomHostnameRequestTest()
        {
            var handler = new HttpMessageHandlerMock();

            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Get, x.Method);
                Assert.Equal("https://api.tumblr.com/v2/blog/tumblr.example.com/posts",
                    x.RequestUri.GetLeftPart(UriPartial.Path));

                var query = HttpUtility.ParseQueryString(x.RequestUri.Query);

                Assert.Equal(ApplicationSettings.TumblrConsumerKey, query["api_key"]);
                Assert.Equal("1234567", query["id"]);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(@"{
  ""meta"": { ""status"": 200, ""msg"": ""OK"" },
  ""response"": { ""blog"": { }, ""posts"": { } }
}"),
                };
            });

            using (var http = new HttpClient(handler))
            {
                var service = new Tumblr(http);

                // Tumblrのカスタムドメイン名を使ってるっぽいURL
                var url = "http://tumblr.example.com/post/1234567/tetetete";
                var thumb = await service.GetThumbnailInfoAsync(url, null, CancellationToken.None)
                    .ConfigureAwait(false);
            }

            Assert.Equal(0, handler.QueueCount);
        }
    }
}
