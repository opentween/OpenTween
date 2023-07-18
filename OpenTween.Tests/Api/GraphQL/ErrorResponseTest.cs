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
using Xunit;

namespace OpenTween.Api.GraphQL
{
    public class ErrorResponseTest
    {
        [Fact]
        public void ThrowIfError_ErrorResponseTest()
        {
            var responseText = File.ReadAllText("Resources/Responses/Error_NotFound.json");
            var exception = Assert.Throws<WebApiException>(() => ErrorResponse.ThrowIfError(responseText));
            Assert.Equal("_Missing: No status found with that ID.", exception.Message);
        }

        [Fact]
        public void ThrowIfError_SuccessResponseTest()
        {
            var responseText = File.ReadAllText("Resources/Responses/TweetDetail.json");
            ErrorResponse.ThrowIfError(responseText);
            // 通常のレスポンスに対しては WebApiException を発生させない
        }
    }
}
