using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Helpers.Device;
using OmegaGo.UI.WindowsUniversal.Infrastructure;
using OmegaGo.UI.WindowsUniversal.UserControls.MainMenu;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class MainMenuView : TransparencyViewBase
    {
        private const string GoFullScreenGlyph = "\uE1D9";
        private const string ExitFullScreenGlyph = "\uE1D8";

        public MainMenuViewModel VM => (MainMenuViewModel)this.ViewModel;

        public MainMenuView()
        {
            this.InitializeComponent();
            Loaded += MainMenuView_Loaded;
        }

        /// <summary>
        /// Title of the main menu tab
        /// </summary>
        public override string TabTitle => Localizer.MainMenu;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/MainMenu.png");

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadLanguageMenu();
            ///TODO: Martin - make this simpler by using itemssource and data binding
            if (!VM.ShowTutorialButton)
            {
                MenuItemsControl.Items.RemoveAt(0);
            }

        }
        
        /// <summary>
        /// Ensures focus setting for Xbox and desktop keyboard
        /// </summary>
        private void MainMenuView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceFamilyHelper.DeviceFamily == DeviceFamily.Xbox || DeviceFamilyHelper.DeviceFamily == DeviceFamily.Desktop)
            {
                FocusOptimization();
            }
            FullScreenButtonIcon.Glyph = FullScreenModeManager.IsFullScreen ?
                ExitFullScreenGlyph : GoFullScreenGlyph;
        }

        /// <summary>
        /// Populates the language menu and sets up its events
        /// </summary>
        private void LoadLanguageMenu()
        {
            //should never happen
            if (LanguagesMenu.Items == null) return;

            LanguagesMenu.Items.Clear();
            foreach (var language in VM.Languages)
            {
                //add each language as menu item and hook up check event
                ToggleMenuFlyoutItem menuItem = new ToggleMenuFlyoutItem
                {
                    //store the related language as tag
                    Tag = language,
                    Text = Localizer.GetString(language.Name),
                };
                menuItem.Click += LanguageMenuItem_Click;
                LanguagesMenu.Items?.Add(menuItem);
            }
            UpdateLanguageMenuSelection();
        }

        /// <summary>
        /// Handles language menu selection
        /// </summary>
        private void LanguageMenuItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var menuItem = sender as ToggleMenuFlyoutItem;
            if (menuItem != null)
            {
                //update Selected language
                VM.SelectedLanguage = menuItem.Tag as GameLanguage;
                UpdateLanguageMenuSelection();
            }
        }
        /// <summary>
        /// Updates the checked languages menu item according to the currently selected language
        /// </summary>
        private void UpdateLanguageMenuSelection()
        {
            foreach (var menuItem in LanguagesMenu.Items.OfType<ToggleMenuFlyoutItem>())
            {
                menuItem.IsChecked = menuItem.Tag == VM.SelectedLanguage;
            }
        }
        
        /// <summary>
        /// Sets the full screen mode
        /// </summary>
        private void FullScreenButtonClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var useFullScreen = !FullScreenModeManager.IsFullScreen;
            FullScreenModeManager.SetFullScreenMode( useFullScreen );
            FullScreenButtonIcon.Glyph = useFullScreen ?
                ExitFullScreenGlyph : GoFullScreenGlyph;
        }

        /// <summary>
        /// Optimization of focus for the menu buttons, used for Xbox and desktop keyboard
        /// </summary>
        private async void FocusOptimization()
        {
            await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    if (!VM.ShowTutorialButton)
                    {
                        SinglePlayerButton.Focus(FocusState.Programmatic);
                    }
                    else
                    {
                        TutorialButton.Focus(FocusState.Programmatic);
                    }
                });
        }

        private void EasterEgg_Tapped(object sender, RightTappedRoutedEventArgs e)
        {
            AppShell.GetForCurrentView().ToggleEasterEgg();
        }

        private void BottomCommandBar_OnClosing(object sender, object e)
        {
            SoundsButton.IsCompact = true;
            LanguageMenuButton.IsCompact = true;
        }

        private void BottomCommandBar_OnOpening(object sender, object e)
        {
            SoundsButton.IsCompact = false;
            LanguageMenuButton.IsCompact = false;
        }
    }
}
