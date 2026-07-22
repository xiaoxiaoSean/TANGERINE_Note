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
            tableLayoutPanel1 = new TableLayoutPanel();
            label0 = new Label();
            label1 = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            checkBox1_2 = new CheckBox();
            checkBox1_1 = new CheckBox();
            label2 = new Label();
            tableLayoutPanel3 = new TableLayoutPanel();
            checkBox2_2 = new CheckBox();
            checkBox2_1 = new CheckBox();
            dateTimePicker1 = new DateTimePicker();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(label0, 0, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 1, 2);
            tableLayoutPanel1.Controls.Add(dateTimePicker1, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 48.57143F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 51.42857F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 105F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 116F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 135F));
            tableLayoutPanel1.Size = new Size(1163, 603);
            tableLayoutPanel1.TabIndex = 0;
            tableLayoutPanel1.Paint += tableLayoutPanel1_Paint;
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
            label2.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label2.Location = new Point(3, 246);
            label2.Name = "label2";
            label2.Size = new Size(164, 46);
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
            // dateTimePicker1
            // 
            dateTimePicker1.Dock = DockStyle.Fill;
            dateTimePicker1.Location = new Point(584, 3);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(576, 30);
            dateTimePicker1.TabIndex = 5;
            // 
            // NoticeSettingForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1163, 603);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "NoticeSettingForm";
            Text = "设置提醒";
            Load += NoticeSettingForm_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
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
    }
}