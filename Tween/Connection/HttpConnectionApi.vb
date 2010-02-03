Imports System.Text
Imports System.Net

Public Class HttpConnectionApi
    Inherits HttpConnection

    Private Shared userName As String = ""
    Private Shared password As String = ""
    Private Shared credential As String = ""

    Protected Function AuthorizeAccount() As Boolean
        Dim authUri As Uri = New Uri("http://twitter.com/account/verify_credentials.xml")
        Dim content As String = ""
        Dim statusCode As HttpStatusCode = GetContent(RequestMethod.ReqGet, authUri, Nothing, content, Nothing)
        If statusCode = HttpStatusCode.OK Then
            Return True
        Else
            Return False
        End If
    End Function

    Protected Function GetContent(ByVal method As RequestMethod, _
            ByVal requestUri As Uri, _
            ByVal param As SortedList(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String)) As HttpStatusCode
        'contentがインスタンスされているかチェック
        If content Is Nothing Then Throw New ArgumentNullException("content")
        '認証済かチェック
        If String.IsNullOrEmpty(userName) Then Throw New Exception("Sequence error. (userName is blank.)")

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    False)
        'API用ヘッダを付加
        AppendApiInfo(webReq)

        Return GetResponse(webReq, content, headerInfo, False)
    End Function

    Private Shared Sub AppendApiInfo(ByVal webRequest As HttpWebRequest)
        webRequest.ContentType = "application/x-www-form-urlencoded"
        webRequest.Accept = "text/html, */*"
        webRequest.Headers.Add(HttpRequestHeader.Authorization, credential)
    End Sub

    Protected Shared Sub Initialize(ByVal userName As String, ByVal password As String)
        HttpConnectionApi.userName = userName
        HttpConnectionApi.password = password
        credential = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password))
    End Sub
End Class
