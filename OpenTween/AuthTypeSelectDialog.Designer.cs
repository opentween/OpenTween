namespace OpenTween
{
    partial class AuthTypeSelectDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthTypeSelectDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.AuthByOAuth2RadioButton = new System.Windows.Forms.RadioButton();
            this.AuthByOAuth1RadioButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.OAuth1ConsumerKeyTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.OAuth1ConsumerSecretTextBox = new System.Windows.Forms.TextBox();
            this.UseTwitterComCookieRadioButton = new System.Windows.Forms.RadioButton();
            this.TwitterComCookieTextBox = new System.Windows.Forms.TextBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // AuthByOAuth2RadioButton
            // 
            resources.ApplyResources(this.AuthByOAuth2RadioButton, "AuthByOAuth2RadioButton");
            this.AuthByOAuth2RadioButton.Name = "AuthByOAuth2RadioButton";
            this.AuthByOAuth2RadioButton.TabStop = true;
            this.AuthByOAuth2RadioButton.UseVisualStyleBackColor = true;
            // 
            // AuthByOAuth1RadioButton
            // 
            resources.ApplyResources(this.AuthByOAuth1RadioButton, "AuthByOAuth1RadioButton");
            this.AuthByOAuth1RadioButton.Name = "AuthByOAuth1RadioButton";
            this.AuthByOAuth1RadioButton.TabStop = true;
            this.AuthByOAuth1RadioButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // OAuth1ConsumerKeyTextBox
            // 
            resources.ApplyResources(this.OAuth1ConsumerKeyTextBox, "OAuth1ConsumerKeyTextBox");
            this.OAuth1ConsumerKeyTextBox.Name = "OAuth1ConsumerKeyTextBox";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // OAuth1ConsumerSecretTextBox
            // 
            resources.ApplyResources(this.OAuth1ConsumerSecretTextBox, "OAuth1ConsumerSecretTextBox");
            this.OAuth1ConsumerSecretTextBox.Name = "OAuth1ConsumerSecretTextBox";
            // 
            // UseTwitterComCookieRadioButton
            // 
            resources.ApplyResources(this.UseTwitterComCookieRadioButton, "UseTwitterComCookieRadioButton");
            this.UseTwitterComCookieRadioButton.Name = "UseTwitterComCookieRadioButton";
            this.UseTwitterComCookieRadioButton.TabStop = true;
            this.UseTwitterComCookieRadioButton.UseVisualStyleBackColor = true;
            // 
            // TwitterComCookieTextBox
            // 
            resources.ApplyResources(this.TwitterComCookieTextBox, "TwitterComCookieTextBox");
            this.TwitterComCookieTextBox.Name = "TwitterComCookieTextBox";
            // 
            // OKButton
            // 
            resources.ApplyResources(this.OKButton, "OKButton");
            this.OKButton.Name = "OKButton";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // AuthTypeSelectDialog
            // 
            this.AcceptButton = this.OKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AuthByOAuth2RadioButton);
            this.Controls.Add(this.AuthByOAuth1RadioButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OAuth1ConsumerKeyTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.OAuth1ConsumerSecretTextBox);
            this.Controls.Add(this.UseTwitterComCookieRadioButton);
            this.Controls.Add(this.TwitterComCookieTextBox);
            this.Controls.Add(this.OKButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AuthTypeSelectDialog";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton AuthByOAuth2RadioButton;
        private System.Windows.Forms.RadioButton AuthByOAuth1RadioButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox OAuth1ConsumerKeyTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox OAuth1ConsumerSecretTextBox;
        private System.Windows.Forms.RadioButton UseTwitterComCookieRadioButton;
        private System.Windows.Forms.TextBox TwitterComCookieTextBox;
        private System.Windows.Forms.Button OKButton;
    }
}