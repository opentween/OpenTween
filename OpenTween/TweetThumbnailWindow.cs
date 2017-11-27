using System;
using System.Drawing;
using System.Windows.Forms;
#nullable enable
namespace OpenTween
{
    public partial class TweetThumbnailWindow : Form
    {
        private Point mouseDownPoint;
        private bool _moved;

        public Image? Image
        {
            set => pictureBox1.Image = value;
            get => pictureBox1.Image;
        }

        public TweetThumbnailWindow(Form tweenMain)
        {
            InitializeComponent();
            tweenMain.Activated += TweenMain_Activated;
        }

        public void HideAndClear()
        {
            this.Hide();
            Image = null;
        }

        private void TweetThumbnailWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.HideAndClear();
        }

        private void TweenMain_Activated(object sender, EventArgs e)
        {
            /// <see cref="TweetThumbnail.ShowThubWindow(Image, ThumbnailInfo)"/> で確認できるように Image を残しておく
            this.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!_moved) this.HideAndClear();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left))
            {
                _moved = false;
                mouseDownPoint = e.Location;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left))
            {
                _moved = true;
                this.Location = new Point(this.Location.X + e.X - mouseDownPoint.X, this.Location.Y + e.Y - mouseDownPoint.Y);
            }
        }
    }
}
