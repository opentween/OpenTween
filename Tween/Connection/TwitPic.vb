Imports System.IO
Imports System.Text
Imports System.Net

Public Class TwitPic
    Inherits HttpConnectionOAuthEcho

    'OAuth関連
    '''<summary>
    '''OAuthのコンシューマー鍵
    '''</summary>
    Private Const ConsumerKey As String = "iOQHfiCUsyOyamW8JJ8jg"

    '''<summary>
    '''OAuthの署名作成用秘密コンシューマーデータ
    '''</summary>
    Private Const ConsumerSecretKey As String = "5PS2oa5f2VaKMPrlZa7DTbz0aFULKd3Ojxqgsm142Dw"

    Private Const PostMethod As String = "POST"
    Private Const GetMethod As String = "GET"
    ''' ToDo:APIKey
    Private Const ApiKey As String = "287b60562aea3cab9f58fa54015848e8"
    ''' ToDo:Supported file type
    Private pictureExt() As String = {".jpg", _
                                    ".jpeg", _
                                    ".gif", _
                                    ".png"}

    Public Function Upload(ByVal mediaFile As FileInfo, _
                       ByVal message As String, _
                       ByRef content As String) As HttpStatusCode
        'Message必須
        If String.IsNullOrEmpty(message) Then message = ""
        'Check filetype and size(Max 5MB)
        If Array.IndexOf(pictureExt, mediaFile.Extension.ToLower) > -1 Then
            If mediaFile.Length > 5242880 Then Throw New ArgumentException("File is too large.")
        Else
            Throw New ArgumentException("Service don't support this filetype.")
        End If

        Dim param As New Dictionary(Of String, String)
        param.Add("key", ApiKey)
        param.Add("message", message)
        Dim binary As New List(Of KeyValuePair(Of String, FileInfo))
        binary.Add(New KeyValuePair(Of String, FileInfo)("media", mediaFile))

        Return GetContent(PostMethod, _
                          New Uri("http://api.twitpic.com/2/upload.xml"), _
                          param, _
                          binary, _
                          content, _
                          Nothing)
    End Function

    Public Sub New(ByVal accessToken As String, ByVal accessTokenSecret As String)
        MyBase.New(New Uri("http://api.twitter.com/"), _
                   New Uri("https://api.twitter.com/1/account/verify_credentials.json"))
        Initialize(ConsumerKey, ConsumerSecretKey, accessToken, accessTokenSecret, "")
    End Sub
End Class
