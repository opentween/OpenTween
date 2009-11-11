<Serializable()> _
Public Class SettingFollower
    Inherits SettingBase(Of SettingFollower)

#Region "Settingクラス基本"
    Public Shared Function Load() As SettingFollower
        Dim setting As SettingFollower = LoadSettings()
        Return setting
    End Function

    Public Sub Save()
        SaveSettings(Me)
    End Sub

    Public Sub New()
        Follower = New List(Of String)
    End Sub

    Public Sub New(ByVal follower As List(Of String))
        Me.Follower = follower
    End Sub

#End Region

    Public Follower As List(Of String)

End Class
