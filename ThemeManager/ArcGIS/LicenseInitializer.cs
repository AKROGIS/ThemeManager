using System;
using System.Windows.Forms;
using ESRI.ArcGIS;

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

      MessageBox.Show("Unable to find ArcGIS v10.  Some features will not work correctly.");
    }
  }
}