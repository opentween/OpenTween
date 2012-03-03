' OpenTween - Client of Twitter
' Copyright (c) 2011      kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
'           (c) 2011      fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
' All rights reserved.
' 
' This file is part of OpenTween.
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

Imports System.Threading.Tasks
Imports System.Net

Public Class FavoriteQueue
    Implements IList(Of Long)

    'Private Shared _instance As New FavoriteQueue
    'Public Shared ReadOnly Property GetInstance As FavoriteQueue
    '    Get
    '        Return _instance
    '    End Get
    'End Property

    Private tw As Twitter
    Private FavoriteCache As New List(Of Long)

    Public Sub AddRange(ByVal stsIds As IEnumerable(Of Long))
        FavoriteCache.AddRange(stsIds)
    End Sub

    'Public Sub FavoriteCacheAdd(ByVal statusId As Long, ByVal res As HttpStatusCode, Optional ByRef isMsg As Boolean = True)
    '    'If Not SettingInfo.Instance.IsUseFavoriteQueue Then Exit Sub
    '    Select Case res
    '        Case HttpStatusCode.BadGateway, HttpStatusCode.BadRequest, HttpStatusCode.ServiceUnavailable, HttpStatusCode.InternalServerError, HttpStatusCode.RequestTimeout
    '            isMsg = False
    '            FavoriteCache.Add(statusId)
    '    End Select
    'End Sub

    Public Sub FavoriteCacheStart()
        If Not FavoriteCache.Count = 0 Then
            Dim _cacheList As New List(Of Long)(FavoriteCache)
            Me.Clear()
            Parallel.ForEach(Of Long)(_cacheList, New Action(Of Long)(Sub(stsId As Long)
                                                                          tw.PostFavAdd(stsId)
                                                                      End Sub))
        End If
    End Sub

    Public Sub Add(ByVal item As Long) Implements System.Collections.Generic.ICollection(Of Long).Add
        If Not Me.Contains(item) Then
            FavoriteCache.Add(item)
        End If
    End Sub

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of Long).Clear
        FavoriteCache.Clear()
        FavoriteCache.TrimExcess()
    End Sub

    Public Function Contains(ByVal item As Long) As Boolean Implements System.Collections.Generic.ICollection(Of Long).Contains
        FavoriteCache.Contains(item)
    End Function

    Public Sub CopyTo(ByVal array() As Long, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of Long).CopyTo
        FavoriteCache.CopyTo(array, arrayIndex)
    End Sub

    Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of Long).Count
        Get
            Return FavoriteCache.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of Long).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Public Function Remove(ByVal item As Long) As Boolean Implements System.Collections.Generic.ICollection(Of Long).Remove
        Return FavoriteCache.Remove(item)
    End Function

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of Long) Implements System.Collections.Generic.IEnumerable(Of Long).GetEnumerator
        Return FavoriteCache.GetEnumerator()
    End Function
    Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return Me.GetEnumerator
    End Function

    Public Function IndexOf(ByVal item As Long) As Integer Implements System.Collections.Generic.IList(Of Long).IndexOf
        Return FavoriteCache.IndexOf(item)
    End Function

    Public Sub Insert(ByVal index As Integer, ByVal item As Long) Implements System.Collections.Generic.IList(Of Long).Insert
        FavoriteCache.Insert(index, item)
    End Sub

    Default Public Property Item(ByVal index As Integer) As Long Implements System.Collections.Generic.IList(Of Long).Item
        Get
            Return FavoriteCache(index)
        End Get
        Set(ByVal value As Long)
            FavoriteCache(index) = value
        End Set
    End Property

    Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of Long).RemoveAt
        FavoriteCache.RemoveAt(index)
    End Sub

    Public Sub New(ByVal twitter As Twitter)
        Me.tw = twitter
    End Sub
End Class
