using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using MvvmCross.Core.ViewModels;
using OmegaGo.UI.WindowsUniversal.Infrastructure;

namespace OmegaGo.UI.WindowsUniversal.Helpers.UI
{
    /// <summary>
    /// Stores visual settings and provides them to the UI for binding purposes
    /// </summary>
    public class VisualSettings : MvxNotifyPropertyChanged
    {
        private Color _backgroundColor;
        private SolidColorBrush _backgroundColorBrush;
        private ElementTheme _elementTheme;

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
        }

        public Brush BackgroundColorBrush => _backgroundColorBrush ??
                                             (_backgroundColorBrush = new SolidColorBrush(BackgroundColor));

        public ElementTheme ElementTheme => _elementTheme;

        /// <summary>
        /// Updates visual settings bindings
        /// </summary>
        public void Refresh()
        {
            _backgroundColor = AppShell.GetForCurrentView().BackgroundColor;
            RaisePropertyChanged(() => BackgroundColor);

            _backgroundColorBrush = null;
            RaisePropertyChanged(()=>BackgroundColorBrush);
            _elementTheme = AppShell.GetForCurrentView().AppTheme;
            RaisePropertyChanged(()=>ElementTheme);
        }
    }
}
