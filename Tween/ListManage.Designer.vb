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
        Me.UserGroup = New System.Windows.Forms.GroupBox()
        Me.UserTweetDateTime = New System.Windows.Forms.Label()
        Me.UserIcon = New System.Windows.Forms.PictureBox()
        Me.UserTweet = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.DeleteUserButton = New System.Windows.Forms.Button()
        Me.UserProfile = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.UserPostsNum = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.UserFollowerNum = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.UserFollowNum = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.UserWeb = New System.Windows.Forms.LinkLabel()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.UserLocation = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.AddListButton = New System.Windows.Forms.Button()
        Me.DeleteListButton = New System.Windows.Forms.Button()
        Me.GetMoreUsersButton = New System.Windows.Forms.Button()
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
        Me.ListGroup = New System.Windows.Forms.GroupBox()
        Me.RefreshUsersButton = New System.Windows.Forms.Button()
        Me.RefreshListsButton = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.CloseButton = New System.Windows.Forms.Button()
        Me.MemberGroup = New System.Windows.Forms.GroupBox()
        Me.UserGroup.SuspendLayout()
        CType(Me.UserIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.ListGroup.SuspendLayout()
        Me.MemberGroup.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListsList
        '
        Me.ListsList.DisplayMember = "Name"
        Me.ListsList.FormattingEnabled = True
        Me.ListsList.ItemHeight = 12
        Me.ListsList.Location = New System.Drawing.Point(12, 24)
        Me.ListsList.Name = "ListsList"
        Me.ListsList.Size = New System.Drawing.Size(215, 184)
        Me.ListsList.TabIndex = 1
        '
        'DescriptionText
        '
        Me.DescriptionText.Location = New System.Drawing.Point(65, 153)
        Me.DescriptionText.Multiline = True
        Me.DescriptionText.Name = "DescriptionText"
        Me.DescriptionText.ReadOnly = True
        Me.DescriptionText.Size = New System.Drawing.Size(140, 56)
        Me.DescriptionText.TabIndex = 10
        Me.DescriptionText.Text = "Description"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label12.Location = New System.Drawing.Point(6, 131)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(53, 12)
        Me.Label12.TabIndex = 7
        Me.Label12.Text = "購読者数"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label10.Location = New System.Drawing.Point(6, 106)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(53, 12)
        Me.Label10.TabIndex = 5
        Me.Label10.Text = "登録者数"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(6, 156)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(29, 12)
        Me.Label6.TabIndex = 9
        Me.Label6.Text = "説明"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(6, 46)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(41, 12)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "リスト名"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(6, 21)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 12)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "作成者"
        '
        'UserList
        '
        Me.UserList.FormattingEnabled = True
        Me.UserList.HorizontalScrollbar = True
        Me.UserList.ItemHeight = 12
        Me.UserList.Location = New System.Drawing.Point(6, 15)
        Me.UserList.Name = "UserList"
        Me.UserList.Size = New System.Drawing.Size(212, 424)
        Me.UserList.TabIndex = 0
        '
        'UserGroup
        '
        Me.UserGroup.Controls.Add(Me.UserTweetDateTime)
        Me.UserGroup.Controls.Add(Me.UserIcon)
        Me.UserGroup.Controls.Add(Me.UserTweet)
        Me.UserGroup.Controls.Add(Me.Label20)
        Me.UserGroup.Controls.Add(Me.DeleteUserButton)
        Me.UserGroup.Controls.Add(Me.UserProfile)
        Me.UserGroup.Controls.Add(Me.Label17)
        Me.UserGroup.Controls.Add(Me.UserPostsNum)
        Me.UserGroup.Controls.Add(Me.Label15)
        Me.UserGroup.Controls.Add(Me.UserFollowerNum)
        Me.UserGroup.Controls.Add(Me.Label13)
        Me.UserGroup.Controls.Add(Me.UserFollowNum)
        Me.UserGroup.Controls.Add(Me.Label9)
        Me.UserGroup.Controls.Add(Me.UserWeb)
        Me.UserGroup.Controls.Add(Me.Label8)
        Me.UserGroup.Controls.Add(Me.UserLocation)
        Me.UserGroup.Controls.Add(Me.Label5)
        Me.UserGroup.Location = New System.Drawing.Point(463, 9)
        Me.UserGroup.Name = "UserGroup"
        Me.UserGroup.Size = New System.Drawing.Size(208, 475)
        Me.UserGroup.TabIndex = 8
        Me.UserGroup.TabStop = False
        Me.UserGroup.Text = "ユーザー情報"
        '
        'UserTweetDateTime
        '
        Me.UserTweetDateTime.Location = New System.Drawing.Point(90, 293)
        Me.UserTweetDateTime.Name = "UserTweetDateTime"
        Me.UserTweetDateTime.Size = New System.Drawing.Size(112, 12)
        Me.UserTweetDateTime.TabIndex = 12
        Me.UserTweetDateTime.Text = "YY/MM/DD HH:MM"
        Me.UserTweetDateTime.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'UserIcon
        '
        Me.UserIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.UserIcon.Location = New System.Drawing.Point(11, 19)
        Me.UserIcon.Name = "UserIcon"
        Me.UserIcon.Size = New System.Drawing.Size(49, 49)
        Me.UserIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.UserIcon.TabIndex = 37
        Me.UserIcon.TabStop = False
        '
        'UserTweet
        '
        Me.UserTweet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UserTweet.Location = New System.Drawing.Point(11, 305)
        Me.UserTweet.Name = "UserTweet"
        Me.UserTweet.Size = New System.Drawing.Size(191, 135)
        Me.UserTweet.TabIndex = 14
        Me.UserTweet.Text = "Label19"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(9, 293)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(75, 12)
        Me.Label20.TabIndex = 13
        Me.Label20.Text = "最近のツイート"
        '
        'DeleteUserButton
        '
        Me.DeleteUserButton.Location = New System.Drawing.Point(9, 443)
        Me.DeleteUserButton.Name = "DeleteUserButton"
        Me.DeleteUserButton.Size = New System.Drawing.Size(193, 23)
        Me.DeleteUserButton.TabIndex = 15
        Me.DeleteUserButton.Text = "リストから削除(&L)"
        Me.DeleteUserButton.UseVisualStyleBackColor = True
        '
        'UserProfile
        '
        Me.UserProfile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UserProfile.Location = New System.Drawing.Point(11, 155)
        Me.UserProfile.Name = "UserProfile"
        Me.UserProfile.Size = New System.Drawing.Size(191, 135)
        Me.UserProfile.TabIndex = 11
        Me.UserProfile.Text = "Label18"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(9, 143)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(53, 12)
        Me.Label17.TabIndex = 10
        Me.Label17.Text = "自己紹介"
        '
        'UserPostsNum
        '
        Me.UserPostsNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UserPostsNum.Location = New System.Drawing.Point(147, 125)
        Me.UserPostsNum.Name = "UserPostsNum"
        Me.UserPostsNum.Size = New System.Drawing.Size(55, 14)
        Me.UserPostsNum.TabIndex = 9
        Me.UserPostsNum.Text = "Label16"
        Me.UserPostsNum.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(147, 109)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(41, 12)
        Me.Label15.TabIndex = 8
        Me.Label15.Text = "発言数"
        '
        'UserFollowerNum
        '
        Me.UserFollowerNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UserFollowerNum.Location = New System.Drawing.Point(79, 125)
        Me.UserFollowerNum.Name = "UserFollowerNum"
        Me.UserFollowerNum.Size = New System.Drawing.Size(55, 14)
        Me.UserFollowerNum.TabIndex = 7
        Me.UserFollowerNum.Text = "Label14"
        Me.UserFollowerNum.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(77, 109)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(49, 12)
        Me.Label13.TabIndex = 6
        Me.Label13.Text = "フォロワー"
        '
        'UserFollowNum
        '
        Me.UserFollowNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UserFollowNum.Location = New System.Drawing.Point(11, 125)
        Me.UserFollowNum.Name = "UserFollowNum"
        Me.UserFollowNum.Size = New System.Drawing.Size(55, 14)
        Me.UserFollowNum.TabIndex = 5
        Me.UserFollowNum.Text = "Label11"
        Me.UserFollowNum.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(9, 109)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(40, 12)
        Me.Label9.TabIndex = 4
        Me.Label9.Text = "フォロー"
        '
        'UserWeb
        '
        Me.UserWeb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UserWeb.Location = New System.Drawing.Point(41, 72)
        Me.UserWeb.Name = "UserWeb"
        Me.UserWeb.Size = New System.Drawing.Size(161, 35)
        Me.UserWeb.TabIndex = 3
        Me.UserWeb.TabStop = True
        Me.UserWeb.Text = "LinkLabel1"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(9, 74)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(26, 12)
        Me.Label8.TabIndex = 2
        Me.Label8.Text = "Web"
        '
        'UserLocation
        '
        Me.UserLocation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UserLocation.Location = New System.Drawing.Point(68, 31)
        Me.UserLocation.Name = "UserLocation"
        Me.UserLocation.Size = New System.Drawing.Size(134, 37)
        Me.UserLocation.TabIndex = 1
        Me.UserLocation.Text = "Label7"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(66, 19)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(41, 12)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "現在地"
        '
        'AddListButton
        '
        Me.AddListButton.Location = New System.Drawing.Point(12, 214)
        Me.AddListButton.Name = "AddListButton"
        Me.AddListButton.Size = New System.Drawing.Size(53, 23)
        Me.AddListButton.TabIndex = 2
        Me.AddListButton.Text = "追加(&A)"
        Me.AddListButton.UseVisualStyleBackColor = True
        '
        'DeleteListButton
        '
        Me.DeleteListButton.Location = New System.Drawing.Point(174, 214)
        Me.DeleteListButton.Name = "DeleteListButton"
        Me.DeleteListButton.Size = New System.Drawing.Size(53, 23)
        Me.DeleteListButton.TabIndex = 4
        Me.DeleteListButton.Text = "削除(&D)"
        Me.DeleteListButton.UseVisualStyleBackColor = True
        '
        'GetMoreUsersButton
        '
        Me.GetMoreUsersButton.Location = New System.Drawing.Point(6, 445)
        Me.GetMoreUsersButton.Name = "GetMoreUsersButton"
        Me.GetMoreUsersButton.Size = New System.Drawing.Size(212, 23)
        Me.GetMoreUsersButton.TabIndex = 1
        Me.GetMoreUsersButton.Text = "さらに取得(&M)"
        Me.GetMoreUsersButton.UseVisualStyleBackColor = True
        '
        'NameTextBox
        '
        Me.NameTextBox.Location = New System.Drawing.Point(65, 43)
        Me.NameTextBox.Name = "NameTextBox"
        Me.NameTextBox.ReadOnly = True
        Me.NameTextBox.Size = New System.Drawing.Size(140, 19)
        Me.NameTextBox.TabIndex = 3
        '
        'UsernameTextBox
        '
        Me.UsernameTextBox.Location = New System.Drawing.Point(65, 18)
        Me.UsernameTextBox.Name = "UsernameTextBox"
        Me.UsernameTextBox.ReadOnly = True
        Me.UsernameTextBox.Size = New System.Drawing.Size(140, 19)
        Me.UsernameTextBox.TabIndex = 1
        '
        'MemberCountTextBox
        '
        Me.MemberCountTextBox.Location = New System.Drawing.Point(65, 103)
        Me.MemberCountTextBox.Name = "MemberCountTextBox"
        Me.MemberCountTextBox.ReadOnly = True
        Me.MemberCountTextBox.Size = New System.Drawing.Size(46, 19)
        Me.MemberCountTextBox.TabIndex = 6
        '
        'SubscriberCountTextBox
        '
        Me.SubscriberCountTextBox.Location = New System.Drawing.Point(65, 128)
        Me.SubscriberCountTextBox.Name = "SubscriberCountTextBox"
        Me.SubscriberCountTextBox.ReadOnly = True
        Me.SubscriberCountTextBox.Size = New System.Drawing.Size(46, 19)
        Me.SubscriberCountTextBox.TabIndex = 8
        '
        'EditCheckBox
        '
        Me.EditCheckBox.Appearance = System.Windows.Forms.Appearance.Button
        Me.EditCheckBox.Location = New System.Drawing.Point(71, 214)
        Me.EditCheckBox.Name = "EditCheckBox"
        Me.EditCheckBox.Size = New System.Drawing.Size(53, 23)
        Me.EditCheckBox.TabIndex = 3
        Me.EditCheckBox.Text = "編集(&E)"
        Me.EditCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.EditCheckBox.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.PrivateRadioButton)
        Me.GroupBox2.Controls.Add(Me.PublicRadioButton)
        Me.GroupBox2.Location = New System.Drawing.Point(8, 68)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(197, 29)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "種別"
        '
        'PrivateRadioButton
        '
        Me.PrivateRadioButton.AutoSize = True
        Me.PrivateRadioButton.Enabled = False
        Me.PrivateRadioButton.Location = New System.Drawing.Point(38, 9)
        Me.PrivateRadioButton.Name = "PrivateRadioButton"
        Me.PrivateRadioButton.Size = New System.Drawing.Size(59, 16)
        Me.PrivateRadioButton.TabIndex = 0
        Me.PrivateRadioButton.TabStop = True
        Me.PrivateRadioButton.Text = "Private"
        Me.PrivateRadioButton.UseVisualStyleBackColor = True
        '
        'PublicRadioButton
        '
        Me.PublicRadioButton.AutoSize = True
        Me.PublicRadioButton.Enabled = False
        Me.PublicRadioButton.Location = New System.Drawing.Point(103, 9)
        Me.PublicRadioButton.Name = "PublicRadioButton"
        Me.PublicRadioButton.Size = New System.Drawing.Size(54, 16)
        Me.PublicRadioButton.TabIndex = 1
        Me.PublicRadioButton.TabStop = True
        Me.PublicRadioButton.Text = "Public"
        Me.PublicRadioButton.UseVisualStyleBackColor = True
        '
        'OKEditButton
        '
        Me.OKEditButton.Enabled = False
        Me.OKEditButton.Location = New System.Drawing.Point(16, 213)
        Me.OKEditButton.Name = "OKEditButton"
        Me.OKEditButton.Size = New System.Drawing.Size(75, 23)
        Me.OKEditButton.TabIndex = 11
        Me.OKEditButton.Text = "OK"
        Me.OKEditButton.UseVisualStyleBackColor = True
        '
        'CancelEditButton
        '
        Me.CancelEditButton.Enabled = False
        Me.CancelEditButton.Location = New System.Drawing.Point(111, 213)
        Me.CancelEditButton.Name = "CancelEditButton"
        Me.CancelEditButton.Size = New System.Drawing.Size(75, 23)
        Me.CancelEditButton.TabIndex = 12
        Me.CancelEditButton.Text = "Cancel"
        Me.CancelEditButton.UseVisualStyleBackColor = True
        '
        'ListGroup
        '
        Me.ListGroup.Controls.Add(Me.Label1)
        Me.ListGroup.Controls.Add(Me.CancelEditButton)
        Me.ListGroup.Controls.Add(Me.GroupBox2)
        Me.ListGroup.Controls.Add(Me.OKEditButton)
        Me.ListGroup.Controls.Add(Me.SubscriberCountTextBox)
        Me.ListGroup.Controls.Add(Me.MemberCountTextBox)
        Me.ListGroup.Controls.Add(Me.UsernameTextBox)
        Me.ListGroup.Controls.Add(Me.NameTextBox)
        Me.ListGroup.Controls.Add(Me.Label4)
        Me.ListGroup.Controls.Add(Me.Label6)
        Me.ListGroup.Controls.Add(Me.Label10)
        Me.ListGroup.Controls.Add(Me.DescriptionText)
        Me.ListGroup.Controls.Add(Me.Label12)
        Me.ListGroup.Location = New System.Drawing.Point(12, 269)
        Me.ListGroup.Name = "ListGroup"
        Me.ListGroup.Size = New System.Drawing.Size(215, 241)
        Me.ListGroup.TabIndex = 6
        Me.ListGroup.TabStop = False
        Me.ListGroup.Text = "詳細"
        '
        'RefreshUsersButton
        '
        Me.RefreshUsersButton.Location = New System.Drawing.Point(153, 472)
        Me.RefreshUsersButton.Name = "RefreshUsersButton"
        Me.RefreshUsersButton.Size = New System.Drawing.Size(65, 23)
        Me.RefreshUsersButton.TabIndex = 2
        Me.RefreshUsersButton.Text = "再取得(&U)"
        Me.RefreshUsersButton.UseVisualStyleBackColor = True
        '
        'RefreshListsButton
        '
        Me.RefreshListsButton.Location = New System.Drawing.Point(12, 243)
        Me.RefreshListsButton.Name = "RefreshListsButton"
        Me.RefreshListsButton.Size = New System.Drawing.Size(215, 23)
        Me.RefreshListsButton.TabIndex = 5
        Me.RefreshListsButton.Text = "再取得(&R)"
        Me.RefreshListsButton.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(10, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(29, 12)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "リスト"
        '
        'CloseButton
        '
        Me.CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.CloseButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CloseButton.Location = New System.Drawing.Point(596, 490)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(75, 23)
        Me.CloseButton.TabIndex = 9
        Me.CloseButton.Text = "閉じる(&C)"
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'MemberGroup
        '
        Me.MemberGroup.Controls.Add(Me.UserList)
        Me.MemberGroup.Controls.Add(Me.GetMoreUsersButton)
        Me.MemberGroup.Controls.Add(Me.RefreshUsersButton)
        Me.MemberGroup.Location = New System.Drawing.Point(233, 9)
        Me.MemberGroup.Name = "MemberGroup"
        Me.MemberGroup.Size = New System.Drawing.Size(224, 501)
        Me.MemberGroup.TabIndex = 7
        Me.MemberGroup.TabStop = False
        Me.MemberGroup.Text = "メンバー"
        '
        'ListManage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.CloseButton
        Me.ClientSize = New System.Drawing.Size(683, 522)
        Me.Controls.Add(Me.MemberGroup)
        Me.Controls.Add(Me.CloseButton)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.RefreshListsButton)
        Me.Controls.Add(Me.ListGroup)
        Me.Controls.Add(Me.DeleteListButton)
        Me.Controls.Add(Me.AddListButton)
        Me.Controls.Add(Me.UserGroup)
        Me.Controls.Add(Me.ListsList)
        Me.Controls.Add(Me.EditCheckBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ListManage"
        Me.ShowInTaskbar = False
        Me.Text = "ListManage"
        Me.TopMost = True
        Me.UserGroup.ResumeLayout(False)
        Me.UserGroup.PerformLayout()
        CType(Me.UserIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ListGroup.ResumeLayout(False)
        Me.ListGroup.PerformLayout()
        Me.MemberGroup.ResumeLayout(False)
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
    Friend WithEvents UserGroup As System.Windows.Forms.GroupBox
    Friend WithEvents AddListButton As System.Windows.Forms.Button
    Friend WithEvents DeleteListButton As System.Windows.Forms.Button
    Friend WithEvents GetMoreUsersButton As System.Windows.Forms.Button
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
    Friend WithEvents ListGroup As System.Windows.Forms.GroupBox
    Friend WithEvents RefreshUsersButton As System.Windows.Forms.Button
    Friend WithEvents UserTweet As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents UserProfile As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents UserFollowerNum As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents UserFollowNum As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents UserWeb As System.Windows.Forms.LinkLabel
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents UserLocation As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents RefreshListsButton As System.Windows.Forms.Button
    Friend WithEvents UserIcon As System.Windows.Forms.PictureBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents CloseButton As System.Windows.Forms.Button
    Friend WithEvents UserPostsNum As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents UserTweetDateTime As System.Windows.Forms.Label
    Friend WithEvents MemberGroup As System.Windows.Forms.GroupBox
End Class
