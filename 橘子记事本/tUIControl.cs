using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;

namespace 橘子记事本
{
    // ====================================================================
    // 替代 SunnyUI 的控件实现 (workbuddy-20260727)
    // 不依赖 SunnyUI 包，用原生 WinForms 实现相同功能的控件
    // 类名与 SunnyUI 保持一致，放在 橘子记事本 命名空间中
    // ====================================================================

    /// <summary>
    /// UI 样式枚举 (workbuddy-20260727)
    /// 与 SunnyUI 的 UIStyle 保持一致的名称和常用值
    /// </summary>
    public enum UIStyle
    {
        Blue,
        DarkBlue,
        Default,
        White,
        Gray,
        Inherited,
        Custom,
        Colorful
    }

    /// <summary>
    /// 通知类型枚举 (workbuddy-20260727)
    /// 与 SunnyUI 的 UINotifierType 保持一致
    /// </summary>
    public enum UINotifierType
    {
        INFO,
        OK,
        ERROR,
        WARNING,
        Ask
    }

    /// <summary>
    /// 带描述信息的事件参数 (workbuddy-20260727)
    /// 与 SunnyUI 的 DescriptionEventArgs 保持一致
    /// </summary>
    public class DescriptionEventArgs : EventArgs
    {
        public string Description { get; set; } = "";
        public int Tag { get; set; }
    }

    /// <summary>
    /// UI 资源类，提供常用本地化文字 (workbuddy-20260727)
    /// </summary>
    public class UIResources
    {
        /// <summary>输入对话框标题</summary>
        public string InputTitle { get; set; } = "输入";
        /// <summary>编辑器不能为空提示</summary>
        public string EditorCantEmpty { get; set; } = "内容不能为空";
    }

    /// <summary>
    /// UI 样式管理器 (workbuddy-20260727)
    /// 与 SunnyUI 的 UIStyles 保持一致的接口
    /// </summary>
    public static class UIStyles
    {
        /// <summary>当前资源</summary>
        public static UIResources CurrentResources { get; } = new UIResources();
    }

    // ====================================================================
    // UITextBox - 替代 SunnyUI.UITextBox (workbuddy-20260727)
    // 继承原生 TextBox，添加 Watermark、ShowText、TextAlignment 等属性
    // ====================================================================
    public class UITextBox : TextBox
    {
        // Windows API：设置水印文字 (workbuddy-20260727)
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        private const int EM_SETCUEBANNER = 0x1501;
        private const int EM_GETCUEBANNER = 0x1502;

        private string _watermark = "";
        private bool _showText = true;
        private ContentAlignment _textAlignment = ContentAlignment.MiddleLeft;
        private int _radius = 5;

        public UITextBox()
        {
            // 默认外观
            BorderStyle = BorderStyle.FixedSingle;
        }

        /// <summary>水印文字（输入框为空时显示的提示文字）(workbuddy-20260727)</summary>
        [Category("外观")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Watermark
        {
            get => _watermark;
            set
            {
                _watermark = value ?? "";
                UpdateWatermark();
            }
        }

        /// <summary>是否显示文本 (workbuddy-20260727)</summary>
        [Category("行为")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowText
        {
            get => _showText;
            set => _showText = value;
        }

        /// <summary>文本对齐方式 (workbuddy-20260727)</summary>
        [Category("外观")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContentAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                _textAlignment = value;
                // 映射到原生 TextBox 的 TextAlign
                TextAlign = value switch
                {
                    ContentAlignment.TopLeft or ContentAlignment.MiddleLeft or ContentAlignment.BottomLeft => HorizontalAlignment.Left,
                    ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter => HorizontalAlignment.Center,
                    _ => HorizontalAlignment.Right
                };
            }
        }

        /// <summary>圆角半径 (workbuddy-20260727)</summary>
        [Category("外观")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Radius
        {
            get => _radius;
            set { _radius = Math.Max(0, value); Invalidate(); }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateWatermark();
        }

        /// <summary>更新水印文字 (workbuddy-20260727)</summary>
        private void UpdateWatermark()
        {
            if (IsHandleCreated && !string.IsNullOrEmpty(_watermark))
            {
                SendMessage(Handle, EM_SETCUEBANNER, (IntPtr)1, _watermark);
            }
        }
    }

    // ====================================================================
    // UIScrollingText - 替代 SunnyUI.UIScrollingText (workbuddy-20260727)
    // 实现文本水平滚动效果
    // ====================================================================
    public class UIScrollingText : Control
    {
        private Timer? _timer;
        private bool _active = true;
        private int _interval = 100;
        private int _radius = 5;
        private float _offsetX = 0f;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get => base.Text;
            set { base.Text = value; Invalidate(); }
        }

        /// <summary>是否激活滚动 (workbuddy-20260727)</summary>
        [Category("行为")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                if (_active) StartScrolling();
                else StopScrolling();
            }
        }

        /// <summary>滚动间隔（毫秒）(workbuddy-20260727)</summary>
        [Category("行为")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Interval
        {
            get => _interval;
            set
            {
                _interval = value;
                if (_timer != null) _timer.Interval = _interval;
            }
        }

        /// <summary>圆角半径 (workbuddy-20260727)</summary>
        [Category("外观")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Radius
        {
            get => _radius;
            set { _radius = Math.Max(0, value); Invalidate(); }
        }

        public UIScrollingText()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (_active && !DesignMode) StartScrolling();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible && _active && IsHandleCreated && !DesignMode)
                StartScrolling();
            else
                StopScrolling();
        }

        private void StartScrolling()
        {
            if (_timer == null)
            {
                _timer = new Timer { Interval = _interval };
                _timer.Tick += (s, e) =>
                {
                    _offsetX -= 1.5f;
                    Invalidate();
                };
            }
            _timer.Start();
        }

        private void StopScrolling()
        {
            _timer?.Stop();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 绘制圆角背景
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = GetRoundedRect(rect, _radius))
            {
                using (SolidBrush bg = new SolidBrush(BackColor))
                    g.FillPath(bg, path);
                using (Pen pen = new Pen(Color.FromArgb(80, 80, 80), 1f))
                    g.DrawPath(pen, path);
            }

            // 绘制滚动文字
            string text = Text ?? "";
            if (!string.IsNullOrEmpty(text))
            {
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                    using (SolidBrush fb = new SolidBrush(ForeColor))
                    {
                        // 测量文字宽度
                        SizeF textSize = g.MeasureString(text, Font);
                        float textWidth = textSize.Width;

                        // 如果文字宽度小于控件宽度，不滚动
                        if (textWidth <= Width)
                        {
                            g.DrawString(text, Font, fb, new RectangleF(0, 0, Width, Height), sf);
                        }
                        else
                        {
                            // 滚动绘制：文字重复两次，中间加间隔
                            float gap = 60f;
                            float totalWidth = textWidth + gap;

                            // 归一化偏移量
                            if (_offsetX < -totalWidth) _offsetX += totalWidth;

                            // 绘制第一份
                            g.DrawString(text, Font, fb, new RectangleF(_offsetX, 0, textWidth, Height), sf);
                            // 绘制第二份（循环）
                            g.DrawString(text, Font, fb, new RectangleF(_offsetX + totalWidth, 0, textWidth, Height), sf);
                        }
                    }
                }
            }
        }

        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0) { path.AddRectangle(rect); return path; }
            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Stop();
                _timer?.Dispose();
                _timer = null;
            }
            base.Dispose(disposing);
        }
    }

    // ====================================================================
    // UISwitch - 替代 SunnyUI.UISwitch (workbuddy-20260727)
    // 开关控件，点击切换开/关状态
    // ====================================================================
    public class UISwitch : Control
    {
        private bool _checked = false;
        private int _radius = 20;

        /// <summary>开关是否打开 (workbuddy-20260727)</summary>
        [Category("行为")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    Invalidate();
                    CheckedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>状态改变事件 (workbuddy-20260727)</summary>
        public event EventHandler? CheckedChanged;

    }

    // ====================================================================
    // UIInputForm - 替代 SunnyUI.UIInputForm (workbuddy-20260727)
    // 输入对话框窗体，包含 Label、Editor(UITextBox)、确定/取消按钮
    // ====================================================================
    public class UIInputForm : Form
    {
        // Win32 API：强制设置前台窗口 (workbuddy-20260727)
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        private readonly Label _label;
        private readonly UITextBox _editor;
        private readonly Button _btnOK;
        private readonly Button _btnCancel;
        private bool _isOK = false;

        /// <summary>描述标签 (workbuddy-20260727)</summary>
        public Label Label => _label;
        /// <summary>输入文本框 (workbuddy-20260727)</summary>
        public UITextBox Editor => _editor;
        /// <summary>是否校验输入为空 (workbuddy-20260727)</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CheckInputEmpty { get; set; } = true;
        /// <summary>用户是否点击了确定 (workbuddy-20260727)</summary>
        public bool IsOK => _isOK;
        /// <summary>样式 (workbuddy-20260727)</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UIStyle Style { get; set; } = UIStyle.Blue;

        public UIInputForm()
        {
            // 窗体基本设置
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Width = 400;
            Height = 180;

            // 创建描述标签
            _label = new Label
            {
                Text = "请输入:",
                Location = new Point(15, 15),
                Size = new Size(360, 25),
                Font = new Font("微软雅黑", 10f)
            };
            Controls.Add(_label);

            // 创建输入文本框
            _editor = new UITextBox
            {
                Location = new Point(15, 45),
                Size = new Size(360, 30),
                Font = new Font("微软雅黑", 10f)
            };
            Controls.Add(_editor);

            // 创建确定按钮
            _btnOK = new Button
            {
                Text = "确定",
                DialogResult = DialogResult.OK,
                Location = new Point(200, 100),
                Size = new Size(80, 30)
            };
            _btnOK.Click += (s, e) =>
            {
                if (CheckInputEmpty && string.IsNullOrEmpty(_editor.Text))
                {
                    MessageBox.Show(UIStyles.CurrentResources.EditorCantEmpty, "提示");
                    _isOK = false;
                    DialogResult = DialogResult.None;
                }
                else
                {
                    _isOK = true;
                    DialogResult = DialogResult.OK;
                }
            };
            Controls.Add(_btnOK);

            // 创建取消按钮
            _btnCancel = new Button
            {
                Text = "取消",
                DialogResult = DialogResult.Cancel,
                Location = new Point(295, 100),
                Size = new Size(80, 30)
            };
            Controls.Add(_btnCancel);

            // 接受/取消按钮
            AcceptButton = _btnOK;
            CancelButton = _btnCancel;

            // 设置初始活动控件 (workbuddy-20260727)
            this.ActiveControl = _editor;

            // Load 事件：句柄已创建，再次确保 ActiveControl 指向文本框
            this.Load += (s, e) =>
            {
                this.ActiveControl = _editor;
            };

            // 窗体显示后强制将对话框设为前台窗口，并设置文本框焦点 (workbuddy-20260727)
            // 问题根因：splash 窗口在独立线程运行，可能持有 Windows 前台焦点，
            // 导致对话框虽显示但无法接收键盘输入。必须用 SetForegroundWindow 强制夺取前台焦点。
            Shown += (s, e) =>
            {
                // 强制将对话框设为前台窗口 (workbuddy-20260727)
                this.Activate();
                if (this.IsHandleCreated)
                {
                    try { SetForegroundWindow(this.Handle); } catch { }
                    try { SwitchToThisWindow(this.Handle, true); } catch { }
                }
                this.ActiveControl = _editor;

                // 用 Timer 延迟设置文本框焦点，确保在窗口前台化之后执行
                var focusTimer = new System.Windows.Forms.Timer { Interval = 10 };
                focusTimer.Tick += (s2, e2) =>
                {
                    focusTimer.Stop();
                    focusTimer.Dispose();
                    try
                    {
                        // 再次确保窗口在前台
                        this.Activate();
                        SetForegroundWindow(this.Handle);
                        _editor.Focus();
                        _editor.SelectAll();
                    }
                    catch { }
                };
                focusTimer.Start();
            };
        }

        /// <summary>渲染（空实现，保持接口兼容）(workbuddy-20260727)</summary>
        public void Render() { }
    }

    // ====================================================================
    // UIInputDialog - 替代 SunnyUI.UIInputDialog (workbuddy-20260727)
    // 静态类，提供 ShowInputPasswordDialog 等静态方法
    // ====================================================================
    public static class UIInputDialog
    {
        /// <summary>
        /// 显示密码输入对话框 (workbuddy-20260727)
        /// </summary>
        public static bool ShowInputPasswordDialog(ref string value, UIStyle style, bool checkEmpty, string desc, bool showMask, int maxLength)
        {
            using var frm = new UIInputForm();
            frm.Text = UIStyles.CurrentResources.InputTitle;
            frm.Label.Text = desc;
            frm.CheckInputEmpty = checkEmpty;
            frm.Editor.PasswordChar = '*';
            frm.Editor.MaxLength = maxLength;
            frm.Style = style;
            frm.ShowInTaskbar = false;
            frm.TopMost = true;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Render();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                value = frm.Editor.Text;
                return true;
            }
            return false;
        }
    }

    // ====================================================================
    // UIMessageTip - 替代 SunnyUI.UIMessageTip (workbuddy-20260727)
    // 显示临时 Toast 提示（成功/错误），自动关闭
    // ====================================================================
    public static class UIMessageTip
    {
        /// <summary>显示成功提示 (workbuddy-20260727)</summary>
        public static void ShowOk(string text, int duration = 2000)
        {
            ShowTip(text, Color.FromArgb(82, 196, 26), Color.White, duration);
        }

        /// <summary>显示错误提示 (workbuddy-20260727)</summary>
        public static void ShowError(string text, int duration = 2000)
        {
            ShowTip(text, Color.FromArgb(245, 108, 108), Color.White, duration);
        }

        /// <summary>显示信息提示 (workbuddy-20260727)</summary>
        public static void ShowInfo(string text, int duration = 2000)
        {
            ShowTip(text, Color.FromArgb(64, 158, 255), Color.White, duration);
        }

        /// <summary>显示 Toast 提示的核心实现 (workbuddy-20260727)</summary>
        private static void ShowTip(string text, Color backColor, Color foreColor, int duration)
        {
            // 在后台线程创建并显示 Toast 窗口，避免阻塞调用方
            Thread tipThread = new Thread(() =>
            {
                try
                {
                    Form tip = new Form
                    {
                        FormBorderStyle = FormBorderStyle.None,
                        ShowInTaskbar = false,
                        TopMost = true,
                        StartPosition = FormStartPosition.Manual,
                        ShowIcon = false,
                        ControlBox = false,
                        MaximizeBox = false,
                        MinimizeBox = false,
                        BackColor = backColor,
                        TransparencyKey = Color.Magenta
                    };

                    // 测量文字大小以确定窗口尺寸
                    using (Bitmap bmp = new Bitmap(1, 1))
                    using (Graphics g = Graphics.FromImage(bmp))
                    using (Font font = new Font("微软雅黑", 11f))
                    {
                        SizeF textSize = g.MeasureString(text, font, Screen.PrimaryScreen.Bounds.Width / 2);
                        int padding = 20;
                        tip.Width = (int)textSize.Width + padding * 2;
                        tip.Height = (int)textSize.Height + padding;

                        // 屏幕顶部居中
                        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                        tip.Left = (screenWidth - tip.Width) / 2;
                        tip.Top = 20;

                        Label label = new Label
                        {
                            Text = text,
                            Font = font,
                            ForeColor = foreColor,
                            BackColor = backColor,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Dock = DockStyle.Fill
                        };
                        tip.Controls.Add(label);
                    }

                    tip.Shown += (s, e) =>
                    {
                        // duration 毫秒后自动关闭
                        Timer timer = new Timer { Interval = duration };
                        timer.Tick += (s2, e2) =>
                        {
                            timer.Stop();
                            timer.Dispose();
                            tip.Close();
                        };
                        timer.Start();
                    };

                    // Application.Run(tip) 会显示 tip 并启动消息循环，tip 关闭后退出
                    tip.FormClosed += (s, e) => Application.ExitThread();
                    Application.Run(tip);
                }
                catch (Exception ex){ MessageBox.Show("橘子记事本发生了以下错误，你已成功确认提醒，但 确认提醒成功 提示没有显示\n这是因为\n" + ex.ToString()); }
            })
            {
                IsBackground = true
            };
            // 必须设为 STA 线程，否则 WinForms 的 Application.Run 无法正常显示窗口 (workbuddy-20260727)
            tipThread.SetApartmentState(ApartmentState.STA);
            tipThread.Start();
        }
    }

    // ====================================================================
    // UINotifier - 替代 SunnyUI.UINotifier (workbuddy-20260727)
    // 屏幕右下角通知提示，支持点击回调和不自动关闭
    // ====================================================================
    public static class UINotifier
    {
        /// <summary>
        /// 显示通知 (workbuddy-20260727)
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="type">通知类型</param>
        /// <param name="title">标题</param>
        /// <param name="autoClose">是否自动关闭</param>
        /// <param name="duration">自动关闭时间（毫秒），0=不自动关闭</param>
        /// <param name="tag">附加数据</param>
        /// <param name="clickHandler">点击回调</param>
        public static void Show(string message, UINotifierType type, string title,
            bool autoClose, int duration, object? tag,
            EventHandler<DescriptionEventArgs>? clickHandler)
        {
            // 根据类型选择颜色
            Color backColor = type switch
            {
                UINotifierType.OK => Color.FromArgb(82, 196, 26),
                UINotifierType.ERROR => Color.FromArgb(245, 108, 108),
                UINotifierType.WARNING => Color.FromArgb(230, 162, 60),
                UINotifierType.Ask => Color.FromArgb(230, 162, 60),
                _ => Color.FromArgb(64, 158, 255)  // INFO
            };

            Thread notifierThread = new Thread(() =>
            {
                try
                {
                    Form notifier = new Form
                    {
                        FormBorderStyle = FormBorderStyle.None,
                        ShowInTaskbar = false,
                        TopMost = true,
                        StartPosition = FormStartPosition.Manual,
                        ShowIcon = false,
                        ControlBox = false,
                        Width = 360,
                        Height = 100
                    };

                    // 定位到屏幕右下角
                    var screen = Screen.PrimaryScreen.WorkingArea;
                    notifier.Left = screen.Right - notifier.Width - 20;
                    notifier.Top = screen.Bottom - notifier.Height - 20;

                    // 标题标签
                    Label titleLabel = new Label
                    {
                        Text = title,
                        Font = new Font("微软雅黑", 10f, FontStyle.Bold),
                        ForeColor = Color.White,
                        BackColor = backColor,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Location = new Point(0, 0),
                        Size = new Size(notifier.Width, 30),
                        Padding = new Padding(10, 0, 0, 0)
                    };
                    notifier.Controls.Add(titleLabel);

                    // 消息标签
                    Label msgLabel = new Label
                    {
                        Text = message,
                        Font = new Font("微软雅黑", 9f),
                        ForeColor = Color.Black,
                        BackColor = Color.White,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Location = new Point(0, 30),
                        Size = new Size(notifier.Width, notifier.Height - 30),
                        Padding = new Padding(10, 5, 10, 5)
                    };
                    notifier.Controls.Add(msgLabel);

                    // 绘制边框
                    notifier.Paint += (s, e) =>
                    {
                        using Pen p = new Pen(backColor, 2);
                        e.Graphics.DrawRectangle(p, 0, 0, notifier.Width - 1, notifier.Height - 1);
                    };

                    // 点击事件 (workbuddy-20260727)
                    void OnClick()
                    {
                        try
                        {
                            clickHandler?.Invoke(notifier, new DescriptionEventArgs
                            {
                                Description = message
                            });
                        }
                        catch { }
                        notifier.Close();
                    }
                    notifier.Click += (s, e) => OnClick();
                    titleLabel.Click += (s, e) => OnClick();
                    msgLabel.Click += (s, e) => OnClick();

                    // 自动关闭
                    if (autoClose && duration > 0)
                    {
                        notifier.Shown += (s, e) =>
                        {
                            Timer timer = new Timer { Interval = duration };
                            timer.Tick += (s2, e2) =>
                            {
                                timer.Stop();
                                timer.Dispose();
                                notifier.Close();
                            };
                            timer.Start();
                        };
                    }

                    notifier.FormClosed += (s, e) => Application.ExitThread();
                    Application.Run(notifier);
                }
                catch { }
            })
            {
                IsBackground = true
            };
            notifierThread.Start();
        }
    }
}
