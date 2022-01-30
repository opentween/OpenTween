namespace OpenTween.Setting.Panel
{
    partial class NotifyPanel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotifyPanel));
            this.CheckBoxNotificationPopup = new System.Windows.Forms.CheckBox();
            this.CheckBoxEnableNotificationSound = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ComboBoxNameInPopup = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CheckBoxUseGrowlForNotification = new System.Windows.Forms.CheckBox();
            this.CheckBoxEnableBlinkOnReply = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CheckBoxNotificationPopup
            // 
            resources.ApplyResources(this.CheckBoxNotificationPopup, "CheckBoxNotificationPopup");
            this.CheckBoxNotificationPopup.Name = "CheckBoxNotificationPopup";
            this.CheckBoxNotificationPopup.UseVisualStyleBackColor = true;
            // 
            // CheckBoxEnableNotificationSound
            // 
            resources.ApplyResources(this.CheckBoxEnableNotificationSound, "CheckBoxEnableNotificationSound");
            this.CheckBoxEnableNotificationSound.Name = "CheckBoxEnableNotificationSound";
            this.CheckBoxEnableNotificationSound.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ComboBoxNameInPopup
            // 
            resources.ApplyResources(this.ComboBoxNameInPopup, "ComboBoxNameInPopup");
            this.ComboBoxNameInPopup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxNameInPopup.FormattingEnabled = true;
            this.ComboBoxNameInPopup.Items.AddRange(new object[] {
            resources.GetString("ComboBoxNameInPopup.Items"),
            resources.GetString("ComboBoxNameInPopup.Items1"),
            resources.GetString("ComboBoxNameInPopup.Items2")});
            this.ComboBoxNameInPopup.Name = "ComboBoxNameInPopup";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // CheckBoxUseGrowlForNotification
            // 
            resources.ApplyResources(this.CheckBoxUseGrowlForNotification, "CheckBoxUseGrowlForNotification");
            this.CheckBoxUseGrowlForNotification.Name = "CheckBoxUseGrowlForNotification";
            this.CheckBoxUseGrowlForNotification.UseVisualStyleBackColor = true;
            // 
            // CheckBoxEnableBlinkOnReply
            // 
            resources.ApplyResources(this.CheckBoxEnableBlinkOnReply, "CheckBoxEnableBlinkOnReply");
            this.CheckBoxEnableBlinkOnReply.Name = "CheckBoxEnableBlinkOnReply";
            this.CheckBoxEnableBlinkOnReply.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CheckBoxNotificationPopup);
            this.panel1.Controls.Add(this.CheckBoxEnableBlinkOnReply);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.ComboBoxNameInPopup);
            this.panel1.Controls.Add(this.CheckBoxEnableNotificationSound);
            this.panel1.Controls.Add(this.CheckBoxUseGrowlForNotification);
            this.panel1.Controls.Add(this.label2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // NotifyPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel1);
            this.Name = "NotifyPanel";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox CheckBoxNotificationPopup;
        private System.Windows.Forms.CheckBox CheckBoxEnableNotificationSound;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ComboBoxNameInPopup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox CheckBoxUseGrowlForNotification;
        private System.Windows.Forms.CheckBox CheckBoxEnableBlinkOnReply;
        private System.Windows.Forms.Panel panel1;
    }
}
