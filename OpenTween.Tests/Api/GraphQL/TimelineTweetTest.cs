﻿// OpenTween - Client of Twitter
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
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using OpenTween.Models;
using Xunit;

namespace OpenTween.Api.GraphQL
{
    public class TimelineTweetTest
    {
        private XElement LoadResponseDocument(string filename)
        {
            using var stream = File.OpenRead($"Resources/Responses/{filename}");
            using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);
            return XElement.Load(jsonReader);
        }

        private TabInformations CreateTabInfo()
        {
            var tabinfo = new TabInformations();
            tabinfo.AddDefaultTabs();
            return tabinfo;
        }

        [Fact]
        public void ExtractTimelineTweets_Single_Test()
        {
            var rootElm = this.LoadResponseDocument("ListLatestTweetsTimeline_SimpleTweet.json");
            var timelineTweets = TimelineTweet.ExtractTimelineTweets(rootElm);
            Assert.Single(timelineTweets);
        }

        [Fact]
        public void ExtractTimelineTweets_Conversation_Test()
        {
            var rootElm = this.LoadResponseDocument("ListLatestTweetsTimeline_Conversation.json");
            var timelineTweets = TimelineTweet.ExtractTimelineTweets(rootElm);
            Assert.Equal(3, timelineTweets.Length);
        }

        [Fact]
        public void ToStatus_WithTwitterPostFactory_SimpleTweet_Test()
        {
            var rootElm = this.LoadResponseDocument("TimelineTweet_SimpleTweet.json");
            var timelineTweet = new TimelineTweet(rootElm);
            var status = timelineTweet.ToTwitterStatus();
            var postFactory = new TwitterPostFactory(this.CreateTabInfo());
            var post = postFactory.CreateFromStatus(status, selfUserId: 1L, new HashSet<long>());

            Assert.Equal("1613784711020826626", post.StatusId.Id);
            Assert.Equal(40480664L, post.UserId);
            Assert.False(post.IsPromoted);
        }

        [Fact]
        public void ToStatus_WithTwitterPostFactory_TweetWithMedia_Test()
        {
            var rootElm = this.LoadResponseDocument("TimelineTweet_TweetWithMedia.json");
            var timelineTweet = new TimelineTweet(rootElm);
            var status = timelineTweet.ToTwitterStatus();
            var postFactory = new TwitterPostFactory(this.CreateTabInfo());
            var post = postFactory.CreateFromStatus(status, selfUserId: 1L, new HashSet<long>());

            Assert.Equal("1614587968567783424", post.StatusId.Id);
            Assert.Equal(40480664L, post.UserId);
            Assert.Equal(2, post.Media.Count);
            Assert.Equal("https://pbs.twimg.com/media/FmgrJiEaAAEU42G.png", post.Media[0].Url);
            Assert.Equal("OpenTweenで @opentween のツイート一覧を表示しているスクショ", post.Media[0].AltText);
            Assert.Equal("https://pbs.twimg.com/media/FmgrJiXaMAEu873.jpg", post.Media[1].Url);
            Assert.Equal("OpenTweenの新しい画像投稿画面を動かしている様子のスクショ", post.Media[1].AltText);
        }

        [Fact]
        public void ToStatus_WithTwitterPostFactory_RetweetedTweet_Test()
        {
            var rootElm = this.LoadResponseDocument("TimelineTweet_RetweetedTweet.json");
            var timelineTweet = new TimelineTweet(rootElm);
            var status = timelineTweet.ToTwitterStatus();
            var postFactory = new TwitterPostFactory(this.CreateTabInfo());
            var post = postFactory.CreateFromStatus(status, selfUserId: 1L, new HashSet<long>());

            Assert.Equal("1617128268548964354", post.StatusId.Id);
            Assert.Equal(40480664L, post.RetweetedByUserId);
            Assert.Equal("1617126084138659840", post.RetweetedId!.Id);
            Assert.Equal(514241801L, post.UserId);
        }

        [Fact]
        public void ToStatus_WithTwitterPostFactory_TweetWithVisibility_Test()
        {
            var rootElm = this.LoadResponseDocument("TimelineTweet_TweetWithVisibility.json");
            var timelineTweet = new TimelineTweet(rootElm);
            var status = timelineTweet.ToTwitterStatus();
            var postFactory = new TwitterPostFactory(this.CreateTabInfo());
            var post = postFactory.CreateFromStatus(status, selfUserId: 1L, new HashSet<long>());

            Assert.Equal("1602775353088524288", post.StatusId.Id);
            Assert.Equal(357750891L, post.UserId);
        }

        [Fact]
        public void ToStatus_WithTwitterPostFactory_SelfThread_Test()
        {
            var rootElm = this.LoadResponseDocument("TimelineTweet_SelfThread.json");
            var timelineTweet = new TimelineTweet(rootElm);
            var status = timelineTweet.ToTwitterStatus();
            var postFactory = new TwitterPostFactory(this.CreateTabInfo());
            var post = postFactory.CreateFromStatus(status, selfUserId: 1L, new HashSet<long>());

            Assert.Equal("1511751702684499968", post.StatusId.Id);
            Assert.Equal(40480664L, post.UserId);
        }

        [Fact]
        public void ToStatus_WithTwitterPostFactory_QuotedTweet_Test()
        {
            var rootElm = this.LoadResponseDocument("TimelineTweet_QuotedTweet.json");
            var timelineTweet = new TimelineTweet(rootElm);
            var status = timelineTweet.ToTwitterStatus();
            var postFactory = new TwitterPostFactory(this.CreateTabInfo());
            var post = postFactory.CreateFromStatus(status, selfUserId: 1L, new HashSet<long>());

            Assert.Equal("1588614645866147840", post.StatusId.Id);
            var quotedPostId = Assert.Single(post.QuoteStatusIds);
            Assert.Equal("1583108196868116480", quotedPostId.Id);
        }

        [Fact]
        public void ToStatus_WithTwitterPostFactory_QuotedTweet_Tombstone_Test()
        {
            var rootElm = this.LoadResponseDocument("TimelineTweet_QuotedTweet_Tombstone.json");
            var timelineTweet = new TimelineTweet(rootElm);
            var status = timelineTweet.ToTwitterStatus();
            var postFactory = new TwitterPostFactory(this.CreateTabInfo());
            var post = postFactory.CreateFromStatus(status, selfUserId: 1L, new HashSet<long>());

            Assert.Equal("1614653321310253057", post.StatusId.Id);
            var quotedPostId = Assert.Single(post.QuoteStatusIds);
            Assert.Equal("1614650279194136576", quotedPostId.Id);
        }

        [Fact]
        public void ToStatus_WithTwitterPostFactory_PromotedTweet_Test()
        {
            var rootElm = this.LoadResponseDocument("TimelineTweet_PromotedTweet.json");
            var timelineTweet = new TimelineTweet(rootElm);
            var status = timelineTweet.ToTwitterStatus();
            var postFactory = new TwitterPostFactory(this.CreateTabInfo());
            var post = postFactory.CreateFromStatus(status, selfUserId: 1L, new HashSet<long>());

            Assert.Equal("1674737917363888129", post.StatusId.Id);
            Assert.Equal(2941313791L, post.UserId);
            Assert.True(post.IsPromoted);
            Assert.Matches(new Regex(@"^\[Promoted\]\n"), post.TextFromApi);
        }

        [Fact]
        public void ToStatus_TweetTombstone_Test()
        {
            var rootElm = this.LoadResponseDocument("TimelineTweet_TweetTombstone.json");
            var timelineTweet = new TimelineTweet(rootElm);

            Assert.True(timelineTweet.IsTombstone);
            var ex = Assert.Throws<WebApiException>(
                () => timelineTweet.ToTwitterStatus()
            );
            Assert.Equal("This Post is from a suspended account. Learn more", ex.Message);
        }
    }
}
