namespace OpenTween.Setting.Panel
{
    partial class ShortUrlPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShortUrlPanel));
            this.ShortenTcoCheck = new System.Windows.Forms.CheckBox();
            this.CheckTinyURL = new System.Windows.Forms.CheckBox();
            this.TextBitlyAccessToken = new System.Windows.Forms.TextBox();
            this.CheckAutoConvertUrl = new System.Windows.Forms.CheckBox();
            this.Label71 = new System.Windows.Forms.Label();
            this.ComboBoxAutoShortUrlFirst = new System.Windows.Forms.ComboBox();
            this.Label77 = new System.Windows.Forms.Label();
            this.ButtonBitlyAuthorize = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ShortenTcoCheck
            // 
            resources.ApplyResources(this.ShortenTcoCheck, "ShortenTcoCheck");
            this.ShortenTcoCheck.Name = "ShortenTcoCheck";
            this.ShortenTcoCheck.UseVisualStyleBackColor = true;
            // 
            // CheckTinyURL
            // 
            resources.ApplyResources(this.CheckTinyURL, "CheckTinyURL");
            this.CheckTinyURL.Name = "CheckTinyURL";
            this.CheckTinyURL.UseVisualStyleBackColor = true;
            // 
            // TextBitlyAccessToken
            // 
            resources.ApplyResources(this.TextBitlyAccessToken, "TextBitlyAccessToken");
            this.TextBitlyAccessToken.Name = "TextBitlyAccessToken";
            // 
            // CheckAutoConvertUrl
            // 
            resources.ApplyResources(this.CheckAutoConvertUrl, "CheckAutoConvertUrl");
            this.CheckAutoConvertUrl.Name = "CheckAutoConvertUrl";
            this.CheckAutoConvertUrl.UseVisualStyleBackColor = true;
            this.CheckAutoConvertUrl.CheckedChanged += new System.EventHandler(this.CheckAutoConvertUrl_CheckedChanged);
            // 
            // Label71
            // 
            resources.ApplyResources(this.Label71, "Label71");
            this.Label71.Name = "Label71";
            // 
            // ComboBoxAutoShortUrlFirst
            // 
            this.ComboBoxAutoShortUrlFirst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxAutoShortUrlFirst.FormattingEnabled = true;
            this.ComboBoxAutoShortUrlFirst.Items.AddRange(new object[] {
            resources.GetString("ComboBoxAutoShortUrlFirst.Items"),
            resources.GetString("ComboBoxAutoShortUrlFirst.Items1"),
            resources.GetString("ComboBoxAutoShortUrlFirst.Items2"),
            resources.GetString("ComboBoxAutoShortUrlFirst.Items3"),
            resources.GetString("ComboBoxAutoShortUrlFirst.Items4"),
            resources.GetString("ComboBoxAutoShortUrlFirst.Items5")});
            resources.ApplyResources(this.ComboBoxAutoShortUrlFirst, "ComboBoxAutoShortUrlFirst");
            this.ComboBoxAutoShortUrlFirst.Name = "ComboBoxAutoShortUrlFirst";
            this.ComboBoxAutoShortUrlFirst.SelectedIndexChanged += new System.EventHandler(this.ComboBoxAutoShortUrlFirst_SelectedIndexChanged);
            // 
            // Label77
            // 
            resources.ApplyResources(this.Label77, "Label77");
            this.Label77.Name = "Label77";
            // 
            // ButtonBitlyAuthorize
            // 
            resources.ApplyResources(this.ButtonBitlyAuthorize, "ButtonBitlyAuthorize");
            this.ButtonBitlyAuthorize.Name = "ButtonBitlyAuthorize";
            this.ButtonBitlyAuthorize.UseVisualStyleBackColor = true;
            this.ButtonBitlyAuthorize.Click += new System.EventHandler(this.ButtonBitlyAuthorize_Click);
            // 
            // ShortUrlPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.ShortenTcoCheck);
            this.Controls.Add(this.CheckTinyURL);
            this.Controls.Add(this.TextBitlyAccessToken);
            this.Controls.Add(this.CheckAutoConvertUrl);
            this.Controls.Add(this.Label71);
            this.Controls.Add(this.ComboBoxAutoShortUrlFirst);
            this.Controls.Add(this.ButtonBitlyAuthorize);
            this.Controls.Add(this.Label77);
            this.Name = "ShortUrlPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox ShortenTcoCheck;
        internal System.Windows.Forms.CheckBox CheckTinyURL;
        internal System.Windows.Forms.CheckBox CheckAutoConvertUrl;
        internal System.Windows.Forms.Label Label71;
        internal System.Windows.Forms.ComboBox ComboBoxAutoShortUrlFirst;
        internal System.Windows.Forms.Label Label77;
        private System.Windows.Forms.Button ButtonBitlyAuthorize;
        private System.Windows.Forms.TextBox TextBitlyAccessToken;
    }
}
