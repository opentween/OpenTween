Option Strict On
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Setting
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

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Setting))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Username = New System.Windows.Forms.TextBox()
        Me.Password = New System.Windows.Forms.TextBox()
        Me.Save = New System.Windows.Forms.Button()
        Me.Cancel = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TimelinePeriod = New System.Windows.Forms.TextBox()
        Me.DMPeriod = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.StartupReaded = New System.Windows.Forms.CheckBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.StatusText = New System.Windows.Forms.TextBox()
        Me.PlaySnd = New System.Windows.Forms.CheckBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.OneWayLv = New System.Windows.Forms.CheckBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnRetweet = New System.Windows.Forms.Button()
        Me.lblRetweet = New System.Windows.Forms.Label()
        Me.Label80 = New System.Windows.Forms.Label()
        Me.ButtonBackToDefaultFontColor = New System.Windows.Forms.Button()
        Me.btnDetailLink = New System.Windows.Forms.Button()
        Me.lblDetailLink = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.btnInputFont = New System.Windows.Forms.Button()
        Me.lblInputFont = New System.Windows.Forms.Label()
        Me.Label65 = New System.Windows.Forms.Label()
        Me.btnInputBackcolor = New System.Windows.Forms.Button()
        Me.lblInputBackcolor = New System.Windows.Forms.Label()
        Me.Label52 = New System.Windows.Forms.Label()
        Me.btnUnread = New System.Windows.Forms.Button()
        Me.lblUnread = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.btnAtTo = New System.Windows.Forms.Button()
        Me.lblAtTo = New System.Windows.Forms.Label()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.btnDetailBack = New System.Windows.Forms.Button()
        Me.lblDetailBackcolor = New System.Windows.Forms.Label()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.btnListBack = New System.Windows.Forms.Button()
        Me.lblListBackcolor = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.btnAtFromTarget = New System.Windows.Forms.Button()
        Me.lblAtFromTarget = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.btnAtTarget = New System.Windows.Forms.Button()
        Me.lblAtTarget = New System.Windows.Forms.Label()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.btnTarget = New System.Windows.Forms.Button()
        Me.lblTarget = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.btnAtSelf = New System.Windows.Forms.Button()
        Me.lblAtSelf = New System.Windows.Forms.Label()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.btnSelf = New System.Windows.Forms.Button()
        Me.lblSelf = New System.Windows.Forms.Label()
        Me.Label36 = New System.Windows.Forms.Label()
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
        Me.FontDialog1 = New System.Windows.Forms.FontDialog()
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
        Me.cmbNameBalloon = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.CheckUseRecommendStatus = New System.Windows.Forms.CheckBox()
        Me.CmbDateTimeFormat = New System.Windows.Forms.ComboBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.CheckBox3 = New System.Windows.Forms.CheckBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.CheckPostCtrlEnter = New System.Windows.Forms.CheckBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.IconSize = New System.Windows.Forms.ComboBox()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.UReadMng = New System.Windows.Forms.CheckBox()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.CheckReadOldPosts = New System.Windows.Forms.CheckBox()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.CheckCloseToExit = New System.Windows.Forms.CheckBox()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.CheckMinimizeToTray = New System.Windows.Forms.CheckBox()
        Me.BrowserPathText = New System.Windows.Forms.TextBox()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.CheckDispUsername = New System.Windows.Forms.CheckBox()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.ComboDispTitle = New System.Windows.Forms.ComboBox()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.ButtonApiCalc = New System.Windows.Forms.Button()
        Me.LabelPostAndGet = New System.Windows.Forms.Label()
        Me.LabelApiUsing = New System.Windows.Forms.Label()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.ListsPeriod = New System.Windows.Forms.TextBox()
        Me.AuthBasicRadio = New System.Windows.Forms.RadioButton()
        Me.AuthOAuthRadio = New System.Windows.Forms.RadioButton()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.AuthClearButton = New System.Windows.Forms.Button()
        Me.AuthUserLabel = New System.Windows.Forms.Label()
        Me.AuthStateLabel = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.AuthorizeButton = New System.Windows.Forms.Button()
        Me.TextCountApiReply = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.PubSearchPeriod = New System.Windows.Forms.TextBox()
        Me.Label69 = New System.Windows.Forms.Label()
        Me.ReplyPeriod = New System.Windows.Forms.TextBox()
        Me.CheckPostAndGet = New System.Windows.Forms.CheckBox()
        Me.Label67 = New System.Windows.Forms.Label()
        Me.TextCountApi = New System.Windows.Forms.TextBox()
        Me.Label54 = New System.Windows.Forms.Label()
        Me.CheckStartupFollowers = New System.Windows.Forms.CheckBox()
        Me.Label51 = New System.Windows.Forms.Label()
        Me.CheckStartupVersion = New System.Windows.Forms.CheckBox()
        Me.CheckPeriodAdjust = New System.Windows.Forms.CheckBox()
        Me.Label74 = New System.Windows.Forms.Label()
        Me.chkGetFav = New System.Windows.Forms.CheckBox()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.HotkeyCheck = New System.Windows.Forms.CheckBox()
        Me.HotkeyCode = New System.Windows.Forms.Label()
        Me.HotkeyText = New System.Windows.Forms.TextBox()
        Me.HotkeyWin = New System.Windows.Forms.CheckBox()
        Me.HotkeyAlt = New System.Windows.Forms.CheckBox()
        Me.HotkeyShift = New System.Windows.Forms.CheckBox()
        Me.HotkeyCtrl = New System.Windows.Forms.CheckBox()
        Me.Label82 = New System.Windows.Forms.Label()
        Me.CheckHashSupple = New System.Windows.Forms.CheckBox()
        Me.Label79 = New System.Windows.Forms.Label()
        Me.CheckAtIdSupple = New System.Windows.Forms.CheckBox()
        Me.TextBitlyPw = New System.Windows.Forms.TextBox()
        Me.Label77 = New System.Windows.Forms.Label()
        Me.TextBitlyId = New System.Windows.Forms.TextBox()
        Me.Label76 = New System.Windows.Forms.Label()
        Me.ComboBoxAutoShortUrlFirst = New System.Windows.Forms.ComboBox()
        Me.Label71 = New System.Windows.Forms.Label()
        Me.CheckProtectNotInclude = New System.Windows.Forms.CheckBox()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.CheckAutoConvertUrl = New System.Windows.Forms.CheckBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.Label57 = New System.Windows.Forms.Label()
        Me.Label56 = New System.Windows.Forms.Label()
        Me.CheckFavRestrict = New System.Windows.Forms.CheckBox()
        Me.CheckTinyURL = New System.Windows.Forms.CheckBox()
        Me.Label50 = New System.Windows.Forms.Label()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.CheckPreviewEnable = New System.Windows.Forms.CheckBox()
        Me.Label81 = New System.Windows.Forms.Label()
        Me.LanguageCombo = New System.Windows.Forms.ComboBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.CheckAlwaysTop = New System.Windows.Forms.CheckBox()
        Me.Label58 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.CheckSortOrderLock = New System.Windows.Forms.CheckBox()
        Me.Label78 = New System.Windows.Forms.Label()
        Me.CheckShowGrid = New System.Windows.Forms.CheckBox()
        Me.Label75 = New System.Windows.Forms.Label()
        Me.CheckMonospace = New System.Windows.Forms.CheckBox()
        Me.Label73 = New System.Windows.Forms.Label()
        Me.chkReadOwnPost = New System.Windows.Forms.CheckBox()
        Me.ReplyIconStateCombo = New System.Windows.Forms.ComboBox()
        Me.Label72 = New System.Windows.Forms.Label()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.ChkNewMentionsBlink = New System.Windows.Forms.CheckBox()
        Me.chkTabIconDisp = New System.Windows.Forms.CheckBox()
        Me.Label68 = New System.Windows.Forms.Label()
        Me.CheckBalloonLimit = New System.Windows.Forms.CheckBox()
        Me.LabelDateTimeFormatApplied = New System.Windows.Forms.Label()
        Me.Label62 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.chkUnreadStyle = New System.Windows.Forms.CheckBox()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.CheckEnableBasicAuth = New System.Windows.Forms.CheckBox()
        Me.TwitterSearchAPIText = New System.Windows.Forms.TextBox()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.TwitterAPIText = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.CheckUseSsl = New System.Windows.Forms.CheckBox()
        Me.Label64 = New System.Windows.Forms.Label()
        Me.ConnectionTimeOut = New System.Windows.Forms.TextBox()
        Me.Label63 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label55 = New System.Windows.Forms.Label()
        Me.TextProxyPassword = New System.Windows.Forms.TextBox()
        Me.LabelProxyPassword = New System.Windows.Forms.Label()
        Me.TextProxyUser = New System.Windows.Forms.TextBox()
        Me.LabelProxyUser = New System.Windows.Forms.Label()
        Me.TextProxyPort = New System.Windows.Forms.TextBox()
        Me.LabelProxyPort = New System.Windows.Forms.Label()
        Me.TextProxyAddress = New System.Windows.Forms.TextBox()
        Me.LabelProxyAddress = New System.Windows.Forms.Label()
        Me.RadioProxySpecified = New System.Windows.Forms.RadioButton()
        Me.RadioProxyIE = New System.Windows.Forms.RadioButton()
        Me.RadioProxyNone = New System.Windows.Forms.RadioButton()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.SearchTextCountApi = New System.Windows.Forms.TextBox()
        Me.Label66 = New System.Windows.Forms.Label()
        Me.FirstTextCountApi = New System.Windows.Forms.TextBox()
        Me.GetMoreTextCountApi = New System.Windows.Forms.TextBox()
        Me.Label53 = New System.Windows.Forms.Label()
        Me.UseChangeGetCount = New System.Windows.Forms.CheckBox()
        Me.CheckNicoms = New System.Windows.Forms.CheckBox()
        Me.Label60 = New System.Windows.Forms.Label()
        Me.ComboBoxOutputzUrlmode = New System.Windows.Forms.ComboBox()
        Me.Label59 = New System.Windows.Forms.Label()
        Me.TextBoxOutputzKey = New System.Windows.Forms.TextBox()
        Me.CheckOutputz = New System.Windows.Forms.CheckBox()
        Me.GroupBox1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        Me.SuspendLayout()
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
        'Save
        '
        Me.Save.DialogResult = System.Windows.Forms.DialogResult.OK
        resources.ApplyResources(Me.Save, "Save")
        Me.Save.Name = "Save"
        Me.Save.UseVisualStyleBackColor = True
        '
        'Cancel
        '
        Me.Cancel.CausesValidation = False
        Me.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.Cancel, "Cancel")
        Me.Cancel.Name = "Cancel"
        Me.Cancel.UseVisualStyleBackColor = True
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'TimelinePeriod
        '
        resources.ApplyResources(Me.TimelinePeriod, "TimelinePeriod")
        Me.TimelinePeriod.Name = "TimelinePeriod"
        '
        'DMPeriod
        '
        resources.ApplyResources(Me.DMPeriod, "DMPeriod")
        Me.DMPeriod.Name = "DMPeriod"
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        '
        'StartupReaded
        '
        resources.ApplyResources(Me.StartupReaded, "StartupReaded")
        Me.StartupReaded.Name = "StartupReaded"
        Me.StartupReaded.UseVisualStyleBackColor = True
        '
        'Label11
        '
        resources.ApplyResources(Me.Label11, "Label11")
        Me.Label11.Name = "Label11"
        '
        'Label12
        '
        resources.ApplyResources(Me.Label12, "Label12")
        Me.Label12.Name = "Label12"
        '
        'StatusText
        '
        resources.ApplyResources(Me.StatusText, "StatusText")
        Me.StatusText.Name = "StatusText"
        '
        'PlaySnd
        '
        resources.ApplyResources(Me.PlaySnd, "PlaySnd")
        Me.PlaySnd.Name = "PlaySnd"
        Me.PlaySnd.UseVisualStyleBackColor = True
        '
        'Label14
        '
        resources.ApplyResources(Me.Label14, "Label14")
        Me.Label14.Name = "Label14"
        '
        'Label15
        '
        Me.Label15.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label15.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        resources.ApplyResources(Me.Label15, "Label15")
        Me.Label15.Name = "Label15"
        '
        'OneWayLv
        '
        resources.ApplyResources(Me.OneWayLv, "OneWayLv")
        Me.OneWayLv.Name = "OneWayLv"
        Me.OneWayLv.UseVisualStyleBackColor = True
        '
        'Label16
        '
        resources.ApplyResources(Me.Label16, "Label16")
        Me.Label16.Name = "Label16"
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
        Me.GroupBox1.Controls.Add(Me.btnInputFont)
        Me.GroupBox1.Controls.Add(Me.lblInputFont)
        Me.GroupBox1.Controls.Add(Me.Label65)
        Me.GroupBox1.Controls.Add(Me.btnInputBackcolor)
        Me.GroupBox1.Controls.Add(Me.lblInputBackcolor)
        Me.GroupBox1.Controls.Add(Me.Label52)
        Me.GroupBox1.Controls.Add(Me.btnUnread)
        Me.GroupBox1.Controls.Add(Me.lblUnread)
        Me.GroupBox1.Controls.Add(Me.Label20)
        Me.GroupBox1.Controls.Add(Me.btnAtTo)
        Me.GroupBox1.Controls.Add(Me.lblAtTo)
        Me.GroupBox1.Controls.Add(Me.Label49)
        Me.GroupBox1.Controls.Add(Me.btnDetailBack)
        Me.GroupBox1.Controls.Add(Me.lblDetailBackcolor)
        Me.GroupBox1.Controls.Add(Me.Label37)
        Me.GroupBox1.Controls.Add(Me.btnListBack)
        Me.GroupBox1.Controls.Add(Me.lblListBackcolor)
        Me.GroupBox1.Controls.Add(Me.Label19)
        Me.GroupBox1.Controls.Add(Me.btnAtFromTarget)
        Me.GroupBox1.Controls.Add(Me.lblAtFromTarget)
        Me.GroupBox1.Controls.Add(Me.Label28)
        Me.GroupBox1.Controls.Add(Me.btnAtTarget)
        Me.GroupBox1.Controls.Add(Me.lblAtTarget)
        Me.GroupBox1.Controls.Add(Me.Label30)
        Me.GroupBox1.Controls.Add(Me.btnTarget)
        Me.GroupBox1.Controls.Add(Me.lblTarget)
        Me.GroupBox1.Controls.Add(Me.Label32)
        Me.GroupBox1.Controls.Add(Me.btnAtSelf)
        Me.GroupBox1.Controls.Add(Me.lblAtSelf)
        Me.GroupBox1.Controls.Add(Me.Label34)
        Me.GroupBox1.Controls.Add(Me.btnSelf)
        Me.GroupBox1.Controls.Add(Me.lblSelf)
        Me.GroupBox1.Controls.Add(Me.Label36)
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
        'btnInputFont
        '
        resources.ApplyResources(Me.btnInputFont, "btnInputFont")
        Me.btnInputFont.Name = "btnInputFont"
        Me.btnInputFont.UseVisualStyleBackColor = True
        '
        'lblInputFont
        '
        Me.lblInputFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblInputFont, "lblInputFont")
        Me.lblInputFont.Name = "lblInputFont"
        '
        'Label65
        '
        resources.ApplyResources(Me.Label65, "Label65")
        Me.Label65.Name = "Label65"
        '
        'btnInputBackcolor
        '
        resources.ApplyResources(Me.btnInputBackcolor, "btnInputBackcolor")
        Me.btnInputBackcolor.Name = "btnInputBackcolor"
        Me.btnInputBackcolor.UseVisualStyleBackColor = True
        '
        'lblInputBackcolor
        '
        Me.lblInputBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblInputBackcolor, "lblInputBackcolor")
        Me.lblInputBackcolor.Name = "lblInputBackcolor"
        '
        'Label52
        '
        resources.ApplyResources(Me.Label52, "Label52")
        Me.Label52.Name = "Label52"
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
        'btnAtTo
        '
        resources.ApplyResources(Me.btnAtTo, "btnAtTo")
        Me.btnAtTo.Name = "btnAtTo"
        Me.btnAtTo.UseVisualStyleBackColor = True
        '
        'lblAtTo
        '
        Me.lblAtTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAtTo, "lblAtTo")
        Me.lblAtTo.Name = "lblAtTo"
        '
        'Label49
        '
        resources.ApplyResources(Me.Label49, "Label49")
        Me.Label49.Name = "Label49"
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
        'btnListBack
        '
        resources.ApplyResources(Me.btnListBack, "btnListBack")
        Me.btnListBack.Name = "btnListBack"
        Me.btnListBack.UseVisualStyleBackColor = True
        '
        'lblListBackcolor
        '
        Me.lblListBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblListBackcolor, "lblListBackcolor")
        Me.lblListBackcolor.Name = "lblListBackcolor"
        '
        'Label19
        '
        resources.ApplyResources(Me.Label19, "Label19")
        Me.Label19.Name = "Label19"
        '
        'btnAtFromTarget
        '
        resources.ApplyResources(Me.btnAtFromTarget, "btnAtFromTarget")
        Me.btnAtFromTarget.Name = "btnAtFromTarget"
        Me.btnAtFromTarget.UseVisualStyleBackColor = True
        '
        'lblAtFromTarget
        '
        Me.lblAtFromTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAtFromTarget, "lblAtFromTarget")
        Me.lblAtFromTarget.Name = "lblAtFromTarget"
        '
        'Label28
        '
        resources.ApplyResources(Me.Label28, "Label28")
        Me.Label28.Name = "Label28"
        '
        'btnAtTarget
        '
        resources.ApplyResources(Me.btnAtTarget, "btnAtTarget")
        Me.btnAtTarget.Name = "btnAtTarget"
        Me.btnAtTarget.UseVisualStyleBackColor = True
        '
        'lblAtTarget
        '
        Me.lblAtTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAtTarget, "lblAtTarget")
        Me.lblAtTarget.Name = "lblAtTarget"
        '
        'Label30
        '
        resources.ApplyResources(Me.Label30, "Label30")
        Me.Label30.Name = "Label30"
        '
        'btnTarget
        '
        resources.ApplyResources(Me.btnTarget, "btnTarget")
        Me.btnTarget.Name = "btnTarget"
        Me.btnTarget.UseVisualStyleBackColor = True
        '
        'lblTarget
        '
        Me.lblTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblTarget, "lblTarget")
        Me.lblTarget.Name = "lblTarget"
        '
        'Label32
        '
        resources.ApplyResources(Me.Label32, "Label32")
        Me.Label32.Name = "Label32"
        '
        'btnAtSelf
        '
        resources.ApplyResources(Me.btnAtSelf, "btnAtSelf")
        Me.btnAtSelf.Name = "btnAtSelf"
        Me.btnAtSelf.UseVisualStyleBackColor = True
        '
        'lblAtSelf
        '
        Me.lblAtSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAtSelf, "lblAtSelf")
        Me.lblAtSelf.Name = "lblAtSelf"
        '
        'Label34
        '
        resources.ApplyResources(Me.Label34, "Label34")
        Me.Label34.Name = "Label34"
        '
        'btnSelf
        '
        resources.ApplyResources(Me.btnSelf, "btnSelf")
        Me.btnSelf.Name = "btnSelf"
        Me.btnSelf.UseVisualStyleBackColor = True
        '
        'lblSelf
        '
        Me.lblSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblSelf, "lblSelf")
        Me.lblSelf.Name = "lblSelf"
        '
        'Label36
        '
        resources.ApplyResources(Me.Label36, "Label36")
        Me.Label36.Name = "Label36"
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
        'cmbNameBalloon
        '
        Me.cmbNameBalloon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNameBalloon.FormattingEnabled = True
        Me.cmbNameBalloon.Items.AddRange(New Object() {resources.GetString("cmbNameBalloon.Items"), resources.GetString("cmbNameBalloon.Items1"), resources.GetString("cmbNameBalloon.Items2")})
        resources.ApplyResources(Me.cmbNameBalloon, "cmbNameBalloon")
        Me.cmbNameBalloon.Name = "cmbNameBalloon"
        '
        'Label10
        '
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Name = "Label10"
        '
        'CheckUseRecommendStatus
        '
        resources.ApplyResources(Me.CheckUseRecommendStatus, "CheckUseRecommendStatus")
        Me.CheckUseRecommendStatus.Name = "CheckUseRecommendStatus"
        Me.CheckUseRecommendStatus.UseVisualStyleBackColor = True
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
        'CheckBox3
        '
        resources.ApplyResources(Me.CheckBox3, "CheckBox3")
        Me.CheckBox3.Name = "CheckBox3"
        Me.CheckBox3.UseVisualStyleBackColor = True
        '
        'Label25
        '
        resources.ApplyResources(Me.Label25, "Label25")
        Me.Label25.Name = "Label25"
        '
        'CheckPostCtrlEnter
        '
        resources.ApplyResources(Me.CheckPostCtrlEnter, "CheckPostCtrlEnter")
        Me.CheckPostCtrlEnter.Name = "CheckPostCtrlEnter"
        Me.CheckPostCtrlEnter.UseVisualStyleBackColor = True
        '
        'Label27
        '
        resources.ApplyResources(Me.Label27, "Label27")
        Me.Label27.Name = "Label27"
        '
        'TextBox3
        '
        resources.ApplyResources(Me.TextBox3, "TextBox3")
        Me.TextBox3.Name = "TextBox3"
        '
        'IconSize
        '
        Me.IconSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.IconSize.FormattingEnabled = True
        Me.IconSize.Items.AddRange(New Object() {resources.GetString("IconSize.Items"), resources.GetString("IconSize.Items1"), resources.GetString("IconSize.Items2"), resources.GetString("IconSize.Items3"), resources.GetString("IconSize.Items4")})
        resources.ApplyResources(Me.IconSize, "IconSize")
        Me.IconSize.Name = "IconSize"
        '
        'Label38
        '
        resources.ApplyResources(Me.Label38, "Label38")
        Me.Label38.Name = "Label38"
        '
        'UReadMng
        '
        resources.ApplyResources(Me.UReadMng, "UReadMng")
        Me.UReadMng.Name = "UReadMng"
        Me.UReadMng.UseVisualStyleBackColor = True
        '
        'Label39
        '
        resources.ApplyResources(Me.Label39, "Label39")
        Me.Label39.Name = "Label39"
        '
        'CheckReadOldPosts
        '
        resources.ApplyResources(Me.CheckReadOldPosts, "CheckReadOldPosts")
        Me.CheckReadOldPosts.Name = "CheckReadOldPosts"
        Me.CheckReadOldPosts.UseVisualStyleBackColor = True
        '
        'Label40
        '
        resources.ApplyResources(Me.Label40, "Label40")
        Me.Label40.Name = "Label40"
        '
        'CheckCloseToExit
        '
        resources.ApplyResources(Me.CheckCloseToExit, "CheckCloseToExit")
        Me.CheckCloseToExit.Name = "CheckCloseToExit"
        Me.CheckCloseToExit.UseVisualStyleBackColor = True
        '
        'Label41
        '
        resources.ApplyResources(Me.Label41, "Label41")
        Me.Label41.Name = "Label41"
        '
        'CheckMinimizeToTray
        '
        resources.ApplyResources(Me.CheckMinimizeToTray, "CheckMinimizeToTray")
        Me.CheckMinimizeToTray.Name = "CheckMinimizeToTray"
        Me.CheckMinimizeToTray.UseVisualStyleBackColor = True
        '
        'BrowserPathText
        '
        resources.ApplyResources(Me.BrowserPathText, "BrowserPathText")
        Me.BrowserPathText.Name = "BrowserPathText"
        '
        'Label44
        '
        resources.ApplyResources(Me.Label44, "Label44")
        Me.Label44.Name = "Label44"
        '
        'CheckDispUsername
        '
        resources.ApplyResources(Me.CheckDispUsername, "CheckDispUsername")
        Me.CheckDispUsername.Name = "CheckDispUsername"
        Me.CheckDispUsername.UseVisualStyleBackColor = True
        '
        'Label46
        '
        resources.ApplyResources(Me.Label46, "Label46")
        Me.Label46.Name = "Label46"
        '
        'Label45
        '
        resources.ApplyResources(Me.Label45, "Label45")
        Me.Label45.Name = "Label45"
        '
        'ComboDispTitle
        '
        Me.ComboDispTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboDispTitle.FormattingEnabled = True
        Me.ComboDispTitle.Items.AddRange(New Object() {resources.GetString("ComboDispTitle.Items"), resources.GetString("ComboDispTitle.Items1"), resources.GetString("ComboDispTitle.Items2"), resources.GetString("ComboDispTitle.Items3"), resources.GetString("ComboDispTitle.Items4"), resources.GetString("ComboDispTitle.Items5"), resources.GetString("ComboDispTitle.Items6"), resources.GetString("ComboDispTitle.Items7")})
        resources.ApplyResources(Me.ComboDispTitle, "ComboDispTitle")
        Me.ComboDispTitle.Name = "ComboDispTitle"
        '
        'Label47
        '
        resources.ApplyResources(Me.Label47, "Label47")
        Me.Label47.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label47.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label47.Name = "Label47"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Controls.Add(Me.TabPage6)
        resources.ApplyResources(Me.TabControl1, "TabControl1")
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.ButtonApiCalc)
        Me.TabPage1.Controls.Add(Me.LabelPostAndGet)
        Me.TabPage1.Controls.Add(Me.LabelApiUsing)
        Me.TabPage1.Controls.Add(Me.Label33)
        Me.TabPage1.Controls.Add(Me.ListsPeriod)
        Me.TabPage1.Controls.Add(Me.AuthBasicRadio)
        Me.TabPage1.Controls.Add(Me.AuthOAuthRadio)
        Me.TabPage1.Controls.Add(Me.Label6)
        Me.TabPage1.Controls.Add(Me.AuthClearButton)
        Me.TabPage1.Controls.Add(Me.AuthUserLabel)
        Me.TabPage1.Controls.Add(Me.AuthStateLabel)
        Me.TabPage1.Controls.Add(Me.Label4)
        Me.TabPage1.Controls.Add(Me.AuthorizeButton)
        Me.TabPage1.Controls.Add(Me.TextCountApiReply)
        Me.TabPage1.Controls.Add(Me.Label7)
        Me.TabPage1.Controls.Add(Me.PubSearchPeriod)
        Me.TabPage1.Controls.Add(Me.Label69)
        Me.TabPage1.Controls.Add(Me.ReplyPeriod)
        Me.TabPage1.Controls.Add(Me.CheckPostAndGet)
        Me.TabPage1.Controls.Add(Me.Label67)
        Me.TabPage1.Controls.Add(Me.TextCountApi)
        Me.TabPage1.Controls.Add(Me.Label54)
        Me.TabPage1.Controls.Add(Me.CheckStartupFollowers)
        Me.TabPage1.Controls.Add(Me.Label51)
        Me.TabPage1.Controls.Add(Me.CheckStartupVersion)
        Me.TabPage1.Controls.Add(Me.CheckPeriodAdjust)
        Me.TabPage1.Controls.Add(Me.Label1)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.Username)
        Me.TabPage1.Controls.Add(Me.Password)
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.TimelinePeriod)
        Me.TabPage1.Controls.Add(Me.Label5)
        Me.TabPage1.Controls.Add(Me.DMPeriod)
        Me.TabPage1.Controls.Add(Me.Label9)
        Me.TabPage1.Controls.Add(Me.StartupReaded)
        Me.TabPage1.Controls.Add(Me.Label74)
        Me.TabPage1.Controls.Add(Me.chkGetFav)
        resources.ApplyResources(Me.TabPage1, "TabPage1")
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.UseVisualStyleBackColor = True
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
        'TextCountApiReply
        '
        resources.ApplyResources(Me.TextCountApiReply, "TextCountApiReply")
        Me.TextCountApiReply.Name = "TextCountApiReply"
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
        'Label54
        '
        resources.ApplyResources(Me.Label54, "Label54")
        Me.Label54.Name = "Label54"
        '
        'CheckStartupFollowers
        '
        resources.ApplyResources(Me.CheckStartupFollowers, "CheckStartupFollowers")
        Me.CheckStartupFollowers.Name = "CheckStartupFollowers"
        Me.CheckStartupFollowers.UseVisualStyleBackColor = True
        '
        'Label51
        '
        resources.ApplyResources(Me.Label51, "Label51")
        Me.Label51.Name = "Label51"
        '
        'CheckStartupVersion
        '
        resources.ApplyResources(Me.CheckStartupVersion, "CheckStartupVersion")
        Me.CheckStartupVersion.Name = "CheckStartupVersion"
        Me.CheckStartupVersion.UseVisualStyleBackColor = True
        '
        'CheckPeriodAdjust
        '
        resources.ApplyResources(Me.CheckPeriodAdjust, "CheckPeriodAdjust")
        Me.CheckPeriodAdjust.Name = "CheckPeriodAdjust"
        Me.CheckPeriodAdjust.UseVisualStyleBackColor = True
        '
        'Label74
        '
        resources.ApplyResources(Me.Label74, "Label74")
        Me.Label74.Name = "Label74"
        '
        'chkGetFav
        '
        resources.ApplyResources(Me.chkGetFav, "chkGetFav")
        Me.chkGetFav.Name = "chkGetFav"
        Me.chkGetFav.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.GroupBox3)
        Me.TabPage2.Controls.Add(Me.Label82)
        Me.TabPage2.Controls.Add(Me.CheckHashSupple)
        Me.TabPage2.Controls.Add(Me.Label79)
        Me.TabPage2.Controls.Add(Me.CheckAtIdSupple)
        Me.TabPage2.Controls.Add(Me.TextBitlyPw)
        Me.TabPage2.Controls.Add(Me.Label77)
        Me.TabPage2.Controls.Add(Me.TextBitlyId)
        Me.TabPage2.Controls.Add(Me.Label76)
        Me.TabPage2.Controls.Add(Me.ComboBoxAutoShortUrlFirst)
        Me.TabPage2.Controls.Add(Me.Label71)
        Me.TabPage2.Controls.Add(Me.CheckProtectNotInclude)
        Me.TabPage2.Controls.Add(Me.Label42)
        Me.TabPage2.Controls.Add(Me.CheckAutoConvertUrl)
        Me.TabPage2.Controls.Add(Me.Label29)
        Me.TabPage2.Controls.Add(Me.Label57)
        Me.TabPage2.Controls.Add(Me.Label56)
        Me.TabPage2.Controls.Add(Me.CheckFavRestrict)
        Me.TabPage2.Controls.Add(Me.CheckTinyURL)
        Me.TabPage2.Controls.Add(Me.Label50)
        Me.TabPage2.Controls.Add(Me.Button3)
        Me.TabPage2.Controls.Add(Me.PlaySnd)
        Me.TabPage2.Controls.Add(Me.Label14)
        Me.TabPage2.Controls.Add(Me.Label15)
        Me.TabPage2.Controls.Add(Me.Label38)
        Me.TabPage2.Controls.Add(Me.BrowserPathText)
        Me.TabPage2.Controls.Add(Me.UReadMng)
        Me.TabPage2.Controls.Add(Me.Label44)
        Me.TabPage2.Controls.Add(Me.CheckCloseToExit)
        Me.TabPage2.Controls.Add(Me.Label40)
        Me.TabPage2.Controls.Add(Me.CheckMinimizeToTray)
        Me.TabPage2.Controls.Add(Me.Label41)
        Me.TabPage2.Controls.Add(Me.Label27)
        Me.TabPage2.Controls.Add(Me.Label39)
        Me.TabPage2.Controls.Add(Me.CheckPostCtrlEnter)
        Me.TabPage2.Controls.Add(Me.CheckReadOldPosts)
        Me.TabPage2.Controls.Add(Me.Label12)
        Me.TabPage2.Controls.Add(Me.StatusText)
        Me.TabPage2.Controls.Add(Me.CheckUseRecommendStatus)
        resources.ApplyResources(Me.TabPage2, "TabPage2")
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.UseVisualStyleBackColor = True
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
        'Label82
        '
        resources.ApplyResources(Me.Label82, "Label82")
        Me.Label82.Name = "Label82"
        '
        'CheckHashSupple
        '
        resources.ApplyResources(Me.CheckHashSupple, "CheckHashSupple")
        Me.CheckHashSupple.Name = "CheckHashSupple"
        Me.CheckHashSupple.UseVisualStyleBackColor = True
        '
        'Label79
        '
        resources.ApplyResources(Me.Label79, "Label79")
        Me.Label79.Name = "Label79"
        '
        'CheckAtIdSupple
        '
        resources.ApplyResources(Me.CheckAtIdSupple, "CheckAtIdSupple")
        Me.CheckAtIdSupple.Name = "CheckAtIdSupple"
        Me.CheckAtIdSupple.UseVisualStyleBackColor = True
        '
        'TextBitlyPw
        '
        resources.ApplyResources(Me.TextBitlyPw, "TextBitlyPw")
        Me.TextBitlyPw.Name = "TextBitlyPw"
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
        'Label76
        '
        resources.ApplyResources(Me.Label76, "Label76")
        Me.Label76.Name = "Label76"
        '
        'ComboBoxAutoShortUrlFirst
        '
        Me.ComboBoxAutoShortUrlFirst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxAutoShortUrlFirst.FormattingEnabled = True
        Me.ComboBoxAutoShortUrlFirst.Items.AddRange(New Object() {resources.GetString("ComboBoxAutoShortUrlFirst.Items"), resources.GetString("ComboBoxAutoShortUrlFirst.Items1"), resources.GetString("ComboBoxAutoShortUrlFirst.Items2"), resources.GetString("ComboBoxAutoShortUrlFirst.Items3"), resources.GetString("ComboBoxAutoShortUrlFirst.Items4")})
        resources.ApplyResources(Me.ComboBoxAutoShortUrlFirst, "ComboBoxAutoShortUrlFirst")
        Me.ComboBoxAutoShortUrlFirst.Name = "ComboBoxAutoShortUrlFirst"
        '
        'Label71
        '
        resources.ApplyResources(Me.Label71, "Label71")
        Me.Label71.Name = "Label71"
        '
        'CheckProtectNotInclude
        '
        resources.ApplyResources(Me.CheckProtectNotInclude, "CheckProtectNotInclude")
        Me.CheckProtectNotInclude.Name = "CheckProtectNotInclude"
        Me.CheckProtectNotInclude.UseVisualStyleBackColor = True
        '
        'Label42
        '
        resources.ApplyResources(Me.Label42, "Label42")
        Me.Label42.Name = "Label42"
        '
        'CheckAutoConvertUrl
        '
        resources.ApplyResources(Me.CheckAutoConvertUrl, "CheckAutoConvertUrl")
        Me.CheckAutoConvertUrl.Name = "CheckAutoConvertUrl"
        Me.CheckAutoConvertUrl.UseVisualStyleBackColor = True
        '
        'Label29
        '
        resources.ApplyResources(Me.Label29, "Label29")
        Me.Label29.Name = "Label29"
        '
        'Label57
        '
        resources.ApplyResources(Me.Label57, "Label57")
        Me.Label57.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label57.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label57.Name = "Label57"
        '
        'Label56
        '
        resources.ApplyResources(Me.Label56, "Label56")
        Me.Label56.Name = "Label56"
        '
        'CheckFavRestrict
        '
        resources.ApplyResources(Me.CheckFavRestrict, "CheckFavRestrict")
        Me.CheckFavRestrict.Name = "CheckFavRestrict"
        Me.CheckFavRestrict.UseVisualStyleBackColor = True
        '
        'CheckTinyURL
        '
        resources.ApplyResources(Me.CheckTinyURL, "CheckTinyURL")
        Me.CheckTinyURL.Name = "CheckTinyURL"
        Me.CheckTinyURL.UseVisualStyleBackColor = True
        '
        'Label50
        '
        resources.ApplyResources(Me.Label50, "Label50")
        Me.Label50.Name = "Label50"
        '
        'Button3
        '
        resources.ApplyResources(Me.Button3, "Button3")
        Me.Button3.Name = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.Label35)
        Me.TabPage3.Controls.Add(Me.CheckPreviewEnable)
        Me.TabPage3.Controls.Add(Me.Label81)
        Me.TabPage3.Controls.Add(Me.LanguageCombo)
        Me.TabPage3.Controls.Add(Me.Label13)
        Me.TabPage3.Controls.Add(Me.CheckAlwaysTop)
        Me.TabPage3.Controls.Add(Me.Label58)
        Me.TabPage3.Controls.Add(Me.Label21)
        Me.TabPage3.Controls.Add(Me.CheckSortOrderLock)
        Me.TabPage3.Controls.Add(Me.Label78)
        Me.TabPage3.Controls.Add(Me.CheckShowGrid)
        Me.TabPage3.Controls.Add(Me.Label75)
        Me.TabPage3.Controls.Add(Me.CheckMonospace)
        Me.TabPage3.Controls.Add(Me.Label73)
        Me.TabPage3.Controls.Add(Me.chkReadOwnPost)
        Me.TabPage3.Controls.Add(Me.ReplyIconStateCombo)
        Me.TabPage3.Controls.Add(Me.Label72)
        Me.TabPage3.Controls.Add(Me.Label43)
        Me.TabPage3.Controls.Add(Me.Label48)
        Me.TabPage3.Controls.Add(Me.ChkNewMentionsBlink)
        Me.TabPage3.Controls.Add(Me.chkTabIconDisp)
        Me.TabPage3.Controls.Add(Me.Label68)
        Me.TabPage3.Controls.Add(Me.CheckBalloonLimit)
        Me.TabPage3.Controls.Add(Me.LabelDateTimeFormatApplied)
        Me.TabPage3.Controls.Add(Me.Label62)
        Me.TabPage3.Controls.Add(Me.Label17)
        Me.TabPage3.Controls.Add(Me.chkUnreadStyle)
        Me.TabPage3.Controls.Add(Me.Label10)
        Me.TabPage3.Controls.Add(Me.ComboDispTitle)
        Me.TabPage3.Controls.Add(Me.Label47)
        Me.TabPage3.Controls.Add(Me.CmbDateTimeFormat)
        Me.TabPage3.Controls.Add(Me.Label45)
        Me.TabPage3.Controls.Add(Me.Label23)
        Me.TabPage3.Controls.Add(Me.cmbNameBalloon)
        Me.TabPage3.Controls.Add(Me.Label46)
        Me.TabPage3.Controls.Add(Me.CheckDispUsername)
        Me.TabPage3.Controls.Add(Me.Label11)
        Me.TabPage3.Controls.Add(Me.Label16)
        Me.TabPage3.Controls.Add(Me.OneWayLv)
        Me.TabPage3.Controls.Add(Me.Label25)
        Me.TabPage3.Controls.Add(Me.IconSize)
        Me.TabPage3.Controls.Add(Me.CheckBox3)
        Me.TabPage3.Controls.Add(Me.TextBox3)
        resources.ApplyResources(Me.TabPage3, "TabPage3")
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'Label35
        '
        resources.ApplyResources(Me.Label35, "Label35")
        Me.Label35.Name = "Label35"
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
        'Label58
        '
        resources.ApplyResources(Me.Label58, "Label58")
        Me.Label58.Name = "Label58"
        '
        'Label21
        '
        resources.ApplyResources(Me.Label21, "Label21")
        Me.Label21.Name = "Label21"
        '
        'CheckSortOrderLock
        '
        resources.ApplyResources(Me.CheckSortOrderLock, "CheckSortOrderLock")
        Me.CheckSortOrderLock.Name = "CheckSortOrderLock"
        Me.CheckSortOrderLock.UseVisualStyleBackColor = True
        '
        'Label78
        '
        resources.ApplyResources(Me.Label78, "Label78")
        Me.Label78.Name = "Label78"
        '
        'CheckShowGrid
        '
        resources.ApplyResources(Me.CheckShowGrid, "CheckShowGrid")
        Me.CheckShowGrid.Name = "CheckShowGrid"
        Me.CheckShowGrid.UseVisualStyleBackColor = True
        '
        'Label75
        '
        resources.ApplyResources(Me.Label75, "Label75")
        Me.Label75.Name = "Label75"
        '
        'CheckMonospace
        '
        resources.ApplyResources(Me.CheckMonospace, "CheckMonospace")
        Me.CheckMonospace.Name = "CheckMonospace"
        Me.CheckMonospace.UseVisualStyleBackColor = True
        '
        'Label73
        '
        resources.ApplyResources(Me.Label73, "Label73")
        Me.Label73.Name = "Label73"
        '
        'chkReadOwnPost
        '
        resources.ApplyResources(Me.chkReadOwnPost, "chkReadOwnPost")
        Me.chkReadOwnPost.Name = "chkReadOwnPost"
        Me.chkReadOwnPost.UseVisualStyleBackColor = True
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
        'Label43
        '
        resources.ApplyResources(Me.Label43, "Label43")
        Me.Label43.Name = "Label43"
        '
        'Label48
        '
        resources.ApplyResources(Me.Label48, "Label48")
        Me.Label48.Name = "Label48"
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
        'Label68
        '
        resources.ApplyResources(Me.Label68, "Label68")
        Me.Label68.Name = "Label68"
        '
        'CheckBalloonLimit
        '
        resources.ApplyResources(Me.CheckBalloonLimit, "CheckBalloonLimit")
        Me.CheckBalloonLimit.Name = "CheckBalloonLimit"
        Me.CheckBalloonLimit.UseVisualStyleBackColor = True
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
        'Label17
        '
        resources.ApplyResources(Me.Label17, "Label17")
        Me.Label17.Name = "Label17"
        '
        'chkUnreadStyle
        '
        resources.ApplyResources(Me.chkUnreadStyle, "chkUnreadStyle")
        Me.chkUnreadStyle.Name = "chkUnreadStyle"
        Me.chkUnreadStyle.UseVisualStyleBackColor = True
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.GroupBox1)
        resources.ApplyResources(Me.TabPage4, "TabPage4")
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.CheckEnableBasicAuth)
        Me.TabPage5.Controls.Add(Me.TwitterSearchAPIText)
        Me.TabPage5.Controls.Add(Me.Label31)
        Me.TabPage5.Controls.Add(Me.TwitterAPIText)
        Me.TabPage5.Controls.Add(Me.Label8)
        Me.TabPage5.Controls.Add(Me.CheckUseSsl)
        Me.TabPage5.Controls.Add(Me.Label64)
        Me.TabPage5.Controls.Add(Me.ConnectionTimeOut)
        Me.TabPage5.Controls.Add(Me.Label63)
        Me.TabPage5.Controls.Add(Me.GroupBox2)
        resources.ApplyResources(Me.TabPage5, "TabPage5")
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.UseVisualStyleBackColor = True
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
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label55)
        Me.GroupBox2.Controls.Add(Me.TextProxyPassword)
        Me.GroupBox2.Controls.Add(Me.LabelProxyPassword)
        Me.GroupBox2.Controls.Add(Me.TextProxyUser)
        Me.GroupBox2.Controls.Add(Me.LabelProxyUser)
        Me.GroupBox2.Controls.Add(Me.TextProxyPort)
        Me.GroupBox2.Controls.Add(Me.LabelProxyPort)
        Me.GroupBox2.Controls.Add(Me.TextProxyAddress)
        Me.GroupBox2.Controls.Add(Me.LabelProxyAddress)
        Me.GroupBox2.Controls.Add(Me.RadioProxySpecified)
        Me.GroupBox2.Controls.Add(Me.RadioProxyIE)
        Me.GroupBox2.Controls.Add(Me.RadioProxyNone)
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
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
        'LabelProxyPassword
        '
        resources.ApplyResources(Me.LabelProxyPassword, "LabelProxyPassword")
        Me.LabelProxyPassword.Name = "LabelProxyPassword"
        '
        'TextProxyUser
        '
        resources.ApplyResources(Me.TextProxyUser, "TextProxyUser")
        Me.TextProxyUser.Name = "TextProxyUser"
        '
        'LabelProxyUser
        '
        resources.ApplyResources(Me.LabelProxyUser, "LabelProxyUser")
        Me.LabelProxyUser.Name = "LabelProxyUser"
        '
        'TextProxyPort
        '
        resources.ApplyResources(Me.TextProxyPort, "TextProxyPort")
        Me.TextProxyPort.Name = "TextProxyPort"
        '
        'LabelProxyPort
        '
        resources.ApplyResources(Me.LabelProxyPort, "LabelProxyPort")
        Me.LabelProxyPort.Name = "LabelProxyPort"
        '
        'TextProxyAddress
        '
        resources.ApplyResources(Me.TextProxyAddress, "TextProxyAddress")
        Me.TextProxyAddress.Name = "TextProxyAddress"
        '
        'LabelProxyAddress
        '
        resources.ApplyResources(Me.LabelProxyAddress, "LabelProxyAddress")
        Me.LabelProxyAddress.Name = "LabelProxyAddress"
        '
        'RadioProxySpecified
        '
        resources.ApplyResources(Me.RadioProxySpecified, "RadioProxySpecified")
        Me.RadioProxySpecified.Name = "RadioProxySpecified"
        Me.RadioProxySpecified.UseVisualStyleBackColor = True
        '
        'RadioProxyIE
        '
        resources.ApplyResources(Me.RadioProxyIE, "RadioProxyIE")
        Me.RadioProxyIE.Checked = True
        Me.RadioProxyIE.Name = "RadioProxyIE"
        Me.RadioProxyIE.TabStop = True
        Me.RadioProxyIE.UseVisualStyleBackColor = True
        '
        'RadioProxyNone
        '
        resources.ApplyResources(Me.RadioProxyNone, "RadioProxyNone")
        Me.RadioProxyNone.Name = "RadioProxyNone"
        Me.RadioProxyNone.UseVisualStyleBackColor = True
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.SearchTextCountApi)
        Me.TabPage6.Controls.Add(Me.Label66)
        Me.TabPage6.Controls.Add(Me.FirstTextCountApi)
        Me.TabPage6.Controls.Add(Me.GetMoreTextCountApi)
        Me.TabPage6.Controls.Add(Me.Label53)
        Me.TabPage6.Controls.Add(Me.UseChangeGetCount)
        Me.TabPage6.Controls.Add(Me.CheckNicoms)
        Me.TabPage6.Controls.Add(Me.Label60)
        Me.TabPage6.Controls.Add(Me.ComboBoxOutputzUrlmode)
        Me.TabPage6.Controls.Add(Me.Label59)
        Me.TabPage6.Controls.Add(Me.TextBoxOutputzKey)
        Me.TabPage6.Controls.Add(Me.CheckOutputz)
        resources.ApplyResources(Me.TabPage6, "TabPage6")
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.UseVisualStyleBackColor = True
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
        'CheckNicoms
        '
        resources.ApplyResources(Me.CheckNicoms, "CheckNicoms")
        Me.CheckNicoms.Name = "CheckNicoms"
        Me.CheckNicoms.UseVisualStyleBackColor = True
        '
        'Label60
        '
        resources.ApplyResources(Me.Label60, "Label60")
        Me.Label60.Name = "Label60"
        '
        'ComboBoxOutputzUrlmode
        '
        Me.ComboBoxOutputzUrlmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxOutputzUrlmode.FormattingEnabled = True
        Me.ComboBoxOutputzUrlmode.Items.AddRange(New Object() {resources.GetString("ComboBoxOutputzUrlmode.Items"), resources.GetString("ComboBoxOutputzUrlmode.Items1")})
        resources.ApplyResources(Me.ComboBoxOutputzUrlmode, "ComboBoxOutputzUrlmode")
        Me.ComboBoxOutputzUrlmode.Name = "ComboBoxOutputzUrlmode"
        '
        'Label59
        '
        resources.ApplyResources(Me.Label59, "Label59")
        Me.Label59.Name = "Label59"
        '
        'TextBoxOutputzKey
        '
        resources.ApplyResources(Me.TextBoxOutputzKey, "TextBoxOutputzKey")
        Me.TextBoxOutputzKey.Name = "TextBoxOutputzKey"
        '
        'CheckOutputz
        '
        resources.ApplyResources(Me.CheckOutputz, "CheckOutputz")
        Me.CheckOutputz.Name = "CheckOutputz"
        Me.CheckOutputz.UseVisualStyleBackColor = True
        '
        'Setting
        '
        Me.AcceptButton = Me.Save
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.Cancel)
        Me.Controls.Add(Me.Save)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Setting"
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage5.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.TabPage6.ResumeLayout(False)
        Me.TabPage6.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Username As System.Windows.Forms.TextBox
    Friend WithEvents Password As System.Windows.Forms.TextBox
    Friend WithEvents Save As System.Windows.Forms.Button
    Friend WithEvents Cancel As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TimelinePeriod As System.Windows.Forms.TextBox
    Friend WithEvents DMPeriod As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents StartupReaded As System.Windows.Forms.CheckBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents StatusText As System.Windows.Forms.TextBox
    Friend WithEvents PlaySnd As System.Windows.Forms.CheckBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents OneWayLv As System.Windows.Forms.CheckBox
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnDetail As System.Windows.Forms.Button
    Friend WithEvents lblDetail As System.Windows.Forms.Label
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents btnOWL As System.Windows.Forms.Button
    Friend WithEvents lblOWL As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents btnFav As System.Windows.Forms.Button
    Friend WithEvents lblFav As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents FontDialog1 As System.Windows.Forms.FontDialog
    Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
    Friend WithEvents btnAtFromTarget As System.Windows.Forms.Button
    Friend WithEvents lblAtFromTarget As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents btnAtTarget As System.Windows.Forms.Button
    Friend WithEvents lblAtTarget As System.Windows.Forms.Label
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents btnTarget As System.Windows.Forms.Button
    Friend WithEvents lblTarget As System.Windows.Forms.Label
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents btnAtSelf As System.Windows.Forms.Button
    Friend WithEvents lblAtSelf As System.Windows.Forms.Label
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents btnSelf As System.Windows.Forms.Button
    Friend WithEvents lblSelf As System.Windows.Forms.Label
    Friend WithEvents Label36 As System.Windows.Forms.Label
    Friend WithEvents cmbNameBalloon As System.Windows.Forms.ComboBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents btnListBack As System.Windows.Forms.Button
    Friend WithEvents lblListBackcolor As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents CheckUseRecommendStatus As System.Windows.Forms.CheckBox
    Friend WithEvents CmbDateTimeFormat As System.Windows.Forms.ComboBox
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents CheckBox3 As System.Windows.Forms.CheckBox
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents CheckPostCtrlEnter As System.Windows.Forms.CheckBox
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents IconSize As System.Windows.Forms.ComboBox
    Friend WithEvents btnDetailBack As System.Windows.Forms.Button
    Friend WithEvents lblDetailBackcolor As System.Windows.Forms.Label
    Friend WithEvents Label37 As System.Windows.Forms.Label
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents UReadMng As System.Windows.Forms.CheckBox
    Friend WithEvents Label39 As System.Windows.Forms.Label
    Friend WithEvents CheckReadOldPosts As System.Windows.Forms.CheckBox
    Friend WithEvents Label40 As System.Windows.Forms.Label
    Friend WithEvents CheckCloseToExit As System.Windows.Forms.CheckBox
    Friend WithEvents Label41 As System.Windows.Forms.Label
    Friend WithEvents CheckMinimizeToTray As System.Windows.Forms.CheckBox
    Friend WithEvents BrowserPathText As System.Windows.Forms.TextBox
    Friend WithEvents Label44 As System.Windows.Forms.Label
    Friend WithEvents CheckDispUsername As System.Windows.Forms.CheckBox
    Friend WithEvents Label46 As System.Windows.Forms.Label
    Friend WithEvents Label45 As System.Windows.Forms.Label
    Friend WithEvents ComboDispTitle As System.Windows.Forms.ComboBox
    Friend WithEvents Label47 As System.Windows.Forms.Label
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents btnAtTo As System.Windows.Forms.Button
    Friend WithEvents lblAtTo As System.Windows.Forms.Label
    Friend WithEvents Label49 As System.Windows.Forms.Label
    Friend WithEvents CheckTinyURL As System.Windows.Forms.CheckBox
    Friend WithEvents Label50 As System.Windows.Forms.Label
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents RadioProxySpecified As System.Windows.Forms.RadioButton
    Friend WithEvents RadioProxyIE As System.Windows.Forms.RadioButton
    Friend WithEvents RadioProxyNone As System.Windows.Forms.RadioButton
    Friend WithEvents TextProxyPort As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyPort As System.Windows.Forms.Label
    Friend WithEvents TextProxyAddress As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyAddress As System.Windows.Forms.Label
    Friend WithEvents TextProxyPassword As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyPassword As System.Windows.Forms.Label
    Friend WithEvents TextProxyUser As System.Windows.Forms.TextBox
    Friend WithEvents LabelProxyUser As System.Windows.Forms.Label
    Friend WithEvents Label55 As System.Windows.Forms.Label
    Friend WithEvents CheckPeriodAdjust As System.Windows.Forms.CheckBox
    Friend WithEvents Label51 As System.Windows.Forms.Label
    Friend WithEvents CheckStartupVersion As System.Windows.Forms.CheckBox
    Friend WithEvents Label54 As System.Windows.Forms.Label
    Friend WithEvents CheckStartupFollowers As System.Windows.Forms.CheckBox
    Friend WithEvents Label56 As System.Windows.Forms.Label
    Friend WithEvents CheckFavRestrict As System.Windows.Forms.CheckBox
    Friend WithEvents Label57 As System.Windows.Forms.Label
    Friend WithEvents CheckAutoConvertUrl As System.Windows.Forms.CheckBox
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents Label59 As System.Windows.Forms.Label
    Friend WithEvents TextBoxOutputzKey As System.Windows.Forms.TextBox
    Friend WithEvents CheckOutputz As System.Windows.Forms.CheckBox
    Friend WithEvents Label60 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxOutputzUrlmode As System.Windows.Forms.ComboBox
    Friend WithEvents btnListFont As System.Windows.Forms.Button
    Friend WithEvents lblListFont As System.Windows.Forms.Label
    Friend WithEvents Label61 As System.Windows.Forms.Label
    Friend WithEvents btnUnread As System.Windows.Forms.Button
    Friend WithEvents lblUnread As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents chkUnreadStyle As System.Windows.Forms.CheckBox
    Friend WithEvents LabelDateTimeFormatApplied As System.Windows.Forms.Label
    Friend WithEvents Label62 As System.Windows.Forms.Label
    Friend WithEvents Label63 As System.Windows.Forms.Label
    Friend WithEvents Label64 As System.Windows.Forms.Label
    Friend WithEvents ConnectionTimeOut As System.Windows.Forms.TextBox
    Friend WithEvents CheckProtectNotInclude As System.Windows.Forms.CheckBox
    Friend WithEvents Label42 As System.Windows.Forms.Label
    Friend WithEvents btnInputBackcolor As System.Windows.Forms.Button
    Friend WithEvents lblInputBackcolor As System.Windows.Forms.Label
    Friend WithEvents Label52 As System.Windows.Forms.Label
    Friend WithEvents btnInputFont As System.Windows.Forms.Button
    Friend WithEvents lblInputFont As System.Windows.Forms.Label
    Friend WithEvents Label65 As System.Windows.Forms.Label
    Friend WithEvents Label67 As System.Windows.Forms.Label
    Friend WithEvents TextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents Label68 As System.Windows.Forms.Label
    Friend WithEvents CheckBalloonLimit As System.Windows.Forms.CheckBox
    Friend WithEvents CheckPostAndGet As System.Windows.Forms.CheckBox
    Friend WithEvents Label69 As System.Windows.Forms.Label
    Friend WithEvents ReplyPeriod As System.Windows.Forms.TextBox
    Friend WithEvents ComboBoxAutoShortUrlFirst As System.Windows.Forms.ComboBox
    Friend WithEvents Label71 As System.Windows.Forms.Label
    Friend WithEvents Label48 As System.Windows.Forms.Label
    Friend WithEvents chkTabIconDisp As System.Windows.Forms.CheckBox
    Friend WithEvents ReplyIconStateCombo As System.Windows.Forms.ComboBox
    Friend WithEvents Label72 As System.Windows.Forms.Label
    Friend WithEvents Label73 As System.Windows.Forms.Label
    Friend WithEvents chkReadOwnPost As System.Windows.Forms.CheckBox
    Friend WithEvents Label74 As System.Windows.Forms.Label
    Friend WithEvents chkGetFav As System.Windows.Forms.CheckBox
    Friend WithEvents Label75 As System.Windows.Forms.Label
    Friend WithEvents CheckMonospace As System.Windows.Forms.CheckBox
    Friend WithEvents CheckUseSsl As System.Windows.Forms.CheckBox
    Friend WithEvents Label76 As System.Windows.Forms.Label
    Friend WithEvents TextBitlyPw As System.Windows.Forms.TextBox
    Friend WithEvents Label77 As System.Windows.Forms.Label
    Friend WithEvents TextBitlyId As System.Windows.Forms.TextBox
    Friend WithEvents Label78 As System.Windows.Forms.Label
    Friend WithEvents CheckShowGrid As System.Windows.Forms.CheckBox
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents CheckSortOrderLock As System.Windows.Forms.CheckBox
    Friend WithEvents Label79 As System.Windows.Forms.Label
    Friend WithEvents CheckAtIdSupple As System.Windows.Forms.CheckBox
    Friend WithEvents CheckAlwaysTop As System.Windows.Forms.CheckBox
    Friend WithEvents Label58 As System.Windows.Forms.Label
    Friend WithEvents btnDetailLink As System.Windows.Forms.Button
    Friend WithEvents lblDetailLink As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents ButtonBackToDefaultFontColor As System.Windows.Forms.Button
    Friend WithEvents btnRetweet As System.Windows.Forms.Button
    Friend WithEvents lblRetweet As System.Windows.Forms.Label
    Friend WithEvents Label80 As System.Windows.Forms.Label
    Friend WithEvents LanguageCombo As System.Windows.Forms.ComboBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label81 As System.Windows.Forms.Label
    Friend WithEvents TextCountApiReply As System.Windows.Forms.TextBox
    Friend WithEvents PubSearchPeriod As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents CheckNicoms As System.Windows.Forms.CheckBox
    Friend WithEvents Label82 As System.Windows.Forms.Label
    Friend WithEvents CheckHashSupple As System.Windows.Forms.CheckBox
    Friend WithEvents AuthorizeButton As System.Windows.Forms.Button
    Friend WithEvents AuthStateLabel As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents AuthUserLabel As System.Windows.Forms.Label
    Friend WithEvents AuthClearButton As System.Windows.Forms.Button
    Friend WithEvents AuthBasicRadio As System.Windows.Forms.RadioButton
    Friend WithEvents AuthOAuthRadio As System.Windows.Forms.RadioButton
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TwitterSearchAPIText As System.Windows.Forms.TextBox
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents TwitterAPIText As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents ListsPeriod As System.Windows.Forms.TextBox
    Friend WithEvents Label35 As System.Windows.Forms.Label
    Friend WithEvents CheckPreviewEnable As System.Windows.Forms.CheckBox
    Friend WithEvents LabelApiUsing As System.Windows.Forms.Label
    Friend WithEvents LabelPostAndGet As System.Windows.Forms.Label
    Friend WithEvents ButtonApiCalc As System.Windows.Forms.Button
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents HotkeyText As System.Windows.Forms.TextBox
    Friend WithEvents HotkeyWin As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyAlt As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyShift As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyCtrl As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyCheck As System.Windows.Forms.CheckBox
    Friend WithEvents HotkeyCode As System.Windows.Forms.Label
    Friend WithEvents Label43 As System.Windows.Forms.Label
    Friend WithEvents ChkNewMentionsBlink As System.Windows.Forms.CheckBox
    Friend WithEvents UseChangeGetCount As System.Windows.Forms.CheckBox
    Friend WithEvents GetMoreTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents Label53 As System.Windows.Forms.Label
    Friend WithEvents FirstTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents CheckEnableBasicAuth As System.Windows.Forms.CheckBox
    Friend WithEvents SearchTextCountApi As System.Windows.Forms.TextBox
    Friend WithEvents Label66 As System.Windows.Forms.Label
End Class
