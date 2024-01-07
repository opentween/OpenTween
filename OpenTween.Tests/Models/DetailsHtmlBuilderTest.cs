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

using System.Xml.Linq;
using System.Xml.XPath;
using Xunit;

namespace OpenTween.Models
{
    public class DetailsHtmlBuilderTest
    {
        [Fact]
        public void Build_Test()
        {
            var settingCommon = new SettingCommon();
            var settingLocal = new SettingLocal();
            using var theme = new ThemeManager(settingLocal);

            var builder = new DetailsHtmlBuilder();
            builder.Prepare(settingCommon, theme);

            var actualHtml = builder.Build("tetete");
            var parsedDocument = XDocument.Parse(actualHtml);
            Assert.Equal("tetete", parsedDocument.XPathSelectElement("/html/body/p").Value);
        }
    }
}
