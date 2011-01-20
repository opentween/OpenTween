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

Imports System.Net
Imports System.Collections.Generic

Public Class HttpVarious
    Inherits HttpConnection

    Private Const PostMethod As String = "POST"
    Private Const GetMethod As String = "GET"
    Private Const HeadMethod As String = "HEAD"

    Public Function GetRedirectTo(ByVal url As String) As String
        Try
            Dim req As HttpWebRequest = CreateRequest(HeadMethod, New Uri(url), Nothing, False)
            req.Timeout = 5000
            req.AllowAutoRedirect = False
            Dim data As String = ""
            Dim head As New Dictionary(Of String, String)
            Dim ret As HttpStatusCode = GetResponse(req, data, head, False)
            If head.ContainsKey("Location") Then
                Return head("Location")
            Else
                Return url
            End If
        Catch ex As Exception
            Return url
        End Try
    End Function

    Public Overloads Function GetImage(ByVal url As Uri) As Image
        Return GetImage(url.ToString, "", 10000, Nothing)
    End Function

    Public Overloads Function GetImage(ByVal url As String) As Image
        Return GetImage(url, "", 10000, Nothing)
    End Function

    Public Overloads Function GetImage(ByVal url As String, ByVal timeout As Integer) As Image
        Return GetImage(url, "", timeout, Nothing)
    End Function

    Public Overloads Function GetImage(ByVal url As String, ByVal referer As String) As Image
        Return GetImage(url, referer, 10000, Nothing)
    End Function

    Public Overloads Function GetImage(ByVal url As String, ByVal referer As String, ByVal timeout As Integer, ByRef errmsg As String) As Image
        Return GetImageInternal(AddressOf CheckValidImage, url, referer, timeout, errmsg)
    End Function

    Public Function GetIconImage(ByVal url As String, ByVal timeout As Integer) As Image
        Return GetImageInternal(AddressOf CheckValidIconImage, url, "", timeout, Nothing)
    End Function

    Private Delegate Function CheckValidImageDelegate(ByVal img As Image, ByVal width As Integer, ByVal height As Integer) As Image

    Private Overloads Function GetImageInternal(ByVal CheckImage As CheckValidImageDelegate, ByVal url As String, ByVal referer As String, ByVal timeout As Integer, ByRef errmsg As String) As Image
        Try
            Dim req As HttpWebRequest = CreateRequest(GetMethod, New Uri(url), Nothing, False)
            If Not String.IsNullOrEmpty(referer) Then req.Referer = referer
            If timeout < 3000 OrElse timeout > 30000 Then
                req.Timeout = 10000
            Else
                req.Timeout = timeout
            End If
            Dim img As Bitmap = Nothing
            Dim ret As HttpStatusCode = GetResponse(req, img, Nothing, False)
            If errmsg IsNot Nothing Then
                If ret = HttpStatusCode.OK Then
                    errmsg = ""
                Else
                    errmsg = ret.ToString
                End If
            End If
            If img IsNot Nothing Then img.Tag = url
            If ret = HttpStatusCode.OK Then Return CheckImage(img, img.Width, img.Height)
            Return Nothing
        Catch ex As WebException
            If errmsg IsNot Nothing Then
                errmsg = ex.Message
            End If
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function PostData(ByVal Url As String, ByVal param As Dictionary(Of String, String)) As Boolean
        Try
            Dim req As HttpWebRequest = CreateRequest(PostMethod, New Uri(Url), param, False)
            Dim res As HttpStatusCode = Me.GetResponse(req, Nothing, False)
            If res = HttpStatusCode.OK Then Return True
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function PostData(ByVal Url As String, ByVal param As Dictionary(Of String, String), ByRef content As String) As Boolean
        Try
            Dim req As HttpWebRequest = CreateRequest(PostMethod, New Uri(Url), param, False)
            Dim res As HttpStatusCode = Me.GetResponse(req, content, Nothing, False)
            If res = HttpStatusCode.OK Then Return True
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Overloads Function GetData(ByVal Url As String, ByVal param As Dictionary(Of String, String), ByRef content As String) As Boolean
        Return GetData(Url, param, content, 100000, Nothing)
    End Function

    Public Overloads Function GetData(ByVal Url As String, ByVal param As Dictionary(Of String, String), ByRef content As String, ByVal timeout As Integer) As Boolean
        Return GetData(Url, param, content, 100000, Nothing)
    End Function

    Public Overloads Function GetData(ByVal Url As String, ByVal param As Dictionary(Of String, String), ByRef content As String, ByVal timeout As Integer, ByRef errmsg As String) As Boolean
        Try
            Dim req As HttpWebRequest = CreateRequest(GetMethod, New Uri(Url), param, False)
            If timeout < 3000 OrElse timeout > 30000 Then
                req.Timeout = 10000
            Else
                req.Timeout = timeout
            End If
            Dim res As HttpStatusCode = Me.GetResponse(req, content, Nothing, False)
            If res = HttpStatusCode.OK Then Return True
            If errmsg IsNot Nothing Then
                errmsg = res.ToString
            End If
            Return False
        Catch ex As Exception
            If errmsg IsNot Nothing Then
                errmsg = ex.Message
            End If
            Return False
        End Try
    End Function

    Public Function GetContent(ByVal method As String, ByVal Url As Uri, ByVal param As Dictionary(Of String, String), ByRef content As String, ByVal headerInfo As Dictionary(Of String, String), ByVal userAgent As String) As HttpStatusCode
        'Searchで使用。呼び出し元で例外キャッチしている。
        Dim req As HttpWebRequest = CreateRequest(method, Url, param, False)
        req.UserAgent = userAgent
        Return Me.GetResponse(req, content, headerInfo, False)
    End Function

    Public Function GetDataToFile(ByVal Url As String, ByVal savePath As String) As Boolean
        Try
            Dim req As HttpWebRequest = CreateRequest(GetMethod, New Uri(Url), Nothing, False)
            req.AutomaticDecompression = DecompressionMethods.Deflate Or DecompressionMethods.GZip
            Using strm As New System.IO.FileStream(savePath, IO.FileMode.Create, IO.FileAccess.Write)
                Try
                    Dim res As HttpStatusCode = Me.GetResponse(req, strm, Nothing, False)
                    strm.Close()
                    If res = HttpStatusCode.OK Then Return True
                    Return False
                Catch ex As Exception
                    strm.Close()
                    Return False
                End Try
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function CheckValidIconImage(ByVal img As Image, ByVal width As Integer, ByVal height As Integer) As Image
        Return CheckValidImage(img, 48, 48)
    End Function

    Public Overloads Function CheckValidImage(ByVal img As Image, ByVal width As Integer, ByVal height As Integer) As Image
        If img Is Nothing Then Return Nothing

        Dim bmp As New Bitmap(width, height)
        Dim tag As Object = img.Tag
        Using g As Graphics = Graphics.FromImage(bmp)
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            g.DrawImage(img, 0, 0, width, height)
        End Using
        img.Dispose()
        bmp.Tag = tag
        Return bmp
    End Function

End Class
