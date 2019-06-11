using System;

namespace NPS.AKRO.ThemeManager.Properties
{
    class Settings
    {
        public static Default Default = new Default()
        {
            AgeInDays = 30,
            KeepMetaDataInMemory = false,
            CheckForArcViewBeforeArcInfo = false,
        };
    }

    class Default
    {
        public int AgeInDays { get; set; }
        public bool KeepMetaDataInMemory { get; set; }
        public bool CheckForArcViewBeforeArcInfo { get; set; }
    }

}


namespace NPS.AKRO.ThemeManager.UI.Forms
{
    class LoadingForm
    {
        public string Message { get; set; }
        public string Path { get; set; }
        public string Command { get; set; }

        public string LoadMetadataWithCatalog { get; set; }
        public string LoadLicense { get; set; }

        public void ShowDialog()
        {
            throw new NotImplementedException();
        }
    }
}

namespace NPS.AKRO.ThemeManager.Model
{
    internal class StyleSheet
    {
        public string TransformText(string xmlString)
        {
            throw new NotImplementedException();
        }
    }
}