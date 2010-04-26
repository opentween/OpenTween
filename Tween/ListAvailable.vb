Imports System.Windows.Forms

Public Class ListAvailable

    Private _selectedList As ListElement = Nothing
    Public ReadOnly Property SelectedList() As ListElement
        Get
            Return _selectedList
        End Get
    End Property

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        If Me.ListsList.SelectedIndex > -1 Then
            _selectedList = DirectCast(Me.ListsList.SelectedItem, ListElement)
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End If
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        _selectedList = Nothing
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub ListAvailable_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Me.ListsList.Items.AddRange(TabInformations.GetInstance.SubscribableLists.ToArray)
        If Me.ListsList.Items.Count > 0 Then
            Me.ListsList.SelectedIndex = 0
        End If
    End Sub

    Private Sub ListsList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListsList.SelectedIndexChanged
        Dim lst As ListElement
        If Me.ListsList.SelectedIndex > -1 Then
            lst = DirectCast(Me.ListsList.SelectedItem, ListElement)
        Else
            lst = Nothing
        End If
        If lst Is Nothing Then
            Me.UsernameLabel.Text = ""
            Me.NameLabel.Text = ""
            Me.StatusLabel.Text = ""
            Me.MemberCountLabel.Text = "0"
            Me.SubscriberCountLabel.Text = "0"
            Me.DescriptionText.Text = ""
        Else
            Me.UsernameLabel.Text = lst.Username + " / " + lst.Nickname
            Me.NameLabel.Text = lst.Name
            If lst.IsPublic Then
                Me.StatusLabel.Text = "Public"
            Else
                Me.StatusLabel.Text = "Private"
            End If
            Me.MemberCountLabel.Text = lst.MemberCount.ToString("#,##0")
            Me.SubscriberCountLabel.Text = lst.SubscriberCount.ToString("#,##0")
            Me.DescriptionText.Text = lst.Description
        End If
    End Sub

    Private Sub RefreshButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshButton.Click
        Dim rslt As String = DirectCast(Me.Owner, TweenMain).TwitterInstance.GetListsApi()
        If rslt <> "" Then
            MessageBox.Show(rslt, "Failed", MessageBoxButtons.OK)
            Exit Sub
        End If
        Me.ListsList.Items.Clear()
        Me.ListsList.Items.AddRange(TabInformations.GetInstance.SubscribableLists.ToArray)
        If Me.ListsList.Items.Count > 0 Then
            Me.ListsList.SelectedIndex = 0
        End If
    End Sub
End Class
