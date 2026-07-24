# 橘子记事本 - 2026年7月24日 开发记录

---

## 一、任务列表 (Task List)

| 序号 | 任务 | 状态 |
|------|------|------|
| 1 | 提醒功能 Timer 实现（refreshTimers / ReminderTimer_Tick / 生命周期） | ✅ 完成 |
| 2 | 通知方式 + 系统托盘 + 过期处理（第二轮） | ✅ 完成 |
| 3 | 过期提醒延迟触发（窗口闪避，2秒延迟） | ✅ 完成 |
| 4 | 创建 ToastForm 确认窗口 | ✅ 完成 |
| 5 | 修改 Form1.cs 实现通知确认和重试机制 | ✅ 完成 |
| 6 | twdata.cs 类型变更 string[] → List\<string\> | ✅ 完成 |
| 7 | Form1.cs 适配 List + 数据一致性检查 | ✅ 完成 |
| 8 | NoticeSettingForm.cs 条件验证 | ✅ 完成 |

---

## 二、对话摘要

本日共完成三轮迭代开发，围绕橘子记事本的**提醒功能**进行了从零到一的构建和完善：

1. **第一轮**：实现了提醒 Timer 调度核心——`refreshTimers()` 重写、`ReminderTimer_Tick` 新增、生命周期管理、多触发点补充，以及 `NoticeSettingForm` 的 Bug 修复。
2. **第二轮**：增加了通知方式（Windows 气泡 / 对话框）、系统托盘驻留、启动时过期提醒处理。
3. **第三轮**：优化用户体验——过期提醒延迟 2 秒触发（避免"糊脸"）、Windows 通知增加"点击确认"机制、未确认每分钟重试、确认后弹出 Toast 提示。

最终构建：**0 错误，51 个警告（全部为已有警告，非本次引入）**。

---

## 三、涉及文件清单

| 文件 | 操作 | 说明 |
|------|------|------|
| `橘子记事本/Form1.cs` | 修改 | 主窗体，所有核心逻辑 |
| `橘子记事本/NoticeSettingForm.cs` | 修改 | 提醒设置弹窗 |
| `橘子记事本/NoticeSettingForm.Designer.cs` | 修改 | 控件文本调整 |
| `橘子记事本/ToastForm.cs` | **新建** | 确认提示 Toast 窗口 |

---

## 四、详细代码说明

### 4.1 ToastForm.cs（新建文件）

**路径**：`橘子记事本/ToastForm.cs`
**作用**：用户点击 Windows 通知确认后，在屏幕顶部居中显示一个无边框的临时提示窗口。

#### 类结构

```
ToastForm : Form
├── _closeTimer : System.Windows.Forms.Timer   // 自动关闭计时器
├── 构造函数 ToastForm(string message)          // 设置窗口外观、Label、计时器
├── CreateParams (override)                     // 窗口扩展样式
├── OnFormClosed (override)                     // 清理计时器
└── ShowToast(string) : static void             // 静态快捷入口
```

#### 逐行注释

```csharp
// L3: XML 文档注释，描述窗口用途
// L6: internal class — 仅程序集内可见
internal class ToastForm : Form
{
    // L8: _closeTimer — 只读字段，在构造函数中初始化，用于 500ms 后自动关闭窗口
    private readonly System.Windows.Forms.Timer _closeTimer;

    // L10: 构造函数，message 为要显示的文字（目前固定传 "确认提醒成功"）
    public ToastForm(string message)
    {
        // L12: FormBorderStyle.None → 无边框窗口
        // L13: ShowInTaskbar = false → 不在任务栏显示
        // L14: TopMost = true → 置顶显示（在所有窗口之上）
        // L15: StartPosition = FormStartPosition.Manual → 手动设置位置
        // L16-L19: 隐藏图标、控制按钮、最大化/最小化按钮
        this.FormBorderStyle = FormBorderStyle.None;
        this.ShowInTaskbar = false;
        this.TopMost = true;
        this.StartPosition = FormStartPosition.Manual;
        this.ShowIcon = false;
        this.ControlBox = false;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // L21-22: 获取主屏幕分辨率（用 PrimaryScreen 取主显示器）
        int screenWidth = Screen.PrimaryScreen!.Bounds.Width;   // 屏幕宽度
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;  // 屏幕高度

        // L24-25: 窗口尺寸 = 屏幕宽的 1/3 × 屏幕高的 1/12
        this.Width = screenWidth / 3;
        this.Height = screenHeight / 12;

        // L26-27: 定位到屏幕顶部水平居中（Left = 屏幕中央，Top = 0 贴顶）
        this.Left = (screenWidth - this.Width) / 2;
        this.Top = 0;

        // L29: 半透明深色背景
        // Color.FromArgb(alpha, r, g, b):
        //   alpha=220 → 约 86% 不透明度（220/255）
        //   r=g=b=30 → 深灰/近黑色
        this.BackColor = Color.FromArgb(220, 30, 30, 30);

        // L31-38: 创建 Label 控件显示提示文字
        var label = new Label
        {
            Text = message,                                    // 显示文字
            ForeColor = Color.White,                           // 白色前景
            Font = new Font("Microsoft YaHei", 13f, ...),     // 微软雅黑 13pt
            TextAlign = ContentAlignment.MiddleCenter,         // 垂直+水平居中
            Dock = DockStyle.Fill                             // 填满整个窗口
        };
        this.Controls.Add(label);

        // L42-48: _closeTimer — 500ms 后触发，停止计时器并关闭窗口
        _closeTimer = new System.Windows.Forms.Timer { Interval = 500 };
        _closeTimer.Tick += (_, _) =>
        {
            _closeTimer.Stop();   // 先停止，防止重复触发
            this.Close();          // 关闭窗口
        };
        _closeTimer.Start();
    }

    // L51-62: 重写 CreateParams，设置窗口扩展样式
    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            // WS_EX_TOOLWINDOW (0x00000080):
            //   - 不在 Alt+Tab 切换列表中显示
            //   - 不在任务栏显示（配合 ShowInTaskbar=false）
            cp.ExStyle |= 0x00000080;

            // WS_EX_NOACTIVATE (0x08000000):
            //   - 窗口显示时不抢夺键盘焦点
            //   - 用户正在做其他事时不会被这个窗口打断
            cp.ExStyle |= 0x08000000;
            return cp;
        }
    }

    // L64-68: 窗口关闭时确保 _closeTimer 被释放
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        try { _closeTimer?.Dispose(); } catch { }
        base.OnFormClosed(e);
    }

    // L73-77: 静态快捷方法
    // 调用方式: ToastForm.ShowToast("确认提醒成功");
    // Show() 是非模态的，窗口独立运行，500ms 后自动 Close()
    public static void ShowToast(string message)
    {
        var toast = new ToastForm(message);
        toast.Show();
    }
}
```

#### 关键设计决策

| 决策 | 原因 |
|------|------|
| 使用 `Show()` 而非 `ShowDialog()` | 非模态，不阻塞调用方代码执行 |
| 贴顶定位 (`Top = 0`) | 顶部居中是最不打扰用户的提醒位置 |
| 深色半透明背景 | 比纯色更现代，比全透明更容易阅读 |
| `WS_EX_NOACTIVATE` | 用户正在打字时弹出 Toast 不会导致当前窗口失焦 |
| `WS_EX_TOOLWINDOW` | 不在 Alt+Tab 中出现，避免造成"多了一个窗口"的困惑 |

---

### 4.2 Form1.cs — 本次所有改动详解

#### 4.2.1 新增字段 (L229-232)

```csharp
// L229-230: 注释说明这两个字段属于"Windows 通知确认机制"模块
// Windows 通知确认机制
private int _pendingReminderIndex = -1;        // L231
private System.Windows.Forms.Timer? _retryTimer; // L232
```

**字段说明**：

| 字段 | 类型 | 初始值 | 含义 |
|------|------|--------|------|
| `_pendingReminderIndex` | `int` | `-1` | 当前等待用户点击确认的提醒笔记索引。<br>值为 `-1` 表示没有待确认的提醒。<br>值为 `≥0` 表示 `twtw.titles[idx]` 这条笔记正在等待确认。 |
| `_retryTimer` | `System.Windows.Forms.Timer?` | `null` | 重试计时器。当 Windows 通知发出但用户未点击时，每 60 秒重新触发一次通知。<br>当用户点击确认或新通知替换旧通知时被停止和释放。 |

#### 4.2.2 Form1_FormClosing — 新增重试计时器清理 (L49-51)

```csharp
// L49-51: 在退出时停止并释放重试计时器，防止后台继续触发
// 插入位置：尺寸防抖计时器清理之后、托盘图标清理之前
try { _retryTimer?.Stop(); } catch { }
try { _retryTimer?.Dispose(); } catch { }
```

**为什么放在这里**：与其他资源清理保持一致的位置。`_retryTimer` 是 Windows.Forms.Timer，依赖消息循环，在 `Application.Exit()` 之前必须释放，否则可能引发 `ObjectDisposedException`。

#### 4.2.3 SetupTrayIcon — 新增 BalloonTipClicked 事件订阅 (L91)

```csharp
// L91: 订阅托盘气泡通知的点击事件
// 当用户点击 Windows 通知栏弹出的气泡时触发
trayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;
```

**BalloonTipClicked vs BalloonTipClosed**：
- `BalloonTipClicked`：用户**主动点击**了气泡 → 确认识别
- `BalloonTipClosed`：气泡**超时消失**或用户点了关闭按钮 → 不视为确认

本次需求只需处理 `Clicked`，因为只有主动点击才算"确认提醒"。

#### 4.2.4 ShowWindowsNotification — 函数签名和逻辑重写 (L109-128)

**改动前**（旧版）：
```csharp
private void ShowWindowsNotification(string title)
{
    if (trayIcon != null)
    {
        trayIcon.BalloonTipTitle = "橘子记事本 - 提醒";
        trayIcon.BalloonTipText = $"「{title}」\n该笔记的提醒时间已到！";
        trayIcon.BalloonTipIcon = ToolTipIcon.Info;
        trayIcon.ShowBalloonTip(8000);
    }
}
```

**改动后**（新版）：
```csharp
// L109: 新增 noteIndex 参数 —— 笔记在 twtw 数组中的索引
private void ShowWindowsNotification(string title, int noteIndex)
{
    // L111: 记录当前等待确认的提醒索引
    //       如果之前有一个待确认的，新通知直接覆盖（旧的不再追踪）
    _pendingReminderIndex = noteIndex;

    // L113-119: 弹出 Windows 气泡通知
    // ShowBalloonTip(8000): 气泡显示 8 秒后自动消失
    if (trayIcon != null)
    {
        trayIcon.BalloonTipTitle = "橘子记事本 - 提醒";
        trayIcon.BalloonTipText = $"「{title}」\n该笔记的提醒时间已到！";
        trayIcon.BalloonTipIcon = ToolTipIcon.Info;
        trayIcon.ShowBalloonTip(8000);
    }

    // L121-127: 启动/重置重试计时器
    // _retryTimer?.Stop():   如果已有重试计时器在运行，先停止
    // _retryTimer?.Dispose(): 释放旧计时器资源
    // new Timer():           创建新计时器
    // Interval = 60000:      60,000 毫秒 = 1 分钟
    // Tick += RetryTimer_Tick: 订阅重试事件
    // Start():               开始倒计时
    _retryTimer?.Stop();
    _retryTimer?.Dispose();
    _retryTimer = new System.Windows.Forms.Timer();
    _retryTimer.Interval = 60000; // 1 分钟
    _retryTimer.Tick += RetryTimer_Tick;
    _retryTimer.Start();
}
```

**调用方变更**（因签名变化）：
- `ReminderTimer_Tick` L478：`ShowWindowsNotification(title, noteIndex);`
- `triggerPastReminders` L202：`ShowWindowsNotification(title, i);`
- `RetryTimer_Tick` L170：`ShowWindowsNotification(title, idx);`（重试时重新调用自己）

#### 4.2.5 TrayIcon_BalloonTipClicked — 新增事件处理器 (L130-157)

```csharp
// L130: 当用户点击 Windows 通知气泡时触发
private void TrayIcon_BalloonTipClicked(object? sender, EventArgs e)
{
    // L132: 防御性检查：如果没有待确认的提醒（_pendingReminderIndex == -1），直接返回
    if (_pendingReminderIndex < 0) return;

    // L134-137: 停止并释放重试计时器
    //          用户已确认，无需再重试 → 清理资源
    _retryTimer?.Stop();
    _retryTimer?.Dispose();
    _retryTimer = null;

    // L139-140: 取出待确认的笔记索引，并将 _pendingReminderIndex 重置为 -1
    //          先取值再重置，防止后续代码中状态不一致
    int idx = _pendingReminderIndex;
    _pendingReminderIndex = -1;

    // L142-146: 将对应笔记的 isNoticeEnabled 设为 false（禁用提醒）
    //          idx < twtw.isNoticeEnabled.Count 边界检查，防止数组越界
    if (idx < (twtw.isNoticeEnabled?.Count ?? 0))
    {
        twtw.isNoticeEnabled[idx] = false;
    }

    // L148-153: 将数据持久化到 tw.tw 文件
    //          JSON 序列化后写入 Application.StartupPath/tw.tw
    try
    {
        File.WriteAllText(
            Path.Combine(Application.StartupPath, "tw.tw"),
            JsonSerializer.Serialize(twtw)
        );
    }
    catch { }

    // L155-156: 显示 Toast 确认窗口
    //          "确认提醒成功" → 屏幕顶部居中，500ms 自动消失
    ToastForm.ShowToast("确认提醒成功");
}
```

**执行时序图**：

```
用户点击 Windows 气泡
    │
    ├─ 1. 检查 _pendingReminderIndex ≥ 0 ?
    │     ├─ 否 → return（无操作）
    │     └─ 是 ↓
    ├─ 2. 停止并释放 _retryTimer
    ├─ 3. 取出 idx，重置 _pendingReminderIndex = -1
    ├─ 4. twtw.isNoticeEnabled[idx] = false
    ├─ 5. 保存 twtw → tw.tw 文件
    └─ 6. ToastForm.ShowToast("确认提醒成功")
```

#### 4.2.6 RetryTimer_Tick — 新增重试逻辑 (L159-171)

```csharp
// L159: 重试计时器的 Tick 事件，每 60 秒触发一次
private void RetryTimer_Tick(object? sender, EventArgs e)
{
    // L161: 先停止计时器
    //      因为 ShowWindowsNotification 内部会重新创建和启动计时器
    _retryTimer?.Stop();

    // L162: 再次检查是否还有待确认的提醒
    //      理论上此时 _pendingReminderIndex 应该 ≥ 0
    if (_pendingReminderIndex < 0) return;

    // L164-167: 根据索引获取笔记标题
    int idx = _pendingReminderIndex;
    string title = (idx < (twtw.titles?.Length ?? 0))
        ? (twtw.titles[idx] ?? "提醒")   // 有标题用标题
        : "提醒";                          // 无标题用默认文字

    // L169-170: 重新调用 ShowWindowsNotification
    //          注意：这会重新创建 _retryTimer，实现循环重试
    ShowWindowsNotification(title, idx);
}
```

**重试循环机制**：

```
ShowWindowsNotification(title, idx)
    │
    ├─ 弹出气泡通知
    └─ 启动 _retryTimer (Interval=60000)
           │
           ├─ 用户点击气泡 → TrayIcon_BalloonTipClicked → 停止 _retryTimer → 结束
           │
           └─ 60 秒后无人点击 → RetryTimer_Tick
                   │
                   └─ 调用 ShowWindowsNotification(title, idx) ← 回到起点
```

#### 4.2.7 ReminderTimer_Tick — 通知分发逻辑修改 (L475-498)

**改动前** (旧版 L411-427)：
```csharp
if (method == 1)
    ShowWindowsNotification(title);       // 旧版签名：无 noteIndex
else
    ShowReminderDialog(title);

// 统一：立即禁用并保存
twtw.isNoticeEnabled[noteIndex] = false;
File.WriteAllText(..., JsonSerializer.Serialize(twtw));
```

**改动后** (新版 L475-498)：
```csharp
// L475-479: method == 1（Windows 通知）
//          不立即禁用，等待用户点击气泡确认
//          ShowWindowsNotification 内部启动重试计时器
if (method == 1)
{
    ShowWindowsNotification(title, noteIndex);
}
// L480-488: method == 2（对话框）
//           对话框是模态的，MessageBox.Show 会阻塞直到用户关闭
//           用户关闭对话框 = 已确认 → 立即禁用
else
{
    ShowReminderDialog(title);
    if (noteIndex < (twtw.isNoticeEnabled?.Count ?? 0))
    {
        twtw.isNoticeEnabled[noteIndex] = false;
    }
}

// L490-498: 只有对话框类型在此处保存文件
//           Windows 通知类型在 TrayIcon_BalloonTipClicked 中保存
if (method != 1)
{
    try
    {
        File.WriteAllText(
            Path.Combine(Application.StartupPath, "tw.tw"),
            JsonSerializer.Serialize(twtw)
        );
    }
    catch { }
}
```

**两种通知方式的对比**：

| 特性 | method=1 (Windows 通知) | method=2 (对话框) |
|------|-------------------------|-------------------|
| 通知方式 | NotifyIcon.BalloonTip（托盘气泡） | MessageBox（模态弹窗） |
| 是否阻塞代码 | 否（异步） | 是（同步阻塞） |
| 确认时机 | 用户点击气泡 | 用户关闭弹窗 |
| 禁用时机 | 点击确认后 | 弹窗关闭后 |
| 保存时机 | 点击确认后 | 弹窗关闭后 |
| 未确认处理 | 每 60 秒重试 | 无需（Modal 阻塞） |

#### 4.2.8 triggerPastReminders — 过期提醒处理修改 (L199-208)

**改动前** (旧版 L142-148)：
```csharp
if (method == 1)
    ShowWindowsNotification(title);     // 旧版无 noteIndex
else
    ShowReminderDialog(title);

twd.isNoticeEnabled[i] = false;         // 统一立即禁用
```

**改动后** (新版 L199-208)：
```csharp
// L199-202: method=1 不立即禁用，与 ReminderTimer_Tick 保持一致
if (method == 1)
{
    ShowWindowsNotification(title, i);
}
// L204-208: method=2 立即禁用（对话框已确认）
else
{
    ShowReminderDialog(title);
    twd.isNoticeEnabled[i] = false;
}
```

**特别注意**：`triggerPastReminders` 在 `Form1_Load` 的第 383 行调用，并且前面有 `await Task.Delay(2000)` 延迟 2 秒。这意味着如果有多条过期提醒且都是 Windows 通知方式，会依次创建通知——但由于 `ShowWindowsNotification` 内部会覆盖 `_pendingReminderIndex`，实际上只有**最后一条**过期提醒被追踪。前面的过期通知虽然弹出了气泡，但用户点击气泡时 `_pendingReminderIndex` 已经指向了最后一条。

**这是一个已知的边界情况**：如果用户有大量过期提醒，建议使用对话框方式（method=2）逐个确认。

---

## 五、完整交互流程图

```
┌──────────────────────────────────────────────────────────────┐
│                    提醒触发 (两种来源)                         │
│                                                              │
│  ① 定时器触发 ReminderTimer_Tick                              │
│  ② 启动时 triggerPastReminders (延迟 2 秒)                    │
└─────────────────────┬────────────────────────────────────────┘
                      │
                      ▼
              ┌───────────────┐
              │ 通知方式判断   │
              └───────┬───────┘
                      │
        ┌─────────────┴─────────────┐
        │                           │
        ▼                           ▼
  ┌──────────┐               ┌──────────┐
  │ method=1 │               │ method=2 │
  │ 气泡通知  │               │ 对话框   │
  └────┬─────┘               └────┬─────┘
       │                          │
       ▼                          │
  ShowWindowsNotification         │
  (title, noteIndex)              │
       │                          │
       ├─ 设置 _pendingReminderIndex = idx
       ├─ 弹出气泡 (8 秒自动消失)
       └─ 启动 _retryTimer (60 秒)
              │                   │
       ┌──────┴──────┐            │
       │             │            │
       ▼             ▼            ▼
   用户点击气泡   60 秒超时   用户关闭弹窗
       │             │            │
       ▼             │            ▼
TrayIcon_            │      twtw.isNotice
BalloonTipClicked    │      Enabled[idx]
       │             │      = false
       ├─ 停止重试    │            │
       ├─ 禁用提醒    │            ▼
       ├─ 保存数据    │      保存到 tw.tw
       └─ ToastForm   │            │
          "确认提醒    │            ▼
          成功"       │      从 timers 列表
          (0.5秒)     │      移除 timer
                      │
                      ▼
              RetryTimer_Tick
                      │
                      └─ 重新调用
                         ShowWindowsNotification
                         (回到气泡通知)
```

---

## 六、变量速查表

### Form1.cs 新增/修改的变量

| 变量 | 类型 | 作用域 | 初始值 | 用途 |
|------|------|--------|--------|------|
| `_pendingReminderIndex` | `int` | 实例字段 | `-1` | 当前待确认提醒的笔记索引；`-1`=无待确认 |
| `_retryTimer` | `System.Windows.Forms.Timer?` | 实例字段 | `null` | 未确认提醒的重试计时器（60 秒周期） |

### Form1.cs 已有变量（本次改动涉及）

| 变量 | 类型 | 用途 |
|------|------|------|
| `trayIcon` | `NotifyIcon?` | 系统托盘图标，承载气泡通知 |
| `trayMenu` | `ContextMenuStrip?` | 托盘右键菜单 |
| `twtw` | `twdata` | 全局数据对象，包含所有笔记和提醒状态 |
| `timers` | `List<System.Windows.Forms.Timer>` | 所有活跃的提醒计时器列表 |
| `timersId` | `List<int>` | 与 timers 对应的笔记索引列表 |
| `_isActuallyExiting` | `bool` | 标记是否为真正退出（用于区分"最小化到托盘"和"退出程序"） |

### twdata 中提醒相关字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `titles` | `string[]` | 所有笔记标题 |
| `isNoticeEnabled` | `List<bool>` | 每条笔记是否启用提醒 |
| `taskNoticeType` | `List<int>` | 提醒类型：1=绝对时间，2=倒计时 |
| `tasksNoticeTime` | `List<DateTime>` | 绝对时间类型的目标时间 |
| `taskNoticeTime2` | `List<TimeSpan>` | 倒计时类型的时长 |
| `tasksNoticeMethod` | `List<int>` | 通知方式：1=Windows 气泡通知，2=对话框 |
| `taskNoticeCfg1` | `List<int>` | 配置项 1（预留） |
| `taskNoticeCfg2` | `List<int>` | 配置项 2（预留） |

### ToastForm 变量

| 变量 | 类型 | 用途 |
|------|------|------|
| `_closeTimer` | `System.Windows.Forms.Timer` | 500ms 后关闭窗口的计时器 |

---

## 七、方法签名变更

### ShowWindowsNotification

```diff
- private void ShowWindowsNotification(string title)
+ private void ShowWindowsNotification(string title, int noteIndex)
```

**影响范围**：所有调用该方法的地方都需要传入 `noteIndex` 参数。

### 新增方法

| 方法 | 签名 | 用途 |
|------|------|------|
| `TrayIcon_BalloonTipClicked` | `(object? sender, EventArgs e)` | 处理气泡点击事件 |
| `RetryTimer_Tick` | `(object? sender, EventArgs e)` | 处理重试计时器 Tick |
| `ToastForm.ShowToast` | `static void (string message)` | 静态快捷入口 |

---

## 八、注意事项 / 已知边界情况

1. **多条过期提醒同时触发**：`triggerPastReminders` 在循环中依次调用 `ShowWindowsNotification`，只有**最后一条**会被 `_pendingReminderIndex` 追踪。前面的过期通知气泡会被后续调用覆盖，用户点击时可能对应错误的笔记。建议对大量过期提醒使用对话框方式（method=2）。

2. **重试计时器泄漏**：确保 `Form1_FormClosing` 中清理了 `_retryTimer`，正常退出不会有泄漏。

3. **ToastForm 内存**：每次调用 `ShowToast` 创建新的 `ToastForm` 实例。窗口关闭（`Close()`）后会触发 `FormClosed` 事件并释放 `_closeTimer`，GC 会回收该实例。无泄漏风险。

4. **气泡通知的系统限制**：Windows 气泡通知一次只能显示一个。如果 `ShowBalloonTip` 在旧气泡消失前再次调用，新气泡会替换旧气泡。这与本设计的"新通知覆盖旧通知"行为一致。

---

## 九、构建结果

```
已成功生成。
    51 个警告（均为已有警告，非本次引入）
    0 个错误
```

输出路径：`橘子记事本\bin\Debug\net10.0-windows\橘子记事本.dll`

（以上为第三轮构建结果。第四轮构建同样 0 错误通过。）

---

## 十、twdata.cs 类型重构：string[] → List\<string\>

### 涉及文件
- `橘子记事本/twdata.cs`

### 改动内容

**第 6-8 行**：

```diff
- public string[] titles { get; set; }
- public string[] notes { get; set; }
- public string[] tasks { get; set; }
+ public List<string> titles { get; set; }
+ public List<string> notes { get; set; }
+ public List<string> tasks { get; set; }
```

### 原因

原 `string[]` 是固定长度数组，JSON 反序列化时如果数据和预期不符容易出错。改用 `List<string>` 后：
1. 与项目中其他提醒相关字段（`tasksNoticeMethod`、`taskNoticeType` 等）类型统一为 `List<T>`
2. 数据一致性检查更简单（所有字段都是 `.Count` 而非混用 `.Length` / `.Count`）
3. JSON 序列化/反序列化对 `List<T>` 支持更好

### 影响范围

| 文件 | 改动 |
|------|------|
| `Form1.cs` | `.Length` → `.Count`、`.ToArray()` → 直接赋值或 `.ToList()` |
| `NoticeSettingForm.cs` | 无影响（不直接引用 titles/notes/tasks 属性） |

---

## 十一、Form1.cs 适配 List\<string\> + 启动数据一致性检查

### 11.1 .Length → .Count 替换

共修改 **7 处**，涉及变量 `titles` 和 `notes` 的数组长度访问：

| 原代码 | 新代码 | 位置 |
|--------|--------|------|
| `twtw.titles?.Length ?? 0` | `twtw.titles?.Count ?? 0` | L165, 449, 468, 824 |
| `twd.titles?.Length ?? 0` | `twd.titles?.Count ?? 0` | L195 |
| `twtw?.notes?.Length ?? 0` | `twtw?.notes?.Count ?? 0` | L825 |

**为什么**：`List<T>` 使用 `.Count` 而非 `.Length`。

### 11.2 .ToArray() → 直接赋值 / .ToList()

共修改 **4 处**：

**保存笔记时（L680-681）**：
```diff
- twtw.titles = titlesList.ToArray();
- twtw.notes = notesList.ToArray();
+ twtw.titles = titlesList;
+ twtw.notes = notesList;
```
`titlesList` 和 `notesList` 已经是 `List<string>`，直接赋值即可，无需转换。

**删除笔记时（L980-981）**：
```diff
- twtw.titles = twtw.titles.Where((_, index) => index != selectedNoteId).ToArray();
- twtw.notes = twtw.notes.Where((_, index) => index != selectedNoteId).ToArray();
+ twtw.titles = twtw.titles.Where((_, index) => index != selectedNoteId).ToList();
+ twtw.notes = twtw.notes.Where((_, index) => index != selectedNoteId).ToList();
```
`Where()` 返回 `IEnumerable<string>`，需要用 `.ToList()` 转为 `List<string>` 赋值。

### 11.3 preparetw() 初始化扩展

```diff
  twdata preparetw(twdata twnull)
  {
+     twnull.titles = new List<string>();
+     twnull.notes = new List<string>();
+     twnull.tasks = new List<string>();
      twnull.tasksNoticeMethod = new List<int>();
      ...
  }
```

之前 `titles`/`notes`/`tasks` 是 `string[]`，JSON 序列化时 new twdata + 空数组也能工作。改为 `List<string>` 后必须显式初始化，否则 `JsonSerializer.Serialize` 对 null 属性的行为不一致。

### 11.4 新增：CheckDataConsistency() 启动数据一致性检查

**位置**：Form1.cs，`preparetw()` 方法之后。

**调用点**：`Form1_Load` 中反序列化成功后、`refreshTimers()` 之前（第 327-336 行）。

```csharp
// 启动时检查数据文件完整性：所有 List 的 Count 必须一致
if (!CheckDataConsistency(twtw))
{
    MessageBox.Show(
        "数据文件出错，程序即将关闭。",
        "橘子记事本 - 数据错误",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
    _isActuallyExiting = true;
    this.Close();
    return;
}
```

**CheckDataConsistency 方法详解**：

```csharp
private bool CheckDataConsistency(twdata twd)
```

检查以下 List 的 Count 是否全部等于笔记数量：

| 被检查的 List | 说明 |
|---------------|------|
| `titles` | 笔记标题 |
| `notes` | 笔记正文 |
| `tasksNoticeMethod` | 提醒方式 |
| `taskNoticeType` | 计时类型 |
| `taskNoticeCfg1` | 配置项 1 |
| `taskNoticeCfg2` | 配置项 2 |
| `tasksNoticeTime` | 绝对时间 |
| `isNoticeEnabled` | 是否启用 |
| `taskNoticeTime2` | 倒计时时长 |

**逻辑流程**：

```
入参 twd
  │
  ├─ twd == null ? → 返回 false
  │
  ├─ titles.Count ≠ notes.Count ? → 返回 false
  │
  ├─ titles.Count == 0 (无笔记) ? → 返回 true (无需查提醒列表)
  │
  └─ 依次检查 7 个提醒相关 List 的 Count == titles.Count
       │
       ├─ 全部一致 → 返回 true
       └─ 任一不一致 → 返回 false
```

**如果检查失败**：弹出对话框 → "数据文件出错，程序即将关闭" → 用户点确定 → `_isActuallyExiting = true` → `this.Close()` → `Application.Exit()`。

---

## 十二、NoticeSettingForm.cs 条件验证

### 12.1 改动概述

将 `NoticeSettingForm_FormClosing` 从"无条件验证"改为"根据提醒启用状态决定验证策略"。

### 12.2 验证策略

```
用户关闭提醒设置窗口
  │
  ├─ twr.isNoticeEnabled[nid] == false (未启用)
  │     └─ 允许随意设定，不验证
  │          ├─ 绝对时间：直接保存
  │          └─ 倒计时：尝试保存，溢出则设置默认值 5 分钟
  │
  └─ twr.isNoticeEnabled[nid] == true (已启用)
        └─ 严格验证
             ├─ 绝对时间：dateTimePicker1.Value < DateTime.Now ?
             │     ├─ 是 → 弹窗 "不能早于当前时间" + 阻止关闭 (e.Cancel=true)
             │     └─ 否 → 保存
             │
             └─ 倒计时：
                   ├─ 构造 TimeSpan 溢出 ?
                   │     └─ 是 → 弹窗 "倒计时太长" + 阻止关闭
                   ├─ ts == TimeSpan.Zero ?
                   │     └─ 是 → 弹窗 "不能为 0" + 阻止关闭
                   └─ 否 → 保存
```

### 12.3 三个验证对话框

| 触发条件 | 对话框文本 | 标题 | 按钮 | 行为 |
|----------|-----------|------|------|------|
| 绝对时间 < 现在 | "设置的提醒时间不能早于当前时间，请重新设置。" | "时间设置错误" | OK | 阻止关闭 |
| 倒计时溢出 | "倒计时太长，请减小时间数值。" | "倒计时设置错误" | OK | 阻止关闭 |
| 倒计时 = 0 | "倒计时不能为 0，请重新设置。" | "倒计时设置错误" | OK | 阻止关闭 |

### 12.4 未启用时的兜底

```csharp
try
{
    twr.taskNoticeTime2[nid] = new TimeSpan(
        (int)hourNumeric.Value, (int)minuteNumeric.Value, (int)secNumeric.Value);
}
catch (ArgumentOutOfRangeException)
{
    // 未启用时不阻止用户关闭，但设置一个安全默认值
    twr.taskNoticeTime2[nid] = TimeSpan.FromMinutes(5);
}
```

**为什么用 5 分钟作为默认值**：用户后续如果启用提醒，5 分钟的倒计时比 0 或超长值更合理，且不会意外触发"为 0"或"太长"的验证。

---

## 十三、数据一致性检查边界情况

| 场景 | 行为 |
|------|------|
| twtw == null（反序列化失败） | `CheckDataConsistency` 返回 false，程序报错退出 |
| titles.Count != notes.Count | 直接返回 false |
| 笔记数 = 0（新建/清空） | 跳过提醒列表检查，返回 true（空文件是合法的） |
| 笔记数 = 5，但某个提醒 List 只有 3 项 | 返回 false，数据被判定为损坏 |
| 某个 List 为 null | `?.Count ?? 0` 返回 0，与笔记数不匹配 → 返回 false |

---

## 十四、第四次构建结果

```
已成功生成。
    51 个警告（均为已有警告，非本次引入）
    0 个错误
```

输出路径：`橘子记事本\bin\Debug\net10.0-windows\橘子记事本.dll`

---

## 十五、提醒启用双重验证：通知方式 (method) 检查

### 15.1 需求

用户设置提醒时，通知方式（`tasksNoticeMethod`）是一个下拉选择：
- `-1` = 未选择
- `1` = Windows 气泡通知
- `2` = 对话框

之前只检查了计时类型（`taskNoticeType != -1`），未检查通知方式。如果用户仅设置了时间但未选择通知方式，勾选启用提醒的 CheckBox 后仍然会启用，导致通知行为不确定。

### 15.2 改动位置

**文件**：`橘子记事本/Form1.cs`
**方法**：`noticeCb_CheckedChanged`（第 807-835 行）
**位置**：第 812 行

### 15.3 改动内容

```diff
- if (twtw.taskNoticeType[noticeId] != -1)
+ if (twtw.taskNoticeType[noticeId] != -1 && twtw.tasksNoticeMethod[noticeId] != -1)
```

### 15.4 完整逻辑

```csharp
private void noticeCb_CheckedChanged(object sender, EventArgs e, int noticeId)
{
    CheckBox cbt = sender as CheckBox ?? new CheckBox { Checked = false };

    if (cbt.Checked)  // 用户勾选了启用提醒
    {
        // ★ 双重验证：计时类型 ≠ -1 且 通知方式 ≠ -1
        if (twtw.taskNoticeType[noticeId] != -1 &&
            twtw.tasksNoticeMethod[noticeId] != -1)
        {
            twtw.isNoticeEnabled[noticeId] = true;   // 全部满足 → 启用
        }
        else
        {
            cbt.Checked = false;   // 任一不满足 → 回退勾选
            return;                // 不保存、不刷新
        }
    }
    else  // 用户取消了勾选 → 禁用提醒
    {
        // 取消勾选只需要 type 已设置（method 不需要检查）
        if (twtw.taskNoticeType[noticeId] != -1)
        {
            twtw.isNoticeEnabled[noticeId] = false;
        }
        else
        {
            return;   // type 都没设置，什么都不做
        }
    }

    // 只有状态改变时才保存并刷新
    File.WriteAllText(..., JsonSerializer.Serialize(twtw));
    refreshTimers(ref timers, twtw);
}
```

### 15.5 逻辑流程图

```
用户勾选 CheckBox (启用提醒)
  │
  ├─ taskNoticeType == -1 ?
  │     └─ 是 → cbt.Checked=false, return (未设置计时类型)
  │
  └─ taskNoticeType != -1
        │
        ├─ tasksNoticeMethod == -1 ?
        │     └─ 是 → cbt.Checked=false, return (未选择通知方式)
        │
        └─ tasksNoticeMethod != -1
              └─ isNoticeEnabled = true → 保存 → refreshTimers

用户取消勾选 (禁用提醒)
  │
  ├─ taskNoticeType == -1 ? → return (未设置任何东西)
  └─ taskNoticeType != -1 → isNoticeEnabled = false → 保存 → refreshTimers
```

### 15.6 为什么取消勾选不需要检查 method

禁用提醒时只需知道"之前是否为这条笔记设置了提醒"。`taskNoticeType` 是设置提醒的第一步（选择"绝对时间"还是"倒计时"），只要 type 已设置，就说明用户曾配置过提醒。此时将 `isNoticeEnabled` 设为 false 是安全的，不需要关注 method 是否为 -1。

### 15.7 与 NoticeSettingForm 的配合

`NoticeSettingForm` 中，新增笔记时 `tasksNoticeMethod` 默认为 `-1`（见 Form1.cs L713）：
```csharp
twtw.tasksNoticeMethod.Add(-1); // 初始化，否则后续会报错null
```

用户必须进入提醒设置窗口（右键 CheckBox → NoticeSettingForm），在下拉框中选择通知方式，`tasksNoticeMethod[nid]` 才会变为 1 或 2。选择后即可在列表页勾选 CheckBox 启用提醒。

---

## 十六、第五轮构建结果

```
已成功生成。
    11 个警告（均为已有警告，非本次引入）
    0 个错误
```

输出路径：`橘子记事本\bin\Debug\net10.0-windows\橘子记事本.dll`

> 注：exe 主文件被运行中的进程（PID 23536）锁定，仅 DLL 编译成功。关闭运行实例后完整构建即可。
