using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.Core.Annotations;
using OmegaGo.UI.Game.Styles;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Timer;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Cheats;
using OmegaGo.UI.WindowsUniversal.Views;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure
{
    //TODO Martin : Reimplement bubble notifications using a custom control, custom view model and custom service, this version mixes it all in app shell
    /// <summary>
    /// The Shell around the whole app
    /// </summary>
    public sealed partial class AppShell : Page, INotifyPropertyChanged
    {
        /// <summary>
        /// Contains the app shells for opened windows
        /// </summary>
        private static readonly Dictionary<Window, AppShell> AppShells = new Dictionary<Window, AppShell>();

        private IGameSettings _settings;

        private AppShell(Window window)
        {
            if (window.Content != null) throw new ArgumentException("App shell can be registered only for Window with empty content", nameof(window));
            this.InitializeComponent();
            window.Content = this;
            AppShells.Add(window, this);

            InitNavigation();

            //debug-only cheats
            InitCheats();
            InitNotifications();
        }

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Gets or sets text in the title bar of the window
        /// </summary>
        public string WindowTitle
        {
            get { return PageTitle.Text; }
            set { PageTitle.Text = value; }
        }

        /// <summary>
        /// Gets or sets icon next to the title bar of the window
        /// </summary>
        public Uri WindowTitleIconUri
        {
            get
            {
                return PageIcon.UriSource;
            }
            set
            {
                if (PageIcon.UriSource != value)
                {
                    PageIcon.UriSource = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the back button in the title bar
        /// </summary>
        public Visibility TitleBarBackButtonVisibility
        {
            get { return BackButton.Visibility; }
            set { BackButton.Visibility = value; }
        }

        /// <summary>
        /// Bubble notifications displayed in in the shell
        /// </summary>
        public ObservableCollection<BubbleNotification> BubbleNotifications { get; } = new ObservableCollection<BubbleNotification>();

        /// <summary>
        /// URL of the background image
        /// </summary>
        public string BackgroundImageUrl
        {
            get
            {
                _settings = _settings ?? Mvx.Resolve<IGameSettings>();
                switch (_settings.Display.BackgroundImage)
                {
                    case BackgroundImage.Go:
                        return "/Assets/MainMenu/backgroundimage.jpg";
                    case BackgroundImage.Forest:
                        return "/Assets/MainMenu/bambooForest.jpg";
                    case BackgroundImage.Shrine:
                        return "/Assets/MainMenu/shintoShrine.jpg";
                    case BackgroundImage.Temple:
                        return "/Assets/MainMenu/ginkakuJi.jpg";
                    case BackgroundImage.None:
                    default:
                        return "/Assets/MainMenu/pixel.png";
                }
            }
        }

        /// <summary>
        /// Background opacity
        /// </summary>
        public float BackgroundOpacity
        {
            get
            {
                _settings = _settings ?? Mvx.Resolve<IGameSettings>();
                switch (_settings.Display.BackgroundImage)
                {
                    case BackgroundImage.Go:
                    case BackgroundImage.None:
                        return 1;
                    case BackgroundImage.Shrine:
                    case BackgroundImage.Temple:
                    case BackgroundImage.Forest:
                        return 0.5f;
                    default:
                        return 1;
                }
            }
        }

        /// <summary>
        /// Background color
        /// </summary>
        public Windows.UI.Xaml.Media.Brush BackgroundColor
        {
            get
            {
                Color color;
                _settings = _settings ?? Mvx.Resolve<IGameSettings>();
                switch (_settings.Display.BackgroundColor)
                {
                    case Game.Styles.BackgroundColor.Basic:
                        color = Color.FromArgb(170, 253, 210, 112);
                        break;
                    case Game.Styles.BackgroundColor.Green:
                        color = Color.FromArgb(220, 164, 242, 167);
                        break;
                    case Game.Styles.BackgroundColor.None:
                    default:
                        color = Colors.Transparent;
                        break;
                }
                return new Windows.UI.Xaml.Media.SolidColorBrush(color);
            }
        }

        /// <summary>
        /// Custom app title bar
        /// </summary>
        public FrameworkElement AppTitleBar => TitleBar;

        /// <summary>
        /// Main frame that hosts app views
        /// </summary>
        public Frame AppFrame => MainFrame;

        /// <summary>
        /// Creates and registers a App Shell for a given Window
        /// </summary>
        /// <param name="window">Window</param>
        /// <returns>App shell</returns>
        public static AppShell CreateForWindow(Window window) => new AppShell(window);

        /// <summary>
        /// Returns the App Shell asociated with the current dispatcher
        /// </summary>
        /// <returns></returns>
        public static AppShell GetForCurrentView()
        {
            AppShell shell = null;
            if (AppShells.TryGetValue(Window.Current, out shell))
            {
                return shell;
            }
            return null;
        }

        /// <summary>
        /// Refreshes the shell visual settings
        /// </summary>
        public void RefreshVisualSettings()
        {
            OnPropertyChanged(nameof(BackgroundOpacity));
            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(BackgroundImageUrl));
        }

        /// <summary>
        /// Sets up the custom title bar
        /// </summary>
        public void SetupCustomTitleBar()
        {
            CoreApplicationViewTitleBar coreTitleBarAppView = CoreApplication.GetCurrentView().TitleBar;
            UpdateTitleBarVisibility();

            coreTitleBarAppView.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            coreTitleBarAppView.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBarAppView.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(DraggableTitleBarArea);
            Window.Current.Activated += WindowTitleBarActivationHandler;
        }

        /// <summary>
        /// Initiates back navigation
        /// </summary>
        /// <returns>Was back navigation handled?</returns>
        public bool GoBack()
        {
            if (AppFrame.CanGoBack)
            {
                var view = AppFrame.Content as ViewBase;
                var vm = view?.ViewModel as ViewModelBase;
                vm?.GoBackCommand.Execute(null);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Add this to a server when SFX is merged in.
        /// </summary>
        /// <param name="notification">The notification.</param>
        public void TriggerBubbleNotification(BubbleNotification notification)
        {
            BubbleNotifications.Add(notification);
            notification.FirstAppeared = DateTime.Now;
        }


        private DispatcherTimer notificationTimer;
        private void InitNotifications()
        {
            notificationTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            notificationTimer.Tick += (sender, e) => ExpireNotifications();
            notificationTimer.Start();
        }

        private void ExpireNotifications()
        {
            for (int ni = BubbleNotifications.Count -1;ni>=0;ni--)
            {
                var notification = BubbleNotifications[ni];
                if (notification.FirstAppeared.AddSeconds(4) < DateTime.Now)
                {
                    BubbleNotifications.RemoveAt(ni);
                }
            }
        }

        /// <summary>
        /// Synchronizes the title bar with the currently displayed view
        /// </summary>
        private void AppFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var view = AppFrame.Content as ViewBase;

            //update title bar back button visibility
            TitleBarBackButtonVisibility = AppFrame.CanGoBack ?
                Visibility.Visible :
                Visibility.Collapsed;

            if (view != null)
            {
                WindowTitleIconUri = view.WindowTitleIconUri;
                var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
                var title = view.WindowTitle;
                WindowTitle = title;
                appView.Title = title;
            }
        }

        /// <summary>
        /// Initializes navigation features
        /// </summary>
        private void InitNavigation()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            Window.Current.CoreWindow.KeyUp += EscapingHandling;
            AppFrame.Navigated += AppFrame_Navigated;
        }


        /// <summary>
        /// Handles the system back navigation
        /// </summary>
        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (GoBack())
            {
                //prevent navigation from the app
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the Esc key for back navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void EscapingHandling(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Escape)
            {
                var view = AppFrame.Content as MainMenuView;
                if (!AppFrame.CanGoBack && view != null)
                {
                    var localizer = (Localizer)Mvx.Resolve<ILocalizationService>();
                    var dialogService = Mvx.Resolve<IDialogService>();
                    if (await dialogService.ShowConfirmationDialogAsync(
                        localizer.QuitText, localizer.QuitCaption, localizer.QuitConfirm, localizer.QuitCancel))
                    {
                        Application.Current.Exit();
                    }
                }
                else if (!args.Handled)
                {
                    args.Handled = true;
                    //handle back navigation as usual
                    GoBack();
                }
            }
        }

        /// <summary>
        /// Changes the opacity of the title bar to respond to deactivation of the window
        /// </summary>
        private void WindowTitleBarActivationHandler(object sender, WindowActivatedEventArgs e)
        {
            var activated = e.WindowActivationState != CoreWindowActivationState.Deactivated;
            AppTitleBar.Opacity = activated ? 1 : 0.5;
        }

        /// <summary>
        /// Updates the title bar layout
        /// </summary>
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Height = sender.Height;
        }

        /// <summary>
        /// Updates the visibility of title bar when change occurs
        /// </summary>
        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarVisibility();
        }

        /// <summary>
        /// Updates the title bar visibility according to the current view
        /// </summary>
        private void UpdateTitleBarVisibility()
        {
            AppTitleBar.Visibility = CoreApplication.GetCurrentView().TitleBar.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the click on the title bar back button
        /// Causes back navigation request
        /// </summary>
        private void TitleBackButton_Click(object sender, RoutedEventArgs e)
        {
            var view = AppFrame.Content as ViewBase;
            var vm = view?.ViewModel as ViewModelBase;
            vm?.GoBackCommand.Execute();
        }

        /// <summary>
        /// Closes a bubble notification
        /// </summary>
        private void CloseNotification_Click(object sender, RoutedEventArgs e)
        {
            BubbleNotifications.Remove(
                ((BubbleNotification)((Button)sender).Tag)
                );
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        private void InitCheats()
        {
            Cheats.Initialize();
        }
    }
}
