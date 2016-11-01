using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class MainMenuView : ViewBase
    {
        public MainMenuViewModel VM => (MainMenuViewModel)this.ViewModel;

        public MainMenuView()
        {
            this.InitializeComponent();
        }
    }
}
