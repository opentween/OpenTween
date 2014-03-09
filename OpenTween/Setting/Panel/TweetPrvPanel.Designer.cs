namespace OpenTween.Setting.Panel
{
    partial class TweetPrvPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TweetPrvPanel));
            this.IsListsIncludeRtsCheckBox = new System.Windows.Forms.CheckBox();
            this.HideDuplicatedRetweetsCheck = new System.Windows.Forms.CheckBox();
            this.LabelDateTimeFormatApplied = new System.Windows.Forms.Label();
            this.Label62 = new System.Windows.Forms.Label();
            this.CmbDateTimeFormat = new System.Windows.Forms.ComboBox();
            this.Label23 = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.IconSize = new System.Windows.Forms.ComboBox();
            this.TextBox3 = new System.Windows.Forms.TextBox();
            this.CheckViewTabBottom = new System.Windows.Forms.CheckBox();
            this.CheckSortOrderLock = new System.Windows.Forms.CheckBox();
            this.CheckShowGrid = new System.Windows.Forms.CheckBox();
            this.chkUnreadStyle = new System.Windows.Forms.CheckBox();
            this.OneWayLv = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // IsListsIncludeRtsCheckBox
            // 
            resources.ApplyResources(this.IsListsIncludeRtsCheckBox, "IsListsIncludeRtsCheckBox");
            this.IsListsIncludeRtsCheckBox.Name = "IsListsIncludeRtsCheckBox";
            this.IsListsIncludeRtsCheckBox.UseVisualStyleBackColor = true;
            // 
            // HideDuplicatedRetweetsCheck
            // 
            resources.ApplyResources(this.HideDuplicatedRetweetsCheck, "HideDuplicatedRetweetsCheck");
            this.HideDuplicatedRetweetsCheck.Name = "HideDuplicatedRetweetsCheck";
            this.HideDuplicatedRetweetsCheck.UseVisualStyleBackColor = true;
            // 
            // LabelDateTimeFormatApplied
            // 
            resources.ApplyResources(this.LabelDateTimeFormatApplied, "LabelDateTimeFormatApplied");
            this.LabelDateTimeFormatApplied.Name = "LabelDateTimeFormatApplied";
            this.LabelDateTimeFormatApplied.VisibleChanged += new System.EventHandler(this.LabelDateTimeFormatApplied_VisibleChanged);
            // 
            // Label62
            // 
            resources.ApplyResources(this.Label62, "Label62");
            this.Label62.Name = "Label62";
            // 
            // CmbDateTimeFormat
            // 
            resources.ApplyResources(this.CmbDateTimeFormat, "CmbDateTimeFormat");
            this.CmbDateTimeFormat.Items.AddRange(new object[] {
            resources.GetString("CmbDateTimeFormat.Items"),
            resources.GetString("CmbDateTimeFormat.Items1"),
            resources.GetString("CmbDateTimeFormat.Items2"),
            resources.GetString("CmbDateTimeFormat.Items3"),
            resources.GetString("CmbDateTimeFormat.Items4"),
            resources.GetString("CmbDateTimeFormat.Items5"),
            resources.GetString("CmbDateTimeFormat.Items6"),
            resources.GetString("CmbDateTimeFormat.Items7"),
            resources.GetString("CmbDateTimeFormat.Items8"),
            resources.GetString("CmbDateTimeFormat.Items9"),
            resources.GetString("CmbDateTimeFormat.Items10")});
            this.CmbDateTimeFormat.Name = "CmbDateTimeFormat";
            this.CmbDateTimeFormat.SelectedIndexChanged += new System.EventHandler(this.CmbDateTimeFormat_SelectedIndexChanged);
            this.CmbDateTimeFormat.TextUpdate += new System.EventHandler(this.CmbDateTimeFormat_TextUpdate);
            this.CmbDateTimeFormat.Validating += new System.ComponentModel.CancelEventHandler(this.CmbDateTimeFormat_Validating);
            // 
            // Label23
            // 
            resources.ApplyResources(this.Label23, "Label23");
            this.Label23.Name = "Label23";
            // 
            // Label11
            // 
            resources.ApplyResources(this.Label11, "Label11");
            this.Label11.Name = "Label11";
            // 
            // IconSize
            // 
            this.IconSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IconSize.FormattingEnabled = true;
            this.IconSize.Items.AddRange(new object[] {
            resources.GetString("IconSize.Items"),
            resources.GetString("IconSize.Items1"),
            resources.GetString("IconSize.Items2"),
            resources.GetString("IconSize.Items3"),
            resources.GetString("IconSize.Items4")});
            resources.ApplyResources(this.IconSize, "IconSize");
            this.IconSize.Name = "IconSize";
            // 
            // TextBox3
            // 
            resources.ApplyResources(this.TextBox3, "TextBox3");
            this.TextBox3.Name = "TextBox3";
            // 
            // CheckViewTabBottom
            // 
            resources.ApplyResources(this.CheckViewTabBottom, "CheckViewTabBottom");
            this.CheckViewTabBottom.Checked = true;
            this.CheckViewTabBottom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckViewTabBottom.Name = "CheckViewTabBottom";
            this.CheckViewTabBottom.UseVisualStyleBackColor = true;
            // 
            // CheckSortOrderLock
            // 
            resources.ApplyResources(this.CheckSortOrderLock, "CheckSortOrderLock");
            this.CheckSortOrderLock.Name = "CheckSortOrderLock";
            this.CheckSortOrderLock.UseVisualStyleBackColor = true;
            // 
            // CheckShowGrid
            // 
            resources.ApplyResources(this.CheckShowGrid, "CheckShowGrid");
            this.CheckShowGrid.Name = "CheckShowGrid";
            this.CheckShowGrid.UseVisualStyleBackColor = true;
            // 
            // chkUnreadStyle
            // 
            resources.ApplyResources(this.chkUnreadStyle, "chkUnreadStyle");
            this.chkUnreadStyle.Name = "chkUnreadStyle";
            this.chkUnreadStyle.UseVisualStyleBackColor = true;
            // 
            // OneWayLv
            // 
            resources.ApplyResources(this.OneWayLv, "OneWayLv");
            this.OneWayLv.Name = "OneWayLv";
            this.OneWayLv.UseVisualStyleBackColor = true;
            // 
            // TweetPrvPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.IsListsIncludeRtsCheckBox);
            this.Controls.Add(this.HideDuplicatedRetweetsCheck);
            this.Controls.Add(this.LabelDateTimeFormatApplied);
            this.Controls.Add(this.Label62);
            this.Controls.Add(this.CmbDateTimeFormat);
            this.Controls.Add(this.Label23);
            this.Controls.Add(this.Label11);
            this.Controls.Add(this.IconSize);
            this.Controls.Add(this.TextBox3);
            this.Controls.Add(this.CheckViewTabBottom);
            this.Controls.Add(this.CheckSortOrderLock);
            this.Controls.Add(this.CheckShowGrid);
            this.Controls.Add(this.chkUnreadStyle);
            this.Controls.Add(this.OneWayLv);
            this.Name = "TweetPrvPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox IsListsIncludeRtsCheckBox;
        internal System.Windows.Forms.CheckBox HideDuplicatedRetweetsCheck;
        internal System.Windows.Forms.Label LabelDateTimeFormatApplied;
        internal System.Windows.Forms.Label Label62;
        internal System.Windows.Forms.ComboBox CmbDateTimeFormat;
        internal System.Windows.Forms.Label Label23;
        internal System.Windows.Forms.Label Label11;
        internal System.Windows.Forms.ComboBox IconSize;
        internal System.Windows.Forms.TextBox TextBox3;
        internal System.Windows.Forms.CheckBox CheckViewTabBottom;
        internal System.Windows.Forms.CheckBox CheckSortOrderLock;
        internal System.Windows.Forms.CheckBox CheckShowGrid;
        internal System.Windows.Forms.CheckBox chkUnreadStyle;
        internal System.Windows.Forms.CheckBox OneWayLv;
    }
}
