Option Strict On
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InputTabName
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(InputTabName))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.OK_Button = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.LabelDescription = New System.Windows.Forms.Label
        Me.TextTabName = New System.Windows.Forms.TextBox
        Me.LabelUsage = New System.Windows.Forms.Label
        Me.ComboUsage = New System.Windows.Forms.ComboBox
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'OK_Button
        '
        resources.ApplyResources(Me.OK_Button, "OK_Button")
        Me.OK_Button.Name = "OK_Button"
        '
        'Cancel_Button
        '
        resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Name = "Cancel_Button"
        '
        'LabelDescription
        '
        resources.ApplyResources(Me.LabelDescription, "LabelDescription")
        Me.LabelDescription.Name = "LabelDescription"
        '
        'TextTabName
        '
        resources.ApplyResources(Me.TextTabName, "TextTabName")
        Me.TextTabName.Name = "TextTabName"
        '
        'LabelUsage
        '
        resources.ApplyResources(Me.LabelUsage, "LabelUsage")
        Me.LabelUsage.Name = "LabelUsage"
        '
        'ComboUsage
        '
        Me.ComboUsage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboUsage.FormattingEnabled = True
        resources.ApplyResources(Me.ComboUsage, "ComboUsage")
        Me.ComboUsage.Name = "ComboUsage"
        '
        'InputTabName
        '
        Me.AcceptButton = Me.OK_Button
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.Controls.Add(Me.ComboUsage)
        Me.Controls.Add(Me.LabelUsage)
        Me.Controls.Add(Me.TextTabName)
        Me.Controls.Add(Me.LabelDescription)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "InputTabName"
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents LabelDescription As System.Windows.Forms.Label
    Friend WithEvents TextTabName As System.Windows.Forms.TextBox
    Friend WithEvents LabelUsage As System.Windows.Forms.Label
    Friend WithEvents ComboUsage As System.Windows.Forms.ComboBox

End Class
