using Sunny.UI.Win32;
using System.Diagnostics;
using static 橘子记事本.Form1;

namespace 橘子记事本
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex mutex = new Mutex(
                true,
            "TANGERINE_TWRITER_SINGLE_INSTANCE",
            out bool createdNew
            );
            if (!createdNew)
            {
                NativeMethods.PostMessage(
                    NativeMethods.HWND_BROADCAST,
                    SingleInstance.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);

                return;
            }
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            if (!File.Exists(Path.Combine(Application.StartupPath, "tw.tw")))
            {
                if (MessageBox.Show("欢迎使用橘子记事本，橘子记事本自带加密功能，同意以下内容以开始使用橘子记事本\n免责声明：数据如果泄露，与橘子记事本及其开发者和贡献者无关\n数据安全不可忽视，请用你信任的软件存储你的保密数据\n数据如果泄露，与橘子记事本及其开发者和贡献者无关", "欢迎", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                    Application.Exit();
                    Environment.Exit(0);
                    return;
                }
            }
            Application.Run(new Form1());            
        }
    }
}