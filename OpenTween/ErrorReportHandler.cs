// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Connection;

namespace OpenTween
{
    public sealed class ErrorReportHandler : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        public ErrorReportHandler()
            => this.RegisterHandlers();

        private void RegisterHandlers()
        {
            TaskScheduler.UnobservedTaskException += this.TaskScheduler_UnobservedTaskException;
            AsyncTimer.UnhandledException += this.AsyncTimer_UnhandledException;
            Application.ThreadException += this.Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += this.AppDomain_UnhandledException;
        }

        private void UnregisterHandlers()
        {
            TaskScheduler.UnobservedTaskException -= this.TaskScheduler_UnobservedTaskException;
            AsyncTimer.UnhandledException -= this.AsyncTimer_UnhandledException;
            Application.ThreadException -= this.Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException -= this.AppDomain_UnhandledException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            this.OnUnhandledException(e.Exception.Flatten());
        }

        private void AsyncTimer_UnhandledException(object sender, ThreadExceptionEventArgs e)
            => this.OnUnhandledException(e.Exception);

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
            => this.OnUnhandledException(e.Exception);

        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
            => this.OnUnhandledException((Exception)e.ExceptionObject);

        private void OnUnhandledException(Exception ex)
        {
#if !DEBUG
            if (ErrorReportHandler.IsExceptionIgnorable(ex))
                return;
#endif

            if (MyCommon.ExceptionOut(ex))
                Application.Exit();
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.IsDisposed = true;
            this.UnregisterHandlers();
        }

        /// <summary>
        /// 無視しても問題のない既知の例外であれば true を返す
        /// </summary>
        public static bool IsExceptionIgnorable(Exception ex)
        {
            if (ex is AggregateException aggregated)
                return aggregated.InnerExceptions.All(x => IsExceptionIgnorable(x));

            if (ex is WebException webEx)
            {
                // SSL/TLS のネゴシエーションに失敗した場合に発生する。なぜかキャッチできない例外
                // https://osdn.net/ticket/browse.php?group_id=6526&tid=37432
                if (webEx.Status == WebExceptionStatus.SecureChannelFailure)
                    return true;
            }

            if (ex is TaskCanceledException cancelEx)
            {
                // ton.twitter.com の画像でタイムアウトした場合、try-catch で例外がキャッチできない
                // https://osdn.net/ticket/browse.php?group_id=6526&tid=37433
                var stackTrace = new StackTrace(cancelEx);
                var lastFrameMethod = stackTrace.GetFrame(stackTrace.FrameCount - 1).GetMethod();
                var matchClass = lastFrameMethod.ReflectedType == typeof(TwitterApiConnection);
                var matchMethod = lastFrameMethod.Name == nameof(TwitterApiConnection.GetStreamAsync);
                if (matchClass && matchMethod)
                    return true;
            }

            return false;
        }
    }
}
