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
        public static readonly ApiKey TwitterConsumerKey = ApiKey.Create("%e%lhBzdIOsT0NwiaraGF6M8Q==%DtKV/SIVVVGA0tnaubpG0c5VL52DtkndCd5bYNPfQ/M=%4pkaNg/LhtyFPUlhMG9v+2DWuYxlYZSB9md5J/azJbA=");

        /// <summary>
        /// Twitter API Key Secret
        /// </summary>
        public static readonly ApiKey TwitterConsumerSecret = ApiKey.Create("%e%qYUFwFZCk/oYRwkF+d5G1A==%wqJcbfvhm3avxQaFha6hjLZkuo07y0EOUzOfT66NyuR4KOe5V3Jgvw6Kvy5V/VKafZHmq8j6xyKzytQQa7kpUw==%VgfPeI7XK/zhds2Tk/QTo5hPe2/4rRsGnLascX5Lp1I=");

        //=====================================================================
        // Foursquare
        // https://developer.foursquare.com/ から取得できます。

        /// <summary>
        /// Foursquare Client Id
        /// </summary>
        public static readonly ApiKey FoursquareClientId = ApiKey.Create("%e%V/zwwnhz2ubgeXExKHm+0Q==%6QVbfha2Mzj0jgfI53jtup5sTov1T36BDdb6Bj3WeceHY/D3/7ZcqIlgIM/9ao6ZK/UWVq2iedgP826JnHL8Mg==%JROQDGrJaw6g4uFUO0SC/sgYgyXhbE/N82zJvYoOx+A=");

        /// <summary>
        /// Foursquare Client Secret
        /// </summary>
        public static readonly ApiKey FoursquareClientSecret = ApiKey.Create("%e%OHLwPR511lrD5I/7q3uwZA==%Qr1q996MLlPhlo0tWCNHsU44sU1Q8MCO23I/fWkJE/gQha1RpEVcrdRXIiCzNC/Z3dKPSZZvPxinOCjOIDTSUw==%NPBn8TnvT24FAPZvAnwhNFGXZ+MB3YCZ6QLplivNtDE=");

        //=====================================================================
        // bit.ly
        // https://bitly.com/a/oauth_apps から取得できます。

        /// <summary>
        /// bit.ly Client ID
        /// </summary>
        public static readonly ApiKey BitlyClientId = ApiKey.Create("%e%D6d+cV9roc9oRyFHlzzpNA==%5PU2keurc+lfK56rsbEftpHz9S/X4xVg8vRKLXHksspOtOeLXHlR55XdPX4vafIr%X5bBNiqWcqeIThNZmDkz3rM2ObtIYPqVV5ijCg4rlME=");

        /// <summary>
        /// bit.ly Client Secret
        /// </summary>
        public static readonly ApiKey BitlyClientSecret = ApiKey.Create("%e%p3whdM7O13e3Wos1EBCgZg==%uaZQscha5IYHVIwedXo4klPBMxzyFmJwB4cO3+hzTVm7KGhnxsHh44aoLfLTzuzI%mqFsvWt7k7ps2HcCjxwbZS66lGcFHae1rG6XWjxMqfY=");

        //=====================================================================
        // TINAMI
        // http://www.tinami.com/api/ から取得できます。

        /// <summary>
        /// TINAMI APIキー
        /// </summary>
        public static readonly ApiKey TINAMIApiKey = ApiKey.Create("%e%7hQ5WcIjLOahw9VNyIU4qA==%z/T24upQAfNub0WiMrxLjg==%Mh+7SC+8SR514jm8tSgrRqn70i+psuox5dyQ5cHh7zs=");

        //=====================================================================
        // Microsoft Translator API (Cognitive Service)
        // https://www.microsoft.com/ja-jp/translator/getstarted.aspx から取得できます。

        /// <summary>
        /// Translator Text API Subscription Key
        /// </summary>
        public static readonly ApiKey TranslatorSubscriptionKey = ApiKey.Create("%e%Ga0nDykCVBaQqG3i4Up2lg==%4YOYlRN+SOB7exH/9NFwyMMJzuiRgPWIk5tNNXOmq1+OVcjUpz9McAlzs6zx8/7D%iX+okw1Wq5um+U3ioZacoTbYVsB9tWGmqu7r2vDtmxY=");

        //=====================================================================
        // Imgur
        // https://api.imgur.com/oauth2/addclient から取得できます

        /// <summary>
        /// Imgur Client ID
        /// </summary>
        public static readonly ApiKey ImgurClientId = ApiKey.Create("%e%f7UHH6fRBgLN6pxmjiiEWg==%vO2rqQQv9tQDeaTJX9FSyg==%98vuz4Pd1lN/HvbtzPG4x8yZcB+aKZjvRSKjvbe+kuA=");

        /// <summary>
        /// Imgur Client Secret
        /// </summary>
        public static readonly ApiKey ImgurClientSecret = ApiKey.Create("%e%mc+eebhAF+zJXPN8Pb6pgQ==%zNuPAx+sT32l1Aank7nZk5YlXLWsLC0tCg/ac09dHe+nO0pBoVQtQ0z6C825olJE%NIaCwpyNvwmj1fsYtgp4i7xfMBYhzyf4mo6wTcTKssQ=");

        //=====================================================================
        // Mobypicture
        // http://www.mobypicture.com/apps/my から取得できます

        /// <summary>
        /// Mobypicture Developer Key
        /// </summary>
        public static readonly ApiKey MobypictureKey = ApiKey.Create("%e%gQlFICkgKMP5bZ15DfOFzQ==%n+3Dfxy4IMtFNljJgdDLuoh1MdzLAk6/ZsKdGugy3BA=%yuM7lNI8qFWYj6ppDdM2bC9EtHO+ms1G6vl9xTjijek=");

        //=====================================================================
        // Tumblr
        // https://www.tumblr.com/oauth/apps から取得できます

        /// <summary>
        /// Tumblr OAuth Consumer Key
        /// </summary>
        public static readonly ApiKey TumblrConsumerKey = ApiKey.Create("%e%WJZTGDwe4njFpgKGQi5jRg==%gY2QO1so6QOfWitwxYP8XbIp98AtN3tDtoK4Q6bUOePkXVlhr4uE/VxhMg56Nblxt34Zy/8hpQSi0hlhuS2drQ==%hGKO3RVp0WaT3coVbo3H65r2pmKB8TTuiIbBzAzaV1A=");
    }
}
