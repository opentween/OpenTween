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
using System.Threading.Tasks;
using System.Windows.Forms;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class OTSplitContainerTest
    {
        [Fact]
        public void IsPanelInvertedGetter_Test()
        {
            using var splitContainer = new OTSplitContainer();
            Assert.False(splitContainer.IsPanelInverted); // デフォルト値

            splitContainer.IsPanelInverted = true;
            Assert.True(splitContainer.IsPanelInverted);

            splitContainer.IsPanelInverted = false;
            Assert.False(splitContainer.IsPanelInverted);
        }

        [Fact]
        public void IsPanelInvertedSetter_InnerControlsTest()
        {
            using var splitContainer = new OTSplitContainer();
            using var buttonA = new Button();
            using var buttonB = new Button();
            using var buttonC = new Button();
            using var buttonD = new Button();

            splitContainer.Panel1.Controls.AddRange(new[] { buttonA, buttonB });
            splitContainer.Panel2.Controls.AddRange(new[] { buttonC, buttonD });

            var baseSplitContainer = (SplitContainer)splitContainer;

            // 反転前の状態 (通常の SplitContainer と同じ挙動)
            Assert.Equal(new[] { buttonA, buttonB }, baseSplitContainer.Panel1.Controls.Cast<Control>());
            Assert.Equal(new[] { buttonC, buttonD }, baseSplitContainer.Panel2.Controls.Cast<Control>());

            // 上下パネルを反転する
            splitContainer.IsPanelInverted = true;

            // Panel1, Panel2 内のコントロールが入れ替わる
            Assert.Equal(new[] { buttonC, buttonD }, baseSplitContainer.Panel1.Controls.Cast<Control>());
            Assert.Equal(new[] { buttonA, buttonB }, baseSplitContainer.Panel2.Controls.Cast<Control>());

            // 元に戻す
            splitContainer.IsPanelInverted = false;

            // Panel1, Panel2 内のコントロールも元に戻る
            Assert.Equal(new[] { buttonA, buttonB }, baseSplitContainer.Panel1.Controls.Cast<Control>());
            Assert.Equal(new[] { buttonC, buttonD }, baseSplitContainer.Panel2.Controls.Cast<Control>());
        }

        [Fact]
        public void IsPanelInvertedSetter_Panel1FixedTest()
        {
            using var splitContainer = new OTSplitContainer();
            splitContainer.FixedPanel = FixedPanel.Panel1;

            var baseSplitContainer = (SplitContainer)splitContainer;

            // 反転前の状態 (通常の SplitContainer と同じ挙動)
            Assert.Equal(FixedPanel.Panel1, baseSplitContainer.FixedPanel);

            // 上下パネルを反転する
            splitContainer.IsPanelInverted = true;
            Assert.Equal(FixedPanel.Panel2, baseSplitContainer.FixedPanel);

            // 元に戻す
            splitContainer.IsPanelInverted = false;
            Assert.Equal(FixedPanel.Panel1, baseSplitContainer.FixedPanel);
        }

        [Fact]
        public void IsPanelInvertedSetter_Panel2FixedTest()
        {
            using var splitContainer = new OTSplitContainer();
            splitContainer.FixedPanel = FixedPanel.Panel2;

            var baseSplitContainer = (SplitContainer)splitContainer;

            // 反転前の状態 (通常の SplitContainer と同じ挙動)
            Assert.Equal(FixedPanel.Panel2, baseSplitContainer.FixedPanel);

            // 上下パネルを反転する
            splitContainer.IsPanelInverted = true;
            Assert.Equal(FixedPanel.Panel1, baseSplitContainer.FixedPanel);

            // 元に戻す
            splitContainer.IsPanelInverted = false;
            Assert.Equal(FixedPanel.Panel2, baseSplitContainer.FixedPanel);
        }

        [Fact]
        public void IsPanelInvertedSetter_NoneFixedTest()
        {
            using var splitContainer = new OTSplitContainer();
            splitContainer.FixedPanel = FixedPanel.None;

            var baseSplitContainer = (SplitContainer)splitContainer;

            // 反転前の状態 (通常の SplitContainer と同じ挙動)
            Assert.Equal(FixedPanel.None, baseSplitContainer.FixedPanel);

            // 上下パネルを反転する
            splitContainer.IsPanelInverted = true;
            Assert.Equal(FixedPanel.None, baseSplitContainer.FixedPanel);

            // 元に戻す
            splitContainer.IsPanelInverted = false;
            Assert.Equal(FixedPanel.None, baseSplitContainer.FixedPanel);
        }

        [Fact]
        public void IsPanelInvertedSetter_PanelMinSizeTest()
        {
            using var splitContainer = new OTSplitContainer { Width = 800, Height = 600 };
            splitContainer.Panel1MinSize = 200;
            splitContainer.Panel2MinSize = 300;

            var baseSplitContainer = (SplitContainer)splitContainer;

            // 反転前の状態 (通常の SplitContainer と同じ挙動)
            Assert.Equal(200, baseSplitContainer.Panel1MinSize);
            Assert.Equal(300, baseSplitContainer.Panel2MinSize);

            // 上下パネルを反転する
            splitContainer.IsPanelInverted = true;
            Assert.Equal(300, baseSplitContainer.Panel1MinSize);
            Assert.Equal(200, baseSplitContainer.Panel2MinSize);

            // 元に戻す
            splitContainer.IsPanelInverted = false;
            Assert.Equal(200, baseSplitContainer.Panel1MinSize);
            Assert.Equal(300, baseSplitContainer.Panel2MinSize);
        }

        [Fact]
        public void IsPanelInvertedSetter_Panel1CollapsedTest()
        {
            using var splitContainer = new OTSplitContainer();
            splitContainer.Panel1Collapsed = true;

            var baseSplitContainer = (SplitContainer)splitContainer;

            // 反転前の状態 (通常の SplitContainer と同じ挙動)
            Assert.True(baseSplitContainer.Panel1Collapsed);
            Assert.False(baseSplitContainer.Panel2Collapsed);

            // 上下パネルを反転する
            splitContainer.IsPanelInverted = true;
            Assert.False(baseSplitContainer.Panel1Collapsed);
            Assert.True(baseSplitContainer.Panel2Collapsed);

            // 元に戻す
            splitContainer.IsPanelInverted = false;
            Assert.True(baseSplitContainer.Panel1Collapsed);
            Assert.False(baseSplitContainer.Panel2Collapsed);
        }

        [Fact]
        public void IsPanelInvertedSetter_Panel2CollapsedTest()
        {
            using var splitContainer = new OTSplitContainer();
            splitContainer.Panel2Collapsed = true;

            var baseSplitContainer = (SplitContainer)splitContainer;

            // 反転前の状態 (通常の SplitContainer と同じ挙動)
            Assert.False(baseSplitContainer.Panel1Collapsed);
            Assert.True(baseSplitContainer.Panel2Collapsed);

            // 上下パネルを反転する
            splitContainer.IsPanelInverted = true;
            Assert.True(baseSplitContainer.Panel1Collapsed);
            Assert.False(baseSplitContainer.Panel2Collapsed);

            // 元に戻す
            splitContainer.IsPanelInverted = false;
            Assert.False(baseSplitContainer.Panel1Collapsed);
            Assert.True(baseSplitContainer.Panel2Collapsed);
        }

        [Fact]
        public void IsPanelInvertedSetter_SplitterDistanceHorizontalTest()
        {
            using var splitContainer = new OTSplitContainer { Width = 800, Height = 600 };
            splitContainer.Orientation = Orientation.Horizontal; // 上下に分割された状態
            splitContainer.SplitterWidth = 5; // 分割線の幅は 5px
            splitContainer.SplitterDistance = 500; // 上から 500px で分割 (下から 300px - 5px)

            var baseSplitContainer = (SplitContainer)splitContainer;

            // 反転前の状態 (通常の SplitContainer と同じ挙動)
            Assert.Equal(500, baseSplitContainer.SplitterDistance);

            // 上下パネルを反転する
            splitContainer.IsPanelInverted = true;
            Assert.Equal(95, baseSplitContainer.SplitterDistance); // 上から 100px - 5px (下から 500px)

            // 元に戻す
            splitContainer.IsPanelInverted = false;
            Assert.Equal(500, baseSplitContainer.SplitterDistance);
        }

        [Fact]
        public void IsPanelInvertedSetter_SplitterDistanceVerticalTest()
        {
            using var splitContainer = new OTSplitContainer { Width = 800, Height = 600 };
            splitContainer.Orientation = Orientation.Vertical; // 左右に分割された状態
            splitContainer.SplitterWidth = 5; // 分割線の幅は 5px
            splitContainer.SplitterDistance = 500; // 左から 500px で分割 (右から 300px - 5px)

            var baseSplitContainer = (SplitContainer)splitContainer;

            // 反転前の状態 (通常の SplitContainer と同じ挙動)
            Assert.Equal(500, baseSplitContainer.SplitterDistance);

            // 左右パネルを反転する
            splitContainer.IsPanelInverted = true;
            Assert.Equal(295, baseSplitContainer.SplitterDistance); // 左から 300px - 5px (右から 500px)

            // 元に戻す
            splitContainer.IsPanelInverted = false;
            Assert.Equal(500, baseSplitContainer.SplitterDistance);
        }

        [Fact]
        public void PanelGetter_InvertedTest()
        {
            using var splitContainer = new OTSplitContainer();
            var panel1 = splitContainer.Panel1;
            var panel2 = splitContainer.Panel2;

            splitContainer.IsPanelInverted = true;

            Assert.Same(panel2, splitContainer.Panel1);
            Assert.Same(panel1, splitContainer.Panel2);
        }

        [Theory]
        [InlineData(FixedPanel.None)]
        [InlineData(FixedPanel.Panel1)]
        [InlineData(FixedPanel.Panel2)]
        public void FixedPanelGetter_InvertedTest(FixedPanel fixedPanel)
        {
            using var splitContainer = new OTSplitContainer();
            splitContainer.FixedPanel = fixedPanel;

            Assert.Equal(fixedPanel, splitContainer.FixedPanel);

            // 反転した状態でも OTSplitterContainer.FixedPanel の値は外見上変化しない
            splitContainer.IsPanelInverted = true;
            Assert.Equal(fixedPanel, splitContainer.FixedPanel);
        }

        [Theory]
        [InlineData(FixedPanel.None, FixedPanel.None)]
        [InlineData(FixedPanel.Panel1, FixedPanel.Panel2)]
        [InlineData(FixedPanel.Panel2, FixedPanel.Panel1)]
        public void FixedPanelSetter_InvertedTest(FixedPanel inputValue, FixedPanel internalValue)
        {
            using var splitContainer = new OTSplitContainer();
            splitContainer.IsPanelInverted = true;

            // 反転中に FixedPanel を変更する
            splitContainer.FixedPanel = inputValue;
            Assert.Equal(internalValue, ((SplitContainer)splitContainer).FixedPanel);
        }

        [Fact]
        public void SplitterDistanceGetter_InvertedVerticalTest()
        {
            using var splitContainer = new OTSplitContainer { Width = 800, Height = 600 };
            splitContainer.Orientation = Orientation.Vertical;
            splitContainer.SplitterWidth = 5;
            splitContainer.SplitterDistance = 500;

            // setter で代入した長さと一致しているか、SplitterDistance と Panel1.Width が一致しているかをテスト
            Assert.Equal(500, splitContainer.SplitterDistance);
            Assert.Equal(splitContainer.Panel1.Width, splitContainer.SplitterDistance);

            // 反転した状態でも OTSplitterContainer.SplitterDistance の値は外見上変化しない
            splitContainer.IsPanelInverted = true;
            Assert.Equal(500, splitContainer.SplitterDistance);
            Assert.Equal(splitContainer.Panel1.Width, splitContainer.SplitterDistance);
        }

        [Fact]
        public void SplitterDistanceGetter_InvertedHorizontalTest()
        {
            using var splitContainer = new OTSplitContainer { Width = 800, Height = 600 };
            splitContainer.Orientation = Orientation.Horizontal;
            splitContainer.SplitterWidth = 5;
            splitContainer.SplitterDistance = 500;

            // setter で代入した長さと一致しているか、SplitterDistance と Panel1.Height が一致しているかをテスト
            Assert.Equal(500, splitContainer.SplitterDistance);
            Assert.Equal(splitContainer.Panel1.Height, splitContainer.SplitterDistance);

            // 反転した状態でも OTSplitterContainer.SplitterDistance の値は外見上変化しない
            splitContainer.IsPanelInverted = true;
            Assert.Equal(500, splitContainer.SplitterDistance);
            Assert.Equal(splitContainer.Panel1.Height, splitContainer.SplitterDistance);
        }

        [Fact]
        public void SplitterDistanceSetter_InvertedVerticalTest()
        {
            using var splitContainer = new OTSplitContainer { Width = 800, Height = 600 };
            splitContainer.Orientation = Orientation.Vertical;
            splitContainer.SplitterWidth = 5;

            splitContainer.IsPanelInverted = true;

            // 反転中に SplitterDistance を変更する
            splitContainer.SplitterDistance = 500;
            Assert.Equal(295, ((SplitContainer)splitContainer).SplitterDistance);
            Assert.Equal(500, ((SplitContainer)splitContainer).Panel2.Width);
        }

        [Fact]
        public void SplitterDistanceSetter_InvertedHorizontalTest()
        {
            using var splitContainer = new OTSplitContainer { Width = 800, Height = 600 };
            splitContainer.Orientation = Orientation.Horizontal;
            splitContainer.SplitterWidth = 5;

            splitContainer.IsPanelInverted = true;

            // 反転中に SplitterDistance を変更する
            splitContainer.SplitterDistance = 500;
            Assert.Equal(95, ((SplitContainer)splitContainer).SplitterDistance);
            Assert.Equal(500, ((SplitContainer)splitContainer).Panel2.Height);
        }

        [Fact]
        public void PanelMinSizeGetter_InvertedTest()
        {
            using var splitContainer = new OTSplitContainer { Width = 800, Height = 600 };
            splitContainer.Panel1MinSize = 200;
            splitContainer.Panel2MinSize = 300;

            Assert.Equal(200, splitContainer.Panel1MinSize);
            Assert.Equal(300, splitContainer.Panel2MinSize);

            // 反転した状態でも OTSplitterContainer.Panel1MinSize, Panel2MinSize の値は外見上変化しない
            splitContainer.IsPanelInverted = true;
            Assert.Equal(200, splitContainer.Panel1MinSize);
            Assert.Equal(300, splitContainer.Panel2MinSize);
        }

        [Fact]
        public void PanelMinSizeSetter_InvertedTest()
        {
            using var splitContainer = new OTSplitContainer { Width = 800, Height = 600 };
            splitContainer.IsPanelInverted = true;

            // 反転中に Panel1MinSize, Panel2MinSize を変更する
            splitContainer.Panel1MinSize = 200;
            splitContainer.Panel2MinSize = 300;
            Assert.Equal(300, ((SplitContainer)splitContainer).Panel1MinSize);
            Assert.Equal(200, ((SplitContainer)splitContainer).Panel2MinSize);
        }

        [Fact]
        public void Panel1CollapsedGetter_InvertedTest()
        {
            using var splitContainer = new OTSplitContainer();
            splitContainer.Panel1Collapsed = true;

            Assert.True(splitContainer.Panel1Collapsed);

            // 反転した状態でも OTSplitterContainer.Panel1Collapsed の値は外見上変化しない
            splitContainer.IsPanelInverted = true;
            Assert.True(splitContainer.Panel1Collapsed);
        }

        [Fact]
        public void Panel1CollapsedSetter_InvertedTest()
        {
            using var splitContainer = new OTSplitContainer();
            splitContainer.IsPanelInverted = true;

            // 反転中に Panel1Collapsed を変更する
            splitContainer.Panel1Collapsed = true;
            Assert.False(((SplitContainer)splitContainer).Panel1Collapsed);
            Assert.True(((SplitContainer)splitContainer).Panel2Collapsed);
        }

        [Fact]
        public void Panel2CollapsedGetter_InvertedTest()
        {
            using var splitContainer = new OTSplitContainer();
            splitContainer.Panel2Collapsed = true;

            Assert.True(splitContainer.Panel2Collapsed);

            // 反転した状態でも OTSplitterContainer.Panel2Collapsed の値は外見上変化しない
            splitContainer.IsPanelInverted = true;
            Assert.True(splitContainer.Panel2Collapsed);
        }

        [Fact]
        public void Panel2CollapsedSetter_InvertedTest()
        {
            using var splitContainer = new OTSplitContainer();
            splitContainer.IsPanelInverted = true;

            // 反転中に Panel2Collapsed を変更する
            splitContainer.Panel2Collapsed = true;
            Assert.True(((SplitContainer)splitContainer).Panel1Collapsed);
            Assert.False(((SplitContainer)splitContainer).Panel2Collapsed);
        }
    }
}
