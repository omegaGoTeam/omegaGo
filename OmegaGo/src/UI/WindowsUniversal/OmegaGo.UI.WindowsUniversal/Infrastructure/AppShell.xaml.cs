using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Services.Store.Engagement;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.Core.Annotations;
using OmegaGo.UI.Controls.Themes;
using OmegaGo.UI.Game.Styles;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Feedback;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Timer;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Extensions.Colors;
using OmegaGo.UI.WindowsUniversal.Helpers.Device;
using OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed;
using OmegaGo.UI.WindowsUniversal.Services.Cheats;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using OmegaGo.UI.WindowsUniversal.Views;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure
{
    //TODO Martin : Reimplement bubble notifications using a custom control, custom view model and custom service, this version mixes it all in app shell
    /// <summary>
    /// The Shell around the whole app
    /// </summary>
    public sealed partial class AppShell : Page, INotifyPropertyChanged
    {
        private const double MinimumTouchAreaSize = 44;

        /// <summary>
        /// Contains the app shells for opened windows
        /// </summary>
        private static readonly Dictionary<Window, AppShell> AppShells = new Dictionary<Window, AppShell>();

        private DispatcherTimer _notificationTimer;

        private IGameSettings _settings;
        private IFeedbackService _feedback;

        private Localizer _localizer;

        private AppShell(Window window)
        {
            if (window.Content != null) throw new ArgumentException("App shell can be registered only for Window with empty content", nameof(window));

            this.InitializeComponent();

            window.Content = this;

            TabManager = new TabManager(this);

            DataContext = this;

            AppShells.Add(window, this);

            InitNavigation();

            //debug-only cheats
            InitCheats();
            InitNotifications();

            InitFeedback();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Manager of tabs
        /// </summary>
        public TabManager TabManager { get; }

        /// <summary>
        /// Localizer for the app shell
        /// </summary>
        public Localizer Localizer => _localizer ?? (_localizer = new Localizer());

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
                return _settings.Display.BackgroundColorOpacity;
            }
        }

        public ElementTheme AppTheme
        {
            get
            {
                _settings = _settings ?? Mvx.Resolve<IGameSettings>();
                return _settings.Display.AppTheme == Controls.Themes.AppTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
            }
        }

        /// <summary>
        /// Background color
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                _settings = _settings ?? Mvx.Resolve<IGameSettings>();
                return _settings.Display.BackgroundColor.ToWindowsColor();
            }
        }

        /// <summary>
        /// Custom app title bar
        /// </summary>
        public FrameworkElement AppTitleBar => TitleBar;


        /// <summary>
        /// Main frame that hosts app views
        /// </summary>
        public Frame UnderlyingFrame => MainFrame;

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
            OnPropertyChanged(nameof(AppTheme));
            UpdateTitleBarVisualSettings();
        }

        /// <summary>
        /// Sets up the custom title bar
        /// </summary>
        public void SetupCustomTitleBar()
        {
            UpdateTitleBarVisualSettings();
            CoreApplicationViewTitleBar coreTitleBarAppView = CoreApplication.GetCurrentView().TitleBar;
            UpdateTitleBarVisibility();

            coreTitleBarAppView.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            coreTitleBarAppView.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBarAppView.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(DraggableTitleBarArea);
            Window.Current.Activated += WindowTitleBarActivationHandler;
            Window.Current.SizeChanged += Window_SizeChanged;
            UpdateTitleBarLayout();
        }

        /// <summary>
        /// Initiates back navigation
        /// </summary>
        /// <returns>Was back navigation handled?</returns>
        public bool GoBack()
        {
            if (UnderlyingFrame.CanGoBack)
            {
                var view = UnderlyingFrame.Content as ViewBase;
                var vm = view?.ViewModel as ViewModelBase;
                vm?.GoBackCommand.Execute(null);
                return true;
            }
            return false;
        }

        //TODO Martin: Move to a separate control along with the UI
        /// <summary>
        /// Add this to a server when SFX is merged in.
        /// </summary>
        /// <param name="notification">The notification.</param>
        public void TriggerBubbleNotification(BubbleNotification notification)
        {
            BubbleNotifications.Add(notification);
            notification.FirstAppeared = DateTime.Now;
        }


        /// <summary>
        /// Updates title bar's visual setttings to match the requested element theme
        /// </summary>
        private void UpdateTitleBarVisualSettings()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = (Color)App.Current.Resources["GameColor"];
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = AppTheme == ElementTheme.Light ? Colors.Black : Colors.White;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverBackgroundColor = Colors.DodgerBlue;
            titleBar.ButtonPressedBackgroundColor = Colors.LightBlue;
            titleBar.ButtonHoverForegroundColor = Colors.White; ;
            titleBar.ButtonPressedForegroundColor = Colors.Black; ;
            titleBar.ButtonInactiveForegroundColor = Colors.DimGray;
            titleBar.ForegroundColor = AppTheme == ElementTheme.Light ? Colors.Black : Colors.White;
            titleBar.InactiveForegroundColor = Colors.DimGray;
            titleBar.InactiveBackgroundColor = (Color)App.Current.Resources["GameColor"];
        }

        /// <summary>
        /// Initializes bubble notifications
        /// </summary>
        private void InitNotifications()
        {
            _notificationTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _notificationTimer.Tick += (sender, e) => ExpireNotifications();
            _notificationTimer.Start();
        }

        private void InitFeedback()
        {
            if (StoreServicesFeedbackLauncher.IsSupported()) FeedbackButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Expires the notifications periodically
        /// </summary>
        private void ExpireNotifications()
        {
            for (int ni = BubbleNotifications.Count - 1; ni >= 0; ni--)
            {
                var notification = BubbleNotifications[ni];
                if (notification.FirstAppeared.AddSeconds(4) < DateTime.Now)
                {
                    BubbleNotifications.RemoveAt(ni);
                }
            }
        }

        /// <summary>
        /// Initializes navigation features
        /// </summary>
        private void InitNavigation()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            Window.Current.CoreWindow.KeyUp += EscapingHandling;
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
        private void EscapingHandling(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Escape)
            {
                //let the tab manager handle global back navigation
                TabManager.HandleGlobalBackNavigation();
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
            RightTitleBarMask.Width = sender.SystemOverlayRightInset;
            UpdateTitleBarLayout();
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
            var visible = CoreApplication.GetCurrentView().TitleBar.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            FeedbackButton.Visibility = visible;
            RightTitleBarMask.Visibility = visible;
        }

        /// <summary>
        /// Closes a bubble notification
        /// </summary>
        private void CloseNotification_Click(object sender, RoutedEventArgs e)
        {
            BubbleNotifications.Remove((BubbleNotification)((Button)sender).Tag);
        }


        /// <summary>
        /// Handles feedback button
        /// </summary>
        private async void FeedbackButton_OnClick(object sender, RoutedEventArgs e)
        {
            _feedback = _feedback ?? Mvx.Resolve<IFeedbackService>();
            await _feedback.LaunchAsync();
        }

        /// <summary>
        /// Opens a new main menu tab
        /// </summary>
        private void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            TabManager.ProcessViewModelRequest(
                new MvxViewModelRequest(
                    typeof(MainMenuViewModel), new MvxBundle(), new MvxBundle(), MvxRequestedBy.UserAction),
                TabNavigationType.NewForegroundTab);
        }

        /// <summary>
        /// Creates shared resources for Win2D rendering
        /// </summary>
        private void PersistentHolderCanvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(RenderService.CreateResourcesAsync(sender).AsAsyncAction());
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
        
        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            UpdateTitleBarLayout();
        }

        private void UpdateTitleBarLayout()
        {
            if (DeviceFamilyHelper.DeviceFamily == DeviceFamily.Desktop)
            {
                var minRightContentWidth = RightTitleBarMask.Width + FeedbackButton.ActualWidth +
                                           MinimumTouchAreaSize; //leeway for dragging
                TabListContainer.MaxWidth = Window.Current.Bounds.Width - minRightContentWidth;
            }
            else
            {
                TabListContainer.MaxWidth = Window.Current.Bounds.Width;
            }
        }
    }
}
