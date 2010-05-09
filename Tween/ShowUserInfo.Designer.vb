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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ShowUserInfo))
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LinkLabelWeb = New System.Windows.Forms.LinkLabel()
        Me.LabelLocation = New System.Windows.Forms.Label()
        Me.LabelName = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.LinkLabelFollowing = New System.Windows.Forms.LinkLabel()
        Me.LinkLabelFollowers = New System.Windows.Forms.LinkLabel()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.LabelCreatedAt = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.LinkLabelTweet = New System.Windows.Forms.LinkLabel()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.LinkLabelFav = New System.Windows.Forms.LinkLabel()
        Me.ButtonFollow = New System.Windows.Forms.Button()
        Me.ButtonUnFollow = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.LabelIsProtected = New System.Windows.Forms.Label()
        Me.LabelIsFollowing = New System.Windows.Forms.Label()
        Me.LabelIsFollowed = New System.Windows.Forms.Label()
        Me.UserPicture = New System.Windows.Forms.PictureBox()
        Me.BackgroundWorkerImageLoader = New System.ComponentModel.BackgroundWorker()
        Me.LabelScreenName = New System.Windows.Forms.Label()
        Me.DescriptionBrowser = New System.Windows.Forms.WebBrowser()
        CType(Me.UserPicture, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ButtonClose
        '
        resources.ApplyResources(Me.ButtonClose, "ButtonClose")
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.UseVisualStyleBackColor = True
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        Me.Label1.UseMnemonic = False
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'LinkLabelWeb
        '
        resources.ApplyResources(Me.LinkLabelWeb, "LinkLabelWeb")
        Me.LinkLabelWeb.Name = "LinkLabelWeb"
        Me.LinkLabelWeb.TabStop = True
        Me.LinkLabelWeb.UseMnemonic = False
        '
        'LabelLocation
        '
        resources.ApplyResources(Me.LabelLocation, "LabelLocation")
        Me.LabelLocation.Name = "LabelLocation"
        Me.LabelLocation.UseMnemonic = False
        '
        'LabelName
        '
        resources.ApplyResources(Me.LabelName, "LabelName")
        Me.LabelName.Name = "LabelName"
        Me.LabelName.UseMnemonic = False
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'LinkLabelFollowing
        '
        resources.ApplyResources(Me.LinkLabelFollowing, "LinkLabelFollowing")
        Me.LinkLabelFollowing.Name = "LinkLabelFollowing"
        Me.LinkLabelFollowing.TabStop = True
        '
        'LinkLabelFollowers
        '
        resources.ApplyResources(Me.LinkLabelFollowers, "LinkLabelFollowers")
        Me.LinkLabelFollowers.Name = "LinkLabelFollowers"
        Me.LinkLabelFollowers.TabStop = True
        '
        'Label7
        '
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Name = "Label7"
        '
        'LabelCreatedAt
        '
        resources.ApplyResources(Me.LabelCreatedAt, "LabelCreatedAt")
        Me.LabelCreatedAt.Name = "LabelCreatedAt"
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'LinkLabelTweet
        '
        resources.ApplyResources(Me.LinkLabelTweet, "LinkLabelTweet")
        Me.LinkLabelTweet.Name = "LinkLabelTweet"
        Me.LinkLabelTweet.TabStop = True
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        '
        'LinkLabelFav
        '
        resources.ApplyResources(Me.LinkLabelFav, "LinkLabelFav")
        Me.LinkLabelFav.Name = "LinkLabelFav"
        Me.LinkLabelFav.TabStop = True
        '
        'ButtonFollow
        '
        resources.ApplyResources(Me.ButtonFollow, "ButtonFollow")
        Me.ButtonFollow.Name = "ButtonFollow"
        Me.ButtonFollow.UseVisualStyleBackColor = True
        '
        'ButtonUnFollow
        '
        resources.ApplyResources(Me.ButtonUnFollow, "ButtonUnFollow")
        Me.ButtonUnFollow.Name = "ButtonUnFollow"
        Me.ButtonUnFollow.UseVisualStyleBackColor = True
        '
        'Label10
        '
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Name = "Label10"
        '
        'LabelIsProtected
        '
        resources.ApplyResources(Me.LabelIsProtected, "LabelIsProtected")
        Me.LabelIsProtected.Name = "LabelIsProtected"
        '
        'LabelIsFollowing
        '
        resources.ApplyResources(Me.LabelIsFollowing, "LabelIsFollowing")
        Me.LabelIsFollowing.Name = "LabelIsFollowing"
        '
        'LabelIsFollowed
        '
        resources.ApplyResources(Me.LabelIsFollowed, "LabelIsFollowed")
        Me.LabelIsFollowed.Name = "LabelIsFollowed"
        '
        'UserPicture
        '
        Me.UserPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.UserPicture, "UserPicture")
        Me.UserPicture.Name = "UserPicture"
        Me.UserPicture.TabStop = False
        '
        'BackgroundWorkerImageLoader
        '
        '
        'LabelScreenName
        '
        resources.ApplyResources(Me.LabelScreenName, "LabelScreenName")
        Me.LabelScreenName.Name = "LabelScreenName"
        '
        'DescriptionBrowser
        '
        Me.DescriptionBrowser.AllowWebBrowserDrop = False
        Me.DescriptionBrowser.IsWebBrowserContextMenuEnabled = False
        resources.ApplyResources(Me.DescriptionBrowser, "DescriptionBrowser")
        Me.DescriptionBrowser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.DescriptionBrowser.Name = "DescriptionBrowser"
        Me.DescriptionBrowser.Url = New System.Uri("", System.UriKind.Relative)
        Me.DescriptionBrowser.WebBrowserShortcutsEnabled = False
        '
        'ShowUserInfo
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.DescriptionBrowser)
        Me.Controls.Add(Me.LabelScreenName)
        Me.Controls.Add(Me.UserPicture)
        Me.Controls.Add(Me.LabelIsFollowed)
        Me.Controls.Add(Me.LabelIsFollowing)
        Me.Controls.Add(Me.LabelIsProtected)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.ButtonUnFollow)
        Me.Controls.Add(Me.ButtonFollow)
        Me.Controls.Add(Me.LinkLabelFav)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.LinkLabelTweet)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.LabelCreatedAt)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.LinkLabelFollowers)
        Me.Controls.Add(Me.LinkLabelFollowing)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.LabelName)
        Me.Controls.Add(Me.LabelLocation)
        Me.Controls.Add(Me.LinkLabelWeb)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ButtonClose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ShowUserInfo"
        Me.ShowIcon = False
        Me.TopMost = True
        CType(Me.UserPicture, System.ComponentModel.ISupportInitialize).EndInit()
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
End Class
