Public Class MyLists
    Private contextUserName As String
    Private _tw As Twitter

    Public Sub New(ByVal userName As String, ByVal tw As Twitter)
        Me.InitializeComponent()

        Me.contextUserName = userName
        Me._tw = tw
    End Sub

    Private Sub MyLists_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.ListsCheckedListBox.Items.AddRange(TabInformations.GetInstance.SubscribableLists.FindAll(Function(item) item.Username = Me._tw.Username).ToArray())

        For i As Integer = 0 To Me.ListsCheckedListBox.Items.Count - 1
            Dim listItem As ListElement = CType(Me.ListsCheckedListBox.Items(i), ListElement)
            Dim myTabsPost As New List(Of PostClass)()
            Dim otherTabsPost As New List(Of PostClass)()
            Dim myTabsOlderPostCreatedAt As DateTime = DateTime.Now
            Dim myTabsUserIDs As New List(Of Long)()
            Dim myTabsUserNames As New List(Of String)()

            For Each tab As TabClass In TabInformations.GetInstance().Tabs.Values
                If tab.TabType = TabUsageType.Lists AndAlso listItem.Id = tab.ListInfo.Id Then
                    myTabsPost.AddRange(tab.Posts.Values)
                    'myTabsPost.AddRange(tab.GetTemporaryPosts())
                ElseIf tab.TabType = TabUsageType.Home OrElse tab.TabType = TabUsageType.Mentions Then
                    otherTabsPost.AddRange(TabInformations.GetInstance().Posts().Values)
                    'otherTabsPost.AddRange(tab.GetTemporaryPosts())
                End If
            Next

            For Each post As PostClass In myTabsPost
                If Not myTabsUserIDs.Contains(post.Uid) Then
                    myTabsUserIDs.Add(post.Uid)
                End If
                If Not myTabsUserNames.Contains(post.Name) Then
                    myTabsUserNames.Add(post.Name)
                End If
                If post.PDate < myTabsOlderPostCreatedAt Then
                    myTabsOlderPostCreatedAt = post.PDate
                End If
            Next

            'リストが空の場合は推定不能なのでAPIから取得する
            If myTabsPost.Count = 0 Then
                Dim ret_ As Boolean
                Dim rslt_ As String = Me._tw.ContainsUserAtList(listItem.Id.ToString(), contextUserName.ToString(), ret_)
                If rslt_ <> "" Then
                    MessageBox.Show("通信エラー (" + rslt_ + ")")
                End If
                Continue For
            End If

            'リストに該当ユーザーのポストが含まれていれば、リストにユーザーが含まれているとする。
            If myTabsPost.Exists(Function(item) item.Name = contextUserName) Then
                Me.ListsCheckedListBox.SetItemChecked(Me.ListsCheckedListBox.Items.IndexOf(listItem), True)
                Continue For
            End If

            'リスト中のユーザーの人数がlistItem.MemberCount以上で、かつ該当のユーザーが含まれていなければ、リストにユーザーは含まれていないとする。
            If listItem.MemberCount <= myTabsUserIDs.Count AndAlso myTabsUserNames.Contains(contextUserName) Then
                Me.ListsCheckedListBox.SetItemChecked(Me.ListsCheckedListBox.Items.IndexOf(listItem), False)
                Continue For
            End If

            'リストに該当ユーザーのポストが含まれていないのにリスト以外で取得したポストの中にリストに含まれるべきポストがある場合は、リストにユーザーは含まれていないとする。
            If otherTabsPost.FindAll(Function(item) item.PDate > myTabsOlderPostCreatedAt AndAlso (item.InReplyToId = 0 OrElse myTabsUserNames.Contains(item.InReplyToUser))).Count > 0 Then
                Me.ListsCheckedListBox.SetItemChecked(Me.ListsCheckedListBox.Items.IndexOf(listItem), False)
                Continue For
            End If

            'ここまでで推定出来なければAPIから取得する
            Dim ret As Boolean
            Dim rslt As String = Me._tw.ContainsUserAtList(listItem.Id.ToString(), contextUserName.ToString(), ret)
            If rslt <> "" Then
                MessageBox.Show("通信エラー (" + rslt + ")")
            End If
        Next
    End Sub

    Private Sub ListsCheckedListBox_ItemCheck(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles ListsCheckedListBox.ItemCheck
        If e.NewValue = CheckState.Checked Then
            Dim list As ListElement = CType(Me.ListsCheckedListBox.Items(e.Index), ListElement)
            Dim rslt As String = Me._tw.AddUserToList(list.Id.ToString(), Me.contextUserName.ToString())
            If rslt <> "" Then
                MessageBox.Show("通信エラー (" + rslt + ")")
            End If
        ElseIf e.NewValue = CheckState.Unchecked Then
            Dim list As ListElement = CType(Me.ListsCheckedListBox.Items(e.Index), ListElement)
            Dim rslt As String = Me._tw.RemoveUserToList(list.Id.ToString(), Me.contextUserName.ToString())
            If rslt <> "" Then
                MessageBox.Show("通信エラー (" + rslt + ")")
            End If
        End If
    End Sub
End Class