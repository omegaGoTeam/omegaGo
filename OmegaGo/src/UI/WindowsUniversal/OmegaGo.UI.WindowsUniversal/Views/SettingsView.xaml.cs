
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Infrastructure;
using System;
using Windows.UI.Xaml.Input;
using OmegaGo.UI.Services.Audio;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class SettingsView : TransparencyViewBase
    {
        public SettingsView()
        {
            this.InitializeComponent();
        }

        public SettingsViewModel VM => (SettingsViewModel)this.ViewModel;
        
        public override string WindowTitle => Localizer.Settings;

        public override Uri WindowTitleIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Settings.png");

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

        private async void SfxVolumeChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            await Sounds.VolumeTestSound.PlayAsync();
        }

        private void AppShellComboBox_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            AppShell.GetForCurrentView().RefreshVisualSettings();
        }
    }
}
