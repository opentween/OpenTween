' Tween - Client of Twitter
' Copyright (c) 2007-2010 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
'           (c) 2008-2010 Moz (@syo68k) <http://iddy.jp/profile/moz/>
'           (c) 2008-2010 takeshik (@takeshik) <http://www.takeshik.org/>
' All rights reserved.
' 
' This file is part of Tween.
' 
' This program is free software; you can redistribute it and/or modify it
' under the terms of the GNU General Public License as published by the Free
' Software Foundation; either version 3 of the License, or (at your option)
' any later version.
' 
' This program is distributed in the hope that it will be useful, but
' WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
' or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
' for more details. 
' 
' You should have received a copy of the GNU General Public License along
' with this program. If not, see <http://www.gnu.org/licenses/>, or write to
' the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
' Boston, MA 02110-1301, USA.
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
        Tab = New TabClass
    End Sub

    Public Sub New(ByVal TabName As String)
        Me.Tab = New TabClass
        Tab.TabName = TabName
    End Sub

#End Region

    Public Shared Sub DeleteConfigFile()
        For Each file As IO.FileInfo In (New IO.DirectoryInfo(My.Application.Info.DirectoryPath + IO.Path.DirectorySeparatorChar)).GetFiles("SettingTab*.xml")
            Try
                file.Delete()
            Catch ex As Exception
                '削除権限がない場合
            End Try
        Next
    End Sub

    Public Tab As TabClass

End Class
