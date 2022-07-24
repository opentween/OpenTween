// OpenTween - Client of Twitter
// Copyright (c) 2922 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Threading.Tasks;
using Moq;
using OpenTween.Api.DataModel;
using OpenTween.Connection;
using Xunit;

namespace OpenTween.Api.TwitterV2
{
    public class GetTimelineRequestTest
    {
        [Fact]
        public async Task StatusesMentionsTimeline_Test()
        {
            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                x.GetAsync<TwitterV2TweetIds>(
                    new Uri("/2/users/100/timelines/reverse_chronological", UriKind.Relative),
                    new Dictionary<string, string>
                    {
                        { "tweet.fields", "id" },
                        { "max_results", "200" },
                        { "until_id", "900" },
                        { "since_id", "100" },
                    },
                    "/2/users/:id/timelines/reverse_chronological"
                )
            )
            .ReturnsAsync(new TwitterV2TweetIds());

            var request = new GetTimelineRequest(userId: 100L)
            {
                MaxResults = 200,
                SinceId = "100",
                UntilId = "900",
            };

            await request.Send(mock.Object).ConfigureAwait(false);

            mock.VerifyAll();
        }
    }
}
