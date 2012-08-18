namespace OpenTween
{
    partial class ShowUserInfo
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowUserInfo));
            this.BackgroundWorkerImageLoader = new System.ComponentModel.BackgroundWorker();
            this.LinkLabel2 = new System.Windows.Forms.LinkLabel();
            this.ButtonBlockDestroy = new System.Windows.Forms.Button();
            this.ButtonReportSpam = new System.Windows.Forms.Button();
            this.SelectionCopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonBlock = new System.Windows.Forms.Button();
            this.TextBoxDescription = new System.Windows.Forms.TextBox();
            this.TextBoxWeb = new System.Windows.Forms.TextBox();
            this.ButtonEdit = new System.Windows.Forms.Button();
            this.LabelId = new System.Windows.Forms.Label();
            this.ContextMenuRecentPostBrowser = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TextBoxLocation = new System.Windows.Forms.TextBox();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TextBoxName = new System.Windows.Forms.TextBox();
            this.Label12 = new System.Windows.Forms.Label();
            this.ButtonSearchPosts = new System.Windows.Forms.Button();
            this.LinkLabel1 = new System.Windows.Forms.LinkLabel();
            this.RecentPostBrowser = new System.Windows.Forms.WebBrowser();
            this.ChangeIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LabelIsVerified = new System.Windows.Forms.Label();
            this.DescriptionBrowser = new System.Windows.Forms.WebBrowser();
            this.LabelScreenName = new System.Windows.Forms.Label();
            this.LabelRecentPost = new System.Windows.Forms.Label();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.ContextMenuUserPicture = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.UserPicture = new System.Windows.Forms.PictureBox();
            this.LinkLabelFav = new System.Windows.Forms.LinkLabel();
            this.Label9 = new System.Windows.Forms.Label();
            this.LabelIsProtected = new System.Windows.Forms.Label();
            this.LabelCreatedAt = new System.Windows.Forms.Label();
            this.LinkLabelTweet = new System.Windows.Forms.LinkLabel();
            this.LabelIsFollowed = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.LabelIsFollowing = new System.Windows.Forms.Label();
            this.LinkLabelFollowers = new System.Windows.Forms.LinkLabel();
            this.ButtonUnFollow = new System.Windows.Forms.Button();
            this.LinkLabelFollowing = new System.Windows.Forms.LinkLabel();
            this.Label6 = new System.Windows.Forms.Label();
            this.LabelName = new System.Windows.Forms.Label();
            this.ButtonFollow = new System.Windows.Forms.Button();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.LabelLocation = new System.Windows.Forms.Label();
            this.OpenFileDialogIcon = new System.Windows.Forms.OpenFileDialog();
            this.LinkLabelWeb = new System.Windows.Forms.LinkLabel();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.ContextMenuRecentPostBrowser.SuspendLayout();
            this.ContextMenuUserPicture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // BackgroundWorkerImageLoader
            // 
            this.BackgroundWorkerImageLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerImageLoader_DoWork);
            this.BackgroundWorkerImageLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerImageLoader_RunWorkerCompleted);
            // 
            // LinkLabel2
            // 
            resources.ApplyResources(this.LinkLabel2, "LinkLabel2");
            this.LinkLabel2.Name = "LinkLabel2";
            this.LinkLabel2.TabStop = true;
            this.LinkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel2_LinkClicked);
            // 
            // ButtonBlockDestroy
            // 
            resources.ApplyResources(this.ButtonBlockDestroy, "ButtonBlockDestroy");
            this.ButtonBlockDestroy.Name = "ButtonBlockDestroy";
            this.ButtonBlockDestroy.UseVisualStyleBackColor = true;
            this.ButtonBlockDestroy.Click += new System.EventHandler(this.ButtonBlockDestroy_Click);
            // 
            // ButtonReportSpam
            // 
            resources.ApplyResources(this.ButtonReportSpam, "ButtonReportSpam");
            this.ButtonReportSpam.Name = "ButtonReportSpam";
            this.ButtonReportSpam.UseVisualStyleBackColor = true;
            this.ButtonReportSpam.Click += new System.EventHandler(this.ButtonReportSpam_Click);
            // 
            // SelectionCopyToolStripMenuItem
            // 
            this.SelectionCopyToolStripMenuItem.Name = "SelectionCopyToolStripMenuItem";
            resources.ApplyResources(this.SelectionCopyToolStripMenuItem, "SelectionCopyToolStripMenuItem");
            this.SelectionCopyToolStripMenuItem.Click += new System.EventHandler(this.SelectionCopyToolStripMenuItem_Click);
            // 
            // ButtonBlock
            // 
            resources.ApplyResources(this.ButtonBlock, "ButtonBlock");
            this.ButtonBlock.Name = "ButtonBlock";
            this.ButtonBlock.UseVisualStyleBackColor = true;
            this.ButtonBlock.Click += new System.EventHandler(this.ButtonBlock_Click);
            // 
            // TextBoxDescription
            // 
            resources.ApplyResources(this.TextBoxDescription, "TextBoxDescription");
            this.TextBoxDescription.Name = "TextBoxDescription";
            this.TextBoxDescription.TabStop = false;
            // 
            // TextBoxWeb
            // 
            resources.ApplyResources(this.TextBoxWeb, "TextBoxWeb");
            this.TextBoxWeb.Name = "TextBoxWeb";
            this.TextBoxWeb.TabStop = false;
            // 
            // ButtonEdit
            // 
            resources.ApplyResources(this.ButtonEdit, "ButtonEdit");
            this.ButtonEdit.Name = "ButtonEdit";
            this.ButtonEdit.UseVisualStyleBackColor = true;
            this.ButtonEdit.Click += new System.EventHandler(this.ButtonEdit_Click);
            // 
            // LabelId
            // 
            resources.ApplyResources(this.LabelId, "LabelId");
            this.LabelId.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelId.Name = "LabelId";
            // 
            // ContextMenuRecentPostBrowser
            // 
            this.ContextMenuRecentPostBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SelectionCopyToolStripMenuItem,
            this.SelectAllToolStripMenuItem});
            this.ContextMenuRecentPostBrowser.Name = "ContextMenuStrip1";
            resources.ApplyResources(this.ContextMenuRecentPostBrowser, "ContextMenuRecentPostBrowser");
            this.ContextMenuRecentPostBrowser.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1_Opening);
            // 
            // SelectAllToolStripMenuItem
            // 
            this.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem";
            resources.ApplyResources(this.SelectAllToolStripMenuItem, "SelectAllToolStripMenuItem");
            this.SelectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItem_Click);
            // 
            // TextBoxLocation
            // 
            resources.ApplyResources(this.TextBoxLocation, "TextBoxLocation");
            this.TextBoxLocation.Name = "TextBoxLocation";
            this.TextBoxLocation.TabStop = false;
            // 
            // ToolTip1
            // 
            this.ToolTip1.ShowAlways = true;
            // 
            // TextBoxName
            // 
            resources.ApplyResources(this.TextBoxName, "TextBoxName");
            this.TextBoxName.Name = "TextBoxName";
            this.TextBoxName.TabStop = false;
            // 
            // Label12
            // 
            resources.ApplyResources(this.Label12, "Label12");
            this.Label12.Name = "Label12";
            // 
            // ButtonSearchPosts
            // 
            resources.ApplyResources(this.ButtonSearchPosts, "ButtonSearchPosts");
            this.ButtonSearchPosts.Name = "ButtonSearchPosts";
            this.ButtonSearchPosts.UseVisualStyleBackColor = true;
            this.ButtonSearchPosts.Click += new System.EventHandler(this.ButtonSearchPosts_Click);
            // 
            // LinkLabel1
            // 
            resources.ApplyResources(this.LinkLabel1, "LinkLabel1");
            this.LinkLabel1.Name = "LinkLabel1";
            this.LinkLabel1.TabStop = true;
            this.LinkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel1_LinkClicked);
            // 
            // RecentPostBrowser
            // 
            this.RecentPostBrowser.AllowWebBrowserDrop = false;
            this.RecentPostBrowser.ContextMenuStrip = this.ContextMenuRecentPostBrowser;
            this.RecentPostBrowser.IsWebBrowserContextMenuEnabled = false;
            resources.ApplyResources(this.RecentPostBrowser, "RecentPostBrowser");
            this.RecentPostBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.RecentPostBrowser.Name = "RecentPostBrowser";
            this.RecentPostBrowser.TabStop = false;
            this.RecentPostBrowser.Url = new System.Uri("about:blank", System.UriKind.Absolute);
            this.RecentPostBrowser.WebBrowserShortcutsEnabled = false;
            this.RecentPostBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowser_Navigating);
            this.RecentPostBrowser.StatusTextChanged += new System.EventHandler(this.WebBrowser_StatusTextChanged);
            // 
            // ChangeIconToolStripMenuItem
            // 
            this.ChangeIconToolStripMenuItem.Name = "ChangeIconToolStripMenuItem";
            resources.ApplyResources(this.ChangeIconToolStripMenuItem, "ChangeIconToolStripMenuItem");
            this.ChangeIconToolStripMenuItem.Click += new System.EventHandler(this.ChangeIconToolStripMenuItem_Click);
            // 
            // LabelIsVerified
            // 
            resources.ApplyResources(this.LabelIsVerified, "LabelIsVerified");
            this.LabelIsVerified.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelIsVerified.Name = "LabelIsVerified";
            // 
            // DescriptionBrowser
            // 
            this.DescriptionBrowser.AllowWebBrowserDrop = false;
            this.DescriptionBrowser.ContextMenuStrip = this.ContextMenuRecentPostBrowser;
            this.DescriptionBrowser.IsWebBrowserContextMenuEnabled = false;
            resources.ApplyResources(this.DescriptionBrowser, "DescriptionBrowser");
            this.DescriptionBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.DescriptionBrowser.Name = "DescriptionBrowser";
            this.DescriptionBrowser.TabStop = false;
            this.DescriptionBrowser.Url = new System.Uri("about:blank", System.UriKind.Absolute);
            this.DescriptionBrowser.WebBrowserShortcutsEnabled = false;
            this.DescriptionBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowser_Navigating);
            this.DescriptionBrowser.StatusTextChanged += new System.EventHandler(this.WebBrowser_StatusTextChanged);
            // 
            // LabelScreenName
            // 
            this.LabelScreenName.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.LabelScreenName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.LabelScreenName, "LabelScreenName");
            this.LabelScreenName.Name = "LabelScreenName";
            // 
            // LabelRecentPost
            // 
            resources.ApplyResources(this.LabelRecentPost, "LabelRecentPost");
            this.LabelRecentPost.Name = "LabelRecentPost";
            // 
            // ButtonClose
            // 
            this.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.ButtonClose, "ButtonClose");
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // ContextMenuUserPicture
            // 
            this.ContextMenuUserPicture.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ChangeIconToolStripMenuItem});
            this.ContextMenuUserPicture.Name = "ContextMenuStrip2";
            resources.ApplyResources(this.ContextMenuUserPicture, "ContextMenuUserPicture");
            // 
            // UserPicture
            // 
            this.UserPicture.BackColor = System.Drawing.Color.White;
            this.UserPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UserPicture.ContextMenuStrip = this.ContextMenuUserPicture;
            resources.ApplyResources(this.UserPicture, "UserPicture");
            this.UserPicture.Name = "UserPicture";
            this.UserPicture.TabStop = false;
            this.UserPicture.DoubleClick += new System.EventHandler(this.UserPicture_DoubleClick);
            this.UserPicture.MouseEnter += new System.EventHandler(this.UserPicture_MouseEnter);
            this.UserPicture.MouseLeave += new System.EventHandler(this.UserPicture_MouseLeave);
            // 
            // LinkLabelFav
            // 
            resources.ApplyResources(this.LinkLabelFav, "LinkLabelFav");
            this.LinkLabelFav.Name = "LinkLabelFav";
            this.LinkLabelFav.TabStop = true;
            this.LinkLabelFav.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelFav_LinkClicked);
            // 
            // Label9
            // 
            resources.ApplyResources(this.Label9, "Label9");
            this.Label9.Name = "Label9";
            // 
            // LabelIsProtected
            // 
            resources.ApplyResources(this.LabelIsProtected, "LabelIsProtected");
            this.LabelIsProtected.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelIsProtected.Name = "LabelIsProtected";
            // 
            // LabelCreatedAt
            // 
            resources.ApplyResources(this.LabelCreatedAt, "LabelCreatedAt");
            this.LabelCreatedAt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelCreatedAt.Name = "LabelCreatedAt";
            // 
            // LinkLabelTweet
            // 
            resources.ApplyResources(this.LinkLabelTweet, "LinkLabelTweet");
            this.LinkLabelTweet.Name = "LinkLabelTweet";
            this.LinkLabelTweet.TabStop = true;
            this.LinkLabelTweet.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelTweet_LinkClicked);
            // 
            // LabelIsFollowed
            // 
            resources.ApplyResources(this.LabelIsFollowed, "LabelIsFollowed");
            this.LabelIsFollowed.Name = "LabelIsFollowed";
            // 
            // Label8
            // 
            resources.ApplyResources(this.Label8, "Label8");
            this.Label8.Name = "Label8";
            // 
            // LabelIsFollowing
            // 
            resources.ApplyResources(this.LabelIsFollowing, "LabelIsFollowing");
            this.LabelIsFollowing.Name = "LabelIsFollowing";
            // 
            // LinkLabelFollowers
            // 
            resources.ApplyResources(this.LinkLabelFollowers, "LinkLabelFollowers");
            this.LinkLabelFollowers.Name = "LinkLabelFollowers";
            this.LinkLabelFollowers.TabStop = true;
            this.LinkLabelFollowers.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelFollowers_LinkClicked);
            // 
            // ButtonUnFollow
            // 
            resources.ApplyResources(this.ButtonUnFollow, "ButtonUnFollow");
            this.ButtonUnFollow.Name = "ButtonUnFollow";
            this.ButtonUnFollow.UseVisualStyleBackColor = true;
            this.ButtonUnFollow.Click += new System.EventHandler(this.ButtonUnFollow_Click);
            // 
            // LinkLabelFollowing
            // 
            resources.ApplyResources(this.LinkLabelFollowing, "LinkLabelFollowing");
            this.LinkLabelFollowing.Name = "LinkLabelFollowing";
            this.LinkLabelFollowing.TabStop = true;
            this.LinkLabelFollowing.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelFollowing_LinkClicked);
            // 
            // Label6
            // 
            resources.ApplyResources(this.Label6, "Label6");
            this.Label6.Name = "Label6";
            // 
            // LabelName
            // 
            this.LabelName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.LabelName, "LabelName");
            this.LabelName.Name = "LabelName";
            this.LabelName.UseMnemonic = false;
            // 
            // ButtonFollow
            // 
            resources.ApplyResources(this.ButtonFollow, "ButtonFollow");
            this.ButtonFollow.Name = "ButtonFollow";
            this.ButtonFollow.UseVisualStyleBackColor = true;
            this.ButtonFollow.Click += new System.EventHandler(this.ButtonFollow_Click);
            // 
            // Label5
            // 
            resources.ApplyResources(this.Label5, "Label5");
            this.Label5.Name = "Label5";
            // 
            // Label7
            // 
            resources.ApplyResources(this.Label7, "Label7");
            this.Label7.Name = "Label7";
            // 
            // Label4
            // 
            resources.ApplyResources(this.Label4, "Label4");
            this.Label4.Name = "Label4";
            // 
            // LabelLocation
            // 
            this.LabelLocation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.LabelLocation, "LabelLocation");
            this.LabelLocation.Name = "LabelLocation";
            this.LabelLocation.UseMnemonic = false;
            // 
            // OpenFileDialogIcon
            // 
            this.OpenFileDialogIcon.FileName = "OpenFileDialog1";
            // 
            // LinkLabelWeb
            // 
            this.LinkLabelWeb.AutoEllipsis = true;
            this.LinkLabelWeb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.LinkLabelWeb, "LinkLabelWeb");
            this.LinkLabelWeb.Name = "LinkLabelWeb";
            this.LinkLabelWeb.TabStop = true;
            this.LinkLabelWeb.UseMnemonic = false;
            this.LinkLabelWeb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelWeb_LinkClicked);
            // 
            // Label1
            // 
            resources.ApplyResources(this.Label1, "Label1");
            this.Label1.Name = "Label1";
            this.Label1.UseMnemonic = false;
            // 
            // Label3
            // 
            resources.ApplyResources(this.Label3, "Label3");
            this.Label3.Name = "Label3";
            // 
            // Label2
            // 
            resources.ApplyResources(this.Label2, "Label2");
            this.Label2.Name = "Label2";
            // 
            // ShowUserInfo
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonClose;
            this.Controls.Add(this.LinkLabel2);
            this.Controls.Add(this.ButtonBlockDestroy);
            this.Controls.Add(this.ButtonReportSpam);
            this.Controls.Add(this.ButtonBlock);
            this.Controls.Add(this.TextBoxDescription);
            this.Controls.Add(this.TextBoxWeb);
            this.Controls.Add(this.ButtonEdit);
            this.Controls.Add(this.LabelId);
            this.Controls.Add(this.TextBoxLocation);
            this.Controls.Add(this.TextBoxName);
            this.Controls.Add(this.Label12);
            this.Controls.Add(this.ButtonSearchPosts);
            this.Controls.Add(this.LinkLabel1);
            this.Controls.Add(this.RecentPostBrowser);
            this.Controls.Add(this.LabelIsVerified);
            this.Controls.Add(this.DescriptionBrowser);
            this.Controls.Add(this.LabelScreenName);
            this.Controls.Add(this.LabelRecentPost);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.UserPicture);
            this.Controls.Add(this.LinkLabelFav);
            this.Controls.Add(this.Label9);
            this.Controls.Add(this.LabelIsProtected);
            this.Controls.Add(this.LabelCreatedAt);
            this.Controls.Add(this.LinkLabelTweet);
            this.Controls.Add(this.LabelIsFollowed);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.LabelIsFollowing);
            this.Controls.Add(this.LinkLabelFollowers);
            this.Controls.Add(this.ButtonUnFollow);
            this.Controls.Add(this.LinkLabelFollowing);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.LabelName);
            this.Controls.Add(this.ButtonFollow);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.LabelLocation);
            this.Controls.Add(this.LinkLabelWeb);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShowUserInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.ShowUserInfo_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShowUserInfo_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShowUserInfo_FormClosed);
            this.Load += new System.EventHandler(this.ShowUserInfo_Load);
            this.Shown += new System.EventHandler(this.ShowUserInfo_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ShowUserInfo_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.ShowUserInfo_DragOver);
            this.MouseEnter += new System.EventHandler(this.ShowUserInfo_MouseEnter);
            this.ContextMenuRecentPostBrowser.ResumeLayout(false);
            this.ContextMenuUserPicture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UserPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.ComponentModel.BackgroundWorker BackgroundWorkerImageLoader;
        internal System.Windows.Forms.LinkLabel LinkLabel2;
        internal System.Windows.Forms.Button ButtonBlockDestroy;
        internal System.Windows.Forms.Button ButtonReportSpam;
        internal System.Windows.Forms.ToolStripMenuItem SelectionCopyToolStripMenuItem;
        internal System.Windows.Forms.Button ButtonBlock;
        internal System.Windows.Forms.TextBox TextBoxDescription;
        internal System.Windows.Forms.TextBox TextBoxWeb;
        internal System.Windows.Forms.Button ButtonEdit;
        internal System.Windows.Forms.Label LabelId;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuRecentPostBrowser;
        internal System.Windows.Forms.ToolStripMenuItem SelectAllToolStripMenuItem;
        internal System.Windows.Forms.TextBox TextBoxLocation;
        internal System.Windows.Forms.ToolTip ToolTip1;
        internal System.Windows.Forms.TextBox TextBoxName;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.Button ButtonSearchPosts;
        internal System.Windows.Forms.LinkLabel LinkLabel1;
        internal System.Windows.Forms.WebBrowser RecentPostBrowser;
        internal System.Windows.Forms.ToolStripMenuItem ChangeIconToolStripMenuItem;
        internal System.Windows.Forms.Label LabelIsVerified;
        internal System.Windows.Forms.WebBrowser DescriptionBrowser;
        internal System.Windows.Forms.Label LabelScreenName;
        internal System.Windows.Forms.Label LabelRecentPost;
        internal System.Windows.Forms.Button ButtonClose;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuUserPicture;
        internal System.Windows.Forms.PictureBox UserPicture;
        internal System.Windows.Forms.LinkLabel LinkLabelFav;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.Label LabelIsProtected;
        internal System.Windows.Forms.Label LabelCreatedAt;
        internal System.Windows.Forms.LinkLabel LinkLabelTweet;
        internal System.Windows.Forms.Label LabelIsFollowed;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Label LabelIsFollowing;
        internal System.Windows.Forms.LinkLabel LinkLabelFollowers;
        internal System.Windows.Forms.Button ButtonUnFollow;
        internal System.Windows.Forms.LinkLabel LinkLabelFollowing;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.Label LabelName;
        internal System.Windows.Forms.Button ButtonFollow;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Label LabelLocation;
        internal System.Windows.Forms.OpenFileDialog OpenFileDialogIcon;
        internal System.Windows.Forms.LinkLabel LinkLabelWeb;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Label2;
    }
}