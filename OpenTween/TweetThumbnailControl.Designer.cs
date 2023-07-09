namespace OpenTween
{
    partial class TweetThumbnailControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TweetThumbnailControl));
            this.scrollBar = new System.Windows.Forms.VScrollBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox = new OpenTween.OTPictureBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToClipboardMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyUrlMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.searchImageGoogleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchImageSauceNaoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // scrollBar
            // 
            resources.ApplyResources(this.scrollBar, "scrollBar");
            this.scrollBar.LargeChange = 1;
            this.scrollBar.Maximum = 0;
            this.scrollBar.Name = "scrollBar";
            this.toolTip.SetToolTip(this.scrollBar, resources.GetString("scrollBar.ToolTip"));
            this.scrollBar.ValueChanged += new System.EventHandler(this.ScrollBar_ValueChanged);
            // 
            // pictureBox
            // 
            resources.ApplyResources(this.pictureBox, "pictureBox");
            this.pictureBox.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.pictureBox.ContextMenuStrip = this.contextMenuStrip;
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.TabStop = false;
            this.toolTip.SetToolTip(this.pictureBox, resources.GetString("pictureBox.ToolTip"));
            this.pictureBox.DoubleClick += new System.EventHandler(this.PictureBox_DoubleClick);
            this.pictureBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseWheel);
            // 
            // contextMenuStrip
            // 
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMenuItem,
            this.copyToClipboardMenuItem,
            this.copyUrlMenuItem,
            this.toolStripSeparator1,
            this.searchImageGoogleMenuItem,
            this.searchImageSauceNaoMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.toolTip.SetToolTip(this.contextMenuStrip, resources.GetString("contextMenuStrip.ToolTip"));
            // 
            // openMenuItem
            // 
            resources.ApplyResources(this.openMenuItem, "openMenuItem");
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // copyToClipboardMenuItem
            // 
            resources.ApplyResources(this.copyToClipboardMenuItem, "copyToClipboardMenuItem");
            this.copyToClipboardMenuItem.Name = "copyToClipboardMenuItem";
            this.copyToClipboardMenuItem.Click += new System.EventHandler(this.CopyToClipboardMenuItem_Click);
            // 
            // copyUrlMenuItem
            // 
            resources.ApplyResources(this.copyUrlMenuItem, "copyUrlMenuItem");
            this.copyUrlMenuItem.Name = "copyUrlMenuItem";
            this.copyUrlMenuItem.Click += new System.EventHandler(this.CopyUrlMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // searchImageGoogleMenuItem
            // 
            resources.ApplyResources(this.searchImageGoogleMenuItem, "searchImageGoogleMenuItem");
            this.searchImageGoogleMenuItem.Name = "searchImageGoogleMenuItem";
            this.searchImageGoogleMenuItem.Click += new System.EventHandler(this.SearchSimilarImageMenuItem_Click);
            // 
            // searchImageSauceNaoMenuItem
            // 
            resources.ApplyResources(this.searchImageSauceNaoMenuItem, "searchImageSauceNaoMenuItem");
            this.searchImageSauceNaoMenuItem.Name = "searchImageSauceNaoMenuItem";
            this.searchImageSauceNaoMenuItem.Click += new System.EventHandler(this.SearchImageSauceNaoMenuItem_Click);
            // 
            // TweetThumbnailControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.scrollBar);
            this.Name = "TweetThumbnailControl";
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected internal System.Windows.Forms.VScrollBar scrollBar;
        protected internal System.Windows.Forms.ToolTip toolTip;
        private OTPictureBox pictureBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem searchImageGoogleMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchImageSauceNaoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyUrlMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
