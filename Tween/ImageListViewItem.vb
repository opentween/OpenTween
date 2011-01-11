Public Class ImageListViewItem
    Inherits ListViewItem

    Private img As Image = Nothing

    Public Sub New(ByVal items() As String, ByVal imageKey As String)
        MyBase.New(items, imageKey)
    End Sub

    Public Sub New(ByVal items() As String, ByVal imageDictionary As ImageDictionary, ByVal imageKey As String)
        MyBase.New(items, imageKey)

        Dim dummy As Image = imageDictionary.Item(imageKey, Sub(getImg)
                                                                If getImg Is Nothing Then Exit Sub
                                                                Me.img = getImg
                                                                If Me.ListView IsNot Nothing AndAlso
                                                                    Me.ListView.Created AndAlso
                                                                    Not Me.ListView.IsDisposed Then Me.ListView.Invoke(Sub()
                                                                                                                           If Me.Index < Me.ListView.VirtualListSize Then Me.ListView.RedrawItems(Me.Index, Me.Index, False)
                                                                                                                       End Sub)
                                                            End Sub)

    End Sub

    Public ReadOnly Property Image As Image
        Get
            Return Me.img
        End Get
    End Property

    Protected Overrides Sub Finalize()
        If Me.img IsNot Nothing Then
            Me.img.Dispose()
            Me.img = Nothing
        End If
        MyBase.Finalize()
    End Sub
End Class
