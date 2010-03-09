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
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Username = New System.Windows.Forms.TextBox
        Me.Password = New System.Windows.Forms.TextBox
        Me.Save = New System.Windows.Forms.Button
        Me.Cancel = New System.Windows.Forms.Button
        Me.Label3 = New System.Windows.Forms.Label
        Me.TimelinePeriod = New System.Windows.Forms.TextBox
        Me.DMPeriod = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.StartupReaded = New System.Windows.Forms.CheckBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.StatusText = New System.Windows.Forms.TextBox
        Me.PlaySnd = New System.Windows.Forms.CheckBox
        Me.Label14 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.OneWayLv = New System.Windows.Forms.CheckBox
        Me.Label16 = New System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.btnRetweet = New System.Windows.Forms.Button
        Me.lblRetweet = New System.Windows.Forms.Label
        Me.Label80 = New System.Windows.Forms.Label
        Me.ButtonBackToDefaultFontColor = New System.Windows.Forms.Button
        Me.btnDetailLink = New System.Windows.Forms.Button
        Me.lblDetailLink = New System.Windows.Forms.Label
        Me.Label18 = New System.Windows.Forms.Label
        Me.btnInputFont = New System.Windows.Forms.Button
        Me.lblInputFont = New System.Windows.Forms.Label
        Me.Label65 = New System.Windows.Forms.Label
        Me.btnInputBackcolor = New System.Windows.Forms.Button
        Me.lblInputBackcolor = New System.Windows.Forms.Label
        Me.Label52 = New System.Windows.Forms.Label
        Me.btnUnread = New System.Windows.Forms.Button
        Me.lblUnread = New System.Windows.Forms.Label
        Me.Label20 = New System.Windows.Forms.Label
        Me.btnAtTo = New System.Windows.Forms.Button
        Me.lblAtTo = New System.Windows.Forms.Label
        Me.Label49 = New System.Windows.Forms.Label
        Me.btnDetailBack = New System.Windows.Forms.Button
        Me.lblDetailBackcolor = New System.Windows.Forms.Label
        Me.Label37 = New System.Windows.Forms.Label
        Me.btnListBack = New System.Windows.Forms.Button
        Me.lblListBackcolor = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.btnAtFromTarget = New System.Windows.Forms.Button
        Me.lblAtFromTarget = New System.Windows.Forms.Label
        Me.Label28 = New System.Windows.Forms.Label
        Me.btnAtTarget = New System.Windows.Forms.Button
        Me.lblAtTarget = New System.Windows.Forms.Label
        Me.Label30 = New System.Windows.Forms.Label
        Me.btnTarget = New System.Windows.Forms.Button
        Me.lblTarget = New System.Windows.Forms.Label
        Me.Label32 = New System.Windows.Forms.Label
        Me.btnAtSelf = New System.Windows.Forms.Button
        Me.lblAtSelf = New System.Windows.Forms.Label
        Me.Label34 = New System.Windows.Forms.Label
        Me.btnSelf = New System.Windows.Forms.Button
        Me.lblSelf = New System.Windows.Forms.Label
        Me.Label36 = New System.Windows.Forms.Label
        Me.btnDetail = New System.Windows.Forms.Button
        Me.lblDetail = New System.Windows.Forms.Label
        Me.Label26 = New System.Windows.Forms.Label
        Me.btnOWL = New System.Windows.Forms.Button
        Me.lblOWL = New System.Windows.Forms.Label
        Me.Label24 = New System.Windows.Forms.Label
        Me.btnFav = New System.Windows.Forms.Button
        Me.lblFav = New System.Windows.Forms.Label
        Me.Label22 = New System.Windows.Forms.Label
        Me.btnListFont = New System.Windows.Forms.Button
        Me.lblListFont = New System.Windows.Forms.Label
        Me.Label61 = New System.Windows.Forms.Label
        Me.FontDialog1 = New System.Windows.Forms.FontDialog
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog
        Me.cmbNameBalloon = New System.Windows.Forms.ComboBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.CheckUseRecommendStatus = New System.Windows.Forms.CheckBox
        Me.CmbDateTimeFormat = New System.Windows.Forms.ComboBox
        Me.Label23 = New System.Windows.Forms.Label
        Me.CheckBox3 = New System.Windows.Forms.CheckBox
        Me.Label25 = New System.Windows.Forms.Label
        Me.CheckPostCtrlEnter = New System.Windows.Forms.CheckBox
        Me.Label27 = New System.Windows.Forms.Label
        Me.TextBox3 = New System.Windows.Forms.TextBox
        Me.IconSize = New System.Windows.Forms.ComboBox
        Me.Label38 = New System.Windows.Forms.Label
        Me.UReadMng = New System.Windows.Forms.CheckBox
        Me.Label39 = New System.Windows.Forms.Label
        Me.CheckReadOldPosts = New System.Windows.Forms.CheckBox
        Me.Label40 = New System.Windows.Forms.Label
        Me.CheckCloseToExit = New System.Windows.Forms.CheckBox
        Me.Label41 = New System.Windows.Forms.Label
        Me.CheckMinimizeToTray = New System.Windows.Forms.CheckBox
        Me.BrowserPathText = New System.Windows.Forms.TextBox
        Me.Label44 = New System.Windows.Forms.Label
        Me.CheckDispUsername = New System.Windows.Forms.CheckBox
        Me.Label46 = New System.Windows.Forms.Label
        Me.Label45 = New System.Windows.Forms.Label
        Me.ComboDispTitle = New System.Windows.Forms.ComboBox
        Me.Label47 = New System.Windows.Forms.Label
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.AuthBasicRadio = New System.Windows.Forms.RadioButton
        Me.AuthOAuthRadio = New System.Windows.Forms.RadioButton
        Me.Label6 = New System.Windows.Forms.Label
        Me.AuthClearButton = New System.Windows.Forms.Button
        Me.AuthUserLabel = New System.Windows.Forms.Label
        Me.AuthStateLabel = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.AuthorizeButton = New System.Windows.Forms.Button
        Me.TextCountApiReply = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.PubSearchPeriod = New System.Windows.Forms.TextBox
        Me.Label69 = New System.Windows.Forms.Label
        Me.ReplyPeriod = New System.Windows.Forms.TextBox
        Me.CheckPostAndGet = New System.Windows.Forms.CheckBox
        Me.Label67 = New System.Windows.Forms.Label
        Me.TextCountApi = New System.Windows.Forms.TextBox
        Me.Label54 = New System.Windows.Forms.Label
        Me.CheckStartupFollowers = New System.Windows.Forms.CheckBox
        Me.Label51 = New System.Windows.Forms.Label
        Me.CheckStartupVersion = New System.Windows.Forms.CheckBox
        Me.CheckPeriodAdjust = New System.Windows.Forms.CheckBox
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.Label82 = New System.Windows.Forms.Label
        Me.CheckHashSupple = New System.Windows.Forms.CheckBox
        Me.Label79 = New System.Windows.Forms.Label
        Me.CheckAtIdSupple = New System.Windows.Forms.CheckBox
        Me.TextBitlyPw = New System.Windows.Forms.TextBox
        Me.Label77 = New System.Windows.Forms.Label
        Me.TextBitlyId = New System.Windows.Forms.TextBox
        Me.Label76 = New System.Windows.Forms.Label
        Me.Label74 = New System.Windows.Forms.Label
        Me.chkGetFav = New System.Windows.Forms.CheckBox
        Me.ComboBoxAutoShortUrlFirst = New System.Windows.Forms.ComboBox
        Me.Label71 = New System.Windows.Forms.Label
        Me.CheckProtectNotInclude = New System.Windows.Forms.CheckBox
        Me.Label42 = New System.Windows.Forms.Label
        Me.CheckAutoConvertUrl = New System.Windows.Forms.CheckBox
        Me.Label29 = New System.Windows.Forms.Label
        Me.Label57 = New System.Windows.Forms.Label
        Me.Label56 = New System.Windows.Forms.Label
        Me.CheckFavRestrict = New System.Windows.Forms.CheckBox
        Me.CheckTinyURL = New System.Windows.Forms.CheckBox
        Me.Label50 = New System.Windows.Forms.Label
        Me.Button3 = New System.Windows.Forms.Button
        Me.TabPage3 = New System.Windows.Forms.TabPage
        Me.Label81 = New System.Windows.Forms.Label
        Me.LanguageCombo = New System.Windows.Forms.ComboBox
        Me.Label13 = New System.Windows.Forms.Label
        Me.CheckAlwaysTop = New System.Windows.Forms.CheckBox
        Me.Label58 = New System.Windows.Forms.Label
        Me.Label21 = New System.Windows.Forms.Label
        Me.CheckSortOrderLock = New System.Windows.Forms.CheckBox
        Me.Label78 = New System.Windows.Forms.Label
        Me.CheckShowGrid = New System.Windows.Forms.CheckBox
        Me.Label75 = New System.Windows.Forms.Label
        Me.CheckMonospace = New System.Windows.Forms.CheckBox
        Me.Label73 = New System.Windows.Forms.Label
        Me.chkReadOwnPost = New System.Windows.Forms.CheckBox
        Me.ReplyIconStateCombo = New System.Windows.Forms.ComboBox
        Me.Label72 = New System.Windows.Forms.Label
        Me.Label48 = New System.Windows.Forms.Label
        Me.chkTabIconDisp = New System.Windows.Forms.CheckBox
        Me.Label68 = New System.Windows.Forms.Label
        Me.CheckBalloonLimit = New System.Windows.Forms.CheckBox
        Me.LabelDateTimeFormatApplied = New System.Windows.Forms.Label
        Me.Label62 = New System.Windows.Forms.Label
        Me.Label17 = New System.Windows.Forms.Label
        Me.chkUnreadStyle = New System.Windows.Forms.CheckBox
        Me.TabPage4 = New System.Windows.Forms.TabPage
        Me.TabPage5 = New System.Windows.Forms.TabPage
        Me.TwitterSearchAPIText = New System.Windows.Forms.TextBox
        Me.Label31 = New System.Windows.Forms.Label
        Me.TwitterAPIText = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.CheckUseSsl = New System.Windows.Forms.CheckBox
        Me.Label64 = New System.Windows.Forms.Label
        Me.ConnectionTimeOut = New System.Windows.Forms.TextBox
        Me.Label63 = New System.Windows.Forms.Label
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.Label55 = New System.Windows.Forms.Label
        Me.TextProxyPassword = New System.Windows.Forms.TextBox
        Me.LabelProxyPassword = New System.Windows.Forms.Label
        Me.TextProxyUser = New System.Windows.Forms.TextBox
        Me.LabelProxyUser = New System.Windows.Forms.Label
        Me.TextProxyPort = New System.Windows.Forms.TextBox
        Me.LabelProxyPort = New System.Windows.Forms.Label
        Me.TextProxyAddress = New System.Windows.Forms.TextBox
        Me.LabelProxyAddress = New System.Windows.Forms.Label
        Me.RadioProxySpecified = New System.Windows.Forms.RadioButton
        Me.RadioProxyIE = New System.Windows.Forms.RadioButton
        Me.RadioProxyNone = New System.Windows.Forms.RadioButton
        Me.TabPage6 = New System.Windows.Forms.TabPage
        Me.CheckNicoms = New System.Windows.Forms.CheckBox
        Me.Label60 = New System.Windows.Forms.Label
        Me.ComboBoxOutputzUrlmode = New System.Windows.Forms.ComboBox
        Me.Label59 = New System.Windows.Forms.Label
        Me.TextBoxOutputzKey = New System.Windows.Forms.TextBox
        Me.CheckOutputz = New System.Windows.Forms.CheckBox
        Me.GroupBox1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = Nothing
        Me.Label1.AccessibleName = Nothing
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Font = Nothing
        Me.Label1.Name = "Label1"
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = Nothing
        Me.Label2.AccessibleName = Nothing
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Font = Nothing
        Me.Label2.Name = "Label2"
        '
        'Username
        '
        Me.Username.AccessibleDescription = Nothing
        Me.Username.AccessibleName = Nothing
        resources.ApplyResources(Me.Username, "Username")
        Me.Username.BackgroundImage = Nothing
        Me.Username.Font = Nothing
        Me.Username.Name = "Username"
        '
        'Password
        '
        Me.Password.AccessibleDescription = Nothing
        Me.Password.AccessibleName = Nothing
        resources.ApplyResources(Me.Password, "Password")
        Me.Password.BackgroundImage = Nothing
        Me.Password.Font = Nothing
        Me.Password.Name = "Password"
        Me.Password.UseSystemPasswordChar = True
        '
        'Save
        '
        Me.Save.AccessibleDescription = Nothing
        Me.Save.AccessibleName = Nothing
        resources.ApplyResources(Me.Save, "Save")
        Me.Save.BackgroundImage = Nothing
        Me.Save.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Save.Font = Nothing
        Me.Save.Name = "Save"
        Me.Save.UseVisualStyleBackColor = True
        '
        'Cancel
        '
        Me.Cancel.AccessibleDescription = Nothing
        Me.Cancel.AccessibleName = Nothing
        resources.ApplyResources(Me.Cancel, "Cancel")
        Me.Cancel.BackgroundImage = Nothing
        Me.Cancel.CausesValidation = False
        Me.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel.Font = Nothing
        Me.Cancel.Name = "Cancel"
        Me.Cancel.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = Nothing
        Me.Label3.AccessibleName = Nothing
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Font = Nothing
        Me.Label3.Name = "Label3"
        '
        'TimelinePeriod
        '
        Me.TimelinePeriod.AccessibleDescription = Nothing
        Me.TimelinePeriod.AccessibleName = Nothing
        resources.ApplyResources(Me.TimelinePeriod, "TimelinePeriod")
        Me.TimelinePeriod.BackgroundImage = Nothing
        Me.TimelinePeriod.Font = Nothing
        Me.TimelinePeriod.Name = "TimelinePeriod"
        '
        'DMPeriod
        '
        Me.DMPeriod.AccessibleDescription = Nothing
        Me.DMPeriod.AccessibleName = Nothing
        resources.ApplyResources(Me.DMPeriod, "DMPeriod")
        Me.DMPeriod.BackgroundImage = Nothing
        Me.DMPeriod.Font = Nothing
        Me.DMPeriod.Name = "DMPeriod"
        '
        'Label5
        '
        Me.Label5.AccessibleDescription = Nothing
        Me.Label5.AccessibleName = Nothing
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Font = Nothing
        Me.Label5.Name = "Label5"
        '
        'Label9
        '
        Me.Label9.AccessibleDescription = Nothing
        Me.Label9.AccessibleName = Nothing
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Font = Nothing
        Me.Label9.Name = "Label9"
        '
        'StartupReaded
        '
        Me.StartupReaded.AccessibleDescription = Nothing
        Me.StartupReaded.AccessibleName = Nothing
        resources.ApplyResources(Me.StartupReaded, "StartupReaded")
        Me.StartupReaded.BackgroundImage = Nothing
        Me.StartupReaded.Font = Nothing
        Me.StartupReaded.Name = "StartupReaded"
        Me.StartupReaded.UseVisualStyleBackColor = True
        '
        'Label11
        '
        Me.Label11.AccessibleDescription = Nothing
        Me.Label11.AccessibleName = Nothing
        resources.ApplyResources(Me.Label11, "Label11")
        Me.Label11.Font = Nothing
        Me.Label11.Name = "Label11"
        '
        'Label12
        '
        Me.Label12.AccessibleDescription = Nothing
        Me.Label12.AccessibleName = Nothing
        resources.ApplyResources(Me.Label12, "Label12")
        Me.Label12.Font = Nothing
        Me.Label12.Name = "Label12"
        '
        'StatusText
        '
        Me.StatusText.AccessibleDescription = Nothing
        Me.StatusText.AccessibleName = Nothing
        resources.ApplyResources(Me.StatusText, "StatusText")
        Me.StatusText.BackgroundImage = Nothing
        Me.StatusText.Font = Nothing
        Me.StatusText.Name = "StatusText"
        '
        'PlaySnd
        '
        Me.PlaySnd.AccessibleDescription = Nothing
        Me.PlaySnd.AccessibleName = Nothing
        resources.ApplyResources(Me.PlaySnd, "PlaySnd")
        Me.PlaySnd.BackgroundImage = Nothing
        Me.PlaySnd.Font = Nothing
        Me.PlaySnd.Name = "PlaySnd"
        Me.PlaySnd.UseVisualStyleBackColor = True
        '
        'Label14
        '
        Me.Label14.AccessibleDescription = Nothing
        Me.Label14.AccessibleName = Nothing
        resources.ApplyResources(Me.Label14, "Label14")
        Me.Label14.Font = Nothing
        Me.Label14.Name = "Label14"
        '
        'Label15
        '
        Me.Label15.AccessibleDescription = Nothing
        Me.Label15.AccessibleName = Nothing
        resources.ApplyResources(Me.Label15, "Label15")
        Me.Label15.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label15.Font = Nothing
        Me.Label15.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label15.Name = "Label15"
        '
        'OneWayLv
        '
        Me.OneWayLv.AccessibleDescription = Nothing
        Me.OneWayLv.AccessibleName = Nothing
        resources.ApplyResources(Me.OneWayLv, "OneWayLv")
        Me.OneWayLv.BackgroundImage = Nothing
        Me.OneWayLv.Font = Nothing
        Me.OneWayLv.Name = "OneWayLv"
        Me.OneWayLv.UseVisualStyleBackColor = True
        '
        'Label16
        '
        Me.Label16.AccessibleDescription = Nothing
        Me.Label16.AccessibleName = Nothing
        resources.ApplyResources(Me.Label16, "Label16")
        Me.Label16.Font = Nothing
        Me.Label16.Name = "Label16"
        '
        'GroupBox1
        '
        Me.GroupBox1.AccessibleDescription = Nothing
        Me.GroupBox1.AccessibleName = Nothing
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.BackgroundImage = Nothing
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
        Me.GroupBox1.Font = Nothing
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'btnRetweet
        '
        Me.btnRetweet.AccessibleDescription = Nothing
        Me.btnRetweet.AccessibleName = Nothing
        resources.ApplyResources(Me.btnRetweet, "btnRetweet")
        Me.btnRetweet.BackgroundImage = Nothing
        Me.btnRetweet.Font = Nothing
        Me.btnRetweet.Name = "btnRetweet"
        Me.btnRetweet.UseVisualStyleBackColor = True
        '
        'lblRetweet
        '
        Me.lblRetweet.AccessibleDescription = Nothing
        Me.lblRetweet.AccessibleName = Nothing
        resources.ApplyResources(Me.lblRetweet, "lblRetweet")
        Me.lblRetweet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblRetweet.Font = Nothing
        Me.lblRetweet.Name = "lblRetweet"
        '
        'Label80
        '
        Me.Label80.AccessibleDescription = Nothing
        Me.Label80.AccessibleName = Nothing
        resources.ApplyResources(Me.Label80, "Label80")
        Me.Label80.Font = Nothing
        Me.Label80.Name = "Label80"
        '
        'ButtonBackToDefaultFontColor
        '
        Me.ButtonBackToDefaultFontColor.AccessibleDescription = Nothing
        Me.ButtonBackToDefaultFontColor.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonBackToDefaultFontColor, "ButtonBackToDefaultFontColor")
        Me.ButtonBackToDefaultFontColor.BackgroundImage = Nothing
        Me.ButtonBackToDefaultFontColor.Font = Nothing
        Me.ButtonBackToDefaultFontColor.Name = "ButtonBackToDefaultFontColor"
        Me.ButtonBackToDefaultFontColor.UseVisualStyleBackColor = True
        '
        'btnDetailLink
        '
        Me.btnDetailLink.AccessibleDescription = Nothing
        Me.btnDetailLink.AccessibleName = Nothing
        resources.ApplyResources(Me.btnDetailLink, "btnDetailLink")
        Me.btnDetailLink.BackgroundImage = Nothing
        Me.btnDetailLink.Font = Nothing
        Me.btnDetailLink.Name = "btnDetailLink"
        Me.btnDetailLink.UseVisualStyleBackColor = True
        '
        'lblDetailLink
        '
        Me.lblDetailLink.AccessibleDescription = Nothing
        Me.lblDetailLink.AccessibleName = Nothing
        resources.ApplyResources(Me.lblDetailLink, "lblDetailLink")
        Me.lblDetailLink.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDetailLink.Font = Nothing
        Me.lblDetailLink.Name = "lblDetailLink"
        '
        'Label18
        '
        Me.Label18.AccessibleDescription = Nothing
        Me.Label18.AccessibleName = Nothing
        resources.ApplyResources(Me.Label18, "Label18")
        Me.Label18.Font = Nothing
        Me.Label18.Name = "Label18"
        '
        'btnInputFont
        '
        Me.btnInputFont.AccessibleDescription = Nothing
        Me.btnInputFont.AccessibleName = Nothing
        resources.ApplyResources(Me.btnInputFont, "btnInputFont")
        Me.btnInputFont.BackgroundImage = Nothing
        Me.btnInputFont.Font = Nothing
        Me.btnInputFont.Name = "btnInputFont"
        Me.btnInputFont.UseVisualStyleBackColor = True
        '
        'lblInputFont
        '
        Me.lblInputFont.AccessibleDescription = Nothing
        Me.lblInputFont.AccessibleName = Nothing
        resources.ApplyResources(Me.lblInputFont, "lblInputFont")
        Me.lblInputFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblInputFont.Font = Nothing
        Me.lblInputFont.Name = "lblInputFont"
        '
        'Label65
        '
        Me.Label65.AccessibleDescription = Nothing
        Me.Label65.AccessibleName = Nothing
        resources.ApplyResources(Me.Label65, "Label65")
        Me.Label65.Font = Nothing
        Me.Label65.Name = "Label65"
        '
        'btnInputBackcolor
        '
        Me.btnInputBackcolor.AccessibleDescription = Nothing
        Me.btnInputBackcolor.AccessibleName = Nothing
        resources.ApplyResources(Me.btnInputBackcolor, "btnInputBackcolor")
        Me.btnInputBackcolor.BackgroundImage = Nothing
        Me.btnInputBackcolor.Font = Nothing
        Me.btnInputBackcolor.Name = "btnInputBackcolor"
        Me.btnInputBackcolor.UseVisualStyleBackColor = True
        '
        'lblInputBackcolor
        '
        Me.lblInputBackcolor.AccessibleDescription = Nothing
        Me.lblInputBackcolor.AccessibleName = Nothing
        resources.ApplyResources(Me.lblInputBackcolor, "lblInputBackcolor")
        Me.lblInputBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblInputBackcolor.Font = Nothing
        Me.lblInputBackcolor.Name = "lblInputBackcolor"
        '
        'Label52
        '
        Me.Label52.AccessibleDescription = Nothing
        Me.Label52.AccessibleName = Nothing
        resources.ApplyResources(Me.Label52, "Label52")
        Me.Label52.Font = Nothing
        Me.Label52.Name = "Label52"
        '
        'btnUnread
        '
        Me.btnUnread.AccessibleDescription = Nothing
        Me.btnUnread.AccessibleName = Nothing
        resources.ApplyResources(Me.btnUnread, "btnUnread")
        Me.btnUnread.BackgroundImage = Nothing
        Me.btnUnread.Font = Nothing
        Me.btnUnread.Name = "btnUnread"
        Me.btnUnread.UseVisualStyleBackColor = True
        '
        'lblUnread
        '
        Me.lblUnread.AccessibleDescription = Nothing
        Me.lblUnread.AccessibleName = Nothing
        resources.ApplyResources(Me.lblUnread, "lblUnread")
        Me.lblUnread.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblUnread.Font = Nothing
        Me.lblUnread.Name = "lblUnread"
        '
        'Label20
        '
        Me.Label20.AccessibleDescription = Nothing
        Me.Label20.AccessibleName = Nothing
        resources.ApplyResources(Me.Label20, "Label20")
        Me.Label20.Font = Nothing
        Me.Label20.Name = "Label20"
        '
        'btnAtTo
        '
        Me.btnAtTo.AccessibleDescription = Nothing
        Me.btnAtTo.AccessibleName = Nothing
        resources.ApplyResources(Me.btnAtTo, "btnAtTo")
        Me.btnAtTo.BackgroundImage = Nothing
        Me.btnAtTo.Font = Nothing
        Me.btnAtTo.Name = "btnAtTo"
        Me.btnAtTo.UseVisualStyleBackColor = True
        '
        'lblAtTo
        '
        Me.lblAtTo.AccessibleDescription = Nothing
        Me.lblAtTo.AccessibleName = Nothing
        resources.ApplyResources(Me.lblAtTo, "lblAtTo")
        Me.lblAtTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtTo.Font = Nothing
        Me.lblAtTo.Name = "lblAtTo"
        '
        'Label49
        '
        Me.Label49.AccessibleDescription = Nothing
        Me.Label49.AccessibleName = Nothing
        resources.ApplyResources(Me.Label49, "Label49")
        Me.Label49.Font = Nothing
        Me.Label49.Name = "Label49"
        '
        'btnDetailBack
        '
        Me.btnDetailBack.AccessibleDescription = Nothing
        Me.btnDetailBack.AccessibleName = Nothing
        resources.ApplyResources(Me.btnDetailBack, "btnDetailBack")
        Me.btnDetailBack.BackgroundImage = Nothing
        Me.btnDetailBack.Font = Nothing
        Me.btnDetailBack.Name = "btnDetailBack"
        Me.btnDetailBack.UseVisualStyleBackColor = True
        '
        'lblDetailBackcolor
        '
        Me.lblDetailBackcolor.AccessibleDescription = Nothing
        Me.lblDetailBackcolor.AccessibleName = Nothing
        resources.ApplyResources(Me.lblDetailBackcolor, "lblDetailBackcolor")
        Me.lblDetailBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDetailBackcolor.Font = Nothing
        Me.lblDetailBackcolor.Name = "lblDetailBackcolor"
        '
        'Label37
        '
        Me.Label37.AccessibleDescription = Nothing
        Me.Label37.AccessibleName = Nothing
        resources.ApplyResources(Me.Label37, "Label37")
        Me.Label37.Font = Nothing
        Me.Label37.Name = "Label37"
        '
        'btnListBack
        '
        Me.btnListBack.AccessibleDescription = Nothing
        Me.btnListBack.AccessibleName = Nothing
        resources.ApplyResources(Me.btnListBack, "btnListBack")
        Me.btnListBack.BackgroundImage = Nothing
        Me.btnListBack.Font = Nothing
        Me.btnListBack.Name = "btnListBack"
        Me.btnListBack.UseVisualStyleBackColor = True
        '
        'lblListBackcolor
        '
        Me.lblListBackcolor.AccessibleDescription = Nothing
        Me.lblListBackcolor.AccessibleName = Nothing
        resources.ApplyResources(Me.lblListBackcolor, "lblListBackcolor")
        Me.lblListBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblListBackcolor.Font = Nothing
        Me.lblListBackcolor.Name = "lblListBackcolor"
        '
        'Label19
        '
        Me.Label19.AccessibleDescription = Nothing
        Me.Label19.AccessibleName = Nothing
        resources.ApplyResources(Me.Label19, "Label19")
        Me.Label19.Font = Nothing
        Me.Label19.Name = "Label19"
        '
        'btnAtFromTarget
        '
        Me.btnAtFromTarget.AccessibleDescription = Nothing
        Me.btnAtFromTarget.AccessibleName = Nothing
        resources.ApplyResources(Me.btnAtFromTarget, "btnAtFromTarget")
        Me.btnAtFromTarget.BackgroundImage = Nothing
        Me.btnAtFromTarget.Font = Nothing
        Me.btnAtFromTarget.Name = "btnAtFromTarget"
        Me.btnAtFromTarget.UseVisualStyleBackColor = True
        '
        'lblAtFromTarget
        '
        Me.lblAtFromTarget.AccessibleDescription = Nothing
        Me.lblAtFromTarget.AccessibleName = Nothing
        resources.ApplyResources(Me.lblAtFromTarget, "lblAtFromTarget")
        Me.lblAtFromTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtFromTarget.Font = Nothing
        Me.lblAtFromTarget.Name = "lblAtFromTarget"
        '
        'Label28
        '
        Me.Label28.AccessibleDescription = Nothing
        Me.Label28.AccessibleName = Nothing
        resources.ApplyResources(Me.Label28, "Label28")
        Me.Label28.Font = Nothing
        Me.Label28.Name = "Label28"
        '
        'btnAtTarget
        '
        Me.btnAtTarget.AccessibleDescription = Nothing
        Me.btnAtTarget.AccessibleName = Nothing
        resources.ApplyResources(Me.btnAtTarget, "btnAtTarget")
        Me.btnAtTarget.BackgroundImage = Nothing
        Me.btnAtTarget.Font = Nothing
        Me.btnAtTarget.Name = "btnAtTarget"
        Me.btnAtTarget.UseVisualStyleBackColor = True
        '
        'lblAtTarget
        '
        Me.lblAtTarget.AccessibleDescription = Nothing
        Me.lblAtTarget.AccessibleName = Nothing
        resources.ApplyResources(Me.lblAtTarget, "lblAtTarget")
        Me.lblAtTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtTarget.Font = Nothing
        Me.lblAtTarget.Name = "lblAtTarget"
        '
        'Label30
        '
        Me.Label30.AccessibleDescription = Nothing
        Me.Label30.AccessibleName = Nothing
        resources.ApplyResources(Me.Label30, "Label30")
        Me.Label30.Font = Nothing
        Me.Label30.Name = "Label30"
        '
        'btnTarget
        '
        Me.btnTarget.AccessibleDescription = Nothing
        Me.btnTarget.AccessibleName = Nothing
        resources.ApplyResources(Me.btnTarget, "btnTarget")
        Me.btnTarget.BackgroundImage = Nothing
        Me.btnTarget.Font = Nothing
        Me.btnTarget.Name = "btnTarget"
        Me.btnTarget.UseVisualStyleBackColor = True
        '
        'lblTarget
        '
        Me.lblTarget.AccessibleDescription = Nothing
        Me.lblTarget.AccessibleName = Nothing
        resources.ApplyResources(Me.lblTarget, "lblTarget")
        Me.lblTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTarget.Font = Nothing
        Me.lblTarget.Name = "lblTarget"
        '
        'Label32
        '
        Me.Label32.AccessibleDescription = Nothing
        Me.Label32.AccessibleName = Nothing
        resources.ApplyResources(Me.Label32, "Label32")
        Me.Label32.Font = Nothing
        Me.Label32.Name = "Label32"
        '
        'btnAtSelf
        '
        Me.btnAtSelf.AccessibleDescription = Nothing
        Me.btnAtSelf.AccessibleName = Nothing
        resources.ApplyResources(Me.btnAtSelf, "btnAtSelf")
        Me.btnAtSelf.BackgroundImage = Nothing
        Me.btnAtSelf.Font = Nothing
        Me.btnAtSelf.Name = "btnAtSelf"
        Me.btnAtSelf.UseVisualStyleBackColor = True
        '
        'lblAtSelf
        '
        Me.lblAtSelf.AccessibleDescription = Nothing
        Me.lblAtSelf.AccessibleName = Nothing
        resources.ApplyResources(Me.lblAtSelf, "lblAtSelf")
        Me.lblAtSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblAtSelf.Font = Nothing
        Me.lblAtSelf.Name = "lblAtSelf"
        '
        'Label34
        '
        Me.Label34.AccessibleDescription = Nothing
        Me.Label34.AccessibleName = Nothing
        resources.ApplyResources(Me.Label34, "Label34")
        Me.Label34.Font = Nothing
        Me.Label34.Name = "Label34"
        '
        'btnSelf
        '
        Me.btnSelf.AccessibleDescription = Nothing
        Me.btnSelf.AccessibleName = Nothing
        resources.ApplyResources(Me.btnSelf, "btnSelf")
        Me.btnSelf.BackgroundImage = Nothing
        Me.btnSelf.Font = Nothing
        Me.btnSelf.Name = "btnSelf"
        Me.btnSelf.UseVisualStyleBackColor = True
        '
        'lblSelf
        '
        Me.lblSelf.AccessibleDescription = Nothing
        Me.lblSelf.AccessibleName = Nothing
        resources.ApplyResources(Me.lblSelf, "lblSelf")
        Me.lblSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSelf.Font = Nothing
        Me.lblSelf.Name = "lblSelf"
        '
        'Label36
        '
        Me.Label36.AccessibleDescription = Nothing
        Me.Label36.AccessibleName = Nothing
        resources.ApplyResources(Me.Label36, "Label36")
        Me.Label36.Font = Nothing
        Me.Label36.Name = "Label36"
        '
        'btnDetail
        '
        Me.btnDetail.AccessibleDescription = Nothing
        Me.btnDetail.AccessibleName = Nothing
        resources.ApplyResources(Me.btnDetail, "btnDetail")
        Me.btnDetail.BackgroundImage = Nothing
        Me.btnDetail.Font = Nothing
        Me.btnDetail.Name = "btnDetail"
        Me.btnDetail.UseVisualStyleBackColor = True
        '
        'lblDetail
        '
        Me.lblDetail.AccessibleDescription = Nothing
        Me.lblDetail.AccessibleName = Nothing
        resources.ApplyResources(Me.lblDetail, "lblDetail")
        Me.lblDetail.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDetail.Font = Nothing
        Me.lblDetail.Name = "lblDetail"
        '
        'Label26
        '
        Me.Label26.AccessibleDescription = Nothing
        Me.Label26.AccessibleName = Nothing
        resources.ApplyResources(Me.Label26, "Label26")
        Me.Label26.Font = Nothing
        Me.Label26.Name = "Label26"
        '
        'btnOWL
        '
        Me.btnOWL.AccessibleDescription = Nothing
        Me.btnOWL.AccessibleName = Nothing
        resources.ApplyResources(Me.btnOWL, "btnOWL")
        Me.btnOWL.BackgroundImage = Nothing
        Me.btnOWL.Font = Nothing
        Me.btnOWL.Name = "btnOWL"
        Me.btnOWL.UseVisualStyleBackColor = True
        '
        'lblOWL
        '
        Me.lblOWL.AccessibleDescription = Nothing
        Me.lblOWL.AccessibleName = Nothing
        resources.ApplyResources(Me.lblOWL, "lblOWL")
        Me.lblOWL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblOWL.Font = Nothing
        Me.lblOWL.Name = "lblOWL"
        '
        'Label24
        '
        Me.Label24.AccessibleDescription = Nothing
        Me.Label24.AccessibleName = Nothing
        resources.ApplyResources(Me.Label24, "Label24")
        Me.Label24.Font = Nothing
        Me.Label24.Name = "Label24"
        '
        'btnFav
        '
        Me.btnFav.AccessibleDescription = Nothing
        Me.btnFav.AccessibleName = Nothing
        resources.ApplyResources(Me.btnFav, "btnFav")
        Me.btnFav.BackgroundImage = Nothing
        Me.btnFav.Font = Nothing
        Me.btnFav.Name = "btnFav"
        Me.btnFav.UseVisualStyleBackColor = True
        '
        'lblFav
        '
        Me.lblFav.AccessibleDescription = Nothing
        Me.lblFav.AccessibleName = Nothing
        resources.ApplyResources(Me.lblFav, "lblFav")
        Me.lblFav.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFav.Font = Nothing
        Me.lblFav.Name = "lblFav"
        '
        'Label22
        '
        Me.Label22.AccessibleDescription = Nothing
        Me.Label22.AccessibleName = Nothing
        resources.ApplyResources(Me.Label22, "Label22")
        Me.Label22.Font = Nothing
        Me.Label22.Name = "Label22"
        '
        'btnListFont
        '
        Me.btnListFont.AccessibleDescription = Nothing
        Me.btnListFont.AccessibleName = Nothing
        resources.ApplyResources(Me.btnListFont, "btnListFont")
        Me.btnListFont.BackgroundImage = Nothing
        Me.btnListFont.Font = Nothing
        Me.btnListFont.Name = "btnListFont"
        Me.btnListFont.UseVisualStyleBackColor = True
        '
        'lblListFont
        '
        Me.lblListFont.AccessibleDescription = Nothing
        Me.lblListFont.AccessibleName = Nothing
        resources.ApplyResources(Me.lblListFont, "lblListFont")
        Me.lblListFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblListFont.Font = Nothing
        Me.lblListFont.Name = "lblListFont"
        '
        'Label61
        '
        Me.Label61.AccessibleDescription = Nothing
        Me.Label61.AccessibleName = Nothing
        resources.ApplyResources(Me.Label61, "Label61")
        Me.Label61.Font = Nothing
        Me.Label61.Name = "Label61"
        '
        'cmbNameBalloon
        '
        Me.cmbNameBalloon.AccessibleDescription = Nothing
        Me.cmbNameBalloon.AccessibleName = Nothing
        resources.ApplyResources(Me.cmbNameBalloon, "cmbNameBalloon")
        Me.cmbNameBalloon.BackgroundImage = Nothing
        Me.cmbNameBalloon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNameBalloon.Font = Nothing
        Me.cmbNameBalloon.FormattingEnabled = True
        Me.cmbNameBalloon.Items.AddRange(New Object() {resources.GetString("cmbNameBalloon.Items"), resources.GetString("cmbNameBalloon.Items1"), resources.GetString("cmbNameBalloon.Items2")})
        Me.cmbNameBalloon.Name = "cmbNameBalloon"
        '
        'Label10
        '
        Me.Label10.AccessibleDescription = Nothing
        Me.Label10.AccessibleName = Nothing
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Font = Nothing
        Me.Label10.Name = "Label10"
        '
        'CheckUseRecommendStatus
        '
        Me.CheckUseRecommendStatus.AccessibleDescription = Nothing
        Me.CheckUseRecommendStatus.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckUseRecommendStatus, "CheckUseRecommendStatus")
        Me.CheckUseRecommendStatus.BackgroundImage = Nothing
        Me.CheckUseRecommendStatus.Font = Nothing
        Me.CheckUseRecommendStatus.Name = "CheckUseRecommendStatus"
        Me.CheckUseRecommendStatus.UseVisualStyleBackColor = True
        '
        'CmbDateTimeFormat
        '
        Me.CmbDateTimeFormat.AccessibleDescription = Nothing
        Me.CmbDateTimeFormat.AccessibleName = Nothing
        resources.ApplyResources(Me.CmbDateTimeFormat, "CmbDateTimeFormat")
        Me.CmbDateTimeFormat.BackgroundImage = Nothing
        Me.CmbDateTimeFormat.Font = Nothing
        Me.CmbDateTimeFormat.Items.AddRange(New Object() {resources.GetString("CmbDateTimeFormat.Items"), resources.GetString("CmbDateTimeFormat.Items1"), resources.GetString("CmbDateTimeFormat.Items2"), resources.GetString("CmbDateTimeFormat.Items3"), resources.GetString("CmbDateTimeFormat.Items4"), resources.GetString("CmbDateTimeFormat.Items5"), resources.GetString("CmbDateTimeFormat.Items6"), resources.GetString("CmbDateTimeFormat.Items7"), resources.GetString("CmbDateTimeFormat.Items8"), resources.GetString("CmbDateTimeFormat.Items9"), resources.GetString("CmbDateTimeFormat.Items10")})
        Me.CmbDateTimeFormat.Name = "CmbDateTimeFormat"
        '
        'Label23
        '
        Me.Label23.AccessibleDescription = Nothing
        Me.Label23.AccessibleName = Nothing
        resources.ApplyResources(Me.Label23, "Label23")
        Me.Label23.Font = Nothing
        Me.Label23.Name = "Label23"
        '
        'CheckBox3
        '
        Me.CheckBox3.AccessibleDescription = Nothing
        Me.CheckBox3.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckBox3, "CheckBox3")
        Me.CheckBox3.BackgroundImage = Nothing
        Me.CheckBox3.Font = Nothing
        Me.CheckBox3.Name = "CheckBox3"
        Me.CheckBox3.UseVisualStyleBackColor = True
        '
        'Label25
        '
        Me.Label25.AccessibleDescription = Nothing
        Me.Label25.AccessibleName = Nothing
        resources.ApplyResources(Me.Label25, "Label25")
        Me.Label25.Font = Nothing
        Me.Label25.Name = "Label25"
        '
        'CheckPostCtrlEnter
        '
        Me.CheckPostCtrlEnter.AccessibleDescription = Nothing
        Me.CheckPostCtrlEnter.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckPostCtrlEnter, "CheckPostCtrlEnter")
        Me.CheckPostCtrlEnter.BackgroundImage = Nothing
        Me.CheckPostCtrlEnter.Font = Nothing
        Me.CheckPostCtrlEnter.Name = "CheckPostCtrlEnter"
        Me.CheckPostCtrlEnter.UseVisualStyleBackColor = True
        '
        'Label27
        '
        Me.Label27.AccessibleDescription = Nothing
        Me.Label27.AccessibleName = Nothing
        resources.ApplyResources(Me.Label27, "Label27")
        Me.Label27.Font = Nothing
        Me.Label27.Name = "Label27"
        '
        'TextBox3
        '
        Me.TextBox3.AccessibleDescription = Nothing
        Me.TextBox3.AccessibleName = Nothing
        resources.ApplyResources(Me.TextBox3, "TextBox3")
        Me.TextBox3.BackgroundImage = Nothing
        Me.TextBox3.Font = Nothing
        Me.TextBox3.Name = "TextBox3"
        '
        'IconSize
        '
        Me.IconSize.AccessibleDescription = Nothing
        Me.IconSize.AccessibleName = Nothing
        resources.ApplyResources(Me.IconSize, "IconSize")
        Me.IconSize.BackgroundImage = Nothing
        Me.IconSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.IconSize.Font = Nothing
        Me.IconSize.FormattingEnabled = True
        Me.IconSize.Items.AddRange(New Object() {resources.GetString("IconSize.Items"), resources.GetString("IconSize.Items1"), resources.GetString("IconSize.Items2"), resources.GetString("IconSize.Items3"), resources.GetString("IconSize.Items4")})
        Me.IconSize.Name = "IconSize"
        '
        'Label38
        '
        Me.Label38.AccessibleDescription = Nothing
        Me.Label38.AccessibleName = Nothing
        resources.ApplyResources(Me.Label38, "Label38")
        Me.Label38.Font = Nothing
        Me.Label38.Name = "Label38"
        '
        'UReadMng
        '
        Me.UReadMng.AccessibleDescription = Nothing
        Me.UReadMng.AccessibleName = Nothing
        resources.ApplyResources(Me.UReadMng, "UReadMng")
        Me.UReadMng.BackgroundImage = Nothing
        Me.UReadMng.Font = Nothing
        Me.UReadMng.Name = "UReadMng"
        Me.UReadMng.UseVisualStyleBackColor = True
        '
        'Label39
        '
        Me.Label39.AccessibleDescription = Nothing
        Me.Label39.AccessibleName = Nothing
        resources.ApplyResources(Me.Label39, "Label39")
        Me.Label39.Font = Nothing
        Me.Label39.Name = "Label39"
        '
        'CheckReadOldPosts
        '
        Me.CheckReadOldPosts.AccessibleDescription = Nothing
        Me.CheckReadOldPosts.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckReadOldPosts, "CheckReadOldPosts")
        Me.CheckReadOldPosts.BackgroundImage = Nothing
        Me.CheckReadOldPosts.Font = Nothing
        Me.CheckReadOldPosts.Name = "CheckReadOldPosts"
        Me.CheckReadOldPosts.UseVisualStyleBackColor = True
        '
        'Label40
        '
        Me.Label40.AccessibleDescription = Nothing
        Me.Label40.AccessibleName = Nothing
        resources.ApplyResources(Me.Label40, "Label40")
        Me.Label40.Font = Nothing
        Me.Label40.Name = "Label40"
        '
        'CheckCloseToExit
        '
        Me.CheckCloseToExit.AccessibleDescription = Nothing
        Me.CheckCloseToExit.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckCloseToExit, "CheckCloseToExit")
        Me.CheckCloseToExit.BackgroundImage = Nothing
        Me.CheckCloseToExit.Font = Nothing
        Me.CheckCloseToExit.Name = "CheckCloseToExit"
        Me.CheckCloseToExit.UseVisualStyleBackColor = True
        '
        'Label41
        '
        Me.Label41.AccessibleDescription = Nothing
        Me.Label41.AccessibleName = Nothing
        resources.ApplyResources(Me.Label41, "Label41")
        Me.Label41.Font = Nothing
        Me.Label41.Name = "Label41"
        '
        'CheckMinimizeToTray
        '
        Me.CheckMinimizeToTray.AccessibleDescription = Nothing
        Me.CheckMinimizeToTray.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckMinimizeToTray, "CheckMinimizeToTray")
        Me.CheckMinimizeToTray.BackgroundImage = Nothing
        Me.CheckMinimizeToTray.Font = Nothing
        Me.CheckMinimizeToTray.Name = "CheckMinimizeToTray"
        Me.CheckMinimizeToTray.UseVisualStyleBackColor = True
        '
        'BrowserPathText
        '
        Me.BrowserPathText.AccessibleDescription = Nothing
        Me.BrowserPathText.AccessibleName = Nothing
        resources.ApplyResources(Me.BrowserPathText, "BrowserPathText")
        Me.BrowserPathText.BackgroundImage = Nothing
        Me.BrowserPathText.Font = Nothing
        Me.BrowserPathText.Name = "BrowserPathText"
        '
        'Label44
        '
        Me.Label44.AccessibleDescription = Nothing
        Me.Label44.AccessibleName = Nothing
        resources.ApplyResources(Me.Label44, "Label44")
        Me.Label44.Font = Nothing
        Me.Label44.Name = "Label44"
        '
        'CheckDispUsername
        '
        Me.CheckDispUsername.AccessibleDescription = Nothing
        Me.CheckDispUsername.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckDispUsername, "CheckDispUsername")
        Me.CheckDispUsername.BackgroundImage = Nothing
        Me.CheckDispUsername.Font = Nothing
        Me.CheckDispUsername.Name = "CheckDispUsername"
        Me.CheckDispUsername.UseVisualStyleBackColor = True
        '
        'Label46
        '
        Me.Label46.AccessibleDescription = Nothing
        Me.Label46.AccessibleName = Nothing
        resources.ApplyResources(Me.Label46, "Label46")
        Me.Label46.Font = Nothing
        Me.Label46.Name = "Label46"
        '
        'Label45
        '
        Me.Label45.AccessibleDescription = Nothing
        Me.Label45.AccessibleName = Nothing
        resources.ApplyResources(Me.Label45, "Label45")
        Me.Label45.Font = Nothing
        Me.Label45.Name = "Label45"
        '
        'ComboDispTitle
        '
        Me.ComboDispTitle.AccessibleDescription = Nothing
        Me.ComboDispTitle.AccessibleName = Nothing
        resources.ApplyResources(Me.ComboDispTitle, "ComboDispTitle")
        Me.ComboDispTitle.BackgroundImage = Nothing
        Me.ComboDispTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboDispTitle.Font = Nothing
        Me.ComboDispTitle.FormattingEnabled = True
        Me.ComboDispTitle.Items.AddRange(New Object() {resources.GetString("ComboDispTitle.Items"), resources.GetString("ComboDispTitle.Items1"), resources.GetString("ComboDispTitle.Items2"), resources.GetString("ComboDispTitle.Items3"), resources.GetString("ComboDispTitle.Items4"), resources.GetString("ComboDispTitle.Items5"), resources.GetString("ComboDispTitle.Items6")})
        Me.ComboDispTitle.Name = "ComboDispTitle"
        '
        'Label47
        '
        Me.Label47.AccessibleDescription = Nothing
        Me.Label47.AccessibleName = Nothing
        resources.ApplyResources(Me.Label47, "Label47")
        Me.Label47.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label47.Font = Nothing
        Me.Label47.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label47.Name = "Label47"
        '
        'TabControl1
        '
        Me.TabControl1.AccessibleDescription = Nothing
        Me.TabControl1.AccessibleName = Nothing
        resources.ApplyResources(Me.TabControl1, "TabControl1")
        Me.TabControl1.BackgroundImage = Nothing
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Controls.Add(Me.TabPage6)
        Me.TabControl1.Font = Nothing
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.AccessibleDescription = Nothing
        Me.TabPage1.AccessibleName = Nothing
        resources.ApplyResources(Me.TabPage1, "TabPage1")
        Me.TabPage1.BackgroundImage = Nothing
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
        Me.TabPage1.Font = Nothing
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'AuthBasicRadio
        '
        Me.AuthBasicRadio.AccessibleDescription = Nothing
        Me.AuthBasicRadio.AccessibleName = Nothing
        resources.ApplyResources(Me.AuthBasicRadio, "AuthBasicRadio")
        Me.AuthBasicRadio.BackgroundImage = Nothing
        Me.AuthBasicRadio.Font = Nothing
        Me.AuthBasicRadio.Name = "AuthBasicRadio"
        Me.AuthBasicRadio.TabStop = True
        Me.AuthBasicRadio.UseVisualStyleBackColor = True
        '
        'AuthOAuthRadio
        '
        Me.AuthOAuthRadio.AccessibleDescription = Nothing
        Me.AuthOAuthRadio.AccessibleName = Nothing
        resources.ApplyResources(Me.AuthOAuthRadio, "AuthOAuthRadio")
        Me.AuthOAuthRadio.BackgroundImage = Nothing
        Me.AuthOAuthRadio.Font = Nothing
        Me.AuthOAuthRadio.Name = "AuthOAuthRadio"
        Me.AuthOAuthRadio.TabStop = True
        Me.AuthOAuthRadio.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AccessibleDescription = Nothing
        Me.Label6.AccessibleName = Nothing
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Font = Nothing
        Me.Label6.Name = "Label6"
        '
        'AuthClearButton
        '
        Me.AuthClearButton.AccessibleDescription = Nothing
        Me.AuthClearButton.AccessibleName = Nothing
        resources.ApplyResources(Me.AuthClearButton, "AuthClearButton")
        Me.AuthClearButton.BackgroundImage = Nothing
        Me.AuthClearButton.Font = Nothing
        Me.AuthClearButton.Name = "AuthClearButton"
        Me.AuthClearButton.UseVisualStyleBackColor = True
        '
        'AuthUserLabel
        '
        Me.AuthUserLabel.AccessibleDescription = Nothing
        Me.AuthUserLabel.AccessibleName = Nothing
        resources.ApplyResources(Me.AuthUserLabel, "AuthUserLabel")
        Me.AuthUserLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AuthUserLabel.Font = Nothing
        Me.AuthUserLabel.Name = "AuthUserLabel"
        '
        'AuthStateLabel
        '
        Me.AuthStateLabel.AccessibleDescription = Nothing
        Me.AuthStateLabel.AccessibleName = Nothing
        resources.ApplyResources(Me.AuthStateLabel, "AuthStateLabel")
        Me.AuthStateLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AuthStateLabel.Font = Nothing
        Me.AuthStateLabel.Name = "AuthStateLabel"
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = Nothing
        Me.Label4.AccessibleName = Nothing
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Font = Nothing
        Me.Label4.Name = "Label4"
        '
        'AuthorizeButton
        '
        Me.AuthorizeButton.AccessibleDescription = Nothing
        Me.AuthorizeButton.AccessibleName = Nothing
        resources.ApplyResources(Me.AuthorizeButton, "AuthorizeButton")
        Me.AuthorizeButton.BackgroundImage = Nothing
        Me.AuthorizeButton.Font = Nothing
        Me.AuthorizeButton.Name = "AuthorizeButton"
        Me.AuthorizeButton.UseVisualStyleBackColor = True
        '
        'TextCountApiReply
        '
        Me.TextCountApiReply.AccessibleDescription = Nothing
        Me.TextCountApiReply.AccessibleName = Nothing
        resources.ApplyResources(Me.TextCountApiReply, "TextCountApiReply")
        Me.TextCountApiReply.BackgroundImage = Nothing
        Me.TextCountApiReply.Font = Nothing
        Me.TextCountApiReply.Name = "TextCountApiReply"
        '
        'Label7
        '
        Me.Label7.AccessibleDescription = Nothing
        Me.Label7.AccessibleName = Nothing
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Font = Nothing
        Me.Label7.Name = "Label7"
        '
        'PubSearchPeriod
        '
        Me.PubSearchPeriod.AccessibleDescription = Nothing
        Me.PubSearchPeriod.AccessibleName = Nothing
        resources.ApplyResources(Me.PubSearchPeriod, "PubSearchPeriod")
        Me.PubSearchPeriod.BackgroundImage = Nothing
        Me.PubSearchPeriod.Font = Nothing
        Me.PubSearchPeriod.Name = "PubSearchPeriod"
        '
        'Label69
        '
        Me.Label69.AccessibleDescription = Nothing
        Me.Label69.AccessibleName = Nothing
        resources.ApplyResources(Me.Label69, "Label69")
        Me.Label69.Font = Nothing
        Me.Label69.Name = "Label69"
        '
        'ReplyPeriod
        '
        Me.ReplyPeriod.AccessibleDescription = Nothing
        Me.ReplyPeriod.AccessibleName = Nothing
        resources.ApplyResources(Me.ReplyPeriod, "ReplyPeriod")
        Me.ReplyPeriod.BackgroundImage = Nothing
        Me.ReplyPeriod.Font = Nothing
        Me.ReplyPeriod.Name = "ReplyPeriod"
        '
        'CheckPostAndGet
        '
        Me.CheckPostAndGet.AccessibleDescription = Nothing
        Me.CheckPostAndGet.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckPostAndGet, "CheckPostAndGet")
        Me.CheckPostAndGet.BackgroundImage = Nothing
        Me.CheckPostAndGet.Font = Nothing
        Me.CheckPostAndGet.Name = "CheckPostAndGet"
        Me.CheckPostAndGet.UseVisualStyleBackColor = True
        '
        'Label67
        '
        Me.Label67.AccessibleDescription = Nothing
        Me.Label67.AccessibleName = Nothing
        resources.ApplyResources(Me.Label67, "Label67")
        Me.Label67.Font = Nothing
        Me.Label67.Name = "Label67"
        '
        'TextCountApi
        '
        Me.TextCountApi.AccessibleDescription = Nothing
        Me.TextCountApi.AccessibleName = Nothing
        resources.ApplyResources(Me.TextCountApi, "TextCountApi")
        Me.TextCountApi.BackgroundImage = Nothing
        Me.TextCountApi.Font = Nothing
        Me.TextCountApi.Name = "TextCountApi"
        '
        'Label54
        '
        Me.Label54.AccessibleDescription = Nothing
        Me.Label54.AccessibleName = Nothing
        resources.ApplyResources(Me.Label54, "Label54")
        Me.Label54.Font = Nothing
        Me.Label54.Name = "Label54"
        '
        'CheckStartupFollowers
        '
        Me.CheckStartupFollowers.AccessibleDescription = Nothing
        Me.CheckStartupFollowers.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckStartupFollowers, "CheckStartupFollowers")
        Me.CheckStartupFollowers.BackgroundImage = Nothing
        Me.CheckStartupFollowers.Font = Nothing
        Me.CheckStartupFollowers.Name = "CheckStartupFollowers"
        Me.CheckStartupFollowers.UseVisualStyleBackColor = True
        '
        'Label51
        '
        Me.Label51.AccessibleDescription = Nothing
        Me.Label51.AccessibleName = Nothing
        resources.ApplyResources(Me.Label51, "Label51")
        Me.Label51.Font = Nothing
        Me.Label51.Name = "Label51"
        '
        'CheckStartupVersion
        '
        Me.CheckStartupVersion.AccessibleDescription = Nothing
        Me.CheckStartupVersion.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckStartupVersion, "CheckStartupVersion")
        Me.CheckStartupVersion.BackgroundImage = Nothing
        Me.CheckStartupVersion.Font = Nothing
        Me.CheckStartupVersion.Name = "CheckStartupVersion"
        Me.CheckStartupVersion.UseVisualStyleBackColor = True
        '
        'CheckPeriodAdjust
        '
        Me.CheckPeriodAdjust.AccessibleDescription = Nothing
        Me.CheckPeriodAdjust.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckPeriodAdjust, "CheckPeriodAdjust")
        Me.CheckPeriodAdjust.BackgroundImage = Nothing
        Me.CheckPeriodAdjust.Font = Nothing
        Me.CheckPeriodAdjust.Name = "CheckPeriodAdjust"
        Me.CheckPeriodAdjust.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.AccessibleDescription = Nothing
        Me.TabPage2.AccessibleName = Nothing
        resources.ApplyResources(Me.TabPage2, "TabPage2")
        Me.TabPage2.BackgroundImage = Nothing
        Me.TabPage2.Controls.Add(Me.Label82)
        Me.TabPage2.Controls.Add(Me.CheckHashSupple)
        Me.TabPage2.Controls.Add(Me.Label79)
        Me.TabPage2.Controls.Add(Me.CheckAtIdSupple)
        Me.TabPage2.Controls.Add(Me.TextBitlyPw)
        Me.TabPage2.Controls.Add(Me.Label77)
        Me.TabPage2.Controls.Add(Me.TextBitlyId)
        Me.TabPage2.Controls.Add(Me.Label76)
        Me.TabPage2.Controls.Add(Me.Label74)
        Me.TabPage2.Controls.Add(Me.chkGetFav)
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
        Me.TabPage2.Font = Nothing
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'Label82
        '
        Me.Label82.AccessibleDescription = Nothing
        Me.Label82.AccessibleName = Nothing
        resources.ApplyResources(Me.Label82, "Label82")
        Me.Label82.Font = Nothing
        Me.Label82.Name = "Label82"
        '
        'CheckHashSupple
        '
        Me.CheckHashSupple.AccessibleDescription = Nothing
        Me.CheckHashSupple.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckHashSupple, "CheckHashSupple")
        Me.CheckHashSupple.BackgroundImage = Nothing
        Me.CheckHashSupple.Font = Nothing
        Me.CheckHashSupple.Name = "CheckHashSupple"
        Me.CheckHashSupple.UseVisualStyleBackColor = True
        '
        'Label79
        '
        Me.Label79.AccessibleDescription = Nothing
        Me.Label79.AccessibleName = Nothing
        resources.ApplyResources(Me.Label79, "Label79")
        Me.Label79.Font = Nothing
        Me.Label79.Name = "Label79"
        '
        'CheckAtIdSupple
        '
        Me.CheckAtIdSupple.AccessibleDescription = Nothing
        Me.CheckAtIdSupple.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckAtIdSupple, "CheckAtIdSupple")
        Me.CheckAtIdSupple.BackgroundImage = Nothing
        Me.CheckAtIdSupple.Font = Nothing
        Me.CheckAtIdSupple.Name = "CheckAtIdSupple"
        Me.CheckAtIdSupple.UseVisualStyleBackColor = True
        '
        'TextBitlyPw
        '
        Me.TextBitlyPw.AccessibleDescription = Nothing
        Me.TextBitlyPw.AccessibleName = Nothing
        resources.ApplyResources(Me.TextBitlyPw, "TextBitlyPw")
        Me.TextBitlyPw.BackgroundImage = Nothing
        Me.TextBitlyPw.Font = Nothing
        Me.TextBitlyPw.Name = "TextBitlyPw"
        Me.TextBitlyPw.UseSystemPasswordChar = True
        '
        'Label77
        '
        Me.Label77.AccessibleDescription = Nothing
        Me.Label77.AccessibleName = Nothing
        resources.ApplyResources(Me.Label77, "Label77")
        Me.Label77.Font = Nothing
        Me.Label77.Name = "Label77"
        '
        'TextBitlyId
        '
        Me.TextBitlyId.AccessibleDescription = Nothing
        Me.TextBitlyId.AccessibleName = Nothing
        resources.ApplyResources(Me.TextBitlyId, "TextBitlyId")
        Me.TextBitlyId.BackgroundImage = Nothing
        Me.TextBitlyId.Font = Nothing
        Me.TextBitlyId.Name = "TextBitlyId"
        '
        'Label76
        '
        Me.Label76.AccessibleDescription = Nothing
        Me.Label76.AccessibleName = Nothing
        resources.ApplyResources(Me.Label76, "Label76")
        Me.Label76.Font = Nothing
        Me.Label76.Name = "Label76"
        '
        'Label74
        '
        Me.Label74.AccessibleDescription = Nothing
        Me.Label74.AccessibleName = Nothing
        resources.ApplyResources(Me.Label74, "Label74")
        Me.Label74.Font = Nothing
        Me.Label74.Name = "Label74"
        '
        'chkGetFav
        '
        Me.chkGetFav.AccessibleDescription = Nothing
        Me.chkGetFav.AccessibleName = Nothing
        resources.ApplyResources(Me.chkGetFav, "chkGetFav")
        Me.chkGetFav.BackgroundImage = Nothing
        Me.chkGetFav.Font = Nothing
        Me.chkGetFav.Name = "chkGetFav"
        Me.chkGetFav.UseVisualStyleBackColor = True
        '
        'ComboBoxAutoShortUrlFirst
        '
        Me.ComboBoxAutoShortUrlFirst.AccessibleDescription = Nothing
        Me.ComboBoxAutoShortUrlFirst.AccessibleName = Nothing
        resources.ApplyResources(Me.ComboBoxAutoShortUrlFirst, "ComboBoxAutoShortUrlFirst")
        Me.ComboBoxAutoShortUrlFirst.BackgroundImage = Nothing
        Me.ComboBoxAutoShortUrlFirst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxAutoShortUrlFirst.Font = Nothing
        Me.ComboBoxAutoShortUrlFirst.FormattingEnabled = True
        Me.ComboBoxAutoShortUrlFirst.Items.AddRange(New Object() {resources.GetString("ComboBoxAutoShortUrlFirst.Items"), resources.GetString("ComboBoxAutoShortUrlFirst.Items1"), resources.GetString("ComboBoxAutoShortUrlFirst.Items2"), resources.GetString("ComboBoxAutoShortUrlFirst.Items3"), resources.GetString("ComboBoxAutoShortUrlFirst.Items4"), resources.GetString("ComboBoxAutoShortUrlFirst.Items5")})
        Me.ComboBoxAutoShortUrlFirst.Name = "ComboBoxAutoShortUrlFirst"
        '
        'Label71
        '
        Me.Label71.AccessibleDescription = Nothing
        Me.Label71.AccessibleName = Nothing
        resources.ApplyResources(Me.Label71, "Label71")
        Me.Label71.Font = Nothing
        Me.Label71.Name = "Label71"
        '
        'CheckProtectNotInclude
        '
        Me.CheckProtectNotInclude.AccessibleDescription = Nothing
        Me.CheckProtectNotInclude.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckProtectNotInclude, "CheckProtectNotInclude")
        Me.CheckProtectNotInclude.BackgroundImage = Nothing
        Me.CheckProtectNotInclude.Font = Nothing
        Me.CheckProtectNotInclude.Name = "CheckProtectNotInclude"
        Me.CheckProtectNotInclude.UseVisualStyleBackColor = True
        '
        'Label42
        '
        Me.Label42.AccessibleDescription = Nothing
        Me.Label42.AccessibleName = Nothing
        resources.ApplyResources(Me.Label42, "Label42")
        Me.Label42.Font = Nothing
        Me.Label42.Name = "Label42"
        '
        'CheckAutoConvertUrl
        '
        Me.CheckAutoConvertUrl.AccessibleDescription = Nothing
        Me.CheckAutoConvertUrl.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckAutoConvertUrl, "CheckAutoConvertUrl")
        Me.CheckAutoConvertUrl.BackgroundImage = Nothing
        Me.CheckAutoConvertUrl.Font = Nothing
        Me.CheckAutoConvertUrl.Name = "CheckAutoConvertUrl"
        Me.CheckAutoConvertUrl.UseVisualStyleBackColor = True
        '
        'Label29
        '
        Me.Label29.AccessibleDescription = Nothing
        Me.Label29.AccessibleName = Nothing
        resources.ApplyResources(Me.Label29, "Label29")
        Me.Label29.Font = Nothing
        Me.Label29.Name = "Label29"
        '
        'Label57
        '
        Me.Label57.AccessibleDescription = Nothing
        Me.Label57.AccessibleName = Nothing
        resources.ApplyResources(Me.Label57, "Label57")
        Me.Label57.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label57.Font = Nothing
        Me.Label57.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label57.Name = "Label57"
        '
        'Label56
        '
        Me.Label56.AccessibleDescription = Nothing
        Me.Label56.AccessibleName = Nothing
        resources.ApplyResources(Me.Label56, "Label56")
        Me.Label56.Font = Nothing
        Me.Label56.Name = "Label56"
        '
        'CheckFavRestrict
        '
        Me.CheckFavRestrict.AccessibleDescription = Nothing
        Me.CheckFavRestrict.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckFavRestrict, "CheckFavRestrict")
        Me.CheckFavRestrict.BackgroundImage = Nothing
        Me.CheckFavRestrict.Font = Nothing
        Me.CheckFavRestrict.Name = "CheckFavRestrict"
        Me.CheckFavRestrict.UseVisualStyleBackColor = True
        '
        'CheckTinyURL
        '
        Me.CheckTinyURL.AccessibleDescription = Nothing
        Me.CheckTinyURL.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckTinyURL, "CheckTinyURL")
        Me.CheckTinyURL.BackgroundImage = Nothing
        Me.CheckTinyURL.Font = Nothing
        Me.CheckTinyURL.Name = "CheckTinyURL"
        Me.CheckTinyURL.UseVisualStyleBackColor = True
        '
        'Label50
        '
        Me.Label50.AccessibleDescription = Nothing
        Me.Label50.AccessibleName = Nothing
        resources.ApplyResources(Me.Label50, "Label50")
        Me.Label50.Font = Nothing
        Me.Label50.Name = "Label50"
        '
        'Button3
        '
        Me.Button3.AccessibleDescription = Nothing
        Me.Button3.AccessibleName = Nothing
        resources.ApplyResources(Me.Button3, "Button3")
        Me.Button3.BackgroundImage = Nothing
        Me.Button3.Font = Nothing
        Me.Button3.Name = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.AccessibleDescription = Nothing
        Me.TabPage3.AccessibleName = Nothing
        resources.ApplyResources(Me.TabPage3, "TabPage3")
        Me.TabPage3.BackgroundImage = Nothing
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
        Me.TabPage3.Controls.Add(Me.Label48)
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
        Me.TabPage3.Font = Nothing
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'Label81
        '
        Me.Label81.AccessibleDescription = Nothing
        Me.Label81.AccessibleName = Nothing
        resources.ApplyResources(Me.Label81, "Label81")
        Me.Label81.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label81.Font = Nothing
        Me.Label81.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label81.Name = "Label81"
        '
        'LanguageCombo
        '
        Me.LanguageCombo.AccessibleDescription = Nothing
        Me.LanguageCombo.AccessibleName = Nothing
        resources.ApplyResources(Me.LanguageCombo, "LanguageCombo")
        Me.LanguageCombo.BackgroundImage = Nothing
        Me.LanguageCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.LanguageCombo.Font = Nothing
        Me.LanguageCombo.FormattingEnabled = True
        Me.LanguageCombo.Items.AddRange(New Object() {resources.GetString("LanguageCombo.Items"), resources.GetString("LanguageCombo.Items1"), resources.GetString("LanguageCombo.Items2")})
        Me.LanguageCombo.Name = "LanguageCombo"
        '
        'Label13
        '
        Me.Label13.AccessibleDescription = Nothing
        Me.Label13.AccessibleName = Nothing
        resources.ApplyResources(Me.Label13, "Label13")
        Me.Label13.Font = Nothing
        Me.Label13.Name = "Label13"
        '
        'CheckAlwaysTop
        '
        Me.CheckAlwaysTop.AccessibleDescription = Nothing
        Me.CheckAlwaysTop.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckAlwaysTop, "CheckAlwaysTop")
        Me.CheckAlwaysTop.BackgroundImage = Nothing
        Me.CheckAlwaysTop.Font = Nothing
        Me.CheckAlwaysTop.Name = "CheckAlwaysTop"
        Me.CheckAlwaysTop.UseVisualStyleBackColor = True
        '
        'Label58
        '
        Me.Label58.AccessibleDescription = Nothing
        Me.Label58.AccessibleName = Nothing
        resources.ApplyResources(Me.Label58, "Label58")
        Me.Label58.Font = Nothing
        Me.Label58.Name = "Label58"
        '
        'Label21
        '
        Me.Label21.AccessibleDescription = Nothing
        Me.Label21.AccessibleName = Nothing
        resources.ApplyResources(Me.Label21, "Label21")
        Me.Label21.Font = Nothing
        Me.Label21.Name = "Label21"
        '
        'CheckSortOrderLock
        '
        Me.CheckSortOrderLock.AccessibleDescription = Nothing
        Me.CheckSortOrderLock.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckSortOrderLock, "CheckSortOrderLock")
        Me.CheckSortOrderLock.BackgroundImage = Nothing
        Me.CheckSortOrderLock.Font = Nothing
        Me.CheckSortOrderLock.Name = "CheckSortOrderLock"
        Me.CheckSortOrderLock.UseVisualStyleBackColor = True
        '
        'Label78
        '
        Me.Label78.AccessibleDescription = Nothing
        Me.Label78.AccessibleName = Nothing
        resources.ApplyResources(Me.Label78, "Label78")
        Me.Label78.Font = Nothing
        Me.Label78.Name = "Label78"
        '
        'CheckShowGrid
        '
        Me.CheckShowGrid.AccessibleDescription = Nothing
        Me.CheckShowGrid.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckShowGrid, "CheckShowGrid")
        Me.CheckShowGrid.BackgroundImage = Nothing
        Me.CheckShowGrid.Font = Nothing
        Me.CheckShowGrid.Name = "CheckShowGrid"
        Me.CheckShowGrid.UseVisualStyleBackColor = True
        '
        'Label75
        '
        Me.Label75.AccessibleDescription = Nothing
        Me.Label75.AccessibleName = Nothing
        resources.ApplyResources(Me.Label75, "Label75")
        Me.Label75.Font = Nothing
        Me.Label75.Name = "Label75"
        '
        'CheckMonospace
        '
        Me.CheckMonospace.AccessibleDescription = Nothing
        Me.CheckMonospace.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckMonospace, "CheckMonospace")
        Me.CheckMonospace.BackgroundImage = Nothing
        Me.CheckMonospace.Font = Nothing
        Me.CheckMonospace.Name = "CheckMonospace"
        Me.CheckMonospace.UseVisualStyleBackColor = True
        '
        'Label73
        '
        Me.Label73.AccessibleDescription = Nothing
        Me.Label73.AccessibleName = Nothing
        resources.ApplyResources(Me.Label73, "Label73")
        Me.Label73.Font = Nothing
        Me.Label73.Name = "Label73"
        '
        'chkReadOwnPost
        '
        Me.chkReadOwnPost.AccessibleDescription = Nothing
        Me.chkReadOwnPost.AccessibleName = Nothing
        resources.ApplyResources(Me.chkReadOwnPost, "chkReadOwnPost")
        Me.chkReadOwnPost.BackgroundImage = Nothing
        Me.chkReadOwnPost.Font = Nothing
        Me.chkReadOwnPost.Name = "chkReadOwnPost"
        Me.chkReadOwnPost.UseVisualStyleBackColor = True
        '
        'ReplyIconStateCombo
        '
        Me.ReplyIconStateCombo.AccessibleDescription = Nothing
        Me.ReplyIconStateCombo.AccessibleName = Nothing
        resources.ApplyResources(Me.ReplyIconStateCombo, "ReplyIconStateCombo")
        Me.ReplyIconStateCombo.BackgroundImage = Nothing
        Me.ReplyIconStateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ReplyIconStateCombo.Font = Nothing
        Me.ReplyIconStateCombo.FormattingEnabled = True
        Me.ReplyIconStateCombo.Items.AddRange(New Object() {resources.GetString("ReplyIconStateCombo.Items"), resources.GetString("ReplyIconStateCombo.Items1"), resources.GetString("ReplyIconStateCombo.Items2")})
        Me.ReplyIconStateCombo.Name = "ReplyIconStateCombo"
        '
        'Label72
        '
        Me.Label72.AccessibleDescription = Nothing
        Me.Label72.AccessibleName = Nothing
        resources.ApplyResources(Me.Label72, "Label72")
        Me.Label72.Font = Nothing
        Me.Label72.Name = "Label72"
        '
        'Label48
        '
        Me.Label48.AccessibleDescription = Nothing
        Me.Label48.AccessibleName = Nothing
        resources.ApplyResources(Me.Label48, "Label48")
        Me.Label48.Font = Nothing
        Me.Label48.Name = "Label48"
        '
        'chkTabIconDisp
        '
        Me.chkTabIconDisp.AccessibleDescription = Nothing
        Me.chkTabIconDisp.AccessibleName = Nothing
        resources.ApplyResources(Me.chkTabIconDisp, "chkTabIconDisp")
        Me.chkTabIconDisp.BackgroundImage = Nothing
        Me.chkTabIconDisp.Font = Nothing
        Me.chkTabIconDisp.Name = "chkTabIconDisp"
        Me.chkTabIconDisp.UseVisualStyleBackColor = True
        '
        'Label68
        '
        Me.Label68.AccessibleDescription = Nothing
        Me.Label68.AccessibleName = Nothing
        resources.ApplyResources(Me.Label68, "Label68")
        Me.Label68.Font = Nothing
        Me.Label68.Name = "Label68"
        '
        'CheckBalloonLimit
        '
        Me.CheckBalloonLimit.AccessibleDescription = Nothing
        Me.CheckBalloonLimit.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckBalloonLimit, "CheckBalloonLimit")
        Me.CheckBalloonLimit.BackgroundImage = Nothing
        Me.CheckBalloonLimit.Font = Nothing
        Me.CheckBalloonLimit.Name = "CheckBalloonLimit"
        Me.CheckBalloonLimit.UseVisualStyleBackColor = True
        '
        'LabelDateTimeFormatApplied
        '
        Me.LabelDateTimeFormatApplied.AccessibleDescription = Nothing
        Me.LabelDateTimeFormatApplied.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelDateTimeFormatApplied, "LabelDateTimeFormatApplied")
        Me.LabelDateTimeFormatApplied.Font = Nothing
        Me.LabelDateTimeFormatApplied.Name = "LabelDateTimeFormatApplied"
        '
        'Label62
        '
        Me.Label62.AccessibleDescription = Nothing
        Me.Label62.AccessibleName = Nothing
        resources.ApplyResources(Me.Label62, "Label62")
        Me.Label62.Font = Nothing
        Me.Label62.Name = "Label62"
        '
        'Label17
        '
        Me.Label17.AccessibleDescription = Nothing
        Me.Label17.AccessibleName = Nothing
        resources.ApplyResources(Me.Label17, "Label17")
        Me.Label17.Font = Nothing
        Me.Label17.Name = "Label17"
        '
        'chkUnreadStyle
        '
        Me.chkUnreadStyle.AccessibleDescription = Nothing
        Me.chkUnreadStyle.AccessibleName = Nothing
        resources.ApplyResources(Me.chkUnreadStyle, "chkUnreadStyle")
        Me.chkUnreadStyle.BackgroundImage = Nothing
        Me.chkUnreadStyle.Font = Nothing
        Me.chkUnreadStyle.Name = "chkUnreadStyle"
        Me.chkUnreadStyle.UseVisualStyleBackColor = True
        '
        'TabPage4
        '
        Me.TabPage4.AccessibleDescription = Nothing
        Me.TabPage4.AccessibleName = Nothing
        resources.ApplyResources(Me.TabPage4, "TabPage4")
        Me.TabPage4.BackgroundImage = Nothing
        Me.TabPage4.Controls.Add(Me.GroupBox1)
        Me.TabPage4.Font = Nothing
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'TabPage5
        '
        Me.TabPage5.AccessibleDescription = Nothing
        Me.TabPage5.AccessibleName = Nothing
        resources.ApplyResources(Me.TabPage5, "TabPage5")
        Me.TabPage5.BackgroundImage = Nothing
        Me.TabPage5.Controls.Add(Me.TwitterSearchAPIText)
        Me.TabPage5.Controls.Add(Me.Label31)
        Me.TabPage5.Controls.Add(Me.TwitterAPIText)
        Me.TabPage5.Controls.Add(Me.Label8)
        Me.TabPage5.Controls.Add(Me.CheckUseSsl)
        Me.TabPage5.Controls.Add(Me.Label64)
        Me.TabPage5.Controls.Add(Me.ConnectionTimeOut)
        Me.TabPage5.Controls.Add(Me.Label63)
        Me.TabPage5.Controls.Add(Me.GroupBox2)
        Me.TabPage5.Font = Nothing
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'TwitterSearchAPIText
        '
        Me.TwitterSearchAPIText.AccessibleDescription = Nothing
        Me.TwitterSearchAPIText.AccessibleName = Nothing
        resources.ApplyResources(Me.TwitterSearchAPIText, "TwitterSearchAPIText")
        Me.TwitterSearchAPIText.BackgroundImage = Nothing
        Me.TwitterSearchAPIText.Font = Nothing
        Me.TwitterSearchAPIText.Name = "TwitterSearchAPIText"
        '
        'Label31
        '
        Me.Label31.AccessibleDescription = Nothing
        Me.Label31.AccessibleName = Nothing
        resources.ApplyResources(Me.Label31, "Label31")
        Me.Label31.Font = Nothing
        Me.Label31.Name = "Label31"
        '
        'TwitterAPIText
        '
        Me.TwitterAPIText.AccessibleDescription = Nothing
        Me.TwitterAPIText.AccessibleName = Nothing
        resources.ApplyResources(Me.TwitterAPIText, "TwitterAPIText")
        Me.TwitterAPIText.BackgroundImage = Nothing
        Me.TwitterAPIText.Font = Nothing
        Me.TwitterAPIText.Name = "TwitterAPIText"
        '
        'Label8
        '
        Me.Label8.AccessibleDescription = Nothing
        Me.Label8.AccessibleName = Nothing
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Font = Nothing
        Me.Label8.Name = "Label8"
        '
        'CheckUseSsl
        '
        Me.CheckUseSsl.AccessibleDescription = Nothing
        Me.CheckUseSsl.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckUseSsl, "CheckUseSsl")
        Me.CheckUseSsl.BackgroundImage = Nothing
        Me.CheckUseSsl.Font = Nothing
        Me.CheckUseSsl.Name = "CheckUseSsl"
        Me.CheckUseSsl.UseVisualStyleBackColor = True
        '
        'Label64
        '
        Me.Label64.AccessibleDescription = Nothing
        Me.Label64.AccessibleName = Nothing
        resources.ApplyResources(Me.Label64, "Label64")
        Me.Label64.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label64.Font = Nothing
        Me.Label64.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label64.Name = "Label64"
        '
        'ConnectionTimeOut
        '
        Me.ConnectionTimeOut.AccessibleDescription = Nothing
        Me.ConnectionTimeOut.AccessibleName = Nothing
        resources.ApplyResources(Me.ConnectionTimeOut, "ConnectionTimeOut")
        Me.ConnectionTimeOut.BackgroundImage = Nothing
        Me.ConnectionTimeOut.Font = Nothing
        Me.ConnectionTimeOut.Name = "ConnectionTimeOut"
        '
        'Label63
        '
        Me.Label63.AccessibleDescription = Nothing
        Me.Label63.AccessibleName = Nothing
        resources.ApplyResources(Me.Label63, "Label63")
        Me.Label63.Font = Nothing
        Me.Label63.Name = "Label63"
        '
        'GroupBox2
        '
        Me.GroupBox2.AccessibleDescription = Nothing
        Me.GroupBox2.AccessibleName = Nothing
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.BackgroundImage = Nothing
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
        Me.GroupBox2.Font = Nothing
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
        '
        'Label55
        '
        Me.Label55.AccessibleDescription = Nothing
        Me.Label55.AccessibleName = Nothing
        resources.ApplyResources(Me.Label55, "Label55")
        Me.Label55.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Label55.Font = Nothing
        Me.Label55.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label55.Name = "Label55"
        '
        'TextProxyPassword
        '
        Me.TextProxyPassword.AccessibleDescription = Nothing
        Me.TextProxyPassword.AccessibleName = Nothing
        resources.ApplyResources(Me.TextProxyPassword, "TextProxyPassword")
        Me.TextProxyPassword.BackgroundImage = Nothing
        Me.TextProxyPassword.Font = Nothing
        Me.TextProxyPassword.Name = "TextProxyPassword"
        Me.TextProxyPassword.UseSystemPasswordChar = True
        '
        'LabelProxyPassword
        '
        Me.LabelProxyPassword.AccessibleDescription = Nothing
        Me.LabelProxyPassword.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelProxyPassword, "LabelProxyPassword")
        Me.LabelProxyPassword.Font = Nothing
        Me.LabelProxyPassword.Name = "LabelProxyPassword"
        '
        'TextProxyUser
        '
        Me.TextProxyUser.AccessibleDescription = Nothing
        Me.TextProxyUser.AccessibleName = Nothing
        resources.ApplyResources(Me.TextProxyUser, "TextProxyUser")
        Me.TextProxyUser.BackgroundImage = Nothing
        Me.TextProxyUser.Font = Nothing
        Me.TextProxyUser.Name = "TextProxyUser"
        '
        'LabelProxyUser
        '
        Me.LabelProxyUser.AccessibleDescription = Nothing
        Me.LabelProxyUser.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelProxyUser, "LabelProxyUser")
        Me.LabelProxyUser.Font = Nothing
        Me.LabelProxyUser.Name = "LabelProxyUser"
        '
        'TextProxyPort
        '
        Me.TextProxyPort.AccessibleDescription = Nothing
        Me.TextProxyPort.AccessibleName = Nothing
        resources.ApplyResources(Me.TextProxyPort, "TextProxyPort")
        Me.TextProxyPort.BackgroundImage = Nothing
        Me.TextProxyPort.Font = Nothing
        Me.TextProxyPort.Name = "TextProxyPort"
        '
        'LabelProxyPort
        '
        Me.LabelProxyPort.AccessibleDescription = Nothing
        Me.LabelProxyPort.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelProxyPort, "LabelProxyPort")
        Me.LabelProxyPort.Font = Nothing
        Me.LabelProxyPort.Name = "LabelProxyPort"
        '
        'TextProxyAddress
        '
        Me.TextProxyAddress.AccessibleDescription = Nothing
        Me.TextProxyAddress.AccessibleName = Nothing
        resources.ApplyResources(Me.TextProxyAddress, "TextProxyAddress")
        Me.TextProxyAddress.BackgroundImage = Nothing
        Me.TextProxyAddress.Font = Nothing
        Me.TextProxyAddress.Name = "TextProxyAddress"
        '
        'LabelProxyAddress
        '
        Me.LabelProxyAddress.AccessibleDescription = Nothing
        Me.LabelProxyAddress.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelProxyAddress, "LabelProxyAddress")
        Me.LabelProxyAddress.Font = Nothing
        Me.LabelProxyAddress.Name = "LabelProxyAddress"
        '
        'RadioProxySpecified
        '
        Me.RadioProxySpecified.AccessibleDescription = Nothing
        Me.RadioProxySpecified.AccessibleName = Nothing
        resources.ApplyResources(Me.RadioProxySpecified, "RadioProxySpecified")
        Me.RadioProxySpecified.BackgroundImage = Nothing
        Me.RadioProxySpecified.Font = Nothing
        Me.RadioProxySpecified.Name = "RadioProxySpecified"
        Me.RadioProxySpecified.UseVisualStyleBackColor = True
        '
        'RadioProxyIE
        '
        Me.RadioProxyIE.AccessibleDescription = Nothing
        Me.RadioProxyIE.AccessibleName = Nothing
        resources.ApplyResources(Me.RadioProxyIE, "RadioProxyIE")
        Me.RadioProxyIE.BackgroundImage = Nothing
        Me.RadioProxyIE.Checked = True
        Me.RadioProxyIE.Font = Nothing
        Me.RadioProxyIE.Name = "RadioProxyIE"
        Me.RadioProxyIE.TabStop = True
        Me.RadioProxyIE.UseVisualStyleBackColor = True
        '
        'RadioProxyNone
        '
        Me.RadioProxyNone.AccessibleDescription = Nothing
        Me.RadioProxyNone.AccessibleName = Nothing
        resources.ApplyResources(Me.RadioProxyNone, "RadioProxyNone")
        Me.RadioProxyNone.BackgroundImage = Nothing
        Me.RadioProxyNone.Font = Nothing
        Me.RadioProxyNone.Name = "RadioProxyNone"
        Me.RadioProxyNone.UseVisualStyleBackColor = True
        '
        'TabPage6
        '
        Me.TabPage6.AccessibleDescription = Nothing
        Me.TabPage6.AccessibleName = Nothing
        resources.ApplyResources(Me.TabPage6, "TabPage6")
        Me.TabPage6.BackgroundImage = Nothing
        Me.TabPage6.Controls.Add(Me.CheckNicoms)
        Me.TabPage6.Controls.Add(Me.Label60)
        Me.TabPage6.Controls.Add(Me.ComboBoxOutputzUrlmode)
        Me.TabPage6.Controls.Add(Me.Label59)
        Me.TabPage6.Controls.Add(Me.TextBoxOutputzKey)
        Me.TabPage6.Controls.Add(Me.CheckOutputz)
        Me.TabPage6.Font = Nothing
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'CheckNicoms
        '
        Me.CheckNicoms.AccessibleDescription = Nothing
        Me.CheckNicoms.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckNicoms, "CheckNicoms")
        Me.CheckNicoms.BackgroundImage = Nothing
        Me.CheckNicoms.Font = Nothing
        Me.CheckNicoms.Name = "CheckNicoms"
        Me.CheckNicoms.UseVisualStyleBackColor = True
        '
        'Label60
        '
        Me.Label60.AccessibleDescription = Nothing
        Me.Label60.AccessibleName = Nothing
        resources.ApplyResources(Me.Label60, "Label60")
        Me.Label60.Font = Nothing
        Me.Label60.Name = "Label60"
        '
        'ComboBoxOutputzUrlmode
        '
        Me.ComboBoxOutputzUrlmode.AccessibleDescription = Nothing
        Me.ComboBoxOutputzUrlmode.AccessibleName = Nothing
        resources.ApplyResources(Me.ComboBoxOutputzUrlmode, "ComboBoxOutputzUrlmode")
        Me.ComboBoxOutputzUrlmode.BackgroundImage = Nothing
        Me.ComboBoxOutputzUrlmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxOutputzUrlmode.Font = Nothing
        Me.ComboBoxOutputzUrlmode.FormattingEnabled = True
        Me.ComboBoxOutputzUrlmode.Items.AddRange(New Object() {resources.GetString("ComboBoxOutputzUrlmode.Items"), resources.GetString("ComboBoxOutputzUrlmode.Items1")})
        Me.ComboBoxOutputzUrlmode.Name = "ComboBoxOutputzUrlmode"
        '
        'Label59
        '
        Me.Label59.AccessibleDescription = Nothing
        Me.Label59.AccessibleName = Nothing
        resources.ApplyResources(Me.Label59, "Label59")
        Me.Label59.Font = Nothing
        Me.Label59.Name = "Label59"
        '
        'TextBoxOutputzKey
        '
        Me.TextBoxOutputzKey.AccessibleDescription = Nothing
        Me.TextBoxOutputzKey.AccessibleName = Nothing
        resources.ApplyResources(Me.TextBoxOutputzKey, "TextBoxOutputzKey")
        Me.TextBoxOutputzKey.BackgroundImage = Nothing
        Me.TextBoxOutputzKey.Font = Nothing
        Me.TextBoxOutputzKey.Name = "TextBoxOutputzKey"
        Me.TextBoxOutputzKey.UseSystemPasswordChar = True
        '
        'CheckOutputz
        '
        Me.CheckOutputz.AccessibleDescription = Nothing
        Me.CheckOutputz.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckOutputz, "CheckOutputz")
        Me.CheckOutputz.BackgroundImage = Nothing
        Me.CheckOutputz.Font = Nothing
        Me.CheckOutputz.Name = "CheckOutputz"
        Me.CheckOutputz.UseVisualStyleBackColor = True
        '
        'Setting
        '
        Me.AcceptButton = Me.Save
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Nothing
        Me.CancelButton = Me.Cancel
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.Cancel)
        Me.Controls.Add(Me.Save)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = Nothing
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
End Class
