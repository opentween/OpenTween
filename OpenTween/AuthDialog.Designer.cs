namespace OpenTween
{
    partial class AuthDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.AuthLinkLabel = new System.Windows.Forms.LinkLabel();
            this.contextMenuLinkLabel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemCopyURL = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.PinTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            this.contextMenuLinkLabel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // AuthLinkLabel
            // 
            this.AuthLinkLabel.AutoEllipsis = true;
            this.AuthLinkLabel.ContextMenuStrip = this.contextMenuLinkLabel;
            resources.ApplyResources(this.AuthLinkLabel, "AuthLinkLabel");
            this.AuthLinkLabel.Name = "AuthLinkLabel";
            this.AuthLinkLabel.TabStop = true;
            this.AuthLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AuthLinkLabel_LinkClicked);
            // 
            // contextMenuLinkLabel
            // 
            this.contextMenuLinkLabel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemCopyURL});
            this.contextMenuLinkLabel.Name = "contextMenuLinkLabel";
            resources.ApplyResources(this.contextMenuLinkLabel, "contextMenuLinkLabel");
            // 
            // MenuItemCopyURL
            // 
            this.MenuItemCopyURL.Name = "MenuItemCopyURL";
            resources.ApplyResources(this.MenuItemCopyURL, "MenuItemCopyURL");
            this.MenuItemCopyURL.Click += new System.EventHandler(this.MenuItemCopyURL_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // PinTextBox
            // 
            resources.ApplyResources(this.PinTextBox, "PinTextBox");
            this.PinTextBox.Name = "PinTextBox";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.CancelBtn, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.OKBtn, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // OKBtn
            // 
            resources.ApplyResources(this.OKBtn, "OKBtn");
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.UseVisualStyleBackColor = true;
            // 
            // AuthDialog
            // 
            this.AcceptButton = this.OKBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.PinTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AuthLinkLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AuthDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.contextMenuLinkLabel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel AuthLinkLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PinTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.ContextMenuStrip contextMenuLinkLabel;
        private System.Windows.Forms.ToolStripMenuItem MenuItemCopyURL;
    }
}