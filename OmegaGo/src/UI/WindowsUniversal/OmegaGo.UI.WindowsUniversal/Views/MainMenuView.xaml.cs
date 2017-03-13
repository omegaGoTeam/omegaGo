using System;
using System.Diagnostics;
using System.Linq;
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
        public MainMenuViewModel VM => (MainMenuViewModel)this.ViewModel;

        public MainMenuView()
        {
            this.InitializeComponent();
            Loaded += MainMenuView_Loaded;
        }

        private void MainMenuView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceFamilyHelper.DeviceFamily == DeviceFamily.Xbox || DeviceFamilyHelper.DeviceFamily == DeviceFamily.Desktop)
            {
                FocusOptimization();
            }
        }

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
                menuItem.Click += MenuItem_Click;
                LanguagesMenu.Items?.Add(menuItem);
            }
            UpdateLanguageMenuSelection();
        }

        private void MenuItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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


        private void GoFullScreen_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FullscreenModeManager.Toggle();
        }

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
    }
}
