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
using System.Windows.Forms;
using OpenTween.Models;

namespace OpenTween
{
    public partial class MediaViewerLightDialog : OTBaseForm
    {
        private readonly MediaViewerLight model;

        public MediaViewerLightDialog(MediaViewerLight model)
        {
            this.InitializeComponent();

            this.model = model;
            this.model.PropertyChanged +=
                (s, e) => this.InvokeAsync(() => this.Model_PropertyChanged(s, e));

            this.UpdateAll();
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MediaViewerLight.LoadState):
                    this.UpdateLoadState();
                    break;
                case nameof(MediaViewerLight.ImageSize):
                case nameof(MediaViewerLight.ReceivedSize):
                    this.UpdateLoadProgress();
                    break;
                case nameof(MediaViewerLight.Image):
                    this.UpdateImage();
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
            this.UpdateLoadState();
            this.UpdateLoadProgress();
            this.UpdateImage();
        }

        private void UpdateLoadState()
        {
            switch (this.model.LoadState)
            {
                case MediaViewerLight.LoadStateEnum.BeforeLoad:
                case MediaViewerLight.LoadStateEnum.HeaderArrived:
                    this.progressBar.Visible = true;
                    this.pictureBox.Visible = false;
                    break;
                case MediaViewerLight.LoadStateEnum.LoadSuccessed:
                    this.progressBar.Visible = false;
                    this.pictureBox.Visible = true;
                    break;
                case MediaViewerLight.LoadStateEnum.LoadError:
                    this.progressBar.Visible = false;
                    this.pictureBox.Visible = true;
                    this.pictureBox.ShowErrorImage();
                    break;
                default:
                    break;
            }
        }

        private void UpdateLoadProgress()
        {
            if (this.model.ImageSize != null && this.model.ReceivedSize != null)
            {
                this.progressBar.Maximum = (int?)this.model.ImageSize ?? 0;
                this.progressBar.Value = (int?)this.model.ReceivedSize ?? 0;
                this.progressBar.Style = ProgressBarStyle.Continuous;
            }
            else
            {
                this.progressBar.Style = ProgressBarStyle.Marquee;
            }
        }

        private void UpdateImage()
            => this.pictureBox.Image = this.model.Image;

        private void MediaViewerLightDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                this.Close();
        }
    }
}
