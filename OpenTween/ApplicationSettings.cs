// OpenTween - Client of Twitter
// Copyright (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenTween
{
    /// <summary>
    /// アプリケーション固有の情報を格納します
    /// </summary>
    /// <remarks>
    /// OpenTween の派生版を作る方法は http://sourceforge.jp/projects/opentween/wiki/HowToFork を参照して下さい。
    /// </remarks>
    internal static class ApplicationSettings
    {
        //=====================================================================
        // アプリケーション情報

        /// <summary>
        /// アプリケーション名
        /// </summary>
        /// <remarks>
        /// 派生版のアプリケーションでは名前にマルチバイト文字を含む場合があります。
        /// ファイル名など英数字のみを含めたい用途ではこのプロパティではなく <see cref="AssemblyName"/> を使用します
        /// </remarks>
        public static string ApplicationName => Application.ProductName;

        /// <summary>
        /// アセンブリ名
        /// </summary>
        public static string AssemblyName => MyCommon.GetAssemblyName();

        //=====================================================================
        // フィードバック送信先
        // 異常終了時などにエラーログ等とともに表示されます。
        
        /// <summary>
        /// フィードバック送信先 (メール)
        /// </summary>
        public const string FeedbackEmailAddress = "kim.upsilon@bucyou.net";

        /// <summary>
        /// フィードバック送信先 (Twitter)
        /// </summary>
        public const string FeedbackTwitterName = "@OpenTween";

        /// <summary>
        /// FeedbackTwitterName のユーザー宛にエラーレポートの DM を送信可能であるか
        /// </summary>
        /// <remarks>
        /// エラーレポートを DM で受け付ける場合は、フォロー外からの DM を受け付ける設定にする必要があります
        /// </remarks>
        public static readonly bool AllowSendErrorReportByDM = true;

        //=====================================================================
        // Web サイト

        /// <summary>
        /// 「ヘルプ」メニューの「(アプリ名) ウェブサイト」クリック時に外部ブラウザで表示する URL
        /// </summary>
        public const string WebsiteUrl = "https://www.opentween.org/";

        /// <summary>
        /// 「ヘルプ」メニューの「ショートカットキー一覧」クリック時に外部ブラウザで表示する URL
        /// </summary>
        /// <remarks>
        /// Tween の Wiki ページのコンテンツはプロプライエタリなため転載不可
        /// </remarks>
        public const string ShortcutKeyUrl = "https://ja.osdn.net/projects/tween/wiki/%E3%82%B7%E3%83%A7%E3%83%BC%E3%83%88%E3%82%AB%E3%83%83%E3%83%88%E3%82%AD%E3%83%BC";

        //=====================================================================
        // アップデートチェック関連

        /// <summary>
        /// 最新バージョンの情報を取得するためのURL
        /// </summary>
        /// <remarks>
        /// version.txt のフォーマットについては http://sourceforge.jp/projects/opentween/wiki/VersionTxt を参照。
        /// 派生プロジェクトなどでこの機能を無効にする場合は null をセットして下さい。
        /// </remarks>
        public static readonly string VersionInfoUrl = "https://www.opentween.org/status/version.txt";

        //=====================================================================
        // 暗号化キー

        /// <summary>
        /// APIキーの暗号化・復号に使用するパスワード
        /// </summary>
        public static readonly string EncryptionPassword = ApplicationName;

        //=====================================================================
        // Twitter
        // https://developer.twitter.com/ から取得できます。

        /// <summary>
        /// Twitter API Key
        /// </summary>
        public static readonly ApiKey TwitterConsumerKey = ApiKey.Create("%e%qDp/VaX9FqN86TBznspjyg==%idrCd4S0jPTfFNhqTc21XT/vADksqLIj9x7MO13O2sQ=%/Zf9x95CNa6cQTXUYcrqR+poU31lZysqmdEqorhruWo=");

        /// <summary>
        /// Twitter API Key Secret
        /// </summary>
        public static readonly ApiKey TwitterConsumerSecret = ApiKey.Create("%e%ocBkKu4aZI5PsbboE7+Ajg==%4IlCFjqusmFPQHbcDutms/1bTOSwgxd3ah1NPCj23WUu66nGjUb5oAEMiNSv41Pb6n5utbFQrgqwJRdl3jVnfQ==%jpx+NlcN1w8UQ8kda1LgyHEmiVwl/PLGCVPotrp6pd8=");

        //=====================================================================
        // Foursquare
        // https://developer.foursquare.com/ から取得できます。

        /// <summary>
        /// Foursquare Client Id
        /// </summary>
        public static readonly ApiKey FoursquareClientId = ApiKey.Create("%e%KyanhtUu0qXIU2AnLkvPnQ==%S/xBHNqFOxoxzvm78ELIxihdzq+Hu7iXboWQEIPztc928zVY/jJ50Qyc/ic/twGzBkdz/Mz+63wfoU/u7jDg4A==%zJDIOdYGgbQ5gJNvBqGl1ETVok7IJrBB/nXxvcUCuPA=");

        /// <summary>
        /// Foursquare Client Secret
        /// </summary>
        public static readonly ApiKey FoursquareClientSecret = ApiKey.Create("%e%YZeT/G9cZ0Lub68LFU15bw==%x9LQxogt6ejhWOAV1toXn0zeDeBpV0lMEmJGRCpsIrizJNl3kDcDKWGu1CYSOXgk2hAoqm5IOBq4RAExE0Z2Qw==%yMdCK6WJo7WmvAYBIJ+qZrxAKVIPOR3nftfoXzyzlgY=");

        //=====================================================================
        // bit.ly
        // https://bitly.com/a/oauth_apps から取得できます。

        /// <summary>
        /// bit.ly Client ID
        /// </summary>
        public static readonly ApiKey BitlyClientId = ApiKey.Create("%e%Yg3QHOAzEo189O5ujHbizA==%CnzHsBl4mUTFf2wJr9cRYd2OzDnnlLux48xhWT0hdkDz+XgGeiBCqZ/L17rGze4r%VCZDb+LcYtPviR0H1QYnRDMugrhL9MXicw3yt2jKAPA=");

        /// <summary>
        /// bit.ly Client Secret
        /// </summary>
        public static readonly ApiKey BitlyClientSecret = ApiKey.Create("%e%NOCzRjTNRC64Hx0d6e4spA==%XJjn/yAsTsmtgLMdFpjptss66DFh15nvV+Ff2omHxYk0tKyc5Wn5qFxVquAQ4Yg3%yJnMWwcs/FfTYJZ1Wg3r7m0TMogAPj85ViUXImom890=");

        //=====================================================================
        // TINAMI
        // http://www.tinami.com/api/ から取得できます。

        /// <summary>
        /// TINAMI APIキー
        /// </summary>
        public static readonly ApiKey TINAMIApiKey = ApiKey.Create("%e%5wz/IYAfWvY9y731F3yCIQ==%7y8i0qD9AF4DqFWjY1zn1w==%eVU155W/1sr3ZPDcuRMGTpSQyGXF4egWFto/HzBdGJ4=");

        //=====================================================================
        // Microsoft Translator API (Cognitive Service)
        // https://www.microsoft.com/ja-jp/translator/getstarted.aspx から取得できます。

        /// <summary>
        /// Translator Text API Subscription Key
        /// </summary>
        public static readonly ApiKey TranslatorSubscriptionKey = ApiKey.Create("%e%N0EPwqCbM0qiNX4h7VsrXQ==%uOf/IdH2RO6fTgrhrvXuJJ7IT+R44aS7ROY3aQFCqqrLHru4fZh2hJAEoAI239BY%p26g6G/ANsAf+1Xq/iLE2zuTwA4ok/zZ61SQkvqqTZ8=");

        //=====================================================================
        // Imgur
        // https://api.imgur.com/oauth2/addclient から取得できます

        /// <summary>
        /// Imgur Client ID
        /// </summary>
        public static readonly ApiKey ImgurClientId = ApiKey.Create("%e%kNnrm5hWWwTPZ9MgqF9osg==%yCbBWxtlZmzdBgR1v57+uQ==%FHJAyKZ7w4c0OWzgU86nq7p2J5trJVjOjdFSOfIArU0=");

        /// <summary>
        /// Imgur Client Secret
        /// </summary>
        public static readonly ApiKey ImgurClientSecret = ApiKey.Create("%e%nLVIw/raU3ozrGmkfIk3Ig==%2iKGe1reB5p6VHkvrMkH1w==%lwwvuejEuy0eZZ9nS4BT1Jw7S7pkLktGPKzsfQErttw=");

        //=====================================================================
        // Mobypicture
        // http://www.mobypicture.com/apps/my から取得できます

        /// <summary>
        /// Mobypicture Developer Key
        /// </summary>
        public static readonly ApiKey MobypictureKey = ApiKey.Create("%e%G9elTyjHy18MCbUvVqHKIw==%TDUSyoO4HS5SX+t50cUlRQ5tIFDib0xjsnCKX+K/+DI=%s2qPqrxXrmi8oeQWoeigqNDbecUAqcYuv2LPRFDLwJk=");

        //=====================================================================
        // Tumblr
        // https://www.tumblr.com/oauth/apps から取得できます

        /// <summary>
        /// Tumblr OAuth Consumer Key
        /// </summary>
        public static readonly ApiKey TumblrConsumerKey = ApiKey.Create("%e%YEjYH0khBP52A9mbP6XMQQ==%V2B5KP9JJrE0A/o1+9fyq1OCnM4Ez1D8Qd3sVaeRKmvNIq8pY76pQB1R2PFMMI1nLZCCUH2PyA959490ujuvig==%GD8FiQzBOUM9lQta+1oTYs1ANE9Qp7+2XamfyQGmiMs=");
    }
}
