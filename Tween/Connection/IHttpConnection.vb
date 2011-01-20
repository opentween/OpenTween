' Tween - Client of Twitter
' Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
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

Imports System.Net
Imports System.IO

Public Interface IHttpConnection

    Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As Stream,
            ByVal userAgent As String) As HttpStatusCode

    Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String), _
            ByVal callback As CallbackDelegate) As HttpStatusCode

    Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByVal binary As List(Of KeyValuePair(Of String, FileInfo)), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String), _
            ByVal callback As CallbackDelegate) As HttpStatusCode

    Sub RequestAbort()

    Function Authenticate(ByVal url As Uri, ByVal username As String, ByVal password As String, ByRef content As String) As HttpStatusCode

ReadOnly Property AuthUsername() As String
    ''' <summary>
    ''' APIメソッドの処理が終了し呼び出し元へ戻る直前に呼ばれるデリゲート
    ''' </summary>
    ''' <param name="sender">メソッド名</param>
    ''' <param name="code">APIメソッドの返したHTTPステータスコード</param>
    ''' <param name="content">APIメソッドの処理結果</param>
    ''' <remarks>contentはNothingになることがあるのでチェックを必ず行うこと</remarks>
    Delegate Sub CallbackDelegate(ByVal sender As Object, ByRef code As HttpStatusCode, ByRef content As String)
End Interface
