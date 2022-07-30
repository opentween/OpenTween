namespace OpenTween
{
    partial class TweetDetailsView
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TweetDetailsView));
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.UserPicture = new OpenTween.OTPictureBox();
            this.ContextMenuUserPicture = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.FollowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UnFollowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowFriendShipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ListManageUserContextToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator37 = new System.Windows.Forms.ToolStripSeparator();
            this.ShowUserStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchPostsDetailNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchAtPostsDetailNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.IconNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReloadIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveIconPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NameLinkLabel = new System.Windows.Forms.LinkLabel();
            this.PostBrowser = new System.Windows.Forms.WebBrowser();
            this.ContextMenuPostBrowser = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SelectionSearchContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchGoogleContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchWikipediaContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchPublicSearchContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CurrentTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.SelectionCopyContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UrlCopyContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectionAllContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.FollowContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FriendshipContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FriendshipAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator36 = new System.Windows.Forms.ToolStripSeparator();
            this.ShowUserStatusContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchPostsDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchAtPostsDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator32 = new System.Windows.Forms.ToolStripSeparator();
            this.IdFilterAddMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ListManageUserContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator33 = new System.Windows.Forms.ToolStripSeparator();
            this.UseHashtagMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectionTranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DateTimeLabel = new System.Windows.Forms.LinkLabel();
            this.SourceLinkLabel = new System.Windows.Forms.LinkLabel();
            this.ContextMenuSource = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SourceCopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SourceUrlCopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserPicture)).BeginInit();
            this.ContextMenuUserPicture.SuspendLayout();
            this.ContextMenuPostBrowser.SuspendLayout();
            this.ContextMenuSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            resources.ApplyResources(this.TableLayoutPanel1, "TableLayoutPanel1");
            this.TableLayoutPanel1.Controls.Add(this.UserPicture, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.NameLinkLabel, 1, 0);
            this.TableLayoutPanel1.Controls.Add(this.PostBrowser, 1, 1);
            this.TableLayoutPanel1.Controls.Add(this.DateTimeLabel, 2, 0);
            this.TableLayoutPanel1.Controls.Add(this.SourceLinkLabel, 3, 0);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            // 
            // UserPicture
            // 
            resources.ApplyResources(this.UserPicture, "UserPicture");
            this.UserPicture.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.UserPicture.BackColor = System.Drawing.Color.White;
            this.UserPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UserPicture.ContextMenuStrip = this.ContextMenuUserPicture;
            this.UserPicture.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UserPicture.Name = "UserPicture";
            this.TableLayoutPanel1.SetRowSpan(this.UserPicture, 2);
            this.UserPicture.TabStop = false;
            this.UserPicture.Click += new System.EventHandler(this.UserPicture_Click);
            // 
            // ContextMenuUserPicture
            // 
            this.ContextMenuUserPicture.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FollowToolStripMenuItem,
            this.UnFollowToolStripMenuItem,
            this.ShowFriendShipToolStripMenuItem,
            this.ListManageUserContextToolStripMenuItem3,
            this.ToolStripSeparator37,
            this.ShowUserStatusToolStripMenuItem,
            this.SearchPostsDetailNameToolStripMenuItem,
            this.SearchAtPostsDetailNameToolStripMenuItem,
            this.ToolStripMenuItem1,
            this.IconNameToolStripMenuItem,
            this.ReloadIconToolStripMenuItem,
            this.SaveIconPictureToolStripMenuItem});
            this.ContextMenuUserPicture.Name = "ContextMenuStrip3";
            this.ContextMenuUserPicture.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            resources.ApplyResources(this.ContextMenuUserPicture, "ContextMenuUserPicture");
            this.ContextMenuUserPicture.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuUserPicture_Opening);
            // 
            // FollowToolStripMenuItem
            // 
            this.FollowToolStripMenuItem.Name = "FollowToolStripMenuItem";
            resources.ApplyResources(this.FollowToolStripMenuItem, "FollowToolStripMenuItem");
            this.FollowToolStripMenuItem.Click += new System.EventHandler(this.FollowToolStripMenuItem_Click);
            // 
            // UnFollowToolStripMenuItem
            // 
            this.UnFollowToolStripMenuItem.Name = "UnFollowToolStripMenuItem";
            resources.ApplyResources(this.UnFollowToolStripMenuItem, "UnFollowToolStripMenuItem");
            this.UnFollowToolStripMenuItem.Click += new System.EventHandler(this.UnFollowToolStripMenuItem_Click);
            // 
            // ShowFriendShipToolStripMenuItem
            // 
            this.ShowFriendShipToolStripMenuItem.Name = "ShowFriendShipToolStripMenuItem";
            resources.ApplyResources(this.ShowFriendShipToolStripMenuItem, "ShowFriendShipToolStripMenuItem");
            this.ShowFriendShipToolStripMenuItem.Click += new System.EventHandler(this.ShowFriendShipToolStripMenuItem_Click);
            // 
            // ListManageUserContextToolStripMenuItem3
            // 
            this.ListManageUserContextToolStripMenuItem3.Name = "ListManageUserContextToolStripMenuItem3";
            resources.ApplyResources(this.ListManageUserContextToolStripMenuItem3, "ListManageUserContextToolStripMenuItem3");
            this.ListManageUserContextToolStripMenuItem3.Click += new System.EventHandler(this.ListManageUserContextToolStripMenuItem_Click);
            // 
            // ToolStripSeparator37
            // 
            this.ToolStripSeparator37.Name = "ToolStripSeparator37";
            resources.ApplyResources(this.ToolStripSeparator37, "ToolStripSeparator37");
            // 
            // ShowUserStatusToolStripMenuItem
            // 
            this.ShowUserStatusToolStripMenuItem.Name = "ShowUserStatusToolStripMenuItem";
            resources.ApplyResources(this.ShowUserStatusToolStripMenuItem, "ShowUserStatusToolStripMenuItem");
            this.ShowUserStatusToolStripMenuItem.Click += new System.EventHandler(this.ShowUserStatusToolStripMenuItem_Click);
            // 
            // SearchPostsDetailNameToolStripMenuItem
            // 
            this.SearchPostsDetailNameToolStripMenuItem.Name = "SearchPostsDetailNameToolStripMenuItem";
            resources.ApplyResources(this.SearchPostsDetailNameToolStripMenuItem, "SearchPostsDetailNameToolStripMenuItem");
            this.SearchPostsDetailNameToolStripMenuItem.Click += new System.EventHandler(this.SearchPostsDetailNameToolStripMenuItem_Click);
            // 
            // SearchAtPostsDetailNameToolStripMenuItem
            // 
            this.SearchAtPostsDetailNameToolStripMenuItem.Name = "SearchAtPostsDetailNameToolStripMenuItem";
            resources.ApplyResources(this.SearchAtPostsDetailNameToolStripMenuItem, "SearchAtPostsDetailNameToolStripMenuItem");
            this.SearchAtPostsDetailNameToolStripMenuItem.Click += new System.EventHandler(this.SearchAtPostsDetailNameToolStripMenuItem_Click);
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            resources.ApplyResources(this.ToolStripMenuItem1, "ToolStripMenuItem1");
            // 
            // IconNameToolStripMenuItem
            // 
            this.IconNameToolStripMenuItem.Name = "IconNameToolStripMenuItem";
            resources.ApplyResources(this.IconNameToolStripMenuItem, "IconNameToolStripMenuItem");
            this.IconNameToolStripMenuItem.Click += new System.EventHandler(this.IconNameToolStripMenuItem_Click);
            // 
            // ReloadIconToolStripMenuItem
            // 
            this.ReloadIconToolStripMenuItem.Name = "ReloadIconToolStripMenuItem";
            resources.ApplyResources(this.ReloadIconToolStripMenuItem, "ReloadIconToolStripMenuItem");
            this.ReloadIconToolStripMenuItem.Click += new System.EventHandler(this.ReloadIconToolStripMenuItem_Click);
            // 
            // SaveIconPictureToolStripMenuItem
            // 
            this.SaveIconPictureToolStripMenuItem.Name = "SaveIconPictureToolStripMenuItem";
            resources.ApplyResources(this.SaveIconPictureToolStripMenuItem, "SaveIconPictureToolStripMenuItem");
            this.SaveIconPictureToolStripMenuItem.Click += new System.EventHandler(this.SaveIconPictureToolStripMenuItem_Click);
            // 
            // NameLinkLabel
            // 
            this.NameLinkLabel.ActiveLinkColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.NameLinkLabel, "NameLinkLabel");
            this.NameLinkLabel.AutoEllipsis = true;
            this.NameLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.NameLinkLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.NameLinkLabel.Name = "NameLinkLabel";
            this.NameLinkLabel.TabStop = true;
            this.NameLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.NameLinkLabel_LinkClicked);
            // 
            // PostBrowser
            // 
            resources.ApplyResources(this.PostBrowser, "PostBrowser");
            this.TableLayoutPanel1.SetColumnSpan(this.PostBrowser, 3);
            this.PostBrowser.ContextMenuStrip = this.ContextMenuPostBrowser;
            this.PostBrowser.IsWebBrowserContextMenuEnabled = false;
            this.PostBrowser.Name = "PostBrowser";
            this.PostBrowser.TabStop = false;
            this.PostBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.PostBrowser_Navigated);
            this.PostBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.PostBrowser_Navigating);
            this.PostBrowser.StatusTextChanged += new System.EventHandler(this.PostBrowser_StatusTextChanged);
            this.PostBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.PostBrowser_PreviewKeyDown);
            // 
            // ContextMenuPostBrowser
            // 
            this.ContextMenuPostBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SelectionSearchContextMenuItem,
            this.ToolStripSeparator13,
            this.SelectionCopyContextMenuItem,
            this.UrlCopyContextMenuItem,
            this.SelectionAllContextMenuItem,
            this.ToolStripSeparator5,
            this.FollowContextMenuItem,
            this.RemoveContextMenuItem,
            this.FriendshipContextMenuItem,
            this.FriendshipAllMenuItem,
            this.ToolStripSeparator36,
            this.ShowUserStatusContextMenuItem,
            this.SearchPostsDetailToolStripMenuItem,
            this.SearchAtPostsDetailToolStripMenuItem,
            this.ToolStripSeparator32,
            this.IdFilterAddMenuItem,
            this.ListManageUserContextToolStripMenuItem,
            this.ToolStripSeparator33,
            this.UseHashtagMenuItem,
            this.SelectionTranslationToolStripMenuItem,
            this.TranslationToolStripMenuItem});
            this.ContextMenuPostBrowser.Name = "ContextMenuStrip4";
            resources.ApplyResources(this.ContextMenuPostBrowser, "ContextMenuPostBrowser");
            this.ContextMenuPostBrowser.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuPostBrowser_Opening);
            // 
            // SelectionSearchContextMenuItem
            // 
            this.SelectionSearchContextMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SearchGoogleContextMenuItem,
            this.SearchWikipediaContextMenuItem,
            this.SearchPublicSearchContextMenuItem,
            this.CurrentTabToolStripMenuItem});
            this.SelectionSearchContextMenuItem.Name = "SelectionSearchContextMenuItem";
            resources.ApplyResources(this.SelectionSearchContextMenuItem, "SelectionSearchContextMenuItem");
            // 
            // SearchGoogleContextMenuItem
            // 
            this.SearchGoogleContextMenuItem.Name = "SearchGoogleContextMenuItem";
            resources.ApplyResources(this.SearchGoogleContextMenuItem, "SearchGoogleContextMenuItem");
            this.SearchGoogleContextMenuItem.Click += new System.EventHandler(this.SearchGoogleContextMenuItem_Click);
            // 
            // SearchWikipediaContextMenuItem
            // 
            this.SearchWikipediaContextMenuItem.Name = "SearchWikipediaContextMenuItem";
            resources.ApplyResources(this.SearchWikipediaContextMenuItem, "SearchWikipediaContextMenuItem");
            this.SearchWikipediaContextMenuItem.Click += new System.EventHandler(this.SearchWikipediaContextMenuItem_Click);
            // 
            // SearchPublicSearchContextMenuItem
            // 
            this.SearchPublicSearchContextMenuItem.Name = "SearchPublicSearchContextMenuItem";
            resources.ApplyResources(this.SearchPublicSearchContextMenuItem, "SearchPublicSearchContextMenuItem");
            this.SearchPublicSearchContextMenuItem.Click += new System.EventHandler(this.SearchPublicSearchContextMenuItem_Click);
            // 
            // CurrentTabToolStripMenuItem
            // 
            this.CurrentTabToolStripMenuItem.Name = "CurrentTabToolStripMenuItem";
            resources.ApplyResources(this.CurrentTabToolStripMenuItem, "CurrentTabToolStripMenuItem");
            this.CurrentTabToolStripMenuItem.Click += new System.EventHandler(this.CurrentTabToolStripMenuItem_Click);
            // 
            // ToolStripSeparator13
            // 
            this.ToolStripSeparator13.Name = "ToolStripSeparator13";
            resources.ApplyResources(this.ToolStripSeparator13, "ToolStripSeparator13");
            // 
            // SelectionCopyContextMenuItem
            // 
            this.SelectionCopyContextMenuItem.Name = "SelectionCopyContextMenuItem";
            resources.ApplyResources(this.SelectionCopyContextMenuItem, "SelectionCopyContextMenuItem");
            this.SelectionCopyContextMenuItem.Click += new System.EventHandler(this.SelectionCopyContextMenuItem_Click);
            // 
            // UrlCopyContextMenuItem
            // 
            resources.ApplyResources(this.UrlCopyContextMenuItem, "UrlCopyContextMenuItem");
            this.UrlCopyContextMenuItem.Name = "UrlCopyContextMenuItem";
            this.UrlCopyContextMenuItem.Click += new System.EventHandler(this.UrlCopyContextMenuItem_Click);
            // 
            // SelectionAllContextMenuItem
            // 
            this.SelectionAllContextMenuItem.Name = "SelectionAllContextMenuItem";
            resources.ApplyResources(this.SelectionAllContextMenuItem, "SelectionAllContextMenuItem");
            this.SelectionAllContextMenuItem.Click += new System.EventHandler(this.SelectionAllContextMenuItem_Click);
            // 
            // ToolStripSeparator5
            // 
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            resources.ApplyResources(this.ToolStripSeparator5, "ToolStripSeparator5");
            // 
            // FollowContextMenuItem
            // 
            this.FollowContextMenuItem.Name = "FollowContextMenuItem";
            resources.ApplyResources(this.FollowContextMenuItem, "FollowContextMenuItem");
            this.FollowContextMenuItem.Click += new System.EventHandler(this.FollowContextMenuItem_Click);
            // 
            // RemoveContextMenuItem
            // 
            this.RemoveContextMenuItem.Name = "RemoveContextMenuItem";
            resources.ApplyResources(this.RemoveContextMenuItem, "RemoveContextMenuItem");
            this.RemoveContextMenuItem.Click += new System.EventHandler(this.RemoveContextMenuItem_Click);
            // 
            // FriendshipContextMenuItem
            // 
            this.FriendshipContextMenuItem.Name = "FriendshipContextMenuItem";
            resources.ApplyResources(this.FriendshipContextMenuItem, "FriendshipContextMenuItem");
            this.FriendshipContextMenuItem.Click += new System.EventHandler(this.FriendshipContextMenuItem_Click);
            // 
            // FriendshipAllMenuItem
            // 
            this.FriendshipAllMenuItem.Name = "FriendshipAllMenuItem";
            resources.ApplyResources(this.FriendshipAllMenuItem, "FriendshipAllMenuItem");
            this.FriendshipAllMenuItem.Click += new System.EventHandler(this.FriendshipAllMenuItem_Click);
            // 
            // ToolStripSeparator36
            // 
            this.ToolStripSeparator36.Name = "ToolStripSeparator36";
            resources.ApplyResources(this.ToolStripSeparator36, "ToolStripSeparator36");
            // 
            // ShowUserStatusContextMenuItem
            // 
            this.ShowUserStatusContextMenuItem.Name = "ShowUserStatusContextMenuItem";
            resources.ApplyResources(this.ShowUserStatusContextMenuItem, "ShowUserStatusContextMenuItem");
            this.ShowUserStatusContextMenuItem.Click += new System.EventHandler(this.ShowUserStatusContextMenuItem_Click);
            // 
            // SearchPostsDetailToolStripMenuItem
            // 
            this.SearchPostsDetailToolStripMenuItem.Name = "SearchPostsDetailToolStripMenuItem";
            resources.ApplyResources(this.SearchPostsDetailToolStripMenuItem, "SearchPostsDetailToolStripMenuItem");
            this.SearchPostsDetailToolStripMenuItem.Click += new System.EventHandler(this.SearchPostsDetailToolStripMenuItem_Click);
            // 
            // SearchAtPostsDetailToolStripMenuItem
            // 
            this.SearchAtPostsDetailToolStripMenuItem.Name = "SearchAtPostsDetailToolStripMenuItem";
            resources.ApplyResources(this.SearchAtPostsDetailToolStripMenuItem, "SearchAtPostsDetailToolStripMenuItem");
            this.SearchAtPostsDetailToolStripMenuItem.Click += new System.EventHandler(this.SearchAtPostsDetailToolStripMenuItem_Click);
            // 
            // ToolStripSeparator32
            // 
            this.ToolStripSeparator32.Name = "ToolStripSeparator32";
            resources.ApplyResources(this.ToolStripSeparator32, "ToolStripSeparator32");
            // 
            // IdFilterAddMenuItem
            // 
            this.IdFilterAddMenuItem.Name = "IdFilterAddMenuItem";
            resources.ApplyResources(this.IdFilterAddMenuItem, "IdFilterAddMenuItem");
            this.IdFilterAddMenuItem.Click += new System.EventHandler(this.IdFilterAddMenuItem_Click);
            // 
            // ListManageUserContextToolStripMenuItem
            // 
            this.ListManageUserContextToolStripMenuItem.Name = "ListManageUserContextToolStripMenuItem";
            resources.ApplyResources(this.ListManageUserContextToolStripMenuItem, "ListManageUserContextToolStripMenuItem");
            this.ListManageUserContextToolStripMenuItem.Click += new System.EventHandler(this.ListManageUserContextToolStripMenuItem_Click);
            // 
            // ToolStripSeparator33
            // 
            this.ToolStripSeparator33.Name = "ToolStripSeparator33";
            resources.ApplyResources(this.ToolStripSeparator33, "ToolStripSeparator33");
            // 
            // UseHashtagMenuItem
            // 
            this.UseHashtagMenuItem.Name = "UseHashtagMenuItem";
            resources.ApplyResources(this.UseHashtagMenuItem, "UseHashtagMenuItem");
            this.UseHashtagMenuItem.Click += new System.EventHandler(this.UseHashtagMenuItem_Click);
            // 
            // SelectionTranslationToolStripMenuItem
            // 
            this.SelectionTranslationToolStripMenuItem.Name = "SelectionTranslationToolStripMenuItem";
            resources.ApplyResources(this.SelectionTranslationToolStripMenuItem, "SelectionTranslationToolStripMenuItem");
            this.SelectionTranslationToolStripMenuItem.Click += new System.EventHandler(this.SelectionTranslationToolStripMenuItem_Click);
            // 
            // TranslationToolStripMenuItem
            // 
            this.TranslationToolStripMenuItem.Name = "TranslationToolStripMenuItem";
            resources.ApplyResources(this.TranslationToolStripMenuItem, "TranslationToolStripMenuItem");
            this.TranslationToolStripMenuItem.Click += new System.EventHandler(this.TranslationToolStripMenuItem_Click);
            // 
            // DateTimeLabel
            // 
            this.DateTimeLabel.ActiveLinkColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.DateTimeLabel, "DateTimeLabel");
            this.DateTimeLabel.AutoEllipsis = true;
            this.DateTimeLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.DateTimeLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.DateTimeLabel.Name = "DateTimeLabel";
            this.DateTimeLabel.TabStop = true;
            this.DateTimeLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DateTimeLabel_LinkClicked);
            // 
            // SourceLinkLabel
            // 
            resources.ApplyResources(this.SourceLinkLabel, "SourceLinkLabel");
            this.SourceLinkLabel.AutoEllipsis = true;
            this.SourceLinkLabel.ContextMenuStrip = this.ContextMenuSource;
            this.SourceLinkLabel.Name = "SourceLinkLabel";
            this.SourceLinkLabel.TabStop = true;
            this.SourceLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SourceLinkLabel_LinkClicked);
            this.SourceLinkLabel.MouseEnter += new System.EventHandler(this.SourceLinkLabel_MouseEnter);
            this.SourceLinkLabel.MouseLeave += new System.EventHandler(this.SourceLinkLabel_MouseLeave);
            // 
            // ContextMenuSource
            // 
            this.ContextMenuSource.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SourceCopyMenuItem,
            this.SourceUrlCopyMenuItem});
            this.ContextMenuSource.Name = "ContextMenuSource";
            resources.ApplyResources(this.ContextMenuSource, "ContextMenuSource");
            this.ContextMenuSource.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuSource_Opening);
            // 
            // SourceCopyMenuItem
            // 
            this.SourceCopyMenuItem.Name = "SourceCopyMenuItem";
            resources.ApplyResources(this.SourceCopyMenuItem, "SourceCopyMenuItem");
            this.SourceCopyMenuItem.Click += new System.EventHandler(this.SourceCopyMenuItem_Click);
            // 
            // SourceUrlCopyMenuItem
            // 
            this.SourceUrlCopyMenuItem.Name = "SourceUrlCopyMenuItem";
            resources.ApplyResources(this.SourceUrlCopyMenuItem, "SourceUrlCopyMenuItem");
            this.SourceUrlCopyMenuItem.Click += new System.EventHandler(this.SourceUrlCopyMenuItem_Click);
            // 
            // TweetDetailsView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.TableLayoutPanel1);
            this.Name = "TweetDetailsView";
            this.FontChanged += new System.EventHandler(this.TweetDetailsView_FontChanged);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserPicture)).EndInit();
            this.ContextMenuUserPicture.ResumeLayout(false);
            this.ContextMenuPostBrowser.ResumeLayout(false);
            this.ContextMenuSource.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal OTPictureBox UserPicture;
        internal System.Windows.Forms.WebBrowser PostBrowser;
        internal System.Windows.Forms.LinkLabel SourceLinkLabel;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuPostBrowser;
        internal System.Windows.Forms.ToolStripMenuItem SelectionSearchContextMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SearchGoogleContextMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SearchWikipediaContextMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SearchPublicSearchContextMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem CurrentTabToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator13;
        internal System.Windows.Forms.ToolStripMenuItem SelectionCopyContextMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem UrlCopyContextMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SelectionAllContextMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator5;
        internal System.Windows.Forms.ToolStripMenuItem FollowContextMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem RemoveContextMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem FriendshipContextMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem FriendshipAllMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator36;
        internal System.Windows.Forms.ToolStripMenuItem ShowUserStatusContextMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SearchPostsDetailToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SearchAtPostsDetailToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator32;
        internal System.Windows.Forms.ToolStripMenuItem IdFilterAddMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ListManageUserContextToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator33;
        internal System.Windows.Forms.ToolStripMenuItem UseHashtagMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SelectionTranslationToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem TranslationToolStripMenuItem;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuUserPicture;
        internal System.Windows.Forms.ToolStripMenuItem FollowToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem UnFollowToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ShowFriendShipToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ListManageUserContextToolStripMenuItem3;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator37;
        internal System.Windows.Forms.ToolStripMenuItem ShowUserStatusToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SearchPostsDetailNameToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SearchAtPostsDetailNameToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
        internal System.Windows.Forms.ToolStripMenuItem IconNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ReloadIconToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SaveIconPictureToolStripMenuItem;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuSource;
        internal System.Windows.Forms.ToolStripMenuItem SourceCopyMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SourceUrlCopyMenuItem;
        private System.Windows.Forms.LinkLabel DateTimeLabel;
        private System.Windows.Forms.LinkLabel NameLinkLabel;
    }
}
