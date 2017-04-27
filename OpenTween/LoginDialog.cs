using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTween
{
    public partial class LoginDialog : OTBaseForm
    {
        public string LoginName => this.textboxLoginName.Text;
        public string Password => this.textboxPassword.Text;

        public Func<Task<bool>> LoginCallback { get; set; } = null;
        public bool LoginSuccessed { get; set; } = false;

        public LoginDialog()
            => this.InitializeComponent();

        private async void buttonLogin_Click(object sender, EventArgs e)
        {
            if (this.LoginCallback == null)
                return;

            try
            {
                using (ControlTransaction.Disabled(this))
                {
                    // AcceptButton によって自動でフォームが閉じられるのを抑制する
                    this.AcceptButton = null;

                    this.LoginSuccessed = await this.LoginCallback();
                    if (this.LoginSuccessed)
                        this.DialogResult = DialogResult.OK;
                }
            }
            finally
            {
                this.AcceptButton = this.buttonLogin;
            }
        }
    }
}
