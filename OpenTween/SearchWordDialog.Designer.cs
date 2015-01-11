namespace OpenTween
{
    partial class SearchWordDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchWordDialog));
            this.checkTimelineRegex = new System.Windows.Forms.CheckBox();
            this.checkTimelineCaseSensitive = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textSearchTimeline = new System.Windows.Forms.TextBox();
            this.buttonSearchTimeline = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSearchTimelineNew = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageTimeline = new System.Windows.Forms.TabPage();
            this.tabPagePublic = new System.Windows.Forms.TabPage();
            this.linkLabelSearchHelp = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSearchPublic = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textSearchPublic = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageTimeline.SuspendLayout();
            this.tabPagePublic.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkTimelineRegex
            // 
            resources.ApplyResources(this.checkTimelineRegex, "checkTimelineRegex");
            this.checkTimelineRegex.Name = "checkTimelineRegex";
            // 
            // checkTimelineCaseSensitive
            // 
            resources.ApplyResources(this.checkTimelineCaseSensitive, "checkTimelineCaseSensitive");
            this.checkTimelineCaseSensitive.Name = "checkTimelineCaseSensitive";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textSearchTimeline
            // 
            resources.ApplyResources(this.textSearchTimeline, "textSearchTimeline");
            this.textSearchTimeline.Name = "textSearchTimeline";
            // 
            // buttonSearchTimeline
            // 
            resources.ApplyResources(this.buttonSearchTimeline, "buttonSearchTimeline");
            this.buttonSearchTimeline.Name = "buttonSearchTimeline";
            this.buttonSearchTimeline.Click += new System.EventHandler(this.buttonSearchTimeline_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.buttonSearchTimeline, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonSearchTimelineNew, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // buttonSearchTimelineNew
            // 
            resources.ApplyResources(this.buttonSearchTimelineNew, "buttonSearchTimelineNew");
            this.buttonSearchTimelineNew.Name = "buttonSearchTimelineNew";
            this.buttonSearchTimelineNew.Click += new System.EventHandler(this.buttonSearchTimelineNew_Click);
            // 
            // tabControl
            // 
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Controls.Add(this.tabPageTimeline);
            this.tabControl.Controls.Add(this.tabPagePublic);
            this.tabControl.HotTrack = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPageTimeline
            // 
            this.tabPageTimeline.Controls.Add(this.tableLayoutPanel1);
            this.tabPageTimeline.Controls.Add(this.label1);
            this.tabPageTimeline.Controls.Add(this.checkTimelineRegex);
            this.tabPageTimeline.Controls.Add(this.textSearchTimeline);
            this.tabPageTimeline.Controls.Add(this.checkTimelineCaseSensitive);
            resources.ApplyResources(this.tabPageTimeline, "tabPageTimeline");
            this.tabPageTimeline.Name = "tabPageTimeline";
            this.tabPageTimeline.UseVisualStyleBackColor = true;
            // 
            // tabPagePublic
            // 
            this.tabPagePublic.Controls.Add(this.linkLabelSearchHelp);
            this.tabPagePublic.Controls.Add(this.tableLayoutPanel2);
            this.tabPagePublic.Controls.Add(this.label2);
            this.tabPagePublic.Controls.Add(this.textSearchPublic);
            resources.ApplyResources(this.tabPagePublic, "tabPagePublic");
            this.tabPagePublic.Name = "tabPagePublic";
            this.tabPagePublic.UseVisualStyleBackColor = true;
            // 
            // linkLabelSearchHelp
            // 
            resources.ApplyResources(this.linkLabelSearchHelp, "linkLabelSearchHelp");
            this.linkLabelSearchHelp.Name = "linkLabelSearchHelp";
            this.linkLabelSearchHelp.TabStop = true;
            this.linkLabelSearchHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSearchHelp_LinkClicked);
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.buttonSearchPublic, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // buttonSearchPublic
            // 
            resources.ApplyResources(this.buttonSearchPublic, "buttonSearchPublic");
            this.buttonSearchPublic.Name = "buttonSearchPublic";
            this.buttonSearchPublic.Click += new System.EventHandler(this.buttonSearchPublic_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textSearchPublic
            // 
            resources.ApplyResources(this.textSearchPublic, "textSearchPublic");
            this.textSearchPublic.Name = "textSearchPublic";
            // 
            // SearchWordDialog
            // 
            this.AcceptButton = this.buttonSearchTimeline;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchWordDialog";
            this.ShowInTaskbar = false;
            this.Shown += new System.EventHandler(this.SearchWordDialog_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchWordDialog_KeyDown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageTimeline.ResumeLayout(false);
            this.tabPageTimeline.PerformLayout();
            this.tabPagePublic.ResumeLayout(false);
            this.tabPagePublic.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSearchTimelineNew;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageTimeline;
        private System.Windows.Forms.TabPage tabPagePublic;
        private System.Windows.Forms.LinkLabel linkLabelSearchHelp;
        private System.Windows.Forms.Button buttonSearchTimeline;
        private System.Windows.Forms.CheckBox checkTimelineRegex;
        private System.Windows.Forms.CheckBox checkTimelineCaseSensitive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textSearchTimeline;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textSearchPublic;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonSearchPublic;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;


    }
}