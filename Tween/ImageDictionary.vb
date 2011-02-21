' Tween - Client of Twitter
' Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
'           (c) 2008-2011 Moz (@syo68k)
'           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
'           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
'           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
' All rights reserved.
' 
' This file is part of Tween.
' 
' This program is free software; you can redistribute it and/or modify it
' under the terms of the GNU General Public License as published by the Free
' Software Foundation; either version 3 of the License, or (at your option)
' any later version.
' 
' This program is distributed in the hope that it will be useful, but
' WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
' or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
' for more details. 
' 
' You should have received a copy of the GNU General Public License along
' with this program. If not, see <http://www.gnu.org/licenses/>, or write to
' the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
' Boston, MA 02110-1301, USA.

Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports System.Runtime.Caching
Imports System.Collections.Specialized

Public Class ImageDictionary
    Implements IDictionary(Of String, Image), IDisposable

    Private ReadOnly lockObject As New Object()

    Private innerDictionary As MemoryCache
    Private waitStack As Stack(Of KeyValuePair(Of String, Action(Of Image)))
    Private cachePolicy As New CacheItemPolicy()
    Private removedCount As Long = 0

    Public Sub New(ByVal cacheMemoryLimit As Integer)
        SyncLock Me.lockObject
            '10Mb,80%
            'キャッシュチェック間隔はデフォルト値（2分毎）
            Me.innerDictionary = New MemoryCache("imageCache",
                                                 New NameValueCollection() From
                                                 {
                                                     {"CacheMemoryLimitMegabytes", cacheMemoryLimit.ToString},
                                                     {"PhysicalMemoryLimitPercentage", "80"}
                                                 })
            Me.waitStack = New Stack(Of KeyValuePair(Of String, Action(Of Image)))
            Me.cachePolicy.RemovedCallback = AddressOf CacheRemoved
            Me.cachePolicy.SlidingExpiration = TimeSpan.FromMinutes(30)     '30分参照されなかったら削除
        End SyncLock
    End Sub

    Public ReadOnly Property CacheCount As Long
        Get
            Return innerDictionary.GetCount
        End Get
    End Property

    Public ReadOnly Property CacheRemoveCount As Long
        Get
            Return removedCount
        End Get
    End Property

    Public ReadOnly Property CacheMemoryLimit As Long
        Get
            Return innerDictionary.CacheMemoryLimit
        End Get
    End Property

    Public ReadOnly Property PhysicalMemoryLimit As Long
        Get
            Return innerDictionary.PhysicalMemoryLimit
        End Get
    End Property

    Public ReadOnly Property PollingInterval As TimeSpan
        Get
            Return innerDictionary.PollingInterval
        End Get
    End Property
    Private Sub CacheRemoved(ByVal item As CacheEntryRemovedArguments)
        DirectCast(item.CacheItem.Value, Image).Dispose()
        removedCount += 1
    End Sub

    Public Sub Add(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Add
        Me.Add(item.Key, item.Value)
    End Sub

    Public Sub Add(ByVal key As String, ByVal value As Image) Implements System.Collections.Generic.IDictionary(Of String, Image).Add
        SyncLock Me.lockObject
            If Me.innerDictionary.Contains(key) Then Exit Sub
            Me.innerDictionary.Add(key, value, Me.cachePolicy)
        End SyncLock
    End Sub

    Public Function Remove(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Remove
        Return Me.Remove(item.Key)
    End Function

    Public Function Remove(ByVal key As String) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).Remove
        SyncLock Me.lockObject
            DirectCast(Me.innerDictionary(key), Image).Dispose()
        End SyncLock
    End Function

    Default ReadOnly Property Item(ByVal key As String, ByVal callBack As Action(Of Image)) As Image
        Get
            SyncLock Me.lockObject
                If Me.innerDictionary(key) IsNot Nothing Then
                    callBack(New Bitmap(DirectCast(Me.innerDictionary(key), Image)))
                Else
                    'スタックに積む
                    Me.waitStack.Push(New KeyValuePair(Of String, Action(Of Image))(key, callBack))
                End If
            End SyncLock

            Return Nothing
        End Get
    End Property

    Default Public Property Item(ByVal key As String) As Image Implements System.Collections.Generic.IDictionary(Of String, Image).Item
        Get
            SyncLock Me.lockObject
                If Me.innerDictionary(key) IsNot Nothing Then
                    Try
                        Return New Bitmap(DirectCast(Me.innerDictionary(key), Image))
                    Catch ex As ArgumentException
                        DirectCast(Me.innerDictionary(key), Image).Dispose()
                        Me.innerDictionary(key) = Nothing
                        Return Nothing
                    End Try
                Else
                    Return Nothing
                End If
            End SyncLock
        End Get
        Set(ByVal value As Image)
            SyncLock Me.lockObject
                DirectCast(Me.innerDictionary(key), Image).Dispose()
                Me.innerDictionary.Add(key, value, Me.cachePolicy)
            End SyncLock
        End Set
    End Property

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Clear
        SyncLock Me.lockObject
            Me.innerDictionary.Trim(100)
        End SyncLock
    End Sub

    Public Function Contains(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Contains
        SyncLock Me.lockObject
            Return Me.innerDictionary.Contains(item.Key) AndAlso Me.innerDictionary(item.Key) Is item.Value
        End SyncLock
    End Function

    Public Sub CopyTo(ByVal array() As System.Collections.Generic.KeyValuePair(Of String, Image), ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).CopyTo
        SyncLock Me.lockObject
            Throw New NotImplementedException()
        End SyncLock
    End Sub

    Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Count
        Get
            SyncLock Me.lockObject
                Return CType(Me.innerDictionary.GetCount(), Integer)
            End SyncLock
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Public Function ContainsKey(ByVal key As String) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).ContainsKey
        Return Me.innerDictionary.Contains(key)
    End Function

    Public ReadOnly Property Keys As System.Collections.Generic.ICollection(Of String) Implements System.Collections.Generic.IDictionary(Of String, Image).Keys
        Get
            SyncLock Me.lockObject
                Throw New NotImplementedException()
            End SyncLock
        End Get
    End Property

    Public Function TryGetValue(ByVal key As String, ByRef value As Image) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).TryGetValue
        SyncLock Me.lockObject
            If Me.innerDictionary.Contains(key) Then
                value = DirectCast(Me.innerDictionary(key), Image)
                Return True
            Else
                Return False
            End If
        End SyncLock
    End Function

    Public ReadOnly Property Values As System.Collections.Generic.ICollection(Of Image) Implements System.Collections.Generic.IDictionary(Of String, Image).Values
        Get
            SyncLock Me.lockObject
                Throw New NotImplementedException()
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
            Me.innerDictionary.Dispose()
        End SyncLock
    End Sub

    '取得一時停止
    Private _pauseGetImage As Boolean = False
    Public Property PauseGetImage As Boolean
        Get
            Return Me._pauseGetImage
        End Get
        Set(ByVal value As Boolean)
            Me._pauseGetImage = value

            Static popping As Boolean = False

            If Not Me._pauseGetImage AndAlso Not popping AndAlso Me.waitStack.Count > 0 Then
                popping = True
                '最新から処理し
                Dim imgDlProc As ThreadStart
                imgDlProc = Sub()
                                While Me.waitStack.Count > 0 AndAlso Not Me._pauseGetImage
                                    Dim req As KeyValuePair(Of String, Action(Of Image))
                                    SyncLock lockObject
                                        req = Me.waitStack.Pop
                                    End SyncLock
                                    Dim proc As New GetImageDelegate(AddressOf GetImage)
                                    proc.BeginInvoke(req, Nothing, Nothing)
                                End While
                                popping = False
                            End Sub
                imgDlProc.BeginInvoke(Nothing, Nothing)
            End If
        End Set
    End Property
    Delegate Sub GetImageDelegate(ByVal arg1 As KeyValuePair(Of String, Action(Of Image)))
    Private Sub GetImage(ByVal downloadAsyncInfo As KeyValuePair(Of String, Action(Of Image)))
        Dim callbackImage As Image = Nothing
        SyncLock lockObject
            If Me.innerDictionary(downloadAsyncInfo.Key) IsNot Nothing Then
                callbackImage = New Bitmap(DirectCast(Me.innerDictionary(downloadAsyncInfo.Key), Image))
            End If
        End SyncLock
        If callbackImage IsNot Nothing Then
            If downloadAsyncInfo.Value IsNot Nothing Then
                downloadAsyncInfo.Value.Invoke(callbackImage)
            End If
            Exit Sub
        End If
        Dim hv As New HttpVarious()
        Dim dlImage As Image = hv.GetImage(downloadAsyncInfo.Key, 10000)
        SyncLock lockObject
            If Me.innerDictionary(downloadAsyncInfo.Key) Is Nothing Then
                If dlImage IsNot Nothing Then
                    Me.innerDictionary.Add(downloadAsyncInfo.Key, dlImage, Me.cachePolicy)
                    callbackImage = New Bitmap(dlImage)
                End If
            Else
                callbackImage = New Bitmap(DirectCast(Me.innerDictionary(downloadAsyncInfo.Key), Image))
            End If
        End SyncLock
        If downloadAsyncInfo.Value IsNot Nothing Then downloadAsyncInfo.Value.Invoke(callbackImage)
    End Sub
End Class