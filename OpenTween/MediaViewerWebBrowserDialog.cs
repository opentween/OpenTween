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

using System.ComponentModel;
using OpenTween.Models;

namespace OpenTween
{
    public partial class MediaViewerWebBrowserDialog : OTBaseForm
    {
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
            this.UpdateHTML();
        }

        private void UpdateHTML()
            => this.webBrowser.DocumentText = this.model.DisplayHTML;
    }
}
