// OpenTween - Client of Twitter
// Copyright (c) 2012 the40san <http://sourceforge.jp/users/the40san/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    /// <summary>
    /// Bingクラスのテストクラス
    /// Translate(string _from, string _to, string _text, out string buf)のテスト未実装です
    /// </summary>
    public class BingTest
    {
        List<string> LanguageTable;
        Bing bing;

        public BingTest()
        {
            bing = new Bing();
            //リフレクション使ってインスタンスから取得するようにしたい
            #region 言語テーブル定義
            LanguageTable = new List<string>() {
            "af",
            "sq",
            "ar-sa",
            "ar-iq",
            "ar-eg",
            "ar-ly",
            "ar-dz",
            "ar-ma",
            "ar-tn",
            "ar-om",
            "ar-ye",
            "ar-sy",
            "ar-jo",
            "ar-lb",
            "ar-kw",
            "ar-ae",
            "ar-bh",
            "ar-qa",
            "eu",
            "bg",
            "be",
            "ca",
            "zh-tw",
            "zh-cn",
            "zh-hk",
            "zh-sg",
            "hr",
            "cs",
            "da",
            "nl",
            "nl-be",
            "en",
            "en-us",
            "en-gb",
            "en-au",
            "en-ca",
            "en-nz",
            "en-ie",
            "en-za",
            "en-jm",
            "en",
            "en-bz",
            "en-tt",
            "et",
            "fo",
            "fa",
            "fi",
            "fr",
            "fr-be",
            "fr-ca",
            "fr-ch",
            "fr-lu",
            "gd",
            "ga",
            "de",
            "de-ch",
            "de-at",
            "de-lu",
            "de-li",
            "el",
            "he",
            "hi",
            "hu",
            "is",
            "id",
            "it",
            "it-ch",
            "ja",
            "ko",
            "ko",
            "lv",
            "lt",
            "mk",
            "ms",
            "mt",
            "no",
            "no",
            "pl",
            "pt-br",
            "pt",
            "rm",
            "ro",
            "ro-mo",
            "ru",
            "ru-mo",
            "sz",
            "sr",
            "sr",
            "sk",
            "sl",
            "sb",
            "es",
            "es-mx",
            "es-gt",
            "es-cr",
            "es-pa",
            "es-do",
            "es-ve",
            "es-co",
            "es-pe",
            "es-ar",
            "es-ec",
            "es-cl",
            "es-uy",
            "es-py",
            "es-bo",
            "es-sv",
            "es-hn",
            "es-ni",
            "es-pr",
            "sx",
            "sv",
            "sv-fi",
            "th",
            "ts",
            "tn",
            "tr",
            "uk",
            "ur",
            "ve",
            "vi",
            "xh",
            "ji",
            "zu"
        };
        #endregion

        }

        //public bool TranslateTest(string _from, string _to, string _text, out string buf)
        //{

        //}

        [Fact]
        public void GetLanguageEnumFromIndexTest()
        {
            Assert.Equal(LanguageTable[0], bing.GetLanguageEnumFromIndex(0));
            Assert.Equal(LanguageTable[1], bing.GetLanguageEnumFromIndex(1));
            Assert.Equal(LanguageTable[LanguageTable.Count - 1], bing.GetLanguageEnumFromIndex(LanguageTable.Count - 1));
        }

        [Fact]
        public void GetIndexFromLanguageEnumTest()
        {
            Assert.Equal(0, bing.GetIndexFromLanguageEnum(LanguageTable[0]));
            Assert.Equal(1, bing.GetIndexFromLanguageEnum(LanguageTable[1]));
            Assert.Equal(LanguageTable.Count - 1, bing.GetIndexFromLanguageEnum(LanguageTable[LanguageTable.Count - 1]));
        }


    }
}
