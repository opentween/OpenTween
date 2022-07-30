namespace OpenTween.Setting.Panel
{
    partial class ActionPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionPanel));
            this.TabMouseLockCheck = new System.Windows.Forms.CheckBox();
            this.Label38 = new System.Windows.Forms.Label();
            this.ListDoubleClickActionComboBox = new System.Windows.Forms.ComboBox();
            this.CheckOpenUserTimeline = new System.Windows.Forms.CheckBox();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.HotkeyCheck = new System.Windows.Forms.CheckBox();
            this.HotkeyCode = new System.Windows.Forms.Label();
            this.HotkeyText = new System.Windows.Forms.TextBox();
            this.HotkeyWin = new System.Windows.Forms.CheckBox();
            this.HotkeyAlt = new System.Windows.Forms.CheckBox();
            this.HotkeyShift = new System.Windows.Forms.CheckBox();
            this.HotkeyCtrl = new System.Windows.Forms.CheckBox();
            this.Label57 = new System.Windows.Forms.Label();
            this.CheckFavRestrict = new System.Windows.Forms.CheckBox();
            this.Button3 = new System.Windows.Forms.Button();
            this.chkReadOwnPost = new System.Windows.Forms.CheckBox();
            this.BrowserPathText = new System.Windows.Forms.TextBox();
            this.UReadMng = new System.Windows.Forms.CheckBox();
            this.Label44 = new System.Windows.Forms.Label();
            this.CheckCloseToExit = new System.Windows.Forms.CheckBox();
            this.CheckMinimizeToTray = new System.Windows.Forms.CheckBox();
            this.CheckReadOldPosts = new System.Windows.Forms.CheckBox();
            this.CheckEnableTwitterV2Api = new System.Windows.Forms.CheckBox();
            this.GroupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabMouseLockCheck
            // 
            resources.ApplyResources(this.TabMouseLockCheck, "TabMouseLockCheck");
            this.TabMouseLockCheck.Name = "TabMouseLockCheck";
            this.TabMouseLockCheck.UseVisualStyleBackColor = true;
            // 
            // Label38
            // 
            resources.ApplyResources(this.Label38, "Label38");
            this.Label38.Name = "Label38";
            // 
            // ListDoubleClickActionComboBox
            // 
            this.ListDoubleClickActionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListDoubleClickActionComboBox.FormattingEnabled = true;
            this.ListDoubleClickActionComboBox.Items.AddRange(new object[] {
            resources.GetString("ListDoubleClickActionComboBox.Items"),
            resources.GetString("ListDoubleClickActionComboBox.Items1"),
            resources.GetString("ListDoubleClickActionComboBox.Items2"),
            resources.GetString("ListDoubleClickActionComboBox.Items3"),
            resources.GetString("ListDoubleClickActionComboBox.Items4"),
            resources.GetString("ListDoubleClickActionComboBox.Items5"),
            resources.GetString("ListDoubleClickActionComboBox.Items6"),
            resources.GetString("ListDoubleClickActionComboBox.Items7"),
            resources.GetString("ListDoubleClickActionComboBox.Items8")});
            resources.ApplyResources(this.ListDoubleClickActionComboBox, "ListDoubleClickActionComboBox");
            this.ListDoubleClickActionComboBox.Name = "ListDoubleClickActionComboBox";
            // 
            // CheckOpenUserTimeline
            // 
            resources.ApplyResources(this.CheckOpenUserTimeline, "CheckOpenUserTimeline");
            this.CheckOpenUserTimeline.Name = "CheckOpenUserTimeline";
            this.CheckOpenUserTimeline.UseVisualStyleBackColor = true;
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.HotkeyCheck);
            this.GroupBox3.Controls.Add(this.HotkeyCode);
            this.GroupBox3.Controls.Add(this.HotkeyText);
            this.GroupBox3.Controls.Add(this.HotkeyWin);
            this.GroupBox3.Controls.Add(this.HotkeyAlt);
            this.GroupBox3.Controls.Add(this.HotkeyShift);
            this.GroupBox3.Controls.Add(this.HotkeyCtrl);
            resources.ApplyResources(this.GroupBox3, "GroupBox3");
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.TabStop = false;
            // 
            // HotkeyCheck
            // 
            resources.ApplyResources(this.HotkeyCheck, "HotkeyCheck");
            this.HotkeyCheck.Name = "HotkeyCheck";
            this.HotkeyCheck.UseVisualStyleBackColor = true;
            this.HotkeyCheck.CheckedChanged += new System.EventHandler(this.HotkeyCheck_CheckedChanged);
            // 
            // HotkeyCode
            // 
            resources.ApplyResources(this.HotkeyCode, "HotkeyCode");
            this.HotkeyCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.HotkeyCode.Name = "HotkeyCode";
            // 
            // HotkeyText
            // 
            resources.ApplyResources(this.HotkeyText, "HotkeyText");
            this.HotkeyText.Name = "HotkeyText";
            this.HotkeyText.ReadOnly = true;
            this.HotkeyText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyText_KeyDown);
            // 
            // HotkeyWin
            // 
            resources.ApplyResources(this.HotkeyWin, "HotkeyWin");
            this.HotkeyWin.Name = "HotkeyWin";
            this.HotkeyWin.UseVisualStyleBackColor = true;
            // 
            // HotkeyAlt
            // 
            resources.ApplyResources(this.HotkeyAlt, "HotkeyAlt");
            this.HotkeyAlt.Name = "HotkeyAlt";
            this.HotkeyAlt.UseVisualStyleBackColor = true;
            // 
            // HotkeyShift
            // 
            resources.ApplyResources(this.HotkeyShift, "HotkeyShift");
            this.HotkeyShift.Name = "HotkeyShift";
            this.HotkeyShift.UseVisualStyleBackColor = true;
            // 
            // HotkeyCtrl
            // 
            resources.ApplyResources(this.HotkeyCtrl, "HotkeyCtrl");
            this.HotkeyCtrl.Name = "HotkeyCtrl";
            this.HotkeyCtrl.UseVisualStyleBackColor = true;
            // 
            // Label57
            // 
            resources.ApplyResources(this.Label57, "Label57");
            this.Label57.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Label57.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Label57.Name = "Label57";
            // 
            // CheckFavRestrict
            // 
            resources.ApplyResources(this.CheckFavRestrict, "CheckFavRestrict");
            this.CheckFavRestrict.Name = "CheckFavRestrict";
            this.CheckFavRestrict.UseVisualStyleBackColor = true;
            // 
            // Button3
            // 
            resources.ApplyResources(this.Button3, "Button3");
            this.Button3.Name = "Button3";
            this.Button3.UseVisualStyleBackColor = true;
            this.Button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // chkReadOwnPost
            // 
            resources.ApplyResources(this.chkReadOwnPost, "chkReadOwnPost");
            this.chkReadOwnPost.Name = "chkReadOwnPost";
            this.chkReadOwnPost.UseVisualStyleBackColor = true;
            // 
            // BrowserPathText
            // 
            resources.ApplyResources(this.BrowserPathText, "BrowserPathText");
            this.BrowserPathText.Name = "BrowserPathText";
            // 
            // UReadMng
            // 
            resources.ApplyResources(this.UReadMng, "UReadMng");
            this.UReadMng.Name = "UReadMng";
            this.UReadMng.UseVisualStyleBackColor = true;
            // 
            // Label44
            // 
            resources.ApplyResources(this.Label44, "Label44");
            this.Label44.Name = "Label44";
            // 
            // CheckCloseToExit
            // 
            resources.ApplyResources(this.CheckCloseToExit, "CheckCloseToExit");
            this.CheckCloseToExit.Name = "CheckCloseToExit";
            this.CheckCloseToExit.UseVisualStyleBackColor = true;
            // 
            // CheckMinimizeToTray
            // 
            resources.ApplyResources(this.CheckMinimizeToTray, "CheckMinimizeToTray");
            this.CheckMinimizeToTray.Name = "CheckMinimizeToTray";
            this.CheckMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // CheckReadOldPosts
            // 
            resources.ApplyResources(this.CheckReadOldPosts, "CheckReadOldPosts");
            this.CheckReadOldPosts.Name = "CheckReadOldPosts";
            this.CheckReadOldPosts.UseVisualStyleBackColor = true;
            // 
            // CheckEnableTwitterV2Api
            // 
            resources.ApplyResources(this.CheckEnableTwitterV2Api, "CheckEnableTwitterV2Api");
            this.CheckEnableTwitterV2Api.Name = "CheckEnableTwitterV2Api";
            this.CheckEnableTwitterV2Api.UseVisualStyleBackColor = true;
            // 
            // ActionPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.TabMouseLockCheck);
            this.Controls.Add(this.Label38);
            this.Controls.Add(this.ListDoubleClickActionComboBox);
            this.Controls.Add(this.CheckOpenUserTimeline);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.Label57);
            this.Controls.Add(this.CheckFavRestrict);
            this.Controls.Add(this.Button3);
            this.Controls.Add(this.chkReadOwnPost);
            this.Controls.Add(this.BrowserPathText);
            this.Controls.Add(this.UReadMng);
            this.Controls.Add(this.Label44);
            this.Controls.Add(this.CheckCloseToExit);
            this.Controls.Add(this.CheckMinimizeToTray);
            this.Controls.Add(this.CheckReadOldPosts);
            this.Controls.Add(this.CheckEnableTwitterV2Api);
            this.Name = "ActionPanel";
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox TabMouseLockCheck;
        internal System.Windows.Forms.Label Label38;
        internal System.Windows.Forms.ComboBox ListDoubleClickActionComboBox;
        internal System.Windows.Forms.CheckBox CheckOpenUserTimeline;
        internal System.Windows.Forms.GroupBox GroupBox3;
        internal System.Windows.Forms.CheckBox HotkeyCheck;
        internal System.Windows.Forms.Label HotkeyCode;
        internal System.Windows.Forms.TextBox HotkeyText;
        internal System.Windows.Forms.CheckBox HotkeyWin;
        internal System.Windows.Forms.CheckBox HotkeyAlt;
        internal System.Windows.Forms.CheckBox HotkeyShift;
        internal System.Windows.Forms.CheckBox HotkeyCtrl;
        internal System.Windows.Forms.Label Label57;
        internal System.Windows.Forms.CheckBox CheckFavRestrict;
        internal System.Windows.Forms.Button Button3;
        internal System.Windows.Forms.CheckBox chkReadOwnPost;
        internal System.Windows.Forms.TextBox BrowserPathText;
        internal System.Windows.Forms.CheckBox UReadMng;
        internal System.Windows.Forms.Label Label44;
        internal System.Windows.Forms.CheckBox CheckCloseToExit;
        internal System.Windows.Forms.CheckBox CheckMinimizeToTray;
        internal System.Windows.Forms.CheckBox CheckReadOldPosts;
        private System.Windows.Forms.CheckBox CheckEnableTwitterV2Api;
    }
}
