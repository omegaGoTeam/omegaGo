using OmegaGo.UI.ViewModels;
using System;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class LibraryView : TransparencyViewBase
    {
        public LibraryViewModel VM => (LibraryViewModel)this.ViewModel;

        public LibraryView()
        {
            this.InitializeComponent();
        }

        public override string TabTitle => Localizer.GameLibrary;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Library.png");
        
    }
}
