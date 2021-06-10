using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    class EsriLicense
    {
        private static AoInitialize _license = null;

        internal static bool IsRunning => _license != null;

        internal static string Message { get; private set; }

        internal static bool Start()
        {
            if (!IsRunning)
            {
                //FIXME: asking for Basic when you have advanced fails and visa versa
                _license = GetLicense(ProductCode.Desktop, esriLicenseProductCode.esriLicenseProductCodeAdvanced);
            }
            return IsRunning;
        }

        internal static void Stop()
        {
            if (IsRunning)
            {
                _license.Shutdown();
                _license = null;
                Message = null;
            }
        }

        private static AoInitialize GetLicense(ProductCode product, esriLicenseProductCode level)
        {
            AoInitialize aoInit = null;
            try
            {
                Trace.TraceInformation($"Obtaining {product}-{level} license");
                RuntimeManager.Bind(product);
                aoInit = new AoInitialize();
                esriLicenseStatus licStatus = aoInit.Initialize(level);
                Message = $"Ready with license.  Status: {licStatus}";
                Trace.TraceInformation(Message);
            }
            catch (Exception ex)
            {
                Stop();
                // Set Message after stop, because stop sets message to null
                Message = $"Fatal Error: {ex.Message}";
                return null;
            }
            return aoInit;
        }

        // Convenience method for internal classes
        internal static async Task GetLicenseAsync()
        {
            if (!IsRunning)
            {
                GisInterface.ProgressorMessage = "Getting ArcObjects License...";
                await Task.Run(() => { Start(); });
                GisInterface.ProgressorMessage = null;
            }
            if (!IsRunning)
                throw new Exception("Could not initialize an ArcGIS license. \n" + Message);
        }

    }
}
