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
Imports Microsoft.Win32

Public Class TweenMain

    '各種設定
    Private _mySize As Size             '画面サイズ
    Private _myLoc As Point             '画面位置
    Private _mySpDis As Integer         '区切り位置
    Private _mySpDis2 As Integer        '発言欄区切り位置
    Private _iconSz As Integer            'アイコンサイズ（現在は16、24、48の3種類。将来直接数字指定可能とする 注：24x24の場合に26と指定しているのはMSゴシック系フォントのための仕様）
    Private _iconCol As Boolean           '1列表示の時True（48サイズのとき）

    '雑多なフラグ類
    Private _initial As Boolean         'True:起動時処理中
    Private _ignoreConfigSave As Boolean         'True:起動時処理中
    'Private listViewItemSorter As ListViewItemComparer      'リストソート用カスタムクラス
    Private _tabDrag As Boolean           'タブドラッグ中フラグ（DoDragDropを実行するかの判定用）
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
    Private modifySettingLocal As Boolean = False
    Private modifySettingCommon As Boolean = False
    Private modifySettingAtId As Boolean = False

    'サブ画面インスタンス
    Private SettingDialog As New Setting()       '設定画面インスタンス
    Private TabDialog As New TabsDialog()        'タブ選択ダイアログインスタンス
    Private SearchDialog As New SearchWord()     '検索画面インスタンス
    'Private _tabs As New List(Of TabStructure)() '要素TabStructureクラスのジェネリックリストインスタンス（タブ情報用）
    Private fDialog As New FilterDialog() 'フィルター編集画面
    Private UrlDialog As New OpenURL()
    Private dialogAsShieldicon As DialogAsShieldIcon    ' シールドアイコン付きダイアログ
    Private AtIdSupl As AtIdSupplement    '@id補助
    Private HashSupl As AtIdSupplement    'Hashtag補助
    Private HashMgr As HashtagManage

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
    'Private TIconList As ImageList        '発言詳細部用アイコン画像リスト
    Private TIconDic As Dictionary(Of String, Image)        '発言詳細部用アイコン画像リスト
    Private TIconSmallList As ImageList   'リスト表示用アイコン画像リスト
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

    Private _history As New List(Of String)()   '発言履歴
    Private _hisIdx As Integer                  '発言履歴カレントインデックス

    '発言投稿時のAPI引数（発言編集時に設定。手書きreplyでは設定されない）
    Private _reply_to_id As Long     ' リプライ先のステータスID 0の場合はリプライではない 注：複数あてのものはリプライではない
    Private _reply_to_name As String    ' リプライ先ステータスの書き込み者の名前

    '時速表示用
    Private _postTimestamps As New List(Of Date)()
    Private _favTimestamps As New List(Of Date)()
    Private _tlTimestamps As New Dictionary(Of Date, Integer)()
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
    Private sf As New StringFormat()
    Private sfTab As New StringFormat()
    'Private _columnIdx As Integer   'ListviewのDisplayIndex退避用（DrawItemで使用）
    'Private _columnChangeFlag As Boolean

    '''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private _statuses As TabInformations
    Private _itemCache() As ListViewItem
    Private _itemCacheIndex As Integer
    Private _postCache() As PostClass
    Private _curTab As TabPage
    Private _curItemIndex As Integer
    Private _curList As DetailsListView
    Private _curPost As PostClass
    Private _isColumnChanged As Boolean = False
    'Private _waitFollower As Boolean = False
    Private _waitTimeline As Boolean = False
    Private _waitReply As Boolean = False
    Private _waitDm As Boolean = False
    Private _waitFav As Boolean = False
    Private _waitPubSearch As Boolean = False
    Private _bw(9) As BackgroundWorker
    Private _bwFollower As BackgroundWorker
    Private cMode As Integer
    Private shield As New ShieldIcon
    Private SecurityManager As InternetSecurityManager

    Private _homeCounter As Integer = 0
    Private _homeCounterAdjuster As Integer = 0
    Private _mentionCounter As Integer = 0
    Private _dmCounter As Integer = 0
    'Private _favCounter As Integer = 0
    Private _pubSearchCounter As Integer = 0
    '''''''''''''''''''''''''''''''''''''''''''''''''''''
#If DEBUG Then
    Private _drawcount As Long = 0
    Private _drawtime As Long = 0
#End If

    'URL短縮のUndo用
    Private Structure urlUndo
        Public Before As String
        Public After As String
    End Structure

    Private urlUndoBuffer As Generic.List(Of urlUndo) = Nothing

    'Backgroundworkerの処理結果通知用引数構造体
    Private Structure GetWorkerResult
        Public retMsg As String                     '処理結果詳細メッセージ。エラー時に値がセットされる
        'Public notifyPosts As List(Of PostClass) '取得した発言。Twitter.MyListItem構造体を要素としたジェネリックリスト
        Public page As Integer                      '取得対象ページ番号
        Public endPage As Integer                   '取得終了ページ番号（継続可能ならインクリメントされて返る。pageと比較して継続判定）
        Public type As WORKERTYPE                   '処理種別
        Public imgs As Dictionary(Of String, Image)                    '新規取得したアイコンイメージ
        Public tName As String                      'Fav追加・削除時のタブ名
        Public ids As List(Of Long)               'Fav追加・削除時のID
        Public sIds As List(Of Long)                  'Fav追加・削除成功分のID
        Public newDM As Boolean
        'Public soundFile As String
        Public addCount As Integer
    End Structure

    'Backgroundworkerへ処理内容を通知するための引数用構造体
    Private Structure GetWorkerArg
        Public page As Integer                      '処理対象ページ番号
        Public endPage As Integer                   '処理終了ページ番号（起動時の読み込みページ数。通常時はpageと同じ値をセット）
        Public type As WORKERTYPE                   '処理種別
        Public status As String                     '発言POST時の発言内容
        Public ids As List(Of Long)               'Fav追加・削除時のItemIndex
        Public sIds As List(Of Long)              'Fav追加・削除成功分のItemIndex
        Public tName As String                      'Fav追加・削除時のタブ名
    End Structure

    '検索処理タイプ
    Private Enum SEARCHTYPE
        DialogSearch
        NextSearch
        PrevSearch
    End Enum

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
        '画面が他画面の裏に隠れると、アイコン画像が再描画されない問題の対応
        If UserPicture.Image IsNot Nothing Then
            UserPicture.Invalidate(False)
        End If
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
        fDialog.Dispose()
        UrlDialog.Dispose()
        _spaceKeyCanceler.Dispose()
        If TIconDic IsNot Nothing AndAlso TIconDic.Keys.Count > 0 Then
            For Each key As String In TIconDic.Keys
                TIconDic(key).Dispose()
            Next
            TIconDic.Clear()
        End If
        If TIconSmallList IsNot Nothing Then TIconSmallList.Dispose()
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
        sf.Dispose()
        sfTab.Dispose()
        For Each bw As BackgroundWorker In _bw
            If bw IsNot Nothing Then
                bw.Dispose()
            End If
        Next
        If _bwFollower IsNot Nothing Then
            _bwFollower.Dispose()
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

        If File.Exists(Path.Combine(dir, "Icons\At.ico")) Then
            Try
                NIconAt = New Icon(Path.Combine(dir, "Icons\At.ico"))
            Catch ex As Exception
            End Try
        End If
        'タスクトレイエラー時アイコン
        If File.Exists(Path.Combine(dir, "Icons\AtRed.ico")) Then
            Try
                NIconAtRed = New Icon(Path.Combine(dir, "Icons\AtRed.ico"))
            Catch ex As Exception
            End Try
        End If
        'タスクトレイオフライン時アイコン
        If File.Exists(Path.Combine(dir, "Icons\AtSmoke.ico")) Then
            Try
                NIconAtSmoke = New Icon(Path.Combine(dir, "Icons\AtSmoke.ico"))
            Catch ex As Exception
            End Try
        End If
        'タスクトレイ更新中アイコン
        'アニメーション対応により4種類読み込み
        If File.Exists(Path.Combine(dir, "Icons\Refresh.ico")) Then
            Try
                NIconRefresh(0) = New Icon(Path.Combine(dir, "Icons\Refresh.ico"))
            Catch ex As Exception
            End Try
        End If
        If File.Exists(Path.Combine(dir, "Icons\Refresh2.ico")) Then
            Try
                NIconRefresh(1) = New Icon(Path.Combine(dir, "Icons\Refresh2.ico"))
            Catch ex As Exception
            End Try
        End If
        If File.Exists(Path.Combine(dir, "Icons\Refresh3.ico")) Then
            Try
                NIconRefresh(2) = New Icon(Path.Combine(dir, "Icons\Refresh3.ico"))
            Catch ex As Exception
            End Try
        End If
        If File.Exists(Path.Combine(dir, "Icons\Refresh4.ico")) Then
            Try
                NIconRefresh(3) = New Icon(Path.Combine(dir, "Icons\Refresh4.ico"))
            Catch ex As Exception
            End Try
        End If
        'タブ見出し未読表示アイコン
        If File.Exists(Path.Combine(dir, "Icons\Tab.ico")) Then
            Try
                TabIcon = New Icon(Path.Combine(dir, "Icons\Tab.ico"))
            Catch ex As Exception
            End Try
        End If
        '画面のアイコン
        If File.Exists(Path.Combine(dir, "Icons\MIcon.ico")) Then
            Try
                MainIcon = New Icon(Path.Combine(dir, "Icons\MIcon.ico"))
            Catch ex As Exception
            End Try
        End If
        'Replyのアイコン
        If File.Exists(Path.Combine(dir, "Icons\Reply.ico")) Then
            Try
                ReplyIcon = New Icon(Path.Combine(dir, "Icons\Reply.ico"))
            Catch ex As Exception
            End Try
        End If
        'Reply点滅のアイコン
        If File.Exists(Path.Combine(dir, "Icons\ReplyBlink.ico")) Then
            Try
                ReplyIconBlink = New Icon(Path.Combine(dir, "Icons\ReplyBlink.ico"))
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        _ignoreConfigSave = True
        Me.Visible = False
        SecurityManager = New InternetSecurityManager(PostBrowser)

        VerUpMenuItem.Image = shield.Icon
        If Not My.Application.CommandLineArgs.Count = 0 AndAlso My.Application.CommandLineArgs.Contains("/d") Then TraceFlag = True

        Me._spaceKeyCanceler = New SpaceKeyCanceler(Me.PostButton)
        AddHandler Me._spaceKeyCanceler.SpaceCancel, AddressOf spaceKeyCanceler_SpaceCancel

        fileVersion = _
            System.Diagnostics.FileVersionInfo.GetVersionInfo( _
            System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion

        LoadIcons() ' アイコン読み込み

        '発言保持クラス
        _statuses = TabInformations.GetInstance()

        'アイコン設定
        Me.Icon = MainIcon              'メインフォーム（TweenMain）
        NotifyIcon1.Icon = NIconAt      'タスクトレイ
        TabImage.Images.Add(TabIcon)    'タブ見出し

        SettingDialog.Owner = Me
        SearchDialog.Owner = Me
        fDialog.Owner = Me
        TabDialog.Owner = Me
        UrlDialog.Owner = Me

        _history.Add("")
        _hisIdx = 0
        _reply_to_id = 0
        _reply_to_name = ""

        '<<<<<<<<<設定関連>>>>>>>>>
        '設定コンバージョン
        ConvertConfig()

        ''設定読み出し
        'ユーザー名とパスワードの取得
        '_username = _cfgCommon.UserName
        '_password = _cfgCommon.Password
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
        sf.Alignment = StringAlignment.Near
        sf.LineAlignment = StringAlignment.Near
        sfTab.Alignment = StringAlignment.Center
        sfTab.LineAlignment = StringAlignment.Center

        '設定画面への反映
        'SettingDialog.UserID = _cfgCommon.UserName                                'ユーザ名
        'SettingDialog.PasswordStr = _cfgCommon.Password                           'パスワード
        SettingDialog.TimelinePeriodInt = _cfgCommon.TimelinePeriod
        SettingDialog.ReplyPeriodInt = _cfgCommon.ReplyPeriod
        SettingDialog.DMPeriodInt = _cfgCommon.DMPeriod
        SettingDialog.PubSearchPeriodInt = _cfgCommon.PubSearchPeriod
        'SettingDialog.NextPageThreshold = _cfgCommon.NextPageThreshold
        'SettingDialog.NextPagesInt = _cfgCommon.NextPages
        SettingDialog.MaxPostNum = _cfgCommon.MaxPostNum
        '不正値チェック
        If Not My.Application.CommandLineArgs.Contains("nolimit") Then
            If SettingDialog.TimelinePeriodInt < 30 AndAlso SettingDialog.TimelinePeriodInt > 0 Then SettingDialog.TimelinePeriodInt = 30
            If SettingDialog.ReplyPeriodInt < 30 AndAlso SettingDialog.ReplyPeriodInt > 0 Then SettingDialog.ReplyPeriodInt = 30
            If SettingDialog.DMPeriodInt < 30 AndAlso SettingDialog.DMPeriodInt > 0 Then SettingDialog.DMPeriodInt = 30
            If SettingDialog.PubSearchPeriodInt < 30 AndAlso SettingDialog.PubSearchPeriodInt > 0 Then SettingDialog.PubSearchPeriodInt = 30
        End If
        '起動時読み込みページ数
        'SettingDialog.ReadPages = _cfgCommon.ReadPages
        'SettingDialog.ReadPagesReply = _cfgCommon.ReadPagesReply
        'SettingDialog.ReadPagesDM = _cfgCommon.ReadPagesDM

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
        'SettingDialog.UseAPI = _cfgCommon.UseApi
        SettingDialog.CountApi = _cfgCommon.CountApi
        SettingDialog.CountApiReply = _cfgCommon.CountApiReply
        SettingDialog.UsePostMethod = False
        SettingDialog.HubServer = _cfgCommon.HubServer
        SettingDialog.BrowserPath = _cfgLocal.BrowserPath
        'SettingDialog.CheckReply = _cfgCommon.CheckReply
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
        'SettingDialog.StartupKey = _cfgCommon.StartupKey
        SettingDialog.StartupFollowers = _cfgCommon.StartupFollowers
        'SettingDialog.StartupAPImodeNoWarning = _cfgCommon.StartupApiModeNoWarning
        SettingDialog.RestrictFavCheck = _cfgCommon.RestrictFavCheck
        SettingDialog.AlwaysTop = _cfgCommon.AlwaysTop
        SettingDialog.UrlConvertAuto = _cfgCommon.UrlConvertAuto

        SettingDialog.OutputzEnabled = _cfgCommon.Outputz
        SettingDialog.OutputzKey = _cfgCommon.OutputzKey
        SettingDialog.OutputzUrlmode = _cfgCommon.OutputzUrlMode

        SettingDialog.UseUnreadStyle = _cfgCommon.UseUnreadStyle
        SettingDialog.DefaultTimeOut = _cfgCommon.DefaultTimeOut
        SettingDialog.ProtectNotInclude = _cfgCommon.ProtectNotInclude
        SettingDialog.PlaySound = _cfgCommon.PlaySound
        SettingDialog.DateTimeFormat = _cfgCommon.DateTimeFormat
        SettingDialog.LimitBalloon = _cfgCommon.LimitBalloon
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

        Dim statregex As New Regex("^0*")
        SettingDialog.RecommendStatusText = " [TWNv" + statregex.Replace(fileVersion.Replace(".", ""), "") + "]"

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

        Outputz.outputzKey = SettingDialog.OutputzKey
        Outputz.outputzEnabled = SettingDialog.OutputzEnabled
        Select Case SettingDialog.OutputzUrlmode
            Case OutputzUrlmode.twittercom
                Outputz.outputzUrl = "http://twitter.com/"
            Case OutputzUrlmode.twittercomWithUsername
                Outputz.outputzUrl = "http://twitter.com/" + Twitter.Username
        End Select

        SettingDialog.Nicoms = _cfgCommon.Nicoms
        'ハッシュタグ関連
        HashSupl = New AtIdSupplement(_cfgCommon.HashTags, "#")
        HashMgr = New HashtagManage(HashSupl, _
                                _cfgCommon.HashTags.ToArray, _
                                _cfgCommon.HashSelected, _
                                _cfgCommon.HashIsPermanent, _
                                _cfgCommon.HashIsHead)
        If HashMgr.UseHash <> "" AndAlso HashMgr.IsPermanent Then HashStripSplitButton.Text = HashMgr.UseHash

        '認証関連
        If _cfgCommon.Token = "" Then _cfgCommon.UserName = ""
        Twitter.Initialize(_cfgCommon.Token, _cfgCommon.TokenSecret, _cfgCommon.UserName)

        _initial = True

        'ユーザー名、パスワードが未設定なら設定画面を表示（初回起動時など）
        If Twitter.Username = "" Then
            '設定せずにキャンセルされた場合はプログラム終了
            If SettingDialog.ShowDialog() = Windows.Forms.DialogResult.Cancel Then
                Application.Exit()  '強制終了
                Exit Sub
            End If
            '設定されたが、依然ユーザー名とパスワードが未設定ならプログラム終了
            If Twitter.Username = "" Then
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

        'Twitter用通信クラス初期化
        'Twitter.Username = SettingDialog.UserID
        'Twitter.Password = SettingDialog.PasswordStr
        HttpConnection.InitializeConnection(SettingDialog.DefaultTimeOut, _
                                            SettingDialog.SelectedProxyType, _
                                            SettingDialog.ProxyAddress, _
                                            SettingDialog.ProxyPort, _
                                            SettingDialog.ProxyUser, _
                                            SettingDialog.ProxyPassword)
        'Twitter.SelectedProxyType = SettingDialog.SelectedProxyType
        'Twitter.ProxyAddress = SettingDialog.ProxyAddress
        'Twitter.ProxyPort = SettingDialog.ProxyPort
        'Twitter.ProxyUser = SettingDialog.ProxyUser
        'Twitter.ProxyPassword = SettingDialog.ProxyPassword
        'Twitter.NextThreshold = SettingDialog.NextPageThreshold   '次頁取得閾値
        'Twitter.NextPages = SettingDialog.NextPagesInt    '閾値オーバー時の読み込みページ数（未使用）
        'Twitter.DefaultTimeOut = SettingDialog.DefaultTimeOut
        Twitter.CountApi = SettingDialog.CountApi
        Twitter.CountApiReply = SettingDialog.CountApiReply
        'Twitter.UseAPI = SettingDialog.UseAPI
        'Twitter.UsePostMethod = False
        Twitter.HubServer = SettingDialog.HubServer
        Twitter.RestrictFavCheck = SettingDialog.RestrictFavCheck
        Twitter.ReadOwnPost = SettingDialog.ReadOwnPost
        Twitter.UseSsl = SettingDialog.UseSsl
        Twitter.BitlyId = SettingDialog.BitlyUser
        Twitter.BitlyKey = SettingDialog.BitlyPwd
        'If IsNetworkAvailable() Then
        '    If SettingDialog.StartupFollowers Then
        '        '_waitFollower = True
        '        GetTimeline(WORKERTYPE.Follower, 0, 0, "")
        '    End If
        'End If

        'ウィンドウ設定
        Me.ClientSize = _cfgLocal.FormSize
        _mySize = Me.ClientSize                     'サイズ保持（最小化・最大化されたまま終了した場合の対応用）
        Me.DesktopLocation = _cfgLocal.FormLocation
        _myLoc = Me.DesktopLocation                        '位置保持（最小化・最大化されたまま終了した場合の対応用）
        'タイトルバー領域
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
        Me.TopMost = SettingDialog.AlwaysTop
        _mySpDis = _cfgLocal.SplitterDistance
        _mySpDis2 = _cfgLocal.StatusTextHeight
        MultiLineMenuItem.Checked = _cfgLocal.StatusMultiline
        Me.Tween_ClientSizeChanged(Me, Nothing)
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
        'Recent取得間隔
        TimerTimeline.Interval = 1000
        'If SettingDialog.TimelinePeriodInt > 0 Then
        '    TimerTimeline.Interval = SettingDialog.TimelinePeriodInt * 1000
        'Else
        '    TimerTimeline.Interval = 600000
        'End If
        ''Reply取得間隔
        'If SettingDialog.ReplyPeriodInt > 0 Then
        '    TimerReply.Interval = SettingDialog.ReplyPeriodInt * 1000
        'Else
        '    TimerReply.Interval = 6000000
        'End If
        ''DM取得間隔
        'If SettingDialog.DMPeriodInt > 0 Then
        '    TimerDM.Interval = SettingDialog.DMPeriodInt * 1000
        'Else
        '    TimerDM.Interval = 6000000
        'End If
        '更新中アイコンアニメーション間隔
        TimerRefreshIcon.Interval = 85
        TimerRefreshIcon.Enabled = True

        '状態表示部の初期化（画面右下）
        StatusLabel.Text = ""
        StatusLabel.AutoToolTip = False
        StatusLabel.ToolTipText = ""
        '文字カウンタ初期化
        lblLen.Text = GetRestStatusCount(True, False).ToString()

        'If SettingDialog.StartupKey Then
        '    Twitter.GetWedata()
        'End If

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
            Twitter.GetIcon = False
        Else
            Twitter.GetIcon = True
            Twitter.IconSize = _iconSz
        End If
        Twitter.TinyUrlResolve = SettingDialog.TinyUrlResolve

        '発言詳細部アイコンをリストアイコンにサイズ変更
        Dim sz As Integer = _iconSz
        If _iconSz = 0 Then
            sz = 16
        End If
        TIconSmallList = New ImageList
        TIconSmallList.ImageSize = New Size(sz, sz)
        TIconSmallList.ColorDepth = ColorDepth.Depth32Bit
        '発言詳細部のアイコンリスト作成
        TIconDic = New Dictionary(Of String, Image)

        Twitter.ListIcon = TIconSmallList
        Twitter.DetailIcon = TIconDic

        StatusLabel.Text = My.Resources.Form1_LoadText1       '画面右下の状態表示を変更
        StatusLabelUrl.Text = ""            '画面左下のリンク先URL表示部を初期化
        NameLabel.Text = ""                 '発言詳細部名前ラベル初期化
        DateTimeLabel.Text = ""             '発言詳細部日時ラベル初期化

        '<<<<<<<<タブ関連>>>>>>>
        'デフォルトタブの存在チェック、ない場合には追加
        If _statuses.GetTabByType(TabUsageType.Home) Is Nothing Then
            If Not _statuses.Tabs.ContainsKey(DEFAULTTAB.RECENT) Then
                _statuses.AddTab(DEFAULTTAB.RECENT, TabUsageType.Home)
            Else
                _statuses.Tabs(DEFAULTTAB.RECENT).TabType = TabUsageType.Home
            End If
        End If
        If _statuses.GetTabByType(TabUsageType.Mentions) Is Nothing Then
            If Not _statuses.Tabs.ContainsKey(DEFAULTTAB.REPLY) Then
                _statuses.AddTab(DEFAULTTAB.REPLY, TabUsageType.Mentions)
            Else
                _statuses.Tabs(DEFAULTTAB.REPLY).TabType = TabUsageType.Mentions
            End If
        End If
        If _statuses.GetTabByType(TabUsageType.DirectMessage) Is Nothing Then
            If Not _statuses.Tabs.ContainsKey(DEFAULTTAB.DM) Then
                _statuses.AddTab(DEFAULTTAB.DM, TabUsageType.DirectMessage)
            Else
                _statuses.Tabs(DEFAULTTAB.DM).TabType = TabUsageType.DirectMessage
            End If
        End If
        If _statuses.GetTabByType(TabUsageType.Favorites) Is Nothing Then
            If Not _statuses.Tabs.ContainsKey(DEFAULTTAB.FAV) Then
                _statuses.AddTab(DEFAULTTAB.FAV, TabUsageType.Favorites)
            Else
                _statuses.Tabs(DEFAULTTAB.FAV).TabType = TabUsageType.Favorites
            End If
        End If
        For Each tn As String In _statuses.Tabs.Keys
            If _statuses.Tabs(tn).TabType = TabUsageType.Undefined Then
                _statuses.Tabs(tn).TabType = TabUsageType.UserDefined
            End If
            If Not AddNewTab(tn, True, _statuses.Tabs(tn).TabType) Then Throw New Exception("タブ作成エラー")
        Next

        Me.JumpReadOpMenuItem.ShortcutKeyDisplayString = "Space"
        CopySTOTMenuItem.ShortcutKeyDisplayString = "Ctrl+C"
        CopyURLMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+C"
        'MenuItemSubSearch.ShortcutKeyDisplayString = "/"
        'ReadedStripMenuItem.ShortcutKeyDisplayString = "B"
        'UnreadStripMenuItem.ShortcutKeyDisplayString = "Shift+B"

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

        TimerColorize.Interval = 200
        TimerColorize.Start()
        _ignoreConfigSave = False
        SaveConfigsAll(False)

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

    Private Function LoadOldConfig() As Boolean
        Dim needToSave As Boolean = False
        _cfgCommon = SettingCommon.Load()
        _cfgLocal = SettingLocal.Load()
        If _cfgCommon.TabList.Count > 0 Then
            For Each tabName As String In _cfgCommon.TabList
                _statuses.Tabs.Add(tabName, SettingTab.Load(tabName).Tab)
                If tabName <> ReplaceInvalidFilename(tabName) Then
                    Dim tb As TabClass = _statuses.Tabs(tabName)
                    _statuses.RemoveTab(tabName)
                    tb.TabName = ReplaceInvalidFilename(tabName)
                    _statuses.Tabs.Add(ReplaceInvalidFilename(tabName), tb)
                    Dim tabSetting As New SettingTab
                    tabSetting.Tab = tb
                    tabSetting.Save()
                    needToSave = True
                End If
            Next
        Else
            _statuses.AddTab(DEFAULTTAB.RECENT, TabUsageType.Home)
            _statuses.AddTab(DEFAULTTAB.REPLY, TabUsageType.Mentions)
            _statuses.AddTab(DEFAULTTAB.DM, TabUsageType.DirectMessage)
            _statuses.AddTab(DEFAULTTAB.FAV, TabUsageType.Favorites)
        End If
        If needToSave Then
            _cfgCommon.TabList.Clear()
            For Each tabName As String In _statuses.Tabs.Keys
                _cfgCommon.TabList.Add(tabName)
            Next
            _cfgCommon.Save()
        End If

        If System.IO.File.Exists(SettingCommon.GetSettingFilePath("")) Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub LoadConfig()
        Dim needToSave As Boolean = False
        _cfgCommon = SettingCommon.Load()
        _cfgLocal = SettingLocal.Load()
        Dim tabs As List(Of TabClass) = SettingTabs.Load().Tabs
        For Each tb As TabClass In tabs
            _statuses.Tabs.Add(tb.TabName, tb)
        Next
        If _statuses.Tabs.Count = 0 Then
            _statuses.AddTab(DEFAULTTAB.RECENT, TabUsageType.Home)
            _statuses.AddTab(DEFAULTTAB.REPLY, TabUsageType.Mentions)
            _statuses.AddTab(DEFAULTTAB.DM, TabUsageType.DirectMessage)
            _statuses.AddTab(DEFAULTTAB.FAV, TabUsageType.Favorites)
        End If
    End Sub

    Private Sub ConvertConfig()
        '新タブ設定ファイル存在チェック
        If System.IO.File.Exists(SettingTabs.GetSettingFilePath("")) Then
            LoadConfig()
            Exit Sub
        End If
        If LoadOldConfig() Then Exit Sub

        '_cfg = SettingToConfig.Load()
        'If _cfg Is Nothing Then Exit Sub

        ''新設定ファイルへ変換
        ''新しくエントリを増設する場合はここに書く必要はない
        '_cfgCommon.AlwaysTop = _cfg.AlwaysTop
        '_cfgCommon.AutoShortUrlFirst = _cfg.AutoShortUrlFirst
        '_cfgLocal.BrowserPath = _cfg.BrowserPath
        '_cfgCommon.CheckReply = _cfg.CheckReply
        '_cfgCommon.CloseToExit = _cfg.CloseToExit
        '_cfgLocal.ColorAtFromTarget = _cfg.ColorAtFromTarget
        '_cfgLocal.ColorAtSelf = _cfg.ColorAtSelf
        '_cfgLocal.ColorAtTarget = _cfg.ColorAtTarget
        '_cfgLocal.ColorFav = _cfg.ColorFav
        '_cfgLocal.ColorOWL = _cfg.ColorOWL
        '_cfgLocal.ColorRead = _cfg.ColorRead
        '_cfgLocal.ColorSelf = _cfg.ColorSelf
        '_cfgLocal.ColorTarget = _cfg.ColorTarget
        '_cfgLocal.ColorUnread = _cfg.ColorUnread
        '_cfgLocal.ColorInputBackcolor = _cfg.ColorInputBackcolor
        '_cfgLocal.ColorInputFont = _cfg.ColorInputFont
        '_cfgCommon.CountApi = _cfg.CountApi
        '_cfgCommon.CultureCode = _cfg.cultureCode
        '_cfgCommon.DateTimeFormat = _cfg.DateTimeFormat
        '_cfgCommon.DefaultTimeOut = _cfg.DefaultTimeOut
        '_cfgCommon.DispLatestPost = _cfg.DispLatestPost
        '_cfgLocal.DisplayIndex1 = _cfg.DisplayIndex1
        '_cfgLocal.DisplayIndex2 = _cfg.DisplayIndex2
        '_cfgLocal.DisplayIndex3 = _cfg.DisplayIndex3
        '_cfgLocal.DisplayIndex4 = _cfg.DisplayIndex4
        '_cfgLocal.DisplayIndex5 = _cfg.DisplayIndex5
        '_cfgLocal.DisplayIndex6 = _cfg.DisplayIndex6
        '_cfgLocal.DisplayIndex7 = _cfg.DisplayIndex7
        '_cfgLocal.DisplayIndex8 = _cfg.DisplayIndex8
        '_cfgCommon.DispUsername = _cfg.DispUsername
        '_cfgCommon.DMPeriod = _cfg.DMPeriod
        '_cfgLocal.FontDetail = _cfg.FontDetail
        '_cfgLocal.FontRead = _cfg.FontRead
        '_cfgLocal.FontUnread = _cfg.FontUnread
        '_cfgLocal.FontInputFont = _cfg.FontInputFont
        '_cfgLocal.FormLocation = _cfg.FormLocation
        '_cfgLocal.FormSize = _cfg.FormSize
        '_cfgCommon.HubServer = _cfg.HubServer
        '_cfgCommon.IconSize = _cfg.IconSize
        '_cfgCommon.LimitBalloon = _cfg.LimitBalloon
        '_cfgCommon.ListLock = _cfg.ListLock
        '_cfgCommon.MaxPostNum = _cfg.MaxPostNum
        '_cfgCommon.MinimizeToTray = _cfg.MinimizeToTray
        '_cfgCommon.NameBalloon = _cfg.NameBalloon
        '_cfgCommon.NewAllPop = _cfg.NewAllPop
        '_cfgCommon.NextPages = _cfg.NextPages
        '_cfgCommon.NextPageThreshold = _cfg.NextPageThreshold
        '_cfgCommon.OneWayLove = _cfg.OneWayLove
        '_cfgCommon.Outputz = _cfg.Outputz
        '_cfgCommon.OutputzKey = _cfg.OutputzKey
        '_cfgCommon.OutputzUrlMode = _cfg.OutputzUrlmode
        '_cfgCommon.Password = _cfg.Password
        '_cfgCommon.PeriodAdjust = _cfg.PeriodAdjust
        '_cfgCommon.PlaySound = _cfg.PlaySound
        '_cfgCommon.PostAndGet = _cfg.PostAndGet
        '_cfgCommon.PostCtrlEnter = _cfg.PostCtrlEnter
        '_cfgCommon.ProtectNotInclude = _cfg.ProtectNotInclude
        '_cfgLocal.ProxyAddress = _cfg.ProxyAddress
        '_cfgLocal.ProxyPassword = _cfg.ProxyPassword
        '_cfgLocal.ProxyPort = _cfg.ProxyPort
        '_cfgLocal.ProxyType = _cfg.ProxyType
        '_cfgLocal.ProxyUser = _cfg.ProxyUser
        '_cfgCommon.Read = _cfg.Read
        '_cfgCommon.ReadPages = _cfg.ReadPages
        '_cfgCommon.ReadPagesDM = _cfg.ReadPagesDM
        '_cfgCommon.ReadPagesReply = _cfg.ReadPagesReply
        '_cfgCommon.RestrictFavCheck = _cfg.RestrictFavCheck
        '_cfgCommon.SortColumn = _cfg.SortColumn
        '_cfgCommon.SortOrder = _cfg.SortOrder
        '_cfgCommon.SortOrderLock = _cfg.SortOrderLock
        '_cfgLocal.SplitterDistance = _cfg.SplitterDistance
        '_cfgCommon.StartupFollowers = _cfg.StartupFollowers
        '_cfgCommon.StartupKey = _cfg.StartupKey
        '_cfgCommon.StartupVersion = _cfg.StartupVersion
        '_cfgCommon.StartupApiModeNoWarning = _cfg.StartupAPImodeNoWarning
        '_cfgLocal.StatusMultiline = _cfg.StatusMultiline
        '_cfgLocal.StatusText = _cfg.StatusText
        '_cfgLocal.StatusTextHeight = _cfg.StatusTextHeight

        'For Each item As KeyValuePair(Of String, TabClass) In _cfg.Tabs
        '    Dim tabSetting As New SettingTab
        '    item.Value.TabName = ReplaceInvalidFilename(item.Value.TabName)
        '    tabSetting.Tab = item.Value
        '    tabSetting.Save()
        '    _cfgCommon.TabList.Add(ReplaceInvalidFilename(item.Key))
        '    If Not _statuses.Tabs.ContainsKey(tabSetting.Tab.TabName) Then
        '        _statuses.Tabs.Add(tabSetting.Tab.TabName, tabSetting.Tab)
        '    ElseIf tabSetting.Tab.TabName = DEFAULTTAB.REPLY Then
        '        _statuses.Tabs(DEFAULTTAB.REPLY) = tabSetting.Tab
        '    End If
        'Next
        '_cfgCommon.TimelinePeriod = _cfg.TimelinePeriod
        '_cfgCommon.TinyUrlResolve = _cfg.TinyURLResolve
        '_cfgCommon.UnreadManage = _cfg.UnreadManage
        '_cfgCommon.UrlConvertAuto = _cfg.UrlConvertAuto
        '_cfgCommon.UseApi = _cfg.UseAPI
        '_cfgCommon.UsePostMethod = _cfg.UsePostMethod
        '_cfgLocal.UseRecommendStatus = _cfg.UseRecommendStatus
        '_cfgCommon.UserName = _cfg.UserName
        '_cfgCommon.UseUnreadStyle = _cfg.UseUnreadStyle
        '_cfgLocal.Width1 = _cfg.Width1
        '_cfgLocal.Width2 = _cfg.Width2
        '_cfgLocal.Width3 = _cfg.Width3
        '_cfgLocal.Width4 = _cfg.Width4
        '_cfgLocal.Width5 = _cfg.Width5
        '_cfgLocal.Width6 = _cfg.Width6
        '_cfgLocal.Width7 = _cfg.Width7
        '_cfgLocal.Width8 = _cfg.Width8
        ''念のため保存
        '_cfgCommon.Save()
        '_cfgLocal.Save()
    End Sub

    'Private Sub Network_NetworkAvailabilityChanged(ByVal sender As Object, ByVal e As Devices.NetworkAvailableEventArgs)
    '    If e.IsNetworkAvailable Then
    '        Dim args As New GetWorkerArg()
    '        PostButton.Enabled = True
    '        FavAddToolStripMenuItem.Enabled = True
    '        FavRemoveToolStripMenuItem.Enabled = True
    '        MoveToHomeToolStripMenuItem.Enabled = True
    '        MoveToRTHomeMenuItem.Enabled = True
    '        MoveToFavToolStripMenuItem.Enabled = True
    '        DeleteStripMenuItem.Enabled = True
    '        RefreshStripMenuItem.Enabled = True
    '        _myStatusOnline = True
    '        If Not _initial Then
    '            'If SettingDialog.DMPeriodInt > 0 Then TimerDM.Enabled = True
    '            'If SettingDialog.TimelinePeriodInt > 0 Then TimerTimeline.Enabled = True
    '            'If SettingDialog.ReplyPeriodInt > 0 Then TimerReply.Enabled = True
    '        Else
    '            GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, 0, "")
    '        End If
    '    Else
    '        _myStatusOnline = False
    '        PostButton.Enabled = False
    '        FavAddToolStripMenuItem.Enabled = False
    '        FavRemoveToolStripMenuItem.Enabled = False
    '        MoveToHomeToolStripMenuItem.Enabled = False
    '        MoveToRTHomeMenuItem.Enabled = False
    '        MoveToFavToolStripMenuItem.Enabled = False
    '        DeleteStripMenuItem.Enabled = False
    '        RefreshStripMenuItem.Enabled = False
    '    End If
    'End Sub

    Private Sub TimerTimeline_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerTimeline.Tick
        If _homeCounter > 0 Then _homeCounter -= 1
        If _mentionCounter > 0 Then _mentionCounter -= 1
        If _dmCounter > 0 Then _dmCounter -= 1
        If _pubSearchCounter > 0 Then _pubSearchCounter -= 1

        If _homeCounter <= 0 AndAlso SettingDialog.TimelinePeriodInt > 0 Then
            GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
        End If
        If _mentionCounter <= 0 AndAlso SettingDialog.ReplyPeriodInt > 0 Then
            GetTimeline(WORKERTYPE.Reply, 1, 0, "")
        End If
        If _dmCounter <= 0 AndAlso SettingDialog.DMPeriodInt > 0 Then
            GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, 0, "")
        End If
        If _pubSearchCounter <= 0 AndAlso SettingDialog.PubSearchPeriodInt > 0 Then
            _pubSearchCounter = SettingDialog.PubSearchPeriodInt
            GetTimeline(WORKERTYPE.PublicSearch, 1, 0, "")
        End If
    End Sub

    Private Sub RefreshTimeline()
        'スクロール制御準備
        Dim smode As Integer = -1    '-1:制御しない,-2:最新へ,その他:topitem使用
        Dim topId As Long = GetScrollPos(smode)
        Dim befCnt As Integer = _curList.VirtualListSize

        '現在の選択状態を退避
        Dim selId As New Dictionary(Of String, Long())
        Dim focusedId As New Dictionary(Of String, Long)
        SaveSelectedStatus(selId, focusedId)

        '更新確定
        Dim notifyPosts() As PostClass = Nothing
        Dim soundFile As String = ""
        Dim addCount As Integer = 0
        addCount = _statuses.SubmitUpdate(soundFile, notifyPosts)

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
                    lst.VirtualListSize = tabInfo.AllCount 'リスト件数更新
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
                        _curList.EnsureVisible(_curList.VirtualListSize - 1)
                    Case -1
                        '制御しない
                    Case Else
                        '表示位置キープ
                        If _curList.VirtualListSize > 0 Then
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
        NotifyNewPosts(notifyPosts, soundFile, addCount)

        SetMainWindowTitle()
        If Not StatusLabelUrl.Text.StartsWith("http") Then SetStatusLabel()

        HashSupl.AddRangeItem(Twitter.GetHashList)

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
                        topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index)
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
                            topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index)
                            smode = 0
                        End If
                    End If
                Else
                    'Id降順
                    If ListLockMenuItem.Checked Then
                        '現在表示位置へ強制スクロール
                        topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index)
                        smode = 0
                    Else
                        '最上行が表示されていたら、制御しない。最上行が表示されていなかったら、現在表示位置へ強制スクロール
                        Dim _item As ListViewItem

                        _item = _curList.GetItemAt(0, 10)     '一番上
                        If _item Is Nothing Then _item = _curList.Items(0)
                        If _item.Index = 0 Then
                            smode = -3  '最上行
                        Else
                            topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index)
                            smode = 0
                        End If
                    End If
                End If
            Else
                '現在表示位置へ強制スクロール
                topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index)
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

    Private Sub NotifyNewPosts(ByVal notifyPosts() As PostClass, ByVal soundFile As String, ByVal addCount As Integer)
        '新着通知
        If ( _
                NewPostPopMenuItem.Checked AndAlso _
                notifyPosts IsNot Nothing AndAlso _
                notifyPosts.Length > 0 AndAlso _
                Not _initial AndAlso _
                ( _
                    ( _
                        SettingDialog.LimitBalloon AndAlso _
                        ( _
                            Me.WindowState = FormWindowState.Minimized OrElse _
                            Not Me.Visible OrElse _
                            Form.ActiveForm Is Nothing _
                        ) _
                    ) OrElse _
                    Not SettingDialog.LimitBalloon _
                ) _
            ) AndAlso _
            Not IsScreenSaverRunning() Then
            Dim sb As New StringBuilder
            Dim reply As Boolean = False
            Dim dm As Boolean = False
            For Each post As PostClass In notifyPosts
                If post.IsReply Then reply = True
                If post.IsDm Then dm = True
                If sb.Length > 0 Then sb.Append(System.Environment.NewLine)
                Select Case SettingDialog.NameBalloon
                    Case NameBalloonEnum.UserID
                        sb.Append(post.Name).Append(" : ")
                    Case NameBalloonEnum.NickName
                        sb.Append(post.Nickname).Append(" : ")
                End Select
                sb.Append(post.Data)
            Next
            If SettingDialog.DispUsername Then NotifyIcon1.BalloonTipTitle = Twitter.Username + " - " Else NotifyIcon1.BalloonTipTitle = ""
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
    End Sub

    'Private Sub Mylist_Scrolled(ByVal sender As Object, ByVal e As System.EventArgs)
    '    'TimerColorize.Stop()
    '    'TimerColorize.Start()
    'End Sub

    Private Sub MyList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If _curList.SelectedIndices.Count <> 1 Then Exit Sub
        'If _curList.SelectedIndices.Count = 0 Then Exit Sub

        _curItemIndex = _curList.SelectedIndices(0)
        'If _curPost Is GetCurTabPost(_curItemIndex) Then Exit Sub 'refreshで既読化されるのを防ぐため追加
        _curPost = GetCurTabPost(_curItemIndex)
        If SettingDialog.UnreadManage Then _statuses.SetRead(True, _curTab.Text, _curItemIndex)
        'MyList.RedrawItems(MyList.SelectedIndices(0), MyList.SelectedIndices(0), False)   'RetrieveVirtualItemが発生することを期待
        'キャッシュの書き換え
        ChangeCacheStyleRead(True, _curItemIndex, _curTab)   '既読へ（フォント、文字色）

        'ColorizeList(-1)    '全キャッシュ更新（背景色）
        'DispSelectedPost()
        ColorizeList()
        TimerColorize.Stop()
        TimerColorize.Start()
        'cMode = 1
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

        'For cnt As Integer = 0 To _itemCache.Length - 1
        '    If Not _postCache(cnt).IsRead AndAlso SettingDialog.UnreadManage AndAlso _statuses.Tabs(_curTab.Text).UnreadManage Then
        '        _itemCache(cnt).Font = _fntUnread
        '    Else
        '        _itemCache(cnt).Font = _fntReaded
        '    End If
        'Next

        If _post Is Nothing Then Exit Sub

        Try
            For cnt As Integer = 0 To _itemCache.Length - 1
                '_itemCache(cnt).BackColor = JudgeColor(_post, _postCache(cnt))
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

        'If Not tPost.IsRead AndAlso SettingDialog.UnreadManage AndAlso _statuses.Tabs(_curTab.Text).UnreadManage Then
        '    Item.Font = _fntUnread
        'Else
        '    Item.Font = _fntReaded
        'End If

        If _post Is Nothing Then Exit Sub

        If Item.Index = -1 Then
            Item.BackColor = JudgeColor(_post, tPost)
        Else
            _curList.ChangeItemBackColor(Item.Index, JudgeColor(_post, tPost))
        End If
    End Sub

    Private Function JudgeColor(ByVal BasePost As PostClass, ByVal TargetPost As PostClass) As Color
        Dim cl As Color
        If TargetPost.Id = BasePost.InReplyToId Then
            '@先
            cl = _clAtTo
        ElseIf TargetPost.IsMe Then
            '自分=発言者
            cl = _clSelf
        ElseIf TargetPost.Name.Equals(BasePost.Name, StringComparison.OrdinalIgnoreCase) Then
            '発言者
            cl = _clTarget
        ElseIf TargetPost.IsReply Then
            '自分宛返信
            cl = _clAtSelf
        ElseIf BasePost.ReplyToList.Contains(TargetPost.Name.ToLower()) Then
            '返信先
            cl = _clAtFromTarget
        ElseIf TargetPost.ReplyToList.Contains(BasePost.Name.ToLower()) Then
            'その人への返信
            cl = _clAtTarget
        Else
            'その他
            'cl = System.Drawing.SystemColors.Window
            cl = _clListBackcolor
        End If
        Return cl
    End Function

    Private Sub PostButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PostButton.Click
        If StatusText.Text.Trim.Length = 0 Then
            DoRefresh()
            Exit Sub
        End If

        _history(_history.Count - 1) = StatusText.Text.Trim

        If SettingDialog.UrlConvertAuto Then
            UrlConvertAutoToolStripMenuItem_Click(Nothing, Nothing)
        ElseIf SettingDialog.Nicoms Then
            UrlConvert(UrlConverter.Nicoms)
        End If
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
            Dim regex As New Regex("^[+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\*)([+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]+|$)", RegexOptions.IgnoreCase)
            If regex.IsMatch(tmpStatus) AndAlso tmpStatus.EndsWith(" .") = False Then adjustCount += 2
        End If

        If ToolStripMenuItemUrlMultibyteSplit.Checked Then
            ' URLと全角文字の切り離し
            Dim regex2 As New Regex("https?:\/\/[-_.!~*'()a-zA-Z0-9;\/?:\@&=+\$,%#]+")
            adjustCount += regex2.Matches(tmpStatus).Count
        End If

        If IdeographicSpaceToSpaceToolStripMenuItem.Checked Then
            ' 文中の全角スペースを半角スペース2個にする
            For i As Integer = 0 To tmpStatus.Length - 1
                If tmpStatus.Substring(i, 1) = "　" Then adjustCount += 1
            Next
        End If


        Dim isCutOff As Boolean = False
        Dim isRemoveFooter As Boolean = My.Computer.Keyboard.ShiftKeyDown
        If StatusText.Multiline AndAlso Not SettingDialog.PostCtrlEnter Then
            '複数行でEnter投稿の場合、Ctrlも押されていたらフッタ付加しない
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
        args.status = header + StatusText.Text.Trim + footer

        If ToolStripMenuItemApiCommandEvasion.Checked Then
            ' APIコマンド回避
            Dim regex As New Regex("^[+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\*)([+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]+|$)", RegexOptions.IgnoreCase)
            If regex.IsMatch(args.status) AndAlso args.status.EndsWith(" .") = False Then args.status += " ."
        End If

        If ToolStripMenuItemUrlMultibyteSplit.Checked Then
            ' URLと全角文字の切り離し
            Dim regex2 As New Regex("https?:\/\/[-_.!~*'()a-zA-Z0-9;\/?:\@&=+\$,%#]+")
            Dim mc2 As Match = regex2.Match(args.status)
            If mc2.Success Then args.status = regex2.Replace(args.status, "$& ")
        End If

        If IdeographicSpaceToSpaceToolStripMenuItem.Checked Then
            ' 文中の全角スペースを半角スペース2個にする
            args.status = args.status.Replace("　", "  ")
        End If

        If isCutOff AndAlso args.status.Length > 140 Then
            args.status = args.status.Substring(0, 140)
            If MessageBox.Show(args.status, "Post or Cancel?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then Exit Sub
        End If

        RunAsync(args)

        'Google検索（試験実装）
        If StatusText.Text.StartsWith("Google:") AndAlso StatusText.Text.Trim.Length > 7 Then
            Dim tmp As String = String.Format(My.Resources.SearchItem2Url, HttpUtility.UrlEncode(StatusText.Text.Substring(7)))
            OpenUriAsync(tmp)
        End If

        DirectCast(ListTab.SelectedTab.Tag, Control).Focus()
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
            _ignoreConfigSave = True
            _endingFlag = True
            TimerTimeline.Enabled = False
            'TimerReply.Enabled = False
            'TimerDM.Enabled = False
            TimerColorize.Enabled = False
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
            rslt.type = args.type
            rslt.tName = args.tName
            e.Result = rslt
            Exit Sub
        End If

        If args.type <> WORKERTYPE.OpenUri Then bw.ReportProgress(0, "") 'Notifyアイコンアニメーション開始
        Select Case args.type
            Case WORKERTYPE.Timeline, WORKERTYPE.Reply
                bw.ReportProgress(50, MakeStatusMessage(args, False))
                ret = Twitter.GetTimelineApi(read, args.type)
                'If SettingDialog.UseAPI Then
                '    ret = Twitter.GetTimelineApi(read, args.type)
                'Else
                '    ret = Twitter.GetTimeline(args.page, read, args.endPage, args.type, rslt.newDM)
                'End If
                '新着時未読クリア
                If ret = "" AndAlso args.type = WORKERTYPE.Timeline AndAlso SettingDialog.ReadOldPosts Then
                    _statuses.SetRead()
                End If
                rslt.addCount = _statuses.DistributePosts()
            Case WORKERTYPE.DirectMessegeRcv    '送信分もまとめて取得
                bw.ReportProgress(50, MakeStatusMessage(args, False))
                ret = Twitter.GetDirectMessageApi(read, WORKERTYPE.DirectMessegeRcv)
                If ret = "" Then ret = Twitter.GetDirectMessageApi(read, WORKERTYPE.DirectMessegeSnt)
                'If SettingDialog.UseAPI Then
                '    ret = Twitter.GetDirectMessageApi(read, WORKERTYPE.DirectMessegeRcv)
                '    If ret = "" Then ret = Twitter.GetDirectMessageApi(read, WORKERTYPE.DirectMessegeSnt)
                'Else
                '    ret = Twitter.GetDirectMessage(args.page, read, args.endPage, args.type)
                'End If
                rslt.addCount = _statuses.DistributePosts()
            Case WORKERTYPE.FavAdd
                'スレッド処理はしない
                For i As Integer = 0 To args.ids.Count - 1
                    Dim post As PostClass = _statuses.Item(args.ids(i))
                    args.page = i + 1
                    bw.ReportProgress(50, MakeStatusMessage(args, False))
                    If Not post.IsFav Then
                        If post.RetweetedId = 0 Then
                            ret = Twitter.PostFavAdd(post.Id)
                        Else
                            ret = Twitter.PostFavAdd(post.RetweetedId)
                        End If
                        If ret.Length = 0 Then
                            args.sIds.Add(post.Id)
                            post.IsFav = True    'リスト再描画必要
                            _favTimestamps.Add(Now)
                            If post.SearchTabName = "" Then
                                '検索タブからのfavは、favタブへ追加せず
                                _statuses.GetTabByType(TabUsageType.Favorites).Add(post.Id, post.IsRead, False)
                            End If
                            '検索タブに反映
                            For Each tb As TabClass In _statuses.GetTabsByType(TabUsageType.PublicSearch)
                                If tb.Contains(post.Id) Then tb.Posts(post.Id).IsFav = True
                            Next
                        End If
                    End If
                Next
                rslt.sIds = args.sIds
            Case WORKERTYPE.FavRemove
                'スレッド処理はしない
                For i As Integer = 0 To args.ids.Count - 1
                    Dim post As PostClass = _statuses.Item(args.ids(i))
                    args.page = i + 1
                    bw.ReportProgress(50, MakeStatusMessage(args, False))
                    If post.IsFav Then
                        If post.RetweetedId = 0 Then
                            ret = Twitter.PostFavRemove(post.Id)
                        Else
                            ret = Twitter.PostFavRemove(post.RetweetedId)
                        End If
                        If ret.Length = 0 Then
                            args.sIds.Add(post.Id)
                            post.IsFav = False    'リスト再描画必要
                            '検索タブに反映
                            For Each tb As TabClass In _statuses.GetTabsByType(TabUsageType.PublicSearch)
                                If tb.Contains(post.Id) Then tb.Posts(post.Id).IsFav = False
                            Next
                        End If
                    End If
                Next
                rslt.sIds = args.sIds
            Case WORKERTYPE.PostMessage
                bw.ReportProgress(200)
                For i As Integer = 0 To 1
                    ret = Twitter.PostStatus(args.status, _reply_to_id)
                    If ret = "" OrElse ret = "OK:Delaying?" OrElse ret.StartsWith("Outputz:") Then Exit For
                Next
                If ret = "" OrElse ret.StartsWith("Outputz") OrElse ret.StartsWith("OK:") Then
                    _reply_to_id = 0
                    _reply_to_name = ""
                End If
                bw.ReportProgress(300)
            Case WORKERTYPE.Retweet
                bw.ReportProgress(200)
                ret = Twitter.PostRetweet(args.ids(0), read)
                bw.ReportProgress(300)
            Case WORKERTYPE.Follower
                bw.ReportProgress(50, My.Resources.UpdateFollowersMenuItem1_ClickText1)
                ret = Twitter.GetFollowersApi()
                'If SettingDialog.UseAPI Then
                '    ret = Twitter.GetFollowersApi()
                'Else
                '    ret = Twitter.GetFollowers(False)       ' Followersリストキャッシュ有効
                'End If
            Case WORKERTYPE.OpenUri
                Dim myPath As String = Convert.ToString(args.status)

                Try
                    If SettingDialog.BrowserPath <> "" Then
                        'Shell(SettingDialog.BrowserPath & " " & myPath)
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
                ret = Twitter.GetFavoritesApi(read, args.type)
                'If SettingDialog.UseAPI Then
                '    ret = Twitter.GetFavoritesApi(read, args.type)
                'Else
                '    ret = Twitter.GetFavorites(args.page, read, args.endPage, args.type, rslt.newDM)
                'End If
                rslt.addCount = _statuses.DistributePosts()
            Case WORKERTYPE.PublicSearch
                bw.ReportProgress(50, MakeStatusMessage(args, False))
                If args.tName = "" Then
                    For Each tb As TabClass In _statuses.GetTabsByType(TabUsageType.PublicSearch)
                        ret = Twitter.GetSearch(read, tb, False)
                    Next
                Else
                    Dim tb As TabClass = _statuses.GetTabByName(args.tName)
                    If tb IsNot Nothing Then
                        ret = Twitter.GetSearch(read, tb, False)
                        If ret = "" AndAlso args.page = -1 Then
                            ret = Twitter.GetSearch(read, tb, True)
                        End If
                    End If
                End If
                '新着時未読クリア
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
                    'Case WORKERTYPE.DirectMessegeSnt
                    '    smsg = My.Resources.GetTimelineWorker_RunWorkerCompletedText12 + AsyncArg.page.ToString() + My.Resources.GetTimelineWorker_RunWorkerCompletedText6
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
                StatusText.Enabled = False
                PostButton.Enabled = False
                ReplyStripMenuItem.Enabled = False
                DMStripMenuItem.Enabled = False
            End If
            If e.ProgressPercentage = 300 Then  '終了
                StatusLabel.Text = My.Resources.PostWorker_RunWorkerCompletedText4
                StatusText.Enabled = True
                PostButton.Enabled = True
                ReplyStripMenuItem.Enabled = True
                DMStripMenuItem.Enabled = True
            End If
        Else
            Dim smsg As String = DirectCast(e.UserState, String)
            If smsg.Length > 0 Then StatusLabel.Text = smsg
        End If
    End Sub

    Private Sub GetTimelineWorker_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)

        If _endingFlag OrElse e.Cancelled Then Exit Sub 'キャンセル

        IsNetworkAvailable()

        'If _myStatusOnline Then
        '    'タイマー再始動
        '    If SettingDialog.TimelinePeriodInt > 0 AndAlso Not TimerTimeline.Enabled Then TimerTimeline.Enabled = True
        '    If SettingDialog.DMPeriodInt > 0 AndAlso Not TimerDM.Enabled Then TimerDM.Enabled = True
        '    If SettingDialog.ReplyPeriodInt > 0 AndAlso Not TimerReply.Enabled Then TimerReply.Enabled = True
        'End If

        If e.Error IsNot Nothing Then
            _myStatusError = True
            _waitTimeline = False
            _waitReply = False
            _waitDm = False
            _waitFav = False
            _waitPubSearch = False
            Throw New Exception("BackgroundWorker Exception", e.Error)
            Exit Sub
        End If

        Dim rslt As GetWorkerResult = DirectCast(e.Result, GetWorkerResult)
        Dim args As New GetWorkerArg()

        If rslt.type = WORKERTYPE.OpenUri Then Exit Sub

        'エラー
        If rslt.retMsg.Length > 0 Then
            _myStatusError = True
            StatusLabel.Text = rslt.retMsg
            'If Twitter.AccountState = ACCOUNT_STATE.Invalid Then
            '    Try
            '        Twitter.AccountState = ACCOUNT_STATE.Validating
            '        SettingStripMenuItem_Click(Nothing, Nothing)
            '        Twitter.AccountState = ACCOUNT_STATE.Valid
            '    Catch ex As Exception
            '        Twitter.AccountState = ACCOUNT_STATE.Invalid
            '    End Try
            'End If
        End If

        If rslt.type = WORKERTYPE.FavRemove Then
            DispSelectedPost()          ' 詳細画面書き直し
            Dim favTabName As String = _statuses.GetTabByType(TabUsageType.Favorites).TabName
            For Each i As Long In rslt.sIds
                _statuses.RemovePost(favTabName, i)
            Next
            If _curTab.Text.Equals(favTabName) Then
                _itemCache = Nothing    'キャッシュ破棄
                _postCache = Nothing
                _curPost = Nothing
                _curItemIndex = -1
            End If
            For Each tp As TabPage In ListTab.TabPages
                If tp.Text = favTabName Then
                    DirectCast(tp.Tag, DetailsListView).VirtualListSize = _statuses.Tabs(favTabName).AllCount
                    Exit For
                End If
            Next
        End If

        'リストに反映
        Dim busy As Boolean = False
        For Each bw As BackgroundWorker In _bw
            If bw IsNot Nothing AndAlso bw.IsBusy Then
                busy = True
                Exit For
            End If
        Next
        If Not busy Then RefreshTimeline() 'background処理なければ、リスト反映

        Select Case rslt.type
            Case WORKERTYPE.Timeline
                _waitTimeline = False
                If Not _initial Then
                    '通常時
                    '自動調整
                    'If Not SettingDialog.UseAPI Then
                    '    If SettingDialog.PeriodAdjust AndAlso SettingDialog.TimelinePeriodInt > 0 Then
                    '        If rslt.addCount >= 20 Then
                    '            _homeCounterAdjuster += 5
                    '            If SettingDialog.TimelinePeriodInt - _homeCounterAdjuster < 15 Then _homeCounterAdjuster = SettingDialog.TimelinePeriodInt - 15
                    '            'Dim itv As Integer = TimerTimeline.Interval
                    '            'itv -= 5000
                    '            'If itv < 15000 Then itv = 15000
                    '            'TimerTimeline.Interval = itv
                    '        Else
                    '            _homeCounterAdjuster -= 1
                    '            If _homeCounterAdjuster < 0 Then _homeCounterAdjuster = 0
                    '            'TimerTimeline.Interval += 1000
                    '            'If TimerTimeline.Interval > SettingDialog.TimelinePeriodInt * 1000 Then TimerTimeline.Interval = SettingDialog.TimelinePeriodInt * 1000
                    '        End If
                    '    End If
                    '    If rslt.newDM Then
                    '        GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, 0, "")
                    '    End If
                    'Else
                    '    'API使用時の取得調整は別途考える（カウント調整？）
                    'End If
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
                _curList.BeginUpdate()
                If rslt.type = WORKERTYPE.FavRemove AndAlso _statuses.Tabs(_curTab.Text).TabType = TabUsageType.Favorites Then
                    '色変えは不要
                Else
                    For i As Integer = 0 To rslt.sIds.Count - 1
                        If _curTab.Text.Equals(rslt.tName) Then
                            Dim idx As Integer = _statuses.Tabs(rslt.tName).IndexOf(rslt.sIds(i))
                            If idx > -1 Then
                                Dim post As PostClass = _statuses.Item(rslt.sIds(i))
                                ChangeCacheStyleRead(post.IsRead, idx, _curTab)
                                If idx = _curItemIndex Then DispSelectedPost() '選択アイテム再表示
                            End If
                        End If
                    Next
                End If
                _curList.EndUpdate()
            Case WORKERTYPE.PostMessage
                urlUndoBuffer = Nothing
                UrlUndoToolStripMenuItem.Enabled = False  'Undoをできないように設定

                If rslt.retMsg.Length > 0 AndAlso Not rslt.retMsg.StartsWith("Outputz") AndAlso rslt.retMsg <> "OK:Delaying?" Then
                    StatusLabel.Text = rslt.retMsg
                Else
                    _postTimestamps.Add(Now)
                    Dim oneHour As Date = Now.Subtract(New TimeSpan(1, 0, 0))
                    For i As Integer = _postTimestamps.Count - 1 To 0 Step -1
                        If _postTimestamps(i).CompareTo(oneHour) < 0 Then
                            _postTimestamps.RemoveAt(i)
                        End If
                    Next

                    If rslt.retMsg.Length > 0 Then StatusLabel.Text = rslt.retMsg 'Outputz失敗時

                    StatusText.Text = ""
                    _history.Add("")
                    _hisIdx = _history.Count - 1
                    If Not HashMgr.IsPermanent AndAlso HashMgr.UseHash <> "" Then
                        HashMgr.ClearHashtag()
                        Me.HashStripSplitButton.Text = "#[-]"
                    End If
                    SetMainWindowTitle()
                End If
                If rslt.retMsg.Length = 0 AndAlso SettingDialog.PostAndGet Then GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
            Case WORKERTYPE.Retweet
                If rslt.retMsg.Length > 0 Then
                    StatusLabel.Text = rslt.retMsg
                Else
                    _postTimestamps.Add(Now)
                    Dim oneHour As Date = Now.Subtract(New TimeSpan(1, 0, 0))
                    For i As Integer = _postTimestamps.Count - 1 To 0 Step -1
                        If _postTimestamps(i).CompareTo(oneHour) < 0 Then
                            _postTimestamps.RemoveAt(i)
                        End If
                    Next
                End If
                If rslt.retMsg.Length = 0 AndAlso SettingDialog.PostAndGet Then GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
            Case WORKERTYPE.Follower
                '_waitFollower = False
                _itemCache = Nothing
                _postCache = Nothing
                _curList.Refresh()
            Case WORKERTYPE.PublicSearch
                _waitPubSearch = False
        End Select

    End Sub

    Private Sub GetTimeline(ByVal WkType As WORKERTYPE, ByVal fromPage As Integer, ByVal toPage As Integer, ByVal tabName As String)
        'toPage=0:通常モード
        If Not IsNetworkAvailable() Then Exit Sub
        ''タイマー初期化
        If WkType = WORKERTYPE.Timeline AndAlso SettingDialog.TimelinePeriodInt > 0 Then
            _homeCounter = SettingDialog.TimelinePeriodInt - _homeCounterAdjuster
        End If
        If WkType = WORKERTYPE.Reply AndAlso SettingDialog.ReplyPeriodInt > 0 Then
            _mentionCounter = SettingDialog.ReplyPeriodInt
        End If
        If WkType = WORKERTYPE.DirectMessegeRcv AndAlso SettingDialog.DMPeriodInt > 0 Then
            _dmCounter = SettingDialog.DMPeriodInt
        End If

        '非同期実行引数設定
        Dim args As New GetWorkerArg
        args.page = fromPage
        args.endPage = toPage
        args.type = WkType
        args.tName = tabName

        RunAsync(args)

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

    Private Function NextPageMessage(ByVal page As Integer) As DialogResult
        Dim flashRslt As Integer = Win32Api.FlashWindow(Me.Handle.ToInt32, 1)
        Return MessageBox.Show((page * 20).ToString + My.Resources.GetTimelineWorker_RunWorkerCompletedText2, _
                           My.Resources.GetTimelineWorker_RunWorkerCompletedText3, _
                           MessageBoxButtons.YesNo, _
                           MessageBoxIcon.Question)
    End Function

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
        If _statuses.Tabs(_curTab.Text).TabType = TabUsageType.DirectMessage OrElse _curList.SelectedIndices.Count = 0 Then Exit Sub

        '複数fav確認msg
        If _curList.SelectedIndices.Count > 1 Then
            If MessageBox.Show(My.Resources.FavAddToolStripMenuItem_ClickText1, My.Resources.FavAddToolStripMenuItem_ClickText2, _
                               MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Exit Sub
            End If
        End If

        Dim args As New GetWorkerArg
        args.ids = New List(Of Long)
        args.sIds = New List(Of Long)
        args.tName = _curTab.Text
        args.type = WORKERTYPE.FavAdd
        For Each idx As Integer In _curList.SelectedIndices
            Dim post As PostClass = GetCurTabPost(idx)
            If Not post.IsFav Then args.ids.Add(post.Id)
        Next
        If args.ids.Count = 0 Then
            StatusLabel.Text = My.Resources.FavAddToolStripMenuItem_ClickText4
            Exit Sub
        End If

        RunAsync(args)
    End Sub

    Private Sub FavRemoveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FavRemoveToolStripMenuItem.Click, UnFavOpMenuItem.Click
        If _statuses.Tabs(_curTab.Text).TabType = TabUsageType.DirectMessage OrElse _curList.SelectedIndices.Count = 0 Then Exit Sub

        If _curList.SelectedIndices.Count > 1 Then
            If MessageBox.Show(My.Resources.FavRemoveToolStripMenuItem_ClickText1, My.Resources.FavRemoveToolStripMenuItem_ClickText2, _
                               MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Exit Sub
            End If
        End If

        Dim args As New GetWorkerArg()
        args.ids = New List(Of Long)()
        args.sIds = New List(Of Long)()
        args.tName = _curTab.Text
        args.type = WORKERTYPE.FavRemove
        For Each idx As Integer In _curList.SelectedIndices
            Dim post As PostClass = GetCurTabPost(idx)
            If post.IsFav Then args.ids.Add(post.Id)
        Next
        If args.ids.Count = 0 Then
            StatusLabel.Text = My.Resources.FavRemoveToolStripMenuItem_ClickText4
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
            OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices(0)).Name)
        End If
    End Sub

    Private Sub MoveToFavToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveToFavToolStripMenuItem.Click, OpenFavOpMenuItem.Click
        If _curList.SelectedIndices.Count > 0 Then
            OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices(0)).Name + "/favorites")
        End If
    End Sub

    Private Sub Tween_ClientSizeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.ClientSizeChanged
        'ショートカットから最小化状態で起動した際の対応
        Static initialize As Boolean = False

        If Me.WindowState <> FormWindowState.Minimized Then
            If initialize Then
                If Me.WindowState = FormWindowState.Normal Then
                    _mySize = Me.ClientSize
                    _mySpDis = Me.SplitContainer1.SplitterDistance
                    If StatusText.Multiline Then _mySpDis2 = Me.StatusText.Height
                    modifySettingLocal = True
                End If
            ElseIf _cfgLocal IsNot Nothing Then
                '初回フォームレイアウト復元
                Try
                    Me.SplitContainer1.SplitterDistance = _cfgLocal.SplitterDistance     'Splitterの位置設定
                    '発言欄複数行
                    StatusText.Multiline = _cfgLocal.StatusMultiline
                    If StatusText.Multiline Then
                        SplitContainer2.SplitterDistance = SplitContainer2.Height - _cfgLocal.StatusTextHeight - SplitContainer2.SplitterWidth
                    Else
                        SplitContainer2.SplitterDistance = SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth
                    End If
                    initialize = True
                Catch ex As Exception
                End Try
            End If
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
        _itemCache = Nothing
        _postCache = Nothing
        _curList.Refresh()
        modifySettingCommon = True
    End Sub

    Private Sub Tween_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LocationChanged
        If Me.WindowState = FormWindowState.Normal Then
            _myLoc = Me.DesktopLocation
            modifySettingLocal = True
        End If
    End Sub

    Private Sub ContextMenuStrip2_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip2.Opening
        If ListTab.SelectedTab Is Nothing Then Exit Sub
        If _statuses Is Nothing OrElse _statuses.Tabs Is Nothing OrElse Not _statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text) Then Exit Sub
        If _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.DirectMessage Then
            FavAddToolStripMenuItem.Enabled = False
            FavRemoveToolStripMenuItem.Enabled = False
            StatusOpenMenuItem.Enabled = False
            FavorareMenuItem.Enabled = False
        Else
            If IsNetworkAvailable() Then
                FavAddToolStripMenuItem.Enabled = True
                FavRemoveToolStripMenuItem.Enabled = True
                StatusOpenMenuItem.Enabled = True
                FavorareMenuItem.Enabled = True
            End If
        End If
        If _curPost Is Nothing OrElse _curPost.IsDm Then
            ReTweetStripMenuItem.Enabled = False
            ReTweetOriginalStripMenuItem.Enabled = False
            QuoteStripMenuItem.Enabled = False
        Else
            ReTweetStripMenuItem.Enabled = True
            ReTweetOriginalStripMenuItem.Enabled = True
            QuoteStripMenuItem.Enabled = True
        End If
        If _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.PublicSearch Then
            RefreshMoreStripMenuItem.Enabled = True
        Else
            RefreshMoreStripMenuItem.Enabled = False
        End If
    End Sub

    Private Sub ReplyStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReplyStripMenuItem.Click, ReplyOpMenuItem.Click
        MakeReplyOrDirectStatus(False, True)
    End Sub

    Private Sub DMStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DMStripMenuItem.Click, DmOpMenuItem.Click
        MakeReplyOrDirectStatus(False, False)
    End Sub

    Private Sub DeleteStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteStripMenuItem.Click, DelOpMenuItem.Click
        If _curTab Is Nothing OrElse _curList Is Nothing Then Exit Sub
        If _statuses.Tabs(_curTab.Text).TabType <> TabUsageType.DirectMessage Then
            Dim myPost As Boolean = False
            For Each idx As Integer In _curList.SelectedIndices
                If GetCurTabPost(idx).IsMe OrElse _
                   GetCurTabPost(idx).RetweetedBy.ToLower = Twitter.Username.ToLower Then
                    myPost = True
                    Exit For
                End If
            Next
            If Not myPost Then Exit Sub
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
                    rtn = Twitter.RemoveDirectMessage(Id)
                Else
                    If _statuses.Item(Id).IsMe OrElse _statuses.Item(Id).RetweetedBy.ToLower = Twitter.Username.ToLower Then
                        rtn = Twitter.RemoveStatus(Id)
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

    Private Sub ReadedStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReadedStripMenuItem.Click, ReadOpMenuItem.Click
        _curList.BeginUpdate()
        If SettingDialog.UnreadManage Then
            For Each idx As Integer In _curList.SelectedIndices
                _statuses.SetRead(True, _curTab.Text, idx)
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
                _statuses.SetRead(False, _curTab.Text, idx)
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
                Case Else
                    GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
            End Select
        Else
            GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
        End If
    End Sub

    Private Sub DoRefreshMore()
        If _curTab IsNot Nothing Then
            Select Case _statuses.Tabs(_curTab.Text).TabType
                'Case TabUsageType.Mentions
                '    GetTimeline(WORKERTYPE.Reply, 1, 0, "")
                'Case TabUsageType.DirectMessage
                '    GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, 0, "")
                'Case TabUsageType.Favorites
                '    GetTimeline(WORKERTYPE.Favorites, 1, 0, "")
                'Case TabUsageType.Profile
                '' TODO
                Case TabUsageType.PublicSearch
                    ' TODO
                    Dim tb As TabClass = _statuses.Tabs(_curTab.Text)
                    If tb.SearchWords = "" Then Exit Sub
                    GetTimeline(WORKERTYPE.PublicSearch, -1, 0, _curTab.Text)
                    'Case Else
                    '    GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
            End Select
        Else
            'GetTimeline(WORKERTYPE.Timeline, 1, 0, "")
        End If
    End Sub

    Private Sub SettingStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingStripMenuItem.Click, SettingFileMenuItem.Click
        'Dim chgUseApi As Boolean = False
        Dim result As DialogResult

        Try
            result = SettingDialog.ShowDialog()
        Catch ex As Exception
            Exit Sub
        End Try
        If result = Windows.Forms.DialogResult.OK Then
            SyncLock _syncObject
                'Twitter.Username = SettingDialog.UserID
                'Twitter.Password = SettingDialog.PasswordStr
                Try
                    If SettingDialog.TimelinePeriodInt > 0 Then
                        _homeCounterAdjuster = 0
                        'If SettingDialog.PeriodAdjust AndAlso Not SettingDialog.UseAPI Then
                        '    If SettingDialog.TimelinePeriodInt < _homeCounter - _homeCounterAdjuster Then
                        '        _homeCounter = SettingDialog.TimelinePeriodInt
                        '        _homeCounterAdjuster = 0
                        '    End If
                        'Else
                        '    _homeCounterAdjuster = 0
                        'End If
                    End If
                Catch ex As Exception
                    ex.Data("Instance") = "Set Timers"
                    ex.Data("IsTerminatePermission") = False
                    Throw
                End Try
                'Twitter.NextThreshold = SettingDialog.NextPageThreshold
                'Twitter.NextPages = SettingDialog.NextPagesInt
                'If Twitter.UseAPI <> SettingDialog.UseAPI AndAlso Not _initial Then
                '    chgUseApi = True
                'End If
                'Twitter.UseAPI = SettingDialog.UseAPI
                Twitter.CountApi = SettingDialog.CountApi
                Twitter.CountApiReply = SettingDialog.CountApiReply
                'Twitter.UsePostMethod = False
                Twitter.HubServer = SettingDialog.HubServer
                Twitter.TinyUrlResolve = SettingDialog.TinyUrlResolve
                Twitter.RestrictFavCheck = SettingDialog.RestrictFavCheck
                Twitter.ReadOwnPost = SettingDialog.ReadOwnPost
                Twitter.UseSsl = SettingDialog.UseSsl
                Twitter.BitlyId = SettingDialog.BitlyUser
                Twitter.BitlyKey = SettingDialog.BitlyPwd

                'Twitter.SelectedProxyType = SettingDialog.SelectedProxyType
                'Twitter.ProxyAddress = SettingDialog.ProxyAddress
                'Twitter.ProxyPort = SettingDialog.ProxyPort
                'Twitter.ProxyUser = SettingDialog.ProxyUser
                'Twitter.ProxyPassword = SettingDialog.ProxyPassword
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

                Outputz.outputzKey = SettingDialog.OutputzKey
                Outputz.outputzEnabled = SettingDialog.OutputzEnabled
                Select Case SettingDialog.OutputzUrlmode
                    Case OutputzUrlmode.twittercom
                        Outputz.outputzUrl = "http://twitter.com/"
                    Case OutputzUrlmode.twittercomWithUsername
                        Outputz.outputzUrl = "http://twitter.com/" + Twitter.Username
                End Select

            End SyncLock
        End If

        Twitter.AccountState = ACCOUNT_STATE.Valid

        Me.TopMost = SettingDialog.AlwaysTop
        SaveConfigsAll(False)

        'If chgUseApi AndAlso SettingDialog.OneWayLove Then doGetFollowersMenu(False) 'API使用を切り替えたら取り直し
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
                AddNewTabForSearch(hash)
                Exit Sub
            Else
                OpenUriAsync(e.Url.OriginalString)
            End If
        End If
    End Sub

    Private Sub AddNewTabForSearch(ByVal searchWord As String)
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
        _statuses.AddTab(tabName, TabUsageType.PublicSearch)
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

    Public Function AddNewTab(ByVal tabName As String, ByVal startup As Boolean, ByVal tabType As TabUsageType) As Boolean
        '重複チェック
        For Each tb As TabPage In ListTab.TabPages
            If tb.Text = tabName Then Return False
        Next

        '新規タブ名チェック
        If tabName = My.Resources.AddNewTabText1 Then Return False
        'If tabName <> ReplaceInvalidFilename(tabName) Then Return False

        'タブタイプ重複チェック
        If Not startup Then
            If tabType = TabUsageType.DirectMessage OrElse _
               tabType = TabUsageType.Favorites OrElse _
               tabType = TabUsageType.Home OrElse _
               tabType = TabUsageType.Mentions Then
                If _statuses.GetTabByType(tabType) IsNot Nothing Then Return False
            End If
        End If
        'Dim myTab As New TabStructure()

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
        'If Not _iconCol Then
        '_colHd2 = New ColumnHeader()
        '_colHd3 = New ColumnHeader()
        '_colHd4 = New ColumnHeader()
        '_colHd5 = New ColumnHeader()
        '_colHd6 = New ColumnHeader()
        '_colHd7 = New ColumnHeader()
        '_colHd8 = New ColumnHeader()
        '_colHd9 = New ColumnHeader()
        'End If

        'If Not startup Then _section.ListElement.Add(New ListElement(tabName))

        Dim cnt As Integer = ListTab.TabPages.Count

        '''ToDo:Create and set controls follow tabtypes

        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ListTab.SuspendLayout()
        Me.SuspendLayout()

        _tabPage.SuspendLayout()



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
            'AddHandler cmbLang.Enter, AddressOf SearchControls_Enter
            'AddHandler cmbLang.Leave, AddressOf SearchControls_Leave

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
            'AddHandler btn.Enter, AddressOf SearchControls_Enter
            'AddHandler btn.Leave, AddressOf SearchControls_Leave

        End If

        Me.ListTab.Controls.Add(_tabPage)
        _tabPage.Controls.Add(_listCustom)

        If tabType = TabUsageType.PublicSearch Then _tabPage.Controls.Add(pnl)

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
        _listCustom.ContextMenuStrip = Me.ContextMenuStrip2
        _listCustom.Dock = DockStyle.Fill
        _listCustom.FullRowSelect = True
        _listCustom.HideSelection = False
        _listCustom.Location = New Point(0, 0)
        _listCustom.Margin = New Padding(0)
        _listCustom.Name = "CList" + Environment.TickCount.ToString()
        _listCustom.ShowItemToolTips = True
        _listCustom.Size = New Size(380, 260)
        _listCustom.TabIndex = 4                                   'これ大丈夫？
        _listCustom.UseCompatibleStateImageBehavior = False
        _listCustom.View = View.Details
        _listCustom.OwnerDraw = True
        _listCustom.VirtualMode = True
        _listCustom.Font = _fntReaded
        _listCustom.BackColor = _clListBackcolor

        _listCustom.GridLines = SettingDialog.ShowGrid

        AddHandler _listCustom.SelectedIndexChanged, AddressOf MyList_SelectedIndexChanged
        AddHandler _listCustom.MouseDoubleClick, AddressOf MyList_MouseDoubleClick
        AddHandler _listCustom.ColumnClick, AddressOf MyList_ColumnClick
        AddHandler _listCustom.DrawColumnHeader, AddressOf MyList_DrawColumnHeader

        Select Case _iconSz
            Case 26, 48
                AddHandler _listCustom.DrawItem, AddressOf MyList_DrawItem
            Case Else
                AddHandler _listCustom.DrawItem, AddressOf MyList_DrawItemDefault
        End Select

        'AddHandler _listCustom.Scrolled, AddressOf Mylist_Scrolled
        AddHandler _listCustom.MouseClick, AddressOf MyList_MouseClick
        AddHandler _listCustom.ColumnReordered, AddressOf MyList_ColumnReordered
        AddHandler _listCustom.ColumnWidthChanged, AddressOf MyList_ColumnWidthChanged
        AddHandler _listCustom.CacheVirtualItems, AddressOf MyList_CacheVirtualItems
        AddHandler _listCustom.RetrieveVirtualItem, AddressOf MyList_RetrieveVirtualItem
        AddHandler _listCustom.DrawSubItem, AddressOf MyList_DrawSubItem
        'AddHandler _listCustom.KeyDown, AddressOf MyList_KeyDown

        _colHd1.Text = ""
        _colHd1.Width = 48
        'If Not _iconCol Then
        _colHd2.Text = My.Resources.AddNewTabText2
        _colHd2.Width = 80
        _colHd3.Text = My.Resources.AddNewTabText3
        _colHd3.Width = 300
        _colHd4.Text = My.Resources.AddNewTabText4_2
        'If SettingDialog.UseAPI Then
        '    _colHd4.Text = My.Resources.AddNewTabText4_2
        'Else
        '    _colHd4.Text = My.Resources.AddNewTabText4
        'End If
        _colHd4.Width = 50
        _colHd5.Text = My.Resources.AddNewTabText5
        _colHd5.Width = 50
        _colHd6.Text = ""
        _colHd6.Width = 16
        _colHd7.Text = ""
        _colHd7.Width = 16
        _colHd8.Text = "Source"
        _colHd8.Width = 50
        'End If

        If (_statuses.Tabs.ContainsKey(tabName) AndAlso _statuses.Tabs(tabName).TabType = TabUsageType.Mentions) _
           OrElse (Not _statuses.IsDefaultTab(tabName) AndAlso tabType <> TabUsageType.PublicSearch) Then
            TabDialog.AddTab(tabName)
        End If

        _listCustom.SmallImageList = TIconSmallList
        '_listCustom.ListViewItemSorter = listViewItemSorter
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

    Public Function RemoveSpecifiedTab(ByVal TabName As String) As Boolean
        Dim idx As Integer = 0
        For idx = 0 To ListTab.TabPages.Count - 1
            If ListTab.TabPages(idx).Text = TabName Then Exit For
        Next

        If _statuses.IsDefaultTab(TabName) Then Return False

        Dim tmp As String = String.Format(My.Resources.RemoveSpecifiedTabText1, Environment.NewLine)
        If MessageBox.Show(tmp, My.Resources.RemoveSpecifiedTabText2, _
                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
            Return False
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

        Select Case _iconSz
            Case 26, 48
                RemoveHandler _listCustom.DrawItem, AddressOf MyList_DrawItem
            Case Else
                RemoveHandler _listCustom.DrawItem, AddressOf MyList_DrawItemDefault
        End Select

        'RemoveHandler _listCustom.Scrolled, AddressOf Mylist_Scrolled
        RemoveHandler _listCustom.MouseClick, AddressOf MyList_MouseClick
        RemoveHandler _listCustom.ColumnReordered, AddressOf MyList_ColumnReordered
        RemoveHandler _listCustom.ColumnWidthChanged, AddressOf MyList_ColumnWidthChanged
        RemoveHandler _listCustom.CacheVirtualItems, AddressOf MyList_CacheVirtualItems
        RemoveHandler _listCustom.RetrieveVirtualItem, AddressOf MyList_RetrieveVirtualItem
        RemoveHandler _listCustom.DrawSubItem, AddressOf MyList_DrawSubItem
        'RemoveHandler _listCustom.KeyDown, AddressOf MyList_KeyDown

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

        'SaveConfigsCommon()
        'SaveConfigsTab(False)

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
    End Sub

    Private Sub ListTab_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListTab.MouseMove
        'タブのD&D
        Dim cpos As New Point(e.X, e.Y)

        If e.Button = Windows.Forms.MouseButtons.Left AndAlso _tabDrag Then
            Dim tn As String = ""
            For i As Integer = 0 To ListTab.TabPages.Count - 1
                Dim rect As Rectangle = ListTab.GetTabRect(i)
                If rect.Left <= cpos.X AndAlso cpos.X <= rect.Right AndAlso _
                   rect.Top <= cpos.Y AndAlso cpos.Y <= rect.Bottom Then
                    tn = ListTab.TabPages(i).Text
                    Exit For
                End If
            Next

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
        SetStatusLabel()
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
            SetStatusLabel()
        End If
    End Sub

    Private Sub StatusText_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles StatusText.KeyPress
        If e.KeyChar = "@" Then
            If Not SettingDialog.UseAtIdSupplement Then Exit Sub
            '@マーク
            ShowSuplDialog(AtIdSupl)
            e.Handled = True
        ElseIf e.KeyChar = "#" Then
            If Not SettingDialog.UseHashSupplement Then Exit Sub
            ShowSuplDialog(HashSupl)
            e.Handled = True
        End If
    End Sub

    Private Sub ShowSuplDialog(ByVal dialog As AtIdSupplement)
        dialog.ShowDialog()
        Me.TopMost = SettingDialog.AlwaysTop
        If dialog.inputText <> "" Then
            Dim fHalf As String = ""
            Dim eHalf As String = ""
            Dim selStart As Integer = StatusText.SelectionStart
            If selStart > 0 Then
                fHalf = StatusText.Text.Substring(0, selStart)
            End If
            If selStart < StatusText.Text.Length Then
                eHalf = StatusText.Text.Substring(selStart)
            End If
            StatusText.Text = fHalf + dialog.inputText + eHalf
            StatusText.SelectionStart = selStart + dialog.inputText.Length
        End If
    End Sub

    Private Sub StatusText_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles StatusText.KeyUp
        'スペースキーで未読ジャンプ
        If Not e.Alt AndAlso Not e.Control AndAlso Not e.Shift Then
            If e.KeyCode = Keys.Space OrElse e.KeyCode = Keys.ProcessKey Then
                If StatusText.Text = " " OrElse StatusText.Text = "　" Then
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
        If (isAuto AndAlso Not My.Computer.Keyboard.ShiftKeyDown) OrElse _
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
                e.Item = New ListViewItem(sitem, -1)
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
        Dim mk As String = ""
        If Post.IsMark Then mk += "♪"
        If Post.IsProtect Then mk += "Ю"
        If Post.InReplyToId > 0 Then mk += "⇒"
        Dim itm As ListViewItem
        If Post.RetweetedId = 0 Then
            Dim sitem() As String = {"", Post.Nickname, Post.Data, Post.PDate.ToString(SettingDialog.DateTimeFormat), Post.Name, "", mk, Post.Source}
            itm = New ListViewItem(sitem, Post.ImageIndex)
        Else
            Dim sitem() As String = {"", Post.Nickname, Post.Data, Post.PDate.ToString(SettingDialog.DateTimeFormat), Post.Name + "(RT:" + Post.RetweetedBy + ")", "", mk, Post.Source}
            itm = New ListViewItem(sitem, Post.ImageIndex)
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

    Private Sub MyList_DrawItemDefault(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawListViewItemEventArgs)
        e.DrawDefault = True
    End Sub

    Private Sub MyList_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawListViewItemEventArgs)
        'アイコンサイズ26,48はオーナードロー（DrawSubItem発生させる）
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
    End Sub

    Private Sub MyList_DrawSubItem(ByVal sender As Object, ByVal e As DrawListViewSubItemEventArgs)
        If e.ItemState = 0 Then Exit Sub
        If e.ColumnIndex > 0 Then
            Dim rct As RectangleF = e.Bounds
            Dim rctB As RectangleF = e.Bounds
            rct.Width = e.Header.Width
            rctB.Width = e.Header.Width
            If _iconCol Then rct.Height = e.Item.Font.Height
            'アイコン以外の列
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
                        e.Graphics.DrawString(System.Environment.NewLine + e.Item.SubItems(2).Text, e.Item.Font, brs, rctB, sf)
                        e.Graphics.DrawString(e.Item.SubItems(4).Text + " / " + e.Item.SubItems(1).Text + " (" + e.Item.SubItems(3).Text + ") " + e.Item.SubItems(5).Text + e.Item.SubItems(6).Text + " [" + e.Item.SubItems(7).Text + "]", fnt, brs, rct, sf)
                        fnt.Dispose()
                    Else
                        e.Graphics.DrawString(e.SubItem.Text, e.Item.Font, brs, rct, sf)
                    End If
                End If
                If flg Then brs.Dispose()
            Else
                If rct.Width > 0 Then
                    '選択中の行
                    Dim fnt As New Font(e.Item.Font, FontStyle.Bold)
                    If DirectCast(sender, Windows.Forms.Control).Focused Then
                        If _iconCol Then
                            e.Graphics.DrawString(System.Environment.NewLine + e.Item.SubItems(2).Text, e.Item.Font, _brsHighLightText, rctB, sf)
                            e.Graphics.DrawString(e.Item.SubItems(4).Text + " / " + e.Item.SubItems(1).Text + " (" + e.Item.SubItems(3).Text + ") " + e.Item.SubItems(5).Text + e.Item.SubItems(6).Text + " [" + e.Item.SubItems(7).Text + "]", fnt, _brsHighLightText, rct, sf)
                        Else
                            e.Graphics.DrawString(e.SubItem.Text, e.Item.Font, _brsHighLightText, rct, sf)
                        End If
                    Else
                        If _iconCol Then
                            e.Graphics.DrawString(System.Environment.NewLine + e.Item.SubItems(2).Text, e.Item.Font, _brsForeColorUnread, rctB, sf)
                            e.Graphics.DrawString(e.Item.SubItems(4).Text + " / " + e.Item.SubItems(1).Text + " (" + e.Item.SubItems(3).Text + ") " + e.Item.SubItems(5).Text + e.Item.SubItems(6).Text + " [" + e.Item.SubItems(7).Text + "]", fnt, _brsForeColorUnread, rct, sf)
                        Else
                            e.Graphics.DrawString(e.SubItem.Text, e.Item.Font, _brsForeColorUnread, rct, sf)
                        End If
                    End If
                    fnt.Dispose()
                End If
            End If
        Else
            'アイコン列はデフォルト描画
            e.DrawDefault = True
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
                            OrElse _search.IsMatch(post.Data, regOpt) _
                            OrElse _search.IsMatch(post.Name, regOpt) _
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
                        OrElse post.Data.IndexOf(_word, fndOpt) > -1 _
                        OrElse post.Name.IndexOf(_word, fndOpt) > -1 _
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

        '現在タブから最終タブまで探索
        For i As Integer = bgnIdx To ListTab.TabPages.Count - 1
            '未読Index取得
            idx = _statuses.GetOldestUnreadId(ListTab.TabPages(i).Text)
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
                idx = _statuses.GetOldestUnreadId(ListTab.TabPages(i).Text)
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
                OpenUriAsync("http://twitter.com/" + post.Name + "/status/" + post.Id.ToString)
            Else
                OpenUriAsync("http://twitter.com/" + post.Name + "/status/" + post.RetweetedId.ToString)
            End If
        End If
    End Sub

    Private Sub FavorareMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles FavorareMenuItem.Click, OpenFavotterOpMenuItem.Click
        If _curList.SelectedIndices.Count > 0 Then
            Dim post As PostClass = _statuses.Item(_curTab.Text, _curList.SelectedIndices(0))
            OpenUriAsync("http://favotter.net/user.php?user=" + post.Name)
        End If
    End Sub

    Private Sub VerUpMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VerUpMenuItem.Click
        CheckNewVersion()
    End Sub

    Private Sub RunTweenUp()

        Dim pinfo As New ProcessStartInfo
        pinfo.UseShellExecute = True
        pinfo.WorkingDirectory = Application.StartupPath
        pinfo.FileName = Path.Combine(Application.StartupPath(), "TweenUp.exe")
        Try
            Process.Start(pinfo)
        Catch ex As Exception
            MsgBox("Failed to execute TweenUp.exe.")
        End Try
    End Sub

    Private Sub CheckNewVersion(Optional ByVal startup As Boolean = False)
        Dim retMsg As String = ""
        Dim strVer As String = ""
        Dim strDetail As String = ""
        Dim forceUpdate As Boolean = My.Computer.Keyboard.ShiftKeyDown

        Try
            retMsg = Twitter.GetVersionInfo()
        Catch ex As Exception
            StatusLabel.Text = My.Resources.CheckNewVersionText9
            If Not startup Then MessageBox.Show(My.Resources.CheckNewVersionText10, My.Resources.CheckNewVersionText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End Try
        If retMsg.Length > 0 Then
            strVer = retMsg.Substring(0, 4)
            If retMsg.Length > 4 Then
                strDetail = retMsg.Substring(5).Trim
            End If
            If strVer.CompareTo(fileVersion.Replace(".", "")) > 0 Then
                Dim tmp As String = String.Format(My.Resources.CheckNewVersionText3, strVer)
                Using dialogAsShieldicon As New DialogAsShieldIcon
                    If dialogAsShieldicon.Show(tmp, strDetail, My.Resources.CheckNewVersionText1, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                        retMsg = Twitter.GetTweenBinary(strVer)
                        If retMsg.Length = 0 Then
                            RunTweenUp()
                            'If startup Then
                            '    Application.Exit()
                            'Else
                            _endingFlag = True
                            dialogAsShieldicon.Dispose()
                            Me.Close()
                            'End If
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
                            retMsg = Twitter.GetTweenBinary(strVer)
                            If retMsg.Length = 0 Then
                                RunTweenUp()
                                'If startup Then
                                '    Application.Exit()
                                'Else
                                _endingFlag = True
                                dialogAsShieldicon.Dispose()
                                Me.Close()
                                'End If
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

    Private Sub TimerColorize_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerColorize.Tick
        If TimerColorize.Enabled = False Then Exit Sub

        TimerColorize.Stop()
        TimerColorize.Enabled = False
        TimerColorize.Interval = 200
        DispSelectedPost()
        '件数関連の場合、タイトル即時書き換え
        If SettingDialog.DispLatestPost <> DispTitleEnum.None AndAlso _
           SettingDialog.DispLatestPost <> DispTitleEnum.Post AndAlso _
           SettingDialog.DispLatestPost <> DispTitleEnum.Ver Then
            SetMainWindowTitle()
        End If
        If Not StatusLabelUrl.Text.StartsWith("http") Then SetStatusLabel()
        For Each tb As TabPage In ListTab.TabPages
            If _statuses.Tabs(tb.Text).UnreadCount = 0 Then
                If SettingDialog.TabIconDisp Then
                    If tb.ImageIndex = 0 Then tb.ImageIndex = -1
                End If
            End If
        Next
        If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
    End Sub

    Private Sub DispSelectedPost()

        If _curList.SelectedIndices.Count = 0 OrElse _curPost Is Nothing Then Exit Sub

        Dim dTxt As String = detailHtmlFormatHeader + _curPost.OriginalData + detailHtmlFormatFooter
        If _statuses.Tabs(_curTab.Text).TabType = TabUsageType.DirectMessage AndAlso _curPost.IsOwl Then
            NameLabel.Text = "DM TO -> "
        ElseIf _statuses.Tabs(_curTab.Text).TabType = TabUsageType.DirectMessage Then
            NameLabel.Text = "DM FROM <- "
        Else
            NameLabel.Text = ""
        End If
        NameLabel.Text += _curPost.Name + "/" + _curPost.Nickname
        If Not String.IsNullOrEmpty(_curPost.RetweetedBy) Then
            NameLabel.Text += " (RT:" + _curPost.RetweetedBy + ")"
        End If
        'If UserPicture.Image IsNot Nothing Then UserPicture.Image.Dispose()
        If _curPost.ImageIndex > -1 Then
            UserPicture.Image = TIconDic(_curPost.ImageUrl)
        Else
            UserPicture.Image = Nothing
        End If
        'UserPicture.Refresh()

        NameLabel.ForeColor = System.Drawing.SystemColors.ControlText
        DateTimeLabel.Text = _curPost.PDate.ToString()
        If _curPost.IsOwl AndAlso (SettingDialog.OneWayLove OrElse _statuses.Tabs(_curTab.Text).TabType = TabUsageType.DirectMessage) Then NameLabel.ForeColor = _clOWL
        If _curPost.RetweetedId > 0 Then NameLabel.ForeColor = _clRetweet
        If _curPost.IsFav Then NameLabel.ForeColor = _clFav

        If DumpPostClassToolStripMenuItem.Checked Then
            Dim sb As New StringBuilder(512)

            sb.Append("-----Start PostClass Dump<br>")
            sb.AppendFormat("Data           : {0}<br>", _curPost.Data)
            sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", _curPost.Data)
            sb.AppendFormat("Id             : {0}<br>", _curPost.Id.ToString)
            sb.AppendFormat("ImageIndex     : {0}<br>", _curPost.ImageIndex.ToString)
            sb.AppendFormat("ImageUrl       : {0}<br>", _curPost.ImageUrl)
            sb.AppendFormat("InReplyToId    : {0}<br>", _curPost.InReplyToId.ToString)
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

            sb.AppendFormat("Name           : {0}<br>", _curPost.Name)
            sb.AppendFormat("NickName       : {0}<br>", _curPost.Nickname)
            sb.AppendFormat("OriginalData   : {0}<br>", _curPost.OriginalData)
            sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", _curPost.OriginalData)
            sb.AppendFormat("PDate          : {0}<br>", _curPost.PDate.ToString)
            sb.AppendFormat("Source         : {0}<br>", _curPost.Source)
            sb.Append("-----End PostClass Dump<br>")

            PostBrowser.Visible = False
            PostBrowser.DocumentText = detailHtmlFormatHeader + sb.ToString + detailHtmlFormatFooter
            PostBrowser.Visible = True
        Else
            Try
                If PostBrowser.DocumentText <> dTxt Then
                    PostBrowser.Visible = False
                    PostBrowser.DocumentText = dTxt
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

    Private Sub ListTab_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ListTab.KeyDown
        If ListTab.SelectedTab IsNot Nothing Then
            If _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.PublicSearch Then
                Dim pnl As Control = ListTab.SelectedTab.Controls("panelSearch")
                If pnl.Controls("comboSearch").Focused OrElse _
                   pnl.Controls("comboLang").Focused OrElse _
                   pnl.Controls("buttonSearch").Focused Then Exit Sub
            End If
        End If
        If e.Modifiers = Keys.None Then
            ' ModifierKeyが押されていない場合
            If e.KeyCode = Keys.N OrElse e.KeyCode = Keys.Right Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoRelPost(True)
                Exit Sub
            End If
            If e.KeyCode = Keys.P OrElse e.KeyCode = Keys.Left Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoRelPost(False)
                Exit Sub
            End If
            If e.KeyCode = Keys.OemPeriod Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoAnchor()
                Exit Sub
            End If
            _anchorFlag = False
            If e.KeyCode = Keys.Space OrElse e.KeyCode = Keys.ProcessKey Then
                e.Handled = True
                e.SuppressKeyPress = True
                JumpUnreadMenuItem_Click(Nothing, Nothing)
            End If
            If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Return Then
                e.Handled = True
                e.SuppressKeyPress = True
                MakeReplyOrDirectStatus()
            End If
            If e.KeyCode = Keys.L Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoPost(True)
            End If
            If e.KeyCode = Keys.H Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoPost(False)
            End If
            If e.KeyCode = Keys.Z Or e.KeyCode = Keys.Oemcomma Then
                e.Handled = True
                e.SuppressKeyPress = True
                MoveTop()
            End If
            If e.KeyCode = Keys.R Then
                e.Handled = True
                e.SuppressKeyPress = True
                DoRefresh()
            End If
            If e.KeyCode = Keys.S Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoNextTab(True)
            End If
            If e.KeyCode = Keys.A Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoNextTab(False)
            End If
            'If e.KeyCode = Keys.OemQuestion Then
            '    e.Handled = True
            '    e.SuppressKeyPress = True
            '    MenuItemSubSearch_Click(Nothing, Nothing)   '/検索
            'End If
            If e.KeyCode = Keys.F Then
                e.Handled = True
                e.SuppressKeyPress = True
                SendKeys.Send("{PGDN}")
            End If
            If e.KeyCode = Keys.B Then
                e.Handled = True
                e.SuppressKeyPress = True
                SendKeys.Send("{PGUP}")
            End If
        End If
        _anchorFlag = False
        If e.Control AndAlso Not e.Alt AndAlso Not e.Shift Then
            ' CTRLキーが押されている場合
            If e.KeyCode = Keys.Home OrElse e.KeyCode = Keys.End Then
                TimerColorize.Stop()
                TimerColorize.Start()
            End If
            If e.KeyCode = Keys.N Then GoNextTab(True)
            If e.KeyCode = Keys.P Then GoNextTab(False)
            'If e.KeyCode = Keys.F Then
            '    e.Handled = True
            '    e.SuppressKeyPress = True
            '    MovePageScroll(True)
            'End If
            'If e.KeyCode = Keys.B Then
            '    e.Handled = True
            '    e.SuppressKeyPress = True
            '    MovePageScroll(False)
            'End If
        End If
        If Not e.Control AndAlso e.Alt AndAlso Not e.Shift Then
            ' ALTキーが押されている場合
            ' 別タブの同じ書き込みへ(ALT+←/→)
            If e.KeyCode = Keys.Right Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoSamePostToAnotherTab(False)
            End If
            If e.KeyCode = Keys.Left Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoSamePostToAnotherTab(True)
            End If
        End If
        If e.Shift AndAlso Not e.Control AndAlso Not e.Alt Then
            ' SHIFTキーが押されている場合
            If e.KeyCode = Keys.H Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoTopEnd(True)
            End If
            If e.KeyCode = Keys.L Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoTopEnd(False)
            End If
            If e.KeyCode = Keys.M Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoMiddle()
            End If
            If e.KeyCode = Keys.G Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoLast()
            End If
            If e.KeyCode = Keys.Z Then
                e.Handled = True
                e.SuppressKeyPress = True
                MoveMiddle()
            End If

            ' お気に入り前後ジャンプ(SHIFT+N←/P→)
            If e.KeyCode = Keys.N OrElse e.KeyCode = Keys.Right Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoFav(True)
            End If
            If e.KeyCode = Keys.P OrElse e.KeyCode = Keys.Left Then
                e.Handled = True
                e.SuppressKeyPress = True
                GoFav(False)
            End If
            'If e.KeyCode = Keys.B Then
            '    e.Handled = True
            '    e.SuppressKeyPress = True
            '    UnreadStripMenuItem_Click(Nothing, Nothing)
            'End If
            If e.KeyCode = Keys.R Then
                e.Handled = True
                e.SuppressKeyPress = True
                DoRefreshMore()
            End If
        End If
        If Not e.Alt Then
            If e.KeyCode = Keys.J Then
                e.Handled = True
                e.SuppressKeyPress = True
                SendKeys.Send("{DOWN}")
            End If
            If e.KeyCode = Keys.K Then
                e.Handled = True
                e.SuppressKeyPress = True
                SendKeys.Send("{UP}")
            End If
        End If
        If e.KeyCode = Keys.C Then
            Dim clstr As String = ""
            If e.Control AndAlso Not e.Alt AndAlso Not e.Shift Then
                e.Handled = True
                e.SuppressKeyPress = True
                CopyStot()
            End If
            If e.Control AndAlso e.Shift AndAlso Not e.Alt Then
                e.Handled = True
                e.SuppressKeyPress = True
                CopyIdUri()
            End If
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
        For Each idx As Integer In _curList.SelectedIndices
            Dim post As PostClass = _statuses.Item(_curTab.Text, idx)
            If post.IsProtect AndAlso SettingDialog.ProtectNotInclude Then Continue For
            If post.RetweetedId = 0 Then
                sb.AppendFormat("{0}:{1} [http://twitter.com/{0}/status/{2}]{3}", post.Name, post.Data, post.Id, Environment.NewLine)
            Else
                sb.AppendFormat("{0}:{1} [http://twitter.com/{2}/status/{3}]{4}", post.Name, post.Data, post.RetweetedBy, post.Id, Environment.NewLine)
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

    Private Sub CopyIdUri()
        Dim clstr As String = ""
        Dim sb As New StringBuilder()
        For Each idx As Integer In _curList.SelectedIndices
            Dim post As PostClass = _statuses.Item(_curTab.Text, idx)
            If String.IsNullOrEmpty(post.RetweetedBy) Then
                sb.AppendFormat("http://twitter.com/{0}/status/{1}{2}", post.Name, post.Id, Environment.NewLine)
            Else
                sb.AppendFormat("http://twitter.com/{0}/status/{1}{2}", post.RetweetedBy, post.Id, Environment.NewLine)
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

        targetId = GetCurTabPost(_curList.SelectedIndices(0)).Id

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
            '_itemCache = Nothing
            '_postCache = Nothing
            For idx As Integer = 0 To DirectCast(ListTab.TabPages(tabidx).Tag, DetailsListView).VirtualListSize - 1
                If _statuses.Item(ListTab.TabPages(tabidx).Text, idx).Id = targetId Then
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
        '_itemCache = Nothing
        '_postCache = Nothing
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
            name = _curPost.Name
        Else
            name = _curPost.RetweetedBy
        End If
        For idx As Integer = fIdx To toIdx Step stp
            If _statuses.Item(_curTab.Text, idx).RetweetedId = 0 Then
                If _statuses.Item(_curTab.Text, idx).Name = name Then
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
            If post.Name = _anchorPost.Name OrElse _
               post.RetweetedBy = _anchorPost.Name OrElse _
               post.Name = _anchorPost.RetweetedBy OrElse _
               (Not String.IsNullOrEmpty(post.RetweetedBy) AndAlso post.RetweetedBy = _anchorPost.RetweetedBy) OrElse _
               _anchorPost.ReplyToList.Contains(post.Name.ToLower()) OrElse _
               _anchorPost.ReplyToList.Contains(post.RetweetedBy.ToLower()) OrElse _
               post.ReplyToList.Contains(_anchorPost.Name.ToLower()) OrElse _
               post.ReplyToList.Contains(_anchorPost.RetweetedBy.ToLower()) Then
                SelectListItem(_curList, idx)
                _curList.EnsureVisible(idx)
                Exit For
            End If
        Next
    End Sub

    Private Sub GoAnchor()
        If _anchorPost Is Nothing Then Exit Sub
        Dim idx As Integer = _statuses.Tabs(_curTab.Text).IndexOf(_anchorPost.Id)
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

    'Private Sub MovePageScroll(ByVal down As Boolean)
    '    Dim _item As ListViewItem
    '    Dim idx As Integer

    '    If down Then
    '        _item = _curList.GetItemAt(0, _curList.ClientSize.Height - 25)
    '        If _item Is Nothing Then
    '            If _curList.VirtualListSize > 0 Then
    '                SelectListItem(_curList, _curList.VirtualListSize - 1)
    '                _curList.EnsureVisible(_curList.VirtualListSize - 1)
    '            End If
    '            Exit Sub
    '        End If

    '        idx = _item.Index
    '        Dim idx2 As Integer = -1
    '        If _curList.Focused Then
    '            idx2 = _curList.FocusedItem.Index
    '        End If
    '        If idx2 >= idx Then
    '            'スクロール
    '            Dim idx3 As Integer = 0
    '            _item = _curList.GetItemAt(0, 25)
    '            If _item IsNot Nothing Then
    '                idx3 = _item.Index
    '            End If
    '            Dim rowCount As Integer = idx - idx3
    '            Dim toIndex As Integer = 0
    '            If idx2 + rowCount > _curList.VirtualListSize - 1 Then
    '                toIndex = _curList.VirtualListSize - 1
    '            Else
    '                toIndex = idx2 + rowCount
    '            End If
    '            SelectListItem(_curList, toIndex)
    '            _curList.EnsureVisible(toIndex)
    '        Else
    '            '最下行を選択
    '            SelectListItem(_curList, idx)
    '        End If
    '    Else
    '        _item = _curList.GetItemAt(0, 25)
    '        If _item Is Nothing Then
    '            If _curList.VirtualListSize > 0 Then
    '                SelectListItem(_curList, 0)
    '                _curList.EnsureVisible(0)
    '            End If
    '            Exit Sub
    '        End If

    '        idx = _item.Index
    '        Dim idx2 As Integer = -1
    '        If _curList.Focused Then
    '            idx2 = _curList.FocusedItem.Index
    '        End If
    '        If idx2 <= idx Then
    '            'スクロール
    '            Dim idx3 As Integer = 0
    '            _item = _curList.GetItemAt(0, _curList.ClientSize.Height - 25)
    '            If _item IsNot Nothing Then
    '                idx3 = _item.Index
    '            End If
    '            Dim rowCount As Integer = idx3 - idx
    '            Dim toIndex As Integer = 0
    '            If idx2 - rowCount < 0 Then
    '                toIndex = 0
    '            Else
    '                toIndex = idx2 - rowCount
    '            End If
    '            SelectListItem(_curList, toIndex)
    '            _curList.EnsureVisible(toIndex)
    '        Else
    '            '最上行を選択
    '            SelectListItem(_curList, idx)
    '        End If
    '    End If

    'End Sub

    Private Sub MyList_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        _anchorFlag = False
    End Sub

    Private Sub StatusText_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusText.Enter
        ' フォーカスの戻り先を StatusText に設定
        Me.Tag = StatusText
        StatusText.BackColor = _clInputBackcolor
    End Sub

    Private Sub StatusText_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusText.Leave
        ' フォーカスがメニューに遷移しないならばフォーカスはタブに移ることを期待
        If ListTab.SelectedTab IsNot Nothing AndAlso MenuStrip1.Tag Is Nothing Then Me.Tag = ListTab.SelectedTab.Tag
        StatusText.BackColor = Color.FromKnownColor(KnownColor.Window)
    End Sub

    Private Sub StatusText_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles StatusText.KeyDown
        If e.Control AndAlso Not e.Alt AndAlso Not e.Shift Then
            If e.KeyCode = Keys.A Then
                StatusText.SelectAll()
            ElseIf e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
                If StatusText.Text.Trim() <> "" Then _history(_hisIdx) = StatusText.Text
                If e.KeyCode = Keys.Up Then
                    _hisIdx -= 1
                    If _hisIdx < 0 Then _hisIdx = 0
                Else
                    _hisIdx += 1
                    If _hisIdx > _history.Count - 1 Then _hisIdx = _history.Count - 1
                End If
                StatusText.Text = _history(_hisIdx)
                StatusText.SelectionStart = StatusText.Text.Length
                e.Handled = True
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.PageUp Then
                If ListTab.SelectedIndex = 0 Then
                    ListTab.SelectedIndex = ListTab.TabCount - 1
                Else
                    ListTab.SelectedIndex -= 1
                End If
                e.Handled = True
                e.SuppressKeyPress = True
                StatusText.Focus()
            ElseIf e.KeyCode = Keys.PageDown Then
                If ListTab.SelectedIndex = ListTab.TabCount - 1 Then
                    ListTab.SelectedIndex = 0
                Else
                    ListTab.SelectedIndex += 1
                End If
                e.Handled = True
                e.SuppressKeyPress = True
                StatusText.Focus()
            End If
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
            If modifySettingCommon Then SaveConfigsCommon()
            If modifySettingLocal Then SaveConfigsLocal()
            If modifySettingAtId AndAlso SettingDialog.UseAtIdSupplement AndAlso AtIdSupl IsNot Nothing Then
                modifySettingAtId = False
                Dim cfgAtId As New SettingAtIdList(AtIdSupl.GetItemList)
                cfgAtId.Save()
            End If
        End If
    End Sub

    Private Sub SaveConfigsCommon()
        If _ignoreConfigSave Then Exit Sub

        modifySettingCommon = False
        SyncLock _syncObject
            _cfgCommon.UserName = Twitter.Username
            '_cfgCommon.Password = SettingDialog.PasswordStr
            _cfgCommon.Token = Twitter.AccessToken
            _cfgCommon.TokenSecret = Twitter.AccessTokenSecret
            '_cfgCommon.NextPageThreshold = SettingDialog.NextPageThreshold
            '_cfgCommon.NextPages = SettingDialog.NextPagesInt
            _cfgCommon.TimelinePeriod = SettingDialog.TimelinePeriodInt
            _cfgCommon.ReplyPeriod = SettingDialog.ReplyPeriodInt
            _cfgCommon.DMPeriod = SettingDialog.DMPeriodInt
            _cfgCommon.PubSearchPeriod = SettingDialog.PubSearchPeriodInt
            _cfgCommon.MaxPostNum = SettingDialog.MaxPostNum
            '_cfgCommon.ReadPages = SettingDialog.ReadPages
            '_cfgCommon.ReadPagesReply = SettingDialog.ReadPagesReply
            '_cfgCommon.ReadPagesDM = SettingDialog.ReadPagesDM
            _cfgCommon.Read = SettingDialog.Readed
            _cfgCommon.IconSize = SettingDialog.IconSz
            _cfgCommon.UnreadManage = SettingDialog.UnreadManage
            _cfgCommon.PlaySound = SettingDialog.PlaySound
            _cfgCommon.OneWayLove = SettingDialog.OneWayLove

            _cfgCommon.NameBalloon = SettingDialog.NameBalloon
            _cfgCommon.PostCtrlEnter = SettingDialog.PostCtrlEnter
            '_cfgCommon.UseApi = SettingDialog.UseAPI
            _cfgCommon.CountApi = SettingDialog.CountApi
            _cfgCommon.CountApiReply = SettingDialog.CountApiReply
            '_cfgCommon.UsePostMethod = False
            _cfgCommon.HubServer = SettingDialog.HubServer
            '_cfgCommon.CheckReply = SettingDialog.CheckReply
            _cfgCommon.PostAndGet = SettingDialog.PostAndGet
            _cfgCommon.DispUsername = SettingDialog.DispUsername
            _cfgCommon.MinimizeToTray = SettingDialog.MinimizeToTray
            _cfgCommon.CloseToExit = SettingDialog.CloseToExit
            _cfgCommon.DispLatestPost = SettingDialog.DispLatestPost
            _cfgCommon.SortOrderLock = SettingDialog.SortOrderLock
            _cfgCommon.TinyUrlResolve = SettingDialog.TinyUrlResolve
            _cfgCommon.PeriodAdjust = SettingDialog.PeriodAdjust
            _cfgCommon.StartupVersion = SettingDialog.StartupVersion
            '_cfgCommon.StartupKey = SettingDialog.StartupKey
            _cfgCommon.StartupFollowers = SettingDialog.StartupFollowers
            '_cfgCommon.StartupApiModeNoWarning = SettingDialog.StartupAPImodeNoWarning
            _cfgCommon.RestrictFavCheck = SettingDialog.RestrictFavCheck
            _cfgCommon.AlwaysTop = SettingDialog.AlwaysTop
            _cfgCommon.UrlConvertAuto = SettingDialog.UrlConvertAuto
            _cfgCommon.Outputz = SettingDialog.OutputzEnabled
            _cfgCommon.OutputzKey = SettingDialog.OutputzKey
            _cfgCommon.OutputzUrlMode = SettingDialog.OutputzUrlmode
            _cfgCommon.UseUnreadStyle = SettingDialog.UseUnreadStyle
            _cfgCommon.DateTimeFormat = SettingDialog.DateTimeFormat
            _cfgCommon.DefaultTimeOut = SettingDialog.DefaultTimeOut
            _cfgCommon.ProtectNotInclude = SettingDialog.ProtectNotInclude
            _cfgCommon.LimitBalloon = SettingDialog.LimitBalloon
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

            _cfgCommon.Save()
        End SyncLock
    End Sub

    Private Sub SaveConfigsLocal()
        If _ignoreConfigSave Then Exit Sub
        SyncLock _syncObject
            modifySettingLocal = False
            _cfgLocal.FormSize = _mySize
            _cfgLocal.FormLocation = _myLoc
            _cfgLocal.SplitterDistance = _mySpDis
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

    'Private Sub SaveConfigsTab(ByVal DeleteBefore As Boolean)
    '    If _ignoreConfigSave Then Exit Sub
    '    Dim cnt As Integer = 0
    '    If ListTab IsNot Nothing AndAlso _
    '       ListTab.TabPages IsNot Nothing AndAlso _
    '       ListTab.TabPages.Count > 0 Then
    '        If DeleteBefore Then SettingTab.DeleteConfigFile() '旧設定ファイル削除
    '        For cnt = 0 To ListTab.TabPages.Count - 1
    '            SaveConfigsTab(ListTab.TabPages(cnt).Text)
    '        Next
    '    End If
    'End Sub

    'Private Sub SaveConfigsTab(ByVal tabName As String)
    '    If _ignoreConfigSave Then Exit Sub
    '    SyncLock _syncObject
    '        Dim tabSetting As New SettingTab
    '        tabSetting.Tab = _statuses.Tabs(tabName)
    '        tabSetting.Save()
    '    End SyncLock
    'End Sub

    Private Sub SaveConfigsTabs()
        Dim tabSetting As New SettingTabs
        For i As Integer = 0 To ListTab.TabPages.Count - 1
            tabSetting.Tabs.Add(_statuses.Tabs(ListTab.TabPages(i).Text))
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
                                 """" & post.Data.Replace(vbLf, "").Replace("""", """""") + """" & vbTab & _
                                 post.PDate.ToString() & vbTab & _
                                 post.Name & vbTab & _
                                 post.Id.ToString() & vbTab & _
                                 post.ImageUrl & vbTab & _
                                 """" & post.OriginalData.Replace(vbLf, "").Replace("""", """""") + """" & vbTab & _
                                 protect)
                    Next
                Else
                    For Each idx As Integer In _curList.SelectedIndices
                        Dim post As PostClass = _statuses.Item(_curTab.Text, idx)
                        Dim protect As String = ""
                        If post.IsProtect Then protect = "Protect"
                        sw.WriteLine(post.Nickname & vbTab & _
                                 """" & post.Data.Replace(vbLf, "").Replace("""", """""") + """" & vbTab & _
                                 post.PDate.ToString() & vbTab & _
                                 post.Name & vbTab & _
                                 post.Id.ToString() & vbTab & _
                                 post.ImageUrl & vbTab & _
                                 """" & post.OriginalData.Replace(vbLf, "").Replace("""", """""") + """" & vbTab & _
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
        If e.KeyCode = Keys.F5 OrElse e.KeyCode = Keys.R Then
            e.IsInputKey = True
            DoRefresh()
        End If
        If e.Modifiers = Keys.None AndAlso (e.KeyCode = Keys.Space OrElse e.KeyCode = Keys.ProcessKey) Then
            e.IsInputKey = True
            JumpUnreadMenuItem_Click(Nothing, Nothing)
        End If
    End Sub

    Public Function TabRename(ByRef tabName As String) As Boolean
        'タブ名変更
        'If _statuses.IsDefaultTab(tabName) Then Return False
        Dim newTabText As String = Nothing
        Using inputName As New InputTabName()
            inputName.TabName = tabName
            inputName.ShowDialog()
            newTabText = inputName.TabName
            inputName.Dispose()
        End Using
        Me.TopMost = SettingDialog.AlwaysTop
        If newTabText <> "" Then
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
                   (Not _statuses.IsDefaultTab(ListTab.TabPages(i).Text) AndAlso _statuses.Tabs(ListTab.TabPages(i).Text).TabType <> TabUsageType.PublicSearch) Then
                    TabDialog.RemoveTab(ListTab.TabPages(i).Text)
                End If
                If ListTab.TabPages(i).Text = tabName Then
                    ListTab.TabPages(i).Text = newTabText
                End If
            Next
            _statuses.RenameTab(tabName, newTabText)

            For i As Integer = 0 To ListTab.TabCount - 1
                If _statuses.Tabs(ListTab.TabPages(i).Text).TabType = TabUsageType.Mentions OrElse _
                   (Not _statuses.IsDefaultTab(ListTab.TabPages(i).Text) AndAlso _statuses.Tabs(ListTab.TabPages(i).Text).TabType <> TabUsageType.PublicSearch) Then
                    If ListTab.TabPages(i).Text = tabName Then
                        ListTab.TabPages(i).Text = newTabText
                    End If
                    TabDialog.AddTab(ListTab.TabPages(i).Text)
                End If
            Next
            SaveConfigsCommon()
            'SaveConfigsTab(newTabText)
            SaveConfigsTabs()
            _rclickTabName = newTabText
            tabName = newTabText
            Return True
        End If
    End Function

    Private Sub Tabs_DoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListTab.MouseDoubleClick
        Dim tn As String = ListTab.SelectedTab.Text
        TabRename(tn)
    End Sub

    Private Sub Tabs_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListTab.MouseDown
        Dim cpos As New Point(e.X, e.Y)
        If e.Button = Windows.Forms.MouseButtons.Left Then
            For i As Integer = 0 To ListTab.TabPages.Count - 1
                Dim rect As Rectangle = ListTab.GetTabRect(i)
                If rect.Left <= cpos.X AndAlso cpos.X <= rect.Right AndAlso _
                   rect.Top <= cpos.Y AndAlso cpos.Y <= rect.Bottom Then
                    _tabDrag = True
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
        If tn = "" Then
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
        If _curPost Is Nothing Then Exit Sub

        ' 複数あてリプライはReplyではなく通常ポスト
        '↑仕様変更で全部リプライ扱いでＯＫ（先頭ドット付加しない）
        '090403暫定でドットを付加しないようにだけ修正。単独と複数の処理は統合できると思われる。
        '090513 all @ replies 廃止の仕様変更によりドット付加に戻し(syo68k)

        If _curList.SelectedIndices.Count > 0 Then
            ' アイテムが1件以上選択されている
            If _curList.SelectedIndices.Count = 1 AndAlso Not isAll AndAlso _curPost IsNot Nothing Then
                ' 単独ユーザー宛リプライまたはDM
                If (_statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.DirectMessage AndAlso isAuto) OrElse (Not isAuto AndAlso Not isReply) Then
                    ' ダイレクトメッセージ
                    StatusText.Text = "D " + _curPost.Name + " " + StatusText.Text
                    StatusText.SelectionStart = StatusText.Text.Length
                    StatusText.Focus()
                    _reply_to_id = 0
                    _reply_to_name = ""
                    Exit Sub
                End If
                If StatusText.Text = "" Then
                    '空の場合

                    ' ステータステキストが入力されていない場合先頭に@ユーザー名を追加する
                    StatusText.Text = "@" + _curPost.Name + " "
                    If _curPost.RetweetedId > 0 Then
                        _reply_to_id = _curPost.RetweetedId
                    Else
                        _reply_to_id = _curPost.Id
                    End If
                    _reply_to_name = _curPost.Name
                Else
                    '何か入力済の場合

                    If isAuto Then
                        '1件選んでEnter or DoubleClick
                        If StatusText.Text.Contains("@" + _curPost.Name + " ") Then
                            If _reply_to_id > 0 AndAlso _reply_to_name = _curPost.Name Then
                                '返信先書き換え
                                If _curPost.RetweetedId > 0 Then
                                    _reply_to_id = _curPost.RetweetedId
                                Else
                                    _reply_to_id = _curPost.Id
                                End If
                                _reply_to_name = _curPost.Name
                            End If
                            Exit Sub
                        End If
                        If Not StatusText.Text.StartsWith("@") Then
                            '文頭＠以外
                            If StatusText.Text.StartsWith(". ") Then
                                ' 複数リプライ
                                StatusText.Text = StatusText.Text.Insert(2, "@" + _curPost.Name + " ")
                                _reply_to_id = 0
                                _reply_to_name = ""
                            Else
                                ' 単独リプライ
                                StatusText.Text = "@" + _curPost.Name + " " + StatusText.Text
                                If _curPost.RetweetedId > 0 Then
                                    _reply_to_id = _curPost.RetweetedId
                                Else
                                    _reply_to_id = _curPost.Id
                                End If
                                _reply_to_name = _curPost.Name
                            End If
                        Else
                            '文頭＠
                            ' 複数リプライ
                            StatusText.Text = ". @" + _curPost.Name + " " + StatusText.Text
                            'StatusText.Text = "@" + _curPost.Name + " " + StatusText.Text
                            _reply_to_id = 0
                            _reply_to_name = ""
                        End If
                    Else
                        '1件選んでCtrl-Rの場合（返信先操作せず）
                        Dim sidx As Integer = StatusText.SelectionStart
                        Dim id As String = "@" + _curPost.Name + " "
                        If sidx > 0 Then
                            If StatusText.Text.Substring(sidx - 1, 1) <> " " Then
                                id = " " + id
                            End If
                        End If
                        StatusText.Text = StatusText.Text.Insert(sidx, id)
                        sidx += id.Length
                        'If StatusText.Text.StartsWith("@") Then
                        '    '複数リプライ
                        '    StatusText.Text = ". " + StatusText.Text.Insert(sidx, " @" + _curPost.Name + " ")
                        '    sidx += 5 + _curPost.Name.Length
                        'Else
                        '    ' 複数リプライ
                        '    StatusText.Text = StatusText.Text.Insert(sidx, " @" + _curPost.Name + " ")
                        '    sidx += 3 + _curPost.Name.Length
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
                        If Not sTxt.Contains("@" + post.Name + " ") Then
                            sTxt = sTxt.Insert(2, "@" + post.Name + " ")
                            'sTxt = "@" + post.Name + " " + sTxt
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
                            If Not ids.Contains("@" + post.Name + " ") AndAlso _
                               Not post.Name.Equals(Twitter.Username, StringComparison.CurrentCultureIgnoreCase) Then
                                ids += "@" + post.Name + " "
                            End If
                            If isAll Then
                                For Each nm As String In post.ReplyToList
                                    If Not ids.Contains("@" + nm + " ") AndAlso _
                                       Not nm.Equals(Twitter.Username, StringComparison.CurrentCultureIgnoreCase) Then
                                        ids += "@" + nm + " "
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
                        If Not ids.Contains("@" + post.Name + " ") AndAlso _
                           Not post.Name.Equals(Twitter.Username, StringComparison.CurrentCultureIgnoreCase) Then
                            ids += "@" + post.Name + " "
                        End If
                        For Each nm As String In post.ReplyToList
                            If Not ids.Contains("@" + nm + " ") AndAlso _
                               Not nm.Equals(Twitter.Username, StringComparison.CurrentCultureIgnoreCase) Then
                                ids += "@" + nm + " "
                            End If
                        Next
                        If ids.Length = 0 Then Exit Sub
                        If StatusText.Text = "" Then
                            '未入力の場合のみ返信先付加
                            StatusText.Text = ids
                            StatusText.SelectionStart = ids.Length
                            StatusText.Focus()
                            If post.RetweetedId > 0 Then
                                _reply_to_id = post.RetweetedId
                            Else
                                _reply_to_id = post.Id
                            End If
                            _reply_to_name = post.Name
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

    Private Sub TimerRefreshIcon_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerRefreshIcon.Tick
        If Not TimerRefreshIcon.Enabled Then Exit Sub
        Static iconCnt As Integer = 0
        Static blinkCnt As Integer = 0
        Static blink As Boolean = False
        Static idle As Boolean = False

        iconCnt += 1
        blinkCnt += 1

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

    Private Sub ContextMenuTabProperty_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuTabProperty.Opening
        '右クリックの場合はタブ名が設定済。アプリケーションキーの場合は現在のタブを対象とする
        If _rclickTabName = "" OrElse sender IsNot ContextMenuTabProperty Then _rclickTabName = ListTab.SelectedTab.Text
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
        Dim oDir As IO.DirectoryInfo = New IO.DirectoryInfo(My.Application.Info.DirectoryPath)
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

        If _statuses.Tabs(_rclickTabName).TabType <> TabUsageType.Mentions AndAlso _statuses.IsDefaultTab(_rclickTabName) Then
            FilterEditMenuItem.Enabled = True
            Me.EditRuleTbMenuItem.Enabled = True
            DeleteTabMenuItem.Enabled = False
            Me.DeleteTbMenuItem.Enabled = False
        ElseIf _statuses.Tabs(_rclickTabName).TabType = TabUsageType.Mentions Then
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

        If _rclickTabName = "" Then Exit Sub
        ChangeTabUnreadManage(_rclickTabName, UreadManageMenuItem.Checked)

        'SaveConfigsTab(_rclickTabName)
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
        SetStatusLabel()
        If Not SettingDialog.TabIconDisp Then ListTab.Refresh()
    End Sub

    Private Sub NotifyDispMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NotifyDispMenuItem.Click, NotifyTbMenuItem.Click
        NotifyDispMenuItem.Checked = DirectCast(sender, ToolStripMenuItem).Checked
        Me.NotifyTbMenuItem.Checked = NotifyDispMenuItem.Checked

        If _rclickTabName = "" Then Exit Sub

        _statuses.Tabs(_rclickTabName).Notify = NotifyDispMenuItem.Checked

        'SaveConfigsTab(_rclickTabName)
        SaveConfigsTabs()
    End Sub

    Private Sub SoundFileComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SoundFileComboBox.SelectedIndexChanged, SoundFileTbComboBox.SelectedIndexChanged
        If soundfileListup OrElse _rclickTabName = "" Then Exit Sub

        _statuses.Tabs(_rclickTabName).SoundFile = DirectCast(DirectCast(sender, ToolStripComboBox).SelectedItem, String)

        'SaveConfigsTab(_rclickTabName)
        SaveConfigsTabs()
    End Sub

    Private Sub DeleteTabMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles DeleteTabMenuItem.Click, DeleteTbMenuItem.Click
        If _rclickTabName = "" OrElse sender Is Me.DeleteTbMenuItem Then _rclickTabName = ListTab.SelectedTab.Text

        RemoveSpecifiedTab(_rclickTabName)
        '_rclickTabName = ""
        'SaveConfigsCommon()
        'SaveConfigsTab(False)
        SaveConfigsTabs()
    End Sub

    Private Sub FilterEditMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FilterEditMenuItem.Click, EditRuleTbMenuItem.Click
        'If _rclickTabName = "" OrElse _rclickTabName = DEFAULTTAB.RECENT OrElse _rclickTabName = DEFAULTTAB.DM _
        '        OrElse _rclickTabName = DEFAULTTAB.FAV Then Exit Sub

        If _rclickTabName = "" Then _rclickTabName = _statuses.GetTabByType(TabUsageType.Home).TabName
        fDialog.SetCurrent(_rclickTabName)
        fDialog.ShowDialog()
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
        'SaveConfigsTab(False)
        SaveConfigsTabs()
    End Sub

    Private Sub AddTabMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddTabMenuItem.Click, CreateTbMenuItem.Click
        Dim tabName As String = Nothing
        Dim tabUsage As TabUsageType
        Using inputName As New InputTabName()
            inputName.TabName = _statuses.GetUniqueTabName
            inputName.IsShowUsage = True
            inputName.ShowDialog()
            tabName = inputName.TabName
            tabUsage = inputName.Usage
            inputName.Dispose()
        End Using
        Me.TopMost = SettingDialog.AlwaysTop
        If tabName <> "" Then
            If Not AddNewTab(tabName, False, tabUsage) Then
                Dim tmp As String = String.Format(My.Resources.AddTabMenuItem_ClickText1, tabName)
                MessageBox.Show(tmp, My.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Else
                '成功
                _statuses.AddTab(tabName, tabUsage)
                'SaveConfigsCommon()
                'SaveConfigsTab(False)
                SaveConfigsTabs()
                If tabUsage = TabUsageType.PublicSearch Then
                    ListTab.SelectedIndex = ListTab.TabPages.Count - 1
                    ListTabSelect(ListTab.TabPages(ListTab.TabPages.Count - 1))
                    ListTab.SelectedTab.Controls("panelSearch").Controls("comboSearch").Focus()
                End If
            End If
        End If
    End Sub

    Private Sub TabMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabMenuItem.Click, CreateTabRuleOpMenuItem.Click
        '選択発言を元にフィルタ追加
        For Each idx As Integer In _curList.SelectedIndices
            Dim tabName As String = ""
            'タブ選択（or追加）
            If Not SelectTab(tabName) Then Exit For

            fDialog.SetCurrent(tabName)
            If _statuses.Item(_curTab.Text, idx).RetweetedId = 0 Then
                fDialog.AddNewFilter(_statuses.Item(_curTab.Text, idx).Name, _statuses.Item(_curTab.Text, idx).Data)
            Else
                fDialog.AddNewFilter(_statuses.Item(_curTab.Text, idx).RetweetedBy, _statuses.Item(_curTab.Text, idx).Data)
            End If
            fDialog.ShowDialog()
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
        'SaveConfigsCommon()
        'SaveConfigsTab(False)
        SaveConfigsTabs()
    End Sub

    Protected Overrides Function ProcessDialogKey( _
        ByVal keyData As Keys) As Boolean
        'TextBox1でEnterを押してもビープ音が鳴らないようにする
        If (keyData And Keys.KeyCode) = Keys.Enter Then
            If StatusText.Focused Then
                '改行
                If StatusText.Multiline AndAlso _
                   (keyData And Keys.Shift) = Keys.Shift AndAlso _
                   (keyData And Keys.Control) <> Keys.Control Then
                    Dim pos1 As Integer = StatusText.SelectionStart
                    If StatusText.SelectionLength > 0 Then
                        StatusText.Text = StatusText.Text.Remove(pos1, StatusText.SelectionLength)  '選択状態文字列削除
                    End If
                    StatusText.Text = StatusText.Text.Insert(pos1, Environment.NewLine)  '改行挿入
                    StatusText.SelectionStart = pos1 + Environment.NewLine.Length    'カーソルを改行の次の文字へ移動
                    Return True
                End If
                '投稿
                If (Not StatusText.Multiline AndAlso _
                        ((keyData And Keys.Control) = Keys.Control AndAlso SettingDialog.PostCtrlEnter) OrElse _
                        ((keyData And Keys.Control) <> Keys.Control AndAlso Not SettingDialog.PostCtrlEnter)) OrElse _
                   (StatusText.Multiline AndAlso _
                        (Not SettingDialog.PostCtrlEnter AndAlso _
                            ((keyData And Keys.Control) <> Keys.Control AndAlso (keyData And Keys.Shift) <> Keys.Shift) OrElse _
                            ((keyData And Keys.Control) = Keys.Control AndAlso (keyData And Keys.Shift) = Keys.Shift)) OrElse _
                        (SettingDialog.PostCtrlEnter AndAlso (keyData And Keys.Control) = Keys.Control)) Then
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

    Private Sub InfoTwitterMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InfoTwitterMenuItem.Click
        If Twitter.InfoTwitter.Trim() = "" Then
            MessageBox.Show(My.Resources.InfoTwitterMenuItem_ClickText1, My.Resources.InfoTwitterMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Dim inf As String = Twitter.InfoTwitter.Trim()
            inf = "<html><head></head><body>" + inf + "</body></html>"
            PostBrowser.Visible = False
            PostBrowser.DocumentText = inf
            PostBrowser.Visible = True
        End If
    End Sub

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
            If Not ids.Contains(post.Name) Then
                Dim fc As New FiltersClass
                ids.Add(post.Name)
                If post.RetweetedId = 0 Then
                    fc.NameFilter = post.Name
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
        'SaveConfigsCommon()
        'SaveConfigsTab(False)
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
                    tabName = inputName.TabName
                    inputName.Dispose()
                End Using
                Me.TopMost = SettingDialog.AlwaysTop
                If tabName <> "" Then
                    If Not AddNewTab(tabName, False, TabUsageType.UserDefined) Then
                        Dim tmp As String = String.Format(My.Resources.IDRuleMenuItem_ClickText2, tabName)
                        MessageBox.Show(tmp, My.Resources.IDRuleMenuItem_ClickText3, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        'もう一度タブ名入力
                    Else
                        _statuses.AddTab(tabName, TabUsageType.UserDefined)
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

    'Private Sub WedataMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WedataMenuItem.Click
    '    Twitter.GetWedata()
    'End Sub

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
                    Try
                        urlStr = IDNDecode(linkElm.GetAttribute("href"))
                    Catch ex As ArgumentException
                        '変なHTML？
                        Exit Sub
                    End Try
                    If String.IsNullOrEmpty(urlStr) Then Continue For
                    UrlDialog.AddUrl(urlEncodeMultibyteChar(urlStr))
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
                AddNewTabForSearch(hash)
                Exit Sub
            End If

            openUrlStr = openUrlStr.Replace("://twitter.com/search?q=#", "://twitter.com/search?q=%23")
            OpenUriAsync(openUrlStr)
        End If
    End Sub

    Private Sub ClearTabMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearTabMenuItem.Click, ClearTbMenuItem.Click
        If _rclickTabName = "" Then Exit Sub
        Dim tmp As String = String.Format(My.Resources.ClearTabMenuItem_ClickText1, Environment.NewLine)
        If MessageBox.Show(tmp, My.Resources.ClearTabMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If

        _statuses.ClearTabIds(_rclickTabName)
        If ListTab.SelectedTab.Text = _rclickTabName Then
            _anchorPost = Nothing
            _anchorFlag = False
            _itemCache = Nothing
            _postCache = Nothing
            _itemCacheIndex = -1
            _curItemIndex = -1
            _curPost = Nothing
        End If
        For Each tb As TabPage In ListTab.TabPages
            If tb.Text = _rclickTabName Then
                tb.ImageIndex = -1
                DirectCast(tb.Tag, DetailsListView).VirtualListSize = 0
                Exit For
            End If
        Next
        If Not SettingDialog.TabIconDisp Then ListTab.Refresh()

        SetMainWindowTitle()
        SetStatusLabel()
    End Sub

    Private Sub SetMainWindowTitle()
        'メインウインドウタイトルの書き換え
        Dim ttl As New StringBuilder(256)
        Dim ur As Integer = 0
        Dim al As Integer = 0
        Static myVer As String = fileVersion
        If SettingDialog.DispLatestPost <> DispTitleEnum.None AndAlso _
           SettingDialog.DispLatestPost <> DispTitleEnum.Post AndAlso _
           SettingDialog.DispLatestPost <> DispTitleEnum.Ver Then
            For Each key As String In _statuses.Tabs.Keys
                ur += _statuses.Tabs(key).UnreadCount
                al += _statuses.Tabs(key).AllCount
            Next
        End If

        If SettingDialog.DispUsername Then ttl.Append(Twitter.Username).Append(" - ")
        ttl.Append("Tween  ")
        Select Case SettingDialog.DispLatestPost
            Case DispTitleEnum.Ver
                ttl.Append("Ver:").Append(myVer)
            Case DispTitleEnum.Post
                If _history IsNot Nothing AndAlso _history.Count > 1 Then
                    ttl.Append(_history(_history.Count - 2).Replace(vbCrLf, ""))
                End If
            Case DispTitleEnum.UnreadRepCount
                ttl.AppendFormat(My.Resources.SetMainWindowTitleText1, _statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + _statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount)
            Case DispTitleEnum.UnreadAllCount
                ttl.AppendFormat(My.Resources.SetMainWindowTitleText2, ur)
            Case DispTitleEnum.UnreadAllRepCount
                ttl.AppendFormat(My.Resources.SetMainWindowTitleText3, ur, _statuses.GetTabByType(TabUsageType.Mentions).UnreadCount + _statuses.GetTabByType(TabUsageType.DirectMessage).UnreadCount)
            Case DispTitleEnum.UnreadCountAllCount
                ttl.AppendFormat(My.Resources.SetMainWindowTitleText4, ur, al)
        End Select

        Try
            Me.Text = ttl.ToString()
        Catch ex As AccessViolationException
            '原因不明。ポスト内容に依存か？たまーに発生するが再現せず。
        End Try
    End Sub

    Private Sub SetStatusLabel()
        'ステータス欄にカウント表示
        'タブ未読数/タブ発言数 全未読数/総発言数 (未読＠＋未読DM数)
        If _statuses Is Nothing Then Exit Sub
        Dim tbRep As TabClass = _statuses.GetTabByType(TabUsageType.Mentions)
        Dim tbDm As TabClass = _statuses.GetTabByType(TabUsageType.DirectMessage)
        If tbRep Is Nothing OrElse tbDm Is Nothing Then Exit Sub
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
            Exit Sub
        End Try

        If Twitter.RemainCountApi > -1 Then
            slbl.Append("[API: " + Twitter.RemainCountApi.ToString + "] ")
        End If
        slbl.AppendFormat(My.Resources.SetStatusLabelText1, tur, tal, ur, al, urat, _postTimestamps.Count, _favTimestamps.Count, _tlCount)
        If SettingDialog.TimelinePeriodInt = 0 Then
            slbl.Append(My.Resources.SetStatusLabelText2)
        Else
            slbl.Append((SettingDialog.TimelinePeriodInt - _homeCounterAdjuster).ToString() + My.Resources.SetStatusLabelText3)
        End If

        StatusLabelUrl.Text = slbl.ToString()
    End Sub

    Private Sub SetNotifyIconText()
        ' タスクトレイアイコンのツールチップテキスト書き換え
        If SettingDialog.DispUsername Then
            NotifyIcon1.Text = Twitter.Username + " - Tween"
        Else
            NotifyIcon1.Text = "Tween"
        End If
    End Sub

    Friend Sub CheckReplyTo(ByVal StatusText As String)
        Dim m As MatchCollection
        'ハッシュタグの保存
        Dim hash As New Regex("(^|[^a-zA-Z0-9_/])[#|＃](?<hash>[a-zA-Z0-9_]+)")
        m = hash.Matches(StatusText)
        Dim hstr As String = ""
        For Each hm As Match In m
            If Not IsNumeric(hm.Result("${hash}")) Then
                If Not hstr.Contains("#" + hm.Result("${hash}") + " ") Then
                    hstr += "#" + hm.Result("${hash}") + " "
                    HashSupl.AddItem("#" + hm.Result("${hash}"))
                End If
            End If
        Next
        If HashMgr.UseHash <> "" AndAlso Not hstr.Contains(HashMgr.UseHash + " ") Then
            hstr += HashMgr.UseHash
        End If
        If hstr <> "" Then HashMgr.AddHashToHistory(hstr.Trim, False)

        ' 本当にリプライ先指定すべきかどうかの判定
        Dim id As New Regex("(^|[ -/:-@[-^`{-~])(?<id>@[a-zA-Z0-9_]+)")

        m = id.Matches(StatusText)

        If SettingDialog.UseAtIdSupplement Then
            Dim bCnt As Integer = AtIdSupl.ItemCount
            For Each mid As Match In m
                AtIdSupl.AddItem(mid.Result("${id}"))
            Next
            If bCnt <> AtIdSupl.ItemCount Then modifySettingAtId = True
        End If

        ' リプライ先ステータスIDの指定がない場合は指定しない
        If _reply_to_id = 0 Then Exit Sub

        ' リプライ先ユーザー名がない場合も指定しない
        If _reply_to_name = "" Then
            _reply_to_id = 0
            Exit Sub
        End If

        ' 通常Reply
        ' 次の条件を満たす場合に in_reply_to_status_id 指定
        ' 1. Twitterによりリンクと判定される @idが文中に1つ含まれる (2009/5/28 リンク化される@IDのみカウントするように修正)
        ' 2. リプライ先ステータスIDが設定されている(リストをダブルクリックで返信している)
        ' 3. 文中に含まれた@idがリプライ先のポスト者のIDと一致する

        If m IsNot Nothing AndAlso Not StatusText.StartsWith(". ") Then
            For Each mid As Match In m
                If mid.Result("${id}") = "@" + _reply_to_name Then
                    Exit Sub
                End If
            Next
        End If
        'If m IsNot Nothing AndAlso m.Count = 1 AndAlso m.Item(0).Value = "@" + _reply_to_name AndAlso Not StatusText.StartsWith(". ") Then
        '    Exit Sub
        'End If

        _reply_to_id = 0
        _reply_to_name = ""

    End Sub

    Private Sub TweenMain_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        If SettingDialog.MinimizeToTray AndAlso WindowState = FormWindowState.Minimized Then
            Me.Visible = False
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
        modifySettingCommon = True
        'SaveConfigsCommon()
    End Sub

    Private Sub SplitContainer1_SplitterMoved(ByVal sender As Object, ByVal e As System.Windows.Forms.SplitterEventArgs) Handles SplitContainer1.SplitterMoved
        If Me.WindowState = FormWindowState.Normal Then
            _mySpDis = SplitContainer1.SplitterDistance
            If StatusText.Multiline Then _mySpDis2 = StatusText.Height
            modifySettingLocal = True
        End If
    End Sub

    Private Sub RepliedStatusOpenMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RepliedStatusOpenMenuItem.Click, OpenRepSourceOpMenuItem.Click
        If _curPost IsNot Nothing AndAlso _curPost.InReplyToUser IsNot Nothing AndAlso _curPost.InReplyToId > 0 Then
            If _statuses.ContainsKey(_curPost.InReplyToId) AndAlso Not My.Computer.Keyboard.ShiftKeyDown Then
                Dim repPost As PostClass = _statuses.Item(_curPost.InReplyToId)
                MessageBox.Show(repPost.Name + " / " + repPost.Nickname + "   (" + repPost.PDate.ToString() + ")" + Environment.NewLine + repPost.Data)
            Else
                OpenUriAsync("http://twitter.com/" + _curPost.InReplyToUser + "/status/" + _curPost.InReplyToId.ToString())
            End If
        End If
    End Sub

    Private Sub ContextMenuStrip3_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip3.Opening
        '発言詳細のアイコン右クリック時のメニュー制御
        If _curList.SelectedIndices.Count > 0 AndAlso _curPost IsNot Nothing Then
            Dim name As String = _curPost.ImageUrl
            If name.Length > 0 Then
                name = IO.Path.GetFileNameWithoutExtension(name.Substring(name.LastIndexOf("/"c)))
                name = name.Substring(0, name.Length - 7) ' "_normal".Length
                Me.IconNameToolStripMenuItem.Enabled = True
                If Me.TIconDic.ContainsKey(_curPost.ImageUrl) AndAlso Me.TIconDic(_curPost.ImageUrl) IsNot Nothing Then
                    Me.SaveIconPictureToolStripMenuItem.Enabled = True
                Else
                    Me.SaveIconPictureToolStripMenuItem.Enabled = False
                End If
                Me.IconNameToolStripMenuItem.Text = name
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
        modifySettingLocal = True
    End Sub

    Private Sub StatusText_MultilineChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusText.MultilineChanged
        If Me.StatusText.Multiline Then
            Me.StatusText.ScrollBars = ScrollBars.Vertical
        Else
            Me.StatusText.ScrollBars = ScrollBars.None
        End If
        modifySettingLocal = True
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
        modifySettingLocal = True
    End Sub

    Private Function UrlConvert(ByVal Converter_Type As UrlConverter) As Boolean
        'Converter_Type=Nicomsの場合は、nicovideoのみ短縮する
        Dim result As String = ""
        Dim url As Regex = New Regex("(?<before>(?:[^\/""':!=]|^|\:))" + _
                                    "(?<url>(?<protocol>https?://|www\.)" + _
                                    "(?<domain>(?:[\.-]|[^\p{P}])+\.[a-z]{2,}(?::[0-9]+)?)" + _
                                    "(?<path>/[a-z0-9!*'();:&=+$/%#\[\]\-_.,~]*[a-z0-9)=#/]?)?" + _
                                    "(?<query>\?[a-z0-9!*'();:&=+$/%#\[\]\-_.,~]*[a-z0-9_&=#])?)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
        'Dim url As Regex = New Regex("(?<![0-9A-Za-z])(?:https?|shttp)://(?:(?:[-_.!~*'()a-zA-Z0-9;:&=+$,]|%[0-9A-Fa-f" + _
        '                             "][0-9A-Fa-f])*@)?(?:(?:[a-zA-Z0-9](?:[-a-zA-Z0-9]*[a-zA-Z0-9])?\.)" + _
        '                             "*[a-zA-Z](?:[-a-zA-Z0-9]*[a-zA-Z0-9])?\.?|[0-9]+\.[0-9]+\.[0-9]+\." + _
        '                             "[0-9]+)(?::[0-9]*)?(?:/(?:[-_.!~*'()a-zA-Z0-9:@&=+$,]|%[0-9A-Fa-f]" + _
        '                             "[0-9A-Fa-f])*(?:;(?:[-_.!~*'()a-zA-Z0-9:@&=+$,]|%[0-9A-Fa-f][0-9A-" + _
        '                             "Fa-f])*)*(?:/(?:[-_.!~*'()a-zA-Z0-9:@&=+$,]|%[0-9A-Fa-f][0-9A-Fa-f" + _
        '                             "])*(?:;(?:[-_.!~*'()a-zA-Z0-9:@&=+$,]|%[0-9A-Fa-f][0-9A-Fa-f])*)*)" + _
        '                             "*)?(?:\?(?:[-_.!~*'()a-zA-Z0-9;/?:@&=+$,]|%[0-9A-Fa-f][0-9A-Fa-f])" + _
        '                             "*)?(?:#(?:[-_.!~*'()a-zA-Z0-9;/?:@&=+$,]|%[0-9A-Fa-f][0-9A-Fa-f])*)?")
        Dim nico As Regex = New Regex("^https?://[a-z]+\.(nicovideo|niconicommons)\.jp/[a-z]+/[a-z0-9]+$")

        If StatusText.SelectionLength > 0 Then
            Dim tmp As String = StatusText.SelectedText
            ' httpから始まらない場合、ExcludeStringで指定された文字列で始まる場合は対象としない
            If tmp.StartsWith("http") Then
                ' 文字列が選択されている場合はその文字列について処理

                'nico.ms使用、nicovideoにマッチしたら変換
                If SettingDialog.Nicoms AndAlso nico.IsMatch(tmp) Then
                    result = Twitter.MakeShortNicoms(tmp)
                    If result.Equals("Can't convert") Then
                        StatusLabel.Text = result.Insert(0, "nico.ms:")
                        Return False
                    End If
                ElseIf Converter_Type <> UrlConverter.Nicoms Then
                    '短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                    result = Twitter.MakeShortUrl(Converter_Type, tmp)
                    If result.Equals("Can't convert") Then
                        StatusLabel.Text = result.Insert(0, Converter_Type.ToString() + ":")
                        Return False
                    End If
                Else
                    Return True
                End If

                If Not result = "" Then
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
            For Each mt As Match In url.Matches(StatusText.Text)
                Dim tmp As String = mt.Result("${url}")
                If tmp.StartsWith("w", StringComparison.OrdinalIgnoreCase) Then tmp = "http://" + tmp
                Dim undotmp As New urlUndo

                '選んだURLを選択（？）
                StatusText.Select(StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length)

                'nico.ms使用、nicovideoにマッチしたら変換
                If SettingDialog.Nicoms AndAlso nico.IsMatch(tmp) Then
                    result = Twitter.MakeShortNicoms(tmp)
                    If result.Equals("Can't convert") Then
                        StatusLabel.Text = result.Insert(0, "nico.ms:")
                        Continue For
                    End If
                ElseIf Converter_Type <> UrlConverter.Nicoms Then
                    '短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                    result = Twitter.MakeShortUrl(Converter_Type, tmp)
                    If result.Equals("Can't convert") Then
                        StatusLabel.Text = result.Insert(0, Converter_Type.ToString() + ":")
                        Continue For
                    End If
                Else
                    Continue For
                End If

                If Not result = "" Then
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

    Private Sub UrlConvertAutoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UrlConvertAutoToolStripMenuItem.Click
        If Not UrlConvert(SettingDialog.AutoShortUrlFirst) Then
            Dim svc As UrlConverter = SettingDialog.AutoShortUrlFirst
            Dim rnd As New Random()
            ' 前回使用した短縮URLサービス以外を選択する
            Do
                svc = CType(rnd.Next(System.Enum.GetNames(GetType(UrlConverter)).Length), UrlConverter)
            Loop Until svc <> SettingDialog.AutoShortUrlFirst
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
        modifySettingCommon = True
    End Sub

    Private Sub ListLockMenuItem_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListLockMenuItem.CheckStateChanged, LockListFileMenuItem.CheckStateChanged
        ListLockMenuItem.Checked = DirectCast(sender, ToolStripMenuItem).Checked
        Me.LockListFileMenuItem.Checked = ListLockMenuItem.Checked
        _cfgCommon.ListLock = ListLockMenuItem.Checked
        modifySettingCommon = True
    End Sub

    Private Sub MenuStrip1_MenuActivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuStrip1.MenuActivate
        ' フォーカスがメニューに移る (MenuStrip1.Tag フラグを立てる)
        MenuStrip1.Tag = New Object()
        MenuStrip1.Select() ' StatusText がフォーカスを持っている場合 Leave が発生
    End Sub

    Private Sub MenuStrip1_MenuDeactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuStrip1.MenuDeactivate
        If Me.Tag IsNot Nothing Then ' 設定された戻り先へ遷移
            DirectCast(Me.Tag, Control).Select()
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
        modifySettingLocal = True
        _isColumnChanged = True
    End Sub

    Private Sub MyList_ColumnWidthChanged(ByVal sender As System.Object, ByVal e As ColumnWidthChangedEventArgs)
        Dim lst As DetailsListView = DirectCast(sender, DetailsListView)
        'Dim changed As Boolean = False
        If _cfgLocal Is Nothing Then Exit Sub
        If _iconCol Then
            If _cfgLocal.Width1 <> lst.Columns(0).Width Then
                _cfgLocal.Width1 = lst.Columns(0).Width
                modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width3 <> lst.Columns(1).Width Then
                _cfgLocal.Width3 = lst.Columns(1).Width
                modifySettingLocal = True
                _isColumnChanged = True
            End If
        Else
            If _cfgLocal.Width1 <> lst.Columns(0).Width Then
                _cfgLocal.Width1 = lst.Columns(0).Width
                modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width2 <> lst.Columns(1).Width Then
                _cfgLocal.Width2 = lst.Columns(1).Width
                modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width3 <> lst.Columns(2).Width Then
                _cfgLocal.Width3 = lst.Columns(2).Width
                modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width4 <> lst.Columns(3).Width Then
                _cfgLocal.Width4 = lst.Columns(3).Width
                modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width5 <> lst.Columns(4).Width Then
                _cfgLocal.Width5 = lst.Columns(4).Width
                modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width6 <> lst.Columns(5).Width Then
                _cfgLocal.Width6 = lst.Columns(5).Width
                modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width7 <> lst.Columns(6).Width Then
                _cfgLocal.Width7 = lst.Columns(6).Width
                modifySettingLocal = True
                _isColumnChanged = True
            End If
            If _cfgLocal.Width8 <> lst.Columns(7).Width Then
                _cfgLocal.Width8 = lst.Columns(7).Width
                modifySettingLocal = True
                _isColumnChanged = True
            End If
        End If
        ' 非表示の時にColumnChangedが呼ばれた場合はForm初期化処理中なので保存しない
        'If changed Then
        '    SaveConfigsLocal()
        'End If
    End Sub

    Private Sub ToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem3.Click
        '発言詳細で「選択文字列をコピー」
        'PostBrowser.Document.ExecCommand("Copy", False, Nothing)
        'SendKeys.Send("^c")
        Dim typ As Type = PostBrowser.ActiveXInstance.GetType()
        Dim _SelObj As Object = typ.InvokeMember("selection", BindingFlags.GetProperty, Nothing, PostBrowser.Document.DomDocument, Nothing)
        Dim _objRange As Object = _SelObj.GetType().InvokeMember("createRange", BindingFlags.InvokeMethod, Nothing, _SelObj, Nothing)
        Dim _selText As String = DirectCast(_objRange.GetType().InvokeMember("text", BindingFlags.GetProperty, Nothing, _objRange, Nothing), String)
        Try
            Clipboard.SetDataObject(_selText, False, 5, 100)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub doSearchToolStrip(ByVal url As String)
        '発言詳細で「選択文字列で検索」（選択文字列取得）
        Dim typ As Type = PostBrowser.ActiveXInstance.GetType()
        Dim _SelObj As Object = typ.InvokeMember("selection", BindingFlags.GetProperty, Nothing, PostBrowser.Document.DomDocument, Nothing)
        Dim _objRange As Object = _SelObj.GetType().InvokeMember("createRange", BindingFlags.InvokeMethod, Nothing, _SelObj, Nothing)
        Dim _selText As String = DirectCast(_objRange.GetType().InvokeMember("text", BindingFlags.GetProperty, Nothing, _objRange, Nothing), String)

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

    Private Sub ToolStripMenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem5.Click
        '発言詳細ですべて選択
        PostBrowser.Document.ExecCommand("SelectAll", False, Nothing)
    End Sub

    Private Sub SearchItem1ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchItem1ToolStripMenuItem.Click
        doSearchToolStrip(My.Resources.SearchItem1Url)
    End Sub

    Private Sub SearchItem2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchItem2ToolStripMenuItem.Click
        doSearchToolStrip(My.Resources.SearchItem2Url)
    End Sub

    Private Sub SearchItem3ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchItem3ToolStripMenuItem.Click
        doSearchToolStrip(My.Resources.SearchItem3Url)
    End Sub

    Private Sub SearchItem4ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchItem4ToolStripMenuItem.Click
        doSearchToolStrip(My.Resources.SearchItem4Url)
    End Sub

    Private Sub ToolStripMenuItem4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem4.Click
        'URLをコピー
        'If PostBrowser.StatusText.StartsWith("http") Then   '念のため
        Try
            Clipboard.SetDataObject(PostBrowser.StatusText, False, 5, 100)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        'End If
    End Sub

    Private Sub ContextMenuStrip4_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip4.Opening
        ' URLコピーの項目の表示/非表示
        If PostBrowser.StatusText.StartsWith("http") Then
            ToolStripMenuItem4.Enabled = True
            If Regex.IsMatch(PostBrowser.StatusText, "^https?://twitter.com/[a-zA-Z0-9_]+$") Then
                FollowContextMenuItem.Enabled = True
                RemoveContextMenuItem.Enabled = True
                FriendshipContextMenuItem.Enabled = True
            Else
                FollowContextMenuItem.Enabled = False
                RemoveContextMenuItem.Enabled = False
                FriendshipContextMenuItem.Enabled = False
            End If
            If Regex.IsMatch(PostBrowser.StatusText, "^https?://twitter.com/search\?q=%23") Then
                UseHashtagMenuItem.Enabled = True
            Else
                UseHashtagMenuItem.Enabled = False
            End If
        Else
            ToolStripMenuItem4.Enabled = False
            FollowContextMenuItem.Enabled = False
            RemoveContextMenuItem.Enabled = False
            FriendshipContextMenuItem.Enabled = False
            UseHashtagMenuItem.Enabled = False
        End If
        ' 文字列選択されていないときは選択文字列関係の項目を非表示に
        Dim _selText As String = PostBrowser_GetSelectionText()
        If _selText Is Nothing Then
            ToolStripMenuItem2.Enabled = False
            ToolStripMenuItem3.Enabled = False
        Else
            ToolStripMenuItem2.Enabled = True
            ToolStripMenuItem3.Enabled = True
        End If
        e.Cancel = False
    End Sub

    Private Function PostBrowser_GetSelectionText() As String
        Dim typ As Type = PostBrowser.ActiveXInstance.GetType()
        Dim _SelObj As Object = typ.InvokeMember("selection", BindingFlags.GetProperty, Nothing, PostBrowser.Document.DomDocument, Nothing)
        Dim _objRange As Object = _SelObj.GetType().InvokeMember("createRange", BindingFlags.InvokeMethod, Nothing, _SelObj, Nothing)
        Return DirectCast(_objRange.GetType().InvokeMember("text", BindingFlags.GetProperty, Nothing, _objRange, Nothing), String)
    End Function

    Private Sub CurrentTabToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CurrentTabToolStripMenuItem.Click
        '発言詳細の選択文字列で現在のタブを検索
        Dim _selText As String = PostBrowser_GetSelectionText()

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
        modifySettingLocal = True
    End Sub

    Private Sub TweenMain_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragDrop
        Dim data As String = TryCast(e.Data.GetData(DataFormats.StringFormat, True), String)
        If data IsNot Nothing Then
            StatusText.Text += data
        End If
    End Sub

    Private Sub TweenMain_DragOver(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragOver
        Dim data As String = TryCast(e.Data.GetData(DataFormats.StringFormat, True), String)
        If data IsNot Nothing Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Function IsNetworkAvailable() As Boolean
        Dim nw As Boolean = True
        Try
            nw = My.Computer.Network.IsAvailable
        Catch ex As Exception
            nw = True
        End Try
        _myStatusOnline = nw
        Return nw
    End Function

    Private Sub OpenUriAsync(ByVal UriString As String)
        Dim args As New GetWorkerArg
        args.type = WORKERTYPE.OpenUri
        args.status = UriString

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

    Private Sub TweenMain_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        'AddHandler My.Computer.Network.NetworkAvailabilityChanged, AddressOf Network_NetworkAvailabilityChanged
        Try
            PostBrowser.Url = New Uri("about:blank")
            PostBrowser.DocumentText = ""       '発言詳細部初期化
        Catch ex As Exception

        End Try

        If IsNetworkAvailable() Then
            If SettingDialog.StartupFollowers Then
                '_waitFollower = True
                GetTimeline(WORKERTYPE.Follower, 0, 0, "")
            End If
            _waitTimeline = True
            GetTimeline(WORKERTYPE.Timeline, 1, 1, "")
            'If SettingDialog.ReadPages > 0 Then
            '    _waitTimeline = True
            '    GetTimeline(WORKERTYPE.Timeline, 1, SettingDialog.ReadPages, "")
            'End If
            _waitReply = True
            GetTimeline(WORKERTYPE.Reply, 1, 1, "")
            'If SettingDialog.ReadPagesReply > 0 Then
            '    _waitReply = True
            '    GetTimeline(WORKERTYPE.Reply, 1, SettingDialog.ReadPagesReply, "")
            'End If
            _waitDm = True
            GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, 1, "")
            'If SettingDialog.ReadPagesDM > 0 Then
            '    _waitDm = True
            '    GetTimeline(WORKERTYPE.DirectMessegeRcv, 1, SettingDialog.ReadPagesDM, "")
            'End If
            _waitFav = True
            GetTimeline(WORKERTYPE.Favorites, 1, 1, "")
            'If SettingDialog.GetFav Then
            '    _waitFav = True
            '    GetTimeline(WORKERTYPE.Favorites, 1, 1, "")
            'End If
            _waitPubSearch = True
            GetTimeline(WORKERTYPE.PublicSearch, 1, 0, "")  'tabname="":全タブ
            Dim i As Integer = 0
            Do While (_waitTimeline OrElse _waitReply OrElse _waitDm OrElse _waitFav OrElse _waitPubSearch) AndAlso Not _endingFlag
                System.Threading.Thread.Sleep(100)
                My.Application.DoEvents()
                i += 1
                If i > 50 Then
                    If Not _endingFlag Then
                        _statuses.DistributePosts()
                        RefreshTimeline()
                    Else
                        Exit Sub
                    End If
                    i = 0
                End If
            Loop

            If _endingFlag Then Exit Sub

            _statuses.DistributePosts()
            RefreshTimeline()

            'バージョンチェック（引数：起動時チェックの場合はTrue･･･チェック結果のメッセージを表示しない）
            If SettingDialog.StartupVersion Then
                CheckNewVersion(True)
            End If

            '' Webモードで起動した場合に警告する
            'If Not SettingDialog.StartupAPImodeNoWarning AndAlso Not SettingDialog.UseAPI Then
            '    If MessageBox.Show(My.Resources.WebModeWarning1 + Environment.NewLine + My.Resources.WebModeWarning2 + Environment.NewLine + My.Resources.WebModeWarning3 + Environment.NewLine + My.Resources.WebModeWarning4, My.Resources.WebModeWarning5, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = Windows.Forms.DialogResult.OK Then
            '        SettingDialog.UseAPI = True
            '        'SaveConfigsCommon()
            '        MessageBox.Show(My.Resources.WebModeWarning6)
            '    Else
            '        MessageBox.Show(My.Resources.WebModeWarning7)
            '        'MessageBox.Show("取得間隔に注意してください。タイムライン取得系APIはRecent,Reply,DMの合計で1時間に" + GetMaxCountApi.ToString() + "回までしか使えません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    End If
            'End If

        Else
            PostButton.Enabled = False
            FavAddToolStripMenuItem.Enabled = False
            FavRemoveToolStripMenuItem.Enabled = False
            MoveToHomeToolStripMenuItem.Enabled = False
            MoveToFavToolStripMenuItem.Enabled = False
            DeleteStripMenuItem.Enabled = False
            RefreshStripMenuItem.Enabled = False
        End If
        _initial = False
        TimerTimeline.Enabled = True
    End Sub

    Private Sub doGetFollowersMenu(ByVal CacheInvalidate As Boolean)
        GetTimeline(WORKERTYPE.Follower, 1, 0, "")
        'Try
        '    StatusLabel.Text = My.Resources.UpdateFollowersMenuItem1_ClickText1
        '    My.Application.DoEvents()
        '    Me.Cursor = Cursors.WaitCursor
        '    Dim ret As String
        '    If SettingDialog.UseAPI Then
        '        ret = Twitter.GetFollowersApi()
        '    Else
        '        ret = Twitter.GetFollowers(CacheInvalidate)
        '    End If
        '    If ret <> "" Then
        '        StatusLabel.Text = My.Resources.UpdateFollowersMenuItem1_ClickText2 & ret
        '        Exit Sub
        '    End If
        '    StatusLabel.Text = My.Resources.UpdateFollowersMenuItem1_ClickText3
        'Finally
        '    Me.Cursor = Cursors.Default
        'End Try
    End Sub

    Private Sub GetFollowersDiffToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetFollowersDiffToolStripMenuItem.Click
        doGetFollowersMenu(False)       ' Followersリストキャッシュ有効
    End Sub

    Private Sub GetFollowersAllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetFollowersAllToolStripMenuItem.Click
        doGetFollowersMenu(True)        ' Followersリストキャッシュ無効
    End Sub

    Private Sub ReTweetStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReTweetStripMenuItem.Click, RtUnOpMenuItem.Click
        'RT @id:内容
        If _curPost IsNot Nothing Then
            If _curPost.IsDm OrElse _
               Not StatusText.Enabled Then Exit Sub

            If SettingDialog.ProtectNotInclude AndAlso _curPost.IsProtect Then
                MessageBox.Show("Protected.")
                Exit Sub
            End If
            Dim rtdata As String = _curPost.OriginalData
            rtdata = CreateRetweet(rtdata)

            StatusText.Text = "RT @" + _curPost.Name + ": " + HttpUtility.HtmlDecode(rtdata)

            StatusText.SelectionStart = 0
            StatusText.Focus()
        End If
    End Sub

    Private Sub ReTweetOriginalStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReTweetOriginalStripMenuItem.Click, RtOpMenuItem.Click
        '公式RT
        If _curPost IsNot Nothing AndAlso Not _curPost.IsDm Then
            If SettingDialog.ProtectNotInclude AndAlso _curPost.IsProtect Then
                MessageBox.Show("Protected.")
                Exit Sub
            End If
            If MessageBox.Show(My.Resources.RetweetQuestion1, "Retweet", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Exit Sub
            End If
            Dim args As New GetWorkerArg
            args.ids = New List(Of Long)
            args.sIds = New List(Of Long)
            args.tName = _curTab.Text
            args.type = WORKERTYPE.Retweet
            args.ids.Add(_curPost.Id)

            RunAsync(args)
        End If

        ''RT @id:内容
        ''元発言のみRT
        'If _curPost IsNot Nothing Then
        '    If _curPost.IsDm OrElse _
        '       Not StatusText.Enabled Then Exit Sub

        '    If SettingDialog.ProtectNotInclude AndAlso _curPost.IsProtect Then
        '        MessageBox.Show("Protected.")
        '        Exit Sub
        '    End If

        '    Dim rtdata As String = _curPost.OriginalData
        '    rtdata = CreateRetweet(rtdata)

        '    Dim rx As New Regex("^(?<multi>(RT @[0-9a-zA-Z_]+\s?:\s?)*)(?<org>RT @[0-9a-zA-Z_]+\s?:)")
        '    If rx.IsMatch(rtdata) Then
        '        StatusText.Text = HttpUtility.HtmlDecode(rx.Replace(rtdata, "${org}"))
        '    Else
        '        StatusText.Text = "RT @" + _curPost.Name + ": " + HttpUtility.HtmlDecode(rtdata)
        '    End If

        '    StatusText.SelectionStart = 0
        '    StatusText.Focus()
        'End If
    End Sub

    Private Function CreateRetweet(ByVal status As String) As String

        ' Twitterにより省略されているURLを含むaタグをキャプチャしてリンク先URLへ置き換える
        '展開しないように変更
        '展開するか判定
        Dim isUrl As Boolean = False
        Dim rx As Regex = New Regex("<a target=""_self"" href=""(?<url>[^""]+)""[^>]*>(?<link>(https?|shttp|ftps?)://[^<]+)</a>")
        Dim ms As MatchCollection = rx.Matches(status)
        For Each m As Match In ms
            If m.Result("${link}").EndsWith("...") Then
                isUrl = True
                Exit For
            End If
        Next
        If isUrl Then
            status = rx.Replace(status, "${url}")
        Else
            status = rx.Replace(status, "${link}")
        End If

        'その他のリンク(@IDなど)を置き換える
        rx = New Regex("@<a target=""_self"" href=""https?://twitter.com/(?<url>[^""]+)""[^>]*>(?<link>[^<]+)</a>")
        status = rx.Replace(status, "@${url}")
        'ハッシュタグ
        rx = New Regex("<a target=""_self"" href=""(?<url>[^""]+)""[^>]*>(?<link>[^<]+)</a>")
        status = rx.Replace(status, "${link}")
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
            DispSelectedPost()
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
        'SaveConfigsCommon()
    End Sub

    Private Sub ContextMenuStripPostMode_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStripPostMode.Opening
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
        If _rclickTabName = "" Then Exit Sub
        TabRename(_rclickTabName)
    End Sub

    Private Sub UnuToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UnuToolStripMenuItem.Click
        UrlConvert(UrlConverter.Unu)
    End Sub

    Private Sub BitlyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BitlyToolStripMenuItem.Click
        UrlConvert(UrlConverter.Bitly)
    End Sub

    Private Sub JmpToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles JmpStripMenuItem.Click
        UrlConvert(UrlConverter.Jmp)
    End Sub

    Private Sub ApiInfoMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ApiInfoMenuItem.Click
        Dim info As New ApiInfo
        Dim tmp As String

        If GetInfoApi(info) Then
            tmp = My.Resources.ApiInfo1 + info.MaxCount.ToString() + Environment.NewLine + _
                My.Resources.ApiInfo2 + info.RemainCount.ToString + Environment.NewLine + _
                My.Resources.ApiInfo3 + info.ResetTime.ToString()
        Else
            tmp = My.Resources.ApiInfo5
        End If
        MessageBox.Show(tmp, My.Resources.ApiInfo4, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub FollowCommandMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FollowCommandMenuItem.Click
        Dim id As String = ""
        If _curPost IsNot Nothing Then id = _curPost.Name
        FollowCommand(id)
    End Sub

    Private Sub FollowCommand(ByVal id As String)
        Using inputName As New InputTabName()
            inputName.FormTitle = "Follow"
            inputName.FormDescription = My.Resources.FRMessage1
            inputName.TabName = id
            If inputName.ShowDialog() = Windows.Forms.DialogResult.OK AndAlso _
               Not String.IsNullOrEmpty(inputName.TabName.Trim()) Then
                Dim ret As String = Twitter.PostFollowCommand(inputName.TabName.Trim())
                If Not String.IsNullOrEmpty(ret) Then
                    MessageBox.Show(My.Resources.FRMessage2 + ret)
                Else
                    MessageBox.Show(My.Resources.FRMessage3)
                End If
            End If
            inputName.Dispose()
        End Using
    End Sub

    Private Sub RemoveCommandMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveCommandMenuItem.Click
        Dim id As String = ""
        If _curPost IsNot Nothing Then id = _curPost.Name
        RemoveCommand(id)
    End Sub

    Private Sub RemoveCommand(ByVal id As String)
        Using inputName As New InputTabName()
            inputName.FormTitle = "Unfollow"
            inputName.FormDescription = My.Resources.FRMessage1
            inputName.TabName = id
            If inputName.ShowDialog() = Windows.Forms.DialogResult.OK AndAlso _
               Not String.IsNullOrEmpty(inputName.TabName.Trim()) Then
                Dim ret As String = Twitter.PostRemoveCommand(inputName.TabName.Trim())
                If Not String.IsNullOrEmpty(ret) Then
                    MessageBox.Show(My.Resources.FRMessage2 + ret)
                Else
                    MessageBox.Show(My.Resources.FRMessage3)
                End If
            End If
            inputName.Dispose()
        End Using
    End Sub

    Private Sub FriendshipMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FriendshipMenuItem.Click
        Dim id As String = ""
        If _curPost IsNot Nothing Then
            id = _curPost.Name
        End If
        ShowFriendship(id)
    End Sub

    Private Sub ShowFriendship(ByVal id As String)
        Using inputName As New InputTabName()
            inputName.FormTitle = "Show Friendships"
            inputName.FormDescription = My.Resources.FRMessage1
            inputName.TabName = id
            If inputName.ShowDialog() = Windows.Forms.DialogResult.OK AndAlso _
               Not String.IsNullOrEmpty(inputName.TabName.Trim()) Then
                Dim isFollowing As Boolean = False
                Dim isFollowed As Boolean = False
                Dim result As String = ""
                id = inputName.TabName.Trim
                Dim ret As String = Twitter.GetFriendshipInfo(id, isFollowing, isFollowed)
                If ret = "" Then
                    If isFollowing Then
                        result = My.Resources.GetFriendshipInfo1 + System.Environment.NewLine
                    Else
                        result = My.Resources.GetFriendshipInfo2 + System.Environment.NewLine
                    End If
                    If isFollowed Then
                        result += My.Resources.GetFriendshipInfo3
                    Else
                        result += My.Resources.GetFriendshipInfo4
                    End If
                    result = id + My.Resources.GetFriendshipInfo5 + System.Environment.NewLine + result
                Else
                    result = ret
                End If
                MessageBox.Show(result)
            End If
            inputName.Dispose()
        End Using
    End Sub

    Private Sub OwnStatusMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OwnStatusMenuItem.Click
        Dim loc As String = ""
        Dim bio As String = ""
        If Not String.IsNullOrEmpty(Twitter.Location) Then
            loc = Twitter.Location
        End If
        If Not String.IsNullOrEmpty(Twitter.Bio) Then
            bio = Twitter.Bio
        End If
        If Twitter.FriendsCount = 0 AndAlso Twitter.FollowersCount = 0 AndAlso Twitter.StatusesCount = 0 AndAlso loc = "" AndAlso bio = "" Then
            MessageBox.Show(My.Resources.ShowYourProfileText1, "Your status", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If
        MessageBox.Show("Following : " + Twitter.FriendsCount.ToString() + Environment.NewLine + _
                        "Followers : " + Twitter.FollowersCount.ToString() + Environment.NewLine + _
                        "Statuses count : " + Twitter.StatusesCount.ToString() + Environment.NewLine + _
                        "Location : " + loc + Environment.NewLine + _
                        "Bio : " + bio, "Your status")
    End Sub

    Private Sub FollowContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FollowContextMenuItem.Click
        Dim m As Match = Regex.Match(PostBrowser.StatusText, "^https?://twitter.com/(?<name>[a-zA-Z0-9_]+)$")
        If m.Success Then
            FollowCommand(m.Result("${name}"))
        End If
    End Sub

    Private Sub RemoveContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveContextMenuItem.Click
        Dim m As Match = Regex.Match(PostBrowser.StatusText, "^https?://twitter.com/(?<name>[a-zA-Z0-9_]+)$")
        If m.Success Then
            RemoveCommand(m.Result("${name}"))
        End If
    End Sub

    Private Sub FriendshipContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FriendshipContextMenuItem.Click
        Dim m As Match = Regex.Match(PostBrowser.StatusText, "^https?://twitter.com/(?<name>[a-zA-Z0-9_]+)$")
        If m.Success Then
            ShowFriendship(m.Result("${name}"))
        End If
    End Sub

    Private Sub IdeographicSpaceToSpaceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IdeographicSpaceToSpaceToolStripMenuItem.Click
        modifySettingCommon = True
    End Sub

    'Private Sub UserPicture_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles UserPicture.Paint
    '    If e.Graphics.InterpolationMode <> Drawing2D.InterpolationMode.HighQualityBicubic Then
    '        e.Graphics.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
    '        UserPicture.GetType().GetMethod("OnPaint", BindingFlags.NonPublic Or BindingFlags.Instance).Invoke(UserPicture, New Object() {e})
    '    End If
    'End Sub

    Private Sub QuoteStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles QuoteStripMenuItem.Click, QtOpMenuItem.Click
        'QT @id:内容
        '返信先情報付加
        If _curPost IsNot Nothing Then
            If _curPost.IsDm OrElse _
               Not StatusText.Enabled Then Exit Sub

            If SettingDialog.ProtectNotInclude AndAlso _curPost.IsProtect Then
                MessageBox.Show("Protected.")
                Exit Sub
            End If
            Dim rtdata As String = _curPost.OriginalData
            rtdata = CreateRetweet(rtdata)

            StatusText.Text = " QT @" + _curPost.Name + ": " + HttpUtility.HtmlDecode(rtdata)
            _reply_to_id = _curPost.Id
            _reply_to_name = _curPost.Name

            StatusText.SelectionStart = 0
            StatusText.Focus()
        End If
    End Sub

    Private Sub SearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '公式検索
        Dim pnl As Control = DirectCast(sender, Control).Parent
        If pnl Is Nothing Then Exit Sub
        Dim tbName As String = pnl.Parent.Text
        Dim tb As TabClass = _statuses.Tabs(tbName)
        Dim cmb As ComboBox = DirectCast(pnl.Controls("comboSearch"), ComboBox)
        Dim cmbLang As ComboBox = DirectCast(pnl.Controls("comboLang"), ComboBox)
        cmb.Text = cmb.Text.Trim
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
            AddNewTab(renamed, False, tb.TabType)
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1
            SaveConfigsTabs()
        End If
    End Sub

    Private Sub MoveToRTHomeMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveToRTHomeMenuItem.Click, OpenRterHomeMenuItem.Click
        If _curList.SelectedIndices.Count > 0 Then
            Dim post As PostClass = GetCurTabPost(_curList.SelectedIndices(0))
            If post.RetweetedId > 0 Then
                OpenUriAsync("http://twitter.com/" + GetCurTabPost(_curList.SelectedIndices(0)).RetweetedBy)
            End If
        End If
    End Sub

    Private Sub IdFilterAddMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IdFilterAddMenuItem.Click
        Dim m As Match = Regex.Match(PostBrowser.StatusText, "^https?://twitter.com/(?<name>[a-zA-Z0-9_]+)$")
        If m.Success Then
            Dim tabName As String = ""

            '未選択なら処理終了
            If _curList.SelectedIndices.Count = 0 Then Exit Sub

            'タブ選択（or追加）
            If Not SelectTab(tabName) Then Exit Sub

            Dim mv As Boolean = False
            Dim mk As Boolean = False
            MoveOrCopy(mv, mk)

            Dim fc As New FiltersClass
            fc.NameFilter = m.Result("${name}")
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
        Dim m As Match = Regex.Match(PostBrowser.StatusText, "^https?://twitter.com/search\?q=%23(?<hash>[a-zA-Z0-9_]+)$")
        If m.Success Then
            HashMgr.SetPermanentHash("#" + m.Result("${hash}"))
            HashStripSplitButton.Text = HashMgr.UseHash
            '使用ハッシュタグとして設定
            modifySettingCommon = True
        End If
    End Sub

    Private Sub StatusLabel_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusLabel.DoubleClick
        MessageBox.Show(StatusLabel.TextHistory, "Logs", MessageBoxButtons.OK, MessageBoxIcon.None)
    End Sub

    Private Sub HashManageMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HashManageMenuItem.Click
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
        Else
            HashStripSplitButton.Text = "#[-]"
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
        modifySettingCommon = True
    End Sub

    Private Sub HashToggleMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles HashToggleMenuItem.Click
        HashMgr.ToggleHash()
        If HashMgr.UseHash <> "" Then
            HashStripSplitButton.Text = HashMgr.UseHash
        Else
            HashStripSplitButton.Text = "#[-]"
        End If
        modifySettingCommon = True
    End Sub

    Private Sub HashStripSplitButton_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles HashStripSplitButton.ButtonClick
        HashToggleMenuItem_Click(Nothing, Nothing)
    End Sub

    Private Sub MenuItemOperate_DropDownOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItemOperate.DropDownOpening
        If ListTab.SelectedTab Is Nothing Then Exit Sub
        If _statuses Is Nothing OrElse _statuses.Tabs Is Nothing OrElse Not _statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text) Then Exit Sub
        If _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.DirectMessage Then
            Me.FavOpMenuItem.Enabled = False
            Me.UnFavOpMenuItem.Enabled = False
            Me.OpenStatusOpMenuItem.Enabled = False
            Me.OpenFavotterOpMenuItem.Enabled = False
        Else
            Me.FavOpMenuItem.Enabled = True
            Me.UnFavOpMenuItem.Enabled = True
            Me.OpenStatusOpMenuItem.Enabled = True
            Me.OpenFavotterOpMenuItem.Enabled = True
        End If
        If _curPost Is Nothing OrElse _curPost.IsDm Then
            Me.RtOpMenuItem.Enabled = False
            Me.RtUnOpMenuItem.Enabled = False
            Me.QtOpMenuItem.Enabled = False
        Else
            Me.RtOpMenuItem.Enabled = True
            Me.RtUnOpMenuItem.Enabled = True
            Me.QtOpMenuItem.Enabled = True
        End If
        If _statuses.Tabs(ListTab.SelectedTab.Text).TabType = TabUsageType.PublicSearch Then
            Me.RefreshPrevOpMenuItem.Enabled = True
        Else
            Me.RefreshPrevOpMenuItem.Enabled = False
        End If
    End Sub

    Private Sub MenuItemTab_DropDownOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItemTab.DropDownOpening
        ContextMenuTabProperty_Opening(sender, Nothing)
    End Sub

End Class
