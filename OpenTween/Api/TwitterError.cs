// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Api
{
    // 参照: https://dev.twitter.com/docs/error-codes-responses

    [DataContract]
    public class TwitterError
    {
        [DataMember(Name = "errors")]
        public TwitterErrorItem[] Errors { get; set; }

        /// <exception cref="SerializationException"/>
        public static TwitterError ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterError>(json);
        }
    }

    [DataContract]
    public class TwitterErrorItem
    {
        [DataMember(Name = "code")]
        public TwitterErrorCode Code { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        public override string ToString()
        {
            if (Enum.IsDefined(typeof(TwitterErrorCode), this.Code))
                return this.Code.ToString();
            else
                return this.Message;
        }
    }

    /// <summary>
    /// Twitter API から返されるエラーコード
    /// </summary>
    public enum TwitterErrorCode : int
    {
        /// <summary>
        /// 不正なリクエスト等によって認証を完了できない場合に発生する。大体クライアントのせい
        /// </summary>
        AuthError = 32,

        /// <summary>
        /// 指定されたリソースが存在しません。HTTP 404 と同等
        /// </summary>
        NotFound = 34,

        /// <summary>
        /// アカウントが凍結されています
        /// </summary>
        SuspendedAccount = 64,

        /// <summary>
        /// REST API v1 は星になりました
        /// </summary>
        APIv1Retired = 68,

        /// <summary>
        /// レートリミットに到達しました
        /// </summary>
        RateLimit = 88,

        /// <summary>
        /// アクセストークンが無効です。不正なトークンまたはユーザーによって失効されています
        /// </summary>
        InvalidToken = 89,

        /// <summary>
        /// SSLを使わずにAPIに接続することはできません
        /// </summary>
        SslIsRequired = 92,

        /// <summary>
        /// サーバーの過負荷によって一時的にアクセスできません
        /// </summary>
        OverCapacity = 130,

        /// <summary>
        /// サーバーの内部エラー
        /// </summary>
        InternalError = 131,

        /// <summary>
        /// oauth_timestamp の時刻が無効。クライアントかサーバーの時計が大幅にずれている
        /// </summary>
        TimestampOutOfRange = 135,

        /// <summary>
        /// ユーザーからブロックされている (公式ドキュメントに記述無し)
        /// </summary>
        Blocked = 136,

        /// <summary>
        /// 既にふぁぼっているツイートをふぁぼろうとした (公式ドキュメントに記述無し)
        /// </summary>
        AlreadyFavorited = 139,

        /// <summary>
        /// フォローの追加が制限されています
        /// </summary>
        FollowLimit = 161,

        /// <summary>
        /// 非公開ユーザーのため閲覧できません
        /// </summary>
        Protected = 179,

        /// <summary>
        /// 投稿されたステータスが重複しています
        /// </summary>
        DuplicateStatus = 187,

        /// <summary>
        /// 認証が必要な API で認証データが含まれていない、または認証データが不正
        /// </summary>
        AuthenticationRequired = 215,

        /// <summary>
        /// スパムの疑いのあるリクエストがブロックされました
        /// </summary>
        RequestBlocked = 226,

        /// <summary>
        /// 廃止されたエンドポイント
        /// </summary>
        RetiredEndpoint = 251,
    }
}
