Imports System.Drawing
Imports System.IO

Public Class ImageCacheDictionary
    Implements IDictionary(Of String, Image), IDisposable

    Dim memoryCacheCount As Integer
    Private innerDictionary As Dictionary(Of String, CachedImage)
    Private sortedKeyList As List(Of String)    '古いもの順

    Public Sub New(ByVal memoryCacheCount As Integer)
        Me.innerDictionary = New Dictionary(Of String, CachedImage)(memoryCacheCount + 1)
        Me.sortedKeyList = New List(Of String)(memoryCacheCount + 1)
        Me.memoryCacheCount = memoryCacheCount
    End Sub

    Public Sub Add(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Add
        Me.Add(item.Key, item.Value)
    End Sub

    Public Sub Add(ByVal key As String, ByVal value As Image) Implements System.Collections.Generic.IDictionary(Of String, Image).Add
        Me.innerDictionary.Add(key, New CachedImage(value))
        Me.sortedKeyList.Add(key)

        If Me.innerDictionary.Count > Me.memoryCacheCount Then
            Me.innerDictionary(Me.sortedKeyList(Me.sortedKeyList.Count - Me.memoryCacheCount - 1)).Chache()
        End If
    End Sub

    Public Function Remove(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Remove
        Return Me.Remove(item.Key)
    End Function

    Public Function Remove(ByVal key As String) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).Remove
        Me.sortedKeyList.Remove(key)
        Me.innerDictionary(key).Dispose()
        Return Me.innerDictionary.Remove(key)
    End Function

    Default Public Property Item(ByVal key As String) As Image Implements System.Collections.Generic.IDictionary(Of String, Image).Item
        Get
            Me.sortedKeyList.Remove(key)
            Me.sortedKeyList.Add(key)
            If Me.sortedKeyList.Count > Me.memoryCacheCount Then
                Me.innerDictionary(Me.sortedKeyList(Me.sortedKeyList.Count - Me.memoryCacheCount - 1)).Chache()
            End If
            Return Me.innerDictionary(key).Image
        End Get
        Set(ByVal value As Image)
            Me.sortedKeyList.Remove(key)
            Me.sortedKeyList.Add(key)
            If Me.sortedKeyList.Count > Me.memoryCacheCount Then
                Me.innerDictionary(Me.sortedKeyList(Me.sortedKeyList.Count - Me.memoryCacheCount - 1)).Chache()
            End If
            Me.innerDictionary(key).Dispose()
            Me.innerDictionary(key) = New CachedImage(value)
        End Set
    End Property

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Clear
        For Each value As CachedImage In Me.innerDictionary.Values
            value.Dispose()
        Next

        Me.innerDictionary.Clear()
        Me.sortedKeyList.Clear()
    End Sub

    Public Function Contains(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Contains
        Return Me.innerDictionary.ContainsKey(item.Key) AndAlso Me.innerDictionary(item.Key) Is item.Value
    End Function

    Public Sub CopyTo(ByVal array() As System.Collections.Generic.KeyValuePair(Of String, Image), ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).CopyTo
        Dim index As Integer = arrayIndex
        For Each Item As KeyValuePair(Of String, CachedImage) In Me.innerDictionary
            If array.Length - 1 < index Then
                Exit For
            End If

            array(index) = New KeyValuePair(Of String, Image)(Item.Key, Item.Value.Image)
            index += 1
        Next
    End Sub

    Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Count
        Get
            Return Me.innerDictionary.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Public Function ContainsKey(ByVal key As String) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).ContainsKey
        Return Me.innerDictionary.ContainsKey(key)
    End Function

    Public ReadOnly Property Keys As System.Collections.Generic.ICollection(Of String) Implements System.Collections.Generic.IDictionary(Of String, Image).Keys
        Get
            Return Me.innerDictionary.Keys
        End Get
    End Property

    Public Function TryGetValue(ByVal key As String, ByRef value As Image) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).TryGetValue
        If Me.innerDictionary.ContainsKey(key) Then
            value = Me.innerDictionary(key).Image
            Return True
        Else
            Return False
        End If
    End Function

    Public ReadOnly Property Values As System.Collections.Generic.ICollection(Of Image) Implements System.Collections.Generic.IDictionary(Of String, Image).Values
        Get
            Dim imgList As New List(Of Image)(Me.memoryCacheCount)
            For Each value As CachedImage In Me.innerDictionary.Values
                imgList.Add(value.Image)
            Next
            Return imgList
        End Get
    End Property

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of System.Collections.Generic.KeyValuePair(Of String, Image)) Implements System.Collections.Generic.IEnumerable(Of System.Collections.Generic.KeyValuePair(Of String, Image)).GetEnumerator
        Throw New NotImplementedException()
        Return Nothing
    End Function

    Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Throw New NotImplementedException()
        Return Nothing
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        For Each item As CachedImage In Me.innerDictionary.Values
            item.Dispose()
        Next
    End Sub

    Private Class CachedImage
        Implements IDisposable

        Private img As Image = Nothing
        Private tmpFilePath As String = Nothing
        Private Shared lockObject As New Object

        Public Sub New(ByVal img As Image)
            Me.img = img
        End Sub

        Public ReadOnly Property Image As Image
            Get
                SyncLock lockObject
                    If Me.img Is Nothing Then
                        Me.img = Image.FromFile(Me.tmpFilePath)
                    End If

                    Return Me.img
                End SyncLock
            End Get
        End Property

        Public Sub Chache()
            SyncLock lockObject
                If Me.tmpFilePath Is Nothing Then
                    Me.tmpFilePath = Path.GetTempFileName()
                    Me.img.Save(Me.tmpFilePath)
                End If
                If Me.img IsNot Nothing Then
                    Me.img.Dispose()
                    Me.img = Nothing
                End If
            End SyncLock
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If Me.img IsNot Nothing Then
                Me.img.Dispose()
            End If

            If Me.tmpFilePath IsNot Nothing Then
                File.Delete(Me.tmpFilePath)
            End If
        End Sub
    End Class
End Class