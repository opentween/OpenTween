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
using NUnit.Framework;

namespace OpenTween
{
    /// <summary>
    /// FiltersClass -> PostFilterRule クラスへの変更で v1.1.2 までの
    /// 設定ファイルと互換性が保たれているかどうかを確認するテスト
    /// </summary>
    [TestFixture]
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

        [Test]
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

            Assert.That(newVersion.FilterName, Is.EqualTo(oldVersion.NameFilter));
            Assert.That(newVersion.ExFilterName, Is.EqualTo(oldVersion.ExNameFilter));
            Assert.That(newVersion.FilterBody, Is.EqualTo(oldVersion.BodyFilterArray));
            Assert.That(newVersion.ExFilterBody, Is.EqualTo(oldVersion.ExBodyFilterArray));
            Assert.That(newVersion.UseNameField, Is.EqualTo(oldVersion.SearchBoth));
            Assert.That(newVersion.ExUseNameField, Is.EqualTo(oldVersion.ExSearchBoth));
            Assert.That(newVersion.MoveMatches, Is.EqualTo(oldVersion.MoveFrom));
            Assert.That(newVersion.MarkMatches, Is.EqualTo(oldVersion.SetMark));
            Assert.That(newVersion.FilterByUrl, Is.EqualTo(oldVersion.SearchUrl));
            Assert.That(newVersion.ExFilterByUrl, Is.EqualTo(oldVersion.ExSearchUrl));
            Assert.That(newVersion.CaseSensitive, Is.EqualTo(oldVersion.CaseSensitive));
            Assert.That(newVersion.ExCaseSensitive, Is.EqualTo(oldVersion.ExCaseSensitive));
            Assert.That(newVersion.UseLambda, Is.EqualTo(oldVersion.UseLambda));
            Assert.That(newVersion.ExUseLambda, Is.EqualTo(oldVersion.ExUseLambda));
            Assert.That(newVersion.UseRegex, Is.EqualTo(oldVersion.UseRegex));
            Assert.That(newVersion.ExUseRegex, Is.EqualTo(oldVersion.ExUseRegex));
            Assert.That(newVersion.FilterRt, Is.EqualTo(oldVersion.IsRt));
            Assert.That(newVersion.ExFilterRt, Is.EqualTo(oldVersion.IsExRt));
            Assert.That(newVersion.FilterSource, Is.EqualTo(oldVersion.Source));
            Assert.That(newVersion.ExFilterSource, Is.EqualTo(oldVersion.ExSource));
        }
    }
}
