// OpenTween - Client of Twitter
// Copyright (c) 2015 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTween
{
    /// <summary>
    /// タスクが待機中であることを表示するダイアログ
    /// </summary>
    /// <remarks>
    /// 一定時間 (Timeout プロパティで指定する) 以上経ってもタスクが完了しない場合にダイアログを表示します。
    /// EnableCancellation メソッドを使用することでキャンセル機能を提供することも可能です。
    /// </remarks>
    public partial class WaitingDialog : OTBaseForm
    {
        private readonly SynchronizationContext synchronizationContext;
        private readonly Lazy<CancellationTokenSource> cancellationTokenSource;

        private bool cancellationEnabled = false;

        /// <summary>
        /// ダイアログを表示せずに待機する最短の待ち時間 (デフォルト0.5秒)
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// ダイアログに表示するメッセージ
        /// </summary>
        public string Message
        {
            get { return this.labelMessage.Text; }
            set { this.labelMessage.Text = value; }
        }

        public WaitingDialog()
        {
            this.InitializeComponent();

            this.synchronizationContext = SynchronizationContext.Current;
            this.cancellationTokenSource = new Lazy<CancellationTokenSource>();

            this.Timeout = TimeSpan.FromMilliseconds(500);
        }

        public WaitingDialog(string message)
            : this()
        {
            this.Message = message;
        }

        /// <summary>
        /// キャンセル機能を有効にし、キャンセルを通知するための CancellationToken を返します
        /// </summary>
        public CancellationToken EnableCancellation()
        {
            this.cancellationEnabled = true;
            this.ControlBox = true;

            var cts = this.cancellationTokenSource.Value;
            return cts.Token;
        }

        public Task WaitForAsync(Task task)
        {
            return this.WaitForAsync(task.ContinueWith(_ => 0));
        }

        public Task WaitForAsync(IWin32Window owner, Task task)
        {
            return this.WaitForAsync(owner, task.ContinueWith(_ => 0));
        }

        public Task<T> WaitForAsync<T>(Task<T> task)
        {
            return this.WaitForAsync(null, task);
        }

        /// <summary>
        /// タスクを待機し、状況に応じて待機中ダイアログを表示します
        /// </summary>
        /// <param name="owner">ダイアログのオーナー</param>
        /// <param name="task">待機するタスク</param>
        public Task<T> WaitForAsync<T>(IWin32Window owner, Task<T> task)
        {
            return Task.Run(async () =>
            {
                // 指定された秒数以内で完了すればダイアログは表示しない
                var timeout = Task.Delay(this.Timeout);
                if (await Task.WhenAny(task, timeout) != timeout)
                    return await task;

                var dialogTask = this.InvokeAsync(() => this.ShowDialog(owner));

                // キャンセルされずにタスクが先に完了したらダイアログを閉じる
                if (await Task.WhenAny(task, dialogTask) != dialogTask)
                    await this.InvokeAsync(() => this.DialogResult = DialogResult.OK);

                return await task;
            });
        }

        /// <summary>
        /// Control.InvokeメソッドのTask版みたいなやつ
        /// </summary>
        private Task<T> InvokeAsync<T>(Func<T> x)
        {
            var tcs = new TaskCompletionSource<T>();
            this.synchronizationContext.Post(_ =>
            {
                var ret = x();
                tcs.SetResult(ret);
            }, null);

            return tcs.Task;
        }

        private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.cancellationEnabled)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    var cts = this.cancellationTokenSource.Value;
                    cts.Cancel();
                }
            }
        }

        private void ProgressDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.cancellationEnabled)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    var cts = this.cancellationTokenSource.Value;
                    cts.Cancel();

                    this.DialogResult = DialogResult.Cancel;
                }
            }
        }
    }
}
