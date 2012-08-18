namespace OpenTween
{
    partial class HashtagManage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HashtagManage));
            this.HistoryHashList = new System.Windows.Forms.ListBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.UnSelectButton = new System.Windows.Forms.Button();
            this.RadioLast = new System.Windows.Forms.RadioButton();
            this.RadioHead = new System.Windows.Forms.RadioButton();
            this.TableLayoutButtons = new System.Windows.Forms.TableLayoutPanel();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.OK_Button = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.PermCancel_Button = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.GroupHashtag = new System.Windows.Forms.GroupBox();
            this.CheckPermanent = new System.Windows.Forms.CheckBox();
            this.PermOK_Button = new System.Windows.Forms.Button();
            this.TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.CheckNotAddToAtReply = new System.Windows.Forms.CheckBox();
            this.GroupDetail = new System.Windows.Forms.GroupBox();
            this.UseHashText = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.TableLayoutButtons.SuspendLayout();
            this.GroupHashtag.SuspendLayout();
            this.TableLayoutPanel2.SuspendLayout();
            this.GroupDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // HistoryHashList
            // 
            this.HistoryHashList.FormattingEnabled = true;
            resources.ApplyResources(this.HistoryHashList, "HistoryHashList");
            this.HistoryHashList.Name = "HistoryHashList";
            this.HistoryHashList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.HistoryHashList.DoubleClick += new System.EventHandler(this.HistoryHashList_DoubleClick);
            this.HistoryHashList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HistoryHashList_KeyDown);
            // 
            // Label3
            // 
            resources.ApplyResources(this.Label3, "Label3");
            this.Label3.Name = "Label3";
            // 
            // UnSelectButton
            // 
            resources.ApplyResources(this.UnSelectButton, "UnSelectButton");
            this.UnSelectButton.Name = "UnSelectButton";
            this.UnSelectButton.UseVisualStyleBackColor = true;
            this.UnSelectButton.Click += new System.EventHandler(this.UnSelectButton_Click);
            // 
            // RadioLast
            // 
            resources.ApplyResources(this.RadioLast, "RadioLast");
            this.RadioLast.Name = "RadioLast";
            this.RadioLast.TabStop = true;
            this.RadioLast.UseVisualStyleBackColor = true;
            // 
            // RadioHead
            // 
            resources.ApplyResources(this.RadioHead, "RadioHead");
            this.RadioHead.Name = "RadioHead";
            this.RadioHead.TabStop = true;
            this.RadioHead.UseVisualStyleBackColor = true;
            // 
            // TableLayoutButtons
            // 
            resources.ApplyResources(this.TableLayoutButtons, "TableLayoutButtons");
            this.TableLayoutButtons.Controls.Add(this.Cancel_Button, 1, 0);
            this.TableLayoutButtons.Controls.Add(this.OK_Button, 0, 0);
            this.TableLayoutButtons.Name = "TableLayoutButtons";
            // 
            // Cancel_Button
            // 
            resources.ApplyResources(this.Cancel_Button, "Cancel_Button");
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // OK_Button
            // 
            resources.ApplyResources(this.OK_Button, "OK_Button");
            this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // DeleteButton
            // 
            resources.ApplyResources(this.DeleteButton, "DeleteButton");
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // PermCancel_Button
            // 
            resources.ApplyResources(this.PermCancel_Button, "PermCancel_Button");
            this.PermCancel_Button.Name = "PermCancel_Button";
            this.PermCancel_Button.Click += new System.EventHandler(this.PermCancel_Button_Click);
            // 
            // EditButton
            // 
            resources.ApplyResources(this.EditButton, "EditButton");
            this.EditButton.Name = "EditButton";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // AddButton
            // 
            resources.ApplyResources(this.AddButton, "AddButton");
            this.AddButton.Name = "AddButton";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // GroupHashtag
            // 
            this.GroupHashtag.Controls.Add(this.HistoryHashList);
            this.GroupHashtag.Controls.Add(this.Label3);
            this.GroupHashtag.Controls.Add(this.UnSelectButton);
            this.GroupHashtag.Controls.Add(this.RadioLast);
            this.GroupHashtag.Controls.Add(this.DeleteButton);
            this.GroupHashtag.Controls.Add(this.RadioHead);
            this.GroupHashtag.Controls.Add(this.EditButton);
            this.GroupHashtag.Controls.Add(this.AddButton);
            this.GroupHashtag.Controls.Add(this.CheckPermanent);
            resources.ApplyResources(this.GroupHashtag, "GroupHashtag");
            this.GroupHashtag.Name = "GroupHashtag";
            this.GroupHashtag.TabStop = false;
            // 
            // CheckPermanent
            // 
            resources.ApplyResources(this.CheckPermanent, "CheckPermanent");
            this.CheckPermanent.Name = "CheckPermanent";
            this.CheckPermanent.UseVisualStyleBackColor = true;
            // 
            // PermOK_Button
            // 
            resources.ApplyResources(this.PermOK_Button, "PermOK_Button");
            this.PermOK_Button.Name = "PermOK_Button";
            this.PermOK_Button.Click += new System.EventHandler(this.PermOK_Button_Click);
            // 
            // TableLayoutPanel2
            // 
            resources.ApplyResources(this.TableLayoutPanel2, "TableLayoutPanel2");
            this.TableLayoutPanel2.Controls.Add(this.PermOK_Button, 0, 0);
            this.TableLayoutPanel2.Controls.Add(this.PermCancel_Button, 1, 0);
            this.TableLayoutPanel2.Name = "TableLayoutPanel2";
            // 
            // CheckNotAddToAtReply
            // 
            resources.ApplyResources(this.CheckNotAddToAtReply, "CheckNotAddToAtReply");
            this.CheckNotAddToAtReply.Checked = true;
            this.CheckNotAddToAtReply.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckNotAddToAtReply.Name = "CheckNotAddToAtReply";
            this.CheckNotAddToAtReply.UseVisualStyleBackColor = true;
            this.CheckNotAddToAtReply.CheckedChanged += new System.EventHandler(this.CheckNotAddToAtReply_CheckedChanged);
            // 
            // GroupDetail
            // 
            this.GroupDetail.Controls.Add(this.TableLayoutPanel2);
            this.GroupDetail.Controls.Add(this.UseHashText);
            this.GroupDetail.Controls.Add(this.Label1);
            resources.ApplyResources(this.GroupDetail, "GroupDetail");
            this.GroupDetail.Name = "GroupDetail";
            this.GroupDetail.TabStop = false;
            // 
            // UseHashText
            // 
            resources.ApplyResources(this.UseHashText, "UseHashText");
            this.UseHashText.Name = "UseHashText";
            this.UseHashText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UseHashText_KeyPress);
            // 
            // Label1
            // 
            resources.ApplyResources(this.Label1, "Label1");
            this.Label1.Name = "Label1";
            // 
            // HashtagManage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TableLayoutButtons);
            this.Controls.Add(this.GroupHashtag);
            this.Controls.Add(this.CheckNotAddToAtReply);
            this.Controls.Add(this.GroupDetail);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HashtagManage";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.HashtagManage_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HashtagManage_KeyDown);
            this.TableLayoutButtons.ResumeLayout(false);
            this.GroupHashtag.ResumeLayout(false);
            this.GroupHashtag.PerformLayout();
            this.TableLayoutPanel2.ResumeLayout(false);
            this.GroupDetail.ResumeLayout(false);
            this.GroupDetail.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ListBox HistoryHashList;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Button UnSelectButton;
        internal System.Windows.Forms.RadioButton RadioLast;
        internal System.Windows.Forms.RadioButton RadioHead;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutButtons;
        internal System.Windows.Forms.Button Cancel_Button;
        internal System.Windows.Forms.Button OK_Button;
        internal System.Windows.Forms.Button DeleteButton;
        internal System.Windows.Forms.Button PermCancel_Button;
        internal System.Windows.Forms.Button EditButton;
        internal System.Windows.Forms.Button AddButton;
        internal System.Windows.Forms.GroupBox GroupHashtag;
        internal System.Windows.Forms.CheckBox CheckPermanent;
        internal System.Windows.Forms.Button PermOK_Button;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel2;
        internal System.Windows.Forms.CheckBox CheckNotAddToAtReply;
        internal System.Windows.Forms.GroupBox GroupDetail;
        internal System.Windows.Forms.TextBox UseHashText;
        internal System.Windows.Forms.Label Label1;
    }
}