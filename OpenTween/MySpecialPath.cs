// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO;

namespace OpenTween
{
    public class MySpecialPath
    {
        //public static string UserAppDataPath
        //{
        //    get
        //    {
        //        return GetFileSystemPath(Environment.SpecialFolder.ApplicationData);
        //    }
        //}
        public static string UserAppDataPath()
        {
            return GetFileSystemPath(Environment.SpecialFolder.ApplicationData);
        }

        //public static string UserAppDataPath(string productName)
        //{
        //    get
        //    {
        //        return GetFileSystemPath(Environment.SpecialFolder.ApplicationData, productName);
        //    }
        //}
        public static string UserAppDataPath(string productName)
        {
            return GetFileSystemPath(Environment.SpecialFolder.ApplicationData, productName);
        }

        public static string CommonAppDataPath
        {
            get
            {
                return GetFileSystemPath(Environment.SpecialFolder.CommonApplicationData);
            }
        }

        public static string LocalUserAppDataPath
        {
            get
            {
                return GetFileSystemPath(Environment.SpecialFolder.LocalApplicationData);
            }
        }

        public static RegistryKey CommonAppDataRegistry
        {
            get
            {
                return GetRegistryPath(Registry.LocalMachine);
            }
        }

        public static RegistryKey UserAppDataRegistry
        {
            get
            {
                return GetRegistryPath(Registry.CurrentUser);
            }
        }


        private static string GetFileSystemPath(Environment.SpecialFolder folder)
        {
            // パスを取得
            var path = string.Format("{0}{3}{1}{3}{2}",
                Environment.GetFolderPath(folder),
                Application.CompanyName,
                Application.ProductName,
                Path.DirectorySeparatorChar.ToString());

            // パスのフォルダを作成
            lock (typeof(Application))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            return path;
        }

        private static string GetFileSystemPath(Environment.SpecialFolder folder, string productName)
        {
            // パスを取得
            var path = string.Format("{0}{3}{1}{3}{2}",
                Environment.GetFolderPath(folder),
                Application.CompanyName,
                productName,
                Path.DirectorySeparatorChar.ToString());

            // パスのフォルダを作成
            lock (typeof(Application))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            return path;
        }

        private static RegistryKey GetRegistryPath(RegistryKey key)
        {
            // パスを取得
            string basePath;
            if (key == Registry.LocalMachine)
            {
                basePath = "SOFTWARE";
            }
            else
            {
                basePath = "Software";
            }
            var path = string.Format(@"{0}\{1}\{2}",
                basePath,
                Application.CompanyName,
                Application.ProductName);
            // パスのレジストリ・キーの取得（および作成）
            return key.CreateSubKey(path);
        }
    }
}
