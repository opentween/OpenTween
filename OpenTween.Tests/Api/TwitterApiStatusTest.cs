// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net.Http;
using System.Text;
using System.Xml;
using OpenTween.Api.DataModel;
using Xunit;
using Xunit.Extensions;

namespace OpenTween.Api
{
    public class TwitterApiStatusTest
    {
        [Fact]
        public void ResetTest()
        {
            var apiStatus = new TwitterApiStatus();

            apiStatus.AccessLimit["/statuses/home_timeline"] = new ApiLimit(150, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0));
            apiStatus.MediaUploadLimit = new ApiLimit(150, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0));
            apiStatus.AccessLevel = TwitterApiAccessLevel.ReadWriteAndDirectMessage;

            apiStatus.Reset();

            Assert.Null(apiStatus.AccessLimit["/statuses/home_timeline"]);
            Assert.Null(apiStatus.MediaUploadLimit);
            Assert.Equal(TwitterApiAccessLevel.Anonymous, apiStatus.AccessLevel);
        }

        public static readonly TheoryData<Dictionary<string, string>, ApiLimit?> ParseRateLimitTestCase = new()
        {
            {
                new Dictionary<string, string>
                {
                    ["X-RateLimit-Limit"] = "150",
                    ["X-RateLimit-Remaining"] = "100",
                    ["X-RateLimit-Reset"] = "1356998400",
                },
                new ApiLimit(150, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0))
            },
            {
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["x-ratelimit-limit"] = "150",
                    ["x-ratelimit-remaining"] = "100",
                    ["x-ratelimit-reset"] = "1356998400",
                },
                new ApiLimit(150, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0))
            },
            {
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["X-RateLimit-Limit"] = "150",
                    ["X-RateLimit-Remaining"] = "100",
                    ["X-RateLimit-Reset"] = "hogehoge",
                },
                null
            },
            {
                new Dictionary<string, string>
                {
                    ["X-RateLimit-Limit"] = "150",
                    ["X-RateLimit-Remaining"] = "100",
                },
                null
            },
        };

        [Theory]
        [MemberData(nameof(ParseRateLimitTestCase))]
        public void ParseRateLimitTest(IDictionary<string, string> header, ApiLimit? expected)
        {
            var limit = TwitterApiStatus.ParseRateLimit(header, "X-RateLimit-");
            Assert.Equal(expected, limit);
        }

        public static readonly TheoryData<Dictionary<string, string>, ApiLimit?> ParseMediaRateLimitTestCase = new()
        {
            {
                new Dictionary<string, string>
                {
                    ["X-MediaRateLimit-Limit"] = "30",
                    ["X-MediaRateLimit-Remaining"] = "20",
                    ["X-MediaRateLimit-Reset"] = "1234567890",
                },
                new ApiLimit(30, 20, new DateTimeUtc(2009, 2, 13, 23, 31, 30))
            },
            {
                new Dictionary<string, string>
                {
                    ["X-MediaRateLimit-Limit"] = "30",
                    ["X-MediaRateLimit-Remaining"] = "20",
                    ["X-MediaRateLimit-Reset"] = "hogehoge",
                },
                null
            },
            {
                new Dictionary<string, string>
                {
                    ["X-MediaRateLimit-Limit"] = "30",
                    ["X-MediaRateLimit-Remaining"] = "20",
                },
                null
            },
        };

        [Theory]
        [MemberData(nameof(ParseMediaRateLimitTestCase))]
        public void ParseMediaRateLimitTest(IDictionary<string, string> header, ApiLimit? expected)
        {
            var limit = TwitterApiStatus.ParseRateLimit(header, "X-MediaRateLimit-");
            Assert.Equal(expected, limit);
        }

        public static readonly TheoryData<Dictionary<string, string>, TwitterApiAccessLevel?> ParseAccessLevelTestCase = new()
        {
            {
                new Dictionary<string, string> { { "X-Access-Level", "read" } },
                TwitterApiAccessLevel.Read
            },
            {
                new Dictionary<string, string> { { "X-Access-Level", "read-write" } },
                TwitterApiAccessLevel.ReadWrite
            },
            {
                new Dictionary<string, string> { { "X-Access-Level", "read-write-directmessages" } },
                TwitterApiAccessLevel.ReadWriteAndDirectMessage
            },
            {
                new Dictionary<string, string> { { "X-Access-Level", "" } }, // 何故かたまに出てくるやつ
                null
            },
            {
                new Dictionary<string, string> { },
                null
            },
        };

        [Theory]
        [MemberData(nameof(ParseAccessLevelTestCase))]
        public void ParseAccessLevelTest(IDictionary<string, string> header, TwitterApiAccessLevel? expected)
        {
            var accessLevel = TwitterApiStatus.ParseAccessLevel(header, "X-Access-Level");
            Assert.Equal(expected, accessLevel);
        }

        [Fact]
        public void UpdateFromHeader_DictionaryTest()
        {
            var status = new TwitterApiStatus();

            var header = new Dictionary<string, string>
            {
                ["X-Rate-Limit-Limit"] = "150",
                ["X-Rate-Limit-Remaining"] = "100",
                ["X-Rate-Limit-Reset"] = "1356998400",
                ["X-MediaRateLimit-Limit"] = "30",
                ["X-MediaRateLimit-Remaining"] = "20",
                ["X-MediaRateLimit-Reset"] = "1357084800",
                ["X-Access-Level"] = "read-write-directmessages",
            };

            Assert.Raises<TwitterApiStatus.AccessLimitUpdatedEventArgs>(
                x => status.AccessLimitUpdated += x,
                x => status.AccessLimitUpdated -= x,
                () => status.UpdateFromHeader(header, "/statuses/home_timeline")
            );

            var rateLimit = status.AccessLimit["/statuses/home_timeline"]!;
            Assert.Equal(150, rateLimit.AccessLimitCount);
            Assert.Equal(100, rateLimit.AccessLimitRemain);
            Assert.Equal(new DateTimeUtc(2013, 1, 1, 0, 0, 0), rateLimit.AccessLimitResetDate);

            var mediaLimit = status.MediaUploadLimit!;
            Assert.Equal(30, mediaLimit.AccessLimitCount);
            Assert.Equal(20, mediaLimit.AccessLimitRemain);
            Assert.Equal(new DateTimeUtc(2013, 1, 2, 0, 0, 0), mediaLimit.AccessLimitResetDate);

            Assert.Equal(TwitterApiAccessLevel.ReadWriteAndDirectMessage, status.AccessLevel);
        }

        [Fact]
        public void UpdateFromHeader_HttpClientTest()
        {
            var status = new TwitterApiStatus();

            var response = new HttpResponseMessage
            {
                Headers =
                {
                    { "x-rate-limit-limit", "150" },
                    { "x-rate-limit-remaining", "100" },
                    { "x-rate-limit-reset", "1356998400" },
                    { "x-mediaratelimit-limit", "30" },
                    { "x-mediaratelimit-remaining", "20" },
                    { "x-mediaratelimit-reset", "1357084800" },
                    { "x-access-level", "read-write-directmessages" },
                },
            };

            Assert.Raises<TwitterApiStatus.AccessLimitUpdatedEventArgs>(
                x => status.AccessLimitUpdated += x,
                x => status.AccessLimitUpdated -= x,
                () => status.UpdateFromHeader(response.Headers, "/statuses/home_timeline")
            );

            var rateLimit = status.AccessLimit["/statuses/home_timeline"]!;
            Assert.Equal(150, rateLimit.AccessLimitCount);
            Assert.Equal(100, rateLimit.AccessLimitRemain);
            Assert.Equal(new DateTimeUtc(2013, 1, 1, 0, 0, 0), rateLimit.AccessLimitResetDate);

            var mediaLimit = status.MediaUploadLimit!;
            Assert.Equal(30, mediaLimit.AccessLimitCount);
            Assert.Equal(20, mediaLimit.AccessLimitRemain);
            Assert.Equal(new DateTimeUtc(2013, 1, 2, 0, 0, 0), mediaLimit.AccessLimitResetDate);

            Assert.Equal(TwitterApiAccessLevel.ReadWriteAndDirectMessage, status.AccessLevel);
        }

        [Fact]
        public void UpdateFromJsonTest()
        {
            var status = new TwitterApiStatus();

            var json = "{\"resources\":{\"statuses\":{\"/statuses/home_timeline\":{\"limit\":150,\"remaining\":100,\"reset\":1356998400}}}}";

            Assert.Raises<TwitterApiStatus.AccessLimitUpdatedEventArgs>(
                x => status.AccessLimitUpdated += x,
                x => status.AccessLimitUpdated -= x,
                () => status.UpdateFromJson(TwitterRateLimits.ParseJson(json))
            );

            var rateLimit = status.AccessLimit["/statuses/home_timeline"]!;
            Assert.Equal(150, rateLimit.AccessLimitCount);
            Assert.Equal(100, rateLimit.AccessLimitRemain);
            Assert.Equal(new DateTimeUtc(2013, 1, 1, 0, 0, 0), rateLimit.AccessLimitResetDate);
        }

        [Fact]
        public void AccessLimitUpdatedTest()
        {
            var apiStatus = new TwitterApiStatus();

            Assert.Raises<TwitterApiStatus.AccessLimitUpdatedEventArgs>(
                x => apiStatus.AccessLimitUpdated += x,
                x => apiStatus.AccessLimitUpdated -= x,
                () => apiStatus.AccessLimit["/statuses/home_timeline"] = new ApiLimit(150, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0))
            );

            Assert.Raises<TwitterApiStatus.AccessLimitUpdatedEventArgs>(
                x => apiStatus.AccessLimitUpdated += x,
                x => apiStatus.AccessLimitUpdated -= x,
                () => apiStatus.Reset()
            );
        }
    }
}
