using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 橘子记事本
{
    public partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            InitializeComponent();
            // 订阅内部文本框的 TextChanged 事件，转发为对外的 SearchTextChanged 事件 (workbuddy-20260727)
            // 当搜索文本框文字改变时，通知外部（MainForm）开始执行搜索逻辑
            uiTextBox1.TextChanged += UiTextBox1_TextChanged;
        }

        /// <summary>
        /// 内部文本框文字改变时的处理：触发对外搜索事件 (workbuddy-20260727)
        /// </summary>
        private void UiTextBox1_TextChanged(object? sender, EventArgs e)
        {
            // 将内部文本框的改变转发为 SearchControl 的公开事件，供 MainForm 订阅
            SearchTextChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            uiTextBox1.Watermark = "在这里输入你想搜索什么，可以换行的";
        }

        /// <summary>
        /// 获取或设置搜索框的文本内容 (workbuddy-20260727)
        /// 外部可通过此属性读取用户输入的搜索词，也可在搜索时自动填入预设内容
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SearchText
        {
            get { return uiTextBox1.Text; }
            set { uiTextBox1.Text = value; }
        }

        string returnContent()
        {
            return uiTextBox1.Text;
        }

        public void reset()
        {
            uiTextBox1.Text = "";
        }

        /// <summary>
        /// 当搜索文本框文字改变时触发的事件 (workbuddy-20260727)
        /// MainForm 订阅此事件以启动异步搜索
        /// </summary>
        public event EventHandler? SearchTextChanged;
    }
}
