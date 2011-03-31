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

Imports System.Threading

Public Class AppendSettingDialog

    Private Shared _instance As New AppendSettingDialog
    Private tw As Twitter
    Private _MytimelinePeriod As Integer
    Private _MyDMPeriod As Integer
    Private _MyPubSearchPeriod As Integer
    Private _MyListsPeriod As Integer
    Private _MyUserTimelinePeriod As Integer
    Private _MyLogDays As Integer
    Private _MyLogUnit As LogUnitEnum
    Private _MyReaded As Boolean
    Private _MyIconSize As IconSizes
    Private _MyStatusText As String
    Private _MyRecommendStatusText As String
    Private _MyUnreadManage As Boolean
    Private _MyPlaySound As Boolean
    Private _MyOneWayLove As Boolean
    Private _fntUnread As Font
    Private _clUnread As Color
    Private _fntReaded As Font
    Private _clReaded As Color
    Private _clFav As Color
    Private _clOWL As Color
    Private _clRetweet As Color
    Private _fntDetail As Font
    Private _clSelf As Color
    Private _clAtSelf As Color
    Private _clTarget As Color
    Private _clAtTarget As Color
    Private _clAtFromTarget As Color
    Private _clAtTo As Color
    Private _clInputBackcolor As Color
    Private _clInputFont As Color
    Private _fntInputFont As Font
    Private _clListBackcolor As Color
    Private _clDetailBackcolor As Color
    Private _clDetail As Color
    Private _clDetailLink As Color
    Private _MyNameBalloon As NameBalloonEnum
    Private _MyPostCtrlEnter As Boolean
    Private _MyPostShiftEnter As Boolean
    Private _usePostMethod As Boolean
    Private _countApi As Integer
    Private _countApiReply As Integer
    Private _browserpath As String
    Private _MyUseRecommendStatus As Boolean
    Private _MyDispUsername As Boolean
    Private _MyDispLatestPost As DispTitleEnum
    Private _MySortOrderLock As Boolean
    Private _MyMinimizeToTray As Boolean
    Private _MyCloseToExit As Boolean
    Private _MyTinyUrlResolve As Boolean
    Private _MyShortUrlForceResolve As Boolean
    Private _MyProxyType As HttpConnection.ProxyType
    Private _MyProxyAddress As String
    Private _MyProxyPort As Integer
    Private _MyProxyUser As String
    Private _MyProxyPassword As String
    Private _MyPeriodAdjust As Boolean
    Private _MyStartupVersion As Boolean
    Private _MyStartupFollowers As Boolean
    Private _MyRestrictFavCheck As Boolean
    Private _MyAlwaysTop As Boolean
    Private _MyUrlConvertAuto As Boolean
    Private _MyOutputz As Boolean
    Private _MyOutputzKey As String
    Private _MyOutputzUrlmode As OutputzUrlmode
    Private _MyNicoms As Boolean
    Private _MyUnreadStyle As Boolean
    Private _MyDateTimeFormat As String
    Private _MyDefaultTimeOut As Integer
    Private _MyLimitBalloon As Boolean
    Private _MyPostAndGet As Boolean
    Private _MyReplyPeriod As Integer
    Private _MyAutoShortUrlFirst As UrlConverter
    Private _MyTabIconDisp As Boolean
    Private _MyReplyIconState As REPLY_ICONSTATE
    Private _MyReadOwnPost As Boolean
    Private _MyGetFav As Boolean
    Private _MyMonoSpace As Boolean
    Private _MyReadOldPosts As Boolean
    Private _MyUseSsl As Boolean
    Private _MyBitlyId As String
    Private _MyBitlyPw As String
    Private _MyShowGrid As Boolean
    Private _MyUseAtIdSupplement As Boolean
    Private _MyUseHashSupplement As Boolean
    Private _MyLanguage As String
    Private _MyIsOAuth As Boolean
    Private _MyTwitterApiUrl As String
    Private _MyTwitterSearchApiUrl As String
    Private _MyPreviewEnable As Boolean
    Private _MoreCountApi As Integer
    Private _FirstCountApi As Integer
    Private _MyUseAdditonalCount As Boolean
    Private _SearchCountApi As Integer
    Private _FavoritesCountApi As Integer
    Private _UserTimelineCountApi As Integer
    Private _ListCountApi As Integer
    Private _MyRetweetNoConfirm As Boolean
    Private _MyUserstreamStartup As Boolean
    Private _MyUserstreamPeriod As Integer
    Private _MyOpenUserTimeline As Boolean

    Private _ValidationError As Boolean = False
    Private _MyEventNotifyEnabled As Boolean
    Private _MyEventNotifyFlag As EVENTTYPE
    Private _isMyEventNotifyFlag As EVENTTYPE
    Private _MyForceEventNotify As Boolean
    Private _MyFavEventUnread As Boolean
    Private _MyTranslateLanguage As String
    Private _soundfileListup As Boolean = False
    Private _MyEventSoundFile As String

    Private _MyDoubleClickAction As Integer
    Private _UserAppointUrl As String
    Public Property HideDuplicatedRetweets As Boolean

    Private Sub TreeViewSetting_BeforeSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs) Handles TreeViewSetting.BeforeSelect
        If Me.TreeViewSetting.SelectedNode Is Nothing Then Exit Sub
        Dim pnl = DirectCast(Me.TreeViewSetting.SelectedNode.Tag, Panel)
        If pnl Is Nothing Then Exit Sub
        pnl.Enabled = False
        pnl.Visible = False
    End Sub

    Private Sub TreeViewSetting_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeViewSetting.AfterSelect
        If e.Node Is Nothing Then Exit Sub
        Dim pnl = DirectCast(e.Node.Tag, Panel)
        If pnl Is Nothing Then Exit Sub
        pnl.Enabled = True
        pnl.Visible = True
    End Sub

    'Private Sub ToggleNodeChange(ByVal node As TreeNode)
    '    If node Is Nothing Then Exit Sub
    '    TreeViewSetting.BeginUpdate()
    '    If node.IsExpanded Then
    '        node.Collapse()
    '    Else
    '        node.Expand()
    '    End If
    '    TreeViewSetting.EndUpdate()
    'End Sub

    'Private Sub TreeViewSetting_DrawNode(ByVal sender As Object, ByVal e As System.Windows.Forms.DrawTreeNodeEventArgs) Handles TreeViewSetting.DrawNode
    '    e.DrawDefault = True
    '    If (e.State And TreeNodeStates.Selected) = TreeNodeStates.Selected Then
    '        Dim pnl = DirectCast(e.Node.Tag, Panel)
    '        If pnl Is Nothing Then Exit Sub
    '        If _curPanel IsNot Nothing Then
    '            If pnl.Name <> _curPanel.Name Then
    '                _curPanel.Enabled = False
    '                _curPanel.Visible = False

    '                _curPanel = pnl
    '                pnl.Enabled = True
    '                pnl.Visible = True
    '            End If
    '        End If
    '    End If
    'End Sub

    'Private Sub TreeViewSetting_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TreeViewSetting.MouseDown
    '    Dim info As TreeViewHitTestInfo = TreeViewSetting.HitTest(e.X, e.Y)
    '    If CBool((info.Location And TreeViewHitTestLocations.Label)) Then
    '        ToggleNodeChange(info.Node)
    '    End If
    'End Sub

    Private Sub Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Save.Click
        If TweenMain.IsNetworkAvailable() AndAlso _
            (ComboBoxAutoShortUrlFirst.SelectedIndex = UrlConverter.Bitly OrElse ComboBoxAutoShortUrlFirst.SelectedIndex = UrlConverter.Jmp) AndAlso _
             (Not String.IsNullOrEmpty(TextBitlyId.Text) OrElse Not String.IsNullOrEmpty(TextBitlyPw.Text)) Then
            If Not BitlyValidation(TextBitlyId.Text, TextBitlyPw.Text) Then
                MessageBox.Show(My.Resources.SettingSave_ClickText1)
                _ValidationError = True
                TreeViewSetting.SelectedNode.Name = "TweetActNode" ' 動作タブを選択
                TextBitlyId.Focus()
                Exit Sub
            Else
                _ValidationError = False
            End If
        Else
            _ValidationError = False
        End If
        If Me.Username.Focused OrElse Me.Password.Focused Then
            If Not Authorize() Then
                _ValidationError = True
                Exit Sub
            End If
        End If
        Try
            _MyUserstreamPeriod = CType(Me.UserstreamPeriod.Text, Integer)
            _MyUserstreamStartup = Me.StartupUserstreamCheck.Checked
            _MyIsOAuth = AuthOAuthRadio.Checked
            _MytimelinePeriod = CType(TimelinePeriod.Text, Integer)
            _MyDMPeriod = CType(DMPeriod.Text, Integer)
            _MyPubSearchPeriod = CType(PubSearchPeriod.Text, Integer)
            _MyListsPeriod = CType(ListsPeriod.Text, Integer)
            _MyReplyPeriod = CType(ReplyPeriod.Text, Integer)
            _MyUserTimelinePeriod = CType(UserTimelinePeriod.Text, Integer)

            _MyReaded = StartupReaded.Checked
            Select Case IconSize.SelectedIndex
                Case 0
                    _MyIconSize = IconSizes.IconNone
                Case 1
                    _MyIconSize = IconSizes.Icon16
                Case 2
                    _MyIconSize = IconSizes.Icon24
                Case 3
                    _MyIconSize = IconSizes.Icon48
                Case 4
                    _MyIconSize = IconSizes.Icon48_2
            End Select
            _MyStatusText = StatusText.Text
            _MyPlaySound = PlaySnd.Checked
            _MyUnreadManage = UReadMng.Checked
            _MyOneWayLove = OneWayLv.Checked

            _fntUnread = lblUnread.Font     '未使用
            _clUnread = lblUnread.ForeColor
            _fntReaded = lblListFont.Font     'リストフォントとして使用
            _clReaded = lblListFont.ForeColor
            _clFav = lblFav.ForeColor
            _clOWL = lblOWL.ForeColor
            _clRetweet = lblRetweet.ForeColor
            _fntDetail = lblDetail.Font
            _clSelf = lblSelf.BackColor
            _clAtSelf = lblAtSelf.BackColor
            _clTarget = lblTarget.BackColor
            _clAtTarget = lblAtTarget.BackColor
            _clAtFromTarget = lblAtFromTarget.BackColor
            _clAtTo = lblAtTo.BackColor
            _clInputBackcolor = lblInputBackcolor.BackColor
            _clInputFont = lblInputFont.ForeColor
            _clListBackcolor = lblListBackcolor.BackColor
            _clDetailBackcolor = lblDetailBackcolor.BackColor
            _clDetail = lblDetail.ForeColor
            _clDetailLink = lblDetailLink.ForeColor
            _fntInputFont = lblInputFont.Font
            Select Case cmbNameBalloon.SelectedIndex
                Case 0
                    _MyNameBalloon = NameBalloonEnum.None
                Case 1
                    _MyNameBalloon = NameBalloonEnum.UserID
                Case 2
                    _MyNameBalloon = NameBalloonEnum.NickName
            End Select

            Select Case ComboBoxPostKeySelect.SelectedIndex
                Case 2
                    _MyPostShiftEnter = True
                    _MyPostCtrlEnter = False
                Case 1
                    _MyPostCtrlEnter = True
                    _MyPostShiftEnter = False
                Case 0
                    _MyPostCtrlEnter = False
                    _MyPostShiftEnter = False
            End Select
            _usePostMethod = False
            _countApi = CType(TextCountApi.Text, Integer)
            _countApiReply = CType(TextCountApiReply.Text, Integer)
            _browserpath = BrowserPathText.Text.Trim
            _MyPostAndGet = CheckPostAndGet.Checked
            _MyUseRecommendStatus = CheckUseRecommendStatus.Checked
            _MyDispUsername = CheckDispUsername.Checked
            _MyCloseToExit = CheckCloseToExit.Checked
            _MyMinimizeToTray = CheckMinimizeToTray.Checked
            Select Case ComboDispTitle.SelectedIndex
                Case 0  'None
                    _MyDispLatestPost = DispTitleEnum.None
                Case 1  'Ver
                    _MyDispLatestPost = DispTitleEnum.Ver
                Case 2  'Post
                    _MyDispLatestPost = DispTitleEnum.Post
                Case 3  'RepCount
                    _MyDispLatestPost = DispTitleEnum.UnreadRepCount
                Case 4  'AllCount
                    _MyDispLatestPost = DispTitleEnum.UnreadAllCount
                Case 5  'Rep+All
                    _MyDispLatestPost = DispTitleEnum.UnreadAllRepCount
                Case 6  'Unread/All
                    _MyDispLatestPost = DispTitleEnum.UnreadCountAllCount
                Case 7 'Count of Status/Follow/Follower
                    _MyDispLatestPost = DispTitleEnum.OwnStatus
            End Select
            _MySortOrderLock = CheckSortOrderLock.Checked
            _MyTinyUrlResolve = CheckTinyURL.Checked
            _MyShortUrlForceResolve = CheckForceResolve.Checked
            ShortUrl.IsResolve = _MyTinyUrlResolve
            ShortUrl.IsForceResolve = _MyShortUrlForceResolve
            If RadioProxyNone.Checked Then
                _MyProxyType = HttpConnection.ProxyType.None
            ElseIf RadioProxyIE.Checked Then
                _MyProxyType = HttpConnection.ProxyType.IE
            Else
                _MyProxyType = HttpConnection.ProxyType.Specified
            End If
            _MyProxyAddress = TextProxyAddress.Text.Trim()
            _MyProxyPort = Integer.Parse(TextProxyPort.Text.Trim())
            _MyProxyUser = TextProxyUser.Text.Trim()
            _MyProxyPassword = TextProxyPassword.Text.Trim()
            _MyPeriodAdjust = CheckPeriodAdjust.Checked
            _MyStartupVersion = CheckStartupVersion.Checked
            _MyStartupFollowers = CheckStartupFollowers.Checked
            _MyRestrictFavCheck = CheckFavRestrict.Checked
            _MyAlwaysTop = CheckAlwaysTop.Checked
            _MyUrlConvertAuto = CheckAutoConvertUrl.Checked
            _MyOutputz = CheckOutputz.Checked
            _MyOutputzKey = TextBoxOutputzKey.Text.Trim()

            Select Case ComboBoxOutputzUrlmode.SelectedIndex
                Case 0
                    _MyOutputzUrlmode = OutputzUrlmode.twittercom
                Case 1
                    _MyOutputzUrlmode = OutputzUrlmode.twittercomWithUsername
            End Select

            _MyNicoms = CheckNicoms.Checked
            _MyUnreadStyle = chkUnreadStyle.Checked
            _MyDateTimeFormat = CmbDateTimeFormat.Text
            _MyDefaultTimeOut = CType(ConnectionTimeOut.Text, Integer)
            _MyRetweetNoConfirm = CheckRetweetNoConfirm.Checked
            _MyLimitBalloon = CheckBalloonLimit.Checked
            _MyEventNotifyEnabled = CheckEventNotify.Checked
            GetEventNotifyFlag(_MyEventNotifyFlag, _isMyEventNotifyFlag)
            _MyForceEventNotify = CheckForceEventNotify.Checked
            _MyFavEventUnread = CheckFavEventUnread.Checked
            _MyTranslateLanguage = (New Google).GetLanguageEnumFromIndex(ComboBoxTranslateLanguage.SelectedIndex)
            _MyEventSoundFile = CStr(ComboBoxEventNotifySound.SelectedItem)
            _MyAutoShortUrlFirst = CType(ComboBoxAutoShortUrlFirst.SelectedIndex, UrlConverter)
            _MyTabIconDisp = chkTabIconDisp.Checked
            _MyReadOwnPost = chkReadOwnPost.Checked
            _MyGetFav = chkGetFav.Checked
            _MyMonoSpace = CheckMonospace.Checked
            _MyReadOldPosts = CheckReadOldPosts.Checked
            _MyUseSsl = CheckUseSsl.Checked
            _MyBitlyId = TextBitlyId.Text
            _MyBitlyPw = TextBitlyPw.Text
            _MyShowGrid = CheckShowGrid.Checked
            _MyUseAtIdSupplement = CheckAtIdSupple.Checked
            _MyUseHashSupplement = CheckHashSupple.Checked
            _MyPreviewEnable = CheckPreviewEnable.Checked
            _MyTwitterApiUrl = TwitterAPIText.Text.Trim
            _MyTwitterSearchApiUrl = TwitterSearchAPIText.Text.Trim
            Select Case ReplyIconStateCombo.SelectedIndex
                Case 0
                    _MyReplyIconState = REPLY_ICONSTATE.None
                Case 1
                    _MyReplyIconState = REPLY_ICONSTATE.StaticIcon
                Case 2
                    _MyReplyIconState = REPLY_ICONSTATE.BlinkIcon
            End Select
            Select Case LanguageCombo.SelectedIndex
                Case 0
                    _MyLanguage = "OS"
                Case 1
                    _MyLanguage = "ja"
                Case 2
                    _MyLanguage = "en"
                Case 3
                    _MyLanguage = "zh-CN"
                Case Else
                    _MyLanguage = "en"
            End Select
            _HotkeyEnabled = Me.HotkeyCheck.Checked
            _HotkeyMod = Keys.None
            If Me.HotkeyAlt.Checked Then _HotkeyMod = _HotkeyMod Or Keys.Alt
            If Me.HotkeyShift.Checked Then _HotkeyMod = _HotkeyMod Or Keys.Shift
            If Me.HotkeyCtrl.Checked Then _HotkeyMod = _HotkeyMod Or Keys.Control
            If Me.HotkeyWin.Checked Then _HotkeyMod = _HotkeyMod Or Keys.LWin
            If IsNumeric(HotkeyCode.Text) Then _HotkeyValue = CInt(HotkeyCode.Text)
            _HotkeyKey = DirectCast(HotkeyText.Tag, Keys)
            _BlinkNewMentions = ChkNewMentionsBlink.Checked
            _MyUseAdditonalCount = UseChangeGetCount.Checked
            _MoreCountApi = CType(GetMoreTextCountApi.Text, Integer)
            _FirstCountApi = CType(FirstTextCountApi.Text, Integer)
            _SearchCountApi = CType(SearchTextCountApi.Text, Integer)
            _FavoritesCountApi = CType(FavoritesTextCountApi.Text, Integer)
            _UserTimelineCountApi = CType(UserTimelineTextCountApi.Text, Integer)
            _ListCountApi = CType(ListTextCountApi.Text, Integer)
            _MyOpenUserTimeline = CheckOpenUserTimeline.Checked
            _MyDoubleClickAction = ListDoubleClickActionComboBox.SelectedIndex
            _UserAppointUrl = UserAppointUrlText.Text
            Me.HideDuplicatedRetweets = Me.HideDuplicatedRetweetsCheck.Checked
        Catch ex As Exception
            MessageBox.Show(My.Resources.Save_ClickText3)
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Exit Sub
        End Try
    End Sub

    Private Sub Setting_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If tw IsNot Nothing AndAlso tw.Username = "" AndAlso e.CloseReason = CloseReason.None Then
            If MessageBox.Show(My.Resources.Setting_FormClosing1, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                e.Cancel = True
            End If
        End If
        If _ValidationError Then
            e.Cancel = True
        End If
        If e.Cancel = False AndAlso TreeViewSetting.SelectedNode IsNot Nothing Then
            Dim curPanel As Panel = CType(TreeViewSetting.SelectedNode.Tag, Panel)
            curPanel.Visible = False
            curPanel.Enabled = False
        End If
    End Sub

    Private Sub Setting_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        tw = DirectCast(Me.Owner, TweenMain).TwitterInstance
        Dim uname As String = tw.Username
        Dim pw As String = tw.Password
        Dim tk As String = tw.AccessToken
        Dim tks As String = tw.AccessTokenSecret
        If Not Me._MyIsOAuth Then
            'BASIC認証時のみ表示
            Me.AuthStateLabel.Enabled = False
            Me.AuthUserLabel.Enabled = False
            Me.AuthClearButton.Enabled = False
            Me.AuthOAuthRadio.Checked = False
            Me.AuthBasicRadio.Checked = True
            Me.CheckEnableBasicAuth.Checked = True
            Me.AuthBasicRadio.Enabled = True
            tw.Initialize(uname, pw)
        Else
            Me.AuthStateLabel.Enabled = True
            Me.AuthUserLabel.Enabled = True
            Me.AuthClearButton.Enabled = True
            Me.AuthOAuthRadio.Checked = True
            Me.AuthBasicRadio.Checked = False
            tw.Initialize(tk, tks, uname)
        End If

        Username.Text = uname
        Password.Text = pw
        If tw.Username = "" Then
            Me.AuthStateLabel.Text = My.Resources.AuthorizeButton_Click4
            Me.AuthUserLabel.Text = ""
        Else
            Me.AuthStateLabel.Text = My.Resources.AuthorizeButton_Click3
            Me.AuthUserLabel.Text = tw.Username
        End If

        Me.StartupUserstreamCheck.Checked = _MyUserstreamStartup
        Me.UserstreamPeriod.Text = _MyUserstreamPeriod.ToString()
        TimelinePeriod.Text = _MytimelinePeriod.ToString()
        ReplyPeriod.Text = _MyReplyPeriod.ToString()
        DMPeriod.Text = _MyDMPeriod.ToString()
        PubSearchPeriod.Text = _MyPubSearchPeriod.ToString()
        ListsPeriod.Text = _MyListsPeriod.ToString()
        UserTimelinePeriod.Text = _MyUserTimelinePeriod.ToString

        StartupReaded.Checked = _MyReaded
        Select Case _MyIconSize
            Case IconSizes.IconNone
                IconSize.SelectedIndex = 0
            Case IconSizes.Icon16
                IconSize.SelectedIndex = 1
            Case IconSizes.Icon24
                IconSize.SelectedIndex = 2
            Case IconSizes.Icon48
                IconSize.SelectedIndex = 3
            Case IconSizes.Icon48_2
                IconSize.SelectedIndex = 4
        End Select
        StatusText.Text = _MyStatusText
        UReadMng.Checked = _MyUnreadManage
        If _MyUnreadManage = False Then
            StartupReaded.Enabled = False
        Else
            StartupReaded.Enabled = True
        End If
        PlaySnd.Checked = _MyPlaySound
        OneWayLv.Checked = _MyOneWayLove

        lblListFont.Font = _fntReaded
        lblUnread.Font = _fntUnread
        lblUnread.ForeColor = _clUnread
        lblListFont.ForeColor = _clReaded
        lblFav.ForeColor = _clFav
        lblOWL.ForeColor = _clOWL
        lblRetweet.ForeColor = _clRetweet
        lblDetail.Font = _fntDetail
        lblSelf.BackColor = _clSelf
        lblAtSelf.BackColor = _clAtSelf
        lblTarget.BackColor = _clTarget
        lblAtTarget.BackColor = _clAtTarget
        lblAtFromTarget.BackColor = _clAtFromTarget
        lblAtTo.BackColor = _clAtTo
        lblInputBackcolor.BackColor = _clInputBackcolor
        lblInputFont.ForeColor = _clInputFont
        lblInputFont.Font = _fntInputFont
        lblListBackcolor.BackColor = _clListBackcolor
        lblDetailBackcolor.BackColor = _clDetailBackcolor
        lblDetail.ForeColor = _clDetail
        lblDetailLink.ForeColor = _clDetailLink

        Select Case _MyNameBalloon
            Case NameBalloonEnum.None
                cmbNameBalloon.SelectedIndex = 0
            Case NameBalloonEnum.UserID
                cmbNameBalloon.SelectedIndex = 1
            Case NameBalloonEnum.NickName
                cmbNameBalloon.SelectedIndex = 2
        End Select

        If _MyPostCtrlEnter Then
            ComboBoxPostKeySelect.SelectedIndex = 1
        ElseIf _MyPostShiftEnter Then
            ComboBoxPostKeySelect.SelectedIndex = 2
        Else
            ComboBoxPostKeySelect.SelectedIndex = 0
        End If

        TextCountApi.Text = _countApi.ToString
        TextCountApiReply.Text = _countApiReply.ToString
        BrowserPathText.Text = _browserpath
        CheckPostAndGet.Checked = _MyPostAndGet
        CheckUseRecommendStatus.Checked = _MyUseRecommendStatus
        CheckDispUsername.Checked = _MyDispUsername
        CheckCloseToExit.Checked = _MyCloseToExit
        CheckMinimizeToTray.Checked = _MyMinimizeToTray
        Select Case _MyDispLatestPost
            Case DispTitleEnum.None
                ComboDispTitle.SelectedIndex = 0
            Case DispTitleEnum.Ver
                ComboDispTitle.SelectedIndex = 1
            Case DispTitleEnum.Post
                ComboDispTitle.SelectedIndex = 2
            Case DispTitleEnum.UnreadRepCount
                ComboDispTitle.SelectedIndex = 3
            Case DispTitleEnum.UnreadAllCount
                ComboDispTitle.SelectedIndex = 4
            Case DispTitleEnum.UnreadAllRepCount
                ComboDispTitle.SelectedIndex = 5
            Case DispTitleEnum.UnreadCountAllCount
                ComboDispTitle.SelectedIndex = 6
            Case DispTitleEnum.OwnStatus
                ComboDispTitle.SelectedIndex = 7
        End Select
        CheckSortOrderLock.Checked = _MySortOrderLock
        CheckTinyURL.Checked = _MyTinyUrlResolve
        CheckForceResolve.Checked = _MyShortUrlForceResolve
        Select Case _MyProxyType
            Case HttpConnection.ProxyType.None
                RadioProxyNone.Checked = True
            Case HttpConnection.ProxyType.IE
                RadioProxyIE.Checked = True
            Case Else
                RadioProxySpecified.Checked = True
        End Select
        Dim chk As Boolean = RadioProxySpecified.Checked
        LabelProxyAddress.Enabled = chk
        TextProxyAddress.Enabled = chk
        LabelProxyPort.Enabled = chk
        TextProxyPort.Enabled = chk
        LabelProxyUser.Enabled = chk
        TextProxyUser.Enabled = chk
        LabelProxyPassword.Enabled = chk
        TextProxyPassword.Enabled = chk

        TextProxyAddress.Text = _MyProxyAddress
        TextProxyPort.Text = _MyProxyPort.ToString
        TextProxyUser.Text = _MyProxyUser
        TextProxyPassword.Text = _MyProxyPassword

        CheckPeriodAdjust.Checked = _MyPeriodAdjust
        CheckStartupVersion.Checked = _MyStartupVersion
        CheckStartupFollowers.Checked = _MyStartupFollowers
        CheckFavRestrict.Checked = _MyRestrictFavCheck
        CheckAlwaysTop.Checked = _MyAlwaysTop
        CheckAutoConvertUrl.Checked = _MyUrlConvertAuto
        CheckOutputz.Checked = _MyOutputz
        TextBoxOutputzKey.Text = _MyOutputzKey

        Select Case _MyOutputzUrlmode
            Case OutputzUrlmode.twittercom
                ComboBoxOutputzUrlmode.SelectedIndex = 0
            Case OutputzUrlmode.twittercomWithUsername
                ComboBoxOutputzUrlmode.SelectedIndex = 1
        End Select

        CheckNicoms.Checked = _MyNicoms
        chkUnreadStyle.Checked = _MyUnreadStyle
        CmbDateTimeFormat.Text = _MyDateTimeFormat
        ConnectionTimeOut.Text = _MyDefaultTimeOut.ToString
        CheckRetweetNoConfirm.Checked = _MyRetweetNoConfirm
        CheckBalloonLimit.Checked = _MyLimitBalloon

        ApplyEventNotifyFlag(_MyEventNotifyEnabled, _MyEventNotifyFlag, _isMyEventNotifyFlag)
        CheckForceEventNotify.Checked = _MyForceEventNotify
        CheckFavEventUnread.Checked = _MyFavEventUnread
        ComboBoxTranslateLanguage.SelectedIndex = (New Google).GetIndexFromLanguageEnum(_MyTranslateLanguage)
        SoundFileListup()
        ComboBoxAutoShortUrlFirst.SelectedIndex = _MyAutoShortUrlFirst
        chkTabIconDisp.Checked = _MyTabIconDisp
        chkReadOwnPost.Checked = _MyReadOwnPost
        chkGetFav.Checked = _MyGetFav
        CheckMonospace.Checked = _MyMonoSpace
        CheckReadOldPosts.Checked = _MyReadOldPosts
        CheckUseSsl.Checked = _MyUseSsl
        TextBitlyId.Text = _MyBitlyId
        TextBitlyPw.Text = _MyBitlyPw
        TextBitlyId.Modified = False
        TextBitlyPw.Modified = False
        CheckShowGrid.Checked = _MyShowGrid
        CheckAtIdSupple.Checked = _MyUseAtIdSupplement
        CheckHashSupple.Checked = _MyUseHashSupplement
        CheckPreviewEnable.Checked = _MyPreviewEnable
        TwitterAPIText.Text = _MyTwitterApiUrl
        TwitterSearchAPIText.Text = _MyTwitterSearchApiUrl
        Select Case _MyReplyIconState
            Case REPLY_ICONSTATE.None
                ReplyIconStateCombo.SelectedIndex = 0
            Case REPLY_ICONSTATE.StaticIcon
                ReplyIconStateCombo.SelectedIndex = 1
            Case REPLY_ICONSTATE.BlinkIcon
                ReplyIconStateCombo.SelectedIndex = 2
        End Select
        Select Case _MyLanguage
            Case "OS"
                LanguageCombo.SelectedIndex = 0
            Case "ja"
                LanguageCombo.SelectedIndex = 1
            Case "en"
                LanguageCombo.SelectedIndex = 2
            Case "zh-CN"
                LanguageCombo.SelectedIndex = 3
            Case Else
                LanguageCombo.SelectedIndex = 0
        End Select
        HotkeyCheck.Checked = _HotkeyEnabled
        HotkeyAlt.Checked = ((_HotkeyMod And Keys.Alt) = Keys.Alt)
        HotkeyCtrl.Checked = ((_HotkeyMod And Keys.Control) = Keys.Control)
        HotkeyShift.Checked = ((_HotkeyMod And Keys.Shift) = Keys.Shift)
        HotkeyWin.Checked = ((_HotkeyMod And Keys.LWin) = Keys.LWin)
        HotkeyCode.Text = _HotkeyValue.ToString
        HotkeyText.Text = _HotkeyKey.ToString
        HotkeyText.Tag = _HotkeyKey
        HotkeyAlt.Enabled = HotkeyEnabled
        HotkeyShift.Enabled = HotkeyEnabled
        HotkeyCtrl.Enabled = HotkeyEnabled
        HotkeyWin.Enabled = HotkeyEnabled
        HotkeyText.Enabled = HotkeyEnabled
        HotkeyCode.Enabled = HotkeyEnabled
        ChkNewMentionsBlink.Checked = _BlinkNewMentions

        CheckOutputz_CheckedChanged(sender, e)

        GetMoreTextCountApi.Text = _MoreCountApi.ToString
        FirstTextCountApi.Text = _FirstCountApi.ToString
        SearchTextCountApi.Text = _SearchCountApi.ToString
        FavoritesTextCountApi.Text = _FavoritesCountApi.ToString
        UserTimelineTextCountApi.Text = _UserTimelineCountApi.ToString
        ListTextCountApi.Text = _ListCountApi.ToString
        UseChangeGetCount.Checked = _MyUseAdditonalCount
        Label28.Enabled = UseChangeGetCount.Checked
        Label30.Enabled = UseChangeGetCount.Checked
        Label53.Enabled = UseChangeGetCount.Checked
        Label66.Enabled = UseChangeGetCount.Checked
        Label17.Enabled = UseChangeGetCount.Checked
        Label25.Enabled = UseChangeGetCount.Checked
        GetMoreTextCountApi.Enabled = UseChangeGetCount.Checked
        FirstTextCountApi.Enabled = UseChangeGetCount.Checked
        SearchTextCountApi.Enabled = UseChangeGetCount.Checked
        FavoritesTextCountApi.Enabled = UseChangeGetCount.Checked
        UserTimelineTextCountApi.Enabled = UseChangeGetCount.Checked
        ListTextCountApi.Enabled = UseChangeGetCount.Checked
        CheckOpenUserTimeline.Checked = _MyOpenUserTimeline
        ListDoubleClickActionComboBox.SelectedIndex = _MyDoubleClickAction
        UserAppointUrlText.Text = _UserAppointUrl
        Me.HideDuplicatedRetweetsCheck.Checked = Me.HideDuplicatedRetweets

        With Me.TreeViewSetting
            .Nodes("BasedNode").Tag = BasedPanel
            .Nodes("BasedNode").Nodes("PeriodNode").Tag = GetPeriodPanel
            .Nodes("BasedNode").Nodes("StartUpNode").Tag = StartupPanel
            .Nodes("BasedNode").Nodes("GetCountNode").Tag = GetCountPanel
            '.Nodes("BasedNode").Nodes("UserStreamNode").Tag = UserStreamPanel
            .Nodes("ActionNode").Tag = ActionPanel
            .Nodes("ActionNode").Nodes("TweetActNode").Tag = TweetActPanel
            .Nodes("PreviewNode").Tag = PreviewPanel
            .Nodes("PreviewNode").Nodes("TweetPrvNode").Tag = TweetPrvPanel
            .Nodes("PreviewNode").Nodes("NotifyNode").Tag = NotifyPanel
            .Nodes("FontNode").Tag = FontPanel
            .Nodes("FontNode").Nodes("FontNode2").Tag = FontPanel2
            .Nodes("ConnectionNode").Tag = ConnectionPanel
            .Nodes("ConnectionNode").Nodes("ProxyNode").Tag = ProxyPanel
            .Nodes("ConnectionNode").Nodes("CooperateNode").Tag = CooperatePanel
            .Nodes("ConnectionNode").Nodes("ShortUrlNode").Tag = ShortUrlPanel

            .SelectedNode = .Nodes(0)
            .ExpandAll()
        End With
        'TreeViewSetting.SelectedNode = TreeViewSetting.TopNode
        ActiveControl = Username
    End Sub

    Private Sub UserstreamPeriod_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles UserstreamPeriod.Validating
        Dim prd As Integer
        Try
            prd = CType(UserstreamPeriod.Text, Integer)
        Catch ex As Exception
            MessageBox.Show(My.Resources.UserstreamPeriod_ValidatingText1)
            e.Cancel = True
            Exit Sub
        End Try

        If prd < 0 OrElse prd > 60 Then
            MessageBox.Show(My.Resources.UserstreamPeriod_ValidatingText1)
            e.Cancel = True
            Exit Sub
        End If
        CalcApiUsing()
    End Sub

    Private Sub TimelinePeriod_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TimelinePeriod.Validating
        Dim prd As Integer
        Try
            prd = CType(TimelinePeriod.Text, Integer)
        Catch ex As Exception
            MessageBox.Show(My.Resources.TimelinePeriod_ValidatingText1)
            e.Cancel = True
            Exit Sub
        End Try

        If prd <> 0 AndAlso (prd < 15 OrElse prd > 6000) Then
            MessageBox.Show(My.Resources.TimelinePeriod_ValidatingText2)
            e.Cancel = True
            Exit Sub
        End If
        CalcApiUsing()
    End Sub

    Private Sub ReplyPeriod_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ReplyPeriod.Validating
        Dim prd As Integer
        Try
            prd = CType(ReplyPeriod.Text, Integer)
        Catch ex As Exception
            MessageBox.Show(My.Resources.TimelinePeriod_ValidatingText1)
            e.Cancel = True
            Exit Sub
        End Try

        If prd <> 0 AndAlso (prd < 15 OrElse prd > 6000) Then
            MessageBox.Show(My.Resources.TimelinePeriod_ValidatingText2)
            e.Cancel = True
            Exit Sub
        End If
        CalcApiUsing()
    End Sub

    Private Sub DMPeriod_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles DMPeriod.Validating
        Dim prd As Integer
        Try
            prd = CType(DMPeriod.Text, Integer)
        Catch ex As Exception
            MessageBox.Show(My.Resources.DMPeriod_ValidatingText1)
            e.Cancel = True
            Exit Sub
        End Try

        If prd <> 0 AndAlso (prd < 15 OrElse prd > 6000) Then
            MessageBox.Show(My.Resources.DMPeriod_ValidatingText2)
            e.Cancel = True
            Exit Sub
        End If
        CalcApiUsing()
    End Sub

    Private Sub PubSearchPeriod_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles PubSearchPeriod.Validating
        Dim prd As Integer
        Try
            prd = CType(PubSearchPeriod.Text, Integer)
        Catch ex As Exception
            MessageBox.Show(My.Resources.PubSearchPeriod_ValidatingText1)
            e.Cancel = True
            Exit Sub
        End Try

        If prd <> 0 AndAlso (prd < 30 OrElse prd > 6000) Then
            MessageBox.Show(My.Resources.PubSearchPeriod_ValidatingText2)
            e.Cancel = True
        End If
    End Sub

    Private Sub ListsPeriod_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ListsPeriod.Validating
        Dim prd As Integer
        Try
            prd = CType(ListsPeriod.Text, Integer)
        Catch ex As Exception
            MessageBox.Show(My.Resources.DMPeriod_ValidatingText1)
            e.Cancel = True
            Exit Sub
        End Try

        If prd <> 0 AndAlso (prd < 15 OrElse prd > 6000) Then
            MessageBox.Show(My.Resources.DMPeriod_ValidatingText2)
            e.Cancel = True
            Exit Sub
        End If
        CalcApiUsing()
    End Sub

    Private Sub UserTimeline_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles UserTimelinePeriod.Validating
        Dim prd As Integer
        Try
            prd = CType(UserTimelinePeriod.Text, Integer)
        Catch ex As Exception
            MessageBox.Show(My.Resources.DMPeriod_ValidatingText1)
            e.Cancel = True
            Exit Sub
        End Try

        If prd <> 0 AndAlso (prd < 15 OrElse prd > 6000) Then
            MessageBox.Show(My.Resources.DMPeriod_ValidatingText2)
            e.Cancel = True
            Exit Sub
        End If
        CalcApiUsing()
    End Sub

    Private Sub UReadMng_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UReadMng.CheckedChanged
        If UReadMng.Checked = True Then
            StartupReaded.Enabled = True
        Else
            StartupReaded.Enabled = False
        End If
    End Sub

    Private Sub btnFontAndColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUnread.Click, btnDetail.Click, btnListFont.Click, btnInputFont.Click
        Dim Btn As Button = CType(sender, Button)
        Dim rtn As DialogResult

        FontDialog1.AllowVerticalFonts = False
        FontDialog1.AllowScriptChange = True
        FontDialog1.AllowSimulations = True
        FontDialog1.AllowVectorFonts = True
        FontDialog1.FixedPitchOnly = False
        FontDialog1.FontMustExist = True
        FontDialog1.ScriptsOnly = False
        FontDialog1.ShowApply = False
        FontDialog1.ShowEffects = True
        FontDialog1.ShowColor = True

        Select Case Btn.Name
            Case "btnUnread"
                FontDialog1.Color = lblUnread.ForeColor
                FontDialog1.Font = lblUnread.Font
            Case "btnDetail"
                FontDialog1.Color = lblDetail.ForeColor
                FontDialog1.Font = lblDetail.Font
            Case "btnListFont"
                FontDialog1.Color = lblListFont.ForeColor
                FontDialog1.Font = lblListFont.Font
            Case "btnInputFont"
                FontDialog1.Color = lblInputFont.ForeColor
                FontDialog1.Font = lblInputFont.Font
        End Select

        Try
            rtn = FontDialog1.ShowDialog
        Catch ex As ArgumentException
            MessageBox.Show(ex.Message)
            Exit Sub
        End Try

        If rtn = Windows.Forms.DialogResult.Cancel Then Exit Sub

        Select Case Btn.Name
            Case "btnUnread"
                lblUnread.ForeColor = FontDialog1.Color
                lblUnread.Font = FontDialog1.Font
            Case "btnDetail"
                lblDetail.ForeColor = FontDialog1.Color
                lblDetail.Font = FontDialog1.Font
            Case "btnListFont"
                lblListFont.ForeColor = FontDialog1.Color
                lblListFont.Font = FontDialog1.Font
            Case "btnInputFont"
                lblInputFont.ForeColor = FontDialog1.Color
                lblInputFont.Font = FontDialog1.Font
        End Select

    End Sub

    Private Sub btnColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelf.Click, btnAtSelf.Click, btnTarget.Click, btnAtTarget.Click, btnAtFromTarget.Click, btnFav.Click, btnOWL.Click, btnInputBackcolor.Click, btnAtTo.Click, btnListBack.Click, btnDetailBack.Click, btnDetailLink.Click, btnRetweet.Click
        Dim Btn As Button = CType(sender, Button)
        Dim rtn As DialogResult

        ColorDialog1.AllowFullOpen = True
        ColorDialog1.AnyColor = True
        ColorDialog1.FullOpen = False
        ColorDialog1.SolidColorOnly = False

        Select Case Btn.Name
            Case "btnSelf"
                ColorDialog1.Color = lblSelf.BackColor
            Case "btnAtSelf"
                ColorDialog1.Color = lblAtSelf.BackColor
            Case "btnTarget"
                ColorDialog1.Color = lblTarget.BackColor
            Case "btnAtTarget"
                ColorDialog1.Color = lblAtTarget.BackColor
            Case "btnAtFromTarget"
                ColorDialog1.Color = lblAtFromTarget.BackColor
            Case "btnFav"
                ColorDialog1.Color = lblFav.ForeColor
            Case "btnOWL"
                ColorDialog1.Color = lblOWL.ForeColor
            Case "btnRetweet"
                ColorDialog1.Color = lblRetweet.ForeColor
            Case "btnInputBackcolor"
                ColorDialog1.Color = lblInputBackcolor.BackColor
            Case "btnAtTo"
                ColorDialog1.Color = lblAtTo.BackColor
            Case "btnListBack"
                ColorDialog1.Color = lblListBackcolor.BackColor
            Case "btnDetailBack"
                ColorDialog1.Color = lblDetailBackcolor.BackColor
            Case "btnDetailLink"
                ColorDialog1.Color = lblDetailLink.ForeColor
        End Select

        rtn = ColorDialog1.ShowDialog

        If rtn = Windows.Forms.DialogResult.Cancel Then Exit Sub

        Select Case Btn.Name
            Case "btnSelf"
                lblSelf.BackColor = ColorDialog1.Color
            Case "btnAtSelf"
                lblAtSelf.BackColor = ColorDialog1.Color
            Case "btnTarget"
                lblTarget.BackColor = ColorDialog1.Color
            Case "btnAtTarget"
                lblAtTarget.BackColor = ColorDialog1.Color
            Case "btnAtFromTarget"
                lblAtFromTarget.BackColor = ColorDialog1.Color
            Case "btnFav"
                lblFav.ForeColor = ColorDialog1.Color
            Case "btnOWL"
                lblOWL.ForeColor = ColorDialog1.Color
            Case "btnRetweet"
                lblRetweet.ForeColor = ColorDialog1.Color
            Case "btnInputBackcolor"
                lblInputBackcolor.BackColor = ColorDialog1.Color
            Case "btnAtTo"
                lblAtTo.BackColor = ColorDialog1.Color
            Case "btnListBack"
                lblListBackcolor.BackColor = ColorDialog1.Color
            Case "btnDetailBack"
                lblDetailBackcolor.BackColor = ColorDialog1.Color
            Case "btnDetailLink"
                lblDetailLink.ForeColor = ColorDialog1.Color
        End Select
    End Sub

    Public Property UserstreamPeriodInt() As Integer
        Get
            Return _MyUserstreamPeriod
        End Get
        Set(ByVal value As Integer)
            _MyUserstreamPeriod = value
        End Set
    End Property

    Public Property UserstreamStartup() As Boolean
        Get
            Return Me._MyUserstreamStartup
        End Get
        Set(ByVal value As Boolean)
            Me._MyUserstreamStartup = value
        End Set
    End Property

    Public Property TimelinePeriodInt() As Integer
        Get
            Return _MytimelinePeriod
        End Get
        Set(ByVal value As Integer)
            _MytimelinePeriod = value
        End Set
    End Property

    Public Property ReplyPeriodInt() As Integer
        Get
            Return _MyReplyPeriod
        End Get
        Set(ByVal value As Integer)
            _MyReplyPeriod = value
        End Set
    End Property

    Public Property DMPeriodInt() As Integer
        Get
            Return _MyDMPeriod
        End Get
        Set(ByVal value As Integer)
            _MyDMPeriod = value
        End Set
    End Property

    Public Property PubSearchPeriodInt() As Integer
        Get
            Return _MyPubSearchPeriod
        End Get
        Set(ByVal value As Integer)
            _MyPubSearchPeriod = value
        End Set
    End Property

    Public Property ListsPeriodInt() As Integer
        Get
            Return _MyListsPeriod
        End Get
        Set(ByVal value As Integer)
            _MyListsPeriod = value
        End Set
    End Property

    Public Property UserTimelinePeriodInt() As Integer
        Get
            Return _MyUserTimelinePeriod
        End Get
        Set(ByVal value As Integer)
            _MyUserTimelinePeriod = value
        End Set
    End Property

    Public Property Readed() As Boolean
        Get
            Return _MyReaded
        End Get
        Set(ByVal value As Boolean)
            _MyReaded = value
        End Set
    End Property

    Public Property IconSz() As IconSizes
        Get
            Return _MyIconSize
        End Get
        Set(ByVal value As IconSizes)
            _MyIconSize = value
        End Set
    End Property

    Public Property Status() As String
        Get
            Return _MyStatusText
        End Get
        Set(ByVal value As String)
            _MyStatusText = value
        End Set
    End Property

    Public Property UnreadManage() As Boolean
        Get
            Return _MyUnreadManage
        End Get
        Set(ByVal value As Boolean)
            _MyUnreadManage = value
        End Set
    End Property

    Public Property PlaySound() As Boolean
        Get
            Return _MyPlaySound
        End Get
        Set(ByVal value As Boolean)
            _MyPlaySound = value
        End Set
    End Property

    Public Property OneWayLove() As Boolean
        Get
            Return _MyOneWayLove
        End Get
        Set(ByVal value As Boolean)
            _MyOneWayLove = value
        End Set
    End Property

    '''''未使用
    Public Property FontUnread() As Font
        Get
            Return _fntUnread
        End Get
        Set(ByVal value As Font)
            _fntUnread = value
            '無視
        End Set
    End Property

    Public Property ColorUnread() As Color
        Get
            Return _clUnread
        End Get
        Set(ByVal value As Color)
            _clUnread = value
        End Set
    End Property

    '''''リストフォントとして使用
    Public Property FontReaded() As Font
        Get
            Return _fntReaded
        End Get
        Set(ByVal value As Font)
            _fntReaded = value
        End Set
    End Property

    Public Property ColorReaded() As Color
        Get
            Return _clReaded
        End Get
        Set(ByVal value As Color)
            _clReaded = value
        End Set
    End Property

    Public Property ColorFav() As Color
        Get
            Return _clFav
        End Get
        Set(ByVal value As Color)
            _clFav = value
        End Set
    End Property

    Public Property ColorOWL() As Color
        Get
            Return _clOWL
        End Get
        Set(ByVal value As Color)
            _clOWL = value
        End Set
    End Property

    Public Property ColorRetweet() As Color
        Get
            Return _clRetweet
        End Get
        Set(ByVal value As Color)
            _clRetweet = value
        End Set
    End Property

    Public Property FontDetail() As Font
        Get
            Return _fntDetail
        End Get
        Set(ByVal value As Font)
            _fntDetail = value
        End Set
    End Property

    Public Property ColorDetail() As Color
        Get
            Return _clDetail
        End Get
        Set(ByVal value As Color)
            _clDetail = value
        End Set
    End Property

    Public Property ColorDetailLink() As Color
        Get
            Return _clDetailLink
        End Get
        Set(ByVal value As Color)
            _clDetailLink = value
        End Set
    End Property

    Public Property ColorSelf() As Color
        Get
            Return _clSelf
        End Get
        Set(ByVal value As Color)
            _clSelf = value
        End Set
    End Property

    Public Property ColorAtSelf() As Color
        Get
            Return _clAtSelf
        End Get
        Set(ByVal value As Color)
            _clAtSelf = value
        End Set
    End Property

    Public Property ColorTarget() As Color
        Get
            Return _clTarget
        End Get
        Set(ByVal value As Color)
            _clTarget = value
        End Set
    End Property

    Public Property ColorAtTarget() As Color
        Get
            Return _clAtTarget
        End Get
        Set(ByVal value As Color)
            _clAtTarget = value
        End Set
    End Property

    Public Property ColorAtFromTarget() As Color
        Get
            Return _clAtFromTarget
        End Get
        Set(ByVal value As Color)
            _clAtFromTarget = value
        End Set
    End Property

    Public Property ColorAtTo() As Color
        Get
            Return _clAtTo
        End Get
        Set(ByVal value As Color)
            _clAtTo = value
        End Set
    End Property

    Public Property ColorInputBackcolor() As Color
        Get
            Return _clInputBackcolor
        End Get
        Set(ByVal value As Color)
            _clInputBackcolor = value
        End Set
    End Property

    Public Property ColorInputFont() As Color
        Get
            Return _clInputFont
        End Get
        Set(ByVal value As Color)
            _clInputFont = value
        End Set
    End Property

    Public Property FontInputFont() As Font
        Get
            Return _fntInputFont
        End Get
        Set(ByVal value As Font)
            _fntInputFont = value
        End Set
    End Property

    Public Property ColorListBackcolor() As Color
        Get
            Return _clListBackcolor
        End Get
        Set(ByVal value As Color)
            _clListBackcolor = value
        End Set
    End Property

    Public Property ColorDetailBackcolor() As Color
        Get
            Return _clDetailBackcolor
        End Get
        Set(ByVal value As Color)
            _clDetailBackcolor = value
        End Set
    End Property

    Public Property NameBalloon() As NameBalloonEnum
        Get
            Return _MyNameBalloon
        End Get
        Set(ByVal value As NameBalloonEnum)
            _MyNameBalloon = value
        End Set
    End Property

    Public Property PostCtrlEnter() As Boolean
        Get
            Return _MyPostCtrlEnter
        End Get
        Set(ByVal value As Boolean)
            _MyPostCtrlEnter = value
        End Set
    End Property

    Public Property PostShiftEnter() As Boolean
        Get
            Return _MyPostShiftEnter
        End Get
        Set(ByVal value As Boolean)
            _MyPostShiftEnter = value
        End Set
    End Property

    Public Property CountApi() As Integer
        Get
            Return _countApi
        End Get
        Set(ByVal value As Integer)
            _countApi = value
        End Set
    End Property

    Public Property CountApiReply() As Integer
        Get
            Return _countApiReply
        End Get
        Set(ByVal value As Integer)
            _countApiReply = value
        End Set
    End Property

    Public Property MoreCountApi() As Integer
        Get
            Return _MoreCountApi
        End Get
        Set(ByVal value As Integer)
            _MoreCountApi = value
        End Set
    End Property

    Public Property FirstCountApi() As Integer
        Get
            Return _FirstCountApi
        End Get
        Set(ByVal value As Integer)
            _FirstCountApi = value
        End Set
    End Property

    Public Property SearchCountApi() As Integer
        Get
            Return _SearchCountApi
        End Get
        Set(ByVal value As Integer)
            _SearchCountApi = value
        End Set
    End Property

    Public Property FavoritesCountApi() As Integer
        Get
            Return _FavoritesCountApi
        End Get
        Set(ByVal value As Integer)
            _FavoritesCountApi = value
        End Set
    End Property

    Public Property UserTimelineCountApi() As Integer
        Get
            Return _UserTimelineCountApi
        End Get
        Set(ByVal value As Integer)
            _UserTimelineCountApi = value
        End Set
    End Property

    Public Property ListCountApi() As Integer
        Get
            Return _ListCountApi
        End Get
        Set(ByVal value As Integer)
            _ListCountApi = value
        End Set
    End Property

    Public Property PostAndGet() As Boolean
        Get
            Return _MyPostAndGet
        End Get
        Set(ByVal value As Boolean)
            _MyPostAndGet = value
        End Set
    End Property

    Public Property UseRecommendStatus() As Boolean
        Get
            Return _MyUseRecommendStatus
        End Get
        Set(ByVal value As Boolean)
            _MyUseRecommendStatus = value
        End Set
    End Property

    Public Property RecommendStatusText() As String
        Get
            Return _MyRecommendStatusText
        End Get
        Set(ByVal value As String)
            _MyRecommendStatusText = value
        End Set
    End Property

    Public Property DispUsername() As Boolean
        Get
            Return _MyDispUsername
        End Get
        Set(ByVal value As Boolean)
            _MyDispUsername = value
        End Set
    End Property

    Public Property CloseToExit() As Boolean
        Get
            Return _MyCloseToExit
        End Get
        Set(ByVal value As Boolean)
            _MyCloseToExit = value
        End Set
    End Property

    Public Property MinimizeToTray() As Boolean
        Get
            Return _MyMinimizeToTray
        End Get
        Set(ByVal value As Boolean)
            _MyMinimizeToTray = value
        End Set
    End Property

    Public Property DispLatestPost() As DispTitleEnum
        Get
            Return _MyDispLatestPost
        End Get
        Set(ByVal value As DispTitleEnum)
            _MyDispLatestPost = value
        End Set
    End Property

    Public Property BrowserPath() As String
        Get
            Return _browserpath
        End Get
        Set(ByVal value As String)
            _browserpath = value
        End Set
    End Property

    Public Property TinyUrlResolve() As Boolean
        Get
            Return _MyTinyUrlResolve
        End Get
        Set(ByVal value As Boolean)
            _MyTinyUrlResolve = value
        End Set
    End Property

    Public Property ShortUrlForceResolve() As Boolean
        Get
            Return _MyShortUrlForceResolve
        End Get
        Set(ByVal value As Boolean)
            _MyShortUrlForceResolve = value
        End Set
    End Property

    Private Sub CheckUseRecommendStatus_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckUseRecommendStatus.CheckedChanged
        If CheckUseRecommendStatus.Checked = True Then
            StatusText.Enabled = False
        Else
            StatusText.Enabled = True
        End If
    End Sub

    Public Property SortOrderLock() As Boolean
        Get
            Return _MySortOrderLock
        End Get
        Set(ByVal value As Boolean)
            _MySortOrderLock = value
        End Set
    End Property

    Public Property SelectedProxyType() As HttpConnection.ProxyType
        Get
            Return _MyProxyType
        End Get
        Set(ByVal value As HttpConnection.ProxyType)
            _MyProxyType = value
        End Set
    End Property

    Public Property ProxyAddress() As String
        Get
            Return _MyProxyAddress
        End Get
        Set(ByVal value As String)
            _MyProxyAddress = value
        End Set
    End Property

    Public Property ProxyPort() As Integer
        Get
            Return _MyProxyPort
        End Get
        Set(ByVal value As Integer)
            _MyProxyPort = value
        End Set
    End Property

    Public Property ProxyUser() As String
        Get
            Return _MyProxyUser
        End Get
        Set(ByVal value As String)
            _MyProxyUser = value
        End Set
    End Property

    Public Property ProxyPassword() As String
        Get
            Return _MyProxyPassword
        End Get
        Set(ByVal value As String)
            _MyProxyPassword = value
        End Set
    End Property

    Public Property PeriodAdjust() As Boolean
        Get
            Return _MyPeriodAdjust
        End Get
        Set(ByVal value As Boolean)
            _MyPeriodAdjust = value
        End Set
    End Property

    Public Property StartupVersion() As Boolean
        Get
            Return _MyStartupVersion
        End Get
        Set(ByVal value As Boolean)
            _MyStartupVersion = value
        End Set
    End Property

    Public Property StartupFollowers() As Boolean
        Get
            Return _MyStartupFollowers
        End Get
        Set(ByVal value As Boolean)
            _MyStartupFollowers = value
        End Set
    End Property

    Public Property RestrictFavCheck() As Boolean
        Get
            Return _MyRestrictFavCheck
        End Get
        Set(ByVal value As Boolean)
            _MyRestrictFavCheck = value
        End Set
    End Property

    Public Property AlwaysTop() As Boolean
        Get
            Return _MyAlwaysTop
        End Get
        Set(ByVal value As Boolean)
            _MyAlwaysTop = value
        End Set
    End Property

    Public Property UrlConvertAuto() As Boolean
        Get
            Return _MyUrlConvertAuto
        End Get
        Set(ByVal value As Boolean)
            _MyUrlConvertAuto = value
        End Set
    End Property
    Public Property OutputzEnabled() As Boolean
        Get
            Return _MyOutputz
        End Get
        Set(ByVal value As Boolean)
            _MyOutputz = value
        End Set
    End Property
    Public Property OutputzKey() As String
        Get
            Return _MyOutputzKey
        End Get
        Set(ByVal value As String)
            _MyOutputzKey = value
        End Set
    End Property
    Public Property OutputzUrlmode() As OutputzUrlmode
        Get
            Return _MyOutputzUrlmode
        End Get
        Set(ByVal value As OutputzUrlmode)
            _MyOutputzUrlmode = value
        End Set
    End Property

    Public Property Nicoms() As Boolean
        Get
            Return _MyNicoms
        End Get
        Set(ByVal value As Boolean)
            _MyNicoms = value
        End Set
    End Property
    Public Property AutoShortUrlFirst() As UrlConverter
        Get
            Return _MyAutoShortUrlFirst
        End Get
        Set(ByVal value As UrlConverter)
            _MyAutoShortUrlFirst = value
        End Set
    End Property

    Public Property UseUnreadStyle() As Boolean
        Get
            Return _MyUnreadStyle
        End Get
        Set(ByVal value As Boolean)
            _MyUnreadStyle = value
        End Set
    End Property

    Public Property DateTimeFormat() As String
        Get
            Return _MyDateTimeFormat
        End Get
        Set(ByVal value As String)
            _MyDateTimeFormat = value
        End Set
    End Property

    Public Property DefaultTimeOut() As Integer
        Get
            Return _MyDefaultTimeOut
        End Get
        Set(ByVal value As Integer)
            _MyDefaultTimeOut = value
        End Set
    End Property

    Public Property RetweetNoConfirm() As Boolean
        Get
            Return _MyRetweetNoConfirm
        End Get
        Set(ByVal value As Boolean)
            _MyRetweetNoConfirm = value
        End Set
    End Property

    Public Property TabIconDisp() As Boolean
        Get
            Return _MyTabIconDisp
        End Get
        Set(ByVal value As Boolean)
            _MyTabIconDisp = value
        End Set
    End Property

    Public Property ReplyIconState() As REPLY_ICONSTATE
        Get
            Return _MyReplyIconState
        End Get
        Set(ByVal value As REPLY_ICONSTATE)
            _MyReplyIconState = value
        End Set
    End Property

    Public Property ReadOwnPost() As Boolean
        Get
            Return _MyReadOwnPost
        End Get
        Set(ByVal value As Boolean)
            _MyReadOwnPost = value
        End Set
    End Property

    Public Property GetFav() As Boolean
        Get
            Return _MyGetFav
        End Get
        Set(ByVal value As Boolean)
            _MyGetFav = value
        End Set
    End Property

    Public Property IsMonospace() As Boolean
        Get
            Return _MyMonoSpace
        End Get
        Set(ByVal value As Boolean)
            _MyMonoSpace = value
        End Set
    End Property

    Public Property ReadOldPosts() As Boolean
        Get
            Return _MyReadOldPosts
        End Get
        Set(ByVal value As Boolean)
            _MyReadOldPosts = value
        End Set
    End Property

    Public Property UseSsl() As Boolean
        Get
            Return _MyUseSsl
        End Get
        Set(ByVal value As Boolean)
            _MyUseSsl = value
        End Set
    End Property

    Public Property BitlyUser() As String
        Get
            Return _MyBitlyId
        End Get
        Set(ByVal value As String)
            _MyBitlyId = value
        End Set
    End Property

    Public Property BitlyPwd() As String
        Get
            Return _MyBitlyPw
        End Get
        Set(ByVal value As String)
            _MyBitlyPw = value
        End Set
    End Property

    Public Property ShowGrid() As Boolean
        Get
            Return _MyShowGrid
        End Get
        Set(ByVal value As Boolean)
            _MyShowGrid = value
        End Set
    End Property

    Public Property UseAtIdSupplement() As Boolean
        Get
            Return _MyUseAtIdSupplement
        End Get
        Set(ByVal value As Boolean)
            _MyUseAtIdSupplement = value
        End Set
    End Property

    Public Property UseHashSupplement() As Boolean
        Get
            Return _MyUseHashSupplement
        End Get
        Set(ByVal value As Boolean)
            _MyUseHashSupplement = value
        End Set
    End Property

    Public Property PreviewEnable() As Boolean
        Get
            Return _MyPreviewEnable
        End Get
        Set(ByVal value As Boolean)
            _MyPreviewEnable = value
        End Set
    End Property

    Public Property UseAdditionalCount() As Boolean
        Get
            Return _MyUseAdditonalCount
        End Get
        Set(ByVal value As Boolean)
            _MyUseAdditonalCount = value
        End Set
    End Property

    Public Property OpenUserTimeline() As Boolean
        Set(ByVal value As Boolean)
            _MyOpenUserTimeline = value
        End Set
        Get
            Return _MyOpenUserTimeline
        End Get
    End Property

    Public Property TwitterApiUrl() As String
        Get
            Return _MyTwitterApiUrl
        End Get
        Set(ByVal value As String)
            _MyTwitterApiUrl = value
        End Set
    End Property

    Public Property TwitterSearchApiUrl() As String
        Get
            Return _MyTwitterSearchApiUrl
        End Get
        Set(ByVal value As String)
            _MyTwitterSearchApiUrl = value
        End Set
    End Property

    Public Property Language() As String
        Get
            Return _MyLanguage
        End Get
        Set(ByVal value As String)
            _MyLanguage = value
        End Set
    End Property

    Public Property IsOAuth() As Boolean
        Get
            Return _MyIsOAuth
        End Get
        Set(ByVal value As Boolean)
            _MyIsOAuth = value
        End Set
    End Property

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Using filedlg As New OpenFileDialog()

            filedlg.Filter = My.Resources.Button3_ClickText1
            filedlg.FilterIndex = 1
            filedlg.Title = My.Resources.Button3_ClickText2
            filedlg.RestoreDirectory = True

            If filedlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
                BrowserPathText.Text = filedlg.FileName
            End If
        End Using
    End Sub

    Private Sub RadioProxySpecified_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioProxySpecified.CheckedChanged
        Dim chk As Boolean = RadioProxySpecified.Checked
        LabelProxyAddress.Enabled = chk
        TextProxyAddress.Enabled = chk
        LabelProxyPort.Enabled = chk
        TextProxyPort.Enabled = chk
        LabelProxyUser.Enabled = chk
        TextProxyUser.Enabled = chk
        LabelProxyPassword.Enabled = chk
        TextProxyPassword.Enabled = chk
    End Sub

    Private Sub TextProxyPort_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TextProxyPort.Validating
        Dim port As Integer
        If TextProxyPort.Text.Trim() = "" Then TextProxyPort.Text = "0"
        If Integer.TryParse(TextProxyPort.Text.Trim(), port) = False Then
            MessageBox.Show(My.Resources.TextProxyPort_ValidatingText1)
            e.Cancel = True
            Exit Sub
        End If
        If port < 0 Or port > 65535 Then
            MessageBox.Show(My.Resources.TextProxyPort_ValidatingText2)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub CheckOutputz_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckOutputz.CheckedChanged
        If CheckOutputz.Checked = True Then
            Label59.Enabled = True
            Label60.Enabled = True
            TextBoxOutputzKey.Enabled = True
            ComboBoxOutputzUrlmode.Enabled = True
        Else
            Label59.Enabled = False
            Label60.Enabled = False
            TextBoxOutputzKey.Enabled = False
            ComboBoxOutputzUrlmode.Enabled = False
        End If
    End Sub

    Private Sub TextBoxOutputzKey_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TextBoxOutputzKey.Validating
        If CheckOutputz.Checked Then
            TextBoxOutputzKey.Text = Trim(TextBoxOutputzKey.Text)
            If TextBoxOutputzKey.Text.Length = 0 Then
                MessageBox.Show(My.Resources.TextBoxOutputzKey_Validating)
                e.Cancel = True
                Exit Sub
            End If
        End If
    End Sub

    Private Function CreateDateTimeFormatSample() As Boolean
        Try
            LabelDateTimeFormatApplied.Text = DateTime.Now.ToString(CmbDateTimeFormat.Text)
        Catch ex As FormatException
            LabelDateTimeFormatApplied.Text = My.Resources.CreateDateTimeFormatSampleText1
            Return False
        End Try
        Return True
    End Function

    Private Sub CmbDateTimeFormat_TextUpdate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbDateTimeFormat.TextUpdate
        CreateDateTimeFormatSample()
    End Sub

    Private Sub CmbDateTimeFormat_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbDateTimeFormat.SelectedIndexChanged
        CreateDateTimeFormatSample()
    End Sub

    Private Sub CmbDateTimeFormat_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles CmbDateTimeFormat.Validating
        If Not CreateDateTimeFormatSample() Then
            MessageBox.Show(My.Resources.CmbDateTimeFormat_Validating)
            e.Cancel = True
        End If
    End Sub

    Private Sub ConnectionTimeOut_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ConnectionTimeOut.Validating
        Dim tm As Integer
        Try
            tm = CInt(ConnectionTimeOut.Text)
        Catch ex As Exception
            MessageBox.Show(My.Resources.ConnectionTimeOut_ValidatingText1)
            e.Cancel = True
            Exit Sub
        End Try

        If tm < HttpTimeOut.MinValue OrElse tm > HttpTimeOut.MaxValue Then
            MessageBox.Show(My.Resources.ConnectionTimeOut_ValidatingText1)
            e.Cancel = True
        End If
    End Sub

    Private Sub LabelDateTimeFormatApplied_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles LabelDateTimeFormatApplied.VisibleChanged
        CreateDateTimeFormatSample()
    End Sub

    Private Sub TextCountApi_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TextCountApi.Validating
        Dim cnt As Integer
        Try
            cnt = Integer.Parse(TextCountApi.Text)
        Catch ex As Exception
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End Try

        If cnt < 20 OrElse cnt > 200 Then
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub TextCountApiReply_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TextCountApiReply.Validating
        Dim cnt As Integer
        Try
            cnt = Integer.Parse(TextCountApiReply.Text)
        Catch ex As Exception
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End Try

        If cnt < 20 OrElse cnt > 200 Then
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Public Property LimitBalloon() As Boolean
        Get
            Return _MyLimitBalloon
        End Get
        Set(ByVal value As Boolean)
            _MyLimitBalloon = value
        End Set
    End Property

    Public Property EventNotifyEnabled As Boolean
        Get
            Return _MyEventNotifyEnabled
        End Get
        Set(ByVal value As Boolean)
            _MyEventNotifyEnabled = value
        End Set
    End Property

    Public Property EventNotifyFlag As EVENTTYPE
        Get
            Return _MyEventNotifyFlag
        End Get
        Set(ByVal value As EVENTTYPE)
            _MyEventNotifyFlag = value
        End Set
    End Property

    Public Property IsMyEventNotifyFlag As EVENTTYPE
        Get
            Return _isMyEventNotifyFlag
        End Get
        Set(ByVal value As EVENTTYPE)
            _isMyEventNotifyFlag = value
        End Set
    End Property

    Public Property ForceEventNotify As Boolean
        Get
            Return _MyForceEventNotify
        End Get
        Set(ByVal value As Boolean)
            _MyForceEventNotify = value
        End Set
    End Property

    Public Property FavEventUnread As Boolean
        Get
            Return _MyFavEventUnread
        End Get
        Set(ByVal value As Boolean)
            _MyFavEventUnread = value
        End Set
    End Property

    Public Property TranslateLanguage As String
        Get
            Return _MyTranslateLanguage
        End Get
        Set(ByVal value As String)
            _MyTranslateLanguage = value
            ComboBoxTranslateLanguage.SelectedIndex = (New Google).GetIndexFromLanguageEnum(value)
        End Set
    End Property

    Public Property EventSoundFile As String
        Get
            Return _MyEventSoundFile
        End Get
        Set(ByVal value As String)
            _MyEventSoundFile = value
        End Set
    End Property

    Public Property ListDoubleClickAction As Integer
        Get
            Return _MyDoubleClickAction
        End Get
        Set(ByVal value As Integer)
            _MyDoubleClickAction = value
        End Set
    End Property

    Public Property UserAppointUrl As String
        Get
            Return _UserAppointUrl
        End Get
        Set(ByVal value As String)
            _UserAppointUrl = value
        End Set
    End Property

    Private Sub ComboBoxAutoShortUrlFirst_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBoxAutoShortUrlFirst.SelectedIndexChanged
        If ComboBoxAutoShortUrlFirst.SelectedIndex = UrlConverter.Bitly OrElse _
           ComboBoxAutoShortUrlFirst.SelectedIndex = UrlConverter.Jmp Then
            TextBitlyId.Enabled = True
            TextBitlyPw.Enabled = True
        Else
            TextBitlyId.Enabled = False
            TextBitlyPw.Enabled = False
        End If
    End Sub

    Private Sub ButtonBackToDefaultFontColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonBackToDefaultFontColor.Click, ButtonBackToDefaultFontColor2.Click
        lblUnread.ForeColor = System.Drawing.SystemColors.ControlText
        lblUnread.Font = New Font(SystemFonts.DefaultFont, FontStyle.Bold Or FontStyle.Underline)

        lblListFont.ForeColor = System.Drawing.SystemColors.ControlText
        lblListFont.Font = System.Drawing.SystemFonts.DefaultFont

        lblDetail.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.ControlText)
        lblDetail.Font = System.Drawing.SystemFonts.DefaultFont

        lblInputFont.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.ControlText)
        lblInputFont.Font = System.Drawing.SystemFonts.DefaultFont

        lblSelf.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.AliceBlue)

        lblAtSelf.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.AntiqueWhite)

        lblTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon)

        lblAtTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LavenderBlush)

        lblAtFromTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Honeydew)

        lblFav.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Red)

        lblOWL.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Blue)

        lblInputBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon)

        lblAtTo.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Pink)

        lblListBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window)

        lblDetailBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window)

        lblDetailLink.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Blue)

        lblRetweet.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Green)
    End Sub

    Private Function Authorize() As Boolean
        Dim user As String = Me.Username.Text.Trim
        Dim pwd As String = Me.Password.Text.Trim
        If String.IsNullOrEmpty(user) OrElse String.IsNullOrEmpty(pwd) Then
            MessageBox.Show(My.Resources.Save_ClickText1)
            Return False
        End If

        '現在の設定内容で通信
        Dim ptype As HttpConnection.ProxyType
        If RadioProxyNone.Checked Then
            ptype = HttpConnection.ProxyType.None
        ElseIf RadioProxyIE.Checked Then
            ptype = HttpConnection.ProxyType.IE
        Else
            ptype = HttpConnection.ProxyType.Specified
        End If
        Dim padr As String = TextProxyAddress.Text.Trim()
        Dim pport As Integer = Integer.Parse(TextProxyPort.Text.Trim())
        Dim pusr As String = TextProxyUser.Text.Trim()
        Dim ppw As String = TextProxyPassword.Text.Trim()

        '通信基底クラス初期化
        HttpConnection.InitializeConnection(20, ptype, padr, pport, pusr, ppw)
        HttpTwitter.TwitterUrl = TwitterAPIText.Text.Trim
        HttpTwitter.TwitterSearchUrl = TwitterSearchAPIText.Text.Trim
        If Me.AuthBasicRadio.Checked Then
            tw.Initialize("", "")
        Else
            tw.Initialize("", "", "")
        End If
        Dim rslt As String = tw.Authenticate(user, pwd)
        If String.IsNullOrEmpty(rslt) Then
            MessageBox.Show(My.Resources.AuthorizeButton_Click1, "Authenticate", MessageBoxButtons.OK)
            Me.AuthStateLabel.Text = My.Resources.AuthorizeButton_Click3
            Me.AuthUserLabel.Text = tw.Username
            Return True
        Else
            MessageBox.Show(My.Resources.AuthorizeButton_Click2 + Environment.NewLine + rslt, "Authenticate", MessageBoxButtons.OK)
            Me.AuthStateLabel.Text = My.Resources.AuthorizeButton_Click4
            Me.AuthUserLabel.Text = ""
            Return False
        End If
    End Function

    Private Sub AuthorizeButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AuthorizeButton.Click
        If Authorize() Then CalcApiUsing()
    End Sub

    Private Sub AuthClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AuthClearButton.Click
        tw.ClearAuthInfo()
        Me.AuthStateLabel.Text = My.Resources.AuthorizeButton_Click4
        Me.AuthUserLabel.Text = ""
        CalcApiUsing()
    End Sub

    Private Sub AuthOAuthRadio_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AuthOAuthRadio.CheckedChanged
        If tw Is Nothing Then Exit Sub
        If AuthBasicRadio.Checked Then
            'BASIC認証時のみ表示
            tw.Initialize("", "")
            Me.AuthStateLabel.Enabled = False
            Me.AuthUserLabel.Enabled = False
            Me.AuthClearButton.Enabled = False
        Else
            tw.Initialize("", "", "")
            Me.AuthStateLabel.Enabled = True
            Me.AuthUserLabel.Enabled = True
            Me.AuthClearButton.Enabled = True
        End If
        Me.AuthStateLabel.Text = My.Resources.AuthorizeButton_Click4
        Me.AuthUserLabel.Text = ""
        CalcApiUsing()
    End Sub

    Private Sub DisplayApiMaxCount()
        If TwitterApiInfo.MaxCount > -1 Then
            LabelApiUsing.Text = String.Format(My.Resources.SettingAPIUse1, TwitterApiInfo.UsingCount, TwitterApiInfo.MaxCount)
        Else
            LabelApiUsing.Text = String.Format(My.Resources.SettingAPIUse1, TwitterApiInfo.UsingCount, "???")
        End If
    End Sub

    Private Sub CalcApiUsing()
        Dim UsingApi As Integer = 0
        Dim tmp As Integer
        Dim ListsTabNum As Integer = 0
        Dim UserTimelineTabNum As Integer = 0
        Dim ApiLists As Integer = 0
        Dim ApiUserTimeline As Integer = 0
        Dim UsingApiUserStream As Integer = 0

        Try
            ' 初回起動時などにNothingの場合あり
            ListsTabNum = TabInformations.GetInstance.GetTabsByType(TabUsageType.Lists).Count
        Catch ex As Exception
            Exit Sub
        End Try

        Try
            ' 初回起動時などにNothingの場合あり
            UserTimelineTabNum = TabInformations.GetInstance.GetTabsByType(TabUsageType.UserTimeline).Count
        Catch ex As Exception
            Exit Sub
        End Try

        ' Recent計算 0は手動更新
        If Integer.TryParse(TimelinePeriod.Text, tmp) Then
            If tmp <> 0 Then
                UsingApi += 3600 \ tmp
            End If
        End If

        ' Reply計算 0は手動更新
        If Integer.TryParse(ReplyPeriod.Text, tmp) Then
            If tmp <> 0 Then
                UsingApi += 3600 \ tmp
            End If
        End If

        ' DM計算 0は手動更新 送受信両方
        If Integer.TryParse(DMPeriod.Text, tmp) Then
            If tmp <> 0 Then
                UsingApi += (3600 \ tmp) * 2
            End If
        End If

        ' Listsタブ計算 0は手動更新
        If Integer.TryParse(ListsPeriod.Text, tmp) Then
            If tmp <> 0 Then
                ApiLists = (3600 \ tmp) * ListsTabNum
                UsingApi += ApiLists
            End If
        End If

        ' UserTimelineタブ計算 0は手動更新
        If Integer.TryParse(UserTimelinePeriod.Text, tmp) Then
            If tmp <> 0 Then
                ApiUserTimeline = (3600 \ tmp) * UserTimelineTabNum
                UsingApi += ApiUserTimeline
            End If
        End If

        If tw IsNot Nothing Then
            If TwitterApiInfo.MaxCount = -1 Then
                If Twitter.AccountState = ACCOUNT_STATE.Valid Then
                    TwitterApiInfo.UsingCount = UsingApi
                    Dim proc As New Thread(New Threading.ThreadStart(Sub()
                                                                         tw.GetInfoApi(Nothing) '取得エラー時はinfoCountは初期状態（値：-1）
                                                                         If Me.IsHandleCreated AndAlso Not Me.IsDisposed Then Invoke(New MethodInvoker(AddressOf DisplayApiMaxCount))
                                                                     End Sub))
                    proc.Start()
                Else
                    LabelApiUsing.Text = String.Format(My.Resources.SettingAPIUse1, UsingApi, "???")
                End If
            Else
                LabelApiUsing.Text = String.Format(My.Resources.SettingAPIUse1, UsingApi, TwitterApiInfo.MaxCount)
            End If
        End If


        LabelPostAndGet.Visible = CheckPostAndGet.Checked AndAlso Not tw.UserStreamEnabled
        LabelUserStreamActive.Visible = tw.UserStreamEnabled

        LabelApiUsingUserStreamEnabled.Text = String.Format(My.Resources.SettingAPIUse2, (ApiLists + ApiUserTimeline).ToString)
        LabelApiUsingUserStreamEnabled.Visible = tw.UserStreamEnabled
    End Sub

    Private Sub CheckPostAndGet_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckPostAndGet.CheckedChanged
        CalcApiUsing()
    End Sub

    Private Sub Setting_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Do
            Thread.Sleep(10)
            If Me.Disposing OrElse Me.IsDisposed Then Exit Sub
        Loop Until Me.IsHandleCreated
        CalcApiUsing()
    End Sub

    Private Sub ButtonApiCalc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonApiCalc.Click
        CalcApiUsing()
    End Sub

    Private Sub New()

        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

    End Sub

    Public Shared ReadOnly Property Instance As AppendSettingDialog
        Get
            Return _instance
        End Get
    End Property

    Private Function BitlyValidation(ByVal id As String, ByVal apikey As String) As Boolean
        If String.IsNullOrEmpty(id) OrElse String.IsNullOrEmpty(apikey) Then
            Return False
        End If

        Dim req As String = "http://api.bit.ly/v3/validate"
        Dim content As String = ""
        Dim param As New Dictionary(Of String, String)

        param.Add("login", "tweenapi")
        param.Add("apiKey", "R_c5ee0e30bdfff88723c4457cc331886b")
        param.Add("x_login", id)
        param.Add("x_apiKey", apikey)
        param.Add("format", "txt")

        If Not (New HttpVarious).PostData(req, param, content) Then
            Return True             ' 通信エラーの場合はとりあえずチェックを通ったことにする
        ElseIf content.Trim() = "1" Then
            Return True             ' 検証成功
        ElseIf content.Trim() = "0" Then
            Return False            ' 検証失敗 APIキーとIDの組み合わせが違う
        Else
            Return True             ' 規定外応答：通信エラーの可能性があるためとりあえずチェックを通ったことにする
        End If
    End Function

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        _ValidationError = False
    End Sub

    Public Property HotkeyEnabled As Boolean
    Public Property HotkeyKey As Keys
    Public Property HotkeyValue As Integer
    Public Property HotkeyMod As Keys

    Private Sub HotkeyText_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles HotkeyText.KeyDown
        'KeyValueで判定する。
        '表示文字とのテーブルを用意すること
        HotkeyText.Text = e.KeyCode.ToString
        HotkeyCode.Text = e.KeyValue.ToString
        HotkeyText.Tag = e.KeyCode
        e.Handled = True
        e.SuppressKeyPress = True
    End Sub

    Private Sub HotkeyCheck_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HotkeyCheck.CheckedChanged
        HotkeyCtrl.Enabled = HotkeyCheck.Checked
        HotkeyAlt.Enabled = HotkeyCheck.Checked
        HotkeyShift.Enabled = HotkeyCheck.Checked
        HotkeyWin.Enabled = HotkeyCheck.Checked
        HotkeyText.Enabled = HotkeyCheck.Checked
        HotkeyCode.Enabled = HotkeyCheck.Checked
    End Sub

    Public Property BlinkNewMentions As Boolean

    Private Sub GetMoreTextCountApi_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles GetMoreTextCountApi.Validating
        Dim cnt As Integer
        Try
            cnt = Integer.Parse(GetMoreTextCountApi.Text)
        Catch ex As Exception
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End Try

        If Not cnt = 0 AndAlso (cnt < 20 OrElse cnt > 200) Then
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub UseChangeGetCount_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles UseChangeGetCount.CheckedChanged
        GetMoreTextCountApi.Enabled = UseChangeGetCount.Checked
        FirstTextCountApi.Enabled = UseChangeGetCount.Checked
        Label28.Enabled = UseChangeGetCount.Checked
        Label30.Enabled = UseChangeGetCount.Checked
        Label53.Enabled = UseChangeGetCount.Checked
        Label66.Enabled = UseChangeGetCount.Checked
        Label17.Enabled = UseChangeGetCount.Checked
        Label25.Enabled = UseChangeGetCount.Checked
        SearchTextCountApi.Enabled = UseChangeGetCount.Checked
        FavoritesTextCountApi.Enabled = UseChangeGetCount.Checked
        UserTimelineTextCountApi.Enabled = UseChangeGetCount.Checked
        ListTextCountApi.Enabled = UseChangeGetCount.Checked
    End Sub

    Private Sub FirstTextCountApi_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles FirstTextCountApi.Validating
        Dim cnt As Integer
        Try
            cnt = Integer.Parse(FirstTextCountApi.Text)
        Catch ex As Exception
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End Try

        If Not cnt = 0 AndAlso (cnt < 20 OrElse cnt > 200) Then
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub CheckEnableBasicAuth_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckEnableBasicAuth.CheckedChanged
        AuthBasicRadio.Enabled = CheckEnableBasicAuth.Checked
    End Sub

    Private Sub SearchTextCountApi_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles SearchTextCountApi.Validating
        Dim cnt As Integer
        Try
            cnt = Integer.Parse(SearchTextCountApi.Text)
        Catch ex As Exception
            MessageBox.Show(My.Resources.TextSearchCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End Try

        If Not cnt = 0 AndAlso (cnt < 20 OrElse cnt > 100) Then
            MessageBox.Show(My.Resources.TextSearchCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub FavoritesTextCountApi_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles FavoritesTextCountApi.Validating
        Dim cnt As Integer
        Try
            cnt = Integer.Parse(FavoritesTextCountApi.Text)
        Catch ex As Exception
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End Try

        If Not cnt = 0 AndAlso (cnt < 20 OrElse cnt > 200) Then
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub UserTimelineTextCountApi_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles UserTimelineTextCountApi.Validating
        Dim cnt As Integer
        Try
            cnt = Integer.Parse(UserTimelineTextCountApi.Text)
        Catch ex As Exception
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End Try

        If Not cnt = 0 AndAlso (cnt < 20 OrElse cnt > 200) Then
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub ListTextCountApi_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ListTextCountApi.Validating
        Dim cnt As Integer
        Try
            cnt = Integer.Parse(ListTextCountApi.Text)
        Catch ex As Exception
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End Try

        If Not cnt = 0 AndAlso (cnt < 20 OrElse cnt > 200) Then
            MessageBox.Show(My.Resources.TextCountApi_Validating1)
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub CheckEventNotify_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                    Handles CheckEventNotify.CheckedChanged, CheckFavoritesEvent.CheckStateChanged, _
                            CheckUnfavoritesEvent.CheckStateChanged, CheckFollowEvent.CheckStateChanged, _
                            CheckListMemberAddedEvent.CheckStateChanged, CheckListMemberRemovedEvent.CheckStateChanged, _
                            CheckListCreatedEvent.CheckStateChanged, CheckUserUpdateEvent.CheckStateChanged
        _MyEventNotifyEnabled = CheckEventNotify.Checked
        GetEventNotifyFlag(_MyEventNotifyFlag, _isMyEventNotifyFlag)
        ApplyEventNotifyFlag(_MyEventNotifyEnabled, _MyEventNotifyFlag, _isMyEventNotifyFlag)
    End Sub

    Private Class EventCheckboxTblElement
        Public CheckBox As CheckBox
        Public Type As EVENTTYPE
    End Class

    Private Function GetEventCheckboxTable() As EventCheckboxTblElement()

        Static _eventCheckboxTable As EventCheckboxTblElement() = {
            New EventCheckboxTblElement With {.CheckBox = CheckFavoritesEvent, .Type = EVENTTYPE.Favorite},
            New EventCheckboxTblElement With {.CheckBox = CheckUnfavoritesEvent, .Type = EVENTTYPE.Unfavorite},
            New EventCheckboxTblElement With {.CheckBox = CheckFollowEvent, .Type = EVENTTYPE.Follow},
            New EventCheckboxTblElement With {.CheckBox = CheckListMemberAddedEvent, .Type = EVENTTYPE.ListMemberAdded},
            New EventCheckboxTblElement With {.CheckBox = CheckListMemberRemovedEvent, .Type = EVENTTYPE.ListMemberRemoved},
            New EventCheckboxTblElement With {.CheckBox = CheckBlockEvent, .Type = EVENTTYPE.Block},
            New EventCheckboxTblElement With {.CheckBox = CheckUserUpdateEvent, .Type = EVENTTYPE.UserUpdate},
            New EventCheckboxTblElement With {.CheckBox = CheckListCreatedEvent, .Type = EVENTTYPE.ListCreated}
        }

        Return _eventCheckboxTable
    End Function
    
    Private Sub GetEventNotifyFlag(ByRef eventnotifyflag As EVENTTYPE, ByRef isMyeventnotifyflag As EVENTTYPE)
        Dim evt As EVENTTYPE = EVENTTYPE.None
        Dim myevt As EVENTTYPE = EVENTTYPE.None

        For Each tbl As EventCheckboxTblElement In GetEventCheckboxTable()
            Select Case tbl.CheckBox.CheckState
                Case CheckState.Checked
                    evt = evt Or tbl.Type
                    myevt = myevt Or tbl.Type
                Case CheckState.Indeterminate
                    evt = evt Or tbl.Type
                Case CheckState.Unchecked
                    '
            End Select
        Next
        eventnotifyflag = evt
        isMyeventnotifyflag = myevt
    End Sub

    Private Sub ApplyEventNotifyFlag(ByVal rootEnabled As Boolean, ByVal eventnotifyflag As EVENTTYPE, ByVal isMyeventnotifyflag As EVENTTYPE)
        Dim evt = eventnotifyflag
        Dim myevt = isMyeventnotifyflag

        CheckEventNotify.Checked = rootEnabled

        For Each tbl As EventCheckboxTblElement In GetEventCheckboxTable()
            If CBool(evt And tbl.Type) Then
                If CBool(myevt And tbl.Type) Then
                    tbl.CheckBox.CheckState = CheckState.Checked
                Else
                    tbl.CheckBox.CheckState = CheckState.Indeterminate
                End If
            Else
                tbl.CheckBox.CheckState = CheckState.Unchecked
            End If
            tbl.CheckBox.Enabled = rootEnabled
        Next

    End Sub

    Private Sub CheckForceEventNotify_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckForceEventNotify.CheckedChanged
        _MyForceEventNotify = CheckEventNotify.Checked
    End Sub

    Private Sub CheckFavEventUnread_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckFavEventUnread.CheckedChanged
        _MyFavEventUnread = CheckFavEventUnread.Checked
    End Sub

    Private Sub ComboBoxTranslateLanguage_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBoxTranslateLanguage.SelectedIndexChanged
        _MyTranslateLanguage = (New Google).GetLanguageEnumFromIndex(ComboBoxTranslateLanguage.SelectedIndex)
    End Sub

    Private Sub SoundFileListup()
        If _MyEventSoundFile Is Nothing Then Exit Sub
        _soundfileListup = True
        ComboBoxEventNotifySound.Items.Clear()
        ComboBoxEventNotifySound.Items.Add("")
        Dim oDir As IO.DirectoryInfo = New IO.DirectoryInfo(My.Application.Info.DirectoryPath + IO.Path.DirectorySeparatorChar)
        If IO.Directory.Exists(IO.Path.Combine(My.Application.Info.DirectoryPath, "Sounds")) Then
            oDir = oDir.GetDirectories("Sounds")(0)
        End If
        For Each oFile As IO.FileInfo In oDir.GetFiles("*.wav")
            ComboBoxEventNotifySound.Items.Add(oFile.Name)
        Next
        Dim idx As Integer = ComboBoxEventNotifySound.Items.IndexOf(_MyEventSoundFile)
        If idx = -1 Then idx = 0
        ComboBoxEventNotifySound.SelectedIndex = idx
        _soundfileListup = False
    End Sub

    Private Sub ComboBoxEventNotifySound_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBoxEventNotifySound.VisibleChanged
        SoundFileListup()
    End Sub

    Private Sub ComboBoxEventNotifySound_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBoxEventNotifySound.SelectedIndexChanged
        If _soundfileListup Then Exit Sub

        _MyEventSoundFile = DirectCast(ComboBoxEventNotifySound.SelectedItem, String)
    End Sub

    Private Sub UserAppointUrlText_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles UserAppointUrlText.Validating
        If Not UserAppointUrlText.Text.StartsWith("http") AndAlso Not UserAppointUrlText.Text = "" Then
            MessageBox.Show("Text Error:正しいURLではありません")
        End If
    End Sub

End Class