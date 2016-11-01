
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class TutorialView : ViewBase
    {
        public TutorialViewModel VM => (TutorialViewModel)this.ViewModel;

        public TutorialView()
        {
            this.InitializeComponent();
        }
    }
}
