using System.Drawing.Drawing2D;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace 橘子记事本
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
        }
        private async void SplashForm_Load(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 1;
            this.UseWaitCursor = false;
            typeof(Control)
    .GetProperty("DoubleBuffered",
        System.Reflection.BindingFlags.Instance |
        System.Reflection.BindingFlags.NonPublic)
    ?.SetValue(this, true);
            GraphicsPath path = new();
            path.AddArc(0, 0, 20, 20, 180, 90);
            path.AddArc(Width - 20, 0, 20, 20, 270, 90);
            path.AddArc(Width - 20, Height - 20, 20, 20, 0, 90);
            path.AddArc(0, Height - 20, 20, 20, 90, 90);
            path.CloseFigure();
            this.Region = new Region(path);
            // 窗口最终位置和大小
            Point finalLocation = this.Location;
            Size finalSize = this.Size;

            // 初始大小（90%）
            Size startSize = new(
                (int)(finalSize.Width * 0.90),
                (int)(finalSize.Height * 0.90));

            // 最终中心
            Point center = new(
                finalLocation.X + finalSize.Width / 2,
                finalLocation.Y + finalSize.Height / 2);

            // 初始位置（下面40像素）
            Point startLocation = new(
                center.X - startSize.Width / 2,
                center.Y - startSize.Height / 2 + 40);

            // 初始化
            this.SuspendLayout();

            this.Opacity = 0;
            this.Size = startSize;
            this.Location = startLocation;

            this.ResumeLayout();

            // 动画参数
            const int frames = 35;

            for (int i = 0; i <= frames; i++)
            {
                double t = (double)i / frames;

                // EaseOutBack
                const double c1 = 1.70158;
                const double c3 = c1 + 1;

                double p =
                    1 +
                    c3 * Math.Pow(t - 1, 3) +
                    c1 * Math.Pow(t - 1, 2);

                // 透明度
                this.Opacity = p > 1 ? 1 : p;

                // 当前大小
                int w = startSize.Width + (int)((finalSize.Width - startSize.Width) * p);
                int h = startSize.Height + (int)((finalSize.Height - startSize.Height) * p);

                this.Size = new Size(w, h);

                // 保持中心不变
                this.Location = new Point(
                    center.X - w / 2,
                    center.Y - h / 2 + (int)((1 - p) * 40));

                await Task.Delay(16);
            }

            // 防止浮点误差
            this.Opacity = 1;
            this.Size = finalSize;
            this.Location = finalLocation;
            await BlinkText("欢迎使用橘子记事本");

        }
        private async Task BlinkText(string text, int interval = 100, int wait = 1000)
        {
            Random random = new();

            while (true)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    char[] chars = text.ToCharArray();
                    chars[i] = ' ';

                    label3.Text = new string(chars).Replace(" ", "   ");
                    await Task.Delay(interval);
                }

                label3.Text = text;
                await Task.Delay(wait);
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

     
    }
}
