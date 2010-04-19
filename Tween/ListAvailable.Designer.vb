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
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(346, 253)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 27)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 21)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 21)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "キャンセル"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(302, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 12)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "作成者"
        '
        'UsernameLabel
        '
        Me.UsernameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UsernameLabel.Location = New System.Drawing.Point(318, 24)
        Me.UsernameLabel.Name = "UsernameLabel"
        Me.UsernameLabel.Size = New System.Drawing.Size(174, 14)
        Me.UsernameLabel.TabIndex = 3
        Me.UsernameLabel.Text = "Label2"
        '
        'NameLabel
        '
        Me.NameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.NameLabel.Location = New System.Drawing.Point(318, 50)
        Me.NameLabel.Name = "NameLabel"
        Me.NameLabel.Size = New System.Drawing.Size(174, 14)
        Me.NameLabel.TabIndex = 5
        Me.NameLabel.Text = "Label3"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(302, 38)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(41, 12)
        Me.Label4.TabIndex = 4
        Me.Label4.Text = "リスト名"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(302, 144)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(29, 12)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "説明"
        '
        'StatusLabel
        '
        Me.StatusLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.StatusLabel.Location = New System.Drawing.Point(318, 76)
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(174, 14)
        Me.StatusLabel.TabIndex = 9
        Me.StatusLabel.Text = "Label7"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(302, 64)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(29, 12)
        Me.Label8.TabIndex = 8
        Me.Label8.Text = "種別"
        '
        'MemberCountLabel
        '
        Me.MemberCountLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.MemberCountLabel.Location = New System.Drawing.Point(318, 102)
        Me.MemberCountLabel.Name = "MemberCountLabel"
        Me.MemberCountLabel.Size = New System.Drawing.Size(46, 14)
        Me.MemberCountLabel.TabIndex = 11
        Me.MemberCountLabel.Text = "Label9"
        Me.MemberCountLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(302, 90)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(53, 12)
        Me.Label10.TabIndex = 10
        Me.Label10.Text = "登録者数"
        '
        'SubscriberCountLabel
        '
        Me.SubscriberCountLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SubscriberCountLabel.Location = New System.Drawing.Point(318, 130)
        Me.SubscriberCountLabel.Name = "SubscriberCountLabel"
        Me.SubscriberCountLabel.Size = New System.Drawing.Size(46, 14)
        Me.SubscriberCountLabel.TabIndex = 13
        Me.SubscriberCountLabel.Text = "Label11"
        Me.SubscriberCountLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(302, 118)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(53, 12)
        Me.Label12.TabIndex = 12
        Me.Label12.Text = "購読者数"
        '
        'DescriptionText
        '
        Me.DescriptionText.Location = New System.Drawing.Point(318, 159)
        Me.DescriptionText.Multiline = True
        Me.DescriptionText.Name = "DescriptionText"
        Me.DescriptionText.ReadOnly = True
        Me.DescriptionText.Size = New System.Drawing.Size(174, 88)
        Me.DescriptionText.TabIndex = 14
        Me.DescriptionText.Text = "Description"
        '
        'RefreshButton
        '
        Me.RefreshButton.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.RefreshButton.Location = New System.Drawing.Point(12, 256)
        Me.RefreshButton.Name = "RefreshButton"
        Me.RefreshButton.Size = New System.Drawing.Size(67, 21)
        Me.RefreshButton.TabIndex = 15
        Me.RefreshButton.Text = "Refresh"
        '
        'ListsList
        '
        Me.ListsList.FormattingEnabled = True
        Me.ListsList.ItemHeight = 12
        Me.ListsList.Location = New System.Drawing.Point(12, 12)
        Me.ListsList.Name = "ListsList"
        Me.ListsList.Size = New System.Drawing.Size(284, 232)
        Me.ListsList.TabIndex = 16
        '
        'ListAvailable
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(504, 291)
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
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ListAvailable"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "ListAvailable"
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
