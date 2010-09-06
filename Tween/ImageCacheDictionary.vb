Imports System.Drawing
Imports System.IO

Public Class ImageCacheDictionary
    Implements IDictionary(Of String, Image), IDisposable

    Dim memoryCacheCount As Integer
    Private dictionary As Dictionary(Of String, Image)
    Private sortedKeyList As List(Of String)
    Private keyTmpFileDictionary As New Dictionary(Of String, String)()

    Public Sub New(ByVal memoryCacheCount As Integer)
        Me.dictionary = New Dictionary(Of String, Image)(memoryCacheCount)
        Me.sortedKeyList = New List(Of String)(memoryCacheCount)
        Me.memoryCacheCount = memoryCacheCount
    End Sub

    Public Sub Add(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Add
        Me.Add1(item.Key, item.Value)
    End Sub

    Public Sub Add1(ByVal key As String, ByVal value As Image) Implements System.Collections.Generic.IDictionary(Of String, Image).Add
        Me.dictionary.Add(key, value)
        Me.sortedKeyList.Add(key)

        Dim path_ As String = Path.GetTempFileName()
        Me.keyTmpFileDictionary.Add(key, path_)
        value.Save(path_)

        While Me.dictionary.Count > memoryCacheCount
            Me.dictionary.Remove(Me.sortedKeyList(0))
            Me.sortedKeyList.RemoveAt(0)
        End While
    End Sub

    Public Function Remove(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Remove
        Return Me.Remove1(item.Key)
    End Function

    Public Function Remove1(ByVal key As String) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).Remove
        Me.sortedKeyList.Remove(key)
        File.Delete(Me.keyTmpFileDictionary(key))
        Me.keyTmpFileDictionary.Remove(key)
        Return Me.dictionary.Remove(key)
    End Function

    Default Public Property Item(ByVal key As String) As Image Implements System.Collections.Generic.IDictionary(Of String, Image).Item
        Get
            If (Not Me.dictionary.ContainsKey(key)) AndAlso Me.keyTmpFileDictionary.ContainsKey(key) Then
                Me.dictionary.Add(key, Image.FromFile(Me.keyTmpFileDictionary(key)))
            End If

            While Me.dictionary.Count > memoryCacheCount
                Me.dictionary.Remove(Me.sortedKeyList(0))
                Me.sortedKeyList.RemoveAt(0)
            End While

            Me.sortedKeyList.Remove(key)
            Me.sortedKeyList.Add(key)
            Return Me.dictionary(key)
        End Get
        Set(ByVal value As Image)
            If (Not Me.dictionary.ContainsKey(key)) AndAlso Me.keyTmpFileDictionary.ContainsKey(key) Then
                Me.dictionary.Add(key, Nothing)
            End If

            Me.sortedKeyList.Remove(key)
            Me.sortedKeyList.Add(key)
            Me.dictionary(key) = value

            value.Save(Me.keyTmpFileDictionary(key))
        End Set
    End Property

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Clear
        Me.dictionary.Clear()
        Me.sortedKeyList.Clear()

        For Each path As String In Me.keyTmpFileDictionary.Values
            File.Delete(path)
        Next
        Me.keyTmpFileDictionary.Clear()
    End Sub

    Public Function Contains(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Contains
        Return Me.keyTmpFileDictionary.ContainsKey(item.Key) AndAlso Me.keyTmpFileDictionary(item.Key) Is item.Value
    End Function

    Public Sub CopyTo(ByVal array() As System.Collections.Generic.KeyValuePair(Of String, Image), ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).CopyTo
        Dim index As Integer = arrayIndex
        For Each Item As KeyValuePair(Of String, Image) In Me.dictionary
            If array.Length - 1 < index Then
                Exit For
            End If

            array(index) = Item
            index += 1
        Next
    End Sub

    Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Count
        Get
            Return Me.keyTmpFileDictionary.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Public Function ContainsKey(ByVal key As String) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).ContainsKey
        Return Me.keyTmpFileDictionary.ContainsKey(key)
    End Function

    Public ReadOnly Property Keys As System.Collections.Generic.ICollection(Of String) Implements System.Collections.Generic.IDictionary(Of String, Image).Keys
        Get
            Return Me.keyTmpFileDictionary.Keys
        End Get
    End Property

    Public Function TryGetValue(ByVal key As String, ByRef value As Image) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).TryGetValue
        If Me.keyTmpFileDictionary.ContainsKey(key) Then
            value = Me.Item(key)
            Return True
        Else
            Return False
        End If
    End Function

    Public ReadOnly Property Values As System.Collections.Generic.ICollection(Of Image) Implements System.Collections.Generic.IDictionary(Of String, Image).Values
        Get
            Dim imageList As New List(Of Image)(Me.keyTmpFileDictionary.Count)
            imageList.AddRange(Me.dictionary.Values)
            For Each key As String In Me.keyTmpFileDictionary.Keys
                If Not Me.dictionary.ContainsKey(key) Then
                    imageList.Add(Image.FromFile(Me.keyTmpFileDictionary(key)))
                End If
            Next

            Return imageList
        End Get
    End Property

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of System.Collections.Generic.KeyValuePair(Of String, Image)) Implements System.Collections.Generic.IEnumerable(Of System.Collections.Generic.KeyValuePair(Of String, Image)).GetEnumerator
        Throw New NotImplementedException()
        Return Me.dictionary.GetEnumerator()
    End Function

    Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Throw New NotImplementedException()
        Return Me.dictionary.GetEnumerator()
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        For Each tmpFilePath As String In Me.keyTmpFileDictionary.Values
            File.Delete(tmpFilePath)
        Next
    End Sub
End Class