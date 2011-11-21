Imports System.Windows.Forms
Imports System.IO

Namespace TweenCustomControl
    Public Class AdsBrowser
        Inherits WebBrowser

        Private adsPath As String
        Private WithEvents refreshTimer As System.Timers.Timer

        Public Sub New()
            MyBase.New()

            adsPath = Path.Combine(Path.GetTempPath, Path.GetRandomFileName)
            File.WriteAllText(adsPath, My.Resources.ads)

            Me.Size = New Size(728 + 5, 90)
            Me.ScrollBarsEnabled = False
            Me.AllowWebBrowserDrop = False
            Me.IsWebBrowserContextMenuEnabled = False
            Me.ScriptErrorsSuppressed = True
            Me.TabStop = False
            Me.WebBrowserShortcutsEnabled = False
            Me.Dock = DockStyle.Fill
            Me.Visible = False
            Me.Navigate(adsPath)
            Me.Visible = True

            Me.refreshTimer = New System.Timers.Timer(45 * 1000)
            Me.refreshTimer.AutoReset = True
            Me.refreshTimer.SynchronizingObject = Me
            Me.refreshTimer.Enabled = True
        End Sub


        Private Sub AdsBrowser_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Me.refreshTimer.Dispose()
            File.Delete(adsPath)
        End Sub

        Private Sub refreshTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles refreshTimer.Elapsed
            Me.Visible = False
            Me.Refresh()
            Me.Visible = True
        End Sub

    End Class
End Namespace