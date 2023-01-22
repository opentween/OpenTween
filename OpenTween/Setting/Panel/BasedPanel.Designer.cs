namespace OpenTween.Setting.Panel
{
    partial class BasedPanel
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasedPanel));
            this.AuthUserCombo = new System.Windows.Forms.ComboBox();
            this.CreateAccountButton = new System.Windows.Forms.Button();
            this.StartAuthButton = new System.Windows.Forms.Button();
            this.AuthClearButton = new System.Windows.Forms.Button();
            this.Label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonMastodonAuth = new System.Windows.Forms.Button();
            this.labelMastodonAccount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AuthUserCombo
            // 
            this.AuthUserCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AuthUserCombo.FormattingEnabled = true;
            resources.ApplyResources(this.AuthUserCombo, "AuthUserCombo");
            this.AuthUserCombo.Name = "AuthUserCombo";
            // 
            // CreateAccountButton
            // 
            resources.ApplyResources(this.CreateAccountButton, "CreateAccountButton");
            this.CreateAccountButton.Name = "CreateAccountButton";
            this.CreateAccountButton.UseVisualStyleBackColor = true;
            // 
            // StartAuthButton
            // 
            resources.ApplyResources(this.StartAuthButton, "StartAuthButton");
            this.StartAuthButton.Name = "StartAuthButton";
            this.StartAuthButton.UseVisualStyleBackColor = true;
            // 
            // AuthClearButton
            // 
            resources.ApplyResources(this.AuthClearButton, "AuthClearButton");
            this.AuthClearButton.Name = "AuthClearButton";
            this.AuthClearButton.UseVisualStyleBackColor = true;
            this.AuthClearButton.Click += new System.EventHandler(this.AuthClearButton_Click);
            // 
            // Label4
            // 
            resources.ApplyResources(this.Label4, "Label4");
            this.Label4.Name = "Label4";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.AuthUserCombo);
            this.panel1.Controls.Add(this.CreateAccountButton);
            this.panel1.Controls.Add(this.StartAuthButton);
            this.panel1.Controls.Add(this.AuthClearButton);
            this.panel1.Controls.Add(this.Label4);
            this.panel1.Controls.Add(this.buttonMastodonAuth);
            this.panel1.Controls.Add(this.labelMastodonAccount);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // buttonMastodonAuth
            // 
            resources.ApplyResources(this.buttonMastodonAuth, "buttonMastodonAuth");
            this.buttonMastodonAuth.Name = "buttonMastodonAuth";
            this.buttonMastodonAuth.UseVisualStyleBackColor = true;
            this.buttonMastodonAuth.Click += new System.EventHandler(this.ButtonMastodonAuth_Click);
            // 
            // labelMastodonAccount
            // 
            this.labelMastodonAccount.AutoEllipsis = true;
            resources.ApplyResources(this.labelMastodonAccount, "labelMastodonAccount");
            this.labelMastodonAccount.Name = "labelMastodonAccount";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // BasedPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel1);
            this.Name = "BasedPanel";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.ComboBox AuthUserCombo;
        internal System.Windows.Forms.Button CreateAccountButton;
        internal System.Windows.Forms.Button StartAuthButton;
        internal System.Windows.Forms.Button AuthClearButton;
        internal System.Windows.Forms.Label Label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonMastodonAuth;
        private System.Windows.Forms.Label labelMastodonAccount;
        private System.Windows.Forms.Label label1;
    }
}
