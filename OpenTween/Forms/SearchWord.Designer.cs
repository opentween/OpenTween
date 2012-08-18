namespace OpenTween
{
    partial class SearchWord
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchWord));
            this.CheckSearchRegex = new System.Windows.Forms.CheckBox();
            this.CheckSearchCaseSensitive = new System.Windows.Forms.CheckBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.SWordText = new System.Windows.Forms.TextBox();
            this.OK_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CheckSearchRegex
            // 
            resources.ApplyResources(this.CheckSearchRegex, "CheckSearchRegex");
            this.CheckSearchRegex.Name = "CheckSearchRegex";
            this.CheckSearchRegex.UseVisualStyleBackColor = true;
            // 
            // CheckSearchCaseSensitive
            // 
            resources.ApplyResources(this.CheckSearchCaseSensitive, "CheckSearchCaseSensitive");
            this.CheckSearchCaseSensitive.Name = "CheckSearchCaseSensitive";
            this.CheckSearchCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // Label1
            // 
            resources.ApplyResources(this.Label1, "Label1");
            this.Label1.Name = "Label1";
            // 
            // SWordText
            // 
            resources.ApplyResources(this.SWordText, "SWordText");
            this.SWordText.Name = "SWordText";
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
            // SearchWord
            // 
            this.AcceptButton = this.OK_Button;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.Controls.Add(this.CheckSearchRegex);
            this.Controls.Add(this.CheckSearchCaseSensitive);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.SWordText);
            this.Controls.Add(this.TableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchWord";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.SearchWord_Shown);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox CheckSearchRegex;
        internal System.Windows.Forms.CheckBox CheckSearchCaseSensitive;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox SWordText;
        internal System.Windows.Forms.Button OK_Button;
        internal System.Windows.Forms.Button Cancel_Button;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;


    }
}