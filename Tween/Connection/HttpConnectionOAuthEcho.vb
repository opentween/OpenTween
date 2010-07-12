Imports System.IO
Imports System.Text
Imports System.Net

Public Class HttpConnectionOAuthEcho
    Inherits HttpConnectionOAuth

    Private Const PostMethod As String = "POST"
    Private Const GetMethod As String = "GET"

    Private _realm As Uri
    Private _serviceProvider As Uri
    Private _token As String
    Private _tokenSecret As String

    Public WriteOnly Property Realm As Uri
        Set(ByVal value As Uri)
            _realm = value
        End Set
    End Property

    Public WriteOnly Property ServiceProvider As Uri
        Set(ByVal value As Uri)
            _serviceProvider = value
        End Set
    End Property

    Protected Overrides Sub AppendOAuthInfo(ByVal webRequest As HttpWebRequest, _
                                ByVal query As Dictionary(Of String, String), _
                                ByVal token As String, _
                                ByVal tokenSecret As String)
        'OAuth共通情報取得
        Dim parameter As Dictionary(Of String, String) = GetOAuthParameter(token)
        'OAuth共通情報にquery情報を追加
        If query IsNot Nothing Then
            For Each item As KeyValuePair(Of String, String) In query
                parameter.Add(item.Key, item.Value)
            Next
        End If
        '署名の作成・追加(GETメソッド固定。ServiceProvider呼び出し用の署名作成)
        parameter.Add("oauth_signature", CreateSignature(tokenSecret, GetMethod, _serviceProvider, parameter))
        'HTTPリクエストのヘッダに追加
        Dim sb As New StringBuilder("OAuth ")
        sb.AppendFormat("realm=""{0}://{1}{2}"",", _realm.Scheme, _realm.Host, _realm.AbsolutePath)
        For Each item As KeyValuePair(Of String, String) In parameter
            '各種情報のうち、oauth_で始まる情報のみ、ヘッダに追加する。各情報はカンマ区切り、データはダブルクォーテーションで括る
            If item.Key.StartsWith("oauth_") Then
                sb.AppendFormat("{0}=""{1}"",", item.Key, UrlEncode(item.Value))
            End If
        Next
        webRequest.Headers.Add("X-Verify-Credentials-Authorization", sb.ToString)
        webRequest.Headers.Add("X-Auth-Service-Provider", String.Format("{0}://{1}{2}", _serviceProvider.Scheme, _serviceProvider.Host, _serviceProvider.AbsolutePath))
    End Sub


    Public Sub New(ByVal realm As Uri, ByVal serviceProvider As Uri)
        _realm = realm
        _serviceProvider = serviceProvider
    End Sub
End Class
