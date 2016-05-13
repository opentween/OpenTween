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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Xunit;
using Xunit.Extensions;

namespace OpenTween.Models
{
    /// <summary>
    /// FiltersClass -> PostFilterRule クラスへの変更で v1.1.2 までの
    /// 設定ファイルと互換性が保たれているかどうかを確認するテスト
    /// </summary>
    public class PostFilterRuleVersion113DeserializeTest
    {
        // OpenTween v1.1.2 時点の FiltersClass クラス
        public sealed class FiltersClass
        {
            public string NameFilter { get; set; }
            public string ExNameFilter { get; set; }
            public string[] BodyFilterArray { get; set; }
            public string[] ExBodyFilterArray { get; set; }
            public bool SearchBoth { get; set; }
            public bool ExSearchBoth { get; set; }
            public bool MoveFrom { get; set; }
            public bool SetMark { get; set; }
            public bool SearchUrl { get; set; }
            public bool ExSearchUrl { get; set; }
            public bool CaseSensitive { get; set; }
            public bool ExCaseSensitive { get; set; }
            public bool UseLambda { get; set; }
            public bool ExUseLambda { get; set; }
            public bool UseRegex { get; set; }
            public bool ExUseRegex { get; set; }
            public bool IsRt { get; set; }
            public bool IsExRt { get; set; }
            public string Source { get; set; }
            public string ExSource { get; set; }
        }

        [Fact]
        public void DeserializeTest()
        {
            var oldVersion = new FiltersClass
            {
                NameFilter = "name",
                ExNameFilter = "exname",
                BodyFilterArray = new[] { "body1", "body2" },
                ExBodyFilterArray = new[] { "exbody1", "exbody2" },
                SearchBoth = true,
                ExSearchBoth = true,
                MoveFrom = true,
                SetMark = true,
                SearchUrl = true,
                ExSearchUrl = true,
                CaseSensitive = true,
                ExCaseSensitive = true,
                UseLambda = true,
                ExUseLambda = true,
                UseRegex = true,
                ExUseRegex = true,
                IsRt = true,
                IsExRt = true,
                Source = "source",
                ExSource = "exsource",
            };

            PostFilterRule newVersion;

            using (var stream = new MemoryStream())
            {
                var oldSerializer = new XmlSerializer(typeof(FiltersClass));
                oldSerializer.Serialize(stream, oldVersion);

                stream.Seek(0, SeekOrigin.Begin);

                var newSerializer = new XmlSerializer(typeof(PostFilterRule));
                newVersion = (PostFilterRule)newSerializer.Deserialize(stream);
            }

            Assert.Equal(oldVersion.NameFilter, newVersion.FilterName);
            Assert.Equal(oldVersion.ExNameFilter, newVersion.ExFilterName);
            Assert.Equal(oldVersion.BodyFilterArray, newVersion.FilterBody);
            Assert.Equal(oldVersion.ExBodyFilterArray, newVersion.ExFilterBody);
            Assert.Equal(oldVersion.SearchBoth, newVersion.UseNameField);
            Assert.Equal(oldVersion.ExSearchBoth, newVersion.ExUseNameField);
            Assert.Equal(oldVersion.MoveFrom, newVersion.MoveMatches);
            Assert.Equal(oldVersion.SetMark, newVersion.MarkMatches);
            Assert.Equal(oldVersion.SearchUrl, newVersion.FilterByUrl);
            Assert.Equal(oldVersion.ExSearchUrl, newVersion.ExFilterByUrl);
            Assert.Equal(oldVersion.CaseSensitive, newVersion.CaseSensitive);
            Assert.Equal(oldVersion.ExCaseSensitive, newVersion.ExCaseSensitive);
            Assert.Equal(oldVersion.UseLambda, newVersion.UseLambda);
            Assert.Equal(oldVersion.ExUseLambda, newVersion.ExUseLambda);
            Assert.Equal(oldVersion.UseRegex, newVersion.UseRegex);
            Assert.Equal(oldVersion.ExUseRegex, newVersion.ExUseRegex);
            Assert.Equal(oldVersion.IsRt, newVersion.FilterRt);
            Assert.Equal(oldVersion.IsExRt, newVersion.ExFilterRt);
            Assert.Equal(oldVersion.Source, newVersion.FilterSource);
            Assert.Equal(oldVersion.ExSource, newVersion.ExFilterSource);
        }
    }
}
