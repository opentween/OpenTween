// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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
using System.Drawing;
using System.Runtime.InteropServices;

namespace OpenTween
{
    public class ShieldIcon
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHSTOCKICONINFO
        {
            public int cbSize;
            public IntPtr hIcon;
            public int iSysImageIndex;
            public int iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;
        }

        [DllImport("shell32.dll")]
        private static extern int SHGetStockIconInfo(int siid, uint uFlags, ref SHSTOCKICONINFO psii); // Windows Vista, Windows Server 2008 以前には存在しない

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        const int SIID_SHIELD = 77;
        const uint SHGFI_ICON = 0x100;
        const uint SHGFI_SMALLICON = 0x1;


        private Image icondata = null;
        private SHSTOCKICONINFO sii;


        public ShieldIcon()
        {
            //NT6 kernelかどうか検査
            if (!MyCommon.IsNT6())
            {
                icondata = null;
                return;
            }

            try
            {
                sii = new SHSTOCKICONINFO();
                sii.cbSize = Marshal.SizeOf(sii);
                sii.hIcon = IntPtr.Zero;

                int ret;
                ret = SHGetStockIconInfo(SIID_SHIELD, SHGFI_ICON | SHGFI_SMALLICON, ref sii);
                if (ret != 0)
                {
                    icondata = null;
                    return;
                }

                icondata = Bitmap.FromHicon(sii.hIcon);
            }
            catch (Exception)
            {
                icondata = null;
            }
        }

        public void Dispose()
        {
            if (icondata != null)
            {
                icondata.Dispose();
            }
        }

        public Image Icon
        {
            get
            {
                //return icondata;
                //シールドアイコンのデータを返さないように　あとでどうにかする
                return null;
            }
        }
    }
}
