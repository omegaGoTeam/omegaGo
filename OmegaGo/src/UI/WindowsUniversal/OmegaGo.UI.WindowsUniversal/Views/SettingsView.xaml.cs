
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class SettingsView : ViewBase
    {
        public SettingsViewModel VM => (SettingsViewModel)this.ViewModel;

        public SettingsView()
        {
            this.InitializeComponent();
        }
    }
}
