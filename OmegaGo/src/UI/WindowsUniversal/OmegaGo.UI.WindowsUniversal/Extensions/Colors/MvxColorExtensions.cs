using Windows.UI;
using MvvmCross.Platform.UI;

namespace OmegaGo.UI.WindowsUniversal.Extensions.Colors
{
    public static class MvxColorExtensions
    {
        public static Color ToUWPColor(this MvxColor color)
        {
            return Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);
        }
    }
}
