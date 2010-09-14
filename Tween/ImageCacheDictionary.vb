Imports System.Drawing
Imports System.IO

Public Class ImageCacheDictionary
    Implements IDictionary(Of String, Image), IDisposable

    Private ReadOnly lockObject As New Object()

    Private cacheDiractoryPath As String
    Private memoryCacheCount As Integer
    Private innerDictionary As Dictionary(Of String, CachedImage)
    Private sortedKeyList As List(Of String)    '古いもの順
    Private fileCacheProcList As New Queue(Of Threading.ThreadStart)()

    Public Sub New(ByVal cacheDirectory As String, ByVal memoryCacheCount As Integer)
        SyncLock Me.lockObject
            Me.innerDictionary = New Dictionary(Of String, CachedImage)(memoryCacheCount + 1)
            Me.sortedKeyList = New List(Of String)(memoryCacheCount + 1)
            Me.memoryCacheCount = memoryCacheCount
            Me.cacheDiractoryPath = cacheDirectory
            If Not Directory.Exists(Me.cacheDiractoryPath) Then
                Directory.CreateDirectory(Me.cacheDiractoryPath)
            End If
        End SyncLock
    End Sub

    Public Sub Add(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Add
        Me.Add(item.Key, item.Value)
    End Sub

    Public Sub Add(ByVal key As String, ByVal value As Image) Implements System.Collections.Generic.IDictionary(Of String, Image).Add
        SyncLock Me.lockObject
            Me.innerDictionary.Add(key, New CachedImage(value, Me.cacheDiractoryPath))
            Me.sortedKeyList.Add(key)

            If Me.innerDictionary.Count > Me.memoryCacheCount Then
                Me.innerDictionary(Me.sortedKeyList(Me.sortedKeyList.Count - Me.memoryCacheCount - 1)).Cache()
            End If
        End SyncLock

        While Me.fileCacheProcList.Count > 0
            Me.fileCacheProcList.Dequeue().Invoke()
        End While
    End Sub

    Public Function Remove(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Remove
        Return Me.Remove(item.Key)
    End Function

    Public Function Remove(ByVal key As String) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).Remove
        SyncLock Me.lockObject
            Me.sortedKeyList.Remove(key)
            Me.innerDictionary(key).Dispose()
            Return Me.innerDictionary.Remove(key)
        End SyncLock
    End Function

    Default Public Property Item(ByVal key As String) As Image Implements System.Collections.Generic.IDictionary(Of String, Image).Item
        Get
            SyncLock Me.lockObject
                Me.sortedKeyList.Remove(key)
                Me.sortedKeyList.Add(key)
                If Me.sortedKeyList.Count > Me.memoryCacheCount Then
                    Dim imgObj As CachedImage = Me.innerDictionary(Me.sortedKeyList(Me.sortedKeyList.Count - Me.memoryCacheCount - 1))
                    If Not imgObj.Cached Then
                        Me.fileCacheProcList.Enqueue(Sub()
                                                         If Me.innerDictionary.ContainsValue(imgObj) Then
                                                             imgObj.Cache()
                                                         End If
                                                     End Sub)
                    End If
                End If
                Return Me.innerDictionary(key).Image
            End SyncLock
        End Get
        Set(ByVal value As Image)
            SyncLock Me.lockObject
                Me.sortedKeyList.Remove(key)
                Me.sortedKeyList.Add(key)
                If Me.sortedKeyList.Count > Me.memoryCacheCount Then
                    Me.innerDictionary(Me.sortedKeyList(Me.sortedKeyList.Count - Me.memoryCacheCount - 1)).Cache()
                End If
                Me.innerDictionary(key).Dispose()
                Me.innerDictionary(key) = New CachedImage(value, Me.cacheDiractoryPath)
            End SyncLock
        End Set
    End Property

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Clear
        SyncLock Me.lockObject
            For Each value As CachedImage In Me.innerDictionary.Values
                value.Dispose()
            Next

            Me.innerDictionary.Clear()
            Me.sortedKeyList.Clear()
        End SyncLock
    End Sub

    Public Function Contains(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Contains
        SyncLock Me.lockObject
            Return Me.innerDictionary.ContainsKey(item.Key) AndAlso Me.innerDictionary(item.Key) Is item.Value
        End SyncLock
    End Function

    Public Sub CopyTo(ByVal array() As System.Collections.Generic.KeyValuePair(Of String, Image), ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).CopyTo
        SyncLock Me.lockObject
            Dim index As Integer = arrayIndex
            For Each Item As KeyValuePair(Of String, CachedImage) In Me.innerDictionary
                If array.Length - 1 < index Then
                    Exit For
                End If

                array(index) = New KeyValuePair(Of String, Image)(Item.Key, Item.Value.Image)
                index += 1
            Next
        End SyncLock
    End Sub

    Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Count
        Get
            SyncLock Me.lockObject
                Return Me.innerDictionary.Count
            End SyncLock
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
            SyncLock Me.lockObject
                Return Me.innerDictionary.Keys
            End SyncLock
        End Get
    End Property

    Public Function TryGetValue(ByVal key As String, ByRef value As Image) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).TryGetValue
        SyncLock Me.lockObject
            If Me.innerDictionary.ContainsKey(key) Then
                value = Me.innerDictionary(key).Image
                Return True
            Else
                Return False
            End If
        End SyncLock
    End Function

    Public ReadOnly Property Values As System.Collections.Generic.ICollection(Of Image) Implements System.Collections.Generic.IDictionary(Of String, Image).Values
        Get
            SyncLock Me.lockObject
                Dim imgList As New List(Of Image)(Me.memoryCacheCount)
                For Each value As CachedImage In Me.innerDictionary.Values
                    imgList.Add(value.Image)
                Next
                Return imgList
            End SyncLock
        End Get
    End Property

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of System.Collections.Generic.KeyValuePair(Of String, Image)) Implements System.Collections.Generic.IEnumerable(Of System.Collections.Generic.KeyValuePair(Of String, Image)).GetEnumerator
        Throw New NotImplementedException()
    End Function

    Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Throw New NotImplementedException()
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        SyncLock Me.lockObject
            For Each item As CachedImage In Me.innerDictionary.Values
                If item.img IsNot Nothing Then
                    item.img.Dispose()
                End If
            Next

            Dim di As New DirectoryInfo(Me.cacheDiractoryPath)
            di.Delete(True)
        End SyncLock
    End Sub

    Public Function GetItemCopy(ByVal key As String) As Image
        SyncLock Me.lockObject
            Me.sortedKeyList.Remove(key)
            Me.sortedKeyList.Add(key)
            If Me.sortedKeyList.Count > Me.memoryCacheCount Then
                Dim imgObj As CachedImage = Me.innerDictionary(Me.sortedKeyList(Me.sortedKeyList.Count - Me.memoryCacheCount - 1))
                If Not imgObj.Cached Then
                    Me.fileCacheProcList.Enqueue(Sub()
                                                     If Me.innerDictionary.ContainsValue(imgObj) Then
                                                         imgObj.Cache()
                                                     End If
                                                 End Sub)
                End If
            End If

            If Me.innerDictionary(key).Image IsNot Nothing Then
                Return New Bitmap(Me.innerDictionary(key).Image)
            Else
                Return Nothing
            End If
        End SyncLock
    End Function

    Private Class CachedImage
        Implements IDisposable

        Private ReadOnly lockObject As New Object()

        Public img As Image = Nothing
        Private tmpFilePath As String = Nothing
        Private cacheDirectoryPath As String

        Public Sub New(ByVal img As Image, ByVal cacheDirectory As String)
            SyncLock Me.lockObject
                Me.img = img
                Me.cacheDirectoryPath = cacheDirectory
            End SyncLock
        End Sub

        Public ReadOnly Property Image As Image
            Get
                SyncLock Me.lockObject
                    If Me.img Is Nothing AndAlso Me.tmpFilePath IsNot Nothing Then
                        Try
                            Dim tempImage As Image = Nothing
                            Using fs As New FileStream(Me.tmpFilePath, FileMode.Open, FileAccess.Read)
                                tempImage = Bitmap.FromStream(fs)
                            End Using
                            Me.img = New Bitmap(tempImage)
                        Catch ex As OutOfMemoryException
                            Dim filePath As String = Path.Combine(Application.StartupPath, Path.GetFileName(Me.tmpFilePath))
                            File.Copy(Me.tmpFilePath, filePath)
                            Throw ex
                        End Try
                    End If

                    Return Me.img
                End SyncLock
            End Get
        End Property

        Public Sub Cache()
            SyncLock Me.lockObject
                If Me.tmpFilePath Is Nothing AndAlso Me.img IsNot Nothing Then
                    Dim tmpFile As String = Nothing

                    Dim err As Boolean = False
                    Do
                        Try
                            err = False
                            tmpFile = Path.Combine(Me.cacheDirectoryPath, Path.GetRandomFileName().Remove(8, 1))

                            Using fs As New FileStream(tmpFile, FileMode.CreateNew, FileAccess.Write)
                                Me.img.Save(fs, Imaging.ImageFormat.Bmp)
                                fs.Flush()
                            End Using
                        Catch ex As InvalidOperationException
                            err = True
                        Catch ex As IOException
                            err = True
                        Catch ex As Exception
                            File.Delete(tmpFile)
                            Me.tmpFilePath = Nothing
                            Exit Sub
                        End Try
                    Loop While err
                    Me.tmpFilePath = tmpFile
                End If
                If Me.img IsNot Nothing Then
                    Me.img.Dispose()
                    Me.img = Nothing
                End If
            End SyncLock
        End Sub

        Public ReadOnly Property Cached As Boolean
            Get
                SyncLock Me.lockObject
                    Return Me.tmpFilePath IsNot Nothing
                End SyncLock
            End Get
        End Property

        Public Sub Dispose() Implements IDisposable.Dispose
            SyncLock Me.lockObject
                If Me.img IsNot Nothing Then
                    Me.img.Dispose()
                End If

                If Me.tmpFilePath IsNot Nothing Then
                    File.Delete(Me.tmpFilePath)
                End If
            End SyncLock
        End Sub
    End Class
End Class