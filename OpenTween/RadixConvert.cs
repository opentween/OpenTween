// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
// All rights reserved.
// 
// This file is part of OpenTween.
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details. 
// 
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTween
{
    public static class RadixConvert
    {
        #region "Int16型およびUInt16型用のメソッド群"

        /// <summary>
        /// 3～36進数の数値文字列をInt16型の数値に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.ToInt16メソッドを使ってください。
        /// ※＋や－の符号や0xなどのプレフィックスには対応していません。
        /// ※引数となる数値文字列に、スペースなどの文字を含めないでください。
        /// </remarks>
        /// <param name="s">数値文字列</param>
        /// <param name="radix">基数</param>
        /// <returns>数値</returns>
        public static short ToInt16(string s, int radix)
        {
            var digit = ToUInt64(s, radix);
            CheckDigitOverflow(digit, Int16.MaxValue);
            return (short)digit;
        }

        /// <summary>
        /// 3～36進数の数値文字列をUInt16型の数値に変換します。
        /// </summary>
        /// <remarks>yy
        /// ※2／8／10／16進数は、Convert.ToUInt16メソッドを使ってください。
        /// ※＋や－の符号や0xなどのプレフィックスには対応していません。
        /// ※引数となる数値文字列に、スペースなどの文字を含めないでください。
        /// </remarks>
        /// <param name="s">数値文字列</param>
        /// <param name="radix">基数</param>
        /// <returns>数値</returns>
        public static ushort ToUInt16(string s, int radix)
        {
            var digit = ToUInt64(s, radix);
            CheckDigitOverflow(digit, UInt16.MaxValue);
            return (ushort)digit;
        }

        /// <summary>
        /// UInt16型の数値を3～36進数の数値文字列に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.Tostringメソッドを使ってください。
        /// ※－符号には対応していません。
        /// </remarks>
        /// <param name="n">数値</param>
        /// <param name="radix">基数</param>
        /// <param name="uppercase">大文字か（true）、小文字か（false）</param>
        /// <returns>数値文字列</returns>
        public static string ToString(short n, int radix, bool uppercase)
        {
            return ToString((ulong)n, radix, uppercase);
        }

        /// <summary>
        /// UInt16型の数値を3～36進数の数値文字列に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.Tostringメソッドを使ってください。
        /// ※－符号には対応していません。
        /// </remarks>
        /// <param name="n">数値</param>
        /// <param name="radix">基数</param>
        /// <param name="uppercase">大文字か（true）、小文字か（false）</param>
        /// <returns>数値文字列</returns>
        public static string ToString(ushort n, int radix, bool uppercase)
        {
            return ToString((ulong)n, radix, uppercase);
        }

        #endregion

        #region "Int32型およびUInt32型用のメソッド群"

        /// <summary>
        /// 3～36進数の数値文字列をInt32型の数値に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.ToInt32メソッドを使ってください。
        /// ※＋や－の符号や0xなどのプレフィックスには対応していません。
        /// ※引数となる数値文字列に、スペースなどの文字を含めないでください。
        /// </remarks>
        /// <param name="s">数値文字列</param>
        /// <param name="radix">基数</param>
        /// <returns>数値</returns>
        public static int ToInt32(string s, int radix)
        {
            var digit = ToUInt64(s, radix);
            CheckDigitOverflow(digit, Int32.MaxValue);
            return (int)digit;
        }

        /// <summary>
        /// 3～36進数の数値文字列をUInt32型の数値に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.ToUInt32メソッドを使ってください。
        /// ※＋や－の符号や0xなどのプレフィックスには対応していません。
        /// ※引数となる数値文字列に、スペースなどの文字を含めないでください。
        /// </remarks>
        /// <param name="s">数値文字列</param>
        /// <param name="radix">基数</param>
        /// <returns>数値</returns>
        public static uint ToUInt32(string s, int radix)
        {
            var digit = ToUInt64(s, radix);
            CheckDigitOverflow(digit, UInt32.MaxValue);
            return (uint)digit;
        }

        /// <summary>
        /// UInt32型の数値を3～36進数の数値文字列に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.Tostringメソッドを使ってください。
        /// ※－符号には対応していません。
        /// </remarks>
        /// <param name="n">数値</param>
        /// <param name="radix">基数</param>
        /// <param name="uppercase">大文字か（true）、小文字か（false）</param>
        /// <returns>数値文字列</returns>
        public static string ToString(int n, int radix, bool uppercase)
        {
            return ToString((ulong)n, radix, uppercase);
        }

        /// <summary>
        /// UInt32型の数値を3～36進数の数値文字列に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.Tostringメソッドを使ってください。
        /// ※－符号には対応していません。
        /// </remarks>
        /// <param name="n">数値</param>
        /// <param name="radix">基数</param>
        /// <param name="uppercase">大文字か（true）、小文字か（false）</param>
        /// <returns>数値文字列</returns>
        public static string ToString(uint n, int radix, bool uppercase)
        {
            return ToString((ulong)n, radix, uppercase);
        }

        #endregion

        #region "Int64型およびUInt64型用のメソッド群"

        /// <summary>
        /// 3～36進数の数値文字列をInt64型の数値に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.ToInt64メソッドを使ってください。
        /// ※＋や－の符号や0xなどのプレフィックスには対応していません。
        /// ※引数となる数値文字列に、スペースなどの文字を含めないでください。
        /// </remarks>
        /// <param name="s">数値文字列</param>
        /// <param name="radix">基数</param>
        /// <returns>数値</returns>
        public static long ToInt64(string s, int radix)
        {
            var digit = ToUInt64(s, radix); ;
            CheckDigitOverflow(digit, Int64.MaxValue);
            return (long)digit;
        }

        /// <summary>
        /// 3～36進数の数値文字列をUInt64型の数値に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.ToUInt64メソッドを使ってください。
        /// ※＋や－の符号や0xなどのプレフィックスには対応していません。
        /// ※引数となる数値文字列に、スペースなどの文字を含めないでください。
        /// </remarks>
        /// <param name="s">数値文字列</param>
        /// <param name="radix">基数</param>
        /// <returns>数値</returns>
        public static ulong ToUInt64(string s, int radix)
        {
            // 引数をチェックをする
            CheckNumberArgument(s);
            CheckRadixArgument(radix);

            var curValue = 0uL;                                       // 変換中の数値
            var maxValue = UInt64.MaxValue / (ulong)radix;            // 最大値の1けた前の数値

            // 数値文字列を解析して数値に変換する
            char num;         // 処理中の1けたの数値文字列
            int digit;    // 処理中の1けたの数値
            int length = s.Length;
            for (int i = 0; i < length; i++)
            {
                num = s[i];
                digit = GetDigitFromNumber(num);
                CheckDigitOutOfRange(digit, radix);

                // 次にradixを掛けるときに数値がオーバーフローしないかを事前にチェックする
                CheckDigitOverflow(curValue, maxValue);
                curValue = curValue * (ulong)radix + (ulong)digit;
            }
            return curValue;
        }

        /// <summary>
        /// UInt64型の数値を3～36進数の数値文字列に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.Tostringメソッドを使ってください。
        /// ※－符号には対応していません。
        /// </remarks>
        /// <param name="n">数値</param>
        /// <param name="radix">基数</param>
        /// <param name="uppercase">大文字か（true）、小文字か（false）</param>
        /// <returns>数値文字列</returns>
        public static string ToString(long n, int radix, bool uppercase)
        {
            return ToString((ulong)n, radix, uppercase);
        }

        /// <summary>
        /// UInt64型の数値を3～36進数の数値文字列に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.Tostringメソッドを使ってください。
        /// ※－符号には対応していません。
        /// </remarks>
        /// <param name="n">数値</param>
        /// <param name="radix">基数</param>
        /// <param name="uppercase">大文字か（true）、小文字か（false）</param>
        /// <returns>数値文字列</returns>
        public static string ToString(ulong n, int radix, bool uppercase)
        {
            // 引数をチェックをする
            CheckRadixArgument(radix);

            // 数値の「0」は、どの進数でも「0」になる
            if (n == 0)
            {
                return "0";
            }

            var curValue = new StringBuilder(41);   // 変換中の数値文字列
            // ※UInt64.MaxValueの数値を3進数で表現すると41けたです。
            var curDigit = n;               // 未処理の数値

            // 数値を解析して数値文字列に変換する
            ulong digit;  // 処理中の1けたの数値
            do
            {
                // 一番下のけたの数値を取り出す
                digit = curDigit % (ulong)radix;
                // 取り出した1けたを切り捨てる
                curDigit = curDigit / (ulong)radix;

                curValue.Insert(0, GetNumberFromDigit((int)digit, uppercase));
            } while (curDigit != 0);

            return curValue.ToString();
        }

        #endregion

        #region "Decimal型用のメソッド群"

        /// <summary>
        /// 3～36進数の数値文字列をDecimal型の数値に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.ToDecimalメソッドを使ってください。
        /// ※＋や－の符号や0xなどのプレフィックスには対応していません。
        /// ※引数となる数値文字列に、スペースなどの文字を含めないでください。
        /// </remarks>
        /// <param name="s">数値文字列</param>
        /// <param name="radix">基数</param>
        /// <returns>数値</returns>
        public static decimal ToDecimal(string s, int radix)
        {
            // 引数をチェックをする
            CheckNumberArgument(s);
            CheckRadixArgument(radix);

            var curValue = 0m;                                         // 変換中の数値
            decimal maxValue = decimal.MaxValue / (decimal)radix;  // 最大値の1けた前の数値

            // 数値文字列を解析して数値に変換する
            char num;         // 処理中の1けたの数値文字列
            int digit;    // 処理中の1けたの数値
            int length = s.Length;
            for (int i = 0; i < length; i++)
            {
                num = s[i];
                digit = GetDigitFromNumber(num);
                CheckDigitOutOfRange(digit, radix);

                // 次にradixを掛けるときに数値がオーバーフローしないかを事前にチェックする
                CheckDigitOverflow(curValue, maxValue);
                curValue = curValue * (decimal)radix + (decimal)digit;
            }
            return curValue;
        }

        /// <summary>
        /// Decimal型の数値を3～36進数の数値文字列に変換します。
        /// </summary>
        /// <remarks>
        /// ※2／8／10／16進数は、Convert.Tostringメソッドを使ってください。
        /// ※－符号には対応していません。
        /// </remarks>
        /// <param name="n">数値</param>
        /// <param name="radix">基数</param>
        /// <param name="uppercase">大文字か（true）、小文字か（false）</param>
        /// <returns>数値文字列</returns>
        public static string ToString(decimal n, int radix, bool uppercase)
        {
            // 引数をチェックをする
            CheckRadixArgument(radix);

            // 数値の「0」は、どの進数でも「0」になる
            if (n == 0)
            {
                return "0";
            }

            var curValue = new StringBuilder(120);  // 変換中の数値文字列
            // ※Decimal.MaxValueの数値を3進数で表現すると120けたです。
            var curDigit = n;             // 未処理の数値

            // 数値を解析して数値文字列に変換する
            decimal digit;   // 処理中の1けたの数値
            do
            {
                // 一番下のけたの数値を取り出す
                digit = curDigit % (decimal)radix;
                // 取り出した1けたを切り捨てる
                curDigit = curDigit / (decimal)radix;

                curValue.Insert(0, GetNumberFromDigit((int)digit, uppercase));
            } while (curDigit != 0);

            return curValue.ToString();
        }

        #endregion

        #region "内部で使用しているメソッド群"

        private static void CheckNumberArgument(string s)
        {
            if (object.Equals(s, null) || object.Equals(s, string.Empty))
            {
                throw new ArgumentException("数値文字列が指定されていません。");
            }
        }

        private static void CheckRadixArgument(int radix)
        {
            if (radix == 2 || radix == 8 || radix == 10 || radix == 16)
            {
                throw new ArgumentException("2／8／10／16進数はSystem.Convertクラスを使ってください。");
            }
            if (radix <= 1 || 36 < radix)
            {
                throw new ArgumentException("3～36進数にしか対応していません。");
            }
        }

        private static void CheckDigitOutOfRange(int digit, int radix)
        {
            if (digit < 0 || radix <= digit)
            {
                throw new ArgumentOutOfRangeException("数値が範囲外です。");
            }
        }

        private static void CheckDigitOverflow(ulong curValue, ulong maxValue)
        {
            if (curValue > maxValue)
            {
                throw new OverflowException("数値が最大値を超えました。");
            }
        }

        private static void CheckDigitOverflow(decimal curValue, decimal maxValue)
        {
            if (curValue > maxValue)
            {
                throw new OverflowException("数値が最大値を超えました。");
            }
        }

        private static int GetDigitFromNumber(char num)
        {
            var ascNum = num;
            if (ascNum >= '0' && ascNum <= '9')
            {
                return num - '0';
            }
            else if (ascNum >= 'A' && ascNum <= 'Z')
            {
                return ascNum - 'A' + 10;
            }
            else if (ascNum >= 'a' && ascNum <= 'z')
            {
                return ascNum - 'a' + 10;
            }
            else
            {
                return -1;
            }
        }

        private static char GetNumberFromDigit(int digit, bool uppercase)
        {
            if (digit < 10)
            {
                return (char)('0' + digit);
            }
            else if (uppercase)
            {
                return (char)('A' + digit - 10);
            }
            else
            {
                return (char)('a' + digit - 10);
            }
        }
        #endregion
    }
}
