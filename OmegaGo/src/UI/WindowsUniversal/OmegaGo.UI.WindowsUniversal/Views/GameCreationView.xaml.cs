using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class GameCreationView : ViewBase
    {
        public GameCreationViewModel VM => (GameCreationViewModel)this.ViewModel;

        public GameCreationView()
        {
            this.InitializeComponent();
        }
    }
}
