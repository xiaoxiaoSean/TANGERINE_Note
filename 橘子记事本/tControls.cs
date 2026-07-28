using System;
using System.Collections.Generic;
using System.Text;

namespace 橘子记事本
{
    internal class tControls
    {
    }
    public class SyncRichTextBox : RichTextBox //by ChatGPT
    {
        public event EventHandler? Scrolled;

        protected override void WndProc(ref Message m)
        {
            const int WM_VSCROLL = 0x115;
            const int WM_MOUSEWHEEL = 0x20A;

            base.WndProc(ref m);

            if (m.Msg == WM_VSCROLL || m.Msg == WM_MOUSEWHEEL)
            {
                Scrolled?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
