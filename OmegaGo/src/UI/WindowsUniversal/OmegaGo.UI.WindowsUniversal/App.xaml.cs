using System;
using System.Collections.Generic;
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
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Init( e );
        }

        private async void Init( LaunchActivatedEventArgs e )
        {
#if DEBUG
            if ( System.Diagnostics.Debugger.IsAttached )
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            _rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if ( _rootFrame == null )
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                _rootFrame = new Frame();
                // Set the default language
                _rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[ 0 ];

                _rootFrame.NavigationFailed += OnNavigationFailed;

                //  Display an extended splash screen if app was not previously running.
                if ( e.PreviousExecutionState != ApplicationExecutionState.Running )
                {

                }
            }

            if ( e.PrelaunchActivated == false )
            {
                if ( _rootFrame.Content == null )
                {                                        
                    ExtendedSplashScreen extendedSplash = new ExtendedSplashScreen( e.SplashScreen, false );
                    _rootFrame.Content = extendedSplash;
                    Window.Current.Content = _rootFrame;
                }
                // Ensure the current window is active
                SetupTitleBar();               
                Window.Current.Activate();
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
                Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
                await InitializeMvvmCrossAsync();
            }
        }

        /// <summary>
        /// Sets up the application title bar design
        /// </summary>
        private void SetupTitleBar()
        {
            //CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            //coreTitleBar.ExtendViewIntoTitleBar = true;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = ( Color )App.Current.Resources[ "GameColor" ];
            titleBar.ButtonBackgroundColor = titleBar.BackgroundColor;
            titleBar.ButtonInactiveBackgroundColor = titleBar.BackgroundColor;
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb( 255, 215, 215, 215 );
            titleBar.ButtonPressedBackgroundColor = Color.FromArgb( 255, 180, 180, 180 );
            titleBar.ButtonHoverForegroundColor = Colors.Black;
            titleBar.ButtonPressedForegroundColor = Colors.Black;
            titleBar.ButtonInactiveForegroundColor = Colors.LightGray;
            titleBar.ForegroundColor = Colors.White;
            titleBar.InactiveForegroundColor = Colors.LightGray;
            titleBar.InactiveBackgroundColor = ( Color )App.Current.Resources[ "GameColor" ];
            SetupStatusBar();
        }

        /// <summary>
        /// Sets up the status bar design
        /// </summary>
        private void SetupStatusBar()
        {
            if ( Windows.Foundation.Metadata.ApiInformation.IsTypePresent( "Windows.UI.ViewManagement.StatusBar" ) )
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = ( Color )App.Current.Resources[ "GameColor" ];
                statusBar.ForegroundColor = Colors.White;
            }
        }

        private bool altIsHeld = false;

        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Control)
            {
                // Yes, it should be Alt+Enter, not Ctrl+Enter, but "Alt" is not yet working.
                altIsHeld = false;
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Enter)
            {
                if (altIsHeld)
                {
                    if (ApplicationView.GetForCurrentView().IsFullScreenMode)
                    {
                        ApplicationView.GetForCurrentView().ExitFullScreenMode();
                    }
                    else
                    {
                        ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                    }
                }
            }
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Control)
            {
                altIsHeld = true;
            }
        }

        private async Task InitializeMvvmCrossAsync()
        {
            if ( _rootFrame == null ) throw new NullReferenceException("Root frame is not initialized"); 
            var setup = new Setup( _rootFrame );
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
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
