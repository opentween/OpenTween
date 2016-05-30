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

//コンパイル後コマンド
//"c:\Program Files\Microsoft.NET\SDK\v2.0\Bin\sgen.exe" /f /a:"$(TargetPath)"
//"C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\sgen.exe" /f /a:"$(TargetPath)"

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Api;
using OpenTween.Api.DataModel;
using OpenTween.Connection;
using OpenTween.Models;
using OpenTween.OpenTweenCustomControl;
using OpenTween.Thumbnail;

namespace OpenTween
{
    public partial class TweenMain : OTBaseForm
    {
        //各種設定
        private Size _mySize;           //画面サイズ
        private Point _myLoc;           //画面位置
        private int _mySpDis;           //区切り位置
        private int _mySpDis2;          //発言欄区切り位置
        private int _mySpDis3;          //プレビュー区切り位置
        private int _iconSz;            //アイコンサイズ（現在は16、24、48の3種類。将来直接数字指定可能とする 注：24x24の場合に26と指定しているのはMSゴシック系フォントのための仕様）
        private bool _iconCol;          //1列表示の時true（48サイズのとき）

        //雑多なフラグ類
        private bool _initial;         //true:起動時処理中
        private bool _initialLayout = true;
        private bool _ignoreConfigSave;         //true:起動時処理中
        private bool _tabDrag;           //タブドラッグ中フラグ（DoDragDropを実行するかの判定用）
        private TabPage _beforeSelectedTab; //タブが削除されたときに前回選択されていたときのタブを選択する為に保持
        private Point _tabMouseDownPoint;
        private string _rclickTabName;      //右クリックしたタブの名前（Tabコントロール機能不足対応）
        private readonly object _syncObject = new object();    //ロック用

        private const string detailHtmlFormatHeaderMono = 
            "<html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=8\">"
            + "<style type=\"text/css\"><!-- "
            + "body, p, pre {margin: 0;} "
            + "pre {font-family: \"%FONT_FAMILY%\", sans-serif; font-size: %FONT_SIZE%pt; background-color:rgb(%BG_COLOR%); word-wrap: break-word; color:rgb(%FONT_COLOR%);} "
            + "a:link, a:visited, a:active, a:hover {color:rgb(%LINK_COLOR%); } "
            + "img.emoji {width: 1em; height: 1em; margin: 0 .05em 0 .1em; vertical-align: -0.1em; border: none;} "
            + ".quote-tweet {border: 1px solid #ccc; margin: 1em; padding: 0.5em;} "
            + ".quote-tweet.reply {border-color: #f33;} "
            + ".quote-tweet-link {color: inherit !important; text-decoration: none;}"
            + "--></style>"
            + "</head><body><pre>";
        private const string detailHtmlFormatFooterMono = "</pre></body></html>";
        private const string detailHtmlFormatHeaderColor = 
            "<html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=8\">"
            + "<style type=\"text/css\"><!-- "
            + "body, p, pre {margin: 0;} "
            + "body {font-family: \"%FONT_FAMILY%\", sans-serif; font-size: %FONT_SIZE%pt; background-color:rgb(%BG_COLOR%); margin: 0; word-wrap: break-word; color:rgb(%FONT_COLOR%);} "
            + "a:link, a:visited, a:active, a:hover {color:rgb(%LINK_COLOR%); } "
            + "img.emoji {width: 1em; height: 1em; margin: 0 .05em 0 .1em; vertical-align: -0.1em; border: none;} "
            + ".quote-tweet {border: 1px solid #ccc; margin: 1em; padding: 0.5em;} "
            + ".quote-tweet.reply {border-color: rgb(%BG_REPLY_COLOR%);} "
            + ".quote-tweet-link {color: inherit !important; text-decoration: none;}"
            + "--></style>"
            + "</head><body><p>";
        private const string detailHtmlFormatFooterColor = "</p></body></html>";
        private string detailHtmlFormatHeader;
        private string detailHtmlFormatFooter;

        private bool _myStatusError = false;
        private bool _myStatusOnline = false;
        private bool soundfileListup = false;
        private FormWindowState _formWindowState = FormWindowState.Normal; // フォームの状態保存用 通知領域からアイコンをクリックして復帰した際に使用する

        //設定ファイル関連
        //private SettingToConfig _cfg; //旧
        internal SettingLocal _cfgLocal;
        private SettingCommon _cfgCommon;

        //twitter解析部
        private TwitterApi twitterApi = new TwitterApi();
        private Twitter tw;

        //Growl呼び出し部
        private GrowlHelper gh = new GrowlHelper(Application.ProductName);

        //サブ画面インスタンス
        internal SearchWordDialog SearchDialog = new SearchWordDialog();     //検索画面インスタンス
        private OpenURL UrlDialog = new OpenURL();
        public AtIdSupplement AtIdSupl;     //@id補助
        public AtIdSupplement HashSupl;    //Hashtag補助
        public HashtagManage HashMgr;
        private EventViewerDialog evtDialog;

        //表示フォント、色、アイコン
        private Font _fntUnread;            //未読用フォント
        private Color _clUnread;            //未読用文字色
        private Font _fntReaded;            //既読用フォント
        private Color _clReaded;            //既読用文字色
        private Color _clFav;               //Fav用文字色
        private Color _clOWL;               //片思い用文字色
        private Color _clRetweet;               //Retweet用文字色
        private Color _clHighLight = Color.FromKnownColor(KnownColor.HighlightText);         //選択中の行用文字色
        private Font _fntDetail;            //発言詳細部用フォント
        private Color _clDetail;              //発言詳細部用色
        private Color _clDetailLink;          //発言詳細部用リンク文字色
        private Color _clDetailBackcolor;     //発言詳細部用背景色
        private Color _clSelf;              //自分の発言用背景色
        private Color _clAtSelf;            //自分宛返信用背景色
        private Color _clTarget;            //選択発言者の他の発言用背景色
        private Color _clAtTarget;          //選択発言中の返信先用背景色
        private Color _clAtFromTarget;      //選択発言者への返信発言用背景色
        private Color _clAtTo;              //選択発言の唯一＠先
        private Color _clListBackcolor;       //リスト部通常発言背景色
        private Color _clInputBackcolor;      //入力欄背景色
        private Color _clInputFont;           //入力欄文字色
        private Font _fntInputFont;           //入力欄フォント
        private ImageCache IconCache;        //アイコン画像リスト
        private Icon NIconAt;               //At.ico             タスクトレイアイコン：通常時
        private Icon NIconAtRed;            //AtRed.ico          タスクトレイアイコン：通信エラー時
        private Icon NIconAtSmoke;          //AtSmoke.ico        タスクトレイアイコン：オフライン時
        private Icon[] NIconRefresh = new Icon[4];       //Refresh.ico        タスクトレイアイコン：更新中（アニメーション用に4種類を保持するリスト）
        private Icon TabIcon;               //Tab.ico            未読のあるタブ用アイコン
        private Icon MainIcon;              //Main.ico           画面左上のアイコン
        private Icon ReplyIcon;               //5g
        private Icon ReplyIconBlink;          //6g

        private ImageList _listViewImageList = new ImageList();    //ListViewItemの高さ変更用

        private PostClass _anchorPost;
        private bool _anchorFlag;        //true:関連発言移動中（関連移動以外のオペレーションをするとfalseへ。trueだとリスト背景色をアンカー発言選択中として描画）

        private List<PostingStatus> _history = new List<PostingStatus>();   //発言履歴
        private int _hisIdx;                  //発言履歴カレントインデックス

        //発言投稿時のAPI引数（発言編集時に設定。手書きreplyでは設定されない）
        private Tuple<long, string> inReplyTo = null; // リプライ先のステータスID・スクリーン名

        //時速表示用
        private List<DateTime> _postTimestamps = new List<DateTime>();
        private List<DateTime> _favTimestamps = new List<DateTime>();

        // 以下DrawItem関連
        private SolidBrush _brsHighLight = new SolidBrush(Color.FromKnownColor(KnownColor.Highlight));
        private SolidBrush _brsBackColorMine;
        private SolidBrush _brsBackColorAt;
        private SolidBrush _brsBackColorYou;
        private SolidBrush _brsBackColorAtYou;
        private SolidBrush _brsBackColorAtFromTarget;
        private SolidBrush _brsBackColorAtTo;
        private SolidBrush _brsBackColorNone;
        private SolidBrush _brsDeactiveSelection = new SolidBrush(Color.FromKnownColor(KnownColor.ButtonFace)); //Listにフォーカスないときの選択行の背景色
        private StringFormat sfTab = new StringFormat();

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private TabInformations _statuses;

        /// <summary>
        /// 現在表示している発言一覧の <see cref="ListView"/> に対するキャッシュ
        /// </summary>
        /// <remarks>
        /// キャッシュクリアのために null が代入されることがあるため、
        /// 使用する場合には <see cref="_listItemCache"/> に対して直接メソッド等を呼び出さずに
        /// 一旦ローカル変数に代入してから参照すること。
        /// </remarks>
        private ListViewItemCache _listItemCache = null;

        internal class ListViewItemCache
        {
            /// <summary>アイテムをキャッシュする対象の <see cref="ListView"/></summary>
            public ListView TargetList { get; set; }

            /// <summary>キャッシュする範囲の開始インデックス</summary>
            public int StartIndex { get; set; }

            /// <summary>キャッシュする範囲の終了インデックス</summary>
            public int EndIndex { get; set; }

            /// <summary>キャッシュされた <see cref="ListViewItem"/> インスタンス</summary>
            public ListViewItem[] ListItem { get; set; }

            /// <summary>キャッシュされた範囲に対応する <see cref="PostClass"/> インスタンス</summary>
            public PostClass[] Post { get; set; }

            /// <summary>キャッシュされたアイテムの件数</summary>
            public int Count
                => this.EndIndex - this.StartIndex + 1;

            /// <summary>指定されたインデックスがキャッシュの範囲内であるか判定します</summary>
            /// <returns><paramref name="index"/> がキャッシュの範囲内であれば true、それ以外は false</returns>
            public bool Contains(int index)
                => index >= this.StartIndex && index <= this.EndIndex;

            /// <summary>指定されたインデックスの範囲が全てキャッシュの範囲内であるか判定します</summary>
            /// <returns><paramref name="rangeStart"/> から <paramref name="rangeEnd"/> の範囲が全てキャッシュの範囲内であれば true、それ以外は false</returns>
            public bool IsSupersetOf(int rangeStart, int rangeEnd)
                => rangeStart >= this.StartIndex && rangeEnd <= this.EndIndex;

            /// <summary>指定されたインデックスの <see cref="ListViewItem"/> と <see cref="PostClass"/> をキャッシュから取得することを試みます</summary>
            /// <returns>取得に成功すれば true、それ以外は false</returns>
            public bool TryGetValue(int index, out ListViewItem item, out PostClass post)
            {
                if (this.Contains(index))
                {
                    item = this.ListItem[index - this.StartIndex];
                    post = this.Post[index - this.StartIndex];
                    return true;
                }
                else
                {
                    item = null;
                    post = null;
                    return false;
                }
            }
        }

        private TabPage _curTab;
        private int _curItemIndex;
        private DetailsListView _curList;
        private PostClass _curPost;
        private bool _isColumnChanged = false;

        private const int MAX_WORKER_THREADS = 20;
        private SemaphoreSlim workerSemaphore = new SemaphoreSlim(MAX_WORKER_THREADS);
        private CancellationTokenSource workerCts = new CancellationTokenSource();
        private IProgress<string> workerProgress;

        private int UnreadCounter = -1;
        private int UnreadAtCounter = -1;

        private string[] ColumnOrgText = new string[9];
        private string[] ColumnText = new string[9];

        private bool _DoFavRetweetFlags = false;
        private bool osResumed = false;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool _colorize = false;

        private System.Timers.Timer TimerTimeline = new System.Timers.Timer();

        private string recommendedStatusFooter;

        //URL短縮のUndo用
        private struct urlUndo
        {
            public string Before;
            public string After;
        }

        private List<urlUndo> urlUndoBuffer = null;

        private struct ReplyChain
        {
            public long OriginalId;
            public long InReplyToId;
            public TabPage OriginalTab;

            public ReplyChain(long originalId, long inReplyToId, TabPage originalTab)
            {
                this.OriginalId = originalId;
                this.InReplyToId = inReplyToId;
                this.OriginalTab = originalTab;
            }
        }

        private Stack<ReplyChain> replyChains; //[, ]でのリプライ移動の履歴
        private Stack<Tuple<TabPage, PostClass>> selectPostChains = new Stack<Tuple<TabPage, PostClass>>(); //ポスト選択履歴

        //検索処理タイプ
        internal enum SEARCHTYPE
        {
            DialogSearch,
            NextSearch,
            PrevSearch,
        }

        private class PostingStatus
        {
            public string status = "";
            public long? inReplyToId = null;
            public string inReplyToName = null;
            public string imageService = "";      //画像投稿サービス名
            public IMediaItem[] mediaItems = null;
            public PostingStatus()
            {
            }
            public PostingStatus(string status, long? replyToId, string replyToName)
            {
                this.status = status;
                this.inReplyToId = replyToId;
                this.inReplyToName = replyToName;
            }
        }

        private void TweenMain_Activated(object sender, EventArgs e)
        {
            //画面がアクティブになったら、発言欄の背景色戻す
            if (StatusText.Focused)
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

                //後始末
                SearchDialog.Dispose();
                UrlDialog.Dispose();
                NIconAt?.Dispose();
                NIconAtRed?.Dispose();
                NIconAtSmoke?.Dispose();
                foreach (var iconRefresh in this.NIconRefresh)
                {
                    iconRefresh?.Dispose();
                }
                TabIcon?.Dispose();
                MainIcon?.Dispose();
                ReplyIcon?.Dispose();
                ReplyIconBlink?.Dispose();
                _listViewImageList.Dispose();
                _brsHighLight.Dispose();
                _brsBackColorMine?.Dispose();
                _brsBackColorAt?.Dispose();
                _brsBackColorYou?.Dispose();
                _brsBackColorAtYou?.Dispose();
                _brsBackColorAtFromTarget?.Dispose();
                _brsBackColorAtTo?.Dispose();
                _brsBackColorNone?.Dispose();
                _brsDeactiveSelection?.Dispose();
                //sf.Dispose();
                sfTab.Dispose();

                this.workerCts.Cancel();

                if (IconCache != null)
                {
                    this.IconCache.CancelAsync();
                    this.IconCache.Dispose();
                }

                this.thumbnailTokenSource?.Dispose();

                this.tw.Dispose();
                this.twitterApi.Dispose();
                this._hookGlobalHotkey.Dispose();
            }

            // 終了時にRemoveHandlerしておかないとメモリリークする
            // http://msdn.microsoft.com/ja-jp/library/microsoft.win32.systemevents.powermodechanged.aspx
            Microsoft.Win32.SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;

            this.disposed = true;
        }

        private void LoadIcons()
        {
            // Icons フォルダ以下のアイコンを読み込み（着せ替えアイコン対応）
            var iconsDir = Path.Combine(Application.StartupPath, "Icons");

            // ウィンドウ左上のアイコン
            var iconMain = this.LoadIcon(Path.Combine(iconsDir, "MIcon.ico"));

            // タブ見出し未読表示アイコン
            var iconTab = this.LoadIcon(Path.Combine(iconsDir, "Tab.ico"));

            // タスクトレイ: 通常時アイコン
            var iconAt = this.LoadIcon(Path.Combine(iconsDir, "At.ico"));

            // タスクトレイ: エラー時アイコン
            var iconAtRed = this.LoadIcon(Path.Combine(iconsDir, "AtRed.ico"));

            // タスクトレイ: オフライン時アイコン
            var iconAtSmoke = this.LoadIcon(Path.Combine(iconsDir, "AtSmoke.ico"));

            // タスクトレイ: Reply通知アイコン (最大2枚でアニメーション可能)
            var iconReply = this.LoadIcon(Path.Combine(iconsDir, "Reply.ico"));
            var iconReplyBlink = this.LoadIcon(Path.Combine(iconsDir, "ReplyBlink.ico"));

            // タスクトレイ: 更新中アイコン (最大4枚でアニメーション可能)
            var iconRefresh1 = this.LoadIcon(Path.Combine(iconsDir, "Refresh.ico"));
            var iconRefresh2 = this.LoadIcon(Path.Combine(iconsDir, "Refresh2.ico"));
            var iconRefresh3 = this.LoadIcon(Path.Combine(iconsDir, "Refresh3.ico"));
            var iconRefresh4 = this.LoadIcon(Path.Combine(iconsDir, "Refresh4.ico"));

            // 読み込んだアイコンを設定 (不足するアイコンはデフォルトのものを設定)

            this.MainIcon = iconMain ?? Properties.Resources.MIcon;
            this.TabIcon = iconTab ?? Properties.Resources.TabIcon;
            this.NIconAt = iconAt ?? iconMain ?? Properties.Resources.At;
            this.NIconAtRed = iconAtRed ?? Properties.Resources.AtRed;
            this.NIconAtSmoke = iconAtSmoke ?? Properties.Resources.AtSmoke;

            if (iconReply != null && iconReplyBlink != null)
            {
                this.ReplyIcon = iconReply;
                this.ReplyIconBlink = iconReplyBlink;
            }
            else
            {
                this.ReplyIcon = iconReply ?? iconReplyBlink ?? Properties.Resources.Reply;
                this.ReplyIconBlink = this.NIconAt;
            }

            if (iconRefresh1 == null)
            {
                this.NIconRefresh = new[] {
                    Properties.Resources.Refresh, Properties.Resources.Refresh2,
                    Properties.Resources.Refresh3, Properties.Resources.Refresh4,
                };
            }
            else if (iconRefresh2 == null)
            {
                this.NIconRefresh = new[] { iconRefresh1 };
            }
            else if (iconRefresh3 == null)
            {
                this.NIconRefresh = new[] { iconRefresh1, iconRefresh2 };
            }
            else if (iconRefresh4 == null)
            {
                this.NIconRefresh = new[] { iconRefresh1, iconRefresh2, iconRefresh3 };
            }
            else // iconRefresh1 から iconRefresh4 まで全て揃っている
            {
                this.NIconRefresh = new[] { iconRefresh1, iconRefresh2, iconRefresh3, iconRefresh4 };
            }
        }

        private Icon LoadIcon(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                return new Icon(filePath);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void InitColumns(ListView list, bool startup)
        {
            this.InitColumnText();

            ColumnHeader[] columns = null;
            try
            {
                if (this._iconCol)
                {
                    columns = new[]
                    {
                        new ColumnHeader(), // アイコン
                        new ColumnHeader(), // 本文
                    };

                    columns[0].Text = this.ColumnText[0];
                    columns[1].Text = this.ColumnText[2];

                    if (startup)
                    {
                        var widthScaleFactor = this.CurrentAutoScaleDimensions.Width / this._cfgLocal.ScaleDimension.Width;

                        columns[0].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width1);
                        columns[1].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width3);
                        columns[0].DisplayIndex = 0;
                        columns[1].DisplayIndex = 1;
                    }
                    else
                    {
                        var idx = 0;
                        foreach (var curListColumn in this._curList.Columns.Cast<ColumnHeader>())
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
                        columns[i].Text = this.ColumnText[i];

                    if (startup)
                    {
                        var widthScaleFactor = this.CurrentAutoScaleDimensions.Width / this._cfgLocal.ScaleDimension.Width;

                        columns[0].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width1);
                        columns[1].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width2);
                        columns[2].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width3);
                        columns[3].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width4);
                        columns[4].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width5);
                        columns[5].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width6);
                        columns[6].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width7);
                        columns[7].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width8);

                        var displayIndex = new[] {
                            this._cfgLocal.DisplayIndex1, this._cfgLocal.DisplayIndex2,
                            this._cfgLocal.DisplayIndex3, this._cfgLocal.DisplayIndex4,
                            this._cfgLocal.DisplayIndex5, this._cfgLocal.DisplayIndex6,
                            this._cfgLocal.DisplayIndex7, this._cfgLocal.DisplayIndex8
                        };

                        foreach (var i in Enumerable.Range(0, displayIndex.Length))
                        {
                            columns[i].DisplayIndex = displayIndex[i];
                        }
                    }
                    else
                    {
                        var idx = 0;
                        foreach (var curListColumn in this._curList.Columns.Cast<ColumnHeader>())
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
            ColumnText[0] = "";
            ColumnText[1] = Properties.Resources.AddNewTabText2;
            ColumnText[2] = Properties.Resources.AddNewTabText3;
            ColumnText[3] = Properties.Resources.AddNewTabText4_2;
            ColumnText[4] = Properties.Resources.AddNewTabText5;
            ColumnText[5] = "";
            ColumnText[6] = "";
            ColumnText[7] = "Source";

            ColumnOrgText[0] = "";
            ColumnOrgText[1] = Properties.Resources.AddNewTabText2;
            ColumnOrgText[2] = Properties.Resources.AddNewTabText3;
            ColumnOrgText[3] = Properties.Resources.AddNewTabText4_2;
            ColumnOrgText[4] = Properties.Resources.AddNewTabText5;
            ColumnOrgText[5] = "";
            ColumnOrgText[6] = "";
            ColumnOrgText[7] = "Source";

            int c = 0;
            switch (_statuses.SortMode)
            {
                case ComparerMode.Nickname:  //ニックネーム
                    c = 1;
                    break;
                case ComparerMode.Data:  //本文
                    c = 2;
                    break;
                case ComparerMode.Id:  //時刻=発言Id
                    c = 3;
                    break;
                case ComparerMode.Name:  //名前
                    c = 4;
                    break;
                case ComparerMode.Source:  //Source
                    c = 7;
                    break;
            }

            if (_iconCol)
            {
                if (_statuses.SortOrder == SortOrder.Descending)
                {
                    // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                    ColumnText[2] = ColumnOrgText[2] + "▾";
                }
                else
                {
                    // U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                    ColumnText[2] = ColumnOrgText[2] + "▴";
                }
            }
            else
            {
                if (_statuses.SortOrder == SortOrder.Descending)
                {
                    // U+25BE BLACK DOWN-POINTING SMALL TRIANGLE
                    ColumnText[c] = ColumnOrgText[c] + "▾";
                }
                else
                {
                    // U+25B4 BLACK UP-POINTING SMALL TRIANGLE
                    ColumnText[c] = ColumnOrgText[c] + "▴";
                }
            }
        }

        private void InitializeTraceFrag()
        {
#if DEBUG
            TraceOutToolStripMenuItem.Checked = true;
            MyCommon.TraceFlag = true;
#endif
            if (!MyCommon.FileVersion.EndsWith("0", StringComparison.Ordinal))
            {
                TraceOutToolStripMenuItem.Checked = true;
                MyCommon.TraceFlag = true;
            }
        }

        private void TweenMain_Load(object sender, EventArgs e)
        {
            _ignoreConfigSave = true;
            this.Visible = false;

            if (MyApplication.StartupOptions.ContainsKey("d"))
                MyCommon.TraceFlag = true;

            InitializeTraceFrag();

            //Win32Api.SetProxy(HttpConnection.ProxyType.Specified, "127.0.0.1", 8080, "user", "pass")

            MyCommon.TwitterApiInfo.AccessLimitUpdated += TwitterApiStatus_AccessLimitUpdated;
            Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            Regex.CacheSize = 100;

            //発言保持クラス
            _statuses = TabInformations.GetInstance();

            //アイコン設定
            LoadIcons();
            this.Icon = MainIcon;              //メインフォーム（TweenMain）
            NotifyIcon1.Icon = NIconAt;      //タスクトレイ
            TabImage.Images.Add(TabIcon);    //タブ見出し

            //<<<<<<<<<設定関連>>>>>>>>>
            ////設定読み出し
            LoadConfig();

            // 現在の DPI と設定保存時の DPI との比を取得する
            var configScaleFactor = this._cfgLocal.GetConfigScaleFactor(this.CurrentAutoScaleDimensions);

            // UIフォント設定
            var fontUIGlobal = this._cfgLocal.FontUIGlobal;
            if (fontUIGlobal != null)
            {
                OTBaseForm.GlobalFont = fontUIGlobal;
                this.Font = fontUIGlobal;
            }

            //不正値チェック
            if (!MyApplication.StartupOptions.ContainsKey("nolimit"))
            {
                if (this._cfgCommon.TimelinePeriod < 15 && this._cfgCommon.TimelinePeriod > 0)
                    this._cfgCommon.TimelinePeriod = 15;

                if (this._cfgCommon.ReplyPeriod < 15 && this._cfgCommon.ReplyPeriod > 0)
                    this._cfgCommon.ReplyPeriod = 15;

                if (this._cfgCommon.DMPeriod < 15 && this._cfgCommon.DMPeriod > 0)
                    this._cfgCommon.DMPeriod = 15;

                if (this._cfgCommon.PubSearchPeriod < 30 && this._cfgCommon.PubSearchPeriod > 0)
                    this._cfgCommon.PubSearchPeriod = 30;

                if (this._cfgCommon.UserTimelinePeriod < 15 && this._cfgCommon.UserTimelinePeriod > 0)
                    this._cfgCommon.UserTimelinePeriod = 15;

                if (this._cfgCommon.ListsPeriod < 15 && this._cfgCommon.ListsPeriod > 0)
                    this._cfgCommon.ListsPeriod = 15;
            }

            if (!Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.Timeline, this._cfgCommon.CountApi))
                this._cfgCommon.CountApi = 60;
            if (!Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.Reply, this._cfgCommon.CountApiReply))
                this._cfgCommon.CountApiReply = 40;

            if (this._cfgCommon.MoreCountApi != 0 && !Twitter.VerifyMoreApiResultCount(this._cfgCommon.MoreCountApi))
                this._cfgCommon.MoreCountApi = 200;
            if (this._cfgCommon.FirstCountApi != 0 && !Twitter.VerifyFirstApiResultCount(this._cfgCommon.FirstCountApi))
                this._cfgCommon.FirstCountApi = 100;

            if (this._cfgCommon.FavoritesCountApi != 0 && !Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.Favorites, this._cfgCommon.FavoritesCountApi))
                this._cfgCommon.FavoritesCountApi = 40;
            if (this._cfgCommon.ListCountApi != 0 && !Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.List, this._cfgCommon.ListCountApi))
                this._cfgCommon.ListCountApi = 100;
            if (this._cfgCommon.SearchCountApi != 0 && !Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.PublicSearch, this._cfgCommon.SearchCountApi))
                this._cfgCommon.SearchCountApi = 100;
            if (this._cfgCommon.UserTimelineCountApi != 0 && !Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.UserTimeline, this._cfgCommon.UserTimelineCountApi))
                this._cfgCommon.UserTimelineCountApi = 20;

            //廃止サービスが選択されていた場合ux.nuへ読み替え
            if (this._cfgCommon.AutoShortUrlFirst < 0)
                this._cfgCommon.AutoShortUrlFirst = MyCommon.UrlConverter.Uxnu;

            TwitterApiConnection.RestApiHost = this._cfgCommon.TwitterApiHost;
            this.tw = new Twitter(this.twitterApi);

            //認証関連
            if (string.IsNullOrEmpty(this._cfgCommon.Token)) this._cfgCommon.UserName = "";
            tw.Initialize(this._cfgCommon.Token, this._cfgCommon.TokenSecret, this._cfgCommon.UserName, this._cfgCommon.UserId);

            _initial = true;

            Networking.Initialize();

            bool saveRequired = false;
            bool firstRun = false;

            //ユーザー名、パスワードが未設定なら設定画面を表示（初回起動時など）
            if (string.IsNullOrEmpty(tw.Username))
            {
                saveRequired = true;
                firstRun = true;

                //設定せずにキャンセルされたか、設定されたが依然ユーザー名が未設定ならプログラム終了
                if (ShowSettingDialog(showTaskbarIcon: true) != DialogResult.OK ||
                    string.IsNullOrEmpty(tw.Username))
                {
                    Application.Exit();  //強制終了
                    return;
                }
            }

            //Twitter用通信クラス初期化
            Networking.DefaultTimeout = TimeSpan.FromSeconds(this._cfgCommon.DefaultTimeOut);
            Networking.SetWebProxy(this._cfgLocal.ProxyType,
                this._cfgLocal.ProxyAddress, this._cfgLocal.ProxyPort,
                this._cfgLocal.ProxyUser, this._cfgLocal.ProxyPassword);
            Networking.ForceIPv4 = this._cfgCommon.ForceIPv4;

            TwitterApiConnection.RestApiHost = this._cfgCommon.TwitterApiHost;
            tw.RestrictFavCheck = this._cfgCommon.RestrictFavCheck;
            tw.ReadOwnPost = this._cfgCommon.ReadOwnPost;
            tw.TrackWord = this._cfgCommon.TrackWord;
            TrackToolStripMenuItem.Checked = !String.IsNullOrEmpty(tw.TrackWord);
            tw.AllAtReply = this._cfgCommon.AllAtReply;
            AllrepliesToolStripMenuItem.Checked = tw.AllAtReply;
            ShortUrl.Instance.DisableExpanding = !this._cfgCommon.TinyUrlResolve;
            ShortUrl.Instance.BitlyId = this._cfgCommon.BilyUser;
            ShortUrl.Instance.BitlyKey = this._cfgCommon.BitlyPwd;

            // アクセストークンが有効であるか確認する
            // ここが Twitter API への最初のアクセスになるようにすること
            try
            {
                this.tw.VerifyCredentials();
            }
            catch (WebApiException ex)
            {
                MessageBox.Show(this, string.Format(Properties.Resources.StartupAuthError_Text, ex.Message),
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //サムネイル関連の初期化
            //プロキシ設定等の通信まわりの初期化が済んでから処理する
            ThumbnailGenerator.InitializeGenerator();

            var imgazyobizinet = ThumbnailGenerator.ImgAzyobuziNetInstance;
            imgazyobizinet.Enabled = this._cfgCommon.EnableImgAzyobuziNet;
            imgazyobizinet.DisabledInDM = this._cfgCommon.ImgAzyobuziNetDisabledInDM;

            Thumbnail.Services.TonTwitterCom.GetApiConnection = () => this.twitterApi.Connection;

            //画像投稿サービス
            ImageSelector.Initialize(tw, this.tw.Configuration, _cfgCommon.UseImageServiceName, _cfgCommon.UseImageService);

            //ハッシュタグ/@id関連
            AtIdSupl = new AtIdSupplement(SettingAtIdList.Load().AtIdList, "@");
            HashSupl = new AtIdSupplement(_cfgCommon.HashTags, "#");
            HashMgr = new HashtagManage(HashSupl,
                                    _cfgCommon.HashTags.ToArray(),
                                    _cfgCommon.HashSelected,
                                    _cfgCommon.HashIsPermanent,
                                    _cfgCommon.HashIsHead,
                                    _cfgCommon.HashIsNotAddToAtReply);
            if (!string.IsNullOrEmpty(HashMgr.UseHash) && HashMgr.IsPermanent) HashStripSplitButton.Text = HashMgr.UseHash;

            //アイコンリスト作成
            this.IconCache = new ImageCache();
            this.tweetDetailsView.IconCache = this.IconCache;

            //フォント＆文字色＆背景色保持
            _fntUnread = this._cfgLocal.FontUnread;
            _clUnread = this._cfgLocal.ColorUnread;
            _fntReaded = this._cfgLocal.FontRead;
            _clReaded = this._cfgLocal.ColorRead;
            _clFav = this._cfgLocal.ColorFav;
            _clOWL = this._cfgLocal.ColorOWL;
            _clRetweet = this._cfgLocal.ColorRetweet;
            _fntDetail = this._cfgLocal.FontDetail;
            _clDetail = this._cfgLocal.ColorDetail;
            _clDetailLink = this._cfgLocal.ColorDetailLink;
            _clDetailBackcolor = this._cfgLocal.ColorDetailBackcolor;
            _clSelf = this._cfgLocal.ColorSelf;
            _clAtSelf = this._cfgLocal.ColorAtSelf;
            _clTarget = this._cfgLocal.ColorTarget;
            _clAtTarget = this._cfgLocal.ColorAtTarget;
            _clAtFromTarget = this._cfgLocal.ColorAtFromTarget;
            _clAtTo = this._cfgLocal.ColorAtTo;
            _clListBackcolor = this._cfgLocal.ColorListBackcolor;
            _clInputBackcolor = this._cfgLocal.ColorInputBackcolor;
            _clInputFont = this._cfgLocal.ColorInputFont;
            _fntInputFont = this._cfgLocal.FontInputFont;

            _brsBackColorMine = new SolidBrush(_clSelf);
            _brsBackColorAt = new SolidBrush(_clAtSelf);
            _brsBackColorYou = new SolidBrush(_clTarget);
            _brsBackColorAtYou = new SolidBrush(_clAtTarget);
            _brsBackColorAtFromTarget = new SolidBrush(_clAtFromTarget);
            _brsBackColorAtTo = new SolidBrush(_clAtTo);
            //_brsBackColorNone = new SolidBrush(Color.FromKnownColor(KnownColor.Window));
            _brsBackColorNone = new SolidBrush(_clListBackcolor);

            // StringFormatオブジェクトへの事前設定
            //sf.Alignment = StringAlignment.Near;             // Textを近くへ配置（左から右の場合は左寄せ）
            //sf.LineAlignment = StringAlignment.Near;         // Textを近くへ配置（上寄せ）
            //sf.FormatFlags = StringFormatFlags.LineLimit;    // 
            sfTab.Alignment = StringAlignment.Center;
            sfTab.LineAlignment = StringAlignment.Center;

            InitDetailHtmlFormat();

            //Regex statregex = new Regex("^0*");
            this.recommendedStatusFooter = " [TWNv" + Regex.Replace(MyCommon.FileVersion.Replace(".", ""), "^0*", "") + "]";

            _history.Add(new PostingStatus());
            _hisIdx = 0;
            this.inReplyTo = null;

            //各種ダイアログ設定
            SearchDialog.Owner = this;
            UrlDialog.Owner = this;

            //新着バルーン通知のチェック状態設定
            NewPostPopMenuItem.Checked = _cfgCommon.NewAllPop;
            this.NotifyFileMenuItem.Checked = NewPostPopMenuItem.Checked;

            //新着取得時のリストスクロールをするか。trueならスクロールしない
            ListLockMenuItem.Checked = _cfgCommon.ListLock;
            this.LockListFileMenuItem.Checked = _cfgCommon.ListLock;
            //サウンド再生（タブ別設定より優先）
            this.PlaySoundMenuItem.Checked = this._cfgCommon.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this._cfgCommon.PlaySound;

            this.IdeographicSpaceToSpaceToolStripMenuItem.Checked = _cfgCommon.WideSpaceConvert;
            this.ToolStripFocusLockMenuItem.Checked = _cfgCommon.FocusLockToStatusText;

            //ウィンドウ設定
            this.ClientSize = ScaleBy(configScaleFactor, _cfgLocal.FormSize);
            _mySize = this.ClientSize; // サイズ保持（最小化・最大化されたまま終了した場合の対応用）
            _myLoc = _cfgLocal.FormLocation;
            //タイトルバー領域
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.DesktopLocation = _cfgLocal.FormLocation;
                Rectangle tbarRect = new Rectangle(this.Location, new Size(_mySize.Width, SystemInformation.CaptionHeight));
                bool outOfScreen = true;
                if (Screen.AllScreens.Length == 1)    //ハングするとの報告
                {
                    foreach (Screen scr in Screen.AllScreens)
                    {
                        if (!Rectangle.Intersect(tbarRect, scr.Bounds).IsEmpty)
                        {
                            outOfScreen = false;
                            break;
                        }
                    }
                    if (outOfScreen)
                    {
                        this.DesktopLocation = new Point(0, 0);
                        _myLoc = this.DesktopLocation;
                    }
                }
            }
            this.TopMost = this._cfgCommon.AlwaysTop;
            _mySpDis = ScaleBy(configScaleFactor.Height, _cfgLocal.SplitterDistance);
            _mySpDis2 = ScaleBy(configScaleFactor.Height, _cfgLocal.StatusTextHeight);
            if (_cfgLocal.PreviewDistance == -1)
            {
                _mySpDis3 = _mySize.Width - ScaleBy(this.CurrentScaleFactor.Width, 150);
                if (_mySpDis3 < 1) _mySpDis3 = ScaleBy(this.CurrentScaleFactor.Width, 50);
                _cfgLocal.PreviewDistance = _mySpDis3;
            }
            else
            {
                _mySpDis3 = ScaleBy(configScaleFactor.Width, _cfgLocal.PreviewDistance);
            }
            MultiLineMenuItem.Checked = _cfgLocal.StatusMultiline;
            //this.Tween_ClientSizeChanged(this, null);
            this.PlaySoundMenuItem.Checked = this._cfgCommon.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this._cfgCommon.PlaySound;
            //入力欄
            StatusText.Font = _fntInputFont;
            StatusText.ForeColor = _clInputFont;

            // SplitContainer2.Panel2MinSize を一行表示の入力欄の高さに合わせる (MS UI Gothic 12pt (96dpi) の場合は 19px)
            this.StatusText.Multiline = false; // _cfgLocal.StatusMultiline の設定は後で反映される
            this.SplitContainer2.Panel2MinSize = this.StatusText.Height;

            // 必要であれば、発言一覧と発言詳細部・入力欄の上下を入れ替える
            SplitContainer1.IsPanelInverted = !this._cfgCommon.StatusAreaAtBottom;

            //全新着通知のチェック状態により、Reply＆DMの新着通知有効無効切り替え（タブ別設定にするため削除予定）
            if (this._cfgCommon.UnreadManage == false)
            {
                ReadedStripMenuItem.Enabled = false;
                UnreadStripMenuItem.Enabled = false;
            }

            //リンク先URL表示部の初期化（画面左下）
            StatusLabelUrl.Text = "";
            //状態表示部の初期化（画面右下）
            StatusLabel.Text = "";
            StatusLabel.AutoToolTip = false;
            StatusLabel.ToolTipText = "";
            //文字カウンタ初期化
            lblLen.Text = this.GetRestStatusCount(this.FormatStatusText("")).ToString();

            this.JumpReadOpMenuItem.ShortcutKeyDisplayString = "Space";
            CopySTOTMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            CopyURLMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            CopyUserIdStripMenuItem.ShortcutKeyDisplayString = "Shift+Alt+C";

            // SourceLinkLabel のテキストが SplitContainer2.Panel2.AccessibleName にセットされるのを防ぐ
            // （タブオーダー順で SourceLinkLabel の次にある PostBrowser が TabStop = false となっているため、
            // さらに次のコントロールである SplitContainer2.Panel2 の AccessibleName がデフォルトで SourceLinkLabel のテキストになってしまう)
            this.SplitContainer2.Panel2.AccessibleName = "";

            ////////////////////////////////////////////////////////////////////////////////
            var sortOrder = (SortOrder)_cfgCommon.SortOrder;
            var mode = ComparerMode.Id;
            switch (_cfgCommon.SortColumn)
            {
                case 0:    //0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                case 5:
                case 6:
                    //ソートしない
                    mode = ComparerMode.Id;  //Idソートに読み替え
                    break;
                case 1:  //ニックネーム
                    mode = ComparerMode.Nickname;
                    break;
                case 2:  //本文
                    mode = ComparerMode.Data;
                    break;
                case 3:  //時刻=発言Id
                    mode = ComparerMode.Id;
                    break;
                case 4:  //名前
                    mode = ComparerMode.Name;
                    break;
                case 7:  //Source
                    mode = ComparerMode.Source;
                    break;
            }
            _statuses.SetSortMode(mode, sortOrder);
            ////////////////////////////////////////////////////////////////////////////////

            ApplyListViewIconSize(this._cfgCommon.IconSize);

            //<<<<<<<<タブ関連>>>>>>>
            // タブの位置を調整する
            SetTabAlignment();

            //デフォルトタブの存在チェック、ない場合には追加
            if (this._statuses.GetTabByType<HomeTabModel>() == null)
                this._statuses.AddTab(new HomeTabModel());

            if (this._statuses.GetTabByType<MentionsTabModel>() == null)
                this._statuses.AddTab(new MentionsTabModel());

            if (this._statuses.GetTabByType<DirectMessagesTabModel>() == null)
                this._statuses.AddTab(new DirectMessagesTabModel());

            if (this._statuses.GetTabByType<FavoritesTabModel>() == null)
                this._statuses.AddTab(new FavoritesTabModel());

            if (this._statuses.GetTabByType<MuteTabModel>() == null)
                this._statuses.AddTab(new MuteTabModel());

            foreach (var tab in _statuses.Tabs.Values)
            {
                // ミュートタブは表示しない
                if (tab.TabType == MyCommon.TabUsageType.Mute)
                    continue;

                if (!AddNewTab(tab, startup: true))
                    throw new TabException(Properties.Resources.TweenMain_LoadText1);
            }

            _curTab = ListTab.SelectedTab;
            _curItemIndex = -1;
            _curList = (DetailsListView)_curTab.Tag;

            if (this._cfgCommon.TabIconDisp)
            {
                ListTab.DrawMode = TabDrawMode.Normal;
            }
            else
            {
                ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                ListTab.DrawItem += ListTab_DrawItem;
                ListTab.ImageList = null;
            }

            if (this._cfgCommon.HotkeyEnabled)
            {
                //////グローバルホットキーの登録
                HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                if ((this._cfgCommon.HotkeyModifier & Keys.Alt) == Keys.Alt)
                    modKey |= HookGlobalHotkey.ModKeys.Alt;
                if ((this._cfgCommon.HotkeyModifier & Keys.Control) == Keys.Control)
                    modKey |= HookGlobalHotkey.ModKeys.Ctrl;
                if ((this._cfgCommon.HotkeyModifier & Keys.Shift) == Keys.Shift)
                    modKey |= HookGlobalHotkey.ModKeys.Shift;
                if ((this._cfgCommon.HotkeyModifier & Keys.LWin) == Keys.LWin)
                    modKey |= HookGlobalHotkey.ModKeys.Win;

                _hookGlobalHotkey.RegisterOriginalHotkey(this._cfgCommon.HotkeyKey, this._cfgCommon.HotkeyValue, modKey);
            }

            if (this._cfgCommon.IsUseNotifyGrowl)
                gh.RegisterGrowl();

            StatusLabel.Text = Properties.Resources.Form1_LoadText1;       //画面右下の状態表示を変更

            SetMainWindowTitle();
            SetNotifyIconText();

            if (!this._cfgCommon.MinimizeToTray || this.WindowState != FormWindowState.Minimized)
            {
                this.Visible = true;
            }

            //タイマー設定
            TimerTimeline.AutoReset = true;
            TimerTimeline.SynchronizingObject = this;
            //Recent取得間隔
            TimerTimeline.Interval = 1000;
            TimerTimeline.Enabled = true;
            //更新中アイコンアニメーション間隔
            TimerRefreshIcon.Interval = 200;
            TimerRefreshIcon.Enabled = true;

            _ignoreConfigSave = false;
            this.TweenMain_Resize(null, null);
            if (saveRequired) SaveConfigsAll(false);

            foreach (var ua in this._cfgCommon.UserAccounts)
            {
                if (ua.UserId == 0 && ua.Username.ToLowerInvariant() == tw.Username.ToLowerInvariant())
                {
                    ua.UserId = tw.UserId;
                    break;
                }
            }

            if (firstRun)
            {
                // 初回起動時だけ右下のメニューを目立たせる
                HashStripSplitButton.ShowDropDown();
            }
        }

        private void InitDetailHtmlFormat()
        {
            if (this._cfgCommon.IsMonospace)
            {
                detailHtmlFormatHeader = detailHtmlFormatHeaderMono;
                detailHtmlFormatFooter = detailHtmlFormatFooterMono;
            }
            else
            {
                detailHtmlFormatHeader = detailHtmlFormatHeaderColor;
                detailHtmlFormatFooter = detailHtmlFormatFooterColor;
            }

            detailHtmlFormatHeader = detailHtmlFormatHeader
                    .Replace("%FONT_FAMILY%", _fntDetail.Name)
                    .Replace("%FONT_SIZE%", _fntDetail.Size.ToString())
                    .Replace("%FONT_COLOR%", $"{_clDetail.R},{_clDetail.G},{_clDetail.B}")
                    .Replace("%LINK_COLOR%", $"{_clDetailLink.R},{_clDetailLink.G},{_clDetailLink.B}")
                    .Replace("%BG_COLOR%", $"{_clDetailBackcolor.R},{_clDetailBackcolor.G},{_clDetailBackcolor.B}")
                    .Replace("%BG_REPLY_COLOR%", $"{_clAtTo.R}, {_clAtTo.G}, {_clAtTo.B}");
        }

        private void ListTab_DrawItem(object sender, DrawItemEventArgs e)
        {
            string txt;
            try
            {
                txt = ListTab.TabPages[e.Index].Text;
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
                if (_statuses.Tabs[txt].UnreadCount > 0)
                    fore = Brushes.Red;
                else
                    fore = System.Drawing.SystemBrushes.ControlText;
            }
            catch (Exception)
            {
                fore = System.Drawing.SystemBrushes.ControlText;
            }
            e.Graphics.DrawString(txt, e.Font, fore, e.Bounds, sfTab);
        }

        private void LoadConfig()
        {
            _cfgCommon = SettingCommon.Load();
            SettingCommon.Instance = this._cfgCommon;
            if (_cfgCommon.UserAccounts == null || _cfgCommon.UserAccounts.Count == 0)
            {
                _cfgCommon.UserAccounts = new List<UserAccount>();
                if (!string.IsNullOrEmpty(_cfgCommon.UserName))
                {
                    UserAccount account = new UserAccount();
                    account.Username = _cfgCommon.UserName;
                    account.UserId = _cfgCommon.UserId;
                    account.Token = _cfgCommon.Token;
                    account.TokenSecret = _cfgCommon.TokenSecret;

                    _cfgCommon.UserAccounts.Add(account);
                }
            }

            _cfgLocal = SettingLocal.Load();

            // v1.2.4 以前の設定には ScaleDimension の項目がないため、現在の DPI と同じとして扱う
            if (_cfgLocal.ScaleDimension.IsEmpty)
                _cfgLocal.ScaleDimension = this.CurrentAutoScaleDimensions;

            var tabsSetting = SettingTabs.Load().Tabs;
            foreach (var tabSetting in tabsSetting)
            {
                TabModel tab;
                switch (tabSetting.TabType)
                {
                    case MyCommon.TabUsageType.Home:
                        tab = new HomeTabModel(tabSetting.TabName);
                        break;
                    case MyCommon.TabUsageType.Mentions:
                        tab = new MentionsTabModel(tabSetting.TabName);
                        break;
                    case MyCommon.TabUsageType.DirectMessage:
                        tab = new DirectMessagesTabModel(tabSetting.TabName);
                        break;
                    case MyCommon.TabUsageType.Favorites:
                        tab = new FavoritesTabModel(tabSetting.TabName);
                        break;
                    case MyCommon.TabUsageType.UserDefined:
                        tab = new FilterTabModel(tabSetting.TabName);
                        break;
                    case MyCommon.TabUsageType.UserTimeline:
                        tab = new UserTimelineTabModel(tabSetting.TabName, tabSetting.User);
                        break;
                    case MyCommon.TabUsageType.PublicSearch:
                        tab = new PublicSearchTabModel(tabSetting.TabName)
                        {
                            SearchWords = tabSetting.SearchWords,
                            SearchLang = tabSetting.SearchLang,
                        };
                        break;
                    case MyCommon.TabUsageType.Lists:
                        tab = new ListTimelineTabModel(tabSetting.TabName, tabSetting.ListInfo);
                        break;
                    case MyCommon.TabUsageType.Mute:
                        tab = new MuteTabModel(tabSetting.TabName);
                        break;
                    default:
                        continue;
                }

                tab.UnreadManage = tabSetting.UnreadManage;
                tab.Protected = tabSetting.Protected;
                tab.Notify = tabSetting.Notify;
                tab.SoundFile = tabSetting.SoundFile;

                if (tab.IsDistributableTabType)
                {
                    var filterTab = (FilterTabModel)tab;
                    filterTab.FilterArray = tabSetting.FilterArray;
                    filterTab.FilterModified = false;
                }

                if (this._statuses.ContainsTab(tab.TabName))
                    tab.TabName = this._statuses.MakeTabName("MyTab");

                this._statuses.AddTab(tab);
            }
            if (_statuses.Tabs.Count == 0)
            {
                _statuses.AddTab(new HomeTabModel());
                _statuses.AddTab(new MentionsTabModel());
                _statuses.AddTab(new DirectMessagesTabModel());
                _statuses.AddTab(new FavoritesTabModel());
            }
        }

        private void TimerInterval_Changed(object sender, IntervalChangedEventArgs e) //Handles SettingDialog.IntervalChanged
        {
            if (!TimerTimeline.Enabled) return;
            ResetTimers = e;
        }

        private IntervalChangedEventArgs ResetTimers = IntervalChangedEventArgs.ResetAll;

        private static int homeCounter = 0;
        private static int mentionCounter = 0;
        private static int dmCounter = 0;
        private static int pubSearchCounter = 0;
        private static int userTimelineCounter = 0;
        private static int listsCounter = 0;
        private static int usCounter = 0;
        private static int ResumeWait = 0;
        private static int refreshFollowers = 0;

        private async void TimerTimeline_Elapsed(object sender, EventArgs e)
        {
            if (homeCounter > 0) Interlocked.Decrement(ref homeCounter);
            if (mentionCounter > 0) Interlocked.Decrement(ref mentionCounter);
            if (dmCounter > 0) Interlocked.Decrement(ref dmCounter);
            if (pubSearchCounter > 0) Interlocked.Decrement(ref pubSearchCounter);
            if (userTimelineCounter > 0) Interlocked.Decrement(ref userTimelineCounter);
            if (listsCounter > 0) Interlocked.Decrement(ref listsCounter);
            if (usCounter > 0) Interlocked.Decrement(ref usCounter);
            Interlocked.Increment(ref refreshFollowers);

            var refreshTasks = new List<Task>();

            ////タイマー初期化
            if (ResetTimers.Timeline || homeCounter <= 0 && this._cfgCommon.TimelinePeriod > 0)
            {
                Interlocked.Exchange(ref homeCounter, this._cfgCommon.TimelinePeriod);
                if (!tw.IsUserstreamDataReceived && !ResetTimers.Timeline)
                    refreshTasks.Add(this.GetHomeTimelineAsync());
                ResetTimers.Timeline = false;
            }
            if (ResetTimers.Reply || mentionCounter <= 0 && this._cfgCommon.ReplyPeriod > 0)
            {
                Interlocked.Exchange(ref mentionCounter, this._cfgCommon.ReplyPeriod);
                if (!tw.IsUserstreamDataReceived && !ResetTimers.Reply)
                    refreshTasks.Add(this.GetReplyAsync());
                ResetTimers.Reply = false;
            }
            if (ResetTimers.DirectMessage || dmCounter <= 0 && this._cfgCommon.DMPeriod > 0)
            {
                Interlocked.Exchange(ref dmCounter, this._cfgCommon.DMPeriod);
                if (!tw.IsUserstreamDataReceived && !ResetTimers.DirectMessage)
                    refreshTasks.Add(this.GetDirectMessagesAsync());
                ResetTimers.DirectMessage = false;
            }
            if (ResetTimers.PublicSearch || pubSearchCounter <= 0 && this._cfgCommon.PubSearchPeriod > 0)
            {
                Interlocked.Exchange(ref pubSearchCounter, this._cfgCommon.PubSearchPeriod);
                if (!ResetTimers.PublicSearch)
                    refreshTasks.Add(this.GetPublicSearchAllAsync());
                ResetTimers.PublicSearch = false;
            }
            if (ResetTimers.UserTimeline || userTimelineCounter <= 0 && this._cfgCommon.UserTimelinePeriod > 0)
            {
                Interlocked.Exchange(ref userTimelineCounter, this._cfgCommon.UserTimelinePeriod);
                if (!ResetTimers.UserTimeline)
                    refreshTasks.Add(this.GetUserTimelineAllAsync());
                ResetTimers.UserTimeline = false;
            }
            if (ResetTimers.Lists || listsCounter <= 0 && this._cfgCommon.ListsPeriod > 0)
            {
                Interlocked.Exchange(ref listsCounter, this._cfgCommon.ListsPeriod);
                if (!ResetTimers.Lists)
                    refreshTasks.Add(this.GetListTimelineAllAsync());
                ResetTimers.Lists = false;
            }
            if (ResetTimers.UserStream || usCounter <= 0 && this._cfgCommon.UserstreamPeriod > 0)
            {
                Interlocked.Exchange(ref usCounter, this._cfgCommon.UserstreamPeriod);
                if (this.tw.UserStreamActive)
                    this.RefreshTimeline();
                ResetTimers.UserStream = false;
            }
            if (refreshFollowers > 6 * 3600)
            {
                Interlocked.Exchange(ref refreshFollowers, 0);
                refreshTasks.AddRange(new[]
                {
                    this.doGetFollowersMenu(),
                    this.RefreshNoRetweetIdsAsync(),
                    this.RefreshTwitterConfigurationAsync(),
                });
            }
            if (osResumed)
            {
                Interlocked.Increment(ref ResumeWait);
                if (ResumeWait > 30)
                {
                    osResumed = false;
                    Interlocked.Exchange(ref ResumeWait, 0);
                    refreshTasks.AddRange(new[]
                    {
                        this.GetHomeTimelineAsync(),
                        this.GetReplyAsync(),
                        this.GetDirectMessagesAsync(),
                        this.GetPublicSearchAllAsync(),
                        this.GetUserTimelineAllAsync(),
                        this.GetListTimelineAllAsync(),
                        this.doGetFollowersMenu(),
                        this.RefreshTwitterConfigurationAsync(),
                    });
                }
            }

            await Task.WhenAll(refreshTasks);
        }

        private void RefreshTimeline()
        {
            // 現在表示中のタブのスクロール位置を退避
            var curListScroll = this.SaveListViewScroll(this._curList, this._statuses.Tabs[this._curTab.Text]);

            // 各タブのリスト上の選択位置などを退避
            var listSelections = this.SaveListViewSelection();

            //更新確定
            PostClass[] notifyPosts;
            string soundFile;
            int addCount;
            bool newMentionOrDm;
            bool isDelete;
            addCount = _statuses.SubmitUpdate(out soundFile, out notifyPosts, out newMentionOrDm, out isDelete);

            if (MyCommon._endingFlag) return;

            //リストに反映＆選択状態復元
            try
            {
                foreach (TabPage tab in ListTab.TabPages)
                {
                    DetailsListView lst = (DetailsListView)tab.Tag;
                    TabModel tabInfo = _statuses.Tabs[tab.Text];
                    if (isDelete || lst.VirtualListSize != tabInfo.AllCount)
                    {
                        using (ControlTransaction.Update(lst))
                        {
                            if (lst.Equals(_curList))
                            {
                                this.PurgeListViewItemCache();
                            }
                            try
                            {
                                lst.VirtualListSize = tabInfo.AllCount; //リスト件数更新
                            }
                            catch (Exception)
                            {
                                //アイコン描画不具合あり？
                            }

                            // 選択位置などを復元
                            this.RestoreListViewSelection(lst, tabInfo, listSelections[tabInfo.TabName]);
                        }
                    }
                    if (tabInfo.UnreadCount > 0)
                        if (this._cfgCommon.TabIconDisp)
                            if (tab.ImageIndex == -1) tab.ImageIndex = 0; //タブアイコン
                }
                if (!this._cfgCommon.TabIconDisp) ListTab.Refresh();
            }
            catch (Exception)
            {
                //ex.Data["Msg"] = "Ref1, UseAPI=" + SettingDialog.UseAPI.ToString();
                //throw;
            }

            // スクロール位置を復元
            this.RestoreListViewScroll(this._curList, this._statuses.Tabs[this._curTab.Text], curListScroll);

            //新着通知
            NotifyNewPosts(notifyPosts, soundFile, addCount, newMentionOrDm);

            SetMainWindowTitle();
            if (!StatusLabelUrl.Text.StartsWith("http", StringComparison.Ordinal)) SetStatusLabelUrl();

            HashSupl.AddRangeItem(tw.GetHashList());

        }

        internal struct ListViewScroll
        {
            public ScrollLockMode ScrollLockMode { get; set; }
            public long? TopItemStatusId { get; set; }
        }

        internal enum ScrollLockMode
        {
            /// <summary>固定しない</summary>
            None,

            /// <summary>最上部に固定する</summary>
            FixedToTop,

            /// <summary>最下部に固定する</summary>
            FixedToBottom,

            /// <summary><see cref="ListViewScroll.TopItemStatusId"/> の位置に固定する</summary>
            FixedToItem,
        }

        /// <summary>
        /// <see cref="ListView"/> のスクロール位置に関する情報を <see cref="ListViewScroll"/> として返します
        /// </summary>
        private ListViewScroll SaveListViewScroll(DetailsListView listView, TabModel tab)
        {
            var listScroll = new ListViewScroll
            {
                ScrollLockMode = this.GetScrollLockMode(listView),
            };

            if (listScroll.ScrollLockMode == ScrollLockMode.FixedToItem)
            {
                var topItem = listView.TopItem;
                if (topItem != null)
                    listScroll.TopItemStatusId = tab.GetStatusIdAt(topItem.Index);
            }

            return listScroll;
        }

        private ScrollLockMode GetScrollLockMode(DetailsListView listView)
        {
            if (this._statuses.SortMode == ComparerMode.Id)
            {
                if (this._statuses.SortOrder == SortOrder.Ascending)
                {
                    // Id昇順
                    if (this.ListLockMenuItem.Checked)
                        return ScrollLockMode.None;

                    // 最下行が表示されていたら、最下行へ強制スクロール。最下行が表示されていなかったら制御しない

                    // 一番下に表示されているアイテム
                    var bottomItem = listView.GetItemAt(0, listView.ClientSize.Height - 1);
                    if (bottomItem == null || bottomItem.Index == listView.VirtualListSize - 1)
                        return ScrollLockMode.FixedToBottom;
                    else
                        return ScrollLockMode.None;
                }
                else
                {
                    // Id降順
                    if (this.ListLockMenuItem.Checked)
                        return ScrollLockMode.FixedToItem;

                    // 最上行が表示されていたら、制御しない。最上行が表示されていなかったら、現在表示位置へ強制スクロール
                    var topItem = listView.TopItem;
                    if (topItem == null || topItem.Index == 0)
                        return ScrollLockMode.FixedToTop;
                    else
                        return ScrollLockMode.FixedToItem;
                }
            }
            else
            {
                return ScrollLockMode.FixedToItem;
            }
        }

        internal struct ListViewSelection
        {
            public long[] SelectedStatusIds { get; set; }
            public long? SelectionMarkStatusId { get; set; }
            public long? FocusedStatusId { get; set; }
        }

        /// <summary>
        /// <see cref="ListView"/> の選択状態を <see cref="ListViewSelection"/> として返します
        /// </summary>
        private IReadOnlyDictionary<string, ListViewSelection> SaveListViewSelection()
        {
            var listsDict = new Dictionary<string, ListViewSelection>();

            foreach (var tabPage in this.ListTab.TabPages.Cast<TabPage>())
            {
                var listView = (DetailsListView)tabPage.Tag;
                var tab = _statuses.Tabs[tabPage.Text];

                ListViewSelection listStatus;
                if (listView.VirtualListSize != 0)
                {
                    listStatus = new ListViewSelection
                    {
                        SelectedStatusIds = this.GetSelectedStatusIds(listView, tab),
                        FocusedStatusId = this.GetFocusedStatusId(listView, tab),
                        SelectionMarkStatusId = this.GetSelectionMarkStatusId(listView, tab),
                    };
                }
                else
                {
                    listStatus = new ListViewSelection
                    {
                        SelectedStatusIds = new long[0],
                        SelectionMarkStatusId = null,
                        FocusedStatusId = null,
                    };
                }

                listsDict[tab.TabName] = listStatus;
            }

            return listsDict;
        }

        private long[] GetSelectedStatusIds(DetailsListView listView, TabModel tab)
        {
            var selectedIndices = listView.SelectedIndices;
            if (selectedIndices.Count > 0 && selectedIndices.Count < 61)
                return tab.GetStatusIdAt(selectedIndices.Cast<int>());
            else
                return null;
        }

        private long? GetFocusedStatusId(DetailsListView listView, TabModel tab)
        {
            var focusedItem = listView.FocusedItem;

            return focusedItem != null ? tab.GetStatusIdAt(focusedItem.Index) : (long?)null;
        }

        private long? GetSelectionMarkStatusId(DetailsListView listView, TabModel tab)
        {
            var selectionMarkIndex = listView.SelectionMark;

            return selectionMarkIndex != -1 ? tab.GetStatusIdAt(selectionMarkIndex) : (long?)null;
        }

        /// <summary>
        /// <see cref="SaveListViewScroll"/> によって保存されたスクロール位置を復元します
        /// </summary>
        private void RestoreListViewScroll(DetailsListView listView, TabModel tab, ListViewScroll listScroll)
        {
            if (listView.VirtualListSize == 0)
                return;

            switch (listScroll.ScrollLockMode)
            {
                case ScrollLockMode.FixedToTop:
                    listView.EnsureVisible(0);
                    break;
                case ScrollLockMode.FixedToBottom:
                    listView.EnsureVisible(listView.VirtualListSize - 1);
                    break;
                case ScrollLockMode.FixedToItem:
                    var topIndex = listScroll.TopItemStatusId != null ? tab.IndexOf(listScroll.TopItemStatusId.Value) : -1;
                    if (topIndex != -1)
                        listView.TopItem = listView.Items[topIndex];
                    break;
                case ScrollLockMode.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// <see cref="SaveListViewSelection"/> によって保存された選択状態を復元します
        /// </summary>
        private void RestoreListViewSelection(DetailsListView listView, TabModel tab, ListViewSelection listSelection)
        {
            // status_id から ListView 上のインデックスに変換
            int[] selectedIndices = null;
            if (listSelection.SelectedStatusIds != null)
                selectedIndices = tab.IndexOf(listSelection.SelectedStatusIds).Where(x => x != -1).ToArray();

            var focusedIndex = -1;
            if (listSelection.FocusedStatusId != null)
                focusedIndex = tab.IndexOf(listSelection.FocusedStatusId.Value);

            var selectionMarkIndex = -1;
            if (listSelection.SelectionMarkStatusId != null)
                selectionMarkIndex = tab.IndexOf(listSelection.SelectionMarkStatusId.Value);

            this.SelectListItem(listView, selectedIndices, focusedIndex, selectionMarkIndex);
        }

        private bool BalloonRequired()
        {
            Twitter.FormattedEvent ev = new Twitter.FormattedEvent();
            ev.Eventtype = MyCommon.EVENTTYPE.None;

            return BalloonRequired(ev);
        }

        private bool IsEventNotifyAsEventType(MyCommon.EVENTTYPE type)
        {
            if (type == MyCommon.EVENTTYPE.None)
                return true;

            if (!this._cfgCommon.EventNotifyEnabled)
                return false;

            return this._cfgCommon.EventNotifyFlag.HasFlag(type);
        }

        private bool IsMyEventNotityAsEventType(Twitter.FormattedEvent ev)
        {
            if (!ev.IsMe)
                return true;

            return this._cfgCommon.IsMyEventNotifyFlag.HasFlag(ev.Eventtype);
        }

        private bool BalloonRequired(Twitter.FormattedEvent ev)
        {
            if (this._initial)
                return false;

            if (NativeMethods.IsScreenSaverRunning())
                return false;

            // 「新着通知」が無効
            if (!this.NewPostPopMenuItem.Checked)
            {
                // 「新着通知が無効でもイベントを通知する」にも該当しない
                if (!this._cfgCommon.ForceEventNotify || ev.Eventtype == MyCommon.EVENTTYPE.None)
                    return false;
            }

            // 「画面最小化・アイコン時のみバルーンを表示する」が有効
            if (this._cfgCommon.LimitBalloon)
            {
                if (this.WindowState != FormWindowState.Minimized && this.Visible && Form.ActiveForm != null)
                    return false;
            }

            return this.IsEventNotifyAsEventType(ev.Eventtype) && this.IsMyEventNotityAsEventType(ev);
        }

        private void NotifyNewPosts(PostClass[] notifyPosts, string soundFile, int addCount, bool newMentions)
        {
            if (this._cfgCommon.ReadOwnPost)
            {
                if (notifyPosts != null && notifyPosts.Length > 0 && notifyPosts.All(x => x.UserId == tw.UserId))
                    return;
            }

            //新着通知
            if (BalloonRequired())
            {
                if (notifyPosts != null && notifyPosts.Length > 0)
                {
                    //Growlは一個ずつばらして通知。ただし、3ポスト以上あるときはまとめる
                    if (this._cfgCommon.IsUseNotifyGrowl)
                    {
                        StringBuilder sb = new StringBuilder();
                        bool reply = false;
                        bool dm = false;

                        foreach (PostClass post in notifyPosts)
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
                            switch (this._cfgCommon.NameBalloon)
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

                            StringBuilder title = new StringBuilder();
                            GrowlHelper.NotifyType nt;
                            if (this._cfgCommon.DispUsername)
                            {
                                title.Append(tw.Username);
                                title.Append(" - ");
                            }
                            else
                            {
                                //title.Clear();
                            }
                            if (dm)
                            {
                                //NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                                //NotifyIcon1.BalloonTipTitle += Application.ProductName + " [DM] " + Properties.Resources.RefreshDirectMessageText1 + " " + addCount.ToString() + Properties.Resources.RefreshDirectMessageText2;
                                title.Append(Application.ProductName);
                                title.Append(" [DM] ");
                                title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                                nt = GrowlHelper.NotifyType.DirectMessage;
                            }
                            else if (reply)
                            {
                                //NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                                //NotifyIcon1.BalloonTipTitle += Application.ProductName + " [Reply!] " + Properties.Resources.RefreshTimelineText1 + " " + addCount.ToString() + Properties.Resources.RefreshTimelineText2;
                                title.Append(Application.ProductName);
                                title.Append(" [Reply!] ");
                                title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                                nt = GrowlHelper.NotifyType.Reply;
                            }
                            else
                            {
                                //NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                                //NotifyIcon1.BalloonTipTitle += Application.ProductName + " " + Properties.Resources.RefreshTimelineText1 + " " + addCount.ToString() + Properties.Resources.RefreshTimelineText2;
                                title.Append(Application.ProductName);
                                title.Append(" ");
                                title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                                nt = GrowlHelper.NotifyType.Notify;
                            }
                            string bText = sb.ToString();
                            if (string.IsNullOrEmpty(bText)) return;

                            var image = this.IconCache.TryGetFromCache(post.ImageUrl);
                            gh.Notify(nt, post.StatusId.ToString(), title.ToString(), bText, image == null ? null : image.Image, post.ImageUrl);
                        }
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        bool reply = false;
                        bool dm = false;
                        foreach (PostClass post in notifyPosts)
                        {
                            if (post.IsReply && !post.IsExcludeReply) reply = true;
                            if (post.IsDm) dm = true;
                            if (sb.Length > 0) sb.Append(System.Environment.NewLine);
                            switch (this._cfgCommon.NameBalloon)
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
                        //if (SettingDialog.DispUsername) { NotifyIcon1.BalloonTipTitle = tw.Username + " - "; } else { NotifyIcon1.BalloonTipTitle = ""; }
                        StringBuilder title = new StringBuilder();
                        ToolTipIcon ntIcon;
                        if (this._cfgCommon.DispUsername)
                        {
                            title.Append(tw.Username);
                            title.Append(" - ");
                        }
                        else
                        {
                            //title.Clear();
                        }
                        if (dm)
                        {
                            //NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                            //NotifyIcon1.BalloonTipTitle += Application.ProductName + " [DM] " + Properties.Resources.RefreshDirectMessageText1 + " " + addCount.ToString() + Properties.Resources.RefreshDirectMessageText2;
                            ntIcon = ToolTipIcon.Warning;
                            title.Append(Application.ProductName);
                            title.Append(" [DM] ");
                            title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                        }
                        else if (reply)
                        {
                            //NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                            //NotifyIcon1.BalloonTipTitle += Application.ProductName + " [Reply!] " + Properties.Resources.RefreshTimelineText1 + " " + addCount.ToString() + Properties.Resources.RefreshTimelineText2;
                            ntIcon = ToolTipIcon.Warning;
                            title.Append(Application.ProductName);
                            title.Append(" [Reply!] ");
                            title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                        }
                        else
                        {
                            //NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                            //NotifyIcon1.BalloonTipTitle += Application.ProductName + " " + Properties.Resources.RefreshTimelineText1 + " " + addCount.ToString() + Properties.Resources.RefreshTimelineText2;
                            ntIcon = ToolTipIcon.Info;
                            title.Append(Application.ProductName);
                            title.Append(" ");
                            title.AppendFormat(Properties.Resources.RefreshTimeline_NotifyText, addCount);
                        }
                        string bText = sb.ToString();
                        if (string.IsNullOrEmpty(bText)) return;
                        //NotifyIcon1.BalloonTipText = sb.ToString();
                        //NotifyIcon1.ShowBalloonTip(500);
                        NotifyIcon1.BalloonTipTitle = title.ToString();
                        NotifyIcon1.BalloonTipText = bText;
                        NotifyIcon1.BalloonTipIcon = ntIcon;
                        NotifyIcon1.ShowBalloonTip(500);
                    }
                }
            }

            //サウンド再生
            if (!_initial && this._cfgCommon.PlaySound && !string.IsNullOrEmpty(soundFile))
            {
                try
                {
                    string dir = Application.StartupPath;
                    if (Directory.Exists(Path.Combine(dir, "Sounds")))
                    {
                        dir = Path.Combine(dir, "Sounds");
                    }
                    using (SoundPlayer player = new SoundPlayer(Path.Combine(dir, soundFile)))
                    {
                        player.Play();
                    }
                }
                catch (Exception)
                {
                }
            }

            //mentions新着時に画面ブリンク
            if (!_initial && this._cfgCommon.BlinkNewMentions && newMentions && Form.ActiveForm == null)
            {
                NativeMethods.FlashMyWindow(this.Handle, NativeMethods.FlashSpecification.FlashTray, 3);
            }
        }

        private void MyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_curList == null || !_curList.Equals(sender) || _curList.SelectedIndices.Count != 1) return;

            _curItemIndex = _curList.SelectedIndices[0];
            if (_curItemIndex > _curList.VirtualListSize - 1) return;

            try
            {
                this._curPost = GetCurTabPost(_curItemIndex);
            }
            catch (ArgumentException)
            {
                return;
            }

            this.PushSelectPostChain();

            this._statuses.SetReadAllTab(_curPost.StatusId, read: true);
            //キャッシュの書き換え
            ChangeCacheStyleRead(true, _curItemIndex);   //既読へ（フォント、文字色）

            ColorizeList();
            _colorize = true;
        }

        private void ChangeCacheStyleRead(bool Read, int Index)
        {
            var tabInfo = _statuses.Tabs[_curTab.Text];
            //Read:true=既読 false=未読
            //未読管理していなかったら既読として扱う
            if (!tabInfo.UnreadManage ||
               !this._cfgCommon.UnreadManage) Read = true;

            var listCache = this._listItemCache;
            if (listCache == null)
                return;

            // キャッシュに含まれていないアイテムは対象外
            ListViewItem itm;
            PostClass post;
            if (!listCache.TryGetValue(Index, out itm, out post))
                return;

            ChangeItemStyleRead(Read, itm, post, ((DetailsListView)_curTab.Tag));
        }

        private void ChangeItemStyleRead(bool Read, ListViewItem Item, PostClass Post, DetailsListView DList)
        {
            Font fnt;
            //フォント
            if (Read)
            {
                fnt = _fntReaded;
                Item.SubItems[5].Text = "";
            }
            else
            {
                fnt = _fntUnread;
                Item.SubItems[5].Text = "★";
            }
            //文字色
            Color cl;
            if (Post.IsFav)
                cl = _clFav;
            else if (Post.RetweetedId != null)
                cl = _clRetweet;
            else if (Post.IsOwl && (Post.IsDm || this._cfgCommon.OneWayLove))
                cl = _clOWL;
            else if (Read || !this._cfgCommon.UseUnreadStyle)
                cl = _clReaded;
            else
                cl = _clUnread;

            if (DList == null || Item.Index == -1)
            {
                Item.ForeColor = cl;
                if (this._cfgCommon.UseUnreadStyle)
                    Item.Font = fnt;
            }
            else
            {
                DList.Update();
                if (this._cfgCommon.UseUnreadStyle)
                    DList.ChangeItemFontAndColor(Item.Index, cl, fnt);
                else
                    DList.ChangeItemForeColor(Item.Index, cl);
                //if (_itemCache != null) DList.RedrawItems(_itemCacheIndex, _itemCacheIndex + _itemCache.Length - 1, false);
            }
        }

        private void ColorizeList()
        {
            //Index:更新対象のListviewItem.Index。Colorを返す。
            //-1は全キャッシュ。Colorは返さない（ダミーを戻す）
            PostClass _post;
            if (_anchorFlag)
                _post = _anchorPost;
            else
                _post = _curPost;

            if (_post == null) return;

            var listCache = this._listItemCache;
            if (listCache == null)
                return;

            var index = listCache.StartIndex;
            foreach (var cachedPost in listCache.Post)
            {
                var backColor = this.JudgeColor(_post, cachedPost);
                this._curList.ChangeItemBackColor(index++, backColor);
            }
        }

        private void ColorizeList(ListViewItem Item, int Index)
        {
            //Index:更新対象のListviewItem.Index。Colorを返す。
            //-1は全キャッシュ。Colorは返さない（ダミーを戻す）
            PostClass _post;
            if (_anchorFlag)
                _post = _anchorPost;
            else
                _post = _curPost;

            PostClass tPost = GetCurTabPost(Index);

            if (_post == null) return;

            if (Item.Index == -1)
                Item.BackColor = JudgeColor(_post, tPost);
            else
                _curList.ChangeItemBackColor(Item.Index, JudgeColor(_post, tPost));
        }

        private Color JudgeColor(PostClass BasePost, PostClass TargetPost)
        {
            Color cl;
            if (TargetPost.StatusId == BasePost.InReplyToStatusId)
                //@先
                cl = _clAtTo;
            else if (TargetPost.IsMe)
                //自分=発言者
                cl = _clSelf;
            else if (TargetPost.IsReply)
                //自分宛返信
                cl = _clAtSelf;
            else if (BasePost.ReplyToList.Contains(TargetPost.ScreenName.ToLowerInvariant()))
                //返信先
                cl = _clAtFromTarget;
            else if (TargetPost.ReplyToList.Contains(BasePost.ScreenName.ToLowerInvariant()))
                //その人への返信
                cl = _clAtTarget;
            else if (TargetPost.ScreenName.Equals(BasePost.ScreenName, StringComparison.OrdinalIgnoreCase))
                //発言者
                cl = _clTarget;
            else
                //その他
                cl = _clListBackcolor;

            return cl;
        }

        private async void PostButton_Click(object sender, EventArgs e)
        {
            if (StatusText.Text.Trim().Length == 0)
            {
                if (!ImageSelector.Enabled)
                {
                    await this.DoRefresh();
                    return;
                }
            }

            if (this.ExistCurrentPost && StatusText.Text.Trim() == string.Format("RT @{0}: {1}", _curPost.ScreenName, _curPost.TextFromApi))
            {
                DialogResult rtResult = MessageBox.Show(string.Format(Properties.Resources.PostButton_Click1, Environment.NewLine),
                                                               "Retweet",
                                                               MessageBoxButtons.YesNoCancel,
                                                               MessageBoxIcon.Question);
                switch (rtResult)
                {
                    case DialogResult.Yes:
                        StatusText.Text = "";
                        await this.doReTweetOfficial(false);
                        return;
                    case DialogResult.Cancel:
                        return;
                }
            }

            var inReplyToStatusId = this.inReplyTo?.Item1;
            var inReplyToScreenName = this.inReplyTo?.Item2;
            _history[_history.Count - 1] = new PostingStatus(StatusText.Text, inReplyToStatusId, inReplyToScreenName);

            if (this._cfgCommon.Nicoms)
            {
                StatusText.SelectionStart = StatusText.Text.Length;
                await UrlConvertAsync(MyCommon.UrlConverter.Nicoms);
            }
            //if (SettingDialog.UrlConvertAuto)
            //{
            //    StatusText.SelectionStart = StatusText.Text.Length;
            //    UrlConvertAutoToolStripMenuItem_Click(null, null);
            //}
            //else if (SettingDialog.Nicoms)
            //{
            //    StatusText.SelectionStart = StatusText.Text.Length;
            //    UrlConvert(UrlConverter.Nicoms);
            //}
            StatusText.SelectionStart = StatusText.Text.Length;
            CheckReplyTo(StatusText.Text);

            var statusText = this.FormatStatusText(this.StatusText.Text);

            if (this.GetRestStatusCount(statusText) < 0)
            {
                // 文字数制限を超えているが強制的に投稿するか
                var ret = MessageBox.Show(Properties.Resources.PostLengthOverMessage1, Properties.Resources.PostLengthOverMessage2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (ret != DialogResult.OK)
                    return;
            }

            var status = new PostingStatus();
            status.status = statusText;

            status.inReplyToId = this.inReplyTo?.Item1;
            status.inReplyToName = this.inReplyTo?.Item2;
            if (ImageSelector.Visible)
            {
                //画像投稿
                if (!ImageSelector.TryGetSelectedMedia(out status.imageService, out status.mediaItems))
                    return;
            }

            this.inReplyTo = null;
            StatusText.Text = "";
            _history.Add(new PostingStatus());
            _hisIdx = _history.Count - 1;
            if (!ToolStripFocusLockMenuItem.Checked)
                ((Control)ListTab.SelectedTab.Tag).Focus();
            urlUndoBuffer = null;
            UrlUndoToolStripMenuItem.Enabled = false;  //Undoをできないように設定

            //Google検索（試験実装）
            if (StatusText.Text.StartsWith("Google:", StringComparison.OrdinalIgnoreCase) && StatusText.Text.Trim().Length > 7)
            {
                string tmp = string.Format(Properties.Resources.SearchItem2Url, Uri.EscapeDataString(StatusText.Text.Substring(7)));
                await this.OpenUriInBrowserAsync(tmp);
            }

            await this.PostMessageAsync(status);
        }

        private void EndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyCommon._endingFlag = true;
            this.Close();
        }

        private void TweenMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this._cfgCommon.CloseToExit && e.CloseReason == CloseReason.UserClosing && MyCommon._endingFlag == false)
            {
                //_endingFlag=false:フォームの×ボタン
                e.Cancel = true;
                this.Visible = false;
            }
            else
            {
                _hookGlobalHotkey.UnregisterAllOriginalHotkey();
                _ignoreConfigSave = true;
                MyCommon._endingFlag = true;
                TimerTimeline.Enabled = false;
                TimerRefreshIcon.Enabled = false;
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

        private Task GetHomeTimelineAsync()
        {
            return this.GetHomeTimelineAsync(loadMore: false);
        }

        private async Task GetHomeTimelineAsync(bool loadMore)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var homeTab = this._statuses.GetTabByType<HomeTabModel>();
                await homeTab.RefreshAsync(this.tw, loadMore, this._initial, this.workerProgress);

                this.RefreshTimeline();
            }
            catch (WebApiException ex)
            {
                this._myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(GetTimeline)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private Task GetReplyAsync()
        {
            return this.GetReplyAsync(loadMore: false);
        }

        private async Task GetReplyAsync(bool loadMore)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var replyTab = this._statuses.GetTabByType<MentionsTabModel>();
                await replyTab.RefreshAsync(this.tw, loadMore, this._initial, this.workerProgress);

                this.RefreshTimeline();
            }
            catch (WebApiException ex)
            {
                this._myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(GetTimeline)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private Task GetDirectMessagesAsync()
        {
            return this.GetDirectMessagesAsync(loadMore: false);
        }

        private async Task GetDirectMessagesAsync(bool loadMore)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var dmTab = this._statuses.GetTabByType<DirectMessagesTabModel>();
                await dmTab.RefreshAsync(this.tw, loadMore, this._initial, this.workerProgress);

                this.RefreshTimeline();
            }
            catch (WebApiException ex)
            {
                this._myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(GetDirectMessage)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private Task GetFavoritesAsync()
        {
            return this.GetFavoritesAsync(loadMore: false);
        }

        private async Task GetFavoritesAsync(bool loadMore)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var favTab = this._statuses.GetTabByType<FavoritesTabModel>();
                await favTab.RefreshAsync(this.tw, loadMore, this._initial, this.workerProgress);

                this.RefreshTimeline();
            }
            catch (WebApiException ex)
            {
                this._myStatusError = true;
                this.StatusLabel.Text = ex.Message;
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private Task GetPublicSearchAllAsync()
        {
            var tabs = this._statuses.GetTabsByType<PublicSearchTabModel>();

            return this.GetPublicSearchAsync(tabs, loadMore: false);
        }

        private Task GetPublicSearchAsync(PublicSearchTabModel tab)
        {
            return this.GetPublicSearchAsync(tab, loadMore: false);
        }

        private Task GetPublicSearchAsync(PublicSearchTabModel tab, bool loadMore)
        {
            return this.GetPublicSearchAsync(new[] { tab }, loadMore);
        }

        private async Task GetPublicSearchAsync(IEnumerable<PublicSearchTabModel> tabs, bool loadMore)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                foreach (var tab in tabs)
                {
                    try
                    {
                        await tab.RefreshAsync(this.tw, loadMore, this._initial, this.workerProgress);
                    }
                    catch (WebApiException ex)
                    {
                        this._myStatusError = true;
                        this.StatusLabel.Text = $"Err:{ex.Message}(GetSearch)";
                    }
                }

                this.RefreshTimeline();
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private Task GetUserTimelineAllAsync()
        {
            var tabs = this._statuses.GetTabsByType<UserTimelineTabModel>();

            return this.GetUserTimelineAsync(tabs, loadMore: false);
        }

        private Task GetUserTimelineAsync(UserTimelineTabModel tab)
        {
            return this.GetUserTimelineAsync(tab, loadMore: false);
        }

        private Task GetUserTimelineAsync(UserTimelineTabModel tab, bool loadMore)
        {
            return this.GetUserTimelineAsync(new[] { tab }, loadMore);
        }

        private async Task GetUserTimelineAsync(IEnumerable<UserTimelineTabModel> tabs, bool loadMore)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                foreach (var tab in tabs)
                {
                    try
                    {
                        await tab.RefreshAsync(this.tw, loadMore, this._initial, this.workerProgress);
                    }
                    catch (WebApiException ex)
                    {
                        this._myStatusError = true;
                        this.StatusLabel.Text = $"Err:{ex.Message}(GetUserTimeline)";
                    }
                }

                this.RefreshTimeline();
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private Task GetListTimelineAllAsync()
        {
            var tabs = this._statuses.GetTabsByType<ListTimelineTabModel>();

            return this.GetListTimelineAsync(tabs, loadMore: false);
        }

        private Task GetListTimelineAsync(ListTimelineTabModel tab)
        {
            return this.GetListTimelineAsync(tab, loadMore: false);
        }

        private Task GetListTimelineAsync(ListTimelineTabModel tab, bool loadMore)
        {
            return this.GetListTimelineAsync(new[] { tab }, loadMore);
        }

        private async Task GetListTimelineAsync(IEnumerable<ListTimelineTabModel> tabs, bool loadMore)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                foreach (var tab in tabs)
                {
                    try
                    {
                        await tab.RefreshAsync(this.tw, loadMore, this._initial, this.workerProgress);
                    }
                    catch (WebApiException ex)
                    {
                        this._myStatusError = true;
                        this.StatusLabel.Text = $"Err:{ex.Message}(GetListStatus)";
                    }
                }

                this.RefreshTimeline();
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task GetRelatedTweetsAsync(RelatedPostsTabModel tab)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                await tab.RefreshAsync(this.tw, this._initial, this.workerProgress);

                this.RefreshTimeline();
            }
            catch (WebApiException ex)
            {
                this._myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(GetRelatedTweets)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task FavAddAsync(long statusId, TabModel tab)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                await this.FavAddAsyncInternal(progress, this.workerCts.Token, statusId, tab);
            }
            catch (WebApiException ex)
            {
                this._myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(PostFavAdd)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task FavAddAsyncInternal(IProgress<string> p, CancellationToken ct, long statusId, TabModel tab)
        {
            if (ct.IsCancellationRequested)
                return;

            if (!CheckAccountValid())
                throw new WebApiException("Auth error. Check your account");

            PostClass post;
            if (!tab.Posts.TryGetValue(statusId, out post))
                return;

            if (post.IsFav)
                return;

            await Task.Run(async () =>
            {
                p.Report(string.Format(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText15, 0, 1, 0));

                try
                {
                    await this.twitterApi.FavoritesCreate(post.RetweetedId ?? post.StatusId)
                        .IgnoreResponse()
                        .ConfigureAwait(false);

                    if (this._cfgCommon.RestrictFavCheck)
                    {
                        var status = await this.twitterApi.StatusesShow(post.RetweetedId ?? post.StatusId)
                            .ConfigureAwait(false);

                        if (status.Favorited != true)
                            throw new WebApiException("NG(Restricted?)");
                    }

                    this._favTimestamps.Add(DateTime.Now);

                    // TLでも取得済みならfav反映
                    if (this._statuses.ContainsKey(statusId))
                    {
                        var postTl = this._statuses[statusId];
                        postTl.IsFav = true;

                        var favTab = this._statuses.GetTabByType(MyCommon.TabUsageType.Favorites);
                        favTab.AddPostQueue(postTl);
                    }

                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                    foreach (var tb in this._statuses.GetTabsInnerStorageType())
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
                var oneHour = DateTime.Now - TimeSpan.FromHours(1);
                foreach (var i in MyCommon.CountDown(this._favTimestamps.Count - 1, 0))
                {
                    if (this._favTimestamps[i] < oneHour)
                        this._favTimestamps.RemoveAt(i);
                }

                this._statuses.DistributePosts();
            });

            if (ct.IsCancellationRequested)
                return;

            this.RefreshTimeline();

            if (this._curList != null && this._curTab != null && this._curTab.Text == tab.TabName)
            {
                using (ControlTransaction.Update(this._curList))
                {
                    var idx = tab.IndexOf(statusId);
                    if (idx != -1)
                        this.ChangeCacheStyleRead(post.IsRead, idx);
                }

                if (statusId == this._curPost.StatusId)
                    await this.DispSelectedPost(true); // 選択アイテム再表示
            }
        }

        private async Task FavRemoveAsync(IReadOnlyList<long> statusIds, TabModel tab)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                await this.FavRemoveAsyncInternal(progress, this.workerCts.Token, statusIds, tab);
            }
            catch (WebApiException ex)
            {
                this._myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(PostFavRemove)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task FavRemoveAsyncInternal(IProgress<string> p, CancellationToken ct, IReadOnlyList<long> statusIds, TabModel tab)
        {
            if (ct.IsCancellationRequested)
                return;

            if (!CheckAccountValid())
                throw new WebApiException("Auth error. Check your account");

            var successIds = new List<long>();

            await Task.Run(async () =>
            {
                //スレッド処理はしない
                var allCount = 0;
                var failedCount = 0;
                foreach (var statusId in statusIds)
                {
                    allCount++;

                    var post = tab.Posts[statusId];

                    p.Report(string.Format(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText17, allCount, statusIds.Count, failedCount));

                    if (!post.IsFav)
                        continue;

                    try
                    {
                        await this.twitterApi.FavoritesDestroy(post.RetweetedId ?? post.StatusId)
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

                    if (this._statuses.ContainsKey(statusId))
                    {
                        this._statuses[statusId].IsFav = false;
                    }

                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                    foreach (var tb in this._statuses.GetTabsInnerStorageType())
                    {
                        if (tb.Contains(statusId))
                            tb.Posts[statusId].IsFav = false;
                    }
                }
            });

            if (ct.IsCancellationRequested)
                return;

            var favTab = this._statuses.GetTabByType(MyCommon.TabUsageType.Favorites);
            foreach (var statusId in successIds)
            {
                // ツイートが削除された訳ではないので IsDeleted はセットしない
                favTab.EnqueueRemovePost(statusId, setIsDeleted: false);
            }

            this.RefreshTimeline();

            if (this._curList != null && this._curTab != null && this._curTab.Text == tab.TabName)
            {
                if (tab.TabType == MyCommon.TabUsageType.Favorites)
                {
                    // 色変えは不要
                }
                else
                {
                    using (ControlTransaction.Update(this._curList))
                    {
                        foreach (var statusId in successIds)
                        {
                            var idx = tab.IndexOf(statusId);
                            if (idx == -1)
                                continue;

                            var post = tab.Posts[statusId];
                            this.ChangeCacheStyleRead(post.IsRead, idx);
                        }
                    }

                    if (successIds.Contains(this._curPost.StatusId))
                        await this.DispSelectedPost(true); // 選択アイテム再表示
                }
            }
        }

        private async Task PostMessageAsync(PostingStatus status)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                await this.PostMessageAsyncInternal(progress, this.workerCts.Token, status);
            }
            catch (WebApiException ex)
            {
                this._myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(PostMessage)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task PostMessageAsyncInternal(IProgress<string> p, CancellationToken ct, PostingStatus status)
        {
            if (ct.IsCancellationRequested)
                return;

            if (!CheckAccountValid())
                throw new WebApiException("Auth error. Check your account");

            p.Report("Posting...");

            var errMsg = "";

            try
            {
                await Task.Run(async () =>
                {
                    if (status.mediaItems == null || status.mediaItems.Length == 0)
                    {
                        await this.tw.PostStatus(status.status, status.inReplyToId)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        var service = ImageSelector.GetService(status.imageService);
                        await service.PostStatusAsync(status.status, status.inReplyToId, status.mediaItems)
                            .ConfigureAwait(false);
                    }
                });

                p.Report(Properties.Resources.PostWorker_RunWorkerCompletedText4);
            }
            catch (WebApiException ex)
            {
                // 処理は中断せずエラーの表示のみ行う
                errMsg = $"Err:{ex.Message}(PostMessage)";
                p.Report(errMsg);
                this._myStatusError = true;
            }
            finally
            {
                // 使い終わった MediaItem は破棄する
                if (status.mediaItems != null)
                {
                    foreach (var disposableItem in status.mediaItems.OfType<IDisposable>())
                    {
                        disposableItem.Dispose();
                    }
                }
            }

            if (ct.IsCancellationRequested)
                return;

            if (!string.IsNullOrEmpty(errMsg) &&
                !errMsg.StartsWith("OK:", StringComparison.Ordinal) &&
                !errMsg.StartsWith("Warn:", StringComparison.Ordinal))
            {
                var ret = MessageBox.Show(
                    string.Format(
                        "{0}   --->   [ " + errMsg + " ]" + Environment.NewLine +
                        "\"" + status.status + "\"" + Environment.NewLine +
                        "{1}",
                        Properties.Resources.StatusUpdateFailed1,
                        Properties.Resources.StatusUpdateFailed2),
                    "Failed to update status",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Question);

                if (ret == DialogResult.Retry)
                {
                    await this.PostMessageAsync(status);
                }
                else
                {
                    // 連投モードのときだけEnterイベントが起きないので強制的に背景色を戻す
                    if (this.ToolStripFocusLockMenuItem.Checked)
                        this.StatusText_Enter(this.StatusText, EventArgs.Empty);
                }
                return;
            }

            this._postTimestamps.Add(DateTime.Now);

            var oneHour = DateTime.Now - TimeSpan.FromHours(1);
            foreach (var i in MyCommon.CountDown(this._postTimestamps.Count - 1, 0))
            {
                if (this._postTimestamps[i] < oneHour)
                    this._postTimestamps.RemoveAt(i);
            }

            if (!this.HashMgr.IsPermanent && !string.IsNullOrEmpty(this.HashMgr.UseHash))
            {
                this.HashMgr.ClearHashtag();
                this.HashStripSplitButton.Text = "#[-]";
                this.HashToggleMenuItem.Checked = false;
                this.HashToggleToolStripMenuItem.Checked = false;
            }

            this.SetMainWindowTitle();

            if (this._cfgCommon.PostAndGet)
            {
                if (this.tw.UserStreamActive)
                    this.RefreshTimeline();
                else
                    await this.GetHomeTimelineAsync();
            }
        }

        private async Task RetweetAsync(IReadOnlyList<long> statusIds)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                await this.RetweetAsyncInternal(progress, this.workerCts.Token, statusIds);
            }
            catch (WebApiException ex)
            {
                this._myStatusError = true;
                this.StatusLabel.Text = $"Err:{ex.Message}(PostRetweet)";
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task RetweetAsyncInternal(IProgress<string> p, CancellationToken ct, IReadOnlyList<long> statusIds)
        {
            if (ct.IsCancellationRequested)
                return;

            if (!CheckAccountValid())
                throw new WebApiException("Auth error. Check your account");

            bool read;
            if (!this._cfgCommon.UnreadManage)
                read = true;
            else
                read = this._initial && this._cfgCommon.Read;

            p.Report("Posting...");

            var retweetTasks = from statusId in statusIds
                               select this.tw.PostRetweet(statusId, read);

            await Task.WhenAll(retweetTasks)
                .ConfigureAwait(false);

            if (ct.IsCancellationRequested)
                return;

            p.Report(Properties.Resources.PostWorker_RunWorkerCompletedText4);

            this._postTimestamps.Add(DateTime.Now);

            var oneHour = DateTime.Now - TimeSpan.FromHours(1);
            foreach (var i in MyCommon.CountDown(this._postTimestamps.Count - 1, 0))
            {
                if (this._postTimestamps[i] < oneHour)
                    this._postTimestamps.RemoveAt(i);
            }

            if (this._cfgCommon.PostAndGet && !this.tw.UserStreamActive)
                await this.GetHomeTimelineAsync();
        }

        private async Task RefreshFollowerIdsAsync()
        {
            await this.workerSemaphore.WaitAsync();
            try
            {
                this.StatusLabel.Text = Properties.Resources.UpdateFollowersMenuItem1_ClickText1;

                await this.tw.RefreshFollowerIds();

                this.StatusLabel.Text = Properties.Resources.UpdateFollowersMenuItem1_ClickText3;

                this.RefreshTimeline();
                this.PurgeListViewItemCache();
                this._curList?.Refresh();
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
                await this.tw.RefreshConfiguration();

                if (this.tw.Configuration.PhotoSizeLimit != 0)
                {
                    foreach (var service in this.ImageSelector.GetServices())
                    {
                        service.UpdateTwitterConfiguration(this.tw.Configuration);
                    }
                }

                this.PurgeListViewItemCache();

                this._curList?.Refresh();
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
                await tw.RefreshMuteUserIdsAsync();
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
                    this.WindowState = _formWindowState;
                }
                this.Activate();
                this.BringToFront();
            }
        }

        private async void MyList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (this._cfgCommon.ListDoubleClickAction)
            {
                case 0:
                    MakeReplyOrDirectStatus();
                    break;
                case 1:
                    await this.FavoriteChange(true);
                    break;
                case 2:
                    if (_curPost != null)
                        await this.ShowUserStatus(_curPost.ScreenName, false);
                    break;
                case 3:
                    ShowUserTimeline();
                    break;
                case 4:
                    ShowRelatedStatusesMenuItem_Click(null, null);
                    break;
                case 5:
                    MoveToHomeToolStripMenuItem_Click(null, null);
                    break;
                case 6:
                    StatusOpenMenuItem_Click(null, null);
                    break;
                case 7:
                    //動作なし
                    break;
            }
        }

        private async void FavAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await this.FavoriteChange(true);
        }

        private async void FavRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await this.FavoriteChange(false);
        }


        private async void FavoriteRetweetMenuItem_Click(object sender, EventArgs e)
        {
            await this.FavoritesRetweetOfficial();
        }

        private async void FavoriteRetweetUnofficialMenuItem_Click(object sender, EventArgs e)
        {
            await this.FavoritesRetweetUnofficial();
        }

        private async Task FavoriteChange(bool FavAdd, bool multiFavoriteChangeDialogEnable = true)
        {
            TabModel tab;
            if (!this._statuses.Tabs.TryGetValue(this._curTab.Text, out tab))
                return;

            //trueでFavAdd,falseでFavRemove
            if (tab.TabType == MyCommon.TabUsageType.DirectMessage || _curList.SelectedIndices.Count == 0
                || !this.ExistCurrentPost) return;

            if (this._curList.SelectedIndices.Count > 1)
            {
                if (FavAdd)
                {
                    // 複数ツイートの一括ふぁぼは禁止
                    // https://support.twitter.com/articles/76915#favoriting
                    MessageBox.Show(string.Format(Properties.Resources.FavoriteLimitCountText, 1));
                    _DoFavRetweetFlags = false;
                    return;
                }
                else
                {
                    if (multiFavoriteChangeDialogEnable)
                    {
                        var confirm = MessageBox.Show(Properties.Resources.FavRemoveToolStripMenuItem_ClickText1,
                            Properties.Resources.FavRemoveToolStripMenuItem_ClickText2,
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                        if (confirm == DialogResult.Cancel)
                            return;
                    }
                }
            }

            if (FavAdd)
            {
                var selectedPost = this.GetCurTabPost(_curList.SelectedIndices[0]);
                if (selectedPost.IsFav)
                {
                    this.StatusLabel.Text = Properties.Resources.FavAddToolStripMenuItem_ClickText4;
                    return;
                }

                await this.FavAddAsync(selectedPost.StatusId, tab);
            }
            else
            {
                var selectedPosts = this._curList.SelectedIndices.Cast<int>()
                    .Select(x => this.GetCurTabPost(x))
                    .Where(x => x.IsFav);

                var statusIds = selectedPosts.Select(x => x.StatusId).ToArray();
                if (statusIds.Length == 0)
                {
                    this.StatusLabel.Text = Properties.Resources.FavRemoveToolStripMenuItem_ClickText4;
                    return;
                }

                await this.FavRemoveAsync(statusIds, tab);
            }
        }

        private PostClass GetCurTabPost(int Index)
        {
            var listCache = this._listItemCache;
            if (listCache != null)
            {
                ListViewItem item;
                PostClass post;
                if (listCache.TryGetValue(Index, out item, out post))
                    return post;
            }

            return _statuses.Tabs[_curTab.Text][Index];
        }

        private async void MoveToHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_curList.SelectedIndices.Count > 0)
                await this.OpenUriInBrowserAsync(MyCommon.TwitterUrl + GetCurTabPost(_curList.SelectedIndices[0]).ScreenName);
            else if (_curList.SelectedIndices.Count == 0)
                await this.OpenUriInBrowserAsync(MyCommon.TwitterUrl);
        }

        private async void MoveToFavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_curList.SelectedIndices.Count > 0)
                await this.OpenUriInBrowserAsync(MyCommon.TwitterUrl + "#!/" + GetCurTabPost(_curList.SelectedIndices[0]).ScreenName + "/favorites");
        }

        private void TweenMain_ClientSizeChanged(object sender, EventArgs e)
        {
            if ((!_initialLayout) && this.Visible)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    _mySize = this.ClientSize;
                    _mySpDis = this.SplitContainer1.SplitterDistance;
                    _mySpDis3 = this.SplitContainer3.SplitterDistance;
                    if (StatusText.Multiline) _mySpDis2 = this.StatusText.Height;
                    ModifySettingLocal = true;
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
            if (this._iconCol)
                return ComparerMode.Id;

            switch (columnIndex)
            {
                case 1: // ニックネーム
                    return ComparerMode.Nickname;
                case 2: // 本文
                    return ComparerMode.Data;
                case 3: // 時刻=発言Id
                    return ComparerMode.Id;
                case 4: // 名前
                    return ComparerMode.Name;
                case 7: // Source
                    return ComparerMode.Source;
                default:
                    // 0:アイコン, 5:未読マーク, 6:プロテクト・フィルターマーク
                    return null;
            }
        }

        /// <summary>
        /// 発言一覧の指定した位置の列でソートする
        /// </summary>
        /// <param name="columnIndex">ソートする列の位置 (表示上の順序で指定)</param>
        private void SetSortColumnByDisplayIndex(int columnIndex)
        {
            // 表示上の列の位置から ColumnHeader を求める
            var col = this._curList.Columns.Cast<ColumnHeader>()
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
            var col = this._curList.Columns.Cast<ColumnHeader>()
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
            if (this._cfgCommon.SortOrderLock)
                return;

            this._statuses.ToggleSortOrder(sortColumn);
            this.InitColumnText();

            var list = this._curList;
            if (_iconCol)
            {
                list.Columns[0].Text = this.ColumnText[0];
                list.Columns[1].Text = this.ColumnText[2];
            }
            else
            {
                for (var i = 0; i <= 7; i++)
                {
                    list.Columns[i].Text = this.ColumnText[i];
                }
            }

            this.PurgeListViewItemCache();

            var tab = this._statuses.Tabs[this._curTab.Text];
            if (tab.AllCount > 0 && this._curPost != null)
            {
                var idx = tab.IndexOf(this._curPost.StatusId);
                if (idx > -1)
                {
                    this.SelectListItem(list, idx);
                    list.EnsureVisible(idx);
                }
            }
            list.Refresh();

            this.ModifySettingCommon = true;
        }

        private void TweenMain_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _myLoc = this.DesktopLocation;
                ModifySettingLocal = true;
            }
        }

        private void ContextMenuOperate_Opening(object sender, CancelEventArgs e)
        {
            if (ListTab.SelectedTab == null) return;
            if (_statuses == null || _statuses.Tabs == null || !_statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text)) return;
            if (!this.ExistCurrentPost)
            {
                ReplyStripMenuItem.Enabled = false;
                ReplyAllStripMenuItem.Enabled = false;
                DMStripMenuItem.Enabled = false;
                ShowProfileMenuItem.Enabled = false;
                ShowUserTimelineContextMenuItem.Enabled = false;
                ListManageUserContextToolStripMenuItem2.Enabled = false;
                MoveToFavToolStripMenuItem.Enabled = false;
                TabMenuItem.Enabled = false;
                IDRuleMenuItem.Enabled = false;
                SourceRuleMenuItem.Enabled = false;
                ReadedStripMenuItem.Enabled = false;
                UnreadStripMenuItem.Enabled = false;
            }
            else
            {
                ShowProfileMenuItem.Enabled = true;
                ListManageUserContextToolStripMenuItem2.Enabled = true;
                ReplyStripMenuItem.Enabled = true;
                ReplyAllStripMenuItem.Enabled = true;
                DMStripMenuItem.Enabled = true;
                ShowUserTimelineContextMenuItem.Enabled = true;
                MoveToFavToolStripMenuItem.Enabled = true;
                TabMenuItem.Enabled = true;
                IDRuleMenuItem.Enabled = true;
                SourceRuleMenuItem.Enabled = true;
                ReadedStripMenuItem.Enabled = true;
                UnreadStripMenuItem.Enabled = true;
            }
            if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.DirectMessage || !this.ExistCurrentPost || _curPost.IsDm)
            {
                FavAddToolStripMenuItem.Enabled = false;
                FavRemoveToolStripMenuItem.Enabled = false;
                StatusOpenMenuItem.Enabled = false;
                FavorareMenuItem.Enabled = false;
                ShowRelatedStatusesMenuItem.Enabled = false;

                ReTweetStripMenuItem.Enabled = false;
                ReTweetUnofficialStripMenuItem.Enabled = false;
                QuoteStripMenuItem.Enabled = false;
                FavoriteRetweetContextMenu.Enabled = false;
                FavoriteRetweetUnofficialContextMenu.Enabled = false;
            }
            else
            {
                FavAddToolStripMenuItem.Enabled = true;
                FavRemoveToolStripMenuItem.Enabled = true;
                StatusOpenMenuItem.Enabled = true;
                FavorareMenuItem.Enabled = true;
                ShowRelatedStatusesMenuItem.Enabled = true;  //PublicSearchの時問題出るかも

                if (_curPost.IsMe)
                {
                    ReTweetStripMenuItem.Enabled = false;  //公式RTは無効に
                    ReTweetUnofficialStripMenuItem.Enabled = true;
                    QuoteStripMenuItem.Enabled = true;
                    FavoriteRetweetContextMenu.Enabled = false;  //公式RTは無効に
                    FavoriteRetweetUnofficialContextMenu.Enabled = true;
                }
                else
                {
                    if (_curPost.IsProtect)
                    {
                        ReTweetStripMenuItem.Enabled = false;
                        ReTweetUnofficialStripMenuItem.Enabled = false;
                        QuoteStripMenuItem.Enabled = false;
                        FavoriteRetweetContextMenu.Enabled = false;
                        FavoriteRetweetUnofficialContextMenu.Enabled = false;
                    }
                    else
                    {
                        ReTweetStripMenuItem.Enabled = true;
                        ReTweetUnofficialStripMenuItem.Enabled = true;
                        QuoteStripMenuItem.Enabled = true;
                        FavoriteRetweetContextMenu.Enabled = true;
                        FavoriteRetweetUnofficialContextMenu.Enabled = true;
                    }
                }
            }
            //if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType != MyCommon.TabUsageType.Favorites)
            //{
            //    RefreshMoreStripMenuItem.Enabled = true;
            //}
            //else
            //{
            //    RefreshMoreStripMenuItem.Enabled = false;
            //}
            if (!this.ExistCurrentPost
                || _curPost.InReplyToStatusId == null)
            {
                RepliedStatusOpenMenuItem.Enabled = false;
            }
            else
            {
                RepliedStatusOpenMenuItem.Enabled = true;
            }
            if (!this.ExistCurrentPost || string.IsNullOrEmpty(_curPost.RetweetedBy))
            {
                MoveToRTHomeMenuItem.Enabled = false;
            }
            else
            {
                MoveToRTHomeMenuItem.Enabled = true;
            }

            if (this.ExistCurrentPost)
            {
                this.DeleteStripMenuItem.Enabled = this._curPost.CanDeleteBy(this.tw.UserId);
                if (this._curPost.RetweetedByUserId == this.tw.UserId)
                    this.DeleteStripMenuItem.Text = Properties.Resources.DeleteMenuText2;
                else
                    this.DeleteStripMenuItem.Text = Properties.Resources.DeleteMenuText1;
            }
        }

        private void ReplyStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReplyOrDirectStatus(false, true);
        }

        private void DMStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReplyOrDirectStatus(false, false);
        }

        private async Task doStatusDelete()
        {
            if (this._curTab == null || this._curList == null)
                return;

            if (this._curList.SelectedIndices.Count == 0)
                return;

            var posts = this._curList.SelectedIndices.Cast<int>()
                .Select(x => this.GetCurTabPost(x))
                .ToArray();

            // 選択されたツイートの中に削除可能なものが一つでもあるか
            if (!posts.Any(x => x.CanDeleteBy(this.tw.UserId)))
                return;

            var ret = MessageBox.Show(this,
                string.Format(Properties.Resources.DeleteStripMenuItem_ClickText1, Environment.NewLine),
                Properties.Resources.DeleteStripMenuItem_ClickText2,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (ret != DialogResult.OK)
                return;

            var focusedIndex = this._curList.FocusedItem?.Index ?? this._curList.TopItem?.Index ?? 0;

            using (ControlTransaction.Cursor(this, Cursors.WaitCursor))
            {
                Exception lastException = null;
                foreach (var post in posts)
                {
                    if (!post.CanDeleteBy(this.tw.UserId))
                        continue;

                    try
                    {
                        if (post.IsDm)
                        {
                            await this.twitterApi.DirectMessagesDestroy(post.StatusId)
                                .IgnoreResponse();
                        }
                        else
                        {
                            if (post.RetweetedId != null && post.UserId == this.tw.UserId)
                                // 他人に RT された自分のツイート
                                await this.twitterApi.StatusesDestroy(post.RetweetedId.Value)
                                    .IgnoreResponse();
                            else
                                // 自分のツイート or 自分が RT したツイート
                                await this.twitterApi.StatusesDestroy(post.StatusId)
                                    .IgnoreResponse();
                        }
                    }
                    catch (WebApiException ex)
                    {
                        lastException = ex;
                        continue;
                    }

                    this._statuses.RemovePostFromAllTabs(post.StatusId, setIsDeleted: true);
                }

                if (lastException == null)
                    this.StatusLabel.Text = Properties.Resources.DeleteStripMenuItem_ClickText4; // 成功
                else
                    this.StatusLabel.Text = Properties.Resources.DeleteStripMenuItem_ClickText3; // 失敗

                this.PurgeListViewItemCache();
                this._curPost = null;
                this._curItemIndex = -1;

                foreach (var tabPage in this.ListTab.TabPages.Cast<TabPage>())
                {
                    var listView = (DetailsListView)tabPage.Tag;
                    var tab = this._statuses.Tabs[tabPage.Text];

                    using (ControlTransaction.Update(listView))
                    {
                        listView.VirtualListSize = tab.AllCount;

                        if (tabPage == this._curTab)
                        {
                            listView.SelectedIndices.Clear();

                            if (tab.AllCount != 0)
                            {
                                int selectedIndex;
                                if (tab.AllCount - 1 > focusedIndex && focusedIndex > -1)
                                    selectedIndex = focusedIndex;
                                else
                                    selectedIndex = tab.AllCount - 1;

                                listView.SelectedIndices.Add(selectedIndex);
                                listView.EnsureVisible(selectedIndex);
                                listView.FocusedItem = listView.Items[selectedIndex];
                            }
                        }
                    }

                    if (this._cfgCommon.TabIconDisp && tab.UnreadCount == 0)
                    {
                        if (tabPage.ImageIndex == 0)
                            tabPage.ImageIndex = -1; // タブアイコン
                    }
                }

                if (!this._cfgCommon.TabIconDisp)
                    this.ListTab.Refresh();
            }
        }

        private async void DeleteStripMenuItem_Click(object sender, EventArgs e)
        {
            await this.doStatusDelete();
        }

        private void ReadedStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Update(this._curList))
            {
                foreach (int idx in _curList.SelectedIndices)
                {
                    var post = this._statuses.Tabs[this._curTab.Text][idx];
                    this._statuses.SetReadAllTab(post.StatusId, read: true);
                    ChangeCacheStyleRead(true, idx);
                }
                ColorizeList();
            }
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (_statuses.Tabs[tb.Text].UnreadCount == 0)
                {
                    if (this._cfgCommon.TabIconDisp)
                    {
                        if (tb.ImageIndex == 0) tb.ImageIndex = -1; //タブアイコン
                    }
                }
            }
            if (!this._cfgCommon.TabIconDisp) ListTab.Refresh();
        }

        private void UnreadStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Update(this._curList))
            {
                foreach (int idx in _curList.SelectedIndices)
                {
                    var post = this._statuses.Tabs[this._curTab.Text][idx];
                    this._statuses.SetReadAllTab(post.StatusId, read: false);
                    ChangeCacheStyleRead(false, idx);
                }
                ColorizeList();
            }
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (_statuses.Tabs[tb.Text].UnreadCount > 0)
                {
                    if (this._cfgCommon.TabIconDisp)
                    {
                        if (tb.ImageIndex == -1) tb.ImageIndex = 0; //タブアイコン
                    }
                }
            }
            if (!this._cfgCommon.TabIconDisp) ListTab.Refresh();
        }

        private async void RefreshStripMenuItem_Click(object sender, EventArgs e)
        {
            await this.DoRefresh();
        }

        private async Task DoRefresh()
        {
            if (_curTab != null)
            {
                TabModel tab;
                if (!this._statuses.Tabs.TryGetValue(this._curTab.Text, out tab))
                    return;

                switch (_statuses.Tabs[_curTab.Text].TabType)
                {
                    case MyCommon.TabUsageType.Mentions:
                        await this.GetReplyAsync();
                        break;
                    case MyCommon.TabUsageType.DirectMessage:
                        await this.GetDirectMessagesAsync();
                        break;
                    case MyCommon.TabUsageType.Favorites:
                        await this.GetFavoritesAsync();
                        break;
                    //case MyCommon.TabUsageType.Profile:
                        //// TODO
                    case MyCommon.TabUsageType.PublicSearch:
                        var searchTab = (PublicSearchTabModel)tab;
                        if (string.IsNullOrEmpty(searchTab.SearchWords)) return;
                        await this.GetPublicSearchAsync(searchTab);
                        break;
                    case MyCommon.TabUsageType.UserTimeline:
                        await this.GetUserTimelineAsync((UserTimelineTabModel)tab);
                        break;
                    case MyCommon.TabUsageType.Lists:
                        var listTab = (ListTimelineTabModel)tab;
                        if (listTab.ListInfo == null || listTab.ListInfo.Id == 0) return;
                        await this.GetListTimelineAsync(listTab);
                        break;
                    default:
                        await this.GetHomeTimelineAsync();
                        break;
                }
            }
            else
            {
                await this.GetHomeTimelineAsync();
            }
        }

        private async Task DoRefreshMore()
        {
            //ページ指定をマイナス1に
            if (_curTab != null)
            {
                TabModel tab;
                if (!this._statuses.Tabs.TryGetValue(this._curTab.Text, out tab))
                    return;

                switch (_statuses.Tabs[_curTab.Text].TabType)
                {
                    case MyCommon.TabUsageType.Mentions:
                        await this.GetReplyAsync(loadMore: true);
                        break;
                    case MyCommon.TabUsageType.DirectMessage:
                        await this.GetDirectMessagesAsync(loadMore: true);
                        break;
                    case MyCommon.TabUsageType.Favorites:
                        await this.GetFavoritesAsync(loadMore: true);
                        break;
                    case MyCommon.TabUsageType.Profile:
                        //// TODO
                        break;
                    case MyCommon.TabUsageType.PublicSearch:
                        var searchTab = (PublicSearchTabModel)tab;
                        if (string.IsNullOrEmpty(searchTab.SearchWords)) return;
                        await this.GetPublicSearchAsync(searchTab, loadMore: true);
                        break;
                    case MyCommon.TabUsageType.UserTimeline:
                        await this.GetUserTimelineAsync((UserTimelineTabModel)tab, loadMore: true);
                        break;
                    case MyCommon.TabUsageType.Lists:
                        var listTab = (ListTimelineTabModel)tab;
                        if (listTab.ListInfo == null || listTab.ListInfo.Id == 0) return;
                        await this.GetListTimelineAsync(listTab, loadMore: true);
                        break;
                    default:
                        await this.GetHomeTimelineAsync(loadMore: true);
                        break;
                }
            }
            else
            {
                await this.GetHomeTimelineAsync(loadMore: true);
            }
        }

        private DialogResult ShowSettingDialog(bool showTaskbarIcon = false)
        {
            DialogResult result = DialogResult.Abort;

            using (var settingDialog = new AppendSettingDialog())
            {
                settingDialog.Icon = this.MainIcon;
                settingDialog.Owner = this;
                settingDialog.ShowInTaskbar = showTaskbarIcon;
                settingDialog.IntervalChanged += this.TimerInterval_Changed;

                settingDialog.tw = this.tw;
                settingDialog.twitterApi = this.twitterApi;

                settingDialog.LoadConfig(this._cfgCommon, this._cfgLocal);

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
                    lock (_syncObject)
                    {
                        settingDialog.SaveConfig(this._cfgCommon, this._cfgLocal);
                    }
                }
            }

            return result;
        }

        private async void SettingStripMenuItem_Click(object sender, EventArgs e)
        {
            // 設定画面表示前のユーザー情報
            var oldUser = new { tw.AccessToken, tw.AccessTokenSecret, tw.Username, tw.UserId };

            var oldIconSz = this._cfgCommon.IconSize;

            if (ShowSettingDialog() == DialogResult.OK)
            {
                lock (_syncObject)
                {
                    tw.RestrictFavCheck = this._cfgCommon.RestrictFavCheck;
                    tw.ReadOwnPost = this._cfgCommon.ReadOwnPost;
                    ShortUrl.Instance.DisableExpanding = !this._cfgCommon.TinyUrlResolve;
                    ShortUrl.Instance.BitlyId = this._cfgCommon.BilyUser;
                    ShortUrl.Instance.BitlyKey = this._cfgCommon.BitlyPwd;
                    TwitterApiConnection.RestApiHost = this._cfgCommon.TwitterApiHost;

                    Networking.DefaultTimeout = TimeSpan.FromSeconds(this._cfgCommon.DefaultTimeOut);
                    Networking.SetWebProxy(this._cfgLocal.ProxyType,
                        this._cfgLocal.ProxyAddress, this._cfgLocal.ProxyPort,
                        this._cfgLocal.ProxyUser, this._cfgLocal.ProxyPassword);
                    Networking.ForceIPv4 = this._cfgCommon.ForceIPv4;

                    ImageSelector.Reset(tw, this.tw.Configuration);

                    try
                    {
                        if (this._cfgCommon.TabIconDisp)
                        {
                            ListTab.DrawItem -= ListTab_DrawItem;
                            ListTab.DrawMode = TabDrawMode.Normal;
                            ListTab.ImageList = this.TabImage;
                        }
                        else
                        {
                            ListTab.DrawItem -= ListTab_DrawItem;
                            ListTab.DrawItem += ListTab_DrawItem;
                            ListTab.DrawMode = TabDrawMode.OwnerDrawFixed;
                            ListTab.ImageList = null;
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
                        if (!this._cfgCommon.UnreadManage)
                        {
                            ReadedStripMenuItem.Enabled = false;
                            UnreadStripMenuItem.Enabled = false;
                            if (this._cfgCommon.TabIconDisp)
                            {
                                foreach (TabPage myTab in ListTab.TabPages)
                                {
                                    myTab.ImageIndex = -1;
                                }
                            }
                        }
                        else
                        {
                            ReadedStripMenuItem.Enabled = true;
                            UnreadStripMenuItem.Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListTab(UnreadManage)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    // タブの表示位置の決定
                    SetTabAlignment();

                    SplitContainer1.IsPanelInverted = !this._cfgCommon.StatusAreaAtBottom;

                    var imgazyobizinet = ThumbnailGenerator.ImgAzyobuziNetInstance;
                    imgazyobizinet.Enabled = this._cfgCommon.EnableImgAzyobuziNet;
                    imgazyobizinet.DisabledInDM = this._cfgCommon.ImgAzyobuziNetDisabledInDM;

                    this.PlaySoundMenuItem.Checked = this._cfgCommon.PlaySound;
                    this.PlaySoundFileMenuItem.Checked = this._cfgCommon.PlaySound;
                    _fntUnread = this._cfgLocal.FontUnread;
                    _clUnread = this._cfgLocal.ColorUnread;
                    _fntReaded = this._cfgLocal.FontRead;
                    _clReaded = this._cfgLocal.ColorRead;
                    _clFav = this._cfgLocal.ColorFav;
                    _clOWL = this._cfgLocal.ColorOWL;
                    _clRetweet = this._cfgLocal.ColorRetweet;
                    _fntDetail = this._cfgLocal.FontDetail;
                    _clDetail = this._cfgLocal.ColorDetail;
                    _clDetailLink = this._cfgLocal.ColorDetailLink;
                    _clDetailBackcolor = this._cfgLocal.ColorDetailBackcolor;
                    _clSelf = this._cfgLocal.ColorSelf;
                    _clAtSelf = this._cfgLocal.ColorAtSelf;
                    _clTarget = this._cfgLocal.ColorTarget;
                    _clAtTarget = this._cfgLocal.ColorAtTarget;
                    _clAtFromTarget = this._cfgLocal.ColorAtFromTarget;
                    _clAtTo = this._cfgLocal.ColorAtTo;
                    _clListBackcolor = this._cfgLocal.ColorListBackcolor;
                    _clInputBackcolor = this._cfgLocal.ColorInputBackcolor;
                    _clInputFont = this._cfgLocal.ColorInputFont;
                    _fntInputFont = this._cfgLocal.FontInputFont;
                    _brsBackColorMine.Dispose();
                    _brsBackColorAt.Dispose();
                    _brsBackColorYou.Dispose();
                    _brsBackColorAtYou.Dispose();
                    _brsBackColorAtFromTarget.Dispose();
                    _brsBackColorAtTo.Dispose();
                    _brsBackColorNone.Dispose();
                    _brsBackColorMine = new SolidBrush(_clSelf);
                    _brsBackColorAt = new SolidBrush(_clAtSelf);
                    _brsBackColorYou = new SolidBrush(_clTarget);
                    _brsBackColorAtYou = new SolidBrush(_clAtTarget);
                    _brsBackColorAtFromTarget = new SolidBrush(_clAtFromTarget);
                    _brsBackColorAtTo = new SolidBrush(_clAtTo);
                    _brsBackColorNone = new SolidBrush(_clListBackcolor);

                    try
                    {
                        if (StatusText.Focused) StatusText.BackColor = _clInputBackcolor;
                        StatusText.Font = _fntInputFont;
                        StatusText.ForeColor = _clInputFont;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    try
                    {
                        InitDetailHtmlFormat();
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "Font";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    try
                    {
                        foreach (TabPage tb in ListTab.TabPages)
                        {
                            if (this._cfgCommon.TabIconDisp)
                            {
                                if (_statuses.Tabs[tb.Text].UnreadCount == 0)
                                    tb.ImageIndex = -1;
                                else
                                    tb.ImageIndex = 0;
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
                        var oldIconCol = _iconCol;

                        if (this._cfgCommon.IconSize != oldIconSz)
                            ApplyListViewIconSize(this._cfgCommon.IconSize);

                        foreach (TabPage tp in ListTab.TabPages)
                        {
                            DetailsListView lst = (DetailsListView)tp.Tag;

                            using (ControlTransaction.Update(lst))
                            {
                                lst.GridLines = this._cfgCommon.ShowGrid;
                                lst.Font = _fntReaded;
                                lst.BackColor = _clListBackcolor;

                                if (_iconCol != oldIconCol)
                                    ResetColumns(lst);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Instance"] = "ListView(IconSize)";
                        ex.Data["IsTerminatePermission"] = false;
                        throw;
                    }

                    SetMainWindowTitle();
                    SetNotifyIconText();

                    this.PurgeListViewItemCache();
                    _curList?.Refresh();
                    ListTab.Refresh();

                    _hookGlobalHotkey.UnregisterAllOriginalHotkey();
                    if (this._cfgCommon.HotkeyEnabled)
                    {
                        ///グローバルホットキーの登録。設定で変更可能にするかも
                        HookGlobalHotkey.ModKeys modKey = HookGlobalHotkey.ModKeys.None;
                        if ((this._cfgCommon.HotkeyModifier & Keys.Alt) == Keys.Alt)
                            modKey |= HookGlobalHotkey.ModKeys.Alt;
                        if ((this._cfgCommon.HotkeyModifier & Keys.Control) == Keys.Control)
                            modKey |= HookGlobalHotkey.ModKeys.Ctrl;
                        if ((this._cfgCommon.HotkeyModifier & Keys.Shift) == Keys.Shift)
                            modKey |=  HookGlobalHotkey.ModKeys.Shift;
                        if ((this._cfgCommon.HotkeyModifier & Keys.LWin) == Keys.LWin)
                            modKey |= HookGlobalHotkey.ModKeys.Win;

                        _hookGlobalHotkey.RegisterOriginalHotkey(this._cfgCommon.HotkeyKey, this._cfgCommon.HotkeyValue, modKey);
                    }

                    if (this._cfgCommon.IsUseNotifyGrowl) gh.RegisterGrowl();
                    try
                    {
                        StatusText_TextChanged(null, null);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                // キャンセル時は Twitter クラスの認証情報を画面表示前の状態に戻す
                this.tw.Initialize(oldUser.AccessToken, oldUser.AccessTokenSecret, oldUser.Username, oldUser.UserId);
            }

            Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;

            this.TopMost = this._cfgCommon.AlwaysTop;
            SaveConfigsAll(false);

            if (tw.Username != oldUser.Username)
                await this.doGetFollowersMenu();
        }

        /// <summary>
        /// タブの表示位置を設定する
        /// </summary>
        private void SetTabAlignment()
        {
            var newAlignment = this._cfgCommon.ViewTabBottom ? TabAlignment.Bottom : TabAlignment.Top;
            if (ListTab.Alignment == newAlignment) return;

            // 各タブのリスト上の選択位置などを退避
            var listSelections = this.SaveListViewSelection();

            ListTab.Alignment = newAlignment;

            foreach (TabPage tab in ListTab.TabPages)
            {
                DetailsListView lst = (DetailsListView)tab.Tag;
                TabModel tabInfo = _statuses.Tabs[tab.Text];
                using (ControlTransaction.Update(lst))
                {
                    // 選択位置などを復元
                    this.RestoreListViewSelection(lst, tabInfo, listSelections[tabInfo.TabName]);
                }
            }
        }

        private void ApplyListViewIconSize(MyCommon.IconSizes iconSz)
        {
            // アイコンサイズの再設定
            _iconCol = false;
            switch (iconSz)
            {
                case MyCommon.IconSizes.IconNone:
                    _iconSz = 0;
                    break;
                case MyCommon.IconSizes.Icon16:
                    _iconSz = 16;
                    break;
                case MyCommon.IconSizes.Icon24:
                    _iconSz = 26;
                    break;
                case MyCommon.IconSizes.Icon48:
                    _iconSz = 48;
                    break;
                case MyCommon.IconSizes.Icon48_2:
                    _iconSz = 48;
                    _iconCol = true;
                    break;
            }

            if (_iconSz > 0)
            {
                // ディスプレイの DPI 設定を考慮したサイズを設定する
                _listViewImageList.ImageSize = new Size(
                    1,
                    (int)Math.Ceiling(this._iconSz * this.CurrentScaleFactor.Height));
            }
            else
            {
                _listViewImageList.ImageSize = new Size(1, 1);
            }
        }

        private void ResetColumns(DetailsListView list)
        {
            using (ControlTransaction.Update(list))
            using (ControlTransaction.Layout(list, false))
            {
                // カラムヘッダの再設定
                list.ColumnClick -= MyList_ColumnClick;
                list.DrawColumnHeader -= MyList_DrawColumnHeader;
                list.ColumnReordered -= MyList_ColumnReordered;
                list.ColumnWidthChanged -= MyList_ColumnWidthChanged;

                var cols = list.Columns.Cast<ColumnHeader>().ToList();
                list.Columns.Clear();
                cols.ForEach(col => col.Dispose());
                cols.Clear();

                InitColumns(list, true);

                list.ColumnClick += MyList_ColumnClick;
                list.DrawColumnHeader += MyList_DrawColumnHeader;
                list.ColumnReordered += MyList_ColumnReordered;
                list.ColumnWidthChanged += MyList_ColumnWidthChanged;
            }
        }

        public void AddNewTabForSearch(string searchWord)
        {
            //同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (var tb in _statuses.GetTabsByType<PublicSearchTabModel>())
            {
                if (tb.SearchWords == searchWord && string.IsNullOrEmpty(tb.SearchLang))
                {
                    foreach (TabPage tp in ListTab.TabPages)
                    {
                        if (tb.TabName == tp.Text)
                        {
                            ListTab.SelectedTab = tp;
                            return;
                        }
                    }
                }
            }
            //ユニークなタブ名生成
            string tabName = searchWord;
            for (int i = 0; i <= 100; i++)
            {
                if (_statuses.ContainsTab(tabName))
                    tabName += "_";
                else
                    break;
            }
            //タブ追加
            var tab = new PublicSearchTabModel(tabName);
            _statuses.AddTab(tab);
            AddNewTab(tab, startup: false);
            //追加したタブをアクティブに
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
            //検索条件の設定
            ComboBox cmb = (ComboBox)ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"];
            cmb.Items.Add(searchWord);
            cmb.Text = searchWord;
            SaveConfigsTabs();
            //検索実行
            this.SearchButton_Click(ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"], null);
        }

        private void ShowUserTimeline()
        {
            if (!this.ExistCurrentPost) return;
            AddNewTabForUserTimeline(_curPost.ScreenName);
        }

        private void SearchComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                TabPage relTp = ListTab.SelectedTab;
                RemoveSpecifiedTab(relTp.Text, false);
                SaveConfigsTabs();
                e.SuppressKeyPress = true;
            }
        }

        public void AddNewTabForUserTimeline(string user)
        {
            //同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (var tb in _statuses.GetTabsByType<UserTimelineTabModel>())
            {
                if (tb.ScreenName == user)
                {
                    foreach (TabPage tp in ListTab.TabPages)
                    {
                        if (tb.TabName == tp.Text)
                        {
                            ListTab.SelectedTab = tp;
                            return;
                        }
                    }
                }
            }
            //ユニークなタブ名生成
            string tabName = "user:" + user;
            while (_statuses.ContainsTab(tabName))
            {
                tabName += "_";
            }
            //タブ追加
            var tab = new UserTimelineTabModel(tabName, user);
            this._statuses.AddTab(tab);
            this.AddNewTab(tab, startup: false);
            //追加したタブをアクティブに
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
            SaveConfigsTabs();
            //検索実行
            this.GetUserTimelineAsync(tab);
        }

        public bool AddNewTab(TabModel tab, bool startup)
        {
            //重複チェック
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (tb.Text == tab.TabName) return false;
            }

            //新規タブ名チェック
            if (tab.TabName == Properties.Resources.AddNewTabText1) return false;

            var _tabPage = new TabPage();
            var _listCustom = new DetailsListView();

            int cnt = ListTab.TabPages.Count;

            ///ToDo:Create and set controls follow tabtypes

            using (ControlTransaction.Update(_listCustom))
            using (ControlTransaction.Layout(this.SplitContainer1.Panel1, false))
            using (ControlTransaction.Layout(this.SplitContainer1.Panel2, false))
            using (ControlTransaction.Layout(this.SplitContainer1, false))
            using (ControlTransaction.Layout(this.ListTab, false))
            using (ControlTransaction.Layout(this))
            using (ControlTransaction.Layout(_tabPage, false))
            {
                _tabPage.Controls.Add(_listCustom);

                /// UserTimeline関連
                var userTab = tab as UserTimelineTabModel;
                var listTab = tab as ListTimelineTabModel;
                var searchTab = tab as PublicSearchTabModel;

                if (userTab != null || listTab != null)
                {
                    var label = new Label();
                    label.Dock = DockStyle.Top;
                    label.Name = "labelUser";
                    label.TabIndex = 0;

                    if (listTab != null)
                    {
                        label.Text = listTab.ListInfo.ToString();
                    }
                    else if (userTab != null)
                    {
                        label.Text = userTab.ScreenName + "'s Timeline";
                    }
                    label.TextAlign = ContentAlignment.MiddleLeft;
                    using (ComboBox tmpComboBox = new ComboBox())
                    {
                        label.Height = tmpComboBox.Height;
                    }
                    _tabPage.Controls.Add(label);
                }
                /// 検索関連の準備
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
                        pnl.Enter += SearchControls_Enter;
                        pnl.Leave += SearchControls_Leave;

                        cmb.Text = "";
                        cmb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                        cmb.Dock = DockStyle.Fill;
                        cmb.Name = "comboSearch";
                        cmb.DropDownStyle = ComboBoxStyle.DropDown;
                        cmb.ImeMode = ImeMode.NoControl;
                        cmb.TabStop = false;
                        cmb.TabIndex = 1;
                        cmb.AutoCompleteMode = AutoCompleteMode.None;
                        cmb.KeyDown += SearchComboBox_KeyDown;

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
                        btn.Click += SearchButton_Click;

                        if (!string.IsNullOrEmpty(searchTab.SearchWords))
                        {
                            cmb.Items.Add(searchTab.SearchWords);
                            cmb.Text = searchTab.SearchWords;
                        }

                        cmbLang.Text = searchTab.SearchLang;

                        _tabPage.Controls.Add(pnl);
                    }
                }

                _tabPage.Tag = _listCustom;
                this.ListTab.Controls.Add(_tabPage);

                _tabPage.Location = new Point(4, 4);
                _tabPage.Name = "CTab" + cnt;
                _tabPage.Size = new Size(380, 260);
                _tabPage.TabIndex = 2 + cnt;
                _tabPage.Text = tab.TabName;
                _tabPage.UseVisualStyleBackColor = true;
                _tabPage.AccessibleRole = AccessibleRole.PageTab;

                _listCustom.AccessibleName = Properties.Resources.AddNewTab_ListView_AccessibleName;
                _listCustom.TabIndex = 1;
                _listCustom.AllowColumnReorder = true;
                _listCustom.ContextMenuStrip = this.ContextMenuOperate;
                _listCustom.ColumnHeaderContextMenuStrip = this.ContextMenuColumnHeader;
                _listCustom.Dock = DockStyle.Fill;
                _listCustom.FullRowSelect = true;
                _listCustom.HideSelection = false;
                _listCustom.Location = new Point(0, 0);
                _listCustom.Margin = new Padding(0);
                _listCustom.Name = "CList" + Environment.TickCount;
                _listCustom.ShowItemToolTips = true;
                _listCustom.Size = new Size(380, 260);
                _listCustom.UseCompatibleStateImageBehavior = false;
                _listCustom.View = View.Details;
                _listCustom.OwnerDraw = true;
                _listCustom.VirtualMode = true;
                _listCustom.Font = _fntReaded;
                _listCustom.BackColor = _clListBackcolor;

                _listCustom.GridLines = this._cfgCommon.ShowGrid;
                _listCustom.AllowDrop = true;

                _listCustom.SmallImageList = _listViewImageList;

                InitColumns(_listCustom, startup);

                _listCustom.SelectedIndexChanged += MyList_SelectedIndexChanged;
                _listCustom.MouseDoubleClick += MyList_MouseDoubleClick;
                _listCustom.ColumnClick += MyList_ColumnClick;
                _listCustom.DrawColumnHeader += MyList_DrawColumnHeader;
                _listCustom.DragDrop += TweenMain_DragDrop;
                _listCustom.DragEnter += TweenMain_DragEnter;
                _listCustom.DragOver += TweenMain_DragOver;
                _listCustom.DrawItem += MyList_DrawItem;
                _listCustom.MouseClick += MyList_MouseClick;
                _listCustom.ColumnReordered += MyList_ColumnReordered;
                _listCustom.ColumnWidthChanged += MyList_ColumnWidthChanged;
                _listCustom.CacheVirtualItems += MyList_CacheVirtualItems;
                _listCustom.RetrieveVirtualItem += MyList_RetrieveVirtualItem;
                _listCustom.DrawSubItem += MyList_DrawSubItem;
                _listCustom.HScrolled += MyList_HScrolled;
            }

            return true;
        }

        public bool RemoveSpecifiedTab(string TabName, bool confirm)
        {
            var tabInfo = _statuses.GetTabByName(TabName);
            if (tabInfo.IsDefaultTabType || tabInfo.Protected) return false;

            if (confirm)
            {
                string tmp = string.Format(Properties.Resources.RemoveSpecifiedTabText1, Environment.NewLine);
                if (MessageBox.Show(tmp, TabName + " " + Properties.Resources.RemoveSpecifiedTabText2,
                                 MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                {
                    return false;
                }
            }

            var _tabPage = ListTab.TabPages.Cast<TabPage>().FirstOrDefault(tp => tp.Text == TabName);
            if (_tabPage == null) return false;

            SetListProperty();   //他のタブに列幅等を反映

            //オブジェクトインスタンスの削除
            DetailsListView _listCustom = (DetailsListView)_tabPage.Tag;
            _tabPage.Tag = null;

            using (ControlTransaction.Layout(this.SplitContainer1.Panel1, false))
            using (ControlTransaction.Layout(this.SplitContainer1.Panel2, false))
            using (ControlTransaction.Layout(this.SplitContainer1, false))
            using (ControlTransaction.Layout(this.ListTab, false))
            using (ControlTransaction.Layout(this))
            using (ControlTransaction.Layout(_tabPage, false))
            {
                if (this.ListTab.SelectedTab == _tabPage)
                {
                    this.ListTab.SelectTab((this._beforeSelectedTab != null && this.ListTab.TabPages.Contains(this._beforeSelectedTab)) ? this._beforeSelectedTab : this.ListTab.TabPages[0]);
                    this._beforeSelectedTab = null;
                }
                this.ListTab.Controls.Remove(_tabPage);

                // 後付けのコントロールを破棄
                if (tabInfo.TabType == MyCommon.TabUsageType.UserTimeline || tabInfo.TabType == MyCommon.TabUsageType.Lists)
                {
                    using (Control label = _tabPage.Controls["labelUser"])
                    {
                        _tabPage.Controls.Remove(label);
                    }
                }
                else if (tabInfo.TabType == MyCommon.TabUsageType.PublicSearch)
                {
                    using (Control pnl = _tabPage.Controls["panelSearch"])
                    {
                        pnl.Enter -= SearchControls_Enter;
                        pnl.Leave -= SearchControls_Leave;
                        _tabPage.Controls.Remove(pnl);

                        foreach (Control ctrl in pnl.Controls)
                        {
                            if (ctrl.Name == "buttonSearch")
                            {
                                ctrl.Click -= SearchButton_Click;
                            }
                            else if (ctrl.Name == "comboSearch")
                            {
                                ctrl.KeyDown -= SearchComboBox_KeyDown;
                            }
                            pnl.Controls.Remove(ctrl);
                            ctrl.Dispose();
                        }
                    }
                }

                _tabPage.Controls.Remove(_listCustom);

                _listCustom.SelectedIndexChanged -= MyList_SelectedIndexChanged;
                _listCustom.MouseDoubleClick -= MyList_MouseDoubleClick;
                _listCustom.ColumnClick -= MyList_ColumnClick;
                _listCustom.DrawColumnHeader -= MyList_DrawColumnHeader;
                _listCustom.DragDrop -= TweenMain_DragDrop;
                _listCustom.DragEnter -= TweenMain_DragEnter;
                _listCustom.DragOver -= TweenMain_DragOver;
                _listCustom.DrawItem -= MyList_DrawItem;
                _listCustom.MouseClick -= MyList_MouseClick;
                _listCustom.ColumnReordered -= MyList_ColumnReordered;
                _listCustom.ColumnWidthChanged -= MyList_ColumnWidthChanged;
                _listCustom.CacheVirtualItems -= MyList_CacheVirtualItems;
                _listCustom.RetrieveVirtualItem -= MyList_RetrieveVirtualItem;
                _listCustom.DrawSubItem -= MyList_DrawSubItem;
                _listCustom.HScrolled -= MyList_HScrolled;

                var cols = _listCustom.Columns.Cast<ColumnHeader>().ToList<ColumnHeader>();
                _listCustom.Columns.Clear();
                cols.ForEach(col => col.Dispose());
                cols.Clear();

                _listCustom.ContextMenuStrip = null;
                _listCustom.ColumnHeaderContextMenuStrip = null;
                _listCustom.Font = null;

                _listCustom.SmallImageList = null;
                _listCustom.ListViewItemSorter = null;

                //キャッシュのクリア
                if (_curTab.Equals(_tabPage))
                {
                    _curTab = null;
                    _curItemIndex = -1;
                    _curList = null;
                    _curPost = null;
                }
                this.PurgeListViewItemCache();
            }

            _tabPage.Dispose();
            _listCustom.Dispose();
            _statuses.RemoveTab(TabName);

            foreach (TabPage tp in ListTab.TabPages)
            {
                DetailsListView lst = (DetailsListView)tp.Tag;
                var count = _statuses.Tabs[tp.Text].AllCount;
                lst.VirtualListSize = count;
            }

            return true;
        }

        private void ListTab_Deselected(object sender, TabControlEventArgs e)
        {
            this.PurgeListViewItemCache();
            _beforeSelectedTab = e.TabPage;
        }

        private void ListTab_MouseMove(object sender, MouseEventArgs e)
        {
            //タブのD&D

            if (!this._cfgCommon.TabMouseLock && e.Button == MouseButtons.Left && _tabDrag)
            {
                string tn = "";
                Rectangle dragEnableRectangle = new Rectangle(_tabMouseDownPoint.X - (SystemInformation.DragSize.Width / 2), _tabMouseDownPoint.Y - (SystemInformation.DragSize.Height / 2), SystemInformation.DragSize.Width, SystemInformation.DragSize.Height);
                if (!dragEnableRectangle.Contains(e.Location))
                {
                    //タブが多段の場合にはMouseDownの前の段階で選択されたタブの段が変わっているので、このタイミングでカーソルの位置からタブを判定出来ない。
                    tn = ListTab.SelectedTab.Text;
                }

                if (string.IsNullOrEmpty(tn)) return;

                foreach (TabPage tb in ListTab.TabPages)
                {
                    if (tb.Text == tn)
                    {
                        ListTab.DoDragDrop(tb, DragDropEffects.All);
                        break;
                    }
                }
            }
            else
            {
                _tabDrag = false;
            }

            Point cpos = new Point(e.X, e.Y);
            for (int i = 0; i < ListTab.TabPages.Count; i++)
            {
                Rectangle rect = ListTab.GetTabRect(i);
                if (rect.Left <= cpos.X & cpos.X <= rect.Right &
                   rect.Top <= cpos.Y & cpos.Y <= rect.Bottom)
                {
                    _rclickTabName = ListTab.TabPages[i].Text;
                    break;
                }
            }
        }

        private async void ListTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            //_curList.Refresh();
            SetMainWindowTitle();
            SetStatusLabelUrl();
            SetApiStatusLabel();
            if (ListTab.Focused || ((Control)ListTab.SelectedTab.Tag).Focused) this.Tag = ListTab.Tag;
            TabMenuControl(ListTab.SelectedTab.Text);
            this.PushSelectPostChain();
            await DispSelectedPost();
        }

        private void SetListProperty()
        {
            //削除などで見つからない場合は処理せず
            if (_curList == null) return;
            if (!_isColumnChanged) return;

            int[] dispOrder = new int[_curList.Columns.Count];
            for (int i = 0; i < _curList.Columns.Count; i++)
            {
                for (int j = 0; j < _curList.Columns.Count; j++)
                {
                    if (_curList.Columns[j].DisplayIndex == i)
                    {
                        dispOrder[i] = j;
                        break;
                    }
                }
            }

            //列幅、列並びを他のタブに設定
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (!tb.Equals(_curTab))
                {
                    if (tb.Tag != null && tb.Controls.Count > 0)
                    {
                        DetailsListView lst = (DetailsListView)tb.Tag;
                        for (int i = 0; i < lst.Columns.Count; i++)
                        {
                            lst.Columns[dispOrder[i]].DisplayIndex = i;
                            lst.Columns[i].Width = _curList.Columns[i].Width;
                        }
                    }
                }
            }

            _isColumnChanged = false;
        }

        private void StatusText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '@')
            {
                if (!this._cfgCommon.UseAtIdSupplement) return;
                //@マーク
                int cnt = AtIdSupl.ItemCount;
                ShowSuplDialog(StatusText, AtIdSupl);
                if (cnt != AtIdSupl.ItemCount) ModifySettingAtId = true;
                e.Handled = true;
            }
            else if (e.KeyChar == '#')
            {
                if (!this._cfgCommon.UseHashSupplement) return;
                ShowSuplDialog(StatusText, HashSupl);
                e.Handled = true;
            }
        }

        public void ShowSuplDialog(TextBox owner, AtIdSupplement dialog)
        {
            ShowSuplDialog(owner, dialog, 0, "");
        }

        public void ShowSuplDialog(TextBox owner, AtIdSupplement dialog, int offset)
        {
            ShowSuplDialog(owner, dialog, offset, "");
        }

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
            this.TopMost = this._cfgCommon.AlwaysTop;
            int selStart = owner.SelectionStart;
            string fHalf = "";
            string eHalf = "";
            if (dialog.DialogResult == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.inputText))
                {
                    if (selStart > 0)
                    {
                        fHalf = owner.Text.Substring(0, selStart - offset);
                    }
                    if (selStart < owner.Text.Length)
                    {
                        eHalf = owner.Text.Substring(selStart);
                    }
                    owner.Text = fHalf + dialog.inputText + eHalf;
                    owner.SelectionStart = selStart + dialog.inputText.Length;
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
            //スペースキーで未読ジャンプ
            if (!e.Alt && !e.Control && !e.Shift)
            {
                if (e.KeyCode == Keys.Space || e.KeyCode == Keys.ProcessKey)
                {
                    bool isSpace = false;
                    foreach (char c in StatusText.Text)
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
                        StatusText.Text = "";
                        JumpUnreadMenuItem_Click(null, null);
                    }
                }
            }
            this.StatusText_TextChanged(null, null);
        }

        private void StatusText_TextChanged(object sender, EventArgs e)
        {
            //文字数カウント
            int pLen = this.GetRestStatusCount(this.FormatStatusText(this.StatusText.Text));
            lblLen.Text = pLen.ToString();
            if (pLen < 0)
            {
                StatusText.ForeColor = Color.Red;
            }
            else
            {
                StatusText.ForeColor = _clInputFont;
            }
            if (string.IsNullOrEmpty(StatusText.Text))
            {
                this.inReplyTo = null;
            }
        }

        /// <summary>
        /// ツイート投稿前のフッター付与などの前処理を行います
        /// </summary>
        private string FormatStatusText(string statusText)
        {
            statusText = statusText.Replace("\r\n", "\n");

            if (this.ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                statusText = Regex.Replace(statusText, @"https?:\/\/[-_.!~*'()a-zA-Z0-9;\/?:\@&=+\$,%#^]+", "$& ");
            }

            if (this.IdeographicSpaceToSpaceToolStripMenuItem.Checked)
            {
                // 文中の全角スペースを半角スペース1個にする
                statusText = statusText.Replace("　", " ");
            }

            // DM の場合はこれ以降の処理を行わない
            if (statusText.StartsWith("D ", StringComparison.OrdinalIgnoreCase))
                return statusText;

            bool disableFooter;
            if (this._cfgCommon.PostShiftEnter)
            {
                disableFooter = MyCommon.IsKeyDown(Keys.Control);
            }
            else
            {
                if (this.StatusText.Multiline && !this._cfgCommon.PostCtrlEnter)
                    disableFooter = MyCommon.IsKeyDown(Keys.Control);
                else
                    disableFooter = MyCommon.IsKeyDown(Keys.Shift);
            }

            if (statusText.Contains("RT @"))
                disableFooter = true;

            var header = "";
            var footer = "";

            var hashtag = this.HashMgr.UseHash;
            if (!string.IsNullOrEmpty(hashtag) && !(this.HashMgr.IsNotAddToAtReply && this.inReplyTo != null))
            {
                if (HashMgr.IsHead)
                    header = HashMgr.UseHash + " ";
                else
                    footer = " " + HashMgr.UseHash;
            }

            if (!disableFooter)
            {
                if (this._cfgLocal.UseRecommendStatus)
                {
                    // 推奨ステータスを使用する
                    footer += this.recommendedStatusFooter;
                }
                else if (!string.IsNullOrEmpty(this._cfgLocal.StatusText))
                {
                    // テキストボックスに入力されている文字列を使用する
                    footer += " " + this._cfgLocal.StatusText.Trim();
                }
            }

            statusText = header + statusText + footer;

            if (this.ToolStripMenuItemPreventSmsCommand.Checked)
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
            //文字数カウント
            var remainCount = this.tw.GetTextLengthRemain(statusText);

            if (this.ImageSelector.Visible && !string.IsNullOrEmpty(this.ImageSelector.ServiceName))
            {
                remainCount -= this.tw.Configuration.CharactersReservedPerMedia;
            }

            return remainCount;
        }

        private void MyList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            if (sender != this._curList)
                return;

            var listCache = this._listItemCache;
            if (listCache != null && listCache.IsSupersetOf(e.StartIndex, e.EndIndex))
            {
                // If the newly requested cache is a subset of the old cache,
                // no need to rebuild everything, so do nothing.
                return;
            }

            // Now we need to rebuild the cache.
            this.CreateCache(e.StartIndex, e.EndIndex);
        }

        private void MyList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var listCache = this._listItemCache;
            if (listCache != null && listCache.TargetList == sender)
            {
                ListViewItem item;
                PostClass cacheItemPost;
                if (listCache.TryGetValue(e.ItemIndex, out item, out cacheItemPost))
                {
                    e.Item = item;
                    return;
                }
            }

            // A cache miss, so create a new ListViewItem and pass it back.
            TabPage tb = (TabPage)((DetailsListView)sender).Parent;
            try
            {
                e.Item = this.CreateItem(tb, _statuses.Tabs[tb.Text][e.ItemIndex], e.ItemIndex);
            }
            catch (Exception)
            {
                // 不正な要求に対する間に合わせの応答
                string[] sitem = {"", "", "", "", "", "", "", ""};
                e.Item = new ImageListViewItem(sitem);
            }
        }

        private void CreateCache(int startIndex, int endIndex)
        {
            var tabInfo = this._statuses.Tabs[this._curTab.Text];

            if (tabInfo.AllCount == 0)
                return;

            // キャッシュ要求（要求範囲±30を作成）
            startIndex = Math.Max(startIndex - 30, 0);
            endIndex = Math.Min(endIndex + 30, tabInfo.AllCount - 1);

            var cacheLength = endIndex - startIndex + 1;

            var posts = tabInfo[startIndex, endIndex]; //配列で取得
            var listItems = Enumerable.Range(0, cacheLength)
                .Select(x => this.CreateItem(this._curTab, posts[x], startIndex + x))
                .ToArray();

            var listCache = new ListViewItemCache
            {
                TargetList = this._curList,
                StartIndex = startIndex,
                EndIndex = endIndex,
                Post = posts,
                ListItem = listItems,
            };

            Interlocked.Exchange(ref this._listItemCache, listCache);
        }

        /// <summary>
        /// DetailsListView のための ListViewItem のキャッシュを消去する
        /// </summary>
        private void PurgeListViewItemCache()
        {
            Interlocked.Exchange(ref this._listItemCache, null);
        }

        private ListViewItem CreateItem(TabPage Tab, PostClass Post, int Index)
        {
            StringBuilder mk = new StringBuilder();
            //if (Post.IsDeleted) mk.Append("×");
            //if (Post.IsMark) mk.Append("♪");
            //if (Post.IsProtect) mk.Append("Ю");
            //if (Post.InReplyToStatusId != null) mk.Append("⇒");
            if (Post.FavoritedCount > 0) mk.Append("+" + Post.FavoritedCount);
            ImageListViewItem itm;
            if (Post.RetweetedId == null)
            {
                string[] sitem= {"",
                                 Post.Nickname,
                                 Post.IsDeleted ? "(DELETED)" : Post.TextSingleLine,
                                 Post.CreatedAt.ToString(this._cfgCommon.DateTimeFormat),
                                 Post.ScreenName,
                                 "",
                                 mk.ToString(),
                                 Post.Source};
                itm = new ImageListViewItem(sitem, this.IconCache, Post.ImageUrl);
            }
            else
            {
                string[] sitem = {"",
                                  Post.Nickname,
                                  Post.IsDeleted ? "(DELETED)" : Post.TextSingleLine,
                                  Post.CreatedAt.ToString(this._cfgCommon.DateTimeFormat),
                                  Post.ScreenName + Environment.NewLine + "(RT:" + Post.RetweetedBy + ")",
                                  "",
                                  mk.ToString(),
                                  Post.Source};
                itm = new ImageListViewItem(sitem, this.IconCache, Post.ImageUrl);
            }
            itm.StateIndex = Post.StateIndex;

            bool read = Post.IsRead;
            //未読管理していなかったら既読として扱う
            if (!_statuses.Tabs[Tab.Text].UnreadManage || !this._cfgCommon.UnreadManage) read = true;
            ChangeItemStyleRead(read, itm, Post, null);
            if (Tab.Equals(_curTab)) ColorizeList(itm, Index);
            return itm;
        }

        /// <summary>
        /// 全てのタブの振り分けルールを反映し直します
        /// </summary>
        private void ApplyPostFilters()
        {
            using (ControlTransaction.Cursor(this, Cursors.WaitCursor))
            {
                this.PurgeListViewItemCache();
                this._curPost = null;
                this._curItemIndex = -1;
                this._statuses.FilterAll();

                foreach (TabPage tabPage in this.ListTab.TabPages)
                {
                    var tab = this._statuses.Tabs[tabPage.Text];

                    var listview = (DetailsListView)tabPage.Tag;
                    using (ControlTransaction.Update(listview))
                    {
                        listview.VirtualListSize = tab.AllCount;
                    }

                    if (this._cfgCommon.TabIconDisp)
                    {
                        if (tab.UnreadCount > 0)
                            tabPage.ImageIndex = 0;
                        else
                            tabPage.ImageIndex = -1;
                    }
                }

                if (!this._cfgCommon.TabIconDisp)
                    this.ListTab.Refresh();

                SetMainWindowTitle();
                SetStatusLabelUrl();
            }
        }

        private void MyList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void MyList_HScrolled(object sender, EventArgs e)
        {
            DetailsListView listView = (DetailsListView)sender;
            listView.Refresh();
        }

        private void MyList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.State == 0) return;
            e.DrawDefault = false;

            SolidBrush brs2 = null;
            if (!e.Item.Selected)     //e.ItemStateでうまく判定できない？？？
            {
                if (e.Item.BackColor == _clSelf)
                    brs2 = _brsBackColorMine;
                else if (e.Item.BackColor == _clAtSelf)
                    brs2 = _brsBackColorAt;
                else if (e.Item.BackColor == _clTarget)
                    brs2 = _brsBackColorYou;
                else if (e.Item.BackColor == _clAtTarget)
                    brs2 = _brsBackColorAtYou;
                else if (e.Item.BackColor == _clAtFromTarget)
                    brs2 = _brsBackColorAtFromTarget;
                else if (e.Item.BackColor == _clAtTo)
                    brs2 = _brsBackColorAtTo;
                else
                    brs2 = _brsBackColorNone;
            }
            else
            {
                //選択中の行
                if (((Control)sender).Focused)
                    brs2 = _brsHighLight;
                else
                    brs2 = _brsDeactiveSelection;
            }
            e.Graphics.FillRectangle(brs2, e.Bounds);
            e.DrawFocusRectangle();
            this.DrawListViewItemIcon(e);
        }

        private void MyList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ItemState == 0) return;

            if (e.ColumnIndex > 0)
            {
                //アイコン以外の列
                RectangleF rct = e.Bounds;
                rct.Width = e.Header.Width;
                int fontHeight = e.Item.Font.Height;
                if (_iconCol)
                {
                    rct.Y += fontHeight;
                    rct.Height -= fontHeight;
                }

                int heightDiff;
                int drawLineCount = Math.Max(1, Math.DivRem((int)rct.Height, fontHeight, out heightDiff));

                //if (heightDiff > fontHeight * 0.7)
                //{
                //    rct.Height += fontHeight;
                //    drawLineCount += 1;
                //}

                //フォントの高さの半分を足してるのは保険。無くてもいいかも。
                if (!_iconCol && drawLineCount <= 1)
                {
                    //rct.Inflate(0, heightDiff / -2);
                    //rct.Height += fontHeight / 2;
                }
                else if (heightDiff < fontHeight * 0.7)
                {
                    //最終行が70%以上欠けていたら、最終行は表示しない
                    //rct.Height = (float)((fontHeight * drawLineCount) + (fontHeight / 2));
                    rct.Height = (fontHeight * drawLineCount) - 1;
                }
                else
                {
                    drawLineCount += 1;
                }

                //if (!_iconCol && drawLineCount > 1)
                //{
                //    rct.Y += fontHeight * 0.2;
                //    if (heightDiff >= fontHeight * 0.8) rct.Height -= fontHeight * 0.2;
                //}

                if (rct.Width > 0)
                {
                    Color color = (!e.Item.Selected) ? e.Item.ForeColor :   //選択されていない行
                        (((Control)sender).Focused) ? _clHighLight :        //選択中の行
                        _clUnread;

                    if (_iconCol)
                    {
                        Rectangle rctB = e.Bounds;
                        rctB.Width = e.Header.Width;
                        rctB.Height = fontHeight;

                        using (Font fnt = new Font(e.Item.Font, FontStyle.Bold))
                        {
                            TextRenderer.DrawText(e.Graphics,
                                                    e.Item.SubItems[2].Text,
                                                    e.Item.Font,
                                                    Rectangle.Round(rct),
                                                    color,
                                                    TextFormatFlags.WordBreak |
                                                    TextFormatFlags.EndEllipsis |
                                                    TextFormatFlags.GlyphOverhangPadding |
                                                    TextFormatFlags.NoPrefix);
                            TextRenderer.DrawText(e.Graphics,
                                                    e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]",
                                                    fnt,
                                                    rctB,
                                                    color,
                                                    TextFormatFlags.SingleLine |
                                                    TextFormatFlags.EndEllipsis |
                                                    TextFormatFlags.GlyphOverhangPadding |
                                                    TextFormatFlags.NoPrefix);
                        }
                    }
                    else if (drawLineCount == 1)
                    {
                        TextRenderer.DrawText(e.Graphics,
                                                e.SubItem.Text,
                                                e.Item.Font,
                                                Rectangle.Round(rct),
                                                color,
                                                TextFormatFlags.SingleLine |
                                                TextFormatFlags.EndEllipsis |
                                                TextFormatFlags.GlyphOverhangPadding |
                                                TextFormatFlags.NoPrefix |
                                                TextFormatFlags.VerticalCenter);
                    }
                    else
                    {
                        TextRenderer.DrawText(e.Graphics,
                                                e.SubItem.Text,
                                                e.Item.Font,
                                                Rectangle.Round(rct),
                                                color,
                                                TextFormatFlags.WordBreak |
                                                TextFormatFlags.EndEllipsis |
                                                TextFormatFlags.GlyphOverhangPadding |
                                                TextFormatFlags.NoPrefix);
                    }
                    //if (e.ColumnIndex == 6) this.DrawListViewItemStateIcon(e, rct);
                }
            }
        }

        private void DrawListViewItemIcon(DrawListViewItemEventArgs e)
        {
            if (_iconSz == 0) return;

            ImageListViewItem item = (ImageListViewItem)e.Item;

            //e.Bounds.Leftが常に0を指すから自前で計算
            Rectangle itemRect = item.Bounds;
            var col0 = e.Item.ListView.Columns[0];
            itemRect.Width = col0.Width;

            if (col0.DisplayIndex > 0)
            {
                foreach (ColumnHeader clm in e.Item.ListView.Columns)
                {
                    if (clm.DisplayIndex < col0.DisplayIndex)
                        itemRect.X += clm.Width;
                }
            }

            // ディスプレイの DPI 設定を考慮したアイコンサイズ
            var realIconSize = new SizeF(this._iconSz * this.CurrentScaleFactor.Width, this._iconSz * this.CurrentScaleFactor.Height).ToSize();
            var realStateSize = new SizeF(16 * this.CurrentScaleFactor.Width, 16 * this.CurrentScaleFactor.Height).ToSize();

            Rectangle iconRect;
            var img = item.Image;
            if (img != null)
            {
                iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, realIconSize), itemRect);
                iconRect.Offset(0, Math.Max(0, (itemRect.Height - realIconSize.Height) / 2));

                if (iconRect.Width > 0)
                {
                    e.Graphics.FillRectangle(Brushes.White, iconRect);
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    try
                    {
                        e.Graphics.DrawImage(img.Image, iconRect);
                    }
                    catch (ArgumentException)
                    {
                        item.RefreshImageAsync();
                    }
                }
            }
            else
            {
                iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, new Size(1, 1)), itemRect);
                //iconRect.Offset(0, Math.Max(0, (itemRect.Height - realIconSize.Height) / 2));

                item.GetImageAsync();
            }

            if (item.StateIndex > -1)
            {
                Rectangle stateRect = Rectangle.Intersect(new Rectangle(new Point(iconRect.X + realIconSize.Width + 2, iconRect.Y), realStateSize), itemRect);
                if (stateRect.Width > 0)
                {
                    //e.Graphics.FillRectangle(Brushes.White, stateRect);
                    //e.Graphics.InterpolationMode = Drawing2D.InterpolationMode.High;
                    e.Graphics.DrawImage(this.PostStateImageList.Images[item.StateIndex], stateRect);
                }
            }
        }

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

        //private void DrawListViewItemStateIcon(DrawListViewSubItemEventArgs e, RectangleF rct)
        //{
        //    ImageListViewItem item = (ImageListViewItem)e.Item;
        //    if (item.StateImageIndex > -1)
        //    {
        //        ////e.Bounds.Leftが常に0を指すから自前で計算
        //        //Rectangle itemRect = item.Bounds;
        //        //itemRect.Width = e.Item.ListView.Columns[4].Width;

        //        //foreach (ColumnHeader clm in e.Item.ListView.Columns)
        //        //{
        //        //    if (clm.DisplayIndex < e.Item.ListView.Columns[4].DisplayIndex)
        //        //    {
        //        //        itemRect.X += clm.Width;
        //        //    }
        //        //}

        //        //Rectangle iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, new Size(_iconSz, _iconSz)), itemRect);
        //        //iconRect.Offset(0, Math.Max(0, (itemRect.Height - _iconSz) / 2));

        //        if (rct.Width > 0)
        //        {
        //            RectangleF stateRect = RectangleF.Intersect(rct, new RectangleF(rct.Location, new Size(18, 16)));
        //            //e.Graphics.FillRectangle(Brushes.White, rct);
        //            //e.Graphics.InterpolationMode = Drawing2D.InterpolationMode.High;
        //            e.Graphics.DrawImage(this.PostStateImageList.Images(item.StateImageIndex), stateRect);
        //        }
        //    }
        //}

        internal void DoTabSearch(string searchWord, bool caseSensitive, bool useRegex, SEARCHTYPE searchType)
        {
            var tab = this._statuses.Tabs[this._curTab.Text];

            if (tab.AllCount == 0)
            {
                MessageBox.Show(Properties.Resources.DoTabSearchText2, Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedIndex = this._curList.SelectedIndices.Count != 0 ? this._curList.SelectedIndices[0] : -1;

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

            this.SelectListItem(this._curList, foundIndex);
            this._curList.EnsureVisible(foundIndex);
        }

        private void MenuItemSubSearch_Click(object sender, EventArgs e)
        {
            // 検索メニュー
            this.ShowSearchDialog();
        }

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
                this.TopMost = this._cfgCommon.AlwaysTop;
                return;
            }
            this.TopMost = this._cfgCommon.AlwaysTop;

            var searchOptions = this.SearchDialog.ResultOptions;
            if (searchOptions.Type == SearchWordDialog.SearchType.Timeline)
            {
                if (searchOptions.NewTab)
                {
                    var tabName = Properties.Resources.SearchResults_TabName;

                    try
                    {
                        tabName = this._statuses.MakeTabName(tabName);
                    }
                    catch (TabException ex)
                    {
                        MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    var resultTab = new LocalSearchTabModel(tabName);
                    this.AddNewTab(resultTab, startup: false);
                    this._statuses.AddTab(resultTab);

                    var targetTab = this._statuses.Tabs[this._curTab.Text];

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

                    this._statuses.DistributePosts();
                    this.RefreshTimeline();

                    var tabPage = this.ListTab.TabPages.Cast<TabPage>()
                        .First(x => x.Text == tabName);

                    this.ListTab.SelectedTab = tabPage;
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
            using (TweenAboutBox about = new TweenAboutBox())
            {
                about.ShowDialog(this);
            }
            this.TopMost = this._cfgCommon.AlwaysTop;
        }

        private void JumpUnreadMenuItem_Click(object sender, EventArgs e)
        {
            int bgnIdx = ListTab.TabPages.IndexOf(_curTab);
            int idx = -1;
            DetailsListView lst = null;

            if (ImageSelector.Enabled)
                return;

            //現在タブから最終タブまで探索
            for (int i = bgnIdx; i < ListTab.TabPages.Count; i++)
            {
                //未読Index取得
                idx = _statuses.Tabs[ListTab.TabPages[i].Text].NextUnreadIndex;
                if (idx > -1)
                {
                    ListTab.SelectedIndex = i;
                    lst = (DetailsListView)ListTab.TabPages[i].Tag;
                    //_curTab = ListTab.TabPages[i];
                    break;
                }
            }

            //未読みつからず＆現在タブが先頭ではなかったら、先頭タブから現在タブの手前まで探索
            if (idx == -1 && bgnIdx > 0)
            {
                for (int i = 0; i < bgnIdx; i++)
                {
                    idx = _statuses.Tabs[ListTab.TabPages[i].Text].NextUnreadIndex;
                    if (idx > -1)
                    {
                        ListTab.SelectedIndex = i;
                        lst = (DetailsListView)ListTab.TabPages[i].Tag;
                        //_curTab = ListTab.TabPages[i];
                        break;
                    }
                }
            }

            //全部調べたが未読見つからず→先頭タブの最新発言へ
            if (idx == -1)
            {
                ListTab.SelectedIndex = 0;
                lst = (DetailsListView)ListTab.TabPages[0].Tag;
                //_curTab = ListTab.TabPages[0];
                if (_statuses.SortOrder == SortOrder.Ascending)
                    idx = lst.VirtualListSize - 1;
                else
                    idx = 0;
            }

            if (lst.VirtualListSize > 0 && idx > -1 && lst.VirtualListSize > idx)
            {
                SelectListItem(lst, idx);
                if (_statuses.SortMode == ComparerMode.Id)
                {
                    if (_statuses.SortOrder == SortOrder.Ascending && lst.Items[idx].Position.Y > lst.ClientSize.Height - _iconSz - 10 ||
                       _statuses.SortOrder == SortOrder.Descending && lst.Items[idx].Position.Y < _iconSz + 10)
                    {
                        MoveTop();
                    }
                    else
                    {
                        lst.EnsureVisible(idx);
                    }
                }
                else
                {
                    lst.EnsureVisible(idx);
                }
            }
            lst.Focus();
        }

        private async void StatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            if (_curList.SelectedIndices.Count > 0 && _statuses.Tabs[_curTab.Text].TabType != MyCommon.TabUsageType.DirectMessage)
            {
                var post = _statuses.Tabs[_curTab.Text][_curList.SelectedIndices[0]];
                await this.OpenUriInBrowserAsync(MyCommon.GetStatusUrl(post));
            }
        }

        private async void FavorareMenuItem_Click(object sender, EventArgs e)
        {
            if (_curList.SelectedIndices.Count > 0)
            {
                PostClass post = _statuses.Tabs[_curTab.Text][_curList.SelectedIndices[0]];
                await this.OpenUriInBrowserAsync(Properties.Resources.FavstarUrl + "users/" + post.ScreenName + "/recent");
            }
        }

        private async void VerUpMenuItem_Click(object sender, EventArgs e)
        {
            await this.CheckNewVersion(false);
        }

        private void RunTweenUp()
        {
            ProcessStartInfo pinfo = new ProcessStartInfo();
            pinfo.UseShellExecute = true;
            pinfo.WorkingDirectory = MyCommon.settingPath;
            pinfo.FileName = Path.Combine(MyCommon.settingPath, "TweenUp3.exe");
            pinfo.Arguments = "\"" + Application.StartupPath + "\"";
            try
            {
                Process.Start(pinfo);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to execute TweenUp3.exe.");
            }
        }

        public class VersionInfo
        {
            public Version Version { get; set; }
            public Uri DownloadUri { get; set; }
            public string ReleaseNote { get; set; }
        }

        /// <summary>
        /// OpenTween の最新バージョンの情報を取得します
        /// </summary>
        public async Task<VersionInfo> GetVersionInfoAsync()
        {
            var versionInfoUrl = new Uri(ApplicationSettings.VersionInfoUrl + "?" +
                DateTime.Now.ToString("yyMMddHHmmss") + Environment.TickCount);

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
                        var msgtext = string.Format(Properties.Resources.CheckNewVersionText7,
                            MyCommon.GetReadableVersion(), MyCommon.GetReadableVersion(versionInfo.Version));
                        msgtext = MyCommon.ReplaceAppName(msgtext);

                        MessageBox.Show(msgtext,
                            MyCommon.ReplaceAppName(Properties.Resources.CheckNewVersionText2),
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return;
                }

                using (var dialog = new UpdateDialog())
                {
                    dialog.SummaryText = string.Format(Properties.Resources.CheckNewVersionText3,
                        MyCommon.GetReadableVersion(versionInfo.Version));
                    dialog.DetailsText = versionInfo.ReleaseNote;

                    if (dialog.ShowDialog(this) == DialogResult.Yes)
                    {
                        await this.OpenUriInBrowserAsync(versionInfo.DownloadUri.OriginalString);
                    }
                }
            }
            catch (Exception)
            {
                this.StatusLabel.Text = Properties.Resources.CheckNewVersionText9;
                if (!startup)
                {
                    MessageBox.Show(Properties.Resources.CheckNewVersionText10,
                        MyCommon.ReplaceAppName(Properties.Resources.CheckNewVersionText2),
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                }
            }
        }

        private async Task Colorize()
        {
            _colorize = false;
            await this.DispSelectedPost();
            //件数関連の場合、タイトル即時書き換え
            if (this._cfgCommon.DispLatestPost != MyCommon.DispTitleEnum.None &&
               this._cfgCommon.DispLatestPost != MyCommon.DispTitleEnum.Post &&
               this._cfgCommon.DispLatestPost != MyCommon.DispTitleEnum.Ver &&
               this._cfgCommon.DispLatestPost != MyCommon.DispTitleEnum.OwnStatus)
            {
                SetMainWindowTitle();
            }
            if (!StatusLabelUrl.Text.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                SetStatusLabelUrl();
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (_statuses.Tabs[tb.Text].UnreadCount == 0)
                {
                    if (this._cfgCommon.TabIconDisp)
                    {
                        if (tb.ImageIndex == 0) tb.ImageIndex = -1;
                    }
                }
            }
            if (!this._cfgCommon.TabIconDisp) ListTab.Refresh();
        }

        public string createDetailHtml(string orgdata)
        {
            if (this._cfgLocal.UseTwemoji)
                orgdata = EmojiFormatter.ReplaceEmojiToImg(orgdata);

            return detailHtmlFormatHeader + orgdata + detailHtmlFormatFooter;
        }

        private Task DispSelectedPost()
        {
            return this.DispSelectedPost(false);
        }

        private PostClass displayPost = new PostClass();

        /// <summary>
        /// サムネイル表示に使用する CancellationToken の生成元
        /// </summary>
        private CancellationTokenSource thumbnailTokenSource = null;

        private async Task DispSelectedPost(bool forceupdate)
        {
            if (_curList.SelectedIndices.Count == 0 || _curPost == null)
                return;

            var oldDisplayPost = this.displayPost;
            this.displayPost = this._curPost;

            if (!forceupdate && this._curPost.Equals(oldDisplayPost))
                return;

            var loadTasks = new List<Task>
            {
                this.tweetDetailsView.ShowPostDetails(this._curPost),
            };

            this.SplitContainer3.Panel2Collapsed = true;

            if (this._cfgCommon.PreviewEnable)
            {
                var oldTokenSource = Interlocked.Exchange(ref this.thumbnailTokenSource, new CancellationTokenSource());
                oldTokenSource?.Cancel();

                var token = this.thumbnailTokenSource.Token;
                loadTasks.Add(this.tweetThumbnail1.ShowThumbnailAsync(_curPost, token));
            }

            try
            {
                await Task.WhenAll(loadTasks);
            }
            catch (OperationCanceledException) { }
        }

        private async void MatomeMenuItem_Click(object sender, EventArgs e)
        {
            await this.OpenApplicationWebsite();
        }

        private async Task OpenApplicationWebsite()
        {
            await this.OpenUriInBrowserAsync(ApplicationSettings.WebsiteUrl);
        }

        private async void ShortcutKeyListMenuItem_Click(object sender, EventArgs e)
        {
            await this.OpenUriInBrowserAsync(ApplicationSettings.ShortcutKeyUrl);
        }

        private async void ListTab_KeyDown(object sender, KeyEventArgs e)
        {
            if (ListTab.SelectedTab != null)
            {
                if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.PublicSearch)
                {
                    Control pnl = ListTab.SelectedTab.Controls["panelSearch"];
                    if (pnl.Controls["comboSearch"].Focused ||
                        pnl.Controls["comboLang"].Focused ||
                        pnl.Controls["buttonSearch"].Focused) return;
                }

                if (e.Control || e.Shift || e.Alt)
                    this._anchorFlag = false;

                Task asyncTask;
                if (CommonKeyDown(e.KeyData, FocusedControl.ListTab, out asyncTask))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }

                if (asyncTask != null)
                    await asyncTask;
            }
        }

        private ShortcutCommand[] shortcutCommands = new ShortcutCommand[0];

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
                    .Do(() => this.MenuItemSearchNext_Click(null, null)),

                ShortcutCommand.Create(Keys.F5)
                    .Do(() => this.DoRefresh()),

                ShortcutCommand.Create(Keys.F6)
                    .Do(() => this.GetReplyAsync()),

                ShortcutCommand.Create(Keys.F7)
                    .Do(() => this.GetDirectMessagesAsync()),

                ShortcutCommand.Create(Keys.Space, Keys.ProcessKey)
                    .NotFocusedOn(FocusedControl.StatusText)
                    .Do(() => { this._anchorFlag = false; this.JumpUnreadMenuItem_Click(null, null); }),

                ShortcutCommand.Create(Keys.G)
                    .NotFocusedOn(FocusedControl.StatusText)
                    .Do(() => { this._anchorFlag = false; this.ShowRelatedStatusesMenuItem_Click(null, null); }),

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
                    .Do(() => this.MakeReplyOrDirectStatus()),

                ShortcutCommand.Create(Keys.R)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.DoRefresh()),

                ShortcutCommand.Create(Keys.L)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => { this._anchorFlag = false; this.GoPost(forward: true); }),

                ShortcutCommand.Create(Keys.H)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => { this._anchorFlag = false; this.GoPost(forward: false); }),

                ShortcutCommand.Create(Keys.Z, Keys.Oemcomma)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => { this._anchorFlag = false; this.MoveTop(); }),

                ShortcutCommand.Create(Keys.S)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => { this._anchorFlag = false; this.GoNextTab(forward: true); }),

                ShortcutCommand.Create(Keys.A)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => { this._anchorFlag = false; this.GoNextTab(forward: false); }),

                // ] in_reply_to参照元へ戻る
                ShortcutCommand.Create(Keys.Oem4)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => { this._anchorFlag = false; return this.GoInReplyToPostTree(); }),

                // [ in_reply_toへジャンプ
                ShortcutCommand.Create(Keys.Oem6)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => { this._anchorFlag = false; this.GoBackInReplyToPostTree(); }),

                ShortcutCommand.Create(Keys.Escape)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => {
                        this._anchorFlag = false;
                        if (ListTab.SelectedTab != null)
                        {
                            var tabtype = _statuses.Tabs[ListTab.SelectedTab.Text].TabType;
                            if (tabtype == MyCommon.TabUsageType.Related || tabtype == MyCommon.TabUsageType.UserTimeline || tabtype == MyCommon.TabUsageType.PublicSearch || tabtype == MyCommon.TabUsageType.SearchResults)
                            {
                                var relTp = ListTab.SelectedTab;
                                RemoveSpecifiedTab(relTp.Text, false);
                                SaveConfigsTabs();
                            }
                        }
                    }),

                // 上下キー, PageUp/Downキー, Home/Endキー は既定の動作を残しつつアンカー初期化
                ShortcutCommand.Create(Keys.Up, Keys.Down, Keys.PageUp, Keys.PageDown, Keys.Home, Keys.End)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this._anchorFlag = false, preventDefault: false),

                // PreviewKeyDownEventArgs.IsInputKey を true にしてスクロールを発生させる
                ShortcutCommand.Create(Keys.Up, Keys.Down)
                    .FocusedOn(FocusedControl.PostBrowser)
                    .Do(() => { }),

                ShortcutCommand.Create(Keys.Control | Keys.R)
                    .Do(() => this.MakeReplyOrDirectStatus(isAuto: false, isReply: true)),

                ShortcutCommand.Create(Keys.Control | Keys.D)
                    .Do(() => this.doStatusDelete()),

                ShortcutCommand.Create(Keys.Control | Keys.M)
                    .Do(() => this.MakeReplyOrDirectStatus(isAuto: false, isReply: false)),

                ShortcutCommand.Create(Keys.Control | Keys.S)
                    .Do(() => this.FavoriteChange(FavAdd: true)),

                ShortcutCommand.Create(Keys.Control | Keys.I)
                    .Do(() => this.doRepliedStatusOpen()),

                ShortcutCommand.Create(Keys.Control | Keys.Q)
                    .Do(() => this.doQuoteOfficial()),

                ShortcutCommand.Create(Keys.Control | Keys.B)
                    .Do(() => this.ReadedStripMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.T)
                    .Do(() => this.HashManageMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.L)
                    .Do(() => this.UrlConvertAutoToolStripMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.Y)
                    .NotFocusedOn(FocusedControl.PostBrowser)
                    .Do(() => this.MultiLineMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.F)
                    .Do(() => this.MenuItemSubSearch_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.U)
                    .Do(() => this.ShowUserTimeline()),

                ShortcutCommand.Create(Keys.Control | Keys.H)
                    .Do(() => this.MoveToHomeToolStripMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.G)
                    .Do(() => this.MoveToFavToolStripMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.O)
                    .Do(() => this.StatusOpenMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.E)
                    .Do(() => this.OpenURLMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.Home, Keys.Control | Keys.End)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this._colorize = true, preventDefault: false),

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
                    .OnlyWhen(() => this.ListTab.TabPages.Count >= 1)
                    .Do(() => this.ListTab.SelectedIndex = 0),

                ShortcutCommand.Create(Keys.Control | Keys.D2)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.ListTab.TabPages.Count >= 2)
                    .Do(() => this.ListTab.SelectedIndex = 1),

                ShortcutCommand.Create(Keys.Control | Keys.D3)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.ListTab.TabPages.Count >= 3)
                    .Do(() => this.ListTab.SelectedIndex = 2),

                ShortcutCommand.Create(Keys.Control | Keys.D4)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.ListTab.TabPages.Count >= 4)
                    .Do(() => this.ListTab.SelectedIndex = 3),

                ShortcutCommand.Create(Keys.Control | Keys.D5)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.ListTab.TabPages.Count >= 5)
                    .Do(() => this.ListTab.SelectedIndex = 4),

                ShortcutCommand.Create(Keys.Control | Keys.D6)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.ListTab.TabPages.Count >= 6)
                    .Do(() => this.ListTab.SelectedIndex = 5),

                ShortcutCommand.Create(Keys.Control | Keys.D7)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.ListTab.TabPages.Count >= 7)
                    .Do(() => this.ListTab.SelectedIndex = 6),

                ShortcutCommand.Create(Keys.Control | Keys.D8)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => this.ListTab.TabPages.Count >= 8)
                    .Do(() => this.ListTab.SelectedIndex = 7),

                ShortcutCommand.Create(Keys.Control | Keys.D9)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.ListTab.SelectedIndex = this.ListTab.TabPages.Count - 1),

                ShortcutCommand.Create(Keys.Control | Keys.A)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => this.StatusText.SelectAll()),

                ShortcutCommand.Create(Keys.Control | Keys.V)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => this.ProcClipboardFromStatusTextWhenCtrlPlusV()),

                ShortcutCommand.Create(Keys.Control | Keys.Up)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => {
                        if (!string.IsNullOrWhiteSpace(StatusText.Text))
                        {
                            var inReplyToStatusId = this.inReplyTo?.Item1;
                            var inReplyToScreenName = this.inReplyTo?.Item2;
                            _history[_hisIdx] = new PostingStatus(StatusText.Text, inReplyToStatusId, inReplyToScreenName);
                        }
                        _hisIdx -= 1;
                        if (_hisIdx < 0) _hisIdx = 0;

                        var historyItem = this._history[this._hisIdx];
                        StatusText.Text = historyItem.status;
                        StatusText.SelectionStart = StatusText.Text.Length;
                        if (historyItem.inReplyToId != null)
                            this.inReplyTo = Tuple.Create(historyItem.inReplyToId.Value, historyItem.inReplyToName);
                        else
                            this.inReplyTo = null;
                    }),

                ShortcutCommand.Create(Keys.Control | Keys.Down)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => {
                        if (!string.IsNullOrWhiteSpace(StatusText.Text))
                        {
                            var inReplyToStatusId = this.inReplyTo?.Item1;
                            var inReplyToScreenName = this.inReplyTo?.Item2;
                            _history[_hisIdx] = new PostingStatus(StatusText.Text, inReplyToStatusId, inReplyToScreenName);
                        }
                        _hisIdx += 1;
                        if (_hisIdx > _history.Count - 1) _hisIdx = _history.Count - 1;

                        var historyItem = this._history[this._hisIdx];
                        StatusText.Text = historyItem.status;
                        StatusText.SelectionStart = StatusText.Text.Length;
                        if (historyItem.inReplyToId != null)
                            this.inReplyTo = Tuple.Create(historyItem.inReplyToId.Value, historyItem.inReplyToName);
                        else
                            this.inReplyTo = null;
                    }),

                ShortcutCommand.Create(Keys.Control | Keys.PageUp, Keys.Control | Keys.P)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => {
                        if (ListTab.SelectedIndex == 0)
                        {
                            ListTab.SelectedIndex = ListTab.TabCount - 1;
                        }
                        else
                        {
                            ListTab.SelectedIndex -= 1;
                        }
                        StatusText.Focus();
                    }),

                ShortcutCommand.Create(Keys.Control | Keys.PageDown, Keys.Control | Keys.N)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => {
                        if (ListTab.SelectedIndex == ListTab.TabCount - 1)
                        {
                            ListTab.SelectedIndex = 0;
                        }
                        else
                        {
                            ListTab.SelectedIndex += 1;
                        }
                        StatusText.Focus();
                    }),

                ShortcutCommand.Create(Keys.Control | Keys.Y)
                    .FocusedOn(FocusedControl.PostBrowser)
                    .Do(() => {
                        MultiLineMenuItem.Checked = !MultiLineMenuItem.Checked;
                        MultiLineMenuItem_Click(null, null);
                    }),

                ShortcutCommand.Create(Keys.Shift | Keys.F3)
                    .Do(() => this.MenuItemSearchPrev_Click(null, null)),

                ShortcutCommand.Create(Keys.Shift | Keys.F5)
                    .Do(() => this.DoRefreshMore()),

                ShortcutCommand.Create(Keys.Shift | Keys.F6)
                    .Do(() => this.GetReplyAsync(loadMore: true)),

                ShortcutCommand.Create(Keys.Shift | Keys.F7)
                    .Do(() => this.GetDirectMessagesAsync(loadMore: true)),

                ShortcutCommand.Create(Keys.Shift | Keys.R)
                    .NotFocusedOn(FocusedControl.StatusText)
                    .Do(() => this.DoRefreshMore()),

                ShortcutCommand.Create(Keys.Shift | Keys.H)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoTopEnd(GoTop: true)),

                ShortcutCommand.Create(Keys.Shift | Keys.L)
                    .FocusedOn(FocusedControl.ListTab)
                    .Do(() => this.GoTopEnd(GoTop: false)),

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
                    .Do(() => this.doReTweetOfficial(isConfirm: true)),

                ShortcutCommand.Create(Keys.Alt | Keys.P)
                    .OnlyWhen(() => this._curPost != null)
                    .Do(() => this.doShowUserStatus(_curPost.ScreenName, ShowInputDialog: false)),

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
                    .Do(() => this.MakeReplyOrDirectStatus(isAuto: false, isReply: true, isAll: true)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.C, Keys.Control | Keys.Shift | Keys.Insert)
                    .Do(() => this.CopyIdUri()),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.F)
                    .OnlyWhen(() => this.ListTab.SelectedTab != null &&
                        this._statuses.Tabs[this.ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.PublicSearch)
                    .Do(() => this.ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus()),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.S)
                    .Do(() => this.FavoriteChange(FavAdd: false)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.B)
                    .Do(() => this.UnreadStripMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.T)
                    .Do(() => this.HashToggleMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.P)
                    .Do(() => this.ImageSelectMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.H)
                    .Do(() => this.doMoveToRTHome()),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.O)
                    .Do(() => this.FavorareMenuItem_Click(null, null)),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.Up)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => {
                        if (_curList != null && _curList.VirtualListSize != 0 &&
                                    _curList.SelectedIndices.Count > 0 && _curList.SelectedIndices[0] > 0)
                        {
                            var idx = _curList.SelectedIndices[0] - 1;
                            SelectListItem(_curList, idx);
                            _curList.EnsureVisible(idx);
                        }
                    }),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.Down)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => {
                        if (_curList != null && _curList.VirtualListSize != 0 && _curList.SelectedIndices.Count > 0
                                    && _curList.SelectedIndices[0] < _curList.VirtualListSize - 1)
                        {
                            var idx = _curList.SelectedIndices[0] + 1;
                            SelectListItem(_curList, idx);
                            _curList.EnsureVisible(idx);
                        }
                    }),

                ShortcutCommand.Create(Keys.Control | Keys.Shift | Keys.Space)
                    .FocusedOn(FocusedControl.StatusText)
                    .Do(() => {
                        if (StatusText.SelectionStart > 0)
                        {
                            int endidx = StatusText.SelectionStart - 1;
                            string startstr = "";
                            for (int i = StatusText.SelectionStart - 1; i >= 0; i--)
                            {
                                char c = StatusText.Text[i];
                                if (Char.IsLetterOrDigit(c) || c == '_')
                                {
                                    continue;
                                }
                                if (c == '@')
                                {
                                    startstr = StatusText.Text.Substring(i + 1, endidx - i);
                                    int cnt = AtIdSupl.ItemCount;
                                    ShowSuplDialog(StatusText, AtIdSupl, startstr.Length + 1, startstr);
                                    if (AtIdSupl.ItemCount != cnt) ModifySettingAtId = true;
                                }
                                else if (c == '#')
                                {
                                    startstr = StatusText.Text.Substring(i + 1, endidx - i);
                                    ShowSuplDialog(StatusText, HashSupl, startstr.Length + 1, startstr);
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
                    .Do(() => this.FavoritesRetweetOfficial()),

                ShortcutCommand.Create(Keys.Control | Keys.Alt | Keys.R)
                    .Do(() => this.FavoritesRetweetUnofficial()),

                ShortcutCommand.Create(Keys.Control | Keys.Alt | Keys.H)
                    .Do(() => this.OpenUserAppointUrl()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.R)
                    .FocusedOn(FocusedControl.PostBrowser)
                    .Do(() => this.doReTweetUnofficial()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.T)
                    .OnlyWhen(() => this.ExistCurrentPost)
                    .Do(() => this.tweetDetailsView.DoTranslation()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.R)
                    .Do(() => this.doReTweetUnofficial()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.C, Keys.Alt | Keys.Shift | Keys.Insert)
                    .Do(() => this.CopyUserId()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.Up)
                    .Do(() => this.tweetThumbnail1.ScrollUp()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.Down)
                    .Do(() => this.tweetThumbnail1.ScrollDown()),

                ShortcutCommand.Create(Keys.Alt | Keys.Shift | Keys.Enter)
                    .FocusedOn(FocusedControl.ListTab)
                    .OnlyWhen(() => !this.SplitContainer3.Panel2Collapsed)
                    .Do(() => this.OpenThumbnailPicture(this.tweetThumbnail1.Thumbnail)),
            };
        }

        internal bool CommonKeyDown(Keys keyData, FocusedControl focusedOn, out Task asyncTask)
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
            int idx = ListTab.SelectedIndex;
            if (forward)
            {
                idx += 1;
                if (idx > ListTab.TabPages.Count - 1) idx = 0;
            }
            else
            {
                idx -= 1;
                if (idx < 0) idx = ListTab.TabPages.Count - 1;
            }
            ListTab.SelectedIndex = idx;
        }

        private void CopyStot()
        {
            string clstr = "";
            StringBuilder sb = new StringBuilder();
            bool IsProtected = false;
            bool isDm = false;
            if (this._curTab != null && this._statuses.GetTabByName(this._curTab.Text) != null) isDm = this._statuses.GetTabByName(this._curTab.Text).TabType == MyCommon.TabUsageType.DirectMessage;
            foreach (int idx in _curList.SelectedIndices)
            {
                PostClass post = _statuses.Tabs[_curTab.Text][idx];
                if (post.IsProtect)
                {
                    IsProtected = true;
                    continue;
                }
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
            if (IsProtected)
            {
                MessageBox.Show(Properties.Resources.CopyStotText1);
            }
            if (sb.Length > 0)
            {
                clstr = sb.ToString();
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
            string clstr = "";
            StringBuilder sb = new StringBuilder();
            if (this._curTab == null) return;
            if (this._statuses.GetTabByName(this._curTab.Text) == null) return;
            if (this._statuses.GetTabByName(this._curTab.Text).TabType == MyCommon.TabUsageType.DirectMessage) return;
            foreach (int idx in _curList.SelectedIndices)
            {
                var post = _statuses.Tabs[_curTab.Text][idx];
                sb.Append(MyCommon.GetStatusUrl(post));
                sb.Append(Environment.NewLine);
            }
            if (sb.Length > 0)
            {
                clstr = sb.ToString();
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

        private void GoFav(bool forward)
        {
            if (_curList.VirtualListSize == 0) return;
            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;

            if (forward)
            {
                if (_curList.SelectedIndices.Count == 0)
                {
                    fIdx = 0;
                }
                else
                {
                    fIdx = _curList.SelectedIndices[0] + 1;
                    if (fIdx > _curList.VirtualListSize - 1) return;
                }
                toIdx = _curList.VirtualListSize;
                stp = 1;
            }
            else
            {
                if (_curList.SelectedIndices.Count == 0)
                {
                    fIdx = _curList.VirtualListSize - 1;
                }
                else
                {
                    fIdx = _curList.SelectedIndices[0] - 1;
                    if (fIdx < 0) return;
                }
                toIdx = -1;
                stp = -1;
            }

            for (int idx = fIdx; idx != toIdx; idx += stp)
            {
                if (_statuses.Tabs[_curTab.Text][idx].IsFav)
                {
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoSamePostToAnotherTab(bool left)
        {
            if (_curList.VirtualListSize == 0) return;
            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;
            long targetId = 0;

            if (_statuses.Tabs[_curTab.Text].TabType == MyCommon.TabUsageType.DirectMessage) return; // Directタブは対象外（見つかるはずがない）
            if (_curList.SelectedIndices.Count == 0) return; //未選択も処理しない

            targetId = GetCurTabPost(_curList.SelectedIndices[0]).StatusId;

            if (left)
            {
                // 左のタブへ
                if (ListTab.SelectedIndex == 0)
                {
                    return;
                }
                else
                {
                    fIdx = ListTab.SelectedIndex - 1;
                }
                toIdx = -1;
                stp = -1;
            }
            else
            {
                // 右のタブへ
                if (ListTab.SelectedIndex == ListTab.TabCount - 1)
                {
                    return;
                }
                else
                {
                    fIdx = ListTab.SelectedIndex + 1;
                }
                toIdx = ListTab.TabCount;
                stp = 1;
            }

            bool found = false;
            for (int tabidx = fIdx; tabidx != toIdx; tabidx += stp)
            {
                if (_statuses.Tabs[ListTab.TabPages[tabidx].Text].TabType == MyCommon.TabUsageType.DirectMessage) continue; // Directタブは対象外
                for (int idx = 0; idx < ((DetailsListView)ListTab.TabPages[tabidx].Tag).VirtualListSize; idx++)
                {
                    if (_statuses.Tabs[ListTab.TabPages[tabidx].Text][idx].StatusId == targetId)
                    {
                        ListTab.SelectedIndex = tabidx;
                        SelectListItem(_curList, idx);
                        _curList.EnsureVisible(idx);
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
        }

        private void GoPost(bool forward)
        {
            if (_curList.SelectedIndices.Count == 0 || _curPost == null) return;
            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;

            if (forward)
            {
                fIdx = _curList.SelectedIndices[0] + 1;
                if (fIdx > _curList.VirtualListSize - 1) return;
                toIdx = _curList.VirtualListSize;
                stp = 1;
            }
            else
            {
                fIdx = _curList.SelectedIndices[0] - 1;
                if (fIdx < 0) return;
                toIdx = -1;
                stp = -1;
            }

            string name = "";
            if (_curPost.RetweetedId == null)
            {
                name = _curPost.ScreenName;
            }
            else
            {
                name = _curPost.RetweetedBy;
            }
            for (int idx = fIdx; idx != toIdx; idx += stp)
            {
                if (_statuses.Tabs[_curTab.Text][idx].RetweetedId == null)
                {
                    if (_statuses.Tabs[_curTab.Text][idx].ScreenName == name)
                    {
                        SelectListItem(_curList, idx);
                        _curList.EnsureVisible(idx);
                        break;
                    }
                }
                else
                {
                    if (_statuses.Tabs[_curTab.Text][idx].RetweetedBy == name)
                    {
                        SelectListItem(_curList, idx);
                        _curList.EnsureVisible(idx);
                        break;
                    }
                }
            }
        }

        private void GoRelPost(bool forward)
        {
            if (_curList.SelectedIndices.Count == 0) return;

            int fIdx = 0;
            int toIdx = 0;
            int stp = 1;
            if (forward)
            {
                fIdx = _curList.SelectedIndices[0] + 1;
                if (fIdx > _curList.VirtualListSize - 1) return;
                toIdx = _curList.VirtualListSize;
                stp = 1;
            }
            else
            {
                fIdx = _curList.SelectedIndices[0] - 1;
                if (fIdx < 0) return;
                toIdx = -1;
                stp = -1;
            }

            if (!_anchorFlag)
            {
                if (_curPost == null) return;
                _anchorPost = _curPost;
                _anchorFlag = true;
            }
            else
            {
                if (_anchorPost == null) return;
            }

            for (int idx = fIdx; idx != toIdx; idx += stp)
            {
                PostClass post = _statuses.Tabs[_curTab.Text][idx];
                if (post.ScreenName == _anchorPost.ScreenName ||
                    post.RetweetedBy == _anchorPost.ScreenName ||
                    post.ScreenName == _anchorPost.RetweetedBy ||
                    (!string.IsNullOrEmpty(post.RetweetedBy) && post.RetweetedBy == _anchorPost.RetweetedBy) ||
                    _anchorPost.ReplyToList.Contains(post.ScreenName.ToLowerInvariant()) ||
                    _anchorPost.ReplyToList.Contains(post.RetweetedBy.ToLowerInvariant()) ||
                    post.ReplyToList.Contains(_anchorPost.ScreenName.ToLowerInvariant()) ||
                    post.ReplyToList.Contains(_anchorPost.RetweetedBy.ToLowerInvariant()))
                {
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                    break;
                }
            }
        }

        private void GoAnchor()
        {
            if (_anchorPost == null) return;
            int idx = _statuses.Tabs[_curTab.Text].IndexOf(_anchorPost.StatusId);
            if (idx == -1) return;

            SelectListItem(_curList, idx);
            _curList.EnsureVisible(idx);
        }

        private void GoTopEnd(bool GoTop)
        {
            if (_curList.VirtualListSize == 0)
                return;

            ListViewItem _item;
            int idx;

            if (GoTop)
            {
                _item = _curList.GetItemAt(0, 25);
                if (_item == null)
                    idx = 0;
                else
                    idx = _item.Index;
            }
            else
            {
                _item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);
                if (_item == null)
                    idx = _curList.VirtualListSize - 1;
                else
                    idx = _item.Index;
            }
            SelectListItem(_curList, idx);
        }

        private void GoMiddle()
        {
            if (_curList.VirtualListSize == 0)
                return;

            ListViewItem _item;
            int idx1;
            int idx2;
            int idx3;

            _item = _curList.GetItemAt(0, 0);
            if (_item == null)
            {
                idx1 = 0;
            }
            else
            {
                idx1 = _item.Index;
            }

            _item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);
            if (_item == null)
            {
                idx2 = _curList.VirtualListSize - 1;
            }
            else
            {
                idx2 = _item.Index;
            }
            idx3 = (idx1 + idx2) / 2;

            SelectListItem(_curList, idx3);
        }

        private void GoLast()
        {
            if (_curList.VirtualListSize == 0) return;

            if (_statuses.SortOrder == SortOrder.Ascending)
            {
                SelectListItem(_curList, _curList.VirtualListSize - 1);
                _curList.EnsureVisible(_curList.VirtualListSize - 1);
            }
            else
            {
                SelectListItem(_curList, 0);
                _curList.EnsureVisible(0);
            }
        }

        private void MoveTop()
        {
            if (_curList.SelectedIndices.Count == 0) return;
            int idx = _curList.SelectedIndices[0];
            if (_statuses.SortOrder == SortOrder.Ascending)
            {
                _curList.EnsureVisible(_curList.VirtualListSize - 1);
            }
            else
            {
                _curList.EnsureVisible(0);
            }
            _curList.EnsureVisible(idx);
        }

        private async Task GoInReplyToPostTree()
        {
            if (_curPost == null) return;

            TabModel curTabClass = _statuses.Tabs[_curTab.Text];

            if (curTabClass.TabType == MyCommon.TabUsageType.PublicSearch && _curPost.InReplyToStatusId == null && _curPost.TextFromApi.Contains("@"))
            {
                try
                {
                    var post = await tw.GetStatusApi(false, _curPost.StatusId);

                    _curPost.InReplyToStatusId = post.InReplyToStatusId;
                    _curPost.InReplyToUser = post.InReplyToUser;
                    _curPost.IsReply = post.IsReply;
                    this.PurgeListViewItemCache();
                    _curList.RedrawItems(_curItemIndex, _curItemIndex, false);
                }
                catch (WebApiException ex)
                {
                    this.StatusLabel.Text = $"Err:{ex.Message}(GetStatus)";
                }
            }

            if (!(this.ExistCurrentPost && _curPost.InReplyToUser != null && _curPost.InReplyToStatusId != null)) return;

            if (replyChains == null || (replyChains.Count > 0 && replyChains.Peek().InReplyToId != _curPost.StatusId))
            {
                replyChains = new Stack<ReplyChain>();
            }
            replyChains.Push(new ReplyChain(_curPost.StatusId, _curPost.InReplyToStatusId.Value, _curTab));

            int inReplyToIndex;
            string inReplyToTabName;
            long inReplyToId = _curPost.InReplyToStatusId.Value;
            string inReplyToUser = _curPost.InReplyToUser;
            //Dictionary<long, PostClass> curTabPosts = curTabClass.Posts;

            var inReplyToPosts = from tab in _statuses.Tabs.Values
                                 orderby tab != curTabClass
                                 from post in tab.Posts.Values
                                 where post.StatusId == inReplyToId
                                 let index = tab.IndexOf(post.StatusId)
                                 where index != -1
                                 select new {Tab = tab, Index = index};

            var inReplyPost = inReplyToPosts.FirstOrDefault();
            if (inReplyPost == null)
            {
                try
                {
                    await Task.Run(async () =>
                    {
                        var post = await tw.GetStatusApi(false, _curPost.InReplyToStatusId.Value)
                            .ConfigureAwait(false);
                        post.IsRead = true;

                        _statuses.AddPost(post);
                        _statuses.DistributePosts();
                    });
                }
                catch (WebApiException ex)
                {
                    this.StatusLabel.Text = $"Err:{ex.Message}(GetStatus)";
                    await this.OpenUriInBrowserAsync(MyCommon.GetStatusUrl(inReplyToUser, inReplyToId));
                    return;
                }

                this.RefreshTimeline();

                inReplyPost = inReplyToPosts.FirstOrDefault();
                if (inReplyPost == null)
                {
                    await this.OpenUriInBrowserAsync(MyCommon.GetStatusUrl(inReplyToUser, inReplyToId));
                    return;
                }
            }
            inReplyToTabName = inReplyPost.Tab.TabName;
            inReplyToIndex = inReplyPost.Index;

            TabPage tabPage = this.ListTab.TabPages.Cast<TabPage>().First((tp) => { return tp.Text == inReplyToTabName; });
            DetailsListView listView = (DetailsListView)tabPage.Tag;

            if (_curTab != tabPage)
            {
                this.ListTab.SelectTab(tabPage);
            }

            this.SelectListItem(listView, inReplyToIndex);
            listView.EnsureVisible(inReplyToIndex);
        }

        private void GoBackInReplyToPostTree(bool parallel = false, bool isForward = true)
        {
            if (_curPost == null) return;

            TabModel curTabClass = _statuses.Tabs[_curTab.Text];
            //Dictionary<long, PostClass> curTabPosts = curTabClass.Posts;

            if (parallel)
            {
                if (_curPost.InReplyToStatusId != null)
                {
                    var posts = from t in _statuses.Tabs
                                from p in t.Value.Posts
                                where p.Value.StatusId != _curPost.StatusId && p.Value.InReplyToStatusId == _curPost.InReplyToStatusId
                                let indexOf = t.Value.IndexOf(p.Value.StatusId)
                                where indexOf > -1
                                orderby isForward ? indexOf : indexOf * -1
                                orderby t.Value != curTabClass
                                select new {Tab = t.Value, Post = p.Value, Index = indexOf};
                    try
                    {
                        var postList = posts.ToList();
                        for (int i = postList.Count - 1; i >= 0; i--)
                        {
                            int index = i;
                            if (postList.FindIndex((pst) => { return pst.Post.StatusId == postList[index].Post.StatusId; }) != index)
                            {
                                postList.RemoveAt(index);
                            }
                        }
                        var post = postList.FirstOrDefault((pst) => { return pst.Tab == curTabClass && isForward ? pst.Index > _curItemIndex : pst.Index < _curItemIndex; });
                        if (post == null) post = postList.FirstOrDefault((pst) => { return pst.Tab != curTabClass; });
                        if (post == null) post = postList.First();
                        this.ListTab.SelectTab(this.ListTab.TabPages.Cast<TabPage>().First((tp) => { return tp.Text == post.Tab.TabName; }));
                        DetailsListView listView = (DetailsListView)this.ListTab.SelectedTab.Tag;
                        SelectListItem(listView, post.Index);
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
                if (replyChains == null || replyChains.Count < 1)
                {
                    var posts = from t in _statuses.Tabs
                                from p in t.Value.Posts
                                where p.Value.InReplyToStatusId == _curPost.StatusId
                                let indexOf = t.Value.IndexOf(p.Value.StatusId)
                                where indexOf > -1
                                orderby indexOf
                                orderby t.Value != curTabClass
                                select new {Tab = t.Value, Index = indexOf};
                    try
                    {
                        var post = posts.First();
                        this.ListTab.SelectTab(this.ListTab.TabPages.Cast<TabPage>().First((tp) => { return tp.Text == post.Tab.TabName; }));
                        DetailsListView listView = (DetailsListView)this.ListTab.SelectedTab.Tag;
                        SelectListItem(listView, post.Index);
                        listView.EnsureVisible(post.Index);
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                }
                else
                {
                    ReplyChain chainHead = replyChains.Pop();
                    if (chainHead.InReplyToId == _curPost.StatusId)
                    {
                        int idx = _statuses.Tabs[chainHead.OriginalTab.Text].IndexOf(chainHead.OriginalId);
                        if (idx == -1)
                        {
                            replyChains = null;
                        }
                        else
                        {
                            try
                            {
                                ListTab.SelectTab(chainHead.OriginalTab);
                            }
                            catch (Exception)
                            {
                                replyChains = null;
                            }
                            SelectListItem(_curList, idx);
                            _curList.EnsureVisible(idx);
                        }
                    }
                    else
                    {
                        replyChains = null;
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
                TabPage tp = null;

                do
                {
                    try
                    {
                        this.selectPostChains.Pop();
                        var tabPostPair = this.selectPostChains.Peek();

                        if (!this.ListTab.TabPages.Contains(tabPostPair.Item1)) continue;  //該当タブが存在しないので無視

                        if (tabPostPair.Item2 != null)
                        {
                            idx = this._statuses.Tabs[tabPostPair.Item1.Text].IndexOf(tabPostPair.Item2.StatusId);
                            if (idx == -1) continue;  //該当ポストが存在しないので無視
                        }

                        tp = tabPostPair.Item1;

                        this.selectPostChains.Pop();
                    }
                    catch (InvalidOperationException)
                    {
                    }

                    break;
                }
                while (this.selectPostChains.Count > 1);

                if (tp == null)
                {
                    //状態がおかしいので処理を中断
                    //履歴が残り1つであればクリアしておく
                    if (this.selectPostChains.Count == 1)
                        this.selectPostChains.Clear();
                    return;
                }

                DetailsListView lst = (DetailsListView)tp.Tag;
                this.ListTab.SelectedTab = tp;
                if (idx > -1)
                {
                    SelectListItem(lst, idx);
                    lst.EnsureVisible(idx);
                }
                lst.Focus();
            }
        }

        private void PushSelectPostChain()
        {
            int count = this.selectPostChains.Count;
            if (count > 0)
            {
                var p = this.selectPostChains.Peek();
                if (p.Item1 == this._curTab)
                {
                    if (p.Item2 == this._curPost) return;  //最新の履歴と同一
                    if (p.Item2 == null) this.selectPostChains.Pop();  //置き換えるため削除
                }
            }
            if (count >= 2500) TrimPostChain();
            this.selectPostChains.Push(Tuple.Create(this._curTab, this._curPost));
        }

        private void TrimPostChain()
        {
            if (this.selectPostChains.Count <= 2000) return;
            var p = new Stack<Tuple<TabPage, PostClass>>(2000);
            for (int i = 0; i < 2000; i++)
            {
                p.Push(this.selectPostChains.Pop());
            }
            this.selectPostChains.Clear();
            for (int i = 0; i < 2000; i++)
            {
                this.selectPostChains.Push(p.Pop());
            }
        }

        private bool GoStatus(long statusId)
        {
            if (statusId == 0) return false;
            for (int tabidx = 0; tabidx < ListTab.TabCount; tabidx++)
            {
                if (_statuses.Tabs[ListTab.TabPages[tabidx].Text].TabType != MyCommon.TabUsageType.DirectMessage && _statuses.Tabs[ListTab.TabPages[tabidx].Text].Contains(statusId))
                {
                    int idx = _statuses.Tabs[ListTab.TabPages[tabidx].Text].IndexOf(statusId);
                    ListTab.SelectedIndex = tabidx;
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                    return true;
                }
            }
            return false;
        }

        private bool GoDirectMessage(long statusId)
        {
            if (statusId == 0) return false;
            for (int tabidx = 0; tabidx < ListTab.TabCount; tabidx++)
            {
                if (_statuses.Tabs[ListTab.TabPages[tabidx].Text].TabType == MyCommon.TabUsageType.DirectMessage && _statuses.Tabs[ListTab.TabPages[tabidx].Text].Contains(statusId))
                {
                    int idx = _statuses.Tabs[ListTab.TabPages[tabidx].Text].IndexOf(statusId);
                    ListTab.SelectedIndex = tabidx;
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                    return true;
                }
            }
            return false;
        }

        private void MyList_MouseClick(object sender, MouseEventArgs e)
        {
            _anchorFlag = false;
        }

        private void StatusText_Enter(object sender, EventArgs e)
        {
            // フォーカスの戻り先を StatusText に設定
            this.Tag = StatusText;
            StatusText.BackColor = _clInputBackcolor;
        }

        public Color InputBackColor
        {
            get { return _clInputBackcolor; }
            set { _clInputBackcolor = value; }
        }

        private void StatusText_Leave(object sender, EventArgs e)
        {
            // フォーカスがメニューに遷移しないならばフォーカスはタブに移ることを期待
            if (ListTab.SelectedTab != null && MenuStrip1.Tag == null) this.Tag = ListTab.SelectedTab.Tag;
            StatusText.BackColor = Color.FromKnownColor(KnownColor.Window);
        }

        private async void StatusText_KeyDown(object sender, KeyEventArgs e)
        {
            Task asyncTask;
            if (CommonKeyDown(e.KeyData, FocusedControl.StatusText, out asyncTask))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            this.StatusText_TextChanged(null, null);

            if (asyncTask != null)
                await asyncTask;
        }

        private void SaveConfigsAll(bool ifModified)
        {
            if (!ifModified)
            {
                SaveConfigsCommon();
                SaveConfigsLocal();
                SaveConfigsTabs();
                SaveConfigsAtId();
            }
            else
            {
                if (ModifySettingCommon) SaveConfigsCommon();
                if (ModifySettingLocal) SaveConfigsLocal();
                if (ModifySettingAtId) SaveConfigsAtId();
            }
        }

        private void SaveConfigsAtId()
        {
            if (_ignoreConfigSave || !this._cfgCommon.UseAtIdSupplement && AtIdSupl == null) return;

            ModifySettingAtId = false;
            SettingAtIdList cfgAtId = new SettingAtIdList(AtIdSupl.GetItemList());
            cfgAtId.Save();
        }

        private void SaveConfigsCommon()
        {
            if (_ignoreConfigSave) return;

            ModifySettingCommon = false;
            lock (_syncObject)
            {
                _cfgCommon.UserName = tw.Username;
                _cfgCommon.UserId = tw.UserId;
                _cfgCommon.Token = tw.AccessToken;
                _cfgCommon.TokenSecret = tw.AccessTokenSecret;

                if (IdeographicSpaceToSpaceToolStripMenuItem != null &&
                   IdeographicSpaceToSpaceToolStripMenuItem.IsDisposed == false)
                {
                    _cfgCommon.WideSpaceConvert = this.IdeographicSpaceToSpaceToolStripMenuItem.Checked;
                }

                _cfgCommon.SortOrder = (int)_statuses.SortOrder;
                switch (_statuses.SortMode)
                {
                    case ComparerMode.Nickname:  //ニックネーム
                        _cfgCommon.SortColumn = 1;
                        break;
                    case ComparerMode.Data:  //本文
                        _cfgCommon.SortColumn = 2;
                        break;
                    case ComparerMode.Id:  //時刻=発言Id
                        _cfgCommon.SortColumn = 3;
                        break;
                    case ComparerMode.Name:  //名前
                        _cfgCommon.SortColumn = 4;
                        break;
                    case ComparerMode.Source:  //Source
                        _cfgCommon.SortColumn = 7;
                        break;
                }

                _cfgCommon.HashTags = HashMgr.HashHistories;
                if (HashMgr.IsPermanent)
                {
                    _cfgCommon.HashSelected = HashMgr.UseHash;
                }
                else
                {
                    _cfgCommon.HashSelected = "";
                }
                _cfgCommon.HashIsHead = HashMgr.IsHead;
                _cfgCommon.HashIsPermanent = HashMgr.IsPermanent;
                _cfgCommon.HashIsNotAddToAtReply = HashMgr.IsNotAddToAtReply;
                if (ToolStripFocusLockMenuItem != null &&
                        ToolStripFocusLockMenuItem.IsDisposed == false)
                {
                    _cfgCommon.FocusLockToStatusText = this.ToolStripFocusLockMenuItem.Checked;
                }
                _cfgCommon.TrackWord = tw.TrackWord;
                _cfgCommon.AllAtReply = tw.AllAtReply;
                _cfgCommon.UseImageService = ImageSelector.ServiceIndex;
                _cfgCommon.UseImageServiceName = ImageSelector.ServiceName;

                _cfgCommon.Save();
            }
        }

        private void SaveConfigsLocal()
        {
            if (_ignoreConfigSave) return;
            lock (_syncObject)
            {
                ModifySettingLocal = false;
                _cfgLocal.ScaleDimension = this.CurrentAutoScaleDimensions;
                _cfgLocal.FormSize = _mySize;
                _cfgLocal.FormLocation = _myLoc;
                _cfgLocal.SplitterDistance = _mySpDis;
                _cfgLocal.PreviewDistance = _mySpDis3;
                _cfgLocal.StatusMultiline = StatusText.Multiline;
                _cfgLocal.StatusTextHeight = _mySpDis2;

                _cfgLocal.FontUnread = _fntUnread;
                _cfgLocal.ColorUnread = _clUnread;
                _cfgLocal.FontRead = _fntReaded;
                _cfgLocal.ColorRead = _clReaded;
                _cfgLocal.FontDetail = _fntDetail;
                _cfgLocal.ColorDetail = _clDetail;
                _cfgLocal.ColorDetailBackcolor = _clDetailBackcolor;
                _cfgLocal.ColorDetailLink = _clDetailLink;
                _cfgLocal.ColorFav = _clFav;
                _cfgLocal.ColorOWL = _clOWL;
                _cfgLocal.ColorRetweet = _clRetweet;
                _cfgLocal.ColorSelf = _clSelf;
                _cfgLocal.ColorAtSelf = _clAtSelf;
                _cfgLocal.ColorTarget = _clTarget;
                _cfgLocal.ColorAtTarget = _clAtTarget;
                _cfgLocal.ColorAtFromTarget = _clAtFromTarget;
                _cfgLocal.ColorAtTo = _clAtTo;
                _cfgLocal.ColorListBackcolor = _clListBackcolor;
                _cfgLocal.ColorInputBackcolor = _clInputBackcolor;
                _cfgLocal.ColorInputFont = _clInputFont;
                _cfgLocal.FontInputFont = _fntInputFont;

                if (_ignoreConfigSave) return;
                _cfgLocal.Save();
            }
        }

        private void SaveConfigsTabs()
        {
            var tabsSetting = new SettingTabs();

            var tabs = this.ListTab.TabPages.Cast<TabPage>()
                .Select(x => this._statuses.Tabs[x.Text])
                .Concat(new[] { this._statuses.GetTabByType(MyCommon.TabUsageType.Mute) });

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

                var filterTab = tab as FilterTabModel;
                if (filterTab != null)
                    tabSetting.FilterArray = filterTab.FilterArray;

                var userTab = tab as UserTimelineTabModel;
                if (userTab != null)
                    tabSetting.User = userTab.ScreenName;

                var searchTab = tab as PublicSearchTabModel;
                if (searchTab != null)
                {
                    tabSetting.SearchWords = searchTab.SearchWords;
                    tabSetting.SearchLang = searchTab.SearchLang;
                }

                var listTab = tab as ListTimelineTabModel;
                if (listTab != null)
                    tabSetting.ListInfo = listTab.ListInfo;

                tabsSetting.Tabs.Add(tabSetting);
            }

            tabsSetting.Save();
        }

        private async void OpenURLFileMenuItem_Click(object sender, EventArgs e)
        {
            string inputText;
            var ret = InputDialog.Show(this, Properties.Resources.OpenURL_InputText, Properties.Resources.OpenURL_Caption, out inputText);
            if (ret != DialogResult.OK)
                return;

            var match = Twitter.StatusUrlRegex.Match(inputText);
            if (!match.Success)
            {
                MessageBox.Show(this, Properties.Resources.OpenURL_InvalidFormat,
                    Properties.Resources.OpenURL_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var statusId = long.Parse(match.Groups["StatusId"].Value);
                await this.OpenRelatedTab(statusId);
            }
            catch (TabException ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveLogMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult rslt = MessageBox.Show(string.Format(Properties.Resources.SaveLogMenuItem_ClickText1, Environment.NewLine),
                    Properties.Resources.SaveLogMenuItem_ClickText2,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel) return;

            SaveFileDialog1.FileName = MyCommon.GetAssemblyName() + "Posts" + DateTime.Now.ToString("yyMMdd-HHmmss") + ".tsv";
            SaveFileDialog1.InitialDirectory = Application.ExecutablePath;
            SaveFileDialog1.Filter = Properties.Resources.SaveLogMenuItem_ClickText3;
            SaveFileDialog1.FilterIndex = 0;
            SaveFileDialog1.Title = Properties.Resources.SaveLogMenuItem_ClickText4;
            SaveFileDialog1.RestoreDirectory = true;

            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!SaveFileDialog1.ValidateNames) return;
                using (StreamWriter sw = new StreamWriter(SaveFileDialog1.FileName, false, Encoding.UTF8))
                {
                    if (rslt == DialogResult.Yes)
                    {
                        //All
                        for (int idx = 0; idx < _curList.VirtualListSize; idx++)
                        {
                            PostClass post = _statuses.Tabs[_curTab.Text][idx];
                            string protect = "";
                            if (post.IsProtect) protect = "Protect";
                            sw.WriteLine(post.Nickname + "\t" +
                                     "\"" + post.TextFromApi.Replace("\n", "").Replace("\"", "\"\"") + "\"" + "\t" +
                                     post.CreatedAt + "\t" +
                                     post.ScreenName + "\t" +
                                     post.StatusId + "\t" +
                                     post.ImageUrl + "\t" +
                                     "\"" + post.Text.Replace("\n", "").Replace("\"", "\"\"") + "\"" + "\t" +
                                     protect);
                        }
                    }
                    else
                    {
                        foreach (int idx in _curList.SelectedIndices)
                        {
                            PostClass post = _statuses.Tabs[_curTab.Text][idx];
                            string protect = "";
                            if (post.IsProtect) protect = "Protect";
                            sw.WriteLine(post.Nickname + "\t" +
                                     "\"" + post.TextFromApi.Replace("\n", "").Replace("\"", "\"\"") + "\"" + "\t" +
                                     post.CreatedAt + "\t" +
                                     post.ScreenName + "\t" +
                                     post.StatusId + "\t" +
                                     post.ImageUrl + "\t" +
                                     "\"" + post.Text.Replace("\n", "").Replace("\"", "\"\"") + "\"" + "\t" +
                                     protect);
                        }
                    }
                }
            }
            this.TopMost = this._cfgCommon.AlwaysTop;
        }

        public bool TabRename(ref string tabName)
        {
            //タブ名変更
            string newTabText = null;
            using (InputTabName inputName = new InputTabName())
            {
                inputName.TabName = tabName;
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel) return false;
                newTabText = inputName.TabName;
            }
            this.TopMost = this._cfgCommon.AlwaysTop;
            if (!string.IsNullOrEmpty(newTabText))
            {
                //新タブ名存在チェック
                for (int i = 0; i < ListTab.TabCount; i++)
                {
                    if (ListTab.TabPages[i].Text == newTabText)
                    {
                        string tmp = string.Format(Properties.Resources.Tabs_DoubleClickText1, newTabText);
                        MessageBox.Show(tmp, Properties.Resources.Tabs_DoubleClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                //タブ名を変更
                for (int i = 0; i < ListTab.TabCount; i++)
                {
                    if (ListTab.TabPages[i].Text == tabName)
                    {
                        ListTab.TabPages[i].Text = newTabText;
                        break;
                    }
                }
                _statuses.RenameTab(tabName, newTabText);

                SaveConfigsCommon();
                SaveConfigsTabs();
                _rclickTabName = newTabText;
                tabName = newTabText;
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
                for (int i = 0; i < this.ListTab.TabPages.Count; i++)
                {
                    if (this.ListTab.GetTabRect(i).Contains(e.Location))
                    {
                        this.RemoveSpecifiedTab(this.ListTab.TabPages[i].Text, true);
                        this.SaveConfigsTabs();
                        break;
                    }
                }
            }
        }

        private void ListTab_DoubleClick(object sender, MouseEventArgs e)
        {
            string tn = ListTab.SelectedTab.Text;
            TabRename(ref tn);
        }

        private void ListTab_MouseDown(object sender, MouseEventArgs e)
        {
            if (this._cfgCommon.TabMouseLock) return;
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < ListTab.TabPages.Count; i++)
                {
                    if (this.ListTab.GetTabRect(i).Contains(e.Location))
                    {
                        _tabDrag = true;
                        _tabMouseDownPoint = e.Location;
                        break;
                    }
                }
            }
            else
            {
                _tabDrag = false;
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

            _tabDrag = false;
            string tn = "";
            bool bef = false;
            Point cpos = new Point(e.X, e.Y);
            Point spos = ListTab.PointToClient(cpos);
            int i;
            for (i = 0; i < ListTab.TabPages.Count; i++)
            {
                Rectangle rect = ListTab.GetTabRect(i);
                if (rect.Left <= spos.X && spos.X <= rect.Right &&
                    rect.Top <= spos.Y && spos.Y <= rect.Bottom)
                {
                    tn = ListTab.TabPages[i].Text;
                    if (spos.X <= (rect.Left + rect.Right) / 2)
                        bef = true;
                    else
                        bef = false;

                    break;
                }
            }

            //タブのないところにドロップ->最後尾へ移動
            if (string.IsNullOrEmpty(tn))
            {
                tn = ListTab.TabPages[ListTab.TabPages.Count - 1].Text;
                bef = false;
                i = ListTab.TabPages.Count - 1;
            }

            TabPage tp = (TabPage)e.Data.GetData(typeof(TabPage));
            if (tp.Text == tn) return;

            ReOrderTab(tp.Text, tn, bef);
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
                var mTp = this.ListTab.TabPages[targetIndex];
                this.ListTab.TabPages.Remove(mTp);

                if (targetIndex < baseIndex)
                    baseIndex--;

                if (isBeforeBaseTab)
                    ListTab.TabPages.Insert(baseIndex, mTp);
                else
                    ListTab.TabPages.Insert(baseIndex + 1, mTp);
            }

            SaveConfigsTabs();
        }

        private void MakeReplyOrDirectStatus(bool isAuto = true, bool isReply = true, bool isAll = false)
        {
            //isAuto:true=先頭に挿入、false=カーソル位置に挿入
            //isReply:true=@,false=DM
            if (!StatusText.Enabled) return;
            if (_curList == null) return;
            if (_curTab == null) return;
            if (!this.ExistCurrentPost) return;

            // 複数あてリプライはReplyではなく通常ポスト
            //↑仕様変更で全部リプライ扱いでＯＫ（先頭ドット付加しない）
            //090403暫定でドットを付加しないようにだけ修正。単独と複数の処理は統合できると思われる。
            //090513 all @ replies 廃止の仕様変更によりドット付加に戻し(syo68k)

            if (_curList.SelectedIndices.Count > 0)
            {
                // アイテムが1件以上選択されている
                if (_curList.SelectedIndices.Count == 1 && !isAll && this.ExistCurrentPost)
                {
                    // 単独ユーザー宛リプライまたはDM
                    if ((_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.DirectMessage && isAuto) || (!isAuto && !isReply))
                    {
                        // ダイレクトメッセージ
                        StatusText.Text = "D " + _curPost.ScreenName + " " + StatusText.Text;
                        StatusText.SelectionStart = StatusText.Text.Length;
                        StatusText.Focus();
                        this.inReplyTo = null;
                        return;
                    }
                    if (string.IsNullOrEmpty(StatusText.Text))
                    {
                        //空の場合

                        // ステータステキストが入力されていない場合先頭に@ユーザー名を追加する
                        StatusText.Text = "@" + _curPost.ScreenName + " ";

                        var inReplyToStatusId = this._curPost.RetweetedId ?? this._curPost.StatusId;
                        var inReplyToScreenName = this._curPost.ScreenName;
                        this.inReplyTo = Tuple.Create(inReplyToStatusId, inReplyToScreenName);
                    }
                    else
                    {
                        //何か入力済の場合

                        if (isAuto)
                        {
                            //1件選んでEnter or DoubleClick
                            if (StatusText.Text.Contains("@" + _curPost.ScreenName + " "))
                            {
                                if (this.inReplyTo?.Item2 == _curPost.ScreenName)
                                {
                                    //返信先書き換え
                                    var inReplyToStatusId = this._curPost.RetweetedId ?? this._curPost.StatusId;
                                    var inReplyToScreenName = this._curPost.ScreenName;
                                    this.inReplyTo = Tuple.Create(inReplyToStatusId, inReplyToScreenName);
                                }
                                return;
                            }
                            if (!StatusText.Text.StartsWith("@", StringComparison.Ordinal))
                            {
                                //文頭＠以外
                                if (StatusText.Text.StartsWith(". ", StringComparison.Ordinal))
                                {
                                    // 複数リプライ
                                    StatusText.Text = StatusText.Text.Insert(2, "@" + _curPost.ScreenName + " ");
                                    this.inReplyTo = null;
                                }
                                else
                                {
                                    // 単独リプライ
                                    StatusText.Text = "@" + _curPost.ScreenName + " " + StatusText.Text;
                                    var inReplyToStatusId = this._curPost.RetweetedId ?? this._curPost.StatusId;
                                    var inReplyToScreenName = this._curPost.ScreenName;
                                    this.inReplyTo = Tuple.Create(inReplyToStatusId, inReplyToScreenName);
                                }
                            }
                            else
                            {
                                //文頭＠
                                // 複数リプライ
                                StatusText.Text = ". @" + _curPost.ScreenName + " " + StatusText.Text;
                                //StatusText.Text = "@" + _curPost.ScreenName + " " + StatusText.Text;
                                this.inReplyTo = null;
                            }
                        }
                        else
                        {
                            //1件選んでCtrl-Rの場合（返信先操作せず）
                            int sidx = StatusText.SelectionStart;
                            string id = "@" + _curPost.ScreenName + " ";
                            if (sidx > 0)
                            {
                                if (StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    id = " " + id;
                                }
                            }
                            StatusText.Text = StatusText.Text.Insert(sidx, id);
                            sidx += id.Length;
                            //if (StatusText.Text.StartsWith("@"))
                            //{
                            //    //複数リプライ
                            //    StatusText.Text = ". " + StatusText.Text.Insert(sidx, " @" + _curPost.ScreenName + " ");
                            //    sidx += 5 + _curPost.ScreenName.Length;
                            //}
                            //else
                            //{
                            //    // 複数リプライ
                            //    StatusText.Text = StatusText.Text.Insert(sidx, " @" + _curPost.ScreenName + " ");
                            //    sidx += 3 + _curPost.ScreenName.Length;
                            //}
                            StatusText.SelectionStart = sidx;
                            StatusText.Focus();
                            //_reply_to_id = 0;
                            //_reply_to_name = null;
                            return;
                        }
                    }
                }
                else
                {
                    // 複数リプライ
                    if (!isAuto && !isReply) return;

                    //C-S-rか、複数の宛先を選択中にEnter/DoubleClick/C-r/C-S-r

                    if (isAuto)
                    {
                        //Enter or DoubleClick

                        string sTxt = StatusText.Text;
                        if (!sTxt.StartsWith(". ", StringComparison.Ordinal))
                        {
                            sTxt = ". " + sTxt;
                            this.inReplyTo = null;
                        }
                        for (int cnt = 0; cnt < _curList.SelectedIndices.Count; cnt++)
                        {
                            PostClass post = _statuses.Tabs[_curTab.Text][_curList.SelectedIndices[cnt]];
                            if (!sTxt.Contains("@" + post.ScreenName + " "))
                            {
                                sTxt = sTxt.Insert(2, "@" + post.ScreenName + " ");
                                //sTxt = "@" + post.ScreenName + " " + sTxt;
                            }
                        }
                        StatusText.Text = sTxt;
                    }
                    else
                    {
                        //C-S-r or C-r
                        if (_curList.SelectedIndices.Count > 1)
                        {
                            //複数ポスト選択

                            string ids = "";
                            int sidx = StatusText.SelectionStart;
                            for (int cnt = 0; cnt < _curList.SelectedIndices.Count; cnt++)
                            {
                                PostClass post = _statuses.Tabs[_curTab.Text][_curList.SelectedIndices[cnt]];
                                if (!ids.Contains("@" + post.ScreenName + " ") &&
                                    !post.ScreenName.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    ids += "@" + post.ScreenName + " ";
                                }
                                if (isAll)
                                {
                                    foreach (string nm in post.ReplyToList)
                                    {
                                        if (!ids.Contains("@" + nm + " ") &&
                                            !nm.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            Match m = Regex.Match(post.TextFromApi, "[@＠](?<id>" + nm + ")([^a-zA-Z0-9]|$)", RegexOptions.IgnoreCase);
                                            if (m.Success)
                                                ids += "@" + m.Result("${id}") + " ";
                                            else
                                                ids += "@" + nm + " ";
                                        }
                                    }
                                }
                            }
                            if (ids.Length == 0) return;
                            if (!StatusText.Text.StartsWith(". ", StringComparison.Ordinal))
                            {
                                StatusText.Text = ". " + StatusText.Text;
                                sidx += 2;
                                this.inReplyTo = null;
                            }
                            if (sidx > 0)
                            {
                                if (StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    ids = " " + ids;
                                }
                            }
                            StatusText.Text = StatusText.Text.Insert(sidx, ids);
                            sidx += ids.Length;
                            //if (StatusText.Text.StartsWith("@"))
                            //{
                            //    StatusText.Text = ". " + StatusText.Text.Insert(sidx, ids);
                            //    sidx += 2 + ids.Length;
                            //}
                            //else
                            //{
                            //    StatusText.Text = StatusText.Text.Insert(sidx, ids);
                            //    sidx += 1 + ids.Length;
                            //}
                            StatusText.SelectionStart = sidx;
                            StatusText.Focus();
                            return;
                        }
                        else
                        {
                            //1件のみ選択のC-S-r（返信元付加する可能性あり）

                            string ids = "";
                            int sidx = StatusText.SelectionStart;
                            PostClass post = _curPost;
                            if (!ids.Contains("@" + post.ScreenName + " ") &&
                                !post.ScreenName.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase))
                            {
                                ids += "@" + post.ScreenName + " ";
                            }
                            foreach (string nm in post.ReplyToList)
                            {
                                if (!ids.Contains("@" + nm + " ") &&
                                    !nm.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Match m = Regex.Match(post.TextFromApi, "[@＠](?<id>" + nm + ")([^a-zA-Z0-9]|$)", RegexOptions.IgnoreCase);
                                    if (m.Success)
                                        ids += "@" + m.Result("${id}") + " ";
                                    else
                                        ids += "@" + nm + " ";
                                }
                            }
                            if (!string.IsNullOrEmpty(post.RetweetedBy))
                            {
                                if (!ids.Contains("@" + post.RetweetedBy + " ") &&
                                   !post.RetweetedBy.Equals(tw.Username, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    ids += "@" + post.RetweetedBy + " ";
                                }
                            }
                            if (ids.Length == 0) return;
                            if (string.IsNullOrEmpty(StatusText.Text))
                            {
                                //未入力の場合のみ返信先付加
                                StatusText.Text = ids;
                                StatusText.SelectionStart = ids.Length;
                                StatusText.Focus();

                                var inReplyToStatusId = this._curPost.RetweetedId ?? this._curPost.StatusId;
                                var inReplyToScreenName = this._curPost.ScreenName;
                                this.inReplyTo = Tuple.Create(inReplyToStatusId, inReplyToScreenName);
                                return;
                            }

                            if (sidx > 0)
                            {
                                if (StatusText.Text.Substring(sidx - 1, 1) != " ")
                                {
                                    ids = " " + ids;
                                }
                            }
                            StatusText.Text = StatusText.Text.Insert(sidx, ids);
                            sidx += ids.Length;
                            StatusText.SelectionStart = sidx;
                            StatusText.Focus();
                            return;
                        }
                    }
                }
                StatusText.SelectionStart = StatusText.Text.Length;
                StatusText.Focus();
            }
        }

        private void ListTab_MouseUp(object sender, MouseEventArgs e)
        {
            _tabDrag = false;
        }

        private static int iconCnt = 0;
        private static int blinkCnt = 0;
        private static bool blink = false;
        private static bool idle = false;

        private async Task RefreshTasktrayIcon()
        {
            if (_colorize)
                await this.Colorize();

            if (!TimerRefreshIcon.Enabled) return;
            //Static usCheckCnt As int = 0

            //Static iconDlListTopItem As ListViewItem = null

            //if (((ListView)ListTab.SelectedTab.Tag).TopItem == iconDlListTopItem)
            //    ((ImageDictionary)this.TIconDic).PauseGetImage = false;
            //else
            //    ((ImageDictionary)this.TIconDic).PauseGetImage = true;
            //
            //iconDlListTopItem = ((ListView)ListTab.SelectedTab.Tag).TopItem;

            iconCnt += 1;
            blinkCnt += 1;
            //usCheckCnt += 1;

            //if (usCheckCnt > 300)    //1min
            //{
            //    usCheckCnt = 0;
            //    if (!this.IsReceivedUserStream)
            //    {
            //        TraceOut("ReconnectUserStream");
            //        tw.ReconnectUserStream();
            //    }
            //}

            var busy = this.workerSemaphore.CurrentCount != MAX_WORKER_THREADS;

            if (iconCnt >= this.NIconRefresh.Length)
            {
                iconCnt = 0;
            }
            if (blinkCnt > 10)
            {
                blinkCnt = 0;
                //未保存の変更を保存
                SaveConfigsAll(true);
            }

            if (busy)
            {
                NotifyIcon1.Icon = NIconRefresh[iconCnt];
                idle = false;
                _myStatusError = false;
                return;
            }

            TabModel tb = _statuses.GetTabByType(MyCommon.TabUsageType.Mentions);
            if (this._cfgCommon.ReplyIconState != MyCommon.REPLY_ICONSTATE.None && tb != null && tb.UnreadCount > 0)
            {
                if (blinkCnt > 0) return;
                blink = !blink;
                if (blink || this._cfgCommon.ReplyIconState == MyCommon.REPLY_ICONSTATE.StaticIcon)
                {
                    NotifyIcon1.Icon = ReplyIcon;
                }
                else
                {
                    NotifyIcon1.Icon = ReplyIconBlink;
                }
                idle = false;
                return;
            }

            if (idle) return;
            idle = true;
            //優先度：エラー→オフライン→アイドル
            //エラーは更新アイコンでクリアされる
            if (_myStatusError)
            {
                NotifyIcon1.Icon = NIconAtRed;
                return;
            }
            if (_myStatusOnline)
            {
                NotifyIcon1.Icon = NIconAt;
            }
            else
            {
                NotifyIcon1.Icon = NIconAtSmoke;
            }
        }

        private async void TimerRefreshIcon_Tick(object sender, EventArgs e)
        {
            //200ms
            await this.RefreshTasktrayIcon();
        }

        private void ContextMenuTabProperty_Opening(object sender, CancelEventArgs e)
        {
            //右クリックの場合はタブ名が設定済。アプリケーションキーの場合は現在のタブを対象とする
            if (string.IsNullOrEmpty(_rclickTabName) || sender != ContextMenuTabProperty)
            {
                if (ListTab != null && ListTab.SelectedTab != null)
                    _rclickTabName = ListTab.SelectedTab.Text;
                else
                    return;
            }

            if (_statuses == null) return;
            if (_statuses.Tabs == null) return;

            TabModel tb = _statuses.Tabs[_rclickTabName];
            if (tb == null) return;

            NotifyDispMenuItem.Checked = tb.Notify;
            this.NotifyTbMenuItem.Checked = tb.Notify;

            soundfileListup = true;
            SoundFileComboBox.Items.Clear();
            this.SoundFileTbComboBox.Items.Clear();
            SoundFileComboBox.Items.Add("");
            this.SoundFileTbComboBox.Items.Add("");
            DirectoryInfo oDir = new DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(Application.StartupPath, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (FileInfo oFile in oDir.GetFiles("*.wav"))
            {
                SoundFileComboBox.Items.Add(oFile.Name);
                this.SoundFileTbComboBox.Items.Add(oFile.Name);
            }
            int idx = SoundFileComboBox.Items.IndexOf(tb.SoundFile);
            if (idx == -1) idx = 0;
            SoundFileComboBox.SelectedIndex = idx;
            this.SoundFileTbComboBox.SelectedIndex = idx;
            soundfileListup = false;
            UreadManageMenuItem.Checked = tb.UnreadManage;
            this.UnreadMngTbMenuItem.Checked = tb.UnreadManage;

            TabMenuControl(_rclickTabName);
        }

        private void TabMenuControl(string tabName)
        {
            var tabInfo = _statuses.GetTabByName(tabName);

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

            if (string.IsNullOrEmpty(_rclickTabName)) return;
            _statuses.Tabs[_rclickTabName].Protected = checkState;

            SaveConfigsTabs();
        }

        private void UreadManageMenuItem_Click(object sender, EventArgs e)
        {
            UreadManageMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.UnreadMngTbMenuItem.Checked = UreadManageMenuItem.Checked;

            if (string.IsNullOrEmpty(_rclickTabName)) return;
            ChangeTabUnreadManage(_rclickTabName, UreadManageMenuItem.Checked);

            SaveConfigsTabs();
        }

        public void ChangeTabUnreadManage(string tabName, bool isManage)
        {
            var idx = this.GetTabPageIndex(tabName);
            if (idx == -1)
                return;

            _statuses.Tabs[tabName].UnreadManage = isManage;
            if (this._cfgCommon.TabIconDisp)
            {
                if (_statuses.Tabs[tabName].UnreadCount > 0)
                    ListTab.TabPages[idx].ImageIndex = 0;
                else
                    ListTab.TabPages[idx].ImageIndex = -1;
            }

            if (_curTab.Text == tabName)
            {
                this.PurgeListViewItemCache();
                _curList.Refresh();
            }

            SetMainWindowTitle();
            SetStatusLabelUrl();
            if (!this._cfgCommon.TabIconDisp) ListTab.Refresh();
        }

        private void NotifyDispMenuItem_Click(object sender, EventArgs e)
        {
            NotifyDispMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.NotifyTbMenuItem.Checked = NotifyDispMenuItem.Checked;

            if (string.IsNullOrEmpty(_rclickTabName)) return;

            _statuses.Tabs[_rclickTabName].Notify = NotifyDispMenuItem.Checked;

            SaveConfigsTabs();
        }

        private void SoundFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (soundfileListup || string.IsNullOrEmpty(_rclickTabName)) return;

            _statuses.Tabs[_rclickTabName].SoundFile = (string)((ToolStripComboBox)sender).SelectedItem;

            SaveConfigsTabs();
        }

        private void DeleteTabMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_rclickTabName) || sender == this.DeleteTbMenuItem) _rclickTabName = ListTab.SelectedTab.Text;

            RemoveSpecifiedTab(_rclickTabName, true);
            SaveConfigsTabs();
        }

        private void FilterEditMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_rclickTabName)) _rclickTabName = _statuses.GetTabByType(MyCommon.TabUsageType.Home).TabName;

            using (var fltDialog = new FilterDialog())
            {
                fltDialog.Owner = this;
                fltDialog.SetCurrent(_rclickTabName);
                fltDialog.ShowDialog(this);
            }
            this.TopMost = this._cfgCommon.AlwaysTop;

            this.ApplyPostFilters();
            SaveConfigsTabs();
        }

        private void AddTabMenuItem_Click(object sender, EventArgs e)
        {
            string tabName = null;
            MyCommon.TabUsageType tabUsage;
            using (InputTabName inputName = new InputTabName())
            {
                inputName.TabName = _statuses.MakeTabName("MyTab");
                inputName.IsShowUsage = true;
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel) return;
                tabName = inputName.TabName;
                tabUsage = inputName.Usage;
            }
            this.TopMost = this._cfgCommon.AlwaysTop;
            if (!string.IsNullOrEmpty(tabName))
            {
                //List対応
                ListElement list = null;
                if (tabUsage == MyCommon.TabUsageType.Lists)
                {
                    using (ListAvailable listAvail = new ListAvailable())
                    {
                        if (listAvail.ShowDialog(this) == DialogResult.Cancel) return;
                        if (listAvail.SelectedList == null) return;
                        list = listAvail.SelectedList;
                    }
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
                        tab = new ListTimelineTabModel(tabName, list);
                        break;
                    default:
                        return;
                }

                if (!_statuses.AddTab(tab) || !AddNewTab(tab, startup: false))
                {
                    string tmp = string.Format(Properties.Resources.AddTabMenuItem_ClickText1, tabName);
                    MessageBox.Show(tmp, Properties.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    //成功
                    SaveConfigsTabs();
                    if (tabUsage == MyCommon.TabUsageType.PublicSearch)
                    {
                        ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                        ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
                    }
                    if (tabUsage == MyCommon.TabUsageType.Lists)
                    {
                        ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                        var listTab = (ListTimelineTabModel)this._statuses.Tabs[this._curTab.Text];
                        this.GetListTimelineAsync(listTab);
                    }
                }
            }
        }

        private void TabMenuItem_Click(object sender, EventArgs e)
        {
            using (var fltDialog = new FilterDialog())
            {
                fltDialog.Owner = this;

                //選択発言を元にフィルタ追加
                foreach (int idx in _curList.SelectedIndices)
                {
                    string tabName;
                    //タブ選択（or追加）
                    if (!SelectTab(out tabName)) return;

                    fltDialog.SetCurrent(tabName);
                    if (_statuses.Tabs[_curTab.Text][idx].RetweetedId == null)
                    {
                        fltDialog.AddNewFilter(_statuses.Tabs[_curTab.Text][idx].ScreenName, _statuses.Tabs[_curTab.Text][idx].TextFromApi);
                    }
                    else
                    {
                        fltDialog.AddNewFilter(_statuses.Tabs[_curTab.Text][idx].RetweetedBy, _statuses.Tabs[_curTab.Text][idx].TextFromApi);
                    }
                    fltDialog.ShowDialog(this);
                    this.TopMost = this._cfgCommon.AlwaysTop;
                }
            }

            this.ApplyPostFilters();
            SaveConfigsTabs();
            if (this.ListTab.SelectedTab != null &&
                ((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices.Count > 0)
            {
                _curPost = _statuses.Tabs[this.ListTab.SelectedTab.Text][((DetailsListView)this.ListTab.SelectedTab.Tag).SelectedIndices[0]];
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            //TextBox1でEnterを押してもビープ音が鳴らないようにする
            if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                if (StatusText.Focused)
                {
                    bool _NewLine = false;
                    bool _Post = false;

                    if (this._cfgCommon.PostCtrlEnter) //Ctrl+Enter投稿時
                    {
                        if (StatusText.Multiline)
                        {
                            if ((keyData & Keys.Shift) == Keys.Shift && (keyData & Keys.Control) != Keys.Control) _NewLine = true;

                            if ((keyData & Keys.Control) == Keys.Control) _Post = true;
                        }
                        else
                        {
                            if (((keyData & Keys.Control) == Keys.Control)) _Post = true;
                        }

                    }
                    else if (this._cfgCommon.PostShiftEnter) //SHift+Enter投稿時
                    {
                        if (StatusText.Multiline)
                        {
                            if ((keyData & Keys.Control) == Keys.Control && (keyData & Keys.Shift) != Keys.Shift) _NewLine = true;

                            if ((keyData & Keys.Shift) == Keys.Shift) _Post = true;
                        }
                        else
                        {
                            if (((keyData & Keys.Shift) == Keys.Shift)) _Post = true;
                        }

                    }
                    else //Enter投稿時
                    {
                        if (StatusText.Multiline)
                        {
                            if ((keyData & Keys.Shift) == Keys.Shift && (keyData & Keys.Control) != Keys.Control) _NewLine = true;

                            if (((keyData & Keys.Control) != Keys.Control && (keyData & Keys.Shift) != Keys.Shift) ||
                                ((keyData & Keys.Control) == Keys.Control && (keyData & Keys.Shift) == Keys.Shift)) _Post = true;
                        }
                        else
                        {
                            if (((keyData & Keys.Shift) == Keys.Shift) ||
                                (((keyData & Keys.Control) != Keys.Control) &&
                                ((keyData & Keys.Shift) != Keys.Shift))) _Post = true;
                        }
                    }

                    if (_NewLine)
                    {
                        int pos1 = StatusText.SelectionStart;
                        if (StatusText.SelectionLength > 0)
                        {
                            StatusText.Text = StatusText.Text.Remove(pos1, StatusText.SelectionLength);  //選択状態文字列削除
                        }
                        StatusText.Text = StatusText.Text.Insert(pos1, Environment.NewLine);  //改行挿入
                        StatusText.SelectionStart = pos1 + Environment.NewLine.Length;    //カーソルを改行の次の文字へ移動
                        return true;
                    }
                    else if (_Post)
                    {
                        PostButton_Click(null, null);
                        return true;
                    }
                }
                else if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.PublicSearch &&
                         (ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focused ||
                         ListTab.SelectedTab.Controls["panelSearch"].Controls["comboLang"].Focused))
                {
                    this.SearchButton_Click(ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"], null);
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private void ReplyAllStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReplyOrDirectStatus(false, true, true);
        }

        private void IDRuleMenuItem_Click(object sender, EventArgs e)
        {
            //未選択なら処理終了
            if (_curList.SelectedIndices.Count == 0) return;

            var tab = this._statuses.Tabs[this._curTab.Text];
            var screenNameArray = this._curList.SelectedIndices.Cast<int>()
                .Select(x => tab[x])
                .Select(x => x.RetweetedId != null ? x.RetweetedBy : x.ScreenName)
                .ToArray();

            this.AddFilterRuleByScreenName(screenNameArray);

            if (screenNameArray.Length != 0)
            {
                List<string> atids = new List<string>();
                foreach (var screenName in screenNameArray)
                {
                    atids.Add("@" + screenName);
                }
                int cnt = AtIdSupl.ItemCount;
                AtIdSupl.AddRangeItem(atids.ToArray());
                if (AtIdSupl.ItemCount != cnt) ModifySettingAtId = true;
            }
        }

        private void SourceRuleMenuItem_Click(object sender, EventArgs e)
        {
            if (this._curList.SelectedIndices.Count == 0)
                return;

            var tab = this._statuses.Tabs[this._curTab.Text];
            var sourceArray = this._curList.SelectedIndices.Cast<int>()
                .Select(x => tab[x].Source).ToArray();

            this.AddFilterRuleBySource(sourceArray);
        }

        public void AddFilterRuleByScreenName(params string[] screenNameArray)
        {
            //タブ選択（or追加）
            string tabName;
            if (!SelectTab(out tabName)) return;

            var tab = (FilterTabModel)this._statuses.Tabs[tabName];

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
            SaveConfigsTabs();
        }

        public void AddFilterRuleBySource(params string[] sourceArray)
        {
            // タブ選択ダイアログを表示（or追加）
            string tabName;
            if (!this.SelectTab(out tabName))
                return;

            var filterTab = (FilterTabModel)this._statuses.Tabs[tabName];

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

        private bool SelectTab(out string tabName)
        {
            do
            {
                tabName = null;

                //振り分け先タブ選択
                using (var dialog = new TabsDialog(_statuses))
                {
                    if (dialog.ShowDialog(this) == DialogResult.Cancel) return false;

                    var selectedTab = dialog.SelectedTab;
                    tabName = selectedTab == null ? null : selectedTab.TabName;
                }

                ListTab.SelectedTab.Focus();
                //新規タブを選択→タブ作成
                if (tabName == null)
                {
                    using (InputTabName inputName = new InputTabName())
                    {
                        inputName.TabName = _statuses.MakeTabName("MyTab");
                        inputName.ShowDialog();
                        if (inputName.DialogResult == DialogResult.Cancel) return false;
                        tabName = inputName.TabName;
                    }
                    this.TopMost = this._cfgCommon.AlwaysTop;
                    if (!string.IsNullOrEmpty(tabName))
                    {
                        var tab = new FilterTabModel(tabName);
                        if (!_statuses.AddTab(tab) || !AddNewTab(tab, startup: false))
                        {
                            string tmp = string.Format(Properties.Resources.IDRuleMenuItem_ClickText2, tabName);
                            MessageBox.Show(tmp, Properties.Resources.IDRuleMenuItem_ClickText3, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //もう一度タブ名入力
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    //既存タブを選択
                    return true;
                }
            }
            while (true);
        }

        private void MoveOrCopy(out bool move, out bool mark)
        {
            {
                //移動するか？
                string _tmp = string.Format(Properties.Resources.IDRuleMenuItem_ClickText4, Environment.NewLine);
                if (MessageBox.Show(_tmp, Properties.Resources.IDRuleMenuItem_ClickText5, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    move = false;
                else
                    move = true;
            }
            if (!move)
            {
                //マークするか？
                string _tmp = string.Format(Properties.Resources.IDRuleMenuItem_ClickText6, Environment.NewLine);
                if (MessageBox.Show(_tmp, Properties.Resources.IDRuleMenuItem_ClickText7, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
        {
            this.CopyStot();
        }

        private void CopyURLMenuItem_Click(object sender, EventArgs e)
        {
            this.CopyIdUri();
        }

        private void SelectAllMenuItem_Click(object sender, EventArgs e)
        {
            if (StatusText.Focused)
            {
                // 発言欄でのCtrl+A
                StatusText.SelectAll();
            }
            else
            {
                // ListView上でのCtrl+A
                NativeMethods.SelectAllItems(this._curList);
            }
        }

        private void MoveMiddle()
        {
            ListViewItem _item;
            int idx1;
            int idx2;

            if (_curList.SelectedIndices.Count == 0) return;

            int idx = _curList.SelectedIndices[0];

            _item = _curList.GetItemAt(0, 25);
            if (_item == null)
                idx1 = 0;
            else
                idx1 = _item.Index;

            _item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);
            if (_item == null)
                idx2 = _curList.VirtualListSize - 1;
            else
                idx2 = _item.Index;

            idx -= Math.Abs(idx1 - idx2) / 2;
            if (idx < 0) idx = 0;

            _curList.EnsureVisible(_curList.VirtualListSize - 1);
            _curList.EnsureVisible(idx);
        }

        private async void OpenURLMenuItem_Click(object sender, EventArgs e)
        {
            var linkElements = this.tweetDetailsView.GetLinkElements();

            if (linkElements.Length > 0)
            {
                UrlDialog.ClearUrl();

                string openUrlStr = "";

                if (linkElements.Length == 1)
                {
                    // ツイートに含まれる URL が 1 つのみの場合
                    //   => OpenURL ダイアログを表示せずにリンクを開く

                    string urlStr = "";
                    try
                    {
                        urlStr = MyCommon.IDNEncode(linkElements[0].GetAttribute("href"));
                    }
                    catch (ArgumentException)
                    {
                        //変なHTML？
                        return;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    if (string.IsNullOrEmpty(urlStr)) return;
                    openUrlStr = MyCommon.urlEncodeMultibyteChar(urlStr);

                    // Ctrl+E で呼ばれた場合を考慮し isReverseSettings の判定を行わない
                    await this.OpenUriAsync(new Uri(openUrlStr));
                }
                else
                {
                    // ツイートに含まれる URL が複数ある場合
                    //   => OpenURL を表示しユーザーが選択したリンクを開く

                    foreach (var linkElm in linkElements)
                    {
                        string urlStr = "";
                        string linkText = "";
                        string href = "";
                        try
                        {
                            urlStr = linkElm.GetAttribute("title");
                            href = MyCommon.IDNEncode(linkElm.GetAttribute("href"));
                            if (string.IsNullOrEmpty(urlStr)) urlStr = href;
                            linkText = linkElm.InnerText;
                        }
                        catch (ArgumentException)
                        {
                            //変なHTML？
                            return;
                        }
                        catch (Exception)
                        {
                            return;
                        }
                        if (string.IsNullOrEmpty(urlStr)) continue;
                        UrlDialog.AddUrl(new OpenUrlItem(linkText, MyCommon.urlEncodeMultibyteChar(urlStr), href));
                    }
                    try
                    {
                        if (UrlDialog.ShowDialog() == DialogResult.OK)
                        {
                            openUrlStr = UrlDialog.SelectedUrl;

                            // Ctrlを押しながらリンクを開いた場合は、設定と逆の動作をするフラグを true としておく
                            await this.OpenUriAsync(new Uri(openUrlStr), MyCommon.IsKeyDown(Keys.Control));
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    this.TopMost = this._cfgCommon.AlwaysTop;
                }
            }
        }

        private void ClearTabMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_rclickTabName)) return;
            ClearTab(_rclickTabName, true);
        }

        private void ClearTab(string tabName, bool showWarning)
        {
            if (showWarning)
            {
                string tmp = string.Format(Properties.Resources.ClearTabMenuItem_ClickText1, Environment.NewLine);
                if (MessageBox.Show(tmp, tabName + " " + Properties.Resources.ClearTabMenuItem_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }

            _statuses.ClearTabIds(tabName);
            if (ListTab.SelectedTab.Text == tabName)
            {
                _anchorPost = null;
                _anchorFlag = false;
                this.PurgeListViewItemCache();
                _curItemIndex = -1;
                _curPost = null;
            }
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (tb.Text == tabName)
                {
                    tb.ImageIndex = -1;
                    ((DetailsListView)tb.Tag).VirtualListSize = 0;
                    break;
                }
            }
            if (!this._cfgCommon.TabIconDisp) ListTab.Refresh();

            SetMainWindowTitle();
            SetStatusLabelUrl();
        }

        private static long followers = 0;

        private void SetMainWindowTitle()
        {
            //メインウインドウタイトルの書き換え
            StringBuilder ttl = new StringBuilder(256);
            int ur = 0;
            int al = 0;
            if (this._cfgCommon.DispLatestPost != MyCommon.DispTitleEnum.None &&
                this._cfgCommon.DispLatestPost != MyCommon.DispTitleEnum.Post &&
                this._cfgCommon.DispLatestPost != MyCommon.DispTitleEnum.Ver &&
                this._cfgCommon.DispLatestPost != MyCommon.DispTitleEnum.OwnStatus)
            {
                foreach (var tab in _statuses.Tabs.Values)
                {
                    ur += tab.UnreadCount;
                    al += tab.AllCount;
                }
            }

            if (this._cfgCommon.DispUsername) ttl.Append(tw.Username).Append(" - ");
            ttl.Append(Application.ProductName);
            ttl.Append("  ");
            switch (this._cfgCommon.DispLatestPost)
            {
                case MyCommon.DispTitleEnum.Ver:
                    ttl.Append("Ver:").Append(MyCommon.GetReadableVersion());
                    break;
                case MyCommon.DispTitleEnum.Post:
                    if (_history != null && _history.Count > 1)
                        ttl.Append(_history[_history.Count - 2].status.Replace("\r\n", " "));
                    break;
                case MyCommon.DispTitleEnum.UnreadRepCount:
                    ttl.AppendFormat(Properties.Resources.SetMainWindowTitleText1, _statuses.GetTabByType(MyCommon.TabUsageType.Mentions).UnreadCount + _statuses.GetTabByType(MyCommon.TabUsageType.DirectMessage).UnreadCount);
                    break;
                case MyCommon.DispTitleEnum.UnreadAllCount:
                    ttl.AppendFormat(Properties.Resources.SetMainWindowTitleText2, ur);
                    break;
                case MyCommon.DispTitleEnum.UnreadAllRepCount:
                    ttl.AppendFormat(Properties.Resources.SetMainWindowTitleText3, ur, _statuses.GetTabByType(MyCommon.TabUsageType.Mentions).UnreadCount + _statuses.GetTabByType(MyCommon.TabUsageType.DirectMessage).UnreadCount);
                    break;
                case MyCommon.DispTitleEnum.UnreadCountAllCount:
                    ttl.AppendFormat(Properties.Resources.SetMainWindowTitleText4, ur, al);
                    break;
                case MyCommon.DispTitleEnum.OwnStatus:
                    if (followers == 0 && tw.FollowersCount > 0) followers = tw.FollowersCount;
                    ttl.AppendFormat(Properties.Resources.OwnStatusTitle, tw.StatusesCount, tw.FriendsCount, tw.FollowersCount, tw.FollowersCount - followers);
                    break;
            }

            try
            {
                this.Text = ttl.ToString();
            }
            catch (AccessViolationException)
            {
                //原因不明。ポスト内容に依存か？たまーに発生するが再現せず。
            }
        }

        private string GetStatusLabelText()
        {
            //ステータス欄にカウント表示
            //タブ未読数/タブ発言数 全未読数/総発言数 (未読＠＋未読DM数)
            if (_statuses == null) return "";
            TabModel tbRep = _statuses.GetTabByType(MyCommon.TabUsageType.Mentions);
            TabModel tbDm = _statuses.GetTabByType(MyCommon.TabUsageType.DirectMessage);
            if (tbRep == null || tbDm == null) return "";
            int urat = tbRep.UnreadCount + tbDm.UnreadCount;
            int ur = 0;
            int al = 0;
            int tur = 0;
            int tal = 0;
            StringBuilder slbl = new StringBuilder(256);
            try
            {
                foreach (var tab in _statuses.Tabs.Values)
                {
                    ur += tab.UnreadCount;
                    al += tab.AllCount;
                    if (_curTab != null && tab.TabName.Equals(_curTab.Text))
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

            UnreadCounter = ur;
            UnreadAtCounter = urat;

            var homeTab = this._statuses.GetTabByType<HomeTabModel>();

            slbl.AppendFormat(Properties.Resources.SetStatusLabelText1, tur, tal, ur, al, urat, _postTimestamps.Count, _favTimestamps.Count, homeTab.TweetsPerHour);
            if (this._cfgCommon.TimelinePeriod == 0)
            {
                slbl.Append(Properties.Resources.SetStatusLabelText2);
            }
            else
            {
                slbl.Append(this._cfgCommon.TimelinePeriod + Properties.Resources.SetStatusLabelText3);
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
                    var endpointName = (e as TwitterApiStatus.AccessLimitUpdatedEventArgs).EndpointName;
                    SetApiStatusLabel(endpointName);
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

        private void SetApiStatusLabel(string endpointName = null)
        {
            if (_curTab == null)
            {
                this.toolStripApiGauge.ApiEndpoint = null;
            }
            else
            {
                var tabType = _statuses.Tabs[_curTab.Text].TabType;

                if (endpointName == null)
                {
                    // 表示中のタブに応じて更新
                    switch (tabType)
                    {
                        case MyCommon.TabUsageType.Home:
                        case MyCommon.TabUsageType.UserDefined:
                            endpointName = "/statuses/home_timeline";
                            break;

                        case MyCommon.TabUsageType.Mentions:
                            endpointName = "/statuses/mentions_timeline";
                            break;

                        case MyCommon.TabUsageType.Favorites:
                            endpointName = "/favorites/list";
                            break;

                        case MyCommon.TabUsageType.DirectMessage:
                            endpointName = "/direct_messages";
                            break;

                        case MyCommon.TabUsageType.UserTimeline:
                            endpointName = "/statuses/user_timeline";
                            break;

                        case MyCommon.TabUsageType.Lists:
                            endpointName = "/lists/statuses";
                            break;

                        case MyCommon.TabUsageType.PublicSearch:
                            endpointName = "/search/tweets";
                            break;

                        case MyCommon.TabUsageType.Related:
                            endpointName = "/statuses/show/:id";
                            break;

                        default:
                            break;
                    }

                    this.toolStripApiGauge.ApiEndpoint = endpointName;
                }
                else
                {
                    // 表示中のタブに関連する endpoint であれば更新
                    var update = false;

                    switch (endpointName)
                    {
                        case "/statuses/home_timeline":
                            update = tabType == MyCommon.TabUsageType.Home ||
                                     tabType == MyCommon.TabUsageType.UserDefined;
                            break;

                        case "/statuses/mentions_timeline":
                            update = tabType == MyCommon.TabUsageType.Mentions;
                            break;

                        case "/favorites/list":
                            update = tabType == MyCommon.TabUsageType.Favorites;
                            break;

                        case "/direct_messages:":
                            update = tabType == MyCommon.TabUsageType.DirectMessage;
                            break;

                        case "/statuses/user_timeline":
                            update = tabType == MyCommon.TabUsageType.UserTimeline;
                            break;

                        case "/lists/statuses":
                            update = tabType == MyCommon.TabUsageType.Lists;
                            break;

                        case "/search/tweets":
                            update = tabType == MyCommon.TabUsageType.PublicSearch;
                            break;

                        case "/statuses/show/:id":
                            update = tabType == MyCommon.TabUsageType.Related;
                            break;

                        default:
                            break;
                    }

                    if (update)
                    {
                        this.toolStripApiGauge.ApiEndpoint = endpointName;
                    }
                }
            }
        }

        private void SetStatusLabelUrl()
        {
            StatusLabelUrl.Text = GetStatusLabelText();
        }

        public void SetStatusLabel(string text)
        {
            StatusLabel.Text = text;
        }

        private void SetNotifyIconText()
        {
            var ur = new StringBuilder(64);

            // タスクトレイアイコンのツールチップテキスト書き換え
            // Tween [未読/@]
            ur.Remove(0, ur.Length);
            if (this._cfgCommon.DispUsername)
            {
                ur.Append(tw.Username);
                ur.Append(" - ");
            }
            ur.Append(Application.ProductName);
#if DEBUG
            ur.Append("(Debug Build)");
#endif
            if (UnreadCounter != -1 && UnreadAtCounter != -1)
            {
                ur.Append(" [");
                ur.Append(UnreadCounter);
                ur.Append("/@");
                ur.Append(UnreadAtCounter);
                ur.Append("]");
            }
            NotifyIcon1.Text = ur.ToString();
        }

        internal void CheckReplyTo(string StatusText)
        {
            MatchCollection m;
            //ハッシュタグの保存
            m = Regex.Matches(StatusText, Twitter.HASHTAG, RegexOptions.IgnoreCase);
            string hstr = "";
            foreach (Match hm in m)
            {
                if (!hstr.Contains("#" + hm.Result("$3") + " "))
                {
                    hstr += "#" + hm.Result("$3") + " ";
                    HashSupl.AddItem("#" + hm.Result("$3"));
                }
            }
            if (!string.IsNullOrEmpty(HashMgr.UseHash) && !hstr.Contains(HashMgr.UseHash + " "))
            {
                hstr += HashMgr.UseHash;
            }
            if (!string.IsNullOrEmpty(hstr)) HashMgr.AddHashToHistory(hstr.Trim(), false);

            // 本当にリプライ先指定すべきかどうかの判定
            m = Regex.Matches(StatusText, "(^|[ -/:-@[-^`{-~])(?<id>@[a-zA-Z0-9_]+)");

            if (this._cfgCommon.UseAtIdSupplement)
            {
                int bCnt = AtIdSupl.ItemCount;
                foreach (Match mid in m)
                {
                    AtIdSupl.AddItem(mid.Result("${id}"));
                }
                if (bCnt != AtIdSupl.ItemCount) ModifySettingAtId = true;
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
                var inReplyToScreenName = this.inReplyTo.Item2;
                if (StatusText.StartsWith("@", StringComparison.Ordinal))
                {
                    if (StatusText.StartsWith("@" + inReplyToScreenName, StringComparison.Ordinal)) return;
                }
                else
                {
                    foreach (Match mid in m)
                    {
                        if (StatusText.Contains("RT " + mid.Result("${id}") + ":") && mid.Result("${id}") == "@" + inReplyToScreenName) return;
                    }
                }
            }

            this.inReplyTo = null;
        }

        private void TweenMain_Resize(object sender, EventArgs e)
        {
            if (!_initialLayout && this._cfgCommon.MinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }
            if (_initialLayout && _cfgLocal != null && this.WindowState == FormWindowState.Normal && this.Visible)
            {
                // 現在の DPI と設定保存時の DPI との比を取得する
                var configScaleFactor = this._cfgLocal.GetConfigScaleFactor(this.CurrentAutoScaleDimensions);

                this.ClientSize = ScaleBy(configScaleFactor, _cfgLocal.FormSize);
                //_mySize = this.ClientSize;                     //サイズ保持（最小化・最大化されたまま終了した場合の対応用）
                this.DesktopLocation = _cfgLocal.FormLocation;
                //_myLoc = this.DesktopLocation;                        //位置保持（最小化・最大化されたまま終了した場合の対応用）

                // Splitterの位置設定
                var splitterDistance = ScaleBy(configScaleFactor.Height, _cfgLocal.SplitterDistance);
                if (splitterDistance > this.SplitContainer1.Panel1MinSize &&
                    splitterDistance < this.SplitContainer1.Height - this.SplitContainer1.Panel2MinSize - this.SplitContainer1.SplitterWidth)
                {
                    this.SplitContainer1.SplitterDistance = splitterDistance;
                }

                //発言欄複数行
                StatusText.Multiline = _cfgLocal.StatusMultiline;
                if (StatusText.Multiline)
                {
                    var statusTextHeight = ScaleBy(configScaleFactor.Height, _cfgLocal.StatusTextHeight);
                    int dis = SplitContainer2.Height - statusTextHeight - SplitContainer2.SplitterWidth;
                    if (dis > SplitContainer2.Panel1MinSize && dis < SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth)
                    {
                        SplitContainer2.SplitterDistance = SplitContainer2.Height - statusTextHeight - SplitContainer2.SplitterWidth;
                    }
                    StatusText.Height = statusTextHeight;
                }
                else
                {
                    if (SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth > 0)
                    {
                        SplitContainer2.SplitterDistance = SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth;
                    }
                }

                var previewDistance = ScaleBy(configScaleFactor.Width, _cfgLocal.PreviewDistance);
                if (previewDistance > this.SplitContainer3.Panel1MinSize && previewDistance < this.SplitContainer3.Width - this.SplitContainer3.Panel2MinSize - this.SplitContainer3.SplitterWidth)
                {
                    this.SplitContainer3.SplitterDistance = previewDistance;
                }

                // Panel2Collapsed は SplitterDistance の設定を終えるまで true にしない
                this.SplitContainer3.Panel2Collapsed = true;

                _initialLayout = false;
            }
            if (this.WindowState != FormWindowState.Minimized)
            {
                _formWindowState = this.WindowState;
            }
        }

        private void PlaySoundMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            PlaySoundMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.PlaySoundFileMenuItem.Checked = PlaySoundMenuItem.Checked;
            if (PlaySoundMenuItem.Checked)
            {
                this._cfgCommon.PlaySound = true;
            }
            else
            {
                this._cfgCommon.PlaySound = false;
            }
            ModifySettingCommon = true;
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this._initialLayout)
                return;

            int splitterDistance;
            switch (this.WindowState)
            {
                case FormWindowState.Normal:
                    splitterDistance = this.SplitContainer1.SplitterDistance;
                    break;
                case FormWindowState.Maximized:
                    // 最大化時は、通常時のウィンドウサイズに換算した SplitterDistance を算出する
                    var normalContainerHeight = this._mySize.Height - this.ToolStripContainer1.TopToolStripPanel.Height - this.ToolStripContainer1.BottomToolStripPanel.Height;
                    splitterDistance = this.SplitContainer1.SplitterDistance - (this.SplitContainer1.Height - normalContainerHeight);
                    splitterDistance = Math.Min(splitterDistance, normalContainerHeight - this.SplitContainer1.SplitterWidth - this.SplitContainer1.Panel2MinSize);
                    break;
                default:
                    return;
            }

            this._mySpDis = splitterDistance;
            this.ModifySettingLocal = true;
        }

        private async Task doRepliedStatusOpen()
        {
            if (this.ExistCurrentPost && _curPost.InReplyToUser != null && _curPost.InReplyToStatusId != null)
            {
                if (MyCommon.IsKeyDown(Keys.Shift))
                {
                    await this.OpenUriInBrowserAsync(MyCommon.GetStatusUrl(_curPost.InReplyToUser, _curPost.InReplyToStatusId.Value));
                    return;
                }
                if (_statuses.ContainsKey(_curPost.InReplyToStatusId.Value))
                {
                    PostClass repPost = _statuses[_curPost.InReplyToStatusId.Value];
                    MessageBox.Show($"{repPost.ScreenName} / {repPost.Nickname}   ({repPost.CreatedAt})" + Environment.NewLine + repPost.TextFromApi);
                }
                else
                {
                    foreach (TabModel tb in _statuses.GetTabsByType(MyCommon.TabUsageType.Lists | MyCommon.TabUsageType.PublicSearch))
                    {
                        if (tb == null || !tb.Contains(_curPost.InReplyToStatusId.Value)) break;
                        PostClass repPost = _statuses[_curPost.InReplyToStatusId.Value];
                        MessageBox.Show($"{repPost.ScreenName} / {repPost.Nickname}   ({repPost.CreatedAt})" + Environment.NewLine + repPost.TextFromApi);
                        return;
                    }
                    await this.OpenUriInBrowserAsync(MyCommon.GetStatusUrl(_curPost.InReplyToUser, _curPost.InReplyToStatusId.Value));
                }
            }
        }

        private async void RepliedStatusOpenMenuItem_Click(object sender, EventArgs e)
        {
            await this.doRepliedStatusOpen();
        }

        private void SplitContainer2_Panel2_Resize(object sender, EventArgs e)
        {
            var multiline = this.SplitContainer2.Panel2.Height > this.SplitContainer2.Panel2MinSize + 2;
            if (multiline != this.StatusText.Multiline)
            {
                this.StatusText.Multiline = multiline;
                MultiLineMenuItem.Checked = multiline;
                ModifySettingLocal = true;
            }
        }

        private void StatusText_MultilineChanged(object sender, EventArgs e)
        {
            if (this.StatusText.Multiline)
                this.StatusText.ScrollBars = ScrollBars.Vertical;
            else
                this.StatusText.ScrollBars = ScrollBars.None;

            ModifySettingLocal = true;
        }

        private void MultiLineMenuItem_Click(object sender, EventArgs e)
        {
            //発言欄複数行
            StatusText.Multiline = MultiLineMenuItem.Checked;
            _cfgLocal.StatusMultiline = MultiLineMenuItem.Checked;
            if (MultiLineMenuItem.Checked)
            {
                if (SplitContainer2.Height - _mySpDis2 - SplitContainer2.SplitterWidth < 0)
                    SplitContainer2.SplitterDistance = 0;
                else
                    SplitContainer2.SplitterDistance = SplitContainer2.Height - _mySpDis2 - SplitContainer2.SplitterWidth;
            }
            else
            {
                SplitContainer2.SplitterDistance = SplitContainer2.Height - SplitContainer2.Panel2MinSize - SplitContainer2.SplitterWidth;
            }
            ModifySettingLocal = true;
        }

        private async Task<bool> UrlConvertAsync(MyCommon.UrlConverter Converter_Type)
        {
            //t.coで投稿時自動短縮する場合は、外部サービスでの短縮禁止
            //if (SettingDialog.UrlConvertAuto && SettingDialog.ShortenTco) return;

            //Converter_Type=Nicomsの場合は、nicovideoのみ短縮する
            //参考資料 RFC3986 Uniform Resource Identifier (URI): Generic Syntax
            //Appendix A.  Collected ABNF for URI
            //http://www.ietf.org/rfc/rfc3986.txt

            string result = "";

            const string nico = @"^https?://[a-z]+\.(nicovideo|niconicommons|nicolive)\.jp/[a-z]+/[a-z0-9]+$";

            if (StatusText.SelectionLength > 0)
            {
                string tmp = StatusText.SelectedText;
                // httpから始まらない場合、ExcludeStringで指定された文字列で始まる場合は対象としない
                if (tmp.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    // 文字列が選択されている場合はその文字列について処理

                    //nico.ms使用、nicovideoにマッチしたら変換
                    if (this._cfgCommon.Nicoms && Regex.IsMatch(tmp, nico))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (Converter_Type != MyCommon.UrlConverter.Nicoms)
                    {
                        //短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                        try
                        {
                            var srcUri = new Uri(MyCommon.urlEncodeMultibyteChar(tmp));
                            var resultUri = await ShortUrl.Instance.ShortenUrlAsync(Converter_Type, srcUri);
                            result = resultUri.AbsoluteUri;
                        }
                        catch (WebApiException e)
                        {
                            this.StatusLabel.Text = Converter_Type + ":" + e.Message;
                            return false;
                        }
                        catch (UriFormatException e)
                        {
                            this.StatusLabel.Text = Converter_Type + ":" + e.Message;
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        urlUndo undotmp = new urlUndo();

                        StatusText.Select(StatusText.Text.IndexOf(tmp, StringComparison.Ordinal), tmp.Length);
                        StatusText.SelectedText = result;

                        //undoバッファにセット
                        undotmp.Before = tmp;
                        undotmp.After = result;

                        if (urlUndoBuffer == null)
                        {
                            urlUndoBuffer = new List<urlUndo>();
                            UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        urlUndoBuffer.Add(undotmp);
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
                foreach (Match mt in Regex.Matches(StatusText.Text, url, RegexOptions.IgnoreCase))
                {
                    if (StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal) == -1) continue;
                    string tmp = mt.Result("${url}");
                    if (tmp.StartsWith("w", StringComparison.OrdinalIgnoreCase)) tmp = "http://" + tmp;
                    urlUndo undotmp = new urlUndo();

                    //選んだURLを選択（？）
                    StatusText.Select(StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length);

                    //nico.ms使用、nicovideoにマッチしたら変換
                    if (this._cfgCommon.Nicoms && Regex.IsMatch(tmp, nico))
                    {
                        result = nicoms.Shorten(tmp);
                    }
                    else if (Converter_Type != MyCommon.UrlConverter.Nicoms)
                    {
                        //短縮URL変換 日本語を含むかもしれないのでURLエンコードする
                        try
                        {
                            var srcUri = new Uri(MyCommon.urlEncodeMultibyteChar(tmp));
                            var resultUri = await ShortUrl.Instance.ShortenUrlAsync(Converter_Type, srcUri);
                            result = resultUri.AbsoluteUri;
                        }
                        catch (HttpRequestException e)
                        {
                            // 例外のメッセージが「Response status code does not indicate success: 500 (Internal Server Error).」
                            // のように長いので「:」が含まれていればそれ以降のみを抽出する
                            var message = e.Message.Split(new[] { ':' }, count: 2).Last();

                            this.StatusLabel.Text = Converter_Type + ":" + message;
                            continue;
                        }
                        catch (WebApiException e)
                        {
                            this.StatusLabel.Text = Converter_Type + ":" + e.Message;
                            continue;
                        }
                        catch (UriFormatException e)
                        {
                            this.StatusLabel.Text = Converter_Type + ":" + e.Message;
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        StatusText.Select(StatusText.Text.IndexOf(mt.Result("${url}"), StringComparison.Ordinal), mt.Result("${url}").Length);
                        StatusText.SelectedText = result;
                        //undoバッファにセット
                        undotmp.Before = mt.Result("${url}");
                        undotmp.After = result;

                        if (urlUndoBuffer == null)
                        {
                            urlUndoBuffer = new List<urlUndo>();
                            UrlUndoToolStripMenuItem.Enabled = true;
                        }

                        urlUndoBuffer.Add(undotmp);
                    }
                }
            }

            return true;
        }

        private void doUrlUndo()
        {
            if (urlUndoBuffer != null)
            {
                string tmp = StatusText.Text;
                foreach (urlUndo data in urlUndoBuffer)
                {
                    tmp = tmp.Replace(data.After, data.Before);
                }
                StatusText.Text = tmp;
                urlUndoBuffer = null;
                UrlUndoToolStripMenuItem.Enabled = false;
                StatusText.SelectionStart = 0;
                StatusText.SelectionLength = 0;
            }
        }

        private async void TinyURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await UrlConvertAsync(MyCommon.UrlConverter.TinyUrl);
        }

        private async void IsgdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await UrlConvertAsync(MyCommon.UrlConverter.Isgd);
        }

        private async void TwurlnlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await UrlConvertAsync(MyCommon.UrlConverter.Twurl);
        }

        private async void UxnuMenuItem_Click(object sender, EventArgs e)
        {
            await UrlConvertAsync(MyCommon.UrlConverter.Uxnu);
        }

        private async void UrlConvertAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!await UrlConvertAsync(this._cfgCommon.AutoShortUrlFirst))
            {
                MyCommon.UrlConverter svc = this._cfgCommon.AutoShortUrlFirst;
                Random rnd = new Random();
                // 前回使用した短縮URLサービス以外を選択する
                do
                {
                    svc = (MyCommon.UrlConverter)rnd.Next(System.Enum.GetNames(typeof(MyCommon.UrlConverter)).Length);
                }
                while (svc == this._cfgCommon.AutoShortUrlFirst || svc == MyCommon.UrlConverter.Nicoms || svc == MyCommon.UrlConverter.Unu);
                await UrlConvertAsync(svc);
            }
        }

        private void UrlUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doUrlUndo();
        }

        private void NewPostPopMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.NotifyFileMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.NewPostPopMenuItem.Checked = this.NotifyFileMenuItem.Checked;
            _cfgCommon.NewAllPop = NewPostPopMenuItem.Checked;
            ModifySettingCommon = true;
        }

        private void ListLockMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            ListLockMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.LockListFileMenuItem.Checked = ListLockMenuItem.Checked;
            _cfgCommon.ListLock = ListLockMenuItem.Checked;
            ModifySettingCommon = true;
        }

        private void MenuStrip1_MenuActivate(object sender, EventArgs e)
        {
            // フォーカスがメニューに移る (MenuStrip1.Tag フラグを立てる)
            MenuStrip1.Tag = new Object();
            MenuStrip1.Select(); // StatusText がフォーカスを持っている場合 Leave が発生
        }

        private void MenuStrip1_MenuDeactivate(object sender, EventArgs e)
        {
            if (this.Tag != null) // 設定された戻り先へ遷移
            {
                if (this.Tag == this.ListTab.SelectedTab)
                    ((Control)this.ListTab.SelectedTab.Tag).Select();
                else
                    ((Control)this.Tag).Select();
            }
            else // 戻り先が指定されていない (初期状態) 場合はタブに遷移
            {
                if (ListTab.SelectedIndex > -1 && ListTab.SelectedTab.HasChildren)
                {
                    this.Tag = ListTab.SelectedTab.Tag;
                    ((Control)this.Tag).Select();
                }
            }
            // フォーカスがメニューに遷移したかどうかを表すフラグを降ろす
            MenuStrip1.Tag = null;
        }

        private void MyList_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            DetailsListView lst = (DetailsListView)sender;
            if (_cfgLocal == null) return;

            if (_iconCol)
            {
                _cfgLocal.Width1 = lst.Columns[0].Width;
                _cfgLocal.Width3 = lst.Columns[1].Width;
            }
            else
            {
                int[] darr = new int[lst.Columns.Count];
                for (int i = 0; i < lst.Columns.Count; i++)
                {
                    darr[lst.Columns[i].DisplayIndex] = i;
                }
                MyCommon.MoveArrayItem(darr, e.OldDisplayIndex, e.NewDisplayIndex);

                for (int i = 0; i < lst.Columns.Count; i++)
                {
                    switch (darr[i])
                    {
                        case 0:
                            _cfgLocal.DisplayIndex1 = i;
                            break;
                        case 1:
                            _cfgLocal.DisplayIndex2 = i;
                            break;
                        case 2:
                            _cfgLocal.DisplayIndex3 = i;
                            break;
                        case 3:
                            _cfgLocal.DisplayIndex4 = i;
                            break;
                        case 4:
                            _cfgLocal.DisplayIndex5 = i;
                            break;
                        case 5:
                            _cfgLocal.DisplayIndex6 = i;
                            break;
                        case 6:
                            _cfgLocal.DisplayIndex7 = i;
                            break;
                        case 7:
                            _cfgLocal.DisplayIndex8 = i;
                            break;
                    }
                }
                _cfgLocal.Width1 = lst.Columns[0].Width;
                _cfgLocal.Width2 = lst.Columns[1].Width;
                _cfgLocal.Width3 = lst.Columns[2].Width;
                _cfgLocal.Width4 = lst.Columns[3].Width;
                _cfgLocal.Width5 = lst.Columns[4].Width;
                _cfgLocal.Width6 = lst.Columns[5].Width;
                _cfgLocal.Width7 = lst.Columns[6].Width;
                _cfgLocal.Width8 = lst.Columns[7].Width;
            }
            ModifySettingLocal = true;
            _isColumnChanged = true;
        }

        private void MyList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            DetailsListView lst = (DetailsListView)sender;
            if (_cfgLocal == null) return;
            if (_iconCol)
            {
                if (_cfgLocal.Width1 != lst.Columns[0].Width)
                {
                    _cfgLocal.Width1 = lst.Columns[0].Width;
                    ModifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width3 != lst.Columns[1].Width)
                {
                    _cfgLocal.Width3 = lst.Columns[1].Width;
                    ModifySettingLocal = true;
                    _isColumnChanged = true;
                }
            }
            else
            {
                if (_cfgLocal.Width1 != lst.Columns[0].Width)
                {
                    _cfgLocal.Width1 = lst.Columns[0].Width;
                    ModifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width2 != lst.Columns[1].Width)
                {
                    _cfgLocal.Width2 = lst.Columns[1].Width;
                    ModifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width3 != lst.Columns[2].Width)
                {
                    _cfgLocal.Width3 = lst.Columns[2].Width;
                    ModifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width4 != lst.Columns[3].Width)
                {
                    _cfgLocal.Width4 = lst.Columns[3].Width;
                    ModifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width5 != lst.Columns[4].Width)
                {
                    _cfgLocal.Width5 = lst.Columns[4].Width;
                    ModifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width6 != lst.Columns[5].Width)
                {
                    _cfgLocal.Width6 = lst.Columns[5].Width;
                    ModifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width7 != lst.Columns[6].Width)
                {
                    _cfgLocal.Width7 = lst.Columns[6].Width;
                    ModifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width8 != lst.Columns[7].Width)
                {
                    _cfgLocal.Width8 = lst.Columns[7].Width;
                    ModifySettingLocal = true;
                    _isColumnChanged = true;
                }
            }
            // 非表示の時にColumnChangedが呼ばれた場合はForm初期化処理中なので保存しない
            //if (changed)
            //{
            //    SaveConfigsLocal();
            //}
        }

        private void SplitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (StatusText.Multiline) _mySpDis2 = StatusText.Height;
            ModifySettingLocal = true;
        }

        private void TweenMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (!e.Data.GetDataPresent(DataFormats.Html, false))  // WebBrowserコントロールからの絵文字画像Drag&Dropは弾く
                {
                    SelectMedia_DragDrop(e);
                }
            }
            else if (e.Data.GetDataPresent("UniformResourceLocatorW"))
            {
                var url = GetUrlFromDataObject(e.Data);

                string appendText;
                if (url.Item2 == null)
                    appendText = url.Item1;
                else
                    appendText = url.Item2 + " " + url.Item1;

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
                string data = (string)e.Data.GetData(DataFormats.StringFormat, true);
                if (data != null) StatusText.Text += data;
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
        internal static Tuple<string, string> GetUrlFromDataObject(IDataObject data)
        {
            if (data.GetDataPresent("text/x-moz-url"))
            {
                // Firefox, Google Chrome で利用可能
                // 参照: https://developer.mozilla.org/ja/docs/DragDrop/Recommended_Drag_Types

                using (var stream = (MemoryStream)data.GetData("text/x-moz-url"))
                {
                    var lines = Encoding.Unicode.GetString(stream.ToArray()).TrimEnd('\0').Split('\n');
                    if (lines.Length < 2)
                        throw new ArgumentException("不正な text/x-moz-url フォーマットです", nameof(data));

                    return new Tuple<string, string>(lines[0], lines[1]);
                }
            }
            else if (data.GetDataPresent("IESiteModeToUrl"))
            {
                // Internet Exproler 用
                // 保護モードが有効なデフォルトの IE では DragDrop イベントが発火しないため使えない

                using (var stream = (MemoryStream)data.GetData("IESiteModeToUrl"))
                {
                    var lines = Encoding.Unicode.GetString(stream.ToArray()).TrimEnd('\0').Split('\0');
                    if (lines.Length < 2)
                        throw new ArgumentException("不正な IESiteModeToUrl フォーマットです", nameof(data));

                    return new Tuple<string, string>(lines[0], lines[1]);
                }
            }
            else if (data.GetDataPresent("UniformResourceLocatorW"))
            {
                // それ以外のブラウザ向け

                using (var stream = (MemoryStream)data.GetData("UniformResourceLocatorW"))
                {
                    var url = Encoding.Unicode.GetString(stream.ToArray()).TrimEnd('\0');
                    return new Tuple<string, string>(url, null);
                }
            }

            throw new NotSupportedException("サポートされていないデータ形式です: " + data.GetFormats()[0]);
        }

        private void TweenMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (!e.Data.GetDataPresent(DataFormats.Html, false))  // WebBrowserコントロールからの絵文字画像Drag&Dropは弾く
                {
                    SelectMedia_DragEnter(e);
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
            bool nw = true;
            nw = MyCommon.IsNetworkAvailable();
            _myStatusOnline = nw;
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
            if( this._cfgCommon.OpenUserTimeline && !isReverseSettings ||
                !this._cfgCommon.OpenUserTimeline && isReverseSettings )
            {
                var userUriMatch = Regex.Match(uriStr, "^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)$");
                if (userUriMatch.Success)
                {
                    var screenName = userUriMatch.Groups["ScreenName"].Value;
                    if (this.IsTwitterId(screenName))
                    {
                        this.AddNewTabForUserTimeline(screenName);
                        return;
                    }
                }
            }

            // どのパターンにも該当しないURL
            await this.OpenUriInBrowserAsync(uriStr);
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
                var statusId = long.Parse(match.Groups[1].Value);
                await this.OpenRelatedTab(statusId);
                return;
            }
        }

        public Task OpenUriInBrowserAsync(string UriString)
        {
            return Task.Run(() =>
            {
                string myPath = UriString;

                try
                {
                    var configBrowserPath = this._cfgLocal.BrowserPath;
                    if (!string.IsNullOrEmpty(configBrowserPath))
                    {
                        if (configBrowserPath.StartsWith("\"", StringComparison.Ordinal) && configBrowserPath.Length > 2 && configBrowserPath.IndexOf("\"", 2, StringComparison.Ordinal) > -1)
                        {
                            int sep = configBrowserPath.IndexOf("\"", 2, StringComparison.Ordinal);
                            string browserPath = configBrowserPath.Substring(1, sep - 1);
                            string arg = "";
                            if (sep < configBrowserPath.Length - 1)
                            {
                                arg = configBrowserPath.Substring(sep + 1);
                            }
                            myPath = arg + " " + myPath;
                            System.Diagnostics.Process.Start(browserPath, myPath);
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(configBrowserPath, myPath);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Process.Start(myPath);
                    }
                }
                catch (Exception)
                {
                    //MessageBox.Show("ブラウザの起動に失敗、またはタイムアウトしました。" + ex.ToString());
                }
            });
        }

        private void ListTabSelect(TabPage _tab)
        {
            SetListProperty();

            this.PurgeListViewItemCache();

            _curTab = _tab;
            _curList = (DetailsListView)_tab.Tag;
            if (_curList.SelectedIndices.Count > 0)
            {
                _curItemIndex = _curList.SelectedIndices[0];
                _curPost = GetCurTabPost(_curItemIndex);
            }
            else
            {
                _curItemIndex = -1;
                _curPost = null;
            }

            _anchorPost = null;
            _anchorFlag = false;

            if (_iconCol)
            {
                ((DetailsListView)_tab.Tag).Columns[1].Text = ColumnText[2];
            }
            else
            {
                for (int i = 0; i < _curList.Columns.Count; i++)
                {
                    ((DetailsListView)_tab.Tag).Columns[i].Text = ColumnText[i];
                }
            }
        }

        private void ListTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            ListTabSelect(e.TabPage);
        }

        private void SelectListItem(DetailsListView LView, int Index)
        {
            //単一
            Rectangle bnd = new Rectangle();
            bool flg = false;
            var item = LView.FocusedItem;
            if (item != null)
            {
                bnd = item.Bounds;
                flg = true;
            }

            do
            {
                LView.SelectedIndices.Clear();
            }
            while (LView.SelectedIndices.Count > 0);
            item = LView.Items[Index];
            item.Selected = true;
            item.Focused = true;

            if (flg) LView.Invalidate(bnd);
        }

        private void SelectListItem(DetailsListView LView , int[] Index, int focusedIndex, int selectionMarkIndex)
        {
            //複数
            Rectangle bnd = new Rectangle();
            bool flg = false;
            var item = LView.FocusedItem;
            if (item != null)
            {
                bnd = item.Bounds;
                flg = true;
            }

            if (Index != null)
            {
                do
                {
                    LView.SelectedIndices.Clear();
                }
                while (LView.SelectedIndices.Count > 0);
                LView.SelectItems(Index);
            }
            if (selectionMarkIndex > -1 && LView.VirtualListSize > selectionMarkIndex)
            {
                LView.SelectionMark = selectionMarkIndex;
            }
            if (focusedIndex > -1 && LView.VirtualListSize > focusedIndex)
            {
                LView.Items[focusedIndex].Focused = true;
            }
            else if (Index != null && Index.Length != 0)
            {
                LView.Items[Index.Last()].Focused = true;
            }

            if (flg) LView.Invalidate(bnd);
        }

        private void StartUserStream()
        {
            tw.NewPostFromStream += tw_NewPostFromStream;
            tw.UserStreamStarted += tw_UserStreamStarted;
            tw.UserStreamStopped += tw_UserStreamStopped;
            tw.PostDeleted += tw_PostDeleted;
            tw.UserStreamEventReceived += tw_UserStreamEventArrived;

            this.RefreshUserStreamsMenu();

            if (this._cfgCommon.UserstreamStartup)
                tw.StartUserStream();
        }

        private async void TweenMain_Shown(object sender, EventArgs e)
        {
            NotifyIcon1.Visible = true;

            if (this.IsNetworkAvailable())
            {
                StartUserStream();

                var loadTasks = new List<Task>
                {
                    this.RefreshMuteUserIdsAsync(),
                    this.RefreshBlockIdsAsync(),
                    this.RefreshNoRetweetIdsAsync(),
                    this.RefreshTwitterConfigurationAsync(),
                    this.GetHomeTimelineAsync(),
                    this.GetReplyAsync(),
                    this.GetDirectMessagesAsync(),
                    this.GetPublicSearchAllAsync(),
                    this.GetUserTimelineAllAsync(),
                    this.GetListTimelineAllAsync(),
                };

                if (this._cfgCommon.StartupFollowers)
                    loadTasks.Add(this.RefreshFollowerIdsAsync());

                if (this._cfgCommon.GetFav)
                    loadTasks.Add(this.GetFavoritesAsync());

                var allTasks = Task.WhenAll(loadTasks);

                var i = 0;
                while (true)
                {
                    var timeout = Task.Delay(5000);
                    if (await Task.WhenAny(allTasks, timeout) != timeout)
                        break;

                    i += 1;
                    if (i > 24) break; // 120秒間初期処理が終了しなかったら強制的に打ち切る

                    if (MyCommon._endingFlag)
                        return;
                }

                if (MyCommon._endingFlag) return;

                if (ApplicationSettings.VersionInfoUrl != null)
                {
                    //バージョンチェック（引数：起動時チェックの場合はtrue･･･チェック結果のメッセージを表示しない）
                    if (this._cfgCommon.StartupVersion)
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
                    SettingStripMenuItem_Click(null, null);
                }

                // 取得失敗の場合は再試行する
                var reloadTasks = new List<Task>();

                if (!tw.GetFollowersSuccess && this._cfgCommon.StartupFollowers)
                    reloadTasks.Add(this.RefreshFollowerIdsAsync());

                if (!tw.GetNoRetweetSuccess)
                    reloadTasks.Add(this.RefreshNoRetweetIdsAsync());

                if (this.tw.Configuration.PhotoSizeLimit == 0)
                    reloadTasks.Add(this.RefreshTwitterConfigurationAsync());

                await Task.WhenAll(reloadTasks);
            }

            _initial = false;

            TimerTimeline.Enabled = true;
        }

        private async Task doGetFollowersMenu()
        {
            await this.RefreshFollowerIdsAsync();
            await this.DispSelectedPost(true);
        }

        private async void GetFollowersAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await this.doGetFollowersMenu();
        }

        private void ReTweetUnofficialStripMenuItem_Click(object sender, EventArgs e)
        {
            doReTweetUnofficial();
        }

        private async Task doReTweetOfficial(bool isConfirm)
        {
            //公式RT
            if (this.ExistCurrentPost)
            {
                if (_curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    _DoFavRetweetFlags = false;
                    return;
                }
                if (_curList.SelectedIndices.Count > 15)
                {
                    MessageBox.Show(Properties.Resources.RetweetLimitText);
                    _DoFavRetweetFlags = false;
                    return;
                }
                else if (_curList.SelectedIndices.Count > 1)
                {
                    string QuestionText = Properties.Resources.RetweetQuestion2;
                    if (_DoFavRetweetFlags) QuestionText = Properties.Resources.FavoriteRetweetQuestionText1;
                    switch (MessageBox.Show(QuestionText, "Retweet", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Cancel:
                        case DialogResult.No:
                            _DoFavRetweetFlags = false;
                            return;
                    }
                }
                else
                {
                    if (_curPost.IsDm || _curPost.IsMe)
                    {
                        _DoFavRetweetFlags = false;
                        return;
                    }
                    if (!this._cfgCommon.RetweetNoConfirm)
                    {
                        string Questiontext = Properties.Resources.RetweetQuestion1;
                        if (_DoFavRetweetFlags) Questiontext = Properties.Resources.FavoritesRetweetQuestionText2;
                        if (isConfirm && MessageBox.Show(Questiontext, "Retweet", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        {
                            _DoFavRetweetFlags = false;
                            return;
                        }
                    }
                }

                var statusIds = new List<long>();
                foreach (int idx in _curList.SelectedIndices)
                {
                    PostClass post = GetCurTabPost(idx);
                    if (!post.IsMe && !post.IsProtect && !post.IsDm)
                        statusIds.Add(post.StatusId);
                }

                await this.RetweetAsync(statusIds);
            }
        }

        private async void ReTweetStripMenuItem_Click(object sender, EventArgs e)
        {
            await this.doReTweetOfficial(true);
        }

        private async Task FavoritesRetweetOfficial()
        {
            if (!this.ExistCurrentPost) return;
            _DoFavRetweetFlags = true;
            var retweetTask = this.doReTweetOfficial(true);
            if (_DoFavRetweetFlags)
            {
                _DoFavRetweetFlags = false;
                var favoriteTask = this.FavoriteChange(true, false);

                await Task.WhenAll(retweetTask, favoriteTask);
            }
            else
            {
                await retweetTask;
            }
        }

        private async Task FavoritesRetweetUnofficial()
        {
            if (this.ExistCurrentPost && !_curPost.IsDm)
            {
                _DoFavRetweetFlags = true;
                var favoriteTask = this.FavoriteChange(true);
                if (!_curPost.IsProtect && _DoFavRetweetFlags)
                {
                    _DoFavRetweetFlags = false;
                    doReTweetUnofficial();
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
            statusHtml = Regex.Replace(statusHtml, "<a href=\"(?<href>.+?)\" title=\"(?<title>.+?)\">(?<text>.+?)</a>", "${title}");
            // メンション
            statusHtml = Regex.Replace(statusHtml, "<a class=\"mention\" href=\"(?<href>.+?)\">(?<text>.+?)</a>", "${text}");
            // ハッシュタグ
            statusHtml = Regex.Replace(statusHtml, "<a class=\"hashtag\" href=\"(?<href>.+?)\">(?<text>.+?)</a>", "${text}");

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

        private async void DumpPostClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tweetDetailsView.DumpPostClass = this.DumpPostClassToolStripMenuItem.Checked;

            if (_curPost != null)
                await this.DispSelectedPost(true);
        }

        private void MenuItemHelp_DropDownOpening(object sender, EventArgs e)
        {
            if (MyCommon.DebugBuild || MyCommon.IsKeyDown(Keys.CapsLock, Keys.Control, Keys.Shift))
                DebugModeToolStripMenuItem.Visible = true;
            else
                DebugModeToolStripMenuItem.Visible = false;
        }

        private void ToolStripMenuItemUrlAutoShorten_CheckedChanged(object sender, EventArgs e)
        {
            this._cfgCommon.UrlConvertAuto = ToolStripMenuItemUrlAutoShorten.Checked;
        }

        private void ContextMenuPostMode_Opening(object sender, CancelEventArgs e)
        {
            ToolStripMenuItemUrlAutoShorten.Checked = this._cfgCommon.UrlConvertAuto;
        }

        private void TraceOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TraceOutToolStripMenuItem.Checked)
                MyCommon.TraceFlag = true;
            else
                MyCommon.TraceFlag = false;
        }

        private void TweenMain_Deactivate(object sender, EventArgs e)
        {
            //画面が非アクティブになったら、発言欄の背景色をデフォルトへ
            this.StatusText_Leave(StatusText, System.EventArgs.Empty);
        }

        private void TabRenameMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_rclickTabName)) return;
            TabRename(ref _rclickTabName);
        }

        private async void BitlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await UrlConvertAsync(MyCommon.UrlConverter.Bitly);
        }

        private async void JmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await UrlConvertAsync(MyCommon.UrlConverter.Jmp);
        }

        private async void ApiUsageInfoMenuItem_Click(object sender, EventArgs e)
        {
            TwitterApiStatus apiStatus;

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

            using (var apiDlg = new ApiInfoDialog())
            {
                apiDlg.ShowDialog(this);
            }
        }

        private async void FollowCommandMenuItem_Click(object sender, EventArgs e)
        {
            var id = _curPost?.ScreenName ?? "";

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
                    var task = this.twitterApi.FriendshipsCreate(id);
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
            var id = _curPost?.ScreenName ?? "";

            await this.RemoveCommand(id, false);
        }

        internal async Task RemoveCommand(string id, bool skipInput)
        {
            if (!skipInput)
            {
                using (var inputName = new InputTabName())
                {
                    inputName.FormTitle = "Unfollow";
                    inputName.FormDescription = Properties.Resources.FRMessage1;
                    inputName.TabName = id;

                    if (inputName.ShowDialog(this) != DialogResult.OK)
                        return;
                    if (string.IsNullOrWhiteSpace(inputName.TabName))
                        return;

                    id = inputName.TabName.Trim();
                }
            }

            using (var dialog = new WaitingDialog(Properties.Resources.RemoveCommandText1))
            {
                try
                {
                    var task = this.twitterApi.FriendshipsDestroy(id);
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
            var id = _curPost?.ScreenName ?? "";

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
                    var task = this.twitterApi.FriendshipsShow(this.twitterApi.CurrentScreenName, id);
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

            string result = "";
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
            foreach (string id in ids)
            {
                bool isFollowing, isFollowed;

                using (var dialog = new WaitingDialog(Properties.Resources.ShowFriendshipText1))
                {
                    var cancellationToken = dialog.EnableCancellation();

                    try
                    {
                        var task = this.twitterApi.FriendshipsShow(this.twitterApi.CurrentScreenName, id);
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

                string result = "";
                string ff = "";

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
                        Properties.Resources.GetFriendshipInfo7 + System.Environment.NewLine + result, Properties.Resources.GetFriendshipInfo8,
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
        {
            await this.doShowUserStatus(tw.Username, false);
            //if (!string.IsNullOrEmpty(tw.UserInfoXml))
            //{
            //    doShowUserStatus(tw.Username, false);
            //}
            //else
            //{
            //    MessageBox.Show(Properties.Resources.ShowYourProfileText1, "Your status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
        }

        // TwitterIDでない固定文字列を調べる（文字列検証のみ　実際に取得はしない）
        // URLから切り出した文字列を渡す

        public bool IsTwitterId(string name)
        {
            if (this.tw.Configuration.NonUsernamePaths == null || this.tw.Configuration.NonUsernamePaths.Length == 0)
                return !Regex.Match(name, @"^(about|jobs|tos|privacy|who_to_follow|download|messages)$", RegexOptions.IgnoreCase).Success;
            else
                return !this.tw.Configuration.NonUsernamePaths.Contains(name.ToLowerInvariant());
        }

        private void IdeographicSpaceToSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifySettingCommon = true;
        }

        private void ToolStripFocusLockMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ModifySettingCommon = true;
        }

        private void doQuoteOfficial()
        {
            if (this.ExistCurrentPost)
            {
                if (_curPost.IsDm ||
                    !StatusText.Enabled) return;

                if (_curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }

                StatusText.Text = " " + MyCommon.GetStatusUrl(_curPost);

                this.inReplyTo = null;

                StatusText.SelectionStart = 0;
                StatusText.Focus();
            }
        }

        private void doReTweetUnofficial()
        {
            //RT @id:内容
            if (this.ExistCurrentPost)
            {
                if (_curPost.IsDm || !StatusText.Enabled)
                    return;

                if (_curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }
                string rtdata = _curPost.Text;
                rtdata = CreateRetweetUnofficial(rtdata, this.StatusText.Multiline);

                StatusText.Text = " RT @" + _curPost.ScreenName + ": " + rtdata;

                // 投稿時に in_reply_to_status_id を付加する
                var inReplyToStatusId = this._curPost.RetweetedId ?? this._curPost.StatusId;
                var inReplyToScreenName = this._curPost.ScreenName;
                this.inReplyTo = Tuple.Create(inReplyToStatusId, inReplyToScreenName);

                StatusText.SelectionStart = 0;
                StatusText.Focus();
            }
        }

        private void QuoteStripMenuItem_Click(object sender, EventArgs e) // Handles QuoteStripMenuItem.Click, QtOpMenuItem.Click
        {
            doQuoteOfficial();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            //公式検索
            Control pnl = ((Control)sender).Parent;
            if (pnl == null) return;
            string tbName = pnl.Parent.Text;
            var tb = (PublicSearchTabModel)_statuses.Tabs[tbName];
            ComboBox cmb = (ComboBox)pnl.Controls["comboSearch"];
            ComboBox cmbLang = (ComboBox)pnl.Controls["comboLang"];
            cmb.Text = cmb.Text.Trim();
            // 検索式演算子 OR についてのみ大文字しか認識しないので強制的に大文字とする
            bool Quote = false;
            StringBuilder buf = new StringBuilder();
            char[] c = cmb.Text.ToCharArray();
            for (int cnt = 0; cnt < cmb.Text.Length; cnt++)
            {
                if (cnt > cmb.Text.Length - 4)
                {
                    buf.Append(cmb.Text.Substring(cnt));
                    break;
                }
                if (c[cnt] == '"')
                {
                    Quote = !Quote;
                }
                else
                {
                    if (!Quote && cmb.Text.Substring(cnt, 4).Equals(" or ", StringComparison.OrdinalIgnoreCase))
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
            if (string.IsNullOrEmpty(cmb.Text))
            {
                listView.Focus();
                SaveConfigsTabs();
                return;
            }
            if (queryChanged)
            {
                int idx = cmb.Items.IndexOf(tb.SearchWords);
                if (idx > -1) cmb.Items.RemoveAt(idx);
                cmb.Items.Insert(0, tb.SearchWords);
                cmb.Text = tb.SearchWords;
                cmb.SelectAll();
                this.PurgeListViewItemCache();
                listView.VirtualListSize = 0;
                _statuses.ClearTabIds(tbName);
                SaveConfigsTabs();   //検索条件の保存
            }

            this.GetPublicSearchAsync(tb);
            listView.Focus();
        }

        private async void RefreshMoreStripMenuItem_Click(object sender, EventArgs e)
        {
            //もっと前を取得
            await this.DoRefreshMore();
        }

        /// <summary>
        /// 指定されたタブのListTabにおける位置を返します
        /// </summary>
        /// <remarks>
        /// 非表示のタブについて -1 が返ることを常に考慮して下さい
        /// </remarks>
        public int GetTabPageIndex(string tabName)
        {
            var index = 0;
            foreach (var tabPage in this.ListTab.TabPages.Cast<TabPage>())
            {
                if (tabPage.Text == tabName)
                    return index;

                index++;
            }

            return -1;
        }

        private void UndoRemoveTabMenuItem_Click(object sender, EventArgs e)
        {
            if (_statuses.RemovedTab.Count == 0)
            {
                MessageBox.Show("There isn't removed tab.", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                DetailsListView listView = null;

                TabModel tb = _statuses.RemovedTab.Pop();
                if (tb.TabType == MyCommon.TabUsageType.Related)
                {
                    var relatedTab = _statuses.GetTabByType(MyCommon.TabUsageType.Related);
                    if (relatedTab != null)
                    {
                        // 関連発言なら既存のタブを置き換える
                        tb.TabName = relatedTab.TabName;
                        this.ClearTab(tb.TabName, false);
                        _statuses.Tabs[tb.TabName] = tb;

                        for (int i = 0; i < ListTab.TabPages.Count; i++)
                        {
                            var tabPage = ListTab.TabPages[i];
                            if (tb.TabName == tabPage.Text)
                            {
                                listView = (DetailsListView)tabPage.Tag;
                                ListTab.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        const string TabName = "Related Tweets";
                        string renamed = TabName;
                        for (int i = 2; i <= 100; i++)
                        {
                            if (!_statuses.ContainsTab(renamed)) break;
                            renamed = TabName + i;
                        }
                        tb.TabName = renamed;

                        _statuses.AddTab(tb);
                        AddNewTab(tb, startup: false);

                        var tabPage = ListTab.TabPages[ListTab.TabPages.Count - 1];
                        listView = (DetailsListView)tabPage.Tag;
                        ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                    }
                }
                else
                {
                    string renamed = tb.TabName;
                    for (int i = 1; i < int.MaxValue; i++)
                    {
                        if (!_statuses.ContainsTab(renamed)) break;
                        renamed = tb.TabName + "(" + i + ")";
                    }
                    tb.TabName = renamed;

                    _statuses.AddTab(tb);
                    AddNewTab(tb, startup: false);

                    var tabPage = ListTab.TabPages[ListTab.TabPages.Count - 1];
                    listView = (DetailsListView)tabPage.Tag;
                    ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                }
                SaveConfigsTabs();

                if (listView != null)
                {
                    using (ControlTransaction.Update(listView))
                    {
                        listView.VirtualListSize = tb.AllCount;
                    }
                }
            }
        }

        private async Task doMoveToRTHome()
        {
            if (_curList.SelectedIndices.Count > 0)
            {
                PostClass post = GetCurTabPost(_curList.SelectedIndices[0]);
                if (post.RetweetedId != null)
                {
                    await this.OpenUriInBrowserAsync("https://twitter.com/" + GetCurTabPost(_curList.SelectedIndices[0]).RetweetedBy);
                }
            }
        }

        private async void MoveToRTHomeMenuItem_Click(object sender, EventArgs e)
        {
            await this.doMoveToRTHome();
        }

        private async void ListManageUserContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var screenName = this._curPost?.ScreenName;
            if (screenName != null)
                await this.ListManageUserContext(screenName);
        }

        public async Task ListManageUserContext(string screenName)
        {
            if (this._statuses.SubscribableLists.Count == 0)
            {
                try
                {
                    using (var dialog = new WaitingDialog(Properties.Resources.ListsGetting))
                    {
                        var cancellationToken = dialog.EnableCancellation();

                        var task = this.tw.GetListsApi();
                        await dialog.WaitForAsync(this, task);

                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
                catch (OperationCanceledException) { return; }
                catch (WebApiException ex)
                {
                    MessageBox.Show("Failed to get lists. (" + ex.Message + ")");
                    return;
                }
            }

            using (MyLists listSelectForm = new MyLists(screenName, this.tw))
            {
                listSelectForm.ShowDialog(this);
            }
        }

        private void SearchControls_Enter(object sender, EventArgs e)
        {
            Control pnl = (Control)sender;
            foreach (Control ctl in pnl.Controls)
            {
                ctl.TabStop = true;
            }
        }

        private void SearchControls_Leave(object sender, EventArgs e)
        {
            Control pnl = (Control)sender;
            foreach (Control ctl in pnl.Controls)
            {
                ctl.TabStop = false;
            }
        }

        private void PublicSearchQueryMenuItem_Click(object sender, EventArgs e)
        {
            if (ListTab.SelectedTab != null)
            {
                if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType != MyCommon.TabUsageType.PublicSearch) return;
                ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
            }
        }

        private void StatusLabel_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(StatusLabel.TextHistory, "Logs", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void HashManageMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult rslt = DialogResult.Cancel;
            try
            {
                rslt = HashMgr.ShowDialog();
            }
            catch (Exception)
            {
                return;
            }
            this.TopMost = this._cfgCommon.AlwaysTop;
            if (rslt == DialogResult.Cancel) return;
            if (!string.IsNullOrEmpty(HashMgr.UseHash))
            {
                HashStripSplitButton.Text = HashMgr.UseHash;
                HashToggleMenuItem.Checked = true;
                HashToggleToolStripMenuItem.Checked = true;
            }
            else
            {
                HashStripSplitButton.Text = "#[-]";
                HashToggleMenuItem.Checked = false;
                HashToggleToolStripMenuItem.Checked = false;
            }
            //if (HashMgr.IsInsert && HashMgr.UseHash != "")
            //{
            //    int sidx = StatusText.SelectionStart;
            //    string hash = HashMgr.UseHash + " ";
            //    if (sidx > 0)
            //    {
            //        if (StatusText.Text.Substring(sidx - 1, 1) != " ")
            //            hash = " " + hash;
            //    }
            //    StatusText.Text = StatusText.Text.Insert(sidx, hash);
            //    sidx += hash.Length;
            //    StatusText.SelectionStart = sidx;
            //    StatusText.Focus();
            //}
            ModifySettingCommon = true;
            this.StatusText_TextChanged(null, null);
        }

        private void HashToggleMenuItem_Click(object sender, EventArgs e)
        {
            HashMgr.ToggleHash();
            if (!string.IsNullOrEmpty(HashMgr.UseHash))
            {
                HashStripSplitButton.Text = HashMgr.UseHash;
                HashToggleMenuItem.Checked = true;
                HashToggleToolStripMenuItem.Checked = true;
            }
            else
            {
                HashStripSplitButton.Text = "#[-]";
                HashToggleMenuItem.Checked = false;
                HashToggleToolStripMenuItem.Checked = false;
            }
            ModifySettingCommon = true;
            this.StatusText_TextChanged(null, null);
        }

        private void HashStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            HashToggleMenuItem_Click(null, null);
        }

        public void SetPermanentHashtag(string hashtag)
        {
            HashMgr.SetPermanentHash("#" + hashtag);
            HashStripSplitButton.Text = HashMgr.UseHash;
            HashToggleMenuItem.Checked = true;
            HashToggleToolStripMenuItem.Checked = true;
            //使用ハッシュタグとして設定
            ModifySettingCommon = true;
        }

        private void MenuItemOperate_DropDownOpening(object sender, EventArgs e)
        {
            if (ListTab.SelectedTab == null) return;
            if (_statuses == null || _statuses.Tabs == null || !_statuses.Tabs.ContainsKey(ListTab.SelectedTab.Text)) return;
            if (!this.ExistCurrentPost)
            {
                this.ReplyOpMenuItem.Enabled = false;
                this.ReplyAllOpMenuItem.Enabled = false;
                this.DmOpMenuItem.Enabled = false;
                this.ShowProfMenuItem.Enabled = false;
                this.ShowUserTimelineToolStripMenuItem.Enabled = false;
                this.ListManageMenuItem.Enabled = false;
                this.OpenFavOpMenuItem.Enabled = false;
                this.CreateTabRuleOpMenuItem.Enabled = false;
                this.CreateIdRuleOpMenuItem.Enabled = false;
                this.CreateSourceRuleOpMenuItem.Enabled = false;
                this.ReadOpMenuItem.Enabled = false;
                this.UnreadOpMenuItem.Enabled = false;
            }
            else
            {
                this.ReplyOpMenuItem.Enabled = true;
                this.ReplyAllOpMenuItem.Enabled = true;
                this.DmOpMenuItem.Enabled = true;
                this.ShowProfMenuItem.Enabled = true;
                this.ShowUserTimelineToolStripMenuItem.Enabled = true;
                this.ListManageMenuItem.Enabled = true;
                this.OpenFavOpMenuItem.Enabled = true;
                this.CreateTabRuleOpMenuItem.Enabled = true;
                this.CreateIdRuleOpMenuItem.Enabled = true;
                this.CreateSourceRuleOpMenuItem.Enabled = true;
                this.ReadOpMenuItem.Enabled = true;
                this.UnreadOpMenuItem.Enabled = true;
            }

            if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.DirectMessage || !this.ExistCurrentPost || _curPost.IsDm)
            {
                this.FavOpMenuItem.Enabled = false;
                this.UnFavOpMenuItem.Enabled = false;
                this.OpenStatusOpMenuItem.Enabled = false;
                this.OpenFavotterOpMenuItem.Enabled = false;
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
                this.OpenFavotterOpMenuItem.Enabled = true;
                this.ShowRelatedStatusesMenuItem2.Enabled = true;  //PublicSearchの時問題出るかも

                if (_curPost.IsMe)
                {
                    this.RtOpMenuItem.Enabled = false;  //公式RTは無効に
                    this.RtUnOpMenuItem.Enabled = true;
                    this.QtOpMenuItem.Enabled = true;
                    this.FavoriteRetweetMenuItem.Enabled = false;  //公式RTは無効に
                    this.FavoriteRetweetUnofficialMenuItem.Enabled = true;
                }
                else
                {
                    if (_curPost.IsProtect)
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
            }

            if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType != MyCommon.TabUsageType.Favorites)
            {
                this.RefreshPrevOpMenuItem.Enabled = true;
            }
            else
            {
                this.RefreshPrevOpMenuItem.Enabled = false;
            }
            if (!this.ExistCurrentPost
                || _curPost.InReplyToStatusId == null)
            {
                OpenRepSourceOpMenuItem.Enabled = false;
            }
            else
            {
                OpenRepSourceOpMenuItem.Enabled = true;
            }
            if (!this.ExistCurrentPost || string.IsNullOrEmpty(_curPost.RetweetedBy))
            {
                OpenRterHomeMenuItem.Enabled = false;
            }
            else
            {
                OpenRterHomeMenuItem.Enabled = true;
            }

            if (this.ExistCurrentPost)
            {
                this.DelOpMenuItem.Enabled = this._curPost.CanDeleteBy(this.tw.UserId);
            }
        }

        private void MenuItemTab_DropDownOpening(object sender, EventArgs e)
        {
            ContextMenuTabProperty_Opening(sender, null);
        }

        public Twitter TwitterInstance
        {
            get { return tw; }
        }

        private void SplitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this._initialLayout)
                return;

            int splitterDistance;
            switch (this.WindowState)
            {
                case FormWindowState.Normal:
                    splitterDistance = this.SplitContainer3.SplitterDistance;
                    break;
                case FormWindowState.Maximized:
                    // 最大化時は、通常時のウィンドウサイズに換算した SplitterDistance を算出する
                    var normalContainerWidth = this._mySize.Width - SystemInformation.Border3DSize.Width * 2;
                    splitterDistance = this.SplitContainer3.SplitterDistance - (this.SplitContainer3.Width - normalContainerWidth);
                    splitterDistance = Math.Min(splitterDistance, normalContainerWidth - this.SplitContainer3.SplitterWidth - this.SplitContainer3.Panel2MinSize);
                    break;
                default:
                    return;
            }

            this._mySpDis3 = splitterDistance;
            this.ModifySettingLocal = true;
        }

        private void MenuItemEdit_DropDownOpening(object sender, EventArgs e)
        {
            if (_statuses.RemovedTab.Count == 0)
            {
                UndoRemoveTabMenuItem.Enabled = false;
            }
            else
            {
                UndoRemoveTabMenuItem.Enabled = true;
            }
            if (ListTab.SelectedTab != null)
            {
                if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.PublicSearch)
                    PublicSearchQueryMenuItem.Enabled = true;
                else
                    PublicSearchQueryMenuItem.Enabled = false;
            }
            else
            {
                PublicSearchQueryMenuItem.Enabled = false;
            }
            if (!this.ExistCurrentPost)
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
                if (_curPost.IsDm) this.CopyURLMenuItem.Enabled = false;
                if (_curPost.IsProtect) this.CopySTOTMenuItem.Enabled = false;
            }
        }

        private void NotifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            SetNotifyIconText();
        }

        private async void UserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var id = _curPost?.ScreenName ?? "";

            await this.ShowUserStatus(id);
        }

        private async Task doShowUserStatus(string id, bool ShowInputDialog)
        {
            TwitterUser user = null;

            if (ShowInputDialog)
            {
                using (var inputName = new InputTabName())
                {
                    inputName.FormTitle = "Show UserStatus";
                    inputName.FormDescription = Properties.Resources.FRMessage1;
                    inputName.TabName = id;

                    if (inputName.ShowDialog(this) != DialogResult.OK)
                        return;
                    if (string.IsNullOrWhiteSpace(inputName.TabName))
                        return;

                    id = inputName.TabName.Trim();
                }
            }

            using (var dialog = new WaitingDialog(Properties.Resources.doShowUserStatusText1))
            {
                var cancellationToken = dialog.EnableCancellation();

                try
                {
                    var task = this.twitterApi.UsersShow(id);
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

            await this.doShowUserStatus(user);
        }

        private async Task doShowUserStatus(TwitterUser user)
        {
            using (var userDialog = new UserInfoDialog(this, this.twitterApi))
            {
                var showUserTask = userDialog.ShowUserAsync(user);
                userDialog.ShowDialog(this);

                this.Activate();
                this.BringToFront();

                // ユーザー情報の表示が完了するまで userDialog を破棄しない
                await showUserTask;
            }
        }

        internal Task ShowUserStatus(string id, bool ShowInputDialog)
        {
            return this.doShowUserStatus(id, ShowInputDialog);
        }

        internal Task ShowUserStatus(string id)
        {
            return this.doShowUserStatus(id, true);
        }

        private async void ShowProfileMenuItem_Click(object sender, EventArgs e)
        {
            if (_curPost != null)
            {
                await this.ShowUserStatus(_curPost.ScreenName, false);
            }
        }

        private async void RtCountMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.ExistCurrentPost)
                return;

            var statusId = this._curPost.RetweetedId ?? this._curPost.StatusId;
            TwitterStatus status;

            using (var dialog = new WaitingDialog(Properties.Resources.RtCountMenuItem_ClickText1))
            {
                var cancellationToken = dialog.EnableCancellation();

                try
                {
                    var task = this.twitterApi.StatusesShow(statusId);
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

        private HookGlobalHotkey _hookGlobalHotkey;
        public TweenMain()
        {
            _hookGlobalHotkey = new HookGlobalHotkey(this);

            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。

            if (!this.DesignMode)
            {
                // デザイナでの編集時にレイアウトが縦方向に数pxずれる問題の対策
                this.StatusText.Dock = DockStyle.Fill;
            }

            this.tweetDetailsView.Owner = this;

            this.TimerTimeline.Elapsed += this.TimerTimeline_Elapsed;
            this._hookGlobalHotkey.HotkeyPressed += _hookGlobalHotkey_HotkeyPressed;
            this.gh.NotifyClicked += GrowlHelper_Callback;

            // メイリオフォント指定時にタブの最小幅が広くなる問題の対策
            this.ListTab.HandleCreated += (s, e) => NativeMethods.SetMinTabWidth((TabControl)s, 40);

            this.ImageSelector.Visible = false;
            this.ImageSelector.Enabled = false;
            this.ImageSelector.FilePickDialog = OpenFileDialog1;

            this.workerProgress = new Progress<string>(x => this.StatusLabel.Text = x);

            this.ReplaceAppName();
            this.InitializeShortcuts();
        }

        private void _hookGlobalHotkey_HotkeyPressed(object sender, KeyEventArgs e)
        {
            if ((this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized) && this.Visible && Form.ActiveForm == this)
            {
                //アイコン化
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
        {
            this.MultiLineMenuItem.PerformClick();
        }

        public PostClass CurPost
        {
            get { return _curPost; }
        }

#region "画像投稿"
        private void ImageSelectMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageSelector.Visible)
                ImageSelector.EndSelection();
            else
                ImageSelector.BeginSelection();
        }

        private void SelectMedia_DragEnter(DragEventArgs e)
        {
            if (ImageSelector.HasUploadableService(((string[])e.Data.GetData(DataFormats.FileDrop, false))[0], true))
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
            ImageSelector.BeginSelection((string[])e.Data.GetData(DataFormats.FileDrop, false));
            StatusText.Focus();
        }

        private void ImageSelector_BeginSelecting(object sender, EventArgs e)
        {
            TimelinePanel.Visible = false;
            TimelinePanel.Enabled = false;
        }

        private void ImageSelector_EndSelecting(object sender, EventArgs e)
        {
            TimelinePanel.Visible = true;
            TimelinePanel.Enabled = true;
            ((DetailsListView)ListTab.SelectedTab.Tag).Focus();
        }

        private void ImageSelector_FilePickDialogOpening(object sender, EventArgs e)
        {
            this.AllowDrop = false;
        }

        private void ImageSelector_FilePickDialogClosed(object sender, EventArgs e)
        {
            this.AllowDrop = true;
        }

        private void ImageSelector_SelectedServiceChanged(object sender, EventArgs e)
        {
            if (ImageSelector.Visible)
            {
                ModifySettingCommon = true;
                SaveConfigsAll(true);

                if (ImageSelector.ServiceName.Equals("Twitter"))
                    this.StatusText_TextChanged(null, null);
            }
        }

        private void ImageSelector_VisibleChanged(object sender, EventArgs e)
        {
            this.StatusText_TextChanged(null, null);
        }

        /// <summary>
        /// StatusTextでCtrl+Vが押下された時の処理
        /// </summary>
        private void ProcClipboardFromStatusTextWhenCtrlPlusV()
        {
            if (Clipboard.ContainsText())
            {
                // clipboardにテキストがある場合は貼り付け処理
                this.StatusText.Paste(Clipboard.GetText());
            }
            else if (Clipboard.ContainsImage())
            {
                // 画像があるので投稿処理を行う
                if (MessageBox.Show(Properties.Resources.PostPictureConfirm3,
                                   Properties.Resources.PostPictureWarn4,
                                   MessageBoxButtons.OKCancel,
                                   MessageBoxIcon.Question,
                                   MessageBoxDefaultButton.Button2)
                               == DialogResult.OK)
                {
                    // clipboardから画像を取得
                    using (var image = Clipboard.GetImage())
                    {
                        this.ImageSelector.BeginSelection(image);
                    }
                }
            }
        }
#endregion

        private void ListManageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ListManage form = new ListManage(tw))
            {
                form.ShowDialog(this);
            }
        }

        public bool ModifySettingCommon { get; set; }
        public bool ModifySettingLocal { get; set; }
        public bool ModifySettingAtId { get; set; }

        private void MenuItemCommand_DropDownOpening(object sender, EventArgs e)
        {
            if (this.ExistCurrentPost && !_curPost.IsDm)
                RtCountMenuItem.Enabled = true;
            else
                RtCountMenuItem.Enabled = false;

            //if (SettingDialog.UrlConvertAuto && SettingDialog.ShortenTco)
            //    TinyUrlConvertToolStripMenuItem.Enabled = false;
            //else
            //    TinyUrlConvertToolStripMenuItem.Enabled = true;
        }

        private void CopyUserIdStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyUserId();
        }

        private void CopyUserId()
        {
            if (_curPost == null) return;
            string clstr = _curPost.ScreenName;
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
            if (this.ExistCurrentPost && !_curPost.IsDm)
            {
                try
                {
                    await this.OpenRelatedTab(this._curPost);
                }
                catch (TabException ex)
                {
                    MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 指定されたツイートに対する関連発言タブを開きます
        /// </summary>
        /// <param name="statusId">表示するツイートのID</param>
        /// <exception cref="TabException">名前の重複が多すぎてタブを作成できない場合</exception>
        private async Task OpenRelatedTab(long statusId)
        {
            var post = this._statuses[statusId];
            if (post == null)
            {
                try
                {
                    post = await this.tw.GetStatusApi(false, statusId);
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
            var tabRelated = this._statuses.GetTabByType<RelatedPostsTabModel>();
            if (tabRelated != null)
            {
                this.RemoveSpecifiedTab(tabRelated.TabName, confirm: false);
            }

            var tabName = this._statuses.MakeTabName("Related Tweets");

            tabRelated = new RelatedPostsTabModel(tabName, post);
            tabRelated.UnreadManage = false;
            tabRelated.Notify = false;

            this._statuses.AddTab(tabRelated);
            this.AddNewTab(tabRelated, startup: false);

            TabPage tabPage;
            for (int i = 0; i < this.ListTab.TabPages.Count; i++)
            {
                tabPage = this.ListTab.TabPages[i];
                if (tabName == tabPage.Text)
                {
                    this.ListTab.SelectedIndex = i;
                    break;
                }
            }

            await this.GetRelatedTweetsAsync(tabRelated);

            tabPage = this.ListTab.TabPages.Cast<TabPage>()
                .FirstOrDefault(x => x.Text == tabRelated.TabName);

            if (tabPage != null)
            {
                // TODO: 非同期更新中にタブが閉じられている場合を厳密に考慮したい

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
            StringBuilder buf = new StringBuilder();
            //buf.AppendFormat("キャッシュメモリ容量         : {0}bytes({1}MB)" + Environment.NewLine, IconCache.CacheMemoryLimit, ((ImageDictionary)IconCache).CacheMemoryLimit / 1048576);
            //buf.AppendFormat("物理メモリ使用割合           : {0}%" + Environment.NewLine, IconCache.PhysicalMemoryLimit);
            buf.AppendFormat("キャッシュエントリ保持数     : {0}" + Environment.NewLine, IconCache.CacheCount);
            buf.AppendFormat("キャッシュエントリ破棄数     : {0}" + Environment.NewLine, IconCache.CacheRemoveCount);
            MessageBox.Show(buf.ToString(), "アイコンキャッシュ使用状況");
        }

        private void tw_UserIdChanged()
        {
            this.ModifySettingCommon = true;
        }

#region "Userstream"
        private void tw_PostDeleted(object sender, PostDeletedEventArgs e)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke((Action) (async () =>
                           {
                               this._statuses.RemovePostFromAllTabs(e.StatusId, setIsDeleted: true);
                               if (_curTab != null && _statuses.Tabs[_curTab.Text].Contains(e.StatusId))
                               {
                                   this.PurgeListViewItemCache();
                                   ((DetailsListView)_curTab.Tag).Update();
                                   if (_curPost != null && _curPost.StatusId == e.StatusId)
                                       await this.DispSelectedPost(true);
                               }
                           }));
                    return;
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

        private int userStreamsRefreshing = 0;

        private async void tw_NewPostFromStream(object sender, EventArgs e)
        {
            if (this._cfgCommon.ReadOldPosts)
            {
                _statuses.SetReadHomeTab(); //新着時未読クリア
            }

            this._statuses.DistributePosts();

            if (this._cfgCommon.UserstreamPeriod > 0) return;

            // userStreamsRefreshing が 0 (インクリメント後は1) であれば RefreshTimeline を実行
            if (Interlocked.Increment(ref this.userStreamsRefreshing) == 1)
            {
                try
                {
                    await this.InvokeAsync(() => this.RefreshTimeline())
                        .ConfigureAwait(false);
                }
                finally
                {
                    Interlocked.Exchange(ref this.userStreamsRefreshing, 0);
                }
            }
        }

        private void tw_UserStreamStarted(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke((Action)(() => this.tw_UserStreamStarted(sender, e)));
                    return;
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

            this.RefreshUserStreamsMenu();
            this.MenuItemUserStream.Enabled = true;

            StatusLabel.Text = "UserStream Started.";
        }

        private void tw_UserStreamStopped(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke((Action)(() => this.tw_UserStreamStopped(sender, e)));
                    return;
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

            this.RefreshUserStreamsMenu();
            this.MenuItemUserStream.Enabled = true;

            StatusLabel.Text = "UserStream Stopped.";
        }

        private void RefreshUserStreamsMenu()
        {
            if (this.tw.UserStreamActive)
            {
                this.MenuItemUserStream.Text = "&UserStream ▶";
                this.StopToolStripMenuItem.Text = "&Stop";
            }
            else
            {
                this.MenuItemUserStream.Text = "&UserStream ■";
                this.StopToolStripMenuItem.Text = "&Start";
            }
        }

        private void tw_UserStreamEventArrived(object sender, UserStreamEventReceivedEventArgs e)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke((Action)(() => this.tw_UserStreamEventArrived(sender, e)));
                    return;
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
            var ev = e.EventData;
            StatusLabel.Text = "Event: " + ev.Event;
            //if (ev.Event == "favorite")
            //{
            //    NotifyFavorite(ev);
            //}
            NotifyEvent(ev);
            if (ev.Event == "favorite" || ev.Event == "unfavorite")
            {
                if (_curTab != null && _statuses.Tabs[_curTab.Text].Contains(ev.Id))
                {
                    this.PurgeListViewItemCache();
                    ((DetailsListView)_curTab.Tag).Update();
                }
                if (ev.Event == "unfavorite" && ev.Username.ToLowerInvariant().Equals(tw.Username.ToLowerInvariant()))
                {
                    var favTab = this._statuses.GetTabByType(MyCommon.TabUsageType.Favorites);
                    favTab.EnqueueRemovePost(ev.Id, setIsDeleted: false);
                }
            }
        }

        private void NotifyEvent(Twitter.FormattedEvent ev)
        {
            //新着通知 
            if (BalloonRequired(ev))
            {
                NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                //if (SettingDialog.DispUsername) NotifyIcon1.BalloonTipTitle = tw.Username + " - "; else NotifyIcon1.BalloonTipTitle = "";
                //NotifyIcon1.BalloonTipTitle += Application.ProductName + " [" + ev.Event.ToUpper() + "] by " + ((string)(!string.IsNullOrEmpty(ev.Username) ? ev.Username : ""), string);
                StringBuilder title = new StringBuilder();
                if (this._cfgCommon.DispUsername)
                {
                    title.Append(tw.Username);
                    title.Append(" - ");
                }
                else
                {
                    //title.Clear();
                }
                title.Append(Application.ProductName);
                title.Append(" [");
                title.Append(ev.Event.ToUpper(CultureInfo.CurrentCulture));
                title.Append("] by ");
                if (!string.IsNullOrEmpty(ev.Username))
                {
                    title.Append(ev.Username);
                }
                else
                {
                    //title.Append("");
                }
                string text;
                if (!string.IsNullOrEmpty(ev.Target))
                {
                    //NotifyIcon1.BalloonTipText = ev.Target;
                    text = ev.Target;
                }
                else
                {
                    //NotifyIcon1.BalloonTipText = " ";
                    text = " ";
                }
                //NotifyIcon1.ShowBalloonTip(500);
                if (this._cfgCommon.IsUseNotifyGrowl)
                {
                    gh.Notify(GrowlHelper.NotifyType.UserStreamEvent,
                              ev.Id.ToString(), title.ToString(), text);
                }
                else
                {
                    NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                    NotifyIcon1.BalloonTipTitle = title.ToString();
                    NotifyIcon1.BalloonTipText = text;
                    NotifyIcon1.ShowBalloonTip(500);
                }
            }

            //サウンド再生
            string snd = this._cfgCommon.EventSoundFile;
            if (!_initial && this._cfgCommon.PlaySound && !string.IsNullOrEmpty(snd))
            {
                if ((ev.Eventtype & this._cfgCommon.EventNotifyFlag) != 0 && IsMyEventNotityAsEventType(ev))
                {
                    try
                    {
                        string dir = Application.StartupPath;
                        if (Directory.Exists(Path.Combine(dir, "Sounds")))
                        {
                            dir = Path.Combine(dir, "Sounds");
                        }
                        using (SoundPlayer player = new SoundPlayer(Path.Combine(dir, snd)))
                        {
                            player.Play();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuItemUserStream.Enabled = false;
            if (StopRefreshAllMenuItem.Checked)
            {
                StopRefreshAllMenuItem.Checked = false;
                return;
            }
            if (this.tw.UserStreamActive)
            {
                tw.StopUserStream();
            }
            else
            {
                tw.StartUserStream();
            }
        }

        private static string inputTrack = "";

        private void TrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrackToolStripMenuItem.Checked)
            {
                using (InputTabName inputForm = new InputTabName())
                {
                    inputForm.TabName = inputTrack;
                    inputForm.FormTitle = "Input track word";
                    inputForm.FormDescription = "Track word";
                    if (inputForm.ShowDialog() != DialogResult.OK)
                    {
                        TrackToolStripMenuItem.Checked = false;
                        return;
                    }
                    inputTrack = inputForm.TabName.Trim();
                }
                if (!inputTrack.Equals(tw.TrackWord))
                {
                    tw.TrackWord = inputTrack;
                    this.ModifySettingCommon = true;
                    TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(inputTrack);
                    tw.ReconnectUserStream();
                }
            }
            else
            {
                tw.TrackWord = "";
                tw.ReconnectUserStream();
            }
            this.ModifySettingCommon = true;
        }

        private void AllrepliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tw.AllAtReply = AllrepliesToolStripMenuItem.Checked;
            this.ModifySettingCommon = true;
            tw.ReconnectUserStream();
        }

        private void EventViewerMenuItem_Click(object sender, EventArgs e)
        {
            if (evtDialog == null || evtDialog.IsDisposed)
            {
                evtDialog = null;
                evtDialog = new EventViewerDialog();
                evtDialog.Owner = this;
                //親の中央に表示
                Point pos = evtDialog.Location;
                pos.X = Convert.ToInt32(this.Location.X + this.Size.Width / 2 - evtDialog.Size.Width / 2);
                pos.Y = Convert.ToInt32(this.Location.Y + this.Size.Height / 2 - evtDialog.Size.Height / 2);
                evtDialog.Location = pos;
            }
            evtDialog.EventSource = tw.StoredEvent;
            if (!evtDialog.Visible)
            {
                evtDialog.Show(this);
            }
            else
            {
                evtDialog.Activate();
            }
            this.TopMost = this._cfgCommon.AlwaysTop;
        }
#endregion

        private void TweenRestartMenuItem_Click(object sender, EventArgs e)
        {
            MyCommon._endingFlag = true;
            try
            {
                this.Close();
                Application.Restart();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to restart. Please run " + Application.ProductName + " manually.");
            }
        }

        private async void OpenOwnFavedMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tw.Username))
                await this.OpenUriInBrowserAsync(Properties.Resources.FavstarUrl + "users/" + tw.Username + "/recent");
        }

        private async void OpenOwnHomeMenuItem_Click(object sender, EventArgs e)
        {
            await this.OpenUriInBrowserAsync(MyCommon.TwitterUrl + tw.Username);
        }

        private bool ExistCurrentPost
        {
            get
            {
                if (_curPost == null) return false;
                if (_curPost.IsDeleted) return false;
                return true;
            }
        }

        private void ShowUserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowUserTimeline();
        }

        private string GetUserIdFromCurPostOrInput(string caption)
        {
            var id = _curPost?.ScreenName ?? "";

            using (InputTabName inputName = new InputTabName())
            {
                inputName.FormTitle = caption;
                inputName.FormDescription = Properties.Resources.FRMessage1;
                inputName.TabName = id;
                if (inputName.ShowDialog() == DialogResult.OK &&
                    !string.IsNullOrEmpty(inputName.TabName.Trim()))
                {
                    id = inputName.TabName.Trim();
                }
                else
                {
                    id = "";
                }
            }
            return id;
        }

        private void UserTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string id = GetUserIdFromCurPostOrInput("Show UserTimeline");
            if (!string.IsNullOrEmpty(id))
            {
                AddNewTabForUserTimeline(id);
            }
        }

        private async void UserFavorareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string id = GetUserIdFromCurPostOrInput("Show Favstar");
            if (!string.IsNullOrEmpty(id))
            {
                await this.OpenUriInBrowserAsync(Properties.Resources.FavstarUrl + "users/" + id + "/recent");
            }
        }

        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume) osResumed = true;
        }

        private void TimelineRefreshEnableChange(bool isEnable)
        {
            if (isEnable)
            {
                tw.StartUserStream();
            }
            else
            {
                tw.StopUserStream();
            }
            TimerTimeline.Enabled = isEnable;
        }

        private void StopRefreshAllMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            TimelineRefreshEnableChange(!StopRefreshAllMenuItem.Checked);
        }

        private async Task OpenUserAppointUrl()
        {
            if (this._cfgCommon.UserAppointUrl != null)
            {
                if (this._cfgCommon.UserAppointUrl.Contains("{ID}") || this._cfgCommon.UserAppointUrl.Contains("{STATUS}"))
                {
                    if (_curPost != null)
                    {
                        string xUrl = this._cfgCommon.UserAppointUrl;
                        xUrl = xUrl.Replace("{ID}", _curPost.ScreenName);

                        var statusId = _curPost.RetweetedId ?? _curPost.StatusId;
                        xUrl = xUrl.Replace("{STATUS}", statusId.ToString());

                        await this.OpenUriInBrowserAsync(xUrl);
                    }
                }
                else
                {
                    await this.OpenUriInBrowserAsync(this._cfgCommon.UserAppointUrl);
                }
            }
        }

        private async void OpenUserSpecifiedUrlMenuItem_Click(object sender, EventArgs e)
        {
            await this.OpenUserAppointUrl();
        }

        private void GrowlHelper_Callback(object sender, GrowlHelper.NotifyCallbackEventArgs e)
        {
            if (Form.ActiveForm == null)
            {
                this.BeginInvoke((Action) (() =>
                {
                    this.Visible = true;
                    if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
                    this.Activate();
                    this.BringToFront();
                    if (e.NotifyType == GrowlHelper.NotifyType.DirectMessage)
                    {
                        if (!this.GoDirectMessage(e.StatusId)) this.StatusText.Focus();
                    }
                    else
                    {
                        if (!this.GoStatus(e.StatusId)) this.StatusText.Focus();
                    }
                }));
            }
        }

        private void ReplaceAppName()
        {
            MatomeMenuItem.Text = MyCommon.ReplaceAppName(MatomeMenuItem.Text);
            AboutMenuItem.Text = MyCommon.ReplaceAppName(AboutMenuItem.Text);
        }

        private void tweetThumbnail1_ThumbnailLoading(object sender, EventArgs e)
        {
            this.SplitContainer3.Panel2Collapsed = false;
        }

        private async void tweetThumbnail1_ThumbnailDoubleClick(object sender, ThumbnailDoubleClickEventArgs e)
        {
            await this.OpenThumbnailPicture(e.Thumbnail);
        }

        private async void tweetThumbnail1_ThumbnailImageSearchClick(object sender, ThumbnailImageSearchEventArgs e)
        {
            await this.OpenUriInBrowserAsync(e.ImageUrl);
        }

        private async Task OpenThumbnailPicture(ThumbnailInfo thumbnail)
        {
            var url = thumbnail.FullSizeImageUrl ?? thumbnail.MediaPageUrl;

            await this.OpenUriInBrowserAsync(url);
        }

        private async void TwitterApiStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await this.OpenUriInBrowserAsync(Twitter.ServiceAvailabilityStatusUrl);
        }

        private void PostButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                this.JumpUnreadMenuItem_Click(null, null);

                e.SuppressKeyPress = true;
            }
        }

        private void ContextMenuColumnHeader_Opening(object sender, CancelEventArgs e)
        {
            this.IconSizeNoneToolStripMenuItem.Checked = this._cfgCommon.IconSize == MyCommon.IconSizes.IconNone;
            this.IconSize16ToolStripMenuItem.Checked = this._cfgCommon.IconSize == MyCommon.IconSizes.Icon16;
            this.IconSize24ToolStripMenuItem.Checked = this._cfgCommon.IconSize == MyCommon.IconSizes.Icon24;
            this.IconSize48ToolStripMenuItem.Checked = this._cfgCommon.IconSize == MyCommon.IconSizes.Icon48;
            this.IconSize48_2ToolStripMenuItem.Checked = this._cfgCommon.IconSize == MyCommon.IconSizes.Icon48_2;

            this.LockListSortOrderToolStripMenuItem.Checked = this._cfgCommon.SortOrderLock;
        }

        private void IconSizeNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeListViewIconSize(MyCommon.IconSizes.IconNone);
        }

        private void IconSize16ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeListViewIconSize(MyCommon.IconSizes.Icon16);
        }

        private void IconSize24ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeListViewIconSize(MyCommon.IconSizes.Icon24);
        }

        private void IconSize48ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeListViewIconSize(MyCommon.IconSizes.Icon48);
        }

        private void IconSize48_2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeListViewIconSize(MyCommon.IconSizes.Icon48_2);
        }

        private void ChangeListViewIconSize(MyCommon.IconSizes iconSize)
        {
            if (this._cfgCommon.IconSize == iconSize) return;

            var oldIconCol = _iconCol;

            this._cfgCommon.IconSize = iconSize;
            ApplyListViewIconSize(iconSize);

            if (_iconCol != oldIconCol)
            {
                foreach (TabPage tp in ListTab.TabPages)
                {
                    ResetColumns((DetailsListView)tp.Tag);
                }
            }

            _curList?.Refresh();

            ModifySettingCommon = true;
        }

        private void LockListSortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var state = this.LockListSortOrderToolStripMenuItem.Checked;
            if (this._cfgCommon.SortOrderLock == state) return;

            this._cfgCommon.SortOrderLock = state;

            ModifySettingCommon = true;
        }

        private void tweetDetailsView_StatusChanged(object sender, TweetDetailsViewStatusChengedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.StatusText))
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
