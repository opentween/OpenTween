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
using OpenTween.Thumbnail;

namespace OpenTween
{
    public enum MediaHandlerType
    {
        /// <summary>外部ブラウザで開く</summary>
        ExternalBrowser,

        /// <summary>軽量ビューアーで開く</summary>
        LightViewer,
    }

    public class MediaHandler
    {
        public MediaHandlerType MediaHandlerType { get; set; }

        public Func<IWin32Window, string, Task>? OpenInBrowser { get; set; }

        public async Task OpenMediaViewer(IWin32Window owner, ThumbnailInfo[] thumbnails, int displayIndex)
        {
            switch (this.MediaHandlerType)
            {
                case MediaHandlerType.ExternalBrowser:
                    await this.OpenMediaInExternalBrowser(owner, thumbnails, displayIndex);
                    break;
                case MediaHandlerType.LightViewer:
                    await this.OpenMediaInLightViewer(owner, thumbnails, displayIndex);
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public async Task OpenMediaInExternalBrowser(IWin32Window owner, ThumbnailInfo[] thumbnails, int displayIndex)
        {
            var mediaUrl = thumbnails[displayIndex].MediaPageUrl;
            if (this.OpenInBrowser != null)
                await this.OpenInBrowser(owner, mediaUrl);
        }

        public async Task OpenMediaInLightViewer(IWin32Window owner, ThumbnailInfo[] thumbnails, int displayIndex)
        {
            using var viewer = new MediaViewerLight();
            using var viewerDialog = new MediaViewerLightDialog(viewer);

            viewer.SetMediaItems(thumbnails);
            var loadTask = Task.Run(() => viewer.SelectMedia(displayIndex));

            viewerDialog.OpenInBrowser = this.OpenInBrowser;
            viewerDialog.ShowDialog(owner);

            await loadTask;
        }
    }
}
