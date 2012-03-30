// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace OpenTween
{
    public partial class TweenAboutBox : Form
    {
        public TweenAboutBox()
        {
            InitializeComponent();
        }

        private void TweenAboutBox_Load(object sender, EventArgs e)
        {
            // フォームのタイトルを設定します。
            this.Text = MyCommon.ReplaceAppName(this.Text);

            // バージョン情報ボックスに表示されたテキストをすべて初期化します。
            // TODO: [プロジェクト] メニューの下にある [プロジェクト プロパティ] ダイアログの [アプリケーション] ペインで、アプリケーションのアセンブリ情報を 
            //    カスタマイズします。
            this.LabelProductName.Text = Application.ProductName;
            this.LabelVersion.Text = String.Format(Properties.Resources.TweenAboutBox_LoadText2, MyCommon.GetReadableVersion());
            this.LabelCopyright.Text = GetApplicationAttribute<AssemblyCopyrightAttribute>().Copyright;
            this.LabelCompanyName.Text = Application.CompanyName;
            this.TextBoxDescription.Text = GetApplicationAttribute<AssemblyDescriptionAttribute>().Description;
            this.ChangeLog.Text = Properties.Resources.ChangeLog;
            this.TextBoxDescription.Text = string.Format(Properties.Resources.Description, ApplicationSettings.FeedbackTwitterName, ApplicationSettings.FeedbackEmailAddress);
        }

        protected T GetApplicationAttribute<T>() where T : Attribute
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            return (T) Attribute.GetCustomAttribute(currentAssembly, typeof(T));
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TweenAboutBox_Shown(object sender, EventArgs e)
        {
            OKButton.Focus();
        }
    }
}
