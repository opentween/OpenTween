Option Strict On
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SearchWord
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SearchWord))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.OK_Button = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.SWordText = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.CheckSearchCaseSensitive = New System.Windows.Forms.CheckBox
        Me.CheckSearchRegex = New System.Windows.Forms.CheckBox
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AccessibleDescription = Nothing
        Me.TableLayoutPanel1.AccessibleName = Nothing
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.BackgroundImage = Nothing
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Font = Nothing
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'OK_Button
        '
        Me.OK_Button.AccessibleDescription = Nothing
        Me.OK_Button.AccessibleName = Nothing
        resources.ApplyResources(Me.OK_Button, "OK_Button")
        Me.OK_Button.BackgroundImage = Nothing
        Me.OK_Button.Font = Nothing
        Me.OK_Button.Name = "OK_Button"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.AccessibleDescription = Nothing
        Me.Cancel_Button.AccessibleName = Nothing
        resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
        Me.Cancel_Button.BackgroundImage = Nothing
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Font = Nothing
        Me.Cancel_Button.Name = "Cancel_Button"
        '
        'SWordText
        '
        Me.SWordText.AccessibleDescription = Nothing
        Me.SWordText.AccessibleName = Nothing
        resources.ApplyResources(Me.SWordText, "SWordText")
        Me.SWordText.BackgroundImage = Nothing
        Me.SWordText.Font = Nothing
        Me.SWordText.Name = "SWordText"
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = Nothing
        Me.Label1.AccessibleName = Nothing
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Font = Nothing
        Me.Label1.Name = "Label1"
        '
        'CheckSearchCaseSensitive
        '
        Me.CheckSearchCaseSensitive.AccessibleDescription = Nothing
        Me.CheckSearchCaseSensitive.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckSearchCaseSensitive, "CheckSearchCaseSensitive")
        Me.CheckSearchCaseSensitive.BackgroundImage = Nothing
        Me.CheckSearchCaseSensitive.Font = Nothing
        Me.CheckSearchCaseSensitive.Name = "CheckSearchCaseSensitive"
        Me.CheckSearchCaseSensitive.UseVisualStyleBackColor = True
        '
        'CheckSearchRegex
        '
        Me.CheckSearchRegex.AccessibleDescription = Nothing
        Me.CheckSearchRegex.AccessibleName = Nothing
        resources.ApplyResources(Me.CheckSearchRegex, "CheckSearchRegex")
        Me.CheckSearchRegex.BackgroundImage = Nothing
        Me.CheckSearchRegex.Font = Nothing
        Me.CheckSearchRegex.Name = "CheckSearchRegex"
        Me.CheckSearchRegex.UseVisualStyleBackColor = True
        '
        'SearchWord
        '
        Me.AcceptButton = Me.OK_Button
        Me.AccessibleDescription = Nothing
        Me.AccessibleName = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Nothing
        Me.CancelButton = Me.Cancel_Button
        Me.Controls.Add(Me.CheckSearchRegex)
        Me.Controls.Add(Me.CheckSearchCaseSensitive)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.SWordText)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Font = Nothing
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = Nothing
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SearchWord"
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents SWordText As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents CheckSearchCaseSensitive As System.Windows.Forms.CheckBox
    Friend WithEvents CheckSearchRegex As System.Windows.Forms.CheckBox

End Class
