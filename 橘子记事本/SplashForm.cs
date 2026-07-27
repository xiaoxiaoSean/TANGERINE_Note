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
            await Task.Delay(500);
            await TypeText("提交个PR",50);
            await Task.Delay(300);
            await DeleteText(00);
            await TypeText("让这个软件更好吧",50);
            await Task.Delay(300);
            await DeleteText(10);

            await BlinkText("欢迎使用橘子记事本");
        }
        private async Task BlinkText(string text, int interval = 100,int wait=1000)
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
        private async Task TypeText(string text, int interval = 100)
        {
            label3.Text = "";

            foreach (char c in text)
            {
                label3.Text += c;
                await Task.Delay(interval);
            }
        }

        private async Task DeleteText(int interval = 100)
        {
            while (label3.Text.Length > 0)
            {
                label3.Text = label3.Text[..^1];
                await Task.Delay(interval);
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
