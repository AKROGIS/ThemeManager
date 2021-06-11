using ArcGIS.Core.Hosting;
using System;
using System.Threading.Tasks;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    // Call ArcGIS.Core.Hosting.Host.Initialize() before constructing any objects from ArcGIS.Core
    // The following conditions must all be met to successfully Initialize CoreHost:
    //     • The process must be 64 bit (i.e. Build Settings of x64
    //     • The process COM threading model must be single threaded apartment. [STAThread]
    //       must be present on the entry point of the application
    //     • ArcGIS Pro must be installed on the host machine
    //     • An ArcGIS Pro license must be available
    // If Initialization fails, a System.Exception will be thrown. The message will contain the reason.
    // Note: An ArcGIS Pro license can either be checked out (i.e. disconnected) or
    //       the 'sign me in automatically' check box is checked on the ArcGIS Pro login popup.
    //       To disconnect your license, run ArcGIS Pro, go to the Backstage, Licensing Tab.
    class EsriLicense
    {
        // True if app tried and failed to get a license
        private static bool _failed = false;
        // True if we have successfully got a license
        private static bool _license = false;

        internal static bool IsRunning => _license;

        internal static string Message { get; private set; }

        internal static bool Start()
        {
            if (!IsRunning & !_failed)
            {
                try
                {
                    Host.Initialize();
                    _license = true;
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    _license = false;
                    _failed = true;
                }
            }
            return IsRunning;
        }

        internal static void Stop()
        {
            if (IsRunning)
            {
                _license = false;
                _failed = true; // Allows the user to retry if things have changed
                Message = null;
            }
        }

        // Convenience method for internal classes
        internal static async Task GetLicenseAsync()
        {
            if (!IsRunning)
            {
                GisInterface.ProgressorMessage = "Checking for ArcGIS Pro License...";
                await Task.Run(() => { Start(); });
                GisInterface.ProgressorMessage = null;
            }
            if (!IsRunning)
                throw new Exception("Could not initialize an ArcGIS license. \n" + Message);
        }

    }
}
