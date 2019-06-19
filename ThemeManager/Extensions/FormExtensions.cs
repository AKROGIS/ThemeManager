using NPS.AKRO.ThemeManager.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NPS.AKRO.ThemeManager.Extensions
{
    public static class FormExtensions
    {
        public static T CommonInit<T>(this T form) where T : Form
        {
            if (form != null)
            {
                if (File.Exists(Settings.Default.AppIcon))
                {
                    try
                    {
                        form.Icon = new Icon(Settings.Default.AppIcon);
                    }
                    catch (Exception ex)
                    {
                        // oh well, I guess we are stuck with the default icon.
                        Debug.Print("Loading " + Settings.Default.AppIcon + " generated an exception: " + ex);
                    }
                }

            }
            return form;
        }
    }
}
