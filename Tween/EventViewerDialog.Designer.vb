<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EventViewerDialog
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
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.CheckExcludeMyEvent = New System.Windows.Forms.CheckBox()
        Me.ButtonRefresh = New System.Windows.Forms.Button()
        Me.TabEventType = New System.Windows.Forms.TabControl()
        Me.TabPageAll = New System.Windows.Forms.TabPage()
        Me.EventList = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPageFav = New System.Windows.Forms.TabPage()
        Me.TabPageUnfav = New System.Windows.Forms.TabPage()
        Me.TabPageFollow = New System.Windows.Forms.TabPage()
        Me.TabPageAddLists = New System.Windows.Forms.TabPage()
        Me.TabPageRemoveLists = New System.Windows.Forms.TabPage()
        Me.TabPageListsCreated = New System.Windows.Forms.TabPage()
        Me.TabPageBlock = New System.Windows.Forms.TabPage()
        Me.TabPageUserUpdate = New System.Windows.Forms.TabPage()
        Me.TextBoxKeyword = New System.Windows.Forms.TextBox()
        Me.CheckRegex = New System.Windows.Forms.CheckBox()
        Me.CheckBoxFilter = New System.Windows.Forms.CheckBox()
        Me.TabEventType.SuspendLayout()
        Me.TabPageAll.SuspendLayout()
        Me.SuspendLayout()
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.OK_Button.Location = New System.Drawing.Point(659, 301)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 21)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'CheckExcludeMyEvent
        '
        Me.CheckExcludeMyEvent.AutoSize = True
        Me.CheckExcludeMyEvent.Location = New System.Drawing.Point(12, 249)
        Me.CheckExcludeMyEvent.Name = "CheckExcludeMyEvent"
        Me.CheckExcludeMyEvent.Size = New System.Drawing.Size(197, 16)
        Me.CheckExcludeMyEvent.TabIndex = 2
        Me.CheckExcludeMyEvent.Text = "自分が発生させたイベントを除外する"
        Me.CheckExcludeMyEvent.UseVisualStyleBackColor = True
        '
        'ButtonRefresh
        '
        Me.ButtonRefresh.Location = New System.Drawing.Point(600, 261)
        Me.ButtonRefresh.Name = "ButtonRefresh"
        Me.ButtonRefresh.Size = New System.Drawing.Size(126, 21)
        Me.ButtonRefresh.TabIndex = 3
        Me.ButtonRefresh.Text = "最新の情報に更新"
        Me.ButtonRefresh.UseVisualStyleBackColor = True
        '
        'TabEventType
        '
        Me.TabEventType.Alignment = System.Windows.Forms.TabAlignment.Bottom
        Me.TabEventType.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabEventType.Controls.Add(Me.TabPageAll)
        Me.TabEventType.Controls.Add(Me.TabPageFav)
        Me.TabEventType.Controls.Add(Me.TabPageUnfav)
        Me.TabEventType.Controls.Add(Me.TabPageFollow)
        Me.TabEventType.Controls.Add(Me.TabPageAddLists)
        Me.TabEventType.Controls.Add(Me.TabPageRemoveLists)
        Me.TabEventType.Controls.Add(Me.TabPageListsCreated)
        Me.TabEventType.Controls.Add(Me.TabPageBlock)
        Me.TabEventType.Controls.Add(Me.TabPageUserUpdate)
        Me.TabEventType.Location = New System.Drawing.Point(12, 12)
        Me.TabEventType.Name = "TabEventType"
        Me.TabEventType.SelectedIndex = 0
        Me.TabEventType.Size = New System.Drawing.Size(714, 221)
        Me.TabEventType.TabIndex = 4
        '
        'TabPageAll
        '
        Me.TabPageAll.Controls.Add(Me.EventList)
        Me.TabPageAll.Location = New System.Drawing.Point(4, 4)
        Me.TabPageAll.Name = "TabPageAll"
        Me.TabPageAll.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageAll.Size = New System.Drawing.Size(706, 195)
        Me.TabPageAll.TabIndex = 0
        Me.TabPageAll.Tag = "All"
        Me.TabPageAll.Text = "全て"
        Me.TabPageAll.UseVisualStyleBackColor = True
        '
        'EventList
        '
        Me.EventList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4})
        Me.EventList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EventList.FullRowSelect = True
        Me.EventList.Location = New System.Drawing.Point(3, 3)
        Me.EventList.Name = "EventList"
        Me.EventList.Size = New System.Drawing.Size(700, 189)
        Me.EventList.TabIndex = 3
        Me.EventList.UseCompatibleStateImageBehavior = False
        Me.EventList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Datetime"
        Me.ColumnHeader1.Width = 86
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Type"
        Me.ColumnHeader2.Width = 90
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Username"
        Me.ColumnHeader3.Width = 106
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Target"
        Me.ColumnHeader4.Width = 360
        '
        'TabPageFav
        '
        Me.TabPageFav.Location = New System.Drawing.Point(4, 4)
        Me.TabPageFav.Name = "TabPageFav"
        Me.TabPageFav.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageFav.Size = New System.Drawing.Size(706, 195)
        Me.TabPageFav.TabIndex = 1
        Me.TabPageFav.Tag = "Favorite"
        Me.TabPageFav.Text = "Favorite"
        Me.TabPageFav.UseVisualStyleBackColor = True
        '
        'TabPageUnfav
        '
        Me.TabPageUnfav.Location = New System.Drawing.Point(4, 4)
        Me.TabPageUnfav.Name = "TabPageUnfav"
        Me.TabPageUnfav.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageUnfav.Size = New System.Drawing.Size(706, 195)
        Me.TabPageUnfav.TabIndex = 2
        Me.TabPageUnfav.Tag = "Unfavorite"
        Me.TabPageUnfav.Text = "Unfavorite"
        Me.TabPageUnfav.UseVisualStyleBackColor = True
        '
        'TabPageFollow
        '
        Me.TabPageFollow.Location = New System.Drawing.Point(4, 4)
        Me.TabPageFollow.Name = "TabPageFollow"
        Me.TabPageFollow.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageFollow.Size = New System.Drawing.Size(706, 195)
        Me.TabPageFollow.TabIndex = 3
        Me.TabPageFollow.Tag = "Follow"
        Me.TabPageFollow.Text = "Follow"
        Me.TabPageFollow.UseVisualStyleBackColor = True
        '
        'TabPageAddLists
        '
        Me.TabPageAddLists.Location = New System.Drawing.Point(4, 4)
        Me.TabPageAddLists.Name = "TabPageAddLists"
        Me.TabPageAddLists.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageAddLists.Size = New System.Drawing.Size(706, 195)
        Me.TabPageAddLists.TabIndex = 4
        Me.TabPageAddLists.Tag = "ListMemberAdded"
        Me.TabPageAddLists.Text = "ListsMemberAdded"
        Me.TabPageAddLists.UseVisualStyleBackColor = True
        '
        'TabPageRemoveLists
        '
        Me.TabPageRemoveLists.Location = New System.Drawing.Point(4, 4)
        Me.TabPageRemoveLists.Name = "TabPageRemoveLists"
        Me.TabPageRemoveLists.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageRemoveLists.Size = New System.Drawing.Size(706, 195)
        Me.TabPageRemoveLists.TabIndex = 5
        Me.TabPageRemoveLists.Tag = "ListMemberRemoved"
        Me.TabPageRemoveLists.Text = "ListsMemberRemoved"
        Me.TabPageRemoveLists.UseVisualStyleBackColor = True
        '
        'TabPageListsCreated
        '
        Me.TabPageListsCreated.Location = New System.Drawing.Point(4, 4)
        Me.TabPageListsCreated.Name = "TabPageListsCreated"
        Me.TabPageListsCreated.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageListsCreated.Size = New System.Drawing.Size(706, 195)
        Me.TabPageListsCreated.TabIndex = 6
        Me.TabPageListsCreated.Tag = "ListCreated"
        Me.TabPageListsCreated.Text = "ListsCreated"
        Me.TabPageListsCreated.UseVisualStyleBackColor = True
        '
        'TabPageBlock
        '
        Me.TabPageBlock.Location = New System.Drawing.Point(4, 4)
        Me.TabPageBlock.Name = "TabPageBlock"
        Me.TabPageBlock.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageBlock.Size = New System.Drawing.Size(706, 195)
        Me.TabPageBlock.TabIndex = 7
        Me.TabPageBlock.Tag = "Block"
        Me.TabPageBlock.Text = "Block"
        Me.TabPageBlock.UseVisualStyleBackColor = True
        '
        'TabPageUserUpdate
        '
        Me.TabPageUserUpdate.Location = New System.Drawing.Point(4, 4)
        Me.TabPageUserUpdate.Name = "TabPageUserUpdate"
        Me.TabPageUserUpdate.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageUserUpdate.Size = New System.Drawing.Size(706, 195)
        Me.TabPageUserUpdate.TabIndex = 8
        Me.TabPageUserUpdate.Tag = "UserUpdate"
        Me.TabPageUserUpdate.Text = "UserUpdate"
        Me.TabPageUserUpdate.UseVisualStyleBackColor = True
        '
        'TextBoxKeyword
        '
        Me.TextBoxKeyword.Location = New System.Drawing.Point(142, 278)
        Me.TextBoxKeyword.Name = "TextBoxKeyword"
        Me.TextBoxKeyword.Size = New System.Drawing.Size(231, 19)
        Me.TextBoxKeyword.TabIndex = 5
        '
        'CheckRegex
        '
        Me.CheckRegex.AutoSize = True
        Me.CheckRegex.Location = New System.Drawing.Point(12, 304)
        Me.CheckRegex.Name = "CheckRegex"
        Me.CheckRegex.Size = New System.Drawing.Size(72, 16)
        Me.CheckRegex.TabIndex = 6
        Me.CheckRegex.Text = "正規表現"
        Me.CheckRegex.UseVisualStyleBackColor = True
        '
        'CheckBoxFilter
        '
        Me.CheckBoxFilter.AutoSize = True
        Me.CheckBoxFilter.Location = New System.Drawing.Point(12, 280)
        Me.CheckBoxFilter.Name = "CheckBoxFilter"
        Me.CheckBoxFilter.Size = New System.Drawing.Size(124, 16)
        Me.CheckBoxFilter.TabIndex = 7
        Me.CheckBoxFilter.Text = "キーワードで絞り込む"
        Me.CheckBoxFilter.UseVisualStyleBackColor = True
        '
        'EventViewerDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.OK_Button
        Me.ClientSize = New System.Drawing.Size(738, 334)
        Me.Controls.Add(Me.CheckBoxFilter)
        Me.Controls.Add(Me.CheckRegex)
        Me.Controls.Add(Me.TextBoxKeyword)
        Me.Controls.Add(Me.TabEventType)
        Me.Controls.Add(Me.ButtonRefresh)
        Me.Controls.Add(Me.CheckExcludeMyEvent)
        Me.Controls.Add(Me.OK_Button)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EventViewerDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Events"
        Me.TabEventType.ResumeLayout(False)
        Me.TabPageAll.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents CheckExcludeMyEvent As System.Windows.Forms.CheckBox
    Friend WithEvents ButtonRefresh As System.Windows.Forms.Button
    Friend WithEvents TabEventType As System.Windows.Forms.TabControl
    Friend WithEvents TabPageAll As System.Windows.Forms.TabPage
    Friend WithEvents EventList As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents TabPageFav As System.Windows.Forms.TabPage
    Friend WithEvents TabPageUnfav As System.Windows.Forms.TabPage
    Friend WithEvents TabPageFollow As System.Windows.Forms.TabPage
    Friend WithEvents TabPageAddLists As System.Windows.Forms.TabPage
    Friend WithEvents TabPageRemoveLists As System.Windows.Forms.TabPage
    Friend WithEvents TabPageListsCreated As System.Windows.Forms.TabPage
    Friend WithEvents TabPageBlock As System.Windows.Forms.TabPage
    Friend WithEvents TabPageUserUpdate As System.Windows.Forms.TabPage
    Friend WithEvents TextBoxKeyword As System.Windows.Forms.TextBox
    Friend WithEvents CheckRegex As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxFilter As System.Windows.Forms.CheckBox

End Class
