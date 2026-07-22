namespace 橘子记事本
{
    partial class NoticeSettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NoticeSettingForm));
            mainPanel = new TableLayoutPanel();
            label0 = new Label();
            label1 = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            checkBox1_2 = new CheckBox();
            checkBox1_1 = new CheckBox();
            label2 = new Label();
            tableLayoutPanel3 = new TableLayoutPanel();
            checkBox2_2 = new CheckBox();
            checkBox2_1 = new CheckBox();
            dateTimePanel = new TableLayoutPanel();
            dateTimePicker1 = new DateTimePicker();
            hmsLayout = new TableLayoutPanel();
            hourNumeric = new NumericUpDown();
            minuteNumeric = new NumericUpDown();
            secNumeric = new NumericUpDown();
            mainPanel.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            dateTimePanel.SuspendLayout();
            hmsLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)hourNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)minuteNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)secNumeric).BeginInit();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.ColumnCount = 2;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainPanel.Controls.Add(label0, 0, 0);
            mainPanel.Controls.Add(label1, 0, 1);
            mainPanel.Controls.Add(tableLayoutPanel2, 1, 1);
            mainPanel.Controls.Add(label2, 0, 2);
            mainPanel.Controls.Add(tableLayoutPanel3, 1, 2);
            mainPanel.Controls.Add(dateTimePanel, 1, 0);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 5;
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 48.57143F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 51.42857F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 105F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 116F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 135F));
            mainPanel.Size = new Size(1163, 603);
            mainPanel.TabIndex = 0;
            mainPanel.Paint += tableLayoutPanel1_Paint;
            // 
            // label0
            // 
            label0.AutoSize = true;
            label0.Dock = DockStyle.Fill;
            label0.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label0.Location = new Point(3, 0);
            label0.Name = "label0";
            label0.Size = new Size(575, 119);
            label0.TabIndex = 0;
            label0.Text = "欢迎来到提醒设置界面";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label1.Location = new Point(3, 119);
            label1.Name = "label1";
            label1.Size = new Size(575, 127);
            label1.TabIndex = 1;
            label1.Text = "时间计量方式";
            label1.MouseClick += label1_MouseClick;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(checkBox1_2, 0, 1);
            tableLayoutPanel2.Controls.Add(checkBox1_1, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(584, 122);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(576, 121);
            tableLayoutPanel2.TabIndex = 2;
            // 
            // checkBox1_2
            // 
            checkBox1_2.AutoSize = true;
            checkBox1_2.Dock = DockStyle.Fill;
            checkBox1_2.Location = new Point(3, 63);
            checkBox1_2.Name = "checkBox1_2";
            checkBox1_2.Size = new Size(570, 55);
            checkBox1_2.TabIndex = 1;
            checkBox1_2.Text = "倒计时";
            checkBox1_2.UseVisualStyleBackColor = true;
            checkBox1_2.CheckStateChanged += checkBox1_2_CheckStateChanged;
            // 
            // checkBox1_1
            // 
            checkBox1_1.AutoSize = true;
            checkBox1_1.Dock = DockStyle.Fill;
            checkBox1_1.Location = new Point(3, 3);
            checkBox1_1.Name = "checkBox1_1";
            checkBox1_1.Size = new Size(570, 54);
            checkBox1_1.TabIndex = 0;
            checkBox1_1.Text = "绝对时间";
            checkBox1_1.UseVisualStyleBackColor = true;
            checkBox1_1.CheckStateChanged += checkBox1_1_CheckStateChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label2.Location = new Point(3, 246);
            label2.Name = "label2";
            label2.Size = new Size(575, 105);
            label2.TabIndex = 3;
            label2.Text = "提醒方式";
            label2.Click += label2_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Controls.Add(checkBox2_2, 0, 1);
            tableLayoutPanel3.Controls.Add(checkBox2_1, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(584, 249);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(576, 99);
            tableLayoutPanel3.TabIndex = 4;
            // 
            // checkBox2_2
            // 
            checkBox2_2.AutoSize = true;
            checkBox2_2.Dock = DockStyle.Fill;
            checkBox2_2.Location = new Point(3, 52);
            checkBox2_2.Name = "checkBox2_2";
            checkBox2_2.Size = new Size(570, 44);
            checkBox2_2.TabIndex = 1;
            checkBox2_2.Text = "T提醒";
            checkBox2_2.UseVisualStyleBackColor = true;
            checkBox2_2.CheckedChanged += checkBox2_2_CheckedChanged;
            // 
            // checkBox2_1
            // 
            checkBox2_1.AutoSize = true;
            checkBox2_1.Dock = DockStyle.Fill;
            checkBox2_1.Location = new Point(3, 3);
            checkBox2_1.Name = "checkBox2_1";
            checkBox2_1.Size = new Size(570, 43);
            checkBox2_1.TabIndex = 0;
            checkBox2_1.Text = "Windows通知";
            checkBox2_1.UseVisualStyleBackColor = true;
            checkBox2_1.CheckStateChanged += checkBox2_1_CheckStateChanged;
            // 
            // dateTimePanel
            // 
            dateTimePanel.ColumnCount = 1;
            dateTimePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            dateTimePanel.Controls.Add(dateTimePicker1, 0, 0);
            dateTimePanel.Controls.Add(hmsLayout, 0, 1);
            dateTimePanel.Dock = DockStyle.Fill;
            dateTimePanel.Location = new Point(584, 3);
            dateTimePanel.Name = "dateTimePanel";
            dateTimePanel.RowCount = 2;
            dateTimePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            dateTimePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            dateTimePanel.Size = new Size(576, 113);
            dateTimePanel.TabIndex = 6;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Dock = DockStyle.Fill;
            dateTimePicker1.Location = new Point(3, 3);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(570, 30);
            dateTimePicker1.TabIndex = 5;
            // 
            // hmsLayout
            // 
            hmsLayout.ColumnCount = 3;
            hmsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45.8221F));
            hmsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54.1779F));
            hmsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 198F));
            hmsLayout.Controls.Add(hourNumeric, 0, 0);
            hmsLayout.Controls.Add(minuteNumeric, 1, 0);
            hmsLayout.Controls.Add(secNumeric, 2, 0);
            hmsLayout.Dock = DockStyle.Fill;
            hmsLayout.Location = new Point(3, 59);
            hmsLayout.Name = "hmsLayout";
            hmsLayout.RowCount = 1;
            hmsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            hmsLayout.Size = new Size(570, 51);
            hmsLayout.TabIndex = 6;
            // 
            // hourNumeric
            // 
            hourNumeric.Dock = DockStyle.Fill;
            hourNumeric.Location = new Point(3, 3);
            hourNumeric.Name = "hourNumeric";
            hourNumeric.Size = new Size(164, 30);
            hourNumeric.TabIndex = 0;
            // 
            // minuteNumeric
            // 
            minuteNumeric.Dock = DockStyle.Fill;
            minuteNumeric.Location = new Point(173, 3);
            minuteNumeric.Name = "minuteNumeric";
            minuteNumeric.Size = new Size(195, 30);
            minuteNumeric.TabIndex = 1;
            // 
            // secNumeric
            // 
            secNumeric.Dock = DockStyle.Fill;
            secNumeric.Location = new Point(374, 3);
            secNumeric.Name = "secNumeric";
            secNumeric.Size = new Size(193, 30);
            secNumeric.TabIndex = 2;
            // 
            // NoticeSettingForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1163, 603);
            Controls.Add(mainPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "NoticeSettingForm";
            Text = "设置提醒";
            Load += NoticeSettingForm_Load;
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            dateTimePanel.ResumeLayout(false);
            hmsLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)hourNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)minuteNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)secNumeric).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainPanel;
        private Label label0;
        private Label label1;
        private TableLayoutPanel tableLayoutPanel2;
        private CheckBox checkBox1_1;
        private CheckBox checkBox1_2;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel3;
        private CheckBox checkBox2_2;
        private CheckBox checkBox2_1;
        private DateTimePicker dateTimePicker1;
        private TableLayoutPanel dateTimePanel;
        private TableLayoutPanel hmsLayout;
        private NumericUpDown hourNumeric;
        private NumericUpDown minuteNumeric;
        private NumericUpDown secNumeric;
    }
}