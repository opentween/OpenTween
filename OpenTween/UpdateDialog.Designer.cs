namespace OpenTween
{
    partial class UpdateDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateDialog));
            this.TextDetail = new System.Windows.Forms.TextBox();
            this.LabelSummary = new System.Windows.Forms.Label();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.YesButton = new System.Windows.Forms.Button();
            this.NoButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.TableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TextDetail
            // 
            resources.ApplyResources(this.TextDetail, "TextDetail");
            this.TextDetail.Name = "TextDetail";
            this.TextDetail.ReadOnly = true;
            // 
            // LabelSummary
            // 
            resources.ApplyResources(this.LabelSummary, "LabelSummary");
            this.LabelSummary.Name = "LabelSummary";
            // 
            // PictureBox1
            // 
            resources.ApplyResources(this.PictureBox1, "PictureBox1");
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.TabStop = false;
            // 
            // TableLayoutPanel1
            // 
            resources.ApplyResources(this.TableLayoutPanel1, "TableLayoutPanel1");
            this.TableLayoutPanel1.Controls.Add(this.YesButton, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.NoButton, 1, 0);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            // 
            // YesButton
            // 
            resources.ApplyResources(this.YesButton, "YesButton");
            this.YesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.YesButton.Name = "YesButton";
            this.YesButton.Click += new System.EventHandler(this.YesButton_Click);
            // 
            // NoButton
            // 
            resources.ApplyResources(this.NoButton, "NoButton");
            this.NoButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.NoButton.Name = "NoButton";
            this.NoButton.Click += new System.EventHandler(this.NoButton_Click);
            // 
            // UpdateDialog
            // 
            this.AcceptButton = this.YesButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.NoButton;
            this.Controls.Add(this.TextDetail);
            this.Controls.Add(this.LabelSummary);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.TableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.UpdateDialog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.TableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox TextDetail;
        internal System.Windows.Forms.Label LabelSummary;
        internal System.Windows.Forms.PictureBox PictureBox1;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.Button YesButton;
        internal System.Windows.Forms.Button NoButton;
    }
}