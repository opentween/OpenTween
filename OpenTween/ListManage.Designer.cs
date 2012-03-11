namespace OpenTween
{
    partial class ListManage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListManage));
            this.UserTweetDateTime = new System.Windows.Forms.Label();
            this.UserIcon = new System.Windows.Forms.PictureBox();
            this.ListsList = new System.Windows.Forms.ListBox();
            this.UserTweet = new System.Windows.Forms.Label();
            this.DeleteUserButton = new System.Windows.Forms.Button();
            this.UserProfile = new System.Windows.Forms.Label();
            this.PrivateRadioButton = new System.Windows.Forms.RadioButton();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.PublicRadioButton = new System.Windows.Forms.RadioButton();
            this.EditCheckBox = new System.Windows.Forms.CheckBox();
            this.Label20 = new System.Windows.Forms.Label();
            this.SubscriberCountTextBox = new System.Windows.Forms.TextBox();
            this.MemberCountTextBox = new System.Windows.Forms.TextBox();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.Label17 = new System.Windows.Forms.Label();
            this.RefreshListsButton = new System.Windows.Forms.Button();
            this.UserPostsNum = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.Label13 = new System.Windows.Forms.Label();
            this.RefreshUsersButton = new System.Windows.Forms.Button();
            this.UserFollowerNum = new System.Windows.Forms.Label();
            this.ListGroup = new System.Windows.Forms.GroupBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.CancelEditButton = new System.Windows.Forms.Button();
            this.OKEditButton = new System.Windows.Forms.Button();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.DescriptionText = new System.Windows.Forms.TextBox();
            this.Label12 = new System.Windows.Forms.Label();
            this.UserFollowNum = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.MemberGroup = new System.Windows.Forms.GroupBox();
            this.UserList = new System.Windows.Forms.ListBox();
            this.GetMoreUsersButton = new System.Windows.Forms.Button();
            this.AddListButton = new System.Windows.Forms.Button();
            this.DeleteListButton = new System.Windows.Forms.Button();
            this.UserWeb = new System.Windows.Forms.LinkLabel();
            this.UserLocation = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.UserGroup = new System.Windows.Forms.GroupBox();
            this.Label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.UserIcon)).BeginInit();
            this.GroupBox2.SuspendLayout();
            this.ListGroup.SuspendLayout();
            this.MemberGroup.SuspendLayout();
            this.UserGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // UserTweetDateTime
            // 
            resources.ApplyResources(this.UserTweetDateTime, "UserTweetDateTime");
            this.UserTweetDateTime.Name = "UserTweetDateTime";
            // 
            // UserIcon
            // 
            this.UserIcon.BackColor = System.Drawing.Color.White;
            this.UserIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.UserIcon, "UserIcon");
            this.UserIcon.Name = "UserIcon";
            this.UserIcon.TabStop = false;
            // 
            // ListsList
            // 
            this.ListsList.DisplayMember = "Name";
            this.ListsList.FormattingEnabled = true;
            resources.ApplyResources(this.ListsList, "ListsList");
            this.ListsList.Name = "ListsList";
            this.ListsList.SelectedIndexChanged += new System.EventHandler(this.ListsList_SelectedIndexChanged);
            // 
            // UserTweet
            // 
            this.UserTweet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.UserTweet, "UserTweet");
            this.UserTweet.Name = "UserTweet";
            // 
            // DeleteUserButton
            // 
            resources.ApplyResources(this.DeleteUserButton, "DeleteUserButton");
            this.DeleteUserButton.Name = "DeleteUserButton";
            this.DeleteUserButton.UseVisualStyleBackColor = true;
            this.DeleteUserButton.Click += new System.EventHandler(this.DeleteUserButton_Click);
            // 
            // UserProfile
            // 
            this.UserProfile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.UserProfile, "UserProfile");
            this.UserProfile.Name = "UserProfile";
            // 
            // PrivateRadioButton
            // 
            resources.ApplyResources(this.PrivateRadioButton, "PrivateRadioButton");
            this.PrivateRadioButton.Name = "PrivateRadioButton";
            this.PrivateRadioButton.TabStop = true;
            this.PrivateRadioButton.UseVisualStyleBackColor = true;
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.PrivateRadioButton);
            this.GroupBox2.Controls.Add(this.PublicRadioButton);
            resources.ApplyResources(this.GroupBox2, "GroupBox2");
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.TabStop = false;
            // 
            // PublicRadioButton
            // 
            resources.ApplyResources(this.PublicRadioButton, "PublicRadioButton");
            this.PublicRadioButton.Name = "PublicRadioButton";
            this.PublicRadioButton.TabStop = true;
            this.PublicRadioButton.UseVisualStyleBackColor = true;
            // 
            // EditCheckBox
            // 
            resources.ApplyResources(this.EditCheckBox, "EditCheckBox");
            this.EditCheckBox.Name = "EditCheckBox";
            this.EditCheckBox.UseVisualStyleBackColor = true;
            this.EditCheckBox.CheckedChanged += new System.EventHandler(this.EditCheckBox_CheckedChanged);
            // 
            // Label20
            // 
            resources.ApplyResources(this.Label20, "Label20");
            this.Label20.Name = "Label20";
            // 
            // SubscriberCountTextBox
            // 
            resources.ApplyResources(this.SubscriberCountTextBox, "SubscriberCountTextBox");
            this.SubscriberCountTextBox.Name = "SubscriberCountTextBox";
            this.SubscriberCountTextBox.ReadOnly = true;
            // 
            // MemberCountTextBox
            // 
            resources.ApplyResources(this.MemberCountTextBox, "MemberCountTextBox");
            this.MemberCountTextBox.Name = "MemberCountTextBox";
            this.MemberCountTextBox.ReadOnly = true;
            // 
            // UsernameTextBox
            // 
            resources.ApplyResources(this.UsernameTextBox, "UsernameTextBox");
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.ReadOnly = true;
            // 
            // Label17
            // 
            resources.ApplyResources(this.Label17, "Label17");
            this.Label17.Name = "Label17";
            // 
            // RefreshListsButton
            // 
            resources.ApplyResources(this.RefreshListsButton, "RefreshListsButton");
            this.RefreshListsButton.Name = "RefreshListsButton";
            this.RefreshListsButton.UseVisualStyleBackColor = true;
            this.RefreshListsButton.Click += new System.EventHandler(this.RefreshListsButton_Click);
            // 
            // UserPostsNum
            // 
            this.UserPostsNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.UserPostsNum, "UserPostsNum");
            this.UserPostsNum.Name = "UserPostsNum";
            // 
            // Label3
            // 
            resources.ApplyResources(this.Label3, "Label3");
            this.Label3.Name = "Label3";
            // 
            // Label15
            // 
            resources.ApplyResources(this.Label15, "Label15");
            this.Label15.Name = "Label15";
            // 
            // CloseButton
            // 
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // Label13
            // 
            resources.ApplyResources(this.Label13, "Label13");
            this.Label13.Name = "Label13";
            // 
            // RefreshUsersButton
            // 
            resources.ApplyResources(this.RefreshUsersButton, "RefreshUsersButton");
            this.RefreshUsersButton.Name = "RefreshUsersButton";
            this.RefreshUsersButton.UseVisualStyleBackColor = true;
            this.RefreshUsersButton.Click += new System.EventHandler(this.RefreshUsersButton_Click);
            // 
            // UserFollowerNum
            // 
            this.UserFollowerNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.UserFollowerNum, "UserFollowerNum");
            this.UserFollowerNum.Name = "UserFollowerNum";
            // 
            // ListGroup
            // 
            this.ListGroup.Controls.Add(this.Label1);
            this.ListGroup.Controls.Add(this.CancelEditButton);
            this.ListGroup.Controls.Add(this.GroupBox2);
            this.ListGroup.Controls.Add(this.OKEditButton);
            this.ListGroup.Controls.Add(this.SubscriberCountTextBox);
            this.ListGroup.Controls.Add(this.MemberCountTextBox);
            this.ListGroup.Controls.Add(this.UsernameTextBox);
            this.ListGroup.Controls.Add(this.NameTextBox);
            this.ListGroup.Controls.Add(this.Label4);
            this.ListGroup.Controls.Add(this.Label6);
            this.ListGroup.Controls.Add(this.Label10);
            this.ListGroup.Controls.Add(this.DescriptionText);
            this.ListGroup.Controls.Add(this.Label12);
            resources.ApplyResources(this.ListGroup, "ListGroup");
            this.ListGroup.Name = "ListGroup";
            this.ListGroup.TabStop = false;
            // 
            // Label1
            // 
            resources.ApplyResources(this.Label1, "Label1");
            this.Label1.Name = "Label1";
            // 
            // CancelEditButton
            // 
            resources.ApplyResources(this.CancelEditButton, "CancelEditButton");
            this.CancelEditButton.Name = "CancelEditButton";
            this.CancelEditButton.UseVisualStyleBackColor = true;
            this.CancelEditButton.Click += new System.EventHandler(this.CancelEditButton_Click);
            // 
            // OKEditButton
            // 
            resources.ApplyResources(this.OKEditButton, "OKEditButton");
            this.OKEditButton.Name = "OKEditButton";
            this.OKEditButton.UseVisualStyleBackColor = true;
            this.OKEditButton.Click += new System.EventHandler(this.OKEditButton_Click);
            // 
            // NameTextBox
            // 
            resources.ApplyResources(this.NameTextBox, "NameTextBox");
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.ReadOnly = true;
            // 
            // Label4
            // 
            resources.ApplyResources(this.Label4, "Label4");
            this.Label4.Name = "Label4";
            // 
            // Label6
            // 
            resources.ApplyResources(this.Label6, "Label6");
            this.Label6.Name = "Label6";
            // 
            // Label10
            // 
            resources.ApplyResources(this.Label10, "Label10");
            this.Label10.Name = "Label10";
            // 
            // DescriptionText
            // 
            resources.ApplyResources(this.DescriptionText, "DescriptionText");
            this.DescriptionText.Name = "DescriptionText";
            this.DescriptionText.ReadOnly = true;
            // 
            // Label12
            // 
            resources.ApplyResources(this.Label12, "Label12");
            this.Label12.Name = "Label12";
            // 
            // UserFollowNum
            // 
            this.UserFollowNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.UserFollowNum, "UserFollowNum");
            this.UserFollowNum.Name = "UserFollowNum";
            // 
            // Label9
            // 
            resources.ApplyResources(this.Label9, "Label9");
            this.Label9.Name = "Label9";
            // 
            // MemberGroup
            // 
            this.MemberGroup.Controls.Add(this.UserList);
            this.MemberGroup.Controls.Add(this.GetMoreUsersButton);
            this.MemberGroup.Controls.Add(this.RefreshUsersButton);
            resources.ApplyResources(this.MemberGroup, "MemberGroup");
            this.MemberGroup.Name = "MemberGroup";
            this.MemberGroup.TabStop = false;
            // 
            // UserList
            // 
            this.UserList.FormattingEnabled = true;
            resources.ApplyResources(this.UserList, "UserList");
            this.UserList.Name = "UserList";
            this.UserList.SelectedIndexChanged += new System.EventHandler(this.UserList_SelectedIndexChanged);
            // 
            // GetMoreUsersButton
            // 
            resources.ApplyResources(this.GetMoreUsersButton, "GetMoreUsersButton");
            this.GetMoreUsersButton.Name = "GetMoreUsersButton";
            this.GetMoreUsersButton.UseVisualStyleBackColor = true;
            this.GetMoreUsersButton.Click += new System.EventHandler(this.GetMoreUsersButton_Click);
            // 
            // AddListButton
            // 
            resources.ApplyResources(this.AddListButton, "AddListButton");
            this.AddListButton.Name = "AddListButton";
            this.AddListButton.UseVisualStyleBackColor = true;
            this.AddListButton.Click += new System.EventHandler(this.AddListButton_Click);
            // 
            // DeleteListButton
            // 
            resources.ApplyResources(this.DeleteListButton, "DeleteListButton");
            this.DeleteListButton.Name = "DeleteListButton";
            this.DeleteListButton.UseVisualStyleBackColor = true;
            this.DeleteListButton.Click += new System.EventHandler(this.DeleteListButton_Click);
            // 
            // UserWeb
            // 
            this.UserWeb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.UserWeb, "UserWeb");
            this.UserWeb.Name = "UserWeb";
            this.UserWeb.TabStop = true;
            this.UserWeb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.UserWeb_LinkClicked);
            // 
            // UserLocation
            // 
            this.UserLocation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.UserLocation, "UserLocation");
            this.UserLocation.Name = "UserLocation";
            // 
            // Label8
            // 
            resources.ApplyResources(this.Label8, "Label8");
            this.Label8.Name = "Label8";
            // 
            // UserGroup
            // 
            this.UserGroup.Controls.Add(this.UserTweetDateTime);
            this.UserGroup.Controls.Add(this.UserIcon);
            this.UserGroup.Controls.Add(this.UserTweet);
            this.UserGroup.Controls.Add(this.Label20);
            this.UserGroup.Controls.Add(this.DeleteUserButton);
            this.UserGroup.Controls.Add(this.UserProfile);
            this.UserGroup.Controls.Add(this.Label17);
            this.UserGroup.Controls.Add(this.UserPostsNum);
            this.UserGroup.Controls.Add(this.Label15);
            this.UserGroup.Controls.Add(this.UserFollowerNum);
            this.UserGroup.Controls.Add(this.Label13);
            this.UserGroup.Controls.Add(this.UserFollowNum);
            this.UserGroup.Controls.Add(this.Label9);
            this.UserGroup.Controls.Add(this.UserWeb);
            this.UserGroup.Controls.Add(this.Label8);
            this.UserGroup.Controls.Add(this.UserLocation);
            this.UserGroup.Controls.Add(this.Label5);
            resources.ApplyResources(this.UserGroup, "UserGroup");
            this.UserGroup.Name = "UserGroup";
            this.UserGroup.TabStop = false;
            // 
            // Label5
            // 
            resources.ApplyResources(this.Label5, "Label5");
            this.Label5.Name = "Label5";
            // 
            // ListManage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.ListsList);
            this.Controls.Add(this.EditCheckBox);
            this.Controls.Add(this.RefreshListsButton);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.ListGroup);
            this.Controls.Add(this.MemberGroup);
            this.Controls.Add(this.AddListButton);
            this.Controls.Add(this.DeleteListButton);
            this.Controls.Add(this.UserGroup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ListManage";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ListManage_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListManage_KeyDown);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.ListManage_Validating);
            ((System.ComponentModel.ISupportInitialize)(this.UserIcon)).EndInit();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.ListGroup.ResumeLayout(false);
            this.ListGroup.PerformLayout();
            this.MemberGroup.ResumeLayout(false);
            this.UserGroup.ResumeLayout(false);
            this.UserGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label UserTweetDateTime;
        internal System.Windows.Forms.PictureBox UserIcon;
        internal System.Windows.Forms.ListBox ListsList;
        internal System.Windows.Forms.Label UserTweet;
        internal System.Windows.Forms.Button DeleteUserButton;
        internal System.Windows.Forms.Label UserProfile;
        internal System.Windows.Forms.RadioButton PrivateRadioButton;
        internal System.Windows.Forms.GroupBox GroupBox2;
        internal System.Windows.Forms.RadioButton PublicRadioButton;
        internal System.Windows.Forms.CheckBox EditCheckBox;
        internal System.Windows.Forms.Label Label20;
        internal System.Windows.Forms.TextBox SubscriberCountTextBox;
        internal System.Windows.Forms.TextBox MemberCountTextBox;
        internal System.Windows.Forms.TextBox UsernameTextBox;
        internal System.Windows.Forms.Label Label17;
        internal System.Windows.Forms.Button RefreshListsButton;
        internal System.Windows.Forms.Label UserPostsNum;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Label15;
        internal System.Windows.Forms.Button CloseButton;
        internal System.Windows.Forms.Label Label13;
        internal System.Windows.Forms.Button RefreshUsersButton;
        internal System.Windows.Forms.Label UserFollowerNum;
        internal System.Windows.Forms.GroupBox ListGroup;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Button CancelEditButton;
        internal System.Windows.Forms.Button OKEditButton;
        internal System.Windows.Forms.TextBox NameTextBox;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.Label Label10;
        internal System.Windows.Forms.TextBox DescriptionText;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.Label UserFollowNum;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.GroupBox MemberGroup;
        internal System.Windows.Forms.ListBox UserList;
        internal System.Windows.Forms.Button GetMoreUsersButton;
        internal System.Windows.Forms.Button AddListButton;
        internal System.Windows.Forms.Button DeleteListButton;
        internal System.Windows.Forms.LinkLabel UserWeb;
        internal System.Windows.Forms.Label UserLocation;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.GroupBox UserGroup;
        internal System.Windows.Forms.Label Label5;
    }
}