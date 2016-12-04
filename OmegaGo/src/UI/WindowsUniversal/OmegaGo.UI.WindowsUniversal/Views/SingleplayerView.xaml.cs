
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class SingleplayerView : TransparencyViewBase
    {
        public SingleplayerViewModel VM => (SingleplayerViewModel)this.ViewModel;

        public SingleplayerView()
        {
            this.InitializeComponent();
        }
    }
}
