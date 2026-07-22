using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using 橘子记事本;
using System.Reflection;
using System.Runtime.InteropServices;

namespace UI测试
{
    [TestClass]
    public class Test_UI
    {
        // Helper to run an action on an STA thread and wait for completion.
        private bool RunInStaThread(Action action, int timeoutMs = 5000)
        {
            Exception? ex = null;
            bool completed = false;
            Thread t = new Thread(() =>
            {
                try
                {
                    ApplicationConfiguration.Initialize();
                    action();
                }
                catch (Exception e)
                {
                    ex = e;
                }
                finally
                {
                    completed = true;
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
            bool finished = t.Join(timeoutMs);
            if (!finished)
            {
                try { t.Abort(); } catch { }
            }
            if (ex != null) throw new AggregateException(ex);
            return completed && finished;
        }

       

        [TestMethod]
        public void checkBox1or2_test()
        {
            bool ok = RunInStaThread(() =>
            {
                var tw = new twdatav1();
                var nf = new NoticeSettingForm(ref tw, 0);
                var ntype = nf.GetType();
                var cb1 = ntype.GetField("checkBox1_1", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(nf) as CheckBox;
                var cb2 = ntype.GetField("checkBox1_2", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(nf) as CheckBox;
                Assert.IsNotNull(cb1);
                Assert.IsNotNull(cb2);

                // sequence 1: check cb2 then check cb1 -> cb2 should be unchecked
                cb2.Checked = true;
                cb1.Checked = true;
                Assert.IsFalse(cb2.Checked, "cb2 should be unchecked after cb1 checked");

                // sequence 2: check cb1 then check cb2 -> cb1 should be unchecked
                cb1.Checked = true;
                cb2.Checked = true;
                Assert.IsFalse(cb1.Checked, "cb1 should be unchecked after cb2 checked");

                nf.Dispose();
            });

            Assert.IsTrue(ok);
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const uint WM_CLOSE = 0x0010;

        [TestMethod]
        public void label1_inputbox_behavior_test()
        {
            bool ok = RunInStaThread(() =>
            {
                var tw = new twdatav1();
                var nf = new NoticeSettingForm(ref tw, 0);
                var ntype = nf.GetType();
                var cb1 = ntype.GetField("checkBox1_1", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(nf) as CheckBox;
                var cb2 = ntype.GetField("checkBox1_2", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(nf) as CheckBox;
                var label1 = ntype.GetField("label1", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(nf) as Label;
                Assert.IsNotNull(cb1); Assert.IsNotNull(cb2); Assert.IsNotNull(label1);

                // ensure both unchecked
                cb1.Checked = false; cb2.Checked = false;

                bool returnedQuickly = false;
                Thread t = new Thread(() =>
                {
                    // call private label1_Click
                    var m = ntype.GetMethod("label1_Click", BindingFlags.Instance | BindingFlags.NonPublic);
                    m.Invoke(nf, new object[] { label1, EventArgs.Empty });
                    returnedQuickly = true;
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                bool finished = t.Join(500); // expect quick return (no inputbox)
                Assert.IsTrue(finished && returnedQuickly, "label1_Click should return quickly when no checkbox is selected");

                // now select cb1 and call label1_Click again; this should open an InputBox and block
                cb1.Checked = true;
                bool blocked = false;
                Thread t2 = new Thread(() =>
                {
                    var m = ntype.GetMethod("label1_Click", BindingFlags.Instance | BindingFlags.NonPublic);
                    m.Invoke(nf, new object[] { label1, EventArgs.Empty });
                    // if returns, then InputBox was closed quickly
                });
                t2.SetApartmentState(ApartmentState.STA);
                t2.IsBackground = true;
                t2.Start();

                // wait briefly to see if the thread is blocked
                Thread.Sleep(500);
                if (t2.IsAlive) blocked = true;

                // if blocked, find the InputBox window by its title and close it to clean up
                if (blocked)
                {
                    IntPtr h = FindWindow(null, "输入时间");
                    if (h != IntPtr.Zero)
                    {
                        PostMessage(h, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }
                }

                // wait for thread to finish
                t2.Join(2000);

                Assert.IsTrue(blocked, "label1_Click should block (show InputBox) when a checkbox is selected");

                nf.Dispose();
            });

            Assert.IsTrue(ok);
        }

        [TestMethod]
        public void AboutForm_button1_closes()
        {
            bool ok = RunInStaThread(() =>
            {
                var af = new AboutForm();
                // show the form so Close() has effect
                af.Show();

                var t = af.GetType();
                var m = t.GetMethod("button1_Click", BindingFlags.Instance | BindingFlags.NonPublic);
                // invoke the click handler
                m.Invoke(af, new object[] { null, EventArgs.Empty });

                // allow the message loop to process the close
                Application.DoEvents();

                // after Close(), the form should be not visible or disposed
                Assert.IsTrue(af.IsDisposed || !af.Visible, "AboutForm should be closed after button1_Click");

                try { if (!af.IsDisposed) af.Dispose(); } catch { }
            });

            Assert.IsTrue(ok);
        }
    }
}
