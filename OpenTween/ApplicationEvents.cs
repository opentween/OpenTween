﻿// OpenTween - Client of Twitter
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Reflection;
using Microsoft.Win32;
using OpenTween.Setting;
using System.Security.Principal;

namespace OpenTween
{
    internal class MyApplication
    {
        public static readonly CultureInfo[] SupportedUICulture = new[]
        {
            new CultureInfo("en"), // 先頭のカルチャはフォールバック先として使用される
            new CultureInfo("ja"),
        };

        /// <summary>
        /// 起動時に指定されたオプションを取得します
        /// </summary>
        public static IDictionary<string, string> StartupOptions { get; private set; }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            WarnIfRunAsAdministrator();

            if (!CheckRuntimeVersion())
            {
                var message = string.Format(Properties.Resources.CheckRuntimeVersion_Error, ".NET Framework 4.6.2");
                MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            StartupOptions = ParseArguments(args);

            if (!SetConfigDirectoryPath())
                return 1;

            SettingManager.LoadAll();

            InitCulture();

            // 同じ設定ファイルを使用する OpenTween プロセスの二重起動を防止する
            string pt = MyCommon.settingPath.Replace("\\", "/") + "/" + Application.ProductName;
            using (Mutex mt = new Mutex(false, pt))
            {
                if (!mt.WaitOne(0, false))
                {
                    var text = string.Format(MyCommon.ReplaceAppName(Properties.Resources.StartupText1), MyCommon.GetAssemblyName());
                    MessageBox.Show(text, MyCommon.ReplaceAppName(Properties.Resources.StartupText2), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    TryActivatePreviousWindow();
                    return 1;
                }

                TaskScheduler.UnobservedTaskException += (s, e) =>
                {
                    e.SetObserved();
                    OnUnhandledException(e.Exception.Flatten());
                };
                Application.ThreadException += (s, e) => OnUnhandledException(e.Exception);
                AppDomain.CurrentDomain.UnhandledException += (s, e) => OnUnhandledException((Exception)e.ExceptionObject);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TweenMain());

                mt.ReleaseMutex();

                return 0;
            }
        }

        /// <summary>
        /// OpenTween が管理者権限で実行されている場合に警告を表示します
        /// </summary>
        private static void WarnIfRunAsAdministrator()
        {
            // UAC が無効なシステムでは警告を表示しない
            using (var lmKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            using (var systemKey = lmKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System\"))
            {
                var enableLUA = (int?)systemKey?.GetValue("EnableLUA");
                if (enableLUA != 1)
                    return;
            }

            using (var currentIdentity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(currentIdentity);
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    var message = string.Format(Properties.Resources.WarnIfRunAsAdministrator_Message, Application.ProductName);
                    MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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

            // .NET Framework 4.6.2 以降で動作しているかチェックする
            // 参照: http://msdn.microsoft.com/en-us/library/hh925568%28v=vs.110%29.aspx

            using (var lmKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            using (var ndpKey = lmKey.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"))
            {
                var releaseKey = (int)ndpKey.GetValue("Release");
                return releaseKey >= 394802;
            }
        }

        /// <summary>
        /// “/key:value”形式の起動オプションを解釈し IDictionary に変換する
        /// </summary>
        /// <remarks>
        /// 不正な形式のオプションは除外されます。
        /// また、重複したキーのオプションが入力された場合は末尾に書かれたオプションが採用されます。
        /// </remarks>
        internal static IDictionary<string, string> ParseArguments(IEnumerable<string> arguments)
        {
            var optionPattern = new Regex(@"^/(.+?)(?::(.*))?$");

            return arguments.Select(x => optionPattern.Match(x))
                .Where(x => x.Success)
                .GroupBy(x => x.Groups[1].Value)
                .ToDictionary(x => x.Key, x => x.Last().Groups[2].Value);
        }

        private static void TryActivatePreviousWindow()
        {
            // 実行中の同じアプリケーションのウィンドウ・ハンドルの取得
            var prevProcess = GetPreviousProcess();
            if (prevProcess == null)
            {
                return;
            }

            IntPtr windowHandle = NativeMethods.GetWindowHandle((uint)prevProcess.Id, Application.ProductName);
            if (windowHandle != IntPtr.Zero)
            {
                NativeMethods.SetActiveWindow(windowHandle);
            }
        }

        private static Process GetPreviousProcess()
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

        private static void OnUnhandledException(Exception ex)
        {
            if (CheckIgnorableError(ex))
                return;

            if (MyCommon.ExceptionOut(ex))
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// 無視しても問題のない既知の例外であれば true を返す
        /// </summary>
        private static bool CheckIgnorableError(Exception ex)
        {
#if DEBUG
            return false;
#else
            if (ex is AggregateException aggregated)
            {
                if (aggregated.InnerExceptions.Count != 1)
                    return false;

                ex = aggregated.InnerExceptions.Single();
            }

            switch (ex)
            {
                case System.Net.WebException webEx:
                    // SSL/TLS のネゴシエーションに失敗した場合に発生する。なぜかキャッチできない例外
                    // https://osdn.net/ticket/browse.php?group_id=6526&tid=37432
                    if (webEx.Status == System.Net.WebExceptionStatus.SecureChannelFailure)
                        return true;
                    break;
                case System.Threading.Tasks.TaskCanceledException cancelEx:
                    // ton.twitter.com の画像でタイムアウトした場合、try-catch で例外がキャッチできない
                    // https://osdn.net/ticket/browse.php?group_id=6526&tid=37433
                    var stackTrace = new System.Diagnostics.StackTrace(cancelEx);
                    var lastFrameMethod = stackTrace.GetFrame(stackTrace.FrameCount - 1).GetMethod();
                    if (lastFrameMethod.ReflectedType == typeof(Connection.TwitterApiConnection) &&
                        lastFrameMethod.Name == nameof(Connection.TwitterApiConnection.GetStreamAsync))
                        return true;
                    break;
            }

            return false;
#endif
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
                catch (CultureNotFoundException) { }
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
            if (StartupOptions.TryGetValue("configDir", out var configDir) && !string.IsNullOrEmpty(configDir))
            {
                // 起動オプション /configDir で設定ファイルの参照先を変更できます
                if (!Directory.Exists(configDir))
                {
                    var text = string.Format(Properties.Resources.ConfigDirectoryNotExist, configDir);
                    MessageBox.Show(text, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                MyCommon.settingPath = Path.GetFullPath(configDir);
            }
            else
            {
                // OpenTween.exe と同じディレクトリに設定ファイルを配置する
                MyCommon.settingPath = Application.StartupPath;

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
                        Application.ProductName,
                    });
                    Directory.CreateDirectory(roamingDir);

                    MyCommon.settingPath = roamingDir;

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
                            var message = string.Format(Properties.Resources.SettingPath_Relocation, Application.StartupPath, MyCommon.settingPath);
                            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
