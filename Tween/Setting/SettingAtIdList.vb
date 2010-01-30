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
