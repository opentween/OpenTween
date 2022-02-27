namespace OpenTween
{
    partial class SendErrorReportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SendErrorReportForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSendByMail = new System.Windows.Forms.Button();
            this.buttonSendByDM = new System.Windows.Forms.Button();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.buttonNotSend = new System.Windows.Forms.Button();
            this.textBoxErrorReport = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.checkBoxAnonymouns = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.buttonSendByMail, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonSendByDM, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonNotSend, 2, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // buttonSendByMail
            // 
            resources.ApplyResources(this.buttonSendByMail, "buttonSendByMail");
            this.buttonSendByMail.Name = "buttonSendByMail";
            this.buttonSendByMail.UseVisualStyleBackColor = true;
            this.buttonSendByMail.Click += new System.EventHandler(this.ButtonSendByMail_Click);
            // 
            // buttonSendByDM
            // 
            this.buttonSendByDM.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.bindingSource, "CanSendByDM", true));
            resources.ApplyResources(this.buttonSendByDM, "buttonSendByDM");
            this.buttonSendByDM.Name = "buttonSendByDM";
            this.buttonSendByDM.UseVisualStyleBackColor = true;
            this.buttonSendByDM.Click += new System.EventHandler(this.ButtonSendByDM_Click);
            // 
            // bindingSource
            // 
            this.bindingSource.DataSource = typeof(OpenTween.ErrorReport);
            // 
            // buttonNotSend
            // 
            this.buttonNotSend.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonNotSend, "buttonNotSend");
            this.buttonNotSend.Name = "buttonNotSend";
            this.buttonNotSend.UseVisualStyleBackColor = true;
            this.buttonNotSend.Click += new System.EventHandler(this.ButtonNotSend_Click);
            // 
            // textBoxErrorReport
            // 
            this.textBoxErrorReport.AcceptsReturn = true;
            resources.ApplyResources(this.textBoxErrorReport, "textBoxErrorReport");
            this.textBoxErrorReport.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "ReportText", true));
            this.textBoxErrorReport.Name = "textBoxErrorReport";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // buttonReset
            // 
            resources.ApplyResources(this.buttonReset, "buttonReset");
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // pictureBoxIcon
            // 
            resources.ApplyResources(this.pictureBoxIcon, "pictureBoxIcon");
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.TabStop = false;
            // 
            // checkBoxAnonymouns
            // 
            resources.ApplyResources(this.checkBoxAnonymouns, "checkBoxAnonymouns");
            this.checkBoxAnonymouns.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSource, "AnonymousReport", true));
            this.checkBoxAnonymouns.Name = "checkBoxAnonymouns";
            this.checkBoxAnonymouns.UseVisualStyleBackColor = true;
            // 
            // SendErrorReportForm
            // 
            this.AcceptButton = this.buttonSendByDM;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonNotSend;
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxErrorReport);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.checkBoxAnonymouns);
            this.Name = "SendErrorReportForm";
            this.ShowIcon = false;
            this.Shown += new System.EventHandler(this.SendErrorReportForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonSendByMail;
        private System.Windows.Forms.Button buttonSendByDM;
        private System.Windows.Forms.TextBox textBoxErrorReport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonNotSend;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.BindingSource bindingSource;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.CheckBox checkBoxAnonymouns;
    }
}