Imports System.Net

Public Interface IHttpConnection

    Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String)) As HttpStatusCode

    Function Authenticate(ByVal url As Uri, ByVal username As String, ByVal password As String) As Boolean

    ReadOnly Property AuthUsername() As String
End Interface
