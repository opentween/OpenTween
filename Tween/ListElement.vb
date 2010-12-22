Public Class ListElement
    Public Id As Long = 0
    Public Name As String = ""
    Public Description As String = ""
    Public Slug As String = ""
    Public IsPublic As Boolean = True
    Public SubscriberCount As Integer = 0   '購読者数
    Public MemberCount As Integer = 0   'リストメンバ数
    Public UserId As Long = 0
    Public Username As String = ""
    Public Nickname As String = ""

    Protected _tw As Twitter

    Private _members As List(Of UserInfo) = Nothing
    Private _cursor As Long = -1

    Public Sub New()

    End Sub

    Public Sub New(ByVal listElementData As TwitterDataModel.ListElementData, ByVal tw As Twitter)
        Me.Description = listElementData.Description
        Me.Id = listElementData.Id
        Me.IsPublic = (listElementData.Mode = "public")
        Me.MemberCount = listElementData.MemberCount
        Me.Name = listElementData.Name
        Me.SubscriberCount = listElementData.MemberCount
        Me.Slug = listElementData.Slug
        Me.Nickname = listElementData.User.Name
        Me.Username = listElementData.User.ScreenName
        Me.UserId = listElementData.User.Id

        Me._tw = tw
    End Sub

    Public Overridable Function Refresh() As String
        Return _tw.EditList(Me.Id.ToString(), Name, Not Me.IsPublic, Me.Description, Me)
    End Function

    <Xml.Serialization.XmlIgnore()>
    Public ReadOnly Property Members As List(Of UserInfo)
        Get
            If Me._members Is Nothing Then Me._members = New List(Of UserInfo)
            Return Me._members
        End Get
    End Property

    <Xml.Serialization.XmlIgnore()>
    Public ReadOnly Property Cursor As Long
        Get
            Return _cursor
        End Get
    End Property

    Public Function RefreshMembers() As String
        Dim users As New List(Of UserInfo)()
        _cursor = -1
        Dim result As String = Me._tw.GetListMembers(Me.Id.ToString(), users, _cursor)
        Me._members = users
        Return If(String.IsNullOrEmpty(result), Me.ToString, result)
    End Function

    Public Function GetMoreMembers() As String
        Dim result As String = Me._tw.GetListMembers(Me.Id.ToString(), Me._members, _cursor)
        Return If(String.IsNullOrEmpty(result), Me.ToString, result)
    End Function

    Public Overrides Function ToString() As String
        Return "@" + Username + "/" + Name + " [" + If(Me.IsPublic, "Public", "Protected") + "]"
    End Function
End Class
