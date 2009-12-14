<Serializable()> _
Public Class SettingTabs
    Inherits SettingBase(Of SettingTabs)
#Region "SettingƒNƒ‰ƒXŠî–{"
    Public Shared Function Load() As SettingTabs
        Dim setting As SettingTabs = LoadSettings("")
        Return setting
    End Function

    Public Sub Save()
        SaveSettings(Me)
    End Sub

    Public Sub New()
        Tabs = New List(Of TabClass)
    End Sub

#End Region

    Public Tabs As List(Of TabClass)

End Class
