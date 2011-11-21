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

Public Class UserInfo
    Public Sub New()

    End Sub

    Public Sub New(ByVal user As TwitterDataModel.User)
        Me.Id = user.Id
        Me.Name = user.Name.Trim()
        Me.ScreenName = user.ScreenName
        Me.Location = user.Location
        Me.Description = user.Description
        Try
            Me.ImageUrl = New Uri(user.ProfileImageUrl)
        Catch ex As Exception
            Me.ImageUrl = Nothing
        End Try
        Me.Url = user.Url
        Me.Protect = user.Protected
        Me.FriendsCount = user.FriendsCount
        Me.FollowersCount = user.FollowersCount
        Me.CreatedAt = DateTimeParse(user.CreatedAt)
        Me.StatusesCount = user.StatusesCount
        Me.Verified = user.Verified
        Me.isFollowing = Me.isFollowing
        If user.Status IsNot Nothing Then
            Me.RecentPost = user.Status.Text
            Me.PostCreatedAt = DateTimeParse(user.Status.CreatedAt)
            Me.PostSource = user.Status.Source
        End If
    End Sub

    Public Id As Int64 = 0
    Public Name As String = ""
    Public ScreenName As String = ""
    Public Location As String = ""
    Public Description As String = ""
    Public ImageUrl As Uri = Nothing
    Public Url As String = ""
    Public Protect As Boolean = False
    Public FriendsCount As Integer = 0
    Public FollowersCount As Integer = 0
    Public FavoriteCount As Integer = 0
    Public CreatedAt As New DateTime
    Public StatusesCount As Integer = 0
    Public Verified As Boolean = False
    Public RecentPost As String = ""
    Public PostCreatedAt As New DateTime
    Public PostSource As String = ""        ' html形式　"<a href="http://sourceforge.jp/projects/tween/wiki/FrontPage" rel="nofollow">Tween</a>"
    Public isFollowing As Boolean = False
    Public isFollowed As Boolean = False

    Public Overrides Function ToString() As String
        Return Me.ScreenName + " / " + Me.Name
    End Function
End Class