<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MyLists
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MyLists))
        Me.ListsCheckedListBox = New System.Windows.Forms.CheckedListBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.追加AToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.削除DToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.更新RToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ListRefreshButton = New System.Windows.Forms.Button()
        Me.CloseButton = New System.Windows.Forms.Button()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListsCheckedListBox
        '
        resources.ApplyResources(Me.ListsCheckedListBox, "ListsCheckedListBox")
        Me.ListsCheckedListBox.CheckOnClick = True
        Me.ListsCheckedListBox.ContextMenuStrip = Me.ContextMenuStrip1
        Me.ListsCheckedListBox.FormattingEnabled = True
        Me.ListsCheckedListBox.Name = "ListsCheckedListBox"
        '
        'ContextMenuStrip1
        '
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.追加AToolStripMenuItem, Me.削除DToolStripMenuItem, Me.ToolStripMenuItem1, Me.更新RToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        '
        '追加AToolStripMenuItem
        '
        resources.ApplyResources(Me.追加AToolStripMenuItem, "追加AToolStripMenuItem")
        Me.追加AToolStripMenuItem.Name = "追加AToolStripMenuItem"
        '
        '削除DToolStripMenuItem
        '
        resources.ApplyResources(Me.削除DToolStripMenuItem, "削除DToolStripMenuItem")
        Me.削除DToolStripMenuItem.Name = "削除DToolStripMenuItem"
        '
        'ToolStripMenuItem1
        '
        resources.ApplyResources(Me.ToolStripMenuItem1, "ToolStripMenuItem1")
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        '
        '更新RToolStripMenuItem
        '
        resources.ApplyResources(Me.更新RToolStripMenuItem, "更新RToolStripMenuItem")
        Me.更新RToolStripMenuItem.Name = "更新RToolStripMenuItem"
        '
        'ListRefreshButton
        '
        resources.ApplyResources(Me.ListRefreshButton, "ListRefreshButton")
        Me.ListRefreshButton.Name = "ListRefreshButton"
        Me.ListRefreshButton.UseVisualStyleBackColor = True
        '
        'CloseButton
        '
        resources.ApplyResources(Me.CloseButton, "CloseButton")
        Me.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'MyLists
        '
        Me.AcceptButton = Me.CloseButton
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.CloseButton
        Me.Controls.Add(Me.CloseButton)
        Me.Controls.Add(Me.ListRefreshButton)
        Me.Controls.Add(Me.ListsCheckedListBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "MyLists"
        Me.ShowInTaskbar = False
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListsCheckedListBox As System.Windows.Forms.CheckedListBox
    Friend WithEvents ListRefreshButton As System.Windows.Forms.Button
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents 追加AToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 削除DToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents 更新RToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CloseButton As System.Windows.Forms.Button
End Class
