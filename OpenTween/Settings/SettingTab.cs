// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace OpenTween
{
    [Serializable]
    public class SettingTab : SettingBase<SettingTab>
    {
#region Settingクラス基本
        public static SettingTab Load(string tabName)
        {
            SettingTab setting = LoadSettings(tabName);
            setting.Tab.TabName = tabName;
            return setting;
        }

        public void Save()
        {
            SaveSettings(this, this.Tab.TabName);
        }

        public SettingTab()
        {
            Tab = new TabClass();
        }

        public SettingTab(string TabName)
        {
            this.Tab = new TabClass();
            Tab.TabName = TabName;
        }
#endregion

        public static void DeleteConfigFile()
        {
            foreach (FileInfo file in (new DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar)).GetFiles("SettingTab*.xml"))
            {
                try
                {
                    file.Delete();
                }
                catch (Exception)
                {
                    //削除権限がない場合
                }
            }
        }

        public TabClass Tab;
    }
}
