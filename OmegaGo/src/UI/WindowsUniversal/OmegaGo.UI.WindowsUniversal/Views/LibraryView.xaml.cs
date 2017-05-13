using OmegaGo.UI.ViewModels;
using System;
using Windows.UI.Core;
using Microsoft.Toolkit.Uwp.UI.Animations.Behaviors;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class LibraryView : TransparencyViewBase
    {
        private const string BlurBehaviorElementName = "BlurBehavior";

        public LibraryViewModel VM => (LibraryViewModel)this.ViewModel;

        public LibraryView()
        {
            this.InitializeComponent();
            Loaded += LibraryView_Loaded;
            Unloaded += LibraryView_Unloaded;
        }

        private void LibraryView_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= LibraryView_BackRequested;
        }

        private void LibraryView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += LibraryView_BackRequested;
        }

        private void LibraryView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (VM.SelectedLibraryItem != null)
            {
                VM.SelectedLibraryItem = null;
                e.Handled = true;                
            }
        }

        public override string TabTitle => Localizer.GameLibrary;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Library.png");

        public void Blur()
        {
            var blur = FindName(BlurBehaviorElementName) as Blur;
            if (blur != null) blur.Value = 3;
        }

        public void Unblur()
        {
            var blur = FindName(BlurBehaviorElementName) as Blur;
            if (blur != null) blur.Value = 0;
        }
    }
}
