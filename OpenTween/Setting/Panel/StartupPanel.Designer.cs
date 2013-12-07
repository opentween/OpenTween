namespace OpenTween.Setting.Panel
{
    partial class StartupPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartupPanel));
            this.StartupReaded = new System.Windows.Forms.CheckBox();
            this.CheckStartupFollowers = new System.Windows.Forms.CheckBox();
            this.CheckStartupVersion = new System.Windows.Forms.CheckBox();
            this.chkGetFav = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // StartupReaded
            // 
            resources.ApplyResources(this.StartupReaded, "StartupReaded");
            this.StartupReaded.Name = "StartupReaded";
            this.StartupReaded.UseVisualStyleBackColor = true;
            // 
            // CheckStartupFollowers
            // 
            resources.ApplyResources(this.CheckStartupFollowers, "CheckStartupFollowers");
            this.CheckStartupFollowers.Name = "CheckStartupFollowers";
            this.CheckStartupFollowers.UseVisualStyleBackColor = true;
            // 
            // CheckStartupVersion
            // 
            resources.ApplyResources(this.CheckStartupVersion, "CheckStartupVersion");
            this.CheckStartupVersion.Name = "CheckStartupVersion";
            this.CheckStartupVersion.UseVisualStyleBackColor = true;
            // 
            // chkGetFav
            // 
            resources.ApplyResources(this.chkGetFav, "chkGetFav");
            this.chkGetFav.Name = "chkGetFav";
            this.chkGetFav.UseVisualStyleBackColor = true;
            // 
            // StartupPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.StartupReaded);
            this.Controls.Add(this.CheckStartupFollowers);
            this.Controls.Add(this.CheckStartupVersion);
            this.Controls.Add(this.chkGetFav);
            this.Name = "StartupPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox StartupReaded;
        internal System.Windows.Forms.CheckBox CheckStartupFollowers;
        internal System.Windows.Forms.CheckBox CheckStartupVersion;
        internal System.Windows.Forms.CheckBox chkGetFav;
    }
}
