Namespace My

    ' 次のイベントは MyApplication に対して利用できます:
    ' 
    ' Startup: アプリケーションが開始されたとき、スタートアップ フォームが作成される前に発生します。
    ' Shutdown: アプリケーション フォームがすべて閉じられた後に発生します。このイベントは、通常の終了以外の方法でアプリケーションが終了されたときには発生しません。
    ' UnhandledException: ハンドルされていない例外がアプリケーションで発生したときに発生するイベントです。
    ' StartupNextInstance: 単一インスタンス アプリケーションが起動され、それが既にアクティブであるときに発生します。 
    ' NetworkAvailabilityChanged: ネットワーク接続が接続されたとき、または切断されたときに発生します。
    Partial Friend Class MyApplication

        Private Sub MyApplication_UnhandledException(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException

            My.Application.Log.DefaultFileLogWriter.Location = Logging.LogFileLocation.ExecutableDirectory
            My.Application.Log.DefaultFileLogWriter.MaxFileSize = 16384
            My.Application.Log.DefaultFileLogWriter.AutoFlush = True
            My.Application.Log.DefaultFileLogWriter.Append = False
            My.Application.Log.WriteException(e.Exception, TraceEventType.Error, DateAndTime.Now.ToString())
            MsgBox(My.Resources.ExceptionMessage & vbCrLf & e.Exception.Message, , My.Resources.ExceptionTitle)

        End Sub
    End Class

End Namespace

