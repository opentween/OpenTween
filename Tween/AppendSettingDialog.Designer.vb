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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AppendSettingDialog))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.TreeViewSetting = New System.Windows.Forms.TreeView()
        Me.ActionPanel = New System.Windows.Forms.Panel()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.ListDoubleClickActionComboBox = New System.Windows.Forms.ComboBox()
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
        Me.CooperatePanel = New System.Windows.Forms.Panel()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.UserAppointUrlText = New System.Windows.Forms.TextBox()
        Me.ComboBoxTranslateLanguage = New System.Windows.Forms.ComboBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.CheckOutputz = New System.Windows.Forms.CheckBox()
        Me.CheckNicoms = New System.Windows.Forms.CheckBox()
        Me.TextBoxOutputzKey = New System.Windows.Forms.TextBox()
        Me.Label60 = New System.Windows.Forms.Label()
        Me.Label59 = New System.Windows.Forms.Label()
        Me.ComboBoxOutputzUrlmode = New System.Windows.Forms.ComboBox()
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
        Me.TweetPrvPanel = New System.Windows.Forms.Panel()
        Me.HideDuplicatedRetweetsCheck = New System.Windows.Forms.CheckBox()
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
        Me.UserstreamPeriod = New System.Windows.Forms.TextBox()
        Me.StartupUserstreamCheck = New System.Windows.Forms.CheckBox()
        Me.Label83 = New System.Windows.Forms.Label()
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
        Me.ShortUrlPanel = New System.Windows.Forms.Panel()
        Me.CheckTinyURL = New System.Windows.Forms.CheckBox()
        Me.TextBitlyPw = New System.Windows.Forms.TextBox()
        Me.CheckAutoConvertUrl = New System.Windows.Forms.CheckBox()
        Me.Label71 = New System.Windows.Forms.Label()
        Me.ComboBoxAutoShortUrlFirst = New System.Windows.Forms.ComboBox()
        Me.Label76 = New System.Windows.Forms.Label()
        Me.Label77 = New System.Windows.Forms.Label()
        Me.TextBitlyId = New System.Windows.Forms.TextBox()
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
        Me.TweetActPanel = New System.Windows.Forms.Panel()
        Me.CheckHashSupple = New System.Windows.Forms.CheckBox()
        Me.CheckAtIdSupple = New System.Windows.Forms.CheckBox()
        Me.ComboBoxPostKeySelect = New System.Windows.Forms.ComboBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.CheckRetweetNoConfirm = New System.Windows.Forms.CheckBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.CheckUseRecommendStatus = New System.Windows.Forms.CheckBox()
        Me.StatusText = New System.Windows.Forms.TextBox()
        Me.StartupPanel = New System.Windows.Forms.Panel()
        Me.StartupReaded = New System.Windows.Forms.CheckBox()
        Me.CheckStartupFollowers = New System.Windows.Forms.CheckBox()
        Me.CheckStartupVersion = New System.Windows.Forms.CheckBox()
        Me.chkGetFav = New System.Windows.Forms.CheckBox()
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
        Me.FontDialog1 = New System.Windows.Forms.FontDialog()
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
        Me.Cancel = New System.Windows.Forms.Button()
        Me.Save = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ActionPanel.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.CooperatePanel.SuspendLayout()
        Me.NotifyPanel.SuspendLayout()
        Me.TweetPrvPanel.SuspendLayout()
        Me.GetPeriodPanel.SuspendLayout()
        Me.PreviewPanel.SuspendLayout()
        Me.BasedPanel.SuspendLayout()
        Me.FontPanel.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.ShortUrlPanel.SuspendLayout()
        Me.ProxyPanel.SuspendLayout()
        Me.ConnectionPanel.SuspendLayout()
        Me.FontPanel2.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.TweetActPanel.SuspendLayout()
        Me.StartupPanel.SuspendLayout()
        Me.GetCountPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        resources.ApplyResources(Me.SplitContainer1.Panel1, "SplitContainer1.Panel1")
        Me.SplitContainer1.Panel1.Controls.Add(Me.TreeViewSetting)
        Me.ToolTip1.SetToolTip(Me.SplitContainer1.Panel1, resources.GetString("SplitContainer1.Panel1.ToolTip"))
        '
        'SplitContainer1.Panel2
        '
        resources.ApplyResources(Me.SplitContainer1.Panel2, "SplitContainer1.Panel2")
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control
        Me.SplitContainer1.Panel2.Controls.Add(Me.ActionPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.CooperatePanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.NotifyPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.TweetPrvPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.GetPeriodPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.PreviewPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.BasedPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.FontPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ShortUrlPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ProxyPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ConnectionPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.FontPanel2)
        Me.SplitContainer1.Panel2.Controls.Add(Me.TweetActPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.StartupPanel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.GetCountPanel)
        Me.ToolTip1.SetToolTip(Me.SplitContainer1.Panel2, resources.GetString("SplitContainer1.Panel2.ToolTip"))
        Me.SplitContainer1.TabStop = False
        Me.ToolTip1.SetToolTip(Me.SplitContainer1, resources.GetString("SplitContainer1.ToolTip"))
        '
        'TreeViewSetting
        '
        resources.ApplyResources(Me.TreeViewSetting, "TreeViewSetting")
        Me.TreeViewSetting.Cursor = System.Windows.Forms.Cursors.Hand
        Me.TreeViewSetting.HideSelection = False
        Me.TreeViewSetting.Name = "TreeViewSetting"
        Me.TreeViewSetting.Nodes.AddRange(New System.Windows.Forms.TreeNode() {CType(resources.GetObject("TreeViewSetting.Nodes"), System.Windows.Forms.TreeNode), CType(resources.GetObject("TreeViewSetting.Nodes1"), System.Windows.Forms.TreeNode), CType(resources.GetObject("TreeViewSetting.Nodes2"), System.Windows.Forms.TreeNode), CType(resources.GetObject("TreeViewSetting.Nodes3"), System.Windows.Forms.TreeNode), CType(resources.GetObject("TreeViewSetting.Nodes4"), System.Windows.Forms.TreeNode)})
        Me.TreeViewSetting.ShowLines = False
        Me.ToolTip1.SetToolTip(Me.TreeViewSetting, resources.GetString("TreeViewSetting.ToolTip"))
        '
        'ActionPanel
        '
        resources.ApplyResources(Me.ActionPanel, "ActionPanel")
        Me.ActionPanel.Controls.Add(Me.Label38)
        Me.ActionPanel.Controls.Add(Me.ListDoubleClickActionComboBox)
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
        Me.ActionPanel.Name = "ActionPanel"
        Me.ToolTip1.SetToolTip(Me.ActionPanel, resources.GetString("ActionPanel.ToolTip"))
        '
        'Label38
        '
        resources.ApplyResources(Me.Label38, "Label38")
        Me.Label38.Name = "Label38"
        Me.ToolTip1.SetToolTip(Me.Label38, resources.GetString("Label38.ToolTip"))
        '
        'ListDoubleClickActionComboBox
        '
        resources.ApplyResources(Me.ListDoubleClickActionComboBox, "ListDoubleClickActionComboBox")
        Me.ListDoubleClickActionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ListDoubleClickActionComboBox.FormattingEnabled = True
        Me.ListDoubleClickActionComboBox.Items.AddRange(New Object() {resources.GetString("ListDoubleClickActionComboBox.Items"), resources.GetString("ListDoubleClickActionComboBox.Items1"), resources.GetString("ListDoubleClickActionComboBox.Items2"), resources.GetString("ListDoubleClickActionComboBox.Items3"), resources.GetString("ListDoubleClickActionComboBox.Items4"), resources.GetString("ListDoubleClickActionComboBox.Items5"), resources.GetString("ListDoubleClickActionComboBox.Items6"), resources.GetString("ListDoubleClickActionComboBox.Items7")})
        Me.ListDoubleClickActionComboBox.Name = "ListDoubleClickActionComboBox"
        Me.ToolTip1.SetToolTip(Me.ListDoubleClickActionComboBox, resources.GetString("ListDoubleClickActionComboBox.ToolTip"))
        '
        'CheckOpenUserTimeline
        '
        resources.ApplyResources(Me.CheckOpenUserTimeline, "CheckOpenUserTimeline")
        Me.CheckOpenUserTimeline.Name = "CheckOpenUserTimeline"
        Me.ToolTip1.SetToolTip(Me.CheckOpenUserTimeline, resources.GetString("CheckOpenUserTimeline.ToolTip"))
        Me.CheckOpenUserTimeline.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        resources.ApplyResources(Me.GroupBox3, "GroupBox3")
        Me.GroupBox3.Controls.Add(Me.HotkeyCheck)
        Me.GroupBox3.Controls.Add(Me.HotkeyCode)
        Me.GroupBox3.Controls.Add(Me.HotkeyText)
        Me.GroupBox3.Controls.Add(Me.HotkeyWin)
        Me.GroupBox3.Controls.Add(Me.HotkeyAlt)
        Me.GroupBox3.Controls.Add(Me.HotkeyShift)
        Me.GroupBox3.Controls.Add(Me.HotkeyCtrl)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.TabStop = False
        Me.ToolTip1.SetToolTip(Me.GroupBox3, resources.GetString("GroupBox3.ToolTip"))
        '
        'HotkeyCheck
        '
        resources.ApplyResources(Me.HotkeyCheck, "HotkeyCheck")
        Me.HotkeyCheck.Name = "HotkeyCheck"
        Me.ToolTip1.SetToolTip(Me.HotkeyCheck, resources.GetString("HotkeyCheck.ToolTip"))
        Me.HotkeyCheck.UseVisualStyleBackColor = True
        '
        'HotkeyCode
        '
        resources.ApplyResources(Me.HotkeyCode, "HotkeyCode")
        Me.HotkeyCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.HotkeyCode.Name = "HotkeyCode"
        Me.ToolTip1.SetToolTip(Me.HotkeyCode, resources.GetString("HotkeyCode.ToolTip"))
        '
        'HotkeyText
        '
        resources.ApplyResources(Me.HotkeyText, "HotkeyText")
        Me.HotkeyText.Name = "HotkeyText"
        Me.HotkeyText.ReadOnly = True
        Me.ToolTip1.SetToolTip(Me.HotkeyText, resources.GetString("HotkeyText.ToolTip"))
        '
        'HotkeyWin
        '
        resources.ApplyResources(Me.HotkeyWin, "HotkeyWin")
        Me.HotkeyWin.Name = "HotkeyWin"
        Me.ToolTip1.SetToolTip(Me.HotkeyWin, resources.GetString("HotkeyWin.ToolTip"))
        Me.HotkeyWin.UseVisualStyleBackColor = True
        '
        'HotkeyAlt
        '
        resources.ApplyResources(Me.HotkeyAlt, "HotkeyAlt")
        Me.HotkeyAlt.Name = "HotkeyAlt"
        Me.ToolTip1.SetToolTip(Me.HotkeyAlt, resources.GetString("HotkeyAlt.ToolTip"))
        Me.HotkeyAlt.UseVisualStyleBackColor = True
        '
        'HotkeyShift
        '
        resources.ApplyResources(Me.HotkeyShift, "HotkeyShift")
        Me.HotkeyShift.Name = "HotkeyShift"
        Me.ToolTip1.SetToolTip(Me.HotkeyShift, resources.GetString("HotkeyShift.ToolTip"))
        Me.HotkeyShift.UseVisualStyleBackColor = True
        '
        'HotkeyCtrl
        '
        resources.ApplyResources(Me.HotkeyCtrl, "HotkeyCtrl")
        Me.HotkeyCtrl.Name = "HotkeyCtrl"
        Me.ToolTip1.SetToolTip(Me.HotkeyCtrl, resources.GetString("HotkeyCtrl.ToolTip"))
        Me.HotkeyCtrl.UseVisualStyleBackColor = True
        '
        'Label57
        '
        resources.ApplyResources(Me.Label57, "Label57")
        Me.Label57.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label57.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label57.Name = "Label57"
        Me.ToolTip1.SetToolTip(Me.Label57, resources.GetString("Label57.ToolTip"))
        '
        'CheckFavRestrict
        '
        resources.ApplyResources(Me.CheckFavRestrict, "CheckFavRestrict")
        Me.CheckFavRestrict.Name = "CheckFavRestrict"
        Me.ToolTip1.SetToolTip(Me.CheckFavRestrict, resources.GetString("CheckFavRestrict.ToolTip"))
        Me.CheckFavRestrict.UseVisualStyleBackColor = True
        '
        'Button3
        '
        resources.ApplyResources(Me.Button3, "Button3")
        Me.Button3.Name = "Button3"
        Me.ToolTip1.SetToolTip(Me.Button3, resources.GetString("Button3.ToolTip"))
        Me.Button3.UseVisualStyleBackColor = True
        '
        'PlaySnd
        '
        resources.ApplyResources(Me.PlaySnd, "PlaySnd")
        Me.PlaySnd.Name = "PlaySnd"
        Me.ToolTip1.SetToolTip(Me.PlaySnd, resources.GetString("PlaySnd.ToolTip"))
        Me.PlaySnd.UseVisualStyleBackColor = True
        '
        'chkReadOwnPost
        '
        resources.ApplyResources(Me.chkReadOwnPost, "chkReadOwnPost")
        Me.chkReadOwnPost.Name = "chkReadOwnPost"
        Me.ToolTip1.SetToolTip(Me.chkReadOwnPost, resources.GetString("chkReadOwnPost.ToolTip"))
        Me.chkReadOwnPost.UseVisualStyleBackColor = True
        '
        'Label15
        '
        resources.ApplyResources(Me.Label15, "Label15")
        Me.Label15.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label15.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label15.Name = "Label15"
        Me.ToolTip1.SetToolTip(Me.Label15, resources.GetString("Label15.ToolTip"))
        '
        'BrowserPathText
        '
        resources.ApplyResources(Me.BrowserPathText, "BrowserPathText")
        Me.BrowserPathText.Name = "BrowserPathText"
        Me.ToolTip1.SetToolTip(Me.BrowserPathText, resources.GetString("BrowserPathText.ToolTip"))
        '
        'UReadMng
        '
        resources.ApplyResources(Me.UReadMng, "UReadMng")
        Me.UReadMng.Name = "UReadMng"
        Me.ToolTip1.SetToolTip(Me.UReadMng, resources.GetString("UReadMng.ToolTip"))
        Me.UReadMng.UseVisualStyleBackColor = True
        '
        'Label44
        '
        resources.ApplyResources(Me.Label44, "Label44")
        Me.Label44.Name = "Label44"
        Me.ToolTip1.SetToolTip(Me.Label44, resources.GetString("Label44.ToolTip"))
        '
        'CheckCloseToExit
        '
        resources.ApplyResources(Me.CheckCloseToExit, "CheckCloseToExit")
        Me.CheckCloseToExit.Name = "CheckCloseToExit"
        Me.ToolTip1.SetToolTip(Me.CheckCloseToExit, resources.GetString("CheckCloseToExit.ToolTip"))
        Me.CheckCloseToExit.UseVisualStyleBackColor = True
        '
        'CheckMinimizeToTray
        '
        resources.ApplyResources(Me.CheckMinimizeToTray, "CheckMinimizeToTray")
        Me.CheckMinimizeToTray.Name = "CheckMinimizeToTray"
        Me.ToolTip1.SetToolTip(Me.CheckMinimizeToTray, resources.GetString("CheckMinimizeToTray.ToolTip"))
        Me.CheckMinimizeToTray.UseVisualStyleBackColor = True
        '
        'CheckReadOldPosts
        '
        resources.ApplyResources(Me.CheckReadOldPosts, "CheckReadOldPosts")
        Me.CheckReadOldPosts.Name = "CheckReadOldPosts"
        Me.ToolTip1.SetToolTip(Me.CheckReadOldPosts, resources.GetString("CheckReadOldPosts.ToolTip"))
        Me.CheckReadOldPosts.UseVisualStyleBackColor = True
        '
        'CooperatePanel
        '
        resources.ApplyResources(Me.CooperatePanel, "CooperatePanel")
        Me.CooperatePanel.Controls.Add(Me.Label39)
        Me.CooperatePanel.Controls.Add(Me.UserAppointUrlText)
        Me.CooperatePanel.Controls.Add(Me.ComboBoxTranslateLanguage)
        Me.CooperatePanel.Controls.Add(Me.Label29)
        Me.CooperatePanel.Controls.Add(Me.CheckOutputz)
        Me.CooperatePanel.Controls.Add(Me.CheckNicoms)
        Me.CooperatePanel.Controls.Add(Me.TextBoxOutputzKey)
        Me.CooperatePanel.Controls.Add(Me.Label60)
        Me.CooperatePanel.Controls.Add(Me.Label59)
        Me.CooperatePanel.Controls.Add(Me.ComboBoxOutputzUrlmode)
        Me.CooperatePanel.Name = "CooperatePanel"
        Me.ToolTip1.SetToolTip(Me.CooperatePanel, resources.GetString("CooperatePanel.ToolTip"))
        '
        'Label39
        '
        resources.ApplyResources(Me.Label39, "Label39")
        Me.Label39.Name = "Label39"
        Me.ToolTip1.SetToolTip(Me.Label39, resources.GetString("Label39.ToolTip"))
        '
        'UserAppointUrlText
        '
        resources.ApplyResources(Me.UserAppointUrlText, "UserAppointUrlText")
        Me.UserAppointUrlText.Name = "UserAppointUrlText"
        Me.ToolTip1.SetToolTip(Me.UserAppointUrlText, resources.GetString("UserAppointUrlText.ToolTip"))
        '
        'ComboBoxTranslateLanguage
        '
        resources.ApplyResources(Me.ComboBoxTranslateLanguage, "ComboBoxTranslateLanguage")
        Me.ComboBoxTranslateLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxTranslateLanguage.FormattingEnabled = True
        Me.ComboBoxTranslateLanguage.Items.AddRange(New Object() {resources.GetString("ComboBoxTranslateLanguage.Items"), resources.GetString("ComboBoxTranslateLanguage.Items1"), resources.GetString("ComboBoxTranslateLanguage.Items2"), resources.GetString("ComboBoxTranslateLanguage.Items3"), resources.GetString("ComboBoxTranslateLanguage.Items4"), resources.GetString("ComboBoxTranslateLanguage.Items5"), resources.GetString("ComboBoxTranslateLanguage.Items6"), resources.GetString("ComboBoxTranslateLanguage.Items7"), resources.GetString("ComboBoxTranslateLanguage.Items8"), resources.GetString("ComboBoxTranslateLanguage.Items9"), resources.GetString("ComboBoxTranslateLanguage.Items10"), resources.GetString("ComboBoxTranslateLanguage.Items11"), resources.GetString("ComboBoxTranslateLanguage.Items12"), resources.GetString("ComboBoxTranslateLanguage.Items13"), resources.GetString("ComboBoxTranslateLanguage.Items14"), resources.GetString("ComboBoxTranslateLanguage.Items15"), resources.GetString("ComboBoxTranslateLanguage.Items16"), resources.GetString("ComboBoxTranslateLanguage.Items17"), resources.GetString("ComboBoxTranslateLanguage.Items18"), resources.GetString("ComboBoxTranslateLanguage.Items19"), resources.GetString("ComboBoxTranslateLanguage.Items20"), resources.GetString("ComboBoxTranslateLanguage.Items21"), resources.GetString("ComboBoxTranslateLanguage.Items22"), resources.GetString("ComboBoxTranslateLanguage.Items23"), resources.GetString("ComboBoxTranslateLanguage.Items24"), resources.GetString("ComboBoxTranslateLanguage.Items25"), resources.GetString("ComboBoxTranslateLanguage.Items26"), resources.GetString("ComboBoxTranslateLanguage.Items27"), resources.GetString("ComboBoxTranslateLanguage.Items28"), resources.GetString("ComboBoxTranslateLanguage.Items29"), resources.GetString("ComboBoxTranslateLanguage.Items30"), resources.GetString("ComboBoxTranslateLanguage.Items31"), resources.GetString("ComboBoxTranslateLanguage.Items32"), resources.GetString("ComboBoxTranslateLanguage.Items33"), resources.GetString("ComboBoxTranslateLanguage.Items34"), resources.GetString("ComboBoxTranslateLanguage.Items35"), resources.GetString("ComboBoxTranslateLanguage.Items36"), resources.GetString("ComboBoxTranslateLanguage.Items37"), resources.GetString("ComboBoxTranslateLanguage.Items38"), resources.GetString("ComboBoxTranslateLanguage.Items39"), resources.GetString("ComboBoxTranslateLanguage.Items40"), resources.GetString("ComboBoxTranslateLanguage.Items41"), resources.GetString("ComboBoxTranslateLanguage.Items42"), resources.GetString("ComboBoxTranslateLanguage.Items43"), resources.GetString("ComboBoxTranslateLanguage.Items44"), resources.GetString("ComboBoxTranslateLanguage.Items45"), resources.GetString("ComboBoxTranslateLanguage.Items46"), resources.GetString("ComboBoxTranslateLanguage.Items47"), resources.GetString("ComboBoxTranslateLanguage.Items48"), resources.GetString("ComboBoxTranslateLanguage.Items49"), resources.GetString("ComboBoxTranslateLanguage.Items50"), resources.GetString("ComboBoxTranslateLanguage.Items51"), resources.GetString("ComboBoxTranslateLanguage.Items52"), resources.GetString("ComboBoxTranslateLanguage.Items53"), resources.GetString("ComboBoxTranslateLanguage.Items54"), resources.GetString("ComboBoxTranslateLanguage.Items55"), resources.GetString("ComboBoxTranslateLanguage.Items56"), resources.GetString("ComboBoxTranslateLanguage.Items57"), resources.GetString("ComboBoxTranslateLanguage.Items58"), resources.GetString("ComboBoxTranslateLanguage.Items59"), resources.GetString("ComboBoxTranslateLanguage.Items60"), resources.GetString("ComboBoxTranslateLanguage.Items61"), resources.GetString("ComboBoxTranslateLanguage.Items62"), resources.GetString("ComboBoxTranslateLanguage.Items63"), resources.GetString("ComboBoxTranslateLanguage.Items64"), resources.GetString("ComboBoxTranslateLanguage.Items65"), resources.GetString("ComboBoxTranslateLanguage.Items66"), resources.GetString("ComboBoxTranslateLanguage.Items67"), resources.GetString("ComboBoxTranslateLanguage.Items68"), resources.GetString("ComboBoxTranslateLanguage.Items69"), resources.GetString("ComboBoxTranslateLanguage.Items70"), resources.GetString("ComboBoxTranslateLanguage.Items71"), resources.GetString("ComboBoxTranslateLanguage.Items72"), resources.GetString("ComboBoxTranslateLanguage.Items73"), resources.GetString("ComboBoxTranslateLanguage.Items74"), resources.GetString("ComboBoxTranslateLanguage.Items75"), resources.GetString("ComboBoxTranslateLanguage.Items76"), resources.GetString("ComboBoxTranslateLanguage.Items77"), resources.GetString("ComboBoxTranslateLanguage.Items78"), resources.GetString("ComboBoxTranslateLanguage.Items79"), resources.GetString("ComboBoxTranslateLanguage.Items80"), resources.GetString("ComboBoxTranslateLanguage.Items81"), resources.GetString("ComboBoxTranslateLanguage.Items82"), resources.GetString("ComboBoxTranslateLanguage.Items83"), resources.GetString("ComboBoxTranslateLanguage.Items84"), resources.GetString("ComboBoxTranslateLanguage.Items85"), resources.GetString("ComboBoxTranslateLanguage.Items86"), resources.GetString("ComboBoxTranslateLanguage.Items87"), resources.GetString("ComboBoxTranslateLanguage.Items88"), resources.GetString("ComboBoxTranslateLanguage.Items89"), resources.GetString("ComboBoxTranslateLanguage.Items90"), resources.GetString("ComboBoxTranslateLanguage.Items91"), resources.GetString("ComboBoxTranslateLanguage.Items92"), resources.GetString("ComboBoxTranslateLanguage.Items93"), resources.GetString("ComboBoxTranslateLanguage.Items94"), resources.GetString("ComboBoxTranslateLanguage.Items95"), resources.GetString("ComboBoxTranslateLanguage.Items96"), resources.GetString("ComboBoxTranslateLanguage.Items97"), resources.GetString("ComboBoxTranslateLanguage.Items98"), resources.GetString("ComboBoxTranslateLanguage.Items99"), resources.GetString("ComboBoxTranslateLanguage.Items100"), resources.GetString("ComboBoxTranslateLanguage.Items101"), resources.GetString("ComboBoxTranslateLanguage.Items102"), resources.GetString("ComboBoxTranslateLanguage.Items103"), resources.GetString("ComboBoxTranslateLanguage.Items104"), resources.GetString("ComboBoxTranslateLanguage.Items105"), resources.GetString("ComboBoxTranslateLanguage.Items106")})
        Me.ComboBoxTranslateLanguage.Name = "ComboBoxTranslateLanguage"
        Me.ToolTip1.SetToolTip(Me.ComboBoxTranslateLanguage, resources.GetString("ComboBoxTranslateLanguage.ToolTip"))
        '
        'Label29
        '
        resources.ApplyResources(Me.Label29, "Label29")
        Me.Label29.Name = "Label29"
        Me.ToolTip1.SetToolTip(Me.Label29, resources.GetString("Label29.ToolTip"))
        '
        'CheckOutputz
        '
        resources.ApplyResources(Me.CheckOutputz, "CheckOutputz")
        Me.CheckOutputz.Name = "CheckOutputz"
        Me.ToolTip1.SetToolTip(Me.CheckOutputz, resources.GetString("CheckOutputz.ToolTip"))
        Me.CheckOutputz.UseVisualStyleBackColor = True
        '
        'CheckNicoms
        '
        resources.ApplyResources(Me.CheckNicoms, "CheckNicoms")
        Me.CheckNicoms.Name = "CheckNicoms"
        Me.ToolTip1.SetToolTip(Me.CheckNicoms, resources.GetString("CheckNicoms.ToolTip"))
        Me.CheckNicoms.UseVisualStyleBackColor = True
        '
        'TextBoxOutputzKey
        '
        resources.ApplyResources(Me.TextBoxOutputzKey, "TextBoxOutputzKey")
        Me.TextBoxOutputzKey.Name = "TextBoxOutputzKey"
        Me.ToolTip1.SetToolTip(Me.TextBoxOutputzKey, resources.GetString("TextBoxOutputzKey.ToolTip"))
        '
        'Label60
        '
        resources.ApplyResources(Me.Label60, "Label60")
        Me.Label60.Name = "Label60"
        Me.ToolTip1.SetToolTip(Me.Label60, resources.GetString("Label60.ToolTip"))
        '
        'Label59
        '
        resources.ApplyResources(Me.Label59, "Label59")
        Me.Label59.Name = "Label59"
        Me.ToolTip1.SetToolTip(Me.Label59, resources.GetString("Label59.ToolTip"))
        '
        'ComboBoxOutputzUrlmode
        '
        resources.ApplyResources(Me.ComboBoxOutputzUrlmode, "ComboBoxOutputzUrlmode")
        Me.ComboBoxOutputzUrlmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxOutputzUrlmode.FormattingEnabled = True
        Me.ComboBoxOutputzUrlmode.Items.AddRange(New Object() {resources.GetString("ComboBoxOutputzUrlmode.Items"), resources.GetString("ComboBoxOutputzUrlmode.Items1")})
        Me.ComboBoxOutputzUrlmode.Name = "ComboBoxOutputzUrlmode"
        Me.ToolTip1.SetToolTip(Me.ComboBoxOutputzUrlmode, resources.GetString("ComboBoxOutputzUrlmode.ToolTip"))
        '
        'NotifyPanel
        '
        resources.ApplyResources(Me.NotifyPanel, "NotifyPanel")
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
        Me.NotifyPanel.Name = "NotifyPanel"
        Me.ToolTip1.SetToolTip(Me.NotifyPanel, resources.GetString("NotifyPanel.ToolTip"))
        '
        'CheckUserUpdateEvent
        '
        resources.ApplyResources(Me.CheckUserUpdateEvent, "CheckUserUpdateEvent")
        Me.CheckUserUpdateEvent.Checked = True
        Me.CheckUserUpdateEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckUserUpdateEvent.Name = "CheckUserUpdateEvent"
        Me.CheckUserUpdateEvent.ThreeState = True
        Me.ToolTip1.SetToolTip(Me.CheckUserUpdateEvent, resources.GetString("CheckUserUpdateEvent.ToolTip"))
        Me.CheckUserUpdateEvent.UseVisualStyleBackColor = True
        '
        'Label35
        '
        resources.ApplyResources(Me.Label35, "Label35")
        Me.Label35.Name = "Label35"
        Me.ToolTip1.SetToolTip(Me.Label35, resources.GetString("Label35.ToolTip"))
        '
        'ComboBoxEventNotifySound
        '
        resources.ApplyResources(Me.ComboBoxEventNotifySound, "ComboBoxEventNotifySound")
        Me.ComboBoxEventNotifySound.FormattingEnabled = True
        Me.ComboBoxEventNotifySound.Name = "ComboBoxEventNotifySound"
        Me.ToolTip1.SetToolTip(Me.ComboBoxEventNotifySound, resources.GetString("ComboBoxEventNotifySound.ToolTip"))
        '
        'CheckFavEventUnread
        '
        resources.ApplyResources(Me.CheckFavEventUnread, "CheckFavEventUnread")
        Me.CheckFavEventUnread.Checked = True
        Me.CheckFavEventUnread.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckFavEventUnread.Name = "CheckFavEventUnread"
        Me.ToolTip1.SetToolTip(Me.CheckFavEventUnread, resources.GetString("CheckFavEventUnread.ToolTip"))
        Me.CheckFavEventUnread.UseVisualStyleBackColor = True
        '
        'CheckListCreatedEvent
        '
        resources.ApplyResources(Me.CheckListCreatedEvent, "CheckListCreatedEvent")
        Me.CheckListCreatedEvent.Checked = True
        Me.CheckListCreatedEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckListCreatedEvent.Name = "CheckListCreatedEvent"
        Me.CheckListCreatedEvent.ThreeState = True
        Me.ToolTip1.SetToolTip(Me.CheckListCreatedEvent, resources.GetString("CheckListCreatedEvent.ToolTip"))
        Me.CheckListCreatedEvent.UseVisualStyleBackColor = True
        '
        'CheckBlockEvent
        '
        resources.ApplyResources(Me.CheckBlockEvent, "CheckBlockEvent")
        Me.CheckBlockEvent.Checked = True
        Me.CheckBlockEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBlockEvent.Name = "CheckBlockEvent"
        Me.CheckBlockEvent.ThreeState = True
        Me.ToolTip1.SetToolTip(Me.CheckBlockEvent, resources.GetString("CheckBlockEvent.ToolTip"))
        Me.CheckBlockEvent.UseVisualStyleBackColor = True
        '
        'CheckForceEventNotify
        '
        resources.ApplyResources(Me.CheckForceEventNotify, "CheckForceEventNotify")
        Me.CheckForceEventNotify.Checked = True
        Me.CheckForceEventNotify.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckForceEventNotify.Name = "CheckForceEventNotify"
        Me.ToolTip1.SetToolTip(Me.CheckForceEventNotify, resources.GetString("CheckForceEventNotify.ToolTip"))
        Me.CheckForceEventNotify.UseVisualStyleBackColor = True
        '
        'CheckListMemberRemovedEvent
        '
        resources.ApplyResources(Me.CheckListMemberRemovedEvent, "CheckListMemberRemovedEvent")
        Me.CheckListMemberRemovedEvent.Checked = True
        Me.CheckListMemberRemovedEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckListMemberRemovedEvent.Name = "CheckListMemberRemovedEvent"
        Me.CheckListMemberRemovedEvent.ThreeState = True
        Me.ToolTip1.SetToolTip(Me.CheckListMemberRemovedEvent, resources.GetString("CheckListMemberRemovedEvent.ToolTip"))
        Me.CheckListMemberRemovedEvent.UseVisualStyleBackColor = True
        '
        'CheckListMemberAddedEvent
        '
        resources.ApplyResources(Me.CheckListMemberAddedEvent, "CheckListMemberAddedEvent")
        Me.CheckListMemberAddedEvent.Checked = True
        Me.CheckListMemberAddedEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckListMemberAddedEvent.Name = "CheckListMemberAddedEvent"
        Me.CheckListMemberAddedEvent.ThreeState = True
        Me.ToolTip1.SetToolTip(Me.CheckListMemberAddedEvent, resources.GetString("CheckListMemberAddedEvent.ToolTip"))
        Me.CheckListMemberAddedEvent.UseVisualStyleBackColor = True
        '
        'CheckFollowEvent
        '
        resources.ApplyResources(Me.CheckFollowEvent, "CheckFollowEvent")
        Me.CheckFollowEvent.Checked = True
        Me.CheckFollowEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckFollowEvent.Name = "CheckFollowEvent"
        Me.CheckFollowEvent.ThreeState = True
        Me.ToolTip1.SetToolTip(Me.CheckFollowEvent, resources.GetString("CheckFollowEvent.ToolTip"))
        Me.CheckFollowEvent.UseVisualStyleBackColor = True
        '
        'CheckUnfavoritesEvent
        '
        resources.ApplyResources(Me.CheckUnfavoritesEvent, "CheckUnfavoritesEvent")
        Me.CheckUnfavoritesEvent.Checked = True
        Me.CheckUnfavoritesEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckUnfavoritesEvent.Name = "CheckUnfavoritesEvent"
        Me.CheckUnfavoritesEvent.ThreeState = True
        Me.ToolTip1.SetToolTip(Me.CheckUnfavoritesEvent, resources.GetString("CheckUnfavoritesEvent.ToolTip"))
        Me.CheckUnfavoritesEvent.UseVisualStyleBackColor = True
        '
        'CheckFavoritesEvent
        '
        resources.ApplyResources(Me.CheckFavoritesEvent, "CheckFavoritesEvent")
        Me.CheckFavoritesEvent.Checked = True
        Me.CheckFavoritesEvent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckFavoritesEvent.Name = "CheckFavoritesEvent"
        Me.CheckFavoritesEvent.ThreeState = True
        Me.ToolTip1.SetToolTip(Me.CheckFavoritesEvent, resources.GetString("CheckFavoritesEvent.ToolTip"))
        Me.CheckFavoritesEvent.UseVisualStyleBackColor = True
        '
        'CheckEventNotify
        '
        resources.ApplyResources(Me.CheckEventNotify, "CheckEventNotify")
        Me.CheckEventNotify.Checked = True
        Me.CheckEventNotify.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckEventNotify.Name = "CheckEventNotify"
        Me.ToolTip1.SetToolTip(Me.CheckEventNotify, resources.GetString("CheckEventNotify.ToolTip"))
        Me.CheckEventNotify.UseVisualStyleBackColor = True
        '
        'TweetPrvPanel
        '
        resources.ApplyResources(Me.TweetPrvPanel, "TweetPrvPanel")
        Me.TweetPrvPanel.Controls.Add(Me.HideDuplicatedRetweetsCheck)
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
        Me.TweetPrvPanel.Name = "TweetPrvPanel"
        Me.ToolTip1.SetToolTip(Me.TweetPrvPanel, resources.GetString("TweetPrvPanel.ToolTip"))
        '
        'HideDuplicatedRetweetsCheck
        '
        resources.ApplyResources(Me.HideDuplicatedRetweetsCheck, "HideDuplicatedRetweetsCheck")
        Me.HideDuplicatedRetweetsCheck.Name = "HideDuplicatedRetweetsCheck"
        Me.ToolTip1.SetToolTip(Me.HideDuplicatedRetweetsCheck, resources.GetString("HideDuplicatedRetweetsCheck.ToolTip"))
        Me.HideDuplicatedRetweetsCheck.UseVisualStyleBackColor = True
        '
        'Label47
        '
        resources.ApplyResources(Me.Label47, "Label47")
        Me.Label47.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label47.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label47.Name = "Label47"
        Me.ToolTip1.SetToolTip(Me.Label47, resources.GetString("Label47.ToolTip"))
        '
        'LabelDateTimeFormatApplied
        '
        resources.ApplyResources(Me.LabelDateTimeFormatApplied, "LabelDateTimeFormatApplied")
        Me.LabelDateTimeFormatApplied.Name = "LabelDateTimeFormatApplied"
        Me.ToolTip1.SetToolTip(Me.LabelDateTimeFormatApplied, resources.GetString("LabelDateTimeFormatApplied.ToolTip"))
        '
        'Label62
        '
        resources.ApplyResources(Me.Label62, "Label62")
        Me.Label62.Name = "Label62"
        Me.ToolTip1.SetToolTip(Me.Label62, resources.GetString("Label62.ToolTip"))
        '
        'CmbDateTimeFormat
        '
        resources.ApplyResources(Me.CmbDateTimeFormat, "CmbDateTimeFormat")
        Me.CmbDateTimeFormat.Items.AddRange(New Object() {resources.GetString("CmbDateTimeFormat.Items"), resources.GetString("CmbDateTimeFormat.Items1"), resources.GetString("CmbDateTimeFormat.Items2"), resources.GetString("CmbDateTimeFormat.Items3"), resources.GetString("CmbDateTimeFormat.Items4"), resources.GetString("CmbDateTimeFormat.Items5"), resources.GetString("CmbDateTimeFormat.Items6"), resources.GetString("CmbDateTimeFormat.Items7"), resources.GetString("CmbDateTimeFormat.Items8"), resources.GetString("CmbDateTimeFormat.Items9"), resources.GetString("CmbDateTimeFormat.Items10")})
        Me.CmbDateTimeFormat.Name = "CmbDateTimeFormat"
        Me.ToolTip1.SetToolTip(Me.CmbDateTimeFormat, resources.GetString("CmbDateTimeFormat.ToolTip"))
        '
        'Label23
        '
        resources.ApplyResources(Me.Label23, "Label23")
        Me.Label23.Name = "Label23"
        Me.ToolTip1.SetToolTip(Me.Label23, resources.GetString("Label23.ToolTip"))
        '
        'Label11
        '
        resources.ApplyResources(Me.Label11, "Label11")
        Me.Label11.Name = "Label11"
        Me.ToolTip1.SetToolTip(Me.Label11, resources.GetString("Label11.ToolTip"))
        '
        'IconSize
        '
        resources.ApplyResources(Me.IconSize, "IconSize")
        Me.IconSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.IconSize.FormattingEnabled = True
        Me.IconSize.Items.AddRange(New Object() {resources.GetString("IconSize.Items"), resources.GetString("IconSize.Items1"), resources.GetString("IconSize.Items2"), resources.GetString("IconSize.Items3"), resources.GetString("IconSize.Items4")})
        Me.IconSize.Name = "IconSize"
        Me.ToolTip1.SetToolTip(Me.IconSize, resources.GetString("IconSize.ToolTip"))
        '
        'TextBox3
        '
        resources.ApplyResources(Me.TextBox3, "TextBox3")
        Me.TextBox3.Name = "TextBox3"
        Me.ToolTip1.SetToolTip(Me.TextBox3, resources.GetString("TextBox3.ToolTip"))
        '
        'CheckSortOrderLock
        '
        resources.ApplyResources(Me.CheckSortOrderLock, "CheckSortOrderLock")
        Me.CheckSortOrderLock.Name = "CheckSortOrderLock"
        Me.ToolTip1.SetToolTip(Me.CheckSortOrderLock, resources.GetString("CheckSortOrderLock.ToolTip"))
        Me.CheckSortOrderLock.UseVisualStyleBackColor = True
        '
        'CheckShowGrid
        '
        resources.ApplyResources(Me.CheckShowGrid, "CheckShowGrid")
        Me.CheckShowGrid.Name = "CheckShowGrid"
        Me.ToolTip1.SetToolTip(Me.CheckShowGrid, resources.GetString("CheckShowGrid.ToolTip"))
        Me.CheckShowGrid.UseVisualStyleBackColor = True
        '
        'chkUnreadStyle
        '
        resources.ApplyResources(Me.chkUnreadStyle, "chkUnreadStyle")
        Me.chkUnreadStyle.Name = "chkUnreadStyle"
        Me.ToolTip1.SetToolTip(Me.chkUnreadStyle, resources.GetString("chkUnreadStyle.ToolTip"))
        Me.chkUnreadStyle.UseVisualStyleBackColor = True
        '
        'OneWayLv
        '
        resources.ApplyResources(Me.OneWayLv, "OneWayLv")
        Me.OneWayLv.Name = "OneWayLv"
        Me.ToolTip1.SetToolTip(Me.OneWayLv, resources.GetString("OneWayLv.ToolTip"))
        Me.OneWayLv.UseVisualStyleBackColor = True
        '
        'GetPeriodPanel
        '
        resources.ApplyResources(Me.GetPeriodPanel, "GetPeriodPanel")
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
        Me.GetPeriodPanel.Controls.Add(Me.UserstreamPeriod)
        Me.GetPeriodPanel.Controls.Add(Me.StartupUserstreamCheck)
        Me.GetPeriodPanel.Controls.Add(Me.Label83)
        Me.GetPeriodPanel.Name = "GetPeriodPanel"
        Me.ToolTip1.SetToolTip(Me.GetPeriodPanel, resources.GetString("GetPeriodPanel.ToolTip"))
        '
        'LabelApiUsingUserStreamEnabled
        '
        resources.ApplyResources(Me.LabelApiUsingUserStreamEnabled, "LabelApiUsingUserStreamEnabled")
        Me.LabelApiUsingUserStreamEnabled.Name = "LabelApiUsingUserStreamEnabled"
        Me.ToolTip1.SetToolTip(Me.LabelApiUsingUserStreamEnabled, resources.GetString("LabelApiUsingUserStreamEnabled.ToolTip"))
        '
        'LabelUserStreamActive
        '
        resources.ApplyResources(Me.LabelUserStreamActive, "LabelUserStreamActive")
        Me.LabelUserStreamActive.Name = "LabelUserStreamActive"
        Me.ToolTip1.SetToolTip(Me.LabelUserStreamActive, resources.GetString("LabelUserStreamActive.ToolTip"))
        '
        'Label21
        '
        resources.ApplyResources(Me.Label21, "Label21")
        Me.Label21.Name = "Label21"
        Me.ToolTip1.SetToolTip(Me.Label21, resources.GetString("Label21.ToolTip"))
        '
        'UserTimelinePeriod
        '
        resources.ApplyResources(Me.UserTimelinePeriod, "UserTimelinePeriod")
        Me.UserTimelinePeriod.Name = "UserTimelinePeriod"
        Me.ToolTip1.SetToolTip(Me.UserTimelinePeriod, resources.GetString("UserTimelinePeriod.ToolTip"))
        '
        'TimelinePeriod
        '
        resources.ApplyResources(Me.TimelinePeriod, "TimelinePeriod")
        Me.TimelinePeriod.Name = "TimelinePeriod"
        Me.ToolTip1.SetToolTip(Me.TimelinePeriod, resources.GetString("TimelinePeriod.ToolTip"))
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        Me.ToolTip1.SetToolTip(Me.Label3, resources.GetString("Label3.ToolTip"))
        '
        'ButtonApiCalc
        '
        resources.ApplyResources(Me.ButtonApiCalc, "ButtonApiCalc")
        Me.ButtonApiCalc.Name = "ButtonApiCalc"
        Me.ToolTip1.SetToolTip(Me.ButtonApiCalc, resources.GetString("ButtonApiCalc.ToolTip"))
        Me.ButtonApiCalc.UseVisualStyleBackColor = True
        '
        'LabelPostAndGet
        '
        resources.ApplyResources(Me.LabelPostAndGet, "LabelPostAndGet")
        Me.LabelPostAndGet.Name = "LabelPostAndGet"
        Me.ToolTip1.SetToolTip(Me.LabelPostAndGet, resources.GetString("LabelPostAndGet.ToolTip"))
        '
        'LabelApiUsing
        '
        resources.ApplyResources(Me.LabelApiUsing, "LabelApiUsing")
        Me.LabelApiUsing.Name = "LabelApiUsing"
        Me.ToolTip1.SetToolTip(Me.LabelApiUsing, resources.GetString("LabelApiUsing.ToolTip"))
        '
        'Label33
        '
        resources.ApplyResources(Me.Label33, "Label33")
        Me.Label33.Name = "Label33"
        Me.ToolTip1.SetToolTip(Me.Label33, resources.GetString("Label33.ToolTip"))
        '
        'ListsPeriod
        '
        resources.ApplyResources(Me.ListsPeriod, "ListsPeriod")
        Me.ListsPeriod.Name = "ListsPeriod"
        Me.ToolTip1.SetToolTip(Me.ListsPeriod, resources.GetString("ListsPeriod.ToolTip"))
        '
        'Label7
        '
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Name = "Label7"
        Me.ToolTip1.SetToolTip(Me.Label7, resources.GetString("Label7.ToolTip"))
        '
        'PubSearchPeriod
        '
        resources.ApplyResources(Me.PubSearchPeriod, "PubSearchPeriod")
        Me.PubSearchPeriod.Name = "PubSearchPeriod"
        Me.ToolTip1.SetToolTip(Me.PubSearchPeriod, resources.GetString("PubSearchPeriod.ToolTip"))
        '
        'Label69
        '
        resources.ApplyResources(Me.Label69, "Label69")
        Me.Label69.Name = "Label69"
        Me.ToolTip1.SetToolTip(Me.Label69, resources.GetString("Label69.ToolTip"))
        '
        'ReplyPeriod
        '
        resources.ApplyResources(Me.ReplyPeriod, "ReplyPeriod")
        Me.ReplyPeriod.Name = "ReplyPeriod"
        Me.ToolTip1.SetToolTip(Me.ReplyPeriod, resources.GetString("ReplyPeriod.ToolTip"))
        '
        'CheckPostAndGet
        '
        resources.ApplyResources(Me.CheckPostAndGet, "CheckPostAndGet")
        Me.CheckPostAndGet.Name = "CheckPostAndGet"
        Me.ToolTip1.SetToolTip(Me.CheckPostAndGet, resources.GetString("CheckPostAndGet.ToolTip"))
        Me.CheckPostAndGet.UseVisualStyleBackColor = True
        '
        'CheckPeriodAdjust
        '
        resources.ApplyResources(Me.CheckPeriodAdjust, "CheckPeriodAdjust")
        Me.CheckPeriodAdjust.Name = "CheckPeriodAdjust"
        Me.ToolTip1.SetToolTip(Me.CheckPeriodAdjust, resources.GetString("CheckPeriodAdjust.ToolTip"))
        Me.CheckPeriodAdjust.UseVisualStyleBackColor = True
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        Me.ToolTip1.SetToolTip(Me.Label5, resources.GetString("Label5.ToolTip"))
        '
        'DMPeriod
        '
        resources.ApplyResources(Me.DMPeriod, "DMPeriod")
        Me.DMPeriod.Name = "DMPeriod"
        Me.ToolTip1.SetToolTip(Me.DMPeriod, resources.GetString("DMPeriod.ToolTip"))
        '
        'UserstreamPeriod
        '
        resources.ApplyResources(Me.UserstreamPeriod, "UserstreamPeriod")
        Me.UserstreamPeriod.Name = "UserstreamPeriod"
        Me.ToolTip1.SetToolTip(Me.UserstreamPeriod, resources.GetString("UserstreamPeriod.ToolTip"))
        '
        'StartupUserstreamCheck
        '
        resources.ApplyResources(Me.StartupUserstreamCheck, "StartupUserstreamCheck")
        Me.StartupUserstreamCheck.Name = "StartupUserstreamCheck"
        Me.ToolTip1.SetToolTip(Me.StartupUserstreamCheck, resources.GetString("StartupUserstreamCheck.ToolTip"))
        Me.StartupUserstreamCheck.UseVisualStyleBackColor = True
        '
        'Label83
        '
        resources.ApplyResources(Me.Label83, "Label83")
        Me.Label83.Name = "Label83"
        Me.ToolTip1.SetToolTip(Me.Label83, resources.GetString("Label83.ToolTip"))
        '
        'PreviewPanel
        '
        resources.ApplyResources(Me.PreviewPanel, "PreviewPanel")
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
        Me.PreviewPanel.Name = "PreviewPanel"
        Me.ToolTip1.SetToolTip(Me.PreviewPanel, resources.GetString("PreviewPanel.ToolTip"))
        '
        'ReplyIconStateCombo
        '
        resources.ApplyResources(Me.ReplyIconStateCombo, "ReplyIconStateCombo")
        Me.ReplyIconStateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ReplyIconStateCombo.FormattingEnabled = True
        Me.ReplyIconStateCombo.Items.AddRange(New Object() {resources.GetString("ReplyIconStateCombo.Items"), resources.GetString("ReplyIconStateCombo.Items1"), resources.GetString("ReplyIconStateCombo.Items2")})
        Me.ReplyIconStateCombo.Name = "ReplyIconStateCombo"
        Me.ToolTip1.SetToolTip(Me.ReplyIconStateCombo, resources.GetString("ReplyIconStateCombo.ToolTip"))
        '
        'Label72
        '
        resources.ApplyResources(Me.Label72, "Label72")
        Me.Label72.Name = "Label72"
        Me.ToolTip1.SetToolTip(Me.Label72, resources.GetString("Label72.ToolTip"))
        '
        'ChkNewMentionsBlink
        '
        resources.ApplyResources(Me.ChkNewMentionsBlink, "ChkNewMentionsBlink")
        Me.ChkNewMentionsBlink.Name = "ChkNewMentionsBlink"
        Me.ToolTip1.SetToolTip(Me.ChkNewMentionsBlink, resources.GetString("ChkNewMentionsBlink.ToolTip"))
        Me.ChkNewMentionsBlink.UseVisualStyleBackColor = True
        '
        'chkTabIconDisp
        '
        resources.ApplyResources(Me.chkTabIconDisp, "chkTabIconDisp")
        Me.chkTabIconDisp.Name = "chkTabIconDisp"
        Me.ToolTip1.SetToolTip(Me.chkTabIconDisp, resources.GetString("chkTabIconDisp.ToolTip"))
        Me.chkTabIconDisp.UseVisualStyleBackColor = True
        '
        'CheckPreviewEnable
        '
        resources.ApplyResources(Me.CheckPreviewEnable, "CheckPreviewEnable")
        Me.CheckPreviewEnable.Name = "CheckPreviewEnable"
        Me.ToolTip1.SetToolTip(Me.CheckPreviewEnable, resources.GetString("CheckPreviewEnable.ToolTip"))
        Me.CheckPreviewEnable.UseVisualStyleBackColor = True
        '
        'Label81
        '
        resources.ApplyResources(Me.Label81, "Label81")
        Me.Label81.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label81.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label81.Name = "Label81"
        Me.ToolTip1.SetToolTip(Me.Label81, resources.GetString("Label81.ToolTip"))
        '
        'LanguageCombo
        '
        resources.ApplyResources(Me.LanguageCombo, "LanguageCombo")
        Me.LanguageCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.LanguageCombo.FormattingEnabled = True
        Me.LanguageCombo.Items.AddRange(New Object() {resources.GetString("LanguageCombo.Items"), resources.GetString("LanguageCombo.Items1"), resources.GetString("LanguageCombo.Items2"), resources.GetString("LanguageCombo.Items3")})
        Me.LanguageCombo.Name = "LanguageCombo"
        Me.ToolTip1.SetToolTip(Me.LanguageCombo, resources.GetString("LanguageCombo.ToolTip"))
        '
        'Label13
        '
        resources.ApplyResources(Me.Label13, "Label13")
        Me.Label13.Name = "Label13"
        Me.ToolTip1.SetToolTip(Me.Label13, resources.GetString("Label13.ToolTip"))
        '
        'CheckAlwaysTop
        '
        resources.ApplyResources(Me.CheckAlwaysTop, "CheckAlwaysTop")
        Me.CheckAlwaysTop.Name = "CheckAlwaysTop"
        Me.ToolTip1.SetToolTip(Me.CheckAlwaysTop, resources.GetString("CheckAlwaysTop.ToolTip"))
        Me.CheckAlwaysTop.UseVisualStyleBackColor = True
        '
        'CheckMonospace
        '
        resources.ApplyResources(Me.CheckMonospace, "CheckMonospace")
        Me.CheckMonospace.Name = "CheckMonospace"
        Me.ToolTip1.SetToolTip(Me.CheckMonospace, resources.GetString("CheckMonospace.ToolTip"))
        Me.CheckMonospace.UseVisualStyleBackColor = True
        '
        'CheckBalloonLimit
        '
        resources.ApplyResources(Me.CheckBalloonLimit, "CheckBalloonLimit")
        Me.CheckBalloonLimit.Name = "CheckBalloonLimit"
        Me.ToolTip1.SetToolTip(Me.CheckBalloonLimit, resources.GetString("CheckBalloonLimit.ToolTip"))
        Me.CheckBalloonLimit.UseVisualStyleBackColor = True
        '
        'Label10
        '
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Name = "Label10"
        Me.ToolTip1.SetToolTip(Me.Label10, resources.GetString("Label10.ToolTip"))
        '
        'ComboDispTitle
        '
        resources.ApplyResources(Me.ComboDispTitle, "ComboDispTitle")
        Me.ComboDispTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboDispTitle.FormattingEnabled = True
        Me.ComboDispTitle.Items.AddRange(New Object() {resources.GetString("ComboDispTitle.Items"), resources.GetString("ComboDispTitle.Items1"), resources.GetString("ComboDispTitle.Items2"), resources.GetString("ComboDispTitle.Items3"), resources.GetString("ComboDispTitle.Items4"), resources.GetString("ComboDispTitle.Items5"), resources.GetString("ComboDispTitle.Items6"), resources.GetString("ComboDispTitle.Items7")})
        Me.ComboDispTitle.Name = "ComboDispTitle"
        Me.ToolTip1.SetToolTip(Me.ComboDispTitle, resources.GetString("ComboDispTitle.ToolTip"))
        '
        'Label45
        '
        resources.ApplyResources(Me.Label45, "Label45")
        Me.Label45.Name = "Label45"
        Me.ToolTip1.SetToolTip(Me.Label45, resources.GetString("Label45.ToolTip"))
        '
        'cmbNameBalloon
        '
        resources.ApplyResources(Me.cmbNameBalloon, "cmbNameBalloon")
        Me.cmbNameBalloon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNameBalloon.FormattingEnabled = True
        Me.cmbNameBalloon.Items.AddRange(New Object() {resources.GetString("cmbNameBalloon.Items"), resources.GetString("cmbNameBalloon.Items1"), resources.GetString("cmbNameBalloon.Items2")})
        Me.cmbNameBalloon.Name = "cmbNameBalloon"
        Me.ToolTip1.SetToolTip(Me.cmbNameBalloon, resources.GetString("cmbNameBalloon.ToolTip"))
        '
        'CheckDispUsername
        '
        resources.ApplyResources(Me.CheckDispUsername, "CheckDispUsername")
        Me.CheckDispUsername.Name = "CheckDispUsername"
        Me.ToolTip1.SetToolTip(Me.CheckDispUsername, resources.GetString("CheckDispUsername.ToolTip"))
        Me.CheckDispUsername.UseVisualStyleBackColor = True
        '
        'CheckBox3
        '
        resources.ApplyResources(Me.CheckBox3, "CheckBox3")
        Me.CheckBox3.Name = "CheckBox3"
        Me.ToolTip1.SetToolTip(Me.CheckBox3, resources.GetString("CheckBox3.ToolTip"))
        Me.CheckBox3.UseVisualStyleBackColor = True
        '
        'BasedPanel
        '
        resources.ApplyResources(Me.BasedPanel, "BasedPanel")
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
        Me.BasedPanel.Name = "BasedPanel"
        Me.ToolTip1.SetToolTip(Me.BasedPanel, resources.GetString("BasedPanel.ToolTip"))
        '
        'AuthBasicRadio
        '
        resources.ApplyResources(Me.AuthBasicRadio, "AuthBasicRadio")
        Me.AuthBasicRadio.Name = "AuthBasicRadio"
        Me.ToolTip1.SetToolTip(Me.AuthBasicRadio, resources.GetString("AuthBasicRadio.ToolTip"))
        Me.AuthBasicRadio.UseVisualStyleBackColor = True
        '
        'AuthOAuthRadio
        '
        resources.ApplyResources(Me.AuthOAuthRadio, "AuthOAuthRadio")
        Me.AuthOAuthRadio.Checked = True
        Me.AuthOAuthRadio.Name = "AuthOAuthRadio"
        Me.AuthOAuthRadio.TabStop = True
        Me.ToolTip1.SetToolTip(Me.AuthOAuthRadio, resources.GetString("AuthOAuthRadio.ToolTip"))
        Me.AuthOAuthRadio.UseVisualStyleBackColor = True
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        Me.ToolTip1.SetToolTip(Me.Label6, resources.GetString("Label6.ToolTip"))
        '
        'AuthClearButton
        '
        resources.ApplyResources(Me.AuthClearButton, "AuthClearButton")
        Me.AuthClearButton.Name = "AuthClearButton"
        Me.ToolTip1.SetToolTip(Me.AuthClearButton, resources.GetString("AuthClearButton.ToolTip"))
        Me.AuthClearButton.UseVisualStyleBackColor = True
        '
        'AuthUserLabel
        '
        resources.ApplyResources(Me.AuthUserLabel, "AuthUserLabel")
        Me.AuthUserLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AuthUserLabel.Name = "AuthUserLabel"
        Me.ToolTip1.SetToolTip(Me.AuthUserLabel, resources.GetString("AuthUserLabel.ToolTip"))
        '
        'AuthStateLabel
        '
        resources.ApplyResources(Me.AuthStateLabel, "AuthStateLabel")
        Me.AuthStateLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AuthStateLabel.Name = "AuthStateLabel"
        Me.ToolTip1.SetToolTip(Me.AuthStateLabel, resources.GetString("AuthStateLabel.ToolTip"))
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        Me.ToolTip1.SetToolTip(Me.Label4, resources.GetString("Label4.ToolTip"))
        '
        'AuthorizeButton
        '
        resources.ApplyResources(Me.AuthorizeButton, "AuthorizeButton")
        Me.AuthorizeButton.Name = "AuthorizeButton"
        Me.ToolTip1.SetToolTip(Me.AuthorizeButton, resources.GetString("AuthorizeButton.ToolTip"))
        Me.AuthorizeButton.UseVisualStyleBackColor = True
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        Me.ToolTip1.SetToolTip(Me.Label1, resources.GetString("Label1.ToolTip"))
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        Me.ToolTip1.SetToolTip(Me.Label2, resources.GetString("Label2.ToolTip"))
        '
        'Username
        '
        resources.ApplyResources(Me.Username, "Username")
        Me.Username.Name = "Username"
        Me.ToolTip1.SetToolTip(Me.Username, resources.GetString("Username.ToolTip"))
        '
        'Password
        '
        resources.ApplyResources(Me.Password, "Password")
        Me.Password.Name = "Password"
        Me.ToolTip1.SetToolTip(Me.Password, resources.GetString("Password.ToolTip"))
        Me.Password.UseSystemPasswordChar = True
        '
        'FontPanel
        '
        resources.ApplyResources(Me.FontPanel, "FontPanel")
        Me.FontPanel.Controls.Add(Me.GroupBox1)
        Me.FontPanel.Name = "FontPanel"
        Me.ToolTip1.SetToolTip(Me.FontPanel, resources.GetString("FontPanel.ToolTip"))
        '
        'GroupBox1
        '
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
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
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        Me.ToolTip1.SetToolTip(Me.GroupBox1, resources.GetString("GroupBox1.ToolTip"))
        '
        'btnRetweet
        '
        resources.ApplyResources(Me.btnRetweet, "btnRetweet")
        Me.btnRetweet.Name = "btnRetweet"
        Me.ToolTip1.SetToolTip(Me.btnRetweet, resources.GetString("btnRetweet.ToolTip"))
        Me.btnRetweet.UseVisualStyleBackColor = True
        '
        'lblRetweet
        '
        resources.ApplyResources(Me.lblRetweet, "lblRetweet")
        Me.lblRetweet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblRetweet.Name = "lblRetweet"
        Me.ToolTip1.SetToolTip(Me.lblRetweet, resources.GetString("lblRetweet.ToolTip"))
        '
        'Label80
        '
        resources.ApplyResources(Me.Label80, "Label80")
        Me.Label80.Name = "Label80"
        Me.ToolTip1.SetToolTip(Me.Label80, resources.GetString("Label80.ToolTip"))
        '
        'ButtonBackToDefaultFontColor
        '
        resources.ApplyResources(Me.ButtonBackToDefaultFontColor, "ButtonBackToDefaultFontColor")
        Me.ButtonBackToDefaultFontColor.Name = "ButtonBackToDefaultFontColor"
        Me.ToolTip1.SetToolTip(Me.ButtonBackToDefaultFontColor, resources.GetString("ButtonBackToDefaultFontColor.ToolTip"))
        Me.ButtonBackToDefaultFontColor.UseVisualStyleBackColor = True
        '
        'btnDetailLink
        '
        resources.ApplyResources(Me.btnDetailLink, "btnDetailLink")
        Me.btnDetailLink.Name = "btnDetailLink"
        Me.ToolTip1.SetToolTip(Me.btnDetailLink, resources.GetString("btnDetailLink.ToolTip"))
        Me.btnDetailLink.UseVisualStyleBackColor = True
        '
        'lblDetailLink
        '
        resources.ApplyResources(Me.lblDetailLink, "lblDetailLink")
        Me.lblDetailLink.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDetailLink.Name = "lblDetailLink"
        Me.ToolTip1.SetToolTip(Me.lblDetailLink, resources.GetString("lblDetailLink.ToolTip"))
        '
        'Label18
        '
        resources.ApplyResources(Me.Label18, "Label18")
        Me.Label18.Name = "Label18"
        Me.ToolTip1.SetToolTip(Me.Label18, resources.GetString("Label18.ToolTip"))
        '
        'btnUnread
        '
        resources.ApplyResources(Me.btnUnread, "btnUnread")
        Me.btnUnread.Name = "btnUnread"
        Me.ToolTip1.SetToolTip(Me.btnUnread, resources.GetString("btnUnread.ToolTip"))
        Me.btnUnread.UseVisualStyleBackColor = True
        '
        'lblUnread
        '
        resources.ApplyResources(Me.lblUnread, "lblUnread")
        Me.lblUnread.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblUnread.Name = "lblUnread"
        Me.ToolTip1.SetToolTip(Me.lblUnread, resources.GetString("lblUnread.ToolTip"))
        '
        'Label20
        '
        resources.ApplyResources(Me.Label20, "Label20")
        Me.Label20.Name = "Label20"
        Me.ToolTip1.SetToolTip(Me.Label20, resources.GetString("Label20.ToolTip"))
        '
        'btnDetailBack
        '
        resources.ApplyResources(Me.btnDetailBack, "btnDetailBack")
        Me.btnDetailBack.Name = "btnDetailBack"
        Me.ToolTip1.SetToolTip(Me.btnDetailBack, resources.GetString("btnDetailBack.ToolTip"))
        Me.btnDetailBack.UseVisualStyleBackColor = True
        '
        'lblDetailBackcolor
        '
        resources.ApplyResources(Me.lblDetailBackcolor, "lblDetailBackcolor")
        Me.lblDetailBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDetailBackcolor.Name = "lblDetailBackcolor"
        Me.ToolTip1.SetToolTip(Me.lblDetailBackcolor, resources.GetString("lblDetailBackcolor.ToolTip"))
        '
        'Label37
        '
        resources.ApplyResources(Me.Label37, "Label37")
        Me.Label37.Name = "Label37"
        Me.ToolTip1.SetToolTip(Me.Label37, resources.GetString("Label37.ToolTip"))
        '
        'btnDetail
        '
        resources.ApplyResources(Me.btnDetail, "btnDetail")
        Me.btnDetail.Name = "btnDetail"
        Me.ToolTip1.SetToolTip(Me.btnDetail, resources.GetString("btnDetail.ToolTip"))
        Me.btnDetail.UseVisualStyleBackColor = True
        '
        'lblDetail
        '
        resources.ApplyResources(Me.lblDetail, "lblDetail")
        Me.lblDetail.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDetail.Name = "lblDetail"
        Me.ToolTip1.SetToolTip(Me.lblDetail, resources.GetString("lblDetail.ToolTip"))
        '
        'Label26
        '
        resources.ApplyResources(Me.Label26, "Label26")
        Me.Label26.Name = "Label26"
        Me.ToolTip1.SetToolTip(Me.Label26, resources.GetString("Label26.ToolTip"))
        '
        'btnOWL
        '
        resources.ApplyResources(Me.btnOWL, "btnOWL")
        Me.btnOWL.Name = "btnOWL"
        Me.ToolTip1.SetToolTip(Me.btnOWL, resources.GetString("btnOWL.ToolTip"))
        Me.btnOWL.UseVisualStyleBackColor = True
        '
        'lblOWL
        '
        resources.ApplyResources(Me.lblOWL, "lblOWL")
        Me.lblOWL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblOWL.Name = "lblOWL"
        Me.ToolTip1.SetToolTip(Me.lblOWL, resources.GetString("lblOWL.ToolTip"))
        '
        'Label24
        '
        resources.ApplyResources(Me.Label24, "Label24")
        Me.Label24.Name = "Label24"
        Me.ToolTip1.SetToolTip(Me.Label24, resources.GetString("Label24.ToolTip"))
        '
        'btnFav
        '
        resources.ApplyResources(Me.btnFav, "btnFav")
        Me.btnFav.Name = "btnFav"
        Me.ToolTip1.SetToolTip(Me.btnFav, resources.GetString("btnFav.ToolTip"))
        Me.btnFav.UseVisualStyleBackColor = True
        '
        'lblFav
        '
        resources.ApplyResources(Me.lblFav, "lblFav")
        Me.lblFav.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFav.Name = "lblFav"
        Me.ToolTip1.SetToolTip(Me.lblFav, resources.GetString("lblFav.ToolTip"))
        '
        'Label22
        '
        resources.ApplyResources(Me.Label22, "Label22")
        Me.Label22.Name = "Label22"
        Me.ToolTip1.SetToolTip(Me.Label22, resources.GetString("Label22.ToolTip"))
        '
        'btnListFont
        '
        resources.ApplyResources(Me.btnListFont, "btnListFont")
        Me.btnListFont.Name = "btnListFont"
        Me.ToolTip1.SetToolTip(Me.btnListFont, resources.GetString("btnListFont.ToolTip"))
        Me.btnListFont.UseVisualStyleBackColor = True
        '
        'lblListFont
        '
        resources.ApplyResources(Me.lblListFont, "lblListFont")
        Me.lblListFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblListFont.Name = "lblListFont"
        Me.ToolTip1.SetToolTip(Me.lblListFont, resources.GetString("lblListFont.ToolTip"))
        '
        'Label61
        '
        resources.ApplyResources(Me.Label61, "Label61")
        Me.Label61.Name = "Label61"
        Me.ToolTip1.SetToolTip(Me.Label61, resources.GetString("Label61.ToolTip"))
        '
        'ShortUrlPanel
        '
        resources.ApplyResources(Me.ShortUrlPanel, "ShortUrlPanel")
        Me.ShortUrlPanel.Controls.Add(Me.CheckTinyURL)
        Me.ShortUrlPanel.Controls.Add(Me.TextBitlyPw)
        Me.ShortUrlPanel.Controls.Add(Me.CheckAutoConvertUrl)
        Me.ShortUrlPanel.Controls.Add(Me.Label71)
        Me.ShortUrlPanel.Controls.Add(Me.ComboBoxAutoShortUrlFirst)
        Me.ShortUrlPanel.Controls.Add(Me.Label76)
        Me.ShortUrlPanel.Controls.Add(Me.Label77)
        Me.ShortUrlPanel.Controls.Add(Me.TextBitlyId)
        Me.ShortUrlPanel.Name = "ShortUrlPanel"
        Me.ToolTip1.SetToolTip(Me.ShortUrlPanel, resources.GetString("ShortUrlPanel.ToolTip"))
        '
        'CheckTinyURL
        '
        resources.ApplyResources(Me.CheckTinyURL, "CheckTinyURL")
        Me.CheckTinyURL.Name = "CheckTinyURL"
        Me.ToolTip1.SetToolTip(Me.CheckTinyURL, resources.GetString("CheckTinyURL.ToolTip"))
        Me.CheckTinyURL.UseVisualStyleBackColor = True
        '
        'TextBitlyPw
        '
        resources.ApplyResources(Me.TextBitlyPw, "TextBitlyPw")
        Me.TextBitlyPw.Name = "TextBitlyPw"
        Me.ToolTip1.SetToolTip(Me.TextBitlyPw, resources.GetString("TextBitlyPw.ToolTip"))
        '
        'CheckAutoConvertUrl
        '
        resources.ApplyResources(Me.CheckAutoConvertUrl, "CheckAutoConvertUrl")
        Me.CheckAutoConvertUrl.Name = "CheckAutoConvertUrl"
        Me.ToolTip1.SetToolTip(Me.CheckAutoConvertUrl, resources.GetString("CheckAutoConvertUrl.ToolTip"))
        Me.CheckAutoConvertUrl.UseVisualStyleBackColor = True
        '
        'Label71
        '
        resources.ApplyResources(Me.Label71, "Label71")
        Me.Label71.Name = "Label71"
        Me.ToolTip1.SetToolTip(Me.Label71, resources.GetString("Label71.ToolTip"))
        '
        'ComboBoxAutoShortUrlFirst
        '
        resources.ApplyResources(Me.ComboBoxAutoShortUrlFirst, "ComboBoxAutoShortUrlFirst")
        Me.ComboBoxAutoShortUrlFirst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxAutoShortUrlFirst.FormattingEnabled = True
        Me.ComboBoxAutoShortUrlFirst.Items.AddRange(New Object() {resources.GetString("ComboBoxAutoShortUrlFirst.Items"), resources.GetString("ComboBoxAutoShortUrlFirst.Items1"), resources.GetString("ComboBoxAutoShortUrlFirst.Items2"), resources.GetString("ComboBoxAutoShortUrlFirst.Items3"), resources.GetString("ComboBoxAutoShortUrlFirst.Items4"), resources.GetString("ComboBoxAutoShortUrlFirst.Items5")})
        Me.ComboBoxAutoShortUrlFirst.Name = "ComboBoxAutoShortUrlFirst"
        Me.ToolTip1.SetToolTip(Me.ComboBoxAutoShortUrlFirst, resources.GetString("ComboBoxAutoShortUrlFirst.ToolTip"))
        '
        'Label76
        '
        resources.ApplyResources(Me.Label76, "Label76")
        Me.Label76.Name = "Label76"
        Me.ToolTip1.SetToolTip(Me.Label76, resources.GetString("Label76.ToolTip"))
        '
        'Label77
        '
        resources.ApplyResources(Me.Label77, "Label77")
        Me.Label77.Name = "Label77"
        Me.ToolTip1.SetToolTip(Me.Label77, resources.GetString("Label77.ToolTip"))
        '
        'TextBitlyId
        '
        resources.ApplyResources(Me.TextBitlyId, "TextBitlyId")
        Me.TextBitlyId.Name = "TextBitlyId"
        Me.ToolTip1.SetToolTip(Me.TextBitlyId, resources.GetString("TextBitlyId.ToolTip"))
        '
        'ProxyPanel
        '
        resources.ApplyResources(Me.ProxyPanel, "ProxyPanel")
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
        Me.ProxyPanel.Name = "ProxyPanel"
        Me.ToolTip1.SetToolTip(Me.ProxyPanel, resources.GetString("ProxyPanel.ToolTip"))
        '
        'Label55
        '
        resources.ApplyResources(Me.Label55, "Label55")
        Me.Label55.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label55.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label55.Name = "Label55"
        Me.ToolTip1.SetToolTip(Me.Label55, resources.GetString("Label55.ToolTip"))
        '
        'TextProxyPassword
        '
        resources.ApplyResources(Me.TextProxyPassword, "TextProxyPassword")
        Me.TextProxyPassword.Name = "TextProxyPassword"
        Me.ToolTip1.SetToolTip(Me.TextProxyPassword, resources.GetString("TextProxyPassword.ToolTip"))
        Me.TextProxyPassword.UseSystemPasswordChar = True
        '
        'RadioProxyNone
        '
        resources.ApplyResources(Me.RadioProxyNone, "RadioProxyNone")
        Me.RadioProxyNone.Name = "RadioProxyNone"
        Me.ToolTip1.SetToolTip(Me.RadioProxyNone, resources.GetString("RadioProxyNone.ToolTip"))
        Me.RadioProxyNone.UseVisualStyleBackColor = True
        '
        'LabelProxyPassword
        '
        resources.ApplyResources(Me.LabelProxyPassword, "LabelProxyPassword")
        Me.LabelProxyPassword.Name = "LabelProxyPassword"
        Me.ToolTip1.SetToolTip(Me.LabelProxyPassword, resources.GetString("LabelProxyPassword.ToolTip"))
        '
        'RadioProxyIE
        '
        resources.ApplyResources(Me.RadioProxyIE, "RadioProxyIE")
        Me.RadioProxyIE.Checked = True
        Me.RadioProxyIE.Name = "RadioProxyIE"
        Me.RadioProxyIE.TabStop = True
        Me.ToolTip1.SetToolTip(Me.RadioProxyIE, resources.GetString("RadioProxyIE.ToolTip"))
        Me.RadioProxyIE.UseVisualStyleBackColor = True
        '
        'TextProxyUser
        '
        resources.ApplyResources(Me.TextProxyUser, "TextProxyUser")
        Me.TextProxyUser.Name = "TextProxyUser"
        Me.ToolTip1.SetToolTip(Me.TextProxyUser, resources.GetString("TextProxyUser.ToolTip"))
        '
        'RadioProxySpecified
        '
        resources.ApplyResources(Me.RadioProxySpecified, "RadioProxySpecified")
        Me.RadioProxySpecified.Name = "RadioProxySpecified"
        Me.ToolTip1.SetToolTip(Me.RadioProxySpecified, resources.GetString("RadioProxySpecified.ToolTip"))
        Me.RadioProxySpecified.UseVisualStyleBackColor = True
        '
        'LabelProxyUser
        '
        resources.ApplyResources(Me.LabelProxyUser, "LabelProxyUser")
        Me.LabelProxyUser.Name = "LabelProxyUser"
        Me.ToolTip1.SetToolTip(Me.LabelProxyUser, resources.GetString("LabelProxyUser.ToolTip"))
        '
        'LabelProxyAddress
        '
        resources.ApplyResources(Me.LabelProxyAddress, "LabelProxyAddress")
        Me.LabelProxyAddress.Name = "LabelProxyAddress"
        Me.ToolTip1.SetToolTip(Me.LabelProxyAddress, resources.GetString("LabelProxyAddress.ToolTip"))
        '
        'TextProxyPort
        '
        resources.ApplyResources(Me.TextProxyPort, "TextProxyPort")
        Me.TextProxyPort.Name = "TextProxyPort"
        Me.ToolTip1.SetToolTip(Me.TextProxyPort, resources.GetString("TextProxyPort.ToolTip"))
        '
        'TextProxyAddress
        '
        resources.ApplyResources(Me.TextProxyAddress, "TextProxyAddress")
        Me.TextProxyAddress.Name = "TextProxyAddress"
        Me.ToolTip1.SetToolTip(Me.TextProxyAddress, resources.GetString("TextProxyAddress.ToolTip"))
        '
        'LabelProxyPort
        '
        resources.ApplyResources(Me.LabelProxyPort, "LabelProxyPort")
        Me.LabelProxyPort.Name = "LabelProxyPort"
        Me.ToolTip1.SetToolTip(Me.LabelProxyPort, resources.GetString("LabelProxyPort.ToolTip"))
        '
        'ConnectionPanel
        '
        resources.ApplyResources(Me.ConnectionPanel, "ConnectionPanel")
        Me.ConnectionPanel.Controls.Add(Me.CheckEnableBasicAuth)
        Me.ConnectionPanel.Controls.Add(Me.TwitterSearchAPIText)
        Me.ConnectionPanel.Controls.Add(Me.Label31)
        Me.ConnectionPanel.Controls.Add(Me.TwitterAPIText)
        Me.ConnectionPanel.Controls.Add(Me.Label8)
        Me.ConnectionPanel.Controls.Add(Me.CheckUseSsl)
        Me.ConnectionPanel.Controls.Add(Me.Label64)
        Me.ConnectionPanel.Controls.Add(Me.ConnectionTimeOut)
        Me.ConnectionPanel.Controls.Add(Me.Label63)
        Me.ConnectionPanel.Name = "ConnectionPanel"
        Me.ToolTip1.SetToolTip(Me.ConnectionPanel, resources.GetString("ConnectionPanel.ToolTip"))
        '
        'CheckEnableBasicAuth
        '
        resources.ApplyResources(Me.CheckEnableBasicAuth, "CheckEnableBasicAuth")
        Me.CheckEnableBasicAuth.Name = "CheckEnableBasicAuth"
        Me.ToolTip1.SetToolTip(Me.CheckEnableBasicAuth, resources.GetString("CheckEnableBasicAuth.ToolTip"))
        Me.CheckEnableBasicAuth.UseVisualStyleBackColor = True
        '
        'TwitterSearchAPIText
        '
        resources.ApplyResources(Me.TwitterSearchAPIText, "TwitterSearchAPIText")
        Me.TwitterSearchAPIText.Name = "TwitterSearchAPIText"
        Me.ToolTip1.SetToolTip(Me.TwitterSearchAPIText, resources.GetString("TwitterSearchAPIText.ToolTip"))
        '
        'Label31
        '
        resources.ApplyResources(Me.Label31, "Label31")
        Me.Label31.Name = "Label31"
        Me.ToolTip1.SetToolTip(Me.Label31, resources.GetString("Label31.ToolTip"))
        '
        'TwitterAPIText
        '
        resources.ApplyResources(Me.TwitterAPIText, "TwitterAPIText")
        Me.TwitterAPIText.Name = "TwitterAPIText"
        Me.ToolTip1.SetToolTip(Me.TwitterAPIText, resources.GetString("TwitterAPIText.ToolTip"))
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        Me.ToolTip1.SetToolTip(Me.Label8, resources.GetString("Label8.ToolTip"))
        '
        'CheckUseSsl
        '
        resources.ApplyResources(Me.CheckUseSsl, "CheckUseSsl")
        Me.CheckUseSsl.Name = "CheckUseSsl"
        Me.ToolTip1.SetToolTip(Me.CheckUseSsl, resources.GetString("CheckUseSsl.ToolTip"))
        Me.CheckUseSsl.UseVisualStyleBackColor = True
        '
        'Label64
        '
        resources.ApplyResources(Me.Label64, "Label64")
        Me.Label64.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label64.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label64.Name = "Label64"
        Me.ToolTip1.SetToolTip(Me.Label64, resources.GetString("Label64.ToolTip"))
        '
        'ConnectionTimeOut
        '
        resources.ApplyResources(Me.ConnectionTimeOut, "ConnectionTimeOut")
        Me.ConnectionTimeOut.Name = "ConnectionTimeOut"
        Me.ToolTip1.SetToolTip(Me.ConnectionTimeOut, resources.GetString("ConnectionTimeOut.ToolTip"))
        '
        'Label63
        '
        resources.ApplyResources(Me.Label63, "Label63")
        Me.Label63.Name = "Label63"
        Me.ToolTip1.SetToolTip(Me.Label63, resources.GetString("Label63.ToolTip"))
        '
        'FontPanel2
        '
        resources.ApplyResources(Me.FontPanel2, "FontPanel2")
        Me.FontPanel2.Controls.Add(Me.GroupBox5)
        Me.FontPanel2.Name = "FontPanel2"
        Me.ToolTip1.SetToolTip(Me.FontPanel2, resources.GetString("FontPanel2.ToolTip"))
        '
        'GroupBox5
        '
        resources.ApplyResources(Me.GroupBox5, "GroupBox5")
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
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.TabStop = False
        Me.ToolTip1.SetToolTip(Me.GroupBox5, resources.GetString("GroupBox5.ToolTip"))
        '
        'Label65
        '
        resources.ApplyResources(Me.Label65, "Label65")
        Me.Label65.Name = "Label65"
        Me.ToolTip1.SetToolTip(Me.Label65, resources.GetString("Label65.ToolTip"))
        '
        'Label52
        '
        resources.ApplyResources(Me.Label52, "Label52")
        Me.Label52.Name = "Label52"
        Me.ToolTip1.SetToolTip(Me.Label52, resources.GetString("Label52.ToolTip"))
        '
        'Label49
        '
        resources.ApplyResources(Me.Label49, "Label49")
        Me.Label49.Name = "Label49"
        Me.ToolTip1.SetToolTip(Me.Label49, resources.GetString("Label49.ToolTip"))
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        Me.ToolTip1.SetToolTip(Me.Label9, resources.GetString("Label9.ToolTip"))
        '
        'Label14
        '
        resources.ApplyResources(Me.Label14, "Label14")
        Me.Label14.Name = "Label14"
        Me.ToolTip1.SetToolTip(Me.Label14, resources.GetString("Label14.ToolTip"))
        '
        'Label16
        '
        resources.ApplyResources(Me.Label16, "Label16")
        Me.Label16.Name = "Label16"
        Me.ToolTip1.SetToolTip(Me.Label16, resources.GetString("Label16.ToolTip"))
        '
        'Label32
        '
        resources.ApplyResources(Me.Label32, "Label32")
        Me.Label32.Name = "Label32"
        Me.ToolTip1.SetToolTip(Me.Label32, resources.GetString("Label32.ToolTip"))
        '
        'Label34
        '
        resources.ApplyResources(Me.Label34, "Label34")
        Me.Label34.Name = "Label34"
        Me.ToolTip1.SetToolTip(Me.Label34, resources.GetString("Label34.ToolTip"))
        '
        'Label36
        '
        resources.ApplyResources(Me.Label36, "Label36")
        Me.Label36.Name = "Label36"
        Me.ToolTip1.SetToolTip(Me.Label36, resources.GetString("Label36.ToolTip"))
        '
        'btnInputFont
        '
        resources.ApplyResources(Me.btnInputFont, "btnInputFont")
        Me.btnInputFont.Name = "btnInputFont"
        Me.ToolTip1.SetToolTip(Me.btnInputFont, resources.GetString("btnInputFont.ToolTip"))
        Me.btnInputFont.UseVisualStyleBackColor = True
        '
        'btnInputBackcolor
        '
        resources.ApplyResources(Me.btnInputBackcolor, "btnInputBackcolor")
        Me.btnInputBackcolor.Name = "btnInputBackcolor"
        Me.ToolTip1.SetToolTip(Me.btnInputBackcolor, resources.GetString("btnInputBackcolor.ToolTip"))
        Me.btnInputBackcolor.UseVisualStyleBackColor = True
        '
        'btnAtTo
        '
        resources.ApplyResources(Me.btnAtTo, "btnAtTo")
        Me.btnAtTo.Name = "btnAtTo"
        Me.ToolTip1.SetToolTip(Me.btnAtTo, resources.GetString("btnAtTo.ToolTip"))
        Me.btnAtTo.UseVisualStyleBackColor = True
        '
        'btnListBack
        '
        resources.ApplyResources(Me.btnListBack, "btnListBack")
        Me.btnListBack.Name = "btnListBack"
        Me.ToolTip1.SetToolTip(Me.btnListBack, resources.GetString("btnListBack.ToolTip"))
        Me.btnListBack.UseVisualStyleBackColor = True
        '
        'btnAtFromTarget
        '
        resources.ApplyResources(Me.btnAtFromTarget, "btnAtFromTarget")
        Me.btnAtFromTarget.Name = "btnAtFromTarget"
        Me.ToolTip1.SetToolTip(Me.btnAtFromTarget, resources.GetString("btnAtFromTarget.ToolTip"))
        Me.btnAtFromTarget.UseVisualStyleBackColor = True
        '
        'btnAtTarget
        '
        resources.ApplyResources(Me.btnAtTarget, "btnAtTarget")
        Me.btnAtTarget.Name = "btnAtTarget"
        Me.ToolTip1.SetToolTip(Me.btnAtTarget, resources.GetString("btnAtTarget.ToolTip"))
        Me.btnAtTarget.UseVisualStyleBackColor = True
        '
        'btnTarget
        '
        resources.ApplyResources(Me.btnTarget, "btnTarget")
        Me.btnTarget.Name = "btnTarget"
        Me.ToolTip1.SetToolTip(Me.btnTarget, resources.GetString("btnTarget.ToolTip"))
        Me.btnTarget.UseVisualStyleBackColor = True
        '
        'btnAtSelf
        '
        resources.ApplyResources(Me.btnAtSelf, "btnAtSelf")
        Me.btnAtSelf.Name = "btnAtSelf"
        Me.ToolTip1.SetToolTip(Me.btnAtSelf, resources.GetString("btnAtSelf.ToolTip"))
        Me.btnAtSelf.UseVisualStyleBackColor = True
        '
        'btnSelf
        '
        resources.ApplyResources(Me.btnSelf, "btnSelf")
        Me.btnSelf.Name = "btnSelf"
        Me.ToolTip1.SetToolTip(Me.btnSelf, resources.GetString("btnSelf.ToolTip"))
        Me.btnSelf.UseVisualStyleBackColor = True
        '
        'lblInputFont
        '
        resources.ApplyResources(Me.lblInputFont, "lblInputFont")
        Me.lblInputFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblInputFont.Name = "lblInputFont"
        Me.ToolTip1.SetToolTip(Me.lblInputFont, resources.GetString("lblInputFont.ToolTip"))
        '
        'lblInputBackcolor
        '
        resources.ApplyResources(Me.lblInputBackcolor, "lblInputBackcolor")
        Me.lblInputBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblInputBackcolor.Name = "lblInputBackcolor"
        Me.ToolTip1.SetToolTip(Me.lblInputBackcolor, resources.GetString("lblInputBackcolor.ToolTip"))
        '
        'lblAtTo
        '
        resources.ApplyResources(Me.lblAtTo, "lblAtTo")
        Me.lblAtTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtTo.Name = "lblAtTo"
        Me.ToolTip1.SetToolTip(Me.lblAtTo, resources.GetString("lblAtTo.ToolTip"))
        '
        'lblListBackcolor
        '
        resources.ApplyResources(Me.lblListBackcolor, "lblListBackcolor")
        Me.lblListBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblListBackcolor.Name = "lblListBackcolor"
        Me.ToolTip1.SetToolTip(Me.lblListBackcolor, resources.GetString("lblListBackcolor.ToolTip"))
        '
        'lblAtFromTarget
        '
        resources.ApplyResources(Me.lblAtFromTarget, "lblAtFromTarget")
        Me.lblAtFromTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtFromTarget.Name = "lblAtFromTarget"
        Me.ToolTip1.SetToolTip(Me.lblAtFromTarget, resources.GetString("lblAtFromTarget.ToolTip"))
        '
        'lblAtTarget
        '
        resources.ApplyResources(Me.lblAtTarget, "lblAtTarget")
        Me.lblAtTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtTarget.Name = "lblAtTarget"
        Me.ToolTip1.SetToolTip(Me.lblAtTarget, resources.GetString("lblAtTarget.ToolTip"))
        '
        'lblTarget
        '
        resources.ApplyResources(Me.lblTarget, "lblTarget")
        Me.lblTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTarget.Name = "lblTarget"
        Me.ToolTip1.SetToolTip(Me.lblTarget, resources.GetString("lblTarget.ToolTip"))
        '
        'lblAtSelf
        '
        resources.ApplyResources(Me.lblAtSelf, "lblAtSelf")
        Me.lblAtSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtSelf.Name = "lblAtSelf"
        Me.ToolTip1.SetToolTip(Me.lblAtSelf, resources.GetString("lblAtSelf.ToolTip"))
        '
        'lblSelf
        '
        resources.ApplyResources(Me.lblSelf, "lblSelf")
        Me.lblSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSelf.Name = "lblSelf"
        Me.ToolTip1.SetToolTip(Me.lblSelf, resources.GetString("lblSelf.ToolTip"))
        '
        'ButtonBackToDefaultFontColor2
        '
        resources.ApplyResources(Me.ButtonBackToDefaultFontColor2, "ButtonBackToDefaultFontColor2")
        Me.ButtonBackToDefaultFontColor2.Name = "ButtonBackToDefaultFontColor2"
        Me.ToolTip1.SetToolTip(Me.ButtonBackToDefaultFontColor2, resources.GetString("ButtonBackToDefaultFontColor2.ToolTip"))
        Me.ButtonBackToDefaultFontColor2.UseVisualStyleBackColor = True
        '
        'TweetActPanel
        '
        resources.ApplyResources(Me.TweetActPanel, "TweetActPanel")
        Me.TweetActPanel.Controls.Add(Me.CheckHashSupple)
        Me.TweetActPanel.Controls.Add(Me.CheckAtIdSupple)
        Me.TweetActPanel.Controls.Add(Me.ComboBoxPostKeySelect)
        Me.TweetActPanel.Controls.Add(Me.Label27)
        Me.TweetActPanel.Controls.Add(Me.CheckRetweetNoConfirm)
        Me.TweetActPanel.Controls.Add(Me.Label12)
        Me.TweetActPanel.Controls.Add(Me.CheckUseRecommendStatus)
        Me.TweetActPanel.Controls.Add(Me.StatusText)
        Me.TweetActPanel.Name = "TweetActPanel"
        Me.ToolTip1.SetToolTip(Me.TweetActPanel, resources.GetString("TweetActPanel.ToolTip"))
        '
        'CheckHashSupple
        '
        resources.ApplyResources(Me.CheckHashSupple, "CheckHashSupple")
        Me.CheckHashSupple.Name = "CheckHashSupple"
        Me.ToolTip1.SetToolTip(Me.CheckHashSupple, resources.GetString("CheckHashSupple.ToolTip"))
        Me.CheckHashSupple.UseVisualStyleBackColor = True
        '
        'CheckAtIdSupple
        '
        resources.ApplyResources(Me.CheckAtIdSupple, "CheckAtIdSupple")
        Me.CheckAtIdSupple.Name = "CheckAtIdSupple"
        Me.ToolTip1.SetToolTip(Me.CheckAtIdSupple, resources.GetString("CheckAtIdSupple.ToolTip"))
        Me.CheckAtIdSupple.UseVisualStyleBackColor = True
        '
        'ComboBoxPostKeySelect
        '
        resources.ApplyResources(Me.ComboBoxPostKeySelect, "ComboBoxPostKeySelect")
        Me.ComboBoxPostKeySelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxPostKeySelect.FormattingEnabled = True
        Me.ComboBoxPostKeySelect.Items.AddRange(New Object() {resources.GetString("ComboBoxPostKeySelect.Items"), resources.GetString("ComboBoxPostKeySelect.Items1"), resources.GetString("ComboBoxPostKeySelect.Items2")})
        Me.ComboBoxPostKeySelect.Name = "ComboBoxPostKeySelect"
        Me.ToolTip1.SetToolTip(Me.ComboBoxPostKeySelect, resources.GetString("ComboBoxPostKeySelect.ToolTip"))
        '
        'Label27
        '
        resources.ApplyResources(Me.Label27, "Label27")
        Me.Label27.Name = "Label27"
        Me.ToolTip1.SetToolTip(Me.Label27, resources.GetString("Label27.ToolTip"))
        '
        'CheckRetweetNoConfirm
        '
        resources.ApplyResources(Me.CheckRetweetNoConfirm, "CheckRetweetNoConfirm")
        Me.CheckRetweetNoConfirm.Name = "CheckRetweetNoConfirm"
        Me.ToolTip1.SetToolTip(Me.CheckRetweetNoConfirm, resources.GetString("CheckRetweetNoConfirm.ToolTip"))
        Me.CheckRetweetNoConfirm.UseVisualStyleBackColor = True
        '
        'Label12
        '
        resources.ApplyResources(Me.Label12, "Label12")
        Me.Label12.Name = "Label12"
        Me.ToolTip1.SetToolTip(Me.Label12, resources.GetString("Label12.ToolTip"))
        '
        'CheckUseRecommendStatus
        '
        resources.ApplyResources(Me.CheckUseRecommendStatus, "CheckUseRecommendStatus")
        Me.CheckUseRecommendStatus.Name = "CheckUseRecommendStatus"
        Me.ToolTip1.SetToolTip(Me.CheckUseRecommendStatus, resources.GetString("CheckUseRecommendStatus.ToolTip"))
        Me.CheckUseRecommendStatus.UseVisualStyleBackColor = True
        '
        'StatusText
        '
        resources.ApplyResources(Me.StatusText, "StatusText")
        Me.StatusText.Name = "StatusText"
        Me.ToolTip1.SetToolTip(Me.StatusText, resources.GetString("StatusText.ToolTip"))
        '
        'StartupPanel
        '
        resources.ApplyResources(Me.StartupPanel, "StartupPanel")
        Me.StartupPanel.Controls.Add(Me.StartupReaded)
        Me.StartupPanel.Controls.Add(Me.CheckStartupFollowers)
        Me.StartupPanel.Controls.Add(Me.CheckStartupVersion)
        Me.StartupPanel.Controls.Add(Me.chkGetFav)
        Me.StartupPanel.Name = "StartupPanel"
        Me.ToolTip1.SetToolTip(Me.StartupPanel, resources.GetString("StartupPanel.ToolTip"))
        '
        'StartupReaded
        '
        resources.ApplyResources(Me.StartupReaded, "StartupReaded")
        Me.StartupReaded.Name = "StartupReaded"
        Me.ToolTip1.SetToolTip(Me.StartupReaded, resources.GetString("StartupReaded.ToolTip"))
        Me.StartupReaded.UseVisualStyleBackColor = True
        '
        'CheckStartupFollowers
        '
        resources.ApplyResources(Me.CheckStartupFollowers, "CheckStartupFollowers")
        Me.CheckStartupFollowers.Name = "CheckStartupFollowers"
        Me.ToolTip1.SetToolTip(Me.CheckStartupFollowers, resources.GetString("CheckStartupFollowers.ToolTip"))
        Me.CheckStartupFollowers.UseVisualStyleBackColor = True
        '
        'CheckStartupVersion
        '
        resources.ApplyResources(Me.CheckStartupVersion, "CheckStartupVersion")
        Me.CheckStartupVersion.Name = "CheckStartupVersion"
        Me.ToolTip1.SetToolTip(Me.CheckStartupVersion, resources.GetString("CheckStartupVersion.ToolTip"))
        Me.CheckStartupVersion.UseVisualStyleBackColor = True
        '
        'chkGetFav
        '
        resources.ApplyResources(Me.chkGetFav, "chkGetFav")
        Me.chkGetFav.Name = "chkGetFav"
        Me.ToolTip1.SetToolTip(Me.chkGetFav, resources.GetString("chkGetFav.ToolTip"))
        Me.chkGetFav.UseVisualStyleBackColor = True
        '
        'GetCountPanel
        '
        resources.ApplyResources(Me.GetCountPanel, "GetCountPanel")
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
        Me.GetCountPanel.Name = "GetCountPanel"
        Me.ToolTip1.SetToolTip(Me.GetCountPanel, resources.GetString("GetCountPanel.ToolTip"))
        '
        'ListTextCountApi
        '
        resources.ApplyResources(Me.ListTextCountApi, "ListTextCountApi")
        Me.ListTextCountApi.Name = "ListTextCountApi"
        Me.ToolTip1.SetToolTip(Me.ListTextCountApi, resources.GetString("ListTextCountApi.ToolTip"))
        '
        'Label25
        '
        resources.ApplyResources(Me.Label25, "Label25")
        Me.Label25.Name = "Label25"
        Me.ToolTip1.SetToolTip(Me.Label25, resources.GetString("Label25.ToolTip"))
        '
        'UserTimelineTextCountApi
        '
        resources.ApplyResources(Me.UserTimelineTextCountApi, "UserTimelineTextCountApi")
        Me.UserTimelineTextCountApi.Name = "UserTimelineTextCountApi"
        Me.ToolTip1.SetToolTip(Me.UserTimelineTextCountApi, resources.GetString("UserTimelineTextCountApi.ToolTip"))
        '
        'Label17
        '
        resources.ApplyResources(Me.Label17, "Label17")
        Me.Label17.Name = "Label17"
        Me.ToolTip1.SetToolTip(Me.Label17, resources.GetString("Label17.ToolTip"))
        '
        'Label30
        '
        resources.ApplyResources(Me.Label30, "Label30")
        Me.Label30.Name = "Label30"
        Me.ToolTip1.SetToolTip(Me.Label30, resources.GetString("Label30.ToolTip"))
        '
        'Label28
        '
        resources.ApplyResources(Me.Label28, "Label28")
        Me.Label28.Name = "Label28"
        Me.ToolTip1.SetToolTip(Me.Label28, resources.GetString("Label28.ToolTip"))
        '
        'Label19
        '
        resources.ApplyResources(Me.Label19, "Label19")
        Me.Label19.Name = "Label19"
        Me.ToolTip1.SetToolTip(Me.Label19, resources.GetString("Label19.ToolTip"))
        '
        'FavoritesTextCountApi
        '
        resources.ApplyResources(Me.FavoritesTextCountApi, "FavoritesTextCountApi")
        Me.FavoritesTextCountApi.Name = "FavoritesTextCountApi"
        Me.ToolTip1.SetToolTip(Me.FavoritesTextCountApi, resources.GetString("FavoritesTextCountApi.ToolTip"))
        '
        'SearchTextCountApi
        '
        resources.ApplyResources(Me.SearchTextCountApi, "SearchTextCountApi")
        Me.SearchTextCountApi.Name = "SearchTextCountApi"
        Me.ToolTip1.SetToolTip(Me.SearchTextCountApi, resources.GetString("SearchTextCountApi.ToolTip"))
        '
        'Label66
        '
        resources.ApplyResources(Me.Label66, "Label66")
        Me.Label66.Name = "Label66"
        Me.ToolTip1.SetToolTip(Me.Label66, resources.GetString("Label66.ToolTip"))
        '
        'FirstTextCountApi
        '
        resources.ApplyResources(Me.FirstTextCountApi, "FirstTextCountApi")
        Me.FirstTextCountApi.Name = "FirstTextCountApi"
        Me.ToolTip1.SetToolTip(Me.FirstTextCountApi, resources.GetString("FirstTextCountApi.ToolTip"))
        '
        'GetMoreTextCountApi
        '
        resources.ApplyResources(Me.GetMoreTextCountApi, "GetMoreTextCountApi")
        Me.GetMoreTextCountApi.Name = "GetMoreTextCountApi"
        Me.ToolTip1.SetToolTip(Me.GetMoreTextCountApi, resources.GetString("GetMoreTextCountApi.ToolTip"))
        '
        'Label53
        '
        resources.ApplyResources(Me.Label53, "Label53")
        Me.Label53.Name = "Label53"
        Me.ToolTip1.SetToolTip(Me.Label53, resources.GetString("Label53.ToolTip"))
        '
        'UseChangeGetCount
        '
        resources.ApplyResources(Me.UseChangeGetCount, "UseChangeGetCount")
        Me.UseChangeGetCount.Name = "UseChangeGetCount"
        Me.ToolTip1.SetToolTip(Me.UseChangeGetCount, resources.GetString("UseChangeGetCount.ToolTip"))
        Me.UseChangeGetCount.UseVisualStyleBackColor = True
        '
        'TextCountApiReply
        '
        resources.ApplyResources(Me.TextCountApiReply, "TextCountApiReply")
        Me.TextCountApiReply.Name = "TextCountApiReply"
        Me.ToolTip1.SetToolTip(Me.TextCountApiReply, resources.GetString("TextCountApiReply.ToolTip"))
        '
        'Label67
        '
        resources.ApplyResources(Me.Label67, "Label67")
        Me.Label67.Name = "Label67"
        Me.ToolTip1.SetToolTip(Me.Label67, resources.GetString("Label67.ToolTip"))
        '
        'TextCountApi
        '
        resources.ApplyResources(Me.TextCountApi, "TextCountApi")
        Me.TextCountApi.Name = "TextCountApi"
        Me.ToolTip1.SetToolTip(Me.TextCountApi, resources.GetString("TextCountApi.ToolTip"))
        '
        'Cancel
        '
        resources.ApplyResources(Me.Cancel, "Cancel")
        Me.Cancel.CausesValidation = False
        Me.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel.Name = "Cancel"
        Me.ToolTip1.SetToolTip(Me.Cancel, resources.GetString("Cancel.ToolTip"))
        Me.Cancel.UseVisualStyleBackColor = True
        '
        'Save
        '
        resources.ApplyResources(Me.Save, "Save")
        Me.Save.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Save.Name = "Save"
        Me.ToolTip1.SetToolTip(Me.Save, resources.GetString("Save.ToolTip"))
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
        Me.ToolTip1.SetToolTip(Me, resources.GetString("$this.ToolTip"))
        Me.TopMost = True
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ActionPanel.ResumeLayout(False)
        Me.ActionPanel.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.CooperatePanel.ResumeLayout(False)
        Me.CooperatePanel.PerformLayout()
        Me.NotifyPanel.ResumeLayout(False)
        Me.NotifyPanel.PerformLayout()
        Me.TweetPrvPanel.ResumeLayout(False)
        Me.TweetPrvPanel.PerformLayout()
        Me.GetPeriodPanel.ResumeLayout(False)
        Me.GetPeriodPanel.PerformLayout()
        Me.PreviewPanel.ResumeLayout(False)
        Me.PreviewPanel.PerformLayout()
        Me.BasedPanel.ResumeLayout(False)
        Me.BasedPanel.PerformLayout()
        Me.FontPanel.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ShortUrlPanel.ResumeLayout(False)
        Me.ShortUrlPanel.PerformLayout()
        Me.ProxyPanel.ResumeLayout(False)
        Me.ProxyPanel.PerformLayout()
        Me.ConnectionPanel.ResumeLayout(False)
        Me.ConnectionPanel.PerformLayout()
        Me.FontPanel2.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.TweetActPanel.ResumeLayout(False)
        Me.TweetActPanel.PerformLayout()
        Me.StartupPanel.ResumeLayout(False)
        Me.StartupPanel.PerformLayout()
        Me.GetCountPanel.ResumeLayout(False)
        Me.GetCountPanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
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
    Private WithEvents TreeViewSetting As System.Windows.Forms.TreeView
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents ListDoubleClickActionComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents Label39 As System.Windows.Forms.Label
    Friend WithEvents UserAppointUrlText As System.Windows.Forms.TextBox
    Friend WithEvents HideDuplicatedRetweetsCheck As System.Windows.Forms.CheckBox
End Class
