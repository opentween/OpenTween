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

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace OpenTween
{
    public abstract class SettingBase<T>
        where T : class, new()
    {
        /// <summary>
        /// XML に含まれる追加データを保持するフィールド
        /// </summary>
        /// <remarks>
        /// 他バージョンから設定をコピーした場合など、クラスに存在しないフィールドが
        /// XML に含まれていた場合に破棄せず保持するため必要となる。
        /// </remarks>
        [XmlAnyElement]
        public XmlElement[] ExtraElements = Array.Empty<XmlElement>();

        private static readonly object LockObj = new();

        protected static T LoadSettings(string settingsPath, string fileId)
        {
            try
            {
                var settingFilePath = GetSettingFilePath(settingsPath, fileId);
                if (!File.Exists(settingFilePath))
                {
                    return new T();
                }

                lock (LockObj)
                {
                    using var fs = new FileStream(settingFilePath, FileMode.Open, FileAccess.Read);
                    fs.Position = 0;
                    var xs = new XmlSerializer(typeof(T));
                    var instance = (T)xs.Deserialize(fs);
                    return instance;
                }
            }
            catch (FileNotFoundException)
            {
                return new T();
            }
            catch (Exception)
            {
                var backupFile = Path.Combine(
                        Path.Combine(
                            Application.StartupPath,
                            ApplicationSettings.AssemblyName + "Backup1st"),
                        typeof(T).Name + fileId + ".xml");
                if (File.Exists(backupFile))
                {
                    try
                    {
                        lock (LockObj)
                        {
                            using var fs = new FileStream(backupFile, FileMode.Open, FileAccess.Read);
                            fs.Position = 0;
                            var xs = new XmlSerializer(typeof(T));
                            var instance = (T)xs.Deserialize(fs);
                            MessageBox.Show("File: " + GetSettingFilePath(settingsPath, fileId) + Environment.NewLine + "Use old setting file, because application can't read this setting file.");
                            return instance;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                MessageBox.Show("File: " + GetSettingFilePath(settingsPath, fileId) + Environment.NewLine + "Use default setting, because application can't read this setting file.");
                return new T();
            }
        }

        protected static T LoadSettings(string settingsPath)
            => LoadSettings(settingsPath, "");

        protected static void SaveSettings(T instance, string settingsPath, string fileId)
        {
            const int SaveRetryMax = 3;

            if (instance == null)
                return;

            var retryCount = 0;
            Exception? lastException = null;

            var filePath = GetSettingFilePath(settingsPath, fileId);
            do
            {
                var tmpfilePath = GetSettingFilePath(settingsPath, "_" + Path.GetRandomFileName());
                try
                {
                    lock (LockObj)
                    {
                        using (var stream = new FileStream(tmpfilePath, FileMode.Create, FileAccess.Write))
                        {
                            var serializer = new XmlSerializer(typeof(T));
                            serializer.Serialize(stream, instance);
                            stream.Flush();
                        }

                        var fileInfo = new FileInfo(tmpfilePath);
                        if (fileInfo.Length != 0)
                        {
                            // 成功
                            File.Copy(tmpfilePath, filePath, true);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    lastException = e;
                }
                finally
                {
                    try
                    {
                        if (File.Exists(tmpfilePath))
                            File.Delete(tmpfilePath);
                    }
                    catch (Exception)
                    {
                    }
                }

                // リトライ
                retryCount++;
                Thread.Sleep(1000);
            }
            while (retryCount <= SaveRetryMax);

            // リトライオーバー
            if (lastException != null)
                MyCommon.ExceptionOut(lastException);

            MessageBox.Show("Can't write setting XML.(" + filePath + ")", "Save Settings", MessageBoxButtons.OK);
        }

        protected static void SaveSettings(T instance, string settingsPath)
            => SaveSettings(instance, settingsPath, "");

        public static string GetSettingFilePath(string settingsPath, string fileId)
            => Path.Combine(settingsPath, typeof(T).Name + fileId + ".xml");
    }
}
