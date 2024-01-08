// OpenTween - Client of Twitter
// Copyright (c) 2024 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

#nullable enable

using System.Collections.Generic;

namespace OpenTween.Models
{
    public class StatusTextHistory
    {
        public readonly record struct HistoryItem(
            string Status,
            (PostId StatusId, string ScreenName)? InReplyTo = null
        );

        internal IReadOnlyList<HistoryItem> Items
            => this.items;

        internal int HistoryIndex
            => this.historyIndex;

        private readonly List<HistoryItem> items = new();
        private int historyIndex = 0;

        public StatusTextHistory()
            => this.items.Add(new(""));

        public void SetCurrentItem(string text, (PostId StatusId, string ScreenName)? inReplyTo)
        {
            if (!string.IsNullOrWhiteSpace(text))
                this.items[this.historyIndex] = new(text, inReplyTo);
        }

        public HistoryItem Back()
        {
            if (this.historyIndex > 0)
                this.historyIndex--;

            return this.items[this.historyIndex];
        }

        public HistoryItem Forward()
        {
            if (this.historyIndex < this.items.Count - 1)
                this.historyIndex++;

            return this.items[this.historyIndex];
        }

        public void AddLast(string text, (PostId StatusId, string ScreenName)? inReplyTo)
        {
            this.historyIndex = this.items.Count - 1;
            this.SetCurrentItem(text, inReplyTo);

            this.items.Add(new(""));
            this.historyIndex++;
        }

        public HistoryItem? Peek()
        {
            if (this.items.Count < 2)
                return null;

            return this.items[this.items.Count - 2];
        }
    }
}
