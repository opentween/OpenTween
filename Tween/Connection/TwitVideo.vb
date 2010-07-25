Imports System.IO
Imports System.Text
Imports System.Net

Public Class TwitVideo
    Inherits HttpConnection

    Private Const PostMethod As String = "POST"
    Private Const GetMethod As String = "GET"
    Private Const ConsumerKey As String = "c72b8327466ad675782538f7c869738d"
    Private multimediaExt() As String = {".avi", _
                                         ".wmv", _
                                         ".flv", _
                                         ".m4v", _
                                         ".mov", _
                                         ".mp4", _
                                         ".rm", _
                                         ".mpeg", _
                                         ".mpg", _
                                         ".3gp", _
                                         ".3g2"}
    Private pictureExt() As String = {".jpg", _
                                    ".jpeg", _
                                    ".gif", _
                                    ".png"}

    Public Function Upload(ByVal mediaFile As FileInfo, _
                           ByVal message As String, _
                           ByVal keyword As String, _
                           ByVal username As String, _
                           ByVal twitter_id As String, _
                           ByRef content As String) As HttpStatusCode
        'Message必須
        If String.IsNullOrEmpty(message) Then Throw New ArgumentException("'Message' is required.")
        'Check filetype and size
        If Array.IndexOf(multimediaExt, mediaFile.Extension.ToLower) > -1 Then
            If mediaFile.Length > 20971520 Then Throw New ArgumentException("File is too large.")
        ElseIf Array.IndexOf(pictureExt, mediaFile.Extension.ToLower) > -1 Then
            If mediaFile.Length > 10485760 Then Throw New ArgumentException("File is too large.")
        Else
            Throw New ArgumentException("Service don't support this filetype.")
        End If
        'Endpoint(URI+Token)
        Const URLBASE As String = "http://api.twitvideo.jp/oauth/upload/"
        Dim data As Byte() = Encoding.ASCII.GetBytes(ConsumerKey.Substring(0, 9) + username)
        Dim bHash As Byte() = (New System.Security.Cryptography.MD5CryptoServiceProvider()).ComputeHash(data)
        Dim url As String = URLBASE + BitConverter.ToString(bHash).ToLower.Replace("-", "")
        'Parameters
        Dim param As New Dictionary(Of String, String)
        param.Add("username", username)
        If Not String.IsNullOrEmpty(twitter_id) Then param.Add("twitter_id", twitter_id)
        If Not String.IsNullOrEmpty(keyword) Then param.Add("keyword", keyword)
        param.Add("type", "xml")
        param.Add("message", message)
        Dim binary As New List(Of KeyValuePair(Of String, FileInfo))
        binary.Add(New KeyValuePair(Of String, FileInfo)("media", mediaFile))
        Me.InstanceTimeout = 60000     'タイムアウト60秒

        Dim req As HttpWebRequest = CreateRequest(PostMethod, _
                                                  New Uri(url), _
                                                  param, _
                                                  binary, _
                                                  False)
        Return Me.GetResponse(req, content, Nothing, False)
    End Function

    Public Function CheckValidExtension(ByVal ext As String) As Boolean
        If Array.IndexOf(pictureExt, ext) > -1 Then
            Return True
        End If
        If Array.IndexOf(multimediaExt, ext) > -1 Then
            Return True
        End If
        Return False
    End Function

    Public Function GetFileType(ByVal ext As String) As UploadFileType
        If Array.IndexOf(pictureExt, ext) > -1 Then
            Return UploadFileType.Picture
        End If
        If Array.IndexOf(multimediaExt, ext) > -1 Then
            Return UploadFileType.MultiMedia
        End If
        Return UploadFileType.Invalid
    End Function

    Public Function GetFileOpenDialogFilter() As String
        Return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png|" + _
                "Movie Files(*.avi;*.wmv;*.flv;*.m4v;*.mov;*.mp4;*.rm;*.mpeg;*.mpg;*.3gp;*.3g2)|*.avi;*.wmv;*.flv;*.m4v;*.mov;*.mp4;*.rm;*.mpeg;*.mpg;*.3gp;*.3g2"
    End Function
End Class
