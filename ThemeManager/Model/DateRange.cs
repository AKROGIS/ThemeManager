
namespace NPS.AKRO.ThemeManager.Model
{
    class Duration
    {
        public Duration(int days, string description)
        {
            Days = days;
            Description = description;
        }

        public int Days { get; set; }
        public string Description { get; set; }
    }
}
