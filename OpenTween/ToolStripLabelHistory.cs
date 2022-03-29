// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenTween.OpenTweenCustomControl
{
    public class ToolStripLabelHistory : ToolStripStatusLabel
    {
        public enum LogLevel
        {
            Lowest = 0,
            Debug = 16,
            Info = 32,
            Notice = 64,
            Warn = 128,
            Err = 192,
            Fatal = 255,
            Highest = 256,
        }

        public readonly record struct LogEntry(
            LogLevel LogLevel,
            DateTimeUtc Timestamp,
            string Summary,
            string Detail
        )
        {
            public LogEntry(DateTimeUtc timestamp, string summary)
                : this(LogLevel.Debug, timestamp, summary, summary)
            {
            }

            public override string ToString()
                => this.Timestamp.ToLocalTime().ToString("T") + ": " + this.Summary;
        }

        private readonly LinkedList<LogEntry> logs;

        private const int MAXCNT = 20;

        public override string Text
        {
            get => base.Text;
            set
            {
                var oneline = value.Replace("\n", " ");
                this.logs.AddLast(new LogEntry(DateTimeUtc.Now, oneline));
                while (this.logs.Count > MAXCNT)
                {
                    this.logs.RemoveFirst();
                }
                base.Text = oneline;
            }
        }

        public string TextHistory
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var e in this.logs)
                {
                    sb.AppendLine(e.ToString());
                }
                return sb.ToString();
            }
        }

        public ToolStripLabelHistory()
            => this.logs = new LinkedList<LogEntry>();
    }
}
