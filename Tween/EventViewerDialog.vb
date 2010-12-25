Imports System.Windows.Forms

Public Class EventViewerDialog
    Public Property EventSource As List(Of Twitter.FormattedEvent)

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub EventViewerDialog_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        If EventSource IsNot Nothing AndAlso EventSource.Count > 0 Then
            For Each x As Twitter.FormattedEvent In EventSource
                Dim s() As String = {x.CreatedAt.ToString, x.Event.ToUpper, x.Username, x.Target}
                Me.EventList.Items.Add(New ListViewItem(s))
            Next
            Me.EventList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
        End If
    End Sub
End Class
