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

        public Color BackgroundColor
        {
            get { return AppShell.GetForCurrentView().BackgroundColor; }
        }

        public Brush BackgroundColorBrush => _backgroundColorBrush ??
                                             (_backgroundColorBrush = new SolidColorBrush(BackgroundColor));

        public ElementTheme ElementTheme => AppShell.GetForCurrentView().AppTheme;

        /// <summary>
        /// Updates visual settings bindings
        /// </summary>
        public void Refresh()
        {
            RaisePropertyChanged(() => BackgroundColor);

            _backgroundColorBrush = null;
            RaisePropertyChanged(()=>BackgroundColorBrush);  
            
            RaisePropertyChanged(()=>ElementTheme);
        }
    }
}
