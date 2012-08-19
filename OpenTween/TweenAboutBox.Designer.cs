namespace OpenTween
{
    partial class TweenAboutBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TweenAboutBox));
            this.LogoPictureBox = new System.Windows.Forms.PictureBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.LabelProductName = new System.Windows.Forms.Label();
            this.LabelVersion = new System.Windows.Forms.Label();
            this.LabelCopyright = new System.Windows.Forms.Label();
            this.LabelCompanyName = new System.Windows.Forms.Label();
            this.TextBoxDescription = new System.Windows.Forms.TextBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.ChangeLog = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            this.TableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogoPictureBox
            // 
            resources.ApplyResources(this.LogoPictureBox, "LogoPictureBox");
            this.LogoPictureBox.Image = global::OpenTween.Properties.Resources.LogoImage;
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.TableLayoutPanel.SetRowSpan(this.LogoPictureBox, 5);
            this.LogoPictureBox.TabStop = false;
            // 
            // TableLayoutPanel
            // 
            resources.ApplyResources(this.TableLayoutPanel, "TableLayoutPanel");
            this.TableLayoutPanel.Controls.Add(this.LogoPictureBox, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.LabelProductName, 1, 0);
            this.TableLayoutPanel.Controls.Add(this.LabelVersion, 1, 1);
            this.TableLayoutPanel.Controls.Add(this.LabelCopyright, 1, 2);
            this.TableLayoutPanel.Controls.Add(this.LabelCompanyName, 1, 3);
            this.TableLayoutPanel.Controls.Add(this.TextBoxDescription, 1, 4);
            this.TableLayoutPanel.Controls.Add(this.OKButton, 1, 6);
            this.TableLayoutPanel.Controls.Add(this.ChangeLog, 0, 5);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            // 
            // LabelProductName
            // 
            resources.ApplyResources(this.LabelProductName, "LabelProductName");
            this.LabelProductName.MaximumSize = new System.Drawing.Size(0, 16);
            this.LabelProductName.Name = "LabelProductName";
            // 
            // LabelVersion
            // 
            resources.ApplyResources(this.LabelVersion, "LabelVersion");
            this.LabelVersion.MaximumSize = new System.Drawing.Size(0, 16);
            this.LabelVersion.Name = "LabelVersion";
            // 
            // LabelCopyright
            // 
            resources.ApplyResources(this.LabelCopyright, "LabelCopyright");
            this.LabelCopyright.MaximumSize = new System.Drawing.Size(0, 16);
            this.LabelCopyright.Name = "LabelCopyright";
            // 
            // LabelCompanyName
            // 
            resources.ApplyResources(this.LabelCompanyName, "LabelCompanyName");
            this.LabelCompanyName.MaximumSize = new System.Drawing.Size(0, 16);
            this.LabelCompanyName.Name = "LabelCompanyName";
            // 
            // TextBoxDescription
            // 
            resources.ApplyResources(this.TextBoxDescription, "TextBoxDescription");
            this.TextBoxDescription.Name = "TextBoxDescription";
            this.TextBoxDescription.ReadOnly = true;
            this.TextBoxDescription.TabStop = false;
            // 
            // OKButton
            // 
            resources.ApplyResources(this.OKButton, "OKButton");
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OKButton.Name = "OKButton";
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // ChangeLog
            // 
            this.TableLayoutPanel.SetColumnSpan(this.ChangeLog, 2);
            resources.ApplyResources(this.ChangeLog, "ChangeLog");
            this.ChangeLog.Name = "ChangeLog";
            this.ChangeLog.ReadOnly = true;
            // 
            // TweenAboutBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TableLayoutPanel);
            this.Name = "TweenAboutBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.TweenAboutBox_Load);
            this.Shown += new System.EventHandler(this.TweenAboutBox_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            this.TableLayoutPanel.ResumeLayout(false);
            this.TableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.PictureBox LogoPictureBox;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        internal System.Windows.Forms.Label LabelProductName;
        internal System.Windows.Forms.Label LabelVersion;
        internal System.Windows.Forms.Label LabelCopyright;
        internal System.Windows.Forms.Label LabelCompanyName;
        internal System.Windows.Forms.TextBox TextBoxDescription;
        internal System.Windows.Forms.Button OKButton;
        internal System.Windows.Forms.TextBox ChangeLog;
    }
}