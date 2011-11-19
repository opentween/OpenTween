Imports System.Text.RegularExpressions

Public Class AuthBrowser
    Public Property UrlString As String
    Public Property PinString As String
    Public Property Auth As Boolean = True

    Private SecurityManager As InternetSecurityManager


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
        Me.SecurityManager = New InternetSecurityManager(Me.AuthWebBrowser)

        Me.AuthWebBrowser.Navigate(New Uri(UrlString))
        If Not Auth Then
            Me.Label1.Visible = False
            Me.PinText.Visible = False
        End If
    End Sub

    Private Sub AuthWebBrowser_Navigating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs) Handles AuthWebBrowser.Navigating
        Me.AddressLabel.Text = e.Url.OriginalString
    End Sub

    Private Sub NextButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NextButton.Click
        PinString = PinText.Text.Trim
        Me.DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        PinString = ""
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub
End Class