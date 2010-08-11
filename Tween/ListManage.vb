Public Class ListManage
    Private tw As Twitter

    Public Sub New(ByVal tw As Twitter)
        Me.InitializeComponent()

        Me.tw = tw
    End Sub


    Private Sub ListManage_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        For Each listItem As ListElement In TabInformations.GetInstance().SubscribableLists.FindAll(Function(i) i.Username = Me.tw.Username)
            Me.ListsList.Items.Add(listItem)
        Next
    End Sub

    Private Sub ListsList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListsList.SelectedIndexChanged
        If Me.ListsList.SelectedItem Is Nothing Then Return

        Dim list As ListElement = CType(Me.ListsList.SelectedItem, ListElement)
        Me.UsernameTextBox.Text = list.Username
        Me.NameTextBox.Text = list.Name
        Me.PublicRadioButton.Checked = list.IsPublic
        Me.PrivateRadioButton.Checked = Not list.IsPublic
        Me.MemberCountTextBox.Text = list.MemberCount.ToString()
        Me.SubscriberCountTextBox.Text = list.SubscriberCount.ToString()
        Me.DescriptionText.Text = list.Description

        Me.UserList.Items.Clear()
        For Each user As UserInfo In list.Members
            Me.UserList.Items.Add(user)
        Next
    End Sub

    Private Sub EditCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditCheckBox.CheckedChanged
        Me.NameTextBox.ReadOnly = Not Me.EditCheckBox.Checked
        Me.PublicRadioButton.Enabled = Me.EditCheckBox.Checked
        Me.PrivateRadioButton.Enabled = Me.EditCheckBox.Checked
        Me.DescriptionText.ReadOnly = Not Me.EditCheckBox.Checked
        Me.ListsList.Enabled = Not Me.EditCheckBox.Checked

        Me.OKEditButton.Enabled = Me.EditCheckBox.Enabled
        Me.CancelEditButton.Enabled = Me.EditCheckBox.Enabled
        Me.EditCheckBox.AutoCheck = Not Me.EditCheckBox.Checked
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKEditButton.Click
        If Me.ListsList.SelectedItem Is Nothing Then Return
        Dim listItem As ListElement = DirectCast(Me.ListsList.SelectedItem, ListElement)

        If Me.NameTextBox.Text = "" Then
            MessageBox.Show("リスト名を入力して下さい。")
            Return
        End If

        listItem.Name = Me.NameTextBox.Text
        listItem.IsPublic = Me.PublicRadioButton.Checked
        listItem.Description = Me.DescriptionText.Text

        Dim rslt As String = listItem.Refresh()

        If rslt <> "" Then
            MessageBox.Show("通信エラー (" + rslt + ")")
            'Me.ListsList.Items.Clear()
            'Me.ListManage_Load(Nothing, EventArgs.Empty)
            Return
        End If

        Me.ListsList.Items.Clear()
        Me.ListManage_Load(Nothing, EventArgs.Empty)

        Me.EditCheckBox.AutoCheck = True
        Me.EditCheckBox.Checked = False

        Me.OKEditButton.Enabled = False
        Me.CancelEditButton.Enabled = False

        Me.ListsList.Refresh()
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelEditButton.Click
        Me.EditCheckBox.AutoCheck = True
        Me.EditCheckBox.Checked = False

        Me.OKEditButton.Enabled = False
        Me.CancelEditButton.Enabled = False

        For i As Integer = 0 To Me.ListsList.Items.Count - 1
            If TypeOf Me.ListsList.Items(i) Is NewListElement Then
                Me.ListsList.Items.RemoveAt(i)
            End If
        Next

        Me.ListsList_SelectedIndexChanged(Me.ListsList, EventArgs.Empty)
    End Sub

    Private Sub RefreshUsersButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshUsersButton.Click
        If Me.ListsList.SelectedItem Is Nothing Then Return
        CType(Me.ListsList.SelectedItem, ListElement).RefreshMembers()
        Me.ListsList_SelectedIndexChanged(Me.ListsList, EventArgs.Empty)
    End Sub

    Private Sub DeleteUserButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteUserButton.Click
        If Me.ListsList.SelectedItem Is Nothing OrElse Me.UserList.SelectedItem Is Nothing Then
            Exit Sub
        End If

        Dim list As ListElement = CType(Me.ListsList.SelectedItem, ListElement)
        Dim user As UserInfo = CType(Me.UserList.SelectedItem, UserInfo)
        If MessageBox.Show(list.Name + "から" + user.ScreenName + "を削除します。", "Tween", MessageBoxButtons.OKCancel) = DialogResult.OK Then
            Dim rslt As String = Me.tw.RemoveUserToList(list.Id.ToString(), user.Id.ToString())

            If rslt <> "" Then
                MessageBox.Show("通信エラー (" + rslt + ")")
            End If

            Me.RefreshUsersButton.PerformClick()

        End If
    End Sub

    Private Sub DeleteListButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteListButton.Click
        If Me.ListsList.SelectedItem Is Nothing Then Return
        Dim list As ListElement = CType(Me.ListsList.SelectedItem, ListElement)

        If MessageBox.Show(list.Name + "リストを削除します。", "Tween", MessageBoxButtons.OKCancel) = DialogResult.OK Then
            Dim rslt As String = ""

            rslt = Me.tw.DeleteList(list.Id.ToString())

            If rslt <> "" Then
                MessageBox.Show("通信エラー (" + rslt + ")")
                Return
            End If

            rslt = Me.tw.GetListsApi()

            If rslt <> "" Then
                MessageBox.Show("通信エラー (" + rslt + ")")
                Return
            End If

            Me.ListsList.Items.Clear()
            Me.ListManage_Load(Me, EventArgs.Empty)
        End If
    End Sub

    Private Sub AddListButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddListButton.Click
        Dim newList As New NewListElement(Me.tw)
        Me.ListsList.Items.Add(newList)
        Me.ListsList.SelectedItem = newList
        Me.EditCheckBox.Checked = True
        Me.EditCheckBox_CheckedChanged(Me.EditCheckBox, EventArgs.Empty)
    End Sub

    Private Class NewListElement
        Inherits ListElement

        Private _isCreated As Boolean = False

        Public Sub New(ByVal tw As Twitter)
            Me._tw = tw
        End Sub

        Public Overrides Function Refresh() As String
            If Me.IsCreated Then
                Return MyBase.Refresh()
            Else
                Dim rslt As String = Me._tw.CreateListApi(Me.Name, Not Me.IsPublic, Me.Description)
                Me._isCreated = (rslt = "")
                Return rslt
            End If
        End Function

        Public ReadOnly Property IsCreated As Boolean
            Get
                Return Me._isCreated
            End Get
        End Property

        Public Overrides Function ToString() As String
            If IsCreated Then
                Return MyBase.ToString()
            Else
                Return "NewList"
            End If
        End Function
    End Class
End Class