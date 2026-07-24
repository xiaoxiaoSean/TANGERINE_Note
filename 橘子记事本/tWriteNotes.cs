using System.ComponentModel;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;

namespace 橘子记事本
{
    public partial class tWriteNotes : Control
    {

        // 唯一标识，可由外部设置与读取

        // 动画相关
        private Timer? _animationTimer;
        private Point _targetLocation;
        private int _initialOffsetY;
        private DateTime _animationStartTime;
        private const int AnimationDurationMs = 1000;
        // 不在控件内部修改父容器的 AutoScroll，交由父窗口统一管理

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point IntendedLocation { get; set; } = Point.Empty;

        // 性能缓存：避免每次 OnPaint 都做昂贵的字体测量与换行
        private bool _layoutValid = false;
        private float _cachedNoteSize = 0f;
        private float _cachedTitleSize = 0f;
        private Font? _cachedNoteFont = null;
        private Font? _cachedTitleFont = null;
        private List<string> _cachedLines = [];
        private int _cachedTitleHeight = 0;
        private int _cachedLineHeight = 0;
        private int _cachedAvailWidth = 0;
        private int _cachedBodyHeight = 0;
        private double _lastInvalidatedProgress = -1.0;
        // 上一次可视矩形，用于部分重绘
        private Rectangle _lastVisualBounds = Rectangle.Empty;
        // 并行动画支持
        private CancellationTokenSource? _parallelCts = null;
        private readonly object _parallelLock = new object();
        // 自动播放控制
        private bool _autoStarted = false;

        public double AnimationProgress { get; private set; } = 1.0;
        // 动画完成事件，外部可订阅
        public event EventHandler? AnimationCompleted;

        public tWriteNotes()
        {
            // 保留 designer 初始化以支持在设计器中正确显示
            try { InitializeComponent(); } catch { }

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            Size = new Size(260, 100);
            BackColor = Color.FromArgb(255, 253, 240);
            ForeColor = Color.FromArgb(40, 40, 40);
            Font = new Font("微软雅黑", 12f, FontStyle.Regular);
            ParentChanged += TWriteNotes_ParentChanged;
        }
        public void toSelectedColor()
        {
            BackColor = Color.FromArgb(255, 240, 200);
            ForeColor = Color.FromArgb(20, 20, 20);
        }
        public void toNormalColor()
        {
            BackColor = Color.FromArgb(255, 253, 240);
            ForeColor = Color.FromArgb(40, 40, 40);
        }
        private void TWriteNotes_ParentChanged(object? sender, EventArgs e)
        {
            // 当被添加到父容器时，尝试在父容器布局完成后自动启动动画
            if (_autoStarted) return;
            var p = Parent as Control;
            if (p == null) return;

            // 如果父容器已经创建句柄并可见，直接开始动画
            if (p.IsHandleCreated && p.Visible && Visible && !DesignMode)
            {
                _autoStarted = true;
                // ensure IntendedLocation set
                if (IntendedLocation == Point.Empty) IntendedLocation = Location;
                try { tAnimationParallel(); } catch { }
                return;
            }

            // 否则订阅父容器的 Layout 事件，布局完成后启动一次动画
            void Handler(object? s, LayoutEventArgs args)
            {
                try
                {
                    if (_autoStarted) return;
                    _autoStarted = true;
                    if (IntendedLocation == Point.Empty) IntendedLocation = Location;
                    try { tAnimationParallel(); } catch { }
                }
                finally
                {
                    try { p.Layout -= Handler; } catch { }
                }
            }

            try { p.Layout += Handler; } catch { }
        }

        /// <summary>
        /// 在独立线程上运行动画，UI 更新通过 BeginInvoke 回到主线程，支持并行动画。
        /// </summary>
        public void tAnimationParallel()
        {
            if (DesignMode) return;

            // 取消已有并行动画
            lock (_parallelLock)
            {
                try { _parallelCts?.Cancel(); } catch { }
                try { _parallelCts?.Dispose(); } catch { }
                _parallelCts = new CancellationTokenSource();
            }

            CancellationToken ct = _parallelCts.Token;

            // 记录目标位置并准备起始位置
            Point target = IntendedLocation != Point.Empty ? IntendedLocation : Location;
            int initialOffset = (int)Math.Round((double)Screen.PrimaryScreen.Bounds.Height / 12);

            // 父容器的滚动由外部（Form1）统一管理，控件内部不修改父容器状态

            // 设置起始视觉位置（在 UI 线程）
            try
            {
                if (IsHandleCreated)
                {
                    BeginInvoke((Action)(() =>
                    {
                        try { _lastVisualBounds = Bounds; } catch { _lastVisualBounds = new Rectangle(Location, Size); }
                        Location = new Point(target.X, target.Y + initialOffset);
                        AnimationProgress = 0.0;
                    }));
                }
            }
            catch { }

            // 在后台执行动画计算和周期性 UI 更新
            Task.Run(async () =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    while (!ct.IsCancellationRequested)
                    {
                        double elapsed = sw.Elapsed.TotalMilliseconds;
                        double progress = Math.Min(1.0, elapsed / AnimationDurationMs);
                        double ease = Math.Pow(progress, 3);
                        int currentOffset = (int)(initialOffset * (1.0 - ease));

                        // 计算目标位置基于 IntendedLocation（实时使用）
                        Point currentTarget = IntendedLocation != Point.Empty ? IntendedLocation : target;

                        // 将位置更新回 UI 线程
                        if (IsHandleCreated)
                        {
                            try
                            {
                                BeginInvoke((Action)(() =>
                                {
                                    try
                                    {
                                        Location = new Point(currentTarget.X, currentTarget.Y + currentOffset);
                                        AnimationProgress = progress;
                                        // 部分重绘
                                        var p = Parent as Control;
                                        if (p != null)
                                        {
                                            try { p.Invalidate(Rectangle.Union(_lastVisualBounds, Bounds), true); }
                                            catch { p.Invalidate(); }
                                        }
                                        else
                                        {
                                            try { Invalidate(); } catch { }
                                        }
                                        try { _lastVisualBounds = Bounds; } catch { }
                                    }
                                    catch { }
                                }));
                            }
                            catch { }
                        }

                        if (progress >= 1.0) break;

                        // 目标帧率 ~60 FPS
                        try
                        {
                            await Task.Delay(16, ct).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                            // 取消请求：安全退出动画循环
                            break;
                        }
                    }
                }
                catch (OperationCanceledException) { }
                finally
                {
                    // 结束时恢复到精确目标并恢复父容器滚动
                    if (IsHandleCreated)
                    {
                        try
                        {
                            BeginInvoke((Action)(() =>
                            {
                                try { Location = IntendedLocation != Point.Empty ? IntendedLocation : target; } catch { }
                                AnimationProgress = 1.0;
                            }));
                        }
                        catch { }
                    }

                    // 父容器滚动由 Form1 管理，此处不恢复任何状态

                    try { lock (_parallelLock) { _parallelCts?.Dispose(); _parallelCts = null; } } catch { }

                    // 触发动画完成事件（在 UI 线程）
                    try
                    {
                        if (IsHandleCreated)
                        {
                            BeginInvoke((Action)(() => AnimationCompleted?.Invoke(this, EventArgs.Empty)));
                        }
                        else
                        {
                            AnimationCompleted?.Invoke(this, EventArgs.Empty);
                        }
                    }
                    catch { }
                }
            }, ct);
        }

        [Category("外观")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Radius { get; set { field = Math.Max(0, value); Invalidate(); } } = 12;

        [Category("内容")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title { get; set { field = value ?? string.Empty; _layoutValid = false; Invalidate(); } } = string.Empty;

        [Category("内容")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string NoteText { get; set { field = value ?? string.Empty; _layoutValid = false; Invalidate(); } } = string.Empty;

        [Category("内容")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int NoteId { get; set; } = 0;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            _layoutValid = false;
            Invalidate();
        }

        // 外部调用以启动入场动画
        public void tAnimation()
        {
            if (DesignMode) return;

            if (!IsHandleCreated)
            {
                try { CreateControl(); } catch { }
            }

            if (_animationTimer != null)
            {
                try { _animationTimer.Stop(); _animationTimer.Dispose(); } catch { }
                _animationTimer = null;
            }
            _targetLocation = Location;
            _initialOffsetY = (int)Math.Round((double)Screen.PrimaryScreen.Bounds.Height / 12);

            // 父容器的滚动由外部（Form1.refreshNotes）统一管理，此处不修改父容器状态

            // 将显示位置设置为目标下方偏移（视觉上）并记录初始可见区域
            Location = new Point(_targetLocation.X, _targetLocation.Y + _initialOffsetY);
            AnimationProgress = 0.0;
            // 记录当前可见区域，用于高效部分重绘
            try { _lastVisualBounds = Bounds; } catch { _lastVisualBounds = new Rectangle(Location, Size); }
            _animationStartTime = DateTime.Now;

            _animationTimer = new Timer
            {
                // 恢复为 60 FPS 更新以保证动画平滑
                Interval = 16
            };
            _animationTimer.Tick += AnimationTimer_Tick;
            _animationTimer.Start();

            Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // 记录目标位置，但不要在此修改 Location（避免在未启动动画时产生固定空隙）
            _targetLocation = Location;
            _initialOffsetY = (int)Math.Round((double)Screen.PrimaryScreen.Bounds.Height / 12);
            AnimationProgress = 1.0;
            // 如果尚未自动启动且处于可见状态，尝试自动启动一次
            if (!_autoStarted && Parent != null && Visible && !DesignMode)
            {
                _autoStarted = true;
                if (IntendedLocation == Point.Empty) IntendedLocation = Location;
                try { tAnimationParallel(); } catch { }
            }
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            if (_animationTimer == null) return;
            double elapsedMs = (DateTime.Now - _animationStartTime).TotalMilliseconds;

            var parent = Parent;
            if (elapsedMs >= AnimationDurationMs)
            {
                _animationTimer.Stop();
                _animationTimer.Dispose();
                _animationTimer = null;

                Rectangle oldBounds = _lastVisualBounds;
                Location = _targetLocation;
                AnimationProgress = 1.0;
                Rectangle newBounds = Bounds;

                // 父容器滚动交由 Form1 管理，这里不再修改父容器状态

                // 部分重绘：使父控件重绘 old/new 区域的并集，减少绘制开销且避免残影
                if (parent != null)
                {
                    Rectangle union = Rectangle.Union(oldBounds, newBounds);
                    try { parent.Invalidate(union, true); } catch { parent.Invalidate(); }
                }
                else
                {
                    Invalidate();
                }

                _lastVisualBounds = newBounds;

                // 对非并行计时器路径，也触发动画完成事件
                try
                {
                    if (IsHandleCreated)
                    {
                        BeginInvoke((Action)(() => AnimationCompleted?.Invoke(this, EventArgs.Empty)));
                    }
                    else
                    {
                        AnimationCompleted?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch { }
            }
            else
            {
                double linearProgress = elapsedMs / AnimationDurationMs;
                double easeIn = Math.Pow(linearProgress, 3);
                int currentOffsetY = (int)(_initialOffsetY * (1.0 - easeIn));

                Rectangle oldBounds = _lastVisualBounds;
                Location = new Point(_targetLocation.X, _targetLocation.Y + currentOffsetY);
                AnimationProgress = linearProgress;
                Rectangle newBounds = Bounds;

                if (parent != null)
                {
                    Rectangle union = Rectangle.Union(oldBounds, newBounds);
                    try { parent.Invalidate(union, true); } catch { parent.Invalidate(); }
                }
                else
                {
                    Invalidate();
                }

                _lastVisualBounds = newBounds;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int alpha = Math.Clamp((int)(AnimationProgress * 255), 0, 255);
            Color back = Color.FromArgb(alpha, BackColor);
            Color fore = Color.FromArgb(alpha, ForeColor);

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int dia = Math.Min(Radius * 2, Math.Min(rect.Width, rect.Height));

            using (GraphicsPath path = new GraphicsPath())
            {
                if (dia <= 0) path.AddRectangle(rect);
                else
                {
                    path.AddArc(rect.X, rect.Y, dia, dia, 180, 90);
                    path.AddArc(rect.Right - dia, rect.Y, dia, dia, 270, 90);
                    path.AddArc(rect.Right - dia, rect.Bottom - dia, dia, dia, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - dia, dia, dia, 90, 90);
                    path.CloseFigure();
                }

                using (SolidBrush b = new SolidBrush(back)) g.FillPath(b, path);
                using (Pen p = new Pen(fore, 1.5f)) g.DrawPath(p, path);
            }
            // 绘制自适应字号的标题（单行，超出以三个英文句号 "..." 截断）和正文（最多 3 行，超出在第三行尾部加 "..."）
            int padding = Math.Max(8, Radius);
            int availWidth = Math.Max(1, ClientSize.Width - (padding * 2));
            int availHeight = Math.Max(1, ClientSize.Height - (padding * 2));

            // 辅助：测量文本像素宽度
            int MeasureTextWidth(string text, Font f)
            {
                if (string.IsNullOrEmpty(text)) return 0;
                Size sz = TextRenderer.MeasureText(text, f, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                return sz.Width;
            }

            // 辅助：裁剪字符串并加三个英文句号 "..."，保证宽度不超出
            string TrimToWidthWithThreeDots(string text, Font f, int maxWidth)
            {
                if (string.IsNullOrEmpty(text)) return string.Empty;
                if (MeasureTextWidth(text, f) <= maxWidth) return text;
                string dots = "...";
                int lo = 0, hi = text.Length;
                while (lo < hi)
                {
                    int mid = (lo + hi + 1) / 2;
                    string cand = text.Substring(0, mid) + dots;
                    if (MeasureTextWidth(cand, f) <= maxWidth) lo = mid;
                    else hi = mid - 1;
                }
                return lo <= 0 ? dots : text.Substring(0, lo) + dots;
            }

            // 辅助：按像素宽度换行，返回最多 maxLines 行，超出则在最后一行添加 "..."
            List<string> WrapTextToLines(string text, Font f, int maxWidth, int maxLines)
            {
                List<string> lines = [];
                if (string.IsNullOrEmpty(text)) return lines;
                int idx = 0, len = text.Length;
                while (idx < len && lines.Count < maxLines)
                {
                    int start = idx;
                    int lastFit = start;
                    for (int j = start + 1; j <= len; j++)
                    {
                        string seg = text.Substring(start, j - start);
                        if (MeasureTextWidth(seg, f) <= maxWidth) lastFit = j;
                        else break;
                    }
                    if (lastFit == start) lastFit = Math.Min(start + 1, len);
                    string line = text.Substring(start, lastFit - start);
                    idx = lastFit;
                    if (lines.Count == maxLines - 1 && idx < len)
                    {
                        line = TrimToWidthWithThreeDots(line + text.Substring(idx), f, maxWidth);
                        lines.Add(line);
                        return lines;
                    }
                    lines.Add(line);
                }
                if (idx < len && lines.Count > 0)
                {
                    int last = lines.Count - 1;
                    lines[last] = TrimToWidthWithThreeDots(lines[last] + text.Substring(idx), f, maxWidth);
                }
                return lines;
            }

            // 使用缓存以避免每次重绘都进行昂贵测量
            if (!_layoutValid || _cachedAvailWidth != availWidth || _cachedBodyHeight != availHeight)
            {
                // 清理旧字体
                try { if (_cachedNoteFont != null && _cachedNoteFont != Font) _cachedNoteFont.Dispose(); } catch { }
                try { if (_cachedTitleFont != null && _cachedTitleFont != Font) _cachedTitleFont.Dispose(); } catch { }
                _cachedLines.Clear();

                // 重新计算字号和换行
                _cachedAvailWidth = availWidth;
                _cachedBodyHeight = availHeight;
                float chosenNote = 8f;
                float chosenTitle = 16f;
                for (float s = 120f; s >= 8f; s -= 1f)
                {
                    using (Font testNote = new Font(Font.FontFamily, s, Font.Style))
                    using (Font testTitle = new Font(Font.FontFamily, Math.Min(s * 2f, 120f), FontStyle.Bold))
                    {
                        int titleW = MeasureTextWidth(Title ?? string.Empty, testTitle);
                        if (titleW > availWidth) continue;

                        Size noteMeasured = TextRenderer.MeasureText(NoteText ?? string.Empty, testNote, new Size(availWidth, int.MaxValue), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding);
                        double approxLines = Math.Ceiling((double)noteMeasured.Height / testNote.Height);
                        if (approxLines <= 3 && noteMeasured.Height <= availHeight - testTitle.Height + 1)
                        {
                            chosenNote = s;
                            chosenTitle = Math.Min(s * 2f, 120f);
                            break;
                        }
                    }
                }

                _cachedNoteSize = chosenNote;
                _cachedTitleSize = chosenTitle;
                try { _cachedNoteFont = new Font(Font.FontFamily, _cachedNoteSize, Font.Style); } catch { _cachedNoteFont = Font; }
                try { _cachedTitleFont = new Font(Font.FontFamily, _cachedTitleSize, FontStyle.Bold); } catch { _cachedTitleFont = Font; }

                // 准备换行结果
                _cachedLines = WrapTextToLines(NoteText ?? string.Empty, _cachedNoteFont, availWidth, 3);
                _cachedTitleHeight = TextRenderer.MeasureText(TrimToWidthWithThreeDots(Title ?? string.Empty, _cachedTitleFont, availWidth), _cachedTitleFont, new Size(availWidth, int.MaxValue), TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Height;

                _cachedLineHeight = TextRenderer.MeasureText("A", _cachedNoteFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Height;

                _layoutValid = true;
            }

            // 绘制标题
            string titleToDrawCached = TrimToWidthWithThreeDots(Title ?? string.Empty, _cachedTitleFont, _cachedAvailWidth);
            Rectangle titleRectCached = new Rectangle(padding, padding, _cachedAvailWidth, _cachedTitleHeight);
            TextRenderer.DrawText(g, titleToDrawCached, _cachedTitleFont, titleRectCached, fore, TextFormatFlags.SingleLine | TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPadding);

            // 绘制正文（最多 3 行），按缓存行高绘制
            int bodyYcached = padding + _cachedTitleHeight;
            int linesToRenderCached = Math.Min(_cachedLines.Count, Math.Min(3, Math.Max(0, _cachedBodyHeight / Math.Max(1, _cachedLineHeight))));
            for (int i = 0; i < linesToRenderCached; i++)
            {
                Rectangle lineRect = new Rectangle(padding, bodyYcached + (i * _cachedLineHeight), _cachedAvailWidth, _cachedLineHeight);
                TextRenderer.DrawText(g, _cachedLines[i], _cachedNoteFont, lineRect, fore, TextFormatFlags.SingleLine | TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPadding);
            }
        }
    }
}