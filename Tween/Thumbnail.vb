' Tween - Client of Twitter
' Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
'           (c) 2008-2011 Moz (@syo68k)
'           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
'           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
'           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
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

Imports System.Text.RegularExpressions
Imports System.ComponentModel
Imports System.Text
Imports System.Xml
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json

Public Class Thumbnail

    Private lckPrev As New Object
    Private _prev As PreviewData
    Private Class PreviewData
        Implements IDisposable

        Public statusId As Long
        Public urls As List(Of KeyValuePair(Of String, String))
        Public pics As New List(Of KeyValuePair(Of String, Image))
        Public tooltipText As New List(Of KeyValuePair(Of String, String))
        Public imageCreators As New List(Of KeyValuePair(Of String, ImageCreatorDelegate))
        Public Sub New(ByVal id As Long, ByVal urlList As List(Of KeyValuePair(Of String, String)), ByVal imageCreatorList As List(Of KeyValuePair(Of String, ImageCreatorDelegate)))
            statusId = id
            urls = urlList
            imageCreators = imageCreatorList
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
    Private Delegate Function UrlCreatorDelegate(ByVal args As GetUrlArgs) As Boolean
    Private Delegate Function ImageCreatorDelegate(ByVal args As CreateImageArgs) As Boolean

    Private Class GetUrlArgs
        Public url As String
        Public imglist As List(Of KeyValuePair(Of String, String))
        Public geoInfo As Google.GlobalLocation
    End Class

    Private Class CreateImageArgs
        Public url As KeyValuePair(Of String, String)
        Public pics As List(Of KeyValuePair(Of String, Image))
        Public tooltipText As List(Of KeyValuePair(Of String, String))
        Public errmsg As String
    End Class

    Private Class ThumbnailService
        Public Name As String
        Public urlCreator As UrlCreatorDelegate
        Public imageCreator As ImageCreatorDelegate

        Public Sub New(ByVal name As String, ByVal urlcreator As UrlCreatorDelegate, ByVal imagecreator As ImageCreatorDelegate)
            Me.Name = name
            Me.urlCreator = urlcreator
            Me.imageCreator = imagecreator
        End Sub
    End Class

    Private ThumbnailServices As ThumbnailService() = {
        New ThumbnailService("ImgUr", AddressOf ImgUr_GetUrl, AddressOf ImgUr_CreateImage),
        New ThumbnailService("DirectLink", AddressOf DirectLink_GetUrl, AddressOf DirectLink_CreateImage),
        New ThumbnailService("TwitPic", AddressOf TwitPic_GetUrl, AddressOf TwitPic_CreateImage),
        New ThumbnailService("yfrog", AddressOf yfrog_GetUrl, AddressOf yfrog_CreateImage),
        New ThumbnailService("Plixi(TweetPhoto)", AddressOf Plixi_GetUrl, AddressOf Plixi_CreateImage),
        New ThumbnailService("MobyPicture", AddressOf MobyPicture_GetUrl, AddressOf MobyPicture_CreateImage),
        New ThumbnailService("携帯百景", AddressOf MovaPic_GetUrl, AddressOf MovaPic_CreateImage),
        New ThumbnailService("はてなフォトライフ", AddressOf Hatena_GetUrl, AddressOf Hatena_CreateImage),
        New ThumbnailService("PhotoShare/bctiny", AddressOf PhotoShare_GetUrl, AddressOf PhotoShare_CreateImage),
        New ThumbnailService("img.ly", AddressOf imgly_GetUrl, AddressOf imgly_CreateImage),
        New ThumbnailService("brightkite", AddressOf brightkite_GetUrl, AddressOf brightkite_CreateImage),
        New ThumbnailService("Twitgoo", AddressOf Twitgoo_GetUrl, AddressOf Twitgoo_CreateImage),
        New ThumbnailService("youtube", AddressOf youtube_GetUrl, AddressOf youtube_CreateImage),
        New ThumbnailService("ニコニコ動画", AddressOf nicovideo_GetUrl, AddressOf nicovideo_CreateImage),
        New ThumbnailService("ニコニコ静画", AddressOf nicoseiga_GetUrl, AddressOf nicoseiga_CreateImage),
        New ThumbnailService("Pixiv", AddressOf Pixiv_GetUrl, AddressOf Pixiv_CreateImage),
        New ThumbnailService("flickr", AddressOf flickr_GetUrl, AddressOf flickr_CreateImage),
        New ThumbnailService("フォト蔵", AddressOf Photozou_GetUrl, AddressOf Photozou_CreateImage),
        New ThumbnailService("TwitVideo", AddressOf TwitVideo_GetUrl, AddressOf TwitVideo_CreateImage),
        New ThumbnailService("Piapro", AddressOf Piapro_GetUrl, AddressOf Piapro_CreateImage),
        New ThumbnailService("Tumblr", AddressOf Tumblr_GetUrl, AddressOf Tumblr_CreateImage),
        New ThumbnailService("ついっぷるフォト", AddressOf TwipplePhoto_GetUrl, AddressOf TwipplePhoto_CreateImage),
        New ThumbnailService("mypix/shamoji", AddressOf mypix_GetUrl, AddressOf mypix_CreateImage),
        New ThumbnailService("ow.ly", AddressOf Owly_GetUrl, AddressOf Owly_CreateImage),
        New ThumbnailService("vimeo", AddressOf Vimeo_GetUrl, AddressOf Vimeo_CreateImage),
        New ThumbnailService("cloudfiles", AddressOf CloudFiles_GetUrl, AddressOf CloudFiles_CreateImage),
        New ThumbnailService("instagram", AddressOf instagram_GetUrl, AddressOf instagram_CreateImage),
        New ThumbnailService("pikubo", AddressOf pikubo_GetUrl, AddressOf pikubo_CreateImage),
        New ThumbnailService("PicPlz", AddressOf PicPlz_GetUrl, AddressOf PicPlz_CreateImage),
        New ThumbnailService("FourSquare", AddressOf Foursquare_GetUrl, AddressOf Foursquare_CreateImage)
        }

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

    Public Sub thumbnail(ByVal id As Long, ByVal links As List(Of String), ByVal geo As PostClass.StatusGeo)
        If Not Owner.IsPreviewEnable Then
            Owner.SplitContainer3.Panel2Collapsed = True
            Exit Sub
        End If
        If Owner.PreviewPicture.Image IsNot Nothing Then
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

        If links.Count = 0 AndAlso geo Is Nothing Then
            Owner.PreviewScrollBar.Maximum = 0
            Owner.PreviewScrollBar.Enabled = False
            Owner.SplitContainer3.Panel2Collapsed = True
            Exit Sub
        End If

        Dim imglist As New List(Of KeyValuePair(Of String, String))
        Dim dlg As New List(Of KeyValuePair(Of String, ImageCreatorDelegate))

        For Each url As String In links
            For Each svc As ThumbnailService In ThumbnailServices
                Dim args As New GetUrlArgs
                args.url = url
                args.imglist = imglist
                If svc.urlCreator(args) Then
                    ' URLに対応したサムネイル作成処理デリゲートをリストに登録
                    dlg.Add(New KeyValuePair(Of String, ImageCreatorDelegate)(url, svc.imageCreator))
                    Exit For
                End If
            Next
        Next
        If geo IsNot Nothing Then
            Dim args As New GetUrlArgs
            args.url = ""
            args.imglist = imglist
            args.geoInfo = New Google.GlobalLocation() With {.Latitude = geo.Lat, .Longitude = geo.Lng}
            If TwitterGeo_GetUrl(args) Then
                ' URLに対応したサムネイル作成処理デリゲートをリストに登録
                dlg.Add(New KeyValuePair(Of String, ImageCreatorDelegate)(args.url, New ImageCreatorDelegate(AddressOf TwitterGeo_CreateImage)))
            End If
        End If
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
        bgw.RunWorkerAsync(New PreviewData(id, imglist, dlg))

    End Sub

    Private Sub ThumbnailProgressChanged(ByVal ProgressPercentage As Integer, Optional ByVal AddMsg As String = "")
        If ProgressPercentage = 0 Then    '開始
            'Owner.SetStatusLabel("Thumbnail generating...")
        ElseIf ProgressPercentage = 100 Then '正常終了
            'Owner.SetStatusLabel("Thumbnail generated.")
        Else ' エラー
            If String.IsNullOrEmpty(AddMsg) Then
                Owner.SetStatusLabel("can't get Thumbnail.")
            Else
                Owner.SetStatusLabel("can't get Thumbnail.(" + AddMsg + ")")
            End If
        End If
    End Sub

    Private Sub bgw_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
        Dim arg As PreviewData = DirectCast(e.Argument, PreviewData)
        Dim worker As BackgroundWorker = DirectCast(sender, BackgroundWorker)
        arg.AdditionalErrorMessage = ""

        For Each url As KeyValuePair(Of String, String) In arg.urls
            Dim args As New CreateImageArgs
            args.url = url
            args.pics = arg.pics
            args.tooltipText = arg.tooltipText
            args.errmsg = ""
            If Not arg.imageCreators.Item(arg.urls.IndexOf(url)).Value(args) Then
                arg.AdditionalErrorMessage = args.errmsg
                arg.IsError = True
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
            If prv IsNot Nothing AndAlso _curPost IsNot Nothing AndAlso prv.statusId = _curPost.StatusId Then
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
                If Not String.IsNullOrEmpty(_prev.tooltipText(0).Value) Then
                    Owner.ToolTip1.SetToolTip(Owner.PreviewPicture, _prev.tooltipText(0).Value)
                Else
                    Owner.ToolTip1.SetToolTip(Owner.PreviewPicture, "")
                End If
            ElseIf _curPost Is Nothing OrElse (_prev IsNot Nothing AndAlso _curPost.StatusId <> _prev.statusId) Then
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
            If _prev IsNot Nothing AndAlso _curPost IsNot Nothing AndAlso _prev.statusId = _curPost.StatusId Then
                If _prev.pics.Count > e.NewValue Then
                    Owner.PreviewPicture.Image = _prev.pics(e.NewValue).Value
                    If Not String.IsNullOrEmpty(_prev.tooltipText(e.NewValue).Value) Then
                        Owner.ToolTip1.Hide(Owner.PreviewPicture)
                        Owner.ToolTip1.SetToolTip(Owner.PreviewPicture, _prev.tooltipText(e.NewValue).Value)
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

#Region "テンプレ"
#If 0 Then
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function ServiceName_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://imgur\.com/(\w+)\.jpg$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://i.imgur.com/${1}l.jpg")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function ServiceName_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltiptext.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function
#End If
#End Region

#Region "ImgUr"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function ImgUr_GetUrl(ByVal args As GetUrlArgs) As Boolean
        Dim mc As Match = Regex.Match(args.url, "^http://imgur\.com/(\w+)\.jpg$", RegexOptions.IgnoreCase)
        If mc.Success Then
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://i.imgur.com/${1}l.jpg")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>

    Private Function ImgUr_CreateImage(ByVal args As CreateImageArgs) As Boolean
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function
#End Region

#Region "画像直リンク"
    Private Function DirectLink_GetUrl(ByVal args As GetUrlArgs) As Boolean
        '画像拡張子で終わるURL（直リンク）
        If IsDirectLink(args.url) Then
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, args.url))
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>

    Private Function DirectLink_CreateImage(ByVal args As CreateImageArgs) As Boolean
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then Return False
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function
#End Region

#Region "TwitPic"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function TwitPic_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://(www\.)?twitpic\.com/(?<photoId>\w+)(/full/?)?$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://twitpic.com/show/thumb/${photoId}")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function TwitPic_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "yfrog"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function yfrog_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://yfrog\.com/(\w+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, args.url + ".th.jpg"))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function yfrog_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "Plixi(TweetPhoto)"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function Plixi_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^(http://tweetphoto\.com/[0-9]+|http://pic\.gd/[a-z0-9]+|http://(lockerz|plixi)\.com/[ps]/[0-9]+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            Const comp As String = "http://api.plixi.com/api/tpapi.svc/imagefromurl?size=thumbnail&url="
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, comp + args.url))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function Plixi_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "MobyPicture"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function MobyPicture_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://moby\.to/(\w+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://mobypicture.com/?${1}:small")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function MobyPicture_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "携帯百景"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function MovaPic_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://movapic\.com/pic/(\w+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://image.movapic.com/pic/s_${1}.jpeg")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function MovaPic_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "はてなフォトライフ"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function Hatena_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://f\.hatena\.ne\.jp/(([a-z])[a-z0-9_-]{1,30}[a-z0-9])/((\d{8})\d+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://img.f.hatena.ne.jp/images/fotolife/${2}/${1}/${4}/${3}_120.jpg")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function Hatena_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "PhotoShare"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function PhotoShare_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://(?:www\.)?bcphotoshare\.com/photos/\d+/(\d+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://images.bcphotoshare.com/storages/${1}/thumb180.jpg")))
            Return True
        End If
        ' 短縮URL
        mc = Regex.Match(args.url, "^http://bctiny\.com/p(\w+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            Try
                args.imglist.Add(New KeyValuePair(Of String, String)(args.url, "http://images.bcphotoshare.com/storages/" + RadixConvert.ToInt32(mc.Result("${1}"), 36).ToString + "/thumb180.jpg"))
                Return True
            Catch ex As ArgumentOutOfRangeException
            End Try
        End If
        Return False
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function PhotoShare_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "img.ly"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function imgly_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://img\.ly/(\w+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://img.ly/show/thumb/${1}")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function imgly_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "brightkite"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function brightkite_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://brightkite\.com/objects/((\w{2})(\w{2})\w+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://cdn.brightkite.com/${2}/${3}/${1}-feed.jpg")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function brightkite_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "Twitgoo"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function Twitgoo_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://twitgoo\.com/(\w+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://twitgoo.com/${1}/mini")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function Twitgoo_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "youtube"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function youtube_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://www\.youtube\.com/watch\?v=([\w\-]+)", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://i.ytimg.com/vi/${1}/default.jpg")))
            Return True
        End If
        mc = Regex.Match(args.url, "^http://youtu\.be/([\w\-]+)", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://i.ytimg.com/vi/${1}/default.jpg")))
            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function youtube_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        ' 参考
        ' http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_video_entries.html
        ' デベロッパー ガイド: Data API プロトコル - 単独の動画情報の取得 - YouTube の API とツール - Google Code
        ' http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_understanding_video_feeds.html#Understanding_Feeds_and_Entries 
        ' デベロッパー ガイド: Data API プロトコル - 動画のフィードとエントリについて - YouTube の API とツール - Google Code
        Dim http As New HttpVarious
        Dim videourl As String = (New HttpVarious).GetRedirectTo(args.url.Key)
        Dim mc As Match = Regex.Match(videourl, "^http://(?:(www\.youtube\.com)|(youtu\.be))/(watch\?v=)?(?<videoid>([\w\-]+))", RegexOptions.IgnoreCase)
        If videourl.StartsWith("http://www.youtube.com/index?ytsession=") Then
            videourl = args.url.Key
            mc = Regex.Match(videourl, "^http://(?:(www\.youtube\.com)|(youtu\.be))/(watch\?v=)?(?<videoid>([\w\-]+))", RegexOptions.IgnoreCase)
        End If
        If mc.Success Then
            Dim apiurl As String = "http://gdata.youtube.com/feeds/api/videos/" + mc.Groups("videoid").Value
            Dim imgurl As String = args.url.Value
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
                            sb.Append(My.Resources.YouTubeInfoText1)
                            sb.Append(tmp)
                            sb.AppendLine()
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        Dim sec As Integer = 0
                        If Integer.TryParse(xentry.Item("yt:duration").Attributes("seconds").Value, sec) Then
                            sb.Append(My.Resources.YouTubeInfoText2)
                            sb.AppendFormat("{0:d}:{1:d2}", sec \ 60, sec Mod 60)
                            sb.AppendLine()
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        Dim tmpdate As New DateTime
                        xentry = CType(xdoc.DocumentElement.SelectSingleNode("/root:entry", nsmgr), XmlElement)
                        If DateTime.TryParse(xentry.Item("published").InnerText, tmpdate) Then
                            sb.Append(My.Resources.YouTubeInfoText3)
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
                            sb.Append(My.Resources.YouTubeInfoText4)
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
                    Dim _img As Image = http.GetImage(imgurl, videourl, 10000, args.errmsg)
                    If _img Is Nothing Then Return False
                    args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
                    args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, sb.ToString.Trim()))
                    Return True
                End If
            End If

        End If
        Return False
    End Function

#End Region

#Region "ニコニコ動画"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function nicovideo_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://(?:(www|ext)\.nicovideo\.jp/watch|nico\.ms)/(?:sm|nm)?([0-9]+)(\?.+)?$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function nicovideo_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim http As New HttpVarious
        Dim mc As Match = Regex.Match(args.url.Value, "^http://(?:(www|ext)\.nicovideo\.jp/watch|nico\.ms)/(?<id>(?:sm|nm)?([0-9]+))(\?.+)?$", RegexOptions.IgnoreCase)
        Dim apiurl As String = "http://www.nicovideo.jp/api/getthumbinfo/" + mc.Groups("id").Value
        Dim src As String = ""
        Dim imgurl As String = ""
        If (New HttpVarious).GetData(apiurl, Nothing, src, 0, args.errmsg, GetUserAgentString()) Then
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
                    args.errmsg = errcode
                    imgurl = ""
                Else
                    args.errmsg = "UnknownResponse"
                    imgurl = ""
                End If

            Catch ex As Exception
                imgurl = ""
                args.errmsg = "Invalid XML"
            End Try

            If Not String.IsNullOrEmpty(imgurl) Then
                Dim _img As Image = http.GetImage(imgurl, args.url.Key, 0, args.errmsg)
                If _img Is Nothing Then Return False
                args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
                args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, sb.ToString.Trim()))
                Return True
            End If
        End If
        Return False
    End Function

#End Region

#Region "ニコニコ静画"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function nicoseiga_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://(?:seiga\.nicovideo\.jp/seiga/|nico\.ms/)im\d+")
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function nicoseiga_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim http As New HttpVarious
        Dim mc As Match = Regex.Match(args.url.Key, "^http://(?:seiga\.nicovideo\.jp/seiga/|nico\.ms/)im(?<id>\d+)")
        If mc.Success Then
            Dim _img As Image = http.GetImage("http://lohas.nicoseiga.jp/thumb/" + mc.Groups("id").Value + "q?", args.url.Key, 0, args.errmsg)
            If _img Is Nothing Then Return False
            args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
            args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
            Return True
        End If
        Return False
    End Function

#End Region

#Region "Pixiv"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function Pixiv_GetUrl(ByVal args As GetUrlArgs) As Boolean
        '参考: http://tail.s68.xrea.com/blog/2009/02/pixivflash.html Pixivの画像をFlashとかで取得する方法など:しっぽのブログ
        'ユーザー向けの画像ページ http://www.pixiv.net/member_illust.php?mode=medium&illust_id=[ID番号]
        '非ログインユーザー向けの画像ページ http://www.pixiv.net/index.php?mode=medium&illust_id=[ID番号]
        'サムネイルURL http://img[サーバー番号].pixiv.net/img/[ユーザー名]/[サムネイルID]_s.[拡張子]
        'サムネイルURLは画像ページから抽出する
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://www\.pixiv\.net/(member_illust|index)\.php\?mode=(medium|big)&(amp;)?illust_id=(?<illustId>[0-9]+)(&(amp;)?tag=(?<tag>.+)?)*$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url.Replace("amp;", ""), mc.Value))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function Pixiv_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim src As String = ""
        'illustIDをキャプチャ
        Dim mc As Match = Regex.Match(args.url.Key, "^http://www\.pixiv\.net/(member_illust|index)\.php\?mode=(medium|big)&(amp;)?illust_id=(?<illustId>[0-9]+)(&(amp;)?tag=(?<tag>.+)?)*$", RegexOptions.IgnoreCase)
        If mc.Groups("tag").Value = "R-18" OrElse mc.Groups("tag").Value = "R-18G" Then
            args.errmsg = "NotSupported"
            Return False
        Else
            Dim http As New HttpVarious
            If http.GetData(Regex.Replace(mc.Groups(0).Value, "amp;", ""), Nothing, src, 0, args.errmsg, "") Then
                Dim _mc As Match = Regex.Match(src, mc.Result("http://img([0-9]+)\.pixiv\.net/img/.+/${illustId}_[ms]\.([a-zA-Z]+)"))
                If _mc.Success Then
                    Dim _img As Image = http.GetImage(_mc.Value, args.url.Key, 0, args.errmsg)
                    If _img Is Nothing Then Return False
                    args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
                    args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
                    Return True
                ElseIf Regex.Match(src, "<span class='error'>ログインしてください</span>").Success Then
                    args.errmsg = "NotSupported"
                Else
                    args.errmsg = "Pattern NotFound"
                End If
            End If
        End If
        Return False
    End Function

#End Region

#Region "flickr"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function flickr_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://www.flickr.com/", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function flickr_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        '参考: http://tanarky.blogspot.com/2010/03/flickr-urlunavailable.html アグレッシブエンジニア: flickr の画像URL仕様についてまとめ(Unavailable画像)
        '画像URL仕様　http://farm{farm}.static.flickr.com/{server}/{id}_{secret}_{size}.{extension} 
        'photostreamなど複数の画像がある場合先頭の一つのみ認識と言うことにする
        '(二つ目のキャプチャ 一つ目の画像はユーザーアイコン）

        Dim src As String = ""
        Dim mc As Match = Regex.Match(args.url.Key, "^http://www.flickr.com/", RegexOptions.IgnoreCase)
        Dim http As New HttpVarious
        If http.GetData(args.url.Key, Nothing, src, 0, args.errmsg, "") Then
            Dim _mc As MatchCollection = Regex.Matches(src, mc.Result("http://farm[0-9]+\.static\.flickr\.com/[0-9]+/.+?\.([a-zA-Z]+)"))
            '二つ以上キャプチャした場合先頭の一つだけ 一つだけの場合はユーザーアイコンしか取れなかった
            If _mc.Count > 1 Then
                Dim _img As Image = http.GetImage(_mc.Item(1).Value, args.url.Key, 0, args.errmsg)
                If _img Is Nothing Then Return False
                args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
                args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
                Return True
            Else
                args.errmsg = "Pattern NotFound"
            End If
        End If
        Return False
    End Function

#End Region

#Region "フォト蔵"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function Photozou_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://photozou\.jp/photo/show/(?<userId>[0-9]+)/(?<photoId>[0-9]+)", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function Photozou_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim http As New HttpVarious
        Dim mc As Match = Regex.Match(args.url.Key, "^http://photozou\.jp/photo/show/(?<userId>[0-9]+)/(?<photoId>[0-9]+)", RegexOptions.IgnoreCase)
        If mc.Success Then
            Dim src As String = ""
            Dim show_info As String = mc.Result("http://api.photozou.jp/rest/photo_info?photo_id=${photoId}")
            If http.GetData(show_info, Nothing, src, 0, args.errmsg, "") Then
                Dim xdoc As New XmlDocument
                Dim thumbnail_url As String = ""
                Try
                    xdoc.LoadXml(src)
                    thumbnail_url = xdoc.SelectSingleNode("/rsp/info/photo/thumbnail_image_url").InnerText
                Catch ex As Exception
                    args.errmsg = ex.Message
                    thumbnail_url = ""
                End Try
                If String.IsNullOrEmpty(thumbnail_url) Then Return False
                Dim _img As Image = http.GetImage(thumbnail_url, args.url.Key)
                If _img Is Nothing Then Return False
                args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
                args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
                Return True
            End If
        End If
        Return False
    End Function

#End Region

#Region "TwitVideo"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function TwitVideo_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://twitvideo\.jp/(\w+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://twitvideo.jp/img/thumb/${1}")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function TwitVideo_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 0, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "Piapro"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function Piapro_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://piapro\.jp/(?:content/[0-9a-z]+|t/[0-9a-zA-Z_\-]+)$")
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function Piapro_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim http As New HttpVarious
        Dim mc As Match = Regex.Match(args.url.Key, "^http://piapro\.jp/(?:content/[0-9a-z]+|t/[0-9a-zA-Z_\-]+)$")
        If mc.Success Then
            Dim src As String = ""
            If http.GetData(args.url.Key, Nothing, src, 0, args.errmsg, "") Then
                Dim _mc As Match = Regex.Match(src, "<meta property=""og:image"" content=""(?<big_img>http://c1\.piapro\.jp/timg/[0-9a-z]+_\d{14}_0500_0500\.(?:jpg|png|gif)?)"" />")
                If _mc.Success Then
                    '各画像には120x120のサムネイルがある（多分）ので、URLを置き換える。元々ページに埋め込まれている画像は500x500
                    Dim r As New System.Text.RegularExpressions.Regex("_\d{4}_\d{4}")
                    Dim min_img_url As String = r.Replace(_mc.Groups("big_img").Value, "_0120_0120")
                    Dim _img As Image = http.GetImage(min_img_url, args.url.Key, 0, args.errmsg)
                    If _img Is Nothing Then Return False
                    args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
                    args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
                    Return True
                Else
                    args.errmsg = "Pattern NotFound"
                End If
            End If
        End If
        Return False
    End Function

#End Region

#Region "Tumblr"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function Tumblr_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://.+\.tumblr\.com/.+/?", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function Tumblr_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim http As New HttpVarious
        Dim TargetUrl As String = http.GetRedirectTo(args.url.Key)
        Dim mc As Match = Regex.Match(TargetUrl, "(?<base>http://.+?\.tumblr\.com/)post/(?<postID>[0-9]+)(/(?<subject>.+?)/)?", RegexOptions.IgnoreCase)
        Dim apiurl As String = mc.Groups("base").Value + "api/read?id=" + mc.Groups("postID").Value
        Dim src As String = ""
        Dim imgurl As String = Nothing
        If http.GetData(apiurl, Nothing, src, 0, args.errmsg, "") Then
            Dim xdoc As New XmlDocument
            Try
                xdoc.LoadXml(src)

                Dim type As String = xdoc.SelectSingleNode("/tumblr/posts/post").Attributes("type").Value
                If type = "photo" Then
                    imgurl = xdoc.SelectSingleNode("/tumblr/posts/post/photo-url").InnerText
                Else
                    args.errmsg = "PostType:" + type
                    imgurl = ""
                End If
            Catch ex As Exception
                imgurl = ""
            End Try

            If Not String.IsNullOrEmpty(imgurl) Then
                Dim _img As Image = http.GetImage(imgurl, args.url.Key, 0, args.errmsg)
                If _img Is Nothing Then Return False
                args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
                args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
                Return True
            End If
        End If
        Return False
    End Function

#End Region

#Region "ついっぷるフォト"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function TwipplePhoto_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://p\.twipple\.jp/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function TwipplePhoto_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim http As New HttpVarious
        Dim mc As Match = Regex.Match(args.url.Key, "^http://p.twipple.jp/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase)
        If mc.Success Then
            Dim src As String = ""
            If http.GetData(args.url.Key, Nothing, src, 0, args.errmsg, "") Then
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

                If String.IsNullOrEmpty(thumbnail_url) Then Return False
                Dim _img As Image = http.GetImage(thumbnail_url, args.url.Key, 0, args.errmsg)
                If _img Is Nothing Then Return False
                args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
                args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
                Return True
            End If
        End If
        Return False
    End Function

#End Region

#Region "mypix/shamoji"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function mypix_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://(www\.mypix\.jp|www\.shamoji\.info)/app\.php/picture/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value + "/thumb.jpg"))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function mypix_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 0, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "ow.ly"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function Owly_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://ow\.ly/i/(\w+)$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://static.ow.ly/photos/thumb/${1}.jpg")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function Owly_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 0, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "vimeo"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function Vimeo_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://vimeo\.com/[0-9]+", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function Vimeo_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim http As New HttpVarious
        Dim mc As Match = Regex.Match(args.url.Key, "http://vimeo\.com/(?<postID>[0-9]+)", RegexOptions.IgnoreCase)
        Dim apiurl As String = "http://vimeo.com/api/v2/video/" + mc.Groups("postID").Value + ".xml"
        Dim src As String = ""
        Dim imgurl As String = Nothing
        If http.GetData(apiurl, Nothing, src, 0, args.errmsg, "") Then
            Dim xdoc As New XmlDocument
            Dim sb As New StringBuilder
            Try
                xdoc.LoadXml(src)
                Try
                    Dim tmp As String = xdoc.SelectSingleNode("videos/video/title").InnerText
                    If Not String.IsNullOrEmpty(tmp) Then
                        sb.Append(My.Resources.VimeoInfoText1)
                        sb.Append(tmp)
                        sb.AppendLine()
                    End If
                Catch ex As Exception
                End Try
                Try
                    Dim tmpdate As New DateTime
                    If DateTime.TryParse(xdoc.SelectSingleNode("videos/video/upload_date").InnerText, tmpdate) Then
                        sb.Append(My.Resources.VimeoInfoText2)
                        sb.Append(tmpdate)
                        sb.AppendLine()
                    End If
                Catch ex As Exception
                End Try
                Try
                    Dim tmp As String = xdoc.SelectSingleNode("videos/video/stats_number_of_likes").InnerText
                    If Not String.IsNullOrEmpty(tmp) Then
                        sb.Append(My.Resources.VimeoInfoText3)
                        sb.Append(tmp)
                        sb.AppendLine()
                    End If
                Catch ex As Exception
                End Try
                Try
                    Dim tmp As String = xdoc.SelectSingleNode("videos/video/stats_number_of_plays").InnerText
                    If Not String.IsNullOrEmpty(tmp) Then
                        sb.Append(My.Resources.VimeoInfoText4)
                        sb.Append(tmp)
                        sb.AppendLine()
                    End If
                Catch ex As Exception
                End Try
                Try
                    Dim tmp As String = xdoc.SelectSingleNode("videos/video/stats_number_of_comments").InnerText
                    If Not String.IsNullOrEmpty(tmp) Then
                        sb.Append(My.Resources.VimeoInfoText5)
                        sb.Append(tmp)
                        sb.AppendLine()
                    End If
                Catch ex As Exception
                End Try
                Try
                    Dim sec As Integer = 0
                    If Integer.TryParse(xdoc.SelectSingleNode("videos/video/duration").InnerText, sec) Then
                        sb.Append(My.Resources.VimeoInfoText6)
                        sb.AppendFormat("{0:d}:{1:d2}", sec \ 60, sec Mod 60)
                        sb.AppendLine()
                    End If
                Catch ex As Exception
                End Try
                Try
                    Dim tmp As String = xdoc.SelectSingleNode("videos/video/thumbnail_medium").InnerText
                    If Not String.IsNullOrEmpty(tmp) Then
                        imgurl = tmp
                    End If
                Catch ex As Exception
                End Try
            Catch ex As Exception
                imgurl = ""
            End Try

            If Not String.IsNullOrEmpty(imgurl) Then
                Dim _img As Image = http.GetImage(imgurl, args.url.Key, 0, args.errmsg)
                If _img Is Nothing Then Return False
                args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
                args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, sb.ToString.Trim()))
                Return True
            End If
        End If
        Return False
    End Function

#End Region

#Region "cloudfiles"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function CloudFiles_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://c[0-9]+\.cdn[0-9]+\.cloudfiles\.rackspacecloud\.com/[a-z_0-9]+", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function CloudFiles_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 0, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function
#End Region

#Region "Instagram"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function instagram_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://instagr.am/p/.+/", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function instagram_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します

        Dim src As String = ""
        Dim http As New HttpVarious
        If http.GetData(args.url.Key, Nothing, src, 0, args.errmsg, "") Then
            Dim mc As Match = Regex.Match(src, "<meta property=""og:image"" content=""(?<url>.+)""/>")
            '二つ以上キャプチャした場合先頭の一つだけ 一つだけの場合はユーザーアイコンしか取れなかった
            If mc.Success Then
                Dim _img As Image = http.GetImage(mc.Groups("url").Value, args.url.Key, 0, args.errmsg)
                If _img Is Nothing Then Return False
                args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
                args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
                Return True
            Else
                args.errmsg = "Pattern NotFound"
            End If
        End If
        Return False
    End Function

#End Region

#Region "pikubo"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function pikubo_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://pikubo\.me/([a-z0-9-_]+)", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Result("http://pikubo.me/q/${1}")))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function pikubo_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します

        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 0, args.errmsg)
        If img Is Nothing Then Return False
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function

#End Region

#Region "PicPlz"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function PicPlz_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^http://picplz\.com/user/\w+/pic/(?<longurl_ids>\w+)/?$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        End If
        mc = Regex.Match(args.url, "^http://picplz\.com/(?<shorturl_ids>\w+)?$", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, mc.Value))
            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>

    Private Class PicPlzDataModel
        <DataContract()>
        Public Class Icon
            <DataMember(Name:="url")> Public Url As String
            <DataMember(Name:="width")> Public Width As Integer
            <DataMember(Name:="height")> Public Height As Integer
        End Class

        <DataContract()>
        Public Class Creator
            <DataMember(Name:="username")> Public Username As String
            <DataMember(Name:="display_name")> Public DisplayName As String
            <DataMember(Name:="following_count")> Public FollowingCount As Int32
            <DataMember(Name:="follower_count")> Public FollowerCount As Int32
            <DataMember(Name:="id")> Public Id As Int32
            <DataMember(Name:="icon")> Public Icon As PicPlzDataModel.Icon
        End Class

        <DataContract()>
        Public Class PicFileInfo
            <DataMember(Name:="width")> Public Width As Integer
            <DataMember(Name:="img_url")> Public ImgUrl As String
            <DataMember(Name:="height")> Public Height As Integer
        End Class


        <DataContract()>
        Public Class PicFiles
            <DataMember(Name:="640r")> Public Pic640r As PicFileInfo
            <DataMember(Name:="100sh")> Public Pic100sh As PicFileInfo
            <DataMember(Name:="320rh")> Public Pic320rh As PicFileInfo
        End Class

        <DataContract()>
        Public Class Pics
            <DataMember(Name:="view_count")> Public ViewCount As Integer
            <DataMember(Name:="creator")> Public Creator As Creator
            <DataMember(Name:="url")> Public Url As String
            <DataMember(Name:="pic_files")> Public PicFiles As PicFiles
            <DataMember(Name:="caption")> Public Caption As String
            <DataMember(Name:="comment_count")> Public CommentCount As Integer
            <DataMember(Name:="like_count")> Public LikeCount As Integer
            <DataMember(Name:="date")> Public _Date As Int64
            <DataMember(Name:="id")> Public Id As Integer
        End Class

        <DataContract()>
        Public Class Value
            <DataMember(Name:="pics")> Public Pics As Pics()
        End Class

        <DataContract()>
        Public Class ResultData
            <DataMember(Name:="result")> Public Result As String
            <DataMember(Name:="value")> Public Value As Value
        End Class
    End Class

    Private Function PicPlz_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim http As New HttpVarious
        Dim mc As Match
        Dim apiurl As String = "http://api.picplz.com/api/v2/pic.json?"
        mc = Regex.Match(args.url.Value, "^http://picplz\.com/user/\w+/pic/(?<longurl_ids>\w+)/?$", RegexOptions.IgnoreCase)
        If mc.Success Then
            apiurl += "longurl_ids=" + mc.Groups("longurl_ids").Value
        Else
            mc = Regex.Match(args.url.Value, "^http://picplz\.com/(?<shorturl_ids>\w+)?$", RegexOptions.IgnoreCase)
            If mc.Success Then
                apiurl += "shorturl_ids=" + mc.Groups("shorturl_ids").Value
            Else
                Return False
            End If
        End If
        Dim src As String = ""
        Dim imgurl As String = ""
        If (New HttpVarious).GetData(apiurl, Nothing, src, 0, args.errmsg, GetUserAgentString()) Then
            Dim sb As New StringBuilder
            Dim serializer As New DataContractJsonSerializer(GetType(PicPlzDataModel.ResultData))
            Dim res As PicPlzDataModel.ResultData

            Try
                res = CreateDataFromJson(Of PicPlzDataModel.ResultData)(src)
            Catch ex As Exception
                Return False
            End Try

            If res.Result = "ok" Then
                Try
                    imgurl = res.Value.Pics(0).PicFiles.Pic320rh.ImgUrl
                Catch ex As Exception

                End Try

                Try
                    sb.Append(res.Value.Pics(0).Caption)
                Catch ex As Exception

                End Try
            Else
                Return False
            End If

            If Not String.IsNullOrEmpty(imgurl) Then
                Dim _img As Image = http.GetImage(imgurl, args.url.Key, 0, args.errmsg)
                If _img Is Nothing Then Return False
                args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, _img))
                args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, sb.ToString.Trim()))
                Return True
            End If
        End If
        Return False
    End Function

#End Region

#Region "Foursquare"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function Foursquare_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        Dim mc As Match = Regex.Match(args.url, "^https?://(4sq|foursquare).com/", RegexOptions.IgnoreCase)
        If mc.Success Then
            ' TODO 成功時はサムネイルURLを作成しimglist.Addする
            'Dim mapsUrl As String = Foursquare.GetInstance.GetMapsUri(args.url)
            'If mapsUrl Is Nothing Then Return False
            args.imglist.Add(New KeyValuePair(Of String, String)(args.url, ""))
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function Foursquare_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim tipsText As String = ""
        Dim mapsUrl As String = Foursquare.GetInstance.GetMapsUri(args.url.Key, tipsText)
        If mapsUrl Is Nothing Then Return False
        Dim img As Image = (New HttpVarious).GetImage(mapsUrl, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, tipsText))
        Return True
    End Function
#End Region

#Region "Twitter Geo"
    ''' <summary>
    ''' URL解析部で呼び出されるサムネイル画像URL作成デリゲート
    ''' </summary>
    ''' <param name="args">Class GetUrlArgs
    '''                                 args.url        URL文字列
    '''                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
    ''' </param>
    ''' <returns>成功した場合True,失敗の場合False</returns>
    ''' <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

    Private Function TwitterGeo_GetUrl(ByVal args As GetUrlArgs) As Boolean
        ' TODO URL判定処理を記述
        If args.geoInfo IsNot Nothing AndAlso (args.geoInfo.Latitude <> 0 OrElse args.geoInfo.Longitude <> 0) Then
            Dim url As String = (New Google).CreateGoogleMapsUri(args.geoInfo)
            args.imglist.Add(New KeyValuePair(Of String, String)(url, url))
            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
    ''' </summary>
    ''' <param name="args">Class CreateImageArgs
    '''                                 url As KeyValuePair(Of String, String)                  元URLとサムネイルURLのKeyValuePair
    '''                                 pics As List(Of KeyValuePair(Of String, Image))         元URLとサムネイル画像のKeyValuePair
    '''                                 tooltiptext As List(Of KeyValuePair(Of String, String)) 元URLとツールチップテキストのKeyValuePair
    '''                                 errmsg As String                                        取得に失敗した際のエラーメッセージ
    ''' </param>
    ''' <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
    ''' なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
    ''' <remarks></remarks>
    Private Function TwitterGeo_CreateImage(ByVal args As CreateImageArgs) As Boolean
        ' TODO: サムネイル画像読み込み処理を記述します
        Dim img As Image = (New HttpVarious).GetImage(args.url.Value, args.url.Key, 10000, args.errmsg)
        If img Is Nothing Then
            Return False
        End If
        ' 成功した場合はURLに対応する画像、ツールチップテキストを登録
        args.pics.Add(New KeyValuePair(Of String, Image)(args.url.Key, img))
        args.tooltipText.Add(New KeyValuePair(Of String, String)(args.url.Key, ""))
        Return True
    End Function
#End Region
End Class