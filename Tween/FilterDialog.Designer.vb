Option Strict On
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FilterDialog
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FilterDialog))
        Me.ButtonClose = New System.Windows.Forms.Button
        Me.ListFilters = New System.Windows.Forms.ListBox
        Me.EditFilterGroup = New System.Windows.Forms.GroupBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.GroupExclude = New System.Windows.Forms.GroupBox
        Me.TextExSource = New System.Windows.Forms.TextBox
        Me.Label12 = New System.Windows.Forms.Label
        Me.CheckExRetweet = New System.Windows.Forms.CheckBox
        Me.CheckExCaseSensitive = New System.Windows.Forms.CheckBox
        Me.RadioExAnd = New System.Windows.Forms.RadioButton
        Me.Label1 = New System.Windows.Forms.Label
        Me.CheckExURL = New System.Windows.Forms.CheckBox
        Me.RadioExPLUS = New System.Windows.Forms.RadioButton
        Me.CheckExRegex = New System.Windows.Forms.CheckBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.ExUID = New System.Windows.Forms.TextBox
        Me.ExMSG1 = New System.Windows.Forms.TextBox
        Me.ExMSG2 = New System.Windows.Forms.TextBox
        Me.GroupMatch = New System.Windows.Forms.GroupBox
        Me.TextSource = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.CheckRetweet = New System.Windows.Forms.CheckBox
        Me.CheckCaseSensitive = New System.Windows.Forms.CheckBox
        Me.RadioAND = New System.Windows.Forms.RadioButton
        Me.Label8 = New System.Windows.Forms.Label
        Me.CheckURL = New System.Windows.Forms.CheckBox
        Me.RadioPLUS = New System.Windows.Forms.RadioButton
        Me.CheckRegex = New System.Windows.Forms.CheckBox
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.UID = New System.Windows.Forms.TextBox
        Me.MSG1 = New System.Windows.Forms.TextBox
        Me.MSG2 = New System.Windows.Forms.TextBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.CheckMark = New System.Windows.Forms.CheckBox
        Me.OptCopy = New System.Windows.Forms.RadioButton
        Me.OptMove = New System.Windows.Forms.RadioButton
        Me.ButtonCancel = New System.Windows.Forms.Button
        Me.ButtonOK = New System.Windows.Forms.Button
        Me.ButtonNew = New System.Windows.Forms.Button
        Me.ButtonDelete = New System.Windows.Forms.Button
        Me.ButtonEdit = New System.Windows.Forms.Button
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.ListTabs = New System.Windows.Forms.ListBox
        Me.ButtonAddTab = New System.Windows.Forms.Button
        Me.ButtonDeleteTab = New System.Windows.Forms.Button
        Me.ButtonRenameTab = New System.Windows.Forms.Button
        Me.CheckManageRead = New System.Windows.Forms.CheckBox
        Me.CheckNotifyNew = New System.Windows.Forms.CheckBox
        Me.ComboSound = New System.Windows.Forms.ComboBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.ButtonUp = New System.Windows.Forms.Button
        Me.ButtonDown = New System.Windows.Forms.Button
        Me.GroupTab = New System.Windows.Forms.GroupBox
        Me.EditFilterGroup.SuspendLayout()
        Me.GroupExclude.SuspendLayout()
        Me.GroupMatch.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupTab.SuspendLayout()
        Me.SuspendLayout()
        '
        'ButtonClose
        '
        Me.ButtonClose.AccessibleDescription = Nothing
        Me.ButtonClose.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonClose, "ButtonClose")
        Me.ButtonClose.BackgroundImage = Nothing
        Me.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonClose.Font = Nothing
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.UseVisualStyleBackColor = True
        '
        'ListFilters
        '
        Me.ListFilters.AccessibleDescription = Nothing
        Me.ListFilters.AccessibleName = Nothing
        resources.ApplyResources(Me.ListFilters, "ListFilters")
        Me.ListFilters.BackgroundImage = Nothing
        Me.ListFilters.Font = Nothing
        Me.ListFilters.FormattingEnabled = True
        Me.ListFilters.Name = "ListFilters"
        '
        'EditFilterGroup
        '
        Me.EditFilterGroup.AccessibleDescription = Nothing
        Me.EditFilterGroup.AccessibleName = Nothing
        resources.ApplyResources(Me.EditFilterGroup, "EditFilterGroup")
        Me.EditFilterGroup.BackgroundImage = Nothing
        Me.EditFilterGroup.Controls.Add(Me.Label11)
        Me.EditFilterGroup.Controls.Add(Me.GroupExclude)
        Me.EditFilterGroup.Controls.Add(Me.GroupMatch)
        Me.EditFilterGroup.Controls.Add(Me.GroupBox1)
        Me.EditFilterGroup.Controls.Add(Me.ButtonCancel)
        Me.EditFilterGroup.Controls.Add(Me.ButtonOK)
        Me.EditFilterGroup.Font = Nothing
        Me.EditFilterGroup.Name = "EditFilterGroup"
        Me.EditFilterGroup.TabStop = False
        '
        'Label11
        '
        Me.Label11.AccessibleDescription = Nothing
        Me.Label11.AccessibleName = Nothing
        resources.ApplyResources(Me.Label11, "Label11")
        Me.Label11.Font = Nothing
        Me.Label11.Name = "Label11"
        '
        'GroupExclude
        '
        Me.GroupExclude.AccessibleDescription = Nothing
        Me.GroupExclude.AccessibleName = Nothing
        resources.ApplyResources(Me.GroupExclude, "GroupExclude")
        Me.GroupExclude.BackgroundImage = Nothing
        Me.GroupExclude.Controls.Add(Me.TextExSource)
        Me.GroupExclude.Controls.Add(Me.Label12)
        Me.GroupExclude.Controls.Add(Me.CheckExRetweet)
        Me.GroupExclude.Controls.Add(Me.CheckExCaseSensitive)
        Me.GroupExclude.Controls.Add(Me.RadioExAnd)
        Me.GroupExclude.Controls.Add(Me.Label1)
        Me.GroupExclude.Controls.Add(Me.CheckExURL)
        Me.GroupExclude.Controls.Add(Me.RadioExPLUS)
        Me.GroupExclude.Controls.Add(Me.CheckExRegex)
        Me.GroupExclude.Controls.Add(Me.Label2)
        Me.GroupExclude.Controls.Add(Me.Label3)
        Me.GroupExclude.Controls.Add(Me.Label4)
        Me.GroupExclude.Controls.Add(Me.ExUID)
        Me.GroupExclude.Controls.Add(Me.ExMSG1)
        Me.GroupExclude.Controls.Add(Me.ExMSG2)
        Me.GroupExclude.Font = Nothing
        Me.GroupExclude.Name = "GroupExclude"
        Me.GroupExclude.TabStop = False
        '
        'TextExSource
        '
        Me.TextExSource.AccessibleDescription = Nothing
        Me.TextExSource.AccessibleName = Nothing
        resources.ApplyResources(Me.TextExSource, "TextExSource")
        Me.TextExSource.BackgroundImage = Nothing
        Me.TextExSource.Font = Nothing
        Me.TextExSource.Name = "TextExSource"
        '
        'Label12
        '
        Me.Label12.AccessibleDescription = Nothing
        Me.Label12.AccessibleName = Nothing
        resources.ApplyResources(Me.Label12, "Label12")
        Me.Label12.Font = Nothing
        Me.Label12.Name = "Label12"
        '
        'CheckExRetweet
        '
        Me.CheckExRetweet.AccessibleDescription = Nothing
        Me.CheckExRetweet.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckExRetweet, "CheckExRetweet")
        Me.CheckExRetweet.BackgroundImage = Nothing
        Me.CheckExRetweet.Font = Nothing
        Me.CheckExRetweet.Name = "CheckExRetweet"
        Me.CheckExRetweet.UseVisualStyleBackColor = True
        '
        'CheckExCaseSensitive
        '
        Me.CheckExCaseSensitive.AccessibleDescription = Nothing
        Me.CheckExCaseSensitive.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckExCaseSensitive, "CheckExCaseSensitive")
        Me.CheckExCaseSensitive.BackgroundImage = Nothing
        Me.CheckExCaseSensitive.Font = Nothing
        Me.CheckExCaseSensitive.Name = "CheckExCaseSensitive"
        Me.CheckExCaseSensitive.UseVisualStyleBackColor = True
        '
        'RadioExAnd
        '
        Me.RadioExAnd.AccessibleDescription = Nothing
        Me.RadioExAnd.AccessibleName = Nothing
        resources.ApplyResources(Me.RadioExAnd, "RadioExAnd")
        Me.RadioExAnd.BackgroundImage = Nothing
        Me.RadioExAnd.Checked = True
        Me.RadioExAnd.Font = Nothing
        Me.RadioExAnd.Name = "RadioExAnd"
        Me.RadioExAnd.TabStop = True
        Me.RadioExAnd.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = Nothing
        Me.Label1.AccessibleName = Nothing
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Font = Nothing
        Me.Label1.Name = "Label1"
        '
        'CheckExURL
        '
        Me.CheckExURL.AccessibleDescription = Nothing
        Me.CheckExURL.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckExURL, "CheckExURL")
        Me.CheckExURL.BackgroundImage = Nothing
        Me.CheckExURL.Font = Nothing
        Me.CheckExURL.Name = "CheckExURL"
        Me.CheckExURL.UseVisualStyleBackColor = True
        '
        'RadioExPLUS
        '
        Me.RadioExPLUS.AccessibleDescription = Nothing
        Me.RadioExPLUS.AccessibleName = Nothing
        resources.ApplyResources(Me.RadioExPLUS, "RadioExPLUS")
        Me.RadioExPLUS.BackgroundImage = Nothing
        Me.RadioExPLUS.Font = Nothing
        Me.RadioExPLUS.Name = "RadioExPLUS"
        Me.RadioExPLUS.UseVisualStyleBackColor = True
        '
        'CheckExRegex
        '
        Me.CheckExRegex.AccessibleDescription = Nothing
        Me.CheckExRegex.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckExRegex, "CheckExRegex")
        Me.CheckExRegex.BackgroundImage = Nothing
        Me.CheckExRegex.Font = Nothing
        Me.CheckExRegex.Name = "CheckExRegex"
        Me.CheckExRegex.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = Nothing
        Me.Label2.AccessibleName = Nothing
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Font = Nothing
        Me.Label2.Name = "Label2"
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = Nothing
        Me.Label3.AccessibleName = Nothing
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Font = Nothing
        Me.Label3.Name = "Label3"
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = Nothing
        Me.Label4.AccessibleName = Nothing
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Font = Nothing
        Me.Label4.Name = "Label4"
        '
        'ExUID
        '
        Me.ExUID.AccessibleDescription = Nothing
        Me.ExUID.AccessibleName = Nothing
        resources.ApplyResources(Me.ExUID, "ExUID")
        Me.ExUID.BackgroundImage = Nothing
        Me.ExUID.Font = Nothing
        Me.ExUID.Name = "ExUID"
        '
        'ExMSG1
        '
        Me.ExMSG1.AccessibleDescription = Nothing
        Me.ExMSG1.AccessibleName = Nothing
        resources.ApplyResources(Me.ExMSG1, "ExMSG1")
        Me.ExMSG1.BackgroundImage = Nothing
        Me.ExMSG1.Font = Nothing
        Me.ExMSG1.Name = "ExMSG1"
        '
        'ExMSG2
        '
        Me.ExMSG2.AccessibleDescription = Nothing
        Me.ExMSG2.AccessibleName = Nothing
        resources.ApplyResources(Me.ExMSG2, "ExMSG2")
        Me.ExMSG2.BackgroundImage = Nothing
        Me.ExMSG2.Font = Nothing
        Me.ExMSG2.Name = "ExMSG2"
        '
        'GroupMatch
        '
        Me.GroupMatch.AccessibleDescription = Nothing
        Me.GroupMatch.AccessibleName = Nothing
        resources.ApplyResources(Me.GroupMatch, "GroupMatch")
        Me.GroupMatch.BackgroundImage = Nothing
        Me.GroupMatch.Controls.Add(Me.TextSource)
        Me.GroupMatch.Controls.Add(Me.Label5)
        Me.GroupMatch.Controls.Add(Me.CheckRetweet)
        Me.GroupMatch.Controls.Add(Me.CheckCaseSensitive)
        Me.GroupMatch.Controls.Add(Me.RadioAND)
        Me.GroupMatch.Controls.Add(Me.Label8)
        Me.GroupMatch.Controls.Add(Me.CheckURL)
        Me.GroupMatch.Controls.Add(Me.RadioPLUS)
        Me.GroupMatch.Controls.Add(Me.CheckRegex)
        Me.GroupMatch.Controls.Add(Me.Label9)
        Me.GroupMatch.Controls.Add(Me.Label7)
        Me.GroupMatch.Controls.Add(Me.Label6)
        Me.GroupMatch.Controls.Add(Me.UID)
        Me.GroupMatch.Controls.Add(Me.MSG1)
        Me.GroupMatch.Controls.Add(Me.MSG2)
        Me.GroupMatch.Font = Nothing
        Me.GroupMatch.Name = "GroupMatch"
        Me.GroupMatch.TabStop = False
        '
        'TextSource
        '
        Me.TextSource.AccessibleDescription = Nothing
        Me.TextSource.AccessibleName = Nothing
        resources.ApplyResources(Me.TextSource, "TextSource")
        Me.TextSource.BackgroundImage = Nothing
        Me.TextSource.Font = Nothing
        Me.TextSource.Name = "TextSource"
        '
        'Label5
        '
        Me.Label5.AccessibleDescription = Nothing
        Me.Label5.AccessibleName = Nothing
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Font = Nothing
        Me.Label5.Name = "Label5"
        '
        'CheckRetweet
        '
        Me.CheckRetweet.AccessibleDescription = Nothing
        Me.CheckRetweet.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckRetweet, "CheckRetweet")
        Me.CheckRetweet.BackgroundImage = Nothing
        Me.CheckRetweet.Font = Nothing
        Me.CheckRetweet.Name = "CheckRetweet"
        Me.CheckRetweet.UseVisualStyleBackColor = True
        '
        'CheckCaseSensitive
        '
        Me.CheckCaseSensitive.AccessibleDescription = Nothing
        Me.CheckCaseSensitive.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckCaseSensitive, "CheckCaseSensitive")
        Me.CheckCaseSensitive.BackgroundImage = Nothing
        Me.CheckCaseSensitive.Font = Nothing
        Me.CheckCaseSensitive.Name = "CheckCaseSensitive"
        Me.CheckCaseSensitive.UseVisualStyleBackColor = True
        '
        'RadioAND
        '
        Me.RadioAND.AccessibleDescription = Nothing
        Me.RadioAND.AccessibleName = Nothing
        resources.ApplyResources(Me.RadioAND, "RadioAND")
        Me.RadioAND.BackgroundImage = Nothing
        Me.RadioAND.Checked = True
        Me.RadioAND.Font = Nothing
        Me.RadioAND.Name = "RadioAND"
        Me.RadioAND.TabStop = True
        Me.RadioAND.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AccessibleDescription = Nothing
        Me.Label8.AccessibleName = Nothing
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Font = Nothing
        Me.Label8.Name = "Label8"
        '
        'CheckURL
        '
        Me.CheckURL.AccessibleDescription = Nothing
        Me.CheckURL.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckURL, "CheckURL")
        Me.CheckURL.BackgroundImage = Nothing
        Me.CheckURL.Font = Nothing
        Me.CheckURL.Name = "CheckURL"
        Me.CheckURL.UseVisualStyleBackColor = True
        '
        'RadioPLUS
        '
        Me.RadioPLUS.AccessibleDescription = Nothing
        Me.RadioPLUS.AccessibleName = Nothing
        resources.ApplyResources(Me.RadioPLUS, "RadioPLUS")
        Me.RadioPLUS.BackgroundImage = Nothing
        Me.RadioPLUS.Font = Nothing
        Me.RadioPLUS.Name = "RadioPLUS"
        Me.RadioPLUS.UseVisualStyleBackColor = True
        '
        'CheckRegex
        '
        Me.CheckRegex.AccessibleDescription = Nothing
        Me.CheckRegex.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckRegex, "CheckRegex")
        Me.CheckRegex.BackgroundImage = Nothing
        Me.CheckRegex.Font = Nothing
        Me.CheckRegex.Name = "CheckRegex"
        Me.CheckRegex.UseVisualStyleBackColor = True
        '
        'Label9
        '
        Me.Label9.AccessibleDescription = Nothing
        Me.Label9.AccessibleName = Nothing
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Font = Nothing
        Me.Label9.Name = "Label9"
        '
        'Label7
        '
        Me.Label7.AccessibleDescription = Nothing
        Me.Label7.AccessibleName = Nothing
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Font = Nothing
        Me.Label7.Name = "Label7"
        '
        'Label6
        '
        Me.Label6.AccessibleDescription = Nothing
        Me.Label6.AccessibleName = Nothing
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Font = Nothing
        Me.Label6.Name = "Label6"
        '
        'UID
        '
        Me.UID.AccessibleDescription = Nothing
        Me.UID.AccessibleName = Nothing
        resources.ApplyResources(Me.UID, "UID")
        Me.UID.BackgroundImage = Nothing
        Me.UID.Font = Nothing
        Me.UID.Name = "UID"
        '
        'MSG1
        '
        Me.MSG1.AccessibleDescription = Nothing
        Me.MSG1.AccessibleName = Nothing
        resources.ApplyResources(Me.MSG1, "MSG1")
        Me.MSG1.BackgroundImage = Nothing
        Me.MSG1.Font = Nothing
        Me.MSG1.Name = "MSG1"
        '
        'MSG2
        '
        Me.MSG2.AccessibleDescription = Nothing
        Me.MSG2.AccessibleName = Nothing
        resources.ApplyResources(Me.MSG2, "MSG2")
        Me.MSG2.BackgroundImage = Nothing
        Me.MSG2.Font = Nothing
        Me.MSG2.Name = "MSG2"
        '
        'GroupBox1
        '
        Me.GroupBox1.AccessibleDescription = Nothing
        Me.GroupBox1.AccessibleName = Nothing
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.BackgroundImage = Nothing
        Me.GroupBox1.Controls.Add(Me.CheckMark)
        Me.GroupBox1.Controls.Add(Me.OptCopy)
        Me.GroupBox1.Controls.Add(Me.OptMove)
        Me.GroupBox1.Font = Nothing
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'CheckMark
        '
        Me.CheckMark.AccessibleDescription = Nothing
        Me.CheckMark.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckMark, "CheckMark")
        Me.CheckMark.BackgroundImage = Nothing
        Me.CheckMark.Font = Nothing
        Me.CheckMark.Name = "CheckMark"
        Me.CheckMark.UseVisualStyleBackColor = True
        '
        'OptCopy
        '
        Me.OptCopy.AccessibleDescription = Nothing
        Me.OptCopy.AccessibleName = Nothing
        resources.ApplyResources(Me.OptCopy, "OptCopy")
        Me.OptCopy.BackgroundImage = Nothing
        Me.OptCopy.Font = Nothing
        Me.OptCopy.Name = "OptCopy"
        Me.OptCopy.TabStop = True
        Me.OptCopy.UseVisualStyleBackColor = True
        '
        'OptMove
        '
        Me.OptMove.AccessibleDescription = Nothing
        Me.OptMove.AccessibleName = Nothing
        resources.ApplyResources(Me.OptMove, "OptMove")
        Me.OptMove.BackgroundImage = Nothing
        Me.OptMove.Font = Nothing
        Me.OptMove.Name = "OptMove"
        Me.OptMove.TabStop = True
        Me.OptMove.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.AccessibleDescription = Nothing
        Me.ButtonCancel.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonCancel, "ButtonCancel")
        Me.ButtonCancel.BackgroundImage = Nothing
        Me.ButtonCancel.Font = Nothing
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'ButtonOK
        '
        Me.ButtonOK.AccessibleDescription = Nothing
        Me.ButtonOK.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonOK, "ButtonOK")
        Me.ButtonOK.BackgroundImage = Nothing
        Me.ButtonOK.Font = Nothing
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonNew
        '
        Me.ButtonNew.AccessibleDescription = Nothing
        Me.ButtonNew.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonNew, "ButtonNew")
        Me.ButtonNew.BackgroundImage = Nothing
        Me.ButtonNew.Font = Nothing
        Me.ButtonNew.Name = "ButtonNew"
        Me.ButtonNew.UseVisualStyleBackColor = True
        '
        'ButtonDelete
        '
        Me.ButtonDelete.AccessibleDescription = Nothing
        Me.ButtonDelete.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonDelete, "ButtonDelete")
        Me.ButtonDelete.BackgroundImage = Nothing
        Me.ButtonDelete.Font = Nothing
        Me.ButtonDelete.Name = "ButtonDelete"
        Me.ButtonDelete.UseVisualStyleBackColor = True
        '
        'ButtonEdit
        '
        Me.ButtonEdit.AccessibleDescription = Nothing
        Me.ButtonEdit.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonEdit, "ButtonEdit")
        Me.ButtonEdit.BackgroundImage = Nothing
        Me.ButtonEdit.Font = Nothing
        Me.ButtonEdit.Name = "ButtonEdit"
        Me.ButtonEdit.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.AccessibleDescription = Nothing
        Me.GroupBox2.AccessibleName = Nothing
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.BackgroundImage = Nothing
        Me.GroupBox2.Controls.Add(Me.ListFilters)
        Me.GroupBox2.Controls.Add(Me.ButtonEdit)
        Me.GroupBox2.Controls.Add(Me.ButtonDelete)
        Me.GroupBox2.Controls.Add(Me.ButtonNew)
        Me.GroupBox2.Controls.Add(Me.EditFilterGroup)
        Me.GroupBox2.Font = Nothing
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
        '
        'ListTabs
        '
        Me.ListTabs.AccessibleDescription = Nothing
        Me.ListTabs.AccessibleName = Nothing
        resources.ApplyResources(Me.ListTabs, "ListTabs")
        Me.ListTabs.BackgroundImage = Nothing
        Me.ListTabs.Font = Nothing
        Me.ListTabs.FormattingEnabled = True
        Me.ListTabs.Name = "ListTabs"
        '
        'ButtonAddTab
        '
        Me.ButtonAddTab.AccessibleDescription = Nothing
        Me.ButtonAddTab.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonAddTab, "ButtonAddTab")
        Me.ButtonAddTab.BackgroundImage = Nothing
        Me.ButtonAddTab.Font = Nothing
        Me.ButtonAddTab.Name = "ButtonAddTab"
        Me.ButtonAddTab.UseVisualStyleBackColor = True
        '
        'ButtonDeleteTab
        '
        Me.ButtonDeleteTab.AccessibleDescription = Nothing
        Me.ButtonDeleteTab.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonDeleteTab, "ButtonDeleteTab")
        Me.ButtonDeleteTab.BackgroundImage = Nothing
        Me.ButtonDeleteTab.Font = Nothing
        Me.ButtonDeleteTab.Name = "ButtonDeleteTab"
        Me.ButtonDeleteTab.UseVisualStyleBackColor = True
        '
        'ButtonRenameTab
        '
        Me.ButtonRenameTab.AccessibleDescription = Nothing
        Me.ButtonRenameTab.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonRenameTab, "ButtonRenameTab")
        Me.ButtonRenameTab.BackgroundImage = Nothing
        Me.ButtonRenameTab.Font = Nothing
        Me.ButtonRenameTab.Name = "ButtonRenameTab"
        Me.ButtonRenameTab.UseVisualStyleBackColor = True
        '
        'CheckManageRead
        '
        Me.CheckManageRead.AccessibleDescription = Nothing
        Me.CheckManageRead.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckManageRead, "CheckManageRead")
        Me.CheckManageRead.BackgroundImage = Nothing
        Me.CheckManageRead.Font = Nothing
        Me.CheckManageRead.Name = "CheckManageRead"
        Me.CheckManageRead.UseVisualStyleBackColor = True
        '
        'CheckNotifyNew
        '
        Me.CheckNotifyNew.AccessibleDescription = Nothing
        Me.CheckNotifyNew.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckNotifyNew, "CheckNotifyNew")
        Me.CheckNotifyNew.BackgroundImage = Nothing
        Me.CheckNotifyNew.Font = Nothing
        Me.CheckNotifyNew.Name = "CheckNotifyNew"
        Me.CheckNotifyNew.UseVisualStyleBackColor = True
        '
        'ComboSound
        '
        Me.ComboSound.AccessibleDescription = Nothing
        Me.ComboSound.AccessibleName = Nothing
        resources.ApplyResources(Me.ComboSound, "ComboSound")
        Me.ComboSound.BackgroundImage = Nothing
        Me.ComboSound.Font = Nothing
        Me.ComboSound.FormattingEnabled = True
        Me.ComboSound.Name = "ComboSound"
        '
        'Label10
        '
        Me.Label10.AccessibleDescription = Nothing
        Me.Label10.AccessibleName = Nothing
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Font = Nothing
        Me.Label10.Name = "Label10"
        '
        'ButtonUp
        '
        Me.ButtonUp.AccessibleDescription = Nothing
        Me.ButtonUp.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonUp, "ButtonUp")
        Me.ButtonUp.BackgroundImage = Nothing
        Me.ButtonUp.Name = "ButtonUp"
        Me.ButtonUp.UseVisualStyleBackColor = True
        '
        'ButtonDown
        '
        Me.ButtonDown.AccessibleDescription = Nothing
        Me.ButtonDown.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonDown, "ButtonDown")
        Me.ButtonDown.BackgroundImage = Nothing
        Me.ButtonDown.Name = "ButtonDown"
        Me.ButtonDown.UseVisualStyleBackColor = True
        '
        'GroupTab
        '
        Me.GroupTab.AccessibleDescription = Nothing
        Me.GroupTab.AccessibleName = Nothing
        resources.ApplyResources(Me.GroupTab, "GroupTab")
        Me.GroupTab.BackgroundImage = Nothing
        Me.GroupTab.Controls.Add(Me.ListTabs)
        Me.GroupTab.Controls.Add(Me.ButtonDown)
        Me.GroupTab.Controls.Add(Me.ButtonAddTab)
        Me.GroupTab.Controls.Add(Me.ButtonUp)
        Me.GroupTab.Controls.Add(Me.ButtonDeleteTab)
        Me.GroupTab.Controls.Add(Me.Label10)
        Me.GroupTab.Controls.Add(Me.ButtonRenameTab)
        Me.GroupTab.Controls.Add(Me.ComboSound)
        Me.GroupTab.Controls.Add(Me.CheckManageRead)
        Me.GroupTab.Controls.Add(Me.CheckNotifyNew)
        Me.GroupTab.Font = Nothing
        Me.GroupTab.Name = "GroupTab"
        Me.GroupTab.TabStop = False
        '
        'FilterDialog
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Nothing
        Me.CancelButton = Me.ButtonClose
        Me.ControlBox = False
        Me.Controls.Add(Me.GroupTab)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.ButtonClose)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = Nothing
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FilterDialog"
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.EditFilterGroup.ResumeLayout(False)
        Me.GroupExclude.ResumeLayout(False)
        Me.GroupExclude.PerformLayout()
        Me.GroupMatch.ResumeLayout(False)
        Me.GroupMatch.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupTab.ResumeLayout(False)
        Me.GroupTab.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ButtonClose As System.Windows.Forms.Button
    Friend WithEvents ListFilters As System.Windows.Forms.ListBox
    Friend WithEvents EditFilterGroup As System.Windows.Forms.GroupBox
    Friend WithEvents RadioPLUS As System.Windows.Forms.RadioButton
    Friend WithEvents RadioAND As System.Windows.Forms.RadioButton
    Friend WithEvents MSG2 As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents MSG1 As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button
    Friend WithEvents ButtonOK As System.Windows.Forms.Button
    Friend WithEvents UID As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents ButtonNew As System.Windows.Forms.Button
    Friend WithEvents ButtonDelete As System.Windows.Forms.Button
    Friend WithEvents ButtonEdit As System.Windows.Forms.Button
    Friend WithEvents CheckURL As System.Windows.Forms.CheckBox
    Friend WithEvents CheckRegex As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents OptMove As System.Windows.Forms.RadioButton
    Friend WithEvents OptCopy As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents ListTabs As System.Windows.Forms.ListBox
    Friend WithEvents GroupMatch As System.Windows.Forms.GroupBox
    Friend WithEvents GroupExclude As System.Windows.Forms.GroupBox
    Friend WithEvents RadioExAnd As System.Windows.Forms.RadioButton
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents CheckExURL As System.Windows.Forms.CheckBox
    Friend WithEvents RadioExPLUS As System.Windows.Forms.RadioButton
    Friend WithEvents CheckExRegex As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ExUID As System.Windows.Forms.TextBox
    Friend WithEvents ExMSG1 As System.Windows.Forms.TextBox
    Friend WithEvents ExMSG2 As System.Windows.Forms.TextBox
    Friend WithEvents CheckMark As System.Windows.Forms.CheckBox
    Friend WithEvents ButtonAddTab As System.Windows.Forms.Button
    Friend WithEvents ButtonDeleteTab As System.Windows.Forms.Button
    Friend WithEvents ButtonRenameTab As System.Windows.Forms.Button
    Friend WithEvents CheckManageRead As System.Windows.Forms.CheckBox
    Friend WithEvents CheckNotifyNew As System.Windows.Forms.CheckBox
    Friend WithEvents ComboSound As System.Windows.Forms.ComboBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents ButtonUp As System.Windows.Forms.Button
    Friend WithEvents ButtonDown As System.Windows.Forms.Button
    Friend WithEvents GroupTab As System.Windows.Forms.GroupBox
    Friend WithEvents CheckExCaseSensitive As System.Windows.Forms.CheckBox
    Friend WithEvents CheckCaseSensitive As System.Windows.Forms.CheckBox
    Friend WithEvents CheckRetweet As System.Windows.Forms.CheckBox
    Friend WithEvents TextExSource As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents CheckExRetweet As System.Windows.Forms.CheckBox
    Friend WithEvents TextSource As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label

End Class
