<Serializable()> _
Public Class SettingTab
    Inherits SettingBase(Of SettingTab)

#Region "Settingクラス基本"
    Public Shared Function Load(ByVal tabName As String) As SettingTab
        Dim setting As SettingTab = LoadSettings(tabName)
        setting.Tab.TabName = tabName
        Return setting
    End Function

    Public Sub Save()
        SaveSettings(Me, Me.Tab.TabName)
    End Sub

    Public Sub New()
        TAB = New TabClass
    End Sub

    Public Sub New(ByVal TabName As String)
        Me.Tab = New TabClass
        TAB.TabName = TabName
    End Sub

#End Region

    Public Shared Sub DeleteConfigFile()
        For Each FileName As String In System.IO.Directory.GetFiles( _
                           My.Application.Info.DirectoryPath, "SettingTab*.xml")
            Try
                IO.File.Delete(FileName)
            Catch ex As Exception
                '削除権限がない場合
            End Try
        Next
    End Sub

    Public Tab As TabClass

End Class
