Public Class ImageListViewItem
    Inherits ListViewItem

    Public Sub New(ByVal items() As String, ByVal imageKey As String)
        MyBase.New(items, imageKey)
    End Sub

    Public Property Image As Image

    Protected Overrides Sub Finalize()
        If Me.Image IsNot Nothing Then
            Me.Image.Dispose()
            Me.Image = Nothing
        End If
        MyBase.Finalize()
    End Sub
End Class
