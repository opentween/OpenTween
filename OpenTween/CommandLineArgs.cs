// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenTween
{
    public class CommandLineArgs : IReadOnlyDictionary<string, string>
    {
        private readonly Dictionary<string, string> parsedArgs;

        public CommandLineArgs(string[] args)
            => this.parsedArgs = this.ParseArguments(args);

        public string this[string key]
            => this.parsedArgs[key];

        public IEnumerable<string> Keys
            => this.parsedArgs.Keys;

        public IEnumerable<string> Values
            => this.parsedArgs.Values;

        public int Count
            => this.parsedArgs.Count;

        public bool ContainsKey(string key)
            => this.parsedArgs.ContainsKey(key);

#pragma warning disable CS8767
        public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
            => this.parsedArgs.TryGetValue(key, out value);
#pragma warning restore CS8767

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => this.parsedArgs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        /// <summary>
        /// “/key:value”形式の起動オプションを解釈し Dictionary に変換する
        /// </summary>
        /// <remarks>
        /// 不正な形式のオプションは除外されます。
        /// また、重複したキーのオプションが入力された場合は末尾に書かれたオプションが採用されます。
        /// </remarks>
        private Dictionary<string, string> ParseArguments(string[] arguments)
        {
            var optionPattern = new Regex(@"^/(.+?)(?::(.*))?$");

            return arguments.Select(x => optionPattern.Match(x))
                .Where(x => x.Success)
                .GroupBy(x => x.Groups[1].Value)
                .ToDictionary(x => x.Key, x => x.Last().Groups[2].Value);
        }
    }
}
