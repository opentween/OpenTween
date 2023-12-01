// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

// コンパイル後コマンド
// "c:\Program Files\Microsoft.NET\SDK\v2.0\Bin\sgen.exe" /f /a:"$(TargetPath)"
// "C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\sgen.exe" /f /a:"$(TargetPath)"

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Api;
using OpenTween.Api.DataModel;
using OpenTween.Api.GraphQL;
using OpenTween.Api.TwitterV2;
using OpenTween.Connection;
using OpenTween.MediaUploadServices;
using OpenTween.Models;
using OpenTween.OpenTweenCustomControl;
using OpenTween.Setting;
using OpenTween.Thumbnail;

namespace OpenTween
{
    public partial class TweenMain : OTBaseForm
    {
        // 各種設定

        /// <summary>画面サイズ</summary>
        private Size mySize;

        /// <summary>画面位置</summary>
        private Point myLoc;

        /// <summary>区切り位置</summary>
        private int mySpDis;

        /// <summary>発言欄区切り位置</summary>
        private int mySpDis2;

        /// <summary>プレビュー区切り位置</summary>
        private int mySpDis3;

        // 雑多なフラグ類
        private bool initial; // true:起動時処理中
        private bool initialLayout = true;
        private bool ignoreConfigSave; // true:起動時処理中

        /// <summary>タブドラッグ中フラグ（DoDragDropを実行するかの判定用）</summary>
        private bool tabDrag;

        private TabPage? beforeSelectedTab; // タブが削除されたときに前回選択されていたときのタブを選択する為に保持
        private Point tabMouseDownPoint;

        /// <summary>右クリックしたタブの名前（Tabコントロール機能不足対応）</summary>
        private string? rclickTabName;

        private readonly object syncObject = new(); // ロック用

        private const string DetailHtmlFormatHead =
            """<head><meta http-equiv="X-UA-Compatible" content="IE=8">"""
            + """<style type="text/css"><!-- """
            + "body, p, pre {margin: 0;} "
            + """body {font-family: "%FONT_FAMILY%", "Segoe UI Emoji", sans-serif; font-size: %FONT_SIZE%pt; background-color:rgb(%BG_COLOR%); word-wrap: break-word; color:rgb(%FONT_COLOR%);} """
            + "pre {font-family: inherit;} "
            + "a:link, a:visited, a:active, a:hover {color:rgb(%LINK_COLOR%); } "
            + "img.emoji {width: 1em; height: 1em; margin: 0 .05em 0 .1em; vertical-align: -0.1em; border: none;} "
            + ".quote-tweet {border: 1px solid #ccc; margin: 1em; padding: 0.5em;} "
            + ".quote-tweet.reply {border-color: rgb(%BG_REPLY_COLOR%);} "
            + ".quote-tweet-link {color: inherit !important; text-decoration: none;}"
            + "--></style>"
            + "</head>";

        private const string DetailHtmlFormatTemplateMono =
            $"<html>{DetailHtmlFormatHead}<body><pre>%CONTENT_HTML%</pre></body></html>";

        private const string DetailHtmlFormatTemplateNormal =
            $"<html>{DetailHtmlFormatHead}<body><p>%CONTENT_HTML%</p></body></html>";

        private string detailHtmlFormatPreparedTemplate = null!;

        private bool myStatusError = false;
        private bool myStatusOnline = false;
        private bool soundfileListup = false;
        private FormWindowState formWindowState = FormWindowState.Normal; // フォームの状態保存用 通知領域からアイコンをクリックして復帰した際に使用する

        // 設定ファイル
        private readonly SettingManager settings;

        // twitter解析部
        private readonly Twitter tw;

        // Growl呼び出し部
        private readonly GrowlHelper gh = new(ApplicationSettings.ApplicationName);

        // サブ画面インスタンス

        /// <summary>検索画面インスタンス</summary>
        internal SearchWordDialog SearchDialog = new();

        private readonly OpenURL urlDialog = new();

        /// <summary>@id補助</summary>
        public AtIdSupplement AtIdSupl = null!;

        /// <summary>Hashtag補助</summary>
        public AtIdSupplement HashSupl = null!;

        public HashtagManage HashMgr = null!;

        // 表示フォント、色、アイコン
        private ThemeManager themeManager;

        /// <summary>アイコン画像リスト</summary>
        private readonly ImageCache iconCache;

        private readonly IconAssetsManager iconAssets;

        private readonly ThumbnailGenerator thumbGenerator;

        /// <summary>発言履歴</summary>
        private readonly List<StatusTextHistory> history = new();

        /// <summary>発言履歴カレントインデックス</summary>
        private int hisIdx;

        // 発言投稿時のAPI引数（発言編集時に設定。手書きreplyでは設定されない）

        /// <summary>リプライ先のステータスID・スクリーン名</summary>
        private (PostId StatusId, string ScreenName)? inReplyTo = null;

        // 時速表示用
        private readonly List<DateTimeUtc> postTimestamps = new();
        private readonly List<DateTimeUtc> favTimestamps = new();

        // 以下DrawItem関連
        private readonly StringFormat sfTab = new();

        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>発言保持クラス</summary>
        private readonly TabInformations statuses;

        private TimelineListViewCache? listCache;
        private TimelineListViewDrawer? listDrawer;
        private readonly Dictionary<string, TimelineListViewState> listViewState = new();

        private bool isColumnChanged = false;

        private const int MaxWorderThreads = 20;
        private readonly SemaphoreSlim workerSemaphore = new(MaxWorderThreads);
        private readonly CancellationTokenSource workerCts = new();
        private readonly IProgress<string> workerProgress = null!;

        private int unreadCounter = -1;
        private int unreadAtCounter = -1;

        private readonly string[] columnOrgText = new string[9];
        private readonly string[] columnText = new string[9];

        private bool doFavRetweetFlags = false;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly TimelineScheduler timelineScheduler = new();
        private readonly DebounceTimer selectionDebouncer;
        private readonly DebounceTimer saveConfigDebouncer;

        private readonly string recommendedStatusFooter;
        private bool urlMultibyteSplit = false;
        private bool preventSmsCommand = true;

        // URL短縮のUndo用
        private readonly record struct UrlUndo(
            string Before,
            string After
        );

        private List<UrlUndo>? urlUndoBuffer = null;

        private readonly record struct ReplyChain(
            PostId OriginalId,
            PostId InReplyToId,
            TabModel OriginalTab
        );

        /// <summary>[, ]でのリプライ移動の履歴</summary>
        private Stack<ReplyChain>? replyChains;

        /// <summary>ポスト選択履歴</summary>
        private readonly Stack<(TabModel, PostClass?)> selectPostChains = new();

        public TabModel CurrentTab
            => this.statuses.SelectedTab;

        public string CurrentTabName
            => this.statuses.SelectedTabName;

        public TabPage CurrentTabPage
            => this.ListTab.TabPages[this.statuses.Tabs.IndexOf(this.CurrentTabName)];

        public DetailsListView CurrentListView
            => (DetailsListView)this.CurrentTabPage.Tag;

        public PostClass? CurrentPost
            => this.CurrentTab.SelectedPost;

        public bool Use2ColumnsMode
            => this.settings.Common.IconSize == MyCommon.IconSizes.Icon48_2;

        /// <summary>検索処理タイプ</summary>
        internal enum SEARCHTYPE
        {
            DialogSearch,
            NextSearch,
            PrevSearch,
        }

        private readonly record struct StatusTextHistory(
            string Status,
            (PostId StatusId, string ScreenName)? InReplyTo = null
        );

        private readonly HookGlobalHotkey hookGlobalHotkey;

        private void TweenMain_Activated(object sender, EventArgs e)
        {
            // 画面がアクティブになったら、発言欄の背景色戻す
            if (this.StatusText.Focused)
            {
                this.StatusText_Enter(this.StatusText, System.EventArgs.Empty);
            }
        }

        private bool disposed = false;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.disposed)
                return;

            if (disposing)
            {
                this.components?.Dispose();

                // 後始末
                this.SearchDialog.Dispose();
                this.urlDialog.Dispose();
                this.themeManager.Dispose();
                this.sfTab.Dispose();

                this.timelineScheduler.Dispose();
                this.workerCts.Cancel();
                this.thumbnailTokenSource?.Dispose();

                this.hookGlobalHotkey.Dispose();
            }

            // 終了時にRemoveHandlerしておかないとメモリリークする
            // http://msdn.microsoft.com/ja-jp/library/microsoft.win32.systemevents.powermodechanged.aspx
            Microsoft.Win32.SystemEvents.PowerModeChanged -= this.SystemEvents_PowerModeChanged;
            Microsoft.Win32.SystemEvents.TimeChanged -= this.SystemEvents_TimeChanged;

            this.disposed = true;
        }

        private void InitColumns(ListView list, bool startup)
        {
            this.InitColumnText();

            ColumnHeader[]? columns = null;
            try
            {
                if (this.Use2ColumnsMode)
                {
                    columns = new[]
                    {
                        new ColumnHeader(), // アイコン
                        new ColumnHeader(), // 本文
                    };

                    columns[0].Text = this.columnText[0];
                    columns[1].Text = this.columnText[2];

                    if (startup)
                    {
                        var widthScaleFactor = this.CurrentAutoScaleDimensions.Width / this.settings.Local.ScaleDimension.Width;

                        columns[0].Width = ScaleBy(widthScaleFactor, this.settings.Local.ColumnsWidth[0]);
                        columns[1].Width = ScaleBy(widthScaleFactor, this.settings.Local.ColumnsWidth[2]);
                        columns[0].DisplayIndex = 0;
                        columns[1].DisplayIndex = 1;
                    }
                    else
                    {
                        var idx = 0;
                        foreach (var curListColumn in this.CurrentListView.Columns.Cast<ColumnHeader>())
                        {
                            columns[idx].Width = curListColumn.Width;
                            columns[idx].DisplayIndex = curListColumn.DisplayIndex;
                            idx++;
                        }
                    }
                }
                else
                {
                    columns = new[]
                    {
                        new ColumnHeader(), // アイコン
                        new ColumnHeader(), // ニックネーム
                        new ColumnHeader(), // 本文
                        new ColumnHeader(), // 日付
                        new ColumnHeader(), // ユーザID
                        new ColumnHeader(), // 未読
                        new ColumnHeader(), // マーク＆プロテクト
                        new ColumnHeader(), // ソース
                    };

                    foreach (var i in Enumerable.Range(0, columns.Length))
                        columns[i].Text = this.columnText[i];

                    if (startup)
                    {
                        var widthScaleFactor = this.CurrentAutoScaleDimensions.Width / this.settings.Local.ScaleDimension.Width;

                        foreach (var (column, index) in columns.WithIndex())
                        {
                            column.Width = ScaleBy(widthScaleFactor, this.settings.Local.ColumnsWidth[index]);
                            column.DisplayIndex = this.settings.Local.ColumnsOrder[index];
                        }
                    }
                    else
                    {
                        var idx = 0;
                        foreach (var curListColumn in this.CurrentListView.Columns.Cast<ColumnHeader>())
                        {
                            columns[idx].Width = curListColumn.Width;
                            columns[idx].DisplayIndex = curListColumn.DisplayIndex;
                            idx++;
                        }
                    }
                }

                list.Columns.AddRange(columns);

                columns = null;
            }
            finally
            {
                if (columns != null)
                {
                    foreach (var column in columns)
                        column?.Dispose();
                }
            }
        }

        private void InitColumnText()
        {
            this.columnText[0] = "";
            this.columnText[1] = Properties.Resources.AddNewTabText2;
            this.columnText[2] = Properties.Resources.AddNewTabText3;
            this.columnText[3] = Properties.Resources.AddNewTabText4_2;
            this.columnText[4] = Properties.Resources.AddNewTabText5;
            this.columnText[5] = "";
            this.columnText[6] = "";
            this.columnText[7] = "Source";

            this.columnOrgText[0] = "";
            this.columnOrgText[1] = Properties.Resources.AddNewTabText2;
            this.columnOrgText[2] = Properties.Resources.AddNewTabText3;
            this.columnOrgText[3] = Properties.Resources.AddNewTabText4_2;
            this.columnOrgText[4] = Properties.Resources.AddNewTabText5;
            this.columnOrgText[5] = "";
            this.columnOrgText[6] = "";
            this.columnOrgText[7] = "Source";

            var c = this.statuses.SortMode switch
            {
                ComparerMode.Nickname => 1, // ニックネーム
                ComparerMode.Data => 2, // 本文
                ComparerMode.Id => 3, // 時刻=発言Id
                ComparerMode.Name => 4, // 名前
                ComparerMode.Source => 7, // Source
                _ => 0,
            };

            if (this.Use2ColumnsMode)
            {
                if (this.statuses.SortOrder == SortOrder.Descending)
                {
                    // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                    this.columnText[2] = this.columnOrgText[2] + "▾";
                }
                else
                {
                    // U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                    this.columnText[2] = this.columnOrgText[2] + "▴";
                }
            }
            else
            {
                if (this.statuses.SortOrder == SortOrder.Descending)
                {
                    // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                    this.columnText[c] = this.columnOrgText[c] + "▾";
                }
                else
                {
                    // U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                    this.columnText[c] = this.columnOrgText[c] + "▴";
                }
            }
        }

        public TweenMain(
            SettingManager settingManager,
            TabInformations tabInfo,
            Twitter twitter,
            ImageCache imageCache,
            IconAssetsManager iconAssets,
            ThumbnailGenerator thumbGenerator
        )
        {
            this.settings = settingManager;
            this.statuses = tabInfo;
            this.tw = twitter;
            this.iconCache = imageCache;
            this.iconAssets = iconAssets;
            this.thumbGenerator = thumbGenerator;

            this.InitializeComponent();

            if (!this.DesignMode)
            {
                // デザイナでの編集時にレイアウトが縦方向に数pxずれる問題の対策
                this.StatusText.Dock = DockStyle.Fill;
            }

            this.hookGlobalHotkey = new HookGlobalHotkey(this);

            this.hookGlobalHotkey.HotkeyPressed += this.HookGlobalHotkey_HotkeyPressed;
            this.gh.NotifyClicked += this.GrowlHelper_Callback;

            // メイリオフォント指定時にタブの最小幅が広くなる問題の対策
            this.ListTab.HandleCreated += (s, e) => NativeMethods.SetMinTabWidth((TabControl)s, 40);

            this.ImageSelector.Visible = false;
            this.ImageSelector.Enabled = false;
            this.ImageSelector.FilePickDialog = this.OpenFileDialog1;

            this.workerProgress = new Progress<string>(x => this.StatusLabel.Text = x);

            this.ReplaceAppName();
            this.InitializeShortcuts();

            this.ignoreConfigSave = true;
            this.Visible = false;

            this.TraceOutToolStripMenuItem.Checked = MyCommon.TraceFlag;

            Microsoft.Win32.SystemEvents.PowerModeChanged += this.SystemEvents_PowerModeChanged;

            Regex.CacheSize = 100;

            // アイコン設定
            this.Icon = this.iconAssets.IconMain; // メインフォーム（TweenMain）
            this.NotifyIcon1.Icon = this.iconAssets.IconTray; // タスクトレイ
            this.TabImage.Images.Add(this.iconAssets.IconTab); // タブ見出し

            // <<<<<<<<<設定関連>>>>>>>>>
            // 設定読み出し
            this.LoadConfig();

            // 現在の DPI と設定保存時の DPI との比を取得する
            var configScaleFactor = this.settings.Local.GetConfigScaleFactor(this.CurrentAutoScaleDimensions);

            // 認証関連
            var account = this.settings.Common.SelectedAccount;
            if (account != null)
                this.tw.Initialize(account.GetTwitterAppToken(), account.Token, account.TokenSecret, account.Username, account.UserId);
            else
                this.tw.Initialize(TwitterAppToken.GetDefault(), "", "", "", 0L);

            this.initial = true;

            this.tw.RestrictFavCheck = this.settings.Common.RestrictFavCheck;
            this.tw.ReadOwnPost = this.settings.Common.ReadOwnPost;

            // アクセストークンが有効であるか確認する
            // ここが Twitter API への最初のアクセスになるようにすること
            try
            {
                this.tw.VerifyCredentials();
            }
            catch (WebApiException ex)
            {
                MessageBox.Show(
                    this,
                    string.Format(Properties.Resources.StartupAuthError_Text, ex.Message),
                    ApplicationSettings.ApplicationName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            // サムネイル関連の初期化
            // プロキシ設定等の通信まわりの初期化が済んでから処理する
            var imgazyobizinet = this.thumbGenerator.ImgAzyobuziNet;
            imgazyobizinet.Enabled = this.settings.Common.EnableImgAzyobuziNet;
            imgazyobizinet.DisabledInDM = this.settings.Common.ImgAzyobuziNetDisabledInDM;
            imgazyobizinet.AutoUpdate = true;

            Thumbnail.Services.TonTwitterCom.GetApiConnection = () => this.tw.Api.Connection;

            // 画像投稿サービス
            this.ImageSelector.Model.InitializeServices(this.tw, this.tw.Configuration);
            this.ImageSelector.Model.SelectMediaService(this.settings.Common.UseImageServiceName, this.settings.Common.UseImageService);

            this.tweetThumbnail1.Model.Initialize(this.thumbGenerator);

            // ハッシュタグ/@id関連
            this.AtIdSupl = new AtIdSupplement(this.settings.AtIdList.AtIdList, "@");
            this.HashSupl = new AtIdSupplement(this.settings.Common.HashTags, "#");
            this.HashMgr = new HashtagManage(this.HashSupl,
                                    this.settings.Common.HashTags.ToArray(),
                                    this.settings.Common.HashSelected,
                                    this.settings.Common.HashIsPermanent,
                                    this.settings.Common.HashIsHead,
                                    this.settings.Common.HashIsNotAddToAtReply);
            if (!MyCommon.IsNullOrEmpty(this.HashMgr.UseHash) && this.HashMgr.IsPermanent) this.HashStripSplitButton.Text = this.HashMgr.UseHash;

            // フォント＆文字色＆背景色保持
            this.themeManager = new(this.settings.Local);
            this.tweetDetailsView.Initialize(this, this.iconCache, this.themeManager);

            // StringFormatオブジェクトへの事前設定
            this.sfTab.Alignment = StringAlignment.Center;
            this.sfTab.LineAlignment = StringAlignment.Center;

            this.InitDetailHtmlFormat();
            this.tweetDetailsView.ClearPostBrowser();

            this.recommendedStatusFooter = " [TWNv" + Regex.Replace(MyCommon.FileVersion.Replace(".", ""), "^0*", "") + "]";

            this.history.Add(new StatusTextHistory(""));
            this.hisIdx = 0;
            this.inReplyTo = null;

            // 各種ダイアログ設定
            this.SearchDialog.Owner = this;
            this.urlDialog.Owner = this;

            // 新着バルーン通知のチェック状態設定
            this.NewPostPopMenuItem.Checked = this.settings.Common.NewAllPop;
            this.NotifyFileMenuItem.Checked = this.NewPostPopMenuItem.Checked;

            // 新着取得時のリストスクロールをするか。trueならスクロールしない
            this.ListLockMenuItem.Checked = this.settings.Common.ListLock;
            this.LockListFileMenuItem.Checked = this.settings.Common.ListLock;
            // サウンド再生（タブ別設定より優先）
            this.PlaySoundMenuItem.Checked = this.settings.Common.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this.settings.Common.PlaySound;

            // ウィンドウ設定
            this.ClientSize = ScaleBy(configScaleFactor, this.settings.Local.FormSize);
            this.mySize = this.ClientSize; // サイズ保持（最小化・最大化されたまま終了した場合の対応用）
            this.myLoc = this.settings.Local.FormLocation;
            // タイトルバー領域
            if (this.WindowState != FormWindowState.Minimized)
            {
                var tbarRect = new Rectangle(this.myLoc, new Size(this.mySize.Width, SystemInformation.CaptionHeight));
                var outOfScreen = true;
                if (Screen.AllScreens.Length == 1) // ハングするとの報告
                {
                    foreach (var scr in Screen.AllScreens)
                    {
                        if (!Rectangle.Intersect(tbarRect, scr.Bounds).IsEmpty)
                        {
                            outOfScreen = false;
                            break;
                        }
                    }

                    if (outOfScreen)
                        this.myLoc = new Point(0, 0);
                }
                this.DesktopLocation = this.myLoc;
            }
            this.TopMost = this.settings.Common.AlwaysTop;
            this.mySpDis = ScaleBy(configScaleFactor.Height, this.settings.Local.SplitterDistance);
            this.mySpDis2 = ScaleBy(configScaleFactor.Height, this.settings.Local.StatusTextHeight);
            this.mySpDis3 = ScaleBy(configScaleFactor.Width, this.settings.Local.PreviewDistance);

            this.PlaySoundMenuItem.Checked = this.settings.Common.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this.settings.Common.PlaySound;
            // 入力欄
            this.StatusText.Font = this.themeManager.FontInputFont;
            this.StatusText.ForeColor = this.themeManager.ColorInputFont;

            // SplitContainer2.Panel2MinSize を一行表示の入力欄の高さに合わせる (MS UI Gothic 12pt (96dpi) の場合は 19px)
            this.StatusText.Multiline = false; // this.settings.Local.StatusMultiline の設定は後で反映される
            this.SplitContainer2.Panel2MinSize = this.StatusText.Height;

            // 必要であれば、発言一覧と発言詳細部・入力欄の上下を入れ替える
            this.SplitContainer1.IsPanelInverted = !this.settings.Common.StatusAreaAtBottom;

            // 全新着通知のチェック状態により、Reply＆DMの新着通知有効無効切り替え（タブ別設定にするため削除予定）
            if (this.settings.Common.UnreadManage == false)
            {
                this.ReadedStripMenuItem.Enabled = false;
                this.UnreadStripMenuItem.Enabled = false;
            }

            // リンク先URL表示部の初期化（画面左下）
            this.StatusLabelUrl.Text = "";
            // 状態表示部の初期化（画面右下）
            this.StatusLabel.Text = "";
            this.StatusLabel.AutoToolTip = false;
            this.StatusLabel.ToolTipText = "";
            // 文字カウンタ初期化
            this.lblLen.Text = this.GetRestStatusCount(this.FormatStatusTextExtended("")).ToString();

            this.JumpReadOpMenuItem.ShortcutKeyDisplayString = "Space";
            this.CopySTOTMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            this.CopyURLMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            this.CopyUserIdStripMenuItem.ShortcutKeyDisplayString = "Shift+Alt+C";

            // SourceLinkLabel のテキストが SplitContainer2.Panel2.AccessibleName にセットされるのを防ぐ
            // （タブオーダー順で SourceLinkLabel の次にある PostBrowser が TabStop = false となっているため、
            // さらに次のコントロールである SplitContainer2.Panel2 の AccessibleName がデフォルトで SourceLinkLabel のテキストになってしまう)
            this.SplitContainer2.Panel2.AccessibleName = "";

            ////////////////////////////////////////////////////////////////////////////////
            var sortOrder = (SortOrder)this.settings.Common.SortOrder;
            var mode = this.settings.Common.SortColumn switch
            {
                // 0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                0 or 5 or 6 => ComparerMode.Id, // Idソートに読み替え
                1 => ComparerMode.Nickname, // ニックネーム
                2 => ComparerMode.Data, // 本文
                3 => ComparerMode.Id, // 時刻=発言Id
                4 => ComparerMode.Name, // 名前
                7 => ComparerMode.Source, // Source
                _ => ComparerMode.Id,
            };
            this.statuses.SetSortMode(mode, sortOrder);
            ////////////////////////////////////////////////////////////////////////////////

            this.ApplyListViewIconSize(this.settings.Common.IconSize);

            // <<<<<<<<タブ関連>>>>>>>
            foreach (var tab in this.statuses.Tabs)
            {
                if (!this.AddNewTab(tab, startup: true))
                    throw new TabException(Properties.Resources.TweenMain_LoadText1);
            }

            this.ListTabSelect(this.ListTab.SelectedTab);

            // タブの位置を調整する
            this.SetTabAlignment();

            MyCommon.TwitterApiInfo.AccessLimitUpdated += this.TwitterApiStatus_AccessLimitUpdated;
            Microsoft.Win32.SystemEvents.TimeChanged += this.SystemEvents_TimeChanged;

            if (this.settings.Common.TabIconDisp)
            {
                this.ListTab.DrawMode = TabDrawMode.Normal;
            }
            else
            {
                this.ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                this.ListTab.DrawItem += this.ListTab_DrawItem;
                this.ListTab.ImageList = null;
            }

            if (this.settings.Common.HotkeyEnabled)
            {
                // グローバルホットキーの登録
                var modKey = HookGlobalHotkey.ModKeys.None;
                if ((this.settings.Common.HotkeyModifier & Keys.Alt) == Keys.Alt)
                    modKey |= HookGlobalHotkey.ModKeys.Alt;
                if ((this.settings.Common.HotkeyModifier & Keys.Control) == Keys.Control)
                    modKey |= HookGlobalHotkey.ModKeys.Ctrl;
                if ((this.settings.Common.HotkeyModifier & Keys.Shift) == Keys.Shift)
                    modKey |= HookGlobalHotkey.ModKeys.Shift;
                if ((this.settings.Common.HotkeyModifier & Keys.LWin) == Keys.LWin)
                    modKey |= HookGlobalHotkey.ModKeys.Win;

                this.hookGlobalHotkey.RegisterOriginalHotkey(this.settings.Common.HotkeyKey, this.settings.Common.HotkeyValue, modKey);
            }

            if (this.settings.Common.IsUseNotifyGrowl)
                this.gh.RegisterGrowl();

            this.StatusLabel.Text = Properties.Resources.Form1_LoadText1;       // 画面右下の状態表示を変更

            this.SetMainWindowTitle();
            this.SetNotifyIconText();

            if (!this.settings.Common.MinimizeToTray || this.WindowState != FormWindowState.Minimized)
            {
                this.Visible = true;
            }

            // タイマー設定

            this.timelineScheduler.UpdateFunc[TimelineSchedulerTaskType.Home] = () => this.InvokeAsync(() => this.RefreshTabAsync<HomeTabModel>());
            this.timelineScheduler.UpdateFunc[TimelineSchedulerTaskType.Mention] = () => this.InvokeAsync(() => this.RefreshTabAsync<MentionsTabModel>());
            this.timelineScheduler.UpdateFunc[TimelineSchedulerTaskType.Dm] = () => this.InvokeAsync(() => this.RefreshTabAsync<DirectMessagesTabModel>());
            this.timelineScheduler.UpdateFunc[TimelineSchedulerTaskType.PublicSearch] = () => this.InvokeAsync(() => this.RefreshTabAsync<PublicSearchTabModel>());
            this.timelineScheduler.UpdateFunc[TimelineSchedulerTaskType.User] = () => this.InvokeAsync(() => this.RefreshTabAsync<UserTimelineTabModel>());
            this.timelineScheduler.UpdateFunc[TimelineSchedulerTaskType.List] = () => this.InvokeAsync(() => this.RefreshTabAsync<ListTimelineTabModel>());
            this.timelineScheduler.UpdateFunc[TimelineSchedulerTaskType.Config] = () => this.InvokeAsync(() => Task.WhenAll(new[]
            {
                this.DoGetFollowersMenu(),
                this.RefreshBlockIdsAsync(),
                this.RefreshMuteUserIdsAsync(),
                this.RefreshNoRetweetIdsAsync(),
                this.RefreshTwitterConfigurationAsync(),
            }));
            this.RefreshTimelineScheduler();

            this.selectionDebouncer = DebounceTimer.Create(() => this.InvokeAsync(() => this.UpdateSelectedPost()), TimeSpan.FromMilliseconds(100), leading: true);
            this.saveConfigDebouncer = DebounceTimer.Create(() => this.InvokeAsync(() => this.SaveConfigsAll(ifModified: true)), TimeSpan.FromSeconds(1));

            // 更新中アイコンアニメーション間隔
            this.TimerRefreshIcon.Interval = 200;
            this.TimerRefreshIcon.Enabled = false;

            this.ignoreConfigSave = false;
            this.TweenMain_Resize(this, EventArgs.Empty);

            if (this.settings.IsFirstRun)
            {
                // 初回起動時だけ右下のメニューを目立たせる
                this.HashStripSplitButton.ShowDropDown();
            }
        }

        private void InitDetailHtmlFormat()
        {
            var htmlTemplate = this.settings.Common.IsMonospace ? DetailHtmlFormatTemplateMono : DetailHtmlFormatTemplateNormal;

            static string ColorToRGBString(Color color)
                => $"{color.R},{color.G},{color.B}";

            this.detailHtmlFormatPreparedTemplate = htmlTemplate
                .Replace("%FONT_FAMILY%", this.themeManager.FontDetail.Name)
                .Replace("%FONT_SIZE%", this.themeManager.FontDetail.Size.ToString())
                .Replace("%FONT_COLOR%", ColorToRGBString(this.themeManager.ColorDetail))
                .Replace("%LINK_COLOR%", ColorToRGBString(this.themeManager.ColorDetailLink))
                .Replace("%BG_COLOR%", ColorToRGBString(this.themeManager.ColorDetailBackcolor))
                .Replace("%BG_REPLY_COLOR%", ColorToRGBString(this.themeManager.ColorAtTo));
        }

        private void ListTab_DrawItem(object sender, DrawItemEventArgs e)
        {
            string txt;
            try
            {
                txt = this.statuses.Tabs[e.Index].TabName;
            }
            catch (Exception)
            {
                return;
            }

            e.Graphics.FillRectangle(System.Drawing.SystemBrushes.Control, e.Bounds);
            if (e.State == DrawItemState.Selected)
            {
                e.DrawFocusRectangle();
            }
            Brush fore;
            try
            {
                if (this.statuses.Tabs[txt].UnreadCount > 0)
                    fore = Brushes.Red;
                else
                    fore = System.Drawing.SystemBrushes.ControlText;
            }
            catch (Exception)
            {
                fore = System.Drawing.SystemBrushes.ControlText;
            }
            e.Graphics.DrawString(txt, e.Font, fore, e.Bounds, this.sfTab);
        }

        private void LoadConfig()
        {
            this.statuses.LoadTabsFromSettings(this.settings.Tabs);
            this.statuses.AddDefaultTabs();
        }

        private void TimerInterval_Changed(object sender, IntervalChangedEventArgs e)
        {
            this.RefreshTimelineScheduler();
        }

        private void RefreshTimelineScheduler()
        {
            static TimeSpan IntervalSecondsOrDisabled(int seconds)
                => seconds == 0 ? Timeout.InfiniteTimeSpan : TimeSpan.FromSeconds(seconds);

            this.timelineScheduler.UpdateInterval[TimelineSchedulerTaskType.Home] = IntervalSecondsOrDisabled(this.settings.Common.TimelinePeriod);
            this.timelineScheduler.UpdateInterval[TimelineSchedulerTaskType.Mention] = IntervalSecondsOrDisabled(this.settings.Common.ReplyPeriod);
            this.timelineScheduler.UpdateInterval[TimelineSchedulerTaskType.Dm] = IntervalSecondsOrDisabled(this.settings.Common.DMPeriod);
            this.timelineScheduler.UpdateInterval[TimelineSchedulerTaskType.PublicSearch] = IntervalSecondsOrDisabled(this.settings.Common.PubSearchPeriod);
            this.timelineScheduler.UpdateInterval[TimelineSchedulerTaskType.User] = IntervalSecondsOrDisabled(this.settings.Common.UserTimelinePeriod);
            this.timelineScheduler.UpdateInterval[TimelineSchedulerTaskType.List] = IntervalSecondsOrDisabled(this.settings.Common.ListsPeriod);
            this.timelineScheduler.UpdateInterval[TimelineSchedulerTaskType.Config] = TimeSpan.FromHours(6);
            this.timelineScheduler.UpdateAfterSystemResume = TimeSpan.FromSeconds(30);

            this.timelineScheduler.RefreshSchedule();
        }

        private void MarkSettingCommonModified()
        {
            if (this.saveConfigDebouncer == null)
                return;

            this.ModifySettingCommon = true;
            _ = this.saveConfigDebouncer.Call();
        }

        private void MarkSettingLocalModified()
        {
            if (this.saveConfigDebouncer == null)
                return;

            this.ModifySettingLocal = true;
            _ = this.saveConfigDebouncer.Call();
        }

        internal void MarkSettingAtIdModified()
        {
            if (this.saveConfigDebouncer == null)
                return;

            this.ModifySettingAtId = true;
            _ = this.saveConfigDebouncer.Call();
        }

        private void RefreshTimeline()
        {
            var curListView = this.CurrentListView;

            // 現在表示中のタブのスクロール位置を退避
            var currentListViewState = this.listViewState[this.CurrentTabName];
            currentListViewState.Save(this.ListLockMenuItem.Checked);

            // 更新確定
            int addCount;
            addCount = this.statuses.SubmitUpdate(
                out var soundFile,
                out var notifyPosts,
                out var newMentionOrDm,
                out var isDelete);

            if (MyCommon.EndingFlag) return;

            // リストに反映＆選択状態復元
            if (this.listCache != null && (this.listCache.IsListSizeMismatched || isDelete))
            {
                using (ControlTransaction.Update(curListView))
                {
                    this.listCache.PurgeCache();
                    this.listCache.UpdateListSize();

                    // 選択位置などを復元
                    currentListViewState.RestoreSelection();
                }
            }

            if (addCount > 0)
            {
                if (this.settings.Common.TabIconDisp)
                {
                    foreach (var (tab, index) in this.statuses.Tabs.WithIndex())
                    {
                        var tabPage = this.ListTab.TabPages[index];
                        if (tab.UnreadCount > 0 && tabPage.ImageIndex != 0)
                            tabPage.ImageIndex = 0; // 未読アイコン
                    }
                }
                else
                {
                    this.ListTab.Refresh();
                }
            }

            // スクロール位置を復元
            currentListViewState.RestoreScroll();

            // 新着通知
            this.NotifyNewPosts(notifyPosts, soundFile, addCount, newMentionOrDm);

            this.SetMainWindowTitle();
            if (!this.StatusLabelUrl.Text.StartsWith("http", StringComparison.Ordinal)) this.SetStatusLabelUrl();

            this.HashSupl.AddRangeItem(this.tw.GetHashList());
        }

        private bool BalloonRequired()
        {
            if (this.initial)
                return false;

            if (NativeMethods.IsScreenSaverRunning())
                return false;

            // 「新着通知」が無効
            if (!this.NewPostPopMenuItem.Checked)
                return false;

            // 「画面最小化・アイコン時のみバルーンを表示する」が有効
            if (this.settings.Common.LimitBalloon)
            {
                if (this.WindowState != FormWindowState.Minimized && this.Visible && Form.ActiveForm != null)
                    return false;
            }

            return true;
        }

        private void NotifyNewPosts(PostClass[] notifyPosts, string soundFile, int addCount, bool newMentions)
        {
            if (this.settings.Common.ReadOwnPost)
            {
                if (notifyPosts != null && notifyPosts.Length > 0 && notifyPosts.All(x => x.UserId == this.tw.UserId))
                    return;
            }

            // 新着通知
            if (this.BalloonRequired())
            {
                if (notifyPosts != null && notifyPosts.Length > 0)
                {
                    // Growlは一個ずつばらして通知。ただし、3ポスト以上あるときはまとめる
                    if (this.settings.Common.IsUseNotifyGrowl)
                    {
                        var sb = new StringBuilder();
                        var reply = false;
                        var dm = false;

                        foreach (var post in notifyPosts)
                        {
                            if (!(notifyPosts.Length > 3))
                            {
                                sb.Clear();
                                reply = false;
                                dm = false;
                            }
                            if (post.IsReply && !post.IsExcludeReply) reply = true;
                            if (post.IsDm) dm = true;
                            if (sb.Length > 0) sb.Append(System.Environment.NewLine);
                            switch (this.settings.Common.NameBalloon)
                            {
                                case MyCommon.NameBalloonEnum.UserID:
                                    sb.Append(post.ScreenName).Append(" : ");
                                    break;
                                case MyCommon.NameBalloonEnum.NickName:
                                    sb.Append(post.Nickname).Append(" : ");
                                    break;
                            }
                            sb.Append(post.TextFromApi);
                            if (notifyPosts.Length > 3)
                            {
                                if (notifyPosts.Last() != post) continue;
                            }

                            var title = new StringBuilder();
                            GrowlHelper.NotifyType nt;
                            if (this.settings.Common.DispUsername)
                            {
                                title.Append(this.tw.Username);
                                title.Append(" - ");
                            }

                            if (dm)
                            {
                                title.Append(ApplicationSettings.ApplicationName);
                                title.Append(" [DM] ");
                                title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                                nt = GrowlHelper.NotifyType.DirectMessage;
                            }
                            else if (reply)
                            {
                                title.Append(ApplicationSettings.ApplicationName);
                                title.Append(" [Reply!] ");
                                title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                                nt = GrowlHelper.NotifyType.Reply;
                            }
                            else
                            {
                                title.Append(ApplicationSettings.ApplicationName);
                                title.Append(" ");
                                title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                                nt = GrowlHelper.NotifyType.Notify;
                            }
                            var bText = sb.ToString();
                            if (MyCommon.IsNullOrEmpty(bText)) return;

                            var image = this.iconCache.TryGetFromCache(post.ImageUrl);
                            this.gh.Notify(nt, post.StatusId.Id, title.ToString(), bText, image?.Image, post.ImageUrl);
                        }
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        var reply = false;
                        var dm = false;
                        foreach (var post in notifyPosts)
                        {
                            if (post.IsReply && !post.IsExcludeReply) reply = true;
                            if (post.IsDm) dm = true;
                            if (sb.Length > 0) sb.Append(System.Environment.NewLine);
                            switch (this.settings.Common.NameBalloon)
                            {
                                case MyCommon.NameBalloonEnum.UserID:
                                    sb.Append(post.ScreenName).Append(" : ");
                                    break;
                                case MyCommon.NameBalloonEnum.NickName:
                                    sb.Append(post.Nickname).Append(" : ");
                                    break;
                            }
                            sb.Append(post.TextFromApi);
                        }

                        var title = new StringBuilder();
                        ToolTipIcon ntIcon;
                        if (this.settings.Common.DispUsername)
                        {
                            title.Append(this.tw.Username);
                            title.Append(" - ");
                        }

                        if (dm)
                        {
                            ntIcon = ToolTipIcon.Warning;
                            title.Append(ApplicationSettings.ApplicationName);
                            title.Append(" [DM] ");
                            title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                        }
                        else if (reply)
                        {
                            ntIcon = ToolTipIcon.Warning;
                            title.Append(ApplicationSettings.ApplicationName);
                            title.Append(" [Reply!] ");
                            title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                        }
                        else
                        {
                            ntIcon = ToolTipIcon.Info;
                            title.Append(ApplicationSettings.ApplicationName);
                            title.Append(" ");
                            title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                        }
                        var bText = sb.ToString();
                        if (MyCommon.IsNullOrEmpty(bText)) return;

                        this.NotifyIcon1.BalloonTipTitle = title.ToString();
                        this.NotifyIcon1.BalloonTipText = bText;
                        this.NotifyIcon1.BalloonTipIcon = ntIcon;
                        this.NotifyIcon1.ShowBalloonTip(500);
                    }
                }
            }

            // サウンド再生
            if (!this.initial && this.settings.Common.PlaySound && !MyCommon.IsNullOrEmpty(soundFile))
            {
                try
                {
                    var dir = Application.StartupPath;
                    if (Directory.Exists(Path.Combine(dir, "Sounds")))
                    {
                        dir = Path.Combine(dir, "Sounds");
                    }
                    using var player = new SoundPlayer(Path.Combine(dir, soundFile));
                    player.Play();
                }
                catch (Exception)
                {
                }
            }

            // mentions新着時に画面ブリンク
            if (!this.initial && this.settings.Common.BlinkNewMentions && newMentions && Form.ActiveForm == null)
            {
                NativeMethods.FlashMyWindow(this.Handle, 3);
            }
        }

        private async void MyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listView = this.CurrentListView;
            if (listView != sender)
                return;

            var indices = listView.SelectedIndices.Cast<int>().ToArray();
            this.CurrentTab.SelectPosts(indices);

            if (indices.Length != 1)
                return;

            var index = indices[0];
            if (index > listView.VirtualListSize - 1) return;

            this.PushSelectPostChain();

            var post = this.CurrentPost!;
            this.statuses.SetReadAllTab(post.StatusId, read: true);

            this.listCache?.RefreshStyle();
            await this.selectionDebouncer.Call();
        }

        private void StatusTextHistoryBack()
        {
            if (!string.IsNullOrWhiteSpace(this.StatusText.Text))
                this.history[this.hisIdx] = new StatusTextHistory(this.StatusText.Text, this.inReplyTo);

            this.hisIdx -= 1;
            if (this.hisIdx < 0)
                this.hisIdx = 0;

            var historyItem = this.history[this.hisIdx];
            this.inReplyTo = historyItem.InReplyTo;
            this.StatusText.Text = historyItem.Status;
            this.StatusText.SelectionStart = this.StatusText.Text.Length;
        }

        private void StatusTextHistoryForward()
        {
            if (!string.IsNullOrWhiteSpace(this.StatusText.Text))
                this.history[this.hisIdx] = new StatusTextHistory(this.StatusText.Text, this.inReplyTo);

            this.hisIdx += 1;
            if (this.hisIdx > this.history.Count - 1)
                this.hisIdx = this.history.Count - 1;

            var historyItem = this.history[this.hisIdx];
            this.inReplyTo = historyItem.InReplyTo;
            this.StatusText.Text = historyItem.Status;
            this.StatusText.SelectionStart = this.StatusText.Text.Length;
        }

        private async void PostButton_Click(object sender, EventArgs e)
        {
            if (this.StatusText.Text.Trim().Length == 0)
            {
                if (!this.ImageSelector.Enabled)
                {
                    await this.DoRefresh();
                    return;
                }
            }

            var currentPost = this.CurrentPost;
            if (this.ExistCurrentPost && currentPost != null && this.StatusText.Text.Trim() == string.Format("RT @{0}: {1}", currentPost.ScreenName, currentPost.TextFromApi))
            {
                var rtResult = MessageBox.Show(string.Format(Properties.Resources.PostButton_Click1, Environment.NewLine),
                                                               "Retweet",
                                                               MessageBoxButtons.YesNoCancel,
                                                               MessageBoxIcon.Question);
                switch (rtResult)
                {
                    case DialogResult.Yes:
                        this.StatusText.Text = "";
                        await this.DoReTweetOfficial(false);
                        return;
                    case DialogResult.Cancel:
                        return;
                }
            }

            if (TextContainsOnlyMentions(this.StatusText.Text))
            {
                var message = string.Format(Properties.Resources.PostConfirmText, this.StatusText.Text);
                var ret = MessageBox.Show(message, ApplicationSettings.ApplicationName, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                if (ret != DialogResult.OK)
                    return;
            }

            this.history[this.history.Count - 1] = new StatusTextHistory(this.StatusText.Text, this.inReplyTo);

            if (this.settings.Common.Nicoms)
            {
                this.StatusText.SelectionStart = this.StatusText.Text.Length;
                await this.UrlConvertAsync(MyCommon.UrlConverter.Nicoms);
            }

            this.StatusText.SelectionStart = this.StatusText.Text.Length;
            this.CheckReplyTo(this.StatusText.Text);

            var status = new PostStatusParams();

            var statusTextCompat = this.FormatStatusText(this.StatusText.Text);
            if (this.GetRestStatusCount(statusTextCompat) >= 0 && this.tw.Api.AppToken.AuthType == APIAuthType.OAuth1)
            {
                // auto_populate_reply_metadata や attachment_url を使用しなくても 140 字以内に
                // 収まる場合はこれらのオプションを使用せずに投稿する
                status.Text = statusTextCompat;
                status.InReplyToStatusId = this.inReplyTo?.StatusId;
            }
            else
            {
                status.Text = this.FormatStatusTextExtended(this.StatusText.Text, out var autoPopulatedUserIds, out var attachmentUrl);
                status.InReplyToStatusId = this.inReplyTo?.StatusId;

                status.AttachmentUrl = attachmentUrl;

                // リプライ先がセットされていても autoPopulatedUserIds が空の場合は auto_populate_reply_metadata を有効にしない
                //  (非公式 RT の場合など)
                var replyToPost = this.inReplyTo != null ? this.statuses[this.inReplyTo.Value.StatusId] : null;
                if (replyToPost != null && autoPopulatedUserIds.Length != 0)
                {
                    status.AutoPopulateReplyMetadata = true;

                    // ReplyToList のうち autoPopulatedUserIds に含まれていないユーザー ID を抽出
                    status.ExcludeReplyUserIds = replyToPost.ReplyToList.Select(x => x.UserId).Except(autoPopulatedUserIds)
                        .ToArray();
                }
            }

            if (this.GetRestStatusCount(status.Text) < 0)
            {
                // 文字数制限を超えているが強制的に投稿するか
                var ret = MessageBox.Show(Properties.Resources.PostLengthOverMessage1, Properties.Resources.PostLengthOverMessage2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (ret != DialogResult.OK)
                    return;
            }

            IMediaUploadService? uploadService = null;
            IMediaItem[]? uploadItems = null;
            if (this.ImageSelector.Visible)
            {
                // 画像投稿
                if (!this.ImageSelector.TryGetSelectedMedia(out var serviceName, out uploadItems))
                    return;

                this.ImageSelector.EndSelection();
                uploadService = this.ImageSelector.Model.GetService(serviceName);
            }

            this.inReplyTo = null;
            this.StatusText.Text = "";
            this.history.Add(new StatusTextHistory(""));
            this.hisIdx = this.history.Count - 1;
            if (!this.settings.Common.FocusLockToStatusText)
                this.CurrentListView.Focus();
            this.urlUndoBuffer = null;
            this.UrlUndoToolStripMenuItem.Enabled = false;  // Undoをできないように設定

            // Google検索（試験実装）
            if (this.StatusText.Text.StartsWith("Google:", StringComparison.OrdinalIgnoreCase) && this.StatusText.Text.Trim().Length > 7)
            {
                var tmp = string.Format(Properties.Resources.SearchItem2Url, Uri.EscapeDataString(this.StatusText.Text.Substring(7)));
                await MyCommon.OpenInBrowserAsync(this, tmp);
            }

            await this.PostMessageAsync(status, uploadService, uploadItems);
        }

        private void EndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyCommon.EndingFlag = true;
            this.Close();
        }

        private void TweenMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.settings.Common.CloseToExit && e.CloseReason == CloseReason.UserClosing && MyCommon.EndingFlag == false)
            {
                // _endingFlag=false:フォームの×ボタン
                e.Cancel = true;
                this.Visible = false;
            }
            else
            {
                this.hookGlobalHotkey.UnregisterAllOriginalHotkey();
                this.ignoreConfigSave = true;
                MyCommon.EndingFlag = true;
                this.timelineScheduler.Enabled = false;
                this.TimerRefreshIcon.Enabled = false;
            }
        }

        private void NotifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            this.Activate();
            this.BringToFront();
        }

        private static int errorCount = 0;

        private static bool CheckAccountValid()
        {
            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid)
            {
                errorCount += 1;
                if (errorCount > 5)
                {
                    errorCount = 0;
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                    return true;
                }
                return false;
            }
            errorCount = 0;
            return true;
        }

        /// <summary>指定された型 <typeparamref name="T"/> に合致する全てのタブを更新します</summary>
        private Task RefreshTabAsync<T>()
            where T : TabModel
            => this.RefreshTabAsync<T>(backward: false);

        /// <summary>指定された型 <typeparamref name="T"/> に合致する全てのタブを更新します</summary>
        private Task RefreshTabAsync<T>(bool backward)
            where T : TabModel
        {
            var loadTasks =
                from tab in this.statuses.GetTabsByType<T>()
                select this.RefreshTabAsync(tab, backward);

            return Task.WhenAll(loadTasks);
        }

        /// <summary>指定されたタブ <paramref name="tab"/> を更新します</summary>
        private Task RefreshTabAsync(TabModel tab)
            => this.RefreshTabAsync(tab, backward: false);

        /// <summary>指定されたタブ <paramref name="tab"/> を更新します</summary>
        private async Task RefreshTabAsync(TabModel tab, bool backward)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                this.RefreshTasktrayIcon();
                await Task.Run(() => tab.RefreshAsync(this.tw, backward, this.initial, this.workerProgress));
            }
            catch (WebApiException ex)
            {
                this.myStatusError = true;
                var tabType = tab switch
                {
                    HomeTabModel => "GetTimeline",
                    MentionsTabModel => "GetTimeline",
                    DirectMessagesTabModel => "GetDirectMessage",
                    FavoritesTabModel => "GetFavorites",
                    PublicSearchTabModel => "GetSearch",
                    UserTimelineTabModel => "GetUserTimeline",
                    ListTimelineTabModel => "GetListStatus",
                    RelatedPostsTabModel => "GetRelatedTweets",
                    _ => tab.GetType().Name.Replace("Model", ""),
                };
                this.StatusLabel.Text = $"Err:{ex.Message}({tabType})";
            }
            finally
            {
                this.RefreshTimeline();
                this.workerSemaphore.Release();
            }
        }

        private async Task FavAddAsync(PostId statusId, TabModel tab)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                this.RefreshTasktrayIcon();
                await this.FavAddAsyncInternal(progress, this.workerCts.Token, statusId, tab);
            }
            catch (WebApiException ex)
            {
                this.myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(PostFavAdd)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task FavAddAsyncInternal(IProgress<string> p, CancellationToken ct, PostId statusId, TabModel tab)
        {
            if (ct.IsCancellationRequested)
                return;

            if (!CheckAccountValid())
                throw new WebApiException("Auth error. Check your account");

            if (!tab.Posts.TryGetValue(statusId, out var post))
                return;

            if (post.IsFav)
                return;

            await Task.Run(async () =>
            {
                p.Report(string.Format(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText15, 0, 1, 0));

                try
                {
                    var twitterStatusId = (post.RetweetedId ?? post.StatusId).ToTwitterStatusId();
                    try
                    {
                        await this.tw.Api.FavoritesCreate(twitterStatusId)
                            .IgnoreResponse()
                            .ConfigureAwait(false);
                    }
                    catch (TwitterApiException ex)
                        when (ex.Errors.All(x => x.Code == TwitterErrorCode.AlreadyFavorited))
                    {
                        // エラーコード 139 のみの場合は成功と見なす
                    }

                    if (this.settings.Common.RestrictFavCheck)
                    {
                        var status = await this.tw.Api.StatusesShow(twitterStatusId)
                            .ConfigureAwait(false);

                        if (status.Favorited != true)
                            throw new WebApiException("NG(Restricted?)");
                    }

                    this.favTimestamps.Add(DateTimeUtc.Now);

                    // TLでも取得済みならfav反映
                    if (this.statuses.Posts.TryGetValue(statusId, out var postTl))
                    {
                        postTl.IsFav = true;

                        var favTab = this.statuses.FavoriteTab;
                        favTab.AddPostQueue(postTl);
                    }

                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                    foreach (var tb in this.statuses.GetTabsInnerStorageType())
                    {
                        if (tb.Contains(statusId))
                            tb.Posts[statusId].IsFav = true;
                    }

                    p.Report(string.Format(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText15, 1, 1, 0));
                }
                catch (WebApiException)
                {
                    p.Report(string.Format(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText15, 1, 1, 1));
                    throw;
                }

                // 時速表示用
                var oneHour = DateTimeUtc.Now - TimeSpan.FromHours(1);
                foreach (var i in MyCommon.CountDown(this.favTimestamps.Count - 1, 0))
                {
                    if (this.favTimestamps[i] < oneHour)
                        this.favTimestamps.RemoveAt(i);
                }

                this.statuses.DistributePosts();
            });

            if (ct.IsCancellationRequested)
                return;

            this.RefreshTimeline();

            if (this.CurrentTabName == tab.TabName)
            {
                using (ControlTransaction.Update(this.CurrentListView))
                {
                    var idx = tab.IndexOf(statusId);
                    if (idx != -1)
                        this.listCache?.RefreshStyle(idx);
                }

                var currentPost = this.CurrentPost;
                if (currentPost != null && statusId == currentPost.StatusId)
                    this.DispSelectedPost(true); // 選択アイテム再表示
            }
        }

        private async Task FavRemoveAsync(IReadOnlyList<PostId> statusIds, TabModel tab)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                this.RefreshTasktrayIcon();
                await this.FavRemoveAsyncInternal(progress, this.workerCts.Token, statusIds, tab);
            }
            catch (WebApiException ex)
            {
                this.myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(PostFavRemove)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task FavRemoveAsyncInternal(IProgress<string> p, CancellationToken ct, IReadOnlyList<PostId> statusIds, TabModel tab)
        {
            if (ct.IsCancellationRequested)
                return;

            if (!CheckAccountValid())
                throw new WebApiException("Auth error. Check your account");

            var successIds = new List<PostId>();

            await Task.Run(async () =>
            {
                // スレッド処理はしない
                var allCount = 0;
                var failedCount = 0;
                foreach (var statusId in statusIds)
                {
                    allCount++;

                    var post = tab.Posts[statusId];

                    p.Report(string.Format(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText17, allCount, statusIds.Count, failedCount));

                    if (!post.IsFav)
                        continue;

                    var twitterStatusId = (post.RetweetedId ?? post.StatusId).ToTwitterStatusId();

                    try
                    {
                        await this.tw.Api.FavoritesDestroy(twitterStatusId)
                            .IgnoreResponse()
                            .ConfigureAwait(false);
                    }
                    catch (WebApiException)
                    {
                        failedCount++;
                        continue;
                    }

                    successIds.Add(statusId);
                    post.IsFav = false; // リスト再描画必要

                    if (this.statuses.Posts.TryGetValue(statusId, out var tabinfoPost))
                        tabinfoPost.IsFav = false;

                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                    foreach (var tb in this.statuses.GetTabsInnerStorageType())
                    {
                        if (tb.Contains(statusId))
                            tb.Posts[statusId].IsFav = false;
                    }
                }
            });

            if (ct.IsCancellationRequested)
                return;

            var favTab = this.statuses.FavoriteTab;
            foreach (var statusId in successIds)
            {
                // ツイートが削除された訳ではないので IsDeleted はセットしない
                favTab.EnqueueRemovePost(statusId, setIsDeleted: false);
            }

            this.RefreshTimeline();

            if (this.CurrentTabName == tab.TabName)
            {
                if (tab.TabType == MyCommon.TabUsageType.Favorites)
                {
                    // 色変えは不要
                }
                else
                {
                    using (ControlTransaction.Update(this.CurrentListView))
                    {
                        foreach (var statusId in successIds)
                        {
                            var idx = tab.IndexOf(statusId);
                            if (idx != -1)
                                this.listCache?.RefreshStyle(idx);
                        }
                    }

                    var currentPost = this.CurrentPost;
                    if (currentPost != null && successIds.Contains(currentPost.StatusId))
                        this.DispSelectedPost(true); // 選択アイテム再表示
                }
            }
        }

        private async Task PostMessageAsync(PostStatusParams postParams, IMediaUploadService? uploadService, IMediaItem[]? uploadItems)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                this.RefreshTasktrayIcon();
                await this.PostMessageAsyncInternal(progress, this.workerCts.Token, postParams, uploadService, uploadItems);
            }
            catch (WebApiException ex)
            {
                this.myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(PostMessage)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task PostMessageAsyncInternal(
            IProgress<string> p,
            CancellationToken ct,
            PostStatusParams postParams,
            IMediaUploadService? uploadService,
            IMediaItem[]? uploadItems)
        {
            if (ct.IsCancellationRequested)
                return;

            if (!CheckAccountValid())
                throw new WebApiException("Auth error. Check your account");

            p.Report("Posting...");

            PostClass? post = null;
            var errMsg = "";

            try
            {
                await Task.Run(async () =>
                {
                    var postParamsWithMedia = postParams;

                    if (uploadService != null && uploadItems != null && uploadItems.Length > 0)
                    {
                        postParamsWithMedia = await uploadService.UploadAsync(uploadItems, postParamsWithMedia)
                            .ConfigureAwait(false);
                    }

                    post = await this.tw.PostStatus(postParamsWithMedia)
                        .ConfigureAwait(false);
                });

                p.Report(Properties.Resources.PostWorker_RunWorkerCompletedText4);
            }
            catch (WebApiException ex)
            {
                // 処理は中断せずエラーの表示のみ行う
                errMsg = $"Err:{ex.Message}(PostMessage)";
                p.Report(errMsg);
                this.myStatusError = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                // アップロード対象のファイルが開けなかった場合など
                errMsg = $"Err:{ex.Message}(PostMessage)";
                p.Report(errMsg);
                this.myStatusError = true;
            }
            finally
            {
                // 使い終わった MediaItem は破棄する
                if (uploadItems != null)
                {
                    foreach (var disposableItem in uploadItems.OfType<IDisposable>())
                    {
                        disposableItem.Dispose();
                    }
                }
            }

            if (ct.IsCancellationRequested)
                return;

            if (!MyCommon.IsNullOrEmpty(errMsg) &&
                !errMsg.StartsWith("OK:", StringComparison.Ordinal) &&
                !errMsg.StartsWith("Warn:", StringComparison.Ordinal))
            {
                var message = string.Format(Properties.Resources.StatusUpdateFailed, errMsg, postParams.Text);

                var ret = MessageBox.Show(
                    message,
                    "Failed to update status",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Question);

                if (ret == DialogResult.Retry)
                {
                    await this.PostMessageAsync(postParams, uploadService, uploadItems);
                }
                else
                {
                    this.StatusTextHistoryBack();
                    this.StatusText.Focus();

                    // 連投モードのときだけEnterイベントが起きないので強制的に背景色を戻す
                    if (this.settings.Common.FocusLockToStatusText)
                        this.StatusText_Enter(this.StatusText, EventArgs.Empty);
                }
                return;
            }

            this.postTimestamps.Add(DateTimeUtc.Now);

            var oneHour = DateTimeUtc.Now - TimeSpan.FromHours(1);
            foreach (var i in MyCommon.CountDown(this.postTimestamps.Count - 1, 0))
            {
                if (this.postTimestamps[i] < oneHour)
                    this.postTimestamps.RemoveAt(i);
            }

            if (!this.HashMgr.IsPermanent && !MyCommon.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                this.HashMgr.ClearHashtag();
                this.HashStripSplitButton.Text = "#[-]";
                this.HashTogglePullDownMenuItem.Checked = false;
                this.HashToggleMenuItem.Checked = false;
            }

            this.SetMainWindowTitle();

            // TLに反映
            if (post != null)
            {
                this.statuses.AddPost(post);
                this.statuses.DistributePosts();
                this.RefreshTimeline();
            }

            if (this.settings.Common.PostAndGet)
                await this.RefreshTabAsync<HomeTabModel>();
        }

        private async Task RetweetAsync(IReadOnlyList<PostId> statusIds)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                this.RefreshTasktrayIcon();
                await this.RetweetAsyncInternal(progress, this.workerCts.Token, statusIds);
            }
            catch (WebApiException ex)
            {
                this.myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(PostRetweet)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task RetweetAsyncInternal(IProgress<string> p, CancellationToken ct, IReadOnlyList<PostId> statusIds)
        {
            if (ct.IsCancellationRequested)
                return;

            if (!CheckAccountValid())
                throw new WebApiException("Auth error. Check your account");

            bool read;
            if (!this.settings.Common.UnreadManage)
                read = true;
            else
                read = this.initial && this.settings.Common.Read;

            p.Report("Posting...");

            var posts = new List<PostClass>();

            await Task.Run(async () =>
            {
                foreach (var statusId in statusIds)
                {
                    var post = await this.tw.PostRetweet(statusId, read).ConfigureAwait(false);
                    if (post != null) posts.Add(post);
                }
            });

            if (ct.IsCancellationRequested)
                return;

            p.Report(Properties.Resources.PostWorker_RunWorkerCompletedText4);

            this.postTimestamps.Add(DateTimeUtc.Now);

            var oneHour = DateTimeUtc.Now - TimeSpan.FromHours(1);
            foreach (var i in MyCommon.CountDown(this.postTimestamps.Count - 1, 0))
            {
                if (this.postTimestamps[i] < oneHour)
                    this.postTimestamps.RemoveAt(i);
            }

            // 自分のRTはTLの更新では取得できない場合があるので、
            // 投稿時取得の有無に関わらず追加しておく
            posts.ForEach(post => this.statuses.AddPost(post));

            if (this.settings.Common.PostAndGet)
            {
                await this.RefreshTabAsync<HomeTabModel>();
            }
            else
            {
                this.statuses.DistributePosts();
                this.RefreshTimeline();
            }
        }

        private async Task RefreshFollowerIdsAsync()
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                this.RefreshTasktrayIcon();
                this.StatusLabel.Text = Properties.Resources.UpdateFollowersMenuItem1_ClickText1;

                await this.tw.RefreshFollowerIds();

                this.StatusLabel.Text = Properties.Resources.UpdateFollowersMenuItem1_ClickText3;

                this.RefreshTimeline();
                this.listCache?.PurgeCache();
                this.CurrentListView.Refresh();
            }
            catch (WebApiException ex)
            {
                this.StatusLabel.Text = $"Err:{ex.Message}(RefreshFollowersIds)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task RefreshNoRetweetIdsAsync()
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                this.RefreshTasktrayIcon();
                await this.tw.RefreshNoRetweetIds();

                this.StatusLabel.Text = "NoRetweetIds refreshed";
            }
            catch (WebApiException ex)
            {
                this.StatusLabel.Text = $"Err:{ex.Message}(RefreshNoRetweetIds)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task RefreshBlockIdsAsync()
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                this.RefreshTasktrayIcon();
                this.StatusLabel.Text = Properties.Resources.UpdateBlockUserText1;

                await this.tw.RefreshBlockIds();

                this.StatusLabel.Text = Properties.Resources.UpdateBlockUserText3;
            }
            catch (WebApiException ex)
            {
                this.StatusLabel.Text = $"Err:{ex.Message}(RefreshBlockIds)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task RefreshTwitterConfigurationAsync()
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                this.RefreshTasktrayIcon();
                await this.tw.RefreshConfiguration();

                if (this.tw.Configuration.PhotoSizeLimit != 0)
                {
                    foreach (var (_, service) in this.ImageSelector.Model.MediaServices)
                    {
                        service.UpdateTwitterConfiguration(this.tw.Configuration);
                    }
                }

                this.listCache?.PurgeCache();
                this.CurrentListView.Refresh();
            }
            catch (WebApiException ex)
            {
                this.StatusLabel.Text = $"Err:{ex.Message}(RefreshConfiguration)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task RefreshMuteUserIdsAsync()
        {
            this.StatusLabel.Text = Properties.Resources.UpdateMuteUserIds_Start;

            try
            {
                await this.tw.RefreshMuteUserIdsAsync();
            }
            catch (WebApiException ex)
            {
                this.StatusLabel.Text = string.Format(Properties.Resources.UpdateMuteUserIds_Error, ex.Message);
                return;
            }

            this.StatusLabel.Text = Properties.Resources.UpdateMuteUserIds_Finish;
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = this.formWindowState;
                }
                this.Activate();
                this.BringToFront();
            }
        }

        private async void MyList_MouseDoubleClick(object sender, MouseEventArgs e)
            => await this.ListItemDoubleClickAction();

        private async Task ListItemDoubleClickAction()
        {
            switch (this.settings.Common.ListDoubleClickAction)
            {
                case MyCommon.ListItemDoubleClickActionType.Reply:
                    this.MakeReplyText();
                    break;
                case MyCommon.ListItemDoubleClickActionType.ReplyAll:
                    this.MakeReplyText(atAll: true);
                    break;
                case MyCommon.ListItemDoubleClickActionType.Favorite:
                    await this.FavoriteChange(true);
                    break;
                case MyCommon.ListItemDoubleClickActionType.ShowProfile:
                    var post = this.CurrentPost;
                    if (post != null)
                        await this.ShowUserStatus(post.ScreenName, false);
                    break;
                case MyCommon.ListItemDoubleClickActionType.ShowTimeline:
                    await this.ShowUserTimeline();
                    break;
                case MyCommon.ListItemDoubleClickActionType.ShowRelated:
                    this.ShowRelatedStatusesMenuItem_Click(this.ShowRelatedStatusesMenuItem, EventArgs.Empty);
                    break;
                case MyCommon.ListItemDoubleClickActionType.OpenHomeInBrowser:
                    this.AuthorOpenInBrowserMenuItem_Click(this.AuthorOpenInBrowserContextMenuItem, EventArgs.Empty);
                    break;
                case MyCommon.ListItemDoubleClickActionType.OpenStatusInBrowser:
                    this.StatusOpenMenuItem_Click(this.StatusOpenMenuItem, EventArgs.Empty);
                    break;
                case MyCommon.ListItemDoubleClickActionType.None:
                default:
                    // 動作なし
                    break;
            }
        }

        private async void FavAddToolStripMenuItem_Click(object sender, EventArgs e)
            => await this.FavoriteChange(true);

        private async void FavRemoveToolStripMenuItem_Click(object sender, EventArgs e)
            => await this.FavoriteChange(false);

        private async void FavoriteRetweetMenuItem_Click(object sender, EventArgs e)
            => await this.FavoritesRetweetOfficial();

        private async void FavoriteRetweetUnofficialMenuItem_Click(object sender, EventArgs e)
            => await this.FavoritesRetweetUnofficial();

        private async Task FavoriteChange(bool favAdd, bool multiFavoriteChangeDialogEnable = true)
        {
            var tab = this.CurrentTab;
            var posts = tab.SelectedPosts;

            // trueでFavAdd,falseでFavRemove
            if (tab.TabType == MyCommon.TabUsageType.DirectMessage || posts.Length == 0
                || !this.ExistCurrentPost) return;

            if (posts.Length > 1)
            {
                if (favAdd)
                {
                    // 複数ツイートの一括ふぁぼは禁止
                    // https://support.twitter.com/articles/76915#favoriting
                    MessageBox.Show(string.Format(Properties.Resources.FavoriteLimitCountText, 1));
                    this.doFavRetweetFlags = false;
                    return;
                }
                else
                {
                    if (multiFavoriteChangeDialogEnable)
                    {
                        var confirm = MessageBox.Show(
                            Properties.Resources.FavRemoveToolStripMenuItem_ClickText1,
                            Properties.Resources.FavRemoveToolStripMenuItem_ClickText2,
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Question);

                        if (confirm == DialogResult.Cancel)
                            return;
                    }
                }
            }

            if (favAdd)
            {
                var selectedPost = posts.Single();
                if (selectedPost.IsFav)
                {
                    this.StatusLabel.Text = Properties.Resources.FavAddToolStripMenuItem_ClickText4;
                    return;
                }

                await this.FavAddAsync(selectedPost.StatusId, tab);
            }
            else
            {
                var selectedPosts = posts.Where(x => x.IsFav);
                var statusIds = selectedPosts.Select(x => x.StatusId).ToArray();
                if (statusIds.Length == 0)
                {
                    this.StatusLabel.Text = Properties.Resources.FavRemoveToolStripMenuItem_ClickText4;
                    return;
                }

                await this.FavRemoveAsync(statusIds, tab);
            }
        }

        private async void AuthorOpenInBrowserMenuItem_Click(object sender, EventArgs e)
        {
            var post = this.CurrentPost;
            if (post != null)
                await MyCommon.OpenInBrowserAsync(this, MyCommon.TwitterUrl + post.ScreenName);
            else
                await MyCommon.OpenInBrowserAsync(this, MyCommon.TwitterUrl);
        }

        private void TweenMain_ClientSizeChanged(object sender, EventArgs e)
        {
            if ((!this.initialLayout) && this.Visible)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.mySize = this.ClientSize;
                    this.mySpDis = this.SplitContainer1.SplitterDistance;
                    this.mySpDis3 = this.SplitContainer3.SplitterDistance;
                    if (this.StatusText.Multiline) this.mySpDis2 = this.StatusText.Height;
                    this.MarkSettingLocalModified();
                }
            }
        }

        private void MyList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var comparerMode = this.GetComparerModeByColumnIndex(e.Column);
            if (comparerMode == null)
                return;

            this.SetSortColumn(comparerMode.Value);
        }

        /// <summary>
        /// 列インデックスからソートを行う ComparerMode を求める
        /// </summary>
        /// <param name="columnIndex">ソートを行うカラムのインデックス (表示上の順序とは異なる)</param>
        /// <returns>ソートを行う ComparerMode。null であればソートを行わない</returns>
        private ComparerMode? GetComparerModeByColumnIndex(int columnIndex)
        {
            if (this.Use2ColumnsMode)
                return ComparerMode.Id;

            return columnIndex switch
            {
                1 => ComparerMode.Nickname, // ニックネーム
                2 => ComparerMode.Data, // 本文
                3 => ComparerMode.Id, // 時刻=発言Id
                4 => ComparerMode.Name, // 名前
                7 => ComparerMode.Source, // Source
                _ => (ComparerMode?)null, // 0:アイコン, 5:未読マーク, 6:プロテクト・フィルターマーク
            };
        }

        /// <summary>
        /// 発言一覧の指定した位置の列でソートする
        /// </summary>
        /// <param name="columnIndex">ソートする列の位置 (表示上の順序で指定)</param>
        private void SetSortColumnByDisplayIndex(int columnIndex)
        {
            // 表示上の列の位置から ColumnHeader を求める
            var col = this.CurrentListView.Columns.Cast<ColumnHeader>()
                .FirstOrDefault(x => x.DisplayIndex == columnIndex);

            if (col == null)
                return;

            var comparerMode = this.GetComparerModeByColumnIndex(col.Index);
            if (comparerMode == null)
                return;

            this.SetSortColumn(comparerMode.Value);
        }

        /// <summary>
        /// 発言一覧の最後列の項目でソートする
        /// </summary>
        private void SetSortLastColumn()
        {
            // 表示上の最後列にある ColumnHeader を求める
            var col = this.CurrentListView.Columns.Cast<ColumnHeader>()
                .OrderByDescending(x => x.DisplayIndex)
                .First();

            var comparerMode = this.GetComparerModeByColumnIndex(col.Index);
            if (comparerMode == null)
                return;

            this.SetSortColumn(comparerMode.Value);
        }

        /// <summary>
        /// 発言一覧を指定された ComparerMode に基づいてソートする
        /// </summary>
        private void SetSortColumn(ComparerMode sortColumn)
        {
            if (this.settings.Common.SortOrderLock)
                return;

            this.statuses.ToggleSortOrder(sortColumn);
            this.InitColumnText();

            var list = this.CurrentListView;
            if (this.Use2ColumnsMode)
            {
                list.Columns[0].Text = this.columnText[0];
                list.Columns[1].Text = this.columnText[2];
            }
            else
            {
                for (var i = 0; i <= 7; i++)
                {
                    list.Columns[i].Text = this.columnText[i];
                }
            }

            this.listCache?.PurgeCache();

            var tab = this.CurrentTab;
            var post = this.CurrentPost;
            if (tab.AllCount > 0 && post != null)
            {
                var idx = tab.IndexOf(post.StatusId);
                if (idx > -1)
                {
                    this.SelectListItem(list, idx);
                    list.EnsureVisible(idx);
                }
            }
            list.Refresh();

            this.MarkSettingCommonModified();
        }

        private void TweenMain_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !this.initialLayout)
            {
                this.myLoc = this.DesktopLocation;
                this.MarkSettingLocalModified();
            }
        }

        private void ContextMenuOperate_Opening(object sender, CancelEventArgs e)
        {
            var post = this.CurrentPost;
            if (!this.ExistCurrentPost)
            {
                this.ReplyStripMenuItem.Enabled = false;
                this.ReplyAllStripMenuItem.Enabled = false;
                this.DMStripMenuItem.Enabled = false;
                this.TabMenuItem.Enabled = false;
                this.IDRuleMenuItem.Enabled = false;
                this.SourceRuleMenuItem.Enabled = false;
                this.ReadedStripMenuItem.Enabled = false;
                this.UnreadStripMenuItem.Enabled = false;
                this.AuthorContextMenuItem.Visible = false;
                this.RetweetedByContextMenuItem.Visible = false;
            }
            else
            {
                this.ReplyStripMenuItem.Enabled = true;
                this.ReplyAllStripMenuItem.Enabled = true;
                this.DMStripMenuItem.Enabled = true;
                this.TabMenuItem.Enabled = true;
                this.IDRuleMenuItem.Enabled = true;
                this.SourceRuleMenuItem.Enabled = true;
                this.ReadedStripMenuItem.Enabled = true;
                this.UnreadStripMenuItem.Enabled = true;
                this.AuthorContextMenuItem.Visible = true;
                this.AuthorContextMenuItem.Text = $"@{post!.ScreenName}";
                this.RetweetedByContextMenuItem.Visible = post.RetweetedByUserId != null;
                this.RetweetedByContextMenuItem.Text = $"@{post.RetweetedBy}";
            }
            var tab = this.CurrentTab;
            if (tab.TabType == MyCommon.TabUsageType.DirectMessage || !this.ExistCurrentPost || post == null || post.IsDm)
            {
                this.FavAddToolStripMenuItem.Enabled = false;
                this.FavRemoveToolStripMenuItem.Enabled = false;
                this.StatusOpenMenuItem.Enabled = false;
                this.ShowRelatedStatusesMenuItem.Enabled = false;

                this.ReTweetStripMenuItem.Enabled = false;
                this.ReTweetUnofficialStripMenuItem.Enabled = false;
                this.QuoteStripMenuItem.Enabled = false;
                this.FavoriteRetweetContextMenu.Enabled = false;
                this.FavoriteRetweetUnofficialContextMenu.Enabled = false;
            }
            else
            {
                this.FavAddToolStripMenuItem.Enabled = true;
                this.FavRemoveToolStripMenuItem.Enabled = true;
                this.StatusOpenMenuItem.Enabled = true;
                this.ShowRelatedStatusesMenuItem.Enabled = true;  // PublicSearchの時問題出るかも

                if (!post.CanRetweetBy(this.tw.UserId))
                {
                    this.ReTweetStripMenuItem.Enabled = false;
                    this.ReTweetUnofficialStripMenuItem.Enabled = false;
                    this.QuoteStripMenuItem.Enabled = false;
                    this.FavoriteRetweetContextMenu.Enabled = false;
                    this.FavoriteRetweetUnofficialContextMenu.Enabled = false;
                }
                else
                {
                    this.ReTweetStripMenuItem.Enabled = true;
                    this.ReTweetUnofficialStripMenuItem.Enabled = true;
                    this.QuoteStripMenuItem.Enabled = true;
                    this.FavoriteRetweetContextMenu.Enabled = true;
                    this.FavoriteRetweetUnofficialContextMenu.Enabled = true;
                }
            }

            if (!this.ExistCurrentPost || post == null || post.InReplyToStatusId == null)
            {
                this.RepliedStatusOpenMenuItem.Enabled = false;
            }
            else
            {
                this.RepliedStatusOpenMenuItem.Enabled = true;
            }

            if (this.ExistCurrentPost && post != null)
            {
                this.DeleteStripMenuItem.Enabled = post.CanDeleteBy(this.tw.UserId);
                if (post.RetweetedByUserId == this.tw.UserId)
                    this.DeleteStripMenuItem.Text = Properties.Resources.DeleteMenuText2;
                else
                    this.DeleteStripMenuItem.Text = Properties.Resources.DeleteMenuText1;
            }
        }

        private void ReplyStripMenuItem_Click(object sender, EventArgs e)
            => this.MakeReplyText();

        private void DMStripMenuItem_Click(object sender, EventArgs e)
            => this.MakeDirectMessageText();

        private async Task DoStatusDelete()
        {
            var posts = this.CurrentTab.SelectedPosts;
            if (posts.Length == 0)
                return;

            // 選択されたツイートの中に削除可能なものが一つでもあるか
            if (!posts.Any(x => x.CanDeleteBy(this.tw.UserId)))
                return;

            var ret = MessageBox.Show(
                this,
                string.Format(Properties.Resources.DeleteStripMenuItem_ClickText1, Environment.NewLine),
                Properties.Resources.DeleteStripMenuItem_ClickText2,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (ret != DialogResult.OK)
                return;

            var currentListView = this.CurrentListView;
            var focusedIndex = currentListView.FocusedItem?.Index ?? currentListView.TopItem?.Index ?? 0;

            using (ControlTransaction.Cursor(this, Cursors.WaitCursor))
            {
                Exception? lastException = null;
                foreach (var post in posts)
                {
                    if (!post.CanDeleteBy(this.tw.UserId))
                        continue;

                    try
                    {
                        if (post.StatusId is TwitterDirectMessageId dmId)
                        {
                            await this.tw.Api.DirectMessagesEventsDestroy(dmId);
                        }
                        else
                        {
                            if (post.RetweetedByUserId == this.tw.UserId)
                            {
                                // 自分が RT したツイート (自分が RT した自分のツイートも含む)
                                //   => RT を取り消し
                                await this.tw.DeleteRetweet(post);
                            }
                            else
                            {
                                if (post.UserId == this.tw.UserId)
                                {
                                    if (post.RetweetedId != null)
                                    {
                                        // 他人に RT された自分のツイート
                                        //   => RT 元の自分のツイートを削除
                                        await this.tw.DeleteTweet(post.RetweetedId.ToTwitterStatusId());
                                    }
                                    else
                                    {
                                        // 自分のツイート
                                        //   => ツイートを削除
                                        await this.tw.DeleteTweet(post.StatusId.ToTwitterStatusId());
                                    }
                                }
                            }
                        }
                    }
                    catch (WebApiException ex)
                    {
                        lastException = ex;
                        continue;
                    }

                    this.statuses.RemovePostFromAllTabs(post.StatusId, setIsDeleted: true);
                }

                if (lastException == null)
                    this.StatusLabel.Text = Properties.Resources.DeleteStripMenuItem_ClickText4; // 成功
                else
                    this.StatusLabel.Text = Properties.Resources.DeleteStripMenuItem_ClickText3; // 失敗

                using (ControlTransaction.Update(currentListView))
                {
                    this.listCache?.PurgeCache();
                    this.listCache?.UpdateListSize();

                    currentListView.SelectedIndices.Clear();

                    var currentTab = this.CurrentTab;
                    if (currentTab.AllCount != 0)
                    {
                        int selectedIndex;
                        if (currentTab.AllCount - 1 > focusedIndex && focusedIndex > -1)
                            selectedIndex = focusedIndex;
                        else
                            selectedIndex = currentTab.AllCount - 1;

                        currentListView.SelectedIndices.Add(selectedIndex);
                        currentListView.EnsureVisible(selectedIndex);
                        currentListView.FocusedItem = currentListView.Items[selectedIndex];
                    }
                }

                foreach (var (tab, index) in this.statuses.Tabs.WithIndex())
                {
                    var tabPage = this.ListTab.TabPages[index];
                    if (this.settings.Common.TabIconDisp && tab.UnreadCount == 0)
                    {
                        if (tabPage.ImageIndex == 0)
                            tabPage.ImageIndex = -1; // タブアイコン
                    }
                }

                if (!this.settings.Common.TabIconDisp)
                    this.ListTab.Refresh();
            }
        }

        private async void DeleteStripMenuItem_Click(object sender, EventArgs e)
            => await this.DoStatusDelete();

        private void ReadedStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Update(this.CurrentListView))
            {
                var tab = this.CurrentTab;
                foreach (var statusId in tab.SelectedStatusIds)
                {
                    this.statuses.SetReadAllTab(statusId, read: true);
                    var idx = tab.IndexOf(statusId);
                    this.listCache?.RefreshStyle(idx);
                }
            }
            if (this.settings.Common.TabIconDisp)
            {
                foreach (var (tab, index) in this.statuses.Tabs.WithIndex())
                {
                    if (tab.UnreadCount == 0)
                    {
                        var tabPage = this.ListTab.TabPages[index];
                        if (tabPage.ImageIndex == 0)
                            tabPage.ImageIndex = -1; // タブアイコン
                    }
                }
            }
            if (!this.settings.Common.TabIconDisp) this.ListTab.Refresh();
        }

        private void UnreadStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Update(this.CurrentListView))
            {
                var tab = this.CurrentTab;
                foreach (var statusId in tab.SelectedStatusIds)
                {
                    this.statuses.SetReadAllTab(statusId, read: false);
                    var idx = tab.IndexOf(statusId);
                    this.listCache?.RefreshStyle(idx);
                }
            }
            if (this.settings.Common.TabIconDisp)
            {
                foreach (var (tab, index) in this.statuses.Tabs.WithIndex())
                {
                    if (tab.UnreadCount > 0)
                    {
                        var tabPage = this.ListTab.TabPages[index];
                        if (tabPage.ImageIndex == -1)
                            tabPage.ImageIndex = 0; // タブアイコン
                    }
                }
            }
            if (!this.settings.Common.TabIconDisp) this.ListTab.Refresh();
        }

        private async void RefreshStripMenuItem_Click(object sender, EventArgs e)
            => await this.DoRefresh();

        private async Task DoRefresh()
            => await this.RefreshTabAsync(this.CurrentTab);

        private async Task DoRefreshMore()
            => await this.RefreshTabAsync(this.CurrentTab, backward: true);

        private DialogResult ShowSettingDialog()
        {
            using var settingDialog = new AppendSettingDialog();
            settingDialog.Icon = this.iconAssets.IconMain;
            settingDialog.IntervalChanged += this.TimerInterval_Changed;

            settingDialog.LoadConfig(this.settings.Common, this.settings.Local);

            DialogResult result;
            try
            {
                result = settingDialog.ShowDialog(this);
            }
            catch (Exception)
            {
                return DialogResult.Abort;
            }

            if (result == DialogResult.OK)
            {
                lock (this.syncObject)
                {
                    settingDialog.SaveConfig(this.settings.Common, this.settings.Local);
                }
            }

            return result;
        }

        private async void SettingStripMenuItem_Click(object sender, EventArgs e)
        {
            // 設定画面表示前のユーザー情報
            var previousUserId = this.settings.Common.UserId;
            var oldIconCol = this.Use2ColumnsMode;

            if (this.ShowSettingDialog() == DialogResult.OK)
            {
                lock (this.syncObject)
                {
                    this.settings.ApplySettings();

                    if (MyCommon.IsNullOrEmpty(this.settings.Common.Token))
                        this.tw.ClearAuthInfo();

                    var account = this.settings.Common.SelectedAccount;
                    if (account != null)
                        this.tw.Initialize(account.GetTwitterAppToken(), account.Token, account.TokenSecret, account.Username, account.UserId);
                    else
                        this.tw.Initialize(TwitterAppToken.GetDefault(), "", "", "", 0L);

                    this.tw.RestrictFavCheck = this.settings.Common.RestrictFavCheck;
                    this.tw.ReadOwnPost = this.settings.Common.ReadOwnPost;

                    this.ImageSelector.Model.InitializeServices(this.tw, this.tw.Configuration);

                    try
                    {
                        if (this.settings.Common.TabIconDisp)
                        {
                            this.ListTab.DrawItem -= this.ListTab_DrawItem;
                            this.ListTab.DrawMode = TabDrawMode.Normal;
                            this.ListTab.ImageList = this.TabImage;
                        }
                        else
                        {
                            this.ListTab.DrawItem -= this.ListTab_DrawItem;
                            this.ListTab.DrawItem += this.ListTab_DrawItem;
                            this.ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                            this.ListTab.ImageList = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(TabIconDisp)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        if (!this.settings.Common.UnreadManage)
                        {
                            this.ReadedStripMenuItem.Enabled = false;
                            this.UnreadStripMenuItem.Enabled = false;
                            if (this.settings.Common.TabIconDisp)
                            {
                                foreach (TabPage myTab in this.ListTab.TabPages)
                                {
                                    myTab.ImageIndex = -1;
                                }
                            }
                        }
                        else
                        {
                            this.ReadedStripMenuItem.Enabled = true;
                            this.UnreadStripMenuItem.Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(UnreadManage)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    // タブの表示位置の決定
                    this.SetTabAlignment();

                    this.SplitContainer1.IsPanelInverted = !this.settings.Common.StatusAreaAtBottom;

                    var imgazyobizinet = this.thumbGenerator.ImgAzyobuziNet;
                    imgazyobizinet.Enabled = this.settings.Common.EnableImgAzyobuziNet;
                    imgazyobizinet.DisabledInDM = this.settings.Common.ImgAzyobuziNetDisabledInDM;

                    this.NewPostPopMenuItem.Checked = this.settings.Common.NewAllPop;
                    this.NotifyFileMenuItem.Checked = this.settings.Common.NewAllPop;
                    this.PlaySoundMenuItem.Checked = this.settings.Common.PlaySound;
                    this.PlaySoundFileMenuItem.Checked = this.settings.Common.PlaySound;

                    var newTheme = new ThemeManager(this.settings.Local);
                    (var oldTheme, this.themeManager) = (this.themeManager, newTheme);
                    this.tweetDetailsView.Theme = this.themeManager;
                    if (this.listDrawer != null)
                        this.listDrawer.Theme = this.themeManager;
                    oldTheme.Dispose();

                    try
                    {
                        if (this.StatusText.Focused)
                            this.StatusText.BackColor = this.themeManager.ColorInputBackcolor;

                        this.StatusText.Font = this.themeManager.FontInputFont;
                        this.StatusText.ForeColor = this.themeManager.ColorInputFont;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    try
                    {
                        this.InitDetailHtmlFormat();
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "Font";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        if (this.settings.Common.TabIconDisp)
                        {
                            foreach (var (tab, index) in this.statuses.Tabs.WithIndex())
                            {
                                var tabPage = this.ListTab.TabPages[index];
                                if (tab.UnreadCount == 0)
                                    tabPage.ImageIndex = -1;
                                else
                                    tabPage.ImageIndex = 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(TabIconDisp no2)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        this.ApplyListViewIconSize(this.settings.Common.IconSize);

                        foreach (TabPage tp in this.ListTab.TabPages)
                        {
                            var lst = (DetailsListView)tp.Tag;

                            using (ControlTransaction.Update(lst))
                            {
                                lst.GridLines = this.settings.Common.ShowGrid;

                                if (this.Use2ColumnsMode != oldIconCol)
                                    this.ResetColumns(lst);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListView(IconSize)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    this.SetMainWindowTitle();
                    this.SetNotifyIconText();

                    this.listCache?.PurgeCache();
                    this.CurrentListView.Refresh();
                    this.ListTab.Refresh();

                    this.hookGlobalHotkey.UnregisterAllOriginalHotkey();
                    if (this.settings.Common.HotkeyEnabled)
                    {
                        // グローバルホットキーの登録。設定で変更可能にするかも
                        var modKey = HookGlobalHotkey.ModKeys.None;
                        if ((this.settings.Common.HotkeyModifier & Keys.Alt) == Keys.Alt)
                            modKey |= HookGlobalHotkey.ModKeys.Alt;
                        if ((this.settings.Common.HotkeyModifier & Keys.Control) == Keys.Control)
                            modKey |= HookGlobalHotkey.ModKeys.Ctrl;
                        if ((this.settings.Common.HotkeyModifier & Keys.Shift) == Keys.Shift)
                            modKey |= HookGlobalHotkey.ModKeys.Shift;
                        if ((this.settings.Common.HotkeyModifier & Keys.LWin) == Keys.LWin)
                            modKey |= HookGlobalHotkey.ModKeys.Win;

                        this.hookGlobalHotkey.RegisterOriginalHotkey(this.settings.Common.HotkeyKey, this.settings.Common.HotkeyValue, modKey);
                    }

                    if (this.settings.Common.IsUseNotifyGrowl) this.gh.RegisterGrowl();
                    try
                    {
                        this.StatusText_TextChanged(this.StatusText, EventArgs.Empty);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;

            this.TopMost = this.settings.Common.AlwaysTop;
            this.SaveConfigsAll(false);

            if (this.tw.UserId != previousUserId)
                await this.DoGetFollowersMenu();
        }

        /// <summary>
        /// タブの表示位置を設定する
        /// </summary>
        private void SetTabAlignment()
        {
            var newAlignment = this.settings.Common.ViewTabBottom ? TabAlignment.Bottom : TabAlignment.Top;
            if (this.ListTab.Alignment == newAlignment) return;

            // リスト上の選択位置などを退避
            var currentListViewState = this.listViewState[this.CurrentTabName];
            currentListViewState.Save(this.ListLockMenuItem.Checked);

            this.ListTab.Alignment = newAlignment;

            currentListViewState.Restore(forceScroll: true);
        }

        private void ApplyListViewIconSize(MyCommon.IconSizes iconSz)
        {
            // アイコンサイズの再設定
            if (this.listDrawer != null)
            {
                this.listDrawer.IconSize = iconSz;
                this.listDrawer.UpdateItemHeight();
            }

            this.listCache?.PurgeCache();
        }

        private void ResetColumns(DetailsListView list)
        {
            using (ControlTransaction.Update(list))
            using (ControlTransaction.Layout(list, false))
            {
                // カラムヘッダの再設定
                list.ColumnClick -= this.MyList_ColumnClick;
                list.DrawColumnHeader -= this.MyList_DrawColumnHeader;
                list.ColumnReordered -= this.MyList_ColumnReordered;
                list.ColumnWidthChanged -= this.MyList_ColumnWidthChanged;

                var cols = list.Columns.Cast<ColumnHeader>().ToList();
                list.Columns.Clear();
                cols.ForEach(col => col.Dispose());
                cols.Clear();

                this.InitColumns(list, true);

                list.ColumnClick += this.MyList_ColumnClick;
                list.DrawColumnHeader += this.MyList_DrawColumnHeader;
                list.ColumnReordered += this.MyList_ColumnReordered;
                list.ColumnWidthChanged += this.MyList_ColumnWidthChanged;
            }
        }

        public void AddNewTabForSearch(string searchWord)
        {
            // 同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (var tb in this.statuses.GetTabsByType<PublicSearchTabModel>())
            {
                if (tb.SearchWords == searchWord && MyCommon.IsNullOrEmpty(tb.SearchLang))
                {
                    var tabIndex = this.statuses.Tabs.IndexOf(tb);
                    this.ListTab.SelectedIndex = tabIndex;
                    return;
                }
            }
            // ユニークなタブ名生成
            var tabName = searchWord;
            for (var i = 0; i <= 100; i++)
            {
                if (this.statuses.ContainsTab(tabName))
                    tabName += "_";
                else
                    break;
            }
            // タブ追加
            var tab = new PublicSearchTabModel(tabName);
            this.statuses.AddTab(tab);
            this.AddNewTab(tab, startup: false);
            // 追加したタブをアクティブに
            this.ListTab.SelectedIndex = this.statuses.Tabs.Count - 1;
            // 検索条件の設定
            var tabPage = this.CurrentTabPage;
            var cmb = (ComboBox)tabPage.Controls["panelSearch"].Controls["comboSearch"];
            cmb.Items.Add(searchWord);
            cmb.Text = searchWord;
            this.SaveConfigsTabs();
            // 検索実行
            this.SearchButton_Click(tabPage.Controls["panelSearch"].Controls["comboSearch"], EventArgs.Empty);
        }

        private async Task ShowUserTimeline()
        {
            var post = this.CurrentPost;
            if (post == null || !this.ExistCurrentPost) return;
            await this.AddNewTabForUserTimeline(post.ScreenName);
        }

        private async Task ShowRetweeterTimeline()
        {
            var retweetedBy = this.CurrentPost?.RetweetedBy;
            if (retweetedBy == null || !this.ExistCurrentPost) return;
            await this.AddNewTabForUserTimeline(retweetedBy);
        }

        private void SearchComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.RemoveSpecifiedTab(this.CurrentTabName, false);
                this.SaveConfigsTabs();
                e.SuppressKeyPress = true;
            }
        }

        public async Task AddNewTabForUserTimeline(string user)
        {
            // 同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (var tb in this.statuses.GetTabsByType<UserTimelineTabModel>())
            {
                if (tb.ScreenName == user)
                {
                    var tabIndex = this.statuses.Tabs.IndexOf(tb);
                    this.ListTab.SelectedIndex = tabIndex;
                    return;
                }
            }
            // ユニークなタブ名生成
            var tabName = "user:" + user;
            while (this.statuses.ContainsTab(tabName))
            {
                tabName += "_";
            }
            // タブ追加
            var tab = new UserTimelineTabModel(tabName, user);
            this.statuses.AddTab(tab);
            this.AddNewTab(tab, startup: false);
            // 追加したタブをアクティブに
            this.ListTab.SelectedIndex = this.statuses.Tabs.Count - 1;
            this.SaveConfigsTabs();
            // 検索実行
            await this.RefreshTabAsync(tab);
        }

        public bool AddNewTab(TabModel tab, bool startup)
        {
            // 重複チェック
            if (this.ListTab.TabPages.Cast<TabPage>().Any(x => x.Text == tab.TabName))
                return false;

            // 新規タブ名チェック
            if (tab.TabName == Properties.Resources.AddNewTabText1) return false;

            var tabPage = new TabPage();
            var listCustom = new DetailsListView();

            var cnt = this.statuses.Tabs.Count;

            // ToDo:Create and set controls follow tabtypes

            using (ControlTransaction.Update(listCustom))
            using (ControlTransaction.Layout(this.SplitContainer1.Panel1, false))
            using (ControlTransaction.Layout(this.SplitContainer1.Panel2, false))
            using (ControlTransaction.Layout(this.SplitContainer1, false))
            using (ControlTransaction.Layout(this.ListTab, false))
            using (ControlTransaction.Layout(this))
            using (ControlTransaction.Layout(tabPage, false))
            {
                tabPage.Controls.Add(listCustom);

                // UserTimeline関連
                var userTab = tab as UserTimelineTabModel;
                var listTab = tab as ListTimelineTabModel;
                var searchTab = tab as PublicSearchTabModel;

                if (userTab != null || listTab != null)
                {
                    var label = new Label
                    {
                        Dock = DockStyle.Top,
                        Name = "labelUser",
                        TabIndex = 0,
                    };

                    if (listTab != null)
                    {
                        label.Text = listTab.ListInfo.ToString();
                    }
                    else if (userTab != null)
                    {
                        label.Text = userTab.ScreenName + "'s Timeline";
                    }
                    label.TextAlign = ContentAlignment.MiddleLeft;
                    using (var tmpComboBox = new ComboBox())
                    {
                        label.Height = tmpComboBox.Height;
                    }
                    tabPage.Controls.Add(label);
                }
                // 検索関連の準備
                else if (searchTab != null)
                {
                    var pnl = new Panel();

                    var lbl = new Label();
                    var cmb = new ComboBox();
                    var btn = new Button();
                    var cmbLang = new ComboBox();

                    using (ControlTransaction.Layout(pnl, false))
                    {
                        pnl.Controls.Add(cmb);
                        pnl.Controls.Add(cmbLang);
                        pnl.Controls.Add(btn);
                        pnl.Controls.Add(lbl);
                        pnl.Name = "panelSearch";
                        pnl.TabIndex = 0;
                        pnl.Dock = DockStyle.Top;
                        pnl.Height = cmb.Height;
                        pnl.Enter += this.SearchControls_Enter;
                        pnl.Leave += this.SearchControls_Leave;

                        cmb.Text = "";
                        cmb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                        cmb.Dock = DockStyle.Fill;
                        cmb.Name = "comboSearch";
                        cmb.DropDownStyle = ComboBoxStyle.DropDown;
                        cmb.ImeMode = ImeMode.NoControl;
                        cmb.TabStop = false;
                        cmb.TabIndex = 1;
                        cmb.AutoCompleteMode = AutoCompleteMode.None;
                        cmb.KeyDown += this.SearchComboBox_KeyDown;

                        cmbLang.Text = "";
                        cmbLang.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                        cmbLang.Dock = DockStyle.Right;
                        cmbLang.Width = 50;
                        cmbLang.Name = "comboLang";
                        cmbLang.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbLang.TabStop = false;
                        cmbLang.TabIndex = 2;
                        cmbLang.Items.Add("");
                        cmbLang.Items.Add("ja");
                        cmbLang.Items.Add("en");
                        cmbLang.Items.Add("ar");
                        cmbLang.Items.Add("da");
                        cmbLang.Items.Add("nl");
                        cmbLang.Items.Add("fa");
                        cmbLang.Items.Add("fi");
                        cmbLang.Items.Add("fr");
                        cmbLang.Items.Add("de");
                        cmbLang.Items.Add("hu");
                        cmbLang.Items.Add("is");
                        cmbLang.Items.Add("it");
                        cmbLang.Items.Add("no");
                        cmbLang.Items.Add("pl");
                        cmbLang.Items.Add("pt");
                        cmbLang.Items.Add("ru");
                        cmbLang.Items.Add("es");
                        cmbLang.Items.Add("sv");
                        cmbLang.Items.Add("th");

                        lbl.Text = "Search(C-S-f)";
                        lbl.Name = "label1";
                        lbl.Dock = DockStyle.Left;
                        lbl.Width = 90;
                        lbl.Height = cmb.Height;
                        lbl.TextAlign = ContentAlignment.MiddleLeft;
                        lbl.TabIndex = 0;

                        btn.Text = "Search";
                        btn.Name = "buttonSearch";
                        btn.UseVisualStyleBackColor = true;
                        btn.Dock = DockStyle.Right;
                        btn.TabStop = false;
                        btn.TabIndex = 3;
                        btn.Click += this.SearchButton_Click;

                        if (!MyCommon.IsNullOrEmpty(searchTab.SearchWords))
                        {
                            cmb.Items.Add(searchTab.SearchWords);
                            cmb.Text = searchTab.SearchWords;
                        }

                        cmbLang.Text = searchTab.SearchLang;

                        tabPage.Controls.Add(pnl);
                    }
                }

                tabPage.Tag = listCustom;
                this.ListTab.Controls.Add(tabPage);

                tabPage.Location = new Point(4, 4);
                tabPage.Name = "CTab" + cnt;
                tabPage.Size = new Size(380, 260);
                tabPage.TabIndex = 2 + cnt;
                tabPage.Text = tab.TabName;
                tabPage.UseVisualStyleBackColor = true;
                tabPage.AccessibleRole = AccessibleRole.PageTab;

                listCustom.AccessibleName = Properties.Resources.AddNewTab_ListView_AccessibleName;
                listCustom.TabIndex = 1;
                listCustom.AllowColumnReorder = true;
                listCustom.ContextMenuStrip = this.ContextMenuOperate;
                listCustom.ColumnHeaderContextMenuStrip = this.ContextMenuColumnHeader;
                listCustom.Dock = DockStyle.Fill;
                listCustom.FullRowSelect = true;
                listCustom.HideSelection = false;
                listCustom.Location = new Point(0, 0);
                listCustom.Margin = new Padding(0);
                listCustom.Name = "CList" + Environment.TickCount;
                listCustom.ShowItemToolTips = true;
                listCustom.Size = new Size(380, 260);
                listCustom.UseCompatibleStateImageBehavior = false;
                listCustom.View = View.Details;
                listCustom.OwnerDraw = true;
                listCustom.VirtualMode = true;

                listCustom.GridLines = this.settings.Common.ShowGrid;
                listCustom.AllowDrop = true;

                this.InitColumns(listCustom, startup);

                listCustom.SelectedIndexChanged += this.MyList_SelectedIndexChanged;
                listCustom.MouseDoubleClick += this.MyList_MouseDoubleClick;
                listCustom.ColumnClick += this.MyList_ColumnClick;
                listCustom.DrawColumnHeader += this.MyList_DrawColumnHeader;
                listCustom.DragDrop += this.TweenMain_DragDrop;
                listCustom.DragEnter += this.TweenMain_DragEnter;
                listCustom.DragOver += this.TweenMain_DragOver;
                listCustom.MouseClick += this.MyList_MouseClick;
                listCustom.ColumnReordered += this.MyList_ColumnReordered;
                listCustom.ColumnWidthChanged += this.MyList_ColumnWidthChanged;
                listCustom.HScrolled += this.MyList_HScrolled;
            }

            var state = new TimelineListViewState(listCustom, tab);
            this.listViewState[tab.TabName] = state;

            return true;
        }

        public bool RemoveSpecifiedTab(string tabName, bool confirm)
        {
            var tabInfo = this.statuses.GetTabByName(tabName);
            if (tabInfo == null || tabInfo.IsDefaultTabType || tabInfo.Protected)
                return false;

            if (confirm)
            {
                var tmp = string.Format(Properties.Resources.RemoveSpecifiedTabText1, Environment.NewLine);
                var result = MessageBox.Show(
                    tmp,
                    tabName + " " + Properties.Resources.RemoveSpecifiedTabText2,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }

            var tabIndex = this.statuses.Tabs.IndexOf(tabName);
            if (tabIndex == -1)
                return false;

            var tabPage = this.ListTab.TabPages[tabIndex];

            this.SetListProperty();   // 他のタブに列幅等を反映

            this.listViewState.Remove(tabName);

            // オブジェクトインスタンスの削除
            var listCustom = (DetailsListView)tabPage.Tag;
            tabPage.Tag = null;

            using (ControlTransaction.Layout(this.SplitContainer1.Panel1, false))
            using (ControlTransaction.Layout(this.SplitContainer1.Panel2, false))
            using (ControlTransaction.Layout(this.SplitContainer1, false))
            using (ControlTransaction.Layout(this.ListTab, false))
            using (ControlTransaction.Layout(this))
            using (ControlTransaction.Layout(tabPage, false))
            {
                if (this.CurrentTabName == tabName)
                {
                    this.ListTab.SelectTab((this.beforeSelectedTab != null && this.ListTab.TabPages.Contains(this.beforeSelectedTab)) ? this.beforeSelectedTab : this.ListTab.TabPages[0]);
                    this.beforeSelectedTab = null;
                }
                this.ListTab.Controls.Remove(tabPage);

                // 後付けのコントロールを破棄
                if (tabInfo.TabType == MyCommon.TabUsageType.UserTimeline || tabInfo.TabType == MyCommon.TabUsageType.Lists)
                {
                    using var label = tabPage.Controls["labelUser"];
                    tabPage.Controls.Remove(label);
                }
                else if (tabInfo.TabType == MyCommon.TabUsageType.PublicSearch)
                {
                    using var pnl = tabPage.Controls["panelSearch"];

                    pnl.Enter -= this.SearchControls_Enter;
                    pnl.Leave -= this.SearchControls_Leave;
                    tabPage.Controls.Remove(pnl);

                    foreach (Control ctrl in pnl.Controls)
                    {
                        if (ctrl.Name == "buttonSearch")
                        {
                            ctrl.Click -= this.SearchButton_Click;
                        }
                        else if (ctrl.Name == "comboSearch")
                        {
                            ctrl.KeyDown -= this.SearchComboBox_KeyDown;
                        }
                        pnl.Controls.Remove(ctrl);
                        ctrl.Dispose();
                    }
                }

                tabPage.Controls.Remove(listCustom);

                listCustom.SelectedIndexChanged -= this.MyList_SelectedIndexChanged;
                listCustom.MouseDoubleClick -= this.MyList_MouseDoubleClick;
                listCustom.ColumnClick -= this.MyList_ColumnClick;
                listCustom.DrawColumnHeader -= this.MyList_DrawColumnHeader;
                listCustom.DragDrop -= this.TweenMain_DragDrop;
                listCustom.DragEnter -= this.TweenMain_DragEnter;
                listCustom.DragOver -= this.TweenMain_DragOver;
                listCustom.MouseClick -= this.MyList_MouseClick;
                listCustom.ColumnReordered -= this.MyList_ColumnReordered;
                listCustom.ColumnWidthChanged -= this.MyList_ColumnWidthChanged;
                listCustom.HScrolled -= this.MyList_HScrolled;

                var cols = listCustom.Columns.Cast<ColumnHeader>().ToList<ColumnHeader>();
                listCustom.Columns.Clear();
                cols.ForEach(col => col.Dispose());
                cols.Clear();

                listCustom.ContextMenuStrip = null;
                listCustom.ColumnHeaderContextMenuStrip = null;
                listCustom.Font = null;

                listCustom.SmallImageList = null;
                listCustom.ListViewItemSorter = null;

                // キャッシュのクリア
                this.listCache?.PurgeCache();
            }

            tabPage.Dispose();
            listCustom.Dispose();
            this.statuses.RemoveTab(tabName);

            return true;
        }

        private void ListTab_Deselected(object sender, TabControlEventArgs e)
        {
            this.listCache?.PurgeCache();
            this.beforeSelectedTab = e.TabPage;
        }

        private void ListTab_MouseMove(object sender, MouseEventArgs e)
        {
            // タブのD&D

            if (!this.settings.Common.TabMouseLock && e.Button == MouseButtons.Left && this.tabDrag)
            {
                var tn = "";
                var dragEnableRectangle = new Rectangle(this.tabMouseDownPoint.X - (SystemInformation.DragSize.Width / 2), this.tabMouseDownPoint.Y - (SystemInformation.DragSize.Height / 2), SystemInformation.DragSize.Width, SystemInformation.DragSize.Height);
                if (!dragEnableRectangle.Contains(e.Location))
                {
                    // タブが多段の場合にはMouseDownの前の段階で選択されたタブの段が変わっているので、このタイミングでカーソルの位置からタブを判定出来ない。
                    tn = this.CurrentTabName;
                }

                if (MyCommon.IsNullOrEmpty(tn)) return;

                var tabIndex = this.statuses.Tabs.IndexOf(tn);
                if (tabIndex != -1)
                {
                    var tabPage = this.ListTab.TabPages[tabIndex];
                    this.ListTab.DoDragDrop(tabPage, DragDropEffects.All);
                }
            }
            else
            {
                this.tabDrag = false;
            }

            var cpos = new Point(e.X, e.Y);
            foreach (var (tab, index) in this.statuses.Tabs.WithIndex())
            {
                var rect = this.ListTab.GetTabRect(index);
                if (rect.Contains(cpos))
                {
                    this.rclickTabName = tab.TabName;
                    break;
                }
            }
        }

        private void ListTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetMainWindowTitle();
            this.SetStatusLabelUrl();
            this.SetApiStatusLabel();
            if (this.ListTab.Focused || ((Control)this.CurrentTabPage.Tag).Focused)
                this.Tag = this.ListTab.Tag;
            this.TabMenuControl(this.CurrentTabName);
            this.PushSelectPostChain();
            this.DispSelectedPost();
        }

        private void SetListProperty()
        {
            if (!this.isColumnChanged) return;

            var currentListView = this.CurrentListView;

            var dispOrder = new int[currentListView.Columns.Count];
            for (var i = 0; i < currentListView.Columns.Count; i++)
            {
                for (var j = 0; j < currentListView.Columns.Count; j++)
                {
                    if (currentListView.Columns[j].DisplayIndex == i)
                    {
                        dispOrder[i] = j;
                        break;
                    }
                }
            }

            // 列幅、列並びを他のタブに設定
            foreach (TabPage tb in this.ListTab.TabPages)
            {
                if (tb.Text == this.CurrentTabName)
                    continue;

                if (tb.Tag != null && tb.Controls.Count > 0)
                {
                    var lst = (DetailsListView)tb.Tag;
                    for (var i = 0; i < lst.Columns.Count; i++)
                    {
                        lst.Columns[dispOrder[i]].DisplayIndex = i;
                        lst.Columns[i].Width = currentListView.Columns[i].Width;
                    }
                }
            }

            this.isColumnChanged = false;
        }

        private void StatusText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '@')
            {
                if (!this.settings.Common.UseAtIdSupplement) return;
                // @マーク
                var cnt = this.AtIdSupl.ItemCount;
                this.ShowSuplDialog(this.StatusText, this.AtIdSupl);
                if (cnt != this.AtIdSupl.ItemCount)
                    this.MarkSettingAtIdModified();
                e.Handled = true;
            }
            else if (e.KeyChar == '#')
            {
                if (!this.settings.Common.UseHashSupplement) return;
                this.ShowSuplDialog(this.StatusText, this.HashSupl);
                e.Handled = true;
            }
        }

        public void ShowSuplDialog(TextBox owner, AtIdSupplement dialog)
            => this.ShowSuplDialog(owner, dialog, 0, "");

        public void ShowSuplDialog(TextBox owner, AtIdSupplement dialog, int offset)
            => this.ShowSuplDialog(owner, dialog, offset, "");

        public void ShowSuplDialog(TextBox owner, AtIdSupplement dialog, int offset, string startswith)
        {
            dialog.StartsWith = startswith;
            if (dialog.Visible)
            {
                dialog.Focus();
            }
            else
            {
                dialog.ShowDialog();
            }
            this.TopMost = this.settings.Common.AlwaysTop;
            var selStart = owner.SelectionStart;
            var fHalf = "";
            var eHalf = "";
            if (dialog.DialogResult == DialogResult.OK)
            {
                if (!MyCommon.IsNullOrEmpty(dialog.InputText))
                {
                    if (selStart > 0)
                    {
                        fHalf = owner.Text.Substring(0, selStart - offset);
                    }
                    if (selStart < owner.Text.Length)
                    {
                        eHalf = owner.Text.Substring(selStart);
                    }
                    owner.Text = fHalf + dialog.InputText + eHalf;
                    owner.SelectionStart = selStart + dialog.InputText.Length;
                }
            }
            else
            {
                if (selStart > 0)
                {
                    fHalf = owner.Text.Substring(0, selStart);
                }
                if (selStart < owner.Text.Length)
                {
                    eHalf = owner.Text.Substring(selStart);
                }
                owner.Text = fHalf + eHalf;
                if (selStart > 0)
                {
                    owner.SelectionStart = selStart;
                }
            }
            owner.Focus();
        }

        private void StatusText_KeyUp(object sender, KeyEventArgs e)
        {
            // スペースキーで未読ジャンプ
            if (!e.Alt && !e.Control && !e.Shift)
            {
                if (e.KeyCode == Keys.Space || e.KeyCode == Keys.ProcessKey)
                {
                    var isSpace = false;
                    foreach (var c in this.StatusText.Text)
                    {
                        if (c == ' ' || c == '　')
                        {
                            isSpace = true;
                        }
                        else
                        {
                            isSpace = false;
                            break;
                        }
                    }
                    if (isSpace)
                    {
                        e.Handled = true;
                        this.StatusText.Text = "";
                        this.JumpUnreadMenuItem_Click(this.JumpUnreadMenuItem, EventArgs.Empty);
                    }
                }
            }
            this.StatusText_TextChanged(this.StatusText, EventArgs.Empty);
        }

        private void StatusText_TextChanged(object sender, EventArgs e)
        {
            // 文字数カウント
            var pLen = this.GetRestStatusCount(this.FormatStatusTextExtended(this.StatusText.Text));
            this.lblLen.Text = pLen.ToString();
            if (pLen < 0)
            {
                this.StatusText.ForeColor = Color.Red;
            }
            else
            {
                this.StatusText.ForeColor = this.themeManager.ColorInputFont;
            }

            this.StatusText.AccessibleDescription = string.Format(Properties.Resources.StatusText_AccessibleDescription, pLen);

            if (MyCommon.IsNullOrEmpty(this.StatusText.Text))
            {
                this.inReplyTo = null;
            }
        }

        /// <summary>
        /// メンション以外の文字列が含まれていないテキストであるか判定します
        /// </summary>
        internal static bool TextContainsOnlyMentions(string text)
        {
            var mentions = TweetExtractor.ExtractMentionEntities(text).OrderBy(x => x.Indices[0]);
            var startIndex = 0;

            foreach (var mention in mentions)
            {
                var textPart = text.Substring(startIndex, mention.Indices[0] - startIndex);

                if (!string.IsNullOrWhiteSpace(textPart))
                    return false;

                startIndex = mention.Indices[1];
            }

            var textPartLast = text.Substring(startIndex);

            if (!string.IsNullOrWhiteSpace(textPartLast))
                return false;

            return true;
        }

        /// <summary>
        /// 投稿時に auto_populate_reply_metadata オプションによって自動で追加されるメンションを除去します
        /// </summary>
        private string RemoveAutoPopuratedMentions(string statusText, out long[] autoPopulatedUserIds)
        {
            var autoPopulatedUserIdList = new List<long>();

            var replyToPost = this.inReplyTo != null ? this.statuses[this.inReplyTo.Value.StatusId] : null;
            if (replyToPost != null)
            {
                if (statusText.StartsWith($"@{replyToPost.ScreenName} ", StringComparison.Ordinal))
                {
                    statusText = statusText.Substring(replyToPost.ScreenName.Length + 2);
                    autoPopulatedUserIdList.Add(replyToPost.UserId);

                    foreach (var (userId, screenName) in replyToPost.ReplyToList)
                    {
                        if (statusText.StartsWith($"@{screenName} ", StringComparison.Ordinal))
                        {
                            statusText = statusText.Substring(screenName.Length + 2);
                            autoPopulatedUserIdList.Add(userId);
                        }
                    }
                }
            }

            autoPopulatedUserIds = autoPopulatedUserIdList.ToArray();

            return statusText;
        }

        /// <summary>
        /// attachment_url に指定可能な URL が含まれていれば除去
        /// </summary>
        private string RemoveAttachmentUrl(string statusText, out string? attachmentUrl)
        {
            attachmentUrl = null;

            // attachment_url は media_id と同時に使用できない
            if (this.ImageSelector.Visible && this.ImageSelector.Model.SelectedMediaService is TwitterPhoto)
                return statusText;

            var match = Twitter.AttachmentUrlRegex.Match(statusText);
            if (!match.Success)
                return statusText;

            attachmentUrl = match.Value;

            // マッチした URL を空白に置換
            statusText = statusText.Substring(0, match.Index);

            // テキストと URL の間にスペースが含まれていれば除去
            return statusText.TrimEnd(' ');
        }

        private string FormatStatusTextExtended(string statusText)
            => this.FormatStatusTextExtended(statusText, out _, out _);

        /// <summary>
        /// <see cref="FormatStatusText"/> に加えて、拡張モードで140字にカウントされない文字列の除去を行います
        /// </summary>
        private string FormatStatusTextExtended(string statusText, out long[] autoPopulatedUserIds, out string? attachmentUrl)
        {
            statusText = this.RemoveAutoPopuratedMentions(statusText, out autoPopulatedUserIds);

            statusText = this.RemoveAttachmentUrl(statusText, out attachmentUrl);

            return this.FormatStatusText(statusText);
        }

        /// <summary>
        /// ツイート投稿前のフッター付与などの前処理を行います
        /// </summary>
        private string FormatStatusText(string statusText)
        {
            statusText = statusText.Replace("\r\n", "\n");

            if (this.urlMultibyteSplit)
            {
                // URLと全角文字の切り離し
                statusText = Regex.Replace(statusText, @"https?:\/\/[-_.!~*'()a-zA-Z0-9;\/?:\@&=+\$,%#^]+", "$& ");
            }

            if (this.settings.Common.WideSpaceConvert)
            {
                // 文中の全角スペースを半角スペース1個にする
                statusText = statusText.Replace("　", " ");
            }

            // DM の場合はこれ以降の処理を行わない
            if (statusText.StartsWith("D ", StringComparison.OrdinalIgnoreCase))
                return statusText;

            bool disableFooter;
            if (this.settings.Common.PostShiftEnter)
            {
                disableFooter = MyCommon.IsKeyDown(Keys.Control);
            }
            else
            {
                if (this.StatusText.Multiline && !this.settings.Common.PostCtrlEnter)
                    disableFooter = MyCommon.IsKeyDown(Keys.Control);
                else
                    disableFooter = MyCommon.IsKeyDown(Keys.Shift);
            }

            if (statusText.Contains("RT @"))
                disableFooter = true;

            // 自分宛のリプライの場合は先頭の「@screen_name 」の部分を除去する (in_reply_to_status_id は維持される)
            if (this.inReplyTo != null && this.inReplyTo.Value.ScreenName == this.tw.Username)
            {
                var mentionSelf = $"@{this.tw.Username} ";
                if (statusText.StartsWith(mentionSelf, StringComparison.OrdinalIgnoreCase))
                {
                    if (statusText.Length > mentionSelf.Length || this.GetSelectedImageService() != null)
                        statusText = statusText.Substring(mentionSelf.Length);
                }
            }

            var header = "";
            var footer = "";

            var hashtag = this.HashMgr.UseHash;
            if (!MyCommon.IsNullOrEmpty(hashtag) && !(this.HashMgr.IsNotAddToAtReply && this.inReplyTo != null))
            {
                if (this.HashMgr.IsHead)
                    header = this.HashMgr.UseHash + " ";
                else
                    footer = " " + this.HashMgr.UseHash;
            }

            if (!disableFooter)
            {
                if (this.settings.Local.UseRecommendStatus)
                {
                    // 推奨ステータスを使用する
                    footer += this.recommendedStatusFooter;
                }
                else if (!MyCommon.IsNullOrEmpty(this.settings.Local.StatusText))
                {
                    // テキストボックスに入力されている文字列を使用する
                    footer += " " + this.settings.Local.StatusText.Trim();
                }
            }

            statusText = header + statusText + footer;

            if (this.preventSmsCommand)
            {
                // ツイートが意図せず SMS コマンドとして解釈されることを回避 (D, DM, M のみ)
                // 参照: https://support.twitter.com/articles/14020

                if (Regex.IsMatch(statusText, @"^[+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]*(d|dm|m)([+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]+|$)", RegexOptions.IgnoreCase)
                    && !Twitter.DMSendTextRegex.IsMatch(statusText))
                {
                    // U+200B (ZERO WIDTH SPACE) を先頭に加えて回避
                    statusText = '\u200b' + statusText;
                }
            }

            return statusText;
        }

        /// <summary>
        /// 投稿欄に表示する入力可能な文字数を計算します
        /// </summary>
        private int GetRestStatusCount(string statusText)
        {
            var remainCount = this.tw.GetTextLengthRemain(statusText);

            var uploadService = this.GetSelectedImageService();
            if (uploadService != null)
            {
                // TODO: ImageSelector で選択中の画像の枚数が mediaCount 引数に渡るようにする
                remainCount -= uploadService.GetReservedTextLength(1);
            }

            return remainCount;
        }

        private IMediaUploadService? GetSelectedImageService()
            => this.ImageSelector.Visible ? this.ImageSelector.Model.SelectedMediaService : null;

        /// <summary>
        /// 全てのタブの振り分けルールを反映し直します
        /// </summary>
        private void ApplyPostFilters()
        {
            using (ControlTransaction.Cursor(this, Cursors.WaitCursor))
            {
                this.statuses.FilterAll();

                var listView = this.CurrentListView;
                using (ControlTransaction.Update(listView))
                {
                    this.listCache?.PurgeCache();
                    this.listCache?.UpdateListSize();
                }

                foreach (var (tab, index) in this.statuses.Tabs.WithIndex())
                {
                    var tabPage = this.ListTab.TabPages[index];

                    if (this.settings.Common.TabIconDisp)
                    {
                        if (tab.UnreadCount > 0)
                            tabPage.ImageIndex = 0;
                        else
                            tabPage.ImageIndex = -1;
                    }
                }

                if (!this.settings.Common.TabIconDisp)
                    this.ListTab.Refresh();

                this.SetMainWindowTitle();
                this.SetStatusLabelUrl();
            }
        }

        private void MyList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
            => e.DrawDefault = true;

        private void MyList_HScrolled(object sender, EventArgs e)
            => ((DetailsListView)sender).Refresh();

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            ScaleChildControl(this.TabImage, factor);

            var tabpages = this.ListTab.TabPages.Cast<TabPage>();
            var listviews = tabpages.Select(x => x.Tag).Cast<ListView>();

            foreach (var listview in listviews)
            {
                ScaleChildControl(listview, factor);
            }
        }

        internal void DoTabSearch(string searchWord, bool caseSensitive, bool useRegex, SEARCHTYPE searchType)
        {
            var tab = this.CurrentTab;

            if (tab.AllCount == 0)
            {
                MessageBox.Show(Properties.Resources.DoTabSearchText2, Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedIndex = tab.SelectedIndex;

            int startIndex;
            switch (searchType)
            {
                case SEARCHTYPE.NextSearch: // 次を検索
                    if (selectedIndex != -1)
                        startIndex = Math.Min(selectedIndex + 1, tab.AllCount - 1);
                    else
                        startIndex = 0;
                    break;
                case SEARCHTYPE.PrevSearch: // 前を検索
                    if (selectedIndex != -1)
                        startIndex = Math.Max(selectedIndex - 1, 0);
                    else
                        startIndex = tab.AllCount - 1;
                    break;
                case SEARCHTYPE.DialogSearch: // ダイアログからの検索
                default:
                    if (selectedIndex != -1)
                        startIndex = selectedIndex;
                    else
                        startIndex = 0;
                    break;
            }

            Func<string, bool> stringComparer;
            try
            {
                stringComparer = this.CreateSearchComparer(searchWord, useRegex, caseSensitive);
            }
            catch (ArgumentException)
            {
                MessageBox.Show(Properties.Resources.DoTabSearchText1, Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var reverse = searchType == SEARCHTYPE.PrevSearch;
            var foundIndex = tab.SearchPostsAll(stringComparer, startIndex, reverse)
                .DefaultIfEmpty(-1).First();

            if (foundIndex == -1)
            {
                MessageBox.Show(Properties.Resources.DoTabSearchText2, Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var listView = this.CurrentListView;
            this.SelectListItem(listView, foundIndex);
            listView.EnsureVisible(foundIndex);
        }

        private void MenuItemSubSearch_Click(object sender, EventArgs e)
            => this.ShowSearchDialog(); // 検索メニュー

        private void MenuItemSearchNext_Click(object sender, EventArgs e)
        {
            var previousSearch = this.SearchDialog.ResultOptions;
            if (previousSearch == null || previousSearch.Type != SearchWordDialog.SearchType.Timeline)
            {
                this.SearchDialog.Reset();
                this.ShowSearchDialog();
                return;
            }

            // 次を検索
            this.DoTabSearch(
                previousSearch.Query,
                previousSearch.CaseSensitive,
                previousSearch.UseRegex,
                SEARCHTYPE.NextSearch);
        }

        private void MenuItemSearchPrev_Click(object sender, EventArgs e)
        {
            var previousSearch = this.SearchDialog.ResultOptions;
            if (previousSearch == null || previousSearch.Type != SearchWordDialog.SearchType.Timeline)
            {
                this.SearchDialog.Reset();
                this.ShowSearchDialog();
                return;
            }

            // 前を検索
            this.DoTabSearch(
                previousSearch.Query,
                previousSearch.CaseSensitive,
                previousSearch.UseRegex,
                SEARCHTYPE.PrevSearch);
        }

        /// <summary>
        /// 検索ダイアログを表示し、検索を実行します
        /// </summary>
        private void ShowSearchDialog()
        {
            if (this.SearchDialog.ShowDialog(this) != DialogResult.OK)
            {
                this.TopMost = this.settings.Common.AlwaysTop;
                return;
            }
            this.TopMost = this.settings.Common.AlwaysTop;

            var searchOptions = this.SearchDialog.ResultOptions!;
            if (searchOptions.Type == SearchWordDialog.SearchType.Timeline)
            {
                if (searchOptions.NewTab)
                {
                    var tabName = Properties.Resources.SearchResults_TabName;

                    try
                    {
                        tabName = this.statuses.MakeTabName(tabName);
                    }
                    catch (TabException ex)
                    {
                        MessageBox.Show(this, ex.Message, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    var resultTab = new LocalSearchTabModel(tabName);
                    this.AddNewTab(resultTab, startup: false);
                    this.statuses.AddTab(resultTab);

                    var targetTab = this.CurrentTab;

                    Func<string, bool> stringComparer;
                    try
                    {
                        stringComparer = this.CreateSearchComparer(searchOptions.Query, searchOptions.UseRegex, searchOptions.CaseSensitive);
                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show(Properties.Resources.DoTabSearchText1, Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var foundIndices = targetTab.SearchPostsAll(stringComparer).ToArray();
                    if (foundIndices.Length == 0)
                    {
                        MessageBox.Show(Properties.Resources.DoTabSearchText2, Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var foundPosts = foundIndices.Select(x => targetTab[x]);
                    foreach (var post in foundPosts)
                    {
                        resultTab.AddPostQueue(post);
                    }

                    this.statuses.DistributePosts();
                    this.RefreshTimeline();

                    var tabIndex = this.statuses.Tabs.IndexOf(tabName);
                    this.ListTab.SelectedIndex = tabIndex;
                }
                else
                {
                    this.DoTabSearch(
                        searchOptions.Query,
                        searchOptions.CaseSensitive,
                        searchOptions.UseRegex,
                        SEARCHTYPE.DialogSearch);
                }
            }
            else if (searchOptions.Type == SearchWordDialog.SearchType.Public)
            {
                this.AddNewTabForSearch(searchOptions.Query);
            }
        }

        /// <summary>発言検索に使用するメソッドを生成します</summary>
        /// <exception cref="ArgumentException">
        /// <paramref name="useRegex"/> が true かつ、<paramref name="query"/> が不正な正規表現な場合
        /// </exception>
        private Func<string, bool> CreateSearchComparer(string query, bool useRegex, bool caseSensitive)
        {
            if (useRegex)
            {
                var regexOption = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                var regex = new Regex(query, regexOption);

                return x => regex.IsMatch(x);
            }
            else
            {
                var comparisonType = caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

                return x => x.IndexOf(query, comparisonType) != -1;
            }
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            using (var about = new TweenAboutBox())
            {
                about.ShowDialog(this);
            }
            this.TopMost = this.settings.Common.AlwaysTop;
        }

        private void JumpUnreadMenuItem_Click(object sender, EventArgs e)
        {
            var bgnIdx = this.statuses.SelectedTabIndex;

            if (this.ImageSelector.Enabled)
                return;

            TabModel? foundTab = null;
            var foundIndex = 0;

            // 現在タブから最終タブまで探索
            foreach (var (tab, index) in this.statuses.Tabs.WithIndex().Skip(bgnIdx))
            {
                var unreadIndex = tab.NextUnreadIndex;
                if (unreadIndex != -1)
                {
                    this.ListTab.SelectedIndex = index;
                    foundTab = tab;
                    foundIndex = unreadIndex;
                    break;
                }
            }

            // 未読みつからず＆現在タブが先頭ではなかったら、先頭タブから現在タブの手前まで探索
            if (foundTab == null && bgnIdx > 0)
            {
                foreach (var (tab, index) in this.statuses.Tabs.WithIndex().Take(bgnIdx))
                {
                    var unreadIndex = tab.NextUnreadIndex;
                    if (unreadIndex != -1)
                    {
                        this.ListTab.SelectedIndex = index;
                        foundTab = tab;
                        foundIndex = unreadIndex;
                        break;
                    }
                }
            }

            DetailsListView lst;

            if (foundTab == null)
            {
                // 全部調べたが未読見つからず→先頭タブの最新発言へ
                this.ListTab.SelectedIndex = 0;
                var tabPage = this.ListTab.TabPages[0];
                var tab = this.statuses.Tabs[0];

                if (tab.AllCount == 0)
                    return;

                if (this.statuses.SortOrder == SortOrder.Ascending)
                    foundIndex = tab.AllCount - 1;
                else
                    foundIndex = 0;

                lst = (DetailsListView)tabPage.Tag;
            }
            else
            {
                var foundTabIndex = this.statuses.Tabs.IndexOf(foundTab);
                lst = (DetailsListView)this.ListTab.TabPages[foundTabIndex].Tag;
            }

            this.SelectListItem(lst, foundIndex);

            if (this.statuses.SortMode == ComparerMode.Id)
            {
                var rowHeight = lst.SmallImageList.ImageSize.Height;
                if (this.statuses.SortOrder == SortOrder.Ascending && lst.Items[foundIndex].Position.Y > lst.ClientSize.Height - rowHeight - 10 ||
                    this.statuses.SortOrder == SortOrder.Descending && lst.Items[foundIndex].Position.Y < rowHeight + 10)
                {
                    this.MoveTop();
                }
                else
                {
                    lst.EnsureVisible(foundIndex);
                }
            }
            else
            {
                lst.EnsureVisible(foundIndex);
            }

            lst.Focus();
        }

        private async void StatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            var tab = this.CurrentTab;
            var post = this.CurrentPost;
            if (post != null && tab.TabType != MyCommon.TabUsageType.DirectMessage)
                await MyCommon.OpenInBrowserAsync(this, MyCommon.GetStatusUrl(post));
        }

        private async void VerUpMenuItem_Click(object sender, EventArgs e)
            => await this.CheckNewVersion(false);

        private void RunTweenUp()
        {
            var pinfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = this.settings.SettingsPath,
                FileName = Path.Combine(this.settings.SettingsPath, "TweenUp3.exe"),
                Arguments = "\"" + Application.StartupPath + "\"",
            };

            try
            {
                Process.Start(pinfo);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to execute TweenUp3.exe.");
            }
        }

        public readonly record struct VersionInfo(
            Version Version,
            Uri DownloadUri,
            string ReleaseNote
        );

        /// <summary>
        /// OpenTween の最新バージョンの情報を取得します
        /// </summary>
        public async Task<VersionInfo> GetVersionInfoAsync()
        {
            var versionInfoUrl = new Uri(ApplicationSettings.VersionInfoUrl + "?" +
                DateTimeUtc.Now.ToString("yyMMddHHmmss") + Environment.TickCount);

            var responseText = await Networking.Http.GetStringAsync(versionInfoUrl)
                .ConfigureAwait(false);

            // 改行2つで前後パートを分割（前半がバージョン番号など、後半が詳細テキスト）
            var msgPart = responseText.Split(new[] { "\n\n", "\r\n\r\n" }, 2, StringSplitOptions.None);

            var msgHeader = msgPart[0].Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
            var msgBody = msgPart.Length == 2 ? msgPart[1] : "";

            msgBody = Regex.Replace(msgBody, "(?<!\r)\n", "\r\n"); // LF -> CRLF

            return new VersionInfo
            {
                Version = Version.Parse(msgHeader[0]),
                DownloadUri = new Uri(msgHeader[1]),
                ReleaseNote = msgBody,
            };
        }

        private async Task CheckNewVersion(bool startup = false)
        {
            if (ApplicationSettings.VersionInfoUrl == null)
                return; // 更新チェック無効化

            try
            {
                var versionInfo = await this.GetVersionInfoAsync();

                if (versionInfo.Version <= Version.Parse(MyCommon.FileVersion))
                {
                    // 更新不要
                    if (!startup)
                    {
                        var msgtext = string.Format(
                            Properties.Resources.CheckNewVersionText7,
                            MyCommon.GetReadableVersion(),
                            MyCommon.GetReadableVersion(versionInfo.Version));
                        msgtext = MyCommon.ReplaceAppName(msgtext);

                        MessageBox.Show(
                            msgtext,
                            MyCommon.ReplaceAppName(Properties.Resources.CheckNewVersionText2),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    return;
                }

                if (startup && versionInfo.Version <= this.settings.Common.SkipUpdateVersion)
                    return;

                using var dialog = new UpdateDialog();

                dialog.SummaryText = string.Format(Properties.Resources.CheckNewVersionText3,
                    MyCommon.GetReadableVersion(versionInfo.Version));
                dialog.DetailsText = versionInfo.ReleaseNote;

                if (dialog.ShowDialog(this) == DialogResult.Yes)
                {
                    await MyCommon.OpenInBrowserAsync(this, versionInfo.DownloadUri);
                }
                else if (dialog.SkipButtonPressed)
                {
                    this.settings.Common.SkipUpdateVersion = versionInfo.Version;
                    this.MarkSettingCommonModified();
                }
            }
            catch (Exception)
            {
                this.StatusLabel.Text = Properties.Resources.CheckNewVersionText9;
                if (!startup)
                {
                    MessageBox.Show(
                        Properties.Resources.CheckNewVersionText10,
                        MyCommon.ReplaceAppName(Properties.Resources.CheckNewVersionText2),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button2);
                }
            }
        }

        private void UpdateSelectedPost()
        {
            // 件数関連の場合、タイトル即時書き換え
            if (this.settings.Common.DispLatestPost != MyCommon.DispTitleEnum.None &&
               this.settings.Common.DispLatestPost != MyCommon.DispTitleEnum.Post &&
               this.settings.Common.DispLatestPost != MyCommon.DispTitleEnum.Ver &&
               this.settings.Common.DispLatestPost != MyCommon.DispTitleEnum.OwnStatus)
            {
                this.SetMainWindowTitle();
            }
            if (!this.StatusLabelUrl.Text.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                this.SetStatusLabelUrl();

            if (this.settings.Common.TabIconDisp)
            {
                foreach (var (tab, index) in this.statuses.Tabs.WithIndex())
                {
                    if (tab.UnreadCount == 0)
                    {
                        var tabPage = this.ListTab.TabPages[index];
                        if (tabPage.ImageIndex == 0)
                            tabPage.ImageIndex = -1;
                    }
                }
            }
            else
            {
                this.ListTab.Refresh();
            }

            this.DispSelectedPost();
        }

        public string CreateDetailHtml(string orgdata)
            => this.detailHtmlFormatPreparedTemplate.Replace("%CONTENT_HTML%", orgdata);

        private void DispSelectedPost()
            => this.DispSelectedPost(false);

        private PostClass displayPost = new();

        /// <summary>
        /// サムネイル表示に使用する CancellationToken の生成元
        /// </summary>
        private CancellationTokenSource? thumbnailTokenSource = null;

        private void DispSelectedPost(bool forceupdate)
        {
            var currentPost = this.CurrentPost;
            if (currentPost == null)
                return;

            var oldDisplayPost = this.displayPost;
            this.displayPost = currentPost;

            if (!forceupdate && currentPost.Equals(oldDisplayPost))
                return;

            var loadTasks = new TaskCollection();
            loadTasks.Add(() => this.tweetDetailsView.ShowPostDetails(currentPost));

            if (this.settings.Common.PreviewEnable)
            {
                var oldTokenSource = Interlocked.Exchange(ref this.thumbnailTokenSource, new CancellationTokenSource());
                oldTokenSource?.Cancel();

                var token = this.thumbnailTokenSource!.Token;
                loadTasks.Add(() => this.PrepareThumbnailControl(currentPost, token));
            }
            else
            {
                this.SplitContainer3.Panel2Collapsed = true;
            }

            // サムネイルの読み込みを待たずに次に選択されたツイートを表示するため await しない
            _ = loadTasks
                .IgnoreException(x => x is OperationCanceledException)
                .RunAll();
        }

        private async Task PrepareThumbnailControl(PostClass post, CancellationToken token)
        {
            var prepareTask = this.tweetThumbnail1.Model.PrepareThumbnails(post, token);

            var timeout = Task.Delay(100);
            if ((await Task.WhenAny(prepareTask, timeout)) == timeout)
            {
                token.ThrowIfCancellationRequested();

                // サムネイル情報の読み込みに時間が掛かっている場合は一旦サムネイル領域を非表示にする
                this.SplitContainer3.Panel2Collapsed = true;
            }

            await prepareTask;
            token.ThrowIfCancellationRequested();

            this.SplitContainer3.Panel2Collapsed = !this.tweetThumbnail1.Model.ThumbnailAvailable;
        }

        private async void MatomeMenuItem_Click(object sender, EventArgs e)
            => await this.OpenApplicationWebsite();

        private async Task OpenApplicationWebsite()
            => await MyCommon.OpenInBrowserAsync(this, ApplicationSettings.WebsiteUrl);

        private async void ShortcutKeyListMenuItem_Click(object sender, EventArgs e)
            => await MyCommon.OpenInBrowserAsync(this, ApplicationSettings.ShortcutKeyUrl);

        private async void ListTab_KeyDown(object sender, KeyEventArgs e)
        {
            var tab = this.CurrentTab;
            if (tab.TabType == MyCommon.TabUsageType.PublicSearch)
            {
                var pnl = this.CurrentTabPage.Controls["panelSearch"];
                if (pnl.Controls["comboSearch"].Focused ||
                    pnl.Controls["comboLang"].Focused ||
                    pnl.Controls["buttonSearch"].Focused) return;
            }

            if (e.Control || e.Shift || e.Alt)
                tab.ClearAnchor();

            if (this.CommonKeyDown(e.KeyData, FocusedControl.ListTab, out var asyncTask))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            if (asyncTask != null)
                await asyncTask;
        }

        private ShortcutCommand[] shortcutCommands = Array.Empty<ShortcutCommand>();

        private void InitializeShortcuts()
        {
            this.shortcutCommands = new[]
            {
                // リストのカーソル移動関係（上下キー、PageUp/Downに該当）
                ShortcutCommand.Create(Keys.J, Keys.Control | Keys.J, Keys.Shift | Keys.J, Keys.Control | Keys.Shift | Keys.J)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => SendKeys.Send("{DOWN}")),

                ShortcutCommand.Create(Keys.K, Keys.Control | Keys.K, Keys.Shift | Keys.K, Keys.Control | Keys.Shift | Keys.K)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => SendKeys.Send("{UP}")),

                ShortcutCommand.Create(Keys.F, Keys.Shift | Keys.F)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => SendKeys.Send("{PGDN}")),

                ShortcutCommand.Create(Keys.B, Keys.Shift | Keys.B)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => SendKeys.Send("{PGUP}")),

                ShortcutCommand.Create(Keys.F1)
                    .Do(() => this.OpenApplicationWebsite()),

                ShortcutCommand.Create(Keys.F3)
                    .Do(() => this.MenuItemSearchNext_Click(this.MenuItemSearchNext, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.F5)
                    .Do(() => this.DoRefresh()),

                ShortcutCommand.Create(Keys.F6)
                    .Do(() => this.RefreshTabAsync<MentionsTabModel>()),

                ShortcutCommand.Create(Keys.F7)
                    .Do(() => this.RefreshTabAsync<DirectMessagesTabModel>()),

                ShortcutCommand.Create(Keys.Space, Keys.ProcessKey)
                    .NotFocusedOn(FocusedControl.StatusText)
                    .Do(() =>
                    {
                        this.CurrentTab.ClearAnchor();
                        this.JumpUnreadMenuItem_Click(this.JumpUnreadMenuItem, EventArgs.Empty);
                    }),

                ShortcutCommand.Create(Keys.G)
                    .NotFocusedOn(FocusedControl.StatusText)
                    .Do(() =>
                    {
                        this.CurrentTab.ClearAnchor();
                        this.ShowRelatedStatusesMenuItem_Click(this.ShowRelatedStatusesMenuItem, EventArgs.Empty);
                    }),

                ShortcutCommand.Create(Keys.Right, Keys.N)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoRelPost(forward: true)),

                ShortcutCommand.Create(Keys.Left, Keys.P)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoRelPost(forward: false)),

                ShortcutCommand.Create(Keys.OemPeriod)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoAnchor()),

                ShortcutCommand.Create(Keys.I)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.StatusText.Enabled)
                    .Do(() => this.StatusText.Focus()),

                ShortcutCommand.Create(Keys.Enter)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.ListItemDoubleClickAction()),

                ShortcutCommand.Create(Keys.R)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.DoRefresh()),

                ShortcutCommand.Create(Keys.L)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() =>
                    {
                        this.CurrentTab.ClearAnchor();
                        this.GoPost(forward: true);
                    }),

                ShortcutCommand.Create(Keys.H)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() =>
                    {
                        this.CurrentTab.ClearAnchor();
                        this.GoPost(forward: false);
                    }),

                ShortcutCommand.Create(Keys.Z, Keys.Oemcomma)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() =>
                    {
                        this.CurrentTab.ClearAnchor();
                        this.MoveTop();
                    }),

                ShortcutCommand.Create(Keys.S)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() =>
                    {
                        this.CurrentTab.ClearAnchor();
                        this.GoNextTab(forward: true);
                    }),

                ShortcutCommand.Create(Keys.A)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() =>
                    {
                        this.CurrentTab.ClearAnchor();
                        this.GoNextTab(forward: false);
                    }),

                // ] in_reply_to参照元へ戻る
                ShortcutCommand.Create(Keys.Oem4)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() =>
                    {
                        this.CurrentTab.ClearAnchor();
                        return this.GoInReplyToPostTree();
                    }),

                // [ in_reply_toへジャンプ
                ShortcutCommand.Create(Keys.Oem6)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() =>
                    {
                        this.CurrentTab.ClearAnchor();
                        this.GoBackInReplyToPostTree();
                    }),

                ShortcutCommand.Create(Keys.Escape)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() =>
                    {
                        this.CurrentTab.ClearAnchor();
                        var tab = this.CurrentTab;
                        var tabtype = tab.TabType;
                        if (tabtype == MyCommon.TabUsageType.Related || tabtype == MyCommon.TabUsageType.UserTimeline || tabtype == MyCommon.TabUsageType.PublicSearch || tabtype == MyCommon.TabUsageType.SearchResults)
                        {
                            this.RemoveSpecifiedTab(tab.TabName, false);
                            this.SaveConfigsTabs();
                        }
                    }),

                // 上下キー, PageUp/Downキー, Home/Endキー は既定の動作を残しつつアンカー初期化
                ShortcutCommand.Create(Keys.Up, Keys.Down, Keys.PageUp, Keys.PageDown, Keys.Home, Keys.End)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.CurrentTab.ClearAnchor(), preventDefault: false),

                // PreviewKeyDownEventArgs.IsInputKey を true にしてスクロールを発生させる
                ShortcutCommand.Create(Keys.Up, Keys.Down)
                    .FocusedOn(FocusedControl.PostBrowser)
                    .Do(() => { }),

                ShortcutCommand.Create(Keys.Control | Keys.R)
                    .Do(() => this.MakeReplyText()),

                ShortcutCommand.Create(Keys.Control | Keys.D)
                    .Do(() => this.DoStatusDelete()),

                ShortcutCommand.Create(Keys.Control | Keys.M)
                    .Do(() => this.MakeDirectMessageText()),

                ShortcutCommand.Create(Keys.Control | Keys.S)
                    .Do(() => this.FavoriteChange(favAdd: true)),

                ShortcutCommand.Create(Keys.Control | Keys.I)
                    .Do(() => this.DoRepliedStatusOpen()),

                ShortcutCommand.Create(Keys.Control | Keys.Q)
                    .Do(() => this.DoQuoteOfficial()),

                ShortcutCommand.Create(Keys.Control | Keys.B)
                    .Do(() => this.ReadedStripMenuItem_Click(this.ReadedStripMenuItem, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Control | Keys.T)
                    .Do(() => this.HashManageMenuItem_Click(this.HashManageMenuItem, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Control | Keys.L)
                    .Do(() => this.UrlConvertAutoToolStripMenuItem_Click(this.UrlConvertAutoToolStripMenuItem, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Control | Keys.Y)
                    .NotFocusedOn(FocusedControl.PostBrowser)
                    .Do(() => this.MultiLineMenuItem_Click(this.MultiLineMenuItem, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Control | Keys.F)
                    .Do(() => this.MenuItemSubSearch_Click(this.MenuItemSubSearch, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Control | Keys.U)
                    .Do(() => this.ShowUserTimeline()),

                ShortcutCommand.Create(Keys.Control | Keys.H)
                    .Do(() => this.AuthorOpenInBrowserMenuItem_Click(this.AuthorOpenInBrowserContextMenuItem, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Control | Keys.O)
                    .Do(() => this.StatusOpenMenuItem_Click(this.StatusOpenMenuItem, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Control | Keys.E)
                    .Do(() => this.OpenURLMenuItem_Click(this.OpenURLMenuItem, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Control | Keys.Home, Keys.Control | Keys.End)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.selectionDebouncer.Call(), preventDefault: false),

                ShortcutCommand.Create(Keys.Control | Keys.N)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoNextTab(forward: true)),

                ShortcutCommand.Create(Keys.Control | Keys.P)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoNextTab(forward: false)),

                ShortcutCommand.Create(Keys.Control | Keys.C, Keys.Control | Keys.Insert)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.CopyStot()),

                // タブダイレクト選択(Ctrl+1～8,Ctrl+9)
                ShortcutCommand.Create(Keys.Control | Keys.D1)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.statuses.Tabs.Count >= 1)
                    .Do(() => this.ListTab.SelectedIndex = 0),

                ShortcutCommand.Create(Keys.Control | Keys.D2)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.statuses.Tabs.Count >= 2)
                    .Do(() => this.ListTab.SelectedIndex = 1),

                ShortcutCommand.Create(Keys.Control | Keys.D3)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.statuses.Tabs.Count >= 3)
                    .Do(() => this.ListTab.SelectedIndex = 2),

                ShortcutCommand.Create(Keys.Control | Keys.D4)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.statuses.Tabs.Count >= 4)
                    .Do(() => this.ListTab.SelectedIndex = 3),

                ShortcutCommand.Create(Keys.Control | Keys.D5)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.statuses.Tabs.Count >= 5)
                    .Do(() => this.ListTab.SelectedIndex = 4),

                ShortcutCommand.Create(Keys.Control | Keys.D6)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.statuses.Tabs.Count >= 6)
                    .Do(() => this.ListTab.SelectedIndex = 5),

                ShortcutCommand.Create(Keys.Control | Keys.D7)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.statuses.Tabs.Count >= 7)
                    .Do(() => this.ListTab.SelectedIndex = 6),

                ShortcutCommand.Create(Keys.Control | Keys.D8)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.statuses.Tabs.Count >= 8)
                    .Do(() => this.ListTab.SelectedIndex = 7),

                ShortcutCommand.Create(Keys.Control | Keys.D9)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.ListTab.SelectedIndex = this.statuses.Tabs.Count - 1),

                ShortcutCommand.Create(Keys.Control | Keys.A)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => this.StatusText.SelectAll()),

                ShortcutCommand.Create(Keys.Control | Keys.V)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => this.ProcClipboardFromStatusTextWhenCtrlPlusV()),

                ShortcutCommand.Create(Keys.Control | Keys.Up)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => this.StatusTextHistoryBack()),

                ShortcutCommand.Create(Keys.Control | Keys.Down)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => this.StatusTextHistoryForward()),

                ShortcutCommand.Create(Keys.Control | Keys.PageUp, Keys.Control | Keys.P)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() =>
                    {
                        if (this.ListTab.SelectedIndex == 0)
                        {
                            this.ListTab.SelectedIndex = this.ListTab.TabCount - 1;
                        }
                        else
                        {
                            this.ListTab.SelectedIndex -= 1;
                        }
                        this.StatusText.Focus();
                    }),

                ShortcutCommand.Create(Keys.Control | Keys.PageDown, Keys.Control | Keys.N)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() =>
                    {
                        if (this.ListTab.SelectedIndex == this.ListTab.TabCount - 1)
                        {
                            this.ListTab.SelectedIndex = 0;
                        }
                        else
                        {
                            this.ListTab.SelectedIndex += 1;
                        }
                        this.StatusText.Focus();
                    }),

                ShortcutCommand.Create(Keys.Control | Keys.Y)
                    .FocusedOn(FocusedControl.PostBrowser)
                    .Do(() =>
                    {
                        var multiline = !this.settings.Local.StatusMultiline;
                        this.settings.Local.StatusMultiline = multiline;
                        this.MultiLineMenuItem.Checked = multiline;
                        this.MultiLineMenuItem_Click(this.MultiLineMenuItem, EventArgs.Empty);
                    }),

                ShortcutCommand.Create(Keys.Shift | Keys.F3)
                    .Do(() => this.MenuItemSearchPrev_Click(this.MenuItemSearchPrev, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Shift | Keys.F5)
                    .Do(() => this.DoRefreshMore()),

                ShortcutCommand.Create(Keys.Shift | Keys.F6)
                    .Do(() => this.RefreshTabAsync<MentionsTabModel>(backward: true)),

                ShortcutCommand.Create(Keys.Shift | Keys.F7)
                    .Do(() => this.RefreshTabAsync<DirectMessagesTabModel>(backward: true)),

                ShortcutCommand.Create(Keys.Shift | Keys.R)
                    .NotFocusedOn(FocusedControl.StatusText)
                    .Do(() => this.DoRefreshMore()),

                ShortcutCommand.Create(Keys.Shift | Keys.H)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoTopEnd(goTop: true)),

                ShortcutCommand.Create(Keys.Shift | Keys.L)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoTopEnd(goTop: false)),

                ShortcutCommand.Create(Keys.Shift | Keys.M)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoMiddle()),

                ShortcutCommand.Create(Keys.Shift | Keys.G)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoLast()),

                ShortcutCommand.Create(Keys.Shift | Keys.Z)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.MoveMiddle()),

                ShortcutCommand.Create(Keys.Shift | Keys.Oem4)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoBackInReplyToPostTree(parallel: true, isForward: false)),

                ShortcutCommand.Create(Keys.Shift | Keys.Oem6)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoBackInReplyToPostTree(parallel: true, isForward: true)),

                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                ShortcutCommand.Create(Keys.Shift | Keys.Right, Keys.Shift | Keys.N)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoFav(forward: true)),

                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                ShortcutCommand.Create(Keys.Shift | Keys.Left, Keys.Shift | Keys.P)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoFav(forward: false)),

                ShortcutCommand.Create(Keys.Shift | Keys.Space)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoBackSelectPostChain()),

                ShortcutCommand.Create(Keys.Alt | Keys.R)
                    .Do(() => this.DoReTweetOfficial(isConfirm: true)),

                ShortcutCommand.Create(Keys.Alt | Keys.P)
                    .OnlyWhen(() => this.CurrentPost != null)
                    .Do(() => this.DoShowUserStatus(this.CurrentPost!.ScreenName, showInputDialog: false)),

                ShortcutCommand.Create(Keys.Alt | Keys.Up)
                    .Do(() => this.tweetDetailsView.ScrollDownPostBrowser(forward: false)),

                ShortcutCommand.Create(Keys.Alt | Keys.Down)
                    .Do(() => this.tweetDetailsView.ScrollDownPostBrowser(forward: true)),

                ShortcutCommand.Create(Keys.Alt | Keys.PageUp)
                    .Do(() => this.tweetDetailsView.PageDownPostBrowser(forward: false)),

                ShortcutCommand.Create(Keys.Alt | Keys.PageDown)
                    .Do(() => this.tweetDetailsView.PageDownPostBrowser(forward: true)),

                // 別タブの同じ書き込みへ(ALT+←/→)
                ShortcutCommand.Create(Keys.Alt | Keys.Right)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoSamePostToAnotherTab(left: false)),

                ShortcutCommand.Create(Keys.Alt | Keys.Left)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoSamePostToAnotherTab(left: true)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.R)
                    .Do(() => this.MakeReplyText(atAll: true)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.C, Keys.Control | Keys.Shift | Keys.Insert)
                    .Do(() => this.CopyIdUri()),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.F)
                    .OnlyWhen(() => this.CurrentTab.TabType == MyCommon.TabUsageType.PublicSearch)
                    .Do(() => this.CurrentTabPage.Controls["panelSearch"].Controls["comboSearch"].Focus()),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.L)
                    .Do(() => this.DoQuoteOfficial()),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.S)
                    .Do(() => this.FavoriteChange(favAdd: false)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.B)
                    .Do(() => this.UnreadStripMenuItem_Click(this.UnreadStripMenuItem, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.T)
                    .Do(() => this.HashToggleMenuItem_Click(this.HashToggleMenuItem, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.P)
                    .Do(() => this.ImageSelectMenuItem_Click(this.ImageSelectMenuItem, EventArgs.Empty)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.H)
                    .Do(() => this.DoMoveToRTHome()),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.Up)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() =>
                    {
                        var tab = this.CurrentTab;
                        var selectedIndex = tab.SelectedIndex;
                        if (selectedIndex != -1 && selectedIndex > 0)
                        {
                            var listView = this.CurrentListView;
                            var idx = selectedIndex - 1;
                            this.SelectListItem(listView, idx);
                            listView.EnsureVisible(idx);
                        }
                    }),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.Down)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() =>
                    {
                        var tab = this.CurrentTab;
                        var selectedIndex = tab.SelectedIndex;
                        if (selectedIndex != -1 && selectedIndex < tab.AllCount - 1)
                        {
                            var listView = this.CurrentListView;
                            var idx = selectedIndex + 1;
                            this.SelectListItem(listView, idx);
                            listView.EnsureVisible(idx);
                        }
                    }),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.Space)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() =>
                    {
                        if (this.StatusText.SelectionStart > 0)
                        {
                            var endidx = this.StatusText.SelectionStart - 1;
                            var startstr = "";
                            for (var i = this.StatusText.SelectionStart - 1; i >= 0; i--)
                            {
                                var c = this.StatusText.Text[i];
                                if (char.IsLetterOrDigit(c) || c == '_')
                                {
                                    continue;
                                }
                                if (c == '@')
                                {
                                    startstr = this.StatusText.Text.Substring(i + 1, endidx - i);
                                    var cnt = this.AtIdSupl.ItemCount;
                                    this.ShowSuplDialog(this.StatusText, this.AtIdSupl, startstr.Length + 1, startstr);
                                    if (this.AtIdSupl.ItemCount != cnt)
                                        this.MarkSettingAtIdModified();
                                }
                                else if (c == '#')
                                {
                                    startstr = this.StatusText.Text.Substring(i + 1, endidx - i);
                                    this.ShowSuplDialog(this.StatusText, this.HashSupl, startstr.Length + 1, startstr);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }),

                // ソートダイレクト選択(Ctrl+Shift+1～8,Ctrl+Shift+9)
                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.D1)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.SetSortColumnByDisplayIndex(0)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.D2)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.SetSortColumnByDisplayIndex(1)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.D3)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.SetSortColumnByDisplayIndex(2)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.D4)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.SetSortColumnByDisplayIndex(3)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.D5)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.SetSortColumnByDisplayIndex(4)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.D6)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.SetSortColumnByDisplayIndex(5)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.D7)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.SetSortColumnByDisplayIndex(6)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.D8)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.SetSortColumnByDisplayIndex(7)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.D9)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.SetSortLastColumn()),

                ShortcutCommand.Create(Keys.Control | Keys.Alt | Keys.S)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.FavoritesRetweetOfficial()),

                ShortcutCommand.Create(Keys.Control | Keys.Alt | Keys.R)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.FavoritesRetweetUnofficial()),

                ShortcutCommand.Create(Keys.Control | Keys.Alt | Keys.H)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.OpenUserAppointUrl()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.R)
                    .FocusedOn(FocusedControl.PostBrowser)
                    .Do(() => this.DoReTweetUnofficial()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.T)
                    .OnlyWhen(() => this.ExistCurrentPost)
                    .Do(() => this.tweetDetailsView.DoTranslation()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.R)
                    .Do(() => this.DoReTweetUnofficial()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.C, Keys.Alt | Keys.Shift | Keys.Insert)
                    .Do(() => this.CopyUserId()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.Up)
                    .Do(() => this.tweetThumbnail1.Model.ScrollUp()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.Down)
                    .Do(() => this.tweetThumbnail1.Model.ScrollDown()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.Enter)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => !this.SplitContainer3.Panel2Collapsed)
                    .Do(() => this.tweetThumbnail1.OpenImageInBrowser()),
            };
        }

        internal bool CommonKeyDown(Keys keyData, FocusedControl focusedOn, out Task? asyncTask)
        {
            // Task を返す非同期処理があれば asyncTask に代入する
            asyncTask = null;

            // ShortcutCommand に対応しているコマンドはここで処理される
            foreach (var command in this.shortcutCommands)
            {
                if (command.IsMatch(keyData, focusedOn))
                {
                    asyncTask = command.RunCommand();
                    return command.PreventDefault;
                }
            }

            return false;
        }

        private void GoNextTab(bool forward)
        {
            var idx = this.statuses.SelectedTabIndex;
            var tabCount = this.statuses.Tabs.Count;
            if (forward)
            {
                idx += 1;
                if (idx > tabCount - 1) idx = 0;
            }
            else
            {
                idx -= 1;
                if (idx < 0) idx = tabCount - 1;
            }
            this.ListTab.SelectedIndex = idx;
        }

        private void CopyStot()
        {
            var sb = new StringBuilder();
            var tab = this.CurrentTab;
            var isProtected = false;
            var isDm = tab.TabType == MyCommon.TabUsageType.DirectMessage;
            foreach (var post in tab.SelectedPosts)
            {
                if (post.IsDeleted) continue;
                if (!isDm)
                {
                    if (post.RetweetedId != null)
                        sb.AppendFormat("{0}:{1} [https://twitter.com/{0}/status/{2}]{3}", post.ScreenName, post.TextSingleLine, post.RetweetedId, Environment.NewLine);
                    else
                        sb.AppendFormat("{0}:{1} [https://twitter.com/{0}/status/{2}]{3}", post.ScreenName, post.TextSingleLine, post.StatusId, Environment.NewLine);
                }
                else
                {
                    sb.AppendFormat("{0}:{1} [{2}]{3}", post.ScreenName, post.TextSingleLine, post.StatusId, Environment.NewLine);
                }
            }
            if (isProtected)
            {
                MessageBox.Show(Properties.Resources.CopyStotText1);
            }
            if (sb.Length > 0)
            {
                var clstr = sb.ToString();
                try
                {
                    Clipboard.SetDataObject(clstr, false, 5, 100);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void CopyIdUri()
        {
            var tab = this.CurrentTab;
            if (tab == null || tab is DirectMessagesTabModel)
                return;

            var copyUrls = new List<string>();
            foreach (var post in tab.SelectedPosts)
                copyUrls.Add(MyCommon.GetStatusUrl(post));

            if (copyUrls.Count == 0)
                return;

            try
            {
                Clipboard.SetDataObject(string.Join(Environment.NewLine, copyUrls), false, 5, 100);
            }
            catch (ExternalException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GoFav(bool forward)
        {
            var tab = this.CurrentTab;
            if (tab.AllCount == 0)
                return;

            var selectedIndex = tab.SelectedIndex;

            int fIdx;
            int toIdx;
            int stp;

            if (forward)
            {
                if (selectedIndex == -1)
                {
                    fIdx = 0;
                }
                else
                {
                    fIdx = selectedIndex + 1;
                    if (fIdx > tab.AllCount - 1)
                        return;
                }
                toIdx = tab.AllCount;
                stp = 1;
            }
            else
            {
                if (selectedIndex == -1)
                {
                    fIdx = tab.AllCount - 1;
                }
                else
                {
                    fIdx = selectedIndex - 1;
                    if (fIdx < 0)
                        return;
                }
                toIdx = -1;
                stp = -1;
            }

            for (var idx = fIdx; idx != toIdx; idx += stp)
            {
                if (tab[idx].IsFav)
                {
                    var listView = this.CurrentListView;
                    this.SelectListItem(listView, idx);
                    listView.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoSamePostToAnotherTab(bool left)
        {
            var tab = this.CurrentTab;

            // Directタブは対象外（見つかるはずがない）
            if (tab.TabType == MyCommon.TabUsageType.DirectMessage)
                return;

            var selectedStatusId = tab.SelectedStatusId;
            if (selectedStatusId == null)
                return;

            int fIdx, toIdx, stp;

            if (left)
            {
                // 左のタブへ
                if (this.ListTab.SelectedIndex == 0)
                {
                    return;
                }
                else
                {
                    fIdx = this.ListTab.SelectedIndex - 1;
                }
                toIdx = -1;
                stp = -1;
            }
            else
            {
                // 右のタブへ
                if (this.ListTab.SelectedIndex == this.ListTab.TabCount - 1)
                {
                    return;
                }
                else
                {
                    fIdx = this.ListTab.SelectedIndex + 1;
                }
                toIdx = this.ListTab.TabCount;
                stp = 1;
            }

            for (var tabidx = fIdx; tabidx != toIdx; tabidx += stp)
            {
                var targetTab = this.statuses.Tabs[tabidx];

                // Directタブは対象外
                if (targetTab.TabType == MyCommon.TabUsageType.DirectMessage)
                    continue;

                var foundIndex = targetTab.IndexOf(selectedStatusId);
                if (foundIndex != -1)
                {
                    this.ListTab.SelectedIndex = tabidx;
                    var listView = this.CurrentListView;
                    this.SelectListItem(listView, foundIndex);
                    listView.EnsureVisible(foundIndex);
                    return;
                }
            }
        }

        private void GoPost(bool forward)
        {
            var tab = this.CurrentTab;
            var currentPost = this.CurrentPost;

            if (currentPost == null)
                return;

            var selectedIndex = tab.SelectedIndex;

            int fIdx, toIdx, stp;

            if (forward)
            {
                fIdx = selectedIndex + 1;
                if (fIdx > tab.AllCount - 1) return;
                toIdx = tab.AllCount;
                stp = 1;
            }
            else
            {
                fIdx = selectedIndex - 1;
                if (fIdx < 0) return;
                toIdx = -1;
                stp = -1;
            }

            string name;
            if (currentPost.RetweetedBy == null)
            {
                name = currentPost.ScreenName;
            }
            else
            {
                name = currentPost.RetweetedBy;
            }
            for (var idx = fIdx; idx != toIdx; idx += stp)
            {
                var post = tab[idx];
                if (post.RetweetedId == null)
                {
                    if (post.ScreenName == name)
                    {
                        var listView = this.CurrentListView;
                        this.SelectListItem(listView, idx);
                        listView.EnsureVisible(idx);
                        break;
                    }
                }
                else
                {
                    if (post.RetweetedBy == name)
                    {
                        var listView = this.CurrentListView;
                        this.SelectListItem(listView, idx);
                        listView.EnsureVisible(idx);
                        break;
                    }
                }
            }
        }

        private void GoRelPost(bool forward)
        {
            var tab = this.CurrentTab;
            var selectedIndex = tab.SelectedIndex;

            if (selectedIndex == -1)
                return;

            int fIdx, toIdx, stp;

            if (forward)
            {
                fIdx = selectedIndex + 1;
                if (fIdx > tab.AllCount - 1) return;
                toIdx = tab.AllCount;
                stp = 1;
            }
            else
            {
                fIdx = selectedIndex - 1;
                if (fIdx < 0) return;
                toIdx = -1;
                stp = -1;
            }

            var anchorPost = tab.AnchorPost;
            if (anchorPost == null)
            {
                var currentPost = this.CurrentPost;
                if (currentPost == null)
                    return;

                anchorPost = currentPost;
                tab.AnchorPost = currentPost;
            }

            for (var idx = fIdx; idx != toIdx; idx += stp)
            {
                var post = tab[idx];
                if (post.ScreenName == anchorPost.ScreenName ||
                    post.RetweetedBy == anchorPost.ScreenName ||
                    post.ScreenName == anchorPost.RetweetedBy ||
                    (!MyCommon.IsNullOrEmpty(post.RetweetedBy) && post.RetweetedBy == anchorPost.RetweetedBy) ||
                    anchorPost.ReplyToList.Any(x => x.UserId == post.UserId) ||
                    anchorPost.ReplyToList.Any(x => x.UserId == post.RetweetedByUserId) ||
                    post.ReplyToList.Any(x => x.UserId == anchorPost.UserId) ||
                    post.ReplyToList.Any(x => x.UserId == anchorPost.RetweetedByUserId))
                {
                    var listView = this.CurrentListView;
                    this.SelectListItem(listView, idx);
                    listView.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoAnchor()
        {
            var anchorStatusId = this.CurrentTab.AnchorStatusId;
            if (anchorStatusId == null)
                return;

            var idx = this.CurrentTab.IndexOf(anchorStatusId);
            if (idx == -1)
                return;

            var listView = this.CurrentListView;
            this.SelectListItem(listView, idx);
            listView.EnsureVisible(idx);
        }

        private void GoTopEnd(bool goTop)
        {
            var listView = this.CurrentListView;
            if (listView.VirtualListSize == 0)
                return;

            ListViewItem item;
            int idx;

            if (goTop)
            {
                item = listView.GetItemAt(0, 25);
                if (item == null)
                    idx = 0;
                else
                    idx = item.Index;
            }
            else
            {
                item = listView.GetItemAt(0, listView.ClientSize.Height - 1);
                if (item == null)
                    idx = listView.VirtualListSize - 1;
                else
                    idx = item.Index;
            }
            this.SelectListItem(listView, idx);
        }

        private void GoMiddle()
        {
            var listView = this.CurrentListView;
            if (listView.VirtualListSize == 0)
                return;

            ListViewItem item;
            int idx1;
            int idx2;
            int idx3;

            item = listView.GetItemAt(0, 0);
            if (item == null)
            {
                idx1 = 0;
            }
            else
            {
                idx1 = item.Index;
            }

            item = listView.GetItemAt(0, listView.ClientSize.Height - 1);
            if (item == null)
            {
                idx2 = listView.VirtualListSize - 1;
            }
            else
            {
                idx2 = item.Index;
            }
            idx3 = (idx1 + idx2) / 2;

            this.SelectListItem(listView, idx3);
        }

        private void GoLast()
        {
            var listView = this.CurrentListView;
            if (listView.VirtualListSize == 0) return;

            if (this.statuses.SortOrder == SortOrder.Ascending)
            {
                this.SelectListItem(listView, listView.VirtualListSize - 1);
                listView.EnsureVisible(listView.VirtualListSize - 1);
            }
            else
            {
                this.SelectListItem(listView, 0);
                listView.EnsureVisible(0);
            }
        }

        private void MoveTop()
        {
            var listView = this.CurrentListView;
            if (listView.SelectedIndices.Count == 0) return;
            var idx = listView.SelectedIndices[0];
            if (this.statuses.SortOrder == SortOrder.Ascending)
            {
                listView.EnsureVisible(listView.VirtualListSize - 1);
            }
            else
            {
                listView.EnsureVisible(0);
            }
            listView.EnsureVisible(idx);
        }

        private async Task GoInReplyToPostTree()
        {
            var curTabClass = this.CurrentTab;
            var currentPost = this.CurrentPost;

            if (currentPost == null)
                return;

            if (curTabClass.TabType == MyCommon.TabUsageType.PublicSearch && currentPost.InReplyToStatusId == null && currentPost.TextFromApi.Contains("@"))
            {
                try
                {
                    var post = await this.tw.GetStatusApi(false, currentPost.StatusId.ToTwitterStatusId());

                    currentPost = currentPost with
                    {
                        InReplyToStatusId = post.InReplyToStatusId,
                        InReplyToUser = post.InReplyToUser,
                        IsReply = post.IsReply,
                    };
                    curTabClass.ReplacePost(currentPost);
                    this.listCache?.PurgeCache();

                    var index = curTabClass.SelectedIndex;
                    this.CurrentListView.RedrawItems(index, index, false);
                }
                catch (WebApiException ex)
                {
                    this.StatusLabel.Text = $"Err:{ex.Message}(GetStatus)";
                }
            }

            if (!(this.ExistCurrentPost && currentPost.InReplyToUser != null && currentPost.InReplyToStatusId != null)) return;

            if (this.replyChains == null || (this.replyChains.Count > 0 && this.replyChains.Peek().InReplyToId != currentPost.StatusId))
            {
                this.replyChains = new Stack<ReplyChain>();
            }
            this.replyChains.Push(new ReplyChain(currentPost.StatusId, currentPost.InReplyToStatusId, curTabClass));

            int inReplyToIndex;
            string inReplyToTabName;
            var inReplyToId = currentPost.InReplyToStatusId;
            var inReplyToUser = currentPost.InReplyToUser;

            var inReplyToPosts = from tab in this.statuses.Tabs
                                 orderby tab != curTabClass
                                 from post in tab.Posts.Values
                                 where post.StatusId == inReplyToId
                                 let index = tab.IndexOf(post.StatusId)
                                 where index != -1
                                 select new { Tab = tab, Index = index };

            var inReplyPost = inReplyToPosts.FirstOrDefault();
            if (inReplyPost == null)
            {
                try
                {
                    await Task.Run(async () =>
                    {
                        var post = await this.tw.GetStatusApi(false, currentPost.InReplyToStatusId.ToTwitterStatusId())
                            .ConfigureAwait(false);
                        post.IsRead = true;

                        this.statuses.AddPost(post);
                        this.statuses.DistributePosts();
                    });
                }
                catch (WebApiException ex)
                {
                    this.StatusLabel.Text = $"Err:{ex.Message}(GetStatus)";
                    await MyCommon.OpenInBrowserAsync(this, MyCommon.GetStatusUrl(inReplyToUser, inReplyToId.ToTwitterStatusId()));
                    return;
                }

                this.RefreshTimeline();

                inReplyPost = inReplyToPosts.FirstOrDefault();
                if (inReplyPost == null)
                {
                    await MyCommon.OpenInBrowserAsync(this, MyCommon.GetStatusUrl(inReplyToUser, inReplyToId.ToTwitterStatusId()));
                    return;
                }
            }
            inReplyToTabName = inReplyPost.Tab.TabName;
            inReplyToIndex = inReplyPost.Index;

            var tabIndex = this.statuses.Tabs.IndexOf(inReplyToTabName);
            var tabPage = this.ListTab.TabPages[tabIndex];
            var listView = (DetailsListView)tabPage.Tag;

            if (this.CurrentTabName != inReplyToTabName)
            {
                this.ListTab.SelectedIndex = tabIndex;
            }

            this.SelectListItem(listView, inReplyToIndex);
            listView.EnsureVisible(inReplyToIndex);
        }

        private void GoBackInReplyToPostTree(bool parallel = false, bool isForward = true)
        {
            var curTabClass = this.CurrentTab;
            var currentPost = this.CurrentPost;

            if (currentPost == null)
                return;

            if (parallel)
            {
                if (currentPost.InReplyToStatusId != null)
                {
                    var posts = from t in this.statuses.Tabs
                                from p in t.Posts
                                where p.Value.StatusId != currentPost.StatusId && p.Value.InReplyToStatusId == currentPost.InReplyToStatusId
                                let indexOf = t.IndexOf(p.Value.StatusId)
                                where indexOf > -1
                                orderby isForward ? indexOf : indexOf * -1
                                orderby t != curTabClass
                                select new { Tab = t, Post = p.Value, Index = indexOf };
                    try
                    {
                        var postList = posts.ToList();
                        for (var i = postList.Count - 1; i >= 0; i--)
                        {
                            var index = i;
                            if (postList.FindIndex(pst => pst.Post.StatusId == postList[index].Post.StatusId) != index)
                            {
                                postList.RemoveAt(index);
                            }
                        }
                        var currentIndex = this.CurrentTab.SelectedIndex;
                        var post = postList.FirstOrDefault(pst => pst.Tab == curTabClass && isForward ? pst.Index > currentIndex : pst.Index < currentIndex);
                        if (post == null) post = postList.FirstOrDefault(pst => pst.Tab != curTabClass);
                        if (post == null) post = postList.First();
                        var tabIndex = this.statuses.Tabs.IndexOf(post.Tab);
                        this.ListTab.SelectedIndex = tabIndex;
                        var listView = this.CurrentListView;
                        this.SelectListItem(listView, post.Index);
                        listView.EnsureVisible(post.Index);
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                }
            }
            else
            {
                if (this.replyChains == null || this.replyChains.Count < 1)
                {
                    var posts = from t in this.statuses.Tabs
                                from p in t.Posts
                                where p.Value.InReplyToStatusId == currentPost.StatusId
                                let indexOf = t.IndexOf(p.Value.StatusId)
                                where indexOf > -1
                                orderby indexOf
                                orderby t != curTabClass
                                select new { Tab = t, Index = indexOf };
                    try
                    {
                        var post = posts.First();
                        var tabIndex = this.statuses.Tabs.IndexOf(post.Tab);
                        this.ListTab.SelectedIndex = tabIndex;
                        var listView = this.CurrentListView;
                        this.SelectListItem(listView, post.Index);
                        listView.EnsureVisible(post.Index);
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                }
                else
                {
                    var chainHead = this.replyChains.Pop();
                    if (chainHead.InReplyToId == currentPost.StatusId)
                    {
                        var tab = chainHead.OriginalTab;
                        if (!this.statuses.Tabs.Contains(tab))
                        {
                            this.replyChains = null;
                        }
                        else
                        {
                            var idx = tab.IndexOf(chainHead.OriginalId);
                            if (idx == -1)
                            {
                                this.replyChains = null;
                            }
                            else
                            {
                                var tabIndex = this.statuses.Tabs.IndexOf(tab);
                                try
                                {
                                    this.ListTab.SelectedIndex = tabIndex;
                                }
                                catch (Exception)
                                {
                                    this.replyChains = null;
                                }
                                var listView = this.CurrentListView;
                                this.SelectListItem(listView, idx);
                                listView.EnsureVisible(idx);
                            }
                        }
                    }
                    else
                    {
                        this.replyChains = null;
                        this.GoBackInReplyToPostTree(parallel);
                    }
                }
            }
        }

        private void GoBackSelectPostChain()
        {
            if (this.selectPostChains.Count > 1)
            {
                var idx = -1;
                TabModel? foundTab = null;

                do
                {
                    try
                    {
                        this.selectPostChains.Pop();
                        var (tab, post) = this.selectPostChains.Peek();

                        if (!this.statuses.Tabs.Contains(tab))
                            continue; // 該当タブが存在しないので無視

                        if (post != null)
                        {
                            idx = tab.IndexOf(post.StatusId);
                            if (idx == -1) continue;  // 該当ポストが存在しないので無視
                        }

                        foundTab = tab;

                        this.selectPostChains.Pop();
                    }
                    catch (InvalidOperationException)
                    {
                    }

                    break;
                }
                while (this.selectPostChains.Count > 1);

                if (foundTab == null)
                {
                    // 状態がおかしいので処理を中断
                    // 履歴が残り1つであればクリアしておく
                    if (this.selectPostChains.Count == 1)
                        this.selectPostChains.Clear();
                    return;
                }

                var tabIndex = this.statuses.Tabs.IndexOf(foundTab);
                var tabPage = this.ListTab.TabPages[tabIndex];
                var lst = (DetailsListView)tabPage.Tag;
                this.ListTab.SelectedIndex = tabIndex;

                if (idx > -1)
                {
                    this.SelectListItem(lst, idx);
                    lst.EnsureVisible(idx);
                }
                lst.Focus();
            }
        }

        private void PushSelectPostChain()
        {
            var currentTab = this.CurrentTab;
            var currentPost = this.CurrentPost;

            var count = this.selectPostChains.Count;
            if (count > 0)
            {
                var (tab, post) = this.selectPostChains.Peek();
                if (tab == currentTab)
                {
                    if (post == currentPost) return;  // 最新の履歴と同一
                    if (post == null) this.selectPostChains.Pop();  // 置き換えるため削除
                }
            }
            if (count >= 2500) this.TrimPostChain();
            this.selectPostChains.Push((currentTab, currentPost));
        }

        private void TrimPostChain()
        {
            if (this.selectPostChains.Count <= 2000) return;
            var p = new Stack<(TabModel, PostClass?)>(2000);
            for (var i = 0; i < 2000; i++)
            {
                p.Push(this.selectPostChains.Pop());
            }
            this.selectPostChains.Clear();
            for (var i = 0; i < 2000; i++)
            {
                this.selectPostChains.Push(p.Pop());
            }
        }

        private bool GoStatus(PostId statusId)
        {
            var tab = this.statuses.Tabs
                .Where(x => x.TabType != MyCommon.TabUsageType.DirectMessage)
                .Where(x => x.Contains(statusId))
                .FirstOrDefault();

            if (tab == null)
                return false;

            var index = tab.IndexOf(statusId);

            var tabIndex = this.statuses.Tabs.IndexOf(tab);
            this.ListTab.SelectedIndex = tabIndex;

            var listView = this.CurrentListView;
            this.SelectListItem(listView, index);
            listView.EnsureVisible(index);

            return true;
        }

        private bool GoDirectMessage(PostId statusId)
        {
            var tab = this.statuses.DirectMessageTab;
            var index = tab.IndexOf(statusId);

            if (index == -1)
                return false;

            var tabIndex = this.statuses.Tabs.IndexOf(tab);
            this.ListTab.SelectedIndex = tabIndex;

            var listView = this.CurrentListView;
            this.SelectListItem(listView, index);
            listView.EnsureVisible(index);

            return true;
        }

        private void MyList_MouseClick(object sender, MouseEventArgs e)
            => this.CurrentTab.ClearAnchor();

        private void StatusText_Enter(object sender, EventArgs e)
        {
            // フォーカスの戻り先を StatusText に設定
            this.Tag = this.StatusText;
            this.StatusText.BackColor = this.themeManager.ColorInputBackcolor;
        }

        public Color InputBackColor
            => this.themeManager.ColorInputBackcolor;

        private void StatusText_Leave(object sender, EventArgs e)
        {
            // フォーカスがメニューに遷移しないならばフォーカスはタブに移ることを期待
            if (this.ListTab.SelectedTab != null && this.MenuStrip1.Tag == null) this.Tag = this.ListTab.SelectedTab.Tag;
            this.StatusText.BackColor = Color.FromKnownColor(KnownColor.Window);
        }

        private async void StatusText_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.CommonKeyDown(e.KeyData, FocusedControl.StatusText, out var asyncTask))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            this.StatusText_TextChanged(this.StatusText, EventArgs.Empty);

            if (asyncTask != null)
                await asyncTask;
        }

        private void SaveConfigsAll(bool ifModified)
        {
            if (!ifModified)
            {
                this.SaveConfigsCommon();
                this.SaveConfigsLocal();
                this.SaveConfigsTabs();
                this.SaveConfigsAtId();
            }
            else
            {
                if (this.ModifySettingCommon) this.SaveConfigsCommon();
                if (this.ModifySettingLocal) this.SaveConfigsLocal();
                if (this.ModifySettingAtId) this.SaveConfigsAtId();
            }
        }

        private void SaveConfigsAtId()
        {
            if (this.ignoreConfigSave || !this.settings.Common.UseAtIdSupplement && this.AtIdSupl == null) return;

            this.ModifySettingAtId = false;
            this.settings.AtIdList.AtIdList = this.AtIdSupl.GetItemList();
            this.settings.SaveAtIdList();
        }

        private void SaveConfigsCommon()
        {
            if (this.ignoreConfigSave) return;

            this.ModifySettingCommon = false;
            lock (this.syncObject)
            {
                this.settings.Common.UserName = this.tw.Username;
                this.settings.Common.UserId = this.tw.UserId;
                this.settings.Common.Token = this.tw.AccessToken;
                this.settings.Common.TokenSecret = this.tw.AccessTokenSecret;
                this.settings.Common.SortOrder = (int)this.statuses.SortOrder;
                this.settings.Common.SortColumn = this.statuses.SortMode switch
                {
                    ComparerMode.Nickname => 1, // ニックネーム
                    ComparerMode.Data => 2, // 本文
                    ComparerMode.Id => 3, // 時刻=発言Id
                    ComparerMode.Name => 4, // 名前
                    ComparerMode.Source => 7, // Source
                    _ => throw new InvalidOperationException($"Invalid sort mode: {this.statuses.SortMode}"),
                };
                this.settings.Common.HashTags = this.HashMgr.HashHistories;
                if (this.HashMgr.IsPermanent)
                {
                    this.settings.Common.HashSelected = this.HashMgr.UseHash;
                }
                else
                {
                    this.settings.Common.HashSelected = "";
                }
                this.settings.Common.HashIsHead = this.HashMgr.IsHead;
                this.settings.Common.HashIsPermanent = this.HashMgr.IsPermanent;
                this.settings.Common.HashIsNotAddToAtReply = this.HashMgr.IsNotAddToAtReply;
                this.settings.Common.UseImageService = this.ImageSelector.Model.SelectedMediaServiceIndex;
                this.settings.Common.UseImageServiceName = this.ImageSelector.Model.SelectedMediaServiceName;

                this.settings.SaveCommon();
            }
        }

        private void SaveConfigsLocal()
        {
            if (this.ignoreConfigSave) return;
            lock (this.syncObject)
            {
                this.ModifySettingLocal = false;
                this.settings.Local.ScaleDimension = this.CurrentAutoScaleDimensions;
                this.settings.Local.FormSize = this.mySize;
                this.settings.Local.FormLocation = this.myLoc;
                this.settings.Local.SplitterDistance = this.mySpDis;
                this.settings.Local.PreviewDistance = this.mySpDis3;
                this.settings.Local.StatusMultiline = this.StatusText.Multiline;
                this.settings.Local.StatusTextHeight = this.mySpDis2;

                if (this.ignoreConfigSave) return;
                this.settings.SaveLocal();
            }
        }

        private void SaveConfigsTabs()
        {
            var tabSettingList = new List<SettingTabs.SettingTabItem>();

            var tabs = this.statuses.Tabs.Append(this.statuses.MuteTab);

            foreach (var tab in tabs)
            {
                if (!tab.IsPermanentTabType)
                    continue;

                var tabSetting = new SettingTabs.SettingTabItem
                {
                    TabName = tab.TabName,
                    TabType = tab.TabType,
                    UnreadManage = tab.UnreadManage,
                    Protected = tab.Protected,
                    Notify = tab.Notify,
                    SoundFile = tab.SoundFile,
                };

                switch (tab)
                {
                    case FilterTabModel filterTab:
                        tabSetting.FilterArray = filterTab.FilterArray;
                        break;
                    case UserTimelineTabModel userTab:
                        tabSetting.User = userTab.ScreenName;
                        tabSetting.UserId = userTab.UserId;
                        break;
                    case PublicSearchTabModel searchTab:
                        tabSetting.SearchWords = searchTab.SearchWords;
                        tabSetting.SearchLang = searchTab.SearchLang;
                        break;
                    case ListTimelineTabModel listTab:
                        tabSetting.ListInfo = listTab.ListInfo;
                        break;
                }

                tabSettingList.Add(tabSetting);
            }

            this.settings.Tabs.Tabs = tabSettingList;
            this.settings.SaveTabs();
        }

        private async void OpenURLFileMenuItem_Click(object sender, EventArgs e)
        {
            static void ShowFormatErrorDialog(IWin32Window owner)
            {
                MessageBox.Show(
                    owner,
                    Properties.Resources.OpenURL_InvalidFormat,
                    Properties.Resources.OpenURL_Caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            var ret = InputDialog.Show(this, Properties.Resources.OpenURL_InputText, Properties.Resources.OpenURL_Caption, out var inputText);
            if (ret != DialogResult.OK)
                return;

            var match = Twitter.StatusUrlRegex.Match(inputText);
            if (!match.Success)
            {
                ShowFormatErrorDialog(this);
                return;
            }

            try
            {
                var statusId = new TwitterStatusId(match.Groups["StatusId"].Value);
                await this.OpenRelatedTab(statusId);
            }
            catch (OverflowException)
            {
                ShowFormatErrorDialog(this);
            }
            catch (TabException ex)
            {
                MessageBox.Show(this, ex.Message, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveLogMenuItem_Click(object sender, EventArgs e)
        {
            var tab = this.CurrentTab;

            var rslt = MessageBox.Show(
                string.Format(Properties.Resources.SaveLogMenuItem_ClickText1, Environment.NewLine),
                Properties.Resources.SaveLogMenuItem_ClickText2,
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel) return;

            this.SaveFileDialog1.FileName = $"{ApplicationSettings.AssemblyName}Posts{DateTimeUtc.Now.ToLocalTime():yyMMdd-HHmmss}.tsv";
            this.SaveFileDialog1.InitialDirectory = Application.ExecutablePath;
            this.SaveFileDialog1.Filter = Properties.Resources.SaveLogMenuItem_ClickText3;
            this.SaveFileDialog1.FilterIndex = 0;
            this.SaveFileDialog1.Title = Properties.Resources.SaveLogMenuItem_ClickText4;
            this.SaveFileDialog1.RestoreDirectory = true;

            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!this.SaveFileDialog1.ValidateNames) return;
                using var sw = new StreamWriter(this.SaveFileDialog1.FileName, false, Encoding.UTF8);
                if (rslt == DialogResult.Yes)
                {
                    // All
                    for (var idx = 0; idx < tab.AllCount; idx++)
                    {
                        var post = tab[idx];
                        var protect = "";
                        if (post.IsProtect)
                            protect = "Protect";
                        sw.WriteLine(post.Nickname + "\t" +
                                 "\"" + post.TextFromApi.Replace("\n", "").Replace("\"", "\"\"") + "\"" + "\t" +
                                 post.CreatedAt.ToLocalTimeString() + "\t" +
                                 post.ScreenName + "\t" +
                                 post.StatusId.Id + "\t" +
                                 post.ImageUrl + "\t" +
                                 "\"" + post.Text.Replace("\n", "").Replace("\"", "\"\"") + "\"" + "\t" +
                                 protect);
                    }
                }
                else
                {
                    foreach (var post in this.CurrentTab.SelectedPosts)
                    {
                        var protect = "";
                        if (post.IsProtect)
                            protect = "Protect";
                        sw.WriteLine(post.Nickname + "\t" +
                                 "\"" + post.TextFromApi.Replace("\n", "").Replace("\"", "\"\"") + "\"" + "\t" +
                                 post.CreatedAt.ToLocalTimeString() + "\t" +
                                 post.ScreenName + "\t" +
                                 post.StatusId.Id + "\t" +
                                 post.ImageUrl + "\t" +
                                 "\"" + post.Text.Replace("\n", "").Replace("\"", "\"\"") + "\"" + "\t" +
                                 protect);
                    }
                }
            }
            this.TopMost = this.settings.Common.AlwaysTop;
        }

        public bool TabRename(string origTabName, [NotNullWhen(true)] out string? newTabName)
        {
            // タブ名変更
            newTabName = null;
            using (var inputName = new InputTabName())
            {
                inputName.TabName = origTabName;
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel) return false;
                newTabName = inputName.TabName;
            }
            this.TopMost = this.settings.Common.AlwaysTop;
            if (!MyCommon.IsNullOrEmpty(newTabName))
            {
                // 新タブ名存在チェック
                if (this.statuses.ContainsTab(newTabName))
                {
                    var tmp = string.Format(Properties.Resources.Tabs_DoubleClickText1, newTabName);
                    MessageBox.Show(tmp, Properties.Resources.Tabs_DoubleClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                var tabIndex = this.statuses.Tabs.IndexOf(origTabName);
                var tabPage = this.ListTab.TabPages[tabIndex];

                // タブ名を変更
                if (tabPage != null)
                    tabPage.Text = newTabName;

                this.statuses.RenameTab(origTabName, newTabName);

                var state = this.listViewState[origTabName];
                this.listViewState.Remove(origTabName);
                this.listViewState[newTabName] = state;

                this.SaveConfigsCommon();
                this.SaveConfigsTabs();
                this.rclickTabName = newTabName;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ListTab_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                foreach (var (tab, index) in this.statuses.Tabs.WithIndex())
                {
                    if (this.ListTab.GetTabRect(index).Contains(e.Location))
                    {
                        this.RemoveSpecifiedTab(tab.TabName, true);
                        this.SaveConfigsTabs();
                        break;
                    }
                }
            }
        }

        private void ListTab_DoubleClick(object sender, MouseEventArgs e)
            => this.TabRename(this.CurrentTabName, out _);

        private void ListTab_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.settings.Common.TabMouseLock) return;
            if (e.Button == MouseButtons.Left)
            {
                foreach (var i in Enumerable.Range(0, this.statuses.Tabs.Count))
                {
                    if (this.ListTab.GetTabRect(i).Contains(e.Location))
                    {
                        this.tabDrag = true;
                        this.tabMouseDownPoint = e.Location;
                        break;
                    }
                }
            }
            else
            {
                this.tabDrag = false;
            }
        }

        private void ListTab_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TabPage)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void ListTab_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TabPage))) return;

            this.tabDrag = false;
            var tn = "";
            var bef = false;
            var cpos = new Point(e.X, e.Y);
            var spos = this.ListTab.PointToClient(cpos);
            foreach (var (tab, index) in this.statuses.Tabs.WithIndex())
            {
                var rect = this.ListTab.GetTabRect(index);
                if (rect.Contains(spos))
                {
                    tn = tab.TabName;
                    if (spos.X <= (rect.Left + rect.Right) / 2)
                        bef = true;
                    else
                        bef = false;

                    break;
                }
            }

            // タブのないところにドロップ->最後尾へ移動
            if (MyCommon.IsNullOrEmpty(tn))
            {
                var lastTab = this.statuses.Tabs.Last();
                tn = lastTab.TabName;
                bef = false;
            }

            var tp = (TabPage)e.Data.GetData(typeof(TabPage));
            if (tp.Text == tn) return;

            this.ReOrderTab(tp.Text, tn, bef);
        }

        public void ReOrderTab(string targetTabText, string baseTabText, bool isBeforeBaseTab)
        {
            var baseIndex = this.GetTabPageIndex(baseTabText);
            if (baseIndex == -1)
                return;

            var targetIndex = this.GetTabPageIndex(targetTabText);
            if (targetIndex == -1)
                return;

            using (ControlTransaction.Layout(this.ListTab))
            {
                // 選択中のタブを Remove メソッドで取り外すと選択状態が変化して Selecting イベントが発生するが、
                // この時 TabInformations と TabControl の並び順が不一致なままで ListTabSelect メソッドが呼ばれてしまう。
                // これを防ぐために、Remove メソッドを呼ぶ前に選択中のタブを切り替えておく必要がある
                this.ListTab.SelectedIndex = targetIndex == 0 ? 1 : 0;

                var tab = this.statuses.Tabs[targetIndex];
                var tabPage = this.ListTab.TabPages[targetIndex];

                this.ListTab.TabPages.Remove(tabPage);

                if (targetIndex < baseIndex)
                    baseIndex--;

                if (!isBeforeBaseTab)
                    baseIndex++;

                this.statuses.MoveTab(baseIndex, tab);

                this.ListTab.TabPages.Insert(baseIndex, tabPage);
            }

            this.SaveConfigsTabs();
        }

        private void MakeDirectMessageText()
        {
            var selectedPosts = this.CurrentTab.SelectedPosts;
            if (selectedPosts.Length > 1)
                return;

            var post = selectedPosts.Single();
            var text = $"D {post.ScreenName} {this.StatusText.Text}";

            this.inReplyTo = null;
            this.StatusText.Text = text;
            this.StatusText.SelectionStart = text.Length;
            this.StatusText.Focus();
        }

        private void MakeReplyText(bool atAll = false)
        {
            var selectedPosts = this.CurrentTab.SelectedPosts;
            if (selectedPosts.Any(x => x.IsDm))
            {
                this.MakeDirectMessageText();
                return;
            }

            if (selectedPosts.Length == 1)
            {
                var post = selectedPosts.Single();
                var inReplyToStatusId = post.RetweetedId ?? post.StatusId;
                var inReplyToScreenName = post.ScreenName;
                this.inReplyTo = (inReplyToStatusId, inReplyToScreenName);
            }
            else
            {
                this.inReplyTo = null;
            }

            var selfScreenName = this.tw.Username;
            var targetScreenNames = new List<string>();
            foreach (var post in selectedPosts)
            {
                if (post.ScreenName != selfScreenName)
                    targetScreenNames.Add(post.ScreenName);

                if (atAll)
                {
                    foreach (var (_, screenName) in post.ReplyToList)
                    {
                        if (screenName != selfScreenName)
                            targetScreenNames.Add(screenName);
                    }
                }
            }

            if (this.inReplyTo != null)
            {
                var (_, screenName) = this.inReplyTo.Value;
                if (screenName == selfScreenName)
                    targetScreenNames.Insert(0, screenName);
            }

            var text = this.StatusText.Text;
            foreach (var screenName in targetScreenNames.AsEnumerable().Reverse())
            {
                var atText = $"@{screenName} ";
                if (!text.Contains(atText))
                    text = atText + text;
            }

            this.StatusText.Text = text;
            this.StatusText.SelectionStart = text.Length;
            this.StatusText.Focus();
        }

        private void ListTab_MouseUp(object sender, MouseEventArgs e)
            => this.tabDrag = false;

        private int iconCnt = 0;
        private int blinkCnt = 0;
        private bool blink = false;

        private void RefreshTasktrayIcon()
        {
            void EnableTasktrayAnimation()
                => this.TimerRefreshIcon.Enabled = true;

            void DisableTasktrayAnimation()
                => this.TimerRefreshIcon.Enabled = false;

            var busyTasks = this.workerSemaphore.CurrentCount != MaxWorderThreads;
            if (busyTasks)
            {
                this.iconCnt += 1;
                if (this.iconCnt >= this.iconAssets.IconTrayRefresh.Length)
                    this.iconCnt = 0;

                this.NotifyIcon1.Icon = this.iconAssets.IconTrayRefresh[this.iconCnt];
                this.myStatusError = false;
                EnableTasktrayAnimation();
                return;
            }

            var replyIconType = this.settings.Common.ReplyIconState;
            var reply = false;
            if (replyIconType != MyCommon.REPLY_ICONSTATE.None)
            {
                var replyTab = this.statuses.GetTabByType<MentionsTabModel>();
                if (replyTab != null && replyTab.UnreadCount > 0)
                    reply = true;
            }

            if (replyIconType == MyCommon.REPLY_ICONSTATE.BlinkIcon && reply)
            {
                this.blinkCnt += 1;
                if (this.blinkCnt > 10)
                    this.blinkCnt = 0;

                if (this.blinkCnt == 0)
                    this.blink = !this.blink;

                this.NotifyIcon1.Icon = this.blink ? this.iconAssets.IconTrayReplyBlink : this.iconAssets.IconTrayReply;
                EnableTasktrayAnimation();
                return;
            }

            DisableTasktrayAnimation();

            this.iconCnt = 0;
            this.blinkCnt = 0;
            this.blink = false;

            // 優先度：リプライ→エラー→オフライン→アイドル
            // エラーは更新アイコンでクリアされる
            if (replyIconType == MyCommon.REPLY_ICONSTATE.StaticIcon && reply)
                this.NotifyIcon1.Icon = this.iconAssets.IconTrayReply;
            else if (this.myStatusError)
                this.NotifyIcon1.Icon = this.iconAssets.IconTrayError;
            else if (this.myStatusOnline)
                this.NotifyIcon1.Icon = this.iconAssets.IconTray;
            else
                this.NotifyIcon1.Icon = this.iconAssets.IconTrayOffline;
        }

        private void TimerRefreshIcon_Tick(object sender, EventArgs e)
            => this.RefreshTasktrayIcon(); // 200ms

        private void ContextMenuTabProperty_Opening(object sender, CancelEventArgs e)
        {
            // 右クリックの場合はタブ名が設定済。アプリケーションキーの場合は現在のタブを対象とする
            if (MyCommon.IsNullOrEmpty(this.rclickTabName) || sender != this.ContextMenuTabProperty)
                this.rclickTabName = this.CurrentTabName;

            if (this.statuses == null) return;
            if (this.statuses.Tabs == null) return;

            if (!this.statuses.Tabs.TryGetValue(this.rclickTabName, out var tb))
                return;

            this.NotifyDispMenuItem.Checked = tb.Notify;
            this.NotifyTbMenuItem.Checked = tb.Notify;

            this.soundfileListup = true;
            this.SoundFileComboBox.Items.Clear();
            this.SoundFileTbComboBox.Items.Clear();
            this.SoundFileComboBox.Items.Add("");
            this.SoundFileTbComboBox.Items.Add("");
            var oDir = new DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(Application.StartupPath, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (var oFile in oDir.GetFiles("*.wav"))
            {
                this.SoundFileComboBox.Items.Add(oFile.Name);
                this.SoundFileTbComboBox.Items.Add(oFile.Name);
            }
            var idx = this.SoundFileComboBox.Items.IndexOf(tb.SoundFile);
            if (idx == -1) idx = 0;
            this.SoundFileComboBox.SelectedIndex = idx;
            this.SoundFileTbComboBox.SelectedIndex = idx;
            this.soundfileListup = false;
            this.UreadManageMenuItem.Checked = tb.UnreadManage;
            this.UnreadMngTbMenuItem.Checked = tb.UnreadManage;

            this.TabMenuControl(this.rclickTabName);
        }

        private void TabMenuControl(string tabName)
        {
            var tabInfo = this.statuses.GetTabByName(tabName)!;

            this.FilterEditMenuItem.Enabled = true;
            this.EditRuleTbMenuItem.Enabled = true;

            if (tabInfo.IsDefaultTabType)
            {
                this.ProtectTabMenuItem.Enabled = false;
                this.ProtectTbMenuItem.Enabled = false;
            }
            else
            {
                this.ProtectTabMenuItem.Enabled = true;
                this.ProtectTbMenuItem.Enabled = true;
            }

            if (tabInfo.IsDefaultTabType || tabInfo.Protected)
            {
                this.ProtectTabMenuItem.Checked = true;
                this.ProtectTbMenuItem.Checked = true;
                this.DeleteTabMenuItem.Enabled = false;
                this.DeleteTbMenuItem.Enabled = false;
            }
            else
            {
                this.ProtectTabMenuItem.Checked = false;
                this.ProtectTbMenuItem.Checked = false;
                this.DeleteTabMenuItem.Enabled = true;
                this.DeleteTbMenuItem.Enabled = true;
            }
        }

        private void ProtectTabMenuItem_Click(object sender, EventArgs e)
        {
            var checkState = ((ToolStripMenuItem)sender).Checked;

            // チェック状態を同期
            this.ProtectTbMenuItem.Checked = checkState;
            this.ProtectTabMenuItem.Checked = checkState;

            // ロック中はタブの削除を無効化
            this.DeleteTabMenuItem.Enabled = !checkState;
            this.DeleteTbMenuItem.Enabled = !checkState;

            if (MyCommon.IsNullOrEmpty(this.rclickTabName)) return;
            this.statuses.Tabs[this.rclickTabName].Protected = checkState;

            this.SaveConfigsTabs();
        }

        private void UreadManageMenuItem_Click(object sender, EventArgs e)
        {
            this.UreadManageMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.UnreadMngTbMenuItem.Checked = this.UreadManageMenuItem.Checked;

            if (MyCommon.IsNullOrEmpty(this.rclickTabName)) return;
            this.ChangeTabUnreadManage(this.rclickTabName, this.UreadManageMenuItem.Checked);

            this.SaveConfigsTabs();
        }

        public void ChangeTabUnreadManage(string tabName, bool isManage)
        {
            var idx = this.GetTabPageIndex(tabName);
            if (idx == -1)
                return;

            var tab = this.statuses.Tabs[tabName];
            tab.UnreadManage = isManage;

            if (this.settings.Common.TabIconDisp)
            {
                var tabPage = this.ListTab.TabPages[idx];
                if (tab.UnreadCount > 0)
                    tabPage.ImageIndex = 0;
                else
                    tabPage.ImageIndex = -1;
            }

            if (this.CurrentTabName == tabName)
            {
                this.listCache?.PurgeCache();
                this.CurrentListView.Refresh();
            }

            this.SetMainWindowTitle();
            this.SetStatusLabelUrl();
            if (!this.settings.Common.TabIconDisp) this.ListTab.Refresh();
        }

        private void NotifyDispMenuItem_Click(object sender, EventArgs e)
        {
            this.NotifyDispMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.NotifyTbMenuItem.Checked = this.NotifyDispMenuItem.Checked;

            if (MyCommon.IsNullOrEmpty(this.rclickTabName)) return;

            this.statuses.Tabs[this.rclickTabName].Notify = this.NotifyDispMenuItem.Checked;

            this.SaveConfigsTabs();
        }

        private void SoundFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.soundfileListup || MyCommon.IsNullOrEmpty(this.rclickTabName)) return;

            this.statuses.Tabs[this.rclickTabName].SoundFile = (string)((ToolStripComboBox)sender).SelectedItem;

            this.SaveConfigsTabs();
        }

        private void DeleteTabMenuItem_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNullOrEmpty(this.rclickTabName) || sender == this.DeleteTbMenuItem)
                this.rclickTabName = this.CurrentTabName;

            this.RemoveSpecifiedTab(this.rclickTabName, true);
            this.SaveConfigsTabs();
        }

        private void FilterEditMenuItem_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNullOrEmpty(this.rclickTabName)) this.rclickTabName = this.statuses.HomeTab.TabName;

            using (var fltDialog = new FilterDialog())
            {
                fltDialog.Owner = this;
                fltDialog.SetCurrent(this.rclickTabName);
                fltDialog.ShowDialog(this);
            }
            this.TopMost = this.settings.Common.AlwaysTop;

            this.ApplyPostFilters();
            this.SaveConfigsTabs();
        }

        private async void AddTabMenuItem_Click(object sender, EventArgs e)
        {
            string? tabName = null;
            MyCommon.TabUsageType tabUsage;
            using (var inputName = new InputTabName())
            {
                inputName.TabName = this.statuses.MakeTabName("MyTab");
                inputName.IsShowUsage = true;
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel) return;
                tabName = inputName.TabName;
                tabUsage = inputName.Usage;
            }
            this.TopMost = this.settings.Common.AlwaysTop;
            if (!MyCommon.IsNullOrEmpty(tabName))
            {
                // List対応
                ListElement? list = null;
                if (tabUsage == MyCommon.TabUsageType.Lists)
                {
                    using var listAvail = new ListAvailable();
                    if (listAvail.ShowDialog(this) == DialogResult.Cancel)
                        return;
                    if (listAvail.SelectedList == null)
                        return;
                    list = listAvail.SelectedList;
                }

                TabModel tab;
                switch (tabUsage)
                {
                    case MyCommon.TabUsageType.UserDefined:
                        tab = new FilterTabModel(tabName);
                        break;
                    case MyCommon.TabUsageType.PublicSearch:
                        tab = new PublicSearchTabModel(tabName);
                        break;
                    case MyCommon.TabUsageType.Lists:
                        tab = new ListTimelineTabModel(tabName, list!);
                        break;
                    default:
                        return;
                }

                if (!this.statuses.AddTab(tab) || !this.AddNewTab(tab, startup: false))
                {
                    var tmp = string.Format(Properties.Resources.AddTabMenuItem_ClickText1, tabName);
                    MessageBox.Show(tmp, Properties.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    // 成功
                    this.SaveConfigsTabs();

                    var tabIndex = this.statuses.Tabs.Count - 1;

                    if (tabUsage == MyCommon.TabUsageType.PublicSearch)
                    {
                        this.ListTab.SelectedIndex = tabIndex;
                        this.CurrentTabPage.Controls["panelSearch"].Controls["comboSearch"].Focus();
                    }
                    if (tabUsage == MyCommon.TabUsageType.Lists)
                    {
                        this.ListTab.SelectedIndex = tabIndex;
                        await this.RefreshTabAsync(this.CurrentTab);
                    }
                }
            }
        }

        private void TabMenuItem_Click(object sender, EventArgs e)
        {
            // 選択発言を元にフィルタ追加
            foreach (var post in this.CurrentTab.SelectedPosts)
            {
                // タブ選択（or追加）
                if (!this.SelectTab(out var tab))
                    return;

                using (var fltDialog = new FilterDialog())
                {
                    fltDialog.Owner = this;
                    fltDialog.SetCurrent(tab.TabName);

                    if (post.RetweetedBy == null)
                    {
                        fltDialog.AddNewFilter(post.ScreenName, post.TextFromApi);
                    }
                    else
                    {
                        fltDialog.AddNewFilter(post.RetweetedBy, post.TextFromApi);
                    }
                    fltDialog.ShowDialog(this);
                }

                this.TopMost = this.settings.Common.AlwaysTop;
            }

            this.ApplyPostFilters();
            this.SaveConfigsTabs();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // TextBox1でEnterを押してもビープ音が鳴らないようにする
            if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                if (this.StatusText.Focused)
                {
                    var newLine = false;
                    var post = false;

                    if (this.settings.Common.PostCtrlEnter) // Ctrl+Enter投稿時
                    {
                        if (this.StatusText.Multiline)
                        {
                            if ((keyData & Keys.Shift) == Keys.Shift && (keyData & Keys.Control) != Keys.Control) newLine = true;

                            if ((keyData & Keys.Control) == Keys.Control) post = true;
                        }
                        else
                        {
                            if ((keyData & Keys.Control) == Keys.Control) post = true;
                        }
                    }
                    else if (this.settings.Common.PostShiftEnter) // SHift+Enter投稿時
                    {
                        if (this.StatusText.Multiline)
                        {
                            if ((keyData & Keys.Control) == Keys.Control && (keyData & Keys.Shift) != Keys.Shift) newLine = true;

                            if ((keyData & Keys.Shift) == Keys.Shift) post = true;
                        }
                        else
                        {
                            if ((keyData & Keys.Shift) == Keys.Shift) post = true;
                        }
                    }
                    else // Enter投稿時
                    {
                        if (this.StatusText.Multiline)
                        {
                            if ((keyData & Keys.Shift) == Keys.Shift && (keyData & Keys.Control) != Keys.Control) newLine = true;

                            if (((keyData & Keys.Control) != Keys.Control && (keyData & Keys.Shift) != Keys.Shift) ||
                                ((keyData & Keys.Control) == Keys.Control && (keyData & Keys.Shift) == Keys.Shift)) post = true;
                        }
                        else
                        {
                            if (((keyData & Keys.Shift) == Keys.Shift) ||
                                (((keyData & Keys.Control) != Keys.Control) &&
                                ((keyData & Keys.Shift) != Keys.Shift))) post = true;
                        }
                    }

                    if (newLine)
                    {
                        var pos1 = this.StatusText.SelectionStart;
                        if (this.StatusText.SelectionLength > 0)
                        {
                            this.StatusText.Text = this.StatusText.Text.Remove(pos1, this.StatusText.SelectionLength);  // 選択状態文字列削除
                        }
                        this.StatusText.Text = this.StatusText.Text.Insert(pos1, Environment.NewLine);  // 改行挿入
                        this.StatusText.SelectionStart = pos1 + Environment.NewLine.Length;    // カーソルを改行の次の文字へ移動
                        return true;
                    }
                    else if (post)
                    {
                        this.PostButton_Click(this.PostButton, EventArgs.Empty);
                        return true;
                    }
                }
                else
                {
                    var tab = this.CurrentTab;
                    if (tab.TabType == MyCommon.TabUsageType.PublicSearch)
                    {
                        var tabPage = this.CurrentTabPage;
                        if (tabPage.Controls["panelSearch"].Controls["comboSearch"].Focused ||
                            tabPage.Controls["panelSearch"].Controls["comboLang"].Focused)
                        {
                            this.SearchButton_Click(tabPage.Controls["panelSearch"].Controls["comboSearch"], EventArgs.Empty);
                            return true;
                        }
                    }
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private void ReplyAllStripMenuItem_Click(object sender, EventArgs e)
            => this.MakeReplyText(atAll: true);

        private void IDRuleMenuItem_Click(object sender, EventArgs e)
        {
            var tab = this.CurrentTab;
            var selectedPosts = tab.SelectedPosts;

            // 未選択なら処理終了
            if (selectedPosts.Length == 0)
                return;

            var screenNameArray = selectedPosts
                .Select(x => x.RetweetedBy ?? x.ScreenName)
                .ToArray();

            this.AddFilterRuleByScreenName(screenNameArray);

            if (screenNameArray.Length != 0)
            {
                var atids = new List<string>();
                foreach (var screenName in screenNameArray)
                {
                    atids.Add("@" + screenName);
                }
                var cnt = this.AtIdSupl.ItemCount;
                this.AtIdSupl.AddRangeItem(atids.ToArray());
                if (this.AtIdSupl.ItemCount != cnt)
                    this.MarkSettingAtIdModified();
            }
        }

        private void SourceRuleMenuItem_Click(object sender, EventArgs e)
        {
            var tab = this.CurrentTab;
            var selectedPosts = tab.SelectedPosts;

            if (selectedPosts.Length == 0)
                return;

            var sourceArray = selectedPosts.Select(x => x.Source).ToArray();

            this.AddFilterRuleBySource(sourceArray);
        }

        public void AddFilterRuleByScreenName(params string[] screenNameArray)
        {
            // タブ選択（or追加）
            if (!this.SelectTab(out var tab)) return;

            bool mv;
            bool mk;
            if (tab.TabType != MyCommon.TabUsageType.Mute)
            {
                this.MoveOrCopy(out mv, out mk);
            }
            else
            {
                // ミュートタブでは常に MoveMatches を true にする
                mv = true;
                mk = false;
            }

            foreach (var screenName in screenNameArray)
            {
                tab.AddFilter(new PostFilterRule
                {
                    FilterName = screenName,
                    UseNameField = true,
                    MoveMatches = mv,
                    MarkMatches = mk,
                    UseRegex = false,
                    FilterByUrl = false,
                });
            }

            this.ApplyPostFilters();
            this.SaveConfigsTabs();
        }

        public void AddFilterRuleBySource(params string[] sourceArray)
        {
            // タブ選択ダイアログを表示（or追加）
            if (!this.SelectTab(out var filterTab))
                return;

            bool mv;
            bool mk;
            if (filterTab.TabType != MyCommon.TabUsageType.Mute)
            {
                // フィルタ動作選択ダイアログを表示（移動/コピー, マーク有無）
                this.MoveOrCopy(out mv, out mk);
            }
            else
            {
                // ミュートタブでは常に MoveMatches を true にする
                mv = true;
                mk = false;
            }

            // 振り分けルールに追加するSource
            foreach (var source in sourceArray)
            {
                filterTab.AddFilter(new PostFilterRule
                {
                    FilterSource = source,
                    MoveMatches = mv,
                    MarkMatches = mk,
                    UseRegex = false,
                    FilterByUrl = false,
                });
            }

            this.ApplyPostFilters();
            this.SaveConfigsTabs();
        }

        private bool SelectTab([NotNullWhen(true)] out FilterTabModel? tab)
        {
            do
            {
                tab = null;

                // 振り分け先タブ選択
                using (var dialog = new TabsDialog(this.statuses))
                {
                    if (dialog.ShowDialog(this) == DialogResult.Cancel) return false;

                    tab = dialog.SelectedTab;
                }

                this.CurrentTabPage.Focus();
                // 新規タブを選択→タブ作成
                if (tab == null)
                {
                    string tabName;
                    using (var inputName = new InputTabName())
                    {
                        inputName.TabName = this.statuses.MakeTabName("MyTab");
                        inputName.ShowDialog();
                        if (inputName.DialogResult == DialogResult.Cancel) return false;
                        tabName = inputName.TabName;
                    }
                    this.TopMost = this.settings.Common.AlwaysTop;
                    if (!MyCommon.IsNullOrEmpty(tabName))
                    {
                        var newTab = new FilterTabModel(tabName);
                        if (!this.statuses.AddTab(newTab) || !this.AddNewTab(newTab, startup: false))
                        {
                            var tmp = string.Format(Properties.Resources.IDRuleMenuItem_ClickText2, tabName);
                            MessageBox.Show(tmp, Properties.Resources.IDRuleMenuItem_ClickText3, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            // もう一度タブ名入力
                        }
                        else
                        {
                            tab = newTab;
                            return true;
                        }
                    }
                }
                else
                {
                    // 既存タブを選択
                    return true;
                }
            }
            while (true);
        }

        private void MoveOrCopy(out bool move, out bool mark)
        {
            {
                // 移動するか？
                var tmp = string.Format(Properties.Resources.IDRuleMenuItem_ClickText4, Environment.NewLine);
                if (MessageBox.Show(tmp, Properties.Resources.IDRuleMenuItem_ClickText5, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    move = false;
                else
                    move = true;
            }
            if (!move)
            {
                // マークするか？
                var tmp = string.Format(Properties.Resources.IDRuleMenuItem_ClickText6, Environment.NewLine);
                if (MessageBox.Show(tmp, Properties.Resources.IDRuleMenuItem_ClickText7, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    mark = true;
                else
                    mark = false;
            }
            else
            {
                mark = false;
            }
        }

        private void CopySTOTMenuItem_Click(object sender, EventArgs e)
            => this.CopyStot();

        private void CopyURLMenuItem_Click(object sender, EventArgs e)
            => this.CopyIdUri();

        private void SelectAllMenuItem_Click(object sender, EventArgs e)
        {
            if (this.StatusText.Focused)
            {
                // 発言欄でのCtrl+A
                this.StatusText.SelectAll();
            }
            else
            {
                // ListView上でのCtrl+A
                NativeMethods.SelectAllItems(this.CurrentListView);
            }
        }

        private void MoveMiddle()
        {
            ListViewItem item;
            int idx1;
            int idx2;

            var listView = this.CurrentListView;
            if (listView.SelectedIndices.Count == 0) return;

            var idx = listView.SelectedIndices[0];

            item = listView.GetItemAt(0, 25);
            if (item == null)
                idx1 = 0;
            else
                idx1 = item.Index;

            item = listView.GetItemAt(0, listView.ClientSize.Height - 1);
            if (item == null)
                idx2 = listView.VirtualListSize - 1;
            else
                idx2 = item.Index;

            idx -= Math.Abs(idx1 - idx2) / 2;
            if (idx < 0) idx = 0;

            listView.EnsureVisible(listView.VirtualListSize - 1);
            listView.EnsureVisible(idx);
        }

        private async void OpenURLMenuItem_Click(object sender, EventArgs e)
        {
            var linkElements = this.tweetDetailsView.GetLinkElements();

            if (linkElements.Length == 0)
                return;

            var links = new List<OpenUrlItem>(linkElements.Length);

            foreach (var linkElm in linkElements)
            {
                var displayUrl = linkElm.GetAttribute("title");
                var href = linkElm.GetAttribute("href");
                var linkedText = linkElm.InnerText;

                if (MyCommon.IsNullOrEmpty(displayUrl))
                    displayUrl = href;

                links.Add(new OpenUrlItem(linkedText, displayUrl, href));
            }

            string selectedUrl;
            bool isReverseSettings;

            if (links.Count == 1)
            {
                // ツイートに含まれる URL が 1 つのみの場合
                //   => OpenURL ダイアログを表示せずにリンクを開く
                selectedUrl = links[0].Href;

                // Ctrl+E で呼ばれた場合を考慮し isReverseSettings の判定を行わない
                isReverseSettings = false;
            }
            else
            {
                // ツイートに含まれる URL が複数ある場合
                //   => OpenURL を表示しユーザーが選択したリンクを開く
                this.urlDialog.ClearUrl();

                foreach (var link in links)
                    this.urlDialog.AddUrl(link);

                if (this.urlDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                this.TopMost = this.settings.Common.AlwaysTop;

                selectedUrl = this.urlDialog.SelectedUrl;

                // Ctrlを押しながらリンクを開いた場合は、設定と逆の動作をするフラグを true としておく
                isReverseSettings = MyCommon.IsKeyDown(Keys.Control);
            }

            await this.OpenUriAsync(new Uri(selectedUrl), isReverseSettings);
        }

        private void ClearTabMenuItem_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNullOrEmpty(this.rclickTabName)) return;
            this.ClearTab(this.rclickTabName, true);
        }

        private void ClearTab(string tabName, bool showWarning)
        {
            if (showWarning)
            {
                var tmp = string.Format(Properties.Resources.ClearTabMenuItem_ClickText1, Environment.NewLine);
                if (MessageBox.Show(tmp, tabName + " " + Properties.Resources.ClearTabMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }

            this.statuses.ClearTabIds(tabName);
            if (this.CurrentTabName == tabName)
            {
                this.CurrentTab.ClearAnchor();
                this.listCache?.PurgeCache();
                this.listCache?.UpdateListSize();
            }

            var tabIndex = this.statuses.Tabs.IndexOf(tabName);
            var tabPage = this.ListTab.TabPages[tabIndex];
            tabPage.ImageIndex = -1;

            if (!this.settings.Common.TabIconDisp) this.ListTab.Refresh();

            this.SetMainWindowTitle();
            this.SetStatusLabelUrl();
        }

        private static long followers = 0;

        private void SetMainWindowTitle()
        {
            // メインウインドウタイトルの書き換え
            var ttl = new StringBuilder(256);
            var ur = 0;
            var al = 0;
            if (this.settings.Common.DispLatestPost != MyCommon.DispTitleEnum.None &&
                this.settings.Common.DispLatestPost != MyCommon.DispTitleEnum.Post &&
                this.settings.Common.DispLatestPost != MyCommon.DispTitleEnum.Ver &&
                this.settings.Common.DispLatestPost != MyCommon.DispTitleEnum.OwnStatus)
            {
                foreach (var tab in this.statuses.Tabs)
                {
                    ur += tab.UnreadCount;
                    al += tab.AllCount;
                }
            }

            if (this.settings.Common.DispUsername) ttl.Append(this.tw.Username).Append(" - ");
            ttl.Append(ApplicationSettings.ApplicationName);
            ttl.Append("  ");
            switch (this.settings.Common.DispLatestPost)
            {
                case MyCommon.DispTitleEnum.Ver:
                    ttl.Append("Ver:").Append(MyCommon.GetReadableVersion());
                    break;
                case MyCommon.DispTitleEnum.Post:
                    if (this.history != null && this.history.Count > 1)
                        ttl.Append(this.history[this.history.Count - 2].Status.Replace("\r\n", " "));
                    break;
                case MyCommon.DispTitleEnum.UnreadRepCount:
                    ttl.AppendFormat(Properties.Resources.SetMainWindowTitleText1, this.statuses.MentionTab.UnreadCount + this.statuses.DirectMessageTab.UnreadCount);
                    break;
                case MyCommon.DispTitleEnum.UnreadAllCount:
                    ttl.AppendFormat(Properties.Resources.SetMainWindowTitleText2, ur);
                    break;
                case MyCommon.DispTitleEnum.UnreadAllRepCount:
                    ttl.AppendFormat(Properties.Resources.SetMainWindowTitleText3, ur, this.statuses.MentionTab.UnreadCount + this.statuses.DirectMessageTab.UnreadCount);
                    break;
                case MyCommon.DispTitleEnum.UnreadCountAllCount:
                    ttl.AppendFormat(Properties.Resources.SetMainWindowTitleText4, ur, al);
                    break;
                case MyCommon.DispTitleEnum.OwnStatus:
                    if (followers == 0 && this.tw.FollowersCount > 0) followers = this.tw.FollowersCount;
                    ttl.AppendFormat(Properties.Resources.OwnStatusTitle, this.tw.StatusesCount, this.tw.FriendsCount, this.tw.FollowersCount, this.tw.FollowersCount - followers);
                    break;
            }

            try
            {
                this.Text = ttl.ToString();
            }
            catch (AccessViolationException)
            {
                // 原因不明。ポスト内容に依存か？たまーに発生するが再現せず。
            }
        }

        private string GetStatusLabelText()
        {
            // ステータス欄にカウント表示
            // タブ未読数/タブ発言数 全未読数/総発言数 (未読＠＋未読DM数)
            if (this.statuses == null) return "";
            var tbRep = this.statuses.MentionTab;
            var tbDm = this.statuses.DirectMessageTab;
            if (tbRep == null || tbDm == null) return "";
            var urat = tbRep.UnreadCount + tbDm.UnreadCount;
            var ur = 0;
            var al = 0;
            var tur = 0;
            var tal = 0;
            var slbl = new StringBuilder(256);
            try
            {
                foreach (var tab in this.statuses.Tabs)
                {
                    ur += tab.UnreadCount;
                    al += tab.AllCount;
                    if (tab.TabName == this.CurrentTabName)
                    {
                        tur = tab.UnreadCount;
                        tal = tab.AllCount;
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

            this.unreadCounter = ur;
            this.unreadAtCounter = urat;

            var homeTab = this.statuses.HomeTab;

            slbl.AppendFormat(Properties.Resources.SetStatusLabelText1, tur, tal, ur, al, urat, this.postTimestamps.Count, this.favTimestamps.Count, homeTab.TweetsPerHour);
            if (this.settings.Common.TimelinePeriod == 0)
            {
                slbl.Append(Properties.Resources.SetStatusLabelText2);
            }
            else
            {
                slbl.Append(this.settings.Common.TimelinePeriod + Properties.Resources.SetStatusLabelText3);
            }
            return slbl.ToString();
        }

        private async void TwitterApiStatus_AccessLimitUpdated(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired && !this.IsDisposed)
                {
                    await this.InvokeAsync(() => this.TwitterApiStatus_AccessLimitUpdated(sender, e));
                }
                else
                {
                    var endpointName = ((TwitterApiStatus.AccessLimitUpdatedEventArgs)e).EndpointName;
                    this.SetApiStatusLabel(endpointName);
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        private void SetApiStatusLabel(string? endpointName = null)
        {
            var tabType = this.CurrentTab.TabType;

            if (endpointName == null)
            {
                var authByCookie = this.tw.Api.AppToken.AuthType == APIAuthType.TwitterComCookie;

                // 表示中のタブに応じて更新
                endpointName = tabType switch
                {
                    MyCommon.TabUsageType.Home => "/statuses/home_timeline",
                    MyCommon.TabUsageType.UserDefined => "/statuses/home_timeline",
                    MyCommon.TabUsageType.Mentions => "/statuses/mentions_timeline",
                    MyCommon.TabUsageType.Favorites => "/favorites/list",
                    MyCommon.TabUsageType.DirectMessage => "/direct_messages/events/list",
                    MyCommon.TabUsageType.UserTimeline =>
                        authByCookie ? UserTweetsAndRepliesRequest.EndpointName : "/statuses/user_timeline",
                    MyCommon.TabUsageType.Lists =>
                        authByCookie ? ListLatestTweetsTimelineRequest.EndpointName : "/lists/statuses",
                    MyCommon.TabUsageType.PublicSearch =>
                        authByCookie ? SearchTimelineRequest.EndpointName : "/search/tweets",
                    MyCommon.TabUsageType.Related => "/statuses/show/:id",
                    _ => null,
                };
                this.toolStripApiGauge.ApiEndpoint = endpointName;
            }
            else
            {
                var currentEndpointName = this.toolStripApiGauge.ApiEndpoint;
                this.toolStripApiGauge.ApiEndpoint = currentEndpointName;
            }
        }

        private void SetStatusLabelUrl()
            => this.StatusLabelUrl.Text = this.GetStatusLabelText();

        public void SetStatusLabel(string text)
            => this.StatusLabel.Text = text;

        private void SetNotifyIconText()
        {
            var ur = new StringBuilder(64);

            // タスクトレイアイコンのツールチップテキスト書き換え
            // Tween [未読/@]
            ur.Remove(0, ur.Length);
            if (this.settings.Common.DispUsername)
            {
                ur.Append(this.tw.Username);
                ur.Append(" - ");
            }
            ur.Append(ApplicationSettings.ApplicationName);
#if DEBUG
            ur.Append("(Debug Build)");
#endif
            if (this.unreadCounter != -1 && this.unreadAtCounter != -1)
            {
                ur.Append(" [");
                ur.Append(this.unreadCounter);
                ur.Append("/@");
                ur.Append(this.unreadAtCounter);
                ur.Append("]");
            }
            this.NotifyIcon1.Text = ur.ToString();
        }

        internal void CheckReplyTo(string statusText)
        {
            MatchCollection m;
            // ハッシュタグの保存
            m = Regex.Matches(statusText, Twitter.Hashtag, RegexOptions.IgnoreCase);
            var hstr = "";
            foreach (Match hm in m)
            {
                if (!hstr.Contains("#" + hm.Result("$3") + " "))
                {
                    hstr += "#" + hm.Result("$3") + " ";
                    this.HashSupl.AddItem("#" + hm.Result("$3"));
                }
            }
            if (!MyCommon.IsNullOrEmpty(this.HashMgr.UseHash) && !hstr.Contains(this.HashMgr.UseHash + " "))
            {
                hstr += this.HashMgr.UseHash;
            }
            if (!MyCommon.IsNullOrEmpty(hstr)) this.HashMgr.AddHashToHistory(hstr.Trim(), false);

            // 本当にリプライ先指定すべきかどうかの判定
            m = Regex.Matches(statusText, "(^|[ -/:-@[-^`{-~])(?<id>@[a-zA-Z0-9_]+)");

            if (this.settings.Common.UseAtIdSupplement)
            {
                var bCnt = this.AtIdSupl.ItemCount;
                foreach (Match mid in m)
                {
                    this.AtIdSupl.AddItem(mid.Result("${id}"));
                }
                if (bCnt != this.AtIdSupl.ItemCount)
                    this.MarkSettingAtIdModified();
            }

            // リプライ先ステータスIDの指定がない場合は指定しない
            if (this.inReplyTo == null)
                return;

            // 通常Reply
            // 次の条件を満たす場合に in_reply_to_status_id 指定
            // 1. Twitterによりリンクと判定される @idが文中に1つ含まれる (2009/5/28 リンク化される@IDのみカウントするように修正)
            // 2. リプライ先ステータスIDが設定されている(リストをダブルクリックで返信している)
            // 3. 文中に含まれた@idがリプライ先のポスト者のIDと一致する

            if (m != null)
            {
                var inReplyToScreenName = this.inReplyTo.Value.ScreenName;
                if (statusText.StartsWith("@", StringComparison.Ordinal))
                {
                    if (statusText.StartsWith("@" + inReplyToScreenName, StringComparison.Ordinal)) return;
                }
                else
                {
                    foreach (Match mid in m)
                    {
                        if (statusText.Contains("RT " + mid.Result("${id}") + ":") && mid.Result("${id}") == "@" + inReplyToScreenName) return;
                    }
                }
            }

            this.inReplyTo = null;
        }

        private void TweenMain_Resize(object sender, EventArgs e)
        {
            if (!this.initialLayout && this.settings.Common.MinimizeToTray && this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }
            if (this.initialLayout && this.settings.Local != null && this.WindowState == FormWindowState.Normal && this.Visible)
            {
                // 現在の DPI と設定保存時の DPI との比を取得する
                var configScaleFactor = this.settings.Local.GetConfigScaleFactor(this.CurrentAutoScaleDimensions);

                this.ClientSize = ScaleBy(configScaleFactor, this.settings.Local.FormSize);

                // Splitterの位置設定
                var splitterDistance = ScaleBy(configScaleFactor.Height, this.settings.Local.SplitterDistance);
                if (splitterDistance > this.SplitContainer1.Panel1MinSize &&
                    splitterDistance < this.SplitContainer1.Height - this.SplitContainer1.Panel2MinSize - this.SplitContainer1.SplitterWidth)
                {
                    this.SplitContainer1.SplitterDistance = splitterDistance;
                }

                // 発言欄複数行
                this.StatusText.Multiline = this.settings.Local.StatusMultiline;
                if (this.StatusText.Multiline)
                {
                    var statusTextHeight = ScaleBy(configScaleFactor.Height, this.settings.Local.StatusTextHeight);
                    var dis = this.SplitContainer2.Height - statusTextHeight - this.SplitContainer2.SplitterWidth;
                    if (dis > this.SplitContainer2.Panel1MinSize && dis < this.SplitContainer2.Height - this.SplitContainer2.Panel2MinSize - this.SplitContainer2.SplitterWidth)
                    {
                        this.SplitContainer2.SplitterDistance = this.SplitContainer2.Height - statusTextHeight - this.SplitContainer2.SplitterWidth;
                    }
                    this.StatusText.Height = statusTextHeight;
                }
                else
                {
                    if (this.SplitContainer2.Height - this.SplitContainer2.Panel2MinSize - this.SplitContainer2.SplitterWidth > 0)
                    {
                        this.SplitContainer2.SplitterDistance = this.SplitContainer2.Height - this.SplitContainer2.Panel2MinSize - this.SplitContainer2.SplitterWidth;
                    }
                }

                var previewDistance = ScaleBy(configScaleFactor.Width, this.settings.Local.PreviewDistance);
                if (previewDistance > this.SplitContainer3.Panel1MinSize && previewDistance < this.SplitContainer3.Width - this.SplitContainer3.Panel2MinSize - this.SplitContainer3.SplitterWidth)
                {
                    this.SplitContainer3.SplitterDistance = previewDistance;
                }

                // Panel2Collapsed は SplitterDistance の設定を終えるまで true にしない
                this.SplitContainer3.Panel2Collapsed = true;

                this.initialLayout = false;
            }
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.formWindowState = this.WindowState;
            }
        }

        private void PlaySoundMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.PlaySoundMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.PlaySoundFileMenuItem.Checked = this.PlaySoundMenuItem.Checked;
            if (this.PlaySoundMenuItem.Checked)
            {
                this.settings.Common.PlaySound = true;
            }
            else
            {
                this.settings.Common.PlaySound = false;
            }
            this.MarkSettingCommonModified();
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.initialLayout)
                return;

            int splitterDistance;
            switch (this.WindowState)
            {
                case FormWindowState.Normal:
                    splitterDistance = this.SplitContainer1.SplitterDistance;
                    break;
                case FormWindowState.Maximized:
                    // 最大化時は、通常時のウィンドウサイズに換算した SplitterDistance を算出する
                    var normalContainerHeight = this.mySize.Height - this.ToolStripContainer1.TopToolStripPanel.Height - this.ToolStripContainer1.BottomToolStripPanel.Height;
                    splitterDistance = this.SplitContainer1.SplitterDistance - (this.SplitContainer1.Height - normalContainerHeight);
                    splitterDistance = Math.Min(splitterDistance, normalContainerHeight - this.SplitContainer1.SplitterWidth - this.SplitContainer1.Panel2MinSize);
                    break;
                default:
                    return;
            }

            this.mySpDis = splitterDistance;
            this.MarkSettingLocalModified();
        }

        private async Task DoRepliedStatusOpen()
        {
            var currentPost = this.CurrentPost;
            if (this.ExistCurrentPost && currentPost != null && currentPost.InReplyToUser != null && currentPost.InReplyToStatusId != null)
            {
                if (MyCommon.IsKeyDown(Keys.Shift))
                {
                    await MyCommon.OpenInBrowserAsync(this, MyCommon.GetStatusUrl(currentPost.InReplyToUser, currentPost.InReplyToStatusId.ToTwitterStatusId()));
                    return;
                }
                if (this.statuses.Posts.TryGetValue(currentPost.InReplyToStatusId, out var repPost))
                {
                    MessageBox.Show($"{repPost.ScreenName} / {repPost.Nickname}   ({repPost.CreatedAt.ToLocalTimeString()})" + Environment.NewLine + repPost.TextFromApi);
                }
                else
                {
                    foreach (var tb in this.statuses.GetTabsByType(MyCommon.TabUsageType.Lists | MyCommon.TabUsageType.PublicSearch))
                    {
                        if (tb == null || !tb.Contains(currentPost.InReplyToStatusId)) break;
                        repPost = tb.Posts[currentPost.InReplyToStatusId];
                        MessageBox.Show($"{repPost.ScreenName} / {repPost.Nickname}   ({repPost.CreatedAt.ToLocalTimeString()})" + Environment.NewLine + repPost.TextFromApi);
                        return;
                    }
                    await MyCommon.OpenInBrowserAsync(this, MyCommon.GetStatusUrl(currentPost.InReplyToUser, currentPost.InReplyToStatusId.ToTwitterStatusId()));
                }
            }
        }

        private async void RepliedStatusOpenMenuItem_Click(object sender, EventArgs e)
            => await this.DoRepliedStatusOpen();

        private void SplitContainer2_Panel2_Resize(object sender, EventArgs e)
        {
            if (this.initialLayout)
                return; // SettingLocal の反映が完了するまで multiline の判定を行わない

            var multiline = this.SplitContainer2.Panel2.Height > this.SplitContainer2.Panel2MinSize + 2;
            if (multiline != this.StatusText.Multiline)
            {
                this.StatusText.Multiline = multiline;
                this.settings.Local.StatusMultiline = multiline;
                this.MarkSettingLocalModified();
            }
        }

        private void StatusText_MultilineChanged(object sender, EventArgs e)
        {
            if (this.StatusText.Multiline)
                this.StatusText.ScrollBars = ScrollBars.Vertical;
            else
                this.StatusText.ScrollBars = ScrollBars.None;

            if (!this.initialLayout)
                this.MarkSettingLocalModified();
        }

        private void MultiLineMenuItem_Click(object sender, EventArgs e)
        {
            // 発言欄複数行
            var menuItemChecked = ((ToolStripMenuItem)sender).Checked;
            this.StatusText.Multiline = menuItemChecked;
            this.settings.Local.StatusMultiline = menuItemChecked;
            if (menuItemChecked)
            {
                if (this.SplitContainer2.Height - this.mySpDis2 - this.SplitContainer2.SplitterWidth < 0)
                    this.SplitContainer2.SplitterDistance = 0;
                else
                    this.SplitContainer2.SplitterDistance = this.SplitContainer2.Height - this.mySpDis2 - this.SplitContainer2.SplitterWidth;
            }
            else
            {
                this.SplitContainer2.SplitterDistance = this.SplitContainer2.Height - this.SplitContainer2.Panel2MinSize - this.SplitContainer2.SplitterWidth;
            }
            this.MarkSettingLocalModified();
        }

        private async Task<bool> UrlConvertAsync(MyCommon.UrlConverter converterType)
        {
            if (converterType == MyCommon.UrlConverter.Bitly || converterType == MyCommon.UrlConverter.Jmp)
            {
                // OAuth2 アクセストークンまたは API キー (旧方式) のいずれも設定されていなければ短縮しない
                if (MyCommon.IsNullOrEmpty(this.settings.Common.BitlyAccessToken) &&
                    (MyCommon.IsNullOrEmpty(this.settings.Common.BilyUser) || MyCommon.IsNullOrEmpty(this.settings.Common.BitlyPwd)))
                {
                    MessageBox.Show(this, Properties.Resources.UrlConvert_BitlyAuthRequired, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // Converter_Type=Nicomsの場合は、nicovideoのみ短縮する
            // 参考資料 RFC3986 Uniform Resource Identifier (URI): Generic Syntax
            // Appendix A.  Collected ABNF for URI
            // http://www.ietf.org/rfc/rfc3986.txt

            const string nico = @"^https?://[a-z]+\.(nicovideo|niconicommons|nicolive)\.jp/[a-z]+/[a-z0-9]+$";

            string result;
            if (this.StatusText.SelectionLength > 0)
            {
                var tmp = this.StatusText.SelectedText;
                // httpから始まらない場合、ExcludeStringで指定された文字列で始まる場合は対象としない
                if (tmp.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    // 文字列が選択されている場合はその文字列について処理

                    // nico.ms使用、nicovideoにマッチしたら変換
                    if (this.settings.Common.Nicoms && Regex.IsMatch(tmp, nico))
                    {
                        result = Nicoms.Shorten(tmp);
                    }
                    else if (converterType != MyCommon.UrlConverter.Nicoms)
                    {
                        // 短縮URL変換
                        try
                        {
                            var srcUri = new Uri(tmp);
                            var resultUri = await ShortUrl.Instance.ShortenUrlAsync(converterType, srcUri);
                            result = resultUri.AbsoluteUri;
                        }
                        catch (WebApiException e)
                        {
                            this.StatusLabel.Text = converterType + ":" + e.Message;
                            return false;
                        }
                        catch (UriFormatException e)
                        {
                            this.StatusLabel.Text = converterType + ":" + e.Message;
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }

                    if (!MyCommon.IsNullOrEmpty(result))
                    {
                        // 短縮 URL が生成されるまでの間に投稿欄から元の URL が削除されていたら中断する
                        var origUrlIndex = this.StatusText.Text.IndexOf(tmp, StringComparison.Ordinal);
                        if (origUrlIndex == -1)
                            return false;

                        this.StatusText.Select(origUrlIndex, tmp.Length);
                        this.StatusText.SelectedText = result;

                        // undoバッファにセット
                        var undo = new UrlUndo
                        {
                            Before = tmp,
                            After = result,
                        };

                        if (this.urlUndoBuffer == null)
                        {
                            this.urlUndoBuffer = new List<UrlUndo>();
                            this.UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        this.urlUndoBuffer.Add(undo);
                    }
                }
            }
            else
            {
                const string url = @"(?<before>(?:[^\""':!=]|^|\:))" +
                                   @"(?<url>(?<protocol>https?://)" +
                                   @"(?<domain>(?:[\.-]|[^\p{P}\s])+\.[a-z]{2,}(?::[0-9]+)?)" +
                                   @"(?<path>/[a-z0-9!*//();:&=+$/%#\-_.,~@]*[a-z0-9)=#/]?)?" +
                                   @"(?<query>\?[a-z0-9!*//();:&=+$/%#\-_.,~@?]*[a-z0-9_&=#/])?)";
                // 正規表現にマッチしたURL文字列をtinyurl化
                foreach (Match mt in Regex.Matches(this.StatusText.Text, url, RegexOptions.IgnoreCase))
                {
                    if (this.StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal) == -1)
                        continue;
                    var tmp = mt.Result("${url}");
                    if (tmp.StartsWith("w", StringComparison.OrdinalIgnoreCase))
                        tmp = "http://" + tmp;

                    // 選んだURLを選択（？）
                    this.StatusText.Select(this.StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length);

                    // nico.ms使用、nicovideoにマッチしたら変換
                    if (this.settings.Common.Nicoms && Regex.IsMatch(tmp, nico))
                    {
                        result = Nicoms.Shorten(tmp);
                    }
                    else if (converterType != MyCommon.UrlConverter.Nicoms)
                    {
                        // 短縮URL変換
                        try
                        {
                            var srcUri = new Uri(tmp);
                            var resultUri = await ShortUrl.Instance.ShortenUrlAsync(converterType, srcUri);
                            result = resultUri.AbsoluteUri;
                        }
                        catch (HttpRequestException e)
                        {
                            // 例外のメッセージが「Response status code does not indicate success: 500 (Internal Server Error).」
                            // のように長いので「:」が含まれていればそれ以降のみを抽出する
                            var message = e.Message.Split(new[] { ':' }, count: 2).Last();

                            this.StatusLabel.Text = converterType + ":" + message;
                            continue;
                        }
                        catch (WebApiException e)
                        {
                            this.StatusLabel.Text = converterType + ":" + e.Message;
                            continue;
                        }
                        catch (UriFormatException e)
                        {
                            this.StatusLabel.Text = converterType + ":" + e.Message;
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    if (!MyCommon.IsNullOrEmpty(result))
                    {
                        // 短縮 URL が生成されるまでの間に投稿欄から元の URL が削除されていたら中断する
                        var origUrlIndex = this.StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal);
                        if (origUrlIndex == -1)
                            return false;

                        this.StatusText.Select(origUrlIndex, mt.Result("${url}").Length);
                        this.StatusText.SelectedText = result;
                        // undoバッファにセット
                        var undo = new UrlUndo
                        {
                            Before = mt.Result("${url}"),
                            After = result,
                        };

                        if (this.urlUndoBuffer == null)
                        {
                            this.urlUndoBuffer = new List<UrlUndo>();
                            this.UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        this.urlUndoBuffer.Add(undo);
                    }
                }
            }

            return true;
        }

        private void DoUrlUndo()
        {
            if (this.urlUndoBuffer != null)
            {
                var tmp = this.StatusText.Text;
                foreach (var data in this.urlUndoBuffer)
                {
                    tmp = tmp.Replace(data.After, data.Before);
                }
                this.StatusText.Text = tmp;
                this.urlUndoBuffer = null;
                this.UrlUndoToolStripMenuItem.Enabled = false;
                this.StatusText.SelectionStart = 0;
                this.StatusText.SelectionLength = 0;
            }
        }

        private async void TinyURLToolStripMenuItem_Click(object sender, EventArgs e)
            => await this.UrlConvertAsync(MyCommon.UrlConverter.TinyUrl);

        private async void IsgdToolStripMenuItem_Click(object sender, EventArgs e)
            => await this.UrlConvertAsync(MyCommon.UrlConverter.Isgd);

        private async void UxnuMenuItem_Click(object sender, EventArgs e)
            => await this.UrlConvertAsync(MyCommon.UrlConverter.Uxnu);

        private async void UrlConvertAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!await this.UrlConvertAsync(this.settings.Common.AutoShortUrlFirst))
            {
                var rnd = new Random();

                MyCommon.UrlConverter svc;
                // 前回使用した短縮URLサービス以外を選択する
                do
                {
                    svc = (MyCommon.UrlConverter)rnd.Next(System.Enum.GetNames(typeof(MyCommon.UrlConverter)).Length);
                }
                while (svc == this.settings.Common.AutoShortUrlFirst || svc == MyCommon.UrlConverter.Nicoms || svc == MyCommon.UrlConverter.Unu);
                await this.UrlConvertAsync(svc);
            }
        }

        private void UrlUndoToolStripMenuItem_Click(object sender, EventArgs e)
            => this.DoUrlUndo();

        private void NewPostPopMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.NotifyFileMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.NewPostPopMenuItem.Checked = this.NotifyFileMenuItem.Checked;
            this.settings.Common.NewAllPop = this.NewPostPopMenuItem.Checked;
            this.MarkSettingCommonModified();
        }

        private void ListLockMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.ListLockMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.LockListFileMenuItem.Checked = this.ListLockMenuItem.Checked;
            this.settings.Common.ListLock = this.ListLockMenuItem.Checked;
            this.MarkSettingCommonModified();
        }

        private void MenuStrip1_MenuActivate(object sender, EventArgs e)
        {
            // フォーカスがメニューに移る (MenuStrip1.Tag フラグを立てる)
            this.MenuStrip1.Tag = new object();
            this.MenuStrip1.Select(); // StatusText がフォーカスを持っている場合 Leave が発生
        }

        private void MenuStrip1_MenuDeactivate(object sender, EventArgs e)
        {
            var currentTabPage = this.CurrentTabPage;
            if (this.Tag != null) // 設定された戻り先へ遷移
            {
                if (this.Tag == currentTabPage)
                    ((Control)currentTabPage.Tag).Select();
                else
                    ((Control)this.Tag).Select();
            }
            else // 戻り先が指定されていない (初期状態) 場合はタブに遷移
            {
                this.Tag = currentTabPage.Tag;
                ((Control)this.Tag).Select();
            }
            // フォーカスがメニューに遷移したかどうかを表すフラグを降ろす
            this.MenuStrip1.Tag = null;
        }

        private void MyList_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            if (this.Use2ColumnsMode)
            {
                e.Cancel = true;
                return;
            }

            var lst = (DetailsListView)sender;
            var columnsCount = lst.Columns.Count;

            var darr = new int[columnsCount];
            for (var i = 0; i < columnsCount; i++)
                darr[lst.Columns[i].DisplayIndex] = i;

            MyCommon.MoveArrayItem(darr, e.OldDisplayIndex, e.NewDisplayIndex);

            for (var i = 0; i < columnsCount; i++)
                this.settings.Local.ColumnsOrder[darr[i]] = i;

            this.MarkSettingLocalModified();
            this.isColumnChanged = true;
        }

        private void MyList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            var lst = (DetailsListView)sender;
            if (this.settings.Local == null) return;

            var modified = false;
            if (this.Use2ColumnsMode)
            {
                if (this.settings.Local.ColumnsWidth[0] != lst.Columns[0].Width)
                {
                    this.settings.Local.ColumnsWidth[0] = lst.Columns[0].Width;
                    modified = true;
                }
                if (this.settings.Local.ColumnsWidth[2] != lst.Columns[1].Width)
                {
                    this.settings.Local.ColumnsWidth[2] = lst.Columns[1].Width;
                    modified = true;
                }
            }
            else
            {
                var columnsCount = lst.Columns.Count;
                for (var i = 0; i < columnsCount; i++)
                {
                    if (this.settings.Local.ColumnsWidth[i] == lst.Columns[i].Width)
                        continue;

                    this.settings.Local.ColumnsWidth[i] = lst.Columns[i].Width;
                    modified = true;
                }
            }
            if (modified)
            {
                this.MarkSettingLocalModified();
                this.isColumnChanged = true;
            }
        }

        private void SplitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.StatusText.Multiline) this.mySpDis2 = this.StatusText.Height;
            this.MarkSettingLocalModified();
        }

        private void TweenMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (!e.Data.GetDataPresent(DataFormats.Html, false)) // WebBrowserコントロールからの絵文字画像Drag&Dropは弾く
                {
                    this.SelectMedia_DragDrop(e);
                }
            }
            else if (e.Data.GetDataPresent("UniformResourceLocatorW"))
            {
                var (url, title) = GetUrlFromDataObject(e.Data);

                string appendText;
                if (title == null)
                    appendText = url;
                else
                    appendText = title + " " + url;

                if (this.StatusText.TextLength == 0)
                    this.StatusText.Text = appendText;
                else
                    this.StatusText.Text += " " + appendText;
            }
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                var text = (string)e.Data.GetData(DataFormats.UnicodeText);
                if (text != null)
                    this.StatusText.Text += text;
            }
            else if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                var data = (string)e.Data.GetData(DataFormats.StringFormat, true);
                if (data != null) this.StatusText.Text += data;
            }
        }

        /// <summary>
        /// IDataObject から URL とタイトルの対を取得します
        /// </summary>
        /// <remarks>
        /// タイトルのみ取得できなかった場合は Value2 が null のタプルを返すことがあります。
        /// </remarks>
        /// <exception cref="ArgumentException">不正なフォーマットが入力された場合</exception>
        /// <exception cref="NotSupportedException">サポートされていないデータが入力された場合</exception>
        internal static (string Url, string? Title) GetUrlFromDataObject(IDataObject data)
        {
            if (data.GetDataPresent("text/x-moz-url"))
            {
                // Firefox, Google Chrome で利用可能
                // 参照: https://developer.mozilla.org/ja/docs/DragDrop/Recommended_Drag_Types

                using var stream = (MemoryStream)data.GetData("text/x-moz-url");
                var lines = Encoding.Unicode.GetString(stream.ToArray()).TrimEnd('\0').Split('\n');
                if (lines.Length < 2)
                    throw new ArgumentException("不正な text/x-moz-url フォーマットです", nameof(data));

                return (lines[0], lines[1]);
            }
            else if (data.GetDataPresent("IESiteModeToUrl"))
            {
                // Internet Exproler 用
                // 保護モードが有効なデフォルトの IE では DragDrop イベントが発火しないため使えない

                using var stream = (MemoryStream)data.GetData("IESiteModeToUrl");
                var lines = Encoding.Unicode.GetString(stream.ToArray()).TrimEnd('\0').Split('\0');
                if (lines.Length < 2)
                    throw new ArgumentException("不正な IESiteModeToUrl フォーマットです", nameof(data));

                return (lines[0], lines[1]);
            }
            else if (data.GetDataPresent("UniformResourceLocatorW"))
            {
                // それ以外のブラウザ向け

                using var stream = (MemoryStream)data.GetData("UniformResourceLocatorW");
                var url = Encoding.Unicode.GetString(stream.ToArray()).TrimEnd('\0');
                return (url, null);
            }

            throw new NotSupportedException("サポートされていないデータ形式です: " + data.GetFormats()[0]);
        }

        private void TweenMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (!e.Data.GetDataPresent(DataFormats.Html, false)) // WebBrowserコントロールからの絵文字画像Drag&Dropは弾く
                {
                    this.SelectMedia_DragEnter(e);
                    return;
                }
            }
            else if (e.Data.GetDataPresent("UniformResourceLocatorW"))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }
            else if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }

            e.Effect = DragDropEffects.None;
        }

        private void TweenMain_DragOver(object sender, DragEventArgs e)
        {
        }

        public bool IsNetworkAvailable()
        {
            var nw = MyCommon.IsNetworkAvailable();
            this.myStatusOnline = nw;
            return nw;
        }

        public async Task OpenUriAsync(Uri uri, bool isReverseSettings = false)
        {
            var uriStr = uri.AbsoluteUri;

            // OpenTween 内部で使用する URL
            if (uri.Authority == "opentween")
            {
                await this.OpenInternalUriAsync(uri);
                return;
            }

            // ハッシュタグを含む Twitter 検索
            if (uri.Host == "twitter.com" && uri.AbsolutePath == "/search" && uri.Query.Contains("q=%23"))
            {
                // ハッシュタグの場合は、タブで開く
                var unescapedQuery = Uri.UnescapeDataString(uri.Query);
                var pos = unescapedQuery.IndexOf('#');
                if (pos == -1) return;

                var hash = unescapedQuery.Substring(pos);
                this.HashSupl.AddItem(hash);
                this.HashMgr.AddHashToHistory(hash.Trim(), false);
                this.AddNewTabForSearch(hash);
                return;
            }

            // ユーザープロフィールURL
            // フラグが立っている場合は設定と逆の動作をする
            if (this.settings.Common.OpenUserTimeline && !isReverseSettings ||
                !this.settings.Common.OpenUserTimeline && isReverseSettings)
            {
                var userUriMatch = Regex.Match(uriStr, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
                if (userUriMatch.Success)
                {
                    var screenName = userUriMatch.Groups["ScreenName"].Value;
                    if (this.IsTwitterId(screenName))
                    {
                        await this.AddNewTabForUserTimeline(screenName);
                        return;
                    }
                }
            }

            // どのパターンにも該当しないURL
            await MyCommon.OpenInBrowserAsync(this, uriStr);
        }

        /// <summary>
        /// OpenTween 内部の機能を呼び出すための URL を開きます
        /// </summary>
        private async Task OpenInternalUriAsync(Uri uri)
        {
            // ツイートを開く (//opentween/status/:status_id)
            var match = Regex.Match(uri.AbsolutePath, @"^/status/(\d+)$");
            if (match.Success)
            {
                var statusId = new TwitterStatusId(match.Groups[1].Value);
                await this.OpenRelatedTab(statusId);
                return;
            }
        }

        private void ListTabSelect(TabPage tabPage)
        {
            this.SetListProperty();

            var previousTabName = this.CurrentTabName;
            if (this.listViewState.TryGetValue(previousTabName, out var previousListViewState))
                previousListViewState.Save(this.ListLockMenuItem.Checked);

            this.listCache?.PurgeCache();

            this.statuses.SelectTab(tabPage.Text);

            this.InitializeTimelineListView();

            var tab = this.CurrentTab;
            tab.ClearAnchor();

            var listView = this.CurrentListView;

            var currentListViewState = this.listViewState[tab.TabName];
            currentListViewState.Restore(forceScroll: true);

            if (this.Use2ColumnsMode)
            {
                listView.Columns[1].Text = this.columnText[2];
            }
            else
            {
                for (var i = 0; i < listView.Columns.Count; i++)
                {
                    listView.Columns[i].Text = this.columnText[i];
                }
            }
        }

        private void InitializeTimelineListView()
        {
            var listView = this.CurrentListView;
            var tab = this.CurrentTab;

            var newCache = new TimelineListViewCache(listView, tab, this.settings.Common);
            (this.listCache, var oldCache) = (newCache, this.listCache);
            oldCache?.Dispose();

            var newDrawer = new TimelineListViewDrawer(listView, tab, this.listCache, this.iconCache, this.themeManager);
            (this.listDrawer, var oldDrawer) = (newDrawer, this.listDrawer);
            oldDrawer?.Dispose();

            newDrawer.IconSize = this.settings.Common.IconSize;
            newDrawer.UpdateItemHeight();
        }

        private void ListTab_Selecting(object sender, TabControlCancelEventArgs e)
            => this.ListTabSelect(e.TabPage);

        private void SelectListItem(DetailsListView lView, int index)
        {
            // 単一
            var bnd = new Rectangle();
            var flg = false;
            var item = lView.FocusedItem;
            if (item != null)
            {
                bnd = item.Bounds;
                flg = true;
            }

            do
            {
                lView.SelectedIndices.Clear();
            }
            while (lView.SelectedIndices.Count > 0);
            item = lView.Items[index];
            item.Selected = true;
            item.Focused = true;

            if (flg) lView.Invalidate(bnd);
        }

        private async void TweenMain_Shown(object sender, EventArgs e)
        {
            this.NotifyIcon1.Visible = true;

            if (this.IsNetworkAvailable())
            {
                var loadTasks = new TaskCollection();

                loadTasks.Add(new[]
                {
                    this.RefreshMuteUserIdsAsync,
                    this.RefreshBlockIdsAsync,
                    this.RefreshNoRetweetIdsAsync,
                    this.RefreshTwitterConfigurationAsync,
                    this.RefreshTabAsync<HomeTabModel>,
                    this.RefreshTabAsync<MentionsTabModel>,
                    this.RefreshTabAsync<DirectMessagesTabModel>,
                    this.RefreshTabAsync<PublicSearchTabModel>,
                    this.RefreshTabAsync<UserTimelineTabModel>,
                    this.RefreshTabAsync<ListTimelineTabModel>,
                });

                if (this.settings.Common.StartupFollowers)
                    loadTasks.Add(this.RefreshFollowerIdsAsync);

                if (this.settings.Common.GetFav)
                    loadTasks.Add(this.RefreshTabAsync<FavoritesTabModel>);

                var allTasks = loadTasks.RunAll();

                var i = 0;
                while (true)
                {
                    var timeout = Task.Delay(5000);
                    if (await Task.WhenAny(allTasks, timeout) != timeout)
                        break;

                    i += 1;
                    if (i > 24) break; // 120秒間初期処理が終了しなかったら強制的に打ち切る

                    if (MyCommon.EndingFlag)
                        return;
                }

                if (MyCommon.EndingFlag) return;

                if (ApplicationSettings.VersionInfoUrl != null)
                {
                    // バージョンチェック（引数：起動時チェックの場合はtrue･･･チェック結果のメッセージを表示しない）
                    if (this.settings.Common.StartupVersion)
                        await this.CheckNewVersion(true);
                }
                else
                {
                    // ApplicationSetting.cs の設定により更新チェックが無効化されている場合
                    this.VerUpMenuItem.Enabled = false;
                    this.VerUpMenuItem.Available = false;
                    this.ToolStripSeparator16.Available = false; // VerUpMenuItem の一つ上にあるセパレータ
                }

                // 権限チェック read/write権限(xAuthで取得したトークン)の場合は再認証を促す
                if (MyCommon.TwitterApiInfo.AccessLevel == TwitterApiAccessLevel.ReadWrite)
                {
                    MessageBox.Show(Properties.Resources.ReAuthorizeText);
                    this.SettingStripMenuItem_Click(this.SettingStripMenuItem, EventArgs.Empty);
                }

                // 取得失敗の場合は再試行する
                var reloadTasks = new TaskCollection();

                if (!this.tw.GetFollowersSuccess && this.settings.Common.StartupFollowers)
                    reloadTasks.Add(() => this.RefreshFollowerIdsAsync());

                if (!this.tw.GetNoRetweetSuccess)
                    reloadTasks.Add(() => this.RefreshNoRetweetIdsAsync());

                if (this.tw.Configuration.PhotoSizeLimit == 0)
                    reloadTasks.Add(() => this.RefreshTwitterConfigurationAsync());

                await reloadTasks.RunAll();
            }

            this.initial = false;

            this.timelineScheduler.Enabled = true;
        }

        private async Task DoGetFollowersMenu()
        {
            await this.RefreshFollowerIdsAsync();
            this.DispSelectedPost(true);
        }

        private async void GetFollowersAllToolStripMenuItem_Click(object sender, EventArgs e)
            => await this.DoGetFollowersMenu();

        private void ReTweetUnofficialStripMenuItem_Click(object sender, EventArgs e)
            => this.DoReTweetUnofficial();

        private async Task DoReTweetOfficial(bool isConfirm)
        {
            // 公式RT
            if (this.ExistCurrentPost)
            {
                var selectedPosts = this.CurrentTab.SelectedPosts;

                if (selectedPosts.Any(x => !x.CanRetweetBy(this.tw.UserId)))
                {
                    if (selectedPosts.Any(x => x.IsProtect))
                        MessageBox.Show("Protected.");

                    this.doFavRetweetFlags = false;
                    return;
                }

                if (selectedPosts.Length > 15)
                {
                    MessageBox.Show(Properties.Resources.RetweetLimitText);
                    this.doFavRetweetFlags = false;
                    return;
                }
                else if (selectedPosts.Length > 1)
                {
                    var questionText = Properties.Resources.RetweetQuestion2;
                    if (this.doFavRetweetFlags) questionText = Properties.Resources.FavoriteRetweetQuestionText1;
                    switch (MessageBox.Show(questionText, "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Cancel:
                        case DialogResult.No:
                            this.doFavRetweetFlags = false;
                            return;
                    }
                }
                else
                {
                    if (!this.settings.Common.RetweetNoConfirm)
                    {
                        var questiontext = Properties.Resources.RetweetQuestion1;
                        if (this.doFavRetweetFlags) questiontext = Properties.Resources.FavoritesRetweetQuestionText2;
                        if (isConfirm && MessageBox.Show(questiontext, "Retweet", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        {
                            this.doFavRetweetFlags = false;
                            return;
                        }
                    }
                }

                var statusIds = selectedPosts.Select(x => x.StatusId).ToList();

                await this.RetweetAsync(statusIds);
            }
        }

        private async void ReTweetStripMenuItem_Click(object sender, EventArgs e)
            => await this.DoReTweetOfficial(true);

        private async Task FavoritesRetweetOfficial()
        {
            if (!this.ExistCurrentPost) return;
            this.doFavRetweetFlags = true;

            var tasks = new TaskCollection();
            tasks.Add(() => this.DoReTweetOfficial(true));

            if (this.doFavRetweetFlags)
            {
                this.doFavRetweetFlags = false;
                tasks.Add(() => this.FavoriteChange(true, false));
            }

            await tasks.RunAll();
        }

        private async Task FavoritesRetweetUnofficial()
        {
            var post = this.CurrentPost;
            if (this.ExistCurrentPost && post != null && !post.IsDm)
            {
                this.doFavRetweetFlags = true;
                var favoriteTask = this.FavoriteChange(true);
                if (!post.IsProtect && this.doFavRetweetFlags)
                {
                    this.doFavRetweetFlags = false;
                    this.DoReTweetUnofficial();
                }

                await favoriteTask;
            }
        }

        /// <summary>
        /// TweetFormatterクラスによって整形された状態のHTMLを、非公式RT用に元のツイートに復元します
        /// </summary>
        /// <param name="statusHtml">TweetFormatterによって整形された状態のHTML</param>
        /// <param name="multiline">trueであればBRタグを改行に、falseであればスペースに変換します</param>
        /// <returns>復元されたツイート本文</returns>
        internal static string CreateRetweetUnofficial(string statusHtml, bool multiline)
        {
            // TweetFormatterクラスによって整形された状態のHTMLを元のツイートに復元します

            // 通常の URL
            statusHtml = Regex.Replace(statusHtml, """<a href="(?<href>.+?)" title="(?<title>.+?)">(?<text>.+?)</a>""", "${title}");
            // メンション
            statusHtml = Regex.Replace(statusHtml, """<a class="mention" href="(?<href>.+?)">(?<text>.+?)</a>""", "${text}");
            // ハッシュタグ
            statusHtml = Regex.Replace(statusHtml, """<a class="hashtag" href="(?<href>.+?)">(?<text>.+?)</a>""", "${text}");
            // 絵文字
            statusHtml = Regex.Replace(statusHtml, """<img class="emoji" src=".+?" alt="(?<text>.+?)" />""", "${text}");

            // <br> 除去
            if (multiline)
                statusHtml = statusHtml.Replace("<br>", Environment.NewLine);
            else
                statusHtml = statusHtml.Replace("<br>", " ");

            // &nbsp; は本来であれば U+00A0 (NON-BREAK SPACE) に置換すべきですが、
            // 現状では半角スペースの代用として &nbsp; を使用しているため U+0020 に置換します
            statusHtml = statusHtml.Replace("&nbsp;", " ");

            return WebUtility.HtmlDecode(statusHtml);
        }

        private void DumpPostClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tweetDetailsView.DumpPostClass = this.DumpPostClassToolStripMenuItem.Checked;

            if (this.CurrentPost != null)
                this.DispSelectedPost(true);
        }

        private void MenuItemHelp_DropDownOpening(object sender, EventArgs e)
        {
            if (MyCommon.DebugBuild || MyCommon.IsKeyDown(Keys.CapsLock, Keys.Control, Keys.Shift))
                this.DebugModeToolStripMenuItem.Visible = true;
            else
                this.DebugModeToolStripMenuItem.Visible = false;
        }

        private void UrlMultibyteSplitMenuItem_CheckedChanged(object sender, EventArgs e)
            => this.urlMultibyteSplit = ((ToolStripMenuItem)sender).Checked;

        private void PreventSmsCommandMenuItem_CheckedChanged(object sender, EventArgs e)
            => this.preventSmsCommand = ((ToolStripMenuItem)sender).Checked;

        private void UrlAutoShortenMenuItem_CheckedChanged(object sender, EventArgs e)
            => this.settings.Common.UrlConvertAuto = ((ToolStripMenuItem)sender).Checked;

        private void IdeographicSpaceToSpaceMenuItem_Click(object sender, EventArgs e)
        {
            this.settings.Common.WideSpaceConvert = ((ToolStripMenuItem)sender).Checked;
            this.MarkSettingCommonModified();
        }

        private void FocusLockMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.settings.Common.FocusLockToStatusText = ((ToolStripMenuItem)sender).Checked;
            this.MarkSettingCommonModified();
        }

        private void PostModeMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.UrlMultibyteSplitMenuItem.Checked = this.urlMultibyteSplit;
            this.PreventSmsCommandMenuItem.Checked = this.preventSmsCommand;
            this.UrlAutoShortenMenuItem.Checked = this.settings.Common.UrlConvertAuto;
            this.IdeographicSpaceToSpaceMenuItem.Checked = this.settings.Common.WideSpaceConvert;
            this.MultiLineMenuItem.Checked = this.settings.Local.StatusMultiline;
            this.FocusLockMenuItem.Checked = this.settings.Common.FocusLockToStatusText;
        }

        private void ContextMenuPostMode_Opening(object sender, CancelEventArgs e)
        {
            this.UrlMultibyteSplitPullDownMenuItem.Checked = this.urlMultibyteSplit;
            this.PreventSmsCommandPullDownMenuItem.Checked = this.preventSmsCommand;
            this.UrlAutoShortenPullDownMenuItem.Checked = this.settings.Common.UrlConvertAuto;
            this.IdeographicSpaceToSpacePullDownMenuItem.Checked = this.settings.Common.WideSpaceConvert;
            this.MultiLinePullDownMenuItem.Checked = this.settings.Local.StatusMultiline;
            this.FocusLockPullDownMenuItem.Checked = this.settings.Common.FocusLockToStatusText;
        }

        private void TraceOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.TraceOutToolStripMenuItem.Checked)
                MyCommon.TraceFlag = true;
            else
                MyCommon.TraceFlag = false;
        }

        private void TweenMain_Deactivate(object sender, EventArgs e)
            => this.StatusText_Leave(this.StatusText, EventArgs.Empty); // 画面が非アクティブになったら、発言欄の背景色をデフォルトへ

        private void TabRenameMenuItem_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNullOrEmpty(this.rclickTabName)) return;

            _ = this.TabRename(this.rclickTabName, out _);
        }

        private async void BitlyToolStripMenuItem_Click(object sender, EventArgs e)
            => await this.UrlConvertAsync(MyCommon.UrlConverter.Bitly);

        private async void JmpToolStripMenuItem_Click(object sender, EventArgs e)
            => await this.UrlConvertAsync(MyCommon.UrlConverter.Jmp);

        private async void ApiUsageInfoMenuItem_Click(object sender, EventArgs e)
        {
            TwitterApiStatus? apiStatus;

            using (var dialog = new WaitingDialog(Properties.Resources.ApiInfo6))
            {
                var cancellationToken = dialog.EnableCancellation();

                try
                {
                    var task = this.tw.GetInfoApi();
                    apiStatus = await dialog.WaitForAsync(this, task);
                }
                catch (WebApiException)
                {
                    apiStatus = null;
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                if (apiStatus == null)
                {
                    MessageBox.Show(Properties.Resources.ApiInfo5, Properties.Resources.ApiInfo4, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            using var apiDlg = new ApiInfoDialog();
            apiDlg.ShowDialog(this);
        }

        private async void FollowCommandMenuItem_Click(object sender, EventArgs e)
        {
            var id = this.CurrentPost?.ScreenName ?? "";

            await this.FollowCommand(id);
        }

        internal async Task FollowCommand(string id)
        {
            using (var inputName = new InputTabName())
            {
                inputName.FormTitle = "Follow";
                inputName.FormDescription = Properties.Resources.FRMessage1;
                inputName.TabName = id;

                if (inputName.ShowDialog(this) != DialogResult.OK)
                    return;
                if (string.IsNullOrWhiteSpace(inputName.TabName))
                    return;

                id = inputName.TabName.Trim();
            }

            using (var dialog = new WaitingDialog(Properties.Resources.FollowCommandText1))
            {
                try
                {
                    var task = this.tw.Api.FriendshipsCreate(id).IgnoreResponse();
                    await dialog.WaitForAsync(this, task);
                }
                catch (WebApiException ex)
                {
                    MessageBox.Show(Properties.Resources.FRMessage2 + ex.Message);
                    return;
                }
            }

            MessageBox.Show(Properties.Resources.FRMessage3);
        }

        private async void RemoveCommandMenuItem_Click(object sender, EventArgs e)
        {
            var id = this.CurrentPost?.ScreenName ?? "";

            await this.RemoveCommand(id, false);
        }

        internal async Task RemoveCommand(string id, bool skipInput)
        {
            if (!skipInput)
            {
                using var inputName = new InputTabName();
                inputName.FormTitle = "Unfollow";
                inputName.FormDescription = Properties.Resources.FRMessage1;
                inputName.TabName = id;

                if (inputName.ShowDialog(this) != DialogResult.OK)
                    return;
                if (string.IsNullOrWhiteSpace(inputName.TabName))
                    return;

                id = inputName.TabName.Trim();
            }

            using (var dialog = new WaitingDialog(Properties.Resources.RemoveCommandText1))
            {
                try
                {
                    var task = this.tw.Api.FriendshipsDestroy(id).IgnoreResponse();
                    await dialog.WaitForAsync(this, task);
                }
                catch (WebApiException ex)
                {
                    MessageBox.Show(Properties.Resources.FRMessage2 + ex.Message);
                    return;
                }
            }

            MessageBox.Show(Properties.Resources.FRMessage3);
        }

        private async void FriendshipMenuItem_Click(object sender, EventArgs e)
        {
            var id = this.CurrentPost?.ScreenName ?? "";

            await this.ShowFriendship(id);
        }

        internal async Task ShowFriendship(string id)
        {
            using (var inputName = new InputTabName())
            {
                inputName.FormTitle = "Show Friendships";
                inputName.FormDescription = Properties.Resources.FRMessage1;
                inputName.TabName = id;

                if (inputName.ShowDialog(this) != DialogResult.OK)
                    return;
                if (string.IsNullOrWhiteSpace(inputName.TabName))
                    return;

                id = inputName.TabName.Trim();
            }

            bool isFollowing, isFollowed;

            using (var dialog = new WaitingDialog(Properties.Resources.ShowFriendshipText1))
            {
                var cancellationToken = dialog.EnableCancellation();

                try
                {
                    var task = this.tw.Api.FriendshipsShow(this.tw.Username, id);
                    var friendship = await dialog.WaitForAsync(this, task);

                    isFollowing = friendship.Relationship.Source.Following;
                    isFollowed = friendship.Relationship.Source.FollowedBy;
                }
                catch (WebApiException ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                        MessageBox.Show($"Err:{ex.Message}(FriendshipsShow)");
                    return;
                }

                if (cancellationToken.IsCancellationRequested)
                    return;
            }

            string result;
            if (isFollowing)
            {
                result = Properties.Resources.GetFriendshipInfo1 + System.Environment.NewLine;
            }
            else
            {
                result = Properties.Resources.GetFriendshipInfo2 + System.Environment.NewLine;
            }
            if (isFollowed)
            {
                result += Properties.Resources.GetFriendshipInfo3;
            }
            else
            {
                result += Properties.Resources.GetFriendshipInfo4;
            }
            result = id + Properties.Resources.GetFriendshipInfo5 + System.Environment.NewLine + result;
            MessageBox.Show(result);
        }

        internal async Task ShowFriendship(string[] ids)
        {
            foreach (var id in ids)
            {
                bool isFollowing, isFollowed;

                using (var dialog = new WaitingDialog(Properties.Resources.ShowFriendshipText1))
                {
                    var cancellationToken = dialog.EnableCancellation();

                    try
                    {
                        var task = this.tw.Api.FriendshipsShow(this.tw.Username, id);
                        var friendship = await dialog.WaitForAsync(this, task);

                        isFollowing = friendship.Relationship.Source.Following;
                        isFollowed = friendship.Relationship.Source.FollowedBy;
                    }
                    catch (WebApiException ex)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                            MessageBox.Show($"Err:{ex.Message}(FriendshipsShow)");
                        return;
                    }

                    if (cancellationToken.IsCancellationRequested)
                        return;
                }

                var result = "";
                var ff = "";

                ff = "  ";
                if (isFollowing)
                {
                    ff += Properties.Resources.GetFriendshipInfo1;
                }
                else
                {
                    ff += Properties.Resources.GetFriendshipInfo2;
                }

                ff += System.Environment.NewLine + "  ";
                if (isFollowed)
                {
                    ff += Properties.Resources.GetFriendshipInfo3;
                }
                else
                {
                    ff += Properties.Resources.GetFriendshipInfo4;
                }
                result += id + Properties.Resources.GetFriendshipInfo5 + System.Environment.NewLine + ff;
                if (isFollowing)
                {
                    if (MessageBox.Show(
                        Properties.Resources.GetFriendshipInfo7 + System.Environment.NewLine + result,
                        Properties.Resources.GetFriendshipInfo8,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        await this.RemoveCommand(id, true);
                    }
                }
                else
                {
                    MessageBox.Show(result);
                }
            }
        }

        private async void OwnStatusMenuItem_Click(object sender, EventArgs e)
            => await this.DoShowUserStatus(this.tw.Username, false);

        // TwitterIDでない固定文字列を調べる（文字列検証のみ　実際に取得はしない）
        // URLから切り出した文字列を渡す

        public bool IsTwitterId(string name)
        {
            if (this.tw.Configuration.NonUsernamePaths == null || this.tw.Configuration.NonUsernamePaths.Length == 0)
                return !Regex.Match(name, @"^(about|jobs|tos|privacy|who_to_follow|download|messages)$", RegexOptions.IgnoreCase).Success;
            else
                return !this.tw.Configuration.NonUsernamePaths.Contains(name, StringComparer.InvariantCultureIgnoreCase);
        }

        private void DoQuoteOfficial()
        {
            var post = this.CurrentPost;
            if (this.ExistCurrentPost && post != null)
            {
                if (post.IsDm || !this.StatusText.Enabled)
                    return;

                if (post.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }

                var selection = (this.StatusText.SelectionStart, this.StatusText.SelectionLength);

                this.inReplyTo = null;

                this.StatusText.Text += " " + MyCommon.GetStatusUrl(post);

                (this.StatusText.SelectionStart, this.StatusText.SelectionLength) = selection;
                this.StatusText.Focus();
            }
        }

        private void DoReTweetUnofficial()
        {
            // RT @id:内容
            var post = this.CurrentPost;
            if (this.ExistCurrentPost && post != null)
            {
                if (post.IsDm || !this.StatusText.Enabled)
                    return;

                if (post.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }
                var rtdata = post.Text;
                rtdata = CreateRetweetUnofficial(rtdata, this.StatusText.Multiline);

                var selection = (this.StatusText.SelectionStart, this.StatusText.SelectionLength);

                // 投稿時に in_reply_to_status_id を付加する
                var inReplyToStatusId = post.RetweetedId ?? post.StatusId;
                var inReplyToScreenName = post.ScreenName;
                this.inReplyTo = (inReplyToStatusId, inReplyToScreenName);

                this.StatusText.Text += " RT @" + post.ScreenName + ": " + rtdata;

                (this.StatusText.SelectionStart, this.StatusText.SelectionLength) = selection;
                this.StatusText.Focus();
            }
        }

        private void QuoteStripMenuItem_Click(object sender, EventArgs e)
            => this.DoQuoteOfficial();

        private async void SearchButton_Click(object sender, EventArgs e)
        {
            // 公式検索
            var pnl = ((Control)sender).Parent;
            if (pnl == null) return;
            var tbName = pnl.Parent.Text;
            var tb = (PublicSearchTabModel)this.statuses.Tabs[tbName];
            var cmb = (ComboBox)pnl.Controls["comboSearch"];
            var cmbLang = (ComboBox)pnl.Controls["comboLang"];
            cmb.Text = cmb.Text.Trim();
            // 検索式演算子 OR についてのみ大文字しか認識しないので強制的に大文字とする
            var quote = false;
            var buf = new StringBuilder();
            var c = cmb.Text.ToCharArray();
            for (var cnt = 0; cnt < cmb.Text.Length; cnt++)
            {
                if (cnt > cmb.Text.Length - 4)
                {
                    buf.Append(cmb.Text.Substring(cnt));
                    break;
                }
                if (c[cnt] == '"')
                {
                    quote = !quote;
                }
                else
                {
                    if (!quote && cmb.Text.Substring(cnt, 4).Equals(" or ", StringComparison.OrdinalIgnoreCase))
                    {
                        buf.Append(" OR ");
                        cnt += 3;
                        continue;
                    }
                }
                buf.Append(c[cnt]);
            }
            cmb.Text = buf.ToString();

            var listView = (DetailsListView)pnl.Parent.Tag;

            var queryChanged = tb.SearchWords != cmb.Text || tb.SearchLang != cmbLang.Text;

            tb.SearchWords = cmb.Text;
            tb.SearchLang = cmbLang.Text;
            if (MyCommon.IsNullOrEmpty(cmb.Text))
            {
                listView.Focus();
                this.SaveConfigsTabs();
                return;
            }
            if (queryChanged)
            {
                var idx = cmb.Items.IndexOf(tb.SearchWords);
                if (idx > -1) cmb.Items.RemoveAt(idx);
                cmb.Items.Insert(0, tb.SearchWords);
                cmb.Text = tb.SearchWords;
                cmb.SelectAll();
                this.statuses.ClearTabIds(tbName);
                this.listCache?.PurgeCache();
                this.listCache?.UpdateListSize();
                this.SaveConfigsTabs();   // 検索条件の保存
            }

            listView.Focus();
            await this.RefreshTabAsync(tb);
        }

        private async void RefreshMoreStripMenuItem_Click(object sender, EventArgs e)
            => await this.DoRefreshMore(); // もっと前を取得

        /// <summary>
        /// 指定されたタブのListTabにおける位置を返します
        /// </summary>
        /// <remarks>
        /// 非表示のタブについて -1 が返ることを常に考慮して下さい
        /// </remarks>
        public int GetTabPageIndex(string tabName)
            => this.statuses.Tabs.IndexOf(tabName);

        private void UndoRemoveTabMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var restoredTab = this.statuses.UndoRemovedTab();
                this.AddNewTab(restoredTab, startup: false);

                var tabIndex = this.statuses.Tabs.Count - 1;
                this.ListTab.SelectedIndex = tabIndex;

                this.SaveConfigsTabs();
            }
            catch (TabException ex)
            {
                MessageBox.Show(this, ex.Message, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DoMoveToRTHome()
        {
            var post = this.CurrentPost;
            if (post != null && post.RetweetedId != null)
                await MyCommon.OpenInBrowserAsync(this, "https://twitter.com/" + post.RetweetedBy);
        }

        private async void RetweetedByOpenInBrowserMenuItem_Click(object sender, EventArgs e)
            => await this.DoMoveToRTHome();

        private void AuthorListManageMenuItem_Click(object sender, EventArgs e)
        {
            var screenName = this.CurrentPost?.ScreenName;
            if (screenName != null)
                this.ListManageUserContext(screenName);
        }

        private void RetweetedByListManageMenuItem_Click(object sender, EventArgs e)
        {
            var screenName = this.CurrentPost?.RetweetedBy;
            if (screenName != null)
                this.ListManageUserContext(screenName);
        }

        public void ListManageUserContext(string screenName)
        {
            using var listSelectForm = new MyLists(screenName, this.tw.Api);
            listSelectForm.ShowDialog(this);
        }

        private void SearchControls_Enter(object sender, EventArgs e)
        {
            var pnl = (Control)sender;
            foreach (Control ctl in pnl.Controls)
            {
                ctl.TabStop = true;
            }
        }

        private void SearchControls_Leave(object sender, EventArgs e)
        {
            var pnl = (Control)sender;
            foreach (Control ctl in pnl.Controls)
            {
                ctl.TabStop = false;
            }
        }

        private void PublicSearchQueryMenuItem_Click(object sender, EventArgs e)
        {
            var tab = this.CurrentTab;
            if (tab.TabType != MyCommon.TabUsageType.PublicSearch) return;
            this.CurrentTabPage.Controls["panelSearch"].Controls["comboSearch"].Focus();
        }

        private void StatusLabel_DoubleClick(object sender, EventArgs e)
            => MessageBox.Show(this.StatusLabel.TextHistory, "Logs", MessageBoxButtons.OK, MessageBoxIcon.None);

        private void HashManageMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult rslt;
            try
            {
                rslt = this.HashMgr.ShowDialog();
            }
            catch (Exception)
            {
                return;
            }
            this.TopMost = this.settings.Common.AlwaysTop;
            if (rslt == DialogResult.Cancel) return;
            if (!MyCommon.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                this.HashStripSplitButton.Text = this.HashMgr.UseHash;
                this.HashTogglePullDownMenuItem.Checked = true;
                this.HashToggleMenuItem.Checked = true;
            }
            else
            {
                this.HashStripSplitButton.Text = "#[-]";
                this.HashTogglePullDownMenuItem.Checked = false;
                this.HashToggleMenuItem.Checked = false;
            }

            this.MarkSettingCommonModified();
            this.StatusText_TextChanged(this.StatusText, EventArgs.Empty);
        }

        private void HashToggleMenuItem_Click(object sender, EventArgs e)
        {
            this.HashMgr.ToggleHash();
            if (!MyCommon.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                this.HashStripSplitButton.Text = this.HashMgr.UseHash;
                this.HashToggleMenuItem.Checked = true;
                this.HashTogglePullDownMenuItem.Checked = true;
            }
            else
            {
                this.HashStripSplitButton.Text = "#[-]";
                this.HashToggleMenuItem.Checked = false;
                this.HashTogglePullDownMenuItem.Checked = false;
            }
            this.MarkSettingCommonModified();
            this.StatusText_TextChanged(this.StatusText, EventArgs.Empty);
        }

        private void HashStripSplitButton_ButtonClick(object sender, EventArgs e)
            => this.HashToggleMenuItem_Click(this.HashToggleMenuItem, EventArgs.Empty);

        public void SetPermanentHashtag(string hashtag)
        {
            this.HashMgr.SetPermanentHash("#" + hashtag);
            this.HashStripSplitButton.Text = this.HashMgr.UseHash;
            this.HashTogglePullDownMenuItem.Checked = true;
            this.HashToggleMenuItem.Checked = true;
            // 使用ハッシュタグとして設定
            this.MarkSettingCommonModified();
        }

        private void MenuItemOperate_DropDownOpening(object sender, EventArgs e)
        {
            var tab = this.CurrentTab;
            var post = this.CurrentPost;
            if (!this.ExistCurrentPost)
            {
                this.ReplyOpMenuItem.Enabled = false;
                this.ReplyAllOpMenuItem.Enabled = false;
                this.DmOpMenuItem.Enabled = false;
                this.CreateTabRuleOpMenuItem.Enabled = false;
                this.CreateIdRuleOpMenuItem.Enabled = false;
                this.CreateSourceRuleOpMenuItem.Enabled = false;
                this.ReadOpMenuItem.Enabled = false;
                this.UnreadOpMenuItem.Enabled = false;
                this.AuthorMenuItem.Visible = false;
                this.RetweetedByMenuItem.Visible = false;
            }
            else
            {
                this.ReplyOpMenuItem.Enabled = true;
                this.ReplyAllOpMenuItem.Enabled = true;
                this.DmOpMenuItem.Enabled = true;
                this.CreateTabRuleOpMenuItem.Enabled = true;
                this.CreateIdRuleOpMenuItem.Enabled = true;
                this.CreateSourceRuleOpMenuItem.Enabled = true;
                this.ReadOpMenuItem.Enabled = true;
                this.UnreadOpMenuItem.Enabled = true;
                this.AuthorMenuItem.Visible = true;
                this.AuthorMenuItem.Text = $"@{post!.ScreenName}";
                this.RetweetedByMenuItem.Visible = post.RetweetedByUserId != null;
                this.RetweetedByMenuItem.Text = $"@{post.RetweetedBy}";
            }

            if (tab.TabType == MyCommon.TabUsageType.DirectMessage || !this.ExistCurrentPost || post == null || post.IsDm)
            {
                this.FavOpMenuItem.Enabled = false;
                this.UnFavOpMenuItem.Enabled = false;
                this.OpenStatusOpMenuItem.Enabled = false;
                this.ShowRelatedStatusesMenuItem2.Enabled = false;
                this.RtOpMenuItem.Enabled = false;
                this.RtUnOpMenuItem.Enabled = false;
                this.QtOpMenuItem.Enabled = false;
                this.FavoriteRetweetMenuItem.Enabled = false;
                this.FavoriteRetweetUnofficialMenuItem.Enabled = false;
            }
            else
            {
                this.FavOpMenuItem.Enabled = true;
                this.UnFavOpMenuItem.Enabled = true;
                this.OpenStatusOpMenuItem.Enabled = true;
                this.ShowRelatedStatusesMenuItem2.Enabled = true;  // PublicSearchの時問題出るかも

                if (!post.CanRetweetBy(this.tw.UserId))
                {
                    this.RtOpMenuItem.Enabled = false;
                    this.RtUnOpMenuItem.Enabled = false;
                    this.QtOpMenuItem.Enabled = false;
                    this.FavoriteRetweetMenuItem.Enabled = false;
                    this.FavoriteRetweetUnofficialMenuItem.Enabled = false;
                }
                else
                {
                    this.RtOpMenuItem.Enabled = true;
                    this.RtUnOpMenuItem.Enabled = true;
                    this.QtOpMenuItem.Enabled = true;
                    this.FavoriteRetweetMenuItem.Enabled = true;
                    this.FavoriteRetweetUnofficialMenuItem.Enabled = true;
                }
            }

            if (tab.TabType != MyCommon.TabUsageType.Favorites)
            {
                this.RefreshPrevOpMenuItem.Enabled = true;
            }
            else
            {
                this.RefreshPrevOpMenuItem.Enabled = false;
            }
            if (!this.ExistCurrentPost || post == null || post.InReplyToStatusId == null)
            {
                this.OpenRepSourceOpMenuItem.Enabled = false;
            }
            else
            {
                this.OpenRepSourceOpMenuItem.Enabled = true;
            }

            if (this.ExistCurrentPost && post != null)
            {
                this.DelOpMenuItem.Enabled = post.CanDeleteBy(this.tw.UserId);
            }
        }

        private void MenuItemTab_DropDownOpening(object sender, EventArgs e)
            => this.ContextMenuTabProperty_Opening(sender, null!);

        public Twitter TwitterInstance
            => this.tw;

        private void SplitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.initialLayout)
                return;

            int splitterDistance;
            switch (this.WindowState)
            {
                case FormWindowState.Normal:
                    splitterDistance = this.SplitContainer3.SplitterDistance;
                    break;
                case FormWindowState.Maximized:
                    // 最大化時は、通常時のウィンドウサイズに換算した SplitterDistance を算出する
                    var normalContainerWidth = this.mySize.Width - SystemInformation.Border3DSize.Width * 2;
                    splitterDistance = this.SplitContainer3.SplitterDistance - (this.SplitContainer3.Width - normalContainerWidth);
                    splitterDistance = Math.Min(splitterDistance, normalContainerWidth - this.SplitContainer3.SplitterWidth - this.SplitContainer3.Panel2MinSize);
                    break;
                default:
                    return;
            }

            this.mySpDis3 = splitterDistance;
            this.MarkSettingLocalModified();
        }

        private void MenuItemEdit_DropDownOpening(object sender, EventArgs e)
        {
            this.UndoRemoveTabMenuItem.Enabled = this.statuses.CanUndoRemovedTab;

            if (this.CurrentTab.TabType == MyCommon.TabUsageType.PublicSearch)
                this.PublicSearchQueryMenuItem.Enabled = true;
            else
                this.PublicSearchQueryMenuItem.Enabled = false;

            var post = this.CurrentPost;
            if (!this.ExistCurrentPost || post == null)
            {
                this.CopySTOTMenuItem.Enabled = false;
                this.CopyURLMenuItem.Enabled = false;
                this.CopyUserIdStripMenuItem.Enabled = false;
            }
            else
            {
                this.CopySTOTMenuItem.Enabled = true;
                this.CopyURLMenuItem.Enabled = true;
                this.CopyUserIdStripMenuItem.Enabled = true;

                if (post.IsDm) this.CopyURLMenuItem.Enabled = false;
                if (post.IsProtect) this.CopySTOTMenuItem.Enabled = false;
            }
        }

        private void NotifyIcon1_MouseMove(object sender, MouseEventArgs e)
            => this.SetNotifyIconText();

        private async void UserStatusToolStripMenuItem_Click(object sender, EventArgs e)
            => await this.ShowUserStatus(this.CurrentPost?.ScreenName ?? "");

        private async Task DoShowUserStatus(string id, bool showInputDialog)
        {
            TwitterUser? user = null;

            if (showInputDialog)
            {
                using var inputName = new InputTabName();
                inputName.FormTitle = "Show UserStatus";
                inputName.FormDescription = Properties.Resources.FRMessage1;
                inputName.TabName = id;

                if (inputName.ShowDialog(this) != DialogResult.OK)
                    return;
                if (string.IsNullOrWhiteSpace(inputName.TabName))
                    return;

                id = inputName.TabName.Trim();
            }

            using (var dialog = new WaitingDialog(Properties.Resources.doShowUserStatusText1))
            {
                var cancellationToken = dialog.EnableCancellation();

                try
                {
                    var task = this.tw.GetUserInfo(id);
                    user = await dialog.WaitForAsync(this, task);
                }
                catch (WebApiException ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                        MessageBox.Show($"Err:{ex.Message}(UsersShow)");
                    return;
                }

                if (cancellationToken.IsCancellationRequested)
                    return;
            }

            await this.DoShowUserStatus(user);
        }

        private async Task DoShowUserStatus(TwitterUser user)
        {
            using var userDialog = new UserInfoDialog(this, this.tw.Api);
            var showUserTask = userDialog.ShowUserAsync(user);
            userDialog.ShowDialog(this);

            this.Activate();
            this.BringToFront();

            // ユーザー情報の表示が完了するまで userDialog を破棄しない
            await showUserTask;
        }

        internal Task ShowUserStatus(string id, bool showInputDialog)
            => this.DoShowUserStatus(id, showInputDialog);

        internal Task ShowUserStatus(string id)
            => this.DoShowUserStatus(id, true);

        private async void AuthorShowProfileMenuItem_Click(object sender, EventArgs e)
        {
            var post = this.CurrentPost;
            if (post != null)
            {
                await this.ShowUserStatus(post.ScreenName, false);
            }
        }

        private async void RetweetedByShowProfileMenuItem_Click(object sender, EventArgs e)
        {
            var retweetedBy = this.CurrentPost?.RetweetedBy;
            if (retweetedBy != null)
            {
                await this.ShowUserStatus(retweetedBy, false);
            }
        }

        private async void RtCountMenuItem_Click(object sender, EventArgs e)
        {
            var post = this.CurrentPost;
            if (!this.ExistCurrentPost || post == null)
                return;

            var statusId = post.RetweetedId ?? post.StatusId;
            TwitterStatus status;

            using (var dialog = new WaitingDialog(Properties.Resources.RtCountMenuItem_ClickText1))
            {
                var cancellationToken = dialog.EnableCancellation();

                try
                {
                    var task = this.tw.Api.StatusesShow(statusId.ToTwitterStatusId());
                    status = await dialog.WaitForAsync(this, task);
                }
                catch (WebApiException ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                        MessageBox.Show(Properties.Resources.RtCountText2 + Environment.NewLine + "Err:" + ex.Message);
                    return;
                }

                if (cancellationToken.IsCancellationRequested)
                    return;
            }

            MessageBox.Show(status.RetweetCount + Properties.Resources.RtCountText1);
        }

        private void HookGlobalHotkey_HotkeyPressed(object sender, KeyEventArgs e)
        {
            if ((this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized) && this.Visible && Form.ActiveForm == this)
            {
                // アイコン化
                this.Visible = false;
            }
            else if (Form.ActiveForm == null)
            {
                this.Visible = true;
                if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
                this.Activate();
                this.BringToFront();
                this.StatusText.Focus();
            }
        }

        private void SplitContainer2_MouseDoubleClick(object sender, MouseEventArgs e)
            => this.MultiLinePullDownMenuItem.PerformClick();

#region "画像投稿"
        private void ImageSelectMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ImageSelector.Visible)
                this.ImageSelector.EndSelection();
            else
                this.ImageSelector.BeginSelection();
        }

        private void SelectMedia_DragEnter(DragEventArgs e)
        {
            if (this.ImageSelector.Model.HasUploadableService(((string[])e.Data.GetData(DataFormats.FileDrop, false))[0], true))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }
            e.Effect = DragDropEffects.None;
        }

        private void SelectMedia_DragDrop(DragEventArgs e)
        {
            this.Activate();
            this.BringToFront();

            var filePathArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            this.ImageSelector.BeginSelection();
            this.ImageSelector.Model.AddMediaItemFromFilePath(filePathArray);
            this.StatusText.Focus();
        }

        private void ImageSelector_BeginSelecting(object sender, EventArgs e)
        {
            this.TimelinePanel.Visible = false;
            this.TimelinePanel.Enabled = false;
        }

        private void ImageSelector_EndSelecting(object sender, EventArgs e)
        {
            this.TimelinePanel.Visible = true;
            this.TimelinePanel.Enabled = true;
            this.CurrentListView.Focus();
        }

        private void ImageSelector_FilePickDialogOpening(object sender, EventArgs e)
            => this.AllowDrop = false;

        private void ImageSelector_FilePickDialogClosed(object sender, EventArgs e)
            => this.AllowDrop = true;

        private void ImageSelector_SelectedServiceChanged(object sender, EventArgs e)
        {
            if (this.ImageSelector.Visible)
            {
                this.MarkSettingCommonModified();
                this.StatusText_TextChanged(this.StatusText, EventArgs.Empty);
            }
        }

        private void ImageSelector_VisibleChanged(object sender, EventArgs e)
            => this.StatusText_TextChanged(this.StatusText, EventArgs.Empty);

        /// <summary>
        /// StatusTextでCtrl+Vが押下された時の処理
        /// </summary>
        private void ProcClipboardFromStatusTextWhenCtrlPlusV()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    // clipboardにテキストがある場合は貼り付け処理
                    this.StatusText.Paste(Clipboard.GetText());
                }
                else if (Clipboard.ContainsImage())
                {
                    // clipboardから画像を取得
                    using var image = Clipboard.GetImage();
                    this.ImageSelector.BeginSelection();
                    this.ImageSelector.Model.AddMediaItemFromImage(image);
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    var files = Clipboard.GetFileDropList().Cast<string>().ToArray();
                    this.ImageSelector.BeginSelection();
                    this.ImageSelector.Model.AddMediaItemFromFilePath(files);
                }
            }
            catch (ExternalException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
#endregion

        private void ListManageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var form = new ListManage(this.tw);
            form.ShowDialog(this);
        }

        private bool ModifySettingCommon { get; set; }

        private bool ModifySettingLocal { get; set; }

        private bool ModifySettingAtId { get; set; }

        private void MenuItemCommand_DropDownOpening(object sender, EventArgs e)
        {
            var post = this.CurrentPost;
            if (this.ExistCurrentPost && post != null && !post.IsDm)
                this.RtCountMenuItem.Enabled = true;
            else
                this.RtCountMenuItem.Enabled = false;
        }

        private void CopyUserIdStripMenuItem_Click(object sender, EventArgs e)
            => this.CopyUserId();

        private void CopyUserId()
        {
            var post = this.CurrentPost;
            if (post == null) return;
            var clstr = post.ScreenName;
            try
            {
                Clipboard.SetDataObject(clstr, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void ShowRelatedStatusesMenuItem_Click(object sender, EventArgs e)
        {
            var post = this.CurrentPost;
            if (this.ExistCurrentPost && post != null && !post.IsDm)
            {
                try
                {
                    await this.OpenRelatedTab(post);
                }
                catch (TabException ex)
                {
                    MessageBox.Show(this, ex.Message, ApplicationSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 指定されたツイートに対する関連発言タブを開きます
        /// </summary>
        /// <param name="statusId">表示するツイートのID</param>
        /// <exception cref="TabException">名前の重複が多すぎてタブを作成できない場合</exception>
        public async Task OpenRelatedTab(PostId statusId)
        {
            var post = this.statuses[statusId];
            if (post == null)
            {
                try
                {
                    post = await this.tw.GetStatusApi(false, statusId.ToTwitterStatusId());
                }
                catch (WebApiException ex)
                {
                    this.StatusLabel.Text = $"Err:{ex.Message}(GetStatus)";
                    return;
                }
            }

            await this.OpenRelatedTab(post);
        }

        /// <summary>
        /// 指定されたツイートに対する関連発言タブを開きます
        /// </summary>
        /// <param name="post">表示する対象となるツイート</param>
        /// <exception cref="TabException">名前の重複が多すぎてタブを作成できない場合</exception>
        private async Task OpenRelatedTab(PostClass post)
        {
            var tabRelated = this.statuses.GetTabByType<RelatedPostsTabModel>();
            if (tabRelated != null)
            {
                this.RemoveSpecifiedTab(tabRelated.TabName, confirm: false);
            }

            var tabName = this.statuses.MakeTabName("Related Tweets");

            tabRelated = new RelatedPostsTabModel(tabName, post)
            {
                UnreadManage = false,
                Notify = false,
            };

            this.statuses.AddTab(tabRelated);
            this.AddNewTab(tabRelated, startup: false);

            this.ListTab.SelectedIndex = this.statuses.Tabs.IndexOf(tabName);

            await this.RefreshTabAsync(tabRelated);

            var tabIndex = this.statuses.Tabs.IndexOf(tabRelated.TabName);

            if (tabIndex != -1)
            {
                // TODO: 非同期更新中にタブが閉じられている場合を厳密に考慮したい

                var tabPage = this.ListTab.TabPages[tabIndex];
                var listView = (DetailsListView)tabPage.Tag;
                var targetPost = tabRelated.TargetPost;
                var index = tabRelated.IndexOf(targetPost.RetweetedId ?? targetPost.StatusId);

                if (index != -1 && index < listView.Items.Count)
                {
                    listView.SelectedIndices.Add(index);
                    listView.Items[index].Focused = true;
                }
            }
        }

        private void CacheInfoMenuItem_Click(object sender, EventArgs e)
        {
            var buf = new StringBuilder();
            buf.AppendFormat("キャッシュエントリ保持数     : {0}" + Environment.NewLine, this.iconCache.CacheCount);
            buf.AppendFormat("キャッシュエントリ破棄数     : {0}" + Environment.NewLine, this.iconCache.CacheRemoveCount);
            MessageBox.Show(buf.ToString(), "アイコンキャッシュ使用状況");
        }

        private void TweenRestartMenuItem_Click(object sender, EventArgs e)
        {
            MyCommon.EndingFlag = true;
            try
            {
                this.Close();
                Application.Restart();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to restart. Please run " + ApplicationSettings.ApplicationName + " manually.");
            }
        }

        private async void OpenOwnHomeMenuItem_Click(object sender, EventArgs e)
            => await MyCommon.OpenInBrowserAsync(this, MyCommon.TwitterUrl + this.tw.Username);

        private bool ExistCurrentPost
        {
            get
            {
                var post = this.CurrentPost;
                return post != null && !post.IsDeleted;
            }
        }

        private async void AuthorShowUserTimelineMenuItem_Click(object sender, EventArgs e)
            => await this.ShowUserTimeline();

        private async void RetweetedByShowUserTimelineMenuItem_Click(object sender, EventArgs e)
            => await this.ShowRetweeterTimeline();

        private string GetUserIdFromCurPostOrInput(string caption)
        {
            var id = this.CurrentPost?.ScreenName ?? "";

            using var inputName = new InputTabName();
            inputName.FormTitle = caption;
            inputName.FormDescription = Properties.Resources.FRMessage1;
            inputName.TabName = id;

            if (inputName.ShowDialog() == DialogResult.OK &&
                !MyCommon.IsNullOrEmpty(inputName.TabName.Trim()))
            {
                id = inputName.TabName.Trim();
            }
            else
            {
                id = "";
            }
            return id;
        }

        private async void UserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var id = this.GetUserIdFromCurPostOrInput("Show UserTimeline");
            if (!MyCommon.IsNullOrEmpty(id))
            {
                await this.AddNewTabForUserTimeline(id);
            }
        }

        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
                this.timelineScheduler.SystemResumed();
        }

        private void SystemEvents_TimeChanged(object sender, EventArgs e)
        {
            var prevTimeOffset = TimeZoneInfo.Local.BaseUtcOffset;

            TimeZoneInfo.ClearCachedData();

            var curTimeOffset = TimeZoneInfo.Local.BaseUtcOffset;

            if (curTimeOffset != prevTimeOffset)
            {
                // タイムゾーンの変更を反映
                this.listCache?.PurgeCache();
                this.CurrentListView.Refresh();

                this.DispSelectedPost(forceupdate: true);
            }

            this.timelineScheduler.Reset();
        }

        private void TimelineRefreshEnableChange(bool isEnable)
        {
            this.timelineScheduler.Enabled = isEnable;
        }

        private void StopRefreshAllMenuItem_CheckedChanged(object sender, EventArgs e)
            => this.TimelineRefreshEnableChange(!this.StopRefreshAllMenuItem.Checked);

        private async Task OpenUserAppointUrl()
        {
            if (!MyCommon.IsNullOrEmpty(this.settings.Common.UserAppointUrl))
            {
                if (this.settings.Common.UserAppointUrl.Contains("{ID}") || this.settings.Common.UserAppointUrl.Contains("{STATUS}"))
                {
                    var post = this.CurrentPost;
                    if (post != null)
                    {
                        var xUrl = this.settings.Common.UserAppointUrl;
                        xUrl = xUrl.Replace("{ID}", post.ScreenName);

                        var statusId = post.RetweetedId ?? post.StatusId;
                        xUrl = xUrl.Replace("{STATUS}", statusId.Id);

                        await MyCommon.OpenInBrowserAsync(this, xUrl);
                    }
                }
                else
                {
                    await MyCommon.OpenInBrowserAsync(this, this.settings.Common.UserAppointUrl);
                }
            }
        }

        private async void OpenUserSpecifiedUrlMenuItem_Click(object sender, EventArgs e)
            => await this.OpenUserAppointUrl();

        private async void GrowlHelper_Callback(object sender, GrowlHelper.NotifyCallbackEventArgs e)
        {
            if (Form.ActiveForm == null)
            {
                await this.InvokeAsync(() =>
                {
                    this.Visible = true;
                    if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
                    this.Activate();
                    this.BringToFront();
                    if (e.NotifyType == GrowlHelper.NotifyType.DirectMessage)
                    {
                        if (!this.GoDirectMessage(new TwitterStatusId(e.StatusId))) this.StatusText.Focus();
                    }
                    else
                    {
                        if (!this.GoStatus(new TwitterStatusId(e.StatusId))) this.StatusText.Focus();
                    }
                });
            }
        }

        private void ReplaceAppName()
        {
            this.MatomeMenuItem.Text = MyCommon.ReplaceAppName(this.MatomeMenuItem.Text);
            this.AboutMenuItem.Text = MyCommon.ReplaceAppName(this.AboutMenuItem.Text);
        }

        private async void TwitterApiStatusToolStripMenuItem_Click(object sender, EventArgs e)
            => await MyCommon.OpenInBrowserAsync(this, Twitter.ServiceAvailabilityStatusUrl);

        private void PostButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                this.JumpUnreadMenuItem_Click(this.JumpUnreadMenuItem, EventArgs.Empty);

                e.SuppressKeyPress = true;
            }
        }

        private void ContextMenuColumnHeader_Opening(object sender, CancelEventArgs e)
        {
            this.IconSizeNoneToolStripMenuItem.Checked = this.settings.Common.IconSize == MyCommon.IconSizes.IconNone;
            this.IconSize16ToolStripMenuItem.Checked = this.settings.Common.IconSize == MyCommon.IconSizes.Icon16;
            this.IconSize24ToolStripMenuItem.Checked = this.settings.Common.IconSize == MyCommon.IconSizes.Icon24;
            this.IconSize48ToolStripMenuItem.Checked = this.settings.Common.IconSize == MyCommon.IconSizes.Icon48;
            this.IconSize48_2ToolStripMenuItem.Checked = this.settings.Common.IconSize == MyCommon.IconSizes.Icon48_2;

            this.LockListSortOrderToolStripMenuItem.Checked = this.settings.Common.SortOrderLock;
        }

        private void IconSizeNoneToolStripMenuItem_Click(object sender, EventArgs e)
            => this.ChangeListViewIconSize(MyCommon.IconSizes.IconNone);

        private void IconSize16ToolStripMenuItem_Click(object sender, EventArgs e)
            => this.ChangeListViewIconSize(MyCommon.IconSizes.Icon16);

        private void IconSize24ToolStripMenuItem_Click(object sender, EventArgs e)
            => this.ChangeListViewIconSize(MyCommon.IconSizes.Icon24);

        private void IconSize48ToolStripMenuItem_Click(object sender, EventArgs e)
            => this.ChangeListViewIconSize(MyCommon.IconSizes.Icon48);

        private void IconSize48_2ToolStripMenuItem_Click(object sender, EventArgs e)
            => this.ChangeListViewIconSize(MyCommon.IconSizes.Icon48_2);

        private void ChangeListViewIconSize(MyCommon.IconSizes iconSize)
        {
            if (this.settings.Common.IconSize == iconSize) return;

            var oldIconCol = this.Use2ColumnsMode;

            this.settings.Common.IconSize = iconSize;
            this.ApplyListViewIconSize(iconSize);

            if (this.Use2ColumnsMode != oldIconCol)
            {
                foreach (TabPage tp in this.ListTab.TabPages)
                {
                    this.ResetColumns((DetailsListView)tp.Tag);
                }
            }

            this.CurrentListView.Refresh();
            this.MarkSettingCommonModified();
        }

        private void LockListSortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var state = this.LockListSortOrderToolStripMenuItem.Checked;
            if (this.settings.Common.SortOrderLock == state) return;

            this.settings.Common.SortOrderLock = state;
            this.MarkSettingCommonModified();
        }

        private void TweetDetailsView_StatusChanged(object sender, TweetDetailsViewStatusChengedEventArgs e)
        {
            if (!MyCommon.IsNullOrEmpty(e.StatusText))
            {
                this.StatusLabelUrl.Text = e.StatusText;
            }
            else
            {
                this.SetStatusLabelUrl();
            }
        }
    }
}
