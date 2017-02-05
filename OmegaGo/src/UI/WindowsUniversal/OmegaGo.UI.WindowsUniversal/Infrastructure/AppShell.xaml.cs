﻿using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Views;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure
{
    /// <summary>
    /// The Shell around the whole app
    /// </summary>
    public sealed partial class AppShell : Page
    {
        /// <summary>
        /// Contains the app shells for opened windows
        /// </summary>
        private static readonly Dictionary<Window, AppShell> AppShells = new Dictionary<Window, AppShell>();

        private AppShell(Window window)
        {
            if (window.Content != null) throw new ArgumentException("App shell can be registered only for Window with empty content", nameof(window));
            this.InitializeComponent();
            window.Content = this;
            AppShells.Add(window, this);
            AppFrame.Navigated += AppFrame_Navigated;
        }

        /// <summary>
        /// Synchronizes the title bar with the currently displayed view
        /// </summary>
        private void AppFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var view = AppFrame.Content as ViewBase;

            if (AppFrame.CanGoBack)
            {
                TitleBarBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                TitleBarBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }

            if (view != null)
            {
                WindowTitle = view.WindowTitle;
                WindowTitleIconUri = view.WindowTitleIconUri;
            }
        }

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
        /// Custom app title bar
        /// </summary>
        public FrameworkElement AppTitleBar => TitleBar;

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
        public AppViewBackButtonVisibility TitleBarBackButtonVisibility
        {
            get
            {
                return BackButton.Visibility == Visibility.Visible ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            }
            set
            {
                BackButton.Visibility = value == AppViewBackButtonVisibility.Visible
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Main frame that hosts app views
        /// </summary>
        public Frame AppFrame => MainFrame;

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

            InitNavigation();
        }

        /// <summary>
        /// Initiates back navigation
        /// </summary>
        public void GoBack()
        {
            if (AppFrame.CanGoBack)
            {
                var view = AppFrame.Content as ViewBase;
                var vm = view?.ViewModel as ViewModelBase;
                vm?.GoBackCommand.Execute(null);
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
            GoBack();
        }

        /// <summary>
        /// Handles the Esc key for back navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EscapingHandling(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Escape )
            {
                if (!args.Handled)
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
    }
}
