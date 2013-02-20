// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
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
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Threading;

namespace OpenTween
{
    abstract public class SettingBase<T> where T : class, new()
    {

        private static object lockObj = new object();

        protected static T LoadSettings(string FileId)
        {
            try
            {
                var settingFilePath = GetSettingFilePath(FileId);
                if (!File.Exists(settingFilePath))
                {
                    return new T();
                }

                lock (lockObj)
                {
                    using (FileStream fs = new FileStream(settingFilePath, FileMode.Open))
                    {
                        fs.Position = 0;
                        XmlSerializer xs = new XmlSerializer(typeof(T));
                        T instance = (T)xs.Deserialize(fs);
                        return instance;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return new T();
            }
            catch (Exception)
            {
                string backupFile = Path.Combine(
                        Path.Combine(
                            Application.StartupPath,
                            MyCommon.GetAssemblyName() + "Backup1st"),
                        typeof(T).Name + FileId + ".xml");
                if (File.Exists(backupFile))
                {
                    try
                    {
                        lock (lockObj)
                        {
                            using (FileStream fs = new FileStream(backupFile, FileMode.Open))
                            {
                                fs.Position = 0;
                                XmlSerializer xs = new XmlSerializer(typeof(T));
                                T instance = (T)xs.Deserialize(fs);
                                MessageBox.Show("File: " + GetSettingFilePath(FileId) + Environment.NewLine + "Use old setting file, because application can't read this setting file.");
                                return instance;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                MessageBox.Show("File: " + GetSettingFilePath(FileId) + Environment.NewLine + "Use default setting, because application can't read this setting file.");
                return new T();
                //ex.Data.Add("FilePath", GetSettingFilePath(FileId));
                //FileInfo fi = new IO.FileInfo(GetSettingFilePath(FileId));
                //ex.Data.Add("FileSize", fi.Length.ToString());
                //ex.Data.Add("FileData", File.ReadAllText(GetSettingFilePath(FileId)));
                //throw;
            }
        }

        protected static T LoadSettings()
        {
            return LoadSettings("");
        }

        protected static void SaveSettings(T Instance, string FileId)
        {
            int cnt = 0;
            bool err = false;
            string fileName = GetSettingFilePath(FileId);
            if (Instance == null) return;
            do
            {
                err = false;
                cnt += 1;
                try
                {
                    lock (lockObj)
                    {
                        using (FileStream fs = new FileStream(fileName, FileMode.Create))
                        {
                            fs.Position = 0;
                            XmlSerializer xs = new XmlSerializer(typeof(T));
                            xs.Serialize(fs, Instance);
                            fs.Flush();
                        }
                        FileInfo fi = new FileInfo(fileName);
                        if (fi.Length == 0)
                        {
                            if (cnt > 3)
                            {
                                throw new Exception();
                            }
                            Thread.Sleep(1000);
                            err = true;
                        }
                    }
                }
                catch (Exception)
                {
                    //検証エラー or 書き込みエラー
                    if (cnt > 3)
                    {
                        //リトライオーバー
                        MessageBox.Show("Can't write setting XML.(" + fileName + ")", "Save Settings", MessageBoxButtons.OK);
                        //throw new System.InvalidOperationException("Can't write setting XML.(" + fileName + ")");
                        return;
                    }
                    //リトライ
                    Thread.Sleep(1000);
                    err = true;
                }
            } while (err);
        }

        protected static void SaveSettings(T Instance)
        {
            SaveSettings(Instance, "");
        }

        public static string GetSettingFilePath(string FileId)
        {
            return Path.Combine(MyCommon.settingPath, typeof(T).Name + FileId + ".xml");
        }
    }
}
