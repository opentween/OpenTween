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

#nullable enable
#pragma warning disable SA1310

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace OpenTween
{
    public class InternetSecurityManager : WebBrowserAPI.IServiceProvider, WebBrowserAPI.IInternetSecurityManager
    {
        #region "HRESULT"
        private enum HRESULT
        {
            S_OK = 0x0,
            S_FALSE = 0x1,
            E_NOTIMPL = unchecked((int)0x80004001),
            E_NOINTERFACE = unchecked((int)0x80004002),
        }
        #endregion

        [Flags]
        public enum POLICY
        {
            ALLOW_ACTIVEX = 0x1,
            ALLOW_SCRIPT = 0x2,
        }

        private readonly object ocx = new();
        private readonly WebBrowserAPI.IServiceProvider ocxServiceProvider;
        private readonly IntPtr profferServicePtr = new();
        private readonly WebBrowserAPI.IProfferService profferService = null!;

        public POLICY SecurityPolicy { get; set; } = 0;

        public InternetSecurityManager(WebBrowser webBrowser)
        {
            // ActiveXコントロール取得
            webBrowser.Url = new Uri("about:blank"); // ActiveXを初期化する

            do
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }
            while (webBrowser.ReadyState != WebBrowserReadyState.Complete);

            this.ocx = webBrowser.ActiveXInstance;

            // IServiceProvider.QueryService() を使って IProfferService を取得
            this.ocxServiceProvider = (WebBrowserAPI.IServiceProvider)this.ocx;

            try
            {
                this.ocxServiceProvider.QueryService(
                    ref WebBrowserAPI.SID_SProfferService,
                    ref WebBrowserAPI.IID_IProfferService,
                    out this.profferServicePtr);
            }
            catch (SEHException ex)
            {
                MyCommon.TraceOut(ex, "ocxServiceProvider.QueryService() HRESULT:" + ex.ErrorCode.ToString("X8") + Environment.NewLine);
                return;
            }
            catch (ExternalException ex)
            {
                MyCommon.TraceOut(ex, "ocxServiceProvider.QueryService() HRESULT:" + ex.ErrorCode.ToString("X8") + Environment.NewLine);
                return;
            }

            this.profferService = (WebBrowserAPI.IProfferService)Marshal.GetObjectForIUnknown(this.profferServicePtr);

            // IProfferService.ProfferService() を使って
            // 自分を IInternetSecurityManager として提供
            try
            {
                this.profferService.ProfferService(
                    ref WebBrowserAPI.IID_IInternetSecurityManager, this, out var cookie);
            }
            catch (SEHException ex)
            {
                MyCommon.TraceOut(ex, "IProfferSerive.ProfferService() HRESULT:" + ex.ErrorCode.ToString("X8") + Environment.NewLine);
                return;
            }
            catch (ExternalException ex)
            {
                MyCommon.TraceOut(ex, "IProfferSerive.ProfferService() HRESULT:" + ex.ErrorCode.ToString("X8") + Environment.NewLine);
                return;
            }
        }

        int WebBrowserAPI.IServiceProvider.QueryService(
            ref Guid guidService,
            ref Guid riid,
            out IntPtr ppvObject)
        {
            ppvObject = IntPtr.Zero;
            if (guidService.CompareTo(
                WebBrowserAPI.IID_IInternetSecurityManager) == 0)
            {
                // 自分から IID_IInternetSecurityManager を
                // QueryInterface して返す
                var punk = Marshal.GetIUnknownForObject(this);
                return Marshal.QueryInterface(punk, ref riid, out ppvObject);
            }
            return (int)HRESULT.E_NOINTERFACE;
        }

        int WebBrowserAPI.IInternetSecurityManager.GetSecurityId(string pwszUrl, byte[] pbSecurityId, ref uint pcbSecurityId, uint dwReserved)
            => WebBrowserAPI.INET_E_DEFAULT_ACTION;

        int WebBrowserAPI.IInternetSecurityManager.GetSecuritySite(out WebBrowserAPI.IInternetSecurityMgrSite? pSite)
        {
            pSite = null;
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        int WebBrowserAPI.IInternetSecurityManager.GetZoneMappings(int dwZone, ref IEnumString? ppenumstring, int dwFlags)
        {
            ppenumstring = null;
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        int WebBrowserAPI.IInternetSecurityManager.MapUrlToZone(string pwszUrl, out int pdwZone, int dwFlags)
        {
            pdwZone = 0;
            if (pwszUrl == "about:blank") return WebBrowserAPI.INET_E_DEFAULT_ACTION;
            try
            {
                var urlStr = MyCommon.IDNEncode(pwszUrl);
                if (urlStr == null) return WebBrowserAPI.URLPOLICY_DISALLOW;
                var url = new Uri(urlStr);
                if (url.Scheme == "data")
                {
                    return WebBrowserAPI.URLPOLICY_DISALLOW;
                }
            }
            catch (Exception)
            {
                return WebBrowserAPI.URLPOLICY_DISALLOW;
            }
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        private const byte URLPOLICY_ALLOW = 0;

        int WebBrowserAPI.IInternetSecurityManager.ProcessUrlAction(string pwszUrl, int dwAction, out byte pPolicy, int cbPolicy, byte pContext, int cbContext, int dwFlags, int dwReserved)
        {
            pPolicy = URLPOLICY_ALLOW;
            // スクリプト実行状態かを検査しポリシー設定
            if (WebBrowserAPI.URLACTION_SCRIPT_MIN <= dwAction &
                dwAction <= WebBrowserAPI.URLACTION_SCRIPT_MAX)
            {
                // スクリプト実行状態
                if ((this.SecurityPolicy & POLICY.ALLOW_SCRIPT) == POLICY.ALLOW_SCRIPT)
                {
                    pPolicy = WebBrowserAPI.URLPOLICY_ALLOW;
                }
                else
                {
                    pPolicy = WebBrowserAPI.URLPOLICY_DISALLOW;
                }
                if (Regex.IsMatch(pwszUrl, @"^https?://((api\.)?twitter\.com/|([a-zA-Z0-9]+\.)?twimg\.com/)")) pPolicy = WebBrowserAPI.URLPOLICY_ALLOW;
                return (int)HRESULT.S_OK;
            }
            // ActiveX実行状態かを検査しポリシー設定
            if (WebBrowserAPI.URLACTION_ACTIVEX_MIN <= dwAction &
                dwAction <= WebBrowserAPI.URLACTION_ACTIVEX_MAX)
            {
                // ActiveX実行状態
                if ((this.SecurityPolicy & POLICY.ALLOW_ACTIVEX) == POLICY.ALLOW_ACTIVEX)
                {
                    pPolicy = WebBrowserAPI.URLPOLICY_ALLOW;
                }
                else
                {
                    pPolicy = WebBrowserAPI.URLPOLICY_DISALLOW;
                }
                return (int)HRESULT.S_OK;
            }
            // 他のものについてはデフォルト処理
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        int WebBrowserAPI.IInternetSecurityManager.QueryCustomPolicy(string pwszUrl, ref Guid guidKey, byte ppPolicy, int pcbPolicy, byte pContext, int cbContext, int dwReserved)
            => WebBrowserAPI.INET_E_DEFAULT_ACTION;

        int WebBrowserAPI.IInternetSecurityManager.SetSecuritySite(WebBrowserAPI.IInternetSecurityMgrSite pSite)
            => WebBrowserAPI.INET_E_DEFAULT_ACTION;

        int WebBrowserAPI.IInternetSecurityManager.SetZoneMapping(int dwZone, string lpszPattern, int dwFlags)
            => WebBrowserAPI.INET_E_DEFAULT_ACTION;
    }

    #region "WebBrowserAPI"
    internal static class WebBrowserAPI
    {
        public static int INET_E_DEFAULT_ACTION = unchecked((int)0x800C0011);

        public enum URLZONE
        {
            URLZONE_LOCAL_MACHINE = 0,
            URLZONE_INTRANET = URLZONE_LOCAL_MACHINE + 1,
            URLZONE_TRUSTED = URLZONE_INTRANET + 1,
            URLZONE_INTERNET = URLZONE_TRUSTED + 1,
            URLZONE_UNTRUSTED = URLZONE_INTERNET + 1,
        }

        public static int URLACTION_MIN = 0x1000;

        public static int URLACTION_DOWNLOAD_MIN = 0x1000;
        public static int URLACTION_DOWNLOAD_SIGNED_ACTIVEX = 0x1001;
        public static int URLACTION_DOWNLOAD_UNSIGNED_ACTIVEX = 0x1004;
        public static int URLACTION_DOWNLOAD_CURR_MAX = 0x1004;
        public static int URLACTION_DOWNLOAD_MAX = 0x11FF;

        public static int URLACTION_ACTIVEX_MIN = 0x1200;
        public static int URLACTION_ACTIVEX_RUN = 0x1200;
        public static int URLPOLICY_ACTIVEX_CHECK_LIST = 0x10000;
        public static int URLACTION_ACTIVEX_OVERRIDE_OBJECT_SAFETY = 0x1201;
        public static int URLACTION_ACTIVEX_OVERRIDE_DATA_SAFETY = 0x1202;
        public static int URLACTION_ACTIVEX_OVERRIDE_SCRIPT_SAFETY = 0x1203;
        public static int URLACTION_SCRIPT_OVERRIDE_SAFETY = 0x1401;
        public static int URLACTION_ACTIVEX_CONFIRM_NOOBJECTSAFETY = 0x1204;
        public static int URLACTION_ACTIVEX_TREATASUNTRUSTED = 0x1205;
        public static int URLACTION_ACTIVEX_NO_WEBOC_SCRIPT = 0x1206;
        public static int URLACTION_ACTIVEX_CURR_MAX = 0x1206;
        public static int URLACTION_ACTIVEX_MAX = 0x13FF;

        public static int URLACTION_SCRIPT_MIN = 0x1400;
        public static int URLACTION_SCRIPT_RUN = 0x1400;
        public static int URLACTION_SCRIPT_JAVA_USE = 0x1402;
        public static int URLACTION_SCRIPT_SAFE_ACTIVEX = 0x1405;
        public static int URLACTION_CROSS_DOMAIN_DATA = 0x1406;
        public static int URLACTION_SCRIPT_PASTE = 0x1407;
        public static int URLACTION_SCRIPT_CURR_MAX = 0x1407;
        public static int URLACTION_SCRIPT_MAX = 0x15FF;

        public static int URLACTION_HTML_MIN = 0x1600;
        public static int URLACTION_HTML_SUBMIT_FORMS = 0x1601; // aggregate next two
        public static int URLACTION_HTML_SUBMIT_FORMS_FROM = 0x1602;
        public static int URLACTION_HTML_SUBMIT_FORMS_TO = 0x1603;
        public static int URLACTION_HTML_FONT_DOWNLOAD = 0x1604;
        public static int URLACTION_HTML_JAVA_RUN = 0x1605; // derive from Java custom policy
        public static int URLACTION_HTML_USERDATA_SAVE = 0x1606;
        public static int URLACTION_HTML_SUBFRAME_NAVIGATE = 0x1607;
        public static int URLACTION_HTML_META_REFRESH = 0x1608;
        public static int URLACTION_HTML_MIXED_CONTENT = 0x1609;
        public static int URLACTION_HTML_MAX = 0x17FF;

        public static int URLACTION_SHELL_MIN = 0x1800;
        public static int URLACTION_SHELL_INSTALL_DTITEMS = 0x1800;
        public static int URLACTION_SHELL_MOVE_OR_COPY = 0x1802;
        public static int URLACTION_SHELL_FILE_DOWNLOAD = 0x1803;
        public static int URLACTION_SHELL_VERB = 0x1804;
        public static int URLACTION_SHELL_WEBVIEW_VERB = 0x1805;
        public static int URLACTION_SHELL_SHELLEXECUTE = 0x1806;
        public static int URLACTION_SHELL_CURR_MAX = 0x1806;
        public static int URLACTION_SHELL_MAX = 0x19FF;

        public static int URLACTION_NETWORK_MIN = 0x1A00;

        public static int URLACTION_CREDENTIALS_USE = 0x1A00;
        public static int URLPOLICY_CREDENTIALS_SILENT_LOGON_OK = 0x0;
        public static int URLPOLICY_CREDENTIALS_MUST_PROMPT_USER = 0x10000;
        public static int URLPOLICY_CREDENTIALS_CONDITIONAL_PROMPT = 0x20000;
        public static int URLPOLICY_CREDENTIALS_ANONYMOUS_ONLY = 0x30000;

        public static int URLACTION_AUTHENTICATE_CLIENT = 0x1A01;
        public static int URLPOLICY_AUTHENTICATE_CLEARTEXT_OK = 0x0;
        public static int URLPOLICY_AUTHENTICATE_CHALLENGE_RESPONSE = 0x10000;
        public static int URLPOLICY_AUTHENTICATE_MUTUAL_ONLY = 0x30000;

        public static int URLACTION_COOKIES = 0x1A02;
        public static int URLACTION_COOKIES_SESSION = 0x1A03;

        public static int URLACTION_CLIENT_CERT_PROMPT = 0x1A04;

        public static int URLACTION_COOKIES_THIRD_PARTY = 0x1A05;
        public static int URLACTION_COOKIES_SESSION_THIRD_PARTY = 0x1A06;

        public static int URLACTION_COOKIES_ENABLED = 0x1A10;

        public static int URLACTION_NETWORK_CURR_MAX = 0x1A10;
        public static int URLACTION_NETWORK_MAX = 0x1BFF;

        public static int URLACTION_JAVA_MIN = 0x1C00;
        public static int URLACTION_JAVA_PERMISSIONS = 0x1C00;
        public static int URLPOLICY_JAVA_PROHIBIT = 0x0;
        public static int URLPOLICY_JAVA_HIGH = 0x10000;
        public static int URLPOLICY_JAVA_MEDIUM = 0x20000;
        public static int URLPOLICY_JAVA_LOW = 0x30000;
        public static int URLPOLICY_JAVA_CUSTOM = 0x800000;
        public static int URLACTION_JAVA_CURR_MAX = 0x1C00;
        public static int URLACTION_JAVA_MAX = 0x1CFF;

        // The following Infodelivery actions should have no default policies
        // in the registry.  They assume that no default policy means fall
        // back to the global restriction.  If an admin sets a policy per
        // zone, then it overrides the global restriction.

        public static int URLACTION_INFODELIVERY_MIN = 0x1D00;
        public static int URLACTION_INFODELIVERY_NO_ADDING_CHANNELS = 0x1D00;
        public static int URLACTION_INFODELIVERY_NO_EDITING_CHANNELS = 0x1D01;
        public static int URLACTION_INFODELIVERY_NO_REMOVING_CHANNELS = 0x1D02;
        public static int URLACTION_INFODELIVERY_NO_ADDING_SUBSCRIPTIONS = 0x1D03;
        public static int URLACTION_INFODELIVERY_NO_EDITING_SUBSCRIPTIONS = 0x1D04;
        public static int URLACTION_INFODELIVERY_NO_REMOVING_SUBSCRIPTIONS = 0x1D05;
        public static int URLACTION_INFODELIVERY_NO_CHANNEL_LOGGING = 0x1D06;
        public static int URLACTION_INFODELIVERY_CURR_MAX = 0x1D06;
        public static int URLACTION_INFODELIVERY_MAX = 0x1DFF;
        public static int URLACTION_CHANNEL_SOFTDIST_MIN = 0x1E00;
        public static int URLACTION_CHANNEL_SOFTDIST_PERMISSIONS = 0x1E05;
        public static int URLPOLICY_CHANNEL_SOFTDIST_PROHIBIT = 0x10000;
        public static int URLPOLICY_CHANNEL_SOFTDIST_PRECACHE = 0x20000;
        public static int URLPOLICY_CHANNEL_SOFTDIST_AUTOINSTALL = 0x30000;
        public static int URLACTION_CHANNEL_SOFTDIST_MAX = 0x1EFF;

        // For each action specified above the system maintains
        // a set of policies for the action.
        // The only policies supported currently are permissions (i.e. is something allowed)
        // and logging status.
        // IMPORTANT: If you are defining your own policies don't overload the meaning of the
        // loword of the policy. You can use the hiword to store any policy bits which are only
        // meaningful to your action.
        // For an example of how to do this look at the URLPOLICY_JAVA above

        // Permissions
        public static byte URLPOLICY_ALLOW = 0x0;
        public static byte URLPOLICY_QUERY = 0x1;
        public static byte URLPOLICY_DISALLOW = 0x3;

        // Notifications are not done when user already queried.
        public static int URLPOLICY_NOTIFY_ON_ALLOW = 0x10;
        public static int URLPOLICY_NOTIFY_ON_DISALLOW = 0x20;

        // Logging is done regardless of whether user was queried.
        public static int URLPOLICY_LOG_ON_ALLOW = 0x40;
        public static int URLPOLICY_LOG_ON_DISALLOW = 0x80;

        public static int URLPOLICY_MASK_PERMISSIONS = 0xF;

        public static int URLPOLICY_DONTCHECKDLGBOX = 0x100;

        // ----------------------------------------------------------------------
        // ここ以下は COM Interface の宣言です。
        public static Guid IID_IProfferService = new("cb728b20-f786-11ce-92ad-00aa00a74cd0");
        public static Guid SID_SProfferService = new("cb728b20-f786-11ce-92ad-00aa00a74cd0");
        public static Guid IID_IInternetSecurityManager = new("79eac9ee-baf9-11ce-8c82-00aa004ba90b");

        [ComImport]
        [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid, out IntPtr ppvObject);
        }

        [ComImport]
        [Guid("cb728b20-f786-11ce-92ad-00aa00a74cd0")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IProfferService
        {
            [PreserveSig]
            int ProfferService([In] ref Guid guidService, [In] IServiceProvider psp, out int cookie);

            [PreserveSig]
            int RevokeService([In] int cookie);
        }

        [ComImport]
        [Guid("79eac9ed-baf9-11ce-8c82-00aa004ba90b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInternetSecurityMgrSite
        {
            [PreserveSig]
            int GetWindow(out IntPtr hwnd);

            [PreserveSig]
            int EnableModeless([In, MarshalAs(UnmanagedType.Bool)] bool fEnable);
        }

        [ComImport]
        [Guid("79eac9ee-baf9-11ce-8c82-00aa004ba90b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInternetSecurityManager
        {
            [PreserveSig]
            int SetSecuritySite([In] IInternetSecurityMgrSite pSite);

            [PreserveSig]
            int GetSecuritySite(out IInternetSecurityMgrSite? pSite);

            [PreserveSig]
            int MapUrlToZone([In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, out int pdwZone, int dwFlags);

            [PreserveSig]
            int GetSecurityId([MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pbSecurityId, ref uint pcbSecurityId, uint dwReserved);

            [PreserveSig]
            int ProcessUrlAction([In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, int dwAction, out byte pPolicy, int cbPolicy, byte pContext, int cbContext, int dwFlags, int dwReserved);

            [PreserveSig]
            int QueryCustomPolicy([In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, ref Guid guidKey, byte ppPolicy, int pcbPolicy, byte pContext, int cbContext, int dwReserved);

            [PreserveSig]
            int SetZoneMapping(int dwZone, [In, MarshalAs(UnmanagedType.LPWStr)] string lpszPattern, int dwFlags);

            [PreserveSig]
            int GetZoneMappings(int dwZone, ref IEnumString? ppenumstring, int dwFlags);
        }
    }
    #endregion
}
