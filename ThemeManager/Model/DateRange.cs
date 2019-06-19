// ReSharper disable All
// Properties are public to be accessed by a WinForm control

namespace NPS.AKRO.ThemeManager.Model
{
    struct Duration
    {
        public Duration(int days, string description)
        {
            Days = days;
            Description = description;
        }

        public int Days { get; }
        public string Description { get; }
    }
}
