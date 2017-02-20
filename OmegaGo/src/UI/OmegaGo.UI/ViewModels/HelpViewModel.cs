using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Help;
using System.Collections.ObjectModel;
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
        private ObservableCollection<HelpPage> _helpItems;
        private HelpPage _selectedHelpItem;

        /// <summary>
        /// Occurs when the contents of the WebView control displaying the current help page should be
        /// updated.
        /// </summary>
        public event EventHandler<string> WebViewContentChanged;

        public HelpViewModel()
        {
            _helpItems = new ObservableCollection<HelpPage>(); 

            foreach (var helpPage in HelpPage.CreateAllHelpPages())
            {
                _helpItems.Add(helpPage);
            }
            
            SelectedHelpItem = _helpItems[0];
        }

        public HelpPage SelectedHelpItem
        {
            get { return _selectedHelpItem; }
            set
            {
                SetProperty(ref _selectedHelpItem, value);
                NavigateToCurrentItem();
            }
        }

        public ObservableCollection<HelpPage> HelpItems => _helpItems;

        public void NavigateToCurrentItem()
        {
            WebViewContentChanged?.Invoke(this, _selectedHelpItem.Content);
        }
    }
}
