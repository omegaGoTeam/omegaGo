using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.ViewModels
{
    /// <summary>
    /// Base for all ViewModels in the app
    /// </summary>
    public class ViewModelBase : MvxViewModel
    {
        private Localizer _localizer = null;    
        private IMvxCommand _goBackCommand;
        private bool _isWorking = false;

        /// <summary>
        /// Localizer for the ViewModel
        /// </summary>
        public Localizer Localizer => _localizer ?? (_localizer = new Localizer());

        /// <summary>
        /// Provides back navigation
        /// </summary>
        public IMvxCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new MvxCommand(GoBack));

        /// <summary>
        /// Used for loading indication
        /// </summary>
        public bool IsWorking
        {
            get
            {                  
                return _isWorking;
            }
            set
            {
                SetProperty(ref _isWorking, value);
            }
        }

        /// <summary>
        /// Opens a view model in a new tab and switches to this tab
        /// </summary>
        /// <typeparam name="TViewModel">View model type to open</typeparam>
        public ITabInfo OpenInNewActiveTab<TViewModel>()
        {
            var newTab = OpenInNewBackgroundTab<TViewModel>();
            SwitchToTab(newTab);
            return newTab;
        }

        /// <summary>
        /// Opens a view model in a tab, but doesn't switch to it
        /// </summary>
        public ITabInfo OpenInNewBackgroundTab<TViewModel>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Switches to a given tab
        /// </summary>
        /// <param name="tab">Tab to switch to</param>
        /// <returns>Was the switch successful?</returns>
        public bool SwitchToTab(ITabInfo tab)
            => SwitchToTab(tab.Id);

        /// <summary>
        /// Switches to a given tab
        /// </summary>
        /// <param name="tabId">Id of the tab to switch to</param>
        /// <returns>Was the switch successful?</returns>
        public bool SwitchToTab(Guid tabId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes the tab this view model is displayed in
        /// </summary>
        /// <returns></returns>
        public bool CloseTab()
        {
            throw new NotImplementedException();
        }        

        /// <summary>
        /// Override this method to do any "on navigated from" work or to stop the navigation altogether.
        /// </summary>
        public virtual void GoBack()
        {
            Close(this);
        }
    }
}
