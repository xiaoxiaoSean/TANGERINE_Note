namespace 橘子记事本
{
    partial class SRControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            replaceBox = new UITextBox();
            searchButton = new Button();
            replaceButton = new Button();
            searchBox = new UITextBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 78.96296F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21.0370369F));
            tableLayoutPanel1.Controls.Add(replaceBox, 0, 1);
            tableLayoutPanel1.Controls.Add(searchButton, 1, 0);
            tableLayoutPanel1.Controls.Add(replaceButton, 1, 1);
            tableLayoutPanel1.Controls.Add(searchBox, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(675, 254);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // replaceBox
            // 
            replaceBox.BorderStyle = BorderStyle.FixedSingle;
            replaceBox.Dock = DockStyle.Fill;
            replaceBox.Location = new Point(3, 130);
            replaceBox.Multiline = true;
            replaceBox.Name = "replaceBox";
            replaceBox.Size = new Size(526, 121);
            replaceBox.TabIndex = 3;
            // 
            // searchButton
            // 
            searchButton.Dock = DockStyle.Fill;
            searchButton.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            searchButton.Location = new Point(535, 3);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(137, 121);
            searchButton.TabIndex = 0;
            searchButton.Text = "搜索";
            searchButton.UseVisualStyleBackColor = true;
            // 
            // replaceButton
            // 
            replaceButton.Dock = DockStyle.Fill;
            replaceButton.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            replaceButton.Location = new Point(535, 130);
            replaceButton.Name = "replaceButton";
            replaceButton.Size = new Size(137, 121);
            replaceButton.TabIndex = 1;
            replaceButton.Text = "替换";
            replaceButton.UseVisualStyleBackColor = true;
            // 
            // searchBox
            // 
            searchBox.BorderStyle = BorderStyle.FixedSingle;
            searchBox.Dock = DockStyle.Fill;
            searchBox.Location = new Point(3, 3);
            searchBox.Multiline = true;
            searchBox.Name = "searchBox";
            searchBox.Size = new Size(526, 121);
            searchBox.TabIndex = 2;
            searchBox.TextChanged += searchBox_TextChanged;
            // 
            // SRControl
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "SRControl";
            Size = new Size(675, 254);
            Load += SRControl_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Button searchButton;
        private Button replaceButton;
        private UITextBox searchBox;
        private UITextBox replaceBox;
    }
}
