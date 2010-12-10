Imports System.Net
Imports System.IO

Public Interface IHttpConnection

    Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As Stream) As HttpStatusCode

    Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String), _
            ByVal callback As CallbackDelegate) As HttpStatusCode

    Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByVal binary As List(Of KeyValuePair(Of String, FileInfo)), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String), _
            ByVal callback As CallbackDelegate) As HttpStatusCode

    Sub RequestAbort()

    Function Authenticate(ByVal url As Uri, ByVal username As String, ByVal password As String, ByRef content As String) As HttpStatusCode

ReadOnly Property AuthUsername() As String
    ''' <summary>
    ''' APIメソッドの処理が終了し呼び出し元へ戻る直前に呼ばれるデリゲート
    ''' </summary>
    ''' <param name="sender">メソッド名</param>
    ''' <param name="code">APIメソッドの返したHTTPステータスコード</param>
    ''' <param name="content">APIメソッドの処理結果</param>
    ''' <remarks>contentはNothingになることがあるのでチェックを必ず行うこと</remarks>
    Delegate Sub CallbackDelegate(ByVal sender As Object, ByRef code As HttpStatusCode, ByRef content As String)
End Interface
