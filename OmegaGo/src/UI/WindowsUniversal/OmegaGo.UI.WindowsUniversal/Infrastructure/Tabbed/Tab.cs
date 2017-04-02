using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
    public class Tab : INotifyPropertyChanged, ITabInfo
    {
        private string _title;

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
        /// Gets the tab's title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the tab's icon
        /// </summary>
        public Uri IconUri => (Frame.Content as ViewBase)?.WindowTitleIconUri;

        /// <summary>
        /// Gets the currently displayed view model
        /// </summary>
        public ViewModelBase CurrentViewModel => (Frame.Content as ViewBase)?.ViewModel as ViewModelBase;

        /// <summary>
        /// Gets or sets a helper tag object
        /// </summary>
        public object Tag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
