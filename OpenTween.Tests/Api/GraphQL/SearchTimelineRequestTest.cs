// OpenTween - Client of Twitter
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;
using System.Threading.Tasks;
using Moq;
using OpenTween.Connection;
using Xunit;

namespace OpenTween.Api.GraphQL
{
    public class SearchTimelineRequestTest
    {
        [Fact]
        public async Task Send_Test()
        {
            using var responseStream = File.OpenRead("Resources/Responses/SearchTimeline_SimpleTweet.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.GetStreamAsync(It.IsAny<Uri>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<string>())
                )
                .Callback<Uri, IDictionary<string, string>, string>((url, param, endpointName) =>
                {
                    Assert.Equal(new("https://twitter.com/i/api/graphql/lZ0GCEojmtQfiUQa5oJSEw/SearchTimeline"), url);
                    Assert.Equal(2, param.Count);
                    Assert.Equal("""{"rawQuery":"#OpenTween","count":20,"product":"Latest"}""", param["variables"]);
                    Assert.True(param.ContainsKey("features"));
                    Assert.Equal("SearchTimeline", endpointName);
                })
                .ReturnsAsync(responseStream);

            var request = new SearchTimelineRequest(rawQuery: "#OpenTween")
            {
                Count = 20,
            };

            var response = await request.Send(mock.Object).ConfigureAwait(false);
            Assert.Single(response.Tweets);
            Assert.Equal("DAADDAABCgABFnlh4hraMAYKAAIOTm0DEhTAAQAIAAIAAAABCAADAAAAAAgABAAAAAAKAAUX8j3ezIAnEAoABhfyPd7Mf9jwAAA", response.CursorTop);
            Assert.Equal("DAADDAABCgABFnlh4hraMAYKAAIOTm0DEhTAAQAIAAIAAAACCAADAAAAAAgABAAAAAAKAAUX8j3ezIAnEAoABhfyPd7Mf9jwAAA", response.CursorBottom);

            mock.VerifyAll();
        }

        [Fact]
        public async Task Send_RequestCursor_Test()
        {
            using var responseStream = File.OpenRead("Resources/Responses/SearchTimeline_SimpleTweet.json");

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.GetStreamAsync(It.IsAny<Uri>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<string>())
                )
                .Callback<Uri, IDictionary<string, string>, string>((url, param, endpointName) =>
                {
                    Assert.Equal(new("https://twitter.com/i/api/graphql/lZ0GCEojmtQfiUQa5oJSEw/SearchTimeline"), url);
                    Assert.Equal(2, param.Count);
                    Assert.Equal("""{"rawQuery":"#OpenTween","count":20,"product":"Latest","cursor":"aaa"}""", param["variables"]);
                    Assert.True(param.ContainsKey("features"));
                    Assert.Equal("SearchTimeline", endpointName);
                })
                .ReturnsAsync(responseStream);

            var request = new SearchTimelineRequest(rawQuery: "#OpenTween")
            {
                Count = 20,
                Cursor = "aaa",
            };

            await request.Send(mock.Object).ConfigureAwait(false);
            mock.VerifyAll();
        }
    }
}
