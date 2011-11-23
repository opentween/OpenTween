﻿' Tween - Client of Twitter
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

Imports System.IO
Imports System.Net
Imports System.Xml

Public Class imgly
    Inherits HttpConnectionOAuthEcho
    Implements IMultimediaShareService

    'OAuth関連
    '''<summary>
    '''OAuthのコンシューマー鍵
    '''</summary>
    Private Const ConsumerKey As String = "tLbG3uS0BIIE8jm1mKzKOfZ6EgUOmWVM"

    '''<summary>
    '''OAuthの署名作成用秘密コンシューマーデータ
    '''</summary>
    Private Const ConsumerSecretKey As String = "M0IMsbl2722iWa+CGPVcNeQmE+TFpJk8B/KW9UUTk3eLOl9Ij005r52JNxVukTzM"

    Private pictureExt() As String = {".jpg", _
                                    ".jpeg", _
                                    ".gif", _
                                    ".png"}

    Private Const MaxFileSize As Long = 4 * 1024 * 1024

    Private tw As Twitter

    Public Function Upload(ByRef filePath As String,
                           ByRef message As String,
                           ByVal reply_to As Long) As String Implements IMultimediaShareService.Upload
        If String.IsNullOrEmpty(filePath) Then Return "Err:File isn't specified."
        If String.IsNullOrEmpty(message) Then message = ""
        Dim mediaFile As FileInfo
        Try
            mediaFile = New FileInfo(filePath)
        Catch ex As NotSupportedException
            Return "Err:" + ex.Message
        End Try
        If mediaFile Is Nothing OrElse Not mediaFile.Exists Then Return "Err:File isn't exists."

        Dim content As String = ""
        Dim ret As HttpStatusCode
        'img.lyへの投稿
        Try
            ret = UploadFile(mediaFile, message, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Dim url As String = ""
        If ret = HttpStatusCode.OK Then
            Dim xd As XmlDocument = New XmlDocument()
            Try
                xd.LoadXml(content)
                'URLの取得
                url = xd.SelectSingleNode("/image/url").InnerText
            Catch ex As XmlException
                Return "Err:" + ex.Message
            Catch ex As Exception
                Return "Err:" + ex.Message
            End Try
        Else
            Return "Err:" + ret.ToString
        End If
        'アップロードまでは成功
        filePath = ""
        If String.IsNullOrEmpty(url) Then url = ""
        'Twitterへの投稿
        '投稿メッセージの再構成
        If String.IsNullOrEmpty(message) Then message = ""
        If message.Length + AppendSettingDialog.Instance.TwitterConfiguration.CharactersReservedPerMedia + 1 > 140 Then
            message = message.Substring(0, 140 - AppendSettingDialog.Instance.TwitterConfiguration.CharactersReservedPerMedia - 1) + " " + url
        Else
            message += " " + url
        End If
        Return tw.PostStatus(message, reply_to)
    End Function

    Private Function UploadFile(ByVal mediaFile As FileInfo, _
                       ByVal message As String, _
                       ByRef content As String) As HttpStatusCode
        'Message必須
        If String.IsNullOrEmpty(message) Then message = ""
        'Check filetype and size(Max 4MB)
        If Not Me.CheckValidExtension(mediaFile.Extension) Then Throw New ArgumentException("Service don't support this filetype.")
        If Not Me.CheckValidFilesize(mediaFile.Extension, mediaFile.Length) Then Throw New ArgumentException("File is too large.")

        Dim param As New Dictionary(Of String, String)
        param.Add("message", message)
        Dim binary As New List(Of KeyValuePair(Of String, FileInfo))
        binary.Add(New KeyValuePair(Of String, FileInfo)("media", mediaFile))
        Me.InstanceTimeout = 60000 'タイムアウト60秒

        Return GetContent(PostMethod, _
                          New Uri("http://img.ly/api/2/upload.xml"), _
                          param, _
                          binary, _
                          content, _
                          Nothing, _
                          Nothing)
    End Function

    Public Function CheckValidExtension(ByVal ext As String) As Boolean Implements IMultimediaShareService.CheckValidExtension
        If Array.IndexOf(pictureExt, ext.ToLower) > -1 Then
            Return True
        End If
        Return False
    End Function

    Public Function GetFileOpenDialogFilter() As String Implements IMultimediaShareService.GetFileOpenDialogFilter
        Return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png"
    End Function

    Public Function GetFileType(ByVal ext As String) As UploadFileType Implements IMultimediaShareService.GetFileType
        If Me.CheckValidExtension(ext) Then
            Return UploadFileType.Picture
        End If
        Return UploadFileType.Invalid
    End Function

    Public Function IsSupportedFileType(ByVal type As UploadFileType) As Boolean Implements IMultimediaShareService.IsSupportedFileType
        Return type.Equals(UploadFileType.Picture)
    End Function

    Public Function CheckValidFilesize(ByVal ext As String, ByVal fileSize As Long) As Boolean Implements IMultimediaShareService.CheckValidFilesize
        If Me.CheckValidExtension(ext) Then
            Return fileSize <= MaxFileSize
        End If
        Return False
    End Function

    Public Function Configuration(ByVal key As String, ByVal value As Object) As Boolean Implements IMultimediaShareService.Configuration
        Return True
    End Function

    Public Sub New(ByVal twitter As Twitter)
        MyBase.New(New Uri("http://api.twitter.com/"), _
                   New Uri("https://api.twitter.com/1/account/verify_credentials.json"))
        tw = twitter
        Initialize(DecryptString(ConsumerKey), DecryptString(ConsumerSecretKey), tw.AccessToken, tw.AccessTokenSecret, "", "")
    End Sub
End Class
