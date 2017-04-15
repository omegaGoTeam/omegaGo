
using System;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Tsumego;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class TsumegoMenuView : TransparencyViewBase
    {
        public TsumegoMenuViewModel VM => (TsumegoMenuViewModel)this.ViewModel;

        public TsumegoMenuView()
        {
            this.InitializeComponent();
        }

        public override string TabTitle => $"{Localizer.TsumegoMenuCaption}";

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Tsumego.png");

        private void GridView_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            VM.MoveToSolveTsumegoProblem((TsumegoProblemInfo) e.ClickedItem);
        }
    }
}
