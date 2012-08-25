using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenTween.Thumbnail.Services
{
    class Foursquare : IThumbnailService
    {
        protected Regex regex;

        public Foursquare(string pattern)
        {
            this.regex = new Regex(pattern);
        }

        public override ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
        {
            var match = this.regex.Match(url);

            if (!match.Success) return null;
            if (!AppendSettingDialog.Instance.IsPreviewFoursquare) return null;
            if (post.PostGeo.Lat != 0 | post.PostGeo.Lng != 0) return null;

            var tipsText = "";
            var mapUrl = OpenTween.Foursquare.GetInstance.GetMapsUri(url, ref tipsText);

            if (mapUrl == null) return null;

            return new ThumbnailInfo()
            {
                ImageUrl = url,
                ThumbnailUrl = mapUrl,
                TooltipText = tipsText,
            };
        }
    }
}
