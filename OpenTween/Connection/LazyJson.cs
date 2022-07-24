// OpenTween - Client of Twitter
// Copyright (c) 2016 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

#nullable enable annotations

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTween.Api;

namespace OpenTween.Connection
{
    /// <summary>
    /// HTTP レスポンスの受信と JSON のパース処理を延期させるクラス
    /// </summary>
    public sealed class LazyJson<T> : IDisposable
    {
        public HttpResponseMessage? Response { get; }

        private T instance;
        private bool completed = false;

        public LazyJson(HttpResponseMessage response)
            => this.Response = response;

        internal LazyJson(T instance)
        {
            this.Response = null;

            this.instance = instance;
            this.completed = true;
        }

        public async Task<T> LoadJsonAsync()
        {
            if (this.completed)
                return this.instance!;

            using var content = this.Response.Content;
            var responseText = await content.ReadAsStringAsync()
                .ConfigureAwait(false);

            try
            {
                this.instance = MyCommon.CreateDataFromJson<T>(responseText);
                this.completed = true;

                return this.instance;
            }
            catch (SerializationException ex)
            {
                throw TwitterApiException.CreateFromException(ex, responseText);
            }
        }

        public void Dispose()
            => this.Response?.Dispose();
    }

    public static class LazyJson
    {
        public static LazyJson<T> Create<T>(T instance)
            => new(instance);
    }

    public static class LazyJsonTaskExtension
    {
        public static async Task IgnoreResponse<T>(this Task<LazyJson<T>> task)
        {
            using var lazyJson = await task.ConfigureAwait(false);
            // レスポンスボディを読み込まず破棄する
        }
    }
}
