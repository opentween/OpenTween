// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;

namespace OpenTween
{
    /// <summary>
    /// アプリケーションの起動要件を満たしているか確認するクラス
    /// </summary>
    public sealed class ApplicationPreconditions
    {
        // .NET Framework ランタイムの最小要件
        // 参照: https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
        private const string RuntimeMinimumVersionName = ".NET Framework 4.7.2";
        private const int RuntimeMinimumVersion = 461808;

        /// <summary>
        /// 全ての起動要件を満たしているか確認する
        /// </summary>
        /// <returns>
        /// 起動に必須な要件を全て満たしている場合は true、それ以外は false。
        /// </returns>
        public static bool CheckAll()
        {
            var conditions = new ApplicationPreconditions();

            if (!conditions.CheckApiKey())
            {
                var message = Properties.Resources.WarnIfApiKeyError_Message;
                ShowMessageBox(message, MessageBoxIcon.Error);
                return false;
            }

            if (!conditions.CheckRuntimeVersion())
            {
                var message = string.Format(Properties.Resources.CheckRuntimeVersion_Error, RuntimeMinimumVersionName);
                ShowMessageBox(message, MessageBoxIcon.Error);
                return false;
            }

            if (!conditions.CheckRunAsNormalUser())
            {
                var message = string.Format(Properties.Resources.WarnIfRunAsAdministrator_Message, ApplicationSettings.ApplicationName);
                ShowMessageBox(message, MessageBoxIcon.Warning);
            }

            return true;
        }

        private static void ShowMessageBox(string message, MessageBoxIcon icon)
            => MessageBox.Show(message, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, icon);

        /// <summary>
        /// API キーが復号できる状態であるか確認する
        /// </summary>
        public bool CheckApiKey()
            => this.CanDecryptApiKey(ApplicationSettings.TwitterConsumerKey);

        /// <summary>
        /// 動作中の .NET Framework のバージョンが適切か確認する
        /// </summary>
        public bool CheckRuntimeVersion()
        {
            // Mono 上で動作している場合は無視する
            if (this.IsRunOnMono())
                return true;

            return this.GetFrameworkReleaseKey() >= RuntimeMinimumVersion;
        }

        /// <summary>
        /// プロセスが管理者権限で実行されていないか確認する
        /// </summary>
        public bool CheckRunAsNormalUser()
        {
            // UAC が無効なシステムでは警告を表示しない
            if (!this.GetEnableLUA())
                return true;

            return !this.IsRunAsAdministrator();
        }

        private bool CanDecryptApiKey(ApiKey apiKey)
            => apiKey.TryGetValue(out _);

        private bool IsRunOnMono()
            => Type.GetType("Mono.Runtime", false) != null;

        private int GetFrameworkReleaseKey()
        {
            using var lmKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            using var ndpKey = lmKey.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\");

            return (int)ndpKey.GetValue("Release");
        }

        private bool GetEnableLUA()
        {
            using var lmKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            using var systemKey = lmKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System\");

            var enableLUA = (int?)systemKey?.GetValue("EnableLUA");
            return enableLUA == 1;
        }

        private bool IsRunAsAdministrator()
        {
            using var currentIdentity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(currentIdentity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
