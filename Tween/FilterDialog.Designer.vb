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
        Me.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.ButtonClose, "ButtonClose")
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.UseVisualStyleBackColor = True
        '
        'ListFilters
        '
        Me.ListFilters.FormattingEnabled = True
        resources.ApplyResources(Me.ListFilters, "ListFilters")
        Me.ListFilters.Name = "ListFilters"
        '
        'EditFilterGroup
        '
        Me.EditFilterGroup.Controls.Add(Me.Label11)
        Me.EditFilterGroup.Controls.Add(Me.GroupExclude)
        Me.EditFilterGroup.Controls.Add(Me.GroupMatch)
        Me.EditFilterGroup.Controls.Add(Me.GroupBox1)
        Me.EditFilterGroup.Controls.Add(Me.ButtonCancel)
        Me.EditFilterGroup.Controls.Add(Me.ButtonOK)
        resources.ApplyResources(Me.EditFilterGroup, "EditFilterGroup")
        Me.EditFilterGroup.Name = "EditFilterGroup"
        Me.EditFilterGroup.TabStop = False
        '
        'Label11
        '
        resources.ApplyResources(Me.Label11, "Label11")
        Me.Label11.Name = "Label11"
        '
        'GroupExclude
        '
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
        resources.ApplyResources(Me.GroupExclude, "GroupExclude")
        Me.GroupExclude.Name = "GroupExclude"
        Me.GroupExclude.TabStop = False
        '
        'CheckExCaseSensitive
        '
        resources.ApplyResources(Me.CheckExCaseSensitive, "CheckExCaseSensitive")
        Me.CheckExCaseSensitive.Name = "CheckExCaseSensitive"
        Me.CheckExCaseSensitive.UseVisualStyleBackColor = True
        '
        'RadioExAnd
        '
        resources.ApplyResources(Me.RadioExAnd, "RadioExAnd")
        Me.RadioExAnd.Checked = True
        Me.RadioExAnd.Name = "RadioExAnd"
        Me.RadioExAnd.TabStop = True
        Me.RadioExAnd.UseVisualStyleBackColor = True
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'CheckExURL
        '
        resources.ApplyResources(Me.CheckExURL, "CheckExURL")
        Me.CheckExURL.Name = "CheckExURL"
        Me.CheckExURL.UseVisualStyleBackColor = True
        '
        'RadioExPLUS
        '
        resources.ApplyResources(Me.RadioExPLUS, "RadioExPLUS")
        Me.RadioExPLUS.Name = "RadioExPLUS"
        Me.RadioExPLUS.UseVisualStyleBackColor = True
        '
        'CheckExRegex
        '
        resources.ApplyResources(Me.CheckExRegex, "CheckExRegex")
        Me.CheckExRegex.Name = "CheckExRegex"
        Me.CheckExRegex.UseVisualStyleBackColor = True
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'ExUID
        '
        resources.ApplyResources(Me.ExUID, "ExUID")
        Me.ExUID.Name = "ExUID"
        '
        'ExMSG1
        '
        resources.ApplyResources(Me.ExMSG1, "ExMSG1")
        Me.ExMSG1.Name = "ExMSG1"
        '
        'ExMSG2
        '
        resources.ApplyResources(Me.ExMSG2, "ExMSG2")
        Me.ExMSG2.Name = "ExMSG2"
        '
        'GroupMatch
        '
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
        resources.ApplyResources(Me.GroupMatch, "GroupMatch")
        Me.GroupMatch.Name = "GroupMatch"
        Me.GroupMatch.TabStop = False
        '
        'CheckCaseSensitive
        '
        resources.ApplyResources(Me.CheckCaseSensitive, "CheckCaseSensitive")
        Me.CheckCaseSensitive.Name = "CheckCaseSensitive"
        Me.CheckCaseSensitive.UseVisualStyleBackColor = True
        '
        'RadioAND
        '
        resources.ApplyResources(Me.RadioAND, "RadioAND")
        Me.RadioAND.Checked = True
        Me.RadioAND.Name = "RadioAND"
        Me.RadioAND.TabStop = True
        Me.RadioAND.UseVisualStyleBackColor = True
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'CheckURL
        '
        resources.ApplyResources(Me.CheckURL, "CheckURL")
        Me.CheckURL.Name = "CheckURL"
        Me.CheckURL.UseVisualStyleBackColor = True
        '
        'RadioPLUS
        '
        resources.ApplyResources(Me.RadioPLUS, "RadioPLUS")
        Me.RadioPLUS.Name = "RadioPLUS"
        Me.RadioPLUS.UseVisualStyleBackColor = True
        '
        'CheckRegex
        '
        resources.ApplyResources(Me.CheckRegex, "CheckRegex")
        Me.CheckRegex.Name = "CheckRegex"
        Me.CheckRegex.UseVisualStyleBackColor = True
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        '
        'Label7
        '
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Name = "Label7"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'UID
        '
        resources.ApplyResources(Me.UID, "UID")
        Me.UID.Name = "UID"
        '
        'MSG1
        '
        resources.ApplyResources(Me.MSG1, "MSG1")
        Me.MSG1.Name = "MSG1"
        '
        'MSG2
        '
        resources.ApplyResources(Me.MSG2, "MSG2")
        Me.MSG2.Name = "MSG2"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.CheckMark)
        Me.GroupBox1.Controls.Add(Me.OptCopy)
        Me.GroupBox1.Controls.Add(Me.OptMove)
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'CheckMark
        '
        resources.ApplyResources(Me.CheckMark, "CheckMark")
        Me.CheckMark.Name = "CheckMark"
        Me.CheckMark.UseVisualStyleBackColor = True
        '
        'OptCopy
        '
        resources.ApplyResources(Me.OptCopy, "OptCopy")
        Me.OptCopy.Name = "OptCopy"
        Me.OptCopy.TabStop = True
        Me.OptCopy.UseVisualStyleBackColor = True
        '
        'OptMove
        '
        resources.ApplyResources(Me.OptMove, "OptMove")
        Me.OptMove.Name = "OptMove"
        Me.OptMove.TabStop = True
        Me.OptMove.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        resources.ApplyResources(Me.ButtonCancel, "ButtonCancel")
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'ButtonOK
        '
        resources.ApplyResources(Me.ButtonOK, "ButtonOK")
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonNew
        '
        resources.ApplyResources(Me.ButtonNew, "ButtonNew")
        Me.ButtonNew.Name = "ButtonNew"
        Me.ButtonNew.UseVisualStyleBackColor = True
        '
        'ButtonDelete
        '
        resources.ApplyResources(Me.ButtonDelete, "ButtonDelete")
        Me.ButtonDelete.Name = "ButtonDelete"
        Me.ButtonDelete.UseVisualStyleBackColor = True
        '
        'ButtonEdit
        '
        resources.ApplyResources(Me.ButtonEdit, "ButtonEdit")
        Me.ButtonEdit.Name = "ButtonEdit"
        Me.ButtonEdit.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.ListFilters)
        Me.GroupBox2.Controls.Add(Me.ButtonEdit)
        Me.GroupBox2.Controls.Add(Me.ButtonDelete)
        Me.GroupBox2.Controls.Add(Me.ButtonNew)
        Me.GroupBox2.Controls.Add(Me.EditFilterGroup)
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
        '
        'ListTabs
        '
        Me.ListTabs.FormattingEnabled = True
        resources.ApplyResources(Me.ListTabs, "ListTabs")
        Me.ListTabs.Name = "ListTabs"
        '
        'ButtonAddTab
        '
        resources.ApplyResources(Me.ButtonAddTab, "ButtonAddTab")
        Me.ButtonAddTab.Name = "ButtonAddTab"
        Me.ButtonAddTab.UseVisualStyleBackColor = True
        '
        'ButtonDeleteTab
        '
        resources.ApplyResources(Me.ButtonDeleteTab, "ButtonDeleteTab")
        Me.ButtonDeleteTab.Name = "ButtonDeleteTab"
        Me.ButtonDeleteTab.UseVisualStyleBackColor = True
        '
        'ButtonRenameTab
        '
        resources.ApplyResources(Me.ButtonRenameTab, "ButtonRenameTab")
        Me.ButtonRenameTab.Name = "ButtonRenameTab"
        Me.ButtonRenameTab.UseVisualStyleBackColor = True
        '
        'CheckManageRead
        '
        resources.ApplyResources(Me.CheckManageRead, "CheckManageRead")
        Me.CheckManageRead.Name = "CheckManageRead"
        Me.CheckManageRead.UseVisualStyleBackColor = True
        '
        'CheckNotifyNew
        '
        resources.ApplyResources(Me.CheckNotifyNew, "CheckNotifyNew")
        Me.CheckNotifyNew.Name = "CheckNotifyNew"
        Me.CheckNotifyNew.UseVisualStyleBackColor = True
        '
        'ComboSound
        '
        Me.ComboSound.FormattingEnabled = True
        resources.ApplyResources(Me.ComboSound, "ComboSound")
        Me.ComboSound.Name = "ComboSound"
        '
        'Label10
        '
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Name = "Label10"
        '
        'ButtonUp
        '
        resources.ApplyResources(Me.ButtonUp, "ButtonUp")
        Me.ButtonUp.Name = "ButtonUp"
        Me.ButtonUp.UseVisualStyleBackColor = True
        '
        'ButtonDown
        '
        resources.ApplyResources(Me.ButtonDown, "ButtonDown")
        Me.ButtonDown.Name = "ButtonDown"
        Me.ButtonDown.UseVisualStyleBackColor = True
        '
        'GroupTab
        '
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
        resources.ApplyResources(Me.GroupTab, "GroupTab")
        Me.GroupTab.Name = "GroupTab"
        Me.GroupTab.TabStop = False
        '
        'FilterDialog
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonClose
        Me.ControlBox = False
        Me.Controls.Add(Me.GroupTab)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.ButtonClose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
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

End Class
