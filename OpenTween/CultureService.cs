// OpenTween - Client of Twitter
// Copyright (c) 2007-2012 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2012 Moz (@syo68k)
//           (c) 2008-2012 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2012 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2012 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2012      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
//           (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

#nullable enable

using System.Globalization;
using System.Linq;
using System.Threading;

namespace OpenTween
{
    public class CultureService
    {
        public static readonly CultureInfo[] SupportedUICulture = new[]
        {
            new CultureInfo("en"), // 先頭のカルチャはフォールバック先として使用される
            new CultureInfo("ja"),
        };

        private readonly SettingCommon settingCommon;

        public CultureService(SettingCommon settingCommon)
            => this.settingCommon = settingCommon;

        public void Initialize()
        {
            var settingCultureStr = this.settingCommon.Language;
            if (settingCultureStr == "OS")
                return;

            try
            {
                var selectedCulture = new CultureInfo(settingCultureStr);

                var preferredCulture = GetPreferredCulture(selectedCulture);
                CultureInfo.DefaultThreadCurrentUICulture = preferredCulture;
                Thread.CurrentThread.CurrentUICulture = preferredCulture;
            }
            catch (CultureNotFoundException)
            {
            }
        }

        /// <summary>
        /// サポートしているカルチャの中から、指定されたカルチャに対して適切なカルチャを選択して返します
        /// </summary>
        public static CultureInfo GetPreferredCulture(CultureInfo culture)
        {
            if (SupportedUICulture.Any(x => x.Contains(culture)))
                return culture;

            return SupportedUICulture[0];
        }
    }
}
