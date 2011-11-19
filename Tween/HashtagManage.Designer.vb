<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HashtagManage
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HashtagManage))
        Me.TableLayoutButtons = New System.Windows.Forms.TableLayoutPanel()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.DeleteButton = New System.Windows.Forms.Button()
        Me.EditButton = New System.Windows.Forms.Button()
        Me.AddButton = New System.Windows.Forms.Button()
        Me.HistoryHashList = New System.Windows.Forms.ListBox()
        Me.UseHashText = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CheckPermanent = New System.Windows.Forms.CheckBox()
        Me.GroupDetail = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.PermOK_Button = New System.Windows.Forms.Button()
        Me.PermCancel_Button = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.RadioLast = New System.Windows.Forms.RadioButton()
        Me.RadioHead = New System.Windows.Forms.RadioButton()
        Me.UnSelectButton = New System.Windows.Forms.Button()
        Me.GroupHashtag = New System.Windows.Forms.GroupBox()
        Me.TableLayoutButtons.SuspendLayout()
        Me.GroupDetail.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.GroupHashtag.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutButtons
        '
        resources.ApplyResources(Me.TableLayoutButtons, "TableLayoutButtons")
        Me.TableLayoutButtons.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutButtons.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutButtons.Name = "TableLayoutButtons"
        '
        'Cancel_Button
        '
        resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Name = "Cancel_Button"
        '
        'OK_Button
        '
        resources.ApplyResources(Me.OK_Button, "OK_Button")
        Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.OK_Button.Name = "OK_Button"
        '
        'DeleteButton
        '
        resources.ApplyResources(Me.DeleteButton, "DeleteButton")
        Me.DeleteButton.Name = "DeleteButton"
        Me.DeleteButton.UseVisualStyleBackColor = True
        '
        'EditButton
        '
        resources.ApplyResources(Me.EditButton, "EditButton")
        Me.EditButton.Name = "EditButton"
        Me.EditButton.UseVisualStyleBackColor = True
        '
        'AddButton
        '
        resources.ApplyResources(Me.AddButton, "AddButton")
        Me.AddButton.Name = "AddButton"
        Me.AddButton.UseVisualStyleBackColor = True
        '
        'HistoryHashList
        '
        resources.ApplyResources(Me.HistoryHashList, "HistoryHashList")
        Me.HistoryHashList.FormattingEnabled = True
        Me.HistoryHashList.Name = "HistoryHashList"
        Me.HistoryHashList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        '
        'UseHashText
        '
        resources.ApplyResources(Me.UseHashText, "UseHashText")
        Me.UseHashText.Name = "UseHashText"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'CheckPermanent
        '
        resources.ApplyResources(Me.CheckPermanent, "CheckPermanent")
        Me.CheckPermanent.Name = "CheckPermanent"
        Me.CheckPermanent.UseVisualStyleBackColor = True
        '
        'GroupDetail
        '
        resources.ApplyResources(Me.GroupDetail, "GroupDetail")
        Me.GroupDetail.Controls.Add(Me.TableLayoutPanel2)
        Me.GroupDetail.Controls.Add(Me.UseHashText)
        Me.GroupDetail.Controls.Add(Me.Label1)
        Me.GroupDetail.Name = "GroupDetail"
        Me.GroupDetail.TabStop = False
        '
        'TableLayoutPanel2
        '
        resources.ApplyResources(Me.TableLayoutPanel2, "TableLayoutPanel2")
        Me.TableLayoutPanel2.Controls.Add(Me.PermOK_Button, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.PermCancel_Button, 1, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        '
        'PermOK_Button
        '
        resources.ApplyResources(Me.PermOK_Button, "PermOK_Button")
        Me.PermOK_Button.Name = "PermOK_Button"
        '
        'PermCancel_Button
        '
        resources.ApplyResources(Me.PermCancel_Button, "PermCancel_Button")
        Me.PermCancel_Button.Name = "PermCancel_Button"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'RadioLast
        '
        resources.ApplyResources(Me.RadioLast, "RadioLast")
        Me.RadioLast.Name = "RadioLast"
        Me.RadioLast.TabStop = True
        Me.RadioLast.UseVisualStyleBackColor = True
        '
        'RadioHead
        '
        resources.ApplyResources(Me.RadioHead, "RadioHead")
        Me.RadioHead.Name = "RadioHead"
        Me.RadioHead.TabStop = True
        Me.RadioHead.UseVisualStyleBackColor = True
        '
        'UnSelectButton
        '
        resources.ApplyResources(Me.UnSelectButton, "UnSelectButton")
        Me.UnSelectButton.Name = "UnSelectButton"
        Me.UnSelectButton.UseVisualStyleBackColor = True
        '
        'GroupHashtag
        '
        resources.ApplyResources(Me.GroupHashtag, "GroupHashtag")
        Me.GroupHashtag.Controls.Add(Me.HistoryHashList)
        Me.GroupHashtag.Controls.Add(Me.Label3)
        Me.GroupHashtag.Controls.Add(Me.UnSelectButton)
        Me.GroupHashtag.Controls.Add(Me.RadioLast)
        Me.GroupHashtag.Controls.Add(Me.DeleteButton)
        Me.GroupHashtag.Controls.Add(Me.RadioHead)
        Me.GroupHashtag.Controls.Add(Me.EditButton)
        Me.GroupHashtag.Controls.Add(Me.AddButton)
        Me.GroupHashtag.Controls.Add(Me.CheckPermanent)
        Me.GroupHashtag.Name = "GroupHashtag"
        Me.GroupHashtag.TabStop = False
        '
        'HashtagManage
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupHashtag)
        Me.Controls.Add(Me.GroupDetail)
        Me.Controls.Add(Me.TableLayoutButtons)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "HashtagManage"
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.TableLayoutButtons.ResumeLayout(False)
        Me.GroupDetail.ResumeLayout(False)
        Me.GroupDetail.PerformLayout()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.GroupHashtag.ResumeLayout(False)
        Me.GroupHashtag.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutButtons As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents DeleteButton As System.Windows.Forms.Button
    Friend WithEvents EditButton As System.Windows.Forms.Button
    Friend WithEvents AddButton As System.Windows.Forms.Button
    Friend WithEvents HistoryHashList As System.Windows.Forms.ListBox
    Friend WithEvents UseHashText As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents CheckPermanent As System.Windows.Forms.CheckBox
    Friend WithEvents GroupDetail As System.Windows.Forms.GroupBox
    Friend WithEvents RadioLast As System.Windows.Forms.RadioButton
    Friend WithEvents RadioHead As System.Windows.Forms.RadioButton
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents PermOK_Button As System.Windows.Forms.Button
    Friend WithEvents PermCancel_Button As System.Windows.Forms.Button
    Friend WithEvents UnSelectButton As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents GroupHashtag As System.Windows.Forms.GroupBox

End Class
