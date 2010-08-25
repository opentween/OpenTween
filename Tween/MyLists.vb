Public Class MyLists
    Private contextUserName As String
    Private _tw As Twitter

    Public Sub New(ByVal userName As String, ByVal tw As Twitter)
        Me.InitializeComponent()

        Me.contextUserName = userName
        Me._tw = tw

        Me.Text = Me.contextUserName + My.Resources.MyLists1
    End Sub

    Private Sub MyLists_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RemoveHandler Me.ListsCheckedListBox.ItemCheck, AddressOf Me.ListsCheckedListBox_ItemCheck

        Me.ListsCheckedListBox.Items.AddRange(TabInformations.GetInstance.SubscribableLists.FindAll(Function(item) item.Username = Me._tw.Username).ToArray())

        For i As Integer = 0 To Me.ListsCheckedListBox.Items.Count - 1
            Dim listItem As ListElement = CType(Me.ListsCheckedListBox.Items(i), ListElement)

            Dim listPost As New List(Of PostClass)()
            Dim otherPost As New List(Of PostClass)()

            For Each tab As TabClass In TabInformations.GetInstance().Tabs.Values
                If tab.TabType = TabUsageType.Lists Then
                    If listItem.Id = tab.ListInfo.Id Then
                        listPost.AddRange(tab.Posts.Values)
                    Else
                        otherPost.AddRange(tab.Posts.Values)
                    End If
                End If
            Next

            'リストが空の場合は推定不能
            If listPost.Count = 0 Then
                Me.ListsCheckedListBox.SetItemCheckState(i, CheckState.Indeterminate)
                Continue For
            End If

            'リストに該当ユーザーのポストが含まれていれば、リストにユーザーが含まれているとする。
            If listPost.Exists(Function(item) item.Name = contextUserName) Then
                Me.ListsCheckedListBox.SetItemChecked(i, True)
                Continue For
            End If

            Dim listPostUserIDs As New List(Of Long)()
            Dim listPostUserNames As New List(Of String)()
            Dim listOlderPostCreatedAt As DateTime = DateTime.MaxValue
            Dim listNewistPostCreatedAt As DateTime = DateTime.MinValue

            For Each post As PostClass In listPost
                If post.Uid > 0 AndAlso Not listPostUserIDs.Contains(post.Uid) Then
                    listPostUserIDs.Add(post.Uid)
                End If
                If post.Name IsNot Nothing AndAlso Not listPostUserNames.Contains(post.Name) Then
                    listPostUserNames.Add(post.Name)
                End If
                If post.PDate < listOlderPostCreatedAt Then
                    listOlderPostCreatedAt = post.PDate
                End If
                If post.PDate > listNewistPostCreatedAt Then
                    listNewistPostCreatedAt = post.PDate
                End If
            Next

            'リスト中のユーザーの人数がlistItem.MemberCount以上で、かつ該当のユーザーが含まれていなければ、リストにユーザーは含まれていないとする。
            If listItem.MemberCount > 0 AndAlso listItem.MemberCount <= listPostUserIDs.Count AndAlso (Not listPostUserNames.Contains(contextUserName)) Then
                Me.ListsCheckedListBox.SetItemChecked(i, False)
                Continue For
            End If

            otherPost.AddRange(TabInformations.GetInstance().Posts().Values)

            'リストに該当ユーザーのポストが含まれていないのにリスト以外で取得したポストの中にリストに含まれるべきポストがある場合は、リストにユーザーは含まれていないとする。
            If otherPost.Exists(Function(item) (item.Name = Me.contextUserName) AndAlso (item.PDate > listOlderPostCreatedAt) AndAlso (item.PDate < listNewistPostCreatedAt) AndAlso ((Not item.IsReply) OrElse listPostUserNames.Contains(item.InReplyToUser))) Then
                Me.ListsCheckedListBox.SetItemChecked(i, False)
                Continue For
            End If

            Me.ListsCheckedListBox.SetItemCheckState(i, CheckState.Indeterminate)
        Next

        AddHandler Me.ListsCheckedListBox.ItemCheck, AddressOf Me.ListsCheckedListBox_ItemCheck
    End Sub

    Private Sub ListRefreshButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListRefreshButton.Click
        Dim rslt As String = Me._tw.GetListsApi()
        If rslt <> "" Then
            MessageBox.Show(String.Format(My.Resources.ListsDeleteFailed, rslt))
        Else
            Me.ListsCheckedListBox.Items.Clear()
            Me.MyLists_Load(Me, EventArgs.Empty)
        End If
    End Sub

    Private Sub ListsCheckedListBox_ItemCheck(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles ListsCheckedListBox.ItemCheck
        Select Case e.CurrentValue
            Case CheckState.Indeterminate
                Dim listItem As ListElement = CType(Me.ListsCheckedListBox.Items(e.Index), ListElement)

                Dim ret As Boolean
                Dim rslt As String = Me._tw.ContainsUserAtList(listItem.Id.ToString(), contextUserName.ToString(), ret)
                If rslt <> "" Then
                    MessageBox.Show(String.Format(My.Resources.ListManageOKButton2, rslt))
                    e.NewValue = CheckState.Indeterminate
                Else
                    If ret Then
                        e.NewValue = CheckState.Checked
                    Else
                        e.NewValue = CheckState.Unchecked
                    End If
                End If
            Case CheckState.Unchecked
                Dim list As ListElement = CType(Me.ListsCheckedListBox.Items(e.Index), ListElement)
                Dim rslt As String = Me._tw.AddUserToList(list.Id.ToString(), Me.contextUserName.ToString())
                If rslt <> "" Then
                    MessageBox.Show(String.Format(My.Resources.ListManageOKButton2, rslt))
                    e.NewValue = CheckState.Indeterminate
                End If
            Case CheckState.Checked
                Dim list As ListElement = CType(Me.ListsCheckedListBox.Items(e.Index), ListElement)
                Dim rslt As String = Me._tw.RemoveUserToList(list.Id.ToString(), Me.contextUserName.ToString())
                If rslt <> "" Then
                    MessageBox.Show(String.Format(My.Resources.ListManageOKButton2, rslt))
                    e.NewValue = CheckState.Indeterminate
                End If
        End Select
    End Sub

    Private Sub ContextMenuStrip1_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening
        e.Cancel = Me.ListsCheckedListBox.SelectedItem Is Nothing
    End Sub

    Private Sub 追加AToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 追加AToolStripMenuItem.Click
        RemoveHandler Me.ListsCheckedListBox.ItemCheck, AddressOf Me.ListsCheckedListBox_ItemCheck
        Me.ListsCheckedListBox.SetItemCheckState(Me.ListsCheckedListBox.SelectedIndex, CheckState.Unchecked)
        AddHandler Me.ListsCheckedListBox.ItemCheck, AddressOf Me.ListsCheckedListBox_ItemCheck
        Me.ListsCheckedListBox.SetItemCheckState(Me.ListsCheckedListBox.SelectedIndex, CheckState.Checked)
    End Sub

    Private Sub 削除DToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 削除DToolStripMenuItem.Click
        RemoveHandler Me.ListsCheckedListBox.ItemCheck, AddressOf Me.ListsCheckedListBox_ItemCheck
        Me.ListsCheckedListBox.SetItemCheckState(Me.ListsCheckedListBox.SelectedIndex, CheckState.Checked)
        AddHandler Me.ListsCheckedListBox.ItemCheck, AddressOf Me.ListsCheckedListBox_ItemCheck
        Me.ListsCheckedListBox.SetItemCheckState(Me.ListsCheckedListBox.SelectedIndex, CheckState.Unchecked)
    End Sub

    Private Sub 更新RToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 更新RToolStripMenuItem.Click
        RemoveHandler Me.ListsCheckedListBox.ItemCheck, AddressOf Me.ListsCheckedListBox_ItemCheck
        Me.ListsCheckedListBox.SetItemCheckState(Me.ListsCheckedListBox.SelectedIndex, CheckState.Indeterminate)
        AddHandler Me.ListsCheckedListBox.ItemCheck, AddressOf Me.ListsCheckedListBox_ItemCheck
        Me.ListsCheckedListBox.SetItemCheckState(Me.ListsCheckedListBox.SelectedIndex, CheckState.Checked)
    End Sub

    Private Sub ListsCheckedListBox_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListsCheckedListBox.MouseDown
        Select Case e.Button
            Case MouseButtons.Left
                '項目が無い部分をクリックしても、選択されている項目のチェック状態が変更されてしまうので、その対策
                For index As Integer = 0 To Me.ListsCheckedListBox.Items.Count - 1
                    If Me.ListsCheckedListBox.GetItemRectangle(index).Contains(e.Location) Then
                        Return
                    End If
                Next
                Me.ListsCheckedListBox.SelectedItem = Nothing
            Case MouseButtons.Right
                'コンテキストメニューの項目実行時にSelectedItemプロパティを利用出来るように
                For index As Integer = 0 To Me.ListsCheckedListBox.Items.Count - 1
                    If Me.ListsCheckedListBox.GetItemRectangle(index).Contains(e.Location) Then
                        Me.ListsCheckedListBox.SetSelected(index, True)
                        Return
                    End If
                Next
                Me.ListsCheckedListBox.SelectedItem = Nothing
        End Select
    End Sub

    Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseButton.Click
        Me.Close()
    End Sub
End Class