using System;
using System.Collections.Generic;
using System.Text;

namespace NPS.AKRO.ThemeManager.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string mainString, string subString, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(mainString) || string.IsNullOrEmpty(subString))
                return false;
            int index = mainString.IndexOf(subString, comparison);
            if (index == -1)
                return false;
            return true;
        }

        public static string Concat(this IEnumerable<string> collection, string joiner)
        {
            if (joiner == null)
                throw new ArgumentNullException("joiner");
            StringBuilder sb = new StringBuilder();
            foreach (string item in collection)
                sb.Append(item + joiner);
            if (joiner.Length > 0 && sb.Length > joiner.Length)
                sb.Remove(sb.Length - joiner.Length, joiner.Length);
            return sb.ToString();
        }
    }
}
