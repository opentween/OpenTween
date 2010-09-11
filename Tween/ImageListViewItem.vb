Public Class ImageListViewItem
    Inherits ListViewItem

    Public Sub New(ByVal items() As String, ByVal imageKey As String)
        MyBase.New(items, imageKey)
    End Sub

    Public Property Image As Image
End Class
