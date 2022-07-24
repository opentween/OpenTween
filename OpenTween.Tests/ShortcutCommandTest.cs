// OpenTween - Client of Twitter
// Copyright (c) 2015 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xunit;

namespace OpenTween
{
    public class ShortcutCommandTest
    {
        [Fact]
        public void Keys_Test()
        {
            var shortcut = ShortcutCommand.Create(Keys.Control | Keys.S)
                .Do(() => { });

            Assert.Equal(new[] { Keys.Control | Keys.S }, shortcut.Shortcuts);

            Assert.False(shortcut.IsMatch(Keys.S, FocusedControl.ListTab));
            Assert.False(shortcut.IsMatch(Keys.Control | Keys.F, FocusedControl.ListTab));
            Assert.True(shortcut.IsMatch(Keys.Control | Keys.S, FocusedControl.ListTab));
        }

        [Fact]
        public void Keys_MultipleTest()
        {
            var shortcut = ShortcutCommand.Create(Keys.Control | Keys.F, Keys.F3)
                .Do(() => { });

            Assert.Equal(new[] { Keys.Control | Keys.F, Keys.F3 }, shortcut.Shortcuts);

            // Ctrl+F, F3 のどちらの入力も受け付ける
            Assert.True(shortcut.IsMatch(Keys.Control | Keys.F, FocusedControl.ListTab));
            Assert.True(shortcut.IsMatch(Keys.F3, FocusedControl.ListTab));
        }

        [Fact]
        public void FocusedOn_Test()
        {
            var shortcut = ShortcutCommand.Create(Keys.F5)
                .FocusedOn(FocusedControl.PostBrowser)
                .Do(() => { });

            Assert.Equal(FocusedControl.PostBrowser, shortcut.FocusedOn);

            // FocusedOn が指定された場合は、そのコントロールにフォーカスがある場合のみ true を返す
            Assert.False(shortcut.IsMatch(Keys.F5, FocusedControl.ListTab));
            Assert.True(shortcut.IsMatch(Keys.F5, FocusedControl.PostBrowser));
        }

        [Fact]
        public void FocusedOn_NoneTest()
        {
            var shortcut = ShortcutCommand.Create(Keys.F5)
                .FocusedOn(FocusedControl.None)
                .Do(() => { });

            Assert.Equal(FocusedControl.None, shortcut.FocusedOn);

            // FocusedControl.None がセットされた場合はフォーカス状態に関係なく true を返す
            Assert.True(shortcut.IsMatch(Keys.F5, FocusedControl.ListTab));
            Assert.True(shortcut.IsMatch(Keys.F5, FocusedControl.PostBrowser));
        }

        [Fact]
        public void NotFocusedOn_Test()
        {
            var shortcut = ShortcutCommand.Create(Keys.F5)
                .NotFocusedOn(FocusedControl.StatusText)
                .Do(() => { });

            Assert.Equal(FocusedControl.StatusText, shortcut.NotFocusedOn);

            // NotFocusedOn が指定された場合は、そのコントロールにフォーカスがある場合以外は true を返す
            Assert.False(shortcut.IsMatch(Keys.F5, FocusedControl.StatusText));
            Assert.True(shortcut.IsMatch(Keys.F5, FocusedControl.ListTab));
        }

        [Fact]
        public void NotFocusedOn_NoneTest()
        {
            var shortcut = ShortcutCommand.Create(Keys.F5)
                .NotFocusedOn(FocusedControl.None)
                .Do(() => { });

            Assert.Equal(FocusedControl.None, shortcut.NotFocusedOn);

            // FocusedControl.None がセットされた場合はフォーカス状態に関係なく true を返す
            Assert.True(shortcut.IsMatch(Keys.F5, FocusedControl.StatusText));
            Assert.True(shortcut.IsMatch(Keys.F5, FocusedControl.ListTab));
        }

        [Fact]
        public void OnlyWhen_Test()
        {
            var hoge = false;

            var shortcut = ShortcutCommand.Create(Keys.F5)
                .OnlyWhen(() => hoge)
                .Do(() => { });

            // OnlyWhen で指定した条件が true の場合のみ true を返す
            Assert.False(shortcut.IsMatch(Keys.F5, FocusedControl.ListTab));

            hoge = true;
            Assert.True(shortcut.IsMatch(Keys.F5, FocusedControl.ListTab));
        }

        [Fact]
        public async Task RunCommand_Test()
        {
            var invoked = false;

            var shortcut = ShortcutCommand.Create(Keys.F5)
                .Do(() => invoked = true);

            Assert.False(invoked);

            await shortcut.RunCommand().ConfigureAwait(false);

            Assert.True(invoked);
        }

        [Fact]
        public async Task RunCommand_AsyncTest()
        {
            var invoked = false;

            var shortcut = ShortcutCommand.Create(Keys.F5)
                .Do(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    invoked = true;
                });

            Assert.False(invoked);

            await shortcut.RunCommand().ConfigureAwait(false);

            Assert.True(invoked);
        }

        [Fact]
        public void PreventDefault_Test()
        {
            var shortcut = ShortcutCommand.Create(Keys.F5)
                .Do(() => { }, preventDefault: true);

            Assert.True(shortcut.PreventDefault);

            shortcut = ShortcutCommand.Create(Keys.F5)
                .Do(() => { }, preventDefault: false);

            Assert.False(shortcut.PreventDefault);
        }
    }
}
