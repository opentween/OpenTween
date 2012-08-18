namespace OpenTween
{
    partial class InputTabName
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputTabName));
            this.ComboUsage = new System.Windows.Forms.ComboBox();
            this.LabelUsage = new System.Windows.Forms.Label();
            this.TextTabName = new System.Windows.Forms.TextBox();
            this.LabelDescription = new System.Windows.Forms.Label();
            this.OK_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ComboUsage
            // 
            this.ComboUsage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboUsage.FormattingEnabled = true;
            resources.ApplyResources(this.ComboUsage, "ComboUsage");
            this.ComboUsage.Name = "ComboUsage";
            this.ComboUsage.SelectedIndexChanged += new System.EventHandler(this.ComboUsage_SelectedIndexChanged);
            // 
            // LabelUsage
            // 
            resources.ApplyResources(this.LabelUsage, "LabelUsage");
            this.LabelUsage.Name = "LabelUsage";
            // 
            // TextTabName
            // 
            resources.ApplyResources(this.TextTabName, "TextTabName");
            this.TextTabName.Name = "TextTabName";
            // 
            // LabelDescription
            // 
            resources.ApplyResources(this.LabelDescription, "LabelDescription");
            this.LabelDescription.Name = "LabelDescription";
            // 
            // OK_Button
            // 
            resources.ApplyResources(this.OK_Button, "OK_Button");
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // Cancel_Button
            // 
            resources.ApplyResources(this.Cancel_Button, "Cancel_Button");
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // TableLayoutPanel1
            // 
            resources.ApplyResources(this.TableLayoutPanel1, "TableLayoutPanel1");
            this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            // 
            // InputTabName
            // 
            this.AcceptButton = this.OK_Button;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.Controls.Add(this.ComboUsage);
            this.Controls.Add(this.LabelUsage);
            this.Controls.Add(this.TextTabName);
            this.Controls.Add(this.LabelDescription);
            this.Controls.Add(this.TableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputTabName";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.InputTabName_Load);
            this.Shown += new System.EventHandler(this.InputTabName_Shown);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ComboBox ComboUsage;
        internal System.Windows.Forms.Label LabelUsage;
        internal System.Windows.Forms.TextBox TextTabName;
        internal System.Windows.Forms.Label LabelDescription;
        internal System.Windows.Forms.Button OK_Button;
        internal System.Windows.Forms.Button Cancel_Button;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
    }
}