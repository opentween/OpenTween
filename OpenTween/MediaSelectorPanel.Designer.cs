namespace OpenTween
{
    partial class MediaSelectorPanel
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

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MediaSelectorPanel));
            this.ImagePathPanel = new System.Windows.Forms.Panel();
            this.ImagefilePathText = new System.Windows.Forms.TextBox();
            this.ImagePageCombo = new System.Windows.Forms.ComboBox();
            this.FilePickButton = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.ImageServiceCombo = new System.Windows.Forms.ComboBox();
            this.ImageCancelButton = new System.Windows.Forms.Button();
            this.AlternativeTextPanel = new System.Windows.Forms.Panel();
            this.AlternativeTextBox = new System.Windows.Forms.TextBox();
            this.AlternativeTextLabel = new System.Windows.Forms.Label();
            this.ImageSelectedPicture = new OpenTween.OTPictureBox();
            this.ImagePathPanel.SuspendLayout();
            this.AlternativeTextPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageSelectedPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // ImagePathPanel
            // 
            this.ImagePathPanel.Controls.Add(this.ImagefilePathText);
            this.ImagePathPanel.Controls.Add(this.ImagePageCombo);
            this.ImagePathPanel.Controls.Add(this.FilePickButton);
            this.ImagePathPanel.Controls.Add(this.Label2);
            this.ImagePathPanel.Controls.Add(this.ImageServiceCombo);
            this.ImagePathPanel.Controls.Add(this.ImageCancelButton);
            resources.ApplyResources(this.ImagePathPanel, "ImagePathPanel");
            this.ImagePathPanel.Name = "ImagePathPanel";
            // 
            // ImagefilePathText
            // 
            resources.ApplyResources(this.ImagefilePathText, "ImagefilePathText");
            this.ImagefilePathText.Name = "ImagefilePathText";
            this.ImagefilePathText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImageSelection_KeyDown);
            this.ImagefilePathText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ImageSelection_KeyPress);
            this.ImagefilePathText.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ImageSelection_PreviewKeyDown);
            this.ImagefilePathText.Validating += new System.ComponentModel.CancelEventHandler(this.ImagefilePathText_Validating);
            // 
            // ImagePageCombo
            // 
            resources.ApplyResources(this.ImagePageCombo, "ImagePageCombo");
            this.ImagePageCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ImagePageCombo.FormattingEnabled = true;
            this.ImagePageCombo.Name = "ImagePageCombo";
            this.ImagePageCombo.SelectedIndexChanged += new System.EventHandler(this.ImagePageCombo_SelectedIndexChanged);
            this.ImagePageCombo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImageSelection_KeyDown);
            this.ImagePageCombo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ImageSelection_KeyPress);
            this.ImagePageCombo.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ImageSelection_PreviewKeyDown);
            // 
            // FilePickButton
            // 
            resources.ApplyResources(this.FilePickButton, "FilePickButton");
            this.FilePickButton.Name = "FilePickButton";
            this.FilePickButton.UseVisualStyleBackColor = true;
            this.FilePickButton.Click += new System.EventHandler(this.FilePickButton_Click);
            this.FilePickButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImageSelection_KeyDown);
            this.FilePickButton.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ImageSelection_KeyPress);
            this.FilePickButton.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ImageSelection_PreviewKeyDown);
            // 
            // Label2
            // 
            resources.ApplyResources(this.Label2, "Label2");
            this.Label2.Name = "Label2";
            // 
            // ImageServiceCombo
            // 
            resources.ApplyResources(this.ImageServiceCombo, "ImageServiceCombo");
            this.ImageServiceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ImageServiceCombo.FormattingEnabled = true;
            this.ImageServiceCombo.Items.AddRange(new object[] {
            resources.GetString("ImageServiceCombo.Items")});
            this.ImageServiceCombo.Name = "ImageServiceCombo";
            this.ImageServiceCombo.SelectedIndexChanged += new System.EventHandler(this.ImageServiceCombo_SelectedIndexChanged);
            this.ImageServiceCombo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImageSelection_KeyDown);
            this.ImageServiceCombo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ImageSelection_KeyPress);
            this.ImageServiceCombo.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ImageSelection_PreviewKeyDown);
            // 
            // ImageCancelButton
            // 
            resources.ApplyResources(this.ImageCancelButton, "ImageCancelButton");
            this.ImageCancelButton.Name = "ImageCancelButton";
            this.ImageCancelButton.UseVisualStyleBackColor = true;
            this.ImageCancelButton.Click += new System.EventHandler(this.ImageCancelButton_Click);
            // 
            // AlternativeTextPanel
            // 
            resources.ApplyResources(this.AlternativeTextPanel, "AlternativeTextPanel");
            this.AlternativeTextPanel.Controls.Add(this.AlternativeTextBox);
            this.AlternativeTextPanel.Controls.Add(this.AlternativeTextLabel);
            this.AlternativeTextPanel.Name = "AlternativeTextPanel";
            // 
            // AlternativeTextBox
            // 
            resources.ApplyResources(this.AlternativeTextBox, "AlternativeTextBox");
            this.AlternativeTextBox.Name = "AlternativeTextBox";
            this.AlternativeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.AlternativeTextBox_Validating);
            // 
            // AlternativeTextLabel
            // 
            resources.ApplyResources(this.AlternativeTextLabel, "AlternativeTextLabel");
            this.AlternativeTextLabel.Name = "AlternativeTextLabel";
            // 
            // ImageSelectedPicture
            // 
            resources.ApplyResources(this.ImageSelectedPicture, "ImageSelectedPicture");
            this.ImageSelectedPicture.Name = "ImageSelectedPicture";
            this.ImageSelectedPicture.TabStop = false;
            // 
            // MediaSelectorPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.ImageSelectedPicture);
            this.Controls.Add(this.AlternativeTextPanel);
            this.Controls.Add(this.ImagePathPanel);
            this.Name = "MediaSelectorPanel";
            this.ImagePathPanel.ResumeLayout(false);
            this.ImagePathPanel.PerformLayout();
            this.AlternativeTextPanel.ResumeLayout(false);
            this.AlternativeTextPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageSelectedPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal OTPictureBox ImageSelectedPicture;
        internal System.Windows.Forms.Panel ImagePathPanel;
        internal System.Windows.Forms.TextBox ImagefilePathText;
        internal System.Windows.Forms.ComboBox ImagePageCombo;
        internal System.Windows.Forms.Button FilePickButton;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.ComboBox ImageServiceCombo;
        internal System.Windows.Forms.Button ImageCancelButton;
        internal System.Windows.Forms.Panel AlternativeTextPanel;
        internal System.Windows.Forms.TextBox AlternativeTextBox;
        internal System.Windows.Forms.Label AlternativeTextLabel;
    }
}
