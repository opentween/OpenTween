Imports System.Net
Imports System.IO

Public Interface IHttpConnection

    Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String)) As HttpStatusCode

    Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByVal binary As List(Of KeyValuePair(Of String, FileInfo)), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String)) As HttpStatusCode

    Function Authenticate(ByVal url As Uri, ByVal username As String, ByVal password As String) As HttpStatusCode

    ReadOnly Property AuthUsername() As String
End Interface
