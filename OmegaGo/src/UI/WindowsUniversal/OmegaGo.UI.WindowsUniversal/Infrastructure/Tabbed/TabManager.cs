using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Platform;
using OmegaGo.Core.Annotations;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.UserControls.Navigation;
using OmegaGo.UI.WindowsUniversal.Views;

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
        /// Indicates that the active tab has been changed
        /// </summary>
        public event EventHandler<Tab> ActiveTabChanged;

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
            set
            {
                if (_activeTab != value)
                {
                    if (_activeTab != null)
                    {
                        (_activeTab.Frame.Content as ViewBase)?.TabDeactivated();
                    }
                    SetProperty(ref _activeTab, value);
                    ActiveTabChanged?.Invoke(this, value);
                    UpdateWindowTitle();
                    if (value != null)
                    {
                        (value.Frame.Content as ViewBase)?.TabActivated();
                    }
                }
            }
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
                ActiveTab = targetTab;
            }
            return targetTab;
        }

        /// <summary>
        /// Invokes active tab changed event
        /// </summary>
        /// <param name="tab">New active tab</param>
        protected virtual void OnActiveTabChanged(Tab tab)
        {
            ActiveTabChanged?.Invoke(this, tab);
        }

        /// <summary>
        /// Creates and adds a new empty tab
        /// </summary>
        /// <returns>Created tab</returns>
        private Tab CreateEmptyTab()
        {
            Frame frame = new Frame();
            frame.NavigationFailed += OnTabNavigationFailed;
            Tab tab = new Tab(frame);
            tab.PropertyChanged += Tab_PropertyChanged;
            Tabs.Add(tab);
            return tab;
        }

        private void Tab_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateWindowTitle();
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

        /// <summary>
        /// Updates the Window title to match the current tab
        /// </summary>
        private void UpdateWindowTitle()
        {
            var viewTitle = ActiveTab?.Title;
            ApplicationView.GetForCurrentView().Title = viewTitle ?? "";
        }

        /// <summary>
        /// Handles back navigation
        /// </summary>
        /// <param name="viewModelToNavigateBack">View model that wants to navigate back</param>
        /// <returns>Was back navigation successful?</returns>
        public bool HandleBackNavigation(IMvxViewModel viewModelToNavigateBack)
        {
            var tab = Tabs.FirstOrDefault(t => t.CurrentViewModel == viewModelToNavigateBack);
            if (tab != null && tab.Frame.CanGoBack)
            {
                tab.Frame.GoBack();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Activates a given tab
        /// </summary>
        /// <param name="tab">Tab to activate</param>
        /// <returns>Was activation successful?</returns>
        public bool SwitchToTab(ITabInfo tab)
        {
            if (Tabs.Contains(tab))
            {
                ActiveTab = (Tab)tab;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Closes a given tab
        /// </summary>
        /// <param name="tab">Tab to be closed</param>
        /// <returns>Was closing successful?</returns>
        public bool CloseTab(ITabInfo tab)
        {
            if (!Tabs.Contains(tab)) return false;

            var closedTab = (Tab)tab;

            //the closed tab is the only tab opened
            if (Tabs.Count == 1)
            {
                if (closedTab.CurrentViewModel.GetType() == typeof(MainMenuViewModel))
                {
                    //shut down the app
                    Application.Current.Exit();
                }
                else
                {
                    //create new main menu tab
                    ProcessViewModelRequest(
                        new MvxViewModelRequest(typeof(MainMenuViewModel), new MvxBundle(), new MvxBundle(),
                            MvxRequestedBy.Unknown),
                        TabNavigationType.NewForegroundTab
                    );
                }
            }
            else
            {
                //is the closed tab active?
                if (closedTab == ActiveTab)
                {
                    //activate a different tab
                    var closedTabIndex = Tabs.IndexOf(closedTab);
                    if ((Tabs.Count - 1) > closedTabIndex)
                    {
                        //activate next tab
                        ActiveTab = Tabs[closedTabIndex + 1];
                    }
                    else
                    {
                        ActiveTab = Tabs[closedTabIndex - 1];
                    }
                }
            }
            Tabs.Remove(closedTab);
            //inform the view that its tab has been closed
            ( closedTab.Frame.Content as ViewBase )?.TabClosed();
            return true;
        }
    }
}
