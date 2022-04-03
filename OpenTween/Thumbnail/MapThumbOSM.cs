// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Models;
using OpenTween.Setting;

namespace OpenTween.Thumbnail
{
    public class MapThumbOSM : MapThumb
    {
        public override Task<ThumbnailInfo> GetThumbnailInfoAsync(PostClass.StatusGeo geo)
        {
            var size = new Size(SettingManager.Instance.Common.MapThumbnailWidth, SettingManager.Instance.Common.MapThumbnailHeight);
            var zoom = SettingManager.Instance.Common.MapThumbnailZoom;

            var thumb = new OSMThumbnailInfo(geo.Latitude, geo.Longitude, zoom, size)
            {
                MediaPageUrl = this.CreateMapLinkUrl(geo.Latitude, geo.Longitude),
            };

            return Task.FromResult((ThumbnailInfo)thumb);
        }

        public string CreateMapLinkUrl(double latitude, double longitude)
        {
            var zoom = SettingManager.Instance.Common.MapThumbnailZoom;

            return $"https://www.openstreetmap.org/?mlat={latitude}&mlon={longitude}#map={zoom}/{latitude}/{longitude}";
        }
    }

    public class OSMThumbnailInfo : ThumbnailInfo
    {
        /// <summary>openstreetmap.org タイルサーバー</summary>
        public static readonly string TileServerBase = "https://a.tile.openstreetmap.org";

        /// <summary>タイル画像一枚当たりの大きさ (ピクセル単位)</summary>
        public static readonly Size TileSize = new(256, 256);

        /// <summary>画像の中心点の緯度</summary>
        public double Latitude { get; }

        /// <summary>画像の中心点の経度</summary>
        public double Longitude { get; }

        /// <summary>地図のズームレベル</summary>
        public int Zoom { get; }

        /// <summary>生成するサムネイル画像のサイズ (ピクセル単位)</summary>
        public Size ThumbnailSize { get; }

        public OSMThumbnailInfo(double latitude, double longitude, int zoom, Size thumbSize)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Zoom = zoom;
            this.ThumbnailSize = thumbSize;
        }

        public override async Task<MemoryImage> LoadThumbnailImageAsync(HttpClient http, CancellationToken cancellationToken)
        {
            // 画像中央に描画されるタイル (ピクセル単位ではなくタイル番号を表す)
            // タイル番号に小数部が含まれているが、これはタイル内の相対的な位置を表すためこのまま保持する
            var centerTileNum = this.WorldToTilePos(this.Longitude, this.Latitude, this.Zoom);

            // 画像左上に描画されるタイル
            var topLeftTileNum = PointF.Add(centerTileNum, new SizeF(-this.ThumbnailSize.Width / 2.0f / TileSize.Width, -this.ThumbnailSize.Height / 2.0f / TileSize.Height));

            // タイル番号の小数部をもとに、タイル画像を描画する際のピクセル単位のオフセットを算出する
            var tileOffset = Size.Round(new SizeF(-TileSize.Width * (topLeftTileNum.X - (int)topLeftTileNum.X), -TileSize.Height * (topLeftTileNum.Y - (int)topLeftTileNum.Y)));

            // 縦横のタイル枚数
            var tileCountX = (int)Math.Ceiling((double)(this.ThumbnailSize.Width + Math.Abs(tileOffset.Width)) / TileSize.Width);
            var tileCountY = (int)Math.Ceiling((double)(this.ThumbnailSize.Height + Math.Abs(tileOffset.Height)) / TileSize.Height);

            // 読み込む対象となるタイル画像が 10 枚を越えていたら中断
            // ex. 一辺が 512px 以内のサムネイル画像を生成する場合、必要なタイル画像は最大で 9 枚
            if (tileCountX * tileCountY > 10)
                throw new OperationCanceledException();

            // タイル画像を読み込む
            var tilesTask = new Task<MemoryImage>[tileCountX, tileCountY];

            foreach (var x in Enumerable.Range(0, tileCountX))
            {
                foreach (var y in Enumerable.Range(0, tileCountY))
                {
                    var tilePos = Point.Add(Point.Truncate(topLeftTileNum), new Size(x, y));
                    tilesTask[x, y] = this.LoadTileImageAsync(http, tilePos);
                }
            }

            await Task.WhenAll(tilesTask.Cast<Task<MemoryImage>>())
                .ConfigureAwait(false);

            using var bitmap = new Bitmap(this.ThumbnailSize.Width, this.ThumbnailSize.Height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.TranslateTransform(tileOffset.Width, tileOffset.Height);

                foreach (var x in Enumerable.Range(0, tileCountX))
                {
                    foreach (var y in Enumerable.Range(0, tileCountY))
                    {
                        using var image = tilesTask[x, y].Result;
                        g.DrawImage(image.Image, TileSize.Width * x, TileSize.Height * y);
                    }
                }
            }

            MemoryImage? result = null;
            try
            {
                result = MemoryImage.CopyFromImage(bitmap);
                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
        }

        /// <summary>指定されたタイル番号のタイル画像を読み込むメソッド</summary>
        private async Task<MemoryImage> LoadTileImageAsync(HttpClient http, Point pos)
        {
            var tileUrl = TileServerBase + $"/{this.Zoom}/{pos.X}/{pos.Y}.png";

            using var stream = await http.GetStreamAsync(tileUrl)
                .ConfigureAwait(false);

            MemoryImage? result = null;
            try
            {
                result = await MemoryImage.CopyFromStreamAsync(stream).ConfigureAwait(false);
                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
        }

        /// <summary>経度・緯度からタイル番号を算出するメソッド</summary>
        private PointF WorldToTilePos(double lon, double lat, int zoom)
        {
            // 計算式は http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#C.23 に基づく
            return new PointF
            {
                X = (float)((lon + 180.0) / 360.0 * (1 << zoom)),
                Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                    1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom)),
            };
        }
    }
}
