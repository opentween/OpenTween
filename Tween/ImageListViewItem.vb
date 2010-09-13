Public Class ImageListViewItem
    Inherits ListViewItem
    Implements IDisposable

    Public Sub New(ByVal items() As String, ByVal imageKey As String)
        MyBase.New(items, imageKey)
    End Sub

    Public Property Image As Image

    Public Sub Dispose() Implements IDisposable.Dispose
        Me.Image.Dispose()
        Me.Image = Nothing
    End Sub
End Class
