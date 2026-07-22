using System.Text.Json;

namespace 橘子记事本
{
    public partial class Form1 : Form
    {
        static SplashForm splash = new SplashForm();
        Thread splashThread = new Thread(() =>
        {
            Application.Run(splash);
        });
        public Form1()
        {
            InitializeComponent();
            splashThread.Start();
            // 表单尺寸变化防抖：如果 200ms 内没有继续变化，则刷新笔记列表
            _sizeChangedTimer = new System.Windows.Forms.Timer();
            _sizeChangedTimer.Interval = 200;
            _sizeChangedTimer.Tick += SizeChangedTimer_Tick;
        }
        private System.Windows.Forms.Timer? _sizeChangedTimer;
        object tw = new object();
        //声明公用对象...
        TabPage editPage = new TabPage("编辑中的笔记");
        EditPage EdPage = new EditPage();
        Label welcomeLabel = new Label();
        Boolean isEditing = false;
        tWriteNotes[] notesToShow = Array.Empty<tWriteNotes>();
        Boolean isSelected = false;
        int selectedNoteId = -1;
        int selectedTab = 0;
        Boolean isNoticeStarted = false;
        List<CheckBox> noticeCheckBoxes = new List<CheckBox>();
        // 动画计数，用于在所有笔记动画结束后恢复滚动
        private int _notesAnimationTotal = 0;
        private int _notesAnimationCompleted = 0;
        private readonly object _notesAnimationLock = new object();
        twdata twtw = new twdata();
        NoticeSettingForm noticeSettingForm;
        //公用对象声明结束--------------
        private async void Form1_Load(object sender, EventArgs e)//tw=tWrite
        {
            string tws = null;
            try
            {
                if (File.Exists(Path.Combine(Application.StartupPath, "tw.tw")))
                {
                    try
                    {
                        tws = File.ReadAllText(Path.Combine(Application.StartupPath, "tw.tw"));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("程序即将关闭，在读取文件时，发生了错误" + ex.ToString(), "橘子记事本发生了一个错误");
                    }
                }
                else
                {
                    try
                    {
                        twtw = new twdata();
                        twtw = preparetw(twtw);
                        File.WriteAllText(Path.Combine(Application.StartupPath, "tw.tw"), JsonSerializer.Serialize(twtw));
                        Application.Restart();
                        tws = File.ReadAllText(Path.Combine(Application.StartupPath, "tw.tw"));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("程序即将关闭，发生了一个错误，在新建空文件时，发生了" + ex.ToString(), "橘子记事本发生了一个错误");
                        this.Close();
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("程序即将关闭，发生了错误" + ex.ToString(), "橘子记事本发生了一个错误");
                this.Close();
                Application.Exit();
            }
            int welcomeLabelWidth = HomePage.ClientSize.Width / 2;
            int welcomeLabelHeight = (int)(HomePage.ClientSize.Height * 0.3);
            welcomeLabel.AutoSize = false;
            welcomeLabel.UseCompatibleTextRendering = false; // ❌ 必须是 false！坚决不用老渲染器
            welcomeLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft; // 靠左上对齐，防止大字号居中计算时溢出
            welcomeLabel.Size = new Size(welcomeLabelWidth, welcomeLabelHeight);
            welcomeLabel.Location = new Point(0, 0);
            // 使用数值计算并转换为整数，避免通过字符串中转导致格式异常
            welcomeLabel.Location = new Point(0, (int)(-welcomeLabelWidth * 0.02));
            HomePage.Controls.Add(welcomeLabel);
            welcomeLabel.Text = "点击进入";
            AutoScaleLabelFontTrue(welcomeLabel);
            int plusx = Math.Max(1, (int)Math.Round(welcomeLabelHeight * 0.02));
            //对label1进行自适应字体大小
            AutoScaleLabelFontTrue(label1);
            welcomeLabel.Click += welcomeLabel_Click;
            try
            {

                twtw = JsonSerializer.Deserialize<twdata>(tws);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    default:
                        MessageBox.Show("程序即将关闭，发生了一个错误，在读取tw.tw文件时，发生了错误" + ex.ToString(), "橘子记事本发生了一个错误");
                        break;
                    case JsonException jsonEx:
                        if (MessageBox.Show("无法解析数据文件，要清空数据文件吗？\n点击是以清空(这将会永久清空数据，无法恢复)\n点击否以关闭程序", "橘子记事本发生了一个错误", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            twtw = new twdata();
                            File.WriteAllText(Path.Combine(Application.StartupPath, "tw.tw"), JsonSerializer.Serialize(twtw));
                            Application.Restart();
                        }
                        else
                        {

                            this.Close();
                            Application.Exit();
                        }
                        break;
                }
            }
            // 确保启动时显示的 Splash 窗口被关闭，防止其消息循环持续运行
            try
            {
                if (splash != null)
                {
                    try
                    {
                        if (splash.IsHandleCreated)
                        {
                            // 在 Splash 窗口线程上安全地关闭窗体
                            splash.Invoke(new Action(() => { try { splash.Close(); } catch { } }));
                        }
                        else
                        {
                            try { splash.Close(); } catch { }
                        }
                    }
                    catch
                    {
                        try { splash.Close(); } catch { }
                    }
                }

                // 等待线程结束，避免后台继续循环
                if (splashThread != null && splashThread.IsAlive)
                {
                    if (!splashThread.Join(2000))
                    {
                        // 如果线程仍未退出，尝试在该线程上退出消息循环
                        try { splash.Invoke(new Action(() => Application.ExitThread())); } catch { }
                        splashThread.Join(1000);
                    }
                }
            }
            catch { }
            // 确保主窗口不是最小化状态（splash 关闭后可能导致 Form1 被最小化）
            try
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
                try { this.Show(); } catch { }
                try { this.Activate(); } catch { }
                /*try
                {
                    // 通过短暂设置 TopMost 来确保窗口置顶一次，然后恢复原状态
                    bool originalTopMost = this.TopMost;
                    this.TopMost = true;
                    this.TopMost = originalTopMost;
                }
                catch { }*/
                try { this.BringToFront(); } catch { }
            }
            catch { }
        }
        twdata preparetw(twdata twnull)
        {
            twnull.tasksNoticeMethod = new List<int>();
            twnull.taskNoticeType = new List<int>();
            twnull.taskNoticeCfg1 = new List<int>();
            twnull.taskNoticeCfg2 = new List<int>();
            twnull.tasksNoticeTime = new List<DateTime>();
            twnull.isNoticeEnabled = new List<Boolean>();
            twnull.taskNoticeTime2 = new List<TimeSpan>();
            return twnull;
        }
        private void welcomeLabel_Click(object sender, EventArgs e)
        {
            mainTab.SelectTab(1);
        }
        private void AutoScaleLabelFontTrue(Label label)
        {
            if (label.ClientSize.Width <= 0 || label.ClientSize.Height <= 0) return;

            // 1. 设置自适应的上下限字号
            float targetFontSize = 120f;
            float minFontSize = 8f;

            // 2. 强制开启这两个属性，确保 Label 的边界和文字渲染对齐
            label.AutoSize = false;
            label.UseCompatibleTextRendering = false; // 必须为 false，使用高效的 TextRenderer 渲染

            // 3. 循环向下探测最完美的字号
            while (targetFontSize > minFontSize)
            {
                using (Font testFont = new Font(label.Font.FontFamily, targetFontSize, label.Font.Style))
                {
                    // 💡 关键：使用安全的 TextRenderer 测量文字在特定字号下的真实物理大小
                    Size textSize = TextRenderer.MeasureText(label.Text, testFont);

                    // 如果字体的宽度和高度都能完美放进 Label 的格子里，说明这个字号是完美的
                    if (textSize.Width < label.ClientSize.Width && textSize.Height < label.ClientSize.Height)
                    {
                        // 找到并赋值，然后退出
                        Font oldFont = label.Font;
                        label.Font = testFont;
                        // 如果旧字体不是系统默认字体，顺手释放防内存泄漏
                        //if (oldFont != null && oldFont != SystemFonts.DefaultFont) oldFont.Dispose();
                        return;
                    }
                }
                // 放不下就让字号减小 1 个单位继续试
                targetFontSize -= 1f;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (welcomeLabel == null) return;

            // 1. 让 Label 的宽度和高度直接拉满，给大字号留出绝对充足的画布
            welcomeLabel.Width = HomePage.ClientSize.Width;
            welcomeLabel.Height = HomePage.ClientSize.Height; // 直接给足高度，防止任何截断

            // 2. 根据容器的物理高度，直接用一个非常安全的比例系数直接赋字号
            // 假设你希望字号大约是容器高度的 15% 左右（可以根据视觉效果微调 0.12f ~ 0.18f）
            float newFontSize = HomePage.ClientSize.Height * 0.15f;
            newFontSize = Math.Max(12f, Math.Min(120f, newFontSize)); // 限制字号范围

            welcomeLabel.Font =
    new Font(
        welcomeLabel.Font.FontFamily,
        newFontSize,
        welcomeLabel.Font.Style
    );
            if (welcomeLabel == null) return;

            //label1自适应字体大小
            // 重启防抖计时器，若 200ms 内没有进一步的 SizeChanged 事件，则触发 refreshNotes
            try
            {
                if (_sizeChangedTimer != null)
                {
                    _sizeChangedTimer.Stop();
                    _sizeChangedTimer.Start();
                }
            }
            catch { }
        }

        private void SizeChangedTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                _sizeChangedTimer?.Stop();
                // 仅在当前可见标签页为 tWritePage 时刷新，避免不必要的重建
                if (mainTab.SelectedTab == tWritePage)
                {
                    refreshNotes();
                }
            }
            catch { }
        }
        private void oprationBox1_MouseEnter(object sender, EventArgs e)
        {
            if (isEditing)
            {
                label1.Text = "按下保存";
            }
            else
            {
                oprationBox1.Image = Properties.Resources.CreateNotes_text;
            }
        }
        private void oprationBox1_MouseLeave(object sender, EventArgs e)
        {
            if (isEditing)
            {
                label1.Text = "正在编辑";
            }
            else
            {
                oprationBox1.Image = Properties.Resources.CreateNotes_icon;
            }
        }

        private void oprationBox2_MouseEnter(object sender, EventArgs e)
        {
            oprationBox2.Image = Properties.Resources.DeleteNotes_text;
        }

        private void oprationBox2_MouseLeave(object sender, EventArgs e)
        {
            oprationBox2.Image = Properties.Resources.DeleteNotes_icon;
        }

        private void oprationBox1_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                // 保存：根据 EdId 判断是新增还是更新
                EdPage.syncData(); // 从 UI 拉取数据到属性
                if ((EdPage.title.Contains("TANGERINE_TWRITER_") && EdPage.title.Contains("\\/\\/")) || (EdPage.note.Contains("TANGERINE_TWRITER_") && EdPage.note.Contains("\\/\\/")))
                {
                    MessageBox.Show("抱歉，此笔记不能保存，请删除关键词TANGERINR_TWRITER_和\\/\\/\n要了解更多信息，请咨询TANGERINE LAB");
                    return;
                }
                List<string> notesList = new List<string>();
                List<string> titlesList = new List<string>();
                if (twtw.titles != null) titlesList = twtw.titles.ToList();
                if (twtw.notes != null) notesList = twtw.notes.ToList();

                if (EdPage.EdId >= 0 && EdPage.EdId < titlesList.Count)
                {
                    // 更新现有笔记
                    titlesList[EdPage.EdId] = EdPage.title ?? string.Empty;
                    notesList[EdPage.EdId] = EdPage.note ?? string.Empty;
                }
                else
                {
                    // 新增笔记
                    titlesList.Add(EdPage.title ?? string.Empty);
                    notesList.Add(EdPage.note ?? string.Empty);
                    twtw.tasksNoticeMethod.Add(-1);//初始化，否则后续会报错null
                    twtw.tasksNoticeTime.Add(DateTime.Now);
                    twtw.taskNoticeType.Add(-1);
                    twtw.taskNoticeCfg1.Add(-1);
                    twtw.taskNoticeCfg2.Add(-1);
                    twtw.isNoticeEnabled.Add(false);
                    twtw.taskNoticeTime2.Add(new TimeSpan(0, 0, 0));
                }
                twtw.titles = titlesList.ToArray();
                twtw.notes = notesList.ToArray();

                try
                {
                    File.WriteAllText(Path.Combine(Application.StartupPath, "tw.tw"), JsonSerializer.Serialize(twtw));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存失败, 因此橘子记事本即将关闭\n因为：\n" + ex, "橘子记事本发生了一个错误");
                    Application.Exit();
                }
                // 关闭编辑页并回到笔记列表
                try { mainTab.TabPages.Remove(editPage); } catch { }
                mainTab.SelectedTab = tWritePage;
                EdPage.reset();
                isEditing = false; // 保存完成
                label1.Text = "橘子记事本";
            }
            else
            {
                EdPage.reset();
                EdPage.EdId = -1;
                mainTab.TabPages.Add(editPage);
                editPage.Controls.Add(EdPage);
                EdPage.Dock = DockStyle.Fill;
                oprationBox1.Image = Properties.Resources.save_3_line;
                mainTab.SelectedTab = editPage;
                label1.Text = "正在编辑";
                isEditing = true;
            }
        }
        void refreshNotice()
        {
            noticeCheckBoxes.Clear();
            int checkBoxY = 0;
            int i = -1;
            if (twtw.titles != null || twtw.notes != null)
            {
                i++;
                foreach (string title in twtw.titles)
                {
                    int noticeId = noticeCheckBoxes.Count;

                    string twtitle = title;

                    if (twtitle.Length > 10)
                    {
                        twtitle = title.Substring(0, 10) + "...";
                    }

                    CheckBox cb = new CheckBox()
                    {
                        Text = twtitle,
                        AutoSize = false,
                        Width = tNoticePage.Width,
                        Height = tNoticePage.Height / 10,
                        Location = new Point(0, checkBoxY)
                    };

                    cb.MouseDown += (sender, e) =>
                    {
                        noticeCheckBox_Click(sender, e, noticeId);
                    };
                    cb.CheckStateChanged += (sender, e) =>
                    {
                        noticeCb_CheckedChanged(sender, e, noticeId);
                    };
                    noticeCheckBoxes.Add(cb);

                    checkBoxY += tNoticePage.Height / 10;
                    if (twtw.isNoticeEnabled[i])
                    {
                        cb.Checked = true;
                    }
                    else
                    {
                        cb.Checked = false;
                    }
                }
            }
            tNoticePage.Controls.Clear();
            tNoticePage.Controls.AddRange(noticeCheckBoxes.ToArray());
            tNoticePage.Refresh();
        }
        private void noticeCb_CheckedChanged(object sender, EventArgs e, int noticeId)
        {
            CheckBox cbt = sender as CheckBox ?? new CheckBox { Checked = false };
            if (cbt.Checked)
            {
                if (twtw.taskNoticeType[noticeId] != -1)
                {
                    twtw.isNoticeEnabled[noticeId] = true;
                }
                else
                {
                    cbt.Checked = false;
                }
            }
            else
            {
                if (twtw.taskNoticeType[noticeId] != -1)
                {
                    twtw.isNoticeEnabled[noticeId] = false;
                }
                else
                {
                    return;
                }
            }
            File.WriteAllText(Path.Combine(Application.StartupPath, "tw.tw"), JsonSerializer.Serialize(twtw));
        }
        private void noticeCheckBox_Click(object? sender, MouseEventArgs e, int noticeId)
        {
            if (e.Button == MouseButtons.Right)
            {
                noticeSettingForm = new NoticeSettingForm(twtw, noticeId);
                noticeSettingForm.ShowDialog();
                twtw = noticeSettingForm.twr;
                noticeSettingForm.Dispose();
                try
                {
                    File.WriteAllText(Path.Combine(Application.StartupPath, "tw.tw"), JsonSerializer.Serialize(twtw));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存失败,程序即将关闭，因为\n" + ex.ToString(), "");
                    this.Close();
                    Application.Exit();
                }
            }
        }
        void refreshNotes()
        {
            notesToShow = Array.Empty<tWriteNotes>();
            tWritePage.Controls.Clear();
            // 在开始构建并启动动画之前，先禁用父容器自动滚动
            tWritePage.AutoScroll = false;
            int titleCount = twtw?.titles?.Length ?? 0;
            int noteCount = twtw?.notes?.Length ?? 0;

            if (titleCount == 0 && noteCount == 0)
                return;

            if (titleCount != noteCount)
            {
                MessageBox.Show("笔记标题数和笔记正文数不相同，无法加载笔记\n你可以使用笔记修正工具来修复你的笔记\n,因此橘子记事本即将关闭");
                Application.Exit();
                return;
            }

            int notesCount = titleCount;
            // 重置动画计数器
            lock (_notesAnimationLock)
            {
                _notesAnimationTotal = notesCount;
                _notesAnimationCompleted = 0;
            }
            notesToShow = new tWriteNotes[notesCount];
            for (int i = 0; i < notesToShow.Length; i++)
            {
                notesToShow[i] = new tWriteNotes();
                notesToShow[i].Width = tWritePage.Width / 2;
                notesToShow[i].Height = tWritePage.Height / 3;
                notesToShow[i].NoteId = i;
                notesToShow[i].Click += note_Click;
                notesToShow[i].DoubleClick += note_doubleClick;
            }
            Boolean isFirstNote = true;
            int nowNoteY = 0;
            foreach (var note in notesToShow)
            {
                note.Title = twtw.titles[Array.IndexOf(notesToShow, note)];
                note.NoteText = twtw.notes[Array.IndexOf(notesToShow, note)];
            }
            foreach (var note in notesToShow)
            {
                Point intended;
                if (isFirstNote)
                {
                    intended = new Point(0, nowNoteY);
                    isFirstNote = false;
                }
                else
                {
                    intended = new Point(tWritePage.Width / 2, nowNoteY);
                    nowNoteY += tWritePage.Height / 3;
                    isFirstNote = true;
                }

                // 防御性赋值：保证控件有可显示的文字
                if (string.IsNullOrWhiteSpace(note.Title)) note.Title = "(无标题)";
                if (string.IsNullOrWhiteSpace(note.NoteText)) note.NoteText = "(无正文)";

                // 先设置预期目标位置，然后把控件放到起始（不偏移）位置
                note.IntendedLocation = intended;
                note.Location = intended;
                tWritePage.Controls.Add(note);
                // 订阅动画完成事件并启动并行动画（在独立线程中运行，但 UI 更新在主线程）
                try { note.AnimationCompleted += Note_AnimationCompleted; } catch { }
                try { note.tAnimationParallel(); } catch { }
            }
        }

        private void Note_AnimationCompleted(object? sender, EventArgs e)
        {
            // 取消订阅并判断是否所有动画完成，完成后恢复 AutoScroll
            try
            {
                if (sender is tWriteNotes tw)
                {
                    try { tw.AnimationCompleted -= Note_AnimationCompleted; } catch { }
                }
            }
            catch { }

            bool restore = false;
            lock (_notesAnimationLock)
            {
                _notesAnimationCompleted++;
                if (_notesAnimationTotal > 0 && _notesAnimationCompleted >= _notesAnimationTotal)
                {
                    restore = true;
                }
            }

            if (restore)
            {
                tWritePage.AutoScroll = true;
            }
        }

        private void mainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedTab = mainTab.SelectedIndex;
            switch (mainTab.SelectedTab)
            {
                default:
                    break;
                case TabPage tabpage when tabpage == tWritePage:
                    refreshNotes();
                    break;
                case TabPage tabpage when tabpage == tNoticePage:
                    refreshNotice();
                    break;
            }
        }
        private void note_doubleClick(object sender, EventArgs e)
        {
            if (!(sender is tWriteNotes twns)) return;

            // 如果编辑页尚未加入选项卡则添加
            if (!mainTab.TabPages.Contains(editPage))
            {
                editPage = new TabPage("编辑中的笔记");
                editPage.Controls.Add(EdPage);
                EdPage.Dock = DockStyle.Fill;
                mainTab.TabPages.Add(editPage);
            }

            // 加载笔记内容到编辑页并切换到编辑模式
            try { EdPage.LoadFromValues(twns.Title, twns.NoteText, twns.NoteId); } catch { EdPage.title = twns.Title; EdPage.note = twns.NoteText; EdPage.EdId = twns.NoteId; }
            mainTab.SelectedTab = editPage;
            isEditing = true;
            try { oprationBox1.Image = Properties.Resources.save_3_line; } catch { }
            try { label1.Text = "正在编辑"; } catch { }
        }
        private void note_Click(object sender, EventArgs e)
        {
            tWriteNotes twns = sender as tWriteNotes;
            if (selectedNoteId == -1)
            {
                noteSelected(twns);
                selectedNoteId = twns.NoteId;
            }
            else
            {
                if (twns.NoteId == selectedNoteId)
                {
                    noteUnselected(twns);
                    selectedNoteId = -1;
                }
                else
                {
                    noteUnselected(notesToShow[selectedNoteId]);
                    noteSelected(twns);
                    selectedNoteId = twns.NoteId;
                }
            }
        }
        private void oprationBox2_Click(object sender, EventArgs e)
        {
            twtw.titles = twtw.titles.Where((_, index) => index != selectedNoteId).ToArray();
            twtw.notes = twtw.notes.Where((_, index) => index != selectedNoteId).ToArray();
            try
            {
                File.WriteAllText(Path.Combine(Application.StartupPath, "tw.tw"), JsonSerializer.Serialize(twtw));
            }
            catch (Exception ex)
            {
                MessageBox.Show("应用 删除 失败,因此橘子记事本即将关闭\n因为：\n" + ex, "橘子记事本发生了一个错误");
                Application.Exit();
            }
            refreshNotes();
        }

        void noteSelected(tWriteNotes noteCard)
        {
            noteCard.toSelectedColor();
        }
        void noteUnselected(tWriteNotes noteCard)
        {
            noteCard.toNormalColor();
        }

        private void logoBox_DoubleClick(object sender, EventArgs e)
        {
            AboutForm af = new AboutForm();
            af.ShowDialog();
        }
    }
}
