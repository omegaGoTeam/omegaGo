
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Infrastructure;
using System;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class SettingsView : TransparencyViewBase
    {
        public SettingsViewModel VM => (SettingsViewModel)this.ViewModel;

        public ObservableCollection<string> AIPrograms { get; set; } = new ObservableCollection<string>();

        public SettingsView()
        {
            this.InitializeComponent();
            foreach (var program in OmegaGo.Core.AI.AISystems.AiPrograms)
            {
                AIPrograms.Add(program.Name);
            }
        }

        public override string WindowTitle => Localizer.Settings;

        public override Uri WindowTitleIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Settings.png");

        private void GoBack_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void Fullscreen_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FullscreenModeManager.Toggle();
        }

        private void SettingsView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.FullscreenMode.IsChecked = ApplicationView.GetForCurrentView().IsFullScreenMode;
        }

        private void FullscreenMode_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FullscreenModeManager.Toggle();
        }
    }
}
