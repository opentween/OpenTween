Imports System.Windows.Forms
Imports System.IO

Namespace TweenCustomControl
    Public Class AdsBrowser
        Inherits WebBrowser

        Private adsPath As String

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
            Me.Navigate(adsPath)

        End Sub

        Private Sub AdsBrowser_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            File.Delete(adsPath)
        End Sub
    End Class
End Namespace