using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenTween.Api;
using Xunit;

namespace OpenTween
{
    public class MediaSelectorTest
    {
        [Fact]
        public void Initialize_TwitterTest()
        {
            using (var mediaSelector = new MediaSelector())
            {
                var twitter = new Twitter();
                var config = TwitterConfiguration.DefaultConfiguration();
                mediaSelector.Initialize(twitter, config, "Twitter");

                Assert.NotEqual(-1, mediaSelector.ImageServiceCombo.Items.IndexOf("Twitter"));

                // 投稿先に Twitter が選択されている
                Assert.Equal("Twitter", mediaSelector.ImageServiceCombo.Text);

                // ページ番号が初期化された状態
                var pages = mediaSelector.ImagePageCombo.Items;
                Assert.Equal(new[] { "1" }, pages.Cast<object>().Select(x => x.ToString()));
            }
        }

        [Fact]
        public void Initialize_yfrogTest()
        {
            using (var mediaSelector = new MediaSelector())
            {
                var twitter = new Twitter();
                var config = TwitterConfiguration.DefaultConfiguration();
                mediaSelector.Initialize(twitter, config, "yfrog");

                // 投稿先に yfrog が選択されている
                Assert.Equal("yfrog", mediaSelector.ImageServiceCombo.Text);

                // ページ番号が初期化された状態
                var pages = mediaSelector.ImagePageCombo.Items;
                Assert.Equal(new[] { "1" }, pages.Cast<object>().Select(x => x.ToString()));
            }
        }

        [Fact]
        public void BeginSelection_BlankTest()
        {
            using (var mediaSelector = new MediaSelector { Visible = false, Enabled = false })
            {
                mediaSelector.Initialize(new Twitter(), TwitterConfiguration.DefaultConfiguration(), "Twitter");

                var eventCalled = false;
                mediaSelector.BeginSelecting += (o, e) => eventCalled = true;

                mediaSelector.BeginSelection();

                Assert.True(eventCalled);

                Assert.True(mediaSelector.Visible);
                Assert.True(mediaSelector.Enabled);

                // 1 ページ目のみ選択可能な状態
                var pages = mediaSelector.ImagePageCombo.Items;
                Assert.Equal(new[] { "1" }, pages.Cast<object>().Select(x => x.ToString()));

                // 1 ページ目が表示されている
                Assert.Equal("1", mediaSelector.ImagePageCombo.Text);
                Assert.Equal("", mediaSelector.ImagefilePathText.Text);
                Assert.Null(mediaSelector.ImageSelectedPicture.Image);
            }
        }

        [Fact]
        public async Task BeginSelection_FilePathTest()
        {
            using (var mediaSelector = new MediaSelector { Visible = false, Enabled = false })
            {
                mediaSelector.Initialize(new Twitter(), TwitterConfiguration.DefaultConfiguration(), "Twitter");

                var eventCalled = false;
                mediaSelector.BeginSelecting += (o, e) => eventCalled = true;

                var images = new[] { "Resources/re.gif" };
                mediaSelector.BeginSelection(images);

                Assert.True(eventCalled);

                Assert.True(mediaSelector.Visible);
                Assert.True(mediaSelector.Enabled);

                // 2 ページ目まで選択可能な状態
                var pages = mediaSelector.ImagePageCombo.Items;
                Assert.Equal(new[] { "1", "2" }, pages.Cast<object>().Select(x => x.ToString()));

                // 1 ページ目が表示されている
                Assert.Equal("1", mediaSelector.ImagePageCombo.Text);
                Assert.Equal(Path.GetFullPath("Resources/re.gif"), mediaSelector.ImagefilePathText.Text);

                using (var imageStream = File.OpenRead("Resources/re.gif"))
                using (var image = await MemoryImage.CopyFromStreamAsync(imageStream).ConfigureAwait(false))
                {
                    Assert.Equal(image, mediaSelector.ImageSelectedPicture.Image);
                }
            }
        }

        [Fact]
        public void BeginSelection_MemoryImageTest()
        {
            using (var mediaSelector = new MediaSelector { Visible = false, Enabled = false })
            {
                mediaSelector.Initialize(new Twitter(), TwitterConfiguration.DefaultConfiguration(), "Twitter");

                var eventCalled = false;
                mediaSelector.BeginSelecting += (o, e) => eventCalled = true;

                using (var bitmap = new Bitmap(width: 200, height: 200))
                {
                    mediaSelector.BeginSelection(bitmap);
                }

                Assert.True(eventCalled);

                Assert.True(mediaSelector.Visible);
                Assert.True(mediaSelector.Enabled);

                // 2 ページ目まで選択可能な状態
                var pages = mediaSelector.ImagePageCombo.Items;
                Assert.Equal(new[] { "1", "2" }, pages.Cast<object>().Select(x => x.ToString()));

                // 1 ページ目が表示されている
                Assert.Equal("1", mediaSelector.ImagePageCombo.Text);
                Assert.True(Regex.IsMatch(mediaSelector.ImagefilePathText.Text, @"^<>MemoryImage://\d+.png$"));

                using (var bitmap = new Bitmap(width: 200, height: 200))
                using (var image = MemoryImage.CopyFromImage(bitmap))
                {
                    Assert.Equal(image, mediaSelector.ImageSelectedPicture.Image);
                }
            }
        }

        [Fact]
        public async Task BeginSelection_MultiImageTest()
        {
            using (var mediaSelector = new MediaSelector { Visible = false, Enabled = false })
            {
                mediaSelector.Initialize(new Twitter(), TwitterConfiguration.DefaultConfiguration(), "Twitter");

                var images = new[] { "Resources/re.gif", "Resources/re1.png" };
                mediaSelector.BeginSelection(images);

                // 3 ページ目まで選択可能な状態
                var pages = mediaSelector.ImagePageCombo.Items;
                Assert.Equal(new[] { "1", "2", "3" }, pages.Cast<object>().Select(x => x.ToString()));

                // 1 ページ目が表示されている
                Assert.Equal("1", mediaSelector.ImagePageCombo.Text);
                Assert.Equal(Path.GetFullPath("Resources/re.gif"), mediaSelector.ImagefilePathText.Text);

                using (var imageStream = File.OpenRead("Resources/re.gif"))
                using (var image = await MemoryImage.CopyFromStreamAsync(imageStream).ConfigureAwait(false))
                {
                    Assert.Equal(image, mediaSelector.ImageSelectedPicture.Image);
                }
            }
        }

        [Fact]
        public void EndSelection_Test()
        {
            using (var mediaSelector = new MediaSelector { Visible = false, Enabled = false })
            {
                mediaSelector.Initialize(new Twitter(), TwitterConfiguration.DefaultConfiguration(), "Twitter");
                mediaSelector.BeginSelection(new[] { "Resources/re.gif" });

                var displayImage = mediaSelector.ImageSelectedPicture.Image; // 表示中の画像

                var eventCalled = false;
                mediaSelector.EndSelecting += (o, e) => eventCalled = true;

                mediaSelector.EndSelection();

                Assert.True(eventCalled);

                Assert.False(mediaSelector.Visible);
                Assert.False(mediaSelector.Enabled);

                Assert.True(displayImage.IsDisposed);
            }
        }

        [Fact]
        public async Task PageChange_Test()
        {
            using (var mediaSelector = new MediaSelector { Visible = false, Enabled = false })
            {
                mediaSelector.Initialize(new Twitter(), TwitterConfiguration.DefaultConfiguration(), "Twitter");

                var images = new[] { "Resources/re.gif", "Resources/re1.png" };
                mediaSelector.BeginSelection(images);

                mediaSelector.ImagePageCombo.SelectedIndex = 0;

                // 1 ページ目
                Assert.Equal("1", mediaSelector.ImagePageCombo.Text);
                Assert.Equal(Path.GetFullPath("Resources/re.gif"), mediaSelector.ImagefilePathText.Text);

                using (var imageStream = File.OpenRead("Resources/re.gif"))
                using (var image = await MemoryImage.CopyFromStreamAsync(imageStream).ConfigureAwait(false))
                {
                    Assert.Equal(image, mediaSelector.ImageSelectedPicture.Image);
                }

                mediaSelector.ImagePageCombo.SelectedIndex = 1;

                // 2 ページ目
                Assert.Equal("2", mediaSelector.ImagePageCombo.Text);
                Assert.Equal(Path.GetFullPath("Resources/re1.png"), mediaSelector.ImagefilePathText.Text);

                using (var imageStream = File.OpenRead("Resources/re1.png"))
                using (var image = await MemoryImage.CopyFromStreamAsync(imageStream).ConfigureAwait(false))
                {
                    Assert.Equal(image, mediaSelector.ImageSelectedPicture.Image);
                }

                mediaSelector.ImagePageCombo.SelectedIndex = 2;

                // 3 ページ目 (新規ページ)
                Assert.Equal("3", mediaSelector.ImagePageCombo.Text);
                Assert.Equal("", mediaSelector.ImagefilePathText.Text);
                Assert.Null(mediaSelector.ImageSelectedPicture.Image);
            }
        }

        [Fact]
        public void PageChange_ImageDisposeTest()
        {
            using (var mediaSelector = new MediaSelector { Visible = false, Enabled = false })
            {
                mediaSelector.Initialize(new Twitter(), TwitterConfiguration.DefaultConfiguration(), "Twitter");

                var images = new[] { "Resources/re.gif", "Resources/re1.png" };
                mediaSelector.BeginSelection(images);

                mediaSelector.ImagePageCombo.SelectedIndex = 0;

                // 1 ページ目
                var page1Image = mediaSelector.ImageSelectedPicture.Image;

                mediaSelector.ImagePageCombo.SelectedIndex = 1;

                // 2 ページ目
                var page2Image = mediaSelector.ImageSelectedPicture.Image;
                Assert.True(page1Image.IsDisposed); // 前ページの画像が破棄されているか

                mediaSelector.ImagePageCombo.SelectedIndex = 2;

                // 3 ページ目 (新規ページ)
                Assert.True(page2Image.IsDisposed); // 前ページの画像が破棄されているか
            }
        }

        [Fact]
        public async Task ImagePathInput_Test()
        {
            using (var mediaSelector = new MediaSelector { Visible = false, Enabled = false })
            {
                mediaSelector.Initialize(new Twitter(), TwitterConfiguration.DefaultConfiguration(), "Twitter");
                mediaSelector.BeginSelection();

                // 画像のファイルパスを入力
                mediaSelector.ImagefilePathText.Text = Path.GetFullPath("Resources/re1.png");
                TestUtils.Validate(mediaSelector.ImagefilePathText);

                // 入力したパスの画像が表示される
                using (var imageStream = File.OpenRead("Resources/re1.png"))
                using (var image = await MemoryImage.CopyFromStreamAsync(imageStream).ConfigureAwait(false))
                {
                    Assert.Equal(image, mediaSelector.ImageSelectedPicture.Image);
                }

                // 2 ページ目まで選択可能な状態
                var pages = mediaSelector.ImagePageCombo.Items;
                Assert.Equal(new[] { "1", "2" }, pages.Cast<object>().Select(x => x.ToString()));
            }
        }

        [Fact]
        public async Task ImagePathInput_ReplaceFileMediaItemTest()
        {
            using (var mediaSelector = new MediaSelector { Visible = false, Enabled = false })
            {
                mediaSelector.Initialize(new Twitter(), TwitterConfiguration.DefaultConfiguration(), "Twitter");

                mediaSelector.BeginSelection(new[] { "Resources/re.gif" });

                // 既に入力されているファイルパスの画像
                var image1 = mediaSelector.ImageSelectedPicture.Image;

                // 別の画像のファイルパスを入力
                mediaSelector.ImagefilePathText.Text = Path.GetFullPath("Resources/re1.png");
                TestUtils.Validate(mediaSelector.ImagefilePathText);

                // 入力したパスの画像が表示される
                using (var imageStream = File.OpenRead("Resources/re1.png"))
                using (var image2 = await MemoryImage.CopyFromStreamAsync(imageStream).ConfigureAwait(false))
                {
                    Assert.Equal(image2, mediaSelector.ImageSelectedPicture.Image);
                }

                // 最初に入力されていたファイルパスの表示用の MemoryImage は破棄される
                Assert.True(image1.IsDisposed);
            }
        }

        [Fact]
        public async Task ImagePathInput_ReplaceMemoryImageMediaItemTest()
        {
            using (var mediaSelector = new MediaSelector { Visible = false, Enabled = false })
            {
                mediaSelector.Initialize(new Twitter(), TwitterConfiguration.DefaultConfiguration(), "Twitter");

                using (var bitmap = new Bitmap(width: 200, height: 200))
                {
                    mediaSelector.BeginSelection(bitmap);
                }

                // 既に入力されているファイルパスの画像
                var image1 = mediaSelector.ImageSelectedPicture.Image;

                // 内部で保持されている MemoryImageMediaItem を取り出す
                var selectedMedia = mediaSelector.ImagePageCombo.SelectedItem;
                var mediaProperty = selectedMedia.GetType().GetProperty("Item");
                var mediaItem = (MemoryImageMediaItem)mediaProperty.GetValue(selectedMedia);

                // 別の画像のファイルパスを入力
                mediaSelector.ImagefilePathText.Text = Path.GetFullPath("Resources/re1.png");
                TestUtils.Validate(mediaSelector.ImagefilePathText);

                // 入力したパスの画像が表示される
                using (var imageStream = File.OpenRead("Resources/re1.png"))
                using (var image2 = await MemoryImage.CopyFromStreamAsync(imageStream).ConfigureAwait(false))
                {
                    Assert.Equal(image2, mediaSelector.ImageSelectedPicture.Image);
                }

                // 最初に入力されていたファイルパスの表示用の MemoryImage は破棄される
                Assert.True(image1.IsDisposed);

                // 参照されなくなった MemoryImageMediaItem も破棄される
                Assert.True(mediaItem.IsDisposed);
            }
        }

        [Fact]
        public void ImageServiceChange_Test()
        {
            using (var mediaSelector = new MediaSelector { Visible = false, Enabled = false })
            {
                mediaSelector.Initialize(new Twitter(), TwitterConfiguration.DefaultConfiguration(), "Twitter");

                Assert.Equal("Twitter", mediaSelector.ServiceName);

                mediaSelector.BeginSelection(new[] { "Resources/re.gif", "Resources/re1.png" });

                // 3 ページ目まで選択可能な状態
                var pages = mediaSelector.ImagePageCombo.Items;
                Assert.Equal(new[] { "1", "2", "3" }, pages.Cast<object>().Select(x => x.ToString()));
                Assert.True(mediaSelector.ImagePageCombo.Enabled);

                var eventCalled = false;
                mediaSelector.SelectedServiceChanged += (o, e) => eventCalled = true;

                // 投稿先を yfrog に変更
                mediaSelector.ImageServiceCombo.SelectedIndex =
                    mediaSelector.ImageServiceCombo.Items.IndexOf("yfrog");

                Assert.True(eventCalled); // SelectedServiceChanged イベントが呼ばれる

                // 1 ページ目のみ選択可能な状態 (Disabled)
                pages = mediaSelector.ImagePageCombo.Items;
                Assert.Equal(new[] { "1" }, pages.Cast<object>().Select(x => x.ToString()));
                Assert.False(mediaSelector.ImagePageCombo.Enabled);

                // 投稿先を Twitter に変更
                mediaSelector.ImageServiceCombo.SelectedIndex =
                    mediaSelector.ImageServiceCombo.Items.IndexOf("Twitter");

                // 2 ページ目まで選択可能な状態
                pages = mediaSelector.ImagePageCombo.Items;
                Assert.Equal(new[] { "1", "2" }, pages.Cast<object>().Select(x => x.ToString()));
                Assert.True(mediaSelector.ImagePageCombo.Enabled);
            }
        }
    }
}
