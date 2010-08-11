<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ListManage
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

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ListsList = New System.Windows.Forms.ListBox()
        Me.DescriptionText = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.UserList = New System.Windows.Forms.ListBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.AddListButton = New System.Windows.Forms.Button()
        Me.DeleteListButton = New System.Windows.Forms.Button()
        Me.RefreshUsersButton = New System.Windows.Forms.Button()
        Me.DeleteUserButton = New System.Windows.Forms.Button()
        Me.NameTextBox = New System.Windows.Forms.TextBox()
        Me.UsernameTextBox = New System.Windows.Forms.TextBox()
        Me.MemberCountTextBox = New System.Windows.Forms.TextBox()
        Me.SubscriberCountTextBox = New System.Windows.Forms.TextBox()
        Me.EditCheckBox = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.PrivateRadioButton = New System.Windows.Forms.RadioButton()
        Me.PublicRadioButton = New System.Windows.Forms.RadioButton()
        Me.OKEditButton = New System.Windows.Forms.Button()
        Me.CancelEditButton = New System.Windows.Forms.Button()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListsList
        '
        Me.ListsList.DisplayMember = "Name"
        Me.ListsList.FormattingEnabled = True
        Me.ListsList.ItemHeight = 12
        Me.ListsList.Location = New System.Drawing.Point(12, 26)
        Me.ListsList.Name = "ListsList"
        Me.ListsList.Size = New System.Drawing.Size(215, 232)
        Me.ListsList.TabIndex = 17
        '
        'DescriptionText
        '
        Me.DescriptionText.Location = New System.Drawing.Point(28, 477)
        Me.DescriptionText.Multiline = True
        Me.DescriptionText.Name = "DescriptionText"
        Me.DescriptionText.ReadOnly = True
        Me.DescriptionText.Size = New System.Drawing.Size(174, 56)
        Me.DescriptionText.TabIndex = 29
        Me.DescriptionText.Text = "Description"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label12.Location = New System.Drawing.Point(85, 423)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(53, 12)
        Me.Label12.TabIndex = 27
        Me.Label12.Text = "購読者数"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label10.Location = New System.Drawing.Point(12, 423)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(53, 12)
        Me.Label10.TabIndex = 25
        Me.Label10.Text = "登録者数"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(12, 462)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(29, 12)
        Me.Label6.TabIndex = 22
        Me.Label6.Text = "説明"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(12, 335)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(41, 12)
        Me.Label4.TabIndex = 20
        Me.Label4.Text = "リスト名"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(12, 298)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 12)
        Me.Label1.TabIndex = 18
        Me.Label1.Text = "作成者"
        '
        'UserList
        '
        Me.UserList.FormattingEnabled = True
        Me.UserList.ItemHeight = 12
        Me.UserList.Location = New System.Drawing.Point(233, 26)
        Me.UserList.Name = "UserList"
        Me.UserList.Size = New System.Drawing.Size(224, 316)
        Me.UserList.TabIndex = 30
        '
        'GroupBox1
        '
        Me.GroupBox1.Location = New System.Drawing.Point(463, 26)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(200, 220)
        Me.GroupBox1.TabIndex = 31
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "選択したユーザー情報"
        '
        'AddListButton
        '
        Me.AddListButton.Location = New System.Drawing.Point(12, 264)
        Me.AddListButton.Name = "AddListButton"
        Me.AddListButton.Size = New System.Drawing.Size(53, 23)
        Me.AddListButton.TabIndex = 32
        Me.AddListButton.Text = "追加"
        Me.AddListButton.UseVisualStyleBackColor = True
        '
        'DeleteListButton
        '
        Me.DeleteListButton.Location = New System.Drawing.Point(174, 264)
        Me.DeleteListButton.Name = "DeleteListButton"
        Me.DeleteListButton.Size = New System.Drawing.Size(53, 23)
        Me.DeleteListButton.TabIndex = 34
        Me.DeleteListButton.Text = "削除"
        Me.DeleteListButton.UseVisualStyleBackColor = True
        '
        'RefreshUsersButton
        '
        Me.RefreshUsersButton.Location = New System.Drawing.Point(233, 348)
        Me.RefreshUsersButton.Name = "RefreshUsersButton"
        Me.RefreshUsersButton.Size = New System.Drawing.Size(133, 23)
        Me.RefreshUsersButton.TabIndex = 35
        Me.RefreshUsersButton.Text = "ユーザー一覧更新"
        Me.RefreshUsersButton.UseVisualStyleBackColor = True
        '
        'DeleteUserButton
        '
        Me.DeleteUserButton.Location = New System.Drawing.Point(463, 252)
        Me.DeleteUserButton.Name = "DeleteUserButton"
        Me.DeleteUserButton.Size = New System.Drawing.Size(114, 23)
        Me.DeleteUserButton.TabIndex = 36
        Me.DeleteUserButton.Text = "リストから削除"
        Me.DeleteUserButton.UseVisualStyleBackColor = True
        '
        'NameTextBox
        '
        Me.NameTextBox.Location = New System.Drawing.Point(28, 350)
        Me.NameTextBox.Name = "NameTextBox"
        Me.NameTextBox.ReadOnly = True
        Me.NameTextBox.Size = New System.Drawing.Size(174, 19)
        Me.NameTextBox.TabIndex = 37
        '
        'UsernameTextBox
        '
        Me.UsernameTextBox.Location = New System.Drawing.Point(28, 313)
        Me.UsernameTextBox.Name = "UsernameTextBox"
        Me.UsernameTextBox.ReadOnly = True
        Me.UsernameTextBox.Size = New System.Drawing.Size(174, 19)
        Me.UsernameTextBox.TabIndex = 39
        '
        'MemberCountTextBox
        '
        Me.MemberCountTextBox.Location = New System.Drawing.Point(28, 438)
        Me.MemberCountTextBox.Name = "MemberCountTextBox"
        Me.MemberCountTextBox.ReadOnly = True
        Me.MemberCountTextBox.Size = New System.Drawing.Size(46, 19)
        Me.MemberCountTextBox.TabIndex = 40
        '
        'SubscriberCountTextBox
        '
        Me.SubscriberCountTextBox.Location = New System.Drawing.Point(101, 438)
        Me.SubscriberCountTextBox.Name = "SubscriberCountTextBox"
        Me.SubscriberCountTextBox.ReadOnly = True
        Me.SubscriberCountTextBox.Size = New System.Drawing.Size(46, 19)
        Me.SubscriberCountTextBox.TabIndex = 41
        '
        'EditCheckBox
        '
        Me.EditCheckBox.Appearance = System.Windows.Forms.Appearance.Button
        Me.EditCheckBox.Location = New System.Drawing.Point(71, 264)
        Me.EditCheckBox.Name = "EditCheckBox"
        Me.EditCheckBox.Size = New System.Drawing.Size(53, 23)
        Me.EditCheckBox.TabIndex = 42
        Me.EditCheckBox.Text = "編集"
        Me.EditCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.EditCheckBox.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.PrivateRadioButton)
        Me.GroupBox2.Controls.Add(Me.PublicRadioButton)
        Me.GroupBox2.Location = New System.Drawing.Point(14, 375)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(188, 45)
        Me.GroupBox2.TabIndex = 43
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "種別"
        '
        'PrivateRadioButton
        '
        Me.PrivateRadioButton.AutoSize = True
        Me.PrivateRadioButton.Enabled = False
        Me.PrivateRadioButton.Location = New System.Drawing.Point(14, 18)
        Me.PrivateRadioButton.Name = "PrivateRadioButton"
        Me.PrivateRadioButton.Size = New System.Drawing.Size(59, 16)
        Me.PrivateRadioButton.TabIndex = 46
        Me.PrivateRadioButton.TabStop = True
        Me.PrivateRadioButton.Text = "Private"
        Me.PrivateRadioButton.UseVisualStyleBackColor = True
        '
        'PublicRadioButton
        '
        Me.PublicRadioButton.AutoSize = True
        Me.PublicRadioButton.Enabled = False
        Me.PublicRadioButton.Location = New System.Drawing.Point(79, 18)
        Me.PublicRadioButton.Name = "PublicRadioButton"
        Me.PublicRadioButton.Size = New System.Drawing.Size(54, 16)
        Me.PublicRadioButton.TabIndex = 0
        Me.PublicRadioButton.TabStop = True
        Me.PublicRadioButton.Text = "Public"
        Me.PublicRadioButton.UseVisualStyleBackColor = True
        '
        'OKEditButton
        '
        Me.OKEditButton.Enabled = False
        Me.OKEditButton.Location = New System.Drawing.Point(208, 510)
        Me.OKEditButton.Name = "OKEditButton"
        Me.OKEditButton.Size = New System.Drawing.Size(75, 23)
        Me.OKEditButton.TabIndex = 44
        Me.OKEditButton.Text = "OK"
        Me.OKEditButton.UseVisualStyleBackColor = True
        '
        'CancelEditButton
        '
        Me.CancelEditButton.Enabled = False
        Me.CancelEditButton.Location = New System.Drawing.Point(291, 510)
        Me.CancelEditButton.Name = "CancelEditButton"
        Me.CancelEditButton.Size = New System.Drawing.Size(75, 23)
        Me.CancelEditButton.TabIndex = 45
        Me.CancelEditButton.Text = "Cancel"
        Me.CancelEditButton.UseVisualStyleBackColor = True
        '
        'ListManage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(683, 538)
        Me.Controls.Add(Me.CancelEditButton)
        Me.Controls.Add(Me.OKEditButton)
        Me.Controls.Add(Me.DeleteUserButton)
        Me.Controls.Add(Me.RefreshUsersButton)
        Me.Controls.Add(Me.DeleteListButton)
        Me.Controls.Add(Me.AddListButton)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.UserList)
        Me.Controls.Add(Me.DescriptionText)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ListsList)
        Me.Controls.Add(Me.NameTextBox)
        Me.Controls.Add(Me.UsernameTextBox)
        Me.Controls.Add(Me.MemberCountTextBox)
        Me.Controls.Add(Me.SubscriberCountTextBox)
        Me.Controls.Add(Me.EditCheckBox)
        Me.Controls.Add(Me.GroupBox2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ListManage"
        Me.ShowInTaskbar = False
        Me.Text = "ListManage"
        Me.TopMost = True
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListsList As System.Windows.Forms.ListBox
    Friend WithEvents DescriptionText As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents UserList As System.Windows.Forms.ListBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents AddListButton As System.Windows.Forms.Button
    Friend WithEvents DeleteListButton As System.Windows.Forms.Button
    Friend WithEvents RefreshUsersButton As System.Windows.Forms.Button
    Friend WithEvents DeleteUserButton As System.Windows.Forms.Button
    Friend WithEvents NameTextBox As System.Windows.Forms.TextBox
    Friend WithEvents UsernameTextBox As System.Windows.Forms.TextBox
    Friend WithEvents MemberCountTextBox As System.Windows.Forms.TextBox
    Friend WithEvents SubscriberCountTextBox As System.Windows.Forms.TextBox
    Friend WithEvents EditCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents PrivateRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents PublicRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents OKEditButton As System.Windows.Forms.Button
    Friend WithEvents CancelEditButton As System.Windows.Forms.Button
End Class
