namespace OpenTween
{
    partial class MediaSelectorPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MediaSelectorPanel));
            this.Label2 = new System.Windows.Forms.Label();
            this.ImageServiceCombo = new System.Windows.Forms.ComboBox();
            this.ImageCancelButton = new System.Windows.Forms.Button();
            this.AlternativeTextPanel = new System.Windows.Forms.Panel();
            this.AlternativeTextBox = new System.Windows.Forms.TextBox();
            this.AlternativeTextLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.MediaListView = new System.Windows.Forms.ListView();
            this.MediaListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MoveToBackMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveToNextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteMediaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AddMediaButton = new System.Windows.Forms.Button();
            this.ServiceSelectPanel = new System.Windows.Forms.Panel();
            this.ImageSelectedPicture = new OpenTween.OTPictureBox();
            this.AlternativeTextPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.MediaListContextMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.ServiceSelectPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageSelectedPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // Label2
            // 
            resources.ApplyResources(this.Label2, "Label2");
            this.Label2.Name = "Label2";
            // 
            // ImageServiceCombo
            // 
            resources.ApplyResources(this.ImageServiceCombo, "ImageServiceCombo");
            this.ImageServiceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ImageServiceCombo.FormattingEnabled = true;
            this.ImageServiceCombo.Items.AddRange(new object[] {
            resources.GetString("ImageServiceCombo.Items")});
            this.ImageServiceCombo.Name = "ImageServiceCombo";
            this.ImageServiceCombo.SelectedIndexChanged += new System.EventHandler(this.ImageServiceCombo_SelectedIndexChanged);
            // 
            // ImageCancelButton
            // 
            resources.ApplyResources(this.ImageCancelButton, "ImageCancelButton");
            this.ImageCancelButton.Name = "ImageCancelButton";
            this.ImageCancelButton.UseVisualStyleBackColor = true;
            this.ImageCancelButton.Click += new System.EventHandler(this.ImageCancelButton_Click);
            // 
            // AlternativeTextPanel
            // 
            resources.ApplyResources(this.AlternativeTextPanel, "AlternativeTextPanel");
            this.AlternativeTextPanel.Controls.Add(this.AlternativeTextBox);
            this.AlternativeTextPanel.Controls.Add(this.AlternativeTextLabel);
            this.AlternativeTextPanel.Name = "AlternativeTextPanel";
            // 
            // AlternativeTextBox
            // 
            resources.ApplyResources(this.AlternativeTextBox, "AlternativeTextBox");
            this.AlternativeTextBox.Name = "AlternativeTextBox";
            this.AlternativeTextBox.Validated += new System.EventHandler(this.AlternativeTextBox_Validated);
            // 
            // AlternativeTextLabel
            // 
            resources.ApplyResources(this.AlternativeTextLabel, "AlternativeTextLabel");
            this.AlternativeTextLabel.Name = "AlternativeTextLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.MediaListView, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // MediaListView
            // 
            this.MediaListView.ContextMenuStrip = this.MediaListContextMenu;
            resources.ApplyResources(this.MediaListView, "MediaListView");
            this.MediaListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.MediaListView.HideSelection = false;
            this.MediaListView.MultiSelect = false;
            this.MediaListView.Name = "MediaListView";
            this.MediaListView.ShowGroups = false;
            this.MediaListView.UseCompatibleStateImageBehavior = false;
            this.MediaListView.SelectedIndexChanged += new System.EventHandler(this.MediaListView_SelectedIndexChanged);
            // 
            // MediaListContextMenu
            // 
            this.MediaListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MoveToBackMenuItem,
            this.MoveToNextMenuItem,
            this.DeleteMediaMenuItem});
            this.MediaListContextMenu.Name = "MediaListContextMenu";
            resources.ApplyResources(this.MediaListContextMenu, "MediaListContextMenu");
            // 
            // MoveToBackMenuItem
            // 
            this.MoveToBackMenuItem.Name = "MoveToBackMenuItem";
            resources.ApplyResources(this.MoveToBackMenuItem, "MoveToBackMenuItem");
            // 
            // MoveToNextMenuItem
            // 
            this.MoveToNextMenuItem.Name = "MoveToNextMenuItem";
            resources.ApplyResources(this.MoveToNextMenuItem, "MoveToNextMenuItem");
            // 
            // DeleteMediaMenuItem
            // 
            this.DeleteMediaMenuItem.Name = "DeleteMediaMenuItem";
            resources.ApplyResources(this.DeleteMediaMenuItem, "DeleteMediaMenuItem");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ImageCancelButton);
            this.panel1.Controls.Add(this.AddMediaButton);
            this.panel1.Controls.Add(this.ServiceSelectPanel);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // AddMediaButton
            // 
            resources.ApplyResources(this.AddMediaButton, "AddMediaButton");
            this.AddMediaButton.Name = "AddMediaButton";
            this.AddMediaButton.UseVisualStyleBackColor = true;
            this.AddMediaButton.Click += new System.EventHandler(this.AddMediaButton_Click);
            // 
            // ServiceSelectPanel
            // 
            resources.ApplyResources(this.ServiceSelectPanel, "ServiceSelectPanel");
            this.ServiceSelectPanel.Controls.Add(this.ImageServiceCombo);
            this.ServiceSelectPanel.Controls.Add(this.Label2);
            this.ServiceSelectPanel.Name = "ServiceSelectPanel";
            // 
            // ImageSelectedPicture
            // 
            resources.ApplyResources(this.ImageSelectedPicture, "ImageSelectedPicture");
            this.ImageSelectedPicture.Name = "ImageSelectedPicture";
            this.ImageSelectedPicture.TabStop = false;
            // 
            // MediaSelectorPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.ImageSelectedPicture);
            this.Controls.Add(this.AlternativeTextPanel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MediaSelectorPanel";
            this.AlternativeTextPanel.ResumeLayout(false);
            this.AlternativeTextPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.MediaListContextMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ServiceSelectPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ImageSelectedPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal OTPictureBox ImageSelectedPicture;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.ComboBox ImageServiceCombo;
        internal System.Windows.Forms.Button ImageCancelButton;
        internal System.Windows.Forms.Panel AlternativeTextPanel;
        internal System.Windows.Forms.TextBox AlternativeTextBox;
        internal System.Windows.Forms.Label AlternativeTextLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListView MediaListView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button AddMediaButton;
        private System.Windows.Forms.Panel ServiceSelectPanel;
        private System.Windows.Forms.ContextMenuStrip MediaListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem MoveToBackMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveToNextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeleteMediaMenuItem;
    }
}
