Imports System.Text.RegularExpressions
Imports System.ComponentModel
Imports System.Text
Imports System.Xml

Public Class Thumbnail

    Private lckPrev As New Object
    Private _prev As PreviewData
    Private Class PreviewData
        Implements IDisposable

        Public statusId As Long
        Public urls As List(Of KeyValuePair(Of String, String))
        Public pics As New List(Of KeyValuePair(Of String, Image))
        Public tooltiptext As New List(Of KeyValuePair(Of String, String))
        Public Sub New(ByVal id As Long, ByVal urlList As List(Of KeyValuePair(Of String, String)))
            statusId = id
            urls = urlList
        End Sub

        Public IsError As Boolean
        Public AdditionalErrorMessage As String

        Private disposedValue As Boolean = False        ' 重複する呼び出しを検出するには

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: 明示的に呼び出されたときにマネージ リソースを解放します
                End If

                ' TODO: 共有のアンマネージ リソースを解放します
                For Each pic As KeyValuePair(Of String, Image) In pics
                    If pic.Value IsNot Nothing Then pic.Value.Dispose()
                Next
            End If
            Me.disposedValue = True
        End Sub

#Region " IDisposable Support "
        ' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
    Private Owner As TweenMain

    Public Sub New(ByVal Owner As TweenMain)
        Me.Owner = Owner

        AddHandler Owner.PreviewScrollBar.Scroll, AddressOf PreviewScrollBar_Scroll
        AddHandler Owner.PreviewPicture.MouseLeave, AddressOf PreviewPicture_MouseLeave
        AddHandler Owner.PreviewPicture.DoubleClick, AddressOf PreviewPicture_DoubleClick
    End Sub

    Private ReadOnly Property _curPost As PostClass
        Get
            Return Owner.CurPost()
        End Get
    End Property

    Private Function IsDirectLink(ByVal url As String) As Boolean
        Return Regex.Match(url, "^http://.*(\.jpg|\.jpeg|\.gif|\.png|\.bmp)$", RegexOptions.IgnoreCase).Success
    End Function

    Public Sub thumbnail(ByVal id As Long, ByVal links As List(Of String))
        If Not Owner.IsPreviewEnable Then
            Owner.SplitContainer3.Panel2Collapsed = True
            Exit Sub
        End If
        If Not Owner.PreviewPicture.Image Is Nothing Then
            Owner.PreviewPicture.Image.Dispose()
            Owner.PreviewPicture.Image = Nothing
            Owner.SplitContainer3.Panel2Collapsed = True
        End If
        'SyncLock lckPrev
        '    If _prev IsNot Nothing Then
        '        _prev.Dispose()
        '        _prev = Nothing
        '    End If
        'End SyncLock

        If links.Count = 0 Then
            Owner.PreviewScrollBar.Maximum = 0
            Owner.PreviewScrollBar.Enabled = False
            Owner.SplitContainer3.Panel2Collapsed = True
            Exit Sub
        End If

        Dim imglist As New List(Of KeyValuePair(Of String, String))

        For Each url As String In links
            'Dim re As Regex
            Dim mc As Match
            'imgur
            mc = Regex.Match(url, "^http://imgur\.com/(\w+)\.jpg$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://i.imgur.com/${1}l.jpg")))
                Continue For
            End If
            '画像拡張子で終わるURL（直リンク）
            If IsDirectLink(url) Then
                imglist.Add(New KeyValuePair(Of String, String)(url, url))
                Continue For
            End If
            'twitpic
            mc = Regex.Match(url, "^http://(www\.)?twitpic\.com/(?<photoId>\w+)(/full/?)?$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://twitpic.com/show/thumb/${photoId}")))
                Continue For
            End If
            'yfrog
            mc = Regex.Match(url, "^http://yfrog\.com/(\w+)$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, url + ".th.jpg"))
                Continue For
            End If
            'tweetphoto
            Const comp As String = "http://TweetPhotoAPI.com/api/TPAPI.svc/imagefromurl?size=thumbnail&url="
            If Regex.IsMatch(url, "^(http://tweetphoto\.com/[0-9]+|http://pic\.gd/[a-z0-9]+)$", RegexOptions.IgnoreCase) Then
                imglist.Add(New KeyValuePair(Of String, String)(url, comp + url))
                Continue For
            End If
            'Mobypicture
            mc = Regex.Match(url, "^http://moby\.to/(\w+)$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://mobypicture.com/?${1}:small")))
                Continue For
            End If
            '携帯百景
            mc = Regex.Match(url, "^http://movapic\.com/pic/(\w+)$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://image.movapic.com/pic/s_${1}.jpeg")))
                Continue For
            End If
            'はてなフォトライフ
            mc = Regex.Match(url, "^http://f\.hatena\.ne\.jp/(([a-z])[a-z0-9_-]{1,30}[a-z0-9])/((\d{8})\d+)$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://img.f.hatena.ne.jp/images/fotolife/${2}/${1}/${4}/${3}_120.jpg")))
                Continue For
            End If
            'PhotoShare
            mc = Regex.Match(url, "^http://(?:www\.)?bcphotoshare\.com/photos/\d+/(\d+)$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://images.bcphotoshare.com/storages/${1}/thumb180.jpg")))
                Continue For
            End If
            'PhotoShare の短縮 URL
            mc = Regex.Match(url, "^http://bctiny\.com/p(\w+)$", RegexOptions.IgnoreCase)
            If mc.Success Then
                Try
                    imglist.Add(New KeyValuePair(Of String, String)(url, "http://images.bcphotoshare.com/storages/" + RadixConvert.ToInt32(mc.Result("${1}"), 32).ToString + "/thumb180.jpg"))
                    Continue For
                Catch ex As ArgumentOutOfRangeException
                End Try
            End If
            'img.ly
            mc = Regex.Match(url, "^http://img\.ly/(\w+)$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://img.ly/show/thumb/${1}")))
                Continue For
            End If
            'brightkite
            mc = Regex.Match(url, "^http://brightkite\.com/objects/((\w{2})(\w{2})\w+)$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://cdn.brightkite.com/${2}/${3}/${1}-feed.jpg")))
                Continue For
            End If
            'Twitgoo
            mc = Regex.Match(url, "^http://twitgoo\.com/(\w+)$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://twitgoo.com/${1}/mini")))
                Continue For
            End If
            'pic.im
            mc = Regex.Match(url, "^http://pic\.im/(\w+)$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://pic.im/website/thumbnail/${1}")))
                Continue For
            End If
            'youtube
            mc = Regex.Match(url, "^http://www\.youtube\.com/watch\?v=([\w\-]+)", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://i.ytimg.com/vi/${1}/default.jpg")))
                Continue For
            End If
            mc = Regex.Match(url, "^http://youtu\.be/([\w\-]+)", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://i.ytimg.com/vi/${1}/default.jpg")))
                Continue For
            End If
            'ニコニコ
            mc = Regex.Match(url, "^http://(?:(www|ext)\.nicovideo\.jp/watch|nico\.ms)/(?:sm|nm)?([0-9]+)(\?.+)?$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Value))
                Continue For
            End If
            'pixiv
            '参考: http://tail.s68.xrea.com/blog/2009/02/pixivflash.html Pixivの画像をFlashとかで取得する方法など:しっぽのブログ
            'ユーザー向けの画像ページ http://www.pixiv.net/member_illust.php?mode=medium&illust_id=[ID番号]
            '非ログインユーザー向けの画像ページ http://www.pixiv.net/index.php?mode=medium&illust_id=[ID番号]
            'サムネイルURL http://img[サーバー番号].pixiv.net/img/[ユーザー名]/[サムネイルID]_s.[拡張子]
            'サムネイルURLは画像ページから抽出する
            mc = Regex.Match(url, "^http://www\.pixiv\.net/(member_illust|index)\.php\?mode=(medium|big)&(amp;)?illust_id=(?<illustId>[0-9]+)(&(amp;)?tag=(?<tag>.+)?)*$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url.Replace("amp;", ""), mc.Value))
                Continue For
            End If
            'flickr
            '参考: http://tanarky.blogspot.com/2010/03/flickr-urlunavailable.html アグレッシブエンジニア: flickr の画像URL仕様についてまとめ(Unavailable画像)
            '画像URL仕様　http://farm{farm}.static.flickr.com/{server}/{id}_{secret}_{size}.{extension} 
            'photostreamなど複数の画像がある場合先頭の一つのみ認識と言うことにする
            '(二つ目のキャプチャ 一つ目の画像はユーザーアイコン）
            mc = Regex.Match(url, "^http://www.flickr.com/", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Value))
                Continue For
            End If
            'TwitVideo
            mc = Regex.Match(url, "^http://twitvideo\.jp/(\w+)$", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Result("http://twitvideo.jp/img/thumb/${1}")))
                Continue For
            End If
            'piapro
            mc = Regex.Match(url, "^http://piapro\.jp/content/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Value))
                Continue For
            End If
            'フォト蔵
            mc = Regex.Match(url, "^http://photozou\.jp/photo/show/(?<userId>[0-9]+)/(?<photoId>[0-9]+)", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Value))
                Continue For
            End If
            'tumblr
            mc = Regex.Match(url, "^http://.+\.tumblr\.com/.+/?", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Value))
                Continue For
            End If
            'ついっぷるフォト
            mc = Regex.Match(url, "^http://p\.twipple\.jp/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Value))
                Continue For
            End If
            'mypix/shamoji
            mc = Regex.Match(url, "^http://(www\.mypix\.jp|www\.shamoji\.info)/app\.php/picture/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase)
            If mc.Success Then
                imglist.Add(New KeyValuePair(Of String, String)(url, mc.Value + "/thumb.jpg"))
                Continue For
            End If
        Next

        If imglist.Count = 0 Then
            Owner.PreviewScrollBar.Maximum = 0
            Owner.PreviewScrollBar.Enabled = False
            Owner.SplitContainer3.Panel2Collapsed = True
            Exit Sub
        End If

        ThumbnailProgressChanged(0)
        Dim bgw As BackgroundWorker
        bgw = New BackgroundWorker()
        AddHandler bgw.DoWork, AddressOf bgw_DoWork
        AddHandler bgw.RunWorkerCompleted, AddressOf bgw_Completed
        bgw.RunWorkerAsync(New PreviewData(id, imglist))

    End Sub

    Private Sub ThumbnailProgressChanged(ByVal ProgressPercentage As Integer, Optional ByVal AddMsg As String = "")
        If ProgressPercentage = 0 Then    '開始
            Owner.SetStatusLabel("Thumbnail generating...")
        ElseIf ProgressPercentage = 100 Then '正常終了
            Owner.SetStatusLabel("Thumbnail generated.")
        Else ' エラー
            If String.IsNullOrEmpty(AddMsg) Then
                Owner.SetStatusLabel("can't get Thumbnail.")
            Else
                Owner.SetStatusLabel("can't get Thumbnail." + AddMsg)
            End If
        End If
    End Sub

    Private Sub bgw_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
        Dim arg As PreviewData = DirectCast(e.Argument, PreviewData)
        Dim worker As BackgroundWorker = DirectCast(sender, BackgroundWorker)
        Dim addMsg As String = ""
        arg.AdditionalErrorMessage = ""

        ' pixiv,Flickr,piapro,フォト蔵,tumblrの解析もこちらでやる
        For Each url As KeyValuePair(Of String, String) In arg.urls
            Dim http As New HttpVarious

            If IsDirectLink(url.Key) Then
                ' 画像直リンク
                Dim img As Image = http.GetImage(url.Value, url.Key)
                If img Is Nothing Then Continue For
                arg.pics.Add(New KeyValuePair(Of String, Image)(url.Key, img))
                arg.tooltiptext.Add(New KeyValuePair(Of String, String)(url.Key, ""))
            ElseIf url.Key.StartsWith("http://www.pixiv.net/") Then
                Dim src As String = ""
                'illustIDをキャプチャ
                Dim mc As Match = Regex.Match(url.Key, "^http://www\.pixiv\.net/(member_illust|index)\.php\?mode=(medium|big)&(amp;)?illust_id=(?<illustId>[0-9]+)(&(amp;)?tag=(?<tag>.+)?)*$", RegexOptions.IgnoreCase)
                If mc.Groups("tag").Value = "R-18" OrElse mc.Groups("tag").Value = "R-18G" Then
                    arg.IsError = True
                    arg.AdditionalErrorMessage = "(NeedLogin.NotSupported)"
                Else
                    If (New HttpVarious).GetData(Regex.Replace(mc.Groups(0).Value, "amp;", ""), Nothing, src, 5000) Then
                        Dim _mc As Match = Regex.Match(src, mc.Result("http://img([0-9]+)\.pixiv\.net/img/.+/${illustId}_s\.([a-zA-Z]+)"))
                        If _mc.Success Then
                            Dim _img As Image = http.GetImage(_mc.Value, url.Key)
                            If _img Is Nothing Then Continue For
                            arg.pics.Add(New KeyValuePair(Of String, Image)(url.Key, _img))
                            arg.tooltiptext.Add(New KeyValuePair(Of String, String)(url.Key, ""))
                        End If
                    End If
                End If
            ElseIf url.Key.StartsWith("http://www.flickr.com/") Then
                Dim mc As Match = Regex.Match(url.Key, "^http://www.flickr.com/", RegexOptions.IgnoreCase)
                If mc.Success Then
                    Dim src As String = ""
                    If (New HttpVarious).GetData(url.Key, Nothing, src, 5000) Then
                        Dim _mc As MatchCollection = Regex.Matches(src, mc.Result("http://farm[0-9]+\.static\.flickr\.com/[0-9]+/.+?\.([a-zA-Z]+)"))
                        '二つ以上キャプチャした場合先頭の一つだけ 一つだけの場合はユーザーアイコンしか取れなかった
                        If _mc.Count > 1 Then
                            Dim _img As Image = http.GetImage(_mc.Item(1).Value, url.Key)
                            If _img Is Nothing Then Continue For
                            arg.pics.Add(New KeyValuePair(Of String, Image)(url.Key, _img))
                            arg.tooltiptext.Add(New KeyValuePair(Of String, String)(url.Key, ""))
                        End If
                    End If
                    Continue For
                End If
            ElseIf url.Key.StartsWith("http://piapro.jp/") Then
                Dim mc As Match = Regex.Match(url.Key, "^http://piapro\.jp/content/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase)
                If mc.Success Then
                    Dim src As String = ""
                    If (New HttpVarious).GetData(url.Key, Nothing, src, 5000) Then
                        Dim _mc As Match = Regex.Match(src, mc.Result("http://c1\.piapro\.jp/timg/${contentId}_\d{14}_\d{4}_\d{4}\.(jpg|png|gif)"))
                        If _mc.Success Then
                            '各画像には120x120のサムネイルがある（多分）ので、URLを置き換える。元々ページに埋め込まれている画像は500x500
                            Dim r As New System.Text.RegularExpressions.Regex("_\d{4}_\d{4}")
                            Dim min_img_url As String = r.Replace(_mc.Value, "_0120_0120")
                            Dim _img As Image = http.GetImage(min_img_url, url.Key)
                            If _img Is Nothing Then Continue For
                            arg.pics.Add(New KeyValuePair(Of String, Image)(url.Key, _img))
                            arg.tooltiptext.Add(New KeyValuePair(Of String, String)(url.Key, ""))
                        End If
                    End If
                End If
                Continue For
            ElseIf url.Key.StartsWith("http://photozou.jp/") Then
                Dim mc As Match = Regex.Match(url.Key, "^http://photozou\.jp/photo/show/(?<userId>[0-9]+)/(?<photoId>[0-9]+)", RegexOptions.IgnoreCase)
                If mc.Success Then
                    Dim src As String = ""
                    Dim show_info As String = mc.Result("http://api.photozou.jp/rest/photo_info?photo_id=${photoId}")
                    If (New HttpVarious).GetData(show_info, Nothing, src, 5000) Then
                        Dim xdoc As New XmlDocument
                        Dim thumbnail_url As String = ""
                        Try
                            xdoc.LoadXml(src)
                            thumbnail_url = xdoc.SelectSingleNode("/rsp/info/photo/thumbnail_image_url").InnerText
                        Catch ex As Exception
                            thumbnail_url = ""
                        End Try
                        If String.IsNullOrEmpty(thumbnail_url) Then Continue For
                        Dim _img As Image = http.GetImage(thumbnail_url, url.Key)
                        If _img Is Nothing Then Continue For
                        arg.pics.Add(New KeyValuePair(Of String, Image)(url.Key, _img))
                        arg.tooltiptext.Add(New KeyValuePair(Of String, String)(url.Key, ""))
                        Continue For
                    End If
                End If
                Continue For
            ElseIf Regex.Match(url.Key, "^http://.+\.tumblr\.com/.+/?", RegexOptions.IgnoreCase).Success Then
                Dim TargetUrl As String = (New HttpVarious).GetRedirectTo(url.Key)
                Dim mc As Match = Regex.Match(TargetUrl, "(?<base>http://.+?\.tumblr\.com/)post/(?<postID>[0-9]+)(/(?<subject>.+?)/)?", RegexOptions.IgnoreCase)
                Dim apiurl As String = mc.Groups("base").Value + "api/read?id=" + mc.Groups("postID").Value
                Dim src As String = ""
                Dim imgurl As String = Nothing
                If (New HttpVarious).GetData(apiurl, Nothing, src, 5000) Then
                    Dim xdoc As New XmlDocument
                    Try
                        xdoc.LoadXml(src)

                        Dim type As String = xdoc.SelectSingleNode("/tumblr/posts/post").Attributes("type").Value
                        If type = "photo" Then
                            imgurl = xdoc.SelectSingleNode("/tumblr/posts/post/photo-url").InnerText
                        Else
                            arg.AdditionalErrorMessage = "(PostType:" + type + ")"
                            arg.IsError = True
                            imgurl = ""
                        End If

                    Catch ex As Exception
                        imgurl = ""
                    End Try

                    If Not String.IsNullOrEmpty(imgurl) Then
                        Dim _img As Image = http.GetImage(imgurl, url.Key)
                        If _img Is Nothing Then Continue For
                        arg.pics.Add(New KeyValuePair(Of String, Image)(url.Key, _img))
                        arg.tooltiptext.Add(New KeyValuePair(Of String, String)(url.Key, ""))
                    End If
                End If
                Continue For
            ElseIf Regex.Match(url.Value, "^http://(?:(www|ext)\.nicovideo\.jp/watch|nico\.ms)/(?:sm|nm)?([0-9]+)(\?.+)?$", RegexOptions.IgnoreCase).Success Then
                Dim mc As Match = Regex.Match(url.Value, "^http://(?:(www|ext)\.nicovideo\.jp/watch|nico\.ms)/(?<id>(?:sm|nm)?([0-9]+))(\?.+)?$", RegexOptions.IgnoreCase)
                Dim apiurl As String = "http://www.nicovideo.jp/api/getthumbinfo/" + mc.Groups("id").Value
                Dim src As String = ""
                Dim imgurl As String = ""
                Dim headers As New Dictionary(Of String, String)
                headers.Add("User-Agent", "Tween/" + fileVersion)
                If (New HttpVarious).GetData(apiurl, headers, src, 5000) Then
                    Dim sb As New StringBuilder
                    Dim xdoc As New XmlDocument
                    Try
                        xdoc.LoadXml(src)
                        Dim status As String = xdoc.SelectSingleNode("/nicovideo_thumb_response").Attributes("status").Value
                        If status = "ok" Then
                            imgurl = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/thumbnail_url").InnerText

                            'ツールチップに動画情報をセットする
                            Dim tmp As String

                            Try
                                tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/title").InnerText
                                If Not String.IsNullOrEmpty(tmp) Then
                                    sb.Append(My.Resources.NiconicoInfoText1)
                                    sb.Append(tmp)
                                    sb.AppendLine()
                                End If
                            Catch ex As Exception

                            End Try

                            Try
                                tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/length").InnerText
                                If Not String.IsNullOrEmpty(tmp) Then
                                    sb.Append(My.Resources.NiconicoInfoText2)
                                    sb.Append(tmp)
                                    sb.AppendLine()
                                End If
                            Catch ex As Exception

                            End Try

                            Try
                                Dim tm As New DateTime
                                tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/first_retrieve").InnerText
                                If DateTime.TryParse(tmp, tm) Then
                                    sb.Append(My.Resources.NiconicoInfoText3)
                                    sb.Append(tm.ToString)
                                    sb.AppendLine()
                                End If
                            Catch ex As Exception

                            End Try

                            Try
                                tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/view_counter").InnerText
                                If Not String.IsNullOrEmpty(tmp) Then
                                    sb.Append(My.Resources.NiconicoInfoText4)
                                    sb.Append(tmp)
                                    sb.AppendLine()
                                End If
                            Catch ex As Exception

                            End Try

                            Try
                                tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/comment_num").InnerText
                                If Not String.IsNullOrEmpty(tmp) Then
                                    sb.Append(My.Resources.NiconicoInfoText5)
                                    sb.Append(tmp)
                                    sb.AppendLine()
                                End If
                            Catch ex As Exception

                            End Try
                            Try
                                tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/mylist_counter").InnerText
                                If Not String.IsNullOrEmpty(tmp) Then
                                    sb.Append(My.Resources.NiconicoInfoText6)
                                    sb.Append(tmp)
                                    sb.AppendLine()
                                End If
                            Catch ex As Exception

                            End Try

                        ElseIf status = "fail" Then
                            Dim errcode As String = xdoc.SelectSingleNode("/nicovideo_thumb_response/error/code").InnerText
                            arg.AdditionalErrorMessage = "(" + errcode + ")"
                            arg.IsError = True
                            imgurl = ""
                        Else
                            arg.AdditionalErrorMessage = "(UnknownResponse)"
                            arg.IsError = True
                            imgurl = ""
                        End If

                    Catch ex As Exception
                        imgurl = ""
                        arg.AdditionalErrorMessage = "(Invalid XML)"
                        arg.IsError = True
                    End Try

                    If Not String.IsNullOrEmpty(imgurl) Then
                        Dim _img As Image = http.GetImage(imgurl, url.Key)
                        If _img Is Nothing Then Continue For
                        arg.pics.Add(New KeyValuePair(Of String, Image)(url.Key, _img))
                        arg.tooltiptext.Add(New KeyValuePair(Of String, String)(url.Key, sb.ToString.Trim()))
                    End If
                End If
            ElseIf url.Key.StartsWith("http://p.twipple.jp/") Then
                Dim mc As Match = Regex.Match(url.Key, "^http://p.twipple.jp/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase)
                If mc.Success Then
                    Dim src As String = ""
                    If (New HttpVarious).GetData(url.Key, Nothing, src, 5000) Then
                        Dim thumbnail_url As String = ""
                        Dim ContentId As String = mc.Groups("contentId").Value
                        Dim DataDir As New StringBuilder

                        ' DataDir作成
                        DataDir.Append("data")
                        For i As Integer = 0 To ContentId.Length - 1
                            DataDir.Append("/")
                            DataDir.Append(ContentId.Chars(i))
                        Next

                        ' サムネイルURL抽出
                        thumbnail_url = Regex.Match(src, "http://p\.twipple\.jp/" + DataDir.ToString() + "_s\.([a-zA-Z]+)").Value

                        If String.IsNullOrEmpty(thumbnail_url) Then Continue For
                        Dim _img As Image = http.GetImage(thumbnail_url, url.Key)
                        If _img Is Nothing Then Continue For
                        arg.pics.Add(New KeyValuePair(Of String, Image)(url.Key, _img))
                        arg.tooltiptext.Add(New KeyValuePair(Of String, String)(url.Key, ""))
                        Continue For
                    End If
                End If
                Continue For
            ElseIf url.Key.StartsWith("http://www.youtube.com/", StringComparison.CurrentCultureIgnoreCase) _
                        OrElse url.Key.StartsWith("http://youtu.be/", StringComparison.InvariantCultureIgnoreCase) Then
                ' YouTube
                ' 参考
                ' http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_video_entries.html
                ' デベロッパー ガイド: Data API プロトコル - 単独の動画情報の取得 - YouTube の API とツール - Google Code
                ' http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_understanding_video_feeds.html#Understanding_Feeds_and_Entries 
                ' デベロッパー ガイド: Data API プロトコル - 動画のフィードとエントリについて - YouTube の API とツール - Google Code
                Dim videourl As String = (New HttpVarious).GetRedirectTo(url.Key)
                Dim mc As Match = Regex.Match(videourl, "^http://(?:(www\.youtube\.com)|(youtu\.be))/(watch\?v=)?(?<videoid>([\w\-]+))", RegexOptions.IgnoreCase)
                If videourl.StartsWith("http://www.youtube.com/index?ytsession=") Then
                    videourl = url.Key
                    mc = Regex.Match(videourl, "^http://(?:(www\.youtube\.com)|(youtu\.be))/(watch\?v=)?(?<videoid>([\w\-]+))", RegexOptions.IgnoreCase)
                End If
                If mc.Success Then
                    Dim apiurl As String = "http://gdata.youtube.com/feeds/api/videos/" + mc.Groups("videoid").Value
                    Dim imgurl As String = url.Value
                    Dim src As String = ""
                    If (New HttpVarious).GetData(apiurl, Nothing, src, 5000) Then
                        Dim sb As New StringBuilder
                        Dim xdoc As New XmlDocument
                        Try
                            xdoc.LoadXml(src)
                            Dim nsmgr As New XmlNamespaceManager(xdoc.NameTable)
                            nsmgr.AddNamespace("root", "http://www.w3.org/2005/Atom")
                            nsmgr.AddNamespace("app", "http://purl.org/atom/app#")
                            nsmgr.AddNamespace("media", "http://search.yahoo.com/mrss/")

                            Dim xentryNode As XmlNode = xdoc.DocumentElement.SelectSingleNode("/root:entry/media:group", nsmgr)
                            Dim xentry As XmlElement = CType(xentryNode, XmlElement)
                            Dim tmp As String = ""
                            Try
                                tmp = xentry.Item("media:title").InnerText
                                If Not String.IsNullOrEmpty(tmp) Then
                                    sb.Append("タイトル:")
                                    sb.Append(tmp)
                                    sb.AppendLine()
                                End If
                            Catch ex As Exception

                            End Try

                            Try
                                Dim sec As Integer = 0

                                If Integer.TryParse(xentry.Item("yt:duration").Attributes("seconds").Value, sec) Then
                                    sb.Append("再生時間:")
                                    sb.AppendFormat("{0:d}:{1:d2}", sec \ 60, sec Mod 60)
                                    sb.AppendLine()
                                End If
                            Catch ex As Exception

                            End Try

                            Try
                                Dim tmpdate As New DateTime

                                xentry = CType(xdoc.DocumentElement.SelectSingleNode("/root:entry", nsmgr), XmlElement)
                                If DateTime.TryParse(xentry.Item("published").InnerText, tmpdate) Then
                                    sb.Append("投稿日時:")
                                    sb.Append(tmpdate)
                                    sb.AppendLine()
                                End If
                            Catch ex As Exception

                            End Try

                            Try
                                Dim count As Integer = 0
                                xentry = CType(xdoc.DocumentElement.SelectSingleNode("/root:entry", nsmgr), XmlElement)
                                tmp = xentry.Item("yt:statistics").Attributes("viewCount").Value

                                If Integer.TryParse(tmp, count) Then
                                    sb.Append("再生数:")
                                    sb.Append(tmp)
                                    sb.AppendLine()
                                End If
                            Catch ex As Exception

                            End Try

                            Try
                                xentry = CType(xdoc.DocumentElement.SelectSingleNode("/root:entry/app:control", nsmgr), XmlElement)
                                If xentry IsNot Nothing Then
                                    sb.Append(xentry.Item("yt:state").Attributes("name").Value)
                                    sb.Append(":")
                                    sb.Append(xentry.Item("yt:state").InnerText)
                                    sb.AppendLine()
                                End If
                            Catch ex As Exception

                            End Try

                            mc = Regex.Match(videourl, "^http://www\.youtube\.com/watch\?v=([\w\-]+)", RegexOptions.IgnoreCase)
                            If mc.Success Then
                                imgurl = mc.Result("http://i.ytimg.com/vi/${1}/default.jpg")
                            End If
                            mc = Regex.Match(videourl, "^http://youtu\.be/([\w\-]+)", RegexOptions.IgnoreCase)
                            If mc.Success Then
                                imgurl = mc.Result("http://i.ytimg.com/vi/${1}/default.jpg")
                            End If

                        Catch ex As Exception

                        End Try

                        If Not String.IsNullOrEmpty(imgurl) Then
                            Dim _img As Image = http.GetImage(imgurl, videourl)
                            If _img Is Nothing Then Continue For
                            arg.pics.Add(New KeyValuePair(Of String, Image)(url.Key, _img))
                            arg.tooltiptext.Add(New KeyValuePair(Of String, String)(url.Key, sb.ToString.Trim()))
                        End If
                    End If

                End If
            Else
                ' 直リンクでなく、パターンに合致しない
                Dim img As Image = http.GetImage(url.Value, url.Key)
                If img Is Nothing Then Continue For
                arg.pics.Add(New KeyValuePair(Of String, Image)(url.Key, img))
                arg.tooltiptext.Add(New KeyValuePair(Of String, String)(url.Key, ""))
            End If
        Next
        If arg.pics.Count = 0 Then
            arg.IsError = True
        Else
            arg.IsError = False
        End If
        e.Result = arg
    End Sub

    Private Sub bgw_Completed(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)
        Dim prv As PreviewData = TryCast(e.Result, PreviewData)
        If prv Is Nothing OrElse prv.IsError Then
            Owner.PreviewScrollBar.Maximum = 0
            Owner.PreviewScrollBar.Enabled = False
            Owner.SplitContainer3.Panel2Collapsed = True
            If prv IsNot Nothing AndAlso Not String.IsNullOrEmpty(prv.AdditionalErrorMessage) Then
                ThumbnailProgressChanged(-1, prv.AdditionalErrorMessage)
            Else
                ThumbnailProgressChanged(-1)
            End If
            Exit Sub
        End If
        SyncLock lckPrev
            If prv IsNot Nothing AndAlso _curPost IsNot Nothing AndAlso prv.statusId = _curPost.Id Then
                _prev = prv
                Owner.SplitContainer3.Panel2Collapsed = False
                Owner.PreviewScrollBar.Maximum = _prev.pics.Count - 1
                If Owner.PreviewScrollBar.Maximum > 0 Then
                    Owner.PreviewScrollBar.Enabled = True
                Else
                    Owner.PreviewScrollBar.Enabled = False
                End If
                Owner.PreviewScrollBar.Value = 0
                Owner.PreviewPicture.Image = _prev.pics(0).Value
                If Not String.IsNullOrEmpty(_prev.tooltiptext(0).Value) Then
                    Owner.ToolTip1.SetToolTip(Owner.PreviewPicture, _prev.tooltiptext(0).Value)
                Else
                    Owner.ToolTip1.SetToolTip(Owner.PreviewPicture, "")
                End If
            ElseIf _curPost Is Nothing OrElse (_prev IsNot Nothing AndAlso _curPost.Id <> _prev.statusId) Then
                Owner.PreviewScrollBar.Maximum = 0
                Owner.PreviewScrollBar.Enabled = False
                Owner.SplitContainer3.Panel2Collapsed = True
            End If
        End SyncLock
        ThumbnailProgressChanged(100)
    End Sub

    Public Sub ScrollThumbnail(ByVal forward As Boolean)
        If forward Then
            Owner.PreviewScrollBar.Value = Math.Min(Owner.PreviewScrollBar.Value + 1, Owner.PreviewScrollBar.Maximum)
            PreviewScrollBar_Scroll(Owner.PreviewScrollBar, New ScrollEventArgs(ScrollEventType.SmallIncrement, Owner.PreviewScrollBar.Value))
        Else
            Owner.PreviewScrollBar.Value = Math.Max(Owner.PreviewScrollBar.Value - 1, Owner.PreviewScrollBar.Minimum)
            PreviewScrollBar_Scroll(Owner.PreviewScrollBar, New ScrollEventArgs(ScrollEventType.SmallDecrement, Owner.PreviewScrollBar.Value))
        End If
    End Sub

    Private Sub PreviewScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs)
        SyncLock lckPrev
            If _prev IsNot Nothing AndAlso _curPost IsNot Nothing AndAlso _prev.statusId = _curPost.Id Then
                If _prev.pics.Count > e.NewValue Then
                    Owner.PreviewPicture.Image = _prev.pics(e.NewValue).Value
                    If Not String.IsNullOrEmpty(_prev.tooltiptext(e.NewValue).Value) Then
                        Owner.ToolTip1.Hide(Owner.PreviewPicture)
                        Owner.ToolTip1.SetToolTip(Owner.PreviewPicture, _prev.tooltiptext(e.NewValue).Value)
                    Else
                        Owner.ToolTip1.SetToolTip(Owner.PreviewPicture, "")
                        Owner.ToolTip1.Hide(Owner.PreviewPicture)
                    End If
                End If
            End If
        End SyncLock
    End Sub

    Private Sub PreviewPicture_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs)
        Owner.ToolTip1.Hide(Owner.PreviewPicture)
    End Sub
    Private Sub PreviewPicture_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs)
        OpenPicture()
    End Sub
    Public Sub OpenPicture()
        If _prev IsNot Nothing Then
            If Owner.PreviewScrollBar.Value < _prev.pics.Count Then
                Owner.OpenUriAsync(_prev.pics(Owner.PreviewScrollBar.Value).Key)
            End If
        End If
    End Sub
End Class
