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

Imports System.ComponentModel

'''<summary>
'''タスクサービス機能付きプログレスバー
'''</summary>
'''<remarks>
'''重要：BackGroundWorkerコンポーネントが実際のタスクサービスを行うため、DoWorkでコントロールを触ることはできない。
'''また、Twitterへの通信を必要とする場合は引数にTwitterInstanceを含めそれを使用すること。呼び出しは次の手順を必要とする。
''' 1.Class生成 2.サービス登録(AddHandler Instance.Servicer.Dowork,AddressOf Handler Servicer.RunWorkerCompletedも同様)
''' 3.Instance.Argumentへ引数セット 4.Instance.InfoMessageへ表示メッセージ設定 5.Instance.ShowDialog()により表示
''' 6. 必要な場合はInstance.ReturnValue(=Servicerのe.Result)を参照し戻り値を得る
''' 7.Dispose タスクサービスが正常終了した場合は自分自身をCloseするので最後にDisposeすること。
'''</remarks>

Public Class FormInfo

    Private _msg As String
    Private _arg As Object = Nothing
    Private _ret As Object = Nothing

    Public Servicer As New BackgroundWorker



    Public Sub New()

        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        AddHandler Servicer.RunWorkerCompleted, AddressOf ResultCatcher

    End Sub

    Public Sub New(ByVal Message As String, _
                   ByVal DoWork As System.ComponentModel.DoWorkEventHandler, _
                   Optional ByVal RunWorkerCompleted As System.ComponentModel.RunWorkerCompletedEventHandler = Nothing, _
                   Optional ByVal Argument As Object = Nothing)

        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        AddHandler Servicer.RunWorkerCompleted, AddressOf ResultCatcher

        Me.InfoMessage = Message
        AddHandler Me.Servicer.DoWork, DoWork

        If RunWorkerCompleted IsNot Nothing Then
            AddHandler Me.Servicer.RunWorkerCompleted, RunWorkerCompleted
        End If

        Me.Argument = Argument

    End Sub

    Private Sub LabelInformation_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LabelInformation.TextChanged
        LabelInformation.Left = (Me.ClientSize.Width - LabelInformation.Size.Width) \ 2
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
            Return _ret
        End Get
    End Property

    Private Sub ResultCatcher(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
        _ret = e.Result
    End Sub

    Private Sub FormInfo_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Servicer.RunWorkerAsync(_arg)
        While Servicer.IsBusy
            Threading.Thread.Sleep(200)
            My.Application.DoEvents()
        End While
        Me.Close()
    End Sub

End Class


