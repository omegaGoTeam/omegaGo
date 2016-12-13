
using MvvmCross.Platform;
using OmegaGo.UI.Services.Tsumego;
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

        private void SolveThisTsumego_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TsumegoProblem problem = TsumegoProblem.SelectedItem as TsumegoProblem;
            if (problem != null)
            {
                VM.MoveToSolveTsumegoProblem(problem);
            }
        }

        private void TransparencyViewBase_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.TsumegoProblem.SelectedIndex = 0;
        }
    }
}
