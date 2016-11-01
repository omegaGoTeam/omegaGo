
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class MultiplayerDashboard : ViewBase
    {
        public MultiplayerDashboardViewModel VM => (MultiplayerDashboardViewModel)this.ViewModel;

        public MultiplayerDashboard()
        {
            this.InitializeComponent();
        }
    }
}
