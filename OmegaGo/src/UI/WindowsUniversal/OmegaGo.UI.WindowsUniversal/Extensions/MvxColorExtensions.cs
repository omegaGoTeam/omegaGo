using MvvmCross.Platform;
using MvvmCross.Platform.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace OmegaGo.UI.WindowsUniversal.Extensions
{
    public static class MvxColorExtensions
    {
        public static Color ToUWPColor(this MvxColor color)
        {
            return Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);
        }
    }
}
