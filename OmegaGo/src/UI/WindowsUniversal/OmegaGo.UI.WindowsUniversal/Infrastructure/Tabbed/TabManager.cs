using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Platform;
using OmegaGo.Core.Annotations;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.WindowsUniversal.UserControls.Navigation;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed
{
    /// <summary>
    /// Manages the tabbed UI, acts as a view model for the TabbedUIControl
    /// </summary>
    public class TabManager : MvxNotifyPropertyChanged
    {
        /// <summary>
        /// Tabbed UI control managed by this tab manager
        /// </summary>
        private readonly AppShell _appShell;

        private Tab _activeTab;

        /// <summary>
        /// Creates tab manager for an app shell
        /// </summary>
        /// <param name="appShell">App shell</param>
        public TabManager(AppShell appShell)
        {
            _appShell = appShell;
        }

        /// <summary>
        /// Currently opened tabs
        /// </summary>
        public ObservableCollection<Tab> Tabs { get; } = new ObservableCollection<Tab>();

        /// <summary>
        /// Gets or sets the active tab
        /// </summary>
        public Tab ActiveTab
        {
            get { return _activeTab; }
            set { SetProperty(ref _activeTab, value); }
        }

        /// <summary>
        /// Activates a tab
        /// </summary>
        /// <param name="tab">Tab to activate</param>
        public void ActivateTab(Tab tab)
        {
            ActiveTab = tab;
        }

        /// <summary>
        /// Processes a view model request
        /// </summary>
        /// <param name="request">View model request</param>
        /// <param name="tabNavigationType">Type of tab navigation to perform</param>
        internal ITabInfo ProcessViewModelRequest(MvxViewModelRequest request, TabNavigationType tabNavigationType)
        {
            //process the request
            var requestTranslator = Mvx.Resolve<IMvxViewsContainer>();
            var viewType = requestTranslator.GetViewType(request.ViewModelType);

            var converter = Mvx.Resolve<IMvxNavigationSerializer>();
            var requestText = converter.Serializer.SerializeObject(request);
        
            //prepare tab
            var targetTab = ActiveTab;
            bool activeAndNeedsNew = tabNavigationType == TabNavigationType.ActiveTab && ActiveTab == null;
            if (activeAndNeedsNew || tabNavigationType != TabNavigationType.ActiveTab)
            {
                targetTab = CreateEmptyTab();
            }
            targetTab.Frame.Navigate(viewType, requestText);
            if (tabNavigationType != TabNavigationType.NewBackgroundTab)
            {
                ActivateTab(targetTab);
            }
            return targetTab;
        }

        /// <summary>
        /// Creates and adds a new empty tab
        /// </summary>
        /// <returns>Created tab</returns>
        private Tab CreateEmptyTab()
        {
            Frame frame = new Frame();
            frame.NavigationFailed += OnTabNavigationFailed;
            frame.ContentTransitions = new TransitionCollection();
            frame.Transitions = new TransitionCollection();
            frame.Navigated += OnTabNavigated;
            Tab tab = new Tab(frame);
            Tabs.Add(tab);
            return tab;
        }

        /// <summary>
        /// Invoked when tab navigation is performed
        /// </summary>
        private void OnTabNavigated(object sender, NavigationEventArgs e)
        {

        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnTabNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
