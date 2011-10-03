Public Class MessageForm

    Public Shadows Function ShowDialog(ByVal message As String) As Windows.Forms.DialogResult
        Label1.Text = message

        ' ラベルコントロールをセンタリング
        Label1.Left = (Me.ClientSize.Width - Label1.Size.Width) \ 2

        Label1.Refresh()
        Label1.Visible = True
        Return MyBase.ShowDialog()
    End Function
End Class