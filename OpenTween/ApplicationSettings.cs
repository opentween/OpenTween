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
        // =====================================================================
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

        // =====================================================================
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

        // =====================================================================
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

        // =====================================================================
        // アップデートチェック関連

        /// <summary>
        /// 最新バージョンの情報を取得するためのURL
        /// </summary>
        /// <remarks>
        /// version.txt のフォーマットについては http://sourceforge.jp/projects/opentween/wiki/VersionTxt を参照。
        /// 派生プロジェクトなどでこの機能を無効にする場合は null をセットして下さい。
        /// </remarks>
        public static readonly string VersionInfoUrl = "https://www.opentween.org/status/version.txt";

        // =====================================================================
        // 暗号化キー

        /// <summary>
        /// APIキーの暗号化・復号に使用するパスワード
        /// </summary>
        public static readonly string EncryptionPassword = ApplicationName;

        // =====================================================================
        // Twitter
        // https://developer.twitter.com/ から取得できます。

        /// <summary>
        /// Twitter API Key
        /// </summary>
        public static readonly ApiKey TwitterConsumerKey = ApiKey.Create("%e%EGv00cec1j05bMzbfAnsug==%ApO4PjZvnhixxFDqNh5qeqUTyAIEhoDvhhTaspVVjoA=%48Wxazf5JpDdIbkG0I8nme6/weUdEmZMRqfXAmCNr+Y=");

        /// <summary>
        /// Twitter API Key Secret
        /// </summary>
        public static readonly ApiKey TwitterConsumerSecret = ApiKey.Create("%e%p93BdDzlwbYIC5Ych/47OQ==%xYZTCYaBxzS4An3o7Qcigjp9QMtu5vi5iEAW/sNgoOoAUyuHJRPP3Ovs20ZV2fAYKxUDiu76dxLfObwI7QjSRA==%YEruRDAQdbJzO+y6kn7+U/uIyIyNra/8Ulo+L6KJcWA=");

        // =====================================================================
        // Foursquare
        // https://developer.foursquare.com/ から取得できます。

        /// <summary>
        /// Foursquare Client Id
        /// </summary>
        public static readonly ApiKey FoursquareClientId = ApiKey.Create("%e%zNX0U6Eul2bdpkzQgcd/WA==%koGoTYoVLvQ6T5bsnI+dJjJ+REMnlJIhTnibhI6MagmHPA5hzY6DQom+PpQxBZX2CS2fZW1Ac82HzTzb3J7gNw==%qOMdPQ3UN7IWT4jJegYQnRxorZm/zaEt7/n6fd1eFcM=");

        /// <summary>
        /// Foursquare Client Secret
        /// </summary>
        public static readonly ApiKey FoursquareClientSecret = ApiKey.Create("%e%3Zw+G9P32WgIi1ooBeDSCg==%coygRxFBSrxMW5o4dkD64ftx0C7axFkJlceb52XqDaiQKkf8q0Szavcw+t3MsxFQTkwL3ob7mehmCSdSpJPLTA==%XTrAyfrDBzdeKGPIXTD2JFU65uZqh8fs9tyf+fLaJeY=");

        // =====================================================================
        // bit.ly
        // https://bitly.com/a/oauth_apps から取得できます。

        /// <summary>
        /// bit.ly Client ID
        /// </summary>
        public static readonly ApiKey BitlyClientId = ApiKey.Create("%e%VtBYaO301PMy/eesjAJwuQ==%wK9fwaXlaq2wOCbJqzfwUzf4h+xAUJ445+wjI6uT3ANyag2LPJBYpfCQ3V+qnNlR%kmyyVMp8JPubACjFEd1zZMV2bddMimSj86e/ONegTeI=");

        /// <summary>
        /// bit.ly Client Secret
        /// </summary>
        public static readonly ApiKey BitlyClientSecret = ApiKey.Create("%e%jvdQqgh0Aj4e8HANczWELA==%FHevK6YJ/5L083pCJnUQ65bfz+sbwLZw4hUXIOw9PXRa8YnmVZ2KMwuHRRbdZUZl%1f/0LoSAuC0wyfKMhiohXNPGWY7wyQbqr2XVyKtjpFk=");

        // =====================================================================
        // TINAMI
        // http://www.tinami.com/api/ から取得できます。

        /// <summary>
        /// TINAMI APIキー
        /// </summary>
        public static readonly ApiKey TINAMIApiKey = ApiKey.Create("%e%OA+C4u5v/9Oc2xIOEKt6Ng==%xLrMgeXaYSIaW5LYEA2viA==%/0bWO/UMCYH2BRxPKvi7JCW/CXxp/JmincpFNRlvhOQ=");

        // =====================================================================
        // Microsoft Translator API (Cognitive Service)
        // https://www.microsoft.com/ja-jp/translator/getstarted.aspx から取得できます。

        /// <summary>
        /// Translator Text API Subscription Key
        /// </summary>
        public static readonly ApiKey TranslatorSubscriptionKey = ApiKey.Create("%e%ysNsopqjk7fEikJNfX3ZtQ==%ZS4Gzq9PSUzBGKabQWi5To2nVLl8R3D+i/7nj5dACFAkBvQkxmrgJCnvON9cdF/+%1bayEnkpd4gbcyTTwB9PILWdku3YS52FuHskkpIhlXQ=");

        // =====================================================================
        // Imgur
        // https://api.imgur.com/oauth2/addclient から取得できます

        /// <summary>
        /// Imgur Client ID
        /// </summary>
        public static readonly ApiKey ImgurClientId = ApiKey.Create("%e%YQirOk0lRw8zjhrTPlyRrQ==%ysNbeo//DruosSIzXquwiw==%126uNNTZshuw/SW9hd1ME/KCMpKSfjeboSRtQRfVQwc=");

        /// <summary>
        /// Imgur Client Secret
        /// </summary>
        public static readonly ApiKey ImgurClientSecret = ApiKey.Create("%e%+IHTR8uXh2Mig6zGkawjeA==%vYQDZ+Tw5Rol4mQRC/yzXywuaNPWgoL5kLIg5VdUiCnSX5EPGQC5+QUkTzGz/fqv%+mOv8ekXLoZKe05yyXJ8Sx0cjuimoPQA30k/571wVRw=");

        // =====================================================================
        // Mobypicture
        // http://www.mobypicture.com/apps/my から取得できます

        /// <summary>
        /// Mobypicture Developer Key
        /// </summary>
        public static readonly ApiKey MobypictureKey = ApiKey.Create("%e%1FQ6vgxHCduEDyyJ7UK1ug==%xNBXVlAgJ8XiXZ1LSgGvEL1Vk1m4EzQ+6T/yigTgRHw=%xCAlJ1gBpe8G1Us6wXZBkYGxccFvRyRhmnFM4ocuhHQ=");

        // =====================================================================
        // Tumblr
        // https://www.tumblr.com/oauth/apps から取得できます

        /// <summary>
        /// Tumblr OAuth Consumer Key
        /// </summary>
        public static readonly ApiKey TumblrConsumerKey = ApiKey.Create("%e%1jQuOn2+l8O9i0/ld/VV5Q==%SiBRxJWj4Cbq/btPs63Rr2xOyw4XgTlV+lVkHkvIapBRkUaqGJxV7R/j5ljX+QW0ruBXNy+cpZFeEZFfB2jvSQ==%3GdYHow0pAlPQRY/wquX1AaxjzSabStRj8zbTXct1S4=");
    }
}
