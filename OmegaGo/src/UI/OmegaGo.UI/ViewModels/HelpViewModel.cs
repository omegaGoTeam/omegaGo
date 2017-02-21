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
        /// <summary>
        /// List of all help items
        /// </summary>
        private readonly List<HelpPage> _helpItems;

        /// <summary>
        /// Stores the selected help item
        /// </summary>
        private HelpPage _selectedHelpItem;
        
        /// <summary>
        /// Creates the View Model
        /// </summary>
        public HelpViewModel()
        {
            _helpItems = HelpPage.CreateAllHelpPages();
            SelectedHelpItem = _helpItems[0];
        }        

        /// <summary>
        /// Enumerates all help items
        /// </summary>
        public IEnumerable<HelpPage> HelpItems => _helpItems;

        /// <summary>
        /// Currently selected help item
        /// </summary>
        public HelpPage SelectedHelpItem
        {
            get
            {
                return _selectedHelpItem;
            }
            set
            {
                SetProperty(ref _selectedHelpItem, value);
            }
        }
    }
}
