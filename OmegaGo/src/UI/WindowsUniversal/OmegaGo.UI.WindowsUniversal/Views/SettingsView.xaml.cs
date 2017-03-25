
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Infrastructure;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml.Input;
using OmegaGo.Core.Annotations;
using OmegaGo.UI.Game.Styles;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.WindowsUniversal.Extensions.Colors;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class SettingsView : TransparencyViewBase, INotifyPropertyChanged
    {
        public SettingsView()
        {
            this.InitializeComponent();
        }

        public SettingsViewModel VM => (SettingsViewModel)this.ViewModel;

        public override string WindowTitle => Localizer.Settings;

        public override Uri WindowTitleIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Settings.png");

        /// <summary>
        /// Gets and sets the full screen mode
        /// </summary>
        public bool IsFullScreen
        {
            get
            {
                return FullScreenModeManager.IsFullScreen;
            }
            set
            {
                if (value != FullScreenModeManager.IsFullScreen)
                {
                    FullScreenModeManager.SetFullScreenMode(value);
                }
            }
        }
        
        public Color SelectedBackgroundColor
        {
            get { return VM.SelectedBackgroundColor.ToWindowsColor(); }
            set
            {
                VM.SelectedBackgroundColor = value.ToBackgroundColor();                
            }
        }

        private void SetDefaultBackgroundColor(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SelectedBackgroundColor = BackgroundColor.Default.ToWindowsColor();
            OnPropertyChanged(nameof(SelectedBackgroundColor));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
