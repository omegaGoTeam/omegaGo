using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Help;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace OmegaGo.UI.ViewModels
{
    /// <summary>
    /// Handles the Help view which shows HTML documents to the user.
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.ViewModelBase" />
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HelpViewModel : ViewModelBase
    {
        private List<HelpPage> _helpItems;
        private HelpPage _selectedHelpItem;

        /// <summary>
        /// Occurs when the contents of the WebView control displaying the current help page should be
        /// updated.
        /// </summary>
        public event EventHandler<string> WebViewContentChanged;

        public HelpViewModel()
        {
            _helpItems  = HelpPage.CreateAllHelpPages();
            SelectedHelpItem = _helpItems[0];
        }

        public HelpPage SelectedHelpItem
        {
            get { return _selectedHelpItem; }
            set {
                SetProperty(ref _selectedHelpItem, value);
                NavigateToCurrentItem();
            }
        }

        public IEnumerable<HelpPage> HelpItems => this._helpItems;

        public void NavigateToCurrentItem()
        {
            WebViewContentChanged?.Invoke(this, _selectedHelpItem.Content);
        }
    }
}
