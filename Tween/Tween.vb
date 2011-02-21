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

'コンパイル後コマンド
'"c:\Program Files\Microsoft.NET\SDK\v2.0\Bin\sgen.exe" /f /a:"$(TargetPath)"
'"C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\sgen.exe" /f /a:"$(TargetPath)"


Imports System
Imports System.Text
Imports System.Text.RegularExpressions
Imports Tween.TweenCustomControl
Imports System.IO
Imports System.Web
Imports System.Reflection
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Threading
Imports System.Linq

Public Class TweenMain

    '各種設定
    Private _mySize As Size             '画面サイズ
    Private _myLoc As Point             '画面位置
    Private _mySpDis As Integer         '区切り位置
    Private _mySpDis2 As Integer        '発言欄区切り位置
    Private _mySpDis3 As Integer        'プレビュー区切り位置
    Private _iconSz As Integer            'アイコンサイズ（現在は16、24、48の3種類。将来直接数字指定可能とする 注：24x24の場合に26と指定しているのはMSゴシック系フォントのための仕様）
    Private _iconCol As Boolean           '1列表示の時True（48サイズのとき）

    '雑多なフラグ類
    Private _initial As Boolean         'True:起動時処理中
    Private _initialLayout As Boolean = True
    Private _ignoreConfigSave As Boolean         'True:起動時処理中
    Private _tabDrag As Boolean           'タブドラッグ中フラグ（DoDragDropを実行するかの判定用）
    Private _beforeSelectedTab As TabPage 'タブが削除されたときに前回選択されていたときのタブを選択する為に保持
    Private _tabMouseDownPoint As Point
    Private _rclickTabName As String      '右クリックしたタブの名前（Tabコントロール機能不足対応）
    Private ReadOnly _syncObject As New Object()    'ロック用
    Private Const detailHtmlFormatMono1 As String = "<html><head><style type=""text/css""><!-- pre {font-family: """
    Private Const detailHtmlFormat2 As String = """, sans-serif; font-size: "
    Private Const detailHtmlFormat3 As String = "pt; word-wrap: break-word; color:rgb("
    Private Const detailHtmlFormat4 As String = ");} a:link, a:visited, a:active, a:hover {color:rgb("
    Private Const detailHtmlFormat5 As String = "); } --></style></head><body style=""margin:0px; background-color:rgb("
    Private Const detailHtmlFormatMono6 As String = ");""><pre>"
    Private Const detailHtmlFormatMono7 As String = "</pre></body></html>"
    Private Const detailHtmlFormat1 As String = "<html><head><style type=""text/css""><!-- p {font-family: """
    Private Const detailHtmlFormat6 As String = ");""><p>"
    Private Const detailHtmlFormat7 As String = "</p></body></html>"
    Private detailHtmlFormatHeader As String
    Private detailHtmlFormatFooter As String
    Private _myStatusError As Boolean = False
    Private _myStatusOnline As Boolean = False
    Private soundfileListup As Boolean = False
    Private _spaceKeyCanceler As SpaceKeyCanceler

    '設定ファイル関連
    'Private _cfg As SettingToConfig '旧
    Private _cfgLocal As SettingLocal
    Private _cfgCommon As SettingCommon
    Private _modifySettingLocal As Boolean = False
    Private _modifySettingCommon As Boolean = False
    Private _modifySettingAtId As Boolean = False

    'twitter解析部
    Private tw As New Twitter

    'サブ画面インスタンス
    Private SettingDialog As AppendSettingDialog = AppendSettingDialog.Instance       '設定画面インスタンス
    Private TabDialog As New TabsDialog        'タブ選択ダイアログインスタンス
    Private SearchDialog As New SearchWord     '検索画面インスタンス
    Private fltDialog As New FilterDialog 'フィルター編集画面
    Private UrlDialog As New OpenURL
    Private dialogAsShieldicon As DialogAsShieldIcon    ' シールドアイコン付きダイアログ
    Public AtIdSupl As AtIdSupplement    '@id補助
    Public HashSupl As AtIdSupplement    'Hashtag補助
    Public HashMgr As HashtagManage
    Private evtDialog As EventViewerDialog

    '表示フォント、色、アイコン
    Private _fntUnread As Font            '未読用フォント
    Private _clUnread As Color            '未読用文字色
    Private _fntReaded As Font            '既読用フォント
    Private _clReaded As Color            '既読用文字色
    Private _clFav As Color               'Fav用文字色
    Private _clOWL As Color               '片思い用文字色
    Private _clRetweet As Color               'Retweet用文字色
    Private _fntDetail As Font            '発言詳細部用フォント
    Private _clDetail As Color              '発言詳細部用色
    Private _clDetailLink As Color          '発言詳細部用リンク文字色
    Private _clDetailBackcolor As Color     '発言詳細部用背景色
    Private _clSelf As Color              '自分の発言用背景色
    Private _clAtSelf As Color            '自分宛返信用背景色
    Private _clTarget As Color            '選択発言者の他の発言用背景色
    Private _clAtTarget As Color          '選択発言中の返信先用背景色
    Private _clAtFromTarget As Color      '選択発言者への返信発言用背景色
    Private _clAtTo As Color              '選択発言の唯一＠先
    Private _clListBackcolor As Color       'リスト部通常発言背景色
    Private _clInputBackcolor As Color      '入力欄背景色
    Private _clInputFont As Color           '入力欄文字色
    Private _fntInputFont As Font           '入力欄フォント
    Private TIconDic As IDictionary(Of String, Image)        'アイコン画像リスト
    Private NIconAt As Icon               'At.ico             タスクトレイアイコン：通常時
    Private NIconAtRed As Icon            'AtRed.ico          タスクトレイアイコン：通信エラー時
    Private NIconAtSmoke As Icon          'AtSmoke.ico        タスクトレイアイコン：オフライン時
    Private NIconRefresh(3) As Icon       'Refresh.ico        タスクトレイアイコン：更新中（アニメーション用に4種類を保持するリスト）
    Private TabIcon As Icon               'Tab.ico            未読のあるタブ用アイコン
    Private MainIcon As Icon              'Main.ico           画面左上のアイコン
    Private ReplyIcon As Icon               '5g
    Private ReplyIconBlink As Icon          '6g

    Private _anchorPost As PostClass
    Private _anchorFlag As Boolean        'True:関連発言移動中（関連移動以外のオペレーションをするとFalseへ。Trueだとリスト背景色をアンカー発言選択中として描画）

    Private _history As New List(Of PostingStatus)   '発言履歴
    Private _hisIdx As Integer                  '発言履歴カレントインデックス

    '発言投稿時のAPI引数（発言編集時に設定。手書きreplyでは設定されない）
    Private _reply_to_id As Long     ' リプライ先のステータスID 0の場合はリプライではない 注：複数あてのものはリプライではない
    Private _reply_to_name As String    ' リプライ先ステータスの書き込み者の名前

    '時速表示用
    Private _postTimestamps As New List(Of Date)
    Private _favTimestamps As New List(Of Date)
    Private _tlTimestamps As New Dictionary(Of Date, Integer)
    Private _tlCount As Integer

    ' 以下DrawItem関連
    Private _brsHighLight As New SolidBrush(Color.FromKnownColor(KnownColor.Highlight))
    Private _brsHighLightText As New SolidBrush(Color.FromKnownColor(KnownColor.HighlightText))
    Private _brsForeColorUnread As SolidBrush
    Private _brsForeColorReaded As SolidBrush
    Private _brsForeColorFav As SolidBrush
    Private _brsForeColorOWL As SolidBrush
    Private _brsForeColorRetweet As SolidBrush
    Private _brsBackColorMine As SolidBrush
    Private _brsBackColorAt As SolidBrush
    Private _brsBackColorYou As SolidBrush
    Private _brsBackColorAtYou As SolidBrush
    Private _brsBackColorAtFromTarget As SolidBrush
    Private _brsBackColorAtTo As SolidBrush
    Private _brsBackColorNone As SolidBrush
    Private _brsDeactiveSelection As New SolidBrush(Color.FromKnownColor(KnownColor.ButtonFace)) 'Listにフォーカスないときの選択行の背景色
    Private sfTab As New StringFormat()

    '''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private _apiGauge As New ToolStripAPIGauge()
    Private _statuses As TabInformations
    Private _itemCache() As ListViewItem
    Private _itemCacheIndex As Integer
    Private _postCache() As PostClass
    Private _curTab As TabPage
    Private _curItemIndex As Integer
    Private _curList As DetailsListView
    Private _curPost As PostClass
    Private _isColumnChanged As Boolean = False
    Private _waitTimeline As Boolean = False
    Private _waitReply As Boolean = False
    Private _waitDm As Boolean = False
    Private _waitFav As Boolean = False
    Private _waitPubSearch As Boolean = False
    Private _waitUserTimeline As Boolean = False
    Private _waitLists As Boolean = False
    Private _bw(18) As BackgroundWorker
    Private _bwFollower As BackgroundWorker
    Private cMode As Integer
    Private shield As New ShieldIcon
    Private SecurityManager As InternetSecurityManager
    Private Thumbnail As Thumbnail

    Private UnreadCounter As Integer = -1
    Private UnreadAtCounter As Integer = -1

    Private ColumnOrgText(8) As String
    Private ColumnText(8) As String

    Private _DoFavRetweetFlags As Boolean = False

    '''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private _postBrowserStatusText As String = ""

    Private _colorize As Boolean = False

    Private WithEvents TimerTimeline As New System.Timers.Timer

    Private WithEvents displayItem As ImageListViewItem

    'URL短縮のUndo用
    Private Structure urlUndo
        Public Before As String
        Public After As String
    End Structure

    Private urlUndoBuffer As Generic.List(Of urlUndo) = Nothing

    Private Structure ReplyChain
        Public OriginalId As Long
        Public InReplyToId As Long
        Public OriginalTab As TabPage

        Sub New(ByVal originalId As Long, ByVal inReplyToId As Long, ByVal originalTab As TabPage)
            Me.OriginalId = originalId
            Me.InReplyToId = inReplyToId
            Me.OriginalTab = originalTab
        End Sub
    End Structure

    Private replyChains As Stack(Of ReplyChain)

    'Backgroundworkerの処理結果通知用引数構造体
    Private Class GetWorkerResult
        Public retMsg As String = ""                     '処理結果詳細メッセージ。エラー時に値がセットされる
        Public page As Integer                      '取得対象ページ番号
        Public endPage As Integer                   '取得終了ページ番号（継続可能ならインクリメントされて返る。pageと比較して継続判定）
        Public type As WORKERTYPE                   '処理種別
        Public imgs As Dictionary(Of String, Image)                    '新規取得したアイコンイメージ
        Public tName As String = ""                  'Fav追加・削除時のタブ名
        Public ids As List(Of Long)               'Fav追加・削除時のID
        Public sIds As List(Of Long)                  'Fav追加・削除成功分のID
        Public newDM As Boolean
        Public addCount As Integer
        Public status As PostingStatus
    End Class

    'Backgroundworkerへ処理内容を通知するための引数用構造体
    Private Class GetWorkerArg
        Public page As Integer                      '処理対象ページ番号
        Public endPage As Integer                   '処理終了ページ番号（起動時の読み込みページ数。通常時はpageと同じ値をセット）
        Public type As WORKERTYPE                   '処理種別
        Public url As String = ""            'URLをブラウザで開くときのアドレス
        Public status As New PostingStatus          '発言POST時の発言内容
        Public ids As List(Of Long)               'Fav追加・削除時のItemIndex
        Public sIds As List(Of Long)              'Fav追加・削除成功分のItemIndex
        Public tName As String = ""            'Fav追加・削除時のタブ名
    End Class

    '検索処理タイプ
    Private Enum SEARCHTYPE
        DialogSearch
        NextSearch
        PrevSearch
    End Enum

    Private Class PostingStatus
        Public status As String = ""
        Public inReplyToId As Long = 0
        Public inReplyToName As String = ""
        Public imageService As String = ""      '画像投稿サービス名
        Public imagePath As String = ""
        Public Sub New()

        End Sub
        Public Sub New(ByVal status As String, ByVal replyToId As Long, ByVal replyToName As String)
            Me.status = status
            Me.inReplyToId = replyToId
            Me.inReplyToName = replyToName
        End Sub
    End Class

    Private Class SpaceKeyCanceler
        Inherits NativeWindow
        Implements IDisposable

        Dim WM_KEYDOWN As Integer = &H100
        Dim VK_SPACE As Integer = &H20

        Public Sub New(ByVal control As Control)
            Me.AssignHandle(control.Handle)
        End Sub

        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            If (m.Msg = WM_KEYDOWN) AndAlso (CInt(m.WParam) = VK_SPACE) Then
                RaiseEvent SpaceCancel(Me, EventArgs.Empty)
                Exit Sub
            End If

            MyBase.WndProc(m)
        End Sub

        Public Event SpaceCancel As EventHandler

        Public Sub Dispose() Implements IDisposable.Dispose
            Me.ReleaseHandle()
        End Sub
    End Class

    Private Sub TweenMain_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        '画面がアクティブになったら、発言欄の背景色戻す
        If StatusText.Focused Then
            Me.StatusText_Enter(Me.StatusText, System.EventArgs.Empty)
        End If
    End Sub

    Private Sub TweenMain_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        '後始末
        SettingDialog.Dispose()
        TabDialog.Dispose()
        SearchDialog.Dispose()
        fltDialog.Dispose()
        UrlDialog.Dispose()
        _spaceKeyCanceler.Dispose()
        If NIconAt IsNot Nothing Then NIconAt.Dispose()
        If NIconAtRed IsNot Nothing Then NIconAtRed.Dispose()
        If NIconAtSmoke IsNot Nothing Then NIconAtSmoke.Dispose()
        If NIconRefresh(0) IsNot Nothing Then NIconRefresh(0).Dispose()
        If NIconRefresh(1) IsNot Nothing Then NIconRefresh(1).Dispose()
        If NIconRefresh(2) IsNot Nothing Then NIconRefresh(2).Dispose()
        If NIconRefresh(3) IsNot Nothing Then NIconRefresh(3).Dispose()
        If TabIcon IsNot Nothing Then TabIcon.Dispose()
        If MainIcon IsNot Nothing Then MainIcon.Dispose()
        If ReplyIcon IsNot Nothing Then ReplyIcon.Dispose()
        If ReplyIconBlink IsNot Nothing Then ReplyIconBlink.Dispose()
        _brsHighLight.Dispose()
        _brsHighLightText.Dispose()
        If _brsForeColorUnread IsNot Nothing Then _brsForeColorUnread.Dispose()
        If _brsForeColorReaded IsNot Nothing Then _brsForeColorReaded.Dispose()
        If _brsForeColorFav IsNot Nothing Then _brsForeColorFav.Dispose()
        If _brsForeColorOWL IsNot Nothing Then _brsForeColorOWL.Dispose()
        If _brsForeColorRetweet IsNot Nothing Then _brsForeColorRetweet.Dispose()
        If _brsBackColorMine IsNot Nothing Then _brsBackColorMine.Dispose()
        If _brsBackColorAt IsNot Nothing Then _brsBackColorAt.Dispose()
        If _brsBackColorYou IsNot Nothing Then _brsBackColorYou.Dispose()
        If _brsBackColorAtYou IsNot Nothing Then _brsBackColorAtYou.Dispose()
        If _brsBackColorAtFromTarget IsNot Nothing Then _brsBackColorAtFromTarget.Dispose()
        If _brsBackColorAtTo IsNot Nothing Then _brsBackColorAtTo.Dispose()
        If _brsBackColorNone IsNot Nothing Then _brsBackColorNone.Dispose()
        If _brsDeactiveSelection IsNot Nothing Then _brsDeactiveSelection.Dispose()
        shield.Dispose()
        'sf.Dispose()
        sfTab.Dispose()
        For Each bw As BackgroundWorker In _bw
            If bw IsNot Nothing Then
                bw.Dispose()
            End If
        Next
        If _bwFollower IsNot Nothing Then
            _bwFollower.Dispose()
        End If
        Me._apiGauge.Dispose()
        If TIconDic IsNot Nothing Then DirectCast(TIconDic, IDisposable).Dispose()
    End Sub

    Private Sub LoadIcon(ByRef IconInstance As Icon, ByVal FileName As String)
        Dim dir As String = Application.StartupPath
        If File.Exists(Path.Combine(dir, FileName)) Then
            Try
                IconInstance = New Icon(Path.Combine(dir, FileName))
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub LoadIcons()
        '着せ替えアイコン対応
        'タスクトレイ通常時アイコン
        Dim dir As String = Application.StartupPath

        NIconAt = My.Resources.At
        NIconAtRed = My.Resources.AtRed
        NIconAtSmoke = My.Resources.AtSmoke
        NIconRefresh(0) = My.Resources.Refresh
        NIconRefresh(1) = My.Resources.Refresh2
        NIconRefresh(2) = My.Resources.Refresh3
        NIconRefresh(3) = My.Resources.Refresh4
        TabIcon = My.Resources.TabIcon
        MainIcon = My.Resources.MIcon
        ReplyIcon = My.Resources.Reply
        ReplyIconBlink = My.Resources.ReplyBlink

        If Not Directory.Exists(Path.Combine(dir, "Icons")) Then
            Exit Sub
        End If

        LoadIcon(NIconAt, "Icons\At.ico")

        'タスクトレイエラー時アイコン
        LoadIcon(NIconAtRed, "Icons\AtRed.ico")

        'タスクトレイオフライン時アイコン
        LoadIcon(NIconAtSmoke, "Icons\AtSmoke.ico")

        'タスクトレイ更新中アイコン
        'アニメーション対応により4種類読み込み
        LoadIcon(NIconRefresh(0), "Icons\Refresh.ico")
        LoadIcon(NIconRefresh(1), "Icons\Refresh2.ico")
        LoadIcon(NIconRefresh(2), "Icons\Refresh3.ico")
        LoadIcon(NIconRefresh(3), "Icons\Refresh4.ico")

        'タブ見出し未読表示アイコン
        LoadIcon(TabIcon, "Icons\Tab.ico")

        '画面のアイコン
        LoadIcon(MainIcon, "Icons\MIcon.ico")

        'Replyのアイコン
        LoadIcon(ReplyIcon, "Icons\Reply.ico")

        'Reply点滅のアイコン
        LoadIcon(ReplyIconBlink, "Icons\ReplyBlink.ico")
    End Sub

    Private Sub InitColumnText()

        ColumnText(0) = ""
        ColumnText(1) = My.Resources.AddNewTabText2
        ColumnText(2) = My.Resources.AddNewTabText3
        ColumnText(3) = My.Resources.AddNewTabText4_2
        ColumnText(4) = My.Resources.AddNewTabText5
        ColumnText(5) = ""
        ColumnText(6) = ""
        ColumnText(7) = "Source"

        ColumnOrgText(0) = ""
        ColumnOrgText(1) = My.Resources.AddNewTabText2
        ColumnOrgText(2) = My.Resources.AddNewTabText3
        ColumnOrgText(3) = My.Resources.AddNewTabText4_2
        ColumnOrgText(4) = My.Resources.AddNewTabText5
        ColumnOrgText(5) = ""
        ColumnOrgText(6) = ""
        ColumnOrgText(7) = "Source"

        Dim c As Integer = 0
        Select Case _statuses.SortMode
            Case IdComparerClass.ComparerMode.Nickname  'ニックネーム
                c = 1
            Case IdComparerClass.ComparerMode.Data  '本文
                c = 2
            Case IdComparerClass.ComparerMode.Id  '時刻=発言Id
                c = 3
            Case IdComparerClass.ComparerMode.Name  '名前
                c = 4
            Case IdComparerClass.ComparerMode.Source  'Source
                c = 7
        End Select

        If _iconCol Then
            If _statuses.SortOrder() = SortOrder.Descending Then
                ' U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                ColumnText(2) = ColumnOrgText(2) + "▾"
            Else
                ' U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                ColumnText(2) = ColumnOrgText(2) + "▴"
            End If
        Else
            If _statuses.SortOrder() = SortOrder.Descending Then
                ' U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                ColumnText(c) = ColumnOrgText(c) + "▾"
            Else
                ' U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                ColumnText(c) = ColumnOrgText(c) + "▴"
            End If
        End If
    End Sub

    Private Sub InitializeTraceFrag()
#If DEBUG Then
        TraceOutToolStripMenuItem.Checked = True
        TraceFlag = True
#End If
        If Not fileVersion.EndsWith("0") Then
            TraceOutToolStripMenuItem.Checked = True
            TraceFlag = True
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        _ignoreConfigSave = True
        Me.Visible = False
        SecurityManager = New InternetSecurityManager(PostBrowser)
        Thumbnail = New Thumbnail(Me)

        AddHandler TwitterApiInfo.Changed, AddressOf SetStatusLabelApiHandler

        VerUpMenuItem.Image = shield.Icon
        If Not My.Application.CommandLineArgs.Count = 0 AndAlso My.Application.CommandLineArgs.Contains("/d") Then TraceFlag = True

        Me._spaceKeyCanceler = New SpaceKeyCanceler(Me.PostButton)
        AddHandler Me._spaceKeyCanceler.SpaceCancel, AddressOf spaceKeyCanceler_SpaceCancel

        Regex.CacheSize = 100

        fileVersion = DirectCast(Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyFileVersionAttribute), False)(0), AssemblyFileVersionAttribute).Version
        InitializeTraceFrag()
        LoadIcons() ' アイコン読み込み

        '発言保持クラス
        _statuses = TabInformations.GetInstance()

        'アイコン設定
        Me.Icon = MainIcon              'メインフォーム（TweenMain）
        NotifyIcon1.Icon = NIconAt      'タスクトレイ
        TabImage.Images.Add(TabIcon)    'タブ見出し

        SettingDialog.Owner = Me
        SearchDialog.Owner = Me
        fltDialog.Owner = Me
        TabDialog.Owner = Me
        UrlDialog.Owner = Me

        _history.Add(New PostingStatus)
        _hisIdx = 0
        _reply_to_id = 0
        _reply_to_name = ""

        '<<<<<<<<<設定関連>>>>>>>>>
        '設定コンバージョン
        'ConvertConfig()

        ''設定読み出し
        LoadConfig()

        '新着バルーン通知のチェック状態設定
        NewPostPopMenuItem.Checked = _cfgCommon.NewAllPop
        Me.NotifyFileMenuItem.Checked = NewPostPopMenuItem.Checked

        'フォント＆文字色＆背景色保持
        _fntUnread = _cfgLocal.FontUnread
        _clUnread = _cfgLocal.ColorUnread
        _fntReaded = _cfgLocal.FontRead
        _clReaded = _cfgLocal.ColorRead
        _clFav = _cfgLocal.ColorFav
        _clOWL = _cfgLocal.ColorOWL
        _clRetweet = _cfgLocal.ColorRetweet
        _fntDetail = _cfgLocal.FontDetail
        _clDetail = _cfgLocal.ColorDetail
        _clDetailLink = _cfgLocal.ColorDetailLink
        _clDetailBackcolor = _cfgLocal.ColorDetailBackcolor
        _clSelf = _cfgLocal.ColorSelf
        _clAtSelf = _cfgLocal.ColorAtSelf
        _clTarget = _cfgLocal.ColorTarget
        _clAtTarget = _cfgLocal.ColorAtTarget
        _clAtFromTarget = _cfgLocal.ColorAtFromTarget
        _clAtTo = _cfgLocal.ColorAtTo
        _clListBackcolor = _cfgLocal.ColorListBackcolor
        _clInputBackcolor = _cfgLocal.ColorInputBackcolor
        _clInputFont = _cfgLocal.ColorInputFont
        _fntInputFont = _cfgLocal.FontInputFont

        _brsForeColorUnread = New SolidBrush(_clUnread)
        _brsForeColorReaded = New SolidBrush(_clReaded)
        _brsForeColorFav = New SolidBrush(_clFav)
        _brsForeColorOWL = New SolidBrush(_clOWL)
        _brsForeColorRetweet = New SolidBrush(_clRetweet)
        _brsBackColorMine = New SolidBrush(_clSelf)
        _brsBackColorAt = New SolidBrush(_clAtSelf)
        _brsBackColorYou = New SolidBrush(_clTarget)
        _brsBackColorAtYou = New SolidBrush(_clAtTarget)
        _brsBackColorAtFromTarget = New SolidBrush(_clAtFromTarget)
        _brsBackColorAtTo = New SolidBrush(_clAtTo)
        '_brsBackColorNone = New SolidBrush(Color.FromKnownColor(KnownColor.Window))
        _brsBackColorNone = New SolidBrush(_clListBackcolor)

        ' StringFormatオブジェクトへの事前設定
        'sf.Alignment = StringAlignment.Near             ' Textを近くへ配置（左から右の場合は左寄せ）
        'sf.LineAlignment = StringAlignment.Near         ' Textを近くへ配置（上寄せ）
        'sf.FormatFlags = StringFormatFlags.LineLimit    ' 
        sfTab.Alignment = StringAlignment.Center
        sfTab.LineAlignment = StringAlignment.Center

        '設定画面への反映
        SettingDialog.IsOAuth = _cfgCommon.IsOAuth
        HttpTwitter.TwitterUrl = _cfgCommon.TwitterUrl
        HttpTwitter.TwitterSearchUrl = _cfgCommon.TwitterSearchUrl
        SettingDialog.TwitterApiUrl = _cfgCommon.TwitterUrl
        SettingDialog.TwitterSearchApiUrl = _cfgCommon.TwitterSearchUrl
        '認証関連
        If _cfgCommon.IsOAuth Then
            If _cfgCommon.Token = "" Then _cfgCommon.UserName = ""
            tw.Initialize(_cfgCommon.Token, _cfgCommon.TokenSecret, _cfgCommon.UserName)
        Else
            tw.Initialize(_cfgCommon.UserName, _cfgCommon.Password)
        End If

        SettingDialog.TimelinePeriodInt = _cfgCommon.TimelinePeriod
        SettingDialog.ReplyPeriodInt = _cfgCommon.ReplyPeriod
        SettingDialog.DMPeriodInt = _cfgCommon.DMPeriod
        SettingDialog.PubSearchPeriodInt = _cfgCommon.PubSearchPeriod
        SettingDialog.UserTimelinePeriodInt = _cfgCommon.UserTimelinePeriod
        SettingDialog.ListsPeriodInt = _cfgCommon.ListsPeriod
        '不正値チェック
        If Not My.Application.CommandLineArgs.Contains("nolimit") Then
            If SettingDialog.TimelinePeriodInt < 15 AndAlso SettingDialog.TimelinePeriodInt > 0 Then SettingDialog.TimelinePeriodInt = 15
            If SettingDialog.ReplyPeriodInt < 15 AndAlso SettingDialog.ReplyPeriodInt > 0 Then SettingDialog.ReplyPeriodInt = 15
            If SettingDialog.DMPeriodInt < 15 AndAlso SettingDialog.DMPeriodInt > 0 Then SettingDialog.DMPeriodInt = 15
            If SettingDialog.PubSearchPeriodInt < 30 AndAlso SettingDialog.PubSearchPeriodInt > 0 Then SettingDialog.PubSearchPeriodInt = 30
            If SettingDialog.UserTimelinePeriodInt < 15 AndAlso SettingDialog.UserTimelinePeriodInt > 0 Then SettingDialog.UserTimelinePeriodInt = 15
            If SettingDialog.ListsPeriodInt < 15 AndAlso SettingDialog.ListsPeriodInt > 0 Then SettingDialog.ListsPeriodInt = 15
        End If

        '起動時読み込み分を既読にするか。Trueなら既読として処理
        SettingDialog.Readed = _cfgCommon.Read
        '新着取得時のリストスクロールをするか。Trueならスクロールしない
        ListLockMenuItem.Checked = _cfgCommon.ListLock
        Me.LockListFileMenuItem.Checked = _cfgCommon.ListLock
        SettingDialog.IconSz = _cfgCommon.IconSize
        '文末ステータス
        SettingDialog.Status = _cfgLocal.StatusText
        '未読管理。Trueなら未読管理する
        SettingDialog.UnreadManage = _cfgCommon.UnreadManage
        'サウンド再生（タブ別設定より優先）
        SettingDialog.PlaySound = _cfgCommon.PlaySound
        PlaySoundMenuItem.Checked = SettingDialog.PlaySound
        Me.PlaySoundFileMenuItem.Checked = SettingDialog.PlaySound
        '片思い表示。Trueなら片思い表示する
        SettingDialog.OneWayLove = _cfgCommon.OneWayLove
        'フォント＆文字色＆背景色
        SettingDialog.FontUnread = _fntUnread
        SettingDialog.ColorUnread = _clUnread
        SettingDialog.FontReaded = _fntReaded
        SettingDialog.ColorReaded = _clReaded
        SettingDialog.ColorFav = _clFav
        SettingDialog.ColorOWL = _clOWL
        SettingDialog.ColorRetweet = _clRetweet
        SettingDialog.FontDetail = _fntDetail
        SettingDialog.ColorDetail = _clDetail
        SettingDialog.ColorDetailLink = _clDetailLink
        SettingDialog.ColorDetailBackcolor = _clDetailBackcolor
        SettingDialog.ColorSelf = _clSelf
        SettingDialog.ColorAtSelf = _clAtSelf
        SettingDialog.ColorTarget = _clTarget
        SettingDialog.ColorAtTarget = _clAtTarget
        SettingDialog.ColorAtFromTarget = _clAtFromTarget
        SettingDialog.ColorAtTo = _clAtTo
        SettingDialog.ColorListBackcolor = _clListBackcolor
        SettingDialog.ColorInputBackcolor = _clInputBackcolor
        SettingDialog.ColorInputFont = _clInputFont
        SettingDialog.FontInputFont = _fntInputFont

        SettingDialog.NameBalloon = _cfgCommon.NameBalloon
        SettingDialog.PostCtrlEnter = _cfgCommon.PostCtrlEnter
        SettingDialog.PostShiftEnter = _cfgCommon.PostShiftEnter

        SettingDialog.CountApi = _cfgCommon.CountApi
        SettingDialog.CountApiReply = _cfgCommon.CountApiReply
        If SettingDialog.CountApi < 20 OrElse SettingDialog.CountApi > 200 Then SettingDialog.CountApi = 60
        If SettingDialog.CountApiReply < 20 OrElse SettingDialog.CountApiReply > 200 Then SettingDialog.CountApiReply = 40

        SettingDialog.BrowserPath = _cfgLocal.BrowserPath
        SettingDialog.PostAndGet = _cfgCommon.PostAndGet
        SettingDialog.UseRecommendStatus = _cfgLocal.UseRecommendStatus
        SettingDialog.DispUsername = _cfgCommon.DispUsername
        SettingDialog.CloseToExit = _cfgCommon.CloseToExit
        SettingDialog.MinimizeToTray = _cfgCommon.MinimizeToTray
        SettingDialog.DispLatestPost = _cfgCommon.DispLatestPost
        SettingDialog.SortOrderLock = _cfgCommon.SortOrderLock
        SettingDialog.TinyUrlResolve = _cfgCommon.TinyUrlResolve

        SettingDialog.SelectedProxyType = _cfgLocal.ProxyType
        SettingDialog.ProxyAddress = _cfgLocal.ProxyAddress
        SettingDialog.ProxyPort = _cfgLocal.ProxyPort
        SettingDialog.ProxyUser = _cfgLocal.ProxyUser
        SettingDialog.ProxyPassword = _cfgLocal.ProxyPassword

        SettingDialog.PeriodAdjust = _cfgCommon.PeriodAdjust
        SettingDialog.StartupVersion = _cfgCommon.StartupVersion
        SettingDialog.StartupFollowers = _cfgCommon.StartupFollowers
        SettingDialog.RestrictFavCheck = _cfgCommon.RestrictFavCheck
        SettingDialog.AlwaysTop = _cfgCommon.AlwaysTop
        SettingDialog.UrlConvertAuto = _cfgCommon.UrlConvertAuto

        SettingDialog.OutputzEnabled = _cfgCommon.Outputz
        SettingDialog.OutputzKey = _cfgCommon.OutputzKey
        SettingDialog.OutputzUrlmode = _cfgCommon.OutputzUrlMode

        SettingDialog.UseUnreadStyle = _cfgCommon.UseUnreadStyle
        SettingDialog.DefaultTimeOut = _cfgCommon.DefaultTimeOut
        SettingDialog.RetweetNoConfirm = _cfgCommon.RetweetNoConfirm
        SettingDialog.PlaySound = _cfgCommon.PlaySound
        SettingDialog.DateTimeFormat = _cfgCommon.DateTimeFormat
        SettingDialog.LimitBalloon = _cfgCommon.LimitBalloon
        SettingDialog.EventNotifyEnabled = _cfgCommon.EventNotifyEnabled
        SettingDialog.EventNotifyFlag = _cfgCommon.EventNotifyFlag
        SettingDialog.IsMyEventNotifyFlag = _cfgCommon.IsMyEventNotifyFlag
        SettingDialog.ForceEventNotify = _cfgCommon.ForceEventNotify
        SettingDialog.FavEventUnread = _cfgCommon.FavEventUnread
        SettingDialog.TranslateLanguage = _cfgCommon.TranslateLanguage
        SettingDialog.EventSoundFile = _cfgCommon.EventSoundFile

        '廃止サービスが選択されていた場合bit.lyへ読み替え
        If _cfgCommon.AutoShortUrlFirst < 0 Then
            _cfgCommon.AutoShortUrlFirst = Tween.UrlConverter.Bitly
        End If

        SettingDialog.AutoShortUrlFirst = _cfgCommon.AutoShortUrlFirst
        SettingDialog.TabIconDisp = _cfgCommon.TabIconDisp
        SettingDialog.ReplyIconState = _cfgCommon.ReplyIconState
        SettingDialog.ReadOwnPost = _cfgCommon.ReadOwnPost
        SettingDialog.GetFav = _cfgCommon.GetFav
        SettingDialog.ReadOldPosts = _cfgCommon.ReadOldPosts
        SettingDialog.UseSsl = _cfgCommon.UseSsl
        SettingDialog.BitlyUser = _cfgCommon.BilyUser
        SettingDialog.BitlyPwd = _cfgCommon.BitlyPwd
        SettingDialog.ShowGrid = _cfgCommon.ShowGrid
        SettingDialog.Language = _cfgCommon.Language
        SettingDialog.UseAtIdSupplement = _cfgCommon.UseAtIdSupplement
        SettingDialog.UseHashSupplement = _cfgCommon.UseHashSupplement
        SettingDialog.PreviewEnable = _cfgCommon.PreviewEnable
        AtIdSupl = New AtIdSupplement(SettingAtIdList.Load().AtIdList, "@")

        SettingDialog.IsMonospace = _cfgCommon.IsMonospace
        If SettingDialog.IsMonospace Then
            detailHtmlFormatHeader = detailHtmlFormatMono1
            detailHtmlFormatFooter = detailHtmlFormatMono7
        Else
            detailHtmlFormatHeader = detailHtmlFormat1
            detailHtmlFormatFooter = detailHtmlFormat7
        End If
        detailHtmlFormatHeader += _fntDetail.Name + detailHtmlFormat2 + _fntDetail.Size.ToString() + detailHtmlFormat3 + _clDetail.R.ToString + "," + _clDetail.G.ToString + "," + _clDetail.B.ToString + detailHtmlFormat4 + _clDetailLink.R.ToString + "," + _clDetailLink.G.ToString + "," + _clDetailLink.B.ToString + detailHtmlFormat5 + _clDetailBackcolor.R.ToString + "," + _clDetailBackcolor.G.ToString + "," + _clDetailBackcolor.B.ToString
        If SettingDialog.IsMonospace Then
            detailHtmlFormatHeader += detailHtmlFormatMono6
        Else
            detailHtmlFormatHeader += detailHtmlFormat6
        End If
        Me.IdeographicSpaceToSpaceToolStripMenuItem.Checked = _cfgCommon.WideSpaceConvert
        Me.ToolStripFocusLockMenuItem.Checked = _cfgCommon.FocusLockToStatusText

        'Dim statregex As New Regex("^0*")
        SettingDialog.RecommendStatusText = " [TWNv" + Regex.Replace(fileVersion.Replace(".", ""), "^0*", "") + "]"

        '書式指定文字列エラーチェック
        Try
            If DateTime.Now.ToString(SettingDialog.DateTimeFormat).Length = 0 Then
                ' このブロックは絶対に実行されないはず
                ' 変換が成功した場合にLengthが0にならない
                SettingDialog.DateTimeFormat = "yyyy/MM/dd H:mm:ss"
            End If
        Catch ex As FormatException
            ' FormatExceptionが発生したら初期値を設定 (=yyyy/MM/dd H:mm:ssとみなされる)
            SettingDialog.DateTimeFormat = "yyyy/MM/dd H:mm:ss"
        End Try

        SettingDialog.Nicoms = _cfgCommon.Nicoms
        SettingDialog.HotkeyEnabled = _cfgCommon.HotkeyEnabled
        SettingDialog.HotkeyMod = _cfgCommon.HotkeyModifier
        SettingDialog.HotkeyKey = _cfgCommon.HotkeyKey
        SettingDialog.HotkeyValue = _cfgCommon.HotkeyValue

        SettingDialog.BlinkNewMentions = _cfgCommon.BlinkNewMentions

        SettingDialog.UseAdditionalCount = _cfgCommon.UseAdditionalCount
        SettingDialog.MoreCountApi = _cfgCommon.MoreCountApi
        SettingDialog.FirstCountApi = _cfgCommon.FirstCountApi
        SettingDialog.SearchCountApi = _cfgCommon.SearchCountApi
        SettingDialog.FavoritesCountApi = _cfgCommon.FavoritesCountApi
        SettingDialog.UserTimelineCountApi = _cfgCommon.UserTimelineCountApi
        SettingDialog.ListCountApi = _cfgCommon.ListCountApi

        SettingDialog.UserstreamStartup = _cfgCommon.UserstreamStartup
        SettingDialog.UserstreamPeriodInt = _cfgCommon.UserstreamPeriod
        SettingDialog.OpenUserTimeline = _cfgCommon.OpenUserTimeline

        'ハッシュタグ関連
        HashSupl = New AtIdSupplement(_cfgCommon.HashTags, "#")
        HashMgr = New HashtagManage(HashSupl, _
                                _cfgCommon.HashTags.ToArray, _
                                _cfgCommon.HashSelected, _
                                _cfgCommon.HashIsPermanent, _
                                _cfgCommon.HashIsHead)
        If HashMgr.UseHash <> "" AndAlso HashMgr.IsPermanent Then HashStripSplitButton.Text = HashMgr.UseHash

        _initial = True

        'アイコンリスト作成
        Try
            TIconDic = New ImageDictionary(50)
        Catch ex As Exception
            MessageBox.Show("Please install [.NET Framework 4 (Full)].")
            Application.Exit()
            Exit Sub
        End Try

        Dim saveRequired As Boolean = False
        'ユーザー名、パスワードが未設定なら設定画面を表示（初回起動時など）
        If tw.Username = "" Then
            saveRequired = True
            '設定せずにキャンセルされた場合はプログラム終了
            If SettingDialog.ShowDialog() = Windows.Forms.DialogResult.Cancel Then
                Application.Exit()  '強制終了
                Exit Sub
            End If
            '設定されたが、依然ユーザー名とパスワードが未設定ならプログラム終了
            If tw.Username = "" Then
                Application.Exit()  '強制終了
                Exit Sub
            End If
            '新しい設定を反映
            'フォント＆文字色＆背景色保持
            _fntUnread = SettingDialog.FontUnread
            _clUnread = SettingDialog.ColorUnread
            _fntReaded = SettingDialog.FontReaded
            _clReaded = SettingDialog.ColorReaded
            _clFav = SettingDialog.ColorFav
            _clOWL = SettingDialog.ColorOWL
            _clRetweet = SettingDialog.ColorRetweet
            _fntDetail = SettingDialog.FontDetail
            _clDetail = SettingDialog.ColorDetail
            _clDetailLink = SettingDialog.ColorDetailLink
            _clDetailBackcolor = SettingDialog.ColorDetailBackcolor
            _clSelf = SettingDialog.ColorSelf
            _clAtSelf = SettingDialog.ColorAtSelf
            _clTarget = SettingDialog.ColorTarget
            _clAtTarget = SettingDialog.ColorAtTarget
            _clAtFromTarget = SettingDialog.ColorAtFromTarget
            _clAtTo = SettingDialog.ColorAtTo
            _clListBackcolor = SettingDialog.ColorListBackcolor
            _clInputBackcolor = SettingDialog.ColorInputBackcolor
            _clInputFont = SettingDialog.ColorInputFont
            _fntInputFont = SettingDialog.FontInputFont
            _brsForeColorUnread.Dispose()
            _brsForeColorReaded.Dispose()
            _brsForeColorFav.Dispose()
            _brsForeColorOWL.Dispose()
            _brsForeColorRetweet.Dispose()
            _brsForeColorUnread = New SolidBrush(_clUnread)
            _brsForeColorReaded = New SolidBrush(_clReaded)
            _brsForeColorFav = New SolidBrush(_clFav)
            _brsForeColorOWL = New SolidBrush(_clOWL)
            _brsForeColorRetweet = New SolidBrush(_clRetweet)
            _brsBackColorMine.Dispose()
            _brsBackColorAt.Dispose()
            _brsBackColorYou.Dispose()
            _brsBackColorAtYou.Dispose()
            _brsBackColorAtFromTarget.Dispose()
            _brsBackColorAtTo.Dispose()
            _brsBackColorNone.Dispose()
            _brsBackColorMine = New SolidBrush(_clSelf)
            _brsBackColorAt = New SolidBrush(_clAtSelf)
            _brsBackColorYou = New SolidBrush(_clTarget)
            _brsBackColorAtYou = New SolidBrush(_clAtTarget)
            _brsBackColorAtFromTarget = New SolidBrush(_clAtFromTarget)
            _brsBackColorAtTo = New SolidBrush(_clAtTo)
            _brsBackColorNone = New SolidBrush(_clListBackcolor)

            If SettingDialog.IsMonospace Then
                detailHtmlFormatHeader = detailHtmlFormatMono1
                detailHtmlFormatFooter = detailHtmlFormatMono7
            Else
                detailHtmlFormatHeader = detailHtmlFormat1
                detailHtmlFormatFooter = detailHtmlFormat7
            End If
            detailHtmlFormatHeader += _fntDetail.Name + detailHtmlFormat2 + _fntDetail.Size.ToString() + detailHtmlFormat3 + _clDetail.R.ToString + "," + _clDetail.G.ToString + "," + _clDetail.B.ToString + detailHtmlFormat4 + _clDetailLink.R.ToString + "," + _clDetailLink.G.ToString + "," + _clDetailLink.B.ToString + detailHtmlFormat5 + _clDetailBackcolor.R.ToString + "," + _clDetailBackcolor.G.ToString + "," + _clDetailBackcolor.B.ToString
            If SettingDialog.IsMonospace Then
                detailHtmlFormatHeader += detailHtmlFormatMono6
            Else
                detailHtmlFormatHeader += detailHtmlFormat6
            End If
            '他の設定項目は、随時設定画面で保持している値を読み出して使用
        End If

        If SettingDialog.HotkeyEnabled Then
            '''グローバルホットキーの登録
            Dim modKey As HookGlobalHotkey.ModKeys = HookGlobalHotkey.ModKeys.None
            If (SettingDialog.HotkeyMod And Keys.Alt) = Keys.Alt Then modKey = modKey Or HookGlobalHotkey.ModKeys.Alt
            If (SettingDialog.HotkeyMod And Keys.Control) = Keys.Control Then modKey = modKey Or HookGlobalHotkey.ModKeys.Ctrl
            If (SettingDialog.HotkeyMod And Keys.Shift) = Keys.Shift Then modKey = modKey Or HookGlobalHotkey.ModKeys.Shift
            If (SettingDialog.HotkeyMod And Keys.LWin) = Keys.LWin Then modKey = modKey Or HookGlobalHotkey.ModKeys.Win

            _hookGlobalHotkey.RegisterOriginalHotkey(SettingDialog.HotkeyKey, SettingDialog.HotkeyValue, modKey)
        End If

        'Twitter用通信クラス初期化
        HttpConnection.InitializeConnection(SettingDialog.DefaultTimeOut, _
                                            SettingDialog.SelectedProxyType, _
                                            SettingDialog.ProxyAddress, _
                                            SettingDialog.ProxyPort, _
                                            SettingDialog.ProxyUser, _
                                            SettingDialog.ProxyPassword)

        tw.RestrictFavCheck = SettingDialog.RestrictFavCheck
        tw.ReadOwnPost = SettingDialog.ReadOwnPost
        tw.UseSsl = SettingDialog.UseSsl
        ShortUrl.IsResolve = SettingDialog.TinyUrlResolve
        ShortUrl.BitlyId = SettingDialog.BitlyUser
        ShortUrl.BitlyKey = SettingDialog.BitlyPwd
        HttpTwitter.TwitterUrl = _cfgCommon.TwitterUrl
        HttpTwitter.TwitterSearchUrl = _cfgCommon.TwitterSearchUrl
        tw.TrackWord = _cfgCommon.TrackWord
        TrackToolStripMenuItem.Checked = Not String.IsNullOrEmpty(tw.TrackWord)
        tw.AllAtReply = _cfgCommon.AllAtReply
        AllrepliesToolStripMenuItem.Checked = tw.AllAtReply

        Outputz.Key = SettingDialog.OutputzKey
        Outputz.Enabled = SettingDialog.OutputzEnabled
        Select Case SettingDialog.OutputzUrlmode
            Case OutputzUrlmode.twittercom
                Outputz.OutUrl = "http://twitter.com/"
            Case OutputzUrlmode.twittercomWithUsername
                Outputz.OutUrl = "http://twitter.com/" + tw.Username
        End Select

        '画像投稿サービス
        SetImageServiceCombo()
        ImageSelectionPanel.Enabled = False

        ImageServiceCombo.SelectedIndex = _cfgCommon.UseImageSurvice

        'ウィンドウ設定
        Me.ClientSize = _cfgLocal.FormSize
        _mySize = _cfgLocal.FormSize                     'サイズ保持（最小化・最大化されたまま終了した場合の対応用）
        _myLoc = _cfgLocal.FormLocation
        'タイトルバー領域
        If Me.WindowState <> FormWindowState.Minimized Then
            Me.DesktopLocation = _cfgLocal.FormLocation
            Dim tbarRect As New Rectangle(Me.Location, New Size(_mySize.Width, SystemInformation.CaptionHeight))
            Dim outOfScreen As Boolean = True
            If Screen.AllScreens.Length = 1 Then    'ハングするとの報告
                For Each scr As Screen In Screen.AllScreens
                    If Not Rectangle.Intersect(tbarRect, scr.Bounds).IsEmpty Then
                        outOfScreen = False
                        Exit For
                    End If
                Next
                If outOfScreen Then
                    Me.DesktopLocation = New Point(0, 0)
                    _myLoc = Me.DesktopLocation
                End If
            End If
        End If
        Me.TopMost = SettingDialog.AlwaysTop
        _mySpDis = _cfgLocal.SplitterDistance
        _mySpDis2 = _cfgLocal.StatusTextHeight
        _mySpDis3 = _cfgLocal.PreviewDistance
        If _mySpDis3 = -1 Then
            _mySpDis3 = _mySize.Width - 150
            If _mySpDis3 < 1 Then _mySpDis3 = 50
            _cfgLocal.PreviewDistance = _mySpDis3
        End If
        MultiLineMenuItem.Checked = _cfgLocal.StatusMultiline
        'Me.Tween_ClientSizeChanged(Me, Nothing)
        PlaySoundMenuItem.Checked = SettingDialog.PlaySound
        Me.PlaySoundFileMenuItem.Checked = SettingDialog.PlaySound
        '入力欄
        StatusText.Font = _fntInputFont
        StatusText.ForeColor = _clInputFont

        '全新着通知のチェック状態により、Reply＆DMの新着通知有効無効切り替え（タブ別設定にするため削除予定）
        If SettingDialog.UnreadManage = False Then
            ReadedStripMenuItem.Enabled = False
            UnreadStripMenuItem.Enabled = False
        End If

        'タイマー設定
        TimerTimeline.AutoReset = True
        TimerTimeline.SynchronizingObject = Me
        'Recent取得間隔
        TimerTimeline.Interval = 1000
        TimerTimeline.Enabled = True

        '更新中アイコンアニメーション間隔
        TimerRefreshIcon.Interval = 200
        TimerRefreshIcon.Enabled = True

        '状態表示部の初期化（画面右下）
        StatusLabel.Text = ""
        StatusLabel.AutoToolTip = False
        StatusLabel.ToolTipText = ""
        '文字カウンタ初期化
        lblLen.Text = GetRestStatusCount(True, False).ToString()

        ''''''''''''''''''''''''''''''''''''''''
        _statuses.SortOrder = DirectCast(_cfgCommon.SortOrder, System.Windows.Forms.SortOrder)
        Dim mode As IdComparerClass.ComparerMode
        Select Case _cfgCommon.SortColumn
            Case 0, 5, 6    '0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                'ソートしない
                mode = IdComparerClass.ComparerMode.Id  'Idソートに読み替え
            Case 1  'ニックネーム
                mode = IdComparerClass.ComparerMode.Nickname
            Case 2  '本文
                mode = IdComparerClass.ComparerMode.Data
            Case 3  '時刻=発言Id
                mode = IdComparerClass.ComparerMode.Id
            Case 4  '名前
                mode = IdComparerClass.ComparerMode.Name
            Case 7  'Source
                mode = IdComparerClass.ComparerMode.Source
        End Select
        _statuses.SortMode = mode
        ''''''''''''''''''''''''''''''''''''''''

        Select Case SettingDialog.IconSz
            Case IconSizes.IconNone
                _iconSz = 0
            Case IconSizes.Icon16
                _iconSz = 16
            Case IconSizes.Icon24
                _iconSz = 26
            Case IconSizes.Icon48
                _iconSz = 48
            Case IconSizes.Icon48_2
                _iconSz = 48
                _iconCol = True
        End Select
        If _iconSz = 0 Then
            tw.GetIcon = False
        Else
            tw.GetIcon = True
            tw.IconSize = _iconSz
        End If
        tw.TinyUrlResolve = SettingDialog.TinyUrlResolve

        '発言詳細部アイコンをリストアイコンにサイズ変更
        Dim sz As Integer = _iconSz
        If _iconSz = 0 Then
            sz = 16
        End If

        tw.DetailIcon = TIconDic

        StatusLabel.Text = My.Resources.Form1_LoadText1       '画面右下の状態表示を変更
        StatusLabelUrl.Text = ""            '画面左下のリンク先URL表示部を初期化
        NameLabel.Text = ""                 '発言詳細部名前ラベル初期化
        DateTimeLabel.Text = ""             '発言詳細部日時ラベル初期化
        SourceLinkLabel.Text = ""           'Source部分初期化

        '<<<<<<<<タブ関連>>>>>>>
        'デフォルトタブの存在チェック、ない場合には追加
        If _statuses.GetTabByType(TabUsageType.Home) Is Nothing Then
            If Not _statuses.Tabs.ContainsKey(DEFAULTTAB.RECENT) Then
                _statuses.AddTab(DEFAULTTAB.RECENT, TabUsageType.Home, Nothing)
            Else
                _statuses.Tabs(DEFAULTTAB.RECENT).TabType = TabUsageType.Home
            End If
        End If
        If _statuses.GetTabByType(TabUsageType.Mentions) Is Nothing Then
            If Not _statuses.Tabs.ContainsKey(DEFAULTTAB.REPLY) Then
                _statuses.AddTab(DEFAULTTAB.REPLY, TabUsageType.Mentions, Nothing)
            Else
                _statuses.Tabs(DEFAULTTAB.REPLY).TabType = TabUsageType.Mentions
            End If
        End If
        If _statuses.GetTabByType(TabUsageType.DirectMessage) Is Nothing Then
            If Not _statuses.Tabs.ContainsKey(DEFAULTTAB.DM) Then
                _statuses.AddTab(DEFAULTTAB.DM, TabUsageType.DirectMessage, Nothing)
            Else
                _statuses.Tabs(DEFAULTTAB.DM).TabType = TabUsageType.DirectMessage
            End If
        End If
        If _statuses.GetTabByType(TabUsageType.Favorites) Is Nothing Then
            If Not _statuses.Tabs.ContainsKey(DEFAULTTAB.FAV) Then
                _statuses.AddTab(DEFAULTTAB.FAV, TabUsageType.Favorites, Nothing)
            Else
                _statuses.Tabs(DEFAULTTAB.FAV).TabType = TabUsageType.Favorites
            End If
        End If
        For Each tn As String In _statuses.Tabs.Keys
            If _statuses.Tabs(tn).TabType = TabUsageType.Undefined Then
                _statuses.Tabs(tn).TabType = TabUsageType.UserDefined
            End If
            If Not AddNewTab(tn, True, _statuses.Tabs(tn).TabType, _statuses.Tabs(tn).ListInfo) Then Throw New Exception(My.Resources.TweenMain_LoadText1)
        Next

        Me.JumpReadOpMenuItem.ShortcutKeyDisplayString = "Space"
        CopySTOTMenuItem.ShortcutKeyDisplayString = "Ctrl+C"
        CopyURLMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+C"
        CopyUserIdStripMenuItem.ShortcutKeyDisplayString = "Shift+Alt+C"

        If SettingDialog.MinimizeToTray = False OrElse Me.WindowState <> FormWindowState.Minimized Then
            Me.Visible = True
        End If
        _curTab = ListTab.SelectedTab
        _curItemIndex = -1
        _curList = DirectCast(_curTab.Tag, DetailsListView)
        SetMainWindowTitle()
        SetNotifyIconText()

        If SettingDialog.TabIconDisp Then
            ListTab.DrawMode = TabDrawMode.Normal
        Else
            ListTab.DrawMode = TabDrawMode.OwnerDrawFixed
            AddHandler ListTab.DrawItem, AddressOf ListTab_DrawItem
            ListTab.ImageList = Nothing
        End If

        _ignoreConfigSave = False
        Me.TweenMain_Resize(Nothing, Nothing)
        If saveRequired Then SaveConfigsAll(False)
    End Sub

    Private Sub spaceKeyCanceler_SpaceCancel(ByVal sender As Object, ByVal e As EventArgs)
        JumpUnreadMenuItem_Click(Nothing, Nothing)
    End Sub

    Private Sub ListTab_DrawItem( _
            ByVal sender As Object, ByVal e As DrawItemEventArgs)
        Dim txt As String
        Try
            txt = ListTab.TabPages(e.Index).Text
        Catch ex As Exception
            Exit Sub
        End Try

        e.Graphics.FillRectangle(System.Drawing.SystemBrushes.Control, e.Bounds)
        If e.State = DrawItemState.Selected Then
            e.DrawFocusRectangle()
        End If
        Dim fore As Brush
        Try
            If _statuses.Tabs(txt).UnreadCount > 0 Then
                fore = Brushes.Red
            Else
                fore = System.Drawing.SystemBrushes.ControlText
            End If
        Catch ex As Exception
            fore = System.Drawing.SystemBrushes.ControlText
        End Try
        e.Graphics.DrawString(txt, e.Font, fore, e.Bounds, sfTab)
    End Sub

    Private Sub LoadConfig()
        Dim needToSave As Boolean = False
        _cfgCommon = SettingCommon.Load()
        _cfgLocal = SettingLocal.Load()
        Dim tabs As List(Of TabClass) = SettingTabs.Load().Tabs
        For Each tb As TabClass In tabs
            _statuses.Tabs.Add(tb.TabName, tb)
        Next
        If _statuses.Tabs.Count = 0 Then
            _statuses.AddTab(DEFAULTTAB.RECENT, TabUsageType.Home, Nothing)
            _statuses.AddTab(DEFAULTTAB.REPLY, TabUsageType.Mentions, Nothing)
            _statuses.AddTab(DEFAULTTAB.DM, TabUsageType.DirectMessage, Nothing)
            _statuses.AddTab(DEFAULTTAB.FAV, TabUsageType.Favorites, Nothing)
        End If
    End Sub

    'Private Sub ConvertConfig()
    '    '新タブ設定ファイル存在チェック
    '    If System.IO.File.Exists(SettingTabs.GetSettingFilePath("")) Then
    '        LoadConfig()
    '        Exit Sub
    '    End If
    '    'LoadOldConfig()
    'End Sub

    Private Sub TimerTimeline_Elapsed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerTimeline.Elapsed
        Static homeCounter As Integer = 0
        Static mentionCounter As Integer = 0
        Static dmCounter As Integer = 0
        Static pubSearchCounter As Integer = 0
        Static userTimelineCounter As Integer = 0
        Static listsCounter As Integer = 0
        Static usCounter As Integer = 0
        Static lastGetTime As DateTime = Now

        If homeCounter > 0 Then Interlocked.Decrement(homeCounter)
        If mentionCounter > 0 Then Interlocked.Decrement(mentionCounter)
        If dmCounter > 0 Then Interlocked.Decrement(dmCounter)
        If pubSearchCounter > 0 Then Interlocked.Decrement(pubSearchCounter)
        If userTimelineCounter > 0 Then Interlocked.Decrement(userTimelineCounter)
        If listsCounter > 0 Then Interlocked.Decrement(listsCounter)
        If usCounter > 0 Then Interlocked.Decrement(usCounter)

        ''タイマー初期化
        If homeCounter <= 0 AndAlso SettingDialog.TimelinePeriodInt > 0 Then
            Interlocked.Exchange(homeCounter, SettingDialog.TimelinePeriodInt)
            If Not tw.IsUserstreamDataReceived Then
                lastGetTime = Now
                GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
            End If
        End If
        If mentionCounter <= 0 AndAlso SettingDialog.ReplyPeriodInt > 0 Then
            Interlocked.Exchange(mentionCounter, SettingDialog.ReplyPeriodInt)
            If Not tw.IsUserstreamDataReceived Then
                lastGetTime = Now
                GetTimeline(WORKERTYPE.Reply, 1, 0, "")
            End If
        End If
        If dmCounter <= 0 AndAlso SettingDialog.DMPeriodInt > 0 Then
            Interlocked.Exchange(dmCounter, SettingDialog.DMPeriodInt)
            If Not tw.IsUserstreamDataReceived Then
                lastGetTime = Now
                GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, 0, "")
            End If
        End If
        If pubSearchCounter <= 0 AndAlso SettingDialog.PubSearchPeriodInt > 0 Then
            Interlocked.Exchange(pubSearchCounter, SettingDialog.PubSearchPeriodInt)
            GetTimeline(WORKERTYPE.PublicSearch, 1, 0, "")
        End If
        If userTimelineCounter <= 0 AndAlso SettingDialog.UserTimelinePeriodInt > 0 Then
            Interlocked.Exchange(userTimelineCounter, SettingDialog.UserTimelinePeriodInt)
            GetTimeline(WORKERTYPE.UserTimeline, 1, 0, "")
        End If
        If listsCounter <= 0 AndAlso SettingDialog.ListsPeriodInt > 0 Then
            Interlocked.Exchange(listsCounter, SettingDialog.ListsPeriodInt)
            GetTimeline(WORKERTYPE.List, 1, 0, "")
        End If
        If usCounter <= 0 AndAlso SettingDialog.UserstreamPeriodInt > 0 Then
            Interlocked.Exchange(usCounter, SettingDialog.UserstreamPeriodInt)
            If Me._isActiveUserstream Then RefreshTimeline(True)
        End If
        If Not tw.IsUserstreamDataReceived AndAlso Now.Subtract(lastGetTime).TotalMinutes > 10 Then
            lastGetTime = Now
            GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
            GetTimeline(WORKERTYPE.Reply, 1, 0, "")
            GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, 0, "")
        End If
    End Sub

    Private Sub RefreshTimeline(ByVal isUserStream As Boolean)
        If isUserStream Then Me.RefreshTasktrayIcon(True)
        'スクロール制御準備
        Dim smode As Integer = -1    '-1:制御しない,-2:最新へ,その他:topitem使用
        Dim topId As Long = GetScrollPos(smode)
        Dim befCnt As Integer = _curList.VirtualListSize

        '現在の選択状態を退避
        Dim selId As New Dictionary(Of String, Long())
        Dim focusedId As New Dictionary(Of String, Long)
        SaveSelectedStatus(selId, focusedId)

        'mentionsの更新前件数を保持
        Dim dmCount As Integer = _statuses.GetTabByType(TabUsageType.DirectMessage).AllCount

        '更新確定
        Dim notifyPosts() As PostClass = Nothing
        Dim soundFile As String = ""
        Dim addCount As Integer = 0
        Dim isMention As Boolean = False
        addCount = _statuses.SubmitUpdate(soundFile, notifyPosts, isMention, isUserStream)

        If _endingFlag Then Exit Sub

        'リストに反映＆選択状態復元
        Try
            For Each tab As TabPage In ListTab.TabPages
                Dim lst As DetailsListView = DirectCast(tab.Tag, DetailsListView)
                Dim tabInfo As TabClass = _statuses.Tabs(tab.Text)
                lst.BeginUpdate()
                If lst.VirtualListSize <> tabInfo.AllCount Then
                    If lst.Equals(_curList) Then
                        _itemCache = Nothing
                        _postCache = Nothing
                    End If
                    Try
                        lst.VirtualListSize = tabInfo.AllCount 'リスト件数更新
                    Catch ex As Exception
                        'アイコン描画不具合あり？
                    End Try
                    Me.SelectListItem(lst, _
                                      _statuses.IndexOf(tab.Text, selId(tab.Text)), _
                                      _statuses.IndexOf(tab.Text, focusedId(tab.Text)))
                End If
                lst.EndUpdate()
                If tabInfo.UnreadCount > 0 Then
                    If SettingDialog.TabIconDisp Then
                        If tab.ImageIndex = -1 Then tab.ImageIndex = 0 'タブアイコン
                    End If
                End If
            Next
            If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
        Catch ex As Exception
            'ex.Data("Msg") = "Ref1, UseAPI=" + SettingDialog.UseAPI.ToString
            'Throw
        End Try

        'スクロール制御後処理
        Try
            If befCnt <> _curList.VirtualListSize Then
                Select Case smode
                    Case -3
                        '最上行
                        _curList.EnsureVisible(0)
                    Case -2
                        '最下行へ
                        If _curList.VirtualListSize > 0 Then _curList.EnsureVisible(_curList.VirtualListSize - 1)
                    Case -1
                        '制御しない
                    Case Else
                        '表示位置キープ
                        If _curList.VirtualListSize > 0 AndAlso _statuses.IndexOf(_curTab.Text, topId) > -1 Then
                            _curList.EnsureVisible(_curList.VirtualListSize - 1)
                            _curList.EnsureVisible(_statuses.IndexOf(_curTab.Text, topId))
                        End If
                End Select
            End If
        Catch ex As Exception
            ex.Data("Msg") = "Ref2"
            Throw
        End Try

        '新着通知
        NotifyNewPosts(notifyPosts,
                       soundFile,
                       addCount,
                       isMention OrElse dmCount <> _statuses.GetTabByType(TabUsageType.DirectMessage).AllCount)

        SetMainWindowTitle()
        If Not StatusLabelUrl.Text.StartsWith("http") Then SetStatusLabelUrl()

        HashSupl.AddRangeItem(tw.GetHashList)

    End Sub

    Private Function GetScrollPos(ByRef smode As Integer) As Long
        Dim topId As Long = -1
        If _curList IsNot Nothing AndAlso _curTab IsNot Nothing AndAlso _curList.VirtualListSize > 0 Then
            If _statuses.SortMode = IdComparerClass.ComparerMode.Id Then
                If _statuses.SortOrder = SortOrder.Ascending Then
                    'Id昇順
                    If ListLockMenuItem.Checked Then
                        '制御しない
                        'smode = -1
                        '現在表示位置へ強制スクロール
                        If _curList.TopItem IsNot Nothing Then topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index)
                        smode = 0
                    Else
                        '最下行が表示されていたら、最下行へ強制スクロール。最下行が表示されていなかったら制御しない
                        Dim _item As ListViewItem
                        _item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1)   '一番下
                        If _item Is Nothing Then _item = _curList.Items(_curList.Items.Count - 1)
                        If _item.Index = _curList.Items.Count - 1 Then
                            smode = -2
                        Else
                            'smode = -1
                            If _curList.TopItem IsNot Nothing Then topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index)
                            smode = 0
                        End If
                    End If
                Else
                    'Id降順
                    If ListLockMenuItem.Checked Then
                        '現在表示位置へ強制スクロール
                        If _curList.TopItem IsNot Nothing Then topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index)
                        smode = 0
                    Else
                        '最上行が表示されていたら、制御しない。最上行が表示されていなかったら、現在表示位置へ強制スクロール
                        Dim _item As ListViewItem

                        _item = _curList.GetItemAt(0, 10)     '一番上
                        If _item Is Nothing Then _item = _curList.Items(0)
                        If _item.Index = 0 Then
                            smode = -3  '最上行
                        Else
                            If _curList.TopItem IsNot Nothing Then topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index)
                            smode = 0
                        End If
                    End If
                End If
            Else
                '現在表示位置へ強制スクロール
                If _curList.TopItem IsNot Nothing Then topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index)
                smode = 0
            End If
        Else
            smode = -1
        End If
        Return topId
    End Function

    Private Sub SaveSelectedStatus(ByVal selId As Dictionary(Of String, Long()), ByVal focusedId As Dictionary(Of String, Long))
        If _endingFlag Then Exit Sub
        For Each tab As TabPage In ListTab.TabPages
            Dim lst As DetailsListView = DirectCast(tab.Tag, DetailsListView)
            If lst.SelectedIndices.Count > 0 AndAlso lst.SelectedIndices.Count < 31 Then
                selId.Add(tab.Text, _statuses.GetId(tab.Text, lst.SelectedIndices))
            Else
                selId.Add(tab.Text, New Long(0) {-1})
            End If
            If lst.FocusedItem IsNot Nothing Then
                focusedId.Add(tab.Text, _statuses.GetId(tab.Text, lst.FocusedItem.Index))
            Else
                focusedId.Add(tab.Text, -1)
            End If
        Next

    End Sub

    Private Overloads Function BalloonRequired() As Boolean
        Return BalloonRequired(New Twitter.FormattedEvent With {.Eventtype = EVENTTYPE.None})
    End Function

    Private Function IsEventNotifyAsEventType(ByVal type As EVENTTYPE) As Boolean
        Return SettingDialog.EventNotifyEnabled AndAlso CBool(type And SettingDialog.EventNotifyFlag) OrElse type = EVENTTYPE.None
    End Function

    Private Function IsMyEventNotityAsEventType(ByVal ev As Twitter.FormattedEvent) As Boolean
        Return If(CBool(ev.Eventtype And SettingDialog.IsMyEventNotifyFlag), True, Not ev.IsMe)
    End Function

    Private Overloads Function BalloonRequired(ByVal ev As Twitter.FormattedEvent) As Boolean
        If (
            IsEventNotifyAsEventType(ev.Eventtype) AndAlso IsMyEventNotityAsEventType(ev) AndAlso
            (NewPostPopMenuItem.Checked OrElse (SettingDialog.ForceEventNotify AndAlso ev.Eventtype <> EVENTTYPE.None)) AndAlso
            Not _initial AndAlso
            (
                (
                    SettingDialog.LimitBalloon AndAlso
                    (
                        Me.WindowState = FormWindowState.Minimized OrElse
                        Not Me.Visible OrElse
                        Form.ActiveForm Is Nothing
                        )
                    ) OrElse
                Not SettingDialog.LimitBalloon
                )
            ) AndAlso
        Not IsScreenSaverRunning() Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub NotifyNewPosts(ByVal notifyPosts() As PostClass, ByVal soundFile As String, ByVal addCount As Integer, ByVal newMentions As Boolean)
        If notifyPosts IsNot Nothing AndAlso _
            notifyPosts.Count > 0 AndAlso _
            Me.SettingDialog.ReadOwnPost AndAlso _
            notifyPosts.All(Function(post) post.UserId.ToString() = tw.UserIdNo OrElse post.ScreenName = tw.Username) Then
            Exit Sub
        End If

        '新着通知
        If BalloonRequired() Then
            If notifyPosts IsNot Nothing AndAlso notifyPosts.Length > 0 Then
                Dim sb As New StringBuilder
                Dim reply As Boolean = False
                Dim dm As Boolean = False
                For Each post As PostClass In notifyPosts
                    If post.IsReply AndAlso Not post.IsExcludeReply Then reply = True
                    If post.IsDm Then dm = True
                    If sb.Length > 0 Then sb.Append(System.Environment.NewLine)
                    Select Case SettingDialog.NameBalloon
                        Case NameBalloonEnum.UserID
                            sb.Append(post.ScreenName).Append(" : ")
                        Case NameBalloonEnum.NickName
                            sb.Append(post.Nickname).Append(" : ")
                    End Select
                    sb.Append(post.TextFromApi)
                Next
                If SettingDialog.DispUsername Then NotifyIcon1.BalloonTipTitle = tw.Username + " - " Else NotifyIcon1.BalloonTipTitle = ""
                If dm Then
                    NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning
                    NotifyIcon1.BalloonTipTitle += "Tween [DM] " + My.Resources.RefreshDirectMessageText1 + " " + addCount.ToString() + My.Resources.RefreshDirectMessageText2
                ElseIf reply Then
                    NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning
                    NotifyIcon1.BalloonTipTitle += "Tween [Reply!] " + My.Resources.RefreshTimelineText1 + " " + addCount.ToString() + My.Resources.RefreshTimelineText2
                Else
                    NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info
                    NotifyIcon1.BalloonTipTitle += "Tween " + My.Resources.RefreshTimelineText1 + " " + addCount.ToString() + My.Resources.RefreshTimelineText2
                End If
                Dim bText As String = sb.ToString
                If String.IsNullOrEmpty(bText) Then Exit Sub
                NotifyIcon1.BalloonTipText = sb.ToString()
                NotifyIcon1.ShowBalloonTip(500)
            End If
        End If

        'サウンド再生
        If Not _initial AndAlso SettingDialog.PlaySound AndAlso soundFile <> "" Then
            Try
                Dim dir As String = My.Application.Info.DirectoryPath
                If Directory.Exists(Path.Combine(dir, "Sounds")) Then
                    dir = Path.Combine(dir, "Sounds")
                End If
                My.Computer.Audio.Play(Path.Combine(dir, soundFile), AudioPlayMode.Background)
            Catch ex As Exception

            End Try
        End If

        'mentions新着時に画面ブリンク
        If Not _initial AndAlso SettingDialog.BlinkNewMentions AndAlso newMentions AndAlso Form.ActiveForm Is Nothing Then
            FlashMyWindow(Me.Handle, FlashSpecification.FlashTray, 3)
        End If
    End Sub

    Private Sub MyList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If _curList.SelectedIndices.Count <> 1 Then Exit Sub

        _curItemIndex = _curList.SelectedIndices(0)

        _curPost = GetCurTabPost(_curItemIndex)

        If SettingDialog.UnreadManage Then _statuses.SetReadAllTab(True, _curTab.Text, _curItemIndex)
        'キャッシュの書き換え
        ChangeCacheStyleRead(True, _curItemIndex, _curTab)   '既読へ（フォント、文字色）

        ColorizeList()
        _colorize = True
    End Sub

    Private Sub ChangeCacheStyleRead(ByVal Read As Boolean, ByVal Index As Integer, ByVal Tab As TabPage)
        'Read:True=既読 False=未読
        '未読管理していなかったら既読として扱う
        If Not _statuses.Tabs(_curTab.Text).UnreadManage OrElse _
           Not SettingDialog.UnreadManage Then Read = True

        '対象の特定
        Dim itm As ListViewItem
        Dim post As PostClass
        If Tab.Equals(_curTab) AndAlso _itemCache IsNot Nothing AndAlso Index >= _itemCacheIndex AndAlso Index < _itemCacheIndex + _itemCache.Length Then
            itm = _itemCache(Index - _itemCacheIndex)
            post = _postCache(Index - _itemCacheIndex)
        Else
            itm = DirectCast(Tab.Tag, DetailsListView).Items(Index)
            post = _statuses.Item(Tab.Text, Index)
        End If

        ChangeItemStyleRead(Read, itm, post, DirectCast(Tab.Tag, DetailsListView))
    End Sub

    Private Sub ChangeItemStyleRead(ByVal Read As Boolean, ByVal Item As ListViewItem, ByVal Post As PostClass, ByVal DList As DetailsListView)
        Dim fnt As Font
        'フォント
        If Read Then
            fnt = _fntReaded
            Item.SubItems(5).Text = ""
        Else
            fnt = _fntUnread
            Item.SubItems(5).Text = "★"
        End If
        '文字色
        Dim cl As Color
        If Post.IsFav Then
            cl = _clFav
        ElseIf Post.RetweetedId > 0 Then
            cl = _clRetweet
        ElseIf Post.IsOwl AndAlso (Post.IsDm OrElse SettingDialog.OneWayLove) Then
            cl = _clOWL
        ElseIf Read OrElse Not SettingDialog.UseUnreadStyle Then
            cl = _clReaded
        Else
            cl = _clUnread
        End If
        If DList Is Nothing OrElse Item.Index = -1 Then
            Item.ForeColor = cl
            If SettingDialog.UseUnreadStyle Then
                Item.Font = fnt
            End If
        Else
            DList.Update()
            If SettingDialog.UseUnreadStyle Then
                DList.ChangeItemFontAndColor(Item.Index, cl, fnt)
            Else
                DList.ChangeItemForeColor(Item.Index, cl)
            End If
            'If _itemCache IsNot Nothing Then DList.RedrawItems(_itemCacheIndex, _itemCacheIndex + _itemCache.Length - 1, False)
        End If
    End Sub

    Private Sub ColorizeList()
        'Index:更新対象のListviewItem.Index。Colorを返す。
        '-1は全キャッシュ。Colorは返さない（ダミーを戻す）
        Dim _post As PostClass
        If _anchorFlag Then
            _post = _anchorPost
        Else
            _post = _curPost
        End If

        If _itemCache Is Nothing Then Exit Sub

        If _post Is Nothing Then Exit Sub

        Try
            For cnt As Integer = 0 To _itemCache.Length - 1
                _curList.ChangeItemBackColor(_itemCacheIndex + cnt, JudgeColor(_post, _postCache(cnt)))
            Next
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ColorizeList(ByVal Item As ListViewItem, ByVal Index As Integer)
        'Index:更新対象のListviewItem.Index。Colorを返す。
        '-1は全キャッシュ。Colorは返さない（ダミーを戻す）
        Dim _post As PostClass
        If _anchorFlag Then
            _post = _anchorPost
        Else
            _post = _curPost
        End If

        Dim tPost As PostClass = GetCurTabPost(Index)

        If _post Is Nothing Then Exit Sub

        If Item.Index = -1 Then
            Item.BackColor = JudgeColor(_post, tPost)
        Else
            _curList.ChangeItemBackColor(Item.Index, JudgeColor(_post, tPost))
        End If
    End Sub

    Private Function JudgeColor(ByVal BasePost As PostClass, ByVal TargetPost As PostClass) As Color
        Dim cl As Color
        If TargetPost.StatusId = BasePost.InReplyToStatusId Then
            '@先
            cl = _clAtTo
        ElseIf TargetPost.IsMe Then
            '自分=発言者
            cl = _clSelf
        ElseIf TargetPost.IsReply Then
            '自分宛返信
            cl = _clAtSelf
        ElseIf BasePost.ReplyToList.Contains(TargetPost.ScreenName.ToLower()) Then
            '返信先
            cl = _clAtFromTarget
        ElseIf TargetPost.ReplyToList.Contains(BasePost.ScreenName.ToLower()) Then
            'その人への返信
            cl = _clAtTarget
        ElseIf TargetPost.ScreenName.Equals(BasePost.ScreenName, StringComparison.OrdinalIgnoreCase) Then
            '発言者
            cl = _clTarget
        Else
            'その他
            cl = _clListBackcolor
        End If
        Return cl
    End Function

    Private Sub PostButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PostButton.Click
        If StatusText.Text.Trim.Length = 0 Then
            If Not ImageSelectionPanel.Enabled Then
                DoRefresh()
                Exit Sub
            End If
        End If

        If Me.ExistCurrentPost AndAlso StatusText.Text.Trim() = String.Format("RT @{0}: {1}", _curPost.ScreenName, _curPost.TextFromApi) Then
            Dim rtResult As DialogResult = MessageBox.Show(String.Format(My.Resources.PostButton_Click1, Environment.NewLine),
                                                           "Retweet",
                                                           MessageBoxButtons.YesNoCancel,
                                                           MessageBoxIcon.Question)
            Select Case rtResult
                Case Windows.Forms.DialogResult.Yes
                    doReTweetOfficial(False)
                    StatusText.Text = ""
                    Exit Sub
                Case Windows.Forms.DialogResult.Cancel
                    Exit Sub
            End Select
        End If

        _history(_history.Count - 1) = New PostingStatus(StatusText.Text.Trim, _reply_to_id, _reply_to_name)

        If SettingDialog.UrlConvertAuto Then
            StatusText.SelectionStart = StatusText.Text.Length
            UrlConvertAutoToolStripMenuItem_Click(Nothing, Nothing)
        ElseIf SettingDialog.Nicoms Then
            StatusText.SelectionStart = StatusText.Text.Length
            UrlConvert(UrlConverter.Nicoms)
        End If
        StatusText.SelectionStart = StatusText.Text.Length
        Dim args As New GetWorkerArg()
        args.page = 0
        args.endPage = 0
        args.type = WORKERTYPE.PostMessage
        CheckReplyTo(StatusText.Text)

        '整形によって増加する文字数を取得
        Dim adjustCount As Integer = 0
        Dim tmpStatus As String = StatusText.Text.Trim
        If ToolStripMenuItemApiCommandEvasion.Checked Then
            ' APIコマンド回避
            If Regex.IsMatch(tmpStatus, _
                "^[+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\*)([+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]+|$)", _
                RegexOptions.IgnoreCase) _
               AndAlso tmpStatus.EndsWith(" .") = False Then adjustCount += 2
        End If

        If ToolStripMenuItemUrlMultibyteSplit.Checked Then
            ' URLと全角文字の切り離し
            adjustCount += Regex.Matches(tmpStatus, "https?:\/\/[-_.!~*'()a-zA-Z0-9;\/?:\@&=+\$,%#^]+").Count
        End If

        Dim isCutOff As Boolean = False
        Dim isRemoveFooter As Boolean = My.Computer.Keyboard.ShiftKeyDown
        If StatusText.Multiline AndAlso Not SettingDialog.PostCtrlEnter Then
            '複数行でEnter投稿の場合、Ctrlも押されていたらフッタ付加しない
            isRemoveFooter = My.Computer.Keyboard.CtrlKeyDown
        End If
        If SettingDialog.PostShiftEnter Then
            isRemoveFooter = My.Computer.Keyboard.CtrlKeyDown
        End If
        If Not isRemoveFooter AndAlso (StatusText.Text.Contains("RT @") OrElse StatusText.Text.Contains("QT @")) Then
            isRemoveFooter = True
        End If
        If GetRestStatusCount(False, Not isRemoveFooter) - adjustCount < 0 Then
            If MessageBox.Show(My.Resources.PostLengthOverMessage1, My.Resources.PostLengthOverMessage2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.OK Then
                isCutOff = True
                If Not SettingDialog.UrlConvertAuto Then UrlConvertAutoToolStripMenuItem_Click(Nothing, Nothing)
                If GetRestStatusCount(False, Not isRemoveFooter) - adjustCount < 0 Then
                    isRemoveFooter = True
                End If
            Else
                Exit Sub
            End If
        End If

        Dim footer As String = ""
        Dim header As String = ""
        If StatusText.Text.StartsWith("D ") OrElse StatusText.Text.StartsWith("d ") Then
            'DM時は何もつけない
            footer = ""
        Else
            'ハッシュタグ
            If HashMgr.UseHash <> "" Then
                If HashMgr.IsHead Then
                    header = HashMgr.UseHash + " "
                Else
                    footer = " " + HashMgr.UseHash
                End If
            End If
            If Not isRemoveFooter Then
                If SettingDialog.UseRecommendStatus Then
                    ' 推奨ステータスを使用する
                    footer += SettingDialog.RecommendStatusText
                Else
                    ' テキストボックスに入力されている文字列を使用する
                    footer += " " + SettingDialog.Status.Trim
                End If
            End If
        End If
        args.status.status = header + StatusText.Text.Trim + footer

        If ToolStripMenuItemApiCommandEvasion.Checked Then
            ' APIコマンド回避
            If Regex.IsMatch(args.status.status, _
                "^[+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\*)([+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]+|$)", _
                RegexOptions.IgnoreCase) _
               AndAlso args.status.status.EndsWith(" .") = False Then args.status.status += " ."
        End If

        If ToolStripMenuItemUrlMultibyteSplit.Checked Then
            ' URLと全角文字の切り離し
            Dim mc2 As Match = Regex.Match(args.status.status, "https?:\/\/[-_.!~*'()a-zA-Z0-9;\/?:\@&=+\$,%#^]+")
            If mc2.Success Then args.status.status = Regex.Replace(args.status.status, "https?:\/\/[-_.!~*'()a-zA-Z0-9;\/?:\@&=+\$,%#^]+", "$& ")
        End If

        If IdeographicSpaceToSpaceToolStripMenuItem.Checked Then
            ' 文中の全角スペースを半角スペース1個にする
            args.status.status = args.status.status.Replace("　", " ")
        End If

        If isCutOff AndAlso args.status.status.Length > 140 Then
            args.status.status = args.status.status.Substring(0, 140)
            Dim AtId As String = "(@|＠)[a-z0-9_/]+$"
            Dim HashTag As String = "(^|[^0-9A-Z&\/\?]+)(#|＃)([0-9A-Z_]*[A-Z_]+)$"
            Dim Url As String = "https?:\/\/[a-z0-9!\*'\(\);:&=\+\$\/%#\[\]\-_\.,~?]+$" '簡易判定
            Dim pattern As String = String.Format("({0})|({1})|({2})", AtId, HashTag, Url)
            Dim mc As Match = Regex.Match(args.status.status, pattern, RegexOptions.IgnoreCase)
            If mc.Success Then
                'さらに@ID、ハッシュタグ、URLと推測される文字列をカットする
                args.status.status = args.status.status.Substring(0, 140 - mc.Value.Length)
            End If
            If MessageBox.Show(args.status.status, "Post or Cancel?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then Exit Sub
        End If

        args.status.inReplyToId = _reply_to_id
        args.status.inReplyToName = _reply_to_name
        If ImageSelectionPanel.Visible Then
            '画像投稿
            If ImageSelectedPicture.Image IsNot ImageSelectedPicture.InitialImage AndAlso _
                ImageServiceCombo.SelectedIndex > -1 AndAlso _
                ImagefilePathText.Text <> "" Then
                If MessageBox.Show(My.Resources.PostPictureConfirm1, _
                                   My.Resources.PostPictureConfirm2, _
                                   MessageBoxButtons.OKCancel, _
                                   MessageBoxIcon.Question, _
                                   MessageBoxDefaultButton.Button1) _
                               = Windows.Forms.DialogResult.Cancel Then
                    TimelinePanel.Visible = True
                    TimelinePanel.Enabled = True
                    ImageSelectionPanel.Visible = False
                    ImageSelectionPanel.Enabled = False
                    If _curList IsNot Nothing Then
                        _curList.Focus()
                    End If
                    Exit Sub
                End If
                args.status.imageService = ImageServiceCombo.Text
                args.status.imagePath = ImagefilePathText.Text
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage
                ImagefilePathText.Text = ""
                TimelinePanel.Visible = True
                TimelinePanel.Enabled = True
                ImageSelectionPanel.Visible = False
                ImageSelectionPanel.Enabled = False
                If _curList IsNot Nothing Then
                    _curList.Focus()
                End If
            Else
                MessageBox.Show(My.Resources.PostPictureWarn1, My.Resources.PostPictureWarn2)
                Exit Sub
            End If
        End If

        RunAsync(args)

        'Google検索（試験実装）
        If StatusText.Text.StartsWith("Google:", StringComparison.OrdinalIgnoreCase) AndAlso StatusText.Text.Trim.Length > 7 Then
            Dim tmp As String = String.Format(My.Resources.SearchItem2Url, HttpUtility.UrlEncode(StatusText.Text.Substring(7)))
            OpenUriAsync(tmp)
        End If

        _reply_to_id = 0
        _reply_to_name = ""
        StatusText.Text = ""
        _history.Add(New PostingStatus)
        _hisIdx = _history.Count - 1
        If Not ToolStripFocusLockMenuItem.Checked Then
            DirectCast(ListTab.SelectedTab.Tag, Control).Focus()
        End If
        urlUndoBuffer = Nothing
        UrlUndoToolStripMenuItem.Enabled = False  'Undoをできないように設定
    End Sub

    Private Sub EndToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EndToolStripMenuItem.Click, EndFileMenuItem.Click
        _endingFlag = True
        Me.Close()
    End Sub

    Private Sub Tween_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If Not SettingDialog.CloseToExit AndAlso e.CloseReason = CloseReason.UserClosing AndAlso _endingFlag = False Then
            '_endingFlag=False:フォームの×ボタン
            e.Cancel = True
            Me.Visible = False
        Else
            _hookGlobalHotkey.UnregisterAllOriginalHotkey()
            _ignoreConfigSave = True
            _endingFlag = True
            TimerTimeline.Enabled = False
            TimerRefreshIcon.Enabled = False
        End If
    End Sub

    Private Sub NotifyIcon1_BalloonTipClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NotifyIcon1.BalloonTipClicked
        Me.Visible = True
        If Me.WindowState = FormWindowState.Minimized Then
            Me.WindowState = FormWindowState.Normal
        End If
        Me.Activate()
    End Sub

    Private Shared Function CheckAccountValid() As Boolean
        Static errorCount As Integer = 0
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then
            errorCount += 1
            If errorCount > 5 Then
                errorCount = 0
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return True
            End If
            Return False
        End If
        errorCount = 0
        Return True
    End Function

    Private Sub GetTimelineWorker_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
        Dim bw As BackgroundWorker = DirectCast(sender, BackgroundWorker)
        If bw.CancellationPending OrElse _endingFlag Then
            e.Cancel = True
            Exit Sub
        End If

        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.BelowNormal

        My.Application.InitCulture()

        Dim ret As String = ""
        Dim rslt As New GetWorkerResult()

        Dim read As Boolean = Not SettingDialog.UnreadManage
        If _initial AndAlso SettingDialog.UnreadManage Then read = SettingDialog.Readed

        Dim args As GetWorkerArg = DirectCast(e.Argument, GetWorkerArg)

        If Not CheckAccountValid() Then
            rslt.retMsg = "Auth error. Check your account"
            rslt.type = WORKERTYPE.ErrorState  'エラー表示のみ行ない、後処理キャンセル
            rslt.tName = args.tName
            e.Result = rslt
            Exit Sub
        End If

        If args.type <> WORKERTYPE.OpenUri Then bw.ReportProgress(0, "") 'Notifyアイコンアニメーション開始
        Select Case args.type
            Case WORKERTYPE.Timeline, WORKERTYPE.Reply
                bw.ReportProgress(50, MakeStatusMessage(args, False))
                ret = tw.GetTimelineApi(read, args.type, args.page = -1, _initial)
                '新着時未読クリア
                If ret = "" AndAlso args.type = WORKERTYPE.Timeline AndAlso SettingDialog.ReadOldPosts Then
                    _statuses.SetRead()
                End If
                '振り分け
                rslt.addCount = _statuses.DistributePosts()
            Case WORKERTYPE.DirectMessegeRcv    '送信分もまとめて取得
                bw.ReportProgress(50, MakeStatusMessage(args, False))
                ret = tw.GetDirectMessageApi(read, WORKERTYPE.DirectMessegeRcv, args.page = -1)
                If ret = "" Then ret = tw.GetDirectMessageApi(read, WORKERTYPE.DirectMessegeSnt, args.page = -1)
                rslt.addCount = _statuses.DistributePosts()
            Case WORKERTYPE.FavAdd
                'スレッド処理はしない
                If _statuses.Tabs.ContainsKey(args.tName) Then
                    Dim tbc As TabClass = _statuses.Tabs(args.tName)
                    For i As Integer = 0 To args.ids.Count - 1
                        Dim post As PostClass = Nothing
                        If tbc.IsInnerStorageTabType Then
                            post = tbc.Posts(args.ids(i))
                        Else
                            post = _statuses.Item(args.ids(i))
                        End If
                        args.page = i + 1
                        bw.ReportProgress(50, MakeStatusMessage(args, False))
                        If Not post.IsFav Then
                            If post.RetweetedId = 0 Then
                                ret = tw.PostFavAdd(post.StatusId)
                            Else
                                ret = tw.PostFavAdd(post.RetweetedId)
                            End If
                            If ret.Length = 0 Then
                                args.sIds.Add(post.StatusId)
                                post.IsFav = True    'リスト再描画必要
                                _favTimestamps.Add(Now)
                                If post.RelTabName = "" Then
                                    '検索,リストタブからのfavは、favタブへ追加せず。それ以外は追加
                                    _statuses.GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, False)
                                Else
                                    '検索・リストタブからのfavで、TLでも取得済みならfav反映
                                    If _statuses.ContainsKey(post.StatusId) Then
                                        Dim postTl As PostClass = _statuses.Item(post.StatusId)
                                        postTl.IsFav = True
                                        _statuses.GetTabByType(TabUsageType.Favorites).Add(postTl.StatusId, postTl.IsRead, False)
                                    End If
                                End If
                                '検索、リストタブに反映
                                For Each tb As TabClass In _statuses.GetTabsByType(TabUsageType.PublicSearch Or TabUsageType.Lists)
                                    If tb.Contains(post.StatusId) Then tb.Posts(post.StatusId).IsFav = True
                                Next
                            End If
                        End If
                    Next
                End If
                rslt.sIds = args.sIds
            Case WORKERTYPE.FavRemove
                'スレッド処理はしない
                If _statuses.Tabs.ContainsKey(args.tName) Then
                    Dim tbc As TabClass = _statuses.Tabs(args.tName)
                    For i As Integer = 0 To args.ids.Count - 1
                        Dim post As PostClass = Nothing
                        If tbc.IsInnerStorageTabType Then
                            post = tbc.Posts(args.ids(i))
                        Else
                            post = _statuses.Item(args.ids(i))
                        End If
                        args.page = i + 1
                        bw.ReportProgress(50, MakeStatusMessage(args, False))
                        If post.IsFav Then
                            If post.RetweetedId = 0 Then
                                ret = tw.PostFavRemove(post.StatusId)
                            Else
                                ret = tw.PostFavRemove(post.RetweetedId)
                            End If
                            If ret.Length = 0 Then
                                args.sIds.Add(post.StatusId)
                                post.IsFav = False    'リスト再描画必要
                                If _statuses.ContainsKey(post.StatusId) Then _statuses.Item(post.StatusId).IsFav = False
                                '検索,リストタブに反映
                                For Each tb As TabClass In _statuses.GetTabsByType(TabUsageType.PublicSearch Or TabUsageType.Lists)
                                    If tb.Contains(post.StatusId) Then tb.Posts(post.StatusId).IsFav = False
                                Next
                            End If
                        End If
                    Next
                End If
                rslt.sIds = args.sIds
            Case WORKERTYPE.PostMessage
                bw.ReportProgress(200)
                If String.IsNullOrEmpty(args.status.imagePath) Then
                    For i As Integer = 0 To 1
                        ret = tw.PostStatus(args.status.status, args.status.inReplyToId)
                        If ret = "" OrElse _
                           ret.StartsWith("OK:") OrElse _
                           ret.StartsWith("Outputz:") OrElse _
                           ret.StartsWith("Warn:") OrElse _
                           ret = "Err:Status is a duplicate." OrElse _
                           args.status.status.StartsWith("D", StringComparison.OrdinalIgnoreCase) OrElse _
                           args.status.status.StartsWith("DM", StringComparison.OrdinalIgnoreCase) OrElse _
                           Twitter.AccountState <> ACCOUNT_STATE.Valid Then
                            Exit For
                        End If
                    Next
                Else
                    Dim picSvc As New PictureService(tw)
                    If String.IsNullOrEmpty(args.status.status) Then args.status.status = ""
                    ret = picSvc.Upload(args.status.imagePath, args.status.status, args.status.imageService)
                End If
                bw.ReportProgress(300)
                rslt.status = args.status
            Case WORKERTYPE.Retweet
                bw.ReportProgress(200)
                For i As Integer = 0 To args.ids.Count - 1
                    ret = tw.PostRetweet(args.ids(i), read)
                Next
                bw.ReportProgress(300)
            Case WORKERTYPE.Follower
                bw.ReportProgress(50, My.Resources.UpdateFollowersMenuItem1_ClickText1)
                ret = tw.GetFollowersApi()
            Case WORKERTYPE.OpenUri
                Dim myPath As String = Convert.ToString(args.url)

                Try
                    If SettingDialog.BrowserPath <> "" Then
                        If SettingDialog.BrowserPath.StartsWith("""") AndAlso SettingDialog.BrowserPath.Length > 2 AndAlso SettingDialog.BrowserPath.IndexOf("""", 2) > -1 Then
                            Dim sep As Integer = SettingDialog.BrowserPath.IndexOf("""", 2)
                            Dim browserPath As String = SettingDialog.BrowserPath.Substring(1, sep - 1)
                            Dim arg As String = ""
                            If sep < SettingDialog.BrowserPath.Length - 1 Then
                                arg = SettingDialog.BrowserPath.Substring(sep + 1)
                            End If
                            myPath = arg + " " + myPath
                            System.Diagnostics.Process.Start(browserPath, myPath)
                        Else
                            System.Diagnostics.Process.Start(SettingDialog.BrowserPath, myPath)
                        End If
                    Else
                        System.Diagnostics.Process.Start(myPath)
                    End If
                Catch ex As Exception
                    '                MessageBox.Show("ブラウザの起動に失敗、またはタイムアウトしました。" + ex.ToString())
                End Try
            Case WORKERTYPE.Favorites
                bw.ReportProgress(50, MakeStatusMessage(args, False))
                ret = tw.GetFavoritesApi(read, args.type)
                rslt.addCount = _statuses.DistributePosts()
            Case WORKERTYPE.PublicSearch
                bw.ReportProgress(50, MakeStatusMessage(args, False))
                If args.tName = "" Then
                    For Each tb As TabClass In _statuses.GetTabsByType(TabUsageType.PublicSearch)
                        If tb.SearchWords <> "" Then ret = tw.GetSearch(read, tb, False)
                    Next
                Else
                    Dim tb As TabClass = _statuses.GetTabByName(args.tName)
                    If tb IsNot Nothing Then
                        ret = tw.GetSearch(read, tb, False)
                        If ret = "" AndAlso args.page = -1 Then
                            ret = tw.GetSearch(read, tb, True)
                        End If
                    End If
                End If
                '振り分け
                rslt.addCount = _statuses.DistributePosts()
            Case WORKERTYPE.UserTimeline
                bw.ReportProgress(50, MakeStatusMessage(args, False))
                Dim count As Integer = 20
                If SettingDialog.UseAdditionalCount Then count = SettingDialog.UserTimelineCountApi
                If args.tName = "" Then
                    For Each tb As TabClass In _statuses.GetTabsByType(TabUsageType.UserTimeline)
                        If tb.User <> "" Then ret = tw.GetUserTimelineApi(read, count, tb.User, tb, False)
                    Next
                Else
                    Dim tb As TabClass = _statuses.GetTabByName(args.tName)
                    If tb IsNot Nothing Then
                        ret = tw.GetUserTimelineApi(read, count, tb.User, tb, args.page = -1)
                    End If
                End If
                '振り分け
                rslt.addCount = _statuses.DistributePosts()
            Case WORKERTYPE.List
                bw.ReportProgress(50, MakeStatusMessage(args, False))
                If args.tName = "" Then
                    '定期更新
                    For Each tb As TabClass In _statuses.GetTabsByType(TabUsageType.Lists)
                        If tb.ListInfo IsNot Nothing AndAlso tb.ListInfo.Id <> 0 Then ret = tw.GetListStatus(read, tb, False, _initial)
                    Next
                Else
                    '手動更新（特定タブのみ更新）
                    Dim tb As TabClass = _statuses.GetTabByName(args.tName)
                    If tb IsNot Nothing Then
                        ret = tw.GetListStatus(read, tb, args.page = -1, _initial)
                    End If
                End If
                '振り分け
                rslt.addCount = _statuses.DistributePosts()
            Case WORKERTYPE.Related
                bw.ReportProgress(50, MakeStatusMessage(args, False))
                Dim tb As TabClass = _statuses.GetTabByName(args.tName)
                ret = tw.GetRelatedResult(read, tb)
                rslt.addCount = _statuses.DistributePosts()
        End Select
        'キャンセル要求
        If bw.CancellationPending Then
            e.Cancel = True
            Exit Sub
        End If

        '時速表示用
        If args.type = WORKERTYPE.FavAdd Then
            Dim oneHour As Date = Now.Subtract(New TimeSpan(1, 0, 0))
            For i As Integer = _favTimestamps.Count - 1 To 0 Step -1
                If _favTimestamps(i).CompareTo(oneHour) < 0 Then
                    _favTimestamps.RemoveAt(i)
                End If
            Next
        End If
        If args.type = WORKERTYPE.Timeline AndAlso Not _initial Then
            SyncLock _syncObject
                Dim tm As Date = Now
                If _tlTimestamps.ContainsKey(tm) Then
                    _tlTimestamps(tm) += rslt.addCount
                Else
                    _tlTimestamps.Add(Now, rslt.addCount)
                End If
                Dim oneHour As Date = Now.Subtract(New TimeSpan(1, 0, 0))
                Dim keys As New List(Of Date)
                _tlCount = 0
                For Each key As Date In _tlTimestamps.Keys
                    If key.CompareTo(oneHour) < 0 Then
                        keys.Add(key)
                    Else
                        _tlCount += _tlTimestamps(key)
                    End If
                Next
                For Each key As Date In keys
                    _tlTimestamps.Remove(key)
                Next
                keys.Clear()
            End SyncLock
        End If

        '終了ステータス
        If args.type <> WORKERTYPE.OpenUri Then bw.ReportProgress(100, MakeStatusMessage(args, True)) 'ステータス書き換え、Notifyアイコンアニメーション開始

        rslt.retMsg = ret
        rslt.type = args.type
        rslt.tName = args.tName
        If args.type = WORKERTYPE.DirectMessegeRcv OrElse _
           args.type = WORKERTYPE.DirectMessegeSnt OrElse _
           args.type = WORKERTYPE.Reply OrElse _
           args.type = WORKERTYPE.Timeline OrElse _
           args.type = WORKERTYPE.Favorites Then
            rslt.page = args.page - 1   '値が正しいか後でチェック。10ページ毎の継続確認
        End If

        e.Result = rslt
    End Sub

    Private Function MakeStatusMessage(ByVal AsyncArg As GetWorkerArg, ByVal Finish As Boolean) As String
        Dim smsg As String = ""
        If Not Finish Then
            '継続中メッセージ
            Select Case AsyncArg.type
                Case WORKERTYPE.Timeline
                    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText5 + AsyncArg.page.ToString() + My.Resources.GetTimelineWorker_RunWorkerCompletedText6
                Case WORKERTYPE.Reply
                    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText4 + AsyncArg.page.ToString() + My.Resources.GetTimelineWorker_RunWorkerCompletedText6
                Case WORKERTYPE.DirectMessegeRcv
                    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText8 + AsyncArg.page.ToString() + My.Resources.GetTimelineWorker_RunWorkerCompletedText6
                Case WORKERTYPE.FavAdd
                    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText15 + AsyncArg.page.ToString() + "/" + AsyncArg.ids.Count.ToString() + _
                                        My.Resources.GetTimelineWorker_RunWorkerCompletedText16 + (AsyncArg.page - AsyncArg.sIds.Count - 1).ToString()
                Case WORKERTYPE.FavRemove
                    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText17 + AsyncArg.page.ToString() + "/" + AsyncArg.ids.Count.ToString() + _
                                        My.Resources.GetTimelineWorker_RunWorkerCompletedText18 + (AsyncArg.page - AsyncArg.sIds.Count - 1).ToString()
                Case WORKERTYPE.Favorites
                    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText19
                Case WORKERTYPE.PublicSearch
                    smsg = "Search refreshing..."
                Case WORKERTYPE.List
                    smsg = "List refreshing..."
                Case WORKERTYPE.Related
                    smsg = "Related refreshing..."
                Case WORKERTYPE.UserTimeline
                    smsg = "UserTimeline refreshing..."
            End Select
        Else
            '完了メッセージ
            Select Case AsyncArg.type
                Case WORKERTYPE.Timeline
                    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText1
                Case WORKERTYPE.Reply
                    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText9
                Case WORKERTYPE.DirectMessegeRcv
                    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText11
                Case WORKERTYPE.DirectMessegeSnt
                    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText13
                Case WORKERTYPE.FavAdd
                    '進捗メッセージ残す
                Case WORKERTYPE.FavRemove
                    '進捗メッセージ残す
                Case WORKERTYPE.Favorites
                    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText20
                Case WORKERTYPE.Follower
                    smsg = My.Resources.UpdateFollowersMenuItem1_ClickText3
                Case WORKERTYPE.PublicSearch
                    smsg = "Search refreshed"
                Case WORKERTYPE.List
                    smsg = "List refreshed"
                Case WORKERTYPE.Related
                    smsg = "Related refreshed"
                Case WORKERTYPE.UserTimeline
                    smsg = "UserTimeline refreshed"
            End Select
        End If
        Return smsg
    End Function

    Private Sub GetTimelineWorker_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs)
        If _endingFlag Then Exit Sub
        If e.ProgressPercentage > 100 Then
            '発言投稿
            If e.ProgressPercentage = 200 Then    '開始
                StatusLabel.Text = "Posting..."
            End If
            If e.ProgressPercentage = 300 Then  '終了
                StatusLabel.Text = My.Resources.PostWorker_RunWorkerCompletedText4
            End If
        Else
            Dim smsg As String = DirectCast(e.UserState, String)
            If smsg.Length > 0 Then StatusLabel.Text = smsg
        End If
    End Sub

    Private Sub GetTimelineWorker_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)

        If _endingFlag OrElse e.Cancelled Then Exit Sub 'キャンセル

        If e.Error IsNot Nothing Then
            _myStatusError = True
            _waitTimeline = False
            _waitReply = False
            _waitDm = False
            _waitFav = False
            _waitPubSearch = False
            _waitUserTimeline = False
            _waitLists = False
            Throw New Exception("BackgroundWorker Exception", e.Error)
            Exit Sub
        End If

        Dim rslt As GetWorkerResult = DirectCast(e.Result, GetWorkerResult)

        If rslt.type = WORKERTYPE.OpenUri Then Exit Sub

        'エラー
        If rslt.retMsg.Length > 0 Then
            _myStatusError = True
            StatusLabel.Text = rslt.retMsg
        End If

        If rslt.type = WORKERTYPE.ErrorState Then Exit Sub

        If rslt.type = WORKERTYPE.FavRemove Then
            Me.RemovePostFromFavTab(rslt.sIds.ToArray)
        End If

        'リストに反映
        'Dim busy As Boolean = False
        'For Each bw As BackgroundWorker In _bw
        '    If bw IsNot Nothing AndAlso bw.IsBusy Then
        '        busy = True
        '        Exit For
        '    End If
        'Next
        'If Not busy Then RefreshTimeline() 'background処理なければ、リスト反映
        If rslt.type = WORKERTYPE.Timeline OrElse _
           rslt.type = WORKERTYPE.Reply OrElse _
           rslt.type = WORKERTYPE.List OrElse _
           rslt.type = WORKERTYPE.PublicSearch OrElse _
           rslt.type = WORKERTYPE.DirectMessegeRcv OrElse _
           rslt.type = WORKERTYPE.DirectMessegeSnt OrElse _
           rslt.type = WORKERTYPE.Favorites OrElse _
           rslt.type = WORKERTYPE.Follower OrElse _
           rslt.type = WORKERTYPE.FavAdd OrElse _
           rslt.type = WORKERTYPE.FavRemove OrElse _
           rslt.type = WORKERTYPE.Related OrElse _
           rslt.type = WORKERTYPE.UserTimeline Then
            RefreshTimeline(False) 'リスト反映
        End If

        Select Case rslt.type
            Case WORKERTYPE.Timeline
                _waitTimeline = False
                If Not _initial Then
                    '    'API使用時の取得調整は別途考える（カウント調整？）
                End If
            Case WORKERTYPE.Reply
                _waitReply = False
                If rslt.newDM AndAlso Not _initial Then
                    GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, 0, "")
                End If
            Case WORKERTYPE.Favorites
                _waitFav = False
            Case WORKERTYPE.DirectMessegeRcv
                _waitDm = False
            Case WORKERTYPE.FavAdd, WORKERTYPE.FavRemove
                If _curList IsNot Nothing AndAlso _curTab IsNot Nothing Then
                    _curList.BeginUpdate()
                    If rslt.type = WORKERTYPE.FavRemove AndAlso _statuses.Tabs(_curTab.Text).TabType = TabUsageType.Favorites Then
                        '色変えは不要
                    Else
                        For i As Integer = 0 To rslt.sIds.Count - 1
                            If _curTab.Text.Equals(rslt.tName) Then
                                Dim idx As Integer = _statuses.Tabs(rslt.tName).IndexOf(rslt.sIds(i))
                                If idx > -1 Then
                                    Dim post As PostClass = Nothing
                                    Dim tb As TabClass = _statuses.Tabs(rslt.tName)
                                    If tb IsNot Nothing Then
                                        If tb.TabType = TabUsageType.Lists OrElse tb.TabType = TabUsageType.PublicSearch Then
                                            post = tb.Posts(rslt.sIds(i))
                                        Else
                                            post = _statuses.Item(rslt.sIds(i))
                                        End If
                                        ChangeCacheStyleRead(post.IsRead, idx, _curTab)
                                    End If
                                    If idx = _curItemIndex Then DispSelectedPost(True) '選択アイテム再表示
                                End If
                            End If
                        Next
                    End If
                    _curList.EndUpdate()
                End If
            Case WORKERTYPE.PostMessage
                If rslt.retMsg = "" OrElse _
                    rslt.retMsg.StartsWith("Outputz") OrElse _
                    rslt.retMsg.StartsWith("OK:") OrElse _
                    rslt.retMsg = "Warn:Status is a duplicate." Then
                    _postTimestamps.Add(Now)
                    Dim oneHour As Date = Now.Subtract(New TimeSpan(1, 0, 0))
                    For i As Integer = _postTimestamps.Count - 1 To 0 Step -1
                        If _postTimestamps(i).CompareTo(oneHour) < 0 Then
                            _postTimestamps.RemoveAt(i)
                        End If
                    Next

                    If Not HashMgr.IsPermanent AndAlso HashMgr.UseHash <> "" Then
                        HashMgr.ClearHashtag()
                        Me.HashStripSplitButton.Text = "#[-]"
                        Me.HashToggleMenuItem.Checked = False
                        Me.HashToggleToolStripMenuItem.Checked = False
                    End If
                    SetMainWindowTitle()
                    rslt.retMsg = ""
                Else
                    Dim retry As DialogResult
                    Try
                        retry = MessageBox.Show(String.Format("{0}   --->   [ " & rslt.retMsg & " ]" & Environment.NewLine & """" & rslt.status.status & """" & Environment.NewLine & "{1}",
                                                            My.Resources.StatusUpdateFailed1,
                                                            My.Resources.StatusUpdateFailed2),
                                                        "Failed to update status",
                                                        MessageBoxButtons.RetryCancel,
                                                        MessageBoxIcon.Question)
                    Catch ex As Exception
                        retry = Windows.Forms.DialogResult.Abort
                    End Try
                    If retry = Windows.Forms.DialogResult.Retry Then
                        Dim args As New GetWorkerArg()
                        args.page = 0
                        args.endPage = 0
                        args.type = WORKERTYPE.PostMessage
                        args.status = rslt.status
                        RunAsync(args)
                    Else
                        If ToolStripFocusLockMenuItem.Checked Then
                            '連投モードのときだけEnterイベントが起きないので強制的に背景色を戻す
                            StatusText_Enter(StatusText, New EventArgs)
                        End If
                    End If
                End If
                If Not _isActiveUserstream AndAlso rslt.retMsg.Length = 0 AndAlso SettingDialog.PostAndGet Then GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
            Case WORKERTYPE.Retweet
                If rslt.retMsg.Length = 0 Then
                    _postTimestamps.Add(Now)
                    Dim oneHour As Date = Now.Subtract(New TimeSpan(1, 0, 0))
                    For i As Integer = _postTimestamps.Count - 1 To 0 Step -1
                        If _postTimestamps(i).CompareTo(oneHour) < 0 Then
                            _postTimestamps.RemoveAt(i)
                        End If
                    Next
                    If Not _isActiveUserstream AndAlso SettingDialog.PostAndGet Then GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
                End If
            Case WORKERTYPE.Follower
                '_waitFollower = False
                _itemCache = Nothing
                _postCache = Nothing
                If _curList IsNot Nothing Then _curList.Refresh()
            Case WORKERTYPE.PublicSearch
                _waitPubSearch = False
            Case WORKERTYPE.UserTimeline
                _waitUserTimeline = False
            Case WORKERTYPE.List
                _waitLists = False
            Case WORKERTYPE.Related
                Dim tb As TabClass = _statuses.GetTabByType(TabUsageType.Related)
                If tb IsNot Nothing AndAlso tb.RelationTargetPost IsNot Nothing AndAlso tb.Contains(tb.RelationTargetPost.StatusId) Then
                    For Each tp As TabPage In ListTab.TabPages
                        If tp.Text = tb.TabName Then
                            DirectCast(tp.Tag, DetailsListView).SelectedIndices.Add(tb.IndexOf(tb.RelationTargetPost.StatusId))
                            DirectCast(tp.Tag, DetailsListView).Items(tb.IndexOf(tb.RelationTargetPost.StatusId)).Focused = True
                            Exit For
                        End If
                    Next
                End If
        End Select

    End Sub

    Private Sub RemovePostFromFavTab(ByVal ids As Int64())
        Dim favTabName As String = _statuses.GetTabByType(TabUsageType.Favorites).TabName
        Dim fidx As Integer
        If _curTab.Text.Equals(favTabName) Then
            If _curList.FocusedItem IsNot Nothing Then
                fidx = _curList.FocusedItem.Index
            ElseIf _curList.TopItem IsNot Nothing Then
                fidx = _curList.TopItem.Index
            Else
                fidx = 0
            End If
        End If

        For Each i As Long In ids
            _statuses.RemoveFavPost(i)
        Next
        If _curTab IsNot Nothing AndAlso _curTab.Text.Equals(favTabName) Then
            _itemCache = Nothing    'キャッシュ破棄
            _postCache = Nothing
            _curPost = Nothing
            '_curItemIndex = -1
        End If
        For Each tp As TabPage In ListTab.TabPages
            If tp.Text = favTabName Then
                DirectCast(tp.Tag, DetailsListView).VirtualListSize = _statuses.Tabs(favTabName).AllCount
                Exit For
            End If
        Next
        If _curTab.Text.Equals(favTabName) Then
            _curList.SelectedIndices.Clear()
            If _statuses.Tabs(favTabName).AllCount > 0 Then
                If _statuses.Tabs(favTabName).AllCount - 1 > fidx AndAlso fidx > -1 Then
                    _curList.SelectedIndices.Add(fidx)
                Else
                    _curList.SelectedIndices.Add(_statuses.Tabs(favTabName).AllCount - 1)
                End If
                If _curList.SelectedIndices.Count > 0 Then
                    _curList.EnsureVisible(_curList.SelectedIndices(0))
                    _curList.FocusedItem = _curList.Items(_curList.SelectedIndices(0))
                End If
            End If
        End If

    End Sub
    Private Sub GetTimeline(ByVal WkType As WORKERTYPE, ByVal fromPage As Integer, ByVal toPage As Integer, ByVal tabName As String)

        If Not IsNetworkAvailable() Then Exit Sub

        '非同期実行引数設定
        Dim args As New GetWorkerArg
        args.page = fromPage
        args.endPage = toPage
        args.type = WkType
        args.tName = tabName

        Static lastTime As New Dictionary(Of WORKERTYPE, DateTime)
        If Not lastTime.ContainsKey(WkType) Then lastTime.Add(WkType, New DateTime)
        Dim period As Double = Now.Subtract(lastTime(WkType)).TotalSeconds
        If period > 1 OrElse period < -1 Then
            lastTime(WkType) = Now
            RunAsync(args)
        End If

        'Timeline取得モードの場合はReplyも同時に取得
        'If Not SettingDialog.UseAPI AndAlso _
        '   Not _initial AndAlso _
        '   WkType = WORKERTYPE.Timeline AndAlso _
        '   SettingDialog.CheckReply Then
        '    'TimerReply.Enabled = False
        '    _mentionCounter = SettingDialog.ReplyPeriodInt
        '    Dim _args As New GetWorkerArg
        '    _args.page = fromPage
        '    _args.endPage = toPage
        '    _args.type = WORKERTYPE.Reply
        '    RunAsync(_args)
        'End If
    End Sub

    Private Sub NotifyIcon1_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me.Visible = True
            If Me.WindowState = FormWindowState.Minimized Then
                Me.WindowState = FormWindowState.Normal
            End If
            Me.Activate()
        End If
    End Sub

    Private Sub MyList_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        MakeReplyOrDirectStatus()
    End Sub

    Private Sub FavAddToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FavAddToolStripMenuItem.Click, FavOpMenuItem.Click
        FavoriteChange(True)
    End Sub

    Private Sub FavRemoveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FavRemoveToolStripMenuItem.Click, UnFavOpMenuItem.Click
        FavoriteChange(False)
    End Sub


    Private Sub FavoriteRetweetMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FavoriteRetweetMenuItem.Click, FavoriteRetweetContextMenu.Click
        FavoritesRetweetOriginal()
    End Sub

    Private Sub FavoriteRetweetUnofficialMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FavoriteRetweetUnofficialMenuItem.Click, FavoriteRetweetUnofficialContextMenu.Click
        FavoritesRetweetUnofficial()
    End Sub

    Private Sub FavoriteChange(ByVal FavAdd As Boolean, Optional ByVal multiFavoriteChangeDialogEnable As Boolean = True)
        'TrueでFavAdd,FalseでFavRemove
        If _statuses.Tabs(_curTab.Text).TabType = TabUsageType.DirectMessage OrElse _curList.SelectedIndices.Count = 0 _
            OrElse Not Me.ExistCurrentPost Then Exit Sub

        '複数fav確認msg
        If _curList.SelectedIndices.Count > 250 AndAlso FavAdd Then
            MessageBox.Show(My.Resources.FavoriteLimitCountText)
            _DoFavRetweetFlags = False
            Exit Sub
        ElseIf multiFavoriteChangeDialogEnable AndAlso _curList.SelectedIndices.Count > 1 Then
            If FavAdd Then
                Dim QuestionText As String = My.Resources.FavAddToolStripMenuItem_ClickText1
                If _DoFavRetweetFlags Then QuestionText = My.Resources.FavoriteRetweetQuestionText3
                If MessageBox.Show(QuestionText, My.Resources.FavAddToolStripMenuItem_ClickText2, _
                                   MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                    _DoFavRetweetFlags = False
                    Exit Sub
                End If
            Else
                If MessageBox.Show(My.Resources.FavRemoveToolStripMenuItem_ClickText1, My.Resources.FavRemoveToolStripMenuItem_ClickText2, _
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                    Exit Sub
                End If
            End If
        End If

        Dim args As New GetWorkerArg
        args.ids = New List(Of Long)
        args.sIds = New List(Of Long)
        args.tName = _curTab.Text
        If FavAdd Then
            args.type = WORKERTYPE.FavAdd
        Else
            args.type = WORKERTYPE.FavRemove
        End If
        For Each idx As Integer In _curList.SelectedIndices
            Dim post As PostClass = GetCurTabPost(idx)
            If FavAdd Then
                If Not post.IsFav Then args.ids.Add(post.StatusId)
            Else
                If post.IsFav Then args.ids.Add(post.StatusId)
            End If
        Next
        If args.ids.Count = 0 Then
            If FavAdd Then
                StatusLabel.Text = My.Resources.FavAddToolStripMenuItem_ClickText4
            Else
                StatusLabel.Text = My.Resources.FavRemoveToolStripMenuItem_ClickText4
            End If
            Exit Sub
        End If

        RunAsync(args)
    End Sub

    Private Function GetCurTabPost(ByVal Index As Integer) As PostClass
        If _postCache IsNot Nothing AndAlso Index >= _itemCacheIndex AndAlso Index < _itemCacheIndex + _postCache.Length Then
            Return _postCache(Index - _itemCacheIndex)
        Else
            Return _statuses.Item(_curTab.Text, Index)
        End If
    End Function


    Private Sub MoveToHomeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveToHomeToolStripMenuItem.Click, OpenHomeOpMenuItem.Click
        If _curList.SelectedIndices.Count > 0 Then
            OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices(0)).ScreenName)
        ElseIf _curList.SelectedIndices.Count = 0 Then
            OpenUriAsync("http://twitter.com/")
        End If
    End Sub

    Private Sub MoveToFavToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveToFavToolStripMenuItem.Click, OpenFavOpMenuItem.Click
        If _curList.SelectedIndices.Count > 0 Then
            OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices(0)).ScreenName + "/favorites")
        End If
    End Sub

    Private Sub Tween_ClientSizeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.ClientSizeChanged
        If (Not _initialLayout) AndAlso _
            Me.Visible AndAlso _
            Me.WindowState = FormWindowState.Normal Then

            'Dim colNo As Integer = 2
            'If _iconCol Then colNo = 1
            'Dim widthDiff As Integer = Me.ClientSize.Width - Me._mySize.Width
            'Dim listView As DetailsListView = CType(Me._curTab.Tag, DetailsListView)
            'Dim column As ColumnHeader = listView.Columns(colNo)
            'column.Width += widthDiff
            'Me.MyList_ColumnWidthChanged(listView, New ColumnWidthChangedEventArgs(colNo))

            _mySize = Me.ClientSize
            _mySpDis = Me.SplitContainer1.SplitterDistance
            _mySpDis3 = Me.SplitContainer3.SplitterDistance
            If StatusText.Multiline Then _mySpDis2 = Me.StatusText.Height
            _modifySettingLocal = True
        End If
    End Sub

    Private Sub MyList_ColumnClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs)
        If SettingDialog.SortOrderLock Then Exit Sub
        Dim mode As IdComparerClass.ComparerMode
        If _iconCol Then
            mode = IdComparerClass.ComparerMode.Id
        Else
            Select Case e.Column
                Case 0, 5, 6    '0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                    'ソートしない
                    Exit Sub
                Case 1  'ニックネーム
                    mode = IdComparerClass.ComparerMode.Nickname
                Case 2  '本文
                    mode = IdComparerClass.ComparerMode.Data
                Case 3  '時刻=発言Id
                    mode = IdComparerClass.ComparerMode.Id
                Case 4  '名前
                    mode = IdComparerClass.ComparerMode.Name
                Case 7  'Source
                    mode = IdComparerClass.ComparerMode.Source
            End Select
        End If
        _statuses.ToggleSortOrder(mode)
        InitColumnText()

        If _iconCol Then
            DirectCast(sender, DetailsListView).Columns.Item(0).Text = ColumnOrgText(0)
            DirectCast(sender, DetailsListView).Columns.Item(1).Text = ColumnText(2)
        Else
            For i As Integer = 0 To 7
                DirectCast(sender, DetailsListView).Columns.Item(i).Text = ColumnOrgText(i)
            Next
            DirectCast(sender, DetailsListView).Columns.Item(e.Column).Text = ColumnText(e.Column)
        End If

        _itemCache = Nothing
        _postCache = Nothing

        If _statuses.Tabs(_curTab.Text).AllCount > 0 AndAlso _curPost IsNot Nothing Then
            Dim idx As Integer = _statuses.Tabs(_curTab.Text).IndexOf(_curPost.StatusId)
            SelectListItem(_curList, idx)
            _curList.EnsureVisible(idx)
        End If
        _curList.Refresh()
        _modifySettingCommon = True
    End Sub

    Private Sub Tween_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LocationChanged
        If Me.WindowState = FormWindowState.Normal AndAlso Not _initialLayout Then
            _myLoc = Me.DesktopLocation
            _modifySettingLocal = True
        End If
    End Sub

    Private Sub ContextMenuOperate_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuOperate.Opening
        If ListTab.SelectedTab Is Nothing Then Exit Sub
        If _statuses Is Nothing OrElse _statuses.Tabs Is Nothing OrElse Not _statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text) Then Exit Sub
        If Not Me.ExistCurrentPost Then
            ReplyStripMenuItem.Enabled = False
            ReplyAllStripMenuItem.Enabled = False
            DMStripMenuItem.Enabled = False
            ShowProfileMenuItem.Enabled = False
            ShowUserTimelineContextMenuItem.Enabled = False
            ListManageUserContextToolStripMenuItem2.Enabled = False
            MoveToFavToolStripMenuItem.Enabled = False
            TabMenuItem.Enabled = False
            IDRuleMenuItem.Enabled = False
            ReadedStripMenuItem.Enabled = False
            UnreadStripMenuItem.Enabled = False
        Else
            ShowProfileMenuItem.Enabled = True
            ListManageUserContextToolStripMenuItem2.Enabled = True
            ReplyStripMenuItem.Enabled = True
            ReplyAllStripMenuItem.Enabled = True
            DMStripMenuItem.Enabled = True
            ShowUserTimelineContextMenuItem.Enabled = True
            MoveToFavToolStripMenuItem.Enabled = True
            TabMenuItem.Enabled = True
            IDRuleMenuItem.Enabled = True
            ReadedStripMenuItem.Enabled = True
            UnreadStripMenuItem.Enabled = True
        End If
        If _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.DirectMessage OrElse Not Me.ExistCurrentPost OrElse _curPost.IsDm Then
            FavAddToolStripMenuItem.Enabled = False
            FavRemoveToolStripMenuItem.Enabled = False
            StatusOpenMenuItem.Enabled = False
            FavorareMenuItem.Enabled = False
            ShowRelatedStatusesMenuItem.Enabled = False

            ReTweetStripMenuItem.Enabled = False
            ReTweetOriginalStripMenuItem.Enabled = False
            QuoteStripMenuItem.Enabled = False
            FavoriteRetweetContextMenu.Enabled = False
            FavoriteRetweetUnofficialContextMenu.Enabled = False
            If Me.ExistCurrentPost AndAlso _curPost.IsDm Then DeleteStripMenuItem.Enabled = True
        Else
            FavAddToolStripMenuItem.Enabled = True
            FavRemoveToolStripMenuItem.Enabled = True
            StatusOpenMenuItem.Enabled = True
            FavorareMenuItem.Enabled = True
            ShowRelatedStatusesMenuItem.Enabled = True  'PublicSearchの時問題出るかも

            If _curPost.IsMe Then
                ReTweetOriginalStripMenuItem.Enabled = False
                FavoriteRetweetContextMenu.Enabled = False
                DeleteStripMenuItem.Enabled = True
            Else
                DeleteStripMenuItem.Enabled = False
                If _curPost.IsProtect Then
                    ReTweetOriginalStripMenuItem.Enabled = False
                    ReTweetStripMenuItem.Enabled = False
                    QuoteStripMenuItem.Enabled = False
                    FavoriteRetweetContextMenu.Enabled = False
                    FavoriteRetweetUnofficialContextMenu.Enabled = False
                Else
                    ReTweetOriginalStripMenuItem.Enabled = True
                    ReTweetStripMenuItem.Enabled = True
                    QuoteStripMenuItem.Enabled = True
                    FavoriteRetweetContextMenu.Enabled = True
                    FavoriteRetweetUnofficialContextMenu.Enabled = True
                End If
            End If
        End If
        If _statuses.Tabs(ListTab.SelectedTab.Text).TabType <> TabUsageType.Favorites Then
            RefreshMoreStripMenuItem.Enabled = True
        Else
            RefreshMoreStripMenuItem.Enabled = False
        End If
        If _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.PublicSearch _
                            OrElse Not Me.ExistCurrentPost _
                            OrElse Not _curPost.InReplyToStatusId > 0 Then
            RepliedStatusOpenMenuItem.Enabled = False
        Else
            RepliedStatusOpenMenuItem.Enabled = True
        End If
        If Not Me.ExistCurrentPost OrElse _curPost.RetweetedBy = "" Then
            MoveToRTHomeMenuItem.Enabled = False
        Else
            MoveToRTHomeMenuItem.Enabled = True
        End If
    End Sub

    Private Sub ReplyStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReplyStripMenuItem.Click, ReplyOpMenuItem.Click
        MakeReplyOrDirectStatus(False, True)
    End Sub

    Private Sub DMStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DMStripMenuItem.Click, DmOpMenuItem.Click
        MakeReplyOrDirectStatus(False, False)
    End Sub

    Private Sub doStatusDelete()
        If _curTab Is Nothing OrElse _curList Is Nothing Then Exit Sub
        If _statuses.Tabs(_curTab.Text).TabType <> TabUsageType.DirectMessage Then
            Dim myPost As Boolean = False
            For Each idx As Integer In _curList.SelectedIndices
                If GetCurTabPost(idx).IsMe OrElse _
                   GetCurTabPost(idx).RetweetedBy.ToLower = tw.Username.ToLower Then
                    myPost = True
                    Exit For
                End If
            Next
            If Not myPost Then Exit Sub
        Else
            If _curList.SelectedIndices.Count = 0 Then
                Exit Sub
            End If
        End If

        Dim tmp As String = String.Format(My.Resources.DeleteStripMenuItem_ClickText1, Environment.NewLine)

        If MessageBox.Show(tmp, My.Resources.DeleteStripMenuItem_ClickText2, _
              MessageBoxButtons.OKCancel, _
              MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then Exit Sub

        Dim fidx As Integer
        If _curList.FocusedItem IsNot Nothing Then
            fidx = _curList.FocusedItem.Index
        ElseIf _curList.TopItem IsNot Nothing Then
            fidx = _curList.TopItem.Index
        Else
            fidx = 0
        End If

        Try
            Me.Cursor = Cursors.WaitCursor

            Dim rslt As Boolean = True
            For Each Id As Long In _statuses.GetId(_curTab.Text, _curList.SelectedIndices)
                Dim rtn As String = ""
                If _statuses.Tabs(_curTab.Text).TabType = TabUsageType.DirectMessage Then
                    rtn = tw.RemoveDirectMessage(Id, _statuses.Item(Id))
                Else
                    If _statuses.Item(Id).IsMe OrElse _statuses.Item(Id).RetweetedBy.ToLower = tw.Username.ToLower Then
                        rtn = tw.RemoveStatus(Id)
                    Else
                        Continue For
                    End If
                End If
                If rtn.Length > 0 Then
                    'エラー
                    rslt = False
                Else
                    _statuses.RemovePost(Id)
                End If
            Next

            If Not rslt Then
                StatusLabel.Text = My.Resources.DeleteStripMenuItem_ClickText3  '失敗
            Else
                StatusLabel.Text = My.Resources.DeleteStripMenuItem_ClickText4  '成功
            End If

            _itemCache = Nothing    'キャッシュ破棄
            _postCache = Nothing
            _curPost = Nothing
            _curItemIndex = -1
            For Each tb As TabPage In ListTab.TabPages
                DirectCast(tb.Tag, DetailsListView).VirtualListSize = _statuses.Tabs(tb.Text).AllCount
                If _curTab.Equals(tb) Then
                    _curList.SelectedIndices.Clear()
                    If _statuses.Tabs(tb.Text).AllCount > 0 Then
                        If _statuses.Tabs(tb.Text).AllCount - 1 > fidx AndAlso fidx > -1 Then
                            _curList.SelectedIndices.Add(fidx)
                        Else
                            _curList.SelectedIndices.Add(_statuses.Tabs(tb.Text).AllCount - 1)
                        End If
                        If _curList.SelectedIndices.Count > 0 Then
                            _curList.EnsureVisible(_curList.SelectedIndices(0))
                            _curList.FocusedItem = _curList.Items(_curList.SelectedIndices(0))
                        End If
                    End If
                End If
                If _statuses.Tabs(tb.Text).UnreadCount = 0 Then
                    If SettingDialog.TabIconDisp Then
                        If tb.ImageIndex = 0 Then tb.ImageIndex = -1 'タブアイコン
                    End If
                End If
            Next
            If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub DeleteStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteStripMenuItem.Click, DelOpMenuItem.Click
        doStatusDelete()
    End Sub

    Private Sub ReadedStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReadedStripMenuItem.Click, ReadOpMenuItem.Click
        _curList.BeginUpdate()
        If SettingDialog.UnreadManage Then
            For Each idx As Integer In _curList.SelectedIndices
                _statuses.SetReadAllTab(True, _curTab.Text, idx)
            Next
        End If
        For Each idx As Integer In _curList.SelectedIndices
            ChangeCacheStyleRead(True, idx, _curTab)
        Next
        ColorizeList()
        _curList.EndUpdate()
        For Each tb As TabPage In ListTab.TabPages
            If _statuses.Tabs(tb.Text).UnreadCount = 0 Then
                If SettingDialog.TabIconDisp Then
                    If tb.ImageIndex = 0 Then tb.ImageIndex = -1 'タブアイコン
                End If
            End If
        Next
        If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
    End Sub

    Private Sub UnreadStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UnreadStripMenuItem.Click, UnreadOpMenuItem.Click
        _curList.BeginUpdate()
        If SettingDialog.UnreadManage Then
            For Each idx As Integer In _curList.SelectedIndices
                _statuses.SetReadAllTab(False, _curTab.Text, idx)
            Next
        End If
        For Each idx As Integer In _curList.SelectedIndices
            ChangeCacheStyleRead(False, idx, _curTab)
        Next
        ColorizeList()
        _curList.EndUpdate()
        For Each tb As TabPage In ListTab.TabPages
            If _statuses.Tabs(tb.Text).UnreadCount > 0 Then
                If SettingDialog.TabIconDisp Then
                    If tb.ImageIndex = -1 Then tb.ImageIndex = 0 'タブアイコン
                End If
            End If
        Next
        If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
    End Sub

    Private Sub RefreshStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshStripMenuItem.Click, RefreshOpMenuItem.Click
        DoRefresh()
    End Sub

    Private Sub DoRefresh()
        If _curTab IsNot Nothing Then
            Select Case _statuses.Tabs(_curTab.Text).TabType
                Case TabUsageType.Mentions
                    GetTimeline(WORKERTYPE.Reply, 1, 0, "")
                Case TabUsageType.DirectMessage
                    GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, 0, "")
                Case TabUsageType.Favorites
                    GetTimeline(WORKERTYPE.Favorites, 1, 0, "")
                    'Case TabUsageType.Profile
                    '' TODO
                Case TabUsageType.PublicSearch
                    '' TODO
                    Dim tb As TabClass = _statuses.Tabs(_curTab.Text)
                    If tb.SearchWords = "" Then Exit Sub
                    GetTimeline(WORKERTYPE.PublicSearch, 1, 0, _curTab.Text)
                Case TabUsageType.UserTimeline
                    GetTimeline(WORKERTYPE.UserTimeline, 1, 0, _curTab.Text)
                Case TabUsageType.Lists
                    '' TODO
                    Dim tb As TabClass = _statuses.Tabs(_curTab.Text)
                    If tb.ListInfo Is Nothing OrElse tb.ListInfo.Id = 0 Then Exit Sub
                    GetTimeline(WORKERTYPE.List, 1, 0, _curTab.Text)
                Case Else
                    GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
            End Select
        Else
            GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
        End If
    End Sub

    Private Sub DoRefreshMore()
        'ページ指定をマイナス1に
        If _curTab IsNot Nothing Then
            Select Case _statuses.Tabs(_curTab.Text).TabType
                Case TabUsageType.Mentions
                    GetTimeline(WORKERTYPE.Reply, -1, 0, "")
                Case TabUsageType.DirectMessage
                    GetTimeline(WORKERTYPE.DirectMessegeRcv, -1, 0, "")
                Case TabUsageType.Favorites
                    '    GetTimeline(WORKERTYPE.Favorites, -1, 0, "")
                Case TabUsageType.Profile
                    '' TODO
                Case TabUsageType.PublicSearch
                    ' TODO
                    Dim tb As TabClass = _statuses.Tabs(_curTab.Text)
                    If tb.SearchWords = "" Then Exit Sub
                    GetTimeline(WORKERTYPE.PublicSearch, -1, 0, _curTab.Text)
                Case TabUsageType.UserTimeline
                    GetTimeline(WORKERTYPE.UserTimeline, -1, 0, _curTab.Text)
                Case TabUsageType.Lists
                    '' TODO
                    Dim tb As TabClass = _statuses.Tabs(_curTab.Text)
                    If tb.ListInfo Is Nothing OrElse tb.ListInfo.Id = 0 Then Exit Sub
                    GetTimeline(WORKERTYPE.List, -1, 0, _curTab.Text)
                Case Else
                    GetTimeline(WORKERTYPE.Timeline, -1, 0, "")
            End Select
        Else
            GetTimeline(WORKERTYPE.Timeline, -1, 0, "")
        End If
    End Sub

    Private Sub SettingStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingStripMenuItem.Click, SettingFileMenuItem.Click
        Dim result As DialogResult
        Dim uid As String = tw.Username.ToLower

        Try
            result = SettingDialog.ShowDialog()
        Catch ex As Exception
            Exit Sub
        End Try

        If result = Windows.Forms.DialogResult.OK Then
            SyncLock _syncObject
                'Try
                '    If SettingDialog.TimelinePeriodInt > 0 Then
                '        _homeCounterAdjuster = 0
                '    End If
                'Catch ex As Exception
                '    ex.Data("Instance") = "Set Timers"
                '    ex.Data("IsTerminatePermission") = False
                '    Throw
                'End Try
                'tw.CountApi = SettingDialog.CountApi
                'tw.CountApiReply = SettingDialog.CountApiReply
                tw.TinyUrlResolve = SettingDialog.TinyUrlResolve
                tw.RestrictFavCheck = SettingDialog.RestrictFavCheck
                tw.ReadOwnPost = SettingDialog.ReadOwnPost
                tw.UseSsl = SettingDialog.UseSsl
                ShortUrl.IsResolve = SettingDialog.TinyUrlResolve
                ShortUrl.BitlyId = SettingDialog.BitlyUser
                ShortUrl.BitlyKey = SettingDialog.BitlyPwd
                HttpTwitter.TwitterUrl = _cfgCommon.TwitterUrl
                HttpTwitter.TwitterSearchUrl = _cfgCommon.TwitterSearchUrl

                HttpConnection.InitializeConnection(SettingDialog.DefaultTimeOut, _
                                                    SettingDialog.SelectedProxyType, _
                                                    SettingDialog.ProxyAddress, _
                                                    SettingDialog.ProxyPort, _
                                                    SettingDialog.ProxyUser, _
                                                    SettingDialog.ProxyPassword)
                Try
                    If SettingDialog.TabIconDisp Then
                        RemoveHandler ListTab.DrawItem, AddressOf ListTab_DrawItem
                        ListTab.DrawMode = TabDrawMode.Normal
                        ListTab.ImageList = Me.TabImage
                    Else
                        RemoveHandler ListTab.DrawItem, AddressOf ListTab_DrawItem
                        AddHandler ListTab.DrawItem, AddressOf ListTab_DrawItem
                        ListTab.DrawMode = TabDrawMode.OwnerDrawFixed
                        ListTab.ImageList = Nothing
                    End If
                Catch ex As Exception
                    ex.Data("Instance") = "ListTab(TabIconDisp)"
                    ex.Data("IsTerminatePermission") = False
                    Throw
                End Try

                Try
                    If Not SettingDialog.UnreadManage Then
                        ReadedStripMenuItem.Enabled = False
                        UnreadStripMenuItem.Enabled = False
                        If SettingDialog.TabIconDisp Then
                            For Each myTab As TabPage In ListTab.TabPages
                                myTab.ImageIndex = -1
                            Next
                        End If
                    Else
                        ReadedStripMenuItem.Enabled = True
                        UnreadStripMenuItem.Enabled = True
                    End If
                Catch ex As Exception
                    ex.Data("Instance") = "ListTab(UnreadManage)"
                    ex.Data("IsTerminatePermission") = False
                    Throw
                End Try

                Try
                    For Each mytab As TabPage In ListTab.TabPages
                        Dim lst As DetailsListView = DirectCast(mytab.Tag, DetailsListView)
                        lst.GridLines = SettingDialog.ShowGrid
                    Next
                Catch ex As Exception
                    ex.Data("Instance") = "ListTab(ShowGrid)"
                    ex.Data("IsTerminatePermission") = False
                    Throw
                End Try

                PlaySoundMenuItem.Checked = SettingDialog.PlaySound
                Me.PlaySoundFileMenuItem.Checked = SettingDialog.PlaySound
                _fntUnread = SettingDialog.FontUnread
                _clUnread = SettingDialog.ColorUnread
                _fntReaded = SettingDialog.FontReaded
                _clReaded = SettingDialog.ColorReaded
                _clFav = SettingDialog.ColorFav
                _clOWL = SettingDialog.ColorOWL
                _clRetweet = SettingDialog.ColorRetweet
                _fntDetail = SettingDialog.FontDetail
                _clDetail = SettingDialog.ColorDetail
                _clDetailLink = SettingDialog.ColorDetailLink
                _clDetailBackcolor = SettingDialog.ColorDetailBackcolor
                _clSelf = SettingDialog.ColorSelf
                _clAtSelf = SettingDialog.ColorAtSelf
                _clTarget = SettingDialog.ColorTarget
                _clAtTarget = SettingDialog.ColorAtTarget
                _clAtFromTarget = SettingDialog.ColorAtFromTarget
                _clAtTo = SettingDialog.ColorAtTo
                _clListBackcolor = SettingDialog.ColorListBackcolor
                _clInputBackcolor = SettingDialog.ColorInputBackcolor
                _clInputFont = SettingDialog.ColorInputFont
                _fntInputFont = SettingDialog.FontInputFont
                Try
                    If StatusText.Focused Then StatusText.BackColor = _clInputBackcolor
                    StatusText.Font = _fntInputFont
                    StatusText.ForeColor = _clInputFont
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try

                _brsForeColorUnread.Dispose()
                _brsForeColorReaded.Dispose()
                _brsForeColorFav.Dispose()
                _brsForeColorOWL.Dispose()
                _brsForeColorRetweet.Dispose()
                _brsForeColorUnread = New SolidBrush(_clUnread)
                _brsForeColorReaded = New SolidBrush(_clReaded)
                _brsForeColorFav = New SolidBrush(_clFav)
                _brsForeColorOWL = New SolidBrush(_clOWL)
                _brsForeColorRetweet = New SolidBrush(_clRetweet)
                _brsBackColorMine.Dispose()
                _brsBackColorAt.Dispose()
                _brsBackColorYou.Dispose()
                _brsBackColorAtYou.Dispose()
                _brsBackColorAtFromTarget.Dispose()
                _brsBackColorAtTo.Dispose()
                _brsBackColorNone.Dispose()
                _brsBackColorMine = New SolidBrush(_clSelf)
                _brsBackColorAt = New SolidBrush(_clAtSelf)
                _brsBackColorYou = New SolidBrush(_clTarget)
                _brsBackColorAtYou = New SolidBrush(_clAtTarget)
                _brsBackColorAtFromTarget = New SolidBrush(_clAtFromTarget)
                _brsBackColorAtTo = New SolidBrush(_clAtTo)
                _brsBackColorNone = New SolidBrush(_clListBackcolor)
                Try
                    If SettingDialog.IsMonospace Then
                        detailHtmlFormatHeader = detailHtmlFormatMono1
                        detailHtmlFormatFooter = detailHtmlFormatMono7
                    Else
                        detailHtmlFormatHeader = detailHtmlFormat1
                        detailHtmlFormatFooter = detailHtmlFormat7
                    End If
                    detailHtmlFormatHeader += _fntDetail.Name + detailHtmlFormat2 + _fntDetail.Size.ToString() + detailHtmlFormat3 + _clDetail.R.ToString + "," + _clDetail.G.ToString + "," + _clDetail.B.ToString + detailHtmlFormat4 + _clDetailLink.R.ToString + "," + _clDetailLink.G.ToString + "," + _clDetailLink.B.ToString + detailHtmlFormat5 + _clDetailBackcolor.R.ToString + "," + _clDetailBackcolor.G.ToString + "," + _clDetailBackcolor.B.ToString
                    If SettingDialog.IsMonospace Then
                        detailHtmlFormatHeader += detailHtmlFormatMono6
                    Else
                        detailHtmlFormatHeader += detailHtmlFormat6
                    End If
                Catch ex As Exception
                    ex.Data("Instance") = "Font"
                    ex.Data("IsTerminatePermission") = False
                    Throw
                End Try
                Try
                    _statuses.SetUnreadManage(SettingDialog.UnreadManage)
                Catch ex As Exception
                    ex.Data("Instance") = "_statuses"
                    ex.Data("IsTerminatePermission") = False
                    Throw
                End Try

                Try
                    For Each tb As TabPage In ListTab.TabPages
                        If SettingDialog.TabIconDisp Then
                            If _statuses.Tabs(tb.Text).UnreadCount = 0 Then
                                tb.ImageIndex = -1
                            Else
                                tb.ImageIndex = 0
                            End If
                        End If
                        If tb.Tag IsNot Nothing AndAlso tb.Controls.Count > 0 Then
                            DirectCast(tb.Tag, DetailsListView).Font = _fntReaded
                            DirectCast(tb.Tag, DetailsListView).BackColor = _clListBackcolor
                        End If
                    Next
                Catch ex As Exception
                    ex.Data("Instance") = "ListTab(TabIconDisp no2)"
                    ex.Data("IsTerminatePermission") = False
                    Throw
                End Try
                SetMainWindowTitle()
                SetNotifyIconText()

                _itemCache = Nothing
                _postCache = Nothing
                If _curList IsNot Nothing Then _curList.Refresh()
                ListTab.Refresh()

                Outputz.Key = SettingDialog.OutputzKey
                Outputz.Enabled = SettingDialog.OutputzEnabled
                Select Case SettingDialog.OutputzUrlmode
                    Case OutputzUrlmode.twittercom
                        Outputz.OutUrl = "http://twitter.com/"
                    Case OutputzUrlmode.twittercomWithUsername
                        Outputz.OutUrl = "http://twitter.com/" + tw.Username
                End Select

                _hookGlobalHotkey.UnregisterAllOriginalHotkey()
                If SettingDialog.HotkeyEnabled Then
                    '''グローバルホットキーの登録。設定で変更可能にするかも
                    Dim modKey As HookGlobalHotkey.ModKeys = HookGlobalHotkey.ModKeys.None
                    If (SettingDialog.HotkeyMod And Keys.Alt) = Keys.Alt Then modKey = modKey Or HookGlobalHotkey.ModKeys.Alt
                    If (SettingDialog.HotkeyMod And Keys.Control) = Keys.Control Then modKey = modKey Or HookGlobalHotkey.ModKeys.Ctrl
                    If (SettingDialog.HotkeyMod And Keys.Shift) = Keys.Shift Then modKey = modKey Or HookGlobalHotkey.ModKeys.Shift
                    If (SettingDialog.HotkeyMod And Keys.LWin) = Keys.LWin Then modKey = modKey Or HookGlobalHotkey.ModKeys.Win

                    _hookGlobalHotkey.RegisterOriginalHotkey(SettingDialog.HotkeyKey, SettingDialog.HotkeyValue, modKey)
                End If

                If uid <> tw.Username Then Me.doGetFollowersMenu()

                SetImageServiceCombo()

                Try
                    StatusText_TextChanged(Nothing, Nothing)
                Catch ex As Exception

                End Try
            End SyncLock
        End If

        Twitter.AccountState = ACCOUNT_STATE.Valid

        Me.TopMost = SettingDialog.AlwaysTop
        SaveConfigsAll(False)
    End Sub

    Private Sub PostBrowser_Navigated(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserNavigatedEventArgs) Handles PostBrowser.Navigated
        If e.Url.AbsoluteUri <> "about:blank" Then
            DispSelectedPost()
            OpenUriAsync(e.Url.OriginalString)
        End If
    End Sub

    Private Sub PostBrowser_Navigating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs) Handles PostBrowser.Navigating
        If e.Url.Scheme = "data" Then
            StatusLabelUrl.Text = PostBrowser.StatusText.Replace("&", "&&")
        ElseIf e.Url.AbsoluteUri <> "about:blank" Then
            e.Cancel = True

            If e.Url.AbsoluteUri.StartsWith("http://twitter.com/search?q=%23") OrElse _
               e.Url.AbsoluteUri.StartsWith("https://twitter.com/search?q=%23") Then
                'ハッシュタグの場合は、タブで開く
                Dim urlStr As String = HttpUtility.UrlDecode(e.Url.AbsoluteUri)
                Dim hash As String = urlStr.Substring(urlStr.IndexOf("#"))
                HashSupl.AddItem(hash)
                HashMgr.AddHashToHistory(hash.Trim, False)
                AddNewTabForSearch(hash)
                Exit Sub
            Else
                Dim m As Match = Regex.Match(e.Url.AbsoluteUri, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$")
                If SettingDialog.OpenUserTimeline AndAlso m.Success AndAlso IsTwitterId(m.Result("${ScreenName}")) Then
                    Me.AddNewTabForUserTimeline(m.Result("${ScreenName}"))
                Else
                    OpenUriAsync(e.Url.OriginalString)
                End If
            End If
        End If
    End Sub

    Public Sub AddNewTabForSearch(ByVal searchWord As String)
        '同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
        For Each tb As TabClass In _statuses.GetTabsByType(TabUsageType.PublicSearch)
            If tb.SearchWords = searchWord AndAlso tb.SearchLang = "" Then
                For Each tp As TabPage In ListTab.TabPages
                    If tb.TabName = tp.Text Then
                        ListTab.SelectedTab = tp
                        Exit Sub
                    End If
                Next
            End If
        Next
        'ユニークなタブ名生成
        Dim tabName As String = searchWord
        For i As Integer = 0 To 100
            If _statuses.ContainsTab(tabName) Then
                tabName += "_"
            Else
                Exit For
            End If
        Next
        'タブ追加
        AddNewTab(tabName, False, TabUsageType.PublicSearch)
        _statuses.AddTab(tabName, TabUsageType.PublicSearch, Nothing)
        '追加したタブをアクティブに
        ListTab.SelectedIndex = ListTab.TabPages.Count - 1
        '検索条件の設定
        Dim cmb As ComboBox = DirectCast(ListTab.SelectedTab.Controls("panelSearch").Controls("comboSearch"), ComboBox)
        cmb.Items.Add(searchWord)
        cmb.Text = searchWord
        SaveConfigsTabs()
        '検索実行
        Me.SearchButton_Click(ListTab.SelectedTab.Controls("panelSearch").Controls("comboSearch"), Nothing)
    End Sub

    Private Sub ShowUserTimeline()
        If Not Me.ExistCurrentPost Then Exit Sub
        AddNewTabForUserTimeline(_curPost.ScreenName)
    End Sub

    Public Sub AddNewTabForUserTimeline(ByVal user As String)
        '同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
        For Each tb As TabClass In _statuses.GetTabsByType(TabUsageType.UserTimeline)
            If tb.User = user Then
                For Each tp As TabPage In ListTab.TabPages
                    If tb.TabName = tp.Text Then
                        ListTab.SelectedTab = tp
                        Exit Sub
                    End If
                Next
            End If
        Next
        'ユニークなタブ名生成
        Dim tabName As String = "user:" + user
        While _statuses.ContainsTab(tabName)
            tabName += "_"
        End While
        'タブ追加
        _statuses.AddTab(tabName, TabUsageType.UserTimeline, Nothing)
        _statuses.Tabs(tabName).User = user
        AddNewTab(tabName, False, TabUsageType.UserTimeline)
        '追加したタブをアクティブに
        ListTab.SelectedIndex = ListTab.TabPages.Count - 1
        SaveConfigsTabs()
        '検索実行

        GetTimeline(WORKERTYPE.UserTimeline, 1, 0, tabName)
    End Sub

    Public Function AddNewTab(ByVal tabName As String, ByVal startup As Boolean, ByVal tabType As TabUsageType, Optional ByVal listInfo As ListElement = Nothing) As Boolean
        '重複チェック
        For Each tb As TabPage In ListTab.TabPages
            If tb.Text = tabName Then Return False
        Next

        '新規タブ名チェック
        If tabName = My.Resources.AddNewTabText1 Then Return False

        'タブタイプ重複チェック
        If Not startup Then
            If tabType = TabUsageType.DirectMessage OrElse _
               tabType = TabUsageType.Favorites OrElse _
               tabType = TabUsageType.Home OrElse _
               tabType = TabUsageType.Mentions OrElse _
               tabType = TabUsageType.Related Then
                If _statuses.GetTabByType(tabType) IsNot Nothing Then Return False
            End If
        End If

        Dim _tabPage As TabPage = New TabPage
        Dim _listCustom As DetailsListView = New DetailsListView
        Dim _colHd1 As ColumnHeader = New ColumnHeader()  'アイコン
        Dim _colHd2 As ColumnHeader = New ColumnHeader()   'ニックネーム
        Dim _colHd3 As ColumnHeader = New ColumnHeader()   '本文
        Dim _colHd4 As ColumnHeader = New ColumnHeader()   '日付
        Dim _colHd5 As ColumnHeader = New ColumnHeader()   'ユーザID
        Dim _colHd6 As ColumnHeader = New ColumnHeader()   '未読
        Dim _colHd7 As ColumnHeader = New ColumnHeader()   'マーク＆プロテクト
        Dim _colHd8 As ColumnHeader = New ColumnHeader()   'ソース

        Dim cnt As Integer = ListTab.TabPages.Count

        '''ToDo:Create and set controls follow tabtypes

        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ListTab.SuspendLayout()
        Me.SuspendLayout()

        _tabPage.SuspendLayout()

        ''' UserTimeline関連
        Dim label As Label = Nothing
        If tabType = TabUsageType.UserTimeline OrElse tabType = TabUsageType.Lists Then
            label = New Label()
            label.Dock = DockStyle.Top
            label.Name = "labelUser"
            If tabType = TabUsageType.Lists Then
                label.Text = listInfo.ToString()
            Else
                label.Text = _statuses.Tabs(tabName).User + "'s Timeline"
            End If
            label.TextAlign = ContentAlignment.MiddleLeft
            Using tmpComboBox As New ComboBox()
                label.Height = tmpComboBox.Height
            End Using
            _tabPage.Controls.Add(label)
        End If

        ''' 検索関連の準備
        Dim pnl As Panel = Nothing
        If tabType = TabUsageType.PublicSearch Then
            pnl = New Panel

            Dim lbl As New Label
            Dim cmb As New ComboBox
            Dim btn As New Button
            Dim cmbLang As New ComboBox

            pnl.SuspendLayout()

            pnl.Controls.Add(cmb)
            pnl.Controls.Add(cmbLang)
            pnl.Controls.Add(btn)
            pnl.Controls.Add(lbl)
            pnl.Name = "panelSearch"
            pnl.Dock = DockStyle.Top
            pnl.Height = cmb.Height
            AddHandler pnl.Enter, AddressOf SearchControls_Enter
            AddHandler pnl.Leave, AddressOf SearchControls_Leave

            cmb.Text = ""
            cmb.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            cmb.Dock = DockStyle.Fill
            cmb.Name = "comboSearch"
            cmb.DropDownStyle = ComboBoxStyle.DropDown
            cmb.ImeMode = Windows.Forms.ImeMode.NoControl
            cmb.TabStop = False
            cmb.AutoCompleteMode = AutoCompleteMode.None

            If _statuses.ContainsTab(tabName) Then
                cmb.Items.Add(_statuses.Tabs(tabName).SearchWords)
                cmb.Text = _statuses.Tabs(tabName).SearchWords
            End If

            cmbLang.Text = ""
            cmbLang.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            cmbLang.Dock = DockStyle.Right
            cmbLang.Width = 50
            cmbLang.Name = "comboLang"
            cmbLang.DropDownStyle = ComboBoxStyle.DropDownList
            cmbLang.TabStop = False
            cmbLang.Items.Add("")
            cmbLang.Items.Add("ja")
            cmbLang.Items.Add("en")
            cmbLang.Items.Add("ar")
            cmbLang.Items.Add("da")
            cmbLang.Items.Add("nl")
            cmbLang.Items.Add("fa")
            cmbLang.Items.Add("fi")
            cmbLang.Items.Add("fr")
            cmbLang.Items.Add("de")
            cmbLang.Items.Add("hu")
            cmbLang.Items.Add("is")
            cmbLang.Items.Add("it")
            cmbLang.Items.Add("no")
            cmbLang.Items.Add("pl")
            cmbLang.Items.Add("pt")
            cmbLang.Items.Add("ru")
            cmbLang.Items.Add("es")
            cmbLang.Items.Add("sv")
            cmbLang.Items.Add("th")
            If _statuses.ContainsTab(tabName) Then cmbLang.Text = _statuses.Tabs(tabName).SearchLang

            lbl.Text = "Search(C-S-f)"
            lbl.Name = "label1"
            lbl.Dock = DockStyle.Left
            lbl.Width = 90
            lbl.Height = cmb.Height
            lbl.TextAlign = ContentAlignment.MiddleLeft

            btn.Text = "Search"
            btn.Name = "buttonSearch"
            btn.UseVisualStyleBackColor = True
            btn.Dock = DockStyle.Right
            btn.TabStop = False
            AddHandler btn.Click, AddressOf SearchButton_Click
        End If

        Me.ListTab.Controls.Add(_tabPage)
        _tabPage.Controls.Add(_listCustom)

        If tabType = TabUsageType.PublicSearch Then _tabPage.Controls.Add(pnl)
        If tabType = TabUsageType.UserTimeline OrElse tabType = TabUsageType.Lists Then _tabPage.Controls.Add(label)

        _tabPage.Location = New Point(4, 4)
        _tabPage.Name = "CTab" + cnt.ToString()
        _tabPage.Size = New Size(380, 260)
        _tabPage.TabIndex = 2 + cnt
        _tabPage.Text = tabName
        _tabPage.UseVisualStyleBackColor = True

        _listCustom.AllowColumnReorder = True
        If Not _iconCol Then
            _listCustom.Columns.AddRange(New ColumnHeader() {_colHd1, _colHd2, _colHd3, _colHd4, _colHd5, _colHd6, _colHd7, _colHd8})
        Else
            _listCustom.Columns.AddRange(New ColumnHeader() {_colHd1, _colHd3})
        End If
        _listCustom.ContextMenuStrip = Me.ContextMenuOperate
        _listCustom.Dock = DockStyle.Fill
        _listCustom.FullRowSelect = True
        _listCustom.HideSelection = False
        _listCustom.Location = New Point(0, 0)
        _listCustom.Margin = New Padding(0)
        _listCustom.Name = "CList" + Environment.TickCount.ToString()
        _listCustom.ShowItemToolTips = True
        _listCustom.Size = New Size(380, 260)
        _listCustom.UseCompatibleStateImageBehavior = False
        _listCustom.View = View.Details
        _listCustom.OwnerDraw = True
        _listCustom.VirtualMode = True
        _listCustom.Font = _fntReaded
        _listCustom.BackColor = _clListBackcolor

        _listCustom.GridLines = SettingDialog.ShowGrid
        _listCustom.AllowDrop = True

        AddHandler _listCustom.SelectedIndexChanged, AddressOf MyList_SelectedIndexChanged
        AddHandler _listCustom.MouseDoubleClick, AddressOf MyList_MouseDoubleClick
        AddHandler _listCustom.ColumnClick, AddressOf MyList_ColumnClick
        AddHandler _listCustom.DrawColumnHeader, AddressOf MyList_DrawColumnHeader
        AddHandler _listCustom.DragDrop, AddressOf TweenMain_DragDrop
        AddHandler _listCustom.DragOver, AddressOf TweenMain_DragOver
        AddHandler _listCustom.DrawItem, AddressOf MyList_DrawItem
        AddHandler _listCustom.MouseClick, AddressOf MyList_MouseClick
        AddHandler _listCustom.ColumnReordered, AddressOf MyList_ColumnReordered
        AddHandler _listCustom.ColumnWidthChanged, AddressOf MyList_ColumnWidthChanged
        AddHandler _listCustom.CacheVirtualItems, AddressOf MyList_CacheVirtualItems
        AddHandler _listCustom.RetrieveVirtualItem, AddressOf MyList_RetrieveVirtualItem
        AddHandler _listCustom.DrawSubItem, AddressOf MyList_DrawSubItem
        AddHandler _listCustom.HScrolled, AddressOf MyList_HScrolled

        InitColumnText()
        _colHd1.Text = ColumnText(0)
        _colHd1.Width = 48
        _colHd2.Text = ColumnText(1)
        _colHd2.Width = 80
        _colHd3.Text = ColumnText(2)
        _colHd3.Width = 300
        _colHd4.Text = ColumnText(3)
        _colHd4.Width = 50
        _colHd5.Text = ColumnText(4)
        _colHd5.Width = 50
        _colHd6.Text = ColumnText(5)
        _colHd6.Width = 16
        _colHd7.Text = ColumnText(6)
        _colHd7.Width = 16
        _colHd8.Text = ColumnText(7)
        _colHd8.Width = 50

        If (_statuses.Tabs.ContainsKey(tabName) AndAlso _statuses.Tabs(tabName).TabType = TabUsageType.Mentions) _
           OrElse (Not _statuses.IsDefaultTab(tabName) AndAlso tabType <> TabUsageType.PublicSearch AndAlso tabType <> TabUsageType.Lists AndAlso tabType <> TabUsageType.Related) Then
            TabDialog.AddTab(tabName)
        End If

        _listCustom.SmallImageList = New ImageList()
        If _iconSz > 0 Then
            _listCustom.SmallImageList.ImageSize = New Size(_iconSz, _iconSz)
        End If

        Dim dispOrder(7) As Integer
        If Not startup Then
            For i As Integer = 0 To _curList.Columns.Count - 1
                For j As Integer = 0 To _curList.Columns.Count - 1
                    If _curList.Columns(j).DisplayIndex = i Then
                        dispOrder(i) = j
                        Exit For
                    End If
                Next
            Next
            For i As Integer = 0 To _curList.Columns.Count - 1
                _listCustom.Columns(i).Width = _curList.Columns(i).Width
                _listCustom.Columns(dispOrder(i)).DisplayIndex = i
            Next
        Else
            If _iconCol Then
                _listCustom.Columns(0).Width = _cfgLocal.Width1
                _listCustom.Columns(1).Width = _cfgLocal.Width3
                _listCustom.Columns(0).DisplayIndex = 0
                _listCustom.Columns(1).DisplayIndex = 1
            Else
                For i As Integer = 0 To 7
                    If _cfgLocal.DisplayIndex1 = i Then
                        dispOrder(i) = 0
                    ElseIf _cfgLocal.DisplayIndex2 = i Then
                        dispOrder(i) = 1
                    ElseIf _cfgLocal.DisplayIndex3 = i Then
                        dispOrder(i) = 2
                    ElseIf _cfgLocal.DisplayIndex4 = i Then
                        dispOrder(i) = 3
                    ElseIf _cfgLocal.DisplayIndex5 = i Then
                        dispOrder(i) = 4
                    ElseIf _cfgLocal.DisplayIndex6 = i Then
                        dispOrder(i) = 5
                    ElseIf _cfgLocal.DisplayIndex7 = i Then
                        dispOrder(i) = 6
                    ElseIf _cfgLocal.DisplayIndex8 = i Then
                        dispOrder(i) = 7
                    End If
                Next
                _listCustom.Columns(0).Width = _cfgLocal.Width1
                _listCustom.Columns(1).Width = _cfgLocal.Width2
                _listCustom.Columns(2).Width = _cfgLocal.Width3
                _listCustom.Columns(3).Width = _cfgLocal.Width4
                _listCustom.Columns(4).Width = _cfgLocal.Width5
                _listCustom.Columns(5).Width = _cfgLocal.Width6
                _listCustom.Columns(6).Width = _cfgLocal.Width7
                _listCustom.Columns(7).Width = _cfgLocal.Width8
                For i As Integer = 0 To 7
                    _listCustom.Columns(dispOrder(i)).DisplayIndex = i
                Next
            End If
        End If

        If tabType = TabUsageType.PublicSearch Then pnl.ResumeLayout(False)

        _tabPage.ResumeLayout(False)

        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ListTab.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()
        _tabPage.Tag = _listCustom
        Return True
    End Function

    Public Function RemoveSpecifiedTab(ByVal TabName As String, ByVal confirm As Boolean) As Boolean
        Dim idx As Integer = 0
        For idx = 0 To ListTab.TabPages.Count - 1
            If ListTab.TabPages(idx).Text = TabName Then Exit For
        Next

        If _statuses.IsDefaultTab(TabName) Then Return False

        If confirm Then
            Dim tmp As String = String.Format(My.Resources.RemoveSpecifiedTabText1, Environment.NewLine)
            If MessageBox.Show(tmp, TabName + " " + My.Resources.RemoveSpecifiedTabText2, _
                             MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Cancel Then
                Return False
            End If
        End If

        SetListProperty()   '他のタブに列幅等を反映

        Dim tabType As TabUsageType = _statuses.Tabs(TabName).TabType

        'オブジェクトインスタンスの削除
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ListTab.SuspendLayout()
        Me.SuspendLayout()

        Dim _tabPage As TabPage = ListTab.TabPages(idx)
        Dim _listCustom As DetailsListView = DirectCast(_tabPage.Tag, DetailsListView)
        _tabPage.Tag = Nothing

        _tabPage.SuspendLayout()

        If Me.ListTab.SelectedTab Is Me.ListTab.TabPages(idx) Then
            Me.ListTab.SelectTab(If(Me._beforeSelectedTab IsNot Nothing AndAlso Me.ListTab.TabPages.Contains(Me._beforeSelectedTab), Me._beforeSelectedTab, Me.ListTab.TabPages(0)))
        End If
        Me.ListTab.Controls.Remove(_tabPage)

        Dim pnl As Control = Nothing
        If tabType = TabUsageType.PublicSearch Then
            pnl = _tabPage.Controls("panelSearch")
            For Each ctrl As Control In pnl.Controls
                If ctrl.Name = "buttonSearch" Then
                    RemoveHandler ctrl.Click, AddressOf SearchButton_Click
                End If
                RemoveHandler ctrl.Enter, AddressOf SearchControls_Enter
                RemoveHandler ctrl.Leave, AddressOf SearchControls_Leave
                pnl.Controls.Remove(ctrl)
                ctrl.Dispose()
            Next
            _tabPage.Controls.Remove(pnl)
        End If

        _tabPage.Controls.Remove(_listCustom)
        _listCustom.Columns.Clear()
        _listCustom.ContextMenuStrip = Nothing

        RemoveHandler _listCustom.SelectedIndexChanged, AddressOf MyList_SelectedIndexChanged
        RemoveHandler _listCustom.MouseDoubleClick, AddressOf MyList_MouseDoubleClick
        RemoveHandler _listCustom.ColumnClick, AddressOf MyList_ColumnClick
        RemoveHandler _listCustom.DrawColumnHeader, AddressOf MyList_DrawColumnHeader
        RemoveHandler _listCustom.DragDrop, AddressOf TweenMain_DragDrop
        RemoveHandler _listCustom.DragOver, AddressOf TweenMain_DragOver
        RemoveHandler _listCustom.DrawItem, AddressOf MyList_DrawItem
        RemoveHandler _listCustom.MouseClick, AddressOf MyList_MouseClick
        RemoveHandler _listCustom.ColumnReordered, AddressOf MyList_ColumnReordered
        RemoveHandler _listCustom.ColumnWidthChanged, AddressOf MyList_ColumnWidthChanged
        RemoveHandler _listCustom.CacheVirtualItems, AddressOf MyList_CacheVirtualItems
        RemoveHandler _listCustom.RetrieveVirtualItem, AddressOf MyList_RetrieveVirtualItem
        RemoveHandler _listCustom.DrawSubItem, AddressOf MyList_DrawSubItem
        RemoveHandler _listCustom.HScrolled, AddressOf MyList_HScrolled

        TabDialog.RemoveTab(TabName)

        _listCustom.SmallImageList = Nothing
        _listCustom.ListViewItemSorter = Nothing

        'キャッシュのクリア
        If _curTab.Equals(_tabPage) Then
            _curTab = Nothing
            _curItemIndex = -1
            _curList = Nothing
            _curPost = Nothing
        End If
        _itemCache = Nothing
        _itemCacheIndex = -1
        _postCache = Nothing

        _tabPage.ResumeLayout(False)

        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ListTab.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

        _tabPage.Dispose()
        _listCustom.Dispose()
        _statuses.RemoveTab(TabName)

        For Each tp As TabPage In ListTab.TabPages
            Dim lst As DetailsListView = DirectCast(tp.Tag, DetailsListView)
            If lst.VirtualListSize <> _statuses.Tabs(tp.Text).AllCount Then
                lst.VirtualListSize = _statuses.Tabs(tp.Text).AllCount
            End If
        Next

        Return True
    End Function

    Private Sub ListTab_Deselected(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles ListTab.Deselected
        _itemCache = Nothing
        _itemCacheIndex = -1
        _postCache = Nothing
        _beforeSelectedTab = e.TabPage
    End Sub

    Private Sub ListTab_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListTab.MouseMove
        'タブのD&D
        Dim cpos As New Point(e.X, e.Y)

        If e.Button = Windows.Forms.MouseButtons.Left AndAlso _tabDrag Then
            Dim tn As String = ""
            Dim dragEnableRectangle As New Rectangle(CInt(_tabMouseDownPoint.X - (SystemInformation.DragSize.Width / 2)), CInt(_tabMouseDownPoint.Y - (SystemInformation.DragSize.Height / 2)), SystemInformation.DragSize.Width, SystemInformation.DragSize.Height)
            If Not dragEnableRectangle.Contains(e.Location) Then
                'タブが多段の場合にはMouseDownの前の段階で選択されたタブの段が変わっているので、このタイミングでカーソルの位置からタブを判定出来ない。
                tn = ListTab.SelectedTab.Text
            End If

            If tn = "" Then Exit Sub

            For Each tb As TabPage In ListTab.TabPages
                If tb.Text = tn Then
                    ListTab.DoDragDrop(tb, DragDropEffects.All)
                    Exit For
                End If
            Next
        Else
            _tabDrag = False
        End If

        For i As Integer = 0 To ListTab.TabPages.Count - 1
            Dim rect As Rectangle = ListTab.GetTabRect(i)
            If rect.Left <= cpos.X And cpos.X <= rect.Right And _
               rect.Top <= cpos.Y And cpos.Y <= rect.Bottom Then
                _rclickTabName = ListTab.TabPages(i).Text
                Exit For
            End If
        Next
    End Sub

    Private Sub ListTab_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListTab.SelectedIndexChanged
        '_curList.Refresh()
        DispSelectedPost()
        SetMainWindowTitle()
        SetStatusLabelUrl()
        If ListTab.Focused OrElse DirectCast(ListTab.SelectedTab.Tag, Control).Focused Then Me.Tag = ListTab.Tag
        TabMenuControl(ListTab.SelectedTab.Text)
    End Sub

    Private Sub SetListProperty()
        '削除などで見つからない場合は処理せず
        If _curList Is Nothing Then Exit Sub
        If Not _isColumnChanged Then Exit Sub

        Dim dispOrder(_curList.Columns.Count - 1) As Integer
        For i As Integer = 0 To _curList.Columns.Count - 1
            For j As Integer = 0 To _curList.Columns.Count - 1
                If _curList.Columns(j).DisplayIndex = i Then
                    dispOrder(i) = j
                    Exit For
                End If
            Next
        Next

        '列幅、列並びを他のタブに設定
        For Each tb As TabPage In ListTab.TabPages
            If Not tb.Equals(_curTab) Then
                If tb.Tag IsNot Nothing AndAlso tb.Controls.Count > 0 Then
                    Dim lst As DetailsListView = DirectCast(tb.Tag, DetailsListView)
                    For i As Integer = 0 To lst.Columns.Count - 1
                        lst.Columns(dispOrder(i)).DisplayIndex = i
                        lst.Columns(i).Width = _curList.Columns(i).Width
                    Next
                End If
            End If
        Next

        _isColumnChanged = False
    End Sub

    Private Sub PostBrowser_StatusTextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles PostBrowser.StatusTextChanged
        If PostBrowser.StatusText.StartsWith("http") OrElse PostBrowser.StatusText.StartsWith("ftp") _
                OrElse PostBrowser.StatusText.StartsWith("data") Then
            StatusLabelUrl.Text = PostBrowser.StatusText.Replace("&", "&&")
        End If
        If PostBrowser.StatusText = "" Then
            SetStatusLabelUrl()
        End If
    End Sub

    Private Sub StatusText_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles StatusText.KeyPress
        If e.KeyChar = "@" Then
            If Not SettingDialog.UseAtIdSupplement Then Exit Sub
            '@マーク
            ShowSuplDialog(StatusText, AtIdSupl)
            e.Handled = True
        ElseIf e.KeyChar = "#" Then
            If Not SettingDialog.UseHashSupplement Then Exit Sub
            ShowSuplDialog(StatusText, HashSupl)
            e.Handled = True
        End If
    End Sub

    Public Overloads Sub ShowSuplDialog(ByVal owner As TextBox, ByVal dialog As AtIdSupplement)
        ShowSuplDialog(owner, dialog, 0, "")
    End Sub

    Public Overloads Sub ShowSuplDialog(ByVal owner As TextBox, ByVal dialog As AtIdSupplement, ByVal offset As Integer)
        ShowSuplDialog(owner, dialog, offset, "")
    End Sub

    Public Overloads Sub ShowSuplDialog(ByVal owner As TextBox, ByVal dialog As AtIdSupplement, ByVal offset As Integer, ByVal startswith As String)
        dialog.StartsWith = startswith
        If dialog.Visible Then
            dialog.Focus()
        Else
            dialog.ShowDialog()
        End If
        Me.TopMost = SettingDialog.AlwaysTop
        Dim selStart As Integer = owner.SelectionStart
        Dim fHalf As String = ""
        Dim eHalf As String = ""
        If dialog.DialogResult = Windows.Forms.DialogResult.OK Then
            If dialog.inputText <> "" Then
                If selStart > 0 Then
                    fHalf = owner.Text.Substring(0, selStart - offset)
                End If
                If selStart < owner.Text.Length Then
                    eHalf = owner.Text.Substring(selStart)
                End If
                owner.Text = fHalf + dialog.inputText + eHalf
                owner.SelectionStart = selStart + dialog.inputText.Length
            End If
        Else
            If selStart > 0 Then
                fHalf = owner.Text.Substring(0, selStart)
            End If
            If selStart < owner.Text.Length Then
                eHalf = owner.Text.Substring(selStart)
            End If
            owner.Text = fHalf + eHalf
            If selStart > 0 Then
                owner.SelectionStart = selStart
            End If
        End If
        owner.Focus()
    End Sub

    Private Sub StatusText_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles StatusText.KeyUp
        'スペースキーで未読ジャンプ
        If Not e.Alt AndAlso Not e.Control AndAlso Not e.Shift Then
            If e.KeyCode = Keys.Space OrElse e.KeyCode = Keys.ProcessKey Then
                Dim isSpace As Boolean = False
                For Each c As Char In StatusText.Text.ToCharArray
                    If c = " " OrElse c = "　" Then
                        isSpace = True
                    Else
                        isSpace = False
                        Exit For
                    End If
                Next
                If isSpace Then
                    e.Handled = True
                    StatusText.Text = ""
                    JumpUnreadMenuItem_Click(Nothing, Nothing)
                End If
            End If
        End If
        Me.StatusText_TextChanged(Nothing, Nothing)
    End Sub

    Private Sub StatusText_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusText.TextChanged
        '文字数カウント
        Dim pLen As Integer = GetRestStatusCount(True, False)
        lblLen.Text = pLen.ToString()
        If pLen < 0 Then
            StatusText.ForeColor = Color.Red
        Else
            StatusText.ForeColor = _clInputFont
        End If
        If StatusText.Text = "" Then
            _reply_to_id = 0
            _reply_to_name = ""
        End If
    End Sub

    Private Function GetRestStatusCount(ByVal isAuto As Boolean, ByVal isAddFooter As Boolean) As Integer
        '文字数カウント
        Dim pLen As Integer = 140 - StatusText.Text.Length
        If (isAuto AndAlso Not My.Computer.Keyboard.CtrlKeyDown AndAlso SettingDialog.PostShiftEnter) OrElse _
           (isAuto AndAlso Not My.Computer.Keyboard.ShiftKeyDown AndAlso Not SettingDialog.PostShiftEnter) OrElse _
           (Not isAuto AndAlso isAddFooter) Then
            If SettingDialog.UseRecommendStatus Then
                pLen -= SettingDialog.RecommendStatusText.Length
            ElseIf SettingDialog.Status.Length > 0 Then
                pLen -= SettingDialog.Status.Length + 1
            End If
        End If
        If HashMgr.UseHash <> "" Then
            pLen -= HashMgr.UseHash.Length + 1
        End If
        Return pLen
    End Function

    Private Sub MyList_CacheVirtualItems(ByVal sender As System.Object, ByVal e As System.Windows.Forms.CacheVirtualItemsEventArgs)
        If _itemCache IsNot Nothing AndAlso _
           e.StartIndex >= _itemCacheIndex AndAlso _
           e.EndIndex < _itemCacheIndex + _itemCache.Length AndAlso _
           _curList.Equals(sender) Then
            'If the newly requested cache is a subset of the old cache, 
            'no need to rebuild everything, so do nothing.
            Return
        End If

        'Now we need to rebuild the cache.
        If _curList.Equals(sender) Then CreateCache(e.StartIndex, e.EndIndex)
    End Sub

    Private Sub MyList_RetrieveVirtualItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.RetrieveVirtualItemEventArgs)
        If _itemCache IsNot Nothing AndAlso e.ItemIndex >= _itemCacheIndex AndAlso e.ItemIndex < _itemCacheIndex + _itemCache.Length AndAlso _curList.Equals(sender) Then
            'A cache hit, so get the ListViewItem from the cache instead of making a new one.
            e.Item = _itemCache(e.ItemIndex - _itemCacheIndex)
        Else
            'A cache miss, so create a new ListViewItem and pass it back.
            Dim tb As TabPage = DirectCast(DirectCast(sender, Tween.TweenCustomControl.DetailsListView).Parent, TabPage)
            Try
                e.Item = CreateItem(tb, _
                                    _statuses.Item(tb.Text, e.ItemIndex), _
                                    e.ItemIndex)
            Catch ex As Exception
                '不正な要求に対する間に合わせの応答
                Dim sitem() As String = {"", "", "", "", "", "", "", ""}
                e.Item = New ImageListViewItem(sitem, "")
            End Try
        End If
    End Sub

    Private Sub CreateCache(ByVal StartIndex As Integer, ByVal EndIndex As Integer)
        Try
            'キャッシュ要求（要求範囲±30を作成）
            StartIndex -= 30
            If StartIndex < 0 Then StartIndex = 0
            EndIndex += 30
            If EndIndex >= _statuses.Tabs(_curTab.Text).AllCount Then EndIndex = _statuses.Tabs(_curTab.Text).AllCount - 1
            _postCache = _statuses.Item(_curTab.Text, StartIndex, EndIndex) '配列で取得
            _itemCacheIndex = StartIndex

            _itemCache = New ListViewItem(_postCache.Length - 1) {}
            For i As Integer = 0 To _postCache.Length - 1
                _itemCache(i) = CreateItem(_curTab, _postCache(i), StartIndex + i)
            Next i
        Catch ex As Exception
            'キャッシュ要求が実データとずれるため（イベントの遅延？）
            _postCache = Nothing
            _itemCache = Nothing
        End Try
    End Sub

    Private Function CreateItem(ByVal Tab As TabPage, ByVal Post As PostClass, ByVal Index As Integer) As ListViewItem
        Dim mk As New StringBuilder
        If Post.IsDeleted Then mk.Append("×")
        If Post.IsMark Then mk.Append("♪")
        If Post.IsProtect Then mk.Append("Ю")
        If Post.InReplyToStatusId > 0 Then mk.Append("⇒")
        If Post.FavoritedCount > 0 Then mk.Append("+" + Post.FavoritedCount.ToString)
        Dim itm As ImageListViewItem
        If Post.RetweetedId = 0 Then
            Dim sitem() As String = {"",
                                     Post.Nickname,
                                     If(Post.IsDeleted, "(DELETED)", Post.TextFromApi),
                                     Post.CreatedAt.ToString(SettingDialog.DateTimeFormat),
                                     Post.ScreenName,
                                     "",
                                     mk.ToString(),
                                     Post.Source}
            itm = New ImageListViewItem(sitem, DirectCast(Me.TIconDic, ImageDictionary), Post.ImageUrl)
        Else
            Dim sitem() As String = {"",
                                     Post.Nickname,
                                     If(Post.IsDeleted, "(DELETED)", Post.TextFromApi),
                                     Post.CreatedAt.ToString(SettingDialog.DateTimeFormat),
                                     Post.ScreenName + Environment.NewLine + "(RT:" + Post.RetweetedBy + ")",
                                     "",
                                     mk.ToString(),
                                     Post.Source}
            itm = New ImageListViewItem(sitem, DirectCast(Me.TIconDic, ImageDictionary), Post.ImageUrl)
        End If

        Dim read As Boolean = Post.IsRead
        '未読管理していなかったら既読として扱う
        If Not _statuses.Tabs(Tab.Text).UnreadManage OrElse _
           Not SettingDialog.UnreadManage Then read = True
        ChangeItemStyleRead(read, itm, Post, Nothing)
        If Tab.Equals(_curTab) Then ColorizeList(itm, Index)
        Return itm
    End Function

    Private Sub MyList_DrawColumnHeader(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawListViewColumnHeaderEventArgs)
        e.DrawDefault = True
    End Sub

    Private Sub MyList_HScrolled(ByVal sender As Object, ByVal e As EventArgs)
        Dim listView As DetailsListView = DirectCast(sender, DetailsListView)
        listView.Refresh()
    End Sub

    Private Sub MyList_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawListViewItemEventArgs)
        If e.State = 0 Then Exit Sub
        e.DrawDefault = False
        If Not e.Item.Selected Then     'e.ItemStateでうまく判定できない？？？
            Dim brs2 As SolidBrush = Nothing
            Select Case e.Item.BackColor
                Case _clSelf
                    brs2 = _brsBackColorMine
                Case _clAtSelf
                    brs2 = _brsBackColorAt
                Case _clTarget
                    brs2 = _brsBackColorYou
                Case _clAtTarget
                    brs2 = _brsBackColorAtYou
                Case _clAtFromTarget
                    brs2 = _brsBackColorAtFromTarget
                Case _clAtTo
                    brs2 = _brsBackColorAtTo
                Case Else
                    brs2 = _brsBackColorNone
            End Select
            e.Graphics.FillRectangle(brs2, e.Bounds)
        Else
            '選択中の行
            If DirectCast(sender, Windows.Forms.Control).Focused Then
                e.Graphics.FillRectangle(_brsHighLight, e.Bounds)
            Else
                e.Graphics.FillRectangle(_brsDeactiveSelection, e.Bounds)
            End If
        End If
        If (e.State And ListViewItemStates.Focused) = ListViewItemStates.Focused Then e.DrawFocusRectangle()
        Me.DrawListViewItemIcon(e)
    End Sub

    Private Sub MyList_DrawSubItem(ByVal sender As Object, ByVal e As DrawListViewSubItemEventArgs)
        If e.ItemState = 0 Then Exit Sub

        If e.ColumnIndex > 0 Then
            'アイコン以外の列
            Dim rct As RectangleF = e.Bounds
            Dim rctB As RectangleF = e.Bounds
            rct.Width = e.Header.Width
            rctB.Width = e.Header.Width
            If _iconCol Then
                rct.Y += e.Item.Font.Height
                rct.Height -= e.Item.Font.Height
                rctB.Height = e.Item.Font.Height
            End If


            Dim heightDiff As Integer
            Dim drawLineCount As Integer = Math.Max(1, Math.DivRem(CType(rct.Height, Integer), e.Item.Font.Height, heightDiff))

            'If heightDiff > e.Item.Font.Height * 0.7 Then
            '    rct.Height += e.Item.Font.Height
            '    drawLineCount += 1
            'End If

            'フォントの高さの半分を足してるのは保険。無くてもいいかも。
            If Not _iconCol AndAlso drawLineCount <= 1 Then
                'rct.Inflate(0, CType(heightDiff / -2, Integer))
                'rct.Height += CType(e.Item.Font.Height / 2, Integer)
            ElseIf heightDiff < e.Item.Font.Height * 0.7 Then
                '最終行が70%以上欠けていたら、最終行は表示しない
                'rct.Height = CType((e.Item.Font.Height * drawLineCount) + (e.Item.Font.Height / 2), Single)
                rct.Height = CType((e.Item.Font.Height * drawLineCount), Single) - 1
            Else
                drawLineCount += 1
            End If

            'If Not _iconCol AndAlso drawLineCount > 1 Then
            '    rct.Y += CType(e.Item.Font.Height * 0.2, Single)
            '    If heightDiff >= e.Item.Font.Height * 0.8 Then rct.Height -= CType(e.Item.Font.Height * 0.2, Single)
            'End If
            If Not e.Item.Selected Then     'e.ItemStateでうまく判定できない？？？
                '選択されていない行
                '文字色
                Dim brs As SolidBrush = Nothing
                Dim flg As Boolean = False
                Select Case e.Item.ForeColor
                    Case _clUnread
                        brs = _brsForeColorUnread
                    Case _clReaded
                        brs = _brsForeColorReaded
                    Case _clFav
                        brs = _brsForeColorFav
                    Case _clOWL
                        brs = _brsForeColorOWL
                    Case _clRetweet
                        brs = _brsForeColorRetweet
                    Case Else
                        brs = New SolidBrush(e.Item.ForeColor)
                        flg = True
                End Select
                If rct.Width > 0 Then
                    If _iconCol Then
                        Dim fnt As New Font(e.Item.Font, FontStyle.Bold)
                        'e.Graphics.DrawString(System.Environment.NewLine + e.Item.SubItems(2).Text, e.Item.Font, brs, rct, sf)
                        'e.Graphics.DrawString(e.Item.SubItems(4).Text + " / " + e.Item.SubItems(1).Text + " (" + e.Item.SubItems(3).Text + ") " + e.Item.SubItems(5).Text + e.Item.SubItems(6).Text + " [" + e.Item.SubItems(7).Text + "]", fnt, brs, rctB, sf)
                        TextRenderer.DrawText(e.Graphics,
                                              e.Item.SubItems(2).Text,
                                              e.Item.Font,
                                              Rectangle.Round(rct),
                                              brs.Color,
                                              TextFormatFlags.WordBreak Or
                                              TextFormatFlags.EndEllipsis Or
                                              TextFormatFlags.GlyphOverhangPadding Or
                                              TextFormatFlags.NoPrefix)
                        TextRenderer.DrawText(e.Graphics,
                                              e.Item.SubItems(4).Text + " / " + e.Item.SubItems(1).Text + " (" + e.Item.SubItems(3).Text + ") " + e.Item.SubItems(5).Text + e.Item.SubItems(6).Text + " [" + e.Item.SubItems(7).Text + "]",
                                              fnt,
                                              Rectangle.Round(rctB),
                                              brs.Color,
                                              TextFormatFlags.SingleLine Or
                                              TextFormatFlags.EndEllipsis Or
                                              TextFormatFlags.GlyphOverhangPadding Or
                                              TextFormatFlags.NoPrefix)
                        fnt.Dispose()
                    ElseIf drawLineCount = 1 Then
                        TextRenderer.DrawText(e.Graphics,
                                              e.SubItem.Text,
                                              e.Item.Font,
                                              Rectangle.Round(rct),
                                              brs.Color,
                                              TextFormatFlags.SingleLine Or
                                              TextFormatFlags.EndEllipsis Or
                                              TextFormatFlags.GlyphOverhangPadding Or
                                              TextFormatFlags.NoPrefix Or
                                              TextFormatFlags.VerticalCenter)
                    Else
                        'e.Graphics.DrawString(e.SubItem.Text, e.Item.Font, brs, rct, sf)
                        TextRenderer.DrawText(e.Graphics,
                                              e.SubItem.Text,
                                              e.Item.Font,
                                              Rectangle.Round(rct),
                                              brs.Color,
                                              TextFormatFlags.WordBreak Or
                                              TextFormatFlags.EndEllipsis Or
                                              TextFormatFlags.GlyphOverhangPadding Or
                                              TextFormatFlags.NoPrefix)
                    End If
                End If
                If flg Then brs.Dispose()
            Else
                If rct.Width > 0 Then
                    '選択中の行
                    Dim fnt As New Font(e.Item.Font, FontStyle.Bold)
                    If DirectCast(sender, Windows.Forms.Control).Focused Then
                        If _iconCol Then
                            'e.Graphics.DrawString(System.Environment.NewLine + e.Item.SubItems(2).Text, e.Item.Font, _brsHighLightText, rct, sf)
                            'e.Graphics.DrawString(e.Item.SubItems(4).Text + " / " + e.Item.SubItems(1).Text + " (" + e.Item.SubItems(3).Text + ") " + e.Item.SubItems(5).Text + e.Item.SubItems(6).Text + " [" + e.Item.SubItems(7).Text + "]", fnt, _brsHighLightText, rctB, sf)
                            TextRenderer.DrawText(e.Graphics,
                                                  e.Item.SubItems(2).Text,
                                                  e.Item.Font,
                                                  Rectangle.Round(rct),
                                                  _brsHighLightText.Color,
                                                  TextFormatFlags.WordBreak Or
                                                  TextFormatFlags.EndEllipsis Or
                                                  TextFormatFlags.GlyphOverhangPadding Or
                                                  TextFormatFlags.NoPrefix)
                            TextRenderer.DrawText(e.Graphics,
                                                  e.Item.SubItems(4).Text + " / " + e.Item.SubItems(1).Text + " (" + e.Item.SubItems(3).Text + ") " + e.Item.SubItems(5).Text + e.Item.SubItems(6).Text + " [" + e.Item.SubItems(7).Text + "]",
                                                  fnt,
                                                  Rectangle.Round(rctB),
                                                  _brsHighLightText.Color,
                                                  TextFormatFlags.SingleLine Or
                                                  TextFormatFlags.EndEllipsis Or
                                                  TextFormatFlags.GlyphOverhangPadding Or
                                                  TextFormatFlags.NoPrefix)
                        ElseIf drawLineCount = 1 Then
                            TextRenderer.DrawText(e.Graphics,
                                                  e.SubItem.Text,
                                                  e.Item.Font,
                                                  Rectangle.Round(rct),
                                                  _brsHighLightText.Color,
                                                  TextFormatFlags.SingleLine Or
                                                  TextFormatFlags.EndEllipsis Or
                                                  TextFormatFlags.GlyphOverhangPadding Or
                                                  TextFormatFlags.NoPrefix Or
                                                  TextFormatFlags.VerticalCenter)
                        Else
                            'e.Graphics.DrawString(e.SubItem.Text, e.Item.Font, _brsHighLightText, rct, sf)
                            TextRenderer.DrawText(e.Graphics,
                                                  e.SubItem.Text,
                                                  e.Item.Font,
                                                  Rectangle.Round(rct),
                                                  _brsHighLightText.Color,
                                                  TextFormatFlags.WordBreak Or
                                                  TextFormatFlags.EndEllipsis Or
                                                  TextFormatFlags.GlyphOverhangPadding Or
                                                  TextFormatFlags.NoPrefix)
                        End If
                    Else
                        If _iconCol Then
                            'e.Graphics.DrawString(System.Environment.NewLine + e.Item.SubItems(2).Text, e.Item.Font, _brsForeColorUnread, rct, sf)
                            'e.Graphics.DrawString(e.Item.SubItems(4).Text + " / " + e.Item.SubItems(1).Text + " (" + e.Item.SubItems(3).Text + ") " + e.Item.SubItems(5).Text + e.Item.SubItems(6).Text + " [" + e.Item.SubItems(7).Text + "]", fnt, _brsForeColorUnread, rctB, sf)
                            TextRenderer.DrawText(e.Graphics,
                                                  e.Item.SubItems(2).Text,
                                                  e.Item.Font,
                                                  Rectangle.Round(rct),
                                                  _brsForeColorUnread.Color,
                                                  TextFormatFlags.WordBreak Or
                                                  TextFormatFlags.EndEllipsis Or
                                                  TextFormatFlags.GlyphOverhangPadding Or
                                                  TextFormatFlags.NoPrefix)
                            TextRenderer.DrawText(e.Graphics,
                                                  e.Item.SubItems(4).Text + " / " + e.Item.SubItems(1).Text + " (" + e.Item.SubItems(3).Text + ") " + e.Item.SubItems(5).Text + e.Item.SubItems(6).Text + " [" + e.Item.SubItems(7).Text + "]",
                                                  fnt,
                                                  Rectangle.Round(rctB),
                                                  _brsForeColorUnread.Color,
                                                  TextFormatFlags.SingleLine Or
                                                  TextFormatFlags.EndEllipsis Or
                                                  TextFormatFlags.GlyphOverhangPadding Or
                                                  TextFormatFlags.NoPrefix)
                        ElseIf drawLineCount = 1 Then
                            TextRenderer.DrawText(e.Graphics,
                                                  e.SubItem.Text,
                                                  e.Item.Font,
                                                  Rectangle.Round(rct),
                                                  _brsForeColorUnread.Color,
                                                  TextFormatFlags.SingleLine Or
                                                  TextFormatFlags.EndEllipsis Or
                                                  TextFormatFlags.GlyphOverhangPadding Or
                                                  TextFormatFlags.NoPrefix Or
                                                  TextFormatFlags.VerticalCenter)
                        Else
                            'e.Graphics.DrawString(e.SubItem.Text, e.Item.Font, _brsForeColorUnread, rct, sf)
                            TextRenderer.DrawText(e.Graphics,
                                                  e.SubItem.Text,
                                                  e.Item.Font,
                                                  Rectangle.Round(rct),
                                                  _brsForeColorUnread.Color,
                                                  TextFormatFlags.WordBreak Or
                                                  TextFormatFlags.EndEllipsis Or
                                                  TextFormatFlags.GlyphOverhangPadding Or
                                                  TextFormatFlags.NoPrefix)
                        End If
                    End If
                    fnt.Dispose()
                End If
            End If
        End If
    End Sub

    Private Sub DrawListViewItemIcon(ByVal e As DrawListViewItemEventArgs)
        Dim item As ImageListViewItem = DirectCast(e.Item, ImageListViewItem)
        If item.Image IsNot Nothing Then
            'e.Bounds.Leftが常に0を指すから自前で計算
            Dim itemRect As Rectangle = item.Bounds
            itemRect.Width = e.Item.ListView.Columns(0).Width

            For Each clm As ColumnHeader In e.Item.ListView.Columns
                If clm.DisplayIndex < e.Item.ListView.Columns(0).DisplayIndex Then
                    itemRect.X += clm.Width
                End If
            Next

            Dim iconRect As Rectangle = Rectangle.Intersect(New Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, New Size(_iconSz, _iconSz)), itemRect)
            iconRect.Offset(0, CType(Math.Max(0, (itemRect.Height - _iconSz) / 2), Integer))

            If iconRect.Width > 0 Then
                e.Graphics.FillRectangle(Brushes.White, iconRect)
                e.Graphics.InterpolationMode = Drawing2D.InterpolationMode.High
                Try
                    e.Graphics.DrawImage(item.Image, iconRect)
                Catch ex As ArgumentException
                    item.RegetImage()
                End Try
            End If
        End If
    End Sub

    Private Sub DoTabSearch(ByVal _word As String, _
                            ByVal CaseSensitive As Boolean, _
                            ByVal UseRegex As Boolean, _
                            ByVal SType As SEARCHTYPE)
        Dim cidx As Integer = 0
        Dim fnd As Boolean = False
        Dim toIdx As Integer
        Dim stp As Integer = 1

        If _curList.VirtualListSize = 0 Then
            MessageBox.Show(My.Resources.DoTabSearchText2, My.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        If _curList.SelectedIndices.Count > 0 Then
            cidx = _curList.SelectedIndices(0)
        End If
        toIdx = _curList.VirtualListSize - 1

        Select Case SType
            Case SEARCHTYPE.DialogSearch    'ダイアログからの検索
                If _curList.SelectedIndices.Count > 0 Then
                    cidx = _curList.SelectedIndices(0)
                Else
                    cidx = 0
                End If
            Case SEARCHTYPE.NextSearch      '次を検索
                If _curList.SelectedIndices.Count > 0 Then
                    cidx = _curList.SelectedIndices(0) + 1
                    If cidx > toIdx Then cidx = toIdx
                Else
                    cidx = 0
                End If
            Case SEARCHTYPE.PrevSearch      '前を検索
                If _curList.SelectedIndices.Count > 0 Then
                    cidx = _curList.SelectedIndices(0) - 1
                    If cidx < 0 Then cidx = 0
                Else
                    cidx = toIdx
                End If
                toIdx = 0
                stp = -1
        End Select

        Dim regOpt As RegexOptions = RegexOptions.None
        Dim fndOpt As StringComparison = StringComparison.Ordinal
        If Not CaseSensitive Then
            regOpt = RegexOptions.IgnoreCase
            fndOpt = StringComparison.OrdinalIgnoreCase
        End If
        Try
RETRY:
            If UseRegex Then
                ' 正規表現検索
                Dim _search As Regex
                Try
                    _search = New Regex(_word)
                    For idx As Integer = cidx To toIdx Step stp
                        Dim post As PostClass = _statuses.Item(_curTab.Text, idx)
                        If _search.IsMatch(post.Nickname, regOpt) _
                            OrElse _search.IsMatch(post.TextFromApi, regOpt) _
                            OrElse _search.IsMatch(post.ScreenName, regOpt) _
                        Then
                            SelectListItem(_curList, idx)
                            _curList.EnsureVisible(idx)
                            Exit Sub
                        End If
                    Next
                Catch ex As ArgumentException
                    MsgBox(My.Resources.DoTabSearchText1, MsgBoxStyle.Critical)
                    Exit Sub
                End Try
            Else
                ' 通常検索
                For idx As Integer = cidx To toIdx Step stp
                    Dim post As PostClass = _statuses.Item(_curTab.Text, idx)
                    If post.Nickname.IndexOf(_word, fndOpt) > -1 _
                        OrElse post.TextFromApi.IndexOf(_word, fndOpt) > -1 _
                        OrElse post.ScreenName.IndexOf(_word, fndOpt) > -1 _
                    Then
                        SelectListItem(_curList, idx)
                        _curList.EnsureVisible(idx)
                        Exit Sub
                    End If
                Next
            End If

            If Not fnd Then
                Select Case SType
                    Case SEARCHTYPE.DialogSearch, SEARCHTYPE.NextSearch
                        toIdx = cidx
                        cidx = 0
                    Case SEARCHTYPE.PrevSearch
                        toIdx = cidx
                        cidx = _curList.Items.Count - 1
                End Select
                fnd = True
                GoTo RETRY
            End If
        Catch ex As ArgumentOutOfRangeException

        End Try
        MessageBox.Show(My.Resources.DoTabSearchText2, My.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub MenuItemSubSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItemSubSearch.Click
        '検索メニュー
        SearchDialog.Owner = Me
        If SearchDialog.ShowDialog() = Windows.Forms.DialogResult.Cancel Then
            Me.TopMost = SettingDialog.AlwaysTop
            Exit Sub
        End If
        Me.TopMost = SettingDialog.AlwaysTop

        If SearchDialog.SWord <> "" Then
            DoTabSearch(SearchDialog.SWord, _
                        SearchDialog.CheckCaseSensitive, _
                        SearchDialog.CheckRegex, _
                        SEARCHTYPE.DialogSearch)
        End If
    End Sub

    Private Sub MenuItemSearchNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItemSearchNext.Click
        '次を検索
        If SearchDialog.SWord = "" Then
            If SearchDialog.ShowDialog() = Windows.Forms.DialogResult.Cancel Then
                Me.TopMost = SettingDialog.AlwaysTop
                Exit Sub
            End If
            Me.TopMost = SettingDialog.AlwaysTop
            If SearchDialog.SWord = "" Then Exit Sub

            DoTabSearch(SearchDialog.SWord, _
                        SearchDialog.CheckCaseSensitive, _
                        SearchDialog.CheckRegex, _
                        SEARCHTYPE.DialogSearch)
        Else
            DoTabSearch(SearchDialog.SWord, _
                        SearchDialog.CheckCaseSensitive, _
                        SearchDialog.CheckRegex, _
                        SEARCHTYPE.NextSearch)
        End If
    End Sub

    Private Sub MenuItemSearchPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItemSearchPrev.Click
        '前を検索
        If SearchDialog.SWord = "" Then
            If SearchDialog.ShowDialog() = Windows.Forms.DialogResult.Cancel Then
                Me.TopMost = SettingDialog.AlwaysTop
                Exit Sub
            End If
            Me.TopMost = SettingDialog.AlwaysTop
            If SearchDialog.SWord = "" Then Exit Sub
        End If

        DoTabSearch(SearchDialog.SWord, _
                    SearchDialog.CheckCaseSensitive, _
                    SearchDialog.CheckRegex, _
                    SEARCHTYPE.PrevSearch)
    End Sub

    Private Sub AboutMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutMenuItem.Click
        TweenAboutBox.ShowDialog()
        Me.TopMost = SettingDialog.AlwaysTop
    End Sub

    Private Sub JumpUnreadMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles JumpUnreadMenuItem.Click, JumpReadOpMenuItem.Click
        Dim bgnIdx As Integer = ListTab.TabPages.IndexOf(_curTab)
        Dim idx As Integer = -1
        Dim lst As DetailsListView = Nothing

        If ImageSelectionPanel.Enabled Then
            Exit Sub
        End If

        '現在タブから最終タブまで探索
        For i As Integer = bgnIdx To ListTab.TabPages.Count - 1
            '未読Index取得
            idx = _statuses.GetOldestUnreadIndex(ListTab.TabPages(i).Text)
            If idx > -1 Then
                ListTab.SelectedIndex = i
                lst = DirectCast(ListTab.TabPages(i).Tag, DetailsListView)
                '_curTab = ListTab.TabPages(i)
                Exit For
            End If
        Next

        '未読みつからず＆現在タブが先頭ではなかったら、先頭タブから現在タブの手前まで探索
        If idx = -1 AndAlso bgnIdx > 0 Then
            For i As Integer = 0 To bgnIdx - 1
                idx = _statuses.GetOldestUnreadIndex(ListTab.TabPages(i).Text)
                If idx > -1 Then
                    ListTab.SelectedIndex = i
                    lst = DirectCast(ListTab.TabPages(i).Tag, DetailsListView)
                    '_curTab = ListTab.TabPages(i)
                    Exit For
                End If
            Next
        End If

        '全部調べたが未読見つからず→先頭タブの最新発言へ
        If idx = -1 Then
            ListTab.SelectedIndex = 0
            lst = DirectCast(ListTab.TabPages(0).Tag, DetailsListView)
            '_curTab = ListTab.TabPages(0)
            If _statuses.SortOrder = SortOrder.Ascending Then
                idx = lst.VirtualListSize - 1
            Else
                idx = 0
            End If
        End If

        If lst.VirtualListSize > 0 AndAlso idx > -1 AndAlso lst.VirtualListSize > idx Then
            SelectListItem(lst, idx)
            If _statuses.SortMode = IdComparerClass.ComparerMode.Id Then
                If _statuses.SortOrder = SortOrder.Ascending AndAlso lst.Items(idx).Position.Y > lst.ClientSize.Height - _iconSz - 10 OrElse _
                   _statuses.SortOrder = SortOrder.Descending AndAlso lst.Items(idx).Position.Y < _iconSz + 10 Then
                    MoveTop()
                Else
                    lst.EnsureVisible(idx)
                End If
            Else
                lst.EnsureVisible(idx)
            End If
        End If
        lst.Focus()
    End Sub

    Private Sub StatusOpenMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusOpenMenuItem.Click, OpenStatusOpMenuItem.Click
        If _curList.SelectedIndices.Count > 0 AndAlso _statuses.Tabs(_curTab.Text).TabType <> TabUsageType.DirectMessage Then
            Dim post As PostClass = _statuses.Item(_curTab.Text, _curList.SelectedIndices(0))
            If post.RetweetedId = 0 Then
                OpenUriAsync("http://twitter.com/" + post.ScreenName + "/status/" + post.StatusId.ToString)
            Else
                OpenUriAsync("http://twitter.com/" + post.ScreenName + "/status/" + post.RetweetedId.ToString)
            End If
        End If
    End Sub

    Private Sub FavorareMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles FavorareMenuItem.Click, OpenFavotterOpMenuItem.Click
        If _curList.SelectedIndices.Count > 0 Then
            Dim post As PostClass = _statuses.Item(_curTab.Text, _curList.SelectedIndices(0))
            OpenUriAsync(My.Resources.FavstarUrl + "users/" + post.ScreenName + "/recent")
        End If
    End Sub

    Private Sub VerUpMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VerUpMenuItem.Click
        CheckNewVersion()
    End Sub

    Private Sub RunTweenUp()

        Dim pinfo As New ProcessStartInfo
        pinfo.UseShellExecute = True
        pinfo.WorkingDirectory = Application.StartupPath
        pinfo.FileName = Path.Combine(Application.StartupPath(), "TweenUp2.exe")
        Try
            Process.Start(pinfo)
        Catch ex As Exception
            MessageBox.Show("Failed to execute TweenUp2.exe.")
        End Try
    End Sub

    Private Sub CheckNewVersion(Optional ByVal startup As Boolean = False)
        Dim retMsg As String = ""
        Dim strVer As String = ""
        Dim strDetail As String = ""
        Dim forceUpdate As Boolean = My.Computer.Keyboard.ShiftKeyDown

        Try
            retMsg = tw.GetVersionInfo()
        Catch ex As Exception
            StatusLabel.Text = My.Resources.CheckNewVersionText9
            If Not startup Then MessageBox.Show(My.Resources.CheckNewVersionText10, My.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
            Exit Sub
        End Try
        If retMsg.Length > 0 Then
            strVer = retMsg.Substring(0, 4)
            If retMsg.Length > 4 Then
                strDetail = retMsg.Substring(5).Trim
            End If
            If fileVersion <> "" AndAlso strVer.CompareTo(fileVersion.Replace(".", "")) > 0 Then
                Dim tmp As String = String.Format(My.Resources.CheckNewVersionText3, strVer)
                Using dialogAsShieldicon As New DialogAsShieldIcon
                    If dialogAsShieldicon.Show(tmp, strDetail, My.Resources.CheckNewVersionText1, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                        retMsg = tw.GetTweenBinary(strVer)
                        If retMsg.Length = 0 Then
                            RunTweenUp()
                            _endingFlag = True
                            dialogAsShieldicon.Dispose()
                            Me.Close()
                            Exit Sub
                        Else
                            If Not startup Then MessageBox.Show(My.Resources.CheckNewVersionText5 + System.Environment.NewLine + retMsg, My.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        End If
                    End If
                    dialogAsShieldicon.Dispose()
                End Using
            Else
                If forceUpdate Then
                    Dim tmp As String = String.Format(My.Resources.CheckNewVersionText6, strVer)
                    Using dialogAsShieldicon As New DialogAsShieldIcon
                        If dialogAsShieldicon.Show(tmp, strDetail, My.Resources.CheckNewVersionText1, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                            retMsg = tw.GetTweenBinary(strVer)
                            If retMsg.Length = 0 Then
                                RunTweenUp()
                                _endingFlag = True
                                dialogAsShieldicon.Dispose()
                                Me.Close()
                                Exit Sub
                            Else
                                If Not startup Then MessageBox.Show(My.Resources.CheckNewVersionText5 + System.Environment.NewLine + retMsg, My.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            End If
                        End If
                        dialogAsShieldicon.Dispose()
                    End Using
                ElseIf Not startup Then
                    MessageBox.Show(My.Resources.CheckNewVersionText7 + fileVersion.Replace(".", "") + My.Resources.CheckNewVersionText8 + strVer, My.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Else
            StatusLabel.Text = My.Resources.CheckNewVersionText9
            If Not startup Then MessageBox.Show(My.Resources.CheckNewVersionText10, My.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub

    Private Sub Colorize()
        _colorize = False
        DispSelectedPost()
        '件数関連の場合、タイトル即時書き換え
        If SettingDialog.DispLatestPost <> DispTitleEnum.None AndAlso _
           SettingDialog.DispLatestPost <> DispTitleEnum.Post AndAlso _
           SettingDialog.DispLatestPost <> DispTitleEnum.Ver AndAlso _
           SettingDialog.DispLatestPost <> DispTitleEnum.OwnStatus Then
            SetMainWindowTitle()
        End If
        If Not StatusLabelUrl.Text.StartsWith("http") Then SetStatusLabelUrl()
        For Each tb As TabPage In ListTab.TabPages
            If _statuses.Tabs(tb.Text).UnreadCount = 0 Then
                If SettingDialog.TabIconDisp Then
                    If tb.ImageIndex = 0 Then tb.ImageIndex = -1
                End If
            End If
        Next
        If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
    End Sub

    Public Function createDetailHtml(ByVal orgdata As String) As String
        Return detailHtmlFormatHeader + orgdata + detailHtmlFormatFooter
    End Function

    Private Sub DisplayItemImage_Downloaded(ByVal sender As Object, ByVal e As EventArgs) Handles displayItem.ImageDownloaded
        If sender.Equals(displayItem) AndAlso displayItem.Image IsNot Nothing Then UserPicture.Image = displayItem.Image
    End Sub

    Private Overloads Sub DispSelectedPost()
        DispSelectedPost(False)
    End Sub

    Private Overloads Sub DispSelectedPost(ByVal forceupdate As Boolean)
        Static displaypost As New PostClass
        If _curList.SelectedIndices.Count = 0 OrElse _
            _curPost Is Nothing Then Exit Sub

        If Not forceupdate AndAlso _curPost.Equals(displaypost) Then
            Exit Sub
        End If

        displaypost = _curPost
        displayItem = DirectCast(_curList.Items(_curList.SelectedIndices(0)), ImageListViewItem)

        Dim dTxt As String = createDetailHtml(If(_curPost.IsDeleted, "(DELETED)", _curPost.Text))
        If _curPost.IsDm Then
            SourceLinkLabel.Tag = Nothing
            SourceLinkLabel.Text = ""
        Else
            Dim mc As Match = Regex.Match(_curPost.SourceHtml, "<a href=""(?<sourceurl>.+?)""")
            If mc.Success Then
                Dim src As String = mc.Groups("sourceurl").Value
                SourceLinkLabel.Tag = mc.Groups("sourceurl").Value
                mc = Regex.Match(src, "^https?://")
                If Not mc.Success Then
                    src = src.Insert(0, "http://twitter.com")
                End If
                SourceLinkLabel.Tag = src
            Else
                SourceLinkLabel.Tag = Nothing
            End If
            If String.IsNullOrEmpty(_curPost.Source) Then
                SourceLinkLabel.Text = ""
                'SourceLinkLabel.Visible = False
            Else
                SourceLinkLabel.Text = _curPost.Source
                'SourceLinkLabel.Visible = True
            End If
        End If
        SourceLinkLabel.TabStop = False

        If _statuses.Tabs(_curTab.Text).TabType = TabUsageType.DirectMessage AndAlso Not _curPost.IsOwl Then
            NameLabel.Text = "DM TO -> "
        ElseIf _statuses.Tabs(_curTab.Text).TabType = TabUsageType.DirectMessage Then
            NameLabel.Text = "DM FROM <- "
        Else
            NameLabel.Text = ""
        End If
        NameLabel.Text += _curPost.ScreenName + "/" + _curPost.Nickname
        NameLabel.Tag = _curPost.ScreenName
        If Not String.IsNullOrEmpty(_curPost.RetweetedBy) Then
            NameLabel.Text += " (RT:" + _curPost.RetweetedBy + ")"
        End If
        If UserPicture.Image IsNot Nothing Then UserPicture.Image.Dispose()
        If Not String.IsNullOrEmpty(_curPost.ImageUrl) AndAlso TIconDic.ContainsKey(_curPost.ImageUrl) Then
            UserPicture.Image = TIconDic(_curPost.ImageUrl)
        Else
            UserPicture.Image = Nothing
        End If

        NameLabel.ForeColor = System.Drawing.SystemColors.ControlText
        DateTimeLabel.Text = _curPost.CreatedAt.ToString()
        If _curPost.IsOwl AndAlso (SettingDialog.OneWayLove OrElse _statuses.Tabs(_curTab.Text).TabType = TabUsageType.DirectMessage) Then NameLabel.ForeColor = _clOWL
        If _curPost.RetweetedId > 0 Then NameLabel.ForeColor = _clRetweet
        If _curPost.IsFav Then NameLabel.ForeColor = _clFav

        If DumpPostClassToolStripMenuItem.Checked Then
            Dim sb As New StringBuilder(512)

            sb.Append("-----Start PostClass Dump<br>")
            sb.AppendFormat("TextFromApi           : {0}<br>", _curPost.TextFromApi)
            sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", _curPost.TextFromApi)
            sb.AppendFormat("StatusId             : {0}<br>", _curPost.StatusId.ToString)
            'sb.AppendFormat("ImageIndex     : {0}<br>", _curPost.ImageIndex.ToString)
            sb.AppendFormat("ImageUrl       : {0}<br>", _curPost.ImageUrl)
            sb.AppendFormat("InReplyToStatusId    : {0}<br>", _curPost.InReplyToStatusId.ToString)
            sb.AppendFormat("InReplyToUser  : {0}<br>", _curPost.InReplyToUser)
            sb.AppendFormat("IsDM           : {0}<br>", _curPost.IsDm.ToString)
            sb.AppendFormat("IsFav          : {0}<br>", _curPost.IsFav.ToString)
            sb.AppendFormat("IsMark         : {0}<br>", _curPost.IsMark.ToString)
            sb.AppendFormat("IsMe           : {0}<br>", _curPost.IsMe.ToString)
            sb.AppendFormat("IsOwl          : {0}<br>", _curPost.IsOwl.ToString)
            sb.AppendFormat("IsProtect      : {0}<br>", _curPost.IsProtect.ToString)
            sb.AppendFormat("IsRead         : {0}<br>", _curPost.IsRead.ToString)
            sb.AppendFormat("IsReply        : {0}<br>", _curPost.IsReply.ToString)

            For Each nm As String In _curPost.ReplyToList
                sb.AppendFormat("ReplyToList    : {0}<br>", nm)
            Next

            sb.AppendFormat("ScreenName           : {0}<br>", _curPost.ScreenName)
            sb.AppendFormat("NickName       : {0}<br>", _curPost.Nickname)
            sb.AppendFormat("Text   : {0}<br>", _curPost.Text)
            sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", _curPost.Text)
            sb.AppendFormat("CreatedAt          : {0}<br>", _curPost.CreatedAt.ToString)
            sb.AppendFormat("Source         : {0}<br>", _curPost.Source)
            sb.AppendFormat("UserId            : {0}<br>", _curPost.UserId)
            sb.AppendFormat("FilterHit      : {0}<br>", _curPost.FilterHit)
            sb.AppendFormat("RetweetedBy    : {0}<br>", _curPost.RetweetedBy)
            sb.AppendFormat("RetweetedId    : {0}<br>", _curPost.RetweetedId)
            sb.AppendFormat("SearchTabName  : {0}<br>", _curPost.RelTabName)
            sb.Append("-----End PostClass Dump<br>")

            PostBrowser.Visible = False
            PostBrowser.DocumentText = detailHtmlFormatHeader + sb.ToString + detailHtmlFormatFooter
            PostBrowser.Visible = True
        Else
            Try
                If PostBrowser.DocumentText <> dTxt Then
                    PostBrowser.Visible = False
                    PostBrowser.DocumentText = dTxt
                    Dim lnks As New List(Of String)
                    For Each lnk As Match In Regex.Matches(dTxt, "<a target=""_self"" href=""(?<url>http[^""]+)""", RegexOptions.IgnoreCase)
                        lnks.Add(lnk.Result("${url}"))
                    Next
                    Thumbnail.thumbnail(_curPost.StatusId, lnks)
                End If
            Catch ex As System.Runtime.InteropServices.COMException
                '原因不明
            Finally
                PostBrowser.Visible = True
            End Try
        End If
    End Sub

    Private Sub MatomeMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MatomeMenuItem.Click
        OpenUriAsync("http://sourceforge.jp/projects/tween/wiki/FrontPage")
    End Sub

    Private Sub ShortcutKeyListMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShortcutKeyListMenuItem.Click
        OpenUriAsync("http://sourceforge.jp/projects/tween/wiki/%E3%82%B7%E3%83%A7%E3%83%BC%E3%83%88%E3%82%AB%E3%83%83%E3%83%88%E3%82%AD%E3%83%BC")
    End Sub

    Private Sub ListTab_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ListTab.KeyDown
        If ListTab.SelectedTab IsNot Nothing Then
            If _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.PublicSearch Then
                Dim pnl As Control = ListTab.SelectedTab.Controls("panelSearch")
                If pnl.Controls("comboSearch").Focused OrElse _
                   pnl.Controls("comboLang").Focused OrElse _
                   pnl.Controls("buttonSearch").Focused Then Exit Sub
            End If
            Dim State As ModifierState = GetModifierState(e.Control, e.Shift, e.Alt)
            If State = ModifierState.NotFlags Then Exit Sub
            If State <> ModifierState.Non Then _anchorFlag = False
            If CommonKeyDown(e.KeyCode, ModifierState.ListTab, State) Then
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If

    End Sub

    Public Function GetModifierState(ByVal sControl As Boolean, ByVal sShift As Boolean, ByVal sAlt As Boolean) As ModifierState
        If Not sAlt AndAlso Not sControl AndAlso Not sShift Then Return ModifierState.Non
        If sControl Then
            If sShift AndAlso Not sAlt Then
                Return ModifierState.CShift
            ElseIf sAlt AndAlso Not sShift Then
                Return ModifierState.CAlt
            ElseIf Not sAlt AndAlso Not sShift Then
                Return ModifierState.Ctrl
            End If
        ElseIf sShift Then
            If sAlt AndAlso Not sControl Then
                Return ModifierState.AShift
            ElseIf Not sAlt AndAlso Not sControl Then
                Return ModifierState.Shift
            End If
        ElseIf sAlt Then
            If Not sControl AndAlso Not sShift Then Return ModifierState.Alt
        End If
        Return ModifierState.NotFlags
    End Function

    Public Enum ModifierState As Integer
        Non = 0
        Alt = 1
        Shift = 2
        Ctrl = 3
        CShift = 11
        CAlt = 12
        AShift = 13
        NotFlags = 20

        ListTab = 101
        PostBrowser = 102
        StatusText = 103
    End Enum

    Public Function CommonKeyDown(ByVal KeyCode As System.Windows.Forms.Keys, ByVal Focused As ModifierState, ByVal Modifier As ModifierState) As Boolean
        Dim Pressed As Boolean = False

        '修飾キーなし
        If Modifier = ModifierState.Non Then
            If KeyCode = Keys.F1 Then
                Pressed = True
                OpenUriAsync("http://sourceforge.jp/projects/tween/wiki/FrontPage")
            ElseIf KeyCode = Keys.F3 Then
                Pressed = True
                MenuItemSearchNext_Click(Nothing, Nothing)
            ElseIf KeyCode = Keys.F5 Then
                Pressed = True
                DoRefresh()
            ElseIf KeyCode = Keys.F6 Then
                Pressed = True
                GetTimeline(WORKERTYPE.Reply, 1, 0, "")
            ElseIf KeyCode = Keys.F7 Then
                Pressed = True
                GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, 0, "")
            End If
            If Focused <> ModifierState.StatusText AndAlso Not Pressed Then
                If KeyCode = Keys.Space OrElse KeyCode = Keys.ProcessKey Then
                    Pressed = True
                    If Focused = ModifierState.ListTab Then _anchorFlag = False
                    JumpUnreadMenuItem_Click(Nothing, Nothing)
                ElseIf KeyCode = Keys.G Then
                    Pressed = True
                    If Focused = ModifierState.ListTab Then _anchorFlag = False
                    ShowRelatedStatusesMenuItem_Click(Nothing, Nothing)
                End If
            End If
            If Focused = ModifierState.ListTab AndAlso Not Pressed Then
                If KeyCode = Keys.N OrElse KeyCode = Keys.Right Then
                    Pressed = True
                    GoRelPost(True)
                    Return Pressed
                ElseIf KeyCode = Keys.P OrElse KeyCode = Keys.Left Then
                    Pressed = True
                    GoRelPost(False)
                    Return Pressed
                ElseIf KeyCode = Keys.OemPeriod Then
                    Pressed = True
                    GoAnchor()
                    Return Pressed
                End If
                _anchorFlag = False
                If KeyCode = Keys.Enter OrElse KeyCode = Keys.Return Then
                    Pressed = True
                    MakeReplyOrDirectStatus()
                ElseIf KeyCode = Keys.L Then
                    Pressed = True
                    GoPost(True)
                ElseIf KeyCode = Keys.H Then
                    Pressed = True
                    GoPost(False)
                ElseIf KeyCode = Keys.J Then
                    Pressed = True
                    SendKeys.Send("{DOWN}")
                ElseIf KeyCode = Keys.K Then
                    Pressed = True
                    SendKeys.Send("{UP}")
                End If
                If KeyCode = Keys.Z Or KeyCode = Keys.Oemcomma Then
                    Pressed = True
                    MoveTop()
                ElseIf KeyCode = Keys.R Then
                    Pressed = True
                    DoRefresh()
                ElseIf KeyCode = Keys.S Then
                    Pressed = True
                    GoNextTab(True)
                ElseIf KeyCode = Keys.A Then
                    Pressed = True
                    GoNextTab(False)
                End If
                'If keycode = Keys.OemQuestion Then
                '    Pressed=true    
                '    MenuItemSubSearch_Click(Nothing, Nothing)   '/検索
                'End If
                If KeyCode = Keys.F Then
                    Pressed = True
                    SendKeys.Send("{PGDN}")
                End If
                If KeyCode = Keys.B Then
                    Pressed = True
                    SendKeys.Send("{PGUP}")
                End If
                If KeyCode = Keys.I Then
                    Pressed = True
                    'SendKeys.Send("{TAB}")
                    If Me.StatusText.Enabled Then Me.StatusText.Focus()
                End If
                ' ] in_reply_to参照元へ戻る
                If KeyCode = Keys.Oem4 Then
                    Pressed = True
                    GoInReplyToPostTree()
                End If
                ' [ in_reply_toへジャンプ
                If KeyCode = Keys.Oem6 Then
                    Pressed = True
                    GoBackInReplyToPostTree()
                End If
                If KeyCode = Keys.Escape Then
                    If ListTab.SelectedTab IsNot Nothing Then
                        Dim tabtype As TabUsageType = _statuses.Tabs(ListTab.SelectedTab.Text).TabType
                        If tabtype = TabUsageType.Related OrElse tabtype = TabUsageType.UserTimeline Then
                            Dim relTp As TabPage = ListTab.SelectedTab
                            RemoveSpecifiedTab(relTp.Text, False)
                            SaveConfigsTabs()
                        End If
                    End If
                End If
            End If
            Return Pressed
        End If

        If Focused = ModifierState.StatusText AndAlso Not Pressed Then
            If KeyCode = Keys.Space AndAlso Modifier = ModifierState.CShift Then
                If StatusText.SelectionStart > 0 Then
                    Dim endidx As Integer = StatusText.SelectionStart - 1
                    Dim startstr As String = ""
                    For i As Integer = StatusText.SelectionStart - 1 To 0 Step -1
                        Dim c As Char = StatusText.Text.Chars(i)
                        If Char.IsLetterOrDigit(c) OrElse c = "_" Then
                            Continue For
                        End If
                        If c = "@" Then
                            Pressed = True
                            startstr = StatusText.Text.Substring(i + 1, endidx - i)
                            ShowSuplDialog(StatusText, AtIdSupl, startstr.Length + 1, startstr)
                        ElseIf c = "#" Then
                            Pressed = True
                            startstr = StatusText.Text.Substring(i + 1, endidx - i)
                            ShowSuplDialog(StatusText, HashSupl, startstr.Length + 1, startstr)
                        Else
                            Exit For
                        End If
                    Next
                    Return Pressed
                End If
            End If
        End If

        'Ctrl+何か
        If Modifier = ModifierState.Ctrl Then
            If KeyCode = Keys.R Then
                Pressed = True
                MakeReplyOrDirectStatus(False, True)
            ElseIf KeyCode = Keys.D Then
                Pressed = True
                doStatusDelete()
            ElseIf KeyCode = Keys.M Then
                Pressed = True
                MakeReplyOrDirectStatus(False, False)
            ElseIf KeyCode = Keys.S Then
                Pressed = True
                FavoriteChange(True)
            ElseIf KeyCode = Keys.I Then
                Pressed = True
                doRepliedStatusOpen()
            ElseIf KeyCode = Keys.Q Then
                Pressed = True
                doQuote()
            ElseIf KeyCode = Keys.B Then
                Pressed = True
                ReadedStripMenuItem_Click(Nothing, Nothing)
            ElseIf KeyCode = Keys.T Then
                Pressed = True
                HashManageMenuItem_Click(Nothing, Nothing)
            ElseIf KeyCode = Keys.L Then
                Pressed = True
                UrlConvertAutoToolStripMenuItem_Click(Nothing, Nothing)
            ElseIf KeyCode = Keys.Y AndAlso Not Focused = ModifierState.PostBrowser Then
                Pressed = True
                MultiLineMenuItem_Click(Nothing, Nothing)
            ElseIf KeyCode = Keys.F Then
                Pressed = True
                MenuItemSubSearch_Click(Nothing, Nothing)
            ElseIf KeyCode = Keys.U Then
                Pressed = True
                ShowUserTimeline()
            End If


            ' Webページを開く動作

            Select Case KeyCode
                Case Keys.H
                    If _curList.SelectedIndices.Count > 0 Then
                        OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices(0)).ScreenName)
                    ElseIf _curList.SelectedIndices.Count = 0 Then
                        OpenUriAsync("http://twitter.com/")
                    End If
                Case Keys.G
                    If _curList.SelectedIndices.Count > 0 Then
                        OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices(0)).ScreenName + "/favorites")
                    End If
                Case Keys.O
                    StatusOpenMenuItem_Click(Nothing, Nothing)
                Case Keys.E
                    OpenURLMenuItem_Click(Nothing, Nothing)
            End Select
            If Not Pressed Then
                If Focused = ModifierState.ListTab Then
                    If KeyCode = Keys.Home OrElse KeyCode = Keys.End Then
                        _colorize = True
                    End If
                    If KeyCode = Keys.N Then
                        Pressed = True
                        GoNextTab(True)
                    ElseIf KeyCode = Keys.P Then
                        Pressed = True
                        GoNextTab(False)
                    ElseIf KeyCode = Keys.C Then
                        Dim clstr As String = ""
                        Pressed = True
                        CopyStot()
                    End If


                    ' タブダイレクト選択(Ctrl+1～8,Ctrl+9)

                    Select Case KeyCode
                        Case Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8
                            Dim tabNo As Integer = KeyCode - Keys.D1
                            If ListTab.TabPages.Count < tabNo Then
                                Exit Function
                            End If
                            ListTab.SelectedIndex = tabNo
                            ListTabSelect(ListTab.TabPages(tabNo))
                            Return Pressed
                        Case Keys.D9
                            ListTab.SelectedIndex = ListTab.TabPages.Count - 1
                            ListTabSelect(ListTab.TabPages(ListTab.TabPages.Count - 1))
                            Return Pressed
                        Case Else
                    End Select
                ElseIf Focused = ModifierState.StatusText Then
                    If KeyCode = Keys.A Then
                        StatusText.SelectAll()
                        Return Pressed
                    ElseIf KeyCode = Keys.Up OrElse KeyCode = Keys.Down Then
                        If StatusText.Text.Trim() <> "" Then
                            _history(_hisIdx) = New PostingStatus(StatusText.Text, _reply_to_id, _reply_to_name)
                        End If
                        If KeyCode = Keys.Up Then
                            _hisIdx -= 1
                            If _hisIdx < 0 Then _hisIdx = 0
                        Else
                            _hisIdx += 1
                            If _hisIdx > _history.Count - 1 Then _hisIdx = _history.Count - 1
                        End If
                        StatusText.Text = _history(_hisIdx).status
                        _reply_to_id = _history(_hisIdx).inReplyToId
                        _reply_to_name = _history(_hisIdx).inReplyToName
                        StatusText.SelectionStart = StatusText.Text.Length
                        Pressed = True
                        Return Pressed
                    End If
                    If KeyCode = Keys.PageUp OrElse KeyCode = Keys.P Then
                        If ListTab.SelectedIndex = 0 Then
                            ListTab.SelectedIndex = ListTab.TabCount - 1
                        Else
                            ListTab.SelectedIndex -= 1
                        End If
                        Pressed = True
                        StatusText.Focus()
                    ElseIf KeyCode = Keys.PageDown OrElse KeyCode = Keys.N Then
                        If ListTab.SelectedIndex = ListTab.TabCount - 1 Then
                            ListTab.SelectedIndex = 0
                        Else
                            ListTab.SelectedIndex += 1
                        End If
                        Pressed = True
                        StatusText.Focus()
                    End If
                Else
                    If KeyCode = Keys.A Then
                        Pressed = True
                        PostBrowser.Document.ExecCommand("SelectAll", False, Nothing)
                    ElseIf KeyCode = Keys.C OrElse KeyCode = Keys.Insert Then
                        Pressed = True
                        Dim _selText As String = WebBrowser_GetSelectionText(PostBrowser)
                        If Not String.IsNullOrEmpty(_selText) Then
                            Try
                                Clipboard.SetDataObject(_selText, False, 5, 100)
                            Catch ex As Exception
                                MessageBox.Show(ex.Message)
                            End Try
                        End If
                    ElseIf KeyCode = Keys.Y Then
                        Pressed = True
                        MultiLineMenuItem.Checked = Not MultiLineMenuItem.Checked
                        MultiLineMenuItem_Click(Nothing, Nothing)
                    End If
                End If

            End If
            Return Pressed
        End If

        'SHift+何か
        If Modifier = ModifierState.Shift Then
            If KeyCode = Keys.F3 Then
                Pressed = True
                MenuItemSearchPrev_Click(Nothing, Nothing)
            ElseIf KeyCode = Keys.F5 Then
                Pressed = True
                DoRefreshMore()
            ElseIf KeyCode = Keys.F6 Then
                Pressed = True
                GetTimeline(WORKERTYPE.Reply, -1, 0, "")
            ElseIf KeyCode = Keys.F7 Then
                Pressed = True
                GetTimeline(WORKERTYPE.DirectMessegeRcv, -1, 0, "")
            End If
            If Focused <> ModifierState.StatusText Then
                If KeyCode = Keys.R AndAlso Not Pressed Then
                    DoRefreshMore()
                    Return True
                End If
                If Focused = ModifierState.ListTab Then
                    If KeyCode = Keys.H Then
                        Pressed = True
                        GoTopEnd(True)
                    ElseIf KeyCode = Keys.L Then
                        Pressed = True
                        GoTopEnd(False)
                    ElseIf KeyCode = Keys.M Then
                        Pressed = True
                        GoMiddle()
                    ElseIf KeyCode = Keys.G Then
                        Pressed = True
                        GoLast()
                    ElseIf KeyCode = Keys.Z Then
                        Pressed = True
                        MoveMiddle()
                    ElseIf KeyCode = Keys.J Then
                        Pressed = True
                        SendKeys.Send("{DOWN}")
                    ElseIf KeyCode = Keys.K Then
                        Pressed = True
                        SendKeys.Send("{UP}")
                    ElseIf KeyCode = Keys.Oem4 Then
                        Pressed = True
                        GoBackInReplyToPostTree(True, False)
                    ElseIf KeyCode = Keys.Oem6 Then
                        Pressed = True
                        GoBackInReplyToPostTree(True, True)
                    End If

                    ' お気に入り前後ジャンプ(SHIFT+N←/P→)
                    If KeyCode = Keys.N OrElse KeyCode = Keys.Right Then
                        Pressed = True
                        GoFav(True)
                    ElseIf KeyCode = Keys.P OrElse KeyCode = Keys.Left Then
                        Pressed = True
                        GoFav(False)
                    End If
                End If
            End If
            Return Pressed
        End If

        'Alt+何か
        If Modifier = ModifierState.Alt Then
            If KeyCode = Keys.R Then
                Pressed = True
                doReTweetOfficial(True)
            ElseIf KeyCode = Keys.P AndAlso _curPost IsNot Nothing Then
                Pressed = True
                doShowUserStatus(_curPost.ScreenName, False)
            End If
            If KeyCode = Keys.Up Then
                ScrollDownPostBrowser(False)
            ElseIf KeyCode = Keys.Down Then
                ScrollDownPostBrowser(True)
            ElseIf KeyCode = Keys.PageUp Then
                PageDownPostBrowser(False)
            ElseIf KeyCode = Keys.PageDown Then
                PageDownPostBrowser(True)
            End If
            If Focused = ModifierState.ListTab AndAlso Not Pressed Then
                ' 別タブの同じ書き込みへ(ALT+←/→)
                If KeyCode = Keys.Right Then
                    Pressed = True
                    GoSamePostToAnotherTab(False)
                ElseIf KeyCode = Keys.Left Then
                    Pressed = True
                    GoSamePostToAnotherTab(True)
                End If
            End If
            Return Pressed
        End If

        'Ctrl+Shift+何か
        If Modifier = ModifierState.CShift Then
            If KeyCode = Keys.R Then
                Pressed = True
                MakeReplyOrDirectStatus(False, True, True)
            ElseIf KeyCode = Keys.C Then
                Dim clstr As String = ""
                Pressed = True
                CopyIdUri()
            ElseIf KeyCode = Keys.F Then
                Pressed = True
                If ListTab.SelectedTab IsNot Nothing Then
                    If _statuses.Tabs(ListTab.SelectedTab.Text).TabType <> TabUsageType.PublicSearch Then Return Pressed
                    ListTab.SelectedTab.Controls("panelSearch").Controls("comboSearch").Focus()
                End If
            ElseIf KeyCode = Keys.S Then
                Pressed = True
                FavoriteChange(False)
            ElseIf KeyCode = Keys.B Then
                Pressed = True
                UnreadStripMenuItem_Click(Nothing, Nothing)
            End If
            If KeyCode = Keys.T Then
                Pressed = True
                HashToggleMenuItem_Click(Nothing, Nothing)
            ElseIf KeyCode = Keys.P Then
                Pressed = True
                ImageSelectMenuItem_Click(Nothing, Nothing)
            ElseIf KeyCode = Keys.H Then
                Pressed = True
                doMoveToRTHome()
            ElseIf KeyCode = Keys.O Then
                Pressed = True
                FavorareMenuItem_Click(Nothing, Nothing)
            End If
            If Not Pressed AndAlso Focused = ModifierState.StatusText Then
                If KeyCode = Keys.Up Then
                    Pressed = True
                    Dim idx As Integer = 0
                    If _curList IsNot Nothing AndAlso _curList.Items.Count <> 0 AndAlso _
                                _curList.SelectedIndices.Count > 0 AndAlso _curList.SelectedIndices(0) > 0 Then
                        idx = _curList.SelectedIndices(0) - 1
                        SelectListItem(_curList, idx)
                        _curList.EnsureVisible(idx)
                    End If
                ElseIf KeyCode = Keys.Down Then
                    Pressed = True
                    Dim idx As Integer = 0
                    If _curList IsNot Nothing AndAlso _curList.Items.Count <> 0 AndAlso _curList.SelectedIndices.Count > 0 _
                                AndAlso _curList.SelectedIndices(0) < _curList.Items.Count - 1 Then
                        idx = _curList.SelectedIndices(0) + 1
                        SelectListItem(_curList, idx)
                        _curList.EnsureVisible(idx)
                    End If
                End If
            End If
            Return Pressed
        End If

        'Ctrl+Alt+何か
        If Modifier = ModifierState.CAlt Then
            If KeyCode = Keys.S Then
                Pressed = True
                FavoritesRetweetOriginal()
            ElseIf KeyCode = Keys.R Then
                Pressed = True
                FavoritesRetweetUnofficial()
            End If
            Return Pressed
        End If

        'Alt+Shift+何か
        If Modifier = ModifierState.AShift Then
            If Focused = ModifierState.PostBrowser Then
                If KeyCode = Keys.R Then
                    doReTweetUnofficial()
                ElseIf KeyCode = Keys.C Then
                    CopyUserId()
                End If
                Return Pressed
            Else
                If KeyCode = Keys.R Then
                    Pressed = True
                    doReTweetUnofficial()
                ElseIf KeyCode = Keys.C Then
                    Pressed = True
                    CopyUserId()
                ElseIf KeyCode = Keys.Up Then
                    Thumbnail.ScrollThumbnail(False)
                ElseIf KeyCode = Keys.Down Then
                    Thumbnail.ScrollThumbnail(True)
                End If
                If Focused = ModifierState.ListTab AndAlso KeyCode = Keys.Enter Then
                    If Not Me.SplitContainer3.Panel2Collapsed Then
                        Thumbnail.OpenPicture()
                    End If
                    Pressed = True
                End If
                Return Pressed
            End If
        End If
        Return Pressed
    End Function

    Private Sub ScrollDownPostBrowser(ByVal forward As Boolean)
        Dim doc As HtmlDocument = PostBrowser.Document
        If doc Is Nothing Then Exit Sub
        If doc.Body Is Nothing Then Exit Sub

        If forward Then
            doc.Body.ScrollTop += SettingDialog.FontDetail.Height
        Else
            doc.Body.ScrollTop -= SettingDialog.FontDetail.Height
        End If
    End Sub

    Private Sub PageDownPostBrowser(ByVal forward As Boolean)
        Dim doc As HtmlDocument = PostBrowser.Document
        If doc Is Nothing Then Exit Sub
        If doc.Body Is Nothing Then Exit Sub

        If forward Then
            doc.Body.ScrollTop += PostBrowser.ClientRectangle.Height - SettingDialog.FontDetail.Height
        Else
            doc.Body.ScrollTop -= PostBrowser.ClientRectangle.Height - SettingDialog.FontDetail.Height
        End If
    End Sub

    Private Sub GoNextTab(ByVal forward As Boolean)
        Dim idx As Integer = ListTab.SelectedIndex
        If forward Then
            idx += 1
            If idx > ListTab.TabPages.Count - 1 Then idx = 0
        Else
            idx -= 1
            If idx < 0 Then idx = ListTab.TabPages.Count - 1
        End If
        ListTab.SelectedIndex = idx
        ListTabSelect(ListTab.TabPages(idx))
    End Sub

    Private Sub CopyStot()
        Dim clstr As String = ""
        Dim sb As New StringBuilder()
        Dim IsProtected As Boolean = False
        Dim isDm As Boolean = False
        If Me._curTab IsNot Nothing AndAlso Me._statuses.GetTabByName(Me._curTab.Text) IsNot Nothing Then isDm = Me._statuses.GetTabByName(Me._curTab.Text).TabType = TabUsageType.DirectMessage
        For Each idx As Integer In _curList.SelectedIndices
            Dim post As PostClass = _statuses.Item(_curTab.Text, idx)
            If post.IsProtect Then
                IsProtected = True
                Continue For
            End If
            If post.IsDeleted Then Continue For
            If Not isDm Then
                If post.RetweetedId > 0 Then
                    sb.AppendFormat("{0}:{1} [http://twitter.com/{0}/status/{2}]{3}", post.ScreenName, post.TextFromApi, post.RetweetedId, Environment.NewLine)
                Else
                    sb.AppendFormat("{0}:{1} [http://twitter.com/{0}/status/{2}]{3}", post.ScreenName, post.TextFromApi, post.StatusId, Environment.NewLine)
                End If
            Else
                sb.AppendFormat("{0}:{1} [{2}]{3}", post.ScreenName, post.TextFromApi, post.StatusId, Environment.NewLine)
            End If
        Next
        If IsProtected Then
            MessageBox.Show(My.Resources.CopyStotText1)
        End If
        If sb.Length > 0 Then
            clstr = sb.ToString()
            Try
                Clipboard.SetDataObject(clstr, False, 5, 100)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    Private Sub CopyIdUri()
        Dim clstr As String = ""
        Dim sb As New StringBuilder()
        If Me._curTab Is Nothing Then Exit Sub
        If Me._statuses.GetTabByName(Me._curTab.Text) Is Nothing Then Exit Sub
        If Me._statuses.GetTabByName(Me._curTab.Text).TabType = TabUsageType.DirectMessage Then Exit Sub
        For Each idx As Integer In _curList.SelectedIndices
            Dim post As PostClass = _statuses.Item(_curTab.Text, idx)
            If post.RetweetedId > 0 Then
                sb.AppendFormat("http://twitter.com/{0}/status/{1}{2}", post.ScreenName, post.RetweetedId, Environment.NewLine)
            Else
                sb.AppendFormat("http://twitter.com/{0}/status/{1}{2}", post.ScreenName, post.StatusId, Environment.NewLine)
            End If
        Next
        If sb.Length > 0 Then
            clstr = sb.ToString()
            Try
                Clipboard.SetDataObject(clstr, False, 5, 100)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    Private Sub GoFav(ByVal forward As Boolean)
        If _curList.VirtualListSize = 0 Then Exit Sub
        Dim fIdx As Integer = 0
        Dim toIdx As Integer = 0
        Dim stp As Integer = 1

        If forward Then
            If _curList.SelectedIndices.Count = 0 Then
                fIdx = 0
            Else
                fIdx = _curList.SelectedIndices(0) + 1
                If fIdx > _curList.VirtualListSize - 1 Then Exit Sub
            End If
            toIdx = _curList.VirtualListSize - 1
            stp = 1
        Else
            If _curList.SelectedIndices.Count = 0 Then
                fIdx = _curList.VirtualListSize - 1
            Else
                fIdx = _curList.SelectedIndices(0) - 1
                If fIdx < 0 Then Exit Sub
            End If
            toIdx = 0
            stp = -1
        End If

        For idx As Integer = fIdx To toIdx Step stp
            If _statuses.Item(_curTab.Text, idx).IsFav Then
                SelectListItem(_curList, idx)
                _curList.EnsureVisible(idx)
                Exit For
            End If
        Next
    End Sub

    Private Sub GoSamePostToAnotherTab(ByVal left As Boolean)
        If _curList.VirtualListSize = 0 Then Exit Sub
        Dim fIdx As Integer = 0
        Dim toIdx As Integer = 0
        Dim stp As Integer = 1
        Dim targetId As Long = 0

        If _statuses.Tabs(_curTab.Text).TabType = TabUsageType.DirectMessage Then Exit Sub ' Directタブは対象外（見つかるはずがない）
        If _curList.SelectedIndices.Count = 0 Then Exit Sub '未選択も処理しない

        targetId = GetCurTabPost(_curList.SelectedIndices(0)).StatusId

        If left Then
            ' 左のタブへ
            If ListTab.SelectedIndex = 0 Then
                Exit Sub
            Else
                fIdx = ListTab.SelectedIndex - 1
            End If
            toIdx = 0
            stp = -1
        Else
            ' 右のタブへ
            If ListTab.SelectedIndex = ListTab.TabCount - 1 Then
                Exit Sub
            Else
                fIdx = ListTab.SelectedIndex + 1
            End If
            toIdx = ListTab.TabCount - 1
            stp = 1
        End If

        Dim found As Boolean = False
        For tabidx As Integer = fIdx To toIdx Step stp
            If _statuses.Tabs(ListTab.TabPages(tabidx).Text).TabType = TabUsageType.DirectMessage Then Continue For ' Directタブは対象外
            For idx As Integer = 0 To DirectCast(ListTab.TabPages(tabidx).Tag, DetailsListView).VirtualListSize - 1
                If _statuses.Item(ListTab.TabPages(tabidx).Text, idx).StatusId = targetId Then
                    ListTab.SelectedIndex = tabidx
                    ListTabSelect(ListTab.TabPages(tabidx))
                    SelectListItem(_curList, idx)
                    _curList.EnsureVisible(idx)
                    found = True
                    Exit For
                End If
            Next
            If found Then Exit For
        Next
    End Sub

    Private Sub GoPost(ByVal forward As Boolean)
        If _curList.SelectedIndices.Count = 0 OrElse _curPost Is Nothing Then Exit Sub
        Dim fIdx As Integer = 0
        Dim toIdx As Integer = 0
        Dim stp As Integer = 1

        If forward Then
            fIdx = _curList.SelectedIndices(0) + 1
            If fIdx > _curList.VirtualListSize - 1 Then Exit Sub
            toIdx = _curList.VirtualListSize - 1
            stp = 1
        Else
            fIdx = _curList.SelectedIndices(0) - 1
            If fIdx < 0 Then Exit Sub
            toIdx = 0
            stp = -1
        End If

        Dim name As String = ""
        If _curPost.RetweetedId = 0 Then
            name = _curPost.ScreenName
        Else
            name = _curPost.RetweetedBy
        End If
        For idx As Integer = fIdx To toIdx Step stp
            If _statuses.Item(_curTab.Text, idx).RetweetedId = 0 Then
                If _statuses.Item(_curTab.Text, idx).ScreenName = name Then
                    SelectListItem(_curList, idx)
                    _curList.EnsureVisible(idx)
                    Exit For
                End If
            Else
                If _statuses.Item(_curTab.Text, idx).RetweetedBy = name Then
                    SelectListItem(_curList, idx)
                    _curList.EnsureVisible(idx)
                    Exit For
                End If
            End If
        Next
    End Sub

    Private Sub GoRelPost(ByVal forward As Boolean)
        If _curList.SelectedIndices.Count = 0 Then Exit Sub

        Dim fIdx As Integer = 0
        Dim toIdx As Integer = 0
        Dim stp As Integer = 1
        If forward Then
            fIdx = _curList.SelectedIndices(0) + 1
            If fIdx > _curList.VirtualListSize - 1 Then Exit Sub
            toIdx = _curList.VirtualListSize - 1
            stp = 1
        Else
            fIdx = _curList.SelectedIndices(0) - 1
            If fIdx < 0 Then Exit Sub
            toIdx = 0
            stp = -1
        End If

        If Not _anchorFlag Then
            If _curPost Is Nothing Then Exit Sub
            _anchorPost = _curPost
            _anchorFlag = True
        Else
            If _anchorPost Is Nothing Then Exit Sub
        End If

        For idx As Integer = fIdx To toIdx Step stp
            Dim post As PostClass = _statuses.Item(_curTab.Text, idx)
            If post.ScreenName = _anchorPost.ScreenName OrElse _
               post.RetweetedBy = _anchorPost.ScreenName OrElse _
               post.ScreenName = _anchorPost.RetweetedBy OrElse _
               (Not String.IsNullOrEmpty(post.RetweetedBy) AndAlso post.RetweetedBy = _anchorPost.RetweetedBy) OrElse _
               _anchorPost.ReplyToList.Contains(post.ScreenName.ToLower()) OrElse _
               _anchorPost.ReplyToList.Contains(post.RetweetedBy.ToLower()) OrElse _
               post.ReplyToList.Contains(_anchorPost.ScreenName.ToLower()) OrElse _
               post.ReplyToList.Contains(_anchorPost.RetweetedBy.ToLower()) Then
                SelectListItem(_curList, idx)
                _curList.EnsureVisible(idx)
                Exit For
            End If
        Next
    End Sub

    Private Sub GoAnchor()
        If _anchorPost Is Nothing Then Exit Sub
        Dim idx As Integer = _statuses.Tabs(_curTab.Text).IndexOf(_anchorPost.StatusId)
        If idx = -1 Then Exit Sub

        SelectListItem(_curList, idx)
        _curList.EnsureVisible(idx)
    End Sub

    Private Sub GoTopEnd(ByVal GoTop As Boolean)
        Dim _item As ListViewItem
        Dim idx As Integer

        If GoTop Then
            _item = _curList.GetItemAt(0, 25)
            If _item Is Nothing Then
                idx = 0
            Else
                idx = _item.Index
            End If
        Else
            _item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1)
            If _item Is Nothing Then
                idx = _curList.VirtualListSize - 1
            Else
                idx = _item.Index
            End If
        End If
        SelectListItem(_curList, idx)
    End Sub

    Private Sub GoMiddle()
        Dim _item As ListViewItem
        Dim idx1 As Integer
        Dim idx2 As Integer
        Dim idx3 As Integer

        _item = _curList.GetItemAt(0, 0)
        If _item Is Nothing Then
            idx1 = 0
        Else
            idx1 = _item.Index
        End If
        _item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1)
        If _item Is Nothing Then
            idx2 = _curList.VirtualListSize - 1
        Else
            idx2 = _item.Index
        End If
        idx3 = (idx1 + idx2) \ 2

        SelectListItem(_curList, idx3)
    End Sub

    Private Sub GoLast()
        If _curList.VirtualListSize = 0 Then Exit Sub

        If _statuses.SortOrder = SortOrder.Ascending Then
            SelectListItem(_curList, _curList.VirtualListSize - 1)
            _curList.EnsureVisible(_curList.VirtualListSize - 1)
        Else
            SelectListItem(_curList, 0)
            _curList.EnsureVisible(0)
        End If
    End Sub

    Private Sub MoveTop()
        If _curList.SelectedIndices.Count = 0 Then Exit Sub
        Dim idx As Integer = _curList.SelectedIndices(0)
        If _statuses.SortOrder = SortOrder.Ascending Then
            _curList.EnsureVisible(_curList.VirtualListSize - 1)
        Else
            _curList.EnsureVisible(0)
        End If
        _curList.EnsureVisible(idx)
    End Sub

    Private Sub GoInReplyToPostTree()
        If _curPost Is Nothing Then Return

        Dim curTabClass As TabClass = _statuses.Tabs(_curTab.Text)

        If curTabClass.TabType = TabUsageType.PublicSearch AndAlso _curPost.InReplyToStatusId = 0 AndAlso _curPost.TextFromApi.Contains("@") Then
            Dim post As PostClass = Nothing
            Dim r As String = tw.GetStatusApi(False, _curPost.StatusId, post)
            If r = "" AndAlso post IsNot Nothing Then
                _curPost.InReplyToStatusId = post.InReplyToStatusId
                _curPost.InReplyToUser = post.InReplyToUser
                _curPost.IsReply = post.IsReply
                _itemCache = Nothing
                _curList.RedrawItems(_curItemIndex, _curItemIndex, False)
            Else
                Me.StatusLabel.Text = r
            End If
        End If

        If Not (Me.ExistCurrentPost AndAlso _curPost.InReplyToUser IsNot Nothing AndAlso _curPost.InReplyToStatusId > 0) Then Return

        If replyChains Is Nothing OrElse (replyChains.Count > 0 AndAlso replyChains.Peek().InReplyToId <> _curPost.StatusId) Then
            replyChains = New Stack(Of ReplyChain)
        End If
        replyChains.Push(New ReplyChain(_curPost.StatusId, _curPost.InReplyToStatusId, _curTab))

        Dim inReplyToIndex As Integer
        Dim inReplyToTabName As String
        Dim inReplyToId As Long = _curPost.InReplyToStatusId
        Dim inReplyToUser As String = _curPost.InReplyToUser
        Dim curTabPosts As Dictionary(Of Long, PostClass)

        If _statuses.Tabs(_curTab.Text).IsInnerStorageTabType Then
            curTabPosts = curTabClass.Posts
        Else
            curTabPosts = _statuses.Posts
        End If

        Dim inReplyToPosts = From tab In _statuses.Tabs.Values
                             Order By tab IsNot curTabClass
                             From post In DirectCast(IIf(tab.IsInnerStorageTabType, tab.Posts, _statuses.Posts), Dictionary(Of Long, PostClass)).Values
                             Where post.StatusId = inReplyToId
                             Let index = tab.IndexOf(post.StatusId)
                             Where index <> -1
                             Select New With {.Tab = tab, .Index = index}

        Try
            Dim inReplyPost = inReplyToPosts.First()
            inReplyToTabName = inReplyPost.Tab.TabName
            inReplyToIndex = inReplyPost.Index
        Catch ex As InvalidOperationException
            Dim post As PostClass = Nothing
            Dim r As String = tw.GetStatusApi(False, _curPost.InReplyToStatusId, post)
            If r = "" AndAlso post IsNot Nothing Then
                post.IsRead = True
                _statuses.AddPost(post)
                _statuses.DistributePosts()
                _statuses.SubmitUpdate(Nothing, Nothing, Nothing, False)
                Me.RefreshTimeline(False)
                Try
                    Dim inReplyPost = inReplyToPosts.First()
                    inReplyToTabName = inReplyPost.Tab.TabName
                    inReplyToIndex = inReplyPost.Index
                Catch ex2 As InvalidOperationException
                    OpenUriAsync("http://twitter.com/" + inReplyToUser + "/statuses/" + inReplyToId.ToString())
                    Exit Sub
                End Try
            Else
                Me.StatusLabel.Text = r
                OpenUriAsync("http://twitter.com/" + inReplyToUser + "/statuses/" + inReplyToId.ToString())
                Exit Sub
            End If
        End Try

        Dim tabPage = Me.ListTab.TabPages.Cast(Of TabPage).First(Function(tp) tp.Text = inReplyToTabName)
        Dim listView = DirectCast(tabPage.Tag, DetailsListView)

        If _curTab IsNot tabPage Then
            Me.ListTab.SelectTab(tabPage)
        End If

        Me.SelectListItem(listView, inReplyToIndex)
        listView.EnsureVisible(inReplyToIndex)
    End Sub

    Private Sub GoBackInReplyToPostTree(Optional ByVal parallel As Boolean = False, Optional ByVal isForward As Boolean = True)
        If _curPost Is Nothing Then Return

        Dim curTabClass As TabClass = _statuses.Tabs(_curTab.Text)
        Dim curTabPosts As Dictionary(Of Long, PostClass) = DirectCast(IIf(curTabClass.IsInnerStorageTabType, curTabClass.Posts, _statuses.Posts), Dictionary(Of Long, PostClass))

        If parallel Then
            If _curPost.InReplyToStatusId <> 0 Then
                Dim posts = From t In _statuses.Tabs
                            From p In DirectCast(IIf(t.Value.IsInnerStorageTabType, t.Value.Posts, _statuses.Posts), Dictionary(Of Long, PostClass))
                            Where p.Value.StatusId <> _curPost.StatusId AndAlso p.Value.InReplyToStatusId = _curPost.InReplyToStatusId
                            Let indexOf = t.Value.IndexOf(p.Value.StatusId)
                            Where indexOf > -1
                            Order By IIf(isForward, indexOf, indexOf * -1)
                            Order By t.Value IsNot curTabClass
                            Select New With {.Tab = t.Value, .Post = p.Value, .Index = indexOf}
                Try
                    Dim postList = posts.ToList()
                    For i As Integer = postList.Count - 1 To 0 Step -1
                        Dim index As Integer = i
                        If postList.FindIndex(Function(pst) pst.Post.StatusId = postList(index).Post.StatusId) <> index Then
                            postList.RemoveAt(index)
                        End If
                    Next
                    Dim post = postList.FirstOrDefault(Function(pst) pst.Tab Is curTabClass AndAlso DirectCast(IIf(isForward, pst.Index > _curItemIndex, pst.Index < _curItemIndex), Boolean))
                    If post Is Nothing Then post = postList.FirstOrDefault(Function(pst) pst.Tab IsNot curTabClass)
                    If post Is Nothing Then post = postList.First()
                    Me.ListTab.SelectTab(Me.ListTab.TabPages.Cast(Of TabPage).First(Function(tp) tp.Text = post.Tab.TabName))
                    Dim listView = DirectCast(Me.ListTab.SelectedTab.Tag, DetailsListView)
                    SelectListItem(listView, post.Index)
                    listView.EnsureVisible(post.Index)
                Catch ex As InvalidOperationException
                    Exit Sub
                End Try
            End If
        Else
            If replyChains Is Nothing OrElse replyChains.Count < 1 Then
                Dim posts = From t In _statuses.Tabs
                            From p In DirectCast(IIf(t.Value.IsInnerStorageTabType, t.Value.Posts, _statuses.Posts), Dictionary(Of Long, PostClass))
                            Where p.Value.InReplyToStatusId = _curPost.StatusId
                            Let indexOf = t.Value.IndexOf(p.Value.StatusId)
                            Where indexOf > -1
                            Order By indexOf
                            Order By t.Value IsNot curTabClass
                            Select New With {.Tab = t.Value, .Index = indexOf}
                Try
                    Dim post = posts.First()
                    Me.ListTab.SelectTab(Me.ListTab.TabPages.Cast(Of TabPage).First(Function(tp) tp.Text = post.Tab.TabName))
                    Dim listView = DirectCast(Me.ListTab.SelectedTab.Tag, DetailsListView)
                    SelectListItem(listView, post.Index)
                    listView.EnsureVisible(post.Index)
                Catch ex As InvalidOperationException
                    Exit Sub
                End Try
            Else
                Dim chainHead As ReplyChain = replyChains.Pop()
                If chainHead.InReplyToId = _curPost.StatusId Then
                    Dim idx As Integer = _statuses.Tabs(chainHead.OriginalTab.Text).IndexOf(chainHead.OriginalId)
                    If idx = -1 Then
                        replyChains = Nothing
                    Else
                        Try
                            ListTab.SelectTab(chainHead.OriginalTab)
                        Catch ex As Exception
                            replyChains = Nothing
                        End Try
                        SelectListItem(_curList, idx)
                        _curList.EnsureVisible(idx)
                    End If
                Else
                    replyChains = Nothing
                    Me.GoBackInReplyToPostTree(parallel)
                End If
            End If
        End If
    End Sub

    Private Sub MyList_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        _anchorFlag = False
    End Sub

    Private Sub StatusText_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusText.Enter
        ' フォーカスの戻り先を StatusText に設定
        Me.Tag = StatusText
        StatusText.BackColor = _clInputBackcolor
    End Sub

    Public Property InputBackColor() As System.Drawing.Color
        Get
            Return _clInputBackcolor
        End Get
        Set(ByVal value As System.Drawing.Color)
            _clInputBackcolor = value
        End Set
    End Property

    Private Sub StatusText_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusText.Leave
        ' フォーカスがメニューに遷移しないならばフォーカスはタブに移ることを期待
        If ListTab.SelectedTab IsNot Nothing AndAlso MenuStrip1.Tag Is Nothing Then Me.Tag = ListTab.SelectedTab.Tag
        StatusText.BackColor = Color.FromKnownColor(KnownColor.Window)
    End Sub

    Private Sub StatusText_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles StatusText.KeyDown
        Dim State As ModifierState = GetModifierState(e.Control, e.Shift, e.Alt)
        If State = ModifierState.NotFlags Then Exit Sub
        If CommonKeyDown(e.KeyCode, ModifierState.StatusText, State) Then
            e.Handled = True
            e.SuppressKeyPress = True
        End If

        Me.StatusText_TextChanged(Nothing, Nothing)
    End Sub

    Private Sub SaveConfigsAll(ByVal ifModified As Boolean)
        If Not ifModified Then
            SaveConfigsCommon()
            SaveConfigsLocal()
            'SaveConfigsTab(True)    'True:事前に設定ファイル削除
            SaveConfigsTabs()
        Else
            If _modifySettingCommon Then SaveConfigsCommon()
            If _modifySettingLocal Then SaveConfigsLocal()
            If _modifySettingAtId AndAlso SettingDialog.UseAtIdSupplement AndAlso AtIdSupl IsNot Nothing Then
                _modifySettingAtId = False
                Dim cfgAtId As New SettingAtIdList(AtIdSupl.GetItemList)
                cfgAtId.Save()
            End If
        End If
    End Sub

    Private Sub SaveConfigsCommon()
        If _ignoreConfigSave Then Exit Sub

        _modifySettingCommon = False
        SyncLock _syncObject
            _cfgCommon.UserName = tw.Username
            _cfgCommon.Password = tw.Password
            _cfgCommon.IsOAuth = SettingDialog.IsOAuth
            _cfgCommon.Token = tw.AccessToken
            _cfgCommon.TokenSecret = tw.AccessTokenSecret
            _cfgCommon.UserstreamStartup = SettingDialog.UserstreamStartup
            _cfgCommon.UserstreamPeriod = SettingDialog.UserstreamPeriodInt
            _cfgCommon.TimelinePeriod = SettingDialog.TimelinePeriodInt
            _cfgCommon.ReplyPeriod = SettingDialog.ReplyPeriodInt
            _cfgCommon.DMPeriod = SettingDialog.DMPeriodInt
            _cfgCommon.PubSearchPeriod = SettingDialog.PubSearchPeriodInt
            _cfgCommon.ListsPeriod = SettingDialog.ListsPeriodInt
            _cfgCommon.UserTimelinePeriod = SettingDialog.UserTimelinePeriodInt
            _cfgCommon.Read = SettingDialog.Readed
            _cfgCommon.IconSize = SettingDialog.IconSz
            _cfgCommon.UnreadManage = SettingDialog.UnreadManage
            _cfgCommon.PlaySound = SettingDialog.PlaySound
            _cfgCommon.OneWayLove = SettingDialog.OneWayLove

            _cfgCommon.NameBalloon = SettingDialog.NameBalloon
            _cfgCommon.PostCtrlEnter = SettingDialog.PostCtrlEnter
            _cfgCommon.PostShiftEnter = SettingDialog.PostShiftEnter
            _cfgCommon.CountApi = SettingDialog.CountApi
            _cfgCommon.CountApiReply = SettingDialog.CountApiReply
            _cfgCommon.PostAndGet = SettingDialog.PostAndGet
            _cfgCommon.DispUsername = SettingDialog.DispUsername
            _cfgCommon.MinimizeToTray = SettingDialog.MinimizeToTray
            _cfgCommon.CloseToExit = SettingDialog.CloseToExit
            _cfgCommon.DispLatestPost = SettingDialog.DispLatestPost
            _cfgCommon.SortOrderLock = SettingDialog.SortOrderLock
            _cfgCommon.TinyUrlResolve = SettingDialog.TinyUrlResolve
            _cfgCommon.PeriodAdjust = SettingDialog.PeriodAdjust
            _cfgCommon.StartupVersion = SettingDialog.StartupVersion
            _cfgCommon.StartupFollowers = SettingDialog.StartupFollowers
            _cfgCommon.RestrictFavCheck = SettingDialog.RestrictFavCheck
            _cfgCommon.AlwaysTop = SettingDialog.AlwaysTop
            _cfgCommon.UrlConvertAuto = SettingDialog.UrlConvertAuto
            _cfgCommon.Outputz = SettingDialog.OutputzEnabled
            _cfgCommon.OutputzKey = SettingDialog.OutputzKey
            _cfgCommon.OutputzUrlMode = SettingDialog.OutputzUrlmode
            _cfgCommon.UseUnreadStyle = SettingDialog.UseUnreadStyle
            _cfgCommon.DateTimeFormat = SettingDialog.DateTimeFormat
            _cfgCommon.DefaultTimeOut = SettingDialog.DefaultTimeOut
            _cfgCommon.RetweetNoConfirm = SettingDialog.RetweetNoConfirm
            _cfgCommon.LimitBalloon = SettingDialog.LimitBalloon
            _cfgCommon.EventNotifyEnabled = SettingDialog.EventNotifyEnabled
            _cfgCommon.EventNotifyFlag = SettingDialog.EventNotifyFlag
            _cfgCommon.IsMyEventNotifyFlag = SettingDialog.IsMyEventNotifyFlag
            _cfgCommon.ForceEventNotify = SettingDialog.ForceEventNotify
            _cfgCommon.FavEventUnread = SettingDialog.FavEventUnread
            _cfgCommon.TranslateLanguage = SettingDialog.TranslateLanguage
            _cfgCommon.EventSoundFile = SettingDialog.EventSoundFile
            _cfgCommon.AutoShortUrlFirst = SettingDialog.AutoShortUrlFirst
            _cfgCommon.TabIconDisp = SettingDialog.TabIconDisp
            _cfgCommon.ReplyIconState = SettingDialog.ReplyIconState
            _cfgCommon.ReadOwnPost = SettingDialog.ReadOwnPost
            _cfgCommon.GetFav = SettingDialog.GetFav
            _cfgCommon.IsMonospace = SettingDialog.IsMonospace
            If IdeographicSpaceToSpaceToolStripMenuItem IsNot Nothing AndAlso _
               IdeographicSpaceToSpaceToolStripMenuItem.IsDisposed = False Then
                _cfgCommon.WideSpaceConvert = Me.IdeographicSpaceToSpaceToolStripMenuItem.Checked
            End If
            _cfgCommon.ReadOldPosts = SettingDialog.ReadOldPosts
            _cfgCommon.UseSsl = SettingDialog.UseSsl
            _cfgCommon.BilyUser = SettingDialog.BitlyUser
            _cfgCommon.BitlyPwd = SettingDialog.BitlyPwd
            _cfgCommon.ShowGrid = SettingDialog.ShowGrid
            _cfgCommon.UseAtIdSupplement = SettingDialog.UseAtIdSupplement
            _cfgCommon.UseHashSupplement = SettingDialog.UseHashSupplement
            _cfgCommon.PreviewEnable = SettingDialog.PreviewEnable
            _cfgCommon.Language = SettingDialog.Language

            _cfgCommon.SortOrder = _statuses.SortOrder
            Select Case _statuses.SortMode
                Case IdComparerClass.ComparerMode.Nickname  'ニックネーム
                    _cfgCommon.SortColumn = 1
                Case IdComparerClass.ComparerMode.Data  '本文
                    _cfgCommon.SortColumn = 2
                Case IdComparerClass.ComparerMode.Id  '時刻=発言Id
                    _cfgCommon.SortColumn = 3
                Case IdComparerClass.ComparerMode.Name  '名前
                    _cfgCommon.SortColumn = 4
                Case IdComparerClass.ComparerMode.Source  'Source
                    _cfgCommon.SortColumn = 7
            End Select

            _cfgCommon.Nicoms = SettingDialog.Nicoms
            _cfgCommon.HashTags = HashMgr.HashHistories
            If HashMgr.IsPermanent Then
                _cfgCommon.HashSelected = HashMgr.UseHash
            Else
                _cfgCommon.HashSelected = ""
            End If
            _cfgCommon.HashIsHead = HashMgr.IsHead
            _cfgCommon.HashIsPermanent = HashMgr.IsPermanent
            _cfgCommon.TwitterUrl = SettingDialog.TwitterApiUrl
            _cfgCommon.TwitterSearchUrl = SettingDialog.TwitterSearchApiUrl
            _cfgCommon.HotkeyEnabled = SettingDialog.HotkeyEnabled
            _cfgCommon.HotkeyModifier = SettingDialog.HotkeyMod
            _cfgCommon.HotkeyKey = SettingDialog.HotkeyKey
            _cfgCommon.HotkeyValue = SettingDialog.HotkeyValue
            _cfgCommon.BlinkNewMentions = SettingDialog.BlinkNewMentions
            If ToolStripFocusLockMenuItem IsNot Nothing AndAlso _
                    ToolStripFocusLockMenuItem.IsDisposed = False Then
                _cfgCommon.FocusLockToStatusText = Me.ToolStripFocusLockMenuItem.Checked
            End If
            _cfgCommon.UseAdditionalCount = SettingDialog.UseAdditionalCount
            _cfgCommon.MoreCountApi = SettingDialog.MoreCountApi
            _cfgCommon.FirstCountApi = SettingDialog.FirstCountApi
            _cfgCommon.SearchCountApi = SettingDialog.SearchCountApi
            _cfgCommon.FavoritesCountApi = SettingDialog.FavoritesCountApi
            _cfgCommon.UserTimelineCountApi = SettingDialog.UserTimelineCountApi
            _cfgCommon.TrackWord = tw.TrackWord
            _cfgCommon.AllAtReply = tw.AllAtReply
            _cfgCommon.OpenUserTimeline = SettingDialog.OpenUserTimeline
            _cfgCommon.ListCountApi = SettingDialog.ListCountApi
            _cfgCommon.UseImageSurvice = ImageServiceCombo.SelectedIndex

            _cfgCommon.Save()
        End SyncLock
    End Sub

    Private Sub SaveConfigsLocal()
        If _ignoreConfigSave Then Exit Sub
        SyncLock _syncObject
            _modifySettingLocal = False
            _cfgLocal.FormSize = _mySize
            _cfgLocal.FormLocation = _myLoc
            _cfgLocal.SplitterDistance = _mySpDis
            _cfgLocal.PreviewDistance = _mySpDis3
            _cfgLocal.StatusMultiline = StatusText.Multiline
            _cfgLocal.StatusTextHeight = _mySpDis2
            _cfgLocal.StatusText = SettingDialog.Status

            _cfgLocal.FontUnread = _fntUnread
            _cfgLocal.ColorUnread = _clUnread
            _cfgLocal.FontRead = _fntReaded
            _cfgLocal.ColorRead = _clReaded
            _cfgLocal.FontDetail = _fntDetail
            _cfgLocal.ColorDetail = _clDetail
            _cfgLocal.ColorDetailBackcolor = _clDetailBackcolor
            _cfgLocal.ColorDetailLink = _clDetailLink
            _cfgLocal.ColorFav = _clFav
            _cfgLocal.ColorOWL = _clOWL
            _cfgLocal.ColorRetweet = _clRetweet
            _cfgLocal.ColorSelf = _clSelf
            _cfgLocal.ColorAtSelf = _clAtSelf
            _cfgLocal.ColorTarget = _clTarget
            _cfgLocal.ColorAtTarget = _clAtTarget
            _cfgLocal.ColorAtFromTarget = _clAtFromTarget
            _cfgLocal.ColorAtTo = _clAtTo
            _cfgLocal.ColorListBackcolor = _clListBackcolor
            _cfgLocal.ColorInputBackcolor = _clInputBackcolor
            _cfgLocal.ColorInputFont = _clInputFont
            _cfgLocal.FontInputFont = _fntInputFont

            _cfgLocal.BrowserPath = SettingDialog.BrowserPath
            _cfgLocal.UseRecommendStatus = SettingDialog.UseRecommendStatus
            _cfgLocal.ProxyType = SettingDialog.SelectedProxyType
            _cfgLocal.ProxyAddress = SettingDialog.ProxyAddress
            _cfgLocal.ProxyPort = SettingDialog.ProxyPort
            _cfgLocal.ProxyUser = SettingDialog.ProxyUser
            _cfgLocal.ProxyPassword = SettingDialog.ProxyPassword
            If _ignoreConfigSave Then Exit Sub
            _cfgLocal.Save()
        End SyncLock
    End Sub

    Private Sub SaveConfigsTabs()
        Dim tabSetting As New SettingTabs
        For i As Integer = 0 To ListTab.TabPages.Count - 1
            If _statuses.Tabs(ListTab.TabPages(i).Text).TabType <> TabUsageType.Related Then tabSetting.Tabs.Add(_statuses.Tabs(ListTab.TabPages(i).Text))
        Next
        tabSetting.Save()
    End Sub

    Private Sub SaveLogMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveLogMenuItem.Click, SaveFileMenuItem.Click
        Dim rslt As DialogResult = MessageBox.Show(String.Format(My.Resources.SaveLogMenuItem_ClickText1, Environment.NewLine), _
                My.Resources.SaveLogMenuItem_ClickText2, _
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
        If rslt = Windows.Forms.DialogResult.Cancel Then Exit Sub

        SaveFileDialog1.FileName = "TweenPosts" + Format(Now, "yyMMdd-HHmmss") + ".tsv"
        SaveFileDialog1.InitialDirectory = My.Application.Info.DirectoryPath
        SaveFileDialog1.Filter = My.Resources.SaveLogMenuItem_ClickText3
        SaveFileDialog1.FilterIndex = 0
        SaveFileDialog1.Title = My.Resources.SaveLogMenuItem_ClickText4
        SaveFileDialog1.RestoreDirectory = True

        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            If Not SaveFileDialog1.ValidateNames Then Exit Sub
            Using sw As StreamWriter = New StreamWriter(SaveFileDialog1.FileName, False, Encoding.UTF8)
                If rslt = Windows.Forms.DialogResult.Yes Then
                    'All
                    For idx As Integer = 0 To _curList.VirtualListSize - 1
                        Dim post As PostClass = _statuses.Item(_curTab.Text, idx)
                        Dim protect As String = ""
                        If post.IsProtect Then protect = "Protect"
                        sw.WriteLine(post.Nickname & vbTab & _
                                 """" & post.TextFromApi.Replace(vbLf, "").Replace("""", """""") + """" & vbTab & _
                                 post.CreatedAt.ToString() & vbTab & _
                                 post.ScreenName & vbTab & _
                                 post.StatusId.ToString() & vbTab & _
                                 post.ImageUrl & vbTab & _
                                 """" & post.Text.Replace(vbLf, "").Replace("""", """""") + """" & vbTab & _
                                 protect)
                    Next
                Else
                    For Each idx As Integer In _curList.SelectedIndices
                        Dim post As PostClass = _statuses.Item(_curTab.Text, idx)
                        Dim protect As String = ""
                        If post.IsProtect Then protect = "Protect"
                        sw.WriteLine(post.Nickname & vbTab & _
                                 """" & post.TextFromApi.Replace(vbLf, "").Replace("""", """""") + """" & vbTab & _
                                 post.CreatedAt.ToString() & vbTab & _
                                 post.ScreenName & vbTab & _
                                 post.StatusId.ToString() & vbTab & _
                                 post.ImageUrl & vbTab & _
                                 """" & post.Text.Replace(vbLf, "").Replace("""", """""") + """" & vbTab & _
                                 protect)
                    Next
                End If
                sw.Close()
                sw.Dispose()
            End Using
        End If
        Me.TopMost = SettingDialog.AlwaysTop
    End Sub

    Private Sub PostBrowser_PreviewKeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles PostBrowser.PreviewKeyDown
        Dim State As ModifierState = GetModifierState(e.Control, e.Shift, e.Alt)
        If State = ModifierState.NotFlags Then Exit Sub
        Dim KeyRes As Boolean = CommonKeyDown(e.KeyCode, ModifierState.PostBrowser, State)
        If KeyRes Then
            e.IsInputKey = True
        End If
    End Sub
    Public Function TabRename(ByRef tabName As String) As Boolean
        'タブ名変更
        Dim newTabText As String = Nothing
        Using inputName As New InputTabName()
            inputName.TabName = tabName
            inputName.ShowDialog()
            If inputName.DialogResult = Windows.Forms.DialogResult.Cancel Then Return False
            newTabText = inputName.TabName
            inputName.Dispose()
        End Using
        Me.TopMost = SettingDialog.AlwaysTop
        If Not String.IsNullOrEmpty(newTabText) Then
            '新タブ名存在チェック
            For i As Integer = 0 To ListTab.TabCount - 1
                If ListTab.TabPages(i).Text = newTabText Then
                    Dim tmp As String = String.Format(My.Resources.Tabs_DoubleClickText1, newTabText)
                    MessageBox.Show(tmp, My.Resources.Tabs_DoubleClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return False
                End If
            Next
            'タブ名のリスト作り直し（デフォルトタブ以外は再作成）
            For i As Integer = 0 To ListTab.TabCount - 1
                If _statuses.Tabs(ListTab.TabPages(i).Text).TabType = TabUsageType.Mentions OrElse _
                   (Not _statuses.IsDefaultTab(ListTab.TabPages(i).Text) AndAlso _statuses.Tabs(ListTab.TabPages(i).Text).TabType <> TabUsageType.PublicSearch AndAlso _statuses.Tabs(ListTab.TabPages(i).Text).TabType <> TabUsageType.Lists AndAlso _statuses.Tabs(ListTab.TabPages(i).Text).TabType <> TabUsageType.Related) Then
                    TabDialog.RemoveTab(ListTab.TabPages(i).Text)
                End If
                If ListTab.TabPages(i).Text = tabName Then
                    ListTab.TabPages(i).Text = newTabText
                End If
            Next
            _statuses.RenameTab(tabName, newTabText)

            For i As Integer = 0 To ListTab.TabCount - 1
                If _statuses.Tabs(ListTab.TabPages(i).Text).TabType = TabUsageType.Mentions OrElse _
                   (Not _statuses.IsDefaultTab(ListTab.TabPages(i).Text) AndAlso _statuses.Tabs(ListTab.TabPages(i).Text).TabType <> TabUsageType.PublicSearch AndAlso _statuses.Tabs(ListTab.TabPages(i).Text).TabType <> TabUsageType.Lists AndAlso _statuses.Tabs(ListTab.TabPages(i).Text).TabType <> TabUsageType.Related) Then
                    If ListTab.TabPages(i).Text = tabName Then
                        ListTab.TabPages(i).Text = newTabText
                    End If
                    TabDialog.AddTab(ListTab.TabPages(i).Text)
                End If
            Next
            SaveConfigsCommon()
            SaveConfigsTabs()
            _rclickTabName = newTabText
            tabName = newTabText
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub ListTab_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListTab.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Middle Then
            For i As Integer = 0 To Me.ListTab.TabPages.Count - 1
                If Me.ListTab.GetTabRect(i).Contains(e.Location) Then
                    Me.RemoveSpecifiedTab(Me.ListTab.TabPages(i).Text, True)
                    Me.SaveConfigsTabs()
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub Tabs_DoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListTab.MouseDoubleClick
        Dim tn As String = ListTab.SelectedTab.Text
        TabRename(tn)
    End Sub

    Private Sub Tabs_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListTab.MouseDown
        Dim cpos As New Point(e.X, e.Y)
        If e.Button = Windows.Forms.MouseButtons.Left Then
            For i As Integer = 0 To ListTab.TabPages.Count - 1
                If Me.ListTab.GetTabRect(i).Contains(e.Location) Then
                    _tabDrag = True
                    _tabMouseDownPoint = e.Location
                    Exit For
                End If
            Next
        Else
            _tabDrag = False
        End If
    End Sub

    Private Sub Tabs_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListTab.DragEnter
        If e.Data.GetDataPresent(GetType(TabPage)) Then
            e.Effect = DragDropEffects.Move
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub Tabs_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListTab.DragDrop
        If Not e.Data.GetDataPresent(GetType(TabPage)) Then Exit Sub

        _tabDrag = False
        Dim tn As String = ""
        Dim bef As Boolean
        Dim cpos As New Point(e.X, e.Y)
        Dim spos As Point = ListTab.PointToClient(cpos)
        Dim i As Integer
        For i = 0 To ListTab.TabPages.Count - 1
            Dim rect As Rectangle = ListTab.GetTabRect(i)
            If rect.Left <= spos.X AndAlso spos.X <= rect.Right AndAlso _
               rect.Top <= spos.Y AndAlso spos.Y <= rect.Bottom Then
                tn = ListTab.TabPages(i).Text
                If spos.X <= (rect.Left + rect.Right) / 2 Then
                    bef = True
                Else
                    bef = False
                End If
                Exit For
            End If
        Next

        'タブのないところにドロップ->最後尾へ移動
        If String.IsNullOrEmpty(tn) Then
            tn = ListTab.TabPages(ListTab.TabPages.Count - 1).Text
            bef = False
            i = ListTab.TabPages.Count - 1
        End If

        Dim tp As TabPage = DirectCast(e.Data.GetData(GetType(TabPage)), TabPage)
        If tp.Text = tn Then Exit Sub

        ReOrderTab(tp.Text, tn, bef)
    End Sub

    Public Sub ReOrderTab(ByVal targetTabText As String, ByVal baseTabText As String, ByVal isBeforeBaseTab As Boolean)
        Dim baseIndex As Integer = 0
        For baseIndex = 0 To ListTab.TabPages.Count - 1
            If ListTab.TabPages(baseIndex).Text = baseTabText Then Exit For
        Next

        ListTab.SuspendLayout()

        Dim mTp As TabPage = Nothing
        For j As Integer = 0 To ListTab.TabPages.Count - 1
            If ListTab.TabPages(j).Text = targetTabText Then
                mTp = ListTab.TabPages(j)
                ListTab.TabPages.Remove(mTp)
                If j < baseIndex Then baseIndex -= 1
                Exit For
            End If
        Next
        If isBeforeBaseTab Then
            ListTab.TabPages.Insert(baseIndex, mTp)
        Else
            ListTab.TabPages.Insert(baseIndex + 1, mTp)
        End If

        ListTab.ResumeLayout()

        SaveConfigsTabs()
    End Sub

    Private Sub MakeReplyOrDirectStatus(Optional ByVal isAuto As Boolean = True, Optional ByVal isReply As Boolean = True, Optional ByVal isAll As Boolean = False)
        'isAuto:True=先頭に挿入、False=カーソル位置に挿入
        'isReply:True=@,False=DM
        If Not StatusText.Enabled Then Exit Sub
        If _curList Is Nothing Then Exit Sub
        If _curTab Is Nothing Then Exit Sub
        If Not Me.ExistCurrentPost Then Exit Sub

        ' 複数あてリプライはReplyではなく通常ポスト
        '↑仕様変更で全部リプライ扱いでＯＫ（先頭ドット付加しない）
        '090403暫定でドットを付加しないようにだけ修正。単独と複数の処理は統合できると思われる。
        '090513 all @ replies 廃止の仕様変更によりドット付加に戻し(syo68k)

        If _curList.SelectedIndices.Count > 0 Then
            ' アイテムが1件以上選択されている
            If _curList.SelectedIndices.Count = 1 AndAlso Not isAll AndAlso Me.ExistCurrentPost Then
                ' 単独ユーザー宛リプライまたはDM
                If (_statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.DirectMessage AndAlso isAuto) OrElse (Not isAuto AndAlso Not isReply) Then
                    ' ダイレクトメッセージ
                    StatusText.Text = "D " + _curPost.ScreenName + " " + StatusText.Text
                    StatusText.SelectionStart = StatusText.Text.Length
                    StatusText.Focus()
                    _reply_to_id = 0
                    _reply_to_name = ""
                    Exit Sub
                End If
                If String.IsNullOrEmpty(StatusText.Text) Then
                    '空の場合

                    ' ステータステキストが入力されていない場合先頭に@ユーザー名を追加する
                    StatusText.Text = "@" + _curPost.ScreenName + " "
                    If _curPost.RetweetedId > 0 Then
                        _reply_to_id = _curPost.RetweetedId
                    Else
                        _reply_to_id = _curPost.StatusId
                    End If
                    _reply_to_name = _curPost.ScreenName
                Else
                    '何か入力済の場合

                    If isAuto Then
                        '1件選んでEnter or DoubleClick
                        If StatusText.Text.Contains("@" + _curPost.ScreenName + " ") Then
                            If _reply_to_id > 0 AndAlso _reply_to_name = _curPost.ScreenName Then
                                '返信先書き換え
                                If _curPost.RetweetedId > 0 Then
                                    _reply_to_id = _curPost.RetweetedId
                                Else
                                    _reply_to_id = _curPost.StatusId
                                End If
                                _reply_to_name = _curPost.ScreenName
                            End If
                            Exit Sub
                        End If
                        If Not StatusText.Text.StartsWith("@") Then
                            '文頭＠以外
                            If StatusText.Text.StartsWith(". ") Then
                                ' 複数リプライ
                                StatusText.Text = StatusText.Text.Insert(2, "@" + _curPost.ScreenName + " ")
                                _reply_to_id = 0
                                _reply_to_name = ""
                            Else
                                ' 単独リプライ
                                StatusText.Text = "@" + _curPost.ScreenName + " " + StatusText.Text
                                If _curPost.RetweetedId > 0 Then
                                    _reply_to_id = _curPost.RetweetedId
                                Else
                                    _reply_to_id = _curPost.StatusId
                                End If
                                _reply_to_name = _curPost.ScreenName
                            End If
                        Else
                            '文頭＠
                            ' 複数リプライ
                            StatusText.Text = ". @" + _curPost.ScreenName + " " + StatusText.Text
                            'StatusText.Text = "@" + _curPost.ScreenName + " " + StatusText.Text
                            _reply_to_id = 0
                            _reply_to_name = ""
                        End If
                    Else
                        '1件選んでCtrl-Rの場合（返信先操作せず）
                        Dim sidx As Integer = StatusText.SelectionStart
                        Dim id As String = "@" + _curPost.ScreenName + " "
                        If sidx > 0 Then
                            If StatusText.Text.Substring(sidx - 1, 1) <> " " Then
                                id = " " + id
                            End If
                        End If
                        StatusText.Text = StatusText.Text.Insert(sidx, id)
                        sidx += id.Length
                        'If StatusText.Text.StartsWith("@") Then
                        '    '複数リプライ
                        '    StatusText.Text = ". " + StatusText.Text.Insert(sidx, " @" + _curPost.ScreenName + " ")
                        '    sidx += 5 + _curPost.ScreenName.Length
                        'Else
                        '    ' 複数リプライ
                        '    StatusText.Text = StatusText.Text.Insert(sidx, " @" + _curPost.ScreenName + " ")
                        '    sidx += 3 + _curPost.ScreenName.Length
                        'End If
                        StatusText.SelectionStart = sidx
                        StatusText.Focus()
                        '_reply_to_id = 0
                        '_reply_to_name = Nothing
                        Exit Sub
                    End If
                End If
            Else
                ' 複数リプライ
                If Not isAuto AndAlso Not isReply Then Exit Sub

                'C-S-rか、複数の宛先を選択中にEnter/DoubleClick/C-r/C-S-r

                If isAuto Then
                    'Enter or DoubleClick

                    Dim sTxt As String = StatusText.Text
                    If Not sTxt.StartsWith(". ") Then
                        sTxt = ". " + sTxt
                        _reply_to_id = 0
                        _reply_to_name = ""
                    End If
                    For cnt As Integer = 0 To _curList.SelectedIndices.Count - 1
                        Dim post As PostClass = _statuses.Item(_curTab.Text, _curList.SelectedIndices(cnt))
                        If Not sTxt.Contains("@" + post.ScreenName + " ") Then
                            sTxt = sTxt.Insert(2, "@" + post.ScreenName + " ")
                            'sTxt = "@" + post.ScreenName + " " + sTxt
                        End If
                    Next
                    StatusText.Text = sTxt
                Else
                    'C-S-r or C-r
                    If _curList.SelectedIndices.Count > 1 Then
                        '複数ポスト選択

                        Dim ids As String = ""
                        Dim sidx As Integer = StatusText.SelectionStart
                        For cnt As Integer = 0 To _curList.SelectedIndices.Count - 1
                            Dim post As PostClass = _statuses.Item(_curTab.Text, _curList.SelectedIndices(cnt))
                            If Not ids.Contains("@" + post.ScreenName + " ") AndAlso _
                               Not post.ScreenName.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase) Then
                                ids += "@" + post.ScreenName + " "
                            End If
                            If isAll Then
                                For Each nm As String In post.ReplyToList
                                    If Not ids.Contains("@" + nm + " ") AndAlso _
                                       Not nm.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase) Then
                                        Dim m As Match = Regex.Match(post.TextFromApi, "[@＠](?<id>" + nm + ")([^a-zA-Z0-9]|$)", RegexOptions.IgnoreCase)
                                        If m.Success Then
                                            ids += "@" + m.Result("${id}") + " "
                                        Else
                                            ids += "@" + nm + " "
                                        End If
                                    End If
                                Next
                            End If
                        Next
                        If ids.Length = 0 Then Exit Sub
                        If Not StatusText.Text.StartsWith(". ") Then
                            StatusText.Text = ". " + StatusText.Text
                            sidx += 2
                            _reply_to_id = 0
                            _reply_to_name = ""
                        End If
                        If sidx > 0 Then
                            If StatusText.Text.Substring(sidx - 1, 1) <> " " Then
                                ids = " " + ids
                            End If
                        End If
                        StatusText.Text = StatusText.Text.Insert(sidx, ids)
                        sidx += ids.Length
                        'If StatusText.Text.StartsWith("@") Then
                        '    StatusText.Text = ". " + StatusText.Text.Insert(sidx, ids)
                        '    sidx += 2 + ids.Length
                        'Else
                        '    StatusText.Text = StatusText.Text.Insert(sidx, ids)
                        '    sidx += 1 + ids.Length
                        'End If
                        StatusText.SelectionStart = sidx
                        StatusText.Focus()
                        Exit Sub
                    Else
                        '1件のみ選択のC-S-r（返信元付加する可能性あり）

                        Dim ids As String = ""
                        Dim sidx As Integer = StatusText.SelectionStart
                        Dim post As PostClass = _curPost
                        If Not ids.Contains("@" + post.ScreenName + " ") AndAlso _
                           Not post.ScreenName.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase) Then
                            ids += "@" + post.ScreenName + " "
                        End If
                        For Each nm As String In post.ReplyToList
                            If Not ids.Contains("@" + nm + " ") AndAlso _
                               Not nm.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase) Then
                                Dim m As Match = Regex.Match(post.TextFromApi, "[@＠](?<id>" + nm + ")([^a-zA-Z0-9]|$)", RegexOptions.IgnoreCase)
                                If m.Success Then
                                    ids += "@" + m.Result("${id}") + " "
                                Else
                                    ids += "@" + nm + " "
                                End If
                            End If
                        Next
                        If Not String.IsNullOrEmpty(post.RetweetedBy) Then
                            If Not ids.Contains("@" + post.RetweetedBy + " ") AndAlso _
                               Not post.RetweetedBy.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase) Then
                                ids += "@" + post.RetweetedBy + " "
                            End If
                        End If
                        If ids.Length = 0 Then Exit Sub
                        If String.IsNullOrEmpty(StatusText.Text) Then
                            '未入力の場合のみ返信先付加
                            StatusText.Text = ids
                            StatusText.SelectionStart = ids.Length
                            StatusText.Focus()
                            If post.RetweetedId > 0 Then
                                _reply_to_id = post.RetweetedId
                            Else
                                _reply_to_id = post.StatusId
                            End If
                            _reply_to_name = post.ScreenName
                            Exit Sub
                        End If

                        If sidx > 0 Then
                            If StatusText.Text.Substring(sidx - 1, 1) <> " " Then
                                ids = " " + ids
                            End If
                        End If
                        StatusText.Text = StatusText.Text.Insert(sidx, ids)
                        sidx += ids.Length
                        StatusText.SelectionStart = sidx
                        StatusText.Focus()
                        Exit Sub
                    End If
                End If
            End If
            StatusText.SelectionStart = StatusText.Text.Length
            StatusText.Focus()
        End If
    End Sub

    Private Sub ListTab_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListTab.MouseUp
        _tabDrag = False
    End Sub

    Private Sub RefreshTasktrayIcon(ByVal forceRefresh As Boolean)
        If _colorize Then Colorize()
        If Not TimerRefreshIcon.Enabled Then Exit Sub
        Static iconCnt As Integer = 0
        Static blinkCnt As Integer = 0
        Static blink As Boolean = False
        Static idle As Boolean = False
        'Static usCheckCnt As Integer = 0

        Static iconDlListTopItem As ListViewItem = Nothing

        If forceRefresh Then idle = False

        If DirectCast(ListTab.SelectedTab.Tag, ListView).TopItem Is iconDlListTopItem Then
            DirectCast(Me.TIconDic, ImageDictionary).PauseGetImage = False
        Else
            DirectCast(Me.TIconDic, ImageDictionary).PauseGetImage = True
        End If
        iconDlListTopItem = DirectCast(ListTab.SelectedTab.Tag, ListView).TopItem

        iconCnt += 1
        blinkCnt += 1
        'usCheckCnt += 1

        'If usCheckCnt > 300 Then    '1min
        '    usCheckCnt = 0
        '    If Not Me.IsReceivedUserStream Then
        '        TraceOut("ReconnectUserStream")
        '        tw.ReconnectUserStream()
        '    End If
        'End If

        Dim busy As Boolean = False
        For Each bw As BackgroundWorker In Me._bw
            If bw IsNot Nothing AndAlso bw.IsBusy Then
                busy = True
                Exit For
            End If
        Next

        If iconCnt > 3 Then
            iconCnt = 0
        End If
        If blinkCnt > 10 Then
            blinkCnt = 0
            '未保存の変更を保存
            SaveConfigsAll(True)
        End If

        If busy Then
            NotifyIcon1.Icon = NIconRefresh(iconCnt)
            idle = False
            _myStatusError = False
            Exit Sub
        End If

        Dim tb As TabClass = _statuses.GetTabByType(TabUsageType.Mentions)
        If SettingDialog.ReplyIconState <> REPLY_ICONSTATE.None AndAlso tb IsNot Nothing AndAlso tb.UnreadCount > 0 Then
            If blinkCnt > 0 Then Exit Sub
            blink = Not blink
            If blink OrElse SettingDialog.ReplyIconState = REPLY_ICONSTATE.StaticIcon Then
                NotifyIcon1.Icon = ReplyIcon
            Else
                NotifyIcon1.Icon = ReplyIconBlink
            End If
            idle = False
            Exit Sub
        End If

        If idle Then Exit Sub
        idle = True
        '優先度：エラー→オフライン→アイドル
        'エラーは更新アイコンでクリアされる
        If _myStatusError Then
            NotifyIcon1.Icon = NIconAtRed
            Exit Sub
        End If
        If _myStatusOnline Then
            NotifyIcon1.Icon = NIconAt
        Else
            NotifyIcon1.Icon = NIconAtSmoke
        End If
    End Sub

    Private Sub TimerRefreshIcon_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerRefreshIcon.Tick
        '200ms
        Me.RefreshTasktrayIcon(False)
    End Sub

    Private Sub ContextMenuTabProperty_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuTabProperty.Opening
        '右クリックの場合はタブ名が設定済。アプリケーションキーの場合は現在のタブを対象とする
        If String.IsNullOrEmpty(_rclickTabName) OrElse sender IsNot ContextMenuTabProperty Then
            If ListTab IsNot Nothing AndAlso ListTab.SelectedTab IsNot Nothing Then
                _rclickTabName = ListTab.SelectedTab.Text
            Else
                Exit Sub
            End If
        End If

        If _statuses Is Nothing Then Exit Sub
        If _statuses.Tabs Is Nothing Then Exit Sub

        Dim tb As TabClass = _statuses.Tabs(_rclickTabName)
        If tb Is Nothing Then Exit Sub

        NotifyDispMenuItem.Checked = tb.Notify
        Me.NotifyTbMenuItem.Checked = tb.Notify

        soundfileListup = True
        SoundFileComboBox.Items.Clear()
        Me.SoundFileTbComboBox.Items.Clear()
        SoundFileComboBox.Items.Add("")
        Me.SoundFileTbComboBox.Items.Add("")
        Dim oDir As IO.DirectoryInfo = New IO.DirectoryInfo(My.Application.Info.DirectoryPath + IO.Path.DirectorySeparatorChar)
        If IO.Directory.Exists(IO.Path.Combine(My.Application.Info.DirectoryPath, "Sounds")) Then
            oDir = oDir.GetDirectories("Sounds")(0)
        End If
        For Each oFile As IO.FileInfo In oDir.GetFiles("*.wav")
            SoundFileComboBox.Items.Add(oFile.Name)
            Me.SoundFileTbComboBox.Items.Add(oFile.Name)
        Next
        Dim idx As Integer = SoundFileComboBox.Items.IndexOf(tb.SoundFile)
        If idx = -1 Then idx = 0
        SoundFileComboBox.SelectedIndex = idx
        Me.SoundFileTbComboBox.SelectedIndex = idx
        soundfileListup = False
        UreadManageMenuItem.Checked = tb.UnreadManage
        Me.UnreadMngTbMenuItem.Checked = tb.UnreadManage

        TabMenuControl(_rclickTabName)
    End Sub

    Private Sub TabMenuControl(ByVal tabName As String)
        If _statuses.Tabs(tabName).TabType <> TabUsageType.Mentions AndAlso _statuses.IsDefaultTab(tabName) Then
            FilterEditMenuItem.Enabled = True
            Me.EditRuleTbMenuItem.Enabled = True
            DeleteTabMenuItem.Enabled = False
            Me.DeleteTbMenuItem.Enabled = False
        ElseIf _statuses.Tabs(tabName).TabType = TabUsageType.Mentions Then
            FilterEditMenuItem.Enabled = True
            Me.EditRuleTbMenuItem.Enabled = True
            DeleteTabMenuItem.Enabled = False
            Me.DeleteTbMenuItem.Enabled = False
        Else
            FilterEditMenuItem.Enabled = True
            Me.EditRuleTbMenuItem.Enabled = True
            DeleteTabMenuItem.Enabled = True
            Me.DeleteTbMenuItem.Enabled = True
        End If
    End Sub

    Private Sub UreadManageMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UreadManageMenuItem.Click, UnreadMngTbMenuItem.Click
        UreadManageMenuItem.Checked = DirectCast(sender, ToolStripMenuItem).Checked
        Me.UnreadMngTbMenuItem.Checked = UreadManageMenuItem.Checked

        If String.IsNullOrEmpty(_rclickTabName) Then Exit Sub
        ChangeTabUnreadManage(_rclickTabName, UreadManageMenuItem.Checked)

        SaveConfigsTabs()
    End Sub

    Public Sub ChangeTabUnreadManage(ByVal tabName As String, ByVal isManage As Boolean)

        Dim idx As Integer
        For idx = 0 To ListTab.TabCount
            If ListTab.TabPages(idx).Text = tabName Then Exit For
        Next

        _statuses.SetTabUnreadManage(tabName, isManage)
        If SettingDialog.TabIconDisp Then
            If _statuses.Tabs(tabName).UnreadCount > 0 Then
                ListTab.TabPages(idx).ImageIndex = 0
            Else
                ListTab.TabPages(idx).ImageIndex = -1
            End If
        End If

        If _curTab.Text = tabName Then
            _itemCache = Nothing
            _postCache = Nothing
            _curList.Refresh()
        End If

        SetMainWindowTitle()
        SetStatusLabelUrl()
        If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
    End Sub

    Private Sub NotifyDispMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NotifyDispMenuItem.Click, NotifyTbMenuItem.Click
        NotifyDispMenuItem.Checked = DirectCast(sender, ToolStripMenuItem).Checked
        Me.NotifyTbMenuItem.Checked = NotifyDispMenuItem.Checked

        If String.IsNullOrEmpty(_rclickTabName) Then Exit Sub

        _statuses.Tabs(_rclickTabName).Notify = NotifyDispMenuItem.Checked

        SaveConfigsTabs()
    End Sub

    Private Sub SoundFileComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SoundFileComboBox.SelectedIndexChanged, SoundFileTbComboBox.SelectedIndexChanged
        If soundfileListup OrElse _rclickTabName = "" Then Exit Sub

        _statuses.Tabs(_rclickTabName).SoundFile = DirectCast(DirectCast(sender, ToolStripComboBox).SelectedItem, String)

        SaveConfigsTabs()
    End Sub

    Private Sub DeleteTabMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles DeleteTabMenuItem.Click, DeleteTbMenuItem.Click
        If String.IsNullOrEmpty(_rclickTabName) OrElse sender Is Me.DeleteTbMenuItem Then _rclickTabName = ListTab.SelectedTab.Text

        RemoveSpecifiedTab(_rclickTabName, True)
        SaveConfigsTabs()
    End Sub

    Private Sub FilterEditMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FilterEditMenuItem.Click, EditRuleTbMenuItem.Click

        If String.IsNullOrEmpty(_rclickTabName) Then _rclickTabName = _statuses.GetTabByType(TabUsageType.Home).TabName
        fltDialog.SetCurrent(_rclickTabName)
        fltDialog.ShowDialog()
        Me.TopMost = SettingDialog.AlwaysTop

        Try
            Me.Cursor = Cursors.WaitCursor
            _itemCache = Nothing
            _postCache = Nothing
            _curPost = Nothing
            _curItemIndex = -1
            _statuses.FilterAll()
            For Each tb As TabPage In ListTab.TabPages
                DirectCast(tb.Tag, DetailsListView).VirtualListSize = _statuses.Tabs(tb.Text).AllCount
                If _statuses.Tabs(tb.Text).UnreadCount > 0 Then
                    If SettingDialog.TabIconDisp Then
                        tb.ImageIndex = 0
                    End If
                Else
                    If SettingDialog.TabIconDisp Then
                        tb.ImageIndex = -1
                    End If
                End If
            Next
            If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
        Finally
            Me.Cursor = Cursors.Default
        End Try
        SaveConfigsTabs()
    End Sub

    Private Sub AddTabMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddTabMenuItem.Click, CreateTbMenuItem.Click
        Dim tabName As String = Nothing
        Dim tabUsage As TabUsageType
        Using inputName As New InputTabName()
            inputName.TabName = _statuses.GetUniqueTabName
            inputName.IsShowUsage = True
            inputName.ShowDialog()
            If inputName.DialogResult = Windows.Forms.DialogResult.Cancel Then Exit Sub
            tabName = inputName.TabName
            tabUsage = inputName.Usage
            inputName.Dispose()
        End Using
        Me.TopMost = SettingDialog.AlwaysTop
        If Not String.IsNullOrEmpty(tabName) Then
            'List対応
            Dim list As ListElement = Nothing
            If tabUsage = TabUsageType.Lists Then
                Using listAvail As New ListAvailable
                    If listAvail.ShowDialog(Me) = Windows.Forms.DialogResult.Cancel Then Exit Sub
                    If listAvail.SelectedList Is Nothing Then Exit Sub
                    list = listAvail.SelectedList
                End Using
            End If
            If Not AddNewTab(tabName, False, tabUsage, list) Then
                Dim tmp As String = String.Format(My.Resources.AddTabMenuItem_ClickText1, tabName)
                MessageBox.Show(tmp, My.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Else
                '成功
                _statuses.AddTab(tabName, tabUsage, list)
                SaveConfigsTabs()
                If tabUsage = TabUsageType.PublicSearch Then
                    ListTab.SelectedIndex = ListTab.TabPages.Count - 1
                    ListTabSelect(ListTab.TabPages(ListTab.TabPages.Count - 1))
                    ListTab.SelectedTab.Controls("panelSearch").Controls("comboSearch").Focus()
                End If
                If tabUsage = TabUsageType.Lists Then
                    ListTab.SelectedIndex = ListTab.TabPages.Count - 1
                    ListTabSelect(ListTab.TabPages(ListTab.TabPages.Count - 1))
                    GetTimeline(WORKERTYPE.List, 1, 0, tabName)
                End If
            End If
        End If
    End Sub

    Private Sub TabMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabMenuItem.Click, CreateTabRuleOpMenuItem.Click
        '選択発言を元にフィルタ追加
        For Each idx As Integer In _curList.SelectedIndices
            Dim tabName As String = ""
            'タブ選択（or追加）
            If Not SelectTab(tabName) Then Exit Sub

            fltDialog.SetCurrent(tabName)
            If _statuses.Item(_curTab.Text, idx).RetweetedId = 0 Then
                fltDialog.AddNewFilter(_statuses.Item(_curTab.Text, idx).ScreenName, _statuses.Item(_curTab.Text, idx).TextFromApi)
            Else
                fltDialog.AddNewFilter(_statuses.Item(_curTab.Text, idx).RetweetedBy, _statuses.Item(_curTab.Text, idx).TextFromApi)
            End If
            fltDialog.ShowDialog()
            Me.TopMost = SettingDialog.AlwaysTop
        Next

        Try
            Me.Cursor = Cursors.WaitCursor
            _itemCache = Nothing
            _postCache = Nothing
            _curPost = Nothing
            _curItemIndex = -1
            _statuses.FilterAll()
            For Each tb As TabPage In ListTab.TabPages
                DirectCast(tb.Tag, DetailsListView).VirtualListSize = _statuses.Tabs(tb.Text).AllCount
                If _statuses.Tabs(tb.Text).UnreadCount > 0 Then
                    If SettingDialog.TabIconDisp Then
                        tb.ImageIndex = 0
                    End If
                Else
                    If SettingDialog.TabIconDisp Then
                        tb.ImageIndex = -1
                    End If
                End If
            Next
            If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
        Finally
            Me.Cursor = Cursors.Default
        End Try
        SaveConfigsTabs()
        If Me.ListTab.SelectedTab IsNot Nothing AndAlso
            DirectCast(Me.ListTab.SelectedTab.Tag, DetailsListView).SelectedIndices.Count > 0 Then
            _curPost = _statuses.Item(Me.ListTab.SelectedTab.Text, DirectCast(Me.ListTab.SelectedTab.Tag, DetailsListView).SelectedIndices(0))
        End If
    End Sub

    Protected Overrides Function ProcessDialogKey( _
        ByVal keyData As Keys) As Boolean
        'TextBox1でEnterを押してもビープ音が鳴らないようにする
        If (keyData And Keys.KeyCode) = Keys.Enter Then
            If StatusText.Focused Then
                Dim _NewLine As Boolean = False
                Dim _Post As Boolean = False

                If SettingDialog.PostCtrlEnter Then 'Ctrl+Enter投稿時
                    If StatusText.Multiline Then
                        If (keyData And Keys.Shift) = Keys.Shift AndAlso (keyData And Keys.Control) <> Keys.Control Then _NewLine = True

                        If (keyData And Keys.Control) = Keys.Control Then _Post = True
                    Else
                        If ((keyData And Keys.Control) = Keys.Control) Then _Post = True
                    End If

                ElseIf SettingDialog.PostShiftEnter Then 'SHift+Enter投稿時
                    If StatusText.Multiline Then
                        If (keyData And Keys.Control) = Keys.Control AndAlso (keyData And Keys.Shift) <> Keys.Shift Then _NewLine = True

                        If (keyData And Keys.Shift) = Keys.Shift Then _Post = True
                    Else
                        If ((keyData And Keys.Shift) = Keys.Shift) Then _Post = True
                    End If

                Else 'Enter投稿時
                    If StatusText.Multiline Then
                        If (keyData And Keys.Shift) = Keys.Shift AndAlso (keyData And Keys.Control) <> Keys.Control Then _NewLine = True

                        If ((keyData And Keys.Control) <> Keys.Control AndAlso (keyData And Keys.Shift) <> Keys.Shift) OrElse _
                            ((keyData And Keys.Control) = Keys.Control AndAlso (keyData And Keys.Shift) = Keys.Shift) Then _Post = True
                    Else
                        If ((keyData And Keys.Shift) = Keys.Shift) OrElse _
                           (((keyData And Keys.Control) <> Keys.Control) AndAlso _
                            ((keyData And Keys.Shift) <> Keys.Shift)) Then _Post = True
                    End If
                End If

                If _NewLine Then
                    Dim pos1 As Integer = StatusText.SelectionStart
                    If StatusText.SelectionLength > 0 Then
                        StatusText.Text = StatusText.Text.Remove(pos1, StatusText.SelectionLength)  '選択状態文字列削除
                    End If
                    StatusText.Text = StatusText.Text.Insert(pos1, Environment.NewLine)  '改行挿入
                    StatusText.SelectionStart = pos1 + Environment.NewLine.Length    'カーソルを改行の次の文字へ移動
                    Return True
                ElseIf _Post Then
                    PostButton_Click(Nothing, Nothing)
                    Return True
                End If
            ElseIf _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.PublicSearch AndAlso _
                    (ListTab.SelectedTab.Controls("panelSearch").Controls("comboSearch").Focused OrElse _
                    ListTab.SelectedTab.Controls("panelSearch").Controls("comboLang").Focused) Then
                Me.SearchButton_Click(ListTab.SelectedTab.Controls("panelSearch").Controls("comboSearch"), Nothing)
                Return True
            End If
        End If

        Return MyBase.ProcessDialogKey(keyData)
    End Function

    Private Sub ReplyAllStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReplyAllStripMenuItem.Click, ReplyAllOpMenuItem.Click
        MakeReplyOrDirectStatus(False, True, True)
    End Sub

    Private Sub IDRuleMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IDRuleMenuItem.Click, CreateIdRuleOpMenuItem.Click
        Dim tabName As String = ""

        '未選択なら処理終了
        If _curList.SelectedIndices.Count = 0 Then Exit Sub

        'タブ選択（or追加）
        If Not SelectTab(tabName) Then Exit Sub

        Dim mv As Boolean = False
        Dim mk As Boolean = False
        MoveOrCopy(mv, mk)

        Dim ids As New List(Of String)
        For Each idx As Integer In _curList.SelectedIndices
            Dim post As PostClass = _statuses.Item(_curTab.Text, idx)
            If Not ids.Contains(post.ScreenName) Then
                Dim fc As New FiltersClass
                ids.Add(post.ScreenName)
                If post.RetweetedId = 0 Then
                    fc.NameFilter = post.ScreenName
                Else
                    fc.NameFilter = post.RetweetedBy
                End If
                fc.SearchBoth = True
                fc.MoveFrom = mv
                fc.SetMark = mk
                fc.UseRegex = False
                fc.SearchUrl = False
                _statuses.Tabs(tabName).AddFilter(fc)
            End If
        Next
        If ids.Count <> 0 Then
            Dim atids As New List(Of String)
            For Each id As String In ids
                atids.Add("@" + id)
            Next
            Dim cnt As Integer = AtIdSupl.ItemCount
            AtIdSupl.AddRangeItem(atids.ToArray)
            If AtIdSupl.ItemCount <> cnt Then _modifySettingAtId = True
        End If

        Try
            Me.Cursor = Cursors.WaitCursor
            _itemCache = Nothing
            _postCache = Nothing
            _curPost = Nothing
            _curItemIndex = -1
            _statuses.FilterAll()
            For Each tb As TabPage In ListTab.TabPages
                DirectCast(tb.Tag, DetailsListView).VirtualListSize = _statuses.Tabs(tb.Text).AllCount
                If _statuses.ContainsTab(tb.Text) Then
                    If _statuses.Tabs(tb.Text).UnreadCount > 0 Then
                        If SettingDialog.TabIconDisp Then
                            tb.ImageIndex = 0
                        End If
                    Else
                        If SettingDialog.TabIconDisp Then
                            tb.ImageIndex = -1
                        End If
                    End If
                End If
            Next
            If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
        Finally
            Me.Cursor = Cursors.Default
        End Try
        SaveConfigsTabs()
    End Sub

    Private Function SelectTab(ByRef tabName As String) As Boolean
        Do
            '振り分け先タブ選択
            If TabDialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then
                Me.TopMost = SettingDialog.AlwaysTop
                Return False
            End If
            Me.TopMost = SettingDialog.AlwaysTop
            tabName = TabDialog.SelectedTabName

            ListTab.SelectedTab.Focus()
            '新規タブを選択→タブ作成
            If tabName = My.Resources.IDRuleMenuItem_ClickText1 Then
                Using inputName As New InputTabName()
                    inputName.TabName = _statuses.GetUniqueTabName
                    inputName.ShowDialog()
                    If inputName.DialogResult = Windows.Forms.DialogResult.Cancel Then Return False
                    tabName = inputName.TabName
                    inputName.Dispose()
                End Using
                Me.TopMost = SettingDialog.AlwaysTop
                If Not String.IsNullOrEmpty(tabName) Then
                    If Not AddNewTab(tabName, False, TabUsageType.UserDefined) Then
                        Dim tmp As String = String.Format(My.Resources.IDRuleMenuItem_ClickText2, tabName)
                        MessageBox.Show(tmp, My.Resources.IDRuleMenuItem_ClickText3, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        'もう一度タブ名入力
                    Else
                        _statuses.AddTab(tabName, TabUsageType.UserDefined, Nothing)
                        Return True
                    End If
                End If
            Else
                '既存タブを選択
                Return True
            End If
        Loop While True

    End Function

    Private Sub MoveOrCopy(ByRef move As Boolean, ByRef mark As Boolean)
        With Block
            '移動するか？
            Dim _tmp As String = String.Format(My.Resources.IDRuleMenuItem_ClickText4, Environment.NewLine)
            If MessageBox.Show(_tmp, My.Resources.IDRuleMenuItem_ClickText5, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                move = False
            Else
                move = True
            End If
        End With
        If Not move Then
            'マークするか？
            Dim _tmp As String = String.Format(My.Resources.IDRuleMenuItem_ClickText6, vbCrLf)
            If MessageBox.Show(_tmp, My.Resources.IDRuleMenuItem_ClickText7, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                mark = True
            Else
                mark = False
            End If
        End If
    End Sub
    Private Sub CopySTOTMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopySTOTMenuItem.Click
        Me.CopyStot()
    End Sub

    Private Sub CopyURLMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyURLMenuItem.Click
        Me.CopyIdUri()
    End Sub

    Private Sub SelectAllMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectAllMenuItem.Click, SelAllOpMenuItem.Click
        If StatusText.Focused Then
            ' 発言欄でのCtrl+A
            StatusText.SelectAll()
        Else
            ' ListView上でのCtrl+A
            For i As Integer = 0 To _curList.VirtualListSize - 1
                _curList.SelectedIndices.Add(i)
            Next
        End If
    End Sub

    Private Sub MoveMiddle()
        Dim _item As ListViewItem
        Dim idx1 As Integer
        Dim idx2 As Integer

        If _curList.SelectedIndices.Count = 0 Then Exit Sub

        Dim idx As Integer = _curList.SelectedIndices(0)

        _item = _curList.GetItemAt(0, 25)
        If _item Is Nothing Then
            idx1 = 0
        Else
            idx1 = _item.Index
        End If
        _item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1)
        If _item Is Nothing Then
            idx2 = _curList.VirtualListSize - 1
        Else
            idx2 = _item.Index
        End If

        idx -= Math.Abs(idx1 - idx2) \ 2
        If idx < 0 Then idx = 0

        _curList.EnsureVisible(_curList.VirtualListSize - 1)
        _curList.EnsureVisible(idx)
    End Sub

    Private Sub OpenURLMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenURLMenuItem.Click, OpenUrlOpMenuItem.Click
        If PostBrowser.Document.Links.Count > 0 Then
            UrlDialog.ClearUrl()

            Dim openUrlStr As String = ""

            If PostBrowser.Document.Links.Count = 1 Then
                Dim urlStr As String = ""
                Try
                    urlStr = IDNDecode(PostBrowser.Document.Links(0).GetAttribute("href"))
                Catch ex As ArgumentException
                    '変なHTML？
                    Exit Sub
                End Try
                If String.IsNullOrEmpty(urlStr) Then Exit Sub
                openUrlStr = urlEncodeMultibyteChar(urlStr)
            Else
                For Each linkElm As HtmlElement In PostBrowser.Document.Links
                    Dim urlStr As String = ""
                    Dim linkText As String = ""
                    Try
                        urlStr = IDNDecode(linkElm.GetAttribute("href"))
                        linkText = linkElm.InnerText
                        If Not linkText.StartsWith("http") AndAlso Not linkText.StartsWith("#") Then
                            linkText = "@" + linkText
                        End If
                    Catch ex As ArgumentException
                        '変なHTML？
                        Exit Sub
                    End Try
                    If String.IsNullOrEmpty(urlStr) Then Continue For
                    UrlDialog.AddUrl(New OpenUrlItem(linkText, urlEncodeMultibyteChar(urlStr)))
                Next
                Try
                    If UrlDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                        openUrlStr = UrlDialog.SelectedUrl
                    End If
                Catch ex As Exception
                    Exit Sub
                End Try
                Me.TopMost = SettingDialog.AlwaysTop
            End If
            If String.IsNullOrEmpty(openUrlStr) Then Exit Sub

            If openUrlStr.StartsWith("http://twitter.com/search?q=%23") OrElse _
               openUrlStr.StartsWith("https://twitter.com/search?q=%23") Then
                'ハッシュタグの場合は、タブで開く
                Dim urlStr As String = HttpUtility.UrlDecode(openUrlStr)
                Dim hash As String = urlStr.Substring(urlStr.IndexOf("#"))
                HashSupl.AddItem(hash)
                HashMgr.AddHashToHistory(hash.Trim, False)
                AddNewTabForSearch(hash)
                Exit Sub
            Else
                Dim m As Match = Regex.Match(openUrlStr, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$")
                If SettingDialog.OpenUserTimeline AndAlso m.Success AndAlso IsTwitterId(m.Result("${ScreenName}")) Then
                    Me.AddNewTabForUserTimeline(m.Result("${ScreenName}"))
                Else
                    OpenUriAsync(openUrlStr)
                End If
                Exit Sub
            End If

            openUrlStr = openUrlStr.Replace("://twitter.com/search?q=#", "://twitter.com/search?q=%23")
            OpenUriAsync(openUrlStr)
        End If
    End Sub

    Private Sub ClearTabMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearTabMenuItem.Click, ClearTbMenuItem.Click
        If String.IsNullOrEmpty(_rclickTabName) Then Exit Sub
        ClearTab(_rclickTabName, True)
    End Sub

    Private Sub ClearTab(ByVal tabName As String, ByVal showWarning As Boolean)
        If showWarning Then
            Dim tmp As String = String.Format(My.Resources.ClearTabMenuItem_ClickText1, Environment.NewLine)
            If MessageBox.Show(tmp, tabName + " " + My.Resources.ClearTabMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Exit Sub
            End If
        End If

        _statuses.ClearTabIds(tabName)
        If ListTab.SelectedTab.Text = tabName Then
            _anchorPost = Nothing
            _anchorFlag = False
            _itemCache = Nothing
            _postCache = Nothing
            _itemCacheIndex = -1
            _curItemIndex = -1
            _curPost = Nothing
        End If
        For Each tb As TabPage In ListTab.TabPages
            If tb.Text = tabName Then
                tb.ImageIndex = -1
                DirectCast(tb.Tag, DetailsListView).VirtualListSize = 0
                Exit For
            End If
        Next
        If Not SettingDialog.TabIconDisp Then ListTab.Refresh()

        SetMainWindowTitle()
        SetStatusLabelUrl()
    End Sub

    Private Sub SetMainWindowTitle()
        'メインウインドウタイトルの書き換え
        Dim ttl As New StringBuilder(256)
        Dim ur As Integer = 0
        Dim al As Integer = 0
        Static myVer As String = fileVersion
        Static followers As Long = 0
        If SettingDialog.DispLatestPost <> DispTitleEnum.None AndAlso _
           SettingDialog.DispLatestPost <> DispTitleEnum.Post AndAlso _
           SettingDialog.DispLatestPost <> DispTitleEnum.Ver AndAlso _
           SettingDialog.DispLatestPost <> DispTitleEnum.OwnStatus Then
            For Each key As String In _statuses.Tabs.Keys
                ur += _statuses.Tabs(key).UnreadCount
                al += _statuses.Tabs(key).AllCount
            Next
        End If

        If SettingDialog.DispUsername Then ttl.Append(tw.Username).Append(" - ")
        ttl.Append("Tween  ")
        Select Case SettingDialog.DispLatestPost
            Case DispTitleEnum.Ver
                ttl.Append("Ver:").Append(myVer)
            Case DispTitleEnum.Post
                If _history IsNot Nothing AndAlso _history.Count > 1 Then
                    ttl.Append(_history(_history.Count - 2).status.Replace(vbCrLf, ""))
                End If
            Case DispTitleEnum.UnreadRepCount
                ttl.AppendFormat(My.Resources.SetMainWindowTitleText1, _statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + _statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount)
            Case DispTitleEnum.UnreadAllCount
                ttl.AppendFormat(My.Resources.SetMainWindowTitleText2, ur)
            Case DispTitleEnum.UnreadAllRepCount
                ttl.AppendFormat(My.Resources.SetMainWindowTitleText3, ur, _statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + _statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount)
            Case DispTitleEnum.UnreadCountAllCount
                ttl.AppendFormat(My.Resources.SetMainWindowTitleText4, ur, al)
            Case DispTitleEnum.OwnStatus
                If followers = 0 AndAlso tw.FollowersCount > 0 Then followers = tw.FollowersCount
                ttl.AppendFormat(My.Resources.OwnStatusTitle, tw.StatusesCount, tw.FriendsCount, tw.FollowersCount, tw.FollowersCount - followers)
        End Select

        Try
            Me.Text = ttl.ToString()
        Catch ex As AccessViolationException
            '原因不明。ポスト内容に依存か？たまーに発生するが再現せず。
        End Try
    End Sub

    Private Function GetStatusLabelText() As String
        'ステータス欄にカウント表示
        'タブ未読数/タブ発言数 全未読数/総発言数 (未読＠＋未読DM数)
        If _statuses Is Nothing Then Return ""
        Dim tbRep As TabClass = _statuses.GetTabByType(TabUsageType.Mentions)
        Dim tbDm As TabClass = _statuses.GetTabByType(TabUsageType.DirectMessage)
        If tbRep Is Nothing OrElse tbDm Is Nothing Then Return ""
        Dim urat As Integer = tbRep.UnreadCount + tbDm.UnreadCount
        Dim ur As Integer = 0
        Dim al As Integer = 0
        Dim tur As Integer = 0
        Dim tal As Integer = 0
        Dim slbl As StringBuilder = New StringBuilder(256)
        Try
            For Each key As String In _statuses.Tabs.Keys
                ur += _statuses.Tabs(key).UnreadCount
                al += _statuses.Tabs(key).AllCount
                If key.Equals(_curTab.Text) Then
                    tur = _statuses.Tabs(key).UnreadCount
                    tal = _statuses.Tabs(key).AllCount
                End If
            Next
        Catch ex As Exception
            Return ""
        End Try

        UnreadCounter = ur
        UnreadAtCounter = urat

        slbl.AppendFormat(My.Resources.SetStatusLabelText1, tur, tal, ur, al, urat, _postTimestamps.Count, _favTimestamps.Count, _tlCount)
        If SettingDialog.TimelinePeriodInt = 0 Then
            slbl.Append(My.Resources.SetStatusLabelText2)
        Else
            slbl.Append(SettingDialog.TimelinePeriodInt.ToString() + My.Resources.SetStatusLabelText3)
        End If
        Return slbl.ToString()
    End Function

    Delegate Sub SetStatusLabelApiDelegate()

    Private Sub SetStatusLabelApiHandler(ByVal sender As Object, ByVal e As ApiInformationChangedEventArgs)
        If InvokeRequired AndAlso Not IsDisposed Then
            Invoke(New SetStatusLabelApiDelegate(AddressOf SetStatusLabelApi))
        Else
            SetStatusLabelApi()
        End If
    End Sub

    Private Sub SetStatusLabelApi()
        Me._apiGauge.RemainCount = TwitterApiInfo.RemainCount
        Me._apiGauge.MaxCount = TwitterApiInfo.MaxCount
        Me._apiGauge.ResetTime = TwitterApiInfo.ResetTime
    End Sub

    Private Sub SetStatusLabelUrl()
        StatusLabelUrl.Text = GetStatusLabelText()
    End Sub

    Public Sub SetStatusLabel(ByVal text As String)
        StatusLabel.Text = text
    End Sub

    Private Sub SetNotifyIconText()
        ' タスクトレイアイコンのツールチップテキスト書き換え
        ' Tween [未読/@]
        Static ur As New StringBuilder(64)
        ur.Remove(0, ur.Length)
        If SettingDialog.DispUsername Then
            ur.Append(tw.Username)
            ur.Append(" - ")
        End If
        ur.Append("Tween")
#If DEBUG Then
        ur.Append("(Debug Build)")
#End If
        If UnreadCounter <> -1 AndAlso UnreadAtCounter <> -1 Then
            ur.Append(" [")
            ur.Append(UnreadCounter)
            ur.Append("/@")
            ur.Append(UnreadAtCounter)
            ur.Append("]")
        End If
        NotifyIcon1.Text = ur.ToString()
    End Sub

    Friend Sub CheckReplyTo(ByVal StatusText As String)
        Dim m As MatchCollection
        'ハッシュタグの保存
        m = Regex.Matches(StatusText, "(^|[^a-zA-Z0-9_/])(#|＃)(?<hash>[a-zA-Z0-9_]+)")
        Dim hstr As String = ""
        For Each hm As Match In m
            If Not IsNumeric(hm.Result("${hash}")) Then
                If Not hstr.Contains("#" + hm.Result("${hash}") + " ") Then
                    hstr += "#" + hm.Result("${hash}") + " "
                    HashSupl.AddItem("#" + hm.Result("${hash}"))
                End If
            End If
        Next
        If Not String.IsNullOrEmpty(HashMgr.UseHash) AndAlso Not hstr.Contains(HashMgr.UseHash + " ") Then
            hstr += HashMgr.UseHash
        End If
        If Not String.IsNullOrEmpty(hstr) Then HashMgr.AddHashToHistory(hstr.Trim, False)

        ' 本当にリプライ先指定すべきかどうかの判定
        m = Regex.Matches(StatusText, "(^|[ -/:-@[-^`{-~])(?<id>@[a-zA-Z0-9_]+)")

        If SettingDialog.UseAtIdSupplement Then
            Dim bCnt As Integer = AtIdSupl.ItemCount
            For Each mid As Match In m
                AtIdSupl.AddItem(mid.Result("${id}"))
            Next
            If bCnt <> AtIdSupl.ItemCount Then _modifySettingAtId = True
        End If

        ' リプライ先ステータスIDの指定がない場合は指定しない
        If _reply_to_id = 0 Then Exit Sub

        ' リプライ先ユーザー名がない場合も指定しない
        If String.IsNullOrEmpty(_reply_to_name) Then
            _reply_to_id = 0
            Exit Sub
        End If

        ' 通常Reply
        ' 次の条件を満たす場合に in_reply_to_status_id 指定
        ' 1. Twitterによりリンクと判定される @idが文中に1つ含まれる (2009/5/28 リンク化される@IDのみカウントするように修正)
        ' 2. リプライ先ステータスIDが設定されている(リストをダブルクリックで返信している)
        ' 3. 文中に含まれた@idがリプライ先のポスト者のIDと一致する

        If m IsNot Nothing Then
            If StatusText.StartsWith("@") Then
                If StatusText.StartsWith("@" + _reply_to_name) Then Exit Sub
            Else
                For Each mid As Match In m
                    If StatusText.Contains("QT " + mid.Result("${id}") + ":") AndAlso mid.Result("${id}") = "@" + _reply_to_name Then Exit Sub
                Next
            End If
        End If

        _reply_to_id = 0
        _reply_to_name = ""

    End Sub

    Private Sub TweenMain_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        If Not _initialLayout AndAlso SettingDialog.MinimizeToTray AndAlso WindowState = FormWindowState.Minimized Then
            Me.Visible = False
        End If
        If _initialLayout AndAlso _cfgLocal IsNot Nothing AndAlso Me.WindowState = FormWindowState.Normal AndAlso Me.Visible Then
            Me.ClientSize = _cfgLocal.FormSize
            '_mySize = Me.ClientSize                     'サイズ保持（最小化・最大化されたまま終了した場合の対応用）
            Me.DesktopLocation = _cfgLocal.FormLocation
            '_myLoc = Me.DesktopLocation                        '位置保持（最小化・最大化されたまま終了した場合の対応用）
            If _cfgLocal.SplitterDistance > Me.SplitContainer1.Panel1MinSize AndAlso _cfgLocal.SplitterDistance < Me.SplitContainer1.Height - Me.SplitContainer1.Panel2MinSize - Me.SplitContainer1.SplitterWidth Then
                Me.SplitContainer1.SplitterDistance = _cfgLocal.SplitterDistance 'Splitterの位置設定
            End If
            '発言欄複数行
            StatusText.Multiline = _cfgLocal.StatusMultiline
            If StatusText.Multiline Then
                Dim dis As Integer = SplitContainer2.Height - _cfgLocal.StatusTextHeight - SplitContainer2.SplitterWidth
                If dis > SplitContainer2.Panel1MinSize AndAlso dis < SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth Then
                    SplitContainer2.SplitterDistance = SplitContainer2.Height - _cfgLocal.StatusTextHeight - SplitContainer2.SplitterWidth
                End If
                StatusText.Height = _cfgLocal.StatusTextHeight
            Else
                If SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth > 0 Then
                    SplitContainer2.SplitterDistance = SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth
                End If
            End If
            If _cfgLocal.PreviewDistance > Me.SplitContainer3.Panel1MinSize AndAlso _cfgLocal.PreviewDistance < Me.SplitContainer3.Width - Me.SplitContainer3.Panel2MinSize - Me.SplitContainer3.SplitterWidth Then
                Me.SplitContainer3.SplitterDistance = _cfgLocal.PreviewDistance
            End If
            _initialLayout = False
        End If
    End Sub

    Private Sub PlaySoundMenuItem_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlaySoundMenuItem.CheckedChanged, PlaySoundFileMenuItem.CheckStateChanged
        PlaySoundMenuItem.Checked = DirectCast(sender, ToolStripMenuItem).Checked
        Me.PlaySoundFileMenuItem.Checked = PlaySoundMenuItem.Checked
        If PlaySoundMenuItem.Checked Then
            SettingDialog.PlaySound = True
        Else
            SettingDialog.PlaySound = False
        End If
        _modifySettingCommon = True
    End Sub

    Private Sub SplitContainer1_SplitterMoved(ByVal sender As Object, ByVal e As System.Windows.Forms.SplitterEventArgs) Handles SplitContainer1.SplitterMoved
        If Me.WindowState = FormWindowState.Normal AndAlso Not _initialLayout Then
            _mySpDis = SplitContainer1.SplitterDistance
            If StatusText.Multiline Then _mySpDis2 = StatusText.Height
            _modifySettingLocal = True
        End If
    End Sub

    Private Sub doRepliedStatusOpen()
        If Me.ExistCurrentPost AndAlso _curPost.InReplyToUser IsNot Nothing AndAlso _curPost.InReplyToStatusId > 0 Then
            If My.Computer.Keyboard.ShiftKeyDown Then
                OpenUriAsync("http://twitter.com/" + _curPost.InReplyToUser + "/status/" + _curPost.InReplyToStatusId.ToString())
                Exit Sub
            End If
            If _statuses.ContainsKey(_curPost.InReplyToStatusId) Then
                Dim repPost As PostClass = _statuses.Item(_curPost.InReplyToStatusId)
                MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi)
            Else
                For Each tb As TabClass In _statuses.GetTabsByType(TabUsageType.Lists Or TabUsageType.PublicSearch)
                    If tb Is Nothing OrElse Not tb.Contains(_curPost.InReplyToStatusId) Then Exit For
                    Dim repPost As PostClass = _statuses.Item(_curPost.InReplyToStatusId)
                    MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi)
                    Exit Sub
                Next
                OpenUriAsync("http://twitter.com/" + _curPost.InReplyToUser + "/status/" + _curPost.InReplyToStatusId.ToString())
            End If
        End If
    End Sub

    Private Sub RepliedStatusOpenMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RepliedStatusOpenMenuItem.Click, OpenRepSourceOpMenuItem.Click
        doRepliedStatusOpen()
    End Sub

    Private Sub ContextMenuUserPicture_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuUserPicture.Opening
        '発言詳細のアイコン右クリック時のメニュー制御
        If _curList.SelectedIndices.Count > 0 AndAlso _curPost IsNot Nothing Then
            Dim name As String = _curPost.ImageUrl
            If name IsNot Nothing AndAlso name.Length > 0 Then
                Dim idx As Integer = name.LastIndexOf("/"c)
                If idx <> -1 Then
                    name = IO.Path.GetFileName(name.Substring(idx))
                    If name.Contains("_normal.") Then
                        name = name.Replace("_normal", "")
                        Me.IconNameToolStripMenuItem.Text = name
                        Me.IconNameToolStripMenuItem.Enabled = True
                    Else
                        Me.IconNameToolStripMenuItem.Enabled = False
                        Me.IconNameToolStripMenuItem.Text = My.Resources.ContextMenuStrip3_OpeningText1
                    End If
                Else
                    Me.IconNameToolStripMenuItem.Enabled = False
                    Me.IconNameToolStripMenuItem.Text = My.Resources.ContextMenuStrip3_OpeningText1
                End If
                If Me.TIconDic.ContainsKey(_curPost.ImageUrl) AndAlso Me.TIconDic(_curPost.ImageUrl) IsNot Nothing Then
                    Me.SaveIconPictureToolStripMenuItem.Enabled = True
                Else
                    Me.SaveIconPictureToolStripMenuItem.Enabled = False
                End If
            Else
                Me.IconNameToolStripMenuItem.Enabled = False
                Me.SaveIconPictureToolStripMenuItem.Enabled = False
                Me.IconNameToolStripMenuItem.Text = My.Resources.ContextMenuStrip3_OpeningText1
            End If
        Else
            Me.IconNameToolStripMenuItem.Enabled = False
            Me.SaveIconPictureToolStripMenuItem.Enabled = False
            Me.IconNameToolStripMenuItem.Text = My.Resources.ContextMenuStrip3_OpeningText2
        End If
        If NameLabel.Tag IsNot Nothing Then
            Dim id As String = DirectCast(NameLabel.Tag, String)
            If id = tw.Username Then
                FollowToolStripMenuItem.Enabled = False
                UnFollowToolStripMenuItem.Enabled = False
                ShowFriendShipToolStripMenuItem.Enabled = False
                ShowUserStatusToolStripMenuItem.Enabled = True
                SearchPostsDetailNameToolStripMenuItem.Enabled = True
                SearchAtPostsDetailNameToolStripMenuItem.Enabled = False
                ListManageUserContextToolStripMenuItem3.Enabled = True
            Else
                FollowToolStripMenuItem.Enabled = True
                UnFollowToolStripMenuItem.Enabled = True
                ShowFriendShipToolStripMenuItem.Enabled = True
                ShowUserStatusToolStripMenuItem.Enabled = True
                SearchPostsDetailNameToolStripMenuItem.Enabled = True
                SearchAtPostsDetailNameToolStripMenuItem.Enabled = True
                ListManageUserContextToolStripMenuItem3.Enabled = True
            End If
        Else
            FollowToolStripMenuItem.Enabled = False
            UnFollowToolStripMenuItem.Enabled = False
            ShowFriendShipToolStripMenuItem.Enabled = False
            ShowUserStatusToolStripMenuItem.Enabled = False
            SearchPostsDetailNameToolStripMenuItem.Enabled = False
            SearchAtPostsDetailNameToolStripMenuItem.Enabled = False
            ListManageUserContextToolStripMenuItem3.Enabled = False
        End If
    End Sub

    Private Sub IconNameToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IconNameToolStripMenuItem.Click
        If _curPost Is Nothing Then Exit Sub
        Dim name As String = _curPost.ImageUrl
        OpenUriAsync(name.Remove(name.LastIndexOf("_normal"), 7)) ' "_normal".Length
    End Sub

    Private Sub SaveOriginalSizeIconPictureToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If _curPost Is Nothing Then Exit Sub
        Dim name As String = _curPost.ImageUrl
        name = IO.Path.GetFileNameWithoutExtension(name.Substring(name.LastIndexOf("/"c)))

        Me.SaveFileDialog1.FileName = name.Substring(0, name.Length - 8) ' "_normal".Length + 1

        If Me.SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            ' STUB
        End If
    End Sub

    Private Sub SaveIconPictureToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveIconPictureToolStripMenuItem.Click
        If _curPost Is Nothing Then Exit Sub
        Dim name As String = _curPost.ImageUrl

        Me.SaveFileDialog1.FileName = name.Substring(name.LastIndexOf("/"c) + 1)

        If Me.SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Using bmp2 As New Bitmap(TIconDic(name).Size.Width, TIconDic(name).Size.Height)
                Using g As Graphics = Graphics.FromImage(bmp2)
                    g.InterpolationMode = Drawing2D.InterpolationMode.High
                    g.DrawImage(TIconDic(name), 0, 0, TIconDic(name).Size.Width, TIconDic(name).Size.Height)
                    g.Dispose()
                End Using
                bmp2.Save(Me.SaveFileDialog1.FileName)
                bmp2.Dispose()
            End Using
        End If
    End Sub

    Private Sub SplitContainer2_Panel2_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SplitContainer2.Panel2.Resize
        Me.StatusText.Multiline = Me.SplitContainer2.Panel2.Height > Me.SplitContainer2.Panel2MinSize + 2
        MultiLineMenuItem.Checked = Me.StatusText.Multiline
        _modifySettingLocal = True
    End Sub

    Private Sub StatusText_MultilineChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusText.MultilineChanged
        If Me.StatusText.Multiline Then
            Me.StatusText.ScrollBars = ScrollBars.Vertical
        Else
            Me.StatusText.ScrollBars = ScrollBars.None
        End If
        _modifySettingLocal = True
    End Sub

    Private Sub MultiLineMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MultiLineMenuItem.Click
        '発言欄複数行
        StatusText.Multiline = MultiLineMenuItem.Checked
        _cfgLocal.StatusMultiline = MultiLineMenuItem.Checked
        If MultiLineMenuItem.Checked Then
            If SplitContainer2.Height - _mySpDis2 - SplitContainer2.SplitterWidth < 0 Then
                SplitContainer2.SplitterDistance = 0
            Else
                SplitContainer2.SplitterDistance = SplitContainer2.Height - _mySpDis2 - SplitContainer2.SplitterWidth
            End If
        Else
            SplitContainer2.SplitterDistance = SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth
        End If
        _modifySettingLocal = True
    End Sub

    Private Function UrlConvert(ByVal Converter_Type As UrlConverter) As Boolean
        'Converter_Type=Nicomsの場合は、nicovideoのみ短縮する
        '参考資料 RFC3986 Uniform Resource Identifier (URI): Generic Syntax
        'Appendix A.  Collected ABNF for URI
        'http://www.ietf.org/rfc/rfc3986.txt

        Dim result As String = ""
        Const url As String = "(?<before>(?:[^\""':!=]|^|\:))" + _
                                   "(?<url>(?<protocol>https?://)" + _
                                   "(?<domain>(?:[\.-]|[^\p{P}\s])+\.[a-z]{2,}(?::[0-9]+)?)" + _
                                   "(?<path>/[a-z0-9!*'();:&=+$/%#\-_.,~@]*[a-z0-9)=#/]?)?" + _
                                   "(?<query>\?[a-z0-9!*'();:&=+$/%#\-_.,~@?]*[a-z0-9_&=#/])?)"

        Const nico As String = "^https?://[a-z]+\.(nicovideo|niconicommons|nicolive)\.jp/[a-z]+/[a-z0-9]+$"

        If StatusText.SelectionLength > 0 Then
            Dim tmp As String = StatusText.SelectedText
            ' httpから始まらない場合、ExcludeStringで指定された文字列で始まる場合は対象としない
            If tmp.StartsWith("http") Then
                ' 文字列が選択されている場合はその文字列について処理

                'nico.ms使用、nicovideoにマッチしたら変換
                If SettingDialog.Nicoms AndAlso Regex.IsMatch(tmp, nico) Then
                    result = nicoms.Shorten(tmp)
                ElseIf Converter_Type <> UrlConverter.Nicoms Then
                    '短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                    result = ShortUrl.Make(Converter_Type, tmp)
                    If result.Equals("Can't convert") Then
                        StatusLabel.Text = result.Insert(0, Converter_Type.ToString() + ":")
                        Return False
                    End If
                Else
                    Return True
                End If

                If Not String.IsNullOrEmpty(result) Then
                    Dim undotmp As New urlUndo

                    StatusText.Select(StatusText.Text.IndexOf(tmp, StringComparison.Ordinal), tmp.Length)
                    StatusText.SelectedText = result

                    'undoバッファにセット
                    undotmp.Before = tmp
                    undotmp.After = result

                    If urlUndoBuffer Is Nothing Then
                        urlUndoBuffer = New List(Of urlUndo)
                        UrlUndoToolStripMenuItem.Enabled = True
                    End If

                    urlUndoBuffer.Add(undotmp)
                End If
            End If
        Else
            ' 正規表現にマッチしたURL文字列をtinyurl化
            For Each mt As Match In Regex.Matches(StatusText.Text, url, RegexOptions.IgnoreCase)
                If StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal) = -1 Then Continue For
                Dim tmp As String = mt.Result("${url}")
                If tmp.StartsWith("w", StringComparison.OrdinalIgnoreCase) Then tmp = "http://" + tmp
                Dim undotmp As New urlUndo

                '選んだURLを選択（？）
                StatusText.Select(StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length)

                'nico.ms使用、nicovideoにマッチしたら変換
                If SettingDialog.Nicoms AndAlso Regex.IsMatch(tmp, nico) Then
                    result = nicoms.Shorten(tmp)
                ElseIf Converter_Type <> UrlConverter.Nicoms Then
                    '短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                    result = ShortUrl.Make(Converter_Type, tmp)
                    If result.Equals("Can't convert") Then
                        StatusLabel.Text = result.Insert(0, Converter_Type.ToString() + ":")
                        Continue For
                    End If
                Else
                    Continue For
                End If

                If Not String.IsNullOrEmpty(result) Then
                    StatusText.Select(StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length)
                    StatusText.SelectedText = result
                    'undoバッファにセット
                    undotmp.Before = mt.Result("${url}")
                    undotmp.After = result

                    If urlUndoBuffer Is Nothing Then
                        urlUndoBuffer = New List(Of urlUndo)
                        UrlUndoToolStripMenuItem.Enabled = True
                    End If

                    urlUndoBuffer.Add(undotmp)
                End If
            Next
        End If

        Return True

    End Function

    Private Sub doUrlUndo()
        If urlUndoBuffer IsNot Nothing Then
            Dim tmp As String = StatusText.Text
            For Each data As urlUndo In urlUndoBuffer
                tmp = tmp.Replace(data.After, data.Before)
            Next
            StatusText.Text = tmp
            urlUndoBuffer = Nothing
            UrlUndoToolStripMenuItem.Enabled = False
        End If
    End Sub

    Private Sub TinyURLToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TinyURLToolStripMenuItem.Click
        UrlConvert(UrlConverter.TinyUrl)
    End Sub

    Private Sub IsgdToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IsgdToolStripMenuItem.Click
        UrlConvert(UrlConverter.Isgd)
    End Sub

    Private Sub TwurlnlToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TwurlnlToolStripMenuItem.Click
        UrlConvert(UrlConverter.Twurl)
    End Sub

    Private Sub UxnuMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UxnuMenuItem.Click
        UrlConvert(UrlConverter.Uxnu)
    End Sub

    Private Sub UrlConvertAutoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UrlConvertAutoToolStripMenuItem.Click
        If Not UrlConvert(SettingDialog.AutoShortUrlFirst) Then
            Dim svc As UrlConverter = SettingDialog.AutoShortUrlFirst
            Dim rnd As New Random()
            ' 前回使用した短縮URLサービス以外を選択する
            Do
                svc = CType(rnd.Next(System.Enum.GetNames(GetType(UrlConverter)).Length), UrlConverter)
            Loop Until svc <> SettingDialog.AutoShortUrlFirst AndAlso svc <> UrlConverter.Nicoms AndAlso svc <> UrlConverter.Unu
            UrlConvert(svc)
        End If
    End Sub

    Private Sub UrlUndoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UrlUndoToolStripMenuItem.Click
        doUrlUndo()
    End Sub

    Private Sub NewPostPopMenuItem_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewPostPopMenuItem.CheckStateChanged, NotifyFileMenuItem.CheckStateChanged
        Me.NotifyFileMenuItem.Checked = DirectCast(sender, ToolStripMenuItem).Checked
        Me.NewPostPopMenuItem.Checked = Me.NotifyFileMenuItem.Checked
        _cfgCommon.NewAllPop = NewPostPopMenuItem.Checked
        _modifySettingCommon = True
    End Sub

    Private Sub ListLockMenuItem_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListLockMenuItem.CheckStateChanged, LockListFileMenuItem.CheckStateChanged
        ListLockMenuItem.Checked = DirectCast(sender, ToolStripMenuItem).Checked
        Me.LockListFileMenuItem.Checked = ListLockMenuItem.Checked
        _cfgCommon.ListLock = ListLockMenuItem.Checked
        _modifySettingCommon = True
    End Sub

    Private Sub MenuStrip1_MenuActivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuStrip1.MenuActivate
        ' フォーカスがメニューに移る (MenuStrip1.Tag フラグを立てる)
        MenuStrip1.Tag = New Object()
        MenuStrip1.Select() ' StatusText がフォーカスを持っている場合 Leave が発生
    End Sub

    Private Sub MenuStrip1_MenuDeactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuStrip1.MenuDeactivate
        If Me.Tag IsNot Nothing Then ' 設定された戻り先へ遷移
            If Me.Tag Is Me.ListTab.SelectedTab Then
                DirectCast(Me.ListTab.SelectedTab.Tag, Control).Select()
            Else
                DirectCast(Me.Tag, Control).Select()
            End If
        Else ' 戻り先が指定されていない (初期状態) 場合はタブに遷移
            If ListTab.SelectedIndex > -1 AndAlso ListTab.SelectedTab.HasChildren Then
                Me.Tag = ListTab.SelectedTab.Tag
                DirectCast(Me.Tag, Control).Select()
            End If
        End If
        ' フォーカスがメニューに遷移したかどうかを表すフラグを降ろす
        MenuStrip1.Tag = Nothing
    End Sub

    Private Sub MyList_ColumnReordered(ByVal sender As System.Object, ByVal e As ColumnReorderedEventArgs)
        Dim lst As DetailsListView = DirectCast(sender, DetailsListView)
        If _cfgLocal Is Nothing Then Exit Sub

        If _iconCol Then
            _cfgLocal.Width1 = lst.Columns(0).Width
            _cfgLocal.Width3 = lst.Columns(1).Width
        Else
            Dim darr(lst.Columns.Count - 1) As Integer
            For i As Integer = 0 To lst.Columns.Count - 1
                darr(lst.Columns(i).DisplayIndex) = i
            Next
            MoveArrayItem(darr, e.OldDisplayIndex, e.NewDisplayIndex)

            For i As Integer = 0 To lst.Columns.Count - 1
                Select Case darr(i)
                    Case 0
                        _cfgLocal.DisplayIndex1 = i
                    Case 1
                        _cfgLocal.DisplayIndex2 = i
                    Case 2
                        _cfgLocal.DisplayIndex3 = i
                    Case 3
                        _cfgLocal.DisplayIndex4 = i
                    Case 4
                        _cfgLocal.DisplayIndex5 = i
                    Case 5
                        _cfgLocal.DisplayIndex6 = i
                    Case 6
                        _cfgLocal.DisplayIndex7 = i
                    Case 7
                        _cfgLocal.DisplayIndex8 = i
                End Select
            Next
            _cfgLocal.Width1 = lst.Columns(0).Width
            _cfgLocal.Width2 = lst.Columns(1).Width
            _cfgLocal.Width3 = lst.Columns(2).Width
            _cfgLocal.Width4 = lst.Columns(3).Width
            _cfgLocal.Width5 = lst.Columns(4).Width
            _cfgLocal.Width6 = lst.Columns(5).Width
            _cfgLocal.Width7 = lst.Columns(6).Width
            _cfgLocal.Width8 = lst.Columns(7).Width
        End If
        _modifySettingLocal = True
        _isColumnChanged = True
    End Sub

    Private Sub MyList_ColumnWidthChanged(ByVal sender As System.Object, ByVal e As ColumnWidthChangedEventArgs)
        Dim lst As DetailsListView = DirectCast(sender, DetailsListView)
        If _cfgLocal Is Nothing Then Exit Sub
        If _iconCol Then
            If _cfgLocal.Width1 <> lst.Columns(0).Width Then
                _cfgLocal.Width1 = lst.Columns(0).Width
                _modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width3 <> lst.Columns(1).Width Then
                _cfgLocal.Width3 = lst.Columns(1).Width
                _modifySettingLocal = True
                _isColumnChanged = True
            End If
        Else
            If _cfgLocal.Width1 <> lst.Columns(0).Width Then
                _cfgLocal.Width1 = lst.Columns(0).Width
                _modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width2 <> lst.Columns(1).Width Then
                _cfgLocal.Width2 = lst.Columns(1).Width
                _modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width3 <> lst.Columns(2).Width Then
                _cfgLocal.Width3 = lst.Columns(2).Width
                _modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width4 <> lst.Columns(3).Width Then
                _cfgLocal.Width4 = lst.Columns(3).Width
                _modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width5 <> lst.Columns(4).Width Then
                _cfgLocal.Width5 = lst.Columns(4).Width
                _modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width6 <> lst.Columns(5).Width Then
                _cfgLocal.Width6 = lst.Columns(5).Width
                _modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width7 <> lst.Columns(6).Width Then
                _cfgLocal.Width7 = lst.Columns(6).Width
                _modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width8 <> lst.Columns(7).Width Then
                _cfgLocal.Width8 = lst.Columns(7).Width
                _modifySettingLocal = True
                _isColumnChanged = True
            End If
        End If
        ' 非表示の時にColumnChangedが呼ばれた場合はForm初期化処理中なので保存しない
        'If changed Then
        '    SaveConfigsLocal()
        'End If
    End Sub

    Public Function WebBrowser_GetSelectionText(ByRef ComponentInstance As WebBrowser) As String
        '発言詳細で「選択文字列をコピー」を行う
        'WebBrowserコンポーネントのインスタンスを渡す
        Dim typ As Type = ComponentInstance.ActiveXInstance.GetType()
        Dim _SelObj As Object = typ.InvokeMember("selection", BindingFlags.GetProperty, Nothing, ComponentInstance.Document.DomDocument, Nothing)
        Dim _objRange As Object = _SelObj.GetType().InvokeMember("createRange", BindingFlags.InvokeMethod, Nothing, _SelObj, Nothing)
        Return DirectCast(_objRange.GetType().InvokeMember("text", BindingFlags.GetProperty, Nothing, _objRange, Nothing), String)
    End Function

    Private Sub SelectionCopyContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectionCopyContextMenuItem.Click
        '発言詳細で「選択文字列をコピー」
        Dim _selText As String = WebBrowser_GetSelectionText(PostBrowser)
        Try
            Clipboard.SetDataObject(_selText, False, 5, 100)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub doSearchToolStrip(ByVal url As String)
        '発言詳細で「選択文字列で検索」（選択文字列取得）
        Dim _selText As String = WebBrowser_GetSelectionText(PostBrowser)

        If _selText IsNot Nothing Then
            If url = My.Resources.SearchItem4Url Then
                '公式検索
                AddNewTabForSearch(_selText)
                Exit Sub
            End If

            Dim tmp As String = String.Format(url, HttpUtility.UrlEncode(_selText))
            OpenUriAsync(tmp)
        End If
    End Sub

    Private Sub SelectionAllContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectionAllContextMenuItem.Click
        '発言詳細ですべて選択
        PostBrowser.Document.ExecCommand("SelectAll", False, Nothing)
    End Sub

    Private Sub SearchWikipediaContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchWikipediaContextMenuItem.Click
        doSearchToolStrip(My.Resources.SearchItem1Url)
    End Sub

    Private Sub SearchGoogleContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchGoogleContextMenuItem.Click
        doSearchToolStrip(My.Resources.SearchItem2Url)
    End Sub

    Private Sub SearchYatsContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchYatsContextMenuItem.Click
        doSearchToolStrip(My.Resources.SearchItem3Url)
    End Sub

    Private Sub SearchPublicSearchContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchPublicSearchContextMenuItem.Click
        doSearchToolStrip(My.Resources.SearchItem4Url)
    End Sub

    Private Sub UrlCopyContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UrlCopyContextMenuItem.Click
        Try
            Clipboard.SetDataObject(Me._postBrowserStatusText, False, 5, 100)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub ContextMenuPostBrowser_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuPostBrowser.Opening
        ' URLコピーの項目の表示/非表示
        If PostBrowser.StatusText.StartsWith("http") Then
            Me._postBrowserStatusText = PostBrowser.StatusText
            Dim name As String = GetUserId()
            UrlCopyContextMenuItem.Enabled = True
            If name IsNot Nothing Then
                FollowContextMenuItem.Enabled = True
                RemoveContextMenuItem.Enabled = True
                FriendshipContextMenuItem.Enabled = True
                ShowUserStatusContextMenuItem.Enabled = True
                SearchPostsDetailToolStripMenuItem.Enabled = True
                IdFilterAddMenuItem.Enabled = True
                ListManageUserContextToolStripMenuItem.Enabled = True
                SearchAtPostsDetailToolStripMenuItem.Enabled = True
            Else
                FollowContextMenuItem.Enabled = False
                RemoveContextMenuItem.Enabled = False
                FriendshipContextMenuItem.Enabled = False
                ShowUserStatusContextMenuItem.Enabled = False
                SearchPostsDetailToolStripMenuItem.Enabled = False
                IdFilterAddMenuItem.Enabled = False
                ListManageUserContextToolStripMenuItem.Enabled = False
                SearchAtPostsDetailToolStripMenuItem.Enabled = False
            End If

            If Regex.IsMatch(Me._postBrowserStatusText, "^https?://twitter.com/search\?q=%23") Then
                UseHashtagMenuItem.Enabled = True
            Else
                UseHashtagMenuItem.Enabled = False
            End If
        Else
            Me._postBrowserStatusText = ""
            UrlCopyContextMenuItem.Enabled = False
            FollowContextMenuItem.Enabled = False
            RemoveContextMenuItem.Enabled = False
            FriendshipContextMenuItem.Enabled = False
            ShowUserStatusContextMenuItem.Enabled = False
            SearchPostsDetailToolStripMenuItem.Enabled = False
            SearchAtPostsDetailToolStripMenuItem.Enabled = False
            UseHashtagMenuItem.Enabled = False
            IdFilterAddMenuItem.Enabled = False
            ListManageUserContextToolStripMenuItem.Enabled = False
        End If
        ' 文字列選択されていないときは選択文字列関係の項目を非表示に
        Dim _selText As String = WebBrowser_GetSelectionText(PostBrowser)
        If _selText Is Nothing Then
            SelectionSearchContextMenuItem.Enabled = False
            SelectionCopyContextMenuItem.Enabled = False
            SelectionTranslationToolStripMenuItem.Enabled = False
        Else
            SelectionSearchContextMenuItem.Enabled = True
            SelectionCopyContextMenuItem.Enabled = True
            SelectionTranslationToolStripMenuItem.Enabled = True
        End If
        '発言内に自分以外のユーザーが含まれてればフォロー状態全表示を有効に
        Dim ma As MatchCollection = Regex.Matches(Me.PostBrowser.DocumentText, "href=""https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?""")
        Dim fAllFlag As Boolean = False
        For Each mu As Match In ma
            If mu.Result("${ScreenName}").ToLower <> tw.Username.ToLower Then
                fAllFlag = True
                Exit For
            End If
        Next
        Me.FriendshipAllMenuItem.Enabled = fAllFlag

        If _curPost Is Nothing Then
            TranslationToolStripMenuItem.Enabled = False
        Else
            TranslationToolStripMenuItem.Enabled = True
        End If

        e.Cancel = False
    End Sub

    Private Sub CurrentTabToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CurrentTabToolStripMenuItem.Click
        '発言詳細の選択文字列で現在のタブを検索
        Dim _selText As String = WebBrowser_GetSelectionText(PostBrowser)

        If _selText IsNot Nothing Then
            SearchDialog.SWord = _selText
            SearchDialog.CheckCaseSensitive = False
            SearchDialog.CheckRegex = False

            DoTabSearch(SearchDialog.SWord, _
                        SearchDialog.CheckCaseSensitive, _
                        SearchDialog.CheckRegex, _
                        SEARCHTYPE.NextSearch)
        End If
    End Sub

    Private Sub SplitContainer2_SplitterMoved(ByVal sender As Object, ByVal e As System.Windows.Forms.SplitterEventArgs) Handles SplitContainer2.SplitterMoved
        If StatusText.Multiline Then _mySpDis2 = StatusText.Height
        _modifySettingLocal = True
    End Sub

    Private Sub TweenMain_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            ImageSelectionPanel.Visible = True
            ImageSelectionPanel.Enabled = True
            TimelinePanel.Visible = False
            TimelinePanel.Enabled = False
            ImagefilePathText.Text = CType(e.Data.GetData(DataFormats.FileDrop, False), String())(0)
            ImageFromSelectedFile()
            Me.Activate()
            StatusText.Focus()
        ElseIf e.Data.GetDataPresent(DataFormats.StringFormat) Then
            Dim data As String = TryCast(e.Data.GetData(DataFormats.StringFormat, True), String)
            If data IsNot Nothing Then StatusText.Text += data
        End If
    End Sub

    Private Sub TweenMain_DragOver(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragOver
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim filename As String = CType(e.Data.GetData(DataFormats.FileDrop, False), String())(0)
            Dim fl As New FileInfo(filename)
            Dim ext As String = fl.Extension
            Dim picsvc As New PictureService(tw)

            If picsvc.IsValidExtension(ext, ImageService) AndAlso _
                    picsvc.GetMaxFileSize(ext, ImageService) >= fl.Length Then
                e.Effect = DragDropEffects.Copy
                Exit Sub
            End If
            For Each svc As String In ImageServiceCombo.Items
                If picsvc.IsValidExtension(ext, svc) AndAlso _
                        picsvc.GetMaxFileSize(ext, svc) >= fl.Length Then
                    ImageServiceCombo.SelectedItem = svc
                    e.Effect = DragDropEffects.Copy
                    Exit Sub
                Else
                    Continue For
                End If
            Next
            e.Effect = DragDropEffects.None
        ElseIf e.Data.GetDataPresent(DataFormats.StringFormat) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Public Function IsNetworkAvailable() As Boolean
        Dim nw As Boolean = True
        Try
            nw = My.Computer.Network.IsAvailable
        Catch ex As Exception
            nw = False
        End Try
        _myStatusOnline = nw
        Return nw
    End Function

    Public Sub OpenUriAsync(ByVal UriString As String)
        Dim args As New GetWorkerArg
        args.type = WORKERTYPE.OpenUri
        args.url = UriString

        RunAsync(args)
    End Sub

    Private Sub ListTabSelect(ByVal _tab As TabPage)
        SetListProperty()

        _itemCache = Nothing
        _itemCacheIndex = -1
        _postCache = Nothing

        _curTab = _tab
        _curList = DirectCast(_tab.Tag, DetailsListView)
        If _curList.SelectedIndices.Count > 0 Then
            _curItemIndex = _curList.SelectedIndices(0)
            _curPost = GetCurTabPost(_curItemIndex)
        Else
            _curItemIndex = -1
            _curPost = Nothing
        End If

        _anchorPost = Nothing
        _anchorFlag = False

        If _iconCol Then
            DirectCast(_tab.Tag, DetailsListView).Columns.Item(1).Text = ColumnText(2)
        Else
            For i As Integer = 0 To _curList.Columns.Count - 1
                DirectCast(_tab.Tag, DetailsListView).Columns.Item(i).Text = ColumnText(i)
            Next
        End If
    End Sub

    Private Sub ListTab_Selecting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles ListTab.Selecting
        ListTabSelect(e.TabPage)
    End Sub

    Private Sub SelectListItem(ByVal LView As DetailsListView, ByVal Index As Integer)
        '単一
        Dim bnd As Rectangle
        Dim flg As Boolean = False
        If LView.FocusedItem IsNot Nothing Then
            bnd = LView.FocusedItem.Bounds
            flg = True
        End If

        LView.SelectedIndices.Clear()
        LView.Items(Index).Selected = True
        LView.Items(Index).Focused = True

        If flg Then LView.Invalidate(bnd)
    End Sub

    Private Sub SelectListItem(ByVal LView As DetailsListView, ByVal Index() As Integer, ByVal FocusedIndex As Integer)
        '複数
        Dim bnd As Rectangle
        Dim flg As Boolean = False
        If LView.FocusedItem IsNot Nothing Then
            bnd = LView.FocusedItem.Bounds
            flg = True
        End If

        If Index IsNot Nothing AndAlso Index(0) > -1 Then
            LView.SelectedIndices.Clear()
            For Each idx As Integer In Index
                LView.SelectedIndices.Add(idx)
            Next
        End If
        If FocusedIndex > -1 Then
            LView.Items(FocusedIndex).Focused = True
        End If
        If flg Then LView.Invalidate(bnd)
    End Sub

    Private Sub RunAsync(ByVal args As GetWorkerArg)
        Dim bw As BackgroundWorker = Nothing
        If args.type <> WORKERTYPE.Follower Then
            For i As Integer = 0 To _bw.Length - 1
                If _bw(i) IsNot Nothing AndAlso Not _bw(i).IsBusy Then
                    bw = _bw(i)
                    Exit For
                End If
            Next
            If bw Is Nothing Then
                For i As Integer = 0 To _bw.Length - 1
                    If _bw(i) Is Nothing Then
                        _bw(i) = New BackgroundWorker
                        bw = _bw(i)
                        bw.WorkerReportsProgress = True
                        bw.WorkerSupportsCancellation = True
                        AddHandler bw.DoWork, AddressOf GetTimelineWorker_DoWork
                        AddHandler bw.ProgressChanged, AddressOf GetTimelineWorker_ProgressChanged
                        AddHandler bw.RunWorkerCompleted, AddressOf GetTimelineWorker_RunWorkerCompleted
                        Exit For
                    End If
                Next
            End If
        Else
            If _bwFollower Is Nothing Then
                _bwFollower = New BackgroundWorker
                bw = _bwFollower
                bw.WorkerReportsProgress = True
                bw.WorkerSupportsCancellation = True
                AddHandler bw.DoWork, AddressOf GetTimelineWorker_DoWork
                AddHandler bw.ProgressChanged, AddressOf GetTimelineWorker_ProgressChanged
                AddHandler bw.RunWorkerCompleted, AddressOf GetTimelineWorker_RunWorkerCompleted
            Else
                If _bwFollower.IsBusy = False Then
                    bw = _bwFollower
                End If
            End If
        End If
        If bw Is Nothing Then Exit Sub

        bw.RunWorkerAsync(args)
    End Sub

    Private Sub StartUserStream()
        AddHandler tw.NewPostFromStream, AddressOf tw_NewPostFromStream
        AddHandler tw.UserStreamStarted, AddressOf tw_UserStreamStarted
        AddHandler tw.UserStreamStopped, AddressOf tw_UserStreamStopped
        AddHandler tw.PostDeleted, AddressOf tw_PostDeleted
        AddHandler tw.UserStreamEventReceived, AddressOf tw_UserStreamEventArrived

        MenuItemUserStream.Text = "&UserStream ■"
        MenuItemUserStream.Enabled = True
        StopToolStripMenuItem.Text = "&Start"
        StopToolStripMenuItem.Enabled = True
        If SettingDialog.UserstreamStartup Then tw.StartUserStream()
    End Sub

    Private Sub TweenMain_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            PostBrowser.Url = New Uri("about:blank")
            PostBrowser.DocumentText = ""       '発言詳細部初期化
        Catch ex As Exception

        End Try

        NotifyIcon1.Visible = True

        If IsNetworkAvailable() Then
            If SettingDialog.StartupFollowers Then
                GetTimeline(WORKERTYPE.Follower, 0, 0, "")
            End If
            StartUserStream()
            _waitTimeline = True
            GetTimeline(WORKERTYPE.Timeline, 1, 1, "")
            _waitReply = True
            GetTimeline(WORKERTYPE.Reply, 1, 1, "")
            _waitDm = True
            GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, 1, "")
            If SettingDialog.GetFav Then
                _waitFav = True
                GetTimeline(WORKERTYPE.Favorites, 1, 1, "")
            End If
            _waitPubSearch = True
            GetTimeline(WORKERTYPE.PublicSearch, 1, 0, "")  'tabname="":全タブ
            _waitUserTimeline = True
            GetTimeline(WORKERTYPE.UserTimeline, 1, 0, "")  'tabname="":全タブ
            _waitLists = True
            GetTimeline(WORKERTYPE.List, 1, 0, "")  'tabname="":全タブ
            Dim i As Integer = 0
            Dim j As Integer = 0
            Do While (IsInitialRead()) AndAlso Not _endingFlag
                System.Threading.Thread.Sleep(100)
                My.Application.DoEvents()
                i += 1
                j += 1
                If j > 1200 Then Exit Do ' 120秒間初期処理が終了しなかったら強制的に打ち切る
                If i > 50 Then
                    If _endingFlag Then
                        Exit Sub
                    End If
                    i = 0
                End If
            Loop

            If _endingFlag Then Exit Sub

            'バージョンチェック（引数：起動時チェックの場合はTrue･･･チェック結果のメッセージを表示しない）
            If SettingDialog.StartupVersion Then
                CheckNewVersion(True)
            End If

            ' 取得失敗の場合は再試行する
            If Not tw.GetFollowersSuccess AndAlso SettingDialog.StartupFollowers Then
                GetTimeline(WORKERTYPE.Follower, 0, 0, "")
            End If
        End If
        _initial = False

        TimerTimeline.Enabled = True
    End Sub

    Private Function IsInitialRead() As Boolean
        Return _waitTimeline OrElse _waitReply OrElse _waitDm OrElse _waitFav OrElse _waitPubSearch OrElse _waitUserTimeline OrElse _waitLists
    End Function

    Private Sub doGetFollowersMenu()
        GetTimeline(WORKERTYPE.Follower, 1, 0, "")
        DispSelectedPost(True)
    End Sub

    Private Sub GetFollowersAllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpdateFollowersMenuItem1.Click
        doGetFollowersMenu()
    End Sub

    Private Sub doReTweetUnofficial()
        'RT @id:内容
        If Me.ExistCurrentPost Then
            If _curPost.IsDm OrElse _
               Not StatusText.Enabled Then Exit Sub

            If _curPost.IsProtect Then
                MessageBox.Show("Protected.")
                Exit Sub
            End If
            Dim rtdata As String = _curPost.Text
            rtdata = CreateRetweetUnofficial(rtdata)

            StatusText.Text = "RT @" + _curPost.ScreenName + ": " + HttpUtility.HtmlDecode(rtdata)

            StatusText.SelectionStart = 0
            StatusText.Focus()
        End If
    End Sub

    Private Sub ReTweetStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReTweetStripMenuItem.Click, RtUnOpMenuItem.Click
        doReTweetUnofficial()
    End Sub

    Private Sub doReTweetOfficial(ByVal isConfirm As Boolean)
        '公式RT
        If Me.ExistCurrentPost Then
            If _curPost.IsProtect Then
                MessageBox.Show("Protected.")
                _DoFavRetweetFlags = False
                Exit Sub
            End If
            If _curList.SelectedIndices.Count > 15 Then
                MessageBox.Show(My.Resources.RetweetLimitText)
                _DoFavRetweetFlags = False
                Exit Sub
            ElseIf _curList.SelectedIndices.Count > 1 Then
                Dim QuestionText As String = My.Resources.RetweetQuestion2
                If _DoFavRetweetFlags Then QuestionText = My.Resources.FavoriteRetweetQuestionText1
                Select Case MessageBox.Show(QuestionText, "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                    Case Windows.Forms.DialogResult.Cancel, Windows.Forms.DialogResult.No
                        _DoFavRetweetFlags = False
                        Exit Sub
                End Select
            Else
                If _curPost.IsDm OrElse _curPost.IsMe Then
                    _DoFavRetweetFlags = False
                    Exit Sub
                End If
                If Not SettingDialog.RetweetNoConfirm Then
                    Dim Questiontext As String = My.Resources.RetweetQuestion1
                    If _DoFavRetweetFlags Then Questiontext = My.Resources.FavoritesRetweetQuestionText2
                    If isConfirm AndAlso MessageBox.Show(Questiontext, "Retweet", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                        _DoFavRetweetFlags = False
                        Exit Sub
                    End If
                End If
            End If
            Dim args As New GetWorkerArg
            args.ids = New List(Of Long)
            args.sIds = New List(Of Long)
            args.tName = _curTab.Text
            args.type = WORKERTYPE.Retweet
            For Each idx As Integer In _curList.SelectedIndices
                Dim post As PostClass = GetCurTabPost(idx)
                If Not post.IsMe AndAlso Not post.IsProtect AndAlso Not post.IsDm Then args.ids.Add(post.StatusId)
            Next
            RunAsync(args)
        End If
    End Sub

    Private Sub ReTweetOriginalStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReTweetOriginalStripMenuItem.Click, RtOpMenuItem.Click
        doReTweetOfficial(True)
    End Sub

    Private Sub FavoritesRetweetOriginal()
        If Not Me.ExistCurrentPost Then Exit Sub
        _DoFavRetweetFlags = True
        doReTweetOfficial(True)
        If _DoFavRetweetFlags Then
            _DoFavRetweetFlags = False
            FavoriteChange(True, False)
        End If
    End Sub

    Private Sub FavoritesRetweetUnofficial()
        If Me.ExistCurrentPost AndAlso Not _curPost.IsDm Then
            _DoFavRetweetFlags = True
            FavoriteChange(True)
            If Not _curPost.IsProtect AndAlso _DoFavRetweetFlags Then
                _DoFavRetweetFlags = False
                doReTweetUnofficial()
            End If
        End If
    End Sub

    Private Function CreateRetweetUnofficial(ByVal status As String) As String

        ' Twitterにより省略されているURLを含むaタグをキャプチャしてリンク先URLへ置き換える
        '展開しないように変更
        '展開するか判定
        Dim isUrl As Boolean = False
        Dim ms As MatchCollection = Regex.Matches(status, "<a target=""_self"" href=""(?<url>[^""]+)""[^>]*>(?<link>(https?|shttp|ftps?)://[^<]+)</a>")
        For Each m As Match In ms
            If m.Result("${link}").EndsWith("...") Then
                isUrl = True
                Exit For
            End If
        Next
        If isUrl Then
            status = Regex.Replace(status, "<a target=""_self"" href=""(?<url>[^""]+)""[^>]*>(?<link>(https?|shttp|ftps?)://[^<]+)</a>", "${url}")
        Else
            status = Regex.Replace(status, "<a target=""_self"" href=""(?<url>[^""]+)""[^>]*>(?<link>(https?|shttp|ftps?)://[^<]+)</a>", "${link}")
        End If

        'その他のリンク(@IDなど)を置き換える
        status = Regex.Replace(status, "@<a target=""_self"" href=""https?://twitter.com/(#!/)?(?<url>[^""]+)""[^>]*>(?<link>[^<]+)</a>", "@${url}")
        'ハッシュタグ
        status = Regex.Replace(status, "<a target=""_self"" href=""(?<url>[^""]+)""[^>]*>(?<link>[^<]+)</a>", "${link}")
        '<br>タグ除去
        If StatusText.Multiline Then
            status = Regex.Replace(status, "(\r\n|\n|\r)?<br>", vbCrLf, RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        Else
            status = Regex.Replace(status, "(\r\n|\n|\r)?<br>", "", RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        End If

        _reply_to_id = 0
        _reply_to_name = ""
        status = status.Replace("&nbsp;", " ")

        Return status
    End Function

    Private Sub DumpPostClassToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DumpPostClassToolStripMenuItem.Click
        If _curPost IsNot Nothing Then
            DispSelectedPost(True)
        End If
    End Sub

    Private Sub MenuItemHelp_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuItemHelp.DropDownOpening
        If DebugBuild OrElse My.Computer.Keyboard.CapsLock AndAlso My.Computer.Keyboard.CtrlKeyDown AndAlso My.Computer.Keyboard.ShiftKeyDown Then
            DebugModeToolStripMenuItem.Visible = True
        Else
            DebugModeToolStripMenuItem.Visible = False
        End If
    End Sub

    Private Sub ToolStripMenuItemUrlAutoShorten_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItemUrlAutoShorten.CheckedChanged
        SettingDialog.UrlConvertAuto = ToolStripMenuItemUrlAutoShorten.Checked
    End Sub

    Private Sub ContextMenuPostMode_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuPostMode.Opening
        ToolStripMenuItemUrlAutoShorten.Checked = SettingDialog.UrlConvertAuto
    End Sub

    Private Sub TraceOutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TraceOutToolStripMenuItem.Click
        If TraceOutToolStripMenuItem.Checked Then
            TraceFlag = True
        Else
            TraceFlag = False
        End If
    End Sub

    Private Sub TweenMain_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
        '画面が非アクティブになったら、発言欄の背景色をデフォルトへ
        Me.StatusText_Leave(StatusText, System.EventArgs.Empty)
    End Sub

    Private Sub TabRenameMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabRenameMenuItem.Click, RenameTbMenuItem.Click
        If String.IsNullOrEmpty(_rclickTabName) Then Exit Sub
        TabRename(_rclickTabName)
    End Sub

    Private Sub BitlyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BitlyToolStripMenuItem.Click
        UrlConvert(UrlConverter.Bitly)
    End Sub

    Private Sub JmpToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles JmpStripMenuItem.Click
        UrlConvert(UrlConverter.Jmp)
    End Sub


    Private Class GetApiInfoArgs
        Public tw As Twitter
        Public info As ApiInfo
    End Class

    Private Sub GetApiInfo_Dowork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Dim args As GetApiInfoArgs = DirectCast(e.Argument, GetApiInfoArgs)
        e.Result = tw.GetInfoApi(args.info)
    End Sub

    Private Sub ApiInfoMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ApiInfoMenuItem.Click
        Dim info As New ApiInfo
        Dim tmp As String
        Dim args As New GetApiInfoArgs With {.tw = tw, .info = info}

        Using dlg As New FormInfo(Me, My.Resources.ApiInfo6, AddressOf GetApiInfo_Dowork, Nothing, args)
            dlg.ShowDialog()
            If CBool(dlg.Result) Then
                tmp = My.Resources.ApiInfo1 + args.info.MaxCount.ToString() + Environment.NewLine + _
                    My.Resources.ApiInfo2 + args.info.RemainCount.ToString() + Environment.NewLine + _
                    My.Resources.ApiInfo3 + args.info.ResetTime.ToString() + Environment.NewLine + _
                    My.Resources.ApiInfo7 + IIf(tw.UserStreamEnabled, My.Resources.Enable, My.Resources.Disable).ToString()
                SetStatusLabelUrl()
            Else
                tmp = My.Resources.ApiInfo5
            End If
        End Using

        MessageBox.Show(tmp, My.Resources.ApiInfo4, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub FollowCommandMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FollowCommandMenuItem.Click
        Dim id As String = ""
        If _curPost IsNot Nothing Then id = _curPost.ScreenName
        FollowCommand(id)
    End Sub

    Private Sub FollowCommand_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Dim arg As FollowRemoveCommandArgs = DirectCast(e.Argument, FollowRemoveCommandArgs)
        e.Result = arg.tw.PostFollowCommand(arg.id)
    End Sub

    Private Sub FollowCommand(ByVal id As String)
        Using inputName As New InputTabName()
            inputName.FormTitle = "Follow"
            inputName.FormDescription = My.Resources.FRMessage1
            inputName.TabName = id
            If inputName.ShowDialog() = Windows.Forms.DialogResult.OK AndAlso _
               Not String.IsNullOrEmpty(inputName.TabName.Trim()) Then
                Dim arg As New FollowRemoveCommandArgs
                arg.tw = tw
                arg.id = inputName.TabName.Trim()
                Using _info As New FormInfo(Me, My.Resources.FollowCommandText1, _
                                            AddressOf FollowCommand_DoWork, _
                                            Nothing, _
                                            arg)
                    _info.ShowDialog()
                    Dim ret As String = DirectCast(_info.Result, String)
                    If Not String.IsNullOrEmpty(ret) Then
                        MessageBox.Show(My.Resources.FRMessage2 + ret)
                    Else
                        MessageBox.Show(My.Resources.FRMessage3)
                    End If
                End Using
            End If
        End Using
    End Sub

    Private Sub RemoveCommandMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveCommandMenuItem.Click
        Dim id As String = ""
        If _curPost IsNot Nothing Then id = _curPost.ScreenName
        RemoveCommand(id, False)
    End Sub

    Private Class FollowRemoveCommandArgs
        Public tw As Tween.Twitter
        Public id As String
    End Class

    Private Sub RemoveCommand_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Dim arg As FollowRemoveCommandArgs = DirectCast(e.Argument, FollowRemoveCommandArgs)
        e.Result = arg.tw.PostRemoveCommand(arg.id)
    End Sub

    Private Sub RemoveCommand(ByVal id As String, ByVal skipInput As Boolean)
        Dim arg As New FollowRemoveCommandArgs
        arg.tw = tw
        arg.id = id
        If Not skipInput Then
            Using inputName As New InputTabName()
                inputName.FormTitle = "Unfollow"
                inputName.FormDescription = My.Resources.FRMessage1
                inputName.TabName = id
                If inputName.ShowDialog() = Windows.Forms.DialogResult.OK AndAlso _
                   Not String.IsNullOrEmpty(inputName.TabName.Trim()) Then
                    arg.tw = tw
                    arg.id = inputName.TabName.Trim()
                Else
                    Exit Sub
                End If
            End Using
        End If

        Using _info As New FormInfo(Me, My.Resources.RemoveCommandText1, _
                                    AddressOf RemoveCommand_DoWork, _
                                    Nothing, _
                                    arg)
            _info.ShowDialog()
            Dim ret As String = DirectCast(_info.Result, String)
            If Not String.IsNullOrEmpty(ret) Then
                MessageBox.Show(My.Resources.FRMessage2 + ret)
            Else
                MessageBox.Show(My.Resources.FRMessage3)
            End If
        End Using
    End Sub

    Private Sub FriendshipMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FriendshipMenuItem.Click
        Dim id As String = ""
        If _curPost IsNot Nothing Then
            id = _curPost.ScreenName
        End If
        ShowFriendship(id)
    End Sub

    Private Class ShowFriendshipArgs
        Public tw As Tween.Twitter
        Public Class FriendshipInfo
            Public id As String = ""
            Public isFollowing As Boolean = False
            Public isFollowed As Boolean = False
            Public isError As Boolean = False
            Public Sub New(ByVal id As String)
                Me.id = id
            End Sub
        End Class
        Public ids As New List(Of FriendshipInfo)
    End Class

    Private Sub ShowFriendship_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Dim arg As ShowFriendshipArgs = DirectCast(e.Argument, ShowFriendshipArgs)
        Dim result As String = ""
        For Each fInfo As ShowFriendshipArgs.FriendshipInfo In arg.ids
            Dim rt As String = arg.tw.GetFriendshipInfo(fInfo.id, fInfo.isFollowing, fInfo.isFollowed)
            If Not String.IsNullOrEmpty(rt) Then
                If String.IsNullOrEmpty(result) Then result = rt
                fInfo.isError = True
            End If
        Next
        e.Result = result
    End Sub

    Private Sub ShowFriendship(ByVal id As String)
        Dim args As New ShowFriendshipArgs
        args.tw = tw
        Using inputName As New InputTabName()
            inputName.FormTitle = "Show Friendships"
            inputName.FormDescription = My.Resources.FRMessage1
            inputName.TabName = id
            If inputName.ShowDialog() = Windows.Forms.DialogResult.OK AndAlso _
               Not String.IsNullOrEmpty(inputName.TabName.Trim()) Then
                Dim ret As String = ""
                args.ids.Add(New ShowFriendshipArgs.FriendshipInfo(inputName.TabName.Trim))
                Using _info As New FormInfo(Me, My.Resources.ShowFriendshipText1, _
                                            AddressOf ShowFriendship_DoWork, _
                                            Nothing, _
                                            args)
                    _info.ShowDialog()
                    ret = DirectCast(_info.Result, String)
                End Using
                Dim result As String = ""
                If String.IsNullOrEmpty(ret) Then
                    If args.ids(0).isFollowing Then
                        result = My.Resources.GetFriendshipInfo1 + System.Environment.NewLine
                    Else
                        result = My.Resources.GetFriendshipInfo2 + System.Environment.NewLine
                    End If
                    If args.ids(0).isFollowed Then
                        result += My.Resources.GetFriendshipInfo3
                    Else
                        result += My.Resources.GetFriendshipInfo4
                    End If
                    result = args.ids(0).id + My.Resources.GetFriendshipInfo5 + System.Environment.NewLine + result
                Else
                    result = ret
                End If
                MessageBox.Show(result)
            End If
        End Using
    End Sub

    Private Sub ShowFriendship(ByVal ids() As String)
        For Each id As String In ids
            Dim ret As String = ""
            Dim args As New ShowFriendshipArgs
            args.tw = tw
            args.ids.Add(New ShowFriendshipArgs.FriendshipInfo(id.Trim))
            Using _info As New FormInfo(Me, My.Resources.ShowFriendshipText1, _
                                        AddressOf ShowFriendship_DoWork, _
                                        Nothing, _
                                        args)
                _info.ShowDialog()
                ret = DirectCast(_info.Result, String)
            End Using
            Dim result As String = ""
            Dim fInfo As ShowFriendshipArgs.FriendshipInfo = args.ids(0)
            Dim ff As String = ""
            If String.IsNullOrEmpty(ret) Then
                ff = "  "
                If fInfo.isFollowing Then
                    ff += My.Resources.GetFriendshipInfo1
                Else
                    ff += My.Resources.GetFriendshipInfo2
                End If
                ff += System.Environment.NewLine + "  "
                If fInfo.isFollowed Then
                    ff += My.Resources.GetFriendshipInfo3
                Else
                    ff += My.Resources.GetFriendshipInfo4
                End If
                result += fInfo.id + My.Resources.GetFriendshipInfo5 + System.Environment.NewLine + ff
                If fInfo.isFollowing Then
                    If MessageBox.Show( _
                        My.Resources.GetFriendshipInfo7 + System.Environment.NewLine + result, My.Resources.GetFriendshipInfo8, _
                        MessageBoxButtons.YesNo, _
                        MessageBoxIcon.Question, _
                        MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
                        RemoveCommand(fInfo.id, True)
                    End If
                Else
                    MessageBox.Show(result)
                End If
            Else
                MessageBox.Show(ret)
            End If
        Next
    End Sub

    Private Sub OwnStatusMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OwnStatusMenuItem.Click
        doShowUserStatus(tw.Username, False)
        'If Not String.IsNullOrEmpty(tw.UserInfoXml) Then
        '    doShowUserStatus(tw.Username, False)
        'Else
        '    MessageBox.Show(My.Resources.ShowYourProfileText1, "Your status", MessageBoxButtons.OK, MessageBoxIcon.Information)
        '    Exit Sub
        'End If
    End Sub

    ' TwitterIDでない固定文字列を調べる（文字列検証のみ　実際に取得はしない）
    ' URLから切り出した文字列を渡す

    Public Function IsTwitterId(ByVal name As String) As Boolean
        Return Not Regex.Match(name, "^(about|jobs|tos|privacy)$").Success
    End Function

    Private Function GetUserId() As String
        Dim m As Match = Regex.Match(Me._postBrowserStatusText, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?$")
        If m.Success AndAlso IsTwitterId(m.Result("${ScreenName}")) Then
            Return m.Result("${ScreenName}")
        Else
            Return Nothing
        End If
    End Function

    Private Sub FollowContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FollowContextMenuItem.Click
        Dim name As String = GetUserId()
        If name IsNot Nothing Then FollowCommand(name)
    End Sub

    Private Sub RemoveContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveContextMenuItem.Click
        Dim name As String = GetUserId()
        If name IsNot Nothing Then RemoveCommand(name, False)
    End Sub

    Private Sub FriendshipContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FriendshipContextMenuItem.Click
        Dim name As String = GetUserId()
        If name IsNot Nothing Then ShowFriendship(name)
    End Sub

    Private Sub FriendshipAllMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FriendshipAllMenuItem.Click
        Dim ma As MatchCollection = Regex.Matches(Me.PostBrowser.DocumentText, "href=""https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?""")
        Dim ids As New List(Of String)
        For Each mu As Match In ma
            If mu.Result("${ScreenName}").ToLower <> tw.Username.ToLower Then
                ids.Add(mu.Result("${ScreenName}"))
            End If
        Next
        ShowFriendship(ids.ToArray)
    End Sub

    Private Sub ShowUserStatusContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowUserStatusContextMenuItem.Click
        Dim name As String = GetUserId()
        If name IsNot Nothing Then ShowUserStatus(name)
    End Sub

    Private Sub SearchPostsDetailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchPostsDetailToolStripMenuItem.Click
        Dim name As String = GetUserId()
        If name IsNot Nothing Then AddNewTabForUserTimeline(name)
    End Sub

    Private Sub SearchAtPostsDetailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchAtPostsDetailToolStripMenuItem.Click
        Dim name As String = GetUserId()
        If name IsNot Nothing Then AddNewTabForSearch("@" + name)
    End Sub

    Private Sub IdeographicSpaceToSpaceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IdeographicSpaceToSpaceToolStripMenuItem.Click
        _modifySettingCommon = True
    End Sub

    Private Sub ToolStripFocusLockMenuItem_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripFocusLockMenuItem.Click
        _modifySettingCommon = True
    End Sub

    Private Sub doQuote()
        'QT @id:内容
        '返信先情報付加
        If Me.ExistCurrentPost Then
            If _curPost.IsDm OrElse _
               Not StatusText.Enabled Then Exit Sub

            If _curPost.IsProtect Then
                MessageBox.Show("Protected.")
                Exit Sub
            End If
            Dim rtdata As String = _curPost.Text
            rtdata = CreateRetweetUnofficial(rtdata)

            StatusText.Text = " QT @" + _curPost.ScreenName + ": " + HttpUtility.HtmlDecode(rtdata)
            If _curPost.RetweetedId = 0 Then
                _reply_to_id = _curPost.StatusId
            Else
                _reply_to_id = _curPost.RetweetedId
            End If
            _reply_to_name = _curPost.ScreenName

            StatusText.SelectionStart = 0
            StatusText.Focus()
        End If
    End Sub

    Private Sub QuoteStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles QuoteStripMenuItem.Click, QtOpMenuItem.Click
        doQuote()
    End Sub

    Private Sub SearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '公式検索
        Dim pnl As Control = DirectCast(sender, Control).Parent
        If pnl Is Nothing Then Exit Sub
        Dim tbName As String = pnl.Parent.Text
        Dim tb As TabClass = _statuses.Tabs(tbName)
        Dim cmb As ComboBox = DirectCast(pnl.Controls("comboSearch"), ComboBox)
        Dim cmbLang As ComboBox = DirectCast(pnl.Controls("comboLang"), ComboBox)
        Dim cmbusline As ComboBox = DirectCast(pnl.Controls("comboUserline"), ComboBox)
        cmb.Text = cmb.Text.Trim
        ' 検索式演算子 OR についてのみ大文字しか認識しないので強制的に大文字とする
        Dim Quote As Boolean = False
        Dim buf As New StringBuilder()
        Dim c As Char() = cmb.Text.ToCharArray()
        For cnt As Integer = 0 To cmb.Text.Length - 1
            If cnt > cmb.Text.Length - 4 Then
                buf.Append(cmb.Text.Substring(cnt))
                Exit For
            End If
            If c(cnt) = CChar("""") Then
                Quote = Not Quote
            Else
                If Not Quote AndAlso cmb.Text.Substring(cnt, 4).Equals(" or ", StringComparison.OrdinalIgnoreCase) Then
                    buf.Append(" OR ")
                    cnt += 3
                    Continue For
                End If
            End If
            buf.Append(c(cnt))
        Next
        cmb.Text = buf.ToString()

        tb.SearchWords = cmb.Text
        tb.SearchLang = cmbLang.Text
        If cmb.Text = "" Then
            DirectCast(ListTab.SelectedTab.Tag, DetailsListView).Focus()
            SaveConfigsTabs()
            Exit Sub
        End If
        If tb.IsQueryChanged Then
            Dim idx As Integer = DirectCast(pnl.Controls("comboSearch"), ComboBox).Items.IndexOf(tb.SearchWords)
            If idx > -1 Then DirectCast(pnl.Controls("comboSearch"), ComboBox).Items.RemoveAt(idx)
            DirectCast(pnl.Controls("comboSearch"), ComboBox).Items.Insert(0, tb.SearchWords)
            cmb.Text = tb.SearchWords
            cmb.SelectAll()
            Dim lst As DetailsListView = DirectCast(pnl.Parent.Tag, DetailsListView)
            lst.VirtualListSize = 0
            lst.Items.Clear()
            _statuses.ClearTabIds(tbName)
            SaveConfigsTabs()   '検索条件の保存
        End If

        GetTimeline(WORKERTYPE.PublicSearch, 1, 0, tbName)
        DirectCast(ListTab.SelectedTab.Tag, DetailsListView).Focus()
    End Sub

    Private Sub RefreshMoreStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshMoreStripMenuItem.Click, RefreshPrevOpMenuItem.Click
        'もっと前を取得
        DoRefreshMore()
    End Sub

    Private Sub UndoRemoveTabMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UndoRemoveTabMenuItem.Click
        If _statuses.RemovedTab Is Nothing Then
            MessageBox.Show("There isn't removed tab.", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        Else
            Dim tb As TabClass = _statuses.RemovedTab
            _statuses.RemovedTab = Nothing
            Dim renamed As String = tb.TabName
            For i As Integer = 1 To Integer.MaxValue
                If Not _statuses.ContainsTab(renamed) Then Exit For
                renamed = tb.TabName + "(" + i.ToString + ")"
            Next
            tb.TabName = renamed
            _statuses.Tabs.Add(renamed, tb)
            AddNewTab(renamed, False, tb.TabType, tb.ListInfo)
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1
            SaveConfigsTabs()
        End If
    End Sub

    Private Sub doMoveToRTHome()
        If _curList.SelectedIndices.Count > 0 Then
            Dim post As PostClass = GetCurTabPost(_curList.SelectedIndices(0))
            If post.RetweetedId > 0 Then
                OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices(0)).RetweetedBy)
            End If
        End If
    End Sub

    Private Sub MoveToRTHomeMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveToRTHomeMenuItem.Click, OpenRterHomeMenuItem.Click
        doMoveToRTHome()
    End Sub

    Private Sub IdFilterAddMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IdFilterAddMenuItem.Click
        Dim name As String = GetUserId()
        If name IsNot Nothing Then
            Dim tabName As String = ""

            '未選択なら処理終了
            If _curList.SelectedIndices.Count = 0 Then Exit Sub

            'タブ選択（or追加）
            If Not SelectTab(tabName) Then Exit Sub

            Dim mv As Boolean = False
            Dim mk As Boolean = False
            MoveOrCopy(mv, mk)

            Dim fc As New FiltersClass
            fc.NameFilter = name
            fc.SearchBoth = True
            fc.MoveFrom = mv
            fc.SetMark = mk
            fc.UseRegex = False
            fc.SearchUrl = False
            _statuses.Tabs(tabName).AddFilter(fc)

            Try
                Me.Cursor = Cursors.WaitCursor
                _itemCache = Nothing
                _postCache = Nothing
                _curPost = Nothing
                _curItemIndex = -1
                _statuses.FilterAll()
                For Each tb As TabPage In ListTab.TabPages
                    DirectCast(tb.Tag, DetailsListView).VirtualListSize = _statuses.Tabs(tb.Text).AllCount
                    If _statuses.Tabs(tb.Text).UnreadCount > 0 Then
                        If SettingDialog.TabIconDisp Then
                            tb.ImageIndex = 0
                        End If
                    Else
                        If SettingDialog.TabIconDisp Then
                            tb.ImageIndex = -1
                        End If
                    End If
                Next
                If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
            Finally
                Me.Cursor = Cursors.Default
            End Try
            SaveConfigsTabs()
        End If
    End Sub

    Private Sub ListManageUserContextToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListManageUserContextToolStripMenuItem.Click, ListManageMenuItem.Click, ListManageUserContextToolStripMenuItem2.Click, ListManageUserContextToolStripMenuItem3.Click
        Dim user As String

        Dim menuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

        If menuItem.Owner Is Me.ContextMenuPostBrowser Then
            user = GetUserId()
            If user Is Nothing Then Return
        ElseIf Me._curPost IsNot Nothing Then
            user = Me._curPost.ScreenName
        Else
            Return
        End If

        Dim list As ListElement = Nothing

        If TabInformations.GetInstance().SubscribableLists.Count = 0 Then
            Dim res As String = Me.tw.GetListsApi()

            If res <> "" Then
                MessageBox.Show("Failed to get lists. (" + res + ")")
                Return
            End If
        End If

        Using listSelectForm As New MyLists(user, Me.tw)
            listSelectForm.ShowDialog()
        End Using
    End Sub

    Private Sub SearchControls_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim pnl As Control = DirectCast(sender, Control)
        For Each ctl As Control In pnl.Controls
            ctl.TabStop = True
        Next
    End Sub

    Private Sub SearchControls_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim pnl As Control = DirectCast(sender, Control)
        For Each ctl As Control In pnl.Controls
            ctl.TabStop = False
        Next
    End Sub

    Private Sub PublicSearchQueryMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PublicSearchQueryMenuItem.Click
        If ListTab.SelectedTab IsNot Nothing Then
            If _statuses.Tabs(ListTab.SelectedTab.Text).TabType <> TabUsageType.PublicSearch Then Exit Sub
            ListTab.SelectedTab.Controls("panelSearch").Controls("comboSearch").Focus()
        End If
    End Sub

    Private Sub UseHashtagMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UseHashtagMenuItem.Click
        Dim m As Match = Regex.Match(Me._postBrowserStatusText, "^https?://twitter.com/search\?q=%23(?<hash>[a-zA-Z0-9_]+)$")
        If m.Success Then
            HashMgr.SetPermanentHash("#" + m.Result("${hash}"))
            HashStripSplitButton.Text = HashMgr.UseHash
            HashToggleMenuItem.Checked = True
            HashToggleToolStripMenuItem.Checked = True
            '使用ハッシュタグとして設定
            _modifySettingCommon = True
        End If
    End Sub

    Private Sub StatusLabel_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusLabel.DoubleClick
        MessageBox.Show(StatusLabel.TextHistory, "Logs", MessageBoxButtons.OK, MessageBoxIcon.None)
    End Sub

    Private Sub HashManageMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HashManageMenuItem.Click, HashManageToolStripMenuItem.Click
        Dim rslt As DialogResult
        Try
            rslt = HashMgr.ShowDialog()
        Catch ex As Exception
            Exit Sub
        End Try
        Me.TopMost = SettingDialog.AlwaysTop
        If rslt = Windows.Forms.DialogResult.Cancel Then Exit Sub
        If HashMgr.UseHash <> "" Then
            HashStripSplitButton.Text = HashMgr.UseHash
            HashToggleMenuItem.Checked = True
            HashToggleToolStripMenuItem.Checked = True
        Else
            HashStripSplitButton.Text = "#[-]"
            HashToggleMenuItem.Checked = False
            HashToggleToolStripMenuItem.Checked = False
        End If
        'If HashMgr.IsInsert AndAlso HashMgr.UseHash <> "" Then
        '    Dim sidx As Integer = StatusText.SelectionStart
        '    Dim hash As String = HashMgr.UseHash + " "
        '    If sidx > 0 Then
        '        If StatusText.Text.Substring(sidx - 1, 1) <> " " Then
        '            hash = " " + hash
        '        End If
        '    End If
        '    StatusText.Text = StatusText.Text.Insert(sidx, hash)
        '    sidx += hash.Length
        '    StatusText.SelectionStart = sidx
        '    StatusText.Focus()
        'End If
        _modifySettingCommon = True
        Me.StatusText_TextChanged(Nothing, Nothing)
    End Sub

    Private Sub HashToggleMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles HashToggleMenuItem.Click, HashToggleToolStripMenuItem.Click
        HashMgr.ToggleHash()
        If HashMgr.UseHash <> "" Then
            HashStripSplitButton.Text = HashMgr.UseHash
            HashToggleMenuItem.Checked = True
            HashToggleToolStripMenuItem.Checked = True
        Else
            HashStripSplitButton.Text = "#[-]"
            HashToggleMenuItem.Checked = False
            HashToggleToolStripMenuItem.Checked = False
        End If
        _modifySettingCommon = True
        Me.StatusText_TextChanged(Nothing, Nothing)
    End Sub

    Private Sub HashStripSplitButton_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles HashStripSplitButton.ButtonClick
        HashToggleMenuItem_Click(Nothing, Nothing)
    End Sub

    Private Sub MenuItemOperate_DropDownOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItemOperate.DropDownOpening
        If ListTab.SelectedTab Is Nothing Then Exit Sub
        If _statuses Is Nothing OrElse _statuses.Tabs Is Nothing OrElse Not _statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text) Then Exit Sub
        If Not Me.ExistCurrentPost Then
            Me.ReplyOpMenuItem.Enabled = False
            Me.ReplyAllOpMenuItem.Enabled = False
            Me.DmOpMenuItem.Enabled = False
            Me.ShowProfMenuItem.Enabled = False
            Me.ShowUserTimelineToolStripMenuItem.Enabled = False
            Me.ListManageMenuItem.Enabled = False
            Me.OpenFavOpMenuItem.Enabled = False
            Me.CreateTabRuleOpMenuItem.Enabled = False
            Me.CreateIdRuleOpMenuItem.Enabled = False
            Me.ReadOpMenuItem.Enabled = False
            Me.UnreadOpMenuItem.Enabled = False
        Else
            Me.ReplyOpMenuItem.Enabled = True
            Me.ReplyAllOpMenuItem.Enabled = True
            Me.DmOpMenuItem.Enabled = True
            Me.ShowProfMenuItem.Enabled = True
            Me.ShowUserTimelineToolStripMenuItem.Enabled = True
            Me.ListManageMenuItem.Enabled = True
            Me.OpenFavOpMenuItem.Enabled = True
            Me.CreateTabRuleOpMenuItem.Enabled = True
            Me.CreateIdRuleOpMenuItem.Enabled = True
            Me.ReadOpMenuItem.Enabled = True
            Me.UnreadOpMenuItem.Enabled = True
        End If

        If _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.DirectMessage OrElse Not Me.ExistCurrentPost OrElse _curPost.IsDm Then
            Me.FavOpMenuItem.Enabled = False
            Me.UnFavOpMenuItem.Enabled = False
            Me.OpenStatusOpMenuItem.Enabled = False
            Me.OpenFavotterOpMenuItem.Enabled = False
            Me.ShowRelatedStatusesMenuItem2.Enabled = False
            Me.RtOpMenuItem.Enabled = False
            Me.RtUnOpMenuItem.Enabled = False
            Me.QtOpMenuItem.Enabled = False
            Me.FavoriteRetweetMenuItem.Enabled = False
            Me.FavoriteRetweetUnofficialMenuItem.Enabled = False
            If Me.ExistCurrentPost AndAlso _curPost.IsDm Then Me.DelOpMenuItem.Enabled = True
        Else
            Me.FavOpMenuItem.Enabled = True
            Me.UnFavOpMenuItem.Enabled = True
            Me.OpenStatusOpMenuItem.Enabled = True
            Me.OpenFavotterOpMenuItem.Enabled = True
            Me.ShowRelatedStatusesMenuItem2.Enabled = True  'PublicSearchの時問題出るかも

            If _curPost.IsMe Then
                Me.RtOpMenuItem.Enabled = False
                Me.FavoriteRetweetMenuItem.Enabled = False
                Me.DelOpMenuItem.Enabled = True
            Else
                Me.DelOpMenuItem.Enabled = False
                If _curPost.IsProtect Then
                    Me.RtOpMenuItem.Enabled = False
                    Me.RtUnOpMenuItem.Enabled = False
                    Me.QtOpMenuItem.Enabled = False
                    Me.FavoriteRetweetMenuItem.Enabled = False
                    Me.FavoriteRetweetUnofficialMenuItem.Enabled = False
                Else
                    Me.RtOpMenuItem.Enabled = True
                    Me.RtUnOpMenuItem.Enabled = True
                    Me.QtOpMenuItem.Enabled = True
                    Me.FavoriteRetweetMenuItem.Enabled = True
                    Me.FavoriteRetweetUnofficialMenuItem.Enabled = True
                End If
            End If
        End If

        If _statuses.Tabs(ListTab.SelectedTab.Text).TabType <> TabUsageType.Favorites Then
            Me.RefreshPrevOpMenuItem.Enabled = True
        Else
            Me.RefreshPrevOpMenuItem.Enabled = False
        End If
        If _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.PublicSearch _
                            OrElse Not Me.ExistCurrentPost _
                            OrElse Not _curPost.InReplyToStatusId > 0 Then
            OpenRepSourceOpMenuItem.Enabled = False
        Else
            OpenRepSourceOpMenuItem.Enabled = True
        End If
        If Not Me.ExistCurrentPost OrElse _curPost.RetweetedBy = "" Then
            OpenRterHomeMenuItem.Enabled = False
        Else
            OpenRterHomeMenuItem.Enabled = True
        End If
    End Sub

    Private Sub MenuItemTab_DropDownOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItemTab.DropDownOpening
        ContextMenuTabProperty_Opening(sender, Nothing)
    End Sub

    Public ReadOnly Property TwitterInstance() As Twitter
        Get
            Return tw
        End Get
    End Property


    Private Sub SplitContainer3_SplitterMoved(ByVal sender As System.Object, ByVal e As System.Windows.Forms.SplitterEventArgs) Handles SplitContainer3.SplitterMoved
        If Me.WindowState = FormWindowState.Normal AndAlso Not _initialLayout Then
            _mySpDis3 = SplitContainer3.SplitterDistance
            _modifySettingLocal = True
        End If
    End Sub

    Private Sub MenuItemEdit_DropDownOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItemEdit.DropDownOpening
        If _statuses.RemovedTab Is Nothing Then
            UndoRemoveTabMenuItem.Enabled = False
        Else
            UndoRemoveTabMenuItem.Enabled = True
        End If
        If ListTab.SelectedTab IsNot Nothing Then
            If _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.PublicSearch Then
                PublicSearchQueryMenuItem.Enabled = True
            Else
                PublicSearchQueryMenuItem.Enabled = False
            End If
        Else
            PublicSearchQueryMenuItem.Enabled = False
        End If
        If Not Me.ExistCurrentPost Then
            Me.CopySTOTMenuItem.Enabled = False
            Me.CopyURLMenuItem.Enabled = False
            Me.CopyUserIdStripMenuItem.Enabled = False
        Else
            Me.CopySTOTMenuItem.Enabled = True
            Me.CopyURLMenuItem.Enabled = True
            Me.CopyUserIdStripMenuItem.Enabled = True
            If _curPost.IsDm Then Me.CopyURLMenuItem.Enabled = False
            If _curPost.IsProtect Then Me.CopySTOTMenuItem.Enabled = False
        End If
    End Sub

    Private Sub NotifyIcon1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseMove
        SetNotifyIconText()
    End Sub

    Private Sub UserStatusToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserStatusToolStripMenuItem.Click
        Dim id As String = ""
        If _curPost IsNot Nothing Then
            id = _curPost.ScreenName
        End If
        ShowUserStatus(id)
    End Sub

    Private Class GetUserInfoArgs
        Public tw As Tween.Twitter
        Public id As String
        Public user As TwitterDataModel.User
    End Class

    Private Sub GetUserInfo_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Dim args As GetUserInfoArgs = DirectCast(e.Argument, GetUserInfoArgs)
        e.Result = args.tw.GetUserInfo(args.id, args.user)
    End Sub

    Private Overloads Sub doShowUserStatus(ByVal id As String, ByVal ShowInputDialog As Boolean)
        Dim result As String = ""
        Dim user As TwitterDataModel.User = Nothing
        Dim args As New GetUserInfoArgs
        If ShowInputDialog Then
            Using inputName As New InputTabName()
                inputName.FormTitle = "Show UserStatus"
                inputName.FormDescription = My.Resources.FRMessage1
                inputName.TabName = id
                If inputName.ShowDialog() = Windows.Forms.DialogResult.OK AndAlso _
                   Not String.IsNullOrEmpty(inputName.TabName.Trim()) Then
                    id = inputName.TabName.Trim
                    args.tw = tw
                    args.id = id
                    args.user = user
                    Using _info As New FormInfo(Me, My.Resources.doShowUserStatusText1, _
                                                AddressOf GetUserInfo_DoWork, _
                                                Nothing, _
                                                args)
                        _info.ShowDialog()
                        Dim ret As String = DirectCast(_info.Result, String)
                        If String.IsNullOrEmpty(ret) Then
                            doShowUserStatus(args.user)
                        Else
                            MessageBox.Show(ret)
                        End If
                    End Using
                End If
            End Using
        Else
            args.tw = tw
            args.id = id
            args.user = user
            Using _info As New FormInfo(Me, My.Resources.doShowUserStatusText1, _
                                        AddressOf GetUserInfo_DoWork, _
                                        Nothing, _
                                        args)
                _info.ShowDialog()
                Dim ret As String = DirectCast(_info.Result, String)
                If String.IsNullOrEmpty(ret) Then
                    doShowUserStatus(args.user)
                Else
                    MessageBox.Show(ret)
                End If
            End Using
        End If
    End Sub

    Private Overloads Sub doShowUserStatus(ByVal user As TwitterDataModel.User)
        Using userinfo As New ShowUserInfo()
            userinfo.User = user
            userinfo.ShowDialog(Me)
        End Using
    End Sub

    Private Overloads Sub ShowUserStatus(ByVal id As String, ByVal ShowInputDialog As Boolean)
        doShowUserStatus(id, ShowInputDialog)
    End Sub

    Private Overloads Sub ShowUserStatus(ByVal id As String)
        doShowUserStatus(id, True)
    End Sub

    Private Sub FollowToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FollowToolStripMenuItem.Click
        If NameLabel.Tag IsNot Nothing Then
            Dim id As String = DirectCast(NameLabel.Tag, String)
            If id <> tw.Username Then
                FollowCommand(id)
            End If
        End If
    End Sub

    Private Sub UnFollowToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UnFollowToolStripMenuItem.Click
        If NameLabel.Tag IsNot Nothing Then
            Dim id As String = DirectCast(NameLabel.Tag, String)
            If id <> tw.Username Then
                RemoveCommand(id, False)
            End If
        End If
    End Sub

    Private Sub ShowFriendShipToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowFriendShipToolStripMenuItem.Click
        If NameLabel.Tag IsNot Nothing Then
            Dim id As String = DirectCast(NameLabel.Tag, String)
            If id <> tw.Username Then
                ShowFriendship(id)
            End If
        End If
    End Sub

    Private Sub ShowUserStatusToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowUserStatusToolStripMenuItem.Click
        If NameLabel.Tag IsNot Nothing Then
            Dim id As String = DirectCast(NameLabel.Tag, String)
            ShowUserStatus(id, False)
        End If
    End Sub

    Private Sub SearchPostsDetailNameToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchPostsDetailNameToolStripMenuItem.Click
        If NameLabel.Tag IsNot Nothing Then
            Dim id As String = DirectCast(NameLabel.Tag, String)
            AddNewTabForUserTimeline(id)
        End If
    End Sub

    Private Sub SearchAtPostsDetailNameToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchAtPostsDetailNameToolStripMenuItem.Click
        If NameLabel.Tag IsNot Nothing Then
            Dim id As String = DirectCast(NameLabel.Tag, String)
            AddNewTabForSearch("@" + id)
        End If
    End Sub

    Private Sub ShowProfileMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowProfileMenuItem.Click, ShowProfMenuItem.Click
        If _curPost IsNot Nothing Then
            ShowUserStatus(_curPost.ScreenName, False)
        End If
    End Sub

    Private Sub GetRetweet_DoWork(ByVal sender As Object, ByVal e As ComponentModel.DoWorkEventArgs)
        Dim counter As Integer = 0

        Dim statusid As Long
        If _curPost.RetweetedId > 0 Then
            statusid = _curPost.RetweetedId
        Else
            statusid = _curPost.StatusId
        End If
        tw.GetStatus_Retweeted_Count(statusid, counter)

        e.Result = counter
    End Sub

    Private Sub RtCountMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RtCountMenuItem.Click
        If Me.ExistCurrentPost Then
            Using _info As New FormInfo(Me, My.Resources.RtCountMenuItem_ClickText1, _
                            AddressOf GetRetweet_DoWork)
                Dim retweet_count As Integer = 0

                ' ダイアログ表示
                _info.ShowDialog()
                retweet_count = CType(_info.Result, Integer)
                If retweet_count < 0 Then
                    MessageBox.Show(My.Resources.RtCountText2)
                Else
                    MessageBox.Show(retweet_count.ToString + My.Resources.RtCountText1)
                End If
            End Using
        End If
    End Sub

    Private WithEvents _hookGlobalHotkey As HookGlobalHotkey
    Public Sub New()
        _instance = Me
        _hookGlobalHotkey = New HookGlobalHotkey(Me)
        ' この呼び出しは、Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        Me._apiGauge.Control.Size = New Size(70, 22)
        Me._apiGauge.Control.Margin = New Padding(0, 3, 0, 2)
        Me._apiGauge.GaugeHeight = 8
        AddHandler Me._apiGauge.Control.DoubleClick, AddressOf Me.ApiInfoMenuItem_Click
        Me.StatusStrip1.Items.Insert(2, Me._apiGauge)
    End Sub

    Private Sub _hookGlobalHotkey_HotkeyPressed(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles _hookGlobalHotkey.HotkeyPressed
        If (Me.WindowState = FormWindowState.Normal OrElse Me.WindowState = FormWindowState.Maximized) AndAlso Me.Visible AndAlso Form.ActiveForm Is Me Then
            'アイコン化
            Me.Visible = False
        ElseIf Form.ActiveForm Is Nothing Then
            Me.Visible = True
            If Me.WindowState = FormWindowState.Minimized Then Me.WindowState = FormWindowState.Normal
            Me.Activate()
            Me.StatusText.Focus()
        End If
    End Sub

    Private Sub UserPicture_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserPicture.MouseEnter
        Me.UserPicture.Cursor = Cursors.Hand
    End Sub

    Private Sub UserPicture_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserPicture.MouseLeave
        Me.UserPicture.Cursor = Cursors.Default
    End Sub

    Private Sub UserPicture_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserPicture.DoubleClick
        If NameLabel.Tag IsNot Nothing Then
            OpenUriAsync("http://twitter.com/" + NameLabel.Tag.ToString)
        End If
    End Sub

    Private Sub SplitContainer2_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles SplitContainer2.MouseDoubleClick
        Me.MultiLineMenuItem.PerformClick()
    End Sub

    Public ReadOnly Property CurPost As PostClass
        Get
            Return _curPost
        End Get
    End Property

    Public ReadOnly Property IsPreviewEnable As Boolean
        Get
            Return SettingDialog.PreviewEnable
        End Get
    End Property

#Region "画像投稿"
    Private Sub ImageSelectMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImageSelectMenuItem.Click
        If ImageSelectionPanel.Visible = True Then
            ImagefilePathText.CausesValidation = False
            TimelinePanel.Visible = True
            TimelinePanel.Enabled = True
            ImageSelectionPanel.Visible = False
            ImageSelectionPanel.Enabled = False
            DirectCast(ListTab.SelectedTab.Tag, DetailsListView).Focus()
            ImagefilePathText.CausesValidation = True
        Else
            ImageSelectionPanel.Visible = True
            ImageSelectionPanel.Enabled = True
            TimelinePanel.Visible = False
            TimelinePanel.Enabled = False
            ImagefilePathText.Focus()
        End If
    End Sub

    Private Sub FilePickButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FilePickButton.Click
        OpenFileDialog1.Filter = (New PictureService(tw)).GetFileOpenDialogFilter(ImageService)
        OpenFileDialog1.Title = My.Resources.PickPictureDialog1
        OpenFileDialog1.FileName = ""

        Try
            Me.AllowDrop = False
            If OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.Cancel Then Exit Sub
        Finally
            Me.AllowDrop = True
        End Try

        ImagefilePathText.Text = OpenFileDialog1.FileName
        ImageFromSelectedFile()
    End Sub

    Private Sub ImagefilePathText_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ImagefilePathText.Validating
        If ImageCancelButton.Focused Then
            ImagefilePathText.CausesValidation = False
            Exit Sub
        End If
        ImagefilePathText.Text = Trim(ImagefilePathText.Text)
        If ImagefilePathText.Text = "" Then
            ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage
            ImageSelectedPicture.Tag = UploadFileType.Invalid
        Else
            ImageFromSelectedFile()
        End If
    End Sub

    Private Sub ImageFromSelectedFile()
        Try
            Dim svc As New PictureService(tw)
            If String.IsNullOrEmpty(Trim(ImagefilePathText.Text)) Then
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage
                ImageSelectedPicture.Tag = UploadFileType.Invalid
                ImagefilePathText.Text = ""
                Exit Sub
            End If

            Dim fl As New FileInfo(Trim(ImagefilePathText.Text))
            If Not svc.IsValidExtension(fl.Extension.ToLower, ImageService) Then
                '画像以外の形式
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage
                ImageSelectedPicture.Tag = UploadFileType.Invalid
                ImagefilePathText.Text = ""
                Exit Sub
            End If

            If svc.GetMaxFileSize(fl.Extension, ImageService) < fl.Length Then
                ' ファイルサイズが大きすぎる
                ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage
                ImageSelectedPicture.Tag = UploadFileType.Invalid
                ImagefilePathText.Text = ""
                MessageBox.Show("File is too large.")
                Exit Sub
            End If

            Select Case svc.GetFileType(fl.Extension.ToLower, ImageService)
                Case UploadFileType.Invalid
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage
                    ImageSelectedPicture.Tag = UploadFileType.Invalid
                    ImagefilePathText.Text = ""
                Case UploadFileType.Picture
                    Dim img As Image = Nothing
                    Using fs As New FileStream(ImagefilePathText.Text, FileMode.Open, FileAccess.Read)
                        img = Image.FromStream(fs)
                        fs.Close()
                    End Using
                    ImageSelectedPicture.Image = (New HttpVarious).CheckValidImage( _
                                img, _
                                img.Width, _
                                img.Height)
                    ImageSelectedPicture.Tag = UploadFileType.Picture
                Case UploadFileType.MultiMedia
                    ImageSelectedPicture.Image = My.Resources.MultiMediaImage
                    ImageSelectedPicture.Tag = UploadFileType.MultiMedia
                Case Else
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage
                    ImageSelectedPicture.Tag = UploadFileType.Invalid
                    ImagefilePathText.Text = ""
            End Select

        Catch ex As FileNotFoundException
            ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage
            ImageSelectedPicture.Tag = UploadFileType.Invalid
            ImagefilePathText.Text = ""
            MessageBox.Show("File not found.")
        Catch ex As Exception
            ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage
            ImageSelectedPicture.Tag = UploadFileType.Invalid
            ImagefilePathText.Text = ""
            MessageBox.Show("The type of this file is not image.")
        End Try
    End Sub

    Private Sub ImageSelection_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles _
        ImagefilePathText.KeyDown, _
        FilePickButton.KeyDown, _
        ImageServiceCombo.KeyDown
        If e.KeyCode = Keys.Escape Then
            ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage
            ImageSelectedPicture.Tag = UploadFileType.Invalid
            TimelinePanel.Visible = True
            TimelinePanel.Enabled = True
            ImageSelectionPanel.Visible = False
            ImageSelectionPanel.Enabled = False
            DirectCast(ListTab.SelectedTab.Tag, DetailsListView).Focus()
            ImagefilePathText.CausesValidation = True
        End If
    End Sub

    Private Sub ImageSelection_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles _
    ImagefilePathText.KeyPress, _
    FilePickButton.KeyPress, _
    ImageServiceCombo.KeyPress
        If Convert.ToInt32(e.KeyChar) = &H1B Then
            ImagefilePathText.CausesValidation = False
            e.Handled = True
        End If
    End Sub

    Private Sub ImageSelection_PreviewKeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles _
    ImagefilePathText.PreviewKeyDown, _
    FilePickButton.PreviewKeyDown, _
    ImageServiceCombo.PreviewKeyDown
        If e.KeyCode = Keys.Escape Then
            ImagefilePathText.CausesValidation = False
        End If
    End Sub

    Private Sub SetImageServiceCombo()
        Dim svc As String = ""
        If ImageServiceCombo.SelectedIndex > -1 Then svc = ImageServiceCombo.SelectedItem.ToString
        ImageServiceCombo.Items.Clear()
        If SettingDialog.IsOAuth Then
            ImageServiceCombo.Items.Add("TwitPic")
            ImageServiceCombo.Items.Add("img.ly")
            ImageServiceCombo.Items.Add("yfrog")
            ImageServiceCombo.Items.Add("Plixi")
        Else
            Exit Sub
        End If
        'ImageServiceCombo.Items.Add("TwitVideo")
        If svc = "" Then
            ImageServiceCombo.SelectedIndex = 0
        Else
            Dim idx As Integer = ImageServiceCombo.Items.IndexOf(svc)
            If idx = -1 Then
                ImageServiceCombo.SelectedIndex = 0
            Else
                ImageServiceCombo.SelectedIndex = idx
            End If
        End If
    End Sub

    Private ReadOnly Property ImageService() As String
        Get
            Return CStr(ImageServiceCombo.SelectedItem)
        End Get
    End Property

    Private Sub ImageCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImageCancelButton.Click
        ImagefilePathText.CausesValidation = False
        TimelinePanel.Visible = True
        TimelinePanel.Enabled = True
        ImageSelectionPanel.Visible = False
        ImageSelectionPanel.Enabled = False
        DirectCast(ListTab.SelectedTab.Tag, DetailsListView).Focus()
        ImagefilePathText.CausesValidation = True
    End Sub

    Private Sub ImageServiceCombo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImageServiceCombo.SelectedIndexChanged
        If ImageSelectedPicture.Tag IsNot Nothing Then
            Dim svc As New PictureService(tw)
            Try
                Dim fi As New FileInfo(ImagefilePathText.Text.Trim)
                If Not (svc.IsValidExtension(fi.Extension, ImageService) AndAlso _
                        svc.GetMaxFileSize(fi.Extension, ImageService) >= fi.Length) Then
                    ImagefilePathText.Text = ""
                    ImageSelectedPicture.Image = ImageSelectedPicture.InitialImage
                    ImageSelectedPicture.Tag = UploadFileType.Invalid
                End If
            Catch ex As Exception

            End Try
            _modifySettingCommon = True
            SaveConfigsAll(False)
        End If
    End Sub
#End Region

    Private Sub ListManageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListManageToolStripMenuItem.Click
        Using form As New ListManage(tw)
            form.ShowDialog(Me)
        End Using
    End Sub

    Public WriteOnly Property ModifySettingCommon() As Boolean
        Set(ByVal value As Boolean)
            _modifySettingCommon = value
        End Set
    End Property

    Public WriteOnly Property ModifySettingLocal() As Boolean
        Set(ByVal value As Boolean)
            _modifySettingLocal = value
        End Set
    End Property

    Public WriteOnly Property ModifySettingAtId() As Boolean
        Set(ByVal value As Boolean)
            _modifySettingAtId = value
        End Set
    End Property

    Private Sub SourceLinkLabel_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles SourceLinkLabel.LinkClicked
        Dim link As String = CType(SourceLinkLabel.Tag, String)
        If Not String.IsNullOrEmpty(link) Then
            OpenUriAsync(link)
        End If
    End Sub

    Private Sub SourceLinkLabel_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SourceLinkLabel.MouseEnter
        Dim link As String = CType(SourceLinkLabel.Tag, String)
        If Not String.IsNullOrEmpty(link) Then
            StatusLabelUrl.Text = link
        End If
    End Sub

    Private Sub SourceLinkLabel_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SourceLinkLabel.MouseLeave
        SetStatusLabelUrl()
    End Sub

    Private Sub MenuItemCommand_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuItemCommand.DropDownOpening
        If Me.ExistCurrentPost AndAlso Not _curPost.IsDm Then
            RtCountMenuItem.Enabled = True
        Else
            RtCountMenuItem.Enabled = False
        End If
    End Sub

    Private Sub CopyUserIdStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyUserIdStripMenuItem.Click
        CopyUserId()
    End Sub

    Private Sub CopyUserId()
        If _curPost Is Nothing Then Exit Sub
        Dim clstr As String = _curPost.ScreenName
        Try
            Clipboard.SetDataObject(clstr, False, 5, 100)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub ShowRelatedStatusesMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowRelatedStatusesMenuItem.Click, ShowRelatedStatusesMenuItem2.Click
        Dim backToTab As TabClass = If(_curTab Is Nothing, _statuses.Tabs(ListTab.SelectedTab.Text), _statuses.Tabs(_curTab.Text))
        If Me.ExistCurrentPost AndAlso Not _curPost.IsDm Then
            'PublicSearchも除外した方がよい？
            If _statuses.GetTabByType(TabUsageType.Related) Is Nothing Then
                Const TabName As String = "Related Tweets"
                Dim tName As String = TabName
                If Not Me.AddNewTab(tName, False, TabUsageType.Related) Then
                    For i As Integer = 2 To 100
                        tName = TabName + i.ToString()
                        If Me.AddNewTab(tName, False, TabUsageType.Related) Then
                            _statuses.AddTab(tName, TabUsageType.Related, Nothing)
                            Exit For
                        End If
                    Next
                Else
                    _statuses.AddTab(tName, TabUsageType.Related, Nothing)
                End If
                _statuses.GetTabByName(tName).UnreadManage = False
                _statuses.GetTabByName(tName).Notify = False
            End If

            Dim tb As TabClass = _statuses.GetTabByType(TabUsageType.Related)
            tb.RelationTargetPost = _curPost
            Me.ClearTab(tb.TabName, False)
            For i As Integer = 0 To ListTab.TabPages.Count - 1
                If tb.TabName = ListTab.TabPages(i).Text Then
                    ListTab.SelectedIndex = i
                    ListTabSelect(ListTab.TabPages(i))
                    Exit For
                End If
            Next

            GetTimeline(WORKERTYPE.Related, 1, 1, tb.TabName)
        End If
    End Sub

    Private Sub CacheInfoMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CacheInfoMenuItem.Click
        Dim buf As New StringBuilder
        buf.AppendFormat("キャッシュメモリ容量         : {0}bytes({1}MB)" + vbCrLf, DirectCast(TIconDic, ImageDictionary).CacheMemoryLimit, DirectCast(TIconDic, ImageDictionary).CacheMemoryLimit / 1048576)
        buf.AppendFormat("物理メモリ使用割合           : {0}%" + vbCrLf, DirectCast(TIconDic, ImageDictionary).PhysicalMemoryLimit)
        buf.AppendFormat("キャッシュエントリ保持数     : {0}" + vbCrLf, DirectCast(TIconDic, ImageDictionary).CacheCount)
        buf.AppendFormat("キャッシュエントリ破棄数     : {0}" + vbCrLf, DirectCast(TIconDic, ImageDictionary).CacheRemoveCount)
        MessageBox.Show(buf.ToString, "アイコンキャッシュ使用状況")
    End Sub

#Region "Userstream"
    Private _isActiveUserstream As Boolean = False

    Private Sub tw_PostDeleted(ByVal id As Long, ByRef post As PostClass)
        _statuses.RemovePostReserve(id, post)
        Try
            If InvokeRequired AndAlso Not IsDisposed Then
                Invoke(Sub()
                           If _curTab IsNot Nothing AndAlso _statuses.Tabs(_curTab.Text).Contains(id) Then
                               _itemCache = Nothing
                               _itemCacheIndex = -1
                               _postCache = Nothing
                               DirectCast(_curTab.Tag, DetailsListView).Update()
                           End If
                       End Sub)
                Exit Sub
            End If
        Catch ex As ObjectDisposedException
            Exit Sub
        End Try
    End Sub

    Private Sub tw_NewPostFromStream()
        If SettingDialog.ReadOldPosts Then
            _statuses.SetRead() '新着時未読クリア
        End If

        Dim rsltAddCount As Integer = _statuses.DistributePosts()
        SyncLock _syncObject
            Dim tm As Date = Now
            If _tlTimestamps.ContainsKey(tm) Then
                _tlTimestamps(tm) += rsltAddCount
            Else
                _tlTimestamps.Add(Now, rsltAddCount)
            End If
            Dim oneHour As Date = Now.Subtract(New TimeSpan(1, 0, 0))
            Dim keys As New List(Of Date)
            _tlCount = 0
            For Each key As Date In _tlTimestamps.Keys
                If key.CompareTo(oneHour) < 0 Then
                    keys.Add(key)
                Else
                    _tlCount += _tlTimestamps(key)
                End If
            Next
            For Each key As Date In keys
                _tlTimestamps.Remove(key)
            Next
            keys.Clear()

            'Static before As DateTime = Now
            'If before.Subtract(Now).Seconds > -5 Then Exit Sub
            'before = Now
        End SyncLock

        If SettingDialog.UserstreamPeriodInt > 0 Then Exit Sub

        Try
            If InvokeRequired AndAlso Not IsDisposed Then
                Invoke(New Action(Of Boolean)(AddressOf RefreshTimeline), True)
                Exit Sub
            End If
        Catch ex As ObjectDisposedException
            Exit Sub
        End Try
    End Sub

    Private Sub tw_UserStreamStarted()
        Me._isActiveUserstream = True
        If InvokeRequired AndAlso Not IsDisposed Then
            Invoke(New MethodInvoker(AddressOf tw_UserStreamStarted))
            Exit Sub
        End If

        MenuItemUserStream.Text = "&UserStream ▶"
        MenuItemUserStream.Enabled = True
        StopToolStripMenuItem.Text = "&Stop"
        StopToolStripMenuItem.Enabled = True

        StatusLabel.Text = "UserStream Started."
    End Sub

    Private Sub tw_UserStreamStopped()
        Me._isActiveUserstream = False
        If InvokeRequired AndAlso Not IsDisposed Then
            Invoke(New MethodInvoker(AddressOf tw_UserStreamStopped))
            Exit Sub
        End If

        MenuItemUserStream.Text = "&UserStream ■"
        MenuItemUserStream.Enabled = True
        StopToolStripMenuItem.Text = "&Start"
        StopToolStripMenuItem.Enabled = True

        StatusLabel.Text = "UserStream Stopped."
    End Sub

    Private Sub tw_UserStreamEventArrived(ByVal ev As Twitter.FormattedEvent)
        If InvokeRequired AndAlso Not IsDisposed Then
            Invoke(New Action(Of Twitter.FormattedEvent)(AddressOf tw_UserStreamEventArrived), ev)
            Exit Sub
        End If
        StatusLabel.Text = "Event: " + ev.Event
        'If ev.Event = "favorite" Then
        '    NotifyFavorite(ev)
        'End If
        NotifyEvent(ev)
        If ev.Event = "favorite" OrElse ev.Event = "unfavorite" Then
            If _curTab IsNot Nothing AndAlso _statuses.Tabs(_curTab.Text).Contains(ev.Id) Then
                _itemCache = Nothing
                _itemCacheIndex = -1
                _postCache = Nothing
                DirectCast(_curTab.Tag, DetailsListView).Update()
            End If
            If ev.Event = "unfavorite" AndAlso ev.Username.ToLower.Equals(tw.Username.ToLower) Then
                RemovePostFromFavTab(New Int64() {ev.Id})
            End If
        End If
    End Sub

    Private Sub NotifyEvent(ByVal ev As Twitter.FormattedEvent)
        '新着通知 
        If BalloonRequired(ev) Then
            NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning
            If SettingDialog.DispUsername Then NotifyIcon1.BalloonTipTitle = tw.Username + " - " Else NotifyIcon1.BalloonTipTitle = ""
            NotifyIcon1.BalloonTipTitle += "Tween [" + ev.Event.ToUpper() + "] by " + DirectCast(IIf(Not String.IsNullOrEmpty(ev.Username), ev.Username, ""), String)
            If Not String.IsNullOrEmpty(ev.Target) Then
                NotifyIcon1.BalloonTipText = ev.Target
            Else
                NotifyIcon1.BalloonTipText = " "
            End If
            NotifyIcon1.ShowBalloonTip(500)
        End If

        'サウンド再生
        Dim snd As String = SettingDialog.EventSoundFile
        If Not _initial AndAlso SettingDialog.PlaySound AndAlso snd <> "" Then
            If CBool(ev.Eventtype And SettingDialog.EventNotifyFlag) AndAlso IsMyEventNotityAsEventType(ev) Then
                Try
                    Dim dir As String = My.Application.Info.DirectoryPath
                    If Directory.Exists(Path.Combine(dir, "Sounds")) Then
                        dir = Path.Combine(dir, "Sounds")
                    End If
                    My.Computer.Audio.Play(Path.Combine(dir, snd), AudioPlayMode.Background)
                Catch ex As Exception

                End Try
            End If
        End If
    End Sub

    Private Sub StopToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StopToolStripMenuItem.Click
        MenuItemUserStream.Enabled = False
        If Me._isActiveUserstream Then
            tw.StopUserStream()
        Else
            tw.StartUserStream()
        End If
    End Sub

    Private Sub TrackToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackToolStripMenuItem.Click
        Static inputTrack As String = ""
        If TrackToolStripMenuItem.Checked Then
            Using inputForm As New InputTabName
                inputForm.TabName = inputTrack
                inputForm.FormTitle = "Input track word"
                inputForm.FormDescription = "Track word"
                If inputForm.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                    TrackToolStripMenuItem.Checked = False
                    Exit Sub
                End If
                inputTrack = inputForm.TabName.Trim()
            End Using
            If Not inputTrack.Equals(tw.TrackWord) Then
                tw.TrackWord = inputTrack
                Me._modifySettingCommon = True
                TrackToolStripMenuItem.Checked = Not String.IsNullOrEmpty(inputTrack)
                tw.ReconnectUserStream()
            End If
        Else
            tw.TrackWord = ""
            tw.ReconnectUserStream()
        End If
        Me._modifySettingCommon = True
    End Sub

    Private Sub AllrepliesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllrepliesToolStripMenuItem.Click
        tw.AllAtReply = AllrepliesToolStripMenuItem.Checked
        Me._modifySettingCommon = True
        tw.ReconnectUserStream()
    End Sub

    Private Sub EventViewerMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EventViewerMenuItem.Click
        If evtDialog Is Nothing OrElse evtDialog.IsDisposed Then
            evtDialog = Nothing
            evtDialog = New EventViewerDialog
            evtDialog.Owner = Me
            '親の中央に表示
            Dim pos As Point = evtDialog.Location
            pos.X = Convert.ToInt32(Me.Location.X + Me.Size.Width / 2 - evtDialog.Size.Width / 2)
            pos.Y = Convert.ToInt32(Me.Location.Y + Me.Size.Height / 2 - evtDialog.Size.Height / 2)
            evtDialog.Location = pos
        End If
        evtDialog.EventSource = tw.StoredEvent
        If Not evtDialog.Visible Then
            evtDialog.Show(Me)
        Else
            evtDialog.Activate()
        End If
        Me.TopMost = Me.SettingDialog.AlwaysTop
    End Sub
#End Region

    Private Sub TweenRestartMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles TweenRestartMenuItem.Click
        _endingFlag = True
        Try
            Application.Restart()
        Catch ex As NullReferenceException
            MessageBox.Show("Failed to restart. Please run Tween manually.")
            Application.Exit()
        End Try
    End Sub

    Private Sub OpenOwnFavedMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles OpenOwnFavedMenuItem.Click
        If Not tw.Username = "" Then OpenUriAsync(My.Resources.FavstarUrl + "users/" + tw.Username + "/recent")
    End Sub

    Private Sub OpenOwnHomeMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles OpenOwnHomeMenuItem.Click
        OpenUriAsync("http://twitter.com/" + tw.Username)
    End Sub

    Private Sub doTranslation(ByVal str As String)
        Dim g As New Google
        Dim buf As String = ""
        If String.IsNullOrEmpty(str) Then Exit Sub
        Dim srclng As String = g.LanguageDetect(str)
        Dim dstlng As String = SettingDialog.TranslateLanguage
        If srclng <> dstlng AndAlso g.Translate(srclng, dstlng, str, buf) Then
            PostBrowser.DocumentText = createDetailHtml(buf)
        End If
    End Sub

    Private Sub TranslationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TranslationToolStripMenuItem.Click
        If Not Me.ExistCurrentPost Then Exit Sub
        doTranslation(_curPost.TextFromApi)
    End Sub

    Private Sub SelectionTranslationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectionTranslationToolStripMenuItem.Click
        doTranslation(WebBrowser_GetSelectionText(PostBrowser))
    End Sub

    Private ReadOnly Property ExistCurrentPost As Boolean
        Get
            If _curPost Is Nothing Then Return False
            If _curPost.IsDeleted Then Return False
            Return True
        End Get
    End Property

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub ShowUserTimelineToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ShowUserTimelineToolStripMenuItem.Click, ShowUserTimelineContextMenuItem.Click
        ShowUserTimeline()
    End Sub

    Private Shared _instance As TweenMain

    Public Shared Function GetInstance() As TweenMain
        Return _instance
    End Function

    Public ReadOnly Property FavEventChangeUnread As Boolean
        Get
            Return SettingDialog.FavEventUnread
        End Get
    End Property

    Private Function GetUserIdFromCurPostOrInput(ByVal caption As String) As String
        Dim id As String = ""
        If _curPost IsNot Nothing Then
            id = _curPost.ScreenName
        End If
        Using inputName As New InputTabName()
            inputName.FormTitle = caption
            inputName.FormDescription = My.Resources.FRMessage1
            inputName.TabName = id
            If inputName.ShowDialog() = Windows.Forms.DialogResult.OK AndAlso _
               Not String.IsNullOrEmpty(inputName.TabName.Trim()) Then
                id = inputName.TabName.Trim
            Else
                id = ""
            End If
        End Using
        Return id
    End Function

    Private Sub UserTimelineToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserTimelineToolStripMenuItem.Click
        Dim id As String = GetUserIdFromCurPostOrInput("Show UserTimeline")
        If Not String.IsNullOrEmpty(id) Then
            AddNewTabForUserTimeline(id)
        End If
    End Sub

    Private Sub UserFavorareToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserFavorareToolStripMenuItem.Click
        Dim id As String = GetUserIdFromCurPostOrInput("Show Favstar")
        If Not String.IsNullOrEmpty(id) Then
            OpenUriAsync(My.Resources.FavstarUrl + "users/" + id + "/recent")
        End If
    End Sub

End Class
