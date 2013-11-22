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
        public void PostMatchOptionsTest()
        {
            var filter = new PostFilterRule { FilterName = "hogehoge" };
            var post = new PostClass { ScreenName = "hogehoge" };

            filter.MoveMatches = false;
            filter.MarkMatches = false;
            Assert.Equal(MyCommon.HITRESULT.Copy, filter.ExecFilter(post));

            filter.MoveMatches = false;
            filter.MarkMatches = true;
            Assert.Equal(MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            filter.MoveMatches = true;
            filter.MarkMatches = false; // 無視される
            Assert.Equal(MyCommon.HITRESULT.Move, filter.ExecFilter(post));

            filter.MoveMatches = true;
            filter.MarkMatches = true; // 無視される
            Assert.Equal(MyCommon.HITRESULT.Move, filter.ExecFilter(post));
        }

        [Theory]
        [InlineData(false, "hogehoge", false)]
        [InlineData(false, "hogehoge", true)]
        [InlineData(true, "(hoge){2}", false)]
        [InlineData(true, "(hoge){2}", true)]
        public void NameTest(bool useRegex, string pattern, bool exclude)
        {
            var filter = new PostFilterRule();
            PostClass post;

            if (!exclude)
            {
                filter.UseRegex = useRegex;
                filter.FilterName = pattern;
            }
            else
            {
                filter.ExUseRegex = useRegex;
                filter.ExFilterName = pattern;
            }

            post = new PostClass { ScreenName = "hogehoge" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "foo" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterName は RetweetedBy にもマッチする
            post = new PostClass { ScreenName = "foo", RetweetedBy = "hogehoge" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "foo", RetweetedBy = "bar" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            if (!useRegex)
            {
                // FilterName は完全一致 (UseRegex = false の場合)
                post = new PostClass { ScreenName = "_hogehoge_" };
                Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));
            }
            else
            {
                // FilterName は部分一致 (UseRegex = true の場合)
                post = new PostClass { ScreenName = "_hogehoge_" };
                Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
            }

            // 大小文字を区別する
            if (!exclude)
                filter.CaseSensitive = true;
            else
                filter.ExCaseSensitive = true;

            post = new PostClass { ScreenName = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            if (!exclude)
                filter.CaseSensitive = false;
            else
                filter.ExCaseSensitive = false;

            post = new PostClass { ScreenName = "HogeHoge" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Theory]
        [InlineData(false, new[] { "aaa", "bbb" }, false)]
        [InlineData(false, new[] { "aaa", "bbb" }, true)]
        [InlineData(true, new[] { "a{3}", "b{3}" }, false)]
        [InlineData(true, new[] { "a{3}", "b{3}" }, true)]
        public void BodyTest(bool useRegex, string[] pattern, bool exclude)
        {
            var filter = new PostFilterRule();
            PostClass post;

            if (!exclude)
            {
                filter.UseRegex = useRegex;
                filter.FilterBody = pattern;
            }
            else
            {
                filter.ExUseRegex = useRegex;
                filter.ExFilterBody = pattern;
            }

            post = new PostClass { TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 片方だけではマッチしない
            post = new PostClass { TextFromApi = "aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の文字列が全て含まれている
            post = new PostClass { TextFromApi = "123aaa456bbb" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // ScreenName にはマッチしない (UseNameField = true の場合)
            post = new PostClass { ScreenName = "aaabbb", TextFromApi = "test" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            if (!exclude)
                filter.CaseSensitive = true;
            else
                filter.ExCaseSensitive = true;

            post = new PostClass { TextFromApi = "AaaBbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            if (!exclude)
                filter.CaseSensitive = false;
            else
                filter.ExCaseSensitive = false;

            post = new PostClass { TextFromApi = "AaaBbb" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Theory]
        [InlineData(false, new[] { "aaa", "bbb" }, false)]
        [InlineData(false, new[] { "aaa", "bbb" }, true)]
        [InlineData(true, new[] { "a{3}", "b{3}" }, false)]
        [InlineData(true, new[] { "a{3}", "b{3}" }, true)]
        public void BodyUrlTest(bool useRegex, string[] pattern, bool exclude)
        {
            var filter = new PostFilterRule();
            PostClass post;

            if (!exclude)
            {
                // FilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
                filter.FilterByUrl = true;

                filter.UseRegex = useRegex;
                filter.FilterBody = pattern;
            }
            else
            {
                // ExFilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
                filter.ExFilterByUrl = true;

                filter.ExUseRegex = useRegex;
                filter.ExFilterBody = pattern;
            }

            post = new PostClass { Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 片方だけではマッチしない
            post = new PostClass { Text = "<a href='http://example.com/aaa'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // FilterBody の文字列が全て含まれている
            post = new PostClass { Text = "<a href='http://example.com/aaabbb'>t.co/hoge</a>" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // ScreenName にはマッチしない (UseNameField = true の場合)
            post = new PostClass { ScreenName = "aaabbb", Text = "<a href='http://example.com/123'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            if (!exclude)
                filter.CaseSensitive = true;
            else
                filter.ExCaseSensitive = true;

            post = new PostClass { Text = "<a href='http://example.com/AaaBbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            if (!exclude)
                filter.CaseSensitive = false;
            else
                filter.ExCaseSensitive = false;

            post = new PostClass { Text = "<a href='http://example.com/AaaBbb'>t.co/hoge</a>" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Theory]
        [InlineData(false, new[] { "aaa", "bbb" }, false)]
        [InlineData(false, new[] { "aaa", "bbb" }, true)]
        [InlineData(true, new[] { "a{3}", "b{3}" }, false)]
        [InlineData(true, new[] { "a{3}", "b{3}" }, true)]
        public void BodyAndNameTest(bool useRegex, string[] pattern, bool exclude)
        {
            var filter = new PostFilterRule();
            PostClass post;

            if (!exclude)
            {
                // UseNameField = false の場合は FilterBody のマッチ対象が Text と ScreenName の両方になる
                filter.UseNameField = false;

                filter.UseRegex = useRegex;
                filter.FilterBody = pattern;
            }
            else
            {
                // ExUseNameField = false の場合は ExFilterBody のマッチ対象が Text と ScreenName の両方になる
                filter.ExUseNameField = false;

                filter.ExUseRegex = useRegex;
                filter.ExFilterBody = pattern;
            }

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
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // TextFromApi と ScreenName に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "aaa", TextFromApi = "bbb" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // TextFromApi と RetweetedBy に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "hoge", TextFromApi = "bbb", RetweetedBy = "aaa" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // RetweetedBy が null でなくても依然として ScreenName にはマッチする
            post = new PostClass { ScreenName = "aaa", TextFromApi = "bbb", RetweetedBy = "hoge" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            if (!useRegex)
            {
                // ScreenName に対しては完全一致 (UseRegex = false の場合)
                post = new PostClass { ScreenName = "_aaa_", TextFromApi = "bbb" };
                Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));
            }
            else
            {
                // ScreenName に対しても部分一致 (UseRegex = true の場合)
                post = new PostClass { ScreenName = "_aaa_", TextFromApi = "bbb" };
                Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
            }

            // TextFromApi に対しては UseRegex に関わらず常に部分一致
            post = new PostClass { ScreenName = "aaa", TextFromApi = "_bbb_" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // 大小文字を区別する
            if (!exclude)
                filter.CaseSensitive = true;
            else
                filter.ExCaseSensitive = true;

            post = new PostClass { ScreenName = "Aaa", TextFromApi = "Bbb" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", TextFromApi = "Bbb", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            if (!exclude)
                filter.CaseSensitive = false;
            else
                filter.ExCaseSensitive = false;

            post = new PostClass { ScreenName = "Aaa", TextFromApi = "Bbb" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", TextFromApi = "Bbb", RetweetedBy = "Aaa" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Theory]
        [InlineData(false, new[] { "aaa", "bbb" }, false)]
        [InlineData(false, new[] { "aaa", "bbb" }, true)]
        [InlineData(true, new[] { "a{3}", "b{3}" }, false)]
        [InlineData(true, new[] { "a{3}", "b{3}" }, true)]
        public void BodyUrlAndNameTest(bool useRegex, string[] pattern, bool exclude)
        {
            var filter = new PostFilterRule();
            PostClass post;

            if (!exclude)
            {
                // FilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
                filter.FilterByUrl = true;

                // UseNameField = false の場合は FilterBody のマッチ対象が Text と ScreenName の両方になる
                filter.UseNameField = false;

                filter.UseRegex = useRegex;
                filter.FilterBody = pattern;
            }
            else
            {
                // ExFilterByUrl = true の場合は TextFromApi ではなく Text がマッチ対象になる
                filter.ExFilterByUrl = true;

                // ExUseNameField = false の場合は ExFilterBody のマッチ対象が Text と ScreenName の両方になる
                filter.ExUseNameField = false;

                filter.ExUseRegex = useRegex;
                filter.ExFilterBody = pattern;
            }

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
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // Text と ScreenName に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // Text と ScreenName に FilterBody の文字列がそれぞれ含まれている
            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>", RetweetedBy = "aaa" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // RetweetedBy が null でなくても依然として ScreenName にはマッチする
            post = new PostClass { ScreenName = "aaa", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>", RetweetedBy = "hoge" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            if (!useRegex)
            {
                // ScreenName に対しては完全一致 (UseRegex = false の場合)
                post = new PostClass { ScreenName = "_aaa_", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>" };
                Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));
            }
            else
            {
                // ScreenName に対しても部分一致 (UseRegex = true の場合)
                post = new PostClass { ScreenName = "_aaa_", Text = "<a href='http://example.com/bbb'>t.co/hoge</a>" };
                Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
            }

            // Text に対しては UseRegex に関わらず常に部分一致
            post = new PostClass { ScreenName = "aaa", Text = "_bbb_" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            // 大小文字を区別する
            if (!exclude)
                filter.CaseSensitive = true;
            else
                filter.ExCaseSensitive = true;

            post = new PostClass { ScreenName = "Aaa", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>", RetweetedBy = "Aaa" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            if (!exclude)
                filter.CaseSensitive = false;
            else
                filter.ExCaseSensitive = false;

            post = new PostClass { ScreenName = "Aaa", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { ScreenName = "hoge", Text = "<a href='http://example.com/Bbb'>t.co/hoge</a>", RetweetedBy = "Aaa" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Theory]
        [InlineData(false, "hogehoge", false)]
        [InlineData(false, "hogehoge", true)]
        [InlineData(true, "(hoge){2}", false)]
        [InlineData(true, "(hoge){2}", true)]
        public void SourceTest(bool useRegex, string pattern, bool exclude)
        {
            var filter = new PostFilterRule();
            PostClass post;

            if (!exclude)
            {
                filter.UseRegex = useRegex;
                filter.FilterSource = pattern;
            }
            else
            {
                filter.ExUseRegex = useRegex;
                filter.ExFilterSource = pattern;
            }

            post = new PostClass { Source = "hogehoge" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { Source = "foo" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            if (!useRegex)
            {
                // FilterSource は完全一致 (UseRegex = false の場合)
                post = new PostClass { Source = "_hogehoge_" };
                Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));
            }
            else
            {
                // FilterSource は部分一致 (UseRegex = true の場合)
                post = new PostClass { Source = "_hogehoge_" };
                Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
            }

            // 大小文字を区別する
            if (!exclude)
                filter.CaseSensitive = true;
            else
                filter.ExCaseSensitive = true;

            post = new PostClass { Source = "HogeHoge" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            if (!exclude)
                filter.CaseSensitive = false;
            else
                filter.ExCaseSensitive = false;

            post = new PostClass { Source = "HogeHoge" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Theory]
        [InlineData(false, "hogehoge", false)]
        [InlineData(false, "hogehoge", true)]
        [InlineData(true, "(hoge){2}", false)]
        [InlineData(true, "(hoge){2}", true)]
        public void SourceHtmlTest(bool useRegex, string pattern, bool exclude)
        {
            var filter = new PostFilterRule();
            PostClass post;

            if (!exclude)
            {
                // FilterByUrl = true の場合は Source ではなく SourceHtml がマッチ対象になる
                filter.FilterByUrl = true;

                filter.UseRegex = useRegex;
                filter.FilterSource = pattern;
            }
            else
            {
                // ExFilterByUrl = true の場合は Source ではなく SourceHtml がマッチ対象になる
                filter.ExFilterByUrl = true;

                filter.ExUseRegex = useRegex;
                filter.ExFilterSource = pattern;
            }

            // FilterSource は UseRegex の値に関わらず部分一致
            post = new PostClass { SourceHtml = "<a href='http://example.com/hogehoge'>****</a>" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { SourceHtml = "<a href='http://example.com/foo'>****</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別する
            if (!exclude)
                filter.CaseSensitive = true;
            else
                filter.ExCaseSensitive = true;

            post = new PostClass { SourceHtml = "<a href='http://example.com/HogeHoge'>****</a>" };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));

            // 大小文字を区別しない
            if (!exclude)
                filter.CaseSensitive = false;
            else
                filter.ExCaseSensitive = false;

            post = new PostClass { SourceHtml = "<a href='http://example.com/HogeHoge'>****</a>" };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsRtTest(bool exclude)
        {
            var filter = new PostFilterRule();
            PostClass post;

            if (!exclude)
                filter.FilterRt = true;
            else
                filter.ExFilterRt = true;

            post = new PostClass { RetweetedBy = "hogehoge", RetweetedId = 123L };
            Assert.Equal(exclude ? MyCommon.HITRESULT.Exclude : MyCommon.HITRESULT.CopyAndMark, filter.ExecFilter(post));

            post = new PostClass { };
            Assert.Equal(MyCommon.HITRESULT.None, filter.ExecFilter(post));
        }
    }
}
