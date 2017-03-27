using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using OmegaGo.UI.Game.Styles;

namespace OmegaGo.UI.WindowsUniversal.Extensions.Colors
{
    public static class WindowsColorExtensions
    {
        public static BackgroundColor ToBackgroundColor(this Color color)
        {
            return new BackgroundColor(color.R, color.G, color.B);
        }
    }
}
