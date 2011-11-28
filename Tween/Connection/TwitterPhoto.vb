Imports System.IO
Imports System.Net
Imports System.Xml

Public Class TwitterPhoto
    Implements IMultimediaShareService

    Private pictureExt() As String = {".jpg", _
                                ".jpeg", _
                                ".gif", _
                                ".png"}

    Private Const MaxfilesizeDefault As Int64 = 3145728

    ' help/configurationにより取得されコンストラクタへ渡される
    Private _MaxFileSize As Int64 = 3145728

    Private tw As Twitter

    Public Function CheckValidExtension(ext As String) As Boolean Implements IMultimediaShareService.CheckValidExtension
        If Array.IndexOf(pictureExt, ext.ToLower) > -1 Then
            Return True
        End If
        Return False
    End Function

    Public Function CheckValidFilesize(ext As String, fileSize As Long) As Boolean Implements IMultimediaShareService.CheckValidFilesize
        If Me.CheckValidExtension(ext) Then
            Return fileSize <= _MaxFileSize
        End If
        Return False
    End Function

    Public Function Configuration(ByVal key As String, ByVal value As Object) As Boolean Implements IMultimediaShareService.Configuration
        If key = "MaxUploadFilesize" Then
            Dim val As Int64
            Try
                val = Convert.ToInt64(value)
                If val > 0 Then
                    _MaxFileSize = val
                Else
                    _MaxFileSize = MaxfilesizeDefault
                End If
            Catch ex As Exception
                _MaxFileSize = MaxfilesizeDefault
                Return False    'error
            End Try
            Return True          ' 正常に設定終了
        End If
        Return True              ' 設定項目がない場合はとりあえずエラー扱いにしない
    End Function

    Public Function GetFileOpenDialogFilter() As String Implements IMultimediaShareService.GetFileOpenDialogFilter
        Return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png"
    End Function

    Public Function GetFileType(ext As String) As MyCommon.UploadFileType Implements IMultimediaShareService.GetFileType
        If Me.CheckValidExtension(ext) Then
            Return UploadFileType.Picture
        End If
        Return UploadFileType.Invalid
    End Function

    Public Function IsSupportedFileType(type As MyCommon.UploadFileType) As Boolean Implements IMultimediaShareService.IsSupportedFileType
        Return type.Equals(UploadFileType.Picture)
    End Function

    Public Function Upload(ByRef filePath As String,
                           ByRef message As String,
                           ByVal reply_to As Long) As String Implements IMultimediaShareService.Upload
        If String.IsNullOrEmpty(filePath) Then Return "Err:File isn't specified."
        If String.IsNullOrEmpty(message) Then message = ""
        Dim mediaFile As FileInfo
        Try
            mediaFile = New FileInfo(filePath)
        Catch ex As NotSupportedException
            Return "Err:" + ex.Message
        End Try
        If Not mediaFile.Exists Then Return "Err:File isn't exists."
        If IsAnimatedGif(filePath) Then Return "Err:Don't support animatedGIF."

        Return tw.PostStatusWithMedia(message, reply_to, mediaFile)
    End Function

    Public Sub New(ByVal twitter As Twitter)
        tw = twitter
    End Sub
End Class
