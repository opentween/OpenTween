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

Public MustInherit Class SettingBase(Of T As {Class, New})

    Private Shared lockObj As New Object

    Protected Shared Function LoadSettings(ByVal FileId As String) As T
        Try
            SyncLock lockObj
                Using fs As New IO.FileStream(GetSettingFilePath(FileId), IO.FileMode.Open)
                    fs.Position = 0
                    Dim xs As New Xml.Serialization.XmlSerializer(GetType(T))
                    Dim instance As T = DirectCast(xs.Deserialize(fs), T)
                    fs.Close()
                    Return instance
                End Using
            End SyncLock
        Catch ex As System.IO.FileNotFoundException
            Return New T()
        Catch ex As Exception
            Dim backupFile As String = IO.Path.Combine( _
                    IO.Path.Combine( _
                        My.Application.Info.DirectoryPath, _
                        "TweenBackup1st"), _
                    GetType(T).Name + FileId + ".xml")
            If IO.File.Exists(backupFile) Then
                Try
                    SyncLock lockObj
                        Using fs As New IO.FileStream(backupFile, IO.FileMode.Open)
                            fs.Position = 0
                            Dim xs As New Xml.Serialization.XmlSerializer(GetType(T))
                            Dim instance As T = DirectCast(xs.Deserialize(fs), T)
                            fs.Close()
                            MessageBox.Show("File: " + GetSettingFilePath(FileId) + Environment.NewLine + "Use old setting file, because application can't read this setting file.")
                            Return instance
                        End Using
                    End SyncLock
                Catch ex2 As Exception
                End Try
            End If
            MessageBox.Show("File: " + GetSettingFilePath(FileId) + Environment.NewLine + "Use default setting, because application can't read this setting file.")
            Return New T()
            'ex.Data.Add("FilePath", GetSettingFilePath(FileId))
            'Dim fi As New IO.FileInfo(GetSettingFilePath(FileId))
            'ex.Data.Add("FileSize", fi.Length.ToString())
            'ex.Data.Add("FileData", IO.File.ReadAllText(GetSettingFilePath(FileId)))
            'Throw
        End Try
    End Function

    Protected Shared Function LoadSettings() As T
        Return LoadSettings("")
    End Function

    Protected Shared Sub SaveSettings(ByVal Instance As T, ByVal FileId As String)
        Dim cnt As Integer = 0
        Dim err As Boolean = False
        Dim fileName As String = GetSettingFilePath(FileId)
        If Instance Is Nothing Then Exit Sub
        Do
            err = False
            cnt += 1
            Try
                SyncLock lockObj
                    Using fs As New IO.FileStream(fileName, IO.FileMode.Create)
                        fs.Position = 0
                        Dim xs As New Xml.Serialization.XmlSerializer(GetType(T))
                        xs.Serialize(fs, Instance)
                        fs.Flush()
                        fs.Close()
                    End Using
                    Dim fi As New IO.FileInfo(fileName)
                    If fi.Length = 0 Then
                        If cnt > 3 Then
                            Throw New Exception
                            Exit Sub
                        End If
                        Threading.Thread.Sleep(1000)
                        err = True
                    End If
                End SyncLock
            Catch ex As Exception
                '検証エラー or 書き込みエラー
                If cnt > 3 Then
                    'リトライオーバー
                    MessageBox.Show("Can't write setting XML.(" + fileName + ")", "Save Settings", MessageBoxButtons.OK)
                    'Throw New System.InvalidOperationException("Can't write setting XML.(" + fileName + ")")
                    Exit Sub
                End If
                'リトライ
                Threading.Thread.Sleep(1000)
                err = True
            End Try
        Loop While err
    End Sub

    Protected Shared Sub SaveSettings(ByVal Instance As T)
        SaveSettings(Instance, "")
    End Sub

    Public Shared Function GetSettingFilePath(ByVal FileId As String) As String
        Return IO.Path.Combine(My.Application.Info.DirectoryPath, GetType(T).Name + FileId + ".xml")
    End Function
End Class
