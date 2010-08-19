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
        Me.ListsCheckedListBox = New System.Windows.Forms.CheckedListBox()
        Me.SuspendLayout()
        '
        'ListsCheckedListBox
        '
        Me.ListsCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListsCheckedListBox.FormattingEnabled = True
        Me.ListsCheckedListBox.Location = New System.Drawing.Point(0, 0)
        Me.ListsCheckedListBox.Name = "ListsCheckedListBox"
        Me.ListsCheckedListBox.SelectionMode = System.Windows.Forms.SelectionMode.None
        Me.ListsCheckedListBox.Size = New System.Drawing.Size(228, 186)
        Me.ListsCheckedListBox.TabIndex = 0
        '
        'MyLists
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(228, 186)
        Me.Controls.Add(Me.ListsCheckedListBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "MyLists"
        Me.Text = "MyLists"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ListsCheckedListBox As System.Windows.Forms.CheckedListBox
End Class
