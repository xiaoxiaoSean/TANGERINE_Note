using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 橘子记事本
{
    public partial class SRControl : UserControl
    {
        public SRControl()
        {
            InitializeComponent();
        }

        private void SRControl_Load(object sender, EventArgs e)
        {
            searchBox.Watermark = "搜索...";
            replaceBox.Watermark = "替换为...";
        }
        private void searchBox_TextChanged(object sender, EventArgs e)
        {


        }
    }
}
