﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MvvmCross.Platform;
using OmegaGo.UI.Infrastructure.Bootstrap;
using OmegaGo.UI.WindowsUniversal.Infrastructure;
using Windows.UI.ViewManagement;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.WindowsUniversal.Helpers.Device;
using OmegaGo.UI.Controls.Styles;
#if WITHOUT_FUEGO
#else
using OmegaGo.UI.WindowsUniversal.Fuego;
#endif
using OmegaGo.UI.WindowsUniversal.Services.Settings;
using OmegaGo.UI.WindowsUniversal.Services.Uncategorized;
using Windows.UI.Xaml.Controls.Primitives;
using MvvmCross.Core.ViewModels;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Services.Files;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
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
            OmegaGo.Core.AI.AISystems.RegisterFuegoBuilder(new FuegoBuilder(), Windows.System.MemoryManager.AppMemoryUsageLimit);
#endif

            InitLanguage();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            OptimizeDisplay();
            if (AppShell.GetForCurrentView() == null)
            {
                await InitAsync(e.SplashScreen);
            }
        }

        private async Task InitAsync(SplashScreen splash)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            var appShell = AppShell.GetForCurrentView();

            if (appShell == null)
            {
                //create app shell to hold app content
                appShell = AppShell.CreateForWindow(Window.Current);

                //create extended splash screen
                ExtendedSplashScreen extendedSplash = new ExtendedSplashScreen(splash, false);
                //temporarily place splash into the root frame
                appShell.UnderlyingFrame.Content = extendedSplash;

                SetupWindowServices(Window.Current);
                await InitializeMvvmCrossAsync();
                //setup the title bar
                await SetupTitleBarAsync();
                InitializeStyle();
            }
            CoreApplication.EnablePrelaunch(true);
            // Ensure the current window is active
            Window.Current.Activate();
        }

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            OptimizeDisplay();
            if (AppShell.GetForCurrentView() == null)
            {
                await InitAsync(args.SplashScreen);
            }
            var file = args.Files.OfType<StorageFile>().FirstOrDefault();
            if (file != null)
            {
                var basicProperties = await file.GetBasicPropertiesAsync();
                var contents = await FileIO.ReadTextAsync(file);
                var fileInfo = new FileContentInfo(file.Name, basicProperties.Size, basicProperties.DateModified, contents);
                Mvx.RegisterSingleton(new LibraryViewModel.NavigationModel() { SgfFileInfo = fileInfo });
                AppShell.GetForCurrentView().TabManager.ProcessViewModelRequest(
                    new MvxViewModelRequest(typeof(LibraryViewModel), new MvxBundle(), new MvxBundle(),
                        MvxRequestedBy.UserAction), TabNavigationType.NewForegroundTab);
            }
        }

        /// <summary>
        /// Sets up services for the whole window
        /// </summary>
        /// <param name="window">App window</param>
        private void SetupWindowServices(Window window)
        {
            FullScreenModeManager.RegisterForWindow(window);
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
        private async Task SetupTitleBarAsync()
        {
            //setup the custom title bar in app shell
            AppShell.GetForCurrentView().SetupCustomTitleBar();

            await SetupStatusBarAsync();
        }

        /// <summary>
        /// Sets up the status bar design
        /// </summary>
        private async Task SetupStatusBarAsync()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = (Color)App.Current.Resources["GameColor"];
                statusBar.ForegroundColor = Colors.Black;
                await statusBar.HideAsync();
            }
        }

        private async Task InitializeMvvmCrossAsync()
        {
            var shell = AppShell.GetForCurrentView();
            if (shell == null) throw new NullReferenceException("Shell is not initialized");
            var setup = new Setup(shell);
            setup.Initialize();

            //hide splash screen
            shell.UnderlyingFrame.Content = null;
            var start = Mvx.Resolve<IAsyncAppStart>();
            await start.StartAsync();
            OnlineStartup.Startup();
        }

        private void InitializeStyle()
        {
            IGameSettings settingsService = Mvx.Resolve<IGameSettings>();

            ControlStyle controlStyle = settingsService.Display.ControlStyle;
            switch (controlStyle)
            {
                case ControlStyle.Wood:
                    Application.Current.Resources.Add(typeof(Button), Application.Current.Resources["woodButtonStyle"]);
                    Application.Current.Resources.Add(typeof(ToggleButton), Application.Current.Resources["woodToggleButtonStyle"]);    // Not currently used

                    Application.Current.Resources["DefaultButtonStyle"] = Application.Current.Resources["woodButtonStyle"];
                    break;
                case ControlStyle.Lite:
                    Application.Current.Resources.Add(typeof(Button), Application.Current.Resources["liteButtonStyle"]);
                    Application.Current.Resources.Add(typeof(ToggleButton), Application.Current.Resources["liteToggleButtonStyle"]);

                    Application.Current.Resources["DefaultButtonStyle"] = Application.Current.Resources["liteButtonStyle"];
                    break;
                case ControlStyle.OperatingSystem:
                    Application.Current.Resources.Add(typeof(Button), Application.Current.Resources["OperatingSystemButtonStyle"]);

                    Application.Current.Resources["DefaultButtonStyle"] = Application.Current.Resources["OperatingSystemButtonStyle"];
                    break;
            }
        }

        private void OptimizeDisplay()
        {
            if (DeviceFamilyHelper.DeviceFamily == DeviceFamily.Xbox)
            {
                //this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
                XboxDisplayOptimization();
            }
        }

        private void XboxDisplayOptimization()
        {
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
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
