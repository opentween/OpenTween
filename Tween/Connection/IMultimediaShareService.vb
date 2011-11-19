
Public Interface IMultimediaShareService
    Function Upload(ByRef filePath As String,
                    ByRef message As String,
                    ByVal reply_to As Long) As String
    Function CheckValidExtension(ByVal ext As String) As Boolean
    Function GetFileOpenDialogFilter() As String
    Function GetFileType(ByVal ext As String) As UploadFileType
    Function IsSupportedFileType(ByVal type As UploadFileType) As Boolean
    Function CheckValidFilesize(ByVal ext As String, ByVal fileSize As Long) As Boolean
    Function Configuration(ByVal key As String, ByVal value As Object) As Boolean
End Interface
