// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System.Drawing;
using Xunit;

namespace OpenTween
{
    public class ThemeManagerTest
    {
        [Fact]
        public void FontDefaultTest()
        {
            var settings = new SettingLocal();
            using var themeManager = new ThemeManager(settings);
            Assert.True(themeManager.FontDetail.IsSystemFont);
            Assert.Equal(nameof(SystemFonts.DefaultFont), themeManager.FontDetail.SystemFontName);
        }

        [Fact]
        public void FontCustomTest()
        {
            var settings = new SettingLocal
            {
                FontDetailStr = "Arial, 9pt",
            };
            using var themeManager = new ThemeManager(settings);
            Assert.False(themeManager.FontDetail.IsSystemFont);
            Assert.Equal("Arial", themeManager.FontDetail.OriginalFontName);
            Assert.Equal(9, themeManager.FontDetail.SizeInPoints);
        }

        [Fact]
        public void ColorDefaultTest()
        {
            var settings = new SettingLocal();
            using var themeManager = new ThemeManager(settings);
            Assert.True(themeManager.ColorDetail.IsSystemColor);
            Assert.Equal(nameof(KnownColor.ControlText), themeManager.ColorDetail.Name);
        }

        [Fact]
        public void ColorCustomTest()
        {
            var settings = new SettingLocal
            {
                ColorDetailStr = "0, 100, 200",
            };
            using var themeManager = new ThemeManager(settings);
            Assert.False(themeManager.ColorDetail.IsSystemColor);
            Assert.Equal(0, themeManager.ColorDetail.R);
            Assert.Equal(100, themeManager.ColorDetail.G);
            Assert.Equal(200, themeManager.ColorDetail.B);
        }
    }
}
