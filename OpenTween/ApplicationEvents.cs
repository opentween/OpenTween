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

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using OpenTween.Setting;

namespace OpenTween
{
    internal class ApplicationEvents
    {
        public static readonly CultureInfo[] SupportedUICulture = new[]
        {
            new CultureInfo("en"), // 先頭のカルチャはフォールバック先として使用される
            new CultureInfo("ja"),
        };

        /// <summary>
        /// 起動時に指定されたオプションを取得します
        /// </summary>
        public static CommandLineArgs StartupOptions { get; private set; } = null!;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using var errorReportHandler = new ErrorReportHandler();

            StartupOptions = new(args);

            WarnIfApiKeyError();
            WarnIfRunAsAdministrator();

            if (!CheckRuntimeVersion())
            {
                var message = string.Format(Properties.Resources.CheckRuntimeVersion_Error, ".NET Framework 4.7.2");
                MessageBox.Show(message, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            if (!SetConfigDirectoryPath())
                return 1;

            SettingManager.LoadAll();

            InitCulture();
            {
                // 同じ設定ファイルを使用する OpenTween プロセスの二重起動を防止する
                var pt = MyCommon.SettingPath.Replace("\\", "/") + "/" + ApplicationSettings.AssemblyName;
                using var mt = new Mutex(false, pt);

                if (!mt.WaitOne(0, false))
                {
                    var text = string.Format(MyCommon.ReplaceAppName(Properties.Resources.StartupText1), ApplicationSettings.AssemblyName);
                    MessageBox.Show(text, MyCommon.ReplaceAppName(Properties.Resources.StartupText2), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    TryActivatePreviousWindow();
                    return 1;
                }

                Application.Run(new TweenMain());

                mt.ReleaseMutex();
            }

            return 0;
        }

        private static void WarnIfApiKeyError()
        {
            var canDecrypt = ApplicationSettings.TwitterConsumerKey.TryGetValue(out _);
            if (!canDecrypt)
            {
                var message = Properties.Resources.WarnIfApiKeyError_Message;
                MessageBox.Show(message, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// OpenTween が管理者権限で実行されている場合に警告を表示します
        /// </summary>
        private static void WarnIfRunAsAdministrator()
        {
            // UAC が無効なシステムでは警告を表示しない
            using var lmKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            using var systemKey = lmKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System\");

            var enableLUA = (int?)systemKey?.GetValue("EnableLUA");
            if (enableLUA != 1)
                return;

            using var currentIdentity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(currentIdentity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                var message = string.Format(Properties.Resources.WarnIfRunAsAdministrator_Message, ApplicationSettings.ApplicationName);
                MessageBox.Show(message, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 動作中の .NET Framework のバージョンが適切かチェックします
        /// </summary>
        private static bool CheckRuntimeVersion()
        {
            // Mono 上で動作している場合はバージョンチェックを無視します
            if (Type.GetType("Mono.Runtime", false) != null)
                return true;

            // .NET Framework 4.7.2 以降で動作しているかチェックする
            // 参照: https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed

            using var lmKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            using var ndpKey = lmKey.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\");

            var releaseKey = (int)ndpKey.GetValue("Release");
            return releaseKey >= 461808;
        }

        private static void TryActivatePreviousWindow()
        {
            // 実行中の同じアプリケーションのウィンドウ・ハンドルの取得
            var prevProcess = GetPreviousProcess();
            if (prevProcess == null)
            {
                return;
            }

            var windowHandle = NativeMethods.GetWindowHandle((uint)prevProcess.Id, ApplicationSettings.ApplicationName);
            if (windowHandle != IntPtr.Zero)
            {
                NativeMethods.SetActiveWindow(windowHandle);
            }
        }

        private static Process? GetPreviousProcess()
        {
            var currentProcess = Process.GetCurrentProcess();
            try
            {
                return Process.GetProcessesByName(currentProcess.ProcessName)
                    .Where(p => p.Id != currentProcess.Id)
                    .FirstOrDefault(p => p.MainModule.FileName.Equals(currentProcess.MainModule.FileName, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return null;
            }
        }

        public static void InitCulture()
        {
            var currentCulture = CultureInfo.CurrentUICulture;

            var settingCultureStr = SettingManager.Common.Language;
            if (settingCultureStr != "OS")
            {
                try
                {
                    currentCulture = new CultureInfo(settingCultureStr);
                }
                catch (CultureNotFoundException)
                {
                }
            }

            var preferredCulture = GetPreferredCulture(currentCulture);
            CultureInfo.DefaultThreadCurrentUICulture = preferredCulture;
            Thread.CurrentThread.CurrentUICulture = preferredCulture;
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

        private static bool SetConfigDirectoryPath()
        {
            if (StartupOptions.TryGetValue("configDir", out var configDir) && !MyCommon.IsNullOrEmpty(configDir))
            {
                // 起動オプション /configDir で設定ファイルの参照先を変更できます
                if (!Directory.Exists(configDir))
                {
                    var text = string.Format(Properties.Resources.ConfigDirectoryNotExist, configDir);
                    MessageBox.Show(text, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                MyCommon.SettingPath = Path.GetFullPath(configDir);
            }
            else
            {
                // OpenTween.exe と同じディレクトリに設定ファイルを配置する
                MyCommon.SettingPath = Application.StartupPath;

                SettingManager.LoadAll();

                try
                {
                    // 設定ファイルが書き込み可能な状態であるかテストする
                    SettingManager.SaveAll();
                }
                catch (UnauthorizedAccessException)
                {
                    // 書き込みに失敗した場合 (Program Files 以下に配置されている場合など)

                    // 通常は C:\Users\ユーザー名\AppData\Roaming\OpenTween\ となる
                    var roamingDir = Path.Combine(new[]
                    {
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        ApplicationSettings.ApplicationName,
                    });
                    Directory.CreateDirectory(roamingDir);

                    MyCommon.SettingPath = roamingDir;

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
                    var startupDirFile = new FileInfo(Path.Combine(Application.StartupPath, "SettingCommon.xml"));
                    var roamingDirFile = new FileInfo(Path.Combine(roamingDir, "SettingCommon.xml"));

                    if (roamingDirFile.Exists && (!startupDirFile.Exists || startupDirFile.LastWriteTime <= roamingDirFile.LastWriteTime))
                    {
                        // 既に Roaming に設定ファイルが存在し、Roaming 内のファイルの方が新しい場合は
                        // StartupPath に設定ファイルが存在しても無視する
                        SettingManager.LoadAll();
                    }
                    else
                    {
                        if (startupDirFile.Exists)
                        {
                            // StartupPath に設定ファイルが存在し、Roaming 内のファイルよりも新しい場合のみ警告を表示する
                            var message = string.Format(Properties.Resources.SettingPath_Relocation, Application.StartupPath, MyCommon.SettingPath);
                            MessageBox.Show(message, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        // Roaming に設定ファイルを作成 (StartupPath に読み込みに成功した設定ファイルがあれば内容がコピーされる)
                        SettingManager.SaveAll();
                    }
                }
            }

            return true;
        }
    }
}
