Imports System.IO
Imports System.Net

Public Interface IMultimediaShareService
    Function Upload(ByRef filePath As String,
                   ByRef message As String) As String
    Function CheckValidExtension(ByVal ext As String) As Boolean
    Function GetFileOpenDialogFilter() As String
    Function GetFileType(ByVal ext As String) As UploadFileType
    Function IsSupportedFileType(ByVal type As UploadFileType) As Boolean
    Function CheckValidFilesize(ByVal ext As String, ByVal fileSize As Long) As Boolean
End Interface
