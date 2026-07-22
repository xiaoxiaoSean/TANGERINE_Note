namespace 橘子记事本
{
    public partial class NoticeSettingForm : Form
    {
        int nid = -1;
        public twdatav1 twr { get; private set; }
        public NoticeSettingForm(twdatav1 twtw, int noticeId)
        {
            InitializeComponent();
            twr = twtw;
            nid = noticeId;
        }
        private void checkBox1_1_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox1_1.Checked)
            {
                twr.taskNoticeType[nid] = 1;
                if (hourNumeric.Visible || minuteNumeric.Visible || secNumeric.Visible)
                {
                    dateTimePicker1.Visible = true;
                    hourNumeric.Visible = false;
                    minuteNumeric.Visible = false;
                    secNumeric.Visible = false;
                }
                checkBox1_2.Checked = false;
            }
            else
            {
                checkBox1_2.Checked = true;

            }
        }

        private void checkBox1_2_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox1_2.Checked)
            {
                checkBox1_1.Checked = false;
                twr.taskNoticeType[nid] = 2;
                if (dateTimePicker1.Visible)
                {
                    dateTimePicker1.Visible = false;
                    hourNumeric.Visible = true;
                    minuteNumeric.Visible = true;
                    secNumeric.Visible = true;
                }
            }
            else
            {
                checkBox1_1.Checked = true;

            }
        }

        private void checkBox2_2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2_2.Checked)
            {
                checkBox2_2.Checked = false;
            }
        }

        private void checkBox2_1_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        void saveFile(object sender, EventArgs e)
        {
            if (checkBox1_1.Checked)
            {
                twr.taskNoticeType[nid] = 1;
            }
            if (checkBox1_2.Checked)
            {
                twr.taskNoticeType[nid] = 2;
            }
            twr.tasksNoticeTime[nid] = dateTimePicker1.Value;
        }
        private void NoticeSettingForm_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Visible = false;
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            hourNumeric.Visible = false;
            minuteNumeric.Visible = false;
            secNumeric.Visible = false;
            if (twr.tasksNoticeTime != null && twr.tasksNoticeTime[nid] != null)
            {
                dateTimePicker1.Value = twr.tasksNoticeTime[nid];
            }

            dateTimePicker1.ShowUpDown = true;
            checkBox1_1.Checked = false;
            checkBox1_2.Checked = false;
            if (twr.taskNoticeType != null && twr.taskNoticeType[nid] != null)
            {
                switch (twr.taskNoticeType[nid])
                {
                    default:
                        break;
                    case 1:
                        checkBox1_1.Checked = true;
                        break;
                    case 2:
                        checkBox1_2.Checked = true;
                        break;
                }
            }

            this.FormClosing += saveFile;
        }
        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && dateTimePicker1.Visible)
            {
                twr.tasksNoticeTime[nid] = dateTimePicker1.Value;
                hourNumeric.Visible = false;
                minuteNumeric.Visible = false;
                secNumeric.Visible = false;
                dateTimePicker1.Visible = false;
            }
            if (e.Button == MouseButtons.Right && hourNumeric.Visible && minuteNumeric.Visible && secNumeric.Visible)
            {
                twr.taskNoticeTime2[nid] = new TimeSpan((int)hourNumeric.Value, (int)minuteNumeric.Value, (int)secNumeric.Value);
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (hourNumeric.Visible || minuteNumeric.Visible || secNumeric.Visible)
                    {
                        hourNumeric.Visible = false;
                        minuteNumeric.Visible = false;
                        secNumeric.Visible = false;
                        return;
                    }
                    if (dateTimePicker1.Visible)
                    {
                        dateTimePicker1.Visible = false;
                    }
                    else
                    {
                        if (twr.taskNoticeType[nid] == 1)
                        {
                            dateTimePicker1.Visible = true;
                            dateTimePicker1.Value = twr.tasksNoticeTime[nid];
                            hourNumeric.Visible = false;
                            minuteNumeric.Visible = false;
                            secNumeric.Visible = false;
                        }
                        else
                        {
                            dateTimePicker1.Visible = false;
                            hourNumeric.Visible = true;
                            minuteNumeric.Visible = true;
                            secNumeric.Visible = true;
                            hourNumeric.Value = twr.taskNoticeTime2[nid].Hours;
                            minuteNumeric.Value = twr.taskNoticeTime2[nid].Minutes;
                            secNumeric.Value = twr.taskNoticeTime2[nid].Seconds;
                        }
                    }
                }
            }
        }
    }
}
