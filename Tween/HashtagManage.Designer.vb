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
        Me.Button1 = New System.Windows.Forms.Button
        Me.Close_Button = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupPermanent.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.Close_Button, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Button2, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.InsertButton, 0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'DeleteButton
        '
        resources.ApplyResources(Me.DeleteButton, "DeleteButton")
        Me.DeleteButton.Name = "DeleteButton"
        Me.DeleteButton.UseVisualStyleBackColor = True
        '
        'ReplaceButton
        '
        resources.ApplyResources(Me.ReplaceButton, "ReplaceButton")
        Me.ReplaceButton.Name = "ReplaceButton"
        Me.ReplaceButton.UseVisualStyleBackColor = True
        '
        'AddButton
        '
        resources.ApplyResources(Me.AddButton, "AddButton")
        Me.AddButton.Name = "AddButton"
        Me.AddButton.UseVisualStyleBackColor = True
        '
        'HistoryHashList
        '
        Me.HistoryHashList.FormattingEnabled = True
        resources.ApplyResources(Me.HistoryHashList, "HistoryHashList")
        Me.HistoryHashList.Name = "HistoryHashList"
        Me.HistoryHashList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        '
        'UseHashText
        '
        resources.ApplyResources(Me.UseHashText, "UseHashText")
        Me.UseHashText.Name = "UseHashText"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'CheckPermanent
        '
        resources.ApplyResources(Me.CheckPermanent, "CheckPermanent")
        Me.CheckPermanent.Name = "CheckPermanent"
        Me.CheckPermanent.UseVisualStyleBackColor = True
        '
        'GroupPermanent
        '
        Me.GroupPermanent.Controls.Add(Me.TableLayoutPanel2)
        Me.GroupPermanent.Controls.Add(Me.UseHashText)
        Me.GroupPermanent.Controls.Add(Me.Label1)
        resources.ApplyResources(Me.GroupPermanent, "GroupPermanent")
        Me.GroupPermanent.Name = "GroupPermanent"
        Me.GroupPermanent.TabStop = False
        '
        'TableLayoutPanel2
        '
        resources.ApplyResources(Me.TableLayoutPanel2, "TableLayoutPanel2")
        Me.TableLayoutPanel2.Controls.Add(Me.PermOK_Button, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.PermCancel_Button, 1, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        '
        'PermOK_Button
        '
        resources.ApplyResources(Me.PermOK_Button, "PermOK_Button")
        Me.PermOK_Button.Name = "PermOK_Button"
        '
        'PermCancel_Button
        '
        resources.ApplyResources(Me.PermCancel_Button, "PermCancel_Button")
        Me.PermCancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.PermCancel_Button.Name = "PermCancel_Button"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'RadioLast
        '
        resources.ApplyResources(Me.RadioLast, "RadioLast")
        Me.RadioLast.Name = "RadioLast"
        Me.RadioLast.TabStop = True
        Me.RadioLast.UseVisualStyleBackColor = True
        '
        'RadioHead
        '
        resources.ApplyResources(Me.RadioHead, "RadioHead")
        Me.RadioHead.Name = "RadioHead"
        Me.RadioHead.TabStop = True
        Me.RadioHead.UseVisualStyleBackColor = True
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'InsertButton
        '
        resources.ApplyResources(Me.InsertButton, "InsertButton")
        Me.InsertButton.Name = "InsertButton"
        Me.InsertButton.UseVisualStyleBackColor = True
        '
        'Button1
        '
        resources.ApplyResources(Me.Button1, "Button1")
        Me.Button1.Name = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Close_Button
        '
        resources.ApplyResources(Me.Close_Button, "Close_Button")
        Me.Close_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close_Button.Name = "Close_Button"
        '
        'Button2
        '
        resources.ApplyResources(Me.Button2, "Button2")
        Me.Button2.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button2.Name = "Button2"
        '
        'HashtagManage
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.RadioLast)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.RadioHead)
        Me.Controls.Add(Me.GroupPermanent)
        Me.Controls.Add(Me.AddButton)
        Me.Controls.Add(Me.ReplaceButton)
        Me.Controls.Add(Me.CheckPermanent)
        Me.Controls.Add(Me.DeleteButton)
        Me.Controls.Add(Me.HistoryHashList)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
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
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Close_Button As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button

End Class
