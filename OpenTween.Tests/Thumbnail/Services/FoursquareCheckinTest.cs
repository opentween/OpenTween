// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace OpenTween.Thumbnail.Services
{
    public class FoursquareCheckinTest
    {
        [Fact]
        public async Task GetThumbnailInfoAsync_RequestTest()
        {
            var handler = new HttpMessageHandlerMock();
            using (var http = new HttpClient(handler))
            {
                var service = new FoursquareCheckin(http);

                handler.Enqueue(x =>
                {
                    Assert.Equal(HttpMethod.Get, x.Method);
                    Assert.Equal("https://api.foursquare.com/v2/checkins/xxxxxxxx",
                        x.RequestUri.GetLeftPart(UriPartial.Path));

                    var query = HttpUtility.ParseQueryString(x.RequestUri.Query);

                    Assert.Equal(ApplicationSettings.FoursquareClientId, query["client_id"]);
                    Assert.Equal(ApplicationSettings.FoursquareClientSecret, query["client_secret"]);
                    Assert.NotNull(query["v"]);
                    Assert.Null(query["signature"]);

                    // リクエストに対するテストなのでレスポンスは適当に返す
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                });

                var post = new PostClass
                {
                    PostGeo = new PostClass.StatusGeo { },
                };

                var thumb = await service.GetThumbnailInfoAsync(
                    "https://foursquare.com/hogehoge/checkin/xxxxxxxx",
                    post, CancellationToken.None);

                Assert.Equal(0, handler.QueueCount);
            }
        }

        [Fact]
        public async Task GetThumbnailInfoAsync_RequestWithSignatureTest()
        {
            var handler = new HttpMessageHandlerMock();
            using (var http = new HttpClient(handler))
            {
                var service = new FoursquareCheckin(http);

                handler.Enqueue(x =>
                {
                    Assert.Equal(HttpMethod.Get, x.Method);
                    Assert.Equal("https://api.foursquare.com/v2/checkins/xxxxxxxx",
                        x.RequestUri.GetLeftPart(UriPartial.Path));

                    var query = HttpUtility.ParseQueryString(x.RequestUri.Query);

                    Assert.Equal(ApplicationSettings.FoursquareClientId, query["client_id"]);
                    Assert.Equal(ApplicationSettings.FoursquareClientSecret, query["client_secret"]);
                    Assert.NotNull(query["v"]);
                    Assert.Equal("aaaaaaa", query["signature"]);

                    // リクエストに対するテストなのでレスポンスは適当に返す
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                });

                var post = new PostClass
                {
                    PostGeo = new PostClass.StatusGeo { },
                };

                var thumb = await service.GetThumbnailInfoAsync(
                    "https://foursquare.com/hogehoge/checkin/xxxxxxxx?s=aaaaaaa",
                    post, CancellationToken.None);

                Assert.Equal(0, handler.QueueCount);
            }
        }

        [Fact]
        public async Task GetThumbnailInfoAsync_GeoLocatedTweetTest()
        {
            var handler = new HttpMessageHandlerMock();
            using (var http = new HttpClient(handler))
            {
                var service = new FoursquareCheckin(http);

                handler.Enqueue(x =>
                {
                    // このリクエストは実行されないはず
                    Assert.True(false);
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                });

                // 既にジオタグが付いているツイートに対しては何もしない
                var post = new PostClass
                {
                    PostGeo = new PostClass.StatusGeo
                    {
                        Lat = 34.35067978344854,
                        Lng = 134.04693603515625,
                    },
                };

                var thumb = await service.GetThumbnailInfoAsync(
                    "https://foursquare.com/hogehoge/checkin/xxxxxxxx?s=aaaaaaa",
                    post, CancellationToken.None);

                Assert.Equal(1, handler.QueueCount);
            }
        }

        [Fact]
        public void ParseInLocation_Test()
        {
            var json = @"{
  ""meta"": { ""code"": 200 },
  ""response"": {
    ""checkin"": {
      ""id"": ""xxxxxxxxx"",
      ""type"": ""checkin"",
      ""venue"": {
        ""id"": ""4b73dedcf964a5206bbe2de3"",
        ""name"": ""高松駅 (Takamatsu Sta.)"",
        ""location"": {
          ""lat"": 34.35067978344854,
          ""lng"": 134.04693603515625
        }
      }
    }
  }
}";
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var location = FoursquareCheckin.ParseIntoLocation(jsonBytes);

            Assert.NotNull(location);
            Assert.Equal(34.35067978344854, location.Latitude);
            Assert.Equal(134.04693603515625, location.Longitude);
        }

        [Fact]
        public void ParseInLocation_PlanetTest()
        {
            var json = @"{
  ""meta"": { ""code"": 200 },
  ""response"": {
    ""checkin"": {
      ""id"": ""xxxxxxxxx"",
      ""type"": ""checkin"",
      ""venue"": {
        ""id"": ""5069d8bdc640385aa7711fe4"",
        ""name"": ""Gale Crater"",
        ""location"": {
          ""planet"": ""mars"",
          ""lat"": 34.201694,
          ""lng"": -118.17166
        }
      }
    }
  }
}";
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var location = FoursquareCheckin.ParseIntoLocation(jsonBytes);

            // 地球以外の位置にあるベニューに対しては null を返す
            Assert.Null(location);
        }

        [Fact]
        public void ParseInLocation_VenueNullTest()
        {
            var json = @"{
  ""meta"": { ""code"": 200 },
  ""response"": {
    ""checkin"": {
      ""id"": ""xxxxxxxxx"",
      ""type"": ""checkin"",
      ""venue"": null
    }
  }
}";
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var location = FoursquareCheckin.ParseIntoLocation(jsonBytes);

            // ベニュー情報が得られなかった場合は null を返す
            Assert.Null(location);
        }
    }
}
