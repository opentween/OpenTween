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
        Me.ListsCheckedListBox = New System.Windows.Forms.CheckedListBox()
        Me.ListRefreshButton = New System.Windows.Forms.Button()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.追加AToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.削除DToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.更新RToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListsCheckedListBox
        '
        Me.ListsCheckedListBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListsCheckedListBox.CheckOnClick = True
        Me.ListsCheckedListBox.ContextMenuStrip = Me.ContextMenuStrip1
        Me.ListsCheckedListBox.FormattingEnabled = True
        Me.ListsCheckedListBox.HorizontalScrollbar = True
        Me.ListsCheckedListBox.IntegralHeight = False
        Me.ListsCheckedListBox.Location = New System.Drawing.Point(0, 0)
        Me.ListsCheckedListBox.Name = "ListsCheckedListBox"
        Me.ListsCheckedListBox.Size = New System.Drawing.Size(228, 177)
        Me.ListsCheckedListBox.TabIndex = 0
        '
        'ListRefreshButton
        '
        Me.ListRefreshButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListRefreshButton.AutoSize = True
        Me.ListRefreshButton.Location = New System.Drawing.Point(0, 178)
        Me.ListRefreshButton.Name = "ListRefreshButton"
        Me.ListRefreshButton.Size = New System.Drawing.Size(96, 23)
        Me.ListRefreshButton.TabIndex = 1
        Me.ListRefreshButton.Text = "リスト一覧を更新"
        Me.ListRefreshButton.UseVisualStyleBackColor = True
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.追加AToolStripMenuItem, Me.削除DToolStripMenuItem, Me.ToolStripMenuItem1, Me.更新RToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(111, 76)
        '
        '追加AToolStripMenuItem
        '
        Me.追加AToolStripMenuItem.Name = "追加AToolStripMenuItem"
        Me.追加AToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.追加AToolStripMenuItem.Text = "追加(&A)"
        '
        '削除DToolStripMenuItem
        '
        Me.削除DToolStripMenuItem.Name = "削除DToolStripMenuItem"
        Me.削除DToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.削除DToolStripMenuItem.Text = "削除(&D)"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(149, 6)
        '
        '更新RToolStripMenuItem
        '
        Me.更新RToolStripMenuItem.Name = "更新RToolStripMenuItem"
        Me.更新RToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.更新RToolStripMenuItem.Text = "更新(&R)"
        '
        'MyLists
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(228, 201)
        Me.Controls.Add(Me.ListRefreshButton)
        Me.Controls.Add(Me.ListsCheckedListBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "MyLists"
        Me.Text = "MyLists"
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
End Class
