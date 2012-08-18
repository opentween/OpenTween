namespace OpenTween
{
    partial class FormInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInfo));
            this.LabelInformation = new System.Windows.Forms.Label();
            this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // LabelInformation
            // 
            resources.ApplyResources(this.LabelInformation, "LabelInformation");
            this.LabelInformation.Name = "LabelInformation";
            this.LabelInformation.TextChanged += new System.EventHandler(this.LabelInformation_TextChanged);
            // 
            // ProgressBar1
            // 
            resources.ApplyResources(this.ProgressBar1, "ProgressBar1");
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // FormInfo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.LabelInformation);
            this.Controls.Add(this.ProgressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormInfo_FormClosed);
            this.Shown += new System.EventHandler(this.FormInfo_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Label LabelInformation;
        internal System.Windows.Forms.ProgressBar ProgressBar1;
    }
}