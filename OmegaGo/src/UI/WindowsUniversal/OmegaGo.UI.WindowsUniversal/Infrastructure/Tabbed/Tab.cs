using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using MvvmCross.Core.ViewModels;
using OmegaGo.Core.Annotations;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Views;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed
{
    /// <summary>
    /// Represents a UI tab
    /// </summary>
    public class Tab : MvxNotifyPropertyChanged, ITabInfo
    {
        private ICommand _closeCommand;
        private ICommand _goBackCommand;
        private string _title;
        private Uri _iconUri;

        /// <summary>
        /// Creates a tab with a given frame
        /// </summary>
        /// <param name="frame">Frame to use for navigation</param>
        public Tab(Frame frame)
        {
            Frame = frame;
        }

        /// <summary>
        /// Unique identification of a tab
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Frame used to display Tab's content
        /// </summary>
        public Frame Frame { get; }

        /// <summary>
        /// Gets the currently displayed view model
        /// </summary>
        public ViewModelBase CurrentViewModel => (Frame.Content as ViewBase)?.ViewModel as ViewModelBase;

        /// <summary>
        /// Close tab command
        /// </summary>
        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new MvxCommand(Close));

        /// <summary>
        /// Go back command
        /// </summary>
        public ICommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new MvxCommand(GoBack));

        /// <summary>
        /// Gets the tab's title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        /// <summary>
        /// Gets the tab's icon
        /// </summary>
        public Uri IconUri
        {
            get { return _iconUri; }
            set { SetProperty(ref _iconUri, value); }
        }

        /// <summary>
        /// Gets or sets a helper tag object
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Goes back in the tab
        /// </summary>
        private void GoBack()
        {
            AppShell.GetForCurrentView().TabManager.HandleBackNavigation(CurrentViewModel);
        }

        /// <summary>
        /// Closes this tab using tab manager
        /// </summary>
        private void Close()
        {
            AppShell.GetForCurrentView().TabManager.CloseTab(this);
        }
    }
}
