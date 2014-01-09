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
            // BasedPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.AuthUserCombo);
            this.Controls.Add(this.CreateAccountButton);
            this.Controls.Add(this.StartAuthButton);
            this.Controls.Add(this.AuthClearButton);
            this.Controls.Add(this.Label4);
            this.Name = "BasedPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ComboBox AuthUserCombo;
        internal System.Windows.Forms.Button CreateAccountButton;
        internal System.Windows.Forms.Button StartAuthButton;
        internal System.Windows.Forms.Button AuthClearButton;
        internal System.Windows.Forms.Label Label4;
    }
}
