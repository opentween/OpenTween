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
using System.Net;
using OpenTween.Thumbnail;

namespace OpenTween.Models
{
    public class MediaViewerWebBrowser : NotifyPropertyChangedBase
    {
        private ThumbnailInfo? displayMedia;
        private string displayHTML = "";
        private ColorRGB backColor = new ColorRGB(0, 0, 0);

        public ThumbnailInfo DisplayMedia
        {
            get => this.displayMedia ?? throw new InvalidOperationException("DispalyMedia is not set");
            private set => this.SetProperty(ref this.displayMedia, value);
        }

        public string DisplayHTML
        {
            get => this.displayHTML;
            private set => this.SetProperty(ref this.displayHTML, value);
        }

        public ColorRGB BackColor
        {
            get => this.backColor;
            private set => this.SetProperty(ref this.backColor, value);
        }

        public void SetMediaItem(ThumbnailInfo media)
        {
            this.DisplayMedia = media;
            this.DisplayHTML = this.CreateDocument();
        }

        public void SetBackColor(ColorRGB color)
        {
            this.BackColor = color;
            this.DisplayHTML = this.CreateDocument();
        }

        private string CreateDocument()
        {
            const string TEMPLATE_HEAD = @"
<!DOCTYPE html>
<meta charset='utf-8'/>
<meta http-equiv='X-UA-Compatible' content='IE=edge'/>
<title>MediaViewerWebBrowserDialog</title>
<style>
html, body {
  height: 100%;
  margin: 0;
}
.media-panel {
  height: 100%;
  background: rgb(###BG_COLOR###);
}
.media-image {
  height: 100%;
  background-size: contain;
  background-position: center;
  background-repeat: no-repeat;
}
.media-video {
  width: 100%;
  height: 100%;
}
</style>
";
            var bgColor = this.BackColor;
            var html = TEMPLATE_HEAD
                .Replace("###BG_COLOR###", $"{bgColor.R},{bgColor.G},{bgColor.B}");

            var media = this.DisplayMedia;

            if (media.VideoUrl != null)
            {
                const string TEMPLATE_VIDEO_BODY = @"
<div class='media-panel'>
  <video class='media-video' preload='metadata' controls>
    <source src='###VIDEO_URI###' type='video/mp4'/>
  </video>
</div>
";
                html += TEMPLATE_VIDEO_BODY
                    .Replace("###VIDEO_URI###", WebUtility.HtmlEncode(media.VideoUrl));
            }
            else
            {
                const string TEMPLATE_IMAGE_BODY = @"
<div class='media-panel media'>
  <div class='media-image' style='background-image: url(###IMAGE_URI###)'></div>
</div>
";
                html += TEMPLATE_IMAGE_BODY
                    .Replace("###IMAGE_URI###", WebUtility.HtmlEncode(Uri.EscapeUriString(media.FullSizeImageUrl ?? media.ThumbnailImageUrl ?? "")));
            }

            return html;
        }
    }
}
