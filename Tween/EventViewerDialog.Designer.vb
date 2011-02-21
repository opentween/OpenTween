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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EventViewerDialog))
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
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.StatusLabelCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.TabEventType.SuspendLayout()
        Me.TabPageAll.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'OK_Button
        '
        resources.ApplyResources(Me.OK_Button, "OK_Button")
        Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.OK_Button.Name = "OK_Button"
        '
        'CheckExcludeMyEvent
        '
        resources.ApplyResources(Me.CheckExcludeMyEvent, "CheckExcludeMyEvent")
        Me.CheckExcludeMyEvent.Name = "CheckExcludeMyEvent"
        Me.CheckExcludeMyEvent.UseVisualStyleBackColor = True
        '
        'ButtonRefresh
        '
        resources.ApplyResources(Me.ButtonRefresh, "ButtonRefresh")
        Me.ButtonRefresh.Name = "ButtonRefresh"
        Me.ButtonRefresh.UseVisualStyleBackColor = True
        '
        'TabEventType
        '
        resources.ApplyResources(Me.TabEventType, "TabEventType")
        Me.TabEventType.Controls.Add(Me.TabPageAll)
        Me.TabEventType.Controls.Add(Me.TabPageFav)
        Me.TabEventType.Controls.Add(Me.TabPageUnfav)
        Me.TabEventType.Controls.Add(Me.TabPageFollow)
        Me.TabEventType.Controls.Add(Me.TabPageAddLists)
        Me.TabEventType.Controls.Add(Me.TabPageRemoveLists)
        Me.TabEventType.Controls.Add(Me.TabPageListsCreated)
        Me.TabEventType.Controls.Add(Me.TabPageBlock)
        Me.TabEventType.Controls.Add(Me.TabPageUserUpdate)
        Me.TabEventType.Name = "TabEventType"
        Me.TabEventType.SelectedIndex = 0
        '
        'TabPageAll
        '
        Me.TabPageAll.Controls.Add(Me.EventList)
        resources.ApplyResources(Me.TabPageAll, "TabPageAll")
        Me.TabPageAll.Name = "TabPageAll"
        Me.TabPageAll.Tag = "All"
        Me.TabPageAll.UseVisualStyleBackColor = True
        '
        'EventList
        '
        Me.EventList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4})
        resources.ApplyResources(Me.EventList, "EventList")
        Me.EventList.FullRowSelect = True
        Me.EventList.Name = "EventList"
        Me.EventList.ShowItemToolTips = True
        Me.EventList.UseCompatibleStateImageBehavior = False
        Me.EventList.View = System.Windows.Forms.View.Details
        Me.EventList.VirtualMode = True
        '
        'ColumnHeader1
        '
        resources.ApplyResources(Me.ColumnHeader1, "ColumnHeader1")
        '
        'ColumnHeader2
        '
        resources.ApplyResources(Me.ColumnHeader2, "ColumnHeader2")
        '
        'ColumnHeader3
        '
        resources.ApplyResources(Me.ColumnHeader3, "ColumnHeader3")
        '
        'ColumnHeader4
        '
        resources.ApplyResources(Me.ColumnHeader4, "ColumnHeader4")
        '
        'TabPageFav
        '
        resources.ApplyResources(Me.TabPageFav, "TabPageFav")
        Me.TabPageFav.Name = "TabPageFav"
        Me.TabPageFav.Tag = "Favorite"
        Me.TabPageFav.UseVisualStyleBackColor = True
        '
        'TabPageUnfav
        '
        resources.ApplyResources(Me.TabPageUnfav, "TabPageUnfav")
        Me.TabPageUnfav.Name = "TabPageUnfav"
        Me.TabPageUnfav.Tag = "Unfavorite"
        Me.TabPageUnfav.UseVisualStyleBackColor = True
        '
        'TabPageFollow
        '
        resources.ApplyResources(Me.TabPageFollow, "TabPageFollow")
        Me.TabPageFollow.Name = "TabPageFollow"
        Me.TabPageFollow.Tag = "Follow"
        Me.TabPageFollow.UseVisualStyleBackColor = True
        '
        'TabPageAddLists
        '
        resources.ApplyResources(Me.TabPageAddLists, "TabPageAddLists")
        Me.TabPageAddLists.Name = "TabPageAddLists"
        Me.TabPageAddLists.Tag = "ListMemberAdded"
        Me.TabPageAddLists.UseVisualStyleBackColor = True
        '
        'TabPageRemoveLists
        '
        resources.ApplyResources(Me.TabPageRemoveLists, "TabPageRemoveLists")
        Me.TabPageRemoveLists.Name = "TabPageRemoveLists"
        Me.TabPageRemoveLists.Tag = "ListMemberRemoved"
        Me.TabPageRemoveLists.UseVisualStyleBackColor = True
        '
        'TabPageListsCreated
        '
        resources.ApplyResources(Me.TabPageListsCreated, "TabPageListsCreated")
        Me.TabPageListsCreated.Name = "TabPageListsCreated"
        Me.TabPageListsCreated.Tag = "ListCreated"
        Me.TabPageListsCreated.UseVisualStyleBackColor = True
        '
        'TabPageBlock
        '
        resources.ApplyResources(Me.TabPageBlock, "TabPageBlock")
        Me.TabPageBlock.Name = "TabPageBlock"
        Me.TabPageBlock.Tag = "Block"
        Me.TabPageBlock.UseVisualStyleBackColor = True
        '
        'TabPageUserUpdate
        '
        resources.ApplyResources(Me.TabPageUserUpdate, "TabPageUserUpdate")
        Me.TabPageUserUpdate.Name = "TabPageUserUpdate"
        Me.TabPageUserUpdate.Tag = "UserUpdate"
        Me.TabPageUserUpdate.UseVisualStyleBackColor = True
        '
        'TextBoxKeyword
        '
        resources.ApplyResources(Me.TextBoxKeyword, "TextBoxKeyword")
        Me.TextBoxKeyword.Name = "TextBoxKeyword"
        '
        'CheckRegex
        '
        resources.ApplyResources(Me.CheckRegex, "CheckRegex")
        Me.CheckRegex.Name = "CheckRegex"
        Me.CheckRegex.UseVisualStyleBackColor = True
        '
        'CheckBoxFilter
        '
        resources.ApplyResources(Me.CheckBoxFilter, "CheckBoxFilter")
        Me.CheckBoxFilter.Name = "CheckBoxFilter"
        Me.CheckBoxFilter.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabelCount})
        resources.ApplyResources(Me.StatusStrip1, "StatusStrip1")
        Me.StatusStrip1.Name = "StatusStrip1"
        '
        'StatusLabelCount
        '
        Me.StatusLabelCount.Name = "StatusLabelCount"
        resources.ApplyResources(Me.StatusLabelCount, "StatusLabelCount")
        '
        'EventViewerDialog
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.OK_Button
        Me.Controls.Add(Me.CheckBoxFilter)
        Me.Controls.Add(Me.CheckRegex)
        Me.Controls.Add(Me.TextBoxKeyword)
        Me.Controls.Add(Me.TabEventType)
        Me.Controls.Add(Me.ButtonRefresh)
        Me.Controls.Add(Me.CheckExcludeMyEvent)
        Me.Controls.Add(Me.OK_Button)
        Me.Controls.Add(Me.StatusStrip1)
        Me.DoubleBuffered = True
        Me.Name = "EventViewerDialog"
        Me.ShowInTaskbar = False
        Me.TabEventType.ResumeLayout(False)
        Me.TabPageAll.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
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
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents StatusLabelCount As System.Windows.Forms.ToolStripStatusLabel

End Class
