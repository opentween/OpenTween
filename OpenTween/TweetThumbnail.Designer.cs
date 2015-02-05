namespace OpenTween
{
    partial class TweetThumbnail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TweetThumbnail));
            this.scrollBar = new System.Windows.Forms.VScrollBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelPictureBox = new System.Windows.Forms.Panel();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.searchSimilarImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.scrollBar.ValueChanged += new System.EventHandler(this.scrollBar_ValueChanged);
            // 
            // panelPictureBox
            // 
            resources.ApplyResources(this.panelPictureBox, "panelPictureBox");
            this.panelPictureBox.Name = "panelPictureBox";
            this.toolTip.SetToolTip(this.panelPictureBox, resources.GetString("panelPictureBox.ToolTip"));
            // 
            // contextMenuStrip
            // 
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchSimilarImageMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.toolTip.SetToolTip(this.contextMenuStrip, resources.GetString("contextMenuStrip.ToolTip"));
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // searchSimilarImageMenuItem
            // 
            resources.ApplyResources(this.searchSimilarImageMenuItem, "searchSimilarImageMenuItem");
            this.searchSimilarImageMenuItem.Name = "searchSimilarImageMenuItem";
            this.searchSimilarImageMenuItem.Click += new System.EventHandler(this.searchSimilarImageMenuItem_Click);
            // 
            // TweetThumbnail
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.panelPictureBox);
            this.Controls.Add(this.scrollBar);
            this.Name = "TweetThumbnail";
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected internal System.Windows.Forms.VScrollBar scrollBar;
        protected internal System.Windows.Forms.ToolTip toolTip;
        protected internal System.Windows.Forms.Panel panelPictureBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem searchSimilarImageMenuItem;
    }
}
