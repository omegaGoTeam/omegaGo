using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.UI.Infrastructure.PresentationHints;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.ViewModels
{
    /// <summary>
    /// Base for all ViewModels in the app
    /// </summary>
    public class ViewModelBase : MvxViewModel
    {
        private readonly ITabProvider _tabProvider = Mvx.Resolve<ITabProvider>();

        private Localizer _localizer = null;

        private IMvxCommand _goBackCommand;
        private bool _isWorking = false;

        /// <summary>
        /// Localizer for the ViewModel
        /// </summary>
        public Localizer Localizer => _localizer ?? (_localizer = new Localizer());

        /// <summary>
        /// Called when the view model is appearing (including background)
        /// </summary>
        public virtual void Appearing()
        {
        }

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
        public ITabInfo OpenInNewActiveTab<TViewModel>() where TViewModel : IMvxViewModel
        {
            return _tabProvider.ShowViewModel(
                new MvxViewModelRequest<TViewModel>(new MvxBundle(), new MvxBundle(), MvxRequestedBy.UserAction),
                TabNavigationType.NewForegroundTab);
        }

        /// <summary>
        /// Opens a view model in a tab, but doesn't switch to it
        /// </summary>
        public ITabInfo OpenInNewBackgroundTab<TViewModel>() where TViewModel : IMvxViewModel
        {
            return _tabProvider.ShowViewModel(
                 new MvxViewModelRequest<TViewModel>(new MvxBundle(), new MvxBundle(), MvxRequestedBy.UserAction),
                 TabNavigationType.NewBackgroundTab);
        }

        /// <summary>
        /// Switches to a given tab
        /// </summary>
        /// <param name="tab">Tab to switch to</param>
        /// <returns>Was the switch successful?</returns>
        public bool SwitchToTab(ITabInfo tab)
            => _tabProvider.SwitchToTab(tab);

        /// <summary>
        /// Gets or sets the title of the view model's tab if on top
        /// </summary>
        public string TabTitle
        {
            get { return _tabProvider.GetTabForViewModel(this)?.Title; }
            set
            {
                var tab = _tabProvider.GetTabForViewModel(this);
                if (tab != null)
                {
                    tab.Title = value;
                }
            }
        }

        /// <summary>
        /// Override this method to do any "on navigated from" work or to stop the navigation altogether.
        /// </summary>
        public async void GoBack()
        {
            if (await CanCloseViewModelAsync())
            {
                ForceClose(this);
            }
        }

        /// <summary>
        /// Checks if the view model can be closed
        /// </summary>
        /// <returns>Can close view model?</returns>
        public virtual Task<bool> CanCloseViewModelAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Forces close of a given view model
        /// </summary>
        /// <param name="viewModel">View model</param>
        public void ForceClose(IMvxViewModel viewModel)
        {
            var provider = Mvx.Resolve<ITabProvider>();
            var tab = provider.GetTabForViewModel(this);
            if (tab != null)
            {
                ChangePresentation(new GoBackPresentationHint(tab));
            }
        }

        /// <summary>
        /// Closes the tab that contains this viewmodel.
        /// </summary>
        public void CloseSelf()
        {
            var provider = Mvx.Resolve<ITabProvider>();
            var tab = provider.GetTabForViewModel(this);
            if (tab != null)
            {
                provider.CloseTab(tab);
            }
        }
    }
}
