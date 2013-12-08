namespace OpenTween.Setting.Panel
{
    partial class ProxyPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProxyPanel));
            this.Label55 = new System.Windows.Forms.Label();
            this.TextProxyPassword = new System.Windows.Forms.TextBox();
            this.RadioProxyNone = new System.Windows.Forms.RadioButton();
            this.LabelProxyPassword = new System.Windows.Forms.Label();
            this.RadioProxyIE = new System.Windows.Forms.RadioButton();
            this.TextProxyUser = new System.Windows.Forms.TextBox();
            this.RadioProxySpecified = new System.Windows.Forms.RadioButton();
            this.LabelProxyUser = new System.Windows.Forms.Label();
            this.LabelProxyAddress = new System.Windows.Forms.Label();
            this.TextProxyPort = new System.Windows.Forms.TextBox();
            this.TextProxyAddress = new System.Windows.Forms.TextBox();
            this.LabelProxyPort = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Label55
            // 
            resources.ApplyResources(this.Label55, "Label55");
            this.Label55.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Label55.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Label55.Name = "Label55";
            // 
            // TextProxyPassword
            // 
            resources.ApplyResources(this.TextProxyPassword, "TextProxyPassword");
            this.TextProxyPassword.Name = "TextProxyPassword";
            this.TextProxyPassword.UseSystemPasswordChar = true;
            // 
            // RadioProxyNone
            // 
            resources.ApplyResources(this.RadioProxyNone, "RadioProxyNone");
            this.RadioProxyNone.Name = "RadioProxyNone";
            this.RadioProxyNone.UseVisualStyleBackColor = true;
            // 
            // LabelProxyPassword
            // 
            resources.ApplyResources(this.LabelProxyPassword, "LabelProxyPassword");
            this.LabelProxyPassword.Name = "LabelProxyPassword";
            // 
            // RadioProxyIE
            // 
            resources.ApplyResources(this.RadioProxyIE, "RadioProxyIE");
            this.RadioProxyIE.Checked = true;
            this.RadioProxyIE.Name = "RadioProxyIE";
            this.RadioProxyIE.TabStop = true;
            this.RadioProxyIE.UseVisualStyleBackColor = true;
            // 
            // TextProxyUser
            // 
            resources.ApplyResources(this.TextProxyUser, "TextProxyUser");
            this.TextProxyUser.Name = "TextProxyUser";
            // 
            // RadioProxySpecified
            // 
            resources.ApplyResources(this.RadioProxySpecified, "RadioProxySpecified");
            this.RadioProxySpecified.Name = "RadioProxySpecified";
            this.RadioProxySpecified.UseVisualStyleBackColor = true;
            this.RadioProxySpecified.CheckedChanged += new System.EventHandler(this.RadioProxySpecified_CheckedChanged);
            // 
            // LabelProxyUser
            // 
            resources.ApplyResources(this.LabelProxyUser, "LabelProxyUser");
            this.LabelProxyUser.Name = "LabelProxyUser";
            // 
            // LabelProxyAddress
            // 
            resources.ApplyResources(this.LabelProxyAddress, "LabelProxyAddress");
            this.LabelProxyAddress.Name = "LabelProxyAddress";
            // 
            // TextProxyPort
            // 
            resources.ApplyResources(this.TextProxyPort, "TextProxyPort");
            this.TextProxyPort.Name = "TextProxyPort";
            this.TextProxyPort.Validating += new System.ComponentModel.CancelEventHandler(this.TextProxyPort_Validating);
            // 
            // TextProxyAddress
            // 
            resources.ApplyResources(this.TextProxyAddress, "TextProxyAddress");
            this.TextProxyAddress.Name = "TextProxyAddress";
            // 
            // LabelProxyPort
            // 
            resources.ApplyResources(this.LabelProxyPort, "LabelProxyPort");
            this.LabelProxyPort.Name = "LabelProxyPort";
            // 
            // ProxyPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.Label55);
            this.Controls.Add(this.TextProxyPassword);
            this.Controls.Add(this.RadioProxyNone);
            this.Controls.Add(this.LabelProxyPassword);
            this.Controls.Add(this.RadioProxyIE);
            this.Controls.Add(this.TextProxyUser);
            this.Controls.Add(this.RadioProxySpecified);
            this.Controls.Add(this.LabelProxyUser);
            this.Controls.Add(this.LabelProxyAddress);
            this.Controls.Add(this.TextProxyPort);
            this.Controls.Add(this.TextProxyAddress);
            this.Controls.Add(this.LabelProxyPort);
            this.Name = "ProxyPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label Label55;
        internal System.Windows.Forms.TextBox TextProxyPassword;
        internal System.Windows.Forms.RadioButton RadioProxyNone;
        internal System.Windows.Forms.Label LabelProxyPassword;
        internal System.Windows.Forms.RadioButton RadioProxyIE;
        internal System.Windows.Forms.TextBox TextProxyUser;
        internal System.Windows.Forms.RadioButton RadioProxySpecified;
        internal System.Windows.Forms.Label LabelProxyUser;
        internal System.Windows.Forms.Label LabelProxyAddress;
        internal System.Windows.Forms.TextBox TextProxyPort;
        internal System.Windows.Forms.TextBox TextProxyAddress;
        internal System.Windows.Forms.Label LabelProxyPort;
    }
}
