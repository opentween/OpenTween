Option Strict On
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TweenMain
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TweenMain))
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.StatusLabelUrl = New System.Windows.Forms.ToolStripStatusLabel
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.ListTab = New System.Windows.Forms.TabControl
        Me.ContextMenuTabProperty = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.AddTabMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TabRenameMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator20 = New System.Windows.Forms.ToolStripSeparator
        Me.UreadManageMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NotifyDispMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SoundFileComboBox = New System.Windows.Forms.ToolStripComboBox
        Me.ToolStripSeparator18 = New System.Windows.Forms.ToolStripSeparator
        Me.FilterEditMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator19 = New System.Windows.Forms.ToolStripSeparator
        Me.ClearTabMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator11 = New System.Windows.Forms.ToolStripSeparator
        Me.DeleteTabMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TabImage = New System.Windows.Forms.ImageList(Me.components)
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.UserPicture = New System.Windows.Forms.PictureBox
        Me.ContextMenuStrip3 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.IconNameToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator
        Me.SaveIconPictureToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NameLabel = New System.Windows.Forms.Label
        Me.PostBrowser = New System.Windows.Forms.WebBrowser
        Me.ContextMenuStrip4 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem
        Me.SearchItem2ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SearchItem1ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SearchItem3ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SearchItem4ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.CurrentTabToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator13 = New System.Windows.Forms.ToolStripSeparator
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
        Me.FollowContextMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RemoveContextMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.FriendshipContextMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DateTimeLabel = New System.Windows.Forms.Label
        Me.StatusText = New System.Windows.Forms.TextBox
        Me.lblLen = New System.Windows.Forms.Label
        Me.PostButton = New System.Windows.Forms.Button
        Me.ButtonPostMode = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.MenuItemFile = New System.Windows.Forms.ToolStripMenuItem
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SettingStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator
        Me.SaveLogMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator17 = New System.Windows.Forms.ToolStripSeparator
        Me.NewPostPopMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.PlaySoundMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ListLockMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator15 = New System.Windows.Forms.ToolStripSeparator
        Me.MultiLineMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator21 = New System.Windows.Forms.ToolStripSeparator
        Me.EndToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuItemEdit = New System.Windows.Forms.ToolStripMenuItem
        Me.CopySTOTMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.CopyURLMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator
        Me.MenuItemSubSearch = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuItemSearchNext = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuItemSearchPrev = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuItemOperate = New System.Windows.Forms.ToolStripMenuItem
        Me.ContextMenuStrip2 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ReplyStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ReplyAllStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DMStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ReTweetOriginalStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ReTweetStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.QuoteStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.FavAddToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.FavRemoveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem6 = New System.Windows.Forms.ToolStripMenuItem
        Me.MoveToHomeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MoveToFavToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusOpenMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RepliedStatusOpenMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.FavorareMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.OpenURLMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem7 = New System.Windows.Forms.ToolStripMenuItem
        Me.TabMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.IDRuleMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
        Me.ToolStripMenuItem11 = New System.Windows.Forms.ToolStripMenuItem
        Me.ReadedStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.UnreadStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.JumpUnreadMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator
        Me.SelectAllMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DeleteStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RefreshStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RefreshMoreStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuItemTab = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuItemCommand = New System.Windows.Forms.ToolStripMenuItem
        Me.TinyUrlConvertToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.UrlConvertAutoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.UrlUndoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TinyURLToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.IsgdToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TwurlnlToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.UnuToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.BitlyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.JmpStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.UpdateFollowersMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.GetFollowersDiffToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.GetFollowersAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem10 = New System.Windows.Forms.ToolStripMenuItem
        Me.BlackFavAddToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.FollowCommandMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RemoveCommandMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.FriendshipMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
        Me.OwnStatusMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuItemHelp = New System.Windows.Forms.ToolStripMenuItem
        Me.MatomeMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator16 = New System.Windows.Forms.ToolStripSeparator
        Me.VerUpMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.WedataMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator14 = New System.Windows.Forms.ToolStripSeparator
        Me.ApiInfoMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.InfoTwitterMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator
        Me.AboutMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DebugModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DumpPostClassToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TraceOutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ContextMenuStripPostMode = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItemUrlMultibyteSplit = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItemApiCommandEvasion = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItemUrlAutoShorten = New System.Windows.Forms.ToolStripMenuItem
        Me.IdeographicSpaceToSpaceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator
        Me.HashSelectComboBox = New System.Windows.Forms.ToolStripComboBox
        Me.TimerTimeline = New System.Windows.Forms.Timer(Me.components)
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.TimerColorize = New System.Windows.Forms.Timer(Me.components)
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog
        Me.TimerRefreshIcon = New System.Windows.Forms.Timer(Me.components)
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.ToolStripContainer1.BottomToolStripPanel.SuspendLayout()
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.TopToolStripPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ContextMenuTabProperty.SuspendLayout()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.UserPicture, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip3.SuspendLayout()
        Me.ContextMenuStrip4.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.ContextMenuStrip2.SuspendLayout()
        Me.ContextMenuStripPostMode.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStripContainer1
        '
        '
        'ToolStripContainer1.BottomToolStripPanel
        '
        Me.ToolStripContainer1.BottomToolStripPanel.Controls.Add(Me.StatusStrip1)
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.SplitContainer1)
        resources.ApplyResources(Me.ToolStripContainer1.ContentPanel, "ToolStripContainer1.ContentPanel")
        resources.ApplyResources(Me.ToolStripContainer1, "ToolStripContainer1")
        Me.ToolStripContainer1.LeftToolStripPanelVisible = False
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        Me.ToolStripContainer1.RightToolStripPanelVisible = False
        '
        'ToolStripContainer1.TopToolStripPanel
        '
        Me.ToolStripContainer1.TopToolStripPanel.Controls.Add(Me.MenuStrip1)
        '
        'StatusStrip1
        '
        resources.ApplyResources(Me.StatusStrip1, "StatusStrip1")
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabelUrl})
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.ShowItemToolTips = True
        '
        'StatusLabelUrl
        '
        Me.StatusLabelUrl.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        Me.StatusLabelUrl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.StatusLabelUrl.Name = "StatusLabelUrl"
        resources.ApplyResources(Me.StatusLabelUrl, "StatusLabelUrl")
        Me.StatusLabelUrl.Spring = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.ListTab)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.TabStop = False
        '
        'ListTab
        '
        resources.ApplyResources(Me.ListTab, "ListTab")
        Me.ListTab.AllowDrop = True
        Me.ListTab.ContextMenuStrip = Me.ContextMenuTabProperty
        Me.ListTab.ImageList = Me.TabImage
        Me.ListTab.Multiline = True
        Me.ListTab.Name = "ListTab"
        Me.ListTab.SelectedIndex = 0
        Me.ListTab.TabStop = False
        '
        'ContextMenuTabProperty
        '
        Me.ContextMenuTabProperty.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddTabMenuItem, Me.TabRenameMenuItem, Me.ToolStripSeparator20, Me.UreadManageMenuItem, Me.NotifyDispMenuItem, Me.SoundFileComboBox, Me.ToolStripSeparator18, Me.FilterEditMenuItem, Me.ToolStripSeparator19, Me.ClearTabMenuItem, Me.ToolStripSeparator11, Me.DeleteTabMenuItem})
        Me.ContextMenuTabProperty.Name = "ContextMenuStrip3"
        Me.ContextMenuTabProperty.OwnerItem = Me.MenuItemTab
        Me.ContextMenuTabProperty.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        resources.ApplyResources(Me.ContextMenuTabProperty, "ContextMenuTabProperty")
        '
        'AddTabMenuItem
        '
        Me.AddTabMenuItem.Name = "AddTabMenuItem"
        resources.ApplyResources(Me.AddTabMenuItem, "AddTabMenuItem")
        '
        'TabRenameMenuItem
        '
        Me.TabRenameMenuItem.Name = "TabRenameMenuItem"
        resources.ApplyResources(Me.TabRenameMenuItem, "TabRenameMenuItem")
        '
        'ToolStripSeparator20
        '
        Me.ToolStripSeparator20.Name = "ToolStripSeparator20"
        resources.ApplyResources(Me.ToolStripSeparator20, "ToolStripSeparator20")
        '
        'UreadManageMenuItem
        '
        Me.UreadManageMenuItem.CheckOnClick = True
        Me.UreadManageMenuItem.Name = "UreadManageMenuItem"
        resources.ApplyResources(Me.UreadManageMenuItem, "UreadManageMenuItem")
        '
        'NotifyDispMenuItem
        '
        Me.NotifyDispMenuItem.CheckOnClick = True
        Me.NotifyDispMenuItem.Name = "NotifyDispMenuItem"
        resources.ApplyResources(Me.NotifyDispMenuItem, "NotifyDispMenuItem")
        '
        'SoundFileComboBox
        '
        Me.SoundFileComboBox.AutoToolTip = True
        Me.SoundFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.SoundFileComboBox.Name = "SoundFileComboBox"
        resources.ApplyResources(Me.SoundFileComboBox, "SoundFileComboBox")
        '
        'ToolStripSeparator18
        '
        Me.ToolStripSeparator18.Name = "ToolStripSeparator18"
        resources.ApplyResources(Me.ToolStripSeparator18, "ToolStripSeparator18")
        '
        'FilterEditMenuItem
        '
        Me.FilterEditMenuItem.Name = "FilterEditMenuItem"
        resources.ApplyResources(Me.FilterEditMenuItem, "FilterEditMenuItem")
        '
        'ToolStripSeparator19
        '
        Me.ToolStripSeparator19.Name = "ToolStripSeparator19"
        resources.ApplyResources(Me.ToolStripSeparator19, "ToolStripSeparator19")
        '
        'ClearTabMenuItem
        '
        Me.ClearTabMenuItem.Name = "ClearTabMenuItem"
        resources.ApplyResources(Me.ClearTabMenuItem, "ClearTabMenuItem")
        '
        'ToolStripSeparator11
        '
        Me.ToolStripSeparator11.Name = "ToolStripSeparator11"
        resources.ApplyResources(Me.ToolStripSeparator11, "ToolStripSeparator11")
        '
        'DeleteTabMenuItem
        '
        Me.DeleteTabMenuItem.Name = "DeleteTabMenuItem"
        resources.ApplyResources(Me.DeleteTabMenuItem, "DeleteTabMenuItem")
        '
        'TabImage
        '
        Me.TabImage.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
        resources.ApplyResources(Me.TabImage, "TabImage")
        Me.TabImage.TransparentColor = System.Drawing.Color.Transparent
        '
        'SplitContainer2
        '
        resources.ApplyResources(Me.SplitContainer2, "SplitContainer2")
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer2.MinimumSize = New System.Drawing.Size(0, 22)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.TableLayoutPanel1)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.StatusText)
        Me.SplitContainer2.Panel2.Controls.Add(Me.lblLen)
        Me.SplitContainer2.Panel2.Controls.Add(Me.PostButton)
        Me.SplitContainer2.Panel2.Controls.Add(Me.ButtonPostMode)
        Me.SplitContainer2.TabStop = False
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.UserPicture, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.NameLabel, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.PostBrowser, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.DateTimeLabel, 2, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'UserPicture
        '
        Me.UserPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.UserPicture.ContextMenuStrip = Me.ContextMenuStrip3
        resources.ApplyResources(Me.UserPicture, "UserPicture")
        Me.UserPicture.Name = "UserPicture"
        Me.TableLayoutPanel1.SetRowSpan(Me.UserPicture, 2)
        Me.UserPicture.TabStop = False
        '
        'ContextMenuStrip3
        '
        Me.ContextMenuStrip3.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.IconNameToolStripMenuItem, Me.ToolStripMenuItem1, Me.SaveIconPictureToolStripMenuItem})
        Me.ContextMenuStrip3.Name = "ContextMenuStrip3"
        Me.ContextMenuStrip3.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        resources.ApplyResources(Me.ContextMenuStrip3, "ContextMenuStrip3")
        '
        'IconNameToolStripMenuItem
        '
        Me.IconNameToolStripMenuItem.Name = "IconNameToolStripMenuItem"
        resources.ApplyResources(Me.IconNameToolStripMenuItem, "IconNameToolStripMenuItem")
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        resources.ApplyResources(Me.ToolStripMenuItem1, "ToolStripMenuItem1")
        '
        'SaveIconPictureToolStripMenuItem
        '
        Me.SaveIconPictureToolStripMenuItem.Name = "SaveIconPictureToolStripMenuItem"
        resources.ApplyResources(Me.SaveIconPictureToolStripMenuItem, "SaveIconPictureToolStripMenuItem")
        '
        'NameLabel
        '
        resources.ApplyResources(Me.NameLabel, "NameLabel")
        Me.NameLabel.Name = "NameLabel"
        '
        'PostBrowser
        '
        Me.PostBrowser.AllowWebBrowserDrop = False
        Me.TableLayoutPanel1.SetColumnSpan(Me.PostBrowser, 2)
        Me.PostBrowser.ContextMenuStrip = Me.ContextMenuStrip4
        resources.ApplyResources(Me.PostBrowser, "PostBrowser")
        Me.PostBrowser.IsWebBrowserContextMenuEnabled = False
        Me.PostBrowser.Name = "PostBrowser"
        Me.PostBrowser.ScriptErrorsSuppressed = True
        Me.PostBrowser.TabStop = False
        Me.PostBrowser.Url = New System.Uri("", System.UriKind.Relative)
        '
        'ContextMenuStrip4
        '
        Me.ContextMenuStrip4.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem2, Me.ToolStripSeparator13, Me.ToolStripMenuItem3, Me.ToolStripMenuItem4, Me.ToolStripMenuItem5, Me.ToolStripSeparator5, Me.FollowContextMenuItem, Me.RemoveContextMenuItem, Me.FriendshipContextMenuItem})
        Me.ContextMenuStrip4.Name = "ContextMenuStrip4"
        resources.ApplyResources(Me.ContextMenuStrip4, "ContextMenuStrip4")
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SearchItem2ToolStripMenuItem, Me.SearchItem1ToolStripMenuItem, Me.SearchItem3ToolStripMenuItem, Me.SearchItem4ToolStripMenuItem, Me.CurrentTabToolStripMenuItem})
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        resources.ApplyResources(Me.ToolStripMenuItem2, "ToolStripMenuItem2")
        '
        'SearchItem2ToolStripMenuItem
        '
        Me.SearchItem2ToolStripMenuItem.Name = "SearchItem2ToolStripMenuItem"
        resources.ApplyResources(Me.SearchItem2ToolStripMenuItem, "SearchItem2ToolStripMenuItem")
        '
        'SearchItem1ToolStripMenuItem
        '
        Me.SearchItem1ToolStripMenuItem.Name = "SearchItem1ToolStripMenuItem"
        resources.ApplyResources(Me.SearchItem1ToolStripMenuItem, "SearchItem1ToolStripMenuItem")
        '
        'SearchItem3ToolStripMenuItem
        '
        Me.SearchItem3ToolStripMenuItem.Name = "SearchItem3ToolStripMenuItem"
        resources.ApplyResources(Me.SearchItem3ToolStripMenuItem, "SearchItem3ToolStripMenuItem")
        '
        'SearchItem4ToolStripMenuItem
        '
        Me.SearchItem4ToolStripMenuItem.Name = "SearchItem4ToolStripMenuItem"
        resources.ApplyResources(Me.SearchItem4ToolStripMenuItem, "SearchItem4ToolStripMenuItem")
        '
        'CurrentTabToolStripMenuItem
        '
        Me.CurrentTabToolStripMenuItem.Name = "CurrentTabToolStripMenuItem"
        resources.ApplyResources(Me.CurrentTabToolStripMenuItem, "CurrentTabToolStripMenuItem")
        '
        'ToolStripSeparator13
        '
        Me.ToolStripSeparator13.Name = "ToolStripSeparator13"
        resources.ApplyResources(Me.ToolStripSeparator13, "ToolStripSeparator13")
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        resources.ApplyResources(Me.ToolStripMenuItem3, "ToolStripMenuItem3")
        '
        'ToolStripMenuItem4
        '
        resources.ApplyResources(Me.ToolStripMenuItem4, "ToolStripMenuItem4")
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        resources.ApplyResources(Me.ToolStripMenuItem5, "ToolStripMenuItem5")
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        resources.ApplyResources(Me.ToolStripSeparator5, "ToolStripSeparator5")
        '
        'FollowContextMenuItem
        '
        Me.FollowContextMenuItem.Name = "FollowContextMenuItem"
        resources.ApplyResources(Me.FollowContextMenuItem, "FollowContextMenuItem")
        '
        'RemoveContextMenuItem
        '
        Me.RemoveContextMenuItem.Name = "RemoveContextMenuItem"
        resources.ApplyResources(Me.RemoveContextMenuItem, "RemoveContextMenuItem")
        '
        'FriendshipContextMenuItem
        '
        Me.FriendshipContextMenuItem.Name = "FriendshipContextMenuItem"
        resources.ApplyResources(Me.FriendshipContextMenuItem, "FriendshipContextMenuItem")
        '
        'DateTimeLabel
        '
        resources.ApplyResources(Me.DateTimeLabel, "DateTimeLabel")
        Me.DateTimeLabel.Name = "DateTimeLabel"
        '
        'StatusText
        '
        resources.ApplyResources(Me.StatusText, "StatusText")
        Me.StatusText.Name = "StatusText"
        '
        'lblLen
        '
        resources.ApplyResources(Me.lblLen, "lblLen")
        Me.lblLen.Name = "lblLen"
        '
        'PostButton
        '
        resources.ApplyResources(Me.PostButton, "PostButton")
        Me.PostButton.Name = "PostButton"
        Me.PostButton.TabStop = False
        Me.PostButton.UseVisualStyleBackColor = True
        '
        'ButtonPostMode
        '
        resources.ApplyResources(Me.ButtonPostMode, "ButtonPostMode")
        Me.ButtonPostMode.Name = "ButtonPostMode"
        Me.ButtonPostMode.TabStop = False
        Me.ButtonPostMode.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        resources.ApplyResources(Me.MenuStrip1, "MenuStrip1")
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuItemFile, Me.MenuItemEdit, Me.MenuItemOperate, Me.MenuItemTab, Me.MenuItemCommand, Me.MenuItemHelp})
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        '
        'MenuItemFile
        '
        Me.MenuItemFile.DropDown = Me.ContextMenuStrip1
        Me.MenuItemFile.Name = "MenuItemFile"
        resources.ApplyResources(Me.MenuItemFile, "MenuItemFile")
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SettingStripMenuItem, Me.ToolStripSeparator9, Me.SaveLogMenuItem, Me.ToolStripSeparator17, Me.NewPostPopMenuItem, Me.PlaySoundMenuItem, Me.ListLockMenuItem, Me.ToolStripSeparator15, Me.MultiLineMenuItem, Me.ToolStripSeparator21, Me.EndToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.OwnerItem = Me.MenuItemFile
        Me.ContextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.ContextMenuStrip1.ShowCheckMargin = True
        Me.ContextMenuStrip1.ShowImageMargin = False
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        '
        'SettingStripMenuItem
        '
        Me.SettingStripMenuItem.Name = "SettingStripMenuItem"
        resources.ApplyResources(Me.SettingStripMenuItem, "SettingStripMenuItem")
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        resources.ApplyResources(Me.ToolStripSeparator9, "ToolStripSeparator9")
        '
        'SaveLogMenuItem
        '
        Me.SaveLogMenuItem.Name = "SaveLogMenuItem"
        resources.ApplyResources(Me.SaveLogMenuItem, "SaveLogMenuItem")
        '
        'ToolStripSeparator17
        '
        Me.ToolStripSeparator17.Name = "ToolStripSeparator17"
        resources.ApplyResources(Me.ToolStripSeparator17, "ToolStripSeparator17")
        '
        'NewPostPopMenuItem
        '
        Me.NewPostPopMenuItem.CheckOnClick = True
        Me.NewPostPopMenuItem.Name = "NewPostPopMenuItem"
        resources.ApplyResources(Me.NewPostPopMenuItem, "NewPostPopMenuItem")
        '
        'PlaySoundMenuItem
        '
        Me.PlaySoundMenuItem.CheckOnClick = True
        Me.PlaySoundMenuItem.Name = "PlaySoundMenuItem"
        resources.ApplyResources(Me.PlaySoundMenuItem, "PlaySoundMenuItem")
        '
        'ListLockMenuItem
        '
        Me.ListLockMenuItem.CheckOnClick = True
        Me.ListLockMenuItem.Name = "ListLockMenuItem"
        resources.ApplyResources(Me.ListLockMenuItem, "ListLockMenuItem")
        '
        'ToolStripSeparator15
        '
        Me.ToolStripSeparator15.Name = "ToolStripSeparator15"
        resources.ApplyResources(Me.ToolStripSeparator15, "ToolStripSeparator15")
        '
        'MultiLineMenuItem
        '
        Me.MultiLineMenuItem.CheckOnClick = True
        Me.MultiLineMenuItem.Name = "MultiLineMenuItem"
        resources.ApplyResources(Me.MultiLineMenuItem, "MultiLineMenuItem")
        '
        'ToolStripSeparator21
        '
        Me.ToolStripSeparator21.Name = "ToolStripSeparator21"
        resources.ApplyResources(Me.ToolStripSeparator21, "ToolStripSeparator21")
        '
        'EndToolStripMenuItem
        '
        Me.EndToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.EndToolStripMenuItem.Name = "EndToolStripMenuItem"
        resources.ApplyResources(Me.EndToolStripMenuItem, "EndToolStripMenuItem")
        '
        'MenuItemEdit
        '
        Me.MenuItemEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopySTOTMenuItem, Me.CopyURLMenuItem, Me.ToolStripSeparator6, Me.MenuItemSubSearch, Me.MenuItemSearchNext, Me.MenuItemSearchPrev})
        Me.MenuItemEdit.Name = "MenuItemEdit"
        resources.ApplyResources(Me.MenuItemEdit, "MenuItemEdit")
        '
        'CopySTOTMenuItem
        '
        Me.CopySTOTMenuItem.Name = "CopySTOTMenuItem"
        resources.ApplyResources(Me.CopySTOTMenuItem, "CopySTOTMenuItem")
        '
        'CopyURLMenuItem
        '
        Me.CopyURLMenuItem.Name = "CopyURLMenuItem"
        resources.ApplyResources(Me.CopyURLMenuItem, "CopyURLMenuItem")
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        resources.ApplyResources(Me.ToolStripSeparator6, "ToolStripSeparator6")
        '
        'MenuItemSubSearch
        '
        Me.MenuItemSubSearch.Name = "MenuItemSubSearch"
        resources.ApplyResources(Me.MenuItemSubSearch, "MenuItemSubSearch")
        '
        'MenuItemSearchNext
        '
        Me.MenuItemSearchNext.Name = "MenuItemSearchNext"
        resources.ApplyResources(Me.MenuItemSearchNext, "MenuItemSearchNext")
        '
        'MenuItemSearchPrev
        '
        Me.MenuItemSearchPrev.Name = "MenuItemSearchPrev"
        resources.ApplyResources(Me.MenuItemSearchPrev, "MenuItemSearchPrev")
        '
        'MenuItemOperate
        '
        Me.MenuItemOperate.DropDown = Me.ContextMenuStrip2
        Me.MenuItemOperate.Name = "MenuItemOperate"
        resources.ApplyResources(Me.MenuItemOperate, "MenuItemOperate")
        '
        'ContextMenuStrip2
        '
        Me.ContextMenuStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ReplyStripMenuItem, Me.ReplyAllStripMenuItem, Me.DMStripMenuItem, Me.ReTweetOriginalStripMenuItem, Me.ReTweetStripMenuItem, Me.QuoteStripMenuItem, Me.ToolStripSeparator2, Me.FavAddToolStripMenuItem, Me.FavRemoveToolStripMenuItem, Me.ToolStripMenuItem6, Me.ToolStripMenuItem7, Me.ToolStripSeparator4, Me.ToolStripMenuItem11, Me.JumpUnreadMenuItem, Me.ToolStripSeparator10, Me.SelectAllMenuItem, Me.DeleteStripMenuItem, Me.RefreshStripMenuItem, Me.RefreshMoreStripMenuItem})
        Me.ContextMenuStrip2.Name = "ContextMenuStrip2"
        Me.ContextMenuStrip2.OwnerItem = Me.MenuItemOperate
        Me.ContextMenuStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        resources.ApplyResources(Me.ContextMenuStrip2, "ContextMenuStrip2")
        '
        'ReplyStripMenuItem
        '
        Me.ReplyStripMenuItem.Name = "ReplyStripMenuItem"
        resources.ApplyResources(Me.ReplyStripMenuItem, "ReplyStripMenuItem")
        '
        'ReplyAllStripMenuItem
        '
        Me.ReplyAllStripMenuItem.Name = "ReplyAllStripMenuItem"
        resources.ApplyResources(Me.ReplyAllStripMenuItem, "ReplyAllStripMenuItem")
        '
        'DMStripMenuItem
        '
        Me.DMStripMenuItem.Name = "DMStripMenuItem"
        resources.ApplyResources(Me.DMStripMenuItem, "DMStripMenuItem")
        '
        'ReTweetOriginalStripMenuItem
        '
        Me.ReTweetOriginalStripMenuItem.Name = "ReTweetOriginalStripMenuItem"
        resources.ApplyResources(Me.ReTweetOriginalStripMenuItem, "ReTweetOriginalStripMenuItem")
        '
        'ReTweetStripMenuItem
        '
        Me.ReTweetStripMenuItem.Name = "ReTweetStripMenuItem"
        resources.ApplyResources(Me.ReTweetStripMenuItem, "ReTweetStripMenuItem")
        '
        'QuoteStripMenuItem
        '
        Me.QuoteStripMenuItem.Name = "QuoteStripMenuItem"
        resources.ApplyResources(Me.QuoteStripMenuItem, "QuoteStripMenuItem")
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
        '
        'FavAddToolStripMenuItem
        '
        Me.FavAddToolStripMenuItem.Name = "FavAddToolStripMenuItem"
        resources.ApplyResources(Me.FavAddToolStripMenuItem, "FavAddToolStripMenuItem")
        '
        'FavRemoveToolStripMenuItem
        '
        Me.FavRemoveToolStripMenuItem.Name = "FavRemoveToolStripMenuItem"
        resources.ApplyResources(Me.FavRemoveToolStripMenuItem, "FavRemoveToolStripMenuItem")
        '
        'ToolStripMenuItem6
        '
        Me.ToolStripMenuItem6.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MoveToHomeToolStripMenuItem, Me.MoveToFavToolStripMenuItem, Me.StatusOpenMenuItem, Me.RepliedStatusOpenMenuItem, Me.FavorareMenuItem, Me.OpenURLMenuItem})
        Me.ToolStripMenuItem6.Name = "ToolStripMenuItem6"
        resources.ApplyResources(Me.ToolStripMenuItem6, "ToolStripMenuItem6")
        '
        'MoveToHomeToolStripMenuItem
        '
        Me.MoveToHomeToolStripMenuItem.Name = "MoveToHomeToolStripMenuItem"
        resources.ApplyResources(Me.MoveToHomeToolStripMenuItem, "MoveToHomeToolStripMenuItem")
        '
        'MoveToFavToolStripMenuItem
        '
        Me.MoveToFavToolStripMenuItem.Name = "MoveToFavToolStripMenuItem"
        resources.ApplyResources(Me.MoveToFavToolStripMenuItem, "MoveToFavToolStripMenuItem")
        '
        'StatusOpenMenuItem
        '
        Me.StatusOpenMenuItem.Name = "StatusOpenMenuItem"
        resources.ApplyResources(Me.StatusOpenMenuItem, "StatusOpenMenuItem")
        '
        'RepliedStatusOpenMenuItem
        '
        Me.RepliedStatusOpenMenuItem.Name = "RepliedStatusOpenMenuItem"
        resources.ApplyResources(Me.RepliedStatusOpenMenuItem, "RepliedStatusOpenMenuItem")
        '
        'FavorareMenuItem
        '
        Me.FavorareMenuItem.Name = "FavorareMenuItem"
        resources.ApplyResources(Me.FavorareMenuItem, "FavorareMenuItem")
        '
        'OpenURLMenuItem
        '
        Me.OpenURLMenuItem.Name = "OpenURLMenuItem"
        resources.ApplyResources(Me.OpenURLMenuItem, "OpenURLMenuItem")
        '
        'ToolStripMenuItem7
        '
        Me.ToolStripMenuItem7.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TabMenuItem, Me.IDRuleMenuItem})
        Me.ToolStripMenuItem7.Name = "ToolStripMenuItem7"
        resources.ApplyResources(Me.ToolStripMenuItem7, "ToolStripMenuItem7")
        '
        'TabMenuItem
        '
        Me.TabMenuItem.Name = "TabMenuItem"
        resources.ApplyResources(Me.TabMenuItem, "TabMenuItem")
        '
        'IDRuleMenuItem
        '
        Me.IDRuleMenuItem.Name = "IDRuleMenuItem"
        resources.ApplyResources(Me.IDRuleMenuItem, "IDRuleMenuItem")
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        resources.ApplyResources(Me.ToolStripSeparator4, "ToolStripSeparator4")
        '
        'ToolStripMenuItem11
        '
        Me.ToolStripMenuItem11.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ReadedStripMenuItem, Me.UnreadStripMenuItem})
        Me.ToolStripMenuItem11.Name = "ToolStripMenuItem11"
        resources.ApplyResources(Me.ToolStripMenuItem11, "ToolStripMenuItem11")
        '
        'ReadedStripMenuItem
        '
        Me.ReadedStripMenuItem.Name = "ReadedStripMenuItem"
        resources.ApplyResources(Me.ReadedStripMenuItem, "ReadedStripMenuItem")
        '
        'UnreadStripMenuItem
        '
        Me.UnreadStripMenuItem.Name = "UnreadStripMenuItem"
        resources.ApplyResources(Me.UnreadStripMenuItem, "UnreadStripMenuItem")
        '
        'JumpUnreadMenuItem
        '
        Me.JumpUnreadMenuItem.Name = "JumpUnreadMenuItem"
        resources.ApplyResources(Me.JumpUnreadMenuItem, "JumpUnreadMenuItem")
        '
        'ToolStripSeparator10
        '
        Me.ToolStripSeparator10.Name = "ToolStripSeparator10"
        resources.ApplyResources(Me.ToolStripSeparator10, "ToolStripSeparator10")
        '
        'SelectAllMenuItem
        '
        Me.SelectAllMenuItem.Name = "SelectAllMenuItem"
        resources.ApplyResources(Me.SelectAllMenuItem, "SelectAllMenuItem")
        '
        'DeleteStripMenuItem
        '
        Me.DeleteStripMenuItem.Name = "DeleteStripMenuItem"
        resources.ApplyResources(Me.DeleteStripMenuItem, "DeleteStripMenuItem")
        '
        'RefreshStripMenuItem
        '
        Me.RefreshStripMenuItem.Name = "RefreshStripMenuItem"
        resources.ApplyResources(Me.RefreshStripMenuItem, "RefreshStripMenuItem")
        '
        'RefreshMoreStripMenuItem
        '
        Me.RefreshMoreStripMenuItem.Name = "RefreshMoreStripMenuItem"
        resources.ApplyResources(Me.RefreshMoreStripMenuItem, "RefreshMoreStripMenuItem")
        '
        'MenuItemTab
        '
        Me.MenuItemTab.DropDown = Me.ContextMenuTabProperty
        Me.MenuItemTab.Name = "MenuItemTab"
        resources.ApplyResources(Me.MenuItemTab, "MenuItemTab")
        '
        'MenuItemCommand
        '
        Me.MenuItemCommand.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TinyUrlConvertToolStripMenuItem, Me.UpdateFollowersMenuItem1, Me.ToolStripMenuItem10, Me.ToolStripSeparator1, Me.FollowCommandMenuItem, Me.RemoveCommandMenuItem, Me.FriendshipMenuItem, Me.ToolStripSeparator3, Me.OwnStatusMenuItem})
        Me.MenuItemCommand.Name = "MenuItemCommand"
        resources.ApplyResources(Me.MenuItemCommand, "MenuItemCommand")
        '
        'TinyUrlConvertToolStripMenuItem
        '
        Me.TinyUrlConvertToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UrlConvertAutoToolStripMenuItem, Me.UrlUndoToolStripMenuItem, Me.TinyURLToolStripMenuItem, Me.IsgdToolStripMenuItem, Me.TwurlnlToolStripMenuItem, Me.UnuToolStripMenuItem, Me.BitlyToolStripMenuItem, Me.JmpStripMenuItem})
        Me.TinyUrlConvertToolStripMenuItem.Name = "TinyUrlConvertToolStripMenuItem"
        resources.ApplyResources(Me.TinyUrlConvertToolStripMenuItem, "TinyUrlConvertToolStripMenuItem")
        '
        'UrlConvertAutoToolStripMenuItem
        '
        Me.UrlConvertAutoToolStripMenuItem.Name = "UrlConvertAutoToolStripMenuItem"
        resources.ApplyResources(Me.UrlConvertAutoToolStripMenuItem, "UrlConvertAutoToolStripMenuItem")
        '
        'UrlUndoToolStripMenuItem
        '
        resources.ApplyResources(Me.UrlUndoToolStripMenuItem, "UrlUndoToolStripMenuItem")
        Me.UrlUndoToolStripMenuItem.Name = "UrlUndoToolStripMenuItem"
        '
        'TinyURLToolStripMenuItem
        '
        Me.TinyURLToolStripMenuItem.Name = "TinyURLToolStripMenuItem"
        resources.ApplyResources(Me.TinyURLToolStripMenuItem, "TinyURLToolStripMenuItem")
        '
        'IsgdToolStripMenuItem
        '
        Me.IsgdToolStripMenuItem.Name = "IsgdToolStripMenuItem"
        resources.ApplyResources(Me.IsgdToolStripMenuItem, "IsgdToolStripMenuItem")
        '
        'TwurlnlToolStripMenuItem
        '
        Me.TwurlnlToolStripMenuItem.Name = "TwurlnlToolStripMenuItem"
        resources.ApplyResources(Me.TwurlnlToolStripMenuItem, "TwurlnlToolStripMenuItem")
        '
        'UnuToolStripMenuItem
        '
        Me.UnuToolStripMenuItem.Name = "UnuToolStripMenuItem"
        resources.ApplyResources(Me.UnuToolStripMenuItem, "UnuToolStripMenuItem")
        '
        'BitlyToolStripMenuItem
        '
        Me.BitlyToolStripMenuItem.Name = "BitlyToolStripMenuItem"
        resources.ApplyResources(Me.BitlyToolStripMenuItem, "BitlyToolStripMenuItem")
        '
        'JmpStripMenuItem
        '
        Me.JmpStripMenuItem.Name = "JmpStripMenuItem"
        resources.ApplyResources(Me.JmpStripMenuItem, "JmpStripMenuItem")
        '
        'UpdateFollowersMenuItem1
        '
        Me.UpdateFollowersMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GetFollowersDiffToolStripMenuItem, Me.GetFollowersAllToolStripMenuItem})
        Me.UpdateFollowersMenuItem1.Name = "UpdateFollowersMenuItem1"
        resources.ApplyResources(Me.UpdateFollowersMenuItem1, "UpdateFollowersMenuItem1")
        '
        'GetFollowersDiffToolStripMenuItem
        '
        resources.ApplyResources(Me.GetFollowersDiffToolStripMenuItem, "GetFollowersDiffToolStripMenuItem")
        Me.GetFollowersDiffToolStripMenuItem.Name = "GetFollowersDiffToolStripMenuItem"
        '
        'GetFollowersAllToolStripMenuItem
        '
        Me.GetFollowersAllToolStripMenuItem.Name = "GetFollowersAllToolStripMenuItem"
        resources.ApplyResources(Me.GetFollowersAllToolStripMenuItem, "GetFollowersAllToolStripMenuItem")
        '
        'ToolStripMenuItem10
        '
        Me.ToolStripMenuItem10.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BlackFavAddToolStripMenuItem})
        Me.ToolStripMenuItem10.Name = "ToolStripMenuItem10"
        resources.ApplyResources(Me.ToolStripMenuItem10, "ToolStripMenuItem10")
        '
        'BlackFavAddToolStripMenuItem
        '
        Me.BlackFavAddToolStripMenuItem.Name = "BlackFavAddToolStripMenuItem"
        resources.ApplyResources(Me.BlackFavAddToolStripMenuItem, "BlackFavAddToolStripMenuItem")
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'FollowCommandMenuItem
        '
        Me.FollowCommandMenuItem.Name = "FollowCommandMenuItem"
        resources.ApplyResources(Me.FollowCommandMenuItem, "FollowCommandMenuItem")
        '
        'RemoveCommandMenuItem
        '
        Me.RemoveCommandMenuItem.Name = "RemoveCommandMenuItem"
        resources.ApplyResources(Me.RemoveCommandMenuItem, "RemoveCommandMenuItem")
        '
        'FriendshipMenuItem
        '
        Me.FriendshipMenuItem.Name = "FriendshipMenuItem"
        resources.ApplyResources(Me.FriendshipMenuItem, "FriendshipMenuItem")
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
        '
        'OwnStatusMenuItem
        '
        Me.OwnStatusMenuItem.Name = "OwnStatusMenuItem"
        resources.ApplyResources(Me.OwnStatusMenuItem, "OwnStatusMenuItem")
        '
        'MenuItemHelp
        '
        Me.MenuItemHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MatomeMenuItem, Me.ToolStripSeparator16, Me.VerUpMenuItem, Me.WedataMenuItem, Me.ToolStripSeparator14, Me.ApiInfoMenuItem, Me.InfoTwitterMenuItem, Me.ToolStripSeparator7, Me.AboutMenuItem, Me.DebugModeToolStripMenuItem})
        Me.MenuItemHelp.Name = "MenuItemHelp"
        resources.ApplyResources(Me.MenuItemHelp, "MenuItemHelp")
        '
        'MatomeMenuItem
        '
        Me.MatomeMenuItem.Name = "MatomeMenuItem"
        resources.ApplyResources(Me.MatomeMenuItem, "MatomeMenuItem")
        '
        'ToolStripSeparator16
        '
        Me.ToolStripSeparator16.Name = "ToolStripSeparator16"
        resources.ApplyResources(Me.ToolStripSeparator16, "ToolStripSeparator16")
        '
        'VerUpMenuItem
        '
        Me.VerUpMenuItem.Name = "VerUpMenuItem"
        resources.ApplyResources(Me.VerUpMenuItem, "VerUpMenuItem")
        '
        'WedataMenuItem
        '
        Me.WedataMenuItem.Name = "WedataMenuItem"
        resources.ApplyResources(Me.WedataMenuItem, "WedataMenuItem")
        '
        'ToolStripSeparator14
        '
        Me.ToolStripSeparator14.Name = "ToolStripSeparator14"
        resources.ApplyResources(Me.ToolStripSeparator14, "ToolStripSeparator14")
        '
        'ApiInfoMenuItem
        '
        Me.ApiInfoMenuItem.Name = "ApiInfoMenuItem"
        resources.ApplyResources(Me.ApiInfoMenuItem, "ApiInfoMenuItem")
        '
        'InfoTwitterMenuItem
        '
        Me.InfoTwitterMenuItem.Name = "InfoTwitterMenuItem"
        resources.ApplyResources(Me.InfoTwitterMenuItem, "InfoTwitterMenuItem")
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        resources.ApplyResources(Me.ToolStripSeparator7, "ToolStripSeparator7")
        '
        'AboutMenuItem
        '
        Me.AboutMenuItem.Name = "AboutMenuItem"
        resources.ApplyResources(Me.AboutMenuItem, "AboutMenuItem")
        '
        'DebugModeToolStripMenuItem
        '
        Me.DebugModeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DumpPostClassToolStripMenuItem, Me.TraceOutToolStripMenuItem})
        Me.DebugModeToolStripMenuItem.Name = "DebugModeToolStripMenuItem"
        resources.ApplyResources(Me.DebugModeToolStripMenuItem, "DebugModeToolStripMenuItem")
        '
        'DumpPostClassToolStripMenuItem
        '
        Me.DumpPostClassToolStripMenuItem.CheckOnClick = True
        Me.DumpPostClassToolStripMenuItem.Name = "DumpPostClassToolStripMenuItem"
        resources.ApplyResources(Me.DumpPostClassToolStripMenuItem, "DumpPostClassToolStripMenuItem")
        '
        'TraceOutToolStripMenuItem
        '
        Me.TraceOutToolStripMenuItem.CheckOnClick = True
        Me.TraceOutToolStripMenuItem.Name = "TraceOutToolStripMenuItem"
        resources.ApplyResources(Me.TraceOutToolStripMenuItem, "TraceOutToolStripMenuItem")
        '
        'ContextMenuStripPostMode
        '
        Me.ContextMenuStripPostMode.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemUrlMultibyteSplit, Me.ToolStripMenuItemApiCommandEvasion, Me.ToolStripMenuItemUrlAutoShorten, Me.IdeographicSpaceToSpaceToolStripMenuItem, Me.ToolStripSeparator8, Me.HashSelectComboBox})
        Me.ContextMenuStripPostMode.Name = "ContextMenuStripPostMode"
        resources.ApplyResources(Me.ContextMenuStripPostMode, "ContextMenuStripPostMode")
        '
        'ToolStripMenuItemUrlMultibyteSplit
        '
        Me.ToolStripMenuItemUrlMultibyteSplit.CheckOnClick = True
        Me.ToolStripMenuItemUrlMultibyteSplit.Name = "ToolStripMenuItemUrlMultibyteSplit"
        resources.ApplyResources(Me.ToolStripMenuItemUrlMultibyteSplit, "ToolStripMenuItemUrlMultibyteSplit")
        '
        'ToolStripMenuItemApiCommandEvasion
        '
        Me.ToolStripMenuItemApiCommandEvasion.Checked = True
        Me.ToolStripMenuItemApiCommandEvasion.CheckOnClick = True
        Me.ToolStripMenuItemApiCommandEvasion.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ToolStripMenuItemApiCommandEvasion.Name = "ToolStripMenuItemApiCommandEvasion"
        resources.ApplyResources(Me.ToolStripMenuItemApiCommandEvasion, "ToolStripMenuItemApiCommandEvasion")
        '
        'ToolStripMenuItemUrlAutoShorten
        '
        Me.ToolStripMenuItemUrlAutoShorten.CheckOnClick = True
        Me.ToolStripMenuItemUrlAutoShorten.Name = "ToolStripMenuItemUrlAutoShorten"
        resources.ApplyResources(Me.ToolStripMenuItemUrlAutoShorten, "ToolStripMenuItemUrlAutoShorten")
        '
        'IdeographicSpaceToSpaceToolStripMenuItem
        '
        Me.IdeographicSpaceToSpaceToolStripMenuItem.Checked = True
        Me.IdeographicSpaceToSpaceToolStripMenuItem.CheckOnClick = True
        Me.IdeographicSpaceToSpaceToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.IdeographicSpaceToSpaceToolStripMenuItem.Name = "IdeographicSpaceToSpaceToolStripMenuItem"
        resources.ApplyResources(Me.IdeographicSpaceToSpaceToolStripMenuItem, "IdeographicSpaceToSpaceToolStripMenuItem")
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        resources.ApplyResources(Me.ToolStripSeparator8, "ToolStripSeparator8")
        '
        'HashSelectComboBox
        '
        Me.HashSelectComboBox.Name = "HashSelectComboBox"
        resources.ApplyResources(Me.HashSelectComboBox, "HashSelectComboBox")
        '
        'TimerTimeline
        '
        Me.TimerTimeline.Interval = 60000
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.ContextMenuStrip = Me.ContextMenuStrip1
        resources.ApplyResources(Me.NotifyIcon1, "NotifyIcon1")
        '
        'TimerColorize
        '
        '
        'TimerRefreshIcon
        '
        Me.TimerRefreshIcon.Interval = 50
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'TweenMain
        '
        Me.AllowDrop = True
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.Controls.Add(Me.ToolStripContainer1)
        Me.Name = "TweenMain"
        Me.ToolStripContainer1.BottomToolStripPanel.ResumeLayout(False)
        Me.ToolStripContainer1.BottomToolStripPanel.PerformLayout()
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.TopToolStripPanel.ResumeLayout(False)
        Me.ToolStripContainer1.TopToolStripPanel.PerformLayout()
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ContextMenuTabProperty.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.Panel2.PerformLayout()
        Me.SplitContainer2.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        CType(Me.UserPicture, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip3.ResumeLayout(False)
        Me.ContextMenuStrip4.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ContextMenuStrip2.ResumeLayout(False)
        Me.ContextMenuStripPostMode.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TimerTimeline As System.Windows.Forms.Timer
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents EndToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuStrip2 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents DMStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DeleteStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RefreshStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SettingStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents TabImage As System.Windows.Forms.ImageList
    Friend WithEvents NewPostPopMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ListLockMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents JumpUnreadMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator15 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents TimerColorize As System.Windows.Forms.Timer
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SaveLogMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator17 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents TimerRefreshIcon As System.Windows.Forms.Timer
    Friend WithEvents ContextMenuTabProperty As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents UreadManageMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NotifyDispMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SoundFileComboBox As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents ToolStripSeparator18 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents DeleteTabMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FilterEditMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator19 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents AddTabMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator20 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SelectAllMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ClearTabMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator11 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents PlaySoundMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ContextMenuStrip3 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents IconNameToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SaveIconPictureToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents ListTab As System.Windows.Forms.TabControl
    Friend WithEvents MenuItemTab As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuItemOperate As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents StatusLabelUrl As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents MenuItemFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuItemEdit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopySTOTMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopyURLMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents MenuItemSubSearch As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuItemSearchNext As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuItemSearchPrev As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuItemCommand As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuItemHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MatomeMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator16 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents VerUpMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WedataMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator14 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents InfoTwitterMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents AboutMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents UserPicture As System.Windows.Forms.PictureBox
    Friend WithEvents NameLabel As System.Windows.Forms.Label
    Friend WithEvents PostBrowser As System.Windows.Forms.WebBrowser
    Friend WithEvents DateTimeLabel As System.Windows.Forms.Label
    Friend WithEvents StatusText As System.Windows.Forms.TextBox
    Friend WithEvents lblLen As System.Windows.Forms.Label
    Friend WithEvents PostButton As System.Windows.Forms.Button
    Friend WithEvents MultiLineMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator21 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents TinyUrlConvertToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UpdateFollowersMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TinyURLToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents IsgdToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UrlConvertAutoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UrlUndoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuStrip4 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator13 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem5 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SearchItem1ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SearchItem2ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SearchItem3ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SearchItem4ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CurrentTabToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GetFollowersDiffToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GetFollowersAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem6 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MoveToHomeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MoveToFavToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusOpenMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RepliedStatusOpenMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FavorareMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenURLMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TabMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents IDRuleMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem11 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReadedStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UnreadStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReplyStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReplyAllStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FavAddToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FavRemoveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem10 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BlackFavAddToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReTweetStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ButtonPostMode As System.Windows.Forms.Button
    Friend WithEvents ContextMenuStripPostMode As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripMenuItemUrlMultibyteSplit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemApiCommandEvasion As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemUrlAutoShorten As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DebugModeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DumpPostClassToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TraceOutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TwurlnlToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TabRenameMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UnuToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BitlyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ApiInfoMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents IdeographicSpaceToSpaceToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FollowCommandMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RemoveCommandMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FriendshipMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents OwnStatusMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReTweetOriginalStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents FollowContextMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RemoveContextMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FriendshipContextMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents JmpStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents QuoteStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RefreshMoreStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents HashSelectComboBox As System.Windows.Forms.ToolStripComboBox

End Class
