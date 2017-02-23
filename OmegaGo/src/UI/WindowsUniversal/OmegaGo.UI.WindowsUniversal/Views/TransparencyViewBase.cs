using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    /// <summary>
    /// Views derived from this View Base get transparent background at runtime
    /// This ensures they still are usable in designer
    /// </summary>
    public abstract class TransparencyViewBase : ViewBase
    {
        public TransparencyViewBase()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.Loading += TransparencyViewBase_Loading;
            }
        }

        /// <summary>
        /// Currently handled are Grid and Border
        /// The logic is more complicated because the elements don't share a common base class
        /// </summary>
        private void TransparencyViewBase_Loading(Windows.UI.Xaml.FrameworkElement sender, object args)
        {
            var transparentBackground = new SolidColorBrush(Colors.Transparent);
            if (Content is Grid)
            {
                ((Grid)Content).Background = transparentBackground;
            }
            else if (Content is Border)
            {
                ((Border)Content).Background = transparentBackground;
            }         
        }
    }
}
