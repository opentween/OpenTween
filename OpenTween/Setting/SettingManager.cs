// OpenTween - Client of Twitter
// Copyright (c) 2017 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Connection;
using OpenTween.Thumbnail;

namespace OpenTween.Setting
{
    public class SettingManager
    {
        public static SettingManager Instance { get; internal set; } = new(null!);

        public string SettingsPath { get; set; }

        public SettingCommon Common { get; internal set; } = new();

        public SettingLocal Local { get; internal set; } = new();

        public SettingTabs Tabs { get; internal set; } = new();

        public SettingAtIdList AtIdList { get; internal set; } = new();

        /// <summary>ユーザによる設定が必要な項目が残っているか</summary>
        public bool IsIncomplete
            => MyCommon.IsNullOrEmpty(this.Common.UserName);

        public bool IsFirstRun { get; private set; } = false;

        public SettingManager(string settingsPath)
            => this.SettingsPath = settingsPath;

        public void LoadAll()
        {
            this.LoadCommon();
            this.LoadLocal();
            this.LoadTabs();
            this.LoadAtIdList();

            this.IsFirstRun = this.IsIncomplete;
        }

        public void LoadCommon()
        {
            var settings = SettingCommon.Load(this.SettingsPath);

            if (settings.UserAccounts == null || settings.UserAccounts.Count == 0)
            {
                settings.UserAccounts = new List<UserAccount>();
                if (!MyCommon.IsNullOrEmpty(settings.UserName))
                {
                    var account = new UserAccount
                    {
                        Username = settings.UserName,
                        UserId = settings.UserId,
                        Token = settings.Token,
                        TokenSecret = settings.TokenSecret,
                    };

                    settings.UserAccounts.Add(account);
                }
            }

            this.Common = settings;
        }

        public void LoadLocal()
            => this.Local = SettingLocal.Load(this.SettingsPath);

        public void LoadTabs()
            => this.Tabs = SettingTabs.Load(this.SettingsPath);

        public void LoadAtIdList()
            => this.AtIdList = SettingAtIdList.Load(this.SettingsPath);

        public void SaveAll()
        {
            this.SaveCommon();
            this.SaveLocal();
            this.SaveTabs();
            this.SaveAtIdList();
        }

        public void SaveCommon()
            => this.Common.Save(this.SettingsPath);

        public void SaveLocal()
            => this.Local.Save(this.SettingsPath);

        public void SaveTabs()
            => this.Tabs.Save(this.SettingsPath);

        public void SaveAtIdList()
            => this.AtIdList.Save(this.SettingsPath);

        public void ApplySettings()
        {
            // 静的フィールドにセットする必要のある設定値を更新
            Networking.DefaultTimeout = TimeSpan.FromSeconds(this.Common.DefaultTimeOut);
            Networking.UploadImageTimeout = TimeSpan.FromSeconds(this.Common.UploadImageTimeout);
            Networking.ForceIPv4 = this.Common.ForceIPv4;
            Networking.SetWebProxy(
                this.Local.ProxyType,
                this.Local.ProxyAddress,
                this.Local.ProxyPort,
                this.Local.ProxyUser,
                this.Local.ProxyPassword);

            TwitterApiConnection.RestApiHost = this.Common.TwitterApiHost;

            ShortUrl.Instance.DisableExpanding = !this.Common.TinyUrlResolve;
            ShortUrl.Instance.BitlyAccessToken = this.Common.BitlyAccessToken;
            ShortUrl.Instance.BitlyId = this.Common.BilyUser;
            ShortUrl.Instance.BitlyKey = this.Common.BitlyPwd;
        }

        public static string? DetermineSettingsPath(CommandLineArgs args)
        {
            if (args.TryGetValue("configDir", out var configDir) && !MyCommon.IsNullOrEmpty(configDir))
            {
                // 起動オプション /configDir で設定ファイルの参照先を変更できます
                if (!Directory.Exists(configDir))
                {
                    var text = string.Format(Properties.Resources.ConfigDirectoryNotExist, configDir);
                    MessageBox.Show(text, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                return Path.GetFullPath(configDir);
            }

            const string settingCommonFilename = "SettingCommon.xml";

            // OpenTween.exe と同じディレクトリに設定ファイルを配置する
            var startupDir = Application.StartupPath;
            var settingCommonPath = Path.Combine(startupDir, settingCommonFilename);
            if (CanWriteFile(settingCommonPath))
                return startupDir;

            // 書き込みに失敗した場合 (Program Files 以下に配置されている場合など)
            // C:\Users\ユーザー名\AppData\Roaming\OpenTween\ 以下に設定ファイルを作成する
            var roamingDir = Path.Combine(new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ApplicationSettings.ApplicationName,
            });
            Directory.CreateDirectory(roamingDir);

            /*
             * 書き込みが制限されたディレクトリ内で起動された場合の設定ファイルの扱い
             *
             *  (A) StartupPath に存在する設定ファイル
             *  (B) Roaming に存在する設定ファイル
             *
             *  1. A も B も存在しない場合
             *    => B を新規に作成する
             *
             *  2. A が存在し、B が存在しない場合
             *    => A の内容を B にコピーする (警告を表示)
             *
             *  3. A が存在せず、B が存在する場合
             *    => B を使用する
             *
             *  4. A も B も存在するが、A の方が更新日時が新しい場合
             *    => A の内容を B にコピーする (警告を表示)
             *
             *  5. A も B も存在するが、B の方が更新日時が新しい場合
             *    => B を使用する
             */
            var startupDirFile = new FileInfo(Path.Combine(startupDir, settingCommonFilename));
            var roamingDirFile = new FileInfo(Path.Combine(roamingDir, settingCommonFilename));

            if (startupDirFile.Exists && (!roamingDirFile.Exists || startupDirFile.LastWriteTime > roamingDirFile.LastWriteTime))
            {
                // StartupPath に設定ファイルが存在し、Roaming 内のファイルよりも新しい場合は警告を表示してファイルをコピーする
                var message = string.Format(Properties.Resources.SettingPath_Relocation, startupDir, roamingDir);
                MessageBox.Show(message, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                var settings = new SettingManager(startupDir);
                settings.LoadAll();
                settings.SettingsPath = roamingDir;
                settings.SaveAll();
            }

            return roamingDir;
        }

        private static bool CanWriteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    using var stream = File.OpenWrite(path);
                    return true;
                }
                else
                {
                    using (var stream = File.OpenWrite(path))
                    {
                    }
                    File.Delete(path);
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
