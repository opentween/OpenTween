Public Class ListElement
    Public Id As Long = 0
    Public Name As String = ""
    Public Description As String = ""
    Public Slug As String = ""
    Public IsPublic As Boolean = True
    Public SubscriberCount As Integer = 0   'çwì«é“êî
    Public MemberCount As Integer = 0   'ÉäÉXÉgÉÅÉìÉoêî
    Public UserId As Long = 0
    Public Username As String = ""
    Public Nickname As String = ""

    Protected _tw As Twitter

    Private _members As List(Of UserInfo) = Nothing
    Private _cursor As Long = -1

    Public Sub New()

    End Sub

    Public Sub New(ByVal xmlNode As Xml.XmlNode, ByVal tw As Twitter)
        Me.Description = xmlNode.Item("description").InnerText
        Me.Id = Long.Parse(xmlNode.Item("id").InnerText)
        Me.IsPublic = (xmlNode.Item("mode").InnerText = "public")
        Me.MemberCount = Integer.Parse(xmlNode.Item("member_count").InnerText)
        Me.Name = xmlNode.Item("name").InnerText
        Me.SubscriberCount = Integer.Parse(xmlNode.Item("subscriber_count").InnerText)
        Me.Slug = xmlNode.Item("slug").InnerText
        Dim xUserEntry As Xml.XmlElement = CType(xmlNode.SelectSingleNode("./user"), Xml.XmlElement)
        Me.Nickname = xUserEntry.Item("name").InnerText
        Me.Username = xUserEntry.Item("screen_name").InnerText
        Me.UserId = Long.Parse(xUserEntry.Item("id").InnerText)

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
