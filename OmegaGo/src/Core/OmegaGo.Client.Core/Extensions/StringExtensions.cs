using System.Globalization;

namespace OmegaGo.Core.Extensions
{
    static class StringExtensions
    {
        public static int AsInteger(this string text)
        {
                return int.Parse(text);
        }
        public static float AsFloat(this string text)
        {
            // We can refactor this later.
            return float.Parse(text, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
        }
    }
}
