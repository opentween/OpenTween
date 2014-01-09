namespace OpenTween.Setting.Panel
{
    partial class GetPeriodPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetPeriodPanel));
            this.UserstreamPeriod = new System.Windows.Forms.TextBox();
            this.Label46 = new System.Windows.Forms.Label();
            this.LabelUserStreamActive = new System.Windows.Forms.Label();
            this.Label21 = new System.Windows.Forms.Label();
            this.UserTimelinePeriod = new System.Windows.Forms.TextBox();
            this.TimelinePeriod = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.LabelPostAndGet = new System.Windows.Forms.Label();
            this.Label33 = new System.Windows.Forms.Label();
            this.ListsPeriod = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.PubSearchPeriod = new System.Windows.Forms.TextBox();
            this.Label69 = new System.Windows.Forms.Label();
            this.ReplyPeriod = new System.Windows.Forms.TextBox();
            this.CheckPostAndGet = new System.Windows.Forms.CheckBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.DMPeriod = new System.Windows.Forms.TextBox();
            this.StartupUserstreamCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // UserstreamPeriod
            // 
            resources.ApplyResources(this.UserstreamPeriod, "UserstreamPeriod");
            this.UserstreamPeriod.Name = "UserstreamPeriod";
            this.UserstreamPeriod.Validating += new System.ComponentModel.CancelEventHandler(this.UserstreamPeriod_Validating);
            // 
            // Label46
            // 
            resources.ApplyResources(this.Label46, "Label46");
            this.Label46.Name = "Label46";
            // 
            // LabelUserStreamActive
            // 
            resources.ApplyResources(this.LabelUserStreamActive, "LabelUserStreamActive");
            this.LabelUserStreamActive.Name = "LabelUserStreamActive";
            // 
            // Label21
            // 
            resources.ApplyResources(this.Label21, "Label21");
            this.Label21.Name = "Label21";
            // 
            // UserTimelinePeriod
            // 
            resources.ApplyResources(this.UserTimelinePeriod, "UserTimelinePeriod");
            this.UserTimelinePeriod.Name = "UserTimelinePeriod";
            // 
            // TimelinePeriod
            // 
            resources.ApplyResources(this.TimelinePeriod, "TimelinePeriod");
            this.TimelinePeriod.Name = "TimelinePeriod";
            this.TimelinePeriod.Validating += new System.ComponentModel.CancelEventHandler(this.TimelinePeriod_Validating);
            // 
            // Label3
            // 
            resources.ApplyResources(this.Label3, "Label3");
            this.Label3.Name = "Label3";
            // 
            // LabelPostAndGet
            // 
            resources.ApplyResources(this.LabelPostAndGet, "LabelPostAndGet");
            this.LabelPostAndGet.Name = "LabelPostAndGet";
            // 
            // Label33
            // 
            resources.ApplyResources(this.Label33, "Label33");
            this.Label33.Name = "Label33";
            // 
            // ListsPeriod
            // 
            resources.ApplyResources(this.ListsPeriod, "ListsPeriod");
            this.ListsPeriod.Name = "ListsPeriod";
            this.ListsPeriod.Validating += new System.ComponentModel.CancelEventHandler(this.ListsPeriod_Validating);
            // 
            // Label7
            // 
            resources.ApplyResources(this.Label7, "Label7");
            this.Label7.Name = "Label7";
            // 
            // PubSearchPeriod
            // 
            resources.ApplyResources(this.PubSearchPeriod, "PubSearchPeriod");
            this.PubSearchPeriod.Name = "PubSearchPeriod";
            this.PubSearchPeriod.Validating += new System.ComponentModel.CancelEventHandler(this.PubSearchPeriod_Validating);
            // 
            // Label69
            // 
            resources.ApplyResources(this.Label69, "Label69");
            this.Label69.Name = "Label69";
            // 
            // ReplyPeriod
            // 
            resources.ApplyResources(this.ReplyPeriod, "ReplyPeriod");
            this.ReplyPeriod.Name = "ReplyPeriod";
            this.ReplyPeriod.Validating += new System.ComponentModel.CancelEventHandler(this.ReplyPeriod_Validating);
            // 
            // CheckPostAndGet
            // 
            resources.ApplyResources(this.CheckPostAndGet, "CheckPostAndGet");
            this.CheckPostAndGet.Name = "CheckPostAndGet";
            this.CheckPostAndGet.UseVisualStyleBackColor = true;
            // 
            // Label5
            // 
            resources.ApplyResources(this.Label5, "Label5");
            this.Label5.Name = "Label5";
            // 
            // DMPeriod
            // 
            resources.ApplyResources(this.DMPeriod, "DMPeriod");
            this.DMPeriod.Name = "DMPeriod";
            this.DMPeriod.Validating += new System.ComponentModel.CancelEventHandler(this.DMPeriod_Validating);
            // 
            // StartupUserstreamCheck
            // 
            resources.ApplyResources(this.StartupUserstreamCheck, "StartupUserstreamCheck");
            this.StartupUserstreamCheck.Name = "StartupUserstreamCheck";
            this.StartupUserstreamCheck.UseVisualStyleBackColor = true;
            // 
            // GetPeriodPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.UserstreamPeriod);
            this.Controls.Add(this.Label46);
            this.Controls.Add(this.LabelUserStreamActive);
            this.Controls.Add(this.Label21);
            this.Controls.Add(this.UserTimelinePeriod);
            this.Controls.Add(this.TimelinePeriod);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.LabelPostAndGet);
            this.Controls.Add(this.Label33);
            this.Controls.Add(this.ListsPeriod);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.PubSearchPeriod);
            this.Controls.Add(this.Label69);
            this.Controls.Add(this.ReplyPeriod);
            this.Controls.Add(this.CheckPostAndGet);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.DMPeriod);
            this.Controls.Add(this.StartupUserstreamCheck);
            this.Name = "GetPeriodPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox UserstreamPeriod;
        internal System.Windows.Forms.Label Label46;
        internal System.Windows.Forms.Label LabelUserStreamActive;
        internal System.Windows.Forms.Label Label21;
        internal System.Windows.Forms.TextBox UserTimelinePeriod;
        internal System.Windows.Forms.TextBox TimelinePeriod;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label LabelPostAndGet;
        internal System.Windows.Forms.Label Label33;
        internal System.Windows.Forms.TextBox ListsPeriod;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.TextBox PubSearchPeriod;
        internal System.Windows.Forms.Label Label69;
        internal System.Windows.Forms.TextBox ReplyPeriod;
        internal System.Windows.Forms.CheckBox CheckPostAndGet;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.TextBox DMPeriod;
        internal System.Windows.Forms.CheckBox StartupUserstreamCheck;
    }
}
