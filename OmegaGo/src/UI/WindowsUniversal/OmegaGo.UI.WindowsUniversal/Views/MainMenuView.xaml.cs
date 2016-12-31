using System.Linq;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Infrastructure;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class MainMenuView : TransparencyViewBase
    {
        public MainMenuViewModel VM => (MainMenuViewModel)this.ViewModel;

        public MainMenuView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadLanguageMenu();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UnloadLanguageMenu();
        }

        private void UnloadLanguageMenu()
        {
            VM.PropertyChanged -= VM_PropertyChanged;
        }

        private void LoadLanguageMenu()
        {
            //should never happen
            if (LanguagesMenu.Items == null) return;

            LanguagesMenu.Items.Clear();
            foreach (var language in VM.Languages)
            {
                //add each language as menu item and hook up check event
                ToggleMenuFlyoutItem menuItem = new ToggleMenuFlyoutItem();
                //store the related language as tag
                menuItem.Tag = language;
                menuItem.Text = Localizer.GetString(language.Name);
                if (language == VM.SelectedLanguage)
                {
                    //check
                    menuItem.IsChecked = true;
                }
                menuItem.Tapped += LanguageMenuItemTapped;
                LanguagesMenu.Items?.Add(menuItem);
            }
            VM.PropertyChanged += VM_PropertyChanged;
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VM.SelectedLanguage))
            {
                //select the right menu item
                foreach (var menuItem in LanguagesMenu.Items.OfType<ToggleMenuFlyoutItem>())
                {
                    menuItem.IsChecked = menuItem.Tag == VM.SelectedLanguage;
                }
            }
        }

        private void LanguageMenuItemTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var menuItem = sender as ToggleMenuFlyoutItem;
            if (menuItem != null)
            {
                //update Selected language
                VM.SelectedLanguage = menuItem.Tag as GameLanguage;
            }
        }

        private void ShowHideTips_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.BorderTips.Visibility = this.BorderTips.Visibility == Windows.UI.Xaml.Visibility.Collapsed
                ? Windows.UI.Xaml.Visibility.Visible
                : Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void GoFullScreen_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FullscreenModeManager.Toggle();
        }
    }
}
