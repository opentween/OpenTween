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

using System;
using System.Drawing;

namespace OpenTween.Models
{
    public class DetailsHtmlBuilder
    {
        private const string TemplateHead =
            """<head><meta http-equiv="X-UA-Compatible" content="IE=8" />"""
            + """<style type="text/css"><!-- """
            + "body, p, pre {margin: 0;} "
            + """body {font-family: "%FONT_FAMILY%", "Segoe UI Emoji", sans-serif; font-size: %FONT_SIZE%pt; background-color:rgb(%BG_COLOR%); word-wrap: break-word; color:rgb(%FONT_COLOR%);} """
            + "pre {font-family: inherit;} "
            + "a:link, a:visited, a:active, a:hover {color:rgb(%LINK_COLOR%); } "
            + "img.emoji {width: 1em; height: 1em; margin: 0 .05em 0 .1em; vertical-align: -0.1em; border: none;} "
            + ".quote-tweet {border: 1px solid #ccc; margin: 1em; padding: 0.5em;} "
            + ".quote-tweet.reply {border-color: rgb(%BG_REPLY_COLOR%);} "
            + ".quote-tweet-link {color: inherit !important; text-decoration: none;}"
            + "--></style>"
            + "</head>";

        private const string TemplateMonospaced =
            $"<html>{TemplateHead}<body><pre>%CONTENT_HTML%</pre></body></html>";

        private const string TemplateProportional =
            $"<html>{TemplateHead}<body><p>%CONTENT_HTML%</p></body></html>";

        private string? preparedTemplate = null;

        public void Prepare(SettingCommon settingCommon, ThemeManager theme)
        {
            var htmlTemplate = settingCommon.IsMonospace ? TemplateMonospaced : TemplateProportional;

            static string ColorToRGBString(Color color)
                => $"{color.R},{color.G},{color.B}";

            this.preparedTemplate = htmlTemplate
                .Replace("%FONT_FAMILY%", theme.FontDetail.Name)
                .Replace("%FONT_SIZE%", theme.FontDetail.Size.ToString())
                .Replace("%FONT_COLOR%", ColorToRGBString(theme.ColorDetail))
                .Replace("%LINK_COLOR%", ColorToRGBString(theme.ColorDetailLink))
                .Replace("%BG_COLOR%", ColorToRGBString(theme.ColorDetailBackcolor))
                .Replace("%BG_REPLY_COLOR%", ColorToRGBString(theme.ColorAtTo));
        }

        public string Build(string contentHtml)
        {
            if (this.preparedTemplate == null)
                throw new InvalidOperationException("Template is not prepared.");

            return this.preparedTemplate.Replace("%CONTENT_HTML%", contentHtml);
        }
    }
}
