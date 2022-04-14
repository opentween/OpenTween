// OpenTween - Client of Twitter
// Copyright (c) 2018 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Models;

namespace OpenTween
{
    public partial class MediaViewerWebBrowserDialog : OTBaseForm
    {
        public Func<IWin32Window, string, Task>? OpenInBrowser;

        private readonly MediaViewerWebBrowser model;

        public MediaViewerWebBrowserDialog(MediaViewerWebBrowser model)
        {
            this.InitializeComponent();

            this.model = model;
            this.model.SetBackColor(new ColorRGB(this.BackColor));

            this.model.PropertyChanged +=
                (s, e) => this.InvokeAsync(() => this.Model_PropertyChanged(s, e));

            this.UpdateAll();
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MediaViewerWebBrowser.MediaItems):
                case nameof(MediaViewerWebBrowser.DisplayMediaIndex):
                    this.UpdateTitle();
                    break;
                case nameof(MediaViewerWebBrowser.DisplayHTML):
                    this.UpdateHTML();
                    break;
                case "":
                case null:
                    this.UpdateAll();
                    break;
                default:
                    break;
            }
        }

        private void UpdateAll()
        {
            this.UpdateTitle();
            this.UpdateHTML();
        }

        private void UpdateTitle()
        {
            const string TITLE_TEMPLATE = "{0}/{1}";

            var mediaCount = this.model.MediaItems.Length;
            var displayIndex = this.model.DisplayMediaIndex;

            if (mediaCount == 1)
                this.Text = "";
            else
                this.Text = string.Format(TITLE_TEMPLATE, displayIndex + 1, mediaCount);
        }

        private void UpdateHTML()
        {
            using (ControlTransaction.Update(this.webBrowser))
                this.webBrowser.DocumentText = this.model.DisplayHTML;
        }

        private async void WebBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;

            switch (e.KeyData)
            {
                case Keys.Up:
                case Keys.Left:
                    this.model.SelectPreviousMedia();
                    break;
                case Keys.Down:
                case Keys.Right:
                    this.model.SelectNextMedia();
                    break;
                case Keys.Enter:
                    this.Close();
                    if (this.OpenInBrowser != null)
                        await this.OpenInBrowser.Invoke(this, this.model.DisplayMedia.MediaPageUrl);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
                default:
                    e.IsInputKey = false;
                    break;
            }
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
            => this.webBrowser.Document.GetElementById("currentMedia")?.Focus();
    }
}
