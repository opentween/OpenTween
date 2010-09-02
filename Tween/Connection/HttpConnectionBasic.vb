Imports System.Text
Imports System.Net
Imports System.IO

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
    Private _userName As String = ""
    '''<summary>
    '''認証用パスワード
    '''</summary>
    Private _password As String = ""
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
    '''<param name="callback">処理終了直前に呼ばれるコールバック関数のデリゲート 不要な場合はNothingを渡すこと</param>
    '''<returns>HTTP応答のステータスコード</returns>
    Public Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String), _
            ByVal callback As IHttpConnection.CallbackDelegate) As HttpStatusCode Implements IHttpConnection.GetContent

        '認証済かチェック
        If String.IsNullOrEmpty(Me.credential) Then Return HttpStatusCode.Unauthorized

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    False)
        'BASIC認証用ヘッダを付加
        AppendApiInfo(webReq)

        Dim code As HttpStatusCode
        If content Is Nothing Then
            code = GetResponse(webReq, headerInfo, False)
        Else
            code = GetResponse(webReq, content, headerInfo, False)
        End If
        If callback IsNot Nothing Then
            callback(Me, code, content)
        End If
        Return code
    End Function

    Public Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByVal binary As List(Of KeyValuePair(Of String, FileInfo)), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String), _
            ByVal callback As IHttpConnection.CallbackDelegate) As HttpStatusCode Implements IHttpConnection.GetContent

        '認証済かチェック
        If String.IsNullOrEmpty(Me.credential) Then Return HttpStatusCode.Unauthorized

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    binary, _
                                                    False)
        'BASIC認証用ヘッダを付加
        AppendApiInfo(webReq)

        Dim code As HttpStatusCode
        If content Is Nothing Then
            code = GetResponse(webReq, headerInfo, False)
        Else
            code = GetResponse(webReq, content, headerInfo, False)
        End If
        If callback IsNot Nothing Then
            callback(Me, code, content)
        End If
        Return code
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
        Me._userName = userName
        Me._password = password
        Me.credential = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password))
    End Sub

    '''<summary>
    '''設定されているユーザー名
    '''</summary>
    Public ReadOnly Property AuthUsername() As String Implements IHttpConnection.AuthUsername
        Get
            Return _userName
        End Get
    End Property

    '''<summary>
    '''パスワード
    '''</summary>
    Public ReadOnly Property Password() As String
        Get
            Return Me._password
        End Get
    End Property

    '''<summary>
    '''BASIC認証で使用するユーザー名とパスワードを設定。指定のURLにGETリクエストを投げて、OK応答なら認証OKとみなして認証情報を保存する
    '''</summary>
    '''<param name="url">認証先のURL</param>
    '''<param name="userName">認証で使用するユーザー名</param>
    '''<param name="password">認証で使用するパスワード</param>
    Public Function Authenticate(ByVal url As Uri, ByVal username As String, ByVal password As String) As HttpStatusCode Implements IHttpConnection.Authenticate
        'urlは認証必要なGETメソッドとする
        Dim orgCre As String = Me.credential
        Me.credential = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password))
        Try
            Dim httpCode As HttpStatusCode = Me.GetContent("GET", url, Nothing, Nothing, Nothing, Nothing)
            If httpCode = HttpStatusCode.OK Then
                Me._userName = username
                Me._password = password
            Else
                Me.credential = orgCre
            End If
            Return httpCode
        Catch ex As Exception
            Me.credential = orgCre
            Throw
        End Try
    End Function

End Class
