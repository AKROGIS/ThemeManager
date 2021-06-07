using ESRI.ArcGIS;
using System; 

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    internal partial class LicenseInitializer
  {
    public LicenseInitializer()
    {
      ResolveBindingEvent += BindingArcGISRuntime;
    }

    void BindingArcGISRuntime(object sender, EventArgs e)
    {
        if (RuntimeManager.Bind(ProductCode.EngineOrDesktop))
            return;

            //  ProductCode[] supportedRuntimes = new[]
            //{ 
            //  ProductCode.Desktop,
            //  ProductCode.Engine,
            //};
            //foreach (ProductCode c in supportedRuntimes)
            //{
            //  if (RuntimeManager.Bind(c))
            //    return;
            //}

            // TODO: Remove dependency on Win Forms
            System.Windows.Forms.MessageBox.Show("Unable to find ArcGIS v10.  Some features will not work correctly.");
    }
  }
}