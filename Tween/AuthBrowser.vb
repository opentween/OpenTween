Imports System.Text.RegularExpressions

Public Class AuthBrowser
    Public Property UrlString As String
    Public Property PinString As String

    Private Sub AuthWebBrowser_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles AuthWebBrowser.DocumentCompleted
        If Me.AuthWebBrowser.Url.OriginalString = "https://api.twitter.com/oauth/authorize" Then
            Dim rg As Regex = New Regex("<code>(\d+)</code>")
            Dim m As Match = rg.Match(Me.AuthWebBrowser.DocumentText)
            If m.Success Then
                PinString = m.Result("${1}")
                PinText.Text = m.Result("${1}")
                PinText.Focus()
            End If
        End If
    End Sub

    Private Sub AuthBrowser_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.AuthWebBrowser.Navigate(New Uri(UrlString))
    End Sub

    Private Sub AuthWebBrowser_Navigating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs) Handles AuthWebBrowser.Navigating
        Me.AddressLabel.Text = e.Url.OriginalString
    End Sub

    Private Sub NextButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NextButton.Click
        PinString = PinText.Text.Trim
        Me.DialogResult = Windows.Forms.DialogResult.OK
    End Sub
End Class