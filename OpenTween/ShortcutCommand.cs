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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTween
{
    public enum FocusedControl
    {
        None,
        ListTab,
        StatusText,
        PostBrowser,
    }

    /// <summary>
    /// ショートカットキーの条件と動作を定義するクラス
    /// </summary>
    public class ShortcutCommand
    {
        private Func<bool> onlyWhen;
        private Func<Task> command;

        /// <summary>
        /// ショートカットキーが動作する条件となるキー入力
        /// </summary>
        public Keys[] Shortcuts { get; private set; }

        /// <summary>
        /// ショートカットキーが動作する条件となるフォーカス状態
        /// </summary>
        public FocusedControl FocusedOn { get; private set; }

        /// <summary>
        /// ショートカットキーが動作する否定条件となるフォーカス状態
        /// </summary>
        public FocusedControl NotFocusedOn { get; private set; }

        /// <summary>
        /// コマンドを実行した後、コントロール既定の動作を無効化するか否か (デフォルトは true)
        /// </summary>
        public bool PreventDefault { get; private set; }

        private ShortcutCommand()
        {
            this.Shortcuts = Array.Empty<Keys>();
            this.command = () => Task.CompletedTask;
            this.onlyWhen = () => true;
            this.FocusedOn = FocusedControl.None;
            this.NotFocusedOn = FocusedControl.None;
            this.PreventDefault = true;
        }

        /// <summary>
        /// コマンドを実行する条件を満たしているか判定します
        /// </summary>
        public bool IsMatch(Keys pressedKey, FocusedControl focusedOn)
        {
            if (!this.Shortcuts.Contains(pressedKey))
                return false;

            if (this.FocusedOn != FocusedControl.None && this.FocusedOn != focusedOn)
                return false;

            if (this.NotFocusedOn != FocusedControl.None && this.NotFocusedOn == focusedOn)
                return false;

            if (!this.onlyWhen())
                return false;

            return true;
        }

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        public async Task RunCommand()
            => await this.command();

        /// <summary>
        /// 新規に ShortcutCommand インスタンスを作成するビルダーを返します
        /// </summary>
        public static ShortcutCommand.Builder Create(params Keys[] shortcuts)
            => new Builder().Keys(shortcuts);

        public class Builder
        {
            private readonly ShortcutCommand instance;

            internal Builder()
                => this.instance = new ShortcutCommand();

            /// <summary>
            /// 指定されたキーが入力された時にショートカットを発動します
            /// </summary>
            public Builder Keys(params Keys[] shortcuts)
            {
                this.instance.Shortcuts = shortcuts;
                return this;
            }

            /// <summary>
            /// 指定されたコントロールにフォーカスが当たっている時のみショートカットを有効にします
            /// </summary>
            public Builder FocusedOn(FocusedControl focusedOn)
            {
                this.instance.FocusedOn = focusedOn;
                return this;
            }

            /// <summary>
            /// 指定されたコントロールにフォーカスが当たっている時はショートカットを有効にしません
            /// </summary>
            public Builder NotFocusedOn(FocusedControl notFocusedOn)
            {
                this.instance.NotFocusedOn = notFocusedOn;
                return this;
            }

            /// <summary>
            /// 指定された条件が true になる間のみショートカットを有効にします
            /// </summary>
            public Builder OnlyWhen(Func<bool> condition)
            {
                this.instance.onlyWhen = condition;
                return this;
            }

            /// <summary>
            /// ショートカットが入力された時に行う動作の内容
            /// </summary>
            public ShortcutCommand Do(Action action, bool preventDefault = true)
                => this.Do(SynchronousTask(action), preventDefault);

            /// <summary>
            /// ショートカットが入力された時に行う動作の内容
            /// </summary>
            public ShortcutCommand Do(Func<Task> action, bool preventDefault = true)
            {
                this.instance.command = action;
                this.instance.PreventDefault = preventDefault;

                return this.instance;
            }

            /// <summary>
            /// Action を Func&lt;Task&gt; に変換します
            /// </summary>
            private static Func<Task> SynchronousTask(Action action)
            {
                return () =>
                {
                    action();
                    return Task.CompletedTask;
                };
            }
        }
    }
}
