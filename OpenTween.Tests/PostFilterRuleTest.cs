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
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class PostFilterRuleTest
    {
        public PostFilterRuleTest()
        {
            PostFilterRule.AutoCompile = true;
        }

        [Fact]
        public void EmptyRuleTest()
        {
            var filter = new PostFilterRule { };
            var post = new PostClass { ScreenName = "hogehoge" };

            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));
        }

        [Fact]
        public void NullTest()
        {
            var filter = new PostFilterRule
            {
                FilterName = null,
                FilterSource = null,
                ExFilterName = null,
                ExFilterSource = null,
            };
            var post = new PostClass { ScreenName = "hogehoge" };

            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            Assert.Throws<ArgumentNullException>(() => filter.FilterBody = null);
            Assert.Throws<ArgumentNullException>(() => filter.ExFilterBody = null);
        }

        [Fact]
        public void MatchOnlyTest()
        {
            var filter = new PostFilterRule { FilterName = "hogehoge" };
            var post = new PostClass { ScreenName = "hogehoge" };

            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExcludeOnlyTest()
        {
            var filter = new PostFilterRule { ExFilterName = "hogehoge" };
            var post = new PostClass { ScreenName = "hogehoge" };

            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void MatchAndExcludeTest()
        {
            var filter = new PostFilterRule { FilterName = "hogehoge", ExFilterSource = "tetete" };
            var post = new PostClass { ScreenName = "hogehoge", Source = "tetete" };

            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));
        }

        [Fact]
        public void PostMatchOptions_CopyTest()
        {
            var filter = new PostFilterRule { FilterName = "hogehoge" };
            var post = new PostClass { ScreenName = "hogehoge" };

            filter.MoveMatches = false;
            filter.MarkMatches = false;
            Assert.Equal(MyCommon.HITRESULT.Copy, filter.ExecFilter(post));
        }

        [Fact]
        public void PostMatchOptions_CopyAndMarkTest()
        {
            var filter = new PostFilterRule { FilterName = "hogehoge" };
            var post = new PostClass { ScreenName = "hogehoge" };

            filter.MoveMatches = false;
            filter.MarkMatches = true;
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void PostMatchOptions_MoveTest()
        {
            var filter = new PostFilterRule { FilterName = "hogehoge" };
            var post = new PostClass { ScreenName = "hogehoge" };

            filter.MoveMatches = true;
            filter.MarkMatches = false; // 無視される
            Assert.Equal(MyCommon.HITRESULT.Move, filter.ExecFilter(post));

            filter.MoveMatches = true;
            filter.MarkMatches = true; // 無視される
            Assert.Equal(MyCommon.HITRESULT.Move, filter.ExecFilter(post));
        }

        [Fact]
        public void Disabled_Test()
        {
            var filter = new PostFilterRule { FilterName = "hogehoge" };
            var post = new PostClass { ScreenName = "hogehoge" };

            filter.Enabled = false;
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterName_Test()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.FilterName = "hogehoge";

            post = new PostClass { ScreenName = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "foo" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterName は RetweetedBy にもマッチする
            post = new PostClass { ScreenName = "foo", RetweetedBy = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "foo", RetweetedBy = "bar" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterName は完全一致 (UseRegex = false の場合)
            post = new PostClass { ScreenName = "_hogehoge_" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { ScreenName = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { ScreenName = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterName_Test()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.ExFilterName = "hogehoge";

            post = new PostClass { ScreenName = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "foo" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterName は RetweetedBy にもマッチする
            post = new PostClass { ScreenName = "foo", RetweetedBy = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "foo", RetweetedBy = "bar" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterName は完全一致 (ExUseRegex = false の場合)
            post = new PostClass { ScreenName = "_hogehoge_" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { ScreenName = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { ScreenName = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterName_RegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.UseRegex = true;
            filter.FilterName = "(hoge){2}";

            post = new PostClass { ScreenName = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "foo" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterName は RetweetedBy にもマッチする
            post = new PostClass { ScreenName = "foo", RetweetedBy = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "foo", RetweetedBy = "bar" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterName は部分一致 (UseRegex = true の場合)
            post = new PostClass { ScreenName = "_hogehoge_" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { ScreenName = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { ScreenName = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterName_RegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.ExUseRegex = true;
            filter.ExFilterName = "(hoge){2}";

            post = new PostClass { ScreenName = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "foo" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterName は RetweetedBy にもマッチする
            post = new PostClass { ScreenName = "foo", RetweetedBy = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "foo", RetweetedBy = "bar" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterName は部分一致 (ExUseRegex = true の場合)
            post = new PostClass { ScreenName = "_hogehoge_" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { ScreenName = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { ScreenName = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterBody_Test()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.FilterBody = new[] { "aaa", "bbb" };

            post = new PostClass { TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 片方だけではマッチしない
            post = new PostClass { TextFromApi = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の文字列が全て含まれている
            post = new PostClass { TextFromApi = "123aaa456bbb" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // ScreenName にはマッチしない (UseNameField = true の場合)
            post = new PostClass { ScreenName = "aaabbb", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { TextFromApi = "AaaBbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { TextFromApi = "AaaBbb" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterBody_Test()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.ExFilterBody = new[] { "aaa", "bbb" };

            post = new PostClass { TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 片方だけではマッチしない
            post = new PostClass { TextFromApi = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の文字列が全て含まれている
            post = new PostClass { TextFromApi = "123aaa456bbb" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // ScreenName にはマッチしない (ExUseNameField = true の場合)
            post = new PostClass { ScreenName = "aaabbb", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { TextFromApi = "AaaBbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { TextFromApi = "AaaBbb" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterBody_RegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.UseRegex = true;
            filter.FilterBody = new[] { "a{3}", "b{3}" };

            post = new PostClass { TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 片方だけではマッチしない
            post = new PostClass { TextFromApi = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の文字列が全て含まれている
            post = new PostClass { TextFromApi = "123aaa456bbb" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // ScreenName にはマッチしない (UseNameField = true の場合)
            post = new PostClass { ScreenName = "aaabbb", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { TextFromApi = "AaaBbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { TextFromApi = "AaaBbb" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterBody_RegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.ExUseRegex = true;
            filter.ExFilterBody = new[] { "a{3}", "b{3}" };

            post = new PostClass { TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 片方だけではマッチしない
            post = new PostClass { TextFromApi = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の文字列が全て含まれている
            post = new PostClass { TextFromApi = "123aaa456bbb" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // ScreenName にはマッチしない (ExUseNameField = true の場合)
            post = new PostClass { ScreenName = "aaabbb", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { TextFromApi = "AaaBbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { TextFromApi = "AaaBbb" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterBody_ByUrlTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // FilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
            filter.FilterByUrl = true;

            filter.FilterBody = new[] { "aaa", "bbb" };

            post = new PostClass { Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 片方だけではマッチしない
            post = new PostClass { Text = "<a href='http://example.com/aaa'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の文字列が全て含まれている
            post = new PostClass { Text = "<a href='http://example.com/aaabbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // ScreenName にはマッチしない (UseNameField = true の場合)
            post = new PostClass { ScreenName = "aaabbb", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { Text = "<a href='http://example.com/AaaBbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { Text = "<a href='http://example.com/AaaBbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterBody_ByUrlTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // ExFilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
            filter.ExFilterByUrl = true;

            filter.ExFilterBody = new[] { "aaa", "bbb" };

            post = new PostClass { Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 片方だけではマッチしない
            post = new PostClass { Text = "<a href='http://example.com/aaa'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の文字列が全て含まれている
            post = new PostClass { Text = "<a href='http://example.com/aaabbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // ScreenName にはマッチしない (ExUseNameField = true の場合)
            post = new PostClass { ScreenName = "aaabbb", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { Text = "<a href='http://example.com/AaaBbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { Text = "<a href='http://example.com/AaaBbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterBody_ByUrlRegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // FilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
            filter.FilterByUrl = true;

            filter.UseRegex = true;
            filter.FilterBody = new[] { "a{3}", "b{3}" };

            post = new PostClass { Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 片方だけではマッチしない
            post = new PostClass { Text = "<a href='http://example.com/aaa'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の文字列が全て含まれている
            post = new PostClass { Text = "<a href='http://example.com/aaabbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // ScreenName にはマッチしない (UseNameField = true の場合)
            post = new PostClass { ScreenName = "aaabbb", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { Text = "<a href='http://example.com/AaaBbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { Text = "<a href='http://example.com/AaaBbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterBody_ByUrlRegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // ExFilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
            filter.ExFilterByUrl = true;

            filter.ExUseRegex = true;
            filter.ExFilterBody = new[] { "a{3}", "b{3}" };

            post = new PostClass { Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 片方だけではマッチしない
            post = new PostClass { Text = "<a href='http://example.com/aaa'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の文字列が全て含まれている
            post = new PostClass { Text = "<a href='http://example.com/aaabbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // ScreenName にはマッチしない (ExUseNameField = true の場合)
            post = new PostClass { ScreenName = "aaabbb", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { Text = "<a href='http://example.com/AaaBbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { Text = "<a href='http://example.com/AaaBbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterBodyAndName_Test()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // UseNameField = false の場合は FilterBody のマッチ対象が TextFromApi と ScreenName の両方になる
            filter.UseNameField = false;

            filter.FilterBody = new[] { "aaa", "bbb" };

            post = new PostClass { ScreenName = "hoge", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "hoge", TextFromApi = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "aaa", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // TextFromApi に FilterBody の文字列が全て含まれている
            post = new PostClass { ScreenName = "hoge", TextFromApi = "123aaa456bbb" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // TextFromApi と ScreenName に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "aaa", TextFromApi = "bbb" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // TextFromApi と RetweetedBy に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "hoge", TextFromApi = "bbb", RetweetedBy = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // RetweetedBy が null でなくても依然として ScreenName にはマッチする
            post = new PostClass { ScreenName = "aaa", TextFromApi = "bbb", RetweetedBy = "hoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // ScreenName に対しては完全一致 (UseRegex = false の場合)
            post = new PostClass { ScreenName = "_aaa_", TextFromApi = "bbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // TextFromApi に対しては UseRegex に関わらず常に部分一致
            post = new PostClass { ScreenName = "aaa", TextFromApi = "_bbb_" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { ScreenName = "Aaa", TextFromApi = "Bbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", TextFromApi = "Bbb", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { ScreenName = "Aaa", TextFromApi = "Bbb" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", TextFromApi = "Bbb", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterBodyAndName_Test()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // ExUseNameField = false の場合は ExFilterBody のマッチ対象が TextFromApi と ScreenName の両方になる
            filter.ExUseNameField = false;

            filter.ExFilterBody = new[] { "aaa", "bbb" };

            post = new PostClass { ScreenName = "hoge", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "hoge", TextFromApi = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "aaa", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // TextFromApi に ExFilterBody の文字列が全て含まれている
            post = new PostClass { ScreenName = "hoge", TextFromApi = "123aaa456bbb" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // TextFromApi と ScreenName に ExFilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "aaa", TextFromApi = "bbb" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // TextFromApi と RetweetedBy に ExFilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "hoge", TextFromApi = "bbb", RetweetedBy = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // RetweetedBy が null でなくても依然として ScreenName にはマッチする
            post = new PostClass { ScreenName = "aaa", TextFromApi = "bbb", RetweetedBy = "hoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // ScreenName に対しては完全一致 (ExUseRegex = false の場合)
            post = new PostClass { ScreenName = "_aaa_", TextFromApi = "bbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // TextFromApi に対しては ExUseRegex に関わらず常に部分一致
            post = new PostClass { ScreenName = "aaa", TextFromApi = "_bbb_" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { ScreenName = "Aaa", TextFromApi = "Bbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", TextFromApi = "Bbb", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { ScreenName = "Aaa", TextFromApi = "Bbb" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", TextFromApi = "Bbb", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterBodyAndName_RegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // UseNameField = false の場合は FilterBody のマッチ対象が TextFromApi と ScreenName の両方になる
            filter.UseNameField = false;

            filter.UseRegex = true;
            filter.FilterBody = new[] { "a{3}", "b{3}" };

            post = new PostClass { ScreenName = "hoge", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "hoge", TextFromApi = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "aaa", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // TextFromApi に FilterBody の文字列が全て含まれている
            post = new PostClass { ScreenName = "hoge", TextFromApi = "123aaa456bbb" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // TextFromApi と ScreenName に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "aaa", TextFromApi = "bbb" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // TextFromApi と RetweetedBy に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "hoge", TextFromApi = "bbb", RetweetedBy = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // RetweetedBy が null でなくても依然として ScreenName にはマッチする
            post = new PostClass { ScreenName = "aaa", TextFromApi = "bbb", RetweetedBy = "hoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // ScreenName に対しても部分一致 (UseRegex = true の場合)
            post = new PostClass { ScreenName = "_aaa_", TextFromApi = "bbb" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // TextFromApi に対しては UseRegex に関わらず常に部分一致
            post = new PostClass { ScreenName = "aaa", TextFromApi = "_bbb_" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { ScreenName = "Aaa", TextFromApi = "Bbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", TextFromApi = "Bbb", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { ScreenName = "Aaa", TextFromApi = "Bbb" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", TextFromApi = "Bbb", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterBodyAndName_RegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // ExUseNameField = false の場合は ExFilterBody のマッチ対象が TextFromApi と ScreenName の両方になる
            filter.ExUseNameField = false;

            filter.ExUseRegex = true;
            filter.ExFilterBody = new[] { "a{3}", "b{3}" };

            post = new PostClass { ScreenName = "hoge", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "hoge", TextFromApi = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "aaa", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // TextFromApi に ExFilterBody の文字列が全て含まれている
            post = new PostClass { ScreenName = "hoge", TextFromApi = "123aaa456bbb" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // TextFromApi と ScreenName に ExFilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "aaa", TextFromApi = "bbb" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // TextFromApi と RetweetedBy に ExFilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "hoge", TextFromApi = "bbb", RetweetedBy = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // RetweetedBy が null でなくても依然として ScreenName にはマッチする
            post = new PostClass { ScreenName = "aaa", TextFromApi = "bbb", RetweetedBy = "hoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // ScreenName に対しても部分一致 (ExUseRegex = true の場合)
            post = new PostClass { ScreenName = "_aaa_", TextFromApi = "bbb" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // TextFromApi に対しては ExUseRegex に関わらず常に部分一致
            post = new PostClass { ScreenName = "aaa", TextFromApi = "_bbb_" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { ScreenName = "Aaa", TextFromApi = "Bbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", TextFromApi = "Bbb", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { ScreenName = "Aaa", TextFromApi = "Bbb" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", TextFromApi = "Bbb", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterBodyAndName_ByUrlTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // FilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
            filter.FilterByUrl = true;

            // UseNameField = false の場合は FilterBody のマッチ対象が Text と ScreenName の両方になる
            filter.UseNameField = false;

            filter.FilterBody = new[] { "aaa", "bbb" };

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/aaa'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // Text に FilterBody の文字列が全て含まれている
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/aaabbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // Text と ScreenName に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // Text と ScreenName に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>", RetweetedBy = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // RetweetedBy が null でなくても依然として ScreenName にはマッチする
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>", RetweetedBy = "hoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // ScreenName に対しては完全一致 (UseRegex = false の場合)
            post = new PostClass { ScreenName = "_aaa_", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // Text に対しては UseRegex に関わらず常に部分一致
            post = new PostClass { ScreenName = "aaa", Text = "_bbb_" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { ScreenName = "Aaa", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { ScreenName = "Aaa", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterBodyAndName_ByUrlTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // ExFilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
            filter.ExFilterByUrl = true;

            // ExUseNameField = false の場合は ExFilterBody のマッチ対象が Text と ScreenName の両方になる
            filter.ExUseNameField = false;

            filter.ExFilterBody = new[] { "aaa", "bbb" };

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/aaa'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // Text に ExFilterBody の文字列が全て含まれている
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/aaabbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // Text と ScreenName に ExFilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // Text と ScreenName に ExFilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>", RetweetedBy = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // RetweetedBy が null でなくても依然として ScreenName にはマッチする
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>", RetweetedBy = "hoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // ScreenName に対しては完全一致 (ExUseRegex = false の場合)
            post = new PostClass { ScreenName = "_aaa_", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // Text に対しては ExUseRegex に関わらず常に部分一致
            post = new PostClass { ScreenName = "aaa", Text = "_bbb_" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { ScreenName = "Aaa", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { ScreenName = "Aaa", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterBodyAndName_ByUrlRegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // FilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
            filter.FilterByUrl = true;

            // UseNameField = false の場合は FilterBody のマッチ対象が Text と ScreenName の両方になる
            filter.UseNameField = false;

            filter.UseRegex = true;
            filter.FilterBody = new[] { "a{3}", "b{3}" };

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/aaa'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // Text に FilterBody の文字列が全て含まれている
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/aaabbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // Text と ScreenName に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // Text と ScreenName に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>", RetweetedBy = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // RetweetedBy が null でなくても依然として ScreenName にはマッチする
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>", RetweetedBy = "hoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // ScreenName に対しても部分一致 (UseRegex = true の場合)
            post = new PostClass { ScreenName = "_aaa_", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // Text に対しては UseRegex に関わらず常に部分一致
            post = new PostClass { ScreenName = "aaa", Text = "_bbb_" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { ScreenName = "Aaa", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { ScreenName = "Aaa", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterBodyAndName_ByUrlRegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // ExFilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
            filter.ExFilterByUrl = true;

            // ExUseNameField = false の場合は ExFilterBody のマッチ対象が Text と ScreenName の両方になる
            filter.ExUseNameField = false;

            filter.ExUseRegex = true;
            filter.ExFilterBody = new[] { "a{3}", "b{3}" };

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/aaa'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterBody の片方だけではマッチしない
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // Text に ExFilterBody の文字列が全て含まれている
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/aaabbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // Text と ScreenName に ExFilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // Text と ScreenName に ExFilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>", RetweetedBy = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // RetweetedBy が null でなくても依然として ScreenName にはマッチする
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>", RetweetedBy = "hoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // ScreenName に対しても部分一致 (ExUseRegex = true の場合)
            post = new PostClass { ScreenName = "_aaa_", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // Text に対しては ExUseRegex に関わらず常に部分一致
            post = new PostClass { ScreenName = "aaa", Text = "_bbb_" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { ScreenName = "Aaa", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { ScreenName = "Aaa", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterSource_Test()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.FilterSource = "hogehoge";

            post = new PostClass { Source = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { Source = "foo" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterSource は完全一致 (UseRegex = false の場合)
            post = new PostClass { Source = "_hogehoge_" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { Source = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { Source = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterSource_Test()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.ExFilterSource = "hogehoge";

            post = new PostClass { Source = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { Source = "foo" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterSource は完全一致 (ExUseRegex = false の場合)
            post = new PostClass { Source = "_hogehoge_" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { Source = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { Source = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterSource_RegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.UseRegex = true;
            filter.FilterSource = "(hoge){2}";

            post = new PostClass { Source = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { Source = "foo" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterSource は部分一致 (UseRegex = true の場合)
            post = new PostClass { Source = "_hogehoge_" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { Source = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { Source = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterSource_RegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.ExUseRegex = true;
            filter.ExFilterSource = "(hoge){2}";

            post = new PostClass { Source = "hogehoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { Source = "foo" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // ExFilterSource は部分一致 (ExUseRegex = true の場合)
            post = new PostClass { Source = "_hogehoge_" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { Source = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { Source = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterSource_ByUrlTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // FilterByUrl = true の場合は Source ではなく SourceHtml がマッチ対象になる
            filter.FilterByUrl = true;

            filter.FilterSource = "hogehoge";

            // FilterSource は UseRegex の値に関わらず部分一致
            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/hogehoge") };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/foo") };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/HogeHoge") };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/HogeHoge") };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterSource_ByUrlTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // ExFilterByUrl = true の場合は Source ではなく SourceHtml がマッチ対象になる
            filter.ExFilterByUrl = true;

            filter.ExFilterSource = "hogehoge";

            // ExFilterSource は ExUseRegex の値に関わらず部分一致
            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/hogehoge") };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/foo") };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/HogeHoge") };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/HogeHoge") };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterSource_ByUrlRegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // FilterByUrl = true の場合は Source ではなく SourceHtml がマッチ対象になる
            filter.FilterByUrl = true;

            filter.UseRegex = true;
            filter.FilterSource = "(hoge){2}";

            // FilterSource は UseRegex の値に関わらず部分一致
            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/hogehoge") };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/foo") };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.CaseSensitive = true;

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/HogeHoge") };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.CaseSensitive = false;

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/HogeHoge") };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterSource_ByUrlRegexTest()
        {
            var filter = new PostFilterRule();
            PostClass post;

            // ExFilterByUrl = true の場合は Source ではなく SourceHtml がマッチ対象になる
            filter.ExFilterByUrl = true;

            filter.ExUseRegex = true;
            filter.ExFilterSource = "(hoge){2}";

            // ExFilterSource は ExUseRegex の値に関わらず部分一致
            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/hogehoge") };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/foo") };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            filter.ExCaseSensitive = true;

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/HogeHoge") };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            filter.ExCaseSensitive = false;

            post = new PostClass { Source = "****", SourceUri = new Uri("http://example.com/HogeHoge") };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));
        }

        [Fact]
        public void FilterRt_Test()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.FilterRt = true;

            post = new PostClass { RetweetedBy = "hogehoge", RetweetedId = 123L };
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));
        }

        [Fact]
        public void ExFilterRt_Test()
        {
            var filter = new PostFilterRule();
            PostClass post;

            filter.ExFilterRt = true;

            post = new PostClass { RetweetedBy = "hogehoge", RetweetedId = 123L };
            Assert.Equal(MyCommon.HITRESULT.Exclude, filter.ExecFilter(post));

            post = new PostClass { };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));
        }

        [Fact]
        public void SetProperty_Test()
        {
            var filter = new PostFilterRule();

            string changedPropeyty = null;

            filter.PropertyChanged += (_, x) => changedPropeyty = x.PropertyName;
            filter.FilterName = "hogehoge";

            Assert.Equal("FilterName", changedPropeyty);
            Assert.True(filter.IsDirty);
        }

        [Fact]
        public void SetProperty_SameValueTest()
        {
            var filter = new PostFilterRule();
            filter.FilterName = "hogehoge";
            filter.Compile();

            string changedPropeyty = null;

            // 値に変化がないので PropertyChanged イベントは発生しない
            filter.PropertyChanged += (_, x) => changedPropeyty = x.PropertyName;
            filter.FilterName = "hogehoge";

            Assert.Null(changedPropeyty);
            Assert.False(filter.IsDirty);
        }

        [Fact]
        public void Equals_Test()
        {
            var filter1 = new PostFilterRule
            {
                FilterName = "name",
                FilterBody = new[] { "body" },
                FilterSource = "source",
                ExFilterName = "nameEx",
                ExFilterBody = new[] { "bodyEx" },
                ExFilterSource = "sourceEx",
            };
            var filter2 = new PostFilterRule
            {
                FilterName = "name",
                FilterBody = new[] { "body" },
                FilterSource = "source",
                ExFilterName = "nameEx",
                ExFilterBody = new[] { "bodyEx" },
                ExFilterSource = "sourceEx",
            };

            Assert.True(filter1.Equals(filter2));
            Assert.True(filter2.Equals(filter1));
        }

        [Fact]
        public void Equals_HasNoMatchConditionsTest()
        {
            var filter1 = new PostFilterRule
            {
                // マッチ条件
                FilterName = "",
                CaseSensitive = true,

                // 除外条件
                ExFilterName = "nameEx",
            };
            var filter2 = new PostFilterRule
            {
                FilterName = "",
                CaseSensitive = false,
                ExFilterName = "nameEx",
            };

            // マッチ条件 (≠除外条件) が無いので CaseSensitive 等の差異は無視する
            Assert.True(filter1.Equals(filter2));
            Assert.True(filter2.Equals(filter1));
        }

        [Fact]
        public void Equals_HasNoExcludeConditionsTest()
        {
            var filter1 = new PostFilterRule
            {
                // マッチ条件
                FilterName = "name",

                // 除外条件
                ExFilterName = "",
                ExCaseSensitive = true,
            };
            var filter2 = new PostFilterRule
            {
                FilterName = "name",
                ExFilterName = "",
                ExCaseSensitive = false,
            };

            // 除外条件が無いので ExCaseSensitive 等の差異は無視する
            Assert.True(filter1.Equals(filter2));
            Assert.True(filter2.Equals(filter1));
        }
    }
}
