using System.Threading.Tasks;
using System.Windows.Forms;

// The following issues result from supporting an expected public API
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    public class PreviewPage
    {
        private readonly Control _defaultControl;

        public PreviewPage(Control parent, Form form)
        {
            _defaultControl = parent.Controls[0];
        }

        public void ShowText(string text)
        {
            _defaultControl.Text = text;
        }

        public async Task ShowMapAsync(string path)
        {
            var msg = "Map preview is only available with ArcGIS 10.x installed.";
            msg += $"\nUnable to display {path}.";
            _defaultControl.Text = msg;
        }

    }
}
