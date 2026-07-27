namespace 橘子记事本
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            mainPanel = new TableLayoutPanel();
            mainTab = new TabControl();
            HomePage = new TabPage();
            tWritePage = new TabPage();
            tNoticePage = new TabPage();
            settingPage = new TabPage();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            pwdNPBox = new Sunny.UI.UITextBox();
            welcomePWD = new Label();
            scrollingPwdText1 = new Sunny.UI.UIScrollingText();
            pwdOPBox = new Sunny.UI.UITextBox();
            changePwdButton = new Button();
            uiSwitch1 = new Sunny.UI.UISwitch();
            uiListBox1 = new ListBox();
            topPanel = new TableLayoutPanel();
            logoBox = new PictureBox();
            label1 = new Label();
            operationPanel = new TableLayoutPanel();
            oprationBox1 = new PictureBox();
            oprationBox2 = new PictureBox();
            oprationBox4 = new PictureBox();
            oprationBox3 = new PictureBox();
            mainPanel.SuspendLayout();
            mainTab.SuspendLayout();
            settingPage.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logoBox).BeginInit();
            operationPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)oprationBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)oprationBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)oprationBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)oprationBox3).BeginInit();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 17F));
            mainPanel.Controls.Add(mainTab, 0, 1);
            mainPanel.Controls.Add(topPanel, 0, 0);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 2;
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            mainPanel.Size = new Size(1378, 788);
            mainPanel.TabIndex = 0;
            // 
            // mainTab
            // 
            mainTab.Controls.Add(HomePage);
            mainTab.Controls.Add(tWritePage);
            mainTab.Controls.Add(tNoticePage);
            mainTab.Controls.Add(settingPage);
            mainTab.Dock = DockStyle.Fill;
            mainTab.Location = new Point(3, 160);
            mainTab.Name = "mainTab";
            mainTab.SelectedIndex = 0;
            mainTab.Size = new Size(1372, 625);
            mainTab.TabIndex = 0;
            mainTab.SelectedIndexChanged += mainTab_SelectedIndexChanged;
            // 
            // HomePage
            // 
            HomePage.BackColor = SystemColors.GradientActiveCaption;
            HomePage.Location = new Point(4, 33);
            HomePage.Name = "HomePage";
            HomePage.Padding = new Padding(3);
            HomePage.Size = new Size(1364, 588);
            HomePage.TabIndex = 0;
            HomePage.Text = "主页";
            // 
            // tWritePage
            // 
            tWritePage.AutoScroll = true;
            tWritePage.Location = new Point(4, 33);
            tWritePage.Name = "tWritePage";
            tWritePage.Padding = new Padding(3);
            tWritePage.Size = new Size(1364, 588);
            tWritePage.TabIndex = 1;
            tWritePage.Text = "笔记";
            tWritePage.UseVisualStyleBackColor = true;
            // 
            // tNoticePage
            // 
            tNoticePage.Location = new Point(4, 33);
            tNoticePage.Name = "tNoticePage";
            tNoticePage.Padding = new Padding(3);
            tNoticePage.Size = new Size(1364, 588);
            tNoticePage.TabIndex = 2;
            tNoticePage.Text = "提醒";
            tNoticePage.UseVisualStyleBackColor = true;
            // 
            // settingPage
            // 
            settingPage.Controls.Add(tableLayoutPanel1);
            settingPage.Location = new Point(4, 33);
            settingPage.Name = "settingPage";
            settingPage.Padding = new Padding(3);
            settingPage.Size = new Size(1364, 588);
            settingPage.TabIndex = 3;
            settingPage.Text = "设置";
            settingPage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13.1811485F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 86.81885F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 0);
            tableLayoutPanel1.Controls.Add(uiListBox1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(1358, 582);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 71.8670044F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.1329918F));
            tableLayoutPanel2.Controls.Add(pwdNPBox, 0, 2);
            tableLayoutPanel2.Controls.Add(welcomePWD, 0, 0);
            tableLayoutPanel2.Controls.Add(scrollingPwdText1, 1, 0);
            tableLayoutPanel2.Controls.Add(pwdOPBox, 0, 1);
            tableLayoutPanel2.Controls.Add(changePwdButton, 0, 3);
            tableLayoutPanel2.Controls.Add(uiSwitch1, 1, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(182, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 5;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 45.9459457F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 54.0540543F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 136F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 122F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
            tableLayoutPanel2.Size = new Size(1173, 576);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // pwdNPBox
            // 
            pwdNPBox.Dock = DockStyle.Fill;
            pwdNPBox.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            pwdNPBox.Location = new Point(4, 227);
            pwdNPBox.Margin = new Padding(4, 5, 4, 5);
            pwdNPBox.MaxLength = 50;
            pwdNPBox.MinimumSize = new Size(1, 16);
            pwdNPBox.Name = "pwdNPBox";
            pwdNPBox.Padding = new Padding(5);
            pwdNPBox.ShowText = false;
            pwdNPBox.Size = new Size(835, 126);
            pwdNPBox.TabIndex = 3;
            pwdNPBox.TextAlignment = ContentAlignment.MiddleLeft;
            pwdNPBox.Watermark = "输入新密码(若要不设密码，就留空，只能写英文字母和数字，最多50位)";
            // 
            // welcomePWD
            // 
            welcomePWD.AutoSize = true;
            welcomePWD.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            welcomePWD.Location = new Point(3, 0);
            welcomePWD.Name = "welcomePWD";
            welcomePWD.Size = new Size(398, 102);
            welcomePWD.TabIndex = 0;
            welcomePWD.Text = "欢迎来到加密设置界面\r\n重要保密数据请用你信任的软件存储\r\n若要更改加密密码，请在下方操作\r\n\r\n";
            // 
            // scrollingPwdText1
            // 
            scrollingPwdText1.Active = true;
            scrollingPwdText1.Dock = DockStyle.Fill;
            scrollingPwdText1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            scrollingPwdText1.Interval = 100;
            scrollingPwdText1.Location = new Point(846, 3);
            scrollingPwdText1.MinimumSize = new Size(1, 1);
            scrollingPwdText1.Name = "scrollingPwdText1";
            scrollingPwdText1.Radius = 90;
            scrollingPwdText1.Size = new Size(324, 96);
            scrollingPwdText1.TabIndex = 1;
            scrollingPwdText1.Text = "免责声明：数据如果泄露，与橘子记事本及其开发者和贡献者无关。  数据安全不可忽视，请用你信任的软件存储你的保密数据，数据如果泄露，与橘子记事本及其开发者和贡献者无关";
            // 
            // pwdOPBox
            // 
            pwdOPBox.Dock = DockStyle.Fill;
            pwdOPBox.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            pwdOPBox.Location = new Point(4, 107);
            pwdOPBox.Margin = new Padding(4, 5, 4, 5);
            pwdOPBox.MaxLength = 50;
            pwdOPBox.MinimumSize = new Size(1, 16);
            pwdOPBox.Name = "pwdOPBox";
            pwdOPBox.Padding = new Padding(5);
            pwdOPBox.ShowText = false;
            pwdOPBox.Size = new Size(835, 110);
            pwdOPBox.TabIndex = 2;
            pwdOPBox.TextAlignment = ContentAlignment.MiddleLeft;
            pwdOPBox.Watermark = "输入旧密码(没设密码就留空)";
            // 
            // changePwdButton
            // 
            changePwdButton.Dock = DockStyle.Fill;
            changePwdButton.Location = new Point(3, 361);
            changePwdButton.Name = "changePwdButton";
            changePwdButton.Size = new Size(837, 116);
            changePwdButton.TabIndex = 4;
            changePwdButton.Text = "确认更改密码";
            changePwdButton.UseVisualStyleBackColor = true;
            changePwdButton.Click += changePwdButton_Click;
            // 
            // uiSwitch1
            // 
            uiSwitch1.Dock = DockStyle.Fill;
            uiSwitch1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSwitch1.Location = new Point(846, 105);
            uiSwitch1.MinimumSize = new Size(1, 1);
            uiSwitch1.Name = "uiSwitch1";
            uiSwitch1.Size = new Size(324, 114);
            uiSwitch1.TabIndex = 5;
            uiSwitch1.Text = "uiSwitch1";
            // 
            // uiListBox1
            // 
            uiListBox1.Dock = DockStyle.Fill;
            uiListBox1.FormattingEnabled = true;
            uiListBox1.Items.AddRange(new object[] { "加密", "开发者选项" });
            uiListBox1.Location = new Point(3, 3);
            uiListBox1.Name = "uiListBox1";
            uiListBox1.Size = new Size(173, 576);
            uiListBox1.TabIndex = 2;
            uiListBox1.Click += uiListBox1_Click;
            // 
            // topPanel
            // 
            topPanel.ColumnCount = 3;
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2921171F));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42.5400734F));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43.21825F));
            topPanel.Controls.Add(logoBox, 0, 0);
            topPanel.Controls.Add(label1, 1, 0);
            topPanel.Controls.Add(operationPanel, 2, 0);
            topPanel.Dock = DockStyle.Fill;
            topPanel.Location = new Point(3, 3);
            topPanel.Name = "topPanel";
            topPanel.RowCount = 1;
            topPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            topPanel.Size = new Size(1372, 151);
            topPanel.TabIndex = 1;
            // 
            // logoBox
            // 
            logoBox.Dock = DockStyle.Fill;
            logoBox.Image = Properties.Resources.logo;
            logoBox.Location = new Point(3, 3);
            logoBox.Name = "logoBox";
            logoBox.Size = new Size(189, 145);
            logoBox.SizeMode = PictureBoxSizeMode.Zoom;
            logoBox.TabIndex = 0;
            logoBox.TabStop = false;
            logoBox.DoubleClick += logoBox_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Microsoft YaHei UI", 48F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label1.Location = new Point(198, 0);
            label1.Name = "label1";
            label1.Size = new Size(577, 151);
            label1.TabIndex = 1;
            label1.Text = "橘子记事本";
            // 
            // operationPanel
            // 
            operationPanel.ColumnCount = 4;
            operationPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48.1203F));
            operationPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 51.8797F));
            operationPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 143F));
            operationPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
            operationPanel.Controls.Add(oprationBox1, 0, 0);
            operationPanel.Controls.Add(oprationBox2, 1, 0);
            operationPanel.Controls.Add(oprationBox4, 3, 0);
            operationPanel.Controls.Add(oprationBox3, 2, 0);
            operationPanel.Dock = DockStyle.Fill;
            operationPanel.Location = new Point(781, 3);
            operationPanel.Name = "operationPanel";
            operationPanel.RowCount = 1;
            operationPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            operationPanel.Size = new Size(588, 145);
            operationPanel.TabIndex = 2;
            // 
            // oprationBox1
            // 
            oprationBox1.Dock = DockStyle.Fill;
            oprationBox1.Image = Properties.Resources.CreateNotes_icon;
            oprationBox1.Location = new Point(3, 3);
            oprationBox1.Name = "oprationBox1";
            oprationBox1.Size = new Size(144, 139);
            oprationBox1.SizeMode = PictureBoxSizeMode.Zoom;
            oprationBox1.TabIndex = 0;
            oprationBox1.TabStop = false;
            oprationBox1.Click += oprationBox1_Click;
            oprationBox1.MouseEnter += oprationBox1_MouseEnter;
            oprationBox1.MouseLeave += oprationBox1_MouseLeave;
            // 
            // oprationBox2
            // 
            oprationBox2.Dock = DockStyle.Fill;
            oprationBox2.Image = Properties.Resources.DeleteNotes_icon;
            oprationBox2.Location = new Point(153, 3);
            oprationBox2.Name = "oprationBox2";
            oprationBox2.Size = new Size(155, 139);
            oprationBox2.SizeMode = PictureBoxSizeMode.Zoom;
            oprationBox2.TabIndex = 1;
            oprationBox2.TabStop = false;
            oprationBox2.Click += oprationBox2_Click;
            oprationBox2.MouseEnter += oprationBox2_MouseEnter;
            oprationBox2.MouseLeave += oprationBox2_MouseLeave;
            // 
            // oprationBox4
            // 
            oprationBox4.Location = new Point(457, 3);
            oprationBox4.Name = "oprationBox4";
            oprationBox4.Size = new Size(128, 139);
            oprationBox4.TabIndex = 3;
            oprationBox4.TabStop = false;
            // 
            // oprationBox3
            // 
            oprationBox3.Location = new Point(314, 3);
            oprationBox3.Name = "oprationBox3";
            oprationBox3.Size = new Size(137, 139);
            oprationBox3.TabIndex = 2;
            oprationBox3.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1378, 788);
            Controls.Add(mainPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "橘子记事本";
            Load += Form1_Load;
            SizeChanged += Form1_SizeChanged;
            mainPanel.ResumeLayout(false);
            mainTab.ResumeLayout(false);
            settingPage.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logoBox).EndInit();
            operationPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)oprationBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)oprationBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)oprationBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)oprationBox3).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainPanel;
        private TabControl mainTab;
        private TabPage HomePage;
        private TabPage tWritePage;
        private TabPage tNoticePage;
        private TableLayoutPanel topPanel;
        private PictureBox logoBox;
        private Label label1;
        private TableLayoutPanel operationPanel;
        private PictureBox oprationBox1;
        private PictureBox oprationBox2;
        private PictureBox oprationBox4;
        private PictureBox oprationBox3;
        private TabPage settingPage;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label welcomePWD;
        private Sunny.UI.UIScrollingText scrollingPwdText1;
        private Sunny.UI.UITextBox pwdOPBox;
        private Sunny.UI.UITextBox pwdNPBox;
        private Button changePwdButton;
        private ListBox uiListBox1;
        private Sunny.UI.UISwitch uiSwitch1;
    }
}
