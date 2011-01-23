' Tween - Client of Twitter
' Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
'           (c) 2008-2011 Moz (@syo68k)
'           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
'           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
'           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
' All rights reserved.
' 
' This file is part of Tween.
' 
' This program is free software; you can redistribute it and/or modify it
' under the terms of the GNU General Public License as published by the Free
' Software Foundation; either version 3 of the License, or (at your option)
' any later version.
' 
' This program is distributed in the hope that it will be useful, but
' WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
' or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
' for more details. 
' 
' You should have received a copy of the GNU General Public License along
' with this program. If not, see <http://www.gnu.org/licenses/>, or write to
' the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
' Boston, MA 02110-1301, USA.

Imports System.Runtime.InteropServices
Imports System.Threading


Public Class InternetSecurityManager
    Implements WebBrowserAPI.IServiceProvider
    Implements WebBrowserAPI.IInternetSecurityManager

#Region "HRESULT"
    Private Class HRESULT
        Public Shared S_OK As Integer = &H0
        Public Shared S_FALSE As Integer = &H1
        Public Shared E_NOTIMPL As Integer = &H80004001
        Public Shared E_NOINTERFACE As Integer = &H80004002
    End Class
#End Region

#Region "WebBrowserAPI"
    Private Class WebBrowserAPI
        Public Shared INET_E_DEFAULT_ACTION As Integer = &H800C0011

        Public Enum URLZONE
            URLZONE_LOCAL_MACHINE = 0
            URLZONE_INTRANET = URLZONE_LOCAL_MACHINE + 1
            URLZONE_TRUSTED = URLZONE_INTRANET + 1
            URLZONE_INTERNET = URLZONE_TRUSTED + 1
            URLZONE_UNTRUSTED = URLZONE_INTERNET + 1
        End Enum

        Public Shared URLACTION_MIN As Integer = &H1000

        Public Shared URLACTION_DOWNLOAD_MIN As Integer = &H1000
        Public Shared URLACTION_DOWNLOAD_SIGNED_ACTIVEX As Integer = &H1001
        Public Shared URLACTION_DOWNLOAD_UNSIGNED_ACTIVEX As Integer = &H1004
        Public Shared URLACTION_DOWNLOAD_CURR_MAX As Integer = &H1004
        Public Shared URLACTION_DOWNLOAD_MAX As Integer = &H11FF

        Public Shared URLACTION_ACTIVEX_MIN As Integer = &H1200
        Public Shared URLACTION_ACTIVEX_RUN As Integer = &H1200
        Public Shared URLPOLICY_ACTIVEX_CHECK_LIST As Integer = &H10000
        Public Shared URLACTION_ACTIVEX_OVERRIDE_OBJECT_SAFETY As Integer = &H1201
        Public Shared URLACTION_ACTIVEX_OVERRIDE_DATA_SAFETY As Integer = &H1202
        Public Shared URLACTION_ACTIVEX_OVERRIDE_SCRIPT_SAFETY As Integer = &H1203
        Public Shared URLACTION_SCRIPT_OVERRIDE_SAFETY As Integer = &H1401
        Public Shared URLACTION_ACTIVEX_CONFIRM_NOOBJECTSAFETY As Integer = &H1204
        Public Shared URLACTION_ACTIVEX_TREATASUNTRUSTED As Integer = &H1205
        Public Shared URLACTION_ACTIVEX_NO_WEBOC_SCRIPT As Integer = &H1206
        Public Shared URLACTION_ACTIVEX_CURR_MAX As Integer = &H1206
        Public Shared URLACTION_ACTIVEX_MAX As Integer = &H13FF

        Public Shared URLACTION_SCRIPT_MIN As Integer = &H1400
        Public Shared URLACTION_SCRIPT_RUN As Integer = &H1400
        Public Shared URLACTION_SCRIPT_JAVA_USE As Integer = &H1402
        Public Shared URLACTION_SCRIPT_SAFE_ACTIVEX As Integer = &H1405
        Public Shared URLACTION_CROSS_DOMAIN_DATA As Integer = &H1406
        Public Shared URLACTION_SCRIPT_PASTE As Integer = &H1407
        Public Shared URLACTION_SCRIPT_CURR_MAX As Integer = &H1407
        Public Shared URLACTION_SCRIPT_MAX As Integer = &H15FF

        Public Shared URLACTION_HTML_MIN As Integer = &H1600
        Public Shared URLACTION_HTML_SUBMIT_FORMS As Integer = &H1601                                 ' aggregate next two
        Public Shared URLACTION_HTML_SUBMIT_FORMS_FROM As Integer = &H1602                            '
        Public Shared URLACTION_HTML_SUBMIT_FORMS_TO As Integer = &H1603                              '
        Public Shared URLACTION_HTML_FONT_DOWNLOAD As Integer = &H1604
        Public Shared URLACTION_HTML_JAVA_RUN As Integer = &H1605                                     ' derive from Java custom policy
        Public Shared URLACTION_HTML_USERDATA_SAVE As Integer = &H1606
        Public Shared URLACTION_HTML_SUBFRAME_NAVIGATE As Integer = &H1607
        Public Shared URLACTION_HTML_META_REFRESH As Integer = &H1608
        Public Shared URLACTION_HTML_MIXED_CONTENT As Integer = &H1609
        Public Shared URLACTION_HTML_MAX As Integer = &H17FF

        Public Shared URLACTION_SHELL_MIN As Integer = &H1800
        Public Shared URLACTION_SHELL_INSTALL_DTITEMS As Integer = &H1800
        Public Shared URLACTION_SHELL_MOVE_OR_COPY As Integer = &H1802
        Public Shared URLACTION_SHELL_FILE_DOWNLOAD As Integer = &H1803
        Public Shared URLACTION_SHELL_VERB As Integer = &H1804
        Public Shared URLACTION_SHELL_WEBVIEW_VERB As Integer = &H1805
        Public Shared URLACTION_SHELL_SHELLEXECUTE As Integer = &H1806
        Public Shared URLACTION_SHELL_CURR_MAX As Integer = &H1806
        Public Shared URLACTION_SHELL_MAX As Integer = &H19FF

        Public Shared URLACTION_NETWORK_MIN As Integer = &H1A00

        Public Shared URLACTION_CREDENTIALS_USE As Integer = &H1A00
        Public Shared URLPOLICY_CREDENTIALS_SILENT_LOGON_OK As Integer = &H0
        Public Shared URLPOLICY_CREDENTIALS_MUST_PROMPT_USER As Integer = &H10000
        Public Shared URLPOLICY_CREDENTIALS_CONDITIONAL_PROMPT As Integer = &H20000
        Public Shared URLPOLICY_CREDENTIALS_ANONYMOUS_ONLY As Integer = &H30000

        Public Shared URLACTION_AUTHENTICATE_CLIENT As Integer = &H1A01
        Public Shared URLPOLICY_AUTHENTICATE_CLEARTEXT_OK As Integer = &H0
        Public Shared URLPOLICY_AUTHENTICATE_CHALLENGE_RESPONSE As Integer = &H10000
        Public Shared URLPOLICY_AUTHENTICATE_MUTUAL_ONLY As Integer = &H30000


        Public Shared URLACTION_COOKIES As Integer = &H1A02
        Public Shared URLACTION_COOKIES_SESSION As Integer = &H1A03

        Public Shared URLACTION_CLIENT_CERT_PROMPT As Integer = &H1A04

        Public Shared URLACTION_COOKIES_THIRD_PARTY As Integer = &H1A05
        Public Shared URLACTION_COOKIES_SESSION_THIRD_PARTY As Integer = &H1A06

        Public Shared URLACTION_COOKIES_ENABLED As Integer = &H1A10

        Public Shared URLACTION_NETWORK_CURR_MAX As Integer = &H1A10
        Public Shared URLACTION_NETWORK_MAX As Integer = &H1BFF


        Public Shared URLACTION_JAVA_MIN As Integer = &H1C00
        Public Shared URLACTION_JAVA_PERMISSIONS As Integer = &H1C00
        Public Shared URLPOLICY_JAVA_PROHIBIT As Integer = &H0
        Public Shared URLPOLICY_JAVA_HIGH As Integer = &H10000
        Public Shared URLPOLICY_JAVA_MEDIUM As Integer = &H20000
        Public Shared URLPOLICY_JAVA_LOW As Integer = &H30000
        Public Shared URLPOLICY_JAVA_CUSTOM As Integer = &H800000
        Public Shared URLACTION_JAVA_CURR_MAX As Integer = &H1C00
        Public Shared URLACTION_JAVA_MAX As Integer = &H1CFF


        ' The following Infodelivery actions should have no default policies
        ' in the registry.  They assume that no default policy means fall
        ' back to the global restriction.  If an admin sets a policy per
        ' zone, then it overrides the global restriction.

        Public Shared URLACTION_INFODELIVERY_MIN As Integer = &H1D00
        Public Shared URLACTION_INFODELIVERY_NO_ADDING_CHANNELS As Integer = &H1D00
        Public Shared URLACTION_INFODELIVERY_NO_EDITING_CHANNELS As Integer = &H1D01
        Public Shared URLACTION_INFODELIVERY_NO_REMOVING_CHANNELS As Integer = &H1D02
        Public Shared URLACTION_INFODELIVERY_NO_ADDING_SUBSCRIPTIONS As Integer = &H1D03
        Public Shared URLACTION_INFODELIVERY_NO_EDITING_SUBSCRIPTIONS As Integer = &H1D04
        Public Shared URLACTION_INFODELIVERY_NO_REMOVING_SUBSCRIPTIONS As Integer = &H1D05
        Public Shared URLACTION_INFODELIVERY_NO_CHANNEL_LOGGING As Integer = &H1D06
        Public Shared URLACTION_INFODELIVERY_CURR_MAX As Integer = &H1D06
        Public Shared URLACTION_INFODELIVERY_MAX As Integer = &H1DFF
        Public Shared URLACTION_CHANNEL_SOFTDIST_MIN As Integer = &H1E00
        Public Shared URLACTION_CHANNEL_SOFTDIST_PERMISSIONS As Integer = &H1E05
        Public Shared URLPOLICY_CHANNEL_SOFTDIST_PROHIBIT As Integer = &H10000
        Public Shared URLPOLICY_CHANNEL_SOFTDIST_PRECACHE As Integer = &H20000
        Public Shared URLPOLICY_CHANNEL_SOFTDIST_AUTOINSTALL As Integer = &H30000
        Public Shared URLACTION_CHANNEL_SOFTDIST_MAX As Integer = &H1EFF

        ' For each action specified above the system maintains
        ' a set of policies for the action.
        ' The only policies supported currently are permissions (i.e. is something allowed)
        ' and logging status.
        ' IMPORTANT: If you are defining your own policies don't overload the meaning of the
        ' loword of the policy. You can use the hiword to store any policy bits which are only
        ' meaningful to your action.
        ' For an example of how to do this look at the URLPOLICY_JAVA above

        ' Permissions
        Public Shared URLPOLICY_ALLOW As Byte = &H0
        Public Shared URLPOLICY_QUERY As Byte = &H1
        Public Shared URLPOLICY_DISALLOW As Byte = &H3

        ' Notifications are not done when user already queried.
        Public Shared URLPOLICY_NOTIFY_ON_ALLOW As Integer = &H10
        Public Shared URLPOLICY_NOTIFY_ON_DISALLOW As Integer = &H20

        ' Logging is done regardless of whether user was queried.
        Public Shared URLPOLICY_LOG_ON_ALLOW As Integer = &H40
        Public Shared URLPOLICY_LOG_ON_DISALLOW As Integer = &H80

        Public Shared URLPOLICY_MASK_PERMISSIONS As Integer = &HF


        Public Shared URLPOLICY_DONTCHECKDLGBOX As Integer = &H100


        ' ----------------------------------------------------------------------
        ' ここ以下は COM Interface の宣言です。
        Public Shared IID_IProfferService As Guid = New Guid("cb728b20-f786-11ce-92ad-00aa00a74cd0")
        Public Shared SID_SProfferService As Guid = New Guid("cb728b20-f786-11ce-92ad-00aa00a74cd0")
        Public Shared IID_IInternetSecurityManager As Guid = New Guid("79eac9ee-baf9-11ce-8c82-00aa004ba90b")

        <ComImport(), _
        Guid("6d5140c1-7436-11ce-8034-00aa006009fa"), _
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
        Public Interface IServiceProvider
            <PreserveSig()> _
            Function QueryService(<[In]()> ByRef guidService As Guid, <[In]()> ByRef riid As Guid, <Out()> ByRef ppvObject As IntPtr) As Integer
        End Interface

        <ComImport(), _
        Guid("cb728b20-f786-11ce-92ad-00aa00a74cd0"), _
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
        Public Interface IProfferService
            <PreserveSig()> _
            Function ProfferService(<[In]()> ByRef guidService As Guid, <[In]()> ByVal psp As IServiceProvider, <Out()> ByRef cookie As Integer) As Integer

            <PreserveSig()> _
            Function RevokeService(<[In]()> ByVal cookie As Integer) As Integer
        End Interface

        <ComImport(), _
        Guid("79eac9ed-baf9-11ce-8c82-00aa004ba90b"), _
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
        Public Interface IInternetSecurityMgrSite
            <PreserveSig()> _
            Function GetWindow(<[Out]()> ByRef hwnd As IntPtr) As Integer

            <PreserveSig()> _
            Function EnableModeless(<[In](), MarshalAs(UnmanagedType.Bool)> ByVal fEnable As Boolean) As Integer
        End Interface

        <ComImport(), _
        Guid("79eac9ee-baf9-11ce-8c82-00aa004ba90b"), _
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
        Public Interface IInternetSecurityManager
            <PreserveSig()> _
            Function SetSecuritySite(<[In]()> ByVal pSite As IInternetSecurityMgrSite) As Integer

            <PreserveSig()> _
            Function GetSecuritySite(<Out()> ByRef pSite As IInternetSecurityMgrSite) As Integer

            <PreserveSig()> _
            Function MapUrlToZone(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal pwszUrl As String, <Out()> ByRef pdwZone As Integer, ByVal dwFlags As Integer) As Integer

            <PreserveSig()> _
            Function GetSecurityId(<MarshalAs(UnmanagedType.LPWStr)> ByVal pwszUrl As String, <MarshalAs(UnmanagedType.LPArray)> ByVal pbSecurityId As Byte(), ByRef pcbSecurityId As UInt32, ByVal dwReserved As UInt32) As Integer

            <PreserveSig()> _
            Function ProcessUrlAction(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal pwszUrl As String, ByVal dwAction As Integer, <Out()> ByRef pPolicy As Byte, ByVal cbPolicy As Integer, ByVal pContext As Byte, ByVal cbContext As Integer, ByVal dwFlags As Integer, ByVal dwReserved As Integer) As Integer

            <PreserveSig()> _
            Function QueryCustomPolicy(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal pwszUrl As String, ByRef guidKey As Guid, ByVal ppPolicy As Byte, ByVal pcbPolicy As Integer, ByVal pContext As Byte, ByVal cbContext As Integer, ByVal dwReserved As Integer) As Integer

            <PreserveSig()> _
            Function SetZoneMapping(ByVal dwZone As Integer, <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal lpszPattern As String, ByVal dwFlags As Integer) As Integer

            <PreserveSig()> _
            Function GetZoneMappings(ByVal dwZone As Integer, ByRef ppenumString As ComTypes.IEnumString, ByVal dwFlags As Integer) As Integer
        End Interface
    End Class
#End Region

    <Flags()> Public Enum POLICY As Integer
        ALLOW_ACTIVEX = &H1
        ALLOW_SCRIPT = &H2
    End Enum

    Private ocx As New Object
    Private ocxServiceProvider As WebBrowserAPI.IServiceProvider
    Private profferServicePtr As New IntPtr
    Private profferService As WebBrowserAPI.IProfferService

    Private _Policy As POLICY = 0 ' DefaultですべてDisAllow

    Public Sub New(ByVal _WebBrowser As System.Windows.Forms.WebBrowser)
        ' ActiveXコントロール取得
        _WebBrowser.DocumentText = "about:blank" 'ActiveXを初期化する

        Do
            Thread.Sleep(100)
            Application.DoEvents()
        Loop Until _WebBrowser.ReadyState = WebBrowserReadyState.Complete

        ocx = _WebBrowser.ActiveXInstance

        ' IServiceProvider.QueryService() を使って IProfferService を取得
        ocxServiceProvider = DirectCast(ocx, WebBrowserAPI.IServiceProvider)

        Try
            ocxServiceProvider.QueryService( _
            WebBrowserAPI.SID_SProfferService, _
            WebBrowserAPI.IID_IProfferService, profferServicePtr)
        Catch ex As SEHException
        Catch ex As ExternalException
            TraceOut(ex, "HRESULT:" + ex.ErrorCode.ToString("X8") + Environment.NewLine)
            Exit Sub
        End Try


        profferService = DirectCast(Marshal.GetObjectForIUnknown(profferServicePtr),  _
            WebBrowserAPI.IProfferService)

        ' IProfferService.ProfferService() を使って
        ' 自分を IInternetSecurityManager として提供
        profferService.ProfferService( _
            WebBrowserAPI.IID_IInternetSecurityManager, Me, cookie:=0)

    End Sub

    Private Function QueryService(ByRef guidService As System.Guid, _
        ByRef riid As System.Guid, ByRef ppvObject As System.IntPtr) _
        As Integer Implements WebBrowserAPI.IServiceProvider.QueryService

        ppvObject = IntPtr.Zero
        If guidService.CompareTo( _
            WebBrowserAPI.IID_IInternetSecurityManager) = 0 Then
            ' 自分から IID_IInternetSecurityManager を
            ' QueryInterface して返す
            Dim punk As IntPtr = Marshal.GetIUnknownForObject(Me)
            Return Marshal.QueryInterface(punk, riid, ppvObject)
        End If
        Return HRESULT.E_NOINTERFACE
    End Function

    Private Function GetSecurityId(ByVal pwszUrl As String, ByVal pbSecurityId() As Byte, ByRef pcbSecurityId As UInteger, ByVal dwReserved As UInteger) As Integer _
            Implements WebBrowserAPI.IInternetSecurityManager.GetSecurityId
        Return WebBrowserAPI.INET_E_DEFAULT_ACTION
    End Function

    Private Function GetSecuritySite(ByRef pSite As WebBrowserAPI.IInternetSecurityMgrSite) As Integer _
            Implements WebBrowserAPI.IInternetSecurityManager.GetSecuritySite
        Return WebBrowserAPI.INET_E_DEFAULT_ACTION
    End Function

    Private Function GetZoneMappings(ByVal dwZone As Integer, ByRef ppenumString As System.Runtime.InteropServices.ComTypes.IEnumString, ByVal dwFlags As Integer) As Integer _
            Implements WebBrowserAPI.IInternetSecurityManager.GetZoneMappings
        ppenumString = Nothing
        Return WebBrowserAPI.INET_E_DEFAULT_ACTION
    End Function

    Private Function MapUrlToZone(ByVal pwszUrl As String, ByRef pdwZone As Integer, ByVal dwFlags As Integer) As Integer _
            Implements WebBrowserAPI.IInternetSecurityManager.MapUrlToZone
        pdwZone = 0
        If pwszUrl = "about:blank" Then Return WebBrowserAPI.INET_E_DEFAULT_ACTION
        Try
            Dim urlStr As String = IDNDecode(pwszUrl)
            If urlStr Is Nothing Then Return WebBrowserAPI.URLPOLICY_DISALLOW
            Dim url As New Uri(urlStr)
            If url.Scheme = "data" Then
                Return WebBrowserAPI.URLPOLICY_DISALLOW
            End If
        Catch ex As Exception
            Return WebBrowserAPI.URLPOLICY_DISALLOW
        End Try
        Return WebBrowserAPI.INET_E_DEFAULT_ACTION
    End Function

    Private Function ProcessUrlAction(ByVal pwszUrl As String, ByVal dwAction As Integer, ByRef pPolicy As Byte, ByVal cbPolicy As Integer, ByVal pContext As Byte, ByVal cbContext As Integer, ByVal dwFlags As Integer, ByVal dwReserved As Integer) As Integer _
            Implements WebBrowserAPI.IInternetSecurityManager.ProcessUrlAction
        'スクリプト実行状態かを検査しポリシー設定
        If WebBrowserAPI.URLACTION_SCRIPT_MIN <= dwAction And _
            dwAction <= WebBrowserAPI.URLACTION_SCRIPT_MAX Then
            ' スクリプト実行状態
            If (_Policy And POLICY.ALLOW_SCRIPT) = POLICY.ALLOW_SCRIPT Then
                pPolicy = WebBrowserAPI.URLPOLICY_ALLOW
            Else
                pPolicy = WebBrowserAPI.URLPOLICY_DISALLOW
            End If
            Return HRESULT.S_OK
        End If
        ' ActiveX実行状態かを検査しポリシー設定
        If WebBrowserAPI.URLACTION_ACTIVEX_MIN <= dwAction And _
            dwAction <= WebBrowserAPI.URLACTION_ACTIVEX_MAX Then
            ' ActiveX実行状態
            If (_Policy And POLICY.ALLOW_ACTIVEX) = POLICY.ALLOW_ACTIVEX Then
                pPolicy = WebBrowserAPI.URLPOLICY_ALLOW
            Else
                pPolicy = WebBrowserAPI.URLPOLICY_DISALLOW
            End If
            Return HRESULT.S_OK
        End If
        '他のものについてはデフォルト処理
        Return WebBrowserAPI.INET_E_DEFAULT_ACTION
    End Function

    Private Function QueryCustomPolicy(ByVal pwszUrl As String, ByRef guidKey As System.Guid, ByVal ppPolicy As Byte, ByVal pcbPolicy As Integer, ByVal pContext As Byte, ByVal cbContext As Integer, ByVal dwReserved As Integer) As Integer _
            Implements WebBrowserAPI.IInternetSecurityManager.QueryCustomPolicy
        Return WebBrowserAPI.INET_E_DEFAULT_ACTION
    End Function

    Private Function SetSecuritySite(ByVal pSite As WebBrowserAPI.IInternetSecurityMgrSite) As Integer _
            Implements WebBrowserAPI.IInternetSecurityManager.SetSecuritySite
        Return WebBrowserAPI.INET_E_DEFAULT_ACTION
    End Function

    Private Function SetZoneMapping(ByVal dwZone As Integer, ByVal lpszPattern As String, ByVal dwFlags As Integer) As Integer _
            Implements WebBrowserAPI.IInternetSecurityManager.SetZoneMapping
        Return WebBrowserAPI.INET_E_DEFAULT_ACTION
    End Function


    Public Property SecurityPolicy() As POLICY
        Get
            Return _Policy
        End Get
        Set(ByVal value As POLICY)
            _Policy = value
        End Set
    End Property

End Class