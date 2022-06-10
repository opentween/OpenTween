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

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using OpenTween.Api;
using OpenTween.Models;
using OpenTween.Setting;

namespace OpenTween
{
    public static class MyCommon
    {
        private static readonly object LockObj = new();

        public static bool EndingFlag { get; set; } // 終了フラグ

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
            Bitly,
            Jmp,
            Uxnu,
            // 特殊
            Nicoms,
            // 廃止
            Unu = -1,
            Twurl = -1,
        }

        public enum ListItemDoubleClickActionType
        {
            // 設定ファイルの互換性を保つため新規の項目は途中に追加しないこと
            Reply,
            Favorite,
            ShowProfile,
            ShowTimeline,
            ShowRelated,
            OpenHomeInBrowser,
            OpenStatusInBrowser,
            None,
            ReplyAll,
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
            MaxValue = 1000,
            DefaultValue = 20,
        }

        // Backgroundworkerへ処理種別を通知するための引数用enum
        public enum WORKERTYPE
        {
            Timeline, // タイムライン取得
            Reply, // 返信取得
            DirectMessegeRcv, // 受信DM取得
            DirectMessegeSnt, // 送信DM取得
            PostMessage, // 発言POST
            FavAdd, // Fav追加
            FavRemove, // Fav削除
            Follower, // Followerリスト取得
            Favorites, // Fav取得
            Retweet, // Retweetする
            PublicSearch, // 公式検索
            List, // Lists
            Related, // 関連発言
            UserTimeline, // UserTimeline
            BlockIds, // Blocking/ids
            Configuration, // Twitter Configuration読み込み
            NoRetweetIds, // RT非表示ユーザー取得
            //////
            ErrorState, // エラー表示のみで後処理終了(認証エラー時など)
        }

        public static class DEFAULTTAB
        {
            public const string RECENT = "Recent";
            public const string REPLY = "Reply";
            public const string DM = "Direct";
            public const string FAV = "Favorites";
            public static readonly string MUTE = Properties.Resources.MuteTabName;
        }

        public static readonly object? Block = null;
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

        public static _Assembly EntryAssembly { get; internal set; }

        public static string FileVersion { get; internal set; }

        static MyCommon()
        {
            var assembly = Assembly.GetExecutingAssembly();
            MyCommon.EntryAssembly = assembly;

            var assemblyVersion = assembly.GetName().Version;
            MyCommon.FileVersion = assemblyVersion.ToString();
        }

        public static string GetErrorLogPath()
            => Path.Combine(Path.GetDirectoryName(MyCommon.EntryAssembly.Location), "ErrorLogs");

        public static void TraceOut(WebApiException ex)
        {
            var message = ExceptionOutMessage(ex);

            if (ex.ResponseText != null)
                message += Environment.NewLine + "------- Response Data -------" + Environment.NewLine + ex.ResponseText;

            TraceOut(TraceFlag, message);
        }

        public static void TraceOut(Exception ex, string message)
        {
            var buf = ExceptionOutMessage(ex);
            TraceOut(TraceFlag, message + Environment.NewLine + buf);
        }

        public static void TraceOut(string message)
            => TraceOut(TraceFlag, message);

        public static void TraceOut(bool outputFlag, string message)
        {
            lock (LockObj)
            {
                if (!outputFlag) return;

                var logPath = MyCommon.GetErrorLogPath();
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                var now = DateTimeUtc.Now;
                var fileName = $"{ApplicationSettings.AssemblyName}Trace-{now.ToLocalTime():yyyyMMdd-HHmmss}.log";
                fileName = Path.Combine(logPath, fileName);

                using var writer = new StreamWriter(fileName);

                writer.WriteLine("**** TraceOut: {0} ****", now.ToLocalTimeString());
                writer.WriteLine(Properties.Resources.TraceOutText1, ApplicationSettings.FeedbackEmailAddress);
                writer.WriteLine(Properties.Resources.TraceOutText2, ApplicationSettings.FeedbackTwitterName);
                writer.WriteLine();
                writer.WriteLine(Properties.Resources.TraceOutText3);
                writer.WriteLine(Properties.Resources.TraceOutText4, Environment.OSVersion.VersionString);
                writer.WriteLine(Properties.Resources.TraceOutText5, Environment.Version);
                writer.WriteLine(Properties.Resources.TraceOutText6, ApplicationSettings.AssemblyName, FileVersion);
                writer.WriteLine(message);
                writer.WriteLine();
            }
        }

        // エラー内容をバッファに書き出し
        // 注意：最終的にファイル出力されるエラーログに記録されるため次の情報は書き出さない
        // 文頭メッセージ、権限、動作環境
        // Dataプロパティにある終了許可フラグのパースもここで行う

        public static string ExceptionOutMessage(Exception ex)
        {
            var isTerminatePermission = true;
            return ExceptionOutMessage(ex, ref isTerminatePermission);
        }

        public static string ExceptionOutMessage(Exception ex, ref bool isTerminatePermission)
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
                        isTerminatePermission = (bool)dt.Value;
                    }
                }
                if (!needHeader)
                {
                    buf.AppendLine("-----End Extra Information-----");
                }
            }
            buf.AppendLine(ex.StackTrace);
            buf.AppendLine();

            // InnerExceptionが存在する場合書き出す
            var innerException = ex.InnerException;
            var nesting = 0;
            while (innerException != null)
            {
                buf.AppendFormat("-----InnerException[{0}]-----\r\n", nesting);
                buf.AppendLine();
                buf.AppendFormat(Properties.Resources.UnhandledExceptionText8, innerException.GetType().FullName, innerException.Message);
                buf.AppendLine();
                if (innerException.Data != null)
                {
                    var needHeader = true;

                    foreach (DictionaryEntry dt in innerException.Data)
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
                            isTerminatePermission = (bool)dt.Value;
                        }
                    }
                    if (!needHeader)
                    {
                        buf.AppendLine("-----End Extra Information-----");
                    }
                }
                buf.AppendLine(innerException.StackTrace);
                buf.AppendLine();
                nesting++;
                innerException = innerException.InnerException;
            }
            return buf.ToString();
        }

        public static bool ExceptionOut(Exception ex)
        {
            lock (LockObj)
            {
                var isTerminatePermission = true;

                var ident = WindowsIdentity.GetCurrent();
                var princ = new WindowsPrincipal(ident);
                var now = DateTimeUtc.Now;

                var errorReport = string.Join(Environment.NewLine,
                    string.Format(Properties.Resources.UnhandledExceptionText1, now.ToLocalTimeString()),

                    // 権限書き出し
                    string.Format(Properties.Resources.UnhandledExceptionText11 + princ.IsInRole(WindowsBuiltInRole.Administrator)),
                    string.Format(Properties.Resources.UnhandledExceptionText12 + princ.IsInRole(WindowsBuiltInRole.User)),
                    "",

                    // OSVersion,AppVersion書き出し
                    string.Format(Properties.Resources.UnhandledExceptionText4),
                    string.Format(Properties.Resources.UnhandledExceptionText5, Environment.OSVersion.VersionString),
                    string.Format(Properties.Resources.UnhandledExceptionText6, Environment.Version),
                    string.Format(Properties.Resources.UnhandledExceptionText7, ApplicationSettings.AssemblyName, FileVersion),

                    ExceptionOutMessage(ex, ref isTerminatePermission));

                var logPath = MyCommon.GetErrorLogPath();
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                var fileName = $"{ApplicationSettings.AssemblyName}-{now.ToLocalTime():yyyyMMdd-HHmmss}.log";
                using (var writer = new StreamWriter(Path.Combine(logPath, fileName)))
                {
                    writer.Write(errorReport);
                }

                var settings = SettingManager.Instance;
                var mainForm = Application.OpenForms.OfType<TweenMain>().FirstOrDefault();

                ErrorReport report;
                if (mainForm != null && !mainForm.IsDisposed)
                    report = new ErrorReport(mainForm.TwitterInstance, errorReport);
                else
                    report = new ErrorReport(errorReport);

                report.AnonymousReport = settings.Common.ErrorReportAnonymous;

                OpenErrorReportDialog(mainForm, report);

                // ダイアログ内で設定が変更されていれば保存する
                if (settings.Common.ErrorReportAnonymous != report.AnonymousReport)
                {
                    settings.Common.ErrorReportAnonymous = report.AnonymousReport;
                    settings.SaveCommon();
                }

                return false;
            }
        }

        private static void OpenErrorReportDialog(Form? owner, ErrorReport report)
        {
            if (owner != null && owner.InvokeRequired)
            {
                owner.Invoke((Action)(() => OpenErrorReportDialog(owner, report)));
                return;
            }

            using var dialog = new SendErrorReportForm();
            dialog.ErrorReport = report;
            dialog.ShowDialog(owner);
        }

        /// <summary>
        /// URLのドメイン名をPunycode展開します。
        /// <para>
        /// ドメイン名がIDNでない場合はそのまま返します。
        /// ドメインラベルの区切り文字はFULLSTOP(.、U002E)に置き換えられます。
        /// </para>
        /// </summary>
        /// <param name="inputUrl">展開対象のURL</param>
        /// <returns>IDNが含まれていた場合はPunycodeに展開したURLをを返します。Punycode展開時にエラーが発生した場合はnullを返します。</returns>
        public static string? IDNEncode(string inputUrl)
        {
            try
            {
                var uriBuilder = new UriBuilder(inputUrl);

                var idnConverter = new IdnMapping();
                uriBuilder.Host = idnConverter.GetAscii(uriBuilder.Host);

                return uriBuilder.Uri.AbsoluteUri;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string IDNDecode(string inputUrl)
        {
            try
            {
                var uriBuilder = new UriBuilder(inputUrl);

                if (uriBuilder.Host != null)
                {
                    var idnConverter = new IdnMapping();
                    uriBuilder.Host = idnConverter.GetUnicode(uriBuilder.Host);
                }

                return uriBuilder.Uri.AbsoluteUri;
            }
            catch (Exception)
            {
                return inputUrl;
            }
        }

        /// <summary>
        /// URL を画面上で人間に読みやすい文字列に変換する（エスケープ解除など）
        /// </summary>
        public static string ConvertToReadableUrl(string inputUrl)
        {
            try
            {
                var outputUrl = inputUrl;

                // Punycodeをデコードする
                outputUrl = MyCommon.IDNDecode(outputUrl);

                // URL内で特殊な意味を持つ記号は元の文字に変換されることを避けるために二重エスケープする
                // 参考: Firefoxの losslessDecodeURI() 関数
                //   http://hg.mozilla.org/mozilla-central/annotate/FIREFOX_AURORA_27_BASE/browser/base/content/browser.js#l2128
                outputUrl = Regex.Replace(outputUrl, @"%(2[3456BCF]|3[ABDF]|40)", @"%25$1", RegexOptions.IgnoreCase);

                // エスケープを解除する
                outputUrl = Uri.UnescapeDataString(outputUrl);

                return outputUrl;
            }
            catch (UriFormatException)
            {
                return inputUrl;
            }
        }

        public static void MoveArrayItem(int[] values, int idx_fr, int idx_to)
        {
            var moved_value = values[idx_fr];
            var num_moved = Math.Abs(idx_fr - idx_to);

            if (idx_to < idx_fr)
            {
                Array.Copy(values, idx_to, values, idx_to + 1, num_moved);
            }
            else
            {
                Array.Copy(values, idx_fr + 1, values, idx_fr, num_moved);
            }

            values[idx_to] = moved_value;
        }

        public static string EncryptString(string str)
        {
            if (MyCommon.IsNullOrEmpty(str)) return "";

            // 文字列をバイト型配列にする
            var bytesIn = Encoding.UTF8.GetBytes(str);

            // DESCryptoServiceProviderオブジェクトの作成
            using var des = new DESCryptoServiceProvider();

            // 共有キーと初期化ベクタを決定
            // パスワードをバイト配列にする
            var bytesKey = Encoding.UTF8.GetBytes("_tween_encrypt_key_");
            // 共有キーと初期化ベクタを設定
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            MemoryStream? msOut = null;
            ICryptoTransform? desdecrypt = null;

            try
            {
                // 暗号化されたデータを書き出すためのMemoryStream
                msOut = new MemoryStream();

                // DES暗号化オブジェクトの作成
                desdecrypt = des.CreateEncryptor();

                // 書き込むためのCryptoStreamの作成
                using var cryptStream = new CryptoStream(msOut, desdecrypt, CryptoStreamMode.Write);

                // Disposeが重複して呼ばれないようにする
                var msTmp = msOut;
                msOut = null;
                desdecrypt = null;

                // 書き込む
                cryptStream.Write(bytesIn, 0, bytesIn.Length);
                cryptStream.FlushFinalBlock();
                // 暗号化されたデータを取得
                var bytesOut = msTmp.ToArray();

                // Base64で文字列に変更して結果を返す
                return Convert.ToBase64String(bytesOut);
            }
            finally
            {
                msOut?.Dispose();
                desdecrypt?.Dispose();
            }
        }

        public static string DecryptString(string str)
        {
            if (MyCommon.IsNullOrEmpty(str)) return "";

            // DESCryptoServiceProviderオブジェクトの作成
            using var des = new DESCryptoServiceProvider();

            // 共有キーと初期化ベクタを決定
            // パスワードをバイト配列にする
            var bytesKey = Encoding.UTF8.GetBytes("_tween_encrypt_key_");
            // 共有キーと初期化ベクタを設定
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            // Base64で文字列をバイト配列に戻す
            var bytesIn = Convert.FromBase64String(str);

            MemoryStream? msIn = null;
            ICryptoTransform? desdecrypt = null;
            CryptoStream? cryptStreem = null;

            try
            {
                // 暗号化されたデータを読み込むためのMemoryStream
                msIn = new MemoryStream(bytesIn);
                // DES復号化オブジェクトの作成
                desdecrypt = des.CreateDecryptor();
                // 読み込むためのCryptoStreamの作成
                cryptStreem = new CryptoStream(msIn, desdecrypt, CryptoStreamMode.Read);

                // Disposeが重複して呼ばれないようにする
                msIn = null;
                desdecrypt = null;

                // 復号化されたデータを取得するためのStreamReader
                using var srOut = new StreamReader(cryptStreem, Encoding.UTF8);

                // Disposeが重複して呼ばれないようにする
                cryptStreem = null;

                // 復号化されたデータを取得する
                var result = srOut.ReadToEnd();

                return result;
            }
            finally
            {
                msIn?.Dispose();
                desdecrypt?.Dispose();
                cryptStreem?.Dispose();
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

        [Flags]
        public enum TabUsageType
        {
            Undefined = 0,
            Home = 1, // Unique
            Mentions = 2, // Unique
            DirectMessage = 4, // Unique
            Favorites = 8, // Unique
            UserDefined = 16,
            LocalQuery = 32, // Pin(no save/no save query/distribute/no update(normal update))
            Profile = 64, // Pin(save/no distribute/manual update)
            PublicSearch = 128, // Pin(save/no distribute/auto update)
            Lists = 256,
            Related = 512,
            UserTimeline = 1024,
            Mute = 2048,
            SearchResults = 4096,
        }

        public static TwitterApiStatus TwitterApiInfo = new();

        public static bool IsAnimatedGif(string filename)
        {
            Image? img = null;
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
                img?.Dispose();
            }
        }

        public static DateTimeUtc DateTimeParse(string input)
        {
            var formats = new[]
            {
                "ddd MMM dd HH:mm:ss zzzz yyyy",
                "ddd, d MMM yyyy HH:mm:ss zzzz",
            };

            if (DateTimeUtc.TryParseExact(input, formats, DateTimeFormatInfo.InvariantInfo, out var result))
                return result;

            TraceOut("Parse Error(DateTimeFormat) : " + input);

            return DateTimeUtc.Now;
        }

        public static T CreateDataFromJson<T>(string content)
        {
            var buf = Encoding.Unicode.GetBytes(content);
            using var stream = new MemoryStream(buf);
            var settings = new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true,
            };
            return (T)new DataContractJsonSerializer(typeof(T), settings).ReadObject(stream);
        }

        public static bool IsNetworkAvailable()
        {
            try
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsValidEmail(string strIn)
        {
            var pattern = @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$";

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, pattern);
        }

        /// <summary>
        /// 指定された修飾キーが押されている状態かを取得します。
        /// </summary>
        /// <param name="keys">状態を調べるキー</param>
        /// <returns><paramref name="keys"/> で指定された修飾キーがすべて押されている状態であれば true。それ以外であれば false。</returns>
        public static bool IsKeyDown(params Keys[] keys)
            => MyCommon.IsKeyDownInternal(Control.ModifierKeys, keys);

        internal static bool IsKeyDownInternal(Keys modifierKeys, Keys[] targetKeys)
        {
            foreach (var key in targetKeys)
            {
                if ((modifierKeys & key) != key)
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
            => MyCommon.EntryAssembly.GetName().Name;

        /// <summary>
        /// 文字列中に含まれる %AppName% をアプリケーション名に置換する
        /// </summary>
        /// <param name="orig">対象となる文字列</param>
        /// <returns>置換後の文字列</returns>
        public static string ReplaceAppName(string orig)
            => MyCommon.ReplaceAppName(orig, ApplicationSettings.ApplicationName);

        /// <summary>
        /// 文字列中に含まれる %AppName% をアプリケーション名に置換する
        /// </summary>
        /// <param name="orig">対象となる文字列</param>
        /// <param name="appname">アプリケーション名</param>
        /// <returns>置換後の文字列</returns>
        public static string ReplaceAppName(string orig, string appname)
            => orig.Replace("%AppName%", appname);

        /// <summary>
        /// 表示用のバージョン番号の文字列を生成する
        /// </summary>
        /// <remarks>
        /// バージョン1.0.0.1のように末尾が0でない（＝開発版）の場合は「1.0.1-beta1」が出力される
        /// </remarks>
        /// <returns>
        /// 生成されたバージョン番号の文字列
        /// </returns>
        public static string GetReadableVersion(string? versionStr = null)
        {
            var version = Version.Parse(versionStr ?? MyCommon.FileVersion);

            return GetReadableVersion(version);
        }

        /// <summary>
        /// 表示用のバージョン番号の文字列を生成する
        /// </summary>
        /// <remarks>
        /// バージョン1.0.0.1のように末尾が0でない（＝開発版）の場合は「1.0.1-dev」のように出力される
        /// </remarks>
        /// <returns>
        /// 生成されたバージョン番号の文字列
        /// </returns>
        public static string GetReadableVersion(Version version)
        {
            var versionNum = new[] { version.Major, version.Minor, version.Build, version.Revision };

            if (versionNum[3] == 0)
            {
                return string.Format("{0}.{1}.{2}", versionNum[0], versionNum[1], versionNum[2]);
            }
            else
            {
                versionNum[2] = versionNum[2] + 1;

                if (versionNum[3] == 1)
                    return string.Format("{0}.{1}.{2}-dev", versionNum[0], versionNum[1], versionNum[2]);
                else
                    return string.Format("{0}.{1}.{2}-dev+build.{3}", versionNum[0], versionNum[1], versionNum[2], versionNum[3]);
            }
        }

        public const string TwitterUrl = "https://twitter.com/";

        public static string GetStatusUrl(PostClass post)
        {
            if (post.RetweetedId == null)
                return GetStatusUrl(post.ScreenName, post.StatusId);
            else
                return GetStatusUrl(post.ScreenName, post.RetweetedId.Value);
        }

        public static string GetStatusUrl(string screenName, long statusId)
            => TwitterUrl + screenName + "/status/" + statusId;

        /// <summary>
        /// 指定された IDictionary を元にクエリ文字列を生成します
        /// </summary>
        /// <param name="param">生成するクエリの key-value コレクション</param>
        public static string BuildQueryString(IEnumerable<KeyValuePair<string, string>> param)
        {
            if (param == null)
                return string.Empty;

            var query = param
                .Where(x => x.Value != null)
                .Select(x => EscapeQueryString(x.Key) + '=' + EscapeQueryString(x.Value));

            return string.Join("&", query);
        }

        // .NET 4.5+: Reserved characters のうち、Uriクラスによってエスケープ強制解除されてしまうものも最初から Unreserved として扱う
        private static readonly HashSet<char> UnreservedChars =
            new("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~!'()*:");

        /// <summary>
        /// 2バイト文字も考慮したクエリ用エンコード
        /// </summary>
        /// <param name="stringToEncode">エンコードする文字列</param>
        /// <returns>エンコード結果文字列</returns>
        public static string EscapeQueryString(string stringToEncode)
        {
            var sb = new StringBuilder(stringToEncode.Length * 2);

            foreach (var b in Encoding.UTF8.GetBytes(stringToEncode))
            {
                if (UnreservedChars.Contains((char)b))
                    sb.Append((char)b);
                else
                    sb.AppendFormat("%{0:X2}", b);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 指定された範囲の整数を昇順に列挙します
        /// </summary>
        /// <remarks>
        /// start, start + 1, start + 2, ..., end の範囲の数列を生成します
        /// </remarks>
        /// <param name="from">数列の先頭の値 (最小値)</param>
        /// <param name="to">数列の末尾の値 (最大値)</param>
        /// <returns>整数を列挙する IEnumerable インスタンス</returns>
        public static IEnumerable<int> CountUp(int from, int to)
        {
            if (from > to)
                return Enumerable.Empty<int>();

            return Enumerable.Range(from, to - from + 1);
        }

        /// <summary>
        /// 指定された範囲の整数を降順に列挙します
        /// </summary>
        /// <remarks>
        /// start, start - 1, start - 2, ..., end の範囲の数列を生成します
        /// </remarks>
        /// <param name="from">数列の先頭の値 (最大値)</param>
        /// <param name="to">数列の末尾の値 (最小値)</param>
        /// <returns>整数を列挙する IEnumerable インスタンス</returns>
        public static IEnumerable<int> CountDown(int from, int to)
        {
            for (var i = from; i >= to; i--)
                yield return i;
        }

        public static IEnumerable<int> CircularCountUp(int length, int startIndex)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex < 0 || startIndex >= length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            // startindex ... 末尾
            var indices = MyCommon.CountUp(startIndex, length - 1);

            // 先頭 ... (startIndex - 1)
            if (startIndex != 0)
                indices = indices.Concat(MyCommon.CountUp(0, startIndex - 1));

            return indices;
        }

        public static IEnumerable<int> CircularCountDown(int length, int startIndex)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex < 0 || startIndex >= length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            // startIndex ... 先頭
            var indices = MyCommon.CountDown(startIndex, 0);

            // 末尾 ... (startIndex + 1)
            if (startIndex != length - 1)
                indices = indices.Concat(MyCommon.CountDown(length - 1, startIndex + 1));

            return indices;
        }

        /// <summary>
        /// 2バイト文字も考慮したUrlエンコード
        /// </summary>
        /// <param name="stringToEncode">エンコードする文字列</param>
        /// <returns>エンコード結果文字列</returns>
        public static string UrlEncode(string stringToEncode)
        {
            const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            var sb = new StringBuilder();
            var bytes = Encoding.UTF8.GetBytes(stringToEncode);

            foreach (var b in bytes)
            {
                if (UnreservedChars.IndexOf((char)b) != -1)
                    sb.Append((char)b);
                else
                    sb.AppendFormat("%{0:X2}", b);
            }
            return sb.ToString();
        }

        public static bool IsNullOrEmpty([NotNullWhen(false)] string? value)
            => string.IsNullOrEmpty(value);

        public static Task OpenInBrowserAsync(IWin32Window? owner, string url)
            => MyCommon.OpenInBrowserAsync(owner, SettingManager.Instance.Local.BrowserPath, url);

        public static async Task OpenInBrowserAsync(IWin32Window? owner, string? browserPath, string url)
        {
            try
            {
                await Task.Run(() =>
                {
                    var startInfo = MyCommon.CreateBrowserProcessStartInfo(browserPath, url);
                    Process.Start(startInfo);
                });
            }
            catch (Win32Exception ex)
            {
                var message = string.Format(Properties.Resources.BrowserStartFailed, ex.Message);
                MessageBox.Show(owner, message, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static ProcessStartInfo CreateBrowserProcessStartInfo(string? browserPathWithArgs, string url)
        {
            if (MyCommon.IsNullOrEmpty(browserPathWithArgs))
            {
                return new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true,
                };
            }

            var quoteEnd = -1;
            if (browserPathWithArgs.StartsWith("\"", StringComparison.Ordinal))
                quoteEnd = browserPathWithArgs.IndexOf("\"", 1, StringComparison.Ordinal);

            string browserPath, browserArgs;
            var isQuoted = quoteEnd != -1;
            if (isQuoted)
            {
                browserPath = browserPathWithArgs.Substring(1, quoteEnd - 1);
                browserArgs = browserPathWithArgs.Substring(quoteEnd + 1).Trim();
            }
            else
            {
                browserPath = browserPathWithArgs;
                browserArgs = "";
            }

            var quotedUrl = "\"" + url.Replace("\"", "\\\"") + "\"";
            var args = MyCommon.IsNullOrEmpty(browserArgs) ? quotedUrl : browserArgs + " " + quotedUrl;

            return new ProcessStartInfo
            {
                FileName = browserPath,
                Arguments = args,
                UseShellExecute = false,
            };
        }

        public static IEnumerable<(int Start, int End)> ToRangeChunk(IEnumerable<int> values)
        {
            var start = -1;
            var end = -1;

            foreach (var value in values.OrderBy(x => x))
            {
                if (start == -1)
                {
                    start = value;
                    end = value;
                }
                else
                {
                    if (value == end + 1)
                    {
                        end = value;
                    }
                    else
                    {
                        yield return (start, end);
                        start = value;
                        end = value;
                    }
                }
            }

            if (start != -1)
                yield return (start, end);
        }
    }
}
