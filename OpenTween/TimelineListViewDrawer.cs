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

using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Models;
using OpenTween.OpenTweenCustomControl;

namespace OpenTween
{
    public sealed class TimelineListViewDrawer : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        public ThemeManager Theme { get; set; }

        public MyCommon.IconSizes IconSize
        {
            get => this.iconSize;
            set
            {
                if (this.iconSize == value)
                    return;

                this.iconSize = value;
                this.ApplyIconSize();
            }
        }

        private bool Use2ColumnsMode
            => this.IconSize == MyCommon.IconSizes.Icon48_2;

        private int IconSizeNumeric
            => this.IconSize switch
            {
                MyCommon.IconSizes.Icon16 => 16,
                MyCommon.IconSizes.Icon24 => 26, // 24x24の場合に26と指定しているのはMSゴシック系フォントのための仕様
                MyCommon.IconSizes.Icon48 => 48,
                MyCommon.IconSizes.Icon48_2 => 48,
                _ => 0,
            };

        private readonly DetailsListView listView;
        private readonly OTBaseForm parentForm;
        private readonly TabModel tab;
        private readonly TimelineListViewCache listViewCache;
        private readonly ImageCache iconCache;
        private readonly ImageList listViewImageList = new(); // ListViewItemの高さ変更用
        private MyCommon.IconSizes iconSize;

        public TimelineListViewDrawer(
            DetailsListView listView,
            TabModel tab,
            TimelineListViewCache listViewCache,
            ImageCache iconCache,
            ThemeManager theme
        )
        {
            this.listView = listView;
            this.parentForm = (OTBaseForm)listView.FindForm();
            this.tab = tab;
            this.listViewCache = listViewCache;
            this.iconCache = iconCache;
            this.Theme = theme;

            this.RegisterHandlers();
            this.listView.SmallImageList = this.listViewImageList;
            this.listView.OwnerDraw = true;
        }

        private void RegisterHandlers()
        {
            this.listView.DrawItem += this.ListView_DrawItem;
            this.listView.DrawSubItem += this.ListView_DrawSubItem;
        }

        private void UnregisterHandlers()
        {
            this.listView.DrawItem -= this.ListView_DrawItem;
            this.listView.DrawSubItem -= this.ListView_DrawSubItem;
        }

        private void ApplyIconSize()
        {
            // ディスプレイの DPI 設定を考慮したサイズを設定する
            var scaledIconHeight = this.IconSize != MyCommon.IconSizes.IconNone
                ? this.listView.LogicalToDeviceUnits(this.IconSizeNumeric)
                : 1;

            this.listViewImageList.ImageSize = new(1, scaledIconHeight);
        }

        private void DrawListViewItemIcon(DrawListViewItemEventArgs e)
        {
            if (this.IconSize == 0) return;

            var item = e.Item;

            // e.Bounds.Leftが常に0を指すから自前で計算
            var itemRect = item.Bounds;
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
            var scaleFactor = this.listView.DeviceDpi / 96f;
            var scaledIconSize = new SizeF(this.IconSizeNumeric * scaleFactor, this.IconSizeNumeric * scaleFactor).ToSize();
            var scaledStateSize = new SizeF(16 * scaleFactor, 16 * scaleFactor).ToSize();

            var iconRect = Rectangle.Intersect(new Rectangle(e.Item.GetBounds(ItemBoundsPortion.Icon).Location, scaledIconSize), itemRect);
            iconRect.Offset(0, Math.Max(0, (itemRect.Height - scaledIconSize.Height) / 2));

            var post = this.tab[item.Index];
            var img = this.LoadListViewIconLazy(post, scaledIconSize.Width);
            if (img != null)
            {
                e.Graphics.FillRectangle(Brushes.White, iconRect);
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                try
                {
                    e.Graphics.DrawImage(img.Image, iconRect);
                }
                catch (ArgumentException)
                {
                }
            }

            if (post.StateIndex > -1)
            {
                var stateRect = Rectangle.Intersect(new Rectangle(new Point(iconRect.X + scaledIconSize.Width + 2, iconRect.Y), scaledStateSize), itemRect);
                if (stateRect.Width > 0)
                    e.Graphics.DrawIcon(this.GetPostStateIcon(post.StateIndex), stateRect);
            }
        }

        private MemoryImage? LoadListViewIconLazy(PostClass post, int scaledIconSize)
        {
            if (scaledIconSize <= 0)
                return null;

            var normalImageUrl = post.ImageUrl;
            if (MyCommon.IsNullOrEmpty(normalImageUrl))
                return null;

            var sizeName = Twitter.DecideProfileImageSize(scaledIconSize);
            var cachedImage = this.iconCache.TryGetLargerOrSameSizeFromCache(normalImageUrl, sizeName);
            if (cachedImage != null)
                return cachedImage;

            // キャッシュにない画像の場合は読み込みが完了してから再描画する
            _ = Task.Run(async () =>
            {
                try
                {
                    var imageUrl = Twitter.CreateProfileImageUrl(normalImageUrl, sizeName);
                    await this.iconCache.DownloadImageAsync(imageUrl);
                }
                catch (InvalidImageException)
                {
                    return;
                }
                catch (HttpRequestException)
                {
                    return;
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                await this.parentForm.InvokeAsync(() =>
                {
                    if (this.listView.IsDisposed)
                        return;

                    if (this.listView.VirtualListSize == 0)
                        return;

                    // ロード中に index の指す行が変化している可能性がある
                    var newIndex = this.tab.IndexOf(post.StatusId);
                    if (newIndex != -1)
                        this.listView.RedrawItems(newIndex, newIndex, true);
                });
            });

            return null;
        }

        private Icon GetPostStateIcon(int stateIndex)
        {
            return stateIndex switch
            {
                0 => Properties.Resources.PostState00,
                1 => Properties.Resources.PostState01,
                2 => Properties.Resources.PostState02,
                3 => Properties.Resources.PostState03,
                4 => Properties.Resources.PostState04,
                5 => Properties.Resources.PostState05,
                6 => Properties.Resources.PostState06,
                7 => Properties.Resources.PostState07,
                8 => Properties.Resources.PostState08,
                9 => Properties.Resources.PostState09,
                10 => Properties.Resources.PostState10,
                11 => Properties.Resources.PostState11,
                12 => Properties.Resources.PostState12,
                13 => Properties.Resources.PostState13,
                14 => Properties.Resources.PostState14,
                _ => throw new IndexOutOfRangeException(),
            };
        }

        private Brush GetBackColorBrush(ListItemBackColor backColor)
        {
            return backColor switch
            {
                ListItemBackColor.Self => this.Theme.BrushSelf,
                ListItemBackColor.AtSelf => this.Theme.BrushAtSelf,
                ListItemBackColor.Target => this.Theme.BrushTarget,
                ListItemBackColor.AtTarget => this.Theme.BrushAtTarget,
                ListItemBackColor.AtFromTarget => this.Theme.BrushAtFromTarget,
                ListItemBackColor.AtTo => this.Theme.BrushAtTo,
                _ => this.Theme.BrushListBackcolor,
            };
        }

        private Color GetForeColor(ListItemForeColor foreColor)
        {
            return foreColor switch
            {
                ListItemForeColor.Fav => this.Theme.ColorFav,
                ListItemForeColor.Retweet => this.Theme.ColorRetweet,
                ListItemForeColor.OWL => this.Theme.ColorOWL,
                ListItemForeColor.Unread => this.Theme.ColorUnread,
                _ => this.Theme.ColorRead,
            };
        }

        private Font GetFont(ListItemFont font)
        {
            return font switch
            {
                ListItemFont.Unread => this.Theme.FontUnread,
                _ => this.Theme.FontReaded,
            };
        }

        private Font GetFontBold(ListItemFont font)
        {
            return font switch
            {
                ListItemFont.Unread => this.Theme.FontUnreadBold,
                _ => this.Theme.FontReadedBold,
            };
        }

        private void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.State == 0) return;
            e.DrawDefault = false;

            var style = this.listViewCache.GetStyle(e.ItemIndex);

            Brush brs2;
            if (!e.Item.Selected) // e.ItemStateでうまく判定できない？？？
            {
                brs2 = this.GetBackColorBrush(style.BackColor);
            }
            else
            {
                // 選択中の行
                if (((Control)sender).Focused)
                    brs2 = this.Theme.BrushHighLight;
                else
                    brs2 = this.Theme.BrushDeactiveSelection;
            }
            e.Graphics.FillRectangle(brs2, e.Bounds);
            e.DrawFocusRectangle();
            this.DrawListViewItemIcon(e);
        }

        private void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ItemState == 0) return;

            if (e.ColumnIndex > 0)
            {
                // アイコン以外の列
                var post = this.tab[e.ItemIndex];
                var style = this.listViewCache.GetStyle(e.ItemIndex);
                var font = this.GetFont(style.Font);

                RectangleF rct = e.Bounds;
                rct.Width = e.Header.Width;
                var fontHeight = font.Height;
                if (this.Use2ColumnsMode)
                {
                    rct.Y += fontHeight;
                    rct.Height -= fontHeight;
                }

                var drawLineCount = Math.Max(1, Math.DivRem((int)rct.Height, fontHeight, out var heightDiff));

                // フォントの高さの半分を足してるのは保険。無くてもいいかも。
                if (this.Use2ColumnsMode || drawLineCount > 1)
                {
                    if (heightDiff < fontHeight * 0.7)
                    {
                        // 最終行が70%以上欠けていたら、最終行は表示しない
                        rct.Height = (fontHeight * drawLineCount) - 1;
                    }
                    else
                    {
                        drawLineCount += 1;
                    }
                }

                if (rct.Width > 0)
                {
                    Color color;
                    if (e.Item.Selected)
                    {
                        color = ((Control)sender).Focused
                            ? this.Theme.ColorHighLight
                            : this.Theme.ColorUnread;
                    }
                    else
                    {
                        color = this.GetForeColor(style.ForeColor);
                    }

                    if (this.Use2ColumnsMode)
                    {
                        var rctB = e.Bounds;
                        rctB.Width = e.Header.Width;
                        rctB.Height = fontHeight;

                        var fontBold = this.GetFontBold(style.Font);

                        var formatFlags1 = TextFormatFlags.WordBreak |
                            TextFormatFlags.EndEllipsis |
                            TextFormatFlags.GlyphOverhangPadding |
                            TextFormatFlags.NoPrefix;

                        TextRenderer.DrawText(
                            e.Graphics,
                            post.IsDeleted ? "(DELETED)" : post.TextSingleLine,
                            font,
                            Rectangle.Round(rct),
                            color,
                            formatFlags1);

                        var formatFlags2 = TextFormatFlags.SingleLine |
                            TextFormatFlags.EndEllipsis |
                            TextFormatFlags.GlyphOverhangPadding |
                            TextFormatFlags.NoPrefix;

                        TextRenderer.DrawText(
                            e.Graphics,
                            e.Item.SubItems[4].Text + " / " + e.Item.SubItems[1].Text + " (" + e.Item.SubItems[3].Text + ") " + e.Item.SubItems[5].Text + e.Item.SubItems[6].Text + " [" + e.Item.SubItems[7].Text + "]",
                            fontBold,
                            rctB,
                            color,
                            formatFlags2);
                    }
                    else
                    {
                        string text;
                        if (e.ColumnIndex != 2)
                            text = e.SubItem.Text;
                        else
                            text = post.IsDeleted ? "(DELETED)" : post.TextSingleLine;

                        if (drawLineCount == 1)
                        {
                            var formatFlags = TextFormatFlags.SingleLine |
                                TextFormatFlags.EndEllipsis |
                                TextFormatFlags.GlyphOverhangPadding |
                                TextFormatFlags.NoPrefix |
                                TextFormatFlags.VerticalCenter;

                            TextRenderer.DrawText(
                                e.Graphics,
                                text,
                                font,
                                Rectangle.Round(rct),
                                color,
                                formatFlags);
                        }
                        else
                        {
                            var formatFlags = TextFormatFlags.WordBreak |
                                TextFormatFlags.EndEllipsis |
                                TextFormatFlags.GlyphOverhangPadding |
                                TextFormatFlags.NoPrefix;

                            TextRenderer.DrawText(
                                e.Graphics,
                                text,
                                font,
                                Rectangle.Round(rct),
                                color,
                                formatFlags);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.UnregisterHandlers();
            this.listView.SmallImageList = null;
            this.listViewImageList.Dispose();
            this.IsDisposed = true;
        }
    }
}
