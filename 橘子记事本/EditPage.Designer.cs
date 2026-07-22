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
            editNoteBox = new TextBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(titleEditBox, 0, 0);
            tableLayoutPanel1.Controls.Add(editNoteBox, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.Size = new Size(1489, 726);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // titleEditBox
            // 
            titleEditBox.Dock = DockStyle.Fill;
            // 使用通用无衬线字体以避免在缺少特定系统字体时抛出 GDI+ 错误，保留字号不变
            titleEditBox.Font = new Font(FontFamily.GenericSansSerif, 20.1428585F, FontStyle.Regular, GraphicsUnit.Point);
            titleEditBox.Location = new Point(3, 3);
            titleEditBox.Multiline = true;
            titleEditBox.Name = "titleEditBox";
            titleEditBox.ScrollBars = ScrollBars.Both;
            titleEditBox.Size = new Size(1483, 139);
            titleEditBox.TabIndex = 0;
            // 
            // editNoteBox
            // 
            editNoteBox.Dock = DockStyle.Fill;
            editNoteBox.Location = new Point(3, 148);
            editNoteBox.Multiline = true;
            editNoteBox.Name = "editNoteBox";
            editNoteBox.ScrollBars = ScrollBars.Both;
            editNoteBox.Size = new Size(1483, 575);
            editNoteBox.TabIndex = 1;
            // 
            // EditPage
            // 
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "EditPage";
            Size = new Size(1489, 726);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TextBox titleEditBox;
        private TextBox editNoteBox;
    }
}
