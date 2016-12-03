using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class LibraryView : TransparencyViewBase
    {
        public LibraryViewModel VM => (LibraryViewModel)this.ViewModel;

        public LibraryView()
        {
            this.InitializeComponent();
        }
    }
}
