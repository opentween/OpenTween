' Tween - Client of Twitter
' Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
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

Imports System.IO
Imports System.Text
Imports System.Net
Imports System.Windows.Forms

Public Class yfrog
    Inherits HttpConnectionOAuthEcho

    'OAuth関連
    '''<summary>
    '''OAuthのコンシューマー鍵
    '''</summary>
    Private Const ConsumerKey As String = "tLbG3uS0BIIE8jm1mKzKOfZ6EgUOmWVM"

    '''<summary>
    '''OAuthの署名作成用秘密コンシューマーデータ
    '''</summary>
    Private Const ConsumerSecretKey As String = "M0IMsbl2722iWa+CGPVcNeQmE+TFpJk8B/KW9UUTk3eLOl9Ij005r52JNxVukTzM"

    Private Const PostMethod As String = "POST"
    Private Const GetMethod As String = "GET"
    Private Const ApiKey As String = "03HJKOWY93b7d7b7a5fa015890f8259cf939e144"
    Private pictureExt() As String = {".jpg", _
                                    ".jpeg", _
                                    ".gif", _
                                    ".png"}

    Private Const MaxFileSize As Long = 5 * 1024 * 1024


    Public Function Upload(ByVal mediaFile As FileInfo, _
                       ByVal message As String, _
                       ByRef content As String) As HttpStatusCode
        'Message必須
        If String.IsNullOrEmpty(message) Then message = ""
        'Check filetype and size(Max 5MB)
        If Array.IndexOf(pictureExt, mediaFile.Extension.ToLower) > -1 Then
            If mediaFile.Length > MaxFileSize Then Throw New ArgumentException("File is too large.")
        Else
            Throw New ArgumentException("Service don't support this filetype.")
        End If

        Dim param As New Dictionary(Of String, String)
        param.Add("key", ApiKey)
        param.Add("message", message)
        Dim binary As New List(Of KeyValuePair(Of String, FileInfo))
        binary.Add(New KeyValuePair(Of String, FileInfo)("media", mediaFile))
        Me.InstanceTimeout = 60000 'タイムアウト60秒

        Return GetContent(PostMethod, _
                          New Uri("http://yfrog.com/api/xauth_upload"), _
                          param, _
                          binary, _
                          content, _
                          Nothing, _
                          Nothing)
    End Function

    Public Function CheckValidExtension(ByVal ext As String) As Boolean
        If Array.IndexOf(pictureExt, ext.ToLower) > -1 Then
            Return True
        End If
        Return False
    End Function

    Public Function GetFileOpenDialogFilter() As String
        Return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png"
    End Function

    Public Function GetFileType(ByVal ext As String) As UploadFileType
        If Array.IndexOf(pictureExt, ext.ToLower) > -1 Then
            Return UploadFileType.Picture
        End If
        Return UploadFileType.Invalid
    End Function

    Public Function IsSupportedFileType(ByVal type As UploadFileType) As Boolean
        Return type.Equals(UploadFileType.Picture)
    End Function

    Public Function GetMaxFileSize(ByVal ext As String) As Long
        If Array.IndexOf(pictureExt, ext.ToLower) > -1 Then
            Return MaxFileSize
        End If
        Return -1
    End Function

    Public Sub New(ByVal accessToken As String, ByVal accessTokenSecret As String)
        MyBase.New(New Uri("http://api.twitter.com/"), _
                   New Uri("https://api.twitter.com/1/account/verify_credentials.xml"))
        Initialize(DecryptString(ConsumerKey), DecryptString(ConsumerSecretKey), accessToken, accessTokenSecret, "")
    End Sub
End Class
