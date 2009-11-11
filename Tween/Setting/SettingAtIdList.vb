<Serializable()> _
Public Class SettingAtIdList
    Inherits SettingBase(Of SettingAtIdList)

#Region "SettingƒNƒ‰ƒXŠî–{"
    Public Shared Function Load() As SettingAtIdList
        Dim setting As SettingAtIdList = LoadSettings()
        Return setting
    End Function

    Public Sub Save()
        SaveSettings(Me)
    End Sub

    Public Sub New()
        AtIdList = New List(Of String)
    End Sub

    Public Sub New(ByVal ids As List(Of String))
        Me.AtIdList = ids
    End Sub

#End Region

    Public AtIdList As List(Of String)
End Class
