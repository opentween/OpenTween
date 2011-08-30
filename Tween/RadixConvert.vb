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

Imports System.Text


Public Class RadixConvert

    ''' <summary>
    ''' インスタンス化を禁止しています。
    ''' </summary>
    Private Sub New()

    End Sub

#Region "Int16型およびUInt16型用のメソッド群"

    ''' <summary>
    ''' 3～36進数の数値文字列をInt16型の数値に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToInt16メソッドを使ってください。
    ''' ※＋や－の符号や0xなどのプレフィックスには対応していません。
    ''' ※引数となる数値文字列に、スペースなどの文字を含めないでください。
    ''' </remarks>
    ''' <param name="s">数値文字列</param>
    ''' <param name="radix">基数</param>
    ''' <returns>数値</returns>
    Public Shared Function ToInt16(ByVal s As String, ByVal radix As Integer) As Short

        Dim digit As ULong = ToUInt64(s, radix)
        CheckDigitOverflow(digit, Int16.MaxValue)
        Return CType(digit, Short)

    End Function

    ''' <summary>
    ''' 3～36進数の数値文字列をUInt16型の数値に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToUInt16メソッドを使ってください。
    ''' ※＋や－の符号や0xなどのプレフィックスには対応していません。
    ''' ※引数となる数値文字列に、スペースなどの文字を含めないでください。
    ''' </remarks>
    ''' <param name="s">数値文字列</param>
    ''' <param name="radix">基数</param>
    ''' <returns>数値</returns>
    Public Shared Function ToUInt16(ByVal s As String, ByVal radix As Integer) As UShort

        Dim digit As ULong = ToUInt64(s, radix)
        CheckDigitOverflow(digit, UInt16.MaxValue)
        Return CType(digit, UShort)

    End Function

    ''' <summary>
    ''' UInt16型の数値を3～36進数の数値文字列に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToStringメソッドを使ってください。
    ''' ※－符号には対応していません。
    ''' </remarks>
    ''' <param name="n">数値</param>
    ''' <param name="radix">基数</param>
    ''' <param name="uppercase">大文字か（true）、小文字か（false）</param>
    ''' <returns>数値文字列</returns>
    Public Overloads Shared Function ToString(ByVal n As Short, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        Return ToString(CType(n, ULong), radix, uppercase)

    End Function

    ''' <summary>
    ''' UInt16型の数値を3～36進数の数値文字列に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToStringメソッドを使ってください。
    ''' ※－符号には対応していません。
    ''' </remarks>
    ''' <param name="n">数値</param>
    ''' <param name="radix">基数</param>
    ''' <param name="uppercase">大文字か（true）、小文字か（false）</param>
    ''' <returns>数値文字列</returns>
    Public Overloads Shared Function ToString(ByVal n As UShort, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        Return ToString(CType(n, ULong), radix, uppercase)

    End Function

#End Region

#Region "Int32型およびUInt32型用のメソッド群"

    ''' <summary>
    ''' 3～36進数の数値文字列をInt32型の数値に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToInt32メソッドを使ってください。
    ''' ※＋や－の符号や0xなどのプレフィックスには対応していません。
    ''' ※引数となる数値文字列に、スペースなどの文字を含めないでください。
    ''' </remarks>
    ''' <param name="s">数値文字列</param>
    ''' <param name="radix">基数</param>
    ''' <returns>数値</returns>
    Public Shared Function ToInt32(ByVal s As String, ByVal radix As Integer) As Integer

        Dim digit As ULong = ToUInt64(s, radix)
        CheckDigitOverflow(digit, Int32.MaxValue)
        Return CType(digit, Integer)

    End Function

    ''' <summary>
    ''' 3～36進数の数値文字列をUInt32型の数値に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToUInt32メソッドを使ってください。
    ''' ※＋や－の符号や0xなどのプレフィックスには対応していません。
    ''' ※引数となる数値文字列に、スペースなどの文字を含めないでください。
    ''' </remarks>
    ''' <param name="s">数値文字列</param>
    ''' <param name="radix">基数</param>
    ''' <returns>数値</returns>
    Public Shared Function ToUInt32(ByVal s As String, ByVal radix As Integer) As UInteger

        Dim digit As ULong = ToUInt64(s, radix)
        CheckDigitOverflow(digit, UInt32.MaxValue)
        Return CType(digit, UInteger)

    End Function

    ''' <summary>
    ''' UInt32型の数値を3～36進数の数値文字列に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToStringメソッドを使ってください。
    ''' ※－符号には対応していません。
    ''' </remarks>
    ''' <param name="n">数値</param>
    ''' <param name="radix">基数</param>
    ''' <param name="uppercase">大文字か（true）、小文字か（false）</param>
    ''' <returns>数値文字列</returns>
    Public Overloads Shared Function ToString(ByVal n As Integer, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        Return ToString(CType(n, ULong), radix, uppercase)

    End Function

    ''' <summary>
    ''' UInt32型の数値を3～36進数の数値文字列に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToStringメソッドを使ってください。
    ''' ※－符号には対応していません。
    ''' </remarks>
    ''' <param name="n">数値</param>
    ''' <param name="radix">基数</param>
    ''' <param name="uppercase">大文字か（true）、小文字か（false）</param>
    ''' <returns>数値文字列</returns>
    Public Overloads Shared Function ToString(ByVal n As UInteger, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        Return ToString(CType(n, ULong), radix, uppercase)

    End Function

#End Region

#Region "Int64型およびUInt64型用のメソッド群"

    ''' <summary>
    ''' 3～36進数の数値文字列をInt64型の数値に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToInt64メソッドを使ってください。
    ''' ※＋や－の符号や0xなどのプレフィックスには対応していません。
    ''' ※引数となる数値文字列に、スペースなどの文字を含めないでください。
    ''' </remarks>
    ''' <param name="s">数値文字列</param>
    ''' <param name="radix">基数</param>
    ''' <returns>数値</returns>
    Public Shared Function ToInt64(ByVal s As String, ByVal radix As Integer) As Long

        Dim digit As ULong = ToUInt64(s, radix)
        CheckDigitOverflow(digit, Int64.MaxValue)
        Return CType(digit, Long)

    End Function

    ''' <summary>
    ''' 3～36進数の数値文字列をUInt64型の数値に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToUInt64メソッドを使ってください。
    ''' ※＋や－の符号や0xなどのプレフィックスには対応していません。
    ''' ※引数となる数値文字列に、スペースなどの文字を含めないでください。
    ''' </remarks>
    ''' <param name="s">数値文字列</param>
    ''' <param name="radix">基数</param>
    ''' <returns>数値</returns>
    Public Shared Function ToUInt64(ByVal s As String, ByVal radix As Integer) As ULong

        ' 引数をチェックをする
        CheckNumberArgument(s)
        CheckRadixArgument(radix)

        Dim curValue As ULong = 0                                       ' 変換中の数値
        Dim maxValue As ULong = CType(UInt64.MaxValue / CType(radix, ULong), ULong)   ' 最大値の1けた前の数値

        ' 数値文字列を解析して数値に変換する
        Dim num As Char         ' 処理中の1けたの数値文字列
        Dim digit As Integer    ' 処理中の1けたの数値
        Dim length As Integer = s.Length
        For i As Integer = 0 To length - 1
            num = s(i)
            digit = GetDigitFromNumber(num)
            CheckDigitOutOfRange(digit, radix)

            ' 次にradixを掛けるときに数値がオーバーフローしないかを事前にチェックする
            CheckDigitOverflow(curValue, maxValue)
            curValue = curValue * CType(radix, ULong) + CType(digit, ULong)
        Next

        Return curValue

    End Function

    ''' <summary>
    ''' UInt64型の数値を3～36進数の数値文字列に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToStringメソッドを使ってください。
    ''' ※－符号には対応していません。
    ''' </remarks>
    ''' <param name="n">数値</param>
    ''' <param name="radix">基数</param>
    ''' <param name="uppercase">大文字か（true）、小文字か（false）</param>
    ''' <returns>数値文字列</returns>
    Public Overloads Shared Function ToString(ByVal n As Long, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        Return ToString(CType(n, ULong), radix, uppercase)

    End Function

    ''' <summary>
    ''' UInt64型の数値を3～36進数の数値文字列に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToStringメソッドを使ってください。
    ''' ※－符号には対応していません。
    ''' </remarks>
    ''' <param name="n">数値</param>
    ''' <param name="radix">基数</param>
    ''' <param name="uppercase">大文字か（true）、小文字か（false）</param>
    ''' <returns>数値文字列</returns>
    Public Overloads Shared Function ToString(ByVal n As ULong, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        ' 引数をチェックをする
        CheckRadixArgument(radix)

        ' 数値の「0」は、どの進数でも「0」になる
        If n = 0 Then
            Return "0"
        End If

        Dim curValue As New StringBuilder(41)   ' 変換中の数値文字列
        ' ※UInt64.MaxValueの数値を3進数で表現すると41けたです。
        Dim curDigit As ULong = n               ' 未処理の数値

        ' 数値を解析して数値文字列に変換する
        Dim digit As ULong  ' 処理中の1けたの数値
        Do
            ' 一番下のけたの数値を取り出す
            digit = curDigit Mod CType(radix, ULong)
            ' 取り出した1けたを切り捨てる
            curDigit = CType(curDigit / CType(radix, ULong), ULong)

            curValue.Insert(0, GetNumberFromDigit(CType(digit, Integer), uppercase))
        Loop While curDigit <> 0

        Return curValue.ToString()

    End Function

#End Region

#Region "Decimal型用のメソッド群"

    ''' <summary>
    ''' 3～36進数の数値文字列をDecimal型の数値に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToDecimalメソッドを使ってください。
    ''' ※＋や－の符号や0xなどのプレフィックスには対応していません。
    ''' ※引数となる数値文字列に、スペースなどの文字を含めないでください。
    ''' </remarks>
    ''' <param name="s">数値文字列</param>
    ''' <param name="radix">基数</param>
    ''' <returns>数値</returns>
    Public Shared Function ToDecimal(ByVal s As String, ByVal radix As Integer) As Decimal

        ' 引数をチェックをする
        CheckNumberArgument(s)
        CheckRadixArgument(radix)

        Dim curValue As Decimal = 0                                         ' 変換中の数値
        Dim maxValue As Decimal = Decimal.MaxValue / CType(radix, Decimal)  ' 最大値の1けた前の数値

        ' 数値文字列を解析して数値に変換する
        Dim num As Char         ' 処理中の1けたの数値文字列
        Dim digit As Integer    ' 処理中の1けたの数値
        Dim length As Integer = s.Length
        For i As Integer = 0 To length - 1
            num = s(i)
            digit = GetDigitFromNumber(num)
            CheckDigitOutOfRange(digit, radix)

            ' 次にradixを掛けるときに数値がオーバーフローしないかを事前にチェックする
            CheckDigitOverflow(curValue, maxValue)
            curValue = curValue * CType(radix, Decimal) + CType(digit, Decimal)
        Next

        Return curValue

    End Function

    ''' <summary>
    ''' Decimal型の数値を3～36進数の数値文字列に変換します。
    ''' </summary>
    ''' <remarks>
    ''' ※2／8／10／16進数は、Convert.ToStringメソッドを使ってください。
    ''' ※－符号には対応していません。
    ''' </remarks>
    ''' <param name="n">数値</param>
    ''' <param name="radix">基数</param>
    ''' <param name="uppercase">大文字か（true）、小文字か（false）</param>
    ''' <returns>数値文字列</returns>
    Public Overloads Shared Function ToString(ByVal n As Decimal, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        ' 引数をチェックをする
        CheckRadixArgument(radix)

        ' 数値の「0」は、どの進数でも「0」になる
        If n = 0 Then
            Return "0"
        End If

        Dim curValue As New StringBuilder(120)  ' 変換中の数値文字列
        ' ※Decimal.MaxValueの数値を3進数で表現すると120けたです。
        Dim curDigit As Decimal = n             ' 未処理の数値

        ' 数値を解析して数値文字列に変換する
        Dim digit As Decimal   ' 処理中の1けたの数値
        Do
            ' 一番下のけたの数値を取り出す
            digit = curDigit Mod CType(radix, Decimal)
            ' 取り出した1けたを切り捨てる
            curDigit = curDigit / CType(radix, Decimal)

            curValue.Insert(0, GetNumberFromDigit(CType(digit, Integer), uppercase))
        Loop While curDigit <> 0

        Return curValue.ToString()

    End Function

#End Region

#Region "内部で使用しているメソッド群"

    Private Shared Sub CheckNumberArgument(ByVal s As String)

        If s = Nothing OrElse s = String.Empty Then
            Throw New ArgumentException("数値文字列が指定されていません。")
        End If

    End Sub

    Private Shared Sub CheckRadixArgument(ByVal radix As Integer)

        If radix = 2 OrElse radix = 8 OrElse radix = 10 OrElse radix = 16 Then
            Throw New ArgumentException("2／8／10／16進数はSystem.Convertクラスを使ってください。")
        End If
        If radix <= 1 OrElse 36 < radix Then
            Throw New ArgumentException("3～36進数にしか対応していません。")
        End If

    End Sub

    Private Shared Sub CheckDigitOutOfRange(ByVal digit As Integer, ByVal radix As Integer)

        If digit < 0 OrElse radix <= digit Then
            Throw New ArgumentOutOfRangeException("数値が範囲外です。")
        End If

    End Sub

    Private Shared Sub CheckDigitOverflow(ByVal curValue As ULong, ByVal maxValue As ULong)

        If curValue > maxValue Then
            Throw New OverflowException("数値が最大値を超えました。")
        End If

    End Sub

    Private Shared Sub CheckDigitOverflow(ByVal curValue As Decimal, ByVal maxValue As Decimal)

        If curValue > maxValue Then
            Throw New OverflowException("数値が最大値を超えました。")
        End If

    End Sub

    Private Shared Function GetDigitFromNumber(ByVal num As Char) As Integer

        Dim ascNum As Integer = Asc(num)
        If ascNum >= Asc("0") AndAlso ascNum <= Asc("9") Then
            Return Asc(num) - Asc("0")
        ElseIf ascNum >= Asc("A") AndAlso ascNum <= Asc("Z") Then
            Return ascNum - Asc("A") + 10
        ElseIf ascNum >= Asc("a") AndAlso ascNum <= Asc("z") Then
            Return ascNum - Asc("a") + 10
        Else
            Return -1
        End If

    End Function

    Private Shared Function GetNumberFromDigit(ByVal digit As Integer, ByVal uppercase As Boolean) As Char

        If digit < 10 Then
            Return Chr(Asc("0") + digit)
        ElseIf uppercase Then
            Return Chr(Asc("A") + digit - 10)
        Else
            Return Chr(Asc("a") + digit - 10)
        End If

    End Function

#End Region

End Class

