using System;

namespace NPS.AKRO.ThemeManager.Properties
{
    class Settings
    {
        public static Default Default = new Default()
        {
            AgeInDays = 30,
        };
    }

    class Default
    {
        public int AgeInDays { get; set; }
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