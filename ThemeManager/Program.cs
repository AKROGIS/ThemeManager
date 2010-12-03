using System;

using System.Threading;
using System.Windows.Forms;
using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.UI.Forms;

namespace NPS.AKRO.ThemeManager
{
    static class Program
    {
        // Introduce a Mutex, to enforce a single instance application.
        // code from http://sanity-free.org/143/csharp_dotnet_single_instance_application.html
        static Mutex mutex = new Mutex(true, "{2FE4D2B8-6C44-4B80-BBB2-0901B57E603B}");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm().CommonInit());
                mutex.ReleaseMutex();
            }
            else
            {
                // send our Win32 message to make the currently running instance
                // jump on top of all the other windows
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
        }
    }
}
