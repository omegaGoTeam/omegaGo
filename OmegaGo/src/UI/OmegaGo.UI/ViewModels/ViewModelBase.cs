using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.ViewModels
{
    /// <summary>
    /// Base for all ViewModels in the app
    /// </summary>
    public class ViewModelBase : MvxViewModel
    {
        private IMvxCommand _goBackCommand;

        /// <summary>
        /// Provides back navigation
        /// </summary>
        public IMvxCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new MvxCommand(() => this.Close(this)));

        private Localizer _localizer = null;

        /// <summary>
        /// Localizer for the ViewModel
        /// </summary>
        public Localizer Localizer => _localizer ?? (_localizer = new Localizer());

        private bool _isWorking = false;

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
    }
}
