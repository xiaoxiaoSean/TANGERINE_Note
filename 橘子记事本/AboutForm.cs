namespace 橘子记事本
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            textBox1.Multiline = true;
            textBox1.Text = "橘子记事本\r\n版本号：1.0.5 \r\n通道:stable\r\n感谢你测试\r\nTANGERINE LAB";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }
    }
}
