namespace OpenTween
{
    partial class MyLists
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyLists));
            this.更新RToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ListRefreshButton = new System.Windows.Forms.Button();
            this.ListsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.ContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.追加AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.削除DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.CloseButton = new System.Windows.Forms.Button();
            this.ContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // 更新RToolStripMenuItem
            // 
            this.更新RToolStripMenuItem.Name = "更新RToolStripMenuItem";
            resources.ApplyResources(this.更新RToolStripMenuItem, "更新RToolStripMenuItem");
            this.更新RToolStripMenuItem.Click += new System.EventHandler(this.更新RToolStripMenuItem_Click);
            // 
            // ListRefreshButton
            // 
            resources.ApplyResources(this.ListRefreshButton, "ListRefreshButton");
            this.ListRefreshButton.Name = "ListRefreshButton";
            this.ListRefreshButton.UseVisualStyleBackColor = true;
            this.ListRefreshButton.Click += new System.EventHandler(this.ListRefreshButton_Click);
            // 
            // ListsCheckedListBox
            // 
            resources.ApplyResources(this.ListsCheckedListBox, "ListsCheckedListBox");
            this.ListsCheckedListBox.CheckOnClick = true;
            this.ListsCheckedListBox.ContextMenuStrip = this.ContextMenuStrip1;
            this.ListsCheckedListBox.FormattingEnabled = true;
            this.ListsCheckedListBox.Name = "ListsCheckedListBox";
            this.ListsCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListsCheckedListBox_ItemCheck);
            this.ListsCheckedListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListsCheckedListBox_MouseDown);
            // 
            // ContextMenuStrip1
            // 
            this.ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.追加AToolStripMenuItem,
            this.削除DToolStripMenuItem,
            this.ToolStripMenuItem1,
            this.更新RToolStripMenuItem});
            this.ContextMenuStrip1.Name = "ContextMenuStrip1";
            resources.ApplyResources(this.ContextMenuStrip1, "ContextMenuStrip1");
            this.ContextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1_Opening);
            // 
            // 追加AToolStripMenuItem
            // 
            this.追加AToolStripMenuItem.Name = "追加AToolStripMenuItem";
            resources.ApplyResources(this.追加AToolStripMenuItem, "追加AToolStripMenuItem");
            this.追加AToolStripMenuItem.Click += new System.EventHandler(this.追加AToolStripMenuItem_Click);
            // 
            // 削除DToolStripMenuItem
            // 
            this.削除DToolStripMenuItem.Name = "削除DToolStripMenuItem";
            resources.ApplyResources(this.削除DToolStripMenuItem, "削除DToolStripMenuItem");
            this.削除DToolStripMenuItem.Click += new System.EventHandler(this.削除DToolStripMenuItem_Click);
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            resources.ApplyResources(this.ToolStripMenuItem1, "ToolStripMenuItem1");
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // MyLists
            // 
            this.AcceptButton = this.CloseButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.ListRefreshButton);
            this.Controls.Add(this.ListsCheckedListBox);
            this.Controls.Add(this.CloseButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MyLists";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.MyLists_Load);
            this.ContextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ToolStripMenuItem 更新RToolStripMenuItem;
        internal System.Windows.Forms.Button ListRefreshButton;
        internal System.Windows.Forms.CheckedListBox ListsCheckedListBox;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem 追加AToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem 削除DToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
        internal System.Windows.Forms.Button CloseButton;
    }
}