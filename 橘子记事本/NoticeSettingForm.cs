namespace 橘子记事本
{
    public partial class NoticeSettingForm : Form
    {
        int nid = -1;
        public twdata twr { get; private set; }
        public NoticeSettingForm(twdata twtw, int noticeId)
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

        private void checkBox2_1_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox2_1.Checked)
            {
                checkBox2_2.Checked = false;
            }
        }

        private void checkBox2_2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2_2.Checked)
            {
                checkBox2_1.Checked = false;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void NoticeSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 如果提醒已启用，进行严格验证；否则允许随意设定
            bool isEnabled = twr.isNoticeEnabled != null
                && nid < twr.isNoticeEnabled.Count
                && twr.isNoticeEnabled[nid];

            if (isEnabled)
            {
                // === 已启用：严格验证 ===
                if (checkBox1_1.Checked) // 绝对时间
                {
                    // 绝对时间不能早于当前时间
                    if (dateTimePicker1.Value < DateTime.Now)
                    {
                        MessageBox.Show(
                            "设置的提醒时间不能早于当前时间，请重新设置。",
                            "时间设置错误",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        e.Cancel = true;
                        return;
                    }
                    twr.taskNoticeType[nid] = 1;
                    twr.tasksNoticeTime[nid] = dateTimePicker1.Value;
                }
                else if (checkBox1_2.Checked) // 倒计时
                {
                    // 尝试构造 TimeSpan，捕获溢出异常
                    TimeSpan ts;
                    try
                    {
                        ts = new TimeSpan((int)hourNumeric.Value, (int)minuteNumeric.Value, (int)secNumeric.Value);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        MessageBox.Show(
                            "倒计时太长，请减小时间数值。",
                            "倒计时设置错误",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        e.Cancel = true;
                        return;
                    }

                    // 倒计时不能为 0
                    if (ts == TimeSpan.Zero)
                    {
                        MessageBox.Show(
                            "倒计时不能为 0，请重新设置。",
                            "倒计时设置错误",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        e.Cancel = true;
                        return;
                    }

                    twr.taskNoticeType[nid] = 2;
                    twr.taskNoticeTime2[nid] = ts;
                }
            }
            else
            {
                // === 未启用：允许随意设定，不验证 ===
                if (checkBox1_1.Checked)
                {
                    twr.taskNoticeType[nid] = 1;
                    twr.tasksNoticeTime[nid] = dateTimePicker1.Value;
                }
                else if (checkBox1_2.Checked)
                {
                    twr.taskNoticeType[nid] = 2;
                    try
                    {
                        twr.taskNoticeTime2[nid] = new TimeSpan((int)hourNumeric.Value, (int)minuteNumeric.Value, (int)secNumeric.Value);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // 未启用时不阻止，但设置一个安全默认值
                        twr.taskNoticeTime2[nid] = TimeSpan.FromMinutes(5);
                    }
                }
            }

            // 保存提醒方式：1=Windows通知, 2=屏幕右下角提醒
            if (checkBox2_1.Checked)
                twr.tasksNoticeMethod[nid] = 1;
            else if (checkBox2_2.Checked)
                twr.tasksNoticeMethod[nid] = 2;
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

            // 恢复提醒方式复选框状态
            checkBox2_1.Checked = false;
            checkBox2_2.Checked = false;
            if (twr.tasksNoticeMethod != null && nid < twr.tasksNoticeMethod.Count)
            {
                switch (twr.tasksNoticeMethod[nid])
                {
                    case 1:
                        checkBox2_1.Checked = true;
                        break;
                    case 2:
                        checkBox2_2.Checked = true;
                        break;
                }
            }

            FormClosing += NoticeSettingForm_FormClosing;
        }
        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // 右键：保存当前值并折叠时间选择器
                if (dateTimePicker1.Visible)
                {
                    twr.tasksNoticeTime[nid] = dateTimePicker1.Value;
                }
                if (hourNumeric.Visible && minuteNumeric.Visible && secNumeric.Visible)
                {
                    twr.taskNoticeTime2[nid] = new TimeSpan((int)hourNumeric.Value, (int)minuteNumeric.Value, (int)secNumeric.Value);
                }
                dateTimePicker1.Visible = false;
                hourNumeric.Visible = false;
                minuteNumeric.Visible = false;
                secNumeric.Visible = false;
            }
            else if (e.Button == MouseButtons.Left)
            {
                // 左键：切换时间选择器的显示/隐藏
                if (hourNumeric.Visible || minuteNumeric.Visible || secNumeric.Visible || dateTimePicker1.Visible)
                {
                    // 当前可见 → 全部隐藏
                    dateTimePicker1.Visible = false;
                    hourNumeric.Visible = false;
                    minuteNumeric.Visible = false;
                    secNumeric.Visible = false;
                }
                else
                {
                    // 当前隐藏 → 根据提醒类型显示对应控件
                    if (twr.taskNoticeType[nid] == 1)
                    {
                        dateTimePicker1.Visible = true;
                        dateTimePicker1.Value = twr.tasksNoticeTime[nid];
                        hourNumeric.Visible = false;
                        minuteNumeric.Visible = false;
                        secNumeric.Visible = false;
                    }
                    else if (twr.taskNoticeType[nid] == 2)
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
