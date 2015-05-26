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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
using OpenTween.Connection;
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
        private SettingLocal _cfgLocal;
        private SettingCommon _cfgCommon;
        private bool _modifySettingLocal = false;
        private bool _modifySettingCommon = false;
        private bool _modifySettingAtId = false;

        //twitter解析部
        private Twitter tw = new Twitter();

        //Growl呼び出し部
        private GrowlHelper gh = new GrowlHelper(Application.ProductName);

        //サブ画面インスタンス
        private SearchWordDialog SearchDialog = new SearchWordDialog();     //検索画面インスタンス
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
        private long? _reply_to_id;     // リプライ先のステータスID 0の場合はリプライではない 注：複数あてのものはリプライではない
        private string _reply_to_name;    // リプライ先ステータスの書き込み者の名前

        //時速表示用
        private List<DateTime> _postTimestamps = new List<DateTime>();
        private List<DateTime> _favTimestamps = new List<DateTime>();
        private Dictionary<DateTime, int> _tlTimestamps = new Dictionary<DateTime, int>();
        private int _tlCount;

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

        // ListViewItem のキャッシュ関連
        private int _itemCacheIndex;
        private ListViewItem[] _itemCache;
        private PostClass[] _postCache;
        private ReaderWriterLockSlim itemCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private TabPage _curTab;
        private int _curItemIndex;
        private DetailsListView _curList;
        private PostClass _curPost;
        private bool _isColumnChanged = false;

        private const int MAX_WORKER_THREADS = 20;
        private SemaphoreSlim workerSemaphore = new SemaphoreSlim(MAX_WORKER_THREADS);
        private CancellationTokenSource workerCts = new CancellationTokenSource();

        private int UnreadCounter = -1;
        private int UnreadAtCounter = -1;

        private string[] ColumnOrgText = new string[9];
        private string[] ColumnText = new string[9];

        private bool _DoFavRetweetFlags = false;
        private bool osResumed = false;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string _postBrowserStatusText = "";

        private bool _colorize = false;

        private System.Timers.Timer TimerTimeline = new System.Timers.Timer();

        private ImageListViewItem displayItem;

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
        private enum SEARCHTYPE
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
            public string[] imagePath = null;
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
                if (this.components != null)
                    this.components.Dispose();

                //後始末
                SearchDialog.Dispose();
                UrlDialog.Dispose();
                if (NIconAt != null) NIconAt.Dispose();
                if (NIconAtRed != null) NIconAtRed.Dispose();
                if (NIconAtSmoke != null) NIconAtSmoke.Dispose();
                foreach (var iconRefresh in this.NIconRefresh)
                {
                    if (iconRefresh != null)
                        iconRefresh.Dispose();
                }
                if (TabIcon != null) TabIcon.Dispose();
                if (MainIcon != null) MainIcon.Dispose();
                if (ReplyIcon != null) ReplyIcon.Dispose();
                if (ReplyIconBlink != null) ReplyIconBlink.Dispose();
                _listViewImageList.Dispose();
                _brsHighLight.Dispose();
                if (_brsBackColorMine != null) _brsBackColorMine.Dispose();
                if (_brsBackColorAt != null) _brsBackColorAt.Dispose();
                if (_brsBackColorYou != null) _brsBackColorYou.Dispose();
                if (_brsBackColorAtYou != null) _brsBackColorAtYou.Dispose();
                if (_brsBackColorAtFromTarget != null) _brsBackColorAtFromTarget.Dispose();
                if (_brsBackColorAtTo != null) _brsBackColorAtTo.Dispose();
                if (_brsBackColorNone != null) _brsBackColorNone.Dispose();
                if (_brsDeactiveSelection != null) _brsDeactiveSelection.Dispose();
                //sf.Dispose();
                sfTab.Dispose();

                this.workerCts.Cancel();

                if (IconCache != null)
                {
                    this.IconCache.CancelAsync();
                    this.IconCache.Dispose();
                }

                if (this.thumbnailTokenSource != null)
                    this.thumbnailTokenSource.Dispose();

                this.itemCacheLock.Dispose();
                this.tw.Dispose();
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
            ColumnHeader _colHd1 = new ColumnHeader();  //アイコン
            ColumnHeader _colHd2 = new ColumnHeader();  //ニックネーム
            ColumnHeader _colHd3 = new ColumnHeader();  //本文
            ColumnHeader _colHd4 = new ColumnHeader();  //日付
            ColumnHeader _colHd5 = new ColumnHeader();  //ユーザID
            ColumnHeader _colHd6 = new ColumnHeader();  //未読
            ColumnHeader _colHd7 = new ColumnHeader();  //マーク＆プロテクト
            ColumnHeader _colHd8 = new ColumnHeader();  //ソース

            if (!_iconCol)
            {
                list.Columns.AddRange(new ColumnHeader[] { _colHd1, _colHd2, _colHd3, _colHd4, _colHd5, _colHd6, _colHd7, _colHd8 });
            }
            else
            {
                list.Columns.AddRange(new ColumnHeader[] { _colHd1, _colHd3 });
            }

            InitColumnText();
            _colHd1.Text = ColumnText[0];
            _colHd1.Width = 48;
            _colHd2.Text = ColumnText[1];
            _colHd2.Width = 80;
            _colHd3.Text = ColumnText[2];
            _colHd3.Width = 300;
            _colHd4.Text = ColumnText[3];
            _colHd4.Width = 50;
            _colHd5.Text = ColumnText[4];
            _colHd5.Width = 50;
            _colHd6.Text = ColumnText[5];
            _colHd6.Width = 16;
            _colHd7.Text = ColumnText[6];
            _colHd7.Width = 16;
            _colHd8.Text = ColumnText[7];
            _colHd8.Width = 50;

            int[] dispOrder = new int[8];
            if (!startup)
            {
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
                for (int i = 0; i < _curList.Columns.Count; i++)
                {
                    list.Columns[i].Width = _curList.Columns[i].Width;
                    list.Columns[dispOrder[i]].DisplayIndex = i;
                }
            }
            else
            {
                var widthScaleFactor = this.CurrentAutoScaleDimensions.Width / this._cfgLocal.ScaleDimension.Width;

                if (_iconCol)
                {
                    list.Columns[0].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width1);
                    list.Columns[1].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width3);
                    list.Columns[0].DisplayIndex = 0;
                    list.Columns[1].DisplayIndex = 1;
                }
                else
                {
                    for (int i = 0; i <= 7; i++)
                    {
                        if (_cfgLocal.DisplayIndex1 == i)
                            dispOrder[i] = 0;
                        else if (_cfgLocal.DisplayIndex2 == i)
                            dispOrder[i] = 1;
                        else if (_cfgLocal.DisplayIndex3 == i)
                            dispOrder[i] = 2;
                        else if (_cfgLocal.DisplayIndex4 == i)
                            dispOrder[i] = 3;
                        else if (_cfgLocal.DisplayIndex5 == i)
                            dispOrder[i] = 4;
                        else if (_cfgLocal.DisplayIndex6 == i)
                            dispOrder[i] = 5;
                        else if (_cfgLocal.DisplayIndex7 == i)
                            dispOrder[i] = 6;
                        else if (_cfgLocal.DisplayIndex8 == i)
                            dispOrder[i] = 7;
                    }
                    list.Columns[0].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width1);
                    list.Columns[1].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width2);
                    list.Columns[2].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width3);
                    list.Columns[3].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width4);
                    list.Columns[4].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width5);
                    list.Columns[5].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width6);
                    list.Columns[6].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width7);
                    list.Columns[7].Width = ScaleBy(widthScaleFactor, _cfgLocal.Width8);
                    for (int i = 0; i <= 7; i++)
                    {
                        list.Columns[dispOrder[i]].DisplayIndex = i;
                    }
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
            if (!MyCommon.FileVersion.EndsWith("0"))
            {
                TraceOutToolStripMenuItem.Checked = true;
                MyCommon.TraceFlag = true;
            }
        }

        private void TweenMain_Load(object sender, EventArgs e)
        {
            _ignoreConfigSave = true;
            this.Visible = false;

            //Win32Api.SetProxy(HttpConnection.ProxyType.Specified, "127.0.0.1", 8080, "user", "pass")

            new InternetSecurityManager(PostBrowser);
            this.PostBrowser.AllowWebBrowserDrop = false;  // COMException を回避するため、ActiveX の初期化が終わってから設定する

            MyCommon.TwitterApiInfo.AccessLimitUpdated += TwitterApiStatus_AccessLimitUpdated;
            Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            if (MyApplication.StartupOptions.ContainsKey("d"))
                MyCommon.TraceFlag = true;

            Regex.CacheSize = 100;

            InitializeTraceFrag();
            LoadIcons(); // アイコン読み込み

            //発言保持クラス
            _statuses = TabInformations.GetInstance();

            //アイコン設定
            this.Icon = MainIcon;              //メインフォーム（TweenMain）
            NotifyIcon1.Icon = NIconAt;      //タスクトレイ
            TabImage.Images.Add(TabIcon);    //タブ見出し

            SearchDialog.Owner = this;
            UrlDialog.Owner = this;

            _history.Add(new PostingStatus());
            _hisIdx = 0;
            _reply_to_id = null;
            _reply_to_name = null;

            //<<<<<<<<<設定関連>>>>>>>>>
            //設定コンバージョン
            //ConvertConfig();

            ////設定読み出し
            LoadConfig();

            // 現在の DPI と設定保存時の DPI との比を取得する
            var configScaleFactor = this._cfgLocal.GetConfigScaleFactor(this.CurrentAutoScaleDimensions);

            //新着バルーン通知のチェック状態設定
            NewPostPopMenuItem.Checked = _cfgCommon.NewAllPop;
            this.NotifyFileMenuItem.Checked = NewPostPopMenuItem.Checked;

            //フォント＆文字色＆背景色保持
            _fntUnread = _cfgLocal.FontUnread;
            _clUnread = _cfgLocal.ColorUnread;
            _fntReaded = _cfgLocal.FontRead;
            _clReaded = _cfgLocal.ColorRead;
            _clFav = _cfgLocal.ColorFav;
            _clOWL = _cfgLocal.ColorOWL;
            _clRetweet = _cfgLocal.ColorRetweet;
            _fntDetail = _cfgLocal.FontDetail;
            _clDetail = _cfgLocal.ColorDetail;
            _clDetailLink = _cfgLocal.ColorDetailLink;
            _clDetailBackcolor = _cfgLocal.ColorDetailBackcolor;
            _clSelf = _cfgLocal.ColorSelf;
            _clAtSelf = _cfgLocal.ColorAtSelf;
            _clTarget = _cfgLocal.ColorTarget;
            _clAtTarget = _cfgLocal.ColorAtTarget;
            _clAtFromTarget = _cfgLocal.ColorAtFromTarget;
            _clAtTo = _cfgLocal.ColorAtTo;
            _clListBackcolor = _cfgLocal.ColorListBackcolor;
            _clInputBackcolor = _cfgLocal.ColorInputBackcolor;
            _clInputFont = _cfgLocal.ColorInputFont;
            _fntInputFont = _cfgLocal.FontInputFont;

            var fontUIGlobal = this._cfgLocal.FontUIGlobal;
            if (fontUIGlobal != null)
            {
                OTBaseForm.GlobalFont = fontUIGlobal;
                this.Font = fontUIGlobal;
            }

            // StringFormatオブジェクトへの事前設定
            //sf.Alignment = StringAlignment.Near;             // Textを近くへ配置（左から右の場合は左寄せ）
            //sf.LineAlignment = StringAlignment.Near;         // Textを近くへ配置（上寄せ）
            //sf.FormatFlags = StringFormatFlags.LineLimit;    // 
            sfTab.Alignment = StringAlignment.Center;
            sfTab.LineAlignment = StringAlignment.Center;

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

            if (this._cfgCommon.CountApi < 20 || this._cfgCommon.CountApi > 200)
                this._cfgCommon.CountApi = 60;
            if (this._cfgCommon.CountApiReply < 20 || this._cfgCommon.CountApiReply > 200)
                this._cfgCommon.CountApiReply = 40;

            HttpTwitter.TwitterUrl = _cfgCommon.TwitterUrl;

            //認証関連
            if (string.IsNullOrEmpty(_cfgCommon.Token)) _cfgCommon.UserName = "";
            tw.Initialize(_cfgCommon.Token, _cfgCommon.TokenSecret, _cfgCommon.UserName, _cfgCommon.UserId);

            //新着取得時のリストスクロールをするか。trueならスクロールしない
            ListLockMenuItem.Checked = _cfgCommon.ListLock;
            this.LockListFileMenuItem.Checked = _cfgCommon.ListLock;
            //サウンド再生（タブ別設定より優先）
            this.PlaySoundMenuItem.Checked = this._cfgCommon.PlaySound;
            this.PlaySoundFileMenuItem.Checked = this._cfgCommon.PlaySound;

            //廃止サービスが選択されていた場合bit.lyへ読み替え
            if (_cfgCommon.AutoShortUrlFirst < 0)
                _cfgCommon.AutoShortUrlFirst = MyCommon.UrlConverter.Uxnu;

            AtIdSupl = new AtIdSupplement(SettingAtIdList.Load().AtIdList, "@");

            this.IdeographicSpaceToSpaceToolStripMenuItem.Checked = _cfgCommon.WideSpaceConvert;
            this.ToolStripFocusLockMenuItem.Checked = _cfgCommon.FocusLockToStatusText;

            //Regex statregex = new Regex("^0*");
            this.recommendedStatusFooter = " [TWNv" + Regex.Replace(MyCommon.FileVersion.Replace(".", ""), "^0*", "") + "]";

            //ハッシュタグ関連
            HashSupl = new AtIdSupplement(_cfgCommon.HashTags, "#");
            HashMgr = new HashtagManage(HashSupl,
                                    _cfgCommon.HashTags.ToArray(),
                                    _cfgCommon.HashSelected,
                                    _cfgCommon.HashIsPermanent,
                                    _cfgCommon.HashIsHead,
                                    _cfgCommon.HashIsNotAddToAtReply);
            if (!string.IsNullOrEmpty(HashMgr.UseHash) && HashMgr.IsPermanent) HashStripSplitButton.Text = HashMgr.UseHash;

            _initial = true;

            Networking.Initialize();

            //アイコンリスト作成
            this.IconCache = new ImageCache();

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

                //新しい設定を反映
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
            }

            _brsBackColorMine = new SolidBrush(_clSelf);
            _brsBackColorAt = new SolidBrush(_clAtSelf);
            _brsBackColorYou = new SolidBrush(_clTarget);
            _brsBackColorAtYou = new SolidBrush(_clAtTarget);
            _brsBackColorAtFromTarget = new SolidBrush(_clAtFromTarget);
            _brsBackColorAtTo = new SolidBrush(_clAtTo);
            //_brsBackColorNone = new SolidBrush(Color.FromKnownColor(KnownColor.Window));
            _brsBackColorNone = new SolidBrush(_clListBackcolor);

            InitDetailHtmlFormat();

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

            //Twitter用通信クラス初期化
            Networking.DefaultTimeout = TimeSpan.FromSeconds(this._cfgCommon.DefaultTimeOut);
            Networking.SetWebProxy(this._cfgLocal.ProxyType,
                this._cfgLocal.ProxyAddress, this._cfgLocal.ProxyPort,
                this._cfgLocal.ProxyUser, this._cfgLocal.ProxyPassword);

            //サムネイル関連の初期化
            //プロキシ設定等の通信まわりの初期化が済んでから処理する
            ThumbnailGenerator.InitializeGenerator();

            var imgazyobizinet = ThumbnailGenerator.ImgAzyobuziNetInstance;
            imgazyobizinet.Enabled = this._cfgCommon.EnableImgAzyobuziNet;
            imgazyobizinet.DisabledInDM = this._cfgCommon.ImgAzyobuziNetDisabledInDM;

            Thumbnail.Services.TonTwitterCom.InitializeOAuthToken = x =>
                x.Initialize(ApplicationSettings.TwitterConsumerKey, ApplicationSettings.TwitterConsumerSecret,
                    this.tw.AccessToken, this.tw.AccessTokenSecret, "", "");

            tw.RestrictFavCheck = this._cfgCommon.RestrictFavCheck;
            tw.ReadOwnPost = this._cfgCommon.ReadOwnPost;
            ShortUrl.Instance.DisableExpanding = !this._cfgCommon.TinyUrlResolve;
            ShortUrl.Instance.BitlyId = this._cfgCommon.BilyUser;
            ShortUrl.Instance.BitlyKey = this._cfgCommon.BitlyPwd;
            HttpTwitter.TwitterUrl = _cfgCommon.TwitterUrl;
            tw.TrackWord = _cfgCommon.TrackWord;
            TrackToolStripMenuItem.Checked = !String.IsNullOrEmpty(tw.TrackWord);
            tw.AllAtReply = _cfgCommon.AllAtReply;
            AllrepliesToolStripMenuItem.Checked = tw.AllAtReply;

            //画像投稿サービス
            ImageSelector.Initialize(tw, this.tw.Configuration, _cfgCommon.UseImageServiceName, _cfgCommon.UseImageService);

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

            // NameLabel のフォントを OTBaseForm.GlobalFont に変更
            this.NameLabel.Font = this.ReplaceToGlobalFont(this.NameLabel.Font);

            // 必要であれば、発言一覧と発言詳細部・入力欄の上下を入れ替える
            SplitContainer1.IsPanelInverted = !this._cfgCommon.StatusAreaAtBottom;

            //全新着通知のチェック状態により、Reply＆DMの新着通知有効無効切り替え（タブ別設定にするため削除予定）
            if (this._cfgCommon.UnreadManage == false)
            {
                ReadedStripMenuItem.Enabled = false;
                UnreadStripMenuItem.Enabled = false;
            }

            if (this._cfgCommon.IsUseNotifyGrowl)
                gh.RegisterGrowl();

            //タイマー設定
            TimerTimeline.AutoReset = true;
            TimerTimeline.SynchronizingObject = this;
            //Recent取得間隔
            TimerTimeline.Interval = 1000;
            TimerTimeline.Enabled = true;

            //更新中アイコンアニメーション間隔
            TimerRefreshIcon.Interval = 200;
            TimerRefreshIcon.Enabled = true;

            //状態表示部の初期化（画面右下）
            StatusLabel.Text = "";
            StatusLabel.AutoToolTip = false;
            StatusLabel.ToolTipText = "";
            //文字カウンタ初期化
            lblLen.Text = GetRestStatusCount(true, false).ToString();

            ////////////////////////////////////////////////////////////////////////////////
            _statuses.SortOrder = (SortOrder)_cfgCommon.SortOrder;
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
            _statuses.SortMode = mode;
            ////////////////////////////////////////////////////////////////////////////////

            ApplyListViewIconSize(this._cfgCommon.IconSize);

            StatusLabel.Text = Properties.Resources.Form1_LoadText1;       //画面右下の状態表示を変更
            StatusLabelUrl.Text = "";            //画面左下のリンク先URL表示部を初期化
            NameLabel.Text = "";                 //発言詳細部名前ラベル初期化
            DateTimeLabel.Text = "";             //発言詳細部日時ラベル初期化
            SourceLinkLabel.Text = "";           //Source部分初期化

            //<<<<<<<<タブ関連>>>>>>>
            //デフォルトタブの存在チェック、ない場合には追加
            if (_statuses.GetTabByType(MyCommon.TabUsageType.Home) == null)
            {
                TabClass tab;
                if (!_statuses.Tabs.TryGetValue(MyCommon.DEFAULTTAB.RECENT, out tab))
                {
                    _statuses.AddTab(MyCommon.DEFAULTTAB.RECENT, MyCommon.TabUsageType.Home, null);
                }
                else
                {
                    tab.TabType = MyCommon.TabUsageType.Home;
                }
            }
            if (_statuses.GetTabByType(MyCommon.TabUsageType.Mentions) == null)
            {
                TabClass tab;
                if (!_statuses.Tabs.TryGetValue(MyCommon.DEFAULTTAB.REPLY, out tab))
                {
                    _statuses.AddTab(MyCommon.DEFAULTTAB.REPLY, MyCommon.TabUsageType.Mentions, null);
                }
                else
                {
                    tab.TabType = MyCommon.TabUsageType.Mentions;
                }
            }
            if (_statuses.GetTabByType(MyCommon.TabUsageType.DirectMessage) == null)
            {
                TabClass tab;
                if (!_statuses.Tabs.TryGetValue(MyCommon.DEFAULTTAB.DM, out tab))
                {
                    _statuses.AddTab(MyCommon.DEFAULTTAB.DM, MyCommon.TabUsageType.DirectMessage, null);
                }
                else
                {
                    tab.TabType = MyCommon.TabUsageType.DirectMessage;
                }
            }
            if (_statuses.GetTabByType(MyCommon.TabUsageType.Favorites) == null)
            {
                TabClass tab;
                if (!_statuses.Tabs.TryGetValue(MyCommon.DEFAULTTAB.FAV, out tab))
                {
                    _statuses.AddTab(MyCommon.DEFAULTTAB.FAV, MyCommon.TabUsageType.Favorites, null);
                }
                else
                {
                    tab.TabType = MyCommon.TabUsageType.Favorites;
                }
            }
            if (_statuses.GetTabByType(MyCommon.TabUsageType.Mute) == null)
            {
                TabClass tab;
                if (!_statuses.Tabs.TryGetValue(MyCommon.DEFAULTTAB.MUTE, out tab))
                {
                    _statuses.AddTab(MyCommon.DEFAULTTAB.MUTE, MyCommon.TabUsageType.Mute, null);
                }
                else
                {
                    tab.TabType = MyCommon.TabUsageType.Mute;
                }
            }

            foreach (var tab in _statuses.Tabs.Values)
            {
                // ミュートタブは表示しない
                if (tab.TabType == MyCommon.TabUsageType.Mute)
                    continue;

                if (tab.TabType == MyCommon.TabUsageType.Undefined)
                {
                    tab.TabType = MyCommon.TabUsageType.UserDefined;
                }
                if (!AddNewTab(tab.TabName, true, tab.TabType, tab.ListInfo))
                    throw new TabException(Properties.Resources.TweenMain_LoadText1);
            }

            this.JumpReadOpMenuItem.ShortcutKeyDisplayString = "Space";
            CopySTOTMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            CopyURLMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            CopyUserIdStripMenuItem.ShortcutKeyDisplayString = "Shift+Alt+C";

            if (!this._cfgCommon.MinimizeToTray || this.WindowState != FormWindowState.Minimized)
            {
                this.Visible = true;
            }
            _curTab = ListTab.SelectedTab;
            _curItemIndex = -1;
            _curList = (DetailsListView)_curTab.Tag;
            SetMainWindowTitle();
            SetNotifyIconText();

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

            _ignoreConfigSave = false;
            this.TweenMain_Resize(null, null);
            if (saveRequired) SaveConfigsAll(false);

            if (tw.UserId == 0)
                tw.VerifyCredentials();

            foreach (var ua in this._cfgCommon.UserAccounts)
            {
                if (ua.UserId == 0 && ua.Username.ToLower() == tw.Username.ToLower())
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

            // タブの位置を調整する
            SetTabAlignment();
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
                    .Replace("%FONT_COLOR%", _clDetail.R.ToString() + "," + _clDetail.G.ToString() + "," + _clDetail.B.ToString())
                    .Replace("%LINK_COLOR%", _clDetailLink.R.ToString() + "," + _clDetailLink.G.ToString() + "," + _clDetailLink.B.ToString())
                    .Replace("%BG_COLOR%", _clDetailBackcolor.R.ToString() + "," + _clDetailBackcolor.G.ToString() + "," + _clDetailBackcolor.B.ToString());
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

            List<TabClass> tabs = SettingTabs.Load().Tabs;
            foreach (TabClass tb in tabs)
            {
                try
                {
                    tb.FilterModified = false;
                    _statuses.Tabs.Add(tb.TabName, tb);
                }
                catch (Exception)
                {
                    tb.TabName = _statuses.GetUniqueTabName();
                    _statuses.Tabs.Add(tb.TabName, tb);
                }
            }
            if (_statuses.Tabs.Count == 0)
            {
                _statuses.AddTab(MyCommon.DEFAULTTAB.RECENT, MyCommon.TabUsageType.Home, null);
                _statuses.AddTab(MyCommon.DEFAULTTAB.REPLY, MyCommon.TabUsageType.Mentions, null);
                _statuses.AddTab(MyCommon.DEFAULTTAB.DM, MyCommon.TabUsageType.DirectMessage, null);
                _statuses.AddTab(MyCommon.DEFAULTTAB.FAV, MyCommon.TabUsageType.Favorites, null);
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
                if (this._isActiveUserstream)
                {
                    refreshTasks.Add(this.RefreshTasktrayIcon(true));
                    this.RefreshTimeline(true);
                }
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

        private void RefreshTimeline(bool isUserStream)
        {
            //スクロール制御準備
            int smode = -1;    //-1:制御しない,-2:最新へ,その他:topitem使用
            long topId = GetScrollPos(ref smode);
            int befCnt = _curList.VirtualListSize;

            //現在の選択状態を退避
            var selId = new Dictionary<string, long[]>();
            var focusedId = new Dictionary<string, Tuple<long, long>>();
            SaveSelectedStatus(selId, focusedId);

            //mentionsの更新前件数を保持
            int dmCount = _statuses.GetTabByType(MyCommon.TabUsageType.DirectMessage).AllCount;

            //更新確定
            PostClass[] notifyPosts = null;
            string soundFile = "";
            int addCount = 0;
            bool isMention = false;
            bool isDelete = false;
            addCount = _statuses.SubmitUpdate(ref soundFile, ref notifyPosts, ref isMention, ref isDelete, isUserStream);

            if (MyCommon._endingFlag) return;

            //リストに反映＆選択状態復元
            try
            {
                foreach (TabPage tab in ListTab.TabPages)
                {
                    DetailsListView lst = (DetailsListView)tab.Tag;
                    TabClass tabInfo = _statuses.Tabs[tab.Text];
                    using (ControlTransaction.Update(lst))
                    {
                        if (isDelete || lst.VirtualListSize != tabInfo.AllCount)
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

                            // status_id から ListView 上のインデックスに変換
                            var selectedIndices = selId[tab.Text] != null
                                ? tabInfo.IndexOf(selId[tab.Text]).Where(x => x != -1).ToArray()
                                : null;
                            var focusedIndex = tabInfo.IndexOf(focusedId[tab.Text].Item1);
                            var selectionMarkIndex = tabInfo.IndexOf(focusedId[tab.Text].Item2);

                            this.SelectListItem(lst, selectedIndices, focusedIndex, selectionMarkIndex);
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

            //スクロール制御後処理
            if (smode != -1)
            {
                try
                {
                    if (befCnt != _curList.VirtualListSize)
                    {
                        switch (smode)
                        {
                            case -3:
                                //最上行
                                if (_curList.VirtualListSize > 0) _curList.EnsureVisible(0);
                                break;
                            case -2:
                                //最下行へ
                                if (_curList.VirtualListSize > 0) _curList.EnsureVisible(_curList.VirtualListSize - 1);
                                break;
                            case -1:
                                //制御しない
                                break;
                            default:
                                //表示位置キープ
                                if (_curList.VirtualListSize > 0 && _statuses.Tabs[_curTab.Text].IndexOf(topId) > -1)
                                {
                                    _curList.EnsureVisible(_curList.VirtualListSize - 1);
                                    _curList.EnsureVisible(_statuses.Tabs[_curTab.Text].IndexOf(topId));
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Data["Msg"] = "Ref2";
                    throw;
                }
            }

            //新着通知
            NotifyNewPosts(notifyPosts,
                           soundFile,
                           addCount,
                           isMention || dmCount != _statuses.GetTabByType(MyCommon.TabUsageType.DirectMessage).AllCount);

            SetMainWindowTitle();
            if (!StatusLabelUrl.Text.StartsWith("http")) SetStatusLabelUrl();

            HashSupl.AddRangeItem(tw.GetHashList());

        }

        private long GetScrollPos(ref int smode)
        {
            long topId = -1;
            if (_curList != null && _curTab != null && _curList.VirtualListSize > 0)
            {
                if (_statuses.SortMode == ComparerMode.Id)
                {
                    if (_statuses.SortOrder == SortOrder.Ascending)
                    {
                        //Id昇順
                        if (ListLockMenuItem.Checked)
                        {
                            //制御しない
                            smode = -1;
                            ////現在表示位置へ強制スクロール
                            //if (_curList.TopItem != null) topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index);
                            //smode = 0;
                        }
                        else
                        {
                            //最下行が表示されていたら、最下行へ強制スクロール。最下行が表示されていなかったら制御しない
                            ListViewItem _item;
                            _item = _curList.GetItemAt(0, _curList.ClientSize.Height - 1);   //一番下
                            if (_item == null) _item = _curList.Items[_curList.VirtualListSize - 1];
                            if (_item.Index == _curList.VirtualListSize - 1)
                            {
                                smode = -2;
                            }
                            else
                            {
                                smode = -1;
                                //if (_curList.TopItem != null) topId = _statuses.GetId(_curTab.Text, _curList.TopItem.Index);
                                //smode = 0;
                            }
                        }
                    }
                    else
                    {
                        //Id降順
                        if (ListLockMenuItem.Checked)
                        {
                            //現在表示位置へ強制スクロール
                            if (_curList.TopItem != null) topId = _statuses.Tabs[_curTab.Text].GetId(_curList.TopItem.Index);
                            smode = 0;
                        }
                        else
                        {
                            //最上行が表示されていたら、制御しない。最上行が表示されていなかったら、現在表示位置へ強制スクロール
                            ListViewItem _item;

                            _item = _curList.GetItemAt(0, 10);     //一番上
                            if (_item == null) _item = _curList.Items[0];
                            if (_item.Index == 0)
                            {
                                smode = -3;  //最上行
                            }
                            else
                            {
                                if (_curList.TopItem != null) topId = _statuses.Tabs[_curTab.Text].GetId(_curList.TopItem.Index);
                                smode = 0;
                            }
                        }
                    }
                }
                else
                {
                    //現在表示位置へ強制スクロール
                    if (_curList.TopItem != null) topId = _statuses.Tabs[_curTab.Text].GetId(_curList.TopItem.Index);
                    smode = 0;
                }
            }
            else
            {
                smode = -1;
            }
            return topId;
        }

        private void SaveSelectedStatus(Dictionary<string, long[]> selId, Dictionary<string, Tuple<long, long>> focusedIdDict)
        {
            if (MyCommon._endingFlag) return;
            foreach (TabPage tab in ListTab.TabPages)
            {
                var lst = (DetailsListView)tab.Tag;
                var tabInfo = _statuses.Tabs[tab.Text];
                if (lst.SelectedIndices.Count > 0 && lst.SelectedIndices.Count < 61)
                {
                    selId.Add(tab.Text, tabInfo.GetId(lst.SelectedIndices));
                }
                else
                {
                    selId.Add(tab.Text, null);
                }

                var focusedItem = lst.FocusedItem;
                var focusedId = focusedItem != null ? tabInfo.GetId(focusedItem.Index) : -2;

                var selectionMarkIndex = lst.SelectionMark;
                var selectionMarkId = selectionMarkIndex != -1 ? tabInfo.GetId(selectionMarkIndex) : -2;

                focusedIdDict[tab.Text] = Tuple.Create(focusedId, selectionMarkId);
            }

        }

        private bool BalloonRequired()
        {
            Twitter.FormattedEvent ev = new Twitter.FormattedEvent();
            ev.Eventtype = MyCommon.EVENTTYPE.None;

            return BalloonRequired(ev);
        }

        private bool IsEventNotifyAsEventType(MyCommon.EVENTTYPE type)
        {
            return this._cfgCommon.EventNotifyEnabled && (type & this._cfgCommon.EventNotifyFlag) != 0 || type == MyCommon.EVENTTYPE.None;
        }

        private bool IsMyEventNotityAsEventType(Twitter.FormattedEvent ev)
        {
            return (ev.Eventtype & this._cfgCommon.IsMyEventNotifyFlag) != 0 ? true : !ev.IsMe;
        }

        private bool BalloonRequired(Twitter.FormattedEvent ev)
        {
            if ((
                IsEventNotifyAsEventType(ev.Eventtype) && IsMyEventNotityAsEventType(ev) &&
                (NewPostPopMenuItem.Checked || (this._cfgCommon.ForceEventNotify && ev.Eventtype != MyCommon.EVENTTYPE.None)) &&
                !_initial &&
                (
                    (
                        this._cfgCommon.LimitBalloon &&
                        (
                            this.WindowState == FormWindowState.Minimized ||
                            !this.Visible ||
                            Form.ActiveForm == null
                            )
                        ) ||
                    !this._cfgCommon.LimitBalloon
                    )
                ) &&
                !NativeMethods.IsScreenSaverRunning())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void NotifyNewPosts(PostClass[] notifyPosts, string soundFile, int addCount, bool newMentions)
        {
            if (notifyPosts != null &&
                notifyPosts.Length > 0 &&
                this._cfgCommon.ReadOwnPost &&
                notifyPosts.All((post) => { return post.UserId == tw.UserId || post.ScreenName == tw.Username; }))
            {
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
                                title.Append(Properties.Resources.RefreshDirectMessageText1);
                                title.Append(" ");
                                title.Append(addCount);
                                title.Append(Properties.Resources.RefreshDirectMessageText2);
                                nt = GrowlHelper.NotifyType.DirectMessage;
                            }
                            else if (reply)
                            {
                                //NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                                //NotifyIcon1.BalloonTipTitle += Application.ProductName + " [Reply!] " + Properties.Resources.RefreshTimelineText1 + " " + addCount.ToString() + Properties.Resources.RefreshTimelineText2;
                                title.Append(Application.ProductName);
                                title.Append(" [Reply!] ");
                                title.Append(Properties.Resources.RefreshTimelineText1);
                                title.Append(" ");
                                title.Append(addCount);
                                title.Append(Properties.Resources.RefreshTimelineText2);
                                nt = GrowlHelper.NotifyType.Reply;
                            }
                            else
                            {
                                //NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                                //NotifyIcon1.BalloonTipTitle += Application.ProductName + " " + Properties.Resources.RefreshTimelineText1 + " " + addCount.ToString() + Properties.Resources.RefreshTimelineText2;
                                title.Append(Application.ProductName);
                                title.Append(" ");
                                title.Append(Properties.Resources.RefreshTimelineText1);
                                title.Append(" ");
                                title.Append(addCount);
                                title.Append(Properties.Resources.RefreshTimelineText2);
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
                            title.Append(Properties.Resources.RefreshDirectMessageText1);
                            title.Append(" ");
                            title.Append(addCount);
                            title.Append(Properties.Resources.RefreshDirectMessageText2);
                        }
                        else if (reply)
                        {
                            //NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                            //NotifyIcon1.BalloonTipTitle += Application.ProductName + " [Reply!] " + Properties.Resources.RefreshTimelineText1 + " " + addCount.ToString() + Properties.Resources.RefreshTimelineText2;
                            ntIcon = ToolTipIcon.Warning;
                            title.Append(Application.ProductName);
                            title.Append(" [Reply!] ");
                            title.Append(Properties.Resources.RefreshTimelineText1);
                            title.Append(" ");
                            title.Append(addCount);
                            title.Append(Properties.Resources.RefreshTimelineText2);
                        }
                        else
                        {
                            //NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                            //NotifyIcon1.BalloonTipTitle += Application.ProductName + " " + Properties.Resources.RefreshTimelineText1 + " " + addCount.ToString() + Properties.Resources.RefreshTimelineText2;
                            ntIcon = ToolTipIcon.Info;
                            title.Append(Application.ProductName);
                            title.Append(" ");
                            title.Append(Properties.Resources.RefreshTimelineText1);
                            title.Append(" ");
                            title.Append(addCount);
                            title.Append(Properties.Resources.RefreshTimelineText2);
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
                _curPost = GetCurTabPost(_curItemIndex);
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

            //対象の特定
            ListViewItem itm = null;
            PostClass post = null;

            this.TryGetListViewItemCache(Index, out itm, out post);

            // キャッシュに含まれていないアイテムは対象外
            if (itm == null)
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

            var itemColors = new Color[] { };
            int itemIndex = -1;

            this.itemCacheLock.EnterReadLock();
            try
            {
                if (this._itemCache == null) return;

                var query = 
                    from i in Enumerable.Range(0, this._itemCache.Length)
                    select this.JudgeColor(_post, this._postCache[i]);
                
                itemColors = query.ToArray();
                itemIndex = _itemCacheIndex;
            }
            finally { this.itemCacheLock.ExitReadLock(); }

            if (itemIndex < 0) return;

            foreach (var backColor in itemColors)
            {
                // この処理中に MyList_CacheVirtualItems が呼ばれることがあるため、
                // 同一スレッド内での二重ロックを避けるためにロックの外で実行する必要がある
                _curList.ChangeItemBackColor(itemIndex++, backColor);
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
            else if (BasePost.ReplyToList.Contains(TargetPost.ScreenName.ToLower()))
                //返信先
                cl = _clAtFromTarget;
            else if (TargetPost.ReplyToList.Contains(BasePost.ScreenName.ToLower()))
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
                    this.DoRefresh();
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

            _history[_history.Count - 1] = new PostingStatus(StatusText.Text, _reply_to_id, _reply_to_name);

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

            //整形によって増加する文字数を取得
            int adjustCount = 0;
            string tmpStatus = StatusText.Text.Trim();
            if (ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(tmpStatus,
                    @"^[+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\*)([+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]+|$)",
                    RegexOptions.IgnoreCase)
                   && tmpStatus.EndsWith(" .") == false) adjustCount += 2;
            }

            if (ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                adjustCount += Regex.Matches(tmpStatus, @"https?:\/\/[-_.!~*'()a-zA-Z0-9;\/?:\@&=+\$,%#^]+").Count;
            }

            bool isCutOff = false;
            bool isRemoveFooter = MyCommon.IsKeyDown(Keys.Shift);
            if (StatusText.Multiline && !this._cfgCommon.PostCtrlEnter)
            {
                //複数行でEnter投稿の場合、Ctrlも押されていたらフッタ付加しない
                isRemoveFooter = MyCommon.IsKeyDown(Keys.Control);
            }
            if (this._cfgCommon.PostShiftEnter)
            {
                isRemoveFooter = MyCommon.IsKeyDown(Keys.Control);
            }
            if (!isRemoveFooter && (StatusText.Text.Contains("RT @") || StatusText.Text.Contains("QT @")))
            {
                isRemoveFooter = true;
            }
            if (GetRestStatusCount(false, !isRemoveFooter) - adjustCount < 0)
            {
                if (MessageBox.Show(Properties.Resources.PostLengthOverMessage1, Properties.Resources.PostLengthOverMessage2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    isCutOff = true;
                    //if (!SettingDialog.UrlConvertAuto) UrlConvertAutoToolStripMenuItem_Click(null, null);
                    if (GetRestStatusCount(false, !isRemoveFooter) - adjustCount < 0)
                    {
                        isRemoveFooter = true;
                    }
                }
                else
                {
                    return;
                }
            }

            string footer = "";
            string header = "";
            if (StatusText.Text.StartsWith("D ") || StatusText.Text.StartsWith("d "))
            {
                //DM時は何もつけない
                footer = "";
            }
            else
            {
                //ハッシュタグ
                if (HashMgr.IsNotAddToAtReply)
                {
                    if (!string.IsNullOrEmpty(HashMgr.UseHash) && _reply_to_id == null && string.IsNullOrEmpty(_reply_to_name))
                    {
                        if (HashMgr.IsHead)
                            header = HashMgr.UseHash + " ";
                        else
                            footer = " " + HashMgr.UseHash;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(HashMgr.UseHash))
                    {
                        if (HashMgr.IsHead)
                            header = HashMgr.UseHash + " ";
                        else
                            footer = " " + HashMgr.UseHash;
                    }
                }
                if (!isRemoveFooter)
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
            }

            var status = new PostingStatus();
            status.status = header + StatusText.Text + footer;

            if (ToolStripMenuItemApiCommandEvasion.Checked)
            {
                // APIコマンド回避
                if (Regex.IsMatch(status.status,
                    @"^[+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]*(get|g|fav|follow|f|on|off|stop|quit|leave|l|whois|w|nudge|n|stats|invite|track|untrack|tracks|tracking|\*)([+\-\[\]\s\\.,*/(){}^~|='&%$#""<>?]+|$)",
                    RegexOptions.IgnoreCase)
                   && status.status.EndsWith(" .") == false) status.status += " .";
            }

            if (ToolStripMenuItemUrlMultibyteSplit.Checked)
            {
                // URLと全角文字の切り離し
                Match mc2 = Regex.Match(status.status, @"https?:\/\/[-_.!~*'()a-zA-Z0-9;\/?:\@&=+\$,%#^]+");
                if (mc2.Success) status.status = Regex.Replace(status.status, @"https?:\/\/[-_.!~*'()a-zA-Z0-9;\/?:\@&=+\$,%#^]+", "$& ");
            }

            if (IdeographicSpaceToSpaceToolStripMenuItem.Checked)
            {
                // 文中の全角スペースを半角スペース1個にする
                status.status = status.status.Replace("　", " ");
            }

            if (isCutOff && status.status.Length > 140)
            {
                status.status = status.status.Substring(0, 140);
                string AtId = @"(@|＠)[a-z0-9_/]+$";
                string HashTag = @"(^|[^0-9A-Z&\/\?]+)(#|＃)([0-9A-Z_]*[A-Z_]+)$";
                string Url = @"https?:\/\/[a-z0-9!\*'\(\);:&=\+\$\/%#\[\]\-_\.,~?]+$"; //簡易判定
                string pattern = string.Format("({0})|({1})|({2})", AtId, HashTag, Url);
                Match mc = Regex.Match(status.status, pattern, RegexOptions.IgnoreCase);
                if (mc.Success)
                {
                    //さらに@ID、ハッシュタグ、URLと推測される文字列をカットする
                    status.status = status.status.Substring(0, 140 - mc.Value.Length);
                }
                if (MessageBox.Show(status.status, "Post or Cancel?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) return;
            }

            status.inReplyToId = _reply_to_id;
            status.inReplyToName = _reply_to_name;
            if (ImageSelector.Visible)
            {
                //画像投稿
                if (!ImageSelector.TryGetSelectedMedia(out status.imageService, out status.imagePath))
                    return;
            }

            _reply_to_id = null;
            _reply_to_name = null;
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
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                await this.GetHomeTimelineAsyncInternal(progress, this.workerCts.Token, loadMore);
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

        private async Task GetHomeTimelineAsyncInternal(IProgress<string> p, CancellationToken ct, bool loadMore)
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

            p.Report(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText5 +
                (loadMore ? "-1" : "1") +
                Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6);

            await Task.Run(() =>
            {
                var err = this.tw.GetTimelineApi(read, MyCommon.WORKERTYPE.Timeline, loadMore, this._initial);

                if (!string.IsNullOrEmpty(err))
                    throw new WebApiException(err);

                // 新着時未読クリア
                if (this._cfgCommon.ReadOldPosts)
                    this._statuses.SetReadHomeTab();

                var addCount = this._statuses.DistributePosts();

                if (!this._initial)
                {
                    lock (this._syncObject)
                    {
                        var tm = DateTime.Now;
                        if (this._tlTimestamps.ContainsKey(tm))
                            this._tlTimestamps[tm] += addCount;
                        else
                            this._tlTimestamps[tm] = addCount;

                        var removeKeys = new List<DateTime>();
                        var oneHour = DateTime.Now - TimeSpan.FromHours(1);

                        this._tlCount = 0;
                        foreach (var pair in this._tlTimestamps)
                        {
                            if (pair.Key < oneHour)
                                removeKeys.Add(pair.Key);
                            else
                                this._tlCount += pair.Value;
                        }

                        foreach (var key in removeKeys)
                            this._tlTimestamps.Remove(key);
                    }
                }
            });

            if (ct.IsCancellationRequested)
                return;

            p.Report(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText1);

            this.RefreshTimeline(false);
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
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                await this.GetReplyAsyncInternal(progress, this.workerCts.Token, loadMore);
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

        private async Task GetReplyAsyncInternal(IProgress<string> p, CancellationToken ct, bool loadMore)
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

            p.Report(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText4 +
                (loadMore ? "-1" : "1") +
                Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6);

            await Task.Run(() =>
            {
                var err = this.tw.GetTimelineApi(read, MyCommon.WORKERTYPE.Reply, loadMore, this._initial);

                if (!string.IsNullOrEmpty(err))
                    throw new WebApiException(err);

                this._statuses.DistributePosts();
            });

            if (ct.IsCancellationRequested)
                return;

            p.Report(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText9);

            this.RefreshTimeline(false);
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
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                await this.GetDirectMessagesAsyncInternal(progress, this.workerCts.Token, loadMore);
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

        private async Task GetDirectMessagesAsyncInternal(IProgress<string> p, CancellationToken ct, bool loadMore)
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

            p.Report(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText8 +
                (loadMore ? "-1" : "1") +
                Properties.Resources.GetTimelineWorker_RunWorkerCompletedText6);

            await Task.Run(() =>
            {
                var err = this.tw.GetDirectMessageApi(read, MyCommon.WORKERTYPE.DirectMessegeRcv, loadMore);
                if (!string.IsNullOrEmpty(err))
                    throw new WebApiException(err);

                var err2 = this.tw.GetDirectMessageApi(read, MyCommon.WORKERTYPE.DirectMessegeSnt, loadMore);
                if (!string.IsNullOrEmpty(err2))
                    throw new WebApiException(err2);

                this._statuses.DistributePosts();
            });

            if (ct.IsCancellationRequested)
                return;

            p.Report(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText11);

            this.RefreshTimeline(false);
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
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                await this.GetFavoritesAsyncInternal(progress, this.workerCts.Token, loadMore);
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

        private async Task GetFavoritesAsyncInternal(IProgress<string> p, CancellationToken ct, bool loadMore)
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

            p.Report(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText19);

            await Task.Run(() =>
            {
                var err = this.tw.GetFavoritesApi(read, MyCommon.WORKERTYPE.Favorites, loadMore);

                if (!string.IsNullOrEmpty(err))
                    throw new WebApiException(err);

                this._statuses.DistributePosts();
            });

            if (ct.IsCancellationRequested)
                return;

            p.Report(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText20);

            this.RefreshTimeline(false);
        }

        private Task GetPublicSearchAllAsync()
        {
            return this.GetPublicSearchAsync(null, loadMore: false);
        }

        private Task GetPublicSearchAsync(TabClass tab)
        {
            return this.GetPublicSearchAsync(tab, loadMore: false);
        }

        private async Task GetPublicSearchAsync(TabClass tab, bool loadMore)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                var tabs = tab != null
                    ? new[] { tab }.AsEnumerable()
                    : this._statuses.GetTabsByType(MyCommon.TabUsageType.PublicSearch);

                await this.GetPublicSearchAsyncInternal(progress, this.workerCts.Token, tabs, loadMore);
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

        private async Task GetPublicSearchAsyncInternal(IProgress<string> p, CancellationToken ct, IEnumerable<TabClass> tabs, bool loadMore)
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

            p.Report("Search refreshing...");

            await Task.Run(() =>
            {
                foreach (var tab in tabs)
                {
                    if (string.IsNullOrEmpty(tab.SearchWords))
                        continue;

                    var err = this.tw.GetSearch(read, tab, false);
                    if (!string.IsNullOrEmpty(err))
                        throw new WebApiException(err);

                    if (loadMore)
                    {
                        var err2 = this.tw.GetSearch(read, tab, true);
                        if (!string.IsNullOrEmpty(err2))
                            throw new WebApiException(err2);
                    }
                }

                this._statuses.DistributePosts();
            });

            if (ct.IsCancellationRequested)
                return;

            p.Report("Search refreshed");

            this.RefreshTimeline(false);
        }

        private Task GetUserTimelineAllAsync()
        {
            return this.GetUserTimelineAsync(null, loadMore: false);
        }

        private Task GetUserTimelineAsync(TabClass tab)
        {
            return this.GetUserTimelineAsync(tab, loadMore: false);
        }

        private async Task GetUserTimelineAsync(TabClass tab, bool loadMore)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                var tabs = tab != null
                    ? new[] { tab }.AsEnumerable()
                    : this._statuses.GetTabsByType(MyCommon.TabUsageType.UserTimeline);

                await this.GetUserTimelineAsyncInternal(progress, this.workerCts.Token, tabs, loadMore);
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

        private async Task GetUserTimelineAsyncInternal(IProgress<string> p, CancellationToken ct, IEnumerable<TabClass> tabs, bool loadMore)
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

            p.Report("UserTimeline refreshing...");

            await Task.Run(() =>
            {
                var count = 20;
                if (this._cfgCommon.UseAdditionalCount)
                    count = this._cfgCommon.UserTimelineCountApi;

                foreach (var tab in tabs)
                {
                    if (string.IsNullOrEmpty(tab.User))
                        continue;

                    var err = this.tw.GetUserTimelineApi(read, count, tab.User, tab, loadMore);
                    if (!string.IsNullOrEmpty(err))
                        throw new WebApiException(err);
                }

                this._statuses.DistributePosts();
            });

            if (ct.IsCancellationRequested)
                return;

            p.Report("UserTimeline refreshed");

            this.RefreshTimeline(false);
        }

        private Task GetListTimelineAllAsync()
        {
            return this.GetListTimelineAsync(null, loadMore: false);
        }

        private Task GetListTimelineAsync(TabClass tab)
        {
            return this.GetListTimelineAsync(tab, loadMore: false);
        }

        private async Task GetListTimelineAsync(TabClass tab, bool loadMore)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                var tabs = tab != null
                    ? new[] { tab }.AsEnumerable()
                    : this._statuses.GetTabsByType(MyCommon.TabUsageType.Lists);

                await this.GetListTimelineAsyncInternal(progress, this.workerCts.Token, tabs, loadMore);
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

        private async Task GetListTimelineAsyncInternal(IProgress<string> p, CancellationToken ct, IEnumerable<TabClass> tabs, bool loadMore)
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

            p.Report("List refreshing...");

            await Task.Run(() =>
            {
                foreach (var tab in tabs)
                {
                    if (tab.ListInfo == null || tab.ListInfo.Id == 0)
                        continue;

                    var err = this.tw.GetListStatus(read, tab, loadMore, this._initial);
                    if (!string.IsNullOrEmpty(err))
                        throw new WebApiException(err);
                }

                this._statuses.DistributePosts();
            });

            if (ct.IsCancellationRequested)
                return;

            p.Report("List refreshed");

            this.RefreshTimeline(false);
        }

        private async Task GetRelatedTweetsAsync(TabClass tab)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                await this.GetRelatedTweetsAsyncInternal(progress, this.workerCts.Token, tab);
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

        private async Task GetRelatedTweetsAsyncInternal(IProgress<string> p, CancellationToken ct, TabClass tab)
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

            p.Report("Related refreshing...");

            await Task.Run(() =>
            {
                var err = this.tw.GetRelatedResult(read, tab);
                if (!string.IsNullOrEmpty(err))
                    throw new WebApiException(err);

                this._statuses.DistributePosts();
            });

            if (ct.IsCancellationRequested)
                return;

            p.Report("Related refreshed");

            this.RefreshTimeline(false);

            var tabPage = this.ListTab.TabPages.Cast<TabPage>()
                .FirstOrDefault(x => x.Text == tab.TabName);

            if (tabPage != null)
            {
                // TODO: 非同期更新中にタブが閉じられている場合を厳密に考慮したい

                var listView = (DetailsListView)tabPage.Tag;
                var index = tab.IndexOf(tab.RelationTargetPost.RetweetedId ?? tab.RelationTargetPost.StatusId);

                if (index != -1 && index < listView.Items.Count)
                {
                    listView.SelectedIndices.Add(index);
                    listView.Items[index].Focused = true;
                }
            }
        }

        private async Task FavAddAsync(IReadOnlyList<long> statusIds, TabClass tab)
        {
            await this.workerSemaphore.WaitAsync();

            try
            {
                var progress = new Progress<string>(x => this.StatusLabel.Text = x);

                await this.FavAddAsyncInternal(progress, this.workerCts.Token, statusIds, tab);
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

        private async Task FavAddAsyncInternal(IProgress<string> p, CancellationToken ct, IReadOnlyList<long> statusIds, TabClass tab)
        {
            if (ct.IsCancellationRequested)
                return;

            if (!CheckAccountValid())
                throw new WebApiException("Auth error. Check your account");

            var successIds = new List<long>();

            await Task.Run(() =>
            {
                //スレッド処理はしない
                var allCount = 0;
                var failedCount = 0;

                foreach (var statusId in statusIds)
                {
                    allCount++;

                    p.Report(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText15 +
                        allCount + "/" + statusIds.Count +
                        Properties.Resources.GetTimelineWorker_RunWorkerCompletedText16 +
                        failedCount);

                    var post = tab.Posts[statusId];

                    if (post.IsFav)
                        continue;

                    var err = this.tw.PostFavAdd(post.RetweetedId ?? post.StatusId);

                    if (!string.IsNullOrEmpty(err))
                    {
                        failedCount++;
                        continue;
                    }

                    successIds.Add(statusId);
                    post.IsFav = true; // リスト再描画必要

                    this._favTimestamps.Add(DateTime.Now);

                    // TLでも取得済みならfav反映
                    if (this._statuses.ContainsKey(statusId))
                    {
                        var postTl = this._statuses[statusId];
                        postTl.IsFav = true;

                        var favTab = this._statuses.GetTabByType(MyCommon.TabUsageType.Favorites);
                        favTab.Add(statusId, postTl.IsRead, false);
                    }

                    // 検索,リスト,UserTimeline,Relatedの各タブに反映
                    foreach (var tb in this._statuses.GetTabsInnerStorageType())
                    {
                        if (tb.Contains(statusId))
                            tb.Posts[statusId].IsFav = true;
                    }
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

            this.RefreshTimeline(false);

            if (this._curList != null && this._curTab != null && this._curTab.Text == tab.TabName)
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

        private async Task FavRemoveAsync(IReadOnlyList<long> statusIds, TabClass tab)
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
                this.StatusLabel.Text = ex.Message;
            }
            finally
            {
                this.workerSemaphore.Release();
            }
        }

        private async Task FavRemoveAsyncInternal(IProgress<string> p, CancellationToken ct, IReadOnlyList<long> statusIds, TabClass tab)
        {
            if (ct.IsCancellationRequested)
                return;

            if (!CheckAccountValid())
                throw new WebApiException("Auth error. Check your account");

            var successIds = new List<long>();

            await Task.Run(() =>
            {
                //スレッド処理はしない
                var allCount = 0;
                var failedCount = 0;
                foreach (var statusId in statusIds)
                {
                    allCount++;

                    var post = tab.Posts[statusId];

                    p.Report(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText17 +
                        allCount + "/" + statusIds.Count +
                        Properties.Resources.GetTimelineWorker_RunWorkerCompletedText18 +
                        failedCount);

                    if (!post.IsFav)
                        continue;

                    var err = this.tw.PostFavRemove(post.RetweetedId ?? post.StatusId);

                    if (!string.IsNullOrEmpty(err))
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

            this.RemovePostFromFavTab(successIds.ToArray());

            this.RefreshTimeline(false);

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
                this.StatusLabel.Text = ex.Message;
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
                    if (status.imagePath == null || status.imagePath.Length == 0 || string.IsNullOrEmpty(status.imagePath[0]))
                    {
                        var err = this.tw.PostStatus(status.status, status.inReplyToId);
                        if (!string.IsNullOrEmpty(err))
                            throw new WebApiException(err);
                    }
                    else
                    {
                        var service = ImageSelector.GetService(status.imageService);
                        await service.PostStatusAsync(status.status, status.inReplyToId, status.imagePath)
                            .ConfigureAwait(false);
                    }
                });

                p.Report(Properties.Resources.PostWorker_RunWorkerCompletedText4);
            }
            catch (WebApiException ex)
            {
                // 処理は中断せずエラーの表示のみ行う
                errMsg = ex.Message;
                p.Report(errMsg);
                this._myStatusError = true;
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
                if (this._isActiveUserstream)
                    this.RefreshTimeline(true);
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
                this.StatusLabel.Text = ex.Message;
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

            await Task.Run(() =>
            {
                foreach (var statusId in statusIds)
                {
                    var err = this.tw.PostRetweet(statusId, read);
                    if (!string.IsNullOrEmpty(err))
                        throw new WebApiException(err);
                }
            });

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

            if (this._cfgCommon.PostAndGet && !this._isActiveUserstream)
                await this.GetHomeTimelineAsync();
        }

        private async Task RefreshFollowerIdsAsync()
        {
            await this.workerSemaphore.WaitAsync();
            try
            {
                this.StatusLabel.Text = Properties.Resources.UpdateFollowersMenuItem1_ClickText1;

                await Task.Run(() => tw.RefreshFollowerIds());

                this.StatusLabel.Text = Properties.Resources.UpdateFollowersMenuItem1_ClickText3;

                this.RefreshTimeline(false);
                this.PurgeListViewItemCache();
                if (this._curList != null)
                    this._curList.Refresh();
            }
            catch (WebApiException ex)
            {
                this.StatusLabel.Text = ex.Message;
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
                await Task.Run(() => tw.RefreshNoRetweetIds());

                this.StatusLabel.Text = "NoRetweetIds refreshed";
            }
            catch (WebApiException ex)
            {
                this.StatusLabel.Text = ex.Message;
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

                await Task.Run(() => tw.RefreshBlockIds());

                this.StatusLabel.Text = Properties.Resources.UpdateBlockUserText3;
            }
            catch (WebApiException ex)
            {
                this.StatusLabel.Text = ex.Message;
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
                await Task.Run(() => tw.RefreshConfiguration());

                if (this.tw.Configuration.PhotoSizeLimit != 0)
                {
                    foreach (var service in this.ImageSelector.GetServices())
                    {
                        service.UpdateTwitterConfiguration(this.tw.Configuration);
                    }
                }

                this.PurgeListViewItemCache();

                if (this._curList != null)
                    this._curList.Refresh();
            }
            catch (WebApiException ex)
            {
                this.StatusLabel.Text = ex.Message;
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

        private void RemovePostFromFavTab(Int64[] ids)
        {
            var favTab = this._statuses.GetTabByType(MyCommon.TabUsageType.Favorites);
            string favTabName = favTab.TabName;
            int fidx = 0;
            if (_curTab.Text.Equals(favTabName))
            {
                if (_curList.FocusedItem != null)
                    fidx = _curList.FocusedItem.Index;
                else if (_curList.TopItem != null)
                    fidx = _curList.TopItem.Index;
                else
                    fidx = 0;
            }

            foreach (long i in ids)
            {
                try
                {
                    _statuses.RemoveFavPost(i);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            if (_curTab != null && _curTab.Text.Equals(favTabName))
            {
                this.PurgeListViewItemCache();
                _curPost = null;
                //_curItemIndex = -1;
            }
            foreach (TabPage tp in ListTab.TabPages)
            {
                if (tp.Text == favTabName)
                {
                    ((DetailsListView)tp.Tag).VirtualListSize = favTab.AllCount;
                    break;
                }
            }
            if (_curTab.Text.Equals(favTabName))
            {
                do
                {
                    _curList.SelectedIndices.Clear();
                }
                while (_curList.SelectedIndices.Count > 0);

                if (favTab.AllCount > 0)
                {
                    if (favTab.AllCount - 1 > fidx && fidx > -1)
                    {
                        _curList.SelectedIndices.Add(fidx);
                    }
                    else
                    {
                        _curList.SelectedIndices.Add(favTab.AllCount - 1);
                    }
                    if (_curList.SelectedIndices.Count > 0)
                    {
                        _curList.EnsureVisible(_curList.SelectedIndices[0]);
                        _curList.FocusedItem = _curList.Items[_curList.SelectedIndices[0]];
                    }
                }
            }
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
            await this.FavoritesRetweetOriginal();
        }

        private async void FavoriteRetweetUnofficialMenuItem_Click(object sender, EventArgs e)
        {
            await this.FavoritesRetweetUnofficial();
        }

        private async Task FavoriteChange(bool FavAdd , bool multiFavoriteChangeDialogEnable = true)
        {
            TabClass tab;
            if (!this._statuses.Tabs.TryGetValue(this._curTab.Text, out tab))
                return;

            //trueでFavAdd,falseでFavRemove
            if (tab.TabType == MyCommon.TabUsageType.DirectMessage || _curList.SelectedIndices.Count == 0
                || !this.ExistCurrentPost) return;

            //複数fav確認msg
            if (_curList.SelectedIndices.Count > 250 && FavAdd)
            {
                MessageBox.Show(Properties.Resources.FavoriteLimitCountText);
                _DoFavRetweetFlags = false;
                return;
            }
            else if (multiFavoriteChangeDialogEnable && _curList.SelectedIndices.Count > 1)
            {
                if (FavAdd)
                {
                    string QuestionText = Properties.Resources.FavAddToolStripMenuItem_ClickText1;
                    if (_DoFavRetweetFlags) QuestionText = Properties.Resources.FavoriteRetweetQuestionText3;
                    if (MessageBox.Show(QuestionText, Properties.Resources.FavAddToolStripMenuItem_ClickText2,
                                       MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        _DoFavRetweetFlags = false;
                        return;
                    }
                }
                else
                {
                    if (MessageBox.Show(Properties.Resources.FavRemoveToolStripMenuItem_ClickText1, Properties.Resources.FavRemoveToolStripMenuItem_ClickText2,
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }

            var statusIds = new List<long>();
            foreach (int idx in _curList.SelectedIndices)
            {
                PostClass post = GetCurTabPost(idx);
                if (FavAdd)
                {
                    if (!post.IsFav)
                        statusIds.Add(post.StatusId);
                }
                else
                {
                    if (post.IsFav)
                        statusIds.Add(post.StatusId);
                }
            }
            if (statusIds.Count == 0)
            {
                if (FavAdd)
                    StatusLabel.Text = Properties.Resources.FavAddToolStripMenuItem_ClickText4;
                else
                    StatusLabel.Text = Properties.Resources.FavRemoveToolStripMenuItem_ClickText4;

                return;
            }

            if (FavAdd)
                await this.FavAddAsync(statusIds, tab);
            else
                await this.FavRemoveAsync(statusIds, tab);
        }

        private PostClass GetCurTabPost(int Index)
        {
            this.itemCacheLock.EnterReadLock();
            try
            {
                if (_postCache != null && Index >= _itemCacheIndex && Index < _itemCacheIndex + _postCache.Length)
                    return _postCache[Index - _itemCacheIndex];
            }
            finally { this.itemCacheLock.ExitReadLock(); }

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
                    _modifySettingLocal = true;
                }
            }
        }

        private void MyList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (this._cfgCommon.SortOrderLock) return;
            var mode = ComparerMode.Id;
            if (_iconCol)
            {
                mode = ComparerMode.Id;
            }
            else
            {
                switch (e.Column)
                {
                    case 0:
                    case 5:
                    case 6:    //0:アイコン,5:未読マーク,6:プロテクト・フィルターマーク
                        //ソートしない
                        return;
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
            }
            _statuses.ToggleSortOrder(mode);
            InitColumnText();

            DetailsListView list = (DetailsListView)sender;
            if (_iconCol)
            {
                list.Columns[0].Text = ColumnOrgText[0];
                list.Columns[1].Text = ColumnText[2];
            }
            else
            {
                for (int i = 0; i <= 7; i++)
                {
                    list.Columns[i].Text = ColumnOrgText[i];
                }
                list.Columns[e.Column].Text = ColumnText[e.Column];
            }

            this.PurgeListViewItemCache();

            if (_statuses.Tabs[_curTab.Text].AllCount > 0 && _curPost != null)
            {
                int idx = _statuses.Tabs[_curTab.Text].IndexOf(_curPost.StatusId);
                if (idx > -1)
                {
                    SelectListItem(_curList, idx);
                    _curList.EnsureVisible(idx);
                }
            }
            _curList.Refresh();
            _modifySettingCommon = true;
        }

        private void TweenMain_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _myLoc = this.DesktopLocation;
                _modifySettingLocal = true;
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
                ReTweetOriginalStripMenuItem.Enabled = false;
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
                    ReTweetOriginalStripMenuItem.Enabled = false;
                    FavoriteRetweetContextMenu.Enabled = false;
                }
                else
                {
                    if (_curPost.IsProtect)
                    {
                        ReTweetOriginalStripMenuItem.Enabled = false;
                        ReTweetStripMenuItem.Enabled = false;
                        QuoteStripMenuItem.Enabled = false;
                        FavoriteRetweetContextMenu.Enabled = false;
                        FavoriteRetweetUnofficialContextMenu.Enabled = false;
                    }
                    else
                    {
                        ReTweetOriginalStripMenuItem.Enabled = true;
                        ReTweetStripMenuItem.Enabled = true;
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

        private void doStatusDelete()
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

            int focusedIndex;
            if (this._curList.FocusedItem != null)
                focusedIndex = this._curList.FocusedItem.Index;
            else if (this._curList.TopItem != null)
                focusedIndex = this._curList.TopItem.Index;
            else
                focusedIndex = 0;

            using (ControlTransaction.Cursor(this, Cursors.WaitCursor))
            {
                string lastError = null;
                foreach (var post in posts)
                {
                    if (!post.CanDeleteBy(this.tw.UserId))
                        continue;

                    string err;
                    if (post.IsDm)
                    {
                        err = this.tw.RemoveDirectMessage(post.StatusId, post);
                    }
                    else
                    {
                        if (post.RetweetedId != null && post.UserId == this.tw.UserId)
                            // 他人に RT された自分のツイート
                            err = this.tw.RemoveStatus(post.RetweetedId.Value);
                        else
                            // 自分のツイート or 自分が RT したツイート
                            err = this.tw.RemoveStatus(post.StatusId);
                    }

                    if (!string.IsNullOrEmpty(err))
                    {
                        lastError = err;
                        continue;
                    }

                    this._statuses.RemovePost(post.StatusId);
                }

                if (lastError == null)
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

        private void DeleteStripMenuItem_Click(object sender, EventArgs e)
        {
            doStatusDelete();
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

        private void RefreshStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoRefresh();
        }

        private void DoRefresh()
        {
            if (_curTab != null)
            {
                TabClass tab;
                if (!this._statuses.Tabs.TryGetValue(this._curTab.Text, out tab))
                    return;

                switch (_statuses.Tabs[_curTab.Text].TabType)
                {
                    case MyCommon.TabUsageType.Mentions:
                        this.GetReplyAsync();
                        break;
                    case MyCommon.TabUsageType.DirectMessage:
                        this.GetDirectMessagesAsync();
                        break;
                    case MyCommon.TabUsageType.Favorites:
                        this.GetFavoritesAsync();
                        break;
                    //case MyCommon.TabUsageType.Profile:
                        //// TODO
                    case MyCommon.TabUsageType.PublicSearch:
                        //// TODO
                        if (string.IsNullOrEmpty(tab.SearchWords)) return;
                        this.GetPublicSearchAsync(tab);
                        break;
                    case MyCommon.TabUsageType.UserTimeline:
                        this.GetUserTimelineAsync(tab);
                        break;
                    case MyCommon.TabUsageType.Lists:
                        //// TODO
                        if (tab.ListInfo == null || tab.ListInfo.Id == 0) return;
                        this.GetListTimelineAsync(tab);
                        break;
                    default:
                        this.GetHomeTimelineAsync();
                        break;
                }
            }
            else
            {
                this.GetHomeTimelineAsync();
            }
        }

        private async Task DoRefreshMore()
        {
            //ページ指定をマイナス1に
            if (_curTab != null)
            {
                TabClass tab;
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
                        // TODO
                        if (string.IsNullOrEmpty(tab.SearchWords)) return;
                        await this.GetPublicSearchAsync(tab, loadMore: true);
                        break;
                    case MyCommon.TabUsageType.UserTimeline:
                        await this.GetUserTimelineAsync(tab, loadMore: true);
                        break;
                    case MyCommon.TabUsageType.Lists:
                        //// TODO
                        if (tab.ListInfo == null || tab.ListInfo.Id == 0) return;
                        await this.GetListTimelineAsync(tab, loadMore: true);
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
                    HttpTwitter.TwitterUrl = _cfgCommon.TwitterUrl;

                    Networking.DefaultTimeout = TimeSpan.FromSeconds(this._cfgCommon.DefaultTimeOut);
                    Networking.SetWebProxy(this._cfgLocal.ProxyType,
                        this._cfgLocal.ProxyAddress, this._cfgLocal.ProxyPort,
                        this._cfgLocal.ProxyUser, this._cfgLocal.ProxyPassword);

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
                    if (_curList != null) _curList.Refresh();
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

            //現在の選択状態を退避
            var selId = new Dictionary<string, long[]>();
            var focusedId = new Dictionary<string, Tuple<long, long>>();
            SaveSelectedStatus(selId, focusedId);

            ListTab.Alignment = newAlignment;

            //選択状態を復帰
            foreach (TabPage tab in ListTab.TabPages)
            {
                DetailsListView lst = (DetailsListView)tab.Tag;
                TabClass tabInfo = _statuses.Tabs[tab.Text];
                using (ControlTransaction.Update(lst))
                {
                    // status_id から ListView 上のインデックスに変換
                    var selectedIndices = selId[tab.Text] != null
                        ? tabInfo.IndexOf(selId[tab.Text]).Where(x => x != -1).ToArray()
                        : null;
                    var focusedIndex = tabInfo.IndexOf(focusedId[tab.Text].Item1);
                    var selectionMarkIndex = tabInfo.IndexOf(focusedId[tab.Text].Item2);

                    this.SelectListItem(lst, selectedIndices, focusedIndex, selectionMarkIndex);
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

        private async void PostBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.AbsoluteUri != "about:blank")
            {
                await this.DispSelectedPost();
                await this.OpenUriInBrowserAsync(e.Url.OriginalString);
            }
        }

        private async void PostBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Scheme == "data")
            {
                StatusLabelUrl.Text = PostBrowser.StatusText.Replace("&", "&&");
            }
            else if (e.Url.AbsoluteUri != "about:blank")
            {
                e.Cancel = true;
                await this.OpenUriAsync(e.Url);
            }
        }

        public void AddNewTabForSearch(string searchWord)
        {
            //同一検索条件のタブが既に存在すれば、そのタブアクティブにして終了
            foreach (TabClass tb in _statuses.GetTabsByType(MyCommon.TabUsageType.PublicSearch))
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
            _statuses.AddTab(tabName, MyCommon.TabUsageType.PublicSearch, null);
            AddNewTab(tabName, false, MyCommon.TabUsageType.PublicSearch);
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
            foreach (TabClass tb in _statuses.GetTabsByType(MyCommon.TabUsageType.UserTimeline))
            {
                if (tb.User == user)
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
            _statuses.AddTab(tabName, MyCommon.TabUsageType.UserTimeline, null);
            var tab = this._statuses.Tabs[tabName];
            tab.User = user;
            AddNewTab(tabName, false, MyCommon.TabUsageType.UserTimeline);
            //追加したタブをアクティブに
            ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
            SaveConfigsTabs();
            //検索実行
            this.GetUserTimelineAsync(tab);
        }

        public bool AddNewTab(string tabName, bool startup, MyCommon.TabUsageType tabType, ListElement listInfo = null)
        {
            //重複チェック
            foreach (TabPage tb in ListTab.TabPages)
            {
                if (tb.Text == tabName) return false;
            }

            //新規タブ名チェック
            if (tabName == Properties.Resources.AddNewTabText1) return false;

            //タブタイプ重複チェック
            if (!startup)
            {
                if (tabType == MyCommon.TabUsageType.DirectMessage ||
                   tabType == MyCommon.TabUsageType.Favorites ||
                   tabType == MyCommon.TabUsageType.Home ||
                   tabType == MyCommon.TabUsageType.Mentions ||
                   tabType == MyCommon.TabUsageType.Related)
                {
                    if (_statuses.GetTabByType(tabType) != null) return false;
                }
            }

            TabPage _tabPage = new TabPage();
            DetailsListView _listCustom = new DetailsListView();

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
                /// UserTimeline関連
                Label label = null;
                if (tabType == MyCommon.TabUsageType.UserTimeline || tabType == MyCommon.TabUsageType.Lists)
                {
                    label = new Label();
                    label.Dock = DockStyle.Top;
                    label.Name = "labelUser";
                    if (tabType == MyCommon.TabUsageType.Lists)
                    {
                        label.Text = listInfo.ToString();
                    }
                    else
                    {
                        label.Text = _statuses.Tabs[tabName].User + "'s Timeline";
                    }
                    label.TextAlign = ContentAlignment.MiddleLeft;
                    using (ComboBox tmpComboBox = new ComboBox())
                    {
                        label.Height = tmpComboBox.Height;
                    }
                    _tabPage.Controls.Add(label);
                }

                /// 検索関連の準備
                Panel pnl = null;
                if (tabType == MyCommon.TabUsageType.PublicSearch)
                {
                    pnl = new Panel();

                    Label lbl = new Label();
                    ComboBox cmb = new ComboBox();
                    Button btn = new Button();
                    ComboBox cmbLang = new ComboBox();

                    pnl.SuspendLayout();

                    pnl.Controls.Add(cmb);
                    pnl.Controls.Add(cmbLang);
                    pnl.Controls.Add(btn);
                    pnl.Controls.Add(lbl);
                    pnl.Name = "panelSearch";
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
                    cmb.AutoCompleteMode = AutoCompleteMode.None;
                    cmb.KeyDown += SearchComboBox_KeyDown;

                    if (_statuses.ContainsTab(tabName))
                    {
                        cmb.Items.Add(_statuses.Tabs[tabName].SearchWords);
                        cmb.Text = _statuses.Tabs[tabName].SearchWords;
                    }

                    cmbLang.Text = "";
                    cmbLang.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                    cmbLang.Dock = DockStyle.Right;
                    cmbLang.Width = 50;
                    cmbLang.Name = "comboLang";
                    cmbLang.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbLang.TabStop = false;
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
                    if (_statuses.ContainsTab(tabName)) cmbLang.Text = _statuses.Tabs[tabName].SearchLang;

                    lbl.Text = "Search(C-S-f)";
                    lbl.Name = "label1";
                    lbl.Dock = DockStyle.Left;
                    lbl.Width = 90;
                    lbl.Height = cmb.Height;
                    lbl.TextAlign = ContentAlignment.MiddleLeft;

                    btn.Text = "Search";
                    btn.Name = "buttonSearch";
                    btn.UseVisualStyleBackColor = true;
                    btn.Dock = DockStyle.Right;
                    btn.TabStop = false;
                    btn.Click += SearchButton_Click;
                }

                this.ListTab.Controls.Add(_tabPage);
                _tabPage.Controls.Add(_listCustom);

                if (tabType == MyCommon.TabUsageType.PublicSearch) _tabPage.Controls.Add(pnl);
                if (tabType == MyCommon.TabUsageType.UserTimeline || tabType == MyCommon.TabUsageType.Lists) _tabPage.Controls.Add(label);

                _tabPage.Location = new Point(4, 4);
                _tabPage.Name = "CTab" + cnt.ToString();
                _tabPage.Size = new Size(380, 260);
                _tabPage.TabIndex = 2 + cnt;
                _tabPage.Text = tabName;
                _tabPage.UseVisualStyleBackColor = true;

                _listCustom.AllowColumnReorder = true;
                _listCustom.ContextMenuStrip = this.ContextMenuOperate;
                _listCustom.ColumnHeaderContextMenuStrip = this.ContextMenuColumnHeader;
                _listCustom.Dock = DockStyle.Fill;
                _listCustom.FullRowSelect = true;
                _listCustom.HideSelection = false;
                _listCustom.Location = new Point(0, 0);
                _listCustom.Margin = new Padding(0);
                _listCustom.Name = "CList" + Environment.TickCount.ToString();
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

                if (tabType == MyCommon.TabUsageType.PublicSearch) pnl.ResumeLayout(false);
            }

            _tabPage.Tag = _listCustom;
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
                if (lst.VirtualListSize != count)
                {
                    lst.VirtualListSize = count;
                }
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
                Rectangle dragEnableRectangle = new Rectangle((int)(_tabMouseDownPoint.X - (SystemInformation.DragSize.Width / 2)), (int)(_tabMouseDownPoint.Y - (SystemInformation.DragSize.Height / 2)), SystemInformation.DragSize.Width, SystemInformation.DragSize.Height);
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

        private void PostBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (PostBrowser.StatusText.StartsWith("http") || PostBrowser.StatusText.StartsWith("ftp")
                        || PostBrowser.StatusText.StartsWith("data"))
                {
                    StatusLabelUrl.Text = PostBrowser.StatusText.Replace("&", "&&");
                }
                if (string.IsNullOrEmpty(PostBrowser.StatusText))
                {
                    SetStatusLabelUrl();
                }
            }
            catch (Exception)
            {
            }
        }

        private void StatusText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '@')
            {
                if (!this._cfgCommon.UseAtIdSupplement) return;
                //@マーク
                int cnt = AtIdSupl.ItemCount;
                ShowSuplDialog(StatusText, AtIdSupl);
                if (cnt != AtIdSupl.ItemCount) _modifySettingAtId = true;
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
                    foreach (char c in StatusText.Text.ToCharArray())
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
            int pLen = GetRestStatusCount(true, false);
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
                _reply_to_id = null;
                _reply_to_name = null;
            }
        }

        private int GetRestStatusCount(bool isAuto, bool isAddFooter)
        {
            //文字数カウント
            var statusText = this.StatusText.Text;
            statusText = statusText.Replace("\r\n", "\n");

            int pLen = 140 - statusText.Length;
            if (this.NotifyIcon1 == null || !this.NotifyIcon1.Visible) return pLen;
            if ((isAuto && !MyCommon.IsKeyDown(Keys.Control) && this._cfgCommon.PostShiftEnter) ||
                (isAuto && !MyCommon.IsKeyDown(Keys.Shift) && !this._cfgCommon.PostShiftEnter) ||
                (!isAuto && isAddFooter))
            {
                if (this._cfgLocal.UseRecommendStatus)
                    pLen -= this.recommendedStatusFooter.Length;
                else if (this._cfgLocal.StatusText.Length > 0)
                    pLen -= this._cfgLocal.StatusText.Length + 1;
            }
            if (!string.IsNullOrEmpty(HashMgr.UseHash))
            {
                pLen -= HashMgr.UseHash.Length + 1;
            }
            //foreach (Match m in Regex.Matches(statusText, "https?:\/\/[-_.!~*//()a-zA-Z0-9;\/?:\@&=+\$,%#^]+"))
            //{
            //    pLen += m.Length - SettingDialog.TwitterConfiguration.ShortUrlLength;
            //}
            foreach (Match m in Regex.Matches(statusText, Twitter.rgUrl, RegexOptions.IgnoreCase))
            {
                string before = m.Result("${before}");
                string url = m.Result("${url}");
                string protocol = m.Result("${protocol}");
                string domain = m.Result("${domain}");
                string path = m.Result("${path}");
                if (protocol.Length == 0)
                {
                    if (Regex.IsMatch(before, Twitter.url_invalid_without_protocol_preceding_chars))
                    {
                        continue;
                    }

                    bool last_url_invalid_match = false;
                    string lasturl = null;
                    foreach (Match mm in Regex.Matches(domain, Twitter.url_valid_ascii_domain, RegexOptions.IgnoreCase))
                    {
                        lasturl = mm.ToString();
                        last_url_invalid_match = Regex.IsMatch(lasturl, Twitter.url_invalid_short_domain, RegexOptions.IgnoreCase);
                        if (!last_url_invalid_match)
                        {
                            pLen += lasturl.Length - this.tw.Configuration.ShortUrlLength;
                        }
                    }

                    if (path.Length != 0)
                    {
                        if (last_url_invalid_match)
                        {
                            pLen += lasturl.Length - this.tw.Configuration.ShortUrlLength;
                        }
                        pLen += path.Length;
                    }
                }
                else
                {
                    int shortUrlLength = protocol == "https://"
                        ? this.tw.Configuration.ShortUrlLengthHttps
                        : this.tw.Configuration.ShortUrlLength;

                    pLen += url.Length - shortUrlLength;
                }
                
                //if (m.Result("${url}").Length > SettingDialog.TwitterConfiguration.ShortUrlLength)
                //{
                //    pLen += m.Result("${url}").Length - SettingDialog.TwitterConfiguration.ShortUrlLength;
                //}
            }
            if (ImageSelector.Visible && !string.IsNullOrEmpty(ImageSelector.ServiceName))
            {
                pLen -= this.tw.Configuration.CharactersReservedPerMedia;
            }
            return pLen;
        }

        private void MyList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            this.itemCacheLock.EnterUpgradeableReadLock();
            try
            {
                if (_curList.Equals(sender))
                {
                    if (_itemCache != null &&
                       e.StartIndex >= _itemCacheIndex &&
                       e.EndIndex < _itemCacheIndex + _itemCache.Length)
                    {
                        //If the newly requested cache is a subset of the old cache, 
                        //no need to rebuild everything, so do nothing.
                        return;
                    }

                    //Now we need to rebuild the cache.
                    CreateCache(e.StartIndex, e.EndIndex);
                }
            }
            finally { this.itemCacheLock.ExitUpgradeableReadLock(); }
        }

        private void MyList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem item = null;
            PostClass cacheItemPost = null;

            if (_curList.Equals(sender))
                this.TryGetListViewItemCache(e.ItemIndex, out item, out cacheItemPost);

            if (item == null)
            {
                //A cache miss, so create a new ListViewItem and pass it back.
                TabPage tb = (TabPage)((DetailsListView)sender).Parent;
                try
                {
                    item = this.CreateItem(tb, _statuses.Tabs[tb.Text][e.ItemIndex], e.ItemIndex);
                }
                catch (Exception)
                {
                    //不正な要求に対する間に合わせの応答
                    string[] sitem = {"", "", "", "", "", "", "", ""};
                    item = new ImageListViewItem(sitem);
                }
            }

            e.Item = item;
        }

        private void CreateCache(int StartIndex, int EndIndex)
        {
            this.itemCacheLock.EnterWriteLock();
            try
            {
                var tabInfo = _statuses.Tabs[_curTab.Text];

                //キャッシュ要求（要求範囲±30を作成）
                StartIndex -= 30;
                if (StartIndex < 0) StartIndex = 0;
                EndIndex += 30;
                if (EndIndex >= tabInfo.AllCount) EndIndex = tabInfo.AllCount - 1;
                _postCache = tabInfo[StartIndex, EndIndex]; //配列で取得
                _itemCacheIndex = StartIndex;

                _itemCache = new ListViewItem[0] {};
                Array.Resize(ref _itemCache, _postCache.Length);

                for (int i = 0; i < _postCache.Length; i++)
                {
                    _itemCache[i] = CreateItem(_curTab, _postCache[i], StartIndex + i);
                }
            }
            catch (Exception)
            {
                //キャッシュ要求が実データとずれるため（イベントの遅延？）
                _postCache = null;
                _itemCacheIndex = -1;
                _itemCache = null;
            }
            finally { this.itemCacheLock.ExitWriteLock(); }
        }

        /// <summary>
        /// DetailsListView のための ListViewItem のキャッシュを消去する
        /// </summary>
        private void PurgeListViewItemCache()
        {
            this.itemCacheLock.EnterWriteLock();
            try
            {
                this._itemCache = null;
                this._itemCacheIndex = -1;
                this._postCache = null;
            }
            finally { this.itemCacheLock.ExitWriteLock(); }
        }

        private bool TryGetListViewItemCache(int index, out ListViewItem item, out PostClass post)
        {
            this.itemCacheLock.EnterReadLock();
            try
            {
                if (this._itemCache != null && index >= this._itemCacheIndex && index < this._itemCacheIndex + this._itemCache.Length)
                {
                    item = this._itemCache[index - _itemCacheIndex];
                    post = this._postCache[index - _itemCacheIndex];
                    return true;
                }
            }
            finally { this.itemCacheLock.ExitReadLock(); }

            item = null;
            post = null;
            return false;
        }

        private ListViewItem CreateItem(TabPage Tab, PostClass Post, int Index)
        {
            StringBuilder mk = new StringBuilder();
            //if (Post.IsDeleted) mk.Append("×");
            //if (Post.IsMark) mk.Append("♪");
            //if (Post.IsProtect) mk.Append("Ю");
            //if (Post.InReplyToStatusId != null) mk.Append("⇒");
            if (Post.FavoritedCount > 0) mk.Append("+" + Post.FavoritedCount.ToString());
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

        private void DoTabSearch(string _word,
                                 bool CaseSensitive,
                                 bool UseRegex,
                                 SEARCHTYPE SType)
        {
            int cidx = 0;
            bool fnd = false;
            int toIdx;
            int stp = 1;

            if (_curList.VirtualListSize == 0)
            {
                MessageBox.Show(Properties.Resources.DoTabSearchText2, Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (_curList.SelectedIndices.Count > 0)
            {
                cidx = _curList.SelectedIndices[0];
            }
            toIdx = _curList.VirtualListSize;

            switch (SType)
            {
                case SEARCHTYPE.DialogSearch:    //ダイアログからの検索
                    if (_curList.SelectedIndices.Count > 0)
                        cidx = _curList.SelectedIndices[0];
                    else
                        cidx = 0;
                    break;
                case SEARCHTYPE.NextSearch:      //次を検索
                    if (_curList.SelectedIndices.Count > 0)
                    {
                        cidx = _curList.SelectedIndices[0] + 1;
                        if (cidx > toIdx) cidx = toIdx;
                    }
                    else
                    {
                        cidx = 0;
                    }
                    break;
                case SEARCHTYPE.PrevSearch:      //前を検索
                    if (_curList.SelectedIndices.Count > 0)
                    {
                        cidx = _curList.SelectedIndices[0] - 1;
                        if (cidx < 0) cidx = 0;
                    }
                    else
                    {
                        cidx = toIdx;
                    }
                    toIdx = -1;
                    stp = -1;
                    break;
            }

            RegexOptions regOpt = RegexOptions.None;
            StringComparison fndOpt = StringComparison.Ordinal;
            if (!CaseSensitive)
            {
                regOpt = RegexOptions.IgnoreCase;
                fndOpt = StringComparison.OrdinalIgnoreCase;
            }
            try
            {
    RETRY:
                if (UseRegex)
                {
                    // 正規表現検索
                    Regex _search;
                    try
                    {
                        _search = new Regex(_word, regOpt);
                        for (int idx = cidx; idx != toIdx; idx += stp)
                        {
                            PostClass post;
                            try
                            {
                                post = _statuses.Tabs[_curTab.Text][idx];
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            if (_search.IsMatch(post.Nickname)
                                || _search.IsMatch(post.TextFromApi)
                                || _search.IsMatch(post.ScreenName))
                            {
                                SelectListItem(_curList, idx);
                                _curList.EnsureVisible(idx);
                                return;
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show(Properties.Resources.DoTabSearchText1, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    // 通常検索
                    for (int idx = cidx; idx != toIdx; idx += stp)
                    {
                        PostClass post;
                        try
                        {
                            post = _statuses.Tabs[_curTab.Text][idx];
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        if (post.Nickname.IndexOf(_word, fndOpt) > -1
                            || post.TextFromApi.IndexOf(_word, fndOpt) > -1
                            || post.ScreenName.IndexOf(_word, fndOpt) > -1)
                        {
                            SelectListItem(_curList, idx);
                            _curList.EnsureVisible(idx);
                            return;
                        }
                    }
                }

                if (!fnd)
                {
                    switch (SType)
                    {
                        case SEARCHTYPE.DialogSearch:
                        case SEARCHTYPE.NextSearch:
                            toIdx = cidx;
                            cidx = 0;
                            break;
                        case SEARCHTYPE.PrevSearch:
                            toIdx = cidx;
                            cidx = _curList.VirtualListSize - 1;
                            break;
                    }
                    fnd = true;
                    goto RETRY;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            MessageBox.Show(Properties.Resources.DoTabSearchText2, Properties.Resources.DoTabSearchText3, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // Recentタブの検索時以外では「新規タブに表示」ボタンを無効化する
            if (this._statuses.Tabs[this._curTab.Text].TabType == MyCommon.TabUsageType.Home)
                this.SearchDialog.DisableNewTabButton = false;
            else
                this.SearchDialog.DisableNewTabButton = true;

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
                    var tabName = searchOptions.Query;

                    try
                    {
                        tabName = this._statuses.MakeTabName(tabName);
                    }
                    catch (TabException ex)
                    {
                        MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    this.AddNewTab(tabName, false, MyCommon.TabUsageType.UserDefined);
                    this._statuses.AddTab(tabName, MyCommon.TabUsageType.UserDefined, null);

                    var filter = new PostFilterRule
                    {
                        FilterBody = new[] { searchOptions.Query },
                        UseRegex = searchOptions.UseRegex,
                        CaseSensitive = searchOptions.CaseSensitive,
                    };
                    this._statuses.Tabs[tabName].AddFilter(filter);

                    var tabPage = this.ListTab.TabPages.Cast<TabPage>()
                        .First(x => x.Text == tabName);

                    this.ListTab.SelectedTab = tabPage;

                    this.ApplyPostFilters();
                    this.SaveConfigsTabs();
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
                idx = _statuses.Tabs[ListTab.TabPages[i].Text].OldestUnreadIndex;
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
                    idx = _statuses.Tabs[ListTab.TabPages[i].Text].OldestUnreadIndex;
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
            if (!StatusLabelUrl.Text.StartsWith("http")) SetStatusLabelUrl();
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

        private async void DisplayItemImage_Downloaded(object sender, EventArgs e)
        {
            if (sender.Equals(displayItem))
            {
                this.ClearUserPicture();

                var img = displayItem.Image;
                try
                {
                    if (img != null)
                        img = await img.CloneAsync();

                    UserPicture.Image = img;
                }
                catch (Exception)
                {
                    UserPicture.ShowErrorImage();
                }
            }
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

            if (displayItem != null)
            {
                displayItem.ImageDownloaded -= this.DisplayItemImage_Downloaded;
                displayItem = null;
            }
            displayItem = (ImageListViewItem)_curList.Items[_curList.SelectedIndices[0]];
            displayItem.ImageDownloaded += this.DisplayItemImage_Downloaded;

            using (ControlTransaction.Update(this.TableLayoutPanel1))
            {
                SourceLinkLabel.Text = this._curPost.Source;
                SourceLinkLabel.Tag = this._curPost.SourceUri;
                SourceLinkLabel.TabStop = false; // Text を更新すると勝手に true にされる

                string nameText;
                if (_curPost.IsDm)
                {
                    if (_curPost.IsOwl)
                        nameText = "DM FROM <- ";
                    else
                        nameText = "DM TO -> ";
                }
                else
                {
                    nameText = "";
                }
                nameText += _curPost.ScreenName + "/" + _curPost.Nickname;
                if (_curPost.RetweetedId != null)
                    nameText += " (RT:" + _curPost.RetweetedBy + ")";

                NameLabel.Text = nameText;
                NameLabel.Tag = _curPost.ScreenName;

                var nameForeColor = SystemColors.ControlText;
                if (_curPost.IsOwl && (this._cfgCommon.OneWayLove || _curPost.IsDm))
                    nameForeColor = this._clOWL;
                if (_curPost.RetweetedId != null)
                    nameForeColor = this._clRetweet;
                if (_curPost.IsFav)
                    nameForeColor = this._clFav;
                NameLabel.ForeColor = nameForeColor;

                this.ClearUserPicture();

                if (!string.IsNullOrEmpty(_curPost.ImageUrl))
                {
                    var image = IconCache.TryGetFromCache(_curPost.ImageUrl);
                    try
                    {
                        UserPicture.Image = image != null ? image.Clone() : null;
                    }
                    catch (Exception)
                    {
                        UserPicture.ShowErrorImage();
                    }
                }

                DateTimeLabel.Text = _curPost.CreatedAt.ToString();
            }

            if (DumpPostClassToolStripMenuItem.Checked)
            {
                StringBuilder sb = new StringBuilder(512);

                sb.Append("-----Start PostClass Dump<br>");
                sb.AppendFormat("TextFromApi           : {0}<br>", _curPost.TextFromApi);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", _curPost.TextFromApi);
                sb.AppendFormat("StatusId             : {0}<br>", _curPost.StatusId.ToString());
                //sb.AppendFormat("ImageIndex     : {0}<br>", _curPost.ImageIndex.ToString());
                sb.AppendFormat("ImageUrl       : {0}<br>", _curPost.ImageUrl);
                sb.AppendFormat("InReplyToStatusId    : {0}<br>", _curPost.InReplyToStatusId.ToString());
                sb.AppendFormat("InReplyToUser  : {0}<br>", _curPost.InReplyToUser);
                sb.AppendFormat("IsDM           : {0}<br>", _curPost.IsDm.ToString());
                sb.AppendFormat("IsFav          : {0}<br>", _curPost.IsFav.ToString());
                sb.AppendFormat("IsMark         : {0}<br>", _curPost.IsMark.ToString());
                sb.AppendFormat("IsMe           : {0}<br>", _curPost.IsMe.ToString());
                sb.AppendFormat("IsOwl          : {0}<br>", _curPost.IsOwl.ToString());
                sb.AppendFormat("IsProtect      : {0}<br>", _curPost.IsProtect.ToString());
                sb.AppendFormat("IsRead         : {0}<br>", _curPost.IsRead.ToString());
                sb.AppendFormat("IsReply        : {0}<br>", _curPost.IsReply.ToString());

                foreach (string nm in _curPost.ReplyToList)
                {
                    sb.AppendFormat("ReplyToList    : {0}<br>", nm);
                }

                sb.AppendFormat("ScreenName           : {0}<br>", _curPost.ScreenName);
                sb.AppendFormat("NickName       : {0}<br>", _curPost.Nickname);
                sb.AppendFormat("Text   : {0}<br>", _curPost.Text);
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", _curPost.Text);
                sb.AppendFormat("CreatedAt          : {0}<br>", _curPost.CreatedAt.ToString());
                sb.AppendFormat("Source         : {0}<br>", _curPost.Source);
                sb.AppendFormat("UserId            : {0}<br>", _curPost.UserId);
                sb.AppendFormat("FilterHit      : {0}<br>", _curPost.FilterHit);
                sb.AppendFormat("RetweetedBy    : {0}<br>", _curPost.RetweetedBy);
                sb.AppendFormat("RetweetedId    : {0}<br>", _curPost.RetweetedId);
                sb.AppendFormat("SearchTabName  : {0}<br>", _curPost.RelTabName);

                sb.AppendFormat("Media.Count    : {0}<br>", _curPost.Media.Count);
                if (_curPost.Media.Count > 0)
                {
                    for (int i = 0; i < _curPost.Media.Count; i++)
                    {
                        var info = _curPost.Media[i];
                        sb.AppendFormat("Media[{0}].Url         : {1}<br>", i, info.Url);
                        sb.AppendFormat("Media[{0}].VideoUrl    : {1}<br>", i, info.VideoUrl ?? "---");
                    }
                }
                sb.Append("-----End PostClass Dump<br>");

                PostBrowser.DocumentText = detailHtmlFormatHeader + sb.ToString() + detailHtmlFormatFooter;
                return;
            }

            var loadTasks = new List<Task>();

            // 同じIDのツイートであれば WebBrowser とサムネイルの更新を行わない
            // (同一ツイートの RT は文面が同じであるため同様に更新しない)
            if (_curPost.StatusId != oldDisplayPost.StatusId)
            {
                this.PostBrowser.DocumentText =
                    this.createDetailHtml(_curPost.IsDeleted ? "(DELETED)" : _curPost.Text);

                this.PostBrowser.Document.Window.ScrollTo(0, 0);

                this.SplitContainer3.Panel2Collapsed = true;

                if (this._cfgCommon.PreviewEnable)
                {
                    var oldTokenSource = Interlocked.Exchange(ref this.thumbnailTokenSource, new CancellationTokenSource());
                    if (oldTokenSource != null)
                        oldTokenSource.Cancel();

                    var token = this.thumbnailTokenSource.Token;
                    loadTasks.Add(this.tweetThumbnail1.ShowThumbnailAsync(_curPost, token));
                }

                loadTasks.Add(this.AppendQuoteTweetAsync(this._curPost));
            }

            try
            {
                await Task.WhenAll(loadTasks);
            }
            catch (OperationCanceledException) { }
        }

        /// <summary>
        /// 発言詳細欄のツイートURLを展開する
        /// </summary>
        private async Task AppendQuoteTweetAsync(PostClass post)
        {
            var statusIds = post.QuoteStatusIds;
            if (statusIds.Length == 0)
                return;

            // 「読み込み中」テキストを表示
            var loadingQuoteHtml = statusIds.Select(x => FormatQuoteTweetHtml(x, Properties.Resources.LoadingText));
            var body = post.Text + string.Concat(loadingQuoteHtml);
            this.PostBrowser.DocumentText = this.createDetailHtml(body);

            // 引用ツイートを読み込み
            var quoteHtmls = await Task.WhenAll(statusIds.Select(x => this.CreateQuoteTweetHtml(x)));

            // 非同期処理中に表示中のツイートが変わっていたらキャンセルされたものと扱う
            if (this._curPost != post || this._curPost.IsDeleted)
                return;

            body = post.Text + string.Concat(quoteHtmls);
            this.PostBrowser.DocumentText = this.createDetailHtml(body);
        }

        private async Task<string> CreateQuoteTweetHtml(long statusId)
        {
            PostClass post = this._statuses[statusId];
            string err = null;
            if (post == null)
            {
                err = await Task.Run(() => this.tw.GetStatusApi(false, statusId, ref post))
                    .ConfigureAwait(false);

                if (!string.IsNullOrEmpty(err))
                    return FormatQuoteTweetHtml(statusId, WebUtility.HtmlEncode(err));

                post.IsRead = true;
                if (!this._statuses.AddQuoteTweet(post))
                    return FormatQuoteTweetHtml(statusId, "This Tweet is unavailable.");
            }

            return FormatQuoteTweetHtml(post);
        }

        internal static string FormatQuoteTweetHtml(PostClass post)
        {
            var innerHtml = "<p>" + StripLinkTagHtml(post.Text) + "</p>" +
                " &mdash; " + WebUtility.HtmlEncode(post.Nickname) +
                " (@" + WebUtility.HtmlEncode(post.ScreenName) + ") " +
                WebUtility.HtmlEncode(post.CreatedAt.ToString());

            return FormatQuoteTweetHtml(post.ScreenName, post.StatusId, innerHtml);
        }

        internal static string FormatQuoteTweetHtml(long statusId, string innerHtml)
        {
            // screenName が不明な場合、とりあえず https://twitter.com/statuses/status/{statusId} にリンクする
            return FormatQuoteTweetHtml("statuses", statusId, innerHtml);
        }

        internal static string FormatQuoteTweetHtml(string screenName, long statusId, string innerHtml)
        {
            return "<a class=\"quote-tweet-link\" href=\"https://twitter.com/" + WebUtility.HtmlEncode(screenName) + "/status/" + statusId + "\">" +
                "<blockquote class=\"quote-tweet\">" + innerHtml + "</blockquote>" +
                "</a>";
        }

        /// <summary>
        /// 指定されたHTMLからリンクを除去します
        /// </summary>
        internal static string StripLinkTagHtml(string html)
        {
            // a 要素はネストされていない前提の正規表現パターン
            return Regex.Replace(html, @"<a[^>]*>(.*?)</a>", "$1");
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
                ModifierState State = GetModifierState(e.Control, e.Shift, e.Alt);
                if (State == ModifierState.NotFlags) return;
                if (State != ModifierState.None) _anchorFlag = false;

                Task asyncTask;
                if (CommonKeyDown(e.KeyCode, FocusedControl.ListTab, State, out asyncTask))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }

                if (asyncTask != null)
                    await asyncTask;
            }
        }

        private ModifierState GetModifierState(bool sControl, bool sShift, bool sAlt)
        {
            ModifierState state = ModifierState.None;
            if (sControl) state = state | ModifierState.Ctrl;
            if (sShift) state = state | ModifierState.Shift;
            if (sAlt) state = state | ModifierState.Alt;
            return state;
        }

        [FlagsAttribute]
        private enum ModifierState
        {
            None = 0,
            Alt = 1,
            Shift = 2,
            Ctrl = 4,
            //CShift = 11,
            //CAlt = 12,
            //AShift = 13,
            NotFlags = 8,

            //ListTab = 101,
            //PostBrowser = 102,
            //StatusText = 103,
        }

        private enum FocusedControl : int
        {
            None,
            ListTab,
            StatusText,
            PostBrowser,
        }

        private bool CommonKeyDown(Keys KeyCode, FocusedControl Focused, ModifierState Modifier, out Task asyncTask)
        {
            // Task を返す非同期処理があれば asyncTask に代入する
            asyncTask = null;

            //リストのカーソル移動関係（上下キー、PageUp/Downに該当）
            if (Focused == FocusedControl.ListTab)
            {
                if (Modifier == (ModifierState.Ctrl | ModifierState.Shift) ||
                    Modifier == ModifierState.Ctrl ||
                    Modifier == ModifierState.None ||
                    Modifier == ModifierState.Shift)
                {
                    if (KeyCode == Keys.J)
                    {
                        SendKeys.Send("{DOWN}");
                        return true;
                    }
                    else if (KeyCode == Keys.K)
                    {
                        SendKeys.Send("{UP}");
                        return true;
                    }
                }
                if (Modifier == ModifierState.Shift ||
                    Modifier == ModifierState.None)
                {
                    if (KeyCode == Keys.F)
                    {
                        SendKeys.Send("{PGDN}");
                        return true;
                    }
                    else if (KeyCode == Keys.B)
                    {
                        SendKeys.Send("{PGUP}");
                        return true;
                    }
                }
            }

            //修飾キーなし
            switch (Modifier)
            {
                case ModifierState.None:
                    //フォーカス関係なし
                    switch (KeyCode)
                    {
                        case Keys.F1:
                            asyncTask = this.OpenApplicationWebsite();
                            return true;
                        case Keys.F3:
                            MenuItemSearchNext_Click(null, null);
                            return true;
                        case Keys.F5:
                            DoRefresh();
                            return true;
                        case Keys.F6:
                            this.GetReplyAsync();
                            return true;
                        case Keys.F7:
                            this.GetDirectMessagesAsync();
                            return true;
                    }
                    if (Focused != FocusedControl.StatusText)
                    {
                        //フォーカスStatusText以外
                        switch (KeyCode)
                        {
                            case Keys.Space:
                            case Keys.ProcessKey:
                                if (Focused == FocusedControl.ListTab) _anchorFlag = false;
                                JumpUnreadMenuItem_Click(null, null);
                                return true;
                            case Keys.G:
                                if (Focused == FocusedControl.ListTab) _anchorFlag = false;
                                ShowRelatedStatusesMenuItem_Click(null, null);
                                return true;
                        }
                    }
                    if (Focused == FocusedControl.ListTab)
                    {
                        //フォーカスList
                        switch (KeyCode)
                        {
                            case Keys.N:
                            case Keys.Right:
                                GoRelPost(true);
                                return true;
                            case Keys.P:
                            case Keys.Left:
                                GoRelPost(false);
                                return true;
                            case Keys.OemPeriod:
                                GoAnchor();
                                return true;
                            case Keys.I:
                                if (this.StatusText.Enabled) this.StatusText.Focus();
                                return true;
                            case Keys.Enter:
                                MakeReplyOrDirectStatus();
                                return true;
                            case Keys.R:
                                DoRefresh();
                                return true;
                        }
                        //以下、アンカー初期化
                        _anchorFlag = false;
                        switch (KeyCode)
                        {
                            case Keys.L:
                                GoPost(true);
                                return true;
                            case Keys.H:
                                GoPost(false);
                                return true;
                            case Keys.Z:
                            case Keys.Oemcomma:
                                MoveTop();
                                return true;
                            case Keys.S:
                                GoNextTab(true);
                                return true;
                            case Keys.A:
                                GoNextTab(false);
                                return true;
                            case Keys.Oem4:
                                // ] in_reply_to参照元へ戻る
                                asyncTask = this.GoInReplyToPostTree();
                                return true;
                            case Keys.Oem6:
                                // [ in_reply_toへジャンプ
                                GoBackInReplyToPostTree();
                                return true;
                            case Keys.Escape:
                                if (ListTab.SelectedTab != null)
                                {
                                    MyCommon.TabUsageType tabtype = _statuses.Tabs[ListTab.SelectedTab.Text].TabType;
                                    if (tabtype == MyCommon.TabUsageType.Related || tabtype == MyCommon.TabUsageType.UserTimeline || tabtype == MyCommon.TabUsageType.PublicSearch)
                                    {
                                        TabPage relTp = ListTab.SelectedTab;
                                        RemoveSpecifiedTab(relTp.Text, false);
                                        SaveConfigsTabs();
                                        return true;
                                    }
                                }
                                break;
                        }
                    }
                    else if (Focused == FocusedControl.PostBrowser)
                    {
                        //フォーカスPostBrowser
                        switch (KeyCode)
                        {
                            case Keys.Up:
                            case Keys.Down:
                                //スクロールを発生させるため、true を返す
                                return true;
                        }
                    }
                    break;
                case ModifierState.Ctrl:
                    //フォーカス関係なし
                    switch (KeyCode)
                    {
                        case Keys.R:
                            MakeReplyOrDirectStatus(false, true);
                            return true;
                        case Keys.D:
                            doStatusDelete();
                            return true;
                        case Keys.M:
                            MakeReplyOrDirectStatus(false, false);
                            return true;
                        case Keys.S:
                            asyncTask = this.FavoriteChange(true);
                            return true;
                        case Keys.I:
                            asyncTask = this.doRepliedStatusOpen();
                            return true;
                        case Keys.Q:
                            doQuote();
                            return true;
                        case Keys.B:
                            ReadedStripMenuItem_Click(null, null);
                            return true;
                        case Keys.T:
                            HashManageMenuItem_Click(null, null);
                            return true;
                        case Keys.L:
                            UrlConvertAutoToolStripMenuItem_Click(null, null);
                            return true;
                        case Keys.Y:
                            if (Focused != FocusedControl.PostBrowser)
                            {
                                MultiLineMenuItem_Click(null, null);
                                return true;
                            }
                            break;
                        case Keys.F:
                            MenuItemSubSearch_Click(null, null);
                            return true;
                        case Keys.U:
                            ShowUserTimeline();
                            return true;
                        case Keys.H:
                            // Webページを開く動作
                            MoveToHomeToolStripMenuItem_Click(null, null);
                            return true;
                        case Keys.G:
                            // Webページを開く動作
                            MoveToFavToolStripMenuItem_Click(null, null);
                            return true;
                        case Keys.O:
                            // Webページを開く動作
                            StatusOpenMenuItem_Click(null, null);
                            return true;
                        case Keys.E:
                            // Webページを開く動作
                            OpenURLMenuItem_Click(null, null);
                            return true;
                    }
                    //フォーカスList
                    if (Focused == FocusedControl.ListTab)
                    {
                        switch (KeyCode)
                        {
                            case Keys.Home:
                            case Keys.End:
                                _colorize = true;
                                return false;            //スルーする
                            case Keys.N:
                                GoNextTab(true);
                                return true;
                            case Keys.P:
                                GoNextTab(false);
                                return true;
                            case Keys.C:
                                CopyStot();
                                return true;
                            case Keys.D1:
                            case Keys.D2:
                            case Keys.D3:
                            case Keys.D4:
                            case Keys.D5:
                            case Keys.D6:
                            case Keys.D7:
                            case Keys.D8:
                                // タブダイレクト選択(Ctrl+1～8,Ctrl+9)
                                int tabNo = KeyCode - Keys.D1;
                                if (ListTab.TabPages.Count < tabNo)
                                    return false;
                                ListTab.SelectedIndex = tabNo;
                                return true;
                            case Keys.D9:
                                ListTab.SelectedIndex = ListTab.TabPages.Count - 1;
                                return true;
                        }
                    }
                    else if (Focused == FocusedControl.StatusText)
                    {
                        //フォーカスStatusText
                        switch (KeyCode)
                        {
                            case Keys.A:
                                StatusText.SelectAll();
                                return true;
                            case Keys.Up:
                            case Keys.Down:
                                if (!string.IsNullOrWhiteSpace(StatusText.Text))
                                {
                                    _history[_hisIdx] = new PostingStatus(StatusText.Text, _reply_to_id, _reply_to_name);
                                }
                                if (KeyCode == Keys.Up)
                                {
                                    _hisIdx -= 1;
                                    if (_hisIdx < 0) _hisIdx = 0;
                                }
                                else
                                {
                                    _hisIdx += 1;
                                    if (_hisIdx > _history.Count - 1) _hisIdx = _history.Count - 1;
                                }
                                StatusText.Text = _history[_hisIdx].status;
                                _reply_to_id = _history[_hisIdx].inReplyToId;
                                _reply_to_name = _history[_hisIdx].inReplyToName;
                                StatusText.SelectionStart = StatusText.Text.Length;
                                return true;
                            case Keys.PageUp:
                            case Keys.P:
                                if (ListTab.SelectedIndex == 0)
                                {
                                    ListTab.SelectedIndex = ListTab.TabCount - 1;
                                }
                                else
                                {
                                    ListTab.SelectedIndex -= 1;
                                }
                                StatusText.Focus();
                                return true;
                            case Keys.PageDown:
                            case Keys.N:
                                if (ListTab.SelectedIndex == ListTab.TabCount - 1)
                                {
                                    ListTab.SelectedIndex = 0;
                                }
                                else
                                {
                                    ListTab.SelectedIndex += 1;
                                }
                                StatusText.Focus();
                                return true;
                        }
                    }
                    else
                    {
                        //フォーカスPostBrowserもしくは関係なし
                        switch (KeyCode)
                        {
                            case Keys.Y:
                                MultiLineMenuItem.Checked = !MultiLineMenuItem.Checked;
                                MultiLineMenuItem_Click(null, null);
                                return true;
                        }
                    }
                    break;
                case ModifierState.Shift:
                    //フォーカス関係なし
                    switch (KeyCode)
                    {
                        case Keys.F3:
                            MenuItemSearchPrev_Click(null, null);
                            return true;
                        case Keys.F5:
                            asyncTask = this.DoRefreshMore();
                            return true;
                        case Keys.F6:
                            asyncTask = this.GetReplyAsync(loadMore: true);
                            return true;
                        case Keys.F7:
                            asyncTask = this.GetDirectMessagesAsync(loadMore: true);
                            return true;
                    }
                    //フォーカスStatusText以外
                    if (Focused != FocusedControl.StatusText)
                    {
                        if (KeyCode == Keys.R)
                        {
                            asyncTask = this.DoRefreshMore();
                            return true;
                        }
                    }
                    //フォーカスリスト
                    if (Focused == FocusedControl.ListTab)
                    {
                        switch (KeyCode)
                        {
                            case Keys.H:
                                GoTopEnd(true);
                                return true;
                            case Keys.L:
                                GoTopEnd(false);
                                return true;
                            case Keys.M:
                                GoMiddle();
                                return true;
                            case Keys.G:
                                GoLast();
                                return true;
                            case Keys.Z:
                                MoveMiddle();
                                return true;
                            case Keys.Oem4:
                                GoBackInReplyToPostTree(true, false);
                                return true;
                            case Keys.Oem6:
                                GoBackInReplyToPostTree(true, true);
                                return true;
                            case Keys.N:
                            case Keys.Right:
                                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                                GoFav(true);
                                return true;
                            case Keys.P:
                            case Keys.Left:
                                // お気に入り前後ジャンプ(SHIFT+N←/P→)
                                GoFav(false);
                                return true;
                            case Keys.Space:
                                this.GoBackSelectPostChain();
                                return true;
                        }
                    }
                    break;
                case ModifierState.Alt:
                    switch (KeyCode)
                    {
                        case Keys.R:
                            asyncTask = this.doReTweetOfficial(true);
                            return true;
                        case Keys.P:
                            if (_curPost != null)
                            {
                                asyncTask = this.doShowUserStatus(_curPost.ScreenName, false);
                                return true;
                            }
                            break;
                        case Keys.Up:
                            ScrollDownPostBrowser(false);
                            return true;
                        case Keys.Down:
                            ScrollDownPostBrowser(true);
                            return true;
                        case Keys.PageUp:
                            PageDownPostBrowser(false);
                            return true;
                        case Keys.PageDown:
                            PageDownPostBrowser(true);
                            return true;
                    }
                    if (Focused == FocusedControl.ListTab)
                    {
                        // 別タブの同じ書き込みへ(ALT+←/→)
                        if (KeyCode == Keys.Right)
                        {
                            GoSamePostToAnotherTab(false);
                            return true;
                        }
                        else if (KeyCode == Keys.Left)
                        {
                            GoSamePostToAnotherTab(true);
                            return true;
                        }
                    }
                    break;
                case ModifierState.Ctrl | ModifierState.Shift:
                    switch (KeyCode)
                    {
                        case Keys.R:
                            MakeReplyOrDirectStatus(false, true, true);
                            return true;
                        case Keys.C:
                            CopyIdUri();
                            return true;
                        case Keys.F:
                            if (ListTab.SelectedTab != null)
                            {
                                if (_statuses.Tabs[ListTab.SelectedTab.Text].TabType == MyCommon.TabUsageType.PublicSearch)
                                {
                                    ListTab.SelectedTab.Controls["panelSearch"].Controls["comboSearch"].Focus();
                                    return true;
                                }
                            }
                            break;
                        case Keys.S:
                            asyncTask = this.FavoriteChange(false);
                            return true;
                        case Keys.B:
                            UnreadStripMenuItem_Click(null, null);
                            return true;
                        case Keys.T:
                            HashToggleMenuItem_Click(null, null);
                            return true;
                        case Keys.P:
                            ImageSelectMenuItem_Click(null, null);
                            return true;
                        case Keys.H:
                            asyncTask = this.doMoveToRTHome();
                            return true;
                        case Keys.O:
                            FavorareMenuItem_Click(null, null);
                            return true;
                    }
                    if (Focused == FocusedControl.StatusText)
                    {
                        int idx = 0;
                        switch (KeyCode)
                        {
                            case Keys.Up:
                                if (_curList != null && _curList.VirtualListSize != 0 &&
                                            _curList.SelectedIndices.Count > 0 && _curList.SelectedIndices[0] > 0)
                                {
                                    idx = _curList.SelectedIndices[0] - 1;
                                    SelectListItem(_curList, idx);
                                    _curList.EnsureVisible(idx);
                                    return true;
                                }
                                break;
                            case Keys.Down:
                                if (_curList != null && _curList.VirtualListSize != 0 && _curList.SelectedIndices.Count > 0
                                            && _curList.SelectedIndices[0] < _curList.VirtualListSize - 1)
                                {
                                    idx = _curList.SelectedIndices[0] + 1;
                                    SelectListItem(_curList, idx);
                                    _curList.EnsureVisible(idx);
                                    return true;
                                }
                                break;
                            case Keys.Space:
                                if (StatusText.SelectionStart > 0)
                                {
                                    int endidx = StatusText.SelectionStart - 1;
                                    string startstr = "";
                                    bool pressed = false;
                                    for (int i = StatusText.SelectionStart - 1; i >= 0; i--)
                                    {
                                        char c = StatusText.Text[i];
                                        if (Char.IsLetterOrDigit(c) || c == '_')
                                        {
                                            continue;
                                        }
                                        if (c == '@')
                                        {
                                            pressed = true;
                                            startstr = StatusText.Text.Substring(i + 1, endidx - i);
                                            int cnt = AtIdSupl.ItemCount;
                                            ShowSuplDialog(StatusText, AtIdSupl, startstr.Length + 1, startstr);
                                            if (AtIdSupl.ItemCount != cnt) _modifySettingAtId = true;
                                        }
                                        else if (c == '#')
                                        {
                                            pressed = true;
                                            startstr = StatusText.Text.Substring(i + 1, endidx - i);
                                            ShowSuplDialog(StatusText, HashSupl, startstr.Length + 1, startstr);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    return pressed;
                                }
                                break;
                        }
                    }
                    else if (Focused == FocusedControl.ListTab)
                    {
                        DetailsListView lst = (DetailsListView)ListTab.SelectedTab.Tag;
                        ColumnHeader col;
                        switch (KeyCode)
                        {
                            case Keys.D1:
                            case Keys.D2:
                            case Keys.D3:
                            case Keys.D4:
                            case Keys.D5:
                            case Keys.D6:
                            case Keys.D7:
                            case Keys.D8:
                                // ソートダイレクト選択(Ctrl+Shift+1～8,Ctrl+Shift+9)
                                int colNo = KeyCode - Keys.D1;
                                if (lst.Columns.Count < colNo) return false;
                                col = lst.Columns.Cast<ColumnHeader>().Where((x) => { return x.DisplayIndex == colNo; }).FirstOrDefault();
                                if (col == null) return false;
                                MyList_ColumnClick(lst, new ColumnClickEventArgs(col.Index));
                                return true;
                            case Keys.D9:
                                col = lst.Columns.Cast<ColumnHeader>().OrderByDescending((x) => { return x.DisplayIndex; }).First();
                                MyList_ColumnClick(lst, new ColumnClickEventArgs(col.Index));
                                return true;
                        }
                    }
                    break;
                case ModifierState.Ctrl | ModifierState.Alt:
                    if (KeyCode == Keys.S)
                    {
                        asyncTask = this.FavoritesRetweetOriginal();
                        return true;
                    }
                    else if (KeyCode == Keys.R)
                    {
                        asyncTask = this.FavoritesRetweetUnofficial();
                        return true;
                    }
                    else if (KeyCode == Keys.H)
                    {
                        asyncTask = this.OpenUserAppointUrl();
                        return true;
                    }
                    break;
                case ModifierState.Alt | ModifierState.Shift:
                    if (Focused == FocusedControl.PostBrowser)
                    {
                        if (KeyCode == Keys.R)
                            doReTweetUnofficial();
                        else if (KeyCode == Keys.C)
                            CopyUserId();
                        return true;
                    }
                    switch (KeyCode)
                    {
                        case Keys.T:
                            if (!this.ExistCurrentPost) return false;
                            asyncTask = this.doTranslation(_curPost.TextFromApi);
                            return true;
                        case Keys.R:
                            doReTweetUnofficial();
                            return true;
                        case Keys.C:
                            CopyUserId();
                            return true;
                        case Keys.Up:
                            this.tweetThumbnail1.ScrollUp();
                            return true;
                        case Keys.Down:
                            this.tweetThumbnail1.ScrollDown();
                            return true;
                    }
                    if (Focused == FocusedControl.ListTab && KeyCode == Keys.Enter)
                    {
                        if (!this.SplitContainer3.Panel2Collapsed)
                        {
                            asyncTask = this.OpenThumbnailPicture(this.tweetThumbnail1.Thumbnail);
                        }
                        return true;
                    }
                    break;
            }

            return false;
        }

        private void ScrollDownPostBrowser(bool forward)
        {
            var doc = PostBrowser.Document;
            if (doc == null) return;

            var tags = doc.GetElementsByTagName("html");
            if (tags.Count > 0)
            {
                if (forward)
                    tags[0].ScrollTop += this._fntDetail.Height;
                else
                    tags[0].ScrollTop -= this._fntDetail.Height;
            }
        }

        private void PageDownPostBrowser(bool forward)
        {
            var doc = PostBrowser.Document;
            if (doc == null) return;

            var tags = doc.GetElementsByTagName("html");
            if (tags.Count > 0)
            {
                if (forward)
                    tags[0].ScrollTop += PostBrowser.ClientRectangle.Height - this._fntDetail.Height;
                else
                    tags[0].ScrollTop -= PostBrowser.ClientRectangle.Height - this._fntDetail.Height;
            }
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
                    _anchorPost.ReplyToList.Contains(post.ScreenName.ToLower()) ||
                    _anchorPost.ReplyToList.Contains(post.RetweetedBy.ToLower()) ||
                    post.ReplyToList.Contains(_anchorPost.ScreenName.ToLower()) ||
                    post.ReplyToList.Contains(_anchorPost.RetweetedBy.ToLower()))
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

            TabClass curTabClass = _statuses.Tabs[_curTab.Text];

            if (curTabClass.TabType == MyCommon.TabUsageType.PublicSearch && _curPost.InReplyToStatusId == null && _curPost.TextFromApi.Contains("@"))
            {
                PostClass post = null;
                string r = tw.GetStatusApi(false, _curPost.StatusId, ref post);
                if (string.IsNullOrEmpty(r) && post != null)
                {
                    _curPost.InReplyToStatusId = post.InReplyToStatusId;
                    _curPost.InReplyToUser = post.InReplyToUser;
                    _curPost.IsReply = post.IsReply;
                    this.PurgeListViewItemCache();
                    _curList.RedrawItems(_curItemIndex, _curItemIndex, false);
                }
                else
                {
                    this.StatusLabel.Text = r;
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
                PostClass post = null;
                string r = tw.GetStatusApi(false, _curPost.InReplyToStatusId.Value, ref post);
                if (!string.IsNullOrEmpty(r) || post == null)
                {
                    this.StatusLabel.Text = r;
                    await this.OpenUriInBrowserAsync("https://twitter.com/" + inReplyToUser + "/statuses/" + inReplyToId.ToString());
                    return;
                }

                post.IsRead = true;
                _statuses.AddPost(post);
                _statuses.DistributePosts();
                //_statuses.SubmitUpdate(null, null, null, false);
                this.RefreshTimeline(false);

                inReplyPost = inReplyToPosts.FirstOrDefault();
                if (inReplyPost == null)
                {
                    await this.OpenUriInBrowserAsync("https://twitter.com/" + inReplyToUser + "/statuses/" + inReplyToId.ToString());
                    return;
                }
                inReplyToTabName = inReplyPost.Tab.TabName;
                inReplyToIndex = inReplyPost.Index;
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

            TabClass curTabClass = _statuses.Tabs[_curTab.Text];
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
            ModifierState State = GetModifierState(e.Control, e.Shift, e.Alt);
            if (State == ModifierState.NotFlags) return;

            Task asyncTask;
            if (CommonKeyDown(e.KeyCode, FocusedControl.StatusText, State, out asyncTask))
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
                if (_modifySettingCommon) SaveConfigsCommon();
                if (_modifySettingLocal) SaveConfigsLocal();
                if (_modifySettingAtId) SaveConfigsAtId();
            }
        }

        private void SaveConfigsAtId()
        {
            if (_ignoreConfigSave || !this._cfgCommon.UseAtIdSupplement && AtIdSupl == null) return;

            _modifySettingAtId = false;
            SettingAtIdList cfgAtId = new SettingAtIdList(AtIdSupl.GetItemList());
            cfgAtId.Save();
        }

        private void SaveConfigsCommon()
        {
            if (_ignoreConfigSave) return;

            _modifySettingCommon = false;
            lock (_syncObject)
            {
                _cfgCommon.UserName = tw.Username;
                _cfgCommon.UserId = tw.UserId;
                _cfgCommon.Password = tw.Password;
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
                _modifySettingLocal = false;
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
            SettingTabs tabSetting = new SettingTabs();
            for (int i = 0; i < ListTab.TabPages.Count; i++)
            {
                if (_statuses.Tabs[ListTab.TabPages[i].Text].TabType != MyCommon.TabUsageType.Related) tabSetting.Tabs.Add(_statuses.Tabs[ListTab.TabPages[i].Text]);
            }
            tabSetting.Tabs.Add(this._statuses.GetTabByType(MyCommon.TabUsageType.Mute));
            tabSetting.Save();
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
                                     post.CreatedAt.ToString() + "\t" +
                                     post.ScreenName + "\t" +
                                     post.StatusId.ToString() + "\t" +
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
                                     post.CreatedAt.ToString() + "\t" +
                                     post.ScreenName + "\t" +
                                     post.StatusId.ToString() + "\t" +
                                     post.ImageUrl + "\t" +
                                     "\"" + post.Text.Replace("\n", "").Replace("\"", "\"\"") + "\"" + "\t" +
                                     protect);
                        }
                    }
                }
            }
            this.TopMost = this._cfgCommon.AlwaysTop;
        }

        private async void PostBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ModifierState State = GetModifierState(e.Control, e.Shift, e.Alt);
            if (State == ModifierState.NotFlags) return;

            Task asyncTask;
            bool KeyRes = CommonKeyDown(e.KeyCode, FocusedControl.PostBrowser, State, out asyncTask);
            if (KeyRes)
            {
                e.IsInputKey = true;
            }
            else
            {
                if (Enum.IsDefined(typeof(Shortcut), (Shortcut)e.KeyData))
                {
                    var shortcut = (Shortcut)e.KeyData;
                    switch (shortcut)
                    {
                        case Shortcut.CtrlA:
                        case Shortcut.CtrlC:
                        case Shortcut.CtrlIns:
                            // 既定の動作を有効にする
                            break;
                        default:
                            // その他のショートカットキーは無効にする
                            e.IsInputKey = true;
                            break;
                    }
                }
            }

            if (asyncTask != null)
                await asyncTask;
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
            Point cpos = new Point(e.X, e.Y);
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
                        _reply_to_id = null;
                        _reply_to_name = null;
                        return;
                    }
                    if (string.IsNullOrEmpty(StatusText.Text))
                    {
                        //空の場合

                        // ステータステキストが入力されていない場合先頭に@ユーザー名を追加する
                        StatusText.Text = "@" + _curPost.ScreenName + " ";
                        if (_curPost.RetweetedId != null)
                        {
                            _reply_to_id = _curPost.RetweetedId.Value;
                        }
                        else
                        {
                            _reply_to_id = _curPost.StatusId;
                        }
                        _reply_to_name = _curPost.ScreenName;
                    }
                    else
                    {
                        //何か入力済の場合

                        if (isAuto)
                        {
                            //1件選んでEnter or DoubleClick
                            if (StatusText.Text.Contains("@" + _curPost.ScreenName + " "))
                            {
                                if (_reply_to_id != null && _reply_to_name == _curPost.ScreenName)
                                {
                                    //返信先書き換え
                                    if (_curPost.RetweetedId != null)
                                    {
                                        _reply_to_id = _curPost.RetweetedId.Value;
                                    }
                                    else
                                    {
                                        _reply_to_id = _curPost.StatusId;
                                    }
                                    _reply_to_name = _curPost.ScreenName;
                                }
                                return;
                            }
                            if (!StatusText.Text.StartsWith("@"))
                            {
                                //文頭＠以外
                                if (StatusText.Text.StartsWith(". "))
                                {
                                    // 複数リプライ
                                    StatusText.Text = StatusText.Text.Insert(2, "@" + _curPost.ScreenName + " ");
                                    _reply_to_id = null;
                                    _reply_to_name = null;
                                }
                                else
                                {
                                    // 単独リプライ
                                    StatusText.Text = "@" + _curPost.ScreenName + " " + StatusText.Text;
                                    if (_curPost.RetweetedId != null)
                                    {
                                        _reply_to_id = _curPost.RetweetedId.Value;
                                    }
                                    else
                                    {
                                        _reply_to_id = _curPost.StatusId;
                                    }
                                    _reply_to_name = _curPost.ScreenName;
                                }
                            }
                            else
                            {
                                //文頭＠
                                // 複数リプライ
                                StatusText.Text = ". @" + _curPost.ScreenName + " " + StatusText.Text;
                                //StatusText.Text = "@" + _curPost.ScreenName + " " + StatusText.Text;
                                _reply_to_id = null;
                                _reply_to_name = null;
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
                        if (!sTxt.StartsWith(". "))
                        {
                            sTxt = ". " + sTxt;
                            _reply_to_id = null;
                            _reply_to_name = null;
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
                            if (!StatusText.Text.StartsWith(". "))
                            {
                                StatusText.Text = ". " + StatusText.Text;
                                sidx += 2;
                                _reply_to_id = null;
                                _reply_to_name = null;
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
                                if (post.RetweetedId != null)
                                {
                                    _reply_to_id = post.RetweetedId.Value;
                                }
                                else
                                {
                                    _reply_to_id = post.StatusId;
                                }
                                _reply_to_name = post.ScreenName;
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

        private async Task RefreshTasktrayIcon(bool forceRefresh)
        {
            if (_colorize)
                await this.Colorize();

            if (!TimerRefreshIcon.Enabled) return;
            //Static usCheckCnt As int = 0

            //Static iconDlListTopItem As ListViewItem = null

            if (forceRefresh) idle = false;

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

            TabClass tb = _statuses.GetTabByType(MyCommon.TabUsageType.Mentions);
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
            await this.RefreshTasktrayIcon(false);
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

            TabClass tb = _statuses.Tabs[_rclickTabName];
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
                inputName.TabName = _statuses.GetUniqueTabName();
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
                if (!_statuses.AddTab(tabName, tabUsage, list) || !AddNewTab(tabName, false, tabUsage, list))
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
                        var tab = this._statuses.Tabs[this._curTab.Text];
                        this.GetListTimelineAsync(tab);
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
            string tabName;

            //未選択なら処理終了
            if (_curList.SelectedIndices.Count == 0) return;

            //タブ選択（or追加）
            if (!SelectTab(out tabName)) return;

            var tab = this._statuses.Tabs[tabName];

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

            List<string> ids = new List<string>();
            foreach (int idx in _curList.SelectedIndices)
            {
                PostClass post = _statuses.Tabs[_curTab.Text][idx];
                if (!ids.Contains(post.ScreenName))
                {
                    PostFilterRule fc = new PostFilterRule();
                    ids.Add(post.ScreenName);
                    if (post.RetweetedId == null)
                    {
                        fc.FilterName = post.ScreenName;
                    }
                    else
                    {
                        fc.FilterName = post.RetweetedBy;
                    }
                    fc.UseNameField = true;
                    fc.MoveMatches = mv;
                    fc.MarkMatches = mk;
                    fc.UseRegex = false;
                    fc.FilterByUrl = false;
                    tab.AddFilter(fc);
                }
            }
            if (ids.Count != 0)
            {
                List<string> atids = new List<string>();
                foreach (string id in ids)
                {
                    atids.Add("@" + id);
                }
                int cnt = AtIdSupl.ItemCount;
                AtIdSupl.AddRangeItem(atids.ToArray());
                if (AtIdSupl.ItemCount != cnt) _modifySettingAtId = true;
            }

            this.ApplyPostFilters();
            SaveConfigsTabs();
        }

        private void SourceRuleMenuItem_Click(object sender, EventArgs e)
        {
            if (this._curList.SelectedIndices.Count == 0)
                return;

            // タブ選択ダイアログを表示（or追加）
            string tabName;
            if (!this.SelectTab(out tabName))
                return;

            var currentTab = this._statuses.Tabs[this._curTab.Text];
            var filterTab = this._statuses.Tabs[tabName];

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
            var sources = new HashSet<string>();

            foreach (var idx in this._curList.SelectedIndices.Cast<int>())
            {
                var post = currentTab[idx];
                var filterSource = post.Source;

                if (sources.Add(filterSource))
                {
                    var filter = new PostFilterRule
                    {
                        FilterSource = filterSource,
                        MoveMatches = mv,
                        MarkMatches = mk,
                        UseRegex = false,
                        FilterByUrl = false,
                    };
                    filterTab.AddFilter(filter);
                }
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
                        inputName.TabName = _statuses.GetUniqueTabName();
                        inputName.ShowDialog();
                        if (inputName.DialogResult == DialogResult.Cancel) return false;
                        tabName = inputName.TabName;
                    }
                    this.TopMost = this._cfgCommon.AlwaysTop;
                    if (!string.IsNullOrEmpty(tabName))
                    {
                        if (!_statuses.AddTab(tabName, MyCommon.TabUsageType.UserDefined, null) || !AddNewTab(tabName, false, MyCommon.TabUsageType.UserDefined))
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
            var linkElements = this.PostBrowser.Document.Links.Cast<HtmlElement>()
                .Where(x => x.GetAttribute("className") != "tweet-quote-link") // 引用ツイートで追加されたリンクを除く
                .ToArray();

            if (linkElements.Length > 0)
            {
                UrlDialog.ClearUrl();

                string openUrlStr = "";

                if (linkElements.Length == 1)
                {
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
                }
                else
                {
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
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    this.TopMost = this._cfgCommon.AlwaysTop;
                }
                if (string.IsNullOrEmpty(openUrlStr)) return;

                await this.OpenUriAsync(new Uri(openUrlStr));
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
            TabClass tbRep = _statuses.GetTabByType(MyCommon.TabUsageType.Mentions);
            TabClass tbDm = _statuses.GetTabByType(MyCommon.TabUsageType.DirectMessage);
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

            slbl.AppendFormat(Properties.Resources.SetStatusLabelText1, tur, tal, ur, al, urat, _postTimestamps.Count, _favTimestamps.Count, _tlCount);
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

        private void TwitterApiStatus_AccessLimitUpdated(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)(() => this.TwitterApiStatus_AccessLimitUpdated(sender, e)));
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

        private static StringBuilder ur = new StringBuilder(64);

        private void SetNotifyIconText()
        {
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
                if (bCnt != AtIdSupl.ItemCount) _modifySettingAtId = true;
            }

            // リプライ先ステータスIDの指定がない場合は指定しない
            if (_reply_to_id == null) return;

            // リプライ先ユーザー名がない場合も指定しない
            if (string.IsNullOrEmpty(_reply_to_name))
            {
                _reply_to_id = null;
                return;
            }

            // 通常Reply
            // 次の条件を満たす場合に in_reply_to_status_id 指定
            // 1. Twitterによりリンクと判定される @idが文中に1つ含まれる (2009/5/28 リンク化される@IDのみカウントするように修正)
            // 2. リプライ先ステータスIDが設定されている(リストをダブルクリックで返信している)
            // 3. 文中に含まれた@idがリプライ先のポスト者のIDと一致する

            if (m != null)
            {
                if (StatusText.StartsWith("@"))
                {
                    if (StatusText.StartsWith("@" + _reply_to_name)) return;
                }
                else
                {
                    foreach (Match mid in m)
                    {
                        if (StatusText.Contains("QT " + mid.Result("${id}") + ":") && mid.Result("${id}") == "@" + _reply_to_name) return;
                    }
                }
            }

            _reply_to_id = null;
            _reply_to_name = null;

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
            _modifySettingCommon = true;
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _mySpDis = SplitContainer1.SplitterDistance;
                if (StatusText.Multiline) _mySpDis2 = StatusText.Height;
                _modifySettingLocal = true;
            }
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
                    MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi);
                }
                else
                {
                    foreach (TabClass tb in _statuses.GetTabsByType(MyCommon.TabUsageType.Lists | MyCommon.TabUsageType.PublicSearch))
                    {
                        if (tb == null || !tb.Contains(_curPost.InReplyToStatusId.Value)) break;
                        PostClass repPost = _statuses[_curPost.InReplyToStatusId.Value];
                        MessageBox.Show(repPost.ScreenName + " / " + repPost.Nickname + "   (" + repPost.CreatedAt.ToString() + ")" + Environment.NewLine + repPost.TextFromApi);
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

        /// <summary>
        /// UserPicture.Image に設定されている画像を破棄します。
        /// </summary>
        private void ClearUserPicture()
        {
            if (this.UserPicture.Image != null)
            {
                var oldImage = this.UserPicture.Image;
                this.UserPicture.Image = null;
                oldImage.Dispose();
            }
        }

        private void ContextMenuUserPicture_Opening(object sender, CancelEventArgs e)
        {
            //発言詳細のアイコン右クリック時のメニュー制御
            if (_curList.SelectedIndices.Count > 0 && _curPost != null)
            {
                string name = _curPost.ImageUrl;
                if (name != null && name.Length > 0)
                {
                    int idx = name.LastIndexOf('/');
                    if (idx != -1)
                    {
                        name = Path.GetFileName(name.Substring(idx));
                        if (name.Contains("_normal.") || name.EndsWith("_normal"))
                        {
                            name = name.Replace("_normal", "");
                            this.IconNameToolStripMenuItem.Text = name;
                            this.IconNameToolStripMenuItem.Enabled = true;
                        }
                        else
                        {
                            this.IconNameToolStripMenuItem.Enabled = false;
                            this.IconNameToolStripMenuItem.Text = Properties.Resources.ContextMenuStrip3_OpeningText1;
                        }
                    }
                    else
                    {
                        this.IconNameToolStripMenuItem.Enabled = false;
                        this.IconNameToolStripMenuItem.Text = Properties.Resources.ContextMenuStrip3_OpeningText1;
                    }

                    this.ReloadIconToolStripMenuItem.Enabled = true;

                    if (this.IconCache.TryGetFromCache(_curPost.ImageUrl) != null)
                    {
                        this.SaveIconPictureToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        this.SaveIconPictureToolStripMenuItem.Enabled = false;
                    }
                }
                else
                {
                    this.IconNameToolStripMenuItem.Enabled = false;
                    this.ReloadIconToolStripMenuItem.Enabled = false;
                    this.SaveIconPictureToolStripMenuItem.Enabled = false;
                    this.IconNameToolStripMenuItem.Text = Properties.Resources.ContextMenuStrip3_OpeningText1;
                }
            }
            else
            {
                this.IconNameToolStripMenuItem.Enabled = false;
                this.ReloadIconToolStripMenuItem.Enabled = false;
                this.SaveIconPictureToolStripMenuItem.Enabled = false;
                this.IconNameToolStripMenuItem.Text = Properties.Resources.ContextMenuStrip3_OpeningText2;
            }
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id == tw.Username)
                {
                    FollowToolStripMenuItem.Enabled = false;
                    UnFollowToolStripMenuItem.Enabled = false;
                    ShowFriendShipToolStripMenuItem.Enabled = false;
                    ShowUserStatusToolStripMenuItem.Enabled = true;
                    SearchPostsDetailNameToolStripMenuItem.Enabled = true;
                    SearchAtPostsDetailNameToolStripMenuItem.Enabled = false;
                    ListManageUserContextToolStripMenuItem3.Enabled = true;
                }
                else
                {
                    FollowToolStripMenuItem.Enabled = true;
                    UnFollowToolStripMenuItem.Enabled = true;
                    ShowFriendShipToolStripMenuItem.Enabled = true;
                    ShowUserStatusToolStripMenuItem.Enabled = true;
                    SearchPostsDetailNameToolStripMenuItem.Enabled = true;
                    SearchAtPostsDetailNameToolStripMenuItem.Enabled = true;
                    ListManageUserContextToolStripMenuItem3.Enabled = true;
                }
            }
            else
            {
                FollowToolStripMenuItem.Enabled = false;
                UnFollowToolStripMenuItem.Enabled = false;
                ShowFriendShipToolStripMenuItem.Enabled = false;
                ShowUserStatusToolStripMenuItem.Enabled = false;
                SearchPostsDetailNameToolStripMenuItem.Enabled = false;
                SearchAtPostsDetailNameToolStripMenuItem.Enabled = false;
                ListManageUserContextToolStripMenuItem3.Enabled = false;
            }
        }

        private async void IconNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_curPost == null) return;
            string name = _curPost.ImageUrl;
            await this.OpenUriInBrowserAsync(name.Remove(name.LastIndexOf("_normal"), 7)); // "_normal".Length
        }

        private async void ReloadIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._curPost == null) return;

            await this.UserPicture.SetImageFromTask(async () =>
            {
                var imageUrl = this._curPost.ImageUrl;

                var image = await this.IconCache.DownloadImageAsync(imageUrl, force: true)
                    .ConfigureAwait(false);

                return await image.CloneAsync()
                    .ConfigureAwait(false);
            });
        }

        private void SaveOriginalSizeIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_curPost == null) return;
            string name = _curPost.ImageUrl;
            name = Path.GetFileNameWithoutExtension(name.Substring(name.LastIndexOf('/')));

            this.SaveFileDialog1.FileName = name.Substring(0, name.Length - 8); // "_normal".Length + 1

            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // STUB
            }
        }

        private void SaveIconPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_curPost == null) return;
            string name = _curPost.ImageUrl;

            this.SaveFileDialog1.FileName = name.Substring(name.LastIndexOf('/') + 1);

            if (this.SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Image orgBmp = new Bitmap(IconCache.TryGetFromCache(name).Image))
                    {
                        using (Bitmap bmp2 = new Bitmap(orgBmp.Size.Width, orgBmp.Size.Height))
                        {
                            using (Graphics g = Graphics.FromImage(bmp2))
                            {
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                                g.DrawImage(orgBmp, 0, 0, orgBmp.Size.Width, orgBmp.Size.Height);
                            }
                            bmp2.Save(this.SaveFileDialog1.FileName);
                        }
                    }
                }
                catch (Exception)
                {
                    //処理中にキャッシュアウトする可能性あり
                }
            }
        }

        private void SplitContainer2_Panel2_Resize(object sender, EventArgs e)
        {
            this.StatusText.Multiline = this.SplitContainer2.Panel2.Height > this.SplitContainer2.Panel2MinSize + 2;
            MultiLineMenuItem.Checked = this.StatusText.Multiline;
            _modifySettingLocal = true;
        }

        private void StatusText_MultilineChanged(object sender, EventArgs e)
        {
            if (this.StatusText.Multiline)
                this.StatusText.ScrollBars = ScrollBars.Vertical;
            else
                this.StatusText.ScrollBars = ScrollBars.None;

            _modifySettingLocal = true;
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
            _modifySettingLocal = true;
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
                if (tmp.StartsWith("http"))
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
            _modifySettingCommon = true;
        }

        private void ListLockMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            ListLockMenuItem.Checked = ((ToolStripMenuItem)sender).Checked;
            this.LockListFileMenuItem.Checked = ListLockMenuItem.Checked;
            _cfgCommon.ListLock = ListLockMenuItem.Checked;
            _modifySettingCommon = true;
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
            _modifySettingLocal = true;
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
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width3 != lst.Columns[1].Width)
                {
                    _cfgLocal.Width3 = lst.Columns[1].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
            }
            else
            {
                if (_cfgLocal.Width1 != lst.Columns[0].Width)
                {
                    _cfgLocal.Width1 = lst.Columns[0].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width2 != lst.Columns[1].Width)
                {
                    _cfgLocal.Width2 = lst.Columns[1].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width3 != lst.Columns[2].Width)
                {
                    _cfgLocal.Width3 = lst.Columns[2].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width4 != lst.Columns[3].Width)
                {
                    _cfgLocal.Width4 = lst.Columns[3].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width5 != lst.Columns[4].Width)
                {
                    _cfgLocal.Width5 = lst.Columns[4].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width6 != lst.Columns[5].Width)
                {
                    _cfgLocal.Width6 = lst.Columns[5].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width7 != lst.Columns[6].Width)
                {
                    _cfgLocal.Width7 = lst.Columns[6].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
                if (_cfgLocal.Width8 != lst.Columns[7].Width)
                {
                    _cfgLocal.Width8 = lst.Columns[7].Width;
                    _modifySettingLocal = true;
                    _isColumnChanged = true;
                }
            }
            // 非表示の時にColumnChangedが呼ばれた場合はForm初期化処理中なので保存しない
            //if (changed)
            //{
            //    SaveConfigsLocal();
            //}
        }

        private void SelectionCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            //発言詳細で「選択文字列をコピー」
            string _selText = this.PostBrowser.GetSelectedText();
            try
            {
                Clipboard.SetDataObject(_selText, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task doSearchToolStrip(string url)
        {
            //発言詳細で「選択文字列で検索」（選択文字列取得）
            string _selText = this.PostBrowser.GetSelectedText();

            if (_selText != null)
            {
                if (url == Properties.Resources.SearchItem4Url)
                {
                    //公式検索
                    AddNewTabForSearch(_selText);
                    return;
                }

                string tmp = string.Format(url, Uri.EscapeDataString(_selText));
                await this.OpenUriInBrowserAsync(tmp);
            }
        }

        private void SelectionAllContextMenuItem_Click(object sender, EventArgs e)
        {
            //発言詳細ですべて選択
            PostBrowser.Document.ExecCommand("SelectAll", false, null);
        }

        private async void SearchWikipediaContextMenuItem_Click(object sender, EventArgs e)
        {
            await this.doSearchToolStrip(Properties.Resources.SearchItem1Url);
        }

        private async void SearchGoogleContextMenuItem_Click(object sender, EventArgs e)
        {
            await this.doSearchToolStrip(Properties.Resources.SearchItem2Url);
        }

        private async void SearchPublicSearchContextMenuItem_Click(object sender, EventArgs e)
        {
            await this.doSearchToolStrip(Properties.Resources.SearchItem4Url);
        }

        private void UrlCopyContextMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MatchCollection mc = Regex.Matches(this.PostBrowser.DocumentText, @"<a[^>]*href=""(?<url>" + this._postBrowserStatusText.Replace(".", @"\.") + @")""[^>]*title=""(?<title>https?://[^""]+)""", RegexOptions.IgnoreCase);
                foreach (Match m in mc)
                {
                    if (m.Groups["url"].Value == this._postBrowserStatusText)
                    {
                        Clipboard.SetDataObject(m.Groups["title"].Value, false, 5, 100);
                        break;
                    }
                }
                if (mc.Count == 0)
                {
                    Clipboard.SetDataObject(this._postBrowserStatusText, false, 5, 100);
                }
                //Clipboard.SetDataObject(this._postBrowserStatusText, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ContextMenuPostBrowser_Opening(object ender, CancelEventArgs e)
        {
            // URLコピーの項目の表示/非表示
            if (PostBrowser.StatusText.StartsWith("http"))
            {
                this._postBrowserStatusText = PostBrowser.StatusText;
                string name = GetUserId();
                UrlCopyContextMenuItem.Enabled = true;
                if (name != null)
                {
                    FollowContextMenuItem.Enabled = true;
                    RemoveContextMenuItem.Enabled = true;
                    FriendshipContextMenuItem.Enabled = true;
                    ShowUserStatusContextMenuItem.Enabled = true;
                    SearchPostsDetailToolStripMenuItem.Enabled = true;
                    IdFilterAddMenuItem.Enabled = true;
                    ListManageUserContextToolStripMenuItem.Enabled = true;
                    SearchAtPostsDetailToolStripMenuItem.Enabled = true;
                }
                else
                {
                    FollowContextMenuItem.Enabled = false;
                    RemoveContextMenuItem.Enabled = false;
                    FriendshipContextMenuItem.Enabled = false;
                    ShowUserStatusContextMenuItem.Enabled = false;
                    SearchPostsDetailToolStripMenuItem.Enabled = false;
                    IdFilterAddMenuItem.Enabled = false;
                    ListManageUserContextToolStripMenuItem.Enabled = false;
                    SearchAtPostsDetailToolStripMenuItem.Enabled = false;
                }

                if (Regex.IsMatch(this._postBrowserStatusText, @"^https?://twitter.com/search\?q=%23"))
                    UseHashtagMenuItem.Enabled = true;
                else
                    UseHashtagMenuItem.Enabled = false;
            }
            else
            {
                this._postBrowserStatusText = "";
                UrlCopyContextMenuItem.Enabled = false;
                FollowContextMenuItem.Enabled = false;
                RemoveContextMenuItem.Enabled = false;
                FriendshipContextMenuItem.Enabled = false;
                ShowUserStatusContextMenuItem.Enabled = false;
                SearchPostsDetailToolStripMenuItem.Enabled = false;
                SearchAtPostsDetailToolStripMenuItem.Enabled = false;
                UseHashtagMenuItem.Enabled = false;
                IdFilterAddMenuItem.Enabled = false;
                ListManageUserContextToolStripMenuItem.Enabled = false;
            }
            // 文字列選択されていないときは選択文字列関係の項目を非表示に
            string _selText = this.PostBrowser.GetSelectedText();
            if (_selText == null)
            {
                SelectionSearchContextMenuItem.Enabled = false;
                SelectionCopyContextMenuItem.Enabled = false;
                SelectionTranslationToolStripMenuItem.Enabled = false;
            }
            else
            {
                SelectionSearchContextMenuItem.Enabled = true;
                SelectionCopyContextMenuItem.Enabled = true;
                SelectionTranslationToolStripMenuItem.Enabled = true;
            }
            //発言内に自分以外のユーザーが含まれてればフォロー状態全表示を有効に
            MatchCollection ma = Regex.Matches(this.PostBrowser.DocumentText, @"href=""https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?""");
            bool fAllFlag = false;
            foreach (Match mu in ma)
            {
                if (mu.Result("${ScreenName}").ToLower() != tw.Username.ToLower())
                {
                    fAllFlag = true;
                    break;
                }
            }
            this.FriendshipAllMenuItem.Enabled = fAllFlag;

            if (_curPost == null)
                TranslationToolStripMenuItem.Enabled = false;
            else
                TranslationToolStripMenuItem.Enabled = true;

            e.Cancel = false;
        }

        private void CurrentTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //発言詳細の選択文字列で現在のタブを検索
            string _selText = this.PostBrowser.GetSelectedText();

            if (_selText != null)
            {
                var searchOptions = new SearchWordDialog.SearchOptions(
                    SearchWordDialog.SearchType.Timeline,
                    _selText,
                    newTab: false,
                    caseSensitive: false,
                    useRegex: false);

                this.SearchDialog.ResultOptions = searchOptions;

                this.DoTabSearch(
                    searchOptions.Query,
                    searchOptions.CaseSensitive,
                    searchOptions.UseRegex,
                    SEARCHTYPE.NextSearch);
            }
        }

        private void SplitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (StatusText.Multiline) _mySpDis2 = StatusText.Height;
            _modifySettingLocal = true;
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
                        throw new ArgumentException("不正な text/x-moz-url フォーマットです", "data");

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
                        throw new ArgumentException("不正な IESiteModeToUrl フォーマットです", "data");

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

        public async Task OpenUriAsync(Uri uri)
        {
            var uriStr = uri.AbsoluteUri;

            // ツイートURL
            var statusUriMatch = Twitter.StatusUrlRegex.Match(uriStr);
            if (statusUriMatch.Success)
            {
                var statusId = long.Parse(statusUriMatch.Groups["StatusId"].Value);
                await this.OpenRelatedTab(statusId);
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
            // Ctrlを押しながらリンクをクリックした場合は設定と逆の動作をする
            if (this._cfgCommon.OpenUserTimeline && !MyCommon.IsKeyDown(Keys.Control) ||
                !this._cfgCommon.OpenUserTimeline && MyCommon.IsKeyDown(Keys.Control))
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
                        if (configBrowserPath.StartsWith("\"") && configBrowserPath.Length > 2 && configBrowserPath.IndexOf("\"", 2) > -1)
                        {
                            int sep = configBrowserPath.IndexOf("\"", 2);
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

            MenuItemUserStream.Text = "&UserStream ■";
            MenuItemUserStream.Enabled = true;
            StopToolStripMenuItem.Text = "&Start";
            StopToolStripMenuItem.Enabled = true;
            if (this._cfgCommon.UserstreamStartup) tw.StartUserStream();
        }

        private async void TweenMain_Shown(object sender, EventArgs e)
        {
            try
            {
                PostBrowser.Url = new Uri("about:blank");
                PostBrowser.DocumentText = "";       //発言詳細部初期化
            }
            catch (Exception)
            {
            }

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

        private void doReTweetUnofficial()
        {
            //RT @id:内容
            if (this.ExistCurrentPost)
            {
                if (_curPost.IsDm ||
                    !StatusText.Enabled) return;

                if (_curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }
                string rtdata = _curPost.Text;
                rtdata = CreateRetweetUnofficial(rtdata, this.StatusText.Multiline);

                this._reply_to_id = null;
                this._reply_to_name = null;

                StatusText.Text = "RT @" + _curPost.ScreenName + ": " + rtdata;

                StatusText.SelectionStart = 0;
                StatusText.Focus();
            }
        }

        private void ReTweetStripMenuItem_Click(object sender, EventArgs e)
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

        private async void ReTweetOriginalStripMenuItem_Click(object sender, EventArgs e)
        {
            await this.doReTweetOfficial(true);
        }

        private async Task FavoritesRetweetOriginal()
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

                var task = Task.Run(() => this.tw.GetInfoApi());
                apiStatus = await dialog.WaitForAsync(this, task);

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
            string id = "";
            if (_curPost != null) id = _curPost.ScreenName;

            await this.FollowCommand(id);
        }

        private async Task FollowCommand(string id)
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
                var task = Task.Run(() => this.tw.PostFollowCommand(id));
                var err = await dialog.WaitForAsync(this, task);
                if (!string.IsNullOrEmpty(err))
                {
                    MessageBox.Show(Properties.Resources.FRMessage2 + err);
                    return;
                }
            }

            MessageBox.Show(Properties.Resources.FRMessage3);
        }

        private async void RemoveCommandMenuItem_Click(object sender, EventArgs e)
        {
            string id = "";
            if (_curPost != null) id = _curPost.ScreenName;

            await this.RemoveCommand(id, false);
        }

        private async Task RemoveCommand(string id, bool skipInput)
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
                var task = Task.Run(() => this.tw.PostRemoveCommand(id));
                var err = await dialog.WaitForAsync(this, task);
                if (!string.IsNullOrEmpty(err))
                {
                    MessageBox.Show(Properties.Resources.FRMessage2 + err);
                    return;
                }
            }

            MessageBox.Show(Properties.Resources.FRMessage3);
        }

        private async void FriendshipMenuItem_Click(object sender, EventArgs e)
        {
            string id = "";
            if (_curPost != null)
                id = _curPost.ScreenName;

            await this.ShowFriendship(id);
        }

        private async Task ShowFriendship(string id)
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

            var isFollowing = false;
            var isFollowed = false;

            using (var dialog = new WaitingDialog(Properties.Resources.ShowFriendshipText1))
            {
                var cancellationToken = dialog.EnableCancellation();

                var task = Task.Run(() => this.tw.GetFriendshipInfo(id, ref isFollowing, ref isFollowed));
                var err = await dialog.WaitForAsync(this, task);

                if (cancellationToken.IsCancellationRequested)
                    return;

                if (!string.IsNullOrEmpty(err))
                {
                    MessageBox.Show(err);
                    return;
                }
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

        private async Task ShowFriendship(string[] ids)
        {
            foreach (string id in ids)
            {
                var isFollowing = false;
                var isFollowed = false;

                using (var dialog = new WaitingDialog(Properties.Resources.ShowFriendshipText1))
                {
                    var cancellationToken = dialog.EnableCancellation();

                    var task = Task.Run(() => this.tw.GetFriendshipInfo(id, ref isFollowing, ref isFollowed));
                    var err = await dialog.WaitForAsync(this, task);

                    if (cancellationToken.IsCancellationRequested)
                        return;

                    if (!string.IsNullOrEmpty(err))
                    {
                        MessageBox.Show(err);
                        continue;
                    }
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
                return !this.tw.Configuration.NonUsernamePaths.Contains(name.ToLower());
        }

        private string GetUserId()
        {
            Match m = Regex.Match(this._postBrowserStatusText, @"^https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?$");
            if (m.Success && IsTwitterId(m.Result("${ScreenName}")))
                return m.Result("${ScreenName}");
            else
                return null;
        }

        private async void FollowContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
                await this.FollowCommand(name);
        }

        private async void RemoveContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
                await this.RemoveCommand(name, false);
        }

        private async void FriendshipContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
                await this.ShowFriendship(name);
        }

        private async void FriendshipAllMenuItem_Click(object sender, EventArgs e)
        {
            MatchCollection ma = Regex.Matches(this.PostBrowser.DocumentText, @"href=""https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/[0-9]+)?""");
            List<string> ids = new List<string>();
            foreach (Match mu in ma)
            {
                if (mu.Result("${ScreenName}").ToLower() != tw.Username.ToLower())
                {
                    ids.Add(mu.Result("${ScreenName}"));
                }
            }

            await this.ShowFriendship(ids.ToArray());
        }

        private async void ShowUserStatusContextMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
                await this.ShowUserStatus(name);
        }

        private void SearchPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null) AddNewTabForUserTimeline(name);
        }

        private void SearchAtPostsDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null) AddNewTabForSearch("@" + name);
        }

        private void IdeographicSpaceToSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _modifySettingCommon = true;
        }

        private void ToolStripFocusLockMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            _modifySettingCommon = true;
        }

        private void doQuote()
        {
            //QT @id:内容
            //返信先情報付加
            if (this.ExistCurrentPost)
            {
                if (_curPost.IsDm ||
                    !StatusText.Enabled) return;

                if (_curPost.IsProtect)
                {
                    MessageBox.Show("Protected.");
                    return;
                }
                string rtdata = _curPost.Text;
                rtdata = CreateRetweetUnofficial(rtdata, this.StatusText.Multiline);

                StatusText.Text = " QT @" + _curPost.ScreenName + ": " + rtdata;
                if (_curPost.RetweetedId == null)
                {
                    _reply_to_id = _curPost.StatusId;
                }
                else
                {
                    _reply_to_id = _curPost.RetweetedId.Value;
                }
                _reply_to_name = _curPost.ScreenName;

                StatusText.SelectionStart = 0;
                StatusText.Focus();
            }
        }

        private void QuoteStripMenuItem_Click(object sender, EventArgs e) // Handles QuoteStripMenuItem.Click, QtOpMenuItem.Click
        {
            doQuote();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            //公式検索
            Control pnl = ((Control)sender).Parent;
            if (pnl == null) return;
            string tbName = pnl.Parent.Text;
            TabClass tb = _statuses.Tabs[tbName];
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

            tb.SearchWords = cmb.Text;
            tb.SearchLang = cmbLang.Text;
            if (string.IsNullOrEmpty(cmb.Text))
            {
                listView.Focus();
                SaveConfigsTabs();
                return;
            }
            if (tb.IsSearchQueryChanged)
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

                TabClass tb = _statuses.RemovedTab.Pop();
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
                            renamed = TabName + i.ToString();
                        }
                        tb.TabName = renamed;
                        AddNewTab(renamed, false, tb.TabType, tb.ListInfo);
                        _statuses.Tabs.Add(renamed, tb);  // 後に

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
                        renamed = tb.TabName + "(" + i.ToString() + ")";
                    }
                    tb.TabName = renamed;
                    _statuses.Tabs.Add(renamed, tb);  // 先に
                    AddNewTab(renamed, false, tb.TabType, tb.ListInfo);

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

        private void IdFilterAddMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetUserId();
            if (name != null)
            {
                string tabName;

                //未選択なら処理終了
                if (_curList.SelectedIndices.Count == 0) return;

                //タブ選択（or追加）
                if (!SelectTab(out tabName)) return;

                var tab = this._statuses.Tabs[tabName];

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

                PostFilterRule fc = new PostFilterRule();
                fc.FilterName = name;
                fc.UseNameField = true;
                fc.MoveMatches = mv;
                fc.MarkMatches = mk;
                fc.UseRegex = false;
                fc.FilterByUrl = false;
                tab.AddFilter(fc);

                this.ApplyPostFilters();
                SaveConfigsTabs();
            }
        }

        private void ListManageUserContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string user;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

            if (menuItem.Owner == this.ContextMenuPostBrowser)
            {
                user = GetUserId();
                if (user == null) return;
            }
            else if (this._curPost != null)
            {
                user = this._curPost.ScreenName;
            }
            else
            {
                return;
            }

            if (TabInformations.GetInstance().SubscribableLists.Count == 0)
            {
                string res = this.tw.GetListsApi();

                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show("Failed to get lists. (" + res + ")");
                    return;
                }
            }

            using (MyLists listSelectForm = new MyLists(user, this.tw))
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

        private void UseHashtagMenuItem_Click(object sender, EventArgs e)
        {
            Match m = Regex.Match(this._postBrowserStatusText, @"^https?://twitter.com/search\?q=%23(?<hash>.+)$");
            if (m.Success)
            {
                HashMgr.SetPermanentHash("#" + Uri.UnescapeDataString(m.Result("${hash}")));
                HashStripSplitButton.Text = HashMgr.UseHash;
                HashToggleMenuItem.Checked = true;
                HashToggleToolStripMenuItem.Checked = true;
                //使用ハッシュタグとして設定
                _modifySettingCommon = true;
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
            _modifySettingCommon = true;
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
            _modifySettingCommon = true;
            this.StatusText_TextChanged(null, null);
        }

        private void HashStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            HashToggleMenuItem_Click(null, null);
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
                    this.RtOpMenuItem.Enabled = false;
                    this.FavoriteRetweetMenuItem.Enabled = false;
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
            if (this.WindowState == FormWindowState.Normal && !_initialLayout)
            {
                _mySpDis3 = SplitContainer3.SplitterDistance;
                _modifySettingLocal = true;
            }
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
            string id = "";
            if (_curPost != null)
            {
                id = _curPost.ScreenName;
            }

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

                var task = Task.Run(() => this.tw.GetUserInfo(id, ref user));
                var err = await dialog.WaitForAsync(this, task);

                if (cancellationToken.IsCancellationRequested)
                    return;

                if (!string.IsNullOrEmpty(err))
                {
                    MessageBox.Show(err);
                    return;
                }
            }

            await this.doShowUserStatus(user);
        }

        private async Task doShowUserStatus(TwitterUser user)
        {
            using (var userDialog = new UserInfoDialog(this, this.tw))
            {
                var showUserTask = userDialog.ShowUserAsync(user);
                userDialog.ShowDialog(this);

                this.Activate();
                this.BringToFront();

                // ユーザー情報の表示が完了するまで userDialog を破棄しない
                await showUserTask;
            }
        }

        private Task ShowUserStatus(string id, bool ShowInputDialog)
        {
            return this.doShowUserStatus(id, ShowInputDialog);
        }

        private Task ShowUserStatus(string id)
        {
            return this.doShowUserStatus(id, true);
        }

        private async void FollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != tw.Username)
                {
                    await this.FollowCommand(id);
                }
            }
        }

        private async void UnFollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != tw.Username)
                {
                    await this.RemoveCommand(id, false);
                }
            }
        }

        private async void ShowFriendShipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                if (id != tw.Username)
                {
                    await this.ShowFriendship(id);
                }
            }
        }

        private async void ShowUserStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                await this.ShowUserStatus(id, false);
            }
        }

        private void SearchPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                AddNewTabForUserTimeline(id);
            }
        }

        private void SearchAtPostsDetailNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                string id = (string)NameLabel.Tag;
                AddNewTabForSearch("@" + id);
            }
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
            int retweetCount = 0;

            using (var dialog = new WaitingDialog(Properties.Resources.RtCountMenuItem_ClickText1))
            {
                var cancellationToken = dialog.EnableCancellation();

                var task = Task.Run(() => this.tw.GetStatus_Retweeted_Count(statusId, ref retweetCount));
                var err = await dialog.WaitForAsync(this, task);

                if (cancellationToken.IsCancellationRequested)
                    return;

                if (!string.IsNullOrEmpty(err))
                {
                    MessageBox.Show(Properties.Resources.RtCountText2 + Environment.NewLine + err);
                    return;
                }
            }

            MessageBox.Show(retweetCount + Properties.Resources.RtCountText1);
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

            this.TimerTimeline.Elapsed += this.TimerTimeline_Elapsed;
            this._hookGlobalHotkey.HotkeyPressed += _hookGlobalHotkey_HotkeyPressed;
            this.gh.NotifyClicked += GrowlHelper_Callback;

            // メイリオフォント指定時にタブの最小幅が広くなる問題の対策
            this.ListTab.HandleCreated += (s, e) => NativeMethods.SetMinTabWidth((TabControl)s, 40);

            this.ImageSelector.Visible = false;
            this.ImageSelector.Enabled = false;
            this.ImageSelector.FilePickDialog = OpenFileDialog1;

            this.ReplaceAppName();
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

        private void UserPicture_MouseEnter(object sender, EventArgs e)
        {
            this.UserPicture.Cursor = Cursors.Hand;
        }

        private void UserPicture_MouseLeave(object sender, EventArgs e)
        {
            this.UserPicture.Cursor = Cursors.Default;
        }

        private async void UserPicture_DoubleClick(object sender, EventArgs e)
        {
            if (NameLabel.Tag != null)
            {
                await this.OpenUriInBrowserAsync(MyCommon.TwitterUrl + NameLabel.Tag.ToString());
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
                _modifySettingCommon = true;
                SaveConfigsAll(true);

                if (ImageSelector.ServiceName.Equals("Twitter"))
                    this.StatusText_TextChanged(null, null);
            }
        }

        private void ImageSelector_VisibleChanged(object sender, EventArgs e)
        {
            this.StatusText_TextChanged(null, null);
        }
#endregion

        private void ListManageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ListManage form = new ListManage(tw))
            {
                form.ShowDialog(this);
            }
        }

        public bool ModifySettingCommon
        {
            set { _modifySettingCommon = value; }
        }

        public bool ModifySettingLocal
        {
            set { _modifySettingLocal = value; }
        }

        public bool ModifySettingAtId
        {
            set { _modifySettingAtId = value; }
        }

        private async void SourceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var sourceUri = (Uri)this.SourceLinkLabel.Tag;
            if (sourceUri != null && e.Button == MouseButtons.Left)
            {
                await this.OpenUriInBrowserAsync(sourceUri.AbsoluteUri);
            }
        }

        private void SourceLinkLabel_MouseEnter(object sender, EventArgs e)
        {
            var sourceUri = (Uri)this.SourceLinkLabel.Tag;
            if (sourceUri != null)
            {
                StatusLabelUrl.Text = MyCommon.ConvertToReadableUrl(sourceUri.AbsoluteUri);
            }
        }

        private void SourceLinkLabel_MouseLeave(object sender, EventArgs e)
        {
            SetStatusLabelUrl();
        }

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
                    post = await Task.Run(() =>
                    {
                        PostClass newPost = null;

                        var err = this.tw.GetStatusApi(false, statusId, ref newPost);
                        if (!string.IsNullOrEmpty(err))
                            throw new WebApiException(err);

                        return newPost;
                    });
                }
                catch (WebApiException ex)
                {
                    this.StatusLabel.Text = ex.Message;
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
            var tabRelated = this._statuses.GetTabByType(MyCommon.TabUsageType.Related);
            string tabName;

            if (tabRelated == null)
            {
                tabName = this._statuses.MakeTabName("Related Tweets");

                this.AddNewTab(tabName, false, MyCommon.TabUsageType.Related);
                this._statuses.AddTab(tabName, MyCommon.TabUsageType.Related, null);

                tabRelated = this._statuses.GetTabByType(MyCommon.TabUsageType.Related);
                tabRelated.UnreadManage = false;
                tabRelated.Notify = false;
            }
            else
            {
                tabName = tabRelated.TabName;
            }

            tabRelated.RelationTargetPost = post;
            this.ClearTab(tabName, false);

            for (int i = 0; i < this.ListTab.TabPages.Count; i++)
            {
                var tabPage = this.ListTab.TabPages[i];
                if (tabName == tabPage.Text)
                {
                    this.ListTab.SelectedIndex = i;
                    break;
                }
            }

            await this.GetRelatedTweetsAsync(tabRelated);
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
            this._modifySettingCommon = true;
        }

#region "Userstream"
        private bool _isActiveUserstream = false;

        private void tw_PostDeleted(object sender, PostDeletedEventArgs e)
        {
            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke((Action) (async () =>
                           {
                               _statuses.RemovePostReserve(e.StatusId);
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

        private void tw_NewPostFromStream(object sender, EventArgs e)
        {
            if (this._cfgCommon.ReadOldPosts)
            {
                _statuses.SetReadHomeTab(); //新着時未読クリア
            }

            int rsltAddCount = _statuses.DistributePosts();
            lock (_syncObject)
            {
                DateTime tm = DateTime.Now;
                if (_tlTimestamps.ContainsKey(tm))
                {
                    _tlTimestamps[tm] += rsltAddCount;
                }
                else
                {
                    _tlTimestamps.Add(tm, rsltAddCount);
                }
                DateTime oneHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                List<DateTime> keys = new List<DateTime>();
                _tlCount = 0;
                foreach (DateTime key in _tlTimestamps.Keys)
                {
                    if (key.CompareTo(oneHour) < 0)
                        keys.Add(key);
                    else
                        _tlCount += _tlTimestamps[key];
                }
                foreach (DateTime key in keys)
                {
                    _tlTimestamps.Remove(key);
                }
                keys.Clear();

                //Static DateTime before = Now;
                //if (before.Subtract(Now).Seconds > -5) return;
                //before = Now;
            }

            if (this._cfgCommon.UserstreamPeriod > 0) return;

            try
            {
                if (InvokeRequired && !IsDisposed)
                {
                    Invoke((Action)(async () =>
                    {
                        await this.RefreshTasktrayIcon(true);
                        this.RefreshTimeline(true);
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

        private void tw_UserStreamStarted(object sender, EventArgs e)
        {
            this._isActiveUserstream = true;
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

            MenuItemUserStream.Text = "&UserStream ▶";
            MenuItemUserStream.Enabled = true;
            StopToolStripMenuItem.Text = "&Stop";
            StopToolStripMenuItem.Enabled = true;

            StatusLabel.Text = "UserStream Started.";
        }

        private void tw_UserStreamStopped(object sender, EventArgs e)
        {
            this._isActiveUserstream = false;
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

            MenuItemUserStream.Text = "&UserStream ■";
            MenuItemUserStream.Enabled = true;
            StopToolStripMenuItem.Text = "&Start";
            StopToolStripMenuItem.Enabled = true;

            StatusLabel.Text = "UserStream Stopped.";
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
                if (ev.Event == "unfavorite" && ev.Username.ToLower().Equals(tw.Username.ToLower()))
                {
                    RemovePostFromFavTab(new long[] {ev.Id});
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
                title.Append(ev.Event.ToUpper());
                title.Append("] by ");
                if (!string.IsNullOrEmpty(ev.Username))
                {
                    title.Append(ev.Username.ToString());
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
            if (this._isActiveUserstream)
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
                    this._modifySettingCommon = true;
                    TrackToolStripMenuItem.Checked = !string.IsNullOrEmpty(inputTrack);
                    tw.ReconnectUserStream();
                }
            }
            else
            {
                tw.TrackWord = "";
                tw.ReconnectUserStream();
            }
            this._modifySettingCommon = true;
        }

        private void AllrepliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tw.AllAtReply = AllrepliesToolStripMenuItem.Checked;
            this._modifySettingCommon = true;
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

        private async Task doTranslation(string str)
        {
            if (string.IsNullOrEmpty(str))
                return;

            var bing = new Bing();
            try
            {
                var translatedText = await bing.TranslateAsync(str,
                    langFrom: null,
                    langTo: this._cfgCommon.TranslateLanguage);

                this.PostBrowser.DocumentText = this.createDetailHtml(translatedText);
            }
            catch (HttpRequestException e)
            {
                this.StatusLabel.Text = "Err:" + e.Message;
            }
        }

        private async void TranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.ExistCurrentPost)
                return;

            await this.doTranslation(this._curPost.TextFromApi);
        }

        private async void SelectionTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var text = this.PostBrowser.GetSelectedText();
            await this.doTranslation(text);
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
            string id = "";
            if (_curPost != null)
            {
                id = _curPost.ScreenName;
            }
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
                        if (_curPost.RetweetedId != null)
                        {
                            xUrl = xUrl.Replace("{STATUS}", _curPost.RetweetedId.ToString());
                        }
                        else
                        {
                            xUrl = xUrl.Replace("{STATUS}", _curPost.StatusId.ToString());
                        }
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

        private void SourceCopyMenuItem_Click(object sender, EventArgs e)
        {
            string selText = SourceLinkLabel.Text;
            try
            {
                Clipboard.SetDataObject(selText, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SourceUrlCopyMenuItem_Click(object sender, EventArgs e)
        {
            var sourceUri = (Uri)this.SourceLinkLabel.Tag;
            try
            {
                Clipboard.SetDataObject(sourceUri.AbsoluteUri, false, 5, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ContextMenuSource_Opening(object sender, CancelEventArgs e)
        {
            if (_curPost == null || !ExistCurrentPost || _curPost.IsDm)
            {
                SourceCopyMenuItem.Enabled = false;
                SourceUrlCopyMenuItem.Enabled = false;
            }
            else
            {
                SourceCopyMenuItem.Enabled = true;
                SourceUrlCopyMenuItem.Enabled = true;
            }
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

            // PreviewDistance が起動のたびに広がっていく問題の回避策
            // FixedPanel が Panel2 に設定された状態で Panel2 を開くと、初回だけ SplitterDistance が再計算されておかしくなるため、
            // None で開いた後に設定するようにする
            if (this.SplitContainer3.FixedPanel == FixedPanel.None)
                this.SplitContainer3.FixedPanel = FixedPanel.Panel2;
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
            var url = thumbnail.FullSizeImageUrl ?? thumbnail.ImageUrl;

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

            if (_curList != null) _curList.Refresh();

            _modifySettingCommon = true;
        }

        private void LockListSortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var state = this.LockListSortOrderToolStripMenuItem.Checked;
            if (this._cfgCommon.SortOrderLock == state) return;

            this._cfgCommon.SortOrderLock = state;

            _modifySettingCommon = true;
        }
    }
}
