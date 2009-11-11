' Tween - Client of Twitter
' Copyright (c) 2007-2009 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
'           (c) 2008-2009 Moz (@syo68k) <http://iddy.jp/profile/moz/>
'           (c) 2008-2009 takeshik (@takeshik) <http://www.takeshik.org/>
' All rights reserved.
' 
' This file is part of Tween.
' 
' This program is free software; you can redistribute it and/or modify it
' under the terms of the GNU General Public License as published by the Free
' Software Foundation; either version 3 of the License, or (at your option)
' any later version.
' 
' This program is distributed in the hope that it will be useful, but
' WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
' or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
' for more details. 
' 
' You should have received a copy of the GNU General Public License along
' with this program. If not, see <http://www.gnu.org/licenses/>, or write to
' the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
' Boston, MA 02110-1301, USA.

Imports System.IO
Imports System.Net
Imports System.Text
Imports System.IO.Compression

Public NotInheritable Class MySocket
    Private _enc As Encoding
    Private _cre As String
    Private _proxy As System.Net.WebProxy
    Private _proxyType As ProxyType
    Private Shared cCon As New System.Net.CookieContainer()
    Private Shared ReadOnly cConLock As New Object
    Private _defaultTimeOut As Integer = HttpTimeOut.DefaultValue * 1000
    Private _remainCountApi As Integer

    Public Enum REQ_TYPE
        ReqGET
        ReqGETBinary
        ReqPOST
        ReqPOSTEncode
        'ReqPOSTEncodeProtoVer1
        'ReqPOSTEncodeProtoVer2
        'ReqPOSTEncodeProtoVer3
        ReqGETForwardTo
        ReqGETFile
        ReqGETFileUp
        ReqGETFileRes
        ReqGETFileDll
        'ReqGetNoCache
        ReqPOSTAPI
        ReqGetAPI
        ReqGetApp
    End Enum

    Public Sub New(ByVal EncodeType As String, _
            ByVal Username As String, _
            ByVal Password As String, _
            ByVal ProxyType As ProxyType, _
            ByVal ProxyAddress As String, _
            ByVal ProxyPort As Integer, _
            ByVal ProxyUser As String, _
            ByVal ProxyPassword As String, _
            ByVal TimeOut As Integer)
        _enc = Encoding.GetEncoding(EncodeType)
        ServicePointManager.Expect100Continue = False
        If Username <> "" Then
            _cre = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ":" + Password))
        End If
        Select Case ProxyType
            Case ProxyType.None
                _proxy = Nothing
            Case ProxyType.Specified
                _proxy = New WebProxy("http://" + ProxyAddress + ":" + ProxyPort.ToString)
                If Not String.IsNullOrEmpty(ProxyUser) OrElse Not String.IsNullOrEmpty(ProxyPassword) Then
                    _proxy.Credentials = New NetworkCredential(ProxyUser, ProxyPassword)
                End If
                'IE設定（システム設定）はデフォルト値なので処理しない
        End Select
        _proxyType = ProxyType
        DefaultTimeOut = TimeOut
        _remainCountApi = -1
    End Sub

    Public Function GetWebResponse(ByVal url As String, _
            ByRef resStatus As String, _
            Optional ByVal reqType As REQ_TYPE = REQ_TYPE.ReqGET, _
            Optional ByVal data As String = "", _
            Optional ByVal referer As String = "", _
            Optional ByVal timeOut As Integer = 20 * 1000, _
            Optional ByVal userAgent As String = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ja; rv:1.9) Gecko/2008051206 Firefox/3.0") As Object
        Dim webReq As HttpWebRequest
        Dim cpolicy As System.Net.Cache.HttpRequestCachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.NoCacheNoStore)

        Try
            webReq = _
                CType(WebRequest.Create(url), HttpWebRequest)

            If DefaultTimeOut = timeOut Then
                webReq.Timeout = DefaultTimeOut
            Else
                webReq.Timeout = timeOut
            End If

            If reqType <> REQ_TYPE.ReqPOSTAPI AndAlso reqType <> REQ_TYPE.ReqGetAPI Then
                webReq.CookieContainer = cCon
                webReq.AutomaticDecompression = DecompressionMethods.Deflate Or DecompressionMethods.GZip
            End If
            webReq.KeepAlive = True
            webReq.AllowAutoRedirect = False
            webReq.UserAgent = userAgent
            'If reqType = REQ_TYPE.ReqGetNoCache Then
            '    webReq.CachePolicy = cpolicy
            'End If
            If _proxyType <> ProxyType.IE Then
                webReq.Proxy = _proxy
            End If

            If referer <> "" Then webReq.Referer = referer
            'POST系
            If reqType = REQ_TYPE.ReqPOST OrElse _
               reqType = REQ_TYPE.ReqPOSTEncode OrElse _
               reqType = REQ_TYPE.ReqPOSTAPI Then
                'reqType = REQ_TYPE.ReqPOSTEncodeProtoVer1 OrElse _
                'reqType = REQ_TYPE.ReqPOSTEncodeProtoVer2 OrElse _
                'reqType = REQ_TYPE.ReqPOSTEncodeProtoVer3 OrElse _
                webReq.Method = "POST"

                If DefaultTimeOut = timeOut Then
                    webReq.Timeout = DefaultTimeOut
                Else
                    webReq.Timeout = timeOut
                End If

                Dim dataB As Byte() = Encoding.ASCII.GetBytes(data)
                webReq.ContentLength = dataB.Length
                Select Case reqType
                    Case REQ_TYPE.ReqPOST
                        webReq.ContentType = "application/x-www-form-urlencoded"
                    Case REQ_TYPE.ReqPOSTEncode
                        '                        webReq.ContentType = "application/x-www-form-urlencoded; charset=" + _enc.WebName
                        webReq.ContentType = "application/x-www-form-urlencoded"
                        webReq.Accept = "text/xml,application/xml,application/xhtml+xml,text/html,text/plain,image/png,*/*"
                        'Case REQ_TYPE.ReqPOSTEncodeProtoVer1
                        '    webReq.ContentType = "application/x-www-form-urlencoded; charset=" + _enc.WebName
                        '    webReq.Accept = "text/javascript, text/html, application/xml, text/xml, */*"
                        '    webReq.Headers.Add("x-prototype-version", "1.6.0.1")
                        '    webReq.Headers.Add("x-requested-with", "XMLHttpRequest")
                        '    webReq.Headers.Add("Accept-Language", "ja,en-us;q=0.7,en;q=0.3")
                        '    webReq.Headers.Add("Accept-Charset", "Shift_JIS,utf-8;q=0.7,*;q=0.7")
                        'Case REQ_TYPE.ReqPOSTEncodeProtoVer2
                        '    webReq.ContentType = "application/x-www-form-urlencoded; charset=" + _enc.WebName
                        '    webReq.Accept = "text/javascript, text/html, application/xml, text/xml, */*"
                        '    webReq.Headers.Add("x-prototype-version", "1.6.0.1")
                        '    webReq.Headers.Add("x-requested-with", "XMLHttpRequest")
                        '    webReq.Headers.Add("Accept-Language", "ja,en-us;q=0.7,en;q=0.3")
                        '    webReq.Headers.Add("Accept-Charset", "Shift_JIS,utf-8;q=0.7,*;q=0.7")
                        'Case REQ_TYPE.ReqPOSTEncodeProtoVer3
                        '    webReq.ContentType = "application/x-www-form-urlencoded; charset=" + _enc.WebName
                        '    webReq.Accept = "application/json, text/javascript, */*"
                        '    webReq.Headers.Add("x-prototype-version", "1.6.0.1")
                        '    webReq.Headers.Add("x-requested-with", "XMLHttpRequest")
                        '    webReq.Headers.Add("Accept-Language", "ja,en-us;q=0.7,en;q=0.3")
                        '    webReq.Headers.Add("Accept-Charset", "Shift_JIS,utf-8;q=0.7,*;q=0.7")
                    Case REQ_TYPE.ReqPOSTAPI
                        webReq.ContentType = "application/x-www-form-urlencoded"
                        webReq.Accept = "text/html, */*"
                        'webReq.Headers.Add("X-Twitter-Client", "Tween")
                        'webReq.Headers.Add("X-Twitter-Client-Version", _version)
                        'webReq.Headers.Add("X-Twitter-Client-URL", "http://www.asahi-net.or.jp/~ne5h-ykmz/tween.xml")
                        webReq.Headers.Add(HttpRequestHeader.Authorization, _cre)
                End Select
                Dim st As Stream = webReq.GetRequestStream()
                st.Write(dataB, 0, dataB.Length)
                st.Close()
                'ElseIf reqType = REQ_TYPE.ReqGET Or reqType = REQ_TYPE.ReqGetNoCache Then
            ElseIf reqType = REQ_TYPE.ReqGET Then
                webReq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
                webReq.Headers.Add("Accept-Language", "ja,en-us;q=0.7,en;q=0.3")
                webReq.Headers.Add("Accept-Charset", "Shift_JIS,utf-8;q=0.7,*;q=0.7")
            ElseIf reqType = REQ_TYPE.ReqGetApp Then
                webReq.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, */*"
                webReq.Headers.Add("Accept-Language", "ja,en-us;q=0.7,en;q=0.3")
                webReq.Headers.Add("Accept-Charset", "Shift_JIS,utf-8;q=0.7,*;q=0.7")
            ElseIf reqType = REQ_TYPE.ReqGetAPI Then
                webReq.ContentType = "application/x-www-form-urlencoded"
                webReq.Accept = "text/html, */*"
                'webReq.Headers.Add("X-Twitter-Client", "Tween")
                'webReq.Headers.Add("X-Twitter-Client-Version", _version)
                'webReq.Headers.Add("X-Twitter-Client-URL", "http://www.asahi-net.or.jp/~ne5h-ykmz/tween.xml")
                webReq.Headers.Add(HttpRequestHeader.Authorization, _cre)
            End If

            Using webRes As HttpWebResponse = CType(webReq.GetResponse(), HttpWebResponse)
                If reqType <> REQ_TYPE.ReqPOSTAPI AndAlso reqType <> REQ_TYPE.ReqGetAPI Then
                    SyncLock cConLock
                        For Each ck As Cookie In webRes.Cookies
                            If ck.Domain.StartsWith(".") Then
                                ck.Domain = ck.Domain.Substring(1, ck.Domain.Length - 1)
                                cCon.Add(ck)
                            End If
                        Next
                    End SyncLock
                End If

                If webRes.StatusCode = HttpStatusCode.Found Then
                    resStatus = webRes.StatusCode.ToString() + " " + webRes.Headers.Item(HttpResponseHeader.Location)
                Else
                    resStatus = webRes.StatusCode.ToString() + " " + webRes.ResponseUri.AbsoluteUri
                End If


                Using strm As Stream = webRes.GetResponseStream()
                    Select Case reqType
                        'Case REQ_TYPE.ReqGET, REQ_TYPE.ReqPOST, REQ_TYPE.ReqPOSTEncode, REQ_TYPE.ReqPOSTEncodeProtoVer1, REQ_TYPE.ReqPOSTEncodeProtoVer2, REQ_TYPE.ReqPOSTEncodeProtoVer3, REQ_TYPE.ReqGetNoCache, REQ_TYPE.ReqPOSTAPI, REQ_TYPE.ReqGetAPI, REQ_TYPE.ReqGetApp
                        Case REQ_TYPE.ReqGET, REQ_TYPE.ReqPOST, REQ_TYPE.ReqPOSTEncode, REQ_TYPE.ReqPOSTAPI, REQ_TYPE.ReqGetAPI, REQ_TYPE.ReqGetApp
                            Dim rtStr As String
                            Using sr As New StreamReader(strm, _enc)
                                rtStr = sr.ReadToEnd()
                            End Using
                            If reqType = REQ_TYPE.ReqGetAPI Then
#If DEBUG Then
                                Diagnostics.Debug.WriteLine(webRes.Headers.Item("X-RateLimit-Limit"))
                                Diagnostics.Debug.WriteLine(webRes.Headers.Item("X-RateLimit-Remaining"))
                                Diagnostics.Debug.WriteLine(webRes.Headers.Item("X-RateLimit-Reset"))
#End If
                                If webRes.Headers.Item("X-RateLimit-Remaining") IsNot Nothing Then
                                    If Not Integer.TryParse(webRes.Headers.Item("X-RateLimit-Remaining"), _remainCountApi) Then _remainCountApi = -1
                                End If
                            End If
                            Return rtStr
                        Case REQ_TYPE.ReqGETBinary
                            Return New System.Drawing.Bitmap(strm)
                            'Dim readData(1023) As Byte
                            'Dim readSize As Integer = 0
                            'Dim img As Image
                            'Dim mem As New MemoryStream
                            'While True
                            '    readSize = strm.Read(readData, 0, readData.Length)
                            '    If readSize = 0 Then
                            '        Exit While
                            '    End If
                            '    mem.Write(readData, 0, readSize)
                            'End While
                            'img = Image.FromStream(mem, True)
                            'Select Case img.RawFormat.Guid
                            '    Case Imaging.ImageFormat.Icon.Guid
                            '        img.Dispose()   '一旦破棄
                            '        mem.Seek(0, SeekOrigin.Begin)   '頭だし
                            '        Using icn As Icon = New Icon(mem)
                            '            If icn Is Nothing Then Return Nothing
                            '            img = icn.ToBitmap()
                            '        End Using
                            '        mem.Close()
                            '    Case Imaging.ImageFormat.Gif.Guid
                            '        Dim fd As New Imaging.FrameDimension(img.FrameDimensionsList(0))
                            '        Dim page As Integer = img.GetFrameCount(fd)
                            '        If page > 1 Then
                            '            Dim eflg As Boolean = False
                            '            '全フレームが読み込み可能か確認
                            '            For i As Integer = 0 To page - 1
                            '                Try
                            '                    img.SelectActiveFrame(fd, i)
                            '                Catch ex As Exception
                            '                    eflg = True
                            '                    Exit For
                            '                End Try
                            '            Next
                            '            If eflg Then
                            '                'エラーが起きたらbitmapに変換
                            '                Dim bmp As New Bitmap(48, 48)
                            '                Using g As Graphics = Graphics.FromImage(bmp)
                            '                    g.InterpolationMode = Drawing2D.InterpolationMode.High
                            '                    g.DrawImage(img, 0, 0, 48, 48)
                            '                End Using
                            '                mem.Close()
                            '                img.Dispose()
                            '                img = bmp
                            '            End If
                            '            'エラーが起きなければ、memorystreamは閉じない（animated gif）
                            '        Else
                            '            mem.Close()
                            '        End If
                            '    Case Else
                            '        mem.Close()
                            'End Select
                            'Return img
                        Case REQ_TYPE.ReqGETFile
                            StreamToFile(strm, Path.Combine(Application.StartupPath(), "TweenNew.exe"), webRes.ContentEncoding)
                        Case REQ_TYPE.ReqGETFileUp
                            StreamToFile(strm, Path.Combine(Application.StartupPath(), "TweenUp.exe"), webRes.ContentEncoding)
                        Case REQ_TYPE.ReqGETFileRes
                            If Directory.Exists(Path.Combine(Application.StartupPath(), "en")) = False Then
                                Directory.CreateDirectory(Path.Combine(Application.StartupPath(), "en"))
                            End If
                            StreamToFile(strm, Path.Combine(Application.StartupPath(), "en\Tween.resourcesNew.dll"), webRes.ContentEncoding)
                        Case REQ_TYPE.ReqGETFileDll
                            StreamToFile(strm, Path.Combine(Application.StartupPath(), "TweenNew.XmlSerializers.dll"), webRes.ContentEncoding)
                        Case REQ_TYPE.ReqGETForwardTo
                            Dim rtStr As String = ""
                            If webRes.StatusCode = HttpStatusCode.MovedPermanently OrElse _
                               webRes.StatusCode = HttpStatusCode.Found OrElse _
                               webRes.StatusCode = HttpStatusCode.SeeOther OrElse _
                               webRes.StatusCode = HttpStatusCode.TemporaryRedirect Then
                                rtStr = webRes.Headers.GetValues("Location")(0)
                                Return rtStr
                            End If
                    End Select
                End Using
            End Using
        Catch ex As System.Net.WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim eres As HttpWebResponse = CType(ex.Response, HttpWebResponse)
                resStatus = "Err: " + eres.StatusCode.ToString() + " " + eres.ResponseUri.AbsoluteUri
                If reqType = REQ_TYPE.ReqGETBinary Then
                    Return Nothing
                Else
                    Return ""
                End If
            Else
                resStatus = "Err: ProtocolError(" + ex.Message + ") " + url
                If reqType = REQ_TYPE.ReqGETBinary Then
                    Return Nothing
                Else
                    Return ""
                End If
            End If
        Catch ex As Exception
            resStatus = "Err: " + ex.Message + " " + url
            If reqType = REQ_TYPE.ReqGETBinary Then
                Return Nothing
            Else
                Return ""
            End If
        End Try

        Return ""
    End Function

    Private Shared Sub StreamToFile(ByVal InStream As Stream, ByVal Path As String, ByVal Encoding As String)
        Dim strm As Stream
        Const BUFFERSIZE As Integer = 512 * 1024
        If Encoding.Equals("gzip") OrElse Encoding.Equals("deflate") Then
            strm = InStream
        Else
            strm = New GZipStream(InStream, CompressionMode.Decompress)
        End If
        Using strm
            Using fs As New FileStream(Path, FileMode.Create, FileAccess.Write)
                Dim b As Integer
                Dim buffer(BUFFERSIZE) As Byte
                While True
                    b = strm.Read(buffer, 0, BUFFERSIZE)
                    If b = 0 Then Exit While
                    fs.Write(buffer, 0, b)
                End While
            End Using
        End Using
    End Sub

    Public Shared Sub ResetCookie()
        SyncLock cConLock
            cCon = New System.Net.CookieContainer()
        End SyncLock
    End Sub

    Public Property DefaultTimeOut() As Integer
        Get
            Return _defaultTimeOut
        End Get
        Set(ByVal value As Integer)
            If value < HttpTimeOut.MinValue OrElse value > HttpTimeOut.MaxValue Then
                ' 範囲外ならデフォルト値設定
                _defaultTimeOut = HttpTimeOut.DefaultValue * 1000
            Else
                _defaultTimeOut = value * 1000
            End If
        End Set
    End Property

    Public ReadOnly Property RemainCountApi() As Integer
        Get
            Return _remainCountApi
        End Get
    End Property
End Class
