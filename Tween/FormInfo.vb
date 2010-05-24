Imports System.ComponentModel

Public Class FormInfo

    Private _msg As String
    Private _arg As Object = Nothing
    Private _ret As Object = Nothing

    Public Servicer As New BackgroundWorker

    Private Sub LabelInformation_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LabelInformation.TextChanged
        LabelInformation.Left = (Me.ClientSize.Width - LabelInformation.Size.Width) \ 2
        LabelInformation.Refresh()
    End Sub

    Public Property InfoMessage() As String
        Get
            Return _msg
        End Get
        Set(ByVal value As String)
            _msg = value
            LabelInformation.Text = _msg
        End Set
    End Property

    Public Property Argument() As Object
        Get
            Return _arg
        End Get
        Set(ByVal value As Object)
            _arg = value
        End Set
    End Property

    Public ReadOnly Property ReturnValue() As Object
        Get
            Return _ret
        End Get
    End Property

    Private Sub ReturnValueCatcher(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
        _ret = e.Result
    End Sub

    Private Sub FormInfo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AddHandler Servicer.RunWorkerCompleted, AddressOf ReturnValueCatcher
    End Sub

    Private Sub FormInfo_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Servicer.RunWorkerAsync(_arg)
        While Servicer.IsBusy
            My.Application.DoEvents()
        End While
        Me.Close()
    End Sub

End Class


