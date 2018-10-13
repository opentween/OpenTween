// OpenTween - Client of Twitter
// Copyright (c) 2015 takke (@takke) <http://takke.jp/>
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenTween
{
    /// <summary>
    /// ツイート内の絵文字を img タグに書き換えるクラス
    /// </summary>
    public static class EmojiFormatter
    {
        public static string ReplaceEmojiToImg(string text)
        {
            // HTMLタグを壊さないように <...> の外側のみを置換する
            return Regex.Replace(text, @"^[^<]+$|^[^<]+(?:<)|(?:>)[^<]+(?:<)|(?:>)[^<]+$", m =>
            {
                return TweetExtractor.EmojiPattern.Replace(m.Value, ReplaceEmojiEntity);
            });
        }

        private static string ReplaceEmojiEntity(System.Text.RegularExpressions.Match m)
        {
            string input = m.Value;
            string codes = "";

            // 異字体セレクタ U+FE0F (emoji style) は除去する (ZWJ を含まない場合のみ)
            if (!input.Contains('\u200D'))
                input = input.Replace("\uFE0F", "");

            for (var i = 0; i < input.Length; i += char.IsSurrogatePair(input, i) ? 2 : 1)
            {
                var codepoint = char.ConvertToUtf32(input, i);

                if (i > 0)
                {
                    codes += "-";
                }

                codes += string.Format("{0:x}", codepoint);
            }

            return "<img class=\"emoji\" src=\"https://twemoji.maxcdn.com/2/72x72/" + codes + ".png\" alt=\"" + input + "\" />";
        }
    }
}
