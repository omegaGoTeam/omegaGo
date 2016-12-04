
using OmegaGo.UI.ViewModels;
using System;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class StatisticsView : TransparencyViewBase
    {
        public StatisticsViewModel VM => (StatisticsViewModel)this.ViewModel;

        public StatisticsView()
        {
            this.InitializeComponent();
        }

        public override string WindowTitle => Localizer.Statistics;

        public override Uri WindowTitleIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Statistics.png");
    }
}
