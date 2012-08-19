// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
// 
// This file is part of OpenTween.
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details. 
// 
// You should have received a copy of the GNU General public License along
// with this program. if not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Web;
using System.Globalization;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Security.Principal;
using System.Runtime.Serialization.Json;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace OpenTween
{
    [Microsoft.VisualBasic.CompilerServices.StandardModule]
    public sealed class MyCommon
    {
        private static readonly object LockObj = new object();
        public static bool _endingFlag;        //終了フラグ
        public static string cultureStr = null;
        public static string settingPath;

        public enum IconSizes
        {
            IconNone = 0,
            Icon16 = 1,
            Icon24 = 2,
            Icon48 = 3,
            Icon48_2 = 4,
        }

        public enum NameBalloonEnum
        {
            None,
            UserID,
            NickName,
        }

        public enum DispTitleEnum
        {
            None,
            Ver,
            Post,
            UnreadRepCount,
            UnreadAllCount,
            UnreadAllRepCount,
            UnreadCountAllCount,
            OwnStatus,
        }

        public enum LogUnitEnum
        {
            Minute,
            Hour,
            Day,
        }

        public enum UploadFileType
        {
            Invalid,
            Picture,
            MultiMedia,
        }

        public enum UrlConverter
        {
            TinyUrl,
            Isgd,
            Twurl,
            Bitly,
            Jmp,
            Uxnu,
            //特殊
            Nicoms,
            //廃止
            Unu = -1,
        }

        public enum OutputzUrlmode
        {
            twittercom,
            twittercomWithUsername,
        }

        public enum HITRESULT
        {
            None,
            Copy,
            CopyAndMark,
            Move,
            Exclude,
        }

        public enum HttpTimeOut
        {
            MinValue = 10,
            MaxValue = 120,
            DefaultValue = 20,
        }

        //Backgroundworkerへ処理種別を通知するための引数用enum
        public enum WORKERTYPE
        {
            Timeline,                //タイムライン取得
            Reply,                   //返信取得
            DirectMessegeRcv,        //受信DM取得
            DirectMessegeSnt,        //送信DM取得
            PostMessage,             //発言POST
            FavAdd,                  //Fav追加
            FavRemove,               //Fav削除
            Follower,                //Followerリスト取得
            OpenUri,                 //Uri開く
            Favorites,               //Fav取得
            Retweet,                 //Retweetする
            PublicSearch,            //公式検索
            List,                    //Lists
            Related,                 //関連発言
            UserStream,              //UserStream
            UserTimeline,            //UserTimeline
            BlockIds,                //Blocking/ids
            Configuration,           //Twitter Configuration読み込み
            //////
            ErrorState,              //エラー表示のみで後処理終了(認証エラー時など)
        }

        public static class DEFAULTTAB
        {
            public const string RECENT = "Recent";
            public const string REPLY = "Reply";
            public const string DM = "Direct";
            public const string FAV = "Favorites";

            //private string dummy;

            //private object ReferenceEquals()
            //{
            //    return new object();
            //}
            //private object Equals()
            //{
            //    return new object();
            //}
        }

        public static readonly object Block = null;
        public static bool TraceFlag = false;

#if DEBUG
        public static bool DebugBuild = true;
#else
        public static bool DebugBuild = false;
#endif

        public enum ACCOUNT_STATE
        {
            Valid,
            Invalid,
        }

        public enum REPLY_ICONSTATE
        {
            None,
            StaticIcon,
            BlinkIcon,
        }

        [FlagsAttribute()]
        public enum EVENTTYPE
        {
            None = 0,
            Favorite = 1,
            Unfavorite = 2,
            Follow = 4,
            ListMemberAdded = 8,
            ListMemberRemoved = 16,
            Block = 32,
            Unblock = 64,
            UserUpdate = 128,
            Deleted = 256,
            ListCreated = 512,
            ListUpdated = 1024,

            All = (None | Favorite | Unfavorite | Follow | ListMemberAdded | ListMemberRemoved |
                   Block | Unblock | UserUpdate | Deleted | ListCreated | ListUpdated),
        }

        public static void TraceOut(Exception ex, string Message)
        {
            var buf = ExceptionOutMessage(ex);
            TraceOut(TraceFlag, Message + Environment.NewLine + buf);
        }

        public static void TraceOut(string Message)
        {
            TraceOut(TraceFlag, Message);
        }

        public static void TraceOut(bool OutputFlag, string Message)
        {
            lock (LockObj)
            {
                if (!OutputFlag) return;
                var now = DateTime.Now;
                var fileName = string.Format("{0}Trace-{1:0000}{2:00}{3:00}-{4:00}{5:00}{6:00}.log", GetAssemblyName(), now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                using (var writer = new StreamWriter(fileName))
                {
                    writer.WriteLine("**** TraceOut: {0} ****", DateTime.Now.ToString());
                    writer.WriteLine(Properties.Resources.TraceOutText1, ApplicationSettings.FeedbackEmailAddress);
                    writer.WriteLine(Properties.Resources.TraceOutText2, ApplicationSettings.FeedbackTwitterName);
                    writer.WriteLine();
                    writer.WriteLine(Properties.Resources.TraceOutText3);
                    writer.WriteLine(Properties.Resources.TraceOutText4, Environment.OSVersion.VersionString);
                    writer.WriteLine(Properties.Resources.TraceOutText5, Environment.Version.ToString());
                    writer.WriteLine(Properties.Resources.TraceOutText6, MyCommon.GetAssemblyName(), fileVersion);
                    writer.WriteLine(Message);
                    writer.WriteLine();
                }
            }
        }

        // エラー内容をバッファに書き出し
        // 注意：最終的にファイル出力されるエラーログに記録されるため次の情報は書き出さない
        // 文頭メッセージ、権限、動作環境
        // Dataプロパティにある終了許可フラグのパースもここで行う

        public static string ExceptionOutMessage(Exception ex)
        {
            bool IsTerminatePermission = true;
            return ExceptionOutMessage(ex, ref IsTerminatePermission);
        }

        public static string ExceptionOutMessage(Exception ex, ref bool IsTerminatePermission)
        {
            if (ex == null) return "";

            var buf = new StringBuilder();

            buf.AppendFormat(Properties.Resources.UnhandledExceptionText8, ex.GetType().FullName, ex.Message);
            buf.AppendLine();
            if (ex.Data != null)
            {
                var needHeader = true;
                foreach (DictionaryEntry dt in ex.Data)
                {
                    if (needHeader)
                    {
                        buf.AppendLine();
                        buf.AppendLine("-------Extra Information-------");
                        needHeader = false;
                    }
                    buf.AppendFormat("{0}  :  {1}", dt.Key, dt.Value);
                    buf.AppendLine();
                    if (dt.Key.Equals("IsTerminatePermission"))
                    {
                        IsTerminatePermission = (bool)dt.Value;
                    }
                }
                if (!needHeader)
                {
                    buf.AppendLine("-----End Extra Information-----");
                }
            }
            buf.AppendLine(ex.StackTrace);
            buf.AppendLine();

            //InnerExceptionが存在する場合書き出す
            var _ex = ex.InnerException;
            var nesting = 0;
            while (_ex != null)
            {
                buf.AppendFormat("-----InnerException[{0}]-----\r\n", nesting);
                buf.AppendLine();
                buf.AppendFormat(Properties.Resources.UnhandledExceptionText8, _ex.GetType().FullName, _ex.Message);
                buf.AppendLine();
                if (_ex.Data != null)
                {
                    var needHeader = true;

                    foreach (DictionaryEntry dt in _ex.Data)
                    {
                        if (needHeader)
                        {
                            buf.AppendLine();
                            buf.AppendLine("-------Extra Information-------");
                            needHeader = false;
                        }
                        buf.AppendFormat("{0}  :  {1}", dt.Key, dt.Value);
                        if (dt.Key.Equals("IsTerminatePermission"))
                        {
                            IsTerminatePermission = (bool)dt.Value;
                        }
                    }
                    if (!needHeader)
                    {
                        buf.AppendLine("-----End Extra Information-----");
                    }
                }
                buf.AppendLine(_ex.StackTrace);
                buf.AppendLine();
                nesting++;
                _ex = _ex.InnerException;
            }
            return buf.ToString();
        }

        public static bool ExceptionOut(Exception ex)
        {
            lock (LockObj)
            {
                var IsTerminatePermission = true;
                var now = DateTime.Now;
                var fileName = string.Format("{0}-{1:0000}{2:00}{3:00}-{4:00}{5:00}{6:00}.log", GetAssemblyName(), now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                using (var writer = new StreamWriter(fileName))
                {
                    var ident = WindowsIdentity.GetCurrent();
                    var princ = new WindowsPrincipal(ident);

                    writer.WriteLine(Properties.Resources.UnhandledExceptionText1, DateTime.Now.ToString());
                    writer.WriteLine(Properties.Resources.UnhandledExceptionText2, ApplicationSettings.FeedbackEmailAddress);
                    writer.WriteLine(Properties.Resources.UnhandledExceptionText3, ApplicationSettings.FeedbackTwitterName);
                    // 権限書き出し
                    writer.WriteLine(Properties.Resources.UnhandledExceptionText11 + princ.IsInRole(WindowsBuiltInRole.Administrator).ToString());
                    writer.WriteLine(Properties.Resources.UnhandledExceptionText12 + princ.IsInRole(WindowsBuiltInRole.User).ToString());
                    writer.WriteLine();
                    // OSVersion,AppVersion書き出し
                    writer.WriteLine(Properties.Resources.UnhandledExceptionText4);
                    writer.WriteLine(Properties.Resources.UnhandledExceptionText5, Environment.OSVersion.VersionString);
                    writer.WriteLine(Properties.Resources.UnhandledExceptionText6, Environment.Version.ToString());
                    writer.WriteLine(Properties.Resources.UnhandledExceptionText7, MyCommon.GetAssemblyName(), fileVersion);

                    writer.Write(ExceptionOutMessage(ex, ref IsTerminatePermission));
                    writer.Flush();
                }

                switch (MessageBox.Show(MyCommon.ReplaceAppName(string.Format(Properties.Resources.UnhandledExceptionText9, fileName, ApplicationSettings.FeedbackEmailAddress, ApplicationSettings.FeedbackTwitterName, Environment.NewLine)),
                                   Properties.Resources.UnhandledExceptionText10, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error))
                {
                    case DialogResult.Yes:
                        Process.Start(fileName);
                        return false;
                    case DialogResult.No:
                        return false;
                    case DialogResult.Cancel:
                        return IsTerminatePermission;
                    default:
                        throw new Exception("");
                }
            }
        }

        /// <summary>
        /// URLに含まれているマルチバイト文字列を%xx形式でエンコードします。
        /// <newpara>
        /// マルチバイト文字のコードはUTF-8またはUnicodeで自動的に判断します。
        /// </newpara>
        /// </summary>
        /// <param name = input>エンコード対象のURL</param>
        /// <returns>マルチバイト文字の部分をUTF-8/%xx形式でエンコードした文字列を返します。</returns>

        public static string urlEncodeMultibyteChar(string _input)
        {
            Uri uri = null;
            var sb = new StringBuilder(256);
            var result = "";
            var c_ = 'd';
            foreach (var c in _input)
            {
                c_ = c;
                if (Convert.ToInt32(c) > 127) break;
            }
            if (Convert.ToInt32(c_) <= 127) return _input;

            var input = HttpUtility.UrlDecode(_input);
        retry:
            foreach (char c in input)
            {
                if (Convert.ToInt32(c) > 255)
                {
                    // Unicodeの場合(1charが複数のバイトで構成されている）
                    // Uriクラスをnewして再構成し、入力をPathAndQueryのみとしてやり直す
                    foreach (var b in Encoding.UTF8.GetBytes(c.ToString()))
                    {
                        sb.AppendFormat("%{0:X2}", b);
                    }
                }
                else if (Convert.ToInt32(c) > 127 || c == '%')
                {
                    // UTF-8の場合
                    // Uriクラスをnewして再構成し、入力をinputからAuthority部分を除去してやり直す
                    if (uri == null)
                    {
                        uri = new Uri(input);
                        input = input.Remove(0, uri.GetLeftPart(UriPartial.Authority).Length);
                        sb.Length = 0;
                        goto retry;
                    }
                    else
                    {
                        sb.Append("%" + Convert.ToInt16(c).ToString("X2").ToUpper());
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (uri == null)
            {
                result = sb.ToString();
            }
            else
            {
                result = uri.GetLeftPart(UriPartial.Authority) + sb.ToString();
            }

            return result;
        }

        ////// <summary>
        ////// URLのドメイン名をPunycode展開します。
        ////// <para>
        ////// ドメイン名がIDNでない場合はそのまま返します。
        ////// ドメインラベルの区切り文字はFULLSTOP(.、U002E)に置き換えられます。
        ////// </para>
        ////// </summary>
        ////// <param name="input">展開対象のURL</param>
        ////// <returns>IDNが含まれていた場合はPunycodeに展開したURLをを返します。Punycode展開時にエラーが発生した場合はnullを返します。</returns>

        public static string IDNDecode(string input)
        {
            var IDNConverter = new IdnMapping();

            if (!input.Contains("://")) return null;

            // ドメイン名をPunycode展開
            string Domain;
            string AsciiDomain;

            try
            {
                Domain = input.Split('/')[2];
                AsciiDomain = IDNConverter.GetAscii(Domain);
            }
            catch (Exception)
            {
                return null;
            }

            return input.Replace("://" + Domain, "://" + AsciiDomain);
        }

        public static void MoveArrayItem(int[] values, int idx_fr, int idx_to)
        {
            var moved_value = values[idx_fr];
            var num_moved = Math.Abs(idx_fr - idx_to);

            if (idx_to < idx_fr)
            {
                Array.Copy(values, idx_to, values,
                    idx_to + 1, num_moved);
            }
            else
            {
                Array.Copy(values, idx_fr + 1, values,
                    idx_fr, num_moved);
            }

            values[idx_to] = moved_value;
        }

        public static string EncryptString(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";

            //文字列をバイト型配列にする
            var bytesIn = Encoding.UTF8.GetBytes(str);

            //DESCryptoServiceProviderオブジェクトの作成
            using (var des = new DESCryptoServiceProvider())
            {
                //共有キーと初期化ベクタを決定
                //パスワードをバイト配列にする
                var bytesKey = Encoding.UTF8.GetBytes("_tween_encrypt_key_");
                //共有キーと初期化ベクタを設定
                des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
                des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

                MemoryStream msOut = null;
                ICryptoTransform desdecrypt = null;

                try
                {
                    //暗号化されたデータを書き出すためのMemoryStream
                    msOut = new MemoryStream();

                    //DES暗号化オブジェクトの作成
                    desdecrypt = des.CreateEncryptor();

                    //書き込むためのCryptoStreamの作成
                    using (CryptoStream cryptStream = new CryptoStream(msOut, desdecrypt, CryptoStreamMode.Write))
                    {
                        //Disposeが重複して呼ばれないようにする
                        MemoryStream msTmp = msOut;
                        msOut = null;
                        desdecrypt = null;

                        //書き込む
                        cryptStream.Write(bytesIn, 0, bytesIn.Length);
                        cryptStream.FlushFinalBlock();
                        //暗号化されたデータを取得
                        var bytesOut = msTmp.ToArray();

                        //Base64で文字列に変更して結果を返す
                        return Convert.ToBase64String(bytesOut);
                    }
                }
                finally
                {
                    if (msOut != null) msOut.Dispose();
                    if (desdecrypt != null) desdecrypt.Dispose();
                }
            }
        }

        public static string DecryptString(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";

            //DESCryptoServiceProviderオブジェクトの作成
            using (var des = new System.Security.Cryptography.DESCryptoServiceProvider())
            {
                //共有キーと初期化ベクタを決定
                //パスワードをバイト配列にする
                var bytesKey = Encoding.UTF8.GetBytes("_tween_encrypt_key_");
                //共有キーと初期化ベクタを設定
                des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
                des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

                //Base64で文字列をバイト配列に戻す
                var bytesIn = Convert.FromBase64String(str);

                MemoryStream msIn = null;
                ICryptoTransform desdecrypt = null;
                CryptoStream cryptStreem = null;

                try
                {
                    //暗号化されたデータを読み込むためのMemoryStream
                    msIn = new MemoryStream(bytesIn);
                    //DES復号化オブジェクトの作成
                    desdecrypt = des.CreateDecryptor();
                    //読み込むためのCryptoStreamの作成
                    cryptStreem = new CryptoStream(msIn, desdecrypt, CryptoStreamMode.Read);

                    //Disposeが重複して呼ばれないようにする
                    msIn = null;
                    desdecrypt = null;

                    //復号化されたデータを取得するためのStreamReader
                    using (StreamReader srOut = new StreamReader(cryptStreem, Encoding.UTF8))
                    {
                        //Disposeが重複して呼ばれないようにする
                        cryptStreem = null;

                        //復号化されたデータを取得する
                        var result = srOut.ReadToEnd();

                        return result;
                    }
                }
                finally
                {
                    if (msIn != null) msIn.Dispose();
                    if (desdecrypt != null) desdecrypt.Dispose();
                    if (cryptStreem != null) cryptStreem.Dispose();
                }
            }
        }

        public static byte[] ResizeBytesArray(byte[] bytes,
                                    int newSize)
        {
            var newBytes = new byte[newSize];
            if (bytes.Length <= newSize)
            {
                foreach (var i in Enumerable.Range(0, bytes.Length))
                {
                    newBytes[i] = bytes[i];
                }
            }
            else
            {
                var pos = 0;
                foreach (var i in Enumerable.Range(0, bytes.Length))
                {
                    newBytes[pos] = unchecked((byte)(newBytes[pos] ^ bytes[i]));
                    pos++;
                    if (pos >= newBytes.Length)
                    {
                        pos = 0;
                    }
                }
            }
            return newBytes;
        }

        public static bool IsNT6()
        {
            //NT6 kernelかどうか検査
            return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major == 6;
        }

        [FlagsAttribute()]
        public enum TabUsageType
        {
            Undefined = 0,
            Home = 1,      //Unique
            Mentions = 2,     //Unique
            DirectMessage = 4,   //Unique
            Favorites = 8,       //Unique
            UserDefined = 16,
            LocalQuery = 32,      //Pin(no save/no save query/distribute/no update(normal update))
            Profile = 64,         //Pin(save/no distribute/manual update)
            PublicSearch = 128,    //Pin(save/no distribute/auto update)
            Lists = 256,
            Related = 512,
            UserTimeline = 1024,
            //RTMyTweet
            //RTByOthers
            //RTByMe
        }

        public static string fileVersion = "";

        public static string GetUserAgentString()
        {
            if (string.IsNullOrEmpty(fileVersion))
            {
                throw new Exception("fileversion is not Initialized.");
            }
            return GetAssemblyName() + "/" + fileVersion;
        }

        public static ApiInformation TwitterApiInfo = new ApiInformation();

        public static bool IsAnimatedGif(string filename)
        {
            Image img = null;
            try
            {
                img = Image.FromFile(filename);
                if (img == null) return false;
                if (img.RawFormat.Guid == ImageFormat.Gif.Guid)
                {
                    var fd = new FrameDimension(img.FrameDimensionsList[0]);
                    var fd_count = img.GetFrameCount(fd);
                    if (fd_count > 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (img != null) img.Dispose();
            }
        }

        public static DateTime DateTimeParse(string input)
        {
            DateTime rslt;
            string[] format = {
                "ddd MMM dd HH:mm:ss zzzz yyyy",
            };
            foreach (var fmt in format)
            {
                if (DateTime.TryParseExact(input,
                                          fmt,
                                          DateTimeFormatInfo.InvariantInfo,
                                          DateTimeStyles.None,
                                          out rslt))
                {
                    return rslt;
                }
                else
                {
                    continue;
                }
            }
            TraceOut("Parse Error(DateTimeFormat) : " + input);
            return new DateTime();
        }

        public static T CreateDataFromJson<T>(string content)
        {
            T data;
            using (var stream = new MemoryStream())
            {
                var buf = Encoding.Unicode.GetBytes(content);
                stream.Write(Encoding.Unicode.GetBytes(content), offset: 0, count: buf.Length);
                stream.Seek(offset: 0, loc: SeekOrigin.Begin);
                data = (T)((new DataContractJsonSerializer(typeof(T))).ReadObject(stream));
            }
            return data;
        }

        public static bool IsNetworkAvailable()
        {
            try
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
            catch(Exception)
            {
                return false;
            }
        }

        static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        /// <summary>
        /// 指定された修飾キーが押されている状態かを取得します。
        /// </summary>
        /// <param name="keys">状態を調べるキー</param>
        /// <returns><paramref name="keys"/> で指定された修飾キーがすべて押されている状態であれば true。それ以外であれば false。</returns>
        public static bool IsKeyDown(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if ((Control.ModifierKeys & key) != key)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// アプリケーションのアセンブリ名を取得します。
        /// </summary>
        /// <remarks>
        /// VB.NETの<code>My.Application.Info.AssemblyName</code>と（ほぼ）同じ動作をします。
        /// </remarks>
        /// <returns>アプリケーションのアセンブリ名</returns>
        public static string GetAssemblyName()
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }

        /// <summary>
        /// 文字列中に含まれる %AppName% をアプリケーション名に置換する
        /// </summary>
        /// <param name="orig">対象となる文字列</param>
        /// <returns>置換後の文字列</returns>
        public static string ReplaceAppName(string orig)
        {
            return orig.Replace("%AppName%", Application.ProductName);
        }

        /// <summary>
        /// 表示用のバージョン番号の文字列を生成する
        /// </summary>
        /// <remarks>
        /// バージョン1.0.0.1のように末尾が0でない（＝開発版）の場合は「1.0.1-beta1」が出力される
        /// </remarks>
        /// <returns>
        /// 生成されたバージョン番号の文字列
        /// </returns>
        public static string GetReadableVersion(string fileVersion = null)
        {
            if (fileVersion == null)
            {
                fileVersion = MyCommon.fileVersion;
            }

            if (string.IsNullOrEmpty(fileVersion))
            {
                return null;
            }

            int[] version = fileVersion.Split('.')
                .Select(x => int.Parse(x)).ToArray();

            if (version[3] == 0)
            {
                return string.Format("{0}.{1}.{2}", version[0], version[1], version[2]);
            }
            else
            {
                version[2] = version[2] + 1;

                // 10を越えたら桁上げ
                if (version[2] > 10)
                {
                    version[1] += version[2] / 10;
                    version[2] %= 10;

                    if (version[1] > 10)
                    {
                        version[0] += version[1] / 10;
                        version[1] %= 10;
                    }
                }

                return string.Format("{0}.{1}.{2}-beta{3}", version[0], version[1], version[2], version[3]);
            }
        }

        public const string TwitterUrl = "https://twitter.com/";

        public static string GetStatusUrl(PostClass post)
        {
            if (post.RetweetedId == 0)
                return GetStatusUrl(post.ScreenName, post.StatusId);
            else
                return GetStatusUrl(post.ScreenName, post.RetweetedId);
        }

        public static string GetStatusUrl(string screenName, long statusId)
        {
            return TwitterUrl + screenName + "/status/" + statusId.ToString();
        }
    }
}
