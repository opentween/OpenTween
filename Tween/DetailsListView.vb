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

Imports System
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Diagnostics

Namespace TweenCustomControl

    Public NotInheritable Class DetailsListView
        Inherits ListView

        Private changeBounds As Rectangle
        Private multiSelected As Boolean
        Private _handlers As New System.ComponentModel.EventHandlerList()

        Public Sub New()
            View = Windows.Forms.View.Details
            FullRowSelect = True
            HideSelection = False
            DoubleBuffered = True
        End Sub

        <System.ComponentModel.DefaultValue(0), _
         System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.Repaint)> _
        Public Shadows Property VirtualListSize() As Integer
            Get
                Return MyBase.VirtualListSize
            End Get
            Set(ByVal value As Integer)
                If value = MyBase.VirtualListSize Then Exit Property
                If MyBase.VirtualListSize > 0 And value > 0 Then
                    Dim topIndex As Integer = 0
                    If MyBase.VirtualListSize < value Then
                        If Me.TopItem Is Nothing Then
                            topIndex = 0
                        Else
                            topIndex = Me.TopItem.Index
                        End If
                        topIndex = Math.Min(topIndex, Math.Abs(value - 1))
                        Me.TopItem = Me.Items(topIndex)
                    Else
                        Me.TopItem = Me.Items(0)
                    End If
                End If
                MyBase.VirtualListSize = value
            End Set
        End Property

        Public Sub ChangeItemBackColor(ByVal index As Integer, ByVal backColor As Color)
            ChangeSubItemBackColor(index, 0, backColor)
        End Sub

        Public Sub ChangeItemForeColor(ByVal index As Integer, ByVal foreColor As Color)
            ChangeSubItemForeColor(index, 0, foreColor)
        End Sub

        Public Sub ChangeItemFont(ByVal index As Integer, ByVal fnt As Font)
            ChangeSubItemFont(index, 0, fnt)
        End Sub

        Public Sub ChangeItemFontAndColor(ByVal index As Integer, ByVal foreColor As Color, ByVal fnt As Font)
            ChangeSubItemStyles(index, 0, BackColor, foreColor, fnt)
        End Sub

        Public Sub ChangeItemStyles(ByVal index As Integer, ByVal backColor As Color, ByVal foreColor As Color, ByVal fnt As Font)
            ChangeSubItemStyles(index, 0, backColor, foreColor, fnt)
        End Sub

        Public Sub ChangeSubItemBackColor(ByVal itemIndex As Integer, ByVal subitemIndex As Integer, ByVal backColor As Color)
            Me.Items(itemIndex).SubItems(subitemIndex).BackColor = backColor
            SetUpdateBounds(itemIndex, subitemIndex)
            Me.Update()
            Me.changeBounds = Rectangle.Empty
        End Sub

        Public Sub ChangeSubItemForeColor(ByVal itemIndex As Integer, ByVal subitemIndex As Integer, ByVal foreColor As Color)
            Me.Items(itemIndex).SubItems(subitemIndex).ForeColor = foreColor
            SetUpdateBounds(itemIndex, subitemIndex)
            Me.Update()
            Me.changeBounds = Rectangle.Empty
        End Sub

        Public Sub ChangeSubItemFont(ByVal itemIndex As Integer, ByVal subitemIndex As Integer, ByVal fnt As Font)
            Me.Items(itemIndex).SubItems(subitemIndex).Font = fnt
            SetUpdateBounds(itemIndex, subitemIndex)
            Me.Update()
            Me.changeBounds = Rectangle.Empty
        End Sub

        Public Sub ChangeSubItemFontAndColor(ByVal itemIndex As Integer, ByVal subitemIndex As Integer, ByVal foreColor As Color, ByVal fnt As Font)
            Me.Items(itemIndex).SubItems(subitemIndex).ForeColor = foreColor
            Me.Items(itemIndex).SubItems(subitemIndex).Font = fnt
            SetUpdateBounds(itemIndex, subitemIndex)
            Me.Update()
            Me.changeBounds = Rectangle.Empty
        End Sub

        Public Sub ChangeSubItemStyles(ByVal itemIndex As Integer, ByVal subitemIndex As Integer, ByVal backColor As Color, ByVal foreColor As Color, ByVal fnt As Font)
            Me.Items(itemIndex).SubItems(subitemIndex).BackColor = backColor
            Me.Items(itemIndex).SubItems(subitemIndex).ForeColor = foreColor
            Me.Items(itemIndex).SubItems(subitemIndex).Font = fnt
            SetUpdateBounds(itemIndex, subitemIndex)
            Me.Update()
            Me.changeBounds = Rectangle.Empty
        End Sub

        Private Sub SetUpdateBounds(ByVal itemIndex As Integer, ByVal subItemIndex As Integer)
            Try
                If itemIndex > Me.Items.Count Then
                    Throw New ArgumentOutOfRangeException("itemIndex")
                End If
                If subItemIndex > Me.Columns.Count Then
                    Throw New ArgumentOutOfRangeException("subItemIndex")
                End If
                Dim item As ListViewItem = Me.Items(itemIndex)
                If item.UseItemStyleForSubItems Then
                    Me.changeBounds = item.Bounds
                Else
                    Me.changeBounds = Me.GetSubItemBounds(itemIndex, subItemIndex)
                End If
            Catch ex As ArgumentException
                'タイミングによりBoundsプロパティが取れない？
                Me.changeBounds = Rectangle.Empty
            End Try
        End Sub

        Private Function GetSubItemBounds(ByVal itemIndex As Integer, ByVal subitemIndex As Integer) As Rectangle
            Dim item As ListViewItem = Me.Items(itemIndex)
            If subitemIndex = 0 And Me.Columns.Count > 0 Then
                Dim col0 As Rectangle = item.Bounds
                Return New Rectangle(col0.Left, col0.Top, item.SubItems(1).Bounds.X + 1, col0.Height)
            Else
                Return item.SubItems(subitemIndex).Bounds
            End If
        End Function

        <DebuggerStepThrough()> _
        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            Const WM_ERASEBKGND As Integer = &H14
            Const WM_PAINT As Integer = &HF

            If m.Msg = WM_ERASEBKGND Then
                If Me.changeBounds <> Rectangle.Empty Then
                    m.Msg = 0
                End If
            End If
            If m.Msg = WM_PAINT Then
                If Me.changeBounds <> Rectangle.Empty Then
                    Win32Api.ValidateRect(Me.Handle, IntPtr.Zero)
                    Me.Invalidate(Me.changeBounds)
                    Me.changeBounds = Rectangle.Empty
                End If
            End If
            Try
                MyBase.WndProc(m)
            Catch ex As ArgumentOutOfRangeException
                'Substringでlengthが0以下。アイコンサイズが影響？
            Catch ex As AccessViolationException
                'WndProcのさらに先で発生する。
            End Try
        End Sub

    End Class
End Namespace
