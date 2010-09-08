Imports System.Net
Imports System.Collections.Generic

Public Class HttpVarious
    Inherits HttpConnection

    Private Const PostMethod As String = "POST"
    Private Const GetMethod As String = "GET"

    Public Function GetRedirectTo(ByVal url As String) As String
        Try
            Dim req As HttpWebRequest = CreateRequest(GetMethod, New Uri(url), Nothing, False)
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
            If ret = HttpStatusCode.OK Then Return CheckValidImage(img)
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

    Public Overloads Function CheckValidImage(ByVal img As Image) As Image
        Return CheckValidImage(img, 48, 48)
    End Function

    Public Overloads Function CheckValidImage(ByVal img As Image, ByVal width As Integer, ByVal height As Integer) As Image
        If img Is Nothing Then Return Nothing

        If img.RawFormat.Guid = Imaging.ImageFormat.Gif.Guid Then
            Dim fd As New System.Drawing.Imaging.FrameDimension(img.FrameDimensionsList(0))
            Dim fd_count As Integer = img.GetFrameCount(fd)
            If fd_count > 1 Then
                Try
                    For i As Integer = 0 To fd_count - 1
                        img.SelectActiveFrame(fd, i)
                    Next
                    Return img
                Catch ex As Exception
                    '不正な画像の場合は、bitmapに書き直し
                    Dim bmp As New Bitmap(width, height)
                    Dim tag As Object = img.Tag
                    Using g As Graphics = Graphics.FromImage(bmp)
                        g.InterpolationMode = Drawing2D.InterpolationMode.High
                        g.DrawImage(img, 0, 0, width, height)
                    End Using
                    img.Dispose()
                    bmp.Tag = tag
                    Return bmp
                End Try
            Else
                Return img
            End If
        Else
            Return img
        End If
    End Function

End Class
