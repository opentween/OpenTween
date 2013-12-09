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
    public partial class FontPanel2 : SettingPanelBase
    {
        public FontPanel2()
        {
            InitializeComponent();
        }

        private void ButtonBackToDefaultFontColor2_Click(object sender, EventArgs e)
        {
            lblInputFont.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
            lblInputFont.Font = System.Drawing.SystemFonts.DefaultFont;

            lblSelf.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.AliceBlue);

            lblAtSelf.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.AntiqueWhite);

            lblTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon);

            lblAtTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LavenderBlush);

            lblAtFromTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Honeydew);

            lblInputBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon);

            lblAtTo.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Pink);

            lblListBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);
        }
    }
}
