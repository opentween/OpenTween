<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ListAvailable
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ListAvailable))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.OK_Button = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.UsernameLabel = New System.Windows.Forms.Label
        Me.NameLabel = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.StatusLabel = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.MemberCountLabel = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.SubscriberCountLabel = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.DescriptionText = New System.Windows.Forms.TextBox
        Me.RefreshButton = New System.Windows.Forms.Button
        Me.ListsList = New System.Windows.Forms.ListBox
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
        'Label1
        '
        Me.Label1.AccessibleDescription = Nothing
        Me.Label1.AccessibleName = Nothing
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Font = Nothing
        Me.Label1.Name = "Label1"
        '
        'UsernameLabel
        '
        Me.UsernameLabel.AccessibleDescription = Nothing
        Me.UsernameLabel.AccessibleName = Nothing
        resources.ApplyResources(Me.UsernameLabel, "UsernameLabel")
        Me.UsernameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UsernameLabel.Font = Nothing
        Me.UsernameLabel.Name = "UsernameLabel"
        '
        'NameLabel
        '
        Me.NameLabel.AccessibleDescription = Nothing
        Me.NameLabel.AccessibleName = Nothing
        resources.ApplyResources(Me.NameLabel, "NameLabel")
        Me.NameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.NameLabel.Font = Nothing
        Me.NameLabel.Name = "NameLabel"
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = Nothing
        Me.Label4.AccessibleName = Nothing
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Font = Nothing
        Me.Label4.Name = "Label4"
        '
        'Label6
        '
        Me.Label6.AccessibleDescription = Nothing
        Me.Label6.AccessibleName = Nothing
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Font = Nothing
        Me.Label6.Name = "Label6"
        '
        'StatusLabel
        '
        Me.StatusLabel.AccessibleDescription = Nothing
        Me.StatusLabel.AccessibleName = Nothing
        resources.ApplyResources(Me.StatusLabel, "StatusLabel")
        Me.StatusLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.StatusLabel.Font = Nothing
        Me.StatusLabel.Name = "StatusLabel"
        '
        'Label8
        '
        Me.Label8.AccessibleDescription = Nothing
        Me.Label8.AccessibleName = Nothing
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Font = Nothing
        Me.Label8.Name = "Label8"
        '
        'MemberCountLabel
        '
        Me.MemberCountLabel.AccessibleDescription = Nothing
        Me.MemberCountLabel.AccessibleName = Nothing
        resources.ApplyResources(Me.MemberCountLabel, "MemberCountLabel")
        Me.MemberCountLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.MemberCountLabel.Font = Nothing
        Me.MemberCountLabel.Name = "MemberCountLabel"
        '
        'Label10
        '
        Me.Label10.AccessibleDescription = Nothing
        Me.Label10.AccessibleName = Nothing
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Font = Nothing
        Me.Label10.Name = "Label10"
        '
        'SubscriberCountLabel
        '
        Me.SubscriberCountLabel.AccessibleDescription = Nothing
        Me.SubscriberCountLabel.AccessibleName = Nothing
        resources.ApplyResources(Me.SubscriberCountLabel, "SubscriberCountLabel")
        Me.SubscriberCountLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SubscriberCountLabel.Font = Nothing
        Me.SubscriberCountLabel.Name = "SubscriberCountLabel"
        '
        'Label12
        '
        Me.Label12.AccessibleDescription = Nothing
        Me.Label12.AccessibleName = Nothing
        resources.ApplyResources(Me.Label12, "Label12")
        Me.Label12.Font = Nothing
        Me.Label12.Name = "Label12"
        '
        'DescriptionText
        '
        Me.DescriptionText.AccessibleDescription = Nothing
        Me.DescriptionText.AccessibleName = Nothing
        resources.ApplyResources(Me.DescriptionText, "DescriptionText")
        Me.DescriptionText.BackgroundImage = Nothing
        Me.DescriptionText.Font = Nothing
        Me.DescriptionText.Name = "DescriptionText"
        Me.DescriptionText.ReadOnly = True
        '
        'RefreshButton
        '
        Me.RefreshButton.AccessibleDescription = Nothing
        Me.RefreshButton.AccessibleName = Nothing
        resources.ApplyResources(Me.RefreshButton, "RefreshButton")
        Me.RefreshButton.BackgroundImage = Nothing
        Me.RefreshButton.Font = Nothing
        Me.RefreshButton.Name = "RefreshButton"
        '
        'ListsList
        '
        Me.ListsList.AccessibleDescription = Nothing
        Me.ListsList.AccessibleName = Nothing
        resources.ApplyResources(Me.ListsList, "ListsList")
        Me.ListsList.BackgroundImage = Nothing
        Me.ListsList.Font = Nothing
        Me.ListsList.FormattingEnabled = True
        Me.ListsList.Name = "ListsList"
        '
        'ListAvailable
        '
        Me.AcceptButton = Me.OK_Button
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Nothing
        Me.CancelButton = Me.Cancel_Button
        Me.Controls.Add(Me.ListsList)
        Me.Controls.Add(Me.RefreshButton)
        Me.Controls.Add(Me.DescriptionText)
        Me.Controls.Add(Me.SubscriberCountLabel)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.MemberCountLabel)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.StatusLabel)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.NameLabel)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.UsernameLabel)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ListAvailable"
        Me.ShowInTaskbar = False
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents UsernameLabel As System.Windows.Forms.Label
    Friend WithEvents NameLabel As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents StatusLabel As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents MemberCountLabel As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents SubscriberCountLabel As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents DescriptionText As System.Windows.Forms.TextBox
    Friend WithEvents RefreshButton As System.Windows.Forms.Button
    Friend WithEvents ListsList As System.Windows.Forms.ListBox

End Class
