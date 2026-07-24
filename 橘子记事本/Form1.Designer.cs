namespace 橘子记事本
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            mainPanel = new TableLayoutPanel();
            mainTab = new TabControl();
            HomePage = new TabPage();
            tWritePage = new TabPage();
            tNoticePage = new TabPage();
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1378, 788);
            Controls.Add(mainPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Load += Form1_Load;
            SizeChanged += Form1_SizeChanged;
            mainPanel.ResumeLayout(false);
            mainTab.ResumeLayout(false);
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
    }
}
