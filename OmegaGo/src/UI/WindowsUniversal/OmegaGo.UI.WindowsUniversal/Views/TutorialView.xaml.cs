
using Windows.UI.Xaml;
using OmegaGo.UI.ViewModels;
using System;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    // ReSharper disable once UnusedMember.Global
    public sealed partial class TutorialView
    {
        private bool _isInitialized = false;
        public TutorialViewModel VM => (TutorialViewModel)this.ViewModel;

        public TutorialView()
        {
            InitializeComponent();
        }

        public override string TabTitle => this.Localizer.Tutorial;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Tutorial.png");
        
        private void BoardControl_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.VM.TapBoardControl();
        }
    }
}
