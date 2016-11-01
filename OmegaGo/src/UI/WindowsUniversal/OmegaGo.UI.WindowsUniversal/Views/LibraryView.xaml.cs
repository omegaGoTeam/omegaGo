using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class LibraryView : ViewBase
    {
        public LibraryViewModel VM => (LibraryViewModel)this.ViewModel;

        public LibraryView()
        {
            this.InitializeComponent();
        }
    }
}
