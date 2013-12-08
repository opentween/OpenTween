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
            this.FontPanel = new System.Windows.Forms.Panel();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRetweet = new System.Windows.Forms.Button();
            this.lblRetweet = new System.Windows.Forms.Label();
            this.Label80 = new System.Windows.Forms.Label();
            this.ButtonBackToDefaultFontColor = new System.Windows.Forms.Button();
            this.btnDetailLink = new System.Windows.Forms.Button();
            this.lblDetailLink = new System.Windows.Forms.Label();
            this.Label18 = new System.Windows.Forms.Label();
            this.btnUnread = new System.Windows.Forms.Button();
            this.lblUnread = new System.Windows.Forms.Label();
            this.Label20 = new System.Windows.Forms.Label();
            this.btnDetailBack = new System.Windows.Forms.Button();
            this.lblDetailBackcolor = new System.Windows.Forms.Label();
            this.Label37 = new System.Windows.Forms.Label();
            this.btnDetail = new System.Windows.Forms.Button();
            this.lblDetail = new System.Windows.Forms.Label();
            this.Label26 = new System.Windows.Forms.Label();
            this.btnOWL = new System.Windows.Forms.Button();
            this.lblOWL = new System.Windows.Forms.Label();
            this.Label24 = new System.Windows.Forms.Label();
            this.btnFav = new System.Windows.Forms.Button();
            this.lblFav = new System.Windows.Forms.Label();
            this.Label22 = new System.Windows.Forms.Label();
            this.btnListFont = new System.Windows.Forms.Button();
            this.lblListFont = new System.Windows.Forms.Label();
            this.Label61 = new System.Windows.Forms.Label();
            this.FontPanel2 = new System.Windows.Forms.Panel();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.Label65 = new System.Windows.Forms.Label();
            this.Label52 = new System.Windows.Forms.Label();
            this.Label49 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label14 = new System.Windows.Forms.Label();
            this.Label16 = new System.Windows.Forms.Label();
            this.Label32 = new System.Windows.Forms.Label();
            this.Label34 = new System.Windows.Forms.Label();
            this.Label36 = new System.Windows.Forms.Label();
            this.btnInputFont = new System.Windows.Forms.Button();
            this.btnInputBackcolor = new System.Windows.Forms.Button();
            this.btnAtTo = new System.Windows.Forms.Button();
            this.btnListBack = new System.Windows.Forms.Button();
            this.btnAtFromTarget = new System.Windows.Forms.Button();
            this.btnAtTarget = new System.Windows.Forms.Button();
            this.btnTarget = new System.Windows.Forms.Button();
            this.btnAtSelf = new System.Windows.Forms.Button();
            this.btnSelf = new System.Windows.Forms.Button();
            this.lblInputFont = new System.Windows.Forms.Label();
            this.lblInputBackcolor = new System.Windows.Forms.Label();
            this.lblAtTo = new System.Windows.Forms.Label();
            this.lblListBackcolor = new System.Windows.Forms.Label();
            this.lblAtFromTarget = new System.Windows.Forms.Label();
            this.lblAtTarget = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.lblAtSelf = new System.Windows.Forms.Label();
            this.lblSelf = new System.Windows.Forms.Label();
            this.ButtonBackToDefaultFontColor2 = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.ColorDialog1 = new System.Windows.Forms.ColorDialog();
            this.FontDialog1 = new System.Windows.Forms.FontDialog();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.FontPanel.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.FontPanel2.SuspendLayout();
            this.GroupBox5.SuspendLayout();
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
            this.FontPanel.Controls.Add(this.GroupBox1);
            this.FontPanel.Name = "FontPanel";
            this.ToolTip1.SetToolTip(this.FontPanel, resources.GetString("FontPanel.ToolTip"));
            // 
            // GroupBox1
            // 
            resources.ApplyResources(this.GroupBox1, "GroupBox1");
            this.GroupBox1.Controls.Add(this.btnRetweet);
            this.GroupBox1.Controls.Add(this.lblRetweet);
            this.GroupBox1.Controls.Add(this.Label80);
            this.GroupBox1.Controls.Add(this.ButtonBackToDefaultFontColor);
            this.GroupBox1.Controls.Add(this.btnDetailLink);
            this.GroupBox1.Controls.Add(this.lblDetailLink);
            this.GroupBox1.Controls.Add(this.Label18);
            this.GroupBox1.Controls.Add(this.btnUnread);
            this.GroupBox1.Controls.Add(this.lblUnread);
            this.GroupBox1.Controls.Add(this.Label20);
            this.GroupBox1.Controls.Add(this.btnDetailBack);
            this.GroupBox1.Controls.Add(this.lblDetailBackcolor);
            this.GroupBox1.Controls.Add(this.Label37);
            this.GroupBox1.Controls.Add(this.btnDetail);
            this.GroupBox1.Controls.Add(this.lblDetail);
            this.GroupBox1.Controls.Add(this.Label26);
            this.GroupBox1.Controls.Add(this.btnOWL);
            this.GroupBox1.Controls.Add(this.lblOWL);
            this.GroupBox1.Controls.Add(this.Label24);
            this.GroupBox1.Controls.Add(this.btnFav);
            this.GroupBox1.Controls.Add(this.lblFav);
            this.GroupBox1.Controls.Add(this.Label22);
            this.GroupBox1.Controls.Add(this.btnListFont);
            this.GroupBox1.Controls.Add(this.lblListFont);
            this.GroupBox1.Controls.Add(this.Label61);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.TabStop = false;
            this.ToolTip1.SetToolTip(this.GroupBox1, resources.GetString("GroupBox1.ToolTip"));
            // 
            // btnRetweet
            // 
            resources.ApplyResources(this.btnRetweet, "btnRetweet");
            this.btnRetweet.Name = "btnRetweet";
            this.ToolTip1.SetToolTip(this.btnRetweet, resources.GetString("btnRetweet.ToolTip"));
            this.btnRetweet.UseVisualStyleBackColor = true;
            this.btnRetweet.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // lblRetweet
            // 
            resources.ApplyResources(this.lblRetweet, "lblRetweet");
            this.lblRetweet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblRetweet.Name = "lblRetweet";
            this.ToolTip1.SetToolTip(this.lblRetweet, resources.GetString("lblRetweet.ToolTip"));
            // 
            // Label80
            // 
            resources.ApplyResources(this.Label80, "Label80");
            this.Label80.Name = "Label80";
            this.ToolTip1.SetToolTip(this.Label80, resources.GetString("Label80.ToolTip"));
            // 
            // ButtonBackToDefaultFontColor
            // 
            resources.ApplyResources(this.ButtonBackToDefaultFontColor, "ButtonBackToDefaultFontColor");
            this.ButtonBackToDefaultFontColor.Name = "ButtonBackToDefaultFontColor";
            this.ToolTip1.SetToolTip(this.ButtonBackToDefaultFontColor, resources.GetString("ButtonBackToDefaultFontColor.ToolTip"));
            this.ButtonBackToDefaultFontColor.UseVisualStyleBackColor = true;
            this.ButtonBackToDefaultFontColor.Click += new System.EventHandler(this.ButtonBackToDefaultFontColor_Click);
            // 
            // btnDetailLink
            // 
            resources.ApplyResources(this.btnDetailLink, "btnDetailLink");
            this.btnDetailLink.Name = "btnDetailLink";
            this.ToolTip1.SetToolTip(this.btnDetailLink, resources.GetString("btnDetailLink.ToolTip"));
            this.btnDetailLink.UseVisualStyleBackColor = true;
            this.btnDetailLink.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // lblDetailLink
            // 
            resources.ApplyResources(this.lblDetailLink, "lblDetailLink");
            this.lblDetailLink.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDetailLink.Name = "lblDetailLink";
            this.ToolTip1.SetToolTip(this.lblDetailLink, resources.GetString("lblDetailLink.ToolTip"));
            // 
            // Label18
            // 
            resources.ApplyResources(this.Label18, "Label18");
            this.Label18.Name = "Label18";
            this.ToolTip1.SetToolTip(this.Label18, resources.GetString("Label18.ToolTip"));
            // 
            // btnUnread
            // 
            resources.ApplyResources(this.btnUnread, "btnUnread");
            this.btnUnread.Name = "btnUnread";
            this.ToolTip1.SetToolTip(this.btnUnread, resources.GetString("btnUnread.ToolTip"));
            this.btnUnread.UseVisualStyleBackColor = true;
            this.btnUnread.Click += new System.EventHandler(this.btnFontAndColor_Click);
            // 
            // lblUnread
            // 
            resources.ApplyResources(this.lblUnread, "lblUnread");
            this.lblUnread.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUnread.Name = "lblUnread";
            this.ToolTip1.SetToolTip(this.lblUnread, resources.GetString("lblUnread.ToolTip"));
            // 
            // Label20
            // 
            resources.ApplyResources(this.Label20, "Label20");
            this.Label20.Name = "Label20";
            this.ToolTip1.SetToolTip(this.Label20, resources.GetString("Label20.ToolTip"));
            // 
            // btnDetailBack
            // 
            resources.ApplyResources(this.btnDetailBack, "btnDetailBack");
            this.btnDetailBack.Name = "btnDetailBack";
            this.ToolTip1.SetToolTip(this.btnDetailBack, resources.GetString("btnDetailBack.ToolTip"));
            this.btnDetailBack.UseVisualStyleBackColor = true;
            this.btnDetailBack.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // lblDetailBackcolor
            // 
            resources.ApplyResources(this.lblDetailBackcolor, "lblDetailBackcolor");
            this.lblDetailBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDetailBackcolor.Name = "lblDetailBackcolor";
            this.ToolTip1.SetToolTip(this.lblDetailBackcolor, resources.GetString("lblDetailBackcolor.ToolTip"));
            // 
            // Label37
            // 
            resources.ApplyResources(this.Label37, "Label37");
            this.Label37.Name = "Label37";
            this.ToolTip1.SetToolTip(this.Label37, resources.GetString("Label37.ToolTip"));
            // 
            // btnDetail
            // 
            resources.ApplyResources(this.btnDetail, "btnDetail");
            this.btnDetail.Name = "btnDetail";
            this.ToolTip1.SetToolTip(this.btnDetail, resources.GetString("btnDetail.ToolTip"));
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnFontAndColor_Click);
            // 
            // lblDetail
            // 
            resources.ApplyResources(this.lblDetail, "lblDetail");
            this.lblDetail.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDetail.Name = "lblDetail";
            this.ToolTip1.SetToolTip(this.lblDetail, resources.GetString("lblDetail.ToolTip"));
            // 
            // Label26
            // 
            resources.ApplyResources(this.Label26, "Label26");
            this.Label26.Name = "Label26";
            this.ToolTip1.SetToolTip(this.Label26, resources.GetString("Label26.ToolTip"));
            // 
            // btnOWL
            // 
            resources.ApplyResources(this.btnOWL, "btnOWL");
            this.btnOWL.Name = "btnOWL";
            this.ToolTip1.SetToolTip(this.btnOWL, resources.GetString("btnOWL.ToolTip"));
            this.btnOWL.UseVisualStyleBackColor = true;
            this.btnOWL.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // lblOWL
            // 
            resources.ApplyResources(this.lblOWL, "lblOWL");
            this.lblOWL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblOWL.Name = "lblOWL";
            this.ToolTip1.SetToolTip(this.lblOWL, resources.GetString("lblOWL.ToolTip"));
            // 
            // Label24
            // 
            resources.ApplyResources(this.Label24, "Label24");
            this.Label24.Name = "Label24";
            this.ToolTip1.SetToolTip(this.Label24, resources.GetString("Label24.ToolTip"));
            // 
            // btnFav
            // 
            resources.ApplyResources(this.btnFav, "btnFav");
            this.btnFav.Name = "btnFav";
            this.ToolTip1.SetToolTip(this.btnFav, resources.GetString("btnFav.ToolTip"));
            this.btnFav.UseVisualStyleBackColor = true;
            this.btnFav.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // lblFav
            // 
            resources.ApplyResources(this.lblFav, "lblFav");
            this.lblFav.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblFav.Name = "lblFav";
            this.ToolTip1.SetToolTip(this.lblFav, resources.GetString("lblFav.ToolTip"));
            // 
            // Label22
            // 
            resources.ApplyResources(this.Label22, "Label22");
            this.Label22.Name = "Label22";
            this.ToolTip1.SetToolTip(this.Label22, resources.GetString("Label22.ToolTip"));
            // 
            // btnListFont
            // 
            resources.ApplyResources(this.btnListFont, "btnListFont");
            this.btnListFont.Name = "btnListFont";
            this.ToolTip1.SetToolTip(this.btnListFont, resources.GetString("btnListFont.ToolTip"));
            this.btnListFont.UseVisualStyleBackColor = true;
            this.btnListFont.Click += new System.EventHandler(this.btnFontAndColor_Click);
            // 
            // lblListFont
            // 
            resources.ApplyResources(this.lblListFont, "lblListFont");
            this.lblListFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblListFont.Name = "lblListFont";
            this.ToolTip1.SetToolTip(this.lblListFont, resources.GetString("lblListFont.ToolTip"));
            // 
            // Label61
            // 
            resources.ApplyResources(this.Label61, "Label61");
            this.Label61.Name = "Label61";
            this.ToolTip1.SetToolTip(this.Label61, resources.GetString("Label61.ToolTip"));
            // 
            // FontPanel2
            // 
            resources.ApplyResources(this.FontPanel2, "FontPanel2");
            this.FontPanel2.Controls.Add(this.GroupBox5);
            this.FontPanel2.Name = "FontPanel2";
            this.ToolTip1.SetToolTip(this.FontPanel2, resources.GetString("FontPanel2.ToolTip"));
            // 
            // GroupBox5
            // 
            resources.ApplyResources(this.GroupBox5, "GroupBox5");
            this.GroupBox5.Controls.Add(this.Label65);
            this.GroupBox5.Controls.Add(this.Label52);
            this.GroupBox5.Controls.Add(this.Label49);
            this.GroupBox5.Controls.Add(this.Label9);
            this.GroupBox5.Controls.Add(this.Label14);
            this.GroupBox5.Controls.Add(this.Label16);
            this.GroupBox5.Controls.Add(this.Label32);
            this.GroupBox5.Controls.Add(this.Label34);
            this.GroupBox5.Controls.Add(this.Label36);
            this.GroupBox5.Controls.Add(this.btnInputFont);
            this.GroupBox5.Controls.Add(this.btnInputBackcolor);
            this.GroupBox5.Controls.Add(this.btnAtTo);
            this.GroupBox5.Controls.Add(this.btnListBack);
            this.GroupBox5.Controls.Add(this.btnAtFromTarget);
            this.GroupBox5.Controls.Add(this.btnAtTarget);
            this.GroupBox5.Controls.Add(this.btnTarget);
            this.GroupBox5.Controls.Add(this.btnAtSelf);
            this.GroupBox5.Controls.Add(this.btnSelf);
            this.GroupBox5.Controls.Add(this.lblInputFont);
            this.GroupBox5.Controls.Add(this.lblInputBackcolor);
            this.GroupBox5.Controls.Add(this.lblAtTo);
            this.GroupBox5.Controls.Add(this.lblListBackcolor);
            this.GroupBox5.Controls.Add(this.lblAtFromTarget);
            this.GroupBox5.Controls.Add(this.lblAtTarget);
            this.GroupBox5.Controls.Add(this.lblTarget);
            this.GroupBox5.Controls.Add(this.lblAtSelf);
            this.GroupBox5.Controls.Add(this.lblSelf);
            this.GroupBox5.Controls.Add(this.ButtonBackToDefaultFontColor2);
            this.GroupBox5.Name = "GroupBox5";
            this.GroupBox5.TabStop = false;
            this.ToolTip1.SetToolTip(this.GroupBox5, resources.GetString("GroupBox5.ToolTip"));
            // 
            // Label65
            // 
            resources.ApplyResources(this.Label65, "Label65");
            this.Label65.Name = "Label65";
            this.ToolTip1.SetToolTip(this.Label65, resources.GetString("Label65.ToolTip"));
            // 
            // Label52
            // 
            resources.ApplyResources(this.Label52, "Label52");
            this.Label52.Name = "Label52";
            this.ToolTip1.SetToolTip(this.Label52, resources.GetString("Label52.ToolTip"));
            // 
            // Label49
            // 
            resources.ApplyResources(this.Label49, "Label49");
            this.Label49.Name = "Label49";
            this.ToolTip1.SetToolTip(this.Label49, resources.GetString("Label49.ToolTip"));
            // 
            // Label9
            // 
            resources.ApplyResources(this.Label9, "Label9");
            this.Label9.Name = "Label9";
            this.ToolTip1.SetToolTip(this.Label9, resources.GetString("Label9.ToolTip"));
            // 
            // Label14
            // 
            resources.ApplyResources(this.Label14, "Label14");
            this.Label14.Name = "Label14";
            this.ToolTip1.SetToolTip(this.Label14, resources.GetString("Label14.ToolTip"));
            // 
            // Label16
            // 
            resources.ApplyResources(this.Label16, "Label16");
            this.Label16.Name = "Label16";
            this.ToolTip1.SetToolTip(this.Label16, resources.GetString("Label16.ToolTip"));
            // 
            // Label32
            // 
            resources.ApplyResources(this.Label32, "Label32");
            this.Label32.Name = "Label32";
            this.ToolTip1.SetToolTip(this.Label32, resources.GetString("Label32.ToolTip"));
            // 
            // Label34
            // 
            resources.ApplyResources(this.Label34, "Label34");
            this.Label34.Name = "Label34";
            this.ToolTip1.SetToolTip(this.Label34, resources.GetString("Label34.ToolTip"));
            // 
            // Label36
            // 
            resources.ApplyResources(this.Label36, "Label36");
            this.Label36.Name = "Label36";
            this.ToolTip1.SetToolTip(this.Label36, resources.GetString("Label36.ToolTip"));
            // 
            // btnInputFont
            // 
            resources.ApplyResources(this.btnInputFont, "btnInputFont");
            this.btnInputFont.Name = "btnInputFont";
            this.ToolTip1.SetToolTip(this.btnInputFont, resources.GetString("btnInputFont.ToolTip"));
            this.btnInputFont.UseVisualStyleBackColor = true;
            this.btnInputFont.Click += new System.EventHandler(this.btnFontAndColor_Click);
            // 
            // btnInputBackcolor
            // 
            resources.ApplyResources(this.btnInputBackcolor, "btnInputBackcolor");
            this.btnInputBackcolor.Name = "btnInputBackcolor";
            this.ToolTip1.SetToolTip(this.btnInputBackcolor, resources.GetString("btnInputBackcolor.ToolTip"));
            this.btnInputBackcolor.UseVisualStyleBackColor = true;
            this.btnInputBackcolor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnAtTo
            // 
            resources.ApplyResources(this.btnAtTo, "btnAtTo");
            this.btnAtTo.Name = "btnAtTo";
            this.ToolTip1.SetToolTip(this.btnAtTo, resources.GetString("btnAtTo.ToolTip"));
            this.btnAtTo.UseVisualStyleBackColor = true;
            this.btnAtTo.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnListBack
            // 
            resources.ApplyResources(this.btnListBack, "btnListBack");
            this.btnListBack.Name = "btnListBack";
            this.ToolTip1.SetToolTip(this.btnListBack, resources.GetString("btnListBack.ToolTip"));
            this.btnListBack.UseVisualStyleBackColor = true;
            this.btnListBack.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnAtFromTarget
            // 
            resources.ApplyResources(this.btnAtFromTarget, "btnAtFromTarget");
            this.btnAtFromTarget.Name = "btnAtFromTarget";
            this.ToolTip1.SetToolTip(this.btnAtFromTarget, resources.GetString("btnAtFromTarget.ToolTip"));
            this.btnAtFromTarget.UseVisualStyleBackColor = true;
            this.btnAtFromTarget.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnAtTarget
            // 
            resources.ApplyResources(this.btnAtTarget, "btnAtTarget");
            this.btnAtTarget.Name = "btnAtTarget";
            this.ToolTip1.SetToolTip(this.btnAtTarget, resources.GetString("btnAtTarget.ToolTip"));
            this.btnAtTarget.UseVisualStyleBackColor = true;
            this.btnAtTarget.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnTarget
            // 
            resources.ApplyResources(this.btnTarget, "btnTarget");
            this.btnTarget.Name = "btnTarget";
            this.ToolTip1.SetToolTip(this.btnTarget, resources.GetString("btnTarget.ToolTip"));
            this.btnTarget.UseVisualStyleBackColor = true;
            this.btnTarget.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnAtSelf
            // 
            resources.ApplyResources(this.btnAtSelf, "btnAtSelf");
            this.btnAtSelf.Name = "btnAtSelf";
            this.ToolTip1.SetToolTip(this.btnAtSelf, resources.GetString("btnAtSelf.ToolTip"));
            this.btnAtSelf.UseVisualStyleBackColor = true;
            this.btnAtSelf.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnSelf
            // 
            resources.ApplyResources(this.btnSelf, "btnSelf");
            this.btnSelf.Name = "btnSelf";
            this.ToolTip1.SetToolTip(this.btnSelf, resources.GetString("btnSelf.ToolTip"));
            this.btnSelf.UseVisualStyleBackColor = true;
            this.btnSelf.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // lblInputFont
            // 
            resources.ApplyResources(this.lblInputFont, "lblInputFont");
            this.lblInputFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblInputFont.Name = "lblInputFont";
            this.ToolTip1.SetToolTip(this.lblInputFont, resources.GetString("lblInputFont.ToolTip"));
            // 
            // lblInputBackcolor
            // 
            resources.ApplyResources(this.lblInputBackcolor, "lblInputBackcolor");
            this.lblInputBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblInputBackcolor.Name = "lblInputBackcolor";
            this.ToolTip1.SetToolTip(this.lblInputBackcolor, resources.GetString("lblInputBackcolor.ToolTip"));
            // 
            // lblAtTo
            // 
            resources.ApplyResources(this.lblAtTo, "lblAtTo");
            this.lblAtTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAtTo.Name = "lblAtTo";
            this.ToolTip1.SetToolTip(this.lblAtTo, resources.GetString("lblAtTo.ToolTip"));
            // 
            // lblListBackcolor
            // 
            resources.ApplyResources(this.lblListBackcolor, "lblListBackcolor");
            this.lblListBackcolor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblListBackcolor.Name = "lblListBackcolor";
            this.ToolTip1.SetToolTip(this.lblListBackcolor, resources.GetString("lblListBackcolor.ToolTip"));
            // 
            // lblAtFromTarget
            // 
            resources.ApplyResources(this.lblAtFromTarget, "lblAtFromTarget");
            this.lblAtFromTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAtFromTarget.Name = "lblAtFromTarget";
            this.ToolTip1.SetToolTip(this.lblAtFromTarget, resources.GetString("lblAtFromTarget.ToolTip"));
            // 
            // lblAtTarget
            // 
            resources.ApplyResources(this.lblAtTarget, "lblAtTarget");
            this.lblAtTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAtTarget.Name = "lblAtTarget";
            this.ToolTip1.SetToolTip(this.lblAtTarget, resources.GetString("lblAtTarget.ToolTip"));
            // 
            // lblTarget
            // 
            resources.ApplyResources(this.lblTarget, "lblTarget");
            this.lblTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTarget.Name = "lblTarget";
            this.ToolTip1.SetToolTip(this.lblTarget, resources.GetString("lblTarget.ToolTip"));
            // 
            // lblAtSelf
            // 
            resources.ApplyResources(this.lblAtSelf, "lblAtSelf");
            this.lblAtSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAtSelf.Name = "lblAtSelf";
            this.ToolTip1.SetToolTip(this.lblAtSelf, resources.GetString("lblAtSelf.ToolTip"));
            // 
            // lblSelf
            // 
            resources.ApplyResources(this.lblSelf, "lblSelf");
            this.lblSelf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSelf.Name = "lblSelf";
            this.ToolTip1.SetToolTip(this.lblSelf, resources.GetString("lblSelf.ToolTip"));
            // 
            // ButtonBackToDefaultFontColor2
            // 
            resources.ApplyResources(this.ButtonBackToDefaultFontColor2, "ButtonBackToDefaultFontColor2");
            this.ButtonBackToDefaultFontColor2.Name = "ButtonBackToDefaultFontColor2";
            this.ToolTip1.SetToolTip(this.ButtonBackToDefaultFontColor2, resources.GetString("ButtonBackToDefaultFontColor2.ToolTip"));
            this.ButtonBackToDefaultFontColor2.UseVisualStyleBackColor = true;
            this.ButtonBackToDefaultFontColor2.Click += new System.EventHandler(this.ButtonBackToDefaultFontColor_Click);
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
            // AppendSettingDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
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
            this.FontPanel.ResumeLayout(false);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.FontPanel2.ResumeLayout(false);
            this.GroupBox5.ResumeLayout(false);
            this.GroupBox5.PerformLayout();
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
        internal System.Windows.Forms.Panel FontPanel;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.Button btnRetweet;
        internal System.Windows.Forms.Label lblRetweet;
        internal System.Windows.Forms.Label Label80;
        internal System.Windows.Forms.Button ButtonBackToDefaultFontColor;
        internal System.Windows.Forms.Button btnDetailLink;
        internal System.Windows.Forms.Label lblDetailLink;
        internal System.Windows.Forms.Label Label18;
        internal System.Windows.Forms.Button btnUnread;
        internal System.Windows.Forms.Label lblUnread;
        internal System.Windows.Forms.Label Label20;
        internal System.Windows.Forms.Button btnDetailBack;
        internal System.Windows.Forms.Label lblDetailBackcolor;
        internal System.Windows.Forms.Label Label37;
        internal System.Windows.Forms.Button btnDetail;
        internal System.Windows.Forms.Label lblDetail;
        internal System.Windows.Forms.Label Label26;
        internal System.Windows.Forms.Button btnOWL;
        internal System.Windows.Forms.Label lblOWL;
        internal System.Windows.Forms.Label Label24;
        internal System.Windows.Forms.Button btnFav;
        internal System.Windows.Forms.Label lblFav;
        internal System.Windows.Forms.Label Label22;
        internal System.Windows.Forms.Button btnListFont;
        internal System.Windows.Forms.Label lblListFont;
        internal System.Windows.Forms.Label Label61;
        internal System.Windows.Forms.Panel FontPanel2;
        internal System.Windows.Forms.GroupBox GroupBox5;
        internal System.Windows.Forms.Label Label65;
        internal System.Windows.Forms.Label Label52;
        internal System.Windows.Forms.Label Label49;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.Label Label14;
        internal System.Windows.Forms.Label Label16;
        internal System.Windows.Forms.Label Label32;
        internal System.Windows.Forms.Label Label34;
        internal System.Windows.Forms.Label Label36;
        internal System.Windows.Forms.Button btnInputFont;
        internal System.Windows.Forms.Button btnInputBackcolor;
        internal System.Windows.Forms.Button btnAtTo;
        internal System.Windows.Forms.Button btnListBack;
        internal System.Windows.Forms.Button btnAtFromTarget;
        internal System.Windows.Forms.Button btnAtTarget;
        internal System.Windows.Forms.Button btnTarget;
        internal System.Windows.Forms.Button btnAtSelf;
        internal System.Windows.Forms.Button btnSelf;
        internal System.Windows.Forms.Label lblInputFont;
        internal System.Windows.Forms.Label lblInputBackcolor;
        internal System.Windows.Forms.Label lblAtTo;
        internal System.Windows.Forms.Label lblListBackcolor;
        internal System.Windows.Forms.Label lblAtFromTarget;
        internal System.Windows.Forms.Label lblAtTarget;
        internal System.Windows.Forms.Label lblTarget;
        internal System.Windows.Forms.Label lblAtSelf;
        internal System.Windows.Forms.Label lblSelf;
        internal System.Windows.Forms.Button ButtonBackToDefaultFontColor2;
        internal System.Windows.Forms.Button Save;
        internal System.Windows.Forms.Button Cancel;
        internal System.Windows.Forms.ColorDialog ColorDialog1;
        internal System.Windows.Forms.FontDialog FontDialog1;
    }
}