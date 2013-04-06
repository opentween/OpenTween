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
using System.Text;
using NUnit.Framework;

namespace OpenTween.Api
{
    [TestFixture]
    class TwitterApiStatusTest
    {
        [Test]
        public void ResetTest()
        {
            var apiStatus = new TwitterApiStatus();

            apiStatus.AccessLimit = new ApiLimit(150, 100, new DateTime(2013, 1, 1, 0, 0, 0));
            apiStatus.MediaUploadLimit = new ApiLimit(150, 100, new DateTime(2013, 1, 1, 0, 0, 0));
            apiStatus.AccessLevel = TwitterApiAccessLevel.ReadWriteAndDirectMessage;

            apiStatus.Reset();

            Assert.That(apiStatus.AccessLimit, Is.Null);
            Assert.That(apiStatus.MediaUploadLimit, Is.Null);
            Assert.That(apiStatus.AccessLevel, Is.EqualTo(TwitterApiAccessLevel.Anonymous));
        }

        public static object[] ParseRateLimit_TestCase =
        {
            new object[] {
                new Dictionary<string, string> {
                    {"X-RateLimit-Limit", "150"},
                    {"X-RateLimit-Remaining", "100"},
                    {"X-RateLimit-Reset", "1356998400"},
                },
                new ApiLimit(150, 100, new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime()),
            },
            new object[] {
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                    {"x-ratelimit-limit", "150"},
                    {"x-ratelimit-remaining", "100"},
                    {"x-ratelimit-reset", "1356998400"},
                },
                new ApiLimit(150, 100, new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime()),
            },
            new object[] {
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                    {"X-RateLimit-Limit", "150"},
                    {"X-RateLimit-Remaining", "100"},
                    {"X-RateLimit-Reset", "hogehoge"},
                },
                null,
            },
            new object[] {
                new Dictionary<string, string> {
                    {"X-RateLimit-Limit", "150"},
                    {"X-RateLimit-Remaining", "100"},
                },
                null,
            },
        };
        [TestCaseSource("ParseRateLimit_TestCase")]
        public void ParseRateLimitTest(IDictionary<string, string> header, ApiLimit expect)
        {
            var limit = TwitterApiStatus.ParseRateLimit(header);
            Assert.That(limit, Is.EqualTo(expect));
        }

        public static object[] ParseMediaRateLimit_TestCase =
        {
            new object[] {
                new Dictionary<string, string> {
                    {"X-MediaRateLimit-Limit", "30"},
                    {"X-MediaRateLimit-Remaining", "20"},
                    {"X-MediaRateLimit-Reset", "1234567890"},
                },
                new ApiLimit(30, 20, new DateTime(2009, 2, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()),
            },
            new object[] {
                new Dictionary<string, string> {
                    {"X-MediaRateLimit-Limit", "30"},
                    {"X-MediaRateLimit-Remaining", "20"},
                    {"X-MediaRateLimit-Reset", "hogehoge"},
                },
                null,
            },
            new object[] {
                new Dictionary<string, string> {
                    {"X-MediaRateLimit-Limit", "30"},
                    {"X-MediaRateLimit-Remaining", "20"},
                },
                null,
            },
        };
        [TestCaseSource("ParseMediaRateLimit_TestCase")]
        public void ParseMediaRateLimitTest(IDictionary<string, string> header, ApiLimit expect)
        {
            var limit = TwitterApiStatus.ParseMediaRateLimit(header);
            Assert.That(limit, Is.EqualTo(expect));
        }

        public static object[] ParseAccessLevel_TestCase =
        {
            new object[] {
                new Dictionary<string, string> { {"X-Access-Level", "read"} },
                TwitterApiAccessLevel.Read,
            },
            new object[] {
                new Dictionary<string, string> { {"X-Access-Level", "read-write"} },
                TwitterApiAccessLevel.ReadWrite,
            },
            new object[] {
                new Dictionary<string, string> { {"X-Access-Level", "read-write-directmessages"} },
                TwitterApiAccessLevel.ReadWriteAndDirectMessage,
            },
            new object[] {
                new Dictionary<string, string> { {"X-Access-Level", ""} }, // 何故かたまに出てくるやつ
                null,
            },
            new object[] {
                new Dictionary<string, string> { },
                null,
            },
        };
        [TestCaseSource("ParseAccessLevel_TestCase")]
        public void ParseAccessLevelTest(IDictionary<string, string> header, TwitterApiAccessLevel? expect)
        {
            var accessLevel = TwitterApiStatus.ParseAccessLevel(header);
            Assert.That(accessLevel, Is.EqualTo(expect));
        }

        [Test]
        public void UpdateFromHeaderTest()
        {
            var status = new TwitterApiStatus();

            var eventCalled = false;
            status.AccessLimitUpdated += (s, e) => eventCalled = true;

            var header = new Dictionary<string, string>
            {
                {"X-RateLimit-Limit", "150"},
                {"X-RateLimit-Remaining", "100"},
                {"X-RateLimit-Reset", "1356998400"},
                {"X-MediaRateLimit-Limit", "30"},
                {"X-MediaRateLimit-Remaining", "20"},
                {"X-MediaRateLimit-Reset", "1357084800"},
                {"X-Access-Level", "read-write-directmessages"},
            };

            status.UpdateFromHeader(header);

            var rateLimit = status.AccessLimit;
            Assert.That(rateLimit.AccessLimitCount, Is.EqualTo(150));
            Assert.That(rateLimit.AccessLimitRemain, Is.EqualTo(100));
            Assert.That(rateLimit.AccessLimitResetDate, Is.EqualTo(new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime()));

            var mediaLimit = status.MediaUploadLimit;
            Assert.That(mediaLimit.AccessLimitCount, Is.EqualTo(30));
            Assert.That(mediaLimit.AccessLimitRemain, Is.EqualTo(20));
            Assert.That(mediaLimit.AccessLimitResetDate, Is.EqualTo(new DateTime(2013, 1, 2, 0, 0, 0, DateTimeKind.Utc).ToLocalTime()));

            Assert.That(status.AccessLevel, Is.EqualTo(TwitterApiAccessLevel.ReadWriteAndDirectMessage));

            Assert.That(eventCalled, Is.True);
        }

        [Test]
        public void UpdateFromApiTest()
        {
            var status = new TwitterApiStatus();

            var eventCalled = false;
            status.AccessLimitUpdated += (s, e) => eventCalled = true;

            var apiResponse = new TwitterDataModel.RateLimitStatus
            {
                HourlyLimit = 150,
                RemainingHits = 100,
                ResetTime = "Tue Jan 01 00:00:00 +0000 2013",
                ResetTimeInSeconds = 1356998400,
                Photos = new TwitterDataModel.MediaRateLimitStatus
                {
                    DailyLimit = 30,
                    RemainingHits = 20,
                    ResetTime = "Wed Jan 02 00:00:00 +0000 2013",
                    RestTimeInSeconds = 1357084800,
                },
            };

            status.UpdateFromApi(apiResponse);

            var rateLimit = status.AccessLimit;
            Assert.That(rateLimit.AccessLimitCount, Is.EqualTo(150));
            Assert.That(rateLimit.AccessLimitRemain, Is.EqualTo(100));
            Assert.That(rateLimit.AccessLimitResetDate, Is.EqualTo(new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime()));

            var mediaLimit = status.MediaUploadLimit;
            Assert.That(mediaLimit.AccessLimitCount, Is.EqualTo(30));
            Assert.That(mediaLimit.AccessLimitRemain, Is.EqualTo(20));
            Assert.That(mediaLimit.AccessLimitResetDate, Is.EqualTo(new DateTime(2013, 1, 2, 0, 0, 0, DateTimeKind.Utc).ToLocalTime()));

            Assert.That(eventCalled, Is.True);
        }

        [Test]
        public void UpdateFromApiTest2()
        {
            var status = new TwitterApiStatus();

            var eventCalled = false;
            status.AccessLimitUpdated += (s, e) => eventCalled = true;

            var apiResponse = new TwitterDataModel.RateLimitStatus
            {
                HourlyLimit = 150,
                RemainingHits = 100,
                ResetTime = "Tue Jan 01 00:00:00 +0000 2013",
                ResetTimeInSeconds = 1356998400,
                Photos = null,
            };

            status.UpdateFromApi(apiResponse);

            var rateLimit = status.AccessLimit;
            Assert.That(rateLimit.AccessLimitCount, Is.EqualTo(150));
            Assert.That(rateLimit.AccessLimitRemain, Is.EqualTo(100));
            Assert.That(rateLimit.AccessLimitResetDate, Is.EqualTo(new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime()));

            Assert.That(status.MediaUploadLimit, Is.Null);

            Assert.That(eventCalled, Is.True);
        }

        [Test]
        public void UpdateFromApiTest3()
        {
            var status = new TwitterApiStatus();

            var eventCalled = false;
            status.AccessLimitUpdated += (s, e) => eventCalled = true;

            Assert.That(() => status.UpdateFromApi(null), Throws.TypeOf<ArgumentNullException>());

            Assert.That(status.AccessLimit, Is.Null);
            Assert.That(status.MediaUploadLimit, Is.Null);

            Assert.That(eventCalled, Is.False);
        }

        [Test]
        public void AccessLimitUpdatedTest()
        {
            var apiStatus = new TwitterApiStatus();

            var eventCount = 0;
            apiStatus.AccessLimitUpdated += (s, e) => eventCount++;

            Assert.That(eventCount, Is.EqualTo(0));

            apiStatus.AccessLimit = new ApiLimit(150, 100, new DateTime(2013, 1, 1, 0, 0, 0));
            Assert.That(eventCount, Is.EqualTo(1));

            apiStatus.Reset();
            Assert.That(eventCount, Is.EqualTo(2));
        }
    }
}
