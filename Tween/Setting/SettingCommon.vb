' Tween - Client of Twitter
' Copyright (c) 2007-2010 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
'           (c) 2008-2010 Moz (@syo68k) <http://iddy.jp/profile/moz/>
'           (c) 2008-2010 takeshik (@takeshik) <http://www.takeshik.org/>
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
<Serializable()> _
Public Class SettingCommon
    Inherits SettingBase(Of SettingCommon)

#Region "Settingクラス基本"
    Public Shared Function Load() As SettingCommon
        Return LoadSettings()
    End Function

    Public Sub Save()
        SaveSettings(Me)
    End Sub
#End Region

    Public UserName As String = ""

    <Xml.Serialization.XmlIgnore()> _
    Public Password As String = ""
    Public Property EncryptPassword() As String
        Get
            Return Encrypt(Password)
        End Get
        Set(ByVal value As String)
            Password = Decrypt(value)
        End Set
    End Property

    Public Token As String = ""
    <Xml.Serialization.XmlIgnore()> _
    Public TokenSecret As String = ""
    Public Property EncryptTokenSecret() As String
        Get
            Return Encrypt(TokenSecret)
        End Get
        Set(ByVal value As String)
            TokenSecret = Decrypt(value)
        End Set
    End Property

    Private Function Encrypt(ByVal password As String) As String
        If String.IsNullOrEmpty(password) Then password = ""
        If password.Length > 0 Then
            Try
                Return EncryptString(password)
            Catch ex As Exception
                Return ""
            End Try
        Else
            Return ""
        End If
    End Function
    Private Function Decrypt(ByVal password As String) As String
        If String.IsNullOrEmpty(password) Then password = ""
        If password.Length > 0 Then
            Try
                password = DecryptString(password)
            Catch ex As Exception
                password = ""
            End Try
        End If
        Return password
    End Function

    Public TabList As New List(Of String)
    Public TimelinePeriod As Integer = 90
    Public ReplyPeriod As Integer = 180
    Public DMPeriod As Integer = 600
    Public PubSearchPeriod As Integer = 180
    Public ListsPeriod As Integer = 180
    Public Read As Boolean = True
    Public ListLock As Boolean = False
    Public IconSize As IconSizes = IconSizes.Icon16
    Public NewAllPop As Boolean = True
    Public PlaySound As Boolean = False
    Public UnreadManage As Boolean = True
    Public OneWayLove As Boolean = True
    Public NameBalloon As NameBalloonEnum = NameBalloonEnum.NickName
    Public PostCtrlEnter As Boolean = False
    Public PostShiftEnter As Boolean = False
    Public CountApi As Integer = 60
    Public CountApiReply As Integer = 40
    Public PostAndGet As Boolean = True
    Public DispUsername As Boolean = False
    Public MinimizeToTray As Boolean = False
    Public CloseToExit As Boolean = False
    Public DispLatestPost As DispTitleEnum = DispTitleEnum.Post
    Public SortOrderLock As Boolean = False
    Public TinyUrlResolve As Boolean = True
    Public PeriodAdjust As Boolean = True
    Public StartupVersion As Boolean = True
    Public StartupFollowers As Boolean = True
    Public RestrictFavCheck As Boolean = False
    Public AlwaysTop As Boolean = False
    Public CultureCode As String = ""
    Public UrlConvertAuto As Boolean = False
    Public Outputz As Boolean = False
    Public SortColumn As Integer = 3
    Public SortOrder As Integer = 1
    Public IsMonospace As Boolean = False
    Public ReadOldPosts As Boolean = False
    Public UseSsl As Boolean = True
    Public Language As String = "OS"
    Public Nicoms As Boolean = False
    Public HashTags As New List(Of String)
    Public HashSelected As String = ""
    Public HashIsPermanent As Boolean = False
    Public HashIsHead As Boolean = False
    Public PreviewEnable As Boolean = True

    <Xml.Serialization.XmlIgnore()> _
    Public OutputzKey As String = ""
    Public Property EncryptOutputzKey() As String
        Get
            Return Encrypt(OutputzKey)
        End Get
        Set(ByVal value As String)
            OutputzKey = Decrypt(value)
        End Set
    End Property

    Public OutputzUrlMode As OutputzUrlmode = MyCommon.OutputzUrlmode.twittercom
    Public AutoShortUrlFirst As UrlConverter = UrlConverter.Bitly
    Public UseUnreadStyle As Boolean = True
    Public DateTimeFormat As String = "yyyy/MM/dd H:mm:ss"
    Public DefaultTimeOut As Integer = 20
    'Public ProtectNotInclude As Boolean = True
    Public RetweetNoConfirm As Boolean = False
    Public LimitBalloon As Boolean = False
    Public TabIconDisp As Boolean = True
    Public ReplyIconState As REPLY_ICONSTATE = REPLY_ICONSTATE.StaticIcon
    Public WideSpaceConvert As Boolean = True
    Public ReadOwnPost As Boolean = False
    Public GetFav As Boolean = True
    Public BilyUser As String = ""
    Public BitlyPwd As String = ""
    Public ShowGrid As Boolean = False
    Public UseAtIdSupplement As Boolean = True
    Public UseHashSupplement As Boolean = True
    Public TwitterUrl As String = "api.twitter.com"
    Public TwitterSearchUrl As String = "search.twitter.com"
    Public IsOAuth As Boolean = True
    Public HotkeyEnabled As Boolean = False
    Public HotkeyModifier As Keys = Keys.None
    Public HotkeyKey As Keys = Keys.None
    Public HotkeyValue As Integer = 0
    Public BlinkNewMentions As Boolean = False
    Public FocusLockToStatusText As Boolean = False
    Public UseAdditionalCount As Boolean = False
    Public MoreCountApi As Integer = 200
    Public FirstCountApi As Integer = 100
    Public SearchCountApi As Integer = 100
    Public FavoritesCountApi As Integer = 40
    Public TrackWord As String = ""
    Public AllAtReply As Boolean = False
    Public UserstreamPeriod As Integer = 3
    Public UserstreamStartup As Boolean = True
End Class
