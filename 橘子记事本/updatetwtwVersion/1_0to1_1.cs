using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 橘子记事本.moveVersion
{
    public partial class _1_0to1_1 : Form
    {
        bool quietUpdate = false;
        twdata twtw=new twdata();
        public _1_0to1_1(bool isQuiet,string originJson)
        {
            InitializeComponent();
            quietUpdate = isQuiet;
        }
        public twdata returnTwdata()
        {
            return twtw;
        }
        private void _1_0to1_1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
