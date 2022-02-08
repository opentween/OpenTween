namespace OpenTween.Setting.Panel
{
    partial class PreviewPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewPanel));
            this.ReplyIconStateCombo = new System.Windows.Forms.ComboBox();
            this.Label72 = new System.Windows.Forms.Label();
            this.chkTabIconDisp = new System.Windows.Forms.CheckBox();
            this.CheckPreviewEnable = new System.Windows.Forms.CheckBox();
            this.CheckPreviewWindowEnable = new System.Windows.Forms.CheckBox();
            this.Label81 = new System.Windows.Forms.Label();
            this.LanguageCombo = new System.Windows.Forms.ComboBox();
            this.Label13 = new System.Windows.Forms.Label();
            this.CheckAlwaysTop = new System.Windows.Forms.CheckBox();
            this.CheckMonospace = new System.Windows.Forms.CheckBox();
            this.CheckBalloonLimit = new System.Windows.Forms.CheckBox();
            this.ComboDispTitle = new System.Windows.Forms.ComboBox();
            this.Label45 = new System.Windows.Forms.Label();
            this.CheckDispUsername = new System.Windows.Forms.CheckBox();
            this.CheckStatusAreaAtBottom = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ReplyIconStateCombo
            // 
            this.ReplyIconStateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ReplyIconStateCombo.FormattingEnabled = true;
            this.ReplyIconStateCombo.Items.AddRange(new object[] {
            resources.GetString("ReplyIconStateCombo.Items"),
            resources.GetString("ReplyIconStateCombo.Items1"),
            resources.GetString("ReplyIconStateCombo.Items2")});
            resources.ApplyResources(this.ReplyIconStateCombo, "ReplyIconStateCombo");
            this.ReplyIconStateCombo.Name = "ReplyIconStateCombo";
            // 
            // Label72
            // 
            resources.ApplyResources(this.Label72, "Label72");
            this.Label72.Name = "Label72";
            // 
            // chkTabIconDisp
            // 
            resources.ApplyResources(this.chkTabIconDisp, "chkTabIconDisp");
            this.chkTabIconDisp.Name = "chkTabIconDisp";
            this.chkTabIconDisp.UseVisualStyleBackColor = true;
            // 
            // CheckPreviewEnable
            // 
            resources.ApplyResources(this.CheckPreviewEnable, "CheckPreviewEnable");
            this.CheckPreviewEnable.Name = "CheckPreviewEnable";
            this.CheckPreviewEnable.UseVisualStyleBackColor = true;
            this.CheckPreviewEnable.CheckedChanged += new System.EventHandler(this.CheckPreviewEnable_CheckedChanged);
            // 
            // CheckPreviewWindowEnable
            // 
            resources.ApplyResources(this.CheckPreviewWindowEnable, "CheckPreviewWindowEnable");
            this.CheckPreviewWindowEnable.Name = "CheckPreviewWindowEnable";
            this.CheckPreviewWindowEnable.UseVisualStyleBackColor = true;
            // 
            // Label81
            // 
            resources.ApplyResources(this.Label81, "Label81");
            this.Label81.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Label81.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Label81.Name = "Label81";
            // 
            // LanguageCombo
            // 
            this.LanguageCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LanguageCombo.FormattingEnabled = true;
            this.LanguageCombo.Items.AddRange(new object[] {
            resources.GetString("LanguageCombo.Items"),
            resources.GetString("LanguageCombo.Items1"),
            resources.GetString("LanguageCombo.Items2")});
            resources.ApplyResources(this.LanguageCombo, "LanguageCombo");
            this.LanguageCombo.Name = "LanguageCombo";
            // 
            // Label13
            // 
            resources.ApplyResources(this.Label13, "Label13");
            this.Label13.Name = "Label13";
            // 
            // CheckAlwaysTop
            // 
            resources.ApplyResources(this.CheckAlwaysTop, "CheckAlwaysTop");
            this.CheckAlwaysTop.Name = "CheckAlwaysTop";
            this.CheckAlwaysTop.UseVisualStyleBackColor = true;
            // 
            // CheckMonospace
            // 
            resources.ApplyResources(this.CheckMonospace, "CheckMonospace");
            this.CheckMonospace.Name = "CheckMonospace";
            this.CheckMonospace.UseVisualStyleBackColor = true;
            // 
            // CheckBalloonLimit
            // 
            resources.ApplyResources(this.CheckBalloonLimit, "CheckBalloonLimit");
            this.CheckBalloonLimit.Name = "CheckBalloonLimit";
            this.CheckBalloonLimit.UseVisualStyleBackColor = true;
            // 
            // ComboDispTitle
            // 
            this.ComboDispTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboDispTitle.FormattingEnabled = true;
            this.ComboDispTitle.Items.AddRange(new object[] {
            resources.GetString("ComboDispTitle.Items"),
            resources.GetString("ComboDispTitle.Items1"),
            resources.GetString("ComboDispTitle.Items2"),
            resources.GetString("ComboDispTitle.Items3"),
            resources.GetString("ComboDispTitle.Items4"),
            resources.GetString("ComboDispTitle.Items5"),
            resources.GetString("ComboDispTitle.Items6"),
            resources.GetString("ComboDispTitle.Items7")});
            resources.ApplyResources(this.ComboDispTitle, "ComboDispTitle");
            this.ComboDispTitle.Name = "ComboDispTitle";
            // 
            // Label45
            // 
            resources.ApplyResources(this.Label45, "Label45");
            this.Label45.Name = "Label45";
            // 
            // CheckDispUsername
            // 
            resources.ApplyResources(this.CheckDispUsername, "CheckDispUsername");
            this.CheckDispUsername.Name = "CheckDispUsername";
            this.CheckDispUsername.UseVisualStyleBackColor = true;
            // 
            // CheckStatusAreaAtBottom
            // 
            resources.ApplyResources(this.CheckStatusAreaAtBottom, "CheckStatusAreaAtBottom");
            this.CheckStatusAreaAtBottom.Name = "CheckStatusAreaAtBottom";
            this.CheckStatusAreaAtBottom.UseVisualStyleBackColor = true;
            // 
            // PreviewPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.ReplyIconStateCombo);
            this.Controls.Add(this.Label72);
            this.Controls.Add(this.chkTabIconDisp);
            this.Controls.Add(this.CheckPreviewEnable);
            this.Controls.Add(this.CheckPreviewWindowEnable);
            this.Controls.Add(this.Label81);
            this.Controls.Add(this.LanguageCombo);
            this.Controls.Add(this.Label13);
            this.Controls.Add(this.CheckAlwaysTop);
            this.Controls.Add(this.CheckMonospace);
            this.Controls.Add(this.CheckBalloonLimit);
            this.Controls.Add(this.ComboDispTitle);
            this.Controls.Add(this.Label45);
            this.Controls.Add(this.CheckDispUsername);
            this.Controls.Add(this.CheckStatusAreaAtBottom);
            this.Name = "PreviewPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.ComboBox ReplyIconStateCombo;
        internal System.Windows.Forms.Label Label72;
        internal System.Windows.Forms.CheckBox chkTabIconDisp;
        internal System.Windows.Forms.CheckBox CheckPreviewEnable;
        internal System.Windows.Forms.CheckBox CheckPreviewWindowEnable;
        internal System.Windows.Forms.Label Label81;
        internal System.Windows.Forms.ComboBox LanguageCombo;
        internal System.Windows.Forms.Label Label13;
        internal System.Windows.Forms.CheckBox CheckAlwaysTop;
        internal System.Windows.Forms.CheckBox CheckMonospace;
        internal System.Windows.Forms.CheckBox CheckBalloonLimit;
        internal System.Windows.Forms.ComboBox ComboDispTitle;
        internal System.Windows.Forms.Label Label45;
        internal System.Windows.Forms.CheckBox CheckDispUsername;
        internal System.Windows.Forms.CheckBox CheckStatusAreaAtBottom;
    }
}
