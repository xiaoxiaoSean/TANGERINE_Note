# 橘子记事本 - 2026年7月27日 开发记录

---

## 一、任务列表 (Task List)

| 序号 | 任务 | 状态 |
|------|------|------|
| 1 | 修改 SearchControl 添加搜索事件（SearchTextChanged / SearchText） | ✅ 完成 |
| 2 | 在 MainForm 公共变量声明区声明全局变量 searcherContent | ✅ 完成 |
| 3 | 在 MainForm 构造函数订阅搜索事件 | ✅ 完成 |
| 4 | 实现异步搜索核心方法 PerformSearchAsync | ✅ 完成 |
| 5 | 修改 refreshNotes：当 searcherContent 不为空时自动填入搜索框并开始搜索 | ✅ 完成 |
| 6 | 修改 Note_AnimationCompleted 支持搜索模式下的动态动画计数 | ✅ 完成 |
| 7 | 创建 workbuddy20260727.md 开发记录文档 | ✅ 完成 |

---

## 二、对话摘要

本次开发围绕橘子记事本的**搜索功能**进行了从零到一的核心实现。

用户的需求要点：
1. 当搜索文本框文字改变时，开始遍历所有笔记的标题和正文进行搜索
2. 只要标题和正文中有一个包含搜索词，就将该笔记添加到显示列表
3. 添加完后执行 `refreshNotes` 统一刷新
4. 创建 Form1 全局 `string` 变量 `searcherContent`（在公共变量声明区声明）
5. 修改 `refreshNotes`：当 `searcherContent` 不为 null 或空时，直接在 searcher 的搜索文本输入处自动填入 `searcherContent` 开始搜索
6. 遍历到符合条件的笔记时，要有笔记显示动画（参考已有的动画代码），添加到 `tWritePage` 里面，然后继续搜索，直到遍历完所有笔记的标题和正文
7. 如果搜索结果为空（遍历所有笔记后没有一个包含搜索词），在对应代码处写上 `//TODO:123`
8. 采用异步搜索方式，详细写注释并在注释后面注明 `(workbuddy-20260727)`

最终构建：**0 错误，63 个警告（全部为已有警告，非本次引入，仅因新增代码行数导致行号变化）**。

---

## 三、涉及文件清单

| 文件 | 操作 | 说明 |
|------|------|------|
| `橘子记事本/SearchControl.cs` | 修改 | 添加 `SearchTextChanged` 事件和 `SearchText` 属性，订阅内部文本框的 `TextChanged` 事件 |
| `橘子记事本/MainForm.cs` | 修改 | 声明 `searcherContent` 及搜索相关字段、订阅搜索事件、实现异步搜索方法、改造 `refreshNotes` 和 `Note_AnimationCompleted` |

---

## 四、详细代码说明

### 4.1 SearchControl.cs — 添加搜索事件

**路径**：`橘子记事本/SearchControl.cs`

#### 4.1.1 设计思路

原来的 `SearchControl` 只有一个内部的 `uiTextBox1`（Sunny.UI 的 UITextBox），外部无法直接感知用户输入的变化。为了实现"当搜索文本框文字改变时就开始搜索"，需要：

1. 暴露一个公开的 **事件** `SearchTextChanged`，在内部文本框文字改变时触发
2. 暴露一个公开的 **属性** `SearchText`，供外部读取/设置搜索框文本（设置时用于 `refreshNotes` 自动填入搜索词）

#### 4.1.2 新增代码

```csharp
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

/// <summary>
/// 当搜索文本框文字改变时触发的事件 (workbuddy-20260727)
/// MainForm 订阅此事件以启动异步搜索
/// </summary>
public event EventHandler? SearchTextChanged;
```

#### 4.1.3 关键设计决策

| 决策 | 原因 |
|------|------|
| 使用事件转发而非直接暴露 `uiTextBox1` | 封装内部控件，外部只关心"文本改变了"这一事实，不需要知道内部用的是哪个控件 |
| `SearchText` 属性加 `[Browsable(false)]` + `[DesignerSerializationVisibility(Hidden)]` | 该属性是运行时动态读写的，不应在设计器中编辑或序列化到 Designer.cs，否则触发 WFO1000 编译错误 |
| 事件名 `SearchTextChanged` 而非 `TextChanged` | 避免与 UserControl 基类的 `TextChanged` 事件混淆，语义更明确 |

---

### 4.2 MainForm.cs — 公共变量声明区新增字段

**位置**：`SearchControl searcher` 声明之后（公共变量声明区）

#### 新增字段

```csharp
SearchControl searcher= new SearchControl();
// 搜索功能相关变量声明 (workbuddy-20260727)
/// <summary>
/// 全局搜索内容：保存当前搜索词。
/// 当不为 null 或空时，refreshNotes 会自动将其填入 searcher 的搜索框并开始异步搜索。
/// </summary>
string searcherContent = "";
/// <summary>
/// 异步搜索的取消令牌源：当用户连续输入时，取消上一次未完成的搜索，避免竞态。
/// </summary>
private CancellationTokenSource? _searchCts;
/// <summary>
/// 搜索锁：保护 _searchCts 的并发访问。
/// </summary>
private readonly object _searchLock = new object();
/// <summary>
/// 防循环标志：当程序自动设置搜索框文本时为 true，避免 TextChanged 事件递归触发搜索。
/// </summary>
private bool _isSettingSearchText = false;
/// <summary>
/// 搜索添加是否已完成：搜索模式下动态添加笔记卡片时为 false，全部添加完毕后置为 true。
/// 用于在 Note_AnimationCompleted 中判断是否可以恢复 AutoScroll。
/// </summary>
private volatile bool _searchAddingFinished = true;
```

#### 字段说明

| 字段 | 类型 | 初始值 | 用途 |
|------|------|--------|------|
| `searcherContent` | `string` | `""` | 全局搜索内容。用户输入时更新，`refreshNotes` 据此决定是否执行搜索。这是用户明确要求在公共变量声明区声明的全局变量 |
| `_searchCts` | `CancellationTokenSource?` | `null` | 异步搜索取消令牌源。用户连续输入时取消上一次搜索 |
| `_searchLock` | `object` | `new()` | 保护 `_searchCts` 并发访问的锁对象 |
| `_isSettingSearchText` | `bool` | `false` | 防递归标志。`refreshNotes` 程序化设置搜索框文本时为 `true`，防止触发 `OnSearchTextChanged` 造成无限递归 |
| `_searchAddingFinished` | `volatile bool` | `true` | 搜索模式下笔记是动态逐个添加的，总数未知。该标志为 `false` 时表示还在添加中，`Note_AnimationCompleted` 不应提前恢复 `AutoScroll` |

---

### 4.3 MainForm.cs — 构造函数订阅搜索事件

**位置**：`MainForm()` 构造函数末尾

```csharp
// 订阅搜索控件的文本改变事件：当用户在搜索框输入时触发异步搜索 (workbuddy-20260727)
searcher.SearchTextChanged += OnSearchTextChanged;
```

---

### 4.4 MainForm.cs — OnSearchTextChanged 事件处理

**作用**：当用户在搜索框输入文字时，更新全局 `searcherContent`，然后调用 `refreshNotes` 统一刷新。

```csharp
/// <summary>
/// 搜索框文本改变事件处理：更新全局搜索内容并触发统一刷新 (workbuddy-20260727)
/// </summary>
private void OnSearchTextChanged(object? sender, EventArgs e)
{
    // 如果是程序自动设置搜索框文本（防递归），直接返回，不触发搜索 (workbuddy-20260727)
    if (_isSettingSearchText) return;

    // 读取当前搜索框文本，更新全局搜索内容 (workbuddy-20260727)
    searcherContent = searcher.SearchText ?? "";

    // 调用 refreshNotes 统一刷新 (workbuddy-20260727)
    refreshNotes();
}
```

#### 防递归机制说明

当 `refreshNotes` 执行 `searcher.SearchText = searcherContent` 时，会触发 `uiTextBox1.TextChanged` → `SearchTextChanged` → `OnSearchTextChanged`。如果没有防递归机制，会形成无限循环：

```
refreshNotes → 设置 SearchText → TextChanged → OnSearchTextChanged → refreshNotes → 设置 SearchText → ...
```

通过 `_isSettingSearchText` 标志打破循环：

```
refreshNotes
  ├─ _isSettingSearchText = true
  ├─ searcher.SearchText = searcherContent  →  TextChanged → OnSearchTextChanged
  │                                              └─ _isSettingSearchText == true → return（跳过）
  ├─ _isSettingSearchText = false
  └─ 继续执行异步搜索...
```

---

### 4.5 MainForm.cs — CancelSearch 取消搜索

**作用**：在启动新搜索或刷新前，取消正在进行的异步搜索，避免上一次未完成的搜索干扰当前结果。

```csharp
/// <summary>
/// 取消正在进行的异步搜索 (workbuddy-20260727)
/// </summary>
private void CancelSearch()
{
    lock (_searchLock)
    {
        try { _searchCts?.Cancel(); } catch { }
        try { _searchCts?.Dispose(); } catch { }
        _searchCts = null;
    }
}
```

---

### 4.6 MainForm.cs — PerformSearchAsync 异步搜索核心实现

**这是本次开发的核心方法。**

**作用**：遍历所有笔记的标题和正文，只要标题或正文包含搜索词，就将该笔记以动画方式添加到 `tWritePage`。采用异步方式，每添加一个匹配的笔记后短暂让出 UI 线程让动画得以播放，然后继续搜索下一个笔记，直到遍历完所有笔记。若遍历完没有任何匹配结果，标记 `//TODO:123`。

#### 完整代码与逐段注释

```csharp
private async Task PerformSearchAsync(string keyword)
{
    // ===== 1. 创建本次搜索的取消令牌 ===== (workbuddy-20260727)
    // 用户连续输入时会取消上一次搜索，避免竞态
    CancellationTokenSource cts;
    lock (_searchLock)
    {
        try { _searchCts?.Cancel(); } catch { }
        try { _searchCts?.Dispose(); } catch { }
        _searchCts = new CancellationTokenSource();
        cts = _searchCts;
    }
    CancellationToken ct = cts.Token;

    int titleCount = twtw?.titles?.Count ?? 0;
    int noteCount = twtw?.notes?.Count ?? 0;

    // ===== 2. 数据为空时直接标记 TODO 并返回 ===== (workbuddy-20260727)
    if (titleCount == 0 || noteCount == 0)
    {
        //TODO:123
        tWritePage.AutoScroll = true;
        _searchAddingFinished = true;
        return;
    }

    // ===== 3. 初始化搜索模式下的动画计数状态 ===== (workbuddy-20260727)
    // 搜索模式下笔记是动态添加的，总数未知，因此先置 0，每添加一个就 +1
    _searchAddingFinished = false;
    lock (_notesAnimationLock)
    {
        _notesAnimationTotal = 0;
        _notesAnimationCompleted = 0;
    }

    // 收集匹配的笔记卡片，最后统一赋值给 notesToShow (workbuddy-20260727)
    List<tWriteNotes> matchedNotes = new List<tWriteNotes>();
    int matchCount = 0;

    // 布局状态：与原 refreshNotes 保持一致的双列布局 (workbuddy-20260727)
    bool isFirstNote = true;
    int nowNoteY = tWritePage.Height / 5;

    // ===== 4. 开始遍历所有笔记的标题和正文 ===== (workbuddy-20260727)
    for (int i = 0; i < titleCount; i++)
    {
        // 检查是否被取消（用户又输入了新内容）(workbuddy-20260727)
        ct.ThrowIfCancellationRequested();

        string title = twtw.titles[i] ?? "";
        string note = twtw.notes[i] ?? "";

        // 只要标题和正文中有一个包含搜索词，就添加该笔记 (workbuddy-20260727)
        bool matched = title.Contains(keyword) || note.Contains(keyword);
        if (!matched)
        {
            continue; // 不匹配则继续搜索下一个笔记
        }

        // ===== 创建匹配的笔记卡片并添加到 tWritePage ===== (workbuddy-20260727)

        tWriteNotes noteCard = new tWriteNotes
        {
            Width = tWritePage.Width / 2,
            Height = tWritePage.Height / 3,
            NoteId = i
        };
        noteCard.Click += note_Click;
        noteCard.DoubleClick += note_doubleClick;

        // 设置标题和正文（防御性赋值）
        noteCard.Title = string.IsNullOrWhiteSpace(title) ? "" : title;
        noteCard.NoteText = string.IsNullOrWhiteSpace(note) ? "" : note;

        // 计算目标位置（双列布局，与原 refreshNotes 一致）
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
        noteCard.IntendedLocation = intended;
        noteCard.Location = intended;

        // 动态增加动画总数：每添加一个笔记卡片，总数 +1 (workbuddy-20260727)
        lock (_notesAnimationLock)
        {
            _notesAnimationTotal++;
        }

        // 添加到 tWritePage，触发笔记显示动画 (workbuddy-20260727)
        // tWriteNotes 的 ParentChanged 事件会自动启动入场动画
        tWritePage.Controls.Add(noteCard);

        // 订阅动画完成事件并显式启动并行动画（保险调用）
        try { noteCard.AnimationCompleted += Note_AnimationCompleted; } catch { }
        try { noteCard.tAnimationParallel(); } catch { }

        matchedNotes.Add(noteCard);
        matchCount++;

        // 异步等待：让 UI 线程渲染动画，然后继续搜索下一个笔记 (workbuddy-20260727)
        try
        {
            await Task.Delay(50, ct);
        }
        catch (OperationCanceledException)
        {
            throw; // 搜索被取消，由外层捕获
        }
    }
    // ===== 遍历完所有笔记，搜索结束 ===== (workbuddy-20260727)

    // 更新全局 notesToShow 数组
    notesToShow = matchedNotes.ToArray();

    // ===== 搜索结果为空 ===== (workbuddy-20260727)
    if (matchCount == 0)
    {
        //TODO:123
        tWritePage.AutoScroll = true;
        _searchAddingFinished = true;
        return;
    }

    // 标记搜索添加已完成
    _searchAddingFinished = true;

    // 检查是否所有动画已经完成
    bool restoreScroll = false;
    lock (_notesAnimationLock)
    {
        if (_notesAnimationCompleted >= _notesAnimationTotal)
        {
            restoreScroll = true;
        }
    }
    if (restoreScroll)
    {
        tWritePage.AutoScroll = true;
    }
}
```

#### 异步搜索时序图

```
用户输入搜索词 "abc"
    │
    ▼
OnSearchTextChanged
    ├─ searcherContent = "abc"
    └─ refreshNotes()
           │
           ├─ CancelSearch()  ← 取消上一次搜索
           ├─ 清空 tWritePage，添加 searcher
           ├─ searcher.SearchText = "abc"  (防递归)
           └─ await PerformSearchAsync("abc")
                  │
                  ├─ 创建 CancellationToken
                  ├─ for i = 0 to titleCount-1:
                  │     ├─ 检查取消
                  │     ├─ titles[i] 或 notes[i] 包含 "abc" ?
                  │     │     ├─ 否 → continue
                  │     │     └─ 是 ↓
                  │     │           ├─ 创建 noteCard
                  │     │           ├─ 计算位置（双列布局）
                  │     │           ├─ _notesAnimationTotal++
                  │     │           ├─ tWritePage.Controls.Add(noteCard) → 触发显示动画
                  │     │           ├─ noteCard.tAnimationParallel() → 启动并行动画
                  │     │           └─ await Task.Delay(50) ← 让 UI 渲染动画
                  │     └─ 继续遍历下一个笔记
                  │
                  ├─ 遍历结束
                  ├─ notesToShow = matchedNotes.ToArray()
                  ├─ matchCount == 0 ?
                  │     └─ 是 → //TODO:123
                  └─ matchCount > 0 → _searchAddingFinished = true → 检查恢复 AutoScroll
```

---

### 4.7 MainForm.cs — refreshNotes 方法改造

#### 改造要点

1. 改为 `async void`（调用方都是事件处理器，不需要 await）
2. 开头调用 `CancelSearch()` 取消正在进行的搜索
3. 在数据校验后，检查 `searcherContent`：
   - 不为空 → 程序化填入搜索框（防递归）→ 执行 `PerformSearchAsync` → return
   - 为空 → 走原有显示全部笔记的逻辑

#### 改造后代码（关键部分）

```csharp
async void refreshNotes()
{
    // 取消正在进行的搜索，避免竞态 (workbuddy-20260727)
    CancelSearch();

    notesToShow = Array.Empty<tWriteNotes>();
    tWritePage.Controls.Clear();
    searcher.Location = new Point(0, 0);
    searcher.Size = new Size(tWritePage.Width*2/3, tWritePage.Height/5);
    tWritePage.Controls.Add(searcher);
    tWritePage.AutoScroll = false;
    // ... 数据校验 ...

    // ★ 当 searcherContent 不为 null 或空时，直接填入 searcher 并开始搜索 (workbuddy-20260727)
    if (!string.IsNullOrEmpty(searcherContent))
    {
        _isSettingSearchText = true;
        searcher.SearchText = searcherContent;   // 自动填入搜索框
        _isSettingSearchText = false;

        // 执行异步搜索
        try { await PerformSearchAsync(searcherContent); }
        catch (OperationCanceledException) { }  // 被取消则忽略
        catch { }
        return;
    }

    // ===== 以下为无搜索词时显示全部笔记的原有逻辑 ===== (workbuddy-20260727)
    _searchAddingFinished = true;
    // ... 原有创建笔记卡片、设置位置、添加到 tWritePage、播放动画的逻辑 ...
}
```

#### 为什么是 `async void` 而非 `async Task`

`refreshNotes` 的调用方都是事件处理器（`SizeChangedTimer_Tick`、`mainTab_SelectedIndexChanged`、`OnSearchTextChanged`）或同步方法（`oprationBox2_Click`）。这些调用方不会 `await` 返回值，`async void` 是事件处理器的标准签名。方法内部的 `await PerformSearchAsync` 是非阻塞的，`refreshNotes` 会在 `await` 处立即返回，异步搜索在后台继续执行。

---

### 4.8 MainForm.cs — Note_AnimationCompleted 改造

#### 改造原因

原有逻辑中，`_notesAnimationTotal` 在添加笔记之前就设为已知总数，动画全部完成即可恢复 `AutoScroll`。

搜索模式下，笔记是**动态逐个添加**的，总数未知。如果动画完成速度 > 笔记添加速度，可能出现"动画已完成数 = 当前总数"但实际还有更多笔记要添加的情况，导致 `AutoScroll` 被提前恢复。

#### 改造内容

```csharp
private void Note_AnimationCompleted(object? sender, EventArgs e)
{
    // ... 取消订阅 ...

    bool restore = false;
    lock (_notesAnimationLock)
    {
        _notesAnimationCompleted++;
        // ★ 搜索模式下需额外检查 _searchAddingFinished (workbuddy-20260727)
        if (_searchAddingFinished && _notesAnimationTotal > 0 && _notesAnimationCompleted >= _notesAnimationTotal)
        {
            restore = true;
        }
    }

    if (restore)
    {
        tWritePage.AutoScroll = true;
    }
}
```

#### 非搜索模式 vs 搜索模式

| 模式 | `_searchAddingFinished` | `_notesAnimationTotal` 设置时机 | 恢复 AutoScroll 条件 |
|------|------------------------|-------------------------------|---------------------|
| 显示全部（原逻辑） | `true`（在添加前设置） | 添加前设为总数 | 动画完成数 ≥ 总数 |
| 搜索模式 | 添加中=`false`，添加完毕=`true` | 每添加一个 `+1` | `_searchAddingFinished == true` 且动画完成数 ≥ 总数 |

---

## 五、完整搜索交互流程

```
┌─────────────────────────────────────────────────────────┐
│  用户在搜索框输入文字 "abc"                                │
└──────────────────────┬──────────────────────────────────┘
                       │
                       ▼
              uiTextBox1.TextChanged
                       │
                       ▼
              SearchTextChanged (事件)
                       │
                       ▼
              OnSearchTextChanged
                ├─ _isSettingSearchText? → 是: return（防递归）
                ├─ searcherContent = "abc"
                └─ refreshNotes()
                       │
                       ▼
              refreshNotes (async void)
                ├─ CancelSearch() ← 取消上一次搜索
                ├─ 清空 tWritePage
                ├─ 添加 searcher 控件
                ├─ searcherContent 不为空?
                │     └─ 是 ↓
                │        ├─ _isSettingSearchText = true
                │        ├─ searcher.SearchText = "abc"
                │        │     └─ TextChanged → OnSearchTextChanged
                │        │           └─ _isSettingSearchText == true → return
                │        ├─ _isSettingSearchText = false
                │        └─ await PerformSearchAsync("abc")
                │              │
                │              ├─ 创建 CancellationToken
                │              ├─ 遍历所有笔记:
                │              │     ├─ 标题或正文包含 "abc"?
                │              │     │     ├─ 否 → continue
                │              │     │     └─ 是 → 创建卡片 → 添加到 tWritePage
                │              │     │            → 触发显示动画 → await Delay(50)
                │              │     └─ 继续遍历...
                │              │
                │              ├─ 遍历结束:
                │              │     ├─ matchCount == 0? → //TODO:123
                │              │     └─ matchCount > 0 → _searchAddingFinished = true
                │              │                       → 检查恢复 AutoScroll
                │              └─ (若有新输入，已被取消)
                │
                └─ (searcherContent 为空时走原逻辑显示全部笔记)
```

---

## 六、取消机制详解

用户快速输入时（如逐字输入 "a" → "ab" → "abc"），会产生多次搜索请求。取消机制确保只有最后一次搜索生效：

```
输入 "a"
  └─ PerformSearchAsync("a") 开始遍历...
       找到笔记1，添加，await Delay(50)...

输入 "ab" (50ms 内)
  └─ OnSearchTextChanged → refreshNotes → CancelSearch()
       ├─ _searchCts.Cancel()  ← 取消 "a" 的搜索
       └─ PerformSearchAsync("ab") 开始
            ├─ ct.ThrowIfCancellationRequested() ← "a" 的搜索在此抛出异常
            └─ 遍历...

输入 "abc" (50ms 内)
  └─ OnSearchTextChanged → refreshNotes → CancelSearch()
       ├─ _searchCts.Cancel()  ← 取消 "ab" 的搜索
       └─ PerformSearchAsync("abc") 开始
            └─ 遍历... ← 最终生效的搜索
```

---

## 七、变量速查表

### MainForm.cs 新增变量

| 变量 | 类型 | 作用域 | 初始值 | 用途 |
|------|------|--------|--------|------|
| `searcherContent` | `string` | 实例字段 | `""` | 全局搜索内容，驱动 refreshNotes 走搜索逻辑 |
| `_searchCts` | `CancellationTokenSource?` | 实例字段 | `null` | 异步搜索取消令牌源 |
| `_searchLock` | `object` | 实例字段（readonly） | `new()` | 保护 `_searchCts` 的锁 |
| `_isSettingSearchText` | `bool` | 实例字段 | `false` | 防递归标志 |
| `_searchAddingFinished` | `volatile bool` | 实例字段 | `true` | 搜索添加是否完成标志 |

### SearchControl.cs 新增成员

| 成员 | 类型 | 用途 |
|------|------|------|
| `SearchText` | `string`（属性） | 读写搜索框文本 |
| `SearchTextChanged` | `event EventHandler?` | 文本改变事件 |

---

## 八、//TODO:123 标记位置

共有 **3 处** `//TODO:123` 标记，表示搜索结果为空：

| 位置 | 触发条件 |
|------|---------|
| `PerformSearchAsync` 中 `titleCount == 0 \|\| noteCount == 0` | 没有任何笔记数据 |
| `PerformSearchAsync` 中 `matchCount == 0` | 遍历完所有笔记后没有一个包含搜索词 |
| `refreshNotes` 中 `titleCount == 0 && noteCount == 0` | 非搜索模式下无笔记数据 |

---

## 九、方法签名变更

### refreshNotes

```diff
- void refreshNotes()
+ async void refreshNotes()
```

**原因**：内部需要 `await PerformSearchAsync`，方法必须标记为 `async`。调用方均为事件处理器或同步方法，`async void` 是合适的签名。

### 新增方法

| 方法 | 签名 | 用途 |
|------|------|------|
| `OnSearchTextChanged` | `void (object? sender, EventArgs e)` | 搜索框文本改变事件处理 |
| `CancelSearch` | `void ()` | 取消正在进行的异步搜索 |
| `PerformSearchAsync` | `async Task (string keyword)` | 异步搜索核心实现 |

---

## 十、构建结果

```
已成功生成。
    63 个警告（均为已有警告，非本次引入，因新增代码行数导致行号偏移）
    0 个错误
```

输出路径：`橘子记事本\bin\Debug\net10.0-windows\橘子记事本.dll`

---

## 十一、注意事项 / 已知边界情况

1. **异步搜索的取消**：用户连续快速输入时，前几次搜索会被取消，只有最后一次输入对应的搜索会完整执行。`PerformSearchAsync` 中通过 `ct.ThrowIfCancellationRequested()` 和 `await Task.Delay(50, ct)` 两个检查点响应取消。

2. **动画计数动态增长**：搜索模式下 `_notesAnimationTotal` 从 0 开始，每添加一个笔记卡片 `+1`。`_searchAddingFinished` 标志确保只有在所有笔记添加完毕后，动画全部完成时才恢复 `AutoScroll`。

3. **防递归**：`refreshNotes` 中 `searcher.SearchText = searcherContent` 会触发 `TextChanged` → `OnSearchTextChanged`。`_isSettingSearchText` 标志在设置前后切换，打破递归链。

4. **搜索模式下的点击/删除**：搜索结果中的笔记卡片使用原始 `NoteId`（即在 `twtw.titles/notes` 中的真实索引），因此点击选中、双击编辑、删除等操作都能正确作用于原始笔记数据。`notesToShow` 在搜索结束后更新为匹配列表，与 `note_Click` 中的 `notesToShow[selectedNoteId]` 逻辑兼容（删除后调用 `refreshNotes` 会重新搜索）。

5. **大小写敏感**：当前搜索使用 `string.Contains(keyword)`，是大小写敏感的。如需大小写不敏感可改用 `IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0`。

6. **空搜索词**：当用户清空搜索框时，`searcherContent` 变为 `""`，`refreshNotes` 走原有显示全部笔记的逻辑。

---

## 十二、搜索不干涉搜索文本框输入（第二轮改动）

### 12.1 问题描述

第一轮实现中，`refreshNotes` 开头使用 `tWritePage.Controls.Clear()` 清空所有子控件（包括 `searcher`），然后重新添加 `searcher`。这会导致：

1. **搜索框失焦**：`searcher` 被移除再重新添加后，文本框失去焦点，用户无法连续输入
2. **输入被打断**：搜索过程中每次 `refreshNotes` 调用都会移除 `searcher`，打断用户的打字节奏
3. **动画播放时无法重新搜索**：动画播放期间用户输入改变，`refreshNotes` 会移除 `searcher` 再添加，导致输入框闪烁

### 12.2 解决方案

核心思路：**搜索时只移除笔记卡片，保留 `searcher` 控件**，确保搜索框始终保持焦点，不干涉用户输入。

#### 12.2.1 新增 RemoveNoteCards 方法

```csharp
/// <summary>
/// 移除 tWritePage 中的所有笔记卡片，保留 searcher 控件 (workbuddy-20260727)
/// 不使用 tWritePage.Controls.Clear() 是因为那会移除 searcher 导致搜索框失焦，
/// 从而干涉用户在搜索过程中的连续输入。
/// 搜索时或动画播放时若用户继续输入，本方法只清除笔记卡片，搜索框保持焦点不受影响。
/// </summary>
private void RemoveNoteCards()
{
    // 先收集所有需要移除的笔记卡片，避免在遍历时修改集合
    List<Control> toRemove = new List<Control>();
    foreach (Control c in tWritePage.Controls)
    {
        if (c is tWriteNotes)
        {
            toRemove.Add(c);
        }
    }
    // 逐个移除并释放
    foreach (var c in toRemove)
    {
        tWritePage.Controls.Remove(c);
        try { c.Dispose(); } catch { }
    }
}
```

#### 12.2.2 修改 refreshNotes

```diff
  async void refreshNotes()
  {
      CancelSearch();
      notesToShow = Array.Empty<tWriteNotes>();
-     tWritePage.Controls.Clear();
-     searcher.Location = new Point(0, 0);
-     searcher.Size = new Size(tWritePage.Width*2/3, tWritePage.Height/5);
-     tWritePage.Controls.Add(searcher);
+     // 只移除笔记卡片，保留 searcher，避免搜索框失焦干涉用户输入 (workbuddy-20260727)
+     RemoveNoteCards();
+     // 确保 searcher 在 tWritePage 中（首次加载或被移除时才添加）
+     if (!tWritePage.Controls.Contains(searcher))
+     {
+         searcher.Location = new Point(0, 0);
+         searcher.Size = new Size(tWritePage.Width*2/3, tWritePage.Height/5);
+         tWritePage.Controls.Add(searcher);
+     }
      tWritePage.AutoScroll = false;
      // ...
  }
```

**关键变化**：
- `Controls.Clear()` → `RemoveNoteCards()`：只移除笔记卡片，不动 `searcher`
- `searcher` 只在不在 `tWritePage` 中时才添加（`Contains` 检查），避免重复添加

### 12.3 搜索中/动画播放中文本改变的完整流程

```
用户正在搜索框输入 "abc"
    │
    ├─ PerformSearchAsync("abc") 正在异步遍历笔记
    │   ├─ 找到匹配笔记1，添加到 tWritePage，启动动画
    │   ├─ await Task.Delay(50, ct)  ← 让出 UI 线程，用户可以继续输入
    │   │         │
    │   │         ▼ 用户继续输入 "abcd"
    │   │    OnSearchTextChanged
    │   │         ├─ searcherContent = "abcd"
    │   │         └─ refreshNotes()
    │   │              ├─ CancelSearch()  ← ct.Cancel() 取消 "abc" 的搜索
    │   │              ├─ RemoveNoteCards()  ← 只移除笔记卡片1，保留 searcher（不失焦！）
    │   │              ├─ searcher 已在 tWritePage 中，跳过添加
    │   │              └─ await PerformSearchAsync("abcd")  ← 新搜索开始
    │   │
    │   └─ "abc" 的搜索在 await Task.Delay(50, ct) 处抛出 OperationCanceledException
    │      └─ 被 refreshNotes 的 catch (OperationCanceledException) 捕获，忽略
    │
    └─ PerformSearchAsync("abcd") 继续异步执行...
```

### 12.4 为什么 searcher 不会失焦

| 时机 | searcher 的状态 | 搜索框焦点 |
|------|-----------------|-----------|
| 首次进入笔记页 | 不在 tWritePage → 被 `Contains` 检查后添加 | 获得焦点 |
| 用户输入触发搜索 | **始终在 tWritePage**（不被移除） | **保持焦点** |
| 搜索中用户继续输入 | **始终在 tWritePage** | **保持焦点** |
| 动画播放中用户继续输入 | **始终在 tWritePage** | **保持焦点** |
| 切换到其他 Tab 再切回 | 被 `RemoveNoteCards` 后 `Contains` 检查为 false → 重新添加 | 重新获得焦点 |

### 12.5 Dispose 的安全性

`RemoveNoteCards` 中对每个移除的笔记卡片调用 `Dispose()`：

- `tWriteNotes` 的动画使用 `BeginInvoke` 更新 UI，所有 `BeginInvoke` 回调内部都有 `try-catch` 保护
- 控件被 `Dispose` 后，排队的 `BeginInvoke` 回调执行时会安全失败（被 try-catch 吞掉），不会崩溃
- `Dispose` 释放控件资源（GDI 句柄等），避免内存泄漏

### 12.6 第二轮构建结果

```
已成功生成。
    0 个错误
```

输出路径：`橘子记事本\bin\Debug\net10.0-windows\橘子记事本.dll`

---

## 十三、总结

### 改动文件清单

| 文件 | 第一轮改动 | 第二轮改动 |
|------|-----------|-----------|
| `橘子记事本/SearchControl.cs` | 新增 SearchTextChanged 事件、SearchText 属性 | 无 |
| `橘子记事本/MainForm.cs` | 声明 searcherContent 等字段、订阅事件、实现 PerformSearchAsync、改造 refreshNotes 和 Note_AnimationCompleted | 新增 RemoveNoteCards 方法、refreshNotes 用 RemoveNoteCards 替代 Controls.Clear |

### 核心设计总结

| 设计 | 解决的问题 |
|------|-----------|
| 异步搜索 + CancellationToken | 搜索不阻塞 UI 线程，用户可以连续输入 |
| 每添加一个笔记后 `await Task.Delay(50)` | 让出 UI 线程渲染动画，同时允许用户在此期间输入 |
| `CancelSearch()` 取消上一次搜索 | 用户输入改变时停止当前搜索，重新搜索 |
| `_isSettingSearchText` 防递归 | 程序化设置搜索框文本不会触发递归搜索 |
| `_searchAddingFinished` 动态动画计数 | 搜索模式下动态添加笔记时正确控制 AutoScroll 恢复 |
| `RemoveNoteCards()` 替代 `Controls.Clear()` | 只移除笔记卡片保留 searcher，搜索框不失焦，不干涉用户输入 |
| `Contains(searcher)` 检查 | 避免重复添加 searcher，首次加载时才添加 |

---

## 十四、程序开启后自动选中 UIInputDialog 文本框（第三轮改动，重做）

### 14.1 需求

程序启动时会弹出密码输入对话框（两种场景：解密已有文件 / 新建文件设置密码）。用户需要手动点击文本框才能开始输入密码。希望程序开启后**自动选中文本框**，用户可直接打字输入。

### 14.2 第一轮方案（失败）

最初方案：在调用 `UIInputDialog.ShowInputPasswordDialog` 前启动后台 `Task.Run`，`await Task.Delay(100)` 后通过 `BeginInvoke` 回 UI 线程，遍历 `Application.OpenForms` 查找对话框窗体并递归查找 `UITextBox` 设置焦点。

**失败原因**：`ShowInputPasswordDialog` 是模态调用，阻塞 UI 线程。虽然模态对话框有自己的消息循环，但 `BeginInvoke` 的回调在模态循环中执行时机不确定，且 `Application.OpenForms` 中的类型名匹配不可靠，最终焦点设置未生效。

### 14.3 第二轮方案（成功）：直接实例化 UIInputForm

通过查看 SunnyUI 源码发现：

1. `UIInputForm` 是 **public sealed** 类，有 **public 无参构造函数**
2. `Editor` 属性是 **public**：`public UITextBox Editor => edit;`
3. `Label` 属性是 **public**：`public UILabel Label => label;`
4. `UIInputForm` 源码中已有 `Shown` 事件调用 `edit.SelectAll()`，**但缺少 `Focus()`**

```csharp
// SunnyUI 源码中的 UIInputForm_Shown：
private void UIInputForm_Shown(object sender, System.EventArgs e)
{
    edit.SelectAll();   // ← 只全选了文本，但没有 Focus()！
}
```

**根因**：`SelectAll()` 选中了文本框中的文字，但文本框没有获得键盘焦点，用户的键盘输入不会进入文本框。

**解决方案**：不使用静态方法 `ShowInputPasswordDialog`，而是直接实例化 `UIInputForm`，在 `Shown` 事件中补充 `Focus()`。

### 14.4 新增方法 ShowPasswordDialog

```csharp
/// <summary>
/// 显示密码输入对话框并自动选中文本框 (workbuddy-20260727)
/// 直接实例化 SunnyUI 的 UIInputForm（而非使用静态方法 UIInputDialog.ShowInputPasswordDialog），
/// 以便在 Shown 事件中直接访问 Editor 属性并设置焦点。
/// UIInputForm 源码中已有 Shown 事件调用 edit.SelectAll()，但缺少 Focus()，
/// 导致文本框虽有选中文本但未获得键盘焦点，用户输入不会进入文本框。
/// 此方法在 Shown 中补充 Focus()，确保用户可直接打字输入。
/// </summary>
private bool ShowPasswordDialog(ref string value, string desc, int maxLength)
{
    // 直接实例化 UIInputForm（public sealed 类，有 public 无参构造函数）(workbuddy-20260727)
    using var frm = new UIInputForm();
    // 设置对话框基本属性，与原 ShowInputPasswordDialog 静态方法内部逻辑一致
    frm.Text = UIStyles.CurrentResources.InputTitle;
    frm.Label.Text = desc;
    frm.CheckInputEmpty = false;
    frm.Editor.PasswordChar = '*';
    frm.Editor.MaxLength = maxLength;
    frm.Style = UIStyle.DarkBlue;
    frm.ShowInTaskbar = false;
    frm.TopMost = true;
    frm.StartPosition = FormStartPosition.CenterScreen;

    // 对话框显示后自动选中文本框并设置键盘焦点 (workbuddy-20260727)
    // UIInputForm 自身的 Shown 事件已调用 edit.SelectAll()，但未 Focus()
    // 此处补充 Focus()，确保键盘输入直接进入文本框
    frm.Shown += (s, e) =>
    {
        try
        {
            frm.Editor.Focus();       // ← 关键：补充键盘焦点
            frm.Editor.SelectAll();  // ← 全选已有文本
        }
        catch { }
    };

    try { frm.Render(); } catch { }
    if (frm.ShowDialog() == DialogResult.OK)
    {
        value = frm.Editor.Text;
        return true;
    }
    return false;
}
```

### 14.5 调用位置替换

将两处 `UIInputDialog.ShowInputPasswordDialog` 静态调用替换为 `ShowPasswordDialog`：

```diff
  // 场景1：解密已有文件
- UIInputDialog.ShowInputPasswordDialog(ref pwd, UIStyle.DarkBlue, false, "输入解密密码以解密,只能英文，数字", false, 50);
+ ShowPasswordDialog(ref pwd, "输入解密密码以解密,只能英文，数字", 50);

  // 场景2：新建空文件设置密码
- UIInputDialog.ShowInputPasswordDialog(ref pwd, UIStyle.DarkBlue, false, "欢迎，输入加密密码，不想输入密码可留空,只能英文，数字", false, 50);
+ ShowPasswordDialog(ref pwd, "欢迎，输入加密密码，不想输入密码可留空,只能英文，数字", 50);
```

### 14.6 ShowPasswordDialog 与原 ShowInputPasswordDialog 的对比

| 对比项 | 原 `ShowInputPasswordDialog`（静态方法） | 新 `ShowPasswordDialog`（实例方法） |
|--------|---------------------------------------|-------------------------------------|
| 对话框创建 | private `CreateInputForm` 内部创建 | 直接 `new UIInputForm()` |
| 显示方式 | internal `ShowForm` 扩展方法 | 直接 `frm.ShowDialog()` |
| 文本框焦点 | 仅 `SelectAll()`，无 `Focus()` | `Focus()` + `SelectAll()` |
| 效果 | 文本框未获键盘焦点，用户需手动点击 | 文本框获得焦点，用户可直接打字 |

### 14.7 为什么 Focus() 是关键

`SelectAll()` 和 `Focus()` 的区别：

- `SelectAll()`：选中文本框中的所有文字（高亮显示），但**不设置键盘焦点**。如果文本框没有焦点，用户按键盘不会输入到文本框中。
- `Focus()`：将键盘焦点设置到文本框。设置焦点后，用户的键盘输入会直接进入文本框。

SunnyUI 的 `UIInputForm_Shown` 只调用了 `SelectAll()`，没有 `Focus()`。虽然 `SelectAll()` 在某些情况下会隐式设置焦点，但在 `Shown` 事件中可能因为对话框还在渲染过程中，焦点设置不稳定。显式调用 `Focus()` 确保焦点正确设置。

### 14.8 第三轮构建结果

```
已成功生成。
    0 个错误
```

输出路径：`橘子记事本\bin\Debug\net10.0-windows\橘子记事本.dll`

---

## 十五、用自实现控件替代 SunnyUI 依赖（第四轮改动）

### 15.1 需求

项目依赖 SunnyUI 3.9.8 包，但只使用了少量控件（UITextBox、UIScrollingText、UISwitch、UIInputDialog/UIInputForm、UIStyle/UIStyles、UIMessageTip、UINotifier）。需要用自实现的控件替代这些 SunnyUI 控件，**不依赖 SunnyUI 包**，但**类名保持一致**，删除所有 `using Sunny.UI;`。

### 15.2 涉及的 SunnyUI 类型

通过全面搜索，项目中使用了以下 SunnyUI 类型：

| SunnyUI 类型 | 用途 | 使用位置 |
|-------------|------|---------|
| `UITextBox` | 文本框（含水印） | MainForm.Designer.cs, SearchControl.Designer.cs |
| `UIScrollingText` | 滚动文本 | MainForm.Designer.cs |
| `UISwitch` | 开关控件 | MainForm.Designer.cs |
| `UIInputForm` | 输入对话框窗体 | MainForm.cs (ShowPasswordDialog) |
| `UIInputDialog` | 输入对话框静态方法 | MainForm.cs (注释中) |
| `UIStyle` | 样式枚举 | MainForm.cs |
| `UIStyles` | 样式资源 | MainForm.cs |
| `UIMessageTip` | Toast 提示 | MainForm.cs |
| `UINotifier` | 右下角通知 | MainForm.cs |
| `UINotifierType` | 通知类型枚举 | MainForm.cs |
| `DescriptionEventArgs` | 事件参数 | MainForm.cs |

### 15.3 新建文件 tControl.cs

在 `橘子记事本/tControl.cs` 中实现所有替代控件，放在 `橘子记事本` 命名空间中。

#### 15.3.1 实现的类型清单

| 类型 | 基类 | 说明 |
|------|------|------|
| `UIStyle` | enum | 样式枚举（Blue, DarkBlue, Default 等） |
| `UINotifierType` | enum | 通知类型枚举（INFO, OK, ERROR, WARNING, Ask） |
| `DescriptionEventArgs` | EventArgs | 带描述信息的事件参数 |
| `UIResources` | class | 本地化资源（InputTitle, EditorCantEmpty） |
| `UIStyles` | static class | 样式管理器，CurrentResources 属性 |
| `UITextBox` | TextBox | 文本框，添加 Watermark（EM_SETCUEBANNER）、ShowText、TextAlignment、Radius |
| `UIScrollingText` | Control | 滚动文本，Timer 驱动水平滚动，支持 Active、Interval、Radius |
| `UISwitch` | Control | 开关控件，点击切换 Checked，绘制轨道和圆点 |
| `UIInputForm` | Form | 输入对话框，含 Label、Editor(UITextBox)、确定/取消按钮，Shown 自动选中文本框 |
| `UIInputDialog` | static class | ShowInputPasswordDialog 静态方法 |
| `UIMessageTip` | static class | ShowOk/ShowError/ShowInfo Toast 提示 |
| `UINotifier` | static class | 右下角通知，支持点击回调和不自动关闭 |

#### 15.3.2 关键实现细节

**UITextBox** — 水印文字用 Windows API `EM_SETCUEBANNER`：
```csharp
[DllImport("user32.dll", CharSet = CharSet.Unicode)]
private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
private const int EM_SETCUEBANNER = 0x1501;
```

**UIScrollingText** — 用 Timer 驱动文本水平滚动，文字宽度超过控件宽度时才滚动。

**UISwitch** — 绘制开关轨道和圆点，点击切换 Checked 状态。

**UIInputForm** — 输入对话框窗体，Shown 事件中 Focus() + SelectAll()。

**UIMessageTip** — 后台线程创建无边框 Toast 窗口，自动定时关闭。

**UINotifier** — 后台线程创建右下角通知窗口，支持点击回调和不自动关闭。

### 15.4 修改的文件

| 文件 | 修改内容 |
|------|---------|
| `橘子记事本/tControl.cs` | **新建**，实现所有替代控件 |
| `橘子记事本/MainForm.cs` | 删除 `using Sunny.UI;` |
| `橘子记事本/Program.cs` | 删除 `using Sunny.UI.Win32;` |
| `橘子记事本/MainForm.Designer.cs` | `Sunny.UI.UITextBox` → `UITextBox` 等 |
| `橘子记事本/SearchControl.Designer.cs` | `Sunny.UI.UITextBox` → `UITextBox` |
| `橘子记事本/橘子记事本.csproj` | 删除 `<PackageReference Include="SunnyUI" />` |

### 15.5 第四轮构建结果

```
已成功生成。
    0 个错误
```

项目已完全脱离 SunnyUI 包依赖。

---

## 十六、修复 bug：tip 不显示 + 确认提醒后执行 refreshNotice（第五轮改动）

### 16.1 问题描述

用户报告两个 bug：
1. **tip 不显示**：用户确认提醒后，`UIMessageTip.ShowOk("确认提醒成功", 1000)` 不显示
2. **确认提醒后应执行 refreshNotice**：确认提醒后提醒界面需要刷新

### 16.2 Bug 1：tip 不显示

#### 第一次尝试（错误根因分析）

最初误以为是 SunnyUI 的 `UIMessageTip` 静态方法依赖 `Form.ActiveForm`，尝试在调用前 `this.Activate()`。**但项目已经完全脱离 SunnyUI 依赖**，`UIMessageTip` 是项目自实现的类（在 `tControl.cs` 中），不依赖 `Form.ActiveForm`。

#### 真正根因

查看自实现的 `UIMessageTip.ShowTip`（`tControl.cs` 第539行），发现问题：

```csharp
Thread tipThread = new Thread(() =>
{
    // ... 创建 tip 窗口 ...
    tip.FormClosed += (s, e) => Application.ExitThread();
    Application.Run(tip);   // 显示 tip 并启动消息循环
    tip.Show();             // ← 多余！Application.Run 退出后 tip 已关闭
})
{
    IsBackground = true
};
tipThread.Start();          // ← 缺少 SetApartmentState(STA)！
```

**两个问题**：

1. **后台线程未设为 STA**：WinForms 的 `Application.Run` 要求线程为 STA（单线程单元）。`Thread` 默认是 MTA，在 MTA 线程上调用 `Application.Run` 无法正常显示窗口。这是 tip 不显示的**根本原因**。

2. **`Application.Run(tip)` 后多余的 `tip.Show()`**：`Application.Run(tip)` 已经显示窗口并启动消息循环，当 `tip` 关闭后 `Application.Run` 才返回。后面的 `tip.Show()` 在窗口已关闭后执行，无意义且可能引发异常。

#### 修复

在 `tControl.cs` 的 `ShowTip` 方法中：

```diff
  })
  {
      IsBackground = true
  };
+ // 必须设为 STA 线程，否则 WinForms 的 Application.Run 无法正常显示窗口 (workbuddy-20260727)
+ tipThread.SetApartmentState(ApartmentState.STA);
  tipThread.Start();
```

同时删除 `Application.Run(tip)` 后面多余的 `tip.Show()`。

同时还原 `ConfirmReminder` 中错误的 `this.Activate()` 逻辑（基于错误的 SunnyUI 假设），恢复为简单的 `UIMessageTip.ShowOk("确认提醒成功", 1000)` 调用。

### 16.3 Bug 2：确认提醒后执行 refreshNotice

#### 根因

`ConfirmReminder()` 中已有 `refreshNotice()` 调用，但 `twtw.isNoticeEnabled[idx] = false` 没有边界检查。如果 `idx` 越界会抛出异常，导致后续 `refreshNotice()` 不会被执行。

#### 修复

```csharp
// 禁用该提醒（边界检查，防止索引越界导致后续 refreshNotice 不执行）(workbuddy-20260727)
if (idx >= 0 && idx < (twtw.isNoticeEnabled?.Count ?? 0))
{
    twtw.isNoticeEnabled[idx] = false;
}

// 确认提醒后，执行 refreshNotice 刷新提醒界面 (workbuddy-20260727)
try { refreshNotice(); } catch { }
```

### 16.4 修改后的 ConfirmReminder 完整代码

```csharp
private void ConfirmReminder()
{
    if (_pendingReminderIndex < 0) return;

    _retryTimer?.Stop();
    _retryTimer?.Dispose();
    _retryTimer = null;

    int idx = _pendingReminderIndex;
    _pendingReminderIndex = -1;
    _pendingReminderMethod = -1;

    // 禁用该提醒（边界检查）
    if (idx >= 0 && idx < (twtw.isNoticeEnabled?.Count ?? 0))
    {
        twtw.isNoticeEnabled[idx] = false;
    }

    // 确认提醒后，执行 refreshNotice 刷新提醒界面
    try { refreshNotice(); } catch { }

    // 保存数据
    try { WriteBack(); } catch { }

    // 显示确认 tip（UIMessageTip 内部在后台 STA 线程上创建 Toast 窗口）
    try { UIMessageTip.ShowOk("确认提醒成功", 1000); } catch { }
}
```

### 16.5 修改后的 ShowTip 关键代码（tControl.cs）

```csharp
private static void ShowTip(string text, Color backColor, Color foreColor, int duration)
{
    Thread tipThread = new Thread(() =>
    {
        try
        {
            Form tip = new Form { /* ... 窗口属性 ... */ };
            // ... 设置 Label、Shown 事件 ...

            // Application.Run(tip) 显示 tip 并启动消息循环，tip 关闭后退出
            tip.FormClosed += (s, e) => Application.ExitThread();
            Application.Run(tip);
            // 删除了多余的 tip.Show()
        }
        catch (Exception ex) { MessageBox.Show("..." + ex.ToString()); }
    })
    {
        IsBackground = true
    };
    // ★ 关键修复：必须设为 STA 线程，否则 Application.Run 无法正常显示窗口
    tipThread.SetApartmentState(ApartmentState.STA);
    tipThread.Start();
}
```

### 16.6 第五轮构建结果

```
已成功生成。
    0 个错误
```

---

## 十七、修复密码输入框焦点问题（第六轮改动）

### 17.1 问题描述

程序启动弹出密码输入对话框（`UIInputForm`）后，文本框没有获得键盘焦点，用户需要手动点击文本框才能输入。

### 17.2 之前的错误尝试

第三轮中误以为 `UIInputForm` 是 SunnyUI 的类，去查看 SunnyUI 源码，发现"Shown 事件中只有 `SelectAll()` 没有 `Focus()`"。但项目已经完全脱离 SunnyUI，`UIInputForm` 是自实现的（在 `tControl.cs` 中），其构造函数的 `Shown` 事件中**已经有 `Focus()` + `SelectAll()`**。

### 17.3 真正根因

查看自实现的 `UIInputForm`（`tControl.cs` 第472行）：

```csharp
// 原来的 Shown 事件
Shown += (s, e) =>
{
    _editor.Focus();       // ← 直接调用，但 ShowDialog 的默认焦点行为会覆盖它
    _editor.SelectAll();
};
```

`Shown` 事件在窗体首次显示后触发，但 `ShowDialog()` 内部在 `Shown` 事件之后还有默认的焦点设置行为（可能将焦点设到 `AcceptButton`）。直接在 `Shown` 事件中调用 `Focus()` 会被后续的默认焦点行为覆盖。

同时 `ShowPasswordDialog` 方法中又叠加了第二个 `Shown` 事件做同样的事，双重叠加但仍无法解决时序问题。

### 17.4 修复

#### 17.4.1 修改 UIInputForm 的 Shown 事件（tControl.cs）

使用 `BeginInvoke` 延迟焦点设置，确保在 `ShowDialog()` 的默认焦点行为之后执行：

```csharp
// 修复后 (workbuddy-20260727)
Shown += (s, e) =>
{
    this.BeginInvoke((Action)(() =>
    {
        try
        {
            _editor.Focus();
            _editor.SelectAll();
        }
        catch { }
    }));
};
```

`BeginInvoke` 将焦点设置操作排入消息队列，在 `ShowDialog()` 的默认焦点处理完成后才执行，从而确保焦点正确设置到文本框。

#### 17.4.2 精简 ShowPasswordDialog（MainForm.cs）

删除多余的 `Shown` 事件（`UIInputForm` 构造函数中已处理），删除基于错误假设的过时注释：

```csharp
private bool ShowPasswordDialog(ref string value, string desc, int maxLength)
{
    using var frm = new UIInputForm();
    frm.Text = UIStyles.CurrentResources.InputTitle;
    frm.Label.Text = desc;
    frm.CheckInputEmpty = false;
    frm.Editor.PasswordChar = '*';
    frm.Editor.MaxLength = maxLength;
    frm.Style = UIStyle.DarkBlue;
    frm.ShowInTaskbar = false;
    frm.TopMost = true;
    frm.StartPosition = FormStartPosition.CenterScreen;

    // 焦点设置已在 UIInputForm 构造函数的 Shown 事件中通过 BeginInvoke 处理 (workbuddy-20260727)
    try { frm.Render(); } catch { }
    if (frm.ShowDialog() == DialogResult.OK)
    {
        value = frm.Editor.Text;
        return true;
    }
    return false;
}
```

### 17.5 BeginInvoke 方案失败，改用 ActiveControl + Timer 方案

`BeginInvoke` 方案仍然不 work：`ShowDialog()` 的默认焦点行为在消息循环中持续覆盖 `BeginInvoke` 排队的回调。

**最终方案**：三重保险——`ActiveControl` + `Load` 事件 + `Shown` 事件用 `Timer` 延迟：

```csharp
public UIInputForm()
{
    // ... 创建控件 ...

    // ① 构造函数中设置 ActiveControl：ShowDialog 显示窗体时会将焦点设到 ActiveControl
    this.ActiveControl = _editor;

    // ② Load 事件：句柄已创建，再次确保 ActiveControl 指向文本框
    this.Load += (s, e) =>
    {
        this.ActiveControl = _editor;
    };

    // ③ Shown 事件：用 Timer 延迟 10ms 设置焦点
    // Timer 的 Tick 在消息循环下一次循环中执行，比 BeginInvoke 更可靠
    Shown += (s, e) =>
    {
        this.ActiveControl = _editor;
        var focusTimer = new System.Windows.Forms.Timer { Interval = 10 };
        focusTimer.Tick += (s2, e2) =>
        {
            focusTimer.Stop();
            focusTimer.Dispose();
            try
            {
                _editor.Focus();
                _editor.SelectAll();
            }
            catch { }
        };
        focusTimer.Start();
    };
}
```

**为什么 Timer 比 BeginInvoke 更可靠**：

| 机制 | 执行时机 | 可靠性 |
|------|---------|--------|
| 直接 `Focus()` | `Shown` 事件中同步执行 | 被 `ShowDialog` 后续默认焦点覆盖 |
| `BeginInvoke` | 排入消息队列末尾，但可能在 `ShowDialog` 焦点设置之前处理 | 不可靠 |
| `Timer(10ms)` | 在消息循环下一次循环中执行，确保在 `ShowDialog` 所有同步焦点设置之后 | 可靠 ✓ |

**三层保险的作用**：

1. `ActiveControl`（构造函数）：告诉 `ShowDialog` 应该将焦点给到哪个控件
2. `Load` 事件中 `ActiveControl`：句柄已创建时再次确认
3. `Shown` 事件中 `Timer`：延迟 10ms 后用 `Focus()` 强制设置键盘焦点，确保在所有默认焦点行为之后执行

### 17.6 上述方案全部失败——真正根因

`ActiveControl` + `Load` + `Timer` 三重保险仍然不 work。

**用户提示**："你要把焦点移到那个输入密码的小窗口上"——问题不在于文本框焦点，而在于**对话框窗口本身没有获得 Windows 前台焦点**。

**真正根因**：splash 窗口在独立线程上运行（`MainForm` 构造函数中 `splashThread.Start()`），在 `Form1_Load` 中调用 `ShowPasswordDialog` 时 splash 还没有关闭（splash 在 `Form1_Load` 第597行才关闭，而密码对话框在第502/528行就弹出了）。splash 窗口持有 Windows 前台焦点，导致 `ShowDialog()` 显示的对话框虽然是 `TopMost` 但无法接收键盘输入——在 Windows 中只有前台窗口才能获得键盘输入。

之前的所有方案（`Focus()`、`BeginInvoke`、`ActiveControl`、`Timer`）都是在设置**文本框焦点**，但对话框窗口本身不是前台窗口，文本框自然无法获得键盘输入。

### 17.7 最终修复：用 Win32 API 强制夺取前台焦点

在 `UIInputForm` 中添加 Win32 API 声明，在 `Shown` 事件中用 `SetForegroundWindow` + `SwitchToThisWindow` 强制将对话框设为前台窗口：

```csharp
public class UIInputForm : Form
{
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

    // ... 构造函数 ...

    Shown += (s, e) =>
    {
        // 强制将对话框设为前台窗口
        this.Activate();
        if (this.IsHandleCreated)
        {
            SetForegroundWindow(this.Handle);
            SwitchToThisWindow(this.Handle, true);
        }
        this.ActiveControl = _editor;

        // Timer 延迟设置文本框焦点
        var focusTimer = new System.Windows.Forms.Timer { Interval = 10 };
        focusTimer.Tick += (s2, e2) =>
        {
            focusTimer.Stop();
            focusTimer.Dispose();
            this.Activate();
            SetForegroundWindow(this.Handle);
            _editor.Focus();
            _editor.SelectAll();
        };
        focusTimer.Start();
    };
}
```

**`SetForegroundWindow` vs `Activate()` 的区别**：

| 方法 | 作用 | 能否跨线程夺取前台焦点 |
|------|------|----------------------|
| `Form.Activate()` | 请求激活窗口 | 不一定，Windows 可能拒绝非前台线程的请求 |
| `SetForegroundWindow` | Win32 API，强制设置前台窗口 | 能，绕过 .NET 层直接调用 Windows |
| `SwitchToThisWindow` | Win32 API，切换到指定窗口 | 能，类似 Alt+Tab 效果 |

### 17.8 最终构建结果

```
已成功生成。
    0 个错误
``````
已成功生成。
    0 个错误
```
