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
            this.MenuItemReload = new System.Windows.Forms.ToolStripMenuItem();
            this.ListRefreshButton = new System.Windows.Forms.Button();
            this.ListsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.ContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.CloseButton = new System.Windows.Forms.Button();
            this.ContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuItemReload
            // 
            this.MenuItemReload.Name = "MenuItemReload";
            resources.ApplyResources(this.MenuItemReload, "MenuItemReload");
            this.MenuItemReload.Click += new System.EventHandler(this.MenuItemReload_Click);
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
            this.ListsCheckedListBox.ContextMenuStrip = this.ContextMenuStrip1;
            this.ListsCheckedListBox.FormattingEnabled = true;
            this.ListsCheckedListBox.Name = "ListsCheckedListBox";
            this.ListsCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListsCheckedListBox_ItemCheck);
            this.ListsCheckedListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListsCheckedListBox_MouseDown);
            // 
            // ContextMenuStrip1
            // 
            this.ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemAdd,
            this.MenuItemDelete,
            this.ToolStripMenuItem1,
            this.MenuItemReload});
            this.ContextMenuStrip1.Name = "ContextMenuStrip1";
            resources.ApplyResources(this.ContextMenuStrip1, "ContextMenuStrip1");
            this.ContextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1_Opening);
            // 
            // MenuItemAdd
            // 
            this.MenuItemAdd.Name = "MenuItemAdd";
            resources.ApplyResources(this.MenuItemAdd, "MenuItemAdd");
            this.MenuItemAdd.Click += new System.EventHandler(this.MenuItemAdd_Click);
            // 
            // MenuItemDelete
            // 
            this.MenuItemDelete.Name = "MenuItemDelete";
            resources.ApplyResources(this.MenuItemDelete, "MenuItemDelete");
            this.MenuItemDelete.Click += new System.EventHandler(this.MenuItemDelete_Click);
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
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
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

        internal System.Windows.Forms.ToolStripMenuItem MenuItemReload;
        internal System.Windows.Forms.Button ListRefreshButton;
        internal System.Windows.Forms.CheckedListBox ListsCheckedListBox;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem MenuItemAdd;
        internal System.Windows.Forms.ToolStripMenuItem MenuItemDelete;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
        internal System.Windows.Forms.Button CloseButton;
    }
}