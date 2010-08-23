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

Imports System.Text
Imports System.Globalization
Imports System.Security.Principal
Imports System.Reflection
Imports System.Web

Public Module MyCommon
    Private ReadOnly LockObj As New Object
    Public _endingFlag As Boolean        '終了フラグ
    Public cultureStr As String = Nothing

    Public Enum IconSizes
        IconNone = 0
        Icon16 = 1
        Icon24 = 2
        Icon48 = 3
        Icon48_2 = 4
    End Enum

    Public Enum NameBalloonEnum
        None
        UserID
        NickName
    End Enum

    Public Enum DispTitleEnum
        None
        Ver
        Post
        UnreadRepCount
        UnreadAllCount
        UnreadAllRepCount
        UnreadCountAllCount
        OwnStatus
    End Enum

    Public Enum LogUnitEnum
        Minute
        Hour
        Day
    End Enum

    Public Enum UploadFileType
        Invalid
        Picture
        MultiMedia
    End Enum

    Public Enum UrlConverter
        TinyUrl
        Isgd
        Twurl
        Unu
        Bitly
        Jmp
        '特殊
        Nicoms
    End Enum

    Public Enum OutputzUrlmode
        twittercom
        twittercomWithUsername
    End Enum

    Public Enum HITRESULT
        None
        Copy
        CopyAndMark
        Move
        Exclude
    End Enum

    Public Enum HttpTimeOut
        MinValue = 10
        MaxValue = 120
        DefaultValue = 20
    End Enum

    'Backgroundworkerへ処理種別を通知するための引数用Enum
    Public Enum WORKERTYPE
        Timeline                'タイムライン取得
        Reply                   '返信取得
        DirectMessegeRcv        '受信DM取得
        DirectMessegeSnt        '送信DM取得
        PostMessage             '発言POST
        FavAdd                  'Fav追加
        FavRemove               'Fav削除
        Follower                'Followerリスト取得
        OpenUri                 'Uri開く
        Favorites               'Fav取得
        Retweet                 'Retweetする
        PublicSearch            '公式検索
        List                    'Lists
        '''
        ErrorState              'エラー表示のみで後処理終了(認証エラー時など)
    End Enum

    Public Structure DEFAULTTAB
        Const RECENT As String = "Recent"
        Const REPLY As String = "Reply"
        Const DM As String = "Direct"
        Const FAV As String = "Favorites"

        Private dummy As String

        Private Shadows Function ReferenceEquals() As Object
            Return New Object
        End Function
        Private Shadows Function Equals() As Object
            Return New Object
        End Function
    End Structure

    Public Const Block As Object = Nothing
    Public TraceFlag As Boolean = False

#If DEBUG Then
    Public DebugBuild As Boolean = True
#Else
    Public DebugBuild As Boolean = False
#End If

    Public Enum ACCOUNT_STATE
        Valid
        Invalid
        Validating
    End Enum

    Public Enum REPLY_ICONSTATE
        None
        StaticIcon
        BlinkIcon
    End Enum

    Public Sub TraceOut(ByVal Message As String)
        TraceOut(TraceFlag, Message)
    End Sub

    Public Sub TraceOut(ByVal OutputFlag As Boolean, ByVal Message As String)
        SyncLock LockObj
            If Not OutputFlag Then Exit Sub
            Dim now As DateTime = DateTime.Now
            Dim fileName As String = String.Format("TweenTrace-{0:0000}{1:00}{2:00}-{3:00}{4:00}{5:00}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second)

            Using writer As IO.StreamWriter = New IO.StreamWriter(fileName)
                writer.WriteLine("**** TraceOut: {0} ****", DateTime.Now.ToString())
                writer.WriteLine(My.Resources.TraceOutText1)
                writer.WriteLine(My.Resources.TraceOutText2)
                writer.WriteLine()
                writer.WriteLine(My.Resources.TraceOutText3)
                writer.WriteLine(My.Resources.TraceOutText4, Environment.OSVersion.VersionString)
                writer.WriteLine(My.Resources.TraceOutText5, Environment.Version.ToString())
                writer.WriteLine(My.Resources.TraceOutText6, fileVersion)
                writer.WriteLine(Message)
                writer.WriteLine()
            End Using
        End SyncLock
    End Sub

    ' エラー内容をバッファに書き出し
    ' 注意：最終的にファイル出力されるエラーログに記録されるため次の情報は書き出さない
    ' 文頭メッセージ、権限、動作環境
    ' Dataプロパティにある終了許可フラグのパースもここで行う

    Public Function ExceptionOut(ByVal ex As Exception, ByVal buffer As String, _
                                 Optional ByRef IsTerminatePermission As Boolean = True) As String
        Dim buf As New StringBuilder

        buf.AppendFormat(My.Resources.UnhandledExceptionText8, ex.GetType().FullName, ex.Message)
        buf.AppendLine()
        If ex.Data IsNot Nothing Then
            Dim needHeader As Boolean = True
            For Each dt As DictionaryEntry In ex.Data
                If needHeader Then
                    buf.AppendLine()
                    buf.AppendLine("-------Extra Information-------")
                    needHeader = False
                End If
                buf.AppendFormat("{0}  :  {1}", dt.Key, dt.Value)
                buf.AppendLine()
                If dt.Key.Equals("IsTerminatePermission") Then
                    IsTerminatePermission = CBool(dt.Value)
                End If
            Next
            If Not needHeader Then
                buf.AppendLine("-----End Extra Information-----")
            End If
        End If
        buf.AppendLine(ex.StackTrace)
        buf.AppendLine()

        'InnerExceptionが存在する場合書き出す
        Dim _ex As Exception = ex.InnerException
        Dim nesting As Integer = 0
        While _ex IsNot Nothing
            buf.AppendFormat("-----InnerException[{0}]-----" + vbCrLf, nesting)
            buf.AppendLine()
            buf.AppendFormat(My.Resources.UnhandledExceptionText8, _ex.GetType().FullName, _ex.Message)
            buf.AppendLine()
            If _ex.Data IsNot Nothing Then
                Dim needHeader As Boolean = True

                For Each dt As DictionaryEntry In _ex.Data
                    If needHeader Then
                        buf.AppendLine()
                        buf.AppendLine("-------Extra Information-------")
                        needHeader = False
                    End If
                    buf.AppendFormat("{0}  :  {1}", dt.Key, dt.Value)
                    If dt.Key.Equals("IsTerminatePermission") Then
                        IsTerminatePermission = CBool(dt.Value)
                    End If
                Next
                If Not needHeader Then
                    buf.AppendLine("-----End Extra Information-----")
                End If
            End If
            buf.AppendLine(_ex.StackTrace)
            buf.AppendLine()
            nesting += 1
            _ex = _ex.InnerException
        End While
        buffer = buf.ToString()
        Return buffer
    End Function

    Public Function ExceptionOut(ByVal ex As Exception) As Boolean
        SyncLock LockObj
            Dim IsTerminatePermission As Boolean = True
            Dim now As DateTime = DateTime.Now
            Dim fileName As String = String.Format("Tween-{0:0000}{1:00}{2:00}-{3:00}{4:00}{5:00}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second)

            Using writer As IO.StreamWriter = New IO.StreamWriter(fileName)
                Dim ident As WindowsIdentity = WindowsIdentity.GetCurrent()
                Dim princ As New WindowsPrincipal(ident)

                writer.WriteLine(My.Resources.UnhandledExceptionText1, DateTime.Now.ToString())
                writer.WriteLine(My.Resources.UnhandledExceptionText2)
                writer.WriteLine(My.Resources.UnhandledExceptionText3)
                ' 権限書き出し
                writer.WriteLine(My.Resources.UnhandledExceptionText11 + princ.IsInRole(WindowsBuiltInRole.Administrator).ToString)
                writer.WriteLine(My.Resources.UnhandledExceptionText12 + princ.IsInRole(WindowsBuiltInRole.User).ToString)
                writer.WriteLine()
                ' OSVersion,AppVersion書き出し
                writer.WriteLine(My.Resources.UnhandledExceptionText4)
                writer.WriteLine(My.Resources.UnhandledExceptionText5, Environment.OSVersion.VersionString)
                writer.WriteLine(My.Resources.UnhandledExceptionText6, Environment.Version.ToString())
                writer.WriteLine(My.Resources.UnhandledExceptionText7, fileVersion)

                Dim buffer As String = Nothing
                writer.Write(ExceptionOut(ex, buffer, IsTerminatePermission))
                writer.Flush()
            End Using

            Select Case MessageBox.Show(String.Format(My.Resources.UnhandledExceptionText9, fileName, Environment.NewLine), _
                               My.Resources.UnhandledExceptionText10, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error)
                Case DialogResult.Yes
                    Diagnostics.Process.Start(fileName)
                    Return False
                Case DialogResult.No
                    Return False
                Case DialogResult.Cancel
                    Return IsTerminatePermission
            End Select
        End SyncLock
    End Function

    ''' <summary>
    ''' URLに含まれているマルチバイト文字列を%xx形式でエンコードします。
    ''' <newpara>
    ''' マルチバイト文字のコードはUTF-8またはUnicodeで自動的に判断します。
    ''' </newpara>
    ''' </summary>
    ''' <param name = input>エンコード対象のURL</param>
    ''' <returns>マルチバイト文字の部分をUTF-8/%xx形式でエンコードした文字列を返します。</returns>

    Public Function urlEncodeMultibyteChar(ByVal _input As String) As String
        Dim uri As Uri = Nothing
        Dim sb As StringBuilder = New StringBuilder(256)
        Dim result As String = ""
        Dim c As Char = "d"c
        For Each c In _input
            If Convert.ToInt32(c) > 127 Then Exit For
        Next
        If Convert.ToInt32(c) <= 127 Then Return _input

        Dim input As String = HttpUtility.UrlDecode(_input)
retry:
        For Each c In input
            If Convert.ToInt32(c) > 255 Then
                ' Unicodeの場合(1charが複数のバイトで構成されている）
                ' UriクラスをNewして再構成し、入力をPathAndQueryのみとしてやり直す
                If uri Is Nothing Then
                    uri = New Uri(input)
                    input = uri.PathAndQuery
                    sb.Length = 0
                    GoTo retry
                End If
            ElseIf Convert.ToInt32(c) > 127 Then
                ' UTF-8の場合
                ' UriクラスをNewして再構成し、入力をinputからAuthority部分を除去してやり直す
                If uri Is Nothing Then
                    uri = New Uri(input)
                    input = input.Remove(0, uri.GetLeftPart(UriPartial.Authority).Length)
                    sb.Length = 0
                    GoTo retry
                Else
                    sb.Append("%" + Convert.ToInt16(c).ToString("X2").ToUpper())
                End If
            Else
                sb.Append(c)
            End If
        Next

        If uri Is Nothing Then
            result = sb.ToString()
        Else
            result = uri.GetLeftPart(UriPartial.Authority) + sb.ToString()
        End If

        Return result
    End Function

    ''' <summary>
    ''' URLのドメイン名をPunycode展開します。
    ''' <para>
    ''' ドメイン名がIDNでない場合はそのまま返します。
    ''' ドメインラベルの区切り文字はFULLSTOP(.、U002E)に置き換えられます。
    ''' </para>
    ''' </summary>
    ''' <param name="input">展開対象のURL</param>
    ''' <returns>IDNが含まれていた場合はPunycodeに展開したURLをを返します。Punycode展開時にエラーが発生した場合はNothingを返します。</returns>

    Public Function IDNDecode(ByVal input As String) As String
        Dim result As String = ""
        Dim IDNConverter As New IdnMapping

        If Not input.Contains("://") Then Return Nothing

        ' ドメイン名をPunycode展開
        Dim Domain As String
        Dim AsciiDomain As String

        Try
            Domain = input.Split("/"c)(2)
            AsciiDomain = IDNConverter.GetAscii(Domain)
        Catch ex As Exception
            Return Nothing
        End Try

        Return input.Replace("://" + Domain, "://" + AsciiDomain)
    End Function

    Public Sub MoveArrayItem(ByVal values() As Integer, ByVal idx_fr As Integer, ByVal idx_to As Integer)
        Dim moved_value As Integer = values(idx_fr)
        Dim num_moved As Integer = Math.Abs(idx_fr - idx_to)

        If idx_to < idx_fr Then
            Array.Copy(values, idx_to, values, _
                idx_to + 1, num_moved)
        Else
            Array.Copy(values, idx_fr + 1, values, _
                idx_fr, num_moved)
        End If

        values(idx_to) = moved_value
    End Sub

    Public Function EncryptString(ByVal str As String) As String
        If String.IsNullOrEmpty(str) Then Return ""

        '文字列をバイト型配列にする
        Dim bytesIn As Byte() = System.Text.Encoding.UTF8.GetBytes(str)

        'DESCryptoServiceProviderオブジェクトの作成
        Dim des As New System.Security.Cryptography.DESCryptoServiceProvider

        '共有キーと初期化ベクタを決定
        'パスワードをバイト配列にする
        Dim bytesKey As Byte() = System.Text.Encoding.UTF8.GetBytes("_tween_encrypt_key_")
        '共有キーと初期化ベクタを設定
        des.Key = ResizeBytesArray(bytesKey, des.Key.Length)
        des.IV = ResizeBytesArray(bytesKey, des.IV.Length)

        '暗号化されたデータを書き出すためのMemoryStream
        Using msOut As New System.IO.MemoryStream
            'DES暗号化オブジェクトの作成
            Using desdecrypt As System.Security.Cryptography.ICryptoTransform = _
                des.CreateEncryptor()

                '書き込むためのCryptoStreamの作成
                Using cryptStream As New System.Security.Cryptography.CryptoStream( _
                    msOut, desdecrypt, _
                    System.Security.Cryptography.CryptoStreamMode.Write)
                    '書き込む
                    cryptStream.Write(bytesIn, 0, bytesIn.Length)
                    cryptStream.FlushFinalBlock()
                    '暗号化されたデータを取得
                    Dim bytesOut As Byte() = msOut.ToArray()

                    '閉じる
                    cryptStream.Close()
                    msOut.Close()

                    'Base64で文字列に変更して結果を返す
                    Return System.Convert.ToBase64String(bytesOut)
                End Using
            End Using
        End Using
    End Function

    Public Function DecryptString(ByVal str As String) As String
        If String.IsNullOrEmpty(str) Then Return ""

        'DESCryptoServiceProviderオブジェクトの作成
        Dim des As New System.Security.Cryptography.DESCryptoServiceProvider

        '共有キーと初期化ベクタを決定
        'パスワードをバイト配列にする
        Dim bytesKey As Byte() = System.Text.Encoding.UTF8.GetBytes("_tween_encrypt_key_")
        '共有キーと初期化ベクタを設定
        des.Key = ResizeBytesArray(bytesKey, des.Key.Length)
        des.IV = ResizeBytesArray(bytesKey, des.IV.Length)

        'Base64で文字列をバイト配列に戻す
        Dim bytesIn As Byte() = System.Convert.FromBase64String(str)
        '暗号化されたデータを読み込むためのMemoryStream
        Using msIn As New System.IO.MemoryStream(bytesIn)
            'DES復号化オブジェクトの作成
            Using desdecrypt As System.Security.Cryptography.ICryptoTransform = _
                des.CreateDecryptor()
                '読み込むためのCryptoStreamの作成
                Using cryptStreem As New System.Security.Cryptography.CryptoStream( _
                    msIn, desdecrypt, _
                    System.Security.Cryptography.CryptoStreamMode.Read)

                    '復号化されたデータを取得するためのStreamReader
                    Using srOut As New System.IO.StreamReader( _
                        cryptStreem, System.Text.Encoding.UTF8)
                        '復号化されたデータを取得する
                        Dim result As String = srOut.ReadToEnd()

                        '閉じる
                        srOut.Close()
                        cryptStreem.Close()
                        msIn.Close()

                        Return result
                    End Using
                End Using
            End Using
        End Using
    End Function

    Public Function ResizeBytesArray(ByVal bytes() As Byte, _
                                ByVal newSize As Integer) As Byte()
        Dim newBytes(newSize - 1) As Byte
        If bytes.Length <= newSize Then
            Dim i As Integer
            For i = 0 To bytes.Length - 1
                newBytes(i) = bytes(i)
            Next i
        Else
            Dim pos As Integer = 0
            Dim i As Integer
            For i = 0 To bytes.Length - 1
                newBytes(pos) = newBytes(pos) Xor bytes(i)
                pos += 1
                If pos >= newBytes.Length Then
                    pos = 0
                End If
            Next i
        End If
        Return newBytes
    End Function

    Public Function IsNT6() As Boolean
        'NT6 kernelかどうか検査
        Return Environment.OSVersion.Platform = PlatformID.Win32NT AndAlso Environment.OSVersion.Version.Major = 6
    End Function

    Public Function ReplaceInvalidFilename(ByVal name As String) As String
        name = name.Replace("\", "[en]")
        name = name.Replace(":", "[c]")
        name = name.Replace("/", "[bs]")
        name = name.Replace("?", "[q]")
        name = name.Replace("*", "[a]")
        name = name.Replace("<", "[lt]")
        name = name.Replace(">", "[gt]")
        name = name.Replace("|", "[p]")
        name = name.Replace(ChrW(&H201D), "[wdq]")
        name = name.Replace("""", "[dq]")

        Return name
    End Function

    <FlagsAttribute()> _
    Public Enum TabUsageType
        Undefined = 0
        Home = 1      'Unique
        Mentions = 2     'Unique
        DirectMessage = 4   'Unique
        Favorites = 8       'Unique
        UserDefined = 16
        LocalQuery = 32      'Pin(no save/no save query/distribute/no update(normal update))
        Profile = 64         'Pin(save/no distribute/manual update)
        PublicSearch = 128    'Pin(save/no distribute/auto update)
        Lists = 256
        'RTMyTweet
        'RTByOthers
        'RTByMe
    End Enum

    Public fileVersion As String

    Public WithEvents TwitterApiInfo As New ApiInformation
End Module
