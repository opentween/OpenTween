Imports System.Threading.Tasks
Imports System.Net

Public Class FavoriteQueue
    Private Shared _instance As New FavoriteQueue
    Public Shared ReadOnly Property GetInstance As FavoriteQueue
        Get
            Return _instance
        End Get
    End Property

    Public FavoriteCache As New List(Of Long)

    Public Sub Add(ByVal stsId As Long)
        If Not FavoriteCache.Contains(stsId) Then
            FavoriteCache.Add(stsId)
        End If
    End Sub

    Public Sub AddRange(ByVal stsIds As IEnumerable(Of Long))
        FavoriteCache.AddRange(stsIds)
    End Sub

    Public Sub Remove(ByVal stsId As Long)
        FavoriteCache.Remove(stsId)
    End Sub

    Public Sub FavoriteCacheAdd(ByVal statusId As Long, ByVal res As HttpStatusCode, Optional ByRef isMsg As Boolean = True)
        'If Not SettingInfo.Instance.IsUseFavoriteQueue Then Exit Sub
        Select Case res
            Case HttpStatusCode.BadGateway, HttpStatusCode.BadRequest, HttpStatusCode.ServiceUnavailable, HttpStatusCode.InternalServerError, HttpStatusCode.RequestTimeout
                isMsg = False
                FavoriteCache.Add(statusId)
        End Select
    End Sub

    Public Sub FavoriteCacheStart()
        If Not FavoriteCache.Count = 0 Then
            Dim _cacheList As New List(Of Long)(FavoriteCache)
            AllClear()
            Parallel.ForEach(Of Long)(_cacheList, New Action(Of Long)(Sub(stsId As Long)
                                                                          TweenMain.GetInstance.TwitterInstance.PostFavAdd(stsId)
                                                                      End Sub))
        End If
    End Sub

    Public ReadOnly Property Count As Integer
        Get
            Return FavoriteCache.Count
        End Get
    End Property

    Public Sub AllClear()
        FavoriteCache.Clear()
        FavoriteCache.TrimExcess()
    End Sub

    Public Function Contains(stsId As Long) As Boolean
        Return FavoriteCache.Contains(stsId)
    End Function

End Class
