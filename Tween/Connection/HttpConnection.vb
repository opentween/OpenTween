Imports System.Net
Imports System.IO
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Text

'''<summary>
'''HttpWebRequest,HttpWebResponseを使用した基本的な通信機能を提供する
'''</summary>
'''<remarks>
'''プロキシ情報などを設定するため、使用前に静的メソッドInitializeConnectionを呼び出すこと。
'''通信方式によって必要になるHTTPヘッダの付加などは、派生クラスで行う。
'''</remarks>
Public Class HttpConnection
    '''<summary>
    '''プロキシ
    '''</summary>
    Private Shared proxy As WebProxy = Nothing

    '''<summary>
    '''ユーザーが選択したプロキシの方式
    '''</summary>
    Private Shared proxyKind As ProxyType = proxyType.IE

    '''<summary>
    '''クッキー保存用コンテナ
    '''</summary>
    Private Shared cookieContainer As New CookieContainer

    '''<summary>
    '''初期化済みフラグ
    '''</summary>
    Private Shared isInitialize As Boolean = False

    Public Enum ProxyType
        None
        IE
        Specified
    End Enum

    '''<summary>
    '''HttpWebRequestオブジェクトを取得する。パラメータはGET/HEAD/DELETEではクエリに、POST/PUTではエンティティボディに変換される。
    '''</summary>
    '''<remarks>
    '''追加で必要となるHTTPヘッダや通信オプションは呼び出し元で付加すること
    '''（Timeout,AutomaticDecompression,AllowAutoRedirect,UserAgent,ContentType,Accept,HttpRequestHeader.Authorization,カスタムヘッダ）
    '''POST/PUTでクエリが必要な場合は、requestUriに含めること。
    '''</remarks>
    '''<param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
    '''<param name="requestUri">通信先URI</param>
    '''<param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
    '''<param name="withCookie">通信にcookieを使用するか</param>
    '''<returns>引数で指定された内容を反映したHttpWebRequestオブジェクト</returns>
    Protected Function CreateRequest(ByVal method As String, _
                                            ByVal requestUri As Uri, _
                                            ByVal param As Dictionary(Of String, String), _
                                            ByVal withCookie As Boolean _
                                        ) As HttpWebRequest
        If Not isInitialize Then Throw New Exception("Sequence error.(not initialized)")

        'GETメソッドの場合はクエリとurlを結合
        Dim ub As New UriBuilder(requestUri.AbsoluteUri)
        If param IsNot Nothing AndAlso (method = "GET" OrElse method = "DELETE" OrElse method = "HEAD") Then
            ub.Query = CreateQueryString(param)
        End If

        Dim webReq As HttpWebRequest = DirectCast(WebRequest.Create(ub.Uri), HttpWebRequest)

        'プロキシ設定
        If proxyKind <> ProxyType.IE Then webReq.Proxy = proxy

        webReq.Method = method
        If method = "POST" OrElse method = "PUT" Then
            webReq.ContentType = "application/x-www-form-urlencoded"
            'POST/PUTメソッドの場合は、ボディデータとしてクエリ構成して書き込み
            Using writer As New StreamWriter(webReq.GetRequestStream)
                writer.Write(CreateQueryString(param))
            End Using
        End If
        'cookie設定
        If withCookie Then webReq.CookieContainer = cookieContainer
        'タイムアウト設定
        webReq.Timeout = DefaultTimeout

        Return webReq
    End Function

    '''<summary>
    '''HTTPの応答を処理し、引数で指定されたストリームに書き込み
    '''</summary>
    '''<remarks>
    '''リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
    '''WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
    '''gzipファイルのダウンロードを想定しているため、他形式の場合は伸張時に問題が発生する可能性があります。
    '''</remarks>
    '''<param name="webRequest">HTTP通信リクエストオブジェクト</param>
    '''<param name="contentStream">[OUT]HTTP応答のボディストリームのコピー先</param>
    '''<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
    '''<param name="withCookie">通信にcookieを使用する</param>
    '''<returns>HTTP応答のステータスコード</returns>
    Protected Function GetResponse(ByVal webRequest As HttpWebRequest, _
                                        ByVal contentStream As Stream, _
                                        ByVal headerInfo As Dictionary(Of String, String), _
                                        ByVal withCookie As Boolean _
                                    ) As HttpStatusCode
        Try
            Using webRes As HttpWebResponse = CType(webRequest.GetResponse(), HttpWebResponse)
                Dim statusCode As HttpStatusCode = webRes.StatusCode
                'cookie保持
                If withCookie Then SaveCookie(webRes.Cookies)
                'リダイレクト応答の場合は、リダイレクト先を設定
                GetHeaderInfo(webRes, headerInfo)
                '応答のストリームをコピーして戻す
                If webRes.ContentLength > 0 Then
                    'gzipなら応答ストリームの内容は伸張済み。それ以外なら伸張する。
                    If webRes.ContentEncoding = "gzip" OrElse webRes.ContentEncoding = "deflate" Then
                        Using stream As Stream = webRes.GetResponseStream()
                            If stream IsNot Nothing Then CopyStream(stream, contentStream)
                        End Using
                    Else
                        Using stream As Stream = New System.IO.Compression.GZipStream(webRes.GetResponseStream, Compression.CompressionMode.Decompress)
                            If stream IsNot Nothing Then CopyStream(stream, contentStream)
                        End Using
                    End If
                End If
                Return statusCode
            End Using
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    '''<summary>
    '''HTTPの応答を処理し、応答ボディデータをテキストとして返却する
    '''</summary>
    '''<remarks>
    '''リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
    '''WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
    '''テキストの文字コードはUTF-8を前提として、エンコードはしていません
    '''</remarks>
    '''<param name="webRequest">HTTP通信リクエストオブジェクト</param>
    '''<param name="contentText">[OUT]HTTP応答のボディデータ</param>
    '''<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
    '''<param name="withCookie">通信にcookieを使用する</param>
    '''<returns>HTTP応答のステータスコード</returns>
    Protected Function GetResponse(ByVal webRequest As HttpWebRequest, _
                                        ByRef contentText As String, _
                                        ByVal headerInfo As Dictionary(Of String, String), _
                                        ByVal withCookie As Boolean _
                                    ) As HttpStatusCode
        Try
            Using webRes As HttpWebResponse = CType(webRequest.GetResponse(), HttpWebResponse)
                Dim statusCode As HttpStatusCode = webRes.StatusCode
                'cookie保持
                If withCookie Then SaveCookie(webRes.Cookies)
                'リダイレクト応答の場合は、リダイレクト先を設定
                GetHeaderInfo(webRes, headerInfo)
                '応答のストリームをテキストに書き出し
                If contentText Is Nothing Then Throw New ArgumentNullException("contentText")
                Using sr As StreamReader = New StreamReader(webRes.GetResponseStream)
                    contentText = sr.ReadToEnd()
                End Using
                Return statusCode
            End Using
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Using sr As StreamReader = New StreamReader(res.GetResponseStream)
                    contentText = sr.ReadToEnd()
                End Using
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    '''<summary>
    '''HTTPの応答を処理します。応答ボディデータが不要な用途向け。
    '''</summary>
    '''<remarks>
    '''リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
    '''WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
    '''</remarks>
    '''<param name="webRequest">HTTP通信リクエストオブジェクト</param>
    '''<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
    '''<param name="withCookie">通信にcookieを使用する</param>
    '''<returns>HTTP応答のステータスコード</returns>
    Protected Function GetResponse(ByVal webRequest As HttpWebRequest, _
                                        ByVal headerInfo As Dictionary(Of String, String), _
                                        ByVal withCookie As Boolean _
                                    ) As HttpStatusCode
        Try
            Using webRes As HttpWebResponse = CType(webRequest.GetResponse(), HttpWebResponse)
                Dim statusCode As HttpStatusCode = webRes.StatusCode
                'cookie保持
                If withCookie Then SaveCookie(webRes.Cookies)
                'リダイレクト応答の場合は、リダイレクト先を設定
                GetHeaderInfo(webRes, headerInfo)
                Return statusCode
            End Using
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    '''<summary>
    '''HTTPの応答を処理し、応答ボディデータをBitmapとして返却します
    '''</summary>
    '''<remarks>
    '''リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
    '''WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
    '''</remarks>
    '''<param name="webRequest">HTTP通信リクエストオブジェクト</param>
    '''<param name="contentText">[OUT]HTTP応答のボディデータを書き込むBitmap</param>
    '''<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
    '''<param name="withCookie">通信にcookieを使用する</param>
    '''<returns>HTTP応答のステータスコード</returns>
    Protected Function GetResponse(ByVal webRequest As HttpWebRequest, _
                                        ByRef contentBitmap As Bitmap, _
                                        ByVal headerInfo As Dictionary(Of String, String), _
                                        ByVal withCookie As Boolean _
                                    ) As HttpStatusCode
        Try
            Using webRes As HttpWebResponse = CType(webRequest.GetResponse(), HttpWebResponse)
                Dim statusCode As HttpStatusCode = webRes.StatusCode
                'cookie保持
                If withCookie Then SaveCookie(webRes.Cookies)
                'リダイレクト応答の場合は、リダイレクト先を設定
                GetHeaderInfo(webRes, headerInfo)
                '応答のストリームをBitmapにして戻す
                'If webRes.ContentLength > 0 Then contentBitmap = New Bitmap(webRes.GetResponseStream)
                contentBitmap = New Bitmap(webRes.GetResponseStream)
                Return statusCode
            End Using
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    '''<summary>
    '''クッキーを保存。ホスト名なしのドメインの場合、ドメイン名から先頭のドットを除去して追加しないと再利用されないため
    '''</summary>
    Private Sub SaveCookie(ByVal cookieCollection As CookieCollection)
        For Each ck As Cookie In cookieCollection
            If ck.Domain.StartsWith(".") Then
                ck.Domain = ck.Domain.Substring(1, ck.Domain.Length - 1)
                cookieContainer.Add(ck)
            End If
        Next
    End Sub

    '''<summary>
    '''in/outのストリームインスタンスを受け取り、コピーして返却
    '''</summary>
    '''<param name="inStream">コピー元ストリームインスタンス。読み取り可であること</param>
    '''<param name="outStream">コピー先ストリームインスタンス。書き込み可であること</param>
    Private Sub CopyStream(ByVal inStream As Stream, ByVal outStream As Stream)
        If inStream Is Nothing Then Throw New ArgumentNullException("inStream")
        If outStream Is Nothing Then Throw New ArgumentNullException("outStream")
        If Not inStream.CanRead Then Throw New ArgumentException("Input stream can not read.")
        If Not outStream.CanWrite Then Throw New ArgumentException("Output stream can not write.")
        If inStream.CanSeek AndAlso inStream.Length = 0 Then Throw New ArgumentException("Input stream do not have data.")

        Do
            Dim buffer(1024) As Byte
            Dim i As Integer = buffer.Length
            i = inStream.Read(buffer, 0, i)
            If i = 0 Then Exit Do
            outStream.Write(buffer, 0, i)
        Loop
    End Sub

    '''<summary>
    '''headerInfoのキー情報で指定されたHTTPヘッダ情報を取得・格納する。redirect応答時はLocationヘッダの内容を追記する
    '''</summary>
    '''<param name="webResponse">HTTP応答</param>
    '''<param name="headerInfo">[IN/OUT]キーにヘッダ名を指定したデータ空のコレクション。取得した値をデータにセットして戻す</param>
    Private Sub GetHeaderInfo(ByVal webResponse As HttpWebResponse, _
                                    ByVal headerInfo As Dictionary(Of String, String))

        If headerInfo Is Nothing Then Exit Sub

        If headerInfo.Count > 0 Then
            Dim keys(headerInfo.Count - 1) As String
            headerInfo.Keys.CopyTo(keys, 0)
            For Each key As String In keys
                If Array.IndexOf(webResponse.Headers.AllKeys, key) > -1 Then
                    headerInfo.Item(key) = webResponse.Headers.Item(key)
                Else
                    headerInfo.Item(key) = ""
                End If
            Next
        End If

        Dim statusCode As HttpStatusCode = webResponse.StatusCode
        If statusCode = HttpStatusCode.MovedPermanently OrElse _
           statusCode = HttpStatusCode.Found OrElse _
           statusCode = HttpStatusCode.SeeOther OrElse _
           statusCode = HttpStatusCode.TemporaryRedirect Then
            If headerInfo.ContainsKey("Location") Then
                headerInfo.Item("Location") = webResponse.Headers.Item("Location")
            Else
                headerInfo.Add("Location", webResponse.Headers.Item("Location"))
            End If
        End If
    End Sub

    '''<summary>
    '''クエリコレクションをkey=value形式の文字列に構成して戻す
    '''</summary>
    '''<param name="param">クエリ、またはポストデータとなるkey-valueコレクション</param>
    Protected Function CreateQueryString(ByVal param As IDictionary(Of String, String)) As String
        If param Is Nothing OrElse param.Count = 0 Then Return String.Empty

        Dim query As New StringBuilder
        For Each key As String In param.Keys
            query.AppendFormat("{0}={1}&", UrlEncode(key), UrlEncode(param(key)))
        Next
        Return query.ToString(0, query.Length - 1)
    End Function

    '''<summary>
    '''クエリ形式（key1=value1&key2=value2&...）の文字列をkey-valueコレクションに詰め直し
    '''</summary>
    '''<param name="queryString">クエリ文字列</param>
    '''<returns>key-valueのコレクション</returns>
    Protected Function ParseQueryString(ByVal queryString As String) As NameValueCollection
        Dim query As New NameValueCollection
        Dim parts() As String = queryString.Split("&"c)
        For Each part As String In parts
            Dim index As Integer = part.IndexOf("="c)
            If index = -1 Then
                query.Add(Uri.UnescapeDataString(part), "")
            Else
                query.Add(Uri.UnescapeDataString(part.Substring(0, index)), Uri.UnescapeDataString(part.Substring(index + 1)))
            End If
        Next
        Return query
    End Function

    '''<summary>
    '''2バイト文字も考慮したUrlエンコード
    '''</summary>
    '''<param name="str">エンコードする文字列</param>
    '''<returns>エンコード結果文字列</returns>
    Protected Function UrlEncode(ByVal stringToEncode As String) As String
        Const UnreservedChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~"
        Dim sb As New StringBuilder
        Dim bytes As Byte() = Encoding.UTF8.GetBytes(stringToEncode)

        For Each b As Byte In bytes
            If UnreservedChars.IndexOf(Chr(b)) <> -1 Then
                sb.Append(Chr(b))
            Else
                sb.AppendFormat("%{0:X2}", b)
            End If
        Next
        Return sb.ToString()
    End Function

#Region "DefaultTimeout"
    '''<summary>
    '''通信タイムアウト時間（ms）
    '''</summary>
    Private Shared timeout As Integer = 20000

    '''<summary>
    '''通信タイムアウト時間（ms）。10～120秒の範囲で指定。範囲外は20秒とする
    '''</summary>
    Protected Shared Property DefaultTimeout() As Integer
        Get
            Return timeout
        End Get
        Set(ByVal value As Integer)
            Const TimeoutMinValue As Integer = 10000
            Const TimeoutMaxValue As Integer = 120000
            Const TimeoutDefaultValue As Integer = 20000
            If value < TimeoutMinValue OrElse value > TimeoutMaxValue Then
                ' 範囲外ならデフォルト値設定
                timeout = TimeoutDefaultValue
            Else
                timeout = value
            End If
        End Set
    End Property
#End Region

    '''<summary>
    '''通信クラスの初期化処理。タイムアウト値とプロキシを設定する
    '''</summary>
    '''<remarks>
    '''通信開始前に最低一度呼び出すこと
    '''</remarks>
    '''<param name="timeout">タイムアウト値（秒）</param>
    '''<param name="proxyType">なし・指定・IEデフォルト</param>
    '''<param name="proxyAddress">プロキシのホスト名orIPアドレス</param>
    '''<param name="proxyPort">プロキシのポート番号</param>
    '''<param name="proxyUser">プロキシ認証が必要な場合のユーザ名。不要なら空文字</param>
    '''<param name="proxyPassword">プロキシ認証が必要な場合のパスワード。不要なら空文字</param>
    Public Shared Sub InitializeConnection( _
            ByVal timeout As Integer, _
            ByVal proxyType As ProxyType, _
            ByVal proxyAddress As String, _
            ByVal proxyPort As Integer, _
            ByVal proxyUser As String, _
            ByVal proxyPassword As String)
        isInitialize = True
        ServicePointManager.Expect100Continue = False
        DefaultTimeout = timeout * 1000     's -> ms
        Select Case proxyType
            Case proxyType.None
                proxy = Nothing
            Case proxyType.Specified
                proxy = New WebProxy("http://" + proxyAddress + ":" + proxyPort.ToString)
                If Not String.IsNullOrEmpty(proxyUser) OrElse Not String.IsNullOrEmpty(proxyPassword) Then
                    proxy.Credentials = New NetworkCredential(proxyUser, proxyPassword)
                End If
            Case proxyType.IE
                'IE設定（システム設定）はデフォルト値なので処理しない
        End Select
        proxyKind = proxyType
    End Sub

End Class
