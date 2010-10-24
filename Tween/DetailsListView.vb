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

        Public Event VScrolled As System.EventHandler
        Public Event HScrolled As System.EventHandler

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

        <StructLayout(LayoutKind.Sequential)>
        Private Structure SCROLLINFO
            Public cbSize As Integer
            Public fMask As Integer
            Public nMin As Integer
            Public nMax As Integer
            Public nPage As Integer
            Public nPos As Integer
            Public nTrackPos As Integer
        End Structure

        Private Enum ScrollBarDirection
            SB_HORZ = 0
            SB_VERT = 1
            SB_CTL = 2
            SB_BOTH = 3
        End Enum

        Private Enum ScrollInfoMask
            SIF_RANGE = &H1
            SIF_PAGE = &H2
            SIF_POS = &H4
            SIF_DISABLENOSCROLL = &H8
            SIF_TRACKPOS = &H10
            SIF_ALL = (SIF_RANGE Or SIF_PAGE Or SIF_POS Or SIF_TRACKPOS)
        End Enum

        <DllImport("user32.dll")> _
        Private Shared Function GetScrollInfo(ByVal hWnd As IntPtr, ByVal fnBar As ScrollBarDirection, ByRef lpsi As SCROLLINFO) As Integer
        End Function

        Private si As New SCROLLINFO With { _
            .cbSize = Len(si), _
            .fMask = ScrollInfoMask.SIF_POS
        }

        <DebuggerStepThrough()> _
        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            Const WM_ERASEBKGND As Integer = &H14
            Const WM_PAINT As Integer = &HF
            Const WM_MOUSEWHEEL As Integer = &H20A
            Const WM_MOUSEHWHEEL As Integer = &H20E
            Const WM_HSCROLL As Integer = &H114
            Const WM_VSCROLL As Integer = &H115
            Const WM_KEYDOWN As Integer = &H100

            Dim hPos As Integer = -1
            Dim vPos As Integer = -1

            Select Case m.Msg
                Case WM_ERASEBKGND
                    If Me.changeBounds <> Rectangle.Empty Then
                        m.Msg = 0
                    End If
                Case WM_PAINT
                    If Me.changeBounds <> Rectangle.Empty Then
                        Win32Api.ValidateRect(Me.Handle, IntPtr.Zero)
                        Me.Invalidate(Me.changeBounds)
                        Me.changeBounds = Rectangle.Empty
                    End If
                Case WM_HSCROLL
                    RaiseEvent HScrolled(Me, EventArgs.Empty)
                Case WM_VSCROLL
                    RaiseEvent VScrolled(Me, EventArgs.Empty)
                Case WM_MOUSEWHEEL, WM_MOUSEHWHEEL, WM_KEYDOWN
                    If GetScrollInfo(Me.Handle, ScrollBarDirection.SB_VERT, si) <> 0 Then
                        vPos = si.nPos
                    End If
                    If GetScrollInfo(Me.Handle, ScrollBarDirection.SB_HORZ, si) <> 0 Then
                        hPos = si.nPos
                    End If
            End Select

            Try
                MyBase.WndProc(m)
            Catch ex As ArgumentOutOfRangeException
                'Substringでlengthが0以下。アイコンサイズが影響？
            Catch ex As AccessViolationException
                'WndProcのさらに先で発生する。
            End Try
            If Me.IsDisposed Then Exit Sub

            If vPos <> -1 Then
                If GetScrollInfo(Me.Handle, ScrollBarDirection.SB_VERT, si) <> 0 AndAlso vPos <> si.nPos Then
                    RaiseEvent VScrolled(Me, EventArgs.Empty)
                End If
            End If
            If hPos <> -1 Then
                If GetScrollInfo(Me.Handle, ScrollBarDirection.SB_HORZ, si) <> 0 AndAlso hPos <> si.nPos Then
                    RaiseEvent HScrolled(Me, EventArgs.Empty)
                End If
            End If
        End Sub
    End Class
End Namespace
