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
            pwdNPBox = new UITextBox();
            welcomePWD = new Label();
            pwdOPBox = new UITextBox();
            changePwdButton = new Button();
            isSoundNoticeButton = new Button();
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
            resources.ApplyResources(mainPanel, "mainPanel");
            mainPanel.Controls.Add(mainTab, 0, 1);
            mainPanel.Controls.Add(topPanel, 0, 0);
            mainPanel.Name = "mainPanel";
            // 
            // mainTab
            // 
            resources.ApplyResources(mainTab, "mainTab");
            mainTab.Controls.Add(HomePage);
            mainTab.Controls.Add(tWritePage);
            mainTab.Controls.Add(tNoticePage);
            mainTab.Controls.Add(settingPage);
            mainTab.Name = "mainTab";
            mainTab.SelectedIndex = 0;
            mainTab.SelectedIndexChanged += mainTab_SelectedIndexChanged;
            // 
            // HomePage
            // 
            resources.ApplyResources(HomePage, "HomePage");
            HomePage.BackColor = SystemColors.GradientActiveCaption;
            HomePage.Name = "HomePage";
            // 
            // tWritePage
            // 
            resources.ApplyResources(tWritePage, "tWritePage");
            tWritePage.Name = "tWritePage";
            tWritePage.UseVisualStyleBackColor = true;
            // 
            // tNoticePage
            // 
            resources.ApplyResources(tNoticePage, "tNoticePage");
            tNoticePage.Name = "tNoticePage";
            tNoticePage.UseVisualStyleBackColor = true;
            // 
            // settingPage
            // 
            resources.ApplyResources(settingPage, "settingPage");
            settingPage.Controls.Add(tableLayoutPanel1);
            settingPage.Name = "settingPage";
            settingPage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 0);
            tableLayoutPanel1.Controls.Add(uiListBox1, 0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
            tableLayoutPanel2.Controls.Add(pwdNPBox, 0, 2);
            tableLayoutPanel2.Controls.Add(welcomePWD, 0, 0);
            tableLayoutPanel2.Controls.Add(pwdOPBox, 0, 1);
            tableLayoutPanel2.Controls.Add(changePwdButton, 0, 3);
            tableLayoutPanel2.Controls.Add(isSoundNoticeButton, 1, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // pwdNPBox
            // 
            resources.ApplyResources(pwdNPBox, "pwdNPBox");
            pwdNPBox.BorderStyle = BorderStyle.FixedSingle;
            pwdNPBox.Name = "pwdNPBox";
            // 
            // welcomePWD
            // 
            resources.ApplyResources(welcomePWD, "welcomePWD");
            welcomePWD.Name = "welcomePWD";
            // 
            // pwdOPBox
            // 
            resources.ApplyResources(pwdOPBox, "pwdOPBox");
            pwdOPBox.BorderStyle = BorderStyle.FixedSingle;
            pwdOPBox.Name = "pwdOPBox";
            // 
            // changePwdButton
            // 
            resources.ApplyResources(changePwdButton, "changePwdButton");
            changePwdButton.Name = "changePwdButton";
            changePwdButton.UseVisualStyleBackColor = true;
            changePwdButton.Click += changePwdButton_Click;
            // 
            // isSoundNoticeButton
            // 
            resources.ApplyResources(isSoundNoticeButton, "isSoundNoticeButton");
            isSoundNoticeButton.Name = "isSoundNoticeButton";
            isSoundNoticeButton.UseVisualStyleBackColor = true;
            isSoundNoticeButton.Click += isSoundNoticeButton_Click;
            // 
            // uiListBox1
            // 
            resources.ApplyResources(uiListBox1, "uiListBox1");
            uiListBox1.FormattingEnabled = true;
            uiListBox1.Items.AddRange(new object[] { resources.GetString("uiListBox1.Items"), resources.GetString("uiListBox1.Items1"), resources.GetString("uiListBox1.Items2") });
            uiListBox1.Name = "uiListBox1";
            uiListBox1.Click += uiListBox1_Click;
            // 
            // topPanel
            // 
            resources.ApplyResources(topPanel, "topPanel");
            topPanel.Controls.Add(logoBox, 0, 0);
            topPanel.Controls.Add(label1, 1, 0);
            topPanel.Controls.Add(operationPanel, 2, 0);
            topPanel.Name = "topPanel";
            // 
            // logoBox
            // 
            resources.ApplyResources(logoBox, "logoBox");
            logoBox.Image = Properties.Resources.logo;
            logoBox.Name = "logoBox";
            logoBox.TabStop = false;
            logoBox.DoubleClick += logoBox_DoubleClick;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // operationPanel
            // 
            resources.ApplyResources(operationPanel, "operationPanel");
            operationPanel.Controls.Add(oprationBox1, 0, 0);
            operationPanel.Controls.Add(oprationBox2, 1, 0);
            operationPanel.Controls.Add(oprationBox4, 3, 0);
            operationPanel.Controls.Add(oprationBox3, 2, 0);
            operationPanel.Name = "operationPanel";
            // 
            // oprationBox1
            // 
            resources.ApplyResources(oprationBox1, "oprationBox1");
            oprationBox1.Image = Properties.Resources.CreateNotes_icon;
            oprationBox1.Name = "oprationBox1";
            oprationBox1.TabStop = false;
            oprationBox1.Click += oprationBox1_Click;
            oprationBox1.MouseEnter += oprationBox1_MouseEnter;
            oprationBox1.MouseLeave += oprationBox1_MouseLeave;
            // 
            // oprationBox2
            // 
            resources.ApplyResources(oprationBox2, "oprationBox2");
            oprationBox2.Image = Properties.Resources.DeleteNotes_icon;
            oprationBox2.Name = "oprationBox2";
            oprationBox2.TabStop = false;
            oprationBox2.Click += oprationBox2_Click;
            oprationBox2.MouseEnter += oprationBox2_MouseEnter;
            oprationBox2.MouseLeave += oprationBox2_MouseLeave;
            // 
            // oprationBox4
            // 
            resources.ApplyResources(oprationBox4, "oprationBox4");
            oprationBox4.Name = "oprationBox4";
            oprationBox4.TabStop = false;
            // 
            // oprationBox3
            // 
            resources.ApplyResources(oprationBox3, "oprationBox3");
            oprationBox3.Image = Properties.Resources.search_replace;
            oprationBox3.Name = "oprationBox3";
            oprationBox3.TabStop = false;
            oprationBox3.Click += oprationBox3_Click;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(mainPanel);
            Name = "MainForm";
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
        private UITextBox pwdOPBox;
        private UITextBox pwdNPBox;
        private Button changePwdButton;
        private ListBox uiListBox1;
        private Button isSoundNoticeButton;
    }
}
