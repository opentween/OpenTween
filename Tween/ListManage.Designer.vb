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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ListManage))
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
        resources.ApplyResources(Me.ListsList, "ListsList")
        Me.ListsList.Name = "ListsList"
        '
        'DescriptionText
        '
        resources.ApplyResources(Me.DescriptionText, "DescriptionText")
        Me.DescriptionText.Name = "DescriptionText"
        Me.DescriptionText.ReadOnly = True
        '
        'Label12
        '
        resources.ApplyResources(Me.Label12, "Label12")
        Me.Label12.Name = "Label12"
        '
        'Label10
        '
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Name = "Label10"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'UserList
        '
        Me.UserList.FormattingEnabled = True
        resources.ApplyResources(Me.UserList, "UserList")
        Me.UserList.Name = "UserList"
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
        resources.ApplyResources(Me.UserGroup, "UserGroup")
        Me.UserGroup.Name = "UserGroup"
        Me.UserGroup.TabStop = False
        '
        'UserTweetDateTime
        '
        resources.ApplyResources(Me.UserTweetDateTime, "UserTweetDateTime")
        Me.UserTweetDateTime.Name = "UserTweetDateTime"
        '
        'UserIcon
        '
        Me.UserIcon.BackColor = System.Drawing.Color.White
        Me.UserIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.UserIcon, "UserIcon")
        Me.UserIcon.Name = "UserIcon"
        Me.UserIcon.TabStop = False
        '
        'UserTweet
        '
        Me.UserTweet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.UserTweet, "UserTweet")
        Me.UserTweet.Name = "UserTweet"
        '
        'Label20
        '
        resources.ApplyResources(Me.Label20, "Label20")
        Me.Label20.Name = "Label20"
        '
        'DeleteUserButton
        '
        resources.ApplyResources(Me.DeleteUserButton, "DeleteUserButton")
        Me.DeleteUserButton.Name = "DeleteUserButton"
        Me.DeleteUserButton.UseVisualStyleBackColor = True
        '
        'UserProfile
        '
        Me.UserProfile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.UserProfile, "UserProfile")
        Me.UserProfile.Name = "UserProfile"
        '
        'Label17
        '
        resources.ApplyResources(Me.Label17, "Label17")
        Me.Label17.Name = "Label17"
        '
        'UserPostsNum
        '
        Me.UserPostsNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.UserPostsNum, "UserPostsNum")
        Me.UserPostsNum.Name = "UserPostsNum"
        '
        'Label15
        '
        resources.ApplyResources(Me.Label15, "Label15")
        Me.Label15.Name = "Label15"
        '
        'UserFollowerNum
        '
        Me.UserFollowerNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.UserFollowerNum, "UserFollowerNum")
        Me.UserFollowerNum.Name = "UserFollowerNum"
        '
        'Label13
        '
        resources.ApplyResources(Me.Label13, "Label13")
        Me.Label13.Name = "Label13"
        '
        'UserFollowNum
        '
        Me.UserFollowNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.UserFollowNum, "UserFollowNum")
        Me.UserFollowNum.Name = "UserFollowNum"
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        '
        'UserWeb
        '
        Me.UserWeb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.UserWeb, "UserWeb")
        Me.UserWeb.Name = "UserWeb"
        Me.UserWeb.TabStop = True
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'UserLocation
        '
        Me.UserLocation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.UserLocation, "UserLocation")
        Me.UserLocation.Name = "UserLocation"
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'AddListButton
        '
        resources.ApplyResources(Me.AddListButton, "AddListButton")
        Me.AddListButton.Name = "AddListButton"
        Me.AddListButton.UseVisualStyleBackColor = True
        '
        'DeleteListButton
        '
        resources.ApplyResources(Me.DeleteListButton, "DeleteListButton")
        Me.DeleteListButton.Name = "DeleteListButton"
        Me.DeleteListButton.UseVisualStyleBackColor = True
        '
        'GetMoreUsersButton
        '
        resources.ApplyResources(Me.GetMoreUsersButton, "GetMoreUsersButton")
        Me.GetMoreUsersButton.Name = "GetMoreUsersButton"
        Me.GetMoreUsersButton.UseVisualStyleBackColor = True
        '
        'NameTextBox
        '
        resources.ApplyResources(Me.NameTextBox, "NameTextBox")
        Me.NameTextBox.Name = "NameTextBox"
        Me.NameTextBox.ReadOnly = True
        '
        'UsernameTextBox
        '
        resources.ApplyResources(Me.UsernameTextBox, "UsernameTextBox")
        Me.UsernameTextBox.Name = "UsernameTextBox"
        Me.UsernameTextBox.ReadOnly = True
        '
        'MemberCountTextBox
        '
        resources.ApplyResources(Me.MemberCountTextBox, "MemberCountTextBox")
        Me.MemberCountTextBox.Name = "MemberCountTextBox"
        Me.MemberCountTextBox.ReadOnly = True
        '
        'SubscriberCountTextBox
        '
        resources.ApplyResources(Me.SubscriberCountTextBox, "SubscriberCountTextBox")
        Me.SubscriberCountTextBox.Name = "SubscriberCountTextBox"
        Me.SubscriberCountTextBox.ReadOnly = True
        '
        'EditCheckBox
        '
        resources.ApplyResources(Me.EditCheckBox, "EditCheckBox")
        Me.EditCheckBox.Name = "EditCheckBox"
        Me.EditCheckBox.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.PrivateRadioButton)
        Me.GroupBox2.Controls.Add(Me.PublicRadioButton)
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
        '
        'PrivateRadioButton
        '
        resources.ApplyResources(Me.PrivateRadioButton, "PrivateRadioButton")
        Me.PrivateRadioButton.Name = "PrivateRadioButton"
        Me.PrivateRadioButton.TabStop = True
        Me.PrivateRadioButton.UseVisualStyleBackColor = True
        '
        'PublicRadioButton
        '
        resources.ApplyResources(Me.PublicRadioButton, "PublicRadioButton")
        Me.PublicRadioButton.Name = "PublicRadioButton"
        Me.PublicRadioButton.TabStop = True
        Me.PublicRadioButton.UseVisualStyleBackColor = True
        '
        'OKEditButton
        '
        resources.ApplyResources(Me.OKEditButton, "OKEditButton")
        Me.OKEditButton.Name = "OKEditButton"
        Me.OKEditButton.UseVisualStyleBackColor = True
        '
        'CancelEditButton
        '
        resources.ApplyResources(Me.CancelEditButton, "CancelEditButton")
        Me.CancelEditButton.Name = "CancelEditButton"
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
        resources.ApplyResources(Me.ListGroup, "ListGroup")
        Me.ListGroup.Name = "ListGroup"
        Me.ListGroup.TabStop = False
        '
        'RefreshUsersButton
        '
        resources.ApplyResources(Me.RefreshUsersButton, "RefreshUsersButton")
        Me.RefreshUsersButton.Name = "RefreshUsersButton"
        Me.RefreshUsersButton.UseVisualStyleBackColor = True
        '
        'RefreshListsButton
        '
        resources.ApplyResources(Me.RefreshListsButton, "RefreshListsButton")
        Me.RefreshListsButton.Name = "RefreshListsButton"
        Me.RefreshListsButton.UseVisualStyleBackColor = True
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'CloseButton
        '
        Me.CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK
        resources.ApplyResources(Me.CloseButton, "CloseButton")
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'MemberGroup
        '
        Me.MemberGroup.Controls.Add(Me.UserList)
        Me.MemberGroup.Controls.Add(Me.GetMoreUsersButton)
        Me.MemberGroup.Controls.Add(Me.RefreshUsersButton)
        resources.ApplyResources(Me.MemberGroup, "MemberGroup")
        Me.MemberGroup.Name = "MemberGroup"
        Me.MemberGroup.TabStop = False
        '
        'ListManage
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.CloseButton
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
