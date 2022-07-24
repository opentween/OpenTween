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
using System.Windows.Forms;
using OpenTween.Connection;
using OpenTween.Setting;

namespace OpenTween
{
    internal class ApplicationEvents
    {
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
            InitializeTraceFrag();

            if (!ApplicationPreconditions.CheckAll())
                return 1;

            var settingsPath = SettingManager.DetermineSettingsPath(StartupOptions);
            if (MyCommon.IsNullOrEmpty(settingsPath))
                return 1;

            var settings = new SettingManager(settingsPath);
            settings.LoadAll();

            using var container = new ApplicationContainer(settings);

            settings.Common.Validate();

            ThemeManager.ApplyGlobalUIFont(settings.Local);
            container.CultureService.Initialize();

            Networking.Initialize();
            settings.ApplySettings();

            // 同じ設定ファイルを使用する OpenTween プロセスの二重起動を防止する
            using var mutex = new ApplicationInstanceMutex(ApplicationSettings.AssemblyName, settings.SettingsPath);

            if (mutex.InstanceExists)
            {
                var text = string.Format(MyCommon.ReplaceAppName(Properties.Resources.StartupText1), ApplicationSettings.AssemblyName);
                MessageBox.Show(text, MyCommon.ReplaceAppName(Properties.Resources.StartupText2), MessageBoxButtons.OK, MessageBoxIcon.Information);

                mutex.TryActivatePreviousInstance();
                return 1;
            }

            if (settings.IsIncomplete)
            {
                var completed = ShowSettingsDialog(settings, container.IconAssetsManager);
                if (!completed)
                    return 1; // 設定が完了しなかったため終了
            }

            Application.Run(container.MainForm);

            return 0;
        }

        private static void InitializeTraceFrag()
        {
            var traceFlag = false;

#if DEBUG
            traceFlag = true;
#endif

            if (StartupOptions.ContainsKey("d"))
                traceFlag = true;

            var version = Version.Parse(MyCommon.FileVersion);
            if (version.Build != 0)
                traceFlag = true;

            MyCommon.TraceFlag = traceFlag;
        }

        private static bool ShowSettingsDialog(SettingManager settings, IconAssetsManager iconAssets)
        {
            using var settingDialog = new AppendSettingDialog();
            settingDialog.Icon = iconAssets.IconMain;
            settingDialog.ShowInTaskbar = true; // この時点では TweenMain が表示されていないため代わりに表示する
            settingDialog.LoadConfig(settings.Common, settings.Local);

            var ret = settingDialog.ShowDialog();
            if (ret != DialogResult.OK)
                return false;

            settingDialog.SaveConfig(settings.Common, settings.Local);

            if (settings.IsIncomplete)
                return false;

            settings.SaveAll();
            return true;
        }
    }
}
