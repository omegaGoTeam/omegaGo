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

        /// <summary>
        /// Stores the selected help item
        /// </summary>
        private HelpPageMenuItem _selectedHelpItem;
        
        /// <summary>
        /// Creates the View Model
        /// </summary>
        public HelpViewModel()
        {
            HelpItems = new ObservableCollection<HelpPageMenuItem>(HelpPage.CreateAllHelpPages()
                .Select((hp, id) => new HelpPageMenuItem(id + 1, hp)));
        }

        public void Init()
        {
            SelectedHelpItem = HelpItems[0];
        }

        /// <summary>
        /// Enumerates all help items
        /// </summary>
        public ObservableCollection<HelpPageMenuItem> HelpItems { get; }

        /// <summary>
        /// Currently selected help item
        /// </summary>
        public HelpPageMenuItem SelectedHelpItem
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
