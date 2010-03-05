Imports System.Text
Imports System.Net

'''<summary>
'''BASIC認証を使用するHTTP通信
'''</summary>
'''<remarks>
'''使用前にユーザー／パスワードを設定する。認証確認を伴う場合はAuthenticateを、認証不要な場合はInitializeを呼ぶこと。
'''</remarks>
Public Class HttpConnectionBasic
    Inherits HttpConnection
    Implements IHttpConnection

    '''<summary>
    '''認証用ユーザー名
    '''</summary>
    Private userName As String = ""
    '''<summary>
    '''認証用パスワード
    '''</summary>
    Private password As String = ""
    '''<summary>
    '''Authorizationヘッダに設定するエンコード済み文字列
    '''</summary>
    Private credential As String = ""

    '''<summary>
    '''BASIC認証で指定のURLとHTTP通信を行い、結果を返す
    '''</summary>
    '''<param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
    '''<param name="requestUri">通信先URI</param>
    '''<param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
    '''<param name="content">[OUT]HTTP応答のボディデータ</param>
    '''<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。必要なヘッダ名を事前に設定しておくこと</param>
    '''<returns>HTTP応答のステータスコード</returns>
    Public Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String)) As HttpStatusCode Implements IHttpConnection.GetContent
        'contentがインスタンスされているかチェック
        If content Is Nothing Then Throw New ArgumentNullException("content")

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    False)
        'BASIC認証用ヘッダを付加
        AppendApiInfo(webReq)

        Return GetResponse(webReq, content, headerInfo, False)
    End Function

    '''<summary>
    '''BASIC認証とREST APIで必要なヘッダを付加
    '''</summary>
    '''<param name="webRequest">付加対象となるHTTPリクエストオブジェクト</param>
    Private Sub AppendApiInfo(ByVal webRequest As HttpWebRequest)
        webRequest.ContentType = "application/x-www-form-urlencoded"
        webRequest.Accept = "text/html, */*"
        webRequest.Headers.Add(HttpRequestHeader.Authorization, credential)
    End Sub

    '''<summary>
    '''BASIC認証で使用するユーザー名とパスワードを設定。
    '''</summary>
    '''<param name="userName">認証で使用するユーザー名</param>
    '''<param name="password">認証で使用するパスワード</param>
    Public Sub Initialize(ByVal userName As String, ByVal password As String)
        Me.userName = userName
        Me.password = password
        Me.credential = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password))
    End Sub

    '''<summary>
    '''設定されているユーザー名
    '''</summary>
    Public ReadOnly Property AuthUsername() As String Implements IHttpConnection.AuthUsername
        Get
            Return userName
        End Get
    End Property

    '''<summary>
    '''BASIC認証で使用するユーザー名とパスワードを設定。指定のURLにGETリクエストを投げて、OK応答なら認証OKとみなして認証情報を保存する
    '''</summary>
    '''<param name="url">認証先のURL</param>
    '''<param name="userName">認証で使用するユーザー名</param>
    '''<param name="password">認証で使用するパスワード</param>
    Public Function Authenticate(ByVal url As String, ByVal username As String, ByVal password As String) As Boolean Implements IHttpConnection.Authenticate
        'urlは認証必要なGETメソッドとする
        Dim orgCre As String = Me.credential
        Me.credential = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password))
        Try
            If Me.GetContent("GET", New Uri(url), Nothing, Nothing, Nothing) = HttpStatusCode.OK Then
                Me.userName = username
                Me.password = password
                Return True
            End If
            Me.credential = orgCre
            Return False
        Catch ex As Exception
            Me.credential = orgCre
            Return False
        End Try
    End Function

End Class
