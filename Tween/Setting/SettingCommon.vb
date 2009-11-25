<Serializable()> _
Public Class SettingCommon
    Inherits SettingBase(Of SettingCommon)

#Region "SettingƒNƒ‰ƒXŠî–{"
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
            Dim pwd As String = Password
            If String.IsNullOrEmpty(pwd) Then pwd = ""
            If pwd.Length > 0 Then
                Try
                    Return EncryptString(pwd)
                Catch ex As Exception
                    Return ""
                End Try
            Else
                Return ""
            End If
        End Get
        Set(ByVal value As String)
            Dim pwd As String = value
            If String.IsNullOrEmpty(pwd) Then pwd = ""
            If pwd.Length > 0 Then
                Try
                    pwd = DecryptString(pwd)
                Catch ex As Exception
                    pwd = ""
                End Try
            End If
            Password = pwd
        End Set
    End Property

    Public TabList As New List(Of String)
    Public NextPageThreshold As Integer = 20
    Public NextPages As Integer = 1
    Public TimelinePeriod As Integer = 90
    Public ReplyPeriod As Integer = 600
    Public DMPeriod As Integer = 600
    Public ReadPages As Integer = 1
    Public ReadPagesReply As Integer = 1
    Public ReadPagesDM As Integer = 1
    Public MaxPostNum As Integer = 125
    Public Read As Boolean = True
    Public ListLock As Boolean = False
    Public IconSize As IconSizes = IconSizes.Icon16
    Public NewAllPop As Boolean = True
    Public PlaySound As Boolean = False
    Public UnreadManage As Boolean = True
    Public OneWayLove As Boolean = True
    Public NameBalloon As NameBalloonEnum = NameBalloonEnum.NickName
    Public PostCtrlEnter As Boolean = False
    Public UseApi As Boolean = True
    Public UsePostMethod As Boolean = False
    Public CountApi As Integer = 60
    Public CheckReply As Boolean = True
    Public PostAndGet As Boolean = True
    Public DispUsername As Boolean = False
    Public MinimizeToTray As Boolean = False
    Public CloseToExit As Boolean = False
    Public DispLatestPost As DispTitleEnum = DispTitleEnum.Post
    Public HubServer As String = "twitter.com"
    Public SortOrderLock As Boolean = False
    Public TinyUrlResolve As Boolean = True
    Public PeriodAdjust As Boolean = True
    Public StartupVersion As Boolean = True
    Public StartupKey As Boolean = True
    Public StartupFollowers As Boolean = True
    Public StartupApiModeNoWarning As Boolean = False
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

    <Xml.Serialization.XmlIgnore()> _
    Public OutputzKey As String = ""
    Public Property EncryptOutputzKey() As String
        Get
            Dim pwd As String = OutputzKey
            If String.IsNullOrEmpty(pwd) Then pwd = ""
            If pwd.Length > 0 Then
                Try
                    Return EncryptString(pwd)
                Catch ex As Exception
                    Return ""
                End Try
            Else
                Return ""
            End If
        End Get
        Set(ByVal value As String)
            Dim pwd As String = value
            If String.IsNullOrEmpty(pwd) Then pwd = ""
            If pwd.Length > 0 Then
                Try
                    pwd = DecryptString(pwd)
                Catch ex As Exception
                    pwd = ""
                End Try
            End If
            OutputzKey = pwd
        End Set
    End Property

    Public OutputzUrlMode As OutputzUrlmode = MyCommon.OutputzUrlmode.twittercom
    Public AutoShortUrlFirst As UrlConverter = UrlConverter.Bitly
    Public UseUnreadStyle As Boolean = True
    Public DateTimeFormat As String = "yyyy/MM/dd H:mm:ss"
    Public DefaultTimeOut As Integer = 20
    Public ProtectNotInclude As Boolean = True
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
End Class
