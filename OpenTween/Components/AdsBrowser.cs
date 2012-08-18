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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Timers;

namespace OpenTween.OpenTweenCustomControl
{
    public class AdsBrowser : WebBrowser
    {
        private string adsPath;
        private System.Timers.Timer refreshTimer;

        public AdsBrowser() : base()
        {
            adsPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.WriteAllText(adsPath, Properties.Resources.ads);

            this.Size = new Size(728 + 5, 90);
            this.ScrollBarsEnabled = false;
            this.AllowWebBrowserDrop = false;
            this.IsWebBrowserContextMenuEnabled = false;
            this.ScriptErrorsSuppressed = true;
            this.TabStop = false;
            this.WebBrowserShortcutsEnabled = false;
            this.Dock = DockStyle.Fill;
            this.Visible = false;
            this.Navigate(adsPath);
            this.Visible = true;

            this.refreshTimer = new System.Timers.Timer(45 * 1000);
            this.refreshTimer.AutoReset = true;
            this.refreshTimer.SynchronizingObject = this;
            this.refreshTimer.Enabled = true;

            this.Disposed += this.AdsBrowser_Disposed;
            this.refreshTimer.Elapsed += this.refreshTimer_Elapsed;
        }

        private void AdsBrowser_Disposed(object sender, EventArgs e)
        {
            this.refreshTimer.Dispose();
            File.Delete(adsPath);
        }

        private void refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Visible = false;
            this.Refresh();
            this.Visible = true;
        }
    }
}
