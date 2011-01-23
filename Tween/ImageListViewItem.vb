' Tween - Client of Twitter
' Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
'           (c) 2008-2011 Moz (@syo68k)
'           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
'           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
'           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
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

Public Class ImageListViewItem
    Inherits ListViewItem

    Private img As Image = Nothing
    Public Event ImageDownloaded(ByVal sender As Object, ByVal e As EventArgs)

    Public Sub New(ByVal items() As String, ByVal imageKey As String)
        MyBase.New(items, imageKey)
    End Sub

    Public Sub New(ByVal items() As String, ByVal imageDictionary As ImageDictionary, ByVal imageKey As String)
        MyBase.New(items, imageKey)

        Dim dummy As Image = imageDictionary.Item(imageKey, Sub(getImg)
                                                                If getImg Is Nothing Then Exit Sub
                                                                Me.img = getImg
                                                                If Me.ListView IsNot Nothing AndAlso
                                                                    Me.ListView.Created AndAlso
                                                                    Not Me.ListView.IsDisposed Then Me.ListView.Invoke(Sub()
                                                                                                                           If Me.Index < Me.ListView.VirtualListSize Then Me.ListView.RedrawItems(Me.Index, Me.Index, False)
                                                                                                                           RaiseEvent ImageDownloaded(Me, EventArgs.Empty)
                                                                                                                       End Sub)
                                                            End Sub)

    End Sub

    Public ReadOnly Property Image As Image
        Get
            Return Me.img
        End Get
    End Property

    Protected Overrides Sub Finalize()
        If Me.img IsNot Nothing Then
            Me.img.Dispose()
            Me.img = Nothing
        End If
        MyBase.Finalize()
    End Sub
End Class
