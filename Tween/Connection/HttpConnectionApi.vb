Imports System.Text
Imports System.Net

Public Class HttpConnectionApi
    Inherits HttpConnection

    Private Shared userName As String = ""
    Private Shared password As String = ""
    Private Shared credential As String = ""

    Protected Function GetContent(ByVal method As RequestMethod, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String), _
            ByVal authRequired As Boolean) As HttpStatusCode
        'contentがインスタンスされているかチェック
        If content Is Nothing Then Throw New ArgumentNullException("content")
        '認証済かチェック
        If authRequired AndAlso String.IsNullOrEmpty(userName) Then Throw New Exception("Sequence error. (userName is blank.)")

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    False)
        'API用ヘッダを付加
        AppendApiInfo(webReq, authRequired)

        Return GetResponse(webReq, content, headerInfo, False)
    End Function

    Private Sub AppendApiInfo(ByVal webRequest As HttpWebRequest, ByVal authRequired As Boolean)
        webRequest.ContentType = "application/x-www-form-urlencoded"
        webRequest.Accept = "text/html, */*"
        If authRequired Then webRequest.Headers.Add(HttpRequestHeader.Authorization, credential)
    End Sub

    Protected Shared Sub Initialize(ByVal userName As String, ByVal password As String)
        HttpConnectionApi.userName = userName
        HttpConnectionApi.password = password
        credential = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password))
    End Sub
End Class
