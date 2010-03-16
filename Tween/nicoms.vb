Public Class nicoms
    Private Shared _nicovideo() As String = { _
        "www.nicovideo.jp/watch/", _
        "live.nicovideo.jp/watch/", _
        "live.nicolive.jp/gate/", _
        "co.nicovideo.jp/community/", _
        "com.nicovideo.jp/community/", _
        "ch.nicovideo.jp/channel/", _
        "nicovideo.jp/watch/", _
        "seiga.nicovideo.jp/bbs/", _
        "www.niconicommons.jp/material/", _
        "niconicommons.jp/material/"}

    Public Shared Function Shorten(ByVal url As String) As String
        '®Œ`ihttp(s)://‚ðíœj
        If url.Length > 7 AndAlso url.Length < 128 AndAlso url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) Then
            url = url.Substring(7)
        Else
            Return url
        End If

        For Each nv As String In _nicovideo
            If url.StartsWith(nv) Then Return String.Format("{0}{1}", "http://nico.ms/", url.Substring(nv.Length))
        Next

        Dim i As Integer
        i = url.IndexOf("nicovideo.jp/user/", StringComparison.OrdinalIgnoreCase)
        If i = 0 OrElse i = 4 Then Return String.Format("{0}{1}", "http://nico.ms/", url.Substring(13 + i))

        i = url.IndexOf("nicovideo.jp/mylist/", StringComparison.OrdinalIgnoreCase)
        If i = 0 OrElse i = 4 Then Return String.Format("{0}{1}", "http://nico.ms/", url.Substring(13 + i))

        i = url.IndexOf("seiga.nicovideo.jp/watch/", StringComparison.OrdinalIgnoreCase)
        If i = 0 Then Return String.Format("{0}{1}", "http://nico.ms/", url.Substring(25))

        return url
    End Function
End Class
