namespace OpenTween
{
    partial class AppendSettingDialog
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppendSettingDialog));
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TreeViewSetting = new System.Windows.Forms.TreeView();
            this.StartupPanel = new OpenTween.Setting.Panel.StartupPanel();
            this.PreviewPanel = new OpenTween.Setting.Panel.PreviewPanel();
            this.TweetActPanel = new OpenTween.Setting.Panel.TweetActPanel();
            this.GetCountPanel = new OpenTween.Setting.Panel.GetCountPanel();
            this.ShortUrlPanel = new OpenTween.Setting.Panel.ShortUrlPanel();
            this.BasedPanel = new OpenTween.Setting.Panel.BasedPanel();
            this.TweetPrvPanel = new OpenTween.Setting.Panel.TweetPrvPanel();
            this.NotifyPanel = new OpenTween.Setting.Panel.NotifyPanel();
            this.CooperatePanel = new OpenTween.Setting.Panel.CooperatePanel();
            this.ProxyPanel = new OpenTween.Setting.Panel.ProxyPanel();
            this.ConnectionPanel = new OpenTween.Setting.Panel.ConnectionPanel();
            this.GetPeriodPanel = new OpenTween.Setting.Panel.GetPeriodPanel();
            this.ActionPanel = new OpenTween.Setting.Panel.ActionPanel();
            this.FontPanel = new OpenTween.Setting.Panel.FontPanel();
            this.FontPanel2 = new OpenTween.Setting.Panel.FontPanel2();
            this.Save = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.ColorDialog1 = new System.Windows.Forms.ColorDialog();
            this.FontDialog1 = new System.Windows.Forms.FontDialog();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SplitContainer1
            // 
            resources.ApplyResources(this.SplitContainer1, "SplitContainer1");
            this.SplitContainer1.Name = "SplitContainer1";
            // 
            // SplitContainer1.Panel1
            // 
            resources.ApplyResources(this.SplitContainer1.Panel1, "SplitContainer1.Panel1");
            this.SplitContainer1.Panel1.Controls.Add(this.TreeViewSetting);
            this.ToolTip1.SetToolTip(this.SplitContainer1.Panel1, resources.GetString("SplitContainer1.Panel1.ToolTip"));
            // 
            // SplitContainer1.Panel2
            // 
            resources.ApplyResources(this.SplitContainer1.Panel2, "SplitContainer1.Panel2");
            this.SplitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.SplitContainer1.Panel2.Controls.Add(this.StartupPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.PreviewPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.TweetActPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.GetCountPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.ShortUrlPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.BasedPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.TweetPrvPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.NotifyPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.CooperatePanel);
            this.SplitContainer1.Panel2.Controls.Add(this.ProxyPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.ConnectionPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.GetPeriodPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.ActionPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.FontPanel);
            this.SplitContainer1.Panel2.Controls.Add(this.FontPanel2);
            this.ToolTip1.SetToolTip(this.SplitContainer1.Panel2, resources.GetString("SplitContainer1.Panel2.ToolTip"));
            this.SplitContainer1.TabStop = false;
            this.ToolTip1.SetToolTip(this.SplitContainer1, resources.GetString("SplitContainer1.ToolTip"));
            // 
            // TreeViewSetting
            // 
            resources.ApplyResources(this.TreeViewSetting, "TreeViewSetting");
            this.TreeViewSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TreeViewSetting.HideSelection = false;
            this.TreeViewSetting.Name = "TreeViewSetting";
            this.TreeViewSetting.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("TreeViewSetting.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("TreeViewSetting.Nodes1"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("TreeViewSetting.Nodes2"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("TreeViewSetting.Nodes3"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("TreeViewSetting.Nodes4")))});
            this.TreeViewSetting.ShowLines = false;
            this.ToolTip1.SetToolTip(this.TreeViewSetting, resources.GetString("TreeViewSetting.ToolTip"));
            this.TreeViewSetting.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeViewSetting_BeforeSelect);
            this.TreeViewSetting.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewSetting_AfterSelect);
            // 
            // StartupPanel
            // 
            resources.ApplyResources(this.StartupPanel, "StartupPanel");
            this.StartupPanel.Name = "StartupPanel";
            this.ToolTip1.SetToolTip(this.StartupPanel, resources.GetString("StartupPanel.ToolTip"));
            // 
            // PreviewPanel
            // 
            resources.ApplyResources(this.PreviewPanel, "PreviewPanel");
            this.PreviewPanel.Name = "PreviewPanel";
            this.ToolTip1.SetToolTip(this.PreviewPanel, resources.GetString("PreviewPanel.ToolTip"));
            // 
            // TweetActPanel
            // 
            resources.ApplyResources(this.TweetActPanel, "TweetActPanel");
            this.TweetActPanel.Name = "TweetActPanel";
            this.ToolTip1.SetToolTip(this.TweetActPanel, resources.GetString("TweetActPanel.ToolTip"));
            // 
            // GetCountPanel
            // 
            resources.ApplyResources(this.GetCountPanel, "GetCountPanel");
            this.GetCountPanel.Name = "GetCountPanel";
            this.ToolTip1.SetToolTip(this.GetCountPanel, resources.GetString("GetCountPanel.ToolTip"));
            // 
            // ShortUrlPanel
            // 
            resources.ApplyResources(this.ShortUrlPanel, "ShortUrlPanel");
            this.ShortUrlPanel.Name = "ShortUrlPanel";
            this.ToolTip1.SetToolTip(this.ShortUrlPanel, resources.GetString("ShortUrlPanel.ToolTip"));
            // 
            // BasedPanel
            // 
            resources.ApplyResources(this.BasedPanel, "BasedPanel");
            this.BasedPanel.Name = "BasedPanel";
            this.ToolTip1.SetToolTip(this.BasedPanel, resources.GetString("BasedPanel.ToolTip"));
            // 
            // TweetPrvPanel
            // 
            resources.ApplyResources(this.TweetPrvPanel, "TweetPrvPanel");
            this.TweetPrvPanel.Name = "TweetPrvPanel";
            this.ToolTip1.SetToolTip(this.TweetPrvPanel, resources.GetString("TweetPrvPanel.ToolTip"));
            // 
            // NotifyPanel
            // 
            resources.ApplyResources(this.NotifyPanel, "NotifyPanel");
            this.NotifyPanel.Name = "NotifyPanel";
            this.ToolTip1.SetToolTip(this.NotifyPanel, resources.GetString("NotifyPanel.ToolTip"));
            // 
            // CooperatePanel
            // 
            resources.ApplyResources(this.CooperatePanel, "CooperatePanel");
            this.CooperatePanel.Name = "CooperatePanel";
            this.ToolTip1.SetToolTip(this.CooperatePanel, resources.GetString("CooperatePanel.ToolTip"));
            // 
            // ProxyPanel
            // 
            resources.ApplyResources(this.ProxyPanel, "ProxyPanel");
            this.ProxyPanel.Name = "ProxyPanel";
            this.ToolTip1.SetToolTip(this.ProxyPanel, resources.GetString("ProxyPanel.ToolTip"));
            // 
            // ConnectionPanel
            // 
            resources.ApplyResources(this.ConnectionPanel, "ConnectionPanel");
            this.ConnectionPanel.Name = "ConnectionPanel";
            this.ToolTip1.SetToolTip(this.ConnectionPanel, resources.GetString("ConnectionPanel.ToolTip"));
            // 
            // GetPeriodPanel
            // 
            resources.ApplyResources(this.GetPeriodPanel, "GetPeriodPanel");
            this.GetPeriodPanel.Name = "GetPeriodPanel";
            this.GetPeriodPanel.IntervalChanged += new System.EventHandler<OpenTween.IntervalChangedEventArgs>(this.GetPeriodPanel_IntervalChanged);
            this.ToolTip1.SetToolTip(this.GetPeriodPanel, resources.GetString("GetPeriodPanel.ToolTip"));
            // 
            // ActionPanel
            // 
            resources.ApplyResources(this.ActionPanel, "ActionPanel");
            this.ActionPanel.Name = "ActionPanel";
            this.ToolTip1.SetToolTip(this.ActionPanel, resources.GetString("ActionPanel.ToolTip"));
            // 
            // FontPanel
            // 
            resources.ApplyResources(this.FontPanel, "FontPanel");
            this.FontPanel.Name = "FontPanel";
            this.ToolTip1.SetToolTip(this.FontPanel, resources.GetString("FontPanel.ToolTip"));
            // 
            // FontPanel2
            // 
            resources.ApplyResources(this.FontPanel2, "FontPanel2");
            this.FontPanel2.Name = "FontPanel2";
            this.ToolTip1.SetToolTip(this.FontPanel2, resources.GetString("FontPanel2.ToolTip"));
            // 
            // Save
            // 
            resources.ApplyResources(this.Save, "Save");
            this.Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Save.Name = "Save";
            this.ToolTip1.SetToolTip(this.Save, resources.GetString("Save.ToolTip"));
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Cancel
            // 
            resources.ApplyResources(this.Cancel, "Cancel");
            this.Cancel.CausesValidation = false;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Name = "Cancel";
            this.ToolTip1.SetToolTip(this.Cancel, resources.GetString("Cancel.ToolTip"));
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // ColorDialog1
            // 
            this.ColorDialog1.AnyColor = true;
            // 
            // FontDialog1
            // 
            this.FontDialog1.AllowVerticalFonts = false;
            this.FontDialog1.FontMustExist = true;
            this.FontDialog1.ShowColor = true;
            // 
            // AppendSettingDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.Cancel;
            this.Controls.Add(this.SplitContainer1);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppendSettingDialog";
            this.ShowInTaskbar = false;
            this.ToolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Setting_FormClosing);
            this.Load += new System.EventHandler(this.Setting_Load);
            this.Shown += new System.EventHandler(this.Setting_Shown);
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.SplitContainer SplitContainer1;
        private System.Windows.Forms.TreeView TreeViewSetting;
        private Setting.Panel.StartupPanel StartupPanel;
        private Setting.Panel.PreviewPanel PreviewPanel;
        internal System.Windows.Forms.ToolTip ToolTip1;
        private Setting.Panel.TweetActPanel TweetActPanel;
        private Setting.Panel.GetCountPanel GetCountPanel;
        private Setting.Panel.ShortUrlPanel ShortUrlPanel;
        private Setting.Panel.BasedPanel BasedPanel;
        private Setting.Panel.TweetPrvPanel TweetPrvPanel;
        private Setting.Panel.NotifyPanel NotifyPanel;
        private Setting.Panel.CooperatePanel CooperatePanel;
        private Setting.Panel.ProxyPanel ProxyPanel;
        private Setting.Panel.ConnectionPanel ConnectionPanel;
        private Setting.Panel.GetPeriodPanel GetPeriodPanel;
        private Setting.Panel.ActionPanel ActionPanel;
        private Setting.Panel.FontPanel FontPanel;
        internal System.Windows.Forms.Button Save;
        internal System.Windows.Forms.Button Cancel;
        internal System.Windows.Forms.ColorDialog ColorDialog1;
        internal System.Windows.Forms.FontDialog FontDialog1;
        private Setting.Panel.FontPanel2 FontPanel2;
    }
}