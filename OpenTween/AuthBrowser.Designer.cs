namespace OpenTween
{
    partial class AuthBrowser
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.AuthWebBrowser = new System.Windows.Forms.WebBrowser();
            this.Cancel = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.PinText = new System.Windows.Forms.TextBox();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.AddressLabel = new System.Windows.Forms.Label();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.NextButton = new System.Windows.Forms.Button();
            this.Panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // AuthWebBrowser
            // 
            this.AuthWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AuthWebBrowser.Location = new System.Drawing.Point(0, 22);
            this.AuthWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.AuthWebBrowser.Name = "AuthWebBrowser";
            this.AuthWebBrowser.Size = new System.Drawing.Size(773, 540);
            this.AuthWebBrowser.TabIndex = 4;
            this.AuthWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.AuthWebBrowser_DocumentCompleted);
            this.AuthWebBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.AuthWebBrowser_Navigating);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(536, 32);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 15);
            this.Cancel.TabIndex = 3;
            this.Cancel.TabStop = false;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.Label1.Location = new System.Drawing.Point(531, 0);
            this.Label1.Name = "Label1";
            this.Label1.Padding = new System.Windows.Forms.Padding(3);
            this.Label1.Size = new System.Drawing.Size(29, 18);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "PIN";
            // 
            // PinText
            // 
            this.PinText.Dock = System.Windows.Forms.DockStyle.Right;
            this.PinText.Location = new System.Drawing.Point(560, 0);
            this.PinText.Name = "PinText";
            this.PinText.Size = new System.Drawing.Size(138, 19);
            this.PinText.TabIndex = 1;
            // 
            // Panel1
            // 
            this.Panel1.AutoSize = true;
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel1.Location = new System.Drawing.Point(0, 22);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(773, 0);
            this.Panel1.TabIndex = 3;
            // 
            // AddressLabel
            // 
            this.AddressLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AddressLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddressLabel.Location = new System.Drawing.Point(0, 0);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new System.Drawing.Size(531, 22);
            this.AddressLabel.TabIndex = 0;
            this.AddressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Panel2
            // 
            this.Panel2.Controls.Add(this.Cancel);
            this.Panel2.Controls.Add(this.AddressLabel);
            this.Panel2.Controls.Add(this.Label1);
            this.Panel2.Controls.Add(this.PinText);
            this.Panel2.Controls.Add(this.NextButton);
            this.Panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel2.Location = new System.Drawing.Point(0, 0);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(773, 22);
            this.Panel2.TabIndex = 5;
            // 
            // NextButton
            // 
            this.NextButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.NextButton.Location = new System.Drawing.Point(698, 0);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(75, 22);
            this.NextButton.TabIndex = 2;
            this.NextButton.Text = "Finish";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // AuthBrowser
            // 
            this.AcceptButton = this.NextButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(773, 562);
            this.Controls.Add(this.AuthWebBrowser);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.Panel2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AuthBrowser";
            this.ShowIcon = false;
            this.Text = "Browser";
            this.Load += new System.EventHandler(this.AuthBrowser_Load);
            this.Panel2.ResumeLayout(false);
            this.Panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.WebBrowser AuthWebBrowser;
        internal System.Windows.Forms.Button Cancel;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox PinText;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.Label AddressLabel;
        internal System.Windows.Forms.Panel Panel2;
        internal System.Windows.Forms.Button NextButton;
    }
}