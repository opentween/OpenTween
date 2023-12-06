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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTween.Models;
using Xunit;

namespace OpenTween
{
    public class TweetDetailsViewTest
    {
        [WinFormsFact]
        public void Initialize_Test()
        {
            using var detailsView = new TweetDetailsView();
        }

        [Fact]
        public void FormatQuoteTweetHtml_PostClassTest()
        {
            var post = new PostClass
            {
                StatusId = new TwitterStatusId("12345"),
                Nickname = "upsilon",
                ScreenName = "kim_upsilon",
                Text = """<a href="https://twitter.com/twitterapi">@twitterapi</a> hogehoge""",
                CreatedAt = new DateTimeUtc(2015, 3, 30, 3, 30, 0),
            };

            // PostClass.Text はリンクを除去するのみでエスケープは行わない
            // (TweetFormatter によって既にエスケープされた文字列が格納されているため)

            var expected = """<a class="quote-tweet-link" href="//opentween/status/12345">""" +
                """<blockquote class="quote-tweet">""" +
                "<p>@twitterapi hogehoge</p> &mdash; upsilon (@kim_upsilon) " + DateTimeUtc.Parse("2015/03/30 3:30:00", DateTimeFormatInfo.InvariantInfo).ToLocalTimeString() +
                "</blockquote></a>";
            Assert.Equal(expected, TweetDetailsView.FormatQuoteTweetHtml(post, isReply: false));
        }

        [Fact]
        public void FormatQuoteTweetHtml_HtmlTest()
        {
            var statusId = new TwitterStatusId("12345"); // リンク先のステータスID
            var html = "<marquee>hogehoge</marquee>"; // HTMLをそのまま出力する (エスケープしない)

            var expected = """<a class="quote-tweet-link" href="//opentween/status/12345">""" +
                """<blockquote class="quote-tweet"><marquee>hogehoge</marquee></blockquote>""" +
                "</a>";
            Assert.Equal(expected, TweetDetailsView.FormatQuoteTweetHtml(statusId, html, isReply: false));
        }

        [Fact]
        public void FormatQuoteTweetHtml_ReplyHtmlTest()
        {
            // blockquote の class に reply が付与される
            var expected = """<a class="quote-tweet-link" href="//opentween/status/12345">""" +
                """<blockquote class="quote-tweet reply">hogehoge</blockquote>""" +
                "</a>";
            Assert.Equal(expected, TweetDetailsView.FormatQuoteTweetHtml(new TwitterStatusId("12345"), "hogehoge", isReply: true));
        }

        [Fact]
        public void StripLinkTagHtml_Test()
        {
            var html = """<a href="https://twitter.com/twitterapi">@twitterapi</a>""";

            var expected = "@twitterapi";
            Assert.Equal(expected, TweetDetailsView.StripLinkTagHtml(html));
        }
    }
}
