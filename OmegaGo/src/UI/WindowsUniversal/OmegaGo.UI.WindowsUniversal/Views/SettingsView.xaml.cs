
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
    }
}
