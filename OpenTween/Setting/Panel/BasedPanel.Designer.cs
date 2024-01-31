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
            this.AccountListLabel = new System.Windows.Forms.Label();
            this.AccountsListBox = new System.Windows.Forms.ListBox();
            this.AddAccountButton = new System.Windows.Forms.Button();
            this.RemoveAccountButton = new System.Windows.Forms.Button();
            this.MakePrimaryButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AccountListLabel
            // 
            resources.ApplyResources(this.AccountListLabel, "AccountListLabel");
            this.AccountListLabel.Name = "AccountListLabel";
            // 
            // AccountsListBox
            // 
            resources.ApplyResources(this.AccountsListBox, "AccountsListBox");
            this.AccountsListBox.FormattingEnabled = true;
            this.AccountsListBox.Name = "AccountsListBox";
            // 
            // AddAccountButton
            // 
            resources.ApplyResources(this.AddAccountButton, "AddAccountButton");
            this.AddAccountButton.Name = "AddAccountButton";
            this.AddAccountButton.UseVisualStyleBackColor = true;
            // 
            // RemoveAccountButton
            // 
            resources.ApplyResources(this.RemoveAccountButton, "RemoveAccountButton");
            this.RemoveAccountButton.Name = "RemoveAccountButton";
            this.RemoveAccountButton.UseVisualStyleBackColor = true;
            this.RemoveAccountButton.Click += new System.EventHandler(this.RemoveAccountButton_Click);
            // 
            // MakePrimaryButton
            // 
            resources.ApplyResources(this.MakePrimaryButton, "MakePrimaryButton");
            this.MakePrimaryButton.Name = "MakePrimaryButton";
            this.MakePrimaryButton.UseVisualStyleBackColor = true;
            this.MakePrimaryButton.Click += new System.EventHandler(this.MakePrimaryButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.AccountListLabel);
            this.panel1.Controls.Add(this.AccountsListBox);
            this.panel1.Controls.Add(this.AddAccountButton);
            this.panel1.Controls.Add(this.RemoveAccountButton);
            this.panel1.Controls.Add(this.MakePrimaryButton);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
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
        internal System.Windows.Forms.Label AccountListLabel;
        internal System.Windows.Forms.ListBox AccountsListBox;
        internal System.Windows.Forms.Button AddAccountButton;
        internal System.Windows.Forms.Button RemoveAccountButton;
        internal System.Windows.Forms.Button MakePrimaryButton;
        private System.Windows.Forms.Panel panel1;
    }
}
