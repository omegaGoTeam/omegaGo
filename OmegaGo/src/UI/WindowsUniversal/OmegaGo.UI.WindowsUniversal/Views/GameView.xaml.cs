using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class GameView : ViewBase
    {
        public GameViewModel VM => (GameViewModel)this.ViewModel;

        public GameView()
        {
            this.InitializeComponent();
        }
    }
}
