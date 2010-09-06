' Tween - Client of Twitter
' Copyright (c) 2007-2010 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
'           (c) 2008-2010 Moz (@syo68k) <http://iddy.jp/profile/moz/>
'           (c) 2008-2010 takeshik (@takeshik) <http://www.takeshik.org/>
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

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports Tween.TweenCustomControl
Imports System.Text.RegularExpressions
Imports System.Web.HttpUtility
Imports System.Text

Public NotInheritable Class PostClass
    Private _Nick As String
    Private _Data As String
    Private _ImageUrl As String
    Private _Name As String
    Private _PDate As Date
    Private _Id As Long
    Private _IsFav As Boolean
    Private _OrgData As String
    Private _IsRead As Boolean
    Private _IsReply As Boolean
    Private _IsExcludeReply As Boolean
    Private _IsProtect As Boolean
    Private _IsOWL As Boolean
    Private _IsMark As Boolean
    Private _InReplyToUser As String
    Private _InReplyToId As Long
    Private _Source As String
    Private _ReplyToList As New List(Of String)
    Private _IsMe As Boolean
    Private _IsDm As Boolean
    Private _statuses As Statuses = Statuses.None
    Private _Uid As Long
    Private _FilterHit As Boolean
    Private _RetweetedBy As String = ""
    Private _RetweetedId As Long = 0
    Private _searchTabName As String = ""

    <FlagsAttribute()> _
    Private Enum Statuses
        None = 0
        Protect = 1
        Mark = 2
        Read = 4
        Reply = 8
    End Enum

    Public Sub New(ByVal Nickname As String, _
            ByVal Data As String, _
            ByVal OriginalData As String, _
            ByVal ImageUrl As String, _
            ByVal Name As String, _
            ByVal PDate As Date, _
            ByVal Id As Long, _
            ByVal IsFav As Boolean, _
            ByVal IsRead As Boolean, _
            ByVal IsReply As Boolean, _
            ByVal IsExcludeReply As Boolean, _
            ByVal IsProtect As Boolean, _
            ByVal IsOwl As Boolean, _
            ByVal IsMark As Boolean, _
            ByVal InReplyToUser As String, _
            ByVal InReplyToId As Long, _
            ByVal Source As String, _
            ByVal ReplyToList As List(Of String), _
            ByVal IsMe As Boolean, _
            ByVal IsDm As Boolean, _
            ByVal Uid As Long, _
            ByVal FilterHit As Boolean, _
            ByVal RetweetedBy As String, _
            ByVal RetweetedId As Long)
        _Nick = Nickname
        _Data = Data
        _ImageUrl = ImageUrl
        _Name = Name
        _PDate = PDate
        _Id = Id
        _IsFav = IsFav
        _OrgData = OriginalData
        _IsRead = IsRead
        _IsReply = IsReply
        _IsExcludeReply = IsExcludeReply
        _IsProtect = IsProtect
        _IsOWL = IsOwl
        _IsMark = IsMark
        _InReplyToUser = InReplyToUser
        _InReplyToId = InReplyToId
        _Source = Source
        _ReplyToList = ReplyToList
        _IsMe = IsMe
        _IsDm = IsDm
        _Uid = Uid
        _FilterHit = FilterHit
        _RetweetedBy = RetweetedBy
        _RetweetedId = RetweetedId
    End Sub

    Public Sub New()
    End Sub

    Public Property Nickname() As String
        Get
            Return _Nick
        End Get
        Set(ByVal value As String)
            _Nick = value
        End Set
    End Property
    Public Property Data() As String
        Get
            Return _Data
        End Get
        Set(ByVal value As String)
            _Data = value
        End Set
    End Property
    Public Property ImageUrl() As String
        Get
            Return _ImageUrl
        End Get
        Set(ByVal value As String)
            _ImageUrl = value
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property
    Public Property PDate() As Date
        Get
            Return _PDate
        End Get
        Set(ByVal value As Date)
            _PDate = value
        End Set
    End Property
    Public Property Id() As Long
        Get
            Return _Id
        End Get
        Set(ByVal value As Long)
            _Id = value
        End Set
    End Property
    Public Property IsFav() As Boolean
        Get
            If Me.RetweetedId > 0 AndAlso TabInformations.GetInstance.RetweetSource(Me.RetweetedId) IsNot Nothing Then
                Return TabInformations.GetInstance.RetweetSource(Me.RetweetedId).IsFav
            Else
                Return _IsFav
            End If
        End Get
        Set(ByVal value As Boolean)
            _IsFav = value
            If Me.RetweetedId > 0 AndAlso TabInformations.GetInstance.RetweetSource(Me.RetweetedId) IsNot Nothing Then
                TabInformations.GetInstance.RetweetSource(Me.RetweetedId).IsFav = value
            End If
        End Set
    End Property
    Public Property OriginalData() As String
        Get
            Return _OrgData
        End Get
        Set(ByVal value As String)
            _OrgData = value
        End Set
    End Property
    Public Property IsRead() As Boolean
        Get
            Return _IsRead
        End Get
        Set(ByVal value As Boolean)
            If value Then
                _statuses = _statuses Or Statuses.Read
            Else
                _statuses = _statuses And Not Statuses.Read
            End If
            _IsRead = value
        End Set
    End Property
    Public Property IsReply() As Boolean
        Get
            Return _IsReply
        End Get
        Set(ByVal value As Boolean)
            _IsReply = value
        End Set
    End Property
    Public Property IsExcludeReply() As Boolean
        Get
            Return _IsExcludeReply
        End Get
        Set(ByVal value As Boolean)
            _IsExcludeReply = value
        End Set
    End Property
    Public Property IsProtect() As Boolean
        Get
            Return _IsProtect
        End Get
        Set(ByVal value As Boolean)
            If value Then
                _statuses = _statuses Or Statuses.Protect
            Else
                _statuses = _statuses And Not Statuses.Protect
            End If
            _IsProtect = value
        End Set
    End Property
    Public Property IsOwl() As Boolean
        Get
            Return _IsOWL
        End Get
        Set(ByVal value As Boolean)
            _IsOWL = value
        End Set
    End Property
    Public Property IsMark() As Boolean
        Get
            Return _IsMark
        End Get
        Set(ByVal value As Boolean)
            If value Then
                _statuses = _statuses Or Statuses.Mark
            Else
                _statuses = _statuses And Not Statuses.Mark
            End If
            _IsMark = value
        End Set
    End Property
    Public Property InReplyToUser() As String
        Get
            Return _InReplyToUser
        End Get
        Set(ByVal value As String)
            _InReplyToUser = value
        End Set
    End Property
    Public Property InReplyToId() As Long
        Get
            Return _InReplyToId
        End Get
        Set(ByVal value As Long)
            _InReplyToId = value
        End Set
    End Property
    Public Property Source() As String
        Get
            Return _Source
        End Get
        Set(ByVal value As String)
            _Source = value
        End Set
    End Property
    Public Property ReplyToList() As List(Of String)
        Get
            Return _ReplyToList
        End Get
        Set(ByVal value As List(Of String))
            _ReplyToList = value
        End Set
    End Property
    Public Property IsMe() As Boolean
        Get
            Return _IsMe
        End Get
        Set(ByVal value As Boolean)
            _IsMe = value
        End Set
    End Property
    Public Property IsDm() As Boolean
        Get
            Return _IsDm
        End Get
        Set(ByVal value As Boolean)
            _IsDm = value
        End Set
    End Property
    Public ReadOnly Property StatusIndex() As Integer
        Get
            Return _statuses
        End Get
    End Property
    Public Property Uid() As Long
        Get
            Return _Uid
        End Get
        Set(ByVal value As Long)
            _Uid = value
        End Set
    End Property
    Public Property FilterHit() As Boolean
        Get
            Return _FilterHit
        End Get
        Set(ByVal value As Boolean)
            _FilterHit = value
        End Set
    End Property
    Public Property RetweetedBy() As String
        Get
            Return _RetweetedBy
        End Get
        Set(ByVal value As String)
            _RetweetedBy = value
        End Set
    End Property
    Public Property RetweetedId() As Long
        Get
            Return _RetweetedId
        End Get
        Set(ByVal value As Long)
            _RetweetedId = value
        End Set
    End Property
    Public Property RelTabName() As String
        Get
            Return _searchTabName
        End Get
        Set(ByVal value As String)
            _searchTabName = value
        End Set
    End Property
End Class

Public NotInheritable Class TabInformations
    '個別タブの情報をDictionaryで保持
    Private _sorter As IdComparerClass
    Private _tabs As New Dictionary(Of String, TabClass)
    Private _statuses As New Dictionary(Of Long, PostClass)
    Private _addedIds As List(Of Long)
    Private _retweets As New Dictionary(Of Long, PostClass)
    Private _removedTab As TabClass = Nothing

    '発言の追加
    'AddPost(複数回) -> DistributePosts          -> SubmitUpdate

    'トランザクション用
    Private _addCount As Integer
    Private _soundFile As String
    Private _notifyPosts As List(Of PostClass)
    Private ReadOnly LockObj As New Object
    Private ReadOnly LockUnread As New Object

    Private Shared _instance As TabInformations = New TabInformations

    'List
    Private _lists As New List(Of ListElement)

    Private Sub New()
        _sorter = New IdComparerClass()
    End Sub

    Public Shared Function GetInstance() As TabInformations
        Return _instance    'singleton
    End Function

    Public Property SubscribableLists() As List(Of ListElement)
        Get
            Return _lists
        End Get
        Set(ByVal value As List(Of ListElement))
            If value IsNot Nothing AndAlso value.Count > 0 Then
                For Each tb As TabClass In Me.GetTabsByType(TabUsageType.Lists)
                    For Each list As ListElement In value
                        If tb.ListInfo.Id = list.Id Then
                            tb.ListInfo = list
                            Exit For
                        End If
                    Next
                Next
            End If
            _lists = value
        End Set
    End Property

    Public Sub AddTab(ByVal TabName As String, ByVal TabType As TabUsageType, ByVal List As ListElement)
        _tabs.Add(TabName, New TabClass(TabName, TabType, List))
        _tabs(TabName).Sorter.Mode = _sorter.Mode
        _tabs(TabName).Sorter.Order = _sorter.Order
    End Sub

    'Public Sub AddTab(ByVal TabName As String, ByVal Tab As TabClass)
    '    _tabs.Add(TabName, Tab)
    'End Sub

    Public Sub RemoveTab(ByVal TabName As String)
        SyncLock LockObj
            If IsDefaultTab(TabName) Then Exit Sub '念のため
            If _tabs(TabName).TabType <> TabUsageType.PublicSearch AndAlso _tabs(TabName).TabType <> TabUsageType.Lists Then
                Dim homeTab As TabClass = GetTabByType(TabUsageType.Home)
                Dim dmName As String = GetTabByType(TabUsageType.DirectMessage).TabName

                For idx As Integer = 0 To _tabs(TabName).AllCount - 1
                    Dim exist As Boolean = False
                    Dim Id As Long = _tabs(TabName).GetId(idx)
                    For Each key As String In _tabs.Keys
                        If Not key = TabName AndAlso key <> dmName Then
                            If _tabs(key).Contains(Id) Then
                                exist = True
                                Exit For
                            End If
                        End If
                    Next
                    If Not exist Then homeTab.Add(Id, _statuses(Id).IsRead, False)
                Next
            End If
            If _removedTab IsNot Nothing Then _removedTab = Nothing
            _removedTab = _tabs(TabName)
            _tabs.Remove(TabName)
        End SyncLock
    End Sub

    Public Property RemovedTab() As TabClass
        Get
            Return _removedTab
        End Get
        Set(ByVal value As TabClass)
            _removedTab = value
        End Set
    End Property

    Public Function ContainsTab(ByVal TabText As String) As Boolean
        Return _tabs.ContainsKey(TabText)
    End Function

    Public Function ContainsTab(ByVal ts As TabClass) As Boolean
        Return _tabs.ContainsValue(ts)
    End Function

    Public Property Tabs() As Dictionary(Of String, TabClass)
        Get
            Return _tabs
        End Get
        Set(ByVal value As Dictionary(Of String, TabClass))
            _tabs = value
        End Set
    End Property

    Public ReadOnly Property KeysTab() As Collections.Generic.Dictionary(Of String, TabClass).KeyCollection
        Get
            Return _tabs.Keys
        End Get
    End Property

    Public Sub SortPosts()
        For Each key As String In _tabs.Keys
            _tabs(key).Sort()
        Next
    End Sub

    Public Property SortOrder() As SortOrder
        Get
            Return _sorter.Order
        End Get
        Set(ByVal value As SortOrder)
            _sorter.Order = value
            For Each key As String In _tabs.Keys
                _tabs(key).Sorter.Order = value
            Next
        End Set
    End Property

    Public Property SortMode() As IdComparerClass.ComparerMode
        Get
            Return _sorter.Mode
        End Get
        Set(ByVal value As IdComparerClass.ComparerMode)
            _sorter.Mode = value
            For Each key As String In _tabs.Keys
                _tabs(key).Sorter.Mode = value
            Next
        End Set
    End Property

    Public Function ToggleSortOrder(ByVal SortMode As IdComparerClass.ComparerMode) As Windows.Forms.SortOrder
        If _sorter.Mode = SortMode Then
            If _sorter.Order = Windows.Forms.SortOrder.Ascending Then
                _sorter.Order = Windows.Forms.SortOrder.Descending
            Else
                _sorter.Order = Windows.Forms.SortOrder.Ascending
            End If
            For Each key As String In _tabs.Keys
                _tabs(key).Sorter.Order = _sorter.Order
            Next
        Else
            _sorter.Mode = SortMode
            _sorter.Order = Windows.Forms.SortOrder.Ascending
            For Each key As String In _tabs.Keys
                _tabs(key).Sorter.Mode = SortMode
                _tabs(key).Sorter.Order = Windows.Forms.SortOrder.Ascending
            Next
        End If
        Me.SortPosts()
        Return _sorter.Order
    End Function

    Public ReadOnly Property RetweetSource(ByVal Id As Long) As PostClass
        Get
            If _retweets.ContainsKey(Id) Then
                Return _retweets(Id)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public Sub RemoveFavPost(ByVal Id As Long)
        SyncLock LockObj
            Dim post As PostClass = Nothing
            Dim tab As TabClass = Me.GetTabByType(TabUsageType.Favorites)
            Dim tn As String = tab.TabName
            If _statuses.ContainsKey(Id) Then
                post = _statuses(Id)
                '指定タブから該当ID削除
                Dim tType As TabUsageType = tab.TabType
                If tab.Contains(Id) Then
                    If tab.UnreadManage AndAlso Not post.IsRead Then    '未読管理
                        SyncLock LockUnread
                            tab.UnreadCount -= 1
                            Me.SetNextUnreadId(Id, tab)
                        End SyncLock
                    End If
                    tab.Remove(Id)
                End If
                'FavタブからRetweet発言を削除する場合は、他の同一参照Retweetも削除
                If tType = TabUsageType.Favorites AndAlso post.RetweetedId > 0 Then
                    For i As Integer = 0 To tab.AllCount - 1
                        Dim rPost As PostClass = Nothing
                        Try
                            rPost = Me.Item(tn, i)
                        Catch ex As ArgumentOutOfRangeException
                            Exit For
                        End Try
                        If rPost.RetweetedId > 0 AndAlso rPost.RetweetedId = post.RetweetedId Then
                            If tab.UnreadManage AndAlso Not rPost.IsRead Then    '未読管理
                                SyncLock LockUnread
                                    tab.UnreadCount -= 1
                                    Me.SetNextUnreadId(rPost.Id, tab)
                                End SyncLock
                            End If
                            tab.Remove(rPost.Id)
                        End If
                    Next
                End If
            End If
            ''TabType=PublicSearchの場合（Postの保存先がTabClass内）
            'If tab.Contains(Id) AndAlso _
            '   (tab.TabType = TabUsageType.PublicSearch OrElse tab.TabType = TabUsageType.DirectMessage) Then
            '    post = tab.Posts(Id)
            '    If tab.UnreadManage AndAlso Not post.IsRead Then    '未読管理
            '        SyncLock LockUnread
            '            tab.UnreadCount -= 1
            '            Me.SetNextUnreadId(Id, tab)
            '        End SyncLock
            '    End If
            '    tab.Remove(Id)
            'End If
        End SyncLock
    End Sub

    Public Sub RemovePost(ByVal Id As Long)
        SyncLock LockObj
            Dim post As PostClass = Nothing
            If _statuses.ContainsKey(Id) Then
                post = _statuses(Id)
                '各タブから該当ID削除
                For Each key As String In _tabs.Keys
                    Dim tab As TabClass = _tabs(key)
                    If tab.Contains(Id) Then
                        If tab.UnreadManage AndAlso Not post.IsRead Then    '未読管理
                            SyncLock LockUnread
                                tab.UnreadCount -= 1
                                Me.SetNextUnreadId(Id, tab)
                            End SyncLock
                        End If
                        tab.Remove(Id)
                    End If
                Next
                _statuses.Remove(Id)
            End If
            For Each tb As TabClass In _tabs.Values
                If (tb.TabType = TabUsageType.PublicSearch OrElse tb.TabType = TabUsageType.DirectMessage OrElse tb.TabType = TabUsageType.Lists) _
                   AndAlso tb.Contains(Id) Then
                    post = tb.Posts(Id)
                    If tb.UnreadManage AndAlso Not post.IsRead Then
                        SyncLock LockUnread
                            tb.UnreadCount -= 1
                            Me.SetNextUnreadId(Id, tb)
                        End SyncLock
                    End If
                    tb.Remove(Id)
                End If
            Next
        End SyncLock
    End Sub

    Public Function GetOldestUnreadId(ByVal TabName As String) As Integer
        Dim tb As TabClass = _tabs(TabName)
        If tb.OldestUnreadId > -1 AndAlso _
           tb.Contains(tb.OldestUnreadId) AndAlso _
           tb.UnreadCount > 0 Then
            '未読アイテムへ
            Dim isRead As Boolean
            If tb.TabType <> TabUsageType.PublicSearch AndAlso tb.TabType <> TabUsageType.DirectMessage AndAlso tb.TabType <> TabUsageType.Lists Then
                isRead = _statuses(tb.OldestUnreadId).IsRead
            Else
                isRead = tb.Posts(tb.OldestUnreadId).IsRead
            End If
            If isRead Then
                '状態不整合（最古未読ＩＤが実は既読）
                SyncLock LockUnread
                    Me.SetNextUnreadId(-1, tb)  '頭から探索
                End SyncLock
                If tb.OldestUnreadId = -1 Then
                    Return -1
                Else
                    Return tb.IndexOf(tb.OldestUnreadId)
                End If
            Else
                Return tb.IndexOf(tb.OldestUnreadId)    '最短経路
            End If
        Else
            '一見未読なさそうだが、未読カウントはあるので探索
            'If tb.UnreadCount > 0 Then
            If Not tb.UnreadManage Then Return -1
            SyncLock LockUnread
                Me.SetNextUnreadId(-1, tb)
            End SyncLock
            If tb.OldestUnreadId = -1 Then
                Return -1
            Else
                Return tb.IndexOf(tb.OldestUnreadId)
            End If
            '    Else
            '    Return -1
            'End If
        End If
    End Function

    Private Sub SetNextUnreadId(ByVal CurrentId As Long, ByVal Tab As TabClass)
        'CurrentID:今既読にしたID(OldestIDの可能性あり)
        '最古未読が設定されていて、既読の場合（1発言以上存在）
        Try
            Dim posts As Dictionary(Of Long, PostClass)
            If Tab.TabType <> TabUsageType.PublicSearch AndAlso Tab.TabType <> TabUsageType.DirectMessage AndAlso Tab.TabType <> TabUsageType.Lists Then
                posts = _statuses
            Else
                posts = Tab.Posts
            End If
            If Tab.OldestUnreadId > -1 AndAlso _
               posts.ContainsKey(Tab.OldestUnreadId) AndAlso _
               posts.Item(Tab.OldestUnreadId).IsRead AndAlso _
               _sorter.Mode = IdComparerClass.ComparerMode.Id Then     '次の未読探索
                If Tab.UnreadCount = 0 Then
                    '未読数０→最古未読なし
                    Tab.OldestUnreadId = -1
                ElseIf Tab.OldestUnreadId = CurrentId AndAlso CurrentId > -1 Then
                    '最古IDを既読にしたタイミング→次のIDから続けて探索
                    Dim idx As Integer = Tab.IndexOf(CurrentId)
                    If idx > -1 Then
                        '続きから探索
                        FindUnreadId(idx, Tab)
                    Else
                        '頭から探索
                        FindUnreadId(-1, Tab)
                    End If
                Else
                    '頭から探索
                    FindUnreadId(-1, Tab)
                End If
            Else
                '頭から探索
                FindUnreadId(-1, Tab)
            End If
        Catch ex As Generic.KeyNotFoundException
            '頭から探索
            FindUnreadId(-1, Tab)
        End Try
    End Sub

    Private Sub FindUnreadId(ByVal StartIdx As Integer, ByVal Tab As TabClass)
        If Tab.AllCount = 0 Then
            Tab.OldestUnreadId = -1
            Tab.UnreadCount = 0
            Exit Sub
        End If
        Dim toIdx As Integer = 0
        Dim stp As Integer = 1
        Tab.OldestUnreadId = -1
        If _sorter.Order = Windows.Forms.SortOrder.Ascending Then
            If StartIdx = -1 Then
                StartIdx = 0
            Else
                'StartIdx += 1
                If StartIdx > Tab.AllCount - 1 Then StartIdx = Tab.AllCount - 1 '念のため
            End If
            toIdx = Tab.AllCount - 1
            If toIdx < 0 Then toIdx = 0 '念のため
            stp = 1
        Else
            If StartIdx = -1 Then
                StartIdx = Tab.AllCount - 1
            Else
                'StartIdx -= 1
            End If
            If StartIdx < 0 Then StartIdx = 0 '念のため
            toIdx = 0
            stp = -1
        End If
        If Tab.TabType <> TabUsageType.PublicSearch AndAlso Tab.TabType <> TabUsageType.DirectMessage AndAlso Tab.TabType <> TabUsageType.Lists Then
            For i As Integer = StartIdx To toIdx Step stp
                If Not _statuses(Tab.GetId(i)).IsRead Then
                    Tab.OldestUnreadId = Tab.GetId(i)
                    Exit For
                End If
            Next
        Else
            For i As Integer = StartIdx To toIdx Step stp
                If Not Tab.Posts(Tab.GetId(i)).IsRead Then
                    Tab.OldestUnreadId = Tab.GetId(i)
                    Exit For
                End If
            Next
        End If
    End Sub

    Public Function DistributePosts() As Integer
        SyncLock LockObj
            '戻り値は追加件数
            'If _addedIds Is Nothing Then Return 0
            'If _addedIds.Count = 0 Then Return 0

            If _addedIds Is Nothing Then _addedIds = New List(Of Long)
            If _notifyPosts Is Nothing Then _notifyPosts = New List(Of PostClass)
            Me.Distribute()    'タブに仮振分
            Dim retCnt As Integer = _addedIds.Count
            _addCount += retCnt
            _addedIds.Clear()
            _addedIds = Nothing     '後始末
            Return retCnt     '件数
        End SyncLock
    End Function

    Public Function SubmitUpdate(ByRef soundFile As String, ByRef notifyPosts As PostClass(), ByRef isMentionIncluded As Boolean) As Integer
        '注：メインスレッドから呼ぶこと
        SyncLock LockObj
            If _notifyPosts Is Nothing Then
                soundFile = ""
                notifyPosts = Nothing
                Return 0
            End If

            For Each tb As TabClass In _tabs.Values
                If tb.TabType = TabUsageType.PublicSearch OrElse tb.TabType = TabUsageType.DirectMessage OrElse tb.TabType = TabUsageType.Lists Then
                    _addCount += tb.GetTemporaryCount
                End If
                tb.AddSubmit(isMentionIncluded)  '振分確定（各タブに反映）
            Next
            Me.SortPosts()

            soundFile = _soundFile
            _soundFile = ""
            notifyPosts = _notifyPosts.ToArray()
            _notifyPosts.Clear()
            _notifyPosts = Nothing
            Dim retCnt As Integer = _addCount
            _addCount = 0
            Return retCnt    '件数（EndUpdateの戻り値と同じ）
        End SyncLock
    End Function

    Private Sub Distribute()
        '各タブのフィルターと照合。合致したらタブにID追加
        '通知メッセージ用に、表示必要な発言リストと再生サウンドを返す
        'notifyPosts = New List(Of PostClass)
        Dim homeTab As TabClass = GetTabByType(TabUsageType.Home)
        Dim replyTab As TabClass = GetTabByType(TabUsageType.Mentions)
        Dim dmTab As TabClass = GetTabByType(TabUsageType.DirectMessage)
        Dim favTab As TabClass = GetTabByType(TabUsageType.Favorites)
        For Each id As Long In _addedIds
            Dim post As PostClass = _statuses(id)
            Dim add As Boolean = False  '通知リスト追加フラグ
            Dim mv As Boolean = False   '移動フラグ（Recent追加有無）
            Dim rslt As HITRESULT = HITRESULT.None
            post.IsExcludeReply = False
            For Each tn As String In _tabs.Keys
                rslt = _tabs(tn).AddFiltered(post)
                If rslt <> HITRESULT.None AndAlso rslt <> HITRESULT.Exclude Then
                    If rslt = HITRESULT.CopyAndMark Then post.IsMark = True 'マークあり
                    If rslt = HITRESULT.Move Then
                        mv = True '移動
                        post.IsMark = False
                    End If
                    If _tabs(tn).Notify Then add = True '通知あり
                    If Not _tabs(tn).SoundFile = "" AndAlso _soundFile = "" Then
                        _soundFile = _tabs(tn).SoundFile 'wavファイル（未設定の場合のみ）
                    End If
                    post.FilterHit = True
                Else
                    If rslt = HITRESULT.Exclude AndAlso _tabs(tn).TabType = TabUsageType.Mentions Then
                        post.IsExcludeReply = True
                    End If
                    post.FilterHit = False
                End If
            Next
            If Not mv Then  '移動されなかったらRecentに追加
                homeTab.Add(post.Id, post.IsRead, True)
                If Not homeTab.SoundFile = "" AndAlso _soundFile = "" Then _soundFile = homeTab.SoundFile
                If homeTab.Notify Then add = True
            End If
            If post.IsReply AndAlso Not post.IsExcludeReply Then    '除外ルール適用のないReplyならReplyタブに追加
                replyTab.Add(post.Id, post.IsRead, True)
                If Not replyTab.SoundFile = "" Then _soundFile = replyTab.SoundFile
                If replyTab.Notify Then add = True
            End If
            If post.IsFav Then    'Fav済み発言だったらFavoritesタブに追加
                If favTab.Contains(post.Id) Then
                    '取得済みなら非通知
                    '_soundFile = ""
                    add = False
                Else
                    favTab.Add(post.Id, post.IsRead, True)
                    If Not String.IsNullOrEmpty(favTab.SoundFile) AndAlso String.IsNullOrEmpty(_soundFile) Then _soundFile = favTab.SoundFile
                    If favTab.Notify Then add = True
                End If
            End If
            If add Then _notifyPosts.Add(post)
        Next
        For Each tb As TabClass In _tabs.Values
            If tb.TabType = TabUsageType.PublicSearch OrElse tb.TabType = TabUsageType.DirectMessage OrElse tb.TabType = TabUsageType.Lists Then
                If tb.Notify Then
                    If tb.GetTemporaryCount > 0 Then
                        For Each post As PostClass In tb.GetTemporaryPosts
                            Dim exist As Boolean = False
                            For Each npost As PostClass In _notifyPosts
                                If npost.Id = post.Id Then
                                    exist = True
                                    Exit For
                                End If
                            Next
                            If Not exist Then _notifyPosts.Add(post)
                        Next
                        If tb.SoundFile <> "" Then
                            If tb.TabType = TabUsageType.DirectMessage OrElse _soundFile = "" Then
                                _soundFile = tb.SoundFile
                            End If
                        End If
                    End If
                End If
            End If
        Next
    End Sub

    Public Sub AddPost(ByVal Item As PostClass)
        SyncLock LockObj
            If Item.RelTabName = "" Then
                If Not Item.IsDm Then
                    If _statuses.ContainsKey(Item.Id) Then
                        If Item.IsFav Then
                            _statuses.Item(Item.Id).IsFav = True
                        Else
                            Exit Sub        '追加済みなら何もしない
                        End If
                    Else
                        _statuses.Add(Item.Id, Item)
                    End If
                    If Item.RetweetedId > 0 Then
                        Me.AddRetweet(Item)
                    End If
                    If Item.IsFav AndAlso _retweets.ContainsKey(Item.Id) Then
                        Exit Sub    'Fav済みのRetweet元発言は追加しない
                    End If
                    If _addedIds Is Nothing Then _addedIds = New List(Of Long) 'タブ追加用IDコレクション準備
                    _addedIds.Add(Item.Id)
                Else
                    'DM
                    Dim tb As TabClass = Me.GetTabByType(TabUsageType.DirectMessage)
                    If tb.Contains(Item.Id) Then Exit Sub
                    tb.AddPostToInnerStorage(Item)
                End If
            Else
                '公式検索、リストの場合
                Dim tb As TabClass
                If Me.Tabs.ContainsKey(Item.RelTabName) Then
                    tb = Me.Tabs(Item.RelTabName)
                Else
                    Exit Sub
                End If
                If tb Is Nothing Then Exit Sub
                If tb.Contains(Item.Id) Then Exit Sub
                'tb.Add(Item.Id, Item.IsRead, True)
                tb.AddPostToInnerStorage(Item)
            End If
        End SyncLock
    End Sub

    Private Sub AddRetweet(ByVal item As PostClass)
        If _retweets.ContainsKey(item.RetweetedId) Then Exit Sub

        _retweets.Add( _
                    item.RetweetedId, _
                    New PostClass( _
                        item.Nickname, _
                        item.Data, _
                        item.OriginalData, _
                        item.ImageUrl, _
                        item.Name, _
                        item.PDate, _
                        item.RetweetedId, _
                        item.IsFav, _
                        item.IsRead, _
                        item.IsReply, _
                        item.IsExcludeReply, _
                        item.IsProtect, _
                        item.IsOwl, _
                        item.IsMark, _
                        item.InReplyToUser, _
                        item.InReplyToId, _
                        item.Source, _
                        item.ReplyToList, _
                        item.IsMe, _
                        item.IsDm, _
                        item.Uid, _
                        item.FilterHit, _
                        "", _
                        0 _
                    ) _
                )
    End Sub

    Public Sub SetRead(ByVal Read As Boolean, ByVal TabName As String, ByVal Index As Integer)
        'Read:True=既読へ　False=未読へ
        Dim tb As TabClass = _tabs(TabName)

        If tb.UnreadManage = False Then Exit Sub '未読管理していなければ終了

        Dim Id As Long = tb.GetId(Index)
        Dim post As PostClass
        If tb.TabType <> TabUsageType.PublicSearch AndAlso tb.TabType <> TabUsageType.DirectMessage AndAlso tb.TabType <> TabUsageType.Lists Then
            post = _statuses(Id)
        Else
            post = tb.Posts(Id)
        End If
        If post.IsRead = Read Then Exit Sub '状態変更なければ終了

        post.IsRead = Read '指定の状態に変更

        SyncLock LockUnread
            If Read Then
                tb.UnreadCount -= 1
                Me.SetNextUnreadId(Id, tb)  '次の未読セット
                '他タブの最古未読ＩＤはタブ切り替え時に。
                If tb.TabType = TabUsageType.PublicSearch OrElse tb.TabType = TabUsageType.DirectMessage OrElse tb.TabType = TabUsageType.Lists Then Exit Sub
                For Each key As String In _tabs.Keys
                    If key <> TabName AndAlso _
                       _tabs(key).UnreadManage AndAlso _
                       _tabs(key).Contains(Id) AndAlso _
                       (_tabs(key).TabType <> TabUsageType.PublicSearch AndAlso _tabs(key).TabType <> TabUsageType.DirectMessage AndAlso _tabs(key).TabType <> TabUsageType.Lists) Then
                        _tabs(key).UnreadCount -= 1
                        If _tabs(key).OldestUnreadId = Id Then _tabs(key).OldestUnreadId = -1
                    End If
                Next
            Else
                tb.UnreadCount += 1
                If tb.OldestUnreadId > Id OrElse tb.OldestUnreadId = -1 Then tb.OldestUnreadId = Id
                If tb.TabType = TabUsageType.PublicSearch OrElse tb.TabType = TabUsageType.DirectMessage OrElse tb.TabType = TabUsageType.Lists Then Exit Sub
                For Each key As String In _tabs.Keys
                    If Not key = TabName AndAlso _
                       _tabs(key).UnreadManage AndAlso _
                       _tabs(key).Contains(Id) AndAlso _
                       (_tabs(key).TabType <> TabUsageType.PublicSearch AndAlso _tabs(key).TabType <> TabUsageType.DirectMessage AndAlso _tabs(key).TabType <> TabUsageType.Lists) Then
                        _tabs(key).UnreadCount += 1
                        If _tabs(key).OldestUnreadId > Id Then _tabs(key).OldestUnreadId = Id
                    End If
                Next
            End If
        End SyncLock
    End Sub

    Public Sub SetRead()
        Dim tb As TabClass = GetTabByType(TabUsageType.Home)
        If tb.UnreadManage = False Then Exit Sub

        For i As Integer = 0 To tb.AllCount - 1
            Dim id As Long = tb.GetId(i)
            If Not _statuses(id).IsReply AndAlso _
               Not _statuses(id).IsRead AndAlso _
               Not _statuses(id).FilterHit Then
                _statuses(id).IsRead = True
                Me.SetNextUnreadId(id, tb)  '次の未読セット
                For Each key As String In _tabs.Keys
                    If _tabs(key).UnreadManage AndAlso _
                       _tabs(key).Contains(id) Then
                        _tabs(key).UnreadCount -= 1
                        If _tabs(key).OldestUnreadId = id Then _tabs(key).OldestUnreadId = -1
                    End If
                Next
            End If
        Next
    End Sub

    Public ReadOnly Property Item(ByVal ID As Long) As PostClass
        Get
            If _statuses.ContainsKey(ID) Then Return _statuses(ID)
            For Each tb As TabClass In _tabs.Values
                If (tb.TabType = TabUsageType.PublicSearch OrElse tb.TabType = TabUsageType.DirectMessage OrElse tb.TabType = TabUsageType.Lists) AndAlso _
                   tb.Contains(ID) Then
                    Return tb.Posts(ID)
                End If
            Next
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Item(ByVal TabName As String, ByVal Index As Integer) As PostClass
        Get
            'If Not _tabs.ContainsKey(TabName) Then Return Nothing
            If _tabs(TabName).TabType = TabUsageType.PublicSearch OrElse _tabs(TabName).TabType = TabUsageType.DirectMessage OrElse _tabs(TabName).TabType = TabUsageType.Lists Then
                Return _tabs(TabName).Posts(_tabs(TabName).GetId(Index))
            Else
                Return _statuses(_tabs(TabName).GetId(Index))
            End If
        End Get
    End Property

    Public ReadOnly Property Item(ByVal TabName As String, ByVal StartIndex As Integer, ByVal EndIndex As Integer) As PostClass()
        Get
            Dim length As Integer = EndIndex - StartIndex + 1
            Dim posts() As PostClass = New PostClass(length - 1) {}
            If _tabs(TabName).TabType = TabUsageType.PublicSearch OrElse _tabs(TabName).TabType = TabUsageType.DirectMessage OrElse _tabs(TabName).TabType = TabUsageType.Lists Then
                For i As Integer = 0 To length - 1
                    posts(i) = _tabs(TabName).Posts(_tabs(TabName).GetId(StartIndex + i))
                Next i
            Else
                For i As Integer = 0 To length - 1
                    posts(i) = _statuses(_tabs(TabName).GetId(StartIndex + i))
                Next i
            End If
            Return posts
        End Get
    End Property

    'Public ReadOnly Property ItemCount() As Integer
    '    Get
    '        SyncLock LockObj
    '            Return _statuses.Count   'DM,公式検索は除く
    '        End SyncLock
    '    End Get
    'End Property

    Public Function ContainsKey(ByVal Id As Long) As Boolean
        'DM,公式検索は非対応
        SyncLock LockObj
            Return _statuses.ContainsKey(Id)
        End SyncLock
    End Function

    Public Function ContainsKey(ByVal Id As Long, ByVal TabName As String) As Boolean
        'DM,公式検索は対応版
        SyncLock LockObj
            Return _tabs(TabName).Contains(Id)
        End SyncLock
    End Function

    Public Sub SetUnreadManage(ByVal Manage As Boolean)
        If Manage Then
            For Each key As String In _tabs.Keys
                Dim tb As TabClass = _tabs(key)
                If tb.UnreadManage Then
                    SyncLock LockUnread
                        Dim cnt As Integer = 0
                        Dim oldest As Long = Long.MaxValue
                        Dim posts As Dictionary(Of Long, PostClass)
                        If tb.TabType <> TabUsageType.PublicSearch AndAlso tb.TabType <> TabUsageType.DirectMessage AndAlso tb.TabType <> TabUsageType.Lists Then
                            posts = _statuses
                        Else
                            posts = tb.Posts
                        End If
                        For Each id As Long In tb.BackupIds
                            If Not posts(id).IsRead Then
                                cnt += 1
                                If oldest > id Then oldest = id
                            End If
                        Next
                        If oldest = Long.MaxValue Then oldest = -1
                        tb.OldestUnreadId = oldest
                        tb.UnreadCount = cnt
                    End SyncLock
                End If
            Next
        Else
            For Each key As String In _tabs.Keys
                Dim tb As TabClass = _tabs(key)
                If tb.UnreadManage AndAlso tb.UnreadCount > 0 Then
                    SyncLock LockUnread
                        tb.UnreadCount = 0
                        tb.OldestUnreadId = -1
                    End SyncLock
                End If
            Next
        End If
    End Sub

    Public Sub RenameTab(ByVal Original As String, ByVal NewName As String)
        Dim tb As TabClass = _tabs(Original)
        _tabs.Remove(Original)
        tb.TabName = NewName
        _tabs.Add(NewName, tb)
    End Sub

    Public Sub FilterAll()
        SyncLock LockObj
            Dim tbr As TabClass = GetTabByType(TabUsageType.Home)
            Dim replyTab As TabClass = GetTabByType(TabUsageType.Mentions)
            For Each key As String In _tabs.Keys
                Dim tb As TabClass = _tabs(key)
                If tb.FilterModified Then
                    tb.FilterModified = False
                    Dim orgIds() As Long = tb.BackupIds()
                    tb.ClearIDs()
                    ''''''''''''''フィルター前のIDsを退避。どのタブにも含まれないidはrecentへ追加
                    ''''''''''''''moveフィルターにヒットした際、recentに該当あればrecentから削除
                    For Each id As Long In _statuses.Keys
                        Dim post As PostClass = _statuses.Item(id)
                        If post.IsDm Then Continue For
                        Dim rslt As HITRESULT = HITRESULT.None
                        rslt = tb.AddFiltered(post)
                        Select Case rslt
                            Case HITRESULT.CopyAndMark
                                post.IsMark = True 'マークあり
                                post.FilterHit = True
                            Case HITRESULT.Move
                                tbr.Remove(post.Id, post.IsRead)
                                post.IsMark = False
                                post.FilterHit = True
                            Case HITRESULT.Copy
                                post.IsMark = False
                                post.FilterHit = True
                            Case HITRESULT.Exclude
                                If key = replyTab.TabName AndAlso post.IsReply Then post.IsExcludeReply = True
                                If post.IsFav Then GetTabByType(TabUsageType.Favorites).Add(post.Id, post.IsRead, True)
                                post.FilterHit = False
                            Case HITRESULT.None
                                If key = replyTab.TabName AndAlso post.IsReply Then replyTab.Add(post.Id, post.IsRead, True)
                                If post.IsFav Then GetTabByType(TabUsageType.Favorites).Add(post.Id, post.IsRead, True)
                                post.FilterHit = False
                        End Select
                    Next
                    tb.AddSubmit()  '振分確定
                    For Each id As Long In orgIds
                        Dim hit As Boolean = False
                        For Each tkey As String In _tabs.Keys
                            If _tabs(tkey).Contains(id) Then
                                hit = True
                                Exit For
                            End If
                        Next
                        If Not hit Then tbr.Add(id, _statuses(id).IsRead, False)
                    Next
                End If
            Next
            Me.SortPosts()
        End SyncLock
    End Sub

    Public Function GetId(ByVal TabName As String, ByVal IndexCollection As ListView.SelectedIndexCollection) As Long()
        If IndexCollection.Count = 0 Then Return Nothing

        Dim tb As TabClass = _tabs(TabName)
        Dim Ids(IndexCollection.Count - 1) As Long
        For i As Integer = 0 To Ids.Length - 1
            Ids(i) = tb.GetId(IndexCollection(i))
        Next
        Return Ids
    End Function

    Public Function GetId(ByVal TabName As String, ByVal Index As Integer) As Long
        Return _tabs(TabName).GetId(Index)
    End Function

    Public Function IndexOf(ByVal TabName As String, ByVal Ids() As Long) As Integer()
        If Ids Is Nothing Then Return Nothing
        Dim idx(Ids.Length - 1) As Integer
        Dim tb As TabClass = _tabs(TabName)
        For i As Integer = 0 To Ids.Length - 1
            idx(i) = tb.IndexOf(Ids(i))
        Next
        Return idx
    End Function

    Public Function IndexOf(ByVal TabName As String, ByVal Id As Long) As Integer
        Return _tabs(TabName).IndexOf(Id)
    End Function

    Public Sub ClearTabIds(ByVal TabName As String)
        '不要なPostを削除
        SyncLock LockObj
            If _tabs(TabName).TabType <> TabUsageType.PublicSearch AndAlso _tabs(TabName).TabType <> TabUsageType.DirectMessage AndAlso _tabs(TabName).TabType <> TabUsageType.Lists Then
                For Each Id As Long In _tabs(TabName).BackupIds
                    Dim Hit As Boolean = False
                    For Each tb As TabClass In _tabs.Values
                        If tb.Contains(Id) Then
                            Hit = True
                            Exit For
                        End If
                    Next
                    If Not Hit Then _statuses.Remove(Id)
                Next
            End If

            '指定タブをクリア
            _tabs(TabName).ClearIDs()
        End SyncLock
    End Sub

    Public Sub SetTabUnreadManage(ByVal TabName As String, ByVal Manage As Boolean)
        Dim tb As TabClass = _tabs(TabName)
        SyncLock LockUnread
            If Manage Then
                Dim cnt As Integer = 0
                Dim oldest As Long = Long.MaxValue
                Dim posts As Dictionary(Of Long, PostClass)
                If tb.TabType <> TabUsageType.PublicSearch AndAlso tb.TabType <> TabUsageType.DirectMessage AndAlso tb.TabType <> TabUsageType.Lists Then
                    posts = _statuses
                Else
                    posts = tb.Posts
                End If
                For Each id As Long In tb.BackupIds
                    If Not posts(id).IsRead Then
                        cnt += 1
                        If oldest > id Then oldest = id
                    End If
                Next
                If oldest = Long.MaxValue Then oldest = -1
                tb.OldestUnreadId = oldest
                tb.UnreadCount = cnt
            Else
                tb.OldestUnreadId = -1
                tb.UnreadCount = 0
            End If
        End SyncLock
        tb.UnreadManage = Manage
    End Sub

    Public Sub RefreshOwl(ByVal follower As List(Of Long))
        SyncLock LockObj
            If follower.Count > 0 Then
                For Each post As PostClass In _statuses.Values
                    'If post.Uid = 0 OrElse post.IsDm Then Continue For
                    If post.IsMe Then
                        post.IsOwl = False
                    Else
                        post.IsOwl = Not follower.Contains(post.Uid)
                    End If
                Next
            Else
                For Each id As Long In _statuses.Keys
                    _statuses(id).IsOwl = False
                Next
            End If
        End SyncLock
    End Sub

    Public Function GetTabByType(ByVal tabType As TabUsageType) As TabClass
        'Home,Mentions,DM,Favは1つに制限する
        'その他のタイプを指定されたら、最初に合致したものを返す
        '合致しなければNothingを返す
        SyncLock LockObj
            For Each tb As TabClass In _tabs.Values
                If tb IsNot Nothing AndAlso tb.TabType = tabType Then Return tb
            Next
            Return Nothing
        End SyncLock
    End Function

    Public Function GetTabsByType(ByVal tabType As TabUsageType) As List(Of TabClass)
        '合致したタブをListで返す
        '合致しなければ空のListを返す
        SyncLock LockObj
            Dim tbs As New List(Of TabClass)
            For Each tb As TabClass In _tabs.Values
                If (tabType And tb.TabType) = tb.TabType Then tbs.Add(tb)
            Next
            Return tbs
        End SyncLock
    End Function

    Public Function GetTabByName(ByVal tabName As String) As TabClass
        SyncLock LockObj
            If _tabs.ContainsKey(tabName) Then Return _tabs(tabName)
            Return Nothing
        End SyncLock
    End Function

    ' デフォルトタブの判定処理
    Public Function IsDefaultTab(ByVal tabName As String) As Boolean
        If tabName IsNot Nothing AndAlso _
           _tabs.ContainsKey(tabName) AndAlso _
           (_tabs(tabName).TabType = TabUsageType.Home OrElse _
           _tabs(tabName).TabType = TabUsageType.Mentions OrElse _
           _tabs(tabName).TabType = TabUsageType.DirectMessage OrElse _
           _tabs(tabName).TabType = TabUsageType.Favorites) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetUniqueTabName() As String
        Dim tabNameTemp As String = "MyTab" + (_tabs.Count + 1).ToString
        For i As Integer = 2 To 100
            If _tabs.ContainsKey(tabNameTemp) Then
                tabNameTemp = "MyTab" + (_tabs.Count + i).ToString
            Else
                Exit For
            End If
        Next
        Return tabNameTemp
    End Function

    Public ReadOnly Property Posts() As Dictionary(Of Long, PostClass)
        Get
            Return _statuses
        End Get
    End Property
End Class

<Serializable()> _
Public NotInheritable Class TabClass
    Private _unreadManage As Boolean = False
    Private _notify As Boolean = False
    Private _soundFile As String = ""
    Private _filters As List(Of FiltersClass)
    Private _oldestUnreadItem As Long = -1     'ID
    Private _unreadCount As Integer = 0
    Private _ids As List(Of Long)
    Private _filterMod As Boolean = False
    Private _tmpIds As New List(Of TemporaryId)
    Private _tabName As String = ""
    Private _tabType As TabUsageType = TabUsageType.Undefined
    Private _posts As New Dictionary(Of Long, PostClass)
    Private _sorter As New IdComparerClass
    Private _oldestId As Long = Long.MaxValue   '古いポスト取得用
    Private _sinceId As Long = 0

#Region "検索"
    'Search query
    Private _searchLang As String = ""
    Private _searchWords As String = ""

    Public Property SearchLang() As String
        Get
            Return _searchLang
        End Get
        Set(ByVal value As String)
            _sinceId = 0
            _searchLang = value
        End Set
    End Property
    Public Property SearchWords() As String
        Get
            Return _searchWords
        End Get
        Set(ByVal value As String)
            _sinceId = 0
            _searchWords = value.Trim
        End Set
    End Property
    Public ReadOnly Property SearchPage() As Integer
        Get
            Return ((_ids.Count \ 40) + 1)
        End Get
    End Property
    Private _beforeQuery As New Dictionary(Of String, String)
    Public Sub SaveQuery(ByVal more As Boolean)
        Dim qry As New Dictionary(Of String, String)
        If String.IsNullOrEmpty(_searchWords) Then
            _beforeQuery = qry
            Exit Sub
        End If
        qry.Add("q", _searchWords)
        If Not String.IsNullOrEmpty(_searchLang) Then qry.Add("lang", _searchLang)
        _beforeQuery = qry
    End Sub

    Public Function IsQueryChanged() As Boolean
        Dim qry As New Dictionary(Of String, String)
        If Not String.IsNullOrEmpty(_searchWords) Then
            qry.Add("q", _searchWords)
            If Not String.IsNullOrEmpty(_searchLang) Then qry.Add("lang", _searchLang)
        End If
        If qry.Count <> _beforeQuery.Count Then Return True

        For Each kvp As KeyValuePair(Of String, String) In qry
            If Not _beforeQuery.ContainsKey(kvp.Key) OrElse _beforeQuery(kvp.Key) <> kvp.Value Then
                Return True
            End If
        Next
    End Function
#End Region

#Region "リスト"
    Private _listInfo As ListElement
    Public Property ListInfo() As ListElement
        Get
            Return _listInfo
        End Get
        Set(ByVal value As ListElement)
            _listInfo = value
        End Set
    End Property
#End Region

    <Xml.Serialization.XmlIgnore()> _
    Public Property OldestId() As Long
        Get
            Return _oldestId
        End Get
        Set(ByVal value As Long)
            _oldestId = value
        End Set
    End Property

    <Xml.Serialization.XmlIgnore()> _
    Public Property SinceId() As Long
        Get
            Return _sinceId
        End Get
        Set(ByVal value As Long)
            _sinceId = value
        End Set
    End Property

    <Xml.Serialization.XmlIgnore()> _
    Public Property Posts() As Dictionary(Of Long, PostClass)
        Get
            Return _posts
        End Get
        Set(ByVal value As Dictionary(Of Long, PostClass))
            _posts = value
        End Set
    End Property

    'Public Function SearchedPost(ByVal Id As Long) As PostClass
    '    If Not _posts.ContainsKey(Id) Then Return Nothing
    '    Return _posts(Id)
    'End Function

    Public Function GetTemporaryPosts() As PostClass()
        Dim tempPosts As New List(Of PostClass)
        If _tmpIds.Count = 0 Then Return tempPosts.ToArray
        For Each tempId As TemporaryId In _tmpIds
            tempPosts.Add(_posts(tempId.Id))
        Next
        Return tempPosts.ToArray
    End Function

    Public Function GetTemporaryCount() As Integer
        Return _tmpIds.Count
    End Function

    Private Structure TemporaryId
        Public Id As Long
        Public Read As Boolean

        Public Sub New(ByVal argId As Long, ByVal argRead As Boolean)
            Id = argId
            Read = argRead
        End Sub
    End Structure

    Public Sub New()
        _filters = New List(Of FiltersClass)
        _notify = True
        _soundFile = ""
        _unreadManage = True
        _ids = New List(Of Long)
        _oldestUnreadItem = -1
        _tabType = TabUsageType.Undefined
        _listInfo = Nothing
    End Sub

    Public Sub New(ByVal TabName As String, ByVal TabType As TabUsageType, ByVal list As ListElement)
        _tabName = TabName
        _filters = New List(Of FiltersClass)
        _notify = True
        _soundFile = ""
        _unreadManage = True
        _ids = New List(Of Long)
        _oldestUnreadItem = -1
        _tabType = TabType
        Me.ListInfo = list
        If TabType = TabUsageType.PublicSearch OrElse TabType = TabUsageType.DirectMessage OrElse TabType = TabUsageType.Lists Then
            _sorter.posts = _posts
        Else
            _sorter.posts = TabInformations.GetInstance.Posts
        End If
    End Sub

    Public Sub Sort()
        _ids.Sort(_sorter.CmpMethod)
    End Sub

    Public ReadOnly Property Sorter() As IdComparerClass
        Get
            Return _sorter
        End Get
    End Property

    '無条件に追加
    Private Sub Add(ByVal ID As Long, ByVal Read As Boolean)
        If Me._ids.Contains(ID) Then Exit Sub

        Me._ids.Add(ID)

        If Not Read AndAlso Me._unreadManage Then
            Me._unreadCount += 1
            If Me._oldestUnreadItem = -1 Then
                Me._oldestUnreadItem = ID
            Else
                If ID < Me._oldestUnreadItem Then Me._oldestUnreadItem = ID
            End If
        End If
    End Sub

    Public Sub Add(ByVal ID As Long, ByVal Read As Boolean, ByVal Temporary As Boolean)
        If Not Temporary Then
            Me.Add(ID, Read)
        Else
            _tmpIds.Add(New TemporaryId(ID, Read))
        End If
    End Sub

    'フィルタに合致したら追加
    Public Function AddFiltered(ByVal post As PostClass) As HITRESULT
        If Me.TabType = TabUsageType.PublicSearch OrElse Me.TabType = TabUsageType.DirectMessage OrElse Me.TabType = TabUsageType.Lists Then Return HITRESULT.None

        Dim rslt As HITRESULT = HITRESULT.None
        '全フィルタ評価（優先順位あり）
        For Each ft As FiltersClass In _filters
            Select Case ft.IsHit(post)   'フィルタクラスでヒット判定
                Case HITRESULT.None
                Case HITRESULT.Copy
                    If rslt <> HITRESULT.CopyAndMark Then rslt = HITRESULT.Copy
                Case HITRESULT.CopyAndMark
                    rslt = HITRESULT.CopyAndMark
                Case HITRESULT.Move
                    rslt = HITRESULT.Move
                Case HITRESULT.Exclude
                    rslt = HITRESULT.Exclude
                    Exit For
            End Select
        Next

        If rslt <> HITRESULT.None AndAlso rslt <> HITRESULT.Exclude Then
            _tmpIds.Add(New TemporaryId(post.Id, post.IsRead))
        End If

        Return rslt 'マーク付けは呼び出し元で行うこと
    End Function

    '検索結果の追加
    Public Sub AddPostToInnerStorage(ByVal Post As PostClass)
        If _posts.ContainsKey(Post.Id) Then Exit Sub
        _posts.Add(Post.Id, Post)
        _tmpIds.Add(New TemporaryId(Post.Id, Post.IsRead))
    End Sub

    Public Sub AddSubmit(ByRef isMentionIncluded As Boolean)
        If _tmpIds.Count = 0 Then Exit Sub
        For Each tId As TemporaryId In _tmpIds
            If Me.TabType = TabUsageType.Mentions AndAlso TabInformations.GetInstance.Item(tId.Id).IsReply Then isMentionIncluded = True
            Me.Add(tId.Id, tId.Read)
        Next
        _tmpIds.Clear()
    End Sub

    Public Sub AddSubmit()
        Dim mention As Boolean
        AddSubmit(mention)
    End Sub

    Public Sub Remove(ByVal Id As Long)
        If Not Me._ids.Contains(Id) Then Exit Sub
        Me._ids.Remove(Id)
        If Me.TabType = TabUsageType.PublicSearch OrElse Me.TabType = TabUsageType.DirectMessage OrElse Me.TabType = TabUsageType.Lists Then _posts.Remove(Id)
    End Sub

    Public Sub Remove(ByVal Id As Long, ByVal Read As Boolean)
        If Not Me._ids.Contains(Id) Then Exit Sub

        If Not Read AndAlso Me._unreadManage Then
            Me._unreadCount -= 1
            Me._oldestUnreadItem = -1
        End If

        Me._ids.Remove(Id)
        If Me.TabType = TabUsageType.PublicSearch OrElse Me.TabType = TabUsageType.DirectMessage OrElse Me.TabType = TabUsageType.Lists Then _posts.Remove(Id)
    End Sub

    Public Property UnreadManage() As Boolean
        Get
            Return _unreadManage
        End Get
        Set(ByVal value As Boolean)
            Me._unreadManage = value
            If Not value Then
                Me._oldestUnreadItem = -1
                Me._unreadCount = 0
            End If
        End Set
    End Property

    Public Property Notify() As Boolean
        Get
            Return _notify
        End Get
        Set(ByVal value As Boolean)
            _notify = value
        End Set
    End Property

    Public Property SoundFile() As String
        Get
            Return _soundFile
        End Get
        Set(ByVal value As String)
            _soundFile = value
        End Set
    End Property

    <Xml.Serialization.XmlIgnore()> _
    Public Property OldestUnreadId() As Long
        Get
            Return _oldestUnreadItem
        End Get
        Set(ByVal value As Long)
            _oldestUnreadItem = value
        End Set
    End Property

    <Xml.Serialization.XmlIgnore()> _
    Public Property UnreadCount() As Integer
        Get
            Return _unreadCount
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then value = 0
            _unreadCount = value
        End Set
    End Property

    Public ReadOnly Property AllCount() As Integer
        Get
            Return Me._ids.Count
        End Get
    End Property

    Public Function GetFilters() As FiltersClass()
        Return _filters.ToArray()
    End Function

    Public Sub RemoveFilter(ByVal filter As FiltersClass)
        _filters.Remove(filter)
        _filterMod = True
    End Sub

    Public Function AddFilter(ByVal filter As FiltersClass) As Boolean
        If _filters.Contains(filter) Then Return False
        _filters.Add(filter)
        _filterMod = True
        Return True
    End Function

    Public Sub EditFilter(ByVal original As FiltersClass, ByVal modified As FiltersClass)
        original.BodyFilter = modified.BodyFilter
        original.NameFilter = modified.NameFilter
        original.SearchBoth = modified.SearchBoth
        original.SearchUrl = modified.SearchUrl
        original.UseRegex = modified.UseRegex
        original.CaseSensitive = modified.CaseSensitive
        original.IsRt = modified.IsRt
        original.Source = modified.Source
        original.ExBodyFilter = modified.ExBodyFilter
        original.ExNameFilter = modified.ExNameFilter
        original.ExSearchBoth = modified.ExSearchBoth
        original.ExSearchUrl = modified.ExSearchUrl
        original.ExUseRegex = modified.ExUseRegex
        original.ExCaseSensitive = modified.ExCaseSensitive
        original.IsExRt = modified.IsExRt
        original.ExSource = modified.ExSource
        original.MoveFrom = modified.MoveFrom
        original.SetMark = modified.SetMark
        _filterMod = True
    End Sub

    <Xml.Serialization.XmlIgnore()> _
    Public Property Filters() As List(Of FiltersClass)
        Get
            Return _filters
        End Get
        Set(ByVal value As List(Of FiltersClass))
            _filters = value
        End Set
    End Property

    Public Property FilterArray() As FiltersClass()
        Get
            Return _filters.ToArray
        End Get
        Set(ByVal value As FiltersClass())
            For Each filters As FiltersClass In value
                _filters.Add(filters)
            Next
        End Set
    End Property

    Public Function Contains(ByVal ID As Long) As Boolean
        Return _ids.Contains(ID)
    End Function

    Public Sub ClearIDs()
        _ids.Clear()
        _tmpIds.Clear()
        _unreadCount = 0
        _oldestUnreadItem = -1
        If _posts IsNot Nothing Then
            _posts.Clear()
        End If
    End Sub

    Public Function GetId(ByVal Index As Integer) As Long
        Return _ids(Index)
    End Function

    Public Function IndexOf(ByVal ID As Long) As Integer
        Return _ids.IndexOf(ID)
    End Function

    <Xml.Serialization.XmlIgnore()> _
    Public Property FilterModified() As Boolean
        Get
            Return _filterMod
        End Get
        Set(ByVal value As Boolean)
            _filterMod = value
        End Set
    End Property

    Public Function BackupIds() As Long()
        Return _ids.ToArray()
    End Function

    Public Property TabName() As String
        Get
            Return _tabName
        End Get
        Set(ByVal value As String)
            _tabName = value
        End Set
    End Property

    Public Property TabType() As TabUsageType
        Get
            Return _tabType
        End Get
        Set(ByVal value As TabUsageType)
            _tabType = value
            If _tabType = TabUsageType.PublicSearch OrElse _tabType = TabUsageType.DirectMessage OrElse _tabType = TabUsageType.Lists Then
                _sorter.posts = _posts
            Else
                _sorter.posts = TabInformations.GetInstance.Posts
            End If
        End Set
    End Property

End Class

<Serializable()> _
Public NotInheritable Class FiltersClass
    Implements System.IEquatable(Of FiltersClass)
    Private _name As String = ""
    Private _body As New List(Of String)
    Private _searchBoth As Boolean = True
    Private _searchUrl As Boolean = False
    Private _caseSensitive As Boolean = False
    Private _useRegex As Boolean = False
    Private _isRt As Boolean = False
    Private _source As String = ""
    Private _exname As String = ""
    Private _exbody As New List(Of String)
    Private _exsearchBoth As Boolean = True
    Private _exsearchUrl As Boolean = False
    Private _exuseRegex As Boolean = False
    Private _excaseSensitive As Boolean = False
    Private _isExRt As Boolean = False
    Private _exSource As String = ""
    Private _moveFrom As Boolean = False
    Private _setMark As Boolean = True

    Public Sub New()

    End Sub

    'フィルタ一覧に表示する文言生成
    Private Function MakeSummary() As String
        Dim fs As New StringBuilder()
        If Not String.IsNullOrEmpty(_name) OrElse _body.Count > 0 OrElse _isRt OrElse Not String.IsNullOrEmpty(_source) Then
            If _searchBoth Then
                If Not String.IsNullOrEmpty(_name) Then
                    fs.AppendFormat(My.Resources.SetFiltersText1, _name)
                Else
                    fs.Append(My.Resources.SetFiltersText2)
                End If
            End If
            If _body.Count > 0 Then
                fs.Append(My.Resources.SetFiltersText3)
                For Each bf As String In _body
                    fs.Append(bf)
                    fs.Append(" ")
                Next
                fs.Length -= 1
                fs.Append(My.Resources.SetFiltersText4)
            End If
            fs.Append("(")
            If _searchBoth Then
                fs.Append(My.Resources.SetFiltersText5)
            Else
                fs.Append(My.Resources.SetFiltersText6)
            End If
            If _useRegex Then
                fs.Append(My.Resources.SetFiltersText7)
            End If
            If _searchUrl Then
                fs.Append(My.Resources.SetFiltersText8)
            End If
            If _caseSensitive Then
                fs.Append(My.Resources.SetFiltersText13)
            End If
            If _isRt Then
                fs.Append("RT/")
            End If
            If Not String.IsNullOrEmpty(_source) Then
                fs.AppendFormat("Src…{0}/", _source)
            End If
            fs.Length -= 1
            fs.Append(")")
        End If
        If Not String.IsNullOrEmpty(_exname) OrElse _exbody.Count > 0 OrElse _isExRt OrElse Not String.IsNullOrEmpty(_exSource) Then
            '除外
            fs.Append(My.Resources.SetFiltersText12)
            If _exsearchBoth Then
                If Not String.IsNullOrEmpty(_exname) Then
                    fs.AppendFormat(My.Resources.SetFiltersText1, _exname)
                Else
                    fs.Append(My.Resources.SetFiltersText2)
                End If
            End If
            If _exbody.Count > 0 Then
                fs.Append(My.Resources.SetFiltersText3)
                For Each bf As String In _exbody
                    fs.Append(bf)
                    fs.Append(" ")
                Next
                fs.Length -= 1
                fs.Append(My.Resources.SetFiltersText4)
            End If
            fs.Append("(")
            If _exsearchBoth Then
                fs.Append(My.Resources.SetFiltersText5)
            Else
                fs.Append(My.Resources.SetFiltersText6)
            End If
            If _exuseRegex Then
                fs.Append(My.Resources.SetFiltersText7)
            End If
            If _exsearchUrl Then
                fs.Append(My.Resources.SetFiltersText8)
            End If
            If _excaseSensitive Then
                fs.Append(My.Resources.SetFiltersText13)
            End If
            If _isExRt Then
                fs.Append("RT/")
            End If
            If Not String.IsNullOrEmpty(_exSource) Then
                fs.AppendFormat("Src…{0}/", _exSource)
            End If
            fs.Length -= 1
            fs.Append(")")
        End If

        fs.Append("(")
        If _moveFrom Then
            fs.Append(My.Resources.SetFiltersText9)
        Else
            fs.Append(My.Resources.SetFiltersText11)
        End If
        If Not _moveFrom AndAlso _setMark Then
            fs.Append(My.Resources.SetFiltersText10)
        ElseIf Not _moveFrom Then
            fs.Length -= 1
        End If

        fs.Append(")")

        Return fs.ToString()
    End Function

    Public Property NameFilter() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Property ExNameFilter() As String
        Get
            Return _exname
        End Get
        Set(ByVal value As String)
            _exname = value
        End Set
    End Property

    <Xml.Serialization.XmlIgnore()> _
    Public Property BodyFilter() As List(Of String)
        Get
            Return _body
        End Get
        Set(ByVal value As List(Of String))
            _body = value
        End Set
    End Property

    Public Property BodyFilterArray() As String()
        Get
            Return _body.ToArray
        End Get
        Set(ByVal value As String())
            _body = New List(Of String)
            For Each filter As String In value
                _body.Add(filter)
            Next
        End Set
    End Property

    <Xml.Serialization.XmlIgnore()> _
    Public Property ExBodyFilter() As List(Of String)
        Get
            Return _exbody
        End Get
        Set(ByVal value As List(Of String))
            _exbody = value
        End Set
    End Property

    Public Property ExBodyFilterArray() As String()
        Get
            Return _exbody.ToArray
        End Get
        Set(ByVal value As String())
            _exbody = New List(Of String)
            For Each filter As String In value
                _exbody.Add(filter)
            Next
        End Set
    End Property

    Public Property SearchBoth() As Boolean
        Get
            Return _searchBoth
        End Get
        Set(ByVal value As Boolean)
            _searchBoth = value
        End Set
    End Property

    Public Property ExSearchBoth() As Boolean
        Get
            Return _exsearchBoth
        End Get
        Set(ByVal value As Boolean)
            _exsearchBoth = value
        End Set
    End Property

    Public Property MoveFrom() As Boolean
        Get
            Return _moveFrom
        End Get
        Set(ByVal value As Boolean)
            _moveFrom = value
        End Set
    End Property

    Public Property SetMark() As Boolean
        Get
            Return _setMark
        End Get
        Set(ByVal value As Boolean)
            _setMark = value
        End Set
    End Property

    Public Property SearchUrl() As Boolean
        Get
            Return _searchUrl
        End Get
        Set(ByVal value As Boolean)
            _searchUrl = value
        End Set
    End Property

    Public Property ExSearchUrl() As Boolean
        Get
            Return _exsearchUrl
        End Get
        Set(ByVal value As Boolean)
            _exsearchUrl = value
        End Set
    End Property

    Public Property CaseSensitive() As Boolean
        Get
            Return _caseSensitive
        End Get
        Set(ByVal value As Boolean)
            _caseSensitive = value
        End Set
    End Property

    Public Property ExCaseSensitive() As Boolean
        Get
            Return _excaseSensitive
        End Get
        Set(ByVal value As Boolean)
            _excaseSensitive = value
        End Set
    End Property

    Public Property UseRegex() As Boolean
        Get
            Return _useRegex
        End Get
        Set(ByVal value As Boolean)
            _useRegex = value
        End Set
    End Property

    Public Property ExUseRegex() As Boolean
        Get
            Return _exuseRegex
        End Get
        Set(ByVal value As Boolean)
            _exuseRegex = value
        End Set
    End Property

    Public Property IsRt() As Boolean
        Get
            Return _isRt
        End Get
        Set(ByVal value As Boolean)
            _isRt = value
        End Set
    End Property

    Public Property IsExRt() As Boolean
        Get
            Return _isExRt
        End Get
        Set(ByVal value As Boolean)
            _isExRt = value
        End Set
    End Property

    Public Property Source() As String
        Get
            Return _source
        End Get
        Set(ByVal value As String)
            _source = value
        End Set
    End Property

    Public Property ExSource() As String
        Get
            Return _exSource
        End Get
        Set(ByVal value As String)
            _exSource = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return MakeSummary()
    End Function

    Public Function IsHit(ByVal post As PostClass) As HITRESULT
        Dim bHit As Boolean = True
        Dim tBody As String
        If _searchUrl Then
            tBody = post.OriginalData
        Else
            tBody = post.Data
        End If
        '検索オプション
        Dim compOpt As System.StringComparison
        Dim rgOpt As System.Text.RegularExpressions.RegexOptions
        If _caseSensitive Then
            compOpt = StringComparison.Ordinal
            rgOpt = RegexOptions.None
        Else
            compOpt = StringComparison.OrdinalIgnoreCase
            rgOpt = RegexOptions.IgnoreCase
        End If
        If _searchBoth Then
            If String.IsNullOrEmpty(_name) OrElse
                (Not _useRegex AndAlso
                 (post.Name.Equals(_name, compOpt) OrElse
                  post.RetweetedBy.Equals(_name, compOpt)
                 )
                ) OrElse
                (_useRegex AndAlso
                 (Regex.IsMatch(post.Name, _name, rgOpt) OrElse
                  (Not String.IsNullOrEmpty(post.RetweetedBy) AndAlso Regex.IsMatch(post.RetweetedBy, _name, rgOpt))
                 )
                ) Then
                For Each fs As String In _body
                    If _useRegex Then
                        If Not Regex.IsMatch(tBody, fs, rgOpt) Then bHit = False
                    Else
                        If _caseSensitive Then
                            If Not tBody.Contains(fs) Then bHit = False
                        Else
                            If Not tBody.ToLower().Contains(fs.ToLower()) Then bHit = False
                        End If
                    End If
                    If Not bHit Then Exit For
                Next
            Else
                bHit = False
            End If
        Else
            For Each fs As String In _body
                If _useRegex Then
                    If Not (Regex.IsMatch(post.Name, fs, rgOpt) OrElse
                            (Not String.IsNullOrEmpty(post.RetweetedBy) AndAlso Regex.IsMatch(post.RetweetedBy, fs, rgOpt)) OrElse
                            Regex.IsMatch(tBody, fs, rgOpt)) Then bHit = False
                Else
                    If _caseSensitive Then
                        If Not (post.Name.Contains(fs) OrElse _
                                post.RetweetedBy.Contains(fs) OrElse _
                                tBody.Contains(fs)) Then bHit = False
                    Else
                        If Not (post.Name.ToLower().Contains(fs.ToLower()) OrElse _
                                post.RetweetedBy.ToLower().Contains(fs.ToLower()) OrElse _
                                tBody.ToLower().Contains(fs.ToLower())) Then bHit = False
                    End If
                End If
                If Not bHit Then Exit For
            Next
        End If
        If _isRt Then
            If post.RetweetedId = 0 Then bHit = False
        End If
        If Not String.IsNullOrEmpty(_source) Then
            If _useRegex Then
                If Not Regex.IsMatch(post.Source, _source, rgOpt) Then bHit = False
            Else
                If Not post.Source.Equals(_source, compOpt) Then bHit = False
            End If
        End If
        If bHit Then
            '除外判定
            If _exsearchUrl Then
                tBody = post.OriginalData
            Else
                tBody = post.Data
            End If

            Dim exFlag As Boolean = False
            If Not String.IsNullOrEmpty(_exname) OrElse _exbody.Count > 0 Then
                If _excaseSensitive Then
                    compOpt = StringComparison.Ordinal
                    rgOpt = RegexOptions.None
                Else
                    compOpt = StringComparison.OrdinalIgnoreCase
                    rgOpt = RegexOptions.IgnoreCase
                End If
                If _exsearchBoth Then
                    If String.IsNullOrEmpty(_exname) OrElse
                        (Not _exuseRegex AndAlso
                         (post.Name.Equals(_exname, compOpt) OrElse
                          post.RetweetedBy.Equals(_exname, compOpt)
                         )
                        ) OrElse
                        (_exuseRegex AndAlso _
                            (Regex.IsMatch(post.Name, _exname, rgOpt) OrElse _
                             (Not String.IsNullOrEmpty(post.RetweetedBy) AndAlso Regex.IsMatch(post.RetweetedBy, _exname, rgOpt))
                            )
                        ) Then
                        If _exbody.Count > 0 Then
                            For Each fs As String In _exbody
                                If _exuseRegex Then
                                    If Regex.IsMatch(tBody, fs, rgOpt) Then exFlag = True
                                Else
                                    If _excaseSensitive Then
                                        If tBody.Contains(fs) Then exFlag = True
                                    Else
                                        If tBody.ToLower().Contains(fs.ToLower()) Then exFlag = True
                                    End If
                                End If
                                If exFlag Then Exit For
                            Next
                        Else
                            exFlag = True
                        End If
                    End If
                Else
                    For Each fs As String In _exbody
                        If _exuseRegex Then
                            If Regex.IsMatch(post.Name, fs, rgOpt) OrElse
                               (Not String.IsNullOrEmpty(post.RetweetedBy) AndAlso Regex.IsMatch(post.RetweetedBy, fs, rgOpt)) OrElse
                               Regex.IsMatch(tBody, fs, rgOpt) Then exFlag = True
                        Else
                            If _excaseSensitive Then
                                If post.Name.Contains(fs) OrElse _
                                   post.RetweetedBy.Contains(fs) OrElse _
                                   tBody.Contains(fs) Then exFlag = True
                            Else
                                If post.Name.ToLower().Contains(fs.ToLower()) OrElse _
                                   post.RetweetedBy.ToLower().Contains(fs.ToLower()) OrElse _
                                   tBody.ToLower().Contains(fs.ToLower()) Then exFlag = True
                            End If
                        End If
                        If exFlag Then Exit For
                    Next
                End If
            End If
            If _isExRt Then
                If post.RetweetedId > 0 Then exFlag = True
            End If
            If Not String.IsNullOrEmpty(_exSource) Then
                If _exuseRegex Then
                    If Regex.IsMatch(post.Source, _exSource, rgOpt) Then exFlag = True
                Else
                    If post.Source.Equals(_exSource, compOpt) Then exFlag = True
                End If
            End If

            If String.IsNullOrEmpty(_name) AndAlso _body.Count = 0 AndAlso Not _isRt AndAlso String.IsNullOrEmpty(_source) Then
                bHit = False
            End If
            If bHit Then
                If Not exFlag Then
                    If _moveFrom Then
                        Return HITRESULT.Move
                    Else
                        If _setMark Then
                            Return HITRESULT.CopyAndMark
                        End If
                        Return HITRESULT.Copy
                    End If
                Else
                    Return HITRESULT.Exclude
                End If
            Else
                If exFlag Then
                    Return HITRESULT.Exclude
                Else
                    Return HITRESULT.None
                End If
            End If
        Else
            Return HITRESULT.None
        End If
    End Function

    Public Overloads Function Equals(ByVal other As FiltersClass) As Boolean _
     Implements System.IEquatable(Of Tween.FiltersClass).Equals

        If Me.BodyFilter.Count <> other.BodyFilter.Count Then Return False
        If Me.ExBodyFilter.Count <> other.ExBodyFilter.Count Then Return False
        For i As Integer = 0 To Me.BodyFilter.Count - 1
            If Me.BodyFilter(i) <> other.BodyFilter(i) Then Return False
        Next
        For i As Integer = 0 To Me.ExBodyFilter.Count - 1
            If Me.ExBodyFilter(i) <> other.ExBodyFilter(i) Then Return False
        Next

        Return (Me.MoveFrom = other.MoveFrom) And _
               (Me.SetMark = other.SetMark) And _
               (Me.NameFilter = other.NameFilter) And _
               (Me.SearchBoth = other.SearchBoth) And _
               (Me.SearchUrl = other.SearchUrl) And _
               (Me.UseRegex = other.UseRegex) And _
               (Me.ExNameFilter = other.ExNameFilter) And _
               (Me.ExSearchBoth = other.ExSearchBoth) And _
               (Me.ExSearchUrl = other.ExSearchUrl) And _
               (Me.ExUseRegex = other.ExUseRegex) And _
               (Me.IsRt = other.IsRt) And _
               (Me.Source = other.Source) And _
               (Me.IsExRt = other.IsExRt) And _
               (Me.ExSource = other.ExSource)
    End Function

    Public Function CopyTo(ByVal destination As FiltersClass) As FiltersClass

        If Me.BodyFilter.Count > 0 Then
            For Each flt As String In Me.BodyFilter
                destination.BodyFilter.Add(String.Copy(flt))
            Next
        End If

        If Me.ExBodyFilter.Count > 0 Then
            For Each flt As String In Me.ExBodyFilter
                destination.ExBodyFilter.Add(String.Copy(flt))
            Next
        End If

        destination.MoveFrom = Me.MoveFrom
        destination.SetMark = Me.SetMark
        destination.NameFilter = Me.NameFilter
        destination.SearchBoth = Me.SearchBoth
        destination.SearchUrl = Me.SearchUrl
        destination.UseRegex = Me.UseRegex
        destination.ExNameFilter = Me.ExNameFilter
        destination.ExSearchBoth = Me.ExSearchBoth
        destination.ExSearchUrl = Me.ExSearchUrl
        destination.ExUseRegex = Me.ExUseRegex
        destination.IsRt = Me.IsRt
        destination.Source = Me.Source
        destination.IsExRt = Me.IsExRt
        destination.ExSource = Me.ExSource
        Return destination
    End Function

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If (obj Is Nothing) OrElse Not (Me.GetType() Is obj.GetType()) Then Return False
        Return Me.Equals(CType(obj, FiltersClass))
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.MoveFrom.GetHashCode Xor _
               Me.SetMark.GetHashCode Xor _
               Me.BodyFilter.GetHashCode Xor _
               Me.NameFilter.GetHashCode Xor _
               Me.SearchBoth.GetHashCode Xor _
               Me.SearchUrl.GetHashCode Xor _
               Me.UseRegex.GetHashCode Xor _
               Me.ExBodyFilter.GetHashCode Xor _
               Me.ExNameFilter.GetHashCode Xor _
               Me.ExSearchBoth.GetHashCode Xor _
               Me.ExSearchUrl.GetHashCode Xor _
               Me.ExUseRegex.GetHashCode Xor _
               Me.IsRt.GetHashCode Xor _
               Me.Source.GetHashCode Xor _
               Me.IsExRt.GetHashCode Xor _
               Me.ExSource.GetHashCode
    End Function
End Class

'ソート比較クラス：ID比較のみ
Public NotInheritable Class IdComparerClass
    Implements IComparer(Of Long)

    ''' <summary>
    ''' 比較する方法
    ''' </summary>
    Public Enum ComparerMode
        Id
        Data
        Name
        Nickname
        Source
    End Enum

    Private _order As SortOrder
    Private _mode As ComparerMode
    Private _statuses As Dictionary(Of Long, PostClass)
    Private _CmpMethod As Comparison(Of Long)

    ''' <summary>
    ''' 昇順か降順か Setの際は同時に比較関数の切り替えを行う
    ''' </summary>
    Public Property Order() As SortOrder
        Get
            Return _order
        End Get
        Set(ByVal Value As SortOrder)
            _order = Value
            SetCmpMethod(_mode, _order)
        End Set
    End Property

    ''' <summary>
    ''' 並び替えの方法 Setの際は同時に比較関数の切り替えを行う
    ''' </summary>
    Public Property Mode() As ComparerMode
        Get
            Return _mode
        End Get
        Set(ByVal Value As ComparerMode)
            _mode = Value
            SetCmpMethod(_mode, _order)
        End Set
    End Property

    ''' <summary>
    ''' ListViewItemComparerクラスのコンストラクタ（引数付は未使用）
    ''' </summary>
    ''' <param name="col">並び替える列番号</param>
    ''' <param name="ord">昇順か降順か</param>
    ''' <param name="cmod">並び替えの方法</param>

    Public Sub New()
        _order = SortOrder.Ascending
        _mode = ComparerMode.Id
        SetCmpMethod(_mode, _order)
    End Sub

    Public WriteOnly Property posts() As Dictionary(Of Long, PostClass)
        Set(ByVal value As Dictionary(Of Long, PostClass))
            _statuses = value
        End Set
    End Property

    ' 指定したソートモードとソートオーダーに従い使用する比較関数のアドレスを返す
    Public Overloads ReadOnly Property CmpMethod(ByVal _sortmode As ComparerMode, ByVal _sortorder As SortOrder) As Comparison(Of Long)
        Get
            Dim _method As Comparison(Of Long) = Nothing
            If _sortorder = SortOrder.Ascending Then
                ' 昇順
                Select Case _sortmode
                    Case ComparerMode.Data
                        _method = AddressOf Compare_ModeData_Ascending
                    Case ComparerMode.Id
                        _method = AddressOf Compare_ModeId_Ascending
                    Case ComparerMode.Name
                        _method = AddressOf Compare_ModeName_Ascending
                    Case ComparerMode.Nickname
                        _method = AddressOf Compare_ModeNickName_Ascending
                    Case ComparerMode.Source
                        _method = AddressOf Compare_ModeSource_Ascending
                End Select
            Else
                ' 降順
                Select Case _sortmode
                    Case ComparerMode.Data
                        _method = AddressOf Compare_ModeData_Descending
                    Case ComparerMode.Id
                        _method = AddressOf Compare_ModeId_Descending
                    Case ComparerMode.Name
                        _method = AddressOf Compare_ModeName_Descending
                    Case ComparerMode.Nickname
                        _method = AddressOf Compare_ModeNickName_Descending
                    Case ComparerMode.Source
                        _method = AddressOf Compare_ModeSource_Descending
                End Select
            End If
            Return _method
        End Get
    End Property

    ' ソートモードとソートオーダーに従い使用する比較関数のアドレスを返す
    ' (overload 現在の使用中の比較関数のアドレスを返す)
    Public Overloads ReadOnly Property CmpMethod() As Comparison(Of Long)
        Get
            Return _CmpMethod
        End Get
    End Property

    ' ソートモードとソートオーダーに従い比較関数のアドレスを切り替え
    Private Sub SetCmpMethod(ByVal mode As ComparerMode, ByVal order As SortOrder)
        _CmpMethod = Me.CmpMethod(mode, order)
    End Sub

    'xがyより小さいときはマイナスの数、大きいときはプラスの数、
    '同じときは0を返す (こちらは未使用 一応比較関数群呼び出しの形のまま残しておく)
    Public Function Compare(ByVal x As Long, ByVal y As Long) _
            As Integer Implements IComparer(Of Long).Compare
        Return _CmpMethod(x, y)
    End Function

    ' 比較用関数群 いずれもステータスIDの順序を考慮する
    ' 本文比較　昇順
    Public Function Compare_ModeData_Ascending(ByVal x As Long, ByVal y As Long) As Integer
        Dim result As Integer = String.Compare(_statuses.Item(x).Data, _statuses.Item(y).Data)
        If result = 0 Then result = x.CompareTo(y)
        Return result
    End Function

    ' 本文比較　降順
    Public Function Compare_ModeData_Descending(ByVal x As Long, ByVal y As Long) As Integer
        Dim result As Integer = String.Compare(_statuses.Item(y).Data, _statuses.Item(x).Data)
        If result = 0 Then result = y.CompareTo(x)
        Return result
    End Function

    ' ステータスID比較　昇順
    Public Function Compare_ModeId_Ascending(ByVal x As Long, ByVal y As Long) As Integer
        Return x.CompareTo(y)
    End Function

    ' ステータスID比較　降順
    Public Function Compare_ModeId_Descending(ByVal x As Long, ByVal y As Long) As Integer
        Return y.CompareTo(x)
    End Function

    ' 表示名比較　昇順
    Public Function Compare_ModeName_Ascending(ByVal x As Long, ByVal y As Long) As Integer
        Dim result As Integer = String.Compare(_statuses.Item(x).Name, _statuses.Item(y).Name)
        If result = 0 Then result = x.CompareTo(y)
        Return result
    End Function

    ' 表示名比較　降順
    Public Function Compare_ModeName_Descending(ByVal x As Long, ByVal y As Long) As Integer
        Dim result As Integer = String.Compare(_statuses.Item(y).Name, _statuses.Item(x).Name)
        If result = 0 Then result = y.CompareTo(x)
        Return result
    End Function

    ' ユーザー名比較　昇順
    Public Function Compare_ModeNickName_Ascending(ByVal x As Long, ByVal y As Long) As Integer
        Dim result As Integer = String.Compare(_statuses.Item(x).Nickname, _statuses.Item(y).Nickname)
        If result = 0 Then result = x.CompareTo(y)
        Return result
    End Function

    ' ユーザー名比較　降順
    Public Function Compare_ModeNickName_Descending(ByVal x As Long, ByVal y As Long) As Integer
        Dim result As Integer = String.Compare(_statuses.Item(y).Nickname, _statuses.Item(x).Nickname)
        If result = 0 Then result = y.CompareTo(x)
        Return result
    End Function

    ' Source比較　昇順
    Public Function Compare_ModeSource_Ascending(ByVal x As Long, ByVal y As Long) As Integer
        Dim result As Integer = String.Compare(_statuses.Item(x).Source, _statuses.Item(y).Source)
        If result = 0 Then result = x.CompareTo(y)
        Return result
    End Function

    ' Source比較　降順
    Public Function Compare_ModeSource_Descending(ByVal x As Long, ByVal y As Long) As Integer
        Dim result As Integer = String.Compare(_statuses.Item(y).Source, _statuses.Item(x).Source)
        If result = 0 Then result = y.CompareTo(x)
        Return result
    End Function
End Class
