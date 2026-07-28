using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/// <summary>
/// 可同步滚动的 RichTextBox。
///
/// 用途：
///     让两个 RichTextBox 的垂直滚动保持一致。
///
/// 原理：
///     1. 监听 Windows 的滚动消息（WndProc）
///     2. 获取当前控件第一可见行
///     3. 获取 Partner（同步对象）的第一可见行
///     4. 计算相差多少行
///     5. 让 Partner 滚动相同的行数
///
/// 作者：ChatGPT（可自行修改）
/// </summary>
public class SyncRichTextBox : RichTextBox
{
    #region Windows 消息常量

    /// <summary>
    /// WM_VSCROLL
    ///
    /// Windows发送的"垂直滚动"消息。
    ///
    /// 当发生以下情况时都会收到：
    ///     - 拖动滚动条
    ///     - 点击滚动条上下按钮
    ///     - PageUp/PageDown
    ///     - 键盘上下方向键
    /// </summary>
    private const int WM_VSCROLL = 0x115;

    /// <summary>
    /// WM_MOUSEWHEEL
    ///
    /// 鼠标滚轮滚动时发送。
    /// </summary>
    private const int WM_MOUSEWHEEL = 0x20A;

    #endregion

    #region RichEdit 消息常量

    /// <summary>
    /// 获取第一可见行。
    ///
    /// 返回值：
    ///     当前窗口最上方正在显示的是第几行。
    ///
    /// 例如：
    ///
    ///     第0行
    ///     第1行
    ///     第2行
    ///     ↓↓↓↓↓
    ///     第100行 ← 屏幕最上面
    ///
    /// 那么返回100。
    /// </summary>
    private const int EM_GETFIRSTVISIBLELINE = 0x00CE;

    /// <summary>
    /// 让 RichTextBox 滚动指定的行数。
    ///
    /// 参数：
    ///     正数 = 向下滚
    ///     负数 = 向上滚
    /// </summary>
    private const int EM_LINESCROLL = 0x00B6;

    #endregion

    #region Win32 API

    /// <summary>
    /// 向 Windows 控件发送消息。
    ///
    /// RichTextBox 本质就是 Windows RichEdit 控件，
    /// 很多功能只能通过 SendMessage 调用。
    /// </summary>
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(
        IntPtr hWnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam);

    #endregion

    #region 同步对象

    /// <summary>
    /// 与当前 RichTextBox 同步滚动的对象。
    ///
    /// Designer 不显示，也不会写入 Designer.cs。
    ///
    /// 示例：
    ///
    /// editor.Partner = lineNumber;
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SyncRichTextBox? Partner { get; set; }

    #endregion

    #region 防止递归同步

    /// <summary>
    /// 是否正在同步。
    ///
    /// 为什么需要？
    ///
    /// 假设：
    ///
    /// A滚动
    /// ↓
    /// B同步滚动
    /// ↓
    /// B又触发滚动
    /// ↓
    /// A继续同步
    /// ↓
    /// 无限循环……
    ///
    /// 所以同步期间把它设为 true，
    /// 收到滚动消息时直接忽略。
    /// </summary>
    private bool _syncing;

    #endregion

    #region Windows 消息处理
    private const int WM_USER = 0x0400;

    private const int EM_GETSCROLLPOS = WM_USER + 221;
    private const int EM_SETSCROLLPOS = WM_USER + 222;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(
        IntPtr hWnd,
        int msg,
        IntPtr wParam,
        ref POINT lParam);
    /// <summary>
    /// Windows 所有消息都会经过这里。
    ///
    /// RichTextBox 没有公开 Scroll 事件，
    /// 所以只能拦截 Windows 消息。
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        // 先让 RichTextBox 自己处理消息。
        base.WndProc(ref m);

        // 如果：
        // ① 收到了滚动消息
        // ② 当前不是同步产生的滚动
        // ③ 存在 Partner
        //
        // 就同步 Partner。
        if ((m.Msg == WM_VSCROLL || m.Msg == WM_MOUSEWHEEL)
            && !_syncing
            && Partner != null)
        {
            SyncPartner();
        }
    }

    #endregion

    #region 同步 Partner

    /// <summary>
    /// 同步 Partner 的滚动位置。
    /// </summary>
    private void SyncPartner()
    {
        if (Partner == null || !Partner.IsHandleCreated)
            return;

        _syncing = true;
        Partner._syncing = true;

        try
        {
            POINT pt = default;

            // 获取自己的像素滚动位置
            SendMessage(
                this.Handle,
                EM_GETSCROLLPOS,
                IntPtr.Zero,
                ref pt);

            // 设置 Partner 的像素滚动位置
            SendMessage(
                Partner.Handle,
                EM_SETSCROLLPOS,
                IntPtr.Zero,
                ref pt);

            // 立即刷新，避免部分系统拖动时延迟
            Partner.Invalidate();
        }
        finally
        {
            Partner._syncing = false;
            _syncing = false;
        }
    }
    #endregion
}