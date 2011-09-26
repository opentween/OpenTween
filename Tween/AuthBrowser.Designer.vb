<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AuthBrowser
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
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.AddressLabel = New System.Windows.Forms.Label()
        Me.AuthWebBrowser = New System.Windows.Forms.WebBrowser()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Cancel = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.PinText = New System.Windows.Forms.TextBox()
        Me.NextButton = New System.Windows.Forms.Button()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.AutoSize = True
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 22)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(773, 0)
        Me.Panel1.TabIndex = 0
        '
        'AddressLabel
        '
        Me.AddressLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.AddressLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AddressLabel.Location = New System.Drawing.Point(0, 0)
        Me.AddressLabel.Name = "AddressLabel"
        Me.AddressLabel.Size = New System.Drawing.Size(531, 22)
        Me.AddressLabel.TabIndex = 0
        Me.AddressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'AuthWebBrowser
        '
        Me.AuthWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AuthWebBrowser.Location = New System.Drawing.Point(0, 22)
        Me.AuthWebBrowser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.AuthWebBrowser.Name = "AuthWebBrowser"
        Me.AuthWebBrowser.Size = New System.Drawing.Size(773, 540)
        Me.AuthWebBrowser.TabIndex = 1
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Cancel)
        Me.Panel2.Controls.Add(Me.AddressLabel)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.PinText)
        Me.Panel2.Controls.Add(Me.NextButton)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(773, 22)
        Me.Panel2.TabIndex = 2
        '
        'Cancel
        '
        Me.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel.Location = New System.Drawing.Point(536, 32)
        Me.Cancel.Name = "Cancel"
        Me.Cancel.Size = New System.Drawing.Size(75, 15)
        Me.Cancel.TabIndex = 3
        Me.Cancel.TabStop = False
        Me.Cancel.Text = "Cancel"
        Me.Cancel.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Right
        Me.Label1.Location = New System.Drawing.Point(531, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(3)
        Me.Label1.Size = New System.Drawing.Size(29, 18)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "PIN"
        '
        'PinText
        '
        Me.PinText.Dock = System.Windows.Forms.DockStyle.Right
        Me.PinText.Location = New System.Drawing.Point(560, 0)
        Me.PinText.Name = "PinText"
        Me.PinText.Size = New System.Drawing.Size(138, 19)
        Me.PinText.TabIndex = 1
        '
        'NextButton
        '
        Me.NextButton.Dock = System.Windows.Forms.DockStyle.Right
        Me.NextButton.Location = New System.Drawing.Point(698, 0)
        Me.NextButton.Name = "NextButton"
        Me.NextButton.Size = New System.Drawing.Size(75, 22)
        Me.NextButton.TabIndex = 2
        Me.NextButton.Text = "Finish"
        Me.NextButton.UseVisualStyleBackColor = True
        '
        'AuthBrowser
        '
        Me.AcceptButton = Me.NextButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel
        Me.ClientSize = New System.Drawing.Size(773, 562)
        Me.Controls.Add(Me.AuthWebBrowser)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AuthBrowser"
        Me.ShowIcon = False
        Me.Text = "Browser"
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents AuthWebBrowser As System.Windows.Forms.WebBrowser
    Friend WithEvents AddressLabel As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents NextButton As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents PinText As System.Windows.Forms.TextBox
    Friend WithEvents Cancel As System.Windows.Forms.Button
End Class
