<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HashtagManage
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HashtagManage))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.Close_Button = New System.Windows.Forms.Button
        Me.DeleteButton = New System.Windows.Forms.Button
        Me.ReplaceButton = New System.Windows.Forms.Button
        Me.AddButton = New System.Windows.Forms.Button
        Me.HistoryHashList = New System.Windows.Forms.ListBox
        Me.UseHashText = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.CheckPermanent = New System.Windows.Forms.CheckBox
        Me.GroupPermanent = New System.Windows.Forms.GroupBox
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel
        Me.PermOK_Button = New System.Windows.Forms.Button
        Me.PermCancel_Button = New System.Windows.Forms.Button
        Me.Label3 = New System.Windows.Forms.Label
        Me.RadioLast = New System.Windows.Forms.RadioButton
        Me.RadioHead = New System.Windows.Forms.RadioButton
        Me.Label2 = New System.Windows.Forms.Label
        Me.InsertButton = New System.Windows.Forms.Button
        Me.EditButton = New System.Windows.Forms.Button
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupPermanent.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AccessibleDescription = Nothing
        Me.TableLayoutPanel1.AccessibleName = Nothing
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.BackgroundImage = Nothing
        Me.TableLayoutPanel1.Controls.Add(Me.Close_Button, 0, 0)
        Me.TableLayoutPanel1.Font = Nothing
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'Close_Button
        '
        Me.Close_Button.AccessibleDescription = Nothing
        Me.Close_Button.AccessibleName = Nothing
        resources.ApplyResources(Me.Close_Button, "Close_Button")
        Me.Close_Button.BackgroundImage = Nothing
        Me.Close_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close_Button.Font = Nothing
        Me.Close_Button.Name = "Close_Button"
        '
        'DeleteButton
        '
        Me.DeleteButton.AccessibleDescription = Nothing
        Me.DeleteButton.AccessibleName = Nothing
        resources.ApplyResources(Me.DeleteButton, "DeleteButton")
        Me.DeleteButton.BackgroundImage = Nothing
        Me.DeleteButton.Font = Nothing
        Me.DeleteButton.Name = "DeleteButton"
        Me.DeleteButton.UseVisualStyleBackColor = True
        '
        'ReplaceButton
        '
        Me.ReplaceButton.AccessibleDescription = Nothing
        Me.ReplaceButton.AccessibleName = Nothing
        resources.ApplyResources(Me.ReplaceButton, "ReplaceButton")
        Me.ReplaceButton.BackgroundImage = Nothing
        Me.ReplaceButton.Font = Nothing
        Me.ReplaceButton.Name = "ReplaceButton"
        Me.ReplaceButton.UseVisualStyleBackColor = True
        '
        'AddButton
        '
        Me.AddButton.AccessibleDescription = Nothing
        Me.AddButton.AccessibleName = Nothing
        resources.ApplyResources(Me.AddButton, "AddButton")
        Me.AddButton.BackgroundImage = Nothing
        Me.AddButton.Font = Nothing
        Me.AddButton.Name = "AddButton"
        Me.AddButton.UseVisualStyleBackColor = True
        '
        'HistoryHashList
        '
        Me.HistoryHashList.AccessibleDescription = Nothing
        Me.HistoryHashList.AccessibleName = Nothing
        resources.ApplyResources(Me.HistoryHashList, "HistoryHashList")
        Me.HistoryHashList.BackgroundImage = Nothing
        Me.HistoryHashList.Font = Nothing
        Me.HistoryHashList.FormattingEnabled = True
        Me.HistoryHashList.Name = "HistoryHashList"
        Me.HistoryHashList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        '
        'UseHashText
        '
        Me.UseHashText.AccessibleDescription = Nothing
        Me.UseHashText.AccessibleName = Nothing
        resources.ApplyResources(Me.UseHashText, "UseHashText")
        Me.UseHashText.BackgroundImage = Nothing
        Me.UseHashText.Font = Nothing
        Me.UseHashText.Name = "UseHashText"
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = Nothing
        Me.Label1.AccessibleName = Nothing
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Font = Nothing
        Me.Label1.Name = "Label1"
        '
        'CheckPermanent
        '
        Me.CheckPermanent.AccessibleDescription = Nothing
        Me.CheckPermanent.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckPermanent, "CheckPermanent")
        Me.CheckPermanent.BackgroundImage = Nothing
        Me.CheckPermanent.Font = Nothing
        Me.CheckPermanent.Name = "CheckPermanent"
        Me.CheckPermanent.UseVisualStyleBackColor = True
        '
        'GroupPermanent
        '
        Me.GroupPermanent.AccessibleDescription = Nothing
        Me.GroupPermanent.AccessibleName = Nothing
        resources.ApplyResources(Me.GroupPermanent, "GroupPermanent")
        Me.GroupPermanent.BackgroundImage = Nothing
        Me.GroupPermanent.Controls.Add(Me.TableLayoutPanel2)
        Me.GroupPermanent.Controls.Add(Me.Label3)
        Me.GroupPermanent.Controls.Add(Me.RadioLast)
        Me.GroupPermanent.Controls.Add(Me.RadioHead)
        Me.GroupPermanent.Controls.Add(Me.UseHashText)
        Me.GroupPermanent.Controls.Add(Me.Label1)
        Me.GroupPermanent.Font = Nothing
        Me.GroupPermanent.Name = "GroupPermanent"
        Me.GroupPermanent.TabStop = False
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.AccessibleDescription = Nothing
        Me.TableLayoutPanel2.AccessibleName = Nothing
        resources.ApplyResources(Me.TableLayoutPanel2, "TableLayoutPanel2")
        Me.TableLayoutPanel2.BackgroundImage = Nothing
        Me.TableLayoutPanel2.Controls.Add(Me.PermOK_Button, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.PermCancel_Button, 1, 0)
        Me.TableLayoutPanel2.Font = Nothing
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        '
        'PermOK_Button
        '
        Me.PermOK_Button.AccessibleDescription = Nothing
        Me.PermOK_Button.AccessibleName = Nothing
        resources.ApplyResources(Me.PermOK_Button, "PermOK_Button")
        Me.PermOK_Button.BackgroundImage = Nothing
        Me.PermOK_Button.Font = Nothing
        Me.PermOK_Button.Name = "PermOK_Button"
        '
        'PermCancel_Button
        '
        Me.PermCancel_Button.AccessibleDescription = Nothing
        Me.PermCancel_Button.AccessibleName = Nothing
        resources.ApplyResources(Me.PermCancel_Button, "PermCancel_Button")
        Me.PermCancel_Button.BackgroundImage = Nothing
        Me.PermCancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.PermCancel_Button.Font = Nothing
        Me.PermCancel_Button.Name = "PermCancel_Button"
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = Nothing
        Me.Label3.AccessibleName = Nothing
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Font = Nothing
        Me.Label3.Name = "Label3"
        '
        'RadioLast
        '
        Me.RadioLast.AccessibleDescription = Nothing
        Me.RadioLast.AccessibleName = Nothing
        resources.ApplyResources(Me.RadioLast, "RadioLast")
        Me.RadioLast.BackgroundImage = Nothing
        Me.RadioLast.Font = Nothing
        Me.RadioLast.Name = "RadioLast"
        Me.RadioLast.TabStop = True
        Me.RadioLast.UseVisualStyleBackColor = True
        '
        'RadioHead
        '
        Me.RadioHead.AccessibleDescription = Nothing
        Me.RadioHead.AccessibleName = Nothing
        resources.ApplyResources(Me.RadioHead, "RadioHead")
        Me.RadioHead.BackgroundImage = Nothing
        Me.RadioHead.Font = Nothing
        Me.RadioHead.Name = "RadioHead"
        Me.RadioHead.TabStop = True
        Me.RadioHead.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = Nothing
        Me.Label2.AccessibleName = Nothing
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Font = Nothing
        Me.Label2.Name = "Label2"
        '
        'InsertButton
        '
        Me.InsertButton.AccessibleDescription = Nothing
        Me.InsertButton.AccessibleName = Nothing
        resources.ApplyResources(Me.InsertButton, "InsertButton")
        Me.InsertButton.BackgroundImage = Nothing
        Me.InsertButton.Font = Nothing
        Me.InsertButton.Name = "InsertButton"
        Me.InsertButton.UseVisualStyleBackColor = True
        '
        'EditButton
        '
        Me.EditButton.AccessibleDescription = Nothing
        Me.EditButton.AccessibleName = Nothing
        resources.ApplyResources(Me.EditButton, "EditButton")
        Me.EditButton.BackgroundImage = Nothing
        Me.EditButton.Font = Nothing
        Me.EditButton.Name = "EditButton"
        Me.EditButton.UseVisualStyleBackColor = True
        '
        'HashtagManage
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Nothing
        Me.Controls.Add(Me.EditButton)
        Me.Controls.Add(Me.InsertButton)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.GroupPermanent)
        Me.Controls.Add(Me.AddButton)
        Me.Controls.Add(Me.ReplaceButton)
        Me.Controls.Add(Me.CheckPermanent)
        Me.Controls.Add(Me.DeleteButton)
        Me.Controls.Add(Me.HistoryHashList)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = Nothing
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "HashtagManage"
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.GroupPermanent.ResumeLayout(False)
        Me.GroupPermanent.PerformLayout()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Close_Button As System.Windows.Forms.Button
    Friend WithEvents DeleteButton As System.Windows.Forms.Button
    Friend WithEvents ReplaceButton As System.Windows.Forms.Button
    Friend WithEvents AddButton As System.Windows.Forms.Button
    Friend WithEvents HistoryHashList As System.Windows.Forms.ListBox
    Friend WithEvents UseHashText As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents CheckPermanent As System.Windows.Forms.CheckBox
    Friend WithEvents GroupPermanent As System.Windows.Forms.GroupBox
    Friend WithEvents RadioLast As System.Windows.Forms.RadioButton
    Friend WithEvents RadioHead As System.Windows.Forms.RadioButton
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents InsertButton As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents PermOK_Button As System.Windows.Forms.Button
    Friend WithEvents PermCancel_Button As System.Windows.Forms.Button
    Friend WithEvents EditButton As System.Windows.Forms.Button

End Class
