using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO;

namespace Tween
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
