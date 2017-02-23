
using OmegaGo.UI.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using OmegaGo.UI.WindowsUniversal.Services.Audio;

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
