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
using System.Text;
using System.Windows.Forms;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class HashtagManageTest
    {
        // _isPermanent絡みの挙動が謎すぎて全然網羅できてない

        [Fact]
        public void InitHashtagHistory_Test()
        {
            var hashtags = new[] { "#foo", "#bar" };

            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, hashtags, "", false, false, false))
            {
                hashDialog.RunSilent = true;

                Assert.Equal(new[] { "#foo", "#bar" }, hashDialog.HistoryHashList.Items.Cast<string>());
                Assert.Equal(new[] { "#foo", "#bar" }, hashDialog.HashHistories);
            }
        }

        [Fact]
        public void InitHashtagHistory_EmptyTest()
        {
            var hashtags = new string[0];

            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, hashtags, "", false, false, false))
            {
                hashDialog.RunSilent = true;

                Assert.Empty(hashDialog.HistoryHashList.Items);
                Assert.Empty(hashDialog.HashHistories);
            }
        }

        [Fact]
        public void AddHashtag_Test()
        {
            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, new string[0], "", false, false, false))
            {
                hashDialog.RunSilent = true;

                TestUtils.FireEvent(hashDialog.AddButton, "Click"); // 「新規 (&N)」ボタン

                hashDialog.UseHashText.Text = "#OpenTween";

                TestUtils.FireEvent(hashDialog.PermOK_Button, "Click"); // 「詳細」グループ内の「OK」ボタン

                Assert.Equal(new[] { "#OpenTween" }, hashDialog.HistoryHashList.Items.Cast<string>());
                Assert.Equal(new[] { "#OpenTween" }, hashDialog.HashHistories);
            }
        }

        [Fact]
        public void AddHashtag_FullWidthTest()
        {
            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, new string[0], "", false, false, false))
            {
                hashDialog.RunSilent = true;

                TestUtils.FireEvent(hashDialog.AddButton, "Click"); // 「新規 (&N)」ボタン

                hashDialog.UseHashText.Text = "＃ほげほげ";

                TestUtils.FireEvent(hashDialog.PermOK_Button, "Click"); // 「詳細」グループ内の「OK」ボタン

                Assert.Equal(new[] { "#ほげほげ" }, hashDialog.HistoryHashList.Items.Cast<string>());
                Assert.Equal(new[] { "#ほげほげ" }, hashDialog.HashHistories);
            }
        }

        [Fact]
        public void AddHashtag_CombiningCharacterSequenceTest()
        {
            // ハッシュタグを表す「#」の直後に結合文字 (濁点など) が続いた場合に対するテスト

            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, new string[0], "", false, false, false))
            {
                hashDialog.RunSilent = true;

                TestUtils.FireEvent(hashDialog.AddButton, "Click"); // 「新規 (&N)」ボタン

                // どんちき└(＾ω＾ )┐♫ ┌( ＾ω＾)┘♫どんちき
                hashDialog.UseHashText.Text = "#゛t゛e゛s゛a゛b゛u゛";

                TestUtils.FireEvent(hashDialog.PermOK_Button, "Click"); // 「詳細」グループ内の「OK」ボタン

                Assert.Equal(new[] { "#゛t゛e゛s゛a゛b゛u゛" }, hashDialog.HistoryHashList.Items.Cast<string>());
                Assert.Equal(new[] { "#゛t゛e゛s゛a゛b゛u゛" }, hashDialog.HashHistories);
            }
        }

        [Fact]
        public void AddHashtag_MultipleTest()
        {
            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, new string[0], "", false, false, false))
            {
                hashDialog.RunSilent = true;

                TestUtils.FireEvent(hashDialog.AddButton, "Click"); // 「新規 (&N)」ボタン

                hashDialog.UseHashText.Text = "#foo #bar";

                TestUtils.FireEvent(hashDialog.PermOK_Button, "Click"); // 「詳細」グループ内の「OK」ボタン

                Assert.Equal(new[] { "#foo #bar" }, hashDialog.HistoryHashList.Items.Cast<string>());
                Assert.Equal(new[] { "#foo #bar" }, hashDialog.HashHistories);
            }
        }

        [Fact]
        public void AddHashtag_InvalidTest()
        {
            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, new string[0], "", false, false, false))
            {
                hashDialog.RunSilent = true;

                TestUtils.FireEvent(hashDialog.AddButton, "Click"); // 「新規 (&N)」ボタン

                hashDialog.UseHashText.Text = "hogehoge";

                TestUtils.FireEvent(hashDialog.PermOK_Button, "Click"); // 「詳細」グループ内の「OK」ボタン
                // 実際にはここでエラーメッセージが出る

                Assert.Empty(hashDialog.HistoryHashList.Items);
                Assert.Empty(hashDialog.HashHistories);
            }
        }

        [Fact]
        public void EditHashtag_Test()
        {
            var hashtags = new[] { "#foo", "#bar" };

            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, hashtags, "", false, false, false))
            {
                hashDialog.RunSilent = true;

                hashDialog.HistoryHashList.SelectedIndices.Add(0);

                TestUtils.FireEvent(hashDialog.EditButton, "Click"); // 「編集(&E)」ボタン

                hashDialog.UseHashText.Text = "#hoge";

                TestUtils.FireEvent(hashDialog.PermOK_Button, "Click"); // 「詳細」グループ内の「OK」ボタン

                Assert.Equal(new[] { "#hoge", "#bar" }, hashDialog.HistoryHashList.Items.Cast<string>());
                Assert.Equal(new[] { "#hoge", "#bar" }, hashDialog.HashHistories);
            }
        }

        [Fact]
        public void DeleteHashtag_Test()
        {
            var hashtags = new[] { "#foo", "#bar" };

            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, hashtags, "", false, false, false))
            {
                hashDialog.RunSilent = true;

                hashDialog.HistoryHashList.SelectedIndices.Add(1);

                TestUtils.FireEvent(hashDialog.DeleteButton, "Click"); // 「削除(&D)」ボタン

                Assert.Equal(new[] { "#foo" }, hashDialog.HistoryHashList.Items.Cast<string>());
                Assert.Equal(new[] { "#foo" }, hashDialog.HashHistories);
            }
        }

        [Fact]
        public void DeleteHashtag_NotSelectTest()
        {
            var hashtags = new[] { "#foo", "#bar" };

            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, hashtags, "", false, false, false))
            {
                hashDialog.RunSilent = true;

                // ハッシュタグを選択していない状態

                TestUtils.FireEvent(hashDialog.DeleteButton, "Click"); // 「削除(&D)」ボタン

                Assert.Equal(new[] { "#foo", "#bar" }, hashDialog.HistoryHashList.Items.Cast<string>());
                Assert.Equal(new[] { "#foo", "#bar" }, hashDialog.HashHistories);
            }
        }

        [Fact]
        public void UnSelectButton_Test()
        {
            var hashtags = new[] { "#foo" };

            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, hashtags, "", false, false, false))
            {
                hashDialog.RunSilent = true;

                hashDialog.HistoryHashList.SelectedIndices.Add(0);

                TestUtils.FireEvent(hashDialog.UnSelectButton, "Click"); // 「非使用(&U)」ボタン

                Assert.Empty(hashDialog.HistoryHashList.SelectedIndices);
            }
        }

        [Fact]
        public void EditModeSwitch_Test()
        {
            using (var atDialog = new AtIdSupplement())
            using (var hashDialog = new HashtagManage(atDialog, new string[0], "", false, false, false))
            {
                hashDialog.RunSilent = true;

                Assert.True(hashDialog.GroupHashtag.Enabled);
                Assert.True(hashDialog.TableLayoutButtons.Enabled);
                Assert.False(hashDialog.GroupDetail.Enabled);

                TestUtils.FireEvent(hashDialog.AddButton, "Click"); // 「新規 (&N)」ボタン

                // 編集モードに入る
                Assert.False(hashDialog.GroupHashtag.Enabled);
                Assert.False(hashDialog.TableLayoutButtons.Enabled);
                Assert.True(hashDialog.GroupDetail.Enabled);

                hashDialog.UseHashText.Text = "#hogehoge";

                TestUtils.FireEvent(hashDialog.PermOK_Button, "Click"); // 「詳細」グループ内の「OK」ボタン

                // 編集モードから抜ける
                Assert.True(hashDialog.GroupHashtag.Enabled);
                Assert.True(hashDialog.TableLayoutButtons.Enabled);
                Assert.False(hashDialog.GroupDetail.Enabled);
            }
        }
    }
}
