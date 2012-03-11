namespace OpenTween
{
    partial class AtIdSupplement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AtIdSupplement));
            this.Label1 = new System.Windows.Forms.Label();
            this.TextId = new System.Windows.Forms.TextBox();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Label1
            // 
            resources.ApplyResources(this.Label1, "Label1");
            this.Label1.Name = "Label1";
            // 
            // TextId
            // 
            this.TextId.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.TextId.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            resources.ApplyResources(this.TextId, "TextId");
            this.TextId.Name = "TextId";
            this.TextId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextId_KeyDown);
            this.TextId.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TextId_PreviewKeyDown);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.ButtonCancel, "ButtonCancel");
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.TabStop = false;
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.ButtonOK, "ButtonOK");
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.TabStop = false;
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // AtIdSupplement
            // 
            this.AcceptButton = this.ButtonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.TextId);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AtIdSupplement";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AtIdSupplement_FormClosed);
            this.Load += new System.EventHandler(this.AtIdSupplement_Load);
            this.Shown += new System.EventHandler(this.AtIdSupplement_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox TextId;
        internal System.Windows.Forms.Button ButtonCancel;
        internal System.Windows.Forms.Button ButtonOK;
        internal System.Windows.Forms.Label label2;

    }
}