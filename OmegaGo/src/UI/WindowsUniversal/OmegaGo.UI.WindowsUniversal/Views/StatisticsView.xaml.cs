
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class StatisticsView : TransparencyViewBase
    {
        public StatisticsViewModel VM => (StatisticsViewModel)this.ViewModel;

        public StatisticsView()
        {
            this.InitializeComponent();
        }
    }
}
