using OmegaGo.Core;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using Windows.Foundation;

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
