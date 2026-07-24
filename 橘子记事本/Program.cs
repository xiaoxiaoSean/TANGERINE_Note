using static 橘子记事本.Form1;

namespace 橘子记事本
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        static Mutex mutex;
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
            Application.Run(new Form1());
        }
    }
}