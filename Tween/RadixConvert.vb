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
    ''' �C���X�^���X�����֎~���Ă��܂��B
    ''' </summary>
    Private Sub New()

    End Sub

#Region "Int16�^�����UInt16�^�p�̃��\�b�h�Q"

    ''' <summary>
    ''' 3�`36�i���̐��l�������Int16�^�̐��l�ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToInt16���\�b�h���g���Ă��������B
    ''' ���{��|�̕�����0x�Ȃǂ̃v���t�B�b�N�X�ɂ͑Ή����Ă��܂���B
    ''' �������ƂȂ鐔�l������ɁA�X�y�[�X�Ȃǂ̕������܂߂Ȃ��ł��������B
    ''' </remarks>
    ''' <param name="s">���l������</param>
    ''' <param name="radix">�</param>
    ''' <returns>���l</returns>
    Public Shared Function ToInt16(ByVal s As String, ByVal radix As Integer) As Short

        Dim digit As ULong = ToUInt64(s, radix)
        CheckDigitOverflow(digit, Int16.MaxValue)
        Return CType(digit, Short)

    End Function

    ''' <summary>
    ''' 3�`36�i���̐��l�������UInt16�^�̐��l�ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToUInt16���\�b�h���g���Ă��������B
    ''' ���{��|�̕�����0x�Ȃǂ̃v���t�B�b�N�X�ɂ͑Ή����Ă��܂���B
    ''' �������ƂȂ鐔�l������ɁA�X�y�[�X�Ȃǂ̕������܂߂Ȃ��ł��������B
    ''' </remarks>
    ''' <param name="s">���l������</param>
    ''' <param name="radix">�</param>
    ''' <returns>���l</returns>
    Public Shared Function ToUInt16(ByVal s As String, ByVal radix As Integer) As UShort

        Dim digit As ULong = ToUInt64(s, radix)
        CheckDigitOverflow(digit, UInt16.MaxValue)
        Return CType(digit, UShort)

    End Function

    ''' <summary>
    ''' UInt16�^�̐��l��3�`36�i���̐��l������ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToString���\�b�h���g���Ă��������B
    ''' ���|�����ɂ͑Ή����Ă��܂���B
    ''' </remarks>
    ''' <param name="n">���l</param>
    ''' <param name="radix">�</param>
    ''' <param name="uppercase">�啶�����itrue�j�A���������ifalse�j</param>
    ''' <returns>���l������</returns>
    Public Overloads Shared Function ToString(ByVal n As Short, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        Return ToString(CType(n, ULong), radix, uppercase)

    End Function

    ''' <summary>
    ''' UInt16�^�̐��l��3�`36�i���̐��l������ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToString���\�b�h���g���Ă��������B
    ''' ���|�����ɂ͑Ή����Ă��܂���B
    ''' </remarks>
    ''' <param name="n">���l</param>
    ''' <param name="radix">�</param>
    ''' <param name="uppercase">�啶�����itrue�j�A���������ifalse�j</param>
    ''' <returns>���l������</returns>
    Public Overloads Shared Function ToString(ByVal n As UShort, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        Return ToString(CType(n, ULong), radix, uppercase)

    End Function

#End Region

#Region "Int32�^�����UInt32�^�p�̃��\�b�h�Q"

    ''' <summary>
    ''' 3�`36�i���̐��l�������Int32�^�̐��l�ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToInt32���\�b�h���g���Ă��������B
    ''' ���{��|�̕�����0x�Ȃǂ̃v���t�B�b�N�X�ɂ͑Ή����Ă��܂���B
    ''' �������ƂȂ鐔�l������ɁA�X�y�[�X�Ȃǂ̕������܂߂Ȃ��ł��������B
    ''' </remarks>
    ''' <param name="s">���l������</param>
    ''' <param name="radix">�</param>
    ''' <returns>���l</returns>
    Public Shared Function ToInt32(ByVal s As String, ByVal radix As Integer) As Integer

        Dim digit As ULong = ToUInt64(s, radix)
        CheckDigitOverflow(digit, Int32.MaxValue)
        Return CType(digit, Integer)

    End Function

    ''' <summary>
    ''' 3�`36�i���̐��l�������UInt32�^�̐��l�ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToUInt32���\�b�h���g���Ă��������B
    ''' ���{��|�̕�����0x�Ȃǂ̃v���t�B�b�N�X�ɂ͑Ή����Ă��܂���B
    ''' �������ƂȂ鐔�l������ɁA�X�y�[�X�Ȃǂ̕������܂߂Ȃ��ł��������B
    ''' </remarks>
    ''' <param name="s">���l������</param>
    ''' <param name="radix">�</param>
    ''' <returns>���l</returns>
    Public Shared Function ToUInt32(ByVal s As String, ByVal radix As Integer) As UInteger

        Dim digit As ULong = ToUInt64(s, radix)
        CheckDigitOverflow(digit, UInt32.MaxValue)
        Return CType(digit, UInteger)

    End Function

    ''' <summary>
    ''' UInt32�^�̐��l��3�`36�i���̐��l������ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToString���\�b�h���g���Ă��������B
    ''' ���|�����ɂ͑Ή����Ă��܂���B
    ''' </remarks>
    ''' <param name="n">���l</param>
    ''' <param name="radix">�</param>
    ''' <param name="uppercase">�啶�����itrue�j�A���������ifalse�j</param>
    ''' <returns>���l������</returns>
    Public Overloads Shared Function ToString(ByVal n As Integer, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        Return ToString(CType(n, ULong), radix, uppercase)

    End Function

    ''' <summary>
    ''' UInt32�^�̐��l��3�`36�i���̐��l������ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToString���\�b�h���g���Ă��������B
    ''' ���|�����ɂ͑Ή����Ă��܂���B
    ''' </remarks>
    ''' <param name="n">���l</param>
    ''' <param name="radix">�</param>
    ''' <param name="uppercase">�啶�����itrue�j�A���������ifalse�j</param>
    ''' <returns>���l������</returns>
    Public Overloads Shared Function ToString(ByVal n As UInteger, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        Return ToString(CType(n, ULong), radix, uppercase)

    End Function

#End Region

#Region "Int64�^�����UInt64�^�p�̃��\�b�h�Q"

    ''' <summary>
    ''' 3�`36�i���̐��l�������Int64�^�̐��l�ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToInt64���\�b�h���g���Ă��������B
    ''' ���{��|�̕�����0x�Ȃǂ̃v���t�B�b�N�X�ɂ͑Ή����Ă��܂���B
    ''' �������ƂȂ鐔�l������ɁA�X�y�[�X�Ȃǂ̕������܂߂Ȃ��ł��������B
    ''' </remarks>
    ''' <param name="s">���l������</param>
    ''' <param name="radix">�</param>
    ''' <returns>���l</returns>
    Public Shared Function ToInt64(ByVal s As String, ByVal radix As Integer) As Long

        Dim digit As ULong = ToUInt64(s, radix)
        CheckDigitOverflow(digit, Int64.MaxValue)
        Return CType(digit, Long)

    End Function

    ''' <summary>
    ''' 3�`36�i���̐��l�������UInt64�^�̐��l�ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToUInt64���\�b�h���g���Ă��������B
    ''' ���{��|�̕�����0x�Ȃǂ̃v���t�B�b�N�X�ɂ͑Ή����Ă��܂���B
    ''' �������ƂȂ鐔�l������ɁA�X�y�[�X�Ȃǂ̕������܂߂Ȃ��ł��������B
    ''' </remarks>
    ''' <param name="s">���l������</param>
    ''' <param name="radix">�</param>
    ''' <returns>���l</returns>
    Public Shared Function ToUInt64(ByVal s As String, ByVal radix As Integer) As ULong

        ' �������`�F�b�N������
        CheckNumberArgument(s)
        CheckRadixArgument(radix)

        Dim curValue As ULong = 0                                       ' �ϊ����̐��l
        Dim maxValue As ULong = CType(UInt64.MaxValue / CType(radix, ULong), ULong)   ' �ő�l��1�����O�̐��l

        ' ���l���������͂��Đ��l�ɕϊ�����
        Dim num As Char         ' ��������1�����̐��l������
        Dim digit As Integer    ' ��������1�����̐��l
        Dim length As Integer = s.Length
        For i As Integer = 0 To length - 1
            num = s(i)
            digit = GetDigitFromNumber(num)
            CheckDigitOutOfRange(digit, radix)

            ' ����radix���|����Ƃ��ɐ��l���I�[�o�[�t���[���Ȃ��������O�Ƀ`�F�b�N����
            CheckDigitOverflow(curValue, maxValue)
            curValue = curValue * CType(radix, ULong) + CType(digit, ULong)
        Next

        Return curValue

    End Function

    ''' <summary>
    ''' UInt64�^�̐��l��3�`36�i���̐��l������ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToString���\�b�h���g���Ă��������B
    ''' ���|�����ɂ͑Ή����Ă��܂���B
    ''' </remarks>
    ''' <param name="n">���l</param>
    ''' <param name="radix">�</param>
    ''' <param name="uppercase">�啶�����itrue�j�A���������ifalse�j</param>
    ''' <returns>���l������</returns>
    Public Overloads Shared Function ToString(ByVal n As Long, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        Return ToString(CType(n, ULong), radix, uppercase)

    End Function

    ''' <summary>
    ''' UInt64�^�̐��l��3�`36�i���̐��l������ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToString���\�b�h���g���Ă��������B
    ''' ���|�����ɂ͑Ή����Ă��܂���B
    ''' </remarks>
    ''' <param name="n">���l</param>
    ''' <param name="radix">�</param>
    ''' <param name="uppercase">�啶�����itrue�j�A���������ifalse�j</param>
    ''' <returns>���l������</returns>
    Public Overloads Shared Function ToString(ByVal n As ULong, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        ' �������`�F�b�N������
        CheckRadixArgument(radix)

        ' ���l�́u0�v�́A�ǂ̐i���ł��u0�v�ɂȂ�
        If n = 0 Then
            Return "0"
        End If

        Dim curValue As New StringBuilder(41)   ' �ϊ����̐��l������
        ' ��UInt64.MaxValue�̐��l��3�i���ŕ\�������41�����ł��B
        Dim curDigit As ULong = n               ' �������̐��l

        ' ���l����͂��Đ��l������ɕϊ�����
        Dim digit As ULong  ' ��������1�����̐��l
        Do
            ' ��ԉ��̂����̐��l�����o��
            digit = curDigit Mod CType(radix, ULong)
            ' ���o����1������؂�̂Ă�
            curDigit = CType(curDigit / CType(radix, ULong), ULong)

            curValue.Insert(0, GetNumberFromDigit(CType(digit, Integer), uppercase))
        Loop While curDigit <> 0

        Return curValue.ToString()

    End Function

#End Region

#Region "Decimal�^�p�̃��\�b�h�Q"

    ''' <summary>
    ''' 3�`36�i���̐��l�������Decimal�^�̐��l�ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToDecimal���\�b�h���g���Ă��������B
    ''' ���{��|�̕�����0x�Ȃǂ̃v���t�B�b�N�X�ɂ͑Ή����Ă��܂���B
    ''' �������ƂȂ鐔�l������ɁA�X�y�[�X�Ȃǂ̕������܂߂Ȃ��ł��������B
    ''' </remarks>
    ''' <param name="s">���l������</param>
    ''' <param name="radix">�</param>
    ''' <returns>���l</returns>
    Public Shared Function ToDecimal(ByVal s As String, ByVal radix As Integer) As Decimal

        ' �������`�F�b�N������
        CheckNumberArgument(s)
        CheckRadixArgument(radix)

        Dim curValue As Decimal = 0                                         ' �ϊ����̐��l
        Dim maxValue As Decimal = Decimal.MaxValue / CType(radix, Decimal)  ' �ő�l��1�����O�̐��l

        ' ���l���������͂��Đ��l�ɕϊ�����
        Dim num As Char         ' ��������1�����̐��l������
        Dim digit As Integer    ' ��������1�����̐��l
        Dim length As Integer = s.Length
        For i As Integer = 0 To length - 1
            num = s(i)
            digit = GetDigitFromNumber(num)
            CheckDigitOutOfRange(digit, radix)

            ' ����radix���|����Ƃ��ɐ��l���I�[�o�[�t���[���Ȃ��������O�Ƀ`�F�b�N����
            CheckDigitOverflow(curValue, maxValue)
            curValue = curValue * CType(radix, Decimal) + CType(digit, Decimal)
        Next

        Return curValue

    End Function

    ''' <summary>
    ''' Decimal�^�̐��l��3�`36�i���̐��l������ɕϊ����܂��B
    ''' </summary>
    ''' <remarks>
    ''' ��2�^8�^10�^16�i���́AConvert.ToString���\�b�h���g���Ă��������B
    ''' ���|�����ɂ͑Ή����Ă��܂���B
    ''' </remarks>
    ''' <param name="n">���l</param>
    ''' <param name="radix">�</param>
    ''' <param name="uppercase">�啶�����itrue�j�A���������ifalse�j</param>
    ''' <returns>���l������</returns>
    Public Overloads Shared Function ToString(ByVal n As Decimal, ByVal radix As Integer, ByVal uppercase As Boolean) As String

        ' �������`�F�b�N������
        CheckRadixArgument(radix)

        ' ���l�́u0�v�́A�ǂ̐i���ł��u0�v�ɂȂ�
        If n = 0 Then
            Return "0"
        End If

        Dim curValue As New StringBuilder(120)  ' �ϊ����̐��l������
        ' ��Decimal.MaxValue�̐��l��3�i���ŕ\�������120�����ł��B
        Dim curDigit As Decimal = n             ' �������̐��l

        ' ���l����͂��Đ��l������ɕϊ�����
        Dim digit As Decimal   ' ��������1�����̐��l
        Do
            ' ��ԉ��̂����̐��l�����o��
            digit = curDigit Mod CType(radix, Decimal)
            ' ���o����1������؂�̂Ă�
            curDigit = curDigit / CType(radix, Decimal)

            curValue.Insert(0, GetNumberFromDigit(CType(digit, Integer), uppercase))
        Loop While curDigit <> 0

        Return curValue.ToString()

    End Function

#End Region

#Region "�����Ŏg�p���Ă��郁�\�b�h�Q"

    Private Shared Sub CheckNumberArgument(ByVal s As String)

        If s = Nothing OrElse s = String.Empty Then
            Throw New ArgumentException("���l�����񂪎w�肳��Ă��܂���B")
        End If

    End Sub

    Private Shared Sub CheckRadixArgument(ByVal radix As Integer)

        If radix = 2 OrElse radix = 8 OrElse radix = 10 OrElse radix = 16 Then
            Throw New ArgumentException("2�^8�^10�^16�i����System.Convert�N���X���g���Ă��������B")
        End If
        If radix <= 1 OrElse 36 < radix Then
            Throw New ArgumentException("3�`36�i���ɂ����Ή����Ă��܂���B")
        End If

    End Sub

    Private Shared Sub CheckDigitOutOfRange(ByVal digit As Integer, ByVal radix As Integer)

        If digit < 0 OrElse radix <= digit Then
            Throw New ArgumentOutOfRangeException("���l���͈͊O�ł��B")
        End If

    End Sub

    Private Shared Sub CheckDigitOverflow(ByVal curValue As ULong, ByVal maxValue As ULong)

        If curValue > maxValue Then
            Throw New OverflowException("���l���ő�l�𒴂��܂����B")
        End If

    End Sub

    Private Shared Sub CheckDigitOverflow(ByVal curValue As Decimal, ByVal maxValue As Decimal)

        If curValue > maxValue Then
            Throw New OverflowException("���l���ő�l�𒴂��܂����B")
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

