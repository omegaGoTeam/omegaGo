using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class GameCreationView : TransparencyViewBase
    {
        public GameCreationViewModel VM => (GameCreationViewModel)this.ViewModel;

        public GameCreationView()
        {
            this.InitializeComponent();
        }
    }
}
