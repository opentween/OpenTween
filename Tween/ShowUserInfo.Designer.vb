<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ShowUserInfo
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ShowUserInfo))
        Me.ButtonClose = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.LinkLabelWeb = New System.Windows.Forms.LinkLabel
        Me.LabelLocation = New System.Windows.Forms.Label
        Me.LabelName = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.LinkLabelFollowing = New System.Windows.Forms.LinkLabel
        Me.LinkLabelFollowers = New System.Windows.Forms.LinkLabel
        Me.Label7 = New System.Windows.Forms.Label
        Me.LabelCreatedAt = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.LinkLabelTweet = New System.Windows.Forms.LinkLabel
        Me.Label9 = New System.Windows.Forms.Label
        Me.LinkLabelFav = New System.Windows.Forms.LinkLabel
        Me.ButtonFollow = New System.Windows.Forms.Button
        Me.ButtonUnFollow = New System.Windows.Forms.Button
        Me.Label10 = New System.Windows.Forms.Label
        Me.LabelIsProtected = New System.Windows.Forms.Label
        Me.LabelIsFollowing = New System.Windows.Forms.Label
        Me.LabelIsFollowed = New System.Windows.Forms.Label
        Me.UserPicture = New System.Windows.Forms.PictureBox
        Me.BackgroundWorkerImageLoader = New System.ComponentModel.BackgroundWorker
        Me.LabelScreenName = New System.Windows.Forms.Label
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SelectionCopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LabelRecentPost = New System.Windows.Forms.Label
        Me.RecentPostBrowser = New System.Windows.Forms.WebBrowser
        Me.DescriptionBrowser = New System.Windows.Forms.WebBrowser
        Me.LabelIsVerified = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        CType(Me.UserPicture, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ButtonClose
        '
        Me.ButtonClose.AccessibleDescription = Nothing
        Me.ButtonClose.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonClose, "ButtonClose")
        Me.ButtonClose.BackgroundImage = Nothing
        Me.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonClose.Font = Nothing
        Me.ButtonClose.Name = "ButtonClose"
        Me.ToolTip1.SetToolTip(Me.ButtonClose, resources.GetString("ButtonClose.ToolTip"))
        Me.ButtonClose.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = Nothing
        Me.Label1.AccessibleName = Nothing
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Font = Nothing
        Me.Label1.Name = "Label1"
        Me.ToolTip1.SetToolTip(Me.Label1, resources.GetString("Label1.ToolTip"))
        Me.Label1.UseMnemonic = False
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = Nothing
        Me.Label2.AccessibleName = Nothing
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Font = Nothing
        Me.Label2.Name = "Label2"
        Me.ToolTip1.SetToolTip(Me.Label2, resources.GetString("Label2.ToolTip"))
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = Nothing
        Me.Label3.AccessibleName = Nothing
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Font = Nothing
        Me.Label3.Name = "Label3"
        Me.ToolTip1.SetToolTip(Me.Label3, resources.GetString("Label3.ToolTip"))
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = Nothing
        Me.Label4.AccessibleName = Nothing
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Font = Nothing
        Me.Label4.Name = "Label4"
        Me.ToolTip1.SetToolTip(Me.Label4, resources.GetString("Label4.ToolTip"))
        '
        'LinkLabelWeb
        '
        Me.LinkLabelWeb.AccessibleDescription = Nothing
        Me.LinkLabelWeb.AccessibleName = Nothing
        resources.ApplyResources(Me.LinkLabelWeb, "LinkLabelWeb")
        Me.LinkLabelWeb.AutoEllipsis = True
        Me.LinkLabelWeb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LinkLabelWeb.Font = Nothing
        Me.LinkLabelWeb.Name = "LinkLabelWeb"
        Me.LinkLabelWeb.TabStop = True
        Me.ToolTip1.SetToolTip(Me.LinkLabelWeb, resources.GetString("LinkLabelWeb.ToolTip"))
        Me.LinkLabelWeb.UseMnemonic = False
        '
        'LabelLocation
        '
        Me.LabelLocation.AccessibleDescription = Nothing
        Me.LabelLocation.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelLocation, "LabelLocation")
        Me.LabelLocation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LabelLocation.Font = Nothing
        Me.LabelLocation.Name = "LabelLocation"
        Me.ToolTip1.SetToolTip(Me.LabelLocation, resources.GetString("LabelLocation.ToolTip"))
        Me.LabelLocation.UseMnemonic = False
        '
        'LabelName
        '
        Me.LabelName.AccessibleDescription = Nothing
        Me.LabelName.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelName, "LabelName")
        Me.LabelName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LabelName.Font = Nothing
        Me.LabelName.Name = "LabelName"
        Me.ToolTip1.SetToolTip(Me.LabelName, resources.GetString("LabelName.ToolTip"))
        Me.LabelName.UseMnemonic = False
        '
        'Label5
        '
        Me.Label5.AccessibleDescription = Nothing
        Me.Label5.AccessibleName = Nothing
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Font = Nothing
        Me.Label5.Name = "Label5"
        Me.ToolTip1.SetToolTip(Me.Label5, resources.GetString("Label5.ToolTip"))
        '
        'Label6
        '
        Me.Label6.AccessibleDescription = Nothing
        Me.Label6.AccessibleName = Nothing
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Font = Nothing
        Me.Label6.Name = "Label6"
        Me.ToolTip1.SetToolTip(Me.Label6, resources.GetString("Label6.ToolTip"))
        '
        'LinkLabelFollowing
        '
        Me.LinkLabelFollowing.AccessibleDescription = Nothing
        Me.LinkLabelFollowing.AccessibleName = Nothing
        resources.ApplyResources(Me.LinkLabelFollowing, "LinkLabelFollowing")
        Me.LinkLabelFollowing.Font = Nothing
        Me.LinkLabelFollowing.Name = "LinkLabelFollowing"
        Me.LinkLabelFollowing.TabStop = True
        Me.ToolTip1.SetToolTip(Me.LinkLabelFollowing, resources.GetString("LinkLabelFollowing.ToolTip"))
        '
        'LinkLabelFollowers
        '
        Me.LinkLabelFollowers.AccessibleDescription = Nothing
        Me.LinkLabelFollowers.AccessibleName = Nothing
        resources.ApplyResources(Me.LinkLabelFollowers, "LinkLabelFollowers")
        Me.LinkLabelFollowers.Font = Nothing
        Me.LinkLabelFollowers.Name = "LinkLabelFollowers"
        Me.LinkLabelFollowers.TabStop = True
        Me.ToolTip1.SetToolTip(Me.LinkLabelFollowers, resources.GetString("LinkLabelFollowers.ToolTip"))
        '
        'Label7
        '
        Me.Label7.AccessibleDescription = Nothing
        Me.Label7.AccessibleName = Nothing
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Font = Nothing
        Me.Label7.Name = "Label7"
        Me.ToolTip1.SetToolTip(Me.Label7, resources.GetString("Label7.ToolTip"))
        '
        'LabelCreatedAt
        '
        Me.LabelCreatedAt.AccessibleDescription = Nothing
        Me.LabelCreatedAt.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelCreatedAt, "LabelCreatedAt")
        Me.LabelCreatedAt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LabelCreatedAt.Font = Nothing
        Me.LabelCreatedAt.Name = "LabelCreatedAt"
        Me.ToolTip1.SetToolTip(Me.LabelCreatedAt, resources.GetString("LabelCreatedAt.ToolTip"))
        '
        'Label8
        '
        Me.Label8.AccessibleDescription = Nothing
        Me.Label8.AccessibleName = Nothing
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Font = Nothing
        Me.Label8.Name = "Label8"
        Me.ToolTip1.SetToolTip(Me.Label8, resources.GetString("Label8.ToolTip"))
        '
        'LinkLabelTweet
        '
        Me.LinkLabelTweet.AccessibleDescription = Nothing
        Me.LinkLabelTweet.AccessibleName = Nothing
        resources.ApplyResources(Me.LinkLabelTweet, "LinkLabelTweet")
        Me.LinkLabelTweet.Font = Nothing
        Me.LinkLabelTweet.Name = "LinkLabelTweet"
        Me.LinkLabelTweet.TabStop = True
        Me.ToolTip1.SetToolTip(Me.LinkLabelTweet, resources.GetString("LinkLabelTweet.ToolTip"))
        '
        'Label9
        '
        Me.Label9.AccessibleDescription = Nothing
        Me.Label9.AccessibleName = Nothing
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Font = Nothing
        Me.Label9.Name = "Label9"
        Me.ToolTip1.SetToolTip(Me.Label9, resources.GetString("Label9.ToolTip"))
        '
        'LinkLabelFav
        '
        Me.LinkLabelFav.AccessibleDescription = Nothing
        Me.LinkLabelFav.AccessibleName = Nothing
        resources.ApplyResources(Me.LinkLabelFav, "LinkLabelFav")
        Me.LinkLabelFav.Font = Nothing
        Me.LinkLabelFav.Name = "LinkLabelFav"
        Me.LinkLabelFav.TabStop = True
        Me.ToolTip1.SetToolTip(Me.LinkLabelFav, resources.GetString("LinkLabelFav.ToolTip"))
        '
        'ButtonFollow
        '
        Me.ButtonFollow.AccessibleDescription = Nothing
        Me.ButtonFollow.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonFollow, "ButtonFollow")
        Me.ButtonFollow.BackgroundImage = Nothing
        Me.ButtonFollow.Font = Nothing
        Me.ButtonFollow.Name = "ButtonFollow"
        Me.ToolTip1.SetToolTip(Me.ButtonFollow, resources.GetString("ButtonFollow.ToolTip"))
        Me.ButtonFollow.UseVisualStyleBackColor = True
        '
        'ButtonUnFollow
        '
        Me.ButtonUnFollow.AccessibleDescription = Nothing
        Me.ButtonUnFollow.AccessibleName = Nothing
        resources.ApplyResources(Me.ButtonUnFollow, "ButtonUnFollow")
        Me.ButtonUnFollow.BackgroundImage = Nothing
        Me.ButtonUnFollow.Font = Nothing
        Me.ButtonUnFollow.Name = "ButtonUnFollow"
        Me.ToolTip1.SetToolTip(Me.ButtonUnFollow, resources.GetString("ButtonUnFollow.ToolTip"))
        Me.ButtonUnFollow.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AccessibleDescription = Nothing
        Me.Label10.AccessibleName = Nothing
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Font = Nothing
        Me.Label10.Name = "Label10"
        Me.ToolTip1.SetToolTip(Me.Label10, resources.GetString("Label10.ToolTip"))
        '
        'LabelIsProtected
        '
        Me.LabelIsProtected.AccessibleDescription = Nothing
        Me.LabelIsProtected.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelIsProtected, "LabelIsProtected")
        Me.LabelIsProtected.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LabelIsProtected.Font = Nothing
        Me.LabelIsProtected.Name = "LabelIsProtected"
        Me.ToolTip1.SetToolTip(Me.LabelIsProtected, resources.GetString("LabelIsProtected.ToolTip"))
        '
        'LabelIsFollowing
        '
        Me.LabelIsFollowing.AccessibleDescription = Nothing
        Me.LabelIsFollowing.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelIsFollowing, "LabelIsFollowing")
        Me.LabelIsFollowing.Font = Nothing
        Me.LabelIsFollowing.Name = "LabelIsFollowing"
        Me.ToolTip1.SetToolTip(Me.LabelIsFollowing, resources.GetString("LabelIsFollowing.ToolTip"))
        '
        'LabelIsFollowed
        '
        Me.LabelIsFollowed.AccessibleDescription = Nothing
        Me.LabelIsFollowed.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelIsFollowed, "LabelIsFollowed")
        Me.LabelIsFollowed.Font = Nothing
        Me.LabelIsFollowed.Name = "LabelIsFollowed"
        Me.ToolTip1.SetToolTip(Me.LabelIsFollowed, resources.GetString("LabelIsFollowed.ToolTip"))
        '
        'UserPicture
        '
        Me.UserPicture.AccessibleDescription = Nothing
        Me.UserPicture.AccessibleName = Nothing
        resources.ApplyResources(Me.UserPicture, "UserPicture")
        Me.UserPicture.BackgroundImage = Nothing
        Me.UserPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.UserPicture.Font = Nothing
        Me.UserPicture.ImageLocation = Nothing
        Me.UserPicture.Name = "UserPicture"
        Me.UserPicture.TabStop = False
        Me.ToolTip1.SetToolTip(Me.UserPicture, resources.GetString("UserPicture.ToolTip"))
        '
        'BackgroundWorkerImageLoader
        '
        '
        'LabelScreenName
        '
        Me.LabelScreenName.AccessibleDescription = Nothing
        Me.LabelScreenName.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelScreenName, "LabelScreenName")
        Me.LabelScreenName.BackColor = System.Drawing.SystemColors.ButtonHighlight
        Me.LabelScreenName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LabelScreenName.Name = "LabelScreenName"
        Me.ToolTip1.SetToolTip(Me.LabelScreenName, resources.GetString("LabelScreenName.ToolTip"))
        '
        'ToolTip1
        '
        Me.ToolTip1.ShowAlways = True
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.AccessibleDescription = Nothing
        Me.ContextMenuStrip1.AccessibleName = Nothing
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        Me.ContextMenuStrip1.BackgroundImage = Nothing
        Me.ContextMenuStrip1.Font = Nothing
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SelectionCopyToolStripMenuItem, Me.SelectAllToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ToolTip1.SetToolTip(Me.ContextMenuStrip1, resources.GetString("ContextMenuStrip1.ToolTip"))
        '
        'SelectionCopyToolStripMenuItem
        '
        Me.SelectionCopyToolStripMenuItem.AccessibleDescription = Nothing
        Me.SelectionCopyToolStripMenuItem.AccessibleName = Nothing
        resources.ApplyResources(Me.SelectionCopyToolStripMenuItem, "SelectionCopyToolStripMenuItem")
        Me.SelectionCopyToolStripMenuItem.BackgroundImage = Nothing
        Me.SelectionCopyToolStripMenuItem.Name = "SelectionCopyToolStripMenuItem"
        Me.SelectionCopyToolStripMenuItem.ShortcutKeyDisplayString = Nothing
        '
        'SelectAllToolStripMenuItem
        '
        Me.SelectAllToolStripMenuItem.AccessibleDescription = Nothing
        Me.SelectAllToolStripMenuItem.AccessibleName = Nothing
        resources.ApplyResources(Me.SelectAllToolStripMenuItem, "SelectAllToolStripMenuItem")
        Me.SelectAllToolStripMenuItem.BackgroundImage = Nothing
        Me.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
        Me.SelectAllToolStripMenuItem.ShortcutKeyDisplayString = Nothing
        '
        'LabelRecentPost
        '
        Me.LabelRecentPost.AccessibleDescription = Nothing
        Me.LabelRecentPost.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelRecentPost, "LabelRecentPost")
        Me.LabelRecentPost.Font = Nothing
        Me.LabelRecentPost.Name = "LabelRecentPost"
        Me.ToolTip1.SetToolTip(Me.LabelRecentPost, resources.GetString("LabelRecentPost.ToolTip"))
        '
        'RecentPostBrowser
        '
        Me.RecentPostBrowser.AccessibleDescription = Nothing
        Me.RecentPostBrowser.AccessibleName = Nothing
        Me.RecentPostBrowser.AllowWebBrowserDrop = False
        resources.ApplyResources(Me.RecentPostBrowser, "RecentPostBrowser")
        Me.RecentPostBrowser.ContextMenuStrip = Me.ContextMenuStrip1
        Me.RecentPostBrowser.IsWebBrowserContextMenuEnabled = False
        Me.RecentPostBrowser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.RecentPostBrowser.Name = "RecentPostBrowser"
        Me.ToolTip1.SetToolTip(Me.RecentPostBrowser, resources.GetString("RecentPostBrowser.ToolTip"))
        Me.RecentPostBrowser.Url = New System.Uri("about:blank", System.UriKind.Absolute)
        Me.RecentPostBrowser.WebBrowserShortcutsEnabled = False
        '
        'DescriptionBrowser
        '
        Me.DescriptionBrowser.AccessibleDescription = Nothing
        Me.DescriptionBrowser.AccessibleName = Nothing
        Me.DescriptionBrowser.AllowWebBrowserDrop = False
        resources.ApplyResources(Me.DescriptionBrowser, "DescriptionBrowser")
        Me.DescriptionBrowser.ContextMenuStrip = Me.ContextMenuStrip1
        Me.DescriptionBrowser.IsWebBrowserContextMenuEnabled = False
        Me.DescriptionBrowser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.DescriptionBrowser.Name = "DescriptionBrowser"
        Me.ToolTip1.SetToolTip(Me.DescriptionBrowser, resources.GetString("DescriptionBrowser.ToolTip"))
        Me.DescriptionBrowser.Url = New System.Uri("about:blank", System.UriKind.Absolute)
        Me.DescriptionBrowser.WebBrowserShortcutsEnabled = False
        '
        'LabelIsVerified
        '
        Me.LabelIsVerified.AccessibleDescription = Nothing
        Me.LabelIsVerified.AccessibleName = Nothing
        resources.ApplyResources(Me.LabelIsVerified, "LabelIsVerified")
        Me.LabelIsVerified.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LabelIsVerified.Font = Nothing
        Me.LabelIsVerified.Name = "LabelIsVerified"
        Me.ToolTip1.SetToolTip(Me.LabelIsVerified, resources.GetString("LabelIsVerified.ToolTip"))
        '
        'Label13
        '
        Me.Label13.AccessibleDescription = Nothing
        Me.Label13.AccessibleName = Nothing
        resources.ApplyResources(Me.Label13, "Label13")
        Me.Label13.Font = Nothing
        Me.Label13.Name = "Label13"
        Me.ToolTip1.SetToolTip(Me.Label13, resources.GetString("Label13.ToolTip"))
        '
        'ShowUserInfo
        '
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Nothing
        Me.CancelButton = Me.ButtonClose
        Me.Controls.Add(Me.LabelIsVerified)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.RecentPostBrowser)
        Me.Controls.Add(Me.UserPicture)
        Me.Controls.Add(Me.DescriptionBrowser)
        Me.Controls.Add(Me.LabelScreenName)
        Me.Controls.Add(Me.LabelRecentPost)
        Me.Controls.Add(Me.LinkLabelFav)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.LabelIsProtected)
        Me.Controls.Add(Me.LabelCreatedAt)
        Me.Controls.Add(Me.LabelIsFollowing)
        Me.Controls.Add(Me.LabelIsFollowed)
        Me.Controls.Add(Me.LinkLabelTweet)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.LinkLabelFollowers)
        Me.Controls.Add(Me.ButtonUnFollow)
        Me.Controls.Add(Me.LinkLabelFollowing)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.LabelName)
        Me.Controls.Add(Me.ButtonFollow)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.LabelLocation)
        Me.Controls.Add(Me.LinkLabelWeb)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ButtonClose)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ShowUserInfo"
        Me.ShowIcon = False
        Me.ToolTip1.SetToolTip(Me, resources.GetString("$this.ToolTip"))
        Me.TopMost = True
        CType(Me.UserPicture, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ButtonClose As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents LinkLabelWeb As System.Windows.Forms.LinkLabel
    Friend WithEvents LabelLocation As System.Windows.Forms.Label
    Friend WithEvents LabelName As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents LinkLabelFollowing As System.Windows.Forms.LinkLabel
    Friend WithEvents LinkLabelFollowers As System.Windows.Forms.LinkLabel
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents LabelCreatedAt As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents LinkLabelTweet As System.Windows.Forms.LinkLabel
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents LinkLabelFav As System.Windows.Forms.LinkLabel
    Friend WithEvents ButtonFollow As System.Windows.Forms.Button
    Friend WithEvents ButtonUnFollow As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents LabelIsProtected As System.Windows.Forms.Label
    Friend WithEvents LabelIsFollowing As System.Windows.Forms.Label
    Friend WithEvents LabelIsFollowed As System.Windows.Forms.Label
    Friend WithEvents UserPicture As System.Windows.Forms.PictureBox
    Friend WithEvents BackgroundWorkerImageLoader As System.ComponentModel.BackgroundWorker
    Friend WithEvents LabelScreenName As System.Windows.Forms.Label
    Friend WithEvents DescriptionBrowser As System.Windows.Forms.WebBrowser
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents SelectionCopyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SelectAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LabelRecentPost As System.Windows.Forms.Label
    Friend WithEvents RecentPostBrowser As System.Windows.Forms.WebBrowser
    Friend WithEvents LabelIsVerified As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
End Class
