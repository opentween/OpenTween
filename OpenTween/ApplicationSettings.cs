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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public const string WebsiteUrl = "http://sourceforge.jp/projects/opentween/wiki/FrontPage";

        /// <summary>
        /// 「ヘルプ」メニューの「ショートカットキー一覧」クリック時に外部ブラウザで表示する URL
        /// </summary>
        /// <remarks>
        /// Tween の Wiki ページのコンテンツはプロプライエタリなため転載不可
        /// </remarks>
        public const string ShortcutKeyUrl = "http://sourceforge.jp/projects/tween/wiki/%E3%82%B7%E3%83%A7%E3%83%BC%E3%83%88%E3%82%AB%E3%83%83%E3%83%88%E3%82%AD%E3%83%BC";

        //=====================================================================
        // アップデートチェック関連

        /// <summary>
        /// 最新バージョンの情報を取得するためのURL
        /// </summary>
        /// <remarks>
        /// version.txt のフォーマットについては http://sourceforge.jp/projects/opentween/wiki/VersionTxt を参照。
        /// 派生プロジェクトなどでこの機能を無効にする場合は null をセットして下さい。
        /// </remarks>
        public static readonly string VersionInfoUrl = "http://www.opentween.org/status/version.txt";

        //=====================================================================
        // Twitter
        // https://dev.twitter.com/ から取得できます。

        /// <summary>
        /// Twitter コンシューマーキー
        /// </summary>
        public const string TwitterConsumerKey = "zIoJPq3FsuViPTAs89FetDHYz";
        public const string TwitterConsumerSecret = "prTAs2fqLv12nHxlMoLQZT8AkpZt0yYb8A7ktGS2VYeRj0TddS";

        //=====================================================================
        // yfrog
        // http://stream.imageshack.us/api/ から取得できます。

        /// <summary>
        /// yfrog APIキー
        /// </summary>
        public const string YfrogApiKey = "HIDP42ZO6314ee2218e2995662bad5ae320c32f1";

        //=====================================================================
        // Foursquare
        // https://developer.foursquare.com/ から取得できます。

        /// <summary>
        /// Foursquare Client Id
        /// </summary>
        public const string FoursquareClientId = "5H3K5YQPT55DNQUFEOAJFNJA5D01ZJGO2ITEAJ3ASRIDONUB";

        /// <summary>
        /// Foursquare Client Secret
        /// </summary>
        public const string FoursquareClientSecret = "JFRHP1L451M3AEPF11UZLTIIUZCZTZRVHVOWB5TQ0AJOVPBB";

        //=====================================================================
        // bit.ly
        // https://bitly.com/a/oauth_apps から取得できます。

        /// <summary>
        /// bit.ly Client ID
        /// </summary>
        public const string BitlyClientId = "ddab8ec50f4459c315cbde9d923cf490923b6d2e";

        /// <summary>
        /// bit.ly Client Secret
        /// </summary>
        public const string BitlyClientSecret = "485c9d03dd264f8eeb4fc65d38e2762c4420cee7";

        //=====================================================================
        // TINAMI
        // http://www.tinami.com/api/ から取得できます。

        /// <summary>
        /// TINAMI APIキー
        /// </summary>
        public const string TINAMIApiKey = "4f48bb4858d36";

        //=====================================================================
        // Microsoft Translator API (Cognitive Service)
        // https://www.microsoft.com/ja-jp/translator/getstarted.aspx から取得できます。

        /// <summary>
        /// Translator Text API Subscription Key
        /// </summary>
        public readonly static string TranslatorSubscriptionKey = "6c47d2ea341148bf856bdbfafd429db7";

        //=====================================================================
        // Imgur
        // https://api.imgur.com/oauth2/addclient から取得できます

        /// <summary>
        /// Imgur Client ID
        /// </summary>
        public readonly static string ImgurClientID = "a5fff36fb83568c";

        /// <summary>
        /// Imgur Client Secret
        /// </summary>
        public readonly static string ImgurClientSecret = "af5d668a9aa83b34a8f0f735e12073edafbc9a5d";

        //=====================================================================
        // Mobypicture
        // http://www.mobypicture.com/apps/my から取得できます

        /// <summary>
        /// Mobypicture Developer Key
        /// </summary>
        public readonly static string MobypictureKey = "quPWTX0UrPHxqdH7";

        //=====================================================================
        // Tumblr
        // https://www.tumblr.com/oauth/apps から取得できます

        /// <summary>
        /// Tumblr OAuth Consumer Key
        /// </summary>
        public readonly static string TumblrConsumerKey = "Nsk62V6wMIqVNbiGyN0g3aDGBlgU7Fcb9GJ8Se0z2MUDHAY15l";
    }
}
