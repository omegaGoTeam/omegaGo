using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class AboutView : TransparencyViewBase
    {
        public AboutViewModel VM => (AboutViewModel)this.ViewModel;

        public AboutView()
        {
            this.InitializeComponent();
        }
    }
}
