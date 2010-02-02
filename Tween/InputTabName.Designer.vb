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
        Me.TableLayoutPanel1.AccessibleDescription = Nothing
        Me.TableLayoutPanel1.AccessibleName = Nothing
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.BackgroundImage = Nothing
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Font = Nothing
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'OK_Button
        '
        Me.OK_Button.AccessibleDescription = Nothing
        Me.OK_Button.AccessibleName = Nothing
        resources.ApplyResources(Me.OK_Button, "OK_Button")
        Me.OK_Button.BackgroundImage = Nothing
        Me.OK_Button.Font = Nothing
        Me.OK_Button.Name = "OK_Button"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.AccessibleDescription = Nothing
        Me.Cancel_Button.AccessibleName = Nothing
        resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
        Me.Cancel_Button.BackgroundImage = Nothing
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Font = Nothing
        Me.Cancel_Button.Name = "Cancel_Button"
        '
        'LabelDescription
        '
        Me.LabelDescription.AccessibleDescription = Nothing
        Me.LabelDescription.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelDescription, "LabelDescription")
        Me.LabelDescription.Font = Nothing
        Me.LabelDescription.Name = "LabelDescription"
        '
        'TextTabName
        '
        Me.TextTabName.AccessibleDescription = Nothing
        Me.TextTabName.AccessibleName = Nothing
        resources.ApplyResources(Me.TextTabName, "TextTabName")
        Me.TextTabName.BackgroundImage = Nothing
        Me.TextTabName.Font = Nothing
        Me.TextTabName.Name = "TextTabName"
        '
        'LabelUsage
        '
        Me.LabelUsage.AccessibleDescription = Nothing
        Me.LabelUsage.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelUsage, "LabelUsage")
        Me.LabelUsage.Font = Nothing
        Me.LabelUsage.Name = "LabelUsage"
        '
        'ComboUsage
        '
        Me.ComboUsage.AccessibleDescription = Nothing
        Me.ComboUsage.AccessibleName = Nothing
        resources.ApplyResources(Me.ComboUsage, "ComboUsage")
        Me.ComboUsage.BackgroundImage = Nothing
        Me.ComboUsage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboUsage.Font = Nothing
        Me.ComboUsage.FormattingEnabled = True
        Me.ComboUsage.Name = "ComboUsage"
        '
        'InputTabName
        '
        Me.AcceptButton = Me.OK_Button
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Nothing
        Me.CancelButton = Me.Cancel_Button
        Me.Controls.Add(Me.ComboUsage)
        Me.Controls.Add(Me.LabelUsage)
        Me.Controls.Add(Me.TextTabName)
        Me.Controls.Add(Me.LabelDescription)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = Nothing
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
