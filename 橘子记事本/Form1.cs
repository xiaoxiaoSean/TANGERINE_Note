using Sunny.UI;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using CredentialManagement;
//主要部分由Sean ren完成
//部分由workbuddy(部分提醒),gemini(自适应文字),ChatGPT(splash窗口部分，加密功能)，Github Copilot(笔记卡片动画，splash窗口部分)完成
//这些ai不仅完成了上述部分，还辅助了其他部分的开发
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
            _sizeChangedTimer = new System.Windows.Forms.Timer
            {
                Interval = 200
            };
            _sizeChangedTimer.Tick += SizeChangedTimer_Tick;
            FormClosing += Form1_FormClosing;
            SetupTrayIcon();
        }
        //两个实例不能同时运行-开始
        public static class SingleInstance
        {
            public static readonly int WM_SHOWME =
                NativeMethods.RegisterWindowMessage("TANGERINE_TWRITER_SHOW");
        }
        internal static class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int RegisterWindowMessage(string lpString);

            [DllImport("user32.dll")]
            public static extern bool PostMessage(
                IntPtr hWnd,
                int Msg,
                IntPtr wParam,
                IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern bool ShowWindow(
                IntPtr hWnd,
                int nCmdShow);

            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(
                IntPtr hWnd);

            public const int SW_RESTORE = 9;

            public static readonly IntPtr HWND_BROADCAST =
                new IntPtr(0xFFFF);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == SingleInstance.WM_SHOWME)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    WindowState = FormWindowState.Normal;
                }

                Show();

                BringToFront();

                Activate();

                NativeMethods.SetForegroundWindow(this.Handle);
                return;
            }

            base.WndProc(ref m);
        }
        //两个实例不能同时运行-结束
        //Credential cred = new Credential();
        private System.Windows.Forms.Timer? _sizeChangedTimer;
        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!_isActuallyExiting)
            {
                // 关闭窗口 → 隐藏到系统托盘，保持程序运行
                e.Cancel = true;
                Hide();
                return;
            }

            // 真正退出：停止并释放所有提醒计时器
            if (timers.Count > 0)
            {
                foreach (var t in timers)
                {
                    try { t.Stop(); } catch { }
                    try { t.Dispose(); } catch { }
                }
                timers.Clear();
            }
            timersId.Clear();
            // 停止并释放尺寸防抖计时器
            try { _sizeChangedTimer?.Stop(); } catch { }
            try { _sizeChangedTimer?.Dispose(); } catch { }
            // 停止重试计时器
            try { _retryTimer?.Stop(); } catch { }
            try { _retryTimer?.Dispose(); } catch { }
            // 清理托盘图标
            if (trayIcon != null)
            {
                try { trayIcon.Visible = false; } catch { }
                try { trayIcon.Dispose(); } catch { }
            }
            if (trayMenu != null)
            {
                try { trayMenu.Dispose(); } catch { }
            }
        }

        private void SetupTrayIcon()
        {
            trayMenu = new ContextMenuStrip();

            var openItem = new ToolStripMenuItem("打开窗口");
            openItem.Click += (s, e) =>
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            };
            trayMenu.Items.Add(openItem);

            var exitItem = new ToolStripMenuItem("退出");
            exitItem.Click += (s, e) => ExitApplication();
            trayMenu.Items.Add(exitItem);

            trayIcon = new NotifyIcon
            {
                Icon = Icon,
                Text = "橘子记事本",
                ContextMenuStrip = trayMenu
            };
            trayIcon.DoubleClick += (s, e) =>
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            };
            trayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;
            trayIcon.Visible = true;
        }

        private void ExitApplication()
        {
            var result = MessageBox.Show(
                "确认退出吗，退出后，提醒将停止",
                "退出确认对话框",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                _isActuallyExiting = true;
                Application.Exit();
            }
        }

        private void ShowWindowsNotification(string title, int noteIndex)
        {
            _pendingReminderIndex = noteIndex;

            if (trayIcon != null)
            {
                trayIcon.BalloonTipTitle = "橘子记事本 - 提醒";
                trayIcon.BalloonTipText = $"「{title}」\n该笔记的提醒时间已到！";
                trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                trayIcon.ShowBalloonTip(8000);
            }

            // 启动重试计时器：每分钟重新弹一次通知，直到用户点击确认
            _retryTimer?.Stop();
            _retryTimer?.Dispose();
            _retryTimer = new System.Windows.Forms.Timer
            {
                Interval = 60000 // 1 分钟
            };
            _retryTimer.Tick += RetryTimer_Tick;
            _retryTimer.Start();
        }

        private void TrayIcon_BalloonTipClicked(object? sender, EventArgs e)
        {
            if (_pendingReminderIndex < 0) return;

            // 停止重试计时器
            _retryTimer?.Stop();
            _retryTimer?.Dispose();
            _retryTimer = null;

            int idx = _pendingReminderIndex;
            _pendingReminderIndex = -1;

            // 禁用该提醒
            if (idx < (twtw.isNoticeEnabled?.Count ?? 0))
            {
                twtw.isNoticeEnabled[idx] = false;
            }

            // 保存数据
            try
            {
                WriteBack();
            }
            catch { }

            // 显示确认 Toast
            UIMessageTip.ShowOk("确认提醒成功", 500);
        }
        void WriteBack()
        {
            // 对象转JSON
            string json = JsonSerializer.Serialize(twtw);


            // AES加密
            byte[] encrypted = CryptoHelper.Encrypt(
                json,
                pwd
            );


            // 保存
            File.WriteAllBytes(
                Path.Combine(Application.StartupPath, "tw.tw"),
                encrypted
            );
        }
        private void DecryptFile()
        {
            try
            {
                string path = Path.Combine(
                    Application.StartupPath,
                    "tw.tw");

                // 读取加密文件
                byte[] encrypted = File.ReadAllBytes(path);

                // 解密得到 JSON
                tws = CryptoHelper.Decrypt(
                    encrypted,
                    pwd);

                // JSON -> 对象
                twtw = JsonSerializer.Deserialize<twdata>(tws);

                if (twtw == null)
                {
                    throw new Exception("数据为空");
                }
            }
            catch (CryptographicException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void RetryTimer_Tick(object? sender, EventArgs e)
        {
            _retryTimer?.Stop();
            if (_pendingReminderIndex < 0) return;

            int idx = _pendingReminderIndex;
            string title = (idx < (twtw.titles?.Count ?? 0))
                ? (twtw.titles[idx] ?? "提醒")
                : "提醒";

            // 重新显示通知
            ShowWindowsNotification(title, idx);
        }

        private void ShowReminderDialog(string title)
        {
            try { System.Media.SystemSounds.Exclamation.Play(); } catch { }
            MessageBox.Show(
                $"「{title}」\n\n该笔记的提醒时间已到！",
                "橘子记事本 - 提醒",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        void triggerPastReminders(twdata twd)
        {
            if (twd.isNoticeEnabled == null || twd.taskNoticeType == null) return;

            for (int i = 0; i < twd.isNoticeEnabled.Count; i++)
            {
                if (!twd.isNoticeEnabled[i]) continue;
                if (i >= twd.taskNoticeType.Count || twd.taskNoticeType[i] != 1) continue;
                if (twd.tasksNoticeTime == null || i >= twd.tasksNoticeTime.Count) continue;
                if (twd.tasksNoticeTime[i] >= DateTime.Now) continue;

                // 已过期的绝对时间提醒，立即触发
                string title = (i < (twd.titles?.Count ?? 0)) ? (twd.titles[i] ?? "提醒") : "提醒";
                int method = (twd.tasksNoticeMethod != null && i < twd.tasksNoticeMethod.Count)
                    ? twd.tasksNoticeMethod[i] : 2;

                if (method == 1)
                {
                    // Windows 通知：不立即禁用，等待用户点击确认
                    ShowWindowsNotification(title, i);
                }
                else
                {
                    ShowReminderDialog(title);
                    twd.isNoticeEnabled[i] = false;
                }
            }
        }
        object tw = new object();
        //开始声明公用对象----------------//
        string tws = "";
        string pwd = "";
        TabPage editPage = new TabPage("编辑中的笔记");
        EditPage EdPage = new EditPage();
        Label welcomeLabel = new Label();
        bool isEditing = false;
        tWriteNotes[] notesToShow = Array.Empty<tWriteNotes>();
        bool isSelected = false;
        int selectedNoteId = -1;
        int selectedTab = 0;
        bool isNoticeStarted = false;
        List<CheckBox> noticeCheckBoxes = [];
        twdata twtw = new twdata();
        NoticeSettingForm noticeSettingForm;
        List<System.Windows.Forms.Timer> timers = [];
        List<int> timersId = [];
        private NotifyIcon? trayIcon;
        private ContextMenuStrip? trayMenu;
        private bool _isActuallyExiting = false;
        // Windows 通知确认机制
        private int _pendingReminderIndex = -1;
        private System.Windows.Forms.Timer? _retryTimer;
        //--------------------------------------------------------------//
        // 动画计数，用于在所有笔记动画结束后恢复滚动
        private int _notesAnimationTotal = 0;
        private int _notesAnimationCompleted = 0;
        private readonly object _notesAnimationLock = new object();
        //--------------------------------------------------------------//
        //公用对象声明结束----------------//
        //Form1开始加载-------------------------------------------------//
        private async void Form1_Load(object sender, EventArgs e)//tw=tWrite
        {
            try
            {
                if (File.Exists(Path.Combine(Application.StartupPath, "tw.tw")))
                {
                    try
                    {
                    incpwd:
                        try
                        {
                            /*cred.Target = "TANGERINE_TWRITER";

                            if (cred.Load())
                            {
                                string pwd = cred.Password;
                            }
                            else
                            {
                                MessageBox.Show("没有保存密码");
                            }
                            try
                            {
                                DecryptFile();
                            }
                            catch (CryptographicException)
                            {
                         /*   incpwd2:
                                try
                                {
                                    UIInputDialog.ShowInputPasswordDialog(ref pwd, UIStyle.DarkBlue, false, "Windows 凭据管理器存储的密码不对，输入正确的密码，然后重新存储", true, 6);
                                    DecryptFile();
                                }
                                catch (CryptographicException)
                                {
                                    MessageBox.Show("密码错误");
                                    goto incpwd2;
                                }
                                cred.Target = "TANGERINE_TWRITER";
                                cred.Delete();
                                cred.Password = pwd;
                                cred.Save();*/
                            //}*/
                            try
                            {
                                pwd = "";
                                DecryptFile();
                            }
                            catch (CryptographicException)
                            {

                            }
                            UIInputDialog.ShowInputPasswordDialog(ref pwd, UIStyle.DarkBlue, false, "输入解密密码以解密,只能英文，数字", true, 6);
                            DecryptFile();
                        }
                        catch (CryptographicException)
                        {
                            MessageBox.Show("密码错误");
                            goto incpwd;
                        }
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
                        //开始新建空文件
                        twtw = new twdata();
                        twtw = preparetw(twtw);
                        string json = JsonSerializer.Serialize(twtw);
                        pwd = "";
                    pwdna:
                        UIInputDialog.ShowInputPasswordDialog(ref pwd, UIStyle.DarkBlue, false, "欢迎，输入加密密码，不想输入密码可留空,只能英文，数字", true, 6);
                        if (!IsAsciiLetterOrNumber(pwd))
                        {
                            MessageBox.Show("只能英文，数字\n请重试", "橘子记事本");
                            pwd = "";
                            goto pwdna;
                        }
                        WriteBack();
                        /*cred.Target = "TANGERINE_TWRITER";
                        cred.Username = "User";
                        cred.Password = pwd;
                        cred.Type = CredentialType.Generic;
                        cred.PersistanceType = PersistanceType.LocalComputer;
                        try
                        {
                            cred.Save();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("密码保存到 Windows 凭据管理器 失败");
                        }*/
                        //新建空文件结束
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("程序即将关闭，发生了一个错误，在新建空文件时，发生了" + ex.ToString(), "橘子记事本发生了一个错误");
                        _isActuallyExiting = true;
                        Close();
                        _isActuallyExiting = true;
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("程序即将关闭，发生了错误" + ex.ToString(), "橘子记事本发生了一个错误");
                Close();
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
            // 启动时检查数据文件完整性：所有 List 的 Count 必须一致
            if (!CheckDataConsistency(twtw))
            {
                MessageBox.Show(
                    "数据文件出错，因为List的Count不一致，程序即将关闭。",
                    "橘子记事本 - 数据错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _isActuallyExiting = true;
                Close();
                return;
            }
            refreshTimers(ref timers, twtw);
            //Form1初始化已完成，开始关闭splash------------------------//
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
                if (WindowState == FormWindowState.Minimized)
                {
                    WindowState = FormWindowState.Normal;
                }
                try { Show(); } catch { }
                try { Activate(); } catch { }
                /*try
                {
                    // 通过短暂设置 TopMost 来确保窗口置顶一次，然后恢复原状态
                    bool originalTopMost = this.TopMost;
                    this.TopMost = true;
                    this.TopMost = originalTopMost;
                }
                catch { }*/
                try { BringToFront(); } catch { }

                // 窗口显示 2 秒后再触发已过期的提醒
                await Task.Delay(2000);
                triggerPastReminders(twtw);
                try
                {
                    WriteBack();
                }
                catch { }
            }
            catch { }
        }
        void refreshTimers(ref List<System.Windows.Forms.Timer> twtimers, twdata twd)
        {
            // 停止并释放所有现有计时器
            if (twtimers.Count > 0)
            {
                foreach (System.Windows.Forms.Timer timert in twtimers)
                {
                    try { timert.Stop(); } catch { }
                    try { timert.Dispose(); } catch { }
                }
            }
            twtimers.Clear();
            timersId.Clear();

            if (twd.isNoticeEnabled == null || twd.taskNoticeType == null) return;

            for (int i = 0; i < twd.isNoticeEnabled.Count; i++)
            {
                if (!twd.isNoticeEnabled[i]) continue;

                int noticeType = i < twd.taskNoticeType.Count ? twd.taskNoticeType[i] : -1;
                if (noticeType != 1 && noticeType != 2) continue;

                double intervalMs = 0;

                if (noticeType == 1) // 绝对时间
                {
                    if (twd.tasksNoticeTime == null || i >= twd.tasksNoticeTime.Count) continue;
                    DateTime targetTime = twd.tasksNoticeTime[i];
                    intervalMs = (targetTime - DateTime.Now).TotalMilliseconds;
                }
                else if (noticeType == 2) // 倒计时
                {
                    if (twd.taskNoticeTime2 == null || i >= twd.taskNoticeTime2.Count) continue;
                    intervalMs = twd.taskNoticeTime2[i].TotalMilliseconds;
                }

                if (intervalMs <= 0) continue; // 时间已过或无效

                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer
                {
                    Tag = i
                };
                timer.Tick += ReminderTimer_Tick;

                // 将间隔上限限制为 int.MaxValue（约 24.8 天）
                timer.Interval = intervalMs > int.MaxValue ? int.MaxValue : Math.Max(1, (int)intervalMs);

                twtimers.Add(timer);
                timersId.Add(i);
                timer.Start();
            }
        }
        private void ReminderTimer_Tick(object? sender, EventArgs e)
        {
            if (sender is not System.Windows.Forms.Timer timer) return;
            timer.Stop();

            int noteIndex = (int)(timer.Tag ?? -1);
            if (noteIndex < 0 || noteIndex >= (twtw.titles?.Count ?? 0)) return;

            // 重新检查绝对时间提醒（处理因间隔上限而被截断的情况）
            if (noteIndex < (twtw.taskNoticeType?.Count ?? 0) &&
                twtw.taskNoticeType[noteIndex] == 1) // 绝对时间
            {
                if (noteIndex < (twtw.tasksNoticeTime?.Count ?? 0))
                {
                    double remainingMs = (twtw.tasksNoticeTime[noteIndex] - DateTime.Now).TotalMilliseconds;
                    if (remainingMs > 1000) // 剩余超过 1 秒，重新调度
                    {
                        timer.Interval = remainingMs > int.MaxValue ? int.MaxValue : Math.Max(1, (int)remainingMs);
                        timer.Start();
                        return;
                    }
                }
            }

            // 触发提醒通知
            string title = (noteIndex < (twtw.titles?.Count ?? 0))
                ? (twtw.titles[noteIndex] ?? "提醒")
                : "提醒";

            int method = (twtw.tasksNoticeMethod != null && noteIndex < twtw.tasksNoticeMethod.Count)
                ? twtw.tasksNoticeMethod[noteIndex] : 2;

            if (method == 1)
            {
                // Windows 通知：不立即禁用，等待用户点击通知确认
                ShowWindowsNotification(title, noteIndex);
            }
            else
            {
                // 对话框是模态的，用户关闭即视为已确认
                ShowReminderDialog(title);
                if (noteIndex < (twtw.isNoticeEnabled?.Count ?? 0))
                {
                    twtw.isNoticeEnabled[noteIndex] = false;
                }
            }

            // 仅对话框类型在此处保存；Windows 通知类型在点击确认时保存
            if (method != 1)
            {
                try
                {
                    WriteBack();
                }
                catch { }
            }

            // 如果当前在提醒页面，刷新 UI
            try
            {
                if (mainTab.SelectedTab == tNoticePage)
                {
                    BeginInvoke(new Action(() => refreshNotice()));
                }
            }
            catch { }

            // 从列表中移除并释放计时器
            timers.Remove(timer);
            try { timer.Dispose(); } catch { }
        }
        bool IsAsciiLetterOrNumber(string str)
        {
            foreach (char c in str)
            {
                if (!((c >= 'a' && c <= 'z') ||
                      (c >= 'A' && c <= 'Z') ||
                      (c >= '0' && c <= '9')))
                {
                    return false;
                }
            }

            return true;
        }
        twdata preparetw(twdata twnull)
        {
            twnull.titles = [];
            twnull.notes = [];
            twnull.tasks = [];
            twnull.tasksNoticeMethod = [];
            twnull.taskNoticeType = [];
            twnull.taskNoticeCfg1 = [];
            twnull.taskNoticeCfg2 = [];
            twnull.tasksNoticeTime = [];
            twnull.isNoticeEnabled = [];
            twnull.taskNoticeTime2 = [];
            return twnull;
        }
        /// <summary>
        /// 检查数据文件中所有 List 的 Count 是否一致。
        /// 不一致说明 JSON 数据损坏，返回 false。
        /// </summary>
        private bool CheckDataConsistency(twdata twd)
        {
            if (twd == null) return false;

            int titleCount = twd.titles?.Count ?? 0;
            int noteCount = twd.notes?.Count ?? 0;

            // 基本校验：标题和正文数量必须一致
            if (titleCount != noteCount) return false;

            // 没有笔记则无需检查提醒列表
            if (titleCount == 0) return true;

            // 所有提醒相关 List 的 Count 必须与笔记数量一致
            return (twd.tasksNoticeMethod?.Count ?? 0) == titleCount
                && (twd.taskNoticeType?.Count ?? 0) == titleCount
                && (twd.taskNoticeCfg1?.Count ?? 0) == titleCount
                && (twd.taskNoticeCfg2?.Count ?? 0) == titleCount
                && (twd.tasksNoticeTime?.Count ?? 0) == titleCount
                && (twd.isNoticeEnabled?.Count ?? 0) == titleCount
                && (twd.taskNoticeTime2?.Count ?? 0) == titleCount;
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
                        //ChatGPT辅助修复了这个问题，上行代码会导致Form1拉起的其他窗口的部分控件无法正常加载
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
                List<string> notesList = [];
                List<string> titlesList = [];
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
                twtw.titles = titlesList;
                twtw.notes = notesList;

                try
                {
                    WriteBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存失败, 因此橘子记事本即将关闭\n因为：\n" + ex, "橘子记事本发生了一个错误");
                    _isActuallyExiting = true;
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
                    cb.Checked = twtw.isNoticeEnabled[i];
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
                if (twtw.taskNoticeType[noticeId] != -1 && twtw.tasksNoticeMethod[noticeId] != -1)
                {
                    twtw.isNoticeEnabled[noticeId] = true;
                }
                else
                {
                    cbt.Checked = false;
                    return;
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
            WriteBack();
            refreshTimers(ref timers, twtw);
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
                    WriteBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存失败,程序即将关闭，因为\n" + ex.ToString(), "");
                    Close();
                    _isActuallyExiting = true;
                    Application.Exit();
                }
                refreshTimers(ref timers, twtw);
                refreshNotice();
            }
        }
        void refreshNotes()
        {
            notesToShow = Array.Empty<tWriteNotes>();
            tWritePage.Controls.Clear();
            // 在开始构建并启动动画之前，先禁用父容器自动滚动
            tWritePage.AutoScroll = false;
            int titleCount = twtw?.titles?.Count ?? 0;
            int noteCount = twtw?.notes?.Count ?? 0;

            if (titleCount == 0 && noteCount == 0)
                return;

            if (titleCount != noteCount)
            {
                MessageBox.Show("笔记标题数和笔记正文数不相同，无法加载笔记\n你可以使用笔记修正工具来修复你的笔记\n,因此橘子记事本即将关闭");
                _isActuallyExiting = true;
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
                notesToShow[i] = new tWriteNotes
                {
                    Width = tWritePage.Width / 2,
                    Height = tWritePage.Height / 3,
                    NoteId = i
                };
                notesToShow[i].Click += note_Click;
                notesToShow[i].DoubleClick += note_doubleClick;
            }
            bool isFirstNote = true;
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
            if (selectedNoteId < 0) return;
            twtw.titles = twtw.titles.Where((_, index) => index != selectedNoteId).ToList();
            twtw.notes = twtw.notes.Where((_, index) => index != selectedNoteId).ToList();
            // 同步清理提醒相关数据，保持索引一致
            if (twtw.tasksNoticeMethod != null && selectedNoteId < twtw.tasksNoticeMethod.Count)
                twtw.tasksNoticeMethod.RemoveAt(selectedNoteId);
            if (twtw.taskNoticeType != null && selectedNoteId < twtw.taskNoticeType.Count)
                twtw.taskNoticeType.RemoveAt(selectedNoteId);
            if (twtw.taskNoticeCfg1 != null && selectedNoteId < twtw.taskNoticeCfg1.Count)
                twtw.taskNoticeCfg1.RemoveAt(selectedNoteId);
            if (twtw.taskNoticeCfg2 != null && selectedNoteId < twtw.taskNoticeCfg2.Count)
                twtw.taskNoticeCfg2.RemoveAt(selectedNoteId);
            if (twtw.tasksNoticeTime != null && selectedNoteId < twtw.tasksNoticeTime.Count)
                twtw.tasksNoticeTime.RemoveAt(selectedNoteId);
            if (twtw.isNoticeEnabled != null && selectedNoteId < twtw.isNoticeEnabled.Count)
                twtw.isNoticeEnabled.RemoveAt(selectedNoteId);
            if (twtw.taskNoticeTime2 != null && selectedNoteId < twtw.taskNoticeTime2.Count)
                twtw.taskNoticeTime2.RemoveAt(selectedNoteId);
            try
            {
                WriteBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("应用 删除 失败,因此橘子记事本即将关闭\n因为：\n" + ex, "橘子记事本发生了一个错误");
                _isActuallyExiting = true;
                Application.Exit();
            }
            selectedNoteId = -1;
            refreshNotes();
            refreshTimers(ref timers, twtw);
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
