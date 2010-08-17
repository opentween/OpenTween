Imports System.Text.RegularExpressions
Imports System.Web

Public Class ShortUrl
    Private Shared _ShortUrlService() As String = { _
        "http://tinyurl.com/", _
        "http://is.gd/", _
        "http://snipurl.com/", _
        "http://snurl.com/", _
        "http://nsfw.in/", _
        "http://qurlyq.com/", _
        "http://dwarfurl.com/", _
        "http://icanhaz.com/", _
        "http://tiny.cc/", _
        "http://urlenco.de/", _
        "http://bit.ly/", _
        "http://piurl.com/", _
        "http://linkbee.com/", _
        "http://traceurl.com/", _
        "http://twurl.nl/", _
        "http://cli.gs/", _
        "http://rubyurl.com/", _
        "http://budurl.com/", _
        "http://ff.im/", _
        "http://twitthis.com/", _
        "http://blip.fm/", _
        "http://tumblr.com/", _
        "http://www.qurl.com/", _
        "http://digg.com/", _
        "http://u.nu/", _
        "http://ustre.am/", _
        "http://pic.gd/", _
        "http://airme.us/", _
        "http://qurl.com/", _
        "http://bctiny.com/", _
        "http://j.mp/", _
        "http://goo.gl/", _
        "http://ow.ly/", _
        "http://bkite.com/", _
        "http://youtu.be/", _
        "http://dlvr.it/", _
        "http://p.tl/", _
        "http://ht.ly/", _
        "http://tl.gd/", _
        "http://htn.to/", _
        "http://amzn.to/", _
        "http://flic.kr/", _
        "http://moi.st/" _
    }

    Private Shared _bitlyId As String = ""
    Private Shared _bitlyKey As String = ""
    Private Shared _isresolve As Boolean = True

    Public Shared WriteOnly Property BitlyId() As String
        Set(ByVal value As String)
            _bitlyId = value
        End Set
    End Property

    Public Shared WriteOnly Property BitlyKey() As String
        Set(ByVal value As String)
            _bitlyKey = value
        End Set
    End Property

    Public Shared Property IsResolve As Boolean
        Get
            Return _isresolve
        End Get
        Set(ByVal value As Boolean)
            _isresolve = value
        End Set
    End Property

    Public Shared Function Resolve(ByVal orgData As String) As String
        If _isresolve Then
            Static urlCache As New Dictionary(Of String, String)
            If urlCache.Count > 500 Then urlCache.Clear() '定期的にリセット

            Dim m As MatchCollection = Regex.Matches(orgData, "<a href=""(?<svc>http://.+?/)(?<path>[^""]+)""", RegexOptions.IgnoreCase)
            Dim urlList As New List(Of String)
            For Each orgUrlMatch As Match In m
                Dim orgUrl As String = orgUrlMatch.Result("${svc}")
                Dim orgUrlPath As String = orgUrlMatch.Result("${path}")
                If Array.IndexOf(_ShortUrlService, orgUrl) > -1 AndAlso _
                   Not urlList.Contains(orgUrl + orgUrlPath) Then
                    urlList.Add(orgUrl + orgUrlPath)
                End If
            Next
            For Each orgUrl As String In urlList
                If urlCache.ContainsKey(orgUrl) Then
                    Try
                        orgData = orgData.Replace("<a href=""" + orgUrl + """", "<a href=""" + urlCache(orgUrl) + """")
                    Catch ex As Exception
                        'Through
                    End Try
                Else
                    Try
                        'urlとして生成できない場合があるらしい
                        'Dim urlstr As String = New Uri(urlEncodeMultibyteChar(orgUrl)).GetLeftPart(UriPartial.Path)
                        Dim retUrlStr As String = ""
                        Dim tmpurlStr As String = New Uri(urlEncodeMultibyteChar(orgUrl)).GetLeftPart(UriPartial.Path)
                        Dim httpVar As New HttpVarious
                        retUrlStr = urlEncodeMultibyteChar(httpVar.GetRedirectTo(tmpurlStr))
                        If retUrlStr.StartsWith("http") Then
                            retUrlStr = retUrlStr.Replace("""", "%22")  'ダブルコーテーションがあるとURL終端と判断されるため、これだけ再エンコード
                            orgData = orgData.Replace("<a href=""" + orgUrl + """", "<a href=""" + retUrlStr + """")
                            urlCache.Add(orgUrl, retUrlStr)
                        End If
                    Catch ex As Exception
                        'Through
                    End Try
                End If
            Next
        End If
        Return orgData
    End Function

    Public Shared Function Make(ByVal ConverterType As UrlConverter, ByVal SrcUrl As String) As String
        Dim src As String = urlEncodeMultibyteChar(SrcUrl)
        Dim param As New Dictionary(Of String, String)
        Dim content As String = ""

        For Each svc As String In _ShortUrlService
            If SrcUrl.StartsWith(svc) Then
                Return "Can't convert"
            End If
        Next

        'nico.msは短縮しない
        If SrcUrl.StartsWith("http://nico.ms/") Then Return "Can't convert"

        SrcUrl = HttpUtility.UrlEncode(SrcUrl)

        Select Case ConverterType
            Case UrlConverter.TinyUrl       'tinyurl
                If SrcUrl.StartsWith("http") Then
                    If "http://tinyurl.com/xxxxxx".Length > src.Length AndAlso Not src.Contains("?") AndAlso Not src.Contains("#") Then
                        ' 明らかに長くなると推測できる場合は圧縮しない
                        content = src
                        Exit Select
                    End If
                    If Not (New HttpVarious).PostData("http://tinyurl.com/api-create.php?url=" + SrcUrl, Nothing, content) Then
                        Return "Can't convert"
                    End If
                End If
                If Not content.StartsWith("http://tinyurl.com/") Then
                    Return "Can't convert"
                End If
            Case UrlConverter.Isgd
                If SrcUrl.StartsWith("http") Then
                    If "http://is.gd/xxxx".Length > src.Length AndAlso Not src.Contains("?") AndAlso Not src.Contains("#") Then
                        ' 明らかに長くなると推測できる場合は圧縮しない
                        content = src
                        Exit Select
                    End If
                    If Not (New HttpVarious).PostData("http://is.gd/api.php?longurl=" + SrcUrl, Nothing, content) Then
                        Return "Can't convert"
                    End If
                End If
                If Not content.StartsWith("http://is.gd/") Then
                    Return "Can't convert"
                End If
            Case UrlConverter.Twurl
                If SrcUrl.StartsWith("http") Then
                    If "http://twurl.nl/xxxxxx".Length > src.Length AndAlso Not src.Contains("?") AndAlso Not src.Contains("#") Then
                        ' 明らかに長くなると推測できる場合は圧縮しない
                        content = src
                        Exit Select
                    End If
                    param.Add("link[url]", SrcUrl)
                    If Not (New HttpVarious).PostData("http://tweetburner.com/links", param, content) Then
                        Return "Can't convert"
                    End If
                End If
                If Not content.StartsWith("http://twurl.nl/") Then
                    Return "Can't convert"
                End If
            Case UrlConverter.Unu
                If SrcUrl.StartsWith("http") Then
                    If "http://u.nu/xxxx".Length > src.Length AndAlso Not src.Contains("?") AndAlso Not src.Contains("#") Then
                        ' 明らかに長くなると推測できる場合は圧縮しない
                        content = src
                        Exit Select
                    End If
                    If Not (New HttpVarious).PostData("http://u.nu/unu-api-simple?url=" + SrcUrl, Nothing, content) Then
                        Return "Can't convert"
                    End If
                End If
                If Not content.StartsWith("http://u.nu") Then
                    Return "Can't convert"
                End If
            Case UrlConverter.Bitly, UrlConverter.Jmp
                Dim BitlyLogin As String = "tweenapi"
                Dim BitlyApiKey As String = "R_c5ee0e30bdfff88723c4457cc331886b"
                If _bitlyId <> "" AndAlso BitlyApiKey <> "" Then
                    BitlyLogin = _bitlyId
                    BitlyApiKey = _bitlyKey
                End If
                Const BitlyApiVersion As String = "2.0.1"
                If SrcUrl.StartsWith("http") Then
                    If "http://bit.ly/xxxx".Length > src.Length AndAlso Not src.Contains("?") AndAlso Not src.Contains("#") Then
                        ' 明らかに長くなると推測できる場合は圧縮しない
                        content = src
                        Exit Select
                    End If
                    Dim req As String = ""
                    If ConverterType = UrlConverter.Bitly Then
                        req = "http://api.bit.ly/shorten?version="
                    Else
                        req = "http://api.j.mp/shorten?version="
                    End If
                    req += BitlyApiVersion + _
                        "&login=" + BitlyLogin + _
                        "&apiKey=" + BitlyApiKey + _
                        "&longUrl=" + SrcUrl
                    If BitlyLogin <> "tweenapi" Then req += "&history=1"
                    If Not (New HttpVarious).PostData(req, Nothing, content) Then
                        Return "Can't convert"
                    Else
                        'Dim rx As Regex = New Regex("""shortUrl"": ""(?<ShortUrl>.*?)""")
                        If Regex.Match(content, """shortUrl"": ""(?<ShortUrl>.*?)""").Success Then
                            content = Regex.Match(content, """shortUrl"": ""(?<ShortUrl>.*?)""").Groups("ShortUrl").Value
                        End If
                    End If
                End If
        End Select
        '変換結果から改行を除去
        Dim ch As Char() = {ControlChars.Cr, ControlChars.Lf}
        content = content.TrimEnd(ch)
        If src.Length < content.Length Then content = src ' 圧縮の結果逆に長くなった場合は圧縮前のURLを返す
        Return content
    End Function
End Class
