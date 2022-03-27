// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenTween
{
    public static class RegexAsync
    {
        public static async Task<string> ReplaceAsync(this Regex regex, string input, Func<Match, Task<string>> evaluator)
        {
            var matches = regex.Matches(input).Cast<Match>();

            var evaluatorTasks = matches
                .GroupBy(x => x.Value) // ToDictionaryする際に key が重複しないようにする
                .Select(async x => new
                {
                    x.Key,
                    Value = await evaluator(x.First()).ConfigureAwait(false),
                });

            var replacements = (await Task.WhenAll(evaluatorTasks).ConfigureAwait(false))
                .ToDictionary(x => x.Key, x => x.Value);

            return regex.Replace(input, x => replacements[x.Value]);
        }
    }
}
