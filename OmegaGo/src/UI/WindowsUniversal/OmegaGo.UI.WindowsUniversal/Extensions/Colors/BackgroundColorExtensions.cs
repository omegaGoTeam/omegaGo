using Windows.UI;
using OmegaGo.UI.Game.Styles;

namespace OmegaGo.UI.WindowsUniversal.Extensions.Colors
{
    public static class BackgroundColorExtensions
    {
        public static Color ToWindowsColor(this BackgroundColor color)
        {
            return Color.FromArgb(255, color.Red, color.Green, color.Blue);
        }
    }
}
