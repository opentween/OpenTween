// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
// All rights reserved.
// 
// This file is part of OpenTween.
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details. 
// 
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Linq;

namespace OpenTween.Thumbnail
{
    public class ThumbnailGenerator
    {
        private object lckPrev = new object();
        private PreviewData _prev;
        private class PreviewData : IDisposable
        {
            public long statusId;
            public List<KeyValuePair<string, string>> urls;
            public List<KeyValuePair<string, Image>> pics = new List<KeyValuePair<string, Image>>();
            public List<KeyValuePair<string, string>> tooltipText = new List<KeyValuePair<string, string>>();
            public List<KeyValuePair<string, ImageCreatorDelegate>> imageCreators = new List<KeyValuePair<string, ImageCreatorDelegate>>();
            public PreviewData(long id, List<KeyValuePair<string, string>> urlList, List<KeyValuePair<string, ImageCreatorDelegate>> imageCreatorList)
            {
                statusId = id;
                urls = urlList;
                imageCreators = imageCreatorList;
            }

            public bool IsError;
            public string AdditionalErrorMessage;

            private bool disposedValue = false;        // 重複する呼び出しを検出するには

            // IDisposable
            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: 明示的に呼び出されたときにマネージ リソースを解放します
                        foreach (var pic in pics)
                        {
                            if (pic.Value != null) pic.Value.Dispose();
                        }
                    }

                    // TODO: 共有のアンマネージ リソースを解放します
                }
                this.disposedValue = true;
            }

#region " IDisposable Support "
            // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
            public void Dispose()
            {
                // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
                Dispose(true);
                GC.SuppressFinalize(this);
            }
#endregion

        }
        private TweenMain Owner;
        private delegate bool UrlCreatorDelegate(GetUrlArgs args);
        private delegate bool ImageCreatorDelegate(CreateImageArgs args);

        private class GetUrlArgs
        {
            public string url;
            public string extended;
            public List<KeyValuePair<string, string>> imglist;
            public GlobalLocation geoInfo;
        }

        private class CreateImageArgs
        {
            public KeyValuePair<string, string> url;
            public List<KeyValuePair<string, Image>> pics;
            public List<KeyValuePair<string, string>> tooltipText;
            public string errmsg;
        }

        private class ThumbnailService
        {
            public string Name;
            public UrlCreatorDelegate urlCreator;
            public ImageCreatorDelegate imageCreator;

            public ThumbnailService(string name, UrlCreatorDelegate urlcreator, ImageCreatorDelegate imagecreator)
            {
                this.Name = name;
                this.urlCreator = urlcreator;
                this.imageCreator = imagecreator;
            }
        }

        private ThumbnailService[] ThumbnailServices;

        public ThumbnailGenerator(TweenMain Owner)
        {
            this.Owner = Owner;

            Owner.PreviewScrollBar.Scroll += PreviewScrollBar_Scroll;
            Owner.PreviewPicture.MouseLeave += PreviewPicture_MouseLeave;
            Owner.PreviewPicture.DoubleClick += PreviewPicture_DoubleClick;

            ThumbnailServices = new[] {
                new ThumbnailService("ImgUr", ImgUr_GetUrl, ImgUr_CreateImage),
                new ThumbnailService("DirectLink", DirectLink_GetUrl, DirectLink_CreateImage),
                new ThumbnailService("TwitPic", TwitPic_GetUrl, TwitPic_CreateImage),
                new ThumbnailService("yfrog", yfrog_GetUrl, yfrog_CreateImage),
                new ThumbnailService("Plixi(TweetPhoto)", Plixi_GetUrl, Plixi_CreateImage),
                new ThumbnailService("MobyPicture", MobyPicture_GetUrl, MobyPicture_CreateImage),
                new ThumbnailService("携帯百景", MovaPic_GetUrl, MovaPic_CreateImage),
                new ThumbnailService("はてなフォトライフ", Hatena_GetUrl, Hatena_CreateImage),
                new ThumbnailService("PhotoShare/bctiny", PhotoShare_GetUrl, PhotoShare_CreateImage),
                new ThumbnailService("img.ly", imgly_GetUrl, imgly_CreateImage),
                new ThumbnailService("Twitgoo", Twitgoo_GetUrl, Twitgoo_CreateImage),
                new ThumbnailService("youtube", youtube_GetUrl, youtube_CreateImage),
                new ThumbnailService("ニコニコ動画", nicovideo_GetUrl, nicovideo_CreateImage),
                new ThumbnailService("ニコニコ静画", nicoseiga_GetUrl, nicoseiga_CreateImage),
                new ThumbnailService("Pixiv", Pixiv_GetUrl, Pixiv_CreateImage),
                new ThumbnailService("flickr", flickr_GetUrl, flickr_CreateImage),
                new ThumbnailService("フォト蔵", Photozou_GetUrl, Photozou_CreateImage),
                new ThumbnailService("TwitVideo", TwitVideo_GetUrl, TwitVideo_CreateImage),
                new ThumbnailService("Piapro", Piapro_GetUrl, Piapro_CreateImage),
                new ThumbnailService("Tumblr", Tumblr_GetUrl, Tumblr_CreateImage),
                new ThumbnailService("ついっぷるフォト", TwipplePhoto_GetUrl, TwipplePhoto_CreateImage),
                new ThumbnailService("mypix/shamoji", mypix_GetUrl, mypix_CreateImage),
                new ThumbnailService("ow.ly", Owly_GetUrl, Owly_CreateImage),
                new ThumbnailService("vimeo", Vimeo_GetUrl, Vimeo_CreateImage),
                new ThumbnailService("cloudfiles", CloudFiles_GetUrl, CloudFiles_CreateImage),
                new ThumbnailService("instagram", instagram_GetUrl, instagram_CreateImage),
                new ThumbnailService("pikubo", pikubo_GetUrl, pikubo_CreateImage),
                new ThumbnailService("FourSquare", Foursquare_GetUrl, Foursquare_CreateImage),
                new ThumbnailService("TINAMI", Tinami_GetUrl, Tinami_CreateImage),
                new ThumbnailService("Twimg", Twimg_GetUrl, Twimg_CreateImage),
                new ThumbnailService("TwitrPix", TwitrPix_GetUrl, TwitrPix_CreateImage),
                new ThumbnailService("Pckles", Pckles_GetUrl, Pckles_CreateImage),
                new ThumbnailService("via.me", ViaMe_GetUrl, ViaMe_CreateImage),
                new ThumbnailService("tuna.be", TunaBe_GetUrl, TunaBe_CreateImage),
            };
        }

        private PostClass _curPost
        {
            get
            {
                return Owner.CurPost;
            }
        }

        private bool IsDirectLink(string url)
        {
            return Regex.Match(url, @"^http://.*(\.jpg|\.jpeg|\.gif|\.png|\.bmp)$", RegexOptions.IgnoreCase).Success;
        }

        public void thumbnail(long id, List<string> links, PostClass.StatusGeo geo, Dictionary<string, string> media)
        {
            if (!Owner.IsPreviewEnable)
            {
                Owner.SplitContainer3.Panel2Collapsed = true;
                return;
            }
            if (Owner.PreviewPicture.Image != null)
            {
                Owner.PreviewPicture.Image.Dispose();
                Owner.PreviewPicture.Image = null;
                Owner.SplitContainer3.Panel2Collapsed = true;
            }
            //lock (lckPrev)
            //{
            //    if (_prev != null)
            //    {
            //        _prev.Dispose();
            //        _prev = null;
            //    }
            //}

            if (links.Count == 0 && geo == null && (media == null || media.Count == 0))
            {
                Owner.PreviewScrollBar.Maximum = 0;
                Owner.PreviewScrollBar.Enabled = false;
                Owner.SplitContainer3.Panel2Collapsed = true;
                return;
            }

            if (media != null && media.Count > 0)
            {
                foreach (var link in links.ToArray())
                {
                    if (media.ContainsKey(link)) links.Remove(link);
                }
            }

            var imglist = new List<KeyValuePair<string, string>>();
            var dlg = new List<KeyValuePair<string, ImageCreatorDelegate>>();

            foreach (var url in links)
            {
                foreach (var svc in ThumbnailServices)
                {
                    var args = new GetUrlArgs();
                    args.url = url;
                    args.imglist = imglist;
                    if (svc.urlCreator(args))
                    {
                        // URLに対応したサムネイル作成処理デリゲートをリストに登録
                        dlg.Add(new KeyValuePair<string, ImageCreatorDelegate>(url, svc.imageCreator));
                        break;
                    }
                }
            }
            if (media != null)
            {
                foreach (var m in media)
                {
                    foreach (var svc in ThumbnailServices)
                    {
                        var args = new GetUrlArgs();
                        args.url = m.Key;
                        args.extended = m.Value;
                        args.imglist = imglist;
                        if (svc.urlCreator(args))
                        {
                            // URLに対応したサムネイル作成処理デリゲートをリストに登録
                            dlg.Add(new KeyValuePair<string, ImageCreatorDelegate>(m.Key, svc.imageCreator));
                            break;
                        }
                    }
                }
            }
            if (geo != null)
            {
                var args = new GetUrlArgs();
                args.url = "";
                args.imglist = imglist;
                args.geoInfo = new GlobalLocation{ Latitude = geo.Lat, Longitude = geo.Lng };
                if (TwitterGeo_GetUrl(args))
                {
                    // URLに対応したサムネイル作成処理デリゲートをリストに登録
                    dlg.Add(new KeyValuePair<string, ImageCreatorDelegate>(args.url, new ImageCreatorDelegate(TwitterGeo_CreateImage)));
                }
            }
            if (imglist.Count == 0)
            {
                Owner.PreviewScrollBar.Maximum = 0;
                Owner.PreviewScrollBar.Enabled = false;
                Owner.SplitContainer3.Panel2Collapsed = true;
                return;
            }

            ThumbnailProgressChanged(0);
            BackgroundWorker bgw;
            bgw = new BackgroundWorker();
            bgw.DoWork += bgw_DoWork;
            bgw.RunWorkerCompleted += bgw_Completed;
            bgw.RunWorkerAsync(new PreviewData(id, imglist, dlg));

        }

        private void ThumbnailProgressChanged(int ProgressPercentage, string AddMsg = "")
        {
            if (ProgressPercentage == 0)    //開始
            {
                //Owner.SetStatusLabel("Thumbnail generating...");
            }
            else if (ProgressPercentage == 100) //正常終了
            {
                //Owner.SetStatusLabel("Thumbnail generated.");
            }
            else // エラー
            {
                if (string.IsNullOrEmpty(AddMsg))
                {
                    Owner.SetStatusLabel("can't get Thumbnail.");
                }
                else
                {
                    Owner.SetStatusLabel("can't get Thumbnail.(" + AddMsg + ")");
                }
            }
        }

        private void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var arg = (PreviewData)e.Argument;
            var worker = (BackgroundWorker)sender;
            arg.AdditionalErrorMessage = "";

            foreach (var url in arg.urls)
            {
                var args = new CreateImageArgs();
                args.url = url;
                args.pics = arg.pics;
                args.tooltipText = arg.tooltipText;
                args.errmsg = "";
                if (!arg.imageCreators[arg.urls.IndexOf(url)].Value(args))
                {
                    arg.AdditionalErrorMessage = args.errmsg;
                    arg.IsError = true;
                }
            }

            if (arg.pics.Count == 0)
            {
                arg.IsError = true;
            }
            else
            {
                arg.IsError = false;
            }
            e.Result = arg;
        }

        private void bgw_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            var prv = e.Result as PreviewData;
            if (prv == null || prv.IsError)
            {
                Owner.PreviewScrollBar.Maximum = 0;
                Owner.PreviewScrollBar.Enabled = false;
                Owner.SplitContainer3.Panel2Collapsed = true;
                if (prv != null && !string.IsNullOrEmpty(prv.AdditionalErrorMessage))
                {
                    ThumbnailProgressChanged(-1, prv.AdditionalErrorMessage);
                }
                else
                {
                    ThumbnailProgressChanged(-1);
                }
                return;
            }
            lock(lckPrev)
            {
                if (prv != null && _curPost != null && prv.statusId == _curPost.StatusId)
                {
                    if (_prev != null)
                    {
                        _prev.Dispose();
                    }
                    _prev = prv;
                    Owner.SplitContainer3.Panel2Collapsed = false;
                    Owner.PreviewScrollBar.Maximum = _prev.pics.Count - 1;
                    if (Owner.PreviewScrollBar.Maximum > 0)
                    {
                        Owner.PreviewScrollBar.Enabled = true;
                    }
                    else
                    {
                        Owner.PreviewScrollBar.Enabled = false;
                    }
                    Owner.PreviewScrollBar.Value = 0;
                    Owner.PreviewPicture.Image = _prev.pics[0].Value;
                    if (!string.IsNullOrEmpty(_prev.tooltipText[0].Value))
                    {
                        Owner.ToolTip1.SetToolTip(Owner.PreviewPicture, _prev.tooltipText[0].Value);
                    }
                    else
                    {
                        Owner.ToolTip1.SetToolTip(Owner.PreviewPicture, "");
                    }
                }
                else if (_curPost == null || (_prev != null && _curPost.StatusId != _prev.statusId))
                {
                    Owner.PreviewScrollBar.Maximum = 0;
                    Owner.PreviewScrollBar.Enabled = false;
                    Owner.SplitContainer3.Panel2Collapsed = true;
                }
            }
            ThumbnailProgressChanged(100);
        }

        public void ScrollThumbnail(bool forward)
        {
            if (forward)
            {
                Owner.PreviewScrollBar.Value = Math.Min(Owner.PreviewScrollBar.Value + 1, Owner.PreviewScrollBar.Maximum);
                PreviewScrollBar_Scroll(Owner.PreviewScrollBar, new ScrollEventArgs(ScrollEventType.SmallIncrement, Owner.PreviewScrollBar.Value));
            }
            else
            {
                Owner.PreviewScrollBar.Value = Math.Max(Owner.PreviewScrollBar.Value - 1, Owner.PreviewScrollBar.Minimum);
                PreviewScrollBar_Scroll(Owner.PreviewScrollBar, new ScrollEventArgs(ScrollEventType.SmallDecrement, Owner.PreviewScrollBar.Value));
            }
        }

        private void PreviewScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            lock(lckPrev)
            {
                if (_prev != null && _curPost != null && _prev.statusId == _curPost.StatusId)
                {
                    if (_prev.pics.Count > e.NewValue)
                    {
                        Owner.PreviewPicture.Image = _prev.pics[e.NewValue].Value;
                        if (!string.IsNullOrEmpty(_prev.tooltipText[e.NewValue].Value))
                        {
                            Owner.ToolTip1.Hide(Owner.PreviewPicture);
                            Owner.ToolTip1.SetToolTip(Owner.PreviewPicture, _prev.tooltipText[e.NewValue].Value);
                        }
                        else
                        {
                            Owner.ToolTip1.SetToolTip(Owner.PreviewPicture, "");
                            Owner.ToolTip1.Hide(Owner.PreviewPicture);
                        }
                    }
                }
            }
        }

        private void PreviewPicture_MouseLeave(object sender, EventArgs e)
        {
            Owner.ToolTip1.Hide(Owner.PreviewPicture);
        }
        private void PreviewPicture_DoubleClick(object sender, EventArgs e)
        {
            OpenPicture();
        }
        public void OpenPicture()
        {
            if (_prev != null)
            {
                if (Owner.PreviewScrollBar.Value < _prev.pics.Count)
                {
                    Owner.OpenUriAsync(_prev.pics[Owner.PreviewScrollBar.Value].Key);
                    //if (AppendSettingDialog.Instance.OpenPicBuiltinBrowser)
                    //{
                    //    using (var ab = new AuthBrowser())
                    //    {
                    //        ab.Auth = false;
                    //        ab.UrlString = _prev.pics[Owner.PreviewScrollBar.Value].Key;
                    //        ab.ShowDialog(Owner)
                    //    }
                    //Else
                    //    Owner.OpenUriAsync(_prev.pics[Owner.PreviewScrollBar.Value].Key)
                    //}
                }
            }
        }

#region "テンプレ"
    #if UNDEFINED__
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool ServiceName_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(args.url, @"^http://imgur\.com/(\w+)\.jpg$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://i.imgur.com/${1}l.jpg")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool ServiceName_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltiptext.Add(new KeyValuePair<string, string>(args.url.Key, ""))
            return true;
        }
    #endif
#endregion

#region "ImgUr"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool ImgUr_GetUrl(GetUrlArgs args)
        {
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                          @"^http://(?:i\.)?imgur\.com/(\w+)(?:\..{3})?$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://img.imgur.com/${1}l.jpg")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>

        private bool ImgUr_CreateImage(CreateImageArgs args)
        {
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }
#endregion

#region "画像直リンク"
        private bool DirectLink_GetUrl(GetUrlArgs args)
        {
            //画像拡張子で終わるURL（直リンク）
            if (IsDirectLink(string.IsNullOrEmpty(args.extended) ? args.url : args.extended))
            {
                args.imglist.Add(new KeyValuePair<string, string>(args.url, string.IsNullOrEmpty(args.extended) ? args.url : args.extended));
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>

        private bool DirectLink_CreateImage(CreateImageArgs args)
        {
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null) return false;
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }
#endregion

#region "TwitPic"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool TwitPic_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://(www\.)?twitpic\.com/(?<photoId>\w+)(/full/?)?$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://twitpic.com/show/thumb/${photoId}")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool TwitPic_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "yfrog"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool yfrog_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://yfrog\.com/(\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, (string.IsNullOrEmpty(args.extended) ? args.url : args.extended) + ":small"));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool yfrog_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "Plixi(TweetPhoto)"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Plixi_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^(http://tweetphoto\.com/[0-9]+|http://pic\.gd/[a-z0-9]+|http://(lockerz|plixi)\.com/[ps]/[0-9]+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                const string comp = "http://api.plixi.com/api/tpapi.svc/imagefromurl?size=thumbnail&url=";
                args.imglist.Add(new KeyValuePair<String, String>(args.url, comp + (String.IsNullOrEmpty(args.extended) ? args.url : args.extended)));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair(Of string, Image> pics)         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Plixi_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var referer = "";
            if (args.url.Key.Contains("t.co"))
            {
                if (args.url.Value.Contains("tweetphoto.com"))
                {
                    referer = "http://tweetphoto.com";
                }
                else if (args.url.Value.Contains("http://lockerz.com"))
                {
                    referer = "http://lockerz.com";
                }
                else
                {
                    referer = "http://plixi.com";
                }
            }
            else
            {
                referer = args.url.Key;
            }
            var img = (new HttpVarious()).GetImage(args.url.Value, referer, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "MobyPicture"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool MobyPicture_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://moby\.to/(\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://mobypicture.com/?${1}:small")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool MobyPicture_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "携帯百景"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool MovaPic_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://movapic\.com/pic/(\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://image.movapic.com/pic/s_${1}.jpeg")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool MovaPic_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "はてなフォトライフ"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Hatena_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://f\.hatena\.ne\.jp/(([a-z])[a-z0-9_-]{1,30}[a-z0-9])/((\d{8})\d+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://img.f.hatena.ne.jp/images/fotolife/${2}/${1}/${4}/${3}_120.jpg")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Hatena_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "PhotoShare"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool PhotoShare_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://(?:www\.)?bcphotoshare\.com/photos/\d+/(\d+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://images.bcphotoshare.com/storages/${1}/thumb180.jpg")));
                return true;
            }
            // 短縮URL
            mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                             @"^http://bctiny\.com/p(\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                try
                {
                    args.imglist.Add(new KeyValuePair<string, string>(args.url, "http://images.bcphotoshare.com/storages/" + RadixConvert.ToInt32(mc.Result("${1}"), 36).ToString() + "/thumb180.jpg"));
                    return true;
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
            return false;
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool PhotoShare_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "img.ly"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool imgly_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://img\.ly/(\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://img.ly/show/thumb/${1}")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool imgly_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "Twitgoo"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Twitgoo_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(String.IsNullOrEmpty(args.extended) ? args.url : args.extended, @"^http://twitgoo\.com/(\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://twitgoo.com/${1}/mini")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Twitgoo_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "youtube"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool youtube_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://www\.youtube\.com/watch\?v=([\w\-]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                //args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://i.ytimg.com/vi/${1}/default.jpg")))
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("${0}")));
                return true;
            }
            mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                             @"^http://youtu\.be/([\w\-]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                //args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://i.ytimg.com/vi/${1}/default.jpg")))
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("${0}")));
                return true;
            }
            return false;
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool youtube_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            // 参考
            // http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_video_entries.html
            // デベロッパー ガイド: Data API プロトコル - 単独の動画情報の取得 - YouTube の API とツール - Google Code
            // http://code.google.com/intl/ja/apis/youtube/2.0/developers_guide_protocol_understanding_video_feeds.html#Understanding_Feeds_and_Entries
            // デベロッパー ガイド: Data API プロトコル - 動画のフィードとエントリについて - YouTube の API とツール - Google Code
            var imgurl = "";
            var mcImg = Regex.Match(args.url.Value, @"^http://(?:(www\.youtube\.com)|(youtu\.be))/(watch\?v=)?(?<videoid>([\w\-]+))", RegexOptions.IgnoreCase);
            if (mcImg.Success)
            {
                imgurl = mcImg.Result("http://i.ytimg.com/vi/${videoid}/default.jpg");
            }
            else
            {
                return false;
            }
            var videourl = (new HttpVarious()).GetRedirectTo(args.url.Value);
            var mc = Regex.Match(videourl, @"^http://(?:(www\.youtube\.com)|(youtu\.be))/(watch\?v=)?(?<videoid>([\w\-]+))", RegexOptions.IgnoreCase);
            if (videourl.StartsWith("http://www.youtube.com/index?ytsession="))
            {
                videourl = args.url.Value;
                mc = Regex.Match(videourl, @"^http://(?:(www\.youtube\.com)|(youtu\.be))/(watch\?v=)?(?<videoid>([\w\-]+))", RegexOptions.IgnoreCase);
            }
            if (mc.Success)
            {
                var apiurl = "http://gdata.youtube.com/feeds/api/videos/" + mc.Groups["videoid"].Value;
                var src = "";
                if ((new HttpVarious()).GetData(apiurl, null, out src, 5000))
                {
                    var sb = new StringBuilder();
                    var xdoc = new XmlDocument();
                    try
                    {
                        xdoc.LoadXml(src);
                        var nsmgr = new XmlNamespaceManager(xdoc.NameTable);
                        nsmgr.AddNamespace("root", "http://www.w3.org/2005/Atom");
                        nsmgr.AddNamespace("app", "http://purl.org/atom/app#");
                        nsmgr.AddNamespace("media", "http://search.yahoo.com/mrss/");

                        var xentryNode = xdoc.DocumentElement.SelectSingleNode("/root:entry/media:group", nsmgr);
                        var xentry = (XmlElement)xentryNode;
                        var tmp = "";
                        try
                        {
                            tmp = xentry["media:title"].InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.YouTubeInfoText1);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch(Exception)
                        {
                        }

                        try
                        {
                            var sec = 0;
                            if (int.TryParse(xentry["yt:duration"].Attributes["seconds"].Value, out sec))
                            {
                                sb.Append(Properties.Resources.YouTubeInfoText2);
                                sb.AppendFormat("{0:d}:{1:d2}", sec / 60, sec % 60);
                                sb.AppendLine();
                            }
                        }
                        catch(Exception)
                        {
                        }

                        try
                        {
                            var tmpdate = new DateTime();
                            xentry = (XmlElement)xdoc.DocumentElement.SelectSingleNode("/root:entry", nsmgr);
                            if (DateTime.TryParse(xentry["published"].InnerText, out tmpdate))
                            {
                                sb.Append(Properties.Resources.YouTubeInfoText3);
                                sb.Append(tmpdate);
                                sb.AppendLine();
                            }
                        }
                        catch(Exception)
                        {
                        }

                        try
                        {
                            var count = 0;
                            xentry = (XmlElement)xdoc.DocumentElement.SelectSingleNode("/root:entry", nsmgr);
                            tmp = xentry["yt:statistics"].Attributes["viewCount"].Value;
                            if (int.TryParse(tmp, out count))
                            {
                                sb.Append(Properties.Resources.YouTubeInfoText4);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch(Exception)
                        {
                        }

                        try
                        {
                            xentry = (XmlElement)xdoc.DocumentElement.SelectSingleNode("/root:entry/app:control", nsmgr);
                            if (xentry != null)
                            {
                                sb.Append(xentry["yt:state"].Attributes["name"].Value);
                                sb.Append(":");
                                sb.Append(xentry["yt:state"].InnerText);
                                sb.AppendLine();
                            }
                        }
                        catch(Exception)
                        {
                        }

                        //mc = Regex.Match(videourl, @"^http://www\.youtube\.com/watch\?v=([\w\-]+)", RegexOptions.IgnoreCase)
                        //if (mc.Success)
                        //{
                        //    imgurl = mc.Result("http://i.ytimg.com/vi/${1}/default.jpg");
                        //}
                        //mc = Regex.Match(videourl, @"^http://youtu\.be/([\w\-]+)", RegexOptions.IgnoreCase)
                        //if (mc.Success)
                        //{
                        //    imgurl = mc.Result("http://i.ytimg.com/vi/${1}/default.jpg");
                        //}

                    }
                    catch(Exception)
                    {

                    }

                    if (!string.IsNullOrEmpty(imgurl))
                    {
                        var http = new HttpVarious();
                        var _img = http.GetImage(imgurl, videourl, 10000, out args.errmsg);
                        if (_img == null) return false;
                        args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, _img));
                        args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, sb.ToString().Trim()));
                        return true;
                    }
                }

            }
            return false;
        }

#endregion

#region "ニコニコ動画"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool nicovideo_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://(?:(www|ext)\.nicovideo\.jp/watch|nico\.ms)/(?:sm|nm)?([0-9]+)(\?.+)?$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool nicovideo_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var http = new HttpVarious();
            var mc = Regex.Match(args.url.Value, @"^http://(?:(www|ext)\.nicovideo\.jp/watch|nico\.ms)/(?<id>(?:sm|nm)?([0-9]+))(\?.+)?$", RegexOptions.IgnoreCase);
            var apiurl = "http://www.nicovideo.jp/api/getthumbinfo/" + mc.Groups["id"].Value;
            var src = "";
            var imgurl = "";
            if ((new HttpVarious()).GetData(apiurl, null, out src, 0, out args.errmsg, MyCommon.GetUserAgentString()))
            {
                var sb = new StringBuilder();
                var xdoc = new XmlDocument();
                try
                {
                    xdoc.LoadXml(src);
                    var status = xdoc.SelectSingleNode("/nicovideo_thumb_response").Attributes["status"].Value;
                    if (status == "ok")
                    {
                        imgurl = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/thumbnail_url").InnerText;

                        //ツールチップに動画情報をセットする
                        string tmp;

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/title").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText1);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch(Exception)
                        {

                        }

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/length").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText2);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch(Exception)
                        {

                        }

                        try
                        {
                            var tm = new DateTime();
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/first_retrieve").InnerText;
                            if (DateTime.TryParse(tmp, out tm))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText3);
                                sb.Append(tm.ToString());
                                sb.AppendLine();
                            }
                        }
                        catch(Exception)
                        {

                        }

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/view_counter").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText4);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch(Exception)
                        {

                        }

                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/comment_num").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText5);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch(Exception)
                        {

                        }
                        try
                        {
                            tmp = xdoc.SelectSingleNode("/nicovideo_thumb_response/thumb/mylist_counter").InnerText;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                sb.Append(Properties.Resources.NiconicoInfoText6);
                                sb.Append(tmp);
                                sb.AppendLine();
                            }
                        }
                        catch(Exception)
                        {

                        }
                    }
                    else if (status == "fail")
                    {
                        var errcode = xdoc.SelectSingleNode("/nicovideo_thumb_response/error/code").InnerText;
                        args.errmsg = errcode;
                        imgurl = "";
                    }
                    else
                    {
                        args.errmsg = "UnknownResponse";
                        imgurl = "";
                    }

                }
                catch(Exception)
                {
                    imgurl = "";
                    args.errmsg = "Invalid XML";
                }

                if (!string.IsNullOrEmpty(imgurl))
                {
                    var _img = http.GetImage(imgurl, args.url.Key, 0, out args.errmsg);
                    if (_img == null) return false;
                    args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, _img));
                    args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, sb.ToString().Trim()));
                    return true;
                }
            }
            return false;
        }

#endregion

#region "ニコニコ静画"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool nicoseiga_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://(?:seiga\.nicovideo\.jp/seiga/|nico\.ms/)im\d+");
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool nicoseiga_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var http = new HttpVarious();
            var mc = Regex.Match(args.url.Value, @"^http://(?:seiga\.nicovideo\.jp/seiga/|nico\.ms/)im(?<id>\d+)");
            if (mc.Success)
            {
                var _img = http.GetImage("http://lohas.nicoseiga.jp/thumb/" + mc.Groups["id"].Value + "q?", args.url.Key, 0, out args.errmsg);
                if (_img == null) return false;
                args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, _img));
                args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
                return true;
            }
            return false;
        }

#endregion

#region "Pixiv"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Pixiv_GetUrl(GetUrlArgs args)
        {
            //参考: http://tail.s68.xrea.com/blog/2009/02/pixivflash.html Pixivの画像をFlashとかで取得する方法など:しっぽのブログ
            //ユーザー向けの画像ページ http://www.pixiv.net/member_illust.php?mode=medium&illust_id=[ID番号]
            //非ログインユーザー向けの画像ページ http://www.pixiv.net/index.php?mode=medium&illust_id=[ID番号]
            //サムネイルURL http://img[サーバー番号].pixiv.net/img/[ユーザー名]/[サムネイルID]_s.[拡張子]
            //サムネイルURLは画像ページから抽出する
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://www\.pixiv\.net/(member_illust|index)\.php\?(|.+&(amp;)?)illust_id=(?<illustId>[0-9]+)(&(amp;)?.+|$)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url.Replace("amp;", ""), mc.Value));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Pixiv_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var src = "";
            //illustIDをキャプチャ
            var mc = Regex.Match(args.url.Value, @"^http://www\.pixiv\.net/(member_illust|index)\.php\?(?=.*mode=(medium|big))(?=.*illust_id=(?<illustId>[0-9]+))(?=.*tag=(?<tag>[^&]+)?)?.*$", RegexOptions.IgnoreCase);
            if (mc.Groups["tag"].Value == "R-18" || mc.Groups["tag"].Value == "R-18G")
            {
                args.errmsg = "NotSupported";
                return false;
            }
            else
            {
                var http = new HttpVarious();
                if (http.GetData(Regex.Replace(mc.Groups[0].Value, "amp;", ""), null, out src, 0, out args.errmsg, ""))
                {
                    var _mc = Regex.Match(src, mc.Result(@"http://i([0-9]+)\.pixiv\.net/.+/${illustId}_[ms]\.([a-zA-Z]+)"));
                    if (_mc.Success)
                    {
                        var _img = http.GetImage(_mc.Value, args.url.Value, 0, out args.errmsg);
                        if (_img == null) return false;
                        args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, _img));
                        args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
                        return true;
                    }
                    else if (Regex.Match(src, "<span class=//error//>ログインしてください</span>").Success)
                    {
                        args.errmsg = "NotSupported";
                    }
                    else
                    {
                        args.errmsg = "Pattern NotFound";
                    }
                }
            }
            return false;
        }

#endregion

#region "flickr"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool flickr_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 "^http://www.flickr.com/", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, string.IsNullOrEmpty(args.extended) ? args.url : args.extended));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool flickr_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            //参考: http://tanarky.blogspot.com/2010/03/flickr-urlunavailable.html アグレッシブエンジニア: flickr の画像URL仕様についてまとめ(Unavailable画像)
            //画像URL仕様　http://farm{farm}.staticflickr.com/{server}/{id}_{secret}_{size}.{extension}
            //photostreamなど複数の画像がある場合先頭の一つのみ認識と言うことにする
            //(二つ目のキャプチャ 一つ目の画像はユーザーアイコン）

            var src = "";
            var mc = Regex.Match(args.url.Value, "^http://www.flickr.com/", RegexOptions.IgnoreCase);
            var http = new HttpVarious();
            if (http.GetData(args.url.Value, null, out src, 0, out args.errmsg, ""))
            {
                var _mc = Regex.Matches(src, mc.Result(@"http://farm[0-9]+\.staticflickr\.com/[0-9]+/.+?\.([a-zA-Z]+)"));
                //二つ以上キャプチャした場合先頭の一つだけ 一つだけの場合はユーザーアイコンしか取れなかった
                if (_mc.Count > 1)
                {
                    var _img = http.GetImage(_mc[1].Value, args.url.Value, 0, out args.errmsg);
                    if (_img == null) return false;
                    args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, _img));
                    args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
                    return true;
                }
                else
                {
                    args.errmsg = "Pattern NotFound";
                }
            }
            return false;
        }

#endregion

#region "フォト蔵"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Photozou_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://photozou\.jp/photo/show/(?<userId>[0-9]+)/(?<photoId>[0-9]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Photozou_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var http = new HttpVarious();
            var mc = Regex.Match(args.url.Value, @"^http://photozou\.jp/photo/show/(?<userId>[0-9]+)/(?<photoId>[0-9]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                var src = "";
                var show_info = mc.Result("http://api.photozou.jp/rest/photo_info?photo_id=${photoId}");
                if (http.GetData(show_info, null, out src, 0, out args.errmsg, ""))
                {
                    var xdoc = new XmlDocument();
                    var thumbnail_url = "";
                    try
                    {
                        xdoc.LoadXml(src);
                        thumbnail_url = xdoc.SelectSingleNode("/rsp/info/photo/thumbnail_image_url").InnerText;
                    }
                    catch(Exception ex)
                    {
                        args.errmsg = ex.Message;
                        thumbnail_url = "";
                    }
                    if (string.IsNullOrEmpty(thumbnail_url)) return false;
                    var _img = http.GetImage(thumbnail_url, args.url.Key);
                    if (_img == null) return false;
                    args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, _img));
                    args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
                    return true;
                }
            }
            return false;
        }

#endregion

#region "TwitVideo"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool TwitVideo_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://twitvideo\.jp/(\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://twitvideo.jp/img/thumb/${1}")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool TwitVideo_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 0, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "Piapro"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Piapro_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://piapro\.jp/(?:content/[0-9a-z]+|t/[0-9a-zA-Z_\-]+)$");
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Piapro_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var http = new HttpVarious();
            var mc = Regex.Match(args.url.Value, @"^http://piapro\.jp/(?:content/[0-9a-z]+|t/[0-9a-zA-Z_\-]+)$");
            if (mc.Success)
            {
                var src = "";
                if (http.GetData(args.url.Key, null, out src, 0, out args.errmsg, ""))
                {
                    var _mc = Regex.Match(src, "<meta property=\"og:image\" content=\"(?<big_img>http://c1\\.piapro\\.jp/timg/[0-9a-z]+_\\d{14}_0500_0500\\.(?:jpg|png|gif)?)\" />");
                    if (_mc.Success)
                    {
                        //各画像には120x120のサムネイルがある（多分）ので、URLを置き換える。元々ページに埋め込まれている画像は500x500
                        var r = new System.Text.RegularExpressions.Regex(@"_\d{4}_\d{4}");
                        var min_img_url = r.Replace(_mc.Groups["big_img"].Value, "_0120_0120");
                        var _img = http.GetImage(min_img_url, args.url.Key, 0, out args.errmsg);
                        if (_img == null) return false;
                        args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, _img));
                        args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
                        return true;
                    }
                    else
                    {
                        args.errmsg = "Pattern NotFound";
                    }
                }
            }
            return false;
        }

#endregion

#region "Tumblr"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Tumblr_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://(.+\.)?tumblr\.com/.+/?", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Tumblr_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var http = new HttpVarious();
            var TargetUrl = args.url.Value;
            var tmp = http.GetRedirectTo(TargetUrl);
            while (!TargetUrl.Equals(tmp))
            {
                TargetUrl = tmp;
                tmp = http.GetRedirectTo(TargetUrl);
            }
            var mc = Regex.Match(TargetUrl, @"(?<base>http://.+?\.tumblr\.com/)post/(?<postID>[0-9]+)(/(?<subject>.+?)/)?", RegexOptions.IgnoreCase);
            var apiurl = mc.Groups["base"].Value + "api/read?id=" + mc.Groups["postID"].Value;
            var src = "";
            string imgurl = null;
            if (http.GetData(apiurl, null, out src, 0, out args.errmsg, ""))
            {
                var xdoc = new XmlDocument();
                try
                {
                    xdoc.LoadXml(src);

                    var type = xdoc.SelectSingleNode("/tumblr/posts/post").Attributes["type"].Value;
                    if (type == "photo")
                    {
                        imgurl = xdoc.SelectSingleNode("/tumblr/posts/post/photo-url").InnerText;
                    }
                    else
                    {
                        args.errmsg = "PostType:" + type;
                        imgurl = "";
                    }
                }
                catch(Exception)
                {
                    imgurl = "";
                }

                if (!string.IsNullOrEmpty(imgurl))
                {
                    var _img = http.GetImage(imgurl, args.url.Key, 0, out args.errmsg);
                    if (_img == null) return false;
                    args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, _img));
                    args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
                    return true;
                }
            }
            return false;
        }

#endregion

#region "ついっぷるフォト"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool TwipplePhoto_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://p\.twipple\.jp/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                // http://p.twipple.jp/wiki/API_Thumbnail/ja
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://p.twipple.jp/show/large/${contentId}")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool TwipplePhoto_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var image = new HttpVarious().GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (image == null)
                return false;

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, image));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "mypix/shamoji"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool mypix_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://(www\.mypix\.jp|www\.shamoji\.info)/app\.php/picture/(?<contentId>[0-9a-z]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value + "/thumb.jpg"));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool mypix_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 0, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "ow.ly"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Owly_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                    @"^http://ow\.ly/i/(\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://static.ow.ly/photos/thumb/${1}.jpg")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Owly_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 0, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "vimeo"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Vimeo_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://vimeo\.com/[0-9]+", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Vimeo_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var http = new HttpVarious();
            var mc = Regex.Match(args.url.Value, @"http://vimeo\.com/(?<postID>[0-9]+)", RegexOptions.IgnoreCase);
            var apiurl = "http://vimeo.com/api/v2/video/" + mc.Groups["postID"].Value + ".xml";
            var src = "";
            string imgurl = null;
            if (http.GetData(apiurl, null, out src, 0, out args.errmsg, ""))
            {
                var xdoc = new XmlDocument();
                var sb = new StringBuilder();
                try
                {
                    xdoc.LoadXml(src);
                    try
                    {
                        var tmp = xdoc.SelectSingleNode("videos/video/title").InnerText;
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            sb.Append(Properties.Resources.VimeoInfoText1);
                            sb.Append(tmp);
                            sb.AppendLine();
                        }
                    }
                    catch(Exception)
                    {
                    }
                    try
                    {
                        var tmpdate = new DateTime();
                        if (DateTime.TryParse(xdoc.SelectSingleNode("videos/video/upload_date").InnerText, out tmpdate))
                        {
                            sb.Append(Properties.Resources.VimeoInfoText2);
                            sb.Append(tmpdate);
                            sb.AppendLine();
                        }
                    }
                    catch(Exception)
                    {
                    }
                    try
                    {
                        var tmp = xdoc.SelectSingleNode("videos/video/stats_number_of_likes").InnerText;
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            sb.Append(Properties.Resources.VimeoInfoText3);
                            sb.Append(tmp);
                            sb.AppendLine();
                        }
                    }
                    catch(Exception)
                    {
                    }
                    try
                    {
                        var tmp = xdoc.SelectSingleNode("videos/video/stats_number_of_plays").InnerText;
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            sb.Append(Properties.Resources.VimeoInfoText4);
                            sb.Append(tmp);
                            sb.AppendLine();
                        }
                    }
                    catch(Exception)
                    {
                    }
                    try
                    {
                        var tmp = xdoc.SelectSingleNode("videos/video/stats_number_of_comments").InnerText;
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            sb.Append(Properties.Resources.VimeoInfoText5);
                            sb.Append(tmp);
                            sb.AppendLine();
                        }
                    }
                    catch(Exception)
                    {
                    }
                    try
                    {
                        var sec = 0;
                        if (int.TryParse(xdoc.SelectSingleNode("videos/video/duration").InnerText, out sec))
                        {
                            sb.Append(Properties.Resources.VimeoInfoText6);
                            sb.AppendFormat("{0:d}:{1:d2}", sec / 60, sec % 60);
                            sb.AppendLine();
                        }
                    }
                    catch(Exception)
                    {
                    }
                    try
                    {
                        var tmp = xdoc.SelectSingleNode("videos/video/thumbnail_medium").InnerText;
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            imgurl = tmp;
                        }
                    }
                    catch(Exception)
                    {
                    }
                }
                catch(Exception)
                {
                    imgurl = "";
                }

                if (!string.IsNullOrEmpty(imgurl))
                {
                    var _img = http.GetImage(imgurl, args.url.Key, 0, out args.errmsg);
                    if (_img == null) return false;
                    args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, _img));
                    args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, sb.ToString().Trim()));
                    return true;
                }
            }
            return false;
        }

#endregion

#region "cloudfiles"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool CloudFiles_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://c[0-9]+\.cdn[0-9]+\.cloudfiles\.rackspacecloud\.com/[a-z_0-9]+", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool CloudFiles_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 0, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }
#endregion

#region "Instagram"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool instagram_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 "^http://instagr.am/p/.+/", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value + "media/?size=m"));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool instagram_CreateImage(CreateImageArgs args)
        {
            var http = new HttpVarious();
            var imgUrl = http.GetRedirectTo(args.url.Value);

            if (string.IsNullOrEmpty(imgUrl)) return false;

            var img = http.GetImage(imgUrl, args.url.Key, 10000, out args.errmsg);
            if (img == null) return false;

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "pikubo"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool pikubo_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://pikubo\.me/([a-z0-9-_]+)", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://pikubo.me/q/${1}")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool pikubo_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します

            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 0, out args.errmsg);
            if (img == null) return false;
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

#endregion

#region "Foursquare"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Foursquare_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 "^https?://(4sq|foursquare).com/", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                //var mapsUrl = Foursquare.GetInstance.GetMapsUri(args.url);
                //if (mapsUrl == null) return false;
                if (!AppendSettingDialog.Instance.IsPreviewFoursquare) return false;
                args.imglist.Add(new KeyValuePair<String, String>(String.IsNullOrEmpty(args.extended) ? args.url : args.extended, ""));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Foursquare_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var tipsText = "";
            var mapsUrl = Foursquare.GetInstance.GetMapsUri(args.url.Key, ref tipsText);
            if (mapsUrl == null) return false;
            var img = (new HttpVarious()).GetImage(mapsUrl, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, tipsText));
            return true;
        }
#endregion

#region "Twitter Geo"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool TwitterGeo_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            if (args.geoInfo != null && (args.geoInfo.Latitude != 0 || args.geoInfo.Longitude != 0))
            {
                var url = MapThumb.GetDefaultInstance().CreateStaticMapUrl(args.geoInfo);
                args.imglist.Add(new KeyValuePair<string, string>(url, url));
                return true;
            }
            return false;
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool TwitterGeo_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            var url = args.url.Value;
            try
            {
                // URLをStaticMapAPIから通常のURLへ変換
                var m = Regex.Match(url, @"^.+=(?<lat>\d+(\.\d+)?),(?<lon>\d+(\.\d+)?)(&.+)?$");
                if (m.Success)
                {
                    var lat = double.Parse(m.Groups["lat"].Value);
                    var lon = double.Parse(m.Groups["lon"].Value);
                    url = MapThumb.GetDefaultInstance().CreateMapLinkUrl(lat, lon);
                }
            }
            catch(Exception)
            {
                url = args.url.Value;
            }
            args.pics.Add(new KeyValuePair<string, Image>(url, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(url, ""));
            return true;
        }
#endregion

#region "TINAMI"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Tinami_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            //http://www.tinami.com/view/250818
            //http://tinami.jp/5dj6 (短縮URL)
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://www\.tinami\.com/view/\d+$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value));
                return true;
            }
            // 短縮URL
            mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                             @"^http://tinami\.jp/(\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                try
                {
                    args.imglist.Add(new KeyValuePair<string, string>(args.url, "http://www.tinami.com/view/" + RadixConvert.ToInt32(mc.Result("${1}"), 36).ToString()));
                    return true;
                }
                catch(ArgumentOutOfRangeException)
                {
                }
            }
            return false;
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Tinami_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var http = new HttpVarious();
            var mc = Regex.Match(args.url.Value, @"^http://www\.tinami\.com/view/(?<ContentId>\d+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                var src = "";
                var ContentInfo = mc.Result("http://api.tinami.com/content/info?api_key=" + ApplicationSettings.TINAMIApiKey +
                                            "&cont_id=${ContentId}");
                if (http.GetData(ContentInfo, null, out src, 0, out args.errmsg, ""))
                {
                    var xdoc = new XmlDocument();
                    var thumbnail_url = "";
                    try
                    {
                        xdoc.LoadXml(src);
                        var stat = xdoc.SelectSingleNode("/rsp").Attributes.GetNamedItem("stat").InnerText;
                        if (stat == "ok")
                        {
                            if (xdoc.SelectSingleNode("/rsp/content/thumbnails/thumbnail_150x150") != null)
                            {
                                var nd = xdoc.SelectSingleNode("/rsp/content/thumbnails/thumbnail_150x150");
                                thumbnail_url = nd.Attributes.GetNamedItem("url").InnerText;
                                if (string.IsNullOrEmpty(thumbnail_url)) return false;
                                var _img = http.GetImage(thumbnail_url, args.url.Key);
                                if (_img == null) return false;
                                args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, _img));
                                args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
                                return true;
                            }
                            else
                            {
                                //エラー処理 エラーメッセージが返ってきた場合はここで処理
                                if (xdoc.SelectSingleNode("/rsp/err") != null)
                                {
                                    args.errmsg = xdoc.SelectSingleNode("/rsp/err").Attributes.GetNamedItem("msg").InnerText;
                                }
                                return false;
                            }
                        }
                        else
                        {
                            // TODO rsp stat=failの際のエラーメッセージ返却はAPI拡張待ち(2011/8/2要望済み)
                            // TODO 後日APIレスポンスを確認し修正すること
                            if (xdoc.SelectSingleNode("/rsp/err") != null)
                            {
                                args.errmsg = xdoc.SelectSingleNode("/rsp/err").Attributes.GetNamedItem("msg").InnerText;
                            }
                            else
                            {
                                args.errmsg = "DeletedOrSuspended";
                            }
                            return false;
                        }
                    }
                    catch(Exception ex)
                    {
                        args.errmsg = ex.Message;
                        return false;
                    }
                }
            }
            return false;
        }

#endregion

#region "Twitter公式"
        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Twimg_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^https?://p\.twimg\.com/.*$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value));
                return true;
            }
            return false;
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Twimg_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var http = new HttpVarious();
            var mc = Regex.Match(args.url.Value, @"^https?://p\.twimg\.com/.*$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                var src = "";
                var ContentInfo = args.url.Value + ":thumb";
                var _img = http.GetImage(ContentInfo, src, 0, out args.errmsg);
                if (_img == null) return false;
                args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, _img));
                args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            }
            return false;
        }

#endregion

        #region TwitrPix

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool TwitrPix_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://twitrpix\.com/(\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://img.twitrpix.com/thumb/$1")));
                return true;
            }
            return false;
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool TwitrPix_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

        #endregion

        #region Pckles

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool Pckles_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^https?://pckles\.com/\w+/\w+$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("$0.resize.jpg")));
                return true;
            }
            return false;
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool Pckles_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var img = (new HttpVarious()).GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (img == null)
            {
                return false;
            }
            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

        #endregion

        #region via.me

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool ViaMe_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^https?://via\.me/-(\w+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Value));
                return true;
            }
            return false;
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>
        private bool ViaMe_CreateImage(CreateImageArgs args)
        {
            var mc = Regex.Match(args.url.Value, @"^https?://via\.me/-(\w+)$", RegexOptions.IgnoreCase);
            var apiUrl = mc.Result("http://via.me/api/v1/posts/$1");

            var src = "";
            if ((new HttpVarious()).GetData(apiUrl, null, out src, 0, out args.errmsg, MyCommon.GetUserAgentString()))
            {
                using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(src), XmlDictionaryReaderQuotas.Max))
                {
                    var xElm = XElement.Load(jsonReader);
                    var thumbUrlElm = ((IEnumerable)xElm.XPathEvaluate("/response/post/thumb_url/text()")).Cast<XText>().FirstOrDefault();
                    if (thumbUrlElm == null)
                    {
                        return false;
                    }

                    var thumbUrl = thumbUrlElm.Value;

                    // TODO: サムネイル画像読み込み処理を記述します
                    var img = (new HttpVarious()).GetImage(thumbUrl, args.url.Key, 10000, out args.errmsg);
                    if (img == null)
                    {
                        return false;
                    }
                    // 成功した場合はURLに対応する画像、ツールチップテキストを登録
                    args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, img));
                    args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region tuna.be

        /// <summary>
        /// URL解析部で呼び出されるサムネイル画像URL作成デリゲート
        /// </summary>
        /// <param name="args">class GetUrlArgs
        ///                                 args.url        URL文字列
        ///                                 args.imglist    解析成功した際にこのリストに元URL、サムネイルURLの形で作成するKeyValuePair
        /// </param>
        /// <returns>成功した場合True,失敗の場合False</returns>
        /// <remarks>args.imglistには呼び出しもとで使用しているimglistをそのまま渡すこと</remarks>

        private bool TunaBe_GetUrl(GetUrlArgs args)
        {
            // TODO URL判定処理を記述
            var mc = Regex.Match(string.IsNullOrEmpty(args.extended) ? args.url : args.extended,
                                 @"^http://tuna\.be/t/(?<entryId>[a-zA-Z0-9\.\-_]+)$", RegexOptions.IgnoreCase);
            if (mc.Success)
            {
                // TODO 成功時はサムネイルURLを作成しimglist.Addする
                // http://tuna.be/api/
                args.imglist.Add(new KeyValuePair<string, string>(args.url, mc.Result("http://tuna.be/show/thumb/${entryId}")));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// BackgroundWorkerから呼び出されるサムネイル画像作成デリゲート
        /// </summary>
        /// <param name="args">class CreateImageArgs
        ///                                 KeyValuePair<string, string> url                  元URLとサムネイルURLのKeyValuePair
        ///                                 List<KeyValuePair<string, Image>> pics         元URLとサムネイル画像のKeyValuePair
        ///                                 List<KeyValuePair<string, string>> tooltiptext 元URLとツールチップテキストのKeyValuePair
        ///                                 string errmsg                                        取得に失敗した際のエラーメッセージ
        /// </param>
        /// <returns>サムネイル画像作成に成功した場合はTrue,失敗した場合はFalse
        /// なお失敗した場合はargs.errmsgにエラーを表す文字列がセットされる</returns>
        /// <remarks></remarks>

        private bool TunaBe_CreateImage(CreateImageArgs args)
        {
            // TODO: サムネイル画像読み込み処理を記述します
            var image = new HttpVarious().GetImage(args.url.Value, args.url.Key, 10000, out args.errmsg);
            if (image == null)
                return false;

            // 成功した場合はURLに対応する画像、ツールチップテキストを登録
            args.pics.Add(new KeyValuePair<string, Image>(args.url.Key, image));
            args.tooltipText.Add(new KeyValuePair<string, string>(args.url.Key, ""));
            return true;
        }

        #endregion
    }
}
