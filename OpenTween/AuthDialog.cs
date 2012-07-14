using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenTween
{
    /// <summary>
    /// OAuth認証のPINコードの入力を求めるダイアログ
    /// </summary>
    public partial class AuthDialog : Form
    {
        public AuthDialog()
        {
            InitializeComponent();
        }

        public string AuthUrl
        {
            get { return AuthLinkLabel.Text; }
            set { AuthLinkLabel.Text = value; }
        }

        public string Pin
        {
            get { return PinTextBox.Text.Trim(); }
            set { PinTextBox.Text = value; }
        }

        private void AuthLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AuthLinkLabel.LinkVisited = true;

            try
            {
                System.Diagnostics.Process.Start(AuthUrl);
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(this, string.Format(Properties.Resources.BrowserStartFailed, ex.ErrorCode), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 指定されたURLにユーザーがアクセスするように指示してPINを入力させるだけ
        /// </summary>
        /// <param name="owner">親ウィンドウ</param>
        /// <param name="authUrl">認証URL</param>
        /// <returns>PIN文字列</returns>
        public static string DoAuth(IWin32Window owner, string authUrl)
        {
            using (var dialog = new AuthDialog())
            {
                dialog.AuthUrl = authUrl;

                dialog.ShowDialog(owner);

                if (dialog.DialogResult == DialogResult.OK)
                    return dialog.Pin;
                else
                    return null;
            }
        }
    }
}
