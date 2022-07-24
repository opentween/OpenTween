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

#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTween
{
    /// <summary>
    /// コントロールにフォーカスが当たってなくても無理矢理 MouseWheel イベントを発生させるクラス
    /// </summary>
    public class MouseWheelMessageFilter : IMessageFilter
    {
        private readonly List<Control> controls = new();

        public MouseWheelMessageFilter()
            => Application.AddMessageFilter(this);

        public void Register(Control target)
            => this.controls.Add(target);

        public void Unregister(Control target)
            => this.controls.Remove(target);

        public bool PreFilterMessage(ref Message m)
        {
            const int WM_MOUSEWHEEL = 0x020A;

            if (m.Msg == WM_MOUSEWHEEL)
            {
                foreach (var control in this.controls)
                {
                    var details = ParseMessage(m);
                    var controlRectangle = control.Parent.RectangleToScreen(control.DisplayRectangle);
                    if (controlRectangle.Contains(details.ScreenLocation))
                    {
                        var clientLocation = control.PointToClient(details.ScreenLocation);

                        var ev = new HandledMouseEventArgs(MouseButtons.None, 0, clientLocation.X, clientLocation.Y, details.WheelDelta);
                        this.RaiseMouseWheelEvent(control, ev);

                        // フォーカスが当たっているか否かに関わらず OnMouseWheel イベントを発生させているため、
                        // 二重にイベントが発生することを防ぐために標準のメッセージ処理は抑制する
                        return true;
                    }
                }
            }

            return false;
        }

        internal readonly record struct MouseEvent(
            Point ScreenLocation,
            int WheelDelta
        );

        internal static MouseEvent ParseMessage(Message m)
        {
            var screenLocation = new Point((int)(m.LParam.ToInt64() & 0xffffffff));
            var wheelDelta = (int)(m.WParam.ToInt64() & 0xffff0000) >> 16;

            return new MouseEvent(screenLocation, wheelDelta);
        }

        public void RaiseMouseWheelEvent(Control control, MouseEventArgs e)
        {
            var method = typeof(Control).GetMethod("OnMouseWheel", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(control, new[] { e });
        }
    }
}
