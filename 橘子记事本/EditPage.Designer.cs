namespace 橘子记事本
{
    partial class EditPage
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
            titleEditBox = new TextBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            editNoteBox = new SyncRichTextBox();
            lineNumBox = new SyncRichTextBox();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(titleEditBox, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.Size = new Size(1260, 622);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // titleEditBox
            // 
            titleEditBox.Dock = DockStyle.Fill;
            titleEditBox.Font = new Font("Microsoft Sans Serif", 20.1428585F);
            titleEditBox.Location = new Point(3, 3);
            titleEditBox.Multiline = true;
            titleEditBox.Name = "titleEditBox";
            titleEditBox.ScrollBars = ScrollBars.Both;
            titleEditBox.Size = new Size(1254, 118);
            titleEditBox.TabIndex = 0;
            titleEditBox.WordWrap = false;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.37958527F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 93.6204147F));
            tableLayoutPanel2.Controls.Add(editNoteBox, 1, 0);
            tableLayoutPanel2.Controls.Add(lineNumBox, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 127);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(1254, 492);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // editNoteBox
            // 
            editNoteBox.Dock = DockStyle.Fill;
            editNoteBox.Location = new Point(83, 3);
            editNoteBox.Name = "editNoteBox";
            editNoteBox.Size = new Size(1168, 486);
            editNoteBox.TabIndex = 1;
            editNoteBox.Text = "";
            editNoteBox.WordWrap = false;
            editNoteBox.TextChanged += editNoteBox_TextChanged;
            // 
            // lineNumBox
            // 
            lineNumBox.Dock = DockStyle.Fill;
            lineNumBox.Location = new Point(3, 3);
            lineNumBox.Name = "lineNumBox";
            lineNumBox.ScrollBars = RichTextBoxScrollBars.None;
            lineNumBox.Size = new Size(74, 486);
            lineNumBox.TabIndex = 2;
            lineNumBox.Text = "";
            // 
            // EditPage
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "EditPage";
            Size = new Size(1260, 622);
            Load += EditPage_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TextBox titleEditBox;
        private TableLayoutPanel tableLayoutPanel2;
        private SyncRichTextBox editNoteBox;
        private SyncRichTextBox lineNumBox;
    }
}
