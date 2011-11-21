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

Imports System.ComponentModel
Imports System.Threading

'''<summary>
'''タスクサービス機能付きプログレスバー
'''</summary>
'''<remarks>
'''重要：BackGroundWorkerコンポーネントが実際のタスクサービスを行うため、DoWorkでコントロールを触ることはできない。
'''また、Twitterへの通信を必要とする場合は引数にTwitterInstanceを含めそれを使用すること。
''' 1.Class生成 2.コンストラクタの引数としてサービス登録(Dowork RunWorkerCompletedも同様 使用しない場合Nothing)
''' 3.Instance.Argumentへ,あるいはコンストラクタ引数へ引数セット 
''' 4.Instance.InfoMessage、またはコンストラクタ引数へ表示メッセージ設定 5.Instance.ShowDialog()により表示
''' 6. 必要な場合はInstance.Result(=Servicerのe.Result)を参照し戻り値を得る
''' 7.Dispose タスクサービスが正常終了した場合は自分自身をCloseするので最後にDisposeすること。
'''</remarks>

Public Class FormInfo

    Private Class BackgroundWorkerServicer
        Inherits BackgroundWorker

        Public Result As Object = Nothing

        Protected Overrides Sub OnRunWorkerCompleted(ByVal e As RunWorkerCompletedEventArgs)
            Me.Result = e.Result
            MyBase.OnRunWorkerCompleted(e)
        End Sub
    End Class

    Private _msg As String
    Private _arg As Object = Nothing

    Private Servicer As New BackgroundWorkerServicer

    Public Sub New(ByVal owner As System.Windows.Forms.Form, _
                   ByVal Message As String, _
                   ByVal DoWork As DoWorkEventHandler)

        doInitialize(owner, Message, DoWork, Nothing, Nothing)
    End Sub

    Public Sub New(ByVal owner As System.Windows.Forms.Form, _
                   ByVal Message As String, _
                   ByVal DoWork As DoWorkEventHandler, _
                   ByVal RunWorkerCompleted As RunWorkerCompletedEventHandler)
        doInitialize(owner, Message, DoWork, RunWorkerCompleted, Nothing)
    End Sub

    Public Sub New(ByVal owner As System.Windows.Forms.Form, _
                   ByVal Message As String, _
                   ByVal DoWork As DoWorkEventHandler, _
                   ByVal RunWorkerCompleted As RunWorkerCompletedEventHandler, _
                   ByVal Argument As Object)
        doInitialize(owner, Message, DoWork, RunWorkerCompleted, Argument)
    End Sub

    Private Sub doInitialize(ByVal owner As System.Windows.Forms.Form, _
                             ByVal Message As String, _
                             ByVal DoWork As DoWorkEventHandler, _
                             ByVal RunWorkerCompleted As RunWorkerCompletedEventHandler, _
                             ByVal Argument As Object)
        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        Me.Owner = owner
        Me.InfoMessage = Message
        AddHandler Me.Servicer.DoWork, DoWork

        If RunWorkerCompleted IsNot Nothing Then
            AddHandler Me.Servicer.RunWorkerCompleted, RunWorkerCompleted
        End If

        Me.Argument = Argument

    End Sub

    Private Sub LabelInformation_TextChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles LabelInformation.TextChanged
        LabelInformation.Refresh()
    End Sub

    '''<summary>
    '''ダイアログに表示されるユーザー向けメッセージを設定あるいは取得する
    '''</summary>
    '''<param name="msg">表示するメッセージ</param>
    '''<returns>現在設定されているメッセージ</returns>
    Public Property InfoMessage() As String
        Get
            Return _msg
        End Get
        Set(ByVal value As String)
            _msg = value
            LabelInformation.Text = _msg
        End Set
    End Property

    '''<summary>
    '''Servicerへ渡すパラメータ
    '''</summary>
    '''<param name="args">Servicerへ渡すパラメータ</param>
    '''<returns>現在設定されているServicerへ渡すパラメータ</returns>
    Public Property Argument() As Object
        Get
            Return _arg
        End Get
        Set(ByVal value As Object)
            _arg = value
        End Set
    End Property

    '''<summary>
    '''Servicerのe.Result
    '''</summary>
    '''<returns>Servicerのe.Result</returns>
    Public ReadOnly Property Result() As Object
        Get
            Return Servicer.Result
        End Get
    End Property

    Private Sub FormInfo_Shown(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Shown
        Servicer.RunWorkerAsync(_arg)
        While Servicer.IsBusy
            Thread.Sleep(100)
            My.Application.DoEvents()
        End While
        Me.TopMost = False          ' MessageBoxが裏に隠れる問題に対応
        Me.Close()
    End Sub

    ' フォームを閉じたあとに親フォームが最前面にならない問題に対応

    Private Sub FormInfo_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        If Owner IsNot Nothing AndAlso Owner.Created Then
            Owner.TopMost = Not Owner.TopMost
            Owner.TopMost = Not Owner.TopMost
        End If
    End Sub
End Class


