<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AppendSettingDialog
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AppendSettingDialog))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.TreeViewSetting = New System.Windows.Forms.TreeView()
        Me.NotifyPanel = New System.Windows.Forms.Panel()
        Me.CheckUserUpdateEvent = New System.Windows.Forms.CheckBox()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.ComboBoxEventNotifySound = New System.Windows.Forms.ComboBox()
        Me.CheckFavEventUnread = New System.Windows.Forms.CheckBox()
        Me.CheckListCreatedEvent = New System.Windows.Forms.CheckBox()
        Me.CheckBlockEvent = New System.Windows.Forms.CheckBox()
        Me.CheckForceEventNotify = New System.Windows.Forms.CheckBox()
        Me.CheckListMemberRemovedEvent = New System.Windows.Forms.CheckBox()
        Me.CheckListMemberAddedEvent = New System.Windows.Forms.CheckBox()
        Me.CheckFollowEvent = New System.Windows.Forms.CheckBox()
        Me.CheckUnfavoritesEvent = New System.Windows.Forms.CheckBox()
        Me.CheckFavoritesEvent = New System.Windows.Forms.CheckBox()
        Me.CheckEventNotify = New System.Windows.Forms.CheckBox()
        Me.GetPeriodPanel = New System.Windows.Forms.Panel()
        Me.LabelApiUsingUserStreamEnabled = New System.Windows.Forms.Label()
        Me.LabelUserStreamActive = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.UserTimelinePeriod = New System.Windows.Forms.TextBox()
        Me.TimelinePeriod = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ButtonApiCalc = New System.Windows.Forms.Button()
        Me.LabelPostAndGet = New System.Windows.Forms.Label()
        Me.LabelApiUsing = New System.Windows.Forms.Label()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.ListsPeriod = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.PubSearchPeriod = New System.Windows.Forms.TextBox()
        Me.Label69 = New System.Windows.Forms.Label()
        Me.ReplyPeriod = New System.Windows.Forms.TextBox()
        Me.CheckPostAndGet = New System.Windows.Forms.CheckBox()
        Me.CheckPeriodAdjust = New System.Windows.Forms.CheckBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.DMPeriod = New System.Windows.Forms.TextBox()
        Me.GetCountPanel = New System.Windows.Forms.Panel()
        Me.ListTextCountApi = New System.Windows.Forms.TextBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.UserTimelineTextCountApi = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.FavoritesTextCountApi = New System.Windows.Forms.TextBox()
        Me.SearchTextCountApi = New System.Windows.Forms.TextBox()
        Me.Label66 = New System.Windows.Forms.Label()
        Me.FirstTextCountApi = New System.Windows.Forms.TextBox()
        Me.GetMoreTextCountApi = New System.Windows.Forms.TextBox()
        Me.Label53 = New System.Windows.Forms.Label()
        Me.UseChangeGetCount = New System.Windows.Forms.CheckBox()
        Me.TextCountApiReply = New System.Windows.Forms.TextBox()
        Me.Label67 = New System.Windows.Forms.Label()
        Me.TextCountApi = New System.Windows.Forms.TextBox()
        Me.BasedPanel = New System.Windows.Forms.Panel()
        Me.AuthBasicRadio = New System.Windows.Forms.RadioButton()
        Me.AuthOAuthRadio = New System.Windows.Forms.RadioButton()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.AuthClearButton = New System.Windows.Forms.Button()
        Me.AuthUserLabel = New System.Windows.Forms.Label()
        Me.AuthStateLabel = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.AuthorizeButton = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Username = New System.Windows.Forms.TextBox()
        Me.Password = New System.Windows.Forms.TextBox()
        Me.StartupPanel = New System.Windows.Forms.Panel()
        Me.StartupReaded = New System.Windows.Forms.CheckBox()
        Me.CheckStartupFollowers = New System.Windows.Forms.CheckBox()
        Me.CheckStartupVersion = New System.Windows.Forms.CheckBox()
        Me.chkGetFav = New System.Windows.Forms.CheckBox()
        Me.UserStreamPanel = New System.Windows.Forms.Panel()
        Me.UserstreamPeriod = New System.Windows.Forms.TextBox()
        Me.StartupUserstreamCheck = New System.Windows.Forms.CheckBox()
        Me.Label83 = New System.Windows.Forms.Label()
        Me.TweetActPanel = New System.Windows.Forms.Panel()
        Me.CheckHashSupple = New System.Windows.Forms.CheckBox()
        Me.CheckAtIdSupple = New System.Windows.Forms.CheckBox()
        Me.ComboBoxPostKeySelect = New System.Windows.Forms.ComboBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.CheckRetweetNoConfirm = New System.Windows.Forms.CheckBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.CheckUseRecommendStatus = New System.Windows.Forms.CheckBox()
        Me.StatusText = New System.Windows.Forms.TextBox()
        Me.PreviewPanel = New System.Windows.Forms.Panel()
        Me.ReplyIconStateCombo = New System.Windows.Forms.ComboBox()
        Me.Label72 = New System.Windows.Forms.Label()
        Me.ChkNewMentionsBlink = New System.Windows.Forms.CheckBox()
        Me.chkTabIconDisp = New System.Windows.Forms.CheckBox()
        Me.CheckPreviewEnable = New System.Windows.Forms.CheckBox()
        Me.Label81 = New System.Windows.Forms.Label()
        Me.LanguageCombo = New System.Windows.Forms.ComboBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.CheckAlwaysTop = New System.Windows.Forms.CheckBox()
        Me.CheckMonospace = New System.Windows.Forms.CheckBox()
        Me.CheckBalloonLimit = New System.Windows.Forms.CheckBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.ComboDispTitle = New System.Windows.Forms.ComboBox()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.cmbNameBalloon = New System.Windows.Forms.ComboBox()
        Me.CheckDispUsername = New System.Windows.Forms.CheckBox()
        Me.CheckBox3 = New System.Windows.Forms.CheckBox()
        Me.TweetPrvPanel = New System.Windows.Forms.Panel()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.LabelDateTimeFormatApplied = New System.Windows.Forms.Label()
        Me.Label62 = New System.Windows.Forms.Label()
        Me.CmbDateTimeFormat = New System.Windows.Forms.ComboBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.IconSize = New System.Windows.Forms.ComboBox()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.CheckSortOrderLock = New System.Windows.Forms.CheckBox()
        Me.CheckShowGrid = New System.Windows.Forms.CheckBox()
        Me.chkUnreadStyle = New System.Windows.Forms.CheckBox()
        Me.OneWayLv = New System.Windows.Forms.CheckBox()
        Me.FontPanel = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnRetweet = New System.Windows.Forms.Button()
        Me.lblRetweet = New System.Windows.Forms.Label()
        Me.Label80 = New System.Windows.Forms.Label()
        Me.ButtonBackToDefaultFontColor = New System.Windows.Forms.Button()
        Me.btnDetailLink = New System.Windows.Forms.Button()
        Me.lblDetailLink = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.btnUnread = New System.Windows.Forms.Button()
        Me.lblUnread = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.btnDetailBack = New System.Windows.Forms.Button()
        Me.lblDetailBackcolor = New System.Windows.Forms.Label()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.btnDetail = New System.Windows.Forms.Button()
        Me.lblDetail = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.btnOWL = New System.Windows.Forms.Button()
        Me.lblOWL = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.btnFav = New System.Windows.Forms.Button()
        Me.lblFav = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.btnListFont = New System.Windows.Forms.Button()
        Me.lblListFont = New System.Windows.Forms.Label()
        Me.Label61 = New System.Windows.Forms.Label()
        Me.FontPanel2 = New System.Windows.Forms.Panel()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Label65 = New System.Windows.Forms.Label()
        Me.Label52 = New System.Windows.Forms.Label()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.btnInputFont = New System.Windows.Forms.Button()
        Me.btnInputBackcolor = New System.Windows.Forms.Button()
        Me.btnAtTo = New System.Windows.Forms.Button()
        Me.btnListBack = New System.Windows.Forms.Button()
        Me.btnAtFromTarget = New System.Windows.Forms.Button()
        Me.btnAtTarget = New System.Windows.Forms.Button()
        Me.btnTarget = New System.Windows.Forms.Button()
        Me.btnAtSelf = New System.Windows.Forms.Button()
        Me.btnSelf = New System.Windows.Forms.Button()
        Me.lblInputFont = New System.Windows.Forms.Label()
        Me.lblInputBackcolor = New System.Windows.Forms.Label()
        Me.lblAtTo = New System.Windows.Forms.Label()
        Me.lblListBackcolor = New System.Windows.Forms.Label()
        Me.lblAtFromTarget = New System.Windows.Forms.Label()
        Me.lblAtTarget = New System.Windows.Forms.Label()
        Me.lblTarget = New System.Windows.Forms.Label()
        Me.lblAtSelf = New System.Windows.Forms.Label()
        Me.lblSelf = New System.Windows.Forms.Label()
        Me.ButtonBackToDefaultFontColor2 = New System.Windows.Forms.Button()
        Me.ConnectionPanel = New System.Windows.Forms.Panel()
        Me.CheckEnableBasicAuth = New System.Windows.Forms.CheckBox()
        Me.TwitterSearchAPIText = New System.Windows.Forms.TextBox()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.TwitterAPIText = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.CheckUseSsl = New System.Windows.Forms.CheckBox()
        Me.Label64 = New System.Windows.Forms.Label()
        Me.ConnectionTimeOut = New System.Windows.Forms.TextBox()
        Me.Label63 = New System.Windows.Forms.Label()
        Me.ProxyPanel = New System.Windows.Forms.Panel()
        Me.Label55 = New System.Windows.Forms.Label()
        Me.TextProxyPassword = New System.Windows.Forms.TextBox()
        Me.RadioProxyNone = New System.Windows.Forms.RadioButton()
        Me.LabelProxyPassword = New System.Windows.Forms.Label()
        Me.RadioProxyIE = New System.Windows.Forms.RadioButton()
        Me.TextProxyUser = New System.Windows.Forms.TextBox()
        Me.RadioProxySpecified = New System.Windows.Forms.RadioButton()
        Me.LabelProxyUser = New System.Windows.Forms.Label()
        Me.LabelProxyAddress = New System.Windows.Forms.Label()
        Me.TextProxyPort = New System.Windows.Forms.TextBox()
        Me.TextProxyAddress = New System.Windows.Forms.TextBox()
        Me.LabelProxyPort = New System.Windows.Forms.Label()
        Me.CooperatePanel = New System.Windows.Forms.Panel()
        Me.ComboBoxTranslateLanguage = New System.Windows.Forms.ComboBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.CheckOutputz = New System.Windows.Forms.CheckBox()
        Me.CheckNicoms = New System.Windows.Forms.CheckBox()
        Me.TextBoxOutputzKey = New System.Windows.Forms.TextBox()
        Me.Label60 = New System.Windows.Forms.Label()
        Me.Label59 = New System.Windows.Forms.Label()
        Me.ComboBoxOutputzUrlmode = New System.Windows.Forms.ComboBox()
        Me.ShortUrlPanel = New System.Windows.Forms.Panel()
        Me.CheckTinyURL = New System.Windows.Forms.CheckBox()
        Me.TextBitlyPw = New System.Windows.Forms.TextBox()
        Me.CheckAutoConvertUrl = New System.Windows.Forms.CheckBox()
        Me.Label71 = New System.Windows.Forms.Label()
        Me.ComboBoxAutoShortUrlFirst = New System.Windows.Forms.ComboBox()
        Me.Label76 = New System.Windows.Forms.Label()
        Me.Label77 = New System.Windows.Forms.Label()
        Me.TextBitlyId = New System.Windows.Forms.TextBox()
        Me.ActionPanel = New System.Windows.Forms.Panel()
        Me.CheckOpenUserTimeline = New System.Windows.Forms.CheckBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.HotkeyCheck = New System.Windows.Forms.CheckBox()
        Me.HotkeyCode = New System.Windows.Forms.Label()
        Me.HotkeyText = New System.Windows.Forms.TextBox()
        Me.HotkeyWin = New System.Windows.Forms.CheckBox()
        Me.HotkeyAlt = New System.Windows.Forms.CheckBox()
        Me.HotkeyShift = New System.Windows.Forms.CheckBox()
        Me.HotkeyCtrl = New System.Windows.Forms.CheckBox()
        Me.Label57 = New System.Windows.Forms.Label()
        Me.CheckFavRestrict = New System.Windows.Forms.CheckBox()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.PlaySnd = New System.Windows.Forms.CheckBox()
        Me.chkReadOwnPost = New System.Windows.Forms.CheckBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.BrowserPathText = New System.Windows.Forms.TextBox()
        Me.UReadMng = New System.Windows.Forms.CheckBox()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.CheckCloseToExit = New System.Windows.Forms.CheckBox()
        Me.CheckMinimizeToTray = New System.Windows.Forms.CheckBox()
        Me.CheckReadOldPosts = New System.Windows.Forms.CheckBox()
        Me.FontDialog1 = New System.Windows.Forms.FontDialog()
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
        Me.Cancel = New System.Windows.Forms.Button()
        Me.Save = New System.Windows.Forms.Button()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.NotifyPanel.SuspendLayout()
        Me.GetPeriodPanel.SuspendLayout()
        Me.GetCountPanel.SuspendLayout()
        Me.BasedPanel.SuspendLayout()
        Me.StartupPanel.SuspendLayout()
        Me.UserStreamPanel.SuspendLayout()
        Me.TweetActPanel.SuspendLayout()
        Me.PreviewPanel.SuspendLayout()
        Me.TweetPrvPanel.SuspendLayout()
        Me.FontPanel.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.FontPanel2.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.ConnectionPanel.SuspendLayout()
        Me.ProxyPanel.SuspendLayout()
        Me.CooperatePanel.SuspendLayout()
        Me.ShortUrlPanel.SuspendLayout()
        Me.ActionPanel.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TreeViewSetting)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control
        Me.SplitContainer1.Panel2.Controls.Add(Me.NotifyPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.GetPeriodPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.GetCountPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.BasedPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.StartupPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.UserStreamPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.TweetActPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.PreviewPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.TweetPrvPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.FontPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.FontPanel2)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ConnectionPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ProxyPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.CooperatePanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ShortUrlPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ActionPanel)
        Me.SplitContainer1.TabStop = False
        '
        'TreeViewSetting
        '
        Me.TreeViewSetting.Cursor = System.Windows.Forms.Cursors.Hand
        resources.ApplyResources(Me.TreeViewSetting, "TreeViewSetting")
        Me.TreeViewSetting.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText
        Me.TreeViewSetting.HideSelection = False
        Me.TreeViewSetting.Name = "TreeViewSetting"
        Me.TreeViewSetting.Nodes.AddRange(New System.Windows.Forms.TreeNode() {CType(resources.GetObject("TreeViewSetting.Nodes"), System.Windows.Forms.TreeNode), CType(resources.GetObject("TreeViewSetting.Nodes1"), System.Windows.Forms.TreeNode), CType(resources.GetObject("TreeViewSetting.Nodes2"), System.Windows.Forms.TreeNode), CType(resources.GetObject("TreeViewSetting.Nodes3"), System.Windows.Forms.TreeNode), CType(resources.GetObject("TreeViewSetting.Nodes4"), System.Windows.Forms.TreeNode)})
        Me.TreeViewSetting.ShowLines = False
        '
        'NotifyPanel
        '
        Me.NotifyPanel.Controls.Add(Me.CheckUserUpdateEvent)
        Me.NotifyPanel.Controls.Add(Me.Label35)
        Me.NotifyPanel.Controls.Add(Me.ComboBoxEventNotifySound)
        Me.NotifyPanel.Controls.Add(Me.CheckFavEventUnread)
        Me.NotifyPanel.Controls.Add(Me.CheckListCreatedEvent)
        Me.NotifyPanel.Controls.Add(Me.CheckBlockEvent)
        Me.NotifyPanel.Controls.Add(Me.CheckForceEventNotify)
        Me.NotifyPanel.Controls.Add(Me.CheckListMemberRemovedEvent)
        Me.NotifyPanel.Controls.Add(Me.CheckListMemberAddedEvent)
        Me.NotifyPanel.Controls.Add(Me.CheckFollowEvent)
        Me.NotifyPanel.Controls.Add(Me.CheckUnfavoritesEvent)
        Me.NotifyPanel.Controls.Add(Me.CheckFavoritesEvent)
        Me.NotifyPanel.Controls.Add(Me.CheckEventNotify)
        resources.ApplyResources(Me.NotifyPanel, "NotifyPanel")
        Me.NotifyPanel.Name = "NotifyPanel"
        '
        'CheckUserUpdateEvent
        '
        resources.ApplyResources(Me.CheckUserUpdateEvent, "CheckUserUpdateEvent")
        Me.CheckUserUpdateEvent.Checked = True
        Me.CheckUserUpdateEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckUserUpdateEvent.Name = "CheckUserUpdateEvent"
        Me.CheckUserUpdateEvent.ThreeState = True
        Me.CheckUserUpdateEvent.UseVisualStyleBackColor = True
        '
        'Label35
        '
        resources.ApplyResources(Me.Label35, "Label35")
        Me.Label35.Name = "Label35"
        '
        'ComboBoxEventNotifySound
        '
        Me.ComboBoxEventNotifySound.FormattingEnabled = True
        resources.ApplyResources(Me.ComboBoxEventNotifySound, "ComboBoxEventNotifySound")
        Me.ComboBoxEventNotifySound.Name = "ComboBoxEventNotifySound"
        '
        'CheckFavEventUnread
        '
        resources.ApplyResources(Me.CheckFavEventUnread, "CheckFavEventUnread")
        Me.CheckFavEventUnread.Checked = True
        Me.CheckFavEventUnread.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckFavEventUnread.Name = "CheckFavEventUnread"
        Me.CheckFavEventUnread.UseVisualStyleBackColor = True
        '
        'CheckListCreatedEvent
        '
        resources.ApplyResources(Me.CheckListCreatedEvent, "CheckListCreatedEvent")
        Me.CheckListCreatedEvent.Checked = True
        Me.CheckListCreatedEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckListCreatedEvent.Name = "CheckListCreatedEvent"
        Me.CheckListCreatedEvent.ThreeState = True
        Me.CheckListCreatedEvent.UseVisualStyleBackColor = True
        '
        'CheckBlockEvent
        '
        resources.ApplyResources(Me.CheckBlockEvent, "CheckBlockEvent")
        Me.CheckBlockEvent.Checked = True
        Me.CheckBlockEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBlockEvent.Name = "CheckBlockEvent"
        Me.CheckBlockEvent.ThreeState = True
        Me.CheckBlockEvent.UseVisualStyleBackColor = True
        '
        'CheckForceEventNotify
        '
        resources.ApplyResources(Me.CheckForceEventNotify, "CheckForceEventNotify")
        Me.CheckForceEventNotify.Checked = True
        Me.CheckForceEventNotify.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckForceEventNotify.Name = "CheckForceEventNotify"
        Me.CheckForceEventNotify.UseVisualStyleBackColor = True
        '
        'CheckListMemberRemovedEvent
        '
        resources.ApplyResources(Me.CheckListMemberRemovedEvent, "CheckListMemberRemovedEvent")
        Me.CheckListMemberRemovedEvent.Checked = True
        Me.CheckListMemberRemovedEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckListMemberRemovedEvent.Name = "CheckListMemberRemovedEvent"
        Me.CheckListMemberRemovedEvent.ThreeState = True
        Me.CheckListMemberRemovedEvent.UseVisualStyleBackColor = True
        '
        'CheckListMemberAddedEvent
        '
        resources.ApplyResources(Me.CheckListMemberAddedEvent, "CheckListMemberAddedEvent")
        Me.CheckListMemberAddedEvent.Checked = True
        Me.CheckListMemberAddedEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckListMemberAddedEvent.Name = "CheckListMemberAddedEvent"
        Me.CheckListMemberAddedEvent.ThreeState = True
        Me.CheckListMemberAddedEvent.UseVisualStyleBackColor = True
        '
        'CheckFollowEvent
        '
        resources.ApplyResources(Me.CheckFollowEvent, "CheckFollowEvent")
        Me.CheckFollowEvent.Checked = True
        Me.CheckFollowEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckFollowEvent.Name = "CheckFollowEvent"
        Me.CheckFollowEvent.ThreeState = True
        Me.CheckFollowEvent.UseVisualStyleBackColor = True
        '
        'CheckUnfavoritesEvent
        '
        resources.ApplyResources(Me.CheckUnfavoritesEvent, "CheckUnfavoritesEvent")
        Me.CheckUnfavoritesEvent.Checked = True
        Me.CheckUnfavoritesEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckUnfavoritesEvent.Name = "CheckUnfavoritesEvent"
        Me.CheckUnfavoritesEvent.ThreeState = True
        Me.CheckUnfavoritesEvent.UseVisualStyleBackColor = True
        '
        'CheckFavoritesEvent
        '
        resources.ApplyResources(Me.CheckFavoritesEvent, "CheckFavoritesEvent")
        Me.CheckFavoritesEvent.Checked = True
        Me.CheckFavoritesEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckFavoritesEvent.Name = "CheckFavoritesEvent"
        Me.CheckFavoritesEvent.ThreeState = True
        Me.CheckFavoritesEvent.UseVisualStyleBackColor = True
        '
        'CheckEventNotify
        '
        resources.ApplyResources(Me.CheckEventNotify, "CheckEventNotify")
        Me.CheckEventNotify.Checked = True
        Me.CheckEventNotify.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckEventNotify.Name = "CheckEventNotify"
        Me.CheckEventNotify.UseVisualStyleBackColor = True
        '
        'GetPeriodPanel
        '
        Me.GetPeriodPanel.Controls.Add(Me.LabelApiUsingUserStreamEnabled)
        Me.GetPeriodPanel.Controls.Add(Me.LabelUserStreamActive)
        Me.GetPeriodPanel.Controls.Add(Me.Label21)
        Me.GetPeriodPanel.Controls.Add(Me.UserTimelinePeriod)
        Me.GetPeriodPanel.Controls.Add(Me.TimelinePeriod)
        Me.GetPeriodPanel.Controls.Add(Me.Label3)
        Me.GetPeriodPanel.Controls.Add(Me.ButtonApiCalc)
        Me.GetPeriodPanel.Controls.Add(Me.LabelPostAndGet)
        Me.GetPeriodPanel.Controls.Add(Me.LabelApiUsing)
        Me.GetPeriodPanel.Controls.Add(Me.Label33)
        Me.GetPeriodPanel.Controls.Add(Me.ListsPeriod)
        Me.GetPeriodPanel.Controls.Add(Me.Label7)
        Me.GetPeriodPanel.Controls.Add(Me.PubSearchPeriod)
        Me.GetPeriodPanel.Controls.Add(Me.Label69)
        Me.GetPeriodPanel.Controls.Add(Me.ReplyPeriod)
        Me.GetPeriodPanel.Controls.Add(Me.CheckPostAndGet)
        Me.GetPeriodPanel.Controls.Add(Me.CheckPeriodAdjust)
        Me.GetPeriodPanel.Controls.Add(Me.Label5)
        Me.GetPeriodPanel.Controls.Add(Me.DMPeriod)
        resources.ApplyResources(Me.GetPeriodPanel, "GetPeriodPanel")
        Me.GetPeriodPanel.Name = "GetPeriodPanel"
        '
        'LabelApiUsingUserStreamEnabled
        '
        resources.ApplyResources(Me.LabelApiUsingUserStreamEnabled, "LabelApiUsingUserStreamEnabled")
        Me.LabelApiUsingUserStreamEnabled.Name = "LabelApiUsingUserStreamEnabled"
        '
        'LabelUserStreamActive
        '
        resources.ApplyResources(Me.LabelUserStreamActive, "LabelUserStreamActive")
        Me.LabelUserStreamActive.Name = "LabelUserStreamActive"
        '
        'Label21
        '
        resources.ApplyResources(Me.Label21, "Label21")
        Me.Label21.Name = "Label21"
        '
        'UserTimelinePeriod
        '
        resources.ApplyResources(Me.UserTimelinePeriod, "UserTimelinePeriod")
        Me.UserTimelinePeriod.Name = "UserTimelinePeriod"
        '
        'TimelinePeriod
        '
        resources.ApplyResources(Me.TimelinePeriod, "TimelinePeriod")
        Me.TimelinePeriod.Name = "TimelinePeriod"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'ButtonApiCalc
        '
        resources.ApplyResources(Me.ButtonApiCalc, "ButtonApiCalc")
        Me.ButtonApiCalc.Name = "ButtonApiCalc"
        Me.ButtonApiCalc.UseVisualStyleBackColor = True
        '
        'LabelPostAndGet
        '
        resources.ApplyResources(Me.LabelPostAndGet, "LabelPostAndGet")
        Me.LabelPostAndGet.Name = "LabelPostAndGet"
        '
        'LabelApiUsing
        '
        resources.ApplyResources(Me.LabelApiUsing, "LabelApiUsing")
        Me.LabelApiUsing.Name = "LabelApiUsing"
        '
        'Label33
        '
        resources.ApplyResources(Me.Label33, "Label33")
        Me.Label33.Name = "Label33"
        '
        'ListsPeriod
        '
        resources.ApplyResources(Me.ListsPeriod, "ListsPeriod")
        Me.ListsPeriod.Name = "ListsPeriod"
        '
        'Label7
        '
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Name = "Label7"
        '
        'PubSearchPeriod
        '
        resources.ApplyResources(Me.PubSearchPeriod, "PubSearchPeriod")
        Me.PubSearchPeriod.Name = "PubSearchPeriod"
        '
        'Label69
        '
        resources.ApplyResources(Me.Label69, "Label69")
        Me.Label69.Name = "Label69"
        '
        'ReplyPeriod
        '
        resources.ApplyResources(Me.ReplyPeriod, "ReplyPeriod")
        Me.ReplyPeriod.Name = "ReplyPeriod"
        '
        'CheckPostAndGet
        '
        resources.ApplyResources(Me.CheckPostAndGet, "CheckPostAndGet")
        Me.CheckPostAndGet.Name = "CheckPostAndGet"
        Me.CheckPostAndGet.UseVisualStyleBackColor = True
        '
        'CheckPeriodAdjust
        '
        resources.ApplyResources(Me.CheckPeriodAdjust, "CheckPeriodAdjust")
        Me.CheckPeriodAdjust.Name = "CheckPeriodAdjust"
        Me.CheckPeriodAdjust.UseVisualStyleBackColor = True
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'DMPeriod
        '
        resources.ApplyResources(Me.DMPeriod, "DMPeriod")
        Me.DMPeriod.Name = "DMPeriod"
        '
        'GetCountPanel
        '
        Me.GetCountPanel.Controls.Add(Me.ListTextCountApi)
        Me.GetCountPanel.Controls.Add(Me.Label25)
        Me.GetCountPanel.Controls.Add(Me.UserTimelineTextCountApi)
        Me.GetCountPanel.Controls.Add(Me.Label17)
        Me.GetCountPanel.Controls.Add(Me.Label30)
        Me.GetCountPanel.Controls.Add(Me.Label28)
        Me.GetCountPanel.Controls.Add(Me.Label19)
        Me.GetCountPanel.Controls.Add(Me.FavoritesTextCountApi)
        Me.GetCountPanel.Controls.Add(Me.SearchTextCountApi)
        Me.GetCountPanel.Controls.Add(Me.Label66)
        Me.GetCountPanel.Controls.Add(Me.FirstTextCountApi)
        Me.GetCountPanel.Controls.Add(Me.GetMoreTextCountApi)
        Me.GetCountPanel.Controls.Add(Me.Label53)
        Me.GetCountPanel.Controls.Add(Me.UseChangeGetCount)
        Me.GetCountPanel.Controls.Add(Me.TextCountApiReply)
        Me.GetCountPanel.Controls.Add(Me.Label67)
        Me.GetCountPanel.Controls.Add(Me.TextCountApi)
        resources.ApplyResources(Me.GetCountPanel, "GetCountPanel")
        Me.GetCountPanel.Name = "GetCountPanel"
        '
        'ListTextCountApi
        '
        resources.ApplyResources(Me.ListTextCountApi, "ListTextCountApi")
        Me.ListTextCountApi.Name = "ListTextCountApi"
        '
        'Label25
        '
        resources.ApplyResources(Me.Label25, "Label25")
        Me.Label25.Name = "Label25"
        '
        'UserTimelineTextCountApi
        '
        resources.ApplyResources(Me.UserTimelineTextCountApi, "UserTimelineTextCountApi")
        Me.UserTimelineTextCountApi.Name = "UserTimelineTextCountApi"
        '
        'Label17
        '
        resources.ApplyResources(Me.Label17, "Label17")
        Me.Label17.Name = "Label17"
        '
        'Label30
        '
        resources.ApplyResources(Me.Label30, "Label30")
        Me.Label30.Name = "Label30"
        '
        'Label28
        '
        resources.ApplyResources(Me.Label28, "Label28")
        Me.Label28.Name = "Label28"
        '
        'Label19
        '
        resources.ApplyResources(Me.Label19, "Label19")
        Me.Label19.Name = "Label19"
        '
        'FavoritesTextCountApi
        '
        resources.ApplyResources(Me.FavoritesTextCountApi, "FavoritesTextCountApi")
        Me.FavoritesTextCountApi.Name = "FavoritesTextCountApi"
        '
        'SearchTextCountApi
        '
        resources.ApplyResources(Me.SearchTextCountApi, "SearchTextCountApi")
        Me.SearchTextCountApi.Name = "SearchTextCountApi"
        '
        'Label66
        '
        resources.ApplyResources(Me.Label66, "Label66")
        Me.Label66.Name = "Label66"
        '
        'FirstTextCountApi
        '
        resources.ApplyResources(Me.FirstTextCountApi, "FirstTextCountApi")
        Me.FirstTextCountApi.Name = "FirstTextCountApi"
        '
        'GetMoreTextCountApi
        '
        resources.ApplyResources(Me.GetMoreTextCountApi, "GetMoreTextCountApi")
        Me.GetMoreTextCountApi.Name = "GetMoreTextCountApi"
        '
        'Label53
        '
        resources.ApplyResources(Me.Label53, "Label53")
        Me.Label53.Name = "Label53"
        '
        'UseChangeGetCount
        '
        resources.ApplyResources(Me.UseChangeGetCount, "UseChangeGetCount")
        Me.UseChangeGetCount.Name = "UseChangeGetCount"
        Me.UseChangeGetCount.UseVisualStyleBackColor = True
        '
        'TextCountApiReply
        '
        resources.ApplyResources(Me.TextCountApiReply, "TextCountApiReply")
        Me.TextCountApiReply.Name = "TextCountApiReply"
        '
        'Label67
        '
        resources.ApplyResources(Me.Label67, "Label67")
        Me.Label67.Name = "Label67"
        '
        'TextCountApi
        '
        resources.ApplyResources(Me.TextCountApi, "TextCountApi")
        Me.TextCountApi.Name = "TextCountApi"
        '
        'BasedPanel
        '
        Me.BasedPanel.Controls.Add(Me.AuthBasicRadio)
        Me.BasedPanel.Controls.Add(Me.AuthOAuthRadio)
        Me.BasedPanel.Controls.Add(Me.Label6)
        Me.BasedPanel.Controls.Add(Me.AuthClearButton)
        Me.BasedPanel.Controls.Add(Me.AuthUserLabel)
        Me.BasedPanel.Controls.Add(Me.AuthStateLabel)
        Me.BasedPanel.Controls.Add(Me.Label4)
        Me.BasedPanel.Controls.Add(Me.AuthorizeButton)
        Me.BasedPanel.Controls.Add(Me.Label1)
        Me.BasedPanel.Controls.Add(Me.Label2)
        Me.BasedPanel.Controls.Add(Me.Username)
        Me.BasedPanel.Controls.Add(Me.Password)
        resources.ApplyResources(Me.BasedPanel, "BasedPanel")
        Me.BasedPanel.Name = "BasedPanel"
        '
        'AuthBasicRadio
        '
        resources.ApplyResources(Me.AuthBasicRadio, "AuthBasicRadio")
        Me.AuthBasicRadio.Name = "AuthBasicRadio"
        Me.AuthBasicRadio.UseVisualStyleBackColor = True
        '
        'AuthOAuthRadio
        '
        resources.ApplyResources(Me.AuthOAuthRadio, "AuthOAuthRadio")
        Me.AuthOAuthRadio.Checked = True
        Me.AuthOAuthRadio.Name = "AuthOAuthRadio"
        Me.AuthOAuthRadio.TabStop = True
        Me.AuthOAuthRadio.UseVisualStyleBackColor = True
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'AuthClearButton
        '
        resources.ApplyResources(Me.AuthClearButton, "AuthClearButton")
        Me.AuthClearButton.Name = "AuthClearButton"
        Me.AuthClearButton.UseVisualStyleBackColor = True
        '
        'AuthUserLabel
        '
        Me.AuthUserLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.AuthUserLabel, "AuthUserLabel")
        Me.AuthUserLabel.Name = "AuthUserLabel"
        '
        'AuthStateLabel
        '
        Me.AuthStateLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.AuthStateLabel, "AuthStateLabel")
        Me.AuthStateLabel.Name = "AuthStateLabel"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'AuthorizeButton
        '
        resources.ApplyResources(Me.AuthorizeButton, "AuthorizeButton")
        Me.AuthorizeButton.Name = "AuthorizeButton"
        Me.AuthorizeButton.UseVisualStyleBackColor = True
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'Username
        '
        resources.ApplyResources(Me.Username, "Username")
        Me.Username.Name = "Username"
        '
        'Password
        '
        resources.ApplyResources(Me.Password, "Password")
        Me.Password.Name = "Password"
        Me.Password.UseSystemPasswordChar = True
        '
        'StartupPanel
        '
        Me.StartupPanel.Controls.Add(Me.StartupReaded)
        Me.StartupPanel.Controls.Add(Me.CheckStartupFollowers)
        Me.StartupPanel.Controls.Add(Me.CheckStartupVersion)
        Me.StartupPanel.Controls.Add(Me.chkGetFav)
        resources.ApplyResources(Me.StartupPanel, "StartupPanel")
        Me.StartupPanel.Name = "StartupPanel"
        '
        'StartupReaded
        '
        resources.ApplyResources(Me.StartupReaded, "StartupReaded")
        Me.StartupReaded.Name = "StartupReaded"
        Me.StartupReaded.UseVisualStyleBackColor = True
        '
        'CheckStartupFollowers
        '
        resources.ApplyResources(Me.CheckStartupFollowers, "CheckStartupFollowers")
        Me.CheckStartupFollowers.Name = "CheckStartupFollowers"
        Me.CheckStartupFollowers.UseVisualStyleBackColor = True
        '
        'CheckStartupVersion
        '
        resources.ApplyResources(Me.CheckStartupVersion, "CheckStartupVersion")
        Me.CheckStartupVersion.Name = "CheckStartupVersion"
        Me.CheckStartupVersion.UseVisualStyleBackColor = True
        '
        'chkGetFav
        '
        resources.ApplyResources(Me.chkGetFav, "chkGetFav")
        Me.chkGetFav.Name = "chkGetFav"
        Me.chkGetFav.UseVisualStyleBackColor = True
        '
        'UserStreamPanel
        '
        Me.UserStreamPanel.Controls.Add(Me.UserstreamPeriod)
        Me.UserStreamPanel.Controls.Add(Me.StartupUserstreamCheck)
        Me.UserStreamPanel.Controls.Add(Me.Label83)
        resources.ApplyResources(Me.UserStreamPanel, "UserStreamPanel")
        Me.UserStreamPanel.Name = "UserStreamPanel"
        '
        'UserstreamPeriod
        '
        resources.ApplyResources(Me.UserstreamPeriod, "UserstreamPeriod")
        Me.UserstreamPeriod.Name = "UserstreamPeriod"
        '
        'StartupUserstreamCheck
        '
        resources.ApplyResources(Me.StartupUserstreamCheck, "StartupUserstreamCheck")
        Me.StartupUserstreamCheck.Name = "StartupUserstreamCheck"
        Me.StartupUserstreamCheck.UseVisualStyleBackColor = True
        '
        'Label83
        '
        resources.ApplyResources(Me.Label83, "Label83")
        Me.Label83.Name = "Label83"
        '
        'TweetActPanel
        '
        Me.TweetActPanel.Controls.Add(Me.CheckHashSupple)
        Me.TweetActPanel.Controls.Add(Me.CheckAtIdSupple)
        Me.TweetActPanel.Controls.Add(Me.ComboBoxPostKeySelect)
        Me.TweetActPanel.Controls.Add(Me.Label27)
        Me.TweetActPanel.Controls.Add(Me.CheckRetweetNoConfirm)
        Me.TweetActPanel.Controls.Add(Me.Label12)
        Me.TweetActPanel.Controls.Add(Me.CheckUseRecommendStatus)
        Me.TweetActPanel.Controls.Add(Me.StatusText)
        resources.ApplyResources(Me.TweetActPanel, "TweetActPanel")
        Me.TweetActPanel.Name = "TweetActPanel"
        '
        'CheckHashSupple
        '
        resources.ApplyResources(Me.CheckHashSupple, "CheckHashSupple")
        Me.CheckHashSupple.Name = "CheckHashSupple"
        Me.CheckHashSupple.UseVisualStyleBackColor = True
        '
        'CheckAtIdSupple
        '
        resources.ApplyResources(Me.CheckAtIdSupple, "CheckAtIdSupple")
        Me.CheckAtIdSupple.Name = "CheckAtIdSupple"
        Me.CheckAtIdSupple.UseVisualStyleBackColor = True
        '
        'ComboBoxPostKeySelect
        '
        Me.ComboBoxPostKeySelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxPostKeySelect.FormattingEnabled = True
        Me.ComboBoxPostKeySelect.Items.AddRange(New Object() {resources.GetString("ComboBoxPostKeySelect.Items"), resources.GetString("ComboBoxPostKeySelect.Items1"), resources.GetString("ComboBoxPostKeySelect.Items2")})
        resources.ApplyResources(Me.ComboBoxPostKeySelect, "ComboBoxPostKeySelect")
        Me.ComboBoxPostKeySelect.Name = "ComboBoxPostKeySelect"
        '
        'Label27
        '
        resources.ApplyResources(Me.Label27, "Label27")
        Me.Label27.Name = "Label27"
        '
        'CheckRetweetNoConfirm
        '
        resources.ApplyResources(Me.CheckRetweetNoConfirm, "CheckRetweetNoConfirm")
        Me.CheckRetweetNoConfirm.Name = "CheckRetweetNoConfirm"
        Me.CheckRetweetNoConfirm.UseVisualStyleBackColor = True
        '
        'Label12
        '
        resources.ApplyResources(Me.Label12, "Label12")
        Me.Label12.Name = "Label12"
        '
        'CheckUseRecommendStatus
        '
        resources.ApplyResources(Me.CheckUseRecommendStatus, "CheckUseRecommendStatus")
        Me.CheckUseRecommendStatus.Name = "CheckUseRecommendStatus"
        Me.CheckUseRecommendStatus.UseVisualStyleBackColor = True
        '
        'StatusText
        '
        resources.ApplyResources(Me.StatusText, "StatusText")
        Me.StatusText.Name = "StatusText"
        '
        'PreviewPanel
        '
        Me.PreviewPanel.Controls.Add(Me.ReplyIconStateCombo)
        Me.PreviewPanel.Controls.Add(Me.Label72)
        Me.PreviewPanel.Controls.Add(Me.ChkNewMentionsBlink)
        Me.PreviewPanel.Controls.Add(Me.chkTabIconDisp)
        Me.PreviewPanel.Controls.Add(Me.CheckPreviewEnable)
        Me.PreviewPanel.Controls.Add(Me.Label81)
        Me.PreviewPanel.Controls.Add(Me.LanguageCombo)
        Me.PreviewPanel.Controls.Add(Me.Label13)
        Me.PreviewPanel.Controls.Add(Me.CheckAlwaysTop)
        Me.PreviewPanel.Controls.Add(Me.CheckMonospace)
        Me.PreviewPanel.Controls.Add(Me.CheckBalloonLimit)
        Me.PreviewPanel.Controls.Add(Me.Label10)
        Me.PreviewPanel.Controls.Add(Me.ComboDispTitle)
        Me.PreviewPanel.Controls.Add(Me.Label45)
        Me.PreviewPanel.Controls.Add(Me.cmbNameBalloon)
        Me.PreviewPanel.Controls.Add(Me.CheckDispUsername)
        Me.PreviewPanel.Controls.Add(Me.CheckBox3)
        resources.ApplyResources(Me.PreviewPanel, "PreviewPanel")
        Me.PreviewPanel.Name = "PreviewPanel"
        '
        'ReplyIconStateCombo
        '
        Me.ReplyIconStateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ReplyIconStateCombo.FormattingEnabled = True
        Me.ReplyIconStateCombo.Items.AddRange(New Object() {resources.GetString("ReplyIconStateCombo.Items"), resources.GetString("ReplyIconStateCombo.Items1"), resources.GetString("ReplyIconStateCombo.Items2")})
        resources.ApplyResources(Me.ReplyIconStateCombo, "ReplyIconStateCombo")
        Me.ReplyIconStateCombo.Name = "ReplyIconStateCombo"
        '
        'Label72
        '
        resources.ApplyResources(Me.Label72, "Label72")
        Me.Label72.Name = "Label72"
        '
        'ChkNewMentionsBlink
        '
        resources.ApplyResources(Me.ChkNewMentionsBlink, "ChkNewMentionsBlink")
        Me.ChkNewMentionsBlink.Name = "ChkNewMentionsBlink"
        Me.ChkNewMentionsBlink.UseVisualStyleBackColor = True
        '
        'chkTabIconDisp
        '
        resources.ApplyResources(Me.chkTabIconDisp, "chkTabIconDisp")
        Me.chkTabIconDisp.Name = "chkTabIconDisp"
        Me.chkTabIconDisp.UseVisualStyleBackColor = True
        '
        'CheckPreviewEnable
        '
        resources.ApplyResources(Me.CheckPreviewEnable, "CheckPreviewEnable")
        Me.CheckPreviewEnable.Name = "CheckPreviewEnable"
        Me.CheckPreviewEnable.UseVisualStyleBackColor = True
        '
        'Label81
        '
        resources.ApplyResources(Me.Label81, "Label81")
        Me.Label81.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label81.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label81.Name = "Label81"
        '
        'LanguageCombo
        '
        Me.LanguageCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.LanguageCombo.FormattingEnabled = True
        Me.LanguageCombo.Items.AddRange(New Object() {resources.GetString("LanguageCombo.Items"), resources.GetString("LanguageCombo.Items1"), resources.GetString("LanguageCombo.Items2"), resources.GetString("LanguageCombo.Items3")})
        resources.ApplyResources(Me.LanguageCombo, "LanguageCombo")
        Me.LanguageCombo.Name = "LanguageCombo"
        '
        'Label13
        '
        resources.ApplyResources(Me.Label13, "Label13")
        Me.Label13.Name = "Label13"
        '
        'CheckAlwaysTop
        '
        resources.ApplyResources(Me.CheckAlwaysTop, "CheckAlwaysTop")
        Me.CheckAlwaysTop.Name = "CheckAlwaysTop"
        Me.CheckAlwaysTop.UseVisualStyleBackColor = True
        '
        'CheckMonospace
        '
        resources.ApplyResources(Me.CheckMonospace, "CheckMonospace")
        Me.CheckMonospace.Name = "CheckMonospace"
        Me.CheckMonospace.UseVisualStyleBackColor = True
        '
        'CheckBalloonLimit
        '
        resources.ApplyResources(Me.CheckBalloonLimit, "CheckBalloonLimit")
        Me.CheckBalloonLimit.Name = "CheckBalloonLimit"
        Me.CheckBalloonLimit.UseVisualStyleBackColor = True
        '
        'Label10
        '
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Name = "Label10"
        '
        'ComboDispTitle
        '
        Me.ComboDispTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboDispTitle.FormattingEnabled = True
        Me.ComboDispTitle.Items.AddRange(New Object() {resources.GetString("ComboDispTitle.Items"), resources.GetString("ComboDispTitle.Items1"), resources.GetString("ComboDispTitle.Items2"), resources.GetString("ComboDispTitle.Items3"), resources.GetString("ComboDispTitle.Items4"), resources.GetString("ComboDispTitle.Items5"), resources.GetString("ComboDispTitle.Items6"), resources.GetString("ComboDispTitle.Items7")})
        resources.ApplyResources(Me.ComboDispTitle, "ComboDispTitle")
        Me.ComboDispTitle.Name = "ComboDispTitle"
        '
        'Label45
        '
        resources.ApplyResources(Me.Label45, "Label45")
        Me.Label45.Name = "Label45"
        '
        'cmbNameBalloon
        '
        Me.cmbNameBalloon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNameBalloon.FormattingEnabled = True
        Me.cmbNameBalloon.Items.AddRange(New Object() {resources.GetString("cmbNameBalloon.Items"), resources.GetString("cmbNameBalloon.Items1"), resources.GetString("cmbNameBalloon.Items2")})
        resources.ApplyResources(Me.cmbNameBalloon, "cmbNameBalloon")
        Me.cmbNameBalloon.Name = "cmbNameBalloon"
        '
        'CheckDispUsername
        '
        resources.ApplyResources(Me.CheckDispUsername, "CheckDispUsername")
        Me.CheckDispUsername.Name = "CheckDispUsername"
        Me.CheckDispUsername.UseVisualStyleBackColor = True
        '
        'CheckBox3
        '
        resources.ApplyResources(Me.CheckBox3, "CheckBox3")
        Me.CheckBox3.Name = "CheckBox3"
        Me.CheckBox3.UseVisualStyleBackColor = True
        '
        'TweetPrvPanel
        '
        Me.TweetPrvPanel.Controls.Add(Me.Label47)
        Me.TweetPrvPanel.Controls.Add(Me.LabelDateTimeFormatApplied)
        Me.TweetPrvPanel.Controls.Add(Me.Label62)
        Me.TweetPrvPanel.Controls.Add(Me.CmbDateTimeFormat)
        Me.TweetPrvPanel.Controls.Add(Me.Label23)
        Me.TweetPrvPanel.Controls.Add(Me.Label11)
        Me.TweetPrvPanel.Controls.Add(Me.IconSize)
        Me.TweetPrvPanel.Controls.Add(Me.TextBox3)
        Me.TweetPrvPanel.Controls.Add(Me.CheckSortOrderLock)
        Me.TweetPrvPanel.Controls.Add(Me.CheckShowGrid)
        Me.TweetPrvPanel.Controls.Add(Me.chkUnreadStyle)
        Me.TweetPrvPanel.Controls.Add(Me.OneWayLv)
        resources.ApplyResources(Me.TweetPrvPanel, "TweetPrvPanel")
        Me.TweetPrvPanel.Name = "TweetPrvPanel"
        '
        'Label47
        '
        resources.ApplyResources(Me.Label47, "Label47")
        Me.Label47.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label47.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label47.Name = "Label47"
        '
        'LabelDateTimeFormatApplied
        '
        resources.ApplyResources(Me.LabelDateTimeFormatApplied, "LabelDateTimeFormatApplied")
        Me.LabelDateTimeFormatApplied.Name = "LabelDateTimeFormatApplied"
        '
        'Label62
        '
        resources.ApplyResources(Me.Label62, "Label62")
        Me.Label62.Name = "Label62"
        '
        'CmbDateTimeFormat
        '
        resources.ApplyResources(Me.CmbDateTimeFormat, "CmbDateTimeFormat")
        Me.CmbDateTimeFormat.Items.AddRange(New Object() {resources.GetString("CmbDateTimeFormat.Items"), resources.GetString("CmbDateTimeFormat.Items1"), resources.GetString("CmbDateTimeFormat.Items2"), resources.GetString("CmbDateTimeFormat.Items3"), resources.GetString("CmbDateTimeFormat.Items4"), resources.GetString("CmbDateTimeFormat.Items5"), resources.GetString("CmbDateTimeFormat.Items6"), resources.GetString("CmbDateTimeFormat.Items7"), resources.GetString("CmbDateTimeFormat.Items8"), resources.GetString("CmbDateTimeFormat.Items9"), resources.GetString("CmbDateTimeFormat.Items10")})
        Me.CmbDateTimeFormat.Name = "CmbDateTimeFormat"
        '
        'Label23
        '
        resources.ApplyResources(Me.Label23, "Label23")
        Me.Label23.Name = "Label23"
        '
        'Label11
        '
        resources.ApplyResources(Me.Label11, "Label11")
        Me.Label11.Name = "Label11"
        '
        'IconSize
        '
        Me.IconSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.IconSize.FormattingEnabled = True
        Me.IconSize.Items.AddRange(New Object() {resources.GetString("IconSize.Items"), resources.GetString("IconSize.Items1"), resources.GetString("IconSize.Items2"), resources.GetString("IconSize.Items3"), resources.GetString("IconSize.Items4")})
        resources.ApplyResources(Me.IconSize, "IconSize")
        Me.IconSize.Name = "IconSize"
        '
        'TextBox3
        '
        resources.ApplyResources(Me.TextBox3, "TextBox3")
        Me.TextBox3.Name = "TextBox3"
        '
        'CheckSortOrderLock
        '
        resources.ApplyResources(Me.CheckSortOrderLock, "CheckSortOrderLock")
        Me.CheckSortOrderLock.Name = "CheckSortOrderLock"
        Me.CheckSortOrderLock.UseVisualStyleBackColor = True
        '
        'CheckShowGrid
        '
        resources.ApplyResources(Me.CheckShowGrid, "CheckShowGrid")
        Me.CheckShowGrid.Name = "CheckShowGrid"
        Me.CheckShowGrid.UseVisualStyleBackColor = True
        '
        'chkUnreadStyle
        '
        resources.ApplyResources(Me.chkUnreadStyle, "chkUnreadStyle")
        Me.chkUnreadStyle.Name = "chkUnreadStyle"
        Me.chkUnreadStyle.UseVisualStyleBackColor = True
        '
        'OneWayLv
        '
        resources.ApplyResources(Me.OneWayLv, "OneWayLv")
        Me.OneWayLv.Name = "OneWayLv"
        Me.OneWayLv.UseVisualStyleBackColor = True
        '
        'FontPanel
        '
        Me.FontPanel.Controls.Add(Me.GroupBox1)
        resources.ApplyResources(Me.FontPanel, "FontPanel")
        Me.FontPanel.Name = "FontPanel"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnRetweet)
        Me.GroupBox1.Controls.Add(Me.lblRetweet)
        Me.GroupBox1.Controls.Add(Me.Label80)
        Me.GroupBox1.Controls.Add(Me.ButtonBackToDefaultFontColor)
        Me.GroupBox1.Controls.Add(Me.btnDetailLink)
        Me.GroupBox1.Controls.Add(Me.lblDetailLink)
        Me.GroupBox1.Controls.Add(Me.Label18)
        Me.GroupBox1.Controls.Add(Me.btnUnread)
        Me.GroupBox1.Controls.Add(Me.lblUnread)
        Me.GroupBox1.Controls.Add(Me.Label20)
        Me.GroupBox1.Controls.Add(Me.btnDetailBack)
        Me.GroupBox1.Controls.Add(Me.lblDetailBackcolor)
        Me.GroupBox1.Controls.Add(Me.Label37)
        Me.GroupBox1.Controls.Add(Me.btnDetail)
        Me.GroupBox1.Controls.Add(Me.lblDetail)
        Me.GroupBox1.Controls.Add(Me.Label26)
        Me.GroupBox1.Controls.Add(Me.btnOWL)
        Me.GroupBox1.Controls.Add(Me.lblOWL)
        Me.GroupBox1.Controls.Add(Me.Label24)
        Me.GroupBox1.Controls.Add(Me.btnFav)
        Me.GroupBox1.Controls.Add(Me.lblFav)
        Me.GroupBox1.Controls.Add(Me.Label22)
        Me.GroupBox1.Controls.Add(Me.btnListFont)
        Me.GroupBox1.Controls.Add(Me.lblListFont)
        Me.GroupBox1.Controls.Add(Me.Label61)
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'btnRetweet
        '
        resources.ApplyResources(Me.btnRetweet, "btnRetweet")
        Me.btnRetweet.Name = "btnRetweet"
        Me.btnRetweet.UseVisualStyleBackColor = True
        '
        'lblRetweet
        '
        Me.lblRetweet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRetweet, "lblRetweet")
        Me.lblRetweet.Name = "lblRetweet"
        '
        'Label80
        '
        resources.ApplyResources(Me.Label80, "Label80")
        Me.Label80.Name = "Label80"
        '
        'ButtonBackToDefaultFontColor
        '
        resources.ApplyResources(Me.ButtonBackToDefaultFontColor, "ButtonBackToDefaultFontColor")
        Me.ButtonBackToDefaultFontColor.Name = "ButtonBackToDefaultFontColor"
        Me.ButtonBackToDefaultFontColor.UseVisualStyleBackColor = True
        '
        'btnDetailLink
        '
        resources.ApplyResources(Me.btnDetailLink, "btnDetailLink")
        Me.btnDetailLink.Name = "btnDetailLink"
        Me.btnDetailLink.UseVisualStyleBackColor = True
        '
        'lblDetailLink
        '
        Me.lblDetailLink.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblDetailLink, "lblDetailLink")
        Me.lblDetailLink.Name = "lblDetailLink"
        '
        'Label18
        '
        resources.ApplyResources(Me.Label18, "Label18")
        Me.Label18.Name = "Label18"
        '
        'btnUnread
        '
        resources.ApplyResources(Me.btnUnread, "btnUnread")
        Me.btnUnread.Name = "btnUnread"
        Me.btnUnread.UseVisualStyleBackColor = True
        '
        'lblUnread
        '
        Me.lblUnread.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblUnread, "lblUnread")
        Me.lblUnread.Name = "lblUnread"
        '
        'Label20
        '
        resources.ApplyResources(Me.Label20, "Label20")
        Me.Label20.Name = "Label20"
        '
        'btnDetailBack
        '
        resources.ApplyResources(Me.btnDetailBack, "btnDetailBack")
        Me.btnDetailBack.Name = "btnDetailBack"
        Me.btnDetailBack.UseVisualStyleBackColor = True
        '
        'lblDetailBackcolor
        '
        Me.lblDetailBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblDetailBackcolor, "lblDetailBackcolor")
        Me.lblDetailBackcolor.Name = "lblDetailBackcolor"
        '
        'Label37
        '
        resources.ApplyResources(Me.Label37, "Label37")
        Me.Label37.Name = "Label37"
        '
        'btnDetail
        '
        resources.ApplyResources(Me.btnDetail, "btnDetail")
        Me.btnDetail.Name = "btnDetail"
        Me.btnDetail.UseVisualStyleBackColor = True
        '
        'lblDetail
        '
        Me.lblDetail.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblDetail, "lblDetail")
        Me.lblDetail.Name = "lblDetail"
        '
        'Label26
        '
        resources.ApplyResources(Me.Label26, "Label26")
        Me.Label26.Name = "Label26"
        '
        'btnOWL
        '
        resources.ApplyResources(Me.btnOWL, "btnOWL")
        Me.btnOWL.Name = "btnOWL"
        Me.btnOWL.UseVisualStyleBackColor = True
        '
        'lblOWL
        '
        Me.lblOWL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblOWL, "lblOWL")
        Me.lblOWL.Name = "lblOWL"
        '
        'Label24
        '
        resources.ApplyResources(Me.Label24, "Label24")
        Me.Label24.Name = "Label24"
        '
        'btnFav
        '
        resources.ApplyResources(Me.btnFav, "btnFav")
        Me.btnFav.Name = "btnFav"
        Me.btnFav.UseVisualStyleBackColor = True
        '
        'lblFav
        '
        Me.lblFav.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblFav, "lblFav")
        Me.lblFav.Name = "lblFav"
        '
        'Label22
        '
        resources.ApplyResources(Me.Label22, "Label22")
        Me.Label22.Name = "Label22"
        '
        'btnListFont
        '
        resources.ApplyResources(Me.btnListFont, "btnListFont")
        Me.btnListFont.Name = "btnListFont"
        Me.btnListFont.UseVisualStyleBackColor = True
        '
        'lblListFont
        '
        Me.lblListFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblListFont, "lblListFont")
        Me.lblListFont.Name = "lblListFont"
        '
        'Label61
        '
        resources.ApplyResources(Me.Label61, "Label61")
        Me.Label61.Name = "Label61"
        '
        'FontPanel2
        '
        Me.FontPanel2.Controls.Add(Me.GroupBox5)
        resources.ApplyResources(Me.FontPanel2, "FontPanel2")
        Me.FontPanel2.Name = "FontPanel2"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.Label65)
        Me.GroupBox5.Controls.Add(Me.Label52)
        Me.GroupBox5.Controls.Add(Me.Label49)
        Me.GroupBox5.Controls.Add(Me.Label9)
        Me.GroupBox5.Controls.Add(Me.Label14)
        Me.GroupBox5.Controls.Add(Me.Label16)
        Me.GroupBox5.Controls.Add(Me.Label32)
        Me.GroupBox5.Controls.Add(Me.Label34)
        Me.GroupBox5.Controls.Add(Me.Label36)
        Me.GroupBox5.Controls.Add(Me.btnInputFont)
        Me.GroupBox5.Controls.Add(Me.btnInputBackcolor)
        Me.GroupBox5.Controls.Add(Me.btnAtTo)
        Me.GroupBox5.Controls.Add(Me.btnListBack)
        Me.GroupBox5.Controls.Add(Me.btnAtFromTarget)
        Me.GroupBox5.Controls.Add(Me.btnAtTarget)
        Me.GroupBox5.Controls.Add(Me.btnTarget)
        Me.GroupBox5.Controls.Add(Me.btnAtSelf)
        Me.GroupBox5.Controls.Add(Me.btnSelf)
        Me.GroupBox5.Controls.Add(Me.lblInputFont)
        Me.GroupBox5.Controls.Add(Me.lblInputBackcolor)
        Me.GroupBox5.Controls.Add(Me.lblAtTo)
        Me.GroupBox5.Controls.Add(Me.lblListBackcolor)
        Me.GroupBox5.Controls.Add(Me.lblAtFromTarget)
        Me.GroupBox5.Controls.Add(Me.lblAtTarget)
        Me.GroupBox5.Controls.Add(Me.lblTarget)
        Me.GroupBox5.Controls.Add(Me.lblAtSelf)
        Me.GroupBox5.Controls.Add(Me.lblSelf)
        Me.GroupBox5.Controls.Add(Me.ButtonBackToDefaultFontColor2)
        resources.ApplyResources(Me.GroupBox5, "GroupBox5")
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.TabStop = False
        '
        'Label65
        '
        resources.ApplyResources(Me.Label65, "Label65")
        Me.Label65.Name = "Label65"
        '
        'Label52
        '
        resources.ApplyResources(Me.Label52, "Label52")
        Me.Label52.Name = "Label52"
        '
        'Label49
        '
        resources.ApplyResources(Me.Label49, "Label49")
        Me.Label49.Name = "Label49"
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        '
        'Label14
        '
        resources.ApplyResources(Me.Label14, "Label14")
        Me.Label14.Name = "Label14"
        '
        'Label16
        '
        resources.ApplyResources(Me.Label16, "Label16")
        Me.Label16.Name = "Label16"
        '
        'Label32
        '
        resources.ApplyResources(Me.Label32, "Label32")
        Me.Label32.Name = "Label32"
        '
        'Label34
        '
        resources.ApplyResources(Me.Label34, "Label34")
        Me.Label34.Name = "Label34"
        '
        'Label36
        '
        resources.ApplyResources(Me.Label36, "Label36")
        Me.Label36.Name = "Label36"
        '
        'btnInputFont
        '
        resources.ApplyResources(Me.btnInputFont, "btnInputFont")
        Me.btnInputFont.Name = "btnInputFont"
        Me.btnInputFont.UseVisualStyleBackColor = True
        '
        'btnInputBackcolor
        '
        resources.ApplyResources(Me.btnInputBackcolor, "btnInputBackcolor")
        Me.btnInputBackcolor.Name = "btnInputBackcolor"
        Me.btnInputBackcolor.UseVisualStyleBackColor = True
        '
        'btnAtTo
        '
        resources.ApplyResources(Me.btnAtTo, "btnAtTo")
        Me.btnAtTo.Name = "btnAtTo"
        Me.btnAtTo.UseVisualStyleBackColor = True
        '
        'btnListBack
        '
        resources.ApplyResources(Me.btnListBack, "btnListBack")
        Me.btnListBack.Name = "btnListBack"
        Me.btnListBack.UseVisualStyleBackColor = True
        '
        'btnAtFromTarget
        '
        resources.ApplyResources(Me.btnAtFromTarget, "btnAtFromTarget")
        Me.btnAtFromTarget.Name = "btnAtFromTarget"
        Me.btnAtFromTarget.UseVisualStyleBackColor = True
        '
        'btnAtTarget
        '
        resources.ApplyResources(Me.btnAtTarget, "btnAtTarget")
        Me.btnAtTarget.Name = "btnAtTarget"
        Me.btnAtTarget.UseVisualStyleBackColor = True
        '
        'btnTarget
        '
        resources.ApplyResources(Me.btnTarget, "btnTarget")
        Me.btnTarget.Name = "btnTarget"
        Me.btnTarget.UseVisualStyleBackColor = True
        '
        'btnAtSelf
        '
        resources.ApplyResources(Me.btnAtSelf, "btnAtSelf")
        Me.btnAtSelf.Name = "btnAtSelf"
        Me.btnAtSelf.UseVisualStyleBackColor = True
        '
        'btnSelf
        '
        resources.ApplyResources(Me.btnSelf, "btnSelf")
        Me.btnSelf.Name = "btnSelf"
        Me.btnSelf.UseVisualStyleBackColor = True
        '
        'lblInputFont
        '
        Me.lblInputFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblInputFont, "lblInputFont")
        Me.lblInputFont.Name = "lblInputFont"
        '
        'lblInputBackcolor
        '
        Me.lblInputBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblInputBackcolor, "lblInputBackcolor")
        Me.lblInputBackcolor.Name = "lblInputBackcolor"
        '
        'lblAtTo
        '
        Me.lblAtTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAtTo, "lblAtTo")
        Me.lblAtTo.Name = "lblAtTo"
        '
        'lblListBackcolor
        '
        Me.lblListBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblListBackcolor, "lblListBackcolor")
        Me.lblListBackcolor.Name = "lblListBackcolor"
        '
        'lblAtFromTarget
        '
        Me.lblAtFromTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAtFromTarget, "lblAtFromTarget")
        Me.lblAtFromTarget.Name = "lblAtFromTarget"
        '
        'lblAtTarget
        '
        Me.lblAtTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAtTarget, "lblAtTarget")
        Me.lblAtTarget.Name = "lblAtTarget"
        '
        'lblTarget
        '
        Me.lblTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblTarget, "lblTarget")
        Me.lblTarget.Name = "lblTarget"
        '
        'lblAtSelf
        '
        Me.lblAtSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAtSelf, "lblAtSelf")
        Me.lblAtSelf.Name = "lblAtSelf"
        '
        'lblSelf
        '
        Me.lblSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblSelf, "lblSelf")
        Me.lblSelf.Name = "lblSelf"
        '
        'ButtonBackToDefaultFontColor2
        '
        resources.ApplyResources(Me.ButtonBackToDefaultFontColor2, "ButtonBackToDefaultFontColor2")
        Me.ButtonBackToDefaultFontColor2.Name = "ButtonBackToDefaultFontColor2"
        Me.ButtonBackToDefaultFontColor2.UseVisualStyleBackColor = True
        '
        'ConnectionPanel
        '
        Me.ConnectionPanel.Controls.Add(Me.CheckEnableBasicAuth)
        Me.ConnectionPanel.Controls.Add(Me.TwitterSearchAPIText)
        Me.ConnectionPanel.Controls.Add(Me.Label31)
        Me.ConnectionPanel.Controls.Add(Me.TwitterAPIText)
        Me.ConnectionPanel.Controls.Add(Me.Label8)
        Me.ConnectionPanel.Controls.Add(Me.CheckUseSsl)
        Me.ConnectionPanel.Controls.Add(Me.Label64)
        Me.ConnectionPanel.Controls.Add(Me.ConnectionTimeOut)
        Me.ConnectionPanel.Controls.Add(Me.Label63)
        resources.ApplyResources(Me.ConnectionPanel, "ConnectionPanel")
        Me.ConnectionPanel.Name = "ConnectionPanel"
        '
        'CheckEnableBasicAuth
        '
        resources.ApplyResources(Me.CheckEnableBasicAuth, "CheckEnableBasicAuth")
        Me.CheckEnableBasicAuth.Name = "CheckEnableBasicAuth"
        Me.CheckEnableBasicAuth.UseVisualStyleBackColor = True
        '
        'TwitterSearchAPIText
        '
        resources.ApplyResources(Me.TwitterSearchAPIText, "TwitterSearchAPIText")
        Me.TwitterSearchAPIText.Name = "TwitterSearchAPIText"
        '
        'Label31
        '
        resources.ApplyResources(Me.Label31, "Label31")
        Me.Label31.Name = "Label31"
        '
        'TwitterAPIText
        '
        resources.ApplyResources(Me.TwitterAPIText, "TwitterAPIText")
        Me.TwitterAPIText.Name = "TwitterAPIText"
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'CheckUseSsl
        '
        resources.ApplyResources(Me.CheckUseSsl, "CheckUseSsl")
        Me.CheckUseSsl.Name = "CheckUseSsl"
        Me.CheckUseSsl.UseVisualStyleBackColor = True
        '
        'Label64
        '
        resources.ApplyResources(Me.Label64, "Label64")
        Me.Label64.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label64.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label64.Name = "Label64"
        '
        'ConnectionTimeOut
        '
        resources.ApplyResources(Me.ConnectionTimeOut, "ConnectionTimeOut")
        Me.ConnectionTimeOut.Name = "ConnectionTimeOut"
        '
        'Label63
        '
        resources.ApplyResources(Me.Label63, "Label63")
        Me.Label63.Name = "Label63"
        '
        'ProxyPanel
        '
        Me.ProxyPanel.Controls.Add(Me.Label55)
        Me.ProxyPanel.Controls.Add(Me.TextProxyPassword)
        Me.ProxyPanel.Controls.Add(Me.RadioProxyNone)
        Me.ProxyPanel.Controls.Add(Me.LabelProxyPassword)
        Me.ProxyPanel.Controls.Add(Me.RadioProxyIE)
        Me.ProxyPanel.Controls.Add(Me.TextProxyUser)
        Me.ProxyPanel.Controls.Add(Me.RadioProxySpecified)
        Me.ProxyPanel.Controls.Add(Me.LabelProxyUser)
        Me.ProxyPanel.Controls.Add(Me.LabelProxyAddress)
        Me.ProxyPanel.Controls.Add(Me.TextProxyPort)
        Me.ProxyPanel.Controls.Add(Me.TextProxyAddress)
        Me.ProxyPanel.Controls.Add(Me.LabelProxyPort)
        resources.ApplyResources(Me.ProxyPanel, "ProxyPanel")
        Me.ProxyPanel.Name = "ProxyPanel"
        '
        'Label55
        '
        resources.ApplyResources(Me.Label55, "Label55")
        Me.Label55.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label55.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label55.Name = "Label55"
        '
        'TextProxyPassword
        '
        resources.ApplyResources(Me.TextProxyPassword, "TextProxyPassword")
        Me.TextProxyPassword.Name = "TextProxyPassword"
        Me.TextProxyPassword.UseSystemPasswordChar = True
        '
        'RadioProxyNone
        '
        resources.ApplyResources(Me.RadioProxyNone, "RadioProxyNone")
        Me.RadioProxyNone.Name = "RadioProxyNone"
        Me.RadioProxyNone.UseVisualStyleBackColor = True
        '
        'LabelProxyPassword
        '
        resources.ApplyResources(Me.LabelProxyPassword, "LabelProxyPassword")
        Me.LabelProxyPassword.Name = "LabelProxyPassword"
        '
        'RadioProxyIE
        '
        resources.ApplyResources(Me.RadioProxyIE, "RadioProxyIE")
        Me.RadioProxyIE.Checked = True
        Me.RadioProxyIE.Name = "RadioProxyIE"
        Me.RadioProxyIE.TabStop = True
        Me.RadioProxyIE.UseVisualStyleBackColor = True
        '
        'TextProxyUser
        '
        resources.ApplyResources(Me.TextProxyUser, "TextProxyUser")
        Me.TextProxyUser.Name = "TextProxyUser"
        '
        'RadioProxySpecified
        '
        resources.ApplyResources(Me.RadioProxySpecified, "RadioProxySpecified")
        Me.RadioProxySpecified.Name = "RadioProxySpecified"
        Me.RadioProxySpecified.UseVisualStyleBackColor = True
        '
        'LabelProxyUser
        '
        resources.ApplyResources(Me.LabelProxyUser, "LabelProxyUser")
        Me.LabelProxyUser.Name = "LabelProxyUser"
        '
        'LabelProxyAddress
        '
        resources.ApplyResources(Me.LabelProxyAddress, "LabelProxyAddress")
        Me.LabelProxyAddress.Name = "LabelProxyAddress"
        '
        'TextProxyPort
        '
        resources.ApplyResources(Me.TextProxyPort, "TextProxyPort")
        Me.TextProxyPort.Name = "TextProxyPort"
        '
        'TextProxyAddress
        '
        resources.ApplyResources(Me.TextProxyAddress, "TextProxyAddress")
        Me.TextProxyAddress.Name = "TextProxyAddress"
        '
        'LabelProxyPort
        '
        resources.ApplyResources(Me.LabelProxyPort, "LabelProxyPort")
        Me.LabelProxyPort.Name = "LabelProxyPort"
        '
        'CooperatePanel
        '
        Me.CooperatePanel.Controls.Add(Me.ComboBoxTranslateLanguage)
        Me.CooperatePanel.Controls.Add(Me.Label29)
        Me.CooperatePanel.Controls.Add(Me.CheckOutputz)
        Me.CooperatePanel.Controls.Add(Me.CheckNicoms)
        Me.CooperatePanel.Controls.Add(Me.TextBoxOutputzKey)
        Me.CooperatePanel.Controls.Add(Me.Label60)
        Me.CooperatePanel.Controls.Add(Me.Label59)
        Me.CooperatePanel.Controls.Add(Me.ComboBoxOutputzUrlmode)
        resources.ApplyResources(Me.CooperatePanel, "CooperatePanel")
        Me.CooperatePanel.Name = "CooperatePanel"
        '
        'ComboBoxTranslateLanguage
        '
        Me.ComboBoxTranslateLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxTranslateLanguage.FormattingEnabled = True
        Me.ComboBoxTranslateLanguage.Items.AddRange(New Object() {resources.GetString("ComboBoxTranslateLanguage.Items"), resources.GetString("ComboBoxTranslateLanguage.Items1"), resources.GetString("ComboBoxTranslateLanguage.Items2"), resources.GetString("ComboBoxTranslateLanguage.Items3"), resources.GetString("ComboBoxTranslateLanguage.Items4"), resources.GetString("ComboBoxTranslateLanguage.Items5"), resources.GetString("ComboBoxTranslateLanguage.Items6"), resources.GetString("ComboBoxTranslateLanguage.Items7"), resources.GetString("ComboBoxTranslateLanguage.Items8"), resources.GetString("ComboBoxTranslateLanguage.Items9"), resources.GetString("ComboBoxTranslateLanguage.Items10"), resources.GetString("ComboBoxTranslateLanguage.Items11"), resources.GetString("ComboBoxTranslateLanguage.Items12"), resources.GetString("ComboBoxTranslateLanguage.Items13"), resources.GetString("ComboBoxTranslateLanguage.Items14"), resources.GetString("ComboBoxTranslateLanguage.Items15"), resources.GetString("ComboBoxTranslateLanguage.Items16"), resources.GetString("ComboBoxTranslateLanguage.Items17"), resources.GetString("ComboBoxTranslateLanguage.Items18"), resources.GetString("ComboBoxTranslateLanguage.Items19"), resources.GetString("ComboBoxTranslateLanguage.Items20"), resources.GetString("ComboBoxTranslateLanguage.Items21"), resources.GetString("ComboBoxTranslateLanguage.Items22"), resources.GetString("ComboBoxTranslateLanguage.Items23"), resources.GetString("ComboBoxTranslateLanguage.Items24"), resources.GetString("ComboBoxTranslateLanguage.Items25"), resources.GetString("ComboBoxTranslateLanguage.Items26"), resources.GetString("ComboBoxTranslateLanguage.Items27"), resources.GetString("ComboBoxTranslateLanguage.Items28"), resources.GetString("ComboBoxTranslateLanguage.Items29"), resources.GetString("ComboBoxTranslateLanguage.Items30"), resources.GetString("ComboBoxTranslateLanguage.Items31"), resources.GetString("ComboBoxTranslateLanguage.Items32"), resources.GetString("ComboBoxTranslateLanguage.Items33"), resources.GetString("ComboBoxTranslateLanguage.Items34"), resources.GetString("ComboBoxTranslateLanguage.Items35"), resources.GetString("ComboBoxTranslateLanguage.Items36"), resources.GetString("ComboBoxTranslateLanguage.Items37"), resources.GetString("ComboBoxTranslateLanguage.Items38"), resources.GetString("ComboBoxTranslateLanguage.Items39"), resources.GetString("ComboBoxTranslateLanguage.Items40"), resources.GetString("ComboBoxTranslateLanguage.Items41"), resources.GetString("ComboBoxTranslateLanguage.Items42"), resources.GetString("ComboBoxTranslateLanguage.Items43"), resources.GetString("ComboBoxTranslateLanguage.Items44"), resources.GetString("ComboBoxTranslateLanguage.Items45"), resources.GetString("ComboBoxTranslateLanguage.Items46"), resources.GetString("ComboBoxTranslateLanguage.Items47"), resources.GetString("ComboBoxTranslateLanguage.Items48"), resources.GetString("ComboBoxTranslateLanguage.Items49"), resources.GetString("ComboBoxTranslateLanguage.Items50"), resources.GetString("ComboBoxTranslateLanguage.Items51"), resources.GetString("ComboBoxTranslateLanguage.Items52"), resources.GetString("ComboBoxTranslateLanguage.Items53"), resources.GetString("ComboBoxTranslateLanguage.Items54"), resources.GetString("ComboBoxTranslateLanguage.Items55"), resources.GetString("ComboBoxTranslateLanguage.Items56"), resources.GetString("ComboBoxTranslateLanguage.Items57"), resources.GetString("ComboBoxTranslateLanguage.Items58"), resources.GetString("ComboBoxTranslateLanguage.Items59"), resources.GetString("ComboBoxTranslateLanguage.Items60"), resources.GetString("ComboBoxTranslateLanguage.Items61"), resources.GetString("ComboBoxTranslateLanguage.Items62"), resources.GetString("ComboBoxTranslateLanguage.Items63"), resources.GetString("ComboBoxTranslateLanguage.Items64"), resources.GetString("ComboBoxTranslateLanguage.Items65"), resources.GetString("ComboBoxTranslateLanguage.Items66"), resources.GetString("ComboBoxTranslateLanguage.Items67"), resources.GetString("ComboBoxTranslateLanguage.Items68"), resources.GetString("ComboBoxTranslateLanguage.Items69"), resources.GetString("ComboBoxTranslateLanguage.Items70"), resources.GetString("ComboBoxTranslateLanguage.Items71"), resources.GetString("ComboBoxTranslateLanguage.Items72"), resources.GetString("ComboBoxTranslateLanguage.Items73"), resources.GetString("ComboBoxTranslateLanguage.Items74"), resources.GetString("ComboBoxTranslateLanguage.Items75"), resources.GetString("ComboBoxTranslateLanguage.Items76"), resources.GetString("ComboBoxTranslateLanguage.Items77"), resources.GetString("ComboBoxTranslateLanguage.Items78"), resources.GetString("ComboBoxTranslateLanguage.Items79"), resources.GetString("ComboBoxTranslateLanguage.Items80"), resources.GetString("ComboBoxTranslateLanguage.Items81"), resources.GetString("ComboBoxTranslateLanguage.Items82"), resources.GetString("ComboBoxTranslateLanguage.Items83"), resources.GetString("ComboBoxTranslateLanguage.Items84"), resources.GetString("ComboBoxTranslateLanguage.Items85"), resources.GetString("ComboBoxTranslateLanguage.Items86"), resources.GetString("ComboBoxTranslateLanguage.Items87"), resources.GetString("ComboBoxTranslateLanguage.Items88"), resources.GetString("ComboBoxTranslateLanguage.Items89"), resources.GetString("ComboBoxTranslateLanguage.Items90"), resources.GetString("ComboBoxTranslateLanguage.Items91"), resources.GetString("ComboBoxTranslateLanguage.Items92"), resources.GetString("ComboBoxTranslateLanguage.Items93"), resources.GetString("ComboBoxTranslateLanguage.Items94"), resources.GetString("ComboBoxTranslateLanguage.Items95"), resources.GetString("ComboBoxTranslateLanguage.Items96"), resources.GetString("ComboBoxTranslateLanguage.Items97"), resources.GetString("ComboBoxTranslateLanguage.Items98"), resources.GetString("ComboBoxTranslateLanguage.Items99"), resources.GetString("ComboBoxTranslateLanguage.Items100"), resources.GetString("ComboBoxTranslateLanguage.Items101"), resources.GetString("ComboBoxTranslateLanguage.Items102"), resources.GetString("ComboBoxTranslateLanguage.Items103"), resources.GetString("ComboBoxTranslateLanguage.Items104"), resources.GetString("ComboBoxTranslateLanguage.Items105"), resources.GetString("ComboBoxTranslateLanguage.Items106")})
        resources.ApplyResources(Me.ComboBoxTranslateLanguage, "ComboBoxTranslateLanguage")
        Me.ComboBoxTranslateLanguage.Name = "ComboBoxTranslateLanguage"
        '
        'Label29
        '
        resources.ApplyResources(Me.Label29, "Label29")
        Me.Label29.Name = "Label29"
        '
        'CheckOutputz
        '
        resources.ApplyResources(Me.CheckOutputz, "CheckOutputz")
        Me.CheckOutputz.Name = "CheckOutputz"
        Me.CheckOutputz.UseVisualStyleBackColor = True
        '
        'CheckNicoms
        '
        resources.ApplyResources(Me.CheckNicoms, "CheckNicoms")
        Me.CheckNicoms.Name = "CheckNicoms"
        Me.CheckNicoms.UseVisualStyleBackColor = True
        '
        'TextBoxOutputzKey
        '
        resources.ApplyResources(Me.TextBoxOutputzKey, "TextBoxOutputzKey")
        Me.TextBoxOutputzKey.Name = "TextBoxOutputzKey"
        '
        'Label60
        '
        resources.ApplyResources(Me.Label60, "Label60")
        Me.Label60.Name = "Label60"
        '
        'Label59
        '
        resources.ApplyResources(Me.Label59, "Label59")
        Me.Label59.Name = "Label59"
        '
        'ComboBoxOutputzUrlmode
        '
        Me.ComboBoxOutputzUrlmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxOutputzUrlmode.FormattingEnabled = True
        Me.ComboBoxOutputzUrlmode.Items.AddRange(New Object() {resources.GetString("ComboBoxOutputzUrlmode.Items"), resources.GetString("ComboBoxOutputzUrlmode.Items1")})
        resources.ApplyResources(Me.ComboBoxOutputzUrlmode, "ComboBoxOutputzUrlmode")
        Me.ComboBoxOutputzUrlmode.Name = "ComboBoxOutputzUrlmode"
        '
        'ShortUrlPanel
        '
        Me.ShortUrlPanel.Controls.Add(Me.CheckTinyURL)
        Me.ShortUrlPanel.Controls.Add(Me.TextBitlyPw)
        Me.ShortUrlPanel.Controls.Add(Me.CheckAutoConvertUrl)
        Me.ShortUrlPanel.Controls.Add(Me.Label71)
        Me.ShortUrlPanel.Controls.Add(Me.ComboBoxAutoShortUrlFirst)
        Me.ShortUrlPanel.Controls.Add(Me.Label76)
        Me.ShortUrlPanel.Controls.Add(Me.Label77)
        Me.ShortUrlPanel.Controls.Add(Me.TextBitlyId)
        resources.ApplyResources(Me.ShortUrlPanel, "ShortUrlPanel")
        Me.ShortUrlPanel.Name = "ShortUrlPanel"
        '
        'CheckTinyURL
        '
        resources.ApplyResources(Me.CheckTinyURL, "CheckTinyURL")
        Me.CheckTinyURL.Name = "CheckTinyURL"
        Me.CheckTinyURL.UseVisualStyleBackColor = True
        '
        'TextBitlyPw
        '
        resources.ApplyResources(Me.TextBitlyPw, "TextBitlyPw")
        Me.TextBitlyPw.Name = "TextBitlyPw"
        '
        'CheckAutoConvertUrl
        '
        resources.ApplyResources(Me.CheckAutoConvertUrl, "CheckAutoConvertUrl")
        Me.CheckAutoConvertUrl.Name = "CheckAutoConvertUrl"
        Me.CheckAutoConvertUrl.UseVisualStyleBackColor = True
        '
        'Label71
        '
        resources.ApplyResources(Me.Label71, "Label71")
        Me.Label71.Name = "Label71"
        '
        'ComboBoxAutoShortUrlFirst
        '
        Me.ComboBoxAutoShortUrlFirst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxAutoShortUrlFirst.FormattingEnabled = True
        Me.ComboBoxAutoShortUrlFirst.Items.AddRange(New Object() {resources.GetString("ComboBoxAutoShortUrlFirst.Items"), resources.GetString("ComboBoxAutoShortUrlFirst.Items1"), resources.GetString("ComboBoxAutoShortUrlFirst.Items2"), resources.GetString("ComboBoxAutoShortUrlFirst.Items3"), resources.GetString("ComboBoxAutoShortUrlFirst.Items4"), resources.GetString("ComboBoxAutoShortUrlFirst.Items5")})
        resources.ApplyResources(Me.ComboBoxAutoShortUrlFirst, "ComboBoxAutoShortUrlFirst")
        Me.ComboBoxAutoShortUrlFirst.Name = "ComboBoxAutoShortUrlFirst"
        '
        'Label76
        '
        resources.ApplyResources(Me.Label76, "Label76")
        Me.Label76.Name = "Label76"
        '
        'Label77
        '
        resources.ApplyResources(Me.Label77, "Label77")
        Me.Label77.Name = "Label77"
        '
        'TextBitlyId
        '
        resources.ApplyResources(Me.TextBitlyId, "TextBitlyId")
        Me.TextBitlyId.Name = "TextBitlyId"
        '
        'ActionPanel
        '
        Me.ActionPanel.Controls.Add(Me.CheckOpenUserTimeline)
        Me.ActionPanel.Controls.Add(Me.GroupBox3)
        Me.ActionPanel.Controls.Add(Me.Label57)
        Me.ActionPanel.Controls.Add(Me.CheckFavRestrict)
        Me.ActionPanel.Controls.Add(Me.Button3)
        Me.ActionPanel.Controls.Add(Me.PlaySnd)
        Me.ActionPanel.Controls.Add(Me.chkReadOwnPost)
        Me.ActionPanel.Controls.Add(Me.Label15)
        Me.ActionPanel.Controls.Add(Me.BrowserPathText)
        Me.ActionPanel.Controls.Add(Me.UReadMng)
        Me.ActionPanel.Controls.Add(Me.Label44)
        Me.ActionPanel.Controls.Add(Me.CheckCloseToExit)
        Me.ActionPanel.Controls.Add(Me.CheckMinimizeToTray)
        Me.ActionPanel.Controls.Add(Me.CheckReadOldPosts)
        resources.ApplyResources(Me.ActionPanel, "ActionPanel")
        Me.ActionPanel.Name = "ActionPanel"
        '
        'CheckOpenUserTimeline
        '
        resources.ApplyResources(Me.CheckOpenUserTimeline, "CheckOpenUserTimeline")
        Me.CheckOpenUserTimeline.Name = "CheckOpenUserTimeline"
        Me.CheckOpenUserTimeline.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.HotkeyCheck)
        Me.GroupBox3.Controls.Add(Me.HotkeyCode)
        Me.GroupBox3.Controls.Add(Me.HotkeyText)
        Me.GroupBox3.Controls.Add(Me.HotkeyWin)
        Me.GroupBox3.Controls.Add(Me.HotkeyAlt)
        Me.GroupBox3.Controls.Add(Me.HotkeyShift)
        Me.GroupBox3.Controls.Add(Me.HotkeyCtrl)
        resources.ApplyResources(Me.GroupBox3, "GroupBox3")
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.TabStop = False
        '
        'HotkeyCheck
        '
        resources.ApplyResources(Me.HotkeyCheck, "HotkeyCheck")
        Me.HotkeyCheck.Name = "HotkeyCheck"
        Me.HotkeyCheck.UseVisualStyleBackColor = True
        '
        'HotkeyCode
        '
        resources.ApplyResources(Me.HotkeyCode, "HotkeyCode")
        Me.HotkeyCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.HotkeyCode.Name = "HotkeyCode"
        '
        'HotkeyText
        '
        resources.ApplyResources(Me.HotkeyText, "HotkeyText")
        Me.HotkeyText.Name = "HotkeyText"
        Me.HotkeyText.ReadOnly = True
        '
        'HotkeyWin
        '
        resources.ApplyResources(Me.HotkeyWin, "HotkeyWin")
        Me.HotkeyWin.Name = "HotkeyWin"
        Me.HotkeyWin.UseVisualStyleBackColor = True
        '
        'HotkeyAlt
        '
        resources.ApplyResources(Me.HotkeyAlt, "HotkeyAlt")
        Me.HotkeyAlt.Name = "HotkeyAlt"
        Me.HotkeyAlt.UseVisualStyleBackColor = True
        '
        'HotkeyShift
        '
        resources.ApplyResources(Me.HotkeyShift, "HotkeyShift")
        Me.HotkeyShift.Name = "HotkeyShift"
        Me.HotkeyShift.UseVisualStyleBackColor = True
        '
        'HotkeyCtrl
        '
        resources.ApplyResources(Me.HotkeyCtrl, "HotkeyCtrl")
        Me.HotkeyCtrl.Name = "HotkeyCtrl"
        Me.HotkeyCtrl.UseVisualStyleBackColor = True
        '
        'Label57
        '
        resources.ApplyResources(Me.Label57, "Label57")
        Me.Label57.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label57.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label57.Name = "Label57"
        '
        'CheckFavRestrict
        '
        resources.ApplyResources(Me.CheckFavRestrict, "CheckFavRestrict")
        Me.CheckFavRestrict.Name = "CheckFavRestrict"
        Me.CheckFavRestrict.UseVisualStyleBackColor = True
        '
        'Button3
        '
        resources.ApplyResources(Me.Button3, "Button3")
        Me.Button3.Name = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'PlaySnd
        '
        resources.ApplyResources(Me.PlaySnd, "PlaySnd")
        Me.PlaySnd.Name = "PlaySnd"
        Me.PlaySnd.UseVisualStyleBackColor = True
        '
        'chkReadOwnPost
        '
        resources.ApplyResources(Me.chkReadOwnPost, "chkReadOwnPost")
        Me.chkReadOwnPost.Name = "chkReadOwnPost"
        Me.chkReadOwnPost.UseVisualStyleBackColor = True
        '
        'Label15
        '
        Me.Label15.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label15.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        resources.ApplyResources(Me.Label15, "Label15")
        Me.Label15.Name = "Label15"
        '
        'BrowserPathText
        '
        resources.ApplyResources(Me.BrowserPathText, "BrowserPathText")
        Me.BrowserPathText.Name = "BrowserPathText"
        '
        'UReadMng
        '
        resources.ApplyResources(Me.UReadMng, "UReadMng")
        Me.UReadMng.Name = "UReadMng"
        Me.UReadMng.UseVisualStyleBackColor = True
        '
        'Label44
        '
        resources.ApplyResources(Me.Label44, "Label44")
        Me.Label44.Name = "Label44"
        '
        'CheckCloseToExit
        '
        resources.ApplyResources(Me.CheckCloseToExit, "CheckCloseToExit")
        Me.CheckCloseToExit.Name = "CheckCloseToExit"
        Me.CheckCloseToExit.UseVisualStyleBackColor = True
        '
        'CheckMinimizeToTray
        '
        resources.ApplyResources(Me.CheckMinimizeToTray, "CheckMinimizeToTray")
        Me.CheckMinimizeToTray.Name = "CheckMinimizeToTray"
        Me.CheckMinimizeToTray.UseVisualStyleBackColor = True
        '
        'CheckReadOldPosts
        '
        resources.ApplyResources(Me.CheckReadOldPosts, "CheckReadOldPosts")
        Me.CheckReadOldPosts.Name = "CheckReadOldPosts"
        Me.CheckReadOldPosts.UseVisualStyleBackColor = True
        '
        'Cancel
        '
        Me.Cancel.CausesValidation = False
        Me.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.Cancel, "Cancel")
        Me.Cancel.Name = "Cancel"
        Me.Cancel.UseVisualStyleBackColor = True
        '
        'Save
        '
        Me.Save.DialogResult = System.Windows.Forms.DialogResult.OK
        resources.ApplyResources(Me.Save, "Save")
        Me.Save.Name = "Save"
        Me.Save.UseVisualStyleBackColor = True
        '
        'AppendSettingDialog
        '
        Me.AcceptButton = Me.Save
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel
        Me.Controls.Add(Me.Cancel)
        Me.Controls.Add(Me.Save)
        Me.Controls.Add(Me.SplitContainer1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AppendSettingDialog"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.NotifyPanel.ResumeLayout(False)
        Me.NotifyPanel.PerformLayout()
        Me.GetPeriodPanel.ResumeLayout(False)
        Me.GetPeriodPanel.PerformLayout()
        Me.GetCountPanel.ResumeLayout(False)
        Me.GetCountPanel.PerformLayout()
        Me.BasedPanel.ResumeLayout(False)
        Me.BasedPanel.PerformLayout()
        Me.StartupPanel.ResumeLayout(False)
        Me.StartupPanel.PerformLayout()
        Me.UserStreamPanel.ResumeLayout(False)
        Me.UserStreamPanel.PerformLayout()
        Me.TweetActPanel.ResumeLayout(False)
        Me.TweetActPanel.PerformLayout()
        Me.PreviewPanel.ResumeLayout(False)
        Me.PreviewPanel.PerformLayout()
        Me.TweetPrvPanel.ResumeLayout(False)
        Me.TweetPrvPanel.PerformLayout()
        Me.FontPanel.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.FontPanel2.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.ConnectionPanel.ResumeLayout(False)
        Me.ConnectionPanel.PerformLayout()
        Me.ProxyPanel.ResumeLayout(False)
        Me.ProxyPanel.PerformLayout()
        Me.CooperatePanel.ResumeLayout(False)
        Me.CooperatePanel.PerformLayout()
        Me.ShortUrlPanel.ResumeLayout(False)
        Me.ShortUrlPanel.PerformLayout()
        Me.ActionPanel.ResumeLayout(False)
        Me.ActionPanel.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents TreeViewSetting As System.Windows.Forms.TreeView
    Friend WithEvents BasedPanel As System.Windows.Forms.Panel
    Friend WithEvents AuthBasicRadio As System.Windows.Forms.RadioButton
    Friend WithEvents AuthOAuthRadio As System.Windows.Forms.RadioButton
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents AuthClearButton As System.Windows.Forms.Button
    Friend WithEvents AuthUserLabel As System.Windows.Forms.Label
    Friend WithEvents AuthStateLabel As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents AuthorizeButton As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Username As System.Windows.Forms.TextBox
    Friend WithEvents Password As System.Windows.Forms.TextBox
    Friend WithEvents GetPeriodPanel As System.Windows.Forms.Panel
    Friend WithEvents TimelinePeriod As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents ButtonApiCalc As System.Windows.Forms.Button
    Friend WithEvents LabelPostAndGet As System.Windows.Forms.Label
    Friend WithEvents LabelApiUsing As System.Windows.Forms.Label
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents ListsPeriod As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents PubSearchPeriod As System.Windows.Forms.TextBox
    Friend WithEvents Label69 As System.Windows.Forms.Label
    Friend WithEvents ReplyPeriod As System.Windows.Forms.TextBox
    Friend WithEvents CheckPostAndGet As System.Windows.Forms.CheckBox
    Friend WithEvents CheckPeriodAdjust As System.Windows.Forms.CheckBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents DMPeriod As System.Windows.Forms.TextBox
    Friend WithEvents StartupPanel As System.Windows.Forms.Panel
    Friend WithEvents GetCountPanel As System.Windows.Forms.Panel
    Friend WithEvents StartupReaded As System.Windows.Forms.CheckBox
    Friend WithEvents CheckStartupFollowers As System.Windows.Forms.CheckBox
    Friend WithEvents CheckStartupVersion As System.Windows.Forms.CheckBox
    Friend WithEvents chkGetFav As System.Windows.Forms.CheckBox
    Friend WithEvents TextCountApiReply As System.Windows.Forms.TextBox
    Friend WithEvents Label67 As System.Windows.Forms.Label
    Friend WithEvents TextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents FavoritesTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents SearchTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents Label66 As System.Windows.Forms.Label
    Friend WithEvents FirstTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents GetMoreTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents Label53 As System.Windows.Forms.Label
    Friend WithEvents UseChangeGetCount As System.Windows.Forms.CheckBox
    Friend WithEvents ActionPanel As System.Windows.Forms.Panel
    Friend WithEvents PreviewPanel As System.Windows.Forms.Panel
    Friend WithEvents FontPanel As System.Windows.Forms.Panel
    Friend WithEvents ConnectionPanel As System.Windows.Forms.Panel
    Friend WithEvents ProxyPanel As System.Windows.Forms.Panel
    Friend WithEvents ComboBoxPostKeySelect As System.Windows.Forms.ComboBox
    Friend WithEvents CheckRetweetNoConfirm As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents HotkeyCheck As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyCode As System.Windows.Forms.Label
    Friend WithEvents HotkeyText As System.Windows.Forms.TextBox
    Friend WithEvents HotkeyWin As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyAlt As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyShift As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyCtrl As System.Windows.Forms.CheckBox
    Friend WithEvents Label77 As System.Windows.Forms.Label
    Friend WithEvents TextBitlyId As System.Windows.Forms.TextBox
    Friend WithEvents Label76 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxAutoShortUrlFirst As System.Windows.Forms.ComboBox
    Friend WithEvents Label71 As System.Windows.Forms.Label
    Friend WithEvents CheckAutoConvertUrl As System.Windows.Forms.CheckBox
    Friend WithEvents Label57 As System.Windows.Forms.Label
    Friend WithEvents CheckFavRestrict As System.Windows.Forms.CheckBox
    Friend WithEvents CheckTinyURL As System.Windows.Forms.CheckBox
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents PlaySnd As System.Windows.Forms.CheckBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents BrowserPathText As System.Windows.Forms.TextBox
    Friend WithEvents UReadMng As System.Windows.Forms.CheckBox
    Friend WithEvents Label44 As System.Windows.Forms.Label
    Friend WithEvents CheckCloseToExit As System.Windows.Forms.CheckBox
    Friend WithEvents CheckMinimizeToTray As System.Windows.Forms.CheckBox
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents CheckReadOldPosts As System.Windows.Forms.CheckBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents StatusText As System.Windows.Forms.TextBox
    Friend WithEvents CheckUseRecommendStatus As System.Windows.Forms.CheckBox
    Friend WithEvents TweetActPanel As System.Windows.Forms.Panel
    Friend WithEvents CheckPreviewEnable As System.Windows.Forms.CheckBox
    Friend WithEvents Label81 As System.Windows.Forms.Label
    Friend WithEvents LanguageCombo As System.Windows.Forms.ComboBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents CheckAlwaysTop As System.Windows.Forms.CheckBox
    Friend WithEvents CheckMonospace As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBalloonLimit As System.Windows.Forms.CheckBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents ComboDispTitle As System.Windows.Forms.ComboBox
    Friend WithEvents Label45 As System.Windows.Forms.Label
    Friend WithEvents cmbNameBalloon As System.Windows.Forms.ComboBox
    Friend WithEvents CheckDispUsername As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox3 As System.Windows.Forms.CheckBox
    Friend WithEvents TweetPrvPanel As System.Windows.Forms.Panel
    Friend WithEvents CheckSortOrderLock As System.Windows.Forms.CheckBox
    Friend WithEvents CheckShowGrid As System.Windows.Forms.CheckBox
    Friend WithEvents chkReadOwnPost As System.Windows.Forms.CheckBox
    Friend WithEvents chkUnreadStyle As System.Windows.Forms.CheckBox
    Friend WithEvents OneWayLv As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnRetweet As System.Windows.Forms.Button
    Friend WithEvents lblRetweet As System.Windows.Forms.Label
    Friend WithEvents Label80 As System.Windows.Forms.Label
    Friend WithEvents ButtonBackToDefaultFontColor As System.Windows.Forms.Button
    Friend WithEvents btnDetailLink As System.Windows.Forms.Button
    Friend WithEvents lblDetailLink As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents btnUnread As System.Windows.Forms.Button
    Friend WithEvents lblUnread As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents btnDetailBack As System.Windows.Forms.Button
    Friend WithEvents lblDetailBackcolor As System.Windows.Forms.Label
    Friend WithEvents Label37 As System.Windows.Forms.Label
    Friend WithEvents btnDetail As System.Windows.Forms.Button
    Friend WithEvents lblDetail As System.Windows.Forms.Label
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents btnOWL As System.Windows.Forms.Button
    Friend WithEvents lblOWL As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents btnFav As System.Windows.Forms.Button
    Friend WithEvents lblFav As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents btnListFont As System.Windows.Forms.Button
    Friend WithEvents lblListFont As System.Windows.Forms.Label
    Friend WithEvents Label61 As System.Windows.Forms.Label
    Friend WithEvents FontDialog1 As System.Windows.Forms.FontDialog
    Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
    Friend WithEvents CheckEnableBasicAuth As System.Windows.Forms.CheckBox
    Friend WithEvents TwitterSearchAPIText As System.Windows.Forms.TextBox
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents TwitterAPIText As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents CheckUseSsl As System.Windows.Forms.CheckBox
    Friend WithEvents Label64 As System.Windows.Forms.Label
    Friend WithEvents ConnectionTimeOut As System.Windows.Forms.TextBox
    Friend WithEvents Label63 As System.Windows.Forms.Label
    Friend WithEvents Label55 As System.Windows.Forms.Label
    Friend WithEvents TextProxyPassword As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyPassword As System.Windows.Forms.Label
    Friend WithEvents TextProxyUser As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyUser As System.Windows.Forms.Label
    Friend WithEvents TextProxyPort As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyPort As System.Windows.Forms.Label
    Friend WithEvents TextProxyAddress As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyAddress As System.Windows.Forms.Label
    Friend WithEvents RadioProxySpecified As System.Windows.Forms.RadioButton
    Friend WithEvents RadioProxyIE As System.Windows.Forms.RadioButton
    Friend WithEvents RadioProxyNone As System.Windows.Forms.RadioButton
    Friend WithEvents FontPanel2 As System.Windows.Forms.Panel
    Friend WithEvents CheckNicoms As System.Windows.Forms.CheckBox
    Friend WithEvents Label60 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxOutputzUrlmode As System.Windows.Forms.ComboBox
    Friend WithEvents Label59 As System.Windows.Forms.Label
    Friend WithEvents TextBoxOutputzKey As System.Windows.Forms.TextBox
    Friend WithEvents CheckOutputz As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents ButtonBackToDefaultFontColor2 As System.Windows.Forms.Button
    Friend WithEvents Cancel As System.Windows.Forms.Button
    Friend WithEvents Save As System.Windows.Forms.Button
    Friend WithEvents TextBitlyPw As System.Windows.Forms.TextBox
    Friend WithEvents lblInputFont As System.Windows.Forms.Label
    Friend WithEvents lblInputBackcolor As System.Windows.Forms.Label
    Friend WithEvents lblAtTo As System.Windows.Forms.Label
    Friend WithEvents lblListBackcolor As System.Windows.Forms.Label
    Friend WithEvents lblAtFromTarget As System.Windows.Forms.Label
    Friend WithEvents lblAtTarget As System.Windows.Forms.Label
    Friend WithEvents lblTarget As System.Windows.Forms.Label
    Friend WithEvents lblAtSelf As System.Windows.Forms.Label
    Friend WithEvents lblSelf As System.Windows.Forms.Label
    Friend WithEvents btnInputFont As System.Windows.Forms.Button
    Friend WithEvents btnInputBackcolor As System.Windows.Forms.Button
    Friend WithEvents btnAtTo As System.Windows.Forms.Button
    Friend WithEvents btnListBack As System.Windows.Forms.Button
    Friend WithEvents btnAtFromTarget As System.Windows.Forms.Button
    Friend WithEvents btnAtTarget As System.Windows.Forms.Button
    Friend WithEvents btnTarget As System.Windows.Forms.Button
    Friend WithEvents btnAtSelf As System.Windows.Forms.Button
    Friend WithEvents btnSelf As System.Windows.Forms.Button
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label47 As System.Windows.Forms.Label
    Friend WithEvents LabelDateTimeFormatApplied As System.Windows.Forms.Label
    Friend WithEvents Label62 As System.Windows.Forms.Label
    Friend WithEvents CmbDateTimeFormat As System.Windows.Forms.ComboBox
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents IconSize As System.Windows.Forms.ComboBox
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents ReplyIconStateCombo As System.Windows.Forms.ComboBox
    Friend WithEvents Label72 As System.Windows.Forms.Label
    Friend WithEvents ChkNewMentionsBlink As System.Windows.Forms.CheckBox
    Friend WithEvents chkTabIconDisp As System.Windows.Forms.CheckBox
    Friend WithEvents UserStreamPanel As System.Windows.Forms.Panel
    Friend WithEvents UserstreamPeriod As System.Windows.Forms.TextBox
    Friend WithEvents StartupUserstreamCheck As System.Windows.Forms.CheckBox
    Friend WithEvents Label83 As System.Windows.Forms.Label
    Friend WithEvents Label65 As System.Windows.Forms.Label
    Friend WithEvents Label52 As System.Windows.Forms.Label
    Friend WithEvents Label49 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents Label36 As System.Windows.Forms.Label
    Friend WithEvents CooperatePanel As System.Windows.Forms.Panel
    Friend WithEvents ShortUrlPanel As System.Windows.Forms.Panel
    Friend WithEvents UserTimelineTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents UserTimelinePeriod As System.Windows.Forms.TextBox
    Friend WithEvents CheckOpenUserTimeline As System.Windows.Forms.CheckBox
    Friend WithEvents CheckHashSupple As System.Windows.Forms.CheckBox
    Friend WithEvents CheckAtIdSupple As System.Windows.Forms.CheckBox
    Friend WithEvents NotifyPanel As System.Windows.Forms.Panel
    Friend WithEvents CheckEventNotify As System.Windows.Forms.CheckBox
    Friend WithEvents CheckUnfavoritesEvent As System.Windows.Forms.CheckBox
    Friend WithEvents CheckFavoritesEvent As System.Windows.Forms.CheckBox
    Friend WithEvents CheckFollowEvent As System.Windows.Forms.CheckBox
    Friend WithEvents CheckListMemberRemovedEvent As System.Windows.Forms.CheckBox
    Friend WithEvents CheckListMemberAddedEvent As System.Windows.Forms.CheckBox
    Friend WithEvents CheckForceEventNotify As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBlockEvent As System.Windows.Forms.CheckBox
    Friend WithEvents ListTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents CheckListCreatedEvent As System.Windows.Forms.CheckBox
    Friend WithEvents CheckFavEventUnread As System.Windows.Forms.CheckBox
    Friend WithEvents ComboBoxTranslateLanguage As System.Windows.Forms.ComboBox
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxEventNotifySound As System.Windows.Forms.ComboBox
    Friend WithEvents Label35 As System.Windows.Forms.Label
    Friend WithEvents LabelUserStreamActive As System.Windows.Forms.Label
    Friend WithEvents LabelApiUsingUserStreamEnabled As System.Windows.Forms.Label
    Friend WithEvents CheckUserUpdateEvent As System.Windows.Forms.CheckBox
End Class
