Public Class MyLists
    Private contextUserName As String
    Private _tw As Twitter

    Public Sub New(ByVal userName As String, ByVal tw As Twitter)
        Me.InitializeComponent()

        Me.contextUserName = userName
        Me._tw = tw

        Me.Text = Me.contextUserName + "を含むリストの管理"
    End Sub

    Private Sub MyLists_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.ListsCheckedListBox.Items.AddRange(TabInformations.GetInstance.SubscribableLists.FindAll(Function(item) item.Username = Me._tw.Username).ToArray())

        For i As Integer = 0 To Me.ListsCheckedListBox.Items.Count - 1
            Dim listItem As ListElement = CType(Me.ListsCheckedListBox.Items(i), ListElement)

            Dim listPost As New List(Of PostClass)()
            Dim otherPost As New List(Of PostClass)()

            Dim listPostUserIDs As New List(Of Long)()
            Dim listPostUserNames As New List(Of String)()
            Dim listOlderPostCreatedAt As DateTime = DateTime.Now

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
            Next

            'リスト中のユーザーの人数がlistItem.MemberCount以上で、かつ該当のユーザーが含まれていなければ、リストにユーザーは含まれていないとする。
            If listItem.MemberCount > 0 AndAlso listItem.MemberCount <= listPostUserIDs.Count AndAlso (Not listPostUserNames.Contains(contextUserName)) Then
                Me.ListsCheckedListBox.SetItemChecked(i, False)
                Continue For
            End If

            otherPost.AddRange(TabInformations.GetInstance().Posts().Values)

            'リストに該当ユーザーのポストが含まれていないのにリスト以外で取得したポストの中にリストに含まれるべきポストがある場合は、リストにユーザーは含まれていないとする。
            If otherPost.Exists(Function(item) (item.Name = Me.contextUserName) AndAlso (item.PDate > listOlderPostCreatedAt) AndAlso ((Not item.IsReply) OrElse listPostUserNames.Contains(item.InReplyToUser))) Then
                Me.ListsCheckedListBox.SetItemChecked(i, False)
                Continue For
            End If

            Me.ListsCheckedListBox.SetItemCheckState(i, CheckState.Indeterminate)
        Next
    End Sub

    Private Sub ListsCheckedListBox_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListsCheckedListBox.MouseClick
        If e.X >= 16 Then Return 'チェックボッス部分のみに反応させる。

        For i As Integer = 0 To Me.ListsCheckedListBox.Items.Count - 1
            If Me.ListsCheckedListBox.GetItemRectangle(i).Contains(e.Location) Then
                If Me.ListsCheckedListBox.GetItemCheckState(i) = CheckState.Indeterminate Then
                    Dim listItem As ListElement = CType(Me.ListsCheckedListBox.Items(i), ListElement)

                    Dim ret As Boolean
                    Dim rslt As String = Me._tw.ContainsUserAtList(listItem.Id.ToString(), contextUserName.ToString(), ret)
                    If rslt <> "" Then
                        MessageBox.Show("通信エラー (" + rslt + ")")
                    Else
                        Me.ListsCheckedListBox.SetItemChecked(i, ret)
                    End If
                Else
                    If Me.ListsCheckedListBox.GetItemCheckState(i) = CheckState.Unchecked Then
                        Dim list As ListElement = CType(Me.ListsCheckedListBox.Items(i), ListElement)
                        Dim rslt As String = Me._tw.AddUserToList(list.Id.ToString(), Me.contextUserName.ToString())
                        If rslt <> "" Then
                            MessageBox.Show("通信エラー (" + rslt + ")")
                        Else
                            Me.ListsCheckedListBox.SetItemChecked(i, True)
                        End If
                    ElseIf Me.ListsCheckedListBox.GetItemCheckState(i) = CheckState.Checked Then
                        Dim list As ListElement = CType(Me.ListsCheckedListBox.Items(i), ListElement)
                        Dim rslt As String = Me._tw.RemoveUserToList(list.Id.ToString(), Me.contextUserName.ToString())
                        If rslt <> "" Then
                            MessageBox.Show("通信エラー (" + rslt + ")")
                        Else
                            Me.ListsCheckedListBox.SetItemChecked(i, False)
                        End If
                    End If
                End If

                Exit For
            End If
        Next
    End Sub
End Class