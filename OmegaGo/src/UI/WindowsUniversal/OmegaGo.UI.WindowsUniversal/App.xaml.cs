using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MvvmCross.Platform;
using OmegaGo.UI.Infrastructure;
using OmegaGo.UI.Infrastructure.Bootstrap;
using OmegaGo.UI.WindowsUniversal.Infrastructure;
using Windows.UI.ViewManagement;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;
#if WITHOUT_FUEGO
#else
using OmegaGo.UI.WindowsUniversal.Fuego;
#endif
using OmegaGo.UI.WindowsUniversal.Services.Settings;

namespace OmegaGo.UI.WindowsUniversal
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private Frame _rootFrame = null;
        
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
#if WITHOUT_FUEGO
#else
            OmegaGo.Core.AI.AISystems.RegisterFuegoBuilder(new FuegoBuilder());
#endif
            InitLanguage();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Init(e);
        }

        private async void Init(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            _rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (_rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                _rootFrame = new Frame();
                // Set the default language
                _rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                _rootFrame.NavigationFailed += OnNavigationFailed;
            }

            if (e.PrelaunchActivated == false)
            {
                if (_rootFrame.Content == null)
                {
                    //create app shell to hold app content
                    var shell = AppShell.CreateForWindow(Window.Current);
                    //create extended splash screen
                    ExtendedSplashScreen extendedSplash = new ExtendedSplashScreen(e.SplashScreen, false);
                    //temporarily place splash into the root frame
                    shell.AppFrame.Content = extendedSplash;
                    //setup the title bar
                    SetupTitleBar();
                }
                // Ensure the current window is active
                Window.Current.Activate();
                SetupWindowServices(Window.Current);
                await InitializeMvvmCrossAsync();
            }
        }

        /// <summary>
        /// Sets up services for the whole window
        /// </summary>
        /// <param name="window">App window</param>
        private void SetupWindowServices(Window window)
        {
            FullscreenModeManager.RegisterForWindow(window);
        }

        /// <summary>
        /// Setup language        
        /// </summary>
        private void InitLanguage()
        {
            var gameSettings = new GameSettings(new SettingsService());
            if (gameSettings.Language != GameLanguages.DefaultLanguage.CultureTag)
            {
                try
                {
                    CultureInfo.CurrentUICulture = new CultureInfo(gameSettings.Language);
                    CultureInfo.CurrentCulture = new CultureInfo(gameSettings.Language);
                }
                catch
                {
                    //ignore, leave default language
                }
            }
        }

        /// <summary>
        /// Sets up the application title bar design
        /// </summary>
        private void SetupTitleBar()
        {
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = (Color)App.Current.Resources["GameColor"];
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.Black;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverBackgroundColor = (Color)App.Current.Resources["TitleBarButtonHoverColor"];
            titleBar.ButtonPressedBackgroundColor = (Color)App.Current.Resources["TitleBarButtonPressedColor"];
            titleBar.ButtonHoverForegroundColor = Colors.Black;
            titleBar.ButtonPressedForegroundColor = Colors.Black;
            titleBar.ButtonInactiveForegroundColor = Colors.DimGray;
            titleBar.ForegroundColor = Colors.Black;
            titleBar.InactiveForegroundColor = Colors.DimGray;
            titleBar.InactiveBackgroundColor = (Color)App.Current.Resources["GameColor"];

            //setup the custom title bar in app shell
            AppShell.GetForCurrentView().SetupCustomTitleBar();

            SetupStatusBar();
        }

        /// <summary>
        /// Sets up the status bar design
        /// </summary>
        private void SetupStatusBar()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = (Color)App.Current.Resources["GameColor"];
                statusBar.ForegroundColor = Colors.Black;
            }
        }

        private async Task InitializeMvvmCrossAsync()
        {
            var shell = AppShell.GetForCurrentView();
            if (shell == null) throw new NullReferenceException("Shell is not initialized");
            var setup = new Setup(shell.AppFrame);
            setup.Initialize();

            var start = Mvx.Resolve<IAsyncAppStart>();
            await start.StartAsync();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            
            deferral.Complete();
        }
    }
}
