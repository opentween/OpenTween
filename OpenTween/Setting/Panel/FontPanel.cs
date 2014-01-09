using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenTween.Setting.Panel
{
    public partial class FontPanel : SettingPanelBase
    {
        public FontPanel()
        {
            InitializeComponent();
        }

        private void ButtonBackToDefaultFontColor_Click(object sender, EventArgs e)
        {
            lblUnread.ForeColor = SystemColors.ControlText;
            lblUnread.Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold | FontStyle.Underline);

            lblListFont.ForeColor = System.Drawing.SystemColors.ControlText;
            lblListFont.Font = System.Drawing.SystemFonts.DefaultFont;

            lblDetail.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
            lblDetail.Font = System.Drawing.SystemFonts.DefaultFont;

            lblFav.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Red);

            lblOWL.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Blue);

            lblDetailBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);

            lblDetailLink.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Blue);

            lblRetweet.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Green);
        }
    }
}
