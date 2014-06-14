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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTween.Api;

namespace OpenTween.Connection
{
    /// <summary>
    /// Twitterでの画像の共有に使用できるサービスを表すインタフェース
    /// </summary>
    public interface IMediaUploadService
    {
        /// <summary>
        /// アップロード可能なメディアの最大枚数
        /// </summary>
        int MaxMediaCount { get; }

        /// <summary>
        /// アップロード可能なファイルの種類を表す文字列 (OpenFileDialog.Filter に使用)
        /// </summary>
        string SupportedFormatsStrForDialog { get; }

        /// <summary>
        /// ファイルの拡張子からアップロード可能なフォーマットであるかを判定します
        /// </summary>
        /// <param name="fileExtension">アップロードするファイルの拡張子 (ピリオドを含む)</param>
        bool CheckFileExtension(string fileExtension);

        /// <summary>
        /// ファイルサイズがアップロード可能な範囲内であるかを判定します
        /// </summary>
        /// <param name="fileExtension">アップロードするファイルの拡張子 (ピリオドを含む)</param>
        /// <param name="fileSize">アップロードするファイルのサイズ (バイト単位)</param>
        bool CheckFileSize(string fileExtension, long fileSize);

        /// <summary>
        /// アップロード可能なファイルサイズの上限を返します
        /// </summary>
        /// <param name="fileExtension">アップロードするファイルの拡張子 (ピリオドを含む)</param>
        /// <returns>ファイルサイズの上限 (バイト単位, nullの場合は上限なし)</returns>
        long? GetMaxFileSize(string fileExtension);

        /// <summary>
        /// メディアのアップロードとツイートの投稿を行います
        /// </summary>
        /// <exception cref="WebApiException"/>
        Task PostStatusAsync(string text, long? inReplyToStatusId, string[] filePaths);

        /// <summary>
        /// 画像URLのために確保する必要のある文字数を返します
        /// </summary>
        /// <param name="mediaCount">アップロードするメディアの個数</param>
        int GetReservedTextLength(int mediaCount);

        /// <summary>
        /// IMediaUploadService で使用する /help/configuration.json の値を更新します
        /// </summary>
        void UpdateTwitterConfiguration(TwitterConfiguration config);
    }
}
